using System.Collections;
using UnityEngine;

namespace CodeBase.Core.UI.Views.Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    // TODO There might be problems when there is ScrollView element on view. Don't use if it's so.
    public class BubbleFadeShowHideAnimation : BaseShowHideAnimation
    {
        [SerializeField] protected CanvasGroup canvasGroup;

        [Header("Animation Parameters")]
        [SerializeField] private float scaleUpFactor = 1.1f;
        [SerializeField] private float scaleDuration = 0.25f;
        [SerializeField] private float fadeDuration = 0.25f;

        protected void Awake()
        {
            if (!canvasGroup)
                TryGetComponent(out canvasGroup);
            if (!canvasGroup)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        protected override IEnumerator PlayShowAnimation()
        {
            yield return BubbleFadeCoroutine(Vector3.zero, Vector3.one, fadeDuration, scaleDuration);
        }

        protected override IEnumerator PlayHideAnimation()
        {
            yield return BubbleFadeCoroutine(Vector3.one, Vector3.zero, fadeDuration, scaleDuration);
        }

        private IEnumerator BubbleFadeCoroutine(Vector3 startScale, Vector3 endScale, float fadeTime, float scaleTime)
        {
            float elapsedTime = 0;
            float scaleHalfTime = scaleTime / 2;
            float fadeHalfTime = fadeTime / 2;

            // 1: increase of scale and start of animation
            while (elapsedTime < scaleHalfTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / scaleHalfTime;
                transform.localScale = Vector3.Lerp(startScale, startScale * scaleUpFactor, t);
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeHalfTime);
                yield return null;
            }

            // 2: reduce scale to 1
            elapsedTime = 0;
            while (elapsedTime < scaleHalfTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / scaleHalfTime;
                transform.localScale = Vector3.Lerp(startScale * scaleUpFactor, endScale, t);
                yield return null;
            }

            transform.localScale = endScale;
            canvasGroup.alpha = (endScale == Vector3.zero) ? 0 : 1;
        }
    }
}
