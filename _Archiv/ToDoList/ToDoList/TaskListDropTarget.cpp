// TaskListDropTarget.cpp: implementation of the CTaskListDropTarget class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TaskListDropTarget.h"

#include "..\shared\wclassdefines.h"
#include "..\shared\winclasses.h"
#include "..\shared\filemisc.h"
#include "..\shared\outlookhelper.h"
#include "..\shared\fileedit.h"

#include "..\3rdparty\msoutl.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

using namespace OutlookAPI;

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

#define IS_WND_TYPE(pWnd, cls, type) (pWnd->IsKindOf(RUNTIME_CLASS(cls)) || CWinClasses::IsClass(*pWnd, type))

CTaskListDropTarget::CTaskListDropTarget() : m_pOwner(NULL), m_nLVPrevHilite(-1), m_pOutlook(NULL)
{
}

CTaskListDropTarget::~CTaskListDropTarget()
{
	delete m_pOutlook;
	m_pOutlook = NULL;
}

BOOL CTaskListDropTarget::Register(CWnd* pTarget, CWnd* pOwner)
{
	m_pOwner = pOwner;

	return COleDropTarget::Register(pTarget);
}

DROPEFFECT CTaskListDropTarget::OnDragEnter(CWnd* pWnd, COleDataObject* /*pObject*/, DWORD /*dwKeyState*/, CPoint /*point*/)
{
	if (IS_WND_TYPE(pWnd, CTreeCtrl, WC_TREEVIEW))
		TreeView_SelectDropTarget(pWnd->GetSafeHwnd(), NULL);
	
	else if (IS_WND_TYPE(pWnd, CListCtrl, WC_LISTVIEW))
	{
		if (m_nLVPrevHilite != -1) // shouldn't happen
			ListView_SetItemState(pWnd->GetSafeHwnd(), m_nLVPrevHilite, 0, LVIS_DROPHILITED); // all items

		m_nLVPrevHilite = -1;
	}

	return DROPEFFECT_NONE;
}

void CTaskListDropTarget::OnDragLeave(CWnd* pWnd)
{
	if (IS_WND_TYPE(pWnd, CTreeCtrl, WC_TREEVIEW))
		TreeView_SelectDropTarget(pWnd->GetSafeHwnd(), NULL);
	
	else if (IS_WND_TYPE(pWnd, CListCtrl, WC_LISTVIEW))
	{
		if (m_nLVPrevHilite != -1) // shouldn't happen
			ListView_SetItemState(pWnd->GetSafeHwnd(), m_nLVPrevHilite, 0, LVIS_DROPHILITED); // all items

		m_nLVPrevHilite = -1;
	}
}

CTaskListDropTarget::TLDT_HITTEST CTaskListDropTarget::DoHitTest(CWnd* pWnd, CPoint point, HTREEITEM& htiHit, int& nHit)
{
	TLDT_HITTEST nHitResult = TLDTHT_NONE;

	htiHit = NULL;
	nHit = -1;

	if (IS_WND_TYPE(pWnd, CTreeCtrl, WC_TREEVIEW))
	{
		TVHITTESTINFO tvhti = { { point.x, point.y }, 0, 0 };
		TreeView_HitTest(pWnd->GetSafeHwnd(), &tvhti);
		
		htiHit = tvhti.hItem;
		nHitResult = TLDTHT_TREE;
	}
	else if (IS_WND_TYPE(pWnd, CListCtrl, WC_LISTVIEW))
	{
		LVHITTESTINFO lvhti = { { point.x, point.y }, 0 };
		ListView_HitTest(pWnd->GetSafeHwnd(), &lvhti);
		
		nHit = lvhti.iItem;
		nHitResult = TLDTHT_LIST;
	}
	else if (IS_WND_TYPE(pWnd, CFileEdit, WC_EDIT))
	{
		if (!(pWnd->GetStyle() & ES_READONLY))
			nHitResult = TLDTHT_FILEEDIT;
	}
	else if (pWnd->IsKindOf(RUNTIME_CLASS(CDialog)) ||
	 		pWnd->IsKindOf(RUNTIME_CLASS(CFrameWnd)))
	{
		// allow dropping only on titlebar
		if ((pWnd->GetStyle() & WS_CAPTION) && point.y < 0)
			nHitResult = TLDTHT_CAPTION;
	}

	return nHitResult;
}

