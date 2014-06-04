using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DragDropFlag = false;
        }

        private bool DragDropFlag;
        //DragEventArgs dea;

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (DragDropFlag)
            {
                String text = ""
                    //   + "e.AllowedEffect: " + e.AllowedEffect.ToString() + "\r\n" +
                    //   "e.Data: " + e.Data.GetData(e.Data.GetFormats()[0]).ToString() + "\r\n" +
                    //   "e.Effect: " + e.Effect.ToString() + "\r\n" +
                    //   "e.KeyState: " + e.KeyState.ToString() + "\r\n" +
                    //   "e.X: " + e.X.ToString() + "\r\n" +
                    //   "e.Y: " + e.Y.ToString() + "\r\n"
                ;
                IDataObject dataObject = e.Data;
                DisplayDataObject(text, dataObject);
            }
        }

        public List<string> DenyList = new List<string>()
        {
            "EnhancedMetafile",
            "Notes Private Data",
            "Link Source",
        };

        private void DisplayDataObject(String textHeader, IDataObject dataObject)
        {
            string[] formats = dataObject.GetFormats();
            StringBuilder normalText = new StringBuilder(textHeader);
            StringBuilder richText = new StringBuilder();
            StringBuilder xmlText = new StringBuilder();
            bool isDataPresent;
            object data;
            string dataString;
            //const string XmlData = "XMLData";
            //if (dataObject.GetDataPresent(XmlData) && (data = dataObject.GetData(XmlData)) != null)
            //{
            //    dataString = data.ToString();
            //    sb.Append(XmlData + ": " + dataString + "\r\n");
            //    if (XmlData.ToLower().Contains("xml"))
            //    {
            //        System.IO.MemoryStream ms = (System.IO.MemoryStream)data;
            //        long position = ms.Position;
            //        //StreamReader sr = new StreamReader(ms);
            //        //String s = sr.ReadToEnd();
            //        //ms.Seek(0, SeekOrigin.Begin);
            //        xmlText.Append(ms.XmlDeserialize<Feed>().ToString());
            //        //richTextBox2.Rtf = ms.XmlDeserialize<Feed>().ToRtfString();
            //        ms.Position = position;
            //    }
            //}
            foreach (string format in formats)
            {
                try
                {
                    isDataPresent = dataObject.GetDataPresent(format);
                    // ???mért nem tudja konvertálni az enhanced metafile-t????
                    if (isDataPresent && !DenyList.Contains(format) && (data = dataObject.GetData(format)) != null)
                    {
                        dataString = data.ToString();
                        normalText.Append(format + ": " + dataString + "\r\n");

                        switch (dataString)
                        {
                            case "System.IO.MemoryStream":
                                {
                                    System.IO.MemoryStream ms = (System.IO.MemoryStream)data;
                                    int b;
                                    long position = ms.Position;
                                    b = DisplayMemory(normalText, ms);
                                    ms.Position = position;
                                    if (format.ToLower().Contains("xml"))
                                        xmlText.Append(ms.XmlDeserialize<Feed>().ToString());
                                    break;
                                }
                            case "System.String[]":
                                {
                                    foreach (string s2 in (System.String[])data)
                                    {
                                        normalText.Append(s2 + "\r\n");
                                    }
                                    break;
                                }
                            default:
                                {
                                    switch (format)
                                    {
                                        case "Rich Text Format":
                                            normalText.Append(format + ": " + dataString + "\r\n");
                                            richText.Append(dataString);
                                            break;
                                        case "Rich Text Format Without Objects":
                                        case "RTF As Text":
                                        case "System.String":
                                        case "UnicodeText":
                                        case "Text":
                                            normalText.Append(format + ": " + dataString + "\r\n");
                                            break;
                                        default:

                                            break;
                                    }
                                    break;
                                }
                        }
                        normalText.Append("\r\n");
                    }
                }
                catch (ExecutionEngineException ex)
                {
                    MessageBox.Show(String.Format("Cannot get format:{0}", format));
                    normalText.AppendFormat("Cannot get format:{0}", format);
                }
                catch (ExternalException ex)
                {
                    MessageBox.Show(String.Format("Cannot get format:{0}", format));
                    normalText.AppendFormat("Cannot get format:{0}", format);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Cannot get format:{0}", format));
                    normalText.AppendFormat("Cannot get format:{0}", format);
                }
            }
            this.textBox1.Text = "Hello World";
            textBox1.Text = normalText.ToString();
            richTextBox1.Rtf = richText.ToString();
            richTextBox2.Text = xmlText.ToString();
        }

        private int DisplayMemory(StringBuilder sb, System.IO.MemoryStream ms)
        {
            int b = 0;
            long DisplayLimit = ms.Length < 1000L ? ms.Length : 1000;

            for (; ms.Position != DisplayLimit; )
            {
                b = ms.ReadByte();
                if (b == 0)
                    sb.Append("\x1");
                else
                    sb.Append(((char)b).ToString());
            }
            return b;
        }

        private string DisplayMemory(System.IO.MemoryStream ms)
        {
            StringBuilder sb = new StringBuilder();
            int b = 0;
            long DisplayLimit = ms.Length < 1000L ? ms.Length : 1000;

            for (; ms.Position != DisplayLimit; )
            {
                b = ms.ReadByte();
                if (b == 0)
                    sb.Append("\x1");
                else
                    sb.Append(((char)b).ToString());
            }
            return sb.ToString();
        }

        //private void textBox1_DragOver(object sender, DragEventArgs e)
        //{
        //    this.Activate();
        //    this.textBox1.Focus();
        //    this.DragDropFlag = true;
        //}

        private void Form1_Resize(object sender, EventArgs e)
        {
            //textBox1.Height = (int)(this.Height * 0.7);
            //richTextBox1.Height = (int)(this.Height * 0.2);
        }

        private void textBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //if (this.DragDropFlag)
            //{
            //    this.DoDragDrop(this.dea.Data, this.dea.AllowedEffect);
            //}
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            this.Activate();
            this.textBox1.Focus();
            this.DragDropFlag = true;
            //dea = e;
            if ((e.AllowedEffect & DragDropEffects.Copy) != DragDropEffects.None && this.cbEnableDrop.Checked)
                e.Effect = DragDropEffects.Copy;
        }

        private void textBox1_DragLeave(object sender, EventArgs e)
        {
            this.OnDeactivate(e);
            this.DragDropFlag = false;
        }

        private void btGetClipboard_Click(object sender, EventArgs e)
        {
            DisplayDataObject("", Clipboard.GetDataObject());
        }
    }
}