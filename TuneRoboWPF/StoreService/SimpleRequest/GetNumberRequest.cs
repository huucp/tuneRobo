using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using artist;
using motion;

namespace TuneRoboWPF.StoreService.SimpleRequest
{
    public class GetNumberAllArtistStoreRequest : StoreRequest
    {
        public GetNumberAllArtistStoreRequest()
        {
            RequestKey = GetType().ToString();
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new NumberAllArtistRequest();
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.NUMBER_ARTIST, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }

    public class GetNumberMotionOfCategoryStoreRequest : StoreRequest
    {
        private NumberMotionOfCategoryRequest.Type Type { get; set; }
        public GetNumberMotionOfCategoryStoreRequest(NumberMotionOfCategoryRequest.Type type)
        {
            Type = type;
            RequestKey = GetType() + type.ToString();
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new NumberMotionOfCategoryRequest()
                              {
                                  type = Type
                              };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.NUMBER_MOTION_OF_CATEGORY, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }

    public class GetNumberMotionOfArtistStoreRequest : StoreRequest
    {
        private ulong ArtistID { get; set; }
        public GetNumberMotionOfArtistStoreRequest(ulong id)
        {
            ArtistID = id;
            RequestKey = GetType() + id.ToString();
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new NumberMotionOfArtistRequest()
                              {
                                  artist_id = ArtistID
                              };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.NUMBER_MOTION_OF_ARTIST, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }

    public class GetNumberMotionSearchStoreRequest : StoreRequest
    {
        private string Query { get; set; }
        public GetNumberMotionSearchStoreRequest(string query)
        {
            Query = query;
            RequestKey = GetType() + query;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new NumberMotionSearchRequest()
                              {
                                  query = Query
                              };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.NUMBER_MOTION_SEARCH, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }

    public class GetNumberMotionDownloadStoreRequest : StoreRequest
    {
        private ulong UserID { get; set; }
        public GetNumberMotionDownloadStoreRequest(ulong id)
        {
            UserID = id;
            RequestKey = GetType() + id.ToString();
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new NumberMotionDownloadRequest()
                              {
                                  user_id = UserID
                              };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.NUMBER_MOTION_DOWNLOAD, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }

    public class GetNumberRatingInfoStoreRquest : StoreRequest
    {
        private ulong MotionID { get; set; }
        public GetNumberRatingInfoStoreRquest(ulong id)
        {
            MotionID = id;
            RequestKey = GetType() + id.ToString();
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new NumberRatingInfoRequest()
                              {
                                  motion_id = MotionID
                              };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.NUMBER_MOTION_RATING_INFO, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
