// ToDoCtrlUndo.cpp: implementation of the CToDoCtrlUndo class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "todolist.h"
#include "ToDoCtrlUndo.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CToDoCtrlUndo::CToDoCtrlUndo() : m_nActiveAction(TDCUAT_NONE)
{

}

CToDoCtrlUndo::~CToDoCtrlUndo()
{
	ASSERT(m_nActiveAction == TDCUAT_NONE);
}

BOOL CToDoCtrlUndo::BeginNewAction(TDCUNDOACTIONTYPE nType)
{
	if (m_nActiveAction != TDCUAT_NONE)
		return FALSE;

	// create new action container
	TDCUNDOACTION tdcua(nType);
	m_aUndo.Add(tdcua);

	m_nActiveAction = nType;

	return TRUE;
}

BOOL CToDoCtrlUndo::EndCurrentAction()
{
//	ASSERT (m_nActiveAction != TDCUAT_NONE);

	if (m_nActiveAction == TDCUAT_NONE)
		return FALSE;

	m_nActiveAction = TDCUAT_NONE;

	// if the action is empty then delete it
	const TDCUNDOACTION& curAction = LastUndoAction();
	int nCurUndo = LastUndoIndex();

	if (curAction.aElements.GetSize() == 0)
		m_aUndo.RemoveAt(nCurUndo);
	
	else
	{
/*
		// or if the current edit just extends the previous edit
		// and wasn't more than 5 seconds ago
		if (nCurUndo > 0 && curAction.nType == TDCUAT_EDIT)
		{
			const TDCUNDOACTION& prevAction = m_aUndo.ElementAt(nCurUndo - 1);

			if (curAction == prevAction && (curAction.timeStamp - prevAction.timeStamp <= 5))
				m_aUndo.RemoveAt(nCurUndo);
		}
*/

		// clear Redo queue
		m_aRedo.RemoveAll();
	}

	return TRUE;
}

BOOL CToDoCtrlUndo::DeleteLastUndoAction()
{
	ASSERT (m_aUndo.GetSize() && !m_aRedo.GetSize());

	if (!m_aUndo.GetSize() || m_aRedo.GetSize())
		return FALSE;

	m_aUndo.RemoveAt(LastUndoIndex());

	return TRUE;
}

BOOL CToDoCtrlUndo::SaveElement(TDCUNDOELMOP nOp, DWORD dwTaskID, DWORD dwParentID, DWORD dwPrevSiblingID, 
								WORD wFlags, const TODOITEM* pTDI)
{
	ASSERT (m_nActiveAction != TDCUAT_NONE);

	if (m_nActiveAction == TDCUAT_NONE)
		return FALSE;

	ASSERT (IsValidElementOperation(nOp));

	if (!IsValidElementOperation(nOp))
		return FALSE;

	// add element to last action
	TDCUNDOELEMENT tdcue(nOp, dwTaskID, dwParentID, dwPrevSiblingID, wFlags, pTDI);

	int nLast = LastUndoIndex();
	m_aUndo[nLast].aElements.Add(tdcue);

	return TRUE;
}

BOOL CToDoCtrlUndo::IsValidElementOperation(TDCUNDOELMOP nOp) const
{
	ASSERT (m_nActiveAction != TDCUAT_NONE);

	if (m_nActiveAction == TDCUAT_NONE)
		return FALSE;

	switch (m_nActiveAction)
	{
	case TDCUAT_EDIT:
		return (nOp == TDCUEO_EDIT);
		
	case TDCUAT_ADD:
		return (nOp == TDCUEO_ADD || nOp == TDCUEO_EDIT);

	case TDCUAT_DELETE:
		return (nOp == TDCUEO_DELETE);

	case TDCUAT_COPY:
	case TDCUAT_PASTE:
		return (nOp == TDCUEO_ADD || nOp == TDCUEO_EDIT);

	case TDCUAT_ARCHIVE:
		return (nOp == TDCUEO_ADD || nOp == TDCUEO_DELETE);

	case TDCUAT_MOVE:
		return (nOp == TDCUEO_MOVE || nOp == TDCUEO_EDIT ||
				nOp == TDCUEO_ADD || nOp == TDCUEO_DELETE);
	}

	// all else
	return TRUE;
}

int CToDoCtrlUndo::GetLastUndoActionTaskIDs(CDWordArray& aIDs) const
{
	if (CanUndo())
		return LastUndoAction().GetTaskIDs(aIDs);

	// else
	return 0;
}

int CToDoCtrlUndo::GetLastRedoActionTaskIDs(CDWordArray& aIDs) const
{
	if (CanRedo())
		return LastRedoAction().GetTaskIDs(aIDs);

	// else
	return 0;
}

TDCUNDOACTION* CToDoCtrlUndo::UndoLastAction()
{
	if (!CanUndo())
		return NULL;

	// get last item from the undo queue
	TDCUNDOACTION& action = LastUndoAction();

	// move it to the redo queue
	m_aRedo.Add(action);
	m_aUndo.RemoveAt(LastUndoIndex());

	// and return a pointer to it so it can be modified if required
	return &LastRedoAction();
}

TDCUNDOACTION* CToDoCtrlUndo::RedoLastAction()
{
	if (!CanRedo())
		return NULL;

	// get last item from the redo queue
	TDCUNDOACTION& action = LastRedoAction();

	// move it to the undo queue
	m_aUndo.Add(action);
	m_aRedo.RemoveAt(LastRedoIndex());

	// and return a pointer to it so it can be modified if required
	return &LastUndoAction();
}

TDCUNDOACTION& CToDoCtrlUndo::LastUndoAction()
{
	ASSERT (CanUndo());

	return m_aUndo.ElementAt(LastUndoIndex());
}

TDCUNDOACTION& CToDoCtrlUndo::LastRedoAction()
{
	ASSERT (CanRedo());

	return m_aRedo.ElementAt(LastRedoIndex());
}

const TDCUNDOACTION& CToDoCtrlUndo::LastUndoAction() const
{
	ASSERT (CanUndo());

	return *(m_aUndo.GetData() + LastUndoIndex());
}

const TDCUNDOACTION& CToDoCtrlUndo::LastRedoAction() const
{
	ASSERT (CanRedo());

	return *(m_aRedo.GetData() + LastRedoIndex());
}

TDCUNDOACTIONTYPE CToDoCtrlUndo::GetLastUndoType() const
{
	ASSERT (CanUndo());

	return (CanUndo() ? LastUndoAction().nType : TDCUAT_NONE);
}

TDCUNDOACTIONTYPE CToDoCtrlUndo::GetLastRedoType() const
{
	ASSERT (CanRedo());

	return (CanRedo() ? LastRedoAction().nType : TDCUAT_NONE);
}

void CToDoCtrlUndo::ResetAll()
{
	EndCurrentAction();
	
	m_aUndo.RemoveAll();
	m_aRedo.RemoveAll();
}
