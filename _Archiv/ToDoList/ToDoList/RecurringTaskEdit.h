// RecurrenceEdit.h: interface for the CRecurringTaskEdit class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_RECURRENCEEDIT_H__4EE655E3_F4B1_44EA_8AAA_39DD459AD8A8__INCLUDED_)
#define AFX_RECURRENCEEDIT_H__4EE655E3_F4B1_44EA_8AAA_39DD459AD8A8__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\shared\enedit.h"
#include "..\shared\propertypagehost.h"
#include "..\shared\weekdaycombobox.h"
#include "..\shared\monthcombobox.h"
#include "..\Shared\WeekdayCheckListBox.h"

#include "ToDoCtrlData.h"

class CTDLRecurringTaskEdit : public CEnEdit  
{
public:
	CTDLRecurringTaskEdit();
	virtual ~CTDLRecurringTaskEdit();

	void GetRecurrenceOptions(TDIRECURRENCE& tr) const;
	void SetRecurrenceOptions(const TDIRECURRENCE& tr);

	void SetDefaultDate(const COleDateTime& date) { m_dtDefault = date; }

	BOOL ModifyStyle(DWORD dwRemove, DWORD dwAdd, UINT nFlags = 0);
	BOOL DoEdit(BOOL bForce = FALSE);

	static CString GetRegularity(const TDIRECURRENCE& tr, BOOL bIncOnce = TRUE);
	static int CalcMaxRegularityWidth(CDC* pDC, BOOL bIncOnce = TRUE);
	static void SetDefaultButtonTip(LPCTSTR szOption);

protected:
	TDIRECURRENCE m_tr;
	BOOL m_bReadOnly;
	COleDateTime m_dtDefault;
	BOOL m_bInOnSetReadOnly;

	static CString s_sOptionsBtnTip;

protected:
	virtual void OnBtnClick(UINT nID);
	virtual void PreSubclassWindow();

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CRecurringTaskEdit)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg HBRUSH CtlColor(CDC* pDC, UINT nCtlColor);
	afx_msg LRESULT OnSetReadOnly(WPARAM wp, LPARAM lp);
	afx_msg void OnStyleChanging(int nStyleType, LPSTYLESTRUCT lpStyleStruct);
	afx_msg BOOL OnReflectChangeDisplayText();
	DECLARE_MESSAGE_MAP()

protected:
	static BOOL IsDefaultString(const CString& sRegularity);
	static CString GetRegularity(TDI_REGULARITY nRegularity, BOOL bIncOnce = TRUE);
};

#endif 

/////////////////////////////////////////////////////////////////////////////
// CTDLRecurringTaskDailyOptionPage dialog

class CTDLRecurringTaskDailyOptionPage : public CPropertyPage
{
// Construction
public:
	CTDLRecurringTaskDailyOptionPage(const TDIRECURRENCE& tr, const COleDateTime& dtDefault);
	~CTDLRecurringTaskDailyOptionPage();

	void GetRecurrenceOptions(TDIRECURRENCE& tr) const;

protected:
// Dialog Data
	//{{AFX_DATA(CTDLRecurringTaskDailyOptionPage)
	int		m_nEveryNumDays;
	int		m_RecreateAfterNumDays;
	int		m_nDailyOption;
	//}}AFX_DATA

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CTDLRecurringTaskDailyOptionPage)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CTDLRecurringTaskDailyOptionPage)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	afx_msg void OnChangeDailyValues();
	DECLARE_MESSAGE_MAP()

};

/////////////////////////////////////////////////////////////////////////////
// CTDLRecurringTaskWeeklyOptionPage dialog

class CTDLRecurringTaskWeeklyOptionPage : public CPropertyPage
{
// Construction
public:
	CTDLRecurringTaskWeeklyOptionPage(const TDIRECURRENCE& tr, const COleDateTime& dtDefault);
	~CTDLRecurringTaskWeeklyOptionPage();

