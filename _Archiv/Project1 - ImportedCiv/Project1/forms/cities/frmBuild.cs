using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for frmBuild.
	/// </summary>
	public class frmBuild : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button butInfo;
		private System.Windows.Forms.Button cmdAdd;
		private System.Windows.Forms.Button cmdRemove;
		private System.Windows.Forms.ListBox lbPossibilities;
		private System.Windows.Forms.PictureBox pictureBox2;

		Construction construction;
	//	CityList city;
		byte owner;
		int city, cityProd;
		Stat.Construction[] list;
	//	Radius radius = new Radius();
		Graphics g;
	//	Rectangle r;
	//	System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
		Bitmap background;

		SolidBrush whiteBrush = new SolidBrush(Color.White),
			blackBrush = new SolidBrush(Color.Black),
			backBrush = new SolidBrush(Color.Wheat);

		static Font smallF = new Font( "Tahoma", 8, FontStyle.Regular ),
			descF = new Font( "Tahoma", 9, FontStyle.Regular ),
			goldF = new Font( "Tahoma", 10, FontStyle.Regular ),
			titleF = new Font( "Tahoma", 12, FontStyle.Regular );
		private System.Windows.Forms.Button cmdDown, cmdUp;
		private System.Windows.Forms.ListBox lbConstructionList;
				
		static Rectangle r;
		/*= new Rectangle( 
			lbPossibilities.Right - 70 - 10 + 5, 
			0, 
			70 - 5, 
			50 - 10 
			);*/
	
		public frmBuild(byte owner1, int city1, int cityProd1)
	/*	{
			frmBuild( owner1, city1, cityProd1 );
		}
		public frmBuild( CityList city, int cityProd1 )*/
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.pictureBox2.Size = new System.Drawing.Size(240, 296);
			this.ControlBox = false;

			owner = owner1;
		//	this.city= city;
			city = city1;
			cityProd = cityProd1;

			r = new Rectangle( 
				lbPossibilities.Right - 70 - 10, 
				2, 
				70, 
				50 - 8 
				); 

			construction = new Construction();
			construction.list = new Stat.Construction[ Form1.game.playerList[ owner ].cityList[ city ].construction.list.Length ];
			for ( int i = 0; i < construction.list.Length; i ++ )
				construction.list[ i ] = Form1.game.playerList[ owner ].cityList[ city ].construction.list[ i ];

			construction.points = Form1.game.playerList[ owner ].cityList[ city ].construction.points;

			background = new Bitmap( this.Width, this.Height );
			g = Graphics.FromImage( background );

			Color transparentColor = Statistics.terrains[0].bmp.GetPixel(0,0);
		//	ia.SetColorKey(transparentColor, transparentColor);

			int pos = 0;

			list = ai.whatCanBeBuilt( owner, city );

			for ( int i = 0; i < list.Length; i ++ )
				lbPossibilities.Items.Add( list[ i ].name ); 

				/*new structures.constructionList[ Statistics.units.Length + Statistics.buildings.Length + 1 ];

			for ( byte i = 0; i < Statistics.units.Length ; i ++ ) //units
				if ( 
					Form1.game.playerList[ 0 ].technos[ Statistics.units[i ].disponibility ].researched && 
					( 
					Statistics.units[ i ].obselete == 0 || 
					!Form1.game.playerList[ 0 ].technos[ Statistics.units[ Statistics.units[ i ].obselete ].disponibility ].researched 
					) &&
					( 
					Statistics.units[ i ].terrain == 1 || 
					Statistics.units[ i ].terrain == 2 || 
					Statistics.units[ i ].terrain == 3 || 
					Form1.game.radius.isNextToWater( Form1.game.playerList[ owner ].cityList[ city ].X, Form1.game.playerList[ owner ].cityList[ city ].Y ) 
					)
					)
				{
					list[ pos ].type = 1; 
					list[ pos ].ind = i; 
					lbPossibilities.Items.Add( Statistics.units[i ].name ); 

					if ( 
						Form1.game.playerList[ owner1 ].cityList[ city1 ].construction.list[ 0 ].type == (byte)enums.constructionType.unit &&
						Form1.game.playerList[ owner1 ].cityList[ city1 ].construction.list[ 0 ].ind == i
						)
						lbPossibilities.SelectedIndex = pos;

					pos ++;
				}

			if ( !Form1.game.playerList[ 0 ].technos[ 0 ].researched )
			{
				list[ pos ].type = 1;
				list[ pos ].ind = 0;
				lbPossibilities.Items.Add( Statistics.units[ 0 ].name );

				if ( 
					Form1.game.playerList[ owner1 ].cityList[ city1 ].construction.list[ 0 ].type == (byte)enums.constructionType.unit &&
					Form1.game.playerList[ owner1 ].cityList[ city1 ].construction.list[ 0 ].ind == 0
					)
					lbPossibilities.SelectedIndex = pos;

				pos ++;
			}

			for ( byte i = 0; i < Statistics.buildings.Length; i ++ ) //buildings
				if ( Statistics.buildings[i ].disponibility == 0 && Form1.game.playerList[ owner ].cityList[ city ].buildingList[ i ] != true)
				{
					list[ pos ].type = 2;
					list[ pos ].ind = i;
					lbPossibilities.Items.Add( Statistics.buildings[i ].name );

					if ( 
						Form1.game.playerList[ owner1 ].cityList[ city1 ].construction.list[ 0 ].type == (byte)enums.constructionType.building &&
						Form1.game.playerList[ owner1 ].cityList[ city1 ].construction.list[ 0 ].ind == i
						)
						lbPossibilities.SelectedIndex = pos;

					pos ++;
				}
			
			list[ pos ].type = 3;
			list[ pos ].ind = 0;
			lbPossibilities.Items.Add( "Wealth" );*/
		//	byte aaa = (byte)enums.constructionType.wealth;

	//		if ( Form1.game.playerList[ owner1 ].cityList[ city1 ].construction.list[ 0 ].type == (byte)enums.constructionType.wealth ) 
			if ( Form1.game.playerList[ owner1 ].cityList[ city1 ].construction.list[ 0 ] is Stat.Wealth ) 
				lbPossibilities.SelectedIndex = pos; 
			else if ( lbPossibilities.Items.Count > 0 )
				lbPossibilities.SelectedIndex = 0; 

			platformSpec.resolution.set( this.Controls );

			platformSpec.manageWindows.setDialogSize( this );
			populateLBlist();
			enableButtons();
		//	pos ++;
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
			this.lbPossibilities = new System.Windows.Forms.ListBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.butInfo = new System.Windows.Forms.Button();
			this.cmdAdd = new System.Windows.Forms.Button();
			this.cmdRemove = new System.Windows.Forms.Button();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.cmdDown = new System.Windows.Forms.Button();
			this.cmdUp = new System.Windows.Forms.Button();
			this.lbConstructionList = new System.Windows.Forms.ListBox();
			// 
			// lbPossibilities
			// 
			this.lbPossibilities.Location = new System.Drawing.Point(8, 58);
			this.lbPossibilities.Size = new System.Drawing.Size(112, 184);
			this.lbPossibilities.SelectedIndexChanged += new System.EventHandler(this.lbPossibilities_SelectedIndexChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 272);
			this.button1.Size = new System.Drawing.Size(48, 20);
			this.button1.Text = "Ok";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(64, 272);
			this.button2.Size = new System.Drawing.Size(56, 20);
			this.button2.Text = "Cancel";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// butInfo
			// 
			this.butInfo.Location = new System.Drawing.Point(8, 8);
			this.butInfo.Size = new System.Drawing.Size(24, 20);
			this.butInfo.Text = "?";
			this.butInfo.Click += new EventHandler(butInfo_Click);
			// 
			// cmdAdd
			// 
			this.cmdAdd.Location = new System.Drawing.Point(72, 248);
			this.cmdAdd.Size = new System.Drawing.Size(48, 20);
			this.cmdAdd.Text = "Add";
			this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
			// 
			// cmdRemove
			// 
			this.cmdRemove.Location = new System.Drawing.Point(8, 248);
			this.cmdRemove.Size = new System.Drawing.Size(56, 20);
			this.cmdRemove.Text = "Remove";
			this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Size = new System.Drawing.Size(1, 1);
			// 
			// cmdDown
			// 
			this.cmdDown.Location = new System.Drawing.Point(128, 272);
			this.cmdDown.Size = new System.Drawing.Size(48, 20);
			this.cmdDown.Text = "Down";
			this.cmdDown.Click += new EventHandler(cmdDown_Click);
			// 
			// cmdUp
			// 
			this.cmdUp.Location = new System.Drawing.Point(184, 272);
			this.cmdUp.Size = new System.Drawing.Size(48, 20);
			this.cmdUp.Text = "Up";
			this.cmdUp.Click += new EventHandler(cmdUp_Click);
			// 
			// lbConstructionList
			// 
			this.lbConstructionList.Location = new System.Drawing.Point(128, 124);
			this.lbConstructionList.Size = new System.Drawing.Size(104, 142);
			this.lbConstructionList.SelectedIndexChanged += new EventHandler(lbConstructionList_SelectedIndexChanged);
			// 
			// frmBuild
			// 
			this.ClientSize = new System.Drawing.Size(240, 296);
			this.Controls.Add(this.cmdDown);
			this.Controls.Add(this.cmdUp);
			this.Controls.Add(this.lbConstructionList);
			this.Controls.Add(this.cmdRemove);
			this.Controls.Add(this.cmdAdd);
			this.Controls.Add(this.butInfo);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.lbPossibilities);
			this.Controls.Add(this.pictureBox2);
			this.Text = "frmBuild";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmBuild_Closing);

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{ // ok
			if ( construction.empty ) // empty
				construction.addAtEnd( new Stat.Wealth() ); //3, 0 );

			Form1.game.playerList[ owner ].cityList[ city ].construction = construction;
			this.Close();
		}

		private void button2_Click(object sender, System.EventArgs e)
		{ // cancel
			this.Close();
		}

