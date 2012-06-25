using System;
using System.Collections.Generic;
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

namespace DecimalInternetClock.CustomControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LedUserControl : UserControl
    {
        public LedUserControl()
        {
            InitializeComponent();
        }

        public bool LedState
        {
            get { return (bool)GetValue(LedStateProperty); }
            set { SetValue(LedStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LedState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LedStateProperty =
            DependencyProperty.Register("LedState", typeof(bool), typeof(LedUserControl), new UIPropertyMetadata(true));

        public byte Transparency
        {
            get { return (byte)GetValue(TransparencyProperty); }
            set { SetValue(TransparencyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transparency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TransparencyProperty =
            DependencyProperty.Register("Transparency", typeof(byte), typeof(LedUserControl), new UIPropertyMetadata((byte)0));
    }
}