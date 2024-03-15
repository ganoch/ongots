using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Threading;

namespace PlaneOnPaper
{
    public partial class connectDialog : Form
    {
        private Dictionary<IPEndPoint, PlaneServerInfo> _infos;
        private BackgroundWorker worker = new BackgroundWorker();
        private bool do_work = true;

        public connectDialog(ref Dictionary<IPEndPoint, PlaneServerInfo> infos)
        {
            InitializeComponent();
            this._infos = infos;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            this.worker.RunWorkerAsync();
            this.FormClosing += new FormClosingEventHandler(connectDialog_FormClosing);
        }

        void connectDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.worker.CancelAsync();
            this.do_work = false;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (do_work)
            {
                Thread.Sleep(1000);
                this.LoadList();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            
        }

        private void connectDialog_Load(object sender, EventArgs e)
        {
            LoadList();
        }

        delegate void LoadListDelegate();

        private void LoadList()
        {
            if (this.listView1.InvokeRequired)
            {
                LoadListDelegate d = new LoadListDelegate(LoadList);
                this.Invoke(d);
            }
            else
            {
                //listView1.Items.Clear();
                bool changed = false;
                foreach (KeyValuePair<IPEndPoint, PlaneServerInfo> kvp in this._infos)
                {
                    bool skip = false;
                    foreach (ListViewItem item in this.listView1.Items)
                    {
                        if (((PlaneServerInfo)item.Tag).Name == kvp.Value.Name
                            && ((PlaneServerInfo)item.Tag).ServerEndPoint == kvp.Value.ServerEndPoint)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip) continue;

                    changed = true;
                    ListViewItem new_item = new ListViewItem(kvp.Value.Name);
                    new_item.SubItems.Add(kvp.Value.PlaneCount.ToString());
                    new_item.SubItems.Add(kvp.Value.ServerEndPoint.Address.ToString());
                    new_item.Tag = kvp.Value;
                    this.listView1.Items.Add(new_item);
                }

                if (changed)
                {
                    foreach (ColumnHeader column in this.listView1.Columns)
                    {
                        column.Width = -1;
                    }
                }
            }

            
        }
    }
}
