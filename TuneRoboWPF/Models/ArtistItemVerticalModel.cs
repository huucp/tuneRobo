using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TuneRoboWPF.Models
{
    public class ArtistItemVerticalModel
    {
        public BitmapImage ArtistIcon = null;
        public string ArtistName = "Tosy";
        public ICommand DescriptionClick = null;
        public ICommand ImageClick = null;
    }
}
