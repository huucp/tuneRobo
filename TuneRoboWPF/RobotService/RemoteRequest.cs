using TuneRoboWPF.Utility;

namespace TuneRoboWPF.RobotService
{
    public class RemoteRequest : RobotRequest
    {
        private int volumeLevel;
        private ulong motionIDToPlay;

        public RemoteRequest(RobotPacket.PacketID id, int vol = -1, ulong motionPlay = 0)
        {
            RequestID = id;
            volumeLevel = vol;
            motionIDToPlay = motionPlay;
        }

        public override byte[] BuildRequest()
        {
            var packet = new RobotPacket(RequestID);

            if (volumeLevel != -1)
            {
                packet.Parameters = new[] { (byte)volumeLevel };
            }

            if (motionIDToPlay != 0)
            {
                var param = GlobalFunction.DecToLE8(motionIDToPlay);
                packet.Parameters = param;
            }
            return packet.BuildPacket();
        }
    }
}