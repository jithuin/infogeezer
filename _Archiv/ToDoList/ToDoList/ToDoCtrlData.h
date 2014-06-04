// ToDoCtrlData.h: interface for the CToDoCtrlData class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TODOCTRLDATA_H__02C3C360_45AB_45DC_B1BF_BCBEA472F0C7__INCLUDED_)
#define AFX_TODOCTRLDATA_H__02C3C360_45AB_45DC_B1BF_BCBEA472F0C7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "tdcstruct.h"
#include "tdcenum.h"
#include "todoctrlundo.h"
#include "todoitem.h"
#include "taskfile.h"
#include <afxtempl.h>

class CToDoCtrlData;
class CBinaryData;

typedef CMap<DWORD, DWORD, TODOITEM*, TODOITEM*&> CTDIMap;

// class to help start and end undo actions
class CUndoAction
{
public:
	CUndoAction(CToDoCtrlData& data, TDCUNDOACTIONTYPE nType = TDCUAT_EDIT);
	~CUndoAction();

protected:
	CToDoCtrlData& m_data;
	BOOL m_bSuccess;
};

class CToDoCtrlData  
{
public:
	CToDoCtrlData(const CWordArray& aStyles);
	virtual ~CToDoCtrlData();

	int BuildDataModel(const CTaskFile& tasks);
	
	inline UINT GetTaskCount() const { return m_mapID2TDI.GetCount(); }
	
	TODOITEM* NewTask(const TODOITEM* pTDIRef = NULL) const;
	TODOITEM* NewTask(const CTaskFile& tasks, HTASKITEM hTask) const;

	void AddTask(DWORD dwTaskID, TODOITEM* pTDI, DWORD dwParentID, DWORD dwPrevSiblingID);
	BOOL DeleteTask(DWORD dwTaskID);
	void DeleteAllTasks();

	BOOL LocateTask(DWORD dwTaskID, TODOSTRUCTURE*& pTDSParent, int& nPos) const;
	TODOSTRUCTURE* LocateTask(DWORD dwTaskID) const;
	const TODOSTRUCTURE* GetStructure() const { return &m_struct; }

	TODOITEM* GetTask(DWORD& dwTaskID, BOOL bTrue = TRUE) const;
	BOOL GetTask(DWORD& dwTaskID, const TODOITEM*& pTDI, const TODOSTRUCTURE*& pTDS, BOOL bTrue = TRUE) const;

	BOOL CanMoveTask(DWORD dwTaskID, DWORD dwDestParentID) const;
	BOOL MoveTask(DWORD dwTaskID, DWORD dwDestParentID, DWORD dwDestPrevSiblingID);
	BOOL MoveTasks(const CDWordArray& aTaskIDs, DWORD dwDestParentID, DWORD dwDestPrevSiblingID);

	// undo/redo
	BOOL BeginNewUndoAction(TDCUNDOACTIONTYPE nType);
	BOOL EndCurrentUndoAction();
	BOOL UndoLastAction(BOOL bUndo, CArrayUndoElements& aElms);
	BOOL CanUndoLastAction(BOOL bUndo) const;
	int GetLastUndoActionTaskIDs(BOOL bUndo, CDWordArray& aIDs) const;
	TDCUNDOACTIONTYPE GetLastUndoActionType(BOOL bUndo) const;
	BOOL DeleteLastUndoAction();

	// use only when this task does not provide a Set method eg. moving tasks
	void ClearUndo() { m_undo.ResetAll(); }

	double GetEarliestDueDate() const;
	
