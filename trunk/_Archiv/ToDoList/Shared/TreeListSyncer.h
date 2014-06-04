#pragma once

#include "..\shared\subclass.h"

#include <wtypes.h>
#include <afxtempl.h>

enum TLS_LINKAGE
{
	TLSL_LEFTDATA_IS_RIGHTITEM,
	TLSL_RIGHTDATA_IS_LEFTITEM,
	TLSL_LEFTDATA_IS_RIGHTDATA,
	TLSL_RIGHTDATA_IS_LEFTDATA,
};

enum
{
	TLSF_SYNCSELECTION	= 0x01,
	TLSF_SYNCFOCUS		= 0x02,
	TLSF_MULTISELECTION	= 0x04,
};

typedef CArray<HTREEITEM, HTREEITEM&> CHTIArray;

class CTreeListSyncer : protected CSubclassWnd, protected CSubclasser
{
public:
	CTreeListSyncer(DWORD dwFlags = TLSF_SYNCSELECTION | TLSF_SYNCFOCUS);
	virtual ~CTreeListSyncer();
	
	BOOL Sync(HWND hwndLeft, HWND hwndRight, TLS_LINKAGE nLink, HWND hwndTreeHeader = NULL);
	BOOL IsSyncing() const;
	void Unsync();
	
	BOOL Resync(HWND hwnd, HWND hwndTo);
	void PostResync(HWND hwnd);
	void EnableResync(BOOL bEnable, HWND hwnd = NULL);
	
	void Resize(const CRect& rect, float fLeftProportion = 0.5f);
	void Resize(const CRect& rLeft, const CRect& rRight);
	void Resize(const CRect& rect, int nLeftWidth);
	
	void Invalidate(BOOL bErase = TRUE);
	BOOL HasFocus() const;
	BOOL HandleEraseBkgnd(CDC* pDC);

	HTREEITEM GetTreeSelItem() const;
	int GetTreeSelItems(CHTIArray& aItems) const;
	void SetTreeSelItems(const CHTIArray& aItems, HTREEITEM htiFocus = NULL);
	void SelectTreeItem(HTREEITEM hti, BOOL bClear = TRUE);

	typedef int (CALLBACK *PFNCOMPARE)(LPARAM, LPARAM, LPARAM);
	void Sort(PFNCOMPARE pfnCompare, DWORD dwData);
	
protected:
	// all tree/list window hooking gets routed to this function
	LRESULT ScWindowProc(HWND hwnd, UINT msg, WPARAM wp, LPARAM lp);
	
	// parent of tree/list gets routed here
	LRESULT WindowProc(HWND hwnd, UINT msg, WPARAM wp, LPARAM lp);
		
protected:
	HWND m_hwndTreeHeader;
	TLS_LINKAGE m_nLinkage;
	BOOL m_bResyncing, m_bResyncEnabled, m_bTreeExpanding;
	BOOL m_bResyncPending, m_bResizePending;
	CSubclassWnd m_scLeft, m_scRight;
	DWORD m_dwFlags;

	// hack for tree selection bug under certain circumstances
	HTREEITEM m_htiSyncSelFix;

#ifdef _DEBUG
	static int GetListSelItem(HWND hwnd, CString& sText);
	static HTREEITEM GetTreeSelItem(HWND hwnd, CString& sText);
	
	int GetListItem(HWND hwndList, HWND hwndTree, HTREEITEM hti, CString& sText) const;
	HTREEITEM GetTreeItem(HWND hwndTree, HWND hwndList, int nItem, CString& sText) const;

	void TraceTreeSelection(LPCTSTR szLocation = NULL) const;
	void TraceListSelection(HWND hwnd, LPCTSTR szLocation = NULL) const;
	void TraceResync(HWND hwnd, HWND hwndTo) const;
#endif
	
