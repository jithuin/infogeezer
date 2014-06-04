// maskedit.cpp : implementation file
//

#include "stdafx.h"
#include "maskedit.h"
#include "winclasses.h"
#include "misc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMaskEdit

IMPLEMENT_DYNAMIC(CMaskEdit, CEdit)

CMaskEdit::CMaskEdit(LPCTSTR szMask, DWORD dwFlags) : m_sMask(szMask), m_dwFlags(dwFlags)
{
	SetMask(szMask, dwFlags);
}

CMaskEdit::~CMaskEdit()
{
}

BEGIN_MESSAGE_MAP(CMaskEdit, CEdit)
	//{{AFX_MSG_MAP(CMaskEdit)
	ON_WM_CHAR()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMaskEdit message handlers

void CMaskEdit::OnChar(UINT nChar, UINT nRepCnt, UINT nFlags) 
{
	switch (nChar)
	{
	case VK_BACK:
		break;
/*
	// Ctrl+Backspace does not work in standard edit controls
	case 0x7F:
		if (Misc::KeyIsPressed(VK_CONTROL))
		{
			// delete word to left of selection start
			int nSelStart, nSelEnd;
			GetSel(nSelStart, nSelEnd);

			if (nSelStart > 0)
			{
				int nWordStart = SendMessage(EM_FINDWORDBREAK, WB_MOVEWORDLEFT, nSelStart);

				if (nWordStart >= 0 && nWordStart < nSelStart)
				{
					SetSel(nWordStart, nSelStart);
					ReplaceSel(_T(""), TRUE);
					return;
				}
			}
		}
		break;
*/

	// something wierd going on with edit control accelerators
	case 3:   // c
	case 22:  // v
	case 24:  // x
		if (CWinClasses::IsEditControl(*this))
		{
			ASSERT (Misc::KeyIsPressed(VK_CONTROL));
			break;
		}
		// else drop thru

	case VK_RETURN:
		if (GetStyle() & ES_WANTRETURN)
			break;
		// else drop thru
		
	case 'c':
	case 'C':
	case 'v':
	case 'V':
	case 'x':
	case 'X':
		if (Misc::KeyIsPressed(VK_CONTROL))
			break;
		// else drop thru
		
	case '.':
		if (m_dwFlags & ME_LOCALIZEDECIMAL)
		{
			CString sDecimal = Misc::GetDecimalSeparator();
	
			if (!sDecimal.IsEmpty())
				nChar = sDecimal[0];
			else
				break;
		}
		// else drop thru
		
	default:
		if (!m_sMask.IsEmpty())
		{
			if ((m_dwFlags & ME_EXCLUDE) && m_sMask.Find((char)nChar) != -1)
				return;

			else if (!(m_dwFlags & ME_EXCLUDE) && m_sMask.Find((char)nChar) == -1)
				return;
		}
		break;
	}

	//CEdit::OnChar(nChar, nRepCnt, nFlags);
	DefWindowProc(WM_CHAR, (WPARAM)nChar, MAKELPARAM(nRepCnt, nFlags));
}

void CMaskEdit::SetMask(LPCTSTR szMask, DWORD dwFlags) 
{ 
	m_sMask = szMask; 
	m_dwFlags = dwFlags; 

	if (dwFlags & ME_LOCALIZEDECIMAL)
	{
		CString sDecimal = Misc::GetDecimalSeparator();

		if (!sDecimal.IsEmpty() && sDecimal != _T(".") && m_sMask.Find('.') != -1)
			m_sMask += sDecimal[0];
	}
}


