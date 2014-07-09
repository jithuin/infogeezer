using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DecimalInternetClock.NamedValues;
using Helpers.WPF;

namespace DecimalInternetClock.CustomViews
{
    /// <summary>
    /// Interaction logic for ClassEditorWindow.xaml
    /// </summary>
    public partial class ClassEditorWindow : Window
    {
        private enum Buttons
        {
            OK,
            Apply,
            Cancel,
        }

        private static Dictionary<String, Buttons> ButtonMap;

        private static void InitButtonMap()
        {
            ButtonMap = new Dictionary<string, Buttons>();
            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                ButtonMap.Add(button.ToString(), button);
            }
        }

        static ClassEditorWindow()
        {
            InitButtonMap();
        }

        public ClassEditorWindow()
        {
            InitializeComponent();

            this.AutomaticRuntimeSize();
        }

        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Item.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(object), typeof(ClassEditorWindow), new UIPropertyMetadata(new object(), new PropertyChangedCallback(ItemChanged)));

        public static void ItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClassEditorWindow cew = (ClassEditorWindow)d;
            PopulateClassMembers(cew.Item, cew.nvlClassMembers.Items);
        }

        private static void PopulateClassMembers(object p, NamedValueList namedValueList)
        {
            // public properties
            foreach (MemberInfo member in p.GetType().GetProperties())
            {
                PropertyInfo pi = (PropertyInfo)member;
                namedValueList.Add(new NamedValuePair(member.Name, pi.GetValue(p, null), !pi.CanWrite));
            }
            // public fields
            foreach (MemberInfo member in p.GetType().GetFields())
            {
                FieldInfo fi = (FieldInfo)member;
                namedValueList.Add(new NamedValuePair(member.Name, fi.GetValue(p)));
            }
            // private fields
            foreach (MemberInfo member in p.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                FieldInfo fi = (FieldInfo)member;
                namedValueList.Add(new NamedValuePair(member.Name, fi.GetValue(p)));
            }
        }

        private void ApplySettings(object p, NamedValueList namedValueList)
        {
            // public properties
            foreach (MemberInfo member in p.GetType().GetProperties())
            {
                PropertyInfo pi = (PropertyInfo)member;
                if (pi.CanWrite)
                    pi.SetValue(p, namedValueList[member.Name], null);
            }
            // public fields
            foreach (MemberInfo member in p.GetType().GetFields())
            {
                FieldInfo fi = (FieldInfo)member;
                fi.SetValue(p, namedValueList[member.Name]);
            }
            // private fields
            foreach (MemberInfo member in p.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                FieldInfo fi = (FieldInfo)member;
                fi.SetValue(p, namedValueList[member.Name]);
            }
        }

        private void ApplySettings()
        {
            ApplySettings(this.Item, this.nvlClassMembers.Items);
        }

        private void btApply_Click(object sender, RoutedEventArgs e)
        {
            String buttonName = ((Button)sender).Tag.ToString();
            switch (ButtonMap[buttonName])
            {
                case Buttons.OK:
                    {
                        ApplySettings();
                        this.Close();
                        return;
                    }
                case Buttons.Apply:
                    {
                        ApplySettings();
                        return;
                    }
                case Buttons.Cancel:
                    {
                        this.Close();
                        return;
                    }
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}