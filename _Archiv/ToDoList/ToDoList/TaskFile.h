// TaskFile.h: interface for the CTaskFile class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TASKFILE_H__BA5D71E7_2770_45FD_A693_A2344B589DF4__INCLUDED_)
#define AFX_TASKFILE_H__BA5D71E7_2770_45FD_A693_A2344B589DF4__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\SHARED\ITaskList.h"

#include <afxtempl.h>

#ifdef NO_TL_ENCRYPTDECRYPT
#	include "..\SHARED\xmlfile.h"
#	define XMLBASE CXmlFile
#else
#	include "..\SHARED\xmlfileex.h"
#	define XMLBASE CXmlFileEx
#endif

// predecs
class CBinaryData;
class TODOITEM;
class CTDCCustomAttribDefinitionArray;
class CTDCCustomAttribDataMap;

struct TASKFILE_HEADER;
struct TDIRECURRENCE; 

typedef CMap<DWORD, DWORD, DWORD, DWORD> CID2IDMap;

class CTaskFile : public ITASKLISTBASE, public XMLBASE
{
	friend class CMultiTaskFile;

public:
	CTaskFile(LPCTSTR szPassword = NULL);
	CTaskFile(const CTaskFile& tasks, LPCTSTR szPassword = NULL);
	CTaskFile(const ITaskList* pTasks, LPCTSTR szPassword = NULL);
	virtual ~CTaskFile();

	BOOL Load(LPCTSTR szFilePath, IXmlParse* pCallback = NULL, BOOL bDecrypt = TRUE);
	BOOL LoadContent(const CString& sContent);

	virtual BOOL LoadEx(IXmlParse* pCallback = NULL);
	virtual BOOL SaveEx();
	virtual BOOL LoadHeader(LPCTSTR szFilePath, TASKFILE_HEADER* pHeader = NULL);

	void SetHeader(const TASKFILE_HEADER& header);
	void GetHeader(TASKFILE_HEADER& header) const;

#ifndef NO_TL_ENCRYPTDECRYPT
	virtual BOOL Decrypt(LPCTSTR szPassword = NULL); 
#endif

	BOOL CopyFrom(const CTaskFile& tasks);
	BOOL CopyFrom(const ITaskList* pTasks);
	BOOL CopyTo(ITaskList* pTasks);
	
	BOOL CopyTaskFrom(const ITaskList* pSrcTasks, HTASKITEM hSrcTask, HTASKITEM hDestParent, 
						BOOL bResetID = FALSE, CID2IDMap* pMapID2ID = NULL);
	BOOL CopyTaskTo(HTASKITEM hSrcTask, ITaskList* pDestTasks, HTASKITEM hDestParent, 
					BOOL bResetID = FALSE, CID2IDMap* pMapID2ID = NULL);

	void Reset();
	int GetTaskCount() const;

#ifndef NO_TL_MERGE
	int Merge(const CTaskFile& tasks, BOOL bByID, BOOL bMoveExist);
	int Merge(LPCTSTR szTaskFilePath, BOOL bByID, BOOL bMoveExist);
#endif

	HTASKITEM NewTask(LPCTSTR szTitle, HTASKITEM hParent, DWORD dwID);

	// Note: we have to leave version above because it implements
	// the equiv method in ITaskList8
	HTASKITEM NewTask(LPCTSTR szTitle, HTASKITEM hParent, DWORD dwID, DWORD dwParentID);

	DWORD GetNextUniqueID() const; 
	BOOL SetNextUniqueID(DWORD dwNextID); 

	BOOL SetCheckedOutTo(const CString& sCheckedOutTo);
	BOOL IsCheckedOutTo(const CString& sCheckedOutTo) const;

	BOOL SetArchive(BOOL bArchive = TRUE);
	BOOL SetFileFormat(unsigned long lFormat);
	BOOL SetCharSet(LPCTSTR szCharSet);
	BOOL SetFileName(LPCTSTR szFilename);

