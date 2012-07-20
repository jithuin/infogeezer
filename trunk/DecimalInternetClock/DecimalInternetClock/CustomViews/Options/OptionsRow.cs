using System;
using System.Windows;
using System.Windows.Controls;

namespace DecimalInternetClock.Options
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UD2_Optic_Module_Calibration.GUI"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UD2_Optic_Module_Calibration.GUI;assembly=UD2_Optic_Module_Calibration.GUI"
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
    ///     <MyNamespace:OptionsRow/>
    ///
    /// </summary>
    public class OptionsRow : ContentControl
    {
        public String Header
        {
            get { return (String)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(String), typeof(OptionsRow), new UIPropertyMetadata(""));

        static OptionsRow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OptionsRow), new FrameworkPropertyMetadata(typeof(OptionsRow)));
        }
    }
}