using System.IO;
using ProtoBuf;
using TuneRoboWPF.Utility;
using artist;
using comm;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class GetFullArtistInfoStoreRequest:StoreRequest
    {
        private ulong ArtistID { get; set; }
        public GetFullArtistInfoStoreRequest(ulong artistID)
        {
            ArtistID = artistID;
            RequestKey = GetType() + artistID.ToString();
        }

        public override void BuildPacket()
        {
            base.BuildPacket();
            var updateArtistRequest = new ArtistInfoRequest()
            {
                artist_id = ArtistID
            };

            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, updateArtistRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.ARTIST_INFO_GET, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
