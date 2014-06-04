#if !defined(AFX_OWNERDRAWCOMBOBOXBASE_H__9FFD5D25_60B4_49B8_A05B_D61AEBC8D936__INCLUDED_)
#define AFX_OWNERDRAWCOMBOBOXBASE_H__9FFD5D25_60B4_49B8_A05B_D61AEBC8D936__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ownerdrawcomboboxbase.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// COwnerdrawComboBoxBase window

class COwnerdrawComboBoxBase : public CComboBox
{
// Construction
public:
	COwnerdrawComboBoxBase();

// Attributes
protected:
	BOOL m_bItemHeightSet;

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(COwnerdrawComboBoxBase)
	public:
	virtual void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
	virtual void PreSubclassWindow();
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~COwnerdrawComboBoxBase();

	// Generated message map functions
protected:
	//{{AFX_MSG(COwnerdrawComboBoxBase)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);

	DECLARE_MESSAGE_MAP()

protected:
	virtual void DrawItemText(CDC& dc, const CRect& rect, int nItem, UINT nItemState,
								DWORD dwItemData, const CString& sItem, BOOL bList);	
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_OWNERDRAWCOMBOBOXBASE_H__9FFD5D25_60B4_49B8_A05B_D61AEBC8D936__INCLUDED_)
