using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using TuneRoboWPF.Utility;
using artist;
using comm;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class ListArtistFollowByUserStoreRequest:StoreRequest
    {
        public ListArtistFollowByUserStoreRequest()
        {
            RequestKey = GetType().ToString();
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new UserArtistRequest()
                              {
                                  user_id = GlobalVariables.CurrentUser.UserID
                              };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.ARTIST_LIST_USER_FOLLOW, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
