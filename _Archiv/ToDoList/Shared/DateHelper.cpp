// DateHelper.cpp: implementation of the CDateHelper class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "DateHelper.h"
#include "TimeHelper.h"
#include "misc.h"

#include <math.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

DWORD CDateHelper::s_dwWeekend = (DHW_SATURDAY | DHW_SUNDAY);

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

BOOL CDateHelper::IsValidDayInMonth(int nDay, int nMonth, int nYear)
{
	return (nMonth >= 1 && nMonth <= 12) &&
			(nDay >= 1 && nDay <= GetDaysInMonth(nMonth, nYear));
}

BOOL CDateHelper::IsValidDayOfMonth(int nDOW, int nWhich, int nMonth)
{
	return (nWhich >= 1 && nWhich <= 5) &&
			(nDOW >= 1 && nDOW <= 7) &&
			(nMonth >= 1 && nMonth <= 12);
}

int CDateHelper::CalcDaysFromTo(const COleDateTime& dateFrom, const COleDateTime& dateTo, BOOL bInclusive)
{
	int nDiff = (int)(floor(dateTo) - floor(dateFrom));

	return nDiff + (bInclusive ? 1 : 0);
}

int CDateHelper::CalcDaysFromTo(const COleDateTime& dateFrom, DH_DATE nTo, BOOL bInclusive)
{
	return CalcDaysFromTo(dateFrom, GetDate(nTo), bInclusive);
}

int CDateHelper::CalcDaysFromTo(DH_DATE nFrom, DH_DATE nTo, BOOL bInclusive)
{
	ASSERT (nFrom <= nTo);

	if (nFrom > nTo)
		return 0;
	
	else if (nFrom == nTo)
		return bInclusive ? 1 : 0;

	// else
	return CalcDaysFromTo(GetDate(nFrom), GetDate(nTo), bInclusive);
}

BOOL CDateHelper::DecodeDate(const CString& sDate, double& date, BOOL bAndTime)
{
	COleDateTime dt;

	if (DecodeDate(sDate, dt, bAndTime))
	{
		date = dt.m_dt;
		return TRUE;
	}

	// else
	return FALSE;
}

BOOL CDateHelper::DecodeRelativeDate(const CString& sDate, COleDateTime& date, BOOL bForceWeekday)
{
	CString sFiltered(sDate);
	sFiltered.MakeUpper();

	if (Misc::FilterString(sFiltered, _T("T+-1234567890DWMY")) == 0)
		return FALSE; // no valid chars

	int nSign = 1; // positive
	DH_UNITS nUnits = DHU_DAYS; // default
	CString sNumber;
	BOOL bDone = FALSE;

	// reset date
	date = GetDate(DHD_TODAY);
	
	for (int nPos = 0; !bDone && (nPos < sFiltered.GetLength()); nPos++)
	{
		TCHAR nChar = sFiltered[nPos];

		switch (nChar)
		{
		case 'T':
			// must be first char if it exists
			if (nPos != 0)
				return FALSE;
			else
				date = GetDate(DHD_TODAY);
			break;

		case '+':
			// must be first/second char if it exists
			if (nPos > 1)
				return FALSE;

			nSign = 1;
			break;

		case '-':
			// must be first/second char if it exists
			if (nPos > 1)
				return FALSE;

			nSign = -1;
			break;

		case DHU_DAYS:
			if (nPos == 0)
				date = GetDate(DHD_TODAY);
			else
				bDone = TRUE;

			nUnits = DHU_DAYS;
			break;

		case DHU_WEEKS:
			// if first char this defines start date
			if (nPos == 0)
				date = GetDate(DHD_ENDTHISWEEK);
			else
				bDone = TRUE;

			nUnits = DHU_WEEKS;			
			break;

		case DHU_MONTHS:
			// if first char this defines start date
			if (nPos == 0)
				date = GetDate(DHD_ENDTHISMONTH);
			else
				bDone = TRUE;

			nUnits = DHU_MONTHS;			
			break;

		case DHU_YEARS:
			// if first char this defines start date
			if (nPos == 0)
				date = GetDate(DHD_ENDTHISYEAR);
			else
				bDone = TRUE;

			nUnits = DHU_YEARS;			
			break;

		default: // must be numbers
			sNumber += nChar;
			break;
		}
	}

	// are we good to go?
	if (date.m_dt == 0 && sNumber.IsEmpty())
		return FALSE;

	if (!sNumber.IsEmpty())
		OffsetDate(date, nSign * _ttoi(sNumber), nUnits, bForceWeekday);

	return TRUE;
}

