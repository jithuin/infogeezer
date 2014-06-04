// autocombobox.cpp : implementation file
//

#include "stdafx.h"
#include "autocombobox.h"
#include "holdredraw.h"
#include "dialoghelper.h"
#include "autoflag.h"
#include "misc.h"
#include "enstring.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////

void AFXAPI DDX_AutoCBString(CDataExchange* pDX, int nIDC, CString& value)
{
	HWND hWndCtrl = pDX->PrepareCtrl(nIDC);

	if (pDX->m_bSaveAndValidate)
	{
		::DDX_CBString(pDX, nIDC, value);
	}
	else
	{
		// try exact first
		int nIndex = (int)::SendMessage(hWndCtrl, CB_FINDSTRINGEXACT, (WPARAM)-1,
										(LPARAM)(LPCTSTR)value);

		// then partial
		if (nIndex == CB_ERR)
		{
			nIndex = ::SendMessage(hWndCtrl, CB_SELECTSTRING, (WPARAM)-1, (LPARAM)(LPCTSTR)value);

			// if there's a match, check its not partial
			if (nIndex != CB_ERR)
			{
				CString sItem;
				int nLen = ::SendMessage(hWndCtrl, CB_GETLBTEXTLEN, nIndex, 0);
				::SendMessage(hWndCtrl, CB_GETLBTEXT, nIndex, (LPARAM)(LPCTSTR)sItem.GetBuffer(nLen + 1));
				sItem.ReleaseBuffer();

				if (value.CompareNoCase(sItem))
					nIndex = CB_ERR;
			}
		}

		if (nIndex == CB_ERR)
		{
			// add it and then select it if not empty
			if (!value.IsEmpty())
				nIndex = ::SendMessage(hWndCtrl, CB_ADDSTRING, 0, (LPARAM)(LPCTSTR)value);

		}

		::SendMessage(hWndCtrl, CB_SETCURSEL, nIndex, 0L);
	}
}

/////////////////////////////////////////////////////////////////////////////
// CAutoComboListBox

BEGIN_MESSAGE_MAP(CAutoComboBox::CAutoComboListBox, CWnd)
	ON_NOTIFY_EX_RANGE(TTN_NEEDTEXT, 0, 0xFFFF, CAutoComboListBox::OnToolTipNotify)
	ON_MESSAGE(WM_MOUSEMOVE, CAutoComboListBox::OnMouseMove)
	ON_MESSAGE(WM_MOUSELEAVE, CAutoComboListBox::OnMouseLeave)
END_MESSAGE_MAP()

int CAutoComboBox::CAutoComboListBox::OnToolHitTest(CPoint /*point*/, TOOLINFO* pTI) const
{
	pTI->hwnd = m_hWnd;
	pTI->uId = (UINT)m_hWnd;
	GetClientRect(&pTI->rect);

	pTI->lpszText = LPSTR_TEXTCALLBACK;
	pTI->uFlags = TTF_ALWAYSTIP;

	return TRUE;
}

BOOL CAutoComboBox::CAutoComboListBox::OnToolTipNotify( UINT /*id*/, NMHDR* pNMHDR, LRESULT* pResult )
{
	TOOLTIPTEXT* pTTT = (TOOLTIPTEXT*)pNMHDR;

	static CEnString tip(_T("'Ctrl+Del' to remove items"));
	pTTT->lpszText = (LPTSTR)(LPCTSTR)tip;

	::SetWindowPos(pNMHDR->hwndFrom, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);

	*pResult = 0;
	return TRUE;
}

LRESULT CAutoComboBox::CAutoComboListBox::OnMouseMove(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
	if (!m_bTooltipsEnabled)
	{
		EnableToolTips(TRUE);
		m_bTooltipsEnabled = TRUE;

		// track when the cursor leaves the list
		TRACKMOUSEEVENT tme = { 0 };

		tme.cbSize = sizeof(tme);
		tme.hwndTrack = m_hWnd;
		tme.dwFlags = TME_LEAVE;

		TrackMouseEvent(&tme);
	}

	return Default();
}

