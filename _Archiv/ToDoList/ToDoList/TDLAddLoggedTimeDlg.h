#if !defined(AFX_TDLADDLOGGEDTIMEDLG_H__1E431AC9_0AA0_44E5_9CAE_723D199D910E__INCLUDED_)
#define AFX_TDLADDLOGGEDTIMEDLG_H__1E431AC9_0AA0_44E5_9CAE_723D199D910E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// TDLAddLoggedTimeDlg.h : header file
//

#include "..\shared\TimeEdit.h"
#include "..\shared\Timecombobox.h"

/////////////////////////////////////////////////////////////////////////////
// CTDLAddLoggedTimeDlg dialog

class CTDLAddLoggedTimeDlg : public CDialog
{
// Construction
public:
	CTDLAddLoggedTimeDlg(DWORD dwTaskID, LPCTSTR szTaskTitle, CWnd* pParent = NULL);   // standard constructor
	double GetLoggedTime() const; // in hours
	COleDateTime GetWhen() const;
	BOOL GetAddToTimeSpent() const { return m_bAddTimeToTimeSpent; }

protected:
// Dialog Data
	//{{AFX_DATA(CTDLAddLoggedTimeDlg)
	enum { IDD = IDD_ADDLOGGEDTIME_DIALOG };
	CDateTimeCtrl	m_dateWhen;
	CTimeComboBox	m_cbTimeWhen;
	double	m_dLoggedTime;
	DWORD	m_dwTaskID;
	CString	m_sTaskTitle;
	BOOL	m_bAddTimeToTimeSpent;
	//}}AFX_DATA
	CTimeEdit	m_eLoggedTime;
	TCHAR m_nUnits;
	COleDateTime m_dtWhen;


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDLAddLoggedTimeDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual void OnOK();
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CTDLAddLoggedTimeDlg)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TDLADDLOGGEDTIMEDLG_H__1E431AC9_0AA0_44E5_9CAE_723D199D910E__INCLUDED_)
