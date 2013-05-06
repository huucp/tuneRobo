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
	}
}