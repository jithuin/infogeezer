/*
 * Készítette a SharpDevelop.
 * Felhasználó: User
 * Dátum: 2012.01.06.
 * Idő: 18:40
 * 
 * A sablon megváltoztatásához használja az Eszközök | Beállítások | Kódolás | Szabvány Fejlécek Szerkesztését.
 */
using System;
using System.Windows.Forms;
  using System.Runtime.InteropServices;

namespace sleep
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			

           
                // Sleeps the machine
                SetSuspendState(false,true, false);
            
		}
		/// <summary>
            /// Suspends the system by shutting power down. Depending on the Hibernate parameter, the system either enters a suspend (sleep) state or hibernation (S4).
            /// </summary>
            /// <param name="hibernate">If this parameter is TRUE, the system hibernates. If the parameter is FALSE, the system is suspended.</param>
            /// <param name="forceCritical">Windows Server 2003, Windows XP, and Windows 2000:  If this parameter is TRUE,
            /// the system suspends operation immediately; if it is FALSE, the system broadcasts a PBT_APMQUERYSUSPEND event to each
            /// application to request permission to suspend operation.</param>
            /// <param name="disableWakeEvent">If this parameter is TRUE, the system disables all wake events. If the parameter is FALSE, any system wake events remain enabled.</param>
            /// <returns>If the function succeeds, the return value is true.</returns>
            /// <remarks>See http://msdn.microsoft.com/en-us/library/aa373201(VS.85).aspx</remarks>
            [DllImport("Powrprof.dll", SetLastError = true)]
            static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
	}
}
