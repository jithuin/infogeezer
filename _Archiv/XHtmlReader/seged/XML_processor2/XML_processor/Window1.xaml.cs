using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Xml;

namespace XML_processor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void btBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "XML files(*.XML;*.HTML;*.XHTML)|*.XML;*.HTML;*.XHTML" +
                             "|All files (*.*)|*.*";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;
            if (myDialog.ShowDialog() == true)
            {
                tbFilePath.Text = myDialog.FileName;
            }
        }

        private void btParse_Click(object sender, RoutedEventArgs e)
        {
            StreamReader sr = null;
            try
            {
                FileInfo fi = new FileInfo(tbFilePath.Text);
                if (fi.Exists)
                {
                    XmlDocument doc = new XmlDocument();
                    sr = fi.OpenText();
                    sr.ReadLine();
                    sr.ReadLine();
                    doc.LoadXml(sr.ReadToEnd());

                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("<xml>");
                    //sb.Append(sr.ReadToEnd());
                    //sb.Append("</xml>");
                    //doc.LoadXml(sb.ToString());

                    //XmlTextReader reader = new XmlTextReader(sr.BaseStream);
                    //do
                    //{
                    //    reader.Read();
                    //} while (reader.NodeType != XmlNodeType.Element);
                    //doc.Load(reader);
                    //doc.Load(sm);

                    Parse_doc(doc, treeView);
                    //tbAttributes.Text = "Kész";
                }
            }
            catch (XmlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }

        private void Parse_doc(XmlDocument doc, TreeView treeView)
        {
            XmlNodeTreeViewItem tvi = new XmlNodeTreeViewItem();
            XmlNode node = doc.FirstChild;
            tvi.Header = node.Name;
            tvi.Node = node;
            Parse_Node(tvi);
            treeView.Items.Add(tvi);
        }

        private void Parse_Node(XmlNodeTreeViewItem UINode_in)
        {
            foreach (XmlNode node in UINode_in.Node.ChildNodes)
            {
                XmlNodeTreeViewItem tvi = new XmlNodeTreeViewItem();
                if (node.Value != null)
                {
                    tvi.Header = node.Value;
                }
                else
                {
                    tvi.Header = node.Name;
                    //if (!node.Name.StartsWith("#"))
                    //    tvi.Name = node.Name;
                    //else
                    //{
                    //    tvi.Name = node.Name.Substring(1);
                    //}
                }
                tvi.Node = node;
                Parse_Node(tvi);
                UINode_in.Items.Add(tvi);
            }
        }

        private string Parse_Attr(XmlNodeTreeViewItem Node_in)
        {
            if (Node_in.Node.Attributes != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (XmlAttribute attr in Node_in.Node.Attributes)
                {
                    sb.Append(attr.Name + "=" + attr.Value + "\r\n");
                }
                return sb.ToString();
            }
            else
            {
                return "";
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            tbAttributes.Text = Parse_Attr((XmlNodeTreeViewItem)e.NewValue);
        }

    }
}
