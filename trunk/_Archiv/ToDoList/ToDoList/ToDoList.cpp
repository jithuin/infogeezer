// ToDoList.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "ToDoList.h"
#include "ToDoListWnd.h"
#include "Preferencesdlg.h"
#include "welcomewizard.h"
#include "tdcmsg.h"
#include "tdlprefmigrationdlg.h"
#include "tdllanguagedlg.h"
#include "TDLCmdlineOptionsDlg.h"
#include "tdlolemessagefilter.h"

#include "..\shared\LimitSingleInstance.h"
#include "..\shared\encommandlineinfo.h"
#include "..\shared\driveinfo.h"
#include "..\shared\enfiledialog.h"
#include "..\shared\enstring.h"
#include "..\shared\regkey.h"
#include "..\shared\filemisc.h"
#include "..\shared\autoflag.h"
#include "..\shared\preferences.h"
#include "..\shared\localizer.h"

#include "..\3rdparty\xmlnodewrapper.h"
#include "..\3rdparty\ini.h"

#include <afxpriv.h>

#include <shlwapi.h>
#pragma comment(lib, "shlwapi.lib")

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

CLimitSingleInstance g_SingleInstanceObj(_T("{3A4EFC98-9BA9-473D-A3CF-6B0FE644470D}")); 

BOOL CALLBACK FindOtherInstance(HWND hwnd, LPARAM lParam);

LPCTSTR REGKEY = _T("AbstractSpoon");
LPCTSTR APPREGKEY = _T("Software\\AbstractSpoon\\ToDoList");

LPCTSTR ONLINEHELP = _T("http://abstractspoon.pbwiki.com/"); 
LPCTSTR CONTACTUS = _T("mailto:abstractspoon2@optusnet.com.au"); 
LPCTSTR FEEDBACKANDSUPPORT = _T("http://www.codeproject.com/KB/applications/todolist2.aspx"); 
LPCTSTR LICENSE = _T("http://www.opensource.org/licenses/eclipse-1.0.php"); 
LPCTSTR ONLINE = _T("http://www.abstractspoon.com/tdl_resources.html"); 
LPCTSTR WIKI = _T("http://abstractspoon.pbwiki.com/"); 
LPCTSTR DONATE = _T("https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business=abstractspoon2%40optusnet%2ecom%2eau&item_name=Software"); 

LPCTSTR FILESTATEKEY = _T("FileStates");
LPCTSTR REMINDERKEY = _T("Reminders");
LPCTSTR DEFAULTKEY = _T("Default");

/////////////////////////////////////////////////////////////////////////////
// CToDoListApp

BEGIN_MESSAGE_MAP(CToDoListApp, CWinApp)
	//{{AFX_MSG_MAP(CToDoListApp)
	ON_COMMAND(ID_HELP_CONTACTUS, OnHelpContactus)
	ON_COMMAND(ID_HELP_FEEDBACKANDSUPPORT, OnHelpFeedbackandsupport)
	ON_COMMAND(ID_HELP_LICENSE, OnHelpLicense)
	ON_COMMAND(ID_HELP_WIKI, OnHelpWiki)
	ON_COMMAND(ID_HELP_COMMANDLINE, OnHelpCommandline)
	ON_COMMAND(ID_HELP_DONATE, OnHelpDonate)
	//}}AFX_MSG_MAP
	ON_COMMAND(ID_HELP, OnHelp)
	ON_COMMAND(ID_TOOLS_IMPORTPREFS, OnImportPrefs)
	ON_UPDATE_COMMAND_UI(ID_TOOLS_IMPORTPREFS, OnUpdateImportPrefs)
	ON_COMMAND(ID_TOOLS_EXPORTPREFS, OnExportPrefs)
	ON_UPDATE_COMMAND_UI(ID_TOOLS_EXPORTPREFS, OnUpdateExportPrefs)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CToDoListApp construction

CToDoListApp::CToDoListApp() : CWinApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}

/////////////////////////////////////////////////////////////////////////////
// The one and only CToDoListApp object

CToDoListApp theApp;

/////////////////////////////////////////////////////////////////////////////
// CToDoListApp initialization

