// GanttTreeList.h: interface for the CGanttTreeList class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_GANTTTREELISTCTRL_H__016B94F3_1D28_4532_97EF_95F1D9D5CE55__INCLUDED_)
#define AFX_GANTTTREELISTCTRL_H__016B94F3_1D28_4532_97EF_95F1D9D5CE55__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


#include "..\shared\itasklist.h"
#include "..\shared\TreeListSyncer.h"
#include "..\shared\enheaderctrl.h"
#include "..\shared\treectrlhelper.h"
#include "..\shared\iuiextension.h"

#include <afxtempl.h>

#define TVN_KEYUP (TVN_FIRST-16)
#define WM_GANTTCTRL_NOTIFY_ZOOM (WM_APP+1)

enum GTLC_MONTH_DISPLAY
{
	GTLC_DISPLAY_FIRST,
	GTLC_DISPLAY_YEARS = GTLC_DISPLAY_FIRST,
	GTLC_DISPLAY_QUARTERS_SHORT,
	GTLC_DISPLAY_QUARTERS_MID,
	GTLC_DISPLAY_QUARTERS_LONG,
	GTLC_DISPLAY_MONTHS_SHORT,
	GTLC_DISPLAY_MONTHS_MID,
	GTLC_DISPLAY_MONTHS_LONG,
	GTLC_DISPLAY_WEEKS_SHORT,
	GTLC_DISPLAY_WEEKS_MID,
	GTLC_DISPLAY_WEEKS_LONG,
	GTLC_DISPLAY_DAYS_SHORT,
	GTLC_DISPLAY_DAYS_MID,
	GTLC_DISPLAY_DAYS_LONG,
		
	GTLC_DISPLAY_COUNT,
	GTLC_DISPLAY_LAST = GTLC_DISPLAY_COUNT - 1
};

enum // options
{
	GTLCF_DISPLAYALLOCTOAFTERITEM	= 0x01,
	GTLCF_AUTOSCROLLTOTASK			= 0x02,
	GTLCF_ENABLESPLITTER			= 0x04,
	GTLCF_CALCPARENTDATES			= 0x08,
	GTLCF_CALCMISSINGSTARTDATES		= 0x10,
	GTLCF_CALCMISSINGDUEDATES		= 0x20,
	GTLCF_USECOLORASBACKGROUND		= 0x40,
};

enum GTLC_PARENTCOLORING 
{
	GTLPC_DEFAULTCOLORING,
	GTLPC_NOCOLOR,
	GTLPC_SPECIFIEDCOLOR,
};

class CThemed;

class CGanttTreeListCtrl : protected CTreeListSyncer  
{
public:
	CGanttTreeListCtrl();
	virtual ~CGanttTreeListCtrl();

	BOOL Initialize(HWND hwndTree, HWND hwndList, UINT nIDHeader, int nMinItemHeight = 17);
	void Release();

	void UpdateTasks(const ITaskList* pTasks, IUI_UPDATETYPE nUpdate, int nEditAttribute = -1);

	DWORD GetSelectedTaskID() const;
	void SelectTask(DWORD dwTaskID);
	HTREEITEM GetSelectedItem() const;
	
	void ExpandAll(BOOL bExpand = TRUE);
	void ExpandItem(HTREEITEM hti, BOOL bExpand = TRUE, BOOL bAndChildren = FALSE);
	BOOL CanExpandItem(HTREEITEM hti, BOOL bExpand = TRUE) const;

	void Resize(const CRect& rect);
	BOOL ZoomIn(BOOL bIn = TRUE);
	BOOL ZoomBy(int nAmount);

	void SetFocus();
	BOOL HasFocus() const { return CTreeListSyncer::HasFocus(); }

	void Resort(BOOL bAllowToggle);
	BOOL IsSorted() const;

	GTLC_MONTH_DISPLAY GetMonthDisplay() const { return m_nMonthDisplay; }
	BOOL SetMonthDisplay(GTLC_MONTH_DISPLAY nNewDisplay);

	void ScrollToToday();
	void ScrollToSelectedTask();

	void EnableOption(DWORD dwOption, BOOL bEnable = TRUE);
	BOOL IsOptionEnabled(DWORD dwOption) const { return (m_dwOptions & dwOption); }

