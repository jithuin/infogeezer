// CalendarFrameWnd.cpp : implementation file
//

#include "stdafx.h"
#include "CalendarExt.h"
#include "CalendarExtResource.h"
#include "CalendarData.h"
#include "CalendarUtils.h"
#include "calendarprefsdlg.h"
#include "CalendarWnd.h"

#include "..\Shared\DialogHelper.h"
#include "..\Shared\DateHelper.h"
#include "..\Shared\TimeHelper.h"
#include "..\Shared\FileMisc.h"
#include "..\Shared\UITheme.h"
#include "..\Shared\themed.h"
#include "..\Shared\dlgunits.h"
#include "..\shared\VersionInfo.h"
#include "..\shared\IPreferences.h"
#include "..\shared\misc.h"

#include "..\todolist\tdcmsg.h"
#include "..\todolist\tdcenum.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define CALENDAR_INI_KEY_SHOWMINICAL		_T("ShowMiniCalendar")
#define CALENDAR_INI_KEY_SHOWSTATUSBAR		_T("ShowStatusBar")
#define CALENDAR_INI_KEY_SHOWWEEKENDS		_T("ShowWeekends")
#define CALENDAR_INI_KEY_NUMWEEKS			_T("NumWeeks")
#define CALENDAR_INI_KEY_WINDOWSIZE			_T("WindowSize")
#define CALENDAR_INI_KEY_COMPLETEDTASKS		_T("CompletedTasks")
#define CALENDAR_INI_KEY_DISPLAYFLAGS		_T("CompletedTasks")
#define CALENDAR_INI_KEY_CELLFONTSIZE		_T("CellFontSize")

/////////////////////////////////////////////////////////////////////////////
// CCalendarWnd

IMPLEMENT_DYNAMIC(CCalendarWnd, CWnd)

CCalendarWnd::CCalendarWnd()
:	m_hParentOfFrame(NULL),
	m_pCalendarData(NULL),
	m_bShowMiniCalendar(TRUE),
	m_bShowStatusBar(FALSE),
	m_bShowWeekends(TRUE),
	m_nNumVisibleWeeks(6),
	m_nCellFontSize(8),
	m_dwDisplayFlags(DISPLAY_DEFAULT),
	m_hIcon(NULL)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_CALENDAR);
	m_pCalendarData = new CCalendarData;
}

CCalendarWnd::~CCalendarWnd()
{
	delete m_pCalendarData;
}


BEGIN_MESSAGE_MAP(CCalendarWnd, CWnd)
	//{{AFX_MSG_MAP(CCalendarWnd)
	ON_WM_SETFOCUS()
	ON_WM_SIZE()
	ON_WM_CREATE()
	ON_WM_ERASEBKGND()
	ON_COMMAND(ID_VIEW_MINICALENDAR, OnViewMiniCalendar)
	ON_UPDATE_COMMAND_UI(ID_VIEW_MINICALENDAR, OnUpdateViewMiniCalendar)
	ON_COMMAND(ID_VIEW_WEEKENDS, OnViewWeekends)
	ON_UPDATE_COMMAND_UI(ID_VIEW_WEEKENDS, OnUpdateViewWeekends)
	ON_COMMAND(ID_VIEW_PREFS, OnViewPrefs)
	ON_UPDATE_COMMAND_UI(ID_VIEW_PREFS, OnUpdateViewPrefs)
	ON_COMMAND(ID_VIEW_NUMWEEKS_1, OnViewNumWeeks1)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NUMWEEKS_1, OnUpdateViewNumWeeks1)
	ON_COMMAND(ID_VIEW_NUMWEEKS_2, OnViewNumWeeks2)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NUMWEEKS_2, OnUpdateViewNumWeeks2)
	ON_COMMAND(ID_VIEW_NUMWEEKS_3, OnViewNumWeeks3)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NUMWEEKS_3, OnUpdateViewNumWeeks3)
	ON_COMMAND(ID_VIEW_NUMWEEKS_4, OnViewNumWeeks4)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NUMWEEKS_4, OnUpdateViewNumWeeks4)
	ON_COMMAND(ID_VIEW_NUMWEEKS_5, OnViewNumWeeks5)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NUMWEEKS_5, OnUpdateViewNumWeeks5)
	ON_COMMAND(ID_VIEW_NUMWEEKS_6, OnViewNumWeeks6)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NUMWEEKS_6, OnUpdateViewNumWeeks6)
	ON_COMMAND(ID_VIEW_NUMWEEKS_7, OnViewNumWeeks7)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NUMWEEKS_7, OnUpdateViewNumWeeks7)
	ON_COMMAND(ID_VIEW_NUMWEEKS_8, OnViewNumWeeks8)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NUMWEEKS_8, OnUpdateViewNumWeeks8)
	ON_COMMAND(ID_VIEW_NUMWEEKS_9, OnViewNumWeeks9)
	ON_UPDATE_COMMAND_UI(ID_VIEW_NUMWEEKS_9, OnUpdateViewNumWeeks9)
	ON_COMMAND(ID_GOTOTODAY, OnGoToToday)
	ON_UPDATE_COMMAND_UI(ID_GOTOTODAY, OnUpdateGoToToday)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCalendarWnd message handlers

