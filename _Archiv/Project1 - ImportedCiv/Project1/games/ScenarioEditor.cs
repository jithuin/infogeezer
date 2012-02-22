using System;
using System.Drawing;
using System.Windows.Forms;

namespace xycv_ppc.games
{
	public class ScenarioEditor
	{
	//	Game game;
		MenuItem miMain,
			miWorld, // inc scenario
			miCase,
			miUnits,
			miCity,
			miPlayers,
			miRelations,
			miDeals,
			miCivilisations,
			miSaveAs,
			miLoad;/*,
				miAddNew,
				miRemove,
				miModify;*/

		public ScenarioEditor( /*Game game */)
		{
			miMain = new MenuItem();

			miWorld = new MenuItem();
			miWorld.Click += new EventHandler(miWorld_Click);
			miMain.MenuItems.Add( miWorld );

			miCase = new MenuItem();
		//	miCase.Click += new EventHandler(miTerrain_Click);
			miMain.MenuItems.Add( miCase );
		}

		private void miWorld_Click(object sender, EventArgs e)
		{
			FrmWorld fw = new FrmWorld();
			fw.ShowDialog();
		}

		#region FrmWorld
		/// <summary>
		/// turn
		/// name
		/// description
		/// </summary>
		private class FrmWorld : Form
		{
			int spacing = 8;
			NumericUpDown nudTurn;
			Label lblYear, lblName, lblDescription;
			TextBox tbName, tbDescription;

			public FrmWorld()
			{
				this.Text = "";
				this.FormBorderStyle = FormBorderStyle.FixedSingle;
				this.Menu = new MainMenu();

				lblYear = new Label();
				lblYear.Width = (this.ClientSize.Width - 3*spacing) * 3 / 4;
				this.Controls.Add( lblYear );

				nudTurn = new NumericUpDown();
				nudTurn.Minimum = 0;
				nudTurn.ValueChanged += new EventHandler(nudTurn_ValueChanged);
				nudTurn.Value = Form1.game.curTurn;
				nudTurn.Width = (this.ClientSize.Width - 3*spacing) / 4;
				nudTurn.Location = new Point( spacing, spacing );
				this.Controls.Add( nudTurn );

				lblYear.Location = new Point( nudTurn.Right + spacing, spacing );

				lblName = new Label();
				lblName.Width = (this.ClientSize.Width - 3*spacing) * 3 / 4;
				lblName.Location = new Point( spacing, nudTurn.Bottom + spacing );
				this.Controls.Add( lblName );

				tbName = new TextBox();
				tbName.Text = ((Scenario)Form1.game).name;
				tbName.Width = this.ClientSize.Width - 2*spacing;
				tbName.Location = new Point( spacing, lblName.Bottom + spacing );
				this.Controls.Add( tbName );

				lblDescription = new Label();
				lblDescription.Width = (this.ClientSize.Width - 3*spacing) * 3 / 4;
				lblDescription.Location = new Point( spacing, tbName.Bottom + spacing );
				this.Controls.Add( lblDescription );
				
				tbDescription = new TextBox();
				tbDescription.Text = ((Scenario)Form1.game).description;
				tbDescription.Multiline = true;
				tbDescription.WordWrap = true;
				tbDescription.Width = this.ClientSize.Width - 2*spacing;
				tbDescription.Location = new Point( spacing, lblDescription.Bottom + spacing );
				tbDescription.Height = this.ClientSize.Height - spacing - tbDescription.Top;
				this.Controls.Add( tbName );
			}

			protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
			{
				Form1.game.curTurn = (int)nudTurn.Value;
				((Scenario)Form1.game).name = tbName.Text;
				((Scenario)Form1.game).description = tbDescription.Text;

				base.OnClosing (e);
			}

			private void nudTurn_ValueChanged(object sender, EventArgs e)
			{
				lblYear.Text = Game.yearString( (int)nudTurn.Value );
			}
		}
		#endregion

		#region FrmUnits
		/// <summary>
		/// add
		/// remove
		/// cbUnits
		/// 
		/// cbType
		/// cbLevel
		/// cbHealth
		/// cbState
		/// </summary>
		private class FrmUnits : Form
		{
			public FrmUnits( Case c )
			{
			}
		}
		#endregion

		#region FrmCity
		/// <summary>
		/// checkPresent
		/// player
		/// nudPop
		/// list? buildings
		/// ? construction
		/// </summary>
		private class FrmCity : Form
		{
			public FrmCity( Case c )
			{
			}
		}
		#endregion

		#region FrmCase
		/// <summary>
		/// comboTerrain
		/// comboCivic
		/// comboMilitary
		/// comboRoad
		/// comboRiver
		/// </summary>
		private class FrmCase : Form
		{
			public FrmCase( Case c )
			{
			}
		}
		#endregion

		#region FrmPlayers
		/// <summary>
		/// add
		/// remove
		/// comboPlayers
		/// 
		/// tbLeader
		/// comboCiv
		/// list? technos
		/// </summary>
		private class FrmPlayers : Form
		{
			public FrmPlayers( Case c )
			{
			}
		}
		#endregion

		#region FrmRelations
		/// <summary>
		/// comboFirstPlayer
		/// comboSecondPlayer
		/// 
		/// checkMadeContact
		/// comboPolitic
		/// comboEconomic
		/// 
	/*	/// comboDeals
		/// add
		/// remove*/
		/// </summary>
		private class FrmRelations : Form
		{
			public FrmRelations()
			{
			}
		}
		#endregion

		#region FrmDeals
		/// <summary>
		/// comboFirstPlayer
		/// comboSecondPlayer
		/// 
		/// checkMadeContact
		/// comboPolitic
		/// comboEconomic
		/// 
		/*	/// comboDeals
			/// add
			/// remove*/
		/// </summary>
		private class FrmDeals : Form
		{
			public FrmDeals()
			{
			}
		}
		#endregion

		#region FrmCivilizations
		/// <summary>
		/// comboFirstPlayer
		/// comboSecondPlayer
		/// 
		/// checkMadeContact
		/// comboPolitic
		/// comboEconomic
		/// 
		/*	/// comboDeals
			/// add
			/// remove*/
		/// </summary>
		private class FrmCivilizations : Form
		{
			public FrmCivilizations()
			{
			}
		}
		#endregion
	}
}
