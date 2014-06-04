// ToolbarHelper.cpp: implementation of the CToolbarHelper class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ToolbarHelper.h"
#include "misc.h"

#include "enbitmap.h"
#include <afxpriv.h>        // for WM_KICKIDLE

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// simple helper class for determining whether commands are disabled or not

class CCmdUITH : public CCmdUI
{

public:
	CCmdUITH() { m_bEnabled = TRUE; }
	virtual void Enable(BOOL bOn = TRUE) { m_bEnabled = bOn; }
	virtual void SetCheck(int /*nCheck*/ = 1) {} // dummy
	virtual void SetRadio(BOOL /*bOn*/ = TRUE) {} // dummy
	virtual void SetText(LPCTSTR /*lpszText*/) {} // dummy

public:
	BOOL m_bEnabled;
};

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CToolbarHelper::CToolbarHelper() : m_pToolbar(NULL), m_bMultiline(FALSE), m_nMultilineWidth(200)
{

}

CToolbarHelper::~CToolbarHelper()
{

}

BOOL CToolbarHelper::Initialize(CToolBar* pToolbar, CWnd* pToolbarParent)
{
	if (!pToolbarParent || !HookWindow(*pToolbarParent))
		return FALSE;

	ASSERT_VALID(pToolbar);
	m_pToolbar = pToolbar;

	InitTooltips();

	return TRUE;
}

BOOL CToolbarHelper::Release(BOOL bClearDropBtns) 
{ 
	if (HookWindow(NULL) && ScHookWindow(NULL))
	{
		if (bClearDropBtns)
		{
			// iterate the buttons the hard way
			POSITION pos = m_mapTHButtons.GetStartPosition();

			while (pos)
			{
				THButton dm;
				UINT nCmdID = 0;

				m_mapTHButtons.GetNextAssoc(pos, nCmdID, dm);
				ClearDropButton(nCmdID, FALSE);
			}
		}

		m_pToolbar = NULL;
		m_mapTHButtons.RemoveAll();
		m_tt.DestroyWindow();

		return TRUE;
	}

	return FALSE;
}

BOOL CToolbarHelper::SetDropButton(UINT nBtnCmdID, UINT nMenuID, int nSubMenu, UINT nDefCmdID, char cHotkey)
{
	THButton dm;

	if (m_mapTHButtons.Lookup(nBtnCmdID, dm))
		return SetButton(nBtnCmdID, nMenuID, nSubMenu, nDefCmdID, cHotkey, dm.szTip);
	else
		return SetButton(nBtnCmdID, nMenuID, nSubMenu, nDefCmdID, cHotkey, _T(""));
}

BOOL CToolbarHelper::SetTooltip(UINT nBtnCmdID, UINT nIDTip)
{
	CString sTip;

	if (sTip.LoadString(nIDTip))
		return SetTooltip(nBtnCmdID, sTip);

	return FALSE;
}

BOOL CToolbarHelper::SetTooltip(UINT nBtnCmdID, LPCTSTR szTip)
{
	THButton dm;

	if (m_mapTHButtons.Lookup(nBtnCmdID, dm))
		return SetButton(nBtnCmdID, dm.nMenuID, dm.nSubMenu, dm.nDefCmdID, dm.cHotKey, szTip);
	else
		return SetButton(nBtnCmdID, 0, 0, 0, 0, szTip);
}

BOOL CToolbarHelper::SetButton(UINT nBtnCmdID, UINT nMenuID, int nSubMenu, UINT nDefCmdID, TCHAR cHotkey, LPCTSTR szTip)
{
	int nIndex = m_pToolbar->CommandToIndex(nBtnCmdID);

	if (nIndex == -1)
		return FALSE;

	if (nMenuID)
	{
		m_pToolbar->GetToolBarCtrl().SetExtendedStyle(TBSTYLE_EX_DRAWDDARROWS);

		DWORD dwStyle = m_pToolbar->GetButtonStyle(nIndex) | TBSTYLE_DROPDOWN;
		m_pToolbar->SetButtonStyle(nIndex, dwStyle);
	}

	THButton dm = { nMenuID, nSubMenu, nDefCmdID, cHotkey, 0 };

	//fabio_2005
#if _MSC_VER >= 1400
	_tcsncpy_s(dm.szTip, 128, szTip, sizeof(dm.szTip) - 1);
#else
	_tcsncpy(dm.szTip, szTip, sizeof(dm.szTip) - 1);
#endif

	m_mapTHButtons[nBtnCmdID] = dm;

	m_pToolbar->RedrawWindow();

	return TRUE;
}

