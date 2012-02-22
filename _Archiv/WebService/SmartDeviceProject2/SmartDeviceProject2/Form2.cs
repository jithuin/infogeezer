using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartDeviceProject2
{
	public partial class Form2 : Form
	{
		public Form2()
		{
			InitializeComponent();
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void menuItem3_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}