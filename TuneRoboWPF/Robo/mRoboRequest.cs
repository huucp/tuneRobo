using System;
using System.Net.Sockets;
using System.Windows;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Robo
{
    public class mRoboRequest : IRequest
    {
        public delegate void SuccessfullyEventHandler(byte[] replyParameter);

        public event SuccessfullyEventHandler ProcessSuccessfully;

        private void OnProcessSuccessfully(byte[] sender)
        {
            SuccessfullyEventHandler handler = ProcessSuccessfully;
            if (handler != null) handler(sender);
        }

        public delegate void ErrorEventHandler(object sender);

        public event ErrorEventHandler ProcessError;

        private void OnProcessError(object sender)
        {
            ErrorEventHandler handler = ProcessError;
            if (handler != null) handler(sender);
        }

        public WirelessConnection Conn;
        public mRoboPacket.PacketID RequestID { get; set; }
        //public int ReturnResult { get; set; }

        private bool SetupConnection()
        {
            //if (Conn.GetConnected()) return true;
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
        /// A complete request and reply
        /// </summary>
        /// <returns></returns>
        public int Process()
        {
            int ReturnResult = 0;
            if (!SetupConnection())
            {
                //MessageBox.Show("Connection error", "", MessageBoxButton.OK, MessageBoxImage.Error);
                OnProcessError(null);
                return 0;
            }

            var packet = BuildRequest();

            try
            {
                Conn.SendPacket(packet);
            }
            catch (SocketException se)
            {
                OnProcessError(null);
                return 0;
            }

            mRoboReply reply = null;
            try
            {
                reply = Conn.ReceiveReply();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
                OnProcessError(null);
                return 0;
            }
            if (reply != null)
            {
                reply.RequestID = RequestID;
                byte[] replyParameter;
                ReturnResult = reply.Process(out replyParameter);
                OnProcessSuccessfully(replyParameter);
            }
            else
            {
                OnProcessError(null);
                ReturnResult = 0;
            }
            ReleaseConenction();
            return ReturnResult;
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