using System;
using CodeBase.Editor.ModuleCreator.Base;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace CodeBase.Editor.ModuleCreator.Tasks.AddScriptsTask
{
    [Serializable]
    public class AddScriptsTask : Task
    {
        [JsonProperty] private string _moduleName;
        [JsonProperty] private string _selectedFolder;
        [JsonProperty] private bool _createInstaller;
        [JsonProperty] private bool _createPresenter;
        [JsonProperty] private bool _createView;
        [JsonProperty] private bool _createModel;
        [JsonProperty] private bool _createAsmdef;

        public AddScriptsTask(string moduleName, string selectedFolder, bool createInstaller,
            bool createPresenter, bool createView, bool createModel, bool createAsmdef)
        {
            _moduleName = moduleName;
            _selectedFolder = selectedFolder;
            _createInstaller = createInstaller;
            _createPresenter = createPresenter;
            _createView = createView;
            _createModel = createModel;
            _createAsmdef = createAsmdef;
            WaitForCompilation = true;
        }

        public override void Execute()
        {
            PathManager.InitializePaths();

            if (TemplateValidator.AreTemplatesAvailable(_createAsmdef))
            {
                ModuleGenerator.CreateModuleFiles(
                    _moduleName,
                    _selectedFolder,
                    _createInstaller,
                    _createPresenter,
                    _createView,
                    _createModel,
                    _createAsmdef);

                AssetDatabase.Refresh();
                Debug.Log($"Module {_moduleName} scripts created successfully.");
            }
            else
                Debug.LogError("Templates are not available.");
        }
    }
}