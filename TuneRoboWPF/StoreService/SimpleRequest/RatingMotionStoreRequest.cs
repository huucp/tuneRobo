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
    public class RatingMotionStoreRequest:StoreRequest
    {
        private ulong MotionID { get; set; }
        private uint Rating { get; set; }
        private ulong VersionID { get; set; }
        private string CommentTitle { get; set; }
        private string CommentContent { get; set; }
        public RatingMotionStoreRequest(ulong motionID, uint rating, ulong versionID, string commentTitle, string commentContent)
        {
            MotionID = motionID;
            Rating = rating;
            VersionID = versionID;
            CommentTitle = commentTitle;
            CommentContent = commentContent;
        }
        public override void BuildPacket()
        {
            base.BuildPacket();
            var request = new RatingRequest()
                              {
                                  user_id = GlobalVariables.CurrentUser.UserID,
                                  motion_id = MotionID,
                                  rating = Rating,
                                  version_id = VersionID,
                                  comment_title = CommentTitle,
                                  comment_content = CommentContent
                              };
            byte[] packetData;
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                packetData = stream.ToArray();
            }
            GlobalVariables.CountRequest++;
            Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.RATING, 2,
                                                       packetData, GlobalVariables.CountRequest);
        }
    }
}
