using TuneRoboWPF.Models;

namespace TuneRoboWPF.ViewModels
{
    public class MotionTitleItemViewModel : ViewModelBase
    {
        private MotionTitleItemModel model = new MotionTitleItemModel();

        public string Title
        {
            get { return model.MotionTitle; }
            set
            {
                model.MotionTitle = value;
                NotifyPropertyChanged("Title");
            }
        }


        public uint Duration
        {
            get { return model.Duration; }
            set
            {
                model.Duration = value;
                NotifyPropertyChanged("Duration");
            }
        }

        public bool TrashVisibility
        {
            get { return model.TrashVisibility; }
            set
            {
                model.TrashVisibility = value;
                NotifyPropertyChanged("TrashVisibility");
            }
        }
        public string HeaderVisibility
        {
            get { return model.HeaderVisibility; }
            set
            {
                model.HeaderVisibility = value;
                NotifyPropertyChanged("HeaderVisibility");
            }
        }
    }
}