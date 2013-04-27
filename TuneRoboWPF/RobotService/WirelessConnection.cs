using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.RobotService
{
    public class WirelessConnection
    {        
        private Socket Connection;
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
            Connection.SendTimeout = GlobalVariables.TIMEOUT;
            Connection.ReceiveTimeout = GlobalVariables.TIMEOUT;

            if (GlobalVariables.IP_WIRELESS == null)
            {
                MessageBox.Show("IP address is invalid", "IP error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
            if (GlobalVariables.PORT_WIRELESS == -1)
            {
                MessageBox.Show("Port number is invalid", "Port number error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }

            var clientIpAddress = IPAddress.Parse(GlobalVariables.IP_WIRELESS);
            var remoteEndPoint = new IPEndPoint(clientIpAddress, GlobalVariables.PORT_WIRELESS);
            try
            {
                Connection.Connect(remoteEndPoint);
            }
            catch (SocketException)
            {
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
