#if !defined(AFX_TODOCTRL_H__5951FDE6_508A_4A9D_A55D_D16EB026AEF7__INCLUDED_)
#define AFX_TODOCTRL_H__5951FDE6_508A_4A9D_A55D_D16EB026AEF7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ToDoCtrl.h : header file
//

#include "TaskListDropTarget.h"
#include "todoctrldata.h"
#include "todoctrlfind.h"
#include "tdctreeview.h"
#include "taskfile.h"
#include "tdcstruct.h"
#include "tdlprioritycombobox.h"
#include "tdlriskcombobox.h"
#include "recurringtaskedit.h"
#include "tdcimagelist.h"

#include "..\shared\runtimedlg.h"
#include "..\shared\dialoghelper.h"
#include "..\shared\orderedtreectrl.h"
#include "..\shared\fileedit.h"
#include "..\shared\sysimagelist.h"
#include "..\shared\urlricheditctrl.h"
#include "..\shared\colorcombobox.h"
#include "..\shared\autocombobox.h"
#include "..\shared\maskedit.h"
#include "..\shared\timeedit.h"
#include "..\shared\TreeDragDropHelper.h"
#include "..\shared\wndPrompt.h"
#include "..\shared\contentmgr.h"
#include "..\shared\encheckcombobox.h"
#include "..\shared\contenttypecombobox.h"
#include "..\shared\timecombobox.h"
#include "..\shared\popupeditctrl.h"
#include "..\shared\uitheme.h"

#include "..\3rdparty\colourpicker.h"

/////////////////////////////////////////////////////////////////////////////
// CToDoCtrl dialog

// predeclarations
class CXmlItem;
class CTaskFile;
class CDeferWndMove;
class CDlgUnits;
struct CTRLITEM;
struct TDCCOLUMN;
class CSpellCheckDlg;
class CPreferences;
class COutlookItemDataMap;
class CTDCCsvColumnMapping;

namespace OutlookAPI
{
	class _MailItem;
	class Selection;
}

class CToDoCtrl : public CRuntimeDlg
{
// Construction
public:
	CToDoCtrl(CContentMgr& mgr, const CONTENTFORMAT& cfDefault);
	virtual ~CToDoCtrl();

	BOOL Create(const RECT& rect, CWnd* pParentWnd, UINT nID, BOOL bVisible = TRUE, BOOL bEnabled = TRUE);

	TDC_FILE Save(const CString& sFilePath = _T(""));
	TDC_FILE Save(CTaskFile& tasks/*out*/, const CString& sFilePath = _T(""));

	TDC_FILE Load(const CString& sFilePath);
	TDC_FILE Load(const CString& sFilePath, CTaskFile& tasks/*out*/);

	BOOL IsUnicode() const { return m_bIsUnicode; }
	void SetUnicode(BOOL bUnicode);
	
	BOOL DelayLoad(const CString& sFilePath, COleDateTime& dtEarliestDue);
	BOOL IsDelayLoaded() const { return m_bDelayLoaded; }

	BOOL ArchiveDoneTasks(const CString& sFilePath, TDC_ARCHIVE nFlags, BOOL bRemoveFlagged); // returns true if any tasks were removed
	BOOL ArchiveSelectedTasks(const CString& sFilePath, BOOL bRemove); // returns true if any tasks were removed

	void EnableEncryption(BOOL bEnable = TRUE);
	BOOL IsEncrypted() const { return (!m_sPassword.IsEmpty()); }
	static BOOL CanEncrypt(); // returns true only if the required encryption capabilities are present
	CString GetPassword() const { return m_sPassword; }
	BOOL VerifyPassword(const CString& sExplanation = _T("")) const;

	void SetMaximizeState(TDC_MAXSTATE nState);
	virtual BOOL WantTaskContextMenu() const { return TRUE; }

	BOOL CheckIn();
	BOOL CheckOut();
	BOOL CheckOut(CString& sCheckedOutTo);
	BOOL IsCheckedOut() const { return m_bCheckedOut; }
	BOOL IsSourceControlled() const { return m_bSourceControlled; }
	BOOL RemoveFromSourceControl();

	void Flush(BOOL bEndTimeTracking = FALSE, BOOL bResetCalcs = FALSE); // called to end current editing actions
	BOOL IsModified() const { return (m_bModified || m_bCommentsChange) && !IsReadOnly(); }
	void SetModified(BOOL bMod = TRUE);

	CString GetFilePath() const { return m_sLastSavePath; }
	void ClearFilePath() { m_sLastSavePath.Empty(); }
	CString GetProjectName() const { return m_sProjectName; }
	void SetProjectName(const CString& sProjectName);
	CString GetFriendlyProjectName(int nUntitledIndex = -1) const;
	void SetFilePath(const CString& sPath) { m_sLastSavePath = sPath; }

	int GetCustomAttributeDefs(CTDCCustomAttribDefinitionArray& aAttrib) const;
	void SetCustomAttributeDefs(const CTDCCustomAttribDefinitionArray& aAttrib);

	BOOL DeleteAllTasks();
	void NewList();

	int GetTasks(CTaskFile& tasks, const TDCGETTASKS& filter = TDCGETTASKS(TDCGT_ALL)) const;
	int GetSelectedTasks(CTaskFile& tasks, const TDCGETTASKS& filter = TDCGETTASKS(TDCGT_ALL), DWORD dwFlags = 0) const;

	BOOL InsertTasks(const CTaskFile& tasks, TDC_INSERTWHERE nWhere);

	void SetReadonly(BOOL bReadOnly) { SetStyle(TDCS_READONLY, bReadOnly); }
	BOOL IsReadOnly() const { return HasStyle(TDCS_READONLY); }

	BOOL SetStyles(const CTDCStylesMap& styles);
	BOOL SetStyle(TDC_STYLE nStyle, BOOL bOn = TRUE); // one style at a time only 
	BOOL HasStyle(TDC_STYLE nStyle) const; // one style at a time only 

	void SetVisibleColumns(const CTDCColumnIDArray& aColumns);
	int GetVisibleColumns(CTDCColumnIDArray& aColumns) const;
	BOOL IsColumnShowing(TDC_COLUMN nColumn) const;
	void SetMaxColumnWidth(int nWidth);

