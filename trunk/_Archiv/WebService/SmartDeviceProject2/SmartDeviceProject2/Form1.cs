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
	public partial class Form1 : Form
	{
		int i;
		public Form1()
		{
			InitializeComponent();
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			i++;
			switch (i)
			{
				case 1:

					this.webBrowser1.DocumentText = "ez a <b>document text</b>";

					break;
				case 2:
					this.webBrowser1.DocumentText = "ez a <i>document text</i>";

					break;
				default:
					this.webBrowser1.DocumentText = "";
					i = 0;
					break;
			}
		}

		private void menuItem4_Click(object sender, EventArgs e)
		{
			Form2 f2 = new Form2();
			f2.ShowDialog();
		}
	}
}