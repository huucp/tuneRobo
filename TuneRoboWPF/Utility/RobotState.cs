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
        }

        public void FindCurrentMotionPlayingIndex()
        {
            if (GlobalVariables.CurrentListMotion == null) return;
            MotionIndex = 0;
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
            Volume = 5;
            TransformState = TransformStates.Closed;
            MusicState = MusicStates.SystemHalt;
            MotionID = 0;
            MotionIndex = 0;
        }
    }
}
