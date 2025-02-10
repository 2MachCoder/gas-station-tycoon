using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CodeBase.Editor.ModuleCreator.Base;
using UnityEditor;
using UnityEngine;

namespace CodeBase.Editor.ModuleCreator.Tasks.AddScriptsTask
{
    public static class ModuleGenerator
    {
        public static string TargetModuleFolderPath { get; private set; }

        public static void CreateModuleFiles(
            string moduleName,
            string selectedFolder,
            bool createInstaller,
            bool createPresenter,
            bool createView,
            bool createModel,
            bool createAsmdef)
        {
            string selectedFolderPath = GetSelectedFolderPath(selectedFolder);
            string targetFolderPath = PathManager.CombinePaths(selectedFolderPath, $"{moduleName}Screen");
            TargetModuleFolderPath = targetFolderPath;
            EnsureModuleFolders(targetFolderPath);

            string scriptsFolderPath = PathManager.CombinePaths(targetFolderPath, "Scripts");

            if (createAsmdef)
                CreateAsmdefFile(targetFolderPath, moduleName);

            CreateSelectedScripts(
                scriptsFolderPath,
                moduleName,
                selectedFolder,
                createInstaller,
                createPresenter,
                createView,
                createModel);
        }

        public static string GetTargetModuleFolderPath(string moduleName, string selectedFolder)
        {
            string selectedFolderPath = GetSelectedFolderPath(selectedFolder);
            return PathManager.CombinePaths(selectedFolderPath, $"{moduleName}Screen");
        }

        private static string GetSelectedFolderPath(string selectedFolder) =>
            selectedFolder switch
            {
                "Additional" => PathManager.AdditionalFolderPath,
                "Base" => PathManager.BaseFolderPath,
                "Test" => PathManager.TestFolderPath,
                _ => PathManager.BaseFolderPath
            };

        private static void EnsureModuleFolders(string targetFolderPath)
        {
            EnsureTargetFolderExists(targetFolderPath);
            EnsureTargetFolderExists(PathManager.CombinePaths(targetFolderPath, "Scripts"));
            EnsureTargetFolderExists(PathManager.CombinePaths(targetFolderPath, "Views"));
        }

        public static void EnsureTargetFolderExists(string targetFolderPath)
        {
            targetFolderPath = targetFolderPath.Replace("\\", "/");
            if (!AssetDatabase.IsValidFolder(targetFolderPath))
            {
                string parentFolder = Path.GetDirectoryName(targetFolderPath)?.Replace("\\", "/");
                string newFolderName = Path.GetFileName(targetFolderPath);
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
            }
        }

        private static void CreateAsmdefFile(string targetFolderPath, string moduleName)
        {
            string templateAsmdefPath = PathManager.
                CombinePaths(PathManager.TemplateModuleFolderPath, "TemplateScreen.asmdef");
            string targetAsmdefPath = PathManager.
                CombinePaths(targetFolderPath, $"{moduleName}Screen.asmdef");
            CopyAndAdjustAsmdef(templateAsmdefPath, targetAsmdefPath, moduleName);
        }

        private static void CreateSelectedScripts(
            string folderPath,
            string moduleName,
            string selectedFolder,
            bool createInstaller,
            bool createPresenter,
            bool createView,
            bool createModel)
        {
            var scriptsToCreate = new List<(bool shouldCreate, string templateFile, string outputFile)>
            {
                (createInstaller, "TemplateScreenInstaller.cs", $"{moduleName}ScreenInstaller.cs"),
                (createPresenter, "TemplateScreenPresenter.cs", $"{moduleName}ScreenPresenter.cs"),
                (createView, "TemplateScreenView.cs", $"{moduleName}ScreenView.cs"),
                (createModel, "TemplateScreenModel.cs", $"{moduleName}ScreenModel.cs"),
            };

            foreach (var (shouldCreate, templateFile, outputFile) in scriptsToCreate)
            {
                if (shouldCreate)
                {
                    string content = GetTemplateContent(templateFile, moduleName, selectedFolder);
                    CreateScript(folderPath, outputFile, content);
                }
            }
        }

        private static string GetTemplateContent(string templateFileName, string moduleName, string selectedFolder)
        {
            string templateFilePath = PathManager.CombinePaths(PathManager.TemplateScriptsFolderPath, templateFileName);
            string content = ReadTemplateFile(templateFilePath);
            if (content == null)
                return null;

            string moduleNameLower = char.ToLower(moduleName[0]) + moduleName.Substring(1);
            content = ReplaceNamespace(content, moduleName, selectedFolder);
            content = ReplaceTemplateOccurrences(content, moduleName, moduleNameLower);
            return content;
        }

        private static string ReadTemplateFile(string templateFilePath) =>
            File.Exists(templateFilePath) ? File.ReadAllText(templateFilePath) : null;

        private static string ReplaceNamespace(string content, string moduleName, string selectedFolder)
        {
            string namespaceReplacement = $"namespace Modules.{selectedFolder}.{moduleName}Screen.Scripts";
            return Regex.Replace(content, @"namespace\s+[\w\.]+", namespaceReplacement);
        }

        private static string ReplaceTemplateOccurrences(string content, string moduleName, string moduleNameLower)
        {
            return Regex.Replace(content, @"(_?)(template)", match =>
            {
                string prefix = match.Groups[1].Value;
                string templateWord = match.Groups[2].Value;
                return prefix + (char.IsUpper(templateWord[0]) ? moduleName : moduleNameLower);
            }, RegexOptions.IgnoreCase);
        }

        private static void CreateScript(string folderPath, string fileName, string scriptContent)
        {
            if (string.IsNullOrEmpty(scriptContent))
                return;

            string filePath = PathManager.CombinePaths(folderPath, fileName);

            WriteToFile(filePath, scriptContent);
        }

        private static void WriteToFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error writing file {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        private static void CopyAndAdjustAsmdef(string templateAsmdefPath, string targetAsmdefPath, string moduleName)
        {
            string content = ReadTemplateFile(templateAsmdefPath);
            if (content == null)
            {
                EditorUtility.DisplayDialog("Missing asmdef Template",
                    $"Template asmdef file not found at {templateAsmdefPath}.\n" +
                    $"\nCannot create asmdef file.", "OK");
                return;
            }
            content = AdjustAsmdefContent(content, moduleName);
            WriteToFile(targetAsmdefPath, content);
        }

        private static string AdjustAsmdefContent(string content, string moduleName) =>
            Regex.Replace(content, @"""name"":\s*""[^""]+""", $@"""name"": ""{moduleName}Screen""");
    }
}