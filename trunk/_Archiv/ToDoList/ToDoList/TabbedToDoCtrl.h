// FilteredToDoCtrl.h: interface for the CTabbedToDoCtrl class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TABBEDTODOCTRL_H__356A6EB9_C7EC_4395_8716_623AFF4A269B__INCLUDED_)
#define AFX_TABBEDTODOCTRL_H__356A6EB9_C7EC_4395_8716_623AFF4A269B__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "ToDoCtrl.h"
#include "tdclistview.h"
#include "tdcviewTabcontrol.h"

#include "..\shared\misc.h"
#include "..\shared\subclass.h"
#include "..\shared\IUIExtension.h"

#include <afxtempl.h>

struct VIEWDATA
{
    VIEWDATA()
		: 
		bNeedResort(FALSE), 
		bModSinceLastSort(FALSE), 
		bMultiSort(FALSE), 
		pExtension(NULL),
		bNeedTaskUpdate(TRUE)
	{
	}

	virtual ~VIEWDATA() {}

	TDSORTCOLUMNS sort;
	CTaskListDropTarget dropTgt;
	IUIExtension* pExtension;

	BOOL bNeedResort;
	BOOL bModSinceLastSort;
	BOOL bMultiSort;
	BOOL bNeedTaskUpdate;
};

class CTabbedToDoCtrl : public CToDoCtrl, public CSubclasser  
{
public:
	CTabbedToDoCtrl(CContentMgr& mgr, const CONTENTFORMAT& cfDefault);
	virtual ~CTabbedToDoCtrl();

	int GetTasks(CTaskFile& tasks, const TDCGETTASKS& filter = TDCGETTASKS(TDCGT_ALL)) const;
	int GetSelectedTasks(CTaskFile& tasks, const TDCGETTASKS& filter = TDCGETTASKS(TDCGT_ALL), DWORD dwFlags = 0) const;

	HTREEITEM CreateNewTask(const CString& sText, TDC_INSERTWHERE nWhere = TDC_INSERTATTOPOFSELTASKPARENT, BOOL bEditText = TRUE);
	BOOL CanCreateNewTask(TDC_INSERTWHERE nInsertWhere) const;

	BOOL SelectTask(DWORD dwTaskID);
	BOOL EditSelectedTask(BOOL bTaskIsNew = FALSE); 

	int FindTasks(const SEARCHPARAMS& params, CResultArray& aResults) const;
	BOOL SelectTask(CString sPart, TDC_SELECTTASK nSelect); 
	
	void SetView(FTC_VIEW nView);
	void SetNextView();
	FTC_VIEW GetView() const { return m_tabViews.GetActiveView(); }

	BOOL SetTreeFont(HFONT hFont); // caller responsible for deleting

	BOOL PtInHeader(CPoint ptScreen) const;
	void SetVisibleColumns(const CTDCColumnIDArray& aColumns);
	BOOL WantTaskContextMenu() const;

	virtual void Sort(TDC_COLUMN nBy, BOOL bAllowToggle = TRUE);
	virtual void MultiSort(const TDSORTCOLUMNS& sort);
	virtual TDC_COLUMN GetSortBy() const;
	virtual void GetSortBy(TDSORTCOLUMNS& sort) const;
	virtual BOOL IsMultiSorting() const;

	BOOL DeleteSelectedTask() { return CToDoCtrl::DeleteSelectedTask(); }
	void SetModified(BOOL bMod = TRUE) { CToDoCtrl::SetModified(bMod); }
	BOOL SetStyle(TDC_STYLE nStyle, BOOL bOn = TRUE) { return CToDoCtrl::SetStyle(nStyle, bOn); }

	BOOL MoveSelectedTask(TDC_MOVETASK nDirection);
	BOOL CanMoveSelectedTask(TDC_MOVETASK nDirection) const;

	BOOL GotoNextTask(TDC_GOTO nDirection); 
	BOOL CanGotoNextTask(TDC_GOTO nDirection) const;
	BOOL GotoNextTopLevelTask(TDC_GOTO nDirection); 
	BOOL CanGotoNextTopLevelTask(TDC_GOTO nDirection) const;

	virtual BOOL CanExpandTasks(TDC_EXPANDCOLLAPSE nWhat, BOOL bExpand) const;
	virtual void ExpandTasks(TDC_EXPANDCOLLAPSE nWhat, BOOL bExpand = TRUE);

	void SetFocusToTasks();
	BOOL TasksHaveFocus() const;

