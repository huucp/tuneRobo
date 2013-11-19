using System;
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
            var index = GlobalVariables.CurrentRobotState.MotionIndex;
            if (index == GlobalVariables.CurrentListMotion.Count - 1)
            {
                index = 0;
                GlobalVariables.CurrentRobotState.MotionIndex = 0;
            }
            else
            {
                index++;
                GlobalVariables.CurrentRobotState.MotionIndex++;
            }
            if (index < 0 || index >= GlobalVariables.CurrentListMotion.Count)
            {
                Request = null;
                return;
            }
            ulong nextMotionID = GlobalVariables.CurrentListMotion[index].MotionID;
            Console.WriteLine("Next track: {0}:{1}", GlobalVariables.CurrentRobotState.MotionIndex, nextMotionID);
            Request = new RemoteRequest(RobotPacket.PacketID.SelectMotionToPlay, -1, nextMotionID);
        }
    }
}
