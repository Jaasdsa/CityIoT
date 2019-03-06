using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CityIoTServiceWatcher
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            this.lab_version.Text = Config.Version;
            this.lab_versionTime.Text = Config.VersionLastTime;

            this.lab_serverVersion.Text = Config.ServerVersion;
            this.lab_serverVerSionTime.Text = Config.ServerVersionLastTime;
        }
    }
}
