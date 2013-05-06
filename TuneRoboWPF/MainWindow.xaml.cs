using System.Windows;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void MainScreen_ContentRendered(object sender, System.EventArgs e)
        {
            if (GlobalVariables.ServerConnection.ConfigAndConnectSocket() == 0)
            {
                MessageBox.Show("Cannot connect to server!", "Connection error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
