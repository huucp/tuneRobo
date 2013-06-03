using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.Windows
{
    /// <summary>
    /// Interaction logic for RatingWindow.xaml
    /// </summary>
    public partial class RatingWindow : Window
    {
        private RatingWindowViewModel ViewModel = new RatingWindowViewModel();
        public RatingWindow()
        {
            InitializeComponent();

            DataContext = new RatingWindowViewModel();
            ViewModel = (RatingWindowViewModel) DataContext;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(ViewModel.Nickname);
            Console.WriteLine(ViewModel.Title);
            Console.WriteLine(ViewModel.Review);
            Console.WriteLine(ViewModel.RatingValue);
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
