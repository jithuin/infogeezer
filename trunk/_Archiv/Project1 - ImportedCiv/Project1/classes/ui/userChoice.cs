using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
//using OpenNETCF.Win32;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for userInput1.
	/// </summary>
	public class userChoice : System.Windows.Forms.Form
	{
		public int result;
		ComboBox cb;

		public userChoice( string title, string text, string[] choices, int Default, string ok, string cancel )
		{
			platformSpec.manageWindows.setUserInputSize( this );
			platformSpec.setFloatingWindow.before( this );
		//	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; 
			this.ControlBox = false; 

			int space = 8;
			this.Text = title; 
		//	this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * 3 / 4; 

			Label lbl = new Label();
			lbl.Text = text;
			lbl.Top = space;
			lbl.Left = space;
			lbl.Width = this.Width - 2 * space;

			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap( 1, 1 );
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage( bmp );
			string[] words = text.Split( " ".ToCharArray() );
			int lines = 1, lineWidth = 0;
			for ( int i = 0; i < words.Length; i++ )
			{
				lineWidth += (int)g.MeasureString( words[ i ] + ( i < words.Length - 1? " " : "" ), lbl.Font ).Width;
				if ( lineWidth >= lbl.Width )
				{
					lines++;
					lineWidth = 0;
					i--;
				}
			}

			lbl.Height = lines * (int)g.MeasureString( words[0 ] + " ", lbl.Font ).Height;

			g.MeasureString( text, lbl.Font );

			cb = new ComboBox();

			cb.Left = space;
			cb.Top = lbl.Bottom + space;
			cb.Width = this.ClientSize.Width - 2 * space;
			for ( int i = 0; i < choices.Length; i++ )
				cb.Items.Add( choices[ i ] );

			cb.SelectedIndex = Default;

			Button cmdOk = new Button();
			cmdOk.Width = ( this.ClientSize.Width - 3 * space ) / 2;
			cmdOk.Left = space;
			cmdOk.Top = cb.Bottom + space;
			cmdOk.Text = ok; //language.getAString();//language.order.ok );//ok;
			cmdOk.Click += new EventHandler(cmdOk_Click);
#if !CF
			cmdOk.FlatStyle = FlatStyle.System;
#endif
			
			Button cmdCancel = new Button();
			cmdCancel.Width = ( this.ClientSize.Width - 3 * space ) / 2;
			cmdCancel.Left = space + cmdOk.Right;
			cmdCancel.Top = cb.Bottom + space;
			cmdCancel.Text = cancel; //language.getAString(); //language.order.cancel );//cancel;
			cmdCancel.Click += new EventHandler(cmdCancel_Click);
#if !CF
			cmdCancel.FlatStyle = FlatStyle.System;
#endif

			this.Controls.Add( lbl );
			this.Controls.Add( cmdOk );
			this.Controls.Add( cmdCancel );
			this.Controls.Add( cb );

			platformSpec.resolution.set( this.Controls );

			this.ClientSize = new Size( this.ClientSize.Width, space * 4 + lbl.Height + cb.Height + cmdOk.Height ); // this.Height = space * 4 + lbl.Height + cb.Height + cmdOk.Height;

			platformSpec.setFloatingWindow.after( this );
			
			this.ClientSize = new Size( this.ClientSize.Width, space + cmdOk.Bottom ); //.Height = space * 4 + lbl.Height + tb.Height + cmdOk.Height;

			this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
			this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;

			cb.Focus();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		private void cmdOk_Click(object sender, EventArgs e)
		{
			result = cb.SelectedIndex;
			this.Close();
		}
		private void cmdCancel_Click(object sender, EventArgs e)
		{
			result = -1;
			this.Close();
		}
	}
}
