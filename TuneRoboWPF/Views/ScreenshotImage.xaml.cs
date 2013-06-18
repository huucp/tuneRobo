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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for ScreenshotImage.xaml
    /// </summary>
    public partial class ScreenshotImage : UserControl
    {
        public ScreenshotImageViewModel ViewModel = new ScreenshotImageViewModel();        
        public ScreenshotImage()
        {
            InitializeComponent();

            DataContext = new ScreenshotImageViewModel();
            ViewModel = (ScreenshotImageViewModel) DataContext;
        }

    }
}
