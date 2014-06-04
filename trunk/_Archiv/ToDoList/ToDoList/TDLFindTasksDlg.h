#if !defined(AFX_TDLFINDTASKSDLG_H__9118493D_32FD_434D_B549_8947D00277CD__INCLUDED_)
#define AFX_TDLFINDTASKSDLG_H__9118493D_32FD_434D_B549_8947D00277CD__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// TDLFindTasksDlg.h : header file
//
#include "TDLfindtaskexpressionlistctrl.h"
#include "tdlfindresultslistctrl.h"
#include "tdlfilteroptioncombobox.h"

#include "..\shared\dockmanager.h"
#include "..\shared\entoolbar.h"
#include "..\shared\toolbarhelper.h"
#include "..\shared\tabbedcombobox.h"
#include "..\shared\wndprompt.h"
#include "..\shared\uitheme.h"

#include "..\3rdparty\lockableheaderctrl.h"

/////////////////////////////////////////////////////////////////////////////
// CTDLFindTasksDlg dialog

const UINT WM_FTD_FIND = ::RegisterWindowMessage(_T("WM_FTD_FIND"));
const UINT WM_FTD_SELECTRESULT = ::RegisterWindowMessage(_T("WM_FTD_SELECTRESULT"));
const UINT WM_FTD_SELECTALL = ::RegisterWindowMessage(_T("WM_FTD_SELECTALL"));
const UINT WM_FTD_CLOSE = ::RegisterWindowMessage(_T("WM_FTD_CLOSE"));
const UINT WM_FTD_APPLYASFILTER = ::RegisterWindowMessage(_T("WM_FTD_APPLYASFILTER"));
const UINT WM_FTD_ADDSEARCH = ::RegisterWindowMessage(_T("WM_FTD_ADDSEARCH"));
const UINT WM_FTD_DELETESEARCH = ::RegisterWindowMessage(_T("WM_FTD_DELETESEARCH"));

class CTDLFindTasksDlg : public CDialog
{
// Construction
public:
	CTDLFindTasksDlg(CWnd* pParent = NULL);   // standard constructor
	~CTDLFindTasksDlg();

	BOOL Initialize(CWnd* pParent, BOOL bDockable = TRUE);
	BOOL Show(BOOL bShow = TRUE);
	void RefreshSearch();

	BOOL GetSearchAllTasklists();
	int GetSearchParams(SEARCHPARAMS& params);
	int GetSearchParams(LPCTSTR szName, SEARCHPARAMS& params);

	CString GetActiveSearch() const { return m_sActiveSearch; }
	int GetSavedSearches(CStringArray& aNames);

	void AddHeaderRow(LPCTSTR szText, BOOL bSpaceAbove = TRUE);
	void AddResult(const SEARCHRESULT& result, LPCTSTR szTask, /*LPCTSTR szPath,*/ const CFilteredToDoCtrl* pTDC);

	int GetResultCount() const; // all tasklists
	int GetResultCount(const CFilteredToDoCtrl* pTDC) const;
	int GetAllResults(CFTDResultsArray& aResults) const;
	int GetResults(const CFilteredToDoCtrl* pTDC, CFTDResultsArray& aResults) const;
	int GetResultIDs(const CFilteredToDoCtrl* pTDC, CDWordArray& aTaskIDs) const;
	void DeleteResults(const CFilteredToDoCtrl* pTDC);

	void RefreshUserPreferences() { m_lcResults.RefreshUserPreferences(); }

	void SetCustomAttributes(const CTDCCustomAttribDefinitionArray& aActiveTasklistAttribDefs,
							const CTDCCustomAttribDefinitionArray& aAllTasklistsAttribDefs);
	
	void SetUITheme(const UITHEME& theme);
	BOOL IsDocked() const { return m_dockMgr.IsDocked(); }


protected:
// Dialog Data
	//{{AFX_DATA(CTDLFindTasksDlg)
	enum { IDD = IDD_FINDTASKS_DIALOG };
	CCheckComboBox	m_cbInclude;
	CComboBox	m_cbSearches;
	//}}AFX_DATA
	CTDLFindTaskExpressionListCtrl	m_lcFindSetup;
	CTDLFindResultsListCtrl	m_lcResults;

