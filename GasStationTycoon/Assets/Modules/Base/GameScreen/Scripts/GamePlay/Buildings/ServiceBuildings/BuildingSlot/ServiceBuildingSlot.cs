using System;
using CodeBase.Core.GamePlay.Buildings.ServiceBuildingSlot;
using CodeBase.CustomDIContainer;
using CodeBase.Systems.DataPersistenceSystem.Data.Game;
using CodeBase.Systems.DataPersistenceSystem.Data.Game.ServiceBuildingSlot;
using CodeBase.Systems.Gameplay;
using Configs.GamePlay.Scripts;
using Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings.BuildingSlot.States;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings.BuildingSlot
{
    public class ServiceBuildingSlot : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ServiceBuildingData serviceBuildingData;
        [SerializeField] private AddressableServiceBuildingPrefabsKeys addressableServiceBuildingPrefabsKeys;
        
        private IServiceBuildingState _currentState;
        private GameObject _cachedServiceModel;
        private CurrencySystem _currencySystem;
        
        public event Action<ServiceBuilding> OnServiceBuildingInitialized;
        
        [field: Header("UI Elements")]
        [field: SerializeField] public TextMeshProUGUI Text { get; internal set; }
        [field: SerializeField] public Canvas Canvas { get; private set; }
        [field: SerializeField] public Image LockImage { get; private set; }
        [field: SerializeField] public Image UnlockImage { get; private set; }
        
        [field: Header("Other components")]
        [field: SerializeField] public BoxCollider TriggerCollider { get; private set; }
        [field: SerializeField] public MeshRenderer MeshRenderer { get; private set; }
        public ServiceBuildingSlotDto SlotData { get; private set; }
        public ServiceBuilding ServiceBuilding { get; private set; }
        public ServiceBuildingSlotState CurrentState
        {
            get => SlotData.state;
            internal set => SlotData.state = value;
        }
        public int ServiceBuildingCost { get; private set; }
        public bool IsModelPreloaded => _cachedServiceModel != null;

        public void Initialize(ServiceBuildingSlotDto serviceBuildingSlotDto)
        {
            ServiceBuildingCost = serviceBuildingData.UnlockCost;
            _currencySystem = RootDiContainer.Instance.Resolve<CurrencySystem>();

            if (SlotData == null) 
                SlotData = serviceBuildingSlotDto;
            else 
                SlotData.state = serviceBuildingSlotDto.state;

            switch (CurrentState)
            {
                case ServiceBuildingSlotState.Locked:
                    ChangeState(new LockedState(this));
                    break;
                case ServiceBuildingSlotState.Unlocked:
                    ChangeState(new UnlockedState(this));
                    break;
                case ServiceBuildingSlotState.Active:
                    ChangeState(new ActiveState(this));
                    break;
            }

            TriggerCollider.enabled = true;
        }


        private void OnTriggerEnter(Collider other)
        {
            _currentState.OnTriggerEnter(other);
        }

        public void ChangeState(IServiceBuildingState newState)
        {
            _currentState = newState;
            _currentState.EnterState();
        }
        
        public bool TrySpendCurrency() => _currencySystem.TrySpendMoney(ServiceBuildingCost);

        public void PreloadServiceBuilding()
        {
            if (CurrentState == ServiceBuildingSlotState.Locked) return;

            var addressableKey = serviceBuildingData.GetAddressableKey(addressableServiceBuildingPrefabsKeys);
            if (string.IsNullOrEmpty(addressableKey)) return;

            Addressables.LoadAssetAsync<GameObject>(addressableKey).Completed += OnModelLoaded;
        }

        private void OnModelLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _cachedServiceModel = handle.Result;

                if (ServiceBuilding != null)
                    OnServiceBuildingInitialized?.Invoke(ServiceBuilding);
            }
            else
                Debug.LogError($"Failed to load service building model for {serviceBuildingData.ServiceType}");
        }
        
        public void InitializeServiceBuilding()
        {
            if (_cachedServiceModel == null)
                return;

            var instance = Instantiate(_cachedServiceModel, transform.position, Quaternion.identity);
            instance.transform.SetParent(transform);
            ServiceBuilding = instance.GetComponent<ServiceBuilding>();
            OnServiceBuildingInitialized?.Invoke(ServiceBuilding);
        }
    }
}
