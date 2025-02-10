using System;
using System.Collections;
using Configs.GamePlay.Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Cars
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CarController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private ParkingSlot _parkingSlot;
        private float _speed = 20f;
        public bool MoveToParkingSlot { get; set; }
        public VehicleType Type { get; set; }

        public event Action<CarController, VehicleType> OnFinishPointReached;
        public event Action<CarController, VehicleType> OnActionCompleted;
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.baseOffset = 0;
            _agent.speed = _speed;
        }

        public void MoveTo(Vector3 destination,  Quaternion rotation)
        {
            _agent.SetDestination(destination); 
            _agent.isStopped = false;
            _agent.speed = _speed;
            _agent.stoppingDistance = 10f;
            
            StartCoroutine(WaitForDestination(rotation));
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<DespawnZone>())
            {
                _agent.speed = 0f;
                _agent.isStopped = true;
                OnFinishPointReached?.Invoke(this, Type);
            }
        }

        public void ActionCompleted()
        {
            StopCoroutine(WaitForDestination(Quaternion.identity));
            _agent.ResetPath();
            OnActionCompleted?.Invoke(this, Type);
        }

        public void OnDespawned()
        {
            _agent.speed = _speed;
            _agent.isStopped = false;
            _agent.ResetPath();
        }
        
        private IEnumerator WaitForDestination(Quaternion rotation)
        {
            while (true)
            {
                if (!_agent.pathPending &&
                    _agent.remainingDistance <= _agent.stoppingDistance &&
                    (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f))
                {
                    _agent.speed = 0;
                    transform.rotation = rotation;
                    transform.position = _agent.destination;
                    yield break; // Shutdown the coroutine
                }
        
                yield return null;
            }
        }
    }
}