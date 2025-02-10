using CodeBase.CustomDIContainer;
using UnityEngine;

namespace CodeBase.Services
{
    public class AudioListenerService
    {
        private const string MainCameraName = "MainCamera";

        public static void EnsureAudioListenerExists(BaseDiContainer resolver)
        {
            var mainCamera = resolver.Resolve<Camera>();
            if (mainCamera == null)
            {
                mainCamera = new GameObject(MainCameraName).AddComponent<Camera>();
                resolver.Inject(mainCamera);
            }

            var audioListener = mainCamera.GetComponent<AudioListener>();
            if (audioListener == null)
                mainCamera.gameObject.AddComponent<AudioListener>();
        }
    }
}