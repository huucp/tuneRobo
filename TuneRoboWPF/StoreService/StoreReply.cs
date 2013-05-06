using System.IO;
using TuneRoboWPF.Utility;
using comm;
using ProtoBuf;

namespace TuneRoboWPF.StoreService
{
    public class StoreReply
    {
        private const int MagicByte = 0xEE;
        private byte[] Packet { get; set; }
        private MessageType.Type ReplyType { get; set; }
        private ulong PacketID;
        public StoreReply(byte[] packet, ulong packetID)
        {
            Packet = packet;
            PacketID = packetID;
        }

        public Reply  ProcessReply()
        {
            if (Packet == null) return null;

            if (Packet[0] != MagicByte || Packet[1] != MagicByte) return null;

            byte[] tmpSize = GlobalFunction.SplitByteArray(Packet, 2, 2);
            if (GlobalFunction.BE2ToDec(tmpSize) != Packet.Length) return null;

            byte[] tmpType = GlobalFunction.SplitByteArray(Packet, 4, 3);
            if (GlobalFunction.BE3ToDec(tmpType) != (decimal) MessageType.Type.REPLY) return null;

            byte[] tmpID = GlobalFunction.SplitByteArray(Packet, 8, 8);
            if (GlobalFunction.BE8ToDec(tmpID) != PacketID) return null;

            var dataReply = GlobalFunction.SplitByteArray(Packet, 16, Packet.Length - 16);
            var reply = Serializer.Deserialize<Reply>(new MemoryStream(dataReply));
            return reply;
        }
    }
    public class StoreReplyData
    {

    }
}
