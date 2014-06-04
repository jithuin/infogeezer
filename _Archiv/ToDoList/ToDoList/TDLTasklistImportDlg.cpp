// TDLTasklistImportDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLTasklistImportDlg.h"
#include "ToDoCtrl.h"
#include "TDLContentMgr.h"
#include "tdcmsg.h"

#include "..\shared\DialogHelper.h"
#include "..\shared\ContentMgr.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDLTasklistImportDlg dialog

static CTDLContentMgr cm;
static CONTENTFORMAT cf = _T("PLAIN_TEXT");

CTDLTasklistImportDlg::CTDLTasklistImportDlg(const CString& sFilePath, CWnd* pParent /*=NULL*/)
	: CDialog(IDD_TDLIMPORTEXPORT_DIALOG, pParent), m_tdc(cm, cf), m_eFilePath(FES_NOBROWSE)
{
	//{{AFX_DATA_INIT(CTDLTasklistImportDlg)
	m_bImportSubtasks = TRUE;
	//}}AFX_DATA_INIT
	m_bResetCreationDate = TRUE;
	m_sFilePath = sFilePath;
}


void CTDLTasklistImportDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLTasklistImportDlg)
	DDX_Check(pDX, IDC_RESETCREATIONDATE, m_bResetCreationDate);
	DDX_Text(pDX, IDC_TDLFILEPATH, m_sFilePath);
	DDX_Control(pDX, IDC_TDLFILEPATH, m_eFilePath);
	DDX_Check(pDX, IDC_IMPORTSUBTASKS, m_bImportSubtasks);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLTasklistImportDlg, CDialog)
	//{{AFX_MSG_MAP(CTDLTasklistImportDlg)
	ON_BN_CLICKED(IDC_SELECTALL, OnSelectall)
	ON_BN_CLICKED(IDC_SELECTNONE, OnSelectnone)
	//}}AFX_MSG_MAP
	ON_REGISTERED_MESSAGE(WM_TDCN_SELECTIONCHANGE, OnTDCNotifySelectionChange)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLTasklistImportDlg message handlers

BOOL CTDLTasklistImportDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// create todoctrl in the space of IDC_TODOCTRL
	CRect rToDoCtrl = CDialogHelper::GetCtrlRect(this, IDC_TODOCTRL);

	if (m_tdc.Create(rToDoCtrl, this, IDC_TODOCTRL+1))
	{
		m_tdc.SetMaximizeState(TDCMS_MAXTASKLIST);
		m_tdc.SetVisibleColumns(CTDCColumnIDArray()); // no columns
		m_tdc.Load(m_sFilePath);
		m_tdc.SetReadonly(TRUE);
		m_tdc.ExpandTasks(TDCEC_ALL);
		m_tdc.SelectAll();
		m_tdc.SetFocusToTasks();
	}
	
	return FALSE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLTasklistImportDlg::OnSelectall() 
{
	m_tdc.SelectAll();
}

void CTDLTasklistImportDlg::OnSelectnone() 
{
	m_tdc.DeselectAll();
}

LRESULT CTDLTasklistImportDlg::OnTDCNotifySelectionChange(WPARAM, LPARAM)
{
	GetDlgItem(IDOK)->EnableWindow(m_tdc.GetSelectedCount());
	GetDlgItem(IDC_IMPORTSUBTASKS)->EnableWindow(m_tdc.SelectedTasksHaveChildren());

	return 0L;
}

void CTDLTasklistImportDlg::OnOK()
{
	CDialog::OnOK();

	m_tdc.GetSelectedTasks(m_tasksSelected, TDCGETTASKS(), (m_bImportSubtasks ? 0 : TDCGSTF_NOTSUBTASKS));
}

int CTDLTasklistImportDlg::GetSelectedTasks(ITaskList8* pTasks)
{
	ASSERT(pTasks);

	if (pTasks)
	{
		ResetSelectedTaskCreationDate(m_tasksSelected.GetFirstTask());

		m_tasksSelected.CopyTo(pTasks);
		return m_tasksSelected.GetTaskCount();
	}

	// else
	return 0;
}

void CTDLTasklistImportDlg::ResetSelectedTaskCreationDate(HTASKITEM hTask)
{
	if (m_bResetCreationDate && hTask)
	{
		m_tasksSelected.SetTaskCreationDate(hTask, time(NULL));

		// first child
		ResetSelectedTaskCreationDate(m_tasksSelected.GetFirstTask(hTask));

		// next task
		ResetSelectedTaskCreationDate(m_tasksSelected.GetNextTask(hTask));
	}
}
