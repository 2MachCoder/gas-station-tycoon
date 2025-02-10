using CodeBase.Core.GamePlay.Buildings.ServiceBuildingSlot;
using CodeBase.Systems.DataPersistenceSystem.Data.Game.ServiceBuildingSlot;
using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings.BuildingSlot.States
{
    public class LockedState : IServiceBuildingState
    {
        private readonly ServiceBuildingSlot _slot;

        public LockedState(ServiceBuildingSlot slot)
        {
            _slot = slot;
        }
        
        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("Building is locked. Can't interact.");
        }

        public void EnterState()
        {
            _slot.CurrentState = ServiceBuildingSlotState.Locked;
            SetupUiElements();
        }

        private void SetupUiElements()
        {
            _slot.MeshRenderer.enabled = true;
            _slot.Canvas.enabled = true;
            _slot.LockImage.enabled = true;
            _slot.UnlockImage.enabled = false;
            _slot.Text.text = string.Empty;
        }
    }
}