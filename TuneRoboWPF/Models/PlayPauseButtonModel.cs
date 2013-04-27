using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TuneRoboWPF.Models
{
    public class PlayPauseButtonModel
    {
        public enum ButtonState
        {
            InActive,Play,Pause
        }
        public string ImageSource;
        public ButtonState State;
    }
}
