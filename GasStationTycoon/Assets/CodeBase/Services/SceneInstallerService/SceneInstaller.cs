using System.Collections.Generic;
using CodeBase.CustomDIContainer;
using UnityEngine;

namespace CodeBase.Services.SceneInstallerService
{
    public abstract class SceneInstaller : MonoBehaviour, ISceneInstaller
    {
        [SerializeField] private List<GameObject> objectsToDelete;

        public abstract void RegisterSceneDependencies(BaseDiContainer container);

        public void RemoveObjectsToDelete()
        {
            foreach (var objectToDelete in objectsToDelete) 
                Destroy(objectToDelete);
        }

        public virtual void InjectMonoBehaviours(IObjectResolver resolver) { }
    }
}