using System;
using System.Windows;
using System.Windows.Forms;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF
{
	/// <summary>
	/// Interaction logic for TransferWindow.xaml
	/// </summary>
	public partial class TransferWindow : Window
	{
        private TransferMotionToRobot Request { get; set; }
	    private TransferWindowViewModel ViewModel;
        public TransferWindow(TransferMotionToRobot request, string motionTitle)
		{
			InitializeComponent();
		    
            Request = request;
            Request.ProcessError += Request_ProcessError;
            Request.ProcessSuccessfully += Request_ProcessSuccessfully;
            Request.ProgressReport += Request_ProgressReport;
            
            ProgressBar.Maximum = 100;

            var viewModel = new TransferWindowViewModel();
            DataContext = viewModel;
            ViewModel = (TransferWindowViewModel)DataContext;
            ViewModel.Title = motionTitle;
		}

        private void Request_ProgressReport(int progressValue)
        {
            Dispatcher.BeginInvoke((Action) delegate
                                                {
                                                    ProgressBar.Value = progressValue;
                                                    ViewModel.WindowTitle = "Transferring..." + progressValue + "%";
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

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (Request== null) return;            
            GlobalVariables.RobotWorker.AddJob(Request);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Request.CancelProcess = true;
        }        
	}
}