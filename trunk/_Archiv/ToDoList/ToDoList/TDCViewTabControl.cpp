// ToDoCtrlViewTabControl.cpp : implementation file
//

#include "stdafx.h"
#include "TDCViewTabControl.h"
#include "tdcmsg.h"
#include "resource.h"

#include "..\shared\deferwndmove.h"
#include "..\shared\misc.h"
#include "..\shared\holdredraw.h"
#include "..\shared\dialoghelper.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDCViewTabControl

CTDCViewTabControl::CTDCViewTabControl(DWORD dwStyles) 
: 
	CTabCtrlEx(TCE_ALL, e_tabBottom), 
	m_nSelIndex(FTCV_UNSET),
	m_dwStyles(dwStyles),
	m_bShowingTabs(TRUE)
{
}

CTDCViewTabControl::~CTDCViewTabControl()
{
	// cleanup view data
	int nIndex = m_aViews.GetSize();

	while (nIndex--)
	{
		delete m_aViews[nIndex].pData;
		m_aViews[nIndex].pData = NULL;
	}
}


BEGIN_MESSAGE_MAP(CTDCViewTabControl, CTabCtrlEx)
	//{{AFX_MSG_MAP(CTDCViewTabControl)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
	ON_NOTIFY_REFLECT(TCN_SELCHANGE, OnSelchangeTabcontrol)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDCViewTabControl message handlers

BOOL CTDCViewTabControl::AttachView(HWND hWnd, FTC_VIEW nView, LPCTSTR szLabel, HICON hIcon, void* pData)
{
	ASSERT (hWnd == NULL || ::IsWindow(hWnd));

	// window and enum must be unique
	if ((hWnd && FindView(hWnd) != -1) || (FindView(nView) != -1))
		return FALSE;

	// prepare tab bar
	if (hIcon)
	{
		if (!m_ilTabs.GetSafeHandle())
		{
			if (m_ilTabs.Create(16, 16, ILC_COLOR32 | ILC_MASK, 2, 1))
				SetImageList(&m_ilTabs);
		}

		if (m_ilTabs.GetSafeHandle())
			m_ilTabs.Add(hIcon);
		else
			hIcon = NULL;
	}

	TDCVIEW view(hWnd, nView, szLabel, pData);
	int nIndex = m_aViews.Add(view);
	int nIcon = m_ilTabs.GetImageCount() - 1;

	if (GetSafeHwnd())
		InsertItem(TCIF_TEXT | TCIF_PARAM | TCIF_IMAGE, nIndex, view.sViewLabel, nIcon, (LPARAM)nView);

	// set initial tab to this if not yet set
//	if (m_nSelIndex == -1)
//		m_nSelIndex = nIndex;

	return TRUE;
}

BOOL CTDCViewTabControl::DetachView(HWND hWnd)
{
	int nFind = FindView(hWnd);

	if (nFind != -1)
	{
		m_aViews.RemoveAt(nFind);
		return TRUE;
	}

	return FALSE;
}

BOOL CTDCViewTabControl::DetachView(FTC_VIEW nView)
{
	int nFind = FindView(nView);

	if (nFind != -1)
	{
		m_aViews.RemoveAt(nFind);
		return TRUE;
	}

	return FALSE;
}

void CTDCViewTabControl::GetViewRect(TDCVIEW& view, CRect& rPos) const
{
	CWnd* pWndView = GetViewWnd(view);

	pWndView->GetWindowRect(rPos);
	pWndView->GetParent()->ScreenToClient(rPos);
}

CWnd* CTDCViewTabControl::GetViewWnd(TDCVIEW& view) const
{
	return CWnd::FromHandle(view.hwndView);
}

BOOL CTDCViewTabControl::SwitchToTab(int nIndex)
{
	ASSERT (GetSafeHwnd());

	if (GetSafeHwnd() == NULL)
		return FALSE;

	if (nIndex == m_nSelIndex || nIndex < 0 || nIndex >= m_aViews.GetSize())
		return FALSE;

	// make sure we have a valid HWND to switch to
	TDCVIEW& newView = m_aViews[nIndex];
	ASSERT(newView.hwndView);

	if (newView.hwndView == NULL)
		return FALSE;

	CRect rView(0, 0, 0, 0);

	if (m_nSelIndex == -1)
	{
		// first time so hide all other views
		int nOther = m_aViews.GetSize();

		while (nOther--)
		{
			if (nOther != nIndex)
				::ShowWindow(m_aViews[nOther].hwndView, SW_HIDE);
		}

		// and if the index > 0, grab the [0] view rect as the best hint
		if (nIndex > 0)
			GetViewRect(m_aViews[0], rView);
	}
	else // just hide the currently visible view
	{
		TDCVIEW& oldView = m_aViews[m_nSelIndex];
		GetViewRect(oldView, rView);
	}

	if (!rView.IsRectEmpty())
		::MoveWindow(newView.hwndView, rView.left, rView.top, rView.Width(), rView.Height(), FALSE);

	::ShowWindow(newView.hwndView, SW_SHOW);

	if (m_nSelIndex != -1)
		::ShowWindow(m_aViews[m_nSelIndex].hwndView, SW_HIDE);

	m_nSelIndex = nIndex;
	SetCurSel(nIndex);

	return TRUE;
}

