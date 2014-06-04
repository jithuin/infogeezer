#if !defined(AFX_ENTOOLBAR_H__9AA29CEC_1405_4BBC_BBD0_94C1BD6D3120__INCLUDED_)
#define AFX_ENTOOLBAR_H__9AA29CEC_1405_4BBC_BBD0_94C1BD6D3120__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// entoolbar.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CEnToolBar window

const COLORREF NO_COLOR = (COLORREF)-1;

class CEnBitmapEx;

class CEnToolBar : public CToolBar
{
	// Construction
public:
	CEnToolBar();
	virtual ~CEnToolBar();
	
	BOOL LoadToolBar(LPCTSTR lpszResourceName, LPCTSTR szImagePath = NULL);
	BOOL LoadToolBar(UINT nIDResource, LPCTSTR szImagePath = NULL);
	BOOL LoadToolBar(UINT nIDResource, UINT nIDImage);
	BOOL SetImage(const CString& sImagePath, COLORREF crMask = NO_COLOR);
	BOOL SetImage(UINT nIDImage, COLORREF crMask = NO_COLOR);
	int GetButtonCount(BOOL bIgnoreSeparators = FALSE) const;
	
	void RefreshButtonStates(BOOL bImmediate = TRUE);
	void SetBackgroundColors(COLORREF crFrom, COLORREF crTo, BOOL bGradient, BOOL bGlass);

	int GetHeight() const;
	int Resize(int cx, CPoint ptTopLeft);

	// Attributes
protected:
	CImageList m_ilDisabled, m_ilNormal;
	COLORREF m_crFrom, m_crTo;
	BOOL m_bGradient, m_bGlass;
	CUIntArray m_aRowHeights;
	
	// Operations
public:
	
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CEnToolBar)
	//}}AFX_VIRTUAL
	
	// Generated message map functions
protected:
	//{{AFX_MSG(CEnToolBar)
	// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG
	afx_msg void OnCustomDraw(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnSettingChange(UINT uFlags, LPCTSTR lpszSection);
	afx_msg LRESULT OnRefreshButtonStates(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnSizeParent(WPARAM /*wParam*/, LPARAM /*lParam*/) { return 0L; }
	afx_msg void OnDestroy();
	DECLARE_MESSAGE_MAP()
		
	virtual LRESULT OnItemPrePaint(LPNMTBCUSTOMDRAW /*lpNMCustomDraw*/) { return CDRF_DODEFAULT; }
	virtual LRESULT OnItemPostPaint(LPNMTBCUSTOMDRAW /*lpNMCustomDraw*/) { return CDRF_DODEFAULT; }
	
	// pseudo message handler
	BOOL OnEraseBkgnd(CDC* pDC);
	
	BOOL SetImage(CEnBitmapEx* pBitmap, COLORREF crMask);
	void RefreshDisabledImageList(CEnBitmapEx* pBitmap, COLORREF crMask);

	BOOL HasBkgndColor() const { return m_crFrom != NO_COLOR; }
	
	int EstimateHeightRequired(int cx) const;
	int EstimateRowsRequired(int cx) const;
	int RefreshRowHeights();
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_ENTOOLBAR_H__9AA29CEC_1405_4BBC_BBD0_94C1BD6D3120__INCLUDED_)
