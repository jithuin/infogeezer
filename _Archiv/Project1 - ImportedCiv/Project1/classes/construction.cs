using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for construction.
	/// </summary>
	public class Construction
	{
		public Construction()
		{
			list = new Stat.Construction[ 10 ]; // new xycv_ppc.structures.constructionList[ 10 ];

	/*		for ( int i = 0; i < list.Length; i ++ )
				list[ i ].ind = -1;*/
		}
		
	//	public structures.constructionList[] list;
		public Stat.Construction[] list;

		public int points; 

		/// <summary>
		/// Down in priority... getting less important, a pos 1 will go to pos 2 actually.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public bool moveDown( int pos )
		{
			/*pos < list.Length - 1 && 
			list[ pos + 1 ].ind != -1*/
			//)

			if ( canMoveDown( pos ) )
			{
				exchange( pos, pos + 1 );
				/*structures.constructionList[] buffer = list;

				list[ pos ] = buffer[ pos + 1 ];
				list[ pos + 1 ] = buffer[ pos ];*/
			
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Up in priority, 2 going to 1
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public bool moveUp( int pos )
		{
			if ( canMoveUp( pos ) )//pos > 1 )
			{
				exchange( pos, pos - 1 );
				/*	structures.constructionList bufferPos = list[ pos ];
					structures.constructionList bufferPos = list[ pos ];*/
				/*	structures.constructionList[] buffer = list;

					list[ pos ]		=	buffer[ pos - 1 ];
					list[ pos - 1 ] =	buffer[ pos ];*/
			
				return true;
			}
			else
			{
				return false;
			}
		}

		public void exchange( int pos0, int pos1 )
		{
			//	structures.constructionList[] buffer = list;

			//	list[ pos0 ] = buffer[ pos1 ];
			//	list[ pos1 ] = buffer[ pos0 ];

			Stat.Construction old0 = list[ pos0 ],
				old1 = list[ pos1 ];

			list[ pos0 ] = old1;
			list[ pos1 ] = old0;
		}

		public void remove( int pos )
		{
			Stat.Construction[] buffer = list;

			for ( int i = pos + 1; i < list.Length; i ++ )
				list[ i - 1 ] = buffer[ i ];

			list[ list.Length - 1 ] = null;
		}

		public bool addAtEnd( Stat.Construction construction )//byte Type, int Ind )
		{
			for ( int i = 0; i < list.Length; i ++ )
				if( list[ i ] == null )
				{
					list[ i ] = construction;
				/*	list[ i ].type = Type;
					list[ i ].ind = Ind;*/
					return true;
				}

			return false;
		}

		public void removeFirst(  )
		{
			remove( 0 );
		}

		public bool canAddToTheList
		{
			get
			{
				
				for ( int i = 0; i < list.Length; i ++ )
					if ( list[ i ] is Stat.Wealth )
						return false;
					else if( list[ i ] == null )
						return true;

				return false;
			}
		}

		public bool canMoveDown( int pos )
		{
			if ( 
				pos + 1 >= list.Length ||
				list[ pos ] is Stat.Wealth ||		//	list[ pos ].constructioType == 3 ||
				list[ pos + 1 ] is Stat.Wealth 
				)
				return false;
			else if ( 
				pos < list.Length - 1 && 
				list[ pos + 1 ] != null //.ind != -1
				)
				return true;
			else
				return false;
		}

		public bool canMoveUp( int pos )
		{
			if ( list[ pos ].type == 3 )
				return false;
			else if ( pos > 0 )
				return true;
			else
				return false;
		}

		public bool empty
		{
			get
			{
				return ( list[ 0 ] == null ); //.ind == -1 );
			}
		}

		public void setFirstAndOnly( Stat.Construction c )
		{
			list[ 0 ] = c;

			for ( int i = 1; i < list.Length; i ++ )
				list[ i ] = null;
		}
	}
}
