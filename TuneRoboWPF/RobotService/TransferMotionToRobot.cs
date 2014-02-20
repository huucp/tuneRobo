using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TuneRoboWPF.Utility;
using NLog;

namespace TuneRoboWPF.RobotService
{
    public class TransferMotionToRobot : IRequest
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private bool _writeProcess = true;

        public delegate void SuccessfullyEventHandler(object sender);

        public event SuccessfullyEventHandler ProcessSuccessfully;

        private void OnProcessSuccessfully(object sender)
        {
            SuccessfullyEventHandler handler = ProcessSuccessfully;
            if (handler != null) handler(sender);
        }

        public delegate void ErrorEventHandler(ErrorCode errorCode, string errorMessage);

        public event ErrorEventHandler ProcessError;

        private void OnProcessError(ErrorCode errorCode, string errorMessage)
        {
            ErrorEventHandler handler = ProcessError;
            if (handler != null) handler(errorCode, errorMessage);
        }

        public delegate void ProgressReportEventHandler(int progressValue);

        public event ProgressReportEventHandler ProgressReport;
        private void OnProgessReport(int progressValue)
        {
            ProgressReportEventHandler handler = ProgressReport;
            if (handler != null) handler(progressValue);
        }

        public enum ErrorCode
        {
            FileNotExist,
            CreatePairError,
            WriteDataError,
            CloseMotionFileError,
            ClosePairError
        }
        private ulong MotionID { get; set; }
        private object cancelLock = new object();
        private bool _cancelProcess;
        public bool CancelProcess
        {
            get
            {
                lock (cancelLock)
                {
                    return _cancelProcess;
                }
            }
            set
            {
                lock (cancelLock)
                {
                    _cancelProcess = value;
                }
            }
        }


        public TransferMotionToRobot(ulong id)
        {
            MotionID = id;
            CancelProcess = false;
        }

        public static List<byte[]> SplitFile(string inputFile, int chunkSize)
        {
            //const int bufferSize = 20 * 1024;
            var list = new List<byte[]>();
            var buffer = new List<byte>();

            var data = File.ReadAllBytes(inputFile);
            //int index = 0;

            int beginIndex = 0;
            int endIndex = chunkSize - 1;

            for (int i = 0; i < data.Length; i++)
            {
                if (beginIndex <= i && i <= endIndex)
                {
                    buffer.Add(data[i]);
                }
                if (i == endIndex)
                {
                    list.Add(buffer.ToArray());
                    buffer.Clear();
                    beginIndex += chunkSize;
                    endIndex += chunkSize;
                    if (endIndex >= data.Length) endIndex = data.Length - 1;
                }
            }
            return list;
        }

        //private StreamWriter writer = new StreamWriter(@"WriteMotion.txt");
        // Transfer process:
        // 1. Create pair
        // 2. Send packets for motion file
        // 3. Close motion transfer
        // 4. Send packets for music file
        // 5. Close pair: success, save motion pair
        // 6. Cancel: cancel the operation, data won't be stored in mRobo,
        //  this command could be called any time after "Create pair" command
        public object Process()
        {
            const int trunkDataSize = 4096;
            string musicPath =
                Path.Combine(GlobalVariables.LOCAL_DIR + GlobalVariables.FOLDER_ROOT + GlobalVariables.FOLDER_PLAYLIST,
                             MotionID.ToString() + ".mp3");
            byte[] musicData = GlobalFunction.FileToByteArray(musicPath);
            if (musicData == null)
            {
                OnProcessError(ErrorCode.FileNotExist, "Music file not exist");
                if (_writeProcess)
                {
                    _logger.Error("Music file not exist");
                    DebugHelper.WriteLine("Music file not exist");
                }
                return null;
            }
            string motionPath =
                Path.Combine(GlobalVariables.LOCAL_DIR + GlobalVariables.FOLDER_ROOT + GlobalVariables.FOLDER_PLAYLIST,
                             MotionID.ToString() + ".mrb");
            byte[] motionData = GlobalFunction.FileToByteArray(motionPath);
            if (motionData == null)
            {
                OnProcessError(ErrorCode.FileNotExist, "Motion file not exist");
                if (_writeProcess)
                {
                    _logger.Error("Motion file not exist");
                    DebugHelper.WriteLine("Motion file not exist");
                }
                return null;
            }

            

