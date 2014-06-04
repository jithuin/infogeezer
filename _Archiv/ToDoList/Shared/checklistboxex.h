#if !defined(AFX_CHECKLISTBOXEX_H__3862911F_2AC1_41DC_822D_CA68941B6FDC__INCLUDED_)
#define AFX_CHECKLISTBOXEX_H__3862911F_2AC1_41DC_822D_CA68941B6FDC__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// checklistboxex.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CCheckListBoxEx window

class CCheckListBoxEx : public CCheckListBox
{
// Construction
public:
	CCheckListBoxEx();

// Attributes
protected:
	CImageList m_ilCheck;
	UINT m_nImageHeight;

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCheckListBoxEx)
	public:
	virtual BOOL OnChildNotify(UINT message, WPARAM wParam, LPARAM lParam, LRESULT* pLResult);
	//}}AFX_VIRTUAL
	virtual void PreSubclassWindow();

// Implementation
public:
	virtual ~CCheckListBoxEx();

	// Generated message map functions
protected:
	//{{AFX_MSG(CCheckListBoxEx)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG
	afx_msg void OnDestroy();
	afx_msg LRESULT OnSetFont(WPARAM , LPARAM);
	DECLARE_MESSAGE_MAP()

	void PreDrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
	void PreMeasureItem(LPMEASUREITEMSTRUCT lpMeasureItemStruct);
	virtual void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);

};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CHECKLISTBOXEX_H__3862911F_2AC1_41DC_822D_CA68941B6FDC__INCLUDED_)
