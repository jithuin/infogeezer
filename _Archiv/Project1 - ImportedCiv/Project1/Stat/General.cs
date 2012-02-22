using System;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for General.
	/// </summary>
	public class General : IComparable
	{
		public string name,
			description; 
		public byte type;

		public General()
		{
		}

		public int CompareTo(object obj)
		{
			return name.CompareTo( ((General)obj).name );
		}
	}
}
