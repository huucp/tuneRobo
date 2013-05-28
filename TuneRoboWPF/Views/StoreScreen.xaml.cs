using System;
using System.Collections.Generic;
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
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using motion;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for StoreScreen.xaml
    /// </summary>
    public partial class StoreScreen : UserControl
    {
        private DockPanel MainWindowDockPanel { get; set; }
        private StoreScreenViewModel ViewModel { get; set; }
        public StoreScreen(DockPanel dock)
        {
            InitializeComponent();
            
            MainWindowDockPanel = dock;
            DataContext = new StoreScreenViewModel();
            ViewModel = (StoreScreenViewModel) DataContext;

            GetHotList();
        }

        
        private List<MotionShortInfo> hotList = new List<MotionShortInfo>();
        private void GetHotList()
        {
            var hotListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.ALL, 0, 20);
            hotListRequest.ProcessSuccessfully += (reply) =>
            {
                hotList.AddRange(reply.list_motion.motion_short_info);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var info in reply.list_motion.motion_short_info)
                    {
                        var motionItem = new MotionItemVertical();
                        motionItem.SetInfo(info);
                        ViewModel.HotItemsList.Add(motionItem);
                    }
                });

            };
            hotListRequest.ProcessError += (reply, msg) =>
            {
                Console.WriteLine("Host list request failed " + msg);                
            };
            GlobalVariables.StoreWorker.AddJob(hotListRequest);
        }
    }
}
