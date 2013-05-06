using System.IO;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using user;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class SignoutStoreRequest : StoreRequest
    {
        public override void BuildPacket()
        {
            base.BuildPacket();
            var signoutRequest = new SignoutRequest()
            {
                user_id = GlobalVariables.CurrentUserID
            };

            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, signoutRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.SIGNOUT, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
