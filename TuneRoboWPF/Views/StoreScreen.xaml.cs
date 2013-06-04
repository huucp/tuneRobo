using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using artist;
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
            ViewModel = (StoreScreenViewModel)DataContext;

            GlobalVariables.StoreWorker.IsClearWorker = false;
            GetHotList();
            GetFeaturedList();
            GetArtistList();
            GlobalVariables.StoreWorker.IsClearWorker = true;
        }

        private List<ArtistShortInfo> artistList = new List<ArtistShortInfo>();
        private void GetArtistList()
        {
            var artistCountRequest = new GetNumberAllArtistStoreRequest();
            artistCountRequest.ProcessSuccessfully += (reply) =>
            {
                var listArtistRequest = new ListAllArtistStoreRequest(0, reply.number_all_artist.number_artist - 1);
                listArtistRequest.ProcessSuccessfully += (listReply) =>
                {
                    artistList.AddRange(listReply.list_artist.artist_short_info);
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        foreach (var info in artistList)
                        {
                            var artistItem = new ArtistItemVertical();
                            artistItem.SetInfo(info);
                            artistItem.ArtistItemClicked += artistItem_ArtistItemClicked;
                            ViewModel.ArtistList.Add(artistItem);
                            DownloadImage(info.artist_id, info.avatar_url, artistItem);
                        }
                    });
                };
                listArtistRequest.ProcessError += (listReply, msg) => Debug.Assert(false, msg);
                GlobalVariables.StoreWorker.AddJob(listArtistRequest);
            };
            artistCountRequest.ProcessError += (reply, msg) => Debug.Assert(false, msg);
            GlobalVariables.StoreWorker.AddJob(artistCountRequest);

        }

        private void artistItem_ArtistItemClicked(ArtistItemVertical sender)
        {
            var lastElement = MainWindowDockPanel.Children[MainWindowDockPanel.Children.Count - 1];
            MainWindowDockPanel.Children.Remove(lastElement);
            var detailScreen = new ArtistDetailScreen();
            MainWindowDockPanel.Children.Add(detailScreen);
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
                        motionItem.MotionClicked += motionItem_MotionClicked;
                        ViewModel.HotItemsList.Add(motionItem);
                        DownloadImage(info.motion_id, info.icon_url, motionItem);
                    }
                });

            };
            hotListRequest.ProcessError += (reply, msg) =>
            {
                Console.WriteLine("Host list request failed " + msg);
            };
            GlobalVariables.StoreWorker.AddJob(hotListRequest);
        }

        private List<MotionShortInfo> featuredList = new List<MotionShortInfo>();
        private void GetFeaturedList()
        {
            var featuredListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.FEATURE, 0, 20);
            featuredListRequest.ProcessSuccessfully += (reply) =>
            {
                featuredList.AddRange(reply.list_motion.motion_short_info);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var info in reply.list_motion.motion_short_info)
                    {
                        var motionItem = new MotionItemVertical();
                        motionItem.SetInfo(info);
                        motionItem.MotionClicked += motionItem_MotionClicked;
                        ViewModel.FeaturedItemsList.Add(motionItem);
                        DownloadImage(info.motion_id, info.icon_url, motionItem);
                    }
                });
            };
            featuredListRequest.ProcessError += (reply, msg) => Debug.Assert(false, msg);
            GlobalVariables.StoreWorker.AddJob(featuredListRequest);
        }

        private void motionItem_MotionClicked(ulong motionID)
        {
            var lastElement = MainWindowDockPanel.Children[MainWindowDockPanel.Children.Count - 1];
            MainWindowDockPanel.Children.Remove(lastElement);
            var detailScreen = new MotionDetailScreen(MainWindowDockPanel,motionID);
            MainWindowDockPanel.Children.Add(detailScreen);
        }

        private void DownloadImage(ulong id, string url, object item)
        {
            var imageDownload = new ImageDownload( url);
            if (item is MotionItemVertical)
            {
                imageDownload.DownloadCompleted += ((MotionItemVertical)item).SetImage;
            }
            if (item is ArtistItemVertical)
            {
                imageDownload.DownloadCompleted += ((ArtistItemVertical)item).SetImage;
            }
            GlobalVariables.ImageDownloadWorker.AddDownload(imageDownload);
        }


        private void HotMotionListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MainScrollViewer_DoMouseWheel(e);
        }

        private void FeaturedMotionListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MainScrollViewer_DoMouseWheel(e);
        }

        private void ArtistLisBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MainScrollViewer_DoMouseWheel(e);
        }

        private void MainScrollViewer_DoMouseWheel(MouseWheelEventArgs e)
        {
            MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
        }

        private void HotMotionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
