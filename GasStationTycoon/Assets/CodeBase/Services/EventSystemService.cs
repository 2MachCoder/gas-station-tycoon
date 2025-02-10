using CodeBase.CustomDIContainer;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.Services
{
    public class EventSystemService : IStartable
    {
        private const string EventSystemName = "EventSystem";
        
        public void Start()
        {
            var eventSystem = CreateEventSystem();
            Object.DontDestroyOnLoad(eventSystem.gameObject);
        }

        private static EventSystem CreateEventSystem()
        {
            var eventSystem = new GameObject(EventSystemName).AddComponent<EventSystem>();
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            Object.DontDestroyOnLoad(eventSystem.gameObject);
            return eventSystem;
        }
    }
    
    /* //TODO Transfer to NewInputSystem
    public class EventSystemService : IStartable
    {
        private const string EventSystemName = "EventSystem";
        
        public void Start()
        {
            var eventSystem = CreateEventSystem();
            Object.DontDestroyOnLoad(eventSystem.gameObject);
        }

        private static EventSystem CreateEventSystem()
        {
            var eventSystem = new GameObject(EventSystemName).AddComponent<EventSystem>();

            var inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();

            var inputActions = new InputActionAsset();
            inputModule.actionsAsset = inputActions;

            Object.DontDestroyOnLoad(eventSystem.gameObject);
            return eventSystem;
        }
    }
    */
}