BOOL CCalendarWnd::Create(DWORD dwStyle, const RECT &rect, CWnd* pParentWnd, UINT nID)
{
	return CWnd::CreateEx(0, NULL, NULL, dwStyle, rect, pParentWnd, nID);
}

void CCalendarWnd::SavePreferences(IPreferences* pPrefs, LPCTSTR szKey) const 
{
	pPrefs->WriteProfileInt(szKey, CALENDAR_INI_KEY_SHOWMINICAL, m_bShowMiniCalendar);
	pPrefs->WriteProfileInt(szKey, CALENDAR_INI_KEY_SHOWWEEKENDS, m_bShowWeekends);
	pPrefs->WriteProfileInt(szKey, CALENDAR_INI_KEY_NUMWEEKS, m_nNumVisibleWeeks);
	pPrefs->WriteProfileInt(szKey, CALENDAR_INI_KEY_DISPLAYFLAGS, m_dwDisplayFlags);
	pPrefs->WriteProfileInt(szKey, CALENDAR_INI_KEY_CELLFONTSIZE, m_nCellFontSize);
}

void CCalendarWnd::LoadPreferences(const IPreferences* pPrefs, LPCTSTR szKey, BOOL bAppOnly) 
{
	// app preferences
	// TODO

	// calendar specific preferences
	if (!bAppOnly)
	{
		m_bShowMiniCalendar = pPrefs->GetProfileInt(szKey, CALENDAR_INI_KEY_SHOWMINICAL, m_bShowMiniCalendar);
		m_bShowWeekends = pPrefs->GetProfileInt(szKey, CALENDAR_INI_KEY_SHOWWEEKENDS, m_bShowWeekends);
		m_nNumVisibleWeeks = pPrefs->GetProfileInt(szKey, CALENDAR_INI_KEY_NUMWEEKS, m_nNumVisibleWeeks);
		m_nCellFontSize = pPrefs->GetProfileInt(szKey, CALENDAR_INI_KEY_CELLFONTSIZE, m_nCellFontSize);
		m_dwDisplayFlags = pPrefs->GetProfileInt(szKey, CALENDAR_INI_KEY_DISPLAYFLAGS, m_dwDisplayFlags);

		// make sure 'today' is visible
		m_BigCalendar.SelectDate(COleDateTime::GetCurrentTime());
	}
}

void CCalendarWnd::SetUITheme(const UITHEME* /*pTheme*/)
{
}

LPCTSTR CCalendarWnd::GetMenuText() const
{
	return _T("Calendar");
}

LPCTSTR CCalendarWnd::GetTypeID() const
{
	static CString sID;

	Misc::GuidToString(CAL_TYPEID, sID); 

	return sID;
}

bool CCalendarWnd::ProcessMessage(MSG* pMsg) 
{
	if (!IsWindowEnabled())
		return false;

	// process editing shortcuts
	// TODO

	return false;
}

void CCalendarWnd::DoAppCommand(IUI_APPCOMMAND nCmd) 
{ 
	// do nothing for now 
}

bool CCalendarWnd::CanDoAppCommand(IUI_APPCOMMAND nCmd) const 
{ 
	// do nothing for now 
	return false;
}

bool CCalendarWnd::GetLabelEditRect(LPRECT pEdit) const
{
	if (m_BigCalendar.GetSelectedTaskEditRect(pEdit))
	{
		// convert to our coordinates
		m_BigCalendar.ClientToScreen(pEdit);
		ScreenToClient(pEdit);
		::InflateRect(pEdit, 2, 2);
		return true;
	}

	// else
	return false;
}

void CCalendarWnd::SelectTask(DWORD dwTaskID)
{
	// TODO
}

