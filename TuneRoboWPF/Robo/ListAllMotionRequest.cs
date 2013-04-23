using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Robo
{
    class ListAllMotionRequest : IRequest
    {
        public delegate void SuccessfullyEventHandler(List<MotionInfo> listMotionInfo);

        public event SuccessfullyEventHandler ProcessSuccessfully;

        private void OnProcessSuccessfully(List<MotionInfo> listMotionInfo)
        {
            SuccessfullyEventHandler handler = ProcessSuccessfully;
            if (handler != null) handler(listMotionInfo);
        }

        public delegate void ErrorEventHandler(object sender);

        public event ErrorEventHandler ProcessError;

        private void OnProcessError(object sender)
        {
            ErrorEventHandler handler = ProcessError;
            if (handler != null) handler(sender);
        }

        private WirelessConnection Connection = new WirelessConnection();
        private bool SetupConnection()
        {
            return (Connection.ConfigAndConnectSocket() == 1);
        }
        public int Process()
        {
            if (!SetupConnection())
            {
                OnProcessError(null);
                return 0;
            }
            int motionCount = GetMotionCount();
            if (motionCount==-1)
            {
                OnProcessError(null);
                return 0;
            }
            var listMotionInfo = new List<MotionInfo>();
            for (int i = 0; i < motionCount; i++ )
            {
                MotionInfo info = GetMotionInfoAtIndex(i);
                if (info==null)
                {
                    OnProcessError(null);
                    return 0;
                }
                listMotionInfo.Add(GetMotionInfoAtIndex(i));
            }
            OnProcessSuccessfully(listMotionInfo);
            return 1;
        }


        private int GetMotionCount()
        {
            int motionCount = -1;
            var getMotionCountRequest = new GetMotionCountRequest(mRoboPacket.PacketID.CountMotions);
            mRoboReply reply = Connection.SendAndReceivePacket(getMotionCountRequest.BuildRequest());
            reply.RequestID = mRoboPacket.PacketID.CountMotions; 
            byte[] replyParameter;
            int result = reply.Process(out replyParameter);
            if (result == 1)
            {
                return (int)GlobalFunction.LE4ToDec(replyParameter);
            }
            return motionCount;
        }

        private MotionInfo GetMotionInfoAtIndex(int motionIndex)
        {
            var getMotionInfoAtIndexRequest = new GetMotionInfoAtIndexRequest(
                mRoboPacket.PacketID.GetInfoMotionAtIndex, motionIndex);
            mRoboReply reply = Connection.SendAndReceivePacket(getMotionInfoAtIndexRequest.BuildRequest());
            reply.RequestID = mRoboPacket.PacketID.GetInfoMotionAtIndex;
            byte[] replyParameter;
            int result = reply.Process(out replyParameter);
            if (result == 0)
            {
                return null;
            }
            return new MotionInfo(replyParameter);
        }
    }
}
