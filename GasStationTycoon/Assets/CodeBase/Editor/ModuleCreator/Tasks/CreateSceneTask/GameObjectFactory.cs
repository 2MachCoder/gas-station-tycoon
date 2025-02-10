using System;
using CodeBase.Implementation.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Editor.ModuleCreator.Tasks.CreateSceneTask
{
    public static class GameObjectFactory
    {
        public static GameObject CreateCanvas()
        {
            GameObject canvas = new GameObject("Canvas");
            Undo.RegisterCreatedObjectUndo(canvas, "Create Canvas");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.AddComponent<ScreenCanvas>();
            return canvas;
        }

        public static GameObject InstantiateViewPrefab(string prefabPath, GameObject parent)
        {
            GameObject viewPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (viewPrefab == null)
            {
                Debug.LogError($"View prefab not found at {prefabPath}");
                return null;
            }

            GameObject viewInstance = PrefabUtility.InstantiatePrefab(viewPrefab) as GameObject;
            if (viewInstance == null)
            {
                Debug.LogError($"Failed to instantiate View prefab at {prefabPath}");
                return null;
            }

            viewInstance.transform.SetParent(parent.transform, false);
            Undo.RegisterCreatedObjectUndo(viewInstance, "Instantiate View Prefab");
            return viewInstance;
        }

        public static GameObject InstantiateInstaller(string installerName, Type installerType)
        {
            if (installerType == null)
            {
                Debug.LogError("Installer type is null.");
                return null;
            }

            GameObject installerObject = new GameObject(installerName);
            Undo.RegisterCreatedObjectUndo(installerObject, "Instantiate Installer");
            installerObject.AddComponent(installerType);
            return installerObject;
        }

        public static Camera CreateModuleCamera()
        {
            GameObject cameraObject = new GameObject("ModuleCamera");
            Undo.RegisterCreatedObjectUndo(cameraObject, "Create Module Camera");
            Camera cameraComponent = cameraObject.AddComponent<Camera>();
            cameraComponent.clearFlags = CameraClearFlags.Skybox;
            cameraComponent.cullingMask = LayerMask.GetMask("Default");
            return cameraComponent;
        }
    }
}
