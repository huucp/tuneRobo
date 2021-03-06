﻿using System;
using System.Collections.Generic;
using TuneRoboWPF.Utility;

namespace TuneRoboWPF.RobotService
{
    public class RobotPacket
    {
        /// <summary>
        /// Command ID
        /// </summary>
        public enum PacketID
        {
            Hello = 0x0101,
            GetState = 0x0102,
            Play = 0x0201,
            Pause = 0x0202,
            Forward = 0x0203,
            Backward = 0x0204,
            OpenTransform = 0x0205,
            CloseTransform = 0x0206,
            VolumeUp = 0x0207,
            VolumeDown = 0x0208,
            Mute = 0x0209,
            SetVolumeLevel = 0x020A,
            SelectMotionToPlay = 0x020B,
            Stop = 0x020C,
            DeleteMotion = 0x0301,
            CountMotions = 0x0309,
            RequestNextFileName = 0x030A,
            EndListFile = 0x030B,
            GetInfoMotionAtIndex = 0x0401,
            GetInfoMotionWithMotionID = 0x0402,
            OpenLogFile = 0x0403,
            ReadLogFile = 0x0404,
            CloseLogFile = 0x0405,
            CreateMotionPair = 0x0406,
            WriteMotionData = 0x0407,
            CloseMotionFile = 0x0408,
            WriteMusicData = 0x0409,
            CloseMotionPair = 0x040A,
            CancelTransferMotionPair = 0x040B
        }


        private const byte IdentificationByte = 128;
        private byte[] ReserveBytes = new byte[] { 0, 0, 0 };
        private PacketID commandID;
        public byte[] Parameters { get; set; }

        public RobotPacket(PacketID id)
        {
            commandID = id;
            Parameters = new byte[] { };
            //if
        }



        public byte[] BuildPacket()
        {
            var dataLength = 4;
            var data = new List<byte>();
            data.AddRange(GlobalFunction.DecToLE2((int)commandID));
            if (commandID == PacketID.Hello)
            {
                dataLength = 2;
            }
            else
            {
                data.AddRange(GlobalVariables.RobotSessionID);
            }

            if (Parameters != null)
            {
                data.AddRange(Parameters);
                dataLength += Parameters.Length;
            }

            var packet = new List<byte>();
            packet.Add(IdentificationByte);
            packet.AddRange(GlobalFunction.DecToLE2(dataLength));
            //packet.AddRange(GlobalFunction.GenerateCrc(data.ToArray()));            
            packet.AddRange(new byte[2] { 0, 0 });
            packet.AddRange(ReserveBytes);
            packet.AddRange(data);

            return packet.ToArray();
        }
    }
}
