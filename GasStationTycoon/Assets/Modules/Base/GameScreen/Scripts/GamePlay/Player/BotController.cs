using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Core.GamePlay;
using CodeBase.Core.GamePlay.Buildings;
using CodeBase.CustomDIContainer;
using Modules.Base.GameScreen.Scripts.GamePlay.Buildings;
using Modules.Base.GameScreen.Scripts.GamePlay.Buildings.ServiceBuildings;
using Modules.Base.GameScreen.Scripts.GamePlay.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Player
{
    public class BotController : BasePlayerController
    {
        [Inject] private BuildingsManager _buildingsManager;
        
        private NavMeshAgent _navMeshAgent;
        private List<ServiceBuilding> _serviceBuildings = new();
        private Vector3 _currentDestinationForTask;
        private bool _hasTask;
        private BaseServiceBuilding _currentServiceBuilding;
        private ATM _atm;
        private int _tasksDone;
        private BaseServiceBuilding _onTriggerServiceBuilding;
        
        public override void Initialize()
        {
            RootDiContainer.Instance.InjectMonoBehaviour(this);
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _serviceBuildings = _buildingsManager.ServiceBuildings;
            _atm = _buildingsManager.Atm;
            _atm.OnMoneyPutOnBalance += LookForNewTask;
            _buildingsManager.OnServiceBuildingAdded += AddServiceBuilding;
            foreach (var building in _serviceBuildings)
            {
                building.OnTaskHasBeenStarted += ChangeTask;
                building.OnTaskCompleted += LookForNewTask;
            }

            StartCoroutine(CheckForAvailableTask());
        }
        
        private void SetTask(Vector3 destination) => _navMeshAgent.SetDestination(destination);

        private void AddServiceBuilding(ServiceBuilding serviceBuilding)
        {
            _serviceBuildings.Add(serviceBuilding);
            serviceBuilding.OnTaskCompleted += LookForNewTask;
            serviceBuilding.OnTaskHasBeenStarted += ChangeTask;
        }

        private IEnumerator CheckForAvailableTask()
        {
            while (_hasTask == false)
            {
                Debug.Log("Waiting for tasks");
                var buildingsWithAvailableTasks = _serviceBuildings.Where(x => x.HasTaskToBeDone)
                    .Where(x => !x.TaskInProgress).ToList();

                if (buildingsWithAvailableTasks.Count != 0 && _hasTask == false)
                {
                    _currentServiceBuilding = buildingsWithAvailableTasks
                        .OrderBy(building => Vector3.Distance(building.transform.position, transform.position))
                        .First();
                    _hasTask = true;
                    Debug.Log(_hasTask);
                    _currentDestinationForTask = _currentServiceBuilding.BotPlace.transform.position;
                    //Debug.Log("Task is assigned");
                    _navMeshAgent.speed = 10f;
                    SetTask(_currentDestinationForTask);
                    break;
                }

                yield return new WaitForSeconds(1f);
            }
        }

        private void LookForNewTask(BaseServiceBuilding serviceBuilding)
        {
            if ((_currentServiceBuilding == serviceBuilding || _onTriggerServiceBuilding == serviceBuilding) && _tasksDone != 3)
            {
                if (_currentServiceBuilding!=_atm)
                    _tasksDone++;

                Debug.Log(_tasksDone);
                _hasTask = false;
                _navMeshAgent.ResetPath();
                Debug.Log("Look for a new task");
                StopAllCoroutines();
                StartCoroutine(CheckForAvailableTask());
            }

            if (_tasksDone == 3)
            {
                StopAllCoroutines();
                GoToAtm(_atm.BotPlace.transform.position);
                _tasksDone = 0;
            }
        }

        private void ChangeTask(BaseServiceBuilding serviceBuilding, BasePlayerController playerController)
        {
            Debug.Log("Change Task");
            if (_currentServiceBuilding == serviceBuilding && playerController != this || !_hasTask)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = 0;
                _hasTask = false;
                StopAllCoroutines();
                StartCoroutine(CheckForAvailableTask());
            }
        }

        private void GoToAtm(Vector3 destination)
        {
            _currentServiceBuilding = _atm;
            _navMeshAgent.SetDestination(destination);
        }

        private void OnTriggerEnter(Collider other)
        {
            var triggeredObject = other.GetComponent<BaseServiceBuilding>();
            if (triggeredObject != null)
                _onTriggerServiceBuilding = triggeredObject;
        }
    }
}