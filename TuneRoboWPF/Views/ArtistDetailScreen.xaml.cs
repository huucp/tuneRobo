using System;
using System.Diagnostics;
using System.Windows.Controls;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using artist;
using TuneRoboWPF.StoreService.SimpleRequest;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for ArtistDetailScreen.xaml
    /// </summary>
    public partial class ArtistDetailScreen : UserControl
    {
        private ArtistDetailScreenViewModel ViewModel = new ArtistDetailScreenViewModel();
        private ulong ArtistID { get; set; }
        private ArtistInfoReply Info { get; set; }
        public ArtistDetailScreen()
        {
            InitializeComponent();

            DataContext = new ArtistDetailScreenViewModel();
            ViewModel = (ArtistDetailScreenViewModel)DataContext;
        }

        public void SetInfo(ulong id)
        {
            ArtistID = id;
            GetArtistInfo();
            GetMotionOfArtist();
        }

        private void GetArtistInfo()
        {
            var artistInfoRequest = new GetFullArtistInfoStoreRequest(ArtistID);
            artistInfoRequest.ProcessSuccessfully += (reply) =>
            {
                Info = reply.artist_info;
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.ArtistName = Info.artist_name;
                    ViewModel.RatingValue = Info.avg_rating/10.0;
                    ViewModel.Biography = Info.description;
                });
                UpdateArtistAvatar(Info.avatar_url);
            };
            artistInfoRequest.ProcessError += (reply, msg) => Debug.Assert(false, msg);
            GlobalVariables.StoreWorker.ForceAddJob(artistInfoRequest);
        }
        private void UpdateArtistAvatar(string url)
        {
            var avatarRequest = new ImageDownload(url);
            avatarRequest.DownloadCompleted += (image) =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.ArtistAvatar = image;
                });
            GlobalVariables.ImageDownloadWorker.AddDownload(avatarRequest);
        }

        private void DownloadMotionImage(string url, MotionItemVertical motion)
        {
            var download = new ImageDownload(url);
            download.DownloadCompleted += (image) => Dispatcher.BeginInvoke((Action) (() => motion.SetImage(image)));
            GlobalVariables.ImageDownloadWorker.AddDownload(download);
        }

        private void GetMotionOfArtist()
        {
            var countRequest = new GetNumberMotionOfArtistStoreRequest(ArtistID);
            countRequest.ProcessSuccessfully += (countReply) =>
            {
                Dispatcher.BeginInvoke((Action)delegate { ViewModel.NumberMotion = countReply.number_motion_artist.number_motion; });
                var motionRequest = new GetMotionOfArtistStoreRequest(ArtistID, 0, countReply.number_motion_artist.number_motion);
                motionRequest.ProcessSuccessfully += (motionReply) =>
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        foreach (var motionInfo in motionReply.artist_motion.motion_short_info)
                        {
                            var motionItemVertical = new MotionItemVertical();
                            motionItemVertical.SetInfo(motionInfo);
                            ViewModel.ArtistMotionsList.Add(motionItemVertical);
                            DownloadMotionImage(motionInfo.icon_url,motionItemVertical);
                            //for (int i = 0; i < 20; i++)
                            //{
                            //    var motionItemVertical = new MotionItemVertical();
                            //    motionItemVertical.SetInfo(motionInfo);
                            //    ViewModel.ArtistMotionsList.Add(motionItemVertical);
                            //}
                        }
                    });
                motionRequest.ProcessError +=
                    (motionReply, msg) => Debug.Assert(false, msg + motionReply.type.ToString());
                GlobalVariables.StoreWorker.ForceAddJob(motionRequest);
            };
            countRequest.ProcessError += (countReply, msg) => Debug.Assert(false, msg + countReply.type.ToString());
            GlobalVariables.StoreWorker.ForceAddJob(countRequest);
        }
    }
}
