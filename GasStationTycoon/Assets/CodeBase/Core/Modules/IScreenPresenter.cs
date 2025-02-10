using System;
using System.Threading.Tasks;

namespace CodeBase.Core.Modules
{
    public interface IScreenPresenter : IDisposable
    {
        Task Enter(object param);
        Task Execute();
        Task Exit();
    }
}