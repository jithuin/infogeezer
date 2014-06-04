// PrefererencesShortcutsPage.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "PreferencesShortcutsPage.h"
#include "todoctrl.h"
#include "tdcstatic.h"

#include "..\shared\winclasses.h"
#include "..\shared\wclassdefines.h"
#include "..\shared\enstring.h"
#include "..\shared\holdredraw.h"
#include "..\shared\treectrlhelper.h"
#include "..\shared\enmenu.h"
#include "..\shared\misc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPreferencesShortcutsPage property page

CPreferencesShortcutsPage::CPreferencesShortcutsPage(CShortcutManager* pMgr, UINT nMenuID, BOOL bIgnoreGrayedItems) 
	: CPreferencesPageBase(CPreferencesShortcutsPage::IDD), 
	m_nMenuID(nMenuID), 
	m_pShortcutMgr(pMgr), 
	m_bIgnoreGrayedItems(bIgnoreGrayedItems),
	m_tcCommands(NCGS_SHOWHEADER)
{
	//{{AFX_DATA_INIT(CPreferencesShortcutsPage)
	m_sOtherCmdID = _T("");
	m_bShowCommandIDs = FALSE;
	//}}AFX_DATA_INIT
}

CPreferencesShortcutsPage::~CPreferencesShortcutsPage()
{
}

void CPreferencesShortcutsPage::DoDataExchange(CDataExchange* pDX)
{
	CPreferencesPageBase::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPreferencesShortcutsPage)
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_CURHOTKEY, m_hkCur);
	DDX_Control(pDX, IDC_COMMANDS, m_tcCommands);
	DDX_Control(pDX, IDC_NEWHOTKEY, m_hkNew);
	DDX_Text(pDX, IDC_INUSE, m_sOtherCmdID);
	DDX_Check(pDX, IDC_SHOWCMDIDS, m_bShowCommandIDs);
}

BEGIN_MESSAGE_MAP(CPreferencesShortcutsPage, CPreferencesPageBase)
	//{{AFX_MSG_MAP(CPreferencesShortcutsPage)
	ON_WM_SIZE()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_ASSIGNSHORTCUT, OnAssignshortcut)
	ON_BN_CLICKED(IDC_SHOWCMDIDS, OnShowCmdIDs)
	ON_BN_CLICKED(IDC_COPYALL, OnCopyall)
	ON_NOTIFY(TVN_SELCHANGED, IDC_COMMANDS, OnSelchangedShortcuts)
	ON_EN_CHANGE(IDC_NEWHOTKEY, OnChangeShortcut)
	ON_REGISTERED_MESSAGE(WM_NCG_DRAWITEMCOLUMN, OnGutterDrawItem)
	ON_REGISTERED_MESSAGE(WM_NCG_RECALCCOLWIDTH, OnGutterRecalcColWidth)
	ON_REGISTERED_MESSAGE(WM_NCG_GETITEMCOLORS, OnGutterGetItemColors)
	ON_NOTIFY(NM_CUSTOMDRAW, IDC_COMMANDS, OnTreeCustomDraw)
	ON_WM_HELPINFO()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPreferencesShortcutsPage message handlers

