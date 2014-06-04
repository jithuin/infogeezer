#include "stdafx.h"
#include "TreeListSyncer.h"

#include "..\shared\WinClasses.h"
#include "..\shared\autoflag.h"
#include "..\shared\themed.h"

#define WM_CHECK_WS_VSCROLL (WM_USER+1)
#define WM_RESYNC           (WM_USER+2)
#define WM_RESIZE           (WM_USER+3)
#define LVS_EX_DOUBLEBUFFER (0x00010000)
#define CDRF_SKIPPOSTPAINT	(0x00000100)

CTreeListSyncer::CTreeListSyncer(DWORD dwFlags) 
	: 
	m_hwndTreeHeader(NULL), 
	m_nLinkage(TLSL_RIGHTDATA_IS_LEFTITEM), 
	m_bResyncing(FALSE),
	m_dwFlags(dwFlags),
	m_htiSyncSelFix(NULL),
	m_bTreeExpanding(FALSE),
	m_bResyncPending(FALSE),
	m_bResizePending(FALSE),
	m_bResyncEnabled(TRUE)
{
}

CTreeListSyncer::~CTreeListSyncer()
{
}

void CTreeListSyncer::PostResync(HWND hwnd, BOOL bIncSelection)
{
	if (m_bResyncPending)
		return;

	m_bResyncPending = TRUE;

	if (m_bResyncEnabled && !m_bResyncing)
		::PostMessage(hwnd, WM_RESYNC, 0, bIncSelection);
}

void CTreeListSyncer::PostResync(HWND hwnd)
{
	PostResync(hwnd, TRUE);
}

void CTreeListSyncer::PostResize(HWND hwnd)
{
	if (m_bResizePending)
		return;

	m_bResizePending = TRUE;

	::PostMessage(hwnd, WM_RESIZE, 0, 0);
}

DWORD CTreeListSyncer::GetStyle(HWND hwnd)
{
	return ::GetWindowLong(hwnd, GWL_STYLE);
}

BOOL CTreeListSyncer::Sync(HWND hwndLeft, HWND hwndRight, TLS_LINKAGE nLink, HWND hwndTreeHeader)
{
	// check for invalid combinations
	if (!(IsList(hwndLeft) && IsList(hwndRight)) &&
		!(IsTree(hwndLeft) && IsList(hwndRight)) &&
		!(IsList(hwndLeft) && IsTree(hwndRight)))
	{
		return FALSE;
	}
	
	m_nLinkage = nLink;
	m_hwndTreeHeader = hwndTreeHeader;
	
	// cache whether the right view had a vert scrollbar visible
	BOOL bRightHadVScroll = HasVScrollBar(Right());
	
	// release any existing hooks
	m_scLeft.HookWindow(NULL);
	m_scRight.HookWindow(NULL);
	HookWindow(NULL); // parent
	
	// re-hook tree/lists
	VERIFY(m_scLeft.HookWindow(hwndLeft, this));
	VERIFY(m_scRight.HookWindow(hwndRight, this));
	
	// rehook parent
	ASSERT(::GetParent(hwndLeft) == ::GetParent(hwndRight));
	VERIFY(HookWindow(::GetParent(hwndRight)));
	
	// resync 
	Resync(hwndLeft, hwndRight);
	
	// hide left scrollbar
	ShowVScrollBar(hwndLeft, FALSE);
	
	// show right scrollbar as required
	ShowVScrollBar(hwndRight, bRightHadVScroll);
	
	if (IsList(hwndLeft))
	{
		ListView_SetExtendedListViewStyleEx(hwndLeft, LVS_EX_DOUBLEBUFFER, LVS_EX_DOUBLEBUFFER);
	}

	if (IsList(hwndRight))
	{
		ListView_SetExtendedListViewStyleEx(hwndRight, LVS_EX_DOUBLEBUFFER, LVS_EX_DOUBLEBUFFER);
	}
	
	// force resize for our scrollbars
	ForceNcCalcSize(hwndRight);
	
	return TRUE;
}

BOOL CTreeListSyncer::IsSyncing() const
{
	return (m_scLeft.GetHwnd() && m_scRight.GetHwnd());
}

void CTreeListSyncer::Unsync()
{
	m_scLeft.HookWindow(NULL);
	m_scRight.HookWindow(NULL);
	m_hwndTreeHeader = NULL;
}

void CTreeListSyncer::ForceNcCalcSize(HWND hwnd)
{
	::SetWindowPos(hwnd, NULL, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER); 
}

void CTreeListSyncer::EnableResync(BOOL bEnable, HWND hwnd) 
{ 
	m_bResyncEnabled = bEnable; 

	if (bEnable && hwnd)
	{
		ForceNcCalcSize(hwnd);
		PostResync(hwnd);
	}
}

BOOL CTreeListSyncer::Resync(HWND hwnd, HWND hwndTo)
{
	return Resync(hwnd, hwndTo, TRUE);
}

