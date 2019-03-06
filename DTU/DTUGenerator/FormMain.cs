using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTUGenerator
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private UDPClient client;
        private UDPClientManager clienManager;
        private bool IsConnected { get; set; }

        // 配置栏
        private void FormMain_Load(object sender, EventArgs e)
        {
            RefreshText();
          //  this.textYuMing.Text = "172.16.10.58";
           // this.numericPort.Value = 6000;
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (IsConnected)
                return;

            if (!this.CheckTel())
            {
                ShowMessageAndAddLog("电话只能为11位数字");
                return;
            }
           this.clienManager = new UDPClientManager();
           this.client= this.clienManager.ConnServer(this.textYuMing.Text.Trim(), (int)this.numericPort.Value);
            if (this.client != null)
            {
                this.IsConnected = true;
                this.StartRead();
                AddLog("已连接服务器");
            }
            else
                this.IsConnected = false;
            this.RefreshText();
        }        
        private void bttnIsDisconnect_Click(object sender, EventArgs e)
        {
            if (!this.IsConnected)
                MessageBox.Show("没有可断开的服务器");
            this.StopRead();
            this.clienManager.ClearSelf();
            this.client = null;
            this.clienManager = null;
            this.IsConnected = false;    
            this.AddLog("已断开服务器连接");
            this.RefreshText();
        }

        // 刷新
        private void RefreshText()
        {
            this.Invoke(new Action(() =>
            {
                // 服务器连接状态
                if (!this.IsConnected)
                {
                    this.Text = "DTU发生器--未连接";
                    this.btnConnect.BackColor = Color.Gainsboro;
                    this.bttnIsDisconnect.BackColor = Color.Red;
                }

                else
                {
                    this.Text = "DTU发生器--已连接";
                    this.btnConnect.BackColor = Color.Aqua;
                    this.bttnIsDisconnect.BackColor = Color.Gainsboro;
                }

                // 定时发送者状态
                if (this.testSenderIsRuning)
                {
                    this.IsStartDataTest.BackColor = Color.Aqua;
                    this.IsStoptDataTest.BackColor = Color.Gainsboro;
                }
                else
                {
                    this.IsStartDataTest.BackColor = Color.Gainsboro;
                    this.IsStoptDataTest.BackColor = Color.Red;
                }

                // 心跳定时者状态
                if (this.heartTesterIsRuning)
                {
                    this.isStartXinTiao.BackColor = Color.Aqua;
                    this.isStopXinTiao.BackColor = Color.Gainsboro;
                }
                else
                {
                    this.isStartXinTiao.BackColor = Color.Gainsboro;
                    this.isStopXinTiao.BackColor = Color.Red;
                }

                // PLC定时者状态
                if (this.plcTesterIsRuning)
                {
                    this.isStartSendPLC.BackColor = Color.Aqua;
                    this.isStopSendPLC.BackColor = Color.Gainsboro;
                }
                else
                {
                    this.isStartSendPLC.BackColor = Color.Gainsboro;
                    this.isStopSendPLC.BackColor = Color.Red;
                }

                // 应答测试者状态
                if (this.taskAnswerIsRuning)
                {
                    this.isStartAnswer.BackColor = Color.Aqua;
                    this.isStopAnswer.BackColor = Color.Gainsboro;
                }
                else
                {
                    this.isStartAnswer.BackColor = Color.Gainsboro;
                    this.isStopAnswer.BackColor = Color.Red;
                }

            }));
        }

        // 清空日志
        private void ToolStripMenuItem_clearLog_Click(object sender, EventArgs e)
        {
            this.textBoxLog.Clear();
        }
        private void ToolStripMenuItem_clearReciveLog_Click(object sender, EventArgs e)
        {
            this.textBoxRecive.Clear();
        }

        // 添加打印
        private void AddLog(string mess)
        {
            this.BeginInvoke(new Action(() =>
            {
                int textLength = this.textBoxLog.TextLength;
                if (textLength > 1024 * 1024)
                    this.textBoxLog.Clear();

                this.textBoxLog.SuspendLayout();
                this.textBoxLog.SelectionStart = this.textBoxLog.TextLength;
                this.textBoxLog.SelectionColor = Color.Blue;
                this.textBoxLog.SelectedText =DateTime.Now.ToString("MM-dd HH:mm:ss") +"  "+ mess + "\r\n";
                this.textBoxLog.ResumeLayout();
                this.textBoxLog.ScrollToCaret();
            }));
        }
        private void AddRecived(string mess)
        {
            this.BeginInvoke(new Action(() =>
            {
                int textLength = this.textBoxRecive.TextLength;
                if (textLength > 1024 * 1024)
                    this.textBoxRecive.Clear();

                this.textBoxRecive.SuspendLayout();
                this.textBoxRecive.SelectionStart = this.textBoxRecive.TextLength;
                this.textBoxRecive.SelectionColor = Color.Blue;
                this.textBoxRecive.SelectedText = DateTime.Now.ToString("MM-dd HH:mm:ss")+"  " + mess + "\r\n";
                this.textBoxRecive.ResumeLayout();
                this.textBoxRecive.ScrollToCaret();
            }));
        }

        // 日志显示
        private void ShowMessageAndAddLog(string mess)
        {
            ShowMessageAndAddLog(mess, MessageBoxIcon.Error);
        }
        private void ShowMessageAndAddLog(string mess,MessageBoxIcon icon)
        {
            AddLog(mess);
            MessageBox.Show(mess, "信息", MessageBoxButtons.OK, icon);
        }

        // 校验
        private bool CheckServerConnect()
        {
            if (!this.IsConnected)
            {
                ShowMessageAndAddLog("未连接服务器");
                return false;
            }
            if (this.client == null || this.clienManager == null)
            {
                ShowMessageAndAddLog("客户端未初始化");
                return false;
            }
            return true;
        }
        private bool CheckTel()
        {
            string tel = this.textTel.Text.Trim();
            if (tel.Length != 11)
                return false;
            try
            {
                foreach (char i in tel)
                {
                    int ii = Convert.ToInt32(i.ToString());
                    if(ii < 0 || ii > 9)
                        return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
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
        private int HexToStr(string hex, byte[] str)
        {
            //将16进制字符串转换放到BYTE数组里，并返回转换后的数据长度，参数是16进制字符串和字节数组 
            int asc = 0;
            int len = 0;
            string s = "";
            string ss = "";
            for (int i = 0; i < hex.Length; i++)
            {
                if (hex[i] != ' ')
                {
                    s = s + hex[i];
                    asc = (byte)hex[i];
                    if ((asc < 48) || ((asc > 57) && (asc < 65)) || ((asc > 70) && (asc < 97)) || (asc > 102))
                    {
                        ShowMessageAndAddLog("字符串中有字超过16进制范围");
                        return 0;
                    }
                }

            }
            for (int j = 0; j < (s.Length - 1); j = j + 2)
            {
                ss = "";
                ss = ss + s[j] + s[j + 1];
                str[len] = (byte)(System.Convert.ToInt32(ss, 16));
                len++;
            }
            return len;
        }
        private byte[] ToBytes(ushort data)
        {
            // 返回两个字节的数组，高位在前，低位在后
            byte[] buffer = new byte[2];
            buffer[0] = (byte)((0xff00 & data) >> 8);//拿到高位
            buffer[1] = (byte)(0xff & data);//拿到低位
            return buffer;
        }
        private byte[] GetBuffer()
        {
            string s = new string((char)0, 1024);       //分配1024字节长度的字符串数组
            byte[] bc = Encoding.Default.GetBytes(s);	//转换到字节数组
            string sendStr = this.textSendText.Text.Trim();
            if (bc.Length == 0)
                return null;
            bc = Encoding.Default.GetBytes(sendStr.ToCharArray(0, sendStr.Length), 0, sendStr.Length);
            int len;
            if (this.radioButton_16Hex.Checked)
            {
                //16进制发送数据
                len = HexToStr(sendStr, bc);
                bc = bc.Take(len).ToArray();
                if (len == 0)
                {
                    AddLog("字节转换错误");
                    return null;
                }
            }
            return bc;
        }
        private byte[] GetHeartBuffer()
        {
            byte[] bc = new byte[15];
            //抬头
            bc[0] = 0x7B;
            bc[1] = 0x01;
            bc[2] = 0x00;
            bc[3] = 0x16;
            //手机号
            string tel = this.textTel.Text.Trim();
            for(int i = 0; i < 11; i++)
            {
                byte ii =Convert.ToByte(Convert.ToUInt16(tel[i].ToString()) + 0x30);
                bc[i + 4] = ii;
            }
            // IPE
            bc=bc.Concat( this.client.ipLocalEndPoint.Address.GetAddressBytes()).ToArray();
            // 使用bitconvet注意反转
            bc = bc.Concat(BitConverter.GetBytes((ushort)(this.client.ipLocalEndPoint.Port)).Reverse()).ToArray();
            // 结束标志
            bc = bc.Concat(new byte[1]{ 0x7B }).ToArray();
            return bc;
        }
        private byte[] GetHeartBufferAnswer()
        {
            byte[] bc = new byte[15];
            
            //抬头
            bc[0] = 0x7B;
            bc[1] = 0x81;
            bc[2] = 0x00;
            bc[3] = 0x10;
            //手机号
            string tel = this.textTel.Text.Trim();
            for (int i = 0; i < 11; i++)
            {
                byte ii = Convert.ToByte(Convert.ToUInt16(tel[i].ToString()) + 0x30);
                bc[i + 4] = ii;
            }
            // 结束标志
            bc = bc.Concat(new byte[1] { 0x7B }).ToArray();
            return bc;
        }
        private byte[] GetPLCBuffer()
        {
            byte[] bc = new byte[16];
            //抬头
            bc[0] = 0x7B;
            bc[1] = 0x09;
            bc[2] = 0x00;
            bc[3] = 0x10;
            //手机号
            string tel = this.textTel.Text.Trim();
            for (int i = 0; i < 11; i++)
            {
                byte ii = Convert.ToByte(Convert.ToUInt16(tel[i].ToString()) + 0x30);
                bc[i + 4] = ii;
            }
            bc[15] = 0x7B;
            // 正文 小于1024
              string data1= string.Format(@"02 10 00 00 00 78 F0 00 01 00 67 00 00 00 00 00 39 00 00 00 
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

            string data = string.Format(@"02 10 00 00 00 78 F0
                                            00 01 00 65 00 00 00 00 00 00 00 01 00 02 00 1A 00 39 00 3A
                                            00 01 00 01 00 02 00 00 00 00 00 00 01 DA 01 DB 01 D5 00 00
                                            00 00 00 00 01 03 01 21 00 01 00 03 00 0F C7 4B 00 00 00 07
                                            D8 29 00 00 00 07 D8 29 00 00 00 01 00 00 00 0A 00 00 00 01
                                            00 00 00 55 00 61 00 4A 00 00 00 00 00 00 00 1C 00 1A 00 15
                                            00 00 00 00 00 00 04 66 04 1A 03 E4 00 00 00 00 00 00 00 06
                                            00 84 02 C1 A2 31 00 D5 00 16 00 1E 00 48 00 01 00 01 00 00
                                            00 00 00 03 
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                            00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 5A");


            byte[] dataBuffer = this.TextToBytes(data);
            bc = bc.Concat(dataBuffer).ToArray();
            return bc;
        }
        private byte[] GetPLCBuffer(byte[] reciveBuffer)
        {
            // 收到数据改为发送数据标识
            reciveBuffer[1] = 0x09;
            // 后8位拿出根据modbus回应方式塞回
            switch (reciveBuffer[17])
            {
                case 0x03:
                    {
                        //  读数据
                        byte[] readBuffer = GetReadValueBytes(reciveBuffer.Skip(16).ToArray());
                        return reciveBuffer.Take(16).Concat(readBuffer).ToArray();
                    }
                case 0x06:
                    {
                        // 写数据
                        byte[] writeBuffer = GetWriteValueBytes(reciveBuffer.Skip(16).ToArray());
                        return reciveBuffer.Take(16).Concat(writeBuffer).ToArray();
                    }
            }
            return reciveBuffer;
        }
        private byte[] GetPLCBufferAnswer()
        {
            byte[] bc = new byte[15] ;

            //抬头
            bc[0] = 0x7B;
            bc[1] = 0x89;
            bc[2] = 0x00;
            bc[3] = 0x10;
            //手机号
            string tel = this.textTel.Text.Trim();
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
        private byte[] GetReadValueBytes(byte[] buffer)
        {
            //拿到地址值作为结果值返回去
            byte[] addr = buffer.Skip(2).Take(2).ToArray();
            buffer[2] = 0x02;
            ////// 使用随机数应答 1-10
            //Random rd = new Random();
            //ushort r= Convert.ToUInt16( rd.Next(1, 1000));
            //buffer = buffer.Take(3).Concat(BitConverter.GetBytes(r).Reverse()).ToArray();
            buffer = buffer.Take(3).Concat(addr).ToArray();
            buffer = CRCTool.AddCRC(buffer);
            return buffer;
        }
        private byte[] GetWriteValueBytes(byte[] buffer)
        {
            // 更改标志位即可
            buffer[1] = 0x10;
            buffer= CRCTool.AddCRC(buffer.Take(6).ToArray());
            return buffer;
        }


        // 数据发送
        private System.Timers.Timer sendDataTimer;
        private void btnSendOneData_Click(object sender, EventArgs e)
        {
            if (!this.CheckServerConnect())
                return;
            SendOnce();

        }
        private void IsStartDataTest_Click(object sender, EventArgs e)
        {
            if (!this.CheckServerConnect())
                return;
            //开始循环调用Dll拿所有DTU方法去更新所有的状态
            this.AddLog("测试数据定时发送已打开");
            SendOnce();
            sendDataTimer = new System.Timers.Timer();
            sendDataTimer.Interval = (int)this.numericSendTimeSpan.Value;
            sendDataTimer.Elapsed += (o, ee) =>
            {
                SendOnce();
            };
            sendDataTimer.Enabled = true;
            this.testSenderIsRuning = true;
            this.RefreshText();
        }
        private void IsStoptDataTest_Click(object sender, EventArgs e)
        {
            if (sendDataTimer != null)
            {
                this.AddLog("测试数据定时发送已关闭");
                sendDataTimer.Enabled = false;
                sendDataTimer.Close();
                sendDataTimer = null;
                this.testSenderIsRuning = false;
                this.RefreshText();
            }
        }
        private void SendOnce()
        {
            try
            {
                byte[] bc = GetBuffer();
                if (bc != null)
                {
                    if (this.clienManager.SendData(this.client, bc, out string errMsg))
                    {
                        AddLog("测试数据发送成功---" + BytesToText(bc, bc.Length));
                    }
                    else
                    {
                        AddLog("测试数据发送失败:" + errMsg + "---" + BytesToText(bc, bc.Length));
                    }

                }
            }
            catch(Exception ex)
            {
                AddLog(ex.Message);
            }

        }
        private bool testSenderIsRuning { get; set; }

        // 心跳测试
        private void isStartXinTiao_Click(object sender, EventArgs e)
        {
            if (!this.CheckServerConnect())
                return;
            //开始循环调用Dll拿所有DTU方法去更新所有的状态
            this.AddLog("心跳测试数据定时发送已打开");
            this.testOnceHeart();
            heartTesterTimer = new System.Timers.Timer();
            heartTesterTimer.Interval = (int)this.numericHeartTimeSpan.Value;
            heartTesterTimer.Elapsed += (o, ee) =>
            {
                this.testOnceHeart();
            };
            heartTesterTimer.Enabled = true;
            this.heartTesterIsRuning = true;
            this.RefreshText();
        }
        private void isStopXinTiao_Click(object sender, EventArgs e)
        {
            if (heartTesterTimer != null)
            {
                this.AddLog("心跳测试数据定时发送已关闭");
                heartTesterTimer.Enabled = false;
                heartTesterTimer.Close();
                heartTesterTimer = null;
                this.heartTesterIsRuning = false;
                this.RefreshText();
            }
        }
        private void btnTestOnceHeart_Click(object sender, EventArgs e)
        {
            if (!CheckServerConnect())
                return;
            testOnceHeart();
        }
        private void testOnceHeart()
        {
            try
            {
                byte[] bc = GetHeartBuffer();
                if (bc != null)
                {
                    if (this.clienManager.SendData(this.client, bc, out string errMsg))
                    {
                        AddLog("心跳数据发送成功---" + BytesToText(bc, bc.Length));
                    }
                    else
                    {
                        AddLog("心跳数据发送失败:" + errMsg + "---" + BytesToText(bc, bc.Length));
                    }
                }
            }
            catch (Exception ex) { AddLog(ex.Message); }

        }
        private System.Timers.Timer heartTesterTimer;
        private bool heartTesterIsRuning { get; set; }

        // DTU定时推送模拟PLC数据
        private void isStartSendPLC_Click(object sender, EventArgs e)
        {
            if (!this.CheckServerConnect())
                return;
            //开始循环调用Dll拿所有DTU方法去更新所有的状态
            this.AddLog("PLC模拟数据定时发送已打开");
            this.testOncePLC();
            plcTesterTimer = new System.Timers.Timer();
            plcTesterTimer.Interval = (int)this.numericUpDownPLCTimeSpan.Value;
            plcTesterTimer.Elapsed += (o, ee) =>
            {
                this.testOncePLC();
            };
            plcTesterTimer.Enabled = true;
            this.plcTesterIsRuning = true;
            this.RefreshText();
        }
        private void isStopSendPLC_Click(object sender, EventArgs e)
        {
            if (plcTesterTimer != null)
            {
                this.AddLog("PLC模拟数据定时发送已关闭");
                plcTesterTimer.Enabled = false;
                plcTesterTimer.Close();
                plcTesterTimer = null;
                this.plcTesterIsRuning = false;
                this.RefreshText();
            }
        }
        private void testOncePLC()
        {
            try
            {
                byte[] bc = GetPLCBuffer();
                if (bc != null)
                {
                    if (this.clienManager.SendData(this.client, bc, out string errMsg))
                    {
                        AddLog("PLC定时数据发送成功---" + BytesToText(bc, bc.Length));
                    }
                    else
                    {
                        AddLog("PLC定时数据发送失败:" + errMsg + "---" + BytesToText(bc, bc.Length));
                    }

                }
            }
            catch(Exception ex)
            {
                AddLog(ex.Message);
            }

        }
        private System.Timers.Timer plcTesterTimer;
        private bool plcTesterIsRuning { get; set; }

        // 应答测试
        private void isStartAnswer_Click(object sender, EventArgs e)
        {
            if (this.taskAnswerIsRuning)
                return;
            this.reciveQueue = new BlockingCollection<byte[]>();
            this.taskAnswer = new Task(() => {
                foreach (byte[] buffer in reciveQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        if (this.radioButton_delay.Checked)
                        {
                            // 营造真实回答环境
                            Random rd = new Random();
                            int min = rd.Next(0, 10);
                            Thread.Sleep(min * 1000);
                        }
                        this.SendPLCOnce(buffer);
                    }
                    catch (Exception ex)
                    {
                        AddLog(ex.Message);
                    }
                }
            });
            this.taskAnswer.Start();
            this.taskAnswerIsRuning = true;
            AddLog("PLC模拟应答工作者已打开");
            this.RefreshText();
        }
        private void isStopAnswer_Click(object sender, EventArgs e)
        {
            if (!this.taskAnswerIsRuning)
                return;
            this.reciveQueue.CompleteAdding();
            //废弃队列暂存的元素
            if (this.reciveQueue.Count>0)
            {
                foreach (var item in this.reciveQueue.ToArray())
                {
                    this.reciveQueue.Take();
                }
                AddLog("接收队列有未处理的任务已全部废弃");
            }
            Task.WaitAll(this.taskAnswer);
            this.reciveQueue = null;
            this.taskAnswer.Dispose();
            this.taskAnswer = null;
            this.taskAnswerIsRuning = false;
            AddLog("PLC模拟应答工作者已关闭");
            this.RefreshText();
        }
        private Task taskAnswer;
        private bool taskAnswerIsRuning { get; set; }
        private BlockingCollection<byte[]> reciveQueue;
        public void AppendBuffer(byte[] buffer)
        {
            if (!this.taskAnswerIsRuning)
                return;
            if (reciveQueue.IsAddingCompleted)
                return;
            if (reciveQueue.Count > 4096)
                return;
            foreach(byte[] curBC in reciveQueue)
            {
                // 17位读写标识，不重复添加相同操作类型
                if (curBC[17] == buffer[17])
                    return;
            }
            //不重复添加
            if (reciveQueue.Contains(buffer))
                return;
            reciveQueue.Add(buffer);
        }
        private void SendPLCOnce(byte[] buffer)
        {
            try
            {
                byte[] bc = GetPLCBuffer(buffer);
                if (bc != null)
                {
                    if (this.clienManager.SendData(this.client, bc, out string errMsg))
                    {
                        AddLog("应答数据发送成功---" + BytesToText(bc, bc.Length));
                    }
                    else
                    {
                        AddLog("应答数据发送失败:" + errMsg + "---" + BytesToText(bc, bc.Length));
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
            }

        }

        // 接收数据
        private void StartRead()
        {
            if (readerIsRuning)
                return;
            this.clienManager.DataReceived += DataReceived;
        }
        private void StopRead()
        {
            if (!readerIsRuning)
                return;
            this.clienManager.DataReceived -= DataReceived;
        }
        private bool readerIsRuning { get; set; }
        private void DataReceived(object sender,byte[] data)
        {
            AddRecived("收到服务数据---" + BytesToText(data, data.Length));
            if (data.Length == 16)
            {
                byte[] bc = GetHeartBufferAnswer();
                if(BytesToText(data, data.Length)== BytesToText(bc, bc.Length))
                {
                    AddLog("注册成功");
                    return;
                }
            }
            if(data.Length ==24)
            {
                byte[] bc = GetPLCBufferAnswer();
                if (BytesToText(data,17) == BytesToText(bc, bc.Length))
                {
                    AddLog("服务器请求客户端数据");
                    // 加入回应队列
                    AppendBuffer(data);
                    return;
                }
            }

        }

        //命令展示区
        private void btnRefreshCommands_Click(object sender, EventArgs e)
        {
            if (!CheckTel())
            {
                ShowMessageAndAddLog("必须填写电话号码且电话只能为11位数字");
                return;
            }
            LoadCommandsDetail();
        }
        private void btn_creatSomeCommands_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"select t.StationID from PandaDTU t where t.终端登录号码='{0}'", this.textTel.Text.Trim());
            string stationID = DBUtil.ExecuteScalar(sql, out string errMsg).ToString();
            if (!string.IsNullOrEmpty(errMsg))
            {
                ShowMessageAndAddLog(errMsg);
                return;
            }
            if (string.IsNullOrEmpty(stationID))
            {
                ShowMessageAndAddLog("未查询到电话号码为" + this.textTel.Text+"的记录");
                return;
            }
            string insertSQL = "";
            for(int i = 1; i <=(int)this.numeric_read.Value; i++)
            {
                insertSQL += string.Format("INSERT INTO PandaCommand (type, sensorID,status) VALUES ( '读','{0}','未完成');", i.ToString()+"-"+stationID);
            }
            for (int i = 1; i <= (int)this.numeric_write.Value; i++)
            {
                insertSQL += string.Format("INSERT INTO PandaCommand (type, sensorID,sensorValue,status) VALUES ( '写','{0}',{1}, '未完成');", i.ToString() + "-" + stationID,i);
            }
            int rows = DBUtil.ExecuteNonQuery(insertSQL,out errMsg);
            if (rows > 0 && errMsg == "")
            {
                ShowMessageAndAddLog(string.Format( "已成功生成【{0}】张读操作票和【{1}】张写操作票", 
                    this.numeric_read.Value.ToString(), this.numeric_write.Value.ToString()),MessageBoxIcon.Information);
                LoadCommandsDetail();
            }
            else
                ShowMessageAndAddLog("操作票生成失败");
        }
        private void btnDelCommands_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"select t.StationID from PandaDTU t where t.终端登录号码='{0}'", this.textTel.Text.Trim());
            string stationID = DBUtil.ExecuteScalar(sql, out string errMsg).ToString();
            if (!string.IsNullOrEmpty(errMsg))
            {
                ShowMessageAndAddLog(errMsg);
                return;
            }
            if (string.IsNullOrEmpty(stationID))
            {
                ShowMessageAndAddLog("未查询到电话号码为" + this.textTel.Text + "的记录");
                return;
            }
            //不能删除服务正在执行的任务
            string delSQL = string.Format(@"delete  PandaCommand where sensorID in (
                              select t.sensorID from PandaCommand t
                              left join Sensor tt on tt.ID=t.sensorID
                              where tt.StationID='{0}' and t.status  in ('未完成','任务成功','任务失败','任务超时'))", stationID);

            int rows = DBUtil.ExecuteNonQuery(delSQL, out errMsg);
            if (rows >= 0 && errMsg == "")
            {
                ShowMessageAndAddLog(string.Format("已成功删除{0}张操作票", rows.ToString()));
                LoadCommandsDetail();
            }
            else
                ShowMessageAndAddLog("操作票删除失败");
        }
        private void LoadCommandsDetail()
        {
            this.dataGridView_commands.Rows.Clear();
            string sql = string.Format(@"  select ts.ID,ts.type as '类型' ,ts.sensorID,ts.sensorValue,ts.status as '任务状态',
                          CONVERT(varchar(100),ts.BeginTime, 120) as 开始时间, 
                          CONVERT(varchar(100),ts.EndTime, 120)  as 结束时间,ts.耗时,ts.Mess as 备注
                          from PandaCommand ts where sensorID in (
                          select ss.ID from Sensor  ss where ss.StationID in(
                          select t.StationID  from PandaDTU t where t.终端登录号码='{0}'))", this.textTel.Text.Trim());
            DataTable dt = DBUtil.ExecuteDataTable(sql, out string errMsg);
            if (!string.IsNullOrEmpty(errMsg))
            {
                ShowMessageAndAddLog(errMsg);
                return;
            }
            foreach(DataRow dr in dt.Rows)
            {
                DataGridViewRow row = new DataGridViewRow();
                switch (dr["任务状态"].ToString())
                {
                    case "任务成功":
                            row.DefaultCellStyle.BackColor = Color.LightGreen;
                        break;
                    case "任务失败":
                    case "任务超时":
                        row.DefaultCellStyle.BackColor = Color.Red;
                        break;
                }
                int index = dataGridView_commands.Rows.Add(row);
                dataGridView_commands.Rows[index].Cells[0].Value = dr["ID"].ToString();
                dataGridView_commands.Rows[index].Cells[1].Value = dr["类型"].ToString();
                dataGridView_commands.Rows[index].Cells[2].Value = dr["sensorID"].ToString();
                dataGridView_commands.Rows[index].Cells[3].Value = dr["sensorValue"].ToString();
                dataGridView_commands.Rows[index].Cells[4].Value = dr["任务状态"].ToString();
                dataGridView_commands.Rows[index].Cells[5].Value = dr["开始时间"].ToString();
                dataGridView_commands.Rows[index].Cells[6].Value = dr["结束时间"].ToString();
                dataGridView_commands.Rows[index].Cells[7].Value = dr["耗时"].ToString();
                dataGridView_commands.Rows[index].Cells[8].Value = dr["备注"].ToString();


            }

        }
        Point pt;
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
         //   pt = Cursor.Position;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void textBoxLog_MouseDown(object sender, MouseEventArgs e)
        {
            pt = Cursor.Position;
        }

        private void textBoxLog_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int px = Cursor.Position.X - pt.X;
                int py = Cursor.Position.Y - pt.Y;
                panel1.Location = new Point(panel1.Location.X + px, panel1.Location.Y + py);
                pt = Cursor.Position;
            }
        }
        private void btnSendOncePLCData_Click(object sender, EventArgs e)
        {
            if (!this.CheckServerConnect())
                return;
            //开始循环调用Dll拿所有DTU方法去更新所有的状态
            this.AddLog("PLC模拟数据定时发送已打开");
            this.testOncePLC();
            this.RefreshText();
        }
    }
}
