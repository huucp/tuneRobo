using TuneRoboWPF.RobotService;
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
        protected override void ClickProcess()
        {
            var index = GlobalVariables.CurrentRobotState.CurrentMotionIndex;
            if (index == 0)
            {
                index = GlobalVariables.CurrentListMotion.Count - 1;
                GlobalVariables.CurrentRobotState.CurrentMotionIndex = GlobalVariables.CurrentListMotion.Count - 1;
            }
            else
            {
                index--;
                GlobalVariables.CurrentRobotState.CurrentMotionIndex--;
            }
            ulong previousMotionID =
                GlobalVariables.CurrentListMotion[index].MotionID;
            Request = new RemoteRequest(RobotPacket.PacketID.SelectMotionToPlay, -1, previousMotionID);
        }
    }
}
