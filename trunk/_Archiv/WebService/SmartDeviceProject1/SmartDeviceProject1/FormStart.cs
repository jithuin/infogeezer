using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SmartDeviceProject1.Properties;

namespace SmartDeviceProject1
{
    public partial class FormStart : Form
    {
        public FormStart()
        {
            InitializeComponent();
            this.lbStartText.Text = "SW Version: " + Resources.SwVersion;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(Resources.SwVersion, "SW Version");
            Application.Exit();
        }

        private void miBrowser_Click(object sender, EventArgs e)
        {
            //Application.Run(new FormBrowser());
            FormBrowser form = new FormBrowser();
            form.Show();
        }

        private void miSlide_Click(object sender, EventArgs e)
        {
            //Application.Run(new FormSlide());
            FormSlide form = new FormSlide();
            form.ShowDialog();
        }
    }
}