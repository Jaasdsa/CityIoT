using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace DTUPumpDataService 
{
    //结构定义
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GPRS_DATA_RECORD
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string m_userid;             // 终端模块号码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string m_recv_date;          // 接收到数据包的时间
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)] //这里做了修改，转换时由ByValTStr变为ByValArray类型，
        public byte[] m_data_buf;           // 存储接收到的数据
        public ushort m_data_len;           // 接收到的数据包长度
        public byte m_data_type;            // 接收到的数据包类型
        public void Initialize()            // 初始化byte[]的字段
        {
            m_data_buf = new byte[1024];
        }
        //UnmanagedType.LPStr
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GPRS_USER_INFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string m_userid;             // 终端模块号码	
        public uint m_sin_addr;             // 终端模块进入 Internet 的代理主机IP地址
        public ushort m_sin_port;           // 终端模块进入 Internet 的代理主机IP端口
        public uint m_local_addr;           // 终端模块在移动网内 IP 地址
        public ushort m_local_port;         // 终端模块在移动网内 IP 端口
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string m_logon_date;         // 终端模块登录时间
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]   // 这里做了修改，转换时由ByValTStr变为ByValArray类型，
        public byte[] m_update_time;        // 终端用户更新时间
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        //	public string	m_update_time;
        public byte m_status;               // 终端模块状态, 1 在线 0 不在线
        public void Initialize()            //初始化byte[]的字段
        {
            m_update_time = new byte[20];
        }
    }
    class HDDTUAPI
    {
        //定义接口函数		
        //启动服务
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int start_gprs_server(
            IntPtr hWnd,
            int wMsg,
            int nServerPort,
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess);
        //启动服务
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int start_net_service(
            IntPtr hWnd,
            int wMsg,
            int nServerPort,
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess);

        //停止服务
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int stop_gprs_server(
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess);
        //停止服务
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int stop_net_service(
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess);

        //读取数据
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int do_read_proc(
            ref GPRS_DATA_RECORD recdPtr,
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess,
            bool reply);

        //发送数据
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int do_send_user_data(
            [MarshalAs(UnmanagedType.LPStr)]
            string userid,
            //[MarshalAs(UnmanagedType.LPStr)]
            //string data,
            byte[] data,
            int len,
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess);

        //获取终端信息
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int get_user_info(
            [MarshalAs(UnmanagedType.LPStr)]
            string userid,
            ref GPRS_USER_INFO infoPtr);


        //设置服务模式
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int SetWorkMode(int nWorkMode);

        //取消阻塞读取
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern void cancel_read_block();

        //使某个DTU下线
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int do_close_one_user(
            [MarshalAs(UnmanagedType.LPStr)]
            string userid,
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess);

        //使所有DTU下线
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int do_close_all_user(
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess);

        //使某个DTU下线
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int do_close_one_user2(
            [MarshalAs(UnmanagedType.LPStr)]
            string userid,
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess);

        //使所有DTU下线
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int do_close_all_user2(
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder mess);

        //设置服务类型
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int SelectProtocol(int nProtocol);

        //指定服务IP
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern void SetCustomIP(int IP);

        //获得最大DTU连接数量
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern uint get_max_user_amount();

        //获取终端信息
        [DllImport(@".\dll\wcomm_dll.dll")]
        public static extern int get_user_at(uint index, ref GPRS_USER_INFO infoPtr);

        //定义一些SOCKET API函数
        [DllImport("Ws2_32.dll")]
        public static extern Int32 inet_addr(string ip);
        [DllImport("Ws2_32.dll")]
        public static extern string inet_ntoa(uint ip);
        [DllImport("Ws2_32.dll")]
        public static extern uint htonl(uint ip);
        [DllImport("Ws2_32.dll")]
        public static extern uint ntohl(uint ip);
        [DllImport("Ws2_32.dll")]
        public static extern ushort htons(ushort ip);
        [DllImport("Ws2_32.dll")]
        public static extern ushort ntohs(ushort ip);

        public const int WM_DTU = 0x400 + 100;


        //DTU日期转换辅助方法
        public static DateTime ConvertToDateTime(byte[] timeBuffer)
        {
            long t_update = (long)(timeBuffer[0])
                                + (long)(timeBuffer[1]) * 256
                                + (long)(timeBuffer[2]) * 256 * 256
                                + (long)(timeBuffer[3]) * 256 * 256 * 256
                                + 3600 * 8;
            double m_update;
            m_update = (t_update * 10000000) / (3600 * 24);
            m_update = m_update / 10000000 + 25569;

            return DateTime.FromOADate(m_update);
        }
        public static string ConvertToDateString(byte[] timeBuffer)
        {
            return ConvertToDateTime(timeBuffer).ToString();
        }
    }
}
