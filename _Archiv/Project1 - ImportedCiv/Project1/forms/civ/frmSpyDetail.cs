using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for frmSpyDetail.
	/// </summary>
	public class frmSpyDetail : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label[] label;
		Button[] cmdAdd;
		SpyBar[] spyBars;
		ComboBox[] comboBoxes;
		byte player;
		int left, width;
		byte selected;
	
		public frmSpyDetail( byte player1 )
		{
			label = new Label[ 4 ];
			player = player1;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Text = "Spying details";

			
			this.label[ 0 ] = new System.Windows.Forms.Label();
			this.label[ 1 ] = new System.Windows.Forms.Label();
			this.label[ 2 ] = new System.Windows.Forms.Label();
			this.label[ 3 ] = new System.Windows.Forms.Label();
			// 
			// label[ 0 ]
			// 
			this.label[ (byte)enums.spyType.gov ].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
			this.label[ (byte)enums.spyType.gov ].Location = new System.Drawing.Point(8, 24 - 16);
			this.label[ (byte)enums.spyType.gov ].Size = new System.Drawing.Size(64, 20);
			this.label[ (byte)enums.spyType.gov ].Text = "Politic";
			// 
			// label[ 1 ]
			// 
			this.label[ (byte)enums.spyType.military ].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
			this.label[ (byte)enums.spyType.military ].Location = new System.Drawing.Point(8, 136 - 16);
			this.label[ (byte)enums.spyType.military ].Size = new System.Drawing.Size(72, 20);
			this.label[ (byte)enums.spyType.military ].Text = "Military";
			// 
			// label[ 2 ]
			// 
			this.label[ (byte)enums.spyType.science ].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
			this.label[ (byte)enums.spyType.science ].Location = new System.Drawing.Point(8, 80 - 16);
			this.label[ (byte)enums.spyType.science ].Size = new System.Drawing.Size(80, 20);
			this.label[ (byte)enums.spyType.science ].Text = "Science";
			// 
			// label[ 3 ]
			// 
			this.label[ (byte)enums.spyType.people ].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular);
			this.label[ (byte)enums.spyType.people ].Location = new System.Drawing.Point(8, 192 - 16);
			this.label[ (byte)enums.spyType.people ].Size = new System.Drawing.Size(72, 20);
			this.label[ (byte)enums.spyType.people ].Text = "People";
			// 
			// frmSpyDetail
			// 
			this.Controls.Add(this.label[ 3 ]);
			this.Controls.Add(this.label[ 2 ]);
			this.Controls.Add(this.label[ 1 ]);
			this.Controls.Add(this.label[ 0 ]);

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			left = 8;
			width = this.Width - 16;
			
			platformSpec.resolution.set( this.Controls );

			dispSpyBars();
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
		//	this.Text = "Spying on " + Form1.game.playerList[ player ].playerName;

		}
		#endregion

		private void dispSpyBars()
		{
			spyBars = new SpyBar[ 4 ];
			cmdAdd = new Button[ 4 ];
			comboBoxes = new ComboBox[ 4 ];
			for ( int i = 0; i < spyBars.Length; i ++ )
			{
				spyBars[ i ] = new SpyBar( left, label[ i ].Bottom + 4, width );
				spyBars[ i ].picBox.Parent = this;
				spyBars[ i ].picBox.BringToFront();
				spyBars[ i ].spyNbr = Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ player ].spies[ i ].nbr;
				spyBars[ i ].enabled = true;
				spyBars[ i ].drawSpies();
				spyBars[ i ].drawAff();

				comboBoxes[ i ] = new ComboBox();
				comboBoxes[ i ].Top = label[ i ].Bottom - comboBoxes[ i ].Height + 3;
				comboBoxes[ i ].Left = this.Width - 8 - comboBoxes[ i ].Width;
				comboBoxes[ i ].Parent = this;
				comboBoxes[ i ].BringToFront();
				
				comboBoxes[ i ].Items.Add( "Passive" );
				comboBoxes[ i ].Items.Add( "Agressive" );
				comboBoxes[ i ].SelectedIndex = Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ player ].spies[ i ].state;
				comboBoxes[ i ].SelectedIndexChanged += new EventHandler(frmSpyDetail_SelectedIndexChanged);

				cmdAdd[ i ] = new Button();
				cmdAdd[ i ].Text = "+";
				cmdAdd[ i ].Width = cmdAdd[ i ].Height;
				cmdAdd[ i ].Enabled = false;
				cmdAdd[ i ].Top = label[ i ].Bottom - cmdAdd[ i ].Height + 3;
				cmdAdd[ i ].Left = comboBoxes[ i ].Left - 8 - cmdAdd[ i ].Width;
				cmdAdd[ i ].Parent = this;
				cmdAdd[ i ].BringToFront();
#if !CF
				cmdAdd[ i ].FlatStyle = FlatStyle.System;
