using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.CustomDIContainer
{
    /// <summary>
    /// The root container for dependency injection.
    /// It holds global (singleton and transient) registrations and manages scoped containers.
    /// </summary>
    public class RootDiContainer : BaseDiContainer
    {
        private readonly Stack<ScopedContainer> _scopes = new();
        private static readonly object Lock = new();
        private static RootDiContainer _instance;
        private const string RootDiContainerName = "RootDiContainer";
        private const string ScopedDiContainerName = "ScopedDiContainer";
        private static bool _isLoaded;
        
        /// <summary>
        /// Singleton instance accessor with double-checked locking.
        /// </summary>
        public static RootDiContainer Instance
        {
            get
            {
                if (_instance != null) 
                    return _instance;

                lock (Lock)
                {
                    if (_instance != null)
                        return _instance;

                    var existingInstance = FindObjectOfType<RootDiContainer>();
                    if (existingInstance != null)
                        _instance = existingInstance;
                    else
                        SetNewRootDiContainer();
                }

                return _instance;
            }
        }

        private static void SetNewRootDiContainer()
        {
            Debug.LogWarning("RootDiContainer instance not found. Creating new instance.");
            
            var containerGameObject = new GameObject(RootDiContainerName);
            _instance = containerGameObject.AddComponent<RootDiContainer>();
            DontDestroyOnLoad(containerGameObject);
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.LogWarning("Duplicate RootDiContainer detected. Destroying this instance.");
                Destroy(gameObject);
            }
        }
        
        private void Start() => InvokeStartables();
        

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateRootDiContainer()
        {
            if (_isLoaded) return;
            _isLoaded = true;
            
            var prefab = Resources.Load<GameObject>("RootDiContainer/DIContainerWithProjectInstaller");

            if (prefab == null)
            {
                Debug.LogError("RootDiContainer prefab not found in Resources!");
                return;
            }

            var instance = Instantiate(prefab);
            _instance = instance.GetComponent<RootDiContainer>();
            DontDestroyOnLoad(instance);
        }
        
        public ScopedContainer BeginScope()
        {
            GameObject scopeObject = new GameObject(ScopedDiContainerName);
            ScopedContainer scope = scopeObject.AddComponent<ScopedContainer>();

            scope.UpdateDependencies();
            _scopes.Push(scope);
    
            return scope;
        }
        
        public void EndScope()
        {
            if (_scopes.Count > 0)
            {
                ScopedContainer scope = _scopes.Pop();
                scope.Dispose();
                Destroy(scope.gameObject); // Cleanup
            }
        }
        
        /// <summary>
        /// Attempts to resolve a service by first checking all active scopes
        /// and then falling back to the global (root) registrations.
        /// </summary>
        public override object Resolve(Type serviceType)
        {
            // Iterate through active scopes in reverse (most local scope first)
            foreach (var scope in _scopes.Reverse()) // LIFO order (last created scope is checked first)
            {
                try
                {
                    // Attempt to resolve the service in the current scope.
                    return scope.Resolve(serviceType); 
                }
                catch (Exception) 
                {
                    // Continue checking the next scope if the current one fails
                }
            }

            // If not found in any scope, check the global container
            return base.Resolve(serviceType);
        }
        
        /// <summary>
        /// Invokes the Start() method on all unique objects that implement IStartable.
        /// </summary>
        private void InvokeStartables()
        {
            HashSet<object> processedInstances = new(); // // Tracks unique instances to prevent duplicate Start() calls

            foreach (var instance in GetRegisteredSingletons().Values)
            {
                if (instance is IStartable startable && processedInstances.Add(startable)) 
                    startable.Start();
            }
        }
    }
}