using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for count.
	/// </summary>
	public class count
	{
	/*	public static int militaryUnits( byte player )
		{
			int un = 0;
			for ( int i = 1; i <= Form1.game.playerList[ player ].unitNumber; i ++ )
				if ( 
					Form1.game.playerList[ player ].unitList[ i ].state !=  (byte)Form1.unitState.dead && 
					Statistics.units[ Form1.game.playerList[ player ].unitList[ i ].type ].speciality != enums.speciality.builder 
					)
					un ++;

			return un;
		}	*/

		public static int militaryFunding( byte player )
		{
			getPFT gp = new getPFT();
			return Form1.game.playerList[ player ].totalTrade * Form1.game.playerList[ player ].preferences.military / 100;
		}

		public static int upkeepCost( byte player )
		{
			int tot = 0;

			for ( int city = 1; city <= Form1.game.playerList[ player ].cityNumber; city ++ )
				if ( Form1.game.playerList[ player ].cityList[ city ].state != (byte)enums.cityState.dead )
					for ( int building = 0; building < Form1.game.playerList[ player ].cityList[ city ].buildingList.Length; building ++ )
						if ( Form1.game.playerList[ player ].cityList[ city ].buildingList[ building ] )
							tot += Statistics.buildings[ building ].upkeep;

			return tot;
		}

		public static int technoNumber( byte player )
		{
			int tot = 0;

			for ( int i = 0; i < Form1.game.playerList[ player ].technos.Length; i ++ )
				if ( Form1.game.playerList[ player ].technos[ i ].researched )
				{
					tot ++;
				}

			return tot;
		}

		public static int technoAdvancement( byte player )
		{
			int tot = 0;

			for ( int i = 0; i < Form1.game.playerList[ player ].technos.Length; i ++ )
				if ( Form1.game.playerList[ player ].technos[ i ].researched )
				{
					tot += Statistics.technologies[ i ].cost;
				}
				else
				{
					tot += Form1.game.playerList[ player ].technos[ i ].pntDiscovered;
				}

			return tot;
		}

		/// <summary>
		/// return player positions, from higher (0) to lower
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static int[] technoPositions() 
		{ 
			int[] ta = new int[ Form1.game.playerList.Length ]; 

			for ( int i = 0; i < ta.Length; i ++ ) 
				if ( !Form1.game.playerList[ i ].dead )
					ta[ i ] = technoAdvancement( (byte)i );
				else
					ta[ i ] = -1;

			return returnOrderPlayer( ta );
		}
		
		/// <summary>
		/// 0st is better
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static int technoPosition( byte player ) 
		{ 
			int[] pos = technoPositions();
			for ( int i = 0; i < pos.Length; i ++ )
				if ( pos[ i ] == player )
					return i;

			return pos.Length - 1;
		}
		
		public static int[] returnOrderPlayer( int[] ta ) 
		{ 
			int[] pos = new int[ Form1.game.playerList.Length ]; 
 
			for ( int i = 0; i < pos.Length; i ++ ) 
				pos[ i ] = i; 
 
			bool mod = true; 
			while ( mod ) 
			{ 
				mod = false; 
				for ( int j = 1; j < pos.Length; j ++ ) 
					if ( ta[ pos[ j - 1 ] ] < ta[ pos[ j ] ] ) 
					{ 
						int buffer = pos[ j - 1 ]; 
						pos[ j - 1 ] = pos[ j ]; 
						pos[ j ] = buffer; 
						mod = true; 
					} 
			} 
 
			return pos; 
		} 
		
		public static int[] descOrder(int[] values)
		{
			long[] values1 = new long[ values.Length ];

			for ( int i = 0; i < values.Length; i++ )
				values1[ i ] = values[ i ];

			return descOrder( values1 );
		}

		public static int[] descOrder( long[] values ) 
		{ 
			int[] pos = new int[ values.Length ]; 

			for ( int i = 0; i < pos.Length; i ++ ) 
				pos[ i ] = i; 
 
			bool mod = true; 
			while ( mod ) 
			{ 
				mod = false; 
				for ( int j = 1; j < pos.Length; j ++ ) 
					if ( values[ pos[ j - 1 ] ] < values[ pos[ j ] ] ) 
					{ 
						int buffer = pos[ j - 1 ]; 
						pos[ j - 1 ] = pos[ j ]; 
						pos[ j ] = buffer; 
						mod = true; 
					} 
			} 

			return pos; 
		}

	/*	public static int[] ascOrder(int[] values)
		{
			long[] values1 = new long[ values.Length ];

			for ( int i = 0; i < values.Length; i++ )
				values1[ i ] = values[ i ];

			return ascOrder( values1 );
		}*/
		
		public static int[] ascOrder(long[] values)
		{
			int[] pos = new int[ values.Length ];

			for ( int i = 0; i < pos.Length; i++ )
				pos[ i ] = i;

			bool mod = true;

			while ( mod )
			{
				mod = false;
				for ( int j = 1; j < pos.Length; j++ )
					if ( values[ pos[ j - 1 ] ] > values[ pos[ j ] ] )
					{
						int buffer = pos[ j - 1 ];

						pos[ j - 1 ] = pos[ j ];
						pos[ j ] = buffer;
						mod = true;
					}
			}

			return pos; 
		}
		public static int[] ascOrder( int[] values ) 
		{ 
			long[] values1 = new long[ values.Length ];
			for ( int i = 0; i < values.Length; i ++ )
				values1[ i ] = values[ i ];

			return ascOrder( values1 );
			/*int[] pos = new int[ values.Length ]; 

			for ( int i = 0; i < pos.Length; i ++ ) 
				pos[ i ] = i; 
 
			bool mod = true; 
			while ( mod ) 
			{ 
				mod = false; 
				for ( int j = 1; j < pos.Length; j ++ ) 
					if ( values[ pos[ j - 1 ] ] > values[ pos[ j ] ] ) 
					{ 
						int buffer = pos[ j - 1 ]; 
						pos[ j - 1 ] = pos[ j ]; 
						pos[ j ] = buffer; 
						mod = true; 
					} 
			} 

			return pos; */
		}
		public static int turnsLeftToBuild(byte owner, int city)
		{
	//		getPFT getPFT1 = new getPFT();
	/*		int turnsLeft;

			if ( Form1.game.playerList[ owner ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.cityBuildType.unit )
				turnsLeft = ( Statistics.units[ Form1.game.playerList[ owner ].cityList[ city ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ owner ].cityList[ city ].construction.points ) / ( Form1.game.playerList[ owner ].cityList[ city ].production + 1 );
			else if ( Form1.game.playerList[ owner ].cityList[ city ].construction.list[ 0 ].type == (byte)enums.cityBuildType.building )// buildings
				turnsLeft = ( Statistics.buildings[ Form1.game.playerList[ owner ].cityList[ city ].construction.list[ 0 ].ind ].cost - Form1.game.playerList[ owner ].cityList[ city ].construction.points ) / ( Form1.game.playerList[ owner ].cityList[ city ].production + 1 );
			else 
				turnsLeft = -1;*/

			int turnsLeft = ( Form1.game.playerList[ owner ].cityList[ city ].construction.list[ 0 ].cost - Form1.game.playerList[ owner ].cityList[ city ].construction.points ) / ( Form1.game.playerList[ owner ].cityList[ city ].production + 1 );

			if ( Form1.game.playerList[ owner ].cityList[ city ].construction.list[ 0 ] is Stat.Wealth )
				return -1;
			else
				return turnsLeft;
		}

		public static int turnsLeftToBuild( byte owner, int city, byte type, int ind )
		{
			getPFT getPFT1 = new getPFT();
			int turnsLeft;
			if ( type == (byte)enums.cityBuildType.unit )
				turnsLeft = ( Statistics.units[ ind ].cost - Form1.game.playerList[ owner ].cityList[ city ].construction.points ) / Form1.game.playerList[ owner ].cityList[ city ].production + 1;
			else if ( type == (byte)enums.cityBuildType.building )// buildings
				turnsLeft = ( Statistics.buildings[ ind ].cost - Form1.game.playerList[ owner ].cityList[ city ].construction.points ) / Form1.game.playerList[ owner ].cityList[ city ].production + 1;
			else 
				turnsLeft = -1;

			return turnsLeft;
		}

		public static int turnsLeftToBuild( byte owner, int city, Stat.Construction c )//byte type, int ind )
		{
			getPFT getPFT1 = new getPFT();
			int turnsLeft;

			if ( c is Stat.Wealth )
				turnsLeft = -1;
			else 
				turnsLeft = ( c.cost - Form1.game.playerList[ owner ].cityList[ city ].construction.points ) / Form1.game.playerList[ owner ].cityList[ city ].production + 1;
			
			return turnsLeft;
		}

		public static int turnsLeftToBuild( byte owner, int city, int pos, Construction construction )
		{
			getPFT getPFT1 = new getPFT();
			int costLeft;
			int cp = construction.points;

			for ( int i = 0; i < pos && cp > 0; i ++ )
				cp -= construction.list[ i ].cost;
			/*	if ( construction.list[ i ].type == (byte)enums.cityBuildType.unit )
					cp -= Statistics.units[ construction.list[ i ].ind ].cost;
				else if ( construction.list[ i ].type == (byte)enums.cityBuildType.building )// buildings
					cp -= Statistics.buildings[ construction.list[ i ].ind ].cost;*/

			if ( construction.list[ pos ] is Stat.Wealth )
				costLeft = -1;
			else
				costLeft = construction.list[ pos ].cost;

		/*		costLeft = Statistics.units[ construction.list[ pos ].ind ].cost;
			else if ( construction.list[ pos ].type == (byte)enums.cityBuildType.building )// buildings
				costLeft = Statistics.buildings[ construction.list[ pos ].ind ].cost;
			else */

			if ( cp > 0 )
				costLeft -= cp;

			int turnsLeft = costLeft / Form1.game.playerList[ owner ].cityList[ city ].production + 1;
				
			if ( turnsLeft < 1 )
				turnsLeft = 1;

			return turnsLeft;
		}

		public static int turnsLeftToBuild( byte owner, int city, Construction construction, Stat.Construction c ) //byte type, int ind )
		{
			getPFT getPFT1 = new getPFT();
			int costLeft;
			int cp = construction.points;

			for ( int i = 0; i < construction.list.Length && construction.list[ i ] != null && cp > 0; i ++ )
			//	if ( construction.list[ i ].type == (byte)enums.cityBuildType.unit )
					cp -= construction.list[ i ].cost;
			/*	else if ( construction.list[ i ].type == (byte)enums.cityBuildType.building ) // buildings
					cp -= construction.list[ i ].cost;*/

			if ( c is Stat.Wealth )
				costLeft = -1;
			else
				costLeft = c.cost;

	/*		if ( type == (byte)enums.cityBuildType.unit )
				costLeft = Statistics.units[ ind ].cost;
			else if ( type == (byte)enums.cityBuildType.building ) // buildings
				costLeft = Statistics.buildings[ ind ].cost;
			else 
				costLeft = -1;		*/

			if ( cp > 0 )
				costLeft -= cp;

			int turnsLeft = costLeft / Form1.game.playerList[ owner ].cityList[ city ].production + 1;
				
			if ( turnsLeft < 1 )
				turnsLeft = 1;

			return turnsLeft;
		}

		public static string turnsLeftToBuildS( byte owner, int city, Stat.Construction c ) //byte type, int ind )
		{
			int tl = turnsLeftToBuild( owner, city, c ); //type, ind ); 
			
			if ( tl == -1 )
				return "";
			else if ( tl < 2 ) 
				return String.Format( language.getAString( language.order.turnLeft ), tl );
			else 
				return String.Format( language.getAString( language.order.turnsLeft ), tl );
		}

		public static string turnsLeftToBuildS( byte owner, int city, byte type, int ind )
		{
			int tl = turnsLeftToBuild( owner, city, type, ind ); 
			
			if ( tl == -1 )
				return "";
			else if ( tl < 2 ) 
				return String.Format( language.getAString( language.order.turnLeft ), tl );
			else 
				return String.Format( language.getAString( language.order.turnsLeft ), tl );
		}

		public static string turnsLeftToBuildS( byte owner, int city, int pos, Construction construction )
		{
			int tl = turnsLeftToBuild( owner, city, pos, construction );
			
			if ( tl == -1 )
				return "";
			else if ( tl < 2 ) 
				return String.Format( language.getAString( language.order.turnLeft ), tl );
			else 
				return String.Format( language.getAString( language.order.turnsLeft ), tl );
		}

		public static string turnsLeftToBuildS( byte owner, int city, Construction construction, Stat.Construction c ) //byte type, int ind )
		{
			int tl = turnsLeftToBuild( owner, city, construction, c ); // type, ind );
			
			if ( tl == -1 )
				return "";
			else if ( tl < 2 ) 
				return String.Format( language.getAString( language.order.turnLeft ), tl );
			else 
				return String.Format( language.getAString( language.order.turnsLeft ), tl );
		}

	/*	public static string turnsToBuildS( byte owner, int city, int pos, Construction construction, byte type, int ind )
		{
			int tl = turnsLeftToBuild( owner, city, pos, construction, type, ind );
			
			if ( tl == -1 )
				return "";
			else if ( tl < 2 ) 
				return String.Format( language.getAString( language.order.turnLeft ), tl );
			else 
				return String.Format( language.getAString( language.order.turnsLeft ), tl );
		}*/
	} 
} 