#region lbPossibilities selected
		private void lbPossibilities_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ( list[ (byte)lbPossibilities.SelectedIndex ] is Stat.Unit ) //.type == 1)
			{
				byte terrain;
				if ( 
					((Stat.Unit)list[ (byte)lbPossibilities.SelectedIndex ]).terrain == 1 || 
					((Stat.Unit)list[ (byte)lbPossibilities.SelectedIndex ]).terrain == 3 
					)
					terrain = (byte)enums.terrainType.plain;
				else
					terrain = (byte)enums.terrainType.coast;

				for ( int y = 0; y < 3; y ++ )
				{
					for ( int x = 1; x < 3; x ++ )
						g.DrawImage( 
							Statistics.terrains[ terrain ].bmp,
								new Rectangle(
								x * Form1.caseWidth - Form1.caseWidth / 2,
								y * Form1.caseHeight - 20 - Form1.caseHeight / 2,
								Statistics.terrains[ terrain ].bmp.Width,
								Statistics.terrains[ terrain ].bmp.Height
							),
							0,
							0,
							Statistics.terrains[ terrain ].bmp.Width,
							Statistics.terrains[ terrain ].bmp.Height,
							GraphicsUnit.Pixel,
							Form1.ia
							);

					for ( int x = 1; x < 3; x ++ )
						g.DrawImage( 
							Statistics.terrains[ terrain ].bmp,
								new Rectangle(
								x * Form1.caseWidth - Form1.caseWidth/* / 2*/,
								y * Form1.caseHeight - 20 /*- Form1.caseHeight / 2*/,
								Statistics.terrains[ terrain ].bmp.Width,
								Statistics.terrains[ terrain ].bmp.Height
							),
							0,
							0,
							Statistics.terrains[ terrain ].bmp.Width,
							Statistics.terrains[ terrain ].bmp.Height,
							GraphicsUnit.Pixel,
							Form1.ia
							);
				}

				g.FillRectangle(
					backBrush,
					0,
					0,
					r.Left,
					this.Height
					);
				g.FillRectangle(
					backBrush,
					r.Left,
					0,
					r.Width,
					r.Top
					);
				g.FillRectangle(
					backBrush,
					r.Left,
					r.Bottom,
					r.Right,
					this.Height - r.Bottom
					);
				g.FillRectangle(
					backBrush,
					r.Right,
					0,
					this.Width - r.Right,
					this.Height
					);
			}
			else
			{
				g.Clear( backBrush.Color );
			}
			
			g.DrawString( "What can be built",
				smallF,
				blackBrush,
				lbPossibilities.Left,
				lbPossibilities.Top - 12
				);
			g.DrawString( "Build list",
				smallF,
				blackBrush,
				lbConstructionList.Left,
				lbConstructionList.Top - 12
				);


			if ( list[ (byte)lbPossibilities.SelectedIndex ] is Stat.Unit ) //.type == 1 ) 
			{ 
				drawTitle( list[ (byte)lbPossibilities.SelectedIndex ].name );
				drawCost( count.turnsLeftToBuildS( owner, city, construction, list[ (byte)lbPossibilities.SelectedIndex ] ) ); //1, list[ (byte)lbPossibilities.SelectedIndex ].ind ) );//String.Format( language.getAString( language.order.gold ), Statistics.units[ (byte)list[ (byte)lbPossibilities.SelectedIndex ].ind ].cost ) );
				drawDescription( 
					"a/d/m: " + 
					((Stat.Unit)list[ (byte)lbPossibilities.SelectedIndex ]).attack.ToString() + "/" + 
					((Stat.Unit)list[ (byte)lbPossibilities.SelectedIndex ]).defense.ToString() + "/" + 
					((Stat.Unit)list[ (byte)lbPossibilities.SelectedIndex ]).move.ToString() + "\n" +
					((Stat.Unit)list[ (byte)lbPossibilities.SelectedIndex ]).description );

				g.DrawRectangle(
					new Pen( Color.Black ),
					r
					);
				
				g.DrawImage(
					((Stat.Unit)list[ (byte)lbPossibilities.SelectedIndex ]).bmp,
					new Rectangle(
						lbPossibilities.Right - 70 - 10,
						0,
						70,
						50
					),
					0,
					0,
					70,
					50,
					GraphicsUnit.Pixel,
					Form1.ia
					);
			} 
			else if ( list[ (byte)lbPossibilities.SelectedIndex ] is Stat.Building ) // == 2 ) 
			{ 
				drawTitle( list[ (byte)lbPossibilities.SelectedIndex ].name );
				drawDescription( list[ (byte)lbPossibilities.SelectedIndex ].description );
		//		drawTitle( Statistics.buildings[ (byte)list[ (byte)lbPossibilities.SelectedIndex ].ind ].name );
				drawCost( count.turnsLeftToBuildS( owner, city, construction, list[ (byte)lbPossibilities.SelectedIndex ] ) );//String.Format( language.getAString( language.order.gold ), Statistics.buildings[ (byte)list[ (byte)lbPossibilities.SelectedIndex ].ind ].cost ) );
	//			drawDescription( Statistics.buildings[ (byte)list[ (byte)lbPossibilities.SelectedIndex ].ind ].description );
			} 
			else if ( list[ (byte)lbPossibilities.SelectedIndex ] is Stat.Wealth )//.type == 3 ) 
			{ 
				drawTitle( list[ (byte)lbPossibilities.SelectedIndex ].name );
				drawDescription( list[ (byte)lbPossibilities.SelectedIndex ].description );
			} 
			else if ( list[ (byte)lbPossibilities.SelectedIndex ] is Stat.SmallWonder )//.type == 3 ) 
			{ 
				drawTitle( list[ (byte)lbPossibilities.SelectedIndex ].name );
				drawDescription( list[ (byte)lbPossibilities.SelectedIndex ].description );
			} 
			else if ( list[ (byte)lbPossibilities.SelectedIndex ] is Stat.Wonder )//.type == 3 ) 
			{ 
				drawTitle( list[ (byte)lbPossibilities.SelectedIndex ].name );
				drawDescription( list[ (byte)lbPossibilities.SelectedIndex ].description );
			} 

			pictureBox2.Image = background;

			lbConstructionList.SelectedIndex = -1;

			enableButtons();


			//g = Graphics.FromImage();
			//blackBrush = new SolidBrush(Color.Black);

			//MessageBox.Show( lbPossibilities.SelectedIndex.ToString() );
			//if (lbPossibilities.SelectedIndex == "Temple")
			//	Form1.game.playerList[ owner ].cityList[ city ].BuildingList.temple = false;
			//MessageBox.Show( lbPossibilities.SelectedIndex.ToString() + "\n" + lbPossibilities.SelectedValue.ToString()  + "\n" + lbPossibilities.SelectedItem.ToString());
		}
		#endregion

		private void cmdAdd_Click(object sender, System.EventArgs e)
		{
			construction.addAtEnd( list[ lbPossibilities.SelectedIndex ] ); //(byte)list[ lbPossibilities.SelectedIndex ].type, (int)list[ lbPossibilities.SelectedIndex ].ind );
			populateLBlist();
			enableButtons();
		}

		private void cmdRemove_Click(object sender, System.EventArgs e)
		{
			int si = lbConstructionList.SelectedIndex;
			construction.remove( lbConstructionList.SelectedIndex );
			
			populateLBlist();

			if ( lbConstructionList.Items.Count > si )
				lbConstructionList.SelectedIndex = si;

			enableButtons();
		}

		private void cmdDown_Click(object sender, System.EventArgs e)
		{
			int pos = lbConstructionList.SelectedIndex;
			if ( construction.moveDown( lbConstructionList.SelectedIndex ) )
			{
				populateLBlist();
				lbConstructionList.SelectedIndex = pos + 1;
			}
		}

		private void cmdUp_Click(object sender, System.EventArgs e)
		{
			int pos = lbConstructionList.SelectedIndex;
			if ( construction.moveUp( lbConstructionList.SelectedIndex ) )
			{
				populateLBlist();
				lbConstructionList.SelectedIndex = pos - 1;
			}
		}

		private void populateLBlist()
		{
			lbConstructionList.Items.Clear();
/*
			if ( construction.list[ 0 ].ind != -1 )
				if ( construction.list[ 0 ].type == 1 )
					lbConstructionList.Items.Add( Statistics.units[ construction.list[ 0 ].ind ].name + "" + count.turnsLeftToBuildS( owner, city ) );
				else if ( construction.list[ 0 ].type == 2 )
					lbConstructionList.Items.Add( Statistics.buildings[ construction.list[ 0 ].ind ].name + "" + count.turnsLeftToBuildS( owner, city ) );
				else if ( construction.list[ 0 ].type == 3 )
					lbConstructionList.Items.Add( "Wealth" );*/
/*
			if ( construction.list[ 0 ] != null )
				if ( construction.list[ i ] is Stat.Wealth )
					lbConstructionList.Items.Add( construction.list[ i ].name );
				else	
					lbConstructionList.Items.Add( construction.list[ 0 ].name + "" + count.turnsLeftToBuildS( owner, city ) );
				*/

			for ( int i = 0; i < construction.list.Length; i ++ )
				if ( construction.list[ i ] != null )
				{
					if ( construction.list[ i ] is Stat.Wealth )
						lbConstructionList.Items.Add( construction.list[ i ].name );
					else
						lbConstructionList.Items.Add( construction.list[ i ].name + " (" + count.turnsLeftToBuild( owner, city, i, construction ).ToString() + ")" );
				/*	if ( construction.list[ i ].type == 1 )
						lbConstructionList.Items.Add( Statistics.units[ construction.list[ i ].ind ].name + " (" + count.turnsLeftToBuild( owner, city, i, construction ).ToString() + ")" );
					else if ( construction.list[ i ].type == 2 )
						lbConstructionList.Items.Add( Statistics.buildings[ construction.list[ i ].ind ].name + " (" + count.turnsLeftToBuild( owner, city, i, construction ).ToString() + ")" );
					else if ( construction.list[ i ].type == 3 )
						lbConstructionList.Items.Add( "Wealth" );*/
				}
				else
					break;
		}

		private void lbConstructionList_SelectedIndexChanged(object sender, EventArgs e)
		{
			enableButtons();
		}

		private void enableButtons()
		{
			if ( lbConstructionList.SelectedIndex != -1 )
			{
				cmdDown.Enabled = construction.canMoveDown( lbConstructionList.SelectedIndex );
				cmdUp.Enabled = construction.canMoveUp( lbConstructionList.SelectedIndex );
				cmdRemove.Enabled = true;
			//	cmdAdd.Enabled = construction.canAddToTheList;

				int si = lbConstructionList.SelectedIndex;
				for ( int i = 0; i < lbPossibilities.Items.Count; i ++ )
					if ( 
						list[ i ].type == construction.list[ lbConstructionList.SelectedIndex ].type &&
						list[ i ].getConstructionType() == construction.list[ lbConstructionList.SelectedIndex ].getConstructionType()
				/*		list[ i ].getConstructionType() == construction.list[ lbConstructionList.SelectedIndex ].getConstructionType() && 
						list[ i ].type == construction.list[ lbConstructionList.SelectedIndex ].type*/
						)
					{
						lbPossibilities.SelectedIndex = i;
						break;
					}

				lbConstructionList.SelectedIndex = si;

				if ( list[ lbPossibilities.SelectedIndex ].type == 2 )
					for ( int i = 0; i < construction.list.Length; i ++ )
						if ( 
							list[ i ].type == construction.list[ lbConstructionList.SelectedIndex ].type &&
							list[ i ].getConstructionType() == construction.list[ lbConstructionList.SelectedIndex ].getConstructionType()
						//	list[ lbPossibilities.SelectedIndex ] == construction.list[ i ]
						/*	list[ lbPossibilities.SelectedIndex ].getConstructionType() == construction.list[ i ].getConstructionType() && 
							list[ lbPossibilities.SelectedIndex ].type == construction.list[ i ].type*/
							)
							cmdAdd.Enabled = false;
			}
			else
			{
				cmdDown.Enabled = false;
				cmdUp.Enabled = false;
				cmdRemove.Enabled = false;
			//	cmdAdd.Enabled = false;
			}
			
			if ( lbPossibilities.SelectedIndex != -1 )
			{
				cmdAdd.Enabled = construction.canAddToTheList;
				if ( list[ lbPossibilities.SelectedIndex ].type == 2 )
					for ( int i = 0; i < construction.list.Length; i ++ )
						if ( 
							list[ lbPossibilities.SelectedIndex ] == construction.list[ i ]
						/*	list[ lbPossibilities.SelectedIndex ].type == construction.list[ i ].type &&
							list[ lbPossibilities.SelectedIndex ].ind == construction.list[ i ].ind */
							)
							cmdAdd.Enabled = false;

				if ( list[ lbPossibilities.SelectedIndex ].type == 1 )
					butInfo.Enabled = true;
				else
					butInfo.Enabled = false;
			}
			else
			{
				cmdAdd.Enabled = false;
				butInfo.Enabled = false;
			}
		}

		private void frmBuild_Closing(object sender, CancelEventArgs e)
		{

		}

		private void drawDescription( string text )
		{
			g.DrawString( 
				text,
				descF,
				blackBrush,
				new Rectangle(
				lbConstructionList.Left,
				50,
				lbConstructionList.Width,
				lbConstructionList.Top - 50 
				)
				);
		}

		private void drawTitle( string text )
		{
			g.DrawString( 
				text,
				titleF,
				blackBrush,
				lbConstructionList.Left,
				4
				);
		}

		private void drawCost( string text )
		{
			g.DrawString( 
				text,
				goldF,
				blackBrush,
				lbConstructionList.Left,
				24
				);
		}

		private void butInfo_Click(object sender, EventArgs e)
		{
			enums.infoType type = enums.infoType.units;
			//if ( list[ lbPossibilities.SelectedIndex ].type == 1 )
		//		type = enums.infoType.units;
			/*else if ( list[ lbPossibilities.SelectedIndex ].type == 2 )
				type = (byte)enums.infoType.;
					 else*/

			if ( list[ lbPossibilities.SelectedIndex ] is Stat.Unit )
				type = enums.infoType.units;
			else if ( list[ lbPossibilities.SelectedIndex ] is Stat.Building )
				type = enums.infoType.buildings;
			else if ( list[ lbPossibilities.SelectedIndex ] is Stat.SmallWonder )
				type = enums.infoType.buildings;
			else if ( list[ lbPossibilities.SelectedIndex ] is Stat.Wonder )
				type = enums.infoType.buildings;

			frmInfo fi;
			if ( list[ lbPossibilities.SelectedIndex ] is Stat.Wealth )
			{
				fi = new frmInfo( 
					enums.infoType.misc,
					0 // wealth type
					);
			}
			else
				fi = new frmInfo( 
					type,
					list[ lbPossibilities.SelectedIndex ].type
					);

			fi.ShowDialog();
		}
	}
}
