using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using DragDrop.Model.Notes;
using Forms = System.Windows.Forms;

namespace DragDrop
{
    public class DragDropHelper : INotifyPropertyChanged
    {
        public DragDropHelper()
        {
            DragDropFlag = false;
        }

        private static bool DragDropFlag;

        //DragEventArgs dea;
        protected static List<string> _denyList = new List<string>()
        {
            "EnhancedMetafile",
            "Notes Private Data",
            "Link Source",
        };

        public List<string> DenyList
        {
            get { return _denyList; }
        }

        public void DragDrop(object sender, DragEventArgs e)
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
                OnPropertyChanged(XmlTextPropertyName);
                Clipboard.SetText(XmlText);
            }
        }

        public void DragEnter(object sender, DragEventArgs e)
        {
            //this.Activate();
            //this.textBox1.Focus();
            DragDropFlag = true;
            //dea = e;
            if ((e.AllowedEffects & DragDropEffects.Copy) != DragDropEffects.None)
                e.Effects = DragDropEffects.Copy;
        }

        //public void DragLeaveWinform(object sender, EventArgs e)
        //{
        //    //this.OnDeactivate(e);
        //    DragDropFlag = false;
        //}

        public void DragLeave(object sender, DragEventArgs e)
        {
            //this.OnDeactivate(e);
            DragDropFlag = false;
        }

        //private void btGetClipboard_Click(object sender, EventArgs e)
        //{
        //    DisplayDataObject("", Clipboard.GetDataObject());
        //}

        StringBuilder normalText = new StringBuilder();
        StringBuilder richText = new StringBuilder();
        StringBuilder xmlText = new StringBuilder();

        public const string XmlTextPropertyName = "XmlText";

        public String XmlText
        {
            get { return xmlText.ToString(); }
        }

        private void DisplayDataObject(String textHeader, IDataObject dataObject)
        {
            string[] formats = dataObject.GetFormats();
            normalText.Remove(0, normalText.Length);
            richText.Remove(0, richText.Length);
            xmlText.Remove(0, xmlText.Length);
            normalText.Append(textHeader);
            bool isDataPresent;
            object data;
            string dataString;

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
                    Forms.MessageBox.Show(String.Format("Cannot get format:{0}", format));
                    normalText.AppendFormat("Cannot get format:{0}", format);
                }
                catch (ExternalException ex)
                {
                    Forms.MessageBox.Show(String.Format("Cannot get format:{0}", format));
                    normalText.AppendFormat("Cannot get format:{0}", format);
                }
                catch (Exception ex)
                {
                    Forms.MessageBox.Show(String.Format("Cannot get format:{0}", format));
                    normalText.AppendFormat("Cannot get format:{0}", format);
                }
            }
            //this.textBox1.Text = "Hello World";
            //textBox1.Text = normalText.ToString();
            //richTextBox1.Rtf = richText.ToString();
            //richTextBox2.Text = xmlText.ToString();
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

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members
    }
}