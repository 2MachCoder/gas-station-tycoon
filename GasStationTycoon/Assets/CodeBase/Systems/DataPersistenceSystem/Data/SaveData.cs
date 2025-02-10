using System;
using CodeBase.Systems.DataPersistenceSystem.Data.Game;

namespace CodeBase.Systems.DataPersistenceSystem.Data
{
    [Serializable]
    public class SaveData
    {
        public GameData gameData;
        public SettingsData settingsData;
        
        public SaveData()
        {
            gameData = new GameData();
            settingsData = new SettingsData();
        }
    }
}