BOOL CTreeListSyncer::Resync(HWND hwnd, HWND hwndTo, BOOL bIncSelection)
{
	if (!m_bResyncEnabled || m_bResyncing)
		return FALSE;

	CAutoFlag af(m_bResyncing, TRUE);
	BOOL bSynced = FALSE;

	if (IsTree(hwnd) && IsList(hwndTo)) // sync Tree to list
	{
		// cache current tree selection
		HTREEITEM htiSel = GetTreeSelItem(hwnd);

		// make sure it's really selected
		if (!IsTreeItemSelected(hwnd, htiSel))
			htiSel = NULL;
		
		// determine whether we need to scroll by comparing the
		// first visible tree item with the first visible list item
		
		// get the lists first visible item
		int nListFirstVisItem = ListView_GetTopIndex(hwndTo);
		ASSERT (nListFirstVisItem != -1);
		
		// and its equivalent tree item
		HTREEITEM htiListFirstVis = GetTreeItem(hwnd, hwndTo, nListFirstVisItem);
		
		// and tree's first visible item
		HTREEITEM htiTreeFirstVis = TreeView_GetFirstVisible(hwnd);
		
		// are these different?
		if (htiTreeFirstVis != htiListFirstVis)
		{
			// if so, then scroll and move the selection
			// to the corresponding list item and update the current
			// tree selection
			TreeView_SelectSetFirstVisible(hwnd, htiListFirstVis);
			htiSel = htiListFirstVis;
			bSynced = TRUE;
		}
		
		// do we need to synchronize the selections?
		if (bIncSelection && (m_dwFlags & TLSF_SYNCSELECTION))
		{
			int nSelCount = ListView_GetSelectedCount(hwndTo);

			if ((m_dwFlags & TLSF_MULTISELECTION) && nSelCount > 1)
			{
				// get focused item first
				int nFocus = ListView_GetNextItem(hwndTo, -1, LVIS_SELECTED | LVIS_FOCUSED);
				HTREEITEM hti = NULL;

				if (nFocus != -1)
				{
					hti = GetTreeItem(hwnd, hwndTo, nFocus);
					ASSERT(hti);
				}

				SelectTreeItem(hwnd, hti, TRUE);

				if (nSelCount > 1)
				{
					int nSel = ListView_GetNextItem(hwndTo, -1, LVIS_SELECTED);
					
					while (nSel != -1)
					{
						HTREEITEM hti = GetTreeItem(hwnd, hwndTo, nSel);
						ASSERT(hti);

						SelectTreeItem(hwnd, hti, FALSE);
						nSel = ListView_GetNextItem(hwndTo, nSel, LVIS_SELECTED);
					}
				}

				bSynced = TRUE;
			}
			else
			{
				// get list selection 
				int nListSelItem = GetListSelItem(hwndTo);
				
				// and its equivalent tree item
				HTREEITEM htiListTreeSel = GetTreeItem(hwnd, hwndTo, nListSelItem);
				
				// restore selection
				if (htiListTreeSel && htiSel != htiListTreeSel)
				{
					// When we sync to an expanded parent item, something
					// goes wrong with the selection and it gets lost.
					// So we track it here only under the specific circumstances
					BOOL bParent = (TreeView_GetChild(hwnd, htiListTreeSel) != NULL);
					BOOL bExpanded = (bParent && IsTreeItemExpanded(hwnd, htiListTreeSel));

					if (htiListTreeSel && bParent)//bExpanded)
						m_htiSyncSelFix = htiListTreeSel;
					else
						m_htiSyncSelFix = NULL;

					SelectTreeItem(hwnd, htiListTreeSel, TRUE);
					bSynced = TRUE;
				}
				else if (nListSelItem == -1)
				{
					SelectTreeItem(hwnd, NULL, TRUE);
					bSynced = TRUE;
				}
			}
		}
	}
	else if (IsList(hwnd) && IsTree(hwndTo)) // syncing list to Tree
	{
		// do we need to synchronize the selections?
		if (bIncSelection && (m_dwFlags & TLSF_SYNCSELECTION))
		{
			CHTIArray aItems;
			int nHTItem = 1;

			// we avoid retrieving the multiple-selected items from the tree
			// if there is no need, so we have to do this in two stages
			if (m_dwFlags & TLSF_MULTISELECTION)
			{
				nHTItem = GetTreeSelItems(hwndTo, NULL, aItems);
			}

			if (nHTItem > 1)
			{
				// clear existing selection first
				ListView_SetItemState(hwnd, -1, 0, LVIS_SELECTED);
				
				// focused item
				HTREEITEM htiFocus = TreeView_GetSelection(hwndTo);

				if (htiFocus)
				{
					int nFocus = GetListItem(hwnd, hwndTo, htiFocus);
					ASSERT(nFocus != -1);

					ListView_SetItemState(hwnd, nFocus, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);
 				}

				while (nHTItem--)
				{
					HTREEITEM hti = aItems[nHTItem];

					if (hti != htiFocus)
					{
						int nSel = GetListItem(hwnd, hwndTo, hti);
						ASSERT(nSel != -1);

						ListView_SetItemState(hwnd, nSel, LVIS_SELECTED, LVIS_SELECTED);
					}
				}

				bSynced = TRUE;
			}
			else // single selection
			{
				// get our selection
				HTREEITEM htiSel = GetTreeSelItem(hwndTo);
				int nTreeSel = GetListItem(hwnd, hwndTo, htiSel);
				
				// do we need to change it
				int nListSel = GetListSelItem(hwnd);
				
				if (nListSel != nTreeSel)
				{
					ListView_SetItemState(hwnd, nListSel, 0, LVIS_SELECTED | LVIS_FOCUSED);
					ListView_SetItemState(hwnd, nTreeSel, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);

					bSynced = TRUE;
				}
			}
		}

		// get our first visible item
		HTREEITEM htiFirstVis = TreeView_GetFirstVisible(hwndTo);

		if (htiFirstVis)
		{
			// find the corresponding list item
			int nTreeFirstVisItem = GetListItem(hwnd, hwndTo, htiFirstVis);
			ASSERT (nTreeFirstVisItem != -1);
			
			// get the lists first visible item
			int nListFirstVisItem = ListView_GetTopIndex(hwnd);
			ASSERT (nListFirstVisItem != -1);
			
			// are these different?
			if (nTreeFirstVisItem != nListFirstVisItem)
			{
				// scroll list to current tree pos
				int nItemHeight = GetItemHeight(hwndTo);
				::SendMessage(hwnd, LVM_SCROLL, 0, (nTreeFirstVisItem - nListFirstVisItem) * nItemHeight);

				bSynced = TRUE;
			}
		}
		
		ResyncListHeader(hwnd);
	}
	else if (IsList(hwnd) && IsList(hwndTo)) // syncing list to list
	{
		//AF_NOREENTRANT
		
		// get scroll pos of 'hwndTo' and 'hwnd'
		int nToFirstVisItem = ListView_GetTopIndex(hwndTo);
		int nFirstVisItem = ListView_GetTopIndex(hwnd);
		
		// and it's equiv item in 'hwnd'
		int nEquivFirstVisItem = GetListItem(hwnd, hwndTo, nToFirstVisItem);
		
		if (nEquivFirstVisItem != nFirstVisItem)
		{
			int nItemHeight = max(GetItemHeight(hwndTo), GetItemHeight(hwnd));
			ListView_Scroll(hwnd, 0, (nEquivFirstVisItem - nFirstVisItem) * nItemHeight);
		}
		
		// do we need to synchronize the selections?
		if (bIncSelection && (m_dwFlags & TLSF_SYNCSELECTION))
		{
			// clear existing selection first
			ListView_SetItemState(hwnd, -1, 0, LVIS_SELECTED);
			
			// then update
			int nSelTo = ListView_GetNextItem(hwndTo, -1, LVIS_SELECTED);
			
			while (nSelTo != -1)
			{
				int nSel = GetListItem(hwnd, hwndTo, nSelTo);
				ASSERT(nSel != -1);
				ListView_SetItemState(hwnd, nSel, LVIS_SELECTED, LVIS_SELECTED);
				
				int nPrevSel = nSel;
				nSelTo = ListView_GetNextItem(hwndTo, nSelTo, LVIS_SELECTED);

				if (nSelTo == nPrevSel)
					break;
			}
		}
		
		ResyncListHeader(hwnd);

		bSynced = TRUE;
	}
	
	if (bSynced)
	{
#ifdef _DEBUG
		TraceResync(hwnd, hwndTo);
#endif

		//::InvalidateRect(hwnd, NULL, FALSE);
		::UpdateWindow(hwnd);

		//::InvalidateRect(hwndTo, NULL, FALSE);
		::UpdateWindow(hwndTo);
	}

	return bSynced;
}

void CTreeListSyncer::InvalidateTreeItem(HWND hwnd, HTREEITEM hti)
{
	ASSERT(IsTree(hwnd));

	CRect rItem;
	TreeView_GetItemRect(hwnd, hti, &rItem, 0);
						
	::InvalidateRect(hwnd, rItem, TRUE);
}

BOOL CTreeListSyncer::HasFocus() const
{
	return HasFocus(Left()) || HasFocus(Right());
}

BOOL CTreeListSyncer::HasFocus(HWND hwnd)
{
	return (::GetFocus() == hwnd);
}

int CTreeListSyncer::GetItemHeight(HWND hwnd)
{ 
	if (IsTree(hwnd))
		return TreeView_GetItemHeight(hwnd);
	
	// else list
	if (ListView_GetItemCount(hwnd))
	{
		CRect rItem;
		ListView_GetItemRect(hwnd, 0, rItem, LVIR_BOUNDS);
		
		return rItem.Height();
	}
	
	// else
	return 16;
}

