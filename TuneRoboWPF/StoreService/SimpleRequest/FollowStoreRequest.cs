using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using user;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class FollowStoreRequest:StoreRequest
    {
        private FollowRequest.Type FollowType { get; set; }
        private ulong ArtistID { get; set; }
        public FollowStoreRequest(FollowRequest.Type type,ulong artistID)
        {
            FollowType = type;
            ArtistID = artistID;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var followRequest = new FollowRequest()
                                    {
                                        type = FollowType,
                                        user_id = GlobalVariables.CurrentUserID,
                                        artist_id = ArtistID
                                    };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, followRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.FOLLOW_UNFOLLOW, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
