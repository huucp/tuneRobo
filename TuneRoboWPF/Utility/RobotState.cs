namespace TuneRoboWPF.Utility
{
    public class RobotState
    {
        public enum TransformState
        {
            Closed = 0,
            Openning = 1,
            Closing = 2,
            Opened = 3
        }
        public enum MusicState
        {
            MusicIdled = 0,
            MusicPlaying = 1,
            MusicPaused = 2,
            SystemHalt = 3
        }
        public int CurrentVolume { get; set; }
        public TransformState CurrentTransformState { get; set; }
        public MusicState CurrentMusicState { get; set; }
        public ulong CurrentMotionID { get; set; }
        public int CurrentMotionIndex { get; set; }

        public void UpdateState(byte[] state)
        {
            if (state == null) return;
            CurrentVolume = state[0];
            CurrentTransformState = (TransformState)state[1];
            CurrentMusicState = (MusicState)state[2];
            CurrentMotionID = GlobalFunction.LE8ToDec(GlobalFunction.SplitByteArray(state, 3, 8));
            FindCurrentMotionPlayingIndex();
        }

        public void FindCurrentMotionPlayingIndex()
        {
            if (GlobalVariables.CurrentListMotion == null) return;
            CurrentMotionIndex = 0;
            for (int i = 0; i < GlobalVariables.CurrentListMotion.Count; i++)
            {
                if (CurrentMotionID == GlobalVariables.CurrentListMotion[i].MotionID)
                {
                    CurrentMotionIndex = i;
                    break;
                }
            }
        }

        public RobotState()
        {
            CurrentVolume = 5;
            CurrentTransformState = TransformState.Closed;
            CurrentMusicState = MusicState.SystemHalt;
            CurrentMotionID = 0;
            CurrentMotionIndex = 0;
        }
    }
}
