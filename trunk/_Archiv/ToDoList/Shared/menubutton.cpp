// menubutton.cpp : implementation file
//

#include "stdafx.h"
#include "menubutton.h"
#include "graphicsmisc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMenuButton

CMenuButton::CMenuButton(UINT nMenuID, int nSubMenu, MENUBTN_STYLE nStyle) 
: m_nMenuID(nMenuID), m_nSubMenu(nSubMenu), m_nStyle(nStyle)
{
}

CMenuButton::~CMenuButton()
{
}


BEGIN_MESSAGE_MAP(CMenuButton, CCustomButton)
	//{{AFX_MSG_MAP(CMenuButton)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMenuButton message handlers

void CMenuButton::SetWindowText(LPCTSTR lpszString)
{
	if (lpszString && *lpszString)
	{
		CString sText(lpszString);

		// add some padding
		DWORD dwStyle = GetStyle();

		if (dwStyle & BS_RIGHT)
			sText += _T("  ");
	
		else if (dwStyle & BS_LEFT)
			sText = _T(" ") + sText;

		CCustomButton::SetWindowText(sText);
	}
	else
		CCustomButton::SetWindowText(lpszString);
}

void CMenuButton::SetWindowText(TCHAR nChar)
{
	TCHAR szString[2] = { nChar, 0 };
	SetWindowText(szString);
}

void CMenuButton::DoExtraPaint(CDC* pDC, const CRect& rExtra)
{
	switch (m_nStyle)
	{
	case MBS_DOWN:
		GraphicsMisc::DrawSymbol(pDC, 0x36, rExtra, DT_RIGHT, &GraphicsMisc::Marlett());
		break;

	case MBS_RIGHT:
		GraphicsMisc::DrawSymbol(pDC, 0x34, rExtra, DT_RIGHT, &GraphicsMisc::Marlett());
		break;

	default:
		ASSERT(0);
	}
}

UINT CMenuButton::TrackPopupMenu(CMenu* pMenu, int nSubMenu)
{
	CMenu* pPopup = (nSubMenu == -1 ? pMenu : pMenu->GetSubMenu(nSubMenu));
	ASSERT(pPopup);

	if (!pPopup)
		return FALSE;

	CPoint pt;
	CRect rWindow;
	GetWindowRect(rWindow);
	
	switch (m_nStyle)
	{
	case MBS_DOWN:
		pt.x = rWindow.left;
		pt.y = rWindow.bottom;
		break;
		
	case MBS_RIGHT:
		pt.x = rWindow.right;
		pt.y = rWindow.top;
		break;
		
	default:
		ASSERT(0);
		return FALSE;
	}
	
	PrepareState(pPopup);

	TPMPARAMS tpmp;
	tpmp.cbSize = sizeof(TPMPARAMS);

	GetWindowRect(&tpmp.rcExclude);

	UINT nCmdID = ::TrackPopupMenuEx(pPopup->GetSafeHmenu(), 
									TPM_LEFTALIGN | TPM_LEFTBUTTON | TPM_RETURNCMD, 
									pt.x, pt.y, *GetParent(), &tpmp);

	return nCmdID; 
}

BOOL CMenuButton::DoAction()
{
	CMenu menu;

	if (menu.LoadMenu(m_nMenuID))
	{
		UINT nCmdID = TrackPopupMenu(&menu, m_nSubMenu);

		if (nCmdID > 0)
			GetParent()->SendMessage(WM_COMMAND, nCmdID, 0);
	}

	// else
	return CCustomButton::DoAction();
}

void CMenuButton::PrepareState(CMenu* pMenu)
{
	// prepare menu items state
	CCmdUI state;
	state.m_pMenu = pMenu;
	state.m_nIndexMax = pMenu->GetMenuItemCount();
	
	for (state.m_nIndex = 0; state.m_nIndex < state.m_nIndexMax; state.m_nIndex++)
	{
		state.m_nID = pMenu->GetMenuItemID(state.m_nIndex);
		
		if (state.m_nID == 0)
			continue; // menu separator or invalid cmd - ignore it
		
		if (state.m_nID == (UINT)-1)
			PrepareState(pMenu->GetSubMenu(state.m_nIndex));
		else
		{
			state.m_pSubMenu = NULL;
			state.DoUpdate(GetParent(), FALSE);  
		}
	}
}