DWORD CTreeListSyncer::GetTreeItemData(HWND hwnd, HTREEITEM hti)
{
	ASSERT(IsTree(hwnd));
	
	if (hti == NULL)
		return NULL;
	
	TVITEM tvi = { 0 };
	tvi.hItem = hti;
	tvi.mask = TVIF_PARAM;
	
	TreeView_GetItem(hwnd, &tvi);
	return tvi.lParam;
}

DWORD CTreeListSyncer::GetListItemData(HWND hwnd, int nItem)
{
	ASSERT(IsList(hwnd));
	
	if (nItem == -1)
		return NULL;
	
	LVITEM lvi = { 0 };
	lvi.iItem = nItem;
	lvi.mask = LVIF_PARAM;
	
	ListView_GetItem(hwnd, &lvi);
	return lvi.lParam;
}

int CTreeListSyncer::GetListSelItem(HWND hwnd)
{
	ASSERT(IsList(hwnd));
	return ListView_GetNextItem(hwnd, -1, LVIS_SELECTED);
}

int CTreeListSyncer::GetListFocusItem(HWND hwnd)
{
	ASSERT(IsList(hwnd));
	return ListView_GetNextItem(hwnd, -1, LVIS_SELECTED | LVIS_FOCUSED);
}

HTREEITEM CTreeListSyncer::GetTreeFocusItem(HWND hwnd)
{
	ASSERT(IsTree(hwnd));
	return TreeView_GetSelection(hwnd);
}

HTREEITEM CTreeListSyncer::GetTreeSelItem(HWND hwnd)
{
	return GetTreeFocusItem(hwnd);
}

HWND CTreeListSyncer::GetTree() const
{
	if (IsTree(Left()))
		return Left();

	else if (IsTree(Right()))
		return Right();

	ASSERT(0);
	return NULL;
}

HTREEITEM CTreeListSyncer::GetTreeSelItem() const
{
	return TreeView_GetSelection(GetTree());
}

int CTreeListSyncer::GetTreeSelectionCount() const
{
	CHTIArray aItems;
	return GetTreeSelItems(aItems);
}

int CTreeListSyncer::GetTreeSelItems(CHTIArray& aItems) const
{
	aItems.RemoveAll();
	return GetTreeSelItems(GetTree(), NULL, aItems);
}

void CTreeListSyncer::SetTreeSelItems(const CHTIArray& aItems, HTREEITEM htiFocus)
{
	HWND hwnd = GetTree();
	SetTreeSelItems(hwnd, aItems, htiFocus);
}

void CTreeListSyncer::SelectTreeItem(HTREEITEM hti, BOOL bClear)
{
	HWND hwnd = GetTree();
	SelectTreeItem(hwnd, hti, bClear);
}

int CTreeListSyncer::GetTreeSelItems(HWND hwnd, HTREEITEM hti, CHTIArray& aItems)
{
	ASSERT(IsTree(hwnd));

	if (hti && TreeItemHasState(hwnd, hti, TVIS_SELECTED))
		aItems.Add(hti);

	// children
	HTREEITEM htiChild = TreeView_GetChild(hwnd, hti);

	while (htiChild)
	{
		GetTreeSelItems(hwnd, htiChild, aItems);
		htiChild = TreeView_GetNextItem(hwnd, htiChild, TVGN_NEXT);
	}

	return aItems.GetSize();
}

void CTreeListSyncer::SetTreeSelItems(HWND hwnd, const CHTIArray& aItems, HTREEITEM htiFocus)
{
	ASSERT(IsTree(hwnd));

	if (htiFocus == NULL && aItems.GetSize())
	{
		htiFocus = aItems[0];
	}

	// removes all other selections
	TreeView_SelectItem(hwnd, htiFocus);

	// rest of selection
	int nItem = aItems.GetSize();

	while (nItem--)
	{
		SetTreeItemState(hwnd, aItems[nItem], TVIS_SELECTED, TVIS_SELECTED);
	}
}

void CTreeListSyncer::SelectTreeItem(HWND hwnd, HTREEITEM hti, BOOL bClear)
{
	ASSERT(IsTree(hwnd));

	if (bClear)
	{
		ClearTreeSelection(hwnd);

		if (hti)
			TreeView_SelectItem(hwnd, hti);
	}

	SetTreeItemState(hwnd, hti, TVIS_SELECTED, TVIS_SELECTED);
}

void CTreeListSyncer::ClearTreeSelection(HWND hwnd)
{
	CHTIArray aItems;
	int nItem = GetTreeSelItems(hwnd, NULL, aItems);

	while (nItem--)
		SetTreeItemState(hwnd, aItems[nItem], 0, TVIS_SELECTED);
}

int CTreeListSyncer::GetListItem(HWND hwndList, HWND hwndTree, HTREEITEM hti) const
{
	ASSERT(IsList(hwndList));
	ASSERT(IsTree(hwndTree));
	
	DWORD dwTreeData = GetTreeItemData(hwndTree, hti);
	
	// convert to item-data
	// item data matches so search for the tree item
	// having the same item data as the list item
	switch(m_nLinkage)
	{
	case TLSL_LEFTDATA_IS_RIGHTDATA:
	case TLSL_RIGHTDATA_IS_LEFTDATA:
		return FindListItem(hwndList, dwTreeData);
		
		// the item data of the left item is the right item
	case TLSL_LEFTDATA_IS_RIGHTITEM:
		if (IsLeft(hwndList)) 
		{
			// the list item's data is the tree HTREEITEM
			return FindListItem(hwndList, (DWORD)hti);
		}
		else // tree is left
		{
			// the tree item's data is the list item's index
			return (int)dwTreeData;
		}
		break;
		
		// the item data of the right item is the left item
	case TLSL_RIGHTDATA_IS_LEFTITEM:
		if (IsLeft(hwndTree))
		{
			// the list item's data is the HTREEITEM
			return FindListItem(hwndList, (DWORD)hti);
		}
		else // list is left
		{
			// the tree item's data is the list item's index
			return (int)dwTreeData;
		}
		break;
	}
	
	// not found
	ASSERT(0);
	return -1;
}

int CTreeListSyncer::FindListItem(HWND hwnd, DWORD dwItemData)
{
	ASSERT(IsList(hwnd));
	
	LVFINDINFO lvfi = { 0 };
	lvfi.flags = LVFI_PARAM;
	lvfi.lParam = dwItemData;
	lvfi.vkDirection = VK_DOWN;
	
	return ListView_FindItem(hwnd, -1, &lvfi);
}

