using System;
using System.Collections.Generic;
using CodeBase.Systems.DataPersistenceSystem.Data.Game.ServiceBuildingSlot;

namespace CodeBase.Systems.DataPersistenceSystem.Data.Game
{
    [Serializable]
    public class GameData
    {
        public List<ServiceBuildingSlotDto> serviceBuildingsSlotsData;
        public int level;
        public int money;

        public GameData()
        {
            serviceBuildingsSlotsData = new List<ServiceBuildingSlotDto>
            {
                new ServiceBuildingSlotDto(),
                new ServiceBuildingSlotDto(),
                new ServiceBuildingSlotDto(),
            };

            level = 0;
            money = 150;
        }
    }
}