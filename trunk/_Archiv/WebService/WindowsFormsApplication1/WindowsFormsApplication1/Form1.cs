using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btGo_Click(object sender, EventArgs e)
        {
					//webService1.WebService1 service = new WindowsFormsApplication1.webService1.WebService1();
					//String text = service.ReadLog();
					//tbBody.Text = text;

					//String s = service.ReadLog();
					//String[] lines = s.Split('\n');
					//foreach (String line in lines)
					//  line.TrimEnd('\r');
					//tbBody.Lines = lines;

					try
					{
						string Url = tbUrl.Text = ParseUrl(tbUrl.Text);
						if (Url.EndsWith("bmp") || Url.EndsWith("jpg") || Url.EndsWith("gif"))
							this.tbBody.Text = ReadFile(GetWebFile(Url));
						else
						{
							this.tbBody.Text = GetWebPage(Url);
							this.wbBody.Url = new Uri(Url);
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}

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
    }
}
