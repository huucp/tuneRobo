using System.Windows.Controls;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF
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

	    public MotionTitleItemViewModel ViewModel;
	}
}