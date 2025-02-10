using UnityEngine;

namespace Configs.GamePlay.Scripts
{
    [CreateAssetMenu(fileName = "NewServiceBuilding", menuName = "ServiceBuildings/Service Building")]
    public class ServiceBuildingData : ScriptableObject
    {
        [SerializeField] private ServiceBuildingType serviceBuildingType; // Enum instead of string
        [SerializeField] private int unlockCost;

        public ServiceBuildingType ServiceType => serviceBuildingType;

        public int UnlockCost => unlockCost;

        public string GetAddressableKey(AddressableServiceBuildingPrefabsKeys config)
        {
            return config.GetAddressableKey(serviceBuildingType);
        }
    }

}