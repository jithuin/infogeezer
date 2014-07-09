using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DecimalInternetClock.Helpers;
using DecimalInternetClock.NamedValues;
using DecimalInternetClock.Properties;
using Helpers.WPF;

namespace DecimalInternetClock.Options
{
    /// <summary>
    /// Interaction logic for OptionsControl.xaml
    /// </summary>
    public partial class OptionsControl : UserControl
    {
        private Dictionary<NamedValueListControl, ApplicationSettingsBase> SettingsVisualBinding;

        public NamedValueList ItemsUiProg
        {
            get { return (NamedValueList)GetValue(ItemsUiProgProperty); }
            set { SetValue(ItemsUiProgProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsUiProg.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsUiProgProperty =
            DependencyProperty.Register("ItemsUiProg", typeof(NamedValueList), typeof(OptionsControl), new UIPropertyMetadata(new NamedValueList()));

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
                        //{this.nvlSettings,Settings.Default},
                        //{this.nvlCalibParams,CalibParams.Default},
                        //{this.nvlUiProg,UiProg.Default},
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
                    visual.Items.Add(new NamedValuePair(item.Name, item.PropertyValue,
                        item.Property.Attributes.ContainsKey(typeof(ApplicationScopedSettingAttribute)))); //if the setting is application scope than it cannot be modified by the user
            }
        }

        private void btApply_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<NamedValueListControl, ApplicationSettingsBase> binding in SettingsVisualBinding)
            {
                NamedValueListControl visual = binding.Key;
                ApplicationSettingsBase settingsDefaultInstance = binding.Value;
                foreach (NamedValuePair item in visual.Items)
                    settingsDefaultInstance[item.Name] = item.Value;
            }
        }
    }
}