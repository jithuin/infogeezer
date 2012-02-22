using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for aiPolitics.
	/// </summary>
	public class aiPolitics
	{

		#region acceptPeaceOffer
		public static bool acceptPeaceOffer( byte asking, byte asked )
		{
			int totAllies = 0, totWars = 0, totIsWinning = 0, totTechno = 0, totPlayers = 0, playerTechno = count.technoNumber( asked );
			int[] allIsWinning = new int[ Form1.game.playerList.Length ];

			for ( int i = 0; i < Form1.game.playerList.Length; i ++ )
				if ( 
					i != asked && 
					!Form1.game.playerList[ i ].dead 
					)
				{ 
					totPlayers ++;
					totTechno += count.technoNumber( (byte)i );

					if ( Form1.game.playerList[ asked ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.war )
					{ 
						totWars ++; 
						int twiw = ai.whoIsWinning( asked, (byte)i ); 
						totIsWinning += twiw; 
						totIsWinning -= 10; 

						allIsWinning[ i ] = twiw; 
						allIsWinning[ i ] -= 10; 
					} 
					else if ( 
						Form1.game.playerList[ asked ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.alliance ||
						Form1.game.playerList[ asked ].foreignRelation[ i ].politic == (byte)Form1.relationPolType.Protector
						) 
					{ 
						totAllies ++; 
					} 
				} 
			
			if ( 
				( // reason to stay in war 
				count.technoNumber( asking ) > count.technoNumber( asked ) + 1 || 
				allIsWinning[ asked ] > 9 
				) && 
				( // not too much wars 
				totWars < totAllies || 
				totWars < totPlayers / 3 
				) 
				) 
				return false; 
			else 
				return true; 
		} 
		#endregion
		
		#region declareWar
		public static void declareWar( byte declarer, byte victim )
		{
			Form1.game.playerList[ declarer ].foreignRelation[ victim ].politic = (byte)Form1.relationPolType.war;
			Form1.game.playerList[ victim ].foreignRelation[ declarer ].politic = (byte)Form1.relationPolType.war;

			if ( victim == Form1.game.curPlayerInd )
			{
				System.Windows.Forms.MessageBox.Show( Statistics.civilizations[ Form1.game.playerList[ declarer ].civType ].name + " declared war on you.  Sorry", "New war" );
			}
			else if ( declarer == Form1.game.curPlayerInd )
			{
			}
			else
			{
				string attackerCalif = "", defenserCalif = "";

				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ declarer ].politic == (byte)Form1.relationPolType.alliance || 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ declarer ].politic == (byte)Form1.relationPolType.Protected || 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ declarer ].politic == (byte)Form1.relationPolType.Protector
					)
					attackerCalif = "Your ally ";
				else if ( Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ declarer ].politic == (byte)Form1.relationPolType.war )
					attackerCalif = "Your ennemy ";
				
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ victim ].politic == (byte)Form1.relationPolType.alliance || 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ victim ].politic == (byte)Form1.relationPolType.Protected || 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ victim ].politic == (byte)Form1.relationPolType.Protector
					)
					defenserCalif = "your ally "; 
				else if ( Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ victim ].politic == (byte)Form1.relationPolType.war )
					defenserCalif = "your ennemy "; 

				System.Windows.Forms.MessageBox.Show( attackerCalif + Statistics.civilizations[ Form1.game.playerList[ declarer ].civType ].name + " declared war on " + defenserCalif + Statistics.civilizations[ Form1.game.playerList[ victim ].civType ].name, "New war" );
			} 

			Form1.game.playerList[ declarer ].memory.add( 
				(byte)enums.playerMemory.war, 
				new int[] { victim, declarer, 0 } // with, who started it,  why 
				); 
			Form1.game.playerList[ victim ].memory.add( 
				(byte)enums.playerMemory.war, 
				new int[] { declarer, declarer, 0 } 
				); 
			Form1.game.playerList[ victim ].memory.cancelExchangesWith( declarer );
			Form1.game.playerList[ declarer ].memory.cancelExchangesWith( victim );
		}
		#endregion
		
		#region declarePeace
		public static void declarePeace( byte declarer, byte victim )
		{
			Form1.game.playerList[ declarer ].foreignRelation[ victim ].politic = (byte)Form1.relationPolType.peace;
			Form1.game.playerList[ victim ].foreignRelation[ declarer ].politic = (byte)Form1.relationPolType.peace;

			if ( victim == Form1.game.curPlayerInd )
			{
			}
			else if ( declarer == Form1.game.curPlayerInd )
			{
			}
			else
			{
				string attackerCalif = "", defenserCalif = "";

				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ declarer ].politic == (byte)Form1.relationPolType.alliance || 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ declarer ].politic == (byte)Form1.relationPolType.Protected || 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ declarer ].politic == (byte)Form1.relationPolType.Protector
					)
					attackerCalif = "Your ally ";
				else if ( Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ declarer ].politic == (byte)Form1.relationPolType.war )
					attackerCalif = "Your ennemy ";
				
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ victim ].politic == (byte)Form1.relationPolType.alliance || 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ victim ].politic == (byte)Form1.relationPolType.Protected || 
					Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ victim ].politic == (byte)Form1.relationPolType.Protector
					)
					defenserCalif = "your ally ";
				else if ( Form1.game.playerList[ Form1.game.curPlayerInd ].foreignRelation[ victim ].politic == (byte)Form1.relationPolType.war )
					defenserCalif = "your ennemy ";

				System.Windows.Forms.MessageBox.Show( attackerCalif + Statistics.civilizations[ Form1.game.playerList[ declarer ].civType ].name + " is now in peace with " + defenserCalif + Statistics.civilizations[ Form1.game.playerList[ victim ].civType ].name, "New war" );
			}

			Form1.game.playerList[ declarer ].memory.add( 
				(byte)enums.playerMemory.peace,
				new int[] { victim, declarer, 0 }
				);
			Form1.game.playerList[ victim ].memory.add( 
				(byte)enums.playerMemory.peace,
				new int[] { declarer, declarer, 0 }
				);
		}
		#endregion

	} 
} 
