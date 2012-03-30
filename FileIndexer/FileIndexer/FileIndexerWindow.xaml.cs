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
using System.ComponentModel;
using System.IO;

namespace FileIndexer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FileIndexerWindow : Window
    {
        public FileIndexerWindow()
        {
            InitializeComponent();
            InitBackroundWorker();
        }

        private void InitBackroundWorker()
        {
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
        }
        DirectoryStructure _dirStruct = new DirectoryStructure();

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                _tbCurrentDirectory.Text += "\r\n Aborted";
            }
            else
            {
                _tbCurrentDirectory.Text += "\r\n Ready";
                _isButton = false;
                _btStartText.Text = "Scan";
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            IScanState state = e.UserState as IScanState;
            if (state != null)
            {
                Dispatcher.BeginInvoke(new Action<IScanState>((st) => 
                { 
                    _tbCurrentDirectory.Text = String.Format("Current Node: {0}", st.CurrentNode );
                    _tbNumberOfScannedNodes.Text = String.Format("\r\nScanned Nodes: {0:D10}", st.NumberOfScannedNodes);
                }), state);
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            _dirStruct.ScanDirectoryStructure(bw, e);
        }

        BackgroundWorker bw = new BackgroundWorker();

        bool _isButton = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_isButton)
            {
                if (_tbRootPath.Text != "")
                {
                    DirectoryInfo di = new DirectoryInfo(_tbRootPath.Text);
                    if (di.Exists)
                    {
                        bw.RunWorkerAsync(di);
                        _isButton = true;
                        _btStartText.Text = "Cancel";
                    }
                }
            }
            else
            {
                bw.CancelAsync();

                _isButton = false;
                _btStartText.Text = "Scan";
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        
    }
}
