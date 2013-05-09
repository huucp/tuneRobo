using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using motion;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class GetMotionOfArtistStoreRequest:StoreRequest
    {
        private ulong ArtistID { get; set; }
        private uint Start { get; set; }
        private uint End { get; set; }

        public GetMotionOfArtistStoreRequest(ulong artistID, uint start, uint end)
        {
            ArtistID = artistID;
            Start = start;
            End = end;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new ArtistMotionRequest()
            {
                artist_id = ArtistID,
                start = Start,
                end = End
            };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.MOTION_OF_ARTIST, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
