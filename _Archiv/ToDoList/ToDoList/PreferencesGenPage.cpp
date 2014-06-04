// PreferencesGenPage.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "PreferencesGenPage.h"

#include "..\shared\OSVersion.h"
#include "..\shared\dialoghelper.h"
#include "..\shared\enstring.h"
#include "..\shared\filemisc.h"
#include "..\shared\localizer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#ifndef BCM_SETSHIELD
#	define BCM_SETSHIELD 0x0000160C
#endif
/////////////////////////////////////////////////////////////////////////////
// CPreferencesGenPage property page

IMPLEMENT_DYNCREATE(CPreferencesGenPage, CPreferencesPageBase)

CPreferencesGenPage::CPreferencesGenPage() : 
   CPreferencesPageBase(CPreferencesGenPage::IDD), 
   m_hkGlobal(TRUE), 
   m_cbLanguages(_T("*.csv")),
   m_eStickiesPath(FES_COMBOSTYLEBTN, CEnString(IDS_STICKIESEXEFILTER))
{
	//{{AFX_DATA_INIT(CPreferencesGenPage)
	m_bMinimizeNoChange = FALSE;
	m_bUseStickies = FALSE;
	m_sStickiesPath = _T("");
	m_bReloadTasklists = FALSE;
	//}}AFX_DATA_INIT

}

CPreferencesGenPage::~CPreferencesGenPage()
{
}

void CPreferencesGenPage::DoDataExchange(CDataExchange* pDX)
{
	CPreferencesPageBase::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPreferencesGenPage)
	DDX_Control(pDX, IDC_NOCHANGETIME, m_cbNoEditTime);
	DDX_Control(pDX, IDC_LANGUAGE, m_cbLanguages);
	DDX_Control(pDX, IDC_VISTASHIELD, m_stVistaShield);
	DDX_Control(pDX, IDC_GLOBALHOTKEY, m_hkGlobal);
	DDX_Check(pDX, IDC_MULTIINSTANCE, m_bMultiInstance);
	DDX_Check(pDX, IDC_SHOWONSTARTUP, m_bShowOnStartup);
	DDX_CBIndex(pDX, IDC_SYSTRAYOPTION, m_nSysTrayOption);
	DDX_Check(pDX, IDC_TOGGLETRAYVISIBILITY, m_bToggleTrayVisibility);
	DDX_Check(pDX, IDC_SPECIFYGLOBALHOTKEY, m_bSpecifyGlobalHotkey);
	DDX_Check(pDX, IDC_ADDFILESTOMRU, m_bAddFilesToMRU);
	DDX_Check(pDX, IDC_ENABLETDLEXTENSION, m_bEnableTDLExtension);
	DDX_Check(pDX, IDC_CHECKFORUPDATES, m_bAutoCheckForUpdates);
	DDX_Check(pDX, IDC_ESCAPEMINIMIZES, m_bEscapeMinimizes);
	DDX_Check(pDX, IDC_ENABLETDLPROTOCOL, m_bEnableTDLProtocol);
	DDX_Check(pDX, IDC_ENABLEDELAYEDLOADING, m_bEnableDelayedLoading);
	DDX_Check(pDX, IDC_ENABLEADD2DICTIONARY, m_bEnableAdd2Dictionary);
	DDX_Check(pDX, IDC_SAVEOTHERPASSWORDS, m_bSaveStoragePasswords);
	DDX_Check(pDX, IDC_MINIMIZEONNOEDIT, m_bMinimizeNoChange);
	DDX_Check(pDX, IDC_USESTICKIES, m_bUseStickies);
	DDX_Text(pDX, IDC_PATHTOSTICKIESEXE, m_sStickiesPath);
	DDX_Control(pDX, IDC_PATHTOSTICKIESEXE, m_eStickiesPath);
	DDX_Check(pDX, IDC_RELOADTASKLISTS, m_bReloadTasklists);
	//}}AFX_DATA_MAP
	DDX_Check(pDX, IDC_ALWAYSONTOP, m_bAlwaysOnTop);
	DDX_Check(pDX, IDC_USESYSTRAY, m_bUseSysTray);
	DDX_Check(pDX, IDC_CONFIRMDELETE, m_bConfirmDelete);
	DDX_Check(pDX, IDC_CONFIRMSAVEONEXIT, m_bConfirmSaveOnExit);

	// custom
	if (pDX->m_bSaveAndValidate)
	{
		WORD wVKeyCode = 0, wModifiers = 0;
		m_hkGlobal.GetHotKey(wVKeyCode, wModifiers);
		m_dwGlobalHotkey = MAKELONG(wVKeyCode, wModifiers);

		m_nMinimizeNoEditTime = CDialogHelper::GetSelectedItemAsValue(m_cbNoEditTime);
	}
	else
	{
		if (m_bSpecifyGlobalHotkey)
			m_hkGlobal.SetHotKey(m_dwGlobalHotkey);

		if (!CDialogHelper::SelectItemByValue(m_cbNoEditTime, m_nMinimizeNoEditTime))
		{
			m_nMinimizeNoEditTime = 10;
			CDialogHelper::SelectItemByValue(m_cbNoEditTime, m_nMinimizeNoEditTime);
		}
	}
}

