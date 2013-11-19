using System;
using System.IO;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.RobotService
{
    public class TransferMotionToRobot : IRequest
    {

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

        //private StreamWriter writer = new StreamWriter(@"WriteMotion.txt");
        // Transfer process:
        // 1. Create pair
        // 2. Send packets for motion file
        // 3. Close motion transfer
        // 4. Send packets for music file
        // 5. End:
        //  a. Close pair: success, save motion pair
        //  b. Cancel: cancel the operation, data won't be stored in mRobo,
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
                return null;
            }
            string motionPath =
                Path.Combine(GlobalVariables.LOCAL_DIR + GlobalVariables.FOLDER_ROOT + GlobalVariables.FOLDER_PLAYLIST,
                             MotionID.ToString() + ".mrb");
            byte[] motionData = GlobalFunction.FileToByteArray(motionPath);
            if (motionData == null)
            {
                OnProcessError(ErrorCode.FileNotExist, "Motion file not exist");
                return null;
            }

            // Create pair
            if (CreatePair() != 1)
            {
                OnProcessError(ErrorCode.CreatePairError, "Create pair false");
                return 0;
            }
#if DEBUG
            Console.WriteLine("Create pair done");
#endif
            int numberOfMotionTrunk = motionData.Length / trunkDataSize;
            int numberOfMusicTrunk = musicData.Length / trunkDataSize;

            int totalPackets = numberOfMotionTrunk + numberOfMusicTrunk + 2;
            int count = 0;
            int currentPercentage = -1;
            int tempPercentage = 0;

            // Transfer motion file
            int remainderMotionTrunkSize = motionData.Length % trunkDataSize;
            for (int i = 0; i < numberOfMotionTrunk; i++)
            {
                byte[] trunkMotionData = GlobalFunction.SplitByteArray(motionData, i * trunkDataSize, trunkDataSize);
                if (WriteData(trunkMotionData, RobotPacket.PacketID.WriteMotionData) != 1)
                {
                    OnProcessError(ErrorCode.WriteDataError, "Write motion error");
                    return 0;
                }
                count++;
                DebugHelper.WriteLineDebug(count);
                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);
                }
            }

            if (remainderMotionTrunkSize != 0)
            {
                byte[] remainderMotionData = GlobalFunction.SplitByteArray(motionData, numberOfMotionTrunk * trunkDataSize,
                                                                          remainderMotionTrunkSize);
                if (WriteData(remainderMotionData, RobotPacket.PacketID.WriteMotionData) != 1)
                {
                    OnProcessError(ErrorCode.WriteDataError, "Write motion error");
                    return 0;
                }
                count++;
                DebugHelper.WriteLineDebug(count);
                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);
                }
            }

            // Close motion file
            if (CloseMotionFile() != 1)
            {
                OnProcessError(ErrorCode.CloseMotionFileError, "Close motion file error");
                return 0;
            }

#if DEBUG
            Console.WriteLine("Transfer motion file done");
#endif
            // Transfer music file

            int remainderMusicTrunkSize = musicData.Length % trunkDataSize;
            for (int i = 0; i < numberOfMusicTrunk; i++)
            {
                byte[] trunkMusicData = GlobalFunction.SplitByteArray(musicData, i * trunkDataSize, trunkDataSize);
                if (WriteData(trunkMusicData, RobotPacket.PacketID.WriteMusicData) != 1)
                {
                    OnProcessError(ErrorCode.WriteDataError, "Write music error");
                    return 0;
                }
                count++;
                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);
                }
            }
            if (remainderMusicTrunkSize != 0)
            {
                byte[] remainderMusicData = GlobalFunction.SplitByteArray(musicData, numberOfMusicTrunk * trunkDataSize,
                                                                     remainderMusicTrunkSize);
                if (WriteData(remainderMusicData, RobotPacket.PacketID.WriteMusicData) != 1)
                {
                    OnProcessError(ErrorCode.WriteDataError, "Write music error");
                    return 0;
                }
                count++;
                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);
                }
            }

            // Close pair
            if (ClosePair() != 1)
            {
                OnProcessError(ErrorCode.ClosePairError, "Close pair error");
                return 0;
            }
            //writer.Close();
#if DEBUG
            Console.WriteLine("Done");
#endif
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
                return 0;
            }
            var writeMotionDataRequest = new WriteDataRequest(data, packetID);
            //var bytes = writeMotionDataRequest.BuildRequest();
            //writer.WriteLine("Write data command");
            //WriteToText(bytes);
            //return 1;
            if (crcCount == WirelessConnection.MaxCrcRetry)
            {
                crcCount = 0;
#if DEBUG
                Console.WriteLine("WriteData: reach max crc");
#endif
                return 0;
            }
            var reply = writeMotionDataRequest.Process() as RobotReplyData;
            if (reply == null)
            {
#if DEBUG
                Console.WriteLine("WriteData: reply is null");
#endif
                return 0;
            }

            // Check reply error
            if (reply.Type == RobotReplyData.ReplyType.CRC)
            {
                return WriteData(data, packetID);
            }
            else if (reply.Type != RobotReplyData.ReplyType.Success)
            {
#if DEBUG
                Console.WriteLine("WriteData: reply is not success");
#endif
                return WriteData(data, packetID);
                //return 0;
            }

            if (GlobalFunction.LE4ToDec(reply.Data) != data.Length)
            {
#if DEBUG
                Console.WriteLine("WriteData: cannot write all data");
#endif
                return 0;
            }
            crcCount = 0;
            return 1;
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
            crcCount++;
            var cancelRequest = new CancelRequest();
            if (crcCount == WirelessConnection.MaxCrcRetry)
            {
                crcCount = 0;
                return 0;
            }
            var reply = cancelRequest.Process() as RobotReplyData;
            if (reply == null) return 0;
            if (reply.Type == RobotReplyData.ReplyType.CRC)
            {
                return SendCancel();
            }
            else if (reply.Type != RobotReplyData.ReplyType.Success) return 0;

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
