using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for civPref.
	/// </summary>
	public class civPref : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.TrackBar trackBar2;
		private System.Windows.Forms.TrackBar trackBar3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox checkBoxSci;
		private System.Windows.Forms.CheckBox checkBoxWealth;
		private System.Windows.Forms.CheckBox checkBoxHappy;
		private System.Windows.Forms.Label label3;
		byte[] values;
		int nationTrade;
		structures.preferences pref;

		SlideBar sbReserve, sbMilitary, sbScience, sbCulture, sbSpies, sbBuildings, sbSpace, sbExchanges;
	
		public civPref()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			getPFT getPft = new getPFT();
			nationTrade = Form1.game.playerList[ Form1.game.curPlayerInd ].totalTrade;

		/*	trackBar1.Value = Form1.game.playerList[ 0 ].preferences.laborProd;
			trackBar2.Value = Form1.game.playerList[ 0 ].preferences.laborTrade;
			trackBar3.Value = Form1.game.playerList[ 0 ].preferences.laborFood;*/

			pref = Form1.game.playerList[ Form1.game.curPlayerInd ].preferences;

			values = new byte[ 6 ];

#region back (color)
	/*		PictureBox pb = new PictureBox();
			pb.Parent = this;

			pb.Location = new Point( 0, 0 );
			pb.Size = new Size( this.Width, this.Height );

			Bitmap back = new Bitmap( this.Width, this.Height );
			Graphics g = Graphics.FromImage( back );
			g.Clear( Form1.defaultBackColor );

			pb.Image = back;*/
