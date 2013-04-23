using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Robo
{
    public class DeleteMotionRequest:mRoboRequest
    {
        private ulong MotionID { get; set; }
        public DeleteMotionRequest( mRoboPacket.PacketID packetID, ulong motionID)
        {
            RequestID = packetID;
            MotionID = motionID;
        }

        public override byte[] BuildRequest()
        {
            var packet = new mRoboPacket(RequestID);
            packet.Parameters = GlobalFunction.DecToLE8(MotionID);
            return packet.BuildPacket();
        }
    }

    public class GetMotionCountRequest:mRoboRequest
    {
        public GetMotionCountRequest(mRoboPacket.PacketID id)
        {
            RequestID = id;
        }
        public override byte[] BuildRequest()
        {
            var packet = new mRoboPacket(RequestID);
            return packet.BuildPacket();
        }
    }

    public class GetMotionInfoAtIndexRequest:mRoboRequest
    {
        private int Index { get; set; }
        public GetMotionInfoAtIndexRequest(mRoboPacket.PacketID id,int index)
        {
            RequestID = id;
            Index = index;
        }

        public override byte[] BuildRequest()
        {
            var packet = new mRoboPacket(RequestID);
            packet.Parameters = GlobalFunction.DecToLE4(Index);
            return packet.BuildPacket();
        }
    }

    public class GetMotionInfoWithID:mRoboRequest
    {
        private ulong MotionID { get; set; }
        public GetMotionInfoWithID(ulong motionID, mRoboPacket.PacketID packetID)
        {
            RequestID = packetID;
            MotionID = motionID;
        }

        public override byte[] BuildRequest()
        {
            var packet = new mRoboPacket(RequestID);
            packet.Parameters = GlobalFunction.DecToLE8(MotionID);
            return packet.BuildPacket();
        }
    }
}
