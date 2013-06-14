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
    public class GetUserInfoStoreRequest:StoreRequest
    {
        public ulong MotionID { get; set; }
        public GetUserInfoStoreRequest(ulong motionID)
        {
            MotionID = motionID;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var getUserInfoRequest = new ProfileRequest()
            {
                user_id = MotionID
            };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, getUserInfoRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.USER_INFO_GET, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