bool CCalendarWnd::SelectTasks(DWORD* pdwTaskIDs, int nTaskCount)
{
	return false; // only support single selection
}

bool CCalendarWnd::WantUpdate(int nAttribute) const
{
	switch (nAttribute)
	{
	case TDCA_TASKNAME:
	case TDCA_DONEDATE:
	case TDCA_DUEDATE:
	case TDCA_STARTDATE:
	case TDCA_PERCENT:
	case TDCA_TIMEEST:
	case TDCA_TIMESPENT:
		return true;
	}

	// all else 
	return false;
}

void CCalendarWnd::UpdateTasks(const ITaskList* pTasks, IUI_UPDATETYPE nUpdate, int nEditAttribute)
{
	m_strTasklistName = pTasks->GetProjectName();
	UpdateTitleBarText();

	if (m_pCalendarData->TasklistUpdated(pTasks, nUpdate, nEditAttribute))
	{
		//notify bigcal that data has been updated
		m_BigCalendar.TasklistUpdated();

		//notify minical that data has been updated
		m_MiniCalendar.TasklistUpdated();
	}
	else
	{
		// TODO
	}
}

void CCalendarWnd::Release()
{
	if (GetSafeHwnd())
		DestroyWindow();
	
	delete this;
}

void CCalendarWnd::OnSetFocus(CWnd* pOldWnd)
{
	//set focus to big calendar (seems the focus gets lost after switching back to the Calendar window from another app)
	m_BigCalendar.SetFocus();
}

void CCalendarWnd::OnSize(UINT nType, int cx, int cy) 
{
	CWnd::OnSize(nType, cx, cy);

	if (m_BigCalendar.GetSafeHwnd())
		ResizeControls(cx, cy);
}

int CCalendarWnd::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CWnd::OnCreate(lpCreateStruct) == -1)
		return -1;

	//set the title of the window
	UpdateTitleBarText();

	//set the pointer to the data object
	m_MiniCalendar.Initialize(this, m_pCalendarData);
	m_BigCalendar.Initialize(this, m_pCalendarData, m_hParentOfFrame);

	//create mini-calendar ctrl
	m_MiniCalendar.Create(NULL, NULL, WS_CHILD | WS_VISIBLE, CRect(0,0,0,0), this, 1);

	//create big-calendar ctrl
	m_BigCalendar.Create(WS_CHILD | WS_VISIBLE, CRect(0,0,0,0), this, 1);

	m_dlgBtns.Create(this);
	m_penBorder.CreatePen(PS_SOLID, 1, GetSysColor(COLOR_3DSHADOW));

	return 0;
}

BOOL CCalendarWnd::OnEraseBkgnd(CDC* pDC)
{
	// clip out children
	CDialogHelper::ExcludeChild(this, pDC, &m_dlgBtns);
	CDialogHelper::ExcludeChild(this, pDC, &m_MiniCalendar);
	CDialogHelper::ExcludeChild(this, pDC, &m_BigCalendar);

	// fill what's left
	CRect rClient;
	GetClientRect(rClient);

	pDC->FillSolidRect(rClient, GetSysColor(COLOR_WINDOW));

	// draw frame in gray
	CPen* pOldPen = pDC->SelectObject(&m_penBorder);
	pDC->Rectangle(rClient);
	pDC->SelectObject(pOldPen);

	return TRUE;
}

void CCalendarWnd::OnViewMiniCalendar()
{
	m_bShowMiniCalendar = !m_bShowMiniCalendar;

	//show/hide the minicalendar as necessary
	if (m_bShowMiniCalendar)
	{
		m_MiniCalendar.ShowWindow(SW_SHOW);
	}
	else
	{
		m_MiniCalendar.ShowWindow(SW_HIDE);
	}

	//call ResizeControls which will resize both calendars
	CRect rcFrame;
	GetClientRect(rcFrame);
	ResizeControls(rcFrame.Width(), rcFrame.Height());
}
void CCalendarWnd::OnUpdateViewMiniCalendar(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);

	if (m_bShowMiniCalendar)
	{
		pCmdUI->SetCheck(1);
	}
	else
	{
		pCmdUI->SetCheck(0);
	}
}

void CCalendarWnd::OnViewWeekends() 
{
	m_bShowWeekends = !m_bShowWeekends;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();

	//call ResizeControls which will resize both calendars
	CRect rcFrame;
	GetClientRect(rcFrame);
	ResizeControls(rcFrame.Width(), rcFrame.Height());
	Invalidate();
}

void CCalendarWnd::OnUpdateViewWeekends(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_bShowWeekends);
}

