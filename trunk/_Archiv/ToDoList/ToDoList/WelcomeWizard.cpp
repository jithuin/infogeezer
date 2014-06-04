// WelcomeWizard.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "WelcomeWizard.h"

#include "..\shared\dialoghelper.h"
#include "..\shared\misc.h"
#include "..\shared\graphicsmisc.h"
#include "..\shared\filemisc.h"
#include "..\Shared\enstring.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////

#ifndef CSIDL_PROGRAM_FILES
#	define CSIDL_PROGRAM_FILES 0x0026
#endif 

/////////////////////////////////////////////////////////////////////////////
// CWelcomeWizard

IMPLEMENT_DYNAMIC(CTDLWelcomeWizard, CPropertySheetEx)

CTDLWelcomeWizard::CTDLWelcomeWizard() : CPropertySheetEx(_T(""), NULL, 0)
{
	InitSheet();
}

void CTDLWelcomeWizard::InitSheet()
{
	m_hFont = GraphicsMisc::CreateFont(_T("Tahoma"));

	m_page1.AttachFont(m_hFont);
	m_page2.AttachFont(m_hFont);
	m_page3.AttachFont(m_hFont);

	AddPage(&m_page1);
	AddPage(&m_page2);
	AddPage(&m_page3);
	SetWizardMode();

//	m_psh.dwFlags |= PSH_WIZARD97 | PSH_HEADER | PSH_USEICONID | PSH_WATERMARK/* | PSH_USEHBMHEADER*/;		
	m_psh.dwFlags &= ~(PSH_HASHELP);		
//	m_psh.hInstance = AfxGetInstanceHandle(); 
//	m_psh.pszIcon = MAKEINTRESOURCE(IDR_MAINFRAME_STD);
//	m_psh.pszbmHeader = MAKEINTRESOURCE(IDB_WIZHEADER);
}

CTDLWelcomeWizard::~CTDLWelcomeWizard()
{
}


BEGIN_MESSAGE_MAP(CTDLWelcomeWizard, CPropertySheetEx)
	//{{AFX_MSG_MAP(CWelcomeWizard)
	//}}AFX_MSG_MAP
	ON_COMMAND(ID_WIZFINISH, OnWizFinish)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CWelcomeWizard message handlers

BOOL CTDLWelcomeWizard::OnInitDialog() 
{
	CPropertySheetEx::OnInitDialog();

	CDialogHelper::SetFont(this, m_hFont);

	HICON hIcon = GraphicsMisc::LoadIcon(IDR_MAINFRAME);
	SetIcon(hIcon, FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLWelcomeWizard::OnWizFinish()
{	
	m_page1.UpdateData();
	m_page2.UpdateData();
	m_page3.UpdateData();

	EndDialog(ID_WIZFINISH);
}

/////////////////////////////////////////////////////////////////////////////
// CWelcomePage1 property page

IMPLEMENT_DYNCREATE(CTDLWelcomePage1, CPropertyPageEx)

CTDLWelcomePage1::CTDLWelcomePage1() : CPropertyPageEx(CTDLWelcomePage1::IDD)
{
	//{{AFX_DATA_INIT(CWelcomePage1)
	m_bShareTasklists = 0;
	//}}AFX_DATA_INIT
	m_psp.dwFlags &= ~(PSP_HASHELP);		
//	m_psp.dwFlags |= PSP_DEFAULT|PSP_HIDEHEADER;

	// disable ini file is user has installed to 'program files'
	m_bInstalledInProgFiles = IsInstalledInProgramFiles();
	m_bUseIniFile = !m_bInstalledInProgFiles;
	m_sIniLabel = CEnString(m_bInstalledInProgFiles ? IDS_DISABLEINILABEL : IDS_DEFAULTINILABEL);
}

CTDLWelcomePage1::~CTDLWelcomePage1()
{
}

void CTDLWelcomePage1::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPageEx::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CWelcomePage1)
	DDX_Radio(pDX, IDC_NOSHARE, m_bShareTasklists);
	DDX_Radio(pDX, IDC_REGISTRY, m_bUseIniFile);
	DDX_Text(pDX, IDC_INILABEL, m_sIniLabel);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CTDLWelcomePage1, CPropertyPageEx)
	//{{AFX_MSG_MAP(CWelcomePage1)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CWelcomePage1 message handlers

