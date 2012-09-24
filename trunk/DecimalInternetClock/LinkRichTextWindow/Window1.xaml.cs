using System;
using System.Collections.Generic;
using System.IO;
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

namespace LinkRichTextWindow
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            //_rtb_Load.Document.AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(Hyperlink_RequestNavigate));
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            global::System.Windows.Forms.MessageBox.Show(e.Uri.AbsoluteUri);
        }

        List<String> _listOfFormats = new List<string>()
            {
                DataFormats.Html,
                DataFormats.OemText,
                DataFormats.Rtf,
                DataFormats.Serializable,
                DataFormats.StringFormat,
                DataFormats.Text,
                DataFormats.Xaml,
                DataFormats.UnicodeText,
            };

        private void Paste_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            TextPointer start = this._rtb.Document.ContentStart,
                        end = this._rtb.Document.ContentEnd;
            TextRange tr = new TextRange(start, end);
            tr.Select(start, end);
            MemoryStream ms;
            StringBuilder sb = new StringBuilder();
            foreach (String dataFormat in _listOfFormats)
            {
                if (tr.CanSave(dataFormat))
                {
                    ms = new MemoryStream();
                    tr.Save(ms, dataFormat);
                    ms.Seek(0, SeekOrigin.Begin);
                    sb.AppendLine(dataFormat);
                    foreach (char c in ms.ToArray().Select<byte, char>((b) => (char)b))
                    {
                        sb.Append(c);
                    }
                    sb.AppendLine();
                }
                //_tb.Text = sb.ToString();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextPointer start = this._rtb.Document.ContentStart,
                        end = this._rtb.Document.ContentEnd;
            TextRange tr = new TextRange(start, end);
            if (tr.CanLoad(DataFormats.Xaml))
            {
                FileInfo fi = new FileInfo("proba.xaml");
                if (fi.Exists)
                {
                    Stream s = fi.Open(FileMode.Open);
                    tr.Load(s, DataFormats.Xaml);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextPointer start = this._rtb.Document.ContentStart,
                        end = this._rtb.Document.ContentEnd;
            TextRange tr = new TextRange(start, end);
            tr.Select(start, end);
            if (tr.CanSave(DataFormats.Xaml))
            {
                FileInfo fi = new FileInfo("proba.xaml");
                if (fi.Exists)
                    fi.Delete();
                FileStream fs = fi.Create();
                tr.Save(fs, DataFormats.Xaml);
                fs.Flush();
                fs.Close();
            }
        }

        private void _rtb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //_rtb.Document.PageWidth = _rtb.ActualWidth - 25; // -scrollbar.Width
            _rtb.Document.Blocks.Max(new Func<Block, decimal>((block) =>
            {
                return block is Table ? 10 : 0;
            }));
        }
    }
}