BOOL CToDoListApp::InitInstance()
{
	AfxOleInit(); // for initializing COM and handling drag and drop via explorer
	AfxEnableControlContainer(); // embedding IE

	CEnCommandLineInfo cmdInfo;
	
	if (!ParseCommandLine(&cmdInfo))
		return FALSE;

	// see if the user just wants to see the commandline options
	if (cmdInfo.HasOption(_T("h")) || cmdInfo.HasOption(_T("help")) || 
		cmdInfo.HasOption(_T("?")))
	{
		OnHelpCommandline();
		return FALSE;
	}

	// before anything else make sure we've got MSXML3 installed
	if (!CXmlDocumentWrapper::IsVersion3orGreater())
	{
		AfxMessageBox(IDS_BADMSXML);
		return FALSE;
	}

	// init prefs 
	if (!InitPreferences(&cmdInfo))
		return FALSE;

	// commandline options
	TDCSTARTUP startup(cmdInfo);

	// does the user want single instance only
	BOOL bSingleInstance = !CPreferencesDlg().GetMultiInstance();

	if (bSingleInstance && g_SingleInstanceObj.IsAnotherInstanceRunning())
	{
		HWND hWnd = NULL;
		EnumWindows(FindOtherInstance, (LPARAM)&hWnd);
		
		// make sure its not closing
		if (hWnd && !::SendMessage(hWnd, WM_TDL_ISCLOSING, 0, 0))
		{
/*			// check version
			int nVer = ::SendMessage(hWnd, WM_TDL_GETVERSION, 0, 0);
			
			if (nVer < CToDoListWnd::GetVersion())
				MessageBox(NULL, CEnString(IDS_EARLIERVERRUNNING), CEnString(IDS_COPYRIGHT), MB_OK);
*/			
			::SendMessage(hWnd, WM_TDL_SHOWWINDOW, 0, 0);

			// pass on file to open
			COPYDATASTRUCT cds;
			
			cds.dwData = TDL_STARTUP;
			cds.cbData = sizeof(startup);
			cds.lpData = (void*)&startup;
			
			::SendMessage(hWnd, WM_COPYDATA, NULL, (LPARAM)&cds);
			::SetForegroundWindow(hWnd);
						
			return FALSE;
		}
	}

	CToDoListWnd* pTDL = new CToDoListWnd;
	
	if (pTDL && pTDL->Create(startup))
	{
		m_pMainWnd = pTDL;
		return TRUE;
	}

	// else
	return FALSE;
}

// our own local version
CString CToDoListApp::AfxGetAppName()
{
	return ((CToDoListWnd*)m_pMainWnd)->GetTitle();
}

BOOL CToDoListApp::ParseCommandLine(CEnCommandLineInfo* pInfo)
{
	ASSERT (pInfo);

	CWinApp::ParseCommandLine(*pInfo); // default

	// check for task link
	CString sLink;

	if (pInfo->GetOption(_T("l"), sLink) && sLink.Find(TDL_PROTOCOL) != -1)
	{
		CString sFilePath;
		DWORD dwID = 0;

		CToDoCtrl::ParseTaskLink(sLink, dwID, sFilePath);

		if (!sFilePath.IsEmpty() && dwID)
		{
			// remove possible trailing slash on file path
			sFilePath.TrimRight('\\');

			// replace possible %20 by spaces
			sFilePath.Replace(_T("%20"), _T(" "));

			// verify the file existence unless the path is relative
			if (/*PathIsRelative(sFilePath) || */FileMisc::FileExists(sFilePath))
			{
				pInfo->m_strFileName = sFilePath;
				pInfo->SetOption(_T("tid"), dwID);
			}
			else
			{
				pInfo->m_strFileName.Empty();
				pInfo->DeleteOption(_T("tid"));

				CEnString sMessage(IDS_TDLLINKLOADFAILED, sFilePath);
				AfxMessageBox(sMessage);

				return FALSE;
			}
		}
	}
	// else validate file path
	else if (!FileMisc::FileExists(pInfo->m_strFileName))
		pInfo->m_strFileName.Empty();

	return TRUE;
}

BOOL CALLBACK FindOtherInstance(HWND hwnd, LPARAM lParam)
{
	CString sCaption;

	int nLen = ::GetWindowTextLength(hwnd);
	::GetWindowText(hwnd, sCaption.GetBuffer(nLen + 1), nLen + 1);
	sCaption.ReleaseBuffer();

	if (sCaption.Find(CEnString(IDS_COPYRIGHT)) != -1)
	{
		HWND* pWnd = (HWND*)lParam;
		*pWnd = hwnd;
		return FALSE;
	}

	return TRUE;
}

