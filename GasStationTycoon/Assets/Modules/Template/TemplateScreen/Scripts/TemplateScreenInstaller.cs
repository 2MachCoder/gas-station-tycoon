using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using CodeBase.CustomDIContainer;

namespace Modules.Template.TemplateScreen.Scripts
{
    public class TemplateInstaller : SceneInstaller
    {
        [SerializeField] private TemplateScreenView templateScreenView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(BaseDiContainer container)
        {
            // Регистрируем UI элементы
            container.RegisterSingleton(screenCanvas);
            container.RegisterSingleton(mainCamera);
            container.RegisterSingleton(templateScreenView);
            
            // Регистрируем модель экрана
            container.RegisterSingleton<TemplateScreenModel, TemplateScreenModel>();

            // Регистрируем TemplatePresenter, учитывая его зависимости в конструкторе
            container.RegisterSingleton<TemplateScreenPresenter, TemplateScreenPresenter>();
        }

        public override void InjectMonoBehaviours(IObjectResolver resolver)
        {
            // Инжектим зависимости в UI-классы
            resolver.InjectMonoBehaviour(templateScreenView);
        }
    }
}