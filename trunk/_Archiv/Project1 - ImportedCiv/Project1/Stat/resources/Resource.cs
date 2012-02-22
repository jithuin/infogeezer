using System;
using System.Drawing;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for ResourceStat.
	/// </summary>
	public class Resource : Stat.General
	{
	//	public string name, description; 

	//	public byte type; 
		public byte terrain; 
		public byte techno; 
		public byte[] terrainTypes; 

		public byte foodBonus,
			prodBonus,
			tradeBonus; 

		public System.Drawing.Bitmap bmp, bmpUnseen; 

		public Resource( byte type )
		{
			this.type = type;
		}

		public Resource( byte type, string name, string description, byte foodBonus, byte prodBonus, byte tradeBonus, byte terrain, byte techno, string desc /*, byte[] terrainTypes*/ )
		{
			this.type = type;
			this.name = name;
			this.foodBonus = foodBonus;
			this.prodBonus = prodBonus;
			this.tradeBonus = tradeBonus;
			this.terrain = terrain;
			this.techno = techno;
			this.description = desc;
			//	Statistics.resources[type].terrainTypes = terrainTypes;
			
			try
			{
				this.bmp = new Bitmap(Form1.appPath + "\\images\\resources\\res" + name + ".png" );
			}
			catch ( Exception e )
			{
				throw new Exception( "Error loading: res" + name + ".png\n\n" + e.Message.ToString() ) ;
			}

			try
			{
				this.bmpUnseen = new Bitmap(Form1.appPath + "\\images\\resources\\res" + name + " oos.png" ); //drawUnseen( Statistics.resources[ type ].bmp );
			}
			catch ( Exception e )
			{
				throw new Exception( "Error loading: res" + name + " oos.png\n\n" + e.Message.ToString() ) ;
			}
		}
	}
}
