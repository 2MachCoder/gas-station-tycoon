using System;

namespace CodeBase.Services.App
{
    public interface IApplicationService
    {
        public event Action ApplicationStart;
        public event Action ApplicationPause;
        public event Action ApplicationResume;
        public event Action ApplicationFocus;
        public event Action ApplicationFocusLost;
    }
}