LRESULT CAutoComboBox::CAutoComboListBox::OnMouseLeave(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
	if (m_bTooltipsEnabled)
	{
		EnableToolTips(FALSE);
		m_bTooltipsEnabled = FALSE;
	}

	return Default();
}

void CAutoComboBox::CAutoComboListBox::EnableToolTips(BOOL bEnable)
{
	m_bTooltipsEnabled = bEnable;

	CWnd::EnableToolTips(bEnable);
}

/////////////////////////////////////////////////////////////////////////////
// CAutoComboBox

CAutoComboBox::CAutoComboBox(DWORD dwFlags) : m_dwFlags(dwFlags), m_bClosingUp(FALSE), m_bEditChange(FALSE)
{
}

CAutoComboBox::~CAutoComboBox()
{
}


BEGIN_MESSAGE_MAP(CAutoComboBox, COwnerdrawComboBoxBase)
	//{{AFX_MSG_MAP(CAutoComboBox)
	ON_WM_SIZE()
	ON_WM_CTLCOLOR()
	//}}AFX_MSG_MAP
	ON_CONTROL_REFLECT_EX(CBN_SELENDCANCEL, OnSelEndCancel)
	ON_CONTROL_REFLECT_EX(CBN_SELENDOK, OnSelEndOK)
	ON_CONTROL_REFLECT_EX(CBN_SELCHANGE, OnSelChange)
	ON_CONTROL_REFLECT_EX(CBN_CLOSEUP, OnCloseUp)
	ON_CONTROL_REFLECT_EX(CBN_DROPDOWN, OnDropDown)
	ON_CONTROL_REFLECT_EX(CBN_EDITCHANGE, OnEditChange)
	ON_NOTIFY_EX_RANGE(TTN_NEEDTEXT, 0, 0xFFFF, CAutoComboListBox::OnToolTipNotify)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CAutoComboBox message handlers

int CAutoComboBox::SetStrings(const CStringArray& aItems) 
{ 
	CComboBox::SetRedraw(FALSE);
	ResetContent(); 
	CComboBox::SetRedraw(TRUE);

	return AddUniqueItems(aItems); 
}

BOOL CAutoComboBox::OnCloseUp()
{
	CAutoFlag af(m_bClosingUp, TRUE);

	if (m_list.GetSafeHwnd())
	{
		m_list.SendMessage(WM_KILLFOCUS);
		m_list.SendMessage(WM_ACTIVATE, WA_INACTIVE);

		m_list.EnableToolTips(FALSE);
		m_list.UnsubclassWindow();
	}

	// notify parent of (possible) selection change
	if (m_bEditChange)
		NotifyParent(CBN_SELCHANGE);
	else
		NotifyParent(CBN_SELENDCANCEL);
	
	return FALSE; // pass to parent
}

BOOL CAutoComboBox::OnDropDown()
{
	return FALSE; // pass to parent
}

BOOL CAutoComboBox::OnEditChange()
{
	m_bEditChange = TRUE;

	return FALSE; // pass to parent
}

BOOL CAutoComboBox::OnSelEndOK()
{
	OnSelChange();
	HandleReturnKey();
	return FALSE; // continue routing
}

BOOL CAutoComboBox::OnSelEndCancel()
{
	// eat this unless we sent it explicitly
	return (!m_bClosingUp);
}

BOOL CAutoComboBox::OnSelChange()
{
	// make sure the edit control is up to date
	if (IsHooked())
	{
		int nSel = GetCurSel();

		if (nSel < GetCount() )
			SetWindowText(GetSelectedItem());
	}

	// eat notification if dropped down
	// ie we don't send selection notifications until the dropdown closes
	return GetDroppedState() && !IsSimpleCombo();
}

CString CAutoComboBox::GetSelectedItem() const
{
	CString sSel;
	int nSel = GetCurSel();

	if (nSel != CB_ERR)
		GetLBText(nSel, sSel);
	
	return sSel;
}

