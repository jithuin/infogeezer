using System;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for Wealth.
	/// </summary>
	public class Wealth : Construction
	{
		public const byte constructionType = 3;

		public Wealth()
		{
			this.name = "Wealth";
			this.description = "Converts all production point in money, on every turn.";
		}

		public override byte getConstructionType()
		{
			return 3;
		}
	}
}
