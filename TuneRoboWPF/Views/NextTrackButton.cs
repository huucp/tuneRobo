using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Views
{
    public class NextTrackButton:ControlButton
    {
        public NextTrackButton()
        {
            ViewModel.ActiveImageSource = GlobalResource.NextTrackImageSource;
            ViewModel.InactiveImageSource = GlobalResource.LockImageSource;
            ViewModel.Active = false;
        }
        protected override void ClickProcess()
        {
            ulong nextMotionID =
                GlobalVariables.CurrentListMotion[GlobalVariables.CurrentRobotState.CurrentMotionIndex + 1].MotionID;
            Request = new RemoteRequest(RobotPacket.PacketID.SelectMotionToPlay,-1,nextMotionID);
        }
    }
}
