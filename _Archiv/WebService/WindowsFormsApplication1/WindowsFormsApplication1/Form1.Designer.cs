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
					this.btGo = new System.Windows.Forms.Button();
					this.tbUrl = new System.Windows.Forms.TextBox();
					this.tbBody = new System.Windows.Forms.TextBox();
					this.wbBody = new System.Windows.Forms.WebBrowser();
					this.SuspendLayout();
					// 
					// btGo
					// 
					this.btGo.Location = new System.Drawing.Point(564, 11);
					this.btGo.Name = "btGo";
					this.btGo.Size = new System.Drawing.Size(75, 23);
					this.btGo.TabIndex = 0;
					this.btGo.Text = "GO";
					this.btGo.UseVisualStyleBackColor = true;
					this.btGo.Click += new System.EventHandler(this.btGo_Click);
					// 
					// tbUrl
					// 
					this.tbUrl.Location = new System.Drawing.Point(12, 13);
					this.tbUrl.Name = "tbUrl";
					this.tbUrl.Size = new System.Drawing.Size(546, 20);
					this.tbUrl.TabIndex = 1;
					// 
					// tbBody
					// 
					this.tbBody.Location = new System.Drawing.Point(1, 40);
					this.tbBody.Multiline = true;
					this.tbBody.Name = "tbBody";
					this.tbBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
					this.tbBody.Size = new System.Drawing.Size(651, 231);
					this.tbBody.TabIndex = 2;
					// 
					// wbBody
					// 
					this.wbBody.Location = new System.Drawing.Point(1, 277);
					this.wbBody.MinimumSize = new System.Drawing.Size(20, 20);
					this.wbBody.Name = "wbBody";
					this.wbBody.Size = new System.Drawing.Size(651, 250);
					this.wbBody.TabIndex = 3;
					// 
					// Form1
					// 
					this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
					this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
					this.ClientSize = new System.Drawing.Size(651, 531);
					this.Controls.Add(this.wbBody);
					this.Controls.Add(this.tbBody);
					this.Controls.Add(this.tbUrl);
					this.Controls.Add(this.btGo);
					this.Name = "Form1";
					this.Text = "Form1";
					this.ResumeLayout(false);
					this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btGo;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.TextBox tbBody;
				private System.Windows.Forms.WebBrowser wbBody;
    }
}