int CAutoComboBox::FindStringExact(int nIndexStart, const CString& sItem, BOOL bCaseSensitive) const
{
	int nFind = nIndexStart; // default
	
	if (!sItem.IsEmpty())
	{
		// because more than one item might exist if were doing a case-sensitive
		// search we can't just stop if the first find doesn't exactly match
		// because there still may be further matches
		BOOL bContinue = TRUE;
		
		while (bContinue)
        {
			int nPrevFind = nFind;
			nFind = COwnerdrawComboBoxBase::FindStringExact(nFind, sItem);
			
			// if no match then definitely done
			if (nFind <= nPrevFind && nFind != nIndexStart)
			{
				nFind = -1;
				bContinue = FALSE;
			}
			else if (!bCaseSensitive)
				bContinue = FALSE;
			else
			{
				// else if (bCaseSensitive)
				ASSERT (nFind != CB_ERR);
				ASSERT (bCaseSensitive);
				
				// test for real exactness because FindStringExact is not case sensitive
				CString sFind;
				GetLBText(nFind, sFind);
				
				bContinue = !(sItem == sFind); // differ in case
			}
        }
	}
	else // special case: look for empty item
	{
		nFind = GetCount();

		while (nFind--)
		{
			CString sLBItem;
			GetLBText(nFind, sLBItem);

			if (sLBItem.IsEmpty())
				break;
		}
	}
	
	return nFind;
}

int CAutoComboBox::DeleteString(LPCTSTR szItem, BOOL bCaseSensitive)
{
	int nItem = FindStringExact(-1, szItem, bCaseSensitive);

	if (nItem != CB_ERR)
		return DeleteString(nItem);

	// else
	return CB_ERR;
}

int CAutoComboBox::DeleteItems(const CStringArray& aItems, BOOL bCaseSensitive)
{
	int nDeleted = 0;
	int nItem = aItems.GetSize();

	while (nItem--)
	{
		if (DeleteString(aItems[nItem], bCaseSensitive) != CB_ERR)
			nDeleted++;
	}

	return nDeleted;
}

BOOL CAutoComboBox::AddEmptyString()
{
	int nItem = FindStringExact(-1, _T(""));

	if (nItem == -1)
	{
		if (AddToStart())
			nItem = COwnerdrawComboBoxBase::InsertString(0, _T(""));
		else
			nItem = COwnerdrawComboBoxBase::AddString(_T(""));
	}

	return (nItem != -1);
}

int CAutoComboBox::AddUniqueItem(const CString& sItem)
{
	return AddUniqueItem(sItem, AddToStart());
}

int CAutoComboBox::AddUniqueItem(const CString& sItem, BOOL bAddToStart)
{
	return InsertUniqueItem(bAddToStart ? 0 : -1, sItem);
}

int CAutoComboBox::InsertUniqueItem(int nIndex, const CString& sNewItem)
{
	if (nIndex < 0 || nIndex > GetCount())
		nIndex = -1; // insert at end if not exists

	CString sItem(sNewItem);
	sItem.TrimLeft();
	sItem.TrimRight();
	
	if (!sItem.IsEmpty())
	{
		int nFind = FindStringExact(-1, sItem, CaseSensitive());
		
		if (nFind != CB_ERR) // items already exists
		{
			CString sLBItem;
			GetLBText(nFind, sLBItem);

			if (nIndex == -1)
				nIndex = nFind; // leave it in it's current position
			
			if (nIndex != nFind || sItem != sLBItem)
			{
				// save selection so we can restore it
				int nSel = GetCurSel();
				CString sSelItem;
				
				if (nSel != CB_ERR)
					GetLBText(nSel, sSelItem);

				// be sure to transfer item data
				DWORD dwItemData = GetItemData(nFind);
				
				DeleteString(nFind); // remove original

				if (COwnerdrawComboBoxBase::GetStyle() & CBS_SORT)
					nIndex = COwnerdrawComboBoxBase::AddString(sItem); // re-insert
				else
					nIndex = COwnerdrawComboBoxBase::InsertString(nIndex, sItem); // re-insert

				SetItemData(nIndex, dwItemData);
				
				if (nIndex != CB_ERR)
					RefreshMaxDropWidth();
				
				// restore selection
				if (nSel != CB_ERR)
					SelectString(-1, sSelItem);
				
				return nIndex;
			}
		}
		else
		{
			if (COwnerdrawComboBoxBase::GetStyle() & CBS_SORT)
				nIndex = COwnerdrawComboBoxBase::AddString(sItem); // re-insert
			else
				nIndex = COwnerdrawComboBoxBase::InsertString(nIndex, sItem); // re-insert
			
			if (nIndex != CB_ERR)
				RefreshMaxDropWidth();
			
			return nIndex;
		}
	}
	
	return CB_ERR; // invalid item
}

