#if !defined(AFX_CALENDARBUTTONSDLG_H__70C85840_0919_4B05_89D0_80769CED628B__INCLUDED_)
#define AFX_CALENDARBUTTONSDLG_H__70C85840_0919_4B05_89D0_80769CED628B__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CalendarButtonsDlg.h : header file
//

#include "..\shared\menubutton.h"

/////////////////////////////////////////////////////////////////////////////
// CCalendarButtonsDlg dialog

class CCalendarButtonsDlg : public CDialog
{
// Construction
public:
	CCalendarButtonsDlg(CWnd* pParent = NULL);   // standard constructor

	BOOL Create(CWnd* pParent);

protected:
// Dialog Data
	//{{AFX_DATA(CCalendarButtonsDlg)
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA
	CMenuButton m_btnView;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCalendarButtonsDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCalendarButtonsDlg)
	afx_msg void OnGotoToday();
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CALENDARBUTTONSDLG_H__70C85840_0919_4B05_89D0_80769CED628B__INCLUDED_)
