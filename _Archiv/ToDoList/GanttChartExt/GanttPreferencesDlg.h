#if !defined(AFX_GANTTPREFERENCESDLG_H__4BEDF571_7002_4C0D_B355_1334515CA1F9__INCLUDED_)
#define AFX_GANTTPREFERENCESDLG_H__4BEDF571_7002_4C0D_B355_1334515CA1F9__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// GanttPreferencesDlg.h : header file
//

#include "..\shared\colorbutton.h"
#include "..\shared\groupline.h"
#include "..\shared\ipreferences.h"

/////////////////////////////////////////////////////////////////////////////
// CGanttPreferencesDlg dialog

class CGanttPreferencesDlg : public CDialog
{
// Construction
public:
	CGanttPreferencesDlg(CWnd* pParent = NULL);   // standard constructor

	BOOL GetDisplayAllocTo() const { return m_bDisplayAllocTo; }
	BOOL GetAutoScrollSelection() const { return m_bAutoScrollSelection; }
	int GetParentColoring(COLORREF& crParent) const;
	BOOL GetAutoCalcParentDates() const { return m_bAutoCalcParentDates; }
	BOOL GetCalculateMissingStartDates() const { return m_bCalculateMissingStartDates; }
	BOOL GetCalculateMissingDueDates() const { return m_bCalculateMissingDueDates; }
	COLORREF GetTodayColor() const;
	COLORREF GetWeekendColor() const;

	void SavePreferences(IPreferences* pPrefs, LPCTSTR szKey) const;
	void LoadPreferences(const IPreferences* pPrefs, LPCTSTR szKey);

protected:
// Dialog Data
	//{{AFX_DATA(CGanttPreferencesDlg)
	BOOL	m_bDisplayAllocTo;
	BOOL	m_bAutoScrollSelection;
	BOOL	m_bSpecifyWeekendColor;
	BOOL	m_bSpecifyTodayColor;
	BOOL	m_bAutoCalcParentDates;
	BOOL	m_bCalculateMissingStartDates;
	BOOL	m_bCalculateMissingDueDates;
	int		m_nParentColoring;
	//}}AFX_DATA
	CColorButton m_btWeekendColor, m_btTodayColor, m_btParentColor;
	COLORREF m_crWeekend, m_crToday, m_crParent;
	CGroupLineManager m_mgrGroupLines;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CGanttPreferencesDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CGanttPreferencesDlg)
	afx_msg void OnSetWeekendcolor();
	afx_msg void OnSetTodaycolor();
	afx_msg void OnWeekendcolor();
	afx_msg void OnTodaycolor();
	afx_msg void OnChangeParentColoring();
	//}}AFX_MSG
	afx_msg void OnSetParentColor();
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_GANTTPREFERENCESDLG_H__4BEDF571_7002_4C0D_B355_1334515CA1F9__INCLUDED_)
