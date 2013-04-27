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
            var viewModel = new MotionTitleItemViewModel();
		    DataContext = viewModel;
		    ViewModel = (MotionTitleItemViewModel) (DataContext);
		}        

        /// <summary>
        /// Set information of motion to item
        /// </summary>
        /// <param name="info">Motion information</param>
        public void SetInfo(MotionInfo info)
        {
            MotionTitle.Text = info.Title;
        }
	    public MotionTitleItemViewModel ViewModel;
	}
}