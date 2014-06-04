// CDialogHelper.cpp: implementation of the CDialogHelper class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "DialogHelper.h"
#include "autoflag.h"
#include "winclasses.h"
#include "wclassdefines.h"
#include "misc.h"
#include "runtimedlg.h"
#include "enstring.h"

#include <afxpriv.h>
#include <float.h>
#include <locale.h>
#include <afxtempl.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

const CRect EMPTY_RECT(0, 0, 0, 0);

//////////////////////////////////////////////////////////////////////
// MFC replacements to prevent popup error messages

BOOL DH_SimpleScanf(LPCTSTR lpszText, LPCTSTR lpszFormat, va_list pData)
{
	ASSERT(lpszText != NULL);
	ASSERT(lpszFormat != NULL);

	ASSERT(*lpszFormat == '%');
	lpszFormat++;        // skip '%'

	BOOL bLong = FALSE;
	BOOL bShort = FALSE;
	if (*lpszFormat == 'l')
	{
		bLong = TRUE;
		lpszFormat++;
	}
	else if (*lpszFormat == 's')
	{
		bShort = TRUE;
		lpszFormat++;
	}

	ASSERT(*lpszFormat == 'd' || *lpszFormat == 'u');
	ASSERT(lpszFormat[1] == '\0');

	while (*lpszText == ' ' || *lpszText == '\t')
		lpszText++;
	TCHAR chFirst = lpszText[0];
	long l, l2;
	if (*lpszFormat == 'd')
	{
		// signed
		l = _tcstol(lpszText, (LPTSTR*)&lpszText, 10);
		l2 = (int)l;
	}
	else
	{
		// unsigned
		if (*lpszText == '-')
			return FALSE;
		l = (long)_tcstoul(lpszText, (LPTSTR*)&lpszText, 10);
		l2 = (unsigned int)l;
	}
	if (l == 0 && chFirst != '0')
		return FALSE;   // could not convert

	while (*lpszText == ' ' || *lpszText == '\t')
		lpszText++;
	if (*lpszText != '\0')
		return FALSE;   // not terminated properly

	if (bShort)
	{
		if ((short)l != l)
			return FALSE;   // too big for short
		*va_arg(pData, short*) = (short)l;
	}
	else
	{
		ASSERT(sizeof(long) == sizeof(int));
		ASSERT(l == l2);
		*va_arg(pData, long*) = l;
	}

	// all ok
	return TRUE;
}

BOOL DH_SimpleFloatParse(LPCTSTR lpszText, double& d)
{
	ASSERT(lpszText != NULL);
	while (*lpszText == ' ' || *lpszText == '\t')
		lpszText++;

	TCHAR chFirst = lpszText[0];
	d = _tcstod(lpszText, (LPTSTR*)&lpszText);
	if (d == 0.0 && chFirst != '0')
		return FALSE;   // could not convert
	while (*lpszText == ' ' || *lpszText == '\t')
		lpszText++;

	if (*lpszText != '\0')
		return FALSE;   // not terminated properly

	return TRUE;
}

void DH_DDX_TextWithFormat(CDataExchange* pDX, int nIDC, LPCTSTR lpszFormat, UINT nIDPrompt, ...)
{
	va_list pData;
	va_start(pData, nIDPrompt);

	HWND hWndCtrl = pDX->PrepareEditCtrl(nIDC);
	TCHAR szBuffer[32];

	if (pDX->m_bSaveAndValidate)
	{
		// the following works for %d, %u, %ld, %lu
		::GetWindowText(hWndCtrl, szBuffer, sizeof(szBuffer) / sizeof(TCHAR));

// *******************************************************************
		if (*szBuffer == 0)
			//fabio_2005
#if _MSC_VER >= 1400
			_tcscpy_s(szBuffer, _T("0"));
#else
			_tcscpy(szBuffer, _T("0"));
#endif

// *******************************************************************

		if (!DH_SimpleScanf(szBuffer, lpszFormat, pData))
		{
// *******************************************************************
//			AfxMessageBox(nIDPrompt);
//			pDX->Fail();        // throws exception
// *******************************************************************
		}
	}
	else
	{
		wvsprintf(szBuffer, lpszFormat, pData);

			// does not support floating point numbers - see dlgfloat.cpp
		AfxSetWindowText(hWndCtrl, szBuffer);
	}

	va_end(pData);
}

