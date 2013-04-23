using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Robo
{
    class RemoteRequest:mRoboRequest
    {
        private int volumeLevel;
        private int motionIDToPlay;

        public RemoteRequest(mRoboPacket.PacketID id, int vol = -1, int motionPlay = -1)
        {
            RequestID = id;
            volumeLevel = vol;
            motionIDToPlay = motionPlay;
        }

        public override byte[] BuildRequest()
        {           
            var packet = new mRoboPacket(RequestID);

            if (volumeLevel != -1)
            {
                packet.Parameters = new[] {(byte)volumeLevel};
            }

            if (motionIDToPlay != -1)
            {
                var param = GlobalFunction.DecToLE8(motionIDToPlay);
                packet.Parameters = param;
            }
            return  packet.BuildPacket() ;
        }
    }
}