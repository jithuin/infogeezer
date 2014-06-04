//Written for the ToDoList (http://www.codeproject.com/KB/applications/todolist2.aspx)
//Design and latest version - http://www.codeproject.com/KB/applications/TDL_Calendar.aspx
//by demon.code.monkey@googlemail.com

#include "stdafx.h"
#include "TaskInfo.h"

CTaskInfo::CTaskInfo(int _nTaskID,
					 const CString& _strTaskTitle,
					 BOOL _bHasStart,
					 BOOL _bHasDue,
					 BOOL _bHasDone,
					 BOOL _bIsComplete)
:	m_nTaskID(_nTaskID),
	m_strTaskTitle(_strTaskTitle),
	m_bHasStart(_bHasStart),
	m_bHasDue(_bHasDue),
	m_bHasDone(_bHasDone),
	m_bIsComplete(_bIsComplete)
{
}

CTaskInfo::~CTaskInfo()
{
}

//copy constructor
CTaskInfo::CTaskInfo(const CTaskInfo& _that)
{
	*this = _that;
}

//assignment operator
CTaskInfo& CTaskInfo::operator=(const CTaskInfo& _that)
{
	m_nTaskID = _that.m_nTaskID;
	m_strTaskTitle = _that.m_strTaskTitle;
	m_bHasStart = _that.m_bHasStart;
	m_bHasDue = _that.m_bHasDue;
	m_bHasDone = _that.m_bHasDone;
	m_bIsComplete = _that.m_bIsComplete;

	return *this;
}

int CTaskInfo::GetTaskID() const
{
	return m_nTaskID;
}

CString CTaskInfo::GetTaskTitle() const
{
	return m_strTaskTitle;
}

BOOL CTaskInfo::HasStart() const
{
	return m_bHasStart;
}

BOOL CTaskInfo::HasDue() const
{
	return m_bHasDue;
}

BOOL CTaskInfo::HasDone() const
{
	return m_bHasDone;
}

BOOL CTaskInfo::IsComplete() const
{
	return m_bIsComplete;
}
