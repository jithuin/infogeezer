using System;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for SmallWonder.
	/// </summary>
	public class SmallWonder : Construction
	{
		public const byte constructionType = 4;

		public SmallWonder( byte type, string name, string description, byte disponibility )
		{
			this.type = type;
			this.name = name;
			this.description = description;
			this.disponibility = disponibility;
	//		this.neededTechno = Statistics.technologies[ techno ];
		}

		public override byte getConstructionType()
		{
			return 4;
		}

		public enum list
		{
			satteliteControl, // launch and control sattelites 
			armyHQ,
			intelligenceHQ, // adv apying 
			globalDefenseSystem, // nuke detection 
		}
	}
}
