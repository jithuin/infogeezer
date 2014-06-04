// TDLColumnSelectionDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLColumnSelectionDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDLColumnSelectionDlg dialog


CTDLColumnSelectionDlg::CTDLColumnSelectionDlg(const CTDCColumnIDArray& aColumns, 
											   const CTDCColumnIDArray& aDefault,
											   BOOL bActiveTasklist, 
											   CWnd* pParent /*=NULL*/)
	: CDialog(CTDLColumnSelectionDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CTDLColumnSelectionDlg)
	m_bActiveTasklist = bActiveTasklist;
	//}}AFX_DATA_INIT

	m_aVisibleColumns.Copy(aColumns);
	m_aDefaultColumns.Copy(aDefault);
}


void CTDLColumnSelectionDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLColumnSelectionDlg)
	DDX_Control(pDX, IDC_COLUMNS, m_lbColumns);
	DDX_Radio(pDX, IDC_ALLTASKLISTS, m_bActiveTasklist);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLColumnSelectionDlg, CDialog)
	//{{AFX_MSG_MAP(CTDLColumnSelectionDlg)
	ON_BN_CLICKED(IDC_SELECTALLCOLS, OnSelectallcols)
	ON_BN_CLICKED(IDC_CLEARALLCOLS, OnClearallcols)
	ON_BN_CLICKED(IDC_DEFAULTCOLS, OnDefaultcols)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLColumnSelectionDlg message handlers

int CTDLColumnSelectionDlg::GetVisibleColumns(CTDCColumnIDArray& aColumns) const
{
	aColumns.Copy(m_aVisibleColumns);

	return aColumns.GetSize();
}

void CTDLColumnSelectionDlg::OnOK()
{
	CDialog::OnOK();

	m_lbColumns.GetVisibleColumns(m_aVisibleColumns);
}

BOOL CTDLColumnSelectionDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	m_lbColumns.SetVisibleColumns(m_aVisibleColumns);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLColumnSelectionDlg::OnSelectallcols() 
{
	m_lbColumns.SetAllColumnsVisible(TRUE);
}

void CTDLColumnSelectionDlg::OnClearallcols() 
{
	m_lbColumns.SetAllColumnsVisible(FALSE);
}

void CTDLColumnSelectionDlg::OnDefaultcols() 
{
	m_lbColumns.SetVisibleColumns(m_aDefaultColumns);
}