	// Gets
	DWORD GetTaskParentID(DWORD dwTaskID) const;
	CString GetTaskTitle(DWORD dwTaskID) const;
	CString GetTaskIcon(DWORD dwTaskID) const;
	COleDateTime GetTaskDate(DWORD dwTaskID, TDC_DATE nDate) const;
	BOOL TaskHasDate(DWORD dwTaskID, TDC_DATE nDate) const;
	COLORREF GetTaskColor(DWORD dwTaskID) const; // -1 on no item selected
	CString GetTaskComments(DWORD dwTaskID) const;
	const CBinaryData& GetTaskCustomComments(DWORD dwTaskID, CString& sCommentsTypeID) const;
	int GetTaskPercent(DWORD dwTaskID, BOOL bCheckIfDone) const;
	double GetTaskTimeEstimate(DWORD dwTaskID, TCHAR& nUnits) const;
	double GetTaskTimeSpent(DWORD dwTaskID, TCHAR& nUnits) const;
	double GetTaskCost(DWORD dwTaskID) const;
	CString GetTaskAllocBy(DWORD dwTaskID) const;
	CString GetTaskCreatedBy(DWORD dwTaskID) const;
	CString GetTaskStatus(DWORD dwTaskID) const;
	int GetTaskAllocTo(DWORD dwTaskID, CStringArray& aAllocTo) const;
	int GetTaskCategories(DWORD dwTaskID, CStringArray& aCategories) const;
	int GetTaskTags(DWORD dwTaskID, CStringArray& aTags) const;
	int GetTaskDependencies(DWORD dwTaskID, CStringArray& aDepends) const;
	CString GetTaskFileRef(DWORD dwTaskID) const;
	CString GetTaskExtID(DWORD dwTaskID) const;
	int GetTaskPriority(DWORD dwTaskID) const;
	int GetTaskRisk(DWORD dwTaskID) const;
	BOOL IsTaskFlagged(DWORD dwTaskID) const;
	BOOL GetTaskRecurrence(DWORD dwTaskID, TDIRECURRENCE& tr) const;
	BOOL GetTaskNextOccurrence(DWORD dwTaskID, COleDateTime& dtNext, BOOL& bDue) const;
	BOOL IsTaskRecurring(DWORD dwTaskID) const;
	CString GetTaskVersion(DWORD dwTaskID) const;
	CString GetTaskPath(DWORD dwTaskID, int nMaxLen = -1) const; 
	CString GetTaskPosition(DWORD dwTaskID) const; 
	CString GetTaskCustomAttributeData(DWORD dwTaskID, const CString& sAttribID) const;

	DWORD GetTrueTaskID(DWORD dwTaskID) const;
	BOOL IsTaskReference(DWORD dwTaskID) const;
	DWORD GetTaskReferenceID(DWORD dwTaskID) const;
	BOOL RemoveOrphanTaskReferences() { return RemoveOrphanTaskReferences(&m_struct); }
	BOOL TaskHasReferences(DWORD dwTaskID) const;
	int GetReferencesToTask(DWORD dwTaskID, CDWordArray& aRefIDs) const;

	int GetTaskDependents(DWORD dwTaskID, CDWordArray& aDependents) const;
	BOOL TaskHasCircularDependencies(DWORD dwTaskID) const;
	BOOL FindDependency(DWORD dwTaskID, DWORD dwDependsID) const;

	BOOL IsTaskDone(DWORD dwTaskID, DWORD dwExtraCheck = TDCCHECKNONE) const;
	BOOL IsTaskStarted(DWORD dwTaskID, BOOL bToday = FALSE) const;
	BOOL IsTaskDue(DWORD dwTaskID, BOOL bToday = FALSE) const;
	BOOL IsTaskOverDue(DWORD dwTaskID) const;
	double CalcDueDate(DWORD dwTaskID) const;
	double CalcStartDate(DWORD dwTaskID) const;
	int GetHighestPriority(DWORD dwTaskID, BOOL bIncludeDue = TRUE) const;
	int GetHighestRisk(DWORD dwTaskID) const;
	int CalcPercentDone(DWORD dwTaskID) const;
	double CalcCost(DWORD dwTaskID) const;
	double CalcTimeEstimate(DWORD dwTaskID, int nUnits) const;
	double CalcTimeSpent(DWORD dwTaskID, int nUnits) const;
	double CalcRemainingTime(DWORD dwTaskID, int& nUnits) const;
	BOOL GetSubtaskTotals(DWORD dwTaskID, int& nSubtasksTotal, int& nSubtasksDone) const;
	BOOL TaskHasIncompleteSubtasks(DWORD dwTaskID, BOOL bExcludeRecurring) const;
	CString FormatTaskAllocTo(DWORD dwTaskID) const;
	CString FormatTaskCategories(DWORD dwTaskID) const;
	CString FormatTaskTags(DWORD dwTaskID) const;

