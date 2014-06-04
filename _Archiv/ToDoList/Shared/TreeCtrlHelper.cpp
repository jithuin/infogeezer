// TreeCtrlHelper.cpp: implementation of the CTreeCtrlHelper class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TreeCtrlHelper.h"
#include "holdredraw.h"
//#include "dlgUnits.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

const int HVISIBLE = 20;

CTreeCtrlHelper::CTreeCtrlHelper(CTreeCtrl& tree) : m_tree(tree)
{
}

CTreeCtrlHelper::~CTreeCtrlHelper()
{
}

BOOL CTreeCtrlHelper::SelectItem(HTREEITEM hti)
{
	// save restore focus if setting to NULL
	CWnd* pFocus = hti ? NULL : CWnd::GetFocus();

	// won't auto edit if item already selected
	if (hti && m_tree.GetSelectedItem() == hti)
		return FALSE;

	m_tree.SelectItem(hti);

	// restore focus if required
	if (pFocus)
		pFocus->PostMessage(WM_SETFOCUS);

	return TRUE;
}

int CTreeCtrlHelper::GetItemHeight(HTREEITEM hti)
{
	if (m_tree.GetSafeHwnd())
	{
		if (hti == NULL)
			hti = m_tree.GetChildItem(NULL);
	}

	if (hti == NULL)
		return 16;

	// else
	CRect rItem;
	m_tree.GetItemRect(hti, rItem, FALSE);

	return rItem.Height();
}

int CTreeCtrlHelper::GetItemPos(HTREEITEM hti, HTREEITEM htiParent)
{
	ASSERT (m_tree.GetParentItem(hti) == htiParent);
	
	int nPos = 1;
	HTREEITEM htiChild = m_tree.GetChildItem(htiParent);
	
	while (htiChild && htiChild != hti)
	{
		nPos++;
		htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
	}
	
	return nPos;
}

int CTreeCtrlHelper::GetItemLevel(HTREEITEM hti)
{
	int nLevel = 0;
	hti = m_tree.GetParentItem(hti);
	
	while (hti)
	{
		nLevel++;
		hti = m_tree.GetParentItem(hti);
	}
	
	return nLevel;
}

BOOL CTreeCtrlHelper::HasFocus(BOOL bIncEditing)
{
	CWnd* pFocus = m_tree.GetFocus();
	
	return (pFocus && (pFocus == &m_tree || (bIncEditing && pFocus == m_tree.GetEditControl())));
}

void CTreeCtrlHelper::ExpandAll(BOOL bExpand)
{
	CHoldRedraw hr(m_tree);
	SetItemStateEx(NULL, bExpand ? TVIS_EXPANDED : 0, TVIS_EXPANDED, TRUE);
}

void CTreeCtrlHelper::ExpandItem(HTREEITEM hti, BOOL bExpand, BOOL bChildren)
{
	// special case: equiv to ExpandAll
	if (hti == NULL && bChildren)
	{
		ExpandAll(bExpand);
		return;
	}
	else if (hti)
	{
		m_tree.SetItemState(hti, bExpand ? TVIS_EXPANDED : 0, TVIS_EXPANDED);
	}
	
	if (bChildren)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);
		
		while (htiChild)
		{
			ExpandItem(htiChild, bExpand, TRUE); // RECURSIVE call
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
	
	// make sure parent items are expanded also
	if (bExpand)
	{
		HTREEITEM htiParent = m_tree.GetParentItem(hti);
		
		while (htiParent)
		{
			m_tree.SetItemState(hti, TVIS_EXPANDED, TVIS_EXPANDED);
			htiParent = m_tree.GetParentItem(htiParent);
		}
	}
}

void CTreeCtrlHelper::GetClientRect(LPRECT lpRect, HTREEITEM htiFrom)
{
	m_tree.GetClientRect(lpRect);

	if (htiFrom)
	{
		CRect rItem;
		m_tree.GetItemRect(htiFrom, rItem, FALSE);
		lpRect->top = max(0, rItem.top); // rItem.top could be invisible
	}
}

HTREEITEM CTreeCtrlHelper::GetFirstVisibleTopLevelItem()
{
	HTREEITEM hti = m_tree.GetFirstVisibleItem();
	HTREEITEM htiTopVis = GetTopLevelItem(hti);

	return htiTopVis;
}

