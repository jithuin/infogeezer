using System;
using System.Runtime.InteropServices;

namespace PInvokeLibrary
{
	/// <summary>
	/// Summary description for MemoryStatus.
	/// </summary>
	public class MemoryStatus
	{
		/// <summary>
		/// This structure contains information about current memory availability.
		/// The GlobalMemoryStatus function uses this structure. 
		///		typedef struct _MEMORYSTATUS 
		///		{ 
		///			DWORD dwLength; 
		///			DWORD dwMemoryLoad; 
		///			DWORD dwTotalPhys; 
		///			DWORD dwAvailPhys; 
		///			DWORD dwTotalPageFile; 
		///			DWORD dwAvailPageFile; 
		///			DWORD dwTotalVirtual; 
		///			DWORD dwAvailVirtual; 
		///		} MEMORYSTATUS, *LPMEMORYSTATUS; 
		/// </summary>
		public class MEMORYSTATUS
		{
			/// <summary>
			/// Initialize an instance of MEMORYSTATUS by setting the
			/// size parameter.
			/// </summary>
			public MEMORYSTATUS()
			{
				dwLength = (uint)Marshal.SizeOf(this);
			}

			/// <summary>
			/// Specifies the size, in bytes, of the MEMORYSTATUS structure. Set
			/// this member to sizeof(MEMORYSTATUS) when passing it to the
			/// GlobalMemoryStatus function.
			/// </summary>
			public uint dwLength;
			/// <summary>
			/// Specifies a number between 0 and 100 that gives a general idea of
			/// current memory utilization, in which 0 indicates no memory use and
			/// 100 indicates full memory use.
			/// </summary>
			public uint dwMemoryLoad;
			/// <summary>
			/// Indicates the total number of bytes of physical memory. 
			/// </summary>
			public uint dwTotalPhys;
			/// <summary>
			/// Indicates the number of bytes of physical memory available.
			/// </summary>
			public uint dwAvailPhys;
			/// <summary>
			/// Indicates the total number of bytes that can be stored in the
			/// paging file. Note that this number does not represent the actual
			/// physical size of the paging file on disk.
			/// </summary>
			public uint dwTotalPageFile;
			/// <summary>
			/// Indicates the number of bytes available in the paging file.
			/// </summary>
			public uint dwAvailPageFile;
			/// <summary>
			/// Indicates the total number of bytes that can be described in the user
			/// mode portion of the virtual address space of the calling process.
			/// </summary>
			public uint dwTotalVirtual;
			/// <summary>
			/// Indicates the number of bytes of unreserved and uncommitted memory
			/// in the user mode portion of the virtual address space of the calling
			/// process.
			/// </summary>
			public uint dwAvailVirtual;
		}

		/// <summary>
		/// This function gets information on the physical and virtual memory of
		/// the system.
		/// </summary>
		/// <param name="lpBuffer">[out] Pointer to a MEMORYSTATUS structure. The
		/// GlobalMemoryStatus function stores information about current memory
		/// availability in this structure.</param>
		[DllImport("CoreDll.dll")]
		public static extern void GlobalMemoryStatus(MEMORYSTATUS lpBuffer);

		/// <summary>
		/// This function retrieves information from the kernel pertaining to object
		/// store and system memory.
		/// </summary>
		/// <param name="lpdwStorePages">Long pointer to the number of pages dedicated
		/// to the store.</param>
		/// <param name="lpdwRamPages">Long pointer to the number of pages dedicated
		/// to system memory.</param>
		/// <param name="lpdwPageSize">Long pointer to the number of bytes in a page.</param>
		/// <returns>TRUE indicates success; FALSE indicates failure.</returns>
		[DllImport("CoreDll.dll")]
		public static extern int GetSystemMemoryDivision
		(
			ref uint lpdwStorePages,
			ref uint lpdwRamPages,
			ref uint lpdwPageSize
		);

		/// <summary>
		/// Run a test of the MemoryStatus class.
		/// </summary>
		/// <param name="showLine">Delegate called to show debug information</param>
		/*public static void TestProc(MainTest.DisplayLineDelegate showLine)
		{
			uint storePages = 0;
			uint ramPages = 0;
			uint pageSize = 0;
			int res = GetSystemMemoryDivision(ref storePages, ref ramPages, ref pageSize);

			showLine(String.Format("{0} store pages", storePages));
			showLine(String.Format("{0} ram pages", ramPages));
			showLine(String.Format("{0} bytes per page", pageSize));

			MEMORYSTATUS memStatus = new MEMORYSTATUS();
			GlobalMemoryStatus(memStatus);

			showLine(String.Format("{0} total MB", memStatus.dwTotalPhys / (1024*1024)));
			showLine(String.Format("{0} Available MB", memStatus.dwAvailPhys / (1024*1024)));
		}*/
	}
}
