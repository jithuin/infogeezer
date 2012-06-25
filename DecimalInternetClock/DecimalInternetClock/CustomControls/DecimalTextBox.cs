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
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UD2_Optic_Module_Calibration.GUI.MiscControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UD2_Optic_Module_Calibration.GUI.MiscControls;assembly=UD2_Optic_Module_Calibration.GUI.MiscControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:DecimalTextBox/>
    ///
    /// </summary>
    public class DecimalTextBox : Control
    {
        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(DecimalTextBox),
                new UIPropertyMetadata("", new PropertyChangedCallback(OnTextPropertyChanged)));

        protected static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecimalTextBox dtb = (DecimalTextBox)d;
            if (e.NewValue != e.OldValue && !dtb.IsChangePending)
            {
                dtb.IsChangePending = true;
                dtb.Value = Convert.ChangeType(e.NewValue, dtb.Value.GetType());
            }
            dtb.IsChangePending = false;
        }

        private void OnTextPropertyChanged(string textValue)
        {
            Value = Convert.ChangeType(textValue, Value.GetType());
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(DecimalTextBox),
                new UIPropertyMetadata(new object(), new PropertyChangedCallback(OnValuePropertyChanged))
            //,
            //new ValidateValueCallback(ValidateValue)
                );

        protected static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DecimalTextBox dtb = (DecimalTextBox)d;
            if (e.NewValue != e.OldValue && !dtb.IsChangePending)
            {
                dtb.IsChangePending = true;
                dtb.Text = e.NewValue.ToString();
            }
            dtb.IsChangePending = false;
        }

        //protected static bool ValidateValue(object value)
        //{
        //    if (value.GetType().IsValueType && value.IsIntegerType())
        //        return true;
        //    else
        //        return true; // TODO_HL: 2 handle validation exception
        //}

        protected bool IsChangePending { get; set; }

        public DecimalTextBox()
        {
            IsChangePending = false;
        }

        static DecimalTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DecimalTextBox), new FrameworkPropertyMetadata(typeof(DecimalTextBox)));
        }
    }
}