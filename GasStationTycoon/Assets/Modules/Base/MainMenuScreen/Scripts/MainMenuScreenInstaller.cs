using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using CodeBase.CustomDIContainer;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuInstaller : SceneInstaller
    {
        [SerializeField] private MainMenuScreenView mainMenuScreenView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;
        
        public override void RegisterSceneDependencies(BaseDiContainer container)
        {
            container.RegisterSingleton(screenCanvas);
            container.RegisterSingleton(mainCamera);
            container.RegisterSingleton(mainMenuScreenView);
            
            container.RegisterSingleton<MainMenuScreenModel>();
            container.RegisterSingleton<MainMenuScreenPresenter>();
        }

        public override void InjectMonoBehaviours(IObjectResolver resolver)
        {
            resolver.InjectMonoBehaviour(mainMenuScreenView);
        }
    }
}