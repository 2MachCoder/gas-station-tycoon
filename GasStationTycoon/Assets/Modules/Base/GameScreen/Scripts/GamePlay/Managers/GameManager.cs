using CodeBase.CustomDIContainer;
using CodeBase.Systems.Gameplay;
using Modules.Base.GameScreen.Scripts.GamePlay.Player;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Managers
{
    public class GameManager
    {
        [Inject] private CarManager _carManager;
        [Inject] private BuildingsManager _buildingsManager;
        [Inject] private PlayerController _playerController;
        [Inject] private CurrencySystem _currencySystem;

        public GameManager()
        {
            RootDiContainer.Instance.Inject(this);
        }

        public void StartGame()
        {
            _carManager.Initialize();
            _buildingsManager.Initialize();
            _playerController.Initialize();
        }

        public void EndGame()
        {
            _buildingsManager.Shutdown();
            _carManager.Shutdown();
        }
    }
}