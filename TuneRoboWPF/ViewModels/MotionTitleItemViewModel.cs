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

        
        public string RectangleFillColor
        {
            get { return model.RectangleFillColor; }
            set
            {
                model.RectangleFillColor = value;
                NotifyPropertyChanged("RectangleFillColor");
            }
        }
    }
}