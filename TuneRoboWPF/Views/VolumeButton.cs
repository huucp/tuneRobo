using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Views
{
    public class VolumeButton:ControlButton
    {
        public VolumeButton()
        {
            ViewModel.ActiveImageSource = GlobalResource.VolumeUpImageSource;
            ViewModel.InactiveImageSource = GlobalResource.LockImageSource;
            ViewModel.Active = false;
        }

        protected override void OnProcessSuccessfully(RobotReplyData data)
        {
        }

        protected override void OnProcessError()
        {
        }
    }
}
