using System.ComponentModel;
using System.Windows;

namespace Helpers.WPF
{
    public static class GuiHelper
    {
        public static void AutomaticRuntimeSize(this FrameworkElement fwe_in)
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                fwe_in.Width = double.NaN;
                fwe_in.Height = double.NaN;
            }
        }
    }
}