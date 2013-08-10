using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
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

            GetRecommendedList();
            GetHotList();
            GetArtistList();
            GetFeaturedList();
        }

        private void GetRecommendedList()
        {
            var recommemendListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.RECOMMENDED, 0, 3);
            recommemendListRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    List<MotionShortInfo> recommendList = reply.list_motion.motion_short_info;
                    if (recommendList.Count > 0)
                    {
                        DownloadCover(recommendList[0].cover, 2);
                        CoverIDList.Add(recommendList[0].motion_id);
                    }
                    if (recommendList.Count > 1)
                    {
                        DownloadCover(recommendList[1].cover, 1);
                        CoverIDList.Add(recommendList[1].motion_id);
                    }
                    if (recommendList.Count > 2)
                    {
                        DownloadCover(recommendList[2].cover, 3);
                        CoverIDList.Add(recommendList[2].motion_id);
                    }
                    if (recommendList.Count > 3)
                    {
                        DownloadCover(recommendList[3].cover, 4);
                        CoverIDList.Add(recommendList[3].motion_id);
                    }
                });
            recommemendListRequest.ProcessError += (reply, msg) =>
            {
                Debug.Assert(false, msg);
                Dispatcher.BeginInvoke((Action)(() => StaticMainWindow.Window.ShowErrorScreen()));
            };
            GlobalVariables.StoreWorker.ForceAddRequest(recommemendListRequest);
        }


        private void DownloadCover(string url, int coverIndex)
        {
            var imageDownload = new ImageDownload(url);
            BitmapImage cacheImage = imageDownload.FindInCacheOrLocal();
            if (cacheImage!=null)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    switch (coverIndex)
                    {
                        case 1:
                            ViewModel.ThumbnailSource1 = cacheImage;
                            break;
                        case 2:
                            ViewModel.ThumbnailSource2 = cacheImage;
                            break;
                        case 3:
                            ViewModel.ThumbnailSource3 = cacheImage;
                            break;
                        case 4:
                            ViewModel.ThumbnailSource4 = cacheImage;
                            break;
                    }
                });
                return;
            }
            imageDownload.DownloadCompleted += image =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    switch (coverIndex)
                    {
                        case 1:
                            ViewModel.ThumbnailSource1 = image;
                            break;
                        case 2:
                            ViewModel.ThumbnailSource2 = image;
                            break;
                        case 3:
                            ViewModel.ThumbnailSource3 = image;
                            break;
                        case 4:
                            ViewModel.ThumbnailSource4 = image;
                            break;
                    }
                });

            GlobalVariables.ImageDownloadWorker.AddDownload(imageDownload);
        }



        private void GetHotList()
        {
            var hotListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.TOPRATED, 0, 15);
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
                    //hotListRequest.ProcessSuccessfully-=
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

            //featuredListRequest.SuccesfullyEvent += OnFeaturedListRequestOnProcessSuccessfully;

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
            BitmapImage cacheImage = imageDownload.FindInCacheOrLocal();
            
            if (item is MotionHorizontalItem)
            {
                if (cacheImage != null)
                {
                    ((MotionHorizontalItem) item).SetImage(cacheImage);
                    return;
                }
                imageDownload.DownloadCompleted += ((MotionHorizontalItem)item).SetImage;
            }
            if (item is ArtistItemVertical)
            {
                if (cacheImage != null)
                {
                    ((ArtistItemVertical)item).SetImage(cacheImage);
                    return;
                }
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

        private void SeeAllFeatured_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var screen = new SeeAllScreen();
            screen.SetCategory("Featured");
            StaticMainWindow.Window.ChangeScreen(screen);
        }

        private void SeeAllHot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var screen = new SeeAllScreen();
            screen.SetCategory("Hot");
            StaticMainWindow.Window.ChangeScreen(screen);
        }


        #region Cover
        private DispatcherTimer timer = new DispatcherTimer();


        private void InitCover()
        {
            //var thumnail1 = new BitmapImage(new Uri(new FileInfo("1.png").FullName, UriKind.RelativeOrAbsolute));
            //thumnail1.Freeze();
            //var thumnail2 = new BitmapImage(new Uri(new FileInfo("2.jpg").FullName, UriKind.RelativeOrAbsolute));
            //thumnail2.Freeze();
            //var thumnail3 = new BitmapImage(new Uri(new FileInfo("3.jpg").FullName, UriKind.RelativeOrAbsolute));
            //thumnail3.Freeze();
            //var thumnail4 = new BitmapImage(new Uri(new FileInfo("4.jpg").FullName, UriKind.RelativeOrAbsolute));
            //thumnail4.Freeze();


            thumbnailList.Add(Thumbnail1);
            thumbnailList.Add(Thumbnail2);
            thumbnailList.Add(Thumbnail3);
            thumbnailList.Add(Thumbnail4);


            firstMargin = thumbnailList[0].Margin;

            //Cover1.Source = Thumbnail2.Source;
            Cover1.Opacity = 0;
            //Cover2.Source = Thumbnail3.Source;

            Panel.SetZIndex(Cover1, 1);
            Panel.SetZIndex(Cover2, 2);

            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            CoverFlowing();
        }

        List<Image> thumbnailList = new List<Image>();
        List<ulong> CoverIDList = new List<ulong>();

        private Thickness firstMargin;

        private bool TranslationAnimateDone = true;
        private bool OpacityAnimateDone = true;


        private void TranslationAnimate(int index, double from, double to)
        {
            var da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = new Duration(TimeSpan.FromSeconds(1));
            var tt = new TranslateTransform();
            var image = thumbnailList[index];
            image.RenderTransform = tt;
            da.Completed += (sender, e) =>
            {
                tt.BeginAnimation(TranslateTransform.YProperty, null);
                if (index == nextReserveImageIndex)
                {
                    image.Margin = firstMargin;
                }
                else image.Margin = new Thickness(0, image.Margin.Top + thumnailTranslationStep, 0, 0);
                if (index == thumbnailList.Count - 1)
                {
                    if (nextReserveImageIndex == 0) nextReserveImageIndex = thumbnailList.Count - 1;
                    else nextReserveImageIndex--;
                }
                TranslationAnimateDone = true;
            };
            TranslationAnimateDone = false;
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
                    Cover2.Source = thumbnailList[nextImageIndex].Source;

                    Cover1CanVisible = false;
                }
                else
                {
                    //Bring cover 2 to front
                    var tmpZIndex = Panel.GetZIndex(Cover2);
                    Panel.SetZIndex(Cover2, Panel.GetZIndex(Cover1));
                    Panel.SetZIndex(Cover1, tmpZIndex);
                    //Change to next image
                    Cover1.Source = thumbnailList[nextImageIndex].Source;
                    Cover1CanVisible = true;
                }
                currentImageIndex--;
                if (currentImageIndex < 0) currentImageIndex = CoverIDList.Count - 1;
                
                nextImageIndex = nextReserveImageIndex + 1;
                if (nextImageIndex == thumbnailList.Count) nextImageIndex = 0;

                OpacityAnimateDone = true;
            };
            OpacityAnimateDone = false;
            image.BeginAnimation(OpacityProperty, da);
        }


        private int nextReserveImageIndex = 3;
        private int nextImageIndex = 0;
        private int currentImageIndex = 2;
        private int thumnailTranslationStep = 111;
        private bool Cover1CanVisible = true;

        private void CoverFlowing()
        {
            if (!OpacityAnimateDone || !TranslationAnimateDone) return;


            for (int i = 0; i < thumbnailList.Count; i++)
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
            CoverFlowing();
        }

        private void CoverBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(CoverBorder);
            if (p.X < Cover1.Width)
            {
                motionItem_MotionClicked(CoverIDList[currentImageIndex]);
            }
        }

        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

    }
}
