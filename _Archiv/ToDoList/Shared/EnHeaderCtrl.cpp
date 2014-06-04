//_ **********************************************************
//_ 
//_ Name: EnHeaderCtrl.cpp 
//_ Purpose: 
//_ Created: 15 September 1998 
//_ Author: D.R.Godson
//_ Modified By: 
//_ 
//_ Copyright (c) 1998 Brilliant Digital Entertainment Inc. 
//_ 
//_ **********************************************************

// EnHeaderCtrl.cpp : implementation file
//

#include "stdafx.h"
#include "EnHeaderCtrl.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CEnHeaderCtrl

CEnHeaderCtrl::CEnHeaderCtrl() : m_nRowCount(1), m_bAllowTracking(TRUE)
{
}

CEnHeaderCtrl::~CEnHeaderCtrl()
{
}


BEGIN_MESSAGE_MAP(CEnHeaderCtrl, CHeaderCtrl)
	//{{AFX_MSG_MAP(CEnHeaderCtrl)
	ON_WM_SETCURSOR()
	ON_WM_LBUTTONDOWN()
	ON_WM_CONTEXTMENU()
	//}}AFX_MSG_MAP
	ON_WM_LBUTTONDBLCLK()
	ON_NOTIFY_REFLECT(HDN_BEGINTRACK, OnBeginTrackHeader)
	ON_MESSAGE(HDM_LAYOUT, OnLayout)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CEnHeaderCtrl message handlers

void CEnHeaderCtrl::SetRowCount(int nRows)
{
	if (nRows <= 0 || nRows == m_nRowCount)
		return;

	m_nRowCount = nRows;

	// force resize
	if (GetSafeHwnd())
		SetWindowPos(NULL, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER); 
}

LRESULT CEnHeaderCtrl::OnLayout(WPARAM wp, LPARAM lp)
{
	// default processing
	LRESULT lResult = CHeaderCtrl::DefWindowProc(HDM_LAYOUT, wp, lp);

	// our handling
	if (m_nRowCount > 1)
	{
		HD_LAYOUT* pLayout = (HD_LAYOUT*)lp;
		int nHeight = (int)(((0.6 * m_nRowCount) + 0.4) * pLayout->pwpos->cy);

		pLayout->pwpos->cy = nHeight;
		pLayout->prc->top = nHeight;

		Invalidate(TRUE);
	}

	return lResult;
}

void CEnHeaderCtrl::OnBeginTrackHeader(NMHDR* /*pNMHDR*/, LRESULT* /*pResult*/) 
{
	// this no longer seems to work with vc++5 because the message never seems to get
	// here. instead we do some trickery in the OnLButtonDown handler
//	*pResult = !m_bAllowTracking;
}

BOOL CEnHeaderCtrl::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
{
	if (m_bAllowTracking)
		return CHeaderCtrl::OnSetCursor(pWnd, nHitTest, message);
	else
	{
		::SetCursor(AfxGetApp()->LoadStandardCursor(IDC_ARROW));
		return FALSE;
	}
}

int CEnHeaderCtrl::AddItem(int nCol, int nWidth, LPCTSTR szText, int nFormat, UINT uIDBitmap)
{
	ASSERT (nCol >= 0 && nWidth > 0);

	HD_ITEM hdi = { 0 };
	CBitmap bm;

	// mask
	hdi.mask = HDI_WIDTH | HDI_FORMAT | HDI_TEXT;
	hdi.mask |= uIDBitmap ? HDI_BITMAP : 0;
	
	// format
	hdi.fmt = nFormat;

	// bitmap
	if (uIDBitmap)
	{
		bm.LoadBitmap(uIDBitmap);
		hdi.hbm = (HBITMAP)bm.GetSafeHandle();
		bm.Detach();
	}
	
	// width
	hdi.cxy = nWidth;

	// text
	hdi.pszText = (LPTSTR)szText;

	return InsertItem(nCol, &hdi);
}

