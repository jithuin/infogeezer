#if !defined(AFX_INPUTLISTCTRL_H__2E5810B0_D7DF_11D1_AB19_0000E8425C3E__INCLUDED_)
#define AFX_INPUTLISTCTRL_H__2E5810B0_D7DF_11D1_AB19_0000E8425C3E__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

// InputListCtrl.h : header file
//

#include "enlistctrl.h"
#include "popupeditctrl.h"
#include <afxtempl.h>

/////////////////////////////////////////////////////////////////////////////
// CInputListCtrl window

enum { NOTVALID, ADDITEM, EDITITEM };

enum IL_COLUMNTYPE 
{ 
	ILCT_TEXT, 
	ILCT_DROPLIST, 
	ILCT_BROWSE, 
	ILCT_DATE 
}; 

class CColumnData2 : public CColumnData
{
public:
	CColumnData2() : CColumnData(), bEditEnabled(TRUE), nType(ILCT_TEXT) { }
	BOOL bEditEnabled;
	IL_COLUMNTYPE nType;
};

class CInputListCtrl : public CEnListCtrl
{
// Construction
public:
	CInputListCtrl();

	void DisableColumnEditing(int nCol, BOOL bDisable);
	BOOL IsColumnEditingDisabled(int nCol) const;
	void AutoAdd(BOOL bRows, BOOL bCols);
	void SetAutoColumnWidth(int nWidth);
	void SetAutoRowPrompt(CString sPrompt);
	void SetAutoColPrompt(CString sPrompt);
	virtual BOOL CanEditSelectedCell() const;
	void EditSelectedCell();
	void EndEdit(BOOL bCancel);
	virtual BOOL CanDeleteSelectedCell() const;
	virtual BOOL DeleteSelectedCell();
	BOOL SetCellText(int nRow, int nCol, CString sText);
	BOOL DeleteAllItems(BOOL bIncludeCols = FALSE);
	void SetCurSel(int nRow, int nCol, BOOL bNotifyParent = FALSE);
	void GetCurSel(int& nRow, int& nCol) const;
	int SetCurSel(int nIndex, bool bNotifyParent = FALSE); // single selection
	int GetCurSel() const;
	int GetLastEdit(int* pRow = NULL, int* pCol = NULL);
	void AllowDuplicates(BOOL bAllow, BOOL bNotify = FALSE);
	int AddRow(CString sRowText, int nImage = -1);
	int AddCol(CString sColText, int nWidth = -1);
	void SetView(int nView);
	void SetColumnType(int nCol, IL_COLUMNTYPE nType);
	IL_COLUMNTYPE GetColumnType(int nCol) const;
	void SetEditMask(LPCTSTR szMask, DWORD dwFlags = 0);
	void SetReadOnly(BOOL bReadOnly);
	void EndEdit();

protected:
	CPopupEditCtrl* GetEditControl();

// Attributes
public:

protected:
	int m_nItemLastSelected;
	int m_nColLastSelected;
	int m_nCurCol;
	int m_nEditItem;
	int m_nEditCol;
	CPopupEditCtrl m_editBox;
	BOOL m_bAutoAddRows;
	BOOL m_bAutoAddCols;
	int m_nAutoColWidth;
	CString m_sAutoRowPrompt;
	CString m_sAutoColPrompt;
	int m_nLastEditRow, m_nLastEditCol, m_nLastEdit;
	BOOL m_bAllowDuplication;
	BOOL m_bNotifyDuplicates;
	CPoint m_ptPopupPos;

private:
	BOOL m_bBaseClassEdit; // for our use ONLY

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CInputListCtrl)
	public:
	virtual void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
	//}}AFX_VIRTUAL
protected:
	virtual void OnEndEdit(UINT uIDCtrl, int* pResult);
	virtual void OnCancelEdit();
	virtual void PreSubclassWindow();

// Implementation
public:
	virtual ~CInputListCtrl();

	// Generated message map functions
protected:
	//{{AFX_MSG(CInputListCtrl)
	afx_msg void OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags);
	afx_msg void OnKillFocus(CWnd* pNewWnd);
	afx_msg void OnSetFocus(CWnd* pOldWnd);
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	//}}AFX_MSG
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
	afx_msg long OnEditEnd(WPARAM wParam, LPARAM lParam);
	afx_msg long OnEditCancel(WPARAM wParam, LPARAM lParam);
	afx_msg long OnShowPopupMenu(WPARAM wParam, LPARAM lParam);
	afx_msg void OnDeleteSelectedCell();
	DECLARE_MESSAGE_MAP()

	virtual void EditCell(int nItem, int nCol);
	virtual BOOL IsEditing() const { return m_editBox.GetSafeHwnd() && m_editBox.IsWindowVisible(); }
	virtual COLORREF GetItemTextColor(int nItem, int nCol, BOOL bSelected, BOOL bDropHighlighted, BOOL bWndFocus);
	virtual COLORREF GetItemBackColor(int nItem, BOOL bSelected, BOOL bDropHighlighted, BOOL bWndFocus);
	virtual CColumnData* GetNewColumnData() const { return new CColumnData2; }
	virtual int CompareItems(DWORD dwItemData1, DWORD dwItemData2, int nSortColumn);
	virtual void GetCellEditRect(int nRow, int nCol, CRect& rCell);
	virtual void PrepareControl(CWnd& /*ctrl*/, int /*nRow*/, int /*nCol*/) {}
	virtual BOOL GetButtonRect(int nRow, int nCol, CRect& rButton) const;
	virtual BOOL DrawButton(CDC* pDC, int nRow, int nCol, CRect& rButton); 
	virtual IL_COLUMNTYPE GetCellType(int nRow, int nCol) const;

	void HideControl(CWnd& ctrl);
	void ShowControl(CWnd& ctrl, int nRow, int nCol);
	void CreateControl(CWnd& ctrl, UINT nID, BOOL bSort = TRUE);
	BOOL IsDuplicateRow(CString sRow, int nRowToIgnore);
	BOOL IsDuplicateCol(CString sCol, int nColToIgnore);
	CRect ScrollCellIntoView(int nRow, int nCol); // returns the final position of the cell 
	BOOL IsPrompt(int nItem, int nCol);
	const CColumnData2* GetColumnData(int nCol) const;
	int InsertRow(CString sRowText, int nItem, int nImage = -1);
	BOOL CanEditCell(int nRow, int nCol) const;
	BOOL CanDeleteCell(int nRow, int nCol) const;
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_INPUTLISTCTRL_H__2E5810B0_D7DF_11D1_AB19_0000E8425C3E__INCLUDED_)