BOOL CDateHelper::DecodeDate(const CString& sDate, COleDateTime& date, BOOL bAndTime)
{
	if (date.ParseDateTime(sDate))
		return TRUE;

	if (DecodeISODate(sDate, date, bAndTime))
		return TRUE;

	// all else
	return DecodeLocalShortDate(sDate, date);
}

time_t CDateHelper::GetTimeT(const COleDateTime& date)
{
	tm time = { date.GetSecond(), 
				date.GetMinute(),
				date.GetHour(), 
				date.GetDay(),
				date.GetMonth() - 1, 
				date.GetYear() - 1900, 
				date.GetDayOfWeek() - 1, 
				date.GetDayOfYear(), 
				-1 };
	
	time_t t = mktime(&time);

	return (t == -1 ? 0 : t);
}

BOOL CDateHelper::DecodeDate(const CString& sDate, time_t& date, BOOL bAndTime)
{
	COleDateTime dt;

	if (DecodeDate(sDate, dt, bAndTime))
	{
		date = GetTimeT(dt);
		return (date > 0);
	}

	return FALSE;
}

BOOL CDateHelper::DecodeISODate(const CString& sDate, COleDateTime& date, BOOL bAndTime)
{
	int nYear = -1, nMonth = -1, nDay = -1, nHour = 0, nMin = 0, nSec = 0;
	const LPCTSTR DATETIME_FMT = _T("%4d-%2d-%2dT%2d:%2d:%2d");

#if _MSC_VER >= 1400
	if (_stscanf_s(sDate, DATETIME_FMT, &nYear, &nMonth, &nDay, &nHour, &nMin, &nSec) >= 3)
#else
	if (_stscanf(sDate, DATETIME_FMT, &nYear, &nMonth, &nDay, &nHour, &nMin, &nSec) >= 3)
#endif
	{
		if (bAndTime)
			return (date.SetDateTime(nYear, nMonth, nDay, nHour, nMin, nSec) == 0);
		else
			return (date.SetDate(nYear, nMonth, nDay) == 0);
	}
			
	return FALSE;
}

BOOL CDateHelper::DecodeLocalShortDate(const CString& sDate, COleDateTime& date)
{
	if (!sDate.IsEmpty())
	{
		// split the string and format by the delimiter
		CString sFormat = Misc::GetShortDateFormat();
		CStringArray aDateParts, aFmtParts;

		Misc::Split(sDate, aDateParts, TRUE, Misc::GetDateSeparator());
		Misc::Split(sFormat, aFmtParts, TRUE, Misc::GetDateSeparator());

		ASSERT (aDateParts.GetSize() == aFmtParts.GetSize());

		if (aDateParts.GetSize() != aFmtParts.GetSize())
			return FALSE;

		// process the parts, deciphering the format
		int nYear = -1, nMonth = -1, nDay = -1;

		for (int nPart = 0; nPart < aFmtParts.GetSize(); nPart++)
		{
			if (aFmtParts[nPart].FindOneOf(_T("Yy")) != -1)
				nYear = _ttoi(aDateParts[nPart]);

			if (aFmtParts[nPart].FindOneOf(_T("Mm")) != -1)
				nMonth = _ttoi(aDateParts[nPart]);

			if (aFmtParts[nPart].FindOneOf(_T("Dd")) != -1)
				nDay = _ttoi(aDateParts[nPart]);
		}

		if (nYear != -1 && nMonth != -1 && nDay != -1)
			return (date.SetDate(nYear, nMonth, nDay) == 0);
	}

	// else
	date.m_dt = 0.0;
	return FALSE;
}

