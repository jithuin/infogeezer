using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for exchanges.
	/// </summary>
	public class exchangesInfo
	{
		public enum type 
		{ 
			gold,
			goldPerc,
			ceaseFire
		} 

		public static void add( byte player, byte other, byte type )
		{
		}

		public static void removeAt( byte player, byte other, int ind )
		{
		}

		public static void removeAll( byte player, byte other )
		{
		}
	}
}
