// TDLShowReminderDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLShowReminderDlg.h"
#include "filteredtodoctrl.h"
#include "todoctrlreminders.h"

#include "..\Shared\enstring.h"
#include "..\Shared\graphicsmisc.h"

#pragma warning(disable: 4201)
#include <Mmsystem.h>
#pragma warning(default: 4201)

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDLShowReminderDlg dialog


CTDLShowReminderDlg::CTDLShowReminderDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CTDLShowReminderDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CTDLShowReminderDlg)
	m_sWhen = _T("");
	m_sTaskTitle = _T("");
	m_nSnoozeIndex = 0;
	//}}AFX_DATA_INIT
}


void CTDLShowReminderDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLShowReminderDlg)
	DDX_Text(pDX, IDC_WHENTEXT, m_sWhen);
	DDX_Text(pDX, IDC_TASKTITLE, m_sTaskTitle);
	DDX_CBIndex(pDX, IDC_SNOOZE, m_nSnoozeIndex);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLShowReminderDlg, CDialog)
	//{{AFX_MSG_MAP(CTDLShowReminderDlg)
	ON_BN_CLICKED(IDSNOOZE, OnSnooze)
	ON_BN_CLICKED(IDGOTOTASK, OnGototask)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLShowReminderDlg message handlers

int CTDLShowReminderDlg::DoModal(const TDCREMINDER& rem)
{
	m_sWhen = rem.FormatWhenString();
	m_sTaskTitle = rem.GetTaskTitle();
	m_sSoundFile = rem.sSoundFile;

	return CDialog::DoModal();
}

BOOL CTDLShowReminderDlg::OnInitDialog()
{
	BOOL bRes = CDialog::OnInitDialog();
	
	// do we need to play a sound?
	if (!m_sSoundFile.IsEmpty())
		PlaySound(m_sSoundFile, NULL, SND_FILENAME);
	
	CWnd* pTitle = GetDlgItem(IDC_TASKTITLE);
	ASSERT(pTitle);
	
	CFont* pFont = pTitle->GetFont();
	ASSERT(pFont);

	if (GraphicsMisc::CreateFont(m_fontBold, *pFont, GMFS_BOLD, GMFS_BOLD))
		pTitle->SetFont(&m_fontBold);
	
	return bRes;
}

void CTDLShowReminderDlg::OnSnooze() 
{
	UpdateData();
	EndDialog(IDSNOOZE);	
}

int CTDLShowReminderDlg::GetSnoozeMinutes() const
{
	const UINT SNOOZE[] = { 5, 10, 15, 20, 30, 45, 60, 120, 180, 240, 300, 360 };
	const UINT SIZESNOOZE = sizeof (SNOOZE) / sizeof (UINT);

	ASSERT (m_nSnoozeIndex < SIZESNOOZE);

	if (m_nSnoozeIndex >= SIZESNOOZE)
		return 60;
	else
		return SNOOZE[m_nSnoozeIndex];
}

void CTDLShowReminderDlg::OnGototask() 
{
	EndDialog(IDGOTOTASK);	
}