void DH_TextFloatFormat(CDataExchange* pDX, int nIDC, void* pData, double value, int nSizeGcvt)
{
	// handle locale specific decimal separator
	setlocale(LC_NUMERIC, "");

	ASSERT(pData != NULL);

	HWND hWndCtrl = pDX->PrepareEditCtrl(nIDC);
	TCHAR szBuffer[32];

	if (pDX->m_bSaveAndValidate)
	{
		::GetWindowText(hWndCtrl, szBuffer, sizeof(szBuffer) / sizeof(TCHAR));

		// *******************************************************************
		if (*szBuffer == 0)
			//fabio_2005
#if _MSC_VER >= 1400
			_tcscpy_s(szBuffer, 32, _T("0"));
#else
			_tcscpy(szBuffer, _T("0"));
#endif

		// *******************************************************************

		double d;

		if (!DH_SimpleFloatParse(szBuffer, d))
		{
// *******************************************************************
//			AfxMessageBox(nIDPrompt);
//			pDX->Fail();        // throws exception
// *******************************************************************
			
			// try English locale
			setlocale(LC_NUMERIC, "English");
			DH_SimpleFloatParse(szBuffer, d);
		}

		if (nSizeGcvt == FLT_DIG)
			*((float*)pData) = (float)d;
		else
			*((double*)pData) = d;
	}
	else
	{
		//fabio_2005
#if _MSC_VER >= 1400
		_stprintf_s(szBuffer, _T("%.*g"), nSizeGcvt, value);
#else
		_stprintf(szBuffer, _T("%.*g"), nSizeGcvt, value);
#endif
		AfxSetWindowText(hWndCtrl, szBuffer);
	}

	// restore decimal separator to '.'
	setlocale(LC_NUMERIC, "English");
}

//////////////////////////////////////////////////////////////////////

void CDialogHelper::DDX_Text(CDataExchange* pDX, int nIDC, BYTE& value)
{
	int n = (int)value;
	if (pDX->m_bSaveAndValidate)
	{
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%u"), AFX_IDP_PARSE_BYTE, &n);

		if (n > 255)
		{
// *******************************************************************
//			AfxMessageBox(AFX_IDP_PARSE_BYTE);
//			pDX->Fail();        // throws exception
// *******************************************************************
		}
		value = (BYTE)n;
	}
	else
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%u"), AFX_IDP_PARSE_BYTE, n);
}

void CDialogHelper::DDX_Text(CDataExchange* pDX, int nIDC, short& value)
{
	if (pDX->m_bSaveAndValidate)
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%sd"), AFX_IDP_PARSE_INT, &value);
	else
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%hd"), AFX_IDP_PARSE_INT, value);
}

void CDialogHelper::DDX_Text(CDataExchange* pDX, int nIDC, int& value)
{
	if (pDX->m_bSaveAndValidate)
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%d"), AFX_IDP_PARSE_INT, &value);
	else
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%d"), AFX_IDP_PARSE_INT, value);
}

void CDialogHelper::DDX_Text(CDataExchange* pDX, int nIDC, UINT& value)
{
	if (pDX->m_bSaveAndValidate)
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%u"), AFX_IDP_PARSE_UINT, &value);
	else
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%u"), AFX_IDP_PARSE_UINT, value);
}

void CDialogHelper::DDX_Text(CDataExchange* pDX, int nIDC, long& value)
{
	if (pDX->m_bSaveAndValidate)
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%ld"), AFX_IDP_PARSE_INT, &value);
	else
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%ld"), AFX_IDP_PARSE_INT, value);
}

void CDialogHelper::DDX_Text(CDataExchange* pDX, int nIDC, DWORD& value)
{
	if (pDX->m_bSaveAndValidate)
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%lu"), AFX_IDP_PARSE_UINT, &value);
	else
		DH_DDX_TextWithFormat(pDX, nIDC, _T("%lu"), AFX_IDP_PARSE_UINT, value);
}

void CDialogHelper::DDX_Text(CDataExchange* pDX, int nIDC, CString& value)
{
	::DDX_Text(pDX, nIDC, value);
}

////////////////////////////////////////////////////////////////////////////////
// floats

void CDialogHelper::DDX_Text(CDataExchange* pDX, int nIDC, float& value)
{
	DH_TextFloatFormat(pDX, nIDC, &value, value, FLT_DIG);
}

void CDialogHelper::DDX_Text(CDataExchange* pDX, int nIDC, double& value)
{
	DH_TextFloatFormat(pDX, nIDC, &value, value, DBL_DIG);
}

