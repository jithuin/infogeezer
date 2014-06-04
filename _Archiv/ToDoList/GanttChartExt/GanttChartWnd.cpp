// GanttChartWnd.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "GanttChartExt.h"
#include "GanttChartWnd.h"

#include "..\shared\misc.h"
#include "..\shared\themed.h"
#include "..\shared\graphicsmisc.h"
#include "..\shared\dialoghelper.h"
#include "..\shared\ipreferences.h"
#include "..\shared\datehelper.h"

#include "..\todolist\tdcenum.h"
#include "..\todolist\tdcmsg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

const COLORREF DEF_ALTLINECOLOR		= RGB(230, 230, 255);
const COLORREF DEF_GRIDLINECOLOR	= RGB(192, 192, 192);
const COLORREF DEF_DONECOLOR		= RGB(128, 128, 128);
const COLORREF NO_COLOR				= (COLORREF)-1;

#ifndef LVS_EX_DOUBLEBUFFER
#define LVS_EX_DOUBLEBUFFER 0x00010000
#endif

/////////////////////////////////////////////////////////////////////////////
// CGanttChartWnd

CGanttChartWnd::CGanttChartWnd(CWnd* pParent /*=NULL*/)
	: 
	CDialog(IDD_GANTTTREE_DIALOG, pParent), 
	m_hIcon(NULL)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_GANTTCHART);
}

CGanttChartWnd::~CGanttChartWnd()
{
}

void CGanttChartWnd::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CGanttChartWnd)
	DDX_Control(pDX, IDC_DIVIDER, m_stDivider);
	DDX_Control(pDX, IDC_GANTTLIST, m_list);
	DDX_Control(pDX, IDC_GANTTTREE, m_tree);
	DDX_CBIndex(pDX, IDC_DISPLAY, m_nDisplay);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_DISPLAY, m_cbDisplayOptions);
}

BEGIN_MESSAGE_MAP(CGanttChartWnd, CDialog)
	//{{AFX_MSG_MAP(CGanttChartWnd)
	ON_BN_CLICKED(IDC_GOTOTODAY, OnGototoday)
	ON_WM_SIZE()
	ON_WM_CTLCOLOR()
	ON_NOTIFY(TVN_KEYUP, IDC_GANTTTREE, OnKeyUpGantt)
	ON_CBN_SELCHANGE(IDC_DISPLAY, OnSelchangeDisplay)
	ON_NOTIFY(NM_CLICK, IDC_GANTTLIST, OnClickGanttList)
	ON_NOTIFY(TVN_SELCHANGED, IDC_GANTTTREE, OnSelchangedGanttTree)
	ON_WM_SETFOCUS()
	ON_BN_CLICKED(IDC_PREFERENCES, OnPreferences)
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_GANTTCTRL_NOTIFY_ZOOM, OnNotifyZoomChange)
	ON_WM_ERASEBKGND()
END_MESSAGE_MAP()


/////////////////////////////////////////////////////////////////////////////
// CGanttChartWnd message handlers

BOOL CGanttChartWnd::Create(DWORD dwStyle, const RECT &rect, CWnd* pParentWnd, UINT nID)
{
	if (CDialog::Create(IDD_GANTTTREE_DIALOG, pParentWnd))
	{
		SetWindowLong(*this, GWL_STYLE, dwStyle);
		SetDlgCtrlID(nID);

		return TRUE;
	}

	return FALSE;
}

void CGanttChartWnd::SavePreferences(IPreferences* pPrefs, LPCTSTR szKey) const 
{
	CString sKey;
	sKey.Format(_T("%s\\%s"), szKey, GetTypeID());

	pPrefs->WriteProfileInt(sKey, _T("MonthDisplay"), m_nDisplay);

	m_dlgPrefs.SavePreferences(pPrefs, sKey);
}

