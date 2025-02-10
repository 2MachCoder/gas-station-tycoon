using System;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.CustomDIContainer;
using CodeBase.Services;
using CodeBase.Services.App;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Implementation.Infrastructure
{
    public class ScreenStateMachine : IScreenStateMachine, IStartable
    {
        [Inject] private readonly IApplicationService _applicationService;
        [Inject] private readonly AudioListenerService _audioListenerService;
        [Inject] private readonly SceneInstallerService _sceneInstallerService;
        [Inject] private readonly ScreenTypeMapper _screenTypeMapper;
        [Inject] private readonly SceneService _sceneService;
        
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1); // reducing the number of threads to one
        private readonly RootDiContainer _container = RootDiContainer.Instance;
        public IScreenPresenter CurrentPresenter { get; private set; }
        
        public ScreenStateMachine() => _container.Inject(this);

        public void Start() => RunScreen(SceneManager.GetActiveScene().name);
        
        private void RunScreen(string sceneName, object param = null)
        {
            ScreenPresenterMap? screenModelMap = SceneNameToEnum(sceneName);
            
            if (screenModelMap != null)
                _ = RunScreen((ScreenPresenterMap)screenModelMap, param);
            else
            {
                _sceneService.AddActiveScene(sceneName);
                _sceneInstallerService.
                    CombineScenes(_container, false);
            }
        }

        public async Task RunScreen(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            Debug.Log("Run Screen: " + screenPresenterMap);
            await _semaphoreSlim.WaitAsync(); //Asynchronously waits to enter the SemaphoreSlim.

            try
            {
                await _sceneService.LoadScenesForModule(screenPresenterMap);
                await _sceneService.UnloadUnusedScenesAsync();

                // creates children for the root installer
                
                var sceneLifetimeScope = _sceneInstallerService.CombineScenes(_container, true);  
                
                // responsible for resolving (or instantiating) the appropriate screen presenter for the
                // specified screenPresenterMap, using a dependency injection container provided by sceneLifetimeScope
                CurrentPresenter = _screenTypeMapper.Resolve(screenPresenterMap, sceneLifetimeScope);
                AudioListenerService.EnsureAudioListenerExists(sceneLifetimeScope.GetContainer());
                
                await CurrentPresenter.Enter(param);
                await CurrentPresenter.Execute();
                await CurrentPresenter.Exit();
                CurrentPresenter.Dispose();
                _container.EndScope();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while running screen {screenPresenterMap}: {ex}");
            }
            finally { _semaphoreSlim.Release(); }
        }

        //tries to convert screen name in string to its name in enum. Can return null if the sceneName is not found
        private static ScreenPresenterMap? SceneNameToEnum(string sceneName)
        {
            if (Enum.TryParse(sceneName, out ScreenPresenterMap result)) return result;
            return null;
        }
    }
}
