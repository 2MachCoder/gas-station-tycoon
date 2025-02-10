using System;
using System.Threading.Tasks;

namespace CodeBase.Core.Patterns.Architecture.MVP
{
    public interface IView : IDisposable
    {
        public Task Show();

        public Task Hide();

        public void HideInstantly();
    }
}