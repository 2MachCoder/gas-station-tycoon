using System.Collections.Generic;
using UnityEngine;

namespace Configs.GamePlay.Scripts
{
    [CreateAssetMenu(fileName = "VehicleConfig", menuName = "Vehicles/VehicleConfig", order = 0)]
    public class VehicleConfig : ScriptableObject
    {
        public List<VehicleData> Vehicles;
    }
    
    [System.Serializable]
    public class VehicleData
    {
        public VehicleType Type;
        public GameObject ModelPrefab;
        public List<Material> Materials;
    }
}