void CGanttChartWnd::LoadPreferences(const IPreferences* pPrefs, LPCTSTR szKey, BOOL bAppOnly) 
{
	CString sKey;
	sKey.Format(_T("%s\\%s"), szKey, GetTypeID());

	// application preferences

	// use task color as background
	BOOL bColorAsBkgnd = pPrefs->GetProfileInt(_T("Preferences"), _T("ColorTaskBackground"), FALSE);
	m_ctrlGantt.EnableOption(GTLCF_USECOLORASBACKGROUND, bColorAsBkgnd);

	// Completion color and strikethru
	BOOL bStrikeThru = pPrefs->GetProfileInt(_T("Preferences"), _T("StrikethroughDone"), TRUE);

	if (pPrefs->GetProfileInt(_T("Preferences"), _T("SpecifyDoneColor"), TRUE))
	{
		COLORREF crDone = pPrefs->GetProfileInt(_T("Preferences\\Colors"), _T("TaskDone"), DEF_DONECOLOR);
		m_ctrlGantt.SetDoneTaskAttributes(crDone, bStrikeThru);
	}
	else
		m_ctrlGantt.SetDoneTaskAttributes(NO_COLOR, bStrikeThru);

	// get alternate line color from app prefs
	if (pPrefs->GetProfileInt(_T("Preferences"), _T("AlternateLineColor"), TRUE))
	{
		COLORREF crAltLine = pPrefs->GetProfileInt(_T("Preferences\\Colors"), _T("AlternateLines"), DEF_ALTLINECOLOR);
		m_ctrlGantt.SetAlternateLineColor(crAltLine);
	}
	else
		m_ctrlGantt.SetAlternateLineColor(NO_COLOR);

	// get grid line color from app prefs
	if (pPrefs->GetProfileInt(_T("Preferences"), _T("SpecifyGridColor"), TRUE))
	{
		COLORREF crGridLine = pPrefs->GetProfileInt(_T("Preferences\\Colors"), _T("GridLines"), DEF_GRIDLINECOLOR);
		m_ctrlGantt.SetGridLineColor(crGridLine);
	}
	else
		m_ctrlGantt.SetGridLineColor(NO_COLOR);

	// full row selection
	BOOL bFullRowSel = pPrefs->GetProfileInt(_T("Preferences"), _T("FullRowSelection"), FALSE);

	if (bFullRowSel)
		m_tree.ModifyStyle(TVS_HASLINES, TVS_FULLROWSELECT);
	else
		m_tree.ModifyStyle(TVS_FULLROWSELECT, TVS_HASLINES);

	DWORD dwWeekends = pPrefs->GetProfileInt(_T("Preferences"), _T("Weekends"), (DHW_SATURDAY | DHW_SUNDAY));
	CDateHelper::SetWeekendDays(dwWeekends);

	// gantt specific options
	if (!bAppOnly)
	{
		m_nDisplay = pPrefs->GetProfileInt(sKey, _T("MonthDisplay"), GTLC_DISPLAY_MONTHS_LONG);
		m_ctrlGantt.SetMonthDisplay((GTLC_MONTH_DISPLAY)m_nDisplay);

		m_dlgPrefs.LoadPreferences(pPrefs, sKey);
		UpdateGanttCtrlPreferences();

		if (GetSafeHwnd())
			UpdateData(FALSE);
	}
}

void CGanttChartWnd::SetUITheme(const UITHEME* pTheme)
{
	if (CThemed::IsThemeActive() && pTheme)
	{
		m_theme = *pTheme;
		m_brBack.CreateSolidBrush(pTheme->crAppBackLight);
	}
	else
		GraphicsMisc::VerifyDeleteObject(m_brBack);
}

bool CGanttChartWnd::ProcessMessage(MSG* pMsg) 
{
	if (!IsWindowEnabled())
		return false;

	// process editing shortcuts
	// TODO

	return false;
}

bool CGanttChartWnd::GetLabelEditRect(LPRECT pEdit) const
{
	HTREEITEM htiSel = m_tree.GetSelectedItem();

	if (m_tree.GetItemRect(htiSel, pEdit, TRUE))
	{
		// convert from tree to 'our' coords
		m_tree.ClientToScreen(pEdit);
		ScreenToClient(pEdit);
		return true;
	}

	return false;
}

void CGanttChartWnd::SelectTask(DWORD dwTaskID)
{
	m_ctrlGantt.SelectTask(dwTaskID);
}

bool CGanttChartWnd::SelectTasks(DWORD* pdwTaskIDs, int nTaskCount)
{
	return false; // only support single selection
}

bool CGanttChartWnd::WantUpdate(int nAttribute) const
{
	switch (nAttribute)
	{
	case TDCA_TASKNAME:
	case TDCA_DONEDATE:
	case TDCA_DUEDATE:
	case TDCA_STARTDATE:
	case TDCA_ALLOCTO:
	case TDCA_COLOR:
// 	case TDCA_PERCENT:
// 	case TDCA_TIMEEST:
// 	case TDCA_TIMESPENT:
		return true;
	}

	// all else 
	return false;
}

