// EditShortcutMgr.cpp: implementation of the CEditShortcutMgr class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "EditShortcutMgr.h"
#include "misc.h"
#include "winclasses.h"
#include "wclassdefines.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CEditShortcutMgr::CEditShortcutMgr()
{

}

CEditShortcutMgr::~CEditShortcutMgr()
{

}

BOOL CEditShortcutMgr::Initialize(DWORD dwSelectAllShortcut)
{
	GetInstance().m_dwSelectAllShortcut = dwSelectAllShortcut;

	return GetInstance().InitHooks(HM_KEYBOARD, WC_EDIT);
}

void CEditShortcutMgr::Release()
{
	GetInstance().ReleaseHooks();
}

BOOL CEditShortcutMgr::OnKeyboard(UINT uVirtKey, UINT /*uFlags*/)
{
	WORD wModifiers = 0;
	
	if (Misc::KeyIsPressed(VK_CONTROL))
		wModifiers |= HOTKEYF_CONTROL;
	
	if (Misc::KeyIsPressed(VK_SHIFT))
		wModifiers |= HOTKEYF_SHIFT;
	
	if (Misc::KeyIsPressed(VK_MENU))
		wModifiers |= HOTKEYF_ALT;
	
	DWORD dwShortcut = MAKELONG(uVirtKey, wModifiers);
	
	if (dwShortcut == m_dwSelectAllShortcut)
	{
		HWND hwnd = ::GetFocus();
		
		if (CWinClasses::IsEditControl(hwnd))
		{
			::SendMessage(hwnd, EM_SETSEL, 0, -1);
			return TRUE;
		}
	}
	
	// all else
	return FALSE;
}
