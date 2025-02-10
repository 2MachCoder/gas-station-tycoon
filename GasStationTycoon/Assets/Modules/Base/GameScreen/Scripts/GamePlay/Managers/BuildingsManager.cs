using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.CustomDIContainer;
using CodeBase.Services.App;
using CodeBase.Systems.DataPersistenceSystem.Data.Game;
using CodeBase.Systems.DataPersistenceSystem.Data.Game.ServiceBuildingSlot;
using CodeBase.Systems.Gameplay;
using Configs.GamePlay.Scripts;
using Modules.Base.GameScreen.Scripts.GamePlay.Buildings;
using Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings;
using Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings.BuildingSlot;
using Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings.BuildingSlot.States;
using Modules.Base.GameScreen.Scripts.GamePlay.Cars;
using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Managers
{
    //TODO Make inversion of data control between DataMediator or even DataPersistenceManager
    public class BuildingsManager : MonoBehaviour
    {
        [SerializeField] private List<ServiceBuildingSlot> serviceBuildingSlotsInUnlockOrder;
        [SerializeField] private LevelsOfMainBuildingToUnlockSlots levelsOfMainBuildingToUnlockSlots;
        [SerializeField] private MainBuilding mainBuilding;
        [SerializeField] private ATM atm;

        [Inject] private GameDataMediatorSystem _gameDataMediator;
        [Inject] private IApplicationService _applicationService;
        
        private List<ServiceBuilding> _serviceBuildings = new();
        private List<ServiceBuildingSlotDto> _serviceBuildingSlotsData = new();
        private int _mainBuildingLevel;
        
        public event Action<ParkingSlot> OnParkingSlotAdded;
        public event Action<ServiceBuilding> OnServiceBuildingAdded; 

        public List<ServiceBuilding> ServiceBuildings => _serviceBuildings;
        public ATM Atm => atm;

        public void Initialize()
        {
            LoadData();

            mainBuilding.Initialize(_mainBuildingLevel);
            mainBuilding.OnLevelUp += CheckBuildingSlotsUnlock;
            Atm.Initialize();

            InitializeBuildingSlots();
            UnlockNextBuildingSlot();
            
            _applicationService.ApplicationPause += SaveData;
            _applicationService.ApplicationFocusLost += SaveData;
        }

        public void Shutdown()
        {
            SaveData();
            mainBuilding.OnLevelUp -= CheckBuildingSlotsUnlock;
            mainBuilding.ShutDown();
            
            _applicationService.ApplicationPause -= SaveData;
            _applicationService.ApplicationFocusLost -= SaveData;
        }

        private void InitializeBuildingSlots()
        {
            foreach (var buildingSlot in serviceBuildingSlotsInUnlockOrder)
            {
                buildingSlot.OnServiceBuildingInitialized += AddServiceBuilding;
                buildingSlot
                    .Initialize(_serviceBuildingSlotsData[serviceBuildingSlotsInUnlockOrder.IndexOf(buildingSlot)]);
            }
        }
        
        private void AddServiceBuilding(ServiceBuilding serviceBuilding)
        {
            ServiceBuildings.Add(serviceBuilding);
            OnServiceBuildingAdded?.Invoke(serviceBuilding);
            OnParkingSlotAdded?.Invoke(serviceBuilding.ParkingSlot);
        }

        private void CheckBuildingSlotsUnlock(int level)
        {
            if (!levelsOfMainBuildingToUnlockSlots.LevelsToUnlock.Contains(level)) return;

            UnlockNextBuildingSlot();
        }

        private void UnlockNextBuildingSlot()
        {
            foreach (var serviceSlot in serviceBuildingSlotsInUnlockOrder
                         .Where(serviceSlot => serviceSlot.CurrentState != ServiceBuildingSlotState.Unlocked && 
                         serviceSlot.CurrentState != ServiceBuildingSlotState.Active))
            {
                serviceSlot.ChangeState(new UnlockedState(serviceSlot));
                return;
            }
        }

        private void LoadData()
        {
            _mainBuildingLevel = _gameDataMediator.GetLevelData();
            _serviceBuildingSlotsData = _gameDataMediator.GetBuildingSlotsData();
        }

        private void SaveData()
        {
            _gameDataMediator.SetLevelData(mainBuilding.Level);

            for (int i = 0; i < serviceBuildingSlotsInUnlockOrder.Count; i++)
            {
                if (i < _serviceBuildingSlotsData.Count)
                    _serviceBuildingSlotsData[i].state = serviceBuildingSlotsInUnlockOrder[i].CurrentState;
                else
                    _serviceBuildingSlotsData.Add(serviceBuildingSlotsInUnlockOrder[i].SlotData);
            }
            
            _gameDataMediator.SetBuildingsData(_serviceBuildingSlotsData);
        }
    }
}
