using System;
using CodeBase.Systems.DataPersistenceSystem;
using CodeBase.Systems.DataPersistenceSystem.Data;

namespace CodeBase.Systems.Gameplay
{
    public class CurrencySystem : IDataPersistence
    {
        private SaveData _saveData = new();
        private int _currentMoney;

        public event Action<int> OnMoneyChanged;

        public int CurrentMoney => _currentMoney;

        public void AddMoney(int amount)
        {
            _currentMoney = CurrentMoney + amount;
            OnMoneyChanged?.Invoke(CurrentMoney);
        }

        public bool TrySpendMoney(int amount)
        {
            if (CurrentMoney < amount) return false;
            _currentMoney = CurrentMoney - amount;
            OnMoneyChanged?.Invoke(CurrentMoney);
            return true;
        }

        public void LoadData(SaveData saveData)
        {
            _saveData = saveData;
            _currentMoney = _saveData.gameData.money;
        }

        public void SaveData(ref SaveData saveData)
        {
            saveData.gameData.money = CurrentMoney;
        }
    }

}