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
	public class userNumberInput : System.Windows.Forms.Form
	{
		public int result;
		TextBox tb;
		Button cmdOk, cmdCancel;
	//	Microsoft.WindowsCE.Forms.InputPanel ip;
		bool onlyNumbers;
		char[] ca;
		long max;

		public userNumberInput(string title, string text, long Default, long Max, string ok, string cancel)
		{
			platformSpec.setFloatingWindow.before( this );
			platformSpec.manageWindows.setUserInputSize( this );
			max = Max;
			ca = "0123456789".ToCharArray();
			int space = 8;
			//this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; 
			this.ControlBox = false; 

			this.Text = title; 
			this.Menu = new MainMenu();
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
				lineWidth += (int)g.MeasureString( words[ i ] + ( i < words.Length - 1 ? " " : "" ), lbl.Font ).Width;
				if ( lineWidth >= lbl.Width )
				{
					lines++;
					lineWidth = 0;
					i--;
				}
			}

			lbl.Height = lines * (int)g.MeasureString( words[0 ] + " ", lbl.Font ).Height;

			g.MeasureString( text, lbl.Font );
			
			tb = new TextBox();
			tb.Left = space;
			tb.Top = lbl.Bottom + space;
			tb.Width = this.ClientSize.Width - 2 * space;
			tb.Text = Default.ToString();
			tb.TextChanged += new EventHandler( tb_TextChanged );
			tb.GotFocus += new EventHandler( tb_GotFocus );
			tb.LostFocus += new EventHandler(tb_LostFocus);

			cmdOk = new Button();
			cmdOk.Width = ( this.ClientSize.Width - 3 * space ) / 2;
			cmdOk.Left = space;
			cmdOk.Top = tb.Bottom + space;
			cmdOk.Text = ok; //language.getAString(); //language.order.ok );//ok;
			cmdOk.Click += new EventHandler(cmdOk_Click);
#if !CF
			cmdOk.FlatStyle = FlatStyle.System;
#endif
			
			cmdCancel = new Button();
			cmdCancel.Width = ( this.ClientSize.Width - 3 * space ) / 2;
			cmdCancel.Left = space + cmdOk.Right;
			cmdCancel.Top = tb.Bottom + space;
			cmdCancel.Text = cancel; //language.getAString(); //language.order.cancel );//cancel;
			cmdCancel.Click += new EventHandler(cmdCancel_Click);
#if !CF
			cmdCancel.FlatStyle = FlatStyle.System;
#endif

		//	ip = new Microsoft.WindowsCE.Forms.InputPanel();
			//	ip.Enabled = true;
			platformSpec.main.showSip();

			this.Controls.Add( lbl );
			this.Controls.Add( cmdOk );
			this.Controls.Add( cmdCancel );
			this.Controls.Add( tb );
			
			platformSpec.resolution.set( this.Controls );

			this.ClientSize = new Size( this.ClientSize.Width, space * 4 + lbl.Height + tb.Height + cmdOk.Height ); // this.Height = space * 4 + lbl.Height + tb.Height + cmdOk.Height;

			platformSpec.setFloatingWindow.after( this );

			this.ClientSize = new Size( this.ClientSize.Width, space * 4 + lbl.Height + tb.Height + cmdOk.Height ); //.Height = space * 4 + lbl.Height + tb.Height + cmdOk.Height;

			this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2 - this.Height / 2;
			this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2 - this.Width / 2;

			tb.Focus();
			tb.SelectionLength = tb.Text.Length ;
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
			result = Convert.ToInt32( tb.Text, 10 );
			this.Close();
		}
		private void cmdCancel_Click(object sender, EventArgs e)
		{
			result = -1;
			this.Close();
		}
		private void tb_TextChanged(object sender, EventArgs e)
		{
			if ( tb.Text.Length == 0 )
				cmdOk.Enabled = false;
			else
			{
				string newText = "";
				char[] oldText = tb.Text.ToCharArray();
				for ( int i = 0; i < oldText.Length; i ++ )
				{
					bool isNbr = false;
					for ( int c = 0; c < ca.Length; c ++ )
						if ( oldText[ i ] == ca[ c ] )
						{
							isNbr = true;
							break;
						}

					if ( isNbr )
					{
						newText += oldText[ i ];
					}
				}
				newText.TrimEnd( " ".ToCharArray() );

				if ( tb.Text.Length != newText.Length )
				{
					tb.Text = newText;
					tb.SelectionStart = tb.Text.Length;
				}

				if ( Convert.ToInt32( tb.Text, 10 ) > max )
				{
					tb.Text = max.ToString();
					tb.SelectionStart = tb.Text.Length;
				}


				if ( tb.Text.Length == 0 )
					cmdOk.Enabled = false;
				else
					cmdOk.Enabled = true;
			}
		}
		private void tb_GotFocus(object sender, EventArgs e)
		{
			platformSpec.main.showSip();
		//	ip.Enabled = true;
		}
		private void tb_LostFocus(object sender, EventArgs e)
		{
		//	ip.Enabled = false;
		}
	}
}
