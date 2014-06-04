// ToDoCtrlListView.cpp : implementation file
//

#include "stdafx.h"
#include "TDCListView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDCListView

CTDCListView::CTDCListView()
{
}

CTDCListView::~CTDCListView()
{
}


BEGIN_MESSAGE_MAP(CTDCListView, CListCtrl)
	//{{AFX_MSG_MAP(CTDCListView)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
	ON_WM_SIZE()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDCListView message handlers

void CTDCListView::PreSubclassWindow()
{
	CListCtrl::PreSubclassWindow();

	ASSERT((GetStyle() & LVS_TYPEMASK) == LVS_REPORT);
	InitHeader();
}

void CTDCListView::OnSize(UINT nType, int cx, int cy)
{
	CListCtrl::OnSize(nType, cx, cy);
	InitHeader();
}

BOOL CTDCListView::InitHeader() const
{
	if (!m_header.GetSafeHwnd())
	{
		m_header.SubclassDlgItem(0, (CWnd*)this);
		m_header.EnableTracking(FALSE);
	}

	// else 
	return (m_header.GetSafeHwnd() != NULL);
}

int CTDCListView::GetHeaderHeight() const 
{
	if (!InitHeader())
		return -1;

	CRect rHeader;
	m_header.GetWindowRect(rHeader);
	ScreenToClient(rHeader);

	return rHeader.bottom;
}

BOOL CTDCListView::PtInHeader(CPoint ptScreen) const
{
	if (!InitHeader())
		return FALSE;

	CRect rHeader;
	m_header.GetWindowRect(rHeader);
		
	return rHeader.PtInRect(ptScreen);
}

void CTDCListView::RedrawHeaderColumn(int nColumn)
{
	if (InitHeader())
	{
		CRect rColumn;

		if (m_header.GetItemRect(nColumn, rColumn))
			m_header.InvalidateRect(rColumn, FALSE);
	}
}

void CTDCListView::SetColumnItemData(int nColumn, DWORD dwItemData)
{
	ASSERT(nColumn >= 0 && nColumn < GetColumnCount());

	if (InitHeader())
	{
		// set column item data
		HD_ITEM hdi = { 0 };
		hdi.mask = HDI_LPARAM;
		hdi.lParam = dwItemData;

		m_header.SetItem(nColumn, &hdi);
	}
}

int CTDCListView::GetColumnDrawAlignment(int nColumn) const
{
	ASSERT(nColumn >= 0 && nColumn < GetColumnCount());

	LVCOLUMN lvc = { LVCF_FMT, 0 };

	if (GetColumn(nColumn, &lvc))
	{
		switch (lvc.fmt & LVCFMT_JUSTIFYMASK)
		{
		case LVCFMT_CENTER:
			return DT_CENTER;

		case LVCFMT_RIGHT:
			return DT_RIGHT;
		}
	}

	// all else
	return DT_LEFT;
}

DWORD CTDCListView::GetColumnItemData(int nColumn) const
{
	ASSERT(m_header.GetSafeHwnd());

	if (InitHeader())
	{
		// set column item data
		HD_ITEM hdi = { HDI_LPARAM, 0 };

		if (m_header.GetItem(nColumn, &hdi))
			return hdi.lParam;
	}

	return 0;
}

int CTDCListView::GetColumnCount() const
{
	if (InitHeader())
		return m_header.GetItemCount();

	return 0;
}

void CTDCListView::RedrawHeader()
{
	if (InitHeader())
		m_header.Invalidate(FALSE);
}

int CTDCListView::SetCurSel(int nIndex)
{
	ASSERT (nIndex >= -1 && nIndex < GetItemCount());
	
	CRect rItem;
	
	UINT nMask = LVIS_SELECTED | LVIS_FOCUSED;

	// clear state of current selection
	int nCurSel = -1;
	POSITION pos = GetFirstSelectedItemPosition();

	if (pos)
		nCurSel = GetNextSelectedItem(pos);

	if (nCurSel != -1)
		SetItemState(nCurSel, 0, nMask);

	// set state of new item
	SetItemState(nIndex, nMask, nMask);

	return nIndex;
}

