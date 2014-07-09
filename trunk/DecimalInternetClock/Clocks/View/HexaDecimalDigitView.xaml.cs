using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Clocks.ViewModel;
using GalaSoft.MvvmLight;
using Helpers.Exceptions;

namespace Clocks.View
{
    /// <summary>
    /// Interaction logic for BinaryHexDigitView.xaml
    /// </summary>
    public partial class HexaDecimalDigitView : UserControl
    {
        public HexaDecimalDigitView()
        {
            InitializeComponent();
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