BOOL CToDoListApp::PreTranslateMessage(MSG* pMsg) 
{
	// give first chance to main window for handling accelerators
	if (m_pMainWnd && m_pMainWnd->PreTranslateMessage(pMsg))
		return TRUE;

	// -------------------------------------------------------------------
	// Implement CWinApp::PreTranslateMessage(pMsg)	ourselves
	// so as to not call CMainFrame::PreTranslateMessage(pMsg) twice

	// if this is a thread-message, short-circuit this function
	if (pMsg->hwnd == NULL && DispatchThreadMessageEx(pMsg))
		return TRUE;

	// walk from target to main window but excluding main window
	for (HWND hWnd = pMsg->hwnd; 
		hWnd != NULL && hWnd != m_pMainWnd->GetSafeHwnd(); 
		hWnd = ::GetParent(hWnd))
	{
		CWnd* pWnd = CWnd::FromHandlePermanent(hWnd);

		if (pWnd != NULL)
		{
			// target window is a C++ window
			if (pWnd->PreTranslateMessage(pMsg))
				return TRUE; // trapped by target window (eg: accelerators)
		}
	}
	// -------------------------------------------------------------------

	return FALSE;       // no special processing
	//return CWinApp::PreTranslateMessage(pMsg);
}

void CToDoListApp::OnHelp() 
{ 
	DoHelp();
}

void CToDoListApp::WinHelp(DWORD dwData, UINT nCmd) 
{
	if (nCmd == HELP_CONTEXT)
		DoHelp((LPCTSTR)dwData);
}

void CToDoListApp::DoHelp(const CString& sHelpRef)
{
	CString sHelpUrl(ONLINEHELP);

	if (sHelpRef.IsEmpty())
		sHelpUrl += _T("FrontPage");
	else
		sHelpUrl += sHelpRef;

	FileMisc::Run(*m_pMainWnd, sHelpUrl, NULL, SW_SHOWNORMAL);
}

