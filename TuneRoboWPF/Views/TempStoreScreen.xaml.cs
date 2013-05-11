﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using TuneRoboWPF.StoreService.BigRequest;
using TuneRoboWPF.Utility;
using TuneRoboWPF.ViewModels;
using TuneRoboWPF.StoreService.SimpleRequest;
using comm;
using motion;
using artist;
using user;
using UserControl = System.Windows.Controls.UserControl;

namespace TuneRoboWPF.Views
{
    /// <summary>
    /// Interaction logic for MainStoreScreen.xaml
    /// </summary>
    public partial class TempStoreScreen : UserControl
    {
        public TempStoreScreen()
        {
            this.InitializeComponent();
            DataContext = new TempStoreScreenViewModel();
            viewModel = (TempStoreScreenViewModel)DataContext;
            viewModel.FeaturedItemsList.Clear();
            viewModel.HotItemsList.Clear();
            viewModel.ArtistItemsList.Clear();
        }
        private TempStoreScreenViewModel viewModel = new TempStoreScreenViewModel();

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Cursor = Cursors.Wait;
                GetHotList();
                GetFeaturedList();
                GetArtistList();
            }
        }

        private bool getHotListDone, getFeaturedListDone, getArtistListDone;
        private List<MotionShortInfo> hotList = new List<MotionShortInfo>();
        private void GetHotList()
        {
            var hotListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.ALL, 0, 20);
            hotListRequest.ProcessSuccessfully += (reply) =>
            {
                hotList.AddRange(reply.list_motion.motion_short_info);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var info in reply.list_motion.motion_short_info)
                    {
                        var motionTitleItem = new MotionTitleItem();
                        motionTitleItem.ViewModel.Title = info.title;
                        viewModel.HotItemsList.Add(motionTitleItem);
                    }

                    if (getFeaturedListDone && getArtistListDone) Cursor = Cursors.Arrow;
                    else getHotListDone = true;
                });

            };
            hotListRequest.ProcessError += (reply, msg) =>
                                               {
                                                   Console.WriteLine("Host list request failed " + msg);
                                                   if (getFeaturedListDone && getArtistListDone)
                                                   {
                                                       Dispatcher.BeginInvoke((Action)delegate
                                                                                           {
                                                                                               Cursor = Cursors.Arrow;
                                                                                           });
                                                   }
                                                   else getHotListDone = true;
                                               };
            GlobalVariables.StoreWorker.AddJob(hotListRequest);
        }

        private void GetFeaturedList()
        {
            var featuredListRequest = new ListCategoryMotionStoreRequest(CategoryMotionRequest.Type.FEATURE, 0, 20);
            featuredListRequest.ProcessSuccessfully += (reply) =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var info in reply.list_motion.motion_short_info)
                    {
                        var motionTitleItem = new MotionTitleItem();
                        motionTitleItem.ViewModel.Title = info.title;
                        viewModel.FeaturedItemsList.Add(motionTitleItem);
                    }
                    if (getHotListDone && getArtistListDone) Cursor = Cursors.Arrow;
                    else getFeaturedListDone = true;
                });

            };
            featuredListRequest.ProcessError += (reply, msg) =>
            {
                Console.WriteLine("Featured list request failed " + msg);
                if (getHotListDone && getArtistListDone)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Cursor = Cursors.Arrow;
                    });
                }
                else getFeaturedListDone = true;
            };
            GlobalVariables.StoreWorker.AddJob(featuredListRequest);
        }

        private List<ArtistShortInfo> artistList = new List<ArtistShortInfo>();
        private void GetArtistList()
        {
            var artistListRequest = new ListAllArtistRequest(0, 20);
            artistListRequest.ProcessSuccessfully += (reply) =>
            {
                artistList.AddRange(reply.list_artist.artist_short_info);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    foreach (var info in reply.list_artist.artist_short_info)
                    {
                        var motionTitleItem = new MotionTitleItem();
                        motionTitleItem.ViewModel.Title = info.artist_name;
                        viewModel.ArtistItemsList.Add(motionTitleItem);
                    }
                    if (getHotListDone && getFeaturedListDone) Cursor = Cursors.Arrow;
                    else getArtistListDone = true;
                });

            };
            artistListRequest.ProcessError += (reply, msg) =>
            {
                Console.WriteLine("Artist list request failed " + msg);
                if (getHotListDone && getFeaturedListDone)
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Cursor = Cursors.Arrow;
                    });
                }
                else getArtistListDone = true;
            };
            GlobalVariables.StoreWorker.AddJob(artistListRequest);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            ulong motionID = hotList[HotListBox.SelectedIndex].motion_id;
            var request = new DownloadMotionStoreRequest(motionID);
            var transferWindow = new TransferWindow(request, motionID.ToString());
            transferWindow.ShowDialog();

        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            ulong motionID = hotList[HotListBox.SelectedIndex].motion_id;
            var infoRequest = new GetMotionFullInfoStoreRequest(motionID);
            infoRequest.ProcessSuccessfully += (reply) =>
                                                   {
                                                       Console.WriteLine("Motion ID: " + reply.motion_info.motion_id);
                                                       Console.WriteLine("Title: " + reply.motion_info.title);
                                                       Console.WriteLine("Description: " + reply.motion_info.description);
                                                       Console.WriteLine("Artist ID: " + reply.motion_info.artist_id);
                                                       Console.WriteLine("Artist name: " + reply.motion_info.artist_name);
                                                       Console.WriteLine("Publisher ID: " + reply.motion_info.publisher_id);
                                                       Console.WriteLine("Version name: " + reply.motion_info.version_name);
                                                       Console.WriteLine("Version ID: " + reply.motion_info.version_id);
                                                       Console.WriteLine("Video url: " + reply.motion_info.video_url);
                                                       Console.WriteLine("Screen shot url: " + reply.motion_info.screenshoot_ulrs[0]);
                                                       Console.WriteLine("View: " + reply.motion_info.view);
                                                       Console.WriteLine("Download count: " + reply.motion_info.download_count);
                                                       Console.WriteLine("Rating: " + reply.motion_info.rating);
                                                       Console.WriteLine("Rating count: " + reply.motion_info.rating_count);
                                                       Console.WriteLine("Motion file size: " + reply.motion_info.motion_file_size);
                                                       Console.WriteLine("Music file size: " + reply.motion_info.music_file_size);
                                                       Console.WriteLine("Duration: " + reply.motion_info.duration);
                                                       Console.WriteLine("Last modified: " + reply.motion_info.lastmodified);
                                                       Console.WriteLine("Update ID: " + reply.motion_info.update_id);
                                                   };
            infoRequest.ProcessError += (reply, msg) =>
                                            {
                                                Console.WriteLine("Get motion info failed: " + msg);
                                            };
            GlobalVariables.StoreWorker.AddJob(infoRequest);
        }

        private void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            ulong artistID = artistList[ArtistListBox.SelectedIndex].artist_id;
            string artistName = artistList[ArtistListBox.SelectedIndex].artist_name;

            var followRequest = new FollowArtistStoreRequest(artistID);
            followRequest.ProcessSuccessfully += (reply) =>
                                                     {
                                                         if ((bool)reply)
                                                         {
                                                             Console.WriteLine("User " + GlobalVariables.CurrentUser +
                                                                               " followed artist " + artistName);
                                                             Console.WriteLine("Now relationship is no follow");
                                                         }
                                                         else
                                                         {
                                                             Console.WriteLine("User " + GlobalVariables.CurrentUser +
                                                                               " didn't follow artist " + artistName);
                                                             Console.WriteLine("Now relationship is follow");
                                                         }
                                                     };
            followRequest.ProcessError += (reply, msg) =>
                                              {
                                                  Console.WriteLine("Follow failed: " + msg);
                                              };
            GlobalVariables.StoreWorker.AddJob(followRequest);
        }

        private void GetVersionButton_Click(object sender, RoutedEventArgs e)
        {
            var listMotionID = new List<ulong>();
            foreach (var motion in hotList)
            {
                listMotionID.Add(motion.motion_id);
            }
            var versionRequest = new GetMotionVersionStoreRequest(listMotionID);
            versionRequest.ProcessSuccessfully += (reply) =>
                                                      {
                                                          for (int i = 0; i < listMotionID.Count; i++)
                                                          {
                                                              Console.WriteLine("Version of motion {0:s} is {1:s} ", hotList[i].title, reply.motion_version.version[i].version_name);
                                                          }
                                                      };
            versionRequest.ProcessError += (reply, msg) =>
                                               {
                                                   Console.WriteLine("Ger motion version failed: " + msg);
                                               };
            GlobalVariables.StoreWorker.AddJob(versionRequest);
        }

        private void GetMotionDownloadByUserButton_Click(object sender, RoutedEventArgs e)
        {
            var request = new GetMotionDownloadByUserStoreRequest(0, 20);
            request.ProcessSuccessfully += (reply) =>
                                               {
                                                   foreach (var motion in reply.user_motion.motion_short_info)
                                                   {
                                                       Console.WriteLine("Motion download by user {0:s} has ID: {1:d}", motion.title, motion.motion_id);
                                                   }
                                               };
            request.ProcessError += (reply, msg) =>
                                        {
                                            Console.WriteLine("Get motion download by user failed" + msg);
                                        };
            GlobalVariables.StoreWorker.AddJob(request);
        }

        private void RatingButton_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            int rating = random.Next(0, 10);

            ulong motionID = 29;
            uint versionID = 27;
            string commentTitle = "good";
            string commentContent = "why not?";

            var ratingRequest = new RatingMotionStoreRequest(motionID, (uint) rating, versionID, commentTitle,
                                                             commentContent);
            ratingRequest.ProcessSuccessfully += (reply) =>
                                                     {
                                                         Console.WriteLine("Rating succeed");
                                                     };
            ratingRequest.ProcessError += (reply, msg) =>
                                              {
                                                  Console.WriteLine("Rating failed: " + msg);
                                              };
            GlobalVariables.StoreWorker.AddJob(ratingRequest);
        }

        private void RatingInfoButton_Click(object sender, RoutedEventArgs e)
        {
            var request = new GetRatingMotionInfoStoreRequest(29, 0, 20);
            request.ProcessSuccessfully += (reply) =>
                                               {
                                                   Console.WriteLine("Rating motion ID: {0:d}",reply.rating_info.motion_id);
                                                   
                                                   for (int i = 0; i < reply.rating_info.version_name.Count;i++)
                                                   {
                                                       Console.WriteLine("User rate: {0:s}", reply.rating_info.user_name[i]);
                                                       Console.WriteLine("Rating: {0:d}", reply.rating_info.rating[i]);
                                                       Console.WriteLine("Rating time: {0:d}", reply.rating_info.rating_time[i]);
                                                       Console.WriteLine("Version rating: {0:s}", reply.rating_info.version_name[i]);
                                                       Console.WriteLine("Comment title: {0:s}", reply.rating_info.comment_title[i]);
                                                       Console.WriteLine("Comment content: {0:s}", reply.rating_info.comment_content[i]);
                                                       Console.WriteLine("=================");
                                                   }
                                               };
            request.ProcessError += (reply, msg) =>
                                        {
                                            Console.WriteLine("Get rating info failed: " + msg);
                                        };
            GlobalVariables.StoreWorker.AddJob(request);
        }

        private void ListArtistFollowUser_Click(object sender, RoutedEventArgs e)
        {
            var request = new ListArtistFollowByUserStoreRequest();
            request.ProcessSuccessfully += (reply) =>
                                               {
                                                   foreach(var artist in reply.user_artist.artist_short_info)
                                                   {
                                                       Console.WriteLine("User {0:s} followed artist {1:s}", GlobalVariables.CurrentUser,artist.artist_name);
                                                   }
                                               };
            request.ProcessError += (reply, msg) =>
                                        {
                                            Console.WriteLine("List artist follow by current failed: {0:s}",msg);
                                        };
            GlobalVariables.StoreWorker.AddJob(request);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchTextBox.Text;
            var request = new SearchMotionStoreRequest(query, 0, 20);
            request.ProcessSuccessfully += (reply) =>
                                               {
                                                   Console.WriteLine("==============");
                                                   foreach (var motion in reply.search_motion.motion_short_info)
                                                   {
                                                       Console.WriteLine("Search result: {0:s} - id: {1:d}",motion.title,motion.motion_id);
                                                   }
                                               };
            request.ProcessError += (reply, msg) =>
                                        {
                                            Console.WriteLine("Search motion failed: {0:s}",msg);
                                        };
            GlobalVariables.StoreWorker.AddJob(request);
        }
    }
}