HTREEITEM CTreeListSyncer::GetTreeItem(HWND hwndTree, HWND hwndList, int nItem) const
{
	ASSERT(IsList(hwndList));
	ASSERT(IsTree(hwndTree));
	
	if (nItem == -1)
		return NULL;
	
	DWORD dwListData = GetListItemData(hwndList, nItem);
	
	// convert to item-data
	switch (m_nLinkage)
	{
		// item data matches so search for the tree item
		// having the same item data as the list item
	case TLSL_LEFTDATA_IS_RIGHTDATA:
	case TLSL_RIGHTDATA_IS_LEFTDATA:
		return FindTreeItem(hwndTree, dwListData);
		
		// the item data of the left item i_ the right item
	case TLSL_LEFTDATA_IS_RIGHTITEM:
		if (IsLeft(hwndList)) 
		{
			// the list item's data is the tree HTREEITEM
			return (HTREEITEM)dwListData;
		}
		else // tree is left
		{
			// the tree item's data is the list items' index
			return FindTreeItem(hwndTree, (DWORD)nItem);
		}
		break;
		
		// the item data of the right item is the left item
	case TLSL_RIGHTDATA_IS_LEFTITEM:
		if (IsLeft(hwndTree))
		{
			// the list item's data is the tree HTREEITEM
			return (HTREEITEM)dwListData;
		}
		else // list is left
		{
			// the tree item's data is the list items' index
			return FindTreeItem(hwndTree, (DWORD)nItem);
		}
		break;
	}
	
	// not found
	ASSERT(0);
	return NULL;
}

int CTreeListSyncer::GetListItem(HWND hwndList1, HWND hwndList2, int nItem2) const
{
	ASSERT(IsList(hwndList1));
	ASSERT(IsList(hwndList2));
	
	if (nItem2 == -1)
		return -1;
	
	DWORD dwListData2 = GetListItemData(hwndList2, nItem2);
	
	// convert to item-data
	switch (m_nLinkage)
	{
		// item data matches so search for the tree item
		// having the same item data as the list item
	case TLSL_LEFTDATA_IS_RIGHTDATA:
	case TLSL_RIGHTDATA_IS_LEFTDATA:
		return FindListItem(hwndList1, dwListData2);
		
		// the item data of the left item is the right item
	case TLSL_LEFTDATA_IS_RIGHTITEM:
		if (IsLeft(hwndList2)) 
		{
			// the list2 item's data is the list1 item's index
			return (int)dwListData2;
		}
		else // list1 is left
		{
			// the list1 item's data is the list2 item's index
			return FindListItem(hwndList1, (DWORD)nItem2);
		}
		break;
		
		// the item data of the right item is the left item
	case TLSL_RIGHTDATA_IS_LEFTITEM:
		if (IsLeft(hwndList1))
		{
			// the list2 item's data is the list1 item's index
			return (int)dwListData2;
		}
		else // list is left
		{
			// the list1 item's data is the list2 item's index
			return FindListItem(hwndList1, (DWORD)nItem2);
		}
		break;
	}
	
	// not found
	ASSERT(0);
	return NULL;
}

HTREEITEM CTreeListSyncer::FindTreeItem(HWND hwndTree, HTREEITEM hti, DWORD dwItemData)
{
	ASSERT(IsTree(hwndTree));
	
	if (hti && GetTreeItemData(hwndTree, hti) == dwItemData)
		return hti;
	
	// check children
	HTREEITEM htiChild = TreeView_GetChild(hwndTree, hti);
	
	while (htiChild)
	{
		HTREEITEM htiFind = FindTreeItem(hwndTree, htiChild, dwItemData);

		if (htiFind)
			return htiFind;

		htiChild = TreeView_GetNextItem(hwndTree, htiChild, TVGN_NEXT);
	}
	
	// not found
	return NULL;
}

HTREEITEM CTreeListSyncer::FindTreeItem(HWND hwndTree, DWORD dwItemData)
{
	HTREEITEM hti = FindVisibleTreeItem(hwndTree, dwItemData);

	if (!hti)
		hti = FindTreeItem(hwndTree, NULL, dwItemData);

	return hti;
}

HTREEITEM CTreeListSyncer::FindVisibleTreeItem(HWND hwndTree, DWORD dwItemData)
{
	ASSERT(IsTree(hwndTree));
	
	HTREEITEM hti = TreeView_GetFirstVisible(hwndTree);
	
	while (hti)
	{
		if (GetTreeItemData(hwndTree, hti) == dwItemData)
			return hti;

		hti = TreeView_GetNextVisible(hwndTree, hti);
	}
	
	// not found
	return NULL;
}

#ifdef _DEBUG
void CTreeListSyncer::TraceTreeSelection(LPCTSTR szLocation) const
{
	if (szLocation)
		TRACE(_T("CTreeListSyncer::TraceTreeSelection(%s, %d)\n"), szLocation, GetTreeSelectionCount());
	else
		TRACE(_T("CTreeListSyncer::TraceTreeSelection(%d)\n"), GetTreeSelectionCount());
}

void CTreeListSyncer::TraceListSelection(HWND hwnd, LPCTSTR szLocation) const
{
	ASSERT(IsList(hwnd));

	if (szLocation)
		TRACE(_T("CTreeListSyncer::TraceListSelection(%s, %d)\n"), szLocation, ListView_GetSelectedCount(hwnd));
	else
		TRACE(_T("CTreeListSyncer::TraceListSelection(%d)\n"), ListView_GetSelectedCount(hwnd));
}

void CTreeListSyncer::TraceResync(HWND hwnd, HWND hwndTo) const
{
	if (IsTree(hwnd))
		TRACE(_T("\nCTreeListSyncer::TraceResync(Tree to List)\n"));

	else if (IsTree(hwndTo))
		TRACE(_T("\nCTreeListSyncer::TraceResync(List to Tree)\n"));

	else
		TRACE(_T("\nCTreeListSyncer::TraceResync(List to List)\n"));
}

int CTreeListSyncer::GetListSelItem(HWND hwnd, CString& sText)
{
	int nSel = GetListSelItem(hwnd);
	
	if (nSel != -1)
	{
		const CListCtrl* pList = (CListCtrl*)CWnd::FromHandle(hwnd);
		sText = pList->GetItemText(nSel, 0);
	}
	
	return nSel;
}

HTREEITEM CTreeListSyncer::GetTreeSelItem(HWND hwnd, CString& sText)
{
	HTREEITEM htiSel = GetTreeSelItem(hwnd);
	
	if (htiSel)
	{
		const CTreeCtrl* pTree = (CTreeCtrl*)CWnd::FromHandle(hwnd);
		sText = pTree->GetItemText(htiSel);
	}
	
	return htiSel;
}

int CTreeListSyncer::GetListItem(HWND hwndList, HWND hwndTree, HTREEITEM hti, CString& sText) const
{
	int nItem = GetListItem(hwndList, hwndTree, hti);
	
	if (nItem != -1)
	{
		const CListCtrl* pList = (CListCtrl*)CWnd::FromHandle(hwndList);
		sText = pList->GetItemText(nItem, 0);
	}
	
	return nItem;
}

HTREEITEM CTreeListSyncer::GetTreeItem(HWND hwndTree, HWND hwndList, int nItem, CString& sText) const
{
	HTREEITEM hti = GetTreeItem(hwndTree, hwndList, nItem);
	
	if (hti)
	{
		const CTreeCtrl* pTree = (CTreeCtrl*)CWnd::FromHandle(hwndTree);
		sText = pTree->GetItemText(hti);
	}
	
	return hti;
}
#endif

HWND CTreeListSyncer::OtherWnd(HWND hwnd) const
{
	ASSERT(IsLeft(hwnd) || IsRight(hwnd));
	
	return (IsLeft(hwnd) ? Right() : Left());
}

BOOL CTreeListSyncer::IsList(HWND hwnd)
{
	return CWinClasses::IsClass(hwnd, WC_LISTVIEW);
}

