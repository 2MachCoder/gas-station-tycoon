using System;
using CodeBase.Systems.DataPersistenceSystem.Data.Game.ServiceBuildingSlot;

namespace CodeBase.Systems.DataPersistenceSystem.Data.Game
{
    [Serializable]
    public class ServiceBuildingSlotDto
    {
        public ServiceBuildingSlotState state; // ✅ Хранит состояние вместо isUnlocked

        public ServiceBuildingSlotDto()
        {
            state = ServiceBuildingSlotState.Locked; // По умолчанию здание закрыто
        }
    }
}