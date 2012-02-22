using System;
using System.Drawing;

namespace xycv_ppc.draw
{
	/// <summary>
	/// Summary description for draw.
	/// </summary>
	public class borders
	{
		static int diffX = 3;
		static int diffY = 1;
		static int modX = 2;
		static int modY = 1;

		public static void draw( Point pos, Point aff, Graphics g, Options.FrontierTypes type )
		{
			if ( type == Options.FrontierTypes.nationColor )
			{ // neigbor
				if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ pos.X, pos.Y ] )
					civSeen( pos, aff, g );
				else if ( Form1.game.playerList[ Form1.game.curPlayerInd ].discovered[ pos.X, pos.Y ] )
					civUnseen( pos, aff, g );
			}
			else
				relations( pos, aff, g );
		}

		public static void drawAll( Point pos, Point aff, Graphics g, Options.FrontierTypes type )
		{
			if ( type == Options.FrontierTypes.nationColor )
				civAll( pos, aff, g );
			else
				relations( pos, aff, g );
		}

#region civAll
		public static void civAll( Point pos, Point aff, Graphics g )
		{
			Point left =	new Point( aff.X + 10, aff.Y + 35 );
			Point right =	new Point( aff.X + 60, aff.Y + 35 );
			Point top =		new Point( aff.X + 35, aff.Y + 20 );
			Point bottom =	new Point( aff.X + 35, aff.Y + 50 );

			Point rtl = Form1.game.radius.returnUpLeft( pos );
			Point rtr = Form1.game.radius.returnUpRight( pos );
			Point rbl = Form1.game.radius.returnDownLeft( pos );
			Point rbr = Form1.game.radius.returnDownRight( pos );

			Point rt = Form1.game.radius.returnUp( pos );
			Point rb = Form1.game.radius.returnDown( pos );
			Point rl = Form1.game.radius.returnLeft( pos );
			Point rr = Form1.game.radius.returnRight( pos );
			
			#region top
			if ( 
				rt.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rt.X, rt.Y ].territory
				)
			{
				Point[] region =	{
										top,
										new Point( top.X + diffX, top.Y + diffY ),
										new Point( top.X, top.Y + diffY * 2 + modY ),
										new Point( top.X - diffX, top.Y + diffY )
									};

				if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rt.X, rt.Y ].territory - 1 ].civType ].color ), 
						region 
						);
				}
				else
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
						region 
						);
				}
			}
			#endregion
			
			#region bottom
			if ( 
				rb.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rb.X, rb.Y ].territory
				)
			{
				Point[] region =	{
										bottom,
										new Point( bottom.X + diffX, bottom.Y - diffY ),
										new Point( bottom.X, bottom.Y - diffY * 2 - modY ),
										new Point( bottom.X - diffX, bottom.Y - diffY )
									};

				if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rb.X, rb.Y ].territory - 1 ].civType ].color ), 
						region 
						);
				}
				else
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
						region 
						);
				}
			}
			#endregion
			
			#region left
			if ( 
				rl.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rl.X, rl.Y ].territory
				)
			{
				Point[] region =	{
										left,
										new Point( left.X + diffX,			left.Y - diffY	),
										new Point( left.X + diffX * 2 - modX,	left.Y	),
										new Point( left.X + diffX,			left.Y + diffY	)
									};

				if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rl.X, rl.Y ].territory - 1 ].civType ].color ), 
						region 
						);
				}
				else
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
						region 
						);
				}
			}
			#endregion
			
			#region right
			if ( 
				rr.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rr.X, rr.Y ].territory
				)
			{
				Point[] region =	{
										right,
										new Point( right.X - diffX,			right.Y - diffY	),
										new Point( right.X - diffX * 2 + modX,	right.Y	),
										new Point( right.X - diffX,			right.Y + diffY	)
									};

				if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rr.X, rr.Y ].territory - 1 ].civType ].color ), 
						region 
						);
				}
				else
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
						region 
						);
				}
			}
			#endregion

			#region topLeft
			if ( 
				rtl.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rtl.X, rtl.Y ].territory
				)
			{
				Point[] topLeft =	{
										left,
										top,
										new Point( top.X + diffX, top.Y + diffY ),
										new Point( left.X + diffX, left.Y + diffY )
									};

				if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rtl.X, rtl.Y ].territory - 1 ].civType ].color ), 
						topLeft 
						);
				}
				else
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
						topLeft 
						);
				}
			}
			#endregion

			#region topRight
			if ( 
				rtr.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rtr.X, rtr.Y ].territory 
				)
			{
				Point[] topRight =	{
										right,
										top,
										new Point( top.X - diffX, top.Y + diffY ),
										new Point( right.X - diffX, right.Y + diffY )
									};

				if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rtr.X, rtr.Y ].territory - 1 ].civType ].color ), 
						topRight 
						);
				}
				else
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
						topRight 
						);
				}
			}
			#endregion

			#region bottomLeft
			if ( 
				rbl.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rbl.X, rbl.Y ].territory 
				)
			{
				Point[] bottomLeft =	{
											left,
											bottom,
											new Point( bottom.X + diffX, bottom.Y - diffY ),
											new Point( left.X + diffX, left.Y - diffY )
										};

				if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rbl.X, rbl.Y ].territory - 1 ].civType ].color ), 
						bottomLeft 
						);
				}
				else
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
						bottomLeft 
						);
				}
			}
			#endregion

			#region bottomRight
			if ( 
				rbr.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rbr.X, rbr.Y ].territory 
				)
			{
				Point[] bottomRight =	{
											right,
											bottom,
											new Point( bottom.X - diffX, bottom.Y - diffY ),
											new Point( right.X - diffX, right.Y - diffY )
										};

				if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rbr.X, rbr.Y ].territory - 1 ].civType ].color ), 
						bottomRight 
						);
				}
				else
				{
					g.FillPolygon( 
						new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
						bottomRight 
						);
				}
			}
			#endregion
		}
		#endregion

#region civSeen
		public static void civSeen( Point pos, Point aff, Graphics g )
		{
			Point left =	new Point( aff.X + 10, aff.Y + 35 );
			Point right =	new Point( aff.X + 60, aff.Y + 35 );
			Point top =		new Point( aff.X + 35, aff.Y + 20 );
			Point bottom =	new Point( aff.X + 35, aff.Y + 50 );

			Point rtl = Form1.game.radius.returnUpLeft( pos );
			Point rtr = Form1.game.radius.returnUpRight( pos );
			Point rbl = Form1.game.radius.returnDownLeft( pos );
			Point rbr = Form1.game.radius.returnDownRight( pos );

			Point rt = Form1.game.radius.returnUp( pos );
			Point rb = Form1.game.radius.returnDown( pos );
			Point rl = Form1.game.radius.returnLeft( pos );
			Point rr = Form1.game.radius.returnRight( pos );
			
			#region top
			if ( rt.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rt.X, rt.Y ] )
			{
				if ( 
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rt.X, rt.Y ].territory
					)
				{
					Point[] region =	{
											top,
											new Point( top.X + diffX, top.Y + diffY ),
											new Point( top.X, top.Y + diffY * 2 + modY ),
											new Point( top.X - diffX, top.Y + diffY )
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rt.X, rt.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			else
			{
				if ( 
					rt.X != -1 &&
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rt.X, rt.Y ].territory
					)
				{
					Point[] region =	{
											top,
											new Point( top.X + diffX, top.Y + diffY ),
											new Point( top.X, top.Y + diffY * 2 + modY ),
											new Point( top.X - diffX, top.Y + diffY )
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rt.X, rt.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			#endregion
			
			#region bottom
			if ( rb.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rb.X, rb.Y ] )
			{
				if ( 
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rb.X, rb.Y ].territory
					)
				{
					Point[] region =	{
											bottom,
											new Point( bottom.X + diffX, bottom.Y - diffY ),
											new Point( bottom.X, bottom.Y - diffY * 2 - modY ),
											new Point( bottom.X - diffX, bottom.Y - diffY )
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rb.X, rb.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			else
			{
				if ( 
					rb.X != -1 &&
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rb.X, rb.Y ].territory
					)
				{
					Point[] region =	{
											bottom,
											new Point( bottom.X + diffX, bottom.Y - diffY ),
											new Point( bottom.X, bottom.Y - diffY * 2 - modY ),
											new Point( bottom.X - diffX, bottom.Y - diffY )
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rb.X, rb.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			#endregion
			
			#region left
			if ( rl.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rl.X, rl.Y ] )
			{
				if ( 
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rl.X, rl.Y ].territory
					)
				{
					Point[] region =	{
											left,
											new Point( left.X + diffX,			left.Y - diffY	),
											new Point( left.X + diffX * 2 - modX,	left.Y	),
											new Point( left.X + diffX,			left.Y + diffY	)
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rl.X, rl.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			else
			{
				if ( 
					rl.X != -1 &&
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rl.X, rl.Y ].territory
					)
				{
					Point[] region =	{
											left,
											new Point( left.X + diffX,			left.Y - diffY	),
											new Point( left.X + diffX * 2 - modX,	left.Y	),
											new Point( left.X + diffX,			left.Y + diffY	)
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rl.X, rl.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			#endregion
			
			#region right
			
			if ( rr.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rr.X, rr.Y ] )
			{
				if ( 
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rr.X, rr.Y ].territory
					)
				{
					Point[] region =	{
											right,
											new Point( right.X - diffX,			right.Y - diffY	),
											new Point( right.X - diffX * 2 + modX,	right.Y	),
											new Point( right.X - diffX,			right.Y + diffY	)
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rr.X, rr.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			else
			{
				if ( 
					rr.X != -1 &&
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rr.X, rr.Y ].territory
					)
				{
					Point[] region =	{
											right,
											new Point( right.X - diffX,			right.Y - diffY	),
											new Point( right.X - diffX * 2 + modX,	right.Y	),
											new Point( right.X - diffX,			right.Y + diffY	)
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rr.X, rr.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			#endregion

			#region topLeft
			
			if ( rtl.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rtl.X, rtl.Y ] )
			{
				if ( 
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rtl.X, rtl.Y ].territory
					)
				{
					Point[] topLeft =	{
											left,
											top,
											new Point( top.X + diffX, top.Y + diffY ),
											new Point( left.X + diffX, left.Y + diffY )
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rtl.X, rtl.Y ].territory - 1 ].civType ].color ), 
							topLeft 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							topLeft 
							);
					}
				}
			}
			else
			{
				if ( 
					rtl.X != -1 &&
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rtl.X, rtl.Y ].territory
					)
				{
					Point[] topLeft =	{
											left,
											top,
											new Point( top.X + diffX, top.Y + diffY ),
											new Point( left.X + diffX, left.Y + diffY )
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rtl.X, rtl.Y ].territory - 1 ].civType ].color ), 
							topLeft 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							topLeft 
							);
					}
				}
			}
			#endregion

			#region topRight
			
			if ( rtr.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rtr.X, rtr.Y ] )
			{
				if ( 
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rtr.X, rtr.Y ].territory 
					)
				{
					Point[] topRight =	{
											right,
											top,
											new Point( top.X - diffX, top.Y + diffY ),
											new Point( right.X - diffX, right.Y + diffY )
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rtr.X, rtr.Y ].territory - 1 ].civType ].color ), 
							topRight 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							topRight 
							);
					}
				}
			}
			else
			{
				if ( 
					rtr.X != -1 &&
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rtr.X, rtr.Y ].territory 
					)
				{
					Point[] topRight =	{
											right,
											top,
											new Point( top.X - diffX, top.Y + diffY ),
											new Point( right.X - diffX, right.Y + diffY )
										};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rtr.X, rtr.Y ].territory - 1 ].civType ].color ), 
							topRight 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							topRight 
							);
					}
				}
			}
			#endregion

			#region bottomLeft
			
			if ( rbl.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rbl.X, rbl.Y ] )
			{
				if ( 
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rbl.X, rbl.Y ].territory 
					)
				{
					Point[] bottomLeft =	{
												left,
												bottom,
												new Point( bottom.X + diffX, bottom.Y - diffY ),
												new Point( left.X + diffX, left.Y - diffY )
											};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rbl.X, rbl.Y ].territory - 1 ].civType ].color ), 
							bottomLeft 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							bottomLeft 
							);
					}
				}
			}
			else
			{
				if ( 
					rbl.X != -1 &&
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rbl.X, rbl.Y ].territory 
					)
				{
					Point[] bottomLeft =	{
												left,
												bottom,
												new Point( bottom.X + diffX, bottom.Y - diffY ),
												new Point( left.X + diffX, left.Y - diffY )
											};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rbl.X, rbl.Y ].territory - 1 ].civType ].color ), 
							bottomLeft 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							bottomLeft 
							);
					}
				}
			}
			#endregion

			#region bottomRight
			
			if ( rbr.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rbr.X, rbr.Y ] )
			{
				if ( 
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rbr.X, rbr.Y ].territory 
					)
				{
					Point[] bottomRight =	{
												right,
												bottom,
												new Point( bottom.X - diffX, bottom.Y - diffY ),
												new Point( right.X - diffX, right.Y - diffY )
											};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rbr.X, rbr.Y ].territory - 1 ].civType ].color ), 
							bottomRight 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							bottomRight 
							);
					}
				}
			}
			else
			{
				if ( 
					rbr.X != -1 &&
					Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rbr.X, rbr.Y ].territory 
					)
				{
					Point[] bottomRight =	{
												right,
												bottom,
												new Point( bottom.X - diffX, bottom.Y - diffY ),
												new Point( right.X - diffX, right.Y - diffY )
											};

					if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rbr.X, rbr.Y ].territory - 1 ].civType ].color ), 
							bottomRight 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							bottomRight 
							);
					}
				}
			}
			#endregion

		}
		#endregion

#region civ unseen
		public static void civUnseen( Point pos, Point aff, Graphics g )
		{
			Point left =	new Point( aff.X + 10, aff.Y + 35 );
			Point right =	new Point( aff.X + 60, aff.Y + 35 );
			Point top =		new Point( aff.X + 35, aff.Y + 20 );
			Point bottom =	new Point( aff.X + 35, aff.Y + 50 );

			Point rtl = Form1.game.radius.returnUpLeft( pos );
			Point rtr = Form1.game.radius.returnUpRight( pos );
			Point rbl = Form1.game.radius.returnDownLeft( pos );
			Point rbr = Form1.game.radius.returnDownRight( pos );

			Point rt = Form1.game.radius.returnUp( pos );
			Point rb = Form1.game.radius.returnDown( pos );
			Point rl = Form1.game.radius.returnLeft( pos );
			Point rr = Form1.game.radius.returnRight( pos );
			
			#region top
			if ( rt.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rt.X, rt.Y ] )
			{
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.grid[ rt.X, rt.Y ].territory
					)
				{
					Point[] region =	{
											top,
											new Point( top.X + diffX, top.Y + diffY ),
											new Point( top.X, top.Y + diffY * 2 + modY ),
											new Point( top.X - diffX, top.Y + diffY )
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rt.X, rt.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			else
			{
				if ( 
					rt.X != -1 &&
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rt.X, rt.Y ].territory
					)
				{
					Point[] region =	{
											top,
											new Point( top.X + diffX, top.Y + diffY ),
											new Point( top.X, top.Y + diffY * 2 + modY ),
											new Point( top.X - diffX, top.Y + diffY )
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rt.X, rt.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			#endregion
			
			#region bottom
			if ( rb.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rb.X, rb.Y ] )
			{
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.grid[ rb.X, rb.Y ].territory
					)
				{
					Point[] region =	{
											bottom,
											new Point( bottom.X + diffX, bottom.Y - diffY ),
											new Point( bottom.X, bottom.Y - diffY * 2 - modY ),
											new Point( bottom.X - diffX, bottom.Y - diffY )
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rb.X, rb.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			else
			{
				if ( 
					rb.X != -1 &&
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rb.X, rb.Y ].territory
					)
				{
					Point[] region =	{
											bottom,
											new Point( bottom.X + diffX, bottom.Y - diffY ),
											new Point( bottom.X, bottom.Y - diffY * 2 - modY ),
											new Point( bottom.X - diffX, bottom.Y - diffY )
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rb.X, rb.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			#endregion
			
			#region left
			if ( rl.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rl.X, rl.Y ] )
			{
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.grid[ rl.X, rl.Y ].territory
					)
				{
					Point[] region =	{
											left,
											new Point( left.X + diffX,			left.Y - diffY	),
											new Point( left.X + diffX * 2 - modX,	left.Y	),
											new Point( left.X + diffX,			left.Y + diffY	)
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rl.X, rl.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			else
			{
				if ( 
					rl.X != -1 &&
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rl.X, rl.Y ].territory
					)
				{
					Point[] region =	{
											left,
											new Point( left.X + diffX,			left.Y - diffY	),
											new Point( left.X + diffX * 2 - modX,	left.Y	),
											new Point( left.X + diffX,			left.Y + diffY	)
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rl.X, rl.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			#endregion
			
			#region right
			if ( rr.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rr.X, rr.Y ] )
			{
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.grid[ rr.X, rr.Y ].territory
					)
				{
					Point[] region =	{
											right,
											new Point( right.X - diffX,			right.Y - diffY	),
											new Point( right.X - diffX * 2 + modX,	right.Y	),
											new Point( right.X - diffX,			right.Y + diffY	)
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rr.X, rr.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			else
			{
				if ( 
					rr.X != -1 &&
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rr.X, rr.Y ].territory
					)
				{
					Point[] region =	{
											right,
											new Point( right.X - diffX,			right.Y - diffY	),
											new Point( right.X - diffX * 2 + modX,	right.Y	),
											new Point( right.X - diffX,			right.Y + diffY	)
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rr.X, rr.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							region 
							);
					}
				}
			}
			#endregion

			#region topLeft
			if ( rtl.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rtl.X, rtl.Y ] )
			{
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.grid[ rtl.X, rtl.Y ].territory
					)
				{
					Point[] topLeft =	{
											left,
											top,
											new Point( top.X + diffX, top.Y + diffY ),
											new Point( left.X + diffX, left.Y + diffY )
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rtl.X, rtl.Y ].territory - 1 ].civType ].color ), 
							topLeft 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							topLeft 
							);
					}
				}
			}
			else
			{
				if ( 
					rtl.X != -1 &&
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rtl.X, rtl.Y ].territory
					)
				{
					Point[] topLeft =	{
											left,
											top,
											new Point( top.X + diffX, top.Y + diffY ),
											new Point( left.X + diffX, left.Y + diffY )
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rtl.X, rtl.Y ].territory - 1 ].civType ].color ), 
							topLeft 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							topLeft 
							);
					}
				}
			}
			#endregion

			#region topRight
			if ( rtr.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rtr.X, rtr.Y ] )
			{
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.grid[ rtr.X, rtr.Y ].territory 
					)
				{
					Point[] topRight =	{
											right,
											top,
											new Point( top.X - diffX, top.Y + diffY ),
											new Point( right.X - diffX, right.Y + diffY )
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rtr.X, rtr.Y ].territory - 1 ].civType ].color ), 
							topRight 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							topRight 
							);
					}
				}
			}
			else
			{
				if ( 
					rtr.X != -1 &&
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rtr.X, rtr.Y ].territory 
					)
				{
					Point[] topRight =	{
											right,
											top,
											new Point( top.X - diffX, top.Y + diffY ),
											new Point( right.X - diffX, right.Y + diffY )
										};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rtr.X, rtr.Y ].territory - 1 ].civType ].color ), 
							topRight 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							topRight 
							);
					}
				}
			}
			#endregion

			#region bottomLeft
			if ( rbl.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rbl.X, rbl.Y ] )
			{
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.grid[ rbl.X, rbl.Y ].territory 
					)
				{
					Point[] bottomLeft =	{
												left,
												bottom,
												new Point( bottom.X + diffX, bottom.Y - diffY ),
												new Point( left.X + diffX, left.Y - diffY )
											};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rbl.X, rbl.Y ].territory - 1 ].civType ].color ), 
							bottomLeft 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							bottomLeft 
							);
					}
				}
			}
			else
			{
				if ( 
					rbl.X != -1 &&
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rbl.X, rbl.Y ].territory 
					)
				{
					Point[] bottomLeft =	{
												left,
												bottom,
												new Point( bottom.X + diffX, bottom.Y - diffY ),
												new Point( left.X + diffX, left.Y - diffY )
											};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rbl.X, rbl.Y ].territory - 1 ].civType ].color ), 
							bottomLeft 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							bottomLeft 
							);
					}
				}
			}
			#endregion

			#region bottomRight
			if ( rbr.X != -1 )
			if ( Form1.game.playerList[ Form1.game.curPlayerInd ].see[ rbr.X, rbr.Y ] )
			{
				if ( 
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.grid[ rbr.X, rbr.Y ].territory
					)
				{
					Point[] bottomRight =	{
												right,
												bottom,
												new Point( bottom.X - diffX, bottom.Y - diffY ),
												new Point( right.X - diffX, right.Y - diffY )
											};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ rbr.X, rbr.Y ].territory - 1 ].civType ].color ), 
							bottomRight 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							bottomRight 
							);
					}
				}
			}
			else
			{
				if ( 
					rbr.X != -1 &&
					Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory != Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rbr.X, rbr.Y ].territory
					)
				{
					Point[] bottomRight =	{
												right,
												bottom,
												new Point( bottom.X - diffX, bottom.Y - diffY ),
												new Point( right.X - diffX, right.Y - diffY )
											};

					if ( Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory == 0 )
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ rbr.X, rbr.Y ].territory - 1 ].civType ].color ), 
							bottomRight 
							);
					}
					else
					{
						g.FillPolygon( 
							new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.playerList[ Form1.game.curPlayerInd ].lastSeen[ pos.X, pos.Y ].territory - 1 ].civType ].color ), 
							bottomRight 
							);
					}
				}
			}
			#endregion

		}
		#endregion

#region relations
		public static void relations( Point pos, Point aff, Graphics g )
		{
			Point left =	new Point( aff.X + 10,	aff.Y + 35 );
			Point right =	new Point( aff.X + 60,	aff.Y + 35 );
			Point top =		new Point( aff.X + 35,	aff.Y + 20 );
			Point bottom =	new Point( aff.X + 35,	aff.Y + 50 );

			Point rtl = Form1.game.radius.returnUpLeft( pos );
			Point rtr = Form1.game.radius.returnUpRight( pos );
			Point rbl = Form1.game.radius.returnDownLeft( pos );
			Point rbr = Form1.game.radius.returnDownRight( pos );

			Point rt = Form1.game.radius.returnUp( pos );
			Point rb = Form1.game.radius.returnDown( pos );
			Point rl = Form1.game.radius.returnLeft( pos );
			Point rr = Form1.game.radius.returnRight( pos );
			
			#region top
			if ( 
				rt.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rt.X, rt.Y ].territory
				)
			{
				Point[] region =	{
										top,
										new Point( top.X + diffX, top.Y + diffY ),
										new Point( top.X, top.Y + diffY * 2 + modY ),
										new Point( top.X - diffX, top.Y + diffY )
									};

				verifyRelation( g, region, pos, rt );
			}
			#endregion
			
			#region bottom
			if ( 
				rb.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rb.X, rb.Y ].territory
				)
			{
				Point[] region =	{
										bottom,
										new Point( bottom.X + diffX, bottom.Y - diffY ),
										new Point( bottom.X, bottom.Y - diffY * 2 - modY ),
										new Point( bottom.X - diffX, bottom.Y - diffY )
									};

				verifyRelation( g, region, pos, rb );
			}
			#endregion
			
			#region left
			if ( 
				rl.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rl.X, rl.Y ].territory
				)
			{
				Point[] region =	{
										left,
										new Point( left.X + diffX,			left.Y - diffY	),
										new Point( left.X + diffX * 2 - modX,	left.Y	),
										new Point( left.X + diffX,			left.Y + diffY	)
									};

				verifyRelation( g, region, pos, rl );
			}
			#endregion
			
			#region right
			if ( 
				rr.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rr.X, rr.Y ].territory
				)
			{
				Point[] region =	{
										right,
										new Point( right.X - diffX,			right.Y - diffY	),
										new Point( right.X - diffX * 2 + modX,	right.Y	),
										new Point( right.X - diffX,			right.Y + diffY	)
									};

				verifyRelation( g, region, pos, rr );
			}
			#endregion

			#region topLeft
			if ( 
				rtl.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rtl.X, rtl.Y ].territory
				)
			{
				Point[] region =	{
										left,
										top,
										new Point( top.X + diffX, top.Y + diffY ),
										new Point( left.X + diffX, left.Y + diffY )
									};

				verifyRelation( g, region, pos, rtl );
			}
			#endregion

			#region topRight
			if ( 
				rtr.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rtr.X, rtr.Y ].territory 
				)
			{
				Point[] region =	{
										right,
										top,
										new Point( top.X - diffX, top.Y + diffY ),
										new Point( right.X - diffX, right.Y + diffY )
									};

				verifyRelation( g, region, pos, rtr );
			}
			#endregion

			#region bottomLeft
			if ( 
				rbl.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rbl.X, rbl.Y ].territory 
				)
			{
				Point[] region =	{
											left,
											bottom,
											new Point( bottom.X + diffX, bottom.Y - diffY ),
											new Point( left.X + diffX, left.Y - diffY )
										};

				verifyRelation( g, region, pos, rbl );
			}
			#endregion

			#region bottomRight
			if ( 
				rbr.X != -1 &&
				Form1.game.grid[ pos.X, pos.Y ].territory != Form1.game.grid[ rbr.X, rbr.Y ].territory 
				)
			{
				Point[] region =	{
											right,
											bottom,
											new Point( bottom.X - diffX, bottom.Y - diffY ),
											new Point( right.X - diffX, right.Y - diffY )
										};

				verifyRelation( g, region, pos, rbr );
			}
			#endregion
		}
#endregion

#region verifyRelation
		private static void verifyRelation( Graphics g, Point[] region, Point pos, Point pnt )
		{
			if ( Form1.game.grid[ pos.X, pos.Y ].territory == 0 )
				g.FillPolygon( new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pnt.X, pnt.Y ].territory - 1 ].civType ].color ), region );
			else if ( Form1.game.grid[ pnt.X, pnt.Y ].territory == 0 )
				g.FillPolygon( new SolidBrush( Statistics.civilizations[ Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].civType ].color ), region );
			else if ( Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].foreignRelation[ Form1.game.grid[ pnt.X, pnt.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.war )
				g.FillPolygon( new SolidBrush( Color.Red ), region );
			else if ( Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].foreignRelation[ Form1.game.grid[ pnt.X, pnt.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.peace )
				g.FillPolygon( new SolidBrush( Color.White ), region );
			else if ( Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].foreignRelation[ Form1.game.grid[ pnt.X, pnt.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.ceaseFire )
				g.FillPolygon( new SolidBrush( Color.Gray ), region );
			else if ( Form1.game.playerList[ Form1.game.grid[ pos.X, pos.Y ].territory - 1 ].foreignRelation[ Form1.game.grid[ pnt.X, pnt.Y ].territory - 1 ].politic == (byte)Form1.relationPolType.alliance )
				g.FillPolygon( new SolidBrush( Color.Gold ), region );
		}
		#endregion


	}
}
