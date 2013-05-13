using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TuneRoboWPF.Utility
{
    public class MotionInfo
    {
        public string FilePath { get; set; }
        public ulong MotionID { get; set; }
        public uint Duration { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string VersionName { get; set; }

        public MotionInfo(byte[] infoData)
        {
            GetInfo(infoData);
        }

        private void GetInfo(byte[] infoData)
        {
            byte[] idBytes = GlobalFunction.SplitByteArray(infoData, 0, 8);
            MotionID = GlobalFunction.LE8ToDec(idBytes);

            byte[] durationBytes = GlobalFunction.SplitByteArray(infoData, 8, 4);
            Duration = GlobalFunction.LE4ToDec(durationBytes);

            int firstCRPos = FindCRPosition(1, infoData);
            byte[] titleBytes = GlobalFunction.SplitByteArray(infoData, 12, firstCRPos + 1 - 12);
            Title = Encoding.UTF8.GetString(titleBytes);

            int secondCRPos = FindCRPosition(2, infoData);
            byte[] artistBytes = GlobalFunction.SplitByteArray(infoData, firstCRPos, secondCRPos - firstCRPos);
            Artist = Encoding.UTF8.GetString(artistBytes);

            byte[] versionBytes = GlobalFunction.SplitByteArray(infoData, 12, infoData.Length - secondCRPos - 1);
            VersionName = Encoding.UTF8.GetString(versionBytes);
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
            VersionName = GetContentString(lines[4]);
            Duration = GetContentUint(lines[5]);
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
            return ulong.Parse(ulongString);
        }

        private uint GetContentUint(string line)
        {
            string uintString = GetContentString(line);
            return uint.Parse(uintString);
        }

        private static string RemoveAllWhiteSpace(string s)
        {
            return s.Replace(" ", "");
        }

        public static bool IsDanceMotion(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath).ToArray();
            if (RemoveAllWhiteSpace(lines[0]) == "type=3") return true;
            return false;
        }
    }
}
