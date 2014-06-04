// TimeEdit.cpp: implementation of the CTimeEdit class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TimeEdit.h"
#include "stringres.h"
#include "misc.h"

#include <math.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//////////////////////////////////////////////////////////////////////

CString CTimeEdit::s_sUnitsBtnTip;

void CTimeEdit::SetDefaultButtonTip(LPCTSTR szUnits)
{
	if (szUnits && *szUnits)
		s_sUnitsBtnTip = szUnits;
}

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

const int BTNWIDTHDLU = 11;
const int LABELLEN = 30;

struct TIMEUNIT
{
	TCHAR nUnits;
	TCHAR szLabel[LABELLEN];
	TCHAR cAbbrLabel;
	UINT nMenuID;
};

enum
{
	ID_MINS = 0x8000,
	ID_HOURS,
	ID_DAYS,
	ID_WEEKS,
	ID_MONTHS,
	ID_YEARS,
};

static TIMEUNIT TIMEUNITS[] = 
{
	{ THU_MINS,		_T(""),	TIME_MIN_ABBREV,	ID_MINS },
	{ THU_HOURS,	_T(""),	TIME_HOUR_ABBREV,	ID_HOURS },
	{ THU_DAYS,		_T(""),	TIME_DAY_ABBREV,	ID_DAYS },
	{ THU_WEEKS,	_T(""),	TIME_WEEK_ABBREV,	ID_WEEKS },
	{ THU_MONTHS,	_T(""), TIME_MONTH_ABBREV,	ID_MONTHS },
	{ THU_YEARS,	_T(""),	TIME_YEAR_ABBREV,	ID_YEARS },
};

static LPCTSTR UNITLABELS[] = 
{
	TIME_MINS,	
	TIME_HOURS,	
	TIME_DAYS,	
	TIME_WEEKS,	
	TIME_MONTHS,
	TIME_YEARS	
};

const int NUM_UNITS = sizeof(TIMEUNITS) / sizeof (TIMEUNIT);

const TIMEUNIT& GetTimeUnit(TCHAR nUnits)
{
	int nItem = NUM_UNITS;

	while (nItem--)
	{
		if (TIMEUNITS[nItem].nUnits == nUnits)
			return TIMEUNITS[nItem];
	}

	return TIMEUNITS[0]; // hours
}

/////////////////////////////////////////////////////////////////////////////////////

CTimeEdit::CTimeEdit(TCHAR nUnits, int nMaxDecPlaces) : m_nUnits(nUnits), m_nMaxDecPlaces(nMaxDecPlaces)
{
	// init static units
	for (int nUnit = 0; nUnit < NUM_UNITS; nUnit++)
	{
		TIMEUNIT& tu = TIMEUNITS[nUnit];

		if (tu.szLabel[0] == 0) // empty string
		{
			//fabio_2005
#if _MSC_VER >= 1400
			_tcsncpy_s(tu.szLabel, LABELLEN, UNITLABELS[nUnit], LABELLEN - 1);
#else
			_tcsncpy(tu.szLabel, UNITLABELS[nUnit], LABELLEN - 1);
#endif

			tu.szLabel[LABELLEN - 1] = 0;
		}
	}

	SetMask(_T(".0123456789"), ME_LOCALIZEDECIMAL);

	CString sLabel;
	sLabel.Format(_T("%c"), GetTimeUnit(nUnits).cAbbrLabel);

	CString sTip(s_sUnitsBtnTip.IsEmpty() ? TIME_UNITS : s_sUnitsBtnTip);
	AddButton(TEBTN_UNITS, sLabel, sTip, CALC_BTNWIDTH);
	SetDropMenuButton(TEBTN_UNITS);
}

CTimeEdit::~CTimeEdit()
{

}

BEGIN_MESSAGE_MAP(CTimeEdit, CEnEdit)
	//{{AFX_MSG_MAP(CTimeEdit)
	//}}AFX_MSG_MAP
	ON_WM_CHAR()
END_MESSAGE_MAP()

void CTimeEdit::OnChar(UINT nChar, UINT nRepCnt, UINT nFlags) 
{
	// first check for exact match with units
	// then try again without case
	for (int bNoCase = 0; bNoCase <= 1; bNoCase++)
	{
		for (int nUnit = 0; nUnit < NUM_UNITS; nUnit++)
		{
			TIMEUNIT& tu = TIMEUNITS[nUnit];
			
			if ((bNoCase && toupper(tu.cAbbrLabel) == toupper(nChar)) ||
				(!bNoCase && tu.cAbbrLabel == nChar))
			{
				CheckSetUnits(tu.nUnits, FALSE);
				return;
			}
		}
	}
	
	CEnEdit::OnChar(nChar, nRepCnt, nFlags);
}

void CTimeEdit::CheckSetUnits(TCHAR nUnits, BOOL bQueryUnits)
{
	if (nUnits != m_nUnits)
	{
		// update the units
		TCHAR nPrevUnits = m_nUnits;
		SetUnits(nUnits);
		
		// inform parent and check for cancel
		if (bQueryUnits)
		{
			if (GetParent()->SendMessage(WM_TEN_UNITSCHANGE, (WPARAM)GetDlgCtrlID(), nPrevUnits))
				SetUnits(nPrevUnits);
		}
		else // fake a time change
		{
			GetParent()->SendMessage(WM_COMMAND, MAKEWPARAM(GetDlgCtrlID(), EN_CHANGE), (LPARAM)GetSafeHwnd());
		}
	}
}

