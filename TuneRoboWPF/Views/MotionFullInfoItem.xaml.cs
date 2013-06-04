using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.Views
{
	/// <summary>
	/// Interaction logic for MotionFullInfoItem.xaml
	/// </summary>
	public partial class MotionFullInfoItem : UserControl
	{
		public MotionFullInfoItem()
		{
			this.InitializeComponent();
		    DataContext = new MotionFullInfoItemViewModel();
		    ViewModel = (MotionFullInfoItemViewModel) DataContext;
		}
        public MotionFullInfoItemViewModel ViewModel = new MotionFullInfoItemViewModel();
        private ulong MotionID { get; set; }

        public void SetMotionInfo(MotionInfo info)
        {
            ViewModel.MotionTitle = info.Title;
            ViewModel.ArtistName = info.Artist;
            ViewModel.RatingValue = 0.6;
            MotionID = info.MotionID;
        }

        private void TransferButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!GlobalVariables.RoboOnline)
            {
                MessageBox.Show("Please connect to robot!", "", MessageBoxButton.OK);
            }
            else
            {
                var transferRequest = new TransferMotionToRobot(MotionID);
                var transferWindow = new Windows.TransferWindow(transferRequest, MotionID.ToString());
                transferWindow.ShowDialog();
            }
        }
	}
}