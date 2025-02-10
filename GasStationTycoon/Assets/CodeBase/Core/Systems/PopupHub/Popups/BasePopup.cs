using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.UI;
using CodeBase.Core.UI.Views.Animations;
using CodeBase.CustomDIContainer;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Core.Systems.PopupHub.Popups
{
    public class BasePopup : MonoBehaviour
    {
        [Inject][HideInInspector] public BasePopupCanvas Canvas;
        //[Inject] protected ISoundService soundService;
        [Inject] protected IPopupHub PopupHub;

        [SerializeField] protected Transform overlayTransform;
        [SerializeField] protected Transform spinnerTransform;
        [SerializeField] private BaseShowHideAnimation showHideAnimation;

        public Button closeButton;

        private TaskCompletionSource<bool> Tcs => _tcs ??= new TaskCompletionSource<bool>();
        private CancellationTokenSource _cts;

        [SerializeField] protected PopupsPriority priority = PopupsPriority.Medium;
        public PopupsPriority Priority => priority;

        private bool _isClosed;
        private TaskCompletionSource<bool> _tcs;
        private bool _isRotating;

        protected virtual void Awake()
        {
            gameObject.SetActive(false);

            if (closeButton != null)
                closeButton.onClick.AddListener(() => HandleCloseButtonClick());
        }

        private async void HandleCloseButtonClick()
        {
            try
            {
                await Close();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error while closing popup: {e}");
            }
        }

        protected async Task ShowSpinner()
        {
            overlayTransform.gameObject.SetActive(true);
            spinnerTransform.gameObject.SetActive(true);

            _cts = new CancellationTokenSource();

            _isRotating = true;
            _ = RotateSpinner(1.5f, _cts.Token);
        }

        private async Task RotateSpinner(float duration, CancellationToken token)
        {
            while (_isRotating && !token.IsCancellationRequested)
            {
                float elapsed = 0f;
                Quaternion startRotation = spinnerTransform.rotation;
                Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, -360);

                while (elapsed < duration && _isRotating)
                {
                    if (token.IsCancellationRequested) return;

                    elapsed += Time.deltaTime;
                    float t = elapsed / duration;
                    spinnerTransform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
                    await Task.Yield();
                }

                spinnerTransform.rotation = endRotation;
            }
        }

        protected void HideSpinner()
        {
            overlayTransform.gameObject.SetActive(false);
            spinnerTransform.gameObject.SetActive(false);
            StopRotation();
        }

        private void StopRotation()
        {
            _isRotating = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public virtual Task Open<T>(T param)
        {
            gameObject.SetActive(true);
            return showHideAnimation.Show();
        }

        public virtual async Task Close()
        {
            if (_isClosed) return;

            try
            {
                _isClosed = true;
                // soundService.Play(GeneralSoundTypes.GeneralPopupClose).Forget();

                await showHideAnimation.Hide();
                StopAllTweens(); // Replaces DOKill
            }
            finally
            {
                Tcs?.TrySetResult(true);
                Destroy(gameObject);
                PopupHub.NotifyPopupClosed();
            }
        }

        private void StopAllTweens()
        {
            // Reset any active transformations manually since DOKill was removed.
            transform.localScale = Vector3.one;
            transform.position = transform.position; // Ensures no lingering transformations.
        }

        public Task<bool> WaitForCompletion() => Tcs.Task;

        private void OnDestroy()
        {
            _isClosed = true;
            StopRotation();
        }
    }
}
