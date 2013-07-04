using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.StoreService
{
    public class StoreConnection
    {
        private const string IPServer = "183.91.7.159";
        //private const string IPServer = "192.168.1.51";
        private const int PortServer = 8769;
        private const int MagicByte = 0xEE;
        private const int HeaderSize = 8;
        public static int RetryTime = 5;

        public Socket Connection;
        private static readonly Lazy<StoreConnection> Lazy = new Lazy<StoreConnection>(() => new StoreConnection());

        public static StoreConnection Instance { get { return Lazy.Value; } }
        private static ulong packetID = 0;
        //private List<MessageType.Type> listID = new List<MessageType.Type>();
        const int BufferLength = 100000;
        public static int DataChunkSize = 32768;

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

                bool success = result.AsyncWaitHandle.WaitOne(1000, true);

                if (!success)
                {
                    //Connection.Shutdown(SocketShutdown.Both);
                    Connection.Close();
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
                var receive = GlobalFunction.SplitByteArray(buffer, 0, ret);
                packet.AddRange(receive);
                //Console.WriteLine("Receive {0:d} bytes", ret);

                while (ret < 4)
                {
                    var ret_ = Connection.Receive(buffer);
                    ret += ret_;
                    var receive_ = GlobalFunction.SplitByteArray(buffer, 0, ret_);
                    packet.AddRange(receive_);
                }

                var tmpSize = GlobalFunction.SplitByteArray(buffer, 2, 2);
                var size = GlobalFunction.BE2ToDec(tmpSize);

                while (ret < size)
                {
                    //Console.WriteLine("need receive more {0:d} bytes.", size - ret);
                    var ret_ = Connection.Receive(buffer);
                    ret += ret_;
                    var receive_ = GlobalFunction.SplitByteArray(buffer, 0, ret_);
                    packet.AddRange(receive_);
                }
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.TimedOut)
                {
                    Console.WriteLine("Receive timed out!");
                    return null;
                }
            }
            return packet.ToArray();
        }


        public object RequestAndReply(object msg)
        {
            try
            {
                Connection.Send(Encode(msg));
            }
            catch (SocketException)
            {
                return null;
            }
            var buffer = new byte[BufferLength];
            var packet = new List<byte>();
            var ret = 0;
            try
            {
                ret = Connection.Receive(buffer);
                var receive = GlobalFunction.SplitByteArray(buffer, 0, ret);
                packet.AddRange(receive);

                while (ret < 4)
                {
                    var ret_ = Connection.Receive(buffer);
                    ret += ret_;
                    var receive_ = GlobalFunction.SplitByteArray(buffer, 0, ret_);
                    packet.AddRange(receive_);
                }

                var tmpSize = GlobalFunction.SplitByteArray(buffer, 2, 2);
                var size = GlobalFunction.BE2ToDec(tmpSize);
                
                while (ret < size)
                {
                    //Console.WriteLine("need receive more {0:d} bytes.", size - ret);
                    var ret_ = Connection.Receive(buffer);
                    ret += ret_;
                    var receive_ = GlobalFunction.SplitByteArray(buffer, 0, ret_);
                    packet.AddRange(receive_);
                }
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.TimedOut)
                {
                    Console.WriteLine("Receive timed out!");
                    return null;
                }
            }

            return Decode(packet.ToArray(), ret);
        }

        /// <summary>
        /// Encode a request
        /// </summary>
        /// <param name="request">input request</param>
        /// <returns>packet</returns>
        private byte[] Encode(object request)
        {
            if (request == null) throw new ArgumentNullException();
            var stream = new MemoryStream();
            var type = 1;

            var data = stream.ToArray();
            packetID++;
            return BuildServerPacket(data.Length + 16, type, 2, data, packetID);
        }

        /// <summary>
        /// Decode a packet
        /// </summary>
        /// <param name="reply">reply packet</param>
        /// <param name="packetSize">packet sizes</param>
        /// <returns>nothing</returns>
        private object Decode(byte[] reply, int packetSize)
        {
            if (reply == null) return null;
            if (reply[0] != MagicByte || reply[1] != MagicByte) return null;

            var tmpSize = GlobalFunction.SplitByteArray(reply, 2, 2);
            if (GlobalFunction.BE2ToDec(tmpSize) != packetSize) return null;

            var tmpType = GlobalFunction.SplitByteArray(reply, 4, 3);

            var tmpID = GlobalFunction.SplitByteArray(reply, 8, 8);
            var id = GlobalFunction.BE8ToDec(tmpID);

            var tmpProfile = GlobalFunction.SplitByteArray(reply, 16, packetSize - 16);
            
            return null;
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