void CGanttChartWnd::UpdateTasks(const ITaskList* pTasks, IUI_UPDATETYPE nUpdate, int nEditAttribute)
{
	m_ctrlGantt.UpdateTasks(pTasks, nUpdate, nEditAttribute);
}

void CGanttChartWnd::Release()
{
	if (GetSafeHwnd())
		DestroyWindow();
	
	delete this;
}

void CGanttChartWnd::DoAppCommand(IUI_APPCOMMAND nCmd) 
{ 
	switch (nCmd)
	{
	case IUI_EXPANDALL:
		m_ctrlGantt.ExpandAll(TRUE);
		break;

	case IUI_COLLAPSEALL:
		m_ctrlGantt.ExpandAll(FALSE);
		break;

	case IUI_EXPANDSELECTED:
		{
			HTREEITEM htiSel = m_ctrlGantt.GetSelectedItem();
			m_ctrlGantt.ExpandItem(htiSel, TRUE, TRUE);
		}
		break;

	case IUI_COLLAPSESELECTED:
		{
			HTREEITEM htiSel = m_ctrlGantt.GetSelectedItem();
			m_ctrlGantt.ExpandItem(htiSel, FALSE);
		}
		break;

	case IUI_TOGGLESORT:
		m_ctrlGantt.Resort(TRUE);
		break;
				
	case IUI_RESORT:
		m_ctrlGantt.Resort(FALSE);
		break;

	case IUI_SETFOCUS:
		m_ctrlGantt.SetFocus();
		break;
	}
}

bool CGanttChartWnd::CanDoAppCommand(IUI_APPCOMMAND nCmd) const 
{ 
	switch (nCmd)
	{
	case IUI_EXPANDALL:
	case IUI_COLLAPSEALL:
		return true;

	case IUI_EXPANDSELECTED:
		{
			HTREEITEM htiSel = m_ctrlGantt.GetSelectedItem();
			return (m_ctrlGantt.CanExpandItem(htiSel, TRUE) != FALSE);
		}
		break;
	case IUI_COLLAPSESELECTED:
		{
			HTREEITEM htiSel = m_ctrlGantt.GetSelectedItem();
			return (m_ctrlGantt.CanExpandItem(htiSel, FALSE) != FALSE);
		}
		break;

	case IUI_TOGGLESORT:
	case IUI_RESORT:
		return (m_ctrlGantt.IsSorted() != FALSE);

	case IUI_SETFOCUS:
		return (m_ctrlGantt.HasFocus() != FALSE);
	}

	// all else
	return false;
}

void CGanttChartWnd::OnGototoday() 
{
	m_ctrlGantt.ScrollToToday();

	// and set focus back to it
	m_ctrlGantt.SetFocus();
}

void CGanttChartWnd::OnSize(UINT nType, int cx, int cy) 
{
	CDialog::OnSize(nType, cx, cy);
	
	Resize(cx, cy);
}

BOOL CGanttChartWnd::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// set extra styles on list
	DWORD dwExStyle = (m_list.GetExtendedStyle() | LVS_EX_DOUBLEBUFFER);
	m_list.SetExtendedStyle(dwExStyle);

	// init syncer
	m_ctrlGantt.Initialize(m_tree, m_list, IDC_TREEHEADER);
	m_ctrlGantt.ExpandAll();

	CRect rClient;
	GetClientRect(rClient);
	Resize(rClient.Width(), rClient.Height());

	m_ctrlGantt.ScrollToToday();

	m_nDisplay = m_ctrlGantt.GetMonthDisplay();
	UpdateData(FALSE);

	m_ctrlGantt.SetFocus();
	
	return FALSE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CGanttChartWnd::Resize(int cx, int cy)
{
	if (m_tree.GetSafeHwnd())
	{
		m_stDivider.MoveWindow(CRect(0, 0, cx, 1));

		CRect rDisplay;
		m_cbDisplayOptions.GetWindowRect(rDisplay);
		ScreenToClient(rDisplay);

		CRect rGantt(0, rDisplay.bottom + 10, cx, cy);
		m_ctrlGantt.Resize(rGantt);
	}
}


