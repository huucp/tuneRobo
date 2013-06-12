using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.Views
{
	/// <summary>
	/// Interaction logic for MotionTitleItem.xaml
	/// </summary>
	public partial class MotionTitleItem : UserControl
	{
		public MotionTitleItem()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
            DataContext = new MotionTitleItemViewModel();
		    ViewModel = (MotionTitleItemViewModel) (DataContext);
		}               
	    public MotionTitleItemViewModel ViewModel;
        public ulong MotionID { get; set; }

        public delegate void DeleteMotionEventHandler(ulong motionID);

        public event DeleteMotionEventHandler DeleteMotion;

        private void OnDeleteMotion(ulong motionID)
        {
            DeleteMotionEventHandler handler = DeleteMotion;
            if (handler != null) handler(motionID);
        }


        // This method raises the DeleteMotion event         
        private void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.Assert(MotionID!=0, "Must set motion ID when create motion item");
            OnDeleteMotion(MotionID);
        }
	}
}