void CAutoComboBox::RefreshMaxDropWidth()
{
   CDialogHelper::RefreshMaxDropWidth(*this);
}

void CAutoComboBox::OnSize(UINT nType, int cx, int cy) 
{
	CWnd* pEdit = GetDlgItem(1001);

	// if the edit control does not have the focus then hide the selection
	if (pEdit && pEdit != GetFocus())
	{
		CHoldRedraw hr(*pEdit);
		COwnerdrawComboBoxBase::OnSize(nType, cx, cy);
	
		pEdit->SendMessage(EM_SETSEL, (UINT)-1, 0);
	}
	else
		COwnerdrawComboBoxBase::OnSize(nType, cx, cy);
}

int CAutoComboBox::AddUniqueItems(const CStringArray& aItems)
{
    int nItemCount = aItems.GetSize(), nCount = 0;

	// maintain order
    for (int nItem = 0; nItem < nItemCount; nItem++)
    {
        if (AddUniqueItem(aItems[nItem], FALSE) != CB_ERR)
            nCount++;
    }

    return (nCount ? CB_OKAY : CB_ERR);
}

int CAutoComboBox::AddUniqueItems(const CAutoComboBox& cbItems)
{
    CStringArray aItems;

    if (cbItems.GetItems(aItems))
        return AddUniqueItems(aItems);

    // else
    return 0;
}

int CAutoComboBox::GetItems(CStringArray& aItems) const
{
    int nItem = GetCount();

    aItems.RemoveAll();
    aItems.SetSize(nItem);

    while (nItem--)
    {
        CString sItem;
		GetLBText(nItem, sItem);

        aItems.SetAt(nItem, sItem); // maintain order
    }

    return aItems.GetSize();
}

// messages destined for the edit control
LRESULT CAutoComboBox::WindowProc(HWND /*hRealWnd*/, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_KEYDOWN:
		// <Ctrl> + <Delete>
		if (AllowDelete() && wp == VK_DELETE && GetDroppedState() && Misc::KeyIsPressed(VK_CONTROL))
		{
			if (DeleteSelectedLBItem())
			{
				// eat message else it'll go to the edit window
				return 0L;
			}
		}
		// <Return>
		else if (wp == VK_RETURN)
		{
			if (GetDroppedState() && !IsSimpleCombo())
				ShowDropDown(FALSE);

			HandleReturnKey();
			
			return 0L;
		}
		else if (wp == VK_DOWN && !GetDroppedState() && !IsSimpleCombo() && GetCount())
		{
			ShowDropDown();
			return 0L; // eat
		}
		break;
		
	case WM_GETDLGCODE:
		// special handling of the return key
		if (lp)
		{
			LPMSG pMsg = (LPMSG)lp;

			if (pMsg->message == WM_KEYDOWN && pMsg->wParam == VK_RETURN)
			{
				// can be simple or not dropped
				if (IsSimpleCombo() || !GetDroppedState())
					return DLGC_WANTALLKEYS;
			}
		}
		break;

	case WM_KILLFOCUS:
		if (m_bEditChange)
			HandleReturnKey();
		break;
	}
	
	return CSubclassWnd::Default();
}