BOOL CToolbarHelper::ClearDropButton(UINT nBtnCmdID, BOOL bRedraw)
{
	int nIndex = m_pToolbar->CommandToIndex(nBtnCmdID);

	if (nIndex == -1)
		return FALSE;

	DWORD dwStyle = m_pToolbar->GetButtonStyle(nIndex) & ~TBSTYLE_DROPDOWN;
	m_pToolbar->SetButtonStyle(nIndex, dwStyle);

	if (bRedraw)
		m_pToolbar->RedrawWindow();

	return TRUE;
}

BOOL CToolbarHelper::SetDefaultMenuID(UINT nBtnCmdID, UINT nDefCmdID)
{
	THButton dm;
	
	if (m_mapTHButtons.Lookup(nBtnCmdID, dm))
	{
		dm.nDefCmdID = nDefCmdID;
		m_mapTHButtons[nBtnCmdID] = dm;

		return TRUE;
	}

	return FALSE;
}

BOOL CToolbarHelper::DisplayDropMenu(UINT nCmdID, BOOL bPressBtn)
{
	// see if we have a menu for it
	THButton dm;
	
	if (m_mapTHButtons.Lookup(nCmdID, dm) && dm.nMenuID)
	{
		CMenu menu, *pSubMenu;
		
		if (menu.LoadMenu(dm.nMenuID))
		{
			pSubMenu = menu.GetSubMenu(dm.nSubMenu);
			
			if (pSubMenu)
			{
				PrepareMenuItems(pSubMenu, GetCWnd());
				pSubMenu->SetDefaultItem(dm.nDefCmdID);
				
				CRect rItem;
				int nIndex = m_pToolbar->CommandToIndex(nCmdID);

				m_pToolbar->GetItemRect(nIndex, rItem);
				m_pToolbar->ClientToScreen(rItem);
			
				if (bPressBtn)
					m_pToolbar->GetToolBarCtrl().PressButton(nCmdID, TRUE);

				pSubMenu->TrackPopupMenu(TPM_LEFTALIGN | TPM_LEFTBUTTON, rItem.left, rItem.bottom, GetCWnd());

				if (bPressBtn)
					m_pToolbar->GetToolBarCtrl().PressButton(nCmdID, FALSE);
				
				return TRUE; // we handled it
			}
		}
	}

	return FALSE;
}

LRESULT CToolbarHelper::WindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_NOTIFY:
		{
			LPNMHDR pNMHDR = (LPNMHDR)lp;
			
			switch (pNMHDR->code)
			{
			case TBN_DROPDOWN:
				// check its our toolbar
				if (pNMHDR->hwndFrom == m_pToolbar->GetSafeHwnd())
				{
					// load the menu
					LPNMTOOLBAR pNMTB = (LPNMTOOLBAR)pNMHDR;
					
					if (DisplayDropMenu((UINT)pNMTB->iItem))
						return FALSE; // we handled it
				}
				break;
				
#ifndef _UNICODE
			case TTN_NEEDTEXTA:
#else
			case TTN_NEEDTEXTW:
#endif
				{
					// to be thorough we will need to handle UNICODE versions of the message also !!
					TOOLTIPTEXT* pTTT = (TOOLTIPTEXT*)pNMHDR;

					// only handle this if it's not already been done
					if (pTTT->lpszText && *(pTTT->lpszText))
						break;
										
					UINT nID = pNMHDR->idFrom;
					
					if (pTTT->uFlags & TTF_IDISHWND) // idFrom is actually the HWND of the tool 
						nID = ::GetDlgCtrlID((HWND)nID);

					// get cursor pos
					CPoint point(::GetMessagePos());
					m_pToolbar->ScreenToClient(&point);
					
					// get tip
					static CString sTipText;
										
					sTipText = GetTip(nID, &point);
					
					if (!sTipText.IsEmpty()) // will be zero on a separator
					{
						pTTT->lpszText = (LPTSTR)(LPCTSTR)sTipText;
						return TRUE;
					}
				}
				break;

			case TTN_SHOW:
				{
					CWnd* pTooltipCtrl = CWnd::FromHandle(pNMHDR->hwndFrom);
					ASSERT (pTooltipCtrl);

					/*int nWidth = */pTooltipCtrl->SendMessage(TTM_SETMAXTIPWIDTH, 0, 
															m_bMultiline ? m_nMultilineWidth : UINT_MAX);
				}
				break;
			}
		}
		break;
	
	case WM_COMMAND:
		{
			HWND hCtrlFrom = (HWND)lp;

			// if m_pToolbar sent the command and we have a mapping for it then
			// change it to the default cmd for that button
			if (hCtrlFrom == *m_pToolbar)
			{
				THButton dm;
				UINT nCmdID = LOWORD(wp);

				if (m_mapTHButtons.Lookup(nCmdID, dm))
				{
					// if we have an enabled default command then send it
					if (dm.nDefCmdID && IsCmdEnabled(dm.nDefCmdID))
						wp = MAKEWPARAM(dm.nDefCmdID, HIWORD(wp));
					else
					{
						BOOL bRes = DisplayDropMenu(nCmdID, TRUE);

						if (bRes)
							return 0L; // we handled it
					}
				}
			}
		}
		break;

	case WM_KICKIDLE:
