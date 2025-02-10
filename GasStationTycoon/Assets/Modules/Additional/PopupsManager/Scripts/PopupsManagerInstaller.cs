using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using CodeBase.Systems.PopupHub;
using UnityEngine;
using CodeBase.Core.Systems.PopupHub.Popups;
using CodeBase.CustomDIContainer;
using CodeBase.Systems.PopupHub.Popups;

namespace Modules.Additional.PopupsManager.Scripts
{
    public class PopupsManagerInstaller : SceneInstaller
    {
        [SerializeField] private BasePopupCanvas popupCanvas;
        [SerializeField] private SettingsPopup settingsPopupPrefab;
        
        public override void RegisterSceneDependencies(BaseDiContainer container)
        {
            container.RegisterSingletonAsImplementedInterfaces(new PopupHub());
            container.RegisterSingleton(popupCanvas);
            RegisterPopupFactories(container);
        }

        private void RegisterPopupFactories(BaseDiContainer container)
        {
            container
                .RegisterSingleton<IBasePopupFactory<SettingsPopup>>
                    (new BasePopupFactory<SettingsPopup>(RootDiContainer.Instance, settingsPopupPrefab));
        }
    }
}