using CodeBase.Core.Patterns.ObjectCreation;
using CodeBase.CustomDIContainer;
using UnityEngine;

namespace CodeBase.Core.Systems.PopupHub.Popups
{
    public interface IBasePopupFactory<out T> : IFactory<Transform, T> where T : BasePopup { }

    public class BasePopupFactory<T> : IBasePopupFactory<T> where T : BasePopup
    {
        private readonly RootDiContainer _container;
        private readonly T _basePopupFactoryPrefab;

        public BasePopupFactory(RootDiContainer container, T basePopupFactoryPrefab)
        {
            _container = container;
            _basePopupFactoryPrefab = basePopupFactoryPrefab;
        }

        public virtual T Create(Transform parentTransform)
        {
            var popupInstance = Object.Instantiate(_basePopupFactoryPrefab, parentTransform);
            
            _container.InjectMonoBehaviour(popupInstance);

            popupInstance.gameObject.SetActive(false);
            return popupInstance;
        }
    }
}