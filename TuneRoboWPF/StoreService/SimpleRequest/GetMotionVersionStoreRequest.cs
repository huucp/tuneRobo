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
    public class GetMotionVersionStoreRequest:StoreRequest
    {
        private List<ulong> MotionID = new List<ulong>();
        public GetMotionVersionStoreRequest(List<ulong> motionID)
        {
            MotionID.AddRange(motionID);
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new MotionVersionRequest();
            request.motion_id.AddRange(MotionID);                  
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.MOTION_VERSION, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
