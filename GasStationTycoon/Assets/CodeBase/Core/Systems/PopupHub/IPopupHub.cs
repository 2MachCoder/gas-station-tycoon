using System.Threading.Tasks;

namespace CodeBase.Core.Systems.PopupHub
{
    public interface IPopupHub
    {
        Task TryOpenNextPopup();
        Task TryOpenNextPopup<T>(T param);
        Task CloseCurrentPopup();
        void NotifyPopupClosed();
        // void OpenSettingsPopup();
    }
}