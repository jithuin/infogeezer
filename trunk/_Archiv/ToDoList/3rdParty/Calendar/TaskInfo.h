//Written for the ToDoList (http://www.codeproject.com/KB/applications/todolist2.aspx)
//Design and latest version - http://www.codeproject.com/KB/applications/TDL_Calendar.aspx
//by demon.code.monkey@googlemail.com

#ifndef _TASKINFO_H_
#define _TASKINFO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <afxtempl.h>

class CTaskInfo
{
public:
	CTaskInfo(int _nTaskID=-1, const CString& _strTaskTitle="", BOOL _bHasStart=FALSE, BOOL _bHasDue=FALSE, BOOL _bHasDone=FALSE, BOOL _bIsComplete=FALSE);
	~CTaskInfo();

	CTaskInfo(const CTaskInfo& _that);				//copy constructor
	CTaskInfo& operator=(const CTaskInfo& _that);	//assignment operator

	int		GetTaskID() const;
	CString GetTaskTitle() const;
	BOOL	HasStart() const;
	BOOL	HasDue() const;
	BOOL	HasDone() const;
	BOOL	IsComplete() const;

protected:
	int		m_nTaskID;
	CString m_strTaskTitle;
	BOOL	m_bHasStart;
	BOOL	m_bHasDue;
	BOOL	m_bHasDone;
	BOOL	m_bIsComplete;
};

typedef CList <CTaskInfo, CTaskInfo&> CTaskInfoList;


#endif//_TASKINFO_H_
