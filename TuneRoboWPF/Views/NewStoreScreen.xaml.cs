using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
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

            InitCover();

            DataContext = new NewStoreScreenViewModel();
            ViewModel = (NewStoreScreenViewModel)DataContext;
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
            var featuredListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.FEATURE, 0, 16);
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

        #region Cover
        private void InitCover()
        {
            Thumnail1.Source = new BitmapImage(new Uri(new FileInfo("1.png").FullName, UriKind.RelativeOrAbsolute));
            Thumnail2.Source = new BitmapImage(new Uri(new FileInfo("2.jpg").FullName, UriKind.RelativeOrAbsolute));
            Thumnail3.Source = new BitmapImage(new Uri(new FileInfo("3.jpg").FullName, UriKind.RelativeOrAbsolute));
            Thumnail4.Source = new BitmapImage(new Uri(new FileInfo("4.jpg").FullName, UriKind.RelativeOrAbsolute));

            thumnailList.Add(Thumnail1);
            thumnailList.Add(Thumnail2);
            thumnailList.Add(Thumnail3);
            thumnailList.Add(Thumnail4);


            firstMargin = thumnailList[0].Margin;

            Cover1.Source = Thumnail2.Source;
            Cover1.Opacity = 0;
            Cover2.Source = Thumnail3.Source;

            Panel.SetZIndex(Cover1, 1);
            Panel.SetZIndex(Cover2, 2);

            //Console.WriteLine(Cover1.Opacity);
            //Console.WriteLine(Cover2.Opacity);

            var CoverWorker = new BackgroundWorker();
            CoverWorker.DoWork += CoverWorker_DoWork;
            CoverWorker.RunWorkerAsync();

            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private int CoverNumber = 0;
        private object CoverNumberLock = new object();
        private bool CanFlow = true;
        private object CanFlowLock = new object();

        private void CoverWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {

                lock (CoverNumberLock)
                {
                    while (CoverNumber == 0)
                    {
                        Monitor.Wait(CoverNumberLock);    
                    }                    
                }

                lock (CanFlowLock)
                {
                    while (!CanFlow)
                    {
                        Monitor.Wait(CanFlowLock);
                    }
                }
                Dispatcher.BeginInvoke((Action) CoverFlowing);
                CoverNumber--;                
            }
            //throw new NotImplementedException();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lock(CoverNumberLock)
            {
                CoverNumber++;
                Monitor.Pulse(CoverNumberLock);
            }
            //CoverFlowing();
        }
        List<Image> thumnailList = new List<Image>();

        private Thickness firstMargin;

        private DispatcherTimer timer = new DispatcherTimer();

        private void TranslationAnimate(int index, double from, double to)
        {
            var da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            var tt = new TranslateTransform();
            var image = thumnailList[index];
            image.RenderTransform = tt;
            da.Completed += (sender, e) =>
            {
                tt.BeginAnimation(TranslateTransform.YProperty, null);
                if (index == nextReserveImageIndex)
                {
                    image.Margin = firstMargin;
                }
                else image.Margin = new Thickness(0, image.Margin.Top + thumnailTranslationStep, 0, 0);
                if (index == thumnailList.Count - 1)
                {
                    if (nextReserveImageIndex == 0) nextReserveImageIndex = thumnailList.Count - 1;
                    else nextReserveImageIndex--;
                }
                
            };
            tt.BeginAnimation(TranslateTransform.YProperty, da);
        }

        private void OpacityAnimate(Image image, double from, double to)
        {
            var da = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            da.Completed += (sender, e) =>
            {
                image.BeginAnimation(OpacityProperty, null);
                if (Cover1CanVisible)
                {
                    //Bring cover1 to front   
                    var tmpZIndex = Panel.GetZIndex(Cover1);
                    Panel.SetZIndex(Cover1, Panel.GetZIndex(Cover2));
                    Panel.SetZIndex(Cover2, tmpZIndex);
                    //Change to next image
                    Cover2.Source = thumnailList[nextImageIndex].Source;

                    Cover1CanVisible = false;
                }
                else
                {
                    //Bring cover 2 to front
                    var tmpZIndex = Panel.GetZIndex(Cover2);
                    Panel.SetZIndex(Cover2, Panel.GetZIndex(Cover1));
                    Panel.SetZIndex(Cover1, tmpZIndex);
                    //Change to next image
                    Cover1.Source = thumnailList[nextImageIndex].Source;
                    Cover1CanVisible = true;
                }
                //Console.WriteLine(Panel.GetZIndex(Cover1));
                //Console.WriteLine(Panel.GetZIndex(Cover2));
                //Console.WriteLine(nextImageIndex);
                //nextImageIndex--;
                //if (nextImageIndex < 0) nextImageIndex = thumnailList.Count - 1;
                nextImageIndex = nextReserveImageIndex + 1;
                if (nextImageIndex == thumnailList.Count) nextImageIndex = 0;
                //Console.WriteLine(nextImageIndex);
                lock (CanFlowLock)
                {
                    CanFlow = true;
                    Monitor.Pulse(CanFlowLock);
                }
            };
            image.BeginAnimation(OpacityProperty, da);
        }


        private int nextReserveImageIndex = 3;
        private int nextImageIndex = 0;
        private int thumnailTranslationStep = 111;
        private bool Cover1CanVisible = true;

        private void CoverFlowing()
        {
            lock(CanFlowLock)
            {
                CanFlow = false;
                Monitor.Pulse(CanFlowLock);
            }
            
            for (int i = 0; i < thumnailList.Count; i++)
            {
                TranslationAnimate(i, 0, thumnailTranslationStep);
            }
            if (Cover1CanVisible)
            {
                Cover1.Opacity = 1;
                OpacityAnimate(Cover2, 1, 0);
            }
            else
            {
                Cover2.Opacity = 1;
                OpacityAnimate(Cover1, 1, 0);
            }
            
        }
        private void CoverDownButton_Click(object sender, RoutedEventArgs e)
        {
            lock (CoverNumberLock)
            {
                CoverNumber++;
                Monitor.Pulse(CoverNumberLock);
            }
            //CoverFlowing();
        }
        #endregion


    }
}
