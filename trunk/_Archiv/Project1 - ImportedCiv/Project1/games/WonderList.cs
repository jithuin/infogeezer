using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for WonderList.
	/// </summary>
	public class WonderList
	{
		public WonderList( int length )
		{
			wondersBuilt = new bool[ length ];
		}

		private bool[] wondersBuilt;
	
		public bool canBuildWonder( int type )
		{
			return !wondersBuilt[ type ];
		}
	
		public void buildWonder( Stat.Construction w )
		{
			wondersBuilt[ w.type ] = true;
		}
	
		public void destroyWonder( Stat.Wonder w )
		{
			wondersBuilt[ w.type ] = false;
		}
	
		public void initialize()
		{
			/*	foreach ( PlayerList player in playerList )
				{
					for ( int c = 1; c <= player.cityNumber; c ++ )
						for (  )
				}*/
		}
	}
}
