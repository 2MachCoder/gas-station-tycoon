using System;
using CodeBase.Core.GamePlay;
using CodeBase.Core.GamePlay.Buildings;
using CodeBase.CustomDIContainer;
using CodeBase.Systems.Gameplay;
using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Buildings
{
    public class ATM : BaseServiceBuilding
    {
        [Inject] private CurrencySystem _currencySystem;

        public event Action<ATM> OnMoneyPutOnBalance; 
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController)) return;
            
            // Debug.Log("Player is in trigger zone");
            ShowProgressBar();
            _ = ExecuteTask(PlayerController);
        }
        
        public void Initialize()
        {
            RootDiContainer.Instance.InjectMonoBehaviour(this);
            progressBar.ReportToZero(progressBar.CurrentRatio);
            progressBar.ResetProgress();
            HideProgressBar();
        }
        
        private void HideProgressBar()
        {
            progressBar.gameObject.SetActive(false);
        }
        
        private void  ShowProgressBar()
        {
            progressBar.gameObject.SetActive(true);
        }
        protected override void CompleteAction(BasePlayerController playerController)
        {
            // Debug.Log("Money was put on balance");
            HideProgressBar();
            _currencySystem.AddMoney(playerController.SpendMoney());
            OnMoneyPutOnBalance?.Invoke(this);
        }
    }
}