void CCalendarWnd::OnViewPrefs()
{
	//show dialog
	CCalendarPrefsDlg dlg(m_dwDisplayFlags, m_nCellFontSize);

	if (dlg.DoModal() == IDOK)
	{
		if (dlg.GetFlags() != m_dwDisplayFlags || dlg.GetCellFontSize() != m_nCellFontSize)
		{
			m_dwDisplayFlags = dlg.GetFlags();
			m_nCellFontSize = dlg.GetCellFontSize();

			//repopulate the calendars
			m_MiniCalendar.Repopulate();
			m_BigCalendar.Repopulate(m_nCellFontSize);
		}
	}

	//set focus to big calendar (seems the focus gets lost after the dialog is shown)
	m_BigCalendar.SetFocus();
}

void CCalendarWnd::OnUpdateViewPrefs(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
}

void CCalendarWnd::OnViewNumWeeks1()
{
	m_nNumVisibleWeeks = 1;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();
}

void CCalendarWnd::OnUpdateViewNumWeeks1(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_nNumVisibleWeeks == 1);
}

void CCalendarWnd::OnViewNumWeeks2()
{
	m_nNumVisibleWeeks = 2;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();
}

void CCalendarWnd::OnUpdateViewNumWeeks2(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_nNumVisibleWeeks == 2);
}

void CCalendarWnd::OnViewNumWeeks3()
{
	m_nNumVisibleWeeks = 3;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();
}

void CCalendarWnd::OnUpdateViewNumWeeks3(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_nNumVisibleWeeks == 3);
}

void CCalendarWnd::OnViewNumWeeks4()
{
	m_nNumVisibleWeeks = 4;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();
}

void CCalendarWnd::OnUpdateViewNumWeeks4(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_nNumVisibleWeeks == 4);
}

void CCalendarWnd::OnViewNumWeeks5()
{
	m_nNumVisibleWeeks = 5;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();
}

void CCalendarWnd::OnUpdateViewNumWeeks5(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_nNumVisibleWeeks == 5);
}

void CCalendarWnd::OnViewNumWeeks6()
{
	m_nNumVisibleWeeks = 6;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();
}
void CCalendarWnd::OnUpdateViewNumWeeks6(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_nNumVisibleWeeks == 6);
}

void CCalendarWnd::OnViewNumWeeks7()
{
	m_nNumVisibleWeeks = 7;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();
} 

void CCalendarWnd::OnUpdateViewNumWeeks7(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_nNumVisibleWeeks == 7);
}

void CCalendarWnd::OnViewNumWeeks8()
{
	m_nNumVisibleWeeks = 8;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();
} 

void CCalendarWnd::OnUpdateViewNumWeeks8(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_nNumVisibleWeeks == 8);
}

void CCalendarWnd::OnViewNumWeeks9()
{
	m_nNumVisibleWeeks = 9;

	//repopulate the calendars
	m_MiniCalendar.Repopulate();
	m_BigCalendar.Repopulate();
} 

void CCalendarWnd::OnUpdateViewNumWeeks9(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
	pCmdUI->SetCheck(m_nNumVisibleWeeks == 9);
}

void CCalendarWnd::OnGoToToday()
{
	m_MiniCalendar.SelectToday();
} 

void CCalendarWnd::OnUpdateGoToToday(CCmdUI* pCmdUI)
{
	pCmdUI->Enable(TRUE);
}

void CCalendarWnd::ResizeControls(int cx, int cy)
{
	CRect rClient(0, 0, cx, cy);
	rClient.DeflateRect(1, 1);

	int nMiniCalWidth = (m_bShowMiniCalendar ? m_MiniCalendar.GetMinWidth() : 0);

	// status bar
	CRect rItem(rClient);

	// buttons
	CRect rBtns;
	m_dlgBtns.GetClientRect(rBtns);

	rItem.right = rItem.left + rBtns.Width();
	rItem.top = rItem.bottom - rBtns.Height();

	m_dlgBtns.MoveWindow(rItem);

	// mini-cal
	rItem.right = rItem.left + nMiniCalWidth;
	rItem.bottom = rItem.top;
	rItem.top = rClient.top;

	m_MiniCalendar.MoveWindow(rItem);

	// big-cal
	rItem.top = rClient.top;
	rItem.bottom = rClient.bottom;
	rItem.left = max(rBtns.Width(), nMiniCalWidth);
	rItem.right = rClient.right;

	m_BigCalendar.MoveWindow(rItem);

	Invalidate(TRUE);
}