	void GetRecurrenceOptions(TDIRECURRENCE& tr) const;

protected:
// Dialog Data
	//{{AFX_DATA(CTDLRecurringTaskWeeklyOptionPage)
	int		m_nWeeklyOption;
	int		m_nEveryNumWeeks;
	int		m_nRecreateAfterNumWeeks;
	//}}AFX_DATA
 	DWORD m_dwWeekdays;
	CWeekdayCheckListBox m_lbWeekdays;

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CTDLRecurringTaskWeeklyOptionPage)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CTDLRecurringTaskWeeklyOptionPage)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	afx_msg void OnChangeWeeklyValues();
	DECLARE_MESSAGE_MAP()

};

/////////////////////////////////////////////////////////////////////////////
// CTDLRecurringTaskMonthlyOptionPage dialog

class CTDLRecurringTaskMonthlyOptionPage : public CPropertyPage
{
// Construction
public:
	CTDLRecurringTaskMonthlyOptionPage(const TDIRECURRENCE& tr, const COleDateTime& dtDefault);
	~CTDLRecurringTaskMonthlyOptionPage();

	void GetRecurrenceOptions(TDIRECURRENCE& tr) const;

protected:
// Dialog Data
	//{{AFX_DATA(CTDLRecurringTaskMonthlyOptionPage)
	int		m_nEveryDayOfMonth;
	int		m_nEveryNumMonths;
	int		m_nSpecificNumber;
	int		m_nSpecificDayOfWeek;
	int		m_nSpecificNumMonths;
	int		m_nRecreateAfterNumMonths;
	int		m_nMonthlyOption;
	//}}AFX_DATA
	CWeekdayComboBox	m_cbWeekdays;

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CTDLRecurringTaskMonthlyOptionPage)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CTDLRecurringTaskMonthlyOptionPage)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	afx_msg void OnChangeMonthlyValues();
	DECLARE_MESSAGE_MAP()

};

/////////////////////////////////////////////////////////////////////////////
// CTDLRecurringTaskYearlyOptionPage dialog

class CTDLRecurringTaskYearlyOptionPage : public CPropertyPage
{
// Construction
public:
	CTDLRecurringTaskYearlyOptionPage(const TDIRECURRENCE& tr, const COleDateTime& dtDefault);
	~CTDLRecurringTaskYearlyOptionPage();

	void GetRecurrenceOptions(TDIRECURRENCE& tr) const;

protected:
// Dialog Data
	//{{AFX_DATA(CTDLRecurringTaskYearlyOptionPage)
	int		m_nYearlyOption;
	int		m_nEveryDayOfMonth;
	int		m_nSpecificNumber;
	int		m_nRecreateAfterNumYears;
	int		m_nSpecificMonth;
	int		m_nSpecificDayOfWeek;
	int		m_nEveryMonth;
	//}}AFX_DATA
	CMonthComboBox	m_cbSpecificMonthList;
	CWeekdayComboBox	m_cbDaysOfWeek;
	CMonthComboBox	m_cbEveryMonthList;

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CTDLRecurringTaskYearlyOptionPage)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CTDLRecurringTaskYearlyOptionPage)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	afx_msg void OnChangeYearlyValues();
	DECLARE_MESSAGE_MAP()

};

/////////////////////////////////////////////////////////////////////////////
// CRecurringTaskOptionDlg dialog

class CTDLRecurringTaskOptionDlg : public CDialog
{
// Construction
public:
	CTDLRecurringTaskOptionDlg(const TDIRECURRENCE& tr, const COleDateTime& dtDefault, CWnd* pParent = NULL);   // standard constructor

	void GetRecurrenceOptions(TDIRECURRENCE& tr) const;

protected:
// Dialog Data
	//{{AFX_DATA(CRecurringTaskOptionDlg)
	int		m_nRecalcFrom;
	int		m_nReuse;
	//}}AFX_DATA
	CTDLRecurringTaskDailyOptionPage m_pageDaily;
	CTDLRecurringTaskWeeklyOptionPage m_pageWeekly;
	CTDLRecurringTaskMonthlyOptionPage m_pageMonthly;
	CTDLRecurringTaskYearlyOptionPage m_pageYearly;
	CPropertyPageHost m_ppHost;
	TDI_REGULARITY m_nFrequency;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CRecurringTaskOptionDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual void OnOK();
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CRecurringTaskOptionDlg)
	afx_msg void OnSelchangeFrequency();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

protected:
	BOOL HasValidData();
};

