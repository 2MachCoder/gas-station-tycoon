using System.Collections;
using UnityEngine;

namespace CodeBase.Core.UI.Views.Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeShowHideAnimation : BaseShowHideAnimation
    {
        [SerializeField] protected CanvasGroup canvasGroup;

        [Header("Animation Parameters")]
        [SerializeField] private float fadeDuration = 0.4f;

        protected void Awake()
        {
            if (!canvasGroup)
                canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        protected override IEnumerator PlayShowAnimation()
        {
            yield return FadeCoroutine(0, 1, fadeDuration);
        }

        protected override IEnumerator PlayHideAnimation()
        {
            yield return FadeCoroutine(1, 0, fadeDuration);
        }

        private IEnumerator FadeCoroutine(float startAlpha, float targetAlpha, float duration)
        {
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }
    }
}