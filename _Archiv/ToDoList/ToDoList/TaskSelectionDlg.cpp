// TaskSelectionDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TaskSelectionDlg.h"

#include "..\shared\Preferences.h"
#include "..\shared\misc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTaskSelectionDlg dialog


CTaskSelectionDlg::CTaskSelectionDlg(LPCTSTR szRegKey, FTC_VIEW nView, BOOL bVisibleColumnsOnly) : 
	CDialog(), m_sRegKey(szRegKey), m_nView(nView)
{
	//{{AFX_DATA_INIT(CTaskSelectionDlg)
	//}}AFX_DATA_INIT
	CPreferences prefs;

	m_bCompletedTasks = prefs.GetProfileInt(m_sRegKey, _T("CompletedTasks"), TRUE);
	m_bIncompleteTasks = prefs.GetProfileInt(m_sRegKey, _T("IncompleteTasks"), TRUE);
	m_nWhatTasks = prefs.GetProfileInt(m_sRegKey, _T("WhatTasks"), TSDT_FILTERED);
	m_bSelectedSubtasks = prefs.GetProfileInt(m_sRegKey, _T("SelectedSubtasks"), TRUE);
	m_bSelectedParentTask = prefs.GetProfileInt(m_sRegKey, _T("SelectedParentTask"), FALSE);
	m_bIncludeComments = prefs.GetProfileInt(m_sRegKey, _T("IncludeCommentsWithVisible"), FALSE);

	if (bVisibleColumnsOnly)
		m_nAttribOption = TSDA_VISIBLE;
	else
		m_nAttribOption = prefs.GetProfileInt(m_sRegKey, _T("AttributeOption"), TSDA_ALL);
	
	CTDCAttributeArray aAttrib;
	CString sGroup = m_sRegKey + _T("\\AttribVisibility");
	int nAttrib = prefs.GetProfileInt(sGroup, _T("Count"), 0);

	while (nAttrib--)
	{
		CString sKey = Misc::MakeKey(_T("att%d"), nAttrib);
		TDC_ATTRIBUTE att = (TDC_ATTRIBUTE)prefs.GetProfileInt(sGroup, sKey, 0);

		aAttrib.Add(att);
	}

	m_lbAttribList.SetVisibleAttributes(aAttrib);
}


void CTaskSelectionDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTaskSelectionDlg)
	DDX_Radio(pDX, IDC_ALLATTRIB, m_nAttribOption);
	DDX_Check(pDX, IDC_INCLUDEPARENTTASK, m_bSelectedParentTask);
	DDX_Control(pDX, IDC_CUSTOMATTRIBLIST, m_lbAttribList);
	DDX_Check(pDX, IDC_INCLUDESUBTASKS, m_bSelectedSubtasks);
	DDX_Check(pDX, IDC_INCLUDECOMMENTS, m_bIncludeComments);
	//}}AFX_DATA_MAP
	DDX_Check(pDX, IDC_INCLUDEDONE, m_bCompletedTasks);
	DDX_Check(pDX, IDC_INCLUDENOTDONE, m_bIncompleteTasks);
	DDX_Radio(pDX, IDC_ALLTASKS, m_nWhatTasks);
}


BEGIN_MESSAGE_MAP(CTaskSelectionDlg, CDialog)
//{{AFX_MSG_MAP(CTaskSelectionDlg)
	ON_BN_CLICKED(IDC_ALLATTRIB, OnChangeAttribOption)
	ON_BN_CLICKED(IDC_CUSTOMATTRIB, OnChangeAttribOption)
	ON_BN_CLICKED(IDC_VISIBLEATTRIB, OnChangeAttribOption)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_ALLTASKS, OnChangetasksOption)
	ON_BN_CLICKED(IDC_FILTERTASKS, OnChangetasksOption)
	ON_BN_CLICKED(IDC_INCLUDEDONE, OnIncludeDone)
	ON_BN_CLICKED(IDC_SELTASK, OnChangetasksOption)
	ON_BN_CLICKED(IDC_INCLUDENOTDONE, OnIncludeNotDone)
	ON_WM_DESTROY()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTaskSelectionDlg message handlers

BOOL CTaskSelectionDlg::Create(UINT nIDRefFrame, CWnd* pParent, UINT nID)
{
	ASSERT (nIDRefFrame && pParent);
	
	if (CDialog::Create(IDD_TASKSELECTION_DIALOG, pParent))
	{
		if (nID != IDC_STATIC)
			SetDlgCtrlID(nID);
		
		CWnd* pFrame = pParent->GetDlgItem(nIDRefFrame);
		
		if (pFrame)
		{
			CRect rFrame;
			pFrame->GetWindowRect(rFrame);
			pParent->ScreenToClient(rFrame);
			
			MoveWindow(rFrame);

			// insert ourselves after this item in the Z-order
			SetWindowPos(pFrame, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);

			// and then hide it
			pFrame->ShowWindow(SW_HIDE);
		}

		return TRUE;
	}
	
	return FALSE;
}

