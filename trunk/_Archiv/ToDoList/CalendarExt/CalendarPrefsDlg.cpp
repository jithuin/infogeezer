// CalendarPrefsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "CalendarExtResource.h"
#include "calendarext.h"
#include "CalendarPrefsDlg.h"
#include "calendardefines.h"

#include "..\Shared\Misc.h"
#include "..\Shared\dialoghelper.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCalendarPrefsDlg dialog


CCalendarPrefsDlg::CCalendarPrefsDlg(DWORD dwFlags, int nCellFontSize, CWnd* pParent /*=NULL*/)
	: CDialog(IDD_TASKPREFS_DIALOG, pParent), m_dwFlags(dwFlags), m_nCellFontSize(nCellFontSize)
{
	//{{AFX_DATA_INIT(CCalendarPrefsDlg)
	//}}AFX_DATA_INIT
	m_bShowIncompleteStartDate = Misc::HasFlag(m_dwFlags, SHOW_INCOMPLETE_STARTDATES);
	m_bShowIncompleteDueDate = Misc::HasFlag(m_dwFlags, SHOW_INCOMPLETE_DUEDATES);
	m_bShowCompleteStartDate = Misc::HasFlag(m_dwFlags, SHOW_COMPLETE_STARTDATES);
	m_bShowCompleteDueDate = Misc::HasFlag(m_dwFlags, SHOW_COMPLETE_DUEDATES);
	m_bShowCompleteDoneDate = Misc::HasFlag(m_dwFlags, SHOW_COMPLETE_DONEDATES);
	m_bGreyCompletedTasks = Misc::HasFlag(m_dwFlags, COMPLETEDTASKS_GREY);
	m_bStrikeThruCompletedTasks = Misc::HasFlag(m_dwFlags, COMPLETEDTASKS_STRIKETHRU);
}

void CCalendarPrefsDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCalendarPrefsDlg)
	DDX_Control(pDX, IDC_FONTSIZE, m_cbCellFontSize);
	DDX_Check(pDX, IDC_SHOWINCOMPLETESTART, m_bShowIncompleteStartDate);
	DDX_Check(pDX, IDC_SHOWINCOMPLETEDUE, m_bShowIncompleteDueDate);
	DDX_Check(pDX, IDC_SHOWCOMPLETESTART, m_bShowCompleteStartDate);
	DDX_Check(pDX, IDC_SHOWCOMPLETEDUE, m_bShowCompleteDueDate);
	DDX_Check(pDX, IDC_SHOWCOMPLETEDONE, m_bShowCompleteDoneDate);
	DDX_Check(pDX, IDC_GREYCOMPLETEDTASKS, m_bGreyCompletedTasks);
	DDX_Check(pDX, IDC_STRIKETHRUCOMPLETEDTASKS, m_bStrikeThruCompletedTasks);
	//}}AFX_DATA_MAP

	if (pDX->m_bSaveAndValidate)
		m_nCellFontSize = CDialogHelper::GetSelectedItemAsValue(m_cbCellFontSize);
	else
		CDialogHelper::SelectItemByValue(m_cbCellFontSize, m_nCellFontSize);
}


BEGIN_MESSAGE_MAP(CCalendarPrefsDlg, CDialog)
	//{{AFX_MSG_MAP(CCalendarPrefsDlg)
	ON_BN_CLICKED(IDC_GREYCOMPLETEDTASKS, OnGreycompletedtasks)
	ON_BN_CLICKED(IDC_STRIKETHRUCOMPLETEDTASKS, OnStrikethrucompletedtasks)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCalendarPrefsDlg message handlers

void CCalendarPrefsDlg::OnGreycompletedtasks() 
{
	UpdateFlags();
}

void CCalendarPrefsDlg::OnStrikethrucompletedtasks() 
{
	UpdateFlags();
}

#define ADDFLAG(want, flag) if (want) m_dwFlags |= flag;

void CCalendarPrefsDlg::UpdateFlags()
{
	UpdateData();

	m_dwFlags = 0;

	ADDFLAG(m_bGreyCompletedTasks, COMPLETEDTASKS_GREY);
	ADDFLAG(m_bStrikeThruCompletedTasks, COMPLETEDTASKS_STRIKETHRU);
	ADDFLAG(m_bShowIncompleteStartDate, SHOW_INCOMPLETE_STARTDATES);
	ADDFLAG(m_bShowIncompleteDueDate, SHOW_INCOMPLETE_DUEDATES);
	ADDFLAG(m_bShowCompleteStartDate, SHOW_COMPLETE_STARTDATES);
	ADDFLAG(m_bShowCompleteDueDate, SHOW_COMPLETE_DUEDATES);
	ADDFLAG(m_bShowCompleteDoneDate, SHOW_COMPLETE_DONEDATES);
}


BOOL CCalendarPrefsDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	m_mgrGroupLines.AddGroupLine(IDC_DISPLAYDONETASKS_GROUP, *this);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CCalendarPrefsDlg::OnOK()
{
	CDialog::OnOK();

	UpdateFlags();
}
