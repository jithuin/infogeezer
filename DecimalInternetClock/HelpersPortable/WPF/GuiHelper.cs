using GalaSoft.MvvmLight;
using System.ComponentModel;
using System.Windows;
using Windows.UI.Xaml;

namespace HelpersPortable.WPF
{
    public static class GuiHelper
    {
        public static void AutomaticRuntimeSize(this FrameworkElement fwe_in)
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                fwe_in.Width = double.NaN;
                fwe_in.Height = double.NaN;
            }
        }
    }
}