﻿using System;
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
        private string Avatar { get; set; }
        public SignupStoreRequest(string email, string name,string avatar)
        {
            Email = email;
            Name = name;
            Avatar = avatar;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new SignupRequest()
                              {
                                  email = Email,
                                  display_name = Name,
                                  avatar_url = Avatar
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
