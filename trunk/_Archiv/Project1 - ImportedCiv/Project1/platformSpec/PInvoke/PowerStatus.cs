using System;
using System.Runtime.InteropServices;

namespace PInvokeLibrary
{
	/// <summary>
	/// Provides access to the device's power and battery status.
	/// </summary>
	public class PowerStatus
	{
		// ACLineStatus
		public const byte AC_LINE_OFFLINE = 0x00;
		public const byte AC_LINE_ONLINE = 0x01;
		public const byte AC_LINE_BACKUP_POWER = 0x02;
		public const byte AC_LINE_UNKNOWN = 0xFF;

		// BatteryFlag and BackupBatteryFlag
		public const byte BATTERY_FLAG_HIGH = 0x01;
		public const byte BATTERY_FLAG_LOW = 0x02;
		public const byte BATTERY_FLAG_CRITICAL = 0x04;
		public const byte BATTERY_FLAG_CHARGING = 0x08;
		public const byte BATTERY_FLAG_NO_BATTERY = 0x80;
		public const byte BATTERY_FLAG_UNKNOWN = 0xFF;

		// BatteryLifePercent and BackupBatteryLifePercent
		public const byte BATTERY_PERCENTAGE_UNKNOWN = 0xFF;

		// BatteryLifeTime, BatteryFullLifeTime, BackupBatteryLifeTime, and BackupBatteryFullLifeTime
		public const uint BATTERY_LIFE_UNKNOWN = 0xFFFFFFFF;

		// BatteryChemistry
		public const byte BATTERY_CHEMISTRY_ALKALINE = 0x01;
		public const byte BATTERY_CHEMISTRY_NICD = 0x02;
		public const byte BATTERY_CHEMISTRY_NIMH = 0x03;
		public const byte BATTERY_CHEMISTRY_LION = 0x04;
		public const byte BATTERY_CHEMISTRY_LIPOLY = 0x05;
		public const byte BATTERY_CHEMISTRY_UNKNOWN = 0xFF;

