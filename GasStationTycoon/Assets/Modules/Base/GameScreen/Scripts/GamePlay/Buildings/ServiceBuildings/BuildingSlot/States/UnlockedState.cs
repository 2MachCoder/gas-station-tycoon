using CodeBase.Core.GamePlay.Buildings.ServiceBuildingSlot;
using CodeBase.Systems.DataPersistenceSystem.Data.Game.ServiceBuildingSlot;
using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings.BuildingSlot.States
{
    public class UnlockedState : IServiceBuildingState
    {
        private readonly ServiceBuildingSlot _slot;

        public UnlockedState(ServiceBuildingSlot slot)
        {
            _slot = slot;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!_slot.TrySpendCurrency()) return;
            
            _slot.ChangeState(new ActiveState(_slot));
        }
        
        public void EnterState()
        {
            _slot.CurrentState = ServiceBuildingSlotState.Unlocked;
            SetupUiElements();
            _slot.PreloadServiceBuilding();
        }

        private void SetupUiElements()
        {
            _slot.MeshRenderer.enabled = true;
            _slot.Canvas.enabled = true;
            _slot.LockImage.enabled = false;
            _slot.UnlockImage.enabled = true;
            _slot.Text.text = $"Buy {_slot.ServiceBuildingCost}";
        }
    }
}