namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grbBaudRate = new System.Windows.Forms.GroupBox();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.grbDataBits = new System.Windows.Forms.GroupBox();
            this.radioButton8 = new System.Windows.Forms.RadioButton();
            this.radioButton7 = new System.Windows.Forms.RadioButton();
            this.grbParity = new System.Windows.Forms.GroupBox();
            this.radioButton11 = new System.Windows.Forms.RadioButton();
            this.radioButton10 = new System.Windows.Forms.RadioButton();
            this.radioButton9 = new System.Windows.Forms.RadioButton();
            this.grbStopBits = new System.Windows.Forms.GroupBox();
            this.radioButton13 = new System.Windows.Forms.RadioButton();
            this.radioButton12 = new System.Windows.Forms.RadioButton();
            this.cbComPortName = new System.Windows.Forms.ComboBox();
            this.btConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.grbBaudRate.SuspendLayout();
            this.grbDataBits.SuspendLayout();
            this.grbParity.SuspendLayout();
            this.grbStopBits.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbBaudRate
            // 
            this.grbBaudRate.Controls.Add(this.radioButton6);
            this.grbBaudRate.Controls.Add(this.radioButton5);
            this.grbBaudRate.Controls.Add(this.radioButton4);
            this.grbBaudRate.Controls.Add(this.radioButton3);
            this.grbBaudRate.Controls.Add(this.radioButton2);
            this.grbBaudRate.Controls.Add(this.radioButton1);
            this.grbBaudRate.Location = new System.Drawing.Point(140, 12);
            this.grbBaudRate.Name = "grbBaudRate";
            this.grbBaudRate.Size = new System.Drawing.Size(124, 91);
            this.grbBaudRate.TabIndex = 0;
            this.grbBaudRate.TabStop = false;
            this.grbBaudRate.Text = "Baud rate";
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Location = new System.Drawing.Point(62, 65);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(55, 17);
            this.radioButton6.TabIndex = 5;
            this.radioButton6.TabStop = true;
            this.radioButton6.Text = "38400";
            this.radioButton6.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Location = new System.Drawing.Point(62, 42);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(55, 17);
            this.radioButton5.TabIndex = 4;
            this.radioButton5.TabStop = true;
            this.radioButton5.Text = "19200";
            this.radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Checked = true;
            this.radioButton4.Location = new System.Drawing.Point(62, 19);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(52, 17);
            this.radioButton4.TabIndex = 3;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = " 9600";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(7, 65);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(49, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "4800";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(7, 42);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(49, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "2400";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(7, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(49, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "1200";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // grbDataBits
            // 
            this.grbDataBits.Controls.Add(this.radioButton8);
            this.grbDataBits.Controls.Add(this.radioButton7);
            this.grbDataBits.Location = new System.Drawing.Point(270, 13);
            this.grbDataBits.Name = "grbDataBits";
            this.grbDataBits.Size = new System.Drawing.Size(64, 90);
            this.grbDataBits.TabIndex = 1;
            this.grbDataBits.TabStop = false;
            this.grbDataBits.Text = "Data bits";
            // 
            // radioButton8
            // 
            this.radioButton8.AutoSize = true;
            this.radioButton8.Checked = true;
            this.radioButton8.Location = new System.Drawing.Point(7, 41);
            this.radioButton8.Name = "radioButton8";
            this.radioButton8.Size = new System.Drawing.Size(31, 17);
            this.radioButton8.TabIndex = 1;
            this.radioButton8.TabStop = true;
            this.radioButton8.Text = "8";
            this.radioButton8.UseVisualStyleBackColor = true;
            // 
            // radioButton7
            // 
            this.radioButton7.AutoSize = true;
            this.radioButton7.Location = new System.Drawing.Point(7, 19);
            this.radioButton7.Name = "radioButton7";
            this.radioButton7.Size = new System.Drawing.Size(31, 17);
            this.radioButton7.TabIndex = 0;
            this.radioButton7.TabStop = true;
            this.radioButton7.Text = "7";
            this.radioButton7.UseVisualStyleBackColor = true;
            // 
            // grbParity
            // 
            this.grbParity.Controls.Add(this.radioButton11);
            this.grbParity.Controls.Add(this.radioButton10);
            this.grbParity.Controls.Add(this.radioButton9);
            this.grbParity.Location = new System.Drawing.Point(340, 13);
            this.grbParity.Name = "grbParity";
            this.grbParity.Size = new System.Drawing.Size(59, 90);
            this.grbParity.TabIndex = 2;
            this.grbParity.TabStop = false;
            this.grbParity.Text = "Parity";
            // 
            // radioButton11
            // 
            this.radioButton11.AutoSize = true;
            this.radioButton11.Location = new System.Drawing.Point(7, 64);
            this.radioButton11.Name = "radioButton11";
            this.radioButton11.Size = new System.Drawing.Size(50, 17);
            this.radioButton11.TabIndex = 2;
            this.radioButton11.TabStop = true;
            this.radioButton11.Text = "Even";
            this.radioButton11.UseVisualStyleBackColor = true;
            // 
            // radioButton10
            // 
            this.radioButton10.AutoSize = true;
            this.radioButton10.Location = new System.Drawing.Point(7, 41);
            this.radioButton10.Name = "radioButton10";
            this.radioButton10.Size = new System.Drawing.Size(45, 17);
            this.radioButton10.TabIndex = 1;
            this.radioButton10.TabStop = true;
            this.radioButton10.Text = "Odd";
            this.radioButton10.UseVisualStyleBackColor = true;
            // 
            // radioButton9
            // 
            this.radioButton9.AutoSize = true;
            this.radioButton9.Checked = true;
            this.radioButton9.Location = new System.Drawing.Point(7, 18);
            this.radioButton9.Name = "radioButton9";
            this.radioButton9.Size = new System.Drawing.Size(51, 17);
            this.radioButton9.TabIndex = 0;
            this.radioButton9.TabStop = true;
            this.radioButton9.Text = "None";
            this.radioButton9.UseVisualStyleBackColor = true;
            // 
            // grbStopBits
            // 
            this.grbStopBits.Controls.Add(this.radioButton13);
            this.grbStopBits.Controls.Add(this.radioButton12);
            this.grbStopBits.Location = new System.Drawing.Point(405, 13);
            this.grbStopBits.Name = "grbStopBits";
            this.grbStopBits.Size = new System.Drawing.Size(62, 90);
            this.grbStopBits.TabIndex = 3;
            this.grbStopBits.TabStop = false;
            this.grbStopBits.Text = "Stop bits";
            // 
            // radioButton13
            // 
            this.radioButton13.AutoSize = true;
            this.radioButton13.Location = new System.Drawing.Point(7, 41);
            this.radioButton13.Name = "radioButton13";
            this.radioButton13.Size = new System.Drawing.Size(31, 17);
            this.radioButton13.TabIndex = 1;
            this.radioButton13.Text = "2";
            this.radioButton13.UseVisualStyleBackColor = true;
            // 
            // radioButton12
            // 
            this.radioButton12.AutoSize = true;
            this.radioButton12.Checked = true;
            this.radioButton12.Location = new System.Drawing.Point(7, 19);
            this.radioButton12.Name = "radioButton12";
            this.radioButton12.Size = new System.Drawing.Size(31, 17);
            this.radioButton12.TabIndex = 0;
            this.radioButton12.TabStop = true;
            this.radioButton12.Text = "1";
            this.radioButton12.UseVisualStyleBackColor = true;
            // 
            // cbComPortName
            // 
            this.cbComPortName.FormattingEnabled = true;
            this.cbComPortName.Location = new System.Drawing.Point(12, 31);
            this.cbComPortName.Name = "cbComPortName";
            this.cbComPortName.Size = new System.Drawing.Size(121, 21);
            this.cbComPortName.TabIndex = 4;
            // 
            // btConnect
            // 
            this.btConnect.Location = new System.Drawing.Point(474, 10);
            this.btConnect.Name = "btConnect";
            this.btConnect.Size = new System.Drawing.Size(75, 23);
            this.btConnect.TabIndex = 5;
            this.btConnect.Text = "Connect";
            this.btConnect.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "COM port name:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 422);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btConnect);
            this.Controls.Add(this.cbComPortName);
            this.Controls.Add(this.grbStopBits);
            this.Controls.Add(this.grbParity);
            this.Controls.Add(this.grbDataBits);
            this.Controls.Add(this.grbBaudRate);
            this.Name = "Form1";
            this.Text = "Form1";
            this.grbBaudRate.ResumeLayout(false);
            this.grbBaudRate.PerformLayout();
            this.grbDataBits.ResumeLayout(false);
            this.grbDataBits.PerformLayout();
            this.grbParity.ResumeLayout(false);
            this.grbParity.PerformLayout();
            this.grbStopBits.ResumeLayout(false);
            this.grbStopBits.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grbBaudRate;
        private System.Windows.Forms.GroupBox grbDataBits;
        private System.Windows.Forms.GroupBox grbParity;
        private System.Windows.Forms.GroupBox grbStopBits;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton8;
        private System.Windows.Forms.RadioButton radioButton7;
        private System.Windows.Forms.ComboBox cbComPortName;
        private System.Windows.Forms.RadioButton radioButton11;
        private System.Windows.Forms.RadioButton radioButton10;
        private System.Windows.Forms.RadioButton radioButton9;
        private System.Windows.Forms.RadioButton radioButton13;
        private System.Windows.Forms.RadioButton radioButton12;
        private System.Windows.Forms.Button btConnect;
        private System.Windows.Forms.Label label1;
    }
}

