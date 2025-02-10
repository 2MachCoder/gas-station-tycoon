using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Core.UI.Views.Animations
{
    public class FlickerAnimation
    {
        private readonly CanvasGroup _lightingCanvasGroup;
        private readonly Image _overlay;
        private const float FlickerDuration = 0.2f;
        private bool _isActive;

        public FlickerAnimation(CanvasGroup lightingCanvasGroup, Image overlay)
        {
            _lightingCanvasGroup = lightingCanvasGroup;
            _overlay = overlay;
        }

        public IEnumerator StartFlickering()
        {
            _isActive = true;

            while (_isActive)
            {
                float opacity = UnityEngine.Random.Range(0f, 0.3f);

                yield return FadeCanvasGroup(_lightingCanvasGroup, opacity, FlickerDuration);
                yield return FadeImageAlpha(_overlay, 1 - opacity, FlickerDuration);

                yield return FadeCanvasGroup(_lightingCanvasGroup, 1, FlickerDuration);
                yield return FadeImageAlpha(_overlay, 0, FlickerDuration);

                yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.8f));
            }
        }

        public void StopFlickering()
        {
            _isActive = false;
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }

        private IEnumerator FadeImageAlpha(Image image, float targetAlpha, float duration)
        {
            Color startColor = image.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, targetAlpha, elapsed / duration));
                yield return null;
            }

            image.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        }
    }
}
