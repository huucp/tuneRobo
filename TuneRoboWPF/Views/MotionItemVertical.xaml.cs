using System;
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
        public delegate void MotionClickEventHandler(MotionItemVertical sender);

        public event MotionClickEventHandler MotionClicked;

        public void OnMotionClick(MotionItemVertical sender)
        {
            MotionClickEventHandler handler = MotionClicked;
            if (handler != null) handler(sender);
        }

        private MotionItemVerticalViewModel ViewModel = new MotionItemVerticalViewModel();
        public MotionItemVertical()
        {
            InitializeComponent();

            DataContext = new MotionItemVerticalViewModel();
            ViewModel = (MotionItemVerticalViewModel)DataContext;

            ViewModel.DescriptionClick = new ViewModelBase.CommandHandler(MotionClickHandler,true);
            ViewModel.ImageClick = new ViewModelBase.CommandHandler(MotionClickHandler, true);
        }

        private void MotionClickHandler()
        {
            OnMotionClick(this);
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
            if (Dispatcher.CheckAccess())
            {
                ViewModel.ArtworkImage = image;
            }
            else
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.ArtworkImage = image;
                });
            }
        }
    }
}
