﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.RobotService
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

        public delegate void ErrorEventHandler(ErrorCode errorCode, string errorMessage);

        public event ErrorEventHandler ProcessError;

        private void OnProcessError(ErrorCode errorCode, string errorMessage)
        {
            ErrorEventHandler handler = ProcessError;
            if (handler != null) handler(errorCode, errorMessage);
        }

        public enum ErrorCode
        {
            SetupConnection = 0,
            MotionCount = 1,
            MotionInfo = 2

        }
        private WirelessConnection Connection = new WirelessConnection();
        private bool SetupConnection()
        {
            return (Connection.ConfigAndConnectSocket() == 1);
        }
        public object Process()
        {
            if (!SetupConnection())
            {
                OnProcessError(ErrorCode.SetupConnection, "Setup connection error");
                return 0;
            }
            int motionCount = GetMotionCount();
            if (motionCount == -1)
            {
                OnProcessError(ErrorCode.MotionCount, "Motion count must be greater than -1");
                return 0;
            }
            var listMotionInfo = new List<MotionInfo>();
            for (int i = 0; i < motionCount; i++)
            {
                MotionInfo info = GetMotionInfoAtIndex(i);
                if (info == null)
                {
                    OnProcessError(ErrorCode.MotionInfo, "Motion info is null");
                    return 0;
                }
                listMotionInfo.Add(GetMotionInfoAtIndex(i));
            }
            OnProcessSuccessfully(listMotionInfo);
            Connection.ReleaseConnection();
            return 1;
        }


        private int GetMotionCount()
        {
            int motionCount = -1;
            var getMotionCountRequest = new GetMotionCountRequest(RobotPacket.PacketID.CountMotions);
            RobotReply reply = Connection.SendAndReceivePacket(getMotionCountRequest.BuildRequest());
            reply.RequestID = RobotPacket.PacketID.CountMotions;
            RobotReplyData robotReplyData = reply.Process();
            if (robotReplyData.Type == RobotReplyData.ReplyType.Success)
            {
                return (int)GlobalFunction.LE4ToDec(robotReplyData.Data);
            }
            return motionCount;
        }

        private MotionInfo GetMotionInfoAtIndex(int motionIndex)
        {
            var getMotionInfoAtIndexRequest = new GetMotionInfoAtIndexRequest(
                RobotPacket.PacketID.GetInfoMotionAtIndex, motionIndex);
            RobotReply reply = Connection.SendAndReceivePacket(getMotionInfoAtIndexRequest.BuildRequest());
            reply.RequestID = RobotPacket.PacketID.GetInfoMotionAtIndex;
            RobotReplyData robotReplyData = reply.Process();
            if (robotReplyData.Type == RobotReplyData.ReplyType.Failed)
            {
                return null;
            }
            return new MotionInfo(robotReplyData.Data);
        }
    }
}
