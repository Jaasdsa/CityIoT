using CityUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTUPumpDataService 
{
    // RTU读值工具者
    public class ModbusRTUReader
    {
        public PointType Type;
        public string Address;
        public int Length;
        public double Scale;
        public int offsetAddress; // 地址便宜量

        public object Value;
        public ValueState state;
        public string mess;


        public ModbusRTUReader Read(byte[] buffer)
        {
            if(!this.Check(out string errMsg))
            {
                MakeFail(errMsg);
                return this;
            }
            int DataKind = GetInfoAddr(this.Address);
            switch (DataKind)
            {
                case 4://保持型寄存器
                    this.ReadKeepValue(buffer);
                    break;
                default:
                    MakeFail("目前只支持读保持性寄存器");
                    break;
            }
            return this;
        }

        // 读取解析协议为moubusRTU保持型寄存器的值
        private void ReadKeepValue(byte[] buffer)
        {
            switch (this.Type)
            {
                case PointType.Ushort:
                    this.ReadIntKeepValue(buffer);
                    break;
                case PointType.Real:
                    this.ReadFloatKeepValue(buffer);
                    break;
                case PointType.Bool:
                    this.ReadBoolKeepValue(buffer);
                    break;
                case PointType.Group:
                    this.ReadGroupKeepValue(buffer);
                    break;
                default:
                    this.mess = "未知的数据类型";
                    this.state = ValueState.Fail;
                    break;
            }
        }
        private void ReadIntKeepValue(byte[] buffer)
        {
            if (this.Scale != 1)
            {
                MakeFail("地址：" + this.Address + "整数型的倍率必须为1");
                return;
            }
            int index = GetSingleBufferIndex(this.Address);
            if (!this.CheckAddrIndex(index, buffer.Length, out string errMsg))
            {
                MakeFail(errMsg);
                return;
            }
            switch (this.Length)
            {
                case 16:
                    {
                        this.Value = DataUtil.ToDoubleByScale(ByteUtil.GetUshortValue(buffer, index), 1);
                        this.state = ValueState.Success;
                        return;
                    }
                case 32:
                    {
                        this.Value = DataUtil.ToDoubleByScale(ByteUtil.Get32BitValue(buffer, index), 1);
                        this.state = ValueState.Success;
                        return;
                    }
                case 48:
                    {
                        this.Value = DataUtil.ToDoubleByScale(ByteUtil.Get48BitValue(buffer, index), 1);
                        this.state = ValueState.Success;
                        return;
                    }
                default:
                    MakeFail("地址：" + this.Address + "未知的数据长度");
                    break;
            }
        }
        private void ReadFloatKeepValue(byte[] buffer)
        {
            int index = GetSingleBufferIndex(this.Address);
            if (!this.CheckAddrIndex(index, buffer.Length, out string errMsg))
            {
                MakeFail(errMsg);
                return;
            }
            switch (this.Length)
            {
                case 16:
                    {
                        this.Value = DataUtil.ToDoubleByScale(ByteUtil.GetUshortValue(buffer, index), this.Scale);
                        this.state = ValueState.Success;
                        return;
                    }
                case 32:
                    {
                        this.Value = DataUtil.ToDoubleByScale(ByteUtil.Get32BitValue(buffer, index), this.Scale);
                        this.state = ValueState.Success;
                        return;
                    }
                case 48:
                    {
                        this.Value = DataUtil.ToDoubleByScale(ByteUtil.Get48BitValue(buffer, index), this.Scale);
                        this.state = ValueState.Success;
                        return;
                    }
                default:
                    this.mess = "未知的数据长度";
                    this.state = ValueState.Fail;
                    break;
            }
        }
        private void ReadBoolKeepValue(byte[] buffer)
        {
            int index = GetSingleBufferIndex(this.Address);
            if (!this.CheckAddrIndex(index, buffer.Length, out string errMsg))
            {
                MakeFail(errMsg);
                return;
            }
            // 地址加上偏移量
            string addr = index.ToString() + this.Address.Substring(this.Address.IndexOf('.'));
            this.Value = ByteUtil.GetBoolValue(buffer, addr) == true ? 1 : 0;
            this.state = ValueState.Success;
            return;
        }
        private void ReadGroupKeepValue(byte[] buffer)
        {
            string[] addrs = this.Address.Split(';');
            if(addrs.Length!=3)
            {
                MakeFail("地址：" + this.Address + "组合地址非法,必须为三个非开关型的点位");
                return;
            }
            double lowVal = 0;
            double middleVal = 0;
            double highVal =0;
            // 地址循环检查
            int i = 0;
            foreach (string addr in addrs)
            {
                int index = GetSingleBufferIndex(addr);
                if (!this.CheckAddrIndex(index, buffer.Length, out string errMsg))
                {
                    MakeFail(errMsg);
                    return;
                }
                if (i == 0)
                {
                    lowVal = DataUtil.ToDoubleByScale(ByteUtil.GetUshortValue(buffer, index), 1);
                }
                else if (i == 1)
                {
                    middleVal = DataUtil.ToDoubleByScale(ByteUtil.GetUshortValue(buffer, index), 1);
                }
                else if (i == 2)
                {
                    highVal = DataUtil.ToDoubleByScale(ByteUtil.GetUshortValue(buffer, index), 1);
                }
                i++;
            }
            // 注意地址存放顺序，低在前，中在中，高在后
            this.Value = DataUtil.ToDoubleByScale(DataUtil.ToDouble(lowVal + middleVal * 10000 + highVal * 100000000), this.Scale);
            this.state = ValueState.Success;
        }

        // 信息地址方法
        private string RemoveInfoAddr(string addr)
        {
            return addr.Trim().Remove(0, 1);
        }
        private int GetInfoAddr(string addr)
        {
            return Int32.Parse(addr.Trim()[0].ToString());
        }
        private int GetSingleBufferIndex(string singleAddr)
        {
            int intAddr = DataUtil.ToInt(singleAddr.Trim().Split('.')[0].ToString().Remove(0, 1))+this.offsetAddress;
            //每个两个字节代表一个地址
            return intAddr * 2;
        }

        //寄存器自我检查
        private bool Check(out string errMsg)
        {
            errMsg = "";
            if (string.IsNullOrEmpty(this.Address))
            {
                errMsg = "寄存器地址不能为空";
                return false;
            }
            if (!DataUtil.CheckScale(this.Scale))
            {
                errMsg = "寄存器地址:" + this.Address + "倍率错误";
                return false;
            }
            switch (this.Type)
            {
                case PointType.Bool:
                    {
                        if (this.Length != 1)
                        {
                            errMsg = "寄存器地址:" + this.Address + "寄存器长度错误,开关型只能为1位";
                            return false;
                        }
                        string[] addrs = this.Address.Trim().Split('.');
                        if (addrs.Length != 3)
                        {
                            errMsg = "寄存器地址:" + this.Address + "错误";
                            return false;
                        }
                        int[] addrsInt = new int[3];
                        try
                        {
                            //地址需为数字
                            addrsInt[0] = Convert.ToInt32(addrs[0]);
                            addrsInt[1] = Convert.ToInt32(addrs[1]);
                            addrsInt[2] = Convert.ToInt32(addrs[2]);
                        }
                        catch
                        {
                            errMsg = "寄存器地址:" + this.Address + "错误,只能为数字组成";
                            return false;
                        }
                        if (addrsInt[1] != 0 && addrsInt[1] != 1)
                        {
                            errMsg = "寄存器地址:" + this.Address + "错误,第二位必须0或1";
                            return false;
                        }
                        if (addrsInt[2] > 7 || addrsInt[2] < 0)
                        {
                            errMsg = "寄存器地址:" + this.Address + "错误,第三位必须0-7";
                            return false;
                        }
                    }
                    break;
                case PointType.Real:
                case PointType.Ushort:
                    {
                        switch (this.Length)
                        {
                            case 16:
                            case 32:
                            case 48:
                                {
                                    string[] addr = this.Address.Trim().Split('.');
                                    if (addr.Length > 1)
                                    {
                                        errMsg = "寄存器地址:" + this.Address + "寄存器地址错误,短整型和浮点型地址有非法字符"+"“.”";
                                        return false;
                                    }
                                    if (DataUtil.ToInt(addr[0]) == 0)
                                    {
                                        errMsg = "寄存器地址:" + this.Address + "寄存器地址错误,只能为数字组成";
                                        return false;
                                    }
                                }
                                break;
                            default:
                                {
                                    errMsg = "寄存器地址:" + this.Address + "寄存器长度非法";
                                    return false;
                                }                                
                        }
                        break;
                    }
                case PointType.Group:
                    {
                        string[] addrSum = this.Address.Trim().Split(';');
                        if (addrSum.Length <1)
                        {
                            errMsg = "组合型地址需多个地址以【;】隔开合成组合地址";
                            return false;
                        }
                        foreach (string addrSingle in addrSum)
                        {
                            string[] addr = addrSingle.Trim().Split('.');
                            if (addr.Length > 1)
                            {
                                errMsg = "寄存器地址:" + this.Address + "寄存器地址错误，合成地址含有非法字符“.”";
                                return false;
                            }
                            if (DataUtil.ToInt(addr[0]) == 0)
                            {
                                errMsg = "寄存器地址:" + this.Address + "寄存器地址错误,合成地址中每个子地址必须为数字";
                                return false;
                            }
                        }
                    }
                    break;
                default:
                    {
                        errMsg = "寄存器地址:" + this.Address + "数据类型未知";
                        return false;
                    }
            }
            return true;
        }
        private void MakeFail(string errMsg)
        {
            this.mess = errMsg;
            this.Value = 0;
            this.state = ValueState.Fail;
        }
        private bool CheckAddrIndex(int index,int bufferLength,out string errMsg)
        {
                errMsg = "";
            if (bufferLength == 0)
            {
                errMsg = "地址：" + this.Address + "数据缓存区长度为零";
                return false;
            }
            if(bufferLength % 2 != 0)
            {
                errMsg = "地址：" + this.Address + "数据缓存区长度为奇数";
                return false;
            }
            if (this.Length == 1)
            {
                if (bufferLength < (index + 2))
                {
                    errMsg = "地址：" + this.Address + "超过接受到数据长度";
                    return false;
                }
            }
            if (this.Length == 16)
            {
                if (bufferLength < (index + 2))
                {
                    errMsg = "地址：" + this.Address + "超过接受到数据长度";
                    return false;
                }
            }
            if (this.Length == 32)
            {
                if (bufferLength < (index + 4))
                {
                    errMsg = "地址：" + this.Address + "超过接受到数据长度";
                    return false;
                }
            }
            if (this.Length == 48)
            {
                if (bufferLength < (index + 6))
                {
                    errMsg = "地址：" + this.Address + "超过接受到数据长度";
                    return false;
                }
            }
            return true;
        }
    }

    class ModbusRTU
    {
        // moubus数据校验
        public static bool CheckData(byte[] buffer,out string errMsg)
        {
            errMsg = "";
            //  CRC16 校验
            if (!CRCTool.CheckCRC16Data(buffer))
            {
                errMsg="modbus数据CRC16校验失败";
                return false;
            }
            if (buffer.Length < 5)
            {
                errMsg = "modbus数据长度太短已定义为异常数据";
                return false;
            }
            return true;
        }        

        public static bool Read(byte[] buffer,PointType type,string plcAddr,int length,float scale,out float Value,out string errMsg)
        {
            Value = 0;
            errMsg ="";
            switch (type)
            {
                case PointType.Bool:
                    {

                        return true;
                    }
                case PointType.Real:
                    {
                        return true;
                    }
                case PointType.Ushort:
                    {
                        return true;
                    }
                default:
                    return false;
            }
        }

        public enum RequestType
        {
            Read,
            Write,
            WriteMulti,
        }
        // modbus请求结构类
        public class Request
        {
            public RequestType Type;
            public byte PlcAddress;
            public ushort RegisterBeginAddress;
            public ushort RegisterCount;
            public ushort RegisterValue;
            public byte DataLength;
            public ushort[] Values;
            public ushort CRC;

            //根据字节数据拿到请求对象
            public static Request FromBytes(byte[] bytes)
            {
                try
                {
                    switch (bytes[1])
                    {
                        case 0x03:
                            {
                                Request request = new Request();
                                request.PlcAddress = bytes[0];
                                request.Type = RequestType.Read;
                                request.RegisterBeginAddress = ByteUtil.ToUshort(bytes, 2);
                                request.RegisterCount = ByteUtil.ToUshort(bytes, 4);
                                request.CRC = BitConverter.ToUInt16(bytes, 6);  // CRC 无需反转高低位
                                return request;
                            }
                        case 0x06:
                            {
                                Request request = new Request();
                                request.PlcAddress = bytes[0];
                                request.Type = RequestType.Write;
                                request.RegisterBeginAddress = ByteUtil.ToUshort(bytes, 2);
                                request.RegisterValue = ByteUtil.ToUshort(bytes, 4);
                                request.CRC = BitConverter.ToUInt16(bytes, 6);  // CRC 无需反转高低位
                                return request;
                            }
                        case 0x10:
                            {
                                Request request = new Request();
                                request.PlcAddress = bytes[0];
                                request.Type = RequestType.WriteMulti;
                                request.RegisterBeginAddress = ByteUtil.ToUshort(bytes, 2);
                                request.RegisterCount = ByteUtil.ToUshort(bytes, 4);
                                request.DataLength = bytes[6];
                                request.Values = ByteUtil.ToUshorts(bytes, 7, request.DataLength);
                                request.CRC = BitConverter.ToUInt16(bytes, bytes.Length - 2);  // CRC 无需反转高低位
                                return request;
                                //写多指测试
                                // ModbusRTURequest r = ModbusRTURequest.FromBytes(new byte[] { 0x01, 0x10, 0x00, 0x01, 0x00, 0x02, 0x04, 0x00, 0x02, 0x00, 0x03, 0x00, 0x00 });

                            }
                    }
                }
                catch (Exception e)
                {
                    TraceManager.AppendErrMsg("根据字节数组转modbus请求对象失败：" + e.Message);
                    return null;
                }
                return null;
            }

            //根据请求对象转成发送的字节数组
            public static byte[] ToBytes(Request request, out string errMsg)
            {
                errMsg = "";
                try
                {
                    switch (request.Type)
                    {
                        case RequestType.Read:
                            {
                                // 8 个字节
                                byte[] buffer = new byte[2];
                                buffer[0] = request.PlcAddress;
                                buffer[1] = 0x03;
                                buffer = buffer.Concat(ByteUtil.ToBytes(request.RegisterBeginAddress))  // 2,3
                                    .Concat(ByteUtil.ToBytes(request.RegisterCount)).ToArray();  // 4,5

                                return CRCTool.AddCRC(buffer);  // 6,7
                                                                //测试代码
                                                                //ModbusRTURequest m = new ModbusRTURequest();
                                                                //m.PlcAddress = 0x01;
                                                                //m.Type = RequestType.Read;
                                                                //m.RegisterBeginAddress = 0x0002;
                                                                //m.RegisterCount = 0x00001;
                                                                //byte[] aa = ModbusRTURequest.ToBytes(m);
                            }
                        case RequestType.Write:
                            {
                                // 8 个字节
                                byte[] buffer = new byte[2];
                                buffer[0] = request.PlcAddress;
                                buffer[1] = 0x06;
                                buffer = buffer.Concat(ByteUtil.ToBytes(request.RegisterBeginAddress))  // 2,3
                                    .Concat(ByteUtil.ToBytes(request.RegisterValue)).ToArray();  // 4,5

                                return CRCTool.AddCRC(buffer);  // 6,7
                                                                //// 测试代码
                                                                //ModbusRTURequest m = new ModbusRTURequest();
                                                                //m.PlcAddress = 0x01;
                                                                //m.Type = RequestType.Write;
                                                                //m.RegisterBeginAddress = 0x0003;
                                                                //m.RegisterValue = 0x003A;
                                                                //byte[] aa = ModbusRTURequest.ToBytes(m);
                            }
                        case RequestType.WriteMulti:
                            {
                                // 8+1+N*2个字节
                                byte[] buffer = new byte[2];
                                buffer[0] = request.PlcAddress;
                                buffer[1] = 0x10;
                                buffer = buffer.Concat(ByteUtil.ToBytes(request.RegisterBeginAddress))  // 2,3
                                    .Concat(ByteUtil.ToBytes(request.RegisterCount)).ToArray();  // 4,5
                                buffer = ByteUtil.BytesAddOne(buffer, request.DataLength); //6
                                buffer = buffer.Concat(ByteUtil.ToBytes(request.Values)).ToArray(); //7---(7+n*2)-1
                                return CRCTool.AddCRC(buffer);
                                //// 测试代码  01 10 0003 0002 04 0001 0002 63 BB
                                //ModbusRTURequest m = new ModbusRTURequest();
                                //m.PlcAddress = 0x01;
                                //m.Type = RequestType.WriteMulti;
                                //m.RegisterBeginAddress = 0x0003;
                                //m.RegisterCount = 0x0002;
                                //m.DataLength = 0x04;
                                //m.WriteMultiValues = new ushort[2] { 0x01, 0x02 };
                                //m.RegisterValue = 0x003A;
                                //byte[] aa = ModbusRTURequest.ToBytes(m);
                            }
                    }
                }
                catch (Exception e)
                {
                    errMsg = "根据modbus请求对象转字节数组失败：" + e.Message;
                    return null;
                }
                errMsg = "根据modbus请求对象转字节数组失败";
                return null;
            }

        }


        // modbus应答类型
        public enum ResponseType
        {
            Read,
            Write,
            WriteMulti,
            ReadError,
            WriteError,
        }
        // modbus应答结构类
        public class ModbusRTUResponse
        {
            public ResponseType Type;
            public byte PlcAddress;
            public ushort RegisterBeginAddress;
            public ushort RegisterCount;
            public ushort RegisterValue;
            public byte DataLength;
            public ushort[] Values;
            public ushort CRC;
            public string errMsg;

            //根据字节数组转成modbus响应上来的对象
            public static ModbusRTUResponse FromBytes(byte[] bytes, out string errMsg)
            {
                errMsg = "";
                try
                {
                    switch (bytes[1])
                    {
                        case 0x03:
                            {
                                if (bytes.Length != 7)
                                    return null;
                                //读寄存器回应
                                ModbusRTUResponse response = new ModbusRTUResponse();
                                response.PlcAddress = bytes[0];
                                response.Type = ResponseType.Read;
                                // 含义：寄存器的个数*2
                                response.DataLength = bytes[2];
                                response.Values = ByteUtil.ToUshorts(bytes, 3, response.DataLength);
                                response.CRC = BitConverter.ToUInt16(bytes, 5);  // CRC 无需反转高低位
                                if (response.DataLength == 0x02)
                                {
                                    // 针对单个寄存器
                                    response.RegisterValue = response.Values[0];
                                    response.RegisterCount = 0x01;
                                }
                                return response;
                            }
                        case 0x10:
                            {
                                if (bytes.Length != 8)
                                    return null;
                                // 目前只有单个寄存器写入
                                ModbusRTUResponse response = new ModbusRTUResponse();
                                response.PlcAddress = bytes[0];
                                response.Type = ResponseType.Write;
                                response.RegisterBeginAddress = ByteUtil.ToUshort(bytes, 2);
                                response.RegisterValue = ByteUtil.ToUshort(bytes, 4);
                                response.CRC = BitConverter.ToUInt16(bytes, 6);  // CRC 无需反转高低位
                                return response;
                            }
                        case 0x83:
                            {
                                if (bytes.Length != 5)
                                    return null;
                                // 读单个寄存器请求出错
                                ModbusRTUResponse response = new ModbusRTUResponse();
                                response.PlcAddress = bytes[0];
                                response.Type = ResponseType.ReadError;
                                response.errMsg = "读寄存器请求出错--" + GetErrMSg(bytes[2]);
                                response.CRC = BitConverter.ToUInt16(bytes, 3);  // CRC 无需反转高低位
                                return response;
                            }
                        case 0x86:
                            {
                                if (bytes.Length != 5)
                                    return null;
                                // 写单个寄存器请求出错
                                ModbusRTUResponse response = new ModbusRTUResponse();
                                response.PlcAddress = bytes[0];
                                response.Type = ResponseType.WriteError;
                                response.errMsg = "写寄存器请求出错--" + GetErrMSg(bytes[2]);
                                response.CRC = BitConverter.ToUInt16(bytes, 3);  // CRC 无需反转高低位
                                return response;
                            }
                    }
                }
                catch (Exception e)
                {
                    errMsg = e.Message;
                    return null;
                }
                errMsg = "字节数组转modbus回应对象出现未知的功能码";
                return null;
            }

            //根据modbus响应的对象转成字节数组
            public static byte[] ToBytes(ModbusRTUResponse request)
            {
                return new byte[0];
            }

            private static string GetErrMSg(byte code)
            {
                switch (code)
                {
                    case 0x01:
                        return "不支持该功能码";
                    case 0x02:
                        return "越界";
                    case 0x03:
                        return "寄存器数量超出范围";
                    case 0x04:
                        return "读写错误";
                }
                return "未知的错误码";
            }
        }

    }



}