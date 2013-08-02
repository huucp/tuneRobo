using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Windows;

namespace TuneRoboWPF.Views
{
	/// <summary>
	/// Interaction logic for UserDetailScreen.xaml
	/// </summary>
	public partial class UserDetailScreen : UserControl
	{
        private UserDetailScreenViewModel ViewModel = new UserDetailScreenViewModel();
		public UserDetailScreen()
		{
			this.InitializeComponent();

		    DataContext = new UserDetailScreenViewModel();
		    ViewModel = (UserDetailScreenViewModel) DataContext;

		}        

        private void DownloadAvatar()
        {
            var imageDownload = new ImageDownload(GlobalVariables.CurrentUser.AvatarURL);
            imageDownload.DownloadCompleted += (image) => 
                Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.Avatar = image;
                });
            imageDownload.DownloadFailed += (s, msg) =>
                                                {
                                                    Dispatcher.BeginInvoke((Action)delegate
                {
                    ViewModel.Avatar = null;
                });
                                                    Debug.Fail(msg);
                                                };
            GlobalVariables.ImageDownloadWorker.AddDownload(imageDownload);
        }

        private void AvatarEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateProfileCall();
        }

        private void UpdateUserProfile()
        {
            var profileRequest = new GetUserInfoStoreRequest(GlobalVariables.CurrentUser.UserID);
            profileRequest.ProcessSuccessfully += reply =>
            {
                GlobalVariables.CurrentUser = new UserProfile(reply.profile);
                Dispatcher.BeginInvoke((Action)delegate
                {                    
                    ViewModel.Username = reply.profile.display_name;
                    GlobalVariables.CurrentUser.DisplayName = reply.profile.display_name;
                    StaticMainWindow.Window.navigationBar.ViewModel.Username = reply.profile.display_name;
                    DownloadAvatar();
                });
            };
            profileRequest.ProcessError += (reply, msg) => Debug.Fail(reply.type.ToString(), msg);
            GlobalVariables.StoreWorker.ForceAddRequest(profileRequest);
        }

        private void UpdateProfileCall()
        {
            var window = new UpdateProfileWindow();
            if(window.ShowDialog(StaticMainWindow.Window) == true)
            {
                UpdateUserProfile();
            }
        }
        private void EditUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateProfileCall();
        }

	    private void UserDetailScreen_Loaded(object sender, RoutedEventArgs e)
	    {
            GlobalVariables.ImageDownloadWorker.ClearAll();
            StaticMainWindow.Window.ShowLoadingScreen();
            ViewModel.Username = GlobalVariables.CurrentUser.DisplayName;
            DownloadAvatar();
	        GetPurchasedList(0,19);
	    }

	    private int numberMotions = 0;
	    private void GetPurchasedList(uint start, uint end)
	    {
	        var purchaseRequest = new GetMotionDownloadByUserStoreRequest(start, end);
	        purchaseRequest.ProcessSuccessfully += reply =>
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var motionInfo in reply.user_motion.motion_short_info)
                    {
                        numberMotions++;
                        var motion = new MotionItemVertical();
                        motion.SetInfo(motionInfo);
                        ViewModel.PurchasedMotionList.Add(motion);
                        motion.MotionClicked += PurchasedMotion_MotionClicked;
                        DownloadMotionImage(motionInfo.icon_url, motion);
                    }
                    StaticMainWindow.Window.ShowContentScreen();
                });
	        purchaseRequest.ProcessError += (reply, msg) =>
            {
                if(reply==null) Debug.Fail("reply is null");
                else Debug.Fail(reply.type.ToString(), msg);
                Dispatcher.BeginInvoke((Action)(() => StaticMainWindow.Window.ShowErrorScreen()));
            };                               
            GlobalVariables.StoreWorker.ForceAddRequest(purchaseRequest);
	    }

        private void PurchasedMotion_MotionClicked(ulong motionID)
        {
            var detailScreen = new MotionDetailScreen();
            detailScreen.SetInfo(motionID);
            StaticMainWindow.Window.ChangeScreen(detailScreen);
        }

        private void DownloadMotionImage(string url, MotionItemVertical motion)
        {
            var download = new ImageDownload(url);
            download.DownloadCompleted += (image) => Dispatcher.BeginInvoke((Action)(() => motion.SetImage(image)));
            download.DownloadFailed += (s, msg) =>
            {
                Dispatcher.BeginInvoke((Action)(() => motion.SetImage(null)));
                Debug.Fail(msg);
            };
            GlobalVariables.ImageDownloadWorker.AddDownload(download);
        }

	    private void ChangePassword_Click(object sender, MouseButtonEventArgs e)
	    {
	        var changePassWindow = new ChangePasswordWindow();
	        changePassWindow.ShowDialog(StaticMainWindow.Window);
	    }
	}
}