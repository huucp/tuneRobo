using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF
{
	/// <summary>
	/// Interaction logic for UserDetailScreen.xaml
	/// </summary>
	public partial class UserDetailScreen : UserControl
	{
        private UserDetailScreenViewModel ViewModel = new UserDetailScreenViewModel();
		public UserDetailScreen()
		{
			this.InitializeComponent();

		    DataContext = new UserDetailScreenViewModel();
		    ViewModel = (UserDetailScreenViewModel) DataContext;
		}
	}
}