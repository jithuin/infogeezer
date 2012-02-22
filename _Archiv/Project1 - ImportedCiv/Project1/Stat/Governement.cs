using System;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for Stat.Governement.
	/// </summary>
	public class Governement : General
	{
		public Governement( 
			enums.governements type, string name, string description, bool oppressive, bool surrenderOnConquer, bool canBeChooseByPlayer, 
			byte prod, byte food, byte trade, byte counterIntPerc, byte sciencePerc, byte techno, Stat.Governement.supportType support )
		{
			this.type = (byte)type;
			this.name = name;
			this.description = description;
			this.oppresive = oppressive;
			this.surrenderOnConquer = surrenderOnConquer;
			this.neededTechno = techno;
			this.thisSupport = support;
			this.canBeChoosedByPlayer = canBeChooseByPlayer;

			this.productionPerc = prod;
			this.foodPerc = food;
			this.tradePerc = trade;
	//		this.popGrowthPerc = popGrowthPerc;	

			this.sciencePerc = sciencePerc;
			this.counterIntPerc = counterIntPerc;
		}
		
		public bool oppresive;
		public bool surrenderOnConquer;
		public bool canBeChoosedByPlayer;
		public byte productionPerc;

		public byte foodPerc;
		public byte tradePerc;
		public byte popGrowthPerc;
	
		// 
		public byte sciencePerc, counterIntPerc;
		// 

		public byte neededTechno;
		public supportType thisSupport;

		public enum supportType: byte
		{
			people,
			military,
			religion,
			wealth
		}
	}
}
