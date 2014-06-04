// ToDoCtrlData.cpp: implementation of the CToDoCtrlFind class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ToDoCtrlFind.h"
#include "ToDoCtrlData.h"
#include "ToDoitem.h"

#include "..\shared\timehelper.h"
#include "..\shared\datehelper.h"
#include "..\shared\misc.h"
#include "..\shared\enstring.h"
#include "..\shared\treectrlhelper.h"

#include <float.h>
#include <math.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CToDoCtrlFind::CToDoCtrlFind(CTreeCtrl& tree, const CToDoCtrlData& data) : 
	m_tree(tree), 
	m_data(data)
{
	
}

CToDoCtrlFind::~CToDoCtrlFind()
{
}

HTREEITEM CToDoCtrlFind::GetItem(DWORD dwID) const 
{ 
	if (dwID == 0)
		return NULL;

	return CTreeCtrlHelper(m_tree).FindItem(dwID);
}

DWORD CToDoCtrlFind::GetTaskID(HTREEITEM hti) const 
{ 
	if (!hti || hti == TVI_ROOT || hti == TVI_FIRST || hti == TVI_LAST)
		return 0;
	
	return m_tree.GetItemData(hti); 
}

TODOITEM* CToDoCtrlFind::GetTask(HTREEITEM hti, BOOL bTrue) const
{
	DWORD dwTaskID = GetTaskID(hti);
	return m_data.GetTask(dwTaskID, bTrue);
}

DWORD CToDoCtrlFind::GetLargestReferenceID(BOOL bVisibleOnly) const
{
	return GetLargestReferenceID(NULL, NULL, bVisibleOnly);
}

CString CToDoCtrlFind::GetLongestAllocTo(BOOL bVisibleOnly) const
{
	return GetLongestAllocTo(NULL, NULL, bVisibleOnly);
}

CString CToDoCtrlFind::GetLongestCategory(BOOL bVisibleOnly) const
{
	return GetLongestCategory(NULL, NULL, bVisibleOnly);
}

CString CToDoCtrlFind::GetLongestTag(BOOL bVisibleOnly) const
{
	return GetLongestTag(NULL, NULL, bVisibleOnly);
}

CString CToDoCtrlFind::GetLongestTime(int nDefUnits, BOOL bTimeEst, BOOL bVisibleOnly) const
{
	return GetLongestTime(NULL, NULL, NULL, nDefUnits, bTimeEst, bVisibleOnly);
}

CString CToDoCtrlFind::GetLongestPosition(BOOL bVisibleOnly) const
{
	return GetLongestPosition(NULL, NULL, NULL, bVisibleOnly);
}

CString CToDoCtrlFind::GetLongestCustomAttribute(const CString& sAttribID, BOOL bVisibleOnly) const
{
	return GetLongestCustomAttribute(NULL, NULL, sAttribID, bVisibleOnly, FALSE);
}

CString CToDoCtrlFind::GetLongestCustomDoubleAttribute(const CString& sAttribID, BOOL bVisibleOnly) const
{
	return GetLongestCustomAttribute(NULL, NULL, sAttribID, bVisibleOnly, TRUE);
}

CString CToDoCtrlFind::GetLongestRecurrence(BOOL bVisibleOnly) const
{
	return GetLongestRecurrence(NULL, NULL, bVisibleOnly);
}

CString CToDoCtrlFind::GetLongestExternalID(BOOL bVisibleOnly) const
{
	return GetLongestExternalID(NULL, NULL, bVisibleOnly);
}

