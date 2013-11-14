using System;
using System.Diagnostics;
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

        public RobotReplyData Process()
        {
            var replyData = new RobotReplyData();
            int packetID = GetReplyID();

            if (packetID == ID_CRC_ERROR)
            {
                replyData.Type =RobotReplyData.ReplyType.CRC;
                return replyData;
            }
            else
            {
                if (!GetSessionID())
                {
                    replyData.Type = RobotReplyData.ReplyType.WrongID;
                    return replyData;
                }
                if (packetID == ID_ACK) return ProcessACKPacket();
                else
                {
                    DebugHelper.WriteLineDebug("Reply packet ID:" + packetID);
                    return ProcessErrorPacket();
                }
            }

        }

        private bool GetSessionID()
        {
            var tmp = GlobalFunction.SplitByteArray(ReplyPacket, 10, 2);
            if (RequestID == RobotPacket.PacketID.Hello)
            {
                GlobalVariables.RobotSessionID = tmp;
                return true;
            }
            if (GlobalFunction.CompareByteArray(tmp, GlobalVariables.RobotSessionID))
            {
                return true;
            }
            Debug.Fail("Wrong sesson ID", String.Format("{0:d} vs {1:d}", GlobalFunction.LE2ToDec(tmp), GlobalFunction.LE2ToDec(GlobalVariables.RobotSessionID)));
            return false;
        }



        public const int ID_ACK = 0x0001;
        public const int ID_ERROR = 0x0002;
        public const int ID_CRC_ERROR = 0x0003;

        private int GetReplyID()
        {
            byte[] tmp = GlobalFunction.SplitByteArray(ReplyPacket, 8, 2);
            return GlobalFunction.LE2ToDec(tmp);
        }
        private RobotReplyData ProcessACKPacket()
        {
            var replyData = new RobotReplyData();
            replyData.Type = RobotReplyData.ReplyType.Success;
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
                case RobotPacket.PacketID.GetState:
                    GetCurrentState();
                    break;
                case RobotPacket.PacketID.WriteMotionData:
                case RobotPacket.PacketID.WriteMusicData:
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

        private RobotReplyData ProcessErrorPacket()
        {
            var replyData = new RobotReplyData();
            DebugHelper.WriteLineDebug("ProcessErrorPacket");
            return replyData;
        }
    }

    public class RobotReplyData
    {
        public enum ReplyType
        {
            Success = 1,
            Failed,
            CRC,
            WrongID
        };

        public ReplyType Type { get; set; }
        public byte[] Data { get; set; }
        public RobotReplyData()
        {
            Type = ReplyType.Failed;
            Data = null;
        }
    }
}
