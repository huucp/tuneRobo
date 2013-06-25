using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TuneRoboWPF.Models;
using TuneRoboWPF.StoreService.BigRequest;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.Windows;
using comm;
using motion;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for MotionDetailScreen.xaml
    /// </summary>
    public partial class MotionDetailScreen : UserControl
    {
        private MotionDetailScreenViewModel ViewModel { get; set; }
        private ulong MotionID { get; set; }
        private motion.MotionInfo Info { get; set; }
        private uint numberOfComment = 0;
        private uint numberOfRelatedMotions = 0;
        public MotionDetailScreen()
        {
            InitializeComponent();

            DataContext = new MotionDetailScreenViewModel();
            ViewModel = (MotionDetailScreenViewModel)DataContext;

        }

        public void SetInfo(ulong motionID, bool newScreen = true)
        {
            MotionID = motionID;
            if (!newScreen) return; // Get from navigation system

            var screen = new Screen(Screen.ScreenType.MotionDetail, MotionID);
            GlobalVariables.Navigation.AddScreen(screen);

        }

        private void GetMotionInfo()
        {
            var infoRequest = new GetMotionFullInfoStoreRequest(MotionID);
            infoRequest.ProcessSuccessfully += delegate(Reply reply)
                                                   {
                                                       Info = reply.motion_info.info;
                                                       UpdateContent(reply.motion_info.info);
                                                   };
            infoRequest.ProcessError += (reply, msg) =>
            {
                Debug.Assert(false, msg);
                Dispatcher.BeginInvoke((Action)(() => StaticMainWindow.Window.ShowErrorScreen()));
            };
            GlobalVariables.StoreWorker.AddRequest(infoRequest);
        }

        private void UpdateContent(motion.MotionInfo info)
        {
            if (Dispatcher.CheckAccess())
            {
                UpdateContentValue(info);
                UpdateDownloadButtonContent();
                StaticMainWindow.Window.ShowContentScreen();
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(delegate
                                                    {
                                                        UpdateContentValue(info);
                                                        UpdateDownloadButtonContent();
                                                        StaticMainWindow.Window.ShowContentScreen();
                                                    }));
            }


            UpdateNumberRating();
            UpdateArtwork(info.icon_url);
            UpdateScreenshots(info.screenshoot_ulrs);

            UpdateComment(0, 20);
            UpdateRelatedMotions(0, 20);
        }

        private void UpdateComment(uint start, uint end)
        {
            var commentRequest = new GetRatingMotionInfoStoreRequest(MotionID, start, end);
            commentRequest.ProcessSuccessfully += (reply) =>
            {
                foreach (var comment in reply.rating_info.ratingInfoMotion)
                {
                    RatingInfo comment1 = comment;
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        var item = new CommentItem(comment1);
                        ReviewStackPanel.Children.Add(item);
                        numberOfComment++;
                        //for (int i = 0; i < 20; i++)
                        //{
                        //    var item = new CommentItem(comment1);
                        //    ReviewStackPanel.Children.Add(item);
                        //    numberOfComment++;
                        //    Console.WriteLine(numberOfComment);
                        //}
                    });
                }
            };
            commentRequest.ProcessError += (reply, msg) => Debug.Assert(false, msg);
            GlobalVariables.StoreWorker.ForceAddRequest(commentRequest);
        }

        private void UpdateContentValue(motion.MotionInfo info)
        {
            ViewModel.MotionTitle = info.title;
            ViewModel.RatingValue = info.rating / 10.0;
            ViewModel.ArtistName = info.artist_name;
            ViewModel.MotionDescription = info.description;
            ViewModel.MoreByTextBlock = string.Format("More by {0}", info.artist_name);
        }

        private void UpdateArtwork(string url)
        {
            var downloadImageRequest = new ImageDownload(url);
            downloadImageRequest.DownloadCompleted += (image) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.CoverImage = image;
                });
            downloadImageRequest.DownloadFailed += (s, msg) => Debug.Fail(msg);
            GlobalVariables.ImageDownloadWorker.AddDownload(downloadImageRequest);
        }

        private string GetYoutubeVideoID(string url)
        {
            string id = Regex.Match(url,
                        @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/ ]{11})").
                Groups[1].Value;
            return id;
        }

        private void LoadYoutubeThumnail(string url)
        {
            string videoID = GetYoutubeVideoID(url);
            string thumnailUrl = "http://img.youtube.com/vi/" + videoID + "/0.jpg";
            var thumnailDownload = new ImageDownload(thumnailUrl);
            thumnailDownload.DownloadCompleted += (image) =>
                Dispatcher.BeginInvoke((Action)(delegate
                {
                    var youtubeImage = new ScreenshotImage();
                    youtubeImage.ViewModel.ScreenshotSource = image;
                    youtubeImage.ViewModel.IsYoutubeThumbnail = true;
                    ViewModel.ScreenshotsList[0] = youtubeImage;
                }));
            thumnailDownload.DownloadFailed += (s, msg) => Debug.Fail(msg);
            GlobalVariables.ImageDownloadWorker.AddDownload(thumnailDownload);
        }

        private void UpdateScreenshots(List<string> urls)
        {
            //var youtubeImage = new MotionDetailScreenModel.ScreenshotImage { ImageSource = (BitmapImage)FindResource("YoutubeImage") };
            Dispatcher.BeginInvoke((Action)(delegate
            {
                var defaultYoutubeImage = new ScreenshotImage();
                defaultYoutubeImage.ViewModel.ScreenshotSource = (BitmapImage)FindResource("YoutubeImage");
                defaultYoutubeImage.ViewModel.IsYoutubeThumbnail = true;
                ViewModel.ScreenshotsList.Add(defaultYoutubeImage);
            }));
            LoadYoutubeThumnail(Info.video_url);
            foreach (string t in urls)
            {
                var downloadImageRequest = new ImageDownload(t);
                downloadImageRequest.DownloadCompleted += (imageSource) =>
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            var image = new ScreenshotImage();
                            image.ViewModel.ScreenshotSource = imageSource;
                            image.ViewModel.IsYoutubeThumbnail = false;
                            ViewModel.ScreenshotsList.Add(image);
                        });
                downloadImageRequest.DownloadFailed += (s, msg) => Debug.Fail(msg);
                GlobalVariables.ImageDownloadWorker.AddDownload(downloadImageRequest);
            }
        }

        private void UpdateRelatedMotions(uint start, uint end)
        {
            var relatedRequest = new GetMotionOfArtistStoreRequest(Info.artist_id, start, end);
            relatedRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var motionInfo in reply.artist_motion.motion_short_info)
                    {
                        if (MotionID == motionInfo.motion_id) continue;
                        var verticalMotionItem = new MotionItemVertical();
                        verticalMotionItem.SetInfo(motionInfo);
                        verticalMotionItem.MotionClicked += RelatedMotions_MotionClicked;
                        ViewModel.RelatedMotionsList.Add(verticalMotionItem);
                        UpdateRelatedMotionCover(motionInfo.icon_url, verticalMotionItem);
                    }
                });
            relatedRequest.ProcessError += (reply, msg) => Debug.Assert(false, msg);
            GlobalVariables.StoreWorker.ForceAddRequest(relatedRequest);
        }

        private void RelatedMotions_MotionClicked(ulong motionid)
        {
            var motionDetailScreen = new MotionDetailScreen();
            motionDetailScreen.SetInfo(motionid);
            StaticMainWindow.Window.ChangeScreen(motionDetailScreen);
        }

        private void UpdateRelatedMotionCover(string url, MotionItemVertical item)
        {
            var coverImage = new ImageDownload(url);
            coverImage.DownloadCompleted += (image) =>
                Dispatcher.BeginInvoke((Action)(() => item.SetImage(image)));
            coverImage.DownloadFailed += (s, msg) => Debug.Fail(msg);
            GlobalVariables.ImageDownloadWorker.AddDownload(coverImage);
        }

        private void UpdateDownloadButtonContent()
        {
            if (!IsOnLocal())
            {
                ViewModel.DownloadButtonContent = (string)FindResource("FreeText");
            }
            else
            {
                var info = GlobalFunction.GetLocalMotionInfo(MotionID);
                if (info.VersionCode < Info.version_id)
                {
                    ViewModel.DownloadButtonContent = (string)FindResource("UpdateText");
                }
                else
                {
                    ViewModel.DownloadButtonContent = (string)FindResource("InstalledText");
                }
            }
        }

        private void UpdateNumberRating()
        {
            var numberRatingRequest = new GetNumberRatingInfoStoreRquest(MotionID);
            numberRatingRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.NumberRating = string.Format("({0})", reply.number_rating_info.number);
                });
            numberRatingRequest.ProcessError += (reply, msg) => Debug.Assert(false, msg + reply.type);
            GlobalVariables.StoreWorker.ForceAddRequest(numberRatingRequest);
        }

        private bool IsOnLocal()
        {
            return (MotionFileOnLocal() && MusicFileOnLocal());
        }

        private bool MotionFileOnLocal()
        {
            string motionPath = GlobalFunction.GetLocalMotionPath(MotionID);
            return File.Exists(motionPath);
        }
        private bool MusicFileOnLocal()
        {
            string musicPath =
                Path.Combine(GlobalVariables.LOCAL_DIR + GlobalVariables.FOLDER_ROOT + GlobalVariables.FOLDER_PLAYLIST,
                             MotionID.ToString() + ".mp3");
            return File.Exists(musicPath);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVariables.UserOnline)
            {
                var signinWindow = new LoginWindow();
                if (signinWindow.ShowDialog(StaticMainWindow.Window) == false) return;
            }
            StaticMainWindow.Window.UpdateLoginSuccessfully();
            var request = new DownloadMotionStoreRequest(MotionID);
            var transferWindow = new TransferWindow(request, Info.title);
            if (transferWindow.ShowDialog(StaticMainWindow.Window) == true)
            {
                ViewModel.DownloadButtonContent = "Installed";
            }
        }

        private void ScreenshotsListbox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            MainScrollViewer.ScrollToVerticalOffset(MainScrollViewer.VerticalOffset - e.Delta);
        }

        private void ScreenshotsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScreenshotsListbox.SelectedIndex == 0)
            {
                Process.Start(Info.video_url);
            }
        }

        private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (IsLoaded)
            {
                if (MainScrollViewer.VerticalOffset + 10 > MainScrollViewer.ScrollableHeight)
                {
                    if (ContentTabControl.SelectedIndex == 1)
                    {
                        UpdateComment(numberOfComment, numberOfComment + 20);
                    }
                }
            }
        }

        private void CheckUserOwnMotionRelation()
        {
            var relationRequest = new UserOwnMotionStoreRequest(MotionID);
            relationRequest.ProcessSuccessfully += (reply) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    switch (reply.user_own_motion.rel)
                    {
                        case UserOwnMotionReply.Rel.OWNED:
                            var ratingWindow = new RatingWindow();
                            ratingWindow.SetInfo(Info.motion_id, Info.version_id);
                            if (ratingWindow.ShowDialog(StaticMainWindow.Window) == true)
                            {

                            }
                            break;
                        case UserOwnMotionReply.Rel.NOT_OWNED:
                            MessageBox.Show("You must own this motion to review");
                            break;
                    }
                });
            relationRequest.ProcessError += (reply, msg) => Debug.Fail(reply.type.ToString(), msg);
            GlobalVariables.StoreWorker.AddRequest(relationRequest);
        }

        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVariables.UserOnline)
            {
                var loginWindow = new LoginWindow();
                if (loginWindow.ShowDialog(StaticMainWindow.Window) == false) return;
                StaticMainWindow.Window.UpdateLoginSuccessfully();
            }
            CheckUserOwnMotionRelation();
        }

        private void ArtistTextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var artistDetailScreen = new ArtistDetailScreen();
            artistDetailScreen.SetInfo(Info.artist_id);
            StaticMainWindow.Window.ChangeScreen(artistDetailScreen);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            StaticMainWindow.Window.ShowLoadingScreen();
            GetMotionInfo();
        }
    }
}