HBRUSH CGanttChartWnd::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor) 
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
	
	if (nCtlColor == CTLCOLOR_STATIC && m_brBack.GetSafeHandle())
	{
		pDC->SetTextColor(m_theme.crAppText);
		pDC->SetBkMode(TRANSPARENT);
		hbr = m_brBack;
	}

	return hbr;
}

BOOL CGanttChartWnd::OnEraseBkgnd(CDC* pDC) 
{
	// let the gantt do its thing
	m_ctrlGantt.HandleEraseBkgnd(pDC);

	// then our background
	if (m_brBack.GetSafeHandle())
	{
		CRect rClient;
		GetClientRect(rClient);

		pDC->FillSolidRect(rClient, m_theme.crAppBackLight);
		return TRUE;
	}
	
	// else
	return CDialog::OnEraseBkgnd(pDC);
}

void CGanttChartWnd::OnKeyUpGantt(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NMTVKEYDOWN* pTVKD = (NMTVKEYDOWN*)pNMHDR;
	
	switch (pTVKD->wVKey)
	{
	case VK_UP:
	case VK_DOWN:
	case VK_PRIOR:
	case VK_NEXT:
		SendParentSelectionUpdate();
		break;
	}
	
	*pResult = 0;
}

void CGanttChartWnd::SendParentSelectionUpdate()
{
	DWORD dwTaskID = m_ctrlGantt.GetSelectedTaskID();
	GetParent()->SendMessage(WM_IUI_SELECTTASK, 0, dwTaskID);
}

void CGanttChartWnd::OnClickGanttList(NMHDR* pNMHDR, LRESULT* pResult) 
{
	SendParentSelectionUpdate();
	*pResult = 0;
}

void CGanttChartWnd::OnSelchangeDisplay() 
{
	UpdateData();
	m_ctrlGantt.SetMonthDisplay((GTLC_MONTH_DISPLAY)m_nDisplay);
}

LRESULT CGanttChartWnd::OnNotifyZoomChange(WPARAM /*wp*/, LPARAM lp)
{
	m_nDisplay = lp;
	m_cbDisplayOptions.SetCurSel(m_nDisplay);

	return 0L;
}

void CGanttChartWnd::OnSelchangedGanttTree(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_TREEVIEW* pNMTreeView = (NM_TREEVIEW*)pNMHDR;

	// we're only interested in non-keyboard changes
	// because keyboard gets handled explicitly above
	if (pNMTreeView->action != TVC_BYKEYBOARD)
	{
		SendParentSelectionUpdate();
	}
	
	*pResult = 0;
}

void CGanttChartWnd::OnSetFocus(CWnd* pOldWnd) 
{
	CDialog::OnSetFocus(pOldWnd);
	
	m_tree.SetFocus();
}

void CGanttChartWnd::OnPreferences() 
{
	if (m_dlgPrefs.DoModal() == IDOK)
	{
		// update gantt control
		UpdateGanttCtrlPreferences();

		// and set focus back to it
		m_ctrlGantt.SetFocus();
	}
}

void CGanttChartWnd::UpdateGanttCtrlPreferences()
{
	m_ctrlGantt.EnableOption(GTLCF_DISPLAYALLOCTOAFTERITEM, m_dlgPrefs.GetDisplayAllocTo());
	m_ctrlGantt.EnableOption(GTLCF_AUTOSCROLLTOTASK, m_dlgPrefs.GetAutoScrollSelection());
	m_ctrlGantt.EnableOption(GTLCF_CALCPARENTDATES, m_dlgPrefs.GetAutoCalcParentDates());
	m_ctrlGantt.EnableOption(GTLCF_CALCMISSINGSTARTDATES, m_dlgPrefs.GetCalculateMissingStartDates());
	m_ctrlGantt.EnableOption(GTLCF_CALCMISSINGDUEDATES, m_dlgPrefs.GetCalculateMissingDueDates());
	m_ctrlGantt.SetTodayColor(m_dlgPrefs.GetTodayColor());
	m_ctrlGantt.SetWeekendColor(m_dlgPrefs.GetWeekendColor());

	COLORREF crParent;
	GTLC_PARENTCOLORING nOption = (GTLC_PARENTCOLORING)m_dlgPrefs.GetParentColoring(crParent);
	m_ctrlGantt.SetParentColoring(nOption, crParent);
}
