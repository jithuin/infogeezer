#if !defined(AFX_PREFERENCESBASE_H__19D0237F_28DC_4B45_9762_DF9B4F7D8492__INCLUDED_)
#define AFX_PREFERENCESBASE_H__19D0237F_28DC_4B45_9762_DF9B4F7D8492__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "preferences.h"
#include "scrollingpropertypagehost.h"

/////////////////////////////////////////////////////////////////////////////
// CPreferencesPageBase dialog

const UINT WM_PPB_CTRLCHANGE = ::RegisterWindowMessage(_T("WM_PPB_CTRLCHANGE"));

class CPreferencesPageBase : public CPropertyPage
{
	DECLARE_DYNCREATE(CPreferencesPageBase)
		
		// Construction
public:
	CPreferencesPageBase(UINT nID = 0);
	~CPreferencesPageBase();
	
	virtual void LoadPreferences(const CPreferences& /*prefs*/) {}
	virtual void SavePreferences(CPreferences& /*prefs*/) {}

	void SetBackgroundColor(COLORREF color);

protected:
	HBRUSH m_brush;
	COLORREF m_crback;

protected:
	CWnd* GetDlgItem(UINT nID) const;

	void OnControlChange(UINT nID = -1);
	BOOL OnEraseBkgnd(CDC* pDC);
	HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	DECLARE_MESSAGE_MAP()

};

/////////////////////////////////////////////////////////////////////////////
// CPreferencesDlgBase dialog

const UINT WM_PDB_ONAPPLY = ::RegisterWindowMessage(_T("WM_PDB_ONAPPLY"));

class CPreferencesDlgBase : public CDialog
{
	// Construction
public:
	CPreferencesDlgBase(UINT nID = 0, CWnd* pParent = NULL);   // standard constructor
	
	int DoModal(int nInitPage = -1);
	void Initialize(CPreferences& prefs);
	
protected:
	int m_nInitPage;
	CPreferences m_prefs;
	CScrollingPropertyPageHost m_pphost;
	
protected:
	afx_msg void OnDestroy();
	DECLARE_MESSAGE_MAP()
		
protected:
	virtual void OnOK();
	virtual BOOL OnInitDialog();
	
	void OnApply(); // called from derived class
	
	BOOL EnsurePageCreated(int nPage) { return m_pphost.EnsurePageCreated(nPage); }
	BOOL EnsurePageCreated(const CPreferencesPageBase* pPage) { return m_pphost.EnsurePageCreated(pPage); }

	virtual BOOL SetActivePage(int nPage);
	virtual BOOL SetActivePage(CPreferencesPageBase* pPage);
	virtual BOOL AddPage(CPreferencesPageBase* pPage);
	
	void LoadPreferences();
	void SavePreferences();
	
	BOOL CreatePPHost(UINT nHostID);
	BOOL CreatePPHost(LPRECT pRect);

	int GetActiveIndex() const { return m_pphost.GetActiveIndex(); }
	CPreferencesPageBase* GetActivePage() { return (CPreferencesPageBase*)m_pphost.GetActivePage(); }
	int FindPage(CPreferencesPageBase* pPage) { return m_pphost.FindPage(pPage); }
	
	void ForwardMessage(UINT nMsg) { m_pphost.ForwardMessage(nMsg); }
	
private:
	void LoadPreferences(CPreferences& prefs);
	void SavePreferences(CPreferences& prefs);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PREFERENCESBASE_H__19D0237F_28DC_4B45_9762_DF9B4F7D8492__INCLUDED_)