DWORD CToDoCtrlFind::GetLargestReferenceID(HTREEITEM hti, const TODOITEM* pTDI, BOOL bVisibleOnly) const
{
	if (hti && !pTDI)
		pTDI = GetTask(hti, FALSE);

	DWORD dwLargest = pTDI ? pTDI->dwTaskRefID : 0;

	if (WantSearchChildren(hti, bVisibleOnly))
	{
		// check children
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			DWORD dwChildLargest = GetLargestReferenceID(htiChild, NULL, bVisibleOnly);
			dwLargest = max(dwLargest, dwChildLargest);

			// next
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	return dwLargest;

}

CString CToDoCtrlFind::GetLongestAllocTo(HTREEITEM hti, const TODOITEM* pTDI, BOOL bVisibleOnly) const
{
	if (hti && !pTDI)
		pTDI = GetTask(hti);

	CString sLongest = pTDI ? m_data.FormatTaskAllocTo(pTDI) : _T("");

	if (WantSearchChildren(hti, bVisibleOnly))
	{
		// check children
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			CString sChildLongest = GetLongestAllocTo(htiChild, NULL, bVisibleOnly);
			sLongest = Misc::GetLongest(sLongest, sChildLongest);

			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	return sLongest;
}

CString CToDoCtrlFind::GetLongestCategory(HTREEITEM hti, const TODOITEM* pTDI, BOOL bVisibleOnly) const
{
	if (hti && !pTDI)
		pTDI = GetTask(hti);

	CString sLongest = pTDI ? m_data.FormatTaskCategories(pTDI) : _T("");

	if (WantSearchChildren(hti, bVisibleOnly))
	{
		// check children
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			CString sChildLongest = GetLongestCategory(htiChild, NULL, bVisibleOnly);
			sLongest = Misc::GetLongest(sLongest, sChildLongest);

			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	return sLongest;
}

CString CToDoCtrlFind::GetLongestTag(HTREEITEM hti, const TODOITEM* pTDI, BOOL bVisibleOnly) const
{
	if (hti && !pTDI)
		pTDI = GetTask(hti);

	CString sLongest = pTDI ? m_data.FormatTaskTags(pTDI) : _T("");

	if (WantSearchChildren(hti, bVisibleOnly))
	{
		// check children
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			CString sChildLongest = GetLongestTag(htiChild, NULL, bVisibleOnly);
			sLongest = Misc::GetLongest(sLongest, sChildLongest);

			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	return sLongest;
}

CString CToDoCtrlFind::GetLongestExternalID(HTREEITEM hti, const TODOITEM* pTDI, BOOL bVisibleOnly) const
{
	if (hti && !pTDI)
		pTDI = GetTask(hti);

	CString sLongest = pTDI ? pTDI->sExternalID : _T("");

	// children
	if (WantSearchChildren(hti, bVisibleOnly))
	{
		// check children
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			CString sChildLongest = GetLongestExternalID(htiChild, NULL, bVisibleOnly);
			sLongest = Misc::GetLongest(sLongest, sChildLongest);

			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	return sLongest;
}

CString CToDoCtrlFind::GetLongestPosition(HTREEITEM hti, const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, BOOL bVisibleOnly) const
{
	if (hti && (!pTDI || !pTDS))
	{
		DWORD dwTaskID = GetTaskID(hti);
		
		if (!m_data.GetTask(dwTaskID, pTDI, pTDS))
			return _T("");
	}

	CString sLongest = (pTDI && pTDS) ? m_data.GetTaskPosition(pTDI, pTDS) : _T("");

	// children
	if (WantSearchChildren(hti, bVisibleOnly))
	{
		// check children
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			CString sChildLongest = GetLongestPosition(htiChild, NULL, NULL, bVisibleOnly);
			sLongest = Misc::GetLongest(sLongest, sChildLongest);

			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	return sLongest;
}

BOOL CToDoCtrlFind::WantSearchChildren(HTREEITEM hti, BOOL bVisibleOnly) const
{
	return ((hti == NULL) || !bVisibleOnly || (m_tree.GetItemState(hti, TVIS_EXPANDED) & TVIS_EXPANDED));
}

CString CToDoCtrlFind::GetLongestCustomAttribute(HTREEITEM hti, const TODOITEM* pTDI, const CString& sAttribID, 
												 BOOL bVisibleOnly, BOOL bDouble) const
{
	if (hti && !pTDI)
		pTDI = GetTask(hti);

	CString sLongest;

	if (pTDI)
		pTDI->mapCustomData.Lookup(sAttribID, sLongest);

	// children
	if (WantSearchChildren(hti, bVisibleOnly))
	{
		// check children
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			CString sChildLongest = GetLongestCustomAttribute(htiChild, NULL, sAttribID, bVisibleOnly, bDouble);
			sLongest = Misc::GetLongest(sLongest, sChildLongest, bDouble);

			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	return sLongest;
}

CString CToDoCtrlFind::GetLongestRecurrence(HTREEITEM hti, const TODOITEM* pTDI, BOOL bVisibleOnly) const
{
	if (hti && !pTDI)
		pTDI = GetTask(hti);

	CString sLongest = pTDI ? pTDI->trRecurrence.sRegularity : _T("");

	// children
	if (WantSearchChildren(hti, bVisibleOnly))
	{
		// check children
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			CString sChildLongest = GetLongestRecurrence(htiChild, NULL, bVisibleOnly);
			sLongest = Misc::GetLongest(sLongest, sChildLongest);

			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	return sLongest;
}

CString CToDoCtrlFind::GetLongestTime(HTREEITEM hti, const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, int nDefUnits, BOOL bTimeEst, BOOL bVisibleOnly) const
{
	if (hti && (!pTDI || !pTDS))
	{
		DWORD dwTaskID = GetTaskID(hti);
		
		if (!m_data.GetTask(dwTaskID, pTDI, pTDS))
		{
			ASSERT(0);
			return _T("");
		}
	}

	CString sLongest;

	if (hti && pTDI && pTDS)
	{
		int nUnits = nDefUnits;

		// get actual task time units
		if (!pTDS->HasSubTasks() || m_data.HasStyle(TDCS_ALLOWPARENTTIMETRACKING))
			nUnits = bTimeEst ? pTDI->nTimeEstUnits : pTDI->nTimeSpentUnits;
				
		// get time
		double dTime = bTimeEst ? m_data.CalcTimeEstimate(pTDI, pTDS, nUnits) : m_data.CalcTimeSpent(pTDI, pTDS, nUnits);
				
		if (dTime > 0 || !m_data.HasStyle(TDCS_HIDEZEROTIMECOST))
		{
			int nDecPlaces = m_data.HasStyle(TDCS_ROUNDTIMEFRACTIONS) ? 0 : 2;
			
			if (m_data.HasStyle(TDCS_DISPLAYHMSTIMEFORMAT))
				sLongest = CTimeHelper().FormatTimeHMS(dTime, nUnits, (BOOL)nDecPlaces);
			else
				sLongest = CTimeHelper().FormatTime(dTime, nUnits, nDecPlaces);
		}
	}

	// children
	if (WantSearchChildren(hti, bVisibleOnly))
	{
		// check children
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			CString sChildLongest = GetLongestTime(htiChild, NULL, NULL, nDefUnits, bTimeEst, bVisibleOnly);
			sLongest = Misc::GetLongest(sLongest, sChildLongest);

			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	return sLongest;
}

int CToDoCtrlFind::GetTaskBreadcrumbs(DWORD dwID, CDWordArray& aBreadcrumbs, DWORD dwFlags) const
{
	if (!Misc::HasFlag(dwFlags, TCFBC_APPEND))
		aBreadcrumbs.RemoveAll();

	HTREEITEM hti = GetItem(dwID);
	
	if (!hti)
		return 0; // ??

	BOOL bUp = Misc::HasFlag(dwFlags, TCFBC_UP);
	BOOL bParentsOnly = Misc::HasFlag(dwFlags, TCFBC_PARENTSONLY);
	BOOL bVisibleOnly = Misc::HasFlag(dwFlags, TCFBC_VISIBLEONLY);
	
	// progressively work our way up the parent chain
	// checking the parent's siblings at each level.
	// Note: we don't search into each task as we traverse it
	CTreeCtrlHelper tch(m_tree);
	
	if (bUp) // this is just like the task's path, from the task back to the root
	{
		HTREEITEM htiPrev;

		while (hti)
		{
			if (bParentsOnly)
				htiPrev = m_tree.GetParentItem(hti);

			else if (bVisibleOnly)
				htiPrev = tch.GetPrevVisibleItem(hti, FALSE);
			else
				htiPrev = tch.GetPrevItem(hti, FALSE);
			
			if (htiPrev == NULL || htiPrev == TVI_ROOT)
				break;
			
			// insert taskid at start
			aBreadcrumbs.InsertAt(0, GetTaskID(htiPrev));
			
			// keep going
			hti = htiPrev;
		}
	}
	else // this is from the task forward to the last parent's siblings
	{
		HTREEITEM htiNext;

		if (bParentsOnly)
		{
			// we don't include our parent
			htiNext = m_tree.GetParentItem(hti);

			while (htiNext)
			{
				htiNext = m_tree.GetParentItem(htiNext);

				if (htiNext == NULL)
					break;

				aBreadcrumbs.InsertAt(0, GetTaskID(htiNext));
			}
		}
		else // parents and siblings, but not children
		{
			htiNext = hti;

			do
			{
				if (bVisibleOnly)
					htiNext = tch.GetNextVisibleItem(htiNext, FALSE);
				else
					htiNext = tch.GetNextItem(htiNext, FALSE);

				if (htiNext)
					aBreadcrumbs.InsertAt(0, GetTaskID(htiNext));
			} 
			while (htiNext);
		}
	}
	
	return aBreadcrumbs.GetSize();
}

BOOL CToDoCtrlFind::FindVisibleTaskWithTime(TDC_DATE nDate)
{
	switch (nDate)
	{
	case TDCD_LASTMOD:
		return m_tree.GetCount(); // always

	case TDCD_START:
		return FindVisibleTaskWithStartTime();

	case TDCD_DONE:
		return FindVisibleTaskWithDoneTime();

	case TDCD_DUE:
		return FindVisibleTaskWithDueTime();

	case TDCD_CREATE:
	case TDCD_CUSTOM:
		return FALSE; // never
	}

	// all else
	return FALSE;
}

BOOL CToDoCtrlFind::FindVisibleTaskWithDueTime() const
{
	if (m_data.HasStyle(TDCS_HIDEDUETIMEFIELD))
		return FALSE;

	HTREEITEM hti = m_tree.GetChildItem(NULL);

	while (hti)
	{
		if (FindVisibleTaskWithDueTime(hti))
			return TRUE;
		
		hti = m_tree.GetNextItem(hti, TVGN_NEXT);
	}
	
	return FALSE;
}

BOOL CToDoCtrlFind::FindVisibleTaskWithDueTime(HTREEITEM hti) const
{
	ASSERT(hti);
	TODOITEM* pTDI = GetTask(hti);
	
	if (pTDI && !pTDI->IsDone() && pTDI->HasDueTime())
		return TRUE;

	// check children
	BOOL bVisible = (m_tree.GetItemState(hti, TVIS_EXPANDED) & TVIS_EXPANDED);

	if (bVisible)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			if (FindVisibleTaskWithDueTime(htiChild))
				return TRUE;

			// next child
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
	
	return FALSE;
}

BOOL CToDoCtrlFind::FindVisibleTaskWithStartTime() const
{
	if (m_data.HasStyle(TDCS_HIDESTARTTIMEFIELD))
		return FALSE;

	HTREEITEM hti = m_tree.GetChildItem(NULL);

	while (hti)
	{
		if (FindVisibleTaskWithStartTime(hti))
			return TRUE;
		
		hti = m_tree.GetNextItem(hti, TVGN_NEXT);
	}
	
	return FALSE;
}

BOOL CToDoCtrlFind::FindVisibleTaskWithStartTime(HTREEITEM hti) const
{
	ASSERT(hti);

	TODOITEM* pTDI = GetTask(hti);
	ASSERT(pTDI);
	
	if (pTDI && !pTDI->IsDone() && pTDI->HasStartTime())
		return TRUE;

	// check children
	BOOL bVisible = (m_tree.GetItemState(hti, TVIS_EXPANDED) & TVIS_EXPANDED);

	if (bVisible)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			if (FindVisibleTaskWithStartTime(htiChild))
				return TRUE;

			// next child
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
	
	return FALSE;
}

BOOL CToDoCtrlFind::FindVisibleTaskWithDoneTime() const
{
	if (m_data.HasStyle(TDCS_HIDEDONETIMEFIELD))
		return FALSE;

	HTREEITEM hti = m_tree.GetChildItem(NULL);

	while (hti)
	{
		if (FindVisibleTaskWithDoneTime(hti))
			return TRUE;
		
		hti = m_tree.GetNextItem(hti, TVGN_NEXT);
	}
	
	return FALSE;
}

BOOL CToDoCtrlFind::FindVisibleTaskWithDoneTime(HTREEITEM hti) const
{
	ASSERT(hti);

	TODOITEM* pTDI = GetTask(hti);
	ASSERT(pTDI);
	
	if (pTDI && pTDI->IsDone() && pTDI->HasDoneTime())
		return TRUE;

	// check children
	BOOL bVisible = (m_tree.GetItemState(hti, TVIS_EXPANDED) & TVIS_EXPANDED);

	if (bVisible)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			if (FindVisibleTaskWithDoneTime(htiChild))
				return TRUE;

			// next child
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
	
	return FALSE;
}

int CToDoCtrlFind::FindTasks(const SEARCHPARAMS& params, CResultArray& aResults) const
{
	if (!m_data.GetTaskCount())
		return 0;

	HTREEITEM hti = m_tree.GetChildItem(NULL);
	
	while (hti)
	{
		FindTasks(hti, params, aResults);
		hti = m_tree.GetNextItem(hti, TVGN_NEXT);
	}
	
	// else
	return aResults.GetSize();
}

void CToDoCtrlFind::FindTasks(HTREEITEM hti, const SEARCHPARAMS& params, CResultArray& aResults) const
{
	SEARCHRESULT result;
	DWORD dwID = GetTaskID(hti);

	// if the item is done and we're ignoring completed tasks 
	// (and by corollary their children) then we can stop right-away
	if (params.bIgnoreDone && m_data.IsTaskDone(dwID, TDCCHECKALL))
		return;

	// also we can ignore parent tasks if required but we still need 
	// to process it's children
	if (m_data.TaskMatches(dwID, params, result))
	{
		// check for overdue tasks
		if (!params.bIgnoreOverDue || !m_data.IsTaskOverDue(dwID))
			aResults.Add(result);
	}
	
	// process children
	HTREEITEM htiChild = m_tree.GetChildItem(hti);
		
	while (htiChild)
	{
		FindTasks(htiChild, params, aResults); // RECURSIVE call
			
		// next
		htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
	}
}


DWORD CToDoCtrlFind::FindFirstTask(const SEARCHPARAMS& params, SEARCHRESULT& result) const
{
	if (!m_data.GetTaskCount())
		return 0;

	return FindFirstTask(m_tree.GetChildItem(NULL), params, result);
}

DWORD CToDoCtrlFind::FindNextTask(DWORD dwStart, const SEARCHPARAMS& params, SEARCHRESULT& result, BOOL bNext) const
{
	HTREEITEM htiNext = GetItem(dwStart);
	CTreeCtrlHelper tch(m_tree);

	while (htiNext)
	{
		DWORD dwNextID = GetTaskID(htiNext);

		if (m_data.TaskMatches(dwNextID, params, result))
			return dwNextID;

		// next item
		htiNext = bNext ? tch.GetNextItem(htiNext) : tch.GetPrevItem(htiNext);
	}

	// else
	return 0; // not found
}

DWORD CToDoCtrlFind::FindFirstTask(HTREEITEM htiStart, const SEARCHPARAMS& params, SEARCHRESULT& result) const
{
	DWORD dwTaskID = GetTaskID(htiStart);

	if (!m_data.TaskMatches(dwTaskID, params, result))
	{
		// also check first child (other children are handled by sibling check below)
		HTREEITEM htiChild = m_tree.GetChildItem(htiStart);

		if (htiChild)
			dwTaskID = FindFirstTask(htiChild, params, result);
		else
			dwTaskID = 0;

		// and first sibling
		if (!dwTaskID)
		{
			HTREEITEM htiNext = m_tree.GetNextItem(htiStart, TVGN_NEXT);

			if (htiNext)
				dwTaskID = FindFirstTask(htiNext, params, result);
		}
	}
	
	return dwTaskID;
}

