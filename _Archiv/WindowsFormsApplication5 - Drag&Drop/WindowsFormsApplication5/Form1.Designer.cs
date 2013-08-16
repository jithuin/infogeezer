namespace WindowsFormsApplication5
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.cbEnableDrop = new System.Windows.Forms.CheckBox();
            this.btGetClipboard = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.AllowDrop = true;
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(635, 318);
            this.textBox1.TabIndex = 0;
            this.textBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox1_DragDrop);
            this.textBox1.DragLeave += new System.EventHandler(this.textBox1_DragLeave);
            this.textBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.textBox1_MouseUp);
            this.textBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.textBox1_DragEnter);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBox1.Location = new System.Drawing.Point(0, 321);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(635, 127);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // cbEnableDrop
            // 
            this.cbEnableDrop.AutoSize = true;
            this.cbEnableDrop.Checked = true;
            this.cbEnableDrop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEnableDrop.Location = new System.Drawing.Point(642, 13);
            this.cbEnableDrop.Name = "cbEnableDrop";
            this.cbEnableDrop.Size = new System.Drawing.Size(83, 17);
            this.cbEnableDrop.TabIndex = 2;
            this.cbEnableDrop.Text = "Enable drop";
            this.cbEnableDrop.UseVisualStyleBackColor = true;
            // 
            // btGetClipboard
            // 
            this.btGetClipboard.Location = new System.Drawing.Point(642, 37);
            this.btGetClipboard.Name = "btGetClipboard";
            this.btGetClipboard.Size = new System.Drawing.Size(95, 23);
            this.btGetClipboard.TabIndex = 3;
            this.btGetClipboard.Text = "Get Clipboard";
            this.btGetClipboard.UseVisualStyleBackColor = true;
            this.btGetClipboard.Click += new System.EventHandler(this.btGetClipboard_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBox2.Location = new System.Drawing.Point(0, 454);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(635, 127);
            this.richTextBox2.TabIndex = 4;
            this.richTextBox2.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 582);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.btGetClipboard);
            this.Controls.Add(this.cbEnableDrop);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox cbEnableDrop;
        private System.Windows.Forms.Button btGetClipboard;
        private System.Windows.Forms.RichTextBox richTextBox2;
    }
}

