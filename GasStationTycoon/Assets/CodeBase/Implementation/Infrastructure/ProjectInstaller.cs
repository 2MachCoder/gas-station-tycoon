using CodeBase.CustomDIContainer;
using CodeBase.Implementation.Systems.DataPersistenceSystem;
using CodeBase.Services;
using CodeBase.Services.App;
using CodeBase.Services.LongInitializationServices;
using CodeBase.Services.SceneInstallerService;
using CodeBase.Systems;
using CodeBase.Systems.DataPersistenceSystem;
using CodeBase.Systems.Gameplay;
using UnityEngine;

namespace CodeBase.Implementation.Infrastructure
{
    [RequireComponent(typeof(ApplicationService))]
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private ApplicationService applicationService;

        private void Awake() => 
            InstallBindings(RootDiContainer.Instance);

        public void InstallBindings(RootDiContainer container)
        {
            RegisterServices(container);
            RegisterSystems(container);
            container.RegisterSingleton(new ScreenTypeMapper());
            container.RegisterSingletonAsImplementedInterfaces(new ScreenStateMachine());
        }

        private void RegisterServices(RootDiContainer container)
        {
            RegisterLongInitializationService(container);

            container.RegisterSingleton(new EventSystemService());
            container.RegisterSingleton(new AudioListenerService());
            container.RegisterSingleton(new SceneService());
            container.RegisterSingleton(new SceneInstallerService());
            container.RegisterSingletonAsImplementedInterfaces(applicationService);
        }

        private static void RegisterLongInitializationService(RootDiContainer container)
        {
            container.RegisterSingleton(new FirstLongInitializationService());
            container.RegisterSingleton(new SecondLongInitializationService());
            container.RegisterSingleton(new ThirdLongInitializationService());
        }

        private static void RegisterSystems(RootDiContainer container)
        {
            container.RegisterSingleton(new GameDataMediatorSystem());
            container.RegisterSingleton(new CurrencySystem());
            container.RegisterSingletonAsImplementedInterfaces(new DataPersistenceManager());
        }
    }
}