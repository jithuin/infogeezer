using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace WindowsFormsApplication6
{
    public partial class FormMain : Form
    {
        #region Constructors and Members
        public FormMain()
        {
            InitializeComponent();
        }
        SerialPort sp;
        delegate void DataWriteDelegate();
        DataWriteDelegate dwd;
        Shape shape;
        #endregion

        #region Init Functions
        private void Form1_Load(object sender, EventArgs e)
        {
            MustChooseSerialPort();
            shape = new Shape();
        }
        void MustChooseSerialPort()
        {
            FormAskPortName f = new FormAskPortName();
            DialogResult dr;
            do
            {
                dr = f.ShowDialog();
            } while ((dr != DialogResult.OK || FormAskPortName.Port == null || FormAskPortName.Port == "") && dr != DialogResult.Cancel);
            if (dr == DialogResult.OK)
                InitSerialPort();
            else
                Application.Exit();
        }
        #endregion

        #region Serial Port Functions
        void ChooseSerialPort()
        {
            FormAskPortName f = new FormAskPortName();
            DialogResult dr;
            dr = f.ShowDialog();
            if (dr == DialogResult.OK || FormAskPortName.Port == null || FormAskPortName.Port == "")
                InitSerialPort();
            else
                MessageBox.Show("Nem sikerült a portváltás!");
        }
        void InitSerialPort()
        {
            if (sp != null && sp.IsOpen) sp.Close();
            sp = new SerialPort(FormAskPortName.Port);
            sp.Open();
            sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            sp.DtrEnable = true;
            dwd = new DataWriteDelegate(dw);
            this.toolStripStatusLabel1.Text = "Connected to " + sp.PortName;
        }
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Invoke(dwd);
        }
        Shape.Directions TranslateSerialMessage(String message)
        {
            switch (message)
            {
                case "L": return Shape.Directions.Left;
                case "M": return Shape.Directions.Down;
                case "R": return Shape.Directions.Right;
                case "S": return Shape.Directions.Turn;
                default: return Shape.Directions.Else;
            }
        }
        void dw()
        {
            String s = sp.ReadExisting();
            this.label1.Text += s;
            this.shape.TransformShape(TranslateSerialMessage(s));
            this.Invalidate();
        }
        #endregion

        #region Menu Button Click Functions
        private void changeSerialPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChooseSerialPort();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout f = new FormAbout();
            f.ShowDialog();
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //this.square.DrawMe(e.Graphics);
            this.shape.DrawMe(e.Graphics);
        }
    }
}
