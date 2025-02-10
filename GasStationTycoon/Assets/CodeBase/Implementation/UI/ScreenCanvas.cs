using CodeBase.Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Implementation.UI
{
    /// <summary>
    /// Implementation of ScreenCanvas inheriting from BaseScreenCanvas.
    /// </summary>
    public class ScreenCanvas : BaseScreenCanvas
    {
        /// <summary>
        /// Specific initialization logic for ScreenCanvas.
        /// </summary>
        public override void InitializeCanvas()
        {
            // Add specific initialization logic here if needed.
            Debug.Log("ScreenCanvas initialized.");
        }
    }
}