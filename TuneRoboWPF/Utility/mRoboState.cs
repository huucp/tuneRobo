namespace TuneRoboWPF.Utility
{
    public class mRoboState
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
        public int CurrentMotionID { get; set; }

        public void UpdateState(byte[] state)
        {
            CurrentVolume = state[0];
            CurrentTransformState = (TransformState)state[1];
            CurrentMusicState = (MusicState) state[2];
            CurrentMotionID = (int)GlobalFunction.LE8ToDec(GlobalFunction.SplitByteArray(state, 3, 8));
        }

        public mRoboState()
        {
            CurrentVolume = 5;
            CurrentTransformState = TransformState.Closed;
            CurrentMusicState = MusicState.SystemHalt;
            CurrentMotionID = 0;
        }
    }
}
