using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace TuneRoboWPF.Models
{
    public class MotionDetailScreenModel
    {
        public BitmapImage CoverImage = null;
        public string DownloadButtonContent = string.Empty;
        public double RatingValue = 0;
        public string ArtistName = "Tosy";
        public ObservableCollection<ScreenshotImage> ScreenshotsList = new ObservableCollection<ScreenshotImage>();
        public BitmapImage ImageSource = null;
        public class ScreenshotImage
        {
            public BitmapImage ImageSource { get; set; }
        }

        public string MotionDescription = string.Empty;
    }
}
