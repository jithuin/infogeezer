using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsMobile.PocketOutlook;
using System.Net;
using System.IO;
using Microsoft.WindowsCE.Forms;
using SmartDeviceProject1.Properties;

namespace SmartDeviceProject1
{
    public partial class FormBrowser : Form
    {
        string prevUrl,currUrl;
        public FormBrowser()
        {
            InitDevice();
            InitializeComponent();
        }

        private void InitDevice()
        {
            prevUrl = "";
            currUrl = "";
        }

        private string ParseUrl(string Url)
        {
            if (!Url.StartsWith("http://"))
            {
                Url = "http://" + Url;
            }
            return Url;
        }
        private string GetWebPage(string Url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd();

            }
            catch (UriFormatException ex)
            {
                MessageBox.Show("Wrong Url", "Error");
            }
            catch (WebException ex)
            {
                MessageBox.Show("Web Error", "Error");
            }
            return "";
        }
        private StreamReader GetWebFile(string Url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                return new StreamReader(resp.GetResponseStream());
            }
            catch (UriFormatException ex)
            {
                MessageBox.Show("Wrong Url", "Error");
            }
            catch (WebException ex)
            {
                MessageBox.Show("Web Error", "Error");
            }
            return StreamReader.Null;
        }
        private string ReadFile(StreamReader fileReader)
        {
            StringBuilder sb = new StringBuilder(1000);
            try
            {
                if (fileReader != StreamReader.Null)
                {
                    for (int i = 0; (i = fileReader.Read()) > -1; )
                    {
                        if (i == 0) i = 20;
                        sb.Append((char)i);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in File Read");
            }
            return sb.ToString();
        }


        private void miExit_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            this.Close();
        }

        private void miAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.SwVersion, "SW Version");
        }


        private void miSoft_Click(object sender, EventArgs e)
        {
            switch (this.miSoft.Text)
            {
                case "OK":
                    {
                        this.Go();
                        break;
                    }
                default:
                    { 
                        break; 
                    }
            }   
        }

        private void Go()
        {
            prevUrl = currUrl;
            currUrl = this.tbSiteAddres.Text;
            try
            {
                string Url = this.tbSiteAddres.Text = this.ParseUrl(this.tbSiteAddres.Text);
                if (Url.EndsWith("bmp") || Url.EndsWith("jpg") || Url.EndsWith("gif"))
                    this.tbSiteBody.Text = ReadFile(GetWebFile(Url));
                else
                    this.tbSiteBody.Text = GetWebPage(Url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown Error");
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // Up
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // Down
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                // Enter
            }

        }

        private void miBack_Click(object sender, EventArgs e)
        {
            this.tbSiteAddres.Text = prevUrl;
            Go();
        }
    }
}