using System;
using CodeBase.CustomDIContainer;
using CodeBase.Systems.Gameplay;
using Modules.Base.GameScreen.Scripts.GamePlay.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Buildings
{
    //TODO Transfer creating bot logic to other manager
    public class MainBuilding : MonoBehaviour
    {
        private const float StockUpgradePrice = 100f;
        private const float PriceMultiplier = 0.3f;
        private const int BotSpawnLevel = 3;

        [Header("UI Elements")]
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private BotController botControllerPrefab;

        [Inject] private CurrencySystem _currencySystem;

        private int _level;
        private TextMeshProUGUI _buttonText;
        private bool _wasBotSpawned;
        private BotController _spawnedBot;

        public event Action<int> OnLevelUp;

        public int Level { get; private set; }

        public void Initialize(int currentLevel)
        {
            RootDiContainer.Instance.InjectMonoBehaviour(this);
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
            _buttonText = upgradeButton.GetComponentInChildren<TextMeshProUGUI>();

            Level = currentLevel;
            UpdateButtonState();
            UpdateLevelText();
            
            if (!_wasBotSpawned && Level >= BotSpawnLevel)
                SpawnBot();
        }

        public void ShutDown()
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
            
            _buttonText = null;
            _currencySystem = null;

            if (_spawnedBot != null)
            {
                Destroy(_spawnedBot.gameObject);
                _spawnedBot = null;
            }

            OnLevelUp = null;
        }

        private void OnUpgradeButtonClicked()
        {
            if (!_currencySystem.TrySpendMoney((int)(StockUpgradePrice * PriceMultiplier * (Level + 1))))
                return;

            Level++;
            OnLevelUp?.Invoke(Level);
            UpdateButtonState();
            UpdateLevelText();

            if (!_wasBotSpawned && Level == BotSpawnLevel)
                SpawnBot();
        }

        private void SpawnBot()
        {
            _wasBotSpawned = true;
            _spawnedBot = Instantiate(botControllerPrefab, Vector3.zero, Quaternion.identity);
            _spawnedBot.transform.SetParent(transform.parent);
            _spawnedBot.Initialize();
        }

        private void UpdateButtonState() =>
            _buttonText.text = "Upgrade " + (int)(StockUpgradePrice * PriceMultiplier * (Level + 1));

        private void UpdateLevelText() => levelText.text = "Level " + Level;
    }
}