HTREEITEM CTreeCtrlHelper::GetFirstVisibleTopLevelItem(int& nPos)
{
	HTREEITEM hti = m_tree.GetFirstVisibleItem();
	HTREEITEM htiTopVis = GetTopLevelItem(hti);

	// iterate the top level items to find out what pos this is
	nPos = GetItemPos(htiTopVis, NULL);
	
	return htiTopVis;
}

void CTreeCtrlHelper::InvalidateItem(HTREEITEM hti, BOOL bChildren)
{
	CRect rItem;
	
	if (m_tree.GetItemRect(hti, rItem, FALSE))
		m_tree.InvalidateRect(rItem);
	
	if (bChildren && IsItemExpanded(hti) > 0)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);
		
		while (htiChild)
		{
			InvalidateItem(htiChild, TRUE);
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
}

void CTreeCtrlHelper::ResetIndexMap() const
{
	m_mapVisibleIndices.RemoveAll();
}

int CTreeCtrlHelper::BuildVisibleIndexMap() const
{
	m_mapVisibleIndices.RemoveAll();

	int nTreeCount = m_tree.GetCount();

	if (nTreeCount)
	{
		m_mapVisibleIndices.InitHashTable(nTreeCount);
		
		HTREEITEM hti = m_tree.GetChildItem(NULL);
		
		while (hti)
		{
			AddVisibleItemToIndex(hti);
			hti = m_tree.GetNextItem(hti, TVGN_NEXT);
		}
	}

	return m_mapVisibleIndices.GetCount();
}

void CTreeCtrlHelper::AddVisibleItemToIndex(HTREEITEM hti) const
{
	ASSERT (hti);
	
	int nIndex = m_mapVisibleIndices.GetCount();
	m_mapVisibleIndices[hti] = nIndex;
	
	if (IsItemExpanded(hti) > 0)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);
		
		while (htiChild)
		{
			AddVisibleItemToIndex(htiChild);
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
}

void CTreeCtrlHelper::EnsureVisibleEx(HTREEITEM hti, BOOL bVPartialOK, BOOL bHPartialOK)
{
	CRect rItem;

	if (m_tree.GetItemRect(hti, rItem, FALSE))
	{
		CRect rClient, rText, rIntersect;

		m_tree.GetClientRect(rClient);
	
		BOOL bNeedHScroll = TRUE, bNeedVScroll = TRUE;

		// vertically
		rClient.InflateRect(0, m_tree.GetItemHeight());

		if (rIntersect.IntersectRect(rClient, rItem))
		{
			if (bVPartialOK || (rItem.top >= rClient.top && rItem.bottom <= rClient.bottom))
				bNeedVScroll = FALSE;
		}

		// horizontally
		rClient.DeflateRect(HVISIBLE, 0);
		m_tree.GetItemRect(hti, rText, TRUE);

		if (rIntersect.IntersectRect(rClient, rText))
		{
			if (bHPartialOK || (rText.left >= rClient.left && rText.right <= rClient.right))
				bNeedHScroll = FALSE;
		}

		// see if we're close enough already
		if (!bNeedVScroll && !bNeedHScroll)
			return;

		// vertical scroll
		if (bNeedVScroll)
		{
			// now get us as close as possible first
			// Only use CTreeCtrl::EnsureVisible if we're not in the right vertical pos
			// because it will also modify the horizontal pos which we don't want
			m_tree.EnsureVisible(hti);
			m_tree.GetItemRect(hti, rItem, FALSE);

			CRect rOrg(rItem);
			
			if (rItem.top < rClient.top || (bVPartialOK && rItem.bottom < rClient.top))
			{
				while (rClient.top > (bVPartialOK ? rItem.bottom : rItem.top))
				{
					m_tree.SendMessage(WM_VSCROLL, SB_LINEUP);
					m_tree.GetItemRect(hti, rItem, TRUE); // check again
					
					// check for no change
					if (rItem == rOrg)
						break;
					
					rOrg = rItem;
				}
			}
			else if (rItem.bottom > rClient.bottom || (bVPartialOK && rItem.top > rClient.bottom))
			{
				while (rClient.bottom < (bVPartialOK ? rItem.top : rItem.bottom))
				{
					m_tree.SendMessage(WM_VSCROLL, SB_LINEDOWN);
					m_tree.GetItemRect(hti, rItem, TRUE); // check again
					
					// check for no change
					if (rItem == rOrg)
						break;
					
					rOrg = rItem;
				}
			}
			
			bNeedHScroll = TRUE;
			m_tree.GetItemRect(hti, rText, TRUE);
		}

		// horizontal scroll
		if (bNeedHScroll)
		{
			rItem = rText;
			CRect rOrg(rItem);

			if (rItem.left < rClient.left || (bHPartialOK && rItem.right < rClient.left))
			{
				while (rClient.left > (bHPartialOK ? rItem.right : rItem.left))
				{
					m_tree.SendMessage(WM_HSCROLL, SB_LINELEFT);
					m_tree.GetItemRect(hti, rItem, TRUE); // check again

					// check for no change
					if (rItem == rOrg)
						break;

					rOrg = rItem;
				}
			}
			else if (rItem.right > rClient.right || (bHPartialOK && rItem.left > rClient.right))
			{
				while (rClient.right < (bHPartialOK ? rItem.left : rItem.right))
				{
					m_tree.SendMessage(WM_HSCROLL, SB_LINERIGHT);
					m_tree.GetItemRect(hti, rItem, TRUE); // check again

					// check for no change
					if (rItem == rOrg)
						break;

					rOrg = rItem;
				}
			}
		}
	}
	else
		m_tree.EnsureVisible(hti);

	m_tree.UpdateWindow();
}

