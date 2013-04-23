using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Robo
{
    public class GetLogFileFromMRobo:IRequest
    {
        public int Process()
        {

            return 1;
        }

        private class OpenLogFileRequet:mRoboRequest
        {
            public override byte[] BuildRequest()
            {
                var packet = new mRoboPacket(mRoboPacket.PacketID.OpenLogFile);
                return packet.BuildPacket();
            }
        }

        private class ReadLogFileRequest:mRoboRequest
        {
            private int NumberOfBytesToRead { get; set; }
            public ReadLogFileRequest(int numberOfBytes)
            {
                NumberOfBytesToRead = numberOfBytes;
            }
            public override byte[] BuildRequest()
            {
                var packet = new mRoboPacket(mRoboPacket.PacketID.ReadLogFile);
                packet.Parameters = GlobalFunction.DecToLE4(NumberOfBytesToRead);
                return packet.BuildPacket();
            }
        }

        private class CloseLogFileRequest:mRoboRequest
        {
            public override byte[] BuildRequest()
            {
                var packet = new mRoboPacket(mRoboPacket.PacketID.CloseLogFile);
                return packet.BuildPacket();
            }
        }
    }
}
