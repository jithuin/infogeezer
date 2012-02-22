using System;
using System.Drawing;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for TerrainStat.
	/// </summary>
	public class Terrain : Stat.General
	{
	//	public string name, description;

		public byte food, trade, production,
			mineBonus, irrigationBonus, roadBonus;

		public byte riverTradeBonus;
		public byte riverFoodBonus;
		public byte ew;
		public byte move;
		public byte difficulty;
		public byte defense;
		public System.Drawing.Bitmap bmp, bmpUnseen;
		public System.Drawing.Color color;
	//	public enums.terrainType type;

		public bool canBuildIrrigation { get { return irrigationBonus > 0; } }
		public bool canBuildMine { get { return mineBonus > 0; } }


		public Terrain( enums.terrainType type )
		{
			this.type = (byte)type;
		}
		public Terrain(byte type, string name, byte ew, byte production, byte food, byte trade, byte move, byte mineBonus, byte irrigationBonus, byte roadBonus, byte riverTradeBonus, byte riverFoodBonus, Color color, byte difficulty, byte defense )
		{
			this.type = type;
			this.name = name;
			//Statistics.terrains[ type ].description = description;

			this.difficulty = difficulty; // building diff
			this.food = food;
			this.production = production;
			this.trade = trade;
			this.move = move;
			this.mineBonus = mineBonus;
			this.irrigationBonus = irrigationBonus;
			this.roadBonus = roadBonus;
			this.riverTradeBonus = riverTradeBonus;
			this.riverFoodBonus = riverFoodBonus;
			this.ew = ew;

			try
			{
				this.bmp = new Bitmap( Form1.appPath + "\\images\\cases\\case" + name + ".png");
				this.bmpUnseen = new Bitmap(Form1.appPath + "\\images\\cases\\case" + name + " oos.png"); // drawUnseen( Statistics.terrains[ terrainType ].bmp );
			}
			catch ( Exception e )
			{
				//	MessageBox.Show( "Error loading: case" + name + ".png", e.Message.ToString() );
				throw new Exception( "Error loading: case" + name + ".png\n\n" + e.Message.ToString() ) ;
				//	Application.Exit();
			}

			this.color = color;
			this.defense = defense;

		}
	}
}
