using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using TuneRoboWPF.Utility;
using comm;
using motion;
using NLog;

namespace TuneRoboWPF.StoreService.BigRequest
{
    public class DownloadMotionStoreRequest : IRequest
    {
        // Log
        #if DEBUG
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endif

        public delegate void SuccessfullyEventHandler(object sender);

        public event SuccessfullyEventHandler ProcessSuccessfully;

        private void OnProcessSuccessfully(object sender)
        {
            SuccessfullyEventHandler handler = ProcessSuccessfully;
            if (handler != null) handler(sender);
        }

        public delegate void ErrorEventHandler(DownloadMotionErrorCode errorCode, string errorMessage);

        public event ErrorEventHandler ProcessError;

        private void OnProcessError(DownloadMotionErrorCode errorCode, string errorMessage)
        {
            ErrorEventHandler handler = ProcessError;
            if (handler != null) handler(errorCode, errorMessage);
        }

        public delegate void CancelEventHandler();

        public event CancelEventHandler ProcessCancel;

        private void OnProcessCancel()
        {
            CancelEventHandler handler = ProcessCancel;
            if (handler != null) handler();
        }

        public delegate void ProgressReportEventHandler(int progressValue);

        public event ProgressReportEventHandler ProgressReport;
        private void OnProgessReport(int progressValue)
        {
            ProgressReportEventHandler handler = ProgressReport;
            if (handler != null) handler(progressValue);
        }


        public enum DownloadMotionErrorCode
        {
            IOError, DBError, NoPermission, ReplyNull, UnknownError
        }

        private object cancelLock = new object();
        private bool _cancelProcess;
        public bool CancelProcess
        {
            get
            {
                lock (cancelLock)
                {
                    return _cancelProcess;
                }
            }
            set
            {
                lock (cancelLock)
                {
                    _cancelProcess = value;
                }
            }
        }

        private ulong MotionID { get; set; }
        public DownloadMotionStoreRequest(ulong motionID)
        {
            MotionID = motionID;
        }

        private DownloadMotionErrorCode ConvertReplyErrorCode(Reply.Type errorCode)
        {
            switch (errorCode)
            {
                case Reply.Type.IO_ERROR:
                    return DownloadMotionErrorCode.IOError;
                case Reply.Type.DB_ERROR:
                    return DownloadMotionErrorCode.DBError;
                case Reply.Type.NO_PERMISSION:
                    return DownloadMotionErrorCode.NoPermission;
                default:
                    return DownloadMotionErrorCode.UnknownError;
            }
        }

        // Download steps:
        // 1. Call MotionDownloadRequest 
        // 2. Call ReadMotionDataRequest to get data


        public object Process()
        {
            int trunkSize = 16384;

#if Debug
            logger.Info("Download motion begin, id motion: " + MotionID);
#endif

            // Get info
            var motionDownloadRequest = new GetDowloadMotionInfoStoreRequest(MotionID);
            var motionDownloadReply = (Reply)motionDownloadRequest.Process();
            if (motionDownloadReply == null)
            {
                OnProcessError(DownloadMotionErrorCode.ReplyNull, "Reply is null");
#if DEBUG
                logger.Error("reply is null");
#endif
                return null;
            }
            if (motionDownloadReply.type != (decimal)Reply.Type.OK)
            {
                OnProcessError(ConvertReplyErrorCode((Reply.Type)motionDownloadReply.type), "Get motion info download failed: " + motionDownloadReply.type);
                #if DEBUG
                logger.Error("Get info motion download error");
                #endif
                return motionDownloadReply;
            }
            ulong motionSize = motionDownloadReply.motion_download.motion_file_size;
            ulong musicSize = motionDownloadReply.motion_download.music_file_size;

            #if DEBUG
            logger.Info("Motion size: " + motionSize);
            logger.Info("Music size: " + musicSize);
            #endif

