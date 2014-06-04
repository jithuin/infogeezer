// ownerdrawcomboboxbase.cpp : implementation file
//

#include "stdafx.h"
#include "dlgunits.h"
#include "ownerdrawcomboboxbase.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// COwnerdrawComboBoxBase

COwnerdrawComboBoxBase::COwnerdrawComboBoxBase() : m_bItemHeightSet(FALSE)
{
}

COwnerdrawComboBoxBase::~COwnerdrawComboBoxBase()
{
}


BEGIN_MESSAGE_MAP(COwnerdrawComboBoxBase, CComboBox)
	//{{AFX_MSG_MAP(COwnerdrawComboBoxBase)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
	ON_WM_CREATE()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// COwnerdrawComboBoxBase message handlers

void COwnerdrawComboBoxBase::DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct) 
{
	CDC dc;
	
	if (!dc.Attach(lpDrawItemStruct->hDC))
		return;

	int nDC = dc.SaveDC();

	BOOL bSelected = (lpDrawItemStruct->itemState & ODS_SELECTED);

	COLORREF crBack = GetSysColor(IsWindowEnabled() ? (bSelected ? COLOR_HIGHLIGHT : COLOR_WINDOW) : COLOR_3DFACE);
	COLORREF crText = GetSysColor(IsWindowEnabled() ? (bSelected ? COLOR_HIGHLIGHTTEXT : COLOR_WINDOWTEXT) : COLOR_GRAYTEXT);

	CRect rItem(lpDrawItemStruct->rcItem);

	dc.FillSolidRect(rItem, crBack);
	dc.SetTextColor(crText);

	if (lpDrawItemStruct->itemAction & ODA_FOCUS)
	{
		dc.DrawFocusRect(rItem);
	}
	else if (lpDrawItemStruct->itemAction & ODA_DRAWENTIRE)
	{
		if (lpDrawItemStruct->itemState & ODS_FOCUS)
			dc.DrawFocusRect(rItem);
	}

	// draw the item
	rItem.DeflateRect(2, 1);

	if (0 <= (int)lpDrawItemStruct->itemID)	// Any item selected?
	{

		// draw text
		CString sText;

		if (GetStyle() & CBS_HASSTRINGS)
		{
			GetLBText(lpDrawItemStruct->itemID, sText);
		}

		// virtual call
		DrawItemText(dc, rItem, 
					lpDrawItemStruct->itemID, 
					lpDrawItemStruct->itemState,
					lpDrawItemStruct->itemData, 
					sText, 
					TRUE);
	}
	else
	{
		DrawItemText(dc, rItem, -1, 0, 0, _T(""), FALSE);
	}

	// Restore the DC state
	dc.RestoreDC(nDC);
	dc.Detach();
}

void COwnerdrawComboBoxBase::DrawItemText(CDC& dc, const CRect& rect, int /*nItem*/, UINT /*nItemState*/,
										  DWORD /*dwItemData*/, const CString& sItem, BOOL /*bList*/)
{
	if (!sItem.IsEmpty())
	{
		dc.DrawText(sItem, (LPRECT)(LPCRECT)rect, DT_SINGLELINE | DT_VCENTER | DT_END_ELLIPSIS | DT_NOPREFIX);
	}
}

void COwnerdrawComboBoxBase::PreSubclassWindow() 
{
	if (!m_bItemHeightSet) 
	{
		m_bItemHeightSet = TRUE;
		
		CDlgUnits dlu(CComboBox::GetParent());
		SetItemHeight(-1, dlu.ToPixelsY(9)); 
	}
	
	CComboBox::PreSubclassWindow();
}

int COwnerdrawComboBoxBase::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CComboBox::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	if (!m_bItemHeightSet) 
	{
		m_bItemHeightSet = TRUE;
		
		CDlgUnits dlu(CComboBox::GetParent());
		SetItemHeight(-1, dlu.ToPixelsY(9)); 
	}
	
	return 0;
}

