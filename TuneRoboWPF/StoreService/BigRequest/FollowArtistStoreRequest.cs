using System.IO;
using ProtoBuf;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using comm;
using user;

namespace TuneRoboWPF.StoreService.BigRequest
{
    public class FollowArtistStoreRequest : IRequest
    {
        public delegate void SuccessfullyEventHandler(object sender);

        public event SuccessfullyEventHandler ProcessSuccessfully;

        private void OnProcessSuccessfully(object sender)
        {
            SuccessfullyEventHandler handler = ProcessSuccessfully;
            if (handler != null) handler(sender);
        }

        public delegate void ErrorEventHandler(Reply.Type errorCode, string errorMessage);

        public event ErrorEventHandler ProcessError;

        private void OnProcessError(Reply.Type errorCode, string errorMessage)
        {
            ErrorEventHandler handler = ProcessError;
            if (handler != null) handler(errorCode, errorMessage);
        }
        private ulong ArtistID { get; set; }
        public FollowArtistStoreRequest(ulong artistID)
        {
            ArtistID = artistID;
        }
        public object Process()
        {
            var relationRequest = new GetUserArtistRelationStoreRequest(ArtistID);
            var relationReply = (Reply)relationRequest.Process();
            if (relationReply.type != (decimal)Reply.Type.OK)
            {
                OnProcessError((Reply.Type)relationReply.type, "Get user-artist relationship failed");
                return null;
            }

            bool isFollow = (relationReply.user_relation.rel == UserRelationReply.Rel.FOLLOW);
            var followRequest = isFollow
                                    ? new FollowStoreRequest(FollowRequest.Type.UNFOLLOW, ArtistID)
                                    : new FollowStoreRequest(FollowRequest.Type.FOLLOW, ArtistID);
            var followReply = (Reply)followRequest.Process();
            if (followReply.type != (decimal)Reply.Type.OK)
            {
                OnProcessError((Reply.Type)followReply.type, "Follow artist request failed");
                return null;
            }
            OnProcessSuccessfully(isFollow);
            return null;
        }
    }
}