BOOL CTreeCtrlHelper::ItemLineIsOdd(HTREEITEM hti) const
{
	// simple check on whether Visible item map has been created
	// for the first time
	if (!m_mapVisibleIndices.GetCount() && m_tree.GetCount())
		BuildVisibleIndexMap();
	
	int nIndex = -1;
	
	if (m_mapVisibleIndices.Lookup(hti, nIndex))
		return (nIndex % 2);
	
	return FALSE;
}

void CTreeCtrlHelper::SetItemIntegral(HTREEITEM hti, int iIntegral)
{
	TVITEMEX tvi;
	tvi.mask = TVIF_HANDLE | TVIF_INTEGRAL;
	tvi.hItem = hti;
	tvi.iIntegral = iIntegral;
	
	m_tree.SetItem((LPTVITEM)&tvi);
}

int CTreeCtrlHelper::IsItemExpanded(HTREEITEM hti, BOOL bFully) const
{
	// is it a parent
	if (!m_tree.ItemHasChildren(hti))
		return -1;

	// check children (check for first failure)
	if (bFully)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			if (IsItemExpanded(htiChild, TRUE) == FALSE)
				return FALSE;

			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}

	// else check item itself
	return (m_tree.GetItemState(hti, TVIS_EXPANDED) & TVIS_EXPANDED);
}

BOOL CTreeCtrlHelper::IsParentItemExpanded(HTREEITEM hti, BOOL bRecursive) const
{
	HTREEITEM htiParent = m_tree.GetParentItem(hti);

	if (!htiParent) // root
		return TRUE; // always expanded

	else if (IsItemExpanded(htiParent))
	{
		if (!bRecursive)
			return TRUE;

		// else recursive call
		return IsParentItemExpanded(htiParent, TRUE);
	}

	// else
	return FALSE;
}

HTREEITEM CTreeCtrlHelper::InsertItem(LPCTSTR lpszItem, int nImage, int nSelImage, 
									 LPARAM lParam, HTREEITEM htiParent, HTREEITEM htiAfter,
									 BOOL bBold, TCH_CHECK nCheck)
{
	UINT nState = (nCheck | (bBold ? TVIS_BOLD : 0));
	UINT nStateMask = (TVIS_BOLD | TVIS_STATEIMAGEMASK);
	
	return m_tree.InsertItem(TVIF_TEXT | TVIF_PARAM | TVIF_STATE | TVIF_IMAGE | TVIF_SELECTEDIMAGE,
							lpszItem, nImage, nSelImage, nState, 
							nStateMask, lParam, htiParent, htiAfter);
}

HTREEITEM CTreeCtrlHelper::InsertItem(LPCTSTR lpszItem, int nImage, int nSelImage, 
									  LPARAM lParam, HTREEITEM htiParent, HTREEITEM htiAfter,
									  BOOL bBold, BOOL bChecked)
{
	return InsertItem(lpszItem, nImage, nSelImage, lParam, htiParent, htiAfter,
					bBold, bChecked ? TCHC_CHECKED : TCHC_UNCHECKED);
}

void CTreeCtrlHelper::SetItemChecked(HTREEITEM hti, TCH_CHECK nChecked)
{
	if (nChecked != GetItemCheckState(hti))
	{
		m_tree.SetItemState(hti, nChecked, TVIS_STATEIMAGEMASK);
		ASSERT(GetItemCheckState(hti) == nChecked);
	}
}