BEGIN_MESSAGE_MAP(CPreferencesGenPage, CPreferencesPageBase)
	//{{AFX_MSG_MAP(CPreferencesGenPage)
	ON_BN_CLICKED(IDC_SPECIFYGLOBALHOTKEY, OnSpecifyglobalhotkey)
	ON_BN_CLICKED(IDC_MULTIINSTANCE, OnMultiinstance)
	ON_BN_CLICKED(IDC_CLEARMRU, OnClearMRU)
	ON_CBN_SELCHANGE(IDC_LANGUAGE, OnSelchangeLanguage)
	ON_BN_CLICKED(IDC_ENABLEADD2DICTIONARY, OnEnableAdd2dictionary)
	ON_BN_CLICKED(IDC_MINIMIZEONNOEDIT, OnMinimizeonnoedit)
	ON_BN_CLICKED(IDC_USESTICKIES, OnUseStickies)
	ON_BN_CLICKED(IDC_CLEANUP_DICTIONARY, OnCleanupDictionary)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_USESYSTRAY, OnUseSystray)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPreferencesGenPage message handlers

BOOL CPreferencesGenPage::OnInitDialog() 
{
	CDialog::OnInitDialog();

	GetDlgItem(IDC_SYSTRAYOPTION)->EnableWindow(m_bUseSysTray);
	GetDlgItem(IDC_TOGGLETRAYVISIBILITY)->EnableWindow(m_bUseSysTray);
	GetDlgItem(IDC_MULTIINSTANCE)->EnableWindow(!m_bSpecifyGlobalHotkey);
	GetDlgItem(IDC_SPECIFYGLOBALHOTKEY)->EnableWindow(!m_bMultiInstance);
	GetDlgItem(IDC_GLOBALHOTKEY)->EnableWindow(!m_bMultiInstance && m_bSpecifyGlobalHotkey);
	GetDlgItem(IDC_NOCHANGETIME)->EnableWindow(m_bMinimizeNoChange);
	GetDlgItem(IDC_STICKIESPATHLABEL)->EnableWindow(m_bUseStickies);
	GetDlgItem(IDC_PATHTOSTICKIESEXE)->EnableWindow(m_bUseStickies);

	if (COSVersion() <= OSV_XP)
		GetDlgItem(IDC_VISTASHIELD)->ShowWindow(SW_HIDE);

	// validate current language
	OnSelchangeLanguage();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPreferencesGenPage::OnUseSystray() 
{
	UpdateData();

	GetDlgItem(IDC_SYSTRAYOPTION)->EnableWindow(m_bUseSysTray);
	GetDlgItem(IDC_TOGGLETRAYVISIBILITY)->EnableWindow(m_bUseSysTray);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesGenPage::OnSpecifyglobalhotkey() 
{
	UpdateData();
	
	GetDlgItem(IDC_GLOBALHOTKEY)->EnableWindow(m_bSpecifyGlobalHotkey);
	GetDlgItem(IDC_MULTIINSTANCE)->EnableWindow(!m_bSpecifyGlobalHotkey);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesGenPage::OnMultiinstance() 
{
	UpdateData();
	
	GetDlgItem(IDC_SPECIFYGLOBALHOTKEY)->EnableWindow(!m_bMultiInstance);
	GetDlgItem(IDC_GLOBALHOTKEY)->EnableWindow(!m_bMultiInstance && m_bSpecifyGlobalHotkey);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesGenPage::OnClearMRU() 
{
   GetParent()->SendMessage(WM_PGP_CLEARMRU);
}

void CPreferencesGenPage::LoadPreferences(const CPreferences& prefs)
{
	// load settings
	m_bAlwaysOnTop = prefs.GetProfileInt(_T("Preferences"), _T("AlwaysOnTop"), FALSE);
	m_bUseSysTray = prefs.GetProfileInt(_T("Preferences"), _T("UseSysTray"), FALSE);
	m_bConfirmDelete = prefs.GetProfileInt(_T("Preferences"), _T("ConfirmDelete"), FALSE);
	m_bConfirmSaveOnExit = prefs.GetProfileInt(_T("Preferences"), _T("ConfirmSaveOnExit"), TRUE);
	m_bMultiInstance = prefs.GetProfileInt(_T("Preferences"), _T("MultiInstance"), FALSE);
	m_bShowOnStartup = prefs.GetProfileInt(_T("Preferences"), _T("ShowOnStartup"), TRUE);
	m_nSysTrayOption = prefs.GetProfileInt(_T("Preferences"), _T("SysTrayOption"), STO_ONCLOSE);
	m_bToggleTrayVisibility = prefs.GetProfileInt(_T("Preferences"), _T("ToggleTrayVisibility"), FALSE);
	m_dwGlobalHotkey = prefs.GetProfileInt(_T("Preferences"), _T("GlobalHotkey"), 0);
	m_bSpecifyGlobalHotkey = (m_dwGlobalHotkey ? 1 : 0);
	m_bAddFilesToMRU = prefs.GetProfileInt(_T("Preferences"), _T("AddFilesToMRU"), TRUE);
	m_bEnableTDLExtension = prefs.GetProfileInt(_T("Preferences"), _T("EnableTDLExtension"), TRUE);
	m_bEnableTDLProtocol = prefs.GetProfileInt(_T("Preferences"), _T("EnableTDLProtocol"), FALSE);
	m_bAutoCheckForUpdates = prefs.GetProfileInt(_T("Preferences"), _T("AutoCheckForUpdates"), FALSE);
	m_bEscapeMinimizes = prefs.GetProfileInt(_T("Preferences"), _T("EscapeMinimizes"), TRUE);
	m_bEnableDelayedLoading = prefs.GetProfileInt(_T("Preferences"), _T("EnableDelayedLoading"), TRUE);
	m_bSaveStoragePasswords = prefs.GetProfileInt(_T("Preferences"), _T("SaveStoragePasswords"), FALSE);
	m_nMinimizeNoEditTime = prefs.GetProfileInt(_T("Preferences"), _T("MinimizeNoEditTime"), 5);
	m_bMinimizeNoChange = prefs.GetProfileInt(_T("Preferences"), _T("MinimizeNoEdit"), FALSE);
	m_bReloadTasklists = prefs.GetProfileInt(_T("Preferences"), _T("ReloadTasklists"), TRUE);

	m_bEnableAdd2Dictionary = prefs.GetProfileInt(_T("Preferences"), _T("EnableAdd2Dictionary"), FALSE);
	CString sLangFile = prefs.GetProfileString(_T("Preferences"), _T("LanguageFile"), _T(""));
	m_cbLanguages.SetLanguageFile(sLangFile);

	m_bUseStickies = prefs.GetProfileInt(_T("Preferences"), _T("UseStickies"), FALSE);
	m_sStickiesPath = prefs.GetProfileString(_T("Preferences"), _T("PathToStickies"));

//	m_b = prefs.GetProfileInt(_T("Preferences"), _T(""), TRUE);
}

void CPreferencesGenPage::SavePreferences(CPreferences& prefs)
{
	// save settings
	prefs.WriteProfileInt(_T("Preferences"), _T("AlwaysOnTop"), m_bAlwaysOnTop);
	prefs.WriteProfileInt(_T("Preferences"), _T("UseSysTray"), m_bUseSysTray);
	prefs.WriteProfileInt(_T("Preferences"), _T("ConfirmDelete"), m_bConfirmDelete);
	prefs.WriteProfileInt(_T("Preferences"), _T("ConfirmSaveOnExit"), m_bConfirmSaveOnExit);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowOnStartup"), m_bShowOnStartup);
	prefs.WriteProfileInt(_T("Preferences"), _T("SysTrayOption"), m_nSysTrayOption);
	prefs.WriteProfileInt(_T("Preferences"), _T("ToggleTrayVisibility"), m_bToggleTrayVisibility);
	prefs.WriteProfileInt(_T("Preferences"), _T("MultiInstance"), m_bMultiInstance);
	prefs.WriteProfileInt(_T("Preferences"), _T("GlobalHotkey"), (m_bSpecifyGlobalHotkey ? m_dwGlobalHotkey : 0));
	prefs.WriteProfileInt(_T("Preferences"), _T("AddFilesToMRU"), m_bAddFilesToMRU);
	prefs.WriteProfileInt(_T("Preferences"), _T("EnableTDLExtension"), m_bEnableTDLExtension);
	prefs.WriteProfileInt(_T("Preferences"), _T("EnableTDLProtocol"), m_bEnableTDLProtocol);
	prefs.WriteProfileInt(_T("Preferences"), _T("AutoCheckForUpdates"), m_bAutoCheckForUpdates);
	prefs.WriteProfileInt(_T("Preferences"), _T("EscapeMinimizes"), m_bEscapeMinimizes);
	prefs.WriteProfileInt(_T("Preferences"), _T("EnableDelayedLoading"), m_bEnableDelayedLoading);
	prefs.WriteProfileInt(_T("Preferences"), _T("EnableAdd2Dictionary"), m_bEnableAdd2Dictionary);
	prefs.WriteProfileInt(_T("Preferences"), _T("SaveStoragePasswords"), m_bSaveStoragePasswords);
	prefs.WriteProfileString(_T("Preferences"), _T("LanguageFile"), m_cbLanguages.GetLanguageFile(TRUE)); // relative path
	prefs.WriteProfileInt(_T("Preferences"), _T("MinimizeNoEditTime"), m_nMinimizeNoEditTime);
	prefs.WriteProfileInt(_T("Preferences"), _T("MinimizeNoEdit"), m_bMinimizeNoChange);
	prefs.WriteProfileInt(_T("Preferences"), _T("UseStickies"), m_bUseStickies);
	prefs.WriteProfileString(_T("Preferences"), _T("PathToStickies"), m_sStickiesPath);
	prefs.WriteProfileInt(_T("Preferences"), _T("ReloadTasklists"), m_bReloadTasklists);

	//	prefs.WriteProfileInt(_T("Preferences"), _T(""), m_b);
}

void CPreferencesGenPage::OnSelchangeLanguage() 
{
	BOOL bDefLang = m_cbLanguages.IsDefaultLanguage();

	// if the user has switched to the default language and there is
	// a 'YourLanguage' in the list uncheck 'Add2Dictionary'
	if (m_bEnableAdd2Dictionary && bDefLang && m_cbLanguages.HasYourLanguage())
	{
		m_bEnableAdd2Dictionary = FALSE;
		UpdateData(FALSE);
	}

	// disable 'Add to Dict' for default language
	GetDlgItem(IDC_ENABLEADD2DICTIONARY)->EnableWindow(!bDefLang);

	// disable the cleanup button for default language
	// or if 'Add to Dict' is disabled
	CString sLangFile = m_cbLanguages.GetLanguageFile();

	BOOL bEnable = (!bDefLang && m_bEnableAdd2Dictionary &&
					sLangFile == CLocalizer::GetDictionaryPath());

	GetDlgItem(IDC_CLEANUP_DICTIONARY)->EnableWindow(bEnable);
}

void CPreferencesGenPage::OnEnableAdd2dictionary() 
{
	UpdateData();

	// if the user has just switched 'Add2Dictionary' On and we are on the
	// default language (english) then look for 'YourLanguage' and change
	// the language to that
	if (m_bEnableAdd2Dictionary && m_cbLanguages.IsDefaultLanguage())
	{
		if (m_cbLanguages.FindStringExact(0, _T("YourLanguage")) != -1)
			m_cbLanguages.SelectLanguage(_T("YourLanguage"));
	}
}

void CPreferencesGenPage::OnMinimizeonnoedit() 
{
	UpdateData();
	GetDlgItem(IDC_NOCHANGETIME)->EnableWindow(m_bMinimizeNoChange);
}

void CPreferencesGenPage::OnUseStickies() 
{
	UpdateData();

	GetDlgItem(IDC_STICKIESPATHLABEL)->EnableWindow(m_bUseStickies);
	GetDlgItem(IDC_PATHTOSTICKIESEXE)->EnableWindow(m_bUseStickies);
}

BOOL CPreferencesGenPage::GetUseStickies(CString& sStickiesPath) const
{ 
	if (m_bUseStickies && FileMisc::FileExists(m_sStickiesPath))
	{
		sStickiesPath = m_sStickiesPath;
		return TRUE;
	}
	
	// else
	return FALSE; 
}

void CPreferencesGenPage::OnCleanupDictionary() 
{
	GetParent()->SendMessage(WM_PGP_CLEANUPDICTIONARY, 0L, (LPARAM)(LPCTSTR)m_cbLanguages.GetLanguageFile());
}