	void SetHtmlImageFolder(LPCTSTR szImgFolder) { m_sHtmlImgFolder = szImgFolder; }
	CString GetHtmlImageFolder() { return m_sHtmlImgFolder; }
	
	BOOL SetCategoryNames(const CStringArray& aCategories);
	int GetCategoryNames(CStringArray& aCategories) const;

	BOOL SetTagNames(const CStringArray& aTags);
	int GetTagNames(CStringArray& aTags) const;

	BOOL SetStatusNames(const CStringArray& aStatuses);
	int GetStatusNames(CStringArray& aStatuses) const;

	BOOL SetAllocToNames(const CStringArray& aAllocTo);
	int GetAllocToNames(CStringArray& aAllocTo) const;

	BOOL SetAllocByNames(const CStringArray& aAllocBy);
	int GetAllocByNames(CStringArray& aAllocBy) const;

	BOOL SetVersionNames(const CStringArray& aVersions);
	int GetVersionNames(CStringArray& aVersions) const;

	BOOL SetMetaData(const CMapStringToString& mapMetaData);
	int GetMetaData(CMapStringToString& mapMetaData) const;

	BOOL SetCustomAttributeDefs(const CTDCCustomAttribDefinitionArray& aAttribDefs);
	int GetCustomAttributeDefs(CTDCCustomAttribDefinitionArray& aAttribDefs) const;

	BOOL SetEarliestDueDate(const COleDateTime& date);
	BOOL GetEarliestDueDate(COleDateTime& date) const;

	CString GetCommentsType() const; 
	void EnableISODates(BOOL bEnable = TRUE) { m_bISODates = bEnable; }

	BOOL CheckOut(LPCTSTR szCheckOutTo, CString& sCheckedOutTo);
	BOOL CheckOut(LPCTSTR szCheckOutTo);

	BOOL SetReportAttributes(LPCTSTR szTitle, const COleDateTime& date = 0.0);
	BOOL HideAttribute(HTASKITEM hTask, LPCTSTR szAttrib, BOOL bHide = TRUE);

	// Task-related methods -----------
	COleDateTime GetTaskLastModifiedOle(HTASKITEM hTask) const;
	COleDateTime GetTaskDoneDateOle(HTASKITEM hTask) const;
	COleDateTime GetTaskDueDateOle(HTASKITEM hTask) const;
	COleDateTime GetTaskStartDateOle(HTASKITEM hTask) const;
	COleDateTime GetTaskCreationDateOle(HTASKITEM hTask) const;

	BOOL SetTaskID(HTASKITEM hTask, unsigned long nID, BOOL bVisible = TRUE);
	BOOL SetTaskReferenceID(HTASKITEM hTask, unsigned long nRefID, BOOL bVisible = TRUE);
	DWORD GetTaskReferenceID(HTASKITEM hTask) const;

	BOOL SetTaskAttributes(HTASKITEM hTask, const TODOITEM* pTDI);
	BOOL GetTaskAttributes(HTASKITEM hTask, TODOITEM* pTDI) const;

	BOOL SetTaskLastModified(HTASKITEM hTask, const COleDateTime& tLastMod);
	BOOL SetTaskDoneDate(HTASKITEM hTask, const COleDateTime& date);
	BOOL SetTaskDueDate(HTASKITEM hTask, const COleDateTime& date);
	BOOL SetTaskStartDate(HTASKITEM hTask, const COleDateTime& date);
	BOOL SetTaskCreationDate(HTASKITEM hTask, const COleDateTime& date);

	BOOL SetTaskRecurrence(HTASKITEM hTask, const TDIRECURRENCE& tr);
	BOOL GetTaskRecurrence(HTASKITEM hTask, TDIRECURRENCE& tr) const;

