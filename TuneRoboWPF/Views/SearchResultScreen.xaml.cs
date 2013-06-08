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


        public void SetQuery(string query)
        {
            Query = query;
            ViewModel.SearchQuery = query;
            UpdateSearchList(0, 19);
        }
        public void UpdateSearchList(uint start, uint end)
        {
            var searchRequest = new SearchMotionStoreRequest(Query, start, end);
            searchRequest.ProcessSuccessfully += (reply) =>
              Dispatcher.BeginInvoke((Action)delegate
                {
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
                });
            searchRequest.ProcessError += (reply, msg) => Debug.Fail(msg, reply.type.ToString());
            GlobalVariables.StoreWorker.ForceAddRequest(searchRequest);
        }

        private void motionFull_MotionClicked(ulong motionID)
        {
            var motionScreen = new MotionDetailScreen(motionID);
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
            if (IsLoaded)
                {

                    if (MainScrollViewer.VerticalOffset + 10 > MainScrollViewer.ScrollableHeight)
                    {
                        UpdateSearchList(numberSearchItem, numberSearchItem + 19);
                    } 
                }
        }
    }
}