	BOOL HandleEraseBkgnd(CDC* pDC);
	void SetAlternateLineColor(COLORREF crAltLine);
	void SetGridLineColor(COLORREF crGridLine);
	void SetTodayColor(COLORREF crToday);
	void SetWeekendColor(COLORREF crWeekend);
	void SetParentColoring(GTLC_PARENTCOLORING nOption, COLORREF color);
	void SetDoneTaskAttributes(COLORREF color, BOOL bStrikeThru);

protected:
	CEnHeaderCtrl m_treeHeader, m_listHeader;
	CImageList m_ilSize;
	HFONT m_hFontDone;
	BOOL m_bSortAscending;
	int m_nSortBy;
	int m_nMonthWidth;
	CArray<int, int> m_aMinMonthWidths;
	GTLC_MONTH_DISPLAY m_nMonthDisplay;
	DWORD m_dwOptions;
	COLORREF m_crAltLine, m_crGridLine, m_crToday, m_crWeekend, m_crParent, m_crDone;
	GTLC_PARENTCOLORING m_nParentColoring;
	CRect m_rect;
	int m_nSplitWidth;
	int m_nItemHeight;

	// keep our own handles to these to speed lookups
	HWND m_hwndList, m_hwndTree;

	struct GANTTITEM 
	{ 
		GANTTITEM();
		virtual ~GANTTITEM() {}

		CString sTitle;
		COleDateTime dtStart, dtStartCalc;
		COleDateTime dtDue, dtDueCalc; 
		COleDateTime dtDone; 
		COLORREF color;
		CString sAllocTo;
		BOOL bParent;
		DWORD dwRefID;

		void MinMaxDates(const GANTTITEM& giOther);
		BOOL IsDone() const;
		BOOL HasStart() const;
		BOOL HasDue() const;
	};
	CMap<DWORD, DWORD, GANTTITEM, GANTTITEM&> m_data;

	struct GANTTDISPLAY
	{ 
		GANTTDISPLAY() : nEndPos(-1) {}

		CString sDisplayText;
		int nEndPos;
	};
	CMap<DWORD, DWORD, GANTTDISPLAY, GANTTDISPLAY&> m_display;

private:
	mutable CTreeCtrlHelper* m_pTCH;

protected:
	LRESULT WindowProc(HWND hwnd, UINT msg, WPARAM wp, LPARAM lp);
	LRESULT ScWindowProc(HWND hwnd, UINT msg, WPARAM wp, LPARAM lp);

protected:
	// pseudo message handlers
	LRESULT OnCustomDrawTree(HWND hwndTree, NMTVCUSTOMDRAW* pTVCD);
	LRESULT OnCustomDrawList(HWND hwndList, NMLVCUSTOMDRAW* pLVCD);
	LRESULT OnCustomDrawListHeader(NMCUSTOMDRAW* pNMCD);

	void DrawListItem(CDC* pDC, int nItem, int nCol, DWORD dwTaskID);
	void PostDrawListItem(CDC* pDC, int nItem, DWORD dwTaskID);
	void DrawTreeItem(CDC* pDC, HTREEITEM hti, int nCol, const GANTTITEM& gi);
	void DrawListHeaderItem(CDC* pDC, int nCol);
	void DrawListHeaderRect(CDC* pDC, const CRect& rItem, const CString& sItem, CThemed* pTheme);

	int DrawGanttBar(CDC* pDC, const CRect& rMonth, int nMonth, int nYear, const GANTTITEM& gi, BOOL bSelected = FALSE);
	void DrawItemDivider(CDC* pDC, const CRect& rItem, BOOL bColumn, BOOL bVert, LPRECT prFocus = NULL);
	BOOL DrawToday(CDC* pDC, const CRect& rMonth, int nMonth, int nYear, BOOL bSelected = FALSE);
	BOOL DrawGanttDone(CDC* pDC, const CRect& rMonth, int nMonth, int nYear, const GANTTITEM& gi, BOOL bSelected = FALSE);
	void DrawGanttParentEnds(CDC* pDC, const GANTTITEM& gi, const CRect& rBar, 
							 const COleDateTime& dtMonthStart, const COleDateTime& dtMonthEnd, HBRUSH hbrParent);

