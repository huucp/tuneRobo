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
        public delegate void SuccessfullyEventHandler(ReplyData replyData);

        public event SuccessfullyEventHandler ProcessSuccessfully;

        private void OnProcessSuccessfully(ReplyData replyData)
        {
            SuccessfullyEventHandler handler = ProcessSuccessfully;
            if (handler != null) handler(replyData);
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
            ReplyNull = 2
        }

        public WirelessConnection Conn;
        public RobotPacket.PacketID RequestID { get; set; }

        /// <summary>
        /// Setup wireless connection
        /// </summary>
        /// <returns>true if success and false if failed</returns>
        private bool SetupConnection()
        {
            Conn = new WirelessConnection();
            return (Conn.ConfigAndConnectSocket() == 1);
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
            var replyData = new ReplyData();
            if (!SetupConnection())
            {
                OnProcessError(ErrorCode.SetupConnection, "Setup connection error");
                return replyData;
            }
            byte[] packet = BuildRequest();

            try
            {
                Conn.SendPacket(packet);
            }
            catch (SocketException se)
            {
                OnProcessError(ErrorCode.SocketException, se.Message);
                return replyData;
            }

            RobotReply reply = null;
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
                OnProcessSuccessfully(replyData);
            }
            else
            {
                OnProcessError(ErrorCode.ReplyNull,"Reply cannot be null");
            }
            ReleaseConenction();
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
