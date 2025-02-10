using CodeBase.CustomDIContainer;
using CodeBase.Systems.DataPersistenceSystem;
using CodeBase.Systems.DataPersistenceSystem.Data;
using CodeBase.Systems.DataPersistenceSystem.Data.Game;
using System.Collections.Generic;

namespace CodeBase.Systems.Gameplay
{
    //TODO May be need an interface
    public class GameDataMediatorSystem : IDataPersistence
    {
        private readonly RootDiContainer _container = RootDiContainer.Instance;
        private SaveData _saveData;
        
        public GameDataMediatorSystem()
        {
            _container.Inject(this);
            _saveData ??= new SaveData();
        }

        public int GetLevelData() => _saveData?.gameData?.level ?? 0;

        public List<ServiceBuildingSlotDto> GetBuildingSlotsData() => 
            _saveData?.gameData?.serviceBuildingsSlotsData ?? new List<ServiceBuildingSlotDto>();

        public void SetLevelData(int newLevel)
        {
            if (newLevel < 0) return; // Уровень не может быть отрицательным
            _saveData.gameData.level = newLevel;
        }

        public void SetBuildingsData(List<ServiceBuildingSlotDto> buildingsData)
        {
            if (buildingsData == null) return;
            _saveData.gameData.serviceBuildingsSlotsData = buildingsData;
        }

        public void LoadData(SaveData saveData) => _saveData = saveData ?? new SaveData();

        public void SaveData(ref SaveData saveData) => saveData.gameData = _saveData.gameData;
    }
}
