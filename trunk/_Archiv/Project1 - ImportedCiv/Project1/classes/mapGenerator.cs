using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for mapGenerator.
	/// </summary>
	public class mapGenerator
	{
		Game game;

		public mapGenerator( Game game )
		{
			this.game = game;
		}

#region generate map
		public bool generateMap( int Width, int Height, byte wantedWaterPerc, byte contSize, byte age )
		{

			Random R = new Random();
	//	//	Radius radius = new Radius();
			bool generationSucessful = false;
			
			int waterCase = 0;

			showProgress.lblInfo = "Preparing grids...";

		#region grids definitions

				byte[,] grid0 = new byte[ Width, Height ]; 
				byte[,] grid1 = new byte[ Width, Height ]; 
				byte[,] grid2 = new byte[ Width, Height ]; 
				byte[,] grid3 = new byte[ Width, Height ]; 
				
				byte[,] grid0Mount = new byte[ Width, Height ]; 
				byte[,] grid1Mount = new byte[ Width, Height ]; 
				byte[,] grid2Mount = new byte[ Width, Height ]; 
				
				byte[,] grid0Desert = new byte[ Width, Height ]; 
				byte[,] grid1Desert = new byte[ Width, Height ]; 
				byte[,] grid2Desert = new byte[ Width, Height ]; 
				
				byte[,] grid0Jungle = new byte[ Width, Height ]; 
				byte[,] grid1Jungle = new byte[ Width, Height ]; 
				byte[,] grid2Jungle = new byte[ Width, Height ]; 
				
				byte[,] grid0Swamp = new byte[ Width, Height ]; 
				byte[,] grid1Swamp = new byte[ Width, Height ]; 
				byte[,] grid2Swamp = new byte[ Width, Height ]; 

				#endregion

		//	int radius4th = 3, nece4th = 15;
			int radius4th, /*nece4th,*/ perc4th;
			switch ( contSize )
			{
				case 0:
					radius4th = 3;
				//	nece4th = 15;
					perc4th = 31;
					break;

				default:
					radius4th = 4;
				//	nece4th = 22; // 25
					perc4th = 27;
					break;

				case 2:
					radius4th = 5;
				//	nece4th = 28; // 35
					perc4th = 23;
					break;
			}

			radius4th += Width / ( 40 ) - 1;

			for ( int k = 0; !generationSucessful; k ++ )
			{
				if ( k > 0 )
					showProgress.lblInfo = "Map generation common exception";

				int progress = 15;

				showProgress.progressBar = 15;
				showProgress.lblInfo = "1st pass...";

			#region 1st pass - generating spikes
				// for ( int i = 0; i < Width * Height * ( 0.31 + 0.001 * wantedWaterPerc ); ++i ) 
				for ( int i = 0; i < Width * Height * 0.38; ++i ) 
				{ 
					int x = R.Next( Width ),
						y = R.Next( Height ); 
					grid0[ x, y ] = 1; 
				} 

				showProgress.incProgressBar();

				for ( int i = 0; i < Width * Height * 0.23; ++i) 
				{ 
					int x = R.Next( Width ), 
						y = R.Next( Height ); 
					grid0Mount[ x, y ] = 1; 
				} 

				showProgress.incProgressBar();

				for ( int i = 0; i < Width * Height * 0.15 / 2; ++i) 
				{ 
					int x = R.Next( Width ), 
						y = (int)( Height * .35 + R.Next( (int)Math.Round( Height *.3 ) ) ); 
					grid0Desert[ x, y ] = 1; 
				}

				showProgress.incProgressBar();

				for ( int i = 0; i < Width * Height * 0.15; ++i) 
				{ 
					int x = R.Next( Width ), 
						y = (int)( Height * .3 + R.Next( (int)( Height *.4 ) ) ); 
					grid0Swamp[ x, y ] = 1; 
				}

				showProgress.incProgressBar();

				for ( int i = 0; i < Width * Height * 0.15; ++i) 
				{ 
					int x = R.Next( Width ),
						y = (int)( Height * .25 + R.Next( (int)Math.Round( Height *.5 ) ) ); 
					grid0Jungle[ x, y ] = 1; 
				}
				#endregion

				showProgress.progressBar = 20;
				showProgress.lblInfo = "2nd pass...";

				progress = 20;

			#region 2nd pass - radius = 1
				for( int x = 0; x < Width; ++x ) 
				{
					for( int y = 0; y < Height; ++y ) 
					{
						int sum = 0,
							sumMount = 0,
							sumDesert = 0,
							sumJungle = 0,
							sumSwamp = 0;
						Point[] sqr = game.radius.returnSquare( x, y, 1);

						for ( int i = 0; i < sqr.Length; i ++ )
						{
							if ( grid0[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
								sum ++;

							if ( grid0Mount[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
								sumMount ++;

							if ( grid0Desert[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
								sumDesert ++;

							if ( grid0Jungle[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
								sumJungle ++;

							if ( grid0Swamp[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
								sumSwamp ++;
						}

						if ( sum > 3 )
							grid1[ x, y ] = 1;

						if ( sumMount > 4 )
							grid1Mount[ x, y ] = 1; // mountain
						else if ( sumMount > 3 )
							grid1Mount[ x, y ] = 2; // hill

						if ( sumDesert > 3 )
							grid1Desert[ x, y ] = 1;

						if ( sumJungle > 3 )
							grid1Jungle[ x, y ] = 1;

						if ( sumSwamp > 4 )
							grid1Swamp[ x, y ] = 1;

					}

					showProgress.progressBar = progress + x * 5 / Width;
				}
				#endregion

				showProgress.progressBar = 25;
				showProgress.lblInfo = "3rd pass...";

				progress = 25;

			#region 3rd pass - radius = 2
				for( int x = 0; x < Width; ++x ) 
				{
					for( int y = 0; y < Height; ++y ) 
					{
						int sum = 0;
						int sumMount = 0;
						int sumDesert = 0;
						int sumJungle = 0;
						int sumSwamp = 0;
						Point[] sqr = game.radius.returnSquare( x, y, 2);

						for ( int i = 0; i < sqr.Length; i ++ )
						{
							if ( grid1[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
								sum ++;

							if ( grid1Mount[ sqr[ i ].X, sqr[ i ].Y ] > 0 )
								sumMount ++;

							if ( grid1Desert[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
								sumDesert ++;

							if ( grid1Jungle[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
								sumJungle ++;

							if ( grid1Swamp[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
								sumSwamp ++;
						}

						if ( sum > 8 )
							grid2[ x, y ] = 1;

						if ( sumMount > 6 )
							grid2Mount[ x, y ] = 1; // mountain
						else if ( sumMount > 5 )
							grid2Mount[ x, y ] = 2; // hill

						if ( sumDesert > 6 )
							grid2Desert[ x, y ] = 1;

						if ( sumJungle > 6 )
							grid2Jungle[ x, y ] = 1;

						if ( sumSwamp > 8 )
							grid2Swamp[ x, y ] = 1;
					}

					showProgress.progressBar = progress + x * 5 / Width;
				}
				#endregion

				showProgress.progressBar = 30;
				showProgress.lblInfo = "4th pass...";
				
				progress = 30;
				
			#region 4th pass - radius = 3 - applying water or land

				switch ( age )
				{

			#region case 0 jeune
					default:
						for( int x = 0; x < Width; ++ x ) 
						{
							for( int y = 0; y < Height; ++ y ) 
							{
								int sum = 0;
								Point[] sqr = game.radius.returnSquare( x, y, radius4th);

								for ( int i = 0; i < sqr.Length; i ++ )
									if ( grid2[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
										sum ++;

								if ( y == 0 || y == Height - 1 )
									grid3[ x, y ] = 1;
								else if ( ( y < Height * .15 || y > Height * .85 ))
								{
									if ( sum > perc4th * sqr.Length * 4 / 3 / 100 ) //20
										grid3[ x, y ] = 1;
								}
								else if ( sum > perc4th * sqr.Length / 100 ) // 15 - 70 // 12 - 60 // 8 - 80
								{
									grid3[ x, y ] = 1;
								}
								else
								{
									grid3[ x, y ] = 0;
								}
							}

							showProgress.progressBar = progress + x * 5 / Width;
						}
						break;
						#endregion

			#region case 1 moyen
					case 1:
						for( int x = 0; x < Width; ++ x ) 
						{
							for( int y = 0; y < Height; ++ y ) 
							{
								int sum = 0;
								Point[] sqr = game.radius.returnSquare( x, y, radius4th);

								for ( int i = 0; i < sqr.Length; i ++ )
									if ( grid2[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
										sum ++;

								if ( y == 0 || y == Height - 1 )
									grid3[ x, y ] = 1;
								else if ( ( y < Height * .15 || y > Height * .85 ))
								{
									if ( sum > perc4th * sqr.Length * 4 / 3 / 100 ) //20
										grid3[ x, y ] = 1;
								}
								else if ( sum > perc4th * sqr.Length / 100 ) // 15 - 70 // 12 - 60 // 8 - 80
									grid3[ x, y ] = 1;
								else
									grid3[ x, y ] = 0;
							}

							showProgress.progressBar = progress + x * 5 / Width;
						}
						break;
						#endregion

			#region case 2 vieux
					case 2:
						for( int x = 0; x < Width; ++ x ) 
						{
							for( int y = 0; y < Height; ++ y ) 
							{
								int sum = 0;
								Point[] sqr = game.radius.returnSquare( x, y, radius4th );

								for ( int i = 0; i < sqr.Length; i ++ )
									if ( grid2[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
										sum ++;

								if ( y == 0 || y == Height - 1 )
								{
									grid3[ x, y ] = 1;
								}
								else if ( ( y < Height * .15 || y > Height * .85 ))
								{
									if ( sum > perc4th * sqr.Length * 4 / 3 / 100 ) //20
									{
										grid3[ x, y ] = 1;
									}
								}
								else if ( sum > perc4th * sqr.Length / 100 ) // 15 - 70 // 12 - 60 // 8 - 80
								{
									grid3[ x, y ] = 1;
								}
								else
								{
									grid3[ x, y ] = 0;
								}
							}

							showProgress.progressBar = progress + x * 5 / Width;
						}
						break;
						#endregion

				}

				#region old
				/*switch ( age )
				{

						#region case 0 jeune
					default:
						for( int x = 0; x < Width; ++ x ) 
							for( int y = 0; y < Height; ++ y ) 
							{
								int sum = 0;
								Point[] sqr = game.radius.returnSquare( x, y, radius4th);

								for ( int i = 0; i < sqr.Length; i ++ )
									if ( grid2[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
										sum ++;

								if ( y == 0 || y == Height - 1 )
									grid3[ x, y ] = 1;
								else if ( ( y < Height * .15 || y > Height * .85 ))
								{
									if ( sum > nece4th * 5/4 ) //20
										grid3[ x, y ] = 1;
								}
								else if ( sum > nece4th ) // 15 - 70 // 12 - 60 // 8 - 80
								{
									grid3[ x, y ] = 1;
								}
								else
								{
									grid3[ x, y ] = 0;
								}
							}
						break;
						#endregion

						#region case 1 moyen
					case 1:
						for( int x = 0; x < Width; ++ x ) 
							for( int y = 0; y < Height; ++ y ) 
							{
								int sum = 0;
								Point[] sqr = game.radius.returnSquare( x, y, radius4th);

								for ( int i = 0; i < sqr.Length; i ++ )
								{
									if ( grid2[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
										sum ++;
								}

								if ( y == 0 || y == Height - 1 )
								{
									grid3[ x, y ] = 1;
								}
								else if ( ( y < Height * .15 || y > Height * .85 ))
								{
									if ( sum > nece4th * 5/4 ) //20
										grid3[ x, y ] = 1;
								}
								else if ( sum > nece4th ) // 15 - 70 // 12 - 60 // 8 - 80
								{
									grid3[ x, y ] = 1;
								}
								else
								{
									grid3[ x, y ] = 0;
								}
							}
						break;
						#endregion

						#region case 2 vieux
					case 2:
						for( int x = 0; x < Width; ++ x ) 
							for( int y = 0; y < Height; ++ y ) 
							{
								int sum = 0;
								Point[] sqr = game.radius.returnSquare( x, y, radius4th);

								for ( int i = 0; i < sqr.Length; i ++ )
								{
									if ( grid2[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
										sum ++;
								}

								if ( y == 0 || y == Height - 1 )
								{
									grid3[ x, y ] = 1;
								}
								else if ( ( y < Height * .15 || y > Height * .85 ))
								{
									if ( sum > nece4th * 5/4 ) //20
									{
										grid3[ x, y ] = 1;
									}
								}
								else if ( sum > nece4th ) // 15 - 70 // 12 - 60 // 8 - 80
								{
									grid3[ x, y ] = 1;
								}
								else
								{
									grid3[ x, y ] = 0;
								}
							}
						break;
						#endregion
				}*/
				#endregion

			#endregion

				showProgress.progressBar = 35;
				showProgress.lblInfo = "Adjusting water percentage...";

				progress = 35;

			//	int watchDog = 0;

			#region count water
				for( int x = 0; x < Width; ++ x ) 
					for( int y = 0; y < Height; ++ y ) 
						if ( grid3[ x, y ] == 0 )
							waterCase ++;

				int perc = waterCase * 100 / ( Width * Height );

			#endregion

			#region just enought water

				if ( perc == wantedWaterPerc ) // if ( perc < wantedWaterPerc + 2 && perc > wantedWaterPerc - 2 )
				{
					generationSucessful = true;
				}
			#endregion

			#region too much water
				else if ( perc < 95 && perc > wantedWaterPerc ) // 15
				{ // 
					int casesToFill = waterCase - wantedWaterPerc * Width * Height / 100,
						ori = casesToFill, 
						watch = 0;

					while( casesToFill > 0 )
					{
						Point change = new Point( R.Next( Width ), R.Next( Height - 4 ) + 2 );

						if ( grid3[ change.X, change.Y ] == 0 )
						{
							Point[] sqr = game.radius.returnEmptySquare( change, 1 );
							int tot = 0;

							for ( int sqrPos = 0; sqrPos < sqr.Length; sqrPos ++ )
								if ( grid3[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ] == 1 )
									tot ++;

							if ( tot > 2 ) // 3
							{ 
								grid3[ change.X, change.Y ] = 1;
								casesToFill --; 

								showProgress.progressBar = progress + ( ori - casesToFill ) * 10 / ori;
							} 
						} 

						watch ++;

						if ( watch > Width * Height * 100 )
							return false;	
					} 

					generationSucessful = true;
				}
			#endregion

			#region not enought water
				else if ( perc < wantedWaterPerc && perc > 45 ) // 15
				{ // 
					int casesToWater = wantedWaterPerc * Width * Height / 100 - waterCase;
					int ori = casesToWater, watch = 0;

					while ( casesToWater > 0 )
					{
						Point change = new Point( R.Next( Width ), R.Next( Height - 4 ) + 2 );

						if ( grid3[change.X, change.Y] == 1 )
						{
							Point[] sqr = game.radius.returnEmptySquare( change, 1 );
							int tot = 0;

							for ( int sqrPos = 0; sqrPos < sqr.Length; sqrPos++ )
								if ( grid3[sqr[sqrPos].X, sqr[sqrPos].Y] == 0 ) //if ( Statistics.terrains[ grid3[ sqr[ sqrPos ].X, sqr[ sqrPos ].Y ] ].ew == 0 )
									tot++;

							sqr = game.radius.returnEmptySquare( change, 4 );
							int totLarge = 0;

							for ( int sqrPos = 0; sqrPos < sqr.Length; sqrPos++ )
								if ( grid3[sqr[sqrPos].X, sqr[sqrPos].Y] == 1 )
									totLarge++;

							/*	switch ( contSize )
								{
									case 0: // small conts
										if ( tot > 2 ) // && totLarge > sqr.Length * 50 / 100 ) 
										{ 
											grid3[ change.X, change.Y ] = 0; // (byte)enums.terrainType.coast; 
											casesToWater --; 

											showProgress.progressBar = progress + ( ori - casesToWater ) * 10 / ori;
										} 
										break;
									
									case 1:	*/
							if ( tot > 3 )
							{
								grid3[change.X, change.Y] = 0; // (byte)enums.terrainType.coast; 
								casesToWater--;

								showProgress.progressBar = progress + (ori - casesToWater) * 5 / ori;
							}
							/*	break;
									
							case 2: // large conts
								if ( tot > 4 ) // && totLarge < sqr.Length * 50 / 100 ) 
								{ 
									grid3[ change.X, change.Y ] = 0; // (byte)enums.terrainType.coast; 
									casesToWater --; 

									showProgress.progressBar = progress + ( ori - casesToWater ) * 5 / ori;
								} 
								break;
						}*/

						}

						watch++;

						if ( watch > Width * Height * 100 )
							return false;
					}

					generationSucessful = true;
				}
			#endregion
				else
				{
					continue;
			//		int bob12 = 4;
				}

			showProgress.progressBar = 45;
			showProgress.lblInfo = "Perfecting map...";

			#region applying cases type

			switch ( age )
			{

			#region case 0 jeune
				default:
					for( int x = 0; x < Width; ++ x ) 
						for( int y = 0; y < Height; ++ y ) 
							if ( grid3[ x, y ] == 1 )
							{
								int sum = 0;
								Point[] sqr = game.radius.returnSquare( x, y, radius4th);

								for ( int i = 0; i < sqr.Length; i ++ )
									if ( grid2[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
										sum ++;

								if ( y == 0 || y == Height - 1 )
									grid3[ x, y ] = (byte)enums.terrainType.glacier;
								else
								if ( ( y < Height * .15 || y > Height * .85 ))
								{
										grid3[ x, y ] = (byte)enums.terrainType.plain;

										if ( grid2Mount[ x, y ] == 1 )
											grid3[ x, y ] = (byte)enums.terrainType.mountain;
										else if ( grid2Mount[ x, y ] == 2 )
											grid3[ x, y ] = (byte)enums.terrainType.hill;
										else
										{
											int rdm = R.Next( 10 );

											if ( rdm < 3 )
												grid3[ x, y ] = (byte)enums.terrainType.prairie;
											else if ( rdm < 8 )
												grid3[ x, y ] = (byte)enums.terrainType.forest;
											else
												grid3[ x, y ] = (byte)enums.terrainType.plain;
										}
								}
								else  // 15 - 70 // 12 - 60 // 8 - 80
								{
									grid3[ x, y ] = (byte)enums.terrainType.plain;

									if ( grid2Mount[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.mountain;
									}
									else if ( grid2Mount[ x, y ] == 2 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.hill;
									}
									else if ( grid2Desert[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.desert;
									}
									else if ( grid2Jungle[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.jungle;
									}
									else if ( grid2Swamp[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.swamp;
									}
									else
									{
										int rdm = R.Next( 6 );

										if ( rdm < 2 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.prairie;
										}
										else if ( rdm < 3 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.forest;
										}
										else
										{
											grid3[ x, y ] = (byte)enums.terrainType.plain;
										}
									}
								}
							}
					break;
					#endregion

			#region case 1 moyen
				case 1:
					for( int x = 0; x < Width; ++ x ) 
						for( int y = 0; y < Height; ++ y ) 
							if ( grid3[ x, y ] == 1 )
							{
								int sum = 0;
								Point[] sqr = game.radius.returnSquare( x, y, radius4th);

								for ( int i = 0; i < sqr.Length; i ++ )
									if ( grid2[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
										sum ++;

								if ( y == 0 || y == Height - 1 )
								{
									grid3[ x, y ] = (byte)enums.terrainType.glacier;
								}
								else if ( ( y < Height * .15 || y > Height * .85 ))
								{
										grid3[ x, y ] = (byte)enums.terrainType.plain;

										if ( grid1Mount[ x, y ] == 1 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.mountain;
										}
										else if ( grid1Mount[ x, y ] == 2 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.hill;
										}
										else
										{
											int rdm = R.Next( 10 );

											if ( rdm < 3 )
											{
												grid3[ x, y ] = (byte)enums.terrainType.prairie;
											}
											else if ( rdm < 8 )
											{
												grid3[ x, y ] = (byte)enums.terrainType.forest;
											}
											else
											{
												grid3[ x, y ] = (byte)enums.terrainType.plain;
											}
										}
								}
								else  // 15 - 70 // 12 - 60 // 8 - 80
								{
									grid3[ x, y ] = (byte)enums.terrainType.plain;

									if ( grid1Mount[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.mountain;
									}
									else if ( grid1Mount[ x, y ] == 2 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.hill;
									}
									else if ( grid2Desert[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.desert;
									}
									else if ( grid1Jungle[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.jungle;
									}
									else if ( grid1Swamp[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.swamp;
									}
									else
									{
										int rdm = R.Next( 6 );

										if ( rdm < 2 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.prairie;
										}
										else if ( rdm < 3 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.forest;
										}
										else
										{
											grid3[ x, y ] = (byte)enums.terrainType.plain;
										}
									}
								}
							}
					break;
					#endregion

			#region case 2 vieux
				case 2:
					for( int x = 0; x < Width; ++ x ) 
						for( int y = 0; y < Height; ++ y ) 
							if ( grid3[ x, y ] == 1 )
							{
								int sum = 0;
								Point[] sqr = game.radius.returnSquare( x, y, radius4th);

								for ( int i = 0; i < sqr.Length; i ++ )
									if ( grid2[ sqr[ i ].X, sqr[ i ].Y ] == 1 )
										sum ++;

								if ( y == 0 || y == Height - 1 )
								{
									grid3[ x, y ] = (byte)enums.terrainType.glacier;
								}
								else if ( ( y < Height * .15 || y > Height * .85 ))
								{
										grid3[ x, y ] = (byte)enums.terrainType.plain;

										if ( grid1Mount[ x, y ] == 1 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.mountain;
										}
										else if ( grid1Mount[ x, y ] == 2 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.hill;
										}
										else
										{
											int rdm = R.Next( 10 );
											if ( rdm < 3 )
											{
												grid3[ x, y ] = (byte)enums.terrainType.prairie;
											}
											else if ( rdm < 8 )
											{
												grid3[ x, y ] = (byte)enums.terrainType.forest;
											}
											else
											{
												grid3[ x, y ] = (byte)enums.terrainType.plain;
											}
										}
								}
								else // 15 - 70 // 12 - 60 // 8 - 80
								{
									grid3[ x, y ] = (byte)enums.terrainType.plain;

									if ( grid1Mount[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.mountain;
									}
									else if ( grid1Mount[ x, y ] == 2 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.hill;
									}
									else if ( grid2Desert[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.desert;
									}
									else if ( grid1Jungle[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.jungle;
									}
									else if ( grid1Swamp[ x, y ] == 1 )
									{
										grid3[ x, y ] = (byte)enums.terrainType.swamp;
									}
									else
									{
										int rdm = R.Next( 6 );

										if ( rdm < 2 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.prairie;
										}
										else if ( rdm < 3 )
										{
											grid3[ x, y ] = (byte)enums.terrainType.forest;
										}
										else
										{
											grid3[ x, y ] = (byte)enums.terrainType.plain;
										}
									}
								}
							}
					break;
					#endregion

			}

		#endregion
				
				showProgress.progressBar ++;

			#region 5th pass - verifications
			for( int x = 0; x < Width; ++ x ) 
				for( int y = 0; y < Height; ++ y ) 
				{

		#region set coats
					if ( grid3[ x, y ] == 0 )
					{ // coasts
						Point[] sqr = game.radius.returnEmptySquare( x, y, 1);

						for ( int i = 0; i < sqr.Length; i ++ )
							if ( 
								grid3[ sqr[ i ].X, sqr[ i ].Y ] != (byte)enums.terrainType.coast && 
								grid3[ sqr[ i ].X, sqr[ i ].Y ] != (byte)enums.terrainType.sea 
								)
							{
								grid3[ x, y ] = (byte)enums.terrainType.coast;
								break;
							}
					}
						#endregion

		#region desert next to water
					else if ( grid3[ x, y ] == (byte)enums.terrainType.desert )
					{ // 
						Point[] sqr = game.radius.returnEmptySquare( x, y, 1);

						for ( int i = 0; i < sqr.Length; i ++ )
						{
							if ( 
								grid3[ sqr[ i ].X, sqr[ i ].Y ] == (byte)enums.terrainType.coast ||
								grid3[ sqr[ i ].X, sqr[ i ].Y ] == (byte)enums.terrainType.sea )
							{
								grid3[ x, y ] = (byte)enums.terrainType.plain;
								break;
							}
						}
					}
					#endregion

				}

				#endregion
				
				showProgress.progressBar ++;
			
			#region  // set far coasts

			int pntFound = waterCase / 25,
				tries = waterCase;
			while ( tries > 0 && pntFound > 0 )
			{
				int x = R.Next( Width ),
					y = R.Next( Height - 2 ) + 1;

				if ( grid3[ x, y ] == (byte)enums.terrainType.sea )
				{ // coasts
					Point[] sqr = game.radius.returnEmptySquare( x, y, 1);
					int tot = 0;

					for ( int i = 0; i < sqr.Length; i ++ )
						if ( grid3[ sqr[ i ].X, sqr[ i ].Y ] == (byte)enums.terrainType.coast )
							tot ++;

					if ( tot > 2 )
					{
						grid3[ x, y ] = (byte)enums.terrainType.coast;
						pntFound --;
					}
				}
				tries--;
			}
			#endregion
				
				showProgress.progressBar ++;

			#region set tundra + glaciers
			for( int x = 0; x < Width; ++ x ) 
			{
				for( int y = 1; y < Height / 8; ++ y ) 
					if ( 
						( grid3[ x, y ] != (byte)enums.terrainType.sea &&
						grid3[ x, y ] != (byte)enums.terrainType.coast ) &&
						R.Next( (int)( (double)y / (double)( Height / 8 ) * 10)) == 0
						)
						grid3[ x, y ] = (byte)enums.terrainType.tundra;

				for( int y = 7 * Height / 8; y < Height - 1; ++ y ) 
					if ( 
						( grid3[ x, y ] != (byte)enums.terrainType.sea &&
						grid3[ x, y ] != (byte)enums.terrainType.coast ) &&
						R.Next( (int)((double)( Height - y ) / (double)( Height / 8 ) * 10)) == 0
						)
						grid3[ x, y ] = (byte)enums.terrainType.tundra;

				for ( int y = 1; y < Height; y += Height-3 )
					if ( R.Next( 5 ) == 0 )
						grid3[ x, y ] = (byte)enums.terrainType.glacier;
			}
				#endregion
				
				showProgress.progressBar ++;

			for( int x = 0; x < Width; ++ x ) 
				for( int y = 0; y < Height; ++ y ) 
					game.grid[ x, y ].type = grid3[ x, y ];

				if ( k >= 5 ) // failure
					return false;
			}

			return true;
		}
#endregion

#region find conts
		public void findContinents( int width, int height)
		{ // no more than 100 continents // non

//		//	Radius radius = new Radius();
			Point[] buffer;
			Point[] pntToGo;
			byte cont = 1;

			for ( int x = 0; x < width; x ++ )
				for ( int y = 0; y < height; y ++ )
				{
					if ( 
						game.grid[ x, y ].type != (byte)enums.terrainType.sea &&
						game.grid[ x, y ].type != (byte)enums.terrainType.coast &&
						game.grid[ x, y ].continent == 0 
						)
					{
						pntToGo = new Point[ 1 ];
						pntToGo[ 0 ] = new Point( x, y );
						//bool completed = false;

						for ( int i = 0; i < pntToGo.Length; i ++ )
						{
							game.grid[ pntToGo[ i ].X, pntToGo[ i ].Y ].continent = cont;
							Point[] sqr = game.radius.returnEmptySquare( pntToGo[ i ].X, pntToGo[ i ].Y, 1 );

							int j = 0;
							for ( int h = 0; h < sqr.Length; h ++ )
							{
								if ( 
									game.grid[ sqr[ h ].X, sqr[ h ].Y ].type != (byte)enums.terrainType.sea &&
									game.grid[ sqr[ h ].X, sqr[ h ].Y ].type != (byte)enums.terrainType.coast &&
									game.grid[ sqr[ h ].X, sqr[ h ].Y ].continent == 0 
									)
								{
									j ++;
								}
							}

							buffer = pntToGo;
							pntToGo = new Point[ buffer.Length + j ];

							for ( int h = 0; h < buffer.Length; h ++ )
							{
								pntToGo[ h ] = buffer[ h ];
							}

							j = 0;
							int k = buffer.Length;
							for ( int h = 0; h < sqr.Length; h ++ )
							{
								if ( 
									game.grid[ sqr[ h ].X, sqr[ h ].Y ].type != (byte)enums.terrainType.sea &&
									game.grid[ sqr[ h ].X, sqr[ h ].Y ].type != (byte)enums.terrainType.coast &&
									game.grid[ sqr[ h ].X, sqr[ h ].Y ].continent == 0 
									)
								{
									game.grid[ sqr[ h ].X, sqr[ h ].Y ].continent = cont;
									pntToGo[ k ] = sqr[ h ]; // sqr[ j ];
									j ++;
									k ++;
								}
							}
						}
						cont ++;
					}
				}
		}
		#endregion

#region set specials
		public void setSpecials()
		{
			int[] contSize = getContSize();
			int[] bestCases = new int[ contSize.Length ];

			for ( int j = 0; j < bestCases.Length; j ++ )
				bestCases[ j ] = j; 
 
			bool mod = true; 
			while ( mod ) 
			{ 
				mod = false; 
				for ( int j = 1; j < bestCases.Length; j ++ ) 
					if ( contSize[ bestCases[ j - 1 ] ] < contSize[ bestCases[ j ] ] ) 
					{ 
						int buffer = bestCases[ j - 1 ]; 
						bestCases[ j - 1 ] = bestCases[ j ]; 
						bestCases[ j ] = buffer; 
						mod = true; 
					} 
			} 

			int biggerCont = bestCases[ 0 ];
			int secBiggerCont, triBiggerCont;

			if ( bestCases.Length > 1 )
			{
				if ( contSize[ bestCases[ 1 ] ] != 0 )
				{
					secBiggerCont = bestCases[ 1 ];
				
					if ( bestCases.Length > 2 )
					{
						if ( contSize[ bestCases[ 2 ] ] != 0 )
						{
							triBiggerCont = bestCases[ 2 ];
						}
						else
						{
							triBiggerCont = bestCases[ 0 ];
						}
					}
					else
					{
						triBiggerCont = bestCases[ 0 ];
					}
				}
				else
				{
					secBiggerCont = bestCases[ 0 ];
					triBiggerCont = bestCases[ 0 ];
				}
			}
			else
			{
				secBiggerCont = bestCases[ 0 ];
				triBiggerCont = bestCases[ 0 ];
			}

//		//	Radius radius = new Radius();
			Random r = new Random();

			#region cheval
			for ( int i = 0; i < contSize[ biggerCont ] / 20 || i < 3; i ++ )
			{
				Point cheval = new Point( r.Next( game.width ), r.Next( game.height - 4 ) + 2 );

				if ( game.grid[ cheval.X, cheval.Y ].continent == biggerCont )
				{
					Point[] sqr = game.radius.returnSquare( cheval.X, cheval.Y, 3 );
					bool caseOk = true;
					for ( int j = 0; j < sqr.Length; j ++ )
						if ( game.grid[ sqr[ j ].X, sqr[ j ].Y ].resources > 0 )
						{
							caseOk = false;
							break;
						}

					if ( caseOk )
						game.grid[ cheval.X, cheval.Y ].resources = (byte)enums.speciaResources.horses;
				}
				else
					i --;
			}
			#endregion
			
			#region elephant
			for ( int i = 0; i < contSize[ secBiggerCont ] / 20 || i < 3; i ++ )
			{
				Point elephant = new Point( r.Next( game.width ), r.Next( game.height - 4 ) + 2 );

				if ( game.grid[ elephant.X, elephant.Y ].continent == secBiggerCont )
				{
					Point[] sqr = game.radius.returnSquare( elephant.X, elephant.Y, 3 );
					bool caseOk = true;
					for ( int j = 0; j < sqr.Length; j ++ )
					{
						if ( game.grid[ sqr[ j ].X, sqr[ j ].Y ].resources > 0 )
						{
							caseOk = false;
							break;
						}
					}

					if ( caseOk )
						game.grid[ elephant.X, elephant.Y ].resources = (byte)enums.speciaResources.elephant;
				}
				else
				{
					i --;
				}
			}
			#endregion
			
			#region camel
			for ( int i = 0; i < contSize[ triBiggerCont ] / 20 || i < 3; i ++ )
			{
				Point camel = new Point( r.Next( game.width ), r.Next( game.height - 4 ) + 2 );

				if ( game.grid[ camel.X, camel.Y ].continent == triBiggerCont )
				{
					Point[] sqr = game.radius.returnSquare( camel.X, camel.Y, 3 );
					bool caseOk = true;
					for ( int j = 0; j < sqr.Length; j ++ )
					{
						if ( game.grid[ sqr[ j ].X, sqr[ j ].Y ].resources > 0 )
						{
							caseOk = false;
							break;
						}
					}

					if ( caseOk )
						game.grid[ camel.X, camel.Y ].resources = (byte)enums.speciaResources.camel;
				}
				else
				{
					i --;
				}
			}
			#endregion
		}
		#endregion

#region setResources
		public void setResources( int quantity )
		{
			Random r = new Random(); 

			for ( int i = 0; i < Statistics.resources.Length; i ++ )
				for ( int j = 0, q = 0; q < quantity; j ++ )
				{
				//	if (  )

					int x = r.Next( game.width );
					int y = r.Next( game.height - 4 ) + 2;

					if ( 
						game.grid[ x, y ].resources == 0 && 
						!game.grid[ x, y ].river &&
						Statistics.terrains[ game.grid[ x, y ].type ].ew == Statistics.resources[ i ].terrain &&
						(
							Statistics.resources[ i ].terrain == 1 ||
							game.grid[ x, y ].type == (byte)enums.terrainType.coast
						)
						)
					{

						bool valid = true;
						for (int rad = 1; rad < 3 && valid; rad ++)
						{
							Point[] sqr = game.radius.returnEmptySquare(x, y, rad);

							for ( int k = 0; k < sqr.Length; k ++ )
								if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].resources > 0 )
								{
									valid = false;
									break;
								}
						}

						if ( valid )
						{
							game.grid[ x, y ].resources = (byte)(100 + i); 
							q ++; 
						}
					}
				}

		}
		#endregion

#region getContSize
		public int[] getContSize()
		{
			int[] contSize = new int[ 100 ];
			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 2; y < game.height - 2; y ++ )
					if ( game.grid[ x, y ].continent > 0 )
						contSize[ game.grid[ x, y ].continent ] ++;

			int[] retour = new int[ 0 ];
			for ( int i = 1; i < contSize.Length - 2; i ++ )
			{
				if ( contSize[ i ] == 0 && contSize[ i + 1 ] == 0 && contSize[ i + 2 ] == 0 )
				{
					
					retour = new int[ i ];
					for ( int j = 0; j < retour.Length ; j ++ )
					{
						retour[ j ] = contSize[ j ];
					}
					break;
				}
			}
				return retour;
		}
		#endregion

#region setRivers
		public void setRivers( int nbr )
		{
			Random r = new Random();
			int riverMinLength = 4;

			int[,] groundLevel = new int[ game.width, game.height ];

		#region set ground level

			for ( int x = 0; x < game.width; x ++ )
				for ( int y = 0; y < game.height; y ++ )
					if ( game.grid[ x, y ].type == (byte)enums.terrainType.coast )
					{
						Point[] sqr = game.radius.returnEmptySquare( x, y, 1 );
						for ( int k = 0; k < sqr.Length; k ++ )
							if ( game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent > 0 )
								groundLevel[ sqr[ k ].X, sqr[ k ].Y ] = 1;
					}

			bool mod = true;
			for ( int n = 1; mod; n ++ )
			{
				mod = false;

				for ( int x = 0; x < game.width; x++ )
					for ( int y = 0; y < game.height; y++ )
						if ( groundLevel[ x, y ] == n )
						{
							Point[] sqr = game.radius.returnEmptySquare( x, y, 1 );

							for ( int k = 0; k < sqr.Length; k ++ )
								if ( 
									groundLevel[ sqr[ k ].X, sqr[ k ].Y ] == 0 && 
									game.grid[ sqr[ k ].X, sqr[ k ].Y ].continent > 0 
									)
								{
									groundLevel[ sqr[ k ].X, sqr[ k ].Y ] = n + 1;
									mod = true;
								}
						}
			}
		#endregion

			for ( int u = 0, tries = 0; u < nbr && tries < 150 * nbr; u++, tries ++ )
			{
				if ( tries > 75 * nbr )
					riverMinLength = 3;

				bool riverValid = true, 
					completedRiver = false;
				Point[] path = new Point[ 100 ];

				path[ 0 ] = new Point( r.Next( game.width ), r.Next( game.height * 90 / 100 ) + game.height * 10 / 100 );
				
				while ( 
					!game.grid[ path[ 0 ].X, path[ 0 ].Y ].river && 
					groundLevel[ path[ 0 ].X, path[ 0 ].Y ] < riverMinLength // 2 
					)
					path[ 0 ] = new Point( r.Next( game.width ), r.Next( game.height * 80 / 100 ) + game.height * 10 / 100 );

				int posOnRiver = 0;

				while ( riverValid && !completedRiver )
				{
					Point[] sqr = game.radius.returnEmptySquare( path[ posOnRiver ].X, path[ posOnRiver ].Y, 1 );
					Point[] pPnts = new Point[ sqr.Length ], ultimatePPnts = new Point[ sqr.Length ];
					int totPPnts = 0, ultimateTotPPnts = 0;

					foreach ( Point p in sqr )
						if ( 
							groundLevel[ p.X, p.Y ] == 0 || 
							game.grid[ p.X, p.Y ].river 
							)
						{
							ultimatePPnts[ ultimateTotPPnts ] = p;
							ultimateTotPPnts ++;
						}
						else if ( 
							groundLevel[ p.X, p.Y ] <= groundLevel[ path[ posOnRiver ].X, path[ posOnRiver ].Y ] && 
							game.grid[ p.X, p.Y ].type != (byte)enums.terrainType.mountain && 
							(
								game.grid[ p.X, p.Y ].type != (byte)enums.terrainType.hill ||
								game.grid[ path[ posOnRiver ].X, path[ posOnRiver ].Y ].type == (byte)enums.terrainType.mountain
							)
							)
						{
							bool comingBack = false;

							for ( int i = 0; i < posOnRiver; i++ )
								if ( game.radius.isNextTo( path[ i ], p ) )
								{
									comingBack = true;
									break;
								}

							if ( !comingBack )
							{
								pPnts[ totPPnts ] = p;
								totPPnts ++;
							}
						}

					if ( ultimateTotPPnts > 0 )
					{
						if ( posOnRiver > riverMinLength / 2 )
						{
						//	Point p = 
							posOnRiver ++;
							path[ posOnRiver ] = ultimatePPnts[ r.Next( ultimateTotPPnts ) ];

						/*	for ( int h = 0; h <= posOnRiver; h ++ )
								if ( !game.grid[ path[ h ].X, path[ h ].Y ].river )
								{
									game.grid[ path[ h ].X, path[ h ].Y ].river = true;
									game.grid[ path[ h ].X, path[ h ].Y ].riversDir = new bool[ 8 ];
								}*/
						/*		game.grid[ path[ w ].X, path[ w ].Y ].river = true;
								game.grid[ path[ w ].X, path[ w ].Y ].riversDir = new bool[ 8 ];*/

							bool endInRiver = game.grid[ path[ posOnRiver ].X, path[ posOnRiver ].Y ].river;

							for ( int h = 0; h <= posOnRiver; h++ ) //way.Length - 1
							//	if ( path[ h ].X >= 0 && path[ h ].Y >= 0 )
								{
								/*	if ( !game.grid[ path[ h ].X, path[ h ].Y ].river )
									{
										game.grid[ path[ h ].X, path[ h ].Y ].river = true;
										game.grid[ path[ h ].X, path[ h ].Y ].riversDir = new bool[ 8 ];
									}*/
									if ( !game.grid[ path[ h ].X, path[ h ].Y ].river )
									{
										game.grid[ path[ h ].X, path[ h ].Y ].river = true;
										game.grid[ path[ h ].X, path[ h ].Y ].riversDir = new bool[ 8 ];
									}

									Point[] sqr0 = game.radius.returnSmallSqrInOrder( path[ h ] );

									for ( int k = 0; k < sqr0.Length; k++ )
										if ( 
											sqr0[ k ].X >= 0 && 
											( 
												(  // prev pos, 1 > h < posOnRiver
													h > 0 && 
													(
														h < posOnRiver ||
														endInRiver
													) && 
													sqr0[ k ] == path[ h - 1 ] 
												) || 
												(  // next pos, 0 < h < posOnRiver
													h < posOnRiver && 
													sqr0[ k ] == path[ h + 1 ] 
												) 
											) 
											)
											game.grid[ path[ h ].X, path[ h ].Y ].riversDir[ k ] = true;
								}

						/*	if ( game.grid[ p.X, p.Y ].river ) // game.grid[ path[ posOnRiver ].X, path[ posOnRiver ].Y ].river )
							{
								Point[] sqr0 = game.radius.returnSmallSqrInOrder( p );

								for ( int k = 0; k < sqr0.Length; k++ )
									if ( /*posOnRiver > 0 && sqr0[ k ].X >= 0 &&* sqr0[ k ] == path[ posOnRiver ] )
										game.grid[ p.X, p.Y ].riversDir[ k ] = true;

						/*		Point[] sqr0 = game.radius.returnSmallSqrInOrder( path[ posOnRiver ] );

								for ( int k = 0; k < sqr0.Length; k++ )
									if ( /*posOnRiver > 0 && sqr0[ k ].X >= 0 &&* sqr0[ k ] == path[ posOnRiver - 1 ] )
										game.grid[ path[ posOnRiver ].X, path[ posOnRiver ].Y ].riversDir[ k ] = true;*
							}
							else
							{
								game.grid[ p.X, p.Y ].river = true;
								game.grid[ p.X, p.Y ].riversDir = new bool[ 8 ];

								Point[] sqr0 = game.radius.returnSmallSqrInOrder( p );

								for ( int k = 0; k < sqr0.Length; k++ )
									if ( /*posOnRiver > 0 && sqr0[ k ].X >= 0 &&* sqr0[ k ] == path[ posOnRiver ] )
										game.grid[ p.X, p.Y ].riversDir[ k ] = true;
							}*/

							completedRiver = true;
						}
						else
							riverValid = false;

						break;
					}
					else if ( totPPnts > 0 )
					{
						path[ posOnRiver + 1 ] = pPnts[ r.Next( totPPnts ) ];
						posOnRiver++;
					}
					else
						riverValid = false;
				}

				if ( !riverValid )
					u--;
			}

		/*	Random r = new Random();
			for ( int i = 0, c = 0; i < nbr; c ++ )
			{
				if ( c > nbr * 100 )
				{
					break;
				}

				int x = r.Next( game.width );
				int y = r.Next( game.height - 6 ) + 3;

				if ( 
					game.grid[ x, y ].type != (byte)enums.terrainType.sea && 
					game.grid[ x, y ].type != (byte)enums.terrainType.coast && 
					game.grid[ x, y ].type != (byte)enums.terrainType.mountain && 
					game.grid[ x, y ].type != (byte)enums.terrainType.hill && 
					game.grid[ x, y ].type != (byte)enums.terrainType.glacier 
					) 
				{ 
					Point[] way = new Point[ 20 ];
					for ( int h = 0; h < way.Length; h ++ )
						way[ h ] = new Point( -1, -1 );

					bool finished = false;
					way[ 0 ] = new Point( -1, -1 );
					way[ 1 ] = new Point( x, y );
					int p = 1;
					for ( ; !finished && p < way.Length - 1; p ++ )
					{
						Point[] sqr = game.radius.returnEmptySquare( way[ p ], 1 );

						for ( int k = 0; k < sqr.Length; k ++ )
							if ( 
								sqr[ k ] != way[ p - 1 ] &&
								(
								game.grid[ sqr[ k ].X, sqr[ k ].Y ].river || 
								Statistics.terrains[ game.grid[ sqr[ k ].X, sqr[ k ].Y ].type ].ew == 0
								)
								)
							{
								way[ p + 1 ] = sqr[ k ];
								finished = true;
								break;
							}

						if ( !finished )
						{
							int tot = 0;
							Point[] possibilities = new Point[ sqr.Length ];
							for ( int k = 0; k < sqr.Length; k ++ )
								if ( 
									game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.sea &&
									game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.coast &&
									game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.mountain &&
									game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.hill &&
									game.grid[ sqr[ k ].X, sqr[ k ].Y ].type != (byte)enums.terrainType.glacier &&
									!game.radius.isNextTo( sqr[ k ], way[ p - 1 ] )
									)
								{
									bool alreadyUsed = false;
									for ( int j = 1; j < p; j ++ )
										if ( game.radius.getDistWith( way[ j ], sqr[ k ] ) < 2 )
											alreadyUsed = true;

									if ( !alreadyUsed )
									{
										possibilities[ tot ] = sqr[ k ];
										tot ++;
									}
								}

							if ( tot > 0 )
								way[ p + 1 ] = possibilities[ r.Next( tot ) ];
							else
								break;
						}
					}

					if ( finished && p > 4 )
					{
						for ( int h = 1; h < p; h ++ ) //way.Length - 1
							if ( 
								way[ h ].X >= 0 && 
								way[ h ].Y >= 0 
								)
							{
								if ( !game.grid[ way[ h ].X, way[ h ].Y ].river )
								{
									game.grid[ way[ h ].X, way[ h ].Y ].river = true;
									game.grid[ way[ h ].X, way[ h ].Y ].riversDir = new bool[ 8 ];
								}

								Point[] sqr = game.radius.returnSmallSqrInOrder( way[ h ] );

								for ( int k = 0; k < sqr.Length; k ++ ) 
									if ( 
										sqr[ k ].X >= 0 &&
										(
											(
												h > 0 && 
												sqr[ k ] == way[ h - 1 ] 
											) || 
											(
												h < way.Length && 
												sqr[ k ] == way[ h + 1 ] 
											)
										)
										) 
										game.grid[ way[ h ].X, way[ h ].Y ].riversDir[ k ] = true; 
							}

						if ( game.grid[ way[ p ].X, way[ p ].Y ].river )
						{
							Point[] sqr = game.radius.returnSmallSqrInOrder( way[ p ] );

							for ( int k = 0; k < sqr.Length; k ++ ) 
								if ( 
									sqr[ k ].X >= 0 &&
									sqr[ k ] == way[ p - 1 ] 
									) 
									game.grid[ way[ p ].X, way[ p ].Y ].riversDir[ k ] = true; 
						}

						i ++;
					}
				}
			}*/
		}
#endregion

	}
}
