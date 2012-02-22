using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for frontier.
	/// </summary>
	public class Frontier
	{
		Game game;

		public Frontier( Game game )
		{
			this.game = game;
		}

	#region setFrontiers
		public void setFrontiers()
		{
			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
					game.grid[ x, y ].territory = 0;

			sbyte[,] gridLvl = new sbyte[ game.width, game.height ];
			bool cityWithThisPop = true;
	//	//	Radius radius = new Radius();

			for ( int player = 0; player < game.playerList.Length; player ++ )
				for ( int city = 1; city <= game.playerList[ player ].cityNumber; city ++ )
					if ( game.playerList[ player ].cityList[ city ].state != (byte)enums.cityState.dead )
					{
						game.grid[ game.playerList[ player ].cityList[ city ].X, game.playerList[ player ].cityList[ city ].Y ].territory = (byte)( player + 1 );
						game.playerList[ player ].see[ game.playerList[ player ].cityList[ city ].X, game.playerList[ player ].cityList[ city ].Y ] = true;
						gridLvl[ game.playerList[ player ].cityList[ city ].X, game.playerList[ player ].cityList[ city ].Y ] = -1;
					}

			for ( sbyte order = 1; order < 100 && cityWithThisPop; order ++ )
			{
				cityWithThisPop = false;

				for ( byte player = 0; player < game.playerList.Length; player ++ )
					for ( int city = 1; city <= game.playerList[ player ].cityNumber; city ++ )
						if ( game.playerList[ player ].cityList[ city ].population / 3 + 1 >= order && game.playerList[ player ].cityList[ city ].state != (byte)enums.cityState.dead )
						{
							cityWithThisPop = true;

							Point[] sqr = game.radius.returnEmptySquare( game.playerList[ player ].cityList[ city ].X, game.playerList[ player ].cityList[ city ].Y, order );

							for ( int k = 0; k < sqr.Length; k ++ )
							{
								if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.sea && game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.coast )
								{
									if ( gridLvl[ sqr[ k ].X, sqr[ k ].Y ] == 0 )
									{
										game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory = (byte)( player + 1 );
										game.playerList[ player ].see[ sqr[ k ].X, sqr[ k ].Y ] = true;
										gridLvl[ sqr[ k ].X, sqr[ k ].Y ] = order;
									}
									else if ( gridLvl[ sqr[ k ].X, sqr[ k ].Y ] == order )
									{
										if ( game.playerList[ player ].culture > game.playerList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].culture )
										{
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory = (byte)( player + 1 );
											game.playerList[ player ].see[ sqr[ k ].X, sqr[ k ].Y ] = true;
											gridLvl[ sqr[ k ].X, sqr[ k ].Y ] = order;
										}
									}
								}
								else if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.coast )
								{
									if ( gridLvl[ sqr[ k ].X, sqr[ k ].Y ] == 0 )
									{
										Point[] sqr1 = game.radius.returnEmptySquare( sqr[ k ].X, sqr[ k ].Y, 1 );
										bool isNextToTerritory = false;
										for ( int i = 0; i < sqr1.Length; i ++ )
											if ( game.grid[ sqr1[ i ].X, sqr1[ i ].Y ].territory - 1 == player )
											{
												isNextToTerritory = true;
												break;
											}

										if ( isNextToTerritory )
										{
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory = (byte)( player + 1 );
											game.playerList[ player ].see[ sqr[ k ].X, sqr[ k ].Y ] = true;
											gridLvl[ sqr[ k ].X, sqr[ k ].Y ] = order;
										}
									}
									else if ( gridLvl[ sqr[ k ].X, sqr[ k ].Y ] == order )
									{
										Point[] sqr1 = game.radius.returnEmptySquare( sqr[ k ].X, sqr[ k ].Y, 1 );
										bool isNextToTerritory = false;
										for ( int i = 0; i < sqr1.Length; i ++ )
											if ( game.grid[ sqr1[ i ].X, sqr1[ i ].Y ].territory - 1 == player )
											{
												isNextToTerritory = true;
												break;
											}

										if ( isNextToTerritory && game.playerList[ player ].culture > game.playerList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].culture )
										{
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory = (byte)( player + 1 );
											game.playerList[ player ].see[ sqr[ k ].X, sqr[ k ].Y ] = true;
											gridLvl[ sqr[ k ].X, sqr[ k ].Y ] = order;
										}
									}
								}
							}
						}
				
			/*	for ( int x = 0; x < game.width; x ++ )
					for ( int y = 0; y < game.height; y ++ )
						if ( game.grid[ x, y ].territory == 0 && game.grid[ x, y ].type != (byte)enums.terrainType.sea )
						{
							byte[] territoryCase = new byte[ game.playerList.Length + 1 ];
							Point[] sqr = game.radius.returnEmptySquare( x, y, 1 );

							for ( int i = 0; i < sqr.Length; i ++ )
								territoryCase[ game.grid[ sqr[ i ].X, sqr[ i ].Y ].territory ] ++;
							
							for ( int i = 1; i < territoryCase.Length; i ++ )
								if ( territoryCase[ i ] > sqr.Length / 2 )
								{
									game.grid[ x, y ].territory = (byte)i;

									if ( i > 0 )
										game.playerList[ i - 1 ].see[ x, y ] = true;

									break;
								}
						}*/
			}
		}
		#endregion

	#region setFrontiersAround
		public void setFrontiersAround( Point pos )
		{
			int xo;
			for ( int x = pos.X - 10; x <= pos.X + 10; x ++ )
			{
				if ( x > game.width )
					xo = x - game.width;
				else if ( x < 0 )
					xo = x + game.width;
				else
					xo = x;
					
				for ( int y = pos.Y - 10 * 2; y <= pos.Y + 10 * 2; y ++ )
					if ( y > 0 && y < game.height )
					{
						game.grid[ xo, y ].territory = 0;
					}
			}

			sbyte[,] gridLvl = new sbyte[ game.width, game.height ];
			bool cityWithThisPop = true;
	//	//	Radius radius = new Radius();

			for ( int player = 0; player < game.playerList.Length; player ++ )
				for ( int city = 1; city <= game.playerList[ player ].cityNumber; city ++ )
					if ( game.playerList[ player ].cityList[ city ].state != (byte)enums.cityState.dead )
					{
						game.grid[ game.playerList[ player ].cityList[ city ].X, game.playerList[ player ].cityList[ city ].Y ].territory = (byte)( player + 1 );
						gridLvl[ game.playerList[ player ].cityList[ city ].X, game.playerList[ player ].cityList[ city ].Y ] = -1;
					}

			for ( sbyte order = 1; order < 100 && cityWithThisPop; order ++ )
			{
				cityWithThisPop = false;

				for ( byte player = 0; player < game.playerList.Length; player ++ )
					for ( int city = 1; city <= game.playerList[ player ].cityNumber; city ++ )
						if ( 
							game.playerList[ player ].cityList[ city ].population / 3 + 1 >= order && 
							game.playerList[ player ].cityList[ city ].state != (byte)enums.cityState.dead 
							)
						{
							if ( 
								game.playerList[ player ].cityList[ city ].X > pos.X - 20 &&
								game.playerList[ player ].cityList[ city ].X <= pos.X + 20 &&
								game.playerList[ player ].cityList[ city ].Y > pos.Y - 20 * 2 &&
								game.playerList[ player ].cityList[ city ].Y > pos.Y - 20 * 2
								)
							{
								cityWithThisPop = true;

								Point[] sqr = game.radius.returnEmptySquare( game.playerList[ player ].cityList[ city ].X, game.playerList[ player ].cityList[ city ].Y, order );

								for ( int k = 0; k < sqr.Length; k ++ )
								{
									if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.sea && game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.coast )
									{
										if ( gridLvl[ sqr[ k ].X, sqr[ k ].Y ] == 0 )
										{
											game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory = (byte)( player + 1 );
											gridLvl[ sqr[ k ].X, sqr[ k ].Y ] = order;
										}
										else if ( gridLvl[ sqr[ k ].X, sqr[ k ].Y ] == order )
										{
											if ( game.playerList[ player ].culture > game.playerList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].culture )
											{
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory = (byte)( player + 1 );
												gridLvl[ sqr[ k ].X, sqr[ k ].Y ] = order;
											}
										}
									}
									else if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].type == (byte)enums.terrainType.coast )
									{
										if ( gridLvl[ sqr[ k ].X, sqr[ k ].Y ] == 0 )
										{
											Point[] sqr1 = game.radius.returnEmptySquare( sqr[ k ].X, sqr[ k ].Y, 1 );
											bool isNextToTerritory = false;
											for ( int i = 0; i < sqr1.Length; i ++ )
												if ( game.grid[ sqr1[ i ].X, sqr1[ i ].Y ].territory - 1 == player )
												{
													isNextToTerritory = true;
													break;
												}

											if ( isNextToTerritory )
											{
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory = (byte)( player + 1 );
												gridLvl[ sqr[ k ].X, sqr[ k ].Y ] = order;
											}
										}
										else if ( gridLvl[ sqr[ k ].X, sqr[ k ].Y ] == order )
										{
											Point[] sqr1 = game.radius.returnEmptySquare( sqr[ k ].X, sqr[ k ].Y, 1 );
											bool isNextToTerritory = false;
											for ( int i = 0; i < sqr1.Length; i ++ )
												if ( game.grid[ sqr1[ i ].X, sqr1[ i ].Y ].territory - 1 == player )
												{
													isNextToTerritory = true;
													break;
												}

											if ( isNextToTerritory && game.playerList[ player ].culture > game.playerList[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory - 1 ].culture )
											{
												game.grid[ sqr[ k ].X, sqr[ k ].Y ].territory = (byte)( player + 1 );
												gridLvl[ sqr[ k ].X, sqr[ k ].Y ] = order;
											}
										}
									}
								}
							}
						}
			}
		}
		#endregion


	}
}