////////////////////////////////////////////////////////////////////////////////

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, BYTE& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	DDX_Text(&dx, nIDC, value);

	return TRUE;
}

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, short& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	DDX_Text(&dx, nIDC, value);

	return TRUE;
}

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, int& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	// this can be used for a variety of control types so we need
	// to figure out what nIDC points to and then code accordingly
	HWND hCtrl = dx.PrepareCtrl(nIDC);

	if (!hCtrl)
		return FALSE;

	CString sClass;

	::GetClassName(hCtrl, sClass.GetBuffer(100), 100);
	sClass.ReleaseBuffer();

	if (CWinClasses::IsEditControl(hCtrl))
		DDX_Text(&dx, nIDC, value);

	else if (CWinClasses::IsClass(hCtrl, WC_COMBOBOX))
		DDX_CBIndex(&dx, nIDC, value);

	else if (CWinClasses::IsClass(hCtrl, WC_LISTBOX))
		DDX_LBIndex(&dx, nIDC, value);

	else if (CWinClasses::IsClass(hCtrl, WC_SCROLLBAR))
		DDX_Scroll(&dx, nIDC, value);

	else if (CWinClasses::IsClass(hCtrl, WC_BUTTON))
	{
		DWORD dwStyle = GetWindowLong(hCtrl, GWL_STYLE);

		if ((dwStyle & BS_CHECKBOX) == BS_CHECKBOX)
			DDX_Check(&dx, nIDC, value);
		else if ((dwStyle & BS_RADIOBUTTON) == BS_RADIOBUTTON)
			DDX_Radio(&dx, nIDC, value);
		else
		{
			ASSERT (0);
			return FALSE;
		}
	}
	else
	{
		ASSERT (0);
		return FALSE;
	}

	return TRUE;
}

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, UINT& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	DDX_Text(&dx, nIDC, value);

	return TRUE;
}

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, long& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	DDX_Text(&dx, nIDC, value);

	return TRUE;
}

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, DWORD& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	DDX_Text(&dx, nIDC, value);

	return TRUE;
}

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, CString& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	// this can be used for a variety of control types so we need
	// to figure out what nIDC points to and then code accordingly
	HWND hCtrl = dx.PrepareCtrl(nIDC);

	if (!hCtrl)
		return FALSE;

	if (CWinClasses::IsEditControl(hCtrl))
		DDX_Text(&dx, nIDC, value);

	else if (CWinClasses::IsClass(hCtrl, WC_COMBOBOX))
		DDX_CBString(&dx, nIDC, value);

	else if (CWinClasses::IsClass(hCtrl, WC_LISTBOX))
		DDX_LBString(&dx, nIDC, value);

	else
	{
//		ASSERT (0);
		return FALSE;
	}

	return TRUE;
}

BOOL CDialogHelper::UpdateDataExact(CWnd* pWnd, int nIDC, CString& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	// this can be used for a variety of control types so we need
	// to figure out what nIDC points to and then code accordingly
	HWND hCtrl = dx.PrepareCtrl(nIDC);

	if (!hCtrl)
		return FALSE;

	else if (CWinClasses::IsClass(hCtrl, WC_COMBOBOX))
		DDX_CBStringExact(&dx, nIDC, value);

	else if (CWinClasses::IsClass(hCtrl, WC_LISTBOX))
		DDX_LBStringExact(&dx, nIDC, value);

	else
	{
		ASSERT (0);
		return FALSE;
	}

	return TRUE;
}

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, float& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	DDX_Text(&dx, nIDC, value);

	return TRUE;
}

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, double& value, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	DDX_Text(&dx, nIDC, value);

	return TRUE;
}

BOOL CDialogHelper::UpdateDataEx(CWnd* pWnd, int nIDC, CWnd& ctrl, BOOL bSaveAndValidate)
{
	CAutoFlag af(m_bInUpdateEx, TRUE);
	CDataExchange dx(pWnd, bSaveAndValidate);

	DDX_Control(&dx, nIDC, ctrl);

	return TRUE;
}

void CDialogHelper::SetFont(CWnd* pWnd, HFONT hFont, BOOL bRedraw)
{
	// don't send to toolbar as it causes all sorts of problems with drop buttons
	// but do send to a toolbar's children
	if (!pWnd->IsKindOf(RUNTIME_CLASS(CToolBar)))
		pWnd->SendMessage(WM_SETFONT, (WPARAM)hFont, bRedraw);

	// children
	CWnd* pChild = pWnd->GetWindow(GW_CHILD);

	while (pChild)
	{
		SetFont(pChild, hFont, bRedraw);
		pChild = pChild->GetNextWindow(GW_HWNDNEXT);
	}
}

