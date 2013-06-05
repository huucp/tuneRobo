using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TuneRoboWPF.ViewModels;
using artist;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for ArtistItemVertical.xaml
    /// </summary>
    public partial class ArtistItemVertical : UserControl
    {
        public delegate void ArtistItemClickEventHandler(ulong artistID);

        public event ArtistItemClickEventHandler ArtistItemClicked;

        public void OnArtistItemClick(ulong artistID)
        {
            ArtistItemClickEventHandler handler = ArtistItemClicked;
            if (handler != null) handler(artistID);
        }
        private ArtistItemVerticalViewModel ViewModel = new ArtistItemVerticalViewModel();
        private ulong ArtistID { get; set; }
        public ArtistItemVertical()
        {
            InitializeComponent();

            DataContext = new ArtistItemVerticalViewModel();
            ViewModel = (ArtistItemVerticalViewModel)DataContext;

            ViewModel.DescriptionClick = new ViewModelBase.CommandHandler(ItemClickHandler, true);
            ViewModel.ImageClick = new ViewModelBase.CommandHandler(ItemClickHandler, true);
        }

        private void ItemClickHandler()
        {
            OnArtistItemClick(ArtistID);
        }

        public void SetInfo(ArtistShortInfo info)
        {
            if (Dispatcher.CheckAccess())
            {
                ViewModel.ArtistName = info.artist_name;
                ArtistID = info.artist_id;
            }
            else
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.ArtistName = info.artist_name;
                    ArtistID = info.artist_id;
                });
            }
        }
        public void SetImage(BitmapImage image)
        {
            if (Dispatcher.CheckAccess())
            {
                ViewModel.ArtistIcon = image;                
            }
            else
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.ArtistIcon = image;
                });
            }
        }
    }
}
