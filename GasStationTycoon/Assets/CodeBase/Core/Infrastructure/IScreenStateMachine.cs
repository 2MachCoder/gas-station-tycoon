using System.Threading.Tasks;
using CodeBase.Core.Modules;

namespace CodeBase.Core.Infrastructure
{
    public interface  IScreenStateMachine
    {
        public IScreenPresenter CurrentPresenter { get; }
        
        Task RunScreen(ScreenPresenterMap screenPresenterMap, object param = null);
    }
}