BOOL CTreeListSyncer::IsTree(HWND hwnd)
{
	return CWinClasses::IsClass(hwnd, WC_TREEVIEW);
}

BOOL CTreeListSyncer::IsTreeHeader(HWND hwnd) const
{
	return (m_hwndTreeHeader == hwnd);
}

BOOL CTreeListSyncer::IsLeft(HWND hwnd) const
{
	return (hwnd == Left());
}

BOOL CTreeListSyncer::IsRight(HWND hwnd) const
{
	return (hwnd == Right());
}

void CTreeListSyncer::ResyncListHeader(HWND hwnd)
{
	ASSERT(IsList(hwnd));
	
	HWND hwndHeader = ListView_GetHeader(hwnd);
	
	if (hwndHeader)
	{
		CRect rHeader, rList;
		::GetWindowRect(hwndHeader, rHeader);
		::ScreenToClient(hwnd, &(rHeader.TopLeft()));
		::ScreenToClient(hwnd, &(rHeader.BottomRight()));
		::GetClientRect(hwnd, rList);
		
		rHeader.right = rList.right;
		::MoveWindow(hwndHeader, rHeader.left, rHeader.top, rHeader.Width(), rHeader.Height(), FALSE); 
	}
}

BOOL CTreeListSyncer::HandleEraseBkgnd(CDC* pDC)
{
	if (HasHScrollBar(m_scLeft.GetHwnd()) != HasHScrollBar(m_scRight.GetHwnd()))
	{
		CRect rLeft, rRight, rPaint;
		
		m_scLeft.GetWindowRect(rLeft);
		ScreenToClient(rLeft);
		
		m_scRight.GetWindowRect(rRight);
		ScreenToClient(rRight);
		
		if (rLeft.bottom > rRight.bottom)
		{
			rPaint = rRight;
			rPaint.top = rPaint.bottom;
			rPaint.bottom = rLeft.bottom;
			
			// allow for vert scrollbar
			if (HasVScrollBar(m_scRight.GetHwnd()))
				rPaint.right -= ::GetSystemMetrics(SM_CXVSCROLL);
		}
		else
		{
			rPaint = rLeft;
			rPaint.top = rPaint.bottom;
			rPaint.bottom = rRight.bottom;
			
			// allow for vert scrollbar
			if (HasVScrollBar(m_scLeft.GetHwnd()))
				rPaint.right -= ::GetSystemMetrics(SM_CXVSCROLL);
		}
		
		if (!rPaint.IsRectEmpty())
		{
			if (CThemed().IsNonClientThemed())
			{
				CThemed th;
				
				if (th.Open(GetCWnd(), _T("SCROLLBAR")))
					th.DrawBackground(pDC, SBP_LOWERTRACKHORZ, SCRBS_NORMAL, rPaint);
				else
					pDC->FillSolidRect(rPaint, ::GetSysColor(COLOR_SCROLLBAR));
			}
			else
				pDC->FillSolidRect(rPaint, ::GetSysColor(COLOR_SCROLLBAR));
			
			pDC->ExcludeClipRect(rPaint);
			return TRUE;
		}
	}

	// all else
	return FALSE;
}

LRESULT CTreeListSyncer::WindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp)
{
	if (!m_bResyncEnabled)
		return CSubclassWnd::Default();

	ASSERT(hRealWnd == GetHwnd());
	
	if (msg == WM_ERASEBKGND)
	{
		CDC* pDC = CDC::FromHandle((HDC)wp);
		HandleEraseBkgnd(pDC);
	}
	// only interested in notifications from the tree/list pair to their parent
	else if (msg == WM_NOTIFY && (wp == m_scLeft.GetDlgCtrlID() || wp == m_scRight.GetDlgCtrlID()))
	{
		LPNMHDR pNMHDR = (LPNMHDR)lp;
		HWND hwnd = pNMHDR->hwndFrom;
		
		// always let everyone else have their turn before we resync.
		LRESULT lr = CSubclassWnd::Default();
		
		switch (pNMHDR->code)
		{
		case NM_CLICK:
			// only handle this if multi-selecting and list supports it
			if ((m_dwFlags & TLSF_MULTISELECTION) && IsList(hwnd) &&
				!(GetStyle(hwnd) & LVS_SINGLESEL))
			{
				Resync(OtherWnd(hwnd), hwnd, TRUE);
			}
			break;
			
		case LVN_ITEMCHANGED:
			// only handle this if not multi-selecting
			if (!(m_dwFlags & TLSF_MULTISELECTION) || (GetStyle(hwnd) & LVS_SINGLESEL))
			{
				NMLISTVIEW* pNMLV = (NMLISTVIEW*)pNMHDR;

				// only interested in selection changes
				if ((pNMLV->uChanged & LVIF_STATE) && (pNMLV->uNewState & LVIS_SELECTED))
				{
					// NOTE: must always handle this even if we are not Syncing Selection
					// because it may have resulted in a scroll-change
					Resync(OtherWnd(hwnd), hwnd, TRUE);
				}
			}
			break;
			
		case TVN_SELCHANGED:
			{
				LPNMTREEVIEW pNMTV = (LPNMTREEVIEW)pNMHDR;
				HTREEITEM htiNew = pNMTV->itemNew.hItem;
				
				if (htiNew != m_htiSyncSelFix)
					m_htiSyncSelFix = NULL;
				
				// NOTE: must always handle this even if we are not Syncing Selection
				// because it may have resulted in a scroll-change
				Resync(OtherWnd(hwnd), hwnd, TRUE);
			}
			break;
			
		case TVN_ITEMEXPANDING:
			m_bTreeExpanding = TRUE;
			PostResync(hwnd, FALSE);
			break;
			
		case TVN_ITEMEXPANDED:
			{
				NMTREEVIEW* pNMTV = (NMTREEVIEW*)pNMHDR;
				InvalidateTreeItem(hwnd, pNMTV->itemNew.hItem);
				m_bTreeExpanding = FALSE;
			}
			break;
			
		case NM_CUSTOMDRAW:
			{
				NMCUSTOMDRAW* pNMCD = (NMCUSTOMDRAW*)pNMHDR;
				
				if (IsList(hwnd))
				{
					LPNMLVCUSTOMDRAW  pLVCD = (LPNMLVCUSTOMDRAW)pNMHDR;
					
					if (pNMCD->dwDrawStage == CDDS_PREPAINT)
					{
						lr |= CDRF_NOTIFYITEMDRAW;
					}
					else
					{
						int nItem = (int)pNMCD->dwItemSpec;
						
						if (pNMCD->dwDrawStage == CDDS_ITEMPREPAINT)
						{
							if (IsListItemSelected(hwnd, nItem))
							{
								pLVCD->clrTextBk = GetSysColor(COLOR_HIGHLIGHT);
								pLVCD->clrText = GetSysColor(COLOR_HIGHLIGHTTEXT);
								
								lr |= CDRF_NEWFONT;
							}
							
							// we register for post-paint if we are synchronising focus
							if (m_dwFlags & TLSF_SYNCFOCUS)
							{
								lr |= CDRF_NOTIFYPOSTPAINT;	
							}
						}
						else if (pNMCD->dwDrawStage == CDDS_ITEMPOSTPAINT)
						{
							if (m_dwFlags & TLSF_SYNCFOCUS)
							{
								if (HasFocus() && !HasFocus(hwnd) && IsListItemSelected(hwnd, nItem))
								{
									CRect rFocus, rText;
									ListView_GetItemRect(hwnd, nItem, rFocus, LVIR_BOUNDS);
									
									// if the first list column is not zero width
									// then we use the start of the item label
									int nColWidth = ::SendMessage(hwnd, LVM_GETCOLUMNWIDTH, 0, 0);
									
									if (nColWidth != 0)
									{
										ListView_GetItemRect(hwnd, nItem, rText, LVIR_LABEL);
										rFocus.left = rText.left;
									}
									
									::DrawFocusRect(pLVCD->nmcd.hdc, rFocus);
									
									lr = CDRF_SKIPDEFAULT | CDRF_SKIPPOSTPAINT;
								}
							}
						}
					}
				}
				// when the tree expands Windows sends custom draw notifications
				// for _every_ tree item, visible or not. For trees with many items
				// this can take a significant amount of time to process so we 
				// disable custom draw while expanding
				else if (IsTree(hwnd) && !m_bTreeExpanding)
				{
					NMTVCUSTOMDRAW* pTVCD = (NMTVCUSTOMDRAW*)pNMCD;
					
					if (pNMCD->dwDrawStage == CDDS_PREPAINT)
					{
						lr = CDRF_NOTIFYITEMDRAW;	
					}
					else 
					{
						HTREEITEM hti = (HTREEITEM)pNMCD->dwItemSpec;
						
						if (pNMCD->dwDrawStage == CDDS_ITEMPREPAINT)
						{
							if (IsTreeItemSelected(hwnd, hti))
							{
								pTVCD->clrTextBk = GetSysColor(COLOR_HIGHLIGHT);
								pTVCD->clrText = GetSysColor(COLOR_HIGHLIGHTTEXT);
								
								lr |= CDRF_NEWFONT;
								
								// we register for post-paint if we are synchronising focus
								if (m_dwFlags & TLSF_SYNCFOCUS)
								{
									lr |= CDRF_NOTIFYPOSTPAINT;	
								}
							}
						}
						else if (pNMCD->dwDrawStage == CDDS_ITEMPOSTPAINT)
						{
							if (IsTreeItemSelected(hwnd, hti))
							{
								// draw focus rect if full-row or if not focused but synced
								BOOL bFullRowSelect = IsTreeFullRowSelect(hwnd);
								BOOL bSyncFocus = ((m_dwFlags & TLSF_SYNCFOCUS) && HasFocus() && !HasFocus(hwnd));
								
								CRect rFocus;
								TreeView_GetItemRect(hwnd, hti, &rFocus, !bFullRowSelect);
								
								if (bFullRowSelect && (HasFocus(hwnd) || bSyncFocus))
								{
									::DrawFocusRect(pTVCD->nmcd.hdc, rFocus);
									lr = CDRF_SKIPDEFAULT | CDRF_SKIPPOSTPAINT;
								}
								else if (!bFullRowSelect && bSyncFocus)
								{
									// extra fiddling because if not full row
									// the width of the focus rectangle
									// changes depending on item expanded states
									HTREEITEM htiParent = TreeView_GetParent(hwnd, hti);
									BOOL bAnyExpanded = TreeHasExpandedItem(hwnd, htiParent);
									BOOL bItemExpanded = IsTreeItemExpanded(hwnd, hti);
									
									if (!bAnyExpanded)
										rFocus.right -= 3;
									
									::DrawFocusRect(pTVCD->nmcd.hdc, rFocus);
									lr = CDRF_SKIPDEFAULT | CDRF_SKIPPOSTPAINT;
								}
							}
						}
					}
				}
			
				return lr;
			}
			break;
		}
	}
	
	return CSubclassWnd::Default();
}

