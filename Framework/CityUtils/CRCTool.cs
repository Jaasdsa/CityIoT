using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityUtils
{
    public static class CRCTool
    {
        //*********数据类型定义比较危险暂时保留不用***********//
        private static uint CRC16_modbus(byte[] modbusData)
        {
            uint i, j;
            uint crc16 = 0xFFFF;
            for (i = 0; i < modbusData.Length; i++)
            {
                crc16 ^= modbusData[i];
                for (j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1)
                        crc16 = crc16 >> 1 ^ 0xA001;
                    else
                        crc16 = crc16 >> 1;
                }
            }
            return crc16;
        }

        // 返回CRC数值，高位在前，低位在后
        static ushort CRC_16(byte[] data)
        {
            uint IX, IY;
            ushort crc = 0xFFFF;//set all 1

            int len = data.Length;
            if (len <= 0)
                crc = 0;
            else
            {
                len--;
                for (IX = 0; IX <= len; IX++)
                {
                    crc = (ushort)(crc ^ (data[IX]));
                    for (IY = 0; IY <= 7; IY++)
                    {
                        if ((crc & 1) != 0)
                            crc = (ushort)((crc >> 1) ^ 0xA001);
                        else
                            crc = (ushort)(crc >> 1); //
                    }
                }
            }

            byte buf1 = (byte)((crc & 0xff00) >> 8);//高位置
            byte buf2 = (byte)(crc & 0x00ff); //低位置

            crc = (ushort)(buf1 << 8);
            crc += buf2;
            return crc;

        }

        // 返回低位在前，高位在后的长度2为byte的CRC数组
        public static byte[] GetCRCByte(ushort crc)
        {
            byte[] crcByte = new byte[2];
            crcByte[1] = (byte)((crc & 0xff00) >> 8);//高位置
            crcByte[0] = (byte)(crc & 0x00ff); //低位置
            return crcByte;
        }

        // 让数组增加两个字节带上CRC校验，低位在前，高位在后
        public static byte[] AddCRC(byte[] buffer)
        {
            byte[] data = new byte[buffer.Length + 2];
            ushort crc = CRC_16(buffer);
            byte[] crcByte = GetCRCByte(crc);
            Buffer.BlockCopy(buffer, 0, data, 0, buffer.Length);//这种方法仅适用于字节数组
            Buffer.BlockCopy(crcByte, 0, data, buffer.Length, crcByte.Length);
            return data;
        }

        // 检验最后两个字节是CRC的数组
        public static bool CheckCRC16Data(byte[] data)
        {
            if (data.Length < 2)
                return false;
            byte[] buffer = new byte[data.Length - 2];
            Buffer.BlockCopy(data, 0, buffer, 0, data.Length - 2);
            byte[] bufferAddCRC = CRCTool.AddCRC(buffer);
            if (bufferAddCRC.Length == data.Length)
            {
                if (data[data.Length - 2] == bufferAddCRC[bufferAddCRC.Length - 2] && data[data.Length - 1] == bufferAddCRC[bufferAddCRC.Length - 1])
                    return true;
            }
            return false;

        }
    }
}