		/// <summary>
		/// This structure contains information about the power status of the system.
		///		typedef struct _SYSTEM_POWER_STATUS_EX2 
		///		{
		///			BYTE ACLineStatus;
		///			BYTE BatteryFlag;
		///			BYTE BatteryLifePercent;
		///			BYTE Reserved1;
		///			DWORD BatteryLifeTime;
		///			DWORD BatteryFullLifeTime;
		///			BYTE Reserved2;
		///			BYTE BackupBatteryFlag;
		///			BYTE BackupBatteryLifePercent;
		///			BYTE Reserved3;
		///			DWORD BackupBatteryLifeTime;
		///			DWORD BackupBatteryFullLifeTime;
		///			DWORD BatteryVoltage;
		///			DWORD BatteryCurrent;
		///			DWORD BatteryAverageCurrent;
		///			DWORD BatteryAverageInterval;
		///			DWORD BatterymAHourConsumed;
		///			DWORD BatteryTemperature;
		///			DWORD BackupBatteryVoltage;
		///			BYTE BatteryChemistry;
		///		} SYSTEM_POWER_STATUS_EX2, *PSYSTEM_POWER_STATUS_EX2,
		///		  *LPSYSTEM_POWER_STATUS_EX2;
		/// </summary>
		public class SYSTEM_POWER_STATUS_EX2
		{
			/// <summary>
			/// AC power status.
			/// </summary>
			public byte ACLineStatus;
			/// <summary>
			/// Battery charge status.
			/// </summary>
			public byte BatteryFlag;
			/// <summary>
			/// Percentage of full battery charge remaining. This member can be a value
			/// in the range 0 to 100, or 255 if the status is unknown. All other values
			/// are reserved.
			/// </summary>
			public byte BatteryLifePercent;
			/// <summary>
			/// Reserved; set to zero.
			/// </summary>
			public byte Reserved1;
			/// <summary>
			/// Number of seconds of battery life remaining, or 0xFFFFFFFF if remaining
			/// seconds are unknown.
			/// </summary>
			public uint BatteryLifeTime;
			/// <summary>
			/// Number of seconds of battery life when at full charge, or 0xFFFFFFFF if
			/// full battery lifetime is unknown.
			/// </summary>
			public uint BatteryFullLifeTime;
			/// <summary>
			/// Reserved; set to zero.
			/// </summary>
			public byte Reserved2;
			/// <summary>
			/// Backup battery charge status.
			/// </summary>
			public byte BackupBatteryFlag;
			/// <summary>
			/// Percentage of full backup battery charge remaining. This value must
			/// be in the range 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN. 
			/// </summary>
			public byte BackupBatteryLifePercent;
			/// <summary>
			/// Reserved; set to zero.
			/// </summary>
			public byte Reserved3;
			/// <summary>
			/// Number of seconds of backup battery life remaining, or
			/// BATTERY_LIFE_UNKNOWN if remaining seconds are unknown.
			/// </summary>
			public uint BackupBatteryLifeTime;
			/// <summary>
			/// Number of seconds of backup battery life when at full charge, or
			/// BATTERY_LIFE_UNKNOWN if full battery lifetime is unknown.
			/// </summary>
			public uint BackupBatteryFullLifeTime;
			/// <summary>
			/// Amount of battery voltage in millivolts (mV). This member can have
			/// a value in the range of 0 to 65,535.
			/// </summary>
			public uint BatteryVoltage;
			/// <summary>
			/// Amount of instantaneous current drain in milliamperes (mA). This member
			/// can have a value in the range of 0 to 32,767 for charge, or 0 to
			/// –32,768 for discharge.
			/// </summary>
			public uint BatteryCurrent;
			/// <summary>
			/// Short-term average of device current drain (mA). This member can have
			/// a value in the range of 0 to 32,767 for charge, or 0 to –32,768 for
			/// discharge.
			/// </summary>
			public uint BatteryAverageCurrent;
			/// <summary>
			/// Time constant in milliseconds (ms) of integration used in reporting
			/// BatteryAverageCurrent.
			/// </summary>
			public uint BatteryAverageInterval;
			/// <summary>
			/// Long-term cumulative average discharge in milliamperes per hour (mAH).
			/// This member can have a value in the range of 0 to –32,768. This value
			/// can be reset by charging or changing the batteries.
			/// </summary>
			public uint BatterymAHourConsumed;
			/// <summary>
			/// Battery temperature in degrees Celsius (°C). This member can have a
			/// value in the range of –3,276.8 to 3,276.7; the increments are 0.1 °C.
			/// </summary>
			public uint BatteryTemperature;
			/// <summary>
			/// Backup battery voltage in mV.
			/// </summary>
			public uint BackupBatteryVoltage;
			/// <summary>
			/// This can be one of the values listed above.
			/// </summary>
			public byte BatteryChemistry;
		}

		/// <summary>
		/// This structure contains information about the power status of the system.
		/// </summary>
		public class SYSTEM_POWER_STATUS_EX
		{
			/// <summary>
			/// AC power status.
			/// </summary>
			public byte ACLineStatus;
			/// <summary>
			/// Battery charge status.
			/// </summary>
			public byte BatteryFlag;
			/// <summary>
			/// Percentage of full battery charge remaining. This member can be a
			/// value in the range 0 to 100, or 255 if status is unknown. All other
			/// values are reserved.
			/// </summary>
			public byte BatteryLifePercent;
			/// <summary>
			/// Reserved; set to zero.
			/// </summary>
			public byte Reserved1;
			/// <summary>
			/// Number of seconds of battery life remaining, or 0xFFFFFFFF if
			/// remaining seconds are unknown.
			/// </summary>
			public uint BatteryLifeTime;
			/// <summary>
			/// Number of seconds of battery life when at full charge, or 0xFFFFFFFF
			/// if full lifetime is unknown.
			/// </summary>
			public uint BatteryFullLifeTime;
			/// <summary>
			/// Reserved; set to zero.
			/// </summary>
			public byte Reserved2;
			/// <summary>
			/// Backup battery charge status.
			/// </summary>
			public byte BackupBatteryFlag;
			/// <summary>
			/// Percentage of full backup battery charge remaining. Must be in the
			/// range 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN.
			/// </summary>
			public byte BackupBatteryLifePercent;
			/// <summary>
			/// Reserved; set to zero
			/// </summary>
			public byte Reserved3;
			/// <summary>
			/// Number of seconds of backup battery life remaining, or
			/// BATTERY_LIFE_UNKNOWN if remaining seconds are unknown.
			/// </summary>
			public uint BackupBatteryLifeTime;
			/// <summary>
			/// Number of seconds of backup battery life when at full charge, or
			/// BATTERY_LIFE_UNKNOWN if full lifetime is unknown.
			/// </summary>
			public uint BackupBatteryFullLifeTime;
		}

