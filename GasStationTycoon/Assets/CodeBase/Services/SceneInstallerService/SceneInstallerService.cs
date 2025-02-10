using System.Collections.Generic;
using CodeBase.CustomDIContainer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Services.SceneInstallerService
{
    public class SceneInstallerService
    {
        [Inject] private SceneService _sceneService;
        private List<ISceneInstaller> _currentScenesInstallers = new();

        public SceneInstallerService() => RootDiContainer.Instance.Inject(this);

        public ScopedContainer CombineScenes(RootDiContainer parentScope, bool removeObjectsToDelete)
        {
            _currentScenesInstallers = FindActiveModulesSceneInstallers();

            if (removeObjectsToDelete)
                RemoveObjectsToDelete();

            // properly create a scoped container
            var sceneScope = parentScope.BeginScope();

            // Get the internal RootDiContainer from ScopedContainer
            //var scopedDiContainer = sceneScope.GetContainer();

            // Register scene dependencies into the scoped container
            foreach (var installer in _currentScenesInstallers) 
                installer.RegisterSceneDependencies(sceneScope);

            // Inject dependencies into scene objects (ScopedContainer implements IObjectResolver)
            foreach (var installer in _currentScenesInstallers) 
                installer.InjectMonoBehaviours(sceneScope);

            return sceneScope;
        }

        private void RemoveObjectsToDelete()
        {
            foreach (var installer in _currentScenesInstallers) 
                installer.RemoveObjectsToDelete();
        }

        private List<ISceneInstaller> FindAllSceneInstallers()
        {
            int sceneCount = SceneManager.sceneCount;
            Scene[] activeScenes = new Scene[sceneCount];

            for (int i = 0; i < sceneCount; i++) 
                activeScenes[i] = SceneManager.GetSceneAt(i);

            return FindSceneInstallersInScenes(activeScenes);
        }

        private List<ISceneInstaller> FindActiveModulesSceneInstallers()
        {
            Scene[] activeScenes = _sceneService.GetActiveModulesScenes();
            if (activeScenes.Length == 0)
                return FindAllSceneInstallers();
            return FindSceneInstallersInScenes(activeScenes);
        }

        private List<ISceneInstaller> FindSceneInstallersInScenes(Scene[] scenes)
        {
            var sceneInstallers = new List<ISceneInstaller>();
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].isLoaded)
                {
                    // Infrastructure GameObjects are the top-level objects in the scene hierarchy (direct children of the scene itself).
                    GameObject[] rootObjects = scenes[i].GetRootGameObjects();
                    foreach (GameObject rootObject in rootObjects)
                    {
                        ISceneInstaller[] installersInRoot = rootObject.
                            GetComponentsInChildren<ISceneInstaller>(true);
                        sceneInstallers.AddRange(installersInRoot);
                    }
                }
            }

            return sceneInstallers;
        }
    }
}