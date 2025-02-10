using CodeBase.Core.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Template.TemplateScreen.Scripts
{
    public class TemplateScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text templateScreenTitle;
        
        public void SetupEventListeners(UnityAction onMainMenuButtonClicked)
        {
            mainMenuButton.onClick.AddListener(onMainMenuButtonClicked);
        }
        
        public void SetTitle(string title)
        {
            if(templateScreenTitle != null)
                templateScreenTitle.text = title;
            else
                Debug.LogWarning("templateScreenTitle is not assigned in the Inspector.");
        }

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners()
        {
            mainMenuButton.onClick.RemoveAllListeners();
        }
    }
}