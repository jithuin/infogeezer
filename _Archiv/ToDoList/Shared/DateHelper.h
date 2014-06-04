// DateHelper.h: interface for the CDateHelper class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_DATEHELPER_H__2A4E63F6_A106_4295_BCBA_06D03CD67AE7__INCLUDED_)
#define AFX_DATEHELPER_H__2A4E63F6_A106_4295_BCBA_06D03CD67AE7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

enum DH_DATE
{
	DHD_TODAY,
	DHD_TOMORROW,
	DHD_ENDTHISWEEK, // start of week + 7
	DHD_ENDNEXTWEEK, // DHD_ENDTHISWEEK + 7
	DHD_ENDTHISMONTH, // beginning of next month - 1
	DHD_ENDNEXTMONTH, // get's trickier :)
	DHD_ENDTHISYEAR,
	DHD_ENDNEXTYEAR,
};

enum DH_UNITS
{
	DHU_DAYS	= 'D',
	DHU_WEEKS	= 'W',
	DHU_MONTHS	= 'M',
	DHU_YEARS	= 'Y',
};

enum
{
	DHFD_ISO	= 0x0001,
	DHFD_DOW	= 0x0002,
	DHFD_TIME	= 0x0004,
	DHFD_NOSEC	= 0x0008,
};

enum // days of week
{
	DHW_SUNDAY		= 0X01,
	DHW_MONDAY		= 0X02,
	DHW_TUESDAY		= 0X04,
	DHW_WEDNESDAY	= 0X08,
	DHW_THURSDAY	= 0X10,
	DHW_FRIDAY		= 0X20,
	DHW_SATURDAY	= 0X40,
};

// Note: 
// 1 <= nMonth <= 12
// 1 <= nDay <= 31

class CDateHelper  
{
public:
	static BOOL IsDateSet(const COleDateTime& date);

	static BOOL IsValidDayInMonth(int nDay, int nMonth, int nYear);
	static BOOL IsValidDayOfMonth(int nDOW, int nWhich, int nMonth);

	static int CalcDaysFromTo(const COleDateTime& dateFrom, const COleDateTime& dateTo, BOOL bInclusive);
	static int CalcDaysFromTo(const COleDateTime& dateFrom, DH_DATE nTo, BOOL bInclusive);
	static int CalcDaysFromTo(DH_DATE nFrom, DH_DATE nTo, BOOL bInclusive);
	static double GetDate(DH_DATE nDate); // 12am

	static int CalcWeekdaysFromTo(const COleDateTime& dateFrom, const COleDateTime& dateTo, BOOL bInclusive);
	static void OffsetDate(COleDateTime& date, int nAmount, DH_UNITS nUnits, BOOL bForceWeekday);

	static CString FormatDate(const COleDateTime& date, DWORD dwFlags = 0);
	static CString FormatCurrentDate(DWORD dwFlags = 0);

	static int GetISODayOfWeek(const COleDateTime& date);   // 1=Mon, 2=Tue, ..., 7=Sun

	// DOW = 'day of week'
	static BOOL FormatDate(const COleDateTime& date, DWORD dwFlags, CString& sDate, CString& sTime, CString& sDow);
	static BOOL FormatCurrentDate(DWORD dwFlags, CString& sDate, CString& sTime, CString& sDow);

	static BOOL DecodeDate(const CString& sDate, COleDateTime& date, BOOL bAndTime = FALSE);
	static BOOL DecodeDate(const CString& sDate, double& date, BOOL bAndTime = FALSE);
	static BOOL DecodeDate(const CString& sDate, time_t& date, BOOL bAndTime = FALSE);
	static BOOL DecodeRelativeDate(const CString& sDate, COleDateTime& date, BOOL bForceWeekday);

	static int GetFirstDayOfWeek();
	static int GetLastDayOfWeek();
	static int GetNextDayOfWeek(int nDOW);
	static int GetDaysInMonth(int nMonth, int nYear); 

	static COleDateTime CalcDate(int nDOW, int nWhich, int nMonth, int nYear);
	static int CalcDayOfMonth(int nDOW, int nWhich, int nMonth, int nYear);
	static int CalcWeekofYear(const COleDateTime& date);

	static CString GetWeekdayName(int nWeekday, BOOL bShort); // 1-7, sun-sat
	static CString GetMonthName(int nMonth, BOOL bShort); // 1-12, jan-nov
	static void GetWeekdayNames(BOOL bShort, CStringArray& aWeekDays); // sun-sat
	static void GetMonthNames(BOOL bShort, CStringArray& aMonths); // jan-dec
	static int CalcLongestWeekdayName(CDC* pDC, BOOL bShort);

	static BOOL IsLeapYear(const COleDateTime& date = COleDateTime::GetCurrentTime());
	static BOOL IsLeapYear(int nYear);

	static void SplitDate(const COleDateTime& date, double& dDateOnly, double& dTimeOnly);
	static COleDateTime MakeDate(const COleDateTime& dtTimeOnly, const COleDateTime& dtDateOnly);

	static BOOL DateHasTime(const COleDateTime& date);
	static COleDateTime GetTimeOnly(const COleDateTime& date);
	static COleDateTime GetDateOnly(const COleDateTime& date);

	static void SetWeekendDays(DWORD dwDays = DHW_SATURDAY | DHW_SUNDAY);
	static BOOL IsWeekend(int nDOW);
	static BOOL IsWeekend(const COleDateTime& date);
	static int GetWeekendDuration();

	static time_t GetTimeT(const COleDateTime& date);

protected:
	static DWORD s_dwWeekend; 

protected:
	static BOOL DecodeISODate(const CString& sDate, COleDateTime& date, BOOL bAndTime = FALSE);
	static BOOL DecodeLocalShortDate(const CString& sDate, COleDateTime& date);
};

#endif // !defined(AFX_DATEHELPER_H__2A4E63F6_A106_4295_BCBA_06D03CD67AE7__INCLUDED_)
