﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TuneRoboWPF.Utility;
using NLog;

namespace TuneRoboWPF.StoreService
{
    public class StoreConnection
    {
        // Log
#if DEBUG
        private static Logger logger = LogManager.GetCurrentClassLogger(); 
#endif

        // Local server
        //private const string IPServer = "183.91.7.159";
        //private const string IPServer = "192.168.1.51";
        //private const int PortServer = 8769;

        // Amazon server
        private const string IPServer = "54.213.2.129";
        private const int PortServer = 1234;

        private const int MagicByte = 0xEE;
        private const int HeaderSize = 8;

        public static int RetryTime = 5;

        public Socket Connection;
        private static readonly Lazy<StoreConnection> Lazy = new Lazy<StoreConnection>(() => new StoreConnection());

        public static StoreConnection Instance { get { return Lazy.Value; } }
        private static ulong packetID = 0;
        //private List<MessageType.Type> listID = new List<MessageType.Type>();
        const int BufferLength = 17000;
        public static int DataChunkSize = 16384;

        private StoreConnection()
        {
            Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        public int ConfigAndConnectSocket()
        {
            var clientIpAddress = IPAddress.Parse(IPServer);
            var remoteEndPoint = new IPEndPoint(clientIpAddress, PortServer);
            //Connection.ExclusiveAddressUse = true;
            try
            {
                Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP)
                                 {
                                     ReceiveTimeout = GlobalVariables.Timeout,
                                     SendTimeout = GlobalVariables.Timeout
                                 };
                //Connection.Connect(remoteEndPoint);
                IAsyncResult result = Connection.BeginConnect(remoteEndPoint, null, null);

                bool success = result.AsyncWaitHandle.WaitOne(3000, true);

                if (!success)
                {
                    //Connection.Shutdown(SocketShutdown.Both);
                    //Connection.Close();
                    //Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    //throw new ApplicationException("Failed to connect server.");
                }
            }
            catch (SocketException)
            {
                return 0;
            }
            return Connection.Connected ? 1 : 0;
        }

        public void SendPacket(byte[] packet)
        {
            Connection.Send(packet);
        }

        public byte[] ReceiveReply()
        {
            var buffer = new byte[BufferLength];
            var packet = new List<byte>();
            try
            {
                var ret = Connection.Receive(buffer);
#if DEBUG
                //logger.Info(ret); 
#endif
                if (ret == 0) return null;
                var receive = GlobalFunction.SplitByteArray(buffer, 0, ret);
                packet.AddRange(receive);
                //Console.WriteLine("Receive {0:d} bytes", ret);

                while (ret < 4)
                {
                    //if (Connection.Available == 0) break;
                    var ret_ = Connection.Receive(buffer);
#if DEBUG
                    logger.Info(ret_); 
#endif
                    if (ret_ == 0) break;
                    ret += ret_;
                    var receive_ = GlobalFunction.SplitByteArray(buffer, 0, ret_);
                    packet.AddRange(receive_);
                }

                var tmpSize = GlobalFunction.SplitByteArray(buffer, 2, 2);
                var size = GlobalFunction.BE2ToDec(tmpSize);

                while (ret < size)
                {
                    //Console.WriteLine("need receive more {0:d} bytes.", size - ret);
                    //if (Connection.Available == 0) break;
                    var ret_ = Connection.Receive(buffer);
#if DEBUG
                    logger.Info(ret_); 
#endif
                    if (ret_ == 0) break;
                    ret += ret_;
                    var receive_ = GlobalFunction.SplitByteArray(buffer, 0, ret_);
                    packet.AddRange(receive_);
                }
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.TimedOut)
                {
#if DEBUG
                    logger.Error(se.Message); 
#endif
                    DebugHelper.WriteLine("Receive timed out!");
                    return null;
                }
            }
            return packet.ToArray();
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

        // Build a packet
        public static byte[] BuildServerPacket(int size, int type, int meta, byte[] data, ulong id = 0)
        {
            var packet = new byte[size];

            // Magic bytes
            packet[0] = 0xEE;
            packet[1] = 0xEE;

            // Size
            var tmpSize = GlobalFunction.DecToBE2(size);
            packet[2] = tmpSize[0];
            packet[3] = tmpSize[1];

            // Type
            var tmpType = GlobalFunction.DecToBE3(type);
            packet[4] = tmpType[0];
            packet[5] = tmpType[1];
            packet[6] = tmpType[2];

            // Meta
            packet[7] = (byte)meta;

            var pos = 8;
            // ID
            if (id > 0)
            {
                var tmpId = GlobalFunction.DecToBE8(id);
                packet[8] = tmpId[0];
                packet[9] = tmpId[1];
                packet[10] = tmpId[2];
                packet[11] = tmpId[3];
                packet[12] = tmpId[4];
                packet[13] = tmpId[5];
                packet[14] = tmpId[6];
                packet[15] = tmpId[7];
                pos = 16;
            }

            Buffer.BlockCopy(data, 0, packet, pos, data.Length);

            return packet;
        }
    }
}
