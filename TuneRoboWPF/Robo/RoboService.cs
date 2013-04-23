using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.Robo
{
    public class RoboService
    {
        public static int GetMotionCount()
        {
            int motionCount = -1;
            var locker = new object();
            bool done = false;
            var getMotionCountRequest = new GetMotionCountRequest(mRoboPacket.PacketID.CountMotions);
            getMotionCountRequest.ProcessSuccessfully += (s) =>
            {
                motionCount = (int)GlobalFunction.LE4ToDec(s);
                lock (locker)
                {
                    done = true;
                    Monitor.Pulse(locker);
                }
            };
            getMotionCountRequest.ProcessError += (s) =>
            {
                lock (locker)
                {
                    done = true;
                    Monitor.Pulse(locker);
                }
            };
            GlobalVariables.MRoboConnectionWorker.AddJob(getMotionCountRequest);
            lock (locker)
            {
                while (!done)
                {
                    Monitor.Wait(locker);
                }
            }
            return motionCount;
        }

        
        public static MotionInfo GetMotionInfoAtIndex(int motionIndex)
        {
            var getMotionInfo = new GetMotionInfoAtIndexRequest(mRoboPacket.PacketID.GetInfoMotionAtIndex, motionIndex);
            MotionInfo motionInfo = null;
            var locker = new object();
            bool done = false;
            getMotionInfo.ProcessSuccessfully += (s) =>
                                                     {
                                                         Console.WriteLine("GetMotionInfoAtIndex: " + motionIndex);
                                                         motionInfo = new MotionInfo(s);
                                                         lock (locker)
                                                         {
                                                             done = true;
                                                             Monitor.Pulse(locker);
                                                         }
                                                     };
            getMotionInfo.ProcessError += (s) =>
                                              {
                                                  Console.WriteLine("GetMotionInfoAtIndex failed");
                                                  lock (locker)
                                                  {
                                                      done = true;
                                                      Monitor.Pulse(locker);
                                                  }
                                              };
            GlobalVariables.MRoboConnectionWorker.AddJob(getMotionInfo);
            lock (locker)
            {
                while (!done)
                {
                    Monitor.Wait(locker);
                }
            }
            return motionInfo;
        }
    }

    public class MotionInfo 
    {
        private byte[] InfoData { get; set; }

        public MotionInfo(byte[] infoData)
        {
            InfoData = infoData;
        }

        public ulong MotionID
        {
            get
            {
                byte[] tmp = GlobalFunction.SplitByteArray(InfoData, 0, 8);
                return GlobalFunction.LE8ToDec(tmp);
            }
        }


        public uint Duration
        {
            get
            {
                byte[] tmp = GlobalFunction.SplitByteArray(InfoData, 8, 4);
                return GlobalFunction.LE4ToDec(tmp);
            }
        }

        private int findCRPosition(int CRIndex)
        {
            int count = 0;
            int index = -1;
            for (int i = 12; i < InfoData.Length; i++)
            {
                if (InfoData[i] == 13)
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


        public string Title
        {
            get
            {
                int firstCRPos = findCRPosition(1);
                byte[] tmp = GlobalFunction.SplitByteArray(InfoData, 12, firstCRPos + 1 - 12);
                return Encoding.UTF8.GetString(tmp);
            }
        }

        public string Artist
        {
            get
            {
                int firstCRPos = findCRPosition(1);
                int secondCRPos = findCRPosition(2);
                byte[] tmp = GlobalFunction.SplitByteArray(InfoData, firstCRPos, secondCRPos - firstCRPos);
                return Encoding.UTF8.GetString(tmp);
            }
        }

        public string VersionName
        {
            get
            {
                int secondCRPos = findCRPosition(2);
                byte[] tmp = GlobalFunction.SplitByteArray(InfoData, 12, InfoData.Length - secondCRPos - 1);
                return Encoding.UTF8.GetString(tmp);
            }
        }
    }
}
