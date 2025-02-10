using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Systems.Gameplay;
using Modules.Base.GameScreen.Scripts.GamePlay.Managers;
using Modules.Base.GameScreen.Scripts.GamePlay.Player;

namespace Modules.Base.GameScreen.Scripts
{
    public class GameScreenPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly GameScreenModel _screenModel;
        private readonly GameScreenView _screenView;
        private readonly TaskCompletionSource<bool> _screenCompletionSource;
        private readonly GameManager _gameManager;
        private readonly PlayerController _playerController;
        private readonly CurrencySystem _currencySystem;
        
        public GameScreenPresenter(IScreenStateMachine screenStateMachine, GameScreenModel screenModel, 
            GameScreenView screenView, GameManager gameManager, PlayerController playerController, 
            CurrencySystem currencySystem)
        {
            _screenStateMachine = screenStateMachine;
            _screenModel = screenModel;
            _screenView = screenView;
            _gameManager = gameManager;
            _playerController = playerController;
            _currencySystem = currencySystem;
            _screenCompletionSource = new TaskCompletionSource<bool>();
        }

        public async Task Enter(object param)
        {
            _screenView.HideInstantly();
            _screenView.SetupEventListeners
            (
                OnMainMenuButtonClicked
            );
            _playerController.OnMoneyChanged += UpdatePlayerMoney;
            _currencySystem.OnMoneyChanged += UpdateTotalMoney;
            _screenView.InitializeMoneyCounters(_currencySystem.CurrentMoney, 0);
            _gameManager.StartGame();
            await _screenView.Show();
        }

        private void UpdatePlayerMoney(int money) => _screenView.UpdatePlayerMoney(money);

        private void UpdateTotalMoney(int money) => _screenView.UpdateAvailableMoney(money);

        public async Task Execute() => await _screenCompletionSource.Task;

        public async Task Exit() => await _screenView.Hide();

        public void Dispose()
        {
            _playerController.OnMoneyChanged -= UpdatePlayerMoney;
            _currencySystem.OnMoneyChanged -= UpdateTotalMoney;
            _screenView.Dispose();
            _screenModel.Dispose();
            _gameManager.EndGame();
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
