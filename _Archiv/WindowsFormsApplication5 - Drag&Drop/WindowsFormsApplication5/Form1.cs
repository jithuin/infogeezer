using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        DragEventArgs dea;
        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            String text =
                "e.AllowedEffect: " + e.AllowedEffect.ToString() + "\r\n" +
                "e.Data: " + e.Data.GetData(e.Data.GetFormats()[0]).ToString() + "\r\n" +
                "e.Effect: " + e.Effect.ToString() + "\r\n" +
                "e.KeyState: " + e.KeyState.ToString() + "\r\n" +
                "e.X: " + e.X.ToString() + "\r\n" +
                "e.Y: " + e.Y.ToString() + "\r\n";
            IDataObject dataObject = e.Data;
            string[] formats = dataObject.GetFormats();
            bool isDataPresent;
            object data;
            string dataString;
            foreach (string s in formats)
            {
                isDataPresent = dataObject.GetDataPresent(s);
                // ???mért nem tudja konvertálni az enhanced metafile-t????
                if (isDataPresent && s!="EnhancedMetafile" && (data = dataObject.GetData(s))!=null)
                {
                    dataString = data.ToString();
                    text += s + ": " + dataString + "\r\n";
                    switch (dataString)
                    {
                        case "System.IO.MemoryStream":
                            {
                                System.IO.MemoryStream ms = (System.IO.MemoryStream)data;
                                int b;
                                //textBox1.Text += "Pos: " + ms.Position + " Len: " + ms.Length + "\r\n";
                                if (ms.Length < 1000)
                                    for (; ms.Position != ms.Length; )
                                    {
                                        b = ms.ReadByte();
                                        if (b == 0)
                                            text += "\x1";
                                        else
                                            text += ((char)b).ToString();
                                    }
                                else
                                    for (; ms.Position != 1000; )
                                    {
                                        b = ms.ReadByte();
                                        if (b == 0)
                                            text += "\x1";
                                        else
                                            text += ((char)b).ToString();
                                    }
                                break;
                            }
                        case "System.String[]":
                            {
                                foreach (string s2 in (System.String[])data)
                                {
                                    text += s2 + "\r\n";
                                }
                                break;
                            }
                        default:
                            {
                                if (s == "Rich Text Format")
                                {
                                    text += s + ": " + dataString + "\r\n";
                                    richTextBox1.Rtf = dataString;
                                }
                                if (s == "Rich Text Format Without Objects") text += s + ": " + dataString + "\r\n";
                                if (s == "RTF As Text") text += s + ": " + dataString + "\r\n";
                                if (s == "System.String") text += s + ": " + dataString + "\r\n";
                                if (s == "UnicodeText") text += s + ": " + dataString + "\r\n";
                                if (s == "Text") text += s + ": " + dataString + "\r\n";

                                //textBox1.Text += e.Data.GetData(s).ToString() + "\r\n";
                                break;
                            }
                    }
                    text += "\r\n";
                }
            }
            this.textBox1.Text = "Hello World";
            textBox1.Text = text;
        }

        //private void textBox1_DragOver(object sender, DragEventArgs e)
        //{
        //    this.Activate();
        //    this.textBox1.Focus();
        //    this.DragDropFlag = true;
        //}

        private void Form1_Resize(object sender, EventArgs e)
        {
            textBox1.Height = (int)(this.Height * 0.7);
            richTextBox1.Height = (int)(this.Height * 0.2);
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
            dea = e;
            if ((e.AllowedEffect & DragDropEffects.Copy)!=DragDropEffects.None)
                e.Effect = DragDropEffects.Copy;
        }

        private void textBox1_DragLeave(object sender, EventArgs e)
        {
            this.OnDeactivate(e);
            this.DragDropFlag = false;
        }

    }
}
