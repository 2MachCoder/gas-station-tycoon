using System;
using CodeBase.Core.GamePlay;
using CodeBase.Core.GamePlay.Buildings;
using CodeBase.Systems.Gameplay;
using Modules.Base.GameScreen.Scripts.GamePlay.Cars;
using UnityEngine.UI;
using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings
{
    public class ServiceBuilding : BaseServiceBuilding
    {
        private const int ServiceCost = 30;
        [SerializeField] private Image icon;
        [SerializeField] private ParkingSlot parkingSlot;

        private CurrencySystem _currencySystem;
        private CarController _carController;

        public event Action<ServiceBuilding, BasePlayerController> OnTaskHasBeenStarted;
        public event Action<ServiceBuilding> OnTaskCompleted;
        
        public ParkingSlot ParkingSlot => parkingSlot;
        public bool HasTaskToBeDone { get; private set; }

        private void Awake()
        {
            progressBar.ReportToZero(progressBar.CurrentRatio);
            progressBar.ResetProgress();
            HideTaskIcon();
            ParkingSlot.IsFree = true;
            _carController = null;
            PlayerController = null;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BasePlayerController playerController))
            {
                PlayerController = playerController;
            }

            if (_carController == null && other.TryGetComponent(out CarController car))
            {
                _carController = car;
                ShowTaskIcon();
                HasTaskToBeDone = true;
            }

            if (PlayerController == null || _carController == null) return;
            
            _taskInProgress = true;
            OnTaskHasBeenStarted?.Invoke(this, PlayerController);
            _ = ExecuteTask();
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);

            if (_carController != null && other.TryGetComponent(out CarController car) && _carController == car)
            {
                //Debug.Log("Car is out of trigger zone");
                _carController = null;
            }
        }

        protected override void CompleteAction(BasePlayerController playerController = null)
        {
            if (_carController == null) return;
            
            HideTaskIcon();
            _carController.ActionCompleted();
            PlayerController.EarnMoney(ServiceCost);
            OnTaskCompleted?.Invoke(this);

            ParkingSlot.IsFree = true;
            _carController = null;
            HasTaskToBeDone = false;
            _taskInProgress = false;
        }

        private void ShowTaskIcon()
        {
            progressBar.gameObject.SetActive(true);
            icon.enabled = true;
        }

        private void HideTaskIcon()
        {
            icon.enabled = false;
            progressBar.gameObject.SetActive(false);
        }
    }
}