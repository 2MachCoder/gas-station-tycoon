using System.Threading.Tasks;

namespace CodeBase.Core.UI.Views.Animations
{
    public interface IAnimationElement
    {
        Task Show();
        Task Hide();
    }
}