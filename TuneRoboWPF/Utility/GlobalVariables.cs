

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using TuneRoboWPF.RobotService;
using TuneRoboWPF.StoreService;
using comm;

namespace TuneRoboWPF.Utility
{
    public static class GlobalVariables
    {
        #region Config parameters
        //=========================OTHER DEFINITION FUNCTIONS ==============================//
        private static string[] mediaExtensions = {".MP3", ".MP4", ".WMA", 
                                                      ".WAV", ".WMV", ".OGG",
                                                        ".AVI"};
        public static string[] MEDIA_EXTENSIONS
        {
            get { return mediaExtensions; }
        }

        public static string MOTION_EXTENSION
        {
            get { return ".MRB"; }
        }

        //Directory system of program
        public static string SYSTEM_DIR { get; set; }

        //Directory of Store local playlist
        public static string LOCAL_DIR { get; set; }

        //Directory of Device
        public static string DEV_DIR { get; set; }

        //Folder ROOT
        private static string _folderRoot = @"\tuneRobo";
        public static string FOLDER_ROOT
        {
            get { return _folderRoot; }
            set { _folderRoot = value; }
        }

        //Folder Play list
        private static string _folderPlaylist = @"\motionplaylist";
        public static string FOLDER_PLAYLIST
        {
            get { return _folderPlaylist; }
            set { _folderPlaylist = value; }
        }

        //Play list file name
        private static string _playlistFile = @"\auto_playlist.txt";
        public static string PLAYLIST_FILE
        {
            get { return _playlistFile; }
            set { _playlistFile = value; }
        }

        //Key file name
        private static string _keyFile = @"\tune_robo_key.bin";
        public static string KEY_FILE
        {
            get { return _keyFile; }
            set { _keyFile = value; }
        }

        //Config file name
        private static string _configFile = @"\tuneRobo.ini";
        public static string CONFIG_FILE
        {
            get { return _configFile; }
            set { _configFile = value; }
        }

        public static string AppDataFolder { get; set; }

        //Home screen
        public static string HIDE_HOME_SCREEN { get; set; }

        // Server connection
        //public static ServerConnection serverConnection = ServerConnection.Instance;

        // Server IP Address
        public static string ServerIP { get; set; }

        // Server PORT number
        public static int ServerPort { get; set; }

        //public static string Username { get; set; }

        // Wireless connection IP Address
        public static string WirelessIP { get; set; }

        // Server PORT number
        public static int WirelessPort { get; set; }
        public static int Timeout
        {
            get { return 30000; }
        }

        #endregion

        // Wireless connection
        public static bool RoboOnline { get; set; }

        // USB connection
        public static bool USB_CONNECTION { get; set; }

        // Online user
        public static bool UserOnline { get; set; }
        public static UserProfile CurrentUser { get; set; }
        // Maximum of packet size
        public const int WirelessPacketMtu = 512;

        public static byte[] RobotSessionID = new byte[] { 0, 0 };

        public static RobotState CurrentRobotState = new RobotState();
        public static List<MotionInfo> CurrentListMotion = new List<MotionInfo>();

        public static StoreWorker StoreWorker = StoreWorker.Instance;
        public static RoboWorker RobotWorker = RoboWorker.Instance;
        public static ImageDownloadWorker ImageDownloadWorker = ImageDownloadWorker.Instance;

        public static StoreConnection ServerConnection = StoreConnection.Instance;
        public static ulong CountRequest = 0;

        public static double RateValueMultiplierFactor = 10.0;
        public const int PacketHeader = 0x0080;


        public static Dictionary<string,BitmapImage> ImageDictionary = new Dictionary<string, BitmapImage>();
        public static Dictionary<string, Reply> RequestDictionary = new Dictionary<string, Reply>();
        public static NavigationSystem Navigation = NavigationSystem.Instance;

        static GlobalVariables()
        {
            USB_CONNECTION = false;
            RoboOnline = false;
            ServerIP = null;
            ServerPort = -1;
            UserOnline = false;
            HIDE_HOME_SCREEN = null;
            DEV_DIR = null;
            LOCAL_DIR = null;
            SYSTEM_DIR = null;
        }
    }
}
