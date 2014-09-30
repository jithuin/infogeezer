using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace DragDrop.Model
{
    public class DefaultDataObject : DataObjectBase
    {
        private List<String> Formats = new List<string>();
        private StringBuilder normalText = new StringBuilder();
        private StringBuilder richText = new StringBuilder();

        protected static List<string> _denyList = new List<string>()
        {
            "EnhancedMetafile",
            "Notes Private Data",
            "Link Source",
            //"DragContext",
        };

        public List<string> DenyList
        {
            get { return _denyList; }
        }

        public DefaultDataObject(IDataObject object_in)
            : base(object_in)
        {
        }

        public override bool IsDataObjectCompatible(System.Windows.IDataObject object_in)
        {
            return true;
        }

        public override string DataString
        {
            get
            {
                if (normalText.ToString() == "")
                    DisplayDataObject("", _dataObject);
                return normalText.ToString();
            }
        }

        private void DisplayDataObject(String textHeader, IDataObject dataObject)
        {
            string[] formats = dataObject.GetFormats();
            normalText.Remove(0, normalText.Length);
            richText.Remove(0, richText.Length);

            normalText.Append(textHeader);
            bool isDataPresent;
            object data;
            string dataString;

            normalText.AppendLine("Formats: ");

            foreach (string format in formats)
                normalText.AppendFormat(" {0}\r\n", format);

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
                                    DisplayMemory(normalText, ms);
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
                                            //normalText.Append(format + ": " + dataString + "\r\n");
                                            richText.Append(dataString);
                                            break;

                                        case "Rich Text Format Without Objects":
                                        case "RTF As Text":
                                        case "System.String":
                                        case "UnicodeText":
                                        case "Text":
                                        default:
                                            break;
                                    }
                                    break;
                                }
                        }
                        normalText.Append("\r\n");
                    }
                    else
                    {
                        normalText.AppendFormat("Cannot get format:{0} (IsDataPresent:{1}\r\n)", format, isDataPresent);
                    }
                }
                //catch (ExecutionEngineException ex)
                //{
                //    Forms.MessageBox.Show(String.Format("Cannot get format:{0}", format));
                //    normalText.AppendFormat("Cannot get format:{0}", format);
                //}
                catch (ExternalException ex)
                {
                    //Forms.MessageBox.Show(String.Format("Cannot get format:{0}", format));
                    normalText.AppendFormat("Cannot get format:{0}\r\n", format);
                }
                catch (Exception ex)
                {
                    //Forms.MessageBox.Show(String.Format("Cannot get format:{0}", format));
                    normalText.AppendFormat("Cannot get format:{0}\r\n", format);
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
    }
}