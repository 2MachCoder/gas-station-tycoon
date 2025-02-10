using CodeBase.Core.Modules;
using CodeBase.CustomDIContainer;
using Modules.Base.GameScreen.Scripts.GamePlay.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Base.GameScreen.Scripts
{
    public class GameScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text gameScreenTitle;
        [SerializeField] private TMP_Text totalMoneyText;
        [SerializeField] private TMP_Text playerMoneyText;
        
        public void SetupEventListeners(UnityAction onMainMenuButtonClicked)
        {
            mainMenuButton.onClick.AddListener(onMainMenuButtonClicked);
        }

        public void InitializeMoneyCounters(int availableMoney, int playerMoney)
        {
            UpdateAvailableMoney(availableMoney);
            UpdatePlayerMoney(playerMoney);
        }
        
        public void UpdateAvailableMoney(int availableMoney) => 
            totalMoneyText.text = "Available money " + availableMoney;

        public void UpdatePlayerMoney(int playerMoney) => 
            playerMoneyText.text = "Money in inventory: " + playerMoney;

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners()
        {
            mainMenuButton.onClick.RemoveAllListeners();
        }
    }
}