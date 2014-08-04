using Clocks.ViewModel;
using GalaSoft.MvvmLight;
using Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PhoneView.View
{
    public sealed partial class HexaDecimalDigitView : UserControl
    {
        public HexaDecimalDigitView()
        {
            this.InitializeComponent();
            if (ViewModelBase.IsInDesignModeStatic)
            {
                DataContext = new HexaDecimalDigitViewModel();
                (DataContext as HexaDecimalDigitViewModel).Now = 0xF;
            }

            if (DataContext != null)
                if (!(DataContext is HexaDecimalDigitViewModel))
                    throw new XArchitectureException();
        }
    }
}
