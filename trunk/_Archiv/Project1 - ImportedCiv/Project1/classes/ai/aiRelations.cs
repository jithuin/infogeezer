using System;
using System.Drawing;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiRelations.
	/// </summary>
	public class aiRelations
	{
		#region endTurn
		public static void endTurn( byte player )
		{
			for ( byte other = 1; other < Form1.game.playerList.Length; other ++ )
				if ( 
					!Form1.game.playerList[ other ].dead 
					&& Form1.game.playerList[ player ].foreignRelation[ other ].madeContact
					)
				{
				// adjust with other opinion
					modifyRelationQuatlityWith( 
						player, 
						(byte)other, 
						( Form1.game.playerList[ other ].foreignRelation[ player ].quality - Form1.game.playerList[ player ].foreignRelation[ other ].quality ) / 10
						); 

				#region units on territory
					if ( 
						Form1.game.playerList[ player ].foreignRelation[ other ].politic == (byte)Form1.relationPolType.ceaseFire ||
						Form1.game.playerList[ player ].foreignRelation[ other ].politic == (byte)Form1.relationPolType.peace
						)
					{
						int[] unitsInTerritory = new int[ Form1.game.playerList[ other ].unitNumber ];
						int nbrUnitsInTerritory = 0;

						for ( int unit = 1; unit <= Form1.game.playerList[ other ].unitNumber; unit ++ )
							if ( 
								Form1.game.playerList[ other ].unitList[ unit ].state != (byte)Form1.unitState.dead &&
								Form1.game.playerList[ other ].unitList[ unit ].state != (byte)Form1.unitState.inTransport &&
								Form1.game.grid[ Form1.game.playerList[ other ].unitList[ unit ].X, Form1.game.playerList[ other ].unitList[ unit ].Y ].territory - 1 == player )
							{
								unitsInTerritory[ nbrUnitsInTerritory ] = unit;
								nbrUnitsInTerritory ++;
							}

						if ( nbrUnitsInTerritory > 0 )
							unitsOnTerritory( other, player, unitsInTerritory, nbrUnitsInTerritory );
					}
					#endregion

				// culture // ( nationTrade != 0 ? nationTrade : 1 )

					int totCulture = Form1.game.playerList[ player ].culture + Form1.game.playerList[ other ].culture;

					/*if ( Form1.game.playerList[ player ].culture + Form1.game.playerList[ other ].culture == 0 )
						modifyRelationQuatlityWith( 
							player, 
							(byte)other, 
							10 * ( Form1.game.playerList[ player ].culture - 5 ) / ( Form1.game.playerList[ player ].culture + Form1.game.playerList[ other ].culture + 1 )
							); 
					else*/
						modifyRelationQuatlityWith( 
							player, 
							(byte)other, 
							10 * ( Form1.game.playerList[ player ].culture - 5 ) / ( totCulture != 0 ? totCulture : 1 )
							); 

				#region friend with friend or ennemy of an ennemy

					for ( int tp = 0; tp < Form1.game.playerList.Length; tp ++ )
						if ( tp != player && tp != other )
						{
							if ( 
								(
								Form1.game.playerList[ player ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.alliance ||
								Form1.game.playerList[ player ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.Protected ||
								Form1.game.playerList[ player ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.Protector
								) &&
								Form1.game.playerList[ player ].foreignRelation[ tp ].quality > 200
								)
							{ // tp is a friend
								if ( 
									Form1.game.playerList[ other ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.alliance ||
									Form1.game.playerList[ other ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.Protected ||
									Form1.game.playerList[ other ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.Protector
									)
								{ // friend of friend
									modifyRelationQuatlityWith( player, other, 10 ); 
								}
								else if (
									Form1.game.playerList[ other ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.war
									)
								{ // ennemy of a friend
									modifyRelationQuatlityWith( player, other, -10 ); 
								}
							}
							else if (
								Form1.game.playerList[ player ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.war
								)
							{ // tp is an ennemy
								if ( 
									Form1.game.playerList[ other ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.alliance ||
									Form1.game.playerList[ other ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.Protected ||
									Form1.game.playerList[ other ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.Protector
									)
								{ // friend of an ennemy
									modifyRelationQuatlityWith( player, other, -10 ); 
								}
								else if (
									Form1.game.playerList[ other ].foreignRelation[ tp ].politic == (byte)Form1.relationPolType.war
									)
								{ // ennemy of an ennemy
									if ( Form1.game.playerList[ player ].foreignRelation[ tp ].quality > Form1.game.playerList[ player ].foreignRelation[ other ].quality )
									{ // hate more tp
										modifyRelationQuatlityWith( player, other, 10 ); 
									}
									else
									{ // hate more other
										modifyRelationQuatlityWith( player, other, -10 ); 
									}
								}
							}
						}
					#endregion

				// strongest player
					modifyRelationQuatlityWith( 
						player, 
						other, 
						10 - ai.whoIsWinning( player, (byte)other )
						); 
				}
		}
		#region
		
		#endregion
		public static void unitsOnTerritory( byte offender, byte offended, int[] units, int unitNbr )
		{
			bool canDeclareWar = false;
			if ( Form1.game.playerList[ offended ].foreignRelation[ offender ].politic == (byte)Form1.relationPolType.ceaseFire )
			{
				modifyRelationQuatlityWith( offended, offender, - 200 ); 
				canDeclareWar = true;
			}
			else if ( Form1.game.playerList[ offended ].foreignRelation[ offender ].politic == (byte)Form1.relationPolType.peace )
			{
				for ( int unit = 0; unit < unitNbr; unit ++ )
				{
					bool isNextToCity = false;
					Point[] sqr = Form1.game.radius.returnEmptySquare( new Point( Form1.game.playerList[ offender ].unitList[ units[ unit ] ].X, Form1.game.playerList[ offender ].unitList[ units[ unit ] ].Y ), 1 );
					for ( int k = 0; k < sqr.Length; k ++ )
						if ( 
							Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].city > 0 && 
							Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == offended
							)
						{
							isNextToCity = true;
							break;
						}

					bool isInCityRange = false;
					if ( !isNextToCity )
					{
						sqr = Form1.game.radius.returnCityRadius( Form1.game.playerList[ offender ].unitList[ units[ unit ] ].X, Form1.game.playerList[ offender ].unitList[ units[ unit ] ].Y );
						for ( int k = 0; k < sqr.Length; k ++ )
							if ( 
								Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].city > 0 && 
								Form1.game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 == offended
								)
							{
								isInCityRange = true;
								break;
							}
					}

					if ( isNextToCity ) 
					{ 
						modifyRelationQuatlityWith( offended, offender, - 100 ); 
						canDeclareWar = true;
					} 
					else if ( isInCityRange ) 
					{
						modifyRelationQuatlityWith( offended, offender, - 50 ); 
					}
					else
					{
						modifyRelationQuatlityWith( offended, offender, - 10 ); 
					}
				}
			}

			if ( canDeclareWar )
			{
				if ( offended == Form1.game.curPlayerInd ) 
					askIfDeclareWarToWBHRFI( offended, offender ); 
				else 
					wantToDeclareWarToWBHRFI( offended, offender ); 
			}
		}
		#endregion
		
		#region wantToDeclareWarToWBHRFI
		/// <summary>
		/// wantToDeclareWarTo without being held responsible for it
		/// </summary>
		/// <param name="player"></param>
		/// <param name="other"></param>
		public static void wantToDeclareWarToWBHRFI( byte player, byte other ) 
		{ // allows the player to declare war without being held responsible, hum... maybe not 
			if ( Form1.game.playerList[ player ].foreignRelation[ other ].quality - ai.whoIsWinning( player, other ) * 10 < - 150 )
				aiPolitics.declareWar( player, other );//startWar( ,  );
		}
		
		public static void askIfDeclareWarToWBHRFI( byte player, byte other ) 
		{ // allows the player to declare war without being held responsible
			if ( 
				MessageBox.Show( 
				"At least one unit from " + 
				Statistics.civilizations[ Form1.game.playerList[ other ].civType ].name + 
				" has approached one of your cities on your territory.  This can be considered as an agression, you could declare war to them without being held responsible for it.  Do you want to do this?", 
				"Ennemy near", 
				MessageBoxButtons.YesNo, 
				MessageBoxIcon.None, 
				MessageBoxDefaultButton.Button1 
				) == DialogResult.Yes 
				)
			{ // declare war
				aiPolitics.declareWar( player, other );//startWar( other, player );
			}
			else
			{ // let it go
			}
		}
		#endregion
		
		#region modifyRelationQuatlityWith
		public static void modifyRelationQuatlityWith( byte player, byte other, int mod ) 
		{ 
			Form1.game.playerList[ player ].foreignRelation[ other ].quality += mod;

			if ( Form1.game.playerList[ player ].foreignRelation[ other ].quality > 1000 )
			{
				Form1.game.playerList[ player ].foreignRelation[ other ].quality = 1000;
			}
			else if ( Form1.game.playerList[ player ].foreignRelation[ other ].quality < - 1000 )
			{
				Form1.game.playerList[ player ].foreignRelation[ other ].quality = - 1000;
			}
		}
		#endregion
		
		#region startWar
	/*	public static void startWar( byte attacker, byte attacked ) 
		{ 
			if ( attacker == Form1.game.curPlayerInd )
			{
			}
			else if ( attacked == Form1.game.curPlayerInd )
			{
				int[] ontmdw = new int[ Form1.game.playerList.Length ];
				int pos = 0;

				for ( int player = 0; player < Form1.game.playerList.Length; player ++ )
					if ( player != attacker && player != attacked )
						if (
							Form1.game.playerList[ attacker ].foreignRelation[ player ].politic == (byte)Form1.relationPolType.alliance ||
							Form1.game.playerList[ attacker ].foreignRelation[ player ].politic == (byte)Form1.relationPolType.Protected ||
							Form1.game.playerList[ attacker ].foreignRelation[ player ].politic == (byte)Form1.relationPolType.Protector
							)
						{
							ontmdw[ pos ] = player;
							pos ++;
						}

				string add = "";;
				if ( pos > 1 )
				{
					add = " Their allies ";
					for ( int p = 0; p < pos - 2; p ++ )
					{
						add += Statistics.civilizations[ Form1.game.playerList[ ontmdw[ p ] ].civType ].name + ", ";
					}

					add += Statistics.civilizations[ Form1.game.playerList[ pos - 2 ].civType ].name + 
						" and " + 
						Statistics.civilizations[ Form1.game.playerList[ pos - 1 ].civType ].name + 
						" may follow them in this war.";
				}
				else if ( pos > 0 )
				{
					add = " Their ally " + Statistics.civilizations[ Form1.game.playerList[ 0 ].civType ].name + " may follow them in this war.";
				}

				MessageBox.Show( 
					Form1.game.playerList[ attacker ].playerName + " of " + Statistics.civilizations[ Form1.game.playerList[ attacker ].civType ].name + " declared war on you." + add,
					"A war has begun" 
					);
			}
			else
			{
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ attacker ].spies[ (byte)enums.spyType.gov ].nbr > 0 ||
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ attacker ].spies[ (byte)enums.spyType.military ].nbr > 0 ||
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ attacker ].spies[ (byte)enums.spyType.people ].nbr > 0 ||
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ attacked ].spies[ (byte)enums.spyType.gov ].nbr > 0 ||
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ attacked ].spies[ (byte)enums.spyType.military ].nbr > 0 ||
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ attacked ].spies[ (byte)enums.spyType.people ].nbr > 0
					)
				{
					MessageBox.Show( 
						Form1.game.playerList[ attacker ].playerName + " of " + Statistics.civilizations[ Form1.game.playerList[ attacker ].civType ].name + " declared war on " + Statistics.civilizations[ Form1.game.playerList[ attacked ].civType ].name + ".",
						"A war has begun" 
						);
				}
			}

			Form1.game.playerList[ attacker ].foreignRelation[ attacked ].politic = (byte)Form1.relationPolType.war;
			Form1.game.playerList[ attacked ].foreignRelation[ attacker ].politic = (byte)Form1.relationPolType.war;

			if ( Form1.game.playerList[ attacker ].foreignRelation[ attacked ].quality > 0 )
				Form1.game.playerList[ attacker ].foreignRelation[ attacked ].quality = 0;
			
			if ( Form1.game.playerList[ attacked ].foreignRelation[ attacker ].quality > 0 )
				Form1.game.playerList[ attacked ].foreignRelation[ attacker ].quality = 0;
		}*/
		#endregion
	}
}
