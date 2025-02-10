using System.Collections.Generic;
using CodeBase.CustomDIContainer;
using Configs.GamePlay.Scripts;
using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Cars
{
    public class CarPool
    {
        [Inject] private readonly CarFactory _carFactory;
        private readonly Dictionary<VehicleType, Queue<CarController>> _carPools = new();

        public CarPool()
        {
            RootDiContainer.Instance.Inject(this);
        }

        public CarController SpawnCar(VehicleType vehicleType, Vector3 position)
        {
            if (_carPools.TryGetValue(vehicleType, out var carsQueue) && carsQueue.Count > 0)
            {
                var car = carsQueue.Dequeue();
                car.gameObject.SetActive(true);
                return car;
            }
            
            return _carFactory.Create(vehicleType, position);
        }

        public void DespawnCar(CarController car, VehicleType vehicleType)
        {
            if (!_carPools.ContainsKey(vehicleType))
            {
                _carPools[vehicleType] = new Queue<CarController>();
            }

            car.gameObject.SetActive(false);
            _carPools[vehicleType].Enqueue(car);
        }
    }
}