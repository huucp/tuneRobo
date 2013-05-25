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
    public class GetMotionDownloadByUserStoreRequest:StoreRequest
    {
        private uint Start { get; set; }
        private uint End { get; set; }
        public GetMotionDownloadByUserStoreRequest(uint start, uint end)
        {
            Start = start;
            End = end;
            RequestKey = GetType() + start.ToString() + end.ToString();
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new UserMotionRequest()
                              {
                                  user_id = GlobalVariables.CurrentUserID,
                                  end = End,
                                  start = Start
                              };
                              byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.MOTION_DOWNLOAD_USER, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