	CWndPromptManager m_mgrPrompts;
	CDockManager m_dockMgr;
	CEnToolBar m_toolbar;
	CToolbarHelper m_tbHelper;
	BOOL m_bDockable;
	int m_nCurSel;
	int	m_bAllTasklists;
	CEnString m_sResultsLabel;
	CString m_sActiveSearch;
	UITHEME m_theme;
	CBrush m_brBkgnd;
	BOOL m_bInitializing;
	CTDCCustomAttribDefinitionArray m_aActiveTDCAttribDefs, m_aAllTDCAttribDefs;
	CStringArray m_aSavedSearches;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDLFindTasksDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	virtual void OnCancel();
	virtual void OnOK();
	//}}AFX_VIRTUAL

// Implementation
protected:
	int DoModal() { return -1; } // not for public use

	// Generated message map functions
	//{{AFX_MSG(CTDLFindTasksDlg)
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnApplyasfilter();
	afx_msg void OnSelchangeTasklistoptions();
	//}}AFX_MSG
	virtual BOOL OnInitDialog();
	afx_msg void OnAddRule();
	afx_msg void OnDeleteRule();
	afx_msg void OnUpdateDeleteRule(CCmdUI* pCmdUI);
	afx_msg void OnFind();
	afx_msg void OnClose();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnDestroy();
	afx_msg void OnGetMinMaxInfo(MINMAXINFO FAR* lpMMI);
	afx_msg void OnSelectall();
	afx_msg void OnDockright();
	afx_msg void OnUpdateDockright(CCmdUI* pCmdUI);
	afx_msg void OnDockleft();
	afx_msg void OnUpdateDockleft(CCmdUI* pCmdUI);
	afx_msg void OnUndock();
	afx_msg void OnUpdateUndock(CCmdUI* pCmdUI);
	afx_msg void OnDockbelow();
	afx_msg void OnUpdateDockbelow(CCmdUI* pCmdUI);
	afx_msg void OnSearchresults();
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnItemchangingResults(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnSaveSearch();
	afx_msg void OnItemchangedRulelist(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnSelchangeSearchlist();
	afx_msg void OnNewSearch();
	afx_msg void OnDeleteSearch();
	afx_msg void OnEditchangeSearchlist();
	afx_msg void OnUpdateDeleteSearch(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSaveSearch(CCmdUI* pCmdUI);
	afx_msg void OnMoveRuleUp();
	afx_msg void OnUpdateMoveRuleUp(CCmdUI* pCmdUI);
	afx_msg void OnMoveRuleDown();
	afx_msg void OnUpdateMoveRuleDown(CCmdUI* pCmdUI);
	afx_msg void OnItemActivated(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg BOOL OnToolTipNotify(UINT id, NMHDR* pNMHDR, LRESULT* pResult );	
	DECLARE_MESSAGE_MAP()

protected:
	void SaveSettings();
	void ResizeDlg(int cx = 0, int cy = 0);
	void LoadSettings();
	CSize GetMinDockedSize(DM_POS nPos);
	int GetNextResult(int nItem, BOOL bDown);
	void SelectItem(int nItem);
	int GetSelectedItem();
	CString GetCurrentSearch();
	BOOL InitializeToolbar();
	void DeleteAllResults();
	void EnableApplyAsFilterButton();

	BOOL LoadSearch(LPCTSTR szName, CSearchParamArray& params, BOOL& bDone, BOOL& bParents, BOOL& bFilteredOut) const;
	BOOL LoadSearch(LPCTSTR szName);
	BOOL SaveSearch(LPCTSTR szName);
	int LoadSearches();
	int SaveSearches();

	BOOL LoadRule(const CPreferences& prefs, const CString& sRule, 
					TDC_ATTRIBUTE& attrib, FIND_ATTRIBTYPE& nFindType, FIND_OPERATOR& op, 
					BOOL& bAnd, DWORD& dwFlags, CString& sValue) const;
	BOOL SaveRule(CPreferences& prefs, const CString& sRule, const SEARCHPARAM& rule) const;

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TDLFINDTASKSDLG_H__9118493D_32FD_434D_B549_8947D00277CD__INCLUDED_)
