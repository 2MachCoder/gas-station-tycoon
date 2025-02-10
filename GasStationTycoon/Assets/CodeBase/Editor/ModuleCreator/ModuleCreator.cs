using CodeBase.Editor.ModuleCreator.Tasks;
using CodeBase.Editor.ModuleCreator.Tasks.AddPrefabTask;
using CodeBase.Editor.ModuleCreator.Tasks.AddScriptsTask;
using CodeBase.Editor.ModuleCreator.Tasks.CreateSceneTask;
using UnityEditor;
using UnityEngine;

namespace CodeBase.Editor.ModuleCreator
{
    public class ModuleCreator : EditorWindow
    {
        private FolderType _selectedFolder = FolderType.Base;
        private bool _createPrefab = true;
        private bool _createScene = true;

        public enum FolderType { Additional, Base, Test }
        private const float GUISpacing = 10f;
        private string _moduleName = "NewModule";
        private bool _createInstaller = true;
        private bool _createPresenter = true;
        private bool _createView = true;
        private bool _createModel = true;
        private bool _createAsmdef = true;

        [MenuItem("Tools/Create Module")]
        public static void ShowWindow() => GetWindow<ModuleCreator>("Create Module");

        private static bool IsValidModuleName(string moduleName) =>
            !string.IsNullOrWhiteSpace(moduleName) && !moduleName.Contains(" ");

        private void OnGUI()
        {
            GUILayout.Label("Module Creator", EditorStyles.boldLabel);
            _moduleName = EditorGUILayout.TextField("Module Name", _moduleName);
            GUILayout.Space(GUISpacing);
            _selectedFolder = (FolderType)EditorGUILayout.EnumPopup("Select Folder", _selectedFolder);
            GUILayout.Space(GUISpacing);
            GUILayout.Label("Select Scripts to Create", EditorStyles.boldLabel);
            _createInstaller = EditorGUILayout.Toggle("Installer", _createInstaller);
            _createPresenter = EditorGUILayout.Toggle("Presenter", _createPresenter);
            _createView = EditorGUILayout.Toggle("View", _createView);
            _createModel = EditorGUILayout.Toggle("Model", _createModel);
            GUILayout.Space(GUISpacing);
            _createAsmdef = EditorGUILayout.Toggle("Create asmdef", _createAsmdef);
            GUILayout.Space(GUISpacing);
            _createPrefab = EditorGUILayout.Toggle("Create Prefab", _createPrefab);
            _createScene = EditorGUILayout.Toggle("Create Scene", _createScene);
            GUILayout.Space(GUISpacing);
            if (GUILayout.Button("Create Module"))
            {
                if (IsValidModuleName(_moduleName))
                    CreateModule();
                else
                {
                    EditorUtility.DisplayDialog("Invalid Name",
                        "Module name cannot be empty or contain spaces.", "OK");
                }
            }
        }

        private void CreateModule()
        {
            Debug.Log("Creating module with name: " + _moduleName);

            var addScriptsTask = new AddScriptsTask(
                _moduleName,
                _selectedFolder.ToString(),
                _createInstaller,
                _createPresenter,
                _createView,
                _createModel,
                _createAsmdef);

            TaskQueue.EnqueueTask(addScriptsTask);

            string targetModuleFolderPath = ModuleGenerator.
                GetTargetModuleFolderPath(_moduleName, _selectedFolder.ToString());

            if (_createPrefab)
            {
                var addPrefabTask = new AddPrefabTask(
                    _moduleName,
                    targetModuleFolderPath);

                TaskQueue.EnqueueTask(addPrefabTask);
            }

            if (_createScene)
            {
                var createSceneTask = new CreateSceneTask(
                    _moduleName,
                    targetModuleFolderPath);

                TaskQueue.EnqueueTask(createSceneTask);
            }

            EditorUtility.DisplayDialog("Module Creation Started",
                "Module creation process has started. Please wait for it to complete.", "OK");

            _moduleName = "NewModule";
        }
    }
}