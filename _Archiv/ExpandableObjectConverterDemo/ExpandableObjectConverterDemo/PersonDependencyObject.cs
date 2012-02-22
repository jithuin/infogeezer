using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ExpandableObjectConverterDemo
{
    public class PersonDependencyObject : DependencyObject
    {
        public static readonly DependencyProperty AgeProperty = DependencyProperty.Register("Age", typeof(int), typeof(PersonDependencyObject));

        public int Age
        {
            get { return (int)GetValue(AgeProperty); }
            set { SetValue(AgeProperty, value); }
        }

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(PersonDependencyObject));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty ShirtColorProperty = DependencyProperty.Register("ShirtColor", typeof(Brush), typeof(PersonDependencyObject));

        public Brush ShirtColor
        {
            get { return (Brush)GetValue(ShirtColorProperty); }
            set { SetValue(ShirtColorProperty, value); }
        }

    }
}
