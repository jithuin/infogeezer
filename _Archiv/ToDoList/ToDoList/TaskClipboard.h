// TaskClipboard.h: interface for the CTaskClipboard class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TASKCLIPBOARD_H__7724479D_9E23_42B2_816F_40FE2B24B9C2__INCLUDED_)
#define AFX_TASKCLIPBOARD_H__7724479D_9E23_42B2_816F_40FE2B24B9C2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "taskfile.h"

class CTaskClipboard  
{
public:
	static void Reset();
	static BOOL IsEmpty();
	static BOOL SetTasks(const CTaskFile& tasks, const CString& sID = _T(""));
	static BOOL GetTasks(CTaskFile& tasks, const CString& sID);
	static CString GetClipID();
	static BOOL ClipIDMatches(const CString& sID);

protected:
	static UINT GetTaskClipFmt();
	static UINT GetIDClipFmt();
	static HWND GetMainWnd();
	static void RemoveTaskReferences(CTaskFile& tasks, HTASKITEM hTask);
};

#endif // !defined(AFX_TASKCLIPBOARD_H__7724479D_9E23_42B2_816F_40FE2B24B9C2__INCLUDED_)
