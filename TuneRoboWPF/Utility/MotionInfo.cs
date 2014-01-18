using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TuneRoboWPF.Utility
{
    public class MotionInfo
    {
        public enum MotionType
        {
            Transform = 1,
            Untransform = 2,
            Dance = 3
        }
        public string FilePath { get; set; }
        public MotionType MType { get; set; }
        public ulong MotionID { get; set; }
        public uint Duration { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public ulong VersionCode { get; set; }

        private Dictionary<string, int> FirstIndex = new Dictionary<string, int>();
        public MotionInfo(byte[] infoData)
        {
            FirstIndex["type"] = 0;
            FirstIndex["id"] = 1;
            FirstIndex["duration"] = 9;
            FirstIndex["rest"] = 13;
            GetInfo(infoData);
        }

        private void GetInfo(byte[] infoData)
        {
            byte[] typeBytes = GlobalFunction.SplitByteArray(infoData, FirstIndex["type"], 1);
            MType = (MotionType)typeBytes[0];

            byte[] idBytes = GlobalFunction.SplitByteArray(infoData, FirstIndex["id"], 8);
            MotionID = GlobalFunction.LE8ToDec(idBytes);

            byte[] durationBytes = GlobalFunction.SplitByteArray(infoData, FirstIndex["duration"], 4);
            Duration = GlobalFunction.LE4ToDec(durationBytes);

            int firstCRPos = FindCRPosition(1, infoData);
            byte[] titleBytes = GlobalFunction.SplitByteArray(infoData, FirstIndex["rest"], firstCRPos - FirstIndex["rest"]);
            Title = Encoding.UTF8.GetString(titleBytes);

            int secondCRPos = FindCRPosition(2, infoData);
            byte[] artistBytes = GlobalFunction.SplitByteArray(infoData, firstCRPos + 1, secondCRPos - firstCRPos - 1);
            Artist = Encoding.UTF8.GetString(artistBytes);

            byte[] versionBytes = GlobalFunction.SplitByteArray(infoData, secondCRPos + 1, infoData.Length - secondCRPos - 2);
            var versionString = Encoding.UTF8.GetString(versionBytes);
            if (!string.IsNullOrWhiteSpace(versionString)) VersionCode = ulong.Parse(versionString);
            else
            {
                Debug.Fail("Cannot get version code");
            }
        }

        private int FindCRPosition(int CRIndex, byte[] data)
        {
            int count = 0;
            int index = -1;
            for (int i = 12; i < data.Length; i++)
            {
                if (data[i] == 13)
                {
                    count++;
                    if (count == CRIndex)
                    {
                        index = i;
                        break;
                    }
                }
            }
            return index;
        }

        public MotionInfo(string filePath)
        {
            GetInfo(filePath);
        }

        private void GetInfo(string filePath)
        {
            FilePath = filePath;
            string[] lines = File.ReadAllLines(filePath).ToArray();

            MotionID = GetContentUlong(lines[1]);
            Title = GetContentString(lines[2]);
            Artist = GetContentString(lines[3]);
            VersionCode = GetContentUlong(lines[4]);
            Duration = (uint)GetContentDouble(lines[5]);
        }

        private int FindEqualOperator(string s)
        {
            return s.IndexOf("=", System.StringComparison.Ordinal);
        }

        private string GetContentString(string line)
        {
            string s = RemoveAllWhiteSpace(line);
            int equalIndex = FindEqualOperator(s);
            return s.Substring(equalIndex + 1, s.Length - equalIndex - 1);
        }

        private ulong GetContentUlong(string line)
        {
            string ulongString = GetContentString(line);
            ulong result;
            if (ulong.TryParse(ulongString, out result))
            {
                return result;
            }
            return 1;
        }

        private uint GetContentUint(string line)
        {
            string uintString = GetContentString(line);
            uint result;
            if (uint.TryParse(uintString, out result))
            {
                return result;
            }
            return 1;
        }

        private double GetContentDouble(string line)
        {
            string doubleString = GetContentString(line);
            double result;
            if (double.TryParse(doubleString, out result))
            {
                return result;
            }
            return 0.0;
        }

        private static string RemoveAllWhiteSpace(string s)
        {
            return s.Trim();
        }

        public static bool IsDanceMotion(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath).ToArray();
            if (RemoveAllWhiteSpace(lines[0]) == "type=3") return true;
            return false;
        }

    }
}
