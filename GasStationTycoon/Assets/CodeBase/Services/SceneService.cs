using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Services
{
    public enum AdditiveScenesMap
    {
        PopupsManager,
        DynamicBackground
    }
    
    public class SceneService
    {
        private readonly List<string> _staticModuleScenes = new();
        private List<string> _activeModuleScenes = new();
        private List<string> _loadedModuleScenes = new();
        private CancellationTokenSource _cts;
        
        public SceneService() // loading of scenes that have to exist during the whole project and adding the additive static scene in the constructor
        {
            AddStaticAdditiveScene(AdditiveScenesMap.PopupsManager);
            LoadStaticScenes(); //UniTask .Forget()
        }

        public Scene[] GetActiveModulesScenes()
        {
            //concat combines the two collections into a single sequence, preserving their order.
            return _activeModuleScenes.Concat(_staticModuleScenes) 
                .Select(SceneManager.GetSceneByName)
                .ToArray();
        }
        
        public async Task LoadStaticScenes() => await LoadScenesAsync(_staticModuleScenes);

        public void AddStaticAdditiveScene(AdditiveScenesMap sceneName) =>
            _staticModuleScenes.Add(sceneName.ToString());

        public void AddActiveScene(string sceneName) => _activeModuleScenes.Add(sceneName);

        public async Task LoadScenesForModule(ScreenPresenterMap screenPresenterMap)
        {
            List<string> scenes = new List<string> { screenPresenterMap.ToString() };
            IEnumerable<AdditiveScenesMap> additionalScenes = GetAdditionalScenes(screenPresenterMap);
            if (additionalScenes != null)
            {
                var sceneNames = additionalScenes.Select(scene => scene.ToString());
                scenes.AddRange(sceneNames);
            }

            Debug.Log("Loading scenes: " + string.Join(", ", scenes));
            await LoadScenesAsync(scenes); //loading of all the needed scenes
            _activeModuleScenes = scenes;
        }

        private static IEnumerable<AdditiveScenesMap> GetAdditionalScenes(ScreenPresenterMap screenPresenterMap)
        {
            return screenPresenterMap switch
            {
                ScreenPresenterMap.Bootstrap => new List<AdditiveScenesMap>(),
                // ScreenPresenterMap.Converter => new List<AdditiveScenesMap> {AdditiveScenesMap.DynamicBackground},
                ScreenPresenterMap.MainMenu => new List<AdditiveScenesMap>(),
                ScreenPresenterMap.Game => new List<AdditiveScenesMap>(),
                _ => null
            };
            
            //ScreenPresenterMap.StartGame: Returns an empty list (no additional scenes).
            //ScreenPresenterMap.Converter: Returns a list containing one item: AdditiveScenesMap.DynamicBackground.
            //Other cases (MainMenu, TicTac): Return empty lists as well.
            //default case returns null if no matching case is found.
        }

        //checks if the current a scene already exists
        private bool IsCurrentScene(string sceneName) =>
            SceneManager.GetActiveScene().name == sceneName; 

        private async Task LoadScenesAsync(List<string> scenes)
        {
            List<Task> loadTasks = new List<Task>();

            foreach (var scene in scenes)
            {
                if (!IsCurrentScene(scene))
                    loadTasks.Add(LoadSceneAsync(scene, true)); //if the scene is not a current scene we add it
                                                                      //to the task list and load it as an additive one
                else
                {
                    if (!_loadedModuleScenes.Contains(scene)) //if the scene is not in the list, we add it, but we don't load it
                        _loadedModuleScenes.Add(scene);
                }
            }

            await Task.WhenAll(loadTasks); //wait until all the additive scenes have been loaded
        }

        private async Task LoadSceneAsync(string sceneName, bool additive)
        {
            //takes SceneManager.LoadSceneAsync as a delegate and in the method we enter a name of a scene and check if it's additive or not
            // Single mode loads a standard Unity Scene which then appears on its own in the Hierarchy window.
            // Additive loads a Scene which appears in the Hierarchy window while another is active (Adds the Scene to the current loaded Scenes).
            await LoadSceneAsyncInternal(() =>
                SceneManager.LoadSceneAsync(sceneName,
                    additive ? LoadSceneMode.Additive : LoadSceneMode.Single));

            if (!_loadedModuleScenes.Contains(sceneName))
                _loadedModuleScenes.Add(sceneName);
        }

        //func - Encapsulates a method that has no parameters and returns a value of the type specified by the
        //TResult parameter. It's essentially a delegate
        
        //allowSceneActivation Allow Scenes to be activated as soon as it is ready. If a
        //LoadSceneAsync.allowSceneActivation is set to false, and another AsyncOperation
        //(e.g. SceneManager.UnloadSceneAsync ) initializes, Unity does not call the second operation until the
        //first AsyncOperation.allowSceneActivation is set to true.
        //TODO Рефакторинг
        private async Task LoadSceneAsyncInternal(Func<AsyncOperation> loadSceneFunc)
        {
            try
            {
                var asyncOperation = loadSceneFunc();

                while (!asyncOperation.isDone)
                {
                    // Если сцена загрузилась на 90% и ждёт активации
                    if (asyncOperation.progress >= 0.9f && !asyncOperation.allowSceneActivation)
                    {
                        asyncOperation.allowSceneActivation = true; // Разрешаем активацию
                    }

                    await Task.Yield();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading scene: {ex.Message}");
            }
        }


        public async Task UnloadUnusedScenesAsync()
        {
            if (_activeModuleScenes == null || _activeModuleScenes.Count == 0)
            {
                Debug.LogWarning("No scenes to load.");
                return;
            }

            var scenesToUnload = _loadedModuleScenes
                .Except(_activeModuleScenes) // exclusion of the active scenes
                .Except(_staticModuleScenes) // exclusion of the static scenes that we need for the whole project
                .ToList();

            var unloadTasks = new List<Task>();

            foreach (var scene in scenesToUnload)
            {
                var sceneToUnload = SceneManager.GetSceneByName(scene);
                if (sceneToUnload.IsValid() && sceneToUnload.isLoaded)
                    unloadTasks.Add(UnloadSceneAsync(scene));
                else
                    Debug.LogWarning($"Scene {scene} is not valid or not loaded.");
            }

            await Task.WhenAll(unloadTasks);

            _loadedModuleScenes = _loadedModuleScenes.Except(scenesToUnload).ToList();
        }

        // Convert AsyncOperation to Task
        private Task UnloadSceneAsync(string sceneName)
        {
            var tcs = new TaskCompletionSource<bool>();
            var asyncOp = SceneManager.UnloadSceneAsync(sceneName);
    
            if (asyncOp == null)
                tcs.SetResult(false);
            else
                asyncOp.completed += _ => tcs.SetResult(true);

            return tcs.Task;
        }
    }
}