BOOL CTreeListSyncer::IsTreeItemExpanded(HWND hwnd, HTREEITEM hti)
{
	return TreeItemHasState(hwnd, hti, TVIS_EXPANDED);
}

BOOL CTreeListSyncer::TreeHasExpandedItem(HWND hwnd, HTREEITEM hti)
{
	ASSERT(IsTree(hwnd));

	hti = TreeView_GetChild(hwnd, hti);

	while (hti)
	{
		if (IsTreeItemExpanded(hwnd, hti))
			return TRUE;

		hti = TreeView_GetNextItem(hwnd, hti, TVGN_NEXT);
	}

	// else
	return FALSE;
}

BOOL CTreeListSyncer::TreeItemHasState(HWND hwnd, HTREEITEM hti, UINT nStateMask)
{
	ASSERT(IsTree(hwnd));

	// handle root
	if (hti == NULL)
		return FALSE;

	const CTreeCtrl* pTree = (CTreeCtrl*)CWnd::FromHandle(hwnd);
	UINT nState = pTree->GetItemState(hti, nStateMask);

	return ((nState & nStateMask) == nStateMask);
}

void CTreeListSyncer::SetTreeItemState(HWND hwnd, HTREEITEM hti, UINT nState, UINT nStateMask)
{
	ASSERT(IsTree(hwnd));

	// handle root
	if (hti == NULL)
		return;

	CTreeCtrl* pTree = (CTreeCtrl*)CWnd::FromHandle(hwnd);
	pTree->SetItemState(hti, nState, nStateMask);
}

BOOL CTreeListSyncer::IsTreeItemSelected(HWND hwnd, HTREEITEM hti) const
{
	return TreeItemHasState(hwnd, hti, TVIS_SELECTED);
}

BOOL CTreeListSyncer::ListItemHasState(HWND hwnd, int nItem, UINT nStateMask)
{
	ASSERT(IsList(hwnd));

	UINT nState = ListView_GetItemState(hwnd, nItem, nStateMask);

	return ((nState & nStateMask) == nStateMask);
}

BOOL CTreeListSyncer::IsListItemSelected(HWND hwnd, int nItem)
{
	return ListItemHasState(hwnd, nItem, LVIS_SELECTED);
}

BOOL CTreeListSyncer::IsTreeFullRowSelect(HWND hwnd)
{
	ASSERT(IsTree(hwnd));

	if (GetStyle(hwnd) & TVS_FULLROWSELECT)
	{
		// check styles that override this one
		if (!(GetStyle(hwnd) & TVS_HASLINES))
			return TRUE;
	}

	// else 
	return FALSE;
}

BOOL CTreeListSyncer::IsListFullRowSelect(HWND hwnd)
{
	ASSERT(IsList(hwnd));

	return (ListView_GetExtendedListViewStyle(hwnd) & LVS_EX_FULLROWSELECT);
}

