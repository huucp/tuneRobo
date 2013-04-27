using TuneRoboWPF.Utility;

namespace TuneRoboWPF.RobotService
{
    public class RobotReply
    {
        private byte[] ReplyPacket { get; set; }
        public RobotPacket.PacketID RequestID { get; set; }
        public RobotReply(byte[] packet)
        {
            ReplyPacket = packet;
        }

        public ReplyData Process()
        {
            var replyData = new ReplyData();
            if(!GetSessionID())
            {
                replyData.Type = ReplyData.ReplyType.Failed;
                return replyData;
            }
            if (CheckReplyID())
            {
                return ProcessACKPacket();
            }
            else
            {
                return ProcessErrorPacket();
            }
        }

        private bool GetSessionID()
        {
            var tmp = GlobalFunction.SplitByteArray(ReplyPacket, 10, 2);
            if (RequestID == RobotPacket.PacketID.Hello)
            {
                GlobalVariables.MRoboSessionID = tmp;
                return true;
            }
            return GlobalFunction.CompareByteArray(tmp, GlobalVariables.MRoboSessionID);
        }

        private bool CheckReplyID()
        {
            byte[] tmp = GlobalFunction.SplitByteArray(ReplyPacket, 8, 2);
            int id = GlobalFunction.LE2ToDec(tmp);
            return (id == GlobalVariables.ID_ACK);
        }
        private ReplyData ProcessACKPacket()
        {
            var replyData = new ReplyData();
            replyData.Type = ReplyData.ReplyType.Success;
            switch (RequestID)
            {
                case RobotPacket.PacketID.Play:
                case RobotPacket.PacketID.Pause:
                case RobotPacket.PacketID.Forward:
                case RobotPacket.PacketID.Backward:
                case RobotPacket.PacketID.OpenTransform:
                case RobotPacket.PacketID.CloseTransform:
                case RobotPacket.PacketID.SetVolumeLevel:
                case RobotPacket.PacketID.SelectMotionToPlay:
                    GetCurrentState();
                    break;
                case RobotPacket.PacketID.WriteMotionData:
                case RobotPacket.PacketID.CountMotions:
                case RobotPacket.PacketID.GetInfoMotionAtIndex:
                    replyData.Data = ProcessInformationRequest();
                    break;                
            }
            return replyData;
        }

        private void GetCurrentState()
        {
            var stateBytes = GlobalFunction.SplitByteArray(ReplyPacket, 12, 11);
            GlobalVariables.CurrentRobotState.UpdateState(stateBytes);
        }

        private byte[] ProcessInformationRequest()
        {
            return GlobalFunction.SplitByteArray(ReplyPacket, 12, ReplyPacket.Length - 12);
        }

        private ReplyData ProcessErrorPacket()
        {
            var replyData = new ReplyData();
            return replyData;
        }
    }

    public class ReplyData
    {
        public enum ReplyType
        {
            Failed = 0,
            Success
        };

        public ReplyType Type { get; set; }
        public byte[] Data { get; set; }
        public ReplyData()
        {
            Type = ReplyType.Failed;
            Data = null;
        }
    }
}