double CDateHelper::GetDate(DH_DATE nDate)
{
	COleDateTime date;

	switch (nDate)
	{
	case DHD_TODAY:
		date = COleDateTime::GetCurrentTime();
		break;

	case DHD_TOMORROW:
		date = GetDate(DHD_TODAY) + 1;
		break;

	case DHD_ENDTHISWEEK:
		{
			// we must get the locale info to find out when this 
			// user's week starts
			date = COleDateTime::GetCurrentTime();
			
			// increment the date until we hit the last day of the week
			// note: we could have kept checking date.GetDayOfWeek but
			// it's a lot of calculation that's just not necessary
			int nLastDOW = GetLastDayOfWeek();
			int nDOW = date.GetDayOfWeek();
			
			while (nDOW != nLastDOW) 
			{
				date += 1;
				nDOW = GetNextDayOfWeek(nDOW);
			}
		}
		break;

	case DHD_ENDNEXTWEEK:
		return GetDate(DHD_ENDTHISWEEK) + 7;

	case DHD_ENDTHISMONTH:
		{
			date = COleDateTime::GetCurrentTime();
			int nThisMonth = date.GetMonth();

			while (date.GetMonth() == nThisMonth)
				date += 20; // much quicker than doing it one day at a time

			date -= date.GetDay(); // because we went into next month
		}
		break;

	case DHD_ENDNEXTMONTH:
		{
			date = GetDate(DHD_ENDTHISMONTH) + 1; // first day of next month
			int nNextMonth = date.GetMonth();

			while (date.GetMonth() == nNextMonth)
				date += 20; // much quicker than doing it one day at a time

			date -= date.GetDay(); // because we went into next month + 1
		}
		break;

	case DHD_ENDTHISYEAR:
		date = COleDateTime::GetCurrentTime(); // for current year
		date = COleDateTime(date.GetYear(), 12, 31, 0, 0, 0);
		break;

	case DHD_ENDNEXTYEAR:
		date = COleDateTime::GetCurrentTime(); // for current year
		date = COleDateTime(date.GetYear() + 1, 12, 31, 0, 0, 0);
		break;

	default:
		ASSERT (0);
		date.m_dt = 0;
		break;
	}

	return GetDateOnly(date);
}

BOOL CDateHelper::IsDateSet(const COleDateTime& date)
{
	return (date.m_status == COleDateTime::valid && date.m_dt > 0.0) ? TRUE : FALSE;
}

int CDateHelper::CalcWeekdaysFromTo(const COleDateTime& dateFrom, const COleDateTime& dateTo, BOOL bInclusive)
{
	int nWeekdays = 0;
	
	COleDateTime dFrom(floor(dateFrom.m_dt));
	COleDateTime dTo(floor(dateTo.m_dt));

	if (bInclusive)
		dTo += 1;

	int nDiff = (int)(double)(dTo - dFrom);

	if (nDiff > 0)
	{
		while (dFrom < dTo)
		{
			int nDOW = dFrom.GetDayOfWeek();

			if (!IsWeekend(nDOW))
				nWeekdays++;

			dFrom += 1;
		}
	}

	return nWeekdays;
}

int CDateHelper::GetISODayOfWeek(const COleDateTime& date) 
{
	int nDOW = date.GetDayOfWeek(); // 1=Sun, 2=Mon, ..., 7=Sat

	// ISO DOWs: 1=Mon, 2=Tue, ..., 7=Sun
	switch (nDOW)
	{
	case 1: /* sun */ return 7;
	case 2: /* mon */ return 1;
	case 3: /* tue */ return 2;
	case 4: /* wed */ return 3;
	case 5: /* thu */ return 4;
	case 6: /* fri */ return 5;
	case 7: /* sat */ return 6;
	}

	ASSERT (0);
	return 1;
}

