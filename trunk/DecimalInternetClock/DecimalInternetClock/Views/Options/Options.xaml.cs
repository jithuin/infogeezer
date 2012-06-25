using System.Windows;
using System.Windows.Input;

namespace DecimalInternetClock.Options
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {

        public Options()
        {
            InitializeComponent();
        }

        private void CommandAcknowledgeOptions_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
