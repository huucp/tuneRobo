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
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using motion;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for MotionHorizontalItem.xaml
    /// </summary>
    public partial class MotionHorizontalItem : UserControl
    {
        public delegate void MotionClickEventHandler(ulong motionID);

        public event MotionClickEventHandler MotionClicked;

        public void OnMotionClick(ulong motionID)
        {
            MotionClickEventHandler handler = MotionClicked;
            if (handler != null) handler(motionID);
        }

        private MotionHorizontalItemViewModel ViewModel = new MotionHorizontalItemViewModel();
        private ulong MotionID { get; set; }
        public MotionHorizontalItem()
        {
            InitializeComponent();

            DataContext = new MotionHorizontalItemViewModel();
            ViewModel = (MotionHorizontalItemViewModel)DataContext;

            ViewModel.MotionClick = new ViewModelBase.CommandHandler(MotionClickHandler, true);
        }

        private void MotionClickHandler()
        {
            OnMotionClick(MotionID);
        }

        public void SetInfo(MotionShortInfo info)
        {
            ViewModel.MotionTitle = info.title;
            ViewModel.ArtistName = info.artist_name;
            ViewModel.RatingValue = info.rating / GlobalVariables.RateValueMultiplierFactor;
            ViewModel.NumberRatings = info.rating_count;
            MotionID = info.motion_id;
        }

        public void SetImage(BitmapImage image)
        {
            if (Dispatcher.CheckAccess())
            {
                ViewModel.CoverImage = image;
            }
            else
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.CoverImage = image;
                });
            }
        }
    }
}
