using System;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for Building.
	/// </summary>
	public class Building : Construction // General
	{
		public const byte constructionType = 2;
		//	public string name, description;

		//	public int cost;
		public int effect;
		//	public byte disponibility;
		public byte upkeep;

		public Building( byte type, string name, int cost, byte disponibility, byte upkeep, string description)
		{
			this.type = type;
			this.name = name;
			this.description = description;
			this.cost = cost;
			this.disponibility = disponibility;
			this.upkeep = upkeep;

			//		this.neededTechno = Statistics.technologies[ disponibility ];
		}

		public override byte getConstructionType()
		{
			return 2;
		}

		public enum list
		{
			// culture
			library,
			university,
	
			// religious
			temple,
			cathedale,
		
			// military
			wall,
			barrack,
			militaryBase,
			airBase,
			navalBase,
			secretBase,

			samSite,
			navalFortress,
			bertha,
			
			// energy
			windMill,
			waterMill, // n river, better than wind
			coalPlant,
			hydroPlant, // n river, better than coal
			nuclearPlant,
			solarPlant,
			
			// reserch
			atelier,
			laboratory
		
		}
	}
}