BOOL CToDoListApp::InitPreferences(CEnCommandLineInfo* pInfo)
{
	BOOL bUseIni = FALSE;

	// get the app file path
	CString sTemp = FileMisc::GetAppFileName(), sDrive, sFolder, sAppName(_T("ToDoList"));
	FileMisc::SplitPath(sTemp, &sDrive, &sFolder);

    // try command line first
    CString sIniPath;

    if (pInfo->GetOption(_T("i"), sIniPath) && !sIniPath.IsEmpty())
    {
		// prefix application path if path is relative
		if (PathIsRelative(sIniPath))
			FileMisc::MakePath(sIniPath, sDrive, sFolder, sIniPath);

        bUseIni = FileMisc::FileExists(sIniPath);
    }

	// else if there is a tasklist on the commandline then try for an
	// ini file of the same name
	if (!bUseIni && !pInfo->m_strFileName.IsEmpty())
	{
	    sIniPath = pInfo->m_strFileName;
		FileMisc::ReplaceExtension(sIniPath, _T("ini"));

        bUseIni = FileMisc::FileExists(sIniPath);

		// if we found one then make sure it does not have single 
		// instance specified
		if (bUseIni)
			WriteProfileInt(_T("Preferences"), _T("MultiInstance"), TRUE);
	}
	
    // else try for an ini file having the same name as the executable
	if (!bUseIni)
    {
		FileMisc::MakePath(sIniPath, sDrive, sFolder, sAppName, _T(".ini"));
		bUseIni = FileMisc::FileExists(sIniPath);
	}

	// Has the user already chosen a language?
	CString sLanguageFile;
	BOOL bAdd2Dictionary = FALSE;
	BOOL bFirstTime = (!bUseIni && !CRegKey::KeyExists(HKEY_CURRENT_USER, _T("Software\\Abstractspoon\\ToDoList")));

	// check existing prefs
	if (!bFirstTime)
	{
		if (bUseIni)
		{
			free((void*)m_pszProfileName);
			m_pszProfileName = _tcsdup(sIniPath);
		}
		else
		{
			SetRegistryKey(REGKEY);
		}

		CPreferences prefs;
		bAdd2Dictionary = prefs.GetProfileInt(_T("Preferences"), _T("EnableAdd2Dictionary"), FALSE);

		// language is stored as relative path
		sLanguageFile = prefs.GetProfileString(_T("Preferences"), _T("LanguageFile"));

		if (!sLanguageFile.IsEmpty() && sLanguageFile != CTDLLanguageComboBox::GetDefaultLanguage())
		{
#ifdef _DEBUG
			// Always point to 'real' source so we update the original
			FileMisc::MakeFullPath(sLanguageFile, _T("D:\\_Code\\ToDoList"));
#else
			FileMisc::MakeFullPath(sLanguageFile, FileMisc::GetAppFolder());
#endif
		}
		else if (bAdd2Dictionary)
		{
			sLanguageFile = CTDLLanguageComboBox::GetLanguageFile(_T("YourLanguage"), _T("csv"));
		}
	}
	// else show language dialog
	if (sLanguageFile.IsEmpty())
	{
		CTDLLanguageDlg dialog;
		
		if (dialog.DoModal() == IDCANCEL)
			return FALSE;

		// else
		sLanguageFile = dialog.GetLanguageFile();
	}

	// init language translation. 
	// 'u' indicates uppercase mode
	if (pInfo->HasOption(_T("u")))
	{
		CLocalizer::Release();
		CLocalizer::Initialize(sLanguageFile, ITTTO_UPPERCASE);
	}
	// 't' indicates 'translation' mode (aka 'Add2Dictionary')
	else if (FileMisc::FileExists(sLanguageFile))
	{
		CLocalizer::Release();

		if (bAdd2Dictionary || pInfo->HasOption(_T("t")))
			CLocalizer::Initialize(sLanguageFile, ITTTO_ADD2DICTIONARY);
		else
			CLocalizer::Initialize(sLanguageFile, ITTTO_TRANSLATEONLY);
	}
	
	// save it off
	CPreferences prefs;

	if (!bFirstTime)
	{
		FileMisc::MakeRelativePath(sLanguageFile, FileMisc::GetAppFolder(), FALSE);
		prefs.WriteProfileString(_T("Preferences"), _T("LanguageFile"), sLanguageFile);

		prefs.WriteProfileInt(_T("Preferences"), _T("EnableAdd2Dictionary"), bAdd2Dictionary);
		prefs.Save();
		
		UpgradePreferences(prefs);
	}
	else  // first time so no ini file exists. show wizard
	{
		CTDLWelcomeWizard wizard;
		
		if (wizard.DoModal() != ID_WIZFINISH)
		{
			// delete ini file that was created to store language choice
			if (bUseIni)
				::DeleteFile(sIniPath);

			return FALSE; // user cancelled so quit app
		}
		
		bUseIni = wizard.GetUseIniFile();

		if (bUseIni)
		{
			free((void*)m_pszProfileName);
			m_pszProfileName = _tcsdup(sIniPath);
		}
		else
			SetRegistryKey(REGKEY);

		// initialize prefs to defaults
		CPreferences prefs;
		CPreferencesDlg dlg;

		dlg.Initialize(prefs);

		// set up some default preferences
		if (wizard.GetShareTasklists()) 
		{
			// set up source control for remote tasklists
			prefs.WriteProfileInt(_T("Preferences"), _T("EnableSourceControl"), TRUE);
			prefs.WriteProfileInt(_T("Preferences"), _T("SourceControlLanOnly"), TRUE);
			prefs.WriteProfileInt(_T("Preferences"), _T("AutoCheckOut"), TRUE);
			prefs.WriteProfileInt(_T("Preferences"), _T("CheckoutOnCheckin"), TRUE);
			prefs.WriteProfileInt(_T("Preferences"), _T("CheckinOnClose"), TRUE);
			prefs.WriteProfileInt(_T("Preferences"), _T("CheckinNoEditTime"), 1);
			prefs.WriteProfileInt(_T("Preferences"), _T("CheckinNoEdit"), TRUE);
			prefs.WriteProfileInt(_T("Preferences"), _T("Use3rdPartySourceControl"), FALSE);
		}
		
		// setup default columns
		CTDCColumnIDArray aColumns;
		wizard.GetVisibleColumns(aColumns);
		
		int nCol = aColumns.GetSize();
		prefs.WriteProfileInt(_T("Preferences\\ColumnVisibility"), _T("Count"), nCol);
		
		while (nCol--)
		{
			CString sKey = Misc::MakeKey(_T("Col%d"), nCol);
			prefs.WriteProfileInt(_T("Preferences\\ColumnVisibility"), sKey, aColumns[nCol]);
		}
		
		if (wizard.GetHideAttributes())
		{
			// hide clutter
			prefs.WriteProfileInt(_T("Preferences"), _T("ShowCtrlsAsColumns"), TRUE);
			prefs.WriteProfileInt(_T("Preferences"), _T("ShowEditMenuAsColumns"), TRUE);
			//prefs.WriteProfileInt("Settings", "ShowFilterBar", FALSE);
		}
		
		// set up initial file
		CString sSample = wizard.GetSampleFilePath();
		
		if (!sSample.IsEmpty())
			pInfo->m_strFileName = sSample;

		// save language choice
		FileMisc::MakeRelativePath(sLanguageFile, FileMisc::GetAppFolder(), FALSE);
		prefs.WriteProfileString(_T("Preferences"), _T("LanguageFile"), sLanguageFile);
		prefs.WriteProfileInt(_T("Preferences"), _T("EnableAdd2Dictionary"), bAdd2Dictionary);
	}

	return TRUE;
}