HFONT CDialogHelper::GetFont(const CWnd* pWnd)
{
   if (pWnd)
      return GetFont(pWnd->GetSafeHwnd());

   return (HFONT)::GetStockObject(DEFAULT_GUI_FONT);
}
 
HFONT CDialogHelper::GetFont(HWND hWnd)
{
   if (hWnd)
   {
      HFONT hFont = (HFONT)::SendMessage(hWnd, WM_GETFONT, 0, 0);

      if (hFont)
         return hFont;
   }

   return (HFONT)::GetStockObject(DEFAULT_GUI_FONT);
}

int CDialogHelper::SetComboBoxItems(CComboBox& combo, const CStringArray& aItems) 
{
	combo.ResetContent();
	
	for (int nItem = 0; nItem < aItems.GetSize(); nItem++)
		combo.AddString(aItems[nItem]);

	if (combo.GetCount())
		RefreshMaxDropWidth(combo);

	return combo.GetCount();
}

BOOL CDialogHelper::IsDroppedComboBox(HWND hCtrl)
{
	if (CWinClasses::IsComboBox(hCtrl))
		return ::SendMessage(hCtrl, CB_GETDROPPEDSTATE, 0, 0);

	return FALSE;
}

BOOL CDialogHelper::ProcessDialogControlShortcut(const MSG* pMsg)
{
	// message id must be WM_KEYDOWN and alt key must be pressed
	if (pMsg->message != WM_SYSKEYDOWN || pMsg->wParam == VK_MENU)
		return FALSE;

	if (!Misc::KeyIsPressed(VK_MENU))
		return FALSE;

	// convert shortcut from virtual key to char
	UINT nShortcut = MapVirtualKey(pMsg->wParam, 2);

	if (!nShortcut)
		return FALSE;
		
	// find the next control having this accelerator
	CWnd* pFocus = CWnd::GetFocus();

	if (pFocus)
	{
		CWnd* pNext = FindNextMatch(pFocus, nShortcut);

		if (pNext)
		{
			pNext->SetFocus();
			return TRUE;
		}
	}

	return FALSE;
}

CWnd* CDialogHelper::FindNextMatch(CWnd* pCurrent, UINT nShortcut)
{
	// only the brute force method would appear to work here
	nShortcut = toupper(nShortcut);

	// if pCurrent is the edit of a combo then we want the combo
	if (CWinClasses::IsEditControl(*pCurrent) &&
		CWinClasses::IsClass(::GetParent(*pCurrent), WC_COMBOBOX))
	{
		pCurrent = pCurrent->GetParent();
	}

	// 1. find the first parent popup window
	CWnd* pTop = pCurrent->GetParent();

	while (pTop && !(pTop->GetStyle() & WS_CAPTION))
		pTop = pTop->GetParent();

	if (pTop && pTop != pCurrent)
	{
		// get a list of all tabbable items within this parent
		CList<CWnd*, CWnd*> lstWnds;

		CWnd* pFirst = pTop->GetNextDlgTabItem(pTop->GetWindow(GW_CHILD));
		CWnd* pChild = pFirst;

		while (pChild)
		{
			lstWnds.AddTail(pChild);
			pChild = pTop->GetNextDlgTabItem(pChild);

			if (pChild == pFirst)
				break;
		}

		if (lstWnds.GetCount() > 1)
		{
			// keep moving the head to the tail until pCurrent is at the head
			// unless pCurrent is not in the list (been disabled or made invisible)
			if (lstWnds.Find(pCurrent))
			{
				CWnd* pHead = lstWnds.GetHead();

				while (pHead != pCurrent && !pHead->IsChild(pCurrent))
				{
					lstWnds.AddTail(lstWnds.RemoveHead());
					pHead = lstWnds.GetHead();
				}

				// remove pCurrent
				lstWnds.RemoveHead();
			}
			else
				TRACE(_T("CDialogHelper::FindNextMatch(pCurrent not found)\n"));

			// now traverse the list looking for preceding static
			// labels with a matching accelerator
			POSITION pos = lstWnds.GetHeadPosition();

			while (pos)
			{
				pChild = lstWnds.GetNext(pos);

				CWnd* pPrev = pChild->GetWindow(GW_HWNDPREV);

				if (CWinClasses::IsClass(pPrev->GetSafeHwnd(), WC_STATIC) &&
					pPrev->IsWindowVisible())
				{
					CString sText;
					pPrev->GetWindowText(sText);

					if (!sText.IsEmpty() && GetShortcut(sText) == nShortcut)
						return pChild;
				}
			}
		}
	}

	// all else
	return NULL;
}

