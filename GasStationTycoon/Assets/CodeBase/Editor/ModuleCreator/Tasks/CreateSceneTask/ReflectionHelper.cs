using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CodeBase.Editor.ModuleCreator.Tasks.CreateSceneTask
{
    public static class ReflectionHelper
    {
        private static readonly Dictionary<string, Type> TypeCache = new();

        public static Type FindType(string fullName)
        {
            if (TypeCache.TryGetValue(fullName, out Type cachedType))
                return cachedType;

            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == fullName);

            if (type != null)
                TypeCache[fullName] = type;
            else
                Debug.LogError($"Type '{fullName}' not found.");

            return type;
        }

        public static void SetPrivateField<T>(object obj, string fieldName, T value)
        {
            if (obj == null)
            {
                Debug.LogError("Object is null when trying to set private field.");
                return;
            }

            FieldInfo field = obj.GetType().GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                field.SetValue(obj, value);
            else
                Debug.LogError($"Field '{fieldName}' not found in {obj.GetType().Name}.");
        }

        public static Component GetComponentByName(GameObject gameObject, string componentName)
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject is null when trying to get component.");
                return null;
            }

            Component component = gameObject.GetComponent(componentName);
            if (component == null)
                Debug.LogError($"Component '{componentName}' not found on {gameObject.name}.");

            return component;
        }
    }
}