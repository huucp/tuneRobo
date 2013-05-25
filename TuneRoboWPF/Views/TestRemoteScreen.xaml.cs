using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for TestRemoteScreen.xaml
    /// </summary>
    public partial class TestRemoteScreen : UserControl
    {
        private DockPanel MainWindowDock { get; set; }
        public TestRemoteScreen(DockPanel dock)
        {
            InitializeComponent();
            MainWindowDock = dock;

            var r = new Random();
            int index = 0;
            foreach (var file in Directory.GetFiles(GlobalFunction.GetSavedDir(), "*.mrb"))
            {
                var motionInfo = new MotionInfo(file);
                var motionItem = new MotionFullInfoItem();
                motionItem.SetMotionInfo(motionInfo);
                motionItem.ViewModel.RatingValue = r.Next(1, 5) / 5.0;
                motionItem.ViewModel.HitTestVisible = false;
                motionItem.ViewModel.Index = ++index;
                Library.Children.Add(motionItem);

            }
        }
    }
}
