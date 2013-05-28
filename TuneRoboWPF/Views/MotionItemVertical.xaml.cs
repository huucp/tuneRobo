using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.Utility;
using motion;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for MotionItemVertical.xaml
    /// </summary>
    public partial class MotionItemVertical : UserControl
    {
        private MotionItemVerticalViewModel ViewModel = new MotionItemVerticalViewModel();
        public MotionItemVertical()
        {
            InitializeComponent();

            DataContext = new MotionItemVerticalViewModel();
            ViewModel = (MotionItemVerticalViewModel) DataContext;
        }

        public void SetInfo(MotionInfo info)
        {
            ViewModel.ArtistName = info.Artist;
            ViewModel.MotionTitle = info.Title;
        }
        
        public void SetInfo(MotionShortInfo info)
        {
            ViewModel.ArtistName = info.artist_name;
            ViewModel.MotionTitle = info.title;
        }

        public void SetImage(BitmapImage image)
        {
            ViewModel.ArtworkImage = image;
        }
    }
}