int CDateHelper::GetFirstDayOfWeek()
{
	TCHAR szFDW[3]; // 2 + NULL

	::GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_IFIRSTDAYOFWEEK, szFDW, 2);

	int nFirstDOW = _ttoi(szFDW);

	// we need days to have same order as COleDateTime::GetDayOfWeek()
	// which is 1 (sun) - 7 (sat)
	switch (nFirstDOW)
	{
	case 0: /* mon */ return 2;
	case 1: /* tue */ return 3;
	case 2: /* wed */ return 4;
	case 3: /* thu */ return 5;
	case 4: /* fri */ return 6;
	case 5: /* sat */ return 7;
	case 6: /* sun */ return 1;
	}

	ASSERT (0);
	return 1;
}

int CDateHelper::GetLastDayOfWeek()
{
	switch (GetFirstDayOfWeek())
	{
	case 2: /* mon */ return 1; // sun
	case 3: /* tue */ return 2; // mon
	case 4: /* wed */ return 3; // tue
	case 5: /* thu */ return 4; // wed
	case 6: /* fri */ return 5; // thu
	case 7: /* sat */ return 6; // fri
	case 1: /* sun */ return 7; // sat
	}

	ASSERT (0);
	return 1;
}

int CDateHelper::GetNextDayOfWeek(int nDOW)
{
	switch (nDOW)
	{
	case 2: /* mon */ return 3; // tue
	case 3: /* tue */ return 4; // wed
	case 4: /* wed */ return 5; // thu
	case 5: /* thu */ return 6; // fri
	case 6: /* fri */ return 7; // sat
	case 7: /* sat */ return 1; // sun
	case 1: /* sun */ return 2; // mon
	}

	ASSERT (0);
	return 1;
}

void CDateHelper::SetWeekendDays(DWORD dwDays)
{
	s_dwWeekend = dwDays;
}

BOOL CDateHelper::IsWeekend(const COleDateTime& date)
{
	return IsWeekend(date.GetDayOfWeek());
}

BOOL CDateHelper::IsWeekend(int nDOW)
{
	switch (nDOW)
	{
	case 1:	return (s_dwWeekend & DHW_SUNDAY);
	case 2: return (s_dwWeekend & DHW_MONDAY);
	case 3: return (s_dwWeekend & DHW_TUESDAY);
	case 4: return (s_dwWeekend & DHW_WEDNESDAY);
	case 5: return (s_dwWeekend & DHW_THURSDAY);
	case 6: return (s_dwWeekend & DHW_FRIDAY);
	case 7: return (s_dwWeekend & DHW_SATURDAY);
	}

	ASSERT (0);
	return FALSE;
}

int CDateHelper::GetWeekendDuration()
{
	int nDuration = 0;

	for (int nDOW = 1; nDOW <= 7; nDOW)
	{
		if (IsWeekend(nDOW))
			nDuration++;
	}

	return nDuration;
}

CString CDateHelper::FormatDate(const COleDateTime& date, DWORD dwFlags)
{
	CString sDate, sTime, sDow;
	
	if (FormatDate(date, dwFlags, sDate, sTime, sDow))
	{
		if (!sDow.IsEmpty())
		{
			sDate = ' ' + sDate;
			sDate = sDow + sDate;
		}

		if (!sTime.IsEmpty())
		{
			if (!(dwFlags & DHFD_ISO))
				sDate += ' ';

			sDate += sTime;
		}
	}

	return sDate;
}

CString CDateHelper::FormatCurrentDate(DWORD dwFlags)
{
	return FormatDate(COleDateTime::GetCurrentTime(), dwFlags);
}