		/// <summary>
		/// This function retrieves the power status of the system. The status indicates
		/// whether the system is running on AC or DC power, whether or not the batteries
		/// are currently charging, and the remaining life of main and backup batteries. 
		/// A remote application interface (RAPI) version of this function exists, and
		/// it is called CeGetSystemPowerStatusEx (RAPI).
		/// </summary>
		/// <param name="lpSystemPowerStatus">[out] Pointer to the SYSTEM_POWER_STATUS_EX
		/// structure receiving the power status information.</param>
		/// <param name="fUpdate">[in] If this Boolean is set to TRUE,
		/// GetSystemPowerStatusEx gets the latest information from the device driver,
		/// otherwise it retrieves cached information that may be out-of-date by
		/// several seconds.</param>
		/// <returns></returns>
		[DllImport("coredll")]
		public static extern uint GetSystemPowerStatusEx(SYSTEM_POWER_STATUS_EX lpSystemPowerStatus, bool fUpdate);

		/// <summary>
		/// This function retrieves battery status information.
		/// </summary>
		/// <param name="lpSystemPowerStatus">[out] Pointer to a buffer that receives
		/// power status information.</param>
		/// <param name="dwLen">[in] Specifies the length of the buffer pointed to by
		/// pSystemPowerStatusEx2.</param>
		/// <param name="fUpdate">[in] Specify TRUE to get the latest information from
		/// the device driver. Specify FALSE to get cached information that may be
		/// out-of-date by several seconds.</param>
		/// <returns></returns>
		[DllImport("coredll")]
		public static extern uint GetSystemPowerStatusEx2(SYSTEM_POWER_STATUS_EX2 lpSystemPowerStatus, uint dwLen, bool fUpdate);

		
		/// <summary>
		/// Run a test of the PowerStatus class.
		/// </summary>
		/// <param name="showLine">Delegate called to show debug information</param>
		/*public static void TestProc(MainTest.DisplayLineDelegate showLine)
		{
			SYSTEM_POWER_STATUS_EX status = new SYSTEM_POWER_STATUS_EX();
			SYSTEM_POWER_STATUS_EX2 status2 = new SYSTEM_POWER_STATUS_EX2();

			if (GetSystemPowerStatusEx(status, false) == 1)
			{
				showLine("SYSTEM_POWER_STATUS_EX:");
				switch (status.ACLineStatus)
				{
					case AC_LINE_OFFLINE: 
						showLine("AC Power Off");
						break;
					case AC_LINE_ONLINE:
						showLine("AC Power On");
						break;
					case AC_LINE_BACKUP_POWER:
						showLine("AC Power backup");
						break;
					default:
						showLine("AC Power status unknown");
						break;
				}

				showLine(String.Format("Battery: {0}%", status.BatteryLifePercent));
				showLine(String.Format("Backup Battery: {0}%", status.BackupBatteryLifePercent));
			}
			else
			{
				showLine("FAILURE: GetSystemPowerStatusEx failed");
			}

			if (GetSystemPowerStatusEx2(status2, (uint)Marshal.SizeOf(status2), false) == (uint)Marshal.SizeOf(status2))
			{
				showLine("SYSTEM_POWER_STATUS_EX2:");
				showLine(String.Format("Battery: {0} mV", status2.BatteryVoltage));
				switch (status2.BatteryChemistry)
				{
					case BATTERY_CHEMISTRY_ALKALINE:
						showLine("Alkaline battery");
						break;
					case BATTERY_CHEMISTRY_NICD:
						showLine("NICD battery");
						break;
					case BATTERY_CHEMISTRY_NIMH:
						showLine("NIMH battery");
						break;
					case BATTERY_CHEMISTRY_LION:
						showLine("LION battery");
						break;
					case BATTERY_CHEMISTRY_LIPOLY:
						showLine("LIPOLY battery");
						break;
					default:
						showLine("Unknown battery chemistry");
						break;
				}
			}
			else
			{
				showLine("FAILURE: GetSystemPowerStatusEx2 failed");
			}
		}*/
	}
}
