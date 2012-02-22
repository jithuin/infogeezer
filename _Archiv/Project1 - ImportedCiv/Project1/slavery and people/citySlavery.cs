using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for playerSlavery.
	/// </summary>
	public class citySlavery
	{
		int city;
		PlayerList player;

		public enum types: byte
		{
			food,
			prod,
			totp1
		}

		public byte[] list;
		public int convertingToSlave;

		public citySlavery( PlayerList player, int city)
		{
			this.player = player;
			this.city = city;
			list = new byte[ 0 ];
		}

		public int total
		{
			get
			{
				return list.Length;// types[ 0 ] + types[ 1 ];
			}
		}


		public void removeAll()
		{
			remove( total );
			player.cityList[ city ].invalidateLastTrade();
		}

		public int totalOfOneType(int ind)
		{
			int tot = 0;

			for ( int i = 0; i < list.Length; i++ )
				if ( list[ i ] == ind )
					tot++;

			return tot;
		}
		public bool isPossible(int ind)
		{
			//	if ( ind == Types.trader )
			return true;
			/*	else
				return false;*/
		}

		public void remove(int nbr)
		{
			byte[] buffer = list;

			list = new byte[ buffer.Length - nbr ];
			for ( int i = 0; i < list.Length; i++ )
				list[ i ] = buffer[ i ];
			player.cityList[ city ].invalidateLastTrade();
		}

		public void add(int nbr)
		{
			byte[] buffer = list;

			list = new byte[ buffer.Length + nbr ];
			buffer.CopyTo( list, 0 );

			Random r = new Random();

			for ( int i = buffer.Length; i < list.Length; i++ )
			{
				list[ i ] = (byte)r.Next( (byte)types.totp1 );
				while ( !isPossible( list[ i ] ) )
				{
					list[ i ] = (byte)r.Next( (byte)types.totp1 );
				}
			}
			player.cityList[ city ].invalidateLastTrade();
		}

		public void switchType(int pos, byte newType)
		{
			list[ pos ] = newType;
			player.cityList[ city ].invalidateLastTrade();
		}

		public void switchNextType(int pos)
		{
			int nextType = ( list[ pos ] + 1 ) % (byte)types.totp1;

			while ( !isPossible( nextType ) )
				nextType = ( nextType + 1 ) % (byte)types.totp1;

			switchType( pos, (byte)nextType );
		}
	}
}