	BOOL IsTaskStarted(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, BOOL bToday = FALSE) const;
	BOOL IsTaskDue(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, BOOL bToday = FALSE) const;
	BOOL IsTaskOverDue(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	double CalcDueDate(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	double CalcStartDate(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	int GetHighestPriority(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, BOOL bIncludeDue = TRUE) const;
	int GetHighestRisk(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	double CalcCost(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	double CalcTimeEstimate(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, int nUnits) const;
	double CalcRemainingTime(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, int& nUnits) const;
	TCHAR GetBestCalcTimeEstUnits(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	double CalcTimeSpent(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, int nUnits) const;
	TCHAR GetBestCalcTimeSpentUnits(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	int CalcPercentDone(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	int CalcPercentFromTime(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const; // spent / estimate
	BOOL GetSubtaskTotals(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, 
							int& nSubtasksTotal, int& nSubtasksDone) const;
	BOOL TaskHasIncompleteSubtasks(const TODOSTRUCTURE* pTDS, BOOL bExcludeRecurring) const;

	BOOL IsTaskDone(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, DWORD dwExtraCheck) const;
	CString GetTaskPath(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	CString GetTaskPosition(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	CString FormatTaskAllocTo(const TODOITEM* pTDI) const;
	CString FormatTaskCategories(const TODOITEM* pTDI) const;
	CString FormatTaskTags(const TODOITEM* pTDI) const;

	// Sets. 0 = failed, 1 = success, -1 = success (no change)
	int SetTaskDate(DWORD dwTaskID, TDC_DATE nDate, const COleDateTime& date);
	int SetTaskColor(DWORD dwTaskID, COLORREF color);
	int SetTaskIcon(DWORD dwTaskID, const CString& sIcon);
	int ClearTaskColor(DWORD dwTaskID) { SetTaskColor(dwTaskID, (COLORREF)-1); }
	int SetTaskComments(DWORD dwTaskID, const CString& sComments, const CBinaryData& customComments = _T(""), const CString& sCommentsTypeID = _T(""));
	int SetTaskCommentsType(DWORD dwTaskID, const CString& sCommentsTypeID);
	int SetTaskPercent(DWORD dwTaskID, int nPercent);
	int SetTaskTimeEstimate(DWORD dwTaskID, double dTime, TCHAR nUnits = TDITU_HOURS);
	int SetTaskTimeSpent(DWORD dwTaskID, double dTime, TCHAR nUnits = TDITU_HOURS);
	int ResetTaskTimeSpent(DWORD dwTaskID, BOOL bAndChildren = FALSE);
	int SetTaskCost(DWORD dwTaskID, double dCost);
	int SetTaskAllocTo(DWORD dwTaskID, const CStringArray& aAllocTo);
	int SetTaskAllocBy(DWORD dwTaskID, const CString& sAllocBy);
	int SetTaskStatus(DWORD dwTaskID, const CString& sStatus);
	int SetTaskCategories(DWORD dwTaskID, const CStringArray& aCategories);
	int SetTaskTags(DWORD dwTaskID, const CStringArray& aTags);
	int SetTaskDependencies(DWORD dwTaskID, const CStringArray& aDepends);
	int SetTaskExtID(DWORD dwTaskID, const CString& sID);
	int SetTaskFileRef(DWORD dwTaskID, const CString& sFilePath);
	int SetTaskPriority(DWORD dwTaskID, int nPriority); // 0-10 (10 is highest)
	int SetTaskRisk(DWORD dwTaskID, int nRisk); // 0-10 (10 is highest)
	int SetTaskTitle(DWORD dwTaskID, const CString& sTitle);
	int SetTaskFlag(DWORD dwTaskID, BOOL bFlagged);
	int SetTaskRecurrence(DWORD dwTaskID, const TDIRECURRENCE& tr);
	int SetTaskVersion(DWORD dwTaskID, const CString& sVersion);
	int OffsetTaskDate(DWORD dwTaskID, TDC_DATE nDate, int nAmount, int nUnits, BOOL bAndSubtasks, BOOL bForceWeekday);
	int SetTaskCustomAttributeData(DWORD dwTaskID, const CString& sAttribID, const CString& sData);

	int CopyTaskAttributes(DWORD dwToTaskID, DWORD dwFromTaskID, const CTDCAttributeArray& aAttribs);
	int CopyTaskAttributes(TODOITEM* pToTDI, DWORD dwFromTaskID, const CTDCAttributeArray& aAttribs);
	
	BOOL TaskMatches(DWORD dwTaskID, const SEARCHPARAMS& params, SEARCHRESULT& result) const;
	BOOL TaskMatches(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, const SEARCHPARAMS& params, SEARCHRESULT& result) const;
	
	BOOL IsTaskTimeTrackable(DWORD dwTaskID) const;
	BOOL IsParentTaskDone(DWORD dwTaskID) const;
	BOOL IsParentTaskDone(const TODOSTRUCTURE* pTDS) const;

	int AreChildTasksDone(DWORD dwTaskID) const;
	int AreChildTasksDone(const TODOSTRUCTURE* pTDS) const;

	int ClearTaskAttribute(DWORD dwTaskID, TDC_ATTRIBUTE nAttrib);
	int ClearTaskCustomAttribute(DWORD dwTaskID, const CString& sAttribID);

	BOOL ApplyLastChangeToSubtasks(DWORD dwTaskID, TDC_ATTRIBUTE nAttrib, BOOL bIncludeBlank = TRUE);
	void ResetCachedCalculations(TDC_ATTRIBUTE nAttrib = TDCA_ALL) const;
	void ResetCachedCalculations(DWORD dwTaskID, TDC_ATTRIBUTE nAttrib = TDCA_ALL) const;
	void ResetCachedCalculations(DWORD dwTaskID, TDC_ATTRIBUTE nAttrib, BOOL bAndParents, BOOL bAndSubtasks) const;

	static CString MapTimeUnits(int nUnits);

	int CompareTasks(DWORD dwTask1ID, DWORD dwTask2ID, TDC_COLUMN nSortBy, BOOL bAscending, BOOL bSortDueTodayHigh) const;
	int CompareTasks(DWORD dwTask1ID, DWORD dwTask2ID, const CString& sCustomAttribID, BOOL bAscending) const;

	int FindTasks(const SEARCHPARAMS& params, CResultArray& aResults) const;
	int FindTasks(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, const SEARCHPARAMS& params, CResultArray& aResults) const;
	inline BOOL HasStyle(int nStyle) const { return m_aStyles[nStyle] ? TRUE : FALSE; }

	void SetDefaultCommentsFormat(const CString& format);
	void SetDefaultTimeUnits(TCHAR nTimeEstUnits, TCHAR nTimeSpentUnits);

protected:
	CTDIMap m_mapID2TDI; // the real data
	const CWordArray& m_aStyles; // CToDoCtrl styles
	CToDoCtrlUndo m_undo;
	CToDoCtrlStructure m_struct;
	CString m_cfDefault;
	TCHAR m_nDefTimeEstUnits, m_nDefTimeSpentUnits;

protected:
	BOOL DeleteTask(TODOSTRUCTURE* pTDSParent, int nPos, BOOL bWithUndo = TRUE);
	BOOL AddTaskToDataModel(const CTaskFile& tasks, HTASKITEM hTask, TODOSTRUCTURE* pTDSParent);
	BOOL RemoveOrphanTaskReferences(TODOSTRUCTURE* pTDSParent);
	int GetReferencesToTask(DWORD dwTaskID, const TODOSTRUCTURE* pTDS, CDWordArray& aRefIDs) const;
	BOOL TaskHasReferences(DWORD dwTaskID, const TODOSTRUCTURE* pTDS) const;

	int FindDependency(DWORD dwTaskID, DWORD dwDependsID, CID2IDMap& mapVisited) const;

	BOOL AddUndoElement(TDCUNDOELMOP nOp, DWORD dwTaskID, DWORD dwParentID = 0, 
						DWORD dwPrevSiblingID = 0, WORD wFlags = 0);
	
	BOOL TaskMatches(const COleDateTime& date, const SEARCHPARAM& sp, SEARCHRESULT& result) const;
	BOOL TaskMatches(const CString& sText, const SEARCHPARAM& sp, SEARCHRESULT& result, BOOL bMatchAsArray = FALSE) const;
	BOOL TaskMatches(double dValue, const SEARCHPARAM& sp, SEARCHRESULT& result) const;
	BOOL TaskMatches(int nValue, const SEARCHPARAM& sp, SEARCHRESULT& result) const;
	BOOL TaskMatches(const CStringArray& aItems, const SEARCHPARAM& sp, SEARCHRESULT& result) const;
	BOOL TaskMatches(const TDCCADATA& data, DWORD dwAttribType, const SEARCHPARAM& sp, SEARCHRESULT& result) const;
	BOOL TaskCommentsMatch(const TODOITEM* pTDI, const SEARCHPARAM& sp, SEARCHRESULT& result) const;

	double SumPercentDone(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	double SumWeightedPercentDone(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS) const;
	
	double CalcStartDueDate(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, BOOL bCheckChildren, BOOL bDue, BOOL bEarliest) const;

	int GetTaskLeafCount(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, BOOL bIncludeDone) const;
	TODOITEM* GetTask(const TODOSTRUCTURE* pTDS) const;

	BOOL ApplyLastChangeToSubtasks(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, TDC_ATTRIBUTE nAttrib, BOOL bIncludeBlank);

	BOOL Locate(DWORD dwParentID, DWORD dwPrevSiblingID, TODOSTRUCTURE*& pTDSParent, int& nPos) const;
	int MoveTask(TODOSTRUCTURE* pTDSSrcParent, int nSrcPos, DWORD dwSrcPrevSiblingID,
							 TODOSTRUCTURE* pTDSDestParent, int nDestPos);
	
	static TDC_ATTRIBUTE MapDateToAttribute(TDC_DATE nDate);

	static int Compare(const COleDateTime& date1, const COleDateTime& date2);
	static int Compare(const CString& sText1, const CString& sText2, BOOL bCheckEmpty = FALSE);
	static int Compare(int nNum1, int nNum2);
	static int Compare(double dNum1, double dNum2);

	static double GetBestDate(double dBest, double dDate, BOOL bEarliest);
};

#endif // !defined(AFX_TODOCTRLDATA_H__02C3C360_45AB_45DC_B1BF_BCBEA472F0C7__INCLUDED_)
