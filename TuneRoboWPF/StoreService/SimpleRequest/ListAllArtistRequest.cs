using System.IO;
using ProtoBuf;
using TuneRoboWPF.Utility;
using artist;
using comm;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class ListAllArtistRequest:StoreRequest
    {
        private uint Start { get; set; }
        private uint End { get; set; }

        public ListAllArtistRequest(uint start, uint end)
        {
            Start = start;
            End = end;
            RequestKey = GetType() + start.ToString() + end.ToString();
        }

        public override void BuildPacket()
        {
            base.BuildPacket();
            var createArtistRequest = new ListArtistRequest()
            {
                start = Start,
                end = End
            };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, createArtistRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.ARTIST_LIST_EXISTED, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
