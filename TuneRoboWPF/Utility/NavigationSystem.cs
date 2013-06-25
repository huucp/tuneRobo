using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public void AddScreen(Screen screen)
        {
            if (CurrentIndex < ScreenList.Count - 1 && !CompareScreen(screen, ScreenList[CurrentIndex + 1])
                && ScreenList.Count - (CurrentIndex + 1) > 0)
            {
                ScreenList.RemoveRange(CurrentIndex + 1, ScreenList.Count - (CurrentIndex + 1));
            }
            ScreenList.Add(screen);
            CurrentIndex++;
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
            return GetCurrentScreen();
        }

        public UserControl Previous()
        {
            if (CurrentIndex < 1)
            {
                return null;
            }
            CurrentIndex--;
            return GetCurrentScreen();
        }

        private UserControl GetCurrentScreen()
        {
            switch (ScreenList[CurrentIndex].Type)
            {
                case Screen.ScreenType.StoreScreen:
                    var storeScreen = new StoreScreen();
                    storeScreen.SetInfo(false);
                    return storeScreen;
                case Screen.ScreenType.MotionDetail:
                    var motionScreen = new MotionDetailScreen();
                    motionScreen.SetInfo(ScreenList[CurrentIndex].Parameter,false);
                    return motionScreen;
                case Screen.ScreenType.ArtistDetail:
                    var artistScreen = new ArtistDetailScreen();
                    artistScreen.SetInfo(ScreenList[CurrentIndex].Parameter);
                    break;
                default:
                    Debug.Fail(string.Format("GetCurrentScreen error, index{0}", CurrentIndex));
                    return null;
            }
            return null;
        }
    }

    public class Screen
    {
        public enum ScreenType
        {
            StoreScreen, MotionDetail, ArtistDetail
        }

        public ScreenType Type { get; set; }
        public ulong Parameter { get; set; }
        public Screen(ScreenType type, ulong param = 0)
        {
            Type = type;
            Parameter = param;
        }
    }
}
