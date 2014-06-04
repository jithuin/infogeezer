// EditPrompt.cpp: implementation of the CGroupLine class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "groupline.h"
#include "winclasses.h"
#include "wclassdefines.h"
#include "themed.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CGroupLine::CGroupLine()
{

}

CGroupLine::~CGroupLine()
{

}

BOOL CGroupLine::Initialize(HWND hwndStatic)
{
	ASSERT (hwndStatic);
	ASSERT (!IsHooked());
	ASSERT (CWinClasses::IsClass(hwndStatic, WC_STATIC));

	if (!IsHooked() && hwndStatic && HookWindow(hwndStatic))
	{
		Invalidate();
		return TRUE;
	}

	return FALSE;
}

LRESULT CGroupLine::WindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_PAINT:
		{
			CWnd* pWnd = GetCWnd();
			CPaintDC dc(pWnd);

			CRect rClient;
			GetClientRect(rClient);

			CString sText;
			pWnd->GetWindowText(sText);

			DWORD dwStyle = pWnd->GetStyle();
			CThemed theme;

			UINT nFlags = DT_TOP;
			
			if (dwStyle & SS_RIGHT)
				nFlags |= DT_RIGHT;
			
			else if (dwStyle & SS_CENTER)
				nFlags |= DT_CENTER;
			else
				nFlags |= DT_LEFT;
			
			HFONT hFont = (HFONT)SendMessage(WM_GETFONT);
			
			if (!hFont)
				hFont = (HFONT)GetStockObject(DEFAULT_GUI_FONT);

			HFONT hOld = (HFONT)::SelectObject(dc, hFont);
			dc.SetBkMode(TRANSPARENT);

			CSize sizeText = dc.GetTextExtent(sText); // default
			
			if (theme.Open(pWnd, _T("BUTTON")))
			{
				theme.DrawText(&dc, BP_GROUPBOX, GBS_NORMAL, sText, nFlags, 0, rClient);

				CRect rExtents;
				theme.GetTextExtent(&dc, BP_GROUPBOX, GBS_NORMAL, sText, nFlags, rExtents, rClient);

				sizeText = rExtents.Size();
			}
			else // unthemed
			{
				dc.SetTextColor(GetSysColor(COLOR_WINDOWTEXT));
				dc.DrawText(sText, rClient, nFlags);
			}

			// draw line
			sizeText.cx += 4;
			
			int nY = ((rClient.top + rClient.bottom) / 2);
			
			if (dwStyle & SS_RIGHT)
			{
				dc.Draw3dRect(rClient.left, nY, rClient.Width() - sizeText.cx, 2, 
					GetSysColor(COLOR_3DSHADOW), GetSysColor(COLOR_3DHIGHLIGHT));
			}
			else if (dwStyle & SS_CENTER)
			{
				dc.Draw3dRect(rClient.left, nY, (rClient.Width() - sizeText.cx) / 2, 2, 
					GetSysColor(COLOR_3DSHADOW), GetSysColor(COLOR_3DHIGHLIGHT));
				dc.Draw3dRect((rClient.Width() + sizeText.cx) / 2, nY, (rClient.Width() - sizeText.cx) / 2, 2, 
					GetSysColor(COLOR_3DSHADOW), GetSysColor(COLOR_3DHIGHLIGHT));
			}
			else
			{
				dc.Draw3dRect(rClient.left + sizeText.cx, nY, rClient.Width() - sizeText.cx, 2, 
					GetSysColor(COLOR_3DSHADOW), GetSysColor(COLOR_3DHIGHLIGHT));
			}

			// cleanup
			::SelectObject(dc, hOld);


			return 0;
		}
		break;
	}

	return CSubclassWnd::WindowProc(hRealWnd, msg, wp, lp);
}

//////////////////////////////////////////////////////////////////////

CGroupLineManager::CGroupLineManager()
{
}

CGroupLineManager::~CGroupLineManager()
{
	// cleanup
	HWND hwnd;
	CGroupLine* pGroupLine;

	POSITION pos = m_mapGroupLines.GetStartPosition();

	while (pos)
	{
		m_mapGroupLines.GetNextAssoc(pos, hwnd, pGroupLine);

		if (pGroupLine->IsValid())
			pGroupLine->HookWindow(NULL);

		delete pGroupLine;
	}
}

BOOL CGroupLineManager::AddGroupLine(UINT nIDStatic, HWND hwndParent)
{
	HWND hwndStatic = ::GetDlgItem(hwndParent, nIDStatic);
	
	if (!hwndStatic)
		return FALSE;

	// have we already got it?
	CGroupLine* pGroupLine = NULL;

	if (m_mapGroupLines.Lookup(hwndStatic, pGroupLine))
		return TRUE;

	// else create new editprompt
	pGroupLine = new CGroupLine;

	if (pGroupLine && pGroupLine->Initialize(hwndStatic))
	{
		m_mapGroupLines[hwndStatic] = pGroupLine;
		return TRUE;
	}
	
	// else
	delete pGroupLine;
	return FALSE;
}

