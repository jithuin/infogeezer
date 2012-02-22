using System;
using System.Drawing;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for Unit.
	/// </summary>
	public class Unit : Construction // General
	{
		public const byte constructionType = 1;
//		public string name, description;
		public byte attack, defense, move;

		public enums.speciality speciality;
		public byte terrain; // 0 water, 1 land, 2 plane, 3 immobile
		public byte transport;

	//	public int cost; 

	//	public byte disponibility; //tech
		public byte obselete; //unit
		public System.Drawing.Bitmap bmp;

		public byte sight, range;

		public bool entrenchable;
		public bool highSeaSync;
		public bool stealth;
		public byte neededResource;

		public override byte getConstructionType()
		{
			return 1;
		}

	/*	public Unit( byte type )
		{
			this.type = type;
		}	*/

		public Unit( 
			byte type, string name, string description, byte attack, byte defense, 
			byte move, enums.speciality speciality, byte terrain, byte transport, 
			int cost, byte disponibility, byte obselete, byte sight, byte range, 
			bool entrenchable, bool highSeaSync, bool stealth, byte neededResource
			)
		{
			this.type = type;
			this.name = name;
			this.description = description;
			this.attack = attack;
			this.defense = defense;
			this.move = move;

			this.speciality = speciality;
			this.terrain = terrain;
			this.transport = transport;

			this.cost = cost; 

			this.disponibility = disponibility; // tech
			this.obselete = obselete; // unit

			this.sight = sight;
			this.range = range;

			this.entrenchable = entrenchable;
			this.highSeaSync = highSeaSync;
			this.stealth = stealth;
			this.neededResource = neededResource;

	//		this.neededTechno = Statistics.technologies[ disponibility ];
			
			try
			{
				this.bmp = new Bitmap( Form1.appPath + "\\images\\units\\" + name + ".png");
			}
			catch ( Exception e )
			{
				throw ( new Exception( "Error loading: " + name + ".png\n\n" + e.Message ) );
			}
		}
	}
}
