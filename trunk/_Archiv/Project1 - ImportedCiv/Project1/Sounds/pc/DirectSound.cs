#if false //!CF
using System;
using Microsoft.DirectX.DirectSound;
using Microsoft.DirectX;

namespace Sounds
{
	/// <summary>
	/// Summary description for DirectSound.
	/// </summary>
	public class DirectSound : SoundPlayer
	{

		private SecondaryBuffer applicationBuffer = null;
		private Device applicationDevice = null;

		public DirectSound( System.Windows.Forms.Form form )
		{
			try
			{
				applicationDevice = new Device();
				applicationDevice.SetCooperativeLevel( form, CooperativeLevel.Priority );
				working = true;
			}
			catch ( Exception e )
			{
				System.Windows.Forms.MessageBox.Show(
					"Le système de son DirectX ne fonctionne pas. Vous pouvez changer la compatibilité des effets sonores à partir des options.\n" + e.Message,
					"erreur"
					);

				working = false;
			}
		}

		public override void play( string file, bool isName )
		{
			if ( isName )
				path = platformSpec.main.appPath + "\\sounds\\" + path;

			if ( working ) 
				try
				{
					file = System.IO.Path.GetDirectoryName( System.Windows.Forms.Application.ExecutablePath ) + "\\" + file;
					applicationBuffer = new SecondaryBuffer( file, applicationDevice );
					applicationBuffer.Volume = -1000;
					applicationBuffer.Play( 0, BufferPlayFlags.Default );
				}
				catch ( Exception e )
				{
					System.Windows.Forms.MessageBox.Show(
						e.Message,
						"erreur"
						);

					if ( e != System.IO.FileNotFoundException )
						working = false;
				}
		}

		public override void stop()
		{
		}
	}
}
#endif