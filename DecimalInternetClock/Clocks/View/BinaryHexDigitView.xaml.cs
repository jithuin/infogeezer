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
    public partial class BinaryHexDigitView : UserControl
    {
        public BinaryHexDigitView()
        {
            InitializeComponent();
            if (ViewModelBase.IsInDesignModeStatic)
            {
                DataContext = new BinaryHexDigitViewModel();
                (DataContext as BinaryHexDigitViewModel).Now = 0xF;
            }

            if (DataContext != null)
                if (!(DataContext is BinaryHexDigitViewModel))
                    throw new XArchitectureException();
        }
    }
}