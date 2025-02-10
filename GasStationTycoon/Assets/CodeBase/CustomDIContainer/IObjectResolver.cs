using System;
using UnityEngine;

namespace CodeBase.CustomDIContainer
{
    /// <summary>
    /// Interface defining methods for resolving dependencies and injecting them into MonoBehaviours.
    /// </summary>
    public interface IObjectResolver 
    {
        T Resolve<T>();
        object Resolve(Type serviceType);
        void InjectMonoBehaviour(MonoBehaviour monoBehaviour);
    }
}