	int GetListItem(HTREEITEM hti);
	void ExpandList(HTREEITEM hti, int& nNextIndex);
	void CollapseList(HTREEITEM hti);
	void ExpandList();
	void CollapseList();
	void GetTreeItemRect(HTREEITEM hti, int nCol, CRect& rItem, BOOL bText = FALSE) const;
	void RecalcListColumnWidths(int nFromWidth, int nToWidth);
	void UpdateColumnsWidthAndText(int nFromWidth, int nToWidth);
	BOOL IsTreeItemLineOdd(HTREEITEM hti);
	void AddListColumn(int nMonth = 0, int nYear = 0);
	void SetListColumnDate(int nCol, int nMonth, int nYear);
	BOOL GetListColumnDate(int nCol, int& nMonth, int& nYear) const;
	void CalculateMinMonthWidths();
	void BuildListColumns();
	void BuildTreeColumns();
	GTLC_MONTH_DISPLAY GetColumnDisplay(int nColWidth);
	int GetColumnWidth() const;
	int GetColumnWidth(GTLC_MONTH_DISPLAY nDisplay) const;
	int GetMonthWidth(int nColWidth) const;
	int GetRequiredColumnCount() const;
	BOOL ZoomTo(GTLC_MONTH_DISPLAY nNewDisplay, int nNewMonthWidth);
	void DeleteTreeItem(HTREEITEM hti);
	void RemoveDeletedTasks(HTREEITEM hti, const ITaskList8* pTasks);
	int FindColumn(int nScrollPos) const;
	int FindColumn(int nMonth, int nYear) const;
	int FindColumn(const COleDateTime& date) const;
	int GetDateInMonths(int nMonth, int nYear) const;
	COleDateTime GetDateFromScrollPos(int nScrollPos) const;
	int GetScrollPosFromDate(const COleDateTime& date) const;
	BOOL GetListColumnRect(int nCol, CRect& rect, BOOL bScrolled = TRUE) const;
	BOOL GetGanttItem(DWORD dwTaskID, GANTTITEM& gi) const;
	BOOL GetGanttDisplay(DWORD dwTaskID, GANTTDISPLAY& gd) const;
	void UpdateTask(const ITaskList11* pTasks, HTASKITEM hTask, IUI_UPDATETYPE nUpdate, int nEditAttribute = -1);
	void ScrollTo(const COleDateTime& date);
	void InitItemHeight();
	int CalcTreeWidth() const;

	BOOL HasAltLineColor() const { return (m_crAltLine != (COLORREF)-1); }
	COLORREF GetWeekendColor(BOOL bSelected) const;
	HBRUSH GetGanttBarColors(const GANTTITEM& gi, COLORREF& crBorder, COLORREF& crFill, BOOL bSelected) const;
 	COLORREF GetTreeTextColor(const GANTTITEM& gi, BOOL bSelected, BOOL bFullRow, BOOL bTitle) const;
 	COLORREF GetTreeTextBkColor(const GANTTITEM& gi, BOOL bSelected, BOOL bAlternate, BOOL bFullRow, BOOL bTitle) const;
	void SetColor(COLORREF& color, COLORREF crNew);

	void RebuildTree(const ITaskList11* pTasks);
	void BuildTreeItem(const ITaskList11* pTasks, HTASKITEM hTask, CTreeCtrl& tree, HTREEITEM htiParent);
	void RecalcParentDates();
	void RecalcParentDates(const CTreeCtrl& tree, HTREEITEM htiParent, GANTTITEM& gi);
	BOOL GetStartDueDates(const GANTTITEM& gi, COleDateTime& dtStart, COleDateTime& dtDue) const;
	BOOL HasDisplayDates(const GANTTITEM& gi) const;
	BOOL HasDoneDate(const GANTTITEM& gi) const;

	CTreeCtrlHelper* TCH();
	const CTreeCtrlHelper* TCH() const;

	inline HWND GetTree() const 
	{ 
		ASSERT(m_hwndTree);
		return m_hwndTree; 
	}

	inline HWND GetList() const
	{
		ASSERT(m_hwndList);
		return m_hwndList; 
	}

	static CString FormatColumnHeaderText(GTLC_MONTH_DISPLAY nDisplay, int nMonth = 0, int nYear = 0);
	static int CALLBACK SortProc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);
	static int GetColumnWidth(GTLC_MONTH_DISPLAY nDisplay, int nMonthWidth);
	static COleDateTime GetDate(time_t tDate, BOOL bEndOfDay);
	static COLORREF GetColor(COLORREF crBase, double dLighter, BOOL bSelected);
	static BOOL CalcDateRect(const CRect& rMonth, int nMonth, int nYear, 
							const COleDateTime& dtFrom, const COleDateTime& dtTo, CRect& rDate);
	static BOOL CalcDateRect(const CRect& rMonth, int nDaysInMonth, 
							const COleDateTime& dtMonthStart, const COleDateTime& dtMonthEnd, 
							const COleDateTime& dtFrom, const COleDateTime& dtTo, CRect& rDate);
	static BOOL GetMonthDates(int nMonth, int nYear, COleDateTime& dtStart, COleDateTime& dtEnd);
	static CString GetTaskAllocTo(const ITaskList11* pTasks, HTASKITEM hTask);
};

#endif // !defined(AFX_GANTTTREELIST_H__016B94F3_1D28_4532_97EF_95F1D9D5CE55__INCLUDED_)
