using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TActionProject
{
    public partial class PagerControl : UserControl
    {
        #region 构造函数

        public PagerControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 分页字段和属性

        private int pageIndex = 1;
        /// <summary>
        /// 当前页面
        /// </summary>
        public virtual int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }

        private int pageSize = 100;
        /// <summary>
        /// 每页记录数
        /// </summary>
        public virtual int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        private int recordCount = 0;
        /// <summary>
        /// 总记录数
        /// </summary>
        public virtual int RecordCount
        {
            get { return recordCount; }
            set
            {
                recordCount = value;
                DrawControl(false, -1);
            }
        }

        private int pageCount = 0;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (pageSize != 0)
                {
                    pageCount = GetPageCount();
                }
                return pageCount;
            }
        }

        #endregion

        #region 页码变化触发事件

        public event EventHandler OnPageChanged;

        #endregion

        #region 分页及相关事件功能实现

        private void SetFormCtrEnabled()
        {
            btnFirst.Enabled = true;
            btnPrev.Enabled = true;
            btnNext.Enabled = true;
            btnLast.Enabled = true;
            btnGo.Enabled = true;
        }

        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <returns></returns>
        private int GetPageCount()
        {
            if (PageSize == 0)
            {
                return 0;
            }
            int pageCount = RecordCount / PageSize;
            if (RecordCount % PageSize == 0)
            {
                pageCount = RecordCount / PageSize;
            }
            else
            {
                pageCount = RecordCount / PageSize + 1;
            }
            return pageCount;
        }

        /// <summary>
        /// 页面控件呈现
        /// </summary>
        private void DrawControl(bool callEvent, int p)
        {
            //设置页码
            if (callEvent) pageIndex = p;

            txtCurrentPage.Text = PageIndex.ToString();
            lblState.Text = string.Format("总共{0}条记录，当前第{1}页，共{2}页，每页{3}条记录", RecordCount, PageIndex, PageCount, PageSize);

            if (pageIndex != -1 && callEvent && OnPageChanged != null)
            {
                OnPageChanged(this, null);//当前分页数字改变时，触发委托事件
            }
            SetFormCtrEnabled();
            if (PageCount == 1)//有且仅有一页
            {
                btnFirst.Enabled = false;
                btnPrev.Enabled = false;
                btnNext.Enabled = false;
                btnLast.Enabled = false;
                btnGo.Enabled = false;
            }
            else if (PageIndex == 1)//第一页
            {
                btnFirst.Enabled = false;
                btnPrev.Enabled = false;
            }
            else if (PageIndex == PageCount)//最后一页
            {
                btnNext.Enabled = false;
                btnLast.Enabled = false;
            }
        }
        #endregion

        #region 相关控件事件


        private void lnkFirst_LinkClicked(object sender, EventArgs e)
        {
            DrawControl(true, 1);
        }

        private void lnkPrev_LinkClicked(object sender, EventArgs e)
        {
            DrawControl(true, Math.Max(1, PageIndex - 1));
        }

        private void lnkNext_LinkClicked(object sender, EventArgs e)
        {
            DrawControl(true, Math.Min(PageCount, PageIndex + 1));
        }

        private void lnkLast_LinkClicked(object sender, EventArgs e)
        {
            DrawControl(true, PageCount);
        }

        /// <summary>
        /// enter键功能
        /// </summary>
        private void txtCurrentPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                btnGo_Click(null, null);
            }
        }

        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGo_Click(object sender, EventArgs e)
        {
            int num = 0;
            if (int.TryParse(txtCurrentPage.Text.Trim(), out num) && num > 0 && num <= PageCount)
            {
                DrawControl(true, num);
            }
        }
        #endregion


    }
}