using System;

namespace TuneRoboWPF.Utility
{
    public class RobotState
    {
        public enum TransformStates
        {
            Closed = 0,
            Openning = 1,
            Closing = 2,
            Opened = 3
        }
        public enum MusicStates
        {
            MusicIdled = 0,
            MusicPlaying = 1,
            MusicPaused = 2,
            SystemHalt = 3
        }
        public int Volume { get; set; }
        public TransformStates TransformState { get; set; }
        public MusicStates MusicState { get; set; }
        public ulong MotionID { get; set; }
        public int MotionIndex { get; set; }

        public void UpdateState(byte[] state)
        {
            if (state == null) return;
            Volume = state[0];
            TransformState = (TransformStates)state[1];
            MusicState = (MusicStates)state[2];
            MotionID = GlobalFunction.LE8ToDec(GlobalFunction.SplitByteArray(state, 3, 8));
            FindCurrentMotionPlayingIndex();
            PrintRoboState();
        }

        private void PrintRoboState()
        {
            Console.WriteLine("================================================");
            Console.WriteLine("Volume: {0}",Volume);
            Console.WriteLine("Transform state: {0}",Enum.GetName(typeof(TransformStates),TransformState));
            Console.WriteLine("Music state: {0}", Enum.GetName(typeof(MusicStates), MusicState));
            Console.WriteLine("Motion ID: {0}",MotionID);
            Console.WriteLine("Play motion index: {0}",MotionIndex);
            Console.WriteLine("================================================");
        }

        public void FindCurrentMotionPlayingIndex()
        {
            if (GlobalVariables.CurrentListMotion == null) return;
            MotionIndex = -1;
            for (int i = 0; i < GlobalVariables.CurrentListMotion.Count; i++)
            {
                if (MotionID == GlobalVariables.CurrentListMotion[i].MotionID)
                {
                    MotionIndex = i;
                    break;
                }
            }
        }

        public RobotState()
        {
            Volume = 0;
            TransformState = TransformStates.Closed;
            MusicState = MusicStates.SystemHalt;
            MotionID = 0;
            MotionIndex = -1;
        }
    }
}
