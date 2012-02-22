using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for uiWrap.
	/// </summary>
	public class uiWrap
	{

		public static string[] buildingListStrings( byte player, int city, Stat.Construction[] ib )
		{
			string[] choices = new string[ ib.Length ];
	//		getPFT getPFT1 = new getPFT();
			int cityProd = Form1.game.playerList[ player ].cityList[ city ].production;

			for ( int h = 0; h < ib.Length; h ++ )
			{
				int turnsLeft = -1;

				choices[ h ] = ib[ h ].name;

				if ( ib[ h ] is Stat.Wealth )
					turnsLeft = turnsLeft = -1000;
				else
					turnsLeft = ( ib[ h ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points ) / ( cityProd + 1 );


		/*		switch ( ib[ h ].type )
				{
					case 1: // units
						choices[ h ] = Statistics.units[ ib[ h ].info ].name;
						turnsLeft = ( Statistics.units[ ib[ h ].info ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points ) / ( cityProd > 0 ? cityProd : 1 );
						break;
												
					case 2: // buildings
						choices[ h ] = Statistics.buildings[ ib[ h ].info ].name;
						turnsLeft = ( Statistics.buildings[ ib[ h ].info ].cost - Form1.game.playerList[ player ].cityList[ city ].construction.points ) / ( cityProd > 0 ? cityProd : 1 );
						break;
												
					case 3:
						choices[ h ] = "Wealth";
						turnsLeft = -1000;
						break;
				}*/

				if ( turnsLeft > 1 )
					choices[ h ] += " ( " + String.Format( language.getAString( language.order.turnsLeft ), turnsLeft ) + " )";
			//		choices[ h ] += " ( " + turnsLeft.ToString() + " turns )";
				else if ( turnsLeft >= 0 ) // -1000 )
					choices[ h ] += " ( " + String.Format( language.getAString( language.order.turnsLeft ), turnsLeft ) + " )";
			//		choices[ h ] += " ( " + turnsLeft.ToString() + " turn )";
			}

			return choices;
		}

		public static string[] technoListStrings( byte player, byte[] technos )
		{
			string[] choices = new string[ technos.Length ];

			getPFT getPft = new getPFT();
		//	int nationTrade = Form1.game.playerList[ player ].totalTrade;
			for ( int h = 0; h < choices.Length; h ++ )
			{
				choices[ h ] = Statistics.technologies[ technos[ h ] ].name;

				int sciTurn = ( Statistics.technologies[ technos[ h ] ].cost - Form1.game.playerList[ Form1.game.curPlayerInd ].technos[ technos[ h ] ].pntDiscovered ) * 100 / ( Form1.game.playerList[ player ].totalTrade * Form1.game.playerList[ player ].preferences.science );
				
				if ( sciTurn > 1 )
					choices[ h ] += " ( " + sciTurn.ToString() + " turns )";
				else
					choices[ h ] += " ( " + sciTurn.ToString() + " turn )";
			}

			return choices;
		}

		public static string unitInfo( byte player, int unit )
		{
			string name = 
				Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].name + ", " + 
				Form1.game.playerList[ player ].unitList[ unit ].health + " hp, " +
				Form1.game.playerList[ player ].unitList[ unit ].moveLeft + " m";
			
			if ( Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction > 0 )
				name += " " + Form1.game.playerList[ player ].unitList[ unit ].moveLeftFraction + @"/3";
			
			if ( Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].transport > 0 )
				name += " " + Form1.game.playerList[ player ].unitList[ unit ].transported + @"/" + Statistics.units[ Form1.game.playerList[ player ].unitList[ unit ].type ].transport;

			return name;
		}

#region chose next techno
		public static void chooseNextTechno( byte player, string text, string caption )
		{
			string technoName = Statistics.technologies[ Form1.game.playerList[ player ].currentResearch ].name;

			ai Ai = new ai();
			byte[] technos = ai.returnDisponibleTechnologies( player );
			byte nextTechno = ai.randomTechnology( player );

			string[] choices = technoListStrings( player, technos );

			userChoice uc = new userChoice(
				caption,
				text,	
				choices,
				0, 
				language.getAString( language.order.uiAccept ),
				language.getAString( language.order.uiOpenTechnoTree )
				);
			uc.ShowDialog();
			int res = uc.result;
						
			if ( res == -1 )
			{ // science tree
				Form1.game.playerList[ player ].currentResearch = (byte)nextTechno;

				sciTree sciTree1 = new sciTree();
				sciTree1.ShowDialog();
			}
			else
			{ // accept
				Form1.game.playerList[ player ].currentResearch = (byte)technos[ res ];
			}
		}
		#endregion

		#region chooseConstruction
		public static void choseConstruction( byte player, int city, bool allNew )
		{
			string text;
			if ( allNew )
			{
				text = "Please chose a construction for " + Form1.game.playerList[ player ].cityList[ city ].name + ".";
			}
			else
			{
				string builtName = Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].name
					;
			/*	if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.cityBuildType.unit )
					builtName = Statistics.units[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].name;
				else //if ( Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.cityBuildType.building )
					builtName = Statistics.buildings[ Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind ].name;*/

				text = String.Format( language.getAString( language.order.uiJustBuiltA ), Form1.game.playerList[ player ].cityList[ city ].name, builtName.ToLower() ); // Form1.game.playerList[ player ].cityList[ city ].name + " just built a " + builtName.ToLower() + ".  Please choose its next construction.";
			}

		//	ai Ai = new ai();

			Stat.Construction[] ib = ai.whatCanBeBuilt(
				player, 
				city 
				);
			string[] choices = uiWrap.buildingListStrings( player, city, ib ); 
			int Default = 0;

			ai.chooseConstruction( player, city );
			for ( int i = 0; i < ib.Length; i ++ )
				if ( 
					Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] == ib[ i ]
		//			Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type == ib[ i ].type &&
		//			Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind == ib[ i ].info
					)
				{
					Default = i;
					break;
				}
			
			userChoice uc = new userChoice( 
				language.getAString( language.order.uiNewConstrution ), 
				text, 
				choices,
				Default, 
				language.getAString( language.order.uiAccept ), 
				language.getAString( language.order.uiGoToCity ) 
				);

			uc.ShowDialog();
			int res = uc.result;
									
			if ( res == -1 )
			{
			//	Ai.chooseConstruction( player, city );
				open.cityFrm( player, city, null );
			}
			else
			{ // si non
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ] = ib[ res ];

		/*		Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].ind = ib[ res ].info;
				Form1.game.playerList[ player ].cityList[ city ].construction.list[ 0 ].type = ib[ res ].type;*/
			}
		}
		#endregion
	}
}
