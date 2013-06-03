using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Win32;
using TuneRoboWPF.RobotService;

namespace TuneRoboWPF.Utility
{    
    class GlobalFunction
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
        string key,
        string val,
        string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
        string key,
        string def,
        StringBuilder retVal,
        int size,
        string filePath);

        public static bool WriteINI(string file, string section, string key, string value)
        {
            bool success = false;
            if (!Directory.Exists(GlobalVariables.SYSTEM_DIR + @"\tuneConfig"))
            {
                Directory.CreateDirectory(GlobalVariables.SYSTEM_DIR + @"\tuneConfig");
            }

            if (Directory.Exists(GlobalVariables.SYSTEM_DIR + @"\tuneConfig"))
            {
                var dir = new DirectoryInfo(GlobalVariables.SYSTEM_DIR + @"\tuneConfig");
                dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                WritePrivateProfileString(section, key, value, file);
                success = true;
            }
            return success;
        }

        public static string ReadINI(string file, string section, string key)
        {
            var SB = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", SB, 255, file);
            return SB.ToString();
        }

        public static Dictionary<string, string> ReadINI(string file)
        {
            var dic = new Dictionary<string, string>();
            // get the configuration file
            try
            {
                var config_file = new FileInfo(file);
                if (config_file.Exists)
                {
                    using (var stream_reader = new StreamReader(config_file.FullName))
                    {
                        string section = "[]";
                        string line;
                        string home_screen = null;
                        string dir = null;
                        string ipServer = null;
                        string portServer = null;
                        string ipWireless = null;
                        string portWireless = null;
                        // Read lines from the file until the end of 
                        while ((line = stream_reader.ReadLine()) != null)
                        {
                            // set the current section name
                            if (line.StartsWith("[") && line.EndsWith("]") && line != section)
                            {
                                section = line.ToUpper();
                            }

                            //Switch case settings
                            if (section == "[SETTINGS]")
                            {
                                // assign keywords from this section
                                if (line.ToUpper().StartsWith("HOMESCREEN=") && line.Length > 11)
                                {
                                    home_screen = line.Substring(11);
                                }
                                else if (line.ToUpper().StartsWith("DIRECTORY=") && line.Length > 10)
                                {
                                    dir = line.Substring(10);
                                }
                                else if (line.ToUpper().StartsWith("IPSERVER=") && line.Length > 9)
                                {
                                    ipServer = line.Substring(9);
                                }
                                else if (line.ToUpper().StartsWith("PORTSERVER=") && line.Length > 11)
                                {
                                    portServer = line.Substring(11);
                                }
                                else if (line.ToUpper().StartsWith("IPWIRELESS=") && line.Length > 11)
                                {
                                    ipWireless = line.Substring(11);
                                }
                                else if (line.ToUpper().StartsWith("PORTWIRELESS=") && line.Length > 13)
                                {
                                    portWireless = line.Substring(13);
                                }
                            }
                        }

                        dic["HOMESCREEN"] = home_screen;
                        dic["DIRECTORY"] = dir;
                        dic["IPSERVER"] = ipServer;
                        dic["PORTSERVER"] = portServer;
                        dic["IPWIRELESS"] = ipWireless;
                        dic["PORTWIRELESS"] = portWireless;
                    }
                }
            }
            catch
            {
                throw new Exception("My INI file is missing.");
            }

            return dic;
        }

        //
        //Construction Directories for LOCAL DIR and DEVICE DIR
        //
        public static void ReadConfig()
        {
            //Local and device directory
            GlobalVariables.SYSTEM_DIR = null;
            GlobalVariables.DEV_DIR = null;
            GlobalVariables.LOCAL_DIR = null;

            string sys_dir = GetSystemDirectory();
            if (null != sys_dir)
            {
                GlobalVariables.SYSTEM_DIR = sys_dir;
                GlobalVariables.LOCAL_DIR = sys_dir;
            }

            Dictionary<string, string> dic = ReadINI(GlobalVariables.SYSTEM_DIR + @"\tuneConfig" + GlobalVariables.CONFIG_FILE);
            if (dic.Count > 0)
            {
                foreach (var pair in dic)
                {
                    if (dic["HOMESCREEN"] != null) GlobalVariables.HIDE_HOME_SCREEN = dic["HOMESCREEN"];
                    if (dic["DIRECTORY"] != null) GlobalVariables.LOCAL_DIR = dic["DIRECTORY"];
                    if (dic["IPSERVER"] != null) GlobalVariables.IP_SERVER = dic["IPSERVER"].Replace(" ", "");
                    if (dic["PORTSERVER"] != null) GlobalVariables.PORT_SERVER = int.Parse(dic["PORTSERVER"].Replace(" ", ""));
                    if (dic["IPWIRELESS"] != null) GlobalVariables.IP_WIRELESS = dic["IPWIRELESS"].Replace(" ", "");
                    if (dic["PORTWIRELESS"] != null) GlobalVariables.PORT_WIRELESS = int.Parse(dic["PORTWIRELESS"].Replace(" ", ""));
                }
            }
        }

        //
        //Get directory value
        //
        public static string GetSystemDirectory()
        {
            string dir_path = null;
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)))
            {
                dir_path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            else if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)))
            {
                dir_path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            else
            {
                if (CheckExistAndCreateDirectory("C:"))
                {
                    dir_path = "C:";
                }
            }
            return dir_path;
        }

        public static void GetTempDataFolder()
        {            
            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                 GlobalVariables.FOLDER_ROOT;
            if (!Directory.Exists(appdataPath))
            {
                Directory.CreateDirectory(appdataPath);
            }
            GlobalVariables.AppDataFolder = appdataPath;            
        }

        // Check exist of folder path if not then create new directory
        public static bool CheckExistAndCreateDirectory(string dir)
        {
            bool chkExist = false;
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    chkExist = true;
                }

                if (Directory.Exists(dir))
                {
                    chkExist = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not create new directory. \n." + ex.ToString());
            }
            return chkExist;
        }

        //Check process is running
        public static bool GetProcessState(string process = null)
        {
            bool exist = false;
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                if (null != theprocess.ProcessName && !theprocess.ProcessName.Equals(""))
                {
                    //Console.WriteLine("Process: {0} ID: {1}", theprocess.ProcessName, theprocess.Id);
                    if (theprocess.ProcessName.ToString().Equals(process))
                    {
                        exist = true;
                    }
                }
            }
            return exist;
        }

        //Run process
        public static void StartProcess(string prog, string arg = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = prog;
            if (null != arg) startInfo.Arguments = arg;
            Process.Start(startInfo);
        }

        //Get Application is installed in system
        public static void GetInstalledApps(string application = null)
        {
            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string skName in rk.GetSubKeyNames())
                {
                    using (RegistryKey sk = rk.OpenSubKey(skName))
                    {
                        try
                        {
                            //Check files extension exist in usb device
                            if (null != sk.GetValue("DisplayName") && !sk.GetValue("DisplayName").Equals(""))
                            {
                                if (null != sk.GetValue("InstallLocation") && !sk.GetValue("InstallLocation").Equals(""))
                                {
                                    //Console.WriteLine("Name: {0}", sk.GetValue("DisplayName"));

                                    if (sk.GetValue("DisplayName").ToString().Contains(application))
                                    {
                                        StartProcess(sk.GetValue("InstallLocation") + application + ".exe");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error : {0}", ex.ToString());
                        }
                    }
                }
            }
        }



        public static bool CompareByteArray(byte[] a1, byte[] a2)
        {
            IStructuralEquatable eqa1 = a1;
            return eqa1.Equals(a2, StructuralComparisons.StructuralEqualityComparer);
        }

        public static string GetSavedDir()
        {
            return GlobalVariables.LOCAL_DIR + GlobalVariables.FOLDER_ROOT + GlobalVariables.FOLDER_PLAYLIST ;
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static DateTime ConvertFromServerTimestamp(double timestamp)
        {
            var serverOriginTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return serverOriginTime.AddSeconds(timestamp);
        }

        public static string GetLocalMotionPath(ulong motionID)
        {
            return
                Path.Combine(GlobalVariables.LOCAL_DIR + GlobalVariables.FOLDER_ROOT + GlobalVariables.FOLDER_PLAYLIST,
                             motionID.ToString() + ".mrb");
        }

        public static Utility.MotionInfo GetLocalMotionInfo(ulong motionID)
        {
            string motionPath = GetLocalMotionPath(motionID);
            return new Utility.MotionInfo(motionPath);
        }

        #region RemoteViaWifi
        // Convert a decimal number to a hexadecimal number in 2 byte little endian format
        public static byte[] DecToLE2(int num)
        {
            var le = new byte[2];
            le[0] = (byte)num;
            le[1] = (byte)(((uint)num >> 8) & 0xFF);
            return le;
        }

        // Convert a hexadecimal number in 2 byte big endian format to a decimal number
        public static int LE2ToDec(byte[] data)
        {
            return (data[1] << 8) | data[0];
        }



        // Convert a decimal number to a hexadecimal number in 4 byte little endian format
        public static byte[] DecToLE4(int num)
        {
            var le = new byte[4];
            le[0] = (byte)num;
            le[1] = (byte)(((uint)num >> 8) & 0xFF);
            le[2] = (byte)(((uint)num >> 16) & 0xFF);
            le[3] = (byte)(((uint)num >> 24) & 0xFF);
            return le;
        }

        // Convert a hexadecimal number in 4 byte little endian format to a decimal number
        //public static int LE4ToDec(byte[] data)
        //{
        //    return (data[3] << 24) | (data[2] << 16) | (data[1] << 8) | data[0];
        //}
        public static uint LE4ToDec(byte[] data)
        {
            return (uint)((data[3] << 24) | (data[2] << 16) | (data[1] << 8) | data[0]);
        }


        /// <summary>
        /// Convert an decimal number to little endian
        /// </summary>
        /// <param name="num">decimal number</param>
        /// <returns></returns>
        public static byte[] DecToLE8(ulong num)
        {
            var le = new byte[8];
            le[0] = (byte)num;
            le[1] = (byte)((num >> 8) & 0xFF);
            le[2] = (byte)((num >> 16) & 0xFF);
            le[3] = (byte)((num >> 24) & 0xFF);
            le[4] = (byte)((num >> 32) & 0xFF);
            le[5] = (byte)((num >> 40) & 0xFF);
            le[6] = (byte)((num >> 48) & 0xFF);
            le[7] = (byte)((num >> 56) & 0xFF);
            return le;
        }

        public static byte[] DecToLE8(int num)
        {
            var le = new byte[8];
            le[0] = (byte)num;
            le[1] = (byte)(((uint)num >> 8) & 0xFF);
            le[2] = (byte)(((uint)num >> 16) & 0xFF);
            le[3] = (byte)(((uint)num >> 24) & 0xFF);
            le[4] = (byte)(((uint)num >> 32) & 0xFF);
            le[5] = (byte)(((uint)num >> 40) & 0xFF);
            le[6] = (byte)(((uint)num >> 48) & 0xFF);
            le[7] = (byte)(((uint)num >> 56) & 0xFF);
            return le;
        }

        public static ulong LE8ToDec(byte[] le)
        {
            return
                (ulong)
                ((le[7] << 56) | (le[6] << 48) | (le[5] << 40) | (le[4] << 32) | (le[3] << 24) | (le[2] << 16) |
                 (le[1] << 8) | le[0]);
        }

        // Convert an array of byte to a hexadecimal string
        public static string ByteArrayToHexString(byte[] ba)
        {
            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        // Convert a hex string to a byte array
        public static byte[] HexStringToByteArray(String hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        // Convert a normal string to a hex string
        public static string ConvertStringToHex(string asciiString)
        {
            var hex = string.Empty;
            foreach (var c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        // Convert a hex string to a normal string
        public static string ConvertHexToString(string hexValue)
        {
            var strValue = string.Empty;
            while (hexValue.Length > 0)
            {
                strValue += System.Convert.ToChar(System.Convert.ToUInt32(hexValue.Substring(0, 2), 16)).ToString();
                hexValue = hexValue.Substring(2, hexValue.Length - 2);
            }
            return strValue;
        }

        // Convert a normal string to byte array
        public static byte[] ConvertStringToByteArray(string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        // Convert a byte array to normal string
        public static string ConvertByteArrayToString(byte[] a)
        {
            return Encoding.ASCII.GetString(a);
        }
        // Take some continuous elements of a byte array, then store in another byte array
        public static byte[] SplitByteArray(IEnumerable<byte> p, int offset, int numberOfBytesTaken)
        {
            return p.Skip(offset).Take(numberOfBytesTaken).ToArray();
        }

        /// <summary>
        /// Generate CRC  of a byte array, little endian encode
        /// </summary>
        /// <param name="byteArray">byte array</param>
        /// <returns></returns>

        public static byte[] GenerateCrc(byte[] byteArray)
        {
            var genCrc = new CRCCCIT(InitialCrcValue.NonZero1);
            byte[] array = genCrc.ComputeChecksumBytes(byteArray);
            return array.Reverse().ToArray();
        }

        /// <summary>
        /// Generate CRC string of a byte array
        /// </summary>
        /// <param name="byteArray">byte array</param>
        /// <returns></returns>

        public static string GenerateCrcInString(byte[] byteArray)
        {
            var crc = GenerateCrc(byteArray);
            return ByteArrayToHexString(crc);
        }

        // Find packet header
        public static int FindPacketHeader(byte[] data)
        {
            var index = -1;
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] == GlobalVariables.PACKET_HEADER)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }



        // Check CRC of a packet
        public static bool CheckCRC(byte[] packet)
        {
            var crcPacket = ByteArrayToHexString(SplitByteArray(packet, 3, 2));
            var data = SplitByteArray(packet, 8, packet.Length - 8);
            var crcData = GenerateCrcInString(data);
            return (crcData == crcPacket);
        }


        // Extract Length of data
        public static int ExtractLengthOfData(IList<byte> p)
        {
            var tmp = SplitByteArray(p, 1, 2);
            return LE2ToDec(tmp);
        }

        // Extract IdCommand
        public static int ExtractIdCommand(IList<byte> p)
        {
            var tmp = SplitByteArray(p, 8, 2);
            return LE2ToDec(tmp);
        }

        // Convert a file to byte array
        public static byte[] FileToByteArray(string filename)
        {
            var fInfo = new FileInfo(filename);
            if (!fInfo.Exists)
            {
                MessageBox.Show("File " + filename + " is not exist", "File error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            if (fInfo.Length > (100 * 1024 * 1024))
            {
                MessageBox.Show("File " + filename + " is too big", "File error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            return File.ReadAllBytes(filename);
        }

        // Update current list motion
        public static void UpdateCurrentListMotion(List<MotionInfo> listMotionInfo)
        {
            GlobalVariables.CurrentListMotion.Clear();
            GlobalVariables.CurrentListMotion.AddRange(listMotionInfo);
        }

        #endregion

        #region Server connection
        // Convert a decimal number to a hexadecimal number in 2 byte big endian format
        public static byte[] DecToBE2(int num)
        {
            var le = new byte[2];
            le[1] = (byte)num;
            le[0] = (byte)(((uint)num >> 8) & 0xFF);
            return le;
        }

        // Convert a hexadecimal number in 2 byte big endian format to a decimal number
        public static int BE2ToDec(byte[] data)
        {
            return (data[0] << 8) | data[1];
        }

        // Convert a decimal number to a hexadecimal number in 3 byte big endian format
        public static byte[] DecToBE3(int num)
        {
            var le = new byte[3];
            le[2] = (byte)num;
            le[1] = (byte)(((uint)num >> 8) & 0xFF);
            le[0] = (byte)(((uint)num >> 16) & 0xFF);
            return le;
        }

        // Convert a hexadecimal number in 3 byte big endian format to a decimal number
        public static int BE3ToDec(byte[] data)
        {
            return (data[0] << 16) | (data[1] << 8) | data[2];
        }

        // Convert a decimal number to a hexadecimal number in 4 byte big endian format
        public static byte[] DecToBE4(int num)
        {
            var le = new byte[4];
            le[3] = (byte)num;
            le[2] = (byte)(((uint)num >> 8) & 0xFF);
            le[1] = (byte)(((uint)num >> 16) & 0xFF);
            le[0] = (byte)(((uint)num >> 24) & 0xFF);
            return le;
        }

        // Convert a hexadecimal number in 4 byte big endian format to a decimal number
        public static int BE4ToDec(byte[] data)
        {
            return (data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3];
        }

        // Convert a decimal number to a hexadecimal number in 8 byte big endian format
        public static byte[] DecToBE8(ulong num)
        {
            var le = new byte[8];

            le[7] = (byte)num;
            le[6] = (byte)((num >> 8) & 0xFF);
            le[5] = (byte)((num >> 16) & 0xFF);
            le[4] = (byte)((num >> 24) & 0xFF);
            le[3] = (byte)((num >> 32) & 0xFF);
            le[2] = (byte)((num >> 40) & 0xFF);
            le[1] = (byte)((num >> 48) & 0xFF);
            le[0] = (byte)((num >> 56) & 0xFF);
            return le;
        }

        // Convert a hexadecimal number in 8 byte big endian format to a decimal number
        public static ulong BE8ToDec(byte[] data)
        {
            return (ulong)((data[0] << 56) | (data[1] << 48) | (data[2] << 40) | (data[3] << 32) | (data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]);
        }

        #endregion
    }
}
