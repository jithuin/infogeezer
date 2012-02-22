using System;

namespace platformSpec
{
	/// <summary>
	/// Summary description for resolution.
	/// </summary>
	public class resolution
	{
	/*	public bool isQVGA
		{
			public 
		}*/
		public static void set( System.Windows.Forms.Form form )
		{
#if CF
			if ( System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 )
			{
				form.ClientSize = new System.Drawing.Size( form.ClientSize.Width*2, form.ClientSize.Height*2 );
				set( form.Controls );
			}
#endif
		}

		public static void set( System.Windows.Forms.Control.ControlCollection cs )
		{
#if CF
			if ( System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 )
			{
				foreach( System.Windows.Forms.Control c in cs )
				{
					c.Width *= 2;
					c.Height *= 2;
					c.Left*= 2;
					c.Top *= 2;

#if !CF
					c.Font = new System.Drawing.Font( c.Font.Name, c.Font.Size * 2, c.Font.Style );// *= 2;
#endif
				}
			}
#endif
		}

		public static void setOne( System.Windows.Forms.Control c )
		{
			if ( System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 )
			{
				c.Width *= 2;
				c.Height *= 2;
				c.Left*= 2;
				c.Top *= 2;

#if !CF
				c.Font = new System.Drawing.Font( c.Font.Name, c.Font.Size * 2, c.Font.Style );// *= 2;
#endif
			}
		}
		
		/*public static int getDrawSizeMod
		{
			get
			{
				if ( 
					System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 && 
					System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height > 320
					)
				{
					return 2;
				}
				else
				{
					return 1;
				}
			}
		}*/

		static private int lastSizeMod = -1;
		public static int sizeMod
		{
			get
			{
#if CF
				if ( lastSizeMod == -1 )
				{
					if ( System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height == 640 )
						lastSizeMod = 2;
					else
						lastSizeMod = 1;

				}

				return lastSizeMod;
#else
				if ( lastSizeMod == -1 )
				{
					lastSizeMod = 1;
					return lastSizeMod;
				}
				else
					return lastSizeMod;
#endif
			}
			set
			{
				lastSizeMod = value;
			}
		}

		public static int mod
		{
			get
			{
		/*		if ( System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 )
					return 2;
				else
					return 1;*/
				return sizeMod;
			}
		}

		public static float getStaticSizeForFont( float i )
		{
#if CF
			if ( System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 )
				return i / 2;
			else
				return i;
#else
			return i;
#endif
		}
	}
}
