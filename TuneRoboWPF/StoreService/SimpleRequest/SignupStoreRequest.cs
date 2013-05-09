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
    public class SignupStoreRequest:StoreRequest
    {
        private string Email { get; set; }
        private string Name { get; set; }
        public SignupStoreRequest(string email, string name)
        {
            Email = email;
            Name = name;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new SignupRequest()
                              {
                                  email = Email,
                                  display_name = Name
                              };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.SIGNUP, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
