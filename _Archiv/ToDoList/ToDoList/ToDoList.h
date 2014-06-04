// ToDoList.h : main header file for the TODOLIST application
//

#if !defined(AFX_TODOLIST_H__CA63D273_DB5E_4DBF_8915_1885E1987A65__INCLUDED_)
#define AFX_TODOLIST_H__CA63D273_DB5E_4DBF_8915_1885E1987A65__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

#include "..\shared\Localizer.h"

/////////////////////////////////////////////////////////////////////////////
// CToDoListApp:
// See ToDoList.cpp for the implementation of this class
//

class CEnCommandLineInfo;
class CPreferences;

struct TDCSTARTUP;

class CToDoListApp : public CWinApp
{
public:
	CToDoListApp();
	
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CToDoListApp)
	public:
	virtual BOOL InitInstance();
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	virtual void WinHelp(DWORD dwData, UINT nCmd = HELP_CONTEXT);
	virtual int DoMessageBox(LPCTSTR lpszPrompt, UINT nType, UINT nIDPrompt);
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

protected:
// Implementation

	//{{AFX_MSG(CToDoListApp)
	afx_msg void OnHelpContactus();
	afx_msg void OnHelpFeedbackandsupport();
	afx_msg void OnHelpLicense();
	afx_msg void OnHelpWiki();
	afx_msg void OnHelpCommandline();
	afx_msg void OnHelpDonate();
	//}}AFX_MSG
	afx_msg void OnHelp();
	afx_msg void OnImportPrefs();
	afx_msg void OnUpdateImportPrefs(CCmdUI* pCmdUI);
	afx_msg void OnExportPrefs();
	afx_msg void OnUpdateExportPrefs(CCmdUI* pCmdUI);
	DECLARE_MESSAGE_MAP()

protected:
	void DoHelp(const CString& sHelpRef = _T(""));
	BOOL InitPreferences(CEnCommandLineInfo* pInfo);
	void UpgradePreferences(CPreferences& prefs);
	BOOL ParseCommandLine(CEnCommandLineInfo* pInfo);

	// our own local version
	CString AfxGetAppName();
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TODOLIST_H__CA63D273_DB5E_4DBF_8915_1885E1987A65__INCLUDED_)
