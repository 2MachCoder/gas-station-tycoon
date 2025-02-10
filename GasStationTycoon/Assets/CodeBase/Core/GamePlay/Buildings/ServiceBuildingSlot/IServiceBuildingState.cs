using UnityEngine;

namespace CodeBase.Core.GamePlay.Buildings.ServiceBuildingSlot
{
    public interface IServiceBuildingState
    {
        void EnterState();
        void OnTriggerEnter(Collider other);
    }
}