using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityUtils
{
    public class ByteUtil
    {
        // 获取16位的CRC计算值
        public static byte[] Test()//将BYTE数组里的数据转换为16进制，参数是BYTE数组，和数组里的数据长度
        {
            //模拟了C0 03四个报警测试开关量，高字节高两位，和低字节低两位1100 0000  0000 0011
            string data = string.Format(@"02 10 00 00 00 78 F0 00 01 00 67 00 01 00 3A 00 39 00 00 00 
                                            03 00 01 00 01 00 01 00 00 00 00 00 00 01 D7 00 AF 00 00 02
                                            37 01 7A 00 4F 05 65 00 15 00 03 00 03 00 00 0F 9D 00 2C 00
                                            37 00 42 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 17
                                            1F 19 7E 19 DD 17 CD 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 0F 9D 00 00 00 00 00 05 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 E3 B7");
            return TextToBytes(data);
        }

        // 字符串转化为16进制函数
        public static string BytesToText(byte[] str, int len)
        {
            string hex = "";
            string s;
            int asc;
            for (int i = 0; i < len; i++)
            {
                s = "";
                asc = str[i];
                //hex = hex + System.Convert.ToString(asc,16);
                s = System.Convert.ToString(asc, 16);
                for (int j = 0; j < s.Length; j++)
                {
                    if (s.Length == 1)
                        hex = hex + '0';
                    if (s[j] == 'a')
                        hex = hex + 'A';
                    else if (s[j] == 'b')
                        hex = hex + 'B';
                    else if (s[j] == 'c')
                        hex = hex + 'C';
                    else if (s[j] == 'd')
                        hex = hex + 'D';
                    else if (s[j] == 'e')
                        hex = hex + 'E';
                    else if (s[j] == 'f')
                        hex = hex + 'F';
                    else
                        hex = hex + s[j];
                }
                if (i < (len - 1))
                    hex = hex + " ";
            }
            return hex;
        }
        // 将16进制字符串转换放到BYTE数组里，并返回转换后的数据长度，参数是16进制字符串和字节数组 
        public static byte[] TextToBytes(string hex)
        {
            hex = hex.Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "").Replace("\t", "");
            int asc = 0;
            int len = 0;
            //string s = "";
            string s = hex;
            string ss = "";
            //for (int i = 0; i < hex.Length; i++)
            //{
            //    if (hex[i] != ' ')
            //    {
            //        s = s + hex[i];
            //        asc = (byte)hex[i];
            //        if ((asc < 48) || ((asc > 57) && (asc < 65)) || ((asc > 70) && (asc < 97)) || (asc > 102))
            //        {
            //            return new byte[0];
            //        }
            //    }
            //}
            for (int i = 0; i < hex.Length; i++)
            {
                asc = (byte)hex[i];
                if ((asc < 48) || ((asc > 57) && (asc < 65)) || ((asc > 70) && (asc < 97)) || (asc > 102))
                {
                    return new byte[0];
                }
            }
            if (s.Length % 2 == 0)
            {
                byte[] buffer = new byte[s.Length / 2];
                for (int j = 0; j < (s.Length - 1); j = j + 2)
                {
                    ss = "";
                    ss = ss + s[j] + s[j + 1];
                    buffer[len] = (byte)(Convert.ToInt32(ss, 16));
                    len++;
                }
                return buffer;

            }
            return new byte[0];
        }

        // 将数据部分字节数组，转成每两个形成一个值，要求传进来的是高字节在前面的字节数组
        public static ushort ToUshort(byte[] buffer, int index)
        {
            byte[] reversed = new byte[2];
            reversed[0] = buffer[index + 1];
            reversed[1] = buffer[index];
            return BitConverter.ToUInt16(reversed, 0);
        }
        public static ushort[] ToUshorts(byte[] buffer, int index, int length)
        {
            // 必须位偶数
            if ((length % 2) != 0)
                return new ushort[0];

            ushort[] ushorts = new ushort[length / 2];

            for (int i = index; i < index + length; i += 2)
            {
                ushorts[(i - index) / 2] = ToUshort(buffer, i);
            }

            return ushorts;
        }

        //获取modbus值
        public static bool GetBoolValue(byte[] buffer, string address)
        {
            string[] addrStrs = address.Split('.');
            int addr = DataUtil.ToInt(addrStrs[0]);
            ushort value = ToUshort(buffer, addr);
            int byteID = DataUtil.ToInt(addrStrs[1]);
            int addrByte = DataUtil.ToInt(addrStrs[2]);
            return GetBoolValue(value, byteID, addrByte);  
            //switch (byteID)
            //{
            //    case 1:
            //        {
            //            return (ushort)((value & (1 << 8 + addrByte)) >> (8 + addrByte)) == 0x01 ? true : false;
            //        }
            //    case 0:
            //        {
            //            return (ushort)((value & (1 << addrByte)) >> (addrByte)) == 0x01 ? true : false;
            //        }
            //}
            //return false;
        }
        public static bool GetBoolValue(ushort dataSourceValue, int byteID, int addrByte)
        {
            try
            {
                switch (byteID)
                {
                    case 1:
                        {
                            return (ushort)((dataSourceValue & (1 << 8 + addrByte)) >> (8 + addrByte)) == 0x01 ? true : false;
                        }
                    case 0:
                        {
                            return (ushort)((dataSourceValue & (1 << addrByte)) >> (addrByte)) == 0x01 ? true : false;
                        }
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static ushort GetUshortValue(byte[] buffer, int address)
        {
            return ToUshort(buffer, address);
        }
        public static int Get32BitValue(byte[] buffer, int address)
        {
            ushort high = ToUshort(buffer, address);
            ushort low = ToUshort(buffer, address + 2);
            // 返回32位的数字
            return (high << 16) + low;
        }
        public static long Get48BitValue(byte[] buffer, int address)
        {
            ushort high = ToUshort(buffer, address);
            ushort middle = ToUshort(buffer, address + 2);
            ushort low = ToUshort(buffer, address + 4);
            // 返回48位的数字
            return (high << 32) + (middle << 16) + low;
        }
        public static ushort Get24BitValue(byte[] buffer, int address)
        {
            byte low, middle, high;
            // 根据地址去低字节，在去掉高四位
            low = Convert.ToByte((buffer[address + 1]) & 0x0F);
            middle = Convert.ToByte(buffer[address + 3] & 0x0F);
            high = Convert.ToByte(buffer[address + 5] & 0x0F);
            // 返回32位的数字
            ushort high1 = (ushort)(high << 8);
            ushort middle1 = (ushort)(middle << 4);
            ushort low1 = (ushort)(low << 0);
            return (ushort)(high1 + middle1 + low1);
        }

        // 返回两个字节的数组，高位在前，低位在后
        public static byte[] ToBytes(ushort data)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)((0xff00 & data) >> 8);//拿到高位
            buffer[1] = (byte)(0xff & data);//拿到低位
            return buffer;
        }
        public static byte[] ToBytes(ushort[] datas)
        {
            List<byte> buffer = new List<byte>();
            foreach (ushort us in datas)
                buffer.AddRange(ToBytes(us));
            return buffer.ToArray();
        }

        // 源数据添加一个字节对象返回长度加1的字节数据 additionData往源数据添加的字节对象
        public static byte[] BytesAddOne(byte[] sourceDatas, byte additionData)
        {
            byte[] TargetDatas = new byte[sourceDatas.Length + 1];
            Buffer.BlockCopy(sourceDatas, 0, TargetDatas, 0, sourceDatas.Length);
            TargetDatas[sourceDatas.Length] = additionData;
            return TargetDatas;
        }

        //通过起始地址，寄存器数量，值数组形成地址和值对应的字典
        public static Dictionary<ushort, ushort> ToDicValue(ushort beginAddress, ushort registerCount, ushort[] values)
        {
            Dictionary<ushort, ushort> result = new Dictionary<ushort, ushort>();
            if (registerCount != values.Length)
                return null;
            for (ushort i = 0; i < registerCount; i++)
            {
                result.Add(beginAddress, values[i]);
                beginAddress += 1;//每16才会有一个新地址
            }
            return result;
        }
        // 拿到地址-值对应字典 
        public static Dictionary<ushort, ushort> ToDicValue(ushort beginAddress, ushort registerCount, byte[] buffer)
        {
            if (buffer.Length % 2 != 0)
                return null;//modbus地址是双字节表示一个寄存器
            ushort[] values = ToUshorts(buffer, 0, buffer.Length);
            return ToDicValue(beginAddress, registerCount, values);
        }

        public static string RemoveRegisterInfoAddr(string addr)
        {
            return addr.Trim().Remove(0, 1);
        }
        public static int GetRegisterInfoAddr(string addr)
        {
            return Int32.Parse(addr.Trim()[0].ToString());
        }
        public static int GetBufferIndex(string addr)
        {
            int intAddr = DataUtil.ToInt(addr.Trim().Split('.')[0].ToString().Remove(0, 1));
            //每个两个字节代表一个地址
            return intAddr * 2;
            //int addr=DataConvert.ToInt(addr.Trim().so. Remove(0, 1))
            //return ;
        }

        // socket通信发送结构
        public static byte[] ToSerializeBuffer<T>(T t)
        {
            string source = JsonConvert.SerializeObject(t);
            return Encoding.UTF8.GetBytes(source);
        }
        public static string ToSerializeObject<T>(T t)
        {
            return JsonConvert.SerializeObject(t);
        }
        public static byte[] ToSerializeBuffer(string source)
        {
            return Encoding.UTF8.GetBytes(source);
        }
        public static T ToDeserializeObject<T>(string JsonData)
        {
            return JsonConvert.DeserializeObject<T>(JsonData);
        }
        public static T ToDeserializeObject<T>(byte[] buffer)
        {
            string source = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            return ToDeserializeObject<T>(source);
        }
        public static string ToDeserializeString(byte[] data)
        {
            return Encoding.UTF8.GetString(data, 0, data.Length); ;
        }
        public static T DeepClone<T>(T t)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(t));
        }

        /// <summary>
        /// 加上换行符
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string AddLineBreak(string data)
        {
            return data + "\r\n";
        }
    }
}
