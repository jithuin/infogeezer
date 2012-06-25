using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DecimalInternetClock.NamedValues;
using DecimalInternetClock.Properties;

namespace DecimalInternetClock.Options
{
    /// <summary>
    /// Interaction logic for OptionsControl.xaml
    /// </summary>
    public partial class OptionsControl : UserControl
    {
        private Dictionary<NamedValueListControl, ApplicationSettingsBase> SettingsVisualBinding;

        public ObservableCollection<NamedValuePair<string, object>> ItemsUiProg
        {
            get { return (ObservableCollection<NamedValuePair<string, object>>)GetValue(ItemsUiProgProperty); }
            set { SetValue(ItemsUiProgProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsUiProg.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsUiProgProperty =
            DependencyProperty.Register("ItemsUiProg", typeof(ObservableCollection<NamedValuePair<string, object>>), typeof(OptionsControl), new UIPropertyMetadata(new ObservableCollection<NamedValuePair<string, object>>()));

        public OptionsControl()
        {
            InitializeComponent();
            InitSettingsVisualBindingDictionary();
            InitLists();
            this.AutomaticRuntimeSize();
        }

        private void InitSettingsVisualBindingDictionary()
        {
            SettingsVisualBinding = new Dictionary<NamedValueListControl, ApplicationSettingsBase>()
                    {
                        {this.nvlSettings,Settings.Default},
                        {this.nvlCalibParams,CalibParams.Default},
                        {this.nvlUiProg,UiProg.Default},
                    };
        }

        private void InitLists()
        {
            foreach (KeyValuePair<NamedValueListControl, ApplicationSettingsBase> binding in SettingsVisualBinding)
            {
                NamedValueListControl visual = binding.Key;
                ApplicationSettingsBase settingsDefaultInstance = binding.Value;
                visual.Items.Clear();
                foreach (SettingsPropertyValue item in settingsDefaultInstance.PropertyValues)
                {
                    NamedValuePair<string, object> nvp = new NamedValuePair<string, object>(item.Name, item.PropertyValue);
                    nvp.IsReadonly = item.Property.Attributes.ContainsKey(typeof(ApplicationScopedSettingAttribute));
                    visual.Items.Add(nvp);
                }
            }
        }

        private void btApply_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<NamedValueListControl, ApplicationSettingsBase> binding in SettingsVisualBinding)
            {
                NamedValueListControl visual = binding.Key;
                ApplicationSettingsBase settingsDefaultInstance = binding.Value;
                foreach (NamedValuePair<string, object> item in visual.Items)
                    settingsDefaultInstance[item.Name] = item.Value;
            }
        }
    }
}