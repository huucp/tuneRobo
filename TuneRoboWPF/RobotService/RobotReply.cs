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
            var tmp = GlobalFunction.SplitByteArray(ReplyPacket, 8, 2);
            var id = GlobalFunction.LE2ToDec(tmp);
            return (id == GlobalVariables.ID_ACK);
        }
        private ReplyData ProcessACKPacket()
        {
            var replyData = new ReplyData();

            return replyData;
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
