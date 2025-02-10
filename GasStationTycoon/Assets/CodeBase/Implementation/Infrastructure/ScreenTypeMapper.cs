using System;
using System.Collections.Generic;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.CustomDIContainer;
using Modules.Base.GameScreen.Scripts;
using Modules.Base.MainMenuScreen.Scripts;

namespace CodeBase.Implementation.Infrastructure
{
    public class ScreenTypeMapper
    {
        private readonly Dictionary<ScreenPresenterMap, Type> _map;

        public ScreenTypeMapper()
        {
            _map = new Dictionary<ScreenPresenterMap, Type> 
            {
                { ScreenPresenterMap.Game, typeof(GameScreenPresenter) },
                { ScreenPresenterMap.MainMenu, typeof(MainMenuScreenPresenter) },
                // { ScreenPresenterMap.MainMenu, typeof(MainMenuScreenPresenter) },
                // { ScreenPresenterMap.Bootstrap, typeof(BootstrapScreenPresenter) }
            };
        }

        public IScreenPresenter Resolve(ScreenPresenterMap screenPresenterMap, IObjectResolver objectResolver) => 
            (IScreenPresenter)objectResolver.Resolve(_map[screenPresenterMap]);
    }
}