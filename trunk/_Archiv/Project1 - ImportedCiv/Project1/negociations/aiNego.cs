using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiNego.
	/// </summary>
	public class aiNego
	{

#region initNego
		public static void initNego( byte player, byte otherPlayer )
		{
			if ( !Form1.game.playerList[ player ].memory.attemptedNegoRecently( otherPlayer ) )
			{
				int p = 1;
				negoList nl = new negoList( new byte[] { otherPlayer, player } );
				int[] whw = nl.whatHeWants( p );
				int totInterestings = 0, totInterest = 0;

				for ( int i = 0; i < whw.Length; i++ )
					if ( whw[ i ] > 0 )
					{
						totInterestings ++;
						totInterest += whw[ i ];
					}

				if ( totInterestings > 0 )
					for ( int i = 0; i < nl.Length; i++ )
						if ( 
							whw[ i ] > totInterestings * 33 / 100 /*&&
							whw[ i ] < totInterestings * 66 / 100*/
							)
							nl.add( i );

				nl = fillProp( nl, (p+1)%2 );
				if ( nl != null )
				{
					for ( int p1 = 0; p1 < 2; p1 ++ )
						Form1.game.playerList[ nl.players[ p1 ] ].memory.addAttemptedNego( nl.players[ (p1+1)%2 ] );

					if ( acceptToInitNego( nl, p ) )
					{
						if ( otherPlayer == Form1.game.curPlayerInd )
						{
							wC.show = false;
							frmNegociation fn = new frmNegociation( Form1.game.curPlayerInd, player, -1 );

							fn.list = nl;
							fn.populateLBs();
							fn.ShowDialog();
							wC.show = true;
						}
						else
						{
							nl = fillProp( nl, p );
							if ( nl != null )
								nl.executeExchange();
						}
					}
				}
			}
		}
#endregion

#region acceptToInitNego
		public static bool acceptToInitNego( negoList nl, int proposer )
		{
			if ( nl.players[ ( proposer + 1 ) % 2 ] == Form1.game.curPlayerInd )
			{
				if( System.Windows.Forms.MessageBox.Show( 
					String.Format( language.getAString( language.order.tradeDoYouWantTo ), Form1.game.playerList[ nl.players[ proposer ] ].playerName ), 
					String.Format( language.getAString( language.order.panNegociatingWith ), Form1.game.playerList[ nl.players[ proposer ] ].civName ),
					System.Windows.Forms.MessageBoxButtons.YesNo,
					System.Windows.Forms.MessageBoxIcon.None,
					System.Windows.Forms.MessageBoxDefaultButton.Button1
					) == System.Windows.Forms.DialogResult.Yes
					)
					return true;
				else
					return false;
			}
			else
			{
				int[] whw = nl.whatHeWants( ( proposer + 1 ) % 2 );
				int totInterestings = 0;

				for ( int i = 0; i < whw.Length; i++ )
					if ( whw[ i ] > 0 )
						totInterestings += whw[ i ];

				if ( totInterestings > 0 )
					return true;
				else
					return false;
			}
		}
#endregion

#region negoValid
		public static bool negoValid( byte playerInit, byte otherPlayer )
		{
			int techno = 0;

			for ( int i = 0; i < Form1.game.playerList[ playerInit ].technos.Length; i ++ )
				if ( 
					Form1.game.playerList[ playerInit ].technos[ i ].researched != Form1.game.playerList[ otherPlayer ].technos[ i ].researched
					)
					techno ++;

			return true;
		}
#endregion

#region evaluateOffer
		/// <summary>
		/// diff = gain player 1 - gain player 0
		/// </summary>
		/// <returns>gain player 1 - gain player 0, if >0 comp accept</returns>
	/*	public static long[] evaluateOffer( negoList list )
		{
			//long[] gain = new long[ 2 ];
			/*int cityValuePerPop = 50,
			cityValuePerBuilding = 20,
			contact = 30,
			techno = 2,
			mapPerCase = 5;

			for ( int p = 0; p < 2; p++ )
			{
				int wiw = ai.whoIsWinning( players[ p ], players[ ( p + 1 ) % 2 ] ) - 10;

				gain[ p ] += moneyGiven[ p ];

				if ( moneyPerTurn[ p ] > 0 )
					for ( int t = 0; t < 20; t++ )
						gain[ p ] += (int)( moneyPerTurn[ p ] * ( 100 - 15 * Math.Pow( t, 0.4 ) ) / 100 );

				for ( int i = 0; i < bilateral.Length; i++ )
					if ( usedInBilateral[ i ] )
						gain[ p ] += getGain( players, bilateral[ i ], p, wiw );

				for ( int i = 0; i < unilateral[ p ].Length; i++ )
					if ( usedInUnilateral[ p ][ i ] )
						gain[ p ] += getGain( players, unilateral[ p ][ i ], p, wiw );
			}

			return gain;//gain[ai] - gain[ (ai+1)%2 ];
		}*/

#endregion

#region getGain
	/*	public static int getGain(byte[] players, structures.intByte ib, int p, int wiw)
		{
			int gain = 0;
			int cityValuePerPop = 50,
				cityValuePerBuilding = 20,
				contact = 30,
				techno = 2,
				mapPerCase = 5;

			if ( ib.type == (byte)nego.infoType.breakAllianceWith )
			{
				if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.alliance || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protected || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protector )
					gain -= 150; // difficulty
					else if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.war )
					gain += 200;
				else
					gain -= 50; // difficulty
			}
			else if ( ib.type == (byte)nego.infoType.warOn )
			{
				if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.alliance || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protected || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protector )
					gain -= 350; // difficulty
					else if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.war )
					gain += 300;
				else
					gain -= 200; // difficulty
			}
			else if ( ib.type == (byte)nego.infoType.embargoOn )
			{
				//	int wiw = ai.whoIsWinning( players[ p ], players[ ( p + 1 ) % 2 ] ) * -1 - 10;
				if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.alliance || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protected || Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.Protector )
					gain -= 350; // difficulty
					else if ( Form1.game.playerList[ players[ p ] ].foreignRelation[ ib.info ].politic == (byte)Form1.relationPolType.war )
					gain += 300;
				else
					gain -= 200; // difficulty
			}
			else if ( ib.type == (byte)nego.infoType.politicTreaty )
			{
				if ( ib.info == (byte)Form1.relationPolType.ceaseFire )
					gain += wiw * -40;
				else if ( ib.info == (byte)Form1.relationPolType.peace )
					gain += wiw * -10;
				else if ( ib.info == (byte)Form1.relationPolType.alliance || ib.info == (byte)Form1.relationPolType.Protected || ib.info == (byte)Form1.relationPolType.Protector )
					gain += wiw * -10;
			}
			else if ( ib.type == (byte)nego.infoType.economicTreaty )
			{
				gain += 10;
			}
			else if ( ib.type == (byte)nego.infoType.votes )
			{
				gain += 10;
			}
			else if ( ib.type == (byte)nego.infoType.giveCity )
			{
				gain -= cityValuePerPop * Form1.game.playerList[ players[ p ] ].cityList[ ib.info ].population + cityValuePerBuilding * Form1.game.playerList[ players[ p ] ].cityList[ ib.info ].buildingCount;
			}
			else if ( ib.type == (byte)nego.infoType.giveContactWith )
			{
				gain -= contact;
			}
			else if ( ib.type == (byte)nego.infoType.giveMap )
			{
				if ( ib.info == 0 ) // world map
				{
					int tot = 0;

					for ( int x = 0; x < Form1.game.width; x++ )
						for ( int y = 0; y < Form1.game.height; y++ )
							if ( !Form1.game.playerList[ players[ p ] ].discovered[ x, y ] && Form1.game.playerList[ players[ ( p + 1 ) % 2 ] ].discovered[ x, y ] )
								tot++;

					gain -= tot * mapPerCase;
				}
				else // territory map
				{
					int tot = 0;

					for ( int x = 0; x < Form1.game.width; x++ )
						for ( int y = 0; y < Form1.game.height; y++ )
							if ( !Form1.game.playerList[ players[ p ] ].discovered[ x, y ] && Form1.game.grid[ x, y ].territory - 1 == players[ ( p + 1 ) % 2 ] )
								tot++;

					gain -= tot * mapPerCase;
				}
			}
			else if ( ib.type == (byte)nego.infoType.giveTechno )
			{
				gain -= Statistics.technologies[ ib.info ].cost * techno;
			}
			else if ( ib.type == (byte)nego.infoType.threat )
			{
				if ( ib.info == 0 ) // war
					gain += wiw * 50;
				else if ( ib.info == 1 ) // embargo
					gain += wiw * 10; // var with exchanges
					else // alliance
					gain += wiw * 20;
			}
			else if ( ib.type == (byte)nego.infoType.giveRegion )
			{
				for ( int c = 1; c <= Form1.game.playerList[ players[ p ] ].cityNumber; c++ )
					if ( Form1.game.playerList[ players[ p ] ].cityList[ c ].state != (byte)enums.cityState.dead && Form1.game.playerList[ players[ p ] ].cityList[ c ].originalOwner == ib.info )
						gain -= cityValuePerPop * Form1.game.playerList[ players[ p ] ].cityList[ ib.info ].population + cityValuePerBuilding * Form1.game.playerList[ players[ p ] ].cityList[ ib.info ].buildingCount;
			}

			return gain;
		}*/
			
#endregion

#region fillProp
		/// <summary>
		/// 
		/// </summary>
		/// <param name="p">ai pos who evaluates</param>
		/// <returns></returns>
		public static negoList fillProp( negoList list, int p )
		{

	/*		negoList oriList = /*new negoList( list.players );
			oriList =* list.getClone();			*/						

			negoList tempList = list.getClone();

			long[] gain = tempList.getTotalGain();
			int[] whw = tempList.whatHeWants( p );

			while ( gain[ p ] < 0 )
			{
				long[] possibilities = new long[ tempList.Length ];
				bool[] valid = new bool[ tempList.Length ];

				for ( int i = 0; i < tempList.Length; i ++ ) 
					if ( !tempList.list[ i ].accepted ) 
					{ 
						long[] pg = tempList.getPossibleGain( i ); 
						if ( 
							pg[ p ] > 0 && 
							pg[ (p+1)%2 ] + gain[ (p+1)%2 ] > 0 
							)
						{
							possibilities[ i ] = whw[ i ] * 300 + pg[ p ]; //pg[ p ] - pg[ ( p + 1 ) % 2 ];
							valid[ i ] = true;
						}
					} 

				int[] order = count.descOrder( possibilities );
				if ( valid[ order[ 0 ] ] )
				{
					long[] pg = tempList.getPossibleGain( order[ 0 ] ); 

					for ( int k = 0; k < 2; k ++ )
						gain[ k ] += pg[ k ];

					tempList.add( order[ 0 ] );
				}
				else
				{
					break;
				}
			}

			if ( tempList.giveMoney[ p ] == 0 )
			{
				while ( 
					gain[ p ] < 0 && 
					tempList.giveMoney[ ( p + 1 ) % 2 ] < tempList.canGiveMoney[ ( p + 1 ) % 2 ]
					)
				{
					gain[ p ] += 1;
					gain[ ( p + 1 ) % 2 ] -= 1;
					tempList.addToGiveMoney( ( p + 1 ) % 2, 1 );
				}

				while ( 
					gain[ p ] < 0 && 
					tempList.moneyPerTurn[ ( p + 1 ) % 2 ] < tempList.canGivePerTurn[ ( p + 1 ) % 2 ] 
					)
				{
					gain[ p ] += tempList.getValuePerTurn( 1 );
					gain[ ( p + 1 ) % 2 ] -= tempList.getValuePerTurn( 1 );
					tempList.addToMoneyPerTurn( ( p + 1 ) % 2, 1 );
				}
			}

			if ( gain[ p ] > 0 )
				return tempList;
			else
				return null;
		}

#endregion

		public struct fillPropReturn
		{
			public int[] moneyGiven;
			public int[] moneyPerTurn;
			public bool[] usedInBilateral;
			public bool[][] usedInUnilateral;
			public bool accepted;
		}

	}
}
