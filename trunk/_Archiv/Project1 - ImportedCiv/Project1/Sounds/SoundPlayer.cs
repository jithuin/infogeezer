#region Using directives

using System;
using System.Text;

#endregion

namespace Sounds
{
	public abstract class SoundPlayer
	{
		public abstract bool play( string path, bool isName );
//		public abstract bool play( byte[] mem );
		public abstract bool stop();
	}
}