#endif
			}
			
			cmdAdd[ (byte)enums.spyType.gov ].Click += new EventHandler(addGov_Click);
			cmdAdd[ (byte)enums.spyType.military ].Click += new EventHandler(addMilitary_Click);
			cmdAdd[ (byte)enums.spyType.people ].Click += new EventHandler(addPeople_Click);
			cmdAdd[ (byte)enums.spyType.science ].Click += new EventHandler(addScience_Click);
			
			spyBars[ (byte)enums.spyType.gov ].picBox.MouseUp += new MouseEventHandler(addGov_MouseUp);
			spyBars[ (byte)enums.spyType.military ].picBox.MouseUp += new MouseEventHandler(addMilitary_MouseUp);
			spyBars[ (byte)enums.spyType.people ].picBox.MouseUp += new MouseEventHandler(addPeople_MouseUp);
			spyBars[ (byte)enums.spyType.science ].picBox.MouseUp += new MouseEventHandler(addScience_MouseUp);

			spyBars[ (byte)enums.spyType.gov ].picBox.MouseDown += new MouseEventHandler(addGov_MouseUp);
			spyBars[ (byte)enums.spyType.military ].picBox.MouseDown += new MouseEventHandler(addMilitary_MouseUp);
			spyBars[ (byte)enums.spyType.people ].picBox.MouseDown += new MouseEventHandler(addPeople_MouseUp);
			spyBars[ (byte)enums.spyType.science ].picBox.MouseDown += new MouseEventHandler(addScience_MouseUp);
			
			enableWHTB();
		}

		private void addGov_Click(object sender, EventArgs e)
		{
			addBut( (byte)enums.spyType.gov );
			enableWHTB();
		}
		private void addMilitary_Click(object sender, EventArgs e)
		{
			addBut( (byte)enums.spyType.military );
			enableWHTB();
		}
		private void addPeople_Click(object sender, EventArgs e)
		{
			addBut( (byte)enums.spyType.people );
			enableWHTB();
		}
		private void addScience_Click(object sender, EventArgs e)
		{
			addBut( (byte)enums.spyType.science );
			enableWHTB();
		}

		private void addGov_MouseUp(object sender, MouseEventArgs e)
		{
			selected = (byte)enums.spyType.gov;
			enableWHTB();
		}
		private void addMilitary_MouseUp(object sender, MouseEventArgs e)
		{
			selected = (byte)enums.spyType.military;
			enableWHTB();
		}
		private void addPeople_MouseUp(object sender, MouseEventArgs e)
		{
			selected = (byte)enums.spyType.people;
			enableWHTB();
		}
		private void addScience_MouseUp(object sender, MouseEventArgs e)
		{
			selected = (byte)enums.spyType.science;
			enableWHTB();
		}

		private void enableWHTB()
		{
			if ( spyBars[ selected ].selected > 0 )
			{
				for ( int i = 0; i < 4; i ++ )
				{
					if ( selected == i )
					{
						cmdAdd[ i ].Text = "-";
						cmdAdd[ i ].Enabled = true;
					}
					else
					{
						cmdAdd[ i ].Text = "+";
						cmdAdd[ i ].Enabled = true;

						spyBars[ i ].selected = 0;
						spyBars[ i ].drawAff();
					}

				}
			}
			else
			{
				for ( int i = 0; i < 4; i ++ )
				{
					cmdAdd[ i ].Text = "+";
					cmdAdd[ i ].Enabled = false;
				}
			}
		}

		private void transfertSpy( byte from, byte to, int nbr )
		{
			Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ player ].spies[ from ].nbr -= nbr;
			Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ player ].spies[ to ].nbr += nbr;

			spyBars[ from ].spyNbr = Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ player ].spies[ from ].nbr;
			spyBars[ from ].selected = 0;
			spyBars[ from ].drawSpies();
			spyBars[ from ].drawAff();
			
			spyBars[ to ].spyNbr = Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ player ].spies[ to ].nbr;
			spyBars[ to ].drawSpies();
			spyBars[ to ].drawAff();
		}

		private void removeSpy( byte from, int nbr )
		{
			Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ player ].spies[ from ].nbr -= nbr;
			Form1.game.playerList[ Form1.game.curPlayerInd ].counterIntNbr += nbr;

			spyBars[ from ].spyNbr = Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ player ].spies[ from ].nbr;
			spyBars[ from ].selected = 0;
			spyBars[ from ].drawSpies();
			spyBars[ from ].drawAff();
		}

		private void addBut( byte but ) 
		{ 
			if ( but == selected ) 
			{ // - 
				removeSpy( selected, spyBars[ selected ].selected ); 
			} 
			else 
			{ // + 
				transfertSpy( selected, but, spyBars[ selected ].selected ); 
			} 
		} 

		private void frmSpyDetail_SelectedIndexChanged(object sender, EventArgs e) 
		{ 
			for ( int i = 0; i < 4; i ++ ) 
			{
				Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ player ].spies[ i ].state = (byte)comboBoxes[ i ].SelectedIndex;
			}
		}
	}
}
