using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.RobotService
{
    public class RobotRequest : IRequest
    {
        public delegate void SuccessfullyEventHandler(RobotReplyData robotReplyData);

        public event SuccessfullyEventHandler ProcessSuccessfully;

        private void OnProcessSuccessfully(RobotReplyData robotReplyData)
        {
            SuccessfullyEventHandler handler = ProcessSuccessfully;
            if (handler != null) handler(robotReplyData);
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
            SocketException = 1,
            ReplyNull = 2,
            ReplyFailed = 3,
            WrongSessionID = 4
        }

        public static WirelessConnection Conn = null;
        public RobotPacket.PacketID RequestID { get; set; }

        /// <summary>
        /// Setup wireless connection
        /// </summary>
        /// <returns>true if success and false if failed</returns>
        private bool SetupConnection()
        {
            if (Conn == null)
            {
                Conn = new WirelessConnection();
                return (Conn.ConfigAndConnectSocket() == 1);
            }
            return true;
        }

        /// <summary>
        /// Release connection
        /// </summary>
        private void ReleaseConenction()
        {
            Conn.ReleaseConnection();
        }

        /// <summary>
        /// Request and reply process
        /// </summary>
        /// <returns>Data of reply</returns>
        public object Process()
        {
            //var replyData = new RobotReplyData();
            RobotReplyData replyData = null;
            if (!SetupConnection())
            {
                OnProcessError(ErrorCode.SetupConnection, "Setup connection error");
                return replyData;
            }
            byte[] packet = BuildRequest();
            RobotReply reply = null;
            int crcCount = 0;
            bool crcError = false;

            do
            {
                crcCount++;
                try
                {
                    Conn.SendPacket(packet);
                }
                catch (SocketException se)
                {
                    OnProcessError(ErrorCode.SocketException, se.Message);
                    return replyData;
                }

                try
                {
                    reply = Conn.ReceiveReply();
                }
                catch (SocketException se)
                {
                    OnProcessError(ErrorCode.SocketException, se.Message);
                    return replyData;
                }
                if (reply != null)
                {
                    reply.RequestID = RequestID;
                    replyData = reply.Process();
                    
                    switch (replyData.Type)
                    {
                        case RobotReplyData.ReplyType.Success:
                            OnProcessSuccessfully(replyData);
                            break;
                        case RobotReplyData.ReplyType.Failed:
                            OnProcessError(ErrorCode.ReplyFailed, "Reply error");
                            break;
                        case RobotReplyData.ReplyType.WrongID:
                            OnProcessError(ErrorCode.WrongSessionID, "Wrong session ID");
                            break;
                        case RobotReplyData.ReplyType.CRC:
                            crcError = true;
                            break;
                    }                    
                }
                else
                {
                    OnProcessError(ErrorCode.ReplyNull, "Reply cannot be null");
                }
            } while (crcCount < WirelessConnection.MaxCrcRetry && crcError);

            //ReleaseConenction();
            return replyData;
        }
        public virtual byte[] BuildRequest()
        {
            return null;
        }
        #region Packet Header
        public byte IdentifyByte = 80;

        /// <summary>
        /// return length of data in bytes
        /// </summary>
        /// <param name="length">length of data</param>
        /// <returns>byte array of length parameter</returns>
        public byte[] GetDataLengthInBytes(int length)
        {
            return GlobalFunction.DecToLE2(length);
        }

        /// <summary>
        /// return cycle redundant check of data in bytes
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>CRC of data in byte array format</returns>
        public byte[] GetCrcInBytes(byte[] data)
        {
            return GlobalFunction.GenerateCrc(data);
        }

        public byte[] ReserveBytes = new byte[] { 0, 0, 0 };
        #endregion
    }
}
