using System;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Robo
{
    public class mRoboReply
    {
        private byte[] ReplyPacket { get; set; }
        public mRoboPacket.PacketID RequestID { get; set; }
        public mRoboReply(byte[] packet)
        {
            ReplyPacket = packet;
        }

        public int Process(out byte[] ReplyParameter)
        {
            ReplyParameter = new byte[]{};
            if (!GetSessionID())
            {                
                Console.WriteLine("Wrong session ID");
                return 0;
            }
            if (CheckReplyID())
            {
                return ProcessACKPacket(ref ReplyParameter);
            }
            else
            {
                ProcessErrorPacket(ref ReplyParameter);
                return 0;
            }
        }

        private bool GetSessionID()
        {
            var tmp = GlobalFunction.SplitByteArray(ReplyPacket, 10, 2);
            if (RequestID == mRoboPacket.PacketID.Hello)
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

        private void  GetState()
        {
            var stateBytes = GlobalFunction.SplitByteArray(ReplyPacket, 12, 11);
            GlobalVariables.CurrentMRoboState.UpdateState(stateBytes);
        }

        private int ProcessACKPacket(ref byte[] replyParameter)
        {
            switch (RequestID)
            {
                case mRoboPacket.PacketID.Play:
                case mRoboPacket.PacketID.Stop:
                case mRoboPacket.PacketID.Forward:
                case mRoboPacket.PacketID.Backward:
                case mRoboPacket.PacketID.OpenTransform:
                case mRoboPacket.PacketID.CloseTransform:
                case mRoboPacket.PacketID.SetVolumeLevel:
                case mRoboPacket.PacketID.SelectMotionToPlay:
                    ProcessRemotePacket();
                    return 1;
                case mRoboPacket.PacketID.WriteMotionData:
                case mRoboPacket.PacketID.CountMotions:
                case mRoboPacket.PacketID.GetInfoMotionAtIndex:
                    replyParameter = ProcessInformationRequest();
                    return 1;                    
                default:
                    return 1;
            }
        }

        private byte[] ProcessInformationRequest()
        {
            return GlobalFunction.SplitByteArray(ReplyPacket, 12, ReplyPacket.Length-12);
        }

        private void ProcessRemotePacket()
        {
            GetState();
        }

        private void ProcessErrorPacket(ref byte[] replyParameter)
        {
            if (RequestID == mRoboPacket.PacketID.Hello)
            {

            }
            else
            {

            }
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
    }
}