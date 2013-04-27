

using System;
using System.Collections.Generic;
using System.Windows;
using TuneRoboWPF.RobotService;

namespace TuneRoboWPF.Utility
{
    public static class GlobalVariables
    {
        #region UI

        //============================== DEFINITIONS GLOBAL VARIABLES ==================================//
        public static string PROGRAM_NAME
        {
            get { return "tuneRobo"; }
        }
        public static string PROGRAM_TOOLTIP
        {
            get { return "tuneRobo version 1.0"; }
        }
        public static string PROGRAM_FONT
        {
            get { return "Times New Roman"; }
        }
        public static string WEBSITE_NAME
        {
            get { return "http://www.tosy.com"; }
        }
        public static string SUCCESS_TITLE
        {
            get { return "Success"; }
        }
        public static string WARNING_TITLE
        {
            get { return "Warning"; }
        }
        public static string ERROR_TITLE
        {
            get { return "Error"; }
        }
        public static string QUESTION_TITLE
        {
            get { return "Question"; }
        }
        public static string SYNC_TITLE
        {
            get { return "Synchronize"; }
        }

        //=========TEXT========//
        public static string SEARCH_TEXT
        {
            get { return "Search Motion"; }
        }
        public static string DEFAULT_FOLDER_TEXT
        {
            get { return "Set Default folder"; }
        }
        public static string BROWSER_TEXT
        {
            get { return "Browser"; }
        }
        public static string LOGIN_TEXT
        {
            get { return "Sign In"; }
        }
        public static string PREFERENCES_TEXT
        {
            get { return "Preferences"; }
        }
        public static string HELP_TEXT
        {
            get { return "Help contents"; }
        }
        public static string MOTION_TEXT
        {
            get { return "View Search Motion"; }
        }
        public static string PLAYLIST_TEXT
        {
            get { return "View Playlist"; }
        }
        public static string MOUNT_DEVICE_TEXT
        {
            get { return "Mount Device"; }
        }
        public static string DEVICE_SCAN_TEXT
        {
            get { return "Scan Devices"; }
        }
        public static string DEVICE_SCANNING_TEXT
        {
            get { return "Scanning"; }
        }

        public static string ABORT_PROCESS_TEXT
        {
            get { return "Abort process!"; }
        }
        //=========BUTTON========//
        public static string SYNC_TO_BUTTON
        {
            get { return "Synchronize to Device"; }
        }
        public static string SYNC_FROM_BUTTON
        {
            get { return "Synchronize from Device"; }
        }
        public static string MOVE_UP_BUTTON
        {
            get { return "Move Row Up"; }
        }
        public static string MOVE_DOWN_BUTTON
        {
            get { return "Move Row Down"; }
        }
        public static string EXIT_BUTTON
        {
            get { return "Exit"; }
        }
        public static string CLOSE_BUTTON
        {
            get { return "Close"; }
        }
        public static string MINIMIZE_BUTTON
        {
            get { return "Minimize"; }
        }
        public static string OK_BUTTON
        {
            get { return "Ok"; }
        }
        public static string CANCEL_BUTTON
        {
            get { return "Cancel"; }
        }

        #endregion
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

        //Home screen
        public static string HIDE_HOME_SCREEN { get; set; }

        // Server connection
        //public static ServerConnection serverConnection = ServerConnection.Instance;

        // Server IP Address
        public static string IP_SERVER { get; set; }

        // Server PORT number
        public static int PORT_SERVER { get; set; }

        public static string USER_NAME { get; set; }

        // Wireless connection IP Address
        public static string IP_WIRELESS { get; set; }

        // Server PORT number
        public static int PORT_WIRELESS { get; set; }
        public static int TIMEOUT
        {
            get { return 500; }
        }

        #endregion

        // Wireless connection
        public static bool WIRELESS_CONNECTION { get; set; }

        // USB connection
        public static bool USB_CONNECTION { get; set; }

        // Online user
        public static bool USER_ONLINE { get; set; }
        // Maximum of packet size
        public const int WirelessPacketMtu = 512;

        public static byte[] MRoboSessionID = new byte[] { 0, 0 };

        public static RobotState CurrentRobotState = new RobotState();
        public static List<MotionInfo> CurrentListMotion = new List<MotionInfo>();

        public static StoreWorker StoreWorker = StoreWorker.Instance;
        public static RoboWorker RobotWorker = RoboWorker.Instance;

        
        public const int PACKET_HEADER = 0x0080;
        public const int ID_ACK = 0x0001;
        public const int ID_ERROR = 0x0002;
        public const int ID_CRC_ERROR = 0x0003;
        static GlobalVariables()
        {
            USB_CONNECTION = false;
            WIRELESS_CONNECTION = false;
            IP_SERVER = null;
            PORT_SERVER = -1;
            USER_ONLINE = false;
            HIDE_HOME_SCREEN = null;
            DEV_DIR = null;
            LOCAL_DIR = null;
            SYSTEM_DIR = null;
        }
    }
}
