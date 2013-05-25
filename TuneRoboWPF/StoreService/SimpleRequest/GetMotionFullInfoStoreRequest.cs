using System.IO;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using motion;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class GetMotionFullInfoStoreRequest : StoreRequest
    {
        private ulong MotionID { get; set; }
        public GetMotionFullInfoStoreRequest(ulong motionID)
        {
            MotionID = motionID;
            RequestKey = GetType() + motionID.ToString();
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var infoRequest = new MotionInfoRequest()
                                  {
                                      motion_id = MotionID
                                  };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, infoRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.MOTION_INFO_GET, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }

}
