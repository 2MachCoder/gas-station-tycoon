using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Core.UI.Views.Animations
{
    public abstract class BaseShowHideAnimation : MonoBehaviour, IAnimationElement
    {
        public Task Show() => RunCoroutineAsTask(PlayShowAnimation());
        public Task Hide() => RunCoroutineAsTask(PlayHideAnimation());

        protected abstract IEnumerator PlayShowAnimation();
        protected abstract IEnumerator PlayHideAnimation();

        private Task RunCoroutineAsTask(IEnumerator coroutine)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (this == null || !gameObject.activeInHierarchy)
            {
                tcs.SetResult(false);
                return tcs.Task;
            }

            StartCoroutine(ExecuteCoroutine(coroutine, tcs));
            return tcs.Task;
        }

        private IEnumerator ExecuteCoroutine(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
        {
            yield return StartCoroutine(SafeCoroutine(coroutine));
            if (this != null) 
                tcs.SetResult(true);
        }

        private IEnumerator SafeCoroutine(IEnumerator coroutine)
        {
            while (coroutine.MoveNext())
            {
                if (this == null || !gameObject.activeInHierarchy)
                    yield break;
                
                yield return coroutine.Current;
            }
        }
    }
}