using System;
using System.Data;
#if CF
using OpenNETCF.Win32;
#endif

/// <summary>
/// platformSpec used to be a seperate dll. All the code differences between Pocket PC  and desktop PC used to be within this namespace. 
/// Now it is integrate to the main executable, and use precompiler directives.
/// </summary>
namespace platformSpec
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class main
	{
#if CF
		public static bool isPPC = true;
#else
		public static bool isPPC = false;
#endif

#if CF
		public static int battryLife
		{
			get
			{
				try
				{
					PInvokeLibrary.PowerStatus.SYSTEM_POWER_STATUS_EX powSatus = new PInvokeLibrary.PowerStatus.SYSTEM_POWER_STATUS_EX();

					if ( PInvokeLibrary.PowerStatus.GetSystemPowerStatusEx( powSatus, false ) == 1 )
						return powSatus.BatteryLifePercent;
					else
						return -1;
				}
				catch 
				{
					System.Windows.Forms.MessageBox.Show( "Error getting battery life" );
					return 0;
				}
			}
		}
#endif

		public static uint deviceMemory
		{
			get
			{
#if CF
				try
				{
					uint storePages = 0, ramPages = 0, pageSize = 0;
					int res = PInvokeLibrary.MemoryStatus.GetSystemMemoryDivision( ref storePages, ref ramPages, ref pageSize );
					PInvokeLibrary.MemoryStatus.MEMORYSTATUS memStatus = new PInvokeLibrary.MemoryStatus.MEMORYSTATUS();

					PInvokeLibrary.MemoryStatus.GlobalMemoryStatus( memStatus );
					return memStatus.dwAvailPhys;
				}
				catch
				{
					System.Windows.Forms.MessageBox.Show( "Error getting memory status" );
					return 0;
				}

#else
				return 0;
#endif
			}
		}
		public static string ownerName
		{
			get
			{
#if CF
				try
				{
					byte[] ownerbytes = (byte[])OpenNETCF.Win32.Registry.CurrentUser.OpenSubKey( @"ControlPanel\Owner" ).GetValue( "Owner" );
					return System.Text.Encoding.Unicode.GetString( ownerbytes, 0, 72 ).TrimEnd( '\0' );
				}
				catch
				{
					System.Windows.Forms.MessageBox.Show( "Error getting owner name" );
					return "";
				}
#else
		//		System.Windows.i
				return Environment.UserName;
				//System.Security.Principal.WindowsIdentity.GetCurrent().Name
#endif
			}
		}

		public static void showSip()
		{
#if CF
			try
			{
				Microsoft.WindowsCE.Forms.InputPanel ip = new Microsoft.WindowsCE.Forms.InputPanel();
				ip.Enabled = true;
			}
			catch
			{
				System.Windows.Forms.MessageBox.Show( "Error showing SIP" );
			}
#else
#endif
		}

		public static string appPath
		{
			get
			{
#if CF
				return System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase );
#else
				return System.IO.Path.GetDirectoryName( System.Windows.Forms.Application.ExecutablePath );
#endif
			}
		}

		public static string gameName
		{
			get
			{
#if CF
				return "Pocket Humanity";
#else
				return "Domestic humanity";
#endif	
			}
		}

		public static System.Drawing.Font getFont( float size )
		{
#if CF
			if ( System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width > 320 )
				return new System.Drawing.Font( "Tahoma", size/2, System.Drawing.FontStyle.Regular );
			else
				return new System.Drawing.Font( "Tahoma", size, System.Drawing.FontStyle.Regular );
#else

			return new System.Drawing.Font( "Tahoma", size, System.Drawing.FontStyle.Regular );
#endif
		}	
	}
}
