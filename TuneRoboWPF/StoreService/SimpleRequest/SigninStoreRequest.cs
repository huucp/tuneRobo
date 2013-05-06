using System.IO;
using TuneRoboWPF.Utility;
using comm;
using user;
using ProtoBuf;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class SigninStoreRequest:StoreRequest
    {
        private string Email { get; set; }
        private string Password { get; set; }
        private SigninRequest.Type SiginType { get; set; }
        public SigninStoreRequest(string email, string password,  SigninRequest.Type type )
        {
            Email = email;
            Password = password;
            SiginType = type;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var siginRequest = new SigninRequest
                                   {
                                       email = Email,
                                       password = Password,
                                       type = SiginType
                                   };

            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream,siginRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.SIGNIN, 2,
                                                       packetData,GlobalVariables.CountRequest);
            
        }
    }
}
