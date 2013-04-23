using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for RemoteControlScreen.xaml
    /// </summary>
    public partial class RemoteControlScreen : UserControl
    {
        public RemoteControlScreen()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
            var remoteViewModel = new RemoteControlScreenModel();
            DataContext = remoteViewModel;
            viewModel = ((RemoteControlScreenModel)(DataContext));

        }

        private RemoteControlScreenModel viewModel;

        private ObservableCollection<MotionTitleItem> AddRemoteItem()
        {
            var listItem = new ObservableCollection<MotionTitleItem>();
            for (int i = 0; i < 50; i++)
            {
                var motionTitleItem = new MotionTitleItem();
                motionTitleItem.MotionTitle.Text = "Motion title " + i;
                listItem.Add(motionTitleItem);
            }
            return listItem;
        }

        private void RemoteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel.LastSelectedMotionItem != null)
            {
                viewModel.LastSelectedMotionItem.ViewModel.RectangleFillColor = "Yellow";
            }
            viewModel.SelectedMotion.ViewModel.RectangleFillColor = "Red";
        }

        private void UnconnectedTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConnectMrobo();
            //GetListMotion();
        }
        private void ConnectMrobo()
        {
            var helloRequest = new RemoteRequest(RobotPacket.PacketID.Hello);
            helloRequest.ProcessSuccessfully += (s) =>
            {
                MessageBox.Show("Connect successfully!", "Connect to mRobo via Wireless connection", MessageBoxButton.OK);
                Dispatcher.BeginInvoke((Action)delegate
                                                   {
                                                       UnconnectedTextBox.Visibility = Visibility.Hidden;
                                                   });
            };
            helloRequest.ProcessError += (e, msg) =>
                                             {
                                                 MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                             };

            GlobalVariables.MRoboConnectionWorker.AddJob(helloRequest);
        }

        private void GetListMotion()
        {
            var listAllMotionRequest = new ListAllMotionRequest();
            listAllMotionRequest.ProcessSuccessfully += (s) =>
                                                            {
                                                                foreach (MotionInfo info in s)
                                                                {
                                                                    Console.WriteLine("MotionID: {0}", info.MotionID);
                                                                }
                                                            };
            listAllMotionRequest.ProcessError += (e, msg) =>
            {

            };
            GlobalVariables.MRoboConnectionWorker.AddJob(listAllMotionRequest);
        }
    }
}