UINT CDialogHelper::GetShortcut(const CString& sText)
{
	for (int nChar = 0; nChar < sText.GetLength() - 1; nChar++)
	{
		if (sText[nChar] == '&' && sText[nChar + 1] != '&')
			return toupper(sText[nChar + 1]);
	}

	// no shortcut
	return 0;
}

UINT CDialogHelper::MessageBoxEx(CWnd* pWnd, UINT nIDText, UINT nIDCaption, UINT nType)
{
	CString sText;
	
	sText.LoadString(nIDText);

	return MessageBoxEx(pWnd, sText, nIDCaption, nType);
}

UINT CDialogHelper::MessageBoxEx(CWnd* pWnd, LPCTSTR szText, UINT nIDCaption, UINT nType)
{
	CString sCaption;
	
	sCaption.LoadString(nIDCaption);

	return pWnd->MessageBox(szText, sCaption, nType);
}

int CDialogHelper::RefreshMaxDropWidth(CComboBox& combo, CDC* pDCRef, int nTabWidth)
{
   int nWidth = CalcMaxTextWidth(combo, 0, TRUE, pDCRef, nTabWidth);
   combo.SetDroppedWidth(nWidth);

   return nWidth;
}

int CDialogHelper::CalcMaxTextWidth(CComboBox& combo, int nMinWidth, BOOL bDropped, CDC* pDCRef, int nTabWidth)
{
	CDC* pDC = pDCRef;
	CFont* pOldFont = NULL;
	
	if (!pDC)
	{
		pDC = combo.GetDC();
		pOldFont = pDC->SelectObject(combo.GetFont());
	}
	
	CEnString sText;
	int nMaxWidth = nMinWidth;
	int nItem = combo.GetCount();
	
	while (nItem--)
	{
		combo.GetLBText(nItem, sText);
		
		int nWidth = pDC->GetTextExtent(sText).cx;
		
		// handle tabs
		if (nTabWidth > 0)
			nWidth += sText.GetCharacterCount('\t', FALSE) * nTabWidth;
		
		nMaxWidth = max(nMaxWidth, nWidth);
	}
	
	// check window text too
	combo.GetWindowText(sText);
	
	int nWidth = pDC->GetTextExtent(sText).cx;
	nMaxWidth = max(nMaxWidth, nWidth);
	
	// dropped width needs some more space
	if (bDropped)
	{
		// Add the avg width to prevent clipping
		TEXTMETRIC tm;
		pDC->GetTextMetrics(&tm);
		nMaxWidth += tm.tmAveCharWidth;
		
		// Adjust the width for a vertical scroll bar and the left and right border.
		nMaxWidth += ::GetSystemMetrics(SM_CXVSCROLL);
		nMaxWidth += (2 * ::GetSystemMetrics(SM_CXEDGE));
	}
	
	// cleanup
	if (!pDCRef)
	{
		pDC->SelectObject(pOldFont);
		combo.ReleaseDC(pDC);
	}
	
	return nMaxWidth;
}

int CDialogHelper::RefreshMaxColumnWidth(CListBox& list, CDC* pDCRef)
{
	int nWidth = CalcMaxTextWidth(list, 0, pDCRef);
	
	if (nWidth > 0)
	{
		// if we are a check-listbox then add space for the checkbox
		if (list.IsKindOf(RUNTIME_CLASS(CCheckListBox)))
			nWidth += 20;

		list.SetColumnWidth(nWidth);
	}
	
	return nWidth;
}

int CDialogHelper::CalcMaxTextWidth(CListBox& list, int nMinWidth, CDC* pDCRef)
{
	// must have multi column style
	if ((list.GetStyle() & LBS_MULTICOLUMN) == 0)
		return 0;
	
	CDC* pDC = pDCRef;
	CFont* pOldFont = NULL;
	
	if (!pDC)
	{
		pDC = list.GetDC();
		pOldFont = pDC->SelectObject(list.GetFont());
	}
	
	CEnString sText;
	int nMaxWidth = nMinWidth;
	int nItem = list.GetCount();
	
	while (nItem--)
	{
		list.GetText(nItem, sText);
		
		int nWidth = pDC->GetTextExtent(sText).cx;
		nMaxWidth = max(nMaxWidth, nWidth);
	}
	
	int nWidth = pDC->GetTextExtent(sText).cx;
	nMaxWidth = max(nMaxWidth, nWidth);
	
	// cleanup
	if (!pDCRef)
	{
		pDC->SelectObject(pOldFont);
		list.ReleaseDC(pDC);
	}
	
	return nMaxWidth;
}