BOOL CDateHelper::FormatDate(const COleDateTime& date, DWORD dwFlags, CString& sDate, CString& sTime, CString& sDow)
{
	if (date.m_dt == 0.0)
		return FALSE;

	SYSTEMTIME st;

	if (!date.GetAsSystemTime(st))
		return FALSE;

	// Date
	CString sFormat;

	if (dwFlags & DHFD_ISO)
		sFormat = "yyyy-MM-dd";
	else
		sFormat = Misc::GetShortDateFormat();

	::GetDateFormat(0, 0, &st, sFormat, sDate.GetBuffer(50), 49);
	sDate.ReleaseBuffer();

	// Day of week
	if (dwFlags & DHFD_DOW)
		sDow = GetWeekdayName(st.wDayOfWeek + 1, TRUE);

	// Time
	if (dwFlags & DHFD_TIME)
	{
		if (dwFlags & DHFD_ISO)
			sTime = _T("T") + CTimeHelper::FormatISOTime(st.wHour, st.wMinute, st.wSecond, !(dwFlags & DHFD_NOSEC));
		else 
			sTime = CTimeHelper::Format24HourTime(st.wHour, st.wMinute, st.wSecond, !(dwFlags & DHFD_NOSEC));
	}
	else 
		sTime.Empty();
	
	return TRUE;
}

BOOL CDateHelper::FormatCurrentDate(DWORD dwFlags, CString& sDate, CString& sTime, CString& sDow)
{
	return FormatDate(COleDateTime::GetCurrentTime(), dwFlags, sDate, sTime, sDow);
}

CString CDateHelper::GetWeekdayName(int nWeekday, BOOL bShort)
{
	LCTYPE lct = bShort ? LOCALE_SABBREVDAYNAME1 : LOCALE_SDAYNAME1;
	CString sWeekday;

	// data check
	if (nWeekday < 1 || nWeekday> 7)
		return "";

	switch (nWeekday)
	{
	case 1: lct = bShort ? LOCALE_SABBREVDAYNAME7 : LOCALE_SDAYNAME7; break; // sun
	case 2:	lct = bShort ? LOCALE_SABBREVDAYNAME1 : LOCALE_SDAYNAME1; break; // mon
	case 3:	lct = bShort ? LOCALE_SABBREVDAYNAME2 : LOCALE_SDAYNAME2; break; // tue
	case 4:	lct = bShort ? LOCALE_SABBREVDAYNAME3 : LOCALE_SDAYNAME3; break; // wed
	case 5:	lct = bShort ? LOCALE_SABBREVDAYNAME4 : LOCALE_SDAYNAME4; break; // thu
	case 6:	lct = bShort ? LOCALE_SABBREVDAYNAME5 : LOCALE_SDAYNAME5; break; // fri
	case 7:	lct = bShort ? LOCALE_SABBREVDAYNAME6 : LOCALE_SDAYNAME6; break; // sat
	}
	
	GetLocaleInfo(LOCALE_USER_DEFAULT, lct, sWeekday.GetBuffer(30),	29);
	sWeekday.ReleaseBuffer();

	return sWeekday;
}

int CDateHelper::CalcLongestWeekdayName(CDC* pDC, BOOL bShort)
{
	int nLongestWDWidth = 0;
		
	// figure out the longest day in pixels
	for (int nWD = 1; nWD <= 7; nWD++)
	{
		int nWDWidth = pDC->GetTextExtent(GetWeekdayName(nWD, bShort)).cx;
		nLongestWDWidth = max(nLongestWDWidth, nWDWidth);
	}
	
	return nLongestWDWidth;
}

