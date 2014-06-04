// CalendarButtonsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "calendarext.h"
#include "calendarextresource.h"
#include "CalendarButtonsDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCalendarButtonsDlg dialog


CCalendarButtonsDlg::CCalendarButtonsDlg(CWnd* pParent /*=NULL*/)
	: CDialog(IDD_BUTTONS_DIALOG, pParent), m_btnView(IDR_CALENDAR, 0, MBS_DOWN)
{
	//{{AFX_DATA_INIT(CCalendarButtonsDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CCalendarButtonsDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCalendarButtonsDlg)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CCalendarButtonsDlg, CDialog)
	//{{AFX_MSG_MAP(CCalendarButtonsDlg)
	ON_BN_CLICKED(IDC_GOTOTODAY, OnGotoToday)
	ON_WM_ERASEBKGND()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCalendarButtonsDlg message handlers

BOOL CCalendarButtonsDlg::Create(CWnd* pParent)
{
	return CDialog::Create(IDD_BUTTONS_DIALOG, pParent);
}

void CCalendarButtonsDlg::OnGotoToday() 
{
	GetParent()->SendMessage(WM_COMMAND, ID_GOTOTODAY, 0);
}

BOOL CCalendarButtonsDlg::OnEraseBkgnd(CDC* pDC) 
{
	CRect rClient;
	GetClientRect(rClient);

	pDC->FillSolidRect(rClient, GetSysColor(COLOR_WINDOW));
	return TRUE;
}

BOOL CCalendarButtonsDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	m_btnView.SubclassDlgItem(IDC_PREFERENCES, this);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}