BOOL CCalendarWnd::OnNotify(WPARAM wParam,
								 LPARAM lParam,
								 LRESULT* pResult) 
{
	NMHDR* pNotifyArea = (NMHDR*)lParam;

	if (pNotifyArea->code == CALENDAR_MSG_SELECTDATE)
	{
		if (pNotifyArea->hwndFrom == m_MiniCalendar.GetSafeHwnd())
		{
			//send SelectDate message to BigCalendar
			COleDateTime dt;
			m_MiniCalendar.GetSelectedDate(dt);
			m_BigCalendar.SelectDate(dt);

			//set focus to big calendar
			m_BigCalendar.SetFocus();
		}
		else if (pNotifyArea->hwndFrom == m_BigCalendar.GetSafeHwnd())
		{
			//send SelectDate message to MiniCalendar (even if hidden)
			COleDateTime dt;
			m_BigCalendar.GetSelectedDate(dt);
			m_MiniCalendar.SelectDate(dt);
		}
		else
		{
			//ASSERT(FALSE);	//who sent it then?
		}
	}
	else if (pNotifyArea->code == CALENDAR_MSG_SELECTTASK)
	{
		AfxGetMainWnd()->SendMessage(WM_TDCM_TASKLINK, m_BigCalendar.GetSelectedTask(), 0L);
	}
	else if (pNotifyArea->code == CALENDAR_MSG_MOUSEWHEEL_UP)
	{
		m_BigCalendar.OnMouseWheel(0, 1, CPoint(0,0));
	}
	else if (pNotifyArea->code == CALENDAR_MSG_MOUSEWHEEL_DOWN)
	{
		m_BigCalendar.OnMouseWheel(0, -1, CPoint(0,0));
	}
	else if (pNotifyArea->code == NM_CLICK)
	{
		if (pNotifyArea->hwndFrom == m_MiniCalendar.GetSafeHwnd())
		{
			//set focus to big calendar
			m_BigCalendar.SetFocus();
		}
		else if (pNotifyArea->hwndFrom == m_BigCalendar.GetSafeHwnd())
		{
			//not bothered
		}
		else
		{
			//ASSERT(FALSE);	//who sent it then?
		}
	}
	return CWnd::OnNotify(wParam, lParam, pResult);
}

DWORD CCalendarWnd::GetDisplayFlags() const
{
	return m_dwDisplayFlags;
}

int CCalendarWnd::GetNumWeeksToDisplay() const
{
	int nReturn = 6;
	if ((m_nNumVisibleWeeks > 0) && (m_nNumVisibleWeeks < 10))
	{
		nReturn = m_nNumVisibleWeeks;
	}
	else
	{
		//ASSERT(FALSE);
	}
	return nReturn;
}

int CCalendarWnd::GetNumDaysToDisplay() const
{
	if (m_bShowWeekends)
	{
		return 7;
	}
	else
	{
		return 5;
	}
}

BOOL CCalendarWnd::IsWeekendsHidden() const
{
	return !m_bShowWeekends;
}

//if date is a weekend and weekends are hidden, returns TRUE
BOOL CCalendarWnd::IsDateHidden(const COleDateTime& _dt) const
{
	if (!m_bShowWeekends)
	{
		int nDayOfWeek = _dt.GetDayOfWeek();
		if ((nDayOfWeek == 1) || (nDayOfWeek == 7)) //7=sat 1=sun
		{
			return TRUE;
		}
	}
	return FALSE;
}

void CCalendarWnd::UpdateTitleBarText()
{
	CString strAppName;
	strAppName.LoadString(IDR_CALENDAR);

	CString strWindowText(strAppName);

	if (!m_strTasklistName.IsEmpty())
		strWindowText.Format(_T("%s - %s"), m_strTasklistName, strAppName);

	SetWindowText(strWindowText);
}

void CCalendarWnd::ZoomIn(BOOL bZoomIn)
{
	int nVisWeeks = m_nNumVisibleWeeks;
	
	if (bZoomIn)
	{
		nVisWeeks--;
		nVisWeeks = max(1, nVisWeeks);
	}
	else
	{
		nVisWeeks++;
		nVisWeeks = min(9, nVisWeeks);
	}
	
	if (nVisWeeks != m_nNumVisibleWeeks)
	{
		m_nNumVisibleWeeks = nVisWeeks;
		m_MiniCalendar.Repopulate();
		m_BigCalendar.Repopulate();
	}
}
