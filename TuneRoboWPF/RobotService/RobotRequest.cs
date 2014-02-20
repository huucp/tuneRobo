using System.Diagnostics;
using System.Net.Sockets;
using TuneRoboWPF.Utility;
using NLog;

namespace TuneRoboWPF.RobotService
{
    public class RobotRequest : IRequest
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

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
            if (!Conn.SocketAlive)
            {
                return (Conn.ConfigAndConnectSocket() == 1);
            }
            return true;
        }

        /// <summary>
        /// Release connection
        /// </summary>
        private void ReleaseConnection()
        {
            Conn.ReleaseConnection();
        }

        /// <summary>
        /// Request and reply process
        /// </summary>
        /// <returns>Data of reply</returns>
        public object Process()
        {
            RobotReplyData replyData = null;
            if (!SetupConnection())
            {
                OnProcessError(ErrorCode.SetupConnection, "Setup connection error");
                _logger.Error("Cannot connect to robot");
                return null;
            }
            byte[] packet = BuildRequest();
            RobotReply reply = null;
            int crcCount = 0;
            bool crcError = false;

            do
            {
                crcCount++;
                //if (GlobalVariables.Sw != null && GlobalVariables.Sw.IsRunning)
                //{
                //    GlobalVariables.Sw.Stop();
                //    _logger.Info("1 processing session duration: " + GlobalVariables.Sw.ElapsedMilliseconds);
                //}
                try
                {
                    Conn.SendPacket(packet);
                }
                catch (SocketException se)
                {
                    OnProcessError(ErrorCode.SocketException, se.Message);
                    _logger.Error("SendPacket socket exception: " + se.Message);
                    return replyData;
                }

                // For testing transfer speed
                //OnProcessSuccessfully(null);
                //return replyData;

                try
                {
                    reply = Conn.ReceiveReply();
                }
                catch (SocketException se)
                {
                    OnProcessError(ErrorCode.SocketException, se.Message);
                    _logger.Error("ReceiveReply socket exception: " + se.Message);
                    return replyData;
                }
                
                //if (GlobalVariables.Sw == null || !GlobalVariables.Sw.IsRunning) GlobalVariables.Sw = Stopwatch.StartNew();
                if (reply != null)
                {
                    reply.RequestID = RequestID;
                    replyData = reply.Process();

                    switch (replyData.Type)
                    {
                        case RobotReplyData.ReplyType.Success:
                            OnProcessSuccessfully(replyData);
                            _logger.Info("Receive reply from robot: type successfully");
                            break;
                        case RobotReplyData.ReplyType.Failed:
                            OnProcessError(ErrorCode.ReplyFailed, "Reply error");
                            _logger.Error("Receive reply from robot: type failed");
                            break;
                        case RobotReplyData.ReplyType.WrongID:
                            OnProcessError(ErrorCode.WrongSessionID, "Wrong session ID");
                            _logger.Error("Receive reply from robot: type wrong session ID");
                            break;
                        case RobotReplyData.ReplyType.CRC:
                            crcError = true;
                            _logger.Error("Receive reply from robot: type CRC error");
                            break;
                    }
                }
                else
                {
                    OnProcessError(ErrorCode.ReplyNull, "Reply cannot be null");
                    _logger.Error("Receive reply from robot: reply null");
                }
            } while (crcCount < WirelessConnection.MaxCrcRetry && crcError);

            //ReleaseConnection();
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