	BOOL SetTaskTextColor(HTASKITEM hTask, COLORREF color);
	BOOL SetTaskPriorityColor(HTASKITEM hTask, COLORREF color);
	BOOL SetTaskCalcTimeEstimate(HTASKITEM hTask, double dTime, TCHAR cUnits);
	BOOL SetTaskCalcTimeSpent(HTASKITEM hTask, double dTime, TCHAR cUnits);
	BOOL SetTaskCalcDueDate(HTASKITEM hTask, const COleDateTime& date);
	BOOL SetTaskCalcStartDate(HTASKITEM hTask, const COleDateTime& date);
	BOOL SetTaskCalcCompletion(HTASKITEM hTask, int nPercent);
	BOOL SetTaskHighestPriority(HTASKITEM hTask, int nPriority);
	BOOL SetTaskHighestRisk(HTASKITEM hTask, int nRisk);
	BOOL SetTaskCalcCost(HTASKITEM hTask, double dCost);

	BOOL SetTaskCategories(HTASKITEM hTask, const CStringArray& aCategories);
	int  GetTaskCategories(HTASKITEM hTask, CStringArray& aCategories) const;

	BOOL SetTaskTags(HTASKITEM hTask, const CStringArray& aTags);
	int  GetTaskTags(HTASKITEM hTask, CStringArray& aTags) const;

	BOOL SetTaskMetaData(HTASKITEM hTask, const CMapStringToString& mapMetaData);
	int GetTaskMetaData(HTASKITEM hTask, CMapStringToString& mapMetaData) const;

	BOOL SetTaskDependencies(HTASKITEM hTask, const CStringArray& aDepends);
	int  GetTaskDependencies(HTASKITEM hTask, CStringArray& aDepends) const;

	BOOL SetTaskAllocatedTo(HTASKITEM hTask, const CStringArray& aAllocTo);
	int  GetTaskAllocatedTo(HTASKITEM hTask, CStringArray& aAllocTo) const;

	BOOL SetTaskCustomComments(HTASKITEM hTask, const CBinaryData& content, const CString& sType);
	BOOL GetTaskCustomComments(HTASKITEM hTask, CBinaryData& content, CString& sType) const;
	BOOL SetTaskHtmlComments(HTASKITEM hTask, const CString& sContent, BOOL bForTransform);

	int GetTaskCustomAttributeData(HTASKITEM hTask, CMapStringToString& mapData) const;
	BOOL SetTaskCustomAttributeData(HTASKITEM hTask, const CMapStringToString& mapData);
	
	BOOL DeleteTaskAttributes(HTASKITEM hTask);// deletes all but child tasks
	BOOL DeleteTask(HTASKITEM hTask);

	//////////////////////////////////////////////////////////////
	// ITaskList11 implementation 
	time_t GetTaskStartDate(HTASKITEM hTask, BOOL bCalc) const;
	LPCTSTR GetTaskStartDateString(HTASKITEM hTask, BOOL bCalc) const;
	bool SetTaskIcon(HTASKITEM hTask, LPCTSTR szIcon);
	LPCTSTR GetTaskIcon(HTASKITEM hTask) const;

	//////////////////////////////////////////////////////////////
	// ITaskList10 implementation 
	LPCTSTR GetMetaData(LPCTSTR szKey) const;
	bool SetMetaData(LPCTSTR szKey, LPCTSTR szMetaData);
	bool ClearMetaData(LPCTSTR szKey);

	LPCTSTR GetTaskMetaData(HTASKITEM hTask, LPCTSTR szKey) const;
	bool SetTaskMetaData(HTASKITEM hTask, LPCTSTR szKey, LPCTSTR szMetaData);
	bool ClearTaskMetaData(HTASKITEM hTask, LPCTSTR szKey);

	int GetCustomAttributeCount() const;
	LPCTSTR GetCustomAttributeLabel(int nIndex) const;
	LPCTSTR GetCustomAttributeID(int nIndex) const;
	LPCTSTR GetCustomAttributeValue(int nIndex, LPCTSTR szItem) const;

