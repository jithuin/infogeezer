using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for getPFT.
	/// </summary>
	public class getPFT
	{
		public getPFT()
		{
			//
			// TODO: Add constructor logic here
			//
		}

#region get prod

		public static byte getCaseProd( int x1, int y1 )
		{
			if ( x1 < 0)
				x1 += Form1.game.width;
			else if ( x1 >= Form1.game.width )
				x1 -= Form1.game.width;

			if ( y1 >= 0 && y1 < Form1.game.height)
			{
				byte tot = 0;
			
				tot += Statistics.terrains[ (byte)Form1.game.grid[ x1, y1 ].type ].production; 
				
				if ( Form1.game.grid[ x1, y1 ].civicImprovement == (byte)Form1.civicImprovementChoice.mine )
					tot += Statistics.terrains[ (byte)Form1.game.grid[ x1, y1 ].type ].mineBonus;

				if ( Form1.game.grid[ x1, y1 ].resources >= 100 )
					tot += Statistics.resources[ Form1.game.grid[ x1, y1 ].resources - 100 ].prodBonus;

				if ( Form1.game.grid[ x1, y1 ].city > 0 )
					tot += 1;

				return tot;
			}
			else
				return 100; // testing purposes
		}

		#endregion

#region get food

		public static byte getCaseFood( int x1, int y1 )
		{
			if ( x1 < 0)
			{
				x1 += Form1.game.width;
			}
			else if ( x1 >= Form1.game.width )
			{
				x1 -= Form1.game.width;
			}

			if ( y1 >= 0 && y1 < Form1.game.height)
			{
				byte tot = 0;

				tot += Statistics.terrains[ (byte)Form1.game.grid[ x1, y1 ].type ].food;

				if ( Form1.game.grid[ x1, y1 ].civicImprovement == (byte)Form1.civicImprovementChoice.irrigation )
					tot += Statistics.terrains[ (byte)Form1.game.grid[ x1, y1 ].type ].irrigationBonus;

				if ( Form1.game.grid[ x1, y1 ].river )
					tot += Statistics.terrains[ (byte)Form1.game.grid[ x1, y1 ].type ].riverFoodBonus;

				if ( Form1.game.grid[ x1, y1 ].resources >= 100 )
					tot += Statistics.resources[ Form1.game.grid[ x1, y1 ].resources - 100 ].foodBonus;

				return tot;
			}
			else
				return 0; //100; // testing purposes
		}

		#endregion

#region get trade

		public static byte getCaseTrade( int x1, int y1 )
		{
			if ( x1 < 0)
			{
				x1 += Form1.game.width;
			}
			else if ( x1 >= Form1.game.width )
			{
				x1 -= Form1.game.width;
			}

			if ( y1 >= 0 && y1 < Form1.game.height)
			{
				byte tot = 0;

				tot += Statistics.terrains[ (byte)Form1.game.grid[ x1, y1 ].type ].trade;

				if ( Form1.game.grid[ x1, y1 ].roadLevel == 1 )
					tot += Statistics.terrains[ (byte)Form1.game.grid[ x1, y1 ].type ].roadBonus;
				else if ( Form1.game.grid[ x1, y1 ].roadLevel == 2 )
					tot += (byte)(Statistics.terrains[ (byte)Form1.game.grid[ x1, y1 ].type ].roadBonus * 2);

				if ( Form1.game.grid[ x1, y1 ].city > 0 )
					tot += (byte)(Form1.game.playerList[ Form1.game.grid[ x1, y1 ].territory - 1 ].cityList[ Form1.game.grid[ x1, y1 ].city ].population / 2 + Form1.game.playerList[ Form1.game.grid[ x1, y1 ].territory - 1 ].cityList[ Form1.game.grid[ x1, y1 ].city ].nonLabor.totalOfOneType( (byte)peopleNonLabor.types.trader ));

				if ( Form1.game.grid[ x1, y1 ].river )
					tot += Statistics.terrains[ (byte)Form1.game.grid[ x1, y1 ].type ].riverTradeBonus;

				if ( Form1.game.grid[ x1, y1 ].resources >= 100 )
					tot += Statistics.resources[ Form1.game.grid[ x1, y1 ].resources - 100 ].tradeBonus;

				return tot;
			}
			else
				return 100; // testing purposes
		}

		#endregion

#region get city science
		public int getCityScience( byte owner, int city )
		{
			return 4;
		}
		#endregion

#region get nation science
		public int getNationScience( byte owner )
		{
			int totSci = 0;
			for ( int i = 1; i <= Form1.game.playerList[ owner ].cityNumber; i ++ ) 
			{
				totSci += getCityScience( owner, i );
			}
			return totSci;
		}
		#endregion

#region get nation trade
		/*public int getNationTrade( byte player )
		{
			int totTra = 1;
			for ( int i = 1; i <= Form1.game.playerList[ player ].cityNumber; i ++ ) 
				if ( Form1.game.playerList[ player ].cityList[ i ].state != (byte)enums.cityState.dead )
				{
					totTra += getCityTrade( player, i );
				}

			return totTra;
		}*/
		#endregion

#region city happiness
		public int getCityHapiness( byte owner, int city )
		{
			return 4;
		}
		#endregion

#region find best case
		public Point findBestCase( byte owner, int city )
		{
			int x1 = Form1.game.playerList[ owner ].cityList[ city ].X;
			int y1 = Form1.game.playerList[ owner ].cityList[ city ].Y;

			//byte j = 0;
			
	//	//	Radius radius = new Radius();
			Point[] pntCovered = Form1.game.radius.returnCityRadius(
				Form1.game.playerList[ owner ].cityList[ city ].X, 
				Form1.game.playerList[ owner ].cityList[ city ].Y 
				);

			double[] caseValue = new double[ pntCovered .Length ];
			
			for ( int i = 0; i < pntCovered .Length ; i ++ )
			{
				if( Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].laborCity == 0 && Form1.game.grid[ pntCovered[ i ].X,  pntCovered[ i ].Y ].city == 0 )
				{
					caseValue[ i ] = 
						Form1.game.playerList[ owner ].preferences.laborProd * getCaseProd( pntCovered[ i ].X,  pntCovered[ i ].Y ) + 
						Form1.game.playerList[ owner ].preferences.laborFood * getCaseFood( pntCovered[ i ].X,  pntCovered[ i ].Y ) + 
						Form1.game.playerList[ owner ].preferences.laborTrade * getCaseTrade( pntCovered[ i ].X,  pntCovered[ i ].Y );

					//Radius radius = new Radius();
					if ( Form1.game.radius.isNextToIrrigation( pntCovered[ i ].X,  pntCovered[ i ].Y ) )
					{
						caseValue[ i ] ++;
					}
				}
			}

			int caseChoose = 0;

			for ( int i = 1; i < pntCovered .Length; i++ )
			{
				if ( caseValue[ caseChoose ] < caseValue[ i ] )
					caseChoose = i ;
			}

			if ( pntCovered[ caseChoose ].Y > 0 )
				return pntCovered[ caseChoose ];
			else
				return new Point( x1 + 1, y1 + 1);

		}
#endregion

	}
}
