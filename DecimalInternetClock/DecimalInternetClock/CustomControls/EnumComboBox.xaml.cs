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
using DecimalInternetClock.Helpers;

namespace DecimalInternetClock.CustomControls
{
    /// <summary>
    /// Interaction logic for EnumComboBox.xaml
    /// </summary>
    public partial class EnumComboBox : UserControl
    {
        public EnumComboBox()
        {
            InitializeComponent();
            this.AutomaticRuntimeSize();
        }

        private enum EDefault
        {
            Null = 0,
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.GetType() != e.OldValue.GetType())
            {
                ComboBox cbEnum = ((EnumComboBox)d).cbEnum;
                cbEnum.Items.Clear();
                foreach (object item in Enum.GetValues(e.NewValue.GetType()))
                {
                    cbEnum.Items.Add(item);
                }
                cbEnum.SelectedIndex = cbEnum.Items.IndexOf(e.NewValue);
            }
        }

        private static bool ValidateIncomingObject(object value)
        {
            return value.GetType().IsEnum;
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(EnumComboBox),
            new UIPropertyMetadata(EDefault.Null, new PropertyChangedCallback(OnPropertyChanged)),
            new ValidateValueCallback(ValidateIncomingObject));

        private void cbEnum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Value = this.cbEnum.SelectedItem;
        }
    }
}