BOOL CDialogHelper::IsChildOrSame(HWND hWnd, HWND hwndChild)
{
	return (hWnd == hwndChild || ::IsChild(hWnd, hwndChild));
}

BOOL CDialogHelper::SelectItemByValue(CComboBox& combo, int nValue)
{
	CString sNum;
	sNum.Format(_T("%d"), nValue);

	return (CB_ERR != combo.SelectString(-1, sNum));
}

int CDialogHelper::FindItemByValue(const CComboBox& combo, int nValue)
{
	CString sNum;
	sNum.Format(_T("%d"), nValue);

	return combo.FindString(-1, sNum);
}

BOOL CDialogHelper::SelectItemByData(CComboBox& combo, DWORD dwItemData)
{
	int nItem = combo.GetCount();
	
	while (nItem--)
	{
		if (combo.GetItemData(nItem) == dwItemData)
		{
			combo.SetCurSel(nItem);
			return TRUE;
		}
	}

	combo.SetCurSel(-1);
	return FALSE;
}

CString CDialogHelper::GetSelectedItem(const CComboBox& combo)
{
	int nSel = combo.GetCurSel();
	CString sItem;

	if (nSel != CB_ERR)
		combo.GetLBText(nSel, sItem);

	return sItem;
}

int CDialogHelper::GetSelectedItemAsValue(const CComboBox& combo)
{
	return _ttoi(GetSelectedItem(combo));
}

DWORD CDialogHelper::GetSelectedItemData(const CComboBox& combo)
{
	return combo.GetItemData(combo.GetCurSel());
}

int CDialogHelper::AddString(CComboBox& combo, LPCTSTR szItem, DWORD dwItemData)
{
	int nIndex = combo.AddString(szItem);

	if (nIndex != CB_ERR)
		combo.SetItemData(nIndex, dwItemData);

	return nIndex;
}

CString CDialogHelper::GetControlLabel(const CWnd* pWnd)
{
	if (!pWnd)
		return "";

	CString sText;

	// check for combo edit
	if (CWinClasses::IsEditControl(*pWnd) && CWinClasses::IsComboBox(*(pWnd->GetParent())))
		pWnd = pWnd->GetParent();

	CWnd* pPrev = pWnd->GetNextWindow(GW_HWNDPREV);
			
	while (pPrev)
	{
		if (CWinClasses::IsClass(*pPrev, WC_STATIC))
		{
			pPrev->GetWindowText(sText);
			sText.Replace(_T("&"), _T(""));
			break;
		}

		pPrev = pPrev->GetNextWindow(GW_HWNDPREV);
	}

	return sText;
}

BOOL CDialogHelper::ControlWantsEnter(HWND hwnd)
{
	CString sClass = CWinClasses::GetClass(hwnd);
	DWORD dwStyle = ::GetWindowLong(hwnd, GWL_STYLE);
	
	if (CWinClasses::IsComboBox(sClass))
	{
		// check if dropped
		if (SendMessage(hwnd, CB_GETDROPPEDSTATE, 0, 0))
			return TRUE;
	}
	// also check for combo's edit box and edits with ES_WANTRETURN
	else if (CWinClasses::IsClass(sClass, WC_EDIT))
	{
		if (dwStyle & ES_WANTRETURN)
			return TRUE;
		
		HWND hwndParent = ::GetParent(hwnd);

		// check if parent is dropped combo
		if (hwndParent && CWinClasses::IsComboBox(hwndParent) && 
			::SendMessage(hwndParent, CB_GETDROPPEDSTATE, 0, 0))
		{
			return TRUE;
		}		
	}
	// and also richedit with ES_WANTRETURN
	else if (CWinClasses::IsRichEditControl(sClass))
	{
		if (dwStyle & ES_WANTRETURN)
			return TRUE;
	}

	return FALSE;
}

void CDialogHelper::OffsetCtrls(const CWnd* pParent, const CUIntArray& aCtrlIDs, int x, int y)
{
	for (int nID = 0; nID < aCtrlIDs.GetSize(); nID++)
	{
		OffsetCtrl(pParent, aCtrlIDs[nID], x, y);
	}
}

void CDialogHelper::ResizeCtrls(const CWnd* pParent, const CUIntArray& aCtrlIDs, int cx, int cy)
{
	for (int nID = 0; nID < aCtrlIDs.GetSize(); nID++)
	{
		ResizeCtrl(pParent, aCtrlIDs[nID], cx, cy);
	}
}

