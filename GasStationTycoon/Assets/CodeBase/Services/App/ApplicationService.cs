using System;
using CodeBase.CustomDIContainer;
using UnityEngine;

namespace CodeBase.Services.App
{
    public class ApplicationService : MonoBehaviour, IApplicationService
    {
        public event Action ApplicationStart;
        public event Action ApplicationPause;
        public event Action ApplicationResume;
        public event Action ApplicationFocus;
        public event Action ApplicationFocusLost;
        
        
        private void Awake()
        {
            // Debug.Log("Application started.");
            ApplicationStart?.Invoke();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                ApplicationPause?.Invoke();
            else
                ApplicationResume?.Invoke();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus) 
                ApplicationFocus?.Invoke();
            else
                ApplicationFocusLost?.Invoke();
        }
    }
}