using System.Threading.Tasks;
using CodeBase.Core.GamePlay.Buildings.ServiceBuildingSlot;
using CodeBase.Systems.DataPersistenceSystem.Data.Game.ServiceBuildingSlot;
using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings.BuildingSlot.States
{
    public class ActiveState : IServiceBuildingState
    {
        private readonly ServiceBuildingSlot _slot;

        public ActiveState(ServiceBuildingSlot slot)
        {
            _slot = slot;
        }
        
        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("Building is already active.");
        }

        public async void EnterState()
        {
            _slot.CurrentState = ServiceBuildingSlotState.Active;
            SetupUiElements();

            if (_slot.ServiceBuilding != null || _slot.IsModelPreloaded)
            {
                _slot.InitializeServiceBuilding();
                return;
            }

            _slot.PreloadServiceBuilding();
            await WaitForModelPreload();
            _slot.InitializeServiceBuilding();
        }

        private async Task WaitForModelPreload()
        {
            while (!_slot.IsModelPreloaded)
                await Task.Yield();
        }

        private void SetupUiElements( )
        {
            _slot.MeshRenderer.enabled = false;
            _slot.Canvas.enabled = false;
            _slot.LockImage.enabled = false;
            _slot.UnlockImage.enabled = false;
        }
    }
}