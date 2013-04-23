using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Windows;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Robo
{
    public sealed class WirelessConnection
    {
        public Socket Connection;
        private static readonly Lazy<WirelessConnection> Lazy = new Lazy<WirelessConnection>(() => new WirelessConnection());
        public byte[] receivePacket;
        public static WirelessConnection CreateInstance { get { return Lazy.Value; } }
        public static WirelessConnection GetInstance()
        {
            var conn = CreateInstance;
            if (conn.GetConnected()) return conn;
            if (conn.ConfigAndConnectSocket() != 1)
            {
                return null;
            }
            //if (conn.checkmRoboExist() != 1)
            //{
            //    return null;
            //}
            //GlobalVariables.WIRELESS_CONNECTION = true;
            return conn;
        }

        // Constructor
        public WirelessConnection()
        {
            Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);            
        }

        /// <summary>
        /// Release connection
        /// </summary>
        public void ReleaseConnection()
        {            
            Connection.Shutdown(SocketShutdown.Both);
            Connection.Close();
        }

        public bool GetConnected()
        {
            return Connection.Connected;
        }

        public void SendPacket(byte[] packet)
        {
            Connection.Send(packet);
        }

        public mRoboReply ReceiveReply()
        {
            var receive = new byte[1024];
            var ret = 0;           
            int temp;
            var tempReceive = new byte[1024];
            var listByte = new List<byte>();
            do
            {
                Array.Clear(tempReceive,0,1024);
                temp = Connection.Receive(tempReceive);
                ret += temp;
                listByte.AddRange(GlobalFunction.SplitByteArray(tempReceive,0,temp));
            } while (temp == 1024);
            receive = listByte.ToArray();

            if (ret == 0)
            {
                Console.WriteLine("Receive nothing!.");
                return null;
            }
            var hearderIndex = GlobalFunction.findPacketHeader(receive);            
            receivePacket = GlobalFunction.SplitByteArray(receive, hearderIndex, ret - hearderIndex);

            if (!GlobalFunction.CheckCRC(receivePacket))
            {
                Console.WriteLine("CRC error");
                 return null;
            }
            return new mRoboReply(receivePacket);
        }


        public int ConfigAndConnectSocket()
        {
            Connection.SendTimeout = GlobalVariables.TIMEOUT;
            Connection.ReceiveTimeout = GlobalVariables.TIMEOUT;

            if (GlobalVariables.IP_WIRELESS == null)
            {
                MessageBox.Show("IP address is invalid", "IP error", MessageBoxButton.OK,MessageBoxImage.Error);
                return 0;
            }
            if (GlobalVariables.PORT_WIRELESS == -1)
            {
                MessageBox.Show("Port number is invalid", "Port number error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }

            var clientIpAddress = IPAddress.Parse(GlobalVariables.IP_WIRELESS);
            var remoteEndPoint = new IPEndPoint(clientIpAddress, GlobalVariables.PORT_WIRELESS);
            //Connection.ExclusiveAddressUse = true;
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

        public mRoboReply SendAndReceivePacket(byte[] sendPacket)
        {
            try
            {
                SendPacket(sendPacket);
            }
            catch (SocketException se)
            {
                //if (se.SocketErrorCode == SocketError.TimedOut)
                //{
                //    return null;
                //}
                return null;
            }

            try
            {
                var reply = ReceiveReply();
                return reply;
            }
            catch (SocketException se)
            {
                //if (se.SocketErrorCode == SocketError.TimedOut)
                //{
                //    return null;
                //}
                return null;
            }
        }
        
    }
}
