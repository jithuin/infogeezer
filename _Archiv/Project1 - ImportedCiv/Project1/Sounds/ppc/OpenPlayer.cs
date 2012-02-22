#if CF
using System;
using OpenNETCF.Multimedia.Audio;

namespace Sounds
{
	/// <summary>
	/// OpenPlayer allows to play multiple sounds at the same time.
	/// </summary>
	public class OpenPlayer : SoundPlayer
	{
		System.Collections.ArrayList list;

		public OpenPlayer()
		{
			list = new System.Collections.ArrayList();
		//	list.//new System.Collections.Specialized.ListDictionary();
		}

		public override bool play(string path, bool isName)
		{
		/*	try
			{*/
				if ( isName )
					path = platformSpec.main.appPath + "\\sounds\\" + path;

				OpenNETCF.Multimedia.Audio.Player p = new OpenNETCF.Multimedia.Audio.Player();
				System.IO.Stream s = new System.IO.FileStream( path, System.IO.FileMode.Open );
				
				p.DonePlaying += new OpenNETCF.Multimedia.Audio.WaveDoneHandler(p_DonePlaying);

				p.Play( s );
				s.Close();
				list.Add( p );
				return true;
		/*	}
			catch
			{
				return false;
			}*/
		}

		public override bool stop()
		{
	//		OpenNETCF.Multimedia.Audio.Player p = new OpenNETCF.Multimedia.Audio.Player();
			foreach ( Player p in list )
				if ( p.Playing )
					p.Stop();

			return true;
		}

		private void p_DonePlaying(object sender, System.IntPtr wParam, System.IntPtr lParam)
		{
			list.Remove( sender );
		}
	}
}
#endif