using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for createStatistics.civilizations.
	/// </summary>
	public class createCivStats : System.Windows.Forms.Form
	{
		int space = 8;
		public ComboBox cbCityList;
		public TextBox tbNationName, tbDescription;
		public TrackBar[] tbColors;
		PictureBox pbColor;
		Graphics g;
		Pen blackPen;
		System.Drawing.Bitmap bmp;
		Button cmdOk, cmdCancel;
	//	Microsoft.WindowsCE.Forms.InputPanel ip;

		public bool resultAccepted;

		public createCivStats()
		{
			//
			// Required for Windows Form Designer support
			//
		//	InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.Menu = new MainMenu();
			this.ControlBox = false;

			bmp = new Bitmap( 1, 1 );
			g = Graphics.FromImage( bmp );

			Label lblNationName = new Label();
			lblNationName.Location = new Point( space, space );
			lblNationName.Text = language.getAString( language.order.cnNationName );
			lblNationName.Width = (int)g.MeasureString( lblNationName.Text, lblNationName.Font ).Width + 4;//this.Width - 2 * space;
			this.Controls.Add( lblNationName );
			lblNationName.SendToBack();

			tbNationName = new TextBox();
			tbNationName.Width = this.Width - 2 * space - lblNationName.Right;/// 2;
			tbNationName.Location = new Point( this.Width - space - tbNationName.Width, space );
			tbNationName.TextChanged += new EventHandler(tbNationName_TextChanged);
			tbNationName.GotFocus += new EventHandler(tbNationName_GotFocus);
			tbNationName.LostFocus += new EventHandler(tbNationName_LostFocus);
			this.Controls.Add( tbNationName );

			Label lblCityList = new Label();
			lblCityList.Location = new Point( space, space + lblNationName.Bottom );
			lblCityList.Text = language.getAString( language.order.cnImportCityFrom );
			lblCityList.Width = (int)g.MeasureString( lblCityList.Text, lblCityList.Font ).Width + 4;//this.Width - 2 * space;
			this.Controls.Add( lblCityList );
			lblCityList.SendToBack();

			cbCityList = new ComboBox();
			cbCityList.Width = this.Width - 2 * space - lblCityList.Right;// 2;
			cbCityList.Location = new Point( lblCityList.Right + space, space + tbNationName.Bottom );
			this.Controls.Add( cbCityList );

			for ( int c = 0; c < Statistics.normalCivilizationNumber; c ++ )
				cbCityList.Items.Add( Statistics.civilizations[ c ].name );

			Random r = new Random();
			cbCityList.SelectedIndex = r.Next( Statistics.normalCivilizationNumber );

			Label lblColor = new Label();
			lblColor.Location = new Point( space, space + lblCityList.Bottom );
			lblColor.Width = this.Width - 2 * space;
			lblColor.Text = language.getAString( language.order.cnColor );
			this.Controls.Add( lblColor );

			pbColor = new PictureBox();
			pbColor.Width = this.Width / 4;// tbColors[ 2 ].Bottom - tbColors[ 0 ].Top - space );
		//	pbColor.Location = new Point( tbColors[ 0 ].Right + space, tbColors[ 0 ].Top );
			this.Controls.Add( pbColor );
			
		//	bmp = new Bitmap( pbColor.Width, pbColor.Height );

			Label[] lblColors = new Label[ 3 ];
			tbColors = new TrackBar[ 3 ];

			int colorStrWidth = (int)g.MeasureString( language.getAString( language.order.cnRed ) + " ", lblColor.Font ).Width;
			if ( colorStrWidth < g.MeasureString( language.getAString( language.order.cnGreen ) + " ", lblColor.Font ).Width )
				colorStrWidth = (int)g.MeasureString( language.getAString( language.order.cnGreen ) + " ", lblColor.Font ).Width;
			if ( colorStrWidth < g.MeasureString( language.getAString( language.order.cnBlue ) + " ", lblColor.Font ).Width )
				colorStrWidth = (int)g.MeasureString( language.getAString( language.order.cnBlue ) + " ", lblColor.Font ).Width;
			

			for ( int i = 0; i < tbColors.Length; i++ )
			{
				lblColors[ i ] = new Label();
				lblColors[ i ].Left = space;
				lblColors[ i ].Width = colorStrWidth;
				this.Controls.Add( lblColors[ i ] );

				tbColors[ i ] = new TrackBar();
				tbColors[ i ].Minimum = 0;
				tbColors[ i ].Maximum = 255;
				tbColors[ i ].LargeChange = 16;
				tbColors[ i ].SmallChange = 1;
				tbColors[ i ].TickFrequency = 64;
				tbColors[ i ].Left = lblColors[ i ].Right;
				tbColors[ i ].Width = this.Width - pbColor.Width - 3 * space - lblColors[ i ].Width;
				tbColors[ i ].ValueChanged += new EventHandler(tbColors_ValueChanged);
				this.Controls.Add( tbColors[ i ] );
			}
			
			lblColors[ 0 ].Text = language.getAString( language.order.cnRed );
			lblColors[ 1 ].Text = language.getAString( language.order.cnGreen );
			lblColors[ 2 ].Text = language.getAString( language.order.cnBlue );

			tbColors[ 0 ].Top = lblColor.Bottom;
			tbColors[ 1 ].Top = tbColors[ 0 ].Bottom;
			tbColors[ 2 ].Top = tbColors[ 1 ].Bottom;
			lblColors[ 0 ].Top = lblColor.Bottom;
			lblColors[ 1 ].Top = tbColors[ 0 ].Bottom;
			lblColors[ 2 ].Top = tbColors[ 1 ].Bottom;

			pbColor.Height = tbColors[ 2 ].Bottom - tbColors[ 0 ].Top - space;
			pbColor.Location = new Point( this.Width - space - pbColor.Width, tbColors[ 0 ].Top );

			blackPen = new Pen( Color.Black );
			
			bmp = new Bitmap( pbColor.Width, pbColor.Height );
			g = Graphics.FromImage( bmp );
			
			for ( int i = 0; i < tbColors.Length; i++ )
				tbColors[ i ].Value = 255;

			cmdOk = new Button();
			cmdOk.Text = language.getAString( language.order.ok );
			cmdOk.Height = 24;
			cmdOk.Width = (this.Width - 3 * space) / 2;
			cmdOk.Location = new Point( space, this.Height - space - cmdOk.Height );
			cmdOk.Enabled = false;
			cmdOk.Click += new EventHandler(cmdOk_Click);
#if !CF
			cmdOk.FlatStyle = FlatStyle.System;
#endif
			this.Controls.Add( cmdOk );

			cmdCancel = new Button();
			cmdCancel.Text = language.getAString( language.order.cancel );
			cmdCancel.Height = 24;
			cmdCancel.Width = (this.Width - 3 * space) / 2;
			cmdCancel.Location = new Point( cmdOk.Right + space, this.Height - space - cmdCancel.Height );
			cmdCancel.Click += new EventHandler(cmdCancel_Click);
#if !CF
			cmdOk.FlatStyle = FlatStyle.System;
#endif
			this.Controls.Add( cmdCancel );

			platformSpec.resolution.set( this.Controls );

		//	ip = new Microsoft.WindowsCE.Forms.InputPanel();
			tbNationName.Focus(); // = true;
		//	ip.Enabled = true;
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Text = "createCivStats";
		}
#endregion

		private void tbColors_ValueChanged(object sender, EventArgs e)
		{
			g.Clear( Color.FromArgb( tbColors[ 0 ].Value, tbColors[ 1 ].Value, tbColors[ 2 ].Value ) );

			g.DrawRectangle(
				blackPen,
				0, 0, 
				pbColor.Width - 1,pbColor.Height - 1
				);
			
			pbColor.Image = bmp;
		}
		private void cmdOk_Click(object sender, EventArgs e)
		{
			resultAccepted = true;
			this.Close();
		}
		private void cmdCancel_Click(object sender, EventArgs e)
		{
			resultAccepted = false;
			this.Close();
		}
		private void tbNationName_TextChanged(object sender, EventArgs e)
		{
			if ( tbNationName.Text.Length > 0 && tbNationName.Text.TrimEnd( " ".ToCharArray() ).Length > 0 )
				cmdOk.Enabled = true;
			else
				cmdOk.Enabled = false;
		}

		private void tbNationName_GotFocus(object sender, EventArgs e)
		{
			platformSpec.main.showSip();
		//	ip.Enabled = true;
		}
		private void tbNationName_LostFocus(object sender, EventArgs e)
		{
		//	ip.Enabled = false;
		}
	}
}
