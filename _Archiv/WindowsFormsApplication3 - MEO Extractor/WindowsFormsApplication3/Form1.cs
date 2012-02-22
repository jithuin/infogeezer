using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            fdOpen.InitialDirectory = "s:\\";
            lvFiles.CheckBoxes = true;
        }

        private String destinationDirectoryName;
        public String DestinationDirectoryName 
        {
            get { return destinationDirectoryName; }
            set 
            {
                FileInfo fi = new FileInfo(value);
                destinationDirectoryName = fi.DirectoryName;
            }
        }

        private const String destFileNameCV = "\\RelativeDeviation.sum";
        private const String destFileNameSD = "\\StandardDeviation.sum";

        #region Main Menu Items

        private void miOpen_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == fdOpen.ShowDialog() && fdOpen.CheckFileExists)
            {
                foreach (String s in fdOpen.FileNames)
                    lvFiles.Items.Add(s);
            }
        }

        private void miNew_Click(object sender, EventArgs e)
        {
            lvFiles.Items.Clear();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void miMakeAll_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Working...";
            DestinationDirectoryName = lvFiles.Items[0].Text;

            FileInfo fi = new FileInfo(DestinationDirectoryName + destFileNameSD);
            if (fi.Exists)
                fi.Delete();
            fi = new FileInfo(DestinationDirectoryName + destFileNameCV);
            if (fi.Exists)
                fi.Delete();

            foreach (ListViewItem lvi in lvFiles.Items)
                Make(lvi);
            lbStatus.Text = "Ready with all files";
        }
        #endregion

        #region Context Menu Items

        private void cmiCheck_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvFiles.SelectedItems)
                lvi.Checked = true;
        }

        private void cmiInvertChecking_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvFiles.Items)
                lvi.Checked = !lvi.Checked;
        }

        private void cmiMakeChecked_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Working...";
            DestinationDirectoryName = lvFiles.CheckedItems[0].Text;
            
            FileInfo fi= new FileInfo(DestinationDirectoryName + destFileNameSD);
            if (fi.Exists) 
                fi.Delete();
            fi = new FileInfo(DestinationDirectoryName + destFileNameCV);
            if (fi.Exists) 
                fi.Delete();

            foreach (ListViewItem lvi in lvFiles.CheckedItems)
                Make(lvi);
            lbStatus.Text = "Ready with checked files";
        }

        private void cmiDeleteSelected_Click(object sender, EventArgs e)
        {
            while (lvFiles.SelectedItems.Count > 0)
                lvFiles.Items.Remove(lvFiles.SelectedItems[0]);
        }

        private void cmiDeleteChecked_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lvFiles.Items.Count; i++)
            {
                if (lvFiles.Items[i].Checked)
                {
                    lvFiles.Items.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion

        private void Make(ListViewItem lvi)
        {
            try
            {
                FileInfo fi = new FileInfo(lvi.Text);
                StreamReader sr;
                if (fi.Exists)
                {
                    sr = new StreamReader(fi.OpenRead(), Encoding.Default);
                    String fileText = sr.ReadToEnd();
                    sr.Close();
                    TextOperationSD(fileText, fi.Name);
                    TextOperationCV(fileText, fi.Name);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error in make function!");
            }
        }

        private void TextOperationSD(String fileText, String serial)
        {
            try
            {
                // prepare the output files
                FileInfo fiDestStDev = new FileInfo(DestinationDirectoryName + destFileNameSD);
                StreamWriter swSD = null;
                if (!fiDestStDev.Exists)
                {
                    swSD = fiDestStDev.CreateText();
                    swSD.Close();
                }
                swSD = new StreamWriter(fiDestStDev.OpenWrite(), Encoding.Default);
                swSD.BaseStream.Seek(0, SeekOrigin.End);

                // string operations
                String[] StringArray = fileText.Split('\x0D', '\x0A');
                long filePosition = swSD.BaseStream.Position;
                int measureCount = 0;
                foreach (String s in StringArray)
                {
                    if (s.StartsWith("Szórás:"))
                    {
                        measureCount++;
                        if (measureCount > 2)
                        {
                            swSD.Flush();
                            swSD.BaseStream.Position = filePosition;
                            measureCount = 0;
                        }
                        swSD.WriteLine(serial + " - " + s);
                    }
                }
                swSD.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error in text operation SD!");
            }
        }

        private void TextOperationCV(String text, String serial)
        {
            try
            {
                // prepare the output files
                FileInfo fiDestRelDev = new FileInfo(DestinationDirectoryName + destFileNameCV);
                StreamWriter swCV = null;
                if (!fiDestRelDev.Exists)
                {
                    swCV = fiDestRelDev.CreateText();
                    swCV.Close();
                }
                swCV = new StreamWriter(fiDestRelDev.OpenWrite(), Encoding.Default);
                swCV.BaseStream.Seek(0, SeekOrigin.End);

                // string operations
                String[] StringArray = text.Split('\x0D', '\x0A');
                long filePosition = swCV.BaseStream.Position;
                int measureCount = 0;
                foreach (String s in StringArray)
                {
                    if (s.StartsWith("CV:"))
                    {
                        measureCount++;
                        if (measureCount > 2)
                        {
                            swCV.Flush();
                            swCV.BaseStream.Seek(filePosition, SeekOrigin.Begin);
                            measureCount = 0;
                        }
                        swCV.WriteLine(serial + " - " + s);
                        
                    }
                }
                swCV.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Error in text operation CV!");
            }
        }
    }
}
