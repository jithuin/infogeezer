using System.Windows.Input;

namespace DecimalInternetClock.CustomCommands
{
    public class CustomCommands
    {
        private static RoutedUICommand proceed = new RoutedUICommand("Proceed", "Proceed", typeof(CustomCommands));

        public static RoutedUICommand Proceed { get { return proceed; } }

        private static RoutedUICommand scanForTestSet = new RoutedUICommand("Scan for test set", "ScanForTestSet", typeof(CustomCommands));

        public static RoutedUICommand ScanForTestSet { get { return scanForTestSet; } }

        private static RoutedUICommand acknowledgeOptions = new RoutedUICommand("Acknowledge Options", "AcknowledgeOptions", typeof(CustomCommands));

        public static RoutedUICommand AcknowledgeOptions { get { return acknowledgeOptions; } }
    }
}