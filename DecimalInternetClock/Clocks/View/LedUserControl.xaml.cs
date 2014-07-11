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

namespace Clocks.View
{
    /// <summary>
    /// Interaction logic for LedUserControl.xaml
    /// </summary>
    public partial class LedUserControl : UserControl, INotifyPropertyChanged
    {
        public LedUserControl()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ForegroundProperty)
                RaisePropertyChanged(LedBrushPropertyName);
        }

        #region LedBrush

        /// <summary>
        /// The <see cref="LedBrush" /> property's name.
        /// </summary>
        public const string LedBrushPropertyName = "LedBrush";

        /// <summary>
        /// Sets and gets the LedBrush property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public Brush LedBrush
        {
            get
            {
                RadialGradientBrush gb = new RadialGradientBrush(Foreground.ToColor(200), Foreground.ToColor(64));
                gb.GradientOrigin = new Point(0.65, 0.35);
                return gb;
            }
        }

        #endregion LedBrush

        #region INotifyPropertyChanged Members

        public void RaisePropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }

    public static class BrushHelper
    {
        public static Color ToColor(this Brush brush_in, byte alpha_in)
        {
            SolidColorBrush brush = brush_in as SolidColorBrush;

            if (brush != null)
            {
                Color c = new Color();

                c.A = (byte)(alpha_in * brush.Color.A / 255);
                c.R = brush.Color.R;
                c.G = brush.Color.G;
                c.B = brush.Color.B;
                return c;
            }
            else
            {
                return Colors.Transparent;
            }
        }
    }
}