void CTreeCtrlHelper::SetItemChecked(HTREEITEM hti, BOOL bChecked)
{
	SetItemChecked(hti, bChecked ? TCHC_CHECKED : TCHC_UNCHECKED);
}

TCH_CHECK CTreeCtrlHelper::GetItemCheckState(HTREEITEM hti) const
{
	DWORD dwState = m_tree.GetItemState(hti, TVIS_STATEIMAGEMASK);

	return (TCH_CHECK)INDEXTOSTATEIMAGEMASK(dwState >> 12);
}

HTREEITEM CTreeCtrlHelper::GetTopLevelItem(HTREEITEM hti) const
{
	if (!hti)
		return NULL;
	
	HTREEITEM htiParent = m_tree.GetParentItem(hti);
	
	while (htiParent)
	{
		hti = htiParent; // cache this because the next parent might be the root
		htiParent = m_tree.GetParentItem(hti);
	}
	
	return hti; // return the one before the root
}

HTREEITEM CTreeCtrlHelper::GetNextTopLevelItem(HTREEITEM hti, BOOL bNext) const
{
	HTREEITEM htiParent = GetTopLevelItem(hti);
	HTREEITEM htiGoto = NULL;
	
	if (htiParent)
	{
		if (bNext)
			htiGoto = m_tree.GetNextItem(htiParent, TVGN_NEXT);
		else
		{
			// if the item is not htiParent then jump to it first
			if (hti != htiParent)
				htiGoto = htiParent;
			else
				htiGoto = m_tree.GetNextItem(htiParent, TVGN_PREVIOUS);

			if (htiGoto == hti)
				htiGoto = NULL; // nowhere to go
		}
	}
		
	return htiGoto;
}

int CTreeCtrlHelper::GetDistanceToEdge(HTREEITEM htiFrom, TCH_EDGE nToEdge) const
{
	CRect rClient, rItem;

	if (m_tree.GetItemRect(htiFrom, rItem, FALSE))
	{
		m_tree.GetClientRect(rClient);
		int nItemHeight = m_tree.GetItemHeight();

		switch (nToEdge)
		{
		case TCHE_TOP:
			return (rItem.top - rClient.top) / nItemHeight;

		case TCHE_BOTTOM:
			return (rClient.bottom - rItem.bottom) / nItemHeight;
		}
	}

	return 0;
}

void CTreeCtrlHelper::SetMinDistanceToEdge(HTREEITEM htiFrom, TCH_EDGE nToEdge, int nItems)
{
	switch (nToEdge)
	{
	case TCHE_BOTTOM:
		{
			int nBorder = GetDistanceToEdge(htiFrom, TCHE_BOTTOM);

			while (nBorder < nItems)
			{
				m_tree.SendMessage(WM_VSCROLL, SB_LINEDOWN);
				nBorder++;
			}
		}
		break;

	case TCHE_TOP:
		{
			int nBorder = GetDistanceToEdge(htiFrom, TCHE_TOP);

			while (nBorder < nItems)
			{
				m_tree.SendMessage(WM_VSCROLL, SB_LINEUP);
				nBorder++;
			}
		}
		break;
	}
}

HTREEITEM CTreeCtrlHelper::GetNextPageVisibleItem(HTREEITEM hti) const
{
	// if we're the last visible item then its just the page count
	// figure out how many items to step
	HTREEITEM htiNext = m_tree.GetNextVisibleItem(hti);

	if (!htiNext || !IsFullyVisible(htiNext))
	{
		int nCount = m_tree.GetVisibleCount();

		// keep going until we get NULL and then return
		// the previous item
		while (hti && nCount--)
		{
			hti = GetNextVisibleItem(hti);

			if (hti)
				htiNext = hti;
		}
	}
	else // we keep going until we're the last visible item
	{
		// keep going until we get NULL and then return
		// the previous item
		while (hti)
		{
			hti = m_tree.GetNextVisibleItem(hti);
			
			if (hti && IsFullyVisible(hti))
				htiNext = hti;
			else
				hti = NULL;
		}
	}

	return (htiNext != hti) ? htiNext : NULL;
}

