//Written for the ToDoList (http://www.codeproject.com/KB/applications/todolist2.aspx)
//Design and latest version - http://www.codeproject.com/KB/applications/TDL_Calendar.aspx
//by demon.code.monkey@googlemail.com

#ifndef _CALENDARDATA_H_
#define _CALENDARDATA_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\..\shared\iuiExtension.h"
#include "..\..\ToDoList\TaskFile.h"

#include "TaskInfo.h"

typedef CMap <HTASKITEM, HTASKITEM&, LPVOID, LPVOID> CMapTimeToTaskItemList;

class CCalendarData
{
public:
	CCalendarData();
	~CCalendarData();

	BOOL	IsImportantDate(const COleDateTime& _dt) const;
	void	GetTasks(const COleDateTime& _dt, CTaskInfoList& _listTasks) const;

	BOOL	GetTaskTitle(DWORD _dwItemID, CString& _strTitle) const;
	BOOL	GetTaskStartDate(DWORD _dwItemID, COleDateTime& _date) const;
	BOOL	GetTaskDueDate(DWORD _dwItemID, COleDateTime& _date) const;
	BOOL	GetTaskDoneDate(DWORD _dwItemID, COleDateTime& _date) const;
	BOOL	GetTaskTimeEstimate(DWORD _dwItemID, double& _dTimeSpent, TCHAR& _cUnit) const;
	BOOL	GetTaskTimeSpent(DWORD _dwItemID, double& _dTimeSpent, TCHAR& _cUnit) const;

	CString GetTaskParentsString(DWORD _dwItemID) const;
	CString GetTaskParentsString(HTASKITEM _hCurrentTask, int& _nNumParents) const;

	BOOL	TasklistUpdated(const ITaskList* _pTasks, IUI_UPDATETYPE nUpdate, int nEditAttribute);
	void	CalculateImportantDates(HTASKITEM _hParentTask=NULL);

// 	static void		ReadValueFromIni(const CString& _strKey, CString& _strValue, LPCTSTR _pszDefault=NULL);
// 	static void		ReadValueFromIni(const CString& _strKey, DWORD& _dwValue, DWORD _dwDefault=0);
// 	static void		ReadValueFromIni(const CString& _strKey, int& _nValue, int _nDefault=0);
// 	static void		ReadValueFromIni(const CString& _strKey, CRect& _rcValue, const CRect& _rcDefault=CRect(0,0,0,0));
// 
// 	static void		WriteValueToIni(const CString& _strKey, const CString& _strValue);
// 	static void		WriteValueToIni(const CString& _strKey, DWORD _dwValue);
// 	static void		WriteValueToIni(const CString& _strKey, const CRect& _rcValue);
// 
// 	static CRect	StringToRect(const CString& _strRect);
// 	static CString	RectToString(const CRect& _rcRect);

protected:
	void	ClearImportantDateMap();
	BOOL	UpdateTasks(const ITaskList* _pTasks, HTASKITEM _hCurrentTask, int nEditAttribute);

#ifdef _DEBUG
	void	DumpTasks(HTASKITEM _hParentTask=NULL);
#endif

//variables
protected:
	int			m_nTaskID;
	CTaskFile	m_tasks;
	CMap <time_t, time_t, CMapTimeToTaskItemList*, CMapTimeToTaskItemList*&> m_mapDateToHTASKITEMs;
};


#endif//_CALENDARDATA_H_