void CDialogHelper::MoveCtrls(const CWnd* pParent, const CUIntArray& aCtrlIDs, int x, int y)
{
	for (int nID = 0; nID < aCtrlIDs.GetSize(); nID++)
	{
		MoveCtrl(pParent, aCtrlIDs[nID], x, y);
	}
}

CRect CDialogHelper::OffsetCtrl(const CWnd* pParent, UINT nCtrlID, int dx, int dy)
{
	ASSERT(pParent);
	ASSERT(nCtrlID);

	if (pParent && nCtrlID)
	{
		CWnd* pCtrl = pParent->GetDlgItem(nCtrlID);
		ASSERT(pCtrl);
	
		if (pCtrl)
			return OffsetCtrl(pParent, pCtrl, dx, dy);
	}
	
	// all else
	return EMPTY_RECT;
}

CRect CDialogHelper::OffsetCtrl(const CWnd* pParent, CWnd* pChild, int dx, int dy)
{
	ASSERT(pParent);
	ASSERT(pChild);

	if (pParent && pChild)
	{
		CRect rChild = GetCtrlRect(pParent, pChild);

		if (dx || dy)
		{
			rChild.OffsetRect(dx, dy);
			pChild->MoveWindow(rChild);
		}

		return rChild;
	}

	// all else
	return EMPTY_RECT;
}

CRect CDialogHelper::GetCtrlRect(const CWnd* pParent, UINT nCtrlID) 
{ 
	ASSERT(pParent);
	ASSERT(nCtrlID);

	if (pParent && nCtrlID)
	{
		const CWnd* pCtrl = pParent->GetDlgItem(nCtrlID);
		ASSERT(pCtrl);

		if (pCtrl)
			return GetCtrlRect(pParent, pCtrl);
	}

	// all else
	return EMPTY_RECT;
}

CRect CDialogHelper::GetCtrlRect(const CWnd* pParent, const CWnd* pChild) 
{ 
	ASSERT(pParent);
	ASSERT(pChild);

	if (pParent && pChild)
	{
		CRect rChild;

		pChild->GetWindowRect(rChild);
		pParent->ScreenToClient(rChild);

		return rChild;
	}

	// all else
	return EMPTY_RECT;
}

CRect CDialogHelper::MoveCtrl(const CWnd* pParent, UINT nCtrlID, int x, int y)
{
	ASSERT(pParent);
	ASSERT(nCtrlID);

	if (pParent && nCtrlID)
	{
		CWnd* pCtrl = pParent->GetDlgItem(nCtrlID);
		ASSERT(pCtrl);
		
		if (pCtrl)
		{
			CRect rChild = GetCtrlRect(pParent, pCtrl);
			
			if (x || y)
			{
				rChild.OffsetRect(x - rChild.left, y - rChild.top);
				pCtrl->MoveWindow(rChild);
			}
			
			return rChild;
		}
	}
	
	// all else
	return EMPTY_RECT;
}

CRect CDialogHelper::ResizeCtrl(const CWnd* pParent, UINT nCtrlID, int cx, int cy)
{
	ASSERT(pParent);
	ASSERT(nCtrlID);

	if (pParent && nCtrlID)
	{
		CWnd* pCtrl = pParent->GetDlgItem(nCtrlID);
		ASSERT(pCtrl);
		
		if (pCtrl)
		{
			CRect rChild = GetCtrlRect(pParent, pCtrl);
			
			CRect rParent;
			pParent->GetClientRect(rParent);
			
			if (cx || cy)
			{
				rChild.right += cx;
				rChild.bottom += cy;
				
				// make sure it also intersects with parent
				if (rChild.IntersectRect(rChild, rParent))
					pCtrl->MoveWindow(rChild);
			}
			
			return rChild;
		}
	}
	
	// all else
	return EMPTY_RECT;
}

void CDialogHelper::SetControlState(const CWnd* pParent, UINT nCtrlID, RT_CTRLSTATE nState)
{
	SetControlState(::GetDlgItem(*pParent, nCtrlID), nState);
}

void CDialogHelper::SetControlsState(const CWnd* pParent, UINT nCtrlIDFrom, UINT nCtrlIDTo, RT_CTRLSTATE nState)
{
	ASSERT (pParent);
	
	for (UINT nID = nCtrlIDFrom; nID <= nCtrlIDTo; nID++)
		SetControlState(::GetDlgItem(*pParent, nID), nState);
}

