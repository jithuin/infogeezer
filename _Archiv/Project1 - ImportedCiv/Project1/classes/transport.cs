using System; 
using System.Drawing; 

namespace xycv_ppc 
{ 
	/// <summary> 
	/// Summary description for transport. 
	/// </summary> 
	public class transport 
	{ 

#region withFreeSpace
		/// <summary>
		/// return global ind
		/// </summary>
		/// <param name="player"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static int[] withFreeSpace( byte player, int x, int y )
		{
			int[] retour = new int[ Form1.game.grid[ x, y ].stack.Length ];
			int pos = 0;

			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++ )
				if ( 
					Form1.game.grid[ x, y ].stack[ i - 1 ].player.player == player &&
					Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.speciality != enums.speciality.carrier &&
					Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.transport > 
					Form1.game.grid[ x, y ].stack[ i - 1 ].transported
					)
				{
					retour[ pos ] = Form1.game.grid[ x, y ].stack[ i - 1 ].ind;
					pos ++;
				}

			int[] retour1 = new int[ pos ];
			for ( int i = 0; i < retour1.Length; i ++ )
				retour1[ i ] = retour[ i ];

			return retour1;
		}
#endregion

#region carrierWithFreeSpace
		/// <summary>
		/// return global ind
		/// </summary>
		/// <param name="player"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static int[] carrierWithFreeSpace(byte player, int x, int y)
		{
			int[] retour = new int[ Form1.game.grid[ x, y ].stack.Length ];
			int pos = 0;

			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++ )
				if ( 
					Form1.game.grid[ x, y ].stack[ i - 1 ].player.player == player && 
					Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.speciality == enums.speciality.carrier &&
					Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.transport > Form1.game.grid[ x, y ].stack[ i - 1 ].transported 
					)
				{
					retour[ pos ] = Form1.game.grid[ x, y ].stack[ i - 1 ].ind;
					pos++;
				}

			int[] retour1 = new int[ pos ];

			for ( int i = 0; i < retour1.Length; i++ )
				retour1[ i ] = retour[ i ];

			return retour1;
		}
#endregion

#region randomTransportWithFreeSpace
		public static int randomTransportWithFreeSpace( byte player, int x, int y )
		{
			int[] ships = withFreeSpace( player, x, y );

			Random r = new Random();
			return ships[ r.Next( ships.Length ) ];
		}
#endregion

#region randomCarrierWithFreeSpace
		public static int randomCarrierWithFreeSpace(byte player, int x, int y)
		{
			int[] ships = carrierWithFreeSpace( player, x, y );
			Random r = new Random();

			return ships[ r.Next( ships.Length ) ];
		}
#endregion

#region spaceInATransport
		
		public static bool spaceInATransport( byte player, int x, int y )
		{
			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++ )
				if ( 
					Form1.game.grid[ x, y ].stack[ i - 1 ].player.player == player &&
					Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.speciality != enums.speciality.builder &&
					Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.transport > 
					Form1.game.grid[ x, y ].stack[ i - 1 ].transported
					)
					return true;

			return false;
		}
#endregion

#region spaceInACarrier
		
		public static bool spaceInACarrier(byte player, int x, int y)
		{
			for ( int i = 1; i <= Form1.game.grid[ x, y ].stack.Length; i++ )
				if ( 
					Form1.game.grid[ x, y ].stack[ i - 1 ].player.player == player && 
					Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.speciality == enums.speciality.builder &&
					Form1.game.grid[ x, y ].stack[ i - 1 ].typeClass.transport > Form1.game.grid[ x, y ].stack[ i - 1 ].transported 
					)
					return true;

			return false;
		}
#endregion

#region unitsWhoCanDisembark
		
		/// <summary>
		/// Return units' position in transport
		/// </summary>
		/// <param name="player"></param>
		/// <param name="transport"></param>
		/// <returns></returns>
		public static int[] unitsWhoCanDisembark( byte player, int transport )
		{
			int[] uwcd = new int[ Form1.game.playerList[ player ].unitList[ transport ].transported ];
			int pos = 0;
			for ( int i = 0; i < Form1.game.playerList[ player ].unitList[ transport ].transported; i++ )
				if ( 
					Form1.game.playerList[ player ].unitList[ Form1.game.playerList[ player ].unitList[ transport ].transport[ i ] ].state != (byte)Form1.unitState.dead &&
					(
					Form1.game.playerList[ player ].unitList[ Form1.game.playerList[ player ].unitList[ transport ].transport[ i ] ].moveLeft > 0 ||
					Form1.game.playerList[ player ].unitList[ Form1.game.playerList[ player ].unitList[ transport ].transport[ i ] ].moveLeftFraction > 0
					)
					)
				{
					uwcd[ pos ] = i;
					pos ++;
				}

			int[] retour = new int[ pos ];
			for ( int i = 0; i < retour.Length; i ++ )
			{
				retour[ i ] = uwcd[ i ];
			}

			return retour;
		}
