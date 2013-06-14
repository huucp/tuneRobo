using System.IO;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using user;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class GetNotificationStoreRequest:StoreRequest
    {
        public override void BuildPacket()
        {
            base.BuildPacket();
            var notificationRequest = new NotificationRequest()
            {
                user_id = GlobalVariables.CurrentUser.UserID
            };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, notificationRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.NOTIFICATION, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
