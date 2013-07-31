using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class SeeAllScreenModel
    {
        public string Category = string.Empty;
        public ObservableCollection<MotionItemVertical> CategoryList = new ObservableCollection<MotionItemVertical>();
        public bool NoResultVisibility = false;
        public string NoResultText = string.Empty;
    }
}
