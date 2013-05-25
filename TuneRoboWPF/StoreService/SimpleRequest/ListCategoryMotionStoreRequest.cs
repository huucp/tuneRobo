using System.IO;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using motion;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class ListCategoryMotionStoreRequest:StoreRequest
    {
        private CategoryMotionRequest.Type CategoryType { get; set; }
        private uint Start { get; set; }
        private uint End { get; set; }

        public ListCategoryMotionStoreRequest(CategoryMotionRequest.Type type, uint start, uint end)
        {
            CategoryType = type;
            Start = start;
            End = end;
            RequestKey = GetType() + type.ToString() + start.ToString() + end.ToString();
        }

        public override void BuildPacket()
        {
            base.BuildPacket();
            var categoryMotionRequest = new CategoryMotionRequest()
                                            {
                                                type = CategoryType,
                                                start = Start,
                                                end = End
                                            };

            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, categoryMotionRequest);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.MOTION_OF_CATEGORY, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