void CDateHelper::OffsetDate(COleDateTime& date, int nAmount, DH_UNITS nUnits, BOOL bForceWeekday)
{
	if (date.m_dt > 0)
	{
		switch (nUnits)
		{
		case DHU_DAYS:
			date += (double)nAmount;
			break;

		case DHU_WEEKS:
			date += nAmount * 7.0;
			break;

		case DHU_MONTHS:
			{
				SYSTEMTIME st;
				date.GetAsSystemTime(st);

				// are we at months end?
				int nDaysInMonth = GetDaysInMonth(st.wMonth, st.wYear);
				BOOL bEndOfMonth = (st.wDay == nDaysInMonth);

				// convert amount to years and months
				st.wYear = (WORD)((int)st.wYear + (nAmount / 12));
				st.wMonth = (WORD)((int)st.wMonth + (nAmount % 12));

				// handle overflow
				if (st.wMonth > 12)
				{
					st.wYear++;
					st.wMonth -= 12;
				}
				else if (st.wMonth < 1)
				{
					st.wYear--;
					st.wMonth += 12;
				}

				// if our start date was the end of the month make
				// sure out end date is too
				nDaysInMonth = GetDaysInMonth(st.wMonth, st.wYear);

				if (bEndOfMonth)
					st.wDay = (WORD)nDaysInMonth;

				else // clip dates to the end of the month
					st.wDay = min(st.wDay, (WORD)nDaysInMonth);

				// update time
				date = COleDateTime(st);
			}
			break;

		case DHU_YEARS:
			{
				SYSTEMTIME st;
				date.GetAsSystemTime(st);

				// update year
				st.wYear = (WORD)((int)st.wYear + nAmount);

				// update time
				date = COleDateTime(st);
			}
			break;
		}

		// does the caller only want weekdays
		if (bForceWeekday)
		{
			while (IsWeekend(date))
				date.m_dt += 1.0;
		}
	}
}

void CDateHelper::GetWeekdayNames(BOOL bShort, CStringArray& aWeekDays)
{
	aWeekDays.RemoveAll();

	for (int nDay = 1; nDay <= 7; nDay++)
		aWeekDays.Add(GetWeekdayName(nDay, bShort));
}

int CDateHelper::GetDaysInMonth(int nMonth, int nYear)
{
	// data check
	ASSERT(nMonth >= 1 && nMonth <= 12);

	if (nMonth < 1 || nMonth> 12)
		return 0;

	switch (nMonth)
	{
	case 1:  return 31; // jan
	case 2:  return IsLeapYear(nYear) ? 29 : 28; // feb
	case 3:  return 31; // mar
	case 4:  return 30; // apr
	case 5:  return 31; // may
	case 6:  return 30; // jun
	case 7:  return 31; // jul
	case 8:  return 31; // aug
	case 9:  return 30; // sep
	case 10: return 31; // oct
	case 11: return 30; // nov
	case 12: return 31; // dec
	}

	ASSERT(0);
	return 0;
}

BOOL CDateHelper::IsLeapYear(const COleDateTime& date)
{
	return IsLeapYear(date.GetYear());
}

BOOL CDateHelper::IsLeapYear(int nYear)
{
	return ((nYear % 4 == 0) && ((nYear % 100 != 0) || (nYear % 400 == 0)));
}

CString CDateHelper::GetMonthName(int nMonth, BOOL bShort)
{
	LCTYPE lct = LOCALE_SABBREVMONTHNAME1;
	CString sMonth;

	// data check
	if (nMonth < 1 || nMonth> 12)
		return "";

	switch (nMonth)
	{
	case 1:  lct = bShort ? LOCALE_SABBREVMONTHNAME1  : LOCALE_SMONTHNAME1;  break; // jan
	case 2:  lct = bShort ? LOCALE_SABBREVMONTHNAME2  : LOCALE_SMONTHNAME2;  break; // feb
	case 3:  lct = bShort ? LOCALE_SABBREVMONTHNAME3  : LOCALE_SMONTHNAME3;  break; // mar
	case 4:  lct = bShort ? LOCALE_SABBREVMONTHNAME4  : LOCALE_SMONTHNAME4;  break; // apr
	case 5:  lct = bShort ? LOCALE_SABBREVMONTHNAME5  : LOCALE_SMONTHNAME5;  break; // may
	case 6:  lct = bShort ? LOCALE_SABBREVMONTHNAME6  : LOCALE_SMONTHNAME6;  break; // jun
	case 7:  lct = bShort ? LOCALE_SABBREVMONTHNAME7  : LOCALE_SMONTHNAME7;  break; // jul
	case 8:  lct = bShort ? LOCALE_SABBREVMONTHNAME8  : LOCALE_SMONTHNAME8;  break; // aug
	case 9:  lct = bShort ? LOCALE_SABBREVMONTHNAME9  : LOCALE_SMONTHNAME9;  break; // sep
	case 10: lct = bShort ? LOCALE_SABBREVMONTHNAME10 : LOCALE_SMONTHNAME10; break; // oct
	case 11: lct = bShort ? LOCALE_SABBREVMONTHNAME11 : LOCALE_SMONTHNAME11; break; // nov
	case 12: lct = bShort ? LOCALE_SABBREVMONTHNAME12 : LOCALE_SMONTHNAME12; break; // dec
	}

	GetLocaleInfo(LOCALE_USER_DEFAULT, lct, sMonth.GetBuffer(30),	29);
	sMonth.ReleaseBuffer();

	return sMonth;
}