void CAutoComboBox::HandleReturnKey()
{
	m_bEditChange = FALSE;

	if (IsHooked())
	{
		CString sEdit;
		GetCWnd()->GetWindowText(sEdit);
		
		int nAdd = AddUniqueItem(sEdit);
		
		if (nAdd != CB_ERR)
			COwnerdrawComboBoxBase::GetParent()->SendMessage(WM_ACB_ITEMADDED, 
												MAKEWPARAM(CWnd::GetDlgCtrlID(), nAdd),
												(LPARAM)(LPCTSTR)sEdit);
		else // send a possible selection change
		{
			if (!sEdit.IsEmpty())
				SelectString(-1, sEdit);
			
			NotifyParent(CBN_SELCHANGE);
		}
	}
}

void CAutoComboBox::NotifyParent(UINT nIDNotify)
{
	CWnd* pParent = CWnd::GetParent();

	if (pParent)
	{
		UINT nID = CWnd::GetDlgCtrlID();
		pParent->SendMessage(WM_COMMAND, MAKEWPARAM(nID, nIDNotify), (LPARAM)m_hWnd);
	}
}

BOOL CAutoComboBox::IsSimpleCombo()
{
	return ((COwnerdrawComboBoxBase::GetStyle() & 0xf) == CBS_SIMPLE);
}

BOOL CAutoComboBox::AllowDelete() const 
{ 
	if (Misc::HasFlag(m_dwFlags, ACBS_ALLOWDELETE))
	{
		return ((COwnerdrawComboBoxBase::GetStyle() & 0xf) != CBS_DROPDOWNLIST);
	}

	else return FALSE;
}

void CAutoComboBox::SetEditMask(LPCTSTR szMask, DWORD dwMaskFlags)
{
	m_eMask.SetMask(szMask, dwMaskFlags);
}

HBRUSH CAutoComboBox::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor) 
{
	HBRUSH hbr = COwnerdrawComboBoxBase::OnCtlColor(pDC, pWnd, nCtlColor);
	
	// hook list box
 	if (nCtlColor == CTLCOLOR_LISTBOX && m_list.GetSafeHwnd() == NULL && AllowDelete())
	{
		m_list.SubclassWindow(pWnd->GetSafeHwnd());

		// enable tooltips for drop-down list boxes
		if (!IsSimpleCombo())
		{
			m_list.EnableToolTips(TRUE);

		//	m_list.SendMessage(WM_SETFOCUS);
		//	m_list.SendMessage(WM_ACTIVATE, WA_ACTIVE);
		}
	}
	
	// and hook edit box
	if (nCtlColor == CTLCOLOR_EDIT && !IsHooked())
	{
		HookWindow(pWnd->GetSafeHwnd());

		// mask
		if (m_eMask.IsMasked())
			m_eMask.SubclassWindow(pWnd->GetSafeHwnd());
	}
	
	return hbr;
}

BOOL CAutoComboBox::DeleteSelectedLBItem()
{
	ASSERT(AllowDelete());
	int nSelItem = m_list.SendMessage(LB_GETCURSEL, 0, 0);
	
	if (nSelItem >= 0)
	{
		CString sCurItem, sSelItem;
		int nCurSel = GetCurSel();
		
		// save existing selection
		if (nCurSel != -1)
			GetWindowText(sCurItem);
		
		GetLBText(nSelItem, sSelItem); // need this for notifying parent
		::SendMessage(GetSafeHwnd(), CB_DELETESTRING, nSelItem, 0);
		
		// restore combo selection
		if (!sCurItem.IsEmpty())
		{
			int nIndex = FindStringExact(-1, sCurItem);
			SetCurSel(nIndex);
		}
		
		// notify parent that we've been fiddling
		COwnerdrawComboBoxBase::GetParent()->SendMessage(WM_ACB_ITEMDELETED, 
												MAKEWPARAM(CWnd::GetDlgCtrlID(), nSelItem), 
												(LPARAM)(LPCTSTR)sSelItem);

		return TRUE;
	}

	// else
	return FALSE;
}