	void SelectAll();
	void DeselectAll(); // call externally only
	void SelectNextTasksInHistory();
	void SelectPrevTasksInHistory();
	BOOL MultiSelectItems(const CDWordArray& aTasks, TSH_SELECT nState = TSHS_SELECT, BOOL bRedraw = TRUE);
	virtual BOOL GetSelectionBoundingRect(CRect& rSelection) const;

	void SetUITheme(const UITHEME& theme);
	void RedrawReminders() const;
	virtual CString GetControlDescription(const CWnd* pCtrl) const;
	virtual void RebuildCustomAttributeUI();

	BOOL AddView(IUIExtension* pExtension);
	BOOL RemoveView(IUIExtension* pExtension);

	virtual void NotifyBeginPreferencesUpdate();
	virtual void NotifyEndPreferencesUpdate();

	// override these so we can notify extensions of color changes
	BOOL SetPriorityColors(const CDWordArray& aColors);
	void SetCompletedTaskColor(COLORREF color);
	void SetFlaggedTaskColor(COLORREF color);
	void SetReferenceTaskColor(COLORREF color);
	void SetAttributeColors(TDC_ATTRIBUTE nAttrib, const CTDCColorMap& colors);
	void SetStartedTaskColors(COLORREF crStarted, COLORREF crStartedToday);
	void SetDueTaskColors(COLORREF crDue, COLORREF crDueToday);

protected:
	CTDCListView m_list;
	CTDCViewTabControl m_tabViews;
	CTaskListDropTarget m_dtList;
	CArray<IUIExtensionWindow*, IUIExtensionWindow*> m_aExtViews;

	DWORD m_dw2ndClickItem;
	BOOL m_bTaskColorChange, m_bUpdatingExtensions;

	// almost all of the tree's view data is stored in CToDoCtrl
	// the only extra piece is whether or not the tree needs to
	// be resorted when we switch to it from another view
	mutable BOOL m_bTreeNeedResort;

// Overrides
protected:
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTabbedToDoCtrl)
	//}}AFX_VIRTUAL
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual BOOL OnInitDialog();

public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);

