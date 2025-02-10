using System;
using CodeBase.Core.UI.ProgressBars;
using UnityEngine.UI;

namespace CodeBase.Implementation.UI.ProgressBars
{
    public class ProgressBarView : BaseProgressBarView
    {
        public Image image;
        public override void Report(float value)
        {
            if (image != null)
                image.fillAmount = Math.Min(value, 1f); //Ensures that value does not exceed 1 (the maximum fill amount)
        }
        public override void ReportToZero(float value)
        {
            if (image != null)
                image.fillAmount = Math.Max(value, 0f);
        }
    }
}