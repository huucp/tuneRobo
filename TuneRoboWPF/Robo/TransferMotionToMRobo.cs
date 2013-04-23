using System;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Robo
{
    public class TransferMotionToMRobo : IRequest
    {
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

        public TransferMotionToMRobo(ulong id)
        {
            MotionID = id;
            CancelProcess = false;
        }

        // Transfer process:
        // 1. Create pair
        // 2. Send packets for motion file
        // 3. Close motion transfer
        // 4. Send packets for music file
        // 5. End:
        //  a. Close pair: success, save motion pair
        //  b. Cancel: cancel the operation, data won't be stored in mRobo,
        //  this command could be called any time after "Create pair" command

        public int Process()
        {
            const int trunkDataSize = 4096;
            byte[] musicData = GlobalFunction.FileToByteArray(GlobalVariables.LOCAL_DIR + MotionID + ".mp3");
            byte[] motionData = GlobalFunction.FileToByteArray(GlobalVariables.LOCAL_DIR + MotionID + ".mrb");

            // Create pair
            if (CreatePair() != 1)
            {
                Console.WriteLine("Create pair false");
                return 0;
            }

            // Transfer motion file
            int numberOfMotionTrunk = motionData.Length / trunkDataSize;
            int remainderMotionTrunkSize = motionData.Length % trunkDataSize;
            for (int i = 0; i < numberOfMotionTrunk; i++)
            {
                byte[] trunkMotionData = GlobalFunction.SplitByteArray(motionData, i * trunkDataSize, trunkDataSize);
                if (WriteData(trunkMotionData, mRoboPacket.PacketID.WriteMotionData) != 1) return 0;
            }

            if (remainderMotionTrunkSize != 0)
            {
                byte[] remainderMotionData = GlobalFunction.SplitByteArray(motionData, numberOfMotionTrunk * trunkDataSize,
                                                                          remainderMotionTrunkSize);
                if (WriteData(remainderMotionData, mRoboPacket.PacketID.WriteMotionData) != 1) return 0;
            }

            // Close motion file
            if (CloseMotionFile() != 1)
            {
                return 0;
            }

            // Transfer music file
            int numberOfMusicTrunk = musicData.Length / trunkDataSize;
            int remainderMusicTrunkSize = musicData.Length % trunkDataSize;
            for (int i = 0; i < numberOfMusicTrunk; i++)
            {
                byte[] trunkMusicData = GlobalFunction.SplitByteArray(musicData, i * trunkDataSize, trunkDataSize);
                if (WriteData(trunkMusicData, mRoboPacket.PacketID.WriteMusicData) != 1)
                {
                    return 0;
                }
            }
            if (remainderMusicTrunkSize != 0)
            {
                byte[] remainderMusicData = GlobalFunction.SplitByteArray(musicData, numberOfMusicTrunk * trunkDataSize,
                                                                     remainderMusicTrunkSize);
                if (WriteData(remainderMusicData, mRoboPacket.PacketID.WriteMusicData) != 1)
                {
                    return 0;
                }
            }

            // Close pair
            if (ClosePair() != 1)
            {
                return 0;
            }
            return 1;
        }

        private int CreatePair()
        {
            var createPairRequest = new CreatePairRequest(MotionID);
            return createPairRequest.Process();
        }

        private int WriteData(byte[] data, mRoboPacket.PacketID packetID)
        {
            if (CancelProcess)
            {
                SendCancel();
                return 0;
            }
            var writeMotionDataRequest = new WriteDataRequest(data, packetID);
            return writeMotionDataRequest.Process() == data.Length ? 1 : 0;
        }

        private int CloseMotionFile()
        {
            var closeMotionFileRequest = new CloseMotionFileRequest();
            return closeMotionFileRequest.Process();
        }

        private int ClosePair()
        {
            var closePairRequest = new ClosePairRequest();
            return closePairRequest.Process();
        }

        private int SendCancel()
        {
            var cancelRequest = new CancelRequest();
            return cancelRequest.Process();
        }

        private class CreatePairRequest : mRoboRequest
        {
            private ulong MotionID { get; set; }
            public CreatePairRequest(ulong id)
            {
                MotionID = id;
            }

            public override byte[] BuildRequest()
            {
                var packet = new mRoboPacket(mRoboPacket.PacketID.CreateMotionPair);
                packet.Parameters = GlobalFunction.DecToLE8(MotionID);
                return packet.BuildPacket();
            }
        }

        private class WriteDataRequest : mRoboRequest
        {
            private byte[] Data { get; set; }
            private mRoboPacket.PacketID PacketID { get; set; }
            public WriteDataRequest(byte[] data, mRoboPacket.PacketID packetID)
            {
                Data = data;
                PacketID = packetID;
            }

            public override byte[] BuildRequest()
            {
                var packet = new mRoboPacket(PacketID);
                packet.Parameters = Data;
                return packet.BuildPacket();
            }
        }

        private class CloseMotionFileRequest : mRoboRequest
        {
            public override byte[] BuildRequest()
            {
                var packet = new mRoboPacket(mRoboPacket.PacketID.CloseMotionFile);
                return packet.BuildPacket();
            }
        }

        private class ClosePairRequest : mRoboRequest
        {
            public override byte[] BuildRequest()
            {
                var packet = new mRoboPacket(mRoboPacket.PacketID.CloseMotionPair);
                return packet.BuildPacket();
            }
        }

        private class CancelRequest:mRoboRequest
        {
            public override byte[] BuildRequest()
            {
                var packet = new mRoboPacket(mRoboPacket.PacketID.CancelTransferMotionPair);
                return packet.BuildPacket();
            }
        }
    }
}