#endregion

#region canDisembarkAUnit

		public static bool canDisembarkAUnit( byte player, int transport )
		{
			for ( int i = 0; i < Form1.game.playerList[ player ].unitList[ transport ].transported; i++ )
				if ( 
					Form1.game.playerList[ player ].unitList[ Form1.game.playerList[ player ].unitList[ transport ].transport[ i ] ].state != (byte)Form1.unitState.dead &&
					(
						Form1.game.playerList[ player ].unitList[ Form1.game.playerList[ player ].unitList[ transport ].transport[ i ] ].moveLeft > 0 ||
						Form1.game.playerList[ player ].unitList[ Form1.game.playerList[ player ].unitList[ transport ].transport[ i ] ].moveLeftFraction > 0
					)
					)
				{
					return true;
				}

			return false;
		}
#endregion

#region askForWhichUnitToDisembark

		/// <summary>
		/// Return unit's position in transport
		/// </summary>
		/// <param name="player"></param>
		/// <param name="transport"></param>
		/// <returns></returns>
		public static int askForWhichUnitToDisembark( byte player, int transport ) 
		{ 
			int[] uwcd = unitsWhoCanDisembark( player, transport ); 

			if ( uwcd.Length == 1 ) 
			{ 
				if ( 
					System.Windows.Forms.MessageBox.Show( 
					"Are you sure you want to disembark " + uiWrap.unitInfo( player, Form1.game.playerList[ player ].unitList[ transport ].transport[ uwcd[ 0 ] ] ).ToLower() + ".", 
					"Unloading unit", 
					System.Windows.Forms.MessageBoxButtons.YesNo, 
					System.Windows.Forms.MessageBoxIcon.None, 
					System.Windows.Forms.MessageBoxDefaultButton.Button1 
					) == System.Windows.Forms.DialogResult.Yes 
					) 
					return uwcd[ 0 ]; 
				else 
					return -1; 
			} 
			else
			{
				string[] choices = new string[ uwcd.Length + 1 ];
				for ( int i = 0; i < uwcd.Length; i++ )
					choices[ i ] = uiWrap.unitInfo( player, Form1.game.playerList[ player ].unitList[ transport ].transport[ uwcd[ i ] ] );

				choices[ choices.Length - 1 ] = "All";

				userChoice ui = new userChoice( 
					language.getAString( language.order.uiUnloadingUnit ),
					language.getAString( language.order.uiPleaseChooseUnitToUnload ),
					choices,
					0,
					language.getAString( language.order.uiUnload ), 
					language.getAString( language.order.cancel ) 
					);	
				ui.ShowDialog();
				int result = ui.result;

				if ( result == -1 )
					return -1;
				else if ( result == uwcd.Length )
					return 100;
				else
					return uwcd[ result ];
			}
		}
		#endregion

#region askForWhichShipToJoin

		/// <summary>
		/// Return transport number
		/// </summary>
		/// <param name="player"></param>
		/// <param name="transport"></param>
		/// <returns></returns>
		public static int askForWhichShipToJoin( byte player, int x, int y )
		{
			int[] twfs = withFreeSpace( player, x, y );
			
			if ( twfs.Length == 1 )
			{
				/*	if ( 
						System.Windows.Forms.MessageBox.Show( 
						"Are you sure you want to load " + uiWrap.unitInfo( player, Form1.game.playerList[ player ].unitList[ transport ].transport[ uwcd[ 0 ] ] ).ToLower() + ".", 
						"Loading unit on ship",
						System.Windows.Forms.MessageBoxButtons.YesNo,
						System.Windows.Forms.MessageBoxIcon.None,
						System.Windows.Forms.MessageBoxDefaultButton.Button1
						) == System.Windows.Forms.DialogResult.Yes 
						)
					{*/
				return twfs[ 0 ];
				/*}
				else
				{
					return -1;
				}*/
			}
			else
			{
				string[] choices = new string[ twfs.Length ];
				for ( int i = 0; i < choices.Length; i ++ )
					choices[ i ] = uiWrap.unitInfo( player, twfs[ i ] );

				userChoice ui = new userChoice( 
					language.getAString( language.order.uiLoadingUnitOnShip ),
					language.getAString( language.order.uiPleaseChooseAShip ),
					choices,
					0, 
					language.getAString( language.order.uiEmbark ), 
					language.getAString( language.order.cancel )
					);
				ui.ShowDialog();
				int result = ui.result;

				if ( result == -1 )
					return -1;
				else
					return twfs[ result ];
			}

		}
		#endregion

