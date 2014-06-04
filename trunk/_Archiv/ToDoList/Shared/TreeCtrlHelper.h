// TreeCtrlHelper.h: interface for the CTreeCtrlHelper class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TREECTRLHELPER_H__F6652DBE_3770_4E1C_A246_057AD6AD16B7__INCLUDED_)
#define AFX_TREECTRLHELPER_H__F6652DBE_3770_4E1C_A246_057AD6AD16B7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <afxtempl.h>

typedef CMap<HTREEITEM, HTREEITEM, int, int&> CMapIndices;
typedef CMap<DWORD, DWORD, HTREEITEM, HTREEITEM&> CHTIMap;

enum TCH_EDGE 
{ 
	TCHE_TOP, 
	TCHE_BOTTOM 
};

enum TCH_CHECK 
{ 
	TCHC_NONE		= 0, 
	TCHC_UNCHECKED	= INDEXTOSTATEIMAGEMASK(1), 
	TCHC_CHECKED	= INDEXTOSTATEIMAGEMASK(2), 
	TCHC_MIXED		= INDEXTOSTATEIMAGEMASK(3) 
};

class CTreeCtrlHelper  
{
public:
	CTreeCtrlHelper(CTreeCtrl& tree);
	virtual ~CTreeCtrlHelper();

	const CTreeCtrl& TreeCtrl() const { return m_tree; }
	CTreeCtrl& TreeCtrl() { return m_tree; }

	HTREEITEM InsertItem(LPCTSTR lpszItem, int nImage, int nSelImage, 
						LPARAM lParam, HTREEITEM htiParent, HTREEITEM htiAfter,
						BOOL bBold, BOOL bChecked);

	HTREEITEM InsertItem(LPCTSTR lpszItem, int nImage, int nSelImage, 
						LPARAM lParam, HTREEITEM htiParent, HTREEITEM htiAfter,
						BOOL bBold, TCH_CHECK nCheck = TCHC_NONE);
	
	BOOL HasFocus(BOOL bIncEditing = TRUE);
	int IsItemExpanded(HTREEITEM hti, BOOL bFully = FALSE) const; // TRUE, FALSE, -1 if no children
	void ExpandAll(BOOL bExpand = TRUE);
	void ExpandItem(HTREEITEM hti, BOOL bExpand = TRUE, BOOL bChildren = FALSE);
	BOOL IsParentItemExpanded(HTREEITEM hti, BOOL bRecursive = FALSE) const;

	void SetItemIntegral(HTREEITEM hti, int iIntegral);
	
	void SetItemChecked(HTREEITEM hti, BOOL bChecked); // 2 state
	void SetItemChecked(HTREEITEM hti, TCH_CHECK nChecked); // 3 state
	TCH_CHECK GetItemCheckState(HTREEITEM hti) const;

	BOOL SelectItem(HTREEITEM hti); // won't auto edit if item already selected
	inline void EndLabelEdit(BOOL bCancel) { SendMessage(m_tree, TVM_ENDEDITLABELNOW, bCancel, 0); }

	void InvalidateItem(HTREEITEM hti, BOOL bChildren = TRUE);
	void GetClientRect(LPRECT lpRect, HTREEITEM htiFrom);
	int GetItemHeight(HTREEITEM hti = NULL);

	int GetItemPos(HTREEITEM hti, HTREEITEM htiParent);
	int GetItemLevel(HTREEITEM hti);

	HTREEITEM FindItem(DWORD dwItemData, HTREEITEM htiStart = NULL) const;

	BOOL IsItemBold(HTREEITEM hti);
	void SetItemBold(HTREEITEM hti, BOOL bBold = TRUE);
	void SetTopLevelItemsBold(BOOL bBold = TRUE);

	HTREEITEM GetLastChildItem(HTREEITEM hti) const;
	HTREEITEM GetLastItem() const;

	void SetItemStateEx(HTREEITEM hti, UINT nState, UINT nMask, BOOL bChildren = FALSE);

	HTREEITEM GetTopLevelItem(HTREEITEM hti) const;
	HTREEITEM GetNextTopLevelItem(HTREEITEM hti, BOOL bNext = TRUE) const;

	// returns the top level item whose child is the first visible item (or itself)
	HTREEITEM GetFirstVisibleTopLevelItem(int& nPos); // return 0 if no items
	HTREEITEM GetFirstVisibleTopLevelItem(); // return 0 if no items

	int BuildVisibleIndexMap() const;
	BOOL ItemLineIsOdd(HTREEITEM hti) const;
	void ResetIndexMap() const;

	// return increments of item height
	void SetMinDistanceToEdge(HTREEITEM htiFrom, TCH_EDGE nToEdge, int nItems);
	int GetDistanceToEdge(HTREEITEM htiFrom, TCH_EDGE nToEdge) const;

	// get next/prev selectable items, NULL if none
	HTREEITEM GetNextPageVisibleItem(HTREEITEM hti) const;
	HTREEITEM GetPrevPageVisibleItem(HTREEITEM hti) const;

	HTREEITEM GetNextVisibleItem(HTREEITEM hti, BOOL bAllowChildren = TRUE) const;
	HTREEITEM GetPrevVisibleItem(HTREEITEM hti, BOOL bAllowChildren = TRUE) const;

	HTREEITEM GetNextItem(HTREEITEM hti, BOOL bAllowChildren = TRUE) const; // next item as-if all items were expanded
	HTREEITEM GetPrevItem(HTREEITEM hti, BOOL bAllowChildren = TRUE) const; // prev item as-if all items were expanded

	int FindItem(HTREEITEM htiFind, HTREEITEM htiStart); // return -1 for above, 1 for below, 0 if same
	BOOL IsFullyVisible(HTREEITEM hti) const;

	void EnsureVisibleEx(HTREEITEM hti, BOOL bVPartialOK = TRUE, BOOL bHPartialOK = TRUE);

	void BuildHTIMap(CHTIMap& mapHTI, HTREEITEM htiRoot = NULL) const;
	void UpdateHTIMapEntry(CHTIMap& mapHTI, HTREEITEM hti) const;

protected:
	CTreeCtrl& m_tree;
	mutable CMapIndices m_mapVisibleIndices;

protected:
	void AddVisibleItemToIndex(HTREEITEM hti) const;

};

#endif // !defined(AFX_TREECTRLHELPER_H__F6652DBE_3770_4E1C_A246_057AD6AD16B7__INCLUDED_)