	static BOOL IsList(HWND hwnd);
	static BOOL IsTree(HWND hwnd);
	static void ResyncListHeader(HWND hwnd);
	static int GetItemHeight(HWND hwnd);
	static BOOL HasVScrollBar(HWND hwnd);
	static BOOL HasHScrollBar(HWND hwnd);
	static BOOL IsItemSelected(HWND hwnd, int nItem);
	static DWORD GetTreeItemData(HWND hwnd, HTREEITEM hti);
	static DWORD GetListItemData(HWND hwnd, int nItem);
	static int GetListSelItem(HWND hwnd);
	static int GetListFocusItem(HWND hwnd);
	static HTREEITEM GetTreeSelItem(HWND hwnd);
	static HTREEITEM GetTreeFocusItem(HWND hwnd);
	static HTREEITEM FindTreeItem(HWND hwndTree, DWORD dwItemData);
	static HTREEITEM FindTreeItem(HWND hwndTree, HTREEITEM hti, DWORD dwItemData);
	static HTREEITEM FindVisibleTreeItem(HWND hwndTree, DWORD dwItemData);
	static int FindListItem(HWND hwnd, DWORD dwItemData);
	static BOOL IsListItemSelected(HWND hwnd, int nItem);
	static BOOL IsTreeFullRowSelect(HWND hwnd);
	static BOOL IsListFullRowSelect(HWND hwnd);
	static BOOL TreeHasExpandedItem(HWND hwnd, HTREEITEM hti);
	static BOOL IsTreeItemExpanded(HWND hwnd, HTREEITEM hti);
	static BOOL TreeItemHasState(HWND hwnd, HTREEITEM hti, UINT nStateMask);
	static void SetTreeItemState(HWND hwnd, HTREEITEM hti, UINT nState, UINT nStateMask);
	static BOOL ListItemHasState(HWND hwnd, int nItem, UINT nStateMask);
	static void ForceNcCalcSize(HWND hwnd);
	static int GetTreeSelItems(HWND hwnd, HTREEITEM hti, CHTIArray& aItems);
	static void ClearTreeSelection(HWND hwnd);
	static void SetTreeSelItems(HWND hwnd, const CHTIArray& aItems, HTREEITEM htiFocus = NULL);
	static void SelectTreeItem(HWND hwnd, HTREEITEM hti, BOOL bClear = TRUE);
	static void InvalidateTreeItem(HWND hwnd, HTREEITEM hti);

	static DWORD GetStyle(HWND hwnd);
	static BOOL HasStyle(HWND hwnd, DWORD dwStyle);
	static BOOL HasFocus(HWND hwnd);
	
	void Resize(HWND hwnd, const CRect& rect, BOOL bInternal);
	void RefreshSize(HWND hwnd);
	void GetBoundingRect(HWND hwnd, CRect& rect) const;
	int GetHeaderHeight(HWND hwnd) const;
	BOOL GetHeaderRect(HWND hwnd, CRect& rect) const;
	HWND OtherWnd(HWND hwnd) const;
	HWND GetTree() const;
	BOOL IsTreeHeader(HWND hwnd) const;
	BOOL IsLeft(HWND hwnd) const;
	BOOL IsRight(HWND hwnd) const;
	HWND Left() const { return m_scLeft.GetHwnd(); }
	HWND Right() const { return m_scRight.GetHwnd(); }
	int GetListItem(HWND hwndList, HWND hwndTree, HTREEITEM hti) const;
	int GetListItem(HWND hwndList1, HWND hwndList2, int nItem2) const;
	HTREEITEM GetTreeItem(HWND hwndTree, HWND hwndList, int nItem) const;
	BOOL ShowVScrollBar(HWND hwnd, BOOL bShow = TRUE, BOOL bRefreshSize = TRUE);
	BOOL IsTreeItemSelected(HWND hwnd, HTREEITEM hti) const;
	BOOL Resync(HWND hwnd, HWND hwndTo, BOOL bIncSelection);
	void PostResync(HWND hwnd, BOOL bIncSelection);
	void PostResize(HWND hwnd);
	int GetTreeSelectionCount() const;
	
private:
	// for sorting list from tree
	typedef CMap<DWORD, DWORD, int, int> CSortMap;
	
	void BuildSortMap(HWND hwndTree, CSortMap& map);
	void BuildSortMap(HWND hwndTree, HTREEITEM hti, CSortMap& map, int& nIndex);
	void SortTreeItem(HWND hwndTree, HTREEITEM hti, PFNCOMPARE pfnCompare, DWORD dwData);

	static int CALLBACK SortListProc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);
};