	LPCTSTR GetTaskCustomAttributeData(HTASKITEM hTask, LPCTSTR szID) const;
	bool SetTaskCustomAttributeData(HTASKITEM hTask, LPCTSTR szID, LPCTSTR szData);
	bool ClearTaskCustomAttributeData(HTASKITEM hTask, LPCTSTR szID);

	//////////////////////////////////////////////////////////////
	// ITaskList9 implementation 
	int GetTaskTagCount(HTASKITEM hTask) const;
	LPCTSTR GetTaskTag(HTASKITEM hTask, int nIndex) const;
	bool AddTaskTag(HTASKITEM hTask, LPCTSTR szTag);

	LPCTSTR GetTaskPositionString(HTASKITEM hTask) const;
	bool SetTaskPosition(HTASKITEM hTask, LPCTSTR szPos);

	//////////////////////////////////////////////////////////////
	// ITaskList8 implementation 
	unsigned long GetTaskParentID(HTASKITEM hTask) const;
	HTASKITEM FindTask(unsigned long dwTaskID) const;
 	bool SetTaskAttribute(HTASKITEM hTask, LPCTSTR szAttrib, LPCTSTR szValue);

	//////////////////////////////////////////////////////////////
	// ITaskList7 implementation 
	int GetTaskDependencyCount(HTASKITEM hTask) const;
	bool AddTaskDependency(HTASKITEM hTask, LPCTSTR szDepends);
	bool AddTaskDependency(HTASKITEM hTask, unsigned long dwID);
	LPCTSTR GetTaskDependency(HTASKITEM hTask, int nIndex) const;

	int GetTaskAllocatedToCount(HTASKITEM hTask) const;
	bool AddTaskAllocatedTo(HTASKITEM hTask, LPCTSTR szAllocTo);
	LPCTSTR GetTaskAllocatedTo(HTASKITEM hTask, int nIndex) const;

	//////////////////////////////////////////////////////////////
	// ITaskList6 implementation 
	LPCTSTR GetTaskVersion(HTASKITEM hTask) const;
	bool GetTaskRecurrence(HTASKITEM hTask, int& nRegularity, DWORD& dwSpecific1, 
									DWORD& dwSpecific2, BOOL& bRecalcFromDue, int& nReuse) const;

	bool SetTaskVersion(HTASKITEM hTask, LPCTSTR szVersion);
	bool SetTaskRecurrence(HTASKITEM hTask, int nRegularity, DWORD dwSpecific1, 
									DWORD dwSpecific2, BOOL bRecalcFromDue, int nReuse);

	//////////////////////////////////////////////////////////////
	// ITaskList5 implementation 
	bool AddTaskCategory(HTASKITEM hTask, LPCTSTR szCategory);

	//////////////////////////////////////////////////////////////
	// ITaskList4 implementation 
	LPCTSTR GetAttribute(LPCTSTR szAttrib) const;

	LPCTSTR GetHtmlCharSet() const;
	LPCTSTR GetReportTitle() const;
	LPCTSTR GetReportDate() const;
	double GetTaskCost(HTASKITEM hTask, BOOL bCalc) const;
	int GetTaskCategoryCount(HTASKITEM hTask) const;
	LPCTSTR GetTaskCategory(HTASKITEM hTask, int nIndex) const;
	LPCTSTR GetTaskDependency(HTASKITEM hTask) const;

	bool SetTaskCost(HTASKITEM hTask, double dCost);
	bool SetTaskDependency(HTASKITEM hTask, LPCTSTR szDepends);

	//////////////////////////////////////////////////////////////
	// ITaskList3 implementation 
	time_t GetTaskDueDate(HTASKITEM hTask, BOOL bCalc) const;
	LPCTSTR GetTaskDueDateString(HTASKITEM hTask, BOOL bCalc) const;
	unsigned long GetTaskTextColor(HTASKITEM hTask) const;
	int GetTaskRisk(HTASKITEM hTask, BOOL bHighest) const;
	LPCTSTR GetTaskExternalID(HTASKITEM hTask) const;

