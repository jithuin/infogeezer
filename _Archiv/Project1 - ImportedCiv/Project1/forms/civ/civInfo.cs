using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for civInfo.
	/// </summary>
	public class civInfo : System.Windows.Forms.Form
	{
	//	private System.Windows.Forms.Label label1;
	//	private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbGov;
		private System.Windows.Forms.ComboBox cbEco;
		private System.Windows.Forms.Button cmdAdopt;
		byte[] cbGovInd, cbEcoInd;
	
		public civInfo()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			cbGovInd = new byte[ Statistics.governements.Length ];
			cbEcoInd = new byte[ Statistics.economies.Length ];

			for ( int i = 0, j = 0; i < Statistics.governements.Length; i++ )
				if ( Form1.game.playerList[ Form1.game.curPlayerInd ].canSwithToGov( (enums.governements)i ) || i == Form1.game.playerList[ Form1.game.curPlayerInd ].govType.type )
				{
					cbGov.Items.Add( Statistics.governements[ i ].name );
					cbGovInd[ j ] = (byte)i;

					if ( i == (byte)Form1.game.playerList[ Form1.game.curPlayerInd ].govType.type )
						cbGov.SelectedIndex = j;

					j++;
				}

			for ( int i = 0, j = 0; i < Statistics.economies.Length; i++ )
				if ( Form1.game.playerList[ Form1.game.curPlayerInd ].canSwithToEco( i ) || i == Form1.game.playerList[ Form1.game.curPlayerInd ].economyType )
				{
					cbEco.Items.Add( Statistics.economies[ i ].name );
					cbEcoInd[ j ] = (byte)i;

					if ( i == Form1.game.playerList[ Form1.game.curPlayerInd ].economyType )
						cbEco.SelectedIndex = j;

					j++;
				}

			this.cbGov.SelectedIndexChanged += new EventHandler( cbGov_SelectedIndexChanged );
			this.cbEco.SelectedIndexChanged += new EventHandler( cbEco_SelectedIndexChanged );

			platformSpec.resolution.set( this.Controls );

			enableAdopt();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
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
			this.cbGov = new System.Windows.Forms.ComboBox();
			this.cbEco = new System.Windows.Forms.ComboBox();
			this.cmdAdopt = new System.Windows.Forms.Button();
			// 
			// cbGov
			// 
			this.cbGov.Location = new System.Drawing.Point(8, 144);
			// 
			// cbEco
			// 
			this.cbEco.Location = new System.Drawing.Point(8, 184);
			// 
			// cmdAdopt
			// 
			this.cmdAdopt.Location = new System.Drawing.Point(152, 264);
			this.cmdAdopt.Text = "Adopt";
			this.cmdAdopt.Click += new System.EventHandler(this.cmdAdopt_Click);
			// 
			// civInfo
			// 
			this.ClientSize = new System.Drawing.Size(240, 296);
			this.Controls.Add(this.cmdAdopt);
			this.Controls.Add(this.cbEco);
			this.Controls.Add(this.cbGov);
			this.Text = "Civilization info";

		}
		#endregion

		private void cmdAdopt_Click(object sender, System.EventArgs e)
		{
			Form1.game.playerList[ Form1.game.curPlayerInd ].economyType = cbEcoInd[ cbEco.SelectedIndex ];
			Form1.game.playerList[ Form1.game.curPlayerInd ].govType = Statistics.governements[ cbGovInd[ cbGov.SelectedIndex ] ];
			enableAdopt();
		}

		private void enableAdopt() 
		{ 
			if ( 
				cbGovInd[ cbGov.SelectedIndex ] != Form1.game.playerList[ Form1.game.curPlayerInd ].govType.type || 
				cbEcoInd[ cbEco.SelectedIndex ] != Form1.game.playerList[ Form1.game.curPlayerInd ].economyType 
				) 
				cmdAdopt.Enabled = true; 
			else 
				cmdAdopt.Enabled = false; 
		}

		private void cbGov_SelectedIndexChanged(object sender, EventArgs e)
		{
			enableAdopt();
		}

		private void cbEco_SelectedIndexChanged(object sender, EventArgs e)
		{
			enableAdopt();
		}
		
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			e.Graphics.Clear( Form1.defaultBackColor );
			Font nationFont = new Font( "Tahoma", 16, FontStyle.Regular);
			e.Graphics.DrawString( 
				Form1.game.playerList[ Form1.game.curPlayerInd ].civName,
				nationFont,
				new SolidBrush( Color.Black ),
				1,
				0
				);

			Font playerFont = new Font( "Tahoma", 12, FontStyle.Regular);
			e.Graphics.DrawString( 
				Form1.game.playerList[ Form1.game.curPlayerInd ].playerName,
				playerFont,
				new SolidBrush( Color.Black ),
				1,
				e.Graphics.MeasureString( Form1.game.playerList[ Form1.game.curPlayerInd ].civName, nationFont ).Height
				);

			Font smalTitleFont = new Font( "Tahoma", 10, FontStyle.Regular );
			e.Graphics.DrawString( 
				"Governement",
				smalTitleFont,
				new SolidBrush( Color.Black ),
				1,
				cbGov.Top - e.Graphics.MeasureString( "Governement", smalTitleFont ).Height
				);
			e.Graphics.DrawString( 
				"Economy",
				smalTitleFont,
				new SolidBrush( Color.Black ),
				1,
				cbEco.Top - e.Graphics.MeasureString( "Economy", smalTitleFont ).Height
				);
		}
	}
}
