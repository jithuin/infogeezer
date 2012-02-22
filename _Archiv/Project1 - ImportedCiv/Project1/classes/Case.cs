using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for Case.
	/// </summary>
	public class Case
	{
		public Case( int X, int Y )
		{
			this.X = X;
			this.Y = Y;
		}

		public int X, Y;

		public byte type;
		public int city;
		public byte selectedLevel;

		public byte militaryImprovement;
		public byte civicImprovement;
		public int stackPos;
		public byte laborOwner;
		public int laborCity;
		public byte continent;
		public byte territory;
		public UnitList[] stack;
		public byte resources;

		public bool polluted;
		public bool isNextToWater;

		public bool river;
		public bool[] riversDir;

		public byte roadLevel;
		public structures.roadList[] roadDir;

		public bool water
		{
			get
			{
				return type == (byte)enums.terrainType.coast ||
					type == (byte)enums.terrainType.sea;
			}
		}

		public bool canBuildCity
		{
			get
			{
				return true;
			}
		}

		/*	public bool canMoveLandUnit
				{
					get
					{
						return true;
					}
				}	*/

		public bool canMoveSeaUnit
		{
			get
			{
				return true;
			}
		}	
	}
}
