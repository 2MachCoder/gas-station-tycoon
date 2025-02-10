using System;
using System.Collections.Generic;
using CodeBase.CustomDIContainer;
using CodeBase.Implementation.Systems.DataPersistenceSystem;
using CodeBase.Services.App;
using CodeBase.Systems.DataPersistenceSystem.Data;
using CodeBase.Systems.Gameplay;
using UnityEngine;

namespace CodeBase.Systems.DataPersistenceSystem
{
    public class DataPersistenceManager : IDataPersistenceManager, IDisposable
    {
        private const string SaveFileName = "persistenceSaveData";
        private const bool UseEncryption = false;
        [Inject] private readonly IApplicationService _applicationService;
        [Inject] private readonly GameDataMediatorSystem _gameDataMediatorSystem;
        [Inject] private readonly CurrencySystem _currencySystem;
        //[Inject] private readonly AudioSystem _audioSystem;
        private readonly RootDiContainer _container = RootDiContainer.Instance;
        private readonly List<IDataPersistence> _dataPersistenceObjects = new();
        private readonly FileDataHandler _dataHandler;
        private SaveData _saveData;

        public DataPersistenceManager()
        {
            _container.Inject(this);

            //TODO: make ApplicationService provide Application.persistentDataPah 
            _dataHandler = new FileDataHandler(Application.persistentDataPath, SaveFileName, UseEncryption);

            _applicationService.ApplicationStart += OnApplicationStart;
            _applicationService.ApplicationPause += OnApplicationPause; 
            _applicationService.ApplicationFocusLost += OnApplicationFocusLost;

            SetDataPersistenceObjects();
            LoadData();
        }

        private void OnApplicationStart() => LoadData();

        private void OnApplicationPause() => SaveData();

        private void OnApplicationFocusLost() => SaveData();

        private void ResetSaveData() => _saveData = new SaveData();

        private void LoadData()
        {
            _saveData = _dataHandler.Load();
            
            if(_saveData == null) ResetSaveData();
            
            foreach (var dataPersistenceObject in _dataPersistenceObjects) 
                dataPersistenceObject.LoadData(_saveData);
        }

        private void SaveData()
        {
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
                dataPersistenceObject.SaveData(ref _saveData);
            _dataHandler.Save(_saveData);
        }

        private void SetDataPersistenceObjects()
        {
            _dataPersistenceObjects.Add(_gameDataMediatorSystem);
            _dataPersistenceObjects.Add(_currencySystem);
        }

        public void Dispose()
        {
            _applicationService.ApplicationStart -= OnApplicationStart;
            _applicationService.ApplicationPause -= OnApplicationPause;
            _applicationService.ApplicationFocusLost -= OnApplicationFocusLost;
        }
    }
}
