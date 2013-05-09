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
    public class GetRatingMotionInfoStoreRequest:StoreRequest
    {
        private ulong MotionID { get; set; }
        private uint Start { get; set; }
        private uint End { get; set; }

        public GetRatingMotionInfoStoreRequest(ulong motionID, uint start, uint end)
        {
            MotionID = motionID;
            Start = start;
            End = end;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new RatingMotionInfoRequest()
                              {
                                  motion_id = MotionID,
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
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.RATING_MOTION_INFO, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
