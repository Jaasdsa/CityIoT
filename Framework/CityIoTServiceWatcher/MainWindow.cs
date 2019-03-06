using CityUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using CityIoTServiceManager;
using System.Collections.Concurrent;
using CityPublicClassLib;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace CityIoTServiceWatcher
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool EnvIsOkay = true;
        private void MainWindow_Load(object sender, EventArgs e) 
        {
            if (!EnvChecker.Check(out string warnMsg))
            {
                ShowMessage(warnMsg, MessageBoxIcon.Warning);
                EnvIsOkay = false;
            }
            // 配置更改通知
            Config.envConfigInfo.evtConfigIsChanged += RefreshControlState;

            StartPrint();
            RefreshControlState();

            // 判断服务是否在运行
            if (new ServiceManager(Config.envConfigInfo).IsCoreServiceRun())
                StartLogSubscriber();  //开始订阅日志
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopRealTime();
            StopLogSubscriber();
            StopPrint();

            // 配置更改通知
            Config.envConfigInfo.evtConfigIsChanged -= RefreshControlState;
        }

        // 实时显示时间
        private void realtimer_Tick(object sender, EventArgs e)
        {
            this.toolStripTime.Text = DataUtil.ToDateString(DateTime.Now);
        }
        private void StopRealTime()
        {
            this.realtimer.Enabled = false;
            this.realtimer.Dispose();
            this.realtimer = null;
        }
        private string GetTitleText()
        {
            return "熊猫智慧水务数据服务管理器" + "--" + Config.envConfigInfo.confProjectInfo.ProjectName;
        }

        // 工具栏事件
        private void iStart_Click(object sender, EventArgs e)
        {
            this.IsStartHander();
        }
        private void isStop_Click(object sender, EventArgs e)
        {
            this.IsStopHander();
        }
        private void IsRestart_Click(object sender, EventArgs e)
        {
            this.IsRestartHander();
        }
        private void isInstal_Click(object sender, EventArgs e)
        {
            this.IsInstalHander();
        }
        private void isUnInstal_Click(object sender, EventArgs e)
        {
            this.IsUnInstalHander();
        }
        private void iClear_Click(object sender, EventArgs e)
        {
            this.IsClearTextHander();
        }

        // 菜单栏事件
        private About formAbout;
        private void isAbout_Click(object sender, EventArgs e)
        {
            if (formAbout != null)
            {
                formAbout.Dispose();
            }
            formAbout = new About();
            formAbout.Show(this);
        }
        private void ProductHelp_Click(object sender, EventArgs e)
        {
            try
            {
                this.helpProviderForProduct.HelpNamespace = Application.StartupPath + @"\HelpDoc\产品帮助.chm";
                Help.ShowHelp(this, this.helpProviderForProduct.HelpNamespace);
            }
            catch(Exception ex)
            {
                ShowMessage("打开产品帮助文档失败,"+ex.Message, MessageBoxIcon.Warning);
            }
        }
        // 注册服务
        private void IsInstalHander()
        {
            ServiceManager manager = new ServiceManager(Config.envConfigInfo);
            string info = manager.IsInstalCoreHander(out string statusCode, out string errMsg);
            if (statusCode == "0000")
                ShowMessage(info, MessageBoxIcon.Information);
            else
                ShowMessage(errMsg, MessageBoxIcon.Error);
            RefreshControlState();
        }
        // 卸载服务
        private void IsUnInstalHander()
        {
            ServiceManager manager = new ServiceManager(Config.envConfigInfo);
            string info = manager.IsUnInstalCoreHander(out string statusCode, out string errMsg);
            if (statusCode == "0000")
                ShowMessage(info, MessageBoxIcon.Information);
            else
                ShowMessage(errMsg, MessageBoxIcon.Error);
            RefreshControlState();
        }
        // 启动服务
        private void IsStartHander()
        {
            // 先检查一波日志环境，防止错误消息看不到
            ServiceManager manager = new ServiceManager(Config.envConfigInfo);
            if (!Config.envConfigInfo.EnvCheckForBeforeRun(out string errMsg))
            {
                ShowMessage(errMsg, MessageBoxIcon.Error);
                return;
            }
            int beginNumber = LoadlogsLastRowNuber();
            string info = manager.IsStartCoreHander(out string statusCode, out errMsg);
            if (statusCode == "0000")
                ShowMessage(info, MessageBoxIcon.Information);
            else
                ShowMessage(errMsg, MessageBoxIcon.Error);
            RefreshControlState();
            LoadAndAppendLogs(beginNumber);
            // 判断服务是否在运行
            if (manager.IsCoreServiceRun())
                StartLogSubscriber();  //开始订阅日志
        }
        // 停止服务
        private void IsStopHander()
        {
            ServiceManager manager = new ServiceManager(Config.envConfigInfo);
            string info = manager.IsStopCoreHander(out string statusCode, out string errMsg);
            if (statusCode == "0000")
                ShowMessage(info, MessageBoxIcon.Information);
            else
                ShowMessage(errMsg, MessageBoxIcon.Error);
            RefreshControlState();
        }
        // 重启服务
        private void IsRestartHander()
        {
            ServiceManager manager = new ServiceManager(Config.envConfigInfo);
            if (!manager.IsCoreServiceRun() && !Config.envConfigInfo.EnvCheckForBeforeRun(out string errMsg))
            {
                ShowMessage(errMsg, MessageBoxIcon.Error);
                return;
            }
            this.textBoxTrace.Clear();// 清空显示
            int beginNumber = LoadlogsLastRowNuber();
            string info = manager.IsRestartCoreHander(out string statusCode, out errMsg);
            if (statusCode == "0000")
                ShowMessage(info, MessageBoxIcon.Information);
            else
                ShowMessage(errMsg, MessageBoxIcon.Error);
            RefreshControlState();
            LoadAndAppendLogs(beginNumber);
            // 判断服务是否在运行
            if (manager.IsCoreServiceRun())
                StartLogSubscriber();  //开始订阅日志
        }
        // 清空日志
        private void IsClearTextHander()
        {
            this.textBoxTrace.Clear();
        }

        // 界面状态更新
        private LogSubscriber logSubscriber;
        public void RefreshControlState()
        {
            Config.ServerIsRuning = false;
            if (!EnvIsOkay)
            {
                this.isInstal.Enabled = false;
                this.isUnInstal.Enabled = false;
                this.iStart.Enabled = false;
                this.isStop.Enabled = false;
                this.IsRestart.Enabled = false;

                return;
            }

            if (ServiceToolEx.IsServiceExist(Config.envConfigInfo.confProjectInfo.ServerName))
            {
                // 工具栏
                this.isInstal.Enabled = false;
                this.isUnInstal.Enabled = true;
                this.IsRestart.Enabled = true;
                // 菜单栏

                if (ServiceToolEx.IsServiceRunning(Config.envConfigInfo.confProjectInfo.ServerName))
                {
                    this.toolStripStatusLabel4.Text = "正在运行";
                    this.toolStripStatusLabel4.ForeColor = Color.ForestGreen;
                    this.isStop.Enabled = true;
                    this.iStart.Enabled = false;

                    Config.ServerIsRuning = true;
                }
                else
                {
                    this.toolStripStatusLabel4.Text = "已停止";
                    this.toolStripStatusLabel4.ForeColor = Color.Red;
                    this.iStart.Enabled = true;
                    this.isStop.Enabled = false;
                }
            }
            else
            {
                this.toolStripStatusLabel4.Text = "未注册";
                this.isInstal.Enabled = true;
                this.isUnInstal.Enabled = false;
                this.iStart.Enabled = false;
                this.isStop.Enabled = false;
                this.IsRestart.Enabled = false;
            }
            this.textBoxTrace.BackColor = Color.FromArgb(0, 64, 0);
            this.textBoxTrace.ForeColor = Color.Lime;
            this.Text = GetTitleText();

            this.toolStripStatusProjectName.Text = Config.envConfigInfo.confProjectInfo.ProjectName;
        }
        private void ShowMessage(string mess, MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Warning:
                    MessageBox.Show(mess, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case MessageBoxIcon.Error:
                    MessageBox.Show(mess, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case MessageBoxIcon.Information:
                    MessageBox.Show(mess, "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }
        // 日志订阅器
        private void StartLogSubscriber()
        {
            // 防止重复打开
            if (logSubscriber != null && this.logSubscriber.IsRuning)
                StopLogSubscriber();

            this.logSubscriber = new LogSubscriber();
            logSubscriber.Start(this.AppendLog, out string errMsg);
            if (logSubscriber.IsRuning)
                AppendLog(new TraceItem(TraceType.Info, ServerTypeName.Watcher, "已连接日志服务器").ToLog());
            else
                AppendLog(new TraceItem(TraceType.Error, ServerTypeName.Watcher, "连接日志服务器失败;" + errMsg).ToLog());
        }
        private void StopLogSubscriber()
        {
            if (logSubscriber != null)
            {
                if (this.logSubscriber.IsRuning)
                    this.logSubscriber.Stop();
            }
        }

        // 打印工作者
        private BlockingCollection<Log> logQueue;
        private Task printTask;
        private bool printTaskIsRuning;
        private int maxLogLength = 1024 * 1024; //1M
        private void StartPrint()
        {
            if (printTaskIsRuning)
                return;

            logQueue = new BlockingCollection<Log>();
            printTask = new Task(() =>
            {
                foreach (Log log in logQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        Print(log);
                    }
                    catch { }
                }

            }, TaskCreationOptions.LongRunning);
            printTask.Start();

            printTaskIsRuning = true;
        }
        private void StopPrint()
        {
            if (!printTaskIsRuning)
                return;

            // 先完成添加
            logQueue.CompleteAdding();
            DateTime time1 = DateTime.Now;
            while (logQueue.Count > 0)
            {
                Thread.Sleep(1);
                // 最多等待10秒避免关不掉
                if (DateTime.Now - time1 > TimeSpan.FromSeconds(10))
                {
                    // TraceManager.AppendErrMsg("命令消费器关闭超时丢弃了" + queue.Count.ToString() + "条任务");
                    break;
                }
            }
            while (logQueue.Count > 0)
            {
                // 等了十秒还没听，队列全部元素废弃
                logQueue.Take();
            }

            logQueue = null;
            Task.WaitAll(printTask);
            printTask.Dispose();
            printTask = null;

            printTaskIsRuning = false;
        }
        private void AppendLog(Log log)
        {
            if (!printTaskIsRuning)
                return;
            if (logQueue.IsAddingCompleted)
                return;
            if (logQueue.Count > 4096)
            {
                return;
            }
            logQueue.Add(log);
        }
        private void Print(Log log)
        {
            // 保证render的时间必须短。否则界面会假死
            Color color = EnvChecker.GetColor(TraceItem.ToTraceItem(log).type);
            string mess = log.serverName + "   " + DataUtil.ToDateString(log.dateTime) + " " + log.text;
            this.BeginInvoke(new Action(() => {
                Render(color, mess);
            }));
        }
        private void Render(Color color, string message)
        {
            int textLength = textBoxTrace.TextLength;
            if (textLength > maxLogLength)
                textBoxTrace.Clear();

            textBoxTrace.SuspendLayout();
            textBoxTrace.SelectionStart = textBoxTrace.TextLength;
            textBoxTrace.SelectionColor = color;
            textBoxTrace.SelectedText = message + "\r\n";
            textBoxTrace.ResumeLayout();
            textBoxTrace.ScrollToCaret();
        }


        // 加载启动服务的历史日志--日志是记录在文件里面
        private int LoadlogsLastRowNuber()
        {
            if (!File.Exists(Config.envConfigInfo.confLogServiceInfo.filePath))
                return 0;
            try
            {
                SQLiteHelper sqlite = new SQLiteHelper(Config.envConfigInfo.confLogServiceInfo.filePath);
                string sql = string.Format(@"select seq from sqlite_sequence where name = '{0}'", Config.envConfigInfo.confLogServiceInfo.tableName);
                return DataUtil.ToInt(sqlite.ExecuteScalar(sql));
            }
            catch (Exception e)
            {
                ShowMessage("读取最新日志索引失败:" + e.Message, MessageBoxIcon.Error);
                return -1;
            }
        }
        private void LoadAndAppendLogs(int beginID)
        {
            if (beginID == -1)
                return;
            if (!File.Exists(Config.envConfigInfo.confLogServiceInfo.filePath))
                return;
            try
            {
                SQLiteHelper sqlite = new SQLiteHelper(Config.envConfigInfo.confLogServiceInfo.filePath);
                string sql = string.Format(@"SELECT ID,
                                       类型,
                                       信息文本,
                                       插入时间,
                                       系统名称
                                  FROM {0} where id> {1};", Config.envConfigInfo.confLogServiceInfo.tableName, beginID);
                DataTable dt = sqlite.ExecuteDataTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    Log log = new Log()
                    {
                        type = DataUtil.ToString(dr["类型"]),
                        text = DataUtil.ToString(dr["信息文本"]),
                        dateTime = DataUtil.ToDateString(dr["插入时间"]),
                        serverName = DataUtil.ToString(dr["系统名称"]),
                    };
                    this.AppendLog(log);
                }
            }
            catch (Exception e)
            {
                ShowMessage("读取最新日志失败:" + e.Message, MessageBoxIcon.Error);
            }


        }
        LogView logView;
        private void iViewLog_Click(object sender, EventArgs e)
        {
            if (logView != null && logView.Visible)
            {
                logView.Close();
                logView.Dispose();
                logView = null;
            }
            else
            {
                logView = new LogView();
                logView.Show(this);
            }
        }

        //private ConfigSet formConfigSet;
        //private void configSet_Click(object sender, EventArgs e)
        //{
        //    if (formConfigSet != null)
        //    {
        //        formConfigSet.Dispose();
        //    }
        //    formConfigSet = new ConfigSet();
        //    formConfigSet.Show(this);
        //}

        private ConfigCenter formConfigCenter;
        private void configSet_Click(object sender, EventArgs e)
        {
            if (formConfigCenter != null)
            {
                formConfigCenter.Dispose();
            }
            formConfigCenter = new ConfigCenter();
            formConfigCenter.Show(this);
        }

    }
}
