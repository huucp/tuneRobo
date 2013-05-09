using System.IO;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using user;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class GetUserArtistRelationStoreRequest:StoreRequest
    {
        private ulong ArtistID { get; set; }
        public GetUserArtistRelationStoreRequest(ulong artistID)
        {
            ArtistID = artistID;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var relationRequest = new UserRelationRequest()
                                      {
                                          user_id = GlobalVariables.CurrentUserID,
                                          artist_id = ArtistID
                                      };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, relationRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int) MessageType.Type.USER_REL_ARTIST, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
