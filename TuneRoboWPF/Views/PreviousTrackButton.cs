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
            var index = GlobalVariables.CurrentRobotState.MotionIndex;
            if (index == 0)
            {
                index = GlobalVariables.CurrentListMotion.Count - 1;
                GlobalVariables.CurrentRobotState.MotionIndex = GlobalVariables.CurrentListMotion.Count - 1;
            }
            else
            {
                index--;
                GlobalVariables.CurrentRobotState.MotionIndex--;
            }
            ulong previousMotionID =
                GlobalVariables.CurrentListMotion[index].MotionID;
            Request = new RemoteRequest(RobotPacket.PacketID.SelectMotionToPlay, -1, previousMotionID);
        }
    }
}
