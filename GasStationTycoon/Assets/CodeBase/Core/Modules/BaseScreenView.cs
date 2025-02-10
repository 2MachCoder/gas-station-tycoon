using System.Threading.Tasks;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Core.UI.Views.Animations;
using UnityEngine;

namespace CodeBase.Core.Modules
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    public abstract class BaseScreenView : MonoBehaviour, IView
    {
        [SerializeField] private BaseShowHideAnimation showHideShowHideAnimation;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
        }

        public virtual async Task Show()
        {
            SetActive(true);
            if (showHideShowHideAnimation != null) 
                await showHideShowHideAnimation.Show();
        }

        public virtual async Task Hide()
        {
            if (showHideShowHideAnimation != null) 
                await showHideShowHideAnimation.Hide();
            SetActive(false);
        }
        
        protected void SetActive(bool isActive)
        {
            _canvas.enabled = isActive;
            _canvasGroup.alpha = isActive ? 1 : 0;
            _canvasGroup.blocksRaycasts = isActive;
            _canvasGroup.interactable = isActive;
            
            gameObject.SetActive(isActive);
        }
        
        public void HideInstantly()
        {
            gameObject.SetActive(false);
        }

        public virtual void Dispose()
        {
            if (this != null) 
                Destroy(gameObject);
        } 
    }
}