CWnd* CTDCViewTabControl::GetActiveWnd() const
{
	ASSERT (GetSafeHwnd());

	if (GetSafeHwnd() == NULL)
		return NULL;

	int nIndex = GetCurSel();

	return (nIndex == -1) ? NULL : CWnd::FromHandle(m_aViews[nIndex].hwndView);
}

FTC_VIEW CTDCViewTabControl::GetActiveView() const
{
	return (m_nSelIndex == FTCV_UNSET) ? FTCV_UNSET : m_aViews[m_nSelIndex].nView;
}

void* CTDCViewTabControl::GetActiveViewData() const
{
	return (m_nSelIndex == FTCV_UNSET) ? NULL : m_aViews[m_nSelIndex].pData;
}

HWND CTDCViewTabControl::GetViewHwnd(FTC_VIEW nView) const
{
	int nIndex = FindView(nView);

	if (nIndex < 0 || nIndex > m_aViews.GetSize())
		return NULL;

	return m_aViews[nIndex].hwndView;
}

CString CTDCViewTabControl::GetViewName(FTC_VIEW nView) const
{
	int nIndex = FindView(nView);

	if (nIndex < 0 || nIndex > m_aViews.GetSize())
		return _T("");

	return m_aViews[nIndex].sViewLabel;
}

BOOL CTDCViewTabControl::SetViewHwnd(FTC_VIEW nView, HWND hWnd)
{
	int nIndex = FindView(nView);

	if (nIndex < 0 || nIndex > m_aViews.GetSize())
		return NULL;

	ASSERT (m_aViews[nIndex].hwndView == NULL);

	if (m_aViews[nIndex].hwndView)
		return FALSE;

	m_aViews[nIndex].hwndView = hWnd;
	return TRUE;
}

void* CTDCViewTabControl::GetViewData(FTC_VIEW nView) const
{
	int nIndex = FindView(nView);

	return (nIndex == -1) ? NULL : m_aViews[nIndex].pData;
}

FTC_VIEW CTDCViewTabControl::GetView(int nIndex) const
{
	if (nIndex < 0 || nIndex > m_aViews.GetSize())
		return FTCV_UNSET;

	// else
	return m_aViews[nIndex].nView;
}

BOOL CTDCViewTabControl::SetActiveView(CWnd* pWnd, BOOL bNotify)
{
	int nOldIndex = m_nSelIndex;
	int nNewIndex = FindView(pWnd->GetSafeHwnd());

	return DoTabChange(nOldIndex, nNewIndex, bNotify);
}

BOOL CTDCViewTabControl::SetActiveView(FTC_VIEW nView, BOOL bNotify)
{
	int nOldIndex = m_nSelIndex;
	int nNewIndex = FindView(nView);

	return DoTabChange(nOldIndex, nNewIndex, bNotify);
}

void CTDCViewTabControl::Resize(const CRect& rect, CDeferWndMove* pDWM, LPRECT prcView)
{
	CRect rTabs, rView;

	if (CalcTabViewRects(rect, rTabs, rView))
	{
		CWnd* pView = GetActiveWnd();
		ASSERT(pView);

		if (pView)
		{
			if (pDWM)
			{
				pDWM->MoveWindow(this, rTabs);
				pDWM->MoveWindow(pView, rView);
			}
			else
			{
				MoveWindow(rTabs);
				pView->MoveWindow(rView);
			}
		}

		if (prcView)
			*prcView = rView;
	}
}

int CTDCViewTabControl::FindView(HWND hWnd) const
{
	int nIndex = m_aViews.GetSize();

	while (nIndex--)
	{
		if (m_aViews[nIndex].hwndView == hWnd)
			return nIndex;
	}

	// else
	return -1;
}

int CTDCViewTabControl::FindView(FTC_VIEW nView) const
{
	int nIndex = m_aViews.GetSize();

	while (nIndex--)
	{
		if (m_aViews[nIndex].nView == nView)
			return nIndex;
	}

	// else
	return -1;
}