            var numberOfMotionTrunk = (int)(motionSize / (decimal)trunkSize);
            var numberOfMusicTrunk = (int)(musicSize / (decimal)trunkSize);

            int totalPackets = numberOfMotionTrunk + numberOfMusicTrunk + 2;
            int count = 0;
            int currentPercentage = -1;
            int tempPercentage = 0;

            // Download motion
            var motionData = new List<byte>();
            for (int i = 0; i < numberOfMotionTrunk; i++)
            {
                if (CancelProcess)
                {
                    OnProcessCancel();
                    #if DEBUG
                    logger.Info("Cancel download");
                    #endif
                    return null;
                }
                var request = new DownloadMotionTrunkDataStoreRequest(MotionID, ReadMotionDataRequest.Type.MOTION,
                                                                      (ulong)(i * trunkSize), (ulong)trunkSize);
                var reply = (Reply)request.Process();
                if (reply == null)
                {
                    OnProcessError(DownloadMotionErrorCode.ReplyNull, "Reply is null");

#if DEBUG
                    logger.Error("Transfer motion error: reply is null"); 
#endif

                    return null;
                }
                if (reply.type != (decimal)Reply.Type.OK)
                {
                    OnProcessError(ConvertReplyErrorCode((Reply.Type)reply.type), "Download motion failed");

#if Debug
                    logger.Error("Transfer motion error: reply is error"); 
#endif

                    return reply;
                }
                motionData.AddRange(reply.read_data.data);
                count++;
                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);


#if Debug
                    logger.Info("Download percentages: " + currentPercentage); 
#endif
                }
            }
            var remainMotionDataSize = (int)(motionSize % (decimal)trunkSize);
            if (remainMotionDataSize != 0)
            {
                var remainMotionRequest = new DownloadMotionTrunkDataStoreRequest(MotionID,
                                                                                      ReadMotionDataRequest.Type.MOTION,
                                                                                      (ulong)(numberOfMotionTrunk * trunkSize),
                                                                                      (ulong)remainMotionDataSize);
                var remainMotionReply = (Reply)remainMotionRequest.Process();
                if (remainMotionReply == null)
                {
                    OnProcessError(DownloadMotionErrorCode.ReplyNull, "Reply is null");
#if Debug
                    logger.Error("Transfer motion error - last package: reply is null"); 
#endif
                    return null;
                }
                if (remainMotionReply.type != (decimal)Reply.Type.OK)
                {
                    OnProcessError(ConvertReplyErrorCode((Reply.Type)remainMotionReply.type), "Download motion failed");
#if Debug
                    logger.Error("Transfer motion error - last package: reply is error"); 
#endif
                    return remainMotionReply;
                }
                motionData.AddRange(remainMotionReply.read_data.data);
                count++;
                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);
#if Debug
                    logger.Error("Transfer motion error - last package: reply is null"); 
#endif
                }
#if Debug
                logger.Info("Download motion done"); 
#endif
            }


            // Download music
            var musicData = new List<byte>();
            for (int i = 0; i < numberOfMusicTrunk; i++)
            {
                if (CancelProcess)
                {
                    OnProcessCancel();
#if Debug
                    logger.Info("Cancel download"); 
#endif
                    return null;
                }
                var request = new DownloadMotionTrunkDataStoreRequest(MotionID, ReadMotionDataRequest.Type.MUSIC,
                                                                      (ulong)(i * trunkSize), (ulong)trunkSize);
                var reply = (Reply)request.Process();
                if (reply == null)
                {
                    OnProcessError(DownloadMotionErrorCode.ReplyNull, "Reply is null");
#if Debug
                    logger.Error("Transfer music error: reply is null"); 
#endif
                    return null;
                }
                if (reply.type != (decimal)Reply.Type.OK)
                {
                    OnProcessError(ConvertReplyErrorCode((Reply.Type)reply.type), "Download motion failed");
#if Debug
                    logger.Error("Transfer music error: reply is error"); 
#endif
                    return reply;
                }
                musicData.AddRange(reply.read_data.data);
                count++;
                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);
