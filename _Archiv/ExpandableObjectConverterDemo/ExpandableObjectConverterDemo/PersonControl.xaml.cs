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
using System.ComponentModel;

namespace ExpandableObjectConverterDemo
{
    /// <summary>
    /// Interaction logic for PersonControl.xaml
    /// </summary>
    public partial class PersonControl : UserControl
    {
        public PersonControl()
        {
            InitializeComponent();
            PersonDependencyObject = new PersonDependencyObject();
        }

        public static readonly DependencyProperty PersonDependencyObjectProperty = DependencyProperty.Register("PersonDependencyObject", typeof(PersonDependencyObject), typeof(PersonControl));

        [Category("APerson")]
        public PersonDependencyObject PersonDependencyObject
        {
            get { return (PersonDependencyObject)GetValue(PersonDependencyObjectProperty); }
            set { SetValue(PersonDependencyObjectProperty, value); }
        }

        public static readonly DependencyProperty ExpandablePersonObjectProperty = DependencyProperty.Register("ExpandablePersonObject", typeof(ExpandablePersonObject), typeof(PersonControl));

        [Category("APerson")]
        public ExpandablePersonObject ExpandablePersonObject
        {
            get { return (ExpandablePersonObject)GetValue(ExpandablePersonObjectProperty); }
            set { SetValue(ExpandablePersonObjectProperty, value); }
        }

        public static readonly DependencyProperty PersonObjectProperty = DependencyProperty.Register("PersonObject", typeof(PersonObject), typeof(PersonControl));

        [Category("APerson")]
        public PersonObject PersonObject
        {
            get { return (PersonObject)GetValue(PersonObjectProperty); }
            set { SetValue(PersonObjectProperty, value); }
        }

        public static readonly DependencyProperty PersonObjectWithExpansionProperty = DependencyProperty.Register("PersonObjectWithExpansion", typeof(PersonObject), typeof(PersonControl));

        [Category("APerson")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PersonObject PersonObjectWithExpansion
        {
            get { return (PersonObject)GetValue(PersonObjectWithExpansionProperty); }
            set { SetValue(PersonObjectWithExpansionProperty, value); }
        }
    }
}
