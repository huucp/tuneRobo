using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Views
{
    public class PreviousTrackButton : ControlButton
    {
        public PreviousTrackButton()
        {
            ViewModel.ActiveImageSource = GlobalResource.PreviousTrackImageSource;
            ViewModel.InactiveImageSource = GlobalResource.LockImageSource;
            ViewModel.Active = false;
        }
    }
}
