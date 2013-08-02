using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace TuneRoboWPF.ViewModels
{
    public class ScreenshotImageViewModel : ViewModelBase
    {
        private BitmapImage screenshotSource = null;
        public BitmapImage ScreenshotSource
        {
            get { return screenshotSource; }
            set
            {
                if(screenshotSource!=value)
                {
                    screenshotSource = value;
                    NotifyPropertyChanged("ScreenshotSource");
                }
            }
        }

        private bool isYoutubeThumnail = false;
        public bool IsYoutubeThumbnail
        {
            get { return isYoutubeThumnail; }
            set
            {
                if (isYoutubeThumnail!=value)
                {
                    isYoutubeThumnail = value;
                    NotifyPropertyChanged("IsYoutubeThumbnail");
                }
            }
        }
    }
}