LRESULT CTreeListSyncer::ScWindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp)
{
	if (hRealWnd != Left() && hRealWnd != Right())
	{
		return CSubclasser::ScDefault(hRealWnd);
	}
	else if (!m_bResyncEnabled && msg != WM_NCCALCSIZE)
	{
		return CSubclasser::ScDefault(hRealWnd);
	}

	switch (msg)
	{
	case WM_VSCROLL:
		// one view has been scrolled => resync other
		if (m_bResyncEnabled)
		{
			LRESULT lr = CSubclasser::ScDefault(hRealWnd);

			if (Resync(OtherWnd(hRealWnd), hRealWnd, FALSE))
			{

			}
			return lr;
		}
		break;
		
	case WM_MOUSEWHEEL:
		if (IsRight(hRealWnd)) // right view has received mousewheel => resync left
		{
			LRESULT lr = CSubclasser::ScDefault(hRealWnd);

			if (Resync(OtherWnd(hRealWnd), hRealWnd, FALSE))
			{

			}
			return lr;
		}
		else // left view has received mousewheel input => response varies
		{
			HWND hwndOther = OtherWnd(hRealWnd);
			
			// if the right view has a vertical scrollbar this takes priority,
			// else if we do not have a horz scrollbar
			if (HasVScrollBar(hwndOther))
			{
				return ::SendMessage(hwndOther, WM_MOUSEWHEEL, wp, lp);
			}
			else if (!HasHScrollBar(hRealWnd))
			{
				return ::SendMessage(hwndOther, WM_MOUSEWHEEL, wp, lp);
			}
			// else default behaviour
		}
		break;

	case WM_NCPAINT:
		if (m_bTreeExpanding && IsLeft(hRealWnd) && IsTree(hRealWnd))
		{
			return 0L; // eat it
		}
		break;
		
	case WM_NCCALCSIZE: 
		{
			// hide VScrollbar first
			if (IsLeft(hRealWnd) && HasStyle(hRealWnd, WS_VSCROLL))
			{
				ShowVScrollBar(hRealWnd, FALSE, FALSE);
			}
			// then default behaviour
		}
		break;
		
	case WM_SIZE: 
		{
			// hide VScrollbar after default behaviour
			LRESULT lr = CSubclasser::ScDefault(hRealWnd);
			
			if (IsLeft(hRealWnd) && HasVScrollBar(hRealWnd))
			{
				ShowVScrollBar(hRealWnd, FALSE, FALSE);
			}
			return lr;
		}
		break;
		
	case WM_CHECK_WS_VSCROLL:
		if (IsLeft(hRealWnd) && ShowVScrollBar(hRealWnd, 0))
		{
			if (IsList(hRealWnd))
			{
				// resize Header bar
				ResyncListHeader(hRealWnd);
			}
		}
		break;
		
	case WM_RESYNC:
		Resync(OtherWnd(hRealWnd), hRealWnd, (BOOL)lp);
		m_bResyncPending = FALSE;
		break;
		
	case WM_STYLECHANGING:
		if (wp & GWL_STYLE)
		{
			LPSTYLESTRUCT lpStyleStruct = (LPSTYLESTRUCT)lp;
			
			// test for scrollbar visibility changes
			BOOL bVScrollBefore = (lpStyleStruct->styleOld & WS_VSCROLL);
			BOOL bVScrollNow = (lpStyleStruct->styleNew & WS_VSCROLL);
			
			BOOL bHScrollBefore = (lpStyleStruct->styleOld & WS_HSCROLL);
			BOOL bHScrollNow = (lpStyleStruct->styleNew & WS_HSCROLL);
			
			// always remove VScrollbar from left window
			if (IsLeft(hRealWnd) && bVScrollNow)
				lpStyleStruct->styleNew &= ~WS_VSCROLL;
			
			// force a resize if the horizontal or vertical scrollbar
			// visibility is changing
			if (bHScrollNow != bHScrollBefore || bVScrollNow != bVScrollBefore)
			{
				PostResize(hRealWnd);
			}
		}
		break;

// 	case WM_LBUTTONDOWN:
// 		if (IsList(hRealWnd) && GetTree())
// 		{
// 			LRESULT lr = CSubclasser::ScDefault(hRealWnd);
// 			Resync(OtherWnd(hRealWnd), hRealWnd, FALSE);
// 			return lr;
// 		}
// 		break;
		
	case WM_RESIZE:
		RefreshSize(hRealWnd);
		m_bResizePending = FALSE;
		break;
		
	case LVM_SCROLL:
		if (IsList(hRealWnd) && IsLeft(hRealWnd))
			::PostMessage(hRealWnd, WM_CHECK_WS_VSCROLL, 0, 0);
		break;

	case WM_TIMER:
		{
			LRESULT lr = CSubclasser::ScDefault(hRealWnd);
			Resync(OtherWnd(hRealWnd), hRealWnd, FALSE);
			return lr;
		}
		break;

	case WM_KILLFOCUS:
		if (m_dwFlags & TLSF_SYNCFOCUS)
			::InvalidateRect(hRealWnd, NULL, FALSE);
		break;

	case WM_SETFOCUS:
		if (m_dwFlags & TLSF_SYNCFOCUS)
			::InvalidateRect(OtherWnd(hRealWnd), NULL, FALSE);
		break;
		
  }
  
  return CSubclasser::ScDefault(hRealWnd);
}

BOOL CTreeListSyncer::ShowVScrollBar(HWND hwnd, BOOL bShow, BOOL bRefreshSize)
{
	DWORD dwStyle = GetStyle(hwnd);
	
	// see if there's anything to do first
	if ((bShow && !(dwStyle & WS_VSCROLL)) || (!bShow && (dwStyle & WS_VSCROLL)))
	{
		if (bShow)
			dwStyle |= WS_VSCROLL;
		else
			dwStyle &= ~WS_VSCROLL;
		
		::SetWindowLong(hwnd, GWL_STYLE, dwStyle);
		::ShowScrollBar(hwnd, SB_VERT, bShow);
		
		if (bRefreshSize)
			RefreshSize(hwnd);
	}
	
	return TRUE;
}

void CTreeListSyncer::Invalidate(BOOL bErase)
{
	m_scLeft.Invalidate(bErase);
	m_scRight.Invalidate(bErase);
}

void CTreeListSyncer::Resize(const CRect& rect, float fLeftProportion)
{
	if (rect.IsRectEmpty())
		return;

	CRect rLeft(rect), rRight(rect);
	
	rLeft.right = rLeft.left + (int)(fLeftProportion * rect.Width());
	rRight.left = rLeft.right;
	
	Resize(rLeft, rRight);
}

void CTreeListSyncer::Resize(const CRect& rect, int nLeftWidth)
{
	if (rect.IsRectEmpty())
		return;

    CRect rLeft(rect), rRight(rect);
	
    rLeft.right = rLeft.left + nLeftWidth;
    rRight.left = rLeft.right;

	Resize(rLeft, rRight);
}

void CTreeListSyncer::Resize(const CRect& rLeft, const CRect& rRight)
{
	// always resize list first to get it's header properly positioned
	if (IsList(Left()))
	{
		Resize(Left(), rLeft, FALSE);
		Resize(Right(), rRight, FALSE);
	}
	else // list is right
	{
		Resize(Right(), rRight, FALSE);
		Resize(Left(), rLeft, FALSE);
	}

	Resync(Left(), Right());
}

void CTreeListSyncer::RefreshSize(HWND hwnd)
{
	CRect rect;
	GetBoundingRect(hwnd, rect);
	Resize(hwnd, rect, TRUE);
}

