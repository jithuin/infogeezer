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
    public partial class FormAskPortName : Form
    {
        public FormAskPortName()
        {
            InitializeComponent();
        }
        List<String> Ports;
        public static String Port;
        private void Form2_Load(object sender, EventArgs e)
        {
            Ports = new List<string>();
            SearchForPorts(Ports);
            PopulateCombobox(Ports);
        }

        private void PopulateCombobox(List<string> Ports)
        {
            if (cbPortNames.Items.Count != 0) cbPortNames.Items.Clear();
            foreach (String s in Ports)
                cbPortNames.Items.Add(s);
        }

        private void SearchForPorts(List<string> Ports)
        {
            if (Ports.Count != 0) Ports.Clear();
            SerialPort sp = new SerialPort();
            for (int i = 1; i < 10; i++)
            {
                try
                {
                    sp.PortName = "COM" + i.ToString();
                    sp.Open();
                    sp.Close();
                    Ports.Add(sp.PortName);
                }
                catch
                {

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (cbPortNames.SelectedItem != null)
                FormAskPortName.Port = cbPortNames.SelectedItem.ToString();
            this.Close();
        }
    }
}
