// MouseWheelMgr.cpp: implementation of the CMouseWheelMgr class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "MouseWheelMgr.h"
#include "winclasses.h"
#include "wclassdefines.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#ifndef GET_WHEEL_DELTA_WPARAM
#	define GET_WHEEL_DELTA_WPARAM(wParam)  ((short)HIWORD(wParam))
#endif // GET_WHEEL_DELTA_WPARAM

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMouseWheelMgr::CMouseWheelMgr()
{

}

CMouseWheelMgr::~CMouseWheelMgr()
{

}

BOOL CMouseWheelMgr::Initialize()
{
	return GetInstance().InitHooks(HM_MOUSE);
}

void CMouseWheelMgr::Release()
{
	GetInstance().ReleaseHooks();
}

BOOL CMouseWheelMgr::OnMouseEx(UINT uMouseMsg, const MOUSEHOOKSTRUCTEX& info)
{
	if (uMouseMsg == WM_MOUSEWHEEL)
	{
		HWND hwndPt = ::WindowFromPoint(info.pt);
		int zDelta = GET_WHEEL_DELTA_WPARAM(info.mouseData);

		if (info.hwnd != hwndPt)  // does the window under the mouse have the focus.
		{
			// modifier keys are not reported in MOUSEHOOKSTRUCTEX
			// so we have to figure them out
			WORD wKeys = 0;

			if (GetKeyState(VK_CONTROL) & 0x8000)
				wKeys |= MK_CONTROL;

			if (GetKeyState(VK_SHIFT) & 0x8000)
				wKeys |= MK_SHIFT;

			if (GetKeyState(VK_LBUTTON) & 0x8000)
				wKeys |= MK_LBUTTON;

			if (GetKeyState(VK_RBUTTON) & 0x8000)
				wKeys |= MK_RBUTTON;

			if (GetKeyState(VK_MBUTTON) & 0x8000)
				wKeys |= MK_MBUTTON;
			
			::PostMessage(hwndPt, WM_MOUSEWHEEL, MAKEWPARAM(wKeys, zDelta), MAKELPARAM(info.pt.x, info.pt.y));
			return TRUE; // eat
		}
		else // special cases not natively supporting mouse wheel
		{
			CString sClass = CWinClasses::GetClass(hwndPt);
			
			if (CWinClasses::IsClass(sClass, WC_DATETIMEPICK) ||
				CWinClasses::IsClass(sClass, WC_MONTHCAL))
			{
				::PostMessage(hwndPt, WM_KEYDOWN, zDelta > 0 ? VK_UP : VK_DOWN, 0L);
			}
		}
	}
	
	// all else
	return FALSE;
}