	BOOL SetPriorityColors(const CDWordArray& aColors); // must have 12 elements
	COLORREF GetPriorityColor(int nPriority) const;
    void SetGridlineColor(COLORREF color);
	void SetCompletedTaskColor(COLORREF color);
	void SetFlaggedTaskColor(COLORREF color);
	void SetReferenceTaskColor(COLORREF color);
	void SetAttributeColors(TDC_ATTRIBUTE nAttrib, const CTDCColorMap& colors);
	void SetStartedTaskColors(COLORREF crStarted, COLORREF crStartedToday);
	void GetStartedTaskColors(COLORREF& crStarted, COLORREF& crStartedToday) { crStarted = m_crStarted; crStartedToday = m_crStartedToday; }
	void SetDueTaskColors(COLORREF crDue, COLORREF crDueToday);
	void GetDueTaskColors(COLORREF& crDue, COLORREF& crDueToday) { crDue = m_crDue; crDueToday = m_crDueToday; }
	void SetAlternateLineColor(COLORREF color) { m_tree.SetAlternateLineColor(color); }

	void SetUITheme(const UITHEME& theme);

	// these return the full list of items in each droplist
	int GetAllocToNames(CStringArray& aNames) const { return m_cbAllocTo.GetItems(aNames); }
	int GetAllocByNames(CStringArray& aNames) const { return m_cbAllocBy.GetItems(aNames); }
	int GetCategoryNames(CStringArray& aNames) const { return m_cbCategory.GetItems(aNames); }
	int GetTagNames(CStringArray& aNames) const { return m_cbTags.GetItems(aNames); }
	int GetStatusNames(CStringArray& aNames) const { return m_cbStatus.GetItems(aNames); }
	int GetVersionNames(CStringArray& aNames) const { return m_cbVersion.GetItems(aNames); }

	void SetDefaultAllocToNames(const CStringArray& aNames);
	void SetDefaultAllocByNames(const CStringArray& aNames);
	void SetDefaultCategoryNames(const CStringArray& aNames);
	void SetDefaultStatusNames(const CStringArray& aNames);
	void SetDefaultTagNames(const CStringArray& aNames);

	HTREEITEM CreateNewTask(const CString& sText, TDC_INSERTWHERE nWhere, BOOL bEditText = TRUE);
	BOOL CanCreateNewTask(TDC_INSERTWHERE nInsertWhere) const;

	void SetSubtaskDragDropPos(BOOL bTop = TRUE) { m_bDragDropSubtasksAtTop = bTop; }

	virtual BOOL SplitSelectedTask(int nNumSubtasks = 2);
	virtual BOOL CanSplitSelectedTask();

	DWORD GetSelectedTaskID() const { return m_find.GetTaskID(GetSelectedItem()); } 
	TODOITEM* GetSelectedTask() const { return m_find.GetTask(GetSelectedItem()); }
	virtual BOOL SelectTask(DWORD dwTaskID);
	int GetSelectedTaskIDs(CDWordArray& aTaskIDs) const;
	int GetSelectedTaskIDs(CDWordArray& aTaskIDs, DWORD& dwFocusedTaskID, BOOL bRemoveChildDupes) const;
	void CacheTreeSelection(TDCSELECTIONCACHE& cache) const;
	void RestoreTreeSelection(const TDCSELECTIONCACHE& cache);

	virtual BOOL IsTaskDone(DWORD dwTaskID) const { return m_data.IsTaskDone(dwTaskID); }
	BOOL IsTaskDone(DWORD dwTaskID, DWORD dwExtraCheck) const { return m_data.IsTaskDone(dwTaskID, dwExtraCheck); }
	virtual BOOL IsTaskRecurring(DWORD dwTaskID) const;

	BOOL DeleteSelectedTask();
	BOOL EditSelectedTask(BOOL bTaskIsNew = FALSE); 
	void SpellcheckSelectedTask(BOOL bTitle); // else comments
	BOOL CanSpellcheckSelectedTaskComments();
	
	BOOL GotoSelectedTaskDependency(); 
	BOOL SetSelectedTaskDependencies(const CStringArray& aDepends);

	BOOL EditSelectedTaskRecurrence(); 
	BOOL SetSelectedTaskRecurrence(const TDIRECURRENCE& tr);

	BOOL EditSelectedTaskIcon(); 
	BOOL ClearSelectedTaskIcon(); 

	BOOL SetSelectedTaskDone(BOOL bDone = TRUE);
	BOOL IsSelectedTaskDone() const;
	BOOL IsSelectedTaskDue() const;
	BOOL OffsetSelectedTaskDate(TDC_DATE nDate, int nAmount, int nUnits, BOOL bAndSubtasks);
	COleDateTime GetEarliestDueDate() const { return m_data.GetEarliestDueDate(); } // entire tasklist

	COLORREF GetSelectedTaskColor() const; // -1 on no item selected
	CString GetSelectedTaskIcon() const;
	CString GetSelectedTaskComments() const;
	const CBinaryData& GetSelectedTaskCustomComments(CString& sCommentsTypeID) const;
	CString GetSelectedTaskTitle() const;
	double GetSelectedTaskTimeEstimate(TCHAR& nUnits) const;
	double GetSelectedTaskTimeSpent(TCHAR& nUnits) const;
	int GetSelectedTaskAllocTo(CStringArray& aAllocTo) const;
	CString GetSelectedTaskAllocBy() const;
	CString GetSelectedTaskStatus() const;
	int GetSelectedTaskCategories(CStringArray& aCats) const;
	int GetSelectedTaskDependencies(CStringArray& aDepends) const;
	int GetSelectedTaskTags(CStringArray& aTags) const;
	CString GetSelectedTaskFileRef() const;
	CString GetSelectedTaskExtID() const;
	int GetSelectedTaskPercent() const;
	int GetSelectedTaskPriority() const;
	int GetSelectedTaskRisk() const;
	double GetSelectedTaskCost() const;
	BOOL IsSelectedTaskFlagged() const;
	BOOL GetSelectedTaskRecurrence(TDIRECURRENCE& tr) const;
	CString GetSelectedTaskVersion() const;
	BOOL SelectedTaskHasDate(TDC_DATE nDate) const;
	CString GetSelectedTaskPath(BOOL bIncludeTaskName, int nMaxLen = -1) const;
	COleDateTime GetSelectedTaskDate(TDC_DATE nDate) const;
	CString GetSelectedTaskCustomAttributeData(const CString& sAttribID) const;
	int GetSelectedTaskCustomAttributeData(CMapStringToString& mapData) const;
	BOOL IsSelectedTaskReference() const;

