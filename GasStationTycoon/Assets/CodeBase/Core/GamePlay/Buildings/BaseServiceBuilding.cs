using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.UI.ProgressBars;
using UnityEngine;

namespace CodeBase.Core.GamePlay.Buildings
{
    public abstract class BaseServiceBuilding : MonoBehaviour
    {
        [SerializeField] protected BaseProgressBarView progressBar;
        [SerializeField] protected GameObject botPlace;
        [SerializeField] protected float timeToCompleteAction = 1.5f;
        [SerializeField] protected float animateToZeroTime = 0.5f;

        protected BasePlayerController PlayerController;
        protected bool _taskInProgress;
        private CancellationTokenSource _cts;
        private bool _requiredActionCompleted;

        public bool TaskInProgress => _taskInProgress;
        public GameObject BotPlace => botPlace;

        protected virtual async void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out BasePlayerController player) && player == PlayerController)
            {
                // Debug.Log(other.gameObject.name + " has exited");
                _cts?.Cancel();

                PlayerController = null;
                
                if (!_requiredActionCompleted)
                    await progressBar.AnimateToZero(animateToZeroTime, progressBar.CurrentRatio);
            }
        }
        
        protected async Task ExecuteTask(BasePlayerController playerController = null)
        {
            _taskInProgress = true;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                await progressBar.Animate(timeToCompleteAction, token);

                if (!token.IsCancellationRequested)
                {
                    progressBar.ReportToZero(progressBar.CurrentRatio);
                    progressBar.ResetProgress();
                    CompleteAction(playerController);
                }
                
            }
            catch (TaskCanceledException)
            {
                _taskInProgress = false;
                _requiredActionCompleted = false;
                 
            }
            finally
            {
                _cts.Dispose();
                _cts = null;
            }
        }

        protected abstract void CompleteAction(BasePlayerController playerController = null);
    }
}