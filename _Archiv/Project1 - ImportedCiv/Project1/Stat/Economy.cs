using System;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for Economy.
	/// </summary>
	public class Economy : General
	{
		public byte techno;
//		public string name;
//		public string desc;
		public bool supportSlavery;
		public bool communism;
		public byte prod;
		public byte food;
		public byte trade;
	
		// 
		public byte sciencePerc, counterIntPerc;
		// 

		public Economy( byte type )
		{
			this.type = type;
		}

		public Economy( 
			byte type, string name, bool communism, bool slavery, byte prod, byte food, byte trade, byte sciencePerc, byte counterIntPerc, byte techno, string desc
			)
		{
			this.type = type;
			this.name = name;
			this.description = desc;
			
			this.prod = prod;
			this.food = food;
			this.trade = trade;
			
			this.supportSlavery = slavery;
			this.communism = communism;
			
			this.techno = techno;

			this.sciencePerc = sciencePerc;
			this.counterIntPerc = counterIntPerc;
		}
	}
}
