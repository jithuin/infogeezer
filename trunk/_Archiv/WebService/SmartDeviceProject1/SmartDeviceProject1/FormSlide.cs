using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartDeviceProject1
{
    public partial class FormSlide : Form
    {
        public FormSlide()
        {
            InitializeComponent();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            label1.Text = vScrollBar1.Value.ToString();
            
        }
    }
}