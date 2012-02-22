using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for preferences.
	/// </summary>
	public class preferences
	{
		byte player;
		public sbyte laborProd;
		public sbyte laborFood;
		public sbyte laborTrade;

		public sbyte reserve;
		public sbyte military;
		public sbyte science;
		public sbyte intelligence;
		public sbyte buildings;
		public sbyte culture;
		public sbyte space;
		public sbyte exchanges;

		public preferences( int player)
		{
			this.player = (byte)player;
		}
	}
}
