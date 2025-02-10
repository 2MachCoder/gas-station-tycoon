using System.Collections.Generic;
using UnityEngine;

namespace Configs.GamePlay.Scripts
{
    [CreateAssetMenu(fileName = "AddressableServiceBuildingPrefabsKeys", menuName = "ServiceBuildings/Addressable Keys Config")]
    public class AddressableServiceBuildingPrefabsKeys : ScriptableObject
    {
        [System.Serializable]
        private struct AddressableKeyEntry
        {
            public ServiceBuildingType type;
            public string addressableKey;
        }

        [SerializeField] private List<AddressableKeyEntry> addressableKeyEntries;

        private Dictionary<ServiceBuildingType, string> keyLookup;

        private void OnEnable()
        {
            keyLookup = new Dictionary<ServiceBuildingType, string>();
            foreach (var entry in addressableKeyEntries)
            {
                if (!keyLookup.ContainsKey(entry.type))
                {
                    keyLookup.Add(entry.type, entry.addressableKey);
                }
                else
                {
                    Debug.LogError($"Duplicate addressable key for {entry.type} in AddressableServiceBuildingPrefabsKeys!");
                }
            }
        }

        public string GetAddressableKey(ServiceBuildingType type)
        {
            if (keyLookup.TryGetValue(type, out string key))
                return key;

            Debug.LogError($"No addressable key found for {type}!");
            return string.Empty;
        }
    }

}