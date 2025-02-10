using CodeBase.CustomDIContainer;

namespace CodeBase.Services.SceneInstallerService
{
    public interface ISceneInstaller
    {
        public void RegisterSceneDependencies(BaseDiContainer container);

        public void RemoveObjectsToDelete();

        public void InjectMonoBehaviours(IObjectResolver resolver);
    }
}