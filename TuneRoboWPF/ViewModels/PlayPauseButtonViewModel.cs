using System.Windows.Controls;
using TuneRoboWPF.Models;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.ViewModels
{
    public class PlayPauseButtonViewModel : ViewModelBase
    {
        private PlayPauseButtonModel model = new PlayPauseButtonModel();
        
        public string ImageSource
        {
            get
            {
                switch (State)
                {
                    case PlayPauseButtonModel.ButtonState.Play:
                        model.ImageSource = GlobalResource.PlayImageSource;
                        break;
                    case PlayPauseButtonModel.ButtonState.Pause:
                        model.ImageSource = GlobalResource.PauseImageSource;
                        break;
                    case PlayPauseButtonModel.ButtonState.InActive:
                        model.ImageSource = GlobalResource.LockImageSource;
                        break;
                }
                return model.ImageSource;
            }            
        }


        public PlayPauseButtonModel.ButtonState State
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