void CToDoListApp::UpgradePreferences(CPreferences& prefs)
{
	// we don't handle the registry because it's too hard (for now)
	if (m_pszRegistryKey)
		return;
	
	// remove preferences for all files _not_ in the MRU list
	// provided there's at least one file in the MRU list
	BOOL bUseMRU = prefs.GetProfileInt(_T("Preferences"), _T("AddFilesToMRU"), FALSE);

	if (!bUseMRU)
		return;

	CStringArray aMRU;
	
	for (int nFile = 0; nFile < 16; nFile++)
	{
		CString sItem, sFile;
		
		sItem.Format(_T("TaskList%d"), nFile + 1);
		sFile = prefs.GetProfileString(_T("MRU"), sItem);
		
		if (sFile.IsEmpty())
			break;
		
		// else
		sFile = FileMisc::GetFileNameFromPath(sFile);
		Misc::AddUniqueItem(sFile, aMRU);
	}
	
	if (aMRU.GetSize())
	{
		CStringArray aSections;
		int nSection = prefs.GetSectionNames(aSections);
		
		while (nSection--)
		{
			CString sSection = aSections[nSection];
			
			// does it start with "FileStates\\" 
			if (sSection.Find(FILESTATEKEY) == 0 || sSection.Find(REMINDERKEY) == 0)
			{
				// split the section name into its parts
				CStringArray aSubSections;
				int nSubSections = Misc::Split(sSection, aSubSections, FALSE, _T("\\"));
				
				if (nSubSections > 1)
				{
					// the file name is the second item
					CString sFilename = aSubSections[1];
					
					// ignore 'Default'
					if (sFilename.CompareNoCase(DEFAULTKEY) != 0 && Misc::Find(aMRU, sFilename) == -1)
						prefs.DeleteSection(sSection);
				}
			}
		}
	}
}

int CToDoListApp::DoMessageBox(LPCTSTR lpszPrompt, UINT nType, UINT /*nIDPrompt*/) 
{
	// make sure app window is visible
	if (m_pMainWnd && (!m_pMainWnd->IsWindowVisible() || m_pMainWnd->IsIconic()))
		m_pMainWnd->SendMessage(WM_TDL_SHOWWINDOW, 0, 0);
	
	CString sTitle(AfxGetAppName()), sInstruction, sText(lpszPrompt);
	CStringArray aPrompt;
	
	int nNumInputs = Misc::Split(lpszPrompt, aPrompt, FALSE, "|");
	
	switch (nNumInputs)
	{
	case 0:
		ASSERT(0);
		break;
		
	case 1:
		// do nothing
		break;
		
	case 2:
		sInstruction = aPrompt[0];
		sText = aPrompt[1];
		break;
		
	case 3:
		sTitle.Format(_T("%s - %s"), AfxGetAppName(), aPrompt[0]);
		sInstruction = aPrompt[1];
		sText = aPrompt[2];
	}
	
	return Misc::ShowMessageBox(m_pMainWnd->GetSafeHwnd(), sTitle, sInstruction, sText, nType);
}

