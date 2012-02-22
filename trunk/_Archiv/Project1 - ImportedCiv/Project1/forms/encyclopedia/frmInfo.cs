using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for frmInfo.
	/// </summary>
	public class frmInfo : System.Windows.Forms.Form
	{
		enums.infoType type;
		int ind;
		private Font titleFont = new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 12 ), FontStyle.Bold ),
			otherFont = new Font( "Tahoma", platformSpec.resolution.getStaticSizeForFont( 9 ), FontStyle.Regular );
		private Brush blackBrush = new SolidBrush( Color.Black );
		private int spacing = 4 * platformSpec.resolution.mod;
		private Pen blackPen = new Pen( Color.Black );

		public frmInfo( enums.infoType type1, int ind1 )
		{
			type = type1;
			ind = ind1;
			
			platformSpec.resolution.set( this.Controls );

			
			switch ( type )
			{

				case enums.infoType.units:
					this.Text = Statistics.units[ ind ].name;
					break;

				case enums.infoType.terrain: 
					this.Text = Statistics.terrains[ ind ].name; 
					break;

				case enums.infoType.techno: 
					this.Text = Statistics.technologies[ ind ].name; 
					break;

				case enums.infoType.gov: 
					this.Text = Statistics.governements[ ind ].name; 
					break;

				case enums.infoType.eco :
					this.Text = Statistics.economies[ ind ].name;
					break;

				case enums.infoType.resources: 
					this.Text = Statistics.resources[ ind ].name;
					break;

				case enums.infoType.tutorial: 
					this.Text = Tutorial.getTextFromInd( ind ).title; 
					break;

				case enums.infoType.buildings: 
					this.Text = Statistics.buildings[ ind ].name; 
					break;

				case enums.infoType.smallWonders: 
					this.Text = Statistics.smallWonders[ ind ].name; 
					break;

				case enums.infoType.wonders: 
					this.Text = Statistics.wonders[ ind ].name; 
					break;

			}
		}


		protected override void OnPaint(PaintEventArgs e)
		{
		//	base.OnPaint(e);
			e.Graphics.Clear( Form1.defaultBackColor );
			int vPos = spacing,
				titleVSpacing = (int)e.Graphics.MeasureString( "b", titleFont ).Height,
				otherVSpacing = (int)e.Graphics.MeasureString( "b", otherFont ).Height;
			Bitmap affBmp;
			Graphics g;
			byte terrain;

			switch ( type )
			{
				#region units X
				case enums.infoType.units:

					affBmp = new Bitmap( this.ClientSize.Width * 2 / 3, 60 * platformSpec.resolution.mod );
					g = Graphics.FromImage( affBmp );

					g.Clear( Color.Black );

				//	byte terrain;
					if ( 
						Statistics.units[ ind ].terrain == 1 || 
						Statistics.units[ ind ].terrain == 3 
						)
						terrain = (byte)enums.terrainType.plain;
					else
						terrain = (byte)enums.terrainType.coast;

					for ( int y = -1; y < affBmp.Width / 50 + 2; y ++ )
					{
						for ( int x = 0; x < affBmp.Height / 15 + 2; x ++ )
							g.DrawImage( 
								Statistics.terrains[ terrain ].bmp,
								new Rectangle(
									x * Form1.caseWidth - Form1.caseWidth / 2 - 10,
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

						for ( int x = 0; x < 6; x ++ )
							g.DrawImage( 
								Statistics.terrains[ terrain ].bmp,
								new Rectangle(
									x * Form1.caseWidth - Form1.caseWidth - 10,
									y * Form1.caseHeight - 20,
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

					g.DrawImage( Statistics.units[ ind ].bmp,
						new Rectangle(
						affBmp.Width / 2 - 35,
						affBmp.Height / 2 - 25,
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

					g.DrawRectangle(
						blackPen,
						0,
						0,
						affBmp.Width - 1,
						affBmp.Height - 1
						);

					// name
					e.Graphics.DrawString( Statistics.units[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// image
					e.Graphics.DrawImage( affBmp, this.ClientSize.Width /6, vPos );
					vPos += spacing + affBmp.Height;

					// need techno
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaNeededTechno ), Statistics.technologies[ Statistics.units[ ind ].disponibility ].name ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;
						
					// cost
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaUnitCost ), Statistics.units[ ind ].cost ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// attack
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaUnitAttack ), Statistics.units[ ind ].attack ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// defense
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaUnitDefense ), Statistics.units[ ind ].defense ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// moves
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaUnitMove ), Statistics.units[ ind ].move ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// entrenchable
					if ( Statistics.units[ ind ].entrenchable )
					{
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaUnitEntrenchable ), otherFont, blackBrush, spacing, vPos );
						vPos += spacing + otherVSpacing;
					}

					// high sea sync
					if ( Statistics.units[ ind ].terrain == 0 )
					{
						if ( Statistics.units[ ind ].highSeaSync )
							e.Graphics.DrawString( language.getAString( language.order.encyclopediaUnitHighSeaSync ), otherFont, blackBrush, spacing, vPos );
						else
							e.Graphics.DrawString( language.getAString( language.order.encyclopediaUnitHighSeaSyncResistant ), otherFont, blackBrush, spacing, vPos );
						
						vPos += spacing + otherVSpacing;
					}

					// transport / capcity
					if ( Statistics.units[ ind ].transport > 0 )
					{
						e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaUnitTransportCapacity ), Statistics.units[ ind ].transport ), otherFont, blackBrush, spacing, vPos );
						vPos += spacing + otherVSpacing;
					}

					// speciality 
					if ( Statistics.units[ ind ].speciality != enums.speciality.none )
					{
					/*	e.Graphics.DrawString( language.getAString( language.order.encyclopediaUnitStealth ), otherFont, blackBrush, spacing, vPos );
						vPos += spacing + otherVSpacing;*/
					}

					// stealth
					if ( Statistics.units[ ind ].stealth )
					{
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaUnitStealth ), otherFont, blackBrush, spacing, vPos );
						vPos += spacing + otherVSpacing;
					}

					// sight
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaUnitSight ), Statistics.units[ ind ].sight ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// range
					if ( 
						Statistics.units[ ind ].speciality == enums.speciality.bombard ||
						Statistics.units[ ind ].speciality == enums.speciality.fighter ||
						Statistics.units[ ind ].speciality == enums.speciality.bomber ||
						Statistics.units[ ind ].speciality == enums.speciality.nukeBomb ||
						Statistics.units[ ind ].speciality == enums.speciality.nukeIC ||
						Statistics.units[ ind ].speciality == enums.speciality.nukeMissile ||
						Statistics.units[ ind ].speciality == enums.speciality.cruiseMissile
						)
					{
						e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaUnitRange ), Statistics.units[ ind ].range ), otherFont, blackBrush, spacing, vPos );
						vPos += spacing + otherVSpacing;
					}

					// obselete
					if ( Statistics.units[ ind ].obselete != 0 )
					{
						e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaUnitRanderedObseleteBy ), Statistics.units[ Statistics.units[ ind ].obselete ].name ), otherFont, blackBrush, spacing, vPos );
						vPos += spacing + otherVSpacing;
					}

					// 
		/*			e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTradeAdv ), Statistics.technologies[ ind ].trade ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// desc
					e.Graphics.DrawString( Statistics.technologies[ ind ].desc, otherFont, blackBrush, spacing, vPos );
*/
/*					label1.Text = Statistics.units[ ind ].name;
					label2.Text = "a/d/m: " + 
						Statistics.units[ ind ].attack.ToString() + "/" + 
						Statistics.units[ ind ].defense.ToString() + "/" + 
						Statistics.units[ ind ].move.ToString();
					label3.Text = Statistics.units[ ind ].cost.ToString() + " gold pieces";
					label4.Text = Statistics.units[ ind ].description ;
*/
					break;
					#endregion

				#region terrain X
				case enums.infoType.terrain: 

					// prepare affBmp
					affBmp = new Bitmap( this.ClientSize.Width * 2 / 3, 60 * platformSpec.resolution.mod );
					g = Graphics.FromImage( affBmp );

					g.Clear( Color.White );

					g.DrawImage( 
						Statistics.terrains[ ind ].bmp,
						new Rectangle(
						affBmp.Width / 2 - 35,
						affBmp.Height / 2 - 25,
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
					

					g.DrawRectangle(
						blackPen,
						0,
						0,
						affBmp.Width - 1,
						affBmp.Height - 1
						);

					// name
					e.Graphics.DrawString( Statistics.terrains[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// image
					e.Graphics.DrawImage( affBmp, this.ClientSize.Width /6, vPos );
					vPos += spacing + affBmp.Height;

					// move diff
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTerrainMoveDifficulty ), Statistics.terrains[ ind ].move ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// def bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTerrainDefenseBonus ), Statistics.terrains[ ind ].defense ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// food
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTerrainFood ), Statistics.terrains[ ind ].food ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// prod
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTerrainProduction ), Statistics.terrains[ ind ].production ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// trade
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTerrainTrade ), Statistics.terrains[ ind ].trade ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// irrigation bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTerrainIrrigationFoodBonus ), Statistics.terrains[ ind ].irrigationBonus ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// mine bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTerrainMineProductionBonus ), Statistics.terrains[ ind ].mineBonus ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// road bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTerrainRoadTradeBonus ), Statistics.terrains[ ind ].roadBonus ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// railroad bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTerrainRailroadTradeBonus ), Statistics.terrains[ ind ].roadBonus * 2 ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// water ?
				/*	if ( Statistics.terrains[ ind ].ew == 0 )
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaEcoCommunism ), otherFont, blackBrush, spacing, vPos );
					else
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaEcoCapitalism ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;*/

					// desc
					e.Graphics.DrawString( Statistics.terrains[ ind ].description, otherFont, blackBrush, spacing, vPos );

					break;
					#endregion

				#region techno X
				case enums.infoType.techno: 

					// name
					e.Graphics.DrawString( Statistics.technologies[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;
						
					// can be researched
					if ( Statistics.technologies[ ind ].canBeResearched )
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaTechnoCanBeResearched ), otherFont, blackBrush, spacing, vPos );
					else
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaTechnoCanNotBeResearched ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// needed technos
					if ( Statistics.technologies[ ind ].canBeResearched && ind != 0 )
					{
						string neededTechnos = "";
						if ( Statistics.technologies[ ind ].needs[ 0 ] == 0 )
						{
							neededTechnos += Statistics.technologies[ 0 ].name; 
						}
						else
						{
							neededTechnos += Statistics.technologies[ Statistics.technologies[ ind ].needs[ 0 ] ].name; 
							for ( int i = 1; i < Statistics.technologies[ ind ].needs.Length; i ++ ) 
								if ( Statistics.technologies[ ind ].needs[ i ] > 0 ) 
									neededTechnos += ", " + Statistics.technologies[ Statistics.technologies[ ind ].needs[ i ] ].name; 
								else break;
						}

						e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTechnoNeededTechnos ), neededTechnos ), otherFont, blackBrush, spacing, vPos );
						vPos += spacing + otherVSpacing;
					}

					// allow technos
					string allowedTechnos = "";
					if ( ind != 0 )
					{
						int k = 0;
						for ( int i = 0; i < Statistics.technologies.Length; i ++ )
							for ( int j = 0; j < Statistics.technologies[ i ].needs.Length; j ++ )
								if ( Statistics.technologies[ i ].needs[ j ] == ind )
								{
									if ( k != 0 )
										allowedTechnos += ", ";

									allowedTechnos += Statistics.technologies[ i ].name; 
									k ++;
									break;
								}

					}
					else
					{
					/*	int k = 0;
						for ( int i = 0; i < Statistics.technologies.Length; i ++ )
							if ( Statistics.technologies[ i ].needs[ 0 ] == 0 )
							{
								if ( k != 0 )
									lblTxt4 += ", ";

								allowedTechnos += Statistics.technologies[ i ].name; 
								k ++;
								break;
							}*/
					}

					if ( allowedTechnos.Length > 0 )
						e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTechnoAllowTechnos ), allowedTechnos ), otherFont, blackBrush, spacing, vPos );
					else
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaTechnoDoesNotAllowAnyTechnos ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// cost ?
					//	e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaFoodAdv ), Statistics.economies[ ind ].food ), otherFont, blackBrush, spacing, vPos );
					//	vPos += spacing + otherVSpacing;

					// desc
					e.Graphics.DrawString( Statistics.technologies[ ind ].description, otherFont, blackBrush, spacing, vPos );

					break;
					#endregion

				#region gov X
				case enums.infoType.gov: 
					
					// name
					e.Graphics.DrawString( Statistics.governements[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// need techno
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaNeededTechno ), Statistics.technologies[ Statistics.governements[ ind ].neededTechno ].name ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;
						
					// oppresive
					if ( Statistics.governements[ ind ].oppresive )
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaGovOppresive ), otherFont, blackBrush, spacing, vPos );
					else
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaGovNonOppresive ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// surrender on conquer
					if ( Statistics.governements[ ind ].surrenderOnConquer )
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaGovSurrenderOnConquer ), otherFont, blackBrush, spacing, vPos );
					else
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaGovResistInvaders ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// can be choosed by player
					if ( Statistics.governements[ ind ].canBeChoosedByPlayer )
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaGovCanBeChosenByPlayers ), otherFont, blackBrush, spacing, vPos );
					else
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaGovUnavailableToPlayers ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// food
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaFoodAdv ), Statistics.governements[ ind ].foodPerc ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// prod
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaProdAdv ), Statistics.governements[ ind ].productionPerc ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// trade
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTradeAdv ), Statistics.governements[ ind ].tradePerc ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// pop

					// support

					// desc
					e.Graphics.DrawString( Statistics.governements[ ind ].description, otherFont, blackBrush, spacing, vPos );




		/*			label4.Top = label1.Bottom + 4; 
					label4.Left = 16; 
					label4.Width = this.Width - 32; 
					label4.Height = this.Height - label4.Bottom - 8; 
					label4.Font = new Font( "Tahoma", 9, FontStyle.Regular ); 
					label4.Parent = this; 

					label1.Text = Statistics.governements[ ind ].name; 
					lblTxt4 = ""; 
					//	string lblTxt2

					if ( Statistics.governements[ ind ].oppresive )
						lblTxt4 += "Oppresive";
					else
						lblTxt4 += "Non-oppresive";

					lblTxt4 += "\n";

					if ( Statistics.governements[ ind ].surrenderOnConquer )
						lblTxt4 += "Surrender on conquer";
					else
						lblTxt4 += "Resist invaders";

					lblTxt4 += "\n\n";

					lblTxt4 += "require " + Statistics.technologies[ Statistics.governements[ ind ].neededTechno ].name; 

					lblTxt4 += "\n\n" + Statistics.governements[ ind ].description;

					label4.Text = lblTxt4;			*/

					break;
					#endregion

				#region eco X
				case enums.infoType.eco :

					// name
					e.Graphics.DrawString( Statistics.economies[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// need techno
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaNeededTechno ), Statistics.technologies[ Statistics.economies[ ind ].techno ].name ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;
						
					// communism
					if ( Statistics.economies[ ind ].communism )
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaEcoCommunism ), otherFont, blackBrush, spacing, vPos );
					else
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaEcoCapitalism ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;
						
					// slavery
					if ( Statistics.economies[ ind ].supportSlavery )
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaEcoSupportSlavery ), otherFont, blackBrush, spacing, vPos );
					else
						e.Graphics.DrawString( language.getAString( language.order.encyclopediaEcoDoesNotSupportSlavery ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// food
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaFoodAdv ), Statistics.economies[ ind ].food ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// prod
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaProdAdv ), Statistics.economies[ ind ].prod ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// trade
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaTradeAdv ), Statistics.economies[ ind ].trade ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// desc
					e.Graphics.DrawString( Statistics.economies[ ind ].description, otherFont, blackBrush, spacing, vPos );

					break;
					#endregion
					
				#region resources X
				case enums.infoType.resources: 
					
					affBmp = new Bitmap( this.ClientSize.Width * 2 / 3, 60 * platformSpec.resolution.mod );
					g = Graphics.FromImage( affBmp );
					g.Clear( Color.Black );

					Random r = new Random();

					if ( Statistics.resources[ ind ].terrain == 1 )
						terrain = (byte)enums.terrainType.plain;
					else
						terrain = (byte)enums.terrainType.coast;

					for ( int y = -1; y < affBmp.Width / 50 + 2; y ++ )
					{
						for ( int x = 0; x < affBmp.Height / 15 + 2; x ++ )
						{
							g.DrawImage( 
								Statistics.terrains[ terrain ].bmp, // Statistics.terrains[ Statistics.resources[ ind ].terrainTypes[ r.Next( Statistics.resources[ ind ].terrainTypes.Length ) ] ].bmp, // To be implemented later
								new Rectangle(
								x * Form1.caseWidth - Form1.caseWidth / 2 - 10,
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
						}
						for ( int x = 0; x < 6; x ++ )
						{
							g.DrawImage( 
								Statistics.terrains[ terrain ].bmp, // Statistics.terrains[ Statistics.resources[ ind ].terrainTypes[ r.Next( Statistics.resources[ ind ].terrainTypes.Length ) ] ].bmp, 
								new Rectangle(
								x * Form1.caseWidth - Form1.caseWidth - 10,
								y * Form1.caseHeight - 20,
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
					}

					g.DrawImage( Statistics.resources[ ind ].bmp,
						new Rectangle(
						affBmp.Width / 2 - 35,
						affBmp.Height / 2 - 25,
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

					g.DrawRectangle(
						blackPen,
						0,
						0,
						affBmp.Width - 1,
						affBmp.Height - 1
						);

					// name
					e.Graphics.DrawString( Statistics.resources[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// image
					e.Graphics.DrawImage( affBmp, this.ClientSize.Width /6, vPos );
					vPos += spacing + affBmp.Height;

					// need techno
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaResourceNeededTechnos ), Statistics.technologies[ Statistics.resources[ ind ].techno ].name ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// food bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaResourceFoodBonus ), Statistics.resources[ ind ].foodBonus ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// prod bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaResourceProductionBonus ), Statistics.resources[ ind ].prodBonus ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// trade bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaResourceTradeBonus ), Statistics.resources[ ind ].tradeBonus ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// desc
					e.Graphics.DrawString( Statistics.resources[ ind ].description, otherFont, blackBrush, spacing, vPos );

					break;
					#endregion
					
				#region specialResources
				case enums.infoType.specialResources: 
					
/*					affBmp = new Bitmap( this.ClientSize.Width * 2 / 3, 60 * platformSpec.resolution.mod );
					g = Graphics.FromImage( affBmp );
					g.Clear( Color.Black );

					Random r = new Random();

					if ( Statistics.resources[ ind ].terrain == 1 )
						terrain = (byte)enums.terrainType.plain;
					else
						terrain = (byte)enums.terrainType.coast;

					for ( int y = -1; y < affBmp.Width / 50 + 2; y ++ )
					{
						for ( int x = 0; x < affBmp.Height / 15 + 2; x ++ )
							g.DrawImage( 
								Statistics.terrains[ terrain ].bmp, // Statistics.terrains[ Statistics.resources[ ind ].terrainTypes[ r.Next( Statistics.resources[ ind ].terrainTypes.Length ) ] ].bmp, // To be implemented later
								new Rectangle(
								x * Form1.caseWidth - Form1.caseWidth / 2 - 10,
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

						for ( int x = 0; x < 6; x ++ )
							g.DrawImage( 
								Statistics.terrains[ terrain ].bmp, // Statistics.terrains[ Statistics.resources[ ind ].terrainTypes[ r.Next( Statistics.resources[ ind ].terrainTypes.Length ) ] ].bmp, 
								new Rectangle(
								x * Form1.caseWidth - Form1.caseWidth - 10,
								y * Form1.caseHeight - 20,
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

					g.DrawImage( Statistics.resources[ ind ].bmp,
						new Rectangle(
						affBmp.Width / 2 - 35,
						affBmp.Height / 2 - 25,
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

					g.DrawRectangle(
						blackPen,
						0,
						0,
						affBmp.Width - 1,
						affBmp.Height - 1
						);

					// name
					e.Graphics.DrawString( Statistics.resources[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// image
					e.Graphics.DrawImage( affBmp, this.ClientSize.Width /6, vPos );
					vPos += spacing + affBmp.Height;

					// need techno
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaResourceNeededTechnos ), Statistics.technologies[ Statistics.resources[ ind ].techno ].name ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// food bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaResourceFoodBonus ), Statistics.resources[ ind ].foodBonus ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// prod bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaResourceProductionBonus ), Statistics.resources[ ind ].prodBonus ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// trade bonus
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaResourceTradeBonus ), Statistics.resources[ ind ].tradeBonus ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// desc
					e.Graphics.DrawString( Statistics.resources[ ind ].description, otherFont, blackBrush, spacing, vPos );
*/
					break;
					#endregion

				#region tutorial X
				case enums.infoType.tutorial: 
			
					Tutorial.structure s = Tutorial.getTextFromInd( ind ); 

					// title
					e.Graphics.DrawString( s.title, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// text
					e.Graphics.DrawString( 
						s.text, otherFont, blackBrush, 
						new Rectangle( spacing, vPos, this.ClientSize.Width - 2 * spacing, this.ClientSize.Height ) 
						);
					vPos += spacing + otherVSpacing;

					break;
					#endregion

				#region civ X
				case enums.infoType.civ: 

					// prepare affBmp
					affBmp = new Bitmap( this.ClientSize.Width * 2 / 3, 16 * platformSpec.resolution.mod );
					g = Graphics.FromImage( affBmp );

					g.Clear( Statistics.civilizations[ ind ].color );					

					g.DrawRectangle(
						blackPen,
						0,
						0,
						affBmp.Width - 1,
						affBmp.Height - 1
						);

					// name
					e.Graphics.DrawString( Statistics.civilizations[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// image
					e.Graphics.DrawImage( affBmp, this.ClientSize.Width /6, vPos );
					vPos += spacing + affBmp.Height;

					// leader names
					string leaderNames = "";

					for ( int i = 0; i < Statistics.civilizations[ ind ].leaderNames.Length; i ++ )
					{
						if ( i != 0 )
							leaderNames += ", ";

						leaderNames += Statistics.civilizations[ ind ].leaderNames[ i ]; 
					}
					
					e.Graphics.DrawString( String.Format( language.getAString( language.order.encyclopediaCivilizationLeaderNames ), leaderNames ), otherFont, blackBrush, spacing, vPos );
					vPos += spacing + otherVSpacing;

					// desc
					e.Graphics.DrawString( Statistics.civilizations[ ind ].description, otherFont, blackBrush, spacing, vPos );

					break;
					#endregion

				#region buildings X
				case enums.infoType.buildings: 

					// title
					e.Graphics.DrawString( Statistics.buildings[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// text
					e.Graphics.DrawString( 
						Statistics.buildings[ ind ].description, otherFont, blackBrush, 
						new Rectangle( spacing, vPos, this.ClientSize.Width - 2 * spacing, this.ClientSize.Height ) 
						);
					vPos += spacing + otherVSpacing;

					break;
					#endregion

				#region smallWonders X
				case enums.infoType.smallWonders: 

					// title
					e.Graphics.DrawString( Statistics.smallWonders[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// text
					e.Graphics.DrawString( 
						Statistics.smallWonders[ ind ].description, otherFont, blackBrush, 
						new Rectangle( spacing, vPos, this.ClientSize.Width - 2 * spacing, this.ClientSize.Height ) 
						);
					vPos += spacing + otherVSpacing;

					break;
					#endregion

				#region wonders X
				case enums.infoType.wonders: 

					// title
					e.Graphics.DrawString( Statistics.wonders[ ind ].name, titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// text
					e.Graphics.DrawString( 
						Statistics.wonders[ ind ].description, otherFont, blackBrush, 
						new Rectangle( spacing, vPos, this.ClientSize.Width - 2 * spacing, this.ClientSize.Height ) 
						);
					vPos += spacing + otherVSpacing;

					break;
					#endregion

				#region misc
				case enums.infoType.misc: 

					// title
					e.Graphics.DrawString( "...", titleFont, blackBrush, spacing, vPos );
					vPos += spacing + titleVSpacing;

					// text
					e.Graphics.DrawString( 
						"...", otherFont, blackBrush, 
						new Rectangle( spacing, vPos, this.ClientSize.Width - 2 * spacing, this.ClientSize.Height ) 
						);
					vPos += spacing + otherVSpacing;

					break;
					#endregion

			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
		//	base.OnPaintBackground (e);
		}



		private void frmInfo_Load(object sender, System.EventArgs e)
		{
		//	draw();
			this.Invalidate();
			this.BringToFront();
		}
	}
}
