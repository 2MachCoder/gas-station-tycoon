using System;
using UnityEngine;

namespace CodeBase.Core.GamePlay
{
    public class BasePlayerController : MonoBehaviour
    {
        protected int _currentMoney;
        
        public event Action<int> OnMoneyChanged;

        public int CurrentMoney => _currentMoney;
        
        public virtual void Initialize()
        {
            OnMoneyChanged?.Invoke(CurrentMoney);
        }
        
        public void EarnMoney(int amount)
        {
            _currentMoney += amount;
            OnMoneyChanged?.Invoke(CurrentMoney);
        }

        public int SpendMoney()
        {
            var money = CurrentMoney;
            _currentMoney = 0;
            OnMoneyChanged?.Invoke(_currentMoney);
            return money;
        }
    }
}