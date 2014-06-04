// timecombobox.cpp : implementation file
//

#include "stdafx.h"
#include "timecombobox.h"
#include "timehelper.h"
#include "misc.h"
#include "localizer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTimeComboBox

CTimeComboBox::CTimeComboBox(DWORD dwStyle) : m_dwStyle(dwStyle)
{
}

CTimeComboBox::~CTimeComboBox()
{
}


BEGIN_MESSAGE_MAP(CTimeComboBox, CComboBox)
	//{{AFX_MSG_MAP(CTimeComboBox)
	ON_WM_CREATE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTimeComboBox message handlers

void CTimeComboBox::PreSubclassWindow() 
{
	CLocalizer::EnableTranslation(*this, FALSE);

	BuildCombo();
	
	// hook the edit ctrl so we can convert '.' and ',' to ':'
	CWnd* pEdit = GetDlgItem(1001);

	if (pEdit)
		ScHookWindow(pEdit->GetSafeHwnd());
	
	CComboBox::PreSubclassWindow();
}

int CTimeComboBox::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CComboBox::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	CLocalizer::EnableTranslation(*this, FALSE);

	BuildCombo();

	// hook the edit ctrl so we can convert '.' and ',' to ':'
	CWnd* pEdit = GetDlgItem(1001);

	if (pEdit)
		ScHookWindow(pEdit->GetSafeHwnd());
	
	return 0;
}

void CTimeComboBox::BuildCombo(BOOL bReset)
{
	CLocalizer::EnableTranslation(*this, FALSE);
	
	ASSERT(bReset || GetCount() == 0);
	
	if (!bReset && GetCount())
		return;
	
	ResetContent();
	
	for (int nHour = 0; nHour < 24; nHour++)
	{
		CString sTime;
		
		if ((m_dwStyle & TCB_NOTIME) && nHour == 0)
		{
			// add empty string representing 'no time'
		}
		else if (m_dwStyle & TCB_ISO)
			sTime = CTimeHelper::FormatISOTime(nHour, 0);
		else // regional settings
			sTime = CTimeHelper::Format24HourTime(nHour, 0);
		
		AddString(sTime);
		
		if (m_dwStyle & TCB_HALFHOURS)
		{
			if (m_dwStyle & TCB_ISO)
				sTime = CTimeHelper::FormatISOTime(nHour, 30);
			else // regional settings
				sTime = CTimeHelper::Format24HourTime(nHour, 30);
			
			AddString(sTime);
		}
	}
}

double CTimeComboBox::GetOleTime() const
{
	return (Get24HourTime() / 24.0);
}

BOOL CTimeComboBox::SetOleTime(double dTime)
{
	// truncate to extract the time only component if it has one
	dTime -= (int)dTime;

	if (dTime <= 0)
	{
		SetCurSel(0);
		return TRUE;
	}
	
	// else
	return Set24HourTime(dTime * 24.0);
}

void CTimeComboBox::SetStyle(DWORD dwStyle)
{
  BOOL bWasISO = (m_dwStyle & TCB_ISO);
  BOOL bIsISO = (dwStyle & TCB_ISO);

  m_dwStyle = dwStyle;

  if (bWasISO != bIsISO)
  {
    double date = GetOleTime();
    BuildCombo(TRUE);
    SetOleTime(date);
  }
}

double CTimeComboBox::Get24HourTime() const
{
	// is this a hack? I'm not sure
	// but we look at the current Windows message being handled and if it's a
	// combo sel change then we use the current selection else we use the
	// window text (it may be an edit change notification)
	const MSG* pMsg = CComboBox::GetCurrentMessage();

	if (pMsg->message == WM_COMMAND && pMsg->lParam == (LPARAM)GetSafeHwnd() &&
		HIWORD(pMsg->wParam) == CBN_SELCHANGE)
	{
		// since the items in the combo are ordered from 1am to 11pm
		// we can use the selection index as a direct link to the hour
		int nSel = GetCurSel();

		if (nSel <= 0) // 'no time'
			return 0;

		// else
		if (m_dwStyle & TCB_HALFHOURS)
			return (nSel * 0.5);
		else
			return nSel;
	}

	// else use window text
	double dTime = 0;
	CString sTime;
	GetWindowText(sTime);

	if (sTime.IsEmpty())
	{
		if (m_dwStyle & TCB_NOTIME)
			return 0; // midnight
		else
			return -1; // error
	}

	// look for a separator
	CString sSep = Misc::GetTimeSeparator();
	int nColon = sTime.Find(sSep);

	if (nColon != -1) // extract hours and minutes
		dTime = _ttof(sTime.Left(nColon)) + _ttof(sTime.Mid(nColon + sSep.GetLength())) / 60;

	// test for military time
	else if (Misc::IsNumber(sTime) && sTime.GetLength() >= 3)
	{
		// if only 3 digits have been typed, add a zero
		if (sTime.GetLength() == 3)
			sTime += '0';

		dTime = _ttof(sTime.Left(2)) + _ttof(sTime.Mid(2)) / 60;
	}
	else // simple number
		dTime = _ttof(sTime);

	// look for PM signifier
	CString sPM = Misc::GetPM();

	if (!sPM.IsEmpty() && dTime < 12)
	{
		sTime.MakeUpper();
		sPM.MakeUpper();
		
		if (sTime.Find(sPM) != -1)
			dTime += 12;
	}

	// truncate to 0-24
	if (dTime > 24)
		dTime = 24;

	else if (dTime < 0)
		dTime = 0;

	return dTime;
}

BOOL CTimeComboBox::Set24HourTime(double dTime)
{
	if (dTime < 0)
	{
		SetCurSel(0);
		return TRUE;
	}
	
	// else
	if (dTime >= 24.0)
		return FALSE;

	// add on half a minute to handle floating point inaccuracies
	dTime += (0.5 / (24 * 60));

	int nHour = (int)dTime;
	int nMin = (int)((dTime - nHour) * 60); // nearest 1 minute

	CString sTime;
  
  if (m_dwStyle & TCB_ISO)
    sTime = CTimeHelper::FormatISOTime(nHour, nMin);
  else
    sTime = CTimeHelper::Format24HourTime(nHour, nMin);

	if (SelectString(-1, sTime) == CB_ERR)
		SetWindowText(sTime);

	return TRUE;
}

LRESULT CTimeComboBox::ScWindowProc(HWND hRealWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
	case WM_CHAR:
		if (wParam == ',' || wParam == '.' && !Misc::GetTimeSeparator().IsEmpty())
		{
			CString sSep = Misc::GetTimeSeparator();
			return CSubclasser::ScWindowProc(hRealWnd, message, sSep[0], lParam);
		}
		break;
	}

	// else
	return ScDefault(hRealWnd);
}
