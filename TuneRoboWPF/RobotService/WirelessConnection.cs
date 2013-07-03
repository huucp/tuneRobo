using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using MessageBoxUtils;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.RobotService
{
    public class WirelessConnection
    {        
        private Socket Connection;
        public const int MaxCrcRetry =3 ;
        public WirelessConnection()
        {
            Connection= new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.IP);
        }
        

        /// <summary>
        /// Config socket and connect to mRobo
        /// </summary>
        /// <returns>1 if success, 0 if failed</returns>
        public int ConfigAndConnectSocket()
        {
            Connection.SendTimeout = GlobalVariables.Timeout;
            Connection.ReceiveTimeout = GlobalVariables.Timeout;

            var titleError = (string)Application.Current.TryFindResource("NetworkErrorText");
            var msgError = (string)Application.Current.TryFindResource("CheckNetworkText");


            if (GlobalVariables.WirelessIP == null)
            {
                WPFMessageBox.Show(StaticMainWindow.Window,msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error,MessageBoxResult.OK);
                Debug.Fail("IP is invalid");
                return 0;
            }
            if (GlobalVariables.WirelessPort == -1)
            {
                WPFMessageBox.Show(StaticMainWindow.Window, msgError, titleError, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                Debug.Fail("Wireless port is invalid");
                return 0;
            }

            var clientIpAddress = IPAddress.Parse(GlobalVariables.WirelessIP);
            var remoteEndPoint = new IPEndPoint(clientIpAddress, GlobalVariables.WirelessPort);
            try
            {
                IAsyncResult result = Connection.BeginConnect(remoteEndPoint, null, null);

                bool success = result.AsyncWaitHandle.WaitOne(1000, true);

                if (!success)
                {
                    Connection.Close();
                    //throw new ApplicationException("Failed to connect server.");
                }
            }
            catch (SocketException e)
            {
                Debug.Fail(e.Message);
                return 0;
            }
            return Connection.Connected ? 1 : 0;
        }

        public void ReleaseConnection()
        {
            Connection.Shutdown(SocketShutdown.Both);
            Connection.Close();
        }

        public void SendPacket(byte[] packet)
        {
            Connection.Send(packet);
        }

        public RobotReply ReceiveReply()
        {
            var receive = new byte[1024];
            var ret = 0;
            int temp;
            var tempReceive = new byte[1024];
            var listByte = new List<byte>();
            do
            {
                Array.Clear(tempReceive, 0, 1024);
                temp = Connection.Receive(tempReceive);
                ret += temp;
                listByte.AddRange(GlobalFunction.SplitByteArray(tempReceive, 0, temp));
            } while (temp == 1024);
            receive = listByte.ToArray();

            if (ret == 0)
            {
                Console.WriteLine("Receive nothing!.");
                return null;
            }
            var hearderIndex = GlobalFunction.FindPacketHeader(receive);
            byte[] receivePacket = GlobalFunction.SplitByteArray(receive, hearderIndex, ret - hearderIndex);

            if (!GlobalFunction.CheckCRC(receivePacket))
            {
                Console.WriteLine("CRC error");
                return null;
            }
            return new RobotReply(receivePacket);
        }

        public RobotReply SendAndReceivePacket(byte[] sendPacket)
        {
            try
            {
                SendPacket(sendPacket);
            }
            catch (SocketException se)
            {                
                return null;
            }

            try
            {
                var reply = ReceiveReply();
                return reply;
            }
            catch (SocketException se)
            {                
                return null;
            }
        }
    }
}
