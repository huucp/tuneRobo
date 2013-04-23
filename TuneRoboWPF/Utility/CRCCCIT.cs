namespace TuneRoboWPF.Utility
{
    public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }

    public class CRCCCIT
    {
        const ushort poly = 4129;
        ushort[] table = new ushort[256];
        ushort initialValue = 0;

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = initialValue;
            for (int i = 0; i < bytes.Length; ++i)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            //return BitConverter.GetBytes(crc);
            return new byte[] { (byte)(crc >> 8), (byte)(crc & 0x00ff) };
        }

        public CRCCCIT(InitialCrcValue initialValue)
        {
            this.initialValue = (ushort)initialValue;
            ushort temp, a;
            for (int i = 0; i < table.Length; ++i)
            {
                temp = 0;
                a = (ushort)(i << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if (((temp ^ a) & 0x8000) != 0)
                    {
                        temp = (ushort)((temp << 1) ^ poly);
                    }
                    else
                    {
                        temp <<= 1;
                    }
                    a <<= 1;
                }
                table[i] = temp;
            }
        }
    }
    //public enum Crc16Mode : ushort { Standard = 0xA001, CcittKermit = 0x8408 }

    //public class CRCCCIT
    //{
    //    static ushort[] table = new ushort[256];

    //    public ushort ComputeChecksum(params byte[] bytes)
    //    {
    //        ushort crc = 0;
    //        for (int i = 0; i < bytes.Length; ++i)
    //        {
    //            byte index = (byte)(crc ^ bytes[i]);
    //            crc = (ushort)((crc >> 8) ^ table[index]);
    //        }
    //        return crc;
    //    }

    //    public byte[] ComputeChecksumBytes(params byte[] bytes)
    //    {
    //        ushort crc = ComputeChecksum(bytes);
    //        return BitConverter.GetBytes(crc);
    //    }

    //    public CRCCCIT(Crc16Mode mode)
    //    {
    //        ushort polynomial = (ushort)mode;
    //        ushort value;
    //        ushort temp;
    //        for (ushort i = 0; i < table.Length; ++i)
    //        {
    //            value = 0;
    //            temp = i;
    //            for (byte j = 0; j < 8; ++j)
    //            {
    //                if (((value ^ temp) & 0x0001) != 0)
    //                {
    //                    value = (ushort)((value >> 1) ^ polynomial);
    //                }
    //                else
    //                {
    //                    value >>= 1;
    //                }
    //                temp >>= 1;
    //            }
    //            table[i] = value;
    //        }
    //    }
    //}
}