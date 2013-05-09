using System.IO;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using user;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class SetUserInfoStoreRequest:StoreRequest
    {
        private string Name { get; set; }
        private string AvatarUrl { get; set; }
        public SetUserInfoStoreRequest(string name, string url)
        {
            Name = name;
            AvatarUrl = url;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var setUserInfoRequest = new UpdateProfileRequest
                                         {
                                             user_id = GlobalVariables.CurrentUserID,
                                             avatar_url = AvatarUrl,
                                             display_name = Name
                                         };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, setUserInfoRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.USER_INFO_SET, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