protected:
	// Generated message map functions
	//{{AFX_MSG(CTabbedToDoCtrl)
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnDestroy();
	//}}AFX_MSG
	afx_msg void OnListHeaderCustomDraw(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnRClickListHeader(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnClickListHeader(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnListSelChanged(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnListClick(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnListDblClick(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnListKeyDown(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnListGetDispInfo(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnMeasureItem(int nIDCtl, LPMEASUREITEMSTRUCT lpMeasureItemStruct);
	afx_msg void OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpDrawItemStruct);
	afx_msg LRESULT OnDropObject(WPARAM wParam, LPARAM lParam);
	afx_msg void OnListGetInfoTip(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg LRESULT OnEditCancel(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterWidthChange(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterRecalcColWidth(WPARAM wParam, LPARAM lParam);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg LRESULT OnPreTabViewChange(WPARAM nOldView, LPARAM nNewView);
	afx_msg LRESULT OnPostTabViewChange(WPARAM nOldView, LPARAM nNewView);
	afx_msg LRESULT OnUIExtSelectTask(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnUIExtModifySelectedTask(WPARAM wParam, LPARAM lParam);

	DECLARE_MESSAGE_MAP()

protected:
	BOOL ModNeedsResort(TDC_ATTRIBUTE nModType) const;
	BOOL ModCausesColorChange(TDC_ATTRIBUTE nModType) const;
	void ResortSelectedTaskParents();

	virtual BOOL DeleteSelectedTask(BOOL bWarnUser, BOOL bResetSel = FALSE);
	virtual BOOL SelectedTasksHaveChildren() const;
	virtual void SetModified(BOOL bMod, TDC_ATTRIBUTE nAttrib, DWORD dwModTaskID);
	virtual void ReposTaskTree(CDeferWndMove* pDWM, const CRect& rPos);
	virtual COLORREF GetItemLineColor(HTREEITEM hti);
	virtual BOOL SetStyle(TDC_STYLE nStyle, BOOL bOn, BOOL bWantUpdate); // one style at a time only 
	virtual void UpdateTasklistVisibility();
	virtual BOOL AddTasksToTree(const CTaskFile& tasks, HTREEITEM htiDest, HTREEITEM htiDestAfter, TDC_RESETIDS nResetID);

	virtual BOOL LoadTasks(const CTaskFile& file);
	virtual void SaveSortState(CPreferences& prefs);
	virtual void LoadSortState(const CPreferences& prefs, const CString& sFilePath);

	// we hook the list view for mouse clicks
	virtual LRESULT ScWindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp);

	void DrawColumnHeaderText(CDC* pDC, int nCol, const CRect& rCol, UINT nState);
	void UpdateListColumnWidths(BOOL bCheckListVisibility = TRUE);
	void RemoveDeletedListItems();
	void RemeasureList();
	void BuildListColumns(BOOL bResizeCols = TRUE);
	void GetItemColors(int nItem, NCGITEMCOLORS& colors);
	void UpdateTreeSelection();
	void UpdateSelectedTaskPath();
	void InvalidateItem(HTREEITEM hti);
	virtual void Resize(int cx = 0, int cy = 0);
	int FindListTask(const CString& sPart, int nStart = 0, BOOL bNext = TRUE);
	int AddItemToList(DWORD dwTaskID);

	int GetFirstSelectedItem() const;
	void ClearListSelection();
	void ResyncListSelection();
	BOOL IsItemSelected(int nItem) const;
	BOOL HasSingleSelectionChanged(DWORD dwSelID) const;
	DWORD GetSingleSelectedTaskID() const;
	DWORD GetFocusedListTaskID() const;
	int GetFocusedListItem() const;
	int GetSelectedListTaskIDs(CDWordArray& aTaskIDs, DWORD& dwFocusedTaskID) const;
	void SetSelectedListTasks(const CDWordArray& aTaskIDs, DWORD dwFocusedTaskID);
	void CacheListSelection(TDCSELECTIONCACHE& cache) const;
	void RestoreListSelection(const TDCSELECTIONCACHE& cache);
	CRect GetSelectedItemsRect() const;

	TDC_COLUMN GetListColumnID(int nCol) const;
	TDCCOLUMN* GetListColumn(int nCol) const;
	int GetListColumnIndex(TDC_COLUMN nColID) const;
	void ScrollClientColumnIntoView();
	int GetListColumnAlignment(int nDTAlign); 

	VIEWDATA* GetActiveViewData() const;
	VIEWDATA* GetViewData(FTC_VIEW nView) const;
	int FindListTask(DWORD dwTaskID) const;
	
	virtual void RebuildList(const void* pContext);
	virtual void AddTreeItemToList(HTREEITEM hti, const void* pContext);

	virtual BOOL GetItemTitleRect(HTREEITEM hti, TDC_TITLERECT nArea, CRect& rect) const;
	BOOL GetItemTitleRect(int nItem, TDC_TITLERECT nArea, CRect& rect, CDC* pDC = NULL, LPCTSTR szTitle = NULL) const;
	TDI_STATE GetListItemState(int nItem);

	HTREEITEM GetTreeItem(int nItem) const;
	int GetListItem(HTREEITEM hti) const;
	DWORD GetTaskID(int nItem) const { return m_list.GetItemData(nItem); }
	DWORD GetTaskID(HTREEITEM hti) const { return CToDoCtrl::GetTaskID(hti); }

	BOOL InListView() const;
	BOOL InTreeView() const;
	BOOL InExtensionView() const;
	BOOL IsViewSet() const;

	void UpdateExtensionViews(TDC_ATTRIBUTE nAttrib);
	void ExtensionDoAppCommand(FTC_VIEW nView, IUI_APPCOMMAND nCmd);
	BOOL ExtensionCanDoAppCommand(FTC_VIEW nView, IUI_APPCOMMAND nCmd) const;
	IUIExtensionWindow* GetExtensionWnd(FTC_VIEW nView, BOOL bAutoCreate);
	BOOL HasAnyExtensionViews() const;
	BOOL AnyExtensionViewWantsChange(TDC_ATTRIBUTE nAttrib) const;
	void BeginExtensionProgress(const VIEWDATA* pData, UINT nMsg = 0);
	void EndExtensionProgress();
	void UpdateExtensionView(IUIExtensionWindow* pExtWnd, const CTaskFile& tasks, IUI_UPDATETYPE nType, TDC_ATTRIBUTE nAttrib = TDCA_NONE);

	virtual VIEWDATA* NewViewData() { return new VIEWDATA(); }

	static BOOL IsExtensionView(FTC_VIEW nView);
};

#endif // !defined(AFX_TABBEDTODOCTRL_H__356A6EB9_C7EC_4395_8716_623AFF4A269B__INCLUDED_)