	bool SetTaskRisk(HTASKITEM hTask, int nRisk);
	bool SetTaskExternalID(HTASKITEM hTask, LPCTSTR szID);

	//////////////////////////////////////////////////////////////
	// ITaskList2 implementation 
	
	LPCTSTR GetTaskCreatedBy(HTASKITEM hTask) const;
	time_t GetTaskCreationDate(HTASKITEM hTask) const;
	LPCTSTR GetTaskCreationDateString(HTASKITEM hTask) const;

	bool SetTaskCreatedBy(HTASKITEM hTask, LPCTSTR szCreatedBy);
	bool SetTaskCreationDate(HTASKITEM hTask, time_t tCreationDate);

	//////////////////////////////////////////////////////////////
	// ITaskList implementation 

	bool IsArchive() const;
	
	LPCTSTR GetProjectName() const;

	bool IsSourceControlled() const;
	LPCTSTR GetCheckOutTo() const;
	bool IsCheckedOut() const { ASSERT(0); return false; } // Deprecated
	
	unsigned long GetFileFormat() const;
	unsigned long GetFileVersion() const;
	
	time_t GetLastModified() const { ASSERT(0); return 0; } // Deprecated

	bool SetProjectName(LPCTSTR szName);
	bool SetFileVersion(unsigned long nVersion);

	HTASKITEM NewTask(LPCTSTR szTitle, HTASKITEM hParent = NULL);

	HTASKITEM GetFirstTask(HTASKITEM hParent = NULL) const;
	HTASKITEM GetNextTask(HTASKITEM hTask) const;

	LPCTSTR GetTaskTitle(HTASKITEM hTask) const;
	LPCTSTR GetTaskComments(HTASKITEM hTask) const;
	LPCTSTR GetTaskAllocatedTo(HTASKITEM hTask) const;
	LPCTSTR GetTaskAllocatedBy(HTASKITEM hTask) const;
	LPCTSTR GetTaskCategory(HTASKITEM hTask) const;
	LPCTSTR GetTaskStatus(HTASKITEM hTask) const;
	LPCTSTR GetTaskFileReferencePath(HTASKITEM hTask) const;
	LPCTSTR GetTaskWebColor(HTASKITEM hTask) const;
	LPCTSTR GetTaskPriorityWebColor(HTASKITEM hTask) const;

	unsigned long GetTaskID(HTASKITEM hTask) const;
	unsigned long GetTaskColor(HTASKITEM hTask) const;
	unsigned long GetTaskPriorityColor(HTASKITEM hTask) const;
	unsigned long GetTaskPosition(HTASKITEM hTask) const;

	int GetTaskPriority(HTASKITEM hTask, BOOL bHighest) const;
	unsigned char GetTaskPercentDone(HTASKITEM hTask, BOOL bCalc) const;

	double GetTaskTimeEstimate(HTASKITEM hTask, TCHAR& cUnits, BOOL bCalc) const;
	double GetTaskTimeSpent(HTASKITEM hTask, TCHAR& cUnits, BOOL bCalc) const;

	time_t GetTaskLastModified(HTASKITEM hTask) const;
	time_t GetTaskDoneDate(HTASKITEM hTask) const;
	time_t GetTaskDueDate(HTASKITEM hTask) const;
	time_t GetTaskStartDate(HTASKITEM hTask) const; // deprecated

	LPCTSTR GetTaskDoneDateString(HTASKITEM hTask) const;
	LPCTSTR GetTaskDueDateString(HTASKITEM hTask) const;
	LPCTSTR GetTaskStartDateString(HTASKITEM hTask) const;
	
