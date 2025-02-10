using System;
using System.Reflection;
using CodeBase.Editor.ModuleCreator.Base;
using CodeBase.Editor.ModuleCreator.Tasks.AddScriptsTask;
using Modules.Template.TemplateScreen.Scripts;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CodeBase.Editor.ModuleCreator.Tasks.AddPrefabTask
{
    public static class PrefabHelper
    {
        public static string CopyTemplatePrefab(string moduleName, string targetModuleFolderPath)
        {
            string targetPrefabFolderPath = PathManager.CombinePaths(targetModuleFolderPath, "Views");
            ModuleGenerator.EnsureTargetFolderExists(targetPrefabFolderPath);

            string templateViewPrefabPath = PathManager.TemplateViewPrefabPath;
            string targetPrefabPath = PathManager.CombinePaths(targetPrefabFolderPath,
                $"{moduleName}View.prefab");
            
            bool copyResult = AssetDatabase.CopyAsset(templateViewPrefabPath, targetPrefabPath);
            if (!copyResult)
            {
                Debug.LogError($"Failed to copy prefab from '{templateViewPrefabPath}'" +
                               $" to '{targetPrefabPath}'.");
                return null;
            }
            AssetDatabase.ImportAsset(targetPrefabPath, ImportAssetOptions.ForceUpdate);

            return targetPrefabPath;
        }

        public static GameObject LoadPrefab(string prefabPath)
        {
            GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefab == null) 
                Debug.LogError($"Failed to load prefab contents at {prefabPath}");
            return prefab;
        }

        public static void ReplaceScriptProperty(GameObject prefabContents, string moduleName, string folderType)
        {
            string fullClassName = $"Modules.{folderType}.{moduleName}Screen.Scripts.{moduleName}ScreenView";
            MonoScript newMonoScript = FindMonoScript(fullClassName);
            if (newMonoScript == null)
            {
                Debug.LogError($"MonoScript for class '{fullClassName}' " +
                               $"not found. Ensure the script is compiled.");
                return;
            }

            TemplateScreenView templateViewComponent = prefabContents.GetComponent<TemplateScreenView>();
            if (templateViewComponent == null)
            {
                Debug.LogError("TemplateScreenView component not found in prefab.");
                return;
            }

            SerializedObject serializedObject = new SerializedObject(templateViewComponent);
            SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");

            if (scriptProperty != null)
            {
                scriptProperty.objectReferenceValue = newMonoScript;
                serializedObject.ApplyModifiedProperties();
                Debug.Log($"Replaced script on TemplateScreenView with {fullClassName}.");
            }
            else
            {
                Debug.LogError("Failed to find 'm_Script' property.");
            }
        }

        public static MonoScript FindMonoScript(string fullClassName)
        {
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (monoScript != null && monoScript.GetClass() != null)
                {
                    if (monoScript.GetClass().FullName == fullClassName)
                        return monoScript;
                }
            }
            Debug.LogError($"MonoScript for class '{fullClassName}' not found.");
            return null;
        }

        public static Type GetViewType(MonoScript monoScript) => monoScript.GetClass();

        public static Component GetViewComponent(GameObject prefabContents, Type viewType)
        {
            Component component = prefabContents.GetComponent(viewType);
            if (component == null) 
                Debug.LogError($"{viewType.Name} component not found in prefab.");
            return component;
        }

        public static void AssignTemplateScreenTitle(GameObject prefabContents,
            string moduleName, Component newViewComponent, Type viewType)
        {
            string fieldName = $"{char.ToLower(moduleName[0])}{moduleName.Substring(1)}ScreenTitle";
            Transform titleTransform = prefabContents.transform.Find("Title");
            if (titleTransform == null)
            {
                Debug.LogError("Title GameObject not found in prefab.");
                return;
            }

            TMP_Text titleText = titleTransform.GetComponent<TMP_Text>();
            if (titleText == null)
            {
                Debug.LogError("TMP_Text component not found on Title GameObject.");
                return;
            }

            FieldInfo fieldInfo = viewType.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
                fieldInfo.SetValue(newViewComponent, titleText);
            else
                Debug.LogError($"Field '{fieldName}' not found in '{viewType.Name}'.");
        }

        public static void InvokeSetTitle(Component newViewComponent, string moduleName, Type viewType)
        {
            MethodInfo setTitleMethod = viewType.GetMethod("SetTitle",
                BindingFlags.Public | BindingFlags.Instance);
            if (setTitleMethod != null)
                setTitleMethod.Invoke(newViewComponent, new object[] { moduleName });
            else
                Debug.LogError($"SetTitle method not found in '{viewType.Name}'.");
        }

        public static void SaveAndUnloadPrefab(GameObject prefabContents, string prefabPath, string moduleName)
        {
            prefabContents.name = $"{moduleName}View";
            PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabContents);
        }

        public static void LogTitleSet(string moduleName, string viewTypeName) => 
            Debug.Log($"Set title to '{moduleName}' in {viewTypeName}.");

        public static void LogComponentNotFound(string viewTypeName) => 
            Debug.LogError($"{viewTypeName} component not found in prefab.");

        public static void LogPrefabCreated(string moduleName) => 
            Debug.Log($"Prefab for module {moduleName} created successfully.");
    }
}