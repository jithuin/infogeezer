#if !defined(AFX_PREFERENCESTOOLPAGE_H__6B9BF0B2_D6DE_4868_B97E_8F6288C77778__INCLUDED_)
#define AFX_PREFERENCESTOOLPAGE_H__6B9BF0B2_D6DE_4868_B97E_8F6288C77778__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PreferencesToolPage.h : header file
//

#include "ToolsHelper.h"

#include "..\shared\fileedit.h"
#include "..\shared\sysimagelist.h"
#include "..\shared\preferencesbase.h"

#include <afxtempl.h>

/////////////////////////////////////////////////////////////////////////////
// CPreferencesToolPage dialog

const UINT WM_PTP_TESTTOOL = ::RegisterWindowMessage(_T("WM_PTP_TESTTOOL"));

class CPreferencesToolPage : public CPreferencesPageBase
{
	DECLARE_DYNCREATE(CPreferencesToolPage)

// Construction
public:
	CPreferencesToolPage();
	~CPreferencesToolPage();

	int GetUserTools(CUserToolArray& aTools) const; // returns the number of tools
	BOOL GetUserTool(int nTool, USERTOOL& tool) const; 

protected:

// Dialog Data
	//{{AFX_DATA(CPreferencesToolPage)
	enum { IDD = IDD_PREFTOOLS_PAGE };
	CEnEdit	m_eCmdLine;
	CFileEdit	m_eToolPath;
	CListCtrl	m_lcTools;
	CString	m_sToolPath;
	CString	m_sCommandLine;
	BOOL	m_bRunMinimized;
	CString		m_sIconPath;
	CFileEdit	m_eIconPath;
	//}}AFX_DATA
	CUserToolArray m_aTools;
	CSysImageList m_ilSys;

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CPreferencesToolPage)
	public:
	virtual void OnOK();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CPreferencesToolPage)
	afx_msg void OnSize(UINT nType, int cx, int cy);
	//}}AFX_MSG
	afx_msg void OnNewtool();
	afx_msg void OnDeletetool();
	afx_msg void OnEndlabeleditToollist(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnItemchangedToollist(NMHDR* pNMHDR, LRESULT* pResult);
	virtual BOOL OnInitDialog();
	afx_msg void OnChangeToolpath();
	afx_msg void OnKeydownToollist(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnChangeCmdline();
	afx_msg void OnInsertPlaceholder(UINT nCmdID);
	afx_msg void OnRunminimized();
	afx_msg void OnTesttool();
	afx_msg void OnChangeIconPath();
	afx_msg void OnImport();
	afx_msg LRESULT OnEEBtnClick(WPARAM wParam, LPARAM lParam);
	DECLARE_MESSAGE_MAP()

	void EnableControls();
	int GetCurSel();
	
	static CString CmdIDToPlaceholder(UINT nCmdID);

   virtual void LoadPreferences(const CPreferences& prefs);
   virtual void SavePreferences(CPreferences& prefs);

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PREFERENCESTOOLPAGE_H__6B9BF0B2_D6DE_4868_B97E_8F6288C77778__INCLUDED_)