	bool IsTaskDone(HTASKITEM hTask) const;
	bool IsTaskDue(HTASKITEM hTask) const;
	bool IsTaskFlagged(HTASKITEM hTask) const;

	bool TaskHasAttribute(HTASKITEM hTask, LPCTSTR szAttrib) const;
	LPCTSTR GetTaskAttribute(HTASKITEM hTask, LPCTSTR szAttrib) const;
	HTASKITEM GetTaskParent(HTASKITEM hTask) const;

	bool SetTaskTitle(HTASKITEM hTask, LPCTSTR szTitle);
	bool SetTaskComments(HTASKITEM hTask, LPCTSTR szComments);
	bool SetTaskAllocatedTo(HTASKITEM hTask, LPCTSTR szAllocTo);
	bool SetTaskAllocatedBy(HTASKITEM hTask, LPCTSTR szAllocBy);
	bool SetTaskCategory(HTASKITEM hTask, LPCTSTR szCategory);
	bool SetTaskStatus(HTASKITEM hTask, LPCTSTR szStatus);
	bool SetTaskFileReferencePath(HTASKITEM hTask, LPCTSTR szFileRefpath);

	bool SetTaskColor(HTASKITEM hTask, unsigned long nColor);
	bool SetTaskWebColor(HTASKITEM hTask, unsigned long nColor);

	bool SetTaskPriority(HTASKITEM hTask, int nPriority);
	bool SetTaskPercentDone(HTASKITEM hTask, unsigned char nPercent);

	bool SetTaskTimeEstimate(HTASKITEM hTask, double dTimeEst, TCHAR cUnits);
	bool SetTaskTimeSpent(HTASKITEM hTask, double dTimeSpent, TCHAR cUnits);

	bool SetTaskLastModified(HTASKITEM hTask, time_t tLastMod);
	bool SetTaskDoneDate(HTASKITEM hTask, time_t tDoneDate);
	bool SetTaskDueDate(HTASKITEM hTask, time_t tDueDate);
	bool SetTaskStartDate(HTASKITEM hTask, time_t tStartDate);

	bool SetTaskPosition(HTASKITEM hTask, unsigned long nPos);
	bool SetTaskFlag(HTASKITEM hTask, bool bFlag);

	/////////////////////////////////////////////////////
	// IUnknown implementation

	HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void __RPC_FAR *__RPC_FAR *ppvObject);
    ULONG STDMETHODCALLTYPE AddRef(void) { return 1; } // do nothing
    ULONG STDMETHODCALLTYPE Release( void) { return 1; } // do nothing

	/////////////////////////////////////////////////////
	
protected:
	DWORD m_dwNextUniqueID;
	BOOL m_bISODates;
	CString m_sHtmlImgFolder;

	mutable CMap <HTASKITEM, HTASKITEM, CXmlItem*, CXmlItem*&> m_mapHandles;

