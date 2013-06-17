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
    public class UserOwnMotionStoreRequest:StoreRequest
    {
        public ulong MotionID { get; set; }
        public UserOwnMotionStoreRequest(ulong motionID)
        {
            MotionID = motionID;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();

            var userOwnMotionRequest = new UserOwnMotionRequest
            {
                user_id = GlobalVariables.CurrentUser.UserID,
                motion_id = MotionID
            };

            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, userOwnMotionRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.CHECK_USER_OWNED_MOTION, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
