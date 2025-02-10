using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using CodeBase.CustomDIContainer;
using Configs.GamePlay.Scripts;
using Modules.Base.GameScreen.Scripts.GamePlay.Buildings;
using Modules.Base.GameScreen.Scripts.GamePlay.Cars;
using Modules.Base.GameScreen.Scripts.GamePlay.Managers;
using Modules.Base.GameScreen.Scripts.GamePlay.Player;

namespace Modules.Base.GameScreen.Scripts
{
    
    public class GameInstaller : SceneInstaller
    {
        [SerializeField] private GameScreenView gameScreenView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private BuildingsManager buildingsManager;
        [SerializeField] private MainBuilding mainBuilding;
        [SerializeField] private VehicleConfig vehicleConfig;
        [SerializeField] private CarManager carManager;
        [SerializeField] private PlayerController playerController;
        
        public override void RegisterSceneDependencies(BaseDiContainer container)
        {
            container.RegisterSingleton(screenCanvas);
            container.RegisterSingleton(mainCamera);
            container.RegisterSingleton(gameScreenView);
            

            // Registration gameplay objects
            container.RegisterSingleton(mainBuilding);
            container.RegisterSingleton(buildingsManager);
            container.InjectMonoBehaviour(buildingsManager);
            container.RegisterSingleton(carManager);
            container.RegisterSingleton(vehicleConfig);
            container.RegisterSingleton(playerController);
            container.RegisterSingleton(new CarFactory());
            container.RegisterSingleton(new CarPool());
            container.RegisterSingleton(new GameManager());
            
            container.RegisterSingleton<GameScreenModel, GameScreenModel>();
            container.RegisterSingleton<GameScreenPresenter, GameScreenPresenter>();
        }

        public override void InjectMonoBehaviours(IObjectResolver resolver)
        {
            resolver.InjectMonoBehaviour(gameScreenView);
        }
    }
}