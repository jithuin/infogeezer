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

namespace DragDrop
{
    /// <summary>
    /// Interaction logic for NotesDragDropTextBox.xaml
    /// </summary>
    public partial class NotesDragDropTextBox : UserControl
    {
        public NotesDragDropTextBox()
        {
            InitializeComponent();
        }

        private DragDrop.DragDropHelper _ddh = new DragDrop.DragDropHelper();

        private void _tbDrop_DragEnter(object sender, DragEventArgs e)
        {
            _ddh.DragEnter(sender, e);
            e.Handled = true;
        }

        private void _tbDrop_DragLeave(object sender, DragEventArgs e)
        {
            _ddh.DragLeave(sender, e);
            e.Handled = true;
        }

        private void _tbDrop_Drop(object sender, DragEventArgs e)
        {
            _ddh.DragDrop(sender, e);
            e.Handled = true;
            this._tbDrop.Text = _ddh.XmlText;
            Clipboard.SetText(_ddh.XmlText);
        }
    }
}