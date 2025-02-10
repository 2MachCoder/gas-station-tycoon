using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Core.UI.Buttons
{
    public class PulsatingButton : MonoBehaviour
    {
        public Button pulsatingButton;
        private Coroutine _animationCoroutine;
        private bool _isAnimating = false;

        public void PlayAnimation()
        {
            if (_isAnimating) return;

            _isAnimating = true;
            _animationCoroutine = StartCoroutine(PulseAnimation());
        }

        public void StopAnimation()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
            
            _isAnimating = false;
            pulsatingButton.transform.localScale = Vector3.one; // Reset to default scale
        }

        private IEnumerator PulseAnimation()
        {
            while (_isAnimating)
            {
                yield return StartCoroutine(ScaleButton(1.2f, 0.3f)); // Scale up
                yield return StartCoroutine(ScaleButton(1.0f, 0.3f)); // Scale down
            }
        }

        private IEnumerator ScaleButton(float targetScale, float duration)
        {
            float elapsed = 0f;
            Vector3 initialScale = pulsatingButton.transform.localScale;
            Vector3 targetVector = Vector3.one * targetScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                pulsatingButton.transform.localScale = Vector3.Lerp(initialScale, targetVector, t);
                yield return null;
            }

            pulsatingButton.transform.localScale = targetVector; // Ensure final scale is set correctly
        }

        private void OnDestroy()
        {
            StopAnimation();
        }
    }
}