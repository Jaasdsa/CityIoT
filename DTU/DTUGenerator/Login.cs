using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DTUGenerator
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private bool IsConnected { get; set; }
        private bool EnvISOk { get; set; }

        private void Login_Load(object sender, EventArgs e)
        {
            this.labelVerssion.Text = Config.Version;

            // 环境检查
            this.EnvISOk = EnvChecker.Check(out string errMsg);
            if (!this.EnvISOk)
            {
                ShowErrMessBox("系统环境异常：" + errMsg);
                return;
            }
            if (!this.EnvISOk)
                return;

            StartLoadDtus();
        }
        private void StartLoadDtus()
        {
            string sql = @"select t.终端登录号码,t.pumpName from(
                         select a.终端登录号码,c.PName pumpName ,ROW_NUMBER() over( partition by c.PName  order by c.PName asc ) r
                         from DTUBase a,PumpJZ b,pump c
                         where a.是否使用=1 and b.DTUCode=a.终端登录号码 and b.PumpId=c.ID and b.PumpJZReadMode='DTU') t
                         where t.r=1;";
            DataTable dt = DBUtil.ExecuteDataTable(sql, out string err);
            if (!string.IsNullOrEmpty(err))
            {
                ShowErrMessBox(err);
                return;
            }
            List<LoginInfo> dtuIDs = new List<LoginInfo>();
            foreach(DataRow dr in dt.Rows)
            {
                LoginInfo dtu=new LoginInfo() { DTUID = DataUtil.ToString(dr["终端登录号码"])  ,PumpName = DataUtil.ToString(dr["pumpName"]) };
                dtuIDs.Add(dtu);
            }
            RenderDtuCombox(dtuIDs.ToArray());
        }

        private void RenderDtuCombox(LoginInfo[] dtus)
        {
            foreach(LoginInfo dtu in dtus)
            {
                this.comboBoxDTUS.Items.Add(dtu);
            }

            if (this.comboBoxDTUS.Items.Count > 0)
                this.comboBoxDTUS.SelectedIndex = 0;
        }

        //  消息展示区
        private void ShowErrMessBox(string mess)
        {
            MessageBox.Show(mess, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        PumpSimulate pumpSimulate;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.OpenPuumView();
        }
        private void OpenPuumView()
        {
            if (!this.EnvISOk)
                return;            

            LoginInfo info = new LoginInfo();
            info =(LoginInfo)( this.comboBoxDTUS.SelectedItem);
            info.IP = this.textYuMing.Text;
            info.Port = DataUtil.ToInt(this.numericPort.Value);

            if (pumpSimulate != null)
            {
                pumpSimulate.Dispose();
            }
            pumpSimulate = new PumpSimulate(info, this);
            this.Hide();
            pumpSimulate.Show();
        }

        //private bool CheckTel()
        //{
        //    // string tel = this.textTel.Text.Trim();
        //    string tel = "";
        //    if (tel.Length != 11)
        //        return false;
        //    try
        //    {
        //        foreach (char i in tel)
        //        {
        //            int ii = Convert.ToInt32(i.ToString());
        //            if (ii < 0 || ii > 9)
        //                return false;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}
    }
}
