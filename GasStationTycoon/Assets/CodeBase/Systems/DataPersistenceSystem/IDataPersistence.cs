using CodeBase.Systems.DataPersistenceSystem.Data;

namespace CodeBase.Systems.DataPersistenceSystem
{
    public interface IDataPersistence
    {
        void LoadData(SaveData saveData);
        void SaveData(ref SaveData saveData);
    }
}