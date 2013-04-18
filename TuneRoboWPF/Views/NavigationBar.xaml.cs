using System;
using System.Windows;
using System.Windows.Controls;
using TuneRoboWPF.ViewModels;
using System.Windows;

namespace TuneRoboWPF
{
	/// <summary>
	/// Interaction logic for NavigationBar.xaml
	/// </summary>
	public partial class NavigationBar : UserControl
	{
		public NavigationBar()
		{
			InitializeComponent();
		    var viewModels = new NavigationBarViewModel();
		    DataContext = viewModels;
		}


        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            UserMenu.Visibility = Visibility.Visible;
            SignInButton.Visibility = Visibility.Collapsed;
            ((NavigationBarViewModel) (DataContext)).Username = "Username";
        }

        private void RemoteButton_Click(object sender, RoutedEventArgs e)
        {
            RemoteButton.Style = (Style)FindResource("ButtonFlatStyle");
        }
	}
}