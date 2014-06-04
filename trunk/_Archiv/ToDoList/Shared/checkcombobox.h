#if !defined(AFX_CHECKCOMBOBOX_H__4DDD3C3D_BDB7_4F07_9C50_75495F6E7F66__INCLUDED_)
#define AFX_CHECKCOMBOBOX_H__4DDD3C3D_BDB7_4F07_9C50_75495F6E7F66__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// checkcombobox.h : header file
//

#include "autocombobox.h"
#include "subclass.h"

/////////////////////////////////////////////////////////////////////////////
// CCheckComboBox window

class CCheckComboBox : public CAutoComboBox, public CSubclasser
{
// Construction
public:
	CCheckComboBox(DWORD dwFlags = 0);

	BOOL GetCheck(int nIndex) const;
	int GetChecked(CStringArray& aItems) const;
	void SetChecked(const CStringArray& aItems);
	int SetCheck(int nIndex, BOOL bCheck = TRUE);
	void CheckAll(BOOL bCheck = TRUE);
	int GetCheckedCount() const;

	BOOL IsAnyChecked() const;
	BOOL IsAnyUnchecked() const;

	CString FormatCheckedItems(LPCTSTR szSep = NULL) const;
	CString GetTooltip() const;

	virtual int AddUniqueItem(const CString& sItem); // returns index or CB_ERR
    virtual int SelectString(int nStartAfter, LPCTSTR lpszString);
	int GetCurSel() const;

protected:
	CString m_sText;
	mutable BOOL m_bDrawing;
	BOOL m_bTextFits;
	BOOL m_bChecking;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCheckComboBox)
	//}}AFX_VIRTUAL

	// for listbox
	virtual LRESULT ScWindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp);

	// for editbox
	virtual LRESULT WindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp);

// Implementation
public:
	virtual ~CCheckComboBox();

	// Generated message map functions
protected:
	//{{AFX_MSG(CCheckComboBox)
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
	//}}AFX_MSG
	afx_msg BOOL OnEditchange();
	afx_msg BOOL OnDropdown();
	afx_msg void OnLBSelChange();
	afx_msg void OnChar(UINT nChar, UINT nRepCnt, UINT nFlags);
	
	DECLARE_MESSAGE_MAP()

	virtual void OnCheckChange(int /*nIndex*/) {}
	virtual void DrawItemText(CDC& dc, const CRect& rect, int nItem, UINT nItemState,
								DWORD dwItemData, const CString& sItem, BOOL bList);	

protected:
	void RecalcText(BOOL bUpdate = TRUE, BOOL bNotify = TRUE);
	void ParseText();
	BOOL IsType(UINT nComboType);
	virtual BOOL DeleteSelectedLBItem();
	virtual void RefreshMaxDropWidth();
	virtual void HandleReturnKey();
	virtual CString GetSelectedItem() const;

	int CalcCheckBoxWidth(HDC hdc = NULL, HWND hwndRef = NULL);

};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CHECKCOMBOBOX_H__4DDD3C3D_BDB7_4F07_9C50_75495F6E7F66__INCLUDED_)
