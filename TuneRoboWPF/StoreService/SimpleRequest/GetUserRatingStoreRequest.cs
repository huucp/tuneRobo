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
    public class GetUserRatingStoreRequest:StoreRequest
    {
        private ulong MotionID { get; set; }
        public GetUserRatingStoreRequest(ulong motionID)
        {
            MotionID = motionID;
            RequestKey = GetType().ToString() + GlobalVariables.CurrentUserID + motionID;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new MyMotionRatingInfoRequest()
            {
                user_id = GlobalVariables.CurrentUserID,
                motion_id = MotionID
            };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.RATING_INFO_USER, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }

}