using System.Windows;
using StaticBindingTest.Storage;

namespace StaticBindingTest
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ButtonClick(object sender, RoutedEventArgs e)
        {
            Manager.Instance.IsOnline = !Manager.Instance.IsOnline;
        }
    }
}
