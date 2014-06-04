// TDLCmdlineOptionsDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLCmdlineOptionsDlg.h"

#include "..\shared\Misc.h"
#include "..\shared\enstring.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#ifndef LVS_EX_LABELTIP
#define LVS_EX_LABELTIP     0x00004000
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDLCmdlineOptionsDlg dialog

CTDLCmdlineOptionsDlg::CTDLCmdlineOptionsDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CTDLCmdlineOptionsDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CTDLCmdlineOptionsDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CTDLCmdlineOptionsDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLCmdlineOptionsDlg)
	DDX_Control(pDX, IDC_CMDLINE_OPTIONS, m_lcOptions);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLCmdlineOptionsDlg, CDialog)
	//{{AFX_MSG_MAP(CTDLCmdlineOptionsDlg)
	ON_BN_CLICKED(IDC_COPYSHORTCUTS, OnCopyshortcuts)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLCmdlineOptionsDlg message handlers

BOOL CTDLCmdlineOptionsDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// columns
	m_lcOptions.InsertColumn(0, _T("Option"));
	m_lcOptions.InsertColumn(1, _T("Description"));

	// options
	CStringArray aOptions;
	int nNumOpt = Misc::Split(GetOptions(), '\n', aOptions);

	for (int nOpt = 0; nOpt < nNumOpt; nOpt++)
	{
		CStringArray aText;

		if (Misc::Split(aOptions[nOpt], '\t', aText) == 2)
		{
			int nIndex = m_lcOptions.InsertItem(nOpt, aText[0]);
			m_lcOptions.SetItemText(nIndex, 1, aText[1]);
		}
	}
	m_lcOptions.SetColumnWidth(0, LVSCW_AUTOSIZE);
	m_lcOptions.SetColumnWidth(1, LVSCW_AUTOSIZE);
	
	// dummy imagelist to increase row height
	if (m_il.Create(1, 16, ILC_COLOR, 1, 1))
		m_lcOptions.SetImageList(&m_il, LVSIL_SMALL);

//	ListView_SetExtendedListViewStyleEx(m_lcOptions, LVS_EX_GRIDLINES, LVS_EX_GRIDLINES);
	ListView_SetExtendedListViewStyleEx(m_lcOptions, LVS_EX_FULLROWSELECT, LVS_EX_FULLROWSELECT);
	ListView_SetExtendedListViewStyleEx(m_lcOptions, LVS_EX_LABELTIP, LVS_EX_LABELTIP);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLCmdlineOptionsDlg::OnCopyshortcuts() 
{

	Misc::CopyTexttoClipboard(GetOptions(), *this);
}

CString CTDLCmdlineOptionsDlg::GetOptions()
{
	CEnString sOptions(IDS_COMMANDLINEOPTIONS);
	sOptions += CEnString(IDS_COMMANDLINETASKOPTIONS);
	sOptions += CEnString(IDS_COMMANDLINETASKOPTIONS2);
	sOptions += CEnString(IDS_COMMANDLINETASKOPTIONS3);

	return sOptions;
}