DROPEFFECT CTaskListDropTarget::OnDragOver(CWnd* pWnd, COleDataObject* pObject, DWORD /*dwKeyState*/, CPoint point)
{
	if (!pWnd->IsWindowEnabled())
		return DROPEFFECT_NONE;

	HTREEITEM htiHit = NULL;
	int nListHit = -1;
	TLDT_HITTEST nHitTest = DoHitTest(pWnd, point, htiHit, nListHit);

	// update drop hilites
	if (nHitTest == TLDTHT_TREE)
	{
		TreeView_SelectDropTarget(pWnd->GetSafeHwnd(), htiHit);
	}
	else if (nHitTest == TLDTHT_LIST)
	{
		// remove previous highlighting
		if (m_nLVPrevHilite != -1 && m_nLVPrevHilite != nListHit)
			ListView_SetItemState(pWnd->GetSafeHwnd(), m_nLVPrevHilite, 0, LVIS_DROPHILITED); 
		
		if (nListHit != -1)
			ListView_SetItemState(pWnd->GetSafeHwnd(), nListHit, LVIS_DROPHILITED, LVIS_DROPHILITED);
		
		m_nLVPrevHilite = nListHit;
	}

	DROPEFFECT deResult = DROPEFFECT_NONE;

	if (COutlookHelper::IsOutlookObject(pObject))
	{
		if (nHitTest == TLDTHT_TREE || nHitTest == TLDTHT_LIST || nHitTest == TLDTHT_FILEEDIT)
			deResult = DROPEFFECT_COPY;
	}
	else
	{
		CStringArray aFilePaths;
		int nNumFiles = FileMisc::GetDroppedFilePaths(pObject, aFilePaths);

		if (nNumFiles)
		{
			CString sFilePath = aFilePaths[0];

			switch (nHitTest)
			{
			case TLDTHT_TREE:
				if (COutlookHelper::IsOutlookObject(sFilePath))
					deResult = DROPEFFECT_COPY;
				
				else if (htiHit && nNumFiles == 1)
					deResult = DROPEFFECT_LINK;
				break;
				
			case TLDTHT_LIST:
				if (COutlookHelper::IsOutlookObject(sFilePath))
					deResult = DROPEFFECT_COPY;
				
				else if (nListHit && nNumFiles == 1)
					deResult = DROPEFFECT_LINK;
				break;
				
			case TLDTHT_FILEEDIT:
				if (nNumFiles == 1)
					deResult = DROPEFFECT_LINK;
				break;
				
			case TLDTHT_CAPTION:
				deResult = DROPEFFECT_COPY;
				break;
			}
		}
	}

	// else
	return deResult;
}

BOOL CTaskListDropTarget::OnDrop(CWnd* pWnd, COleDataObject* pObject, DROPEFFECT /*dropEffect*/, CPoint point)
{
	CString sClass = CWinClasses::GetClass(*pWnd);
	m_pOwner->SetForegroundWindow();

	TLDT_DATA data;
	data.pObject = pObject;

	TLDT_HITTEST nHitTest = DoHitTest(pWnd, point, data.hti, data.nItem);

	if (COutlookHelper::IsOutlookObject(pObject) && InitializeOutlook())
	{
		data.pOutlookSelection = m_pOutlook->GetSelection();
		data.pObject = NULL;

		m_pOwner->SendMessage(WM_TLDT_DROP, (WPARAM)&data, (LPARAM)pWnd);

		// cleanup
		delete data.pOutlookSelection;
		delete m_pOutlook;
		m_pOutlook = NULL;
	}
	else
	{
		CStringArray aFilePaths;
		int nNumFiles = FileMisc::GetDroppedFilePaths(pObject, aFilePaths);

		if (nNumFiles)
		{
			data.pFilePaths = &aFilePaths;

			switch (nHitTest)
			{
			case TLDTHT_TREE:
			case TLDTHT_LIST:
				m_pOwner->SendMessage(WM_TLDT_DROP, (WPARAM)&data, (LPARAM)pWnd);
				break;
				
			case TLDTHT_FILEEDIT:
				if (nNumFiles == 1)
					m_pOwner->SendMessage(WM_TLDT_DROP, (WPARAM)&data, (LPARAM)pWnd);
				break;
				
			case TLDTHT_CAPTION:
				m_pOwner->SendMessage(WM_TLDT_DROP, (WPARAM)&data, (LPARAM)pWnd);
				break;
			}
		}
	}
	
	// cleanup
	if (nHitTest == TLDTHT_TREE)
	{
		TreeView_SelectDropTarget(pWnd->GetSafeHwnd(), NULL);
	}
	else if (nHitTest == TLDTHT_LIST)
	{
		// remove previous highlighting
		if (m_nLVPrevHilite != -1)
		{
			ListView_SetItemState(pWnd->GetSafeHwnd(), m_nLVPrevHilite, 0, LVIS_DROPHILITED); 
			m_nLVPrevHilite = -1;
		}
		
		if (data.nItem != -1)
			ListView_SetItemState(pWnd->GetSafeHwnd(), data.nItem, 0, LVIS_DROPHILITED);
	}

	return TRUE; // because we handle it
}

BOOL CTaskListDropTarget::InitializeOutlook()
{
	if (m_pOutlook == NULL)
	{
		m_pOutlook = new COutlookHelper;
	}

	return (m_pOutlook != NULL);
}

