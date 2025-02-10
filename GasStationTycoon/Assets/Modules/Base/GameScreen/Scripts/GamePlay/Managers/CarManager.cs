using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.CustomDIContainer;
using Configs.GamePlay.Scripts;
using Modules.Base.GameScreen.Scripts.GamePlay.Cars;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Managers
{
    public class CarManager : MonoBehaviour
    {
        [SerializeField] private GameObject spawnPointLane1;
        [SerializeField] private GameObject endPointLane1;
        [SerializeField] private GameObject spawnPointLane2;
        [SerializeField] private GameObject endPointLane2;

        [Inject] private BuildingsManager _buildingsManager;
        [Inject] private CarPool _carPool;

        private List<ParkingSlot> _parkingSlots = new();
        private List<CarController> _cars = new();
        private bool _carSpawnEnabled;
        
        public void Initialize()
        {
            RootDiContainer.Instance.InjectMonoBehaviour(this);
            _carSpawnEnabled = true;
            _buildingsManager.OnParkingSlotAdded += AddParkingSlot;
            StartCoroutine(SpawnDecorationCars());
            StartCoroutine(CheckFreeSpotsAndSpawnCar());
        }

        private void AddParkingSlot(ParkingSlot parkingSlot)
        {
            _parkingSlots.Add(parkingSlot);
        }

        //TODO Parameters Refactoring
        private void SpawnCar(VehicleType vehicleType, Vector3 spawnPosition, Transform targetPosition)
        {
            var car = _carPool.SpawnCar(vehicleType, spawnPosition);
            car.transform.SetParent(targetPosition.parent);
            car.Type = vehicleType;
            _cars.Add(car);
            car.OnActionCompleted += MoveCarToFinalPoint;
            car.OnFinishPointReached += DespawnCar;
            car.transform.position = spawnPosition;
            car.MoveTo(targetPosition.transform.position, targetPosition.rotation);
        }
        
        private void MoveCarToFinalPoint(CarController car,  VehicleType vehicleType)
        {
            var finalPosition = endPointLane2.transform;
            car.MoveTo(finalPosition.position, finalPosition.rotation);
        }

        public void DespawnCar(CarController car, VehicleType vehicleType)
        {
            car.OnFinishPointReached -= DespawnCar;
            car.OnActionCompleted -= MoveCarToFinalPoint;
            car.OnDespawned();
            _carPool.DespawnCar(car, vehicleType);
            _cars.Remove(car);
        }

        private VehicleType RandomCarType()
        {
            Array values = Enum.GetValues(typeof(VehicleType));
            return (VehicleType)values.GetValue(Random.Range(0, values.Length));
        }

        private IEnumerator SpawnDecorationCars()
        {
            while (_carSpawnEnabled)
            {
                SpawnCar(RandomCarType(), spawnPointLane1.transform.position, endPointLane1.transform);
                yield return new WaitForSeconds(Random.Range(2f, 5f));
                SpawnCar(RandomCarType(), spawnPointLane2.transform.position, endPointLane2.transform);
                yield return new WaitForSeconds(Random.Range(2f, 5f));
            }
        }

        private IEnumerator CheckFreeSpotsAndSpawnCar()
        {
            while (_carSpawnEnabled)
            {
                var freeParkingSlots = _parkingSlots.Where(x => x.IsFree);
                
                if (_parkingSlots != null && freeParkingSlots.Any()) 
                    SpawnCarsForFreeSlots();

                yield return new WaitForSeconds(1f);
            }
        }

        private void SpawnCarsForFreeSlots()
        {
            foreach (var parkingSlot in _parkingSlots.Where(parkingSlot => parkingSlot.IsFree))
            {
                SpawnCar(RandomCarType(), spawnPointLane2.transform.position,
                    parkingSlot.CarPosition.transform);
                parkingSlot.IsFree = false;
            }
        }

        public void Shutdown()
        {
            _carSpawnEnabled = false;
            StopAllCoroutines();
            //TODO Resolve bug with foreach below
            // foreach (var car in _cars) DespawnCar(car, car.Type);
        }
    }
}