void CTDCViewTabControl::PreSubclassWindow() 
{
	CXPTabCtrl::PreSubclassWindow();

	ShowTabControl(m_bShowingTabs);

	// add tabs
	int nNumView = m_aViews.GetSize();

	for (int nView = 0; nView < nNumView; nView++)
	{
		const TDCVIEW& view = m_aViews[nView];
		InsertItem(TCIF_TEXT | TCIF_PARAM | TCIF_IMAGE, nView, view.sViewLabel, nView, (LPARAM)view.nView);
	}

	SwitchToTab(0);
}

void CTDCViewTabControl::ShowTabControl(BOOL bShow) 
{ 
	m_bShowingTabs = bShow; 

	if (GetSafeHwnd())
	{
		ShowWindow(bShow ? SW_SHOW : SW_HIDE);
		EnableWindow(bShow);
	}
}

BOOL CTDCViewTabControl::CalcTabViewRects(const CRect& rPos, CRect& rTabs, CRect& rView)
{
	if (!GetSafeHwnd())
		return FALSE;

	if (m_bShowingTabs)
	{
		rTabs = rPos;
		rTabs.bottom = rTabs.top;
		AdjustRect(TRUE, rTabs);

		int nTabHeight = rTabs.Height();
		rTabs = rView = rPos;

		switch (GetOrientation())
		{
			case e_tabTop:	  
				rTabs.bottom = rTabs.top + nTabHeight - 5;
				rTabs.right += 2;
				rView.top = rTabs.bottom;
				break;

			case e_tabBottom: 
				rTabs.top = rTabs.bottom - nTabHeight + 5;
				rTabs.right += 2;
				rView.bottom = rTabs.top;
				break;

			case e_tabLeft:	  
				rTabs.right = rTabs.left + nTabHeight; 
				rView.left = rTabs.right;
				break;

			case e_tabRight:  
				rTabs.left = rTabs.right - nTabHeight;
				rView.right = rTabs.left;
				break;

			default:
				return FALSE;
		}
	}
	else
	{
		rTabs.SetRectEmpty();
		rView = rPos;
	}

	return TRUE;
}

void CTDCViewTabControl::OnSelchangeTabcontrol(NMHDR* /*pNMHDR*/, LRESULT* pResult) 
{
	int nOldIndex = m_nSelIndex;
	int nNewIndex = GetCurSel();

	if (DoTabChange(nOldIndex, nNewIndex, TRUE) == FALSE)
		SetCurSel(nOldIndex);

	*pResult = 0;
}

void CTDCViewTabControl::ActivateNextView(BOOL bNext)
{
	int nCurView = FindView(GetActiveView());
	int nOrgView = nCurView, nNewView = nCurView;

	// keep iterating until we find a valid view or until
	// we return to the start
	do
	{
		if (bNext)
		{
			nNewView++;

			if (nNewView == m_aViews.GetSize())
				nNewView = 0;
		}
		else
		{
			nNewView--;

			if (nNewView == -1)
				nNewView = (m_aViews.GetSize() - 1);
		}
		ASSERT (nNewView >= 0 && nNewView < m_aViews.GetSize());
	}
	while (nNewView != nOrgView && DoTabChange(nCurView, nNewView, TRUE) == FALSE);
}

BOOL CTDCViewTabControl::DoTabChange(int nOldIndex, int nNewIndex, BOOL bNotify)
{
	ASSERT (nOldIndex != nNewIndex);

	if (nOldIndex == nNewIndex)
		return FALSE;

	// check if the previous view had the focus
	HWND hOldView = (nOldIndex != -1) ? m_aViews[nOldIndex].hwndView : NULL;
	BOOL bHadFocus = TRUE;

	if (hOldView)
	{
		HWND hFocus = ::GetFocus();
		bHadFocus = (hFocus == NULL) || CDialogHelper::IsChildOrSame(hOldView, hFocus);
	}

	// notify parent before the change
	if (bNotify)
	{
		BOOL bCancel = GetParent()->SendMessage(WM_TDCN_VIEWPRECHANGE, GetView(nOldIndex), GetView(nNewIndex));

		if (bCancel)
			return FALSE;
	}

	if (SwitchToTab(nNewIndex))
	{
		ASSERT (nOldIndex != m_nSelIndex);

		// notify parent after the change
		if (bNotify)
			GetParent()->SendMessage(WM_TDCN_VIEWPOSTCHANGE, GetView(nOldIndex), GetView(nNewIndex));

		// restore focus		
		HWND hNewView = m_aViews[nNewIndex].hwndView;

		if (bHadFocus)
			::SetFocus(hNewView);
	}

	return TRUE;
}

DWORD CTDCViewTabControl::ModifyStyles(DWORD dwStyle, BOOL bAdd)
{
	if (bAdd)
		m_dwStyles |= dwStyle;
	else
		m_dwStyles &= dwStyle;

	return m_dwStyles;
}
