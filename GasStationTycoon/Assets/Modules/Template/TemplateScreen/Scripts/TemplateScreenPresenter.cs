using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;

namespace Modules.Template.TemplateScreen.Scripts
{
    public class TemplateScreenPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly TemplateScreenModel _screenModel;
        private readonly TemplateScreenView _screenView;
        private readonly TaskCompletionSource<bool> _screenCompletionSource;
        
        public TemplateScreenPresenter(IScreenStateMachine screenStateMachine, 
            TemplateScreenModel screenModel, TemplateScreenView screenView)
        {
            _screenStateMachine = screenStateMachine;
            _screenModel = screenModel;
            _screenView = screenView;
            _screenCompletionSource = new TaskCompletionSource<bool>();
        }

        public async Task Enter(object param)
        {
            _screenView.HideInstantly();
            _screenView.SetupEventListeners
            (
                OnMainMenuButtonClicked
            );
            await _screenView.Show();
        }

        public async Task Execute() => await _screenCompletionSource.Task;

        public async Task Exit() => await _screenView.Hide();

        public void Dispose()
        {
            _screenView.Dispose();
            _screenModel.Dispose();
        }
        
        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ScreenPresenterMap.MainMenu);

        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _screenCompletionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }
    }
}
