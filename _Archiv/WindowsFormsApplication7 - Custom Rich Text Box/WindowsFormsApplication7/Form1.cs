using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication7
{
    public partial class Form1 : Form
    {
        //Constructor
        public Form1()
        {
            InitializeComponent();
        }

        //Member functions
        String LoadText()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(ofd.OpenFile());
                return sr.ReadToEnd();
            }
            return "";
        }

        //Event handlers
        private void customControl11_Resize(object sender, EventArgs e)
        {
            this.customControl11.Invalidate();
            this.Update();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.customControl11.Text = LoadText();
            this.customControl11.Invalidate();
            this.customControl11.Update();
        }

        private void toolStripComboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.toolStripComboBoxSize.SelectedItem!=null)
            {
                int size=int.Parse(this.toolStripComboBoxSize.SelectedItem.ToString());
                Fonts.Change(size);
                this.ToolStripMenuItemfont.HideDropDown();
                //this.ToolStripMenuItemfont.
                this.customControl11.Invalidate();
                this.Update();
            }
        }

        private void toolStripComboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.toolStripComboBoxType.SelectedItem != null)
            {
                string type = this.toolStripComboBoxType.SelectedItem.ToString();
                Fonts.Change(type);
                this.ToolStripMenuItemfont.HideDropDown();
                //this.ToolStripMenuItemfont.
                this.customControl11.Invalidate();
                this.Update();
            }
        }

        
    }
}