void CDialogHelper::SetControlState(HWND hCtrl, RT_CTRLSTATE nState)
{
	if (hCtrl)
	{
		switch (nState)
		{
		case RTCS_ENABLED:
			::EnableWindow(hCtrl, TRUE);
			
			if (CWinClasses::IsEditControl(hCtrl))
				::SendMessage(hCtrl, EM_SETREADONLY, FALSE, 0);
			break;
			
		case RTCS_DISABLED:
			::EnableWindow(hCtrl, FALSE);
			break;
			
		case RTCS_READONLY:
			if (CWinClasses::IsEditControl(hCtrl))
			{
				::EnableWindow(hCtrl, TRUE);
				::SendMessage(hCtrl, EM_SETREADONLY, TRUE, 0);
			}
			else
			{
				BOOL bStatic = CWinClasses::IsClass(hCtrl, WC_STATIC);
				::EnableWindow(hCtrl, bStatic);
			}
			break;
		}
	}
}

int CDialogHelper::GetChildControlIDs(const CWnd* pParent, CUIntArray& aCtrlIDs, LPCTSTR szClass)
{
	aCtrlIDs.RemoveAll();

	CWnd* pChild = pParent->GetWindow(GW_CHILD);

	while (pChild)
	{
		if (!szClass || CWinClasses::IsClass(*pChild, szClass))
		{
			aCtrlIDs.Add(pChild->GetDlgCtrlID());
		}

		pChild = pChild->GetNextWindow();
	}

	return aCtrlIDs.GetSize();
}

void CDialogHelper::RemoveControlID(UINT nCtrlID, CUIntArray& aCtrlIDs)
{
	int nFind = Misc::Find(aCtrlIDs, nCtrlID);

	if (nFind != -1)
		aCtrlIDs.RemoveAt(nFind);
}


void CDialogHelper::ShowControls(const CWnd* pParent, const CUIntArray& aCtrlIDs, BOOL bShow)
{
	ASSERT (pParent);
	
	for (int nID = 0; nID < aCtrlIDs.GetSize(); nID++)
		ShowControl(pParent, aCtrlIDs[nID], bShow);
}

void CDialogHelper::ShowControls(const CWnd* pParent, UINT nCtrlIDFrom, UINT nCtrlIDTo, BOOL bShow)
{
	ASSERT (pParent);
	
	for (UINT nID = nCtrlIDFrom; nID <= nCtrlIDTo; nID++)
		ShowControl(pParent, nID, bShow);
}

void CDialogHelper::ShowControl(const CWnd* pParent, UINT nCtrlID, BOOL bShow)
{
	ShowControl(pParent->GetDlgItem(nCtrlID), bShow);
}

void CDialogHelper::ShowControl(CWnd* pCtrl, BOOL bShow)
{
	if (!pCtrl)
		return;
	
	pCtrl->ShowWindow(bShow ? SW_SHOW : SW_HIDE);
}

void CDialogHelper::ExcludeControls(const CWnd* pParent, CDC* pDC, UINT nCtrlIDFrom, UINT nCtrlIDTo, BOOL bIgnoreCorners)
{
	ASSERT (pParent);
	
	for (UINT nID = nCtrlIDFrom; nID <= nCtrlIDTo; nID++)
		ExcludeChild(pParent, pDC, nID, bIgnoreCorners);
}

void CDialogHelper::ExcludeChildren(const CWnd* pParent, CDC* pDC, BOOL bIgnoreCorners)
{
	ASSERT (pParent);

	CWnd* pChild = pParent->GetWindow(GW_CHILD);
	
	while (pChild)
	{
		ExcludeChild(pParent, pDC, pChild, bIgnoreCorners);
		pChild = pChild->GetWindow(GW_HWNDNEXT);
	}
}

void CDialogHelper::ExcludeChild(const CWnd* pParent, CDC* pDC, const CWnd* pChild, BOOL bIgnoreCorners)
{
	if (!pParent || !pChild)
		return;

	// don't clip transparent controls
	DWORD dwExStyle = pChild->GetExStyle();

	if (pChild->IsWindowVisible() && !(dwExStyle & WS_EX_TRANSPARENT))
	{
		CRect rClip = GetCtrlRect(pParent, pChild);

		if (bIgnoreCorners)
		{
			rClip.DeflateRect(1, 0);
			pDC->ExcludeClipRect(rClip);
			
			rClip.DeflateRect(-1, 1);
		}
			
		pDC->ExcludeClipRect(rClip);
    }
}

void CDialogHelper::ExcludeChild(const CWnd* pParent, CDC* pDC, UINT nCtrlID, BOOL bIgnoreCorners)
{
	ExcludeChild(pParent, pDC, pParent->GetDlgItem(nCtrlID), bIgnoreCorners);
}
