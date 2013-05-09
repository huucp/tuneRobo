using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using TuneRoboWPF.StoreService.BigRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.StoreService.SimpleRequest;
using motion;
using UserControl = System.Windows.Controls.UserControl;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for MainStoreScreen.xaml
    /// </summary>
    public partial class TempStoreScreen : UserControl
    {
        public TempStoreScreen()
        {
            this.InitializeComponent();
            DataContext = new TempStoreScreenViewModel();
            viewModel = (TempStoreScreenViewModel)DataContext;
            viewModel.FeaturedItemsList.Clear();
            viewModel.HotItemsList.Clear();
            viewModel.ArtistItemsList.Clear();
        }
        private TempStoreScreenViewModel viewModel = new TempStoreScreenViewModel();

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Cursor = Cursors.Wait;
                GetHotList();
                GetFeaturedList();
                GetArtistList();
            }
        }

        private bool getHotListDone, getFeaturedListDone, getArtistListDone;
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
                        var motionTitleItem = new MotionTitleItem();
                        motionTitleItem.ViewModel.Title = info.title;
                        viewModel.HotItemsList.Add(motionTitleItem);
                    }

                    if (getFeaturedListDone && getArtistListDone) Cursor = Cursors.Arrow;
                    else getHotListDone = true;
                });

            };
            hotListRequest.ProcessError += (reply, msg) =>
                                               {
                                                   Console.WriteLine("Host list request failed " + msg);
                                                   if (getFeaturedListDone && getArtistListDone)
                                                   {
                                                       Dispatcher.BeginInvoke((Action) delegate
                                                                                           {
                                                                                               Cursor = Cursors.Arrow;
                                                                                           });
                                                   }
                                                   else getHotListDone = true;
                                               };
            GlobalVariables.StoreWorker.AddJob(hotListRequest);
        }

        private void GetFeaturedList()
        {
            var featuredListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.FEATURE, 0, 20);
            featuredListRequest.ProcessSuccessfully += (reply) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {                    
                    foreach (var info in reply.list_motion.motion_short_info)
                    {
                        var motionTitleItem = new MotionTitleItem();
                        motionTitleItem.ViewModel.Title = info.title;
                        viewModel.FeaturedItemsList.Add(motionTitleItem);
                    }
                    if (getHotListDone && getArtistListDone) Cursor = Cursors.Arrow;
                    else getFeaturedListDone = true;
                });

            };
            featuredListRequest.ProcessError += (reply, msg) =>
            {
                Console.WriteLine("Featured list request failed " + msg);
                if (getHotListDone && getArtistListDone)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Cursor = Cursors.Arrow;
                    });
                }
                else getFeaturedListDone = true;
            };
            GlobalVariables.StoreWorker.AddJob(featuredListRequest);
        }

        private void GetArtistList()
        {
            var artistListRequest = new ListAllArtistRequest(0, 20);
            artistListRequest.ProcessSuccessfully += (reply) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var info in reply.list_artist.artist_short_info)
                    {
                        var motionTitleItem = new MotionTitleItem();
                        motionTitleItem.ViewModel.Title = info.artist_name;
                        viewModel.ArtistItemsList.Add(motionTitleItem);
                    }
                    if (getHotListDone && getFeaturedListDone) Cursor = Cursors.Arrow;
                    else getArtistListDone = true;
                });

            };
            artistListRequest.ProcessError += (reply, msg) =>
            {
                Console.WriteLine("Artist list request failed " + msg);
                if (getHotListDone && getFeaturedListDone)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Cursor = Cursors.Arrow;
                    });
                }
                else getArtistListDone = true;
            };
            GlobalVariables.StoreWorker.AddJob(artistListRequest);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            ulong motionID = hotList[HotListBox.SelectedIndex].motion_id;
            var request = new DownloadMotionStoreRequest(motionID);
            var transferWindow = new TransferWindow(request, motionID.ToString());
            transferWindow.ShowDialog();

        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            ulong motionID = hotList[HotListBox.SelectedIndex].motion_id;
            var infoRequest = new GetMotionFullInfoStoreRequest(motionID);
            infoRequest.ProcessSuccessfully += (reply) =>
                                                   {
                                                       Console.WriteLine("Motion ID: " +reply.motion_info.motion_id);
                                                       Console.WriteLine("Title: " +reply.motion_info.title);
                                                       Console.WriteLine("Description: " +reply.motion_info.description);
                                                       Console.WriteLine("Artist ID: " +reply.motion_info.artist_id);
                                                       Console.WriteLine("Artist name: " +reply.motion_info.artist_name);
                                                       Console.WriteLine("Publisher ID: " +reply.motion_info.publisher_id);
                                                       Console.WriteLine("Version name: " +reply.motion_info.version_name);
                                                       Console.WriteLine("Version ID: " +reply.motion_info.version_id);
                                                       Console.WriteLine("Video url: " +reply.motion_info.video_url);
                                                       Console.WriteLine("Screen shot url: " +reply.motion_info.screenshoot_ulrs[0]);
                                                       Console.WriteLine("View: " +reply.motion_info.view);
                                                       Console.WriteLine("Download count: " +reply.motion_info.download_count);
                                                       Console.WriteLine("Rating: " +reply.motion_info.rating);
                                                       Console.WriteLine("Rating count: " + reply.motion_info.rating_count);
                                                       Console.WriteLine("Motion file size: " + reply.motion_info.motion_file_size);
                                                       Console.WriteLine("Music file size: " + reply.motion_info.music_file_size);
                                                       Console.WriteLine("Duration: " + reply.motion_info.duration);
                                                       Console.WriteLine("Last modified: " + reply.motion_info.lastmodified);
                                                       Console.WriteLine("Update ID: " + reply.motion_info.update_id);
                                                   };
            infoRequest.ProcessError += (reply,msg) =>
                                            {
                                                Console.WriteLine("Get motion info failed: " + msg);
                                            };
            GlobalVariables.StoreWorker.AddJob(infoRequest);
        }
    }
}