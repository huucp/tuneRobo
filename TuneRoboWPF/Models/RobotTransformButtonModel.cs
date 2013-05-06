namespace TuneRoboWPF.Models
{
    public class RobotTransformButtonModel
    {
        public enum ButtonState
        {
            InActive, Transform, Untransform
        }
        public string ImageSource;
        public ButtonState State;
    }
}
