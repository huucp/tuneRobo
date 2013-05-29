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
        public delegate void ArtistItemClickEventHandler(ArtistItemVertical sender);

        public event ArtistItemClickEventHandler ArtistItemClicked;

        public void OnArtistItemClick(ArtistItemVertical sender)
        {
            ArtistItemClickEventHandler handler = ArtistItemClicked;
            if (handler != null) handler(sender);
        }
        private ArtistItemVerticalViewModel ViewModel = new ArtistItemVerticalViewModel();
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
            OnArtistItemClick(this);
        }

        public void SetInfo(ArtistShortInfo info)
        {
            if (Dispatcher.CheckAccess())
            {
                ViewModel.ArtistName = info.artist_name;

            }
            else
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.ArtistName = info.artist_name;
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
