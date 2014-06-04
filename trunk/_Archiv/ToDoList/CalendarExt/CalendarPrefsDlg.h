#if !defined(AFX_CALENDARPREFSDLG_H__52954DAC_A39D_4004_9B2C_30003858BA59__INCLUDED_)
#define AFX_CALENDARPREFSDLG_H__52954DAC_A39D_4004_9B2C_30003858BA59__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CalendarPrefsDlg.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CCalendarPrefsDlg dialog

#include "..\shared\groupline.h"

class CCalendarPrefsDlg : public CDialog
{
// Construction
public:
	CCalendarPrefsDlg(DWORD dwFlags=0, int nCellFontSize = 8, CWnd* pParent = NULL);   // standard constructor

	DWORD GetFlags() const { return m_dwFlags; }
	int GetCellFontSize() const { return m_nCellFontSize; }

protected:
// Dialog Data
	//{{AFX_DATA(CCalendarPrefsDlg)
	CComboBox	m_cbCellFontSize;
	BOOL	m_bShowIncompleteStartDate;
	BOOL	m_bShowIncompleteDueDate;
	BOOL	m_bShowCompleteStartDate;
	BOOL	m_bShowCompleteDueDate;
	BOOL	m_bShowCompleteDoneDate;
	BOOL	m_bGreyCompletedTasks;
	BOOL	m_bStrikeThruCompletedTasks;
	int		m_nCellFontSize;
	//}}AFX_DATA
	DWORD m_dwFlags;
	CGroupLineManager m_mgrGroupLines;


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCalendarPrefsDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	virtual void OnOK();

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCalendarPrefsDlg)
	afx_msg void OnGreycompletedtasks();
	afx_msg void OnStrikethrucompletedtasks();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

	void UpdateFlags();
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CALENDARPREFSDLG_H__52954DAC_A39D_4004_9B2C_30003858BA59__INCLUDED_)