//	    m_pToolbar->OnUpdateCmdUI((CFrameWnd*)GetCWnd(), FALSE);
		break;

	case WM_DESTROY:
		{
			// must call rest of chain first
			LRESULT lr =  CSubclassWnd::WindowProc(hRealWnd, msg, wp, lp);
			HookWindow(NULL);
			return lr;
		}
	}

	return CSubclassWnd::WindowProc(hRealWnd, msg, wp, lp);
}

LRESULT CToolbarHelper::ScWindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_MOUSEMOVE:
	case WM_MOUSELEAVE:
		m_tt.RelayEvent(const_cast<MSG*>(GetCurrentMessage()));
		break;

	case WM_SIZE:
		{
			LRESULT lr = CSubclasser::ScWindowProc(hRealWnd, msg, wp, lp);
			RefreshTooltipRects();
			return lr;
		}
	}

	return CSubclasser::ScDefault(hRealWnd);
}

void CToolbarHelper::InitTooltips()
{
	if (!m_tt.Create(GetCWnd(), TTS_ALWAYSTIP))
		return;

	// hook the toolbar for mouse messages
	ScHookWindow(m_pToolbar->GetSafeHwnd());

	// turn off default tooltips
	m_pToolbar->SetBarStyle(m_pToolbar->GetBarStyle() & ~CBRS_TOOLTIPS);

	// and activate it
	m_tt.Activate(TRUE);

	// set up tools for each of the toolar buttons
	int nBtnCount = m_pToolbar->GetToolBarCtrl().GetButtonCount();

	for (int nBtn = 0; nBtn < nBtnCount; nBtn++)
	{
		if (m_pToolbar->GetItemID(nBtn) != ID_SEPARATOR)
		{
			CRect rBtn;
			m_pToolbar->GetItemRect(nBtn, rBtn);

			m_tt.AddTool(m_pToolbar, LPSTR_TEXTCALLBACK, rBtn, m_pToolbar->GetItemID(nBtn));
		}
	}
}

void CToolbarHelper::RefreshTooltipRects()
{
	int nBtnCount = m_pToolbar->GetToolBarCtrl().GetButtonCount();

	for (int nBtn = 0; nBtn < nBtnCount; nBtn++)
	{
		if (m_pToolbar->GetItemID(nBtn) != ID_SEPARATOR)
		{
			CRect rBtn;
			m_pToolbar->GetItemRect(nBtn, rBtn);

			m_tt.SetToolRect(m_pToolbar, m_pToolbar->GetItemID(nBtn), rBtn);
		}
	}
}

