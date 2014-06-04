// TDLAddLoggedTimeDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLAddLoggedTimeDlg.h"

#include "..\shared\DateHelper.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDLAddLoggedTimeDlg dialog


CTDLAddLoggedTimeDlg::CTDLAddLoggedTimeDlg(DWORD dwTaskID, LPCTSTR szTaskTitle, CWnd* pParent /*=NULL*/)
	: CDialog(CTDLAddLoggedTimeDlg::IDD, pParent), m_cbTimeWhen(TCB_HALFHOURS)
{
	//{{AFX_DATA_INIT(CTDLAddLoggedTimeDlg)
	m_dLoggedTime = 0.0;
	m_dwTaskID = dwTaskID;
	m_sTaskTitle = szTaskTitle;
	//}}AFX_DATA_INIT
	m_dtWhen = COleDateTime::GetCurrentTime();
	m_bAddTimeToTimeSpent = AfxGetApp()->GetProfileInt(_T("Preferences"), _T("AddLoggedTimeToTimeSpent"), TRUE);
	m_nUnits = (TCHAR)AfxGetApp()->GetProfileInt(_T("Preferences"), _T("AddLoggedTimeUnits"), THU_MINS);

	m_eLoggedTime.EnableNegativeTimes(TRUE);
}


void CTDLAddLoggedTimeDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLAddLoggedTimeDlg)
	DDX_Control(pDX, IDC_DATETIMEPICKER1, m_dateWhen);
	DDX_Control(pDX, IDC_COMBO1, m_cbTimeWhen);
	DDX_Text(pDX, IDC_LOGGEDTIME, m_dLoggedTime);
	DDX_Text(pDX, IDC_TASKID, m_dwTaskID);
	DDX_Text(pDX, IDC_TASKTITLE, m_sTaskTitle);
	DDX_Control(pDX, IDC_LOGGEDTIME, m_eLoggedTime);
	DDX_Check(pDX, IDC_ADDTIMETOTIMESPENT, m_bAddTimeToTimeSpent);
	//}}AFX_DATA_MAP

	if (pDX->m_bSaveAndValidate)
	{
		m_nUnits = m_eLoggedTime.GetUnits();

		m_dateWhen.GetTime(m_dtWhen);
		COleDateTime time = m_cbTimeWhen.GetOleTime();

		m_dtWhen = CDateHelper::GetDateOnly(m_dtWhen) + time;
	}
	else
	{
		m_eLoggedTime.SetUnits(m_nUnits);

		m_dateWhen.SetTime(m_dtWhen);
		m_cbTimeWhen.SetOleTime(CDateHelper::GetTimeOnly(m_dtWhen));
	}
}


BEGIN_MESSAGE_MAP(CTDLAddLoggedTimeDlg, CDialog)
	//{{AFX_MSG_MAP(CTDLAddLoggedTimeDlg)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLAddLoggedTimeDlg message handlers

double CTDLAddLoggedTimeDlg::GetLoggedTime() const // in hours
{ 
	return CTimeHelper().GetTime(m_dLoggedTime, m_nUnits, THU_HOURS);
}

COleDateTime CTDLAddLoggedTimeDlg::GetWhen() const
{
	return m_dtWhen;
}

void CTDLAddLoggedTimeDlg::OnOK()
{
	CDialog::OnOK();

	AfxGetApp()->WriteProfileInt(_T("Preferences"), _T("AddLoggedTimeToTimeSpent"), m_bAddTimeToTimeSpent);
	AfxGetApp()->WriteProfileInt(_T("Preferences"), _T("AddLoggedTimeUnits"), m_nUnits);
}
