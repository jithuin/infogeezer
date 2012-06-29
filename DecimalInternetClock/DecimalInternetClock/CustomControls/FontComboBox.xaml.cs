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
    /// Interaction logic for FontComboBox.xaml
    /// </summary>
    public partial class FontComboBox : UserControl
    {
        public FontComboBox()
        {
            InitializeComponent();
            this.AutomaticRuntimeSize();
            InitComboBox();
        }

        private void InitComboBox()
        {
            foreach (FontFamily ff in Fonts.SystemFontFamilies)
            {
                cbFont.Items.Add(ff);
            }
        }

        public FontFamily SelectedFontFamily
        {
            get { return (FontFamily)GetValue(SelectedFontFamilyProperty); }
            set { SetValue(SelectedFontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedFontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedFontFamilyProperty =
            DependencyProperty.Register("SelectedFontFamily", typeof(FontFamily), typeof(FontComboBox), new UIPropertyMetadata(new FontFamily("Calibri")));

        private void cbFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFontFamily = cbFont.SelectedItem as FontFamily;
        }
    }
}