using System;
using System.Windows;
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
            RobotRequest.ProcessSuccessfully += Request_ProcessSuccessfully;
            RobotRequest.ProgressReport += Request_ProgressReport;
            
            ProgressBar.Maximum = 100;

            var viewModel = new TransferWindowViewModel();
            DataContext = viewModel;
            ViewModel = (TransferWindowViewModel)DataContext;
            ViewModel.Title = motionTitle;
            ViewModel.TransferText = (string)FindResource("TransferringText");
		}

        public TransferWindow(DownloadMotionStoreRequest request, string motionTitle)
        {
            InitializeComponent();

            StoreRequest = request;
            StoreRequest.ProcessError += Request_ProcessError;
            StoreRequest.ProcessSuccessfully += Request_ProcessSuccessfully;
            StoreRequest.ProgressReport += Request_ProgressReport;

            ProgressBar.Maximum = 100;

            var viewModel = new TransferWindowViewModel();
            DataContext = viewModel;
            ViewModel = (TransferWindowViewModel)DataContext;
            ViewModel.Title = motionTitle;
            ViewModel.TransferText = (string) FindResource("DownloadingText");
        }

	    private void Request_ProgressReport(int progressValue)
        {
            Dispatcher.BeginInvoke((Action) delegate
                                                {
                                                    ProgressBar.Value = progressValue;
                                                    ViewModel.Percentage = progressValue;
                                                });
        }

        private void Request_ProcessSuccessfully(object sender)
        {
            Dispatcher.BeginInvoke((Action) delegate
                                                {
                                                    DialogResult = true; 
                                                    Close();
                                                });
        }

        private void Request_ProcessError(TransferMotionToRobot.ErrorCode errorCode, string errorMessage)
        {
            Dispatcher.BeginInvoke((Action) delegate
                                                {
                                                    DialogResult = false;
                                                    Console.WriteLine(errorMessage);
                                                    Close();
                                                });
        }

        private void Request_ProcessError(Reply.Type errorcode, string errorMessage)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                DialogResult = false;
                Console.WriteLine(errorMessage);
                Close();
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