BOOL CPreferencesShortcutsPage::OnInitDialog()  
{
	CPreferencesPageBase::OnInitDialog();

	m_tcCommands.AddGutterColumn(PSP_SHORTCUTCOLUMNID, CEnString(IDS_PSP_SHORTCUT));

	m_tcCommands.SetGutterColumnHeaderTitle(NCG_CLIENTCOLUMNID, CEnString(IDS_PSP_MENUITEM));
	m_tcCommands.ShowGutterPosColumn(FALSE);
	m_tcCommands.SetGridlineColor(OTC_GRIDCOLOR);
	m_tcCommands.EnableGutterColumnHeaderClicking(PSP_SHORTCUTCOLUMNID, FALSE);
	m_tcCommands.EnableGutterColumnHeaderClicking(NCG_CLIENTCOLUMNID, FALSE);
	
	// show the 'copy all' button
	GetDlgItem(IDC_COPYALL)->ShowWindow(m_bShowCommandIDs ? SW_SHOW : SW_HIDE);
	GetDlgItem(IDC_COPYALL)->EnableWindow(m_bShowCommandIDs);

	if (m_nMenuID && m_pShortcutMgr)
	{
		m_tcCommands.SendMessage(WM_NULL);
		CHoldRedraw hr(m_tcCommands);

		CEnMenu menu;
		HTREEITEM htiFirst = NULL;

		if (menu.LoadMenu(m_nMenuID, NULL, TRUE))
		{
			for (int nPos = 0; nPos < (int)menu.GetMenuItemCount(); nPos++)
			{
				HTREEITEM hti = AddMenuItem(NULL, &menu, nPos);

				if (!htiFirst)
					htiFirst = hti;

				m_tcCommands.Expand(hti, TVE_EXPAND);
			}
		}

		if (m_bShowCommandIDs)
			AddCommandIDsToTree(TVI_ROOT, TRUE);

		// add miscellaneous un-editable shortcuts
		AddMiscShortcuts();

		if (htiFirst)
			m_tcCommands.EnsureVisible(htiFirst);

		// init hotkey controls
		// note: we no longer pass in m_pShortcutMgr->GetInvalidComb() because
		// the hotkey control does a less than perfect job of respecting these
		m_hkNew.SetRules(0, 0);
	}
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

HTREEITEM CPreferencesShortcutsPage::AddMenuItem(HTREEITEM htiParent, const CMenu* pMenu, int nPos)
{
	CString sItem;
	pMenu->GetMenuString(nPos, sItem, MF_BYPOSITION);

	// remove '&'
	sItem.Replace(_T("&&"), _T("~~~"));
	sItem.Replace(_T("&"), _T(""));
	sItem.Replace(_T("~~~"), _T("&"));
	
	// remove everything after '\t'
	int nTab = sItem.Find('\t');
	
	if (nTab >= 0)
		sItem = sItem.Left(nTab);
	
	if (!sItem.IsEmpty())
	{
		// don't add disabled items if req
		if (m_bIgnoreGrayedItems && (pMenu->GetMenuState(nPos, MF_BYPOSITION) & MF_GRAYED))
			return NULL;

		HTREEITEM hti = m_tcCommands.InsertItem(sItem, htiParent ? htiParent : TVI_ROOT);

		if (hti)
		{
			UINT nCmdID = pMenu->GetMenuItemID(nPos);
			
			if (nCmdID == (UINT)-1) // submenu
			{
				// make top level items bold
				if (!htiParent)
					m_tcCommands.SetItemState(hti, TVIS_BOLD, TVIS_BOLD);

				CMenu* pSubMenu = pMenu->GetSubMenu(nPos);

				if (pSubMenu)
				{
					for (int nSubPos = 0; nSubPos < (int)pSubMenu->GetMenuItemCount(); nSubPos++)
						m_tcCommands.Expand(AddMenuItem(hti, pSubMenu, nSubPos), TVE_EXPAND);
				}
			}
			else if (!IsMiscCommandID(nCmdID)) // fixes a bug where misc ids were being saved
			{
				DWORD dwShortcut = m_pShortcutMgr->GetShortcut(nCmdID);

				if (dwShortcut)
				{
					m_mapID2Shortcut[nCmdID] = dwShortcut;
					m_mapShortcut2HTI[dwShortcut] = hti;
				}

				m_tcCommands.SetItemData(hti, nCmdID);
			}

			return hti;
		}
	}

	return NULL;
}

void CPreferencesShortcutsPage::AddMiscShortcuts()
{
	if (!NUM_MISCSHORTCUTS)
		return;

	// Add parent placeholder
	HTREEITEM htiParent = m_tcCommands.InsertItem(CEnString(IDS_MISCSHORTCUTS), TVI_ROOT);
	m_tcCommands.SetItemState(htiParent, TVIS_BOLD, TVIS_BOLD);

	// add children
	for (int nItem = 0; nItem < NUM_MISCSHORTCUTS; nItem++)
	{
		DWORD dwShortcut = MISC_SHORTCUTS[nItem].dwShortcut;
		
		if (dwShortcut)
		{
			CEnString sMisc(MISC_SHORTCUTS[nItem].nIDShortcut);
			HTREEITEM hti = m_tcCommands.InsertItem(sMisc, htiParent);

			// make fake command IDs so it does not intersect with normal IDs
			UINT nCmdID = MAKELONG(0, nItem + 1);
			
			if (dwShortcut)
			{
				m_mapID2Shortcut[nCmdID] = dwShortcut;
				m_mapShortcut2HTI[dwShortcut] = hti;
			}
			
			m_tcCommands.SetItemData(hti, nCmdID);
		}		
	}
	
	// expand parent
	m_tcCommands.Expand(htiParent, TVE_EXPAND);
}

void CPreferencesShortcutsPage::OnSelchangedShortcuts(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_TREEVIEW* pNMTreeView = (NM_TREEVIEW*)pNMHDR;

	UINT nCmdID = (UINT)pNMTreeView->itemNew.lParam;
	DWORD dwShortcut = 0;

	m_mapID2Shortcut.Lookup(nCmdID, dwShortcut);

	WORD wVKeyCode = LOWORD(dwShortcut);
	WORD wModifiers = HIWORD(dwShortcut);

	m_hkCur.SetHotKey(wVKeyCode, wModifiers);
	m_hkNew.SetHotKey(wVKeyCode, wModifiers);

	// if it's a misc item then disable keys
	BOOL bMisc = IsMiscCommandID(nCmdID);
	BOOL bCanHaveShortcut = !bMisc && !m_tcCommands.ItemHasChildren(pNMTreeView->itemNew.hItem);

	m_hkNew.EnableWindow(bCanHaveShortcut);
	GetDlgItem(IDC_CURLABEL)->EnableWindow(bCanHaveShortcut);
	GetDlgItem(IDC_NEWLABEL)->EnableWindow(bCanHaveShortcut);

	// test for reserved shortcut
	// and disable assign button as feedback
	if (bCanHaveShortcut && CToDoCtrl::IsReservedShortcut(dwShortcut))
	{
		bCanHaveShortcut = FALSE;
		m_sOtherCmdID.LoadString(IDS_PSP_RESERVED);
	}
	else
		m_sOtherCmdID.Empty();

	GetDlgItem(IDC_ASSIGNSHORTCUT)->EnableWindow(bCanHaveShortcut);
	UpdateData(FALSE);
	
	*pResult = 0;
}

BOOL CPreferencesShortcutsPage::IsMiscCommandID(UINT nCmdID)
{
	return (LOWORD(nCmdID) == 0) && (HIWORD(nCmdID) != 0);
}

void CPreferencesShortcutsPage::OnOK()
{
	// copy all the changes to m_pShortcutMgr
	// except for reserved shortcuts
	POSITION pos = m_mapID2Shortcut.GetStartPosition();

	while (pos)
	{
		UINT nCmdID = 0;
		DWORD dwShortcut = 0;

		m_mapID2Shortcut.GetNextAssoc(pos, nCmdID, dwShortcut);
		ASSERT (nCmdID);

		if (!IsMiscCommandID(nCmdID))
			m_pShortcutMgr->SetShortcut(nCmdID, dwShortcut);
	}

	m_pShortcutMgr->SaveSettings();
}

BOOL CPreferencesShortcutsPage::OnHelpInfo(HELPINFO* pHelpInfo)
{
	// eat this if the hotkeyctrl has the focus
	if (GetFocus() == &m_hkNew)
		return FALSE;

	// else
	return CPreferencesPageBase::OnHelpInfo(pHelpInfo);
}

void CPreferencesShortcutsPage::OnAssignshortcut() 
{
	HTREEITEM htiSel = m_tcCommands.GetSelectedItem();

	if (!htiSel)
		return;
	
	UINT nCmdID = m_tcCommands.GetItemData(htiSel);

	if (nCmdID)
	{
		// remove any shortcut currently assigned to nCmdID
		DWORD dwPrevSC = 0;

		if (m_mapID2Shortcut.Lookup(nCmdID, dwPrevSC))
			m_mapShortcut2HTI.RemoveKey(dwPrevSC);

		WORD wVKeyCode = 0, wModifiers = 0;
		m_hkNew.GetHotKey(wVKeyCode, wModifiers);

		DWORD dwShortcut = MAKELONG(wVKeyCode, wModifiers);
		
		// handle special case where user is explicitly deleting a 
		// shortcut
		if (!dwShortcut)
			dwShortcut = NO_SHORTCUT;
		// else if anyone has this shortcut we must remove their mapping first
		else
		{
			HTREEITEM htiOther = NULL;

			if (m_mapShortcut2HTI.Lookup(dwShortcut, htiOther) && htiOther)
			{
				UINT nOtherCmdID = m_tcCommands.GetItemData(htiOther);

				if (nOtherCmdID)
					m_mapID2Shortcut.RemoveKey(nOtherCmdID);
			}
		}

		// update maps
		m_mapID2Shortcut[nCmdID] = dwShortcut;

		if (dwShortcut != NO_SHORTCUT)
		{
			m_mapShortcut2HTI[dwShortcut] = htiSel;
			m_hkCur.SetHotKey(LOWORD(dwShortcut), HIWORD(dwShortcut));
		}

		m_sOtherCmdID.Empty();

		m_tcCommands.RecalcGutter();
		m_tcCommands.RedrawGutter();

		CTreeCtrlHelper(m_tcCommands).InvalidateItem(htiSel);

		UpdateData(FALSE);
	}

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesShortcutsPage::OnChangeShortcut()
{
	HTREEITEM htiSel = m_tcCommands.GetSelectedItem();

	if (!htiSel)
		return;

	UINT nCmdID = m_tcCommands.GetItemData(htiSel);

	WORD wVKeyCode = 0, wModifiers = 0;
	m_hkNew.GetHotKey(wVKeyCode, wModifiers);

	// validate modifiers but only if a 'main' key has been pressed
	if (wVKeyCode)
	{
		WORD wValidModifiers = m_pShortcutMgr->ValidateModifiers(wModifiers, wVKeyCode);

		if (wValidModifiers != wModifiers)
		{
			wModifiers = wValidModifiers;
			m_hkNew.SetHotKey(wVKeyCode, wModifiers);
		}
	}

	DWORD dwShortcut = MAKELONG(wVKeyCode, wModifiers);

	// if anyone has this shortcut we show who it is
	BOOL bReserved = FALSE;
	HTREEITEM htiOther = NULL;

	m_mapShortcut2HTI.Lookup(dwShortcut, htiOther);

	if (CToDoCtrl::IsReservedShortcut(dwShortcut))
	{
		m_sOtherCmdID.LoadString(IDS_PSP_RESERVED);
		bReserved = TRUE;
	}
	else if (htiOther && m_tcCommands.GetItemData(htiOther) != nCmdID)
	{
		m_sOtherCmdID.Format(IDS_PSP_CURRENTLYASSIGNED, m_tcCommands.GetItemText(htiOther));
	}
	else
		m_sOtherCmdID.Empty();

	GetDlgItem(IDC_ASSIGNSHORTCUT)->EnableWindow(!bReserved);
	UpdateData(FALSE);

	CPreferencesPageBase::OnControlChange();
}

LRESULT CPreferencesShortcutsPage::OnGutterDrawItem(WPARAM /*wParam*/, LPARAM lParam)
{
	NCGDRAWITEM* pNCGDI = (NCGDRAWITEM*)lParam;

	if (pNCGDI->nColID == PSP_SHORTCUTCOLUMNID)
	{
		CRect rItem(pNCGDI->rItem);
		HTREEITEM hti = (HTREEITEM)pNCGDI->dwItem;

		if (m_tcCommands.ItemHasChildren(hti))
		{
			pNCGDI->pDC->FillSolidRect(rItem, ::GetSysColor(COLOR_3DHIGHLIGHT));
			pNCGDI->pDC->FillSolidRect(rItem.right - 1, rItem.top, 1, rItem.Height(), OTC_GRIDCOLOR);
		}
		else
		{
			UINT nCmdID = m_tcCommands.GetItemData(hti);
			DWORD dwShortcut = 0;
		
			m_mapID2Shortcut.Lookup(nCmdID, dwShortcut);

			if (dwShortcut)
			{
				rItem.left += 3;

				// test for reserved shortcut and mark in red
				if (CToDoCtrl::IsReservedShortcut(dwShortcut) && !IsMiscCommandID(nCmdID))
					pNCGDI->pDC->SetTextColor(255);

				CString sText = m_pShortcutMgr->GetShortcutText(dwShortcut);

				if (!sText.IsEmpty())
					pNCGDI->pDC->DrawText(sText, rItem, DT_SINGLELINE | DT_VCENTER | DT_LEFT);

				rItem.left -= 3;
			}
			
			// vertical divider
			pNCGDI->pDC->FillSolidRect(rItem.right - 1, rItem.top, 1, rItem.Height(), OTC_GRIDCOLOR);
		}

		// horz divider
		rItem.top = rItem.bottom - 1;
		pNCGDI->pDC->FillSolidRect(rItem, OTC_GRIDCOLOR);
	
		return TRUE; // we handled it
	}

	return FALSE;
}

LRESULT CPreferencesShortcutsPage::OnGutterRecalcColWidth(WPARAM /*wParam*/, LPARAM lParam)
{
	NCGRECALCCOLUMN* pNCRC = (NCGRECALCCOLUMN*)lParam;

	if (pNCRC->nColID == PSP_SHORTCUTCOLUMNID)
	{
		int nLongest = 0;
		HTREEITEM hti = m_tcCommands.GetNextItem(TVI_ROOT, TVGN_CHILD);

		while (hti)
		{
			int nWidth = GetLongestShortcutText(hti, pNCRC->pDC);
			nLongest = max(nLongest, nWidth);

			hti = m_tcCommands.GetNextItem(hti, TVGN_NEXT);
		}

		if (nLongest)
			nLongest += 6; // some padding

		pNCRC->nWidth = max(40, nLongest);

		return TRUE; // we handled it
	}

	return FALSE;
}

LRESULT CPreferencesShortcutsPage::OnGutterGetItemColors(WPARAM /*wParam*/, LPARAM lParam)
{
	NCGITEMCOLORS* pColors = (NCGITEMCOLORS*)lParam;
	HTREEITEM hti = (HTREEITEM)pColors->dwItem;

	if (m_tcCommands.GetSelectedItem() == hti)
	{
		pColors->crBack = GetSysColor(COLOR_HIGHLIGHT);
		pColors->crText = GetSysColor(COLOR_HIGHLIGHTTEXT);
		pColors->bBackSet = pColors->bTextSet = TRUE;
	}

	return 0L;
}

int CPreferencesShortcutsPage::GetLongestShortcutText(HTREEITEM hti, CDC* pDC)
{
	int nLongest = 0;
	int nCmdID = m_tcCommands.GetItemData(hti);

	if (nCmdID)
	{
		DWORD dwShortcut = 0;
		m_mapID2Shortcut.Lookup(nCmdID, dwShortcut);

		if (dwShortcut)
		{
			CString sShortcut = m_pShortcutMgr->GetShortcutText(dwShortcut);
			nLongest = sShortcut.IsEmpty() ? 0 : pDC->GetTextExtent(sShortcut).cx;
		}
	}

	HTREEITEM htiChild = m_tcCommands.GetChildItem(hti);

	while (htiChild)
	{
		int nWidth = GetLongestShortcutText(htiChild, pDC);
		nLongest = max(nLongest, nWidth);

		htiChild = m_tcCommands.GetNextItem(htiChild, TVGN_NEXT);
	}

	return nLongest;
}

void CPreferencesShortcutsPage::OnTreeCustomDraw(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMCUSTOMDRAW* pNMCD = (NMCUSTOMDRAW*)pNMHDR;

	if (pNMCD->dwDrawStage == CDDS_PREPAINT)
		*pResult |= CDRF_NOTIFYITEMDRAW | CDRF_NOTIFYPOSTPAINT;	
		
	else if (pNMCD->dwDrawStage == CDDS_ITEMPREPAINT)
	{
		NMTVCUSTOMDRAW* pTVCD = (NMTVCUSTOMDRAW*)pNMCD;

		// set colors
		HTREEITEM hti = (HTREEITEM)pTVCD->nmcd.dwItemSpec;

		if (m_tcCommands.ItemHasChildren(hti)) // popup menu
		{
			CRect rItem, rText;
			m_tcCommands.GetItemRect(hti, rItem, FALSE);
			m_tcCommands.GetItemRect(hti, rText, TRUE);
			rItem.left = rText.left - 2;

			CDC* pDC = CDC::FromHandle(pNMCD->hdc);
			pDC->FillSolidRect(rItem, GetSysColor(COLOR_3DSHADOW));
			pDC->SetBkMode(TRANSPARENT);

			pTVCD->clrTextBk = GetSysColor(COLOR_3DSHADOW);
			pTVCD->clrText = 0;

			*pResult |= CDRF_NEWFONT;
		}
		else if (pNMCD->uItemState & CDIS_SELECTED)
		{
			pTVCD->clrTextBk = GetSysColor(COLOR_HIGHLIGHT);
			pTVCD->clrText = GetSysColor(COLOR_HIGHLIGHTTEXT);

			*pResult |= CDRF_NEWFONT;
		}
		else // test for reserved shortcut
		{
			DWORD dwShortcut = 0;
			UINT nCmdID = pTVCD->nmcd.lItemlParam;
			m_mapID2Shortcut.Lookup(nCmdID, dwShortcut);

			if (CToDoCtrl::IsReservedShortcut(dwShortcut) && !IsMiscCommandID(nCmdID))
			{
				pTVCD->clrText = 255;
				*pResult |= CDRF_NEWFONT;
			}
		}
	}
	else if (pNMCD->dwDrawStage == CDDS_ITEMPOSTPAINT)
	{
		CDC* pDC = CDC::FromHandle(pNMCD->hdc);
		NMTVCUSTOMDRAW* pTVCD = (NMTVCUSTOMDRAW*)pNMCD;

		CRect rItem(pNMCD->rc);
		rItem.top = rItem.bottom - 1;

		// if the bottom of the text coincides with the bottom of the 
		// item and we have the then take care not to draw over the focus rect
		if (pNMCD->uItemState & CDIS_FOCUS)
		{
			HTREEITEM hti = (HTREEITEM)pTVCD->nmcd.dwItemSpec;

			CRect rText;
			m_tcCommands.GetItemRect(hti, rText, TRUE);

			if (rText.bottom == rItem.bottom)
			{
				pDC->FillSolidRect(rItem.left, rItem.bottom - 1, rText.left - rItem.left, 1, OTC_GRIDCOLOR);
				pDC->FillSolidRect(rText.right, rItem.bottom - 1, rItem.right - rText.right, 1, OTC_GRIDCOLOR);
			}
			else
				pDC->FillSolidRect(rItem, OTC_GRIDCOLOR);
		}
		else
			pDC->FillSolidRect(rItem, OTC_GRIDCOLOR);
	}
}


BOOL CPreferencesShortcutsPage::PreTranslateMessage(MSG* pMsg) 
{
	// special handling for hotkeys
	if (CWinClasses::IsClass(pMsg->hwnd, WC_HOTKEY))
		return FALSE;

	// handle delete key when tree has the focus
	switch (pMsg->message)
	{
	case WM_KEYDOWN:
		switch (pMsg->wParam)
		{
		case VK_DELETE:
			{
				if (GetFocus() == &m_tcCommands)
				{
					HTREEITEM htiSel = m_tcCommands.GetSelectedItem();

					if (htiSel)
					{
						UINT nCmdID = m_tcCommands.GetItemData(htiSel);

						if (nCmdID)
						{
							DWORD dwShortcut = 0;

							if (m_mapID2Shortcut.Lookup(nCmdID, dwShortcut))
								m_mapShortcut2HTI.RemoveKey(dwShortcut);

							m_mapID2Shortcut[nCmdID] = 0;
						}
					}

					m_hkCur.SetHotKey(0, 0);
					m_hkNew.SetHotKey(0, 0);
					m_sOtherCmdID.Empty();

					m_tcCommands.RecalcGutter();
					m_tcCommands.RedrawGutter();
				}
			}
			break;
		}
		break;
	}
	
	return CPreferencesPageBase::PreTranslateMessage(pMsg);
}

void CPreferencesShortcutsPage::LoadPreferences(const CPreferences& prefs)
{
	m_bShowCommandIDs = prefs.GetProfileInt(_T("KeyboardShortcuts"), _T("ShowCommandIDs"), FALSE);
}

void CPreferencesShortcutsPage::SavePreferences(CPreferences& prefs)
{
	prefs.WriteProfileInt(_T("KeyboardShortcuts"), _T("ShowCommandIDs"), m_bShowCommandIDs);
}

void CPreferencesShortcutsPage::OnShowCmdIDs() 
{
	UpdateData();

	// show the 'copy all' button
	GetDlgItem(IDC_COPYALL)->ShowWindow(m_bShowCommandIDs ? SW_SHOW : SW_HIDE);
	GetDlgItem(IDC_COPYALL)->EnableWindow(m_bShowCommandIDs);

//	CHoldRedraw hr(m_tcCommands);
	AddCommandIDsToTree(TVI_ROOT, m_bShowCommandIDs);
}

void CPreferencesShortcutsPage::AddCommandIDsToTree(HTREEITEM hti, BOOL bAdd)
{
	if (!hti)
		return;

	if (hti != TVI_ROOT)
	{
		CString sItem = m_tcCommands.GetItemText(hti);
		UINT nCmdID = m_tcCommands.GetItemData(hti);

		if (nCmdID && !IsMiscCommandID(nCmdID))
		{
			CString sCmdID;
			sCmdID.Format(_T(" (%d)"), nCmdID);

			if (bAdd)
			{
				m_tcCommands.SetItemText(hti, sItem + sCmdID);
			}
			else
			{
				// strip off command ID
				int nFind = sItem.Find(sCmdID);
				ASSERT(nFind != -1);

				m_tcCommands.SetItemText(hti, sItem.Left(nFind));
			}
		}

		// siblings
		AddCommandIDsToTree(m_tcCommands.GetNextItem(hti, TVGN_NEXT), bAdd);
	}

	// children
	AddCommandIDsToTree(m_tcCommands.GetChildItem(hti), bAdd);
}

void CPreferencesShortcutsPage::OnCopyall() 
{
	CString sOutput;
	
	if (CopyItem(TVI_ROOT, sOutput))
	{
		VERIFY(Misc::CopyTexttoClipboard(sOutput, *this)); 
	}
}

BOOL CPreferencesShortcutsPage::CopyItem(HTREEITEM hti, CString& sOutput)
{
	if (!hti)
		return FALSE;

	// copy self
	if (hti != TVI_ROOT)
	{
		if (!m_tcCommands.ItemHasChildren(hti))
		{
			// ignore Reserved menu commands
			if (IsMiscCommandID(m_tcCommands.GetItemData(hti)))
				return FALSE;

			CString sItem = m_tcCommands.GetItemText(hti);
			ASSERT(!sItem.IsEmpty());

			if (!sItem.IsEmpty())
			{
				// build path by recursing up the parent chain
				HTREEITEM htiParent = m_tcCommands.GetParentItem(hti);

				while (htiParent)
				{
					CString sParent = m_tcCommands.GetItemText(htiParent);
					ASSERT(!sParent.IsEmpty());
					
					if (!sParent.IsEmpty())
						sItem = sParent + _T(" > ") + sItem;

					// parent's parent
					htiParent = m_tcCommands.GetParentItem(htiParent);
				}

				sOutput += sItem;
				sOutput += _T("\r\n");
			}
		}
	}

	// then children
	CopyItem(m_tcCommands.GetChildItem(hti), sOutput);

	// then siblings
	if (hti != TVI_ROOT)
	{
		// add a spacer between top-level items
		if (m_tcCommands.GetParentItem(hti) == NULL)
			sOutput += _T("\r\n");

		CopyItem(m_tcCommands.GetNextItem(hti, TVGN_NEXT), sOutput);
	}

	return (!sOutput.IsEmpty());
}

void CPreferencesShortcutsPage::OnSize(UINT nType, int cx, int cy) 
{
	CPreferencesPageBase::OnSize(nType, cx, cy);
	
	if (m_tcCommands.GetSafeHwnd())
	{
		// calculate border
		CPoint ptBorders = CDialogHelper::GetCtrlRect(this, IDC_COMMANDS).TopLeft();
		
		// calc offsets
		int nXOffset = cx - (CDialogHelper::GetCtrlRect(this, IDC_NEWHOTKEY).right + ptBorders.x);
		int nYOffset = cy - (CDialogHelper::GetCtrlRect(this, IDC_COMMANDS).bottom + ptBorders.y);

		// move controls
		CDialogHelper::OffsetCtrl(this, IDC_CURLABEL, nXOffset);
		CDialogHelper::OffsetCtrl(this, IDC_CURHOTKEY, nXOffset);
		CDialogHelper::OffsetCtrl(this, IDC_NEWLABEL, nXOffset);
		CDialogHelper::OffsetCtrl(this, IDC_NEWHOTKEY, nXOffset);
		CDialogHelper::OffsetCtrl(this, IDC_ASSIGNSHORTCUT, nXOffset);
		
		CDialogHelper::OffsetCtrl(this, IDC_COPYALL, nXOffset, nYOffset);
		CDialogHelper::OffsetCtrl(this, IDC_SHOWCMDIDS, nXOffset, nYOffset);

		CDialogHelper::ResizeCtrl(this, IDC_COMMANDS, nXOffset, nYOffset);
	}
}
