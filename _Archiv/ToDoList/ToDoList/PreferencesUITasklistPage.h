#if !defined(AFX_PREFERENCESUITASKLISTPAGE_H__9612D6FB_2A00_46DA_99A4_1AC6270F060D__INCLUDED_)
#define AFX_PREFERENCESUITASKLISTPAGE_H__9612D6FB_2A00_46DA_99A4_1AC6270F060D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PreferencesUITasklistPage.h : header file
//

#include "TDLColumnListBox.h"

#include "..\shared\dialoghelper.h"
#include "..\shared\groupline.h"
#include "..\shared\preferencesbase.h"

/////////////////////////////////////////////////////////////////////////////
// CPreferencesUITasklistPage dialog

class CPreferencesUITasklistPage : public CPreferencesPageBase, protected CDialogHelper
{
// Construction
public:
	CPreferencesUITasklistPage();
	~CPreferencesUITasklistPage();

	BOOL GetShowInfoTips() const { return m_bShowInfoTips; }
	BOOL GetShowComments() const { return m_bShowComments; }
	int GetDefaultColumns(CTDCColumnIDArray& aColumns) const;
	void SetDefaultColumns(const CTDCColumnIDArray& aColumns);
	BOOL GetShowPathInHeader() const { return m_bShowPathInHeader; }
	BOOL GetStrikethroughDone() const { return m_bStrikethroughDone; }
	BOOL GetFullRowSelection() const { return m_bFullRowSelection; }
	BOOL GetTreeCheckboxes() const { return m_bTreeCheckboxes; }
	BOOL GetTreeTaskIcons() const { return m_bTreeTaskIcons; }
	BOOL GetDisplayDatesInISO() const { return m_bUseISOForDates; }
	BOOL GetShowWeekdayInDates() const { return m_bShowWeekdayInDates; }
	BOOL GetShowParentsAsFolders() const { return m_bShowParentsAsFolders; }
	BOOL GetDisplayFirstCommentLine() const { return m_bShowComments && m_bDisplayFirstCommentLine; }
	int GetMaxInfoTipCommentsLength() const;
	BOOL GetHidePercentForDoneTasks() const { return m_bHidePercentForDoneTasks; }
	BOOL GetHideZeroTimeCost() const { return m_bHideZeroTimeCost; }
	BOOL GetHideStartDueForDoneTasks() const { return m_bHideStartDueForDoneTasks; }
	BOOL GetShowPercentAsProgressbar() const { return m_bShowPercentAsProgressbar; }
	BOOL GetRoundTimeFractions() const { return m_bRoundTimeFractions; }
	BOOL GetShowNonFilesAsText() const { return m_bShowNonFilesAsText; }
	BOOL GetUseHMSTimeFormat() const { return m_bUseHMSTimeFormat; }
	BOOL GetAutoFocusTasklist() const { return m_bAutoFocusTasklist; }
	BOOL GetShowSubtaskCompletion() const { return m_bShowSubtaskCompletion; }
	BOOL GetShowColumnsOnRight() const { return m_bShowColumnsOnRight; }
	BOOL GetHideDueTimeField() const { return m_bHideDueTimeField; }
	BOOL GetHideStartTimeField() const { return m_bHideStartTimeField; }
	BOOL GetHideDoneTimeField() const { return m_bHideDoneTimeField; }
	int GetMaxColumnWidth() const;
	BOOL GetAlwaysHideListParents() const { return m_bHideListParents; }
	int GetPercentDoneIncrement() const { return m_nPercentIncrement; }
//	BOOL Get() const { return m_b; }

protected:
// Dialog Data
	//{{AFX_DATA(CPreferencesUITasklistPage)
	enum { IDD = IDD_PREFUITASKLIST_PAGE };
	CComboBox	m_cbPercentIncrement;
	BOOL	m_bUseISOForDates;
	BOOL	m_bShowWeekdayInDates;
	BOOL	m_bShowParentsAsFolders;
	BOOL	m_bDisplayFirstCommentLine;
	int		m_nMaxInfoTipCommentsLength;
	BOOL	m_bLimitInfoTipCommentsLength;
	BOOL	m_bAutoFocusTasklist;
	BOOL	m_bShowSubtaskCompletion;
	BOOL	m_bShowColumnsOnRight;
	BOOL	m_bHideDueTimeField;
	BOOL	m_bHideStartTimeField;
	BOOL	m_bLimitColumnWidths;
	int		m_nMaxColumnWidth;
	BOOL	m_bHideDoneTimeField;
	BOOL	m_bHideListParents;
	int		m_nPercentIncrement;
	//}}AFX_DATA
	BOOL	m_bShowPathInHeader;
	BOOL	m_bStrikethroughDone;
	BOOL	m_bFullRowSelection;
	BOOL	m_bTreeCheckboxes;
	BOOL	m_bTreeTaskIcons;
	CTDLColumnListBox m_lbColumnVisibility;
	BOOL	m_bShowInfoTips;
	BOOL	m_bShowComments;
	BOOL	m_bShowPercentColumn;
	BOOL	m_bShowPriorityColumn;
	int		m_nSelColumnVisibility;
	BOOL	m_bHideZeroTimeCost;
	BOOL	m_bHideStartDueForDoneTasks;
	BOOL	m_bRoundTimeFractions;
	BOOL	m_bShowNonFilesAsText;
	BOOL	m_bUseHMSTimeFormat;
	BOOL	m_bShowPercentAsProgressbar;
	BOOL	m_bHidePercentForDoneTasks;
	CGroupLineManager m_mgrGroupLines;

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CPreferencesUITasklistPage)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CPreferencesUITasklistPage)
	afx_msg void OnShowcomments();
	afx_msg void OnShowinfotips();
	afx_msg void OnLimitinfotipcomments();
	afx_msg void OnLimitcolwidths();
	afx_msg void OnTreecheckboxes();
	afx_msg void OnShowparentsasfolders();
	afx_msg void OnTreetaskicons();
	//}}AFX_MSG
	virtual BOOL OnInitDialog();
	DECLARE_MESSAGE_MAP()

	void SaveColumns() const;
   virtual void LoadPreferences(const CPreferences& prefs);
   virtual void SavePreferences(CPreferences& prefs);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PREFERENCESUITASKLISTPAGE_H__9612D6FB_2A00_46DA_99A4_1AC6270F060D__INCLUDED_)
