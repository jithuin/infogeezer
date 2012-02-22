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
using System.Windows.Shapes;

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorPickerDialog.xaml
    /// </summary>
    public partial class ColorPickerDialog : Window
    {
        public Color SelectedColor 
        {
            get
            {
                return this.colorPicker.SelectedColor;
            }
            set
            {
                this.colorPicker.SelectedColor = value;
            }
        }
        public ColorPickerDialog()
        {
            InitializeComponent();
        }
        public ColorPickerDialog(Color initColor) : this() { SelectedColor = initColor; }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