// static helper
void CToolbarHelper::PrepareMenuItems(CMenu* pMenu, CWnd* pWnd)
{
	// update item states
	CCmdUI state;
	
	state.m_pMenu = pMenu;
	state.m_nIndexMax = pMenu->GetMenuItemCount();
	
	for (state.m_nIndex = 0; state.m_nIndex < state.m_nIndexMax; state.m_nIndex++)
	{
		UINT nCmdID = pMenu->GetMenuItemID(state.m_nIndex);
		
		if (nCmdID == (UINT)-1) // submenu
		{
			CMenu* pSubMenu = pMenu->GetSubMenu(state.m_nIndex);

			if (pSubMenu)
				PrepareMenuItems(pSubMenu, pWnd);
		}
		else if (nCmdID != 0)
		{
			state.m_nID = nCmdID;
			pWnd->OnCmdMsg(state.m_nID, CN_UPDATE_COMMAND_UI, &state, NULL);
		}
	}
}

BOOL CToolbarHelper::IsCmdEnabled(UINT nCmdID) const
{
	CWnd* pParent = CWnd::FromHandle(GetHwnd());

	if (pParent)
	{
		CCmdUITH state;
		
		state.m_nIndexMax = 1;
		state.m_nIndex = 0;
		state.m_nID = nCmdID;

		pParent->OnCmdMsg(nCmdID, CN_UPDATE_COMMAND_UI, &state, NULL);

		return state.m_bEnabled;
	}

	return TRUE;
}

CString CToolbarHelper::GetTip(UINT nID, LPPOINT pPoint) const
{
	if (!nID)
		return ""; // separator

	// are we over the dropbutton?
	BOOL bOverDropBtn = FALSE;

	if (pPoint)
	{
		CSize sizeBtn(m_pToolbar->GetToolBarCtrl().GetButtonSize());
		CRect rButton;

		m_pToolbar->GetToolBarCtrl().GetRect(nID, rButton);
		rButton.left += sizeBtn.cx;

		if (rButton.PtInRect(*pPoint))
			bOverDropBtn = TRUE;
	}

	CString sTip;

	// do we have a mapping for this
	THButton dm;
				
	if (m_mapTHButtons.Lookup(nID, dm))
	{
		if (!bOverDropBtn)
		{
			// try the default item first
			if (dm.nDefCmdID && IsCmdEnabled(dm.nDefCmdID)/* && (dm.nDefCmdID != nID)*/)
			{
				sTip = GetTip(dm.nDefCmdID);

				if (!sTip.IsEmpty())
					return sTip;
			}
		}

		// try override tip
		if (lstrlen(dm.szTip))
			return dm.szTip;
	}

	return GetTip(nID);
}

CString CToolbarHelper::GetTip(UINT nID)
{
	CString sTip;

	if (nID > 0 && sTip.LoadString(nID) && !sTip.IsEmpty())
	{	
		// tip text starts at '\n' 
		int nStartTip = sTip.Find('\n');
		
		if (nStartTip != -1) 
			sTip = sTip.Right(sTip.GetLength() - nStartTip - 1);

		else // strip '...' if present
			sTip.Replace(_T("."), _T(""));

		sTip.TrimLeft();
		sTip.TrimRight();
	}
	
	return sTip;
}

void CToolbarHelper::EnableMultilineText(BOOL bEnable, int nWidth)
{
	m_bMultiline = bEnable;
	m_nMultilineWidth = nWidth;
}

BOOL CToolbarHelper::ProcessMessage(MSG* pMsg)
{
	if (!IsWindowEnabled() || !m_pToolbar->IsWindowEnabled())
		return FALSE;

	// see if key press matches any hotkey
	if (pMsg->message == WM_SYSKEYDOWN && pMsg->wParam != VK_MENU && Misc::KeyIsPressed(VK_MENU))
	{
		int nKey = (int)pMsg->wParam;
		char cLower = 0, cUpper = 0;

		if (islower(nKey))
		{
			cLower = (char)nKey;
			cUpper = (char)toupper(nKey);
		}
		else if (isupper(nKey))
		{
			cUpper = (char)nKey;
			cLower = (char)tolower(nKey);
		}
		else
			cUpper = cLower = (char)nKey;

		// iterate the buttons the hard way
		POSITION pos = m_mapTHButtons.GetStartPosition();

		while (pos)
		{
			THButton dm;
			UINT nCmdID = 0;

			m_mapTHButtons.GetNextAssoc(pos, nCmdID, dm);

			if (nCmdID && (dm.cHotKey == cLower || dm.cHotKey == cUpper))
			{
				DisplayDropMenu(nCmdID, TRUE);
				return TRUE;
			}
		}
	}

	return FALSE;
}

