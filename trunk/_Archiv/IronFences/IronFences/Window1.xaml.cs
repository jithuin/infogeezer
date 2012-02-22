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
using System.Collections.Specialized;

namespace IronFences
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        

        private void StackPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetType() == typeof(System.Windows.DataObject))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetType() == typeof(System.Windows.DataObject))
            {
                DataObject dto = (DataObject)e.Data;
                foreach (String s in dto.GetFileDropList())
                {
                    LinkIcon li = new LinkIcon();
                    li.Link = s;
                    li.Padding = new Thickness(3);
                    //li.Width = listView.ActualWidth-9;
                    listView.Items.Add(li);
                    
                }
                e.Handled = true;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Shutdown();
        }
        
    }
}