#endregion

			
			platformSpec.resolution.set( this.Controls );

			affSlideBars();
			updateSlideBars();
			setValues();
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
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.trackBar2 = new System.Windows.Forms.TrackBar();
			this.trackBar3 = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.checkBoxSci = new System.Windows.Forms.CheckBox();
			this.label9 = new System.Windows.Forms.Label();
			this.checkBoxWealth = new System.Windows.Forms.CheckBox();
			this.checkBoxHappy = new System.Windows.Forms.CheckBox();
			// 
			// trackBar1
			// 
			this.trackBar1.LargeChange = 1;
			this.trackBar1.Location = new System.Drawing.Point(48, 176);
			this.trackBar1.Size = new System.Drawing.Size(168, 45);
			this.trackBar1.Visible = false;
			this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
			// 
			// trackBar2
			// 
			this.trackBar2.LargeChange = 1;
			this.trackBar2.Location = new System.Drawing.Point(48, 240);
			this.trackBar2.Size = new System.Drawing.Size(168, 45);
			this.trackBar2.Visible = false;
			this.trackBar2.ValueChanged += new System.EventHandler(this.trackBar2_ValueChanged);
			// 
			// trackBar3
			// 
			this.trackBar3.LargeChange = 1;
			this.trackBar3.Location = new System.Drawing.Point(48, 208);
			this.trackBar3.Size = new System.Drawing.Size(168, 45);
			this.trackBar3.Visible = false;
			this.trackBar3.ValueChanged += new System.EventHandler(this.trackBar3_ValueChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-8, 184);
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.Text = "Prod.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.label1.Visible = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(0, 216);
			this.label2.Size = new System.Drawing.Size(56, 16);
			this.label2.Text = "Food";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.label2.Visible = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 248);
			this.label3.Size = new System.Drawing.Size(56, 16);
			this.label3.Text = "Trade";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.label3.Visible = false;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular);
			this.label8.Location = new System.Drawing.Point(8, 152);
			this.label8.Size = new System.Drawing.Size(176, 24);
			this.label8.Text = "Automatic labor position:";
			this.label8.Visible = false;
			// 
			// checkBoxSci
			// 
			this.checkBoxSci.Location = new System.Drawing.Point(216, 40);
			this.checkBoxSci.Size = new System.Drawing.Size(16, 16);
			this.checkBoxSci.Text = "checkBoxSci";
			this.checkBoxSci.Visible = false;
			this.checkBoxSci.CheckStateChanged += new System.EventHandler(this.checkBoxSci_CheckStateChanged);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(208, 16);
			this.label9.Size = new System.Drawing.Size(32, 16);
			this.label9.Text = "Lock";
			this.label9.Visible = false;
			// 
			// checkBoxWealth
			// 
			this.checkBoxWealth.Location = new System.Drawing.Point(216, 72);
			this.checkBoxWealth.Size = new System.Drawing.Size(16, 16);
			this.checkBoxWealth.Text = "checkBoxWealthlth";
			this.checkBoxWealth.Visible = false;
			this.checkBoxWealth.CheckStateChanged += new System.EventHandler(this.checkBoxWealth_CheckStateChanged);
			// 
			// checkBoxHappy
			// 
			this.checkBoxHappy.Location = new System.Drawing.Point(216, 104);
			this.checkBoxHappy.Size = new System.Drawing.Size(16, 16);
			this.checkBoxHappy.Text = "checkBoxHappypy";
			this.checkBoxHappy.Visible = false;
			this.checkBoxHappy.CheckStateChanged += new System.EventHandler(this.checkBoxHappy_CheckStateChanged);
			// 
			// civPref
			// 
			this.ClientSize = new System.Drawing.Size(240, 288);
			this.Controls.Add(this.checkBoxHappy);
			this.Controls.Add(this.checkBoxWealth);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.checkBoxSci);
			this.Controls.Add(this.trackBar2);
			this.Controls.Add(this.trackBar3);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.trackBar1);
			this.Controls.Add(this.label8);
			this.Text = "Preferences";
			this.Closed += new System.EventHandler(this.civPref_Closed);

		}
		#endregion

		private void civPref_Closed( object sender, System.EventArgs e )
		{
		/*	Form1.game.playerList[ 0 ].preferences.laborProd = (sbyte)trackBar1.Value;
			Form1.game.playerList[ 0 ].preferences.laborFood = (sbyte)trackBar3.Value;
			Form1.game.playerList[ 0 ].preferences.laborTrade = (sbyte)trackBar2.Value;*/

			Form1.game.playerList[ Form1.game.curPlayerInd ].preferences = pref;

		/*	Form1.game.playerList[ 0 ].preferences.science = (byte)trackBarSci.Value;
			Form1.game.playerList[ 0 ].preferences.reserve = (byte)trackBarWealth.Value;
			Form1.game.playerList[ 0 ].prefHapiness = (byte)trackBarHappy.Value;*/
		}
		


		private void checkBoxSci_CheckStateChanged(object sender, System.EventArgs e)
		{ // sci
		//	checkChecked();
			
		}

		private void checkBoxWealth_CheckStateChanged(object sender, System.EventArgs e)
		{ // wealth
		//	checkChecked();
		
		}

		private void checkBoxHappy_CheckStateChanged(object sender, System.EventArgs e)
		{ // happiness
		//	checkChecked();
		
		}

		private void trackBar1_ValueChanged(object sender, System.EventArgs e)
		{ // pref prod
			Form1.game.playerList[ 0 ].preferences.laborProd = (sbyte)trackBar1.Value;
		}

		private void trackBar3_ValueChanged(object sender, System.EventArgs e)
		{ // pref food
			Form1.game.playerList[ 0 ].preferences.laborFood = (sbyte)trackBar3.Value;
		}

		private void trackBar2_ValueChanged(object sender, System.EventArgs e)
		{ // pref trade
			Form1.game.playerList[ 0 ].preferences.laborTrade = (sbyte)trackBar2.Value;
		}

		private void affSlideBars()
		{
			int i = 0, xMin = 10, height = 24 * platformSpec.resolution.mod, width = this.ClientSize.Width - 16, dest = 8;

		#region sbReserve
			sbReserve = new SlideBar( 
				xMin, 
				i * ( height + dest ) + dest,
				width,
				height,
				-100,
				100
				);
			i ++;

			sbReserve.backColor = Form1.defaultBackColor;
			sbReserve.picBox.Parent = this;
			sbReserve.maxInfo = " %";
			sbReserve.showMax = true;
			sbReserve.defaultValue = Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.reserve;

			sbReserve.red = (int)(- 1 * ( Form1.game.playerList[ Form1.game.curPlayerInd ].money * 100 ) / ( nationTrade * 10 ));
			sbReserve.yellow = 0;

			sbReserve.enabled = false;
			sbReserve.Text= "Reserve";
			sbReserve.drawSlider();
			#endregion

		#region sbMilitary
			sbMilitary = new SlideBar( 
				xMin, 
				i * ( height + dest ) + dest,
				width,
				height,
				0,
				100
				);
			i ++;

			int k = Form1.game.playerList[ Form1.game.curPlayerInd ].totMilitaryUnits; //count.militaryUnits( Form1.game.curPlayerInd );
			/*int k = 0;
			for ( int j = 0; j <= Form1.game.playerList[ Form1.game.curPlayerInd ].unitNumber; j ++ )
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].unitList[ j ].state != (byte)Form1.unitState.dead &&
					Statistics.units[ Form1.game.playerList[ Form1.game.curPlayerInd ].unitList[ j ].type ].speciality != enums.speciality.builder
					)
				{
					k ++;
				}*/

			if ( k > 0 )
			{
				sbMilitary.yellow = ( k * 3 * 100 ) / nationTrade;
				sbMilitary.red = ( k * 3 * 100 * 4 / 5 ) / nationTrade;
			}

			sbMilitary.backColor = Form1.defaultBackColor;
			sbMilitary.picBox.Parent = this;
		//	sbMilitary.ValueMax = 100;
			sbMilitary.Value = 50;
			sbMilitary.defaultValue = Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.military;
			sbMilitary.enabled = true;
			sbMilitary.Text= "Military";
			sbMilitary.picBox.MouseUp += new MouseEventHandler(military_MouseUp);
			sbMilitary.drawSlider();
			#endregion

		#region sbScience
			sbScience = new SlideBar( 
				xMin, 
				i * ( height + dest ) + dest,
				width,
				height,
				0,
				100
				);
			i ++;

			sbScience.backColor = Form1.defaultBackColor;
			sbScience.picBox.Parent = this;
		//	sbScience.ValueMax = 100;
			sbScience.Value = 50;
			sbScience.defaultValue = Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.science;
			sbScience.enabled = true;
			sbScience.Text= "Science";
			sbScience.picBox.MouseUp += new MouseEventHandler(science_MouseUp);
			sbScience.drawSlider();
			#endregion

		#region sbCulture
			sbCulture = new SlideBar( 
				xMin, 
				i * ( height + dest ) + dest,
				width,
				height,
				0,
				100
				);
			i ++;

			sbCulture.backColor = Form1.defaultBackColor;
			sbCulture.picBox.Parent = this;
		//	sbCulture.ValueMax = 100;
			sbCulture.Value = 50;
			sbCulture.defaultValue = Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.culture;
			sbCulture.enabled = true;
			sbCulture.Text= "Culture";
			
	//		byte playerCultPos = 1;
			byte lowerPlayer = 0;
			bool found = false;
			int playerCult = ( Form1.game.playerList[ Form1.game.curPlayerInd ].culture * - 10 / 100 + pref.culture * nationTrade ) * 5; // turns

			for ( int j = 0; j < Form1.game.playerList.Length; j ++ )
			{
				if ( 
					i != Form1.game.curPlayerInd && 
					Form1.game.playerList[ j ].culture < playerCult &&
					( Form1.game.playerList[ j ].culture > Form1.game.playerList[ lowerPlayer ].culture || !found )
					)
				{
					lowerPlayer = (byte)j;
					found = true;
				}
			}

			if ( found )
				sbCulture.red = - ( - 10 * Form1.game.playerList[ Form1.game.curPlayerInd ].culture * 5 - 100 * Form1.game.playerList[ lowerPlayer ].culture ) / ( 100 * nationTrade * 5 );

			sbCulture.picBox.MouseUp += new MouseEventHandler(culture_MouseUp);
			sbCulture.drawSlider();
		#endregion

		#region sbSpies
			sbSpies = new SlideBar( 
				xMin, 
				i * ( height + dest ) + dest,
				width,
				height,
				0,
				100
				);
			i ++;

			sbSpies.backColor = Form1.defaultBackColor;
			sbSpies.picBox.Parent = this;
	//		sbSpies.ValueMax = 100;
			sbSpies.Value = 50;
			sbSpies.defaultValue = Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.intelligence;
			sbSpies.enabled = true;
			sbSpies.Text= "Intelligence";
			sbSpies.picBox.MouseUp += new MouseEventHandler(spies_MouseUp);
			sbSpies.drawSlider();
			#endregion

		#region sbBuildings
			sbBuildings = new SlideBar( 
				xMin, 
				i * ( height + dest ) + dest,
				width,
				height,
				0,
				100
				);
			i ++;

			int upkeepCost = count.upkeepCost( Form1.game.curPlayerInd );

			if ( upkeepCost > 0 )
				sbBuildings.red = upkeepCost * 100 / nationTrade;

			sbBuildings.backColor = Form1.defaultBackColor;
			sbBuildings.picBox.Parent = this;
			sbBuildings.Value = 50;
			sbBuildings.defaultValue = Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.buildings;
			sbBuildings.enabled = true;
			sbBuildings.Text= "Buildings upkeep";
			sbBuildings.picBox.MouseUp += new MouseEventHandler(buildings_MouseUp);
			sbBuildings.drawSlider();
			#endregion

		#region sbSpace
			sbSpace = new SlideBar( 
				xMin, 
				i * ( height + dest ) + dest,
				width,
				height,
				0,
				100
				);

			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].technos[ (byte)Form1.technoList.ancientDemocraty ].researched )
			{
				i ++;

				sbSpace.backColor = Form1.defaultBackColor;
				sbSpace.picBox.Parent = this;
				sbSpace.Value = 50;
				sbSpace.defaultValue = Form1.game.playerList[ Form1.game.curPlayerInd ].preferences.space;
				sbSpace.enabled = true;
				sbSpace.Text= "Space program";
				sbSpace.picBox.MouseUp += new MouseEventHandler(space_MouseUp);
				sbSpace.drawSlider();
			}
			#endregion

		#region sbExchanges
			sbExchanges = new SlideBar( 
				xMin, 
				i * ( height + dest ) + dest,
				width,
				height,
				- 100,
				100
				);
			i ++;

			sbExchanges.backColor = Form1.defaultBackColor;
			sbExchanges.picBox.Parent = this; 
			sbExchanges.ValueMax = 100;
			sbExchanges.ValueMin = -100; 
			sbExchanges.Value = pref.exchanges; 
			sbExchanges.enabled = false; 
			sbExchanges.Text = "International exchanges"; 

			int goldPerTurn = 0;// = pref.exchanges * nationTrade / 100;
			int[ , ] exs = Form1.game.playerList[ Form1.game.curPlayerInd ].memory.getCurrentExchanges();
			for ( int p = 0; p < Form1.game.playerList.Length; p++ )
				goldPerTurn += exs[ p, 0 ];

			sbExchanges.Text2 = goldPerTurn.ToString() + " gold/turn";
			sbExchanges.drawSlider(); 
			
		#endregion 

		}

		private void updateSlideBars()
		{

		#region sbReserve

			sbReserve.Value = pref.reserve;
			sbReserve.drawSlider();
			#endregion

		#region sbMilitary

			sbMilitary.Value = pref.military;
			//sbMilitary.drawSlider();
			#endregion

		#region sbScience

			sbScience.Value = pref.science;
			sbScience.drawSlider();
			#endregion

		#region sbCulture

			sbCulture.Value = pref.culture;
			sbCulture.drawSlider();
			#endregion

		#region sbSpies

			sbSpies.Value = pref.intelligence;
			sbSpies.drawSlider();
			#endregion

		#region sbBuildings

			sbBuildings.Value = pref.buildings;
			sbBuildings.drawSlider();
			#endregion

		#region sbSpace

			sbSpace.Value = pref.space;
		/*	sbSpace.red = 40;
			sbSpace.green = 100;	*/
			sbSpace.drawSlider();
#endregion

		#region sbExchanges

		/*	sbExchanges.Value = pref.exchanges;
			sbExchanges.drawSlider();*/

		#endregion

		}

		private void setValues()
		{
			int tot = pref.buildings + pref.culture + pref.intelligence + pref.military + pref.space + pref.science + pref.exchanges;
			int left = 200 - tot;
			pref.reserve = (sbyte)( 100 - tot);
			sbReserve.Value = (sbyte)( 100 - tot);
			drawReserve();

			sbMilitary.max = sbMilitary.Value + left;
			sbScience.max = sbScience.Value + left;
			sbCulture.max = sbCulture.Value + left;
			sbSpies.max = sbSpies.Value + left;
			sbBuildings.max = sbBuildings.Value + left;
			sbSpace.max = sbSpace.Value + left;

			drawMilitary();
			drawScience();
			drawSpies();
			drawCulture();

			Form1.game.playerList[ Form1.game.curPlayerInd ].preferences = pref;
		}

		private void military_MouseUp(object sender, MouseEventArgs e)
		{
			pref.military = (sbyte)sbMilitary.Value;
			setValues();
		}

		private void science_MouseUp(object sender, MouseEventArgs e)
		{
			pref.science = (sbyte)sbScience.Value;
			setValues();
		}

		private void culture_MouseUp(object sender, MouseEventArgs e)
		{
			pref.culture = (sbyte)sbCulture.Value;
			setValues();
		}

		private void spies_MouseUp(object sender, MouseEventArgs e)
		{
			pref.intelligence = (sbyte)sbSpies.Value;
			setValues();
		}

		private void buildings_MouseUp(object sender, MouseEventArgs e)
		{
			pref.buildings = (sbyte)sbBuildings.Value;
			setValues();
		}

		private void space_MouseUp(object sender, MouseEventArgs e)
		{
			pref.space = (sbyte)sbSpace.Value;
			setValues();
		}

		private void drawReserve()
		{
			sbReserve.Text2 = String.Format( "{0} gold / turn", nationTrade * pref.reserve / 100 ) ;
			sbReserve.drawSlider();
		}

		private void drawScience()
		{
			if ( nationTrade > 0 && pref.science > 0 )
			{
				float sciTurn = ( Statistics.technologies[ Form1.game.playerList[ Form1.game.curPlayerInd ].currentResearch ].cost - Form1.game.playerList[ Form1.game.curPlayerInd ].technos[ Form1.game.playerList[ Form1.game.curPlayerInd ].currentResearch ].pntDiscovered ) * 100 / ( nationTrade * pref.science );

				if ( Convert.ToInt32( sciTurn ) > 1 )
				{
					sbScience.Text2 = Convert.ToInt32( sciTurn ).ToString() + " turns left";
				}
				else
				{
					sbScience.Text2 = Convert.ToInt32( sciTurn ).ToString() + " turn left";
				}

				sbScience.drawSlider();
			}
		}

		private void drawMilitary()
		{
			int units = ( nationTrade * pref.military / 100 ) / 3;

			if ( Convert.ToInt32( units ) > 1 )
			{
				sbMilitary.Text2 = Convert.ToInt32( units ).ToString() + " units";
			}
			else
			{
				sbMilitary.Text2 = Convert.ToInt32( units ).ToString() + " unit";
			}

			sbMilitary.drawSlider();
		}
		
		private void drawSpies()
		{
			if ( nationTrade > 0 && pref.intelligence >= 0 )
			{
				float spyNbr = ( nationTrade * pref.intelligence / 100 ) / Form1.goldPerSpy;

				if ( Convert.ToInt32( spyNbr ) > 1 )
				{
					sbSpies.Text2 = Convert.ToInt32( spyNbr ).ToString() + " spies";
				}
				else
				{
					sbSpies.Text2 = Convert.ToInt32( spyNbr ).ToString() + " spy";
				}

				sbSpies.drawSlider();
			}
		}

		private void drawCulture()
		{
		/*	byte playerCultPos = 1;
			int playerCult = ( Form1.game.playerList[ Form1.game.curPlayerInd ].culture * - 10 / 100 + pref.culture * nationTrade ) * 5; // turns

			for ( int i = 0; i < Form1.game.playerList.Length; i ++ )
				if ( i != Form1.game.curPlayerInd && Form1.game.playerList[ i ].culture > playerCult )
				{
					playerCultPos ++;
				}

			sbCulture.Text2 = playerCultPos.ToString() + " / " + Form1.game.playerList.Length.ToString() + " higher culture in 5 turns";*/
			sbCulture.drawSlider();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
	//		e.Graphics.Clear( Form1.defaultBackColor );
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			e.Graphics.Clear( Form1.defaultBackColor );
		}

	}
}
