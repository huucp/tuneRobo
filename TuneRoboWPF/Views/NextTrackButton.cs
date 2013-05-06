using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Views
{
    public class NextTrackButton : ControlButton
    {
        public NextTrackButton()
        {
            ViewModel.ActiveImageSource = GlobalResource.NextTrackImageSource;
            ViewModel.InactiveImageSource = GlobalResource.LockImageSource;
            ViewModel.Active = false;
        }
        protected override void ClickProcess()
        {
            var index = GlobalVariables.CurrentRobotState.CurrentMotionIndex;
            if (index == GlobalVariables.CurrentListMotion.Count - 1)
            {
                index = 0;
                GlobalVariables.CurrentRobotState.CurrentMotionIndex = 0;
            }
            else
            {
                index++;
                GlobalVariables.CurrentRobotState.CurrentMotionIndex++;
            }
            ulong nextMotionID =
                GlobalVariables.CurrentListMotion[index].MotionID;
            Request = new RemoteRequest(RobotPacket.PacketID.SelectMotionToPlay, -1, nextMotionID);
        }
    }
}