	CString GetTaskPath(DWORD dwTaskID, int nMaxLen = -1) const { return m_data.GetTaskPath(dwTaskID, nMaxLen); }
	CString GetTaskTitle(DWORD dwTaskID) const { return m_data.GetTaskTitle(dwTaskID); }
	COleDateTime GetTaskDate(DWORD dwID, TDC_DATE nDate) const;

	double CalcSelectedTaskTimeEstimate(TCHAR nUnits = TDITU_HOURS) const;
	double CalcSelectedTaskTimeSpent(TCHAR nUnits = TDITU_HOURS) const;
	double CalcSelectedTaskCost() const;

	BOOL SetSelectedTaskColor(COLORREF color);
	BOOL ClearSelectedTaskColor() { return SetSelectedTaskColor((COLORREF)-1); }
	BOOL SetSelectedTaskTitle(const CString& sTitle);
	BOOL SetSelectedTaskPercentDone(int nPercent);
	BOOL SetSelectedTaskTimeEstimate(double dTime, TCHAR nUnits = TDITU_HOURS);
	BOOL SetSelectedTaskTimeSpent(double dTime, TCHAR nUnits = TDITU_HOURS);
	BOOL SetSelectedTaskTimeEstimateUnits(TCHAR nUnits, BOOL bRecalcTime = FALSE);
	BOOL SetSelectedTaskTimeSpentUnits(TCHAR nUnits, BOOL bRecalcTime = FALSE);
	BOOL SetSelectedTaskAllocTo(const CStringArray& aAllocTo);
	BOOL SetSelectedTaskAllocBy(const CString& sAllocBy);
	BOOL SetSelectedTaskStatus(const CString& sStatus);
	BOOL SetSelectedTaskCategories(const CStringArray& aCats);
	BOOL SetSelectedTaskTags(const CStringArray& aTags);
	BOOL SetSelectedTaskPriority(int nPriority); // 0-10 (10 is highest)
	BOOL SetSelectedTaskRisk(int nRisk); // 0-10 (10 is highest)
	BOOL SetSelectedTaskFileRef(const CString& sFilePath);
	BOOL SetSelectedTaskExtID(const CString& sID);
	BOOL SetSelectedTaskFlag(BOOL bFlagged);
	BOOL SetSelectedTaskCost(double dCost);
	BOOL SetSelectedTaskVersion(const CString& sVersion);
	BOOL SetSelectedTaskComments(const CString& sComments, const CBinaryData& customComments);
	BOOL SetSelectedTaskIcon(const CString& sIcon); 
	BOOL SetSelectedTaskDate(TDC_DATE nDate, const COleDateTime& date);
	BOOL SetSelectedTaskCustomAttributeData(const CString& sAttribID, const CString& sData);

	BOOL CanClearSelectedTaskFocusedAttribute() const;
	BOOL ClearSelectedTaskFocusedAttribute();
	BOOL CanClearSelectedTaskAttribute(TDC_ATTRIBUTE nAttrib) const;
	BOOL ClearSelectedTaskAttribute(TDC_ATTRIBUTE nAttrib);

	BOOL CanPasteText(); // into focused control
	BOOL PasteText(const CString& sText); // into focused control

	BOOL IncrementSelectedTaskPercentDone(BOOL bUp = TRUE); // +ve or -ve
	BOOL IncrementSelectedTaskPriority(BOOL bUp = TRUE); // +ve or -ve
	void SetPercentDoneIncrement(int nAmount);

	BOOL GotoSelectedTaskFileRef();

	// time tracking
	void PauseTimeTracking(BOOL bPause = TRUE) { m_bTimeTrackingPaused = bPause; }
	BOOL TimeTrackSelectedTask();
	BOOL CanTimeTrackSelectedTask() const;
	BOOL IsSelectedTaskBeingTimeTracked() const;
	BOOL IsActivelyTimeTracking() const; // this instant
	CString GetSelectedTaskTimeLogPath() const;
	virtual void EndTimeTracking();
	void ResetTimeTracking() { m_dwTickLast = GetTickCount(); }
	void AddTimeToTaskLogFile(DWORD dwTaskID, double dHours, const COleDateTime& dtWhen, BOOL bAddToTimeSpent = FALSE);

	static void SetInheritedParentAttributes(const CTDCAttributeArray& aAttribs, BOOL bUpdateAttrib);
	void SetDefaultTaskAttributes(const TODOITEM& tdi);

