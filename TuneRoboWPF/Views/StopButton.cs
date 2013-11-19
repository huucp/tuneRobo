using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Views
{
    public class StopButton : ControlButton
    {
        public StopButton()
        {
            ViewModel.ActiveImageSource = GlobalResource.StopImageSource;
            ViewModel.InactiveImageSource = GlobalResource.LockImageSource;
            ViewModel.Active = false;
        }
        protected override void ClickProcess()
        {
            var stopRequest = new RemoteRequest(RobotPacket.PacketID.Stop);
            stopRequest.ProcessSuccessfully += (data) =>
                                                   {

                                                   };
            stopRequest.ProcessError += (data, msg) => Debug.Fail(msg);
            GlobalVariables.RobotWorker.AddJob(stopRequest);
        }
    }
}
