﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Models
{
    public class StoreScreenModel
    {
        public ObservableCollection<MotionFullInfoItem> HotItemsList = new ObservableCollection<MotionFullInfoItem>();
    }
}
