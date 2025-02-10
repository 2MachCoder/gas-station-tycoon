using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CodeBase.CustomDIContainer
{
    /// <summary>
    /// Base class for DI containers. Provides methods for registering and resolving services.
    /// </summary>
    public abstract class BaseDiContainer : MonoBehaviour, IObjectResolver, IDisposable
    {
        // Dictionaries to hold singleton, transient and factory registrations.
        private readonly Dictionary<Type, Type> _lazySingletons = new();
        private readonly Dictionary<Type, object> _singletons = new();
        private readonly Dictionary<Type, Type> _transients = new();
        private readonly Dictionary<Type, object> _factories = new();


        // TODO add Dep DTO (remove Get Methods)
        protected Dictionary<Type, Type> GetRegisteredLazySingletons() => new(_lazySingletons);
        protected Dictionary<Type, object> GetRegisteredSingletons() => new(_singletons);
        protected Dictionary<Type, Type> GetRegisteredTransients() => new(_transients);
        protected Dictionary<Type, object> GetFactories() => new(_factories);

        public void UpdateDependencies()
        {
            var rootDiContainer = RootDiContainer.Instance;
            if (rootDiContainer == this)
                return;
            
            // TODO add Dep DTO (remove Set Methods)
            foreach (var entry in rootDiContainer.GetRegisteredLazySingletons()) 
                _lazySingletons[entry.Key] = entry.Value;
            foreach (var entry in rootDiContainer.GetRegisteredSingletons()) 
                _singletons[entry.Key] = entry.Value;
            foreach (var entry in rootDiContainer.GetRegisteredTransients()) 
                _transients[entry.Key] = entry.Value;
            foreach (var entry in rootDiContainer.GetFactories()) 
                _factories[entry.Key] = entry.Value;
        }
        
        public void RegisterSingleton<TService, TImplementation>() where TImplementation : TService
        {
            if (!typeof(TService).IsAssignableFrom(typeof(TImplementation)))
                throw new InvalidOperationException($"{typeof(TImplementation)} does not implement {typeof(TService)}!");
            
            var instance = (TService)CreateInstance(typeof(TImplementation));
            _singletons[typeof(TService)] = instance;
        }
        
        public void RegisterSingleton<TService>() where TService : class
        {
            var instance = (TService)CreateInstance(typeof(TService));
            _singletons[typeof(TService)] = instance;
        }

        public void RegisterSingleton<TService>(TService instance) where TService : class => 
            _singletons[typeof(TService)] = instance;
        
        public void RegisterLazySingleton<TService, TImplementation>()
            where TImplementation : TService
        {
            if (!typeof(TService).IsAssignableFrom(typeof(TImplementation)))
                throw new InvalidOperationException($"{typeof(TImplementation)} does not implement {typeof(TService)}!");

            _lazySingletons[typeof(TService)] = typeof(TImplementation);
        }
        
        public void RegisterLazySingleton<TService>() where TService : class =>
            _lazySingletons[typeof(TService)] = typeof(TService);

        public void RegisterSingletonAsImplementedInterfaces<TImplementation>(TImplementation instance) 
            where TImplementation : class
        {
            var implementationType = instance.GetType();
            var interfaces = implementationType.GetInterfaces();
    
            foreach (var interfaceType in interfaces) 
                _singletons[interfaceType] = instance;
        }

        public void RegisterTransient<TService, TImplementation>() where TImplementation : TService
        {
            if (!typeof(TService).IsAssignableFrom(typeof(TImplementation)))
                throw new InvalidOperationException($"{typeof(TImplementation)} does not implement {typeof(TService)}!");
            _transients[typeof(TService)] = typeof(TImplementation);
        }
        
        public void RegisterFactory<TImplementation>(TImplementation factory) where TImplementation : class
        {
            Type implementationType = factory.GetType();
            Type[] interfaces = implementationType.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                _factories[interfaceType] = factory;
                Debug.Log($"Factory registered for {interfaceType.Name}");
            }
        }
        
        public TService Resolve<TService>() => 
            (TService)Resolve(typeof(TService));

        /// <summary>
        /// Resolves a service by checking singleton, factory and transient registrations.
        /// Throws an exception if the service is not found.
        /// </summary>
        public virtual object Resolve(Type serviceType)
        {
            if (_singletons.TryGetValue(serviceType, out var singletonInstance))
                return singletonInstance;

            if (_factories.TryGetValue(serviceType, out var factoryInstance))
            {
                Debug.Log($"Resolving {serviceType.Name} via factory.");
                return factoryInstance;
            }
            
            if (_transients.TryGetValue(serviceType, out var implementationType))
                return CreateInstance(implementationType);
            
            if (_lazySingletons.TryGetValue(serviceType, out var lazyType))
            {
                object createdInstance = CreateInstance(lazyType);
                _singletons[serviceType] = createdInstance;
                _lazySingletons.Remove(serviceType);
                return createdInstance;
            }

            // Debug.LogError($"Service of type {serviceType.Name} is not registered in the DI container!");
            throw new Exception($"Service of type {serviceType} is not registered.");
        }

        /// <summary>
        /// Resolves all services registered that can be assigned to TService.
        /// </summary>
        public List<TService> ResolveAll<TService>()
        {
            var results = new List<TService>();

            // 1. Find all singletons that implement TService.
            results.AddRange(_singletons.Values.OfType<TService>());

            // 2. Find factories registered as Func<TService> (if any) and invoke them.
            results.AddRange(
                _factories.Values
                    .OfType<Func<TService>>() // Если в `Factories` хранится делегат-фабрика
                    .Select(factory => factory.Invoke()) // Вызываем фабрики
            );

            // 3. For transients, create new instances.
            results.AddRange(
                _transients
                    .Where(pair => typeof(TService).IsAssignableFrom(pair.Key))
                    .Select(pair => (TService)CreateInstance(pair.Value))
            );

            return results;
        }
        
        public void Unregister<T>()
        {
            _singletons.Remove(typeof(T));
            _transients.Remove(typeof(T));
        }

        public void InjectMonoBehaviour(MonoBehaviour monoBehaviour) => 
            Inject(monoBehaviour);

        /// <summary>
        /// Uses reflection to inject dependencies into fields and properties marked with [InjectAttribute].
        /// </summary>
        public void Inject(object instance)
        {
            Type type = instance.GetType();
    
            // Inject into fields
            foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                         .Where(f => f.GetCustomAttribute<InjectAttribute>() != null)) 
                field.SetValue(instance, Resolve(field.FieldType));

            // Inject into properties
            foreach (var property in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => p.GetCustomAttribute<InjectAttribute>() != null)) 
                property.SetValue(instance, Resolve(property.PropertyType));
        }

        /// <summary>
        /// Creates an instance of the specified type using the constructor with the most parameters.
        /// Recursively resolves all constructor dependencies.
        /// </summary>
        private object CreateInstance(Type type)
        {
            // Choose the constructor with the most parameters.
            ConstructorInfo constructor = SelectConstructorWithMostParameters(type);
            if (constructor == null)
                throw new Exception($"No public constructors found for {type}");

            // Resolve constructor parameters.
            object[] parameterInstances = ResolveConstructorParameters(constructor);

            // Invoke constructor and create instance.
            object instance;
            try { instance = constructor.Invoke(parameterInstances); }
            catch (Exception ex)
            { throw new Exception($"Error invoking constructor of {type}: {ex.Message}", ex); }

            // Inject dependencies into the created instance.
            try { Inject(instance); }
            catch (Exception ex)
            { throw new Exception($"Error injecting dependencies into instance of {type}: {ex.Message}", ex); }

            return instance;
        }
        
        private static ConstructorInfo SelectConstructorWithMostParameters(Type type)
        {
            // Get all public constructors.
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length == 0)
                return null;

            // Select the constructor with the most parameters.
            ConstructorInfo bestConstructor = constructors[0];
            int maxParams = bestConstructor.GetParameters().Length;
            for (int i = 1; i < constructors.Length; i++)
            {
                int currentParamCount = constructors[i].GetParameters().Length;
                if (currentParamCount > maxParams)
                {
                    bestConstructor = constructors[i];
                    maxParams = currentParamCount;
                }
            }
            return bestConstructor;
        }
        
        private object[] ResolveConstructorParameters(ConstructorInfo constructor)
        {
            // Get all parameters for the constructor.
            ParameterInfo[] paramInfos = constructor.GetParameters();
            object[] parameterInstances = new object[paramInfos.Length];

            // Resolve each parameter individually.
            for (int i = 0; i < paramInfos.Length; i++)
            {
                try { parameterInstances[i] = Resolve(paramInfos[i].ParameterType); }
                catch (Exception ex)
                {
                    var typeName = paramInfos[i].ParameterType.Name.Split(".").Last();
                    throw new Exception($"Failed to resolve parameter '{paramInfos[i].Name}' of type" +
                                        $" '{typeName}' for constructor of '{constructor.DeclaringType}'.", ex);
                }
            }
            return parameterInstances;
        }

        public void Dispose()
        {
            _lazySingletons.Clear();
            _singletons.Clear();
            _transients.Clear();
            _factories.Clear();
        }
    }
}