BOOL CTDLWelcomePage1::OnInitDialog() 
{
	CDialogHelper::SetFont(this, m_hFont);
	CPropertyPageEx::OnInitDialog();

	// disable ini file is user has installed to 'program files'
	GetDlgItem(IDC_INIFILE)->EnableWindow(!m_bInstalledInProgFiles);
	GetDlgItem(IDC_INILABEL)->EnableWindow(!m_bInstalledInProgFiles);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CTDLWelcomePage1::IsInstalledInProgramFiles()
{
	CString sAppPath = FileMisc::GetAppFolder(), sProgFiles;

	if (!FileMisc::GetSpecialFolder(CSIDL_PROGRAM_FILES, sProgFiles))
		return FALSE;

	sAppPath.MakeUpper();
	sProgFiles.MakeUpper();

	return (sAppPath.Left(sProgFiles.GetLength()) == sProgFiles);
}

/////////////////////////////////////////////////////////////////////////////
// CWelcomePage2 property page

IMPLEMENT_DYNCREATE(CTDLWelcomePage2, CPropertyPageEx)

CTDLWelcomePage2::CTDLWelcomePage2() : CPropertyPageEx(CTDLWelcomePage2::IDD)
{
	//{{AFX_DATA_INIT(CWelcomePage2)
	//}}AFX_DATA_INIT
	m_psp.dwFlags &= ~(PSP_HASHELP);		
	m_psp.dwFlags |= PSP_DEFAULT|PSP_HIDEHEADER;
}

CTDLWelcomePage2::~CTDLWelcomePage2()
{
}

void CTDLWelcomePage2::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPageEx::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CWelcomePage2)
	DDX_Control(pDX, IDC_COLUMNLIST, m_lbColumns);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLWelcomePage2, CPropertyPageEx)
	//{{AFX_MSG_MAP(CWelcomePage2)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CWelcomePage2 message handlers

BOOL CTDLWelcomePage2::OnInitDialog() 
{
	CDialogHelper::SetFont(this, m_hFont);

	return CPropertyPageEx::OnInitDialog();
}

BOOL CTDLWelcomePage2::OnSetActive() 
{
	((CPropertySheet*)GetParent())->SetWizardButtons(PSWIZB_BACK | PSWIZB_NEXT);
	
	return CPropertyPageEx::OnSetActive();
}

/////////////////////////////////////////////////////////////////////////////
// CWelcomePage3 property page

IMPLEMENT_DYNCREATE(CTDLWelcomePage3, CPropertyPageEx)

CTDLWelcomePage3::CTDLWelcomePage3() : CPropertyPageEx(CTDLWelcomePage3::IDD),
	m_eSampleTasklist(FES_COMBOSTYLEBTN | FES_RELATIVEPATHS)
{
	//{{AFX_DATA_INIT(CWelcomePage3)
	m_bHideAttrib = 1;
	m_bViewSample = 1;
	//}}AFX_DATA_INIT

	CEnString sFilter(IDS_TDLFILEOPENFILTER);
	m_eSampleTasklist.SetFilter(sFilter);
	m_eSampleTasklist.SetCurrentFolder(FileMisc::GetAppResourceFolder() + _T("\\TaskLists"));
	m_sSampleTaskList = "Introduction.tdl";

	m_psp.dwFlags &= ~(PSP_HASHELP);		
	m_psp.dwFlags |= PSP_DEFAULT|PSP_HIDEHEADER;
}

CTDLWelcomePage3::~CTDLWelcomePage3()
{
}

void CTDLWelcomePage3::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPageEx::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CWelcomePage3)
	DDX_Control(pDX, IDC_SAMPLETASKLIST, m_eSampleTasklist);
	DDX_Text(pDX, IDC_SAMPLETASKLIST, m_sSampleTaskList);
	DDX_Radio(pDX, IDC_ALLOPTIONS, m_bHideAttrib);
	DDX_Radio(pDX, IDC_NOSAMPLE, m_bViewSample);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLWelcomePage3, CPropertyPageEx)
	//{{AFX_MSG_MAP(CWelcomePage3)
	ON_BN_CLICKED(IDC_NOSAMPLE, OnNosample)
	ON_BN_CLICKED(IDC_SAMPLE, OnSample)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CWelcomePage3 message handlers

BOOL CTDLWelcomePage3::OnInitDialog() 
{
	CDialogHelper::SetFont(this, m_hFont);
	CPropertyPageEx::OnInitDialog();
	
	m_eSampleTasklist.SetButtonWidthDLU(1, 14);
	m_eSampleTasklist.EnableWindow(m_bViewSample);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLWelcomePage3::OnNosample() 
{
	UpdateData();
	m_eSampleTasklist.EnableWindow(m_bViewSample);
}

CString CTDLWelcomePage3::GetSampleFilePath() const 
{ 
	if (m_bViewSample)
		return FileMisc::GetFullPath(m_sSampleTaskList, FileMisc::GetAppResourceFolder() + _T("\\TaskLists"));
	else
		return _T("");
}

void CTDLWelcomePage3::OnSample() 
{
	UpdateData();
	m_eSampleTasklist.EnableWindow(m_bViewSample);
}


BOOL CTDLWelcomePage3::OnSetActive() 
{
	((CPropertySheetEx*)GetParent())->SetWizardButtons(PSWIZB_BACK | PSWIZB_FINISH);
	
	return CPropertyPageEx::OnSetActive();
}


