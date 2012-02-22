#if CF
#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace Sounds
{
	class CorePlayer : SoundPlayer
	{
		[DllImport( "coredll" )]
		private static extern bool PlaySound( string szBack, IntPtr hMod, PlayBackFlags flags );
/*
		[DllImport( "coredll" )]
		private static extern bool PlaySound( IntPtr pMem, IntPtr hMod, PlayBackFlags flags );
*/
		[Flags]
		enum PlayBackFlags : int
		{
			SND_SYNC		= 0x0000,		// play synchronously (default) 
			SND_ASYNC		= 0x0001,		// play asynchronously 
			SND_NODEFAULT	= 0x0002,		// silence (!default) if Back not found 
			SND_MEMORY		= 0x0004,		// pszBack points to a memory file 
			SND_LOOP		= 0x0008,		// loop the Back until next sndPlayBack 
			SND_NOSTOP		= 0x0010,		// don't stop any currently playing Back
			SND_NOWAIT		= 0x00002000,	// don't wait if the driver is busy
			SND_ALIAS		= 0x00010000,	// name is a registry alias 
			SND_ALIAS_ID	= 0x00110000,	// alias is a predefined ID
			SND_FILENAME	= 0x00020000,	// name is file name
			SND_RESOURCE	= 0x00040004,	// name is resource name or atom
			SND_PURGE		= 0x0040,		// purge non-static events for task 
			SND_APPLICATION	= 0x0080		// look for application specific association 
		}

		public CorePlayer()
		{
		}

		public override bool play( string path, bool isName  )
		{
			if ( isName )
				path = platformSpec.main.appPath + "\\sounds\\" + path;

			return PlaySound( path, IntPtr.Zero, PlayBackFlags.SND_FILENAME|PlayBackFlags.SND_NOSTOP|PlayBackFlags.SND_ASYNC );
		}

/*		public override bool play( byte[] mem )
		{
			GCHandle gch = GCHandle.Alloc( mem, GCHandleType.Pinned );
			int pPtr;

			unsafe
			{
				pPtr = (int)gch.AddrOfPinnedObject().ToPointer();
				pPtr += 4;
			}

			bool success = PlaySound( (IntPtr)pPtr, IntPtr.Zero, PlayBackFlags.SND_MEMORY|PlayBackFlags.SND_NOSTOP|PlayBackFlags.SND_ASYNC );

			gch.Free();
			return success;
		}		*/

		public override bool stop()
		{
			return PlaySound( null, IntPtr.Zero, PlayBackFlags.SND_PURGE );
		}
	}
}
#endif