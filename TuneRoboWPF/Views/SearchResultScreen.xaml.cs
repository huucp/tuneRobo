using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for SearchResultScreen.xaml
    /// </summary>
    public partial class SearchResultScreen : UserControl
    {
        private SearchResultScreenViewModel ViewModel { get; set; }
        private string Query { get; set; }
        private uint numberSearchItem = 0;
        public SearchResultScreen()
        {
            InitializeComponent();

            DataContext = new SearchResultScreenViewModel();
            ViewModel = (SearchResultScreenViewModel)DataContext;

        }


        public void SetQuery(string query, bool newScreen = true)
        {
            Query = query;
            ViewModel.SearchQuery = query;
            if (!newScreen) return;// Get from navigation system

            var screen = new Screen(Screen.ScreenType.Search, query);
            GlobalVariables.Navigation.AddScreen(screen);            
        }

        private bool FirstLoad = true;
        public void UpdateSearchList(uint start, uint end)
        {
            var searchRequest = new SearchMotionStoreRequest(Query, start, end);
            searchRequest.ProcessSuccessfully += (reply) =>
              Dispatcher.BeginInvoke((Action)delegate
                {
                    if (reply.search_motion.motion_short_info.Count==0)
                    {
                        ViewModel.NoResultVisibility = true;
                        //return;
                    }
                    foreach (var info in reply.search_motion.motion_short_info)
                    {
                        var motionFull = new MotionItemVertical();
                        motionFull.SetInfo(info);
                        motionFull.MotionClicked += motionFull_MotionClicked;
                        UpdateSearchMotionCover(info.icon_url, motionFull);
                        ViewModel.SearchList.Add(motionFull);
                        numberSearchItem++;
                        //for (int i = 0; i < 10; i++)
                        //{
                        //    var motionFull = new MotionItemVertical();
                        //    motionFull.SetInfo(info);
                        //    motionFull.MotionClicked += motionFull_MotionClicked;
                        //    UpdateSearchMotionCover(info.icon_url, motionFull);
                        //    ViewModel.SearchList.Add(motionFull);

                        //}
                        
                    }
                    if (FirstLoad)
                    {
                        StaticMainWindow.Window.ShowContentScreen();
                        FirstLoad = false;
                    }
                });
            searchRequest.ProcessError += (reply, msg) =>
            {
                Debug.Fail(msg, reply.type.ToString());
                Dispatcher.BeginInvoke((Action)(() => StaticMainWindow.Window.ShowErrorScreen()));
            };
            GlobalVariables.StoreWorker.ForceAddRequest(searchRequest);
        }

        private void motionFull_MotionClicked(ulong motionID)
        {
            var motionScreen = new MotionDetailScreen();
            motionScreen.SetInfo(motionID);
            StaticMainWindow.Window.ChangeScreen(motionScreen);
        }

        private void UpdateSearchMotionCover(string url, MotionItemVertical item)
        {
            var coverImage = new ImageDownload(url);
            coverImage.DownloadCompleted += (image) =>
                Dispatcher.BeginInvoke((Action)(() => item.SetImage(image)));
            GlobalVariables.ImageDownloadWorker.AddDownload(coverImage);
        }

        private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (IsLoaded )
                {

                    if (MainScrollViewer.VerticalOffset + 10 > MainScrollViewer.ScrollableHeight)
                    {
                        UpdateSearchList(numberSearchItem, numberSearchItem + 19);
                    } 
                }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            StaticMainWindow.Window.ShowLoadingScreen();
            UpdateSearchList(0,19);
        }
    }
}