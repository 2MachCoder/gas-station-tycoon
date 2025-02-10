using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Core.UI.ProgressBars
{
    public abstract class BaseProgressBarView : MonoBehaviour, IProgress<float>
    {
        public abstract void Report(float value);
        public abstract void ReportToZero(float value);

        public bool canAnimate;
        public bool canAnimateToZero;
        private float _currentRatio;
        
        public float CurrentRatio => _currentRatio;

        // Track the active animation task
        private Task _currentAnimationTask;
        private CancellationTokenSource _cts = new();

        public async Task Animate(float duration, CancellationToken token, float value = 1f)
        {
            canAnimate = true;
            var ratio = _currentRatio;
            var multiplier = value / duration;

            try
            {
                while (ratio < value && canAnimate)
                {
                    if (token.IsCancellationRequested) break;

                    _currentRatio = ratio;
                    ratio += Time.deltaTime * multiplier;
                    Report(ratio);
                    await Task.Yield();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Animation error: {e}");
            }
            finally
            {
                canAnimate = false;
                _currentRatio = 0;
            }
        }

        public async Task AnimateToZero(float duration, float currentValue)
        {
            canAnimateToZero = true;
            var ratio = currentValue;
            var multiplier = currentValue / duration;

            try
            {
                while (ratio > 0 && canAnimateToZero)
                {
                    ratio -= Time.deltaTime * multiplier;
                    ReportToZero(ratio);
                    await Task.Yield();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Animation to zero error: {e}");
            }
            finally
            {
                canAnimateToZero = false;
            }
        }

        public void PauseAnimate()
        {
            canAnimate = false;
        }

        public void ResetProgress()
        {
            _currentRatio = 0;
        }

        public async void ResumeAnimation()
        {
            if (canAnimate) return;

            canAnimate = true;

            try
            {
                _cts = new CancellationTokenSource();
                await Animate(1f, _cts.Token, _currentRatio);
            }
            catch (Exception e)
            {
                Debug.LogError($"Resume animation error: {e}");
            }
        }

    }
}
