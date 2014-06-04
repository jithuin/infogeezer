#if !defined(AFX_AUTOCOMBOBOX_H__34DD95FF_CCAE_4D8E_9162_D860B65CD448__INCLUDED_)
#define AFX_AUTOCOMBOBOX_H__34DD95FF_CCAE_4D8E_9162_D860B65CD448__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// autocombobox.h : header file
//

#include "Subclass.h"
#include "maskedit.h"
#include "misc.h"
#include "ownerdrawcomboboxbase.h"

const UINT WM_ACB_ITEMADDED = ::RegisterWindowMessage(_T("WM_ACB_ITEMADDED"));
const UINT WM_ACB_ITEMDELETED = ::RegisterWindowMessage(_T("WM_ACB_ITEMDELETED"));

enum 
{
	ACBS_ALLOWDELETE	= 0x01,
	ACBS_CASESENSITIVE	= 0x02,
	ACBS_ADDTOSTART		= 0x04,
};

/////////////////////////////////////////////////////////////////////////////
// CAutoComboBox window

// replacement DDX routine
void AFXAPI DDX_AutoCBString(CDataExchange* pDX, int nIDC, CString& value);

class CAutoComboBox : public COwnerdrawComboBoxBase, protected CSubclassWnd
{
	// Construction
public:
	CAutoComboBox(DWORD dwFlags = 0);
	virtual ~CAutoComboBox();
	
	// Operations
public:
    virtual int AddString(LPCTSTR szItem) { return AddUniqueItem(szItem); }
    virtual int InsertString(int nIndex, LPCTSTR szItem) { return InsertUniqueItem(nIndex, szItem); }

    virtual int AddStrings(const CStringArray& aItems) { return AddUniqueItems(aItems); }
    virtual int SetStrings(const CStringArray& aItems);

	virtual int DeleteString(UINT nIndex) { return COwnerdrawComboBoxBase::DeleteString(nIndex);	}
	virtual int DeleteString(LPCTSTR szItem, BOOL bCaseSensitive = FALSE);

	virtual int DeleteItems(const CStringArray& aItems, BOOL bCaseSensitive = FALSE); // returns deleted count
	
	virtual int AddUniqueItem(const CString& sItem); // returns index or CB_ERR
    virtual int InsertUniqueItem(int nIndex, const CString& sItem); // returns index or CB_ERR
	
    virtual int AddUniqueItems(const CStringArray& aItems); // returns num items added
    virtual int AddUniqueItems(const CAutoComboBox& cbItems); // returns num items added
	
    virtual int GetItems(CStringArray& aItems) const; // returns item count
    virtual int SelectString(int nStartAfter, LPCTSTR lpszString) { return COwnerdrawComboBoxBase::SelectString(nStartAfter, lpszString); }
	
	BOOL AddEmptyString();

    // helpers
    int FindStringExact(int nIndexStart, const CString& sItem, BOOL bCaseSensitive) const;
    int FindStringExact(int nIndexStart, LPCTSTR lpszFind) const
		{ return FindStringExact(nIndexStart, lpszFind, FALSE); }

	void Flush() { if (m_bEditChange) HandleReturnKey(); }

	void SetEditMask(LPCTSTR szMask, DWORD dwMaskFlags = 0);
	
protected:
	DWORD m_dwFlags;
	BOOL m_bEditChange;
	BOOL m_bClosingUp;
	CMaskEdit m_eMask;

	class CAutoComboListBox : public CWnd
	{
	public:
		CAutoComboListBox() : m_bTooltipsEnabled(FALSE) {}
		~CAutoComboListBox() {}

		int OnToolHitTest(CPoint point, TOOLINFO* pTI) const;
		BOOL OnToolTipNotify( UINT id, NMHDR* pNMHDR, LRESULT* pResult );

		afx_msg LRESULT OnMouseMove(WPARAM wParam, LPARAM lParam);
		afx_msg LRESULT OnMouseLeave(WPARAM wParam, LPARAM lParam);

		void EnableToolTips(BOOL bEnable);
		BOOL m_bTooltipsEnabled;

		DECLARE_MESSAGE_MAP()
	};

	CAutoComboListBox m_list;

	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAutoComboBox)
protected:
	//}}AFX_VIRTUAL
	virtual LRESULT WindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp);
	
	// Generated message map functions
protected:
	//{{AFX_MSG(CAutoComboBox)
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	afx_msg void OnKillFocus(CWnd* pNewWnd);
	//}}AFX_MSG
	afx_msg BOOL OnSelEndCancel();
	afx_msg BOOL OnSelEndOK();
	afx_msg BOOL OnSelChange();
	afx_msg BOOL OnDropDown();
	afx_msg BOOL OnCloseUp();
	afx_msg BOOL OnEditChange();
//	afx_msg BOOL OnKillFocus();
	
	DECLARE_MESSAGE_MAP()
		
protected:
	virtual BOOL DeleteSelectedLBItem();
	virtual void RefreshMaxDropWidth();
	
	BOOL AllowDelete() const;
	BOOL CaseSensitive() const { return Misc::HasFlag(m_dwFlags, ACBS_CASESENSITIVE); }
	BOOL AddToStart() const { return Misc::HasFlag(m_dwFlags, ACBS_ADDTOSTART); }
	int AddUniqueItem(const CString& sItem, BOOL bAddToStart);
	
	BOOL IsSimpleCombo();
	virtual void HandleReturnKey();
	void NotifyParent(UINT nIDNotify);
	virtual CString GetSelectedItem() const;

};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_AUTOCOMBOBOX_H__34DD95FF_CCAE_4D8E_9162_D860B65CD448__INCLUDED_)
