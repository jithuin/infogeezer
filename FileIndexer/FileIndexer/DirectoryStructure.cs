using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;

namespace FileIndexer
{
    public class DirectoryStructure
    {
        public DirectoryStructure()
        {

        }

        public void ScanDirectoryStructure(BackgroundWorker bw, DoWorkEventArgs e)
        {
            DirectoryInfo di = e.Argument as DirectoryInfo;
            if (di != null)
                _listOfDirectories.Add(new ScannableDirectoryInfo(di));
            for (int i = 0; i < _listOfDirectories.Count && bw.CancellationPending != true; i++)
            {
                if (ScanThisDirectory(_listOfDirectories[i]))
                {
                    bw.ReportProgress(0, new ScanState(_listOfDirectories[i].DirectoryInfo.FullName, _listOfDirectories.Count + NumberOfFiles));
                }

            }
            if (bw.CancellationPending)
                e.Cancel = true;

        }
        List<ScannableDirectoryInfo> _listOfDirectories = new List<ScannableDirectoryInfo>();
        long NumberOfFiles = 0;
        public bool ScanThisDirectory(ScannableDirectoryInfo sdi)
        {
            bool _errorFlag = false;
            if (sdi.Scanned == true)
                return false;
            try
            {
                _listOfDirectories.AddRange(sdi.DirectoryInfo
                .GetDirectories()
                .Select<DirectoryInfo, ScannableDirectoryInfo>(new Func<DirectoryInfo, ScannableDirectoryInfo>(
                    (di) =>
                    {
                        return new ScannableDirectoryInfo(di);
                    }
                    )));
                NumberOfFiles += sdi.DirectoryInfo.GetFiles().Count();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.Message);
                _errorFlag = true;
            }

            return !_errorFlag;
        }
    }

    #region Scan State
    public interface IScanState
    {
        long NumberOfScannedNodes { get; }
        String CurrentNode { get; }
    }

    public class ScanState : IScanState
    {
        public ScanState(String currentNode_in, long numberOfScannedNodes_in)
        {
            this.CurrentNode = currentNode_in;
            this.NumberOfScannedNodes = numberOfScannedNodes_in;
        }

        #region IScanState Members

        public long NumberOfScannedNodes
        {
            get;
            protected set;
        }

        public string CurrentNode
        {
            get;
            protected set;
        }

        #endregion
    }
    #endregion

    #region Scannable Directory Info
    public interface IScannable
    {
        bool Scanned { get; set; }
    }

    public class ScannableDirectoryInfo : IScannable
    {
        public ScannableDirectoryInfo()
        {

        }
        public ScannableDirectoryInfo(DirectoryInfo di_in)
        {
            DirectoryInfo = di_in;
        }

        public DirectoryInfo DirectoryInfo { get; set; }

        #region IScannable Members

        public bool Scanned
        {
            get;
            set;
        }

        #endregion
    }
    #endregion

}
