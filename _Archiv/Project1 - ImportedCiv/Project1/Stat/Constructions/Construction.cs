using System;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for Construction.
	/// </summary>
	public abstract class Construction : General
	{
//		public const byte constructionType = 0;
		public int cost;
		public byte disponibility;
//		public Technology neededTechno;

		public Construction()
		{
		}

		public abstract byte getConstructionType();
	/*	{
	//		return 0; // constructionType;
		}*/

	/*	public static bool operator == ( Construction c0, Construction c1)
		{
			try
			{
				return c0.getConstructionType() == c1.getConstructionType() && 
					c0.type == c1.type;
			}
			catch
			{
				return false;
			}
		}

		public static bool operator != ( Construction c0, Construction c1)
		{
			try
			{
			return c0.getConstructionType() != c1.getConstructionType() || 
				c0.type != c1.type;
			}
			catch
			{
				return false;
			}
		}

		public override bool Equals(object obj)
		{
			try
			{
			return obj is Stat.Construction &&
				this == (Stat.Construction)obj;
			}
			catch
			{
				return false;
			}
		}*/

	}
}
