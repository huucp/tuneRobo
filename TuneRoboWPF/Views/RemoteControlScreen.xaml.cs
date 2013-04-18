using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF
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
            ObservableCollection<MotionTitleItem> remoteListItem = AddRemoteItem();
            viewModel = ((RemoteControlScreenModel) (DataContext));

            viewModel.RemoteListItem = remoteListItem;
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
    }
}