	// sort functions
	virtual void Sort(TDC_COLUMN nBy, BOOL bAllowToggle = TRUE); // calling twice with the same param will toggle ascending attrib
	virtual void MultiSort(const TDSORTCOLUMNS& sort);
	virtual TDC_COLUMN GetSortBy() const { return m_sort.nBy1; }
	virtual void GetSortBy(TDSORTCOLUMNS& sort) const;
	void Resort(BOOL bAllowToggle = FALSE);
	BOOL IsSortable() const;
	virtual BOOL IsMultiSorting() const;
	static int CALLBACK SortFunc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort); 
	static int CALLBACK SortFuncMulti(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort); 
	static int SortTasks(LPARAM lParam1, LPARAM lParam2, TDC_COLUMN nSortBy, BOOL bAscending, 
						BOOL bSortDueTodayHigh, DWORD dwTimeTrackedID, const CToDoCtrlData* pData,
						const CTDCCustomAttribDefinitionArray* pAttibuteDefs);

	// move functions
	virtual BOOL MoveSelectedTask(TDC_MOVETASK nDirection);
	virtual BOOL CanMoveSelectedTask(TDC_MOVETASK nDirection) const;

	virtual BOOL GotoNextTask(TDC_GOTO nDirection); 
	virtual BOOL CanGotoNextTask(TDC_GOTO nDirection) const;
	virtual BOOL GotoNextTopLevelTask(TDC_GOTO nDirection); 
	virtual BOOL CanGotoNextTopLevelTask(TDC_GOTO nDirection) const;

	virtual BOOL CanExpandTasks(TDC_EXPANDCOLLAPSE nWhat, BOOL bExpand) const;
	virtual void ExpandTasks(TDC_EXPANDCOLLAPSE nWhat, BOOL bExpand = TRUE);

	// copy/paste functions
	BOOL CutSelectedTask();
	BOOL CopySelectedTask() const;
	void ClearCopiedItem() const;
	BOOL PasteTasks(TDC_PASTE nWhere);
	BOOL CanPaste() const;

	void CToDoCtrl::ResetFileVersion(unsigned int nTo = 0) { m_nFileVersion = max(nTo, 0); }
	DWORD CToDoCtrl::GetFileVersion() const { return m_nFileVersion == 0 ? 1 : m_nFileVersion; }
    TDC_FILEFMT CompareFileFormat() const; // older, same, newer
	
	// tree related
	HTREEITEM GetSelectedItem() const;
	UINT GetTaskCount() const { return m_data.GetTaskCount(); }
	BOOL ItemHasChildren(HTREEITEM hti) const { return m_tree.ItemHasChildren(hti); }
	BOOL ItemHasParent(HTREEITEM hti) const { return (NULL != m_tree.GetParentItem(hti)); }
	int GetSelectedCount() const { return Selection().GetCount(); }
	BOOL IsItemSelected(HTREEITEM hti) const { return Selection().HasItem(hti); }
	BOOL HasSelection() const { return GetSelectedCount(); }
	BOOL IsTaskLabelEditing() const;

	virtual BOOL TasksHaveFocus() const { return (::GetFocus() == m_tree); }
	virtual void SetFocusToTasks();
	virtual void SetFocusToComments();
	virtual CString GetControlDescription(const CWnd* pCtrl) const;
	virtual BOOL GetSelectionBoundingRect(CRect& rSelection) const;

	void SelectItem(HTREEITEM hti);
	void SelectAll();
	void DeselectAll(); // call externally only
	BOOL MultiSelectItems(const CDWordArray& aTasks, TSH_SELECT nState = TSHS_SELECT, BOOL bRedraw = TRUE);
	BOOL MultiSelectItem(HTREEITEM hti, TSH_SELECT nState = TSHS_SELECT, BOOL bRedraw = TRUE);
	BOOL MultiSelectItems(HTREEITEM htiFrom, HTREEITEM htiTo, TSH_SELECT nState = TSHS_SELECT, BOOL bRedraw = TRUE);

	BOOL SelectTask(CString sPart, TDC_SELECTTASK nSlect);
	virtual BOOL SelectedTasksHaveChildren() const;
	BOOL SelectedTasksHaveIcons() const;
	BOOL SelectedTasksAreAllDone() const;

	BOOL CanSelectNextTasksInHistory() const { return Selection().HasNextSelection(); }
	void SelectNextTasksInHistory();
	BOOL CanSelectPrevTasksInHistory() const { return Selection().HasPrevSelection(); }
	void SelectPrevTasksInHistory();

	BOOL SetTreeFont(HFONT hFont); // setter responsible for deleting
	BOOL SetCommentsFont(HFONT hFont); // setter responsible for deleting

	BOOL SetCheckImageList(HIMAGELIST hImageList); // setter responsible for deleting
	HIMAGELIST GetCheckImageList() const { return m_hilDone; }
	const CTDCImageList& GetTaskIconImageList() const { return m_ilTaskIcons; }

	int FindTasks(const SEARCHPARAMS& params, CResultArray& aResults) const;
	DWORD FindFirstTask(const SEARCHPARAMS& params, SEARCHRESULT& result) const;
	DWORD FindNextTask(const SEARCHPARAMS& params, SEARCHRESULT& result) const;
	BOOL HasTask(DWORD dwTaskID) const { return (m_data.GetTask(dwTaskID) != NULL); }

	// undo/redo
	BOOL UndoLastAction(BOOL bUndo);
	BOOL CanUndoLastAction(BOOL bUndo) const;

	// misc
	void Spellcheck();
	void SetMaxInfotipCommentsLength(int nLength) { m_nMaxInfotipCommentsLength = max(-1, nLength); } // -1 to switch off
	COleDateTime GetLastTaskModified() { return m_tLastTaskMod; }
	BOOL PtInHeader(CPoint ptScreen) const;
	TDC_COLUMN ColumnHitTest(CPoint ptScreen) const;
	void RedrawReminders() const;
	void SetLayoutPositions(TDC_UILOCATION nControlsPos, TDC_UILOCATION nCommentsPos, BOOL bResize);

	CString GetPreferencesKey(const CString& sSubKey = _T(""), const CString& sFilePath = _T("")) const;
	static BOOL GetPreferencesKey(const CString& sFilePath, const CString& sSubKey, CString& sKey);
	virtual void NotifyBeginPreferencesUpdate() {}
	virtual void NotifyEndPreferencesUpdate() {}

	static void ParseTaskLink(const CString& sLink, DWORD& dwTaskID, CString& sFile);
	static BOOL IsReservedShortcut(DWORD dwShortcut);
	static void EnableExtendedSelection(BOOL bCtrl, BOOL bShift);

