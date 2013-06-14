﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Views;
using TuneRoboWPF.Windows;

namespace TuneRoboWPF
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
            imageDownload.DownloadFailed += (s, msg) => Debug.Fail(msg);
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
                    DownloadAvatar();
                });
            };
            profileRequest.ProcessError += (reply, msg) => Debug.Fail(reply.type.ToString(), msg);
            GlobalVariables.StoreWorker.ForceAddRequest(profileRequest);
        }

        private void UpdateProfileCall()
        {
            var window = new UpdateProfileWindow();
            if(window.ShowDialog() == true)
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
                });
	        purchaseRequest.ProcessError += (reply, msg) => Debug.Fail(reply.type.ToString(), msg);
            GlobalVariables.StoreWorker.ForceAddRequest(purchaseRequest);
	    }

        private void PurchasedMotion_MotionClicked(ulong motionID)
        {
            var detailScreen = new MotionDetailScreen(motionID);
            StaticMainWindow.Window.ChangeScreen(detailScreen);
        }

        private void DownloadMotionImage(string url, MotionItemVertical motion)
        {
            var download = new ImageDownload(url);
            download.DownloadCompleted += (image) => Dispatcher.BeginInvoke((Action)(() => motion.SetImage(image)));
            download.DownloadFailed += (s, msg) => Debug.Fail(msg);
            GlobalVariables.ImageDownloadWorker.AddDownload(download);
        }
	}
}