HTREEITEM CTreeCtrlHelper::GetPrevPageVisibleItem(HTREEITEM hti) const
{
	// if we're the first visible item then its just the page count
	// figure out how many items to step
	HTREEITEM htiPrev = m_tree.GetPrevVisibleItem(hti);

	if (!htiPrev || !IsFullyVisible(htiPrev))
	{
		int nCount = m_tree.GetVisibleCount();

		// keep going until we get NULL and then return
		// the previous item
		while (hti && nCount--)
		{
			hti = GetPrevVisibleItem(hti);

			if (hti)
				htiPrev = hti;
		}
	}
	else // we keep going until we're the first visible item
	{
		// keep going until we get NULL and then return
		// the previous item
		while (hti)
		{
			hti = m_tree.GetPrevVisibleItem(hti);
			
			if (hti && IsFullyVisible(hti))
				htiPrev = hti;
			else
				hti = NULL;
		}
	}

	return (htiPrev != hti) ? htiPrev : NULL;
}

HTREEITEM CTreeCtrlHelper::GetNextVisibleItem(HTREEITEM hti, BOOL bAllowChildren) const
{
	HTREEITEM htiNext = NULL;
	
	// try our first child if we're expanded
	if (bAllowChildren && IsItemExpanded(hti) > 0)
		htiNext = m_tree.GetChildItem(hti);
	else
	{
		// try next sibling
		htiNext = m_tree.GetNextItem(hti, TVGN_NEXT);
		
		// finally look up the parent chain as far as we can
		if (!htiNext)
		{
			HTREEITEM htiParent = hti;

			while (htiParent && !htiNext)
			{
				htiParent = m_tree.GetParentItem(htiParent);

				if (htiParent)
					htiNext = m_tree.GetNextItem(htiParent, TVGN_NEXT);
			}
		}
	}

	if (htiNext == TVI_ROOT || htiNext == hti)
		return NULL;

	// else
	return htiNext;
}

HTREEITEM CTreeCtrlHelper::GetNextItem(HTREEITEM hti, BOOL bAllowChildren) const
{
	HTREEITEM htiNext = NULL;
	
	// try our first child if we have one
	if (bAllowChildren && m_tree.ItemHasChildren(hti))
		htiNext = m_tree.GetChildItem(hti);
	else
	{
		// try next sibling
		htiNext = m_tree.GetNextItem(hti, TVGN_NEXT);
		
		// finally look up the parent chain as far as we can
		if (!htiNext)
		{
			HTREEITEM htiParent = hti;

			while (htiParent && !htiNext)
			{
				htiParent = m_tree.GetParentItem(htiParent);

				if (htiParent)
					htiNext = m_tree.GetNextItem(htiParent, TVGN_NEXT);
			}
		}
	}

	if (htiNext == TVI_ROOT || htiNext == hti)
		return NULL;

	// else
	return htiNext;
}

HTREEITEM CTreeCtrlHelper::GetPrevItem(HTREEITEM hti, BOOL bAllowChildren) const
{
	// try our prior sibling
	HTREEITEM htiPrev = m_tree.GetNextItem(hti, TVGN_PREVIOUS);
	
	// if we have one then first try its last child
	if (htiPrev)
	{
		if (bAllowChildren && m_tree.ItemHasChildren(htiPrev))
			htiPrev = GetLastChildItem(htiPrev);

		// else we settle for htiPrev as-is
	}
	else // get parent
		htiPrev = m_tree.GetParentItem(hti);

	if (htiPrev == TVI_ROOT || htiPrev == hti)
		return NULL;

	// else
	return htiPrev;
}


HTREEITEM CTreeCtrlHelper::GetPrevVisibleItem(HTREEITEM hti, BOOL bAllowChildren) const
{
	// try our prior sibling
	HTREEITEM htiPrev = m_tree.GetNextItem(hti, TVGN_PREVIOUS);
	
	// if we have one then first try its last child
	if (htiPrev)
	{
		if (bAllowChildren && IsItemExpanded(htiPrev) > 0)
		{
			HTREEITEM htiChild = m_tree.GetChildItem(htiPrev);
			
			while (htiChild)
			{
				htiPrev = htiChild;
				htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
			}
		}
		// else we settle for htiPrev as-is
	}
	else // get parent
		htiPrev = m_tree.GetParentItem(hti);

	if (htiPrev == TVI_ROOT || htiPrev == hti)
		return NULL;

	// else
	return htiPrev;
}

BOOL CTreeCtrlHelper::IsFullyVisible(HTREEITEM hti) const
{
	CRect rClient, rItem, rIntersect;

	m_tree.GetClientRect(rClient);
	m_tree.GetItemRect(hti, rItem, FALSE);

	return (rIntersect.IntersectRect(rItem, rClient) && rIntersect == rItem);
}