void CDateHelper::GetMonthNames(BOOL bShort, CStringArray& aMonths)
{
	aMonths.RemoveAll();

	for (int nMonth = 1; nMonth <= 12; nMonth++)
		aMonths.Add(GetMonthName(nMonth, bShort));
}

COleDateTime CDateHelper::GetTimeOnly(const COleDateTime& date)
{
	return (date.m_dt - GetDateOnly(date));
}

BOOL CDateHelper::DateHasTime(const COleDateTime& date)
{
	return (GetTimeOnly(date) > 0);
}

COleDateTime CDateHelper::GetDateOnly(const COleDateTime& date)
{
	return floor(date.m_dt);
}

void CDateHelper::SplitDate(const COleDateTime& date, double& dDateOnly, double& dTimeOnly)
{
	dDateOnly = GetDateOnly(date);
	dTimeOnly = GetTimeOnly(date);
}

COleDateTime CDateHelper::MakeDate(const COleDateTime& dtTimeOnly, const COleDateTime& dtDateOnly)
{
	double dDateOnly = GetDateOnly(dtDateOnly);
	double dTimeOnly = GetTimeOnly(dtTimeOnly);
	
	return dDateOnly + dTimeOnly;
}

int CDateHelper::CalcDayOfMonth(int nDOW, int nWhich, int nMonth, int nYear)
{
	// data check
	ASSERT(nMonth >= 1 && nMonth <= 12);
	ASSERT(nDOW >= 1 && nDOW <= 7);
	ASSERT(nWhich >= 1 && nWhich <= 5);

	if (nMonth < 1 || nMonth> 12 || nDOW < 1 || nDOW > 7 || nWhich < 1 || nWhich > 5)
		return -1;

	// start with first day of month
	int nDay = 1;
	COleDateTime date(nYear, nMonth, nDay, 0, 0, 0);

	// get it's day of week
	int nWeekDay = date.GetDayOfWeek();

	// move forwards until we hit the requested day of week
	while (nWeekDay != nDOW)
	{
		nDay++;
		nWeekDay = GetNextDayOfWeek(nWeekDay);
	}

	// add a week at a time
	nWhich--;

	while (nWhich--)
		nDay += 7;

	// if we've gone passed the end of the month
	// deduct a week
	if (nDay > GetDaysInMonth(nMonth, nYear))
		nDay -= 7;

	return nDay;
}

COleDateTime CDateHelper::CalcDate(int nDOW, int nWhich, int nMonth, int nYear)
{
	int nDay = CalcDayOfMonth(nDOW, nWhich, nMonth, nYear);

	if (nDay == -1)
		return COleDateTime((time_t)-1);

	return COleDateTime(nYear, nMonth, nDay, 0, 0, 0);
}

int CDateHelper::CalcWeekofYear(const COleDateTime& date)
{
	// http://en.wikipedia.org/wiki/ISO_week_date#Calculating_the_week_number_of_a_given_date
	//
	// Using ISO weekday numbers (running from 1 for Monday to 7 for Sunday), 
	// subtract the weekday from the ordinal date, then add 10. Divide the result by 7. 
	// Ignore the remainder; the quotient equals the week number.

	int nDayOfYear = date.GetDayOfYear();
	int nISODOW = GetISODayOfWeek(date);

	return (nDayOfYear - nISODOW + 10) / 7;
}

