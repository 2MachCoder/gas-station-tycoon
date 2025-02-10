using System;
using CodeBase.Core.UI;
using CodeBase.Editor.ModuleCreator.Base;
using CodeBase.Editor.ModuleCreator.Tasks.AddScriptsTask;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CodeBase.Editor.ModuleCreator.Tasks.CreateSceneTask
{
    [Serializable]
    public class CreateSceneTask : Task
    {
        [JsonProperty] private string _moduleName;
        [JsonProperty] private string _targetModuleFolderPath;

        public CreateSceneTask(string moduleName, string targetModuleFolderPath)
        {
            _moduleName = moduleName;
            _targetModuleFolderPath = targetModuleFolderPath;
            WaitForCompilation = true;
        }

        public override void Execute()
        {
            string sceneFolderPath = PathManager.CombinePaths(_targetModuleFolderPath, "Scene");
            ModuleGenerator.EnsureTargetFolderExists(sceneFolderPath);
            string scenePath = PathManager.CombinePaths(sceneFolderPath, $"{_moduleName}.unity");
            CreateNewScene(scenePath);

            GameObject canvas = GameObjectFactory.CreateCanvas();
            if (canvas == null)
            {
                Debug.LogError("Failed to create Canvas.");
                return;
            }

            string viewPrefabPath = PathManager.CombinePaths(_targetModuleFolderPath, 
                "Views", $"{_moduleName}View.prefab");
            GameObject viewInstance = GameObjectFactory.InstantiateViewPrefab(viewPrefabPath, canvas);
            if (viewInstance == null)
            {
                Debug.LogError("Failed to instantiate View prefab.");
                return;
            }

            string installerName = $"{_moduleName}Installer";
            string folderType = PathManager.GetFolderType(_targetModuleFolderPath);
            string installerFullName = $"Modules.{folderType}.{_moduleName}Screen.Scripts.{installerName}";
            Type installerType = ReflectionHelper.FindType(installerFullName);
            if (installerType == null)
            {
                Debug.LogError($"Installer type '{installerName}' not found.");
                return;
            }

            GameObject installerObject = GameObjectFactory.InstantiateInstaller(installerName, installerType);
            if (installerObject == null)
            {
                Debug.LogError("Failed to instantiate Installer.");
                return;
            }

            Camera camera = GameObjectFactory.CreateModuleCamera();
            if (camera == null)
            {
                Debug.LogError("Failed to create Module Camera.");
                return;
            }

            AssignInstallerFields(installerObject, viewInstance, canvas, camera);
            AssignScreensCanvasFields(canvas, camera);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        private void CreateNewScene(string scenePath)
        {
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (newScene.IsValid())
            {
                EditorSceneManager.SaveScene(newScene, scenePath);
                SceneManager.SetActiveScene(newScene);
                Debug.Log($"New scene created at: {scenePath}");
            }
            else
                Debug.LogError("Failed to create a new scene.");
        }

        private void AssignInstallerFields(GameObject installerObject, GameObject viewInstance,
            GameObject canvas, Camera camera)
        {
            Component installerComponent = ReflectionHelper.
                GetComponentByName(installerObject, installerObject.name);
            if (installerComponent == null)
                return;

            string fieldPrefix = char.ToLower(_moduleName[0]) + _moduleName.Substring(1);
            string viewFieldName = $"{fieldPrefix}ScreenView";
            Component viewComponent = viewInstance.GetComponent($"{_moduleName}ScreenView");
            if (viewComponent != null)
                ReflectionHelper.SetPrivateField(installerComponent, viewFieldName, viewComponent);
            else
                Debug.LogError($"View component '{_moduleName}ScreenView' not found on View prefab.");

            var screenCanvas = canvas.GetComponent<BaseScreenCanvas>();
            if (screenCanvas != null)
                ReflectionHelper.SetPrivateField(installerComponent, "screenCanvas", screenCanvas);
            else
                Debug.LogError("ScreenCanvas component not found on Canvas.");

            if (camera != null)
                ReflectionHelper.SetPrivateField(installerComponent, "mainCamera", camera);
            else
                Debug.LogError("Main Camera is null.");
        }

        private void AssignScreensCanvasFields(GameObject canvas, Camera camera)
        {
            var screenCanvas = canvas.GetComponent<BaseScreenCanvas>();
            if (screenCanvas == null)
            {
                Debug.LogError("ScreenCanvas component not found on Canvas.");
                return;
            }

            CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
            if (canvasScaler != null)
                ReflectionHelper.SetPrivateField(screenCanvas, "canvasScaler", canvasScaler);
            else
                Debug.LogError("CanvasScaler component not found on Canvas.");

            if (camera != null)
                ReflectionHelper.SetPrivateField(screenCanvas, "uiCamera", camera);
            else
                Debug.LogError("UI Camera is null.");
        }
    }
}