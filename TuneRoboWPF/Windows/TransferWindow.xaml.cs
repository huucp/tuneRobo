using System;
using System.Windows;
using MessageBoxUtils;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.StoreService.BigRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using comm;

namespace TuneRoboWPF.Windows
{
	/// <summary>
	/// Interaction logic for TransferWindow.xaml
	/// </summary>
	public partial class TransferWindow : Window
	{
        private TransferMotionToRobot RobotRequest { get; set; }
        private DownloadMotionStoreRequest StoreRequest { get; set; }
	    private TransferWindowViewModel ViewModel;
        public TransferWindow(TransferMotionToRobot request, string motionTitle)
		{
			InitializeComponent();
		    
            RobotRequest = request;
            RobotRequest.ProcessError += Request_ProcessError;
            RobotRequest.ProcessSuccessfully += RobotRequest_ProcessSuccessfully;
            RobotRequest.ProgressReport += Request_ProgressReport;
            
            ProgressBar.Maximum = 100;

            var viewModel = new TransferWindowViewModel();
            DataContext = viewModel;
            ViewModel = (TransferWindowViewModel)DataContext;
            ViewModel.Title = motionTitle;
            ViewModel.TransferText = (string)TryFindResource("TransferringText");
		}

        public TransferWindow(DownloadMotionStoreRequest request, string motionTitle)
        {
            InitializeComponent();

            StoreRequest = request;
            StoreRequest.ProcessError += Request_ProcessError;
            StoreRequest.ProcessSuccessfully += StoreRequest_ProcessSuccessfully;
            StoreRequest.ProgressReport += Request_ProgressReport;
            StoreRequest.ProcessCancel += StoreRequest_ProcessCancel;

            ProgressBar.Maximum = 100;

            var viewModel = new TransferWindowViewModel();
            DataContext = viewModel;
            ViewModel = (TransferWindowViewModel)DataContext;
            ViewModel.Title = motionTitle;
            ViewModel.TransferText = (string) TryFindResource("DownloadingText");
        }

        private void StoreRequest_ProcessCancel()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                DialogResult = true;
                Close();                
            });
        }

	    private void Request_ProgressReport(int progressValue)
        {
            Dispatcher.BeginInvoke((Action) delegate
                                                {
                                                    ProgressBar.Value = progressValue;
                                                    ViewModel.Percentage = progressValue;
                                                });
        }
        private void RobotRequest_ProcessSuccessfully(object sender)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                DialogResult = true;
                Close();
                var title = (string)TryFindResource("TransferCompletedText");
                WPFMessageBox.Show(StaticMainWindow.Window, "", title, MessageBoxButton.OK, MessageBoxImage.Information,
                                   MessageBoxResult.OK);
            });
        }

        private void StoreRequest_ProcessSuccessfully(object sender)
        {
            Dispatcher.BeginInvoke((Action) delegate
            {
                DialogResult = true;                
                Close();
                var title = (string) TryFindResource("DownloadCompletedText");
                WPFMessageBox.Show(StaticMainWindow.Window, "", title, MessageBoxButton.OK, MessageBoxImage.Information,
                                   MessageBoxResult.OK);
            });
        }

        private void Request_ProcessError(TransferMotionToRobot.ErrorCode errorCode, string errorMessage)
        {
            Dispatcher.BeginInvoke((Action) delegate
                                                {
                                                    DialogResult = false;
                                                    Console.WriteLine(errorMessage);                                                    
                                                    Close();
                                                    var title = (string)TryFindResource("TransferErrorText");
                                                    WPFMessageBox.Show(StaticMainWindow.Window, "", title, MessageBoxButton.OK, MessageBoxImage.Information,
                                                                       MessageBoxResult.OK);
                                                });
        }

        private void Request_ProcessError(DownloadMotionStoreRequest.DownloadMotionErrorCode errorcode, string errorMessage)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                DialogResult = false;
                Console.WriteLine(errorMessage);
                Close();
                var title = (string)TryFindResource("DownloadErrorText");
                WPFMessageBox.Show(StaticMainWindow.Window, "", title, MessageBoxButton.OK, MessageBoxImage.Information,
                                   MessageBoxResult.OK);
            });
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (RobotRequest == null && StoreRequest == null) return;            
            if (RobotRequest!=null) GlobalVariables.RobotWorker.AddJob(RobotRequest);
            else
            {
                GlobalVariables.StoreWorker.AddRequest(StoreRequest);
            }
        }        

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (RobotRequest != null) RobotRequest.CancelProcess = true;
            else StoreRequest.CancelProcess = true;
        }

        public bool? ShowDialog(Window owner)
        {
            Owner = owner;
            return ShowDialog();
        }        
	}
}