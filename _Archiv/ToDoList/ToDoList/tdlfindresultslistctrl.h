#if !defined(AFX_TDLFINDRESULTSLISTCTRL_H__E3FBC372_D7CC_457E_B7BB_1036256A64E9__INCLUDED_)
#define AFX_TDLFINDRESULTSLISTCTRL_H__E3FBC372_D7CC_457E_B7BB_1036256A64E9__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// tdlfindresultslistctrl.h : header file
//

#include "tdcstruct.h"
#include "..\shared\enlistctrl.h"

/////////////////////////////////////////////////////////////////////////////
// CTDLFindResultsListCtrl window

class CTDLFindResultsListCtrl : public CEnListCtrl
{
// Construction
public:
	CTDLFindResultsListCtrl();

// Attributes
public:
	BOOL AddHeaderRow(LPCTSTR szText, BOOL bSpaceAbove = TRUE);
	int AddResult(const SEARCHRESULT& result, LPCTSTR szTask, /*LPCTSTR szPath,*/ const CFilteredToDoCtrl* pTDC);

	int GetResultCount() const; // all tasklists
	int GetResultCount(const CFilteredToDoCtrl* pTDC) const;
	int GetAllResults(CFTDResultsArray& aResults) const;
	int GetResults(const CFilteredToDoCtrl* pTDC, CFTDResultsArray& aResults) const;
	int GetResultIDs(const CFilteredToDoCtrl* pTDC, CDWordArray& aTaskIDs) const;

	void DeleteResults(const CFilteredToDoCtrl* pTDC);
	void DeleteAllResults();
	
	FTDRESULT* GetResult(int nItem) const { return (FTDRESULT*)GetItemData(nItem); }

	void RefreshUserPreferences();

protected:
	CFont m_fontBold, m_fontStrike;
	COLORREF m_crDone;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDLFindResultsListCtrl)
	//}}AFX_VIRTUAL
	virtual void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
	virtual void PreSubclassWindow();

// Implementation
public:
	virtual ~CTDLFindResultsListCtrl();

	// Generated message map functions
protected:
	//{{AFX_MSG(CTDLFindResultsListCtrl)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()

protected:
  	virtual COLORREF GetItemTextColor(int nItem, int nSubItem, BOOL bSelected, BOOL bDropHighlighted, BOOL bWndFocus) const;
	virtual CFont* GetItemFont(int nItem, int nSubItem);
	int GetNextResult(int nItem, BOOL bDown);

};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TDLFINDRESULTSLISTCTRL_H__E3FBC372_D7CC_457E_B7BB_1036256A64E9__INCLUDED_)