protected:
	void BuildHandleMap() const;
	void ClearHandleMap() const;
	void AddTaskToMap(const CXmlItem* pXITask, BOOL bRecurse) const;
	void RemoveTaskFromMap(const CXmlItem* pXITask) const;
	CXmlItem* TaskFromHandle(HTASKITEM hTask) const;

	void UpgradeArrays(HTASKITEM hTask = NULL);

	virtual CXmlItem* NewItem(const CString& sName = _T(""));
	
	double GetTaskTime(HTASKITEM hTask, const CString& sTimeItem) const;
	time_t GetTaskDate(HTASKITEM hTask, const CString& sDateItem, BOOL bIncTime) const;
	COleDateTime GetTaskDateOle(HTASKITEM hTask, const CString& sDateItem, BOOL bIncTime) const;
	unsigned char GetTaskUChar(HTASKITEM hTask, const CString& sUCharItem) const;
	unsigned long GetTaskULong(HTASKITEM hTask, const CString& sULongItem) const;
	int GetTaskInt(HTASKITEM hTask, const CString& sIntItem) const;
	CString GetTaskString(HTASKITEM hTask, const CString& sStringItem) const;
	double GetTaskDouble(HTASKITEM hTask, const CString& sDoubleItem) const;
	TCHAR GetTaskTimeUnits(HTASKITEM hTask, const CString& sUnitsItem) const;
	
	CString GetTaskAttribute(HTASKITEM hTask, const CString& sAttrib, const CString& sKey) const;
	bool SetTaskAttribute(HTASKITEM hTask, const CString& sAttrib, const CString& sKey, const CString& sValue);
	bool DeleteTaskAttribute(HTASKITEM hTask, const CString& sAttrib, const CString& sKey);
	BOOL IsCustomDateAttribute(const CString& sTypeID) const;

	bool SetTaskDate(HTASKITEM hTask, const CString& sDateItem, time_t tVal, BOOL bIncTime);
	bool SetTaskDate(HTASKITEM hTask, const CString& sDateItem, const COleDateTime& tVal, BOOL bIncTime, const CString& sDateStringItem = _T(""));
	bool SetTaskUChar(HTASKITEM hTask, const CString& sUCharItem, unsigned char cVal);
	bool SetTaskULong(HTASKITEM hTask, const CString& sULongItem, unsigned long lVal);
	bool SetTaskInt(HTASKITEM hTask, const CString& sIntItem, int iVal);
	bool SetTaskString(HTASKITEM hTask, const CString& sStringItem, const CString& sVal, XI_TYPE nType = XIT_ATTRIB);
	bool SetTaskDouble(HTASKITEM hTask, const CString& sDoubleItem, double dVal);
	bool SetTaskTime(HTASKITEM hTask, const CString& sTimeItem, double dTime,
					 const CString& sUnitsItem, TCHAR cUnits);

	// for handling arrays at *task* level
	bool AddTaskArrayItem(HTASKITEM hTask, const CString& sItemTag, const CString& sItem);
	CString GetTaskArrayItem(HTASKITEM hTask, const CString& sItemTag, int nIndex) const;
	BOOL SetTaskArray(HTASKITEM hTask, const CString& sItemTag, const CStringArray& aItems);
	int GetTaskArray(HTASKITEM hTask, const CString& sItemTag, CStringArray& aItems) const;
	bool DeleteTaskArray(HTASKITEM hTask, const CString& sItemTag);
	int GetTaskArraySize(HTASKITEM hTask, const CString& sItemTag) const;

	// for handling arrays at *tasklist* level
	BOOL SetArray(const CString& sItemTag, const CStringArray& aItems);
	int GetArray(const CString& sItemTag, CStringArray& aItems) const;

	/////////////////////////////////////////////////////////////////////////////////////
	// legacy support for reading 'old' arrays
	bool LegacyDeleteArray(const CString& sItemTag);
	bool LegacyDeleteTaskArray(HTASKITEM hTask, const CString& sNumItemTag, const CString& sItemTag);
	int LegacyGetArray(const CString& sItemTag, CStringArray& aItems) const;
	int LegacyGetTaskArray(HTASKITEM hTask, const CString& sNumItemTag, 
				  	 const CString& sItemTag, CStringArray& aItems) const;
	/////////////////////////////////////////////////////////////////////////////////////

	// helpers
	static BOOL CopyTask(const ITaskList* pSrcTasks, HTASKITEM hSrcTask, 
						ITaskList* pDestTasks, HTASKITEM hDestParent, 
						BOOL bResetID = FALSE, CID2IDMap* pMapID2ID = NULL);

	static CString GetWebColor(COLORREF color);
	static BOOL SetMetaData(CXmlItem* pXItem, const CMapStringToString& mapMetaData);
	static int GetMetaData(const CXmlItem* pXItem, CMapStringToString& mapMetaData);

};

#endif // !defined(AFX_TASKFILE_H__BA5D71E7_2770_45FD_A693_A2344B589DF4__INCLUDED_)
