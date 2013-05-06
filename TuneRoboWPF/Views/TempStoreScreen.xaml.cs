using System.Windows.Controls;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.Views
{
	/// <summary>
	/// Interaction logic for MainStoreScreen.xaml
	/// </summary>
	public partial class TempStoreScreen : UserControl
	{
		public TempStoreScreen()
		{
			this.InitializeComponent();
		    DataContext = new TempStoreScreenViewModel();
		    viewModel = (TempStoreScreenViewModel) DataContext;
		}
        private TempStoreScreenViewModel viewModel = new TempStoreScreenViewModel();
	}
}