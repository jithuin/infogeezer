/* PERF__FREZ/PUB
 * ====================================================
 * BigCalendarCtrl.h
 * Frederic Rezeau
 * 
 * Perf'Control Personal Edition calendar 
 *
 * If you like it, feel free to use it. 
 *
 * www.performlabs.com 
 * ==================================================== 
 * Original file written by Frederic Rezeau (http://www.codeproject.com/KB/miscctrl/CCalendarCtrl.aspx)
 * Rewritten for the ToDoList (http://www.codeproject.com/KB/applications/todolist2.aspx)
 * Design and latest version - http://www.codeproject.com/KB/applications/TDL_Calendar.aspx
 * by demon.code.monkey@googlemail.com
 */

#ifndef _BIGCALENDARCTRL_H_
#define _BIGCALENDARCTRL_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define CALENDAR_LIGHTGREY		RGB(235,235,235)
#define CALENDAR_LINE_HEIGHT	15
#define CALENDAR_HEADER_HEIGHT	18
#define CALENDAR_ROWS			9
#define CALENDAR_COLUMNS		7

class CBigCalendarTask;
class CCalendarData;
class CCalendarWnd;


/////////////////////////////////////////////////////////////////////////////
// class CBigCalendarCell --   
/////////////////////////////////////////////////////////////////////////////
struct CBigCalendarCell
{
	COleDateTime	date;
	CStringArray	arrTasks;
	CDWordArray		arrTaskIDs;
	CDWordArray		arrHasStartTask;
	CDWordArray		arrHasDueTask;
	CDWordArray		arrHasDoneTask;
	CDWordArray		arrIsCompleteTask;
	int				nListboxID;
};

/////////////////////////////////////////////////////////////////////////////
// class CBigCalendarCtrl --   
/////////////////////////////////////////////////////////////////////////////
class CBigCalendarCtrl : public CWnd
{
public:
	CBigCalendarCtrl();
	virtual ~CBigCalendarCtrl();
	void Initialize(CCalendarWnd* _pWnd, CCalendarData* _pCalendarData, HWND _hwndMessageRoutingWindow);

	BOOL Create(DWORD _dwStyle, const CRect& _rect, CWnd* _pParent, UINT _nID);
	void Repopulate(int nCellFontSize = -1);
	void SelectDate(const COleDateTime& _date);
	void TasklistUpdated();

	const CCalendarData* GetCalendarData() const { return m_pCalendarData; }

	//called from CBigCalendarTask
	void ClickedOnTaskInListbox();
	void SelectTask(int _nTaskListboxID, int _nTaskID);

	void GetSelectedDate(COleDateTime& _date) const;
	DWORD GetSelectedTask() const;
	BOOL GetSelectedTaskEditRect(LPRECT pRect) const;

	//{{AFX_VIRTUAL(CBigCalendarCtrl)
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	//}}AFX_VIRTUAL

protected:	
	BOOL	GetGridCellFromPoint(CPoint _point, int& _nRow, int& _nCol) const;
	BOOL	GetRectFromCell(int _nRow, int _nCol, CRect& _rect) const;
	void	GetLastSelectedGridCell(int& _nRow, int& _nCol) const;

	CBigCalendarTask*		GetTaskListboxFromTaskListboxID(int _nListboxID) const;
	CBigCalendarTask*		GetTaskListboxFromCell(int _nRow, int _nCol) const;
	CBigCalendarTask*		GetTaskListboxFromDate(const COleDateTime& _date) const;
	const CBigCalendarCell*	GetCell(const COleDateTime& _date) const;

	BOOL	IsDateVisible(const COleDateTime& _date, BOOL* _pbAfter=NULL) const;

	static COLORREF GetFadedBlue(unsigned char _percent);

	void EnterCell();
	void LeaveCell();

	void GotoMonth(const COleDateTime& _date);
	void Goto(const COleDateTime& _date, BOOL _bSelect=FALSE);

	void DrawHeader(CDC* _pDC);
	void DrawGrid(CDC* _pDC);
	void DrawCells(CDC* _pDC);

	void FireNotifySelectDate();
	void SendSelectDateMessageToParent(BOOL bAndSendSelectTaskIDMsg);

	void ScrollDown(int _nLines);
	void ScrollUp(int _nLines);

	void CreateTasks(BOOL _bRecreate=FALSE);
	void ResizeTasks(BOOL _bRepopulateTasks=FALSE);
	void RepopulateAllCells(const COleDateTime& _dateFirstCell);

	BOOL WantIncompleteStartDates() const;
	BOOL WantIncompleteDueDates() const;
	BOOL WantCompletedStartDates() const;
	BOOL WantCompletedDueDates() const;
	BOOL WantCompletedDoneDates() const;

	//{{AFX_MSG(CBigCalendarCtrl)
	afx_msg void OnPaint();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	public:
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	CCalendarWnd*		m_pWnd;
	CCalendarData*		m_pCalendarData;
	HWND				m_hwndMessageRoutingWindow;

	BOOL				m_bTracking;
	BOOL				m_bMonthIsOdd;
	int					m_nFirstWeekDay; // 1 = sunday
	int					m_nSelectedTaskID;
	int					m_nVscrollMax;
	int					m_nVscrollPos;
	int					m_nCellFontSize;

	CFont*				m_pFont;
	CFont*				m_pFontBold;
	CBigCalendarCell	m_dayCells[CALENDAR_ROWS][CALENDAR_COLUMNS];
	COleDateTime		m_dateSelected;
	CPtrList			m_listCalendarTasks;
};


#endif//_BIGCALENDARCTRL_H_
