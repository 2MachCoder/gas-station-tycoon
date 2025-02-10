using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.Patterns.ObjectCreation;
using CodeBase.Core.Systems.PopupHub;
using CodeBase.Core.Systems.PopupHub.Popups;
using CodeBase.Core.UI;
using CodeBase.CustomDIContainer;
using UnityEngine;

namespace CodeBase.Systems.PopupHub
{
    //TODO Waits for fix
    public class PopupHub : IPopupHub
    {
        [NonSerialized] public BasePopup CurrentPopup;

        // [Inject] private BasePopupCanvas _canvas;
        // [Inject] private IBasePopupFactory<SettingsPopup> _firstPopupFactory;

        private readonly PopupsPriorityQueue _popup = new();
        private readonly Stack<BasePopup> _popups = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public PopupHub()
        {
            RootDiContainer.Instance.Inject(this);
        }

        private void CreateAndOpenPopup(IFactory<Transform, BasePopup> basePopupFactory)
        {
            // var popup = basePopupFactory.Create(_canvas.PopupParent);
            // EnqueuePopup(popup);
        }

        private void CreateAndOpenPopup<T>(IFactory<Transform, BasePopup> basePopupFactory, T param)
        {
            // var popup = basePopupFactory.Create(_canvas.PopupParent);
            // EnqueuePopup(popup, param);
        }

        private void EnqueuePopup(BasePopup popup)
        {
            _popup.Enqueue(popup); 
            TryOpenNextPopup();
        }

        private void EnqueuePopup<T>(BasePopup popup, T param)
        {
            _popup.Enqueue(popup);  
            TryOpenNextPopup(param);
        }

        public async Task TryOpenNextPopup() => await TryOpenNextPopup<object>(null);

        public async Task TryOpenNextPopup<T>(T param)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (CurrentPopup == null && _popup.TryDequeue(out var nextPopup)) 
                {
                    CurrentPopup = nextPopup;
                    _popups.Push(CurrentPopup);
                    CurrentPopup.gameObject.SetActive(true);
                    await CurrentPopup.Open(param);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while opening popup '{CurrentPopup.name}' : {ex.Message}");
            }
            finally { _semaphoreSlim.Release(); }
        }


        public async Task CloseCurrentPopup()
        {
            if (CurrentPopup != null)
            {
                await CurrentPopup.Close();
                CurrentPopup = null;
                TryOpenNextPopup();  
            }
        }

        public void NotifyPopupClosed()
        {
            CurrentPopup = null;
            TryOpenNextPopup();
        }

        // public void OpenSettingsPopup() => CreateAndOpenPopup(_firstPopupFactory);
    }
}
