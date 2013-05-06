using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using TuneRoboWPF.Utility;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace TuneRoboWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            GlobalFunction.ReadConfig();            
        }
    }
}
