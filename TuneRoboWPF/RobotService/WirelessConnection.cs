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
using NLog;

namespace TuneRoboWPF.RobotService
{
    public class WirelessConnection
    {
        private bool _needDebugger = true;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private Socket Connection;
        public const int MaxCrcRetry = 6;
        public WirelessConnection()
        {
            Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }


        /// <summary>
        /// Config socket and connect to mRobo
        /// </summary>
        /// <returns>1 if success, 0 if failed</returns>
        public int ConfigAndConnectSocket()
        {
            Connection.SendTimeout = GlobalVariables.Timeout;
            Connection.ReceiveTimeout = GlobalVariables.Timeout;

            var titleError = (string)Application.Current.TryFindResource("WirelessConnectionkErrorText");
            var msgError = (string)Application.Current.TryFindResource("CheckNetworkText");


            //if (GlobalVariables.WirelessIP == null)
            //{
            //    Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            //                                                         WPFMessageBox.Show(StaticMainWindow.Window,
            //                                                                            msgError, titleError,
            //                                                                            MessageBoxButton.OK,
            //                                                                            MessageBoxImage.Error,
            //                                                                            MessageBoxResult.OK)));

            //    Debug.Fail("IP is invalid");
            //    return 0;
            //}
            //if (GlobalVariables.WirelessPort == -1)
            //{
            //    Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            //                                                         WPFMessageBox.Show(StaticMainWindow.Window,
            //                                                                            msgError, titleError,
            //                                                                            MessageBoxButton.OK,
            //                                                                            MessageBoxImage.Error,
            //                                                                            MessageBoxResult.OK)));
            //    Debug.Fail("Wireless port is invalid");
            //    return 0;
            //}

            //var clientIpAddress = IPAddress.Parse(GlobalVariables.WirelessIP);
            //var remoteEndPoint = new IPEndPoint(clientIpAddress, GlobalVariables.WirelessPort);
            var clientIpAddress = IPAddress.Parse("169.254.1.1");
            var remoteEndPoint = new IPEndPoint(clientIpAddress, 6868);
            try
            {
                IAsyncResult result = Connection.BeginConnect(remoteEndPoint, null, null);

                bool success = result.AsyncWaitHandle.WaitOne(1000, true);

                //if (!success)
                //{
                //    Connection.Close();
                //}
            }
            catch (SocketException e)
            {
                if (_needDebugger) _logger.Fatal("Connect socket error");
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
            int temp = 0;
            var tempReceive = new byte[1024];
            var listByte = new List<byte>();
            do
            {
                Array.Clear(tempReceive, 0, 1024);
                try
                {
                    temp = Connection.Receive(tempReceive);
                }
                catch (SocketException se)
                {
                    DebugHelper.WriteLine(se.Message);
                    if (_needDebugger) _logger.Error("Receive: socket exception: " + se.Message);
                    return null;
                }
                ret += temp;
                listByte.AddRange(GlobalFunction.SplitByteArray(tempReceive, 0, temp));
            } while (temp == 1024);
            receive = listByte.ToArray();

            if (ret == 0)
            {
                DebugHelper.WriteLine("Receive nothing!.");
                if (_needDebugger) _logger.Error("Receive: nothing");
                return null;
            }
            var hearderIndex = GlobalFunction.FindPacketHeader(receive);
            byte[] receivePacket = GlobalFunction.SplitByteArray(receive, hearderIndex, ret - hearderIndex);

            //if (!GlobalFunction.CheckCRC(receivePacket))
            //{
            //    DebugHelper.WriteLine("CRC error");
            //    if (_needDebugger) _logger.Error("Receive: CRC error");
            //    return null;
            //}
            return new RobotReply(receivePacket);
        }

        public RobotReply SendAndReceivePacket(byte[] sendPacket)
        {
            if (!SocketAlive)
            {
                if (ConfigAndConnectSocket() != 1)
                {
                    if (_needDebugger)
                    {
                        _logger.Error("Cannot re-connect to robot");
                    }
                    return null;
                }
            }
            try
            {
                SendPacket(sendPacket);
            }
            catch (SocketException se)
            {
                if (_needDebugger) _logger.Error("Send: socket exception: " + se.Message);
                return null;
            }

            try
            {
                var reply = ReceiveReply();
                return reply;
            }
            catch (SocketException se)
            {
                if (_needDebugger) _logger.Error("Receive: ", "socket exception: " + se.Message);
                return null;
            }
        }

        /// <summary>
        /// Check socket alive
        /// s.Poll returns true if
        ///     connection is closed, reset, terminated or pending (meaning no active connection)
        ///     connection is active and there is data available for reading
        /// s.Available returns number of bytes available for reading
        ///     if both are true:
        ///     there is no data available to read so connection is not active
        /// </summary>
        /// <param name="s">socket</param>
        /// <returns>true if alive and against</returns>
        private bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if ((part1 & part2) || !s.Connected)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Get connection state
        /// </summary>
        public bool SocketAlive
        {
            get
            {
                return SocketConnected(Connection);
            }
        }

    }
}
