using TuneRoboWPF.Models;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.ViewModels
{
    public class RobotTransformButtonViewModel:ViewModelBase
    {
        private RobotTransformButtonModel model = new RobotTransformButtonModel();
        public string ImageSource
        {
            get
            {
                switch (State)
                {
                    case RobotTransformButtonModel.ButtonState.Transform:
                        model.ImageSource = GlobalResource.SmileIcon;
                        break;
                    case RobotTransformButtonModel.ButtonState.Untransform:
                        model.ImageSource = GlobalResource.SleepIcon;
                        break;
                    case RobotTransformButtonModel.ButtonState.InActive:
                        model.ImageSource = GlobalResource.LockImageSource;
                        break;
                }
                return model.ImageSource;
            }
        }


        public RobotTransformButtonModel.ButtonState State
        {
            get { return model.State; }
            set
            {
                model.State = value;
                NotifyPropertyChanged("ImageSource");
            }
        }
    }
}