void CToDoListApp::OnImportPrefs() 
{
	// default location is always app folder
	CString sIniPath = FileMisc::GetAppFileName();
	sIniPath.MakeLower();
	sIniPath.Replace(_T("exe"), _T("ini"));
	
	CPreferences prefs;
	CFileOpenDialog dialog(IDS_IMPORTPREFS_TITLE, 
							_T("ini"), 
							sIniPath, 
							EOFN_DEFAULTOPEN, 
							CEnString(IDS_INIFILEFILTER));
	
	if (dialog.DoModal(&prefs) == IDOK)
	{
		CRegKey reg;
		
		if (reg.Open(HKEY_CURRENT_USER, APPREGKEY) == ERROR_SUCCESS)
		{
			sIniPath = dialog.GetPathName();
			
			if (reg.ImportFromIni(sIniPath)) // => import ini to registry
			{
				// use them now?
				// only ask if we're not already using the registry
				BOOL bUsingIni = (m_pszRegistryKey == NULL);

				if (bUsingIni)
				{
					if (AfxMessageBox(IDS_POSTIMPORTPREFS, MB_YESNO | MB_ICONQUESTION) == IDYES)
					{
						// renames existing prefs file
						CString sNewName(sIniPath);
						sNewName += _T(".bak");
						DeleteFile(sNewName);
						
						if (MoveFile(sIniPath, sNewName))
						{
							// and initialize the registry 
							SetRegistryKey(REGKEY);
							
							// reset prefs
							m_pMainWnd->SendMessage(WM_TDL_REFRESHPREFS);
						}
					}
				}
				else // reset prefs
					m_pMainWnd->SendMessage(WM_TDL_REFRESHPREFS);
			}
			else // notify user
			{
				CEnString sMessage(IDS_INVALIDPREFFILE, dialog.GetFileName());
				AfxMessageBox(sMessage, MB_OK | MB_ICONEXCLAMATION);
			}
		}
	}
}

void CToDoListApp::OnUpdateImportPrefs(CCmdUI* pCmdUI) 
{
	pCmdUI->Enable(TRUE);
}

void CToDoListApp::OnExportPrefs() 
{
	ASSERT (m_pszRegistryKey != NULL);

	CRegKey reg;

	if (reg.Open(HKEY_CURRENT_USER, APPREGKEY) == ERROR_SUCCESS)
	{
		// default location is always app folder
		CString sAppPath = FileMisc::GetAppFileName();

		CString sIniPath(sAppPath);
		sIniPath.MakeLower();
		sIniPath.Replace(_T("exe"), _T("ini"));
		
		CPreferences prefs;
		CFileSaveDialog dialog(IDS_IMPORTPREFS_TITLE, 
								_T("ini"), 
								sIniPath, 
								EOFN_DEFAULTSAVE, 
								CEnString(IDS_INIFILEFILTER));
		
		if (dialog.DoModal(&prefs) == IDOK)
		{
			BOOL bUsingReg = (m_pszRegistryKey != NULL);
			sIniPath = dialog.GetPathName();

			if (bUsingReg && reg.ExportToIni(sIniPath))
			{
				// use them now? 
				CString sAppFolder, sIniFolder;
				
				FileMisc::SplitPath(sAppPath, NULL, &sAppFolder);
				FileMisc::SplitPath(sIniPath, NULL, &sIniFolder);
				
				// only if they're in the same folder as the exe
				if (sIniFolder.CompareNoCase(sAppFolder) == 0)
				{
					if (AfxMessageBox(IDS_POSTEXPORTPREFS, MB_YESNO | MB_ICONQUESTION) == IDYES)
					{
						free((void*)m_pszRegistryKey);
						m_pszRegistryKey = NULL;
						
						free((void*)m_pszProfileName);
						m_pszProfileName = _tcsdup(sIniPath);
						
						// reset prefs
						m_pMainWnd->SendMessage(WM_TDL_REFRESHPREFS);
					}
				}
			}
		}
	}
}

void CToDoListApp::OnUpdateExportPrefs(CCmdUI* pCmdUI) 
{
	BOOL bUsingReg = (m_pszRegistryKey != NULL);
	pCmdUI->Enable(bUsingReg);
}

void CToDoListApp::OnHelpContactus() 
{
	FileMisc::Run(*m_pMainWnd, CONTACTUS, NULL, SW_SHOWNORMAL);
}

void CToDoListApp::OnHelpFeedbackandsupport() 
{
	FileMisc::Run(*m_pMainWnd, FEEDBACKANDSUPPORT, NULL, SW_SHOWNORMAL);
}

void CToDoListApp::OnHelpLicense() 
{
	FileMisc::Run(*m_pMainWnd, LICENSE, NULL, SW_SHOWNORMAL);
}

void CToDoListApp::OnHelpWiki() 
{
	FileMisc::Run(*m_pMainWnd, WIKI, NULL, SW_SHOWNORMAL);
}

void CToDoListApp::OnHelpCommandline() 
{
	CTDLCmdlineOptionsDlg dialog;
	dialog.DoModal();
}

void CToDoListApp::OnHelpDonate() 
{
	FileMisc::Run(*m_pMainWnd, DONATE, NULL, SW_SHOWNORMAL);
}

int CToDoListApp::ExitInstance() 
{
	// TODO: Add your specialized code here and/or call the base class
	CLocalizer::Release();

	return CWinApp::ExitInstance();
}
