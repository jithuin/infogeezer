// KeyboardShortcutDisplayDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "KeyboardShortcutDisplayDlg.h"

#include "..\shared\enstring.h"
#include "..\shared\misc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#ifndef LVS_EX_LABELTIP
#define LVS_EX_LABELTIP     0x00004000
#endif

/////////////////////////////////////////////////////////////////////////////
// CKeyboardShortcutDisplayDlg dialog

CKeyboardShortcutDisplayDlg::CKeyboardShortcutDisplayDlg(const CStringArray& aMapping, char cDelim, CWnd* pParent /*=NULL*/)
	: CDialog(CKeyboardShortcutDisplayDlg::IDD, pParent), m_aMapping(aMapping), m_cDelim(cDelim)

{
	//{{AFX_DATA_INIT(CKeyboardShortcutDisplayDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CKeyboardShortcutDisplayDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CKeyboardShortcutDisplayDlg)
	DDX_Control(pDX, IDC_SHORTCUTS, m_lcShortcuts);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CKeyboardShortcutDisplayDlg, CDialog)
	//{{AFX_MSG_MAP(CKeyboardShortcutDisplayDlg)
	ON_BN_CLICKED(IDC_COPYSHORTCUTS, OnCopyshortcuts)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CKeyboardShortcutDisplayDlg message handlers

BOOL CKeyboardShortcutDisplayDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

	if (m_aMapping.GetSize() == 0)
	{
		EndDialog(IDCANCEL);
		return FALSE;
	}

	m_lcShortcuts.InsertColumn(0, _T("Shortcut"));
	m_lcShortcuts.InsertColumn(1, _T("Menu Option"));

	for (int nItem = 0; nItem < m_aMapping.GetSize(); nItem++)
	{
		const CString& sItem = m_aMapping[nItem];
		int nDelim = sItem.Find(m_cDelim);

		if (nDelim != -1)
		{
			int nIndex = m_lcShortcuts.InsertItem(nItem, sItem.Left(nDelim));
			m_lcShortcuts.SetItemText(nIndex, 1, sItem.Mid(nDelim + 1));
		}
		else
			m_lcShortcuts.InsertItem(nItem, _T(""));
	}

	m_lcShortcuts.SetColumnWidth(0, LVSCW_AUTOSIZE);
	m_lcShortcuts.SetColumnWidth(1, LVSCW_AUTOSIZE);

	if (m_il.Create(1, 16, ILC_COLOR, 1, 1))
		m_lcShortcuts.SetImageList(&m_il, LVSIL_SMALL);

//	ListView_SetExtendedListViewStyleEx(m_lcShortcuts, LVS_EX_GRIDLINES, LVS_EX_GRIDLINES);
	ListView_SetExtendedListViewStyleEx(m_lcShortcuts, LVS_EX_FULLROWSELECT, LVS_EX_FULLROWSELECT);
	ListView_SetExtendedListViewStyleEx(m_lcShortcuts, LVS_EX_LABELTIP, LVS_EX_LABELTIP);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CKeyboardShortcutDisplayDlg::OnCopyshortcuts() 
{
	CString sText, sLine;

	for (int nItem = 0; nItem < m_aMapping.GetSize(); nItem++)
	{
		const CString& sItem = m_aMapping[nItem];
		int nDelim = sItem.Find(m_cDelim);

		if (nDelim != -1)
		{
			sLine.Format(_T("%s\t\t%s\n"), sItem.Left(nDelim), sItem.Mid(nDelim + 1));
			sText += sLine;
		}
		else
			sText += _T("\n");
	}

	Misc::CopyTexttoClipboard(sText, *this);
}
