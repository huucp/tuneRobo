using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using TuneRoboWPF.Views;

namespace TuneRoboWPF.Utility
{
    public class NavigationSystem
    {

        private static readonly Lazy<NavigationSystem> Lazy = new Lazy<NavigationSystem>(() => new NavigationSystem());

        public static NavigationSystem Instance { get { return Lazy.Value; } }

        private List<Screen> ScreenList { get; set; }

        private int CurrentIndex { get; set; }
        private NavigationSystem()
        {
            ScreenList = new List<Screen>();
            CurrentIndex = -1;
        }

        private void UpdateNavigationButton()
        {
            // if (CurrentIndex==-1) return;
            if (CurrentIndex > ScreenList.Count - 2)
            {

                StaticMainWindow.Window.navigationBar.ViewModel.ForwardEnable = false;
            }
            else
            {
                StaticMainWindow.Window.navigationBar.ViewModel.ForwardEnable = true;
            }
            if (CurrentIndex < 1)
            {

                StaticMainWindow.Window.navigationBar.ViewModel.PreviousEnable = false;
            }
            else
            {
                StaticMainWindow.Window.navigationBar.ViewModel.PreviousEnable = true;
            }


        }

        public void AddScreen(Screen screen)
        {
            if (CurrentIndex < ScreenList.Count - 1 && !CompareScreen(screen, ScreenList[CurrentIndex + 1])
                && ScreenList.Count - (CurrentIndex + 1) > 0)
            {
                ScreenList.RemoveRange(CurrentIndex + 1, ScreenList.Count - (CurrentIndex + 1));
            }
            ScreenList.Add(screen);
            CurrentIndex++;
            UpdateNavigationButton();
        }

        private bool CompareScreen(Screen a, Screen b)
        {
            return ((a.Type == b.Type) && (a.Parameter == b.Parameter));
        }

        public UserControl Forward()
        {
            if (CurrentIndex > ScreenList.Count - 2)
            {
                return null;
            }
            CurrentIndex++;
            UpdateNavigationButton();
            return GetCurrentScreen();
        }

        public UserControl Previous()
        {
            if (CurrentIndex < 1)
            {
                return null;
            }
            CurrentIndex--;
            UpdateNavigationButton();
            return GetCurrentScreen();
        }

        private UserControl GetCurrentScreen()
        {
            switch (ScreenList[CurrentIndex].Type)
            {
                case Screen.ScreenType.StoreScreen:
                    var storeScreen = new NewStoreScreen();
                    storeScreen.SetInfo(false);
                    return storeScreen;
                case Screen.ScreenType.MotionDetail:
                    var motionScreen = new MotionDetailScreen();
                    var motionID = (ulong)ScreenList[CurrentIndex].Parameter;
                    motionScreen.SetInfo(motionID, false);
                    return motionScreen;
                case Screen.ScreenType.ArtistDetail:
                    var artistScreen = new ArtistDetailScreen();
                    var artistID = (ulong)ScreenList[CurrentIndex].Parameter;
                    artistScreen.SetInfo(artistID, false);
                    return artistScreen;
                case Screen.ScreenType.Search:
                    var searchScreen = new SearchResultScreen();
                    var query = (string)ScreenList[CurrentIndex].Parameter;
                    searchScreen.SetQuery(query, false);
                    return searchScreen;
                case Screen.ScreenType.SeeAll:
                    var seeAllScreen = new SeeAllScreen();
                    var category = (string)ScreenList[CurrentIndex].Parameter;
                    seeAllScreen.SetCategory(category, false);
                    return seeAllScreen;
                default:
                    Debug.Fail(string.Format("GetCurrentScreen error, index: {0}", CurrentIndex));
                    return null;
            }
            return null;
        }
    }

    public class Screen
    {
        public enum ScreenType
        {
            StoreScreen, MotionDetail, ArtistDetail, Search, SeeAll
        }

        public ScreenType Type { get; set; }
        public object Parameter { get; set; }
        public Screen(ScreenType type, object param = null)
        {
            Type = type;
            Parameter = param;
        }
    }
}