protected:
	CDateTimeCtrl m_dateStart, m_dateDue, m_dateDone;
	CTimeComboBox m_timeDue, m_timeStart, m_timeDone;
	CTDLPriorityComboBox m_cbPriority;
	CTDLRiskComboBox m_cbRisk;
	CEnEdit m_eExternalID, m_eDependency;
	CSpinButtonCtrl m_spinPercent;
	CMaskEdit m_ePercentDone, m_eCost;
	CAutoComboBox m_cbAllocBy;
	CAutoComboBox m_cbStatus;
	CCheckComboBox m_cbCategory, m_cbAllocTo, m_cbTags;
	CTimeEdit m_eTimeEstimate, m_eTimeSpent;
	CFileEdit m_eFileRef;
	CContentCtrl m_ctrlComments;
	CTDLRecurringTaskEdit m_eRecurrence;
	CAutoComboBox m_cbVersion;
	CContentTypeComboBox m_cbCommentsType;
	CColourPicker m_cpColour;
	CPopupEditCtrl m_eTaskName;
	
	CTDCTreeView m_tree;

	HFONT m_hFontTree, m_hFontDone, m_hFontComments, m_hFontBold;
	HIMAGELIST m_hilDone;
	CSysImageList m_ilFileRef;
	CTDCImageList m_ilTaskIcons;
	CBrush m_brDue, m_brDueToday, m_brUIBack;
	COLORREF m_crStarted, m_crStartedToday, m_crDue; 
	COLORREF m_crDueToday, m_crFlagged, m_crReference, m_crDone;
	UITHEME m_theme;

	CToDoCtrlData m_data;
	CToDoCtrlFind m_find;

	TDSORTCOLUMNS m_sort;
	CWordArray m_aStyles;
	CDWordArray m_aPriorityColors;
	CString m_sXmlHeader;
	CTaskListDropTarget m_dtTree, m_dtFileRef;
	CString m_sLastSavePath;
	int m_nCommentsSize;
	CString m_sPassword;
	WORD m_wKeyPress;
	CTreeDragDropHelper m_treeDragDrop;
	CTDCColorMap m_mapAttribColors;
	TDC_ATTRIBUTE m_nColorByAttrib;
	int m_nMaxInfotipCommentsLength;
	TDLSELECTION m_selection;
	CWndPromptManager m_mgrPrompts;
	COleDateTime m_tLastTaskMod;
	CContentMgr& m_mgrContent;
	CTDCColumnIDArray m_aVisibleColumns;
	CStringArray m_aDefCategory, m_aDefStatus, m_aDefAllocTo, m_aDefAllocBy, m_aDefTags;
	TDC_MAXSTATE m_nMaxState;
	TDC_UILOCATION m_nControlsPos, m_nCommentsPos;
	int m_nPercentIncrement;

	CString m_sFileRefPath;
	CString m_sTextComments;
	CBinaryData m_customComments;
	CString m_sAllocBy;
	CString m_sStatus;
	CStringArray m_aCategory, m_aAllocTo, m_aTags;
	CString m_sProjectName;
	CString m_sExternalID, m_sDepends;
	double m_dTimeEstimate, m_dTimeSpent;
	double m_dCost;
	double m_dLogTime; // in hours
	int m_nPriority;
	int m_nRisk;
	int m_nPercentDone;
	TCHAR m_nTimeEstUnits, m_nTimeSpentUnits;
	CONTENTFORMAT m_cfComments, m_cfDefault;
	TDIRECURRENCE m_tRecurrence;
	CString m_sOccurrence;
	CString m_sVersion;
	COLORREF m_crColour;
	float m_fAveHeaderCharWidth;
	int m_nMaxColWidth;
	CMapStringToString m_mapMetaData;
	CMapStringToString m_mapCustomCtrlData;

	CTDCCustomAttribDefinitionArray m_aCustomAttribDefs;
	CTDCCustomControlArray m_aCustomControls;

	DWORD m_dwNextUniqueID;
	DWORD m_nFileVersion;
	DWORD m_nFileFormat;
	DWORD m_dwTimeTrackTaskID;
	DWORD m_dwTickLast; // time tracking
	DWORD m_dwLastAddedID;
	DWORD m_dwEditingID;

	BOOL m_bModified;
	BOOL m_bArchive;
	BOOL m_bModSinceLastSort;
	BOOL m_bCheckedOut; // intentionally not a style
	BOOL m_bSplitting; // dragging comments splitter
	BOOL m_bTimeTrackingPaused;
	BOOL m_bSourceControlled;
	BOOL m_bDragDropSubtasksAtTop;
	BOOL m_bCommentsChange;
	BOOL m_bDelayLoaded;
	BOOL m_bMultiSort;
	BOOL m_bFirstLoadCommentsPrefs;
	BOOL m_bIsUnicode;
	BOOL m_bDeletingAll;

	static int s_nCommentsSize; // TDCS_SHAREDCOMMENTSHEIGHT
	static TODOITEM s_tdDefault;
	static short s_nExtendedSelection;
	static CTDCAttributeArray s_aParentAttribs; // inheritable attribs for new tasks
	static BOOL s_bUpdateInheritAttrib; // update as changes are made to parents

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CToDoCtrl)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	virtual void PreSubclassWindow();
	virtual BOOL OnInitDialog();
	virtual void OnTimerMidnight();

	// Implementation
