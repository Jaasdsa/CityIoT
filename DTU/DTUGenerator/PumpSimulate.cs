using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DTUGenerator
{

    public partial class PumpSimulate : Form
    {
        private LoginInfo loginInfo;
        private Login loginForm;
        private string curTitle;
        private bool IsExit { get; set; } = true;

        private UDPClient client;
        private UDPClientManager clienManager;
        private bool IsConnected { get; set; }

        private int maxLogLength = 1024 * 1024; //1M

        // 窗体事件
        public PumpSimulate(LoginInfo info, Login from)
        {
            InitializeComponent();
            this.loginInfo = info;
            this.loginForm = from;
        }
        private void PumpSimulate_Load(object sender, EventArgs e)
        {
            Start();
        }
        private void PumpSimulate_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
            Application.Exit();

            //  if(this.loginForm.)

            //   if (this.IsExit)
            //        this.loginForm.Close();
        }

        // 服务控制
        private void Start()
        {
            if (this.loginInfo == null)
            {
                AddErrorTrace("登录传递的DTU信息为空对象");
                return;
            }
            this.curTitle=this.Text + "---" + this.loginInfo.PumpName + "-" + this.loginInfo.DTUID;
            this.Text = this.curTitle;
            this.clienManager = new UDPClientManager();
            this.clienManager.evtError += AddErrorTrace;//添加报错信息
            this.client = this.clienManager.ConnServer(this.loginInfo.IP, this.loginInfo.Port);
            if (this.client != null)
            {
                this.IsConnected = true;
                AddDebugTrace("已连接服务器");
                this.RefrehViewValue();

                this.StartListen();//开始监听线程
                this.StartHeart();//开始心跳线程
                this.StartAutoPLCDataServer();
                this.StartPLCDataServer();


            }
            else
            {
                this.IsConnected = false;
                AddErrorTrace("连接服务器" + this.loginInfo.IP + "失败");
                return;
            }
        }
        private void Stop()
        {
            if (this.IsConnected == false || this.client == null || this.clienManager == null)
                return;
            this.StopPLCDataServer();
            this.StopHeart();
            this.StoptListen();
            this.clienManager.ClearSelf();
            this.clienManager = null;

        }

        // 下线退出事件区
        private void isClose_Click(object sender, EventArgs e)
        {
            Stop();
            // 两种方法都是可以滴，第一种更粗暴
            Application.Exit();
            //loginForm.Close();
        }
        private void isOffline_Click(object sender, EventArgs e)
        {
            Stop();
            // 重新启动
            Application.Restart();
        }

        // 泵控制按钮事件
        private Color stopColor=  System.Drawing.SystemColors.Control;
        private Color runColor= Color.LawnGreen;
        private string btnRunText = "启动";
        private string btnStopText = "停止";
        private void btn1PC_Click(object sender, EventArgs e)
        {
            if (this.btn1PC.Text == btnRunText)
            {
                // 启动
                this.p1pStart.Show();
                this.p1pStop.Hide();
                this.btn1PC.Text = btnStopText;
                this.btn1PC.BackColor = runColor;
            }
            else
            {
                // 停止
                this.p1pStart.Hide();
                this.p1pStop.Show();
                this.btn1PC.Text = btnRunText;
                this.btn1PC.BackColor = stopColor;
            }

        }
        private void btn2PC_Click(object sender, EventArgs e)
        {
            if (this.btn2PC.Text == btnRunText)
            {
                // 启动
                this.p2pStart.Show();
                this.p2pStop.Hide();
                this.btn2PC.Text = btnStopText;
                this.btn2PC.BackColor = runColor;
            }
            else
            {
                // 停止
                this.p2pStart.Hide();
                this.p2pStop.Show();
                this.btn2PC.Text = btnRunText;
                this.btn2PC.BackColor = stopColor;
            }
        }
        private void btn3PC_Click(object sender, EventArgs e)
        {
            if (this.btn3PC.Text == btnRunText)
            {
                // 启动
                this.p3pStart.Show();
                this.p3pStop.Hide();
                this.btn3PC.Text = btnStopText;
                this.btn3PC.BackColor = runColor;
            }
            else
            {
                // 停止
                this.p3pStart.Hide();
                this.p3pStop.Show();
                this.btn3PC.Text = btnRunText;
                this.btn3PC.BackColor = stopColor;
            }
        }
        private void btn4PC_Click(object sender, EventArgs e)
        {
            if (this.btn4PC.Text == btnRunText)
            {
                // 启动
                this.p4pStart.Show();
                this.p4pStop.Hide();
                this.btn4PC.Text = btnStopText;
                this.btn4PC.BackColor = runColor;
            }
            else
            {
                // 停止
                this.p4pStart.Hide();
                this.p4pStop.Show();
                this.btn4PC.Text = btnRunText;
                this.btn4PC.BackColor = stopColor;
            }
        }
        private void btn5PC_Click(object sender, EventArgs e)
        {
            if (this.btn5PC.Text == btnRunText)
            {
                // 启动
                this.p5pStart.Show();
                this.p5pStop.Hide();
                this.btn5PC.Text = btnStopText;
                this.btn5PC.BackColor = runColor;
            }
            else
            {
                // 停止
                this.p5pStart.Hide();
                this.p5pStop.Show();
                this.btn5PC.Text = btnRunText;
                this.btn5PC.BackColor = stopColor;
            }
        }

        // 控件刷新方法
        private void RefrehViewValue()
        {
            this.BeginInvoke(new Action(() => {
                this.RefreshButton();
                this.RefreshLab();
            }));
        }
        private void RefreshButton()
        {
            if (btn1PC.Text == btnStopText)
            {
                this.p1pStart.Show();
                this.p1pStop.Hide();
                this.btn1PC.BackColor = Color.LawnGreen;
            }
            else
            {
                this.p1pStart.Hide();
                this.p1pStop.Show();
                this.btn1PC.BackColor = stopColor;
            }
            if (btn2PC.Text == btnStopText)
            {
                this.p2pStart.Show();
                this.p2pStop.Hide();
                this.btn2PC.BackColor = Color.LawnGreen;
            }
            else
            {
                this.p2pStart.Hide();
                this.p2pStop.Show();
                this.btn2PC.BackColor = stopColor;
            }
            if (btn3PC.Text == btnStopText)
            {
                this.p3pStart.Show();
                this.p3pStop.Hide();
                this.btn3PC.BackColor = Color.LawnGreen;
            }
            else
            {
                this.p3pStart.Hide();
                this.p3pStop.Show();
                this.btn3PC.BackColor = stopColor;
            }
            if (btn4PC.Text == btnStopText)
            {
                this.p4pStart.Show();
                this.p4pStop.Hide();
                this.btn4PC.BackColor = Color.LawnGreen;
            }
            else
            {
                this.p4pStart.Hide();
                this.p4pStop.Show();
                this.btn4PC.BackColor = stopColor;
            }
            if (btn5PC.Text == btnStopText)
            {
                this.p5pStart.Show();
                this.p5pStop.Hide();
                this.btn5PC.BackColor = Color.LawnGreen;
            }
            else
            {
                this.p5pStart.Hide();
                this.p5pStop.Show();
                this.btn5PC.BackColor = stopColor;
            }
        }
        private void RefreshLab()
        {
            this.lab1A.Text = this.numIn1A.Value.ToString();
            this.lab2A.Text = this.numIn2A.Value.ToString();
            this.lab3A.Text = this.numIn3A.Value.ToString();

            this.labJinShun.Text = this.numInJinShun.Value.ToString();
            this.labJinYa.Text = this.numInJinShiYa.Value.ToString();
            this.labChuShiYa.Text = this.numInChuShiYa.Value.ToString();
            this.labChuSheYa.Text = this.numInChuSheYa.Value.ToString();
            this.labChuShun.Text = this.numInChuShun.Value.ToString();
            this.labChuLei.Text = this.numInChuLei.Value.ToString();

            if(this.CurPLCAddr==1)
                this.Text = this.curTitle+"--低区";
            else if (this.CurPLCAddr == 2)
                this.Text = this.curTitle + "--中区";
            else if (this.CurPLCAddr == 3)
                this.Text = this.curTitle + "--高区";
        }

        // 参数改变事件
        private void numInJinShun_ValueChanged(object sender, EventArgs e)
        {
            this.labJinShun.Text = this.numInJinShun.Value.ToString();
        }
        private void numInChuShun_ValueChanged(object sender, EventArgs e)
        {
            this.labChuShun.Text = this.numInChuShun.Value.ToString();
        }
        private void numInChuLei_ValueChanged(object sender, EventArgs e)
        {
            this.labChuLei.Text = this.numInChuLei.ToString();
        }
        private void numInJinShiYa_ValueChanged(object sender, EventArgs e)
        {
            this.labJinYa.Text = this.numInJinShiYa.Value.ToString();
        }
        private void numInChuShiYa_ValueChanged(object sender, EventArgs e)
        {
            this.labChuShiYa.Text = this.numInChuShiYa.Value.ToString();
        }
        private void numInChuSheYa_ValueChanged(object sender, EventArgs e)
        {
            this.labChuSheYa.Text = this.numInChuSheYa.Value.ToString();
        }
        private void numIn1A_ValueChanged(object sender, EventArgs e)
        {
            this.lab1A.Text = this.numIn1A.Value.ToString();
        }
        private void numIn2A_ValueChanged(object sender, EventArgs e)
        {
            this.lab2A.Text = this.numIn2A.Value.ToString();
        }
        private void numIn3A_ValueChanged(object sender, EventArgs e)
        {
            this.lab3A.Text = this.numIn3A.Value.ToString();
        }

        // 日志功能区
        private void renderOneTraceText(TraceText trace)
        {
            this.BeginInvoke(new Action(() => {
                int textLength = this.textBoxTrace.TextLength;
                if (textLength > maxLogLength)
                    this.textBoxTrace.Clear();

                this.textBoxTrace.SuspendLayout();
                this.textBoxTrace.SelectionStart = this.textBoxTrace.TextLength;
                this.textBoxTrace.SelectionColor = trace.Color;
                this.textBoxTrace.SelectedText = trace.Text + "\r\n";
                this.textBoxTrace.ResumeLayout();
                this.textBoxTrace.ScrollToCaret();
            }));
        }
        private void AddDebugTrace(string mess)
        {
            renderOneTraceText(TraceText.ToDebugTrace(mess));
        }
        private void AddErrorTrace(string mess)
        {
            renderOneTraceText(TraceText.ToErrorTrace(mess));
        }

        // 校验
        private bool CheckServerConnect()
        {
            if (!this.IsConnected)
            {
                AddErrorTrace("未连接服务器");
                return false;
            }
            if (this.client == null || this.clienManager == null)
            {
                AddErrorTrace("客户端未初始化");
                return false;
            }
            return true;
        }
        public  bool EqualArray(byte[] souce, byte[] target)
        {
            if (souce == null && target != null)
                return false;
            if (souce != null && target == null)
                return false;
            if (souce.Length != target.Length)
                return false;
            int i = 0;
            foreach (byte item in souce)
            {
                if (item != target[i])
                    return false;
                i++;
            }
            return true;
        }

        // 心跳服务
        private void StartHeart()
        {
            if (!this.CheckServerConnect())
                return;

            AddDebugTrace("心跳服务已经打开");
            this.testOnceHeart();
            heartTesterTimer = new System.Timers.Timer();
            heartTesterTimer.Interval = Config.heartTime*1000;
            heartTesterTimer.Elapsed += (o, ee) =>
            {
                this.testOnceHeart();
            };
            heartTesterTimer.Enabled = true;
            this.heartTesterIsRuning = true;
        }
        private void StopHeart()
        {
            if (heartTesterTimer == null)
                return;
            if (!this.heartTesterIsRuning)
                return;
            heartTesterTimer.Enabled = false;
            heartTesterTimer.Close();
            heartTesterTimer = null;
            this.heartTesterIsRuning = false;
            AddDebugTrace("心跳服务已经关闭");
        }
        private void testOnceHeart()
        {
            if(string.IsNullOrEmpty(this.loginInfo.DTUID))
            {
                AddErrorTrace("DTU身份编号不能为空对象" );
                return;
            }
            try
            {
                byte[] bc = GetHeartBuffer(this.loginInfo.DTUID);
                if (bc != null)
                {
                    if (this.clienManager.SendData(this.client, bc, out string errMsg))
                    {
                      //  AddDebugTrace("心跳数据发送成功");
                    }
                    else
                    {
                        AddErrorTrace("心跳数据发送失败:" + errMsg);
                    }
                }
            }
            catch (Exception ex) { AddErrorTrace(ex.Message); }

        }
        private System.Timers.Timer heartTesterTimer;
        private bool heartTesterIsRuning { get; set; }

        // PLC 数据服务
        private void StartPLCDataServer()
        {
            if (!this.CheckServerConnect())
                return;

            AddDebugTrace("PLC数据服务已经打开");
            this.testOncePLCData();
            plcTesterTimer = new System.Timers.Timer();
            plcTesterTimer.Interval = Config.plcRefreshTime * 1000;
            plcTesterTimer.Elapsed += (o, ee) =>
            {
                this.testOncePLCData();
            };
            plcTesterTimer.Enabled = true;
            this.plcTesterIsRuning = true;
        }
        private void StopPLCDataServer()
        {
            if (plcTesterTimer == null)
                return;
            if (!this.heartTesterIsRuning)
                return;
            plcTesterTimer.Enabled = false;
            plcTesterTimer.Close();
            plcTesterTimer = null;
            this.plcTesterIsRuning = false;
            AddDebugTrace("PLC数据服务已经关闭");
        }
        private void testOncePLCData()
        {
            if (this.AutoPLCDataFlag)
                return;
            try
            {
                byte[] bc = GetPLCBuffer(this.loginInfo.DTUID);
                if (bc == null)
                    return;
                if (!this.EqualArray(bc, plcDataCache))
                {
                    // 跟缓存比较不一样立即推送上去
                    this.SendPLCData(bc);
                }
                else
                {
                    // 一样的话超过设定的时间才推送
                    TimeSpan span = DateTime.Now - plcSendTimeCache;
                    if (span.TotalSeconds > Config.plcDataTime)
                        this.SendPLCData(bc);
                }
            }
            catch (Exception ex)
            {
                AddErrorTrace("plc数据定时推送失败"+ex.Message);
            }
        }
        private void SendPLCData(byte[] buffer)
        {
            if (this.clienManager.SendData(this.client, buffer, out string errMsg))
            {
                this.plcDataCache = buffer;
                this.plcSendTimeCache = DateTime.Now;
               // AddDebugTrace("PLC数据推送成功" + BytesToText(buffer, buffer.Length));
                AddDebugTrace("PLC数据推送成功");
            }
            else
            {
                // AddLog("PLC定时数据发送失败:" + errMsg + "---" + BytesToText(bc, bc.Length));
                AddErrorTrace("PLC数据推送失败");
            }
        }
        private System.Timers.Timer plcTesterTimer;
        private bool plcTesterIsRuning { get; set; }
        private byte[] plcDataCache = null;
        private DateTime plcSendTimeCache;

        // 监听服务
        private void StartListen()
        {
            if (listenerIsRuning)
                return;
            this.clienManager.DataReceived += DataReceived;
            this.listenerIsRuning = true;
            AddDebugTrace("监听线程已打开");
        }
        private void StoptListen()
        {
            if (!listenerIsRuning)
                return;
            this.clienManager.DataReceived -= DataReceived;
            AddDebugTrace("监听线程已关闭");
            this.listenerIsRuning = false;
        }
        private bool listenerIsRuning { get; set; }
        private void DataReceived(object sender, byte[] data)
        {
            if (!this.listenerIsRuning)
                return;

          //  AddDebugTrace("收到服务数据---" + BytesToText(data, data.Length));
            if (data.Length == 16)
            {
                byte[] bc = GetHeartBufferAnswer(this.loginInfo.DTUID);
                if (BytesToText(data, data.Length) == BytesToText(bc, bc.Length))
                {
                    AddDebugTrace("注册成功");
                    return;
                }
            }
            if (data.Length == 24)
            {
                byte[] bc = GetPLCBufferAnswer(this.loginInfo.DTUID);
                if (BytesToText(data, 17) == BytesToText(bc, bc.Length))
                {
                    AddDebugTrace("服务器请求客户端数据");
                    // 加入回应队列
                   // AppendBuffer(data);
                    return;
                }
            }

        }

        // 字节转换操作方法
        private string BytesToText(byte[] str, int len)
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
        private byte[] TextToBytes(string hex)
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
        private byte[] GetHeartBuffer(string dtuID)
        {
            byte[] bc = new byte[15];
            //抬头
            bc[0] = 0x7B;
            bc[1] = 0x01;
            bc[2] = 0x00;
            bc[3] = 0x16;
            //手机号
            string tel = dtuID.Trim();
            for (int i = 0; i < 11; i++)
            {
                byte ii = Convert.ToByte(Convert.ToUInt16(tel[i].ToString()) + 0x30);
                bc[i + 4] = ii;
            }
            // IPE
            bc = bc.Concat(this.client.ipLocalEndPoint.Address.GetAddressBytes()).ToArray();
            // 使用bitconvet注意反转
            bc = bc.Concat(BitConverter.GetBytes((ushort)(this.client.ipLocalEndPoint.Port)).Reverse()).ToArray();
            // 结束标志
            bc = bc.Concat(new byte[1] { 0x7B }).ToArray();
            return bc;
        }
        private byte[] GetHeartBufferAnswer(string dtuID)
        {
            byte[] bc = new byte[15];

            //抬头
            bc[0] = 0x7B;
            bc[1] = 0x81;
            bc[2] = 0x00;
            bc[3] = 0x10;
            //手机号
            string tel = dtuID.Trim();
            for (int i = 0; i < 11; i++)
            {
                byte ii = Convert.ToByte(Convert.ToUInt16(tel[i].ToString()) + 0x30);
                bc[i + 4] = ii;
            }
            // 结束标志
            bc = bc.Concat(new byte[1] { 0x7B }).ToArray();
            return bc;
        }
        private byte[] GetPLCBufferAnswer(string dtuID)
        {
            byte[] bc = new byte[15];

            //抬头
            bc[0] = 0x7B;
            bc[1] = 0x89;
            bc[2] = 0x00;
            bc[3] = 0x10;
            //手机号
            string tel = dtuID.Trim();
            for (int i = 0; i < 11; i++)
            {
                byte ii = Convert.ToByte(Convert.ToUInt16(tel[i].ToString()) + 0x30);
                bc[i + 4] = ii;
            }
            // 结束标志
            bc = bc.Concat(new byte[1] { 0x7B }).ToArray();

            //正文--PLC地址
            bc = bc.Concat(new byte[1] { 0x01 }).ToArray();
            return bc;
        }

        // 数值转字节数组方法
        private byte[] Get16Buffer(ushort value)
        {
            ushort r = Convert.ToUInt16(value);
            return BitConverter.GetBytes(r).Reverse().ToArray();
        }
        private  byte[] Get32Buffer(UInt32 value)
        {
            UInt32 r = Convert.ToUInt32(value);
            return BitConverter.GetBytes(r).Reverse().ToArray();
        }
        private  byte[] Get48Buffer(UInt64 value)
        {
            UInt64 r = Convert.ToUInt64(value);
            return BitConverter.GetBytes(r).Reverse().Skip(2).ToArray();
        }

        // 替换实时值到数组
        private void ReplaceRealValue(ref byte[] plcBodyData)
        {
            byte[] plcData = plcBodyData.Skip(7).ToArray();
            // 泵替换
            this.ReplacePump16Bit(this.btn1PC.Text, GetBufferIndex(PointsAddr.P1State), ref plcData);
            this.ReplacePump16Bit(this.btn2PC.Text, GetBufferIndex(PointsAddr.P2State), ref plcData);
            this.ReplacePump16Bit(this.btn3PC.Text, GetBufferIndex(PointsAddr.P3State), ref plcData);
            this.ReplacePump16Bit(this.btn4PC.Text, GetBufferIndex(PointsAddr.PSP1State), ref plcData);
            this.ReplacePump16Bit(this.btn5PC.Text, GetBufferIndex(PointsAddr.PSP2State), ref plcData);
            // 其它16位参数替换
            this.ReplaceNumRic16Bit(this.numIn1A.Value, GetBufferIndex(PointsAddr.Bian1A), ref plcData);
            this.ReplaceNumRic16Bit(this.numIn2A.Value, GetBufferIndex(PointsAddr.Bian2A), ref plcData);
            this.ReplaceNumRic16Bit(this.numIn3A.Value, GetBufferIndex(PointsAddr.Bian3A), ref plcData);

            this.ReplaceNumRic16Bit(this.numInJinShiYa.Value, GetBufferIndex(PointsAddr.JinShiYa), ref plcData);
            this.ReplaceNumRic16Bit(this.numInJinShun.Value, GetBufferIndex(PointsAddr.JinShun), ref plcData);

            this.ReplaceNumRic16Bit(this.numInChuShiYa.Value, GetBufferIndex(PointsAddr.ChuShiYa), ref plcData);
            this.ReplaceNumRic16Bit(this.numInChuShun.Value, GetBufferIndex(PointsAddr.ChuShun), ref plcData);
            this.ReplaceNumRic16Bit(this.numInChuSheYa.Value, GetBufferIndex(PointsAddr.ChuSheYa), ref plcData);
            // 48位值替换
            this.ReplaceNumRic32Bit(this.numInChuLei.Value, GetBufferIndex(PointsAddr.ChuLei), ref plcData);

            plcBodyData = plcBodyData.Take(7).Concat(plcData).ToArray();
        }
        private void ReplacePump16Bit(string btnText,int beginIndex, ref byte[] plcBodyData)
        {
            ushort r = GetPumpRealValue(btnText.Trim());
            byte[] rb = Get16Buffer(r);
            ReplaceBuffer(beginIndex, 2, rb, ref plcBodyData);
        }
        private void ReplaceNumRic16Bit(decimal numricValue, int beginIndex, ref byte[] plcBodyData)
        {
            ushort r = GetDecimalValue(numricValue);
            byte[] rb = Get16Buffer(r);
            ReplaceBuffer(beginIndex, 2, rb, ref plcBodyData);
        }
        private ushort GetPumpRealValue(string btnText)
        {
            if (btnText.Trim() == btnStopText)
                return 2;
           else
                return 1;
        }
        private ushort GetDecimalValue(decimal souce)
        {
            int times = GetPointIndex(souce);
            if (times == 0)
                return Convert.ToUInt16(souce);
            for(int i = 0; i <times; i++)
            {
                souce = souce * 10;
            }
            return Convert.ToUInt16(souce);
        }
        private int GetPointIndex(decimal source)
        {
            string numStr = source.ToString();
            if (numStr.IndexOf('.') == -1)
                return 0;
            int result = numStr.Length - numStr.IndexOf('.') - 1;
            return result;
        }
        private void ReplaceNumRic32Bit(decimal numricValue, int beginIndex, ref byte[] plcBodyData)
        {
            UInt32 r = Convert.ToUInt32(numricValue);
            byte[] rb = Get32Buffer(r);
            ReplaceBuffer(beginIndex, 4, rb, ref plcBodyData);
        }
        private void ReplaceBuffer(int beginIndex, int length, byte[] source, ref byte[] target)
        {
            if (length == 2)
            {
                target[beginIndex] = source[0];
                target[beginIndex + 1] = source[1];
            }
            else if (length == 4)
            {
                target[beginIndex] = source[0];
                target[beginIndex + 1] = source[1];
                target[beginIndex + 2] = source[2];
                target[beginIndex + 3] = source[3];
            }
            else if (length == 6)
            {
                target[beginIndex] = source[0];
                target[beginIndex + 1] = source[1];
                target[beginIndex + 2] = source[2];
                target[beginIndex + 3] = source[3];
                target[beginIndex + 4] = source[4];
                target[beginIndex + 5] = source[5];
            }
        }
        private int GetBufferIndex(string addr)
        {
            addr = addr.Trim().Split('.')[0].Substring(2);
            // 偏移一位，两个字节表一个地址
            return GetBufferIndex(DataUtil.ToInt(addr));
        }
        private int GetBufferIndex(int addr)
        {
            // 偏移一位，两个字节表一个地址
            return (addr - 1) * 2;
        }

        // 获取PLC实时值方法
        private byte[] GetPLCBuffer(string dtuID)
        {
            byte[] head = GetPLCHeadData(dtuID);
            byte[] initBody = GetInitPLCBodyData();
            ReplaceRealValue(ref initBody);
            byte[] body = CRCTool.AddCRC(initBody);
            return head.Concat(body).ToArray();
        }
        private byte[] GetPLCHeadData(string dtuID)
        {
            byte[] bc = new byte[16];
            //抬头
            bc[0] = 0x7B;
            bc[1] = 0x09;
            bc[2] = 0x00;
            bc[3] = 0x10;
            //手机号
            string tel = dtuID.Trim();
            for (int i = 0; i < 11; i++)
            {
                byte ii = Convert.ToByte(Convert.ToUInt16(tel[i].ToString()) + 0x30);
                bc[i + 4] = ii;
            }
            bc[15] = 0x7B;
            return bc;
        }
        private byte[] GetInitPLCBodyData()
        {
            // 先准备好250*2有效数据+7个排位+2个字节的CRC
              string data = string.Format(@"02 10 00 00 00 78 F0
                                            00 01 00 65 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   

                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   

                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00    

                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   

                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00   
                                            ");
            return this.TextToBytes(data);
        }
        private ushort CurPLCAddr { get; set; } = 0;
        private ushort GetCurPLCAddr()
        {
            if (this.CurPLCAddr == 1 || (this.CurPLCAddr == 2))
                this.CurPLCAddr++;
            else
                this.CurPLCAddr=1;
            return this.CurPLCAddr;
        }

        //获取随机PLC数据方法
        private byte[] GetPLCAutoBuffer(string dtuID)
        {
            byte[] head = GetPLCHeadData(dtuID);
            byte[] initBody = GetInitPLCBodyData();
            this.Invoke(new Action(() => {
                ReplaceAutoRealValue(ref initBody);
            }));
            byte[] aa = initBody;
            byte[] body = CRCTool.AddCRC(initBody);
            return head.Concat(body).ToArray();
        }
        private void ReplaceAutoRealValue(ref byte[] plcBodyData)
        {
            byte[] plcData = plcBodyData.Skip(7).ToArray();
            // pLC地址
            decimal p = GetCurPLCAddr();
            this.ReplaceNumRic16Bit(p, GetBufferIndex(PointsAddr.PLCAddr), ref plcData);
            
            // 泵状态
            decimal p1 = GetRandom(1, 2, 1);
            decimal p2 = GetRandom(1, 2, 1);
            decimal p3 = GetRandom(1, 2, 1);
            decimal p4 = GetRandom(1, 2, 1);
            decimal p5 = GetRandom(1, 2, 1);
            this.ReplaceNumRic16Bit(p1, GetBufferIndex(PointsAddr.P1State), ref plcData);
            this.ReplaceNumRic16Bit(p2, GetBufferIndex(PointsAddr.P2State), ref plcData);
            this.ReplaceNumRic16Bit(p3, GetBufferIndex(PointsAddr.P3State), ref plcData);
            this.ReplaceNumRic16Bit(p4, GetBufferIndex(PointsAddr.PSP1State), ref plcData);
            this.ReplaceNumRic16Bit(p5, GetBufferIndex(PointsAddr.PSP2State), ref plcData);

            this.btn1PC.Text = p1 == 1 ? this.btnStopText : this.btnRunText;
            this.btn2PC.Text = p2 == 1 ? this.btnStopText : this.btnRunText;
            this.btn3PC.Text = p3 == 1 ? this.btnStopText : this.btnRunText;
            this.btn4PC.Text = p4 == 1 ? this.btnStopText : this.btnRunText;
            this.btn5PC.Text = p5== 1 ? this.btnStopText : this.btnRunText;

            // 进出水实际压力
            decimal jinYa = GetRandom(40, 65, 0.01);
            decimal chuYa = GetRandom(40, 65, 0.01);
            this.ReplaceNumRic16Bit(jinYa, GetBufferIndex(PointsAddr.JinShiYa), ref plcData);
            this.ReplaceNumRic16Bit(chuYa, GetBufferIndex(PointsAddr.ChuShiYa), ref plcData);

            this.numInJinShiYa.Value = jinYa;
            this.numInChuShiYa.Value = chuYa;
            // 设定压力
            decimal chuSheYa = GetRandom(40, 65, 0.01);
            this.ReplaceNumRic16Bit(chuSheYa, GetBufferIndex(PointsAddr.ChuSheYa), ref plcData);
            this.numInChuSheYa.Value = chuSheYa;
            // 变频器电流
            decimal a1 = GetRandom(25, 40, 0.1);
            decimal a2 = GetRandom(25, 40, 0.1);
            decimal a3 = GetRandom(25, 40, 0.1);
            this.ReplaceNumRic16Bit(a1, GetBufferIndex(PointsAddr.Bian1A), ref plcData);
            this.ReplaceNumRic16Bit(a2, GetBufferIndex(PointsAddr.Bian2A), ref plcData);
            this.ReplaceNumRic16Bit(a3, GetBufferIndex(PointsAddr.Bian3A), ref plcData);
            this.numIn1A.Value = a1;
            this.numIn2A.Value = a2;
            this.numIn3A.Value = a3;
            // 泵运行时间
            this.ReplaceNumRic16Bit(GetRandom(300, 3456, 1), GetBufferIndex(PointsAddr.P1Time), ref plcData);
            this.ReplaceNumRic16Bit(GetRandom(300, 3456, 1), GetBufferIndex(PointsAddr.P2Time), ref plcData);
            this.ReplaceNumRic16Bit(GetRandom(300, 3456, 1), GetBufferIndex(PointsAddr.P3Time), ref plcData);
            // 进水流量
            decimal jinShun = GetRandom(0, 2500, 0.01);
            decimal jinLei = GetRandom(1234, 560000, 1);
            this.ReplaceNumRic16Bit(jinShun, GetBufferIndex(PointsAddr.JinShun), ref plcData);
            this.ReplaceNumRic32Bit(jinLei, GetBufferIndex(PointsAddr.JinLei), ref plcData);
            this.numInJinShun.Value = jinShun;
            // 出水流量
            decimal chuShun = GetRandom(0, 2500, 0.01);
            decimal chuLei = GetRandom(1234, 560000, 1);
            this.ReplaceNumRic16Bit(chuShun, GetBufferIndex(PointsAddr.ChuShun), ref plcData);
            this.ReplaceNumRic32Bit(chuLei, GetBufferIndex(PointsAddr.ChuLei), ref plcData);
            this.numInChuShun.Value = chuShun;
            this.numInChuLei.Value = chuLei;
            //累计电量
            this.ReplaceNumRic32Bit(GetRandom(1234, 560000, 1), GetBufferIndex(PointsAddr.LJDianLiang), ref plcData);
            //变频器功率
            this.ReplaceNumRic16Bit(GetRandom(0, 300, 0.1), GetBufferIndex(PointsAddr.Bian1P), ref plcData);
            this.ReplaceNumRic16Bit(GetRandom(0, 300, 0.1), GetBufferIndex(PointsAddr.Bian2P), ref plcData);
            this.ReplaceNumRic16Bit(GetRandom(0, 300, 0.1), GetBufferIndex(PointsAddr.Bian3P), ref plcData);

            // 报警模拟
            decimal baojing = GetRandom(0, 255,1);
           // this.ReplaceNumRic16Bit(baojing, GetBufferIndex(PointsAddr.Baojing), ref plcData);

            plcBodyData = plcBodyData.Take(7).Concat(plcData).ToArray();
        }

        private decimal GetRandom(int beginNum,int endNum,double scale)
        {
              endNum= endNum+1;
              Random rd = new Random(Guid.NewGuid().GetHashCode());
            return (decimal) (rd.Next(beginNum, endNum) * scale);
        }

        // PLC数据随机生成
        private bool AutoPLCDataFlag { get; set; } = true;
        private void RefreshAutoBtn()
        {
            if (AutoPLCDataFlag)
                this.btnAutoPLCData.BackColor = this.runColor;
            else
                this.btnAutoPLCData.BackColor = this.stopColor;
        }
        private void btnAutoPLCData_Click(object sender, EventArgs e)
        {
            if (this.AutoPLCDataFlag)
                StopAutoPLCDataServer();
            else
                StartAutoPLCDataServer();
        }

        // PLC数据随机生成服务
        private System.Timers.Timer plcAutocTesterTimer;
        private void StartAutoPLCDataServer()
        {
            if (!this.CheckServerConnect())
                return;

            this.testOncePLCAutoData();
            plcAutocTesterTimer = new System.Timers.Timer();
            plcAutocTesterTimer.Interval = Config.plcRefreshTime * 1000;
            plcAutocTesterTimer.Elapsed += (o, ee) =>
            {
                this.testOncePLCAutoData();
            };
            plcAutocTesterTimer.Enabled = true;

            this.AutoPLCDataFlag = true;
            this.RefreshAutoBtn();
            AddDebugTrace("PLC随机生成数据服务已经打开");
        }
        private void StopAutoPLCDataServer()
        {
            if (plcAutocTesterTimer == null)
                return;
            if (!this.heartTesterIsRuning)
                return;
            plcAutocTesterTimer.Enabled = false;
            plcAutocTesterTimer.Close();
            plcAutocTesterTimer = null;
            this.AutoPLCDataFlag = false;
            this.RefreshAutoBtn();
            AddDebugTrace("PLC数据服务已经关闭");
        }
        private void testOncePLCAutoData()
        {
            if (!this.AutoPLCDataFlag)
                return;
            try
            {
                byte[] bc = GetPLCAutoBuffer(this.loginInfo.DTUID);
                if (bc == null)
                    return;
                this.SendPLCData(bc);
                this.RefrehViewValue();
            }
            catch (Exception ex)
            {
                AddErrorTrace("plc数据定时推送失败" + ex.Message);
            }
        }

    }

    public class TraceText
    {
        public Color Color;
        public string Text;

        public static TraceText ToDebugTrace(string mess)
        {
            TraceText trace = new TraceText();
            trace.Color = Color.Blue;
            trace.Text = DateTime.Now.ToString("HH:mm:ss  ") + mess;
            return trace;
        }

        public static TraceText ToErrorTrace(string mess)
        {
            TraceText trace = new TraceText();
            trace.Color = Color.Red;
            trace.Text = DateTime.Now.ToString("HH:mm:ss  ")+mess;
            return trace;
        }
    }

}
