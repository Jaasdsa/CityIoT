using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CityPublicClassLib;

namespace CityIoTServiceWatcher
{
    public partial class DBConf : UserControl
    {
        public DBConf()
        {
            InitializeComponent();
        }

        EnvConfigInfo config = EnvConfigInfo.SingleInstanceForCS;

        private void DBConf_Load(object sender, EventArgs e)
        {
            txt_IP.Text = config.confDataBaseInfo.IP;
            txt_UserName.Text = config.confDataBaseInfo.User;
            txt_Password.Text = config.confDataBaseInfo.Password;
            comboBox_DB.Text = config.confDataBaseInfo.ServerName;

            if (!config.EnvIsOkay)
            {
                btn_Test.Enabled = false;
                btn_Save.Enabled = false;
            }

            ReadDBHistory();
        }

        private void ReadDBHistory()
        {
            ClearHistory();
            List<CityUtils.ConnectionItem> list = config.GetDBHistroyList(out string errMsg);
            foreach(CityUtils.ConnectionItem item in list)
            {
                DataGridViewRow row = new DataGridViewRow();
                int index = DGV_History.Rows.Add(row);
                DGV_History.Rows[index].Cells["ID"].Value = index + 1;
                DGV_History.Rows[index].Cells["HostName"].Value = item.ip;
                DGV_History.Rows[index].Cells["DBName"].Value = item.database;
                DGV_History.Rows[index].Cells["UserName"].Value = item.user;
                DGV_History.Rows[index].Cells["PassWord"].Value = item.password;
                DGV_History.Rows[index].Cells["SaveDate"].Value = item.saveTime;
            }
        }

        private void ClearHistory()
        {
            while (DGV_History.Rows.Count != 0)
            {
                DGV_History.Rows.RemoveAt(0);
            }
        }

        //数据库下拉列表
        private void comboBox_DB_DropDown(object sender, EventArgs e)
        {
            ConfDataBaseInfo info = new ConfDataBaseInfo();

            if (string.IsNullOrWhiteSpace(txt_IP.Text))
            {
                MessageBox.Show("请输入服务器名!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_UserName.Text))
            {
                MessageBox.Show("请输入登录名!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_Password.Text))
            {
                MessageBox.Show("请输入密码!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            info.IP = txt_IP.Text;
            info.User = txt_UserName.Text;
            info.Password = txt_Password.Text;

            List<string> list = config.GetDBList(info, out string errMsg);

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                MessageBox.Show("获取数据库列表失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                comboBox_DB.Items.Clear();
                foreach(string name in list)
                {
                    comboBox_DB.Items.Add(name);
                }
                comboBox_DB.SelectedItem = Config.envConfigInfo.confProjectInfo.ServerName;
            }
        }

        //测试连接
        private void btn_Test_Click(object sender, EventArgs e)
        {
            ConfDataBaseInfo info = new ConfDataBaseInfo();

            if (comboBox_DB.Text == "")
            {
                MessageBox.Show("请选择数据库名", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            info.IP = txt_IP.Text;
            info.User = txt_UserName.Text;
            info.Password = txt_Password.Text;
            info.ServerName = comboBox_DB.Text;

            if (config.CheckDBConnect(info, out string errMsg))
            {
                MessageBox.Show("测试连接成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("测试连接失败!," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //保存连接-1：修改配置文件信息；2、存入日志 数据库历史信息中
        private void btn_Save_Click(object sender, EventArgs e)
        {
            ConfDataBaseInfo info = new ConfDataBaseInfo();

            if (comboBox_DB.Text == "")
            {
                MessageBox.Show("请选择数据库名", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            info.IP = txt_IP.Text;
            info.User = txt_UserName.Text;
            info.Password = txt_Password.Text;
            info.ServerName = comboBox_DB.Text;

            if(config.UpdateDBInfo(info, out string errMsg))
            {
                if(config.SaveDBInfo(info, out errMsg))
                {
                    MessageBox.Show("保存连接成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ReadDBHistory();
                }
                else
                {
                    MessageBox.Show("保存连接历史信息失败!," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("保存连接信息失败!," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //清除历史
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            if(config.ClearDBHistoryInfo(out string errMsg))
            {
                MessageBox.Show("清除历史成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearHistory();
            }
            else
            {
                MessageBox.Show("清除历史失败!," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //快速切换数据库配置 / 删除
        private void DGV_History_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ConfDataBaseInfo info = new ConfDataBaseInfo();
            int curIndex = e.RowIndex;
            DGV_History.Rows[curIndex].Selected = true;
            info.IP = Convert.ToString(DGV_History.Rows[curIndex].Cells["HostName"].Value);
            info.ServerName = Convert.ToString(DGV_History.Rows[curIndex].Cells["DBName"].Value);
            info.User = Convert.ToString(DGV_History.Rows[curIndex].Cells["UserName"].Value);
            info.Password = Convert.ToString(DGV_History.Rows[curIndex].Cells["PassWord"].Value);

            txt_IP.Text = info.IP;
            txt_UserName.Text = info.User;
            txt_Password.Text = info.Password;
            comboBox_DB.Text = info.ServerName;

            if (e.ColumnIndex == 6)
            {
                if (config.DeleteSignleDBHistory(info, out string errMsg))
                {
                    MessageBox.Show("清除历史成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DGV_History.Rows.RemoveAt(curIndex);
                }
                else
                {
                    MessageBox.Show("清除历史失败!," + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
