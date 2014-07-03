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
using DecimalInternetClock.Clocks.ViewModel;
using DecimalInternetClock.ViewModel;

namespace DecimalInternetClock.Clocks.View
{
    /// <summary>
    /// Interaction logic for BinaryHexDigitView.xaml
    /// </summary>
    public partial class BinaryHexDigitView : UserControl
    {
        public BinaryHexDigitView()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                BinaryHexDigitViewModel vm = new BinaryHexDigitViewModel();
                vm.view = this;
                vm.StrokeThickness = 10;
                vm.Foreground = Brushes.Black;
                vm.ActualWidth = 100;
                vm.Now = 0xF;

                DataContext = vm;
            }
            if (DataContext != null)
                if (!(DataContext is BinaryHexDigitViewModel))
                    throw new XArchitectureException();
            //else
            //{
            //    BinaryHexDigitViewModel vm = this.DataContext as BinaryHexDigitViewModel;
            //    if (vm != null)
            //        vm.ActualWidth = this.ActualWidth;
            //}
            DataContextChanged += new DependencyPropertyChangedEventHandler(BinaryHexDigitView_DataContextChanged);
        }

        private void BinaryHexDigitView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BinaryHexDigitViewModel vm = this.DataContext as BinaryHexDigitViewModel;
            if (vm != null)
            {
                vm.view = this;
                //vm.ActualWidth = this.ActualWidth;
            }
        }

        //private void this_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    BinaryHexDigitViewModel vm = this.DataContext as BinaryHexDigitViewModel;
        //    if (vm != null)
        //        vm.ActualWidth = this.ActualWidth;
        //}
    }
}