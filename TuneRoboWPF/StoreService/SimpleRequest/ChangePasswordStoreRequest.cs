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
    public class ChangePasswordStoreRequest:StoreRequest
    {
        private string OldPassword { get; set; }
        private string NewPassword { get; set; }
        public ChangePasswordStoreRequest(string oldPass, string newPass)
        {
            OldPassword = oldPass;
            NewPassword = newPass;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var changePassRequest = new ChangePassRequest
                                        {
                                            user_id = GlobalVariables.CurrentUser.UserID,
                                            new_pass = NewPassword,
                                            old_pass = OldPassword
                                        };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, changePassRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.USER_CHANGE_PASS, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
