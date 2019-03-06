using CityPublicClassLib;
using CityUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CityIoTServiceWatcher
{
    public partial class LogView : Form
    {
        public LogView() 
        {
            InitializeComponent();
        }
        private bool logFileIsOkay = false;

        // 交互事件
        private void FormLog_Load(object sender, EventArgs e)
        {
            this.comboBoxServerType.Items.AddRange(new string[] { "调度器", "OPC", "DTU", "内核", "监视器", "二供报警", "web接入服务", "项目", "命令","历史抽稀" });
            this.comboBoxLogType.Items.AddRange(new string[] { "流水", "信息", "警告", "错误" });
            this.dateTimePickerStart.Value = DateTime.Now.AddDays(-1);

            this.pagerControl.OnPageChanged += new EventHandler(pagerControl_OnPageChanged);

            // 检查日志文件状态
            if(!Config.envConfigInfo.EnvIsOkay|| !Config.envConfigInfo.confLogServiceInfo.EnvIsOkay)
            {
                this.logFileIsOkay = false;
                return;
            }
            this.logFileIsOkay = true;
            this.SearchHander();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SearchHander();
        }

        // 加载渲染历史
        private void SearchHander()
        {
            if (!this.logFileIsOkay)
                return;

            this.listViewLog.Items.Clear();
            SQLiteHelper sqlite = new SQLiteHelper(Config.envConfigInfo.confLogServiceInfo.filePath);

            string startDate = DataUtil.ToDateString(new DateTime(dateTimePickerStart.Value.Year, dateTimePickerStart.Value.Month, dateTimePickerStart.Value.Day));
            string stopDate = DataUtil.ToDateString(dateTimePickerStop.Value);
            string serverType = comboBoxServerType.Text;
            string logType = comboBoxLogType.Text;

            int pageSize = this.pagerControl.PageSize;
            int pageIndex = this.pagerControl.PageIndex;

            string whereServerType = string.IsNullOrWhiteSpace(serverType) ? "" : " and 系统名称='" + serverType + "'";
            string whereLogType = string.IsNullOrWhiteSpace(logType) ? "" : " and 类型='" + logType + "'";
            string where = "插入时间>'" + startDate + "' and 插入时间<'" + stopDate + "'" + whereServerType + whereLogType;
            string sql = string.Format(@"select ID,类型,类型,信息文本,插入时间,系统名称 from {0} where {1} ", Config.envConfigInfo.confLogServiceInfo.tableName, where);

            DataTable dt = sqlite.GetPageQuery(sql, "ID", true, pageSize, pageIndex, out string err, out int totalNum);
            if (!string.IsNullOrWhiteSpace(err))
            {
                ShowMessage(err, MessageBoxIcon.Error);
                return;
            }
            if (dt.Rows.Count > 0)
                RenderTable(dt, totalNum);

        }
        private void RenderTable(DataTable dt, int totalNum)
        {
            FormLog_SizeChanged(null, null);
            this.pagerControl.RecordCount = totalNum;
            if (dt.Rows.Count == 0)
                return;

            List<ListViewItem> items = new List<ListViewItem>();
            foreach (DataRow dr in dt.Rows)
            {
                ListViewItem lvi = new ListViewItem();
                Log log = new Log()
                {
                    id = DataUtil.ToString(dr["ID"]),
                    serverName = DataUtil.ToString(dr["系统名称"]),
                    type = DataUtil.ToString(dr["类型"]),
                    dateTime = DataUtil.ToString(dr["插入时间"]),
                    text = DataUtil.ToString(dr["信息文本"])
                };
                // 文本绑定
                lvi.Tag = log.id;
                lvi.Text = log.id;
                lvi.SubItems.Add(log.serverName);
                lvi.SubItems.Add(log.dateTime);
                lvi.SubItems.Add(log.type);
                lvi.SubItems.Add(log.text);
                // 配色
                lvi.UseItemStyleForSubItems = false;
                TraceItem item = TraceItem.ToTraceItem(log);
                lvi.BackColor = EnvChecker.GetColor(TraceItem.ToTraceItem(log).type);
                items.Add(lvi);
            }
            this.Invoke(new Action(() => { this.listViewLog.Items.AddRange(items.ToArray()); }));
        }
        private void pagerControl_OnPageChanged(object sender, EventArgs e)
        {
            SearchHander();
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

        // 控件大小自适应
        private void FormLog_SizeChanged(object sender, EventArgs e)
        {
            if (this.listViewLog.Columns.Count > 0)
                this.listViewLog.Columns[this.listViewLog.Columns.Count - 1].Width = this.listViewLog.ClientSize.Width - this.listViewLog.Columns[0].Width;
        }

        // 右键复制信息文本
        private void listViewLog_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //filesList.ContextMenuStrip = contextMenuStrip1;
                //选中列表中数据才显示 空白处不显示

                if (this.listViewLog.SelectedItems.Count > 0)
                {
                    System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                    this.contextMenuCopy.Show(this.listViewLog, p);
                }
            }
        }
        private void ToolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            if (this.listViewLog.SelectedItems.Count != 1)
                return;
            var selectItem = this.listViewLog.SelectedItems[0];
            string text = "";
            try
            {
                text = selectItem.SubItems[selectItem.SubItems.Count - 1].Text;
                Clipboard.SetDataObject(text);
            }
            catch { }
        }

    }
}
