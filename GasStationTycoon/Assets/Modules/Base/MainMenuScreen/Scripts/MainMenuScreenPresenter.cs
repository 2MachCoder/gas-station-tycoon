using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Systems.PopupHub;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly MainMenuScreenModel _screenModel;
        private readonly MainMenuScreenView _screenView;
        private readonly IPopupHub _popupHub;
        private readonly TaskCompletionSource<bool> _screenCompletionSource;
        
        public MainMenuScreenPresenter(IScreenStateMachine screenStateMachine, 
            MainMenuScreenModel screenModel, MainMenuScreenView screenView)
        {
            _screenStateMachine = screenStateMachine;
            _screenModel = screenModel;
            _screenView = screenView;
            // _popupHub = RootDiContainer.Instance.Resolve<IPopupHub>();
            _screenCompletionSource = new TaskCompletionSource<bool>();
        }

        public async Task Enter(object param)
        {
            _screenView.HideInstantly();
            _screenView.SetupEventListeners
            (
                OnPlayButtonClicked,
                OnSettingsButtonClicked
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
        
        //TODO Waits for fix
        private void OnPlayButtonClicked() => 
            RunNewScreen(ScreenPresenterMap.Game);

        private void OnSettingsButtonClicked()
        {
            //Waits for fix
            //_popupHub.OpenSettingsPopup();
        }

        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _screenCompletionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }
    }
}
