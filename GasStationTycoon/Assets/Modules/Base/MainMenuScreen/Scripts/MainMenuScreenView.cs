using CodeBase.Core.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenView : BaseScreenView
    {
        [SerializeField] private TMP_Text mainMenuScreenTitle;
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;

        public void SetupEventListeners(UnityAction onMainMenuButtonClicked, UnityAction onSettingsButtonClicked)
        {
            playButton.onClick.AddListener(onMainMenuButtonClicked);
            settingsButton.onClick.AddListener(onSettingsButtonClicked);
        }

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners()
        {
            playButton.onClick.RemoveAllListeners();
            settingsButton.onClick.RemoveAllListeners();
        }
    }
}