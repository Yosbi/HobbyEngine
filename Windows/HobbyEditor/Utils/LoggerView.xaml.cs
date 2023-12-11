using System.Windows;
using System.Windows.Controls;

namespace HobbyEditor.Utils
{
    /// <summary>
    /// Interaction logic for LoggerView.xaml
    /// </summary>
    public partial class LoggerView : UserControl
    {
        public LoggerView()
        {
            InitializeComponent();
        }

        private void _onClearButtonClick(object sender, RoutedEventArgs e)
        {
            Logger.Clear();
        }

        private void _onMessageFilterButtonClick(object sender, RoutedEventArgs e)
        {
            var filter = 0x0;
            if (toggleInfos.IsChecked == true)
                filter |= (int)MessageType.Info;
            if (toggleWarnings.IsChecked == true)
                filter |= (int)MessageType.Warning;
            if (toggleErrors.IsChecked == true)
                filter |= (int)MessageType.Error;

            Logger.SetFilter(filter);
        }
    }
}
