#if !CF
using System;
using System.Runtime.InteropServices;

namespace Sounds
{
	/// <summary>
	/// Summary description for WinSound.
	/// </summary>
	public class WinSound : SoundPlayer
	{
		bool working;

		public WinSound()
		{
			working = true;
		}

		[DllImport("WinMM.dll")]
		public static extern bool  PlaySound(byte[]wfname, int fuSound);

		//  flag values for SoundFlags  argument on PlaySound
		private const int SND_SYNC          = 0x0000;  // play synchronously (default) 
		private const int SND_ASYNC         = 0x0001;  // play asynchronously 
		private const int SND_NODEFAULT     = 0x0002;  // silence (!default) if sound not found 
		private const int SND_MEMORY        = 0x0004;  // pszSound points to a memory file 
		private const int SND_LOOP          = 0x0008;  // loop the sound until next sndPlaySound 
		private const int SND_NOSTOP        = 0x0010;  // don't stop any currently playing sound 

		private const int SND_NOWAIT		= 0x00002000; // don't wait if the driver is busy 
		private const int SND_ALIAS			= 0x00010000; // name is a registry alias 
		private const int SND_ALIAS_ID		= 0x00110000; // alias is a predefined ID 
		private const int SND_FILENAME		= 0x00020000; // name is file name 
		private const int SND_RESOURCE		= 0x00040004; // name is resource name or atom 
		private const int SND_PURGE         = 0x0040;  // purge non-static events for task 
		private const int SND_APPLICATION   = 0x0080;  // look for application specific association 

		public override bool play( string file, bool isName )
		{
			if ( isName )
				file = platformSpec.main.appPath + "\\sounds\\" + file;

			if ( working ) 
				try
				{
					byte[] bytes = new Byte[ 256 ];		//	Max path length
					bytes = System.Text.Encoding.ASCII.GetBytes( file );
					return PlaySound( bytes, SND_FILENAME | SND_ASYNC );
				}
				catch ( Exception e )
				{
					System.Windows.Forms.MessageBox.Show(
						e.Message,
						"erreur"
						);
					if ( !(e is System.IO.FileNotFoundException) )
						working = false;
				}

			return false;
		}

		public override bool stop()
		{
			return PlaySound( null, SND_PURGE );
			
		}
	}
}
#endif