void CTaskSelectionDlg::OnDestroy() 
{
	UpdateData();
	
	CDialog::OnDestroy();
	
	// save settings
	CPreferences prefs;
	
	prefs.WriteProfileInt(m_sRegKey, _T("CompletedTasks"), m_bCompletedTasks);
	prefs.WriteProfileInt(m_sRegKey, _T("IncompleteTasks"), m_bIncompleteTasks);
	prefs.WriteProfileInt(m_sRegKey, _T("WhatTasks"), m_nWhatTasks);
	prefs.WriteProfileInt(m_sRegKey, _T("AttributeOption"), m_nAttribOption);
	prefs.WriteProfileInt(m_sRegKey, _T("SelectedSubtasks"), m_bSelectedSubtasks);
	prefs.WriteProfileInt(m_sRegKey, _T("SelectedParentTask"), m_bSelectedParentTask);
	prefs.WriteProfileInt(m_sRegKey, _T("IncludeCommentsWithVisible"), m_bIncludeComments);

	CTDCAttributeArray aAttrib;
	int nAttrib = m_lbAttribList.GetVisibleAttributes(aAttrib);
	CString sGroup = m_sRegKey + _T("\\AttribVisibility");

	prefs.WriteProfileInt(sGroup, _T("Count"), nAttrib);

	while (nAttrib--)
	{
		CString sKey = Misc::MakeKey(_T("att%d"), nAttrib);
		prefs.WriteProfileInt(sGroup, sKey, aAttrib[nAttrib]);
	}
}

void CTaskSelectionDlg::OnChangetasksOption() 
{
	UpdateData();
	
	BOOL bWantSelTasks = GetWantSelectedTasks();
	
	GetDlgItem(IDC_INCLUDEDONE)->EnableWindow(!bWantSelTasks);
	GetDlgItem(IDC_INCLUDENOTDONE)->EnableWindow(!bWantSelTasks);
	
	GetDlgItem(IDC_INCLUDESUBTASKS)->EnableWindow(bWantSelTasks);
	GetDlgItem(IDC_INCLUDEPARENTTASK)->EnableWindow(bWantSelTasks);
	
	GetParent()->SendMessage(WM_TASKSELDLG_CHANGE);
}

void CTaskSelectionDlg::OnIncludeDone() 
{
	UpdateData();
	
	// prevent the user unchecking both tick boxes
	if (!m_bCompletedTasks && !m_bIncompleteTasks)
	{
		m_bIncompleteTasks = TRUE;
		UpdateData(FALSE);
	}
	
	GetParent()->SendMessage(WM_TASKSELDLG_CHANGE);
}
void CTaskSelectionDlg::OnIncludeNotDone() 
{
	UpdateData();
	
	// prevent the user unchecking both tick boxes
	if (!m_bCompletedTasks && !m_bIncompleteTasks)
	{
		m_bCompletedTasks = TRUE;
		UpdateData(FALSE);
	}
	
	GetParent()->SendMessage(WM_TASKSELDLG_CHANGE);
}

BOOL CTaskSelectionDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	BOOL bWantSelTasks = GetWantSelectedTasks();
		
	GetDlgItem(IDC_CUSTOMATTRIBLIST)->EnableWindow(m_nAttribOption == TSDA_USER);
	GetDlgItem(IDC_INCLUDECOMMENTS)->EnableWindow(m_nAttribOption == TSDA_VISIBLE);

	GetDlgItem(IDC_INCLUDEDONE)->EnableWindow(!bWantSelTasks);
	GetDlgItem(IDC_INCLUDENOTDONE)->EnableWindow(!bWantSelTasks);

	GetDlgItem(IDC_INCLUDESUBTASKS)->EnableWindow(bWantSelTasks);
	GetDlgItem(IDC_INCLUDEPARENTTASK)->EnableWindow(bWantSelTasks);

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CTaskSelectionDlg::SetWantWhatTasks(TSD_TASKS nWhat)
{
	m_nWhatTasks = nWhat;

	if (GetSafeHwnd())
	{
		UpdateData(FALSE);
		
		BOOL bWantSelTasks = GetWantSelectedTasks();
		
		GetDlgItem(IDC_INCLUDEDONE)->EnableWindow(!bWantSelTasks);
		GetDlgItem(IDC_INCLUDENOTDONE)->EnableWindow(!bWantSelTasks);

		GetDlgItem(IDC_INCLUDESUBTASKS)->EnableWindow(bWantSelTasks);
		GetDlgItem(IDC_INCLUDEPARENTTASK)->EnableWindow(bWantSelTasks);
	}
}

void CTaskSelectionDlg::SetWantCompletedTasks(BOOL bWant)
{
	// prevent the user unchecking both tick boxes
	if (!bWant && !m_bIncompleteTasks)
		m_bIncompleteTasks = TRUE;
	
	m_bCompletedTasks = bWant;

	if (GetSafeHwnd())
		UpdateData(FALSE);
}

void CTaskSelectionDlg::SetWantInCompleteTasks(BOOL bWant)
{
	// prevent the user unchecking both tick boxes
	if (!bWant && !m_bCompletedTasks)
		m_bCompletedTasks = TRUE;
	
	m_bIncompleteTasks = bWant;

	if (GetSafeHwnd())
		UpdateData(FALSE);
}

void CTaskSelectionDlg::OnChangeAttribOption() 
{
	UpdateData();

	GetDlgItem(IDC_CUSTOMATTRIBLIST)->EnableWindow(m_nAttribOption == TSDA_USER);
	GetDlgItem(IDC_INCLUDECOMMENTS)->EnableWindow(m_nAttribOption == TSDA_VISIBLE);
}

int CTaskSelectionDlg::GetUserAttributes(CTDCAttributeArray& aAttrib) const
{
	aAttrib.RemoveAll();

	if (m_nAttribOption == TSDA_USER)
	{
		m_lbAttribList.GetVisibleAttributes(aAttrib);
		aAttrib.Add(TDCA_CUSTOMATTRIB); // always
	}
	else
		ASSERT(0);

	return aAttrib.GetSize();
}
