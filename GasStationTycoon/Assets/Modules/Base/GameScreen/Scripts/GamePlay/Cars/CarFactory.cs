using System.Linq;
using CodeBase.Core.Patterns.ObjectCreation;
using CodeBase.CustomDIContainer;
using Configs.GamePlay.Scripts;
using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Cars
{
    public class CarFactory : IFactory<VehicleType, Vector3, CarController>
    {
        [Inject] private VehicleConfig _vehicleConfig;

        public CarFactory()
        {
            RootDiContainer.Instance.Inject(this);
        }
        
        public CarController Create(VehicleType vehicleType, Vector3 spawnPosition)
        {
            var vehicleData = _vehicleConfig.Vehicles.FirstOrDefault(v => v.Type == vehicleType);
            if (vehicleData == null)
            {
                Debug.LogError($"Vehicle type {vehicleType} not found in VehicleConfig!");
                return null;
            }

            var position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
            var newCar = Object.Instantiate(vehicleData.ModelPrefab, position, Quaternion.identity);
            if (newCar == null)
            {
                Debug.LogError($"Failed to instantiate vehicle of type {vehicleType}");
                return null;
            }
            
            var carController = newCar.AddComponent<CarController>();
            ApplyRandomMaterial(newCar, vehicleData);
            return carController;
        }
        
        private void ApplyRandomMaterial(GameObject car, VehicleData vehicle)
        {
            if (vehicle.Materials == null || vehicle.Materials.Count == 0)
            {
                Debug.LogWarning($"No materials defined for vehicle type {vehicle.Type}");
                return;
            }

            var carRenderer = car.GetComponentInChildren<Renderer>();
            if (carRenderer == null)
            {
                Debug.LogWarning("No Renderer component found in the car prefab.");
                return;
            }

            carRenderer.material = vehicle.Materials[Random.Range(0, vehicle.Materials.Count)];
        }
    }
}