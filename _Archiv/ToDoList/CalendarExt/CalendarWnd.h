#if !defined(AFX_CALENDARWND_H__47616F96_0B5B_4F86_97A2_93B9DC796EB4__INCLUDED_)
#define AFX_CALENDARWND_H__47616F96_0B5B_4F86_97A2_93B9DC796EB4__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CalendarWnd.h : header file
//

#include "CalendarDefines.h"
#include "CalendarButtonsDlg.h"

#include "..\Shared\IUIExtension.h"
#include "..\3rdParty\Calendar\BigCalendarCtrl.h"
#include "..\3rdParty\Calendar\MiniCalendarCtrl.h"

class CCalendarData;
struct UITHEME;

/////////////////////////////////////////////////////////////////////////////
// CCalendarWnd frame

class CCalendarWnd : public CWnd, public IUIExtensionWindow
{
	DECLARE_DYNAMIC(CCalendarWnd)

public:
	CCalendarWnd();
	virtual ~CCalendarWnd();

	BOOL Create(DWORD dwStyle, const RECT &rect, CWnd* pParentWnd, UINT nID);
	void Release();

	// IUIExtensionWindow
	LPCTSTR GetTypeID() const;
	HICON GetIcon() const { return m_hIcon; }
	LPCTSTR GetMenuText() const;

	void SetReadOnly(bool bReadOnly) {}
	HWND GetHwnd() const { return GetSafeHwnd(); }
	void SetUITheme(const UITHEME* pTheme);

	bool GetLabelEditRect(LPRECT pEdit) const;

	void LoadPreferences(const IPreferences* pPrefs, LPCTSTR szKey, BOOL bAppOnly);
	void SavePreferences(IPreferences* pPrefs, LPCTSTR szKey) const;

	void UpdateTasks(const ITaskList* pTasks, IUI_UPDATETYPE nUpdate, int nEditAttribute);
	bool WantUpdate(int nAttribute) const;

	void SelectTask(DWORD dwTaskID);
	bool SelectTasks(DWORD* pdwTaskIDs, int nTaskCount);

	bool ProcessMessage(MSG* pMsg);
	void DoAppCommand(IUI_APPCOMMAND nCmd);
	bool CanDoAppCommand(IUI_APPCOMMAND nCmd) const;

	void ZoomIn(BOOL bZoomIn);
	DWORD GetDisplayFlags() const;
	int  GetNumWeeksToDisplay() const;
	int	 GetNumDaysToDisplay() const;
	BOOL IsWeekendsHidden() const;
	BOOL IsDateHidden(const COleDateTime& _dt) const;

//protected member functions
protected:
	void ResizeControls(int cx, int cy);
	void UpdateTitleBarText();

//protected member variables
protected:
	CBigCalendarCtrl	m_BigCalendar;
	CMiniCalendarCtrl	m_MiniCalendar;
	CCalendarData*		m_pCalendarData;
	HWND				m_hParentOfFrame;
	HICON				m_hIcon;
	CCalendarButtonsDlg m_dlgBtns;
	CPen				m_penBorder;

	DWORD				m_dwDisplayFlags;
	int					m_nNumVisibleWeeks;
	BOOL				m_bShowMiniCalendar;
	BOOL				m_bShowStatusBar;
	BOOL				m_bShowWeekends;
	CString				m_strTasklistName;
	int					m_nCellFontSize;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCalendarWnd)
	protected:
	virtual BOOL OnNotify(WPARAM wParam, LPARAM lParam, LRESULT* pResult);
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CCalendarWnd)
	afx_msg void OnSetFocus(CWnd* pOldWnd);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnViewMiniCalendar();
	afx_msg void OnUpdateViewMiniCalendar(CCmdUI* pCmdUI);
	afx_msg void OnViewWeekends();
	afx_msg void OnUpdateViewWeekends(CCmdUI* pCmdUI);
	afx_msg void OnViewPrefs();
	afx_msg void OnUpdateViewPrefs(CCmdUI* pCmdUI);
	afx_msg void OnViewNumWeeks1();
	afx_msg void OnUpdateViewNumWeeks1(CCmdUI* pCmdUI);
	afx_msg void OnViewNumWeeks2();
	afx_msg void OnUpdateViewNumWeeks2(CCmdUI* pCmdUI);
	afx_msg void OnViewNumWeeks3();
	afx_msg void OnUpdateViewNumWeeks3(CCmdUI* pCmdUI);
	afx_msg void OnViewNumWeeks4();
	afx_msg void OnUpdateViewNumWeeks4(CCmdUI* pCmdUI);
	afx_msg void OnViewNumWeeks5();
	afx_msg void OnUpdateViewNumWeeks5(CCmdUI* pCmdUI);
	afx_msg void OnViewNumWeeks6();
	afx_msg void OnUpdateViewNumWeeks6(CCmdUI* pCmdUI);
	afx_msg void OnViewNumWeeks7();
	afx_msg void OnUpdateViewNumWeeks7(CCmdUI* pCmdUI);
	afx_msg void OnViewNumWeeks8();
	afx_msg void OnUpdateViewNumWeeks8(CCmdUI* pCmdUI);
	afx_msg void OnViewNumWeeks9();
	afx_msg void OnUpdateViewNumWeeks9(CCmdUI* pCmdUI);
	afx_msg void OnGoToToday();
	afx_msg void OnUpdateGoToToday(CCmdUI* pCmdUI);
	//}}AFX_MSG
//	afx_msg void OnCheckforUpdates();
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CALENDARFRAMEWND_H__47616F96_0B5B_4F86_97A2_93B9DC796EB4__INCLUDED_)