#if Debug
                    logger.Info("Download percentages: " + currentPercentage); 
#endif
                }
            }
            var remainMusicDataSize = (int)(musicSize % (decimal)trunkSize);
            if (remainMusicDataSize != 0)
            {
                var remainMusicRequest = new DownloadMotionTrunkDataStoreRequest(MotionID,
                                                                                      ReadMotionDataRequest.Type.MUSIC,
                                                                                      (ulong)(numberOfMusicTrunk * trunkSize),
                                                                                      (ulong)remainMusicDataSize);
                var remainMusicReply = (Reply)remainMusicRequest.Process();
                if (remainMusicReply == null)
                {
                    OnProcessError(DownloadMotionErrorCode.ReplyNull, "Reply is null");
#if Debug
                    logger.Error("Transfer music error - last package: reply is null"); 
#endif
                    return null;
                }
                if (remainMusicReply.type != (decimal)Reply.Type.OK)
                {
                    OnProcessError(ConvertReplyErrorCode((Reply.Type)remainMusicReply.type), "Download motion failed");
#if Debug
                    logger.Error("Transfer music error - last package: reply is error"); 
#endif
                    return remainMusicReply;
                }
                musicData.AddRange(remainMusicReply.read_data.data);
                count++;
                tempPercentage = count * 100 / totalPackets;
                if (tempPercentage > currentPercentage)
                {
                    currentPercentage = tempPercentage;
                    OnProgessReport(currentPercentage);
#if Debug
                    logger.Info("Download percentages: " + currentPercentage); 
#endif
                }
            }

            string musicPath = Path.Combine(GlobalFunction.GetSavedDir(), MotionID.ToString() + ".mp3");
            File.WriteAllBytes(musicPath, musicData.ToArray());

            string motionPath = Path.Combine(GlobalFunction.GetSavedDir(), MotionID.ToString() + ".mrb");
            File.WriteAllBytes(motionPath, motionData.ToArray());

            OnProgessReport(100);

#if Debug
            logger.Info("Download done!"); 
#endif

            OnProcessSuccessfully(null);
            return null;
        }

        private class GetDowloadMotionInfoStoreRequest : StoreRequest
        {
            private ulong MotionID { get; set; }
            public GetDowloadMotionInfoStoreRequest(ulong id)
            {
                MotionID = id;
            }
            public override void BuildPacket()
            {
                base.BuildPacket();
                var request = new MotionDownloadRequest()
                                  {
                                      motion_id = MotionID
                                  };
                byte[] packetData;
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, request);
                    packetData = stream.ToArray();
                }
                GlobalVariables.CountRequest++;
                Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.DOWNLOAD_MOTION, 2,
                                                           packetData, GlobalVariables.CountRequest);
            }
        }

        private class DownloadMotionTrunkDataStoreRequest : StoreRequest
        {
            private ulong MotionID { get; set; }
            private ReadMotionDataRequest.Type Type { get; set; }
            private ulong Offset { get; set; }
            private ulong Size { get; set; }

            public DownloadMotionTrunkDataStoreRequest(ulong id, ReadMotionDataRequest.Type type, ulong offset, ulong size)
            {
                MotionID = id;
                Type = type;
                Offset = offset;
                Size = size;
            }

            public override void BuildPacket()
            {
                base.BuildPacket();
                var request = new ReadMotionDataRequest()
                                  {
                                      motion_id = MotionID,
                                      type = Type,
                                      offset = Offset,
                                      size = Size
                                  };
                byte[] packetData;
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, request);
                    packetData = stream.ToArray();
                }
                GlobalVariables.CountRequest++;
                Packet = StoreConnection.BuildServerPacket(packetData.Length + 16, (int)MessageType.Type.READ_MOTION_DATA, 2,
                                                           packetData, GlobalVariables.CountRequest);
            }
        }

    }
}