void CTimeEdit::EnableNegativeTimes(BOOL bEnable)
{
	if (bEnable)
		SetMask(_T("-.0123456789"), ME_LOCALIZEDECIMAL);
	else
		SetMask(_T(".0123456789"), ME_LOCALIZEDECIMAL);
}

void CTimeEdit::PreSubclassWindow() 
{
	CEnEdit::PreSubclassWindow();

	CString sLabel;
	sLabel.Format(_T("%c"), GetTimeUnit(m_nUnits).cAbbrLabel);
	SetButtonCaption(1, sLabel);
}

double CTimeEdit::GetTime() const
{
	CString sTime;
	GetWindowText(sTime);
	return Misc::Atof(sTime);
}

void CTimeEdit::SetTime(double dTime)
{
	CString sTime = CTimeHelper::FormatTime(dTime, m_nMaxDecPlaces);
	RemoveTrailingZeros(sTime);

	SetWindowText(sTime);
}

void CTimeEdit::SetTime(double dTime, TCHAR nUnits)
{
	if (dTime != GetTime())
	{
		SetTime(dTime);
		SetUnits(nUnits);
	}
}

void CTimeEdit::SetUnits(TCHAR nUnits)
{
	if (nUnits != m_nUnits)
	{
		m_nUnits = nUnits;

		if (GetSafeHwnd())
		{
			CString sLabel;
			sLabel.Format(_T("%c"), GetTimeUnit(nUnits).cAbbrLabel);
			SetButtonCaption(1, sLabel);
		}
	}
}

void CTimeEdit::SetMaxDecimalPlaces(int nMaxDecPlaces)
{
	if (m_nMaxDecPlaces != nMaxDecPlaces)
	{
		m_nMaxDecPlaces = nMaxDecPlaces;

		SetTime(GetTime());
	}
}

void CTimeEdit::OnBtnClick(UINT nID)
{
	if (nID != TEBTN_UNITS)
		return;

	CMenu menu;
	
	if (menu.CreatePopupMenu())
	{			
		int nUnit = 0; 
		for (nUnit = 0; nUnit < NUM_UNITS; nUnit++)
		{
			const TIMEUNIT& tu = TIMEUNITS[nUnit];

			menu.AppendMenu(MF_STRING, tu.nMenuID, tu.szLabel);

			if (tu.nUnits == m_nUnits)
				menu.CheckMenuItem(nUnit, MF_CHECKED | MF_BYPOSITION);
		}
		
		UINT nCmdID = TrackPopupMenu(nID, &menu, FALSE);

		if (nCmdID <= 0)
			return; // cancelled

		// handle result
		for (nUnit = 0; nUnit < NUM_UNITS; nUnit++)
		{
			const TIMEUNIT& tu = TIMEUNITS[nUnit];

			if (tu.nMenuID == nCmdID)
			{
				// update the units
				CheckSetUnits(tu.nUnits, TRUE);
				break;
			}
		}
	}
}

double CTimeEdit::GetTime(TCHAR nUnits) const
{
	return CTimeHelper().GetTime(GetTime(), m_nUnits, nUnits);
}

CString CTimeEdit::FormatTime(BOOL bUnits) const
{
	return CTimeHelper().FormatTime(GetTime(), bUnits ? m_nUnits : 0, m_nMaxDecPlaces);
}

CString CTimeEdit::FormatTimeHMS() const
{
	return CTimeHelper().FormatTimeHMS(GetTime(), GetUnits(), TRUE); 
}

void CTimeEdit::OnSetReadOnly(BOOL bReadOnly)
{
	EnableButton(1, !bReadOnly && IsWindowEnabled());
}

void CTimeEdit::SetUnits(TCHAR nUnits, LPCTSTR szLongUnits, LPCTSTR szAbbrevUnits)
{
	for (int nUnit = 0; nUnit < NUM_UNITS; nUnit++)
	{
		TIMEUNIT& tu = TIMEUNITS[nUnit];

		if (tu.nUnits == nUnits)
		{
			if (szLongUnits && *szLongUnits)
			{
				//fabio_2005
#if _MSC_VER >= 1300
				_tcsncpy_s(tu.szLabel, LABELLEN, szLongUnits, LABELLEN - 1);
#else
				_tcsncpy(tu.szLabel, szLongUnits, LABELLEN - 1);
#endif

				tu.szLabel[LABELLEN - 1] = 0;
			}

			if (szAbbrevUnits && *szAbbrevUnits)
				tu.cAbbrLabel = szAbbrevUnits[0];
		}
	}

	// abbrev units
	CTimeHelper::SetUnits(nUnits, szAbbrevUnits);
}

void CTimeEdit::RemoveTrailingZeros(CString& sTime)
{
	sTime.TrimRight();

	while (!sTime.IsEmpty())
	{
		int nLen = sTime.GetLength();

		if (sTime[nLen - 1] == '0')
			sTime = sTime.Left(nLen - 1);

		else if (sTime[nLen - 1] == '.')
		{
			sTime = sTime.Left(nLen - 1);
			break;
		}
		else
			break;
	}
}