protected:
	void UpdateComments(BOOL bSaveAndValidate); 
	
	// Generated message map functions
	//{{AFX_MSG(CToDoCtrl)
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
	//}}AFX_MSG
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnDestroy();
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	afx_msg void OnCaptureChanged(CWnd *pWnd);
	afx_msg void OnShowWindow(BOOL bShow, UINT nStatus);
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnTreeGetDispInfo(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTreeBeginlabeledit(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTreeSelChanged(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnDatechange(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTreeCustomDraw(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTreeChangeFocus(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTreeKeyDown(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTreeGetInfoTip(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg LRESULT OnTreeDragDrop(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnTreeDragOver(WPARAM wParam, LPARAM lParam);
	afx_msg void OnChangePriority();
	afx_msg LRESULT OnChangeComments(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnCommentsKillFocus(WPARAM wParam, LPARAM lParam);
	afx_msg void OnChangePercent();
	afx_msg void OnChangeTimeEstimate();
	afx_msg void OnChangeTimeSpent();
	afx_msg void OnSelChangeDueTime();
	afx_msg void OnSelChangeDoneTime();
	afx_msg void OnSelChangeStartTime();
	afx_msg void OnSelChangeAllocTo();
	afx_msg void OnSelChangeAllocBy();
	afx_msg void OnSelChangeStatus();
	afx_msg void OnSelChangeVersion();
	afx_msg void OnSelChangeCategory();
	afx_msg void OnSelChangeTag();
	afx_msg void OnSelCancelAllocTo();
	afx_msg void OnSelCancelCategory();
	afx_msg void OnSelCancelTag();
	afx_msg void OnChangeRisk();
	afx_msg void OnChangeProjectName();
	afx_msg void OnChangeCost();
	afx_msg void OnChangeDependency();
	afx_msg void OnChangeExternalID();
	afx_msg void OnChangeFileRefPath();
	afx_msg void OnChangeRecurrence();
	afx_msg LRESULT OnGutterWantRedraw(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterDrawItem(WPARAM wParam, LPARAM lParam); 
	afx_msg LRESULT OnGutterDrawItemColumn(WPARAM wParam, LPARAM lParam); 
	afx_msg LRESULT OnGutterPostDrawItem(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterRecalcColWidth(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterNotifyHeaderClick(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterNotifyScroll(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterWidthChange(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterGetCursor(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterNotifyItemClick(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGutterGetItemColors(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnEEBtnClick(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnCustomUrl(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnTimeUnitsChange(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnDropObject(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnFileEditWantIcon(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnFileEditDisplayFile(WPARAM wParam, LPARAM lParam);
	afx_msg void OnGotoFileRef();
	afx_msg LRESULT OnTreeRestoreFocusedItem(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnTDCHasClipboard(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnTDCGetClipboard(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnTDCDoTaskLink(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnAutoComboAddDelete(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGetFont(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnCommentsWantSpellCheck(WPARAM wParam, LPARAM lParam);
	afx_msg void OnSelChangeCommentsType();
	afx_msg void OnSettingChange(UINT uFlags, LPCTSTR lpszSection);
	afx_msg LRESULT OnRefreshPercentSpinVisibility(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnChangeColour(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnEditEnd(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnEditCancel(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnRecreateRecurringTask(WPARAM wParam, LPARAM lParam);

	// custom data notifications
	afx_msg void OnCustomAttributeChange(UINT nCtrlID, NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnCustomAttributeChange(UINT nCtrlID);
	DECLARE_MESSAGE_MAP()

	TODOITEM* GetTask(DWORD dwTaskID) const { return m_data.GetTask(dwTaskID); }
	DWORD GetTaskID(HTREEITEM hti) const { return m_find.GetTaskID(hti); }
	DWORD GetTrueTaskID(HTREEITEM hti) const { return m_data.GetTrueTaskID(m_find.GetTaskID(hti)); }

	void InvalidateSelectedItem();
	virtual void InvalidateItem(HTREEITEM hti);
	void UpdateTask(TDC_ATTRIBUTE nAttrib, DWORD dwFlags = 0);
	void UpdateControls(HTREEITEM hti = NULL);
	virtual void UpdateSelectedTaskPath();
	CRect GetSplitterRect();
	void SetCtrlDate(CDateTimeCtrl& ctrl, const COleDateTime& date);
	BOOL CreateContentControl(BOOL bResendComments = FALSE);
	int GetMaxTaskCategoryWidth(CDC* pDC, BOOL bVisibleOnly = TRUE);
	int GetMaxTaskTagWidth(CDC* pDC, BOOL bVisibleOnly = TRUE);
	void RefreshItemStates(HTREEITEM hti = NULL, BOOL bBold = TRUE, BOOL bCheck = TRUE, BOOL bAndChildren = TRUE);
	void IncrementTrackedTime();
	BOOL FormatDate(const COleDateTime& date, TDC_DATE nDate, CString& sDate, CString& sTime, CString& sDow);
	int CalcMaxDateColWidth(TDC_DATE nDate, CDC* pDC);
	BOOL SetCommentsFont(HFONT hFont, BOOL bResendComments);
	virtual BOOL SetStyle(TDC_STYLE nStyle, BOOL bOn, BOOL bWantUpdate);
	short CalcMinItemImageHeight() const;
	void UpdateItemHeight();
	int GetNextPercentDone(int nPercent, BOOL bUp);
	BOOL GetAttributeColor(const CString& sAttrib, COLORREF& color) const;
	
	BOOL RemoveOrphanTaskReferences(HTREEITEM hti = NULL);
	BOOL IsTaskReference(DWORD dwTaskID) const { return m_data.IsTaskReference(dwTaskID); }

	virtual void EndLabelEdit(BOOL bCancel);
	BOOL ShowLabelEdit(const CRect& rPos);
	virtual BOOL GetItemTitleRect(HTREEITEM hti, TDC_TITLERECT nArea, CRect& rect) const;

	// internal version so we can tell how we've been called
	BOOL SetSelectedTaskComments(const CString& sComments, const CBinaryData& customComments, BOOL bInternal);
	
	int SetTaskDone(HTREEITEM hti, const COleDateTime& date, TDC_SETTASKDONE nSetChildren);
	BOOL SetSelectedTaskDone(const COleDateTime& date, BOOL bDateEdited);
	int CheckWantSubtasksCompleted();
	BOOL SetSelectedTaskDate(TDC_DATE nDate, const COleDateTime& date, BOOL bDateEdited);

	virtual TODOITEM* CreateNewTask(HTREEITEM htiParent); // overridable
	virtual BOOL DeleteSelectedTask(BOOL bWarnUser, BOOL bResetSel = FALSE);
	virtual COLORREF GetItemLineColor(HTREEITEM hti) { return m_tree.GetItemLineColor(hti); }

	virtual void SetModified(BOOL bMod, TDC_ATTRIBUTE nAttrib, DWORD dwModTaskID);
	void ResetSelectedTaskCachedCalculations(TDC_ATTRIBUTE nAttrib) const;
	void ResetCachedCalculations(TDC_ATTRIBUTE nAttrib = TDCA_ALL) const;

	void SaveGlobals(CTaskFile& file) const;
	void LoadGlobals(const CTaskFile& file);

	void SaveCustomAttributeDefinitions(CTaskFile& file) const;
	void LoadCustomAttributeDefinitions(const CTaskFile& file);
	virtual void RebuildCustomAttributeUI();

	DWORD GetTaskParentID(HTREEITEM hti) const;

	virtual BOOL ModNeedsResort(TDC_ATTRIBUTE nModType) const;
	BOOL ModNeedsResort(TDC_ATTRIBUTE nModType, TDC_COLUMN nSortBy) const;
	static BOOL IsSortable(TDC_COLUMN nSortBy);
	virtual void ResortSelectedTaskParents();

	BOOL HandleCustomColumnClick(TDC_COLUMN nColID);
	void ProcessItemLButtonDown(HTREEITEM htiHit, int nHitFlags, TDC_COLUMN nColID);
	void ProcessItemLButtonUp(HTREEITEM htiHit, int nHitFlags, TDC_COLUMN nColID);
	UINT MapColumnToCtrlID(TDC_COLUMN nColID) const;
	TDC_COLUMN MapCtrlIDToColumn(UINT nCtrlID) const;
	CString GetSourceControlID(BOOL bAlternate = FALSE) const;

	BOOL CopyCurrentSelection() const;
	BOOL IsClipboardEmpty(BOOL bCheckID = FALSE) const;
	CString GetClipboardID() const;
	void RestoreTreeSelection(const CDWordArray& aTaskIDs, DWORD dwDefaultID = 0);

	inline CTreeSelectionHelper& Selection() { return m_selection.selection; }
	inline const CTreeSelectionHelper& Selection() const { return m_selection.selection; }

	virtual void Resize(int cx = 0, int cy = 0);
	int GetControls(CTDCControlArray& aControls, BOOL bVisible) const;
	BOOL IsCtrlShowing(const CTRLITEM& ctrl) const;
	int GetMinNonCommentSize();
	virtual void UpdateTasklistVisibility();
	void ShowHideControls();
	void ShowHideControl(const CTRLITEM& ctrl);
	void EnableDisableControls(HTREEITEM hti);
	void EnableDisableControl(const CTRLITEM& ctrl, DWORD dwTaskID, BOOL bEnable, BOOL bReadOnly, BOOL bIsParent, BOOL bEditTime);

	void ReposControl(const CTRLITEM& ctrl, CDeferWndMove* pDWM, const CDlgUnits* pDLU, 
						const CRect& rItem, int nClientRight);
	void ReposControls(CDeferWndMove* pDWM, CRect& rAvailable /*in/out*/);
	void ReposComments(CDeferWndMove* pDWM, CRect& rAvailable /*in/out*/);
	void ReposProjectName(CDeferWndMove* pDWM, CRect& rAvailable /*in/out*/);
	virtual void ReposTaskTree(CDeferWndMove* pDWM, const CRect& rAvailable /*in*/);

	int AddTreeChildrenToTaskFile(HTREEITEM hti, CTaskFile& file, HTASKITEM hTask, const TDCGETTASKS& filter) const;
	BOOL AddTreeItemToTaskFile(HTREEITEM hti, CTaskFile& file, HTASKITEM hParentTask, const TDCGETTASKS& filter, BOOL bWantSubtasks = TRUE, DWORD dwParentID = 0) const;
	BOOL AddItemAndParentToTaskFile(HTREEITEM hti, CTaskFile& tasks, const TDCGETTASKS& filter, BOOL bAllParents, BOOL bWantSubtasks) const;
	BOOL SetTaskAttributes(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, CTaskFile& file, HTASKITEM hTask, const TDCGETTASKS& filter, BOOL bTitleCommentsOnly) const;

	BOOL AddSubTasksToTaskFile(const TODOSTRUCTURE* pTDSParent, CTaskFile& tasks, HTASKITEM hParentTask) const;
	BOOL AddTaskToTaskFile(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, /*int nPos,*/ CTaskFile& tasks, HTASKITEM hParentTask) const;
	BOOL SetAllTaskAttributes(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, CTaskFile& file, HTASKITEM hTask) const;

	HTREEITEM AddTaskToTreeItem(const CTaskFile& file, HTASKITEM hTask, HTREEITEM htiParent = NULL, HTREEITEM htiAfter = TVI_LAST, TDC_RESETIDS nResetID = TDCR_NO);
	virtual BOOL AddTasksToTree(const CTaskFile& tasks, HTREEITEM htiDest, HTREEITEM htiDestAfter, TDC_RESETIDS nResetID);
	HTREEITEM InsertItem(const CString& sText, HTREEITEM htiParent, HTREEITEM htiAfter, BOOL bEdit);
	BOOL GetInsertLocation(TDC_INSERTWHERE nWhere, HTREEITEM& htiDest, HTREEITEM& htiDestAfter) const;

	virtual BOOL LoadTasks(const CTaskFile& file);
	BOOL CheckRestoreBackupFile(const CString& sFilePath);

	static TDCCOLUMN* GetColumn(TDC_COLUMN nColID);
	static int GetColumnIndex(TDC_COLUMN nColID);

	void SaveExpandedState(CPreferences& prefs); // keyed by last filepath
	HTREEITEM LoadExpandedState(const CPreferences& prefs); // returns the previously selected item if any
	void SaveSplitPos(CPreferences& prefs);
	void LoadSplitPos(const CPreferences& prefs);
	virtual void SaveSortState(CPreferences& prefs); // keyed by last filepath
	virtual void LoadSortState(const CPreferences& prefs, const CString& sFilePath); // keyed by filepath
	int SaveTreeExpandedState(CPreferences& prefs, const CString& sKey, HTREEITEM hti = NULL, int nStart = 0); 
	void LoadTreeExpandedState(const CPreferences& prefs, const CString& sKey); 
	
	void TimeTrackTask(HTREEITEM hti);

	BOOL SetTextChange(int nChange, CString& sItem, const CString& sNewItem, TDC_ATTRIBUTE nAttrib, UINT nIDC, DWORD dwTaskID, CAutoComboBox* pCombo = NULL);
	void DrawGutterItemText(CDC* pDC, const CString& sText, const CRect& rect, int nAlign, COLORREF crText = (COLORREF)-1, CFont* pFont = NULL, BOOL bSymbol = FALSE);
	void DrawGutterItemDate(CDC* pDC, const COleDateTime& date, TDC_DATE nDate, const CRect& rect, COLORREF crText = (COLORREF)-1);
	virtual void GetTaskTextColors(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, COLORREF& crText, COLORREF& crBack, BOOL bAllowInvert = TRUE) const;

	void RecalcSelectedTimeEstimate();
	BOOL SpellcheckItem(HTREEITEM hti, CSpellCheckDlg* pSpellChecker, BOOL bTitle, BOOL bNotifyNoErrors);
	BOOL SpellcheckItem(HTREEITEM hti, CSpellCheckDlg* pSpellChecker);
	BOOL CanSpellcheckComments();

	BOOL GotoFile(const CString& sUrl, BOOL bShellExecute = TRUE);
	BOOL ShowTaskLink(DWORD dwTaskID, const CString& sLink);

	BOOL SelectionHasIncompleteDependencies(CString& sIncomplete) const;
	BOOL TaskHasIncompleteDependencies(HTREEITEM hti, CString& sIncomplete) const;
	BOOL SelectionHasIncompleteSubtasks(BOOL bExcludeRecurring) const;
	int SelectionHasCircularDependencies(CDWordArray& aTaskIDs) const;
	BOOL SelectionHasDates(TDC_DATE nDate, BOOL bAll = FALSE) const;
	int GetSelectionRecurringTaskCount() const;

	void HandleUnsavedComments();
	BOOL UndoLastActionItems(const CArrayUndoElements& aElms);
	void LoadTaskIcons();
	void InitEditPrompts();

	void ShowTaskHasIncompleteDependenciesError(const CString& sIncomplete);
	void ShowTaskHasCircularDependenciesError(const CDWordArray& aTaskIDs) const;

	void FixupParentCompletion(HTREEITEM htiParent);
	void FixupTaskDependencyDates(DWORD dwTaskID, TDC_DATE nDate);
	UINT AdjustTaskDates(DWORD dwTaskID, DWORD dwDependencyID, TDC_DATE nDate);
	
	BOOL CanMoveItem(HTREEITEM hti, TDC_MOVETASK nDirection) const;
	BOOL MoveSelection(TDC_MOVETASK nDirection);
	BOOL MoveSelectionDown();
	BOOL MoveSelectionUp();
	BOOL MoveSelectionLeft(); // up a level
	BOOL MoveSelectionRight(); // down a level
	HTREEITEM MoveItem(HTREEITEM hti, HTREEITEM htiDestParent, HTREEITEM htiDestPrevSibling);

	typedef CMap<DWORD, DWORD, DWORD, DWORD&> CMapID2ID;
	void PrepareTaskIDsForPaste(CTaskFile& tasks, TDC_RESETIDS nResetID) const;
	void BuildTaskIDMapForPaste(CTaskFile& tasks, HTASKITEM hTask, 
						DWORD& dwNextID, CMapID2ID& mapID, TDC_RESETIDS nResetID) const;
	void PrepareTaskIDsForPaste(CTaskFile& tasks, HTASKITEM hTask, const CMapID2ID& mapID) const;
	BOOL PrepareTaskIDsForPaste(CString& sLink, const CMapID2ID& mapID) const;
	void PrepareTaskIDsForPasteAsRef(CTaskFile& tasks) const;

	virtual int GetArchivableTasks(CTaskFile& tasks, BOOL bSelectedOnly = FALSE) const;
	void RemoveArchivedTasks(const CTaskFile& tasks, TDC_ARCHIVE nRemove, BOOL bRemoveFlagged);
	BOOL RemoveArchivedTask(const CTaskFile& tasks, HTASKITEM hTask, TDC_ARCHIVE nRemove, BOOL bRemoveFlagged);
	virtual BOOL RemoveArchivedTask(DWORD dwTaskID);
	BOOL ArchiveTasks(const CString& sArchivePath, const CTaskFile& tasks); // helper to avoud code dupe

	void SearchAndExpand(const SEARCHPARAMS& params, BOOL bExpand);
	void AppendTaskFileHeader(CTaskFile& tasks) const;

	enum TDI_STATE 
	{
		TDIS_NONE,
		TDIS_SELECTED,
		TDIS_SELECTEDNOTFOCUSED,
		TDIS_DROPHILITED,
		TDIS_SELECTEDDROPHILITED,
	};

	TDI_STATE GetTreeItemState(HTREEITEM hti);
	BOOL DrawItemColumn(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, const NCGDRAWITEM* pNCGDI, TDI_STATE nState, DWORD dwRefID);
	void GetItemColors(DWORD dwID, NCGITEMCOLORS* pColors, TDI_STATE nState);
	void DrawCommentsText(CDC* pDC, const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, const CRect& rText, TDI_STATE nState);
	void DrawItemBackColor(CDC* pDC, const LPRECT rect, COLORREF crBack);

	void MultiSortTree(const TDSORTCOLUMNS& sort, HTREEITEM htiRoot = NULL, BOOL bSortChildren = TRUE);
	void SortTree(TDC_COLUMN nBy, BOOL bAscending, HTREEITEM htiRoot = NULL, BOOL bSortChildren = TRUE);
	void SortTreeItem(HTREEITEM hti, const TDSORTPARAMS& ss, BOOL bMulti = FALSE);

	static int CALLBACK CompareFunc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort); 
	static int CALLBACK CompareFuncMulti(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort); 
	static TDC_COLUMN MapSortByToColumn(int nSortBy); // backwards compatibility

	static BOOL WantUpdateInheritedAttibute(TDC_ATTRIBUTE nAttrib);
	void ApplyLastInheritedChangeToSubtasks(DWORD dwID, TDC_ATTRIBUTE nAttrib);

	// used for building/creating the tree for saving/loading
	// not for overriding
	int GetAllTasks(CTaskFile& tasks) const;
	HTREEITEM SetAllTasks(const CTaskFile& tasks);

	virtual HTREEITEM RebuildTree(const void* pContext = NULL);
	virtual BOOL BuildTreeItem(HTREEITEM hti, const TODOSTRUCTURE* pTDS, const void* pContext);
	virtual BOOL WantAddTask(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, const void* pContext) const;
	HTREEITEM InsertTreeItem(const TODOITEM* pTDI, DWORD dwID, HTREEITEM htiParent, HTREEITEM htiAfter);

	int CreateTasksFromOutlookObjects(TLDT_DATA* pData);
	BOOL CreateTaskFromOutlookObject(const CTDCCsvColumnMapping& aMapping, OutlookAPI::_MailItem* pItem, BOOL bWantConfidential, CTaskFile& tasks);

	TDC_ATTRIBUTE GetFocusedControlAttribute() const;
	CString FormatInfoTip(DWORD dwTaskID) const;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TODOCTRL_H__5951FDE6_508A_4A9D_A55D_D16EB026AEF7__INCLUDED_)
