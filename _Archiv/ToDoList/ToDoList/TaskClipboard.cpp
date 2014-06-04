// TaskClipboard.cpp: implementation of the CTaskClipboard class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TaskClipboard.h"

#include "..\shared\misc.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

const LPCTSTR TDL_CLIPFMTTASKS = _T("CF_TODOLIST_TASKS");
const LPCTSTR TDL_CLIPFMTID = _T("CF_TODOLIST_ID");
const LPCTSTR DEF_CLIPID = _T("_emptyID_");

//////////////////////////////////////////////////////////////////////

void CTaskClipboard::Reset()
{
	if (!IsEmpty())
		::EmptyClipboard();
}

BOOL CTaskClipboard::IsEmpty()
{
	return !Misc::ClipboardHasFormat(GetIDClipFmt(), GetMainWnd());
}

BOOL CTaskClipboard::SetTasks(const CTaskFile& tasks, const CString& sID)
{
	ASSERT(tasks.GetTaskCount());
	
	CString sXML; 
	CString sClipID = (sID.IsEmpty() ? DEF_CLIPID : sID);
	
	if (tasks.Export(sXML) && !sXML.IsEmpty())
	{
		sClipID.MakeUpper();

		return (Misc::CopyTexttoClipboard(sXML, GetMainWnd(), GetTaskClipFmt()) &&
				Misc::CopyTexttoClipboard(sClipID, GetMainWnd(), GetIDClipFmt(), FALSE));
	}
	
	// else
	return FALSE;
}

BOOL CTaskClipboard::ClipIDMatches(const CString& sID)
{
	return (!sID.IsEmpty() && (sID.CompareNoCase(GetClipID()) == 0));
}

BOOL CTaskClipboard::GetTasks(CTaskFile& tasks, const CString& sID)
{
	CString sXML = Misc::GetClipboardText(GetMainWnd(), GetTaskClipFmt()); 
	
	if (tasks.LoadContent(sXML))
	{
		CString sClipID = (sID.IsEmpty() ? DEF_CLIPID : sID);

		// remove task references if the clip IDs do not match
		if (sClipID.CompareNoCase(GetClipID()) != 0)
		{
			RemoveTaskReferences(tasks, tasks.GetFirstTask());
		}
	}

	return tasks.GetTaskCount();
}

void CTaskClipboard::RemoveTaskReferences(CTaskFile& tasks, HTASKITEM hTask)
{
	if (!hTask)
		return;

	// handle next sibling first in case we want to delete hTask
	RemoveTaskReferences(tasks, tasks.GetNextTask(hTask));

	// delete if reference
	if (tasks.GetTaskReferenceID(hTask))
	{
		tasks.DeleteTask(hTask);
	}
	else // process children
	{
		RemoveTaskReferences(tasks, tasks.GetFirstTask(hTask));
	}
}

CString CTaskClipboard::GetClipID()
{
	return Misc::GetClipboardText(GetMainWnd(), GetIDClipFmt()); 
}

UINT CTaskClipboard::GetTaskClipFmt()
{
	static UINT nClip = ::RegisterClipboardFormat(TDL_CLIPFMTTASKS);
	return nClip;
}

UINT CTaskClipboard::GetIDClipFmt()
{
	static UINT nClip = ::RegisterClipboardFormat(TDL_CLIPFMTID);
	return nClip;
}

HWND CTaskClipboard::GetMainWnd()
{
	return (*AfxGetMainWnd());
}