void CTreeListSyncer::Resize(HWND hwnd, const CRect& rect, BOOL bInternal)
{
	CRect rWnd(rect);
	
	// handle tree header
	if (IsTree(hwnd))
	{
		// get current header rects
		CRect rThisHeader, rOtherHeader;
		BOOL bThisWndHasHeader = GetHeaderRect(hwnd, rThisHeader);
		BOOL bOtherWndHasHeader = GetHeaderRect(OtherWnd(hwnd), rOtherHeader);
		
		if (bThisWndHasHeader && !bOtherWndHasHeader)
		{
			// we have a tree header but no list header
			::ShowWindow(m_hwndTreeHeader, SW_HIDE);
		}
		else if (!bThisWndHasHeader && bOtherWndHasHeader)
		{
			::ShowWindow(m_hwndTreeHeader, SW_HIDE);
			rWnd.top = rOtherHeader.bottom;
		}
		else if (bThisWndHasHeader && bOtherWndHasHeader)
		{
			rWnd.top = rOtherHeader.bottom;
			rThisHeader.top = rOtherHeader.top;
			rThisHeader.bottom = rOtherHeader.bottom;
			rThisHeader.left = rWnd.left;
			rThisHeader.right = rWnd.right;
			
			::ShowWindow(m_hwndTreeHeader, SW_SHOW);
			::MoveWindow(m_hwndTreeHeader, rThisHeader.left, rThisHeader.top, rThisHeader.Width(), rThisHeader.Height(), TRUE);
		}
	}
	
	// if this wnd does not have a scrollbar but the other does
	// then make an allowance so that the client areas match
	BOOL bThisWndHasHScroll = HasHScrollBar(hwnd);
	BOOL bOtherWndHasHScroll = HasHScrollBar(OtherWnd(hwnd));
	
	if (!bThisWndHasHScroll && bOtherWndHasHScroll)
		rWnd.bottom -= GetSystemMetrics(SM_CYHSCROLL);
	
	// resize wnd
	CRect rPrev;
	GetBoundingRect(hwnd, rPrev);

	if (rWnd != rPrev)
	{
		::MoveWindow(hwnd, rWnd.left, rWnd.top, rWnd.Width(), rWnd.Height(), TRUE);
		
		// because horizontal and vertical scrollbars can 
		// unexpectedly appear in response to a resize we
		// retrigger another resize but only if this call
		// originated externally
		if (!bInternal)
			PostResize(hwnd);
	}
}

BOOL CTreeListSyncer::HasVScrollBar(HWND hwnd)
{
	return ((GetStyle(hwnd) & WS_VSCROLL) == WS_VSCROLL);
}

BOOL CTreeListSyncer::HasHScrollBar(HWND hwnd)
{
	return ((GetStyle(hwnd) & WS_HSCROLL) == WS_HSCROLL);
}

BOOL CTreeListSyncer::GetHeaderRect(HWND hwnd, CRect& rect) const
{
	ASSERT(IsList(hwnd) || IsList(OtherWnd(hwnd)));
	
	if (IsTree(hwnd) && m_hwndTreeHeader == NULL)
		return FALSE;
	
	// we always key off list header for determining the header height
	CRect rHeader;
	HWND hwndListHeader = IsList(hwnd) ? ListView_GetHeader(hwnd) : ListView_GetHeader(OtherWnd(hwnd));

	if (hwndListHeader)
		::GetWindowRect(hwndListHeader, rHeader);
	
	else if (IsTree(hwnd) && m_hwndTreeHeader) // revert to tree header
		::GetWindowRect(m_hwndTreeHeader, rHeader);
	
	// convert to parent coordinates
	HWND hwndParent = ::GetParent(hwnd);
	::ScreenToClient(hwndParent, (LPPOINT)&(rHeader.TopLeft()));
	::ScreenToClient(hwndParent, (LPPOINT)&(rHeader.BottomRight()));
	
	GetBoundingRect(hwnd, rect);
	rect.top = rHeader.top;
	rect.bottom = rHeader.bottom;
	
	return (rHeader.Height() > 0);
}

void CTreeListSyncer::GetBoundingRect(HWND hwnd, CRect& rect) const
{
	::GetWindowRect(hwnd, rect);
	
	// make sure our top and bottom extents take other window into consideration
	CRect rOther;
	::GetWindowRect(OtherWnd(hwnd), rOther);
	
	rect.top = min(rect.top, rOther.top);
	rect.bottom = max(rect.bottom, rOther.bottom);
	
	// convert to parent coordinates
	HWND hwndParent = ::GetParent(hwnd);
	::ScreenToClient(hwndParent, (LPPOINT)&(rect.TopLeft()));
	::ScreenToClient(hwndParent, (LPPOINT)&(rect.BottomRight()));
}

BOOL CTreeListSyncer::HasStyle(HWND hwnd, DWORD dwStyle)
{
	return ((GetStyle(hwnd) & dwStyle) == dwStyle);
}

void CTreeListSyncer::Sort(PFNCOMPARE pfnCompare, DWORD dwData)
{
	// sort the tree first
	BOOL bTreeIsLeft = IsTree(Left());
	BOOL bTreeIsRight = IsTree(Right());

	if (bTreeIsLeft || bTreeIsRight)
	{
		// tree is left
		HWND hwndTree = Left();
		HWND hwndList = Right();

		if (bTreeIsRight)
		{
			hwndList = Left();
			hwndTree = Right();
		}

		// sort root item recursively
		SortTreeItem(hwndTree, NULL, pfnCompare, dwData);

		// sort list by tree order
		CSortMap map;
		int nIndex = 0;

		BuildSortMap(hwndTree, NULL, map, nIndex);

		ListView_SortItems(hwndList, SortListProc, (DWORD)&map);
	}
	else // both lists
	{
		ListView_SortItems(Left(), pfnCompare, dwData);
		ListView_SortItems(Right(), pfnCompare, dwData);
	}
}

void CTreeListSyncer::SortTreeItem(HWND hwndTree, HTREEITEM hti, PFNCOMPARE pfnCompare, DWORD dwData)
{
	TVSORTCB tvSort = { hti, pfnCompare, dwData };

	// sort item itself
	TreeView_SortChildrenCB(hwndTree, &tvSort, 0);

	// then its children
	HTREEITEM htiChild = TreeView_GetChild(hwndTree, hti);

	while (htiChild)
	{
		SortTreeItem(hwndTree, htiChild, pfnCompare, dwData);
		htiChild = TreeView_GetNextItem(hwndTree, htiChild, TVGN_NEXT);
	}
}

void CTreeListSyncer::BuildSortMap(HWND hwndTree, CSortMap& map)
{
	int nNextIndex = 0;
	map.RemoveAll();

	BuildSortMap(hwndTree, NULL, map, nNextIndex);
}

void CTreeListSyncer::BuildSortMap(HWND hwndTree, HTREEITEM hti, CSortMap& map, int& nNextIndex)
{
	ASSERT(IsTree(hwndTree));

	HTREEITEM htiChild = TreeView_GetChild(hwndTree, hti);

	while (htiChild)
	{
		DWORD dwItemData = GetTreeItemData(hwndTree, htiChild);
		map[dwItemData] = nNextIndex++;

		// map this child's children
		BuildSortMap(hwndTree, htiChild, map, nNextIndex);
	
		// next child
		htiChild = TreeView_GetNextItem(hwndTree, htiChild, TVGN_NEXT);
	}
}

int CALLBACK CTreeListSyncer::SortListProc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
	const CSortMap* pMap = (CSortMap*)lParamSort;
	int nPos1, nPos2;

	if (pMap->Lookup(lParam1, nPos1) && pMap->Lookup(lParam2, nPos2))
	{
		return (nPos1 - nPos2);
	}

	// all else
	ASSERT(0);
	return 0;
}

