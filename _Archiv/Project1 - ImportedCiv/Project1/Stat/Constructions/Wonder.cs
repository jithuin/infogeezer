using System;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for Wonder.
	/// </summary>
	public class Wonder : Construction // General
	{
		public const byte constructionType = 5;
		public int culture;
		public byte happy;

		public Wonder( byte type, string name, string description, byte disponibility )
		{
			this.type = type;
			this.name = name;
			this.description = description;
			this.disponibility = disponibility;
	//		this.neededTechno = Statistics.technologies[ techno ];
		}

		public override byte getConstructionType()
		{
			return 5;
		}

		public enum list
		{
			greatPyramid, // all desert +1 prod, +1 trade
			chinaWall, // generate forteress on borders
			hangingGarden, // +2 food in each city
			alexandriaLighthouse, // + 1 or 2 sea units movement
			stoneEdge, // 
			sixtineChapel,
			statueOfLiberty, // 
			tourEffel,
			colossusRhodes,
			tajMahal,
			hubbleTelescope,
			iss, // ? ally with space tech ?
			manhattanProject, // unlock nuke worldwide
		}
	}
}