            int numberOfMotionTrunk = motionData.Length / trunkDataSize;
            int numberOfMusicTrunk = musicData.Length / trunkDataSize;

            int totalPackets = numberOfMotionTrunk + numberOfMusicTrunk + 2;
            int count = 0;
            int currentPercentage = -1;
            int tempPercentage = 0;
            

            List<byte[]> listMotionTrunk = SplitFile(motionPath, trunkDataSize);
            
            List<byte[]> listMusicTrunk = SplitFile(musicPath, trunkDataSize);           

            // Create pair
            if (CreatePair() != 1)
            {
                OnProcessError(ErrorCode.CreatePairError, "Create pair false");
                if (_writeProcess)
                {
                    _logger.Error("Create pair false");
                    DebugHelper.WriteLine("Create pair false");
                }
                return 0;
            }

            if (_writeProcess)
            {
                _logger.Info("Create pair done");
                DebugHelper.WriteLine("Create pair done");
            }
            // Transfer motion
            foreach (var trunk in listMotionTrunk)
            {
                var writeResult = WriteData(trunk, RobotPacket.PacketID.WriteMotionData);
                if (writeResult == 0)
                {
                    OnProcessError(ErrorCode.WriteDataError, "Write motion error");
                    if (_writeProcess)
                    {
                        _logger.Error("Write motion error");
                        DebugHelper.WriteLine("Write motion error");
                    }
                    return 0;
                }
                if (writeResult == 2)
                {
                    return 0;
                }
                count++;

                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);
                    if (_writeProcess)
                    {
                        DebugHelper.WriteLine("Percentages: " + tempPercentage);
                        _logger.Info("Percentages: " + tempPercentage);
                    }
                }
            }

            // Close motion file
            if (CloseMotionFile() != 1)
            {
                OnProcessError(ErrorCode.CloseMotionFileError, "Close motion file error");
                if (_writeProcess)
                {
                    DebugHelper.WriteLine("Close motion file error");
                    _logger.Error("Close motion file error");
                }
                return 0;
            }
            if (_writeProcess)
            {
                DebugHelper.WriteLine("Transfer motion file done");
                _logger.Info("Transfer motion file done");
            }

            // Transfer music
            foreach (var trunk in listMusicTrunk)
            {
                var writeResult = WriteData(trunk, RobotPacket.PacketID.WriteMusicData);
                if (writeResult == 0)
                {
                    OnProcessError(ErrorCode.WriteDataError, "Write music error");
                    if (_writeProcess)
                    {
                        DebugHelper.WriteLine("Write music error");
                        _logger.Error("Write music error");
                    }
                    return 0;
                }
                if (writeResult == 2)
                {
                    return 0;
                }
                count++;
                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);
                    if (_writeProcess)
                    {
                        DebugHelper.WriteLine("Percentages: " + tempPercentage);
                        _logger.Info("Percentages: " + tempPercentage);
                    }
                }
            }

            //Close pair
            if (ClosePair() != 1)
            {
                OnProcessError(ErrorCode.ClosePairError, "Close pair error");
                if (_writeProcess)
                {
                    DebugHelper.WriteLine("Close pair error");
                    _logger.Error("Close pair error");
                }
                return 0;
            }
            //writer.Close();
            if (_writeProcess)
            {
                DebugHelper.WriteLine("Close pair done");
                _logger.Info("Close pair done");
            }
            OnProgessReport(100);
            OnProcessSuccessfully(null);
            return 1;
        }

        private void WriteToText(byte[] data)
        {
            var s = GlobalFunction.ByteArrayToHexString(data);
            //writer.WriteLine(s);
        }

        private int CreatePair()
        {
            crcCount++;
            var createPairRequest = new CreatePairRequest(MotionID);
            //var bytes = createPairRequest.BuildRequest();
            //writer.WriteLine("Create pair command");
            //WriteToText(bytes);
            //return 1;
            if (crcCount == WirelessConnection.MaxCrcRetry)
            {
                crcCount = 0;
                return 0;
            }
            var reply = createPairRequest.Process() as RobotReplyData;
            if (reply == null) return 0;
            if (reply.Type == RobotReplyData.ReplyType.CRC)
            {
                return CloseMotionFile();
            }
            else if (reply.Type != RobotReplyData.ReplyType.Success) return 0;

            crcCount = 0;
            return 1;
        }

        private int crcCount = 0;

        private int WriteData(byte[] data, RobotPacket.PacketID packetID)
        {
            crcCount++;
            if (CancelProcess)
            {
                SendCancel();
                return 2;
            }
            var writeMotionDataRequest = new WriteDataRequest(data, packetID);
            //var bytes = writeMotionDataRequest.BuildRequest();
            //SendData(bytes);
            //System.Threading.Thread.Sleep(5);
            //return 1;
            if (crcCount == WirelessConnection.MaxCrcRetry)
            {
                crcCount = 0;
                if (_writeProcess)
                {
                    DebugHelper.WriteLine("WriteData: reach max crc");
                    _logger.Error("WriteData: reach max crc");
                }

                return 0;
            }
            var reply = writeMotionDataRequest.Process() as RobotReplyData;
            if (reply == null)
            {
                if (_writeProcess)
                {
                    DebugHelper.WriteLine("WriteData: reply is null");
                    _logger.Error("WriteData: reply is null");
                }

                return 0;
            }

            // Check reply error
            if (reply.Type == RobotReplyData.ReplyType.CRC)
            {
                DebugHelper.WriteLine("WriteData: CRC error");
                _logger.Error("WriteData: CRC error");
                return WriteData(data, packetID);
            }
            else if (reply.Type != RobotReplyData.ReplyType.Success)
            {
                if (_writeProcess)
                {
                    DebugHelper.WriteLine("WriteData: reply is not success");
                    _logger.Error("WriteData: reply is not success");
                }

                return WriteData(data, packetID);
                //return 0;
            }

            if (reply.Data.Length == 0)
            {
                if (_writeProcess) _logger.Error("WriteData: reply data is null");
                return 0;
            }
            if (GlobalFunction.LE4ToDec(reply.Data) != data.Length)
            {
                if (_writeProcess)
                {
                    DebugHelper.WriteLine("WriteData: cannot write all data");
                    _logger.Error("WriteData: cannot write all data");
                }

                return 0;
            }
            crcCount = 0;
            return 1;
        }

        private void SendData(byte[] bytes)
        {
            try
            {
                RobotRequest.Conn.SendPacket(bytes);
            }
            catch (Exception e)
            {
                if (_writeProcess) _logger.Error("SendData error:" + e.Message);
                return;
            }

        }

        private int CloseMotionFile()
        {
            crcCount++;
            var closeMotionFileRequest = new CloseMotionFileRequest();
            //var bytes = closeMotionFileRequest.BuildRequest();
            //writer.WriteLine("Close motion file command");
            //WriteToText(bytes);
            //return 1;
            if (crcCount == WirelessConnection.MaxCrcRetry)
            {
                crcCount = 0;
                return 0;
            }
            var reply = closeMotionFileRequest.Process() as RobotReplyData;
            if (reply == null) return 0;
            if (reply.Type == RobotReplyData.ReplyType.CRC)
            {
                return CloseMotionFile();
            }
            else if (reply.Type != RobotReplyData.ReplyType.Success) return 0;

            crcCount = 0;
            return 1;
        }

        private int ClosePair()
        {
            crcCount++;
            var closePairRequest = new ClosePairRequest();
            //var bytes = closePairRequest.BuildRequest();
            //writer.WriteLine("Close pair command");
            //WriteToText(bytes);
            //return 1;

            if (crcCount == WirelessConnection.MaxCrcRetry)
            {
                crcCount = 0;
                return 0;
            }
            var reply = closePairRequest.Process() as RobotReplyData;
            if (reply == null) return 0;
            if (reply.Type == RobotReplyData.ReplyType.CRC)
            {
                return ClosePair();
            }
            else if (reply.Type != RobotReplyData.ReplyType.Success) return 0;

            crcCount = 0;
            return 1;
        }

        private int SendCancel()
        {
            //return 1;
            crcCount++;
            var cancelRequest = new CancelRequest();
            if (crcCount == WirelessConnection.MaxCrcRetry)
            {
                crcCount = 0;
                return 0;
            }
            var reply = cancelRequest.Process() as RobotReplyData;
            if (reply == null)
            {
                if (_writeProcess)
                {
                    DebugHelper.WriteLine("SendCancel: reply null");
                    _logger.Error("SendCancel: reply null");
                }
                return 0;
            }
            if (reply.Type == RobotReplyData.ReplyType.CRC)
            {
                if (_writeProcess)
                {
                    _logger.Error("SendCancel: CRC error");
                    DebugHelper.WriteLine("SendCancel: CRC");
                }
                return SendCancel();
            }
            else if (reply.Type != RobotReplyData.ReplyType.Success)
            {
                if (_writeProcess)
                {
                    _logger.Error("SendCancel: not success");
                    DebugHelper.WriteLine("SendCancel: not success");
                }
                return 0;
            }

            crcCount = 0;
            return 1;
        }

        private class CreatePairRequest : RobotRequest
        {
            private ulong MotionID { get; set; }
            public CreatePairRequest(ulong id)
            {
                MotionID = id;
                RequestID = RobotPacket.PacketID.CreateMotionPair;
            }

            public override byte[] BuildRequest()
            {
                var packet = new RobotPacket(RobotPacket.PacketID.CreateMotionPair);
                packet.Parameters = GlobalFunction.DecToLE8(MotionID);
                return packet.BuildPacket();
            }
        }

        private class WriteDataRequest : RobotRequest
        {
            private byte[] Data { get; set; }
            private RobotPacket.PacketID PacketID { get; set; }
            public WriteDataRequest(byte[] data, RobotPacket.PacketID packetID)
            {
                Data = data;
                PacketID = packetID;
                RequestID = packetID;
            }

            public override byte[] BuildRequest()
            {
                var packet = new RobotPacket(PacketID);
                packet.Parameters = Data;
                return packet.BuildPacket();
            }
        }

        private class CloseMotionFileRequest : RobotRequest
        {
            public CloseMotionFileRequest()
            {
                RequestID = RobotPacket.PacketID.CloseMotionFile;
            }
            public override byte[] BuildRequest()
            {
                var packet = new RobotPacket(RobotPacket.PacketID.CloseMotionFile);
                return packet.BuildPacket();
            }
        }

        private class ClosePairRequest : RobotRequest
        {
            public ClosePairRequest()
            {
                RequestID = RobotPacket.PacketID.CloseMotionPair;
            }
            public override byte[] BuildRequest()
            {
                var packet = new RobotPacket(RobotPacket.PacketID.CloseMotionPair);
                return packet.BuildPacket();
            }
        }

        private class CancelRequest : RobotRequest
        {
            public CancelRequest()
            {
                RequestID = RobotPacket.PacketID.CancelTransferMotionPair;
            }
            public override byte[] BuildRequest()
            {
                var packet = new RobotPacket(RobotPacket.PacketID.CancelTransferMotionPair);
                return packet.BuildPacket();
            }
        }
    }
}
