using System.IO;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using user;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class ForgotPasswordStoreRequest:StoreRequest
    {
        private string Email { get; set; }
        public ForgotPasswordStoreRequest(string email)
        {
            Email = email;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var forgotPassRequest = new ForgotPassRequest
                                        {
                                            email = Email
                                        };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, forgotPassRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.USER_FORGOT_PASS, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
