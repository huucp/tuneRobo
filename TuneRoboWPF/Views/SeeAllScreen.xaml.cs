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
using TuneRoboWPF.StoreService;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for SeeAllScreen.xaml
    /// </summary>
    public partial class SeeAllScreen : UserControl
    {
        private SeeAllScreenViewModel ViewModel = new SeeAllScreenViewModel();
        public SeeAllScreen()
        {
            InitializeComponent();

            DataContext = new SeeAllScreenViewModel();
            ViewModel = (SeeAllScreenViewModel)DataContext;
        }

        public void SetCategory(string category, bool newScreen = true)
        {
            ViewModel.Category = category;
            if (!newScreen) return;// Get from navigation system

            var screen = new Screen(Screen.ScreenType.SeeAll, category);
            GlobalVariables.Navigation.AddScreen(screen);
        }

        private bool FirstLoad = true;
        private uint TotalNumberMotion = 0;

        private void GetMotionList(uint start, uint end)
        {
            var request = new ListCategoryMotionStoreRequest(ViewModel.CategoryType, start, end);
            request.ProcessSuccessfully += (reply) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (reply.list_motion.motion_short_info.Count == 0 && FirstLoad)
                    {
                        ViewModel.NoResultVisibility = true;
                    }
                    foreach (var info in reply.list_motion.motion_short_info)
                    {
                        var motion = new MotionItemVertical();
                        motion.SetInfo(info);
                        motion.MotionClicked += motion_MotionClicked;
                        UpdateMotionCover(info.icon_url, motion);
                        ViewModel.CategoryList.Add(motion);
                        TotalNumberMotion++;
                    }
                    if (FirstLoad)
                    {
                        StaticMainWindow.Window.ShowContentScreen();
                        FirstLoad = false;
                    }
                });
                
            };
            request.ProcessError += (reply, msg) =>
            {
                if (reply == null) Debug.Fail("reply is null");
                else
                {
                    Debug.Fail(msg, reply.type.ToString());
                    if (FirstLoad) Dispatcher.BeginInvoke((Action)(() => StaticMainWindow.Window.ShowErrorScreen()));
                }
            };
            GlobalVariables.StoreWorker.ForceAddRequest(request);
        }

        private void motion_MotionClicked(ulong motionID)
        {
            var motionScreen = new MotionDetailScreen();
            motionScreen.SetInfo(motionID);
            StaticMainWindow.Window.ChangeScreen(motionScreen);
        }
        private void UpdateMotionCover(string url, MotionItemVertical item)
        {
            var coverImage = new ImageDownload(url);
            BitmapImage cacheImage = coverImage.FindInCacheOrLocal();
            if (cacheImage!=null)
            {
                Dispatcher.BeginInvoke((Action)(() => item.SetImage(cacheImage)));
                return;
            }
            coverImage.DownloadCompleted += (image) =>
                Dispatcher.BeginInvoke((Action)(() => item.SetImage(image)));
            GlobalVariables.ImageDownloadWorker.AddDownload(coverImage);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalVariables.ImageDownloadWorker.ClearAll();
            StaticMainWindow.Window.ShowLoadingScreen();
            GetMotionList(0, 39);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (IsLoaded && !FirstLoad)
            {
                if (MainScrollViewer.VerticalOffset + 10 > MainScrollViewer.ScrollableHeight)
                {
                    GetMotionList(TotalNumberMotion, TotalNumberMotion + 19);
                }
            }
        }
    }
}
