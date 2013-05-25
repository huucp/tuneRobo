using TuneRoboWPF.Utility;

namespace TuneRoboWPF.RobotService
{
    public class DeleteMotionRequest:RobotRequest
    {
        private ulong MotionID { get; set; }
        public DeleteMotionRequest(RobotPacket.PacketID packetID, ulong motionID)
        {
            RequestID = packetID;
            MotionID = motionID;
        }

        public override byte[] BuildRequest()
        {
            var packet = new RobotPacket(RequestID);
            packet.Parameters = GlobalFunction.DecToLE8(MotionID);
            return packet.BuildPacket();
        }
    }

    public class GetMotionCountRequest:RobotRequest
    {
        public GetMotionCountRequest(RobotPacket.PacketID id)
        {
            RequestID = id;
        }
        public override byte[] BuildRequest()
        {
            var packet = new RobotPacket(RequestID);
            return packet.BuildPacket();
        }
    }

    public class GetMotionInfoAtIndexRequest:RobotRequest
    {
        private int Index { get; set; }
        public GetMotionInfoAtIndexRequest(RobotPacket.PacketID id,int index)
        {
            RequestID = id;
            Index = index;
        }

        public override byte[] BuildRequest()
        {
            var packet = new RobotPacket(RequestID);
            packet.Parameters = GlobalFunction.DecToLE4(Index);
            return packet.BuildPacket();
        }
    }

    public class GetMotionInfoWithIDRequest:RobotRequest
    {
        private ulong MotionID { get; set; }
        public GetMotionInfoWithIDRequest(ulong motionID, RobotPacket.PacketID packetID)
        {
            RequestID = packetID;
            MotionID = motionID;
        }

        public override byte[] BuildRequest()
        {
            var packet = new RobotPacket(RequestID);
            packet.Parameters = GlobalFunction.DecToLE8(MotionID);
            return packet.BuildPacket();
        }
    }

    public class GetStateRequest:RobotRequest
    {
        public GetStateRequest()
        {
            RequestID = RobotPacket.PacketID.GetState;
        }
        public override byte[] BuildRequest()
        {
            var packet = new RobotPacket(RequestID);
            return packet.BuildPacket();
        }
    }
}