#region removeUnitFromTransport

		/// <summary>
		/// 
		/// </summary>
		/// <param name="player"></param>
		/// <param name="unit">Unit global ind</param>
		/// <param name="transport">Unit ind</param>
		public static void removeUnitFromTransport( byte player, int unit, int transport )
		{
			int localInd = -1;
			for ( int i = 0; i < Form1.game.playerList[ player ].unitList[ transport ].transported; i ++ )
				if ( Form1.game.playerList[ player ].unitList[ transport ].transport[ i ] == unit )
				{
					localInd = i;
					break;
				}

			if ( localInd != -1 )
			{
				int[] buffer = new int[ Form1.game.playerList[ player ].unitList[ transport ].transport.Length ]; 
				for ( int i = 0; i < Form1.game.playerList[ player ].unitList[ transport ].transport.Length; i ++ )
					buffer[ i ] = Form1.game.playerList[ player ].unitList[ transport ].transport[ i ];

				for ( int i = 0, j = 0; i < Form1.game.playerList[ player ].unitList[ transport ].transported; i ++ ) 
					if ( i != localInd ) 
					{ 
						Form1.game.playerList[ player ].unitList[ transport ].transport[ j ] = buffer[ i ]; 
						j ++; 
					} 

				Form1.game.playerList[ player ].unitList[ transport ].transported --;
			}
			else
				System.Windows.Forms.MessageBox.Show( 
					"Unit not in transport", 
					"Error in removeUnitFromTransport, HAaa!" 
					); 
		}
		#endregion

#region nearWhatCont

		public static int[] nearWhatCont( byte player, int unit )
		{
			int[] conts = new int[ 9 ];
			int pos = 0, 
				x = Form1.game.playerList[ player ].unitList[ unit ].X, 
				y = Form1.game.playerList[ player ].unitList[ unit ].Y;

			if ( 
				Form1.game.grid[ x, y ].type != (byte)enums.terrainType.sea /* && 
				Form1.game.grid[ x, y ].type != (byte)enums.terrainType.coast */
				)
			{
				if ( Form1.game.grid[ x, y ].continent > 0 )
				{
					/*if ( pos >= conts.Length )
					{
						int[] buffer = conts;
						conts = new int[ buffer.Length + 10 ];
						buffer.CopyTo( conts, 0 );
					}*/

					conts[ pos ] = Form1.game.grid[ x, y ].continent;
					pos ++;
				}

				Point[] sqr = Form1.game.radius.returnEmptySquare( x, y, 1 );
				for ( int i = 0; i < sqr.Length; i ++ )
					if ( Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].continent > 0 )
					{
						bool alreadyUsed = false;
						for ( int c = 0; c < pos; c ++ )
							if ( conts[ c ] == Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].continent )
							{
								alreadyUsed = true;
								break;
							}

						if ( !alreadyUsed )
						{
							conts[ pos ] = Form1.game.grid[ sqr[ i ].X, sqr[ i ].Y ].continent;
							pos++;
						}
					}
				

				int[] retour = new int[ pos ];

				for ( int c = 0; c < pos; c ++ )
					retour[ c ] = conts[ c ];

				return retour;
			}
			else
			{
				return new int[ 0 ]; 
			}

		}
		#endregion

#region transportOf

		public static int transportOf( byte player, int unit )
		{
			for ( int i = 1; i <= Form1.game.playerList[ player ].unitNumber; i ++ )
				if ( 
					Form1.game.playerList[ player ].unitList[ i ].state != (byte)Form1.unitState.dead &&
					Form1.game.playerList[ player ].unitList[ i ].transported > 0
					)
					for ( int j = 0; j < Form1.game.playerList[ player ].unitList[ i ].transported; j ++ )
						if ( Form1.game.playerList[ player ].unitList[ i ].transport[ j ] == unit )
							return i;

			return -1;
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="player"></param>
		/// <param name="unit">boat</param>
		/// <param name="destOnLand">dest to pickup or disembark</param>
		/// <returns></returns>
		public static Point destOnWater( byte player, int unit, Point destOnLand )
		{
			structures.pntWithDist[] pwd = Form1.game.radius.findNearestCaseTo( 
				destOnLand.X, 
				destOnLand.Y, 
				Form1.game.playerList[ player ].unitList[ unit ].X, 
				Form1.game.playerList[ player ].unitList[ unit ].Y, 
				0, 
				player, 
				true, 
				false 
				); 

			if ( pwd.Length > 0 )
				return new Point( pwd[ 0 ].X, pwd[ 0 ].Y ); 
			else
				return new Point( -1, -1 ); 
		}

		public static int findTransporterOf(byte player, int unit)
		{
			for ( int i = 1; i <= Form1.game.playerList[ player ].unitNumber; i ++ )
				if ( !Form1.game.playerList[ player ].unitList[ i ].dead )
					for ( int t = 0; t < Form1.game.playerList[ player ].unitList[ i ].transported; t ++ )
						if ( Form1.game.playerList[ player ].unitList[ i ].transport[ t ] == unit )
							return i;

			return -1;
		}
	} 
} 
