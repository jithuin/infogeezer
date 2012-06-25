using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using e77.MeasureBase;
using e77.Ud2.UnitTesting;

namespace UD2_Optic_Module_Calibration.GUI
{
    /// <summary>
    /// Interaction logic for KeyValueList.xaml
    /// </summary>
    public partial class NamedValueListControl : UserControl
    {
        public NamedValueListControl()
        {
            InitializeComponent();
            Items = new NamedValueList(); //ref.: (1)
            // This (1) is needed because:
            // http://msdn.microsoft.com/en-us/library/aa970563.aspx
            // Short explanation:
            // If your property is a reference type, the default value specified in dependency property
            // metadata is not a default value per instance; instead it is a default value that applies
            // to all instances of the type.
            //
            this.AutomaticRuntimeSize();
        }

        public NamedValueList Items
        {
            get { return (NamedValueList)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(NamedValueList), typeof(NamedValueListControl), new UIPropertyMetadata(new NamedValueList()));

        public override string ToString()
        {
            return String.Format("{0}", this.Name);
        }
    }

    public class TypedDataTemplate : DataTemplate
    {
    }

    public class DataClassTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Szöveg típus
        /// </summary>
        public DataTemplate StringDataTemplate { get; set; }

        /// <summary>
        /// Halmaz típus
        /// </summary>
        public DataTemplate SetDataTemplate { get; set; }

        /// <summary>
        /// Korlátos egész típus
        /// </summary>
        /// <remarks>
        /// Korlátos azaz, alsó és felső korláttal rendelkező egész típusokhoz
        /// </remarks>
        public DataTemplate IntRangeDataTemplate { get; set; }

        public DataTemplate DoubleRangeDataTemplate { get; set; }

        public DataTemplate BoolDataTemplate { get; set; }

        public DataTemplate RemissionTemplate { get; set; }

        public DataTemplate ReadonlyTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item_in, DependencyObject container)
        {
            NamedValuePair<string, object> item = item_in as NamedValuePair<string, object>;

            if (item != null)
            {
                if (item.Name == "position")
                    return IntRangeDataTemplate;

                if (item.IsReadonly)
                    return ReadonlyTemplate;
                Object value = item.Value;

                if (value is String)
                    return StringDataTemplate;

                //IntRangeDataTemplate.Resources.Values. IntRangeDataTemplate.Resources.OfType<RangeValueConverter>().SingleOrDefault();
                if (value is double || value is float)
                    return DoubleRangeDataTemplate;
                if (value is bool)
                    return BoolDataTemplate;
                if (value != null)
                    if (value.GetType().IsEnum)
                        return SetDataTemplate;
                    else if (value.IsIntegerType())
                        return IntRangeDataTemplate;
            }
            return ReadonlyTemplate;
        }
    }

    public class RangeValueConverter : MarkupExtension, IValueConverter, INotifyPropertyChanged
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return value.ToString();
            else
                return "-- (nem áll rendelkezésre adat)";
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion INotifyPropertyChanged Members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class IntRangeValueConverter : RangeValueConverter
    {
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int ret;
            if (int.TryParse((String)value, out ret))
                return ret;
            else
                throw new ArgumentException();
        }
    }

    public class FloatRangeValueConverter : RangeValueConverter
    {
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            float ret;
            if (float.TryParse((String)value, out ret))
                return ret;
            else
                throw new ArgumentException();
        }
    }

    public class RemissionDivisionConverter : DependencyObject, IValueConverter
    {
        #region Properties
        //private int _divider = 1;
        //public int Divider
        //{
        //    get { return _divider; }
        //    set
        //    {
        //        if (_divider != value)
        //        {
        //            _divider = value;
        //            OnPropertyChanged("Divider");
        //        }
        //    }
        //}

        public int Divider
        {
            get { return (int)GetValue(DividerProperty); }
            set { SetValue(DividerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Divider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DividerProperty =
            DependencyProperty.Register("Divider", typeof(int), typeof(RemissionDivisionConverter), new UIPropertyMetadata(1));

        #endregion Properties

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((int)value) / Divider;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int.Parse((string)value) * Divider);
        }

        #endregion IValueConverter Members
    }
}