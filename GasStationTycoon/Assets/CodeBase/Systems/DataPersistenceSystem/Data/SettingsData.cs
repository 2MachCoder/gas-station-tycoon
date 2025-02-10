using System;

namespace CodeBase.Systems.DataPersistenceSystem.Data
{
    [Serializable]
    public class SettingsData
    {
        public float masterVolume;
        public float musicVolume;

        public SettingsData()
        {
            masterVolume = 1f;
            musicVolume = 0.5f;
        }
    }
}