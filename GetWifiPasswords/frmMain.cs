using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetWifiPasswords
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            lvList.View = View.Details;
            lvList.Columns.Add("SSID");
            lvList.Columns.Add("Password");
            lvList.Columns[0].Width= (lvList.Width / 2) - 20;
            lvList.Columns[1].Width = (lvList.Width / 2) - 20;
            lvList.FullRowSelect = true;          
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            foreach (string ssid in getSSIDs())
            {
                string pwd = getPassword(ssid);
                ListViewItem item = new ListViewItem(ssid);
                item.SubItems.Add(pwd);
                lvList.Items.Add(item);
            }
            lblLoading.Visible = false;
        }
        private void frmMain_Resize(object sender, EventArgs e)
        {
            lvList.Width = this.Width - 50;
            lvList.Height = this.Height - 65;
            lvList.Columns[0].Width = (lvList.Width / 2) - 20;
            lvList.Columns[1].Width = (lvList.Width / 2) - 20;
        }
        private List<string> getSSIDs()
        {
            List<string> list = new List<string>();
            string output = "";

            Process process = new Process();
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = "wlan show profiles";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            output = process.StandardOutput.ReadToEnd();
            foreach (var s in output.Split('\n'))
            {
                if(s.IndexOf("   :") > 5)
                {
                    var ssid = s.Substring(s.IndexOf(":") +1).Replace("\r","").Trim();
                    list.Add(ssid);
                }                
            }

            process.WaitForExit();
            return list;
        }
        private string getPassword(string ssid)
        {
            string output = "";
            string pwd = "";

            Process process = new Process();
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = "wlan show profile name=\"" + ssid + "\" key=clear";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            output = process.StandardOutput.ReadToEnd();
            if (output.IndexOf("Key Content") > 2)
            {
                int s = output.IndexOf("Key Content") + 11;
                output = output.Substring(s);
                int e = output.IndexOf("\r");
                output = output.Substring(2, e-1);
                pwd = output.Replace("\r", "").Trim().Replace(":","");
            }
            
            process.WaitForExit();
            return pwd;
        }

        private void lvList_DoubleClick(object sender, EventArgs e)
        {
            if(lvList.SelectedIndices.Count == 1)
                Clipboard.SetText(lvList.SelectedItems[0].SubItems[1].Text);
        }
    }
}
