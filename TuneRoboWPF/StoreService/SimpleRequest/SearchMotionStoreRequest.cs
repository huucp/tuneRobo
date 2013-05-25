using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using motion;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class SearchMotionStoreRequest:StoreRequest
    {
        private string Query { get; set; }
        private uint Start { get; set; }
        private uint End { get; set; }
        public SearchMotionStoreRequest(string query, uint start, uint end)
        {
            Query = query;
            Start = start;
            End = end;
            RequestKey = GetType() + query + start.ToString() + end.ToString();
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new SearchMotionRequest()
                              {
                                  end = End,
                                  start = Start,
                                  query = Query
                              };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.MOTION_SEARCH, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