BOOL CEnHeaderCtrl::SetItem(int nCol, HDITEM* pHeaderItem)
{
	return CHeaderCtrl::SetItem(nCol, pHeaderItem);
}

BOOL CEnHeaderCtrl::SetItem(int nCol, int nWidth, LPCTSTR szText, DWORD dwData)
{
	ASSERT (nCol >= 0 && nCol < GetItemCount());
	HD_ITEM hdi = { 0 };

	hdi.mask = HDI_WIDTH | HDI_LPARAM | HDI_TEXT;
	hdi.cxy = nWidth;
	hdi.pszText = (LPTSTR)szText;
	hdi.lParam = dwData;

	return SetItem(nCol, &hdi);
}

BOOL CEnHeaderCtrl::SetItemWidth(int nCol, int nWidth)
{
	ASSERT (nCol >= 0 && nCol < GetItemCount());
	HD_ITEM hdi = { 0 };

	hdi.mask = HDI_WIDTH;
	hdi.cxy = nWidth;

	return SetItem(nCol, &hdi);
}

int CEnHeaderCtrl::GetItemWidth(int nCol) const
{
	ASSERT (nCol >= 0 && nCol < GetItemCount());
	HD_ITEM hdi = { 0 };

	hdi.mask = HDI_WIDTH;
	GetItem(nCol, &hdi);

	return hdi.cxy;
}

BOOL CEnHeaderCtrl::SetItemData(int nCol, DWORD dwData)
{
	ASSERT (nCol >= 0 && nCol < GetItemCount());
	HD_ITEM hdi = { 0 };

	hdi.mask = HDI_LPARAM;
	hdi.lParam = dwData;

	return SetItem(nCol, &hdi);
}

DWORD CEnHeaderCtrl::GetItemData(int nCol) const
{
	ASSERT (nCol >= 0 && nCol < GetItemCount());
	HD_ITEM hdi = { 0 };

	hdi.mask = HDI_LPARAM;
	GetItem(nCol, &hdi);

	return hdi.lParam;
}

BOOL CEnHeaderCtrl::SetItemText(int nCol, LPCTSTR szText)
{
	ASSERT (nCol >= 0 && nCol < GetItemCount());
	HD_ITEM hdi = { 0 };

	hdi.mask = HDI_TEXT;
	hdi.pszText = (LPTSTR)szText;

	return SetItem(nCol, &hdi);
}

CString CEnHeaderCtrl::GetItemText(int nCol) const
{
	ASSERT (nCol >= 0 && nCol < GetItemCount());
	HD_ITEM hdi = { 0 };

	TCHAR szText[128];

	hdi.mask = HDI_TEXT;
	hdi.pszText = szText;
	hdi.cchTextMax = 127;

	GetItem(nCol, &hdi);

	return hdi.pszText;
}

void CEnHeaderCtrl::OnLButtonDblClk(UINT nFlags, CPoint point) 
{
	// eat the message if not tracking enabled
	if (m_bAllowTracking)
		CHeaderCtrl::OnLButtonDblClk(nFlags, point);
}

void CEnHeaderCtrl::OnLButtonDown(UINT nFlags, CPoint point) 
{
	// see OnBeginTrackHeader for why the following trickery is required
	if (!m_bAllowTracking)
	{
		// check to see whether the mouse is over the divider and if it is
		// shift the y coord off the header 
		int nCol = 0;
		int nDivider = 0;

		while (nCol < GetItemCount())
		{
			nDivider += GetItemWidth(nCol);

			if ((point.x >= nDivider - 8) && (point.x <= nDivider + 8))
			{
				point.y = -50;
				break;
			}

			// next col
			nCol++;
		}
		// do default processing
		CHeaderCtrl::DefWindowProc(WM_LBUTTONDOWN, (WPARAM)nFlags, MAKELPARAM(point.x, point.y));
	}
	else
		CHeaderCtrl::OnLButtonDown(nFlags, point);
}

void CEnHeaderCtrl::OnContextMenu(CWnd* /*pWnd*/, CPoint /*point*/) 
{
	// do nothing	
}
