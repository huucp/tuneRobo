using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using artist;
using motion;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for NewStoreScreen.xaml
    /// </summary>
    public partial class NewStoreScreen : UserControl
    {
        private NewStoreScreenViewModel ViewModel = new NewStoreScreenViewModel();
        public NewStoreScreen()
        {
            InitializeComponent();

            DataContext = new NewStoreScreenViewModel();
            ViewModel = (NewStoreScreenViewModel) DataContext;
        }

        public void SetInfo(bool newScreen = true)
        {
            if (!newScreen) return;// Get from Navigation System

            var screen = new Screen(Screen.ScreenType.StoreScreen);
            GlobalVariables.Navigation.AddScreen(screen);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalVariables.ImageDownloadWorker.ClearAll();
            StaticMainWindow.Window.ShowLoadingScreen();

            GetHotList();
            GetArtistList();
            GetFeaturedList();
        }

        private void GetHotList()
        {
            var hotListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.ALL, 0, 15);
            hotListRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var info in reply.list_motion.motion_short_info)
                    {
                        var motionItem = new MotionHorizontalItem();
                        motionItem.SetInfo(info);
                        motionItem.MotionClicked += motionItem_MotionClicked;
                        ViewModel.HotMotionsList.Add(motionItem);
                        DownloadImage(info.motion_id, info.icon_url, motionItem);
                    }
                });
            hotListRequest.ProcessError += (reply, msg) =>
            {
                Debug.Assert(false, msg);
                Dispatcher.BeginInvoke((Action)(() => StaticMainWindow.Window.ShowErrorScreen()));
            };
            GlobalVariables.StoreWorker.ForceAddRequest(hotListRequest);
        }

        private void GetArtistList()
        {
            var artistCountRequest = new GetNumberAllArtistStoreRequest();
            artistCountRequest.ProcessSuccessfully += (reply) =>
            {
                var listArtistRequest = new ListAllArtistStoreRequest(0, reply.number_all_artist.number_artist - 1);
                listArtistRequest.ProcessSuccessfully += (listReply) =>
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        foreach (var info in listReply.list_artist.artist_short_info)
                        {
                            var artistItem = new ArtistItemVertical();
                            artistItem.SetInfo(info);
                            artistItem.ArtistItemClicked += artistItem_ArtistItemClicked;
                            ViewModel.ArtistsList.Add(artistItem);
                            DownloadImage(info.artist_id, info.avatar_url, artistItem);
                        }                        
                    });
                listArtistRequest.ProcessError += (listReply, msg) =>
                {
                    Debug.Assert(false, msg);
                    Dispatcher.BeginInvoke((Action)(() => StaticMainWindow.Window.ShowErrorScreen()));
                };
                GlobalVariables.StoreWorker.ForceAddRequest(listArtistRequest);
            };
            artistCountRequest.ProcessError += (reply, msg) =>
            {
                Debug.Assert(false, msg);
                Dispatcher.BeginInvoke((Action)(() => StaticMainWindow.Window.ShowErrorScreen()));
            };
            GlobalVariables.StoreWorker.ForceAddRequest(artistCountRequest);
        }
        private void artistItem_ArtistItemClicked(ulong artistID)
        {
            var detailScreen = new ArtistDetailScreen();
            detailScreen.SetInfo(artistID);
            StaticMainWindow.Window.ChangeScreen(detailScreen);
        }

        private void GetFeaturedList()
        {
            var featuredListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.FEATURE, 0, 15);
            featuredListRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var info in reply.list_motion.motion_short_info)
                    {
                        var motionItem = new MotionHorizontalItem();
                        motionItem.SetInfo(info);
                        motionItem.MotionClicked += motionItem_MotionClicked;
                        ViewModel.FeaturedMotionsList.Add(motionItem);
                        DownloadImage(info.motion_id, info.icon_url, motionItem);
                    }
                    StaticMainWindow.Window.ShowContentScreen();
                });
            featuredListRequest.ProcessError += (reply, msg) =>
            {
                Debug.Assert(false, msg);
                Dispatcher.BeginInvoke((Action)(() => StaticMainWindow.Window.ShowErrorScreen()));
            };
            GlobalVariables.StoreWorker.ForceAddRequest(featuredListRequest);
        }

        private void motionItem_MotionClicked(ulong motionID)
        {
            var detailScreen = new MotionDetailScreen();
            detailScreen.SetInfo(motionID);
            StaticMainWindow.Window.ChangeScreen(detailScreen);
        }

        private void DownloadImage(ulong id, string url, object item)
        {
            var imageDownload = new ImageDownload(url);
            if (item is MotionHorizontalItem)
            {
                imageDownload.DownloadCompleted += ((MotionHorizontalItem)item).SetImage;
            }
            if (item is ArtistItemVertical)
            {
                imageDownload.DownloadCompleted += ((ArtistItemVertical)item).SetImage;
            }
            GlobalVariables.ImageDownloadWorker.AddDownload(imageDownload);
        }

        private void ArtistLisBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MainScrollViewer_DoMouseWheel(e);
        }
        private void MainScrollViewer_DoMouseWheel(MouseWheelEventArgs e)
        {
            MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
        }
    }
}