int CTreeCtrlHelper::FindItem(HTREEITEM htiFind, HTREEITEM htiStart)
{
	// try same first
	if (htiFind == htiStart)
		return 0;

	// then try up
	HTREEITEM htiPrev = GetPrevVisibleItem(htiStart);

	while (htiPrev)
	{
		if (htiPrev == htiFind)
			return -1;

		htiPrev = GetPrevVisibleItem(htiPrev);
	}

	// else try down
	HTREEITEM htiNext = GetNextVisibleItem(htiStart);

	while (htiNext)
	{
		if (htiNext == htiFind)
			return 1;

		htiNext = GetNextVisibleItem(htiNext);
	}

	// else
	return 0;
}

BOOL CTreeCtrlHelper::IsItemBold(HTREEITEM hti)
{
	return (m_tree.GetItemState(hti, TVIS_BOLD) & TVIS_BOLD);
}

void CTreeCtrlHelper::SetItemBold(HTREEITEM hti, BOOL bBold)
{
	m_tree.SetItemState(hti, bBold ? TVIS_BOLD : 0, TVIS_BOLD);
}

void CTreeCtrlHelper::SetItemStateEx(HTREEITEM hti, UINT nState, UINT nMask, BOOL bChildren)
{
	if (hti)
		m_tree.SetItemState(hti, nState, nMask);

	if (bChildren)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);
		
		while (htiChild)
		{
			SetItemStateEx(htiChild, nState, nMask, TRUE);
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
}

void CTreeCtrlHelper::SetTopLevelItemsBold(BOOL bBold)
{
	// clear all bold states
	SetItemStateEx(NULL, 0, TVIS_BOLD, TRUE);

	// set top items bold
	if (bBold)
	{
		HTREEITEM hti = m_tree.GetChildItem(NULL);
		
		while (hti)
		{
			SetItemBold(hti, TRUE);
			hti = m_tree.GetNextItem(hti, TVGN_NEXT);
		}
	}
}

HTREEITEM CTreeCtrlHelper::FindItem(DWORD dwID, HTREEITEM htiStart) const
{
	// try htiStart first
	if (htiStart && m_tree.GetItemData(htiStart) == dwID)
		return htiStart;
	
	// else try htiStart's children
	HTREEITEM htiFound = NULL;
	HTREEITEM htiChild = m_tree.GetChildItem(htiStart);
	
	while (htiChild && !htiFound)
	{
		htiFound = FindItem(dwID, htiChild);
		htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
	}
	
	return htiFound;
}

void CTreeCtrlHelper::BuildHTIMap(CHTIMap& mapHTI, HTREEITEM htiRoot) const
{
	mapHTI.RemoveAll();

	int nTreeCount = m_tree.GetCount();

	if (nTreeCount)
	{
		mapHTI.InitHashTable(nTreeCount);
		
		// traverse top-level items
		HTREEITEM hti = m_tree.GetChildItem(htiRoot);
		
		while (hti)
		{
			UpdateHTIMapEntry(mapHTI, hti);
			hti = m_tree.GetNextItem(hti, TVGN_NEXT);
		}
	}
}

void CTreeCtrlHelper::UpdateHTIMapEntry(CHTIMap& mapHTI, HTREEITEM hti) const
{
	// update our own mapping
	mapHTI[m_tree.GetItemData(hti)] = hti;
	
	// then our children
	HTREEITEM htiChild = m_tree.GetChildItem(hti);
	
	while (htiChild)
	{
		UpdateHTIMapEntry(mapHTI, htiChild);
		htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
	}
}

HTREEITEM CTreeCtrlHelper::GetLastChildItem(HTREEITEM hti) const
{
	HTREEITEM htiLast = NULL, htiChild = m_tree.GetChildItem(hti);

	while (htiChild)
	{
		htiLast = htiChild;
		htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
	}

	return htiLast;
}

HTREEITEM CTreeCtrlHelper::GetLastItem() const
{
	// we want the last child of the last child etc
	HTREEITEM htiLast = GetLastChildItem(NULL);
	HTREEITEM htiPrevLast = NULL;

	while (htiLast)
	{
		htiPrevLast = htiLast; // cache this
		htiLast = GetLastChildItem(htiLast); // last child of htiLast
	}

	return htiPrevLast;
}
