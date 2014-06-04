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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using GalaSoft.MvvmLight.Command;

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

        public RelayCommand ExitCommand
        {
            get
            {
                return _exitCommand;
            }
            set
            {
                _exitCommand = value;
            }
        }

        RelayCommand _exitCommand = new RelayCommand(new Action(() => Application.Current.Shutdown()));

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public FlowDocument RichText
        {
            get { return (FlowDocument)GetValue(RichTextProperty); }
            set { SetValue(RichTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RichText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RichTextProperty =
            DependencyProperty.Register("RichText", typeof(FlowDocument), typeof(Window1), new UIPropertyMetadata(null));

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
            FileInfo fi = new FileInfo("proba.xaml");

            if (tr.CanLoad(DataFormats.Xaml) && fi.Exists)
            {
                using (Stream s = fi.Open(FileMode.Open))
                {
                    SetDefaultColorOfTextInXmlStream(s);

                    tr.Load(s, DataFormats.Xaml);
                }
            }
        }

        private void SetDefaultColorOfTextInXmlStream(Stream s)
        {
            /*
            XmlDocument xml = new XmlDocument();
            long beginLength = s.Length;
            xml.Load(s);
            s.Seek(0, SeekOrigin.Begin); // Because Load moved the caret to the end of the stream, and later we need it.
            XmlNode node = xml.FirstChild;
            while (node.Name != "Section" || node.NextSibling != null)
                node = node.NextSibling;

            node.Attributes["Foreground"].Value = this._rtb.Foreground.ToString(); // set the xml default color to the foreground color of the RichTextBox

            XmlTextWriter xmlWriter = new XmlTextWriter(s, Encoding.Default);
            xmlWriter.Formatting = Formatting.None;
            if (beginLength > s.Length)
                throw new InvalidOperationException();
            xml.Save(xmlWriter);
            long endPosition = xmlWriter.BaseStream.Position;
            if (s.Length > endPosition)
                s.SetLength(endPosition); // prevent garbage text on the end of the file to affect the xml reading methods

            s.Seek(0, SeekOrigin.Begin);
            String str = new StreamReader(s).ReadToEnd();
            s.Seek(0, SeekOrigin.Begin);
            /*/
            string foregroundAttributeString = "Foreground=\"";
            String str = new StreamReader(s).ReadToEnd();
            int startIndex = str.IndexOf("Section", 0);
            int endIndex = str.IndexOf('>', startIndex);
            int modStartIndex = str.IndexOf(foregroundAttributeString, startIndex, endIndex - startIndex + 1) + foregroundAttributeString.Length;
            int modEndIndex = str.IndexOf("\"", modStartIndex);
            StringBuilder sb = new StringBuilder();
            sb.Append(str.Substring(0, modStartIndex));
            sb.Append(this._rtb.Foreground.ToString());
            sb.Append(str.Substring(modEndIndex));
            s.Seek(0, SeekOrigin.Begin);
            new StreamWriter(s).Write(sb.ToString());

            //*/
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