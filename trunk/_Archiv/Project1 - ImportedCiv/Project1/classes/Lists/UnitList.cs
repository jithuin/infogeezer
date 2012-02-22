using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for UnitList.
	/// </summary>
	public class UnitList
	{
		public const int HIGHEST_AAGUN_RANGE = 3;

		public PlayerList player;
		public int ind;

		public UnitList( PlayerList player, int ind )
		{
			this.player = player;
			this.ind = ind;
		}
		
		public int X;
		public int Y;

		public System.Drawing.Point pos
		{
			get 
			{
				return new System.Drawing.Point( X, Y );
			}
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		public Case onCase
		{
			get
			{
				return player.game.grid[ X, Y ];
			}
		}

		public byte state;
		public sbyte health;

		public byte maxHealth
		{
			get
			{
				switch ( level )
				{
					default:
						return 3;
			//			break;

					case 1:
						return 4;
			//			break;

					case 2:
						return 5;
			//			break;
				}
			}
		}

		public int stack;
		public byte type;

		public sbyte moveLeft;
		public sbyte moveLeftFraction;

		public bool anyMoveLeft
		{
			get
			{
				return moveLeft > 0 || moveLeftFraction > 0;
			}
		}

		public int[] transport;
		public int transported;
		public byte level;
		public System.Drawing.Point dest;
		//public bool entrenchable;
		
		public bool automated;

		public bool dead
		{
			get
			{
				return state == (byte)Form1.unitState.dead;
			}
		}

		public Stat.Unit typeClass
		{
			get
			{
				return Statistics.units[ type ];
			}
		}

		public static bool operator == ( UnitList u0, UnitList u1 )
		{
			try
			{
				return u0.Equals( u1 );
			}
			catch
			{
				return false;
			}
		}

		public static bool operator != ( UnitList u0, UnitList u1 )
		{
			try
			{
				return // ( (object)this == null && o == null ) ||
					!u0.Equals( u1 ); 
			}
			catch
			{
				return false;
			}
		}

		public override bool Equals( Object o )
		{
			return ( (object)this == null && o == null ) ||
				(
				(object)this != null && 
				o != null &&
				this.ind == ((UnitList)o).ind && 
				this.player.player == ((UnitList)o).player.player
				);
		}

		public void kill( PlayerList killer )
		{
			kill();

			if ( ind == 1 )
				player.kill( killer );
		}
		public void kill()// PlayerList killer )
		{
			for ( int i = 0; i < transported; i ++ )
				player.unitList[ transport[ i ] ].state = (byte)Form1.unitState.dead;

			//	int x = game.playerList[ player ].unitList[ unit ].X;
			//	int y = game.playerList[ player ].unitList[ unit ].Y;

			/*	for ( int i = 0; i < game.caseImps.Length; i ++ )
					if ( game.caseImps[ i ].owner == player )
						for ( int u = 0; u < game.caseImps[ i ].units.Length; u ++ )
							if ( game.caseImps[ i ].units[ u ].ind == unit )*/

			if ( typeClass.speciality == enums.speciality.builder )
				caseImprovement.removeUnitFromCaseImps( player.player, ind );

			if ( state == (byte)Form1.unitState.inTransport )
				xycv_ppc.transport.removeUnitFromTransport( player.player, ind, xycv_ppc.transport.findTransporterOf( player.player, ind ) );
			else
			{
				move.moveUnitFromCase( X, Y, player.player, ind );
				player.game.grid[ X, Y ].stackPos = player.game.grid[ X, Y ].stack.Length;
			}

			state = (byte)Form1.unitState.dead;

			/*	bool oneUnitLeft = false;
				for ( int u = 0; u < player.unitNumber; u ++ )
					if ( !player.unitList[ u ].dead )
					{
						oneUnitLeft = true;
						break;
					}

				if ( !oneUnitLeft )*/

			/*			if ( ind == 0 )
							player.kill( killer );
			*/
		}

		public bool canBeUpgraded
		{
			get
			{
				return this.typeClass.obselete != 0 && 
					player.technos[ Statistics.units[ this.typeClass.obselete ].disponibility ].researched;
			}
		}

		public void upgrade()
		{
			this.type = this.typeClass.obselete;
	//		byte unitType = Statistics.units[ game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].type ].obselete;
			
	//		game.playerList[ Form1.game.curPlayerInd ].unitList[ selected.unit ].kill();
			//	unitDelete( Form1.game.curPlayerInd, selected.unit );
	//		createUnit( x, y, Form1.game.curPlayerInd, unitType );

			moveLeft = 0;
			moveLeftFraction = 0;

	//		showNextUnit( Form1.game.curPlayerInd, selected.unit );
		}
	}
}
