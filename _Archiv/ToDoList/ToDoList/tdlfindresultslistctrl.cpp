// tdlfindresultslistctrl.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "tdlfindresultslistctrl.h"

#include "..\shared\preferences.h"
#include "..\shared\misc.h"
#include "..\shared\graphicsmisc.h"

#include "..\3rdparty\shellicons.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#ifndef LVS_EX_LABELTIP
#define LVS_EX_LABELTIP     0x00004000
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDLFindResultsListCtrl

CTDLFindResultsListCtrl::CTDLFindResultsListCtrl()
{
}

CTDLFindResultsListCtrl::~CTDLFindResultsListCtrl()
{
	GraphicsMisc::VerifyDeleteObject(m_fontBold);
	GraphicsMisc::VerifyDeleteObject(m_fontStrike);
}


BEGIN_MESSAGE_MAP(CTDLFindResultsListCtrl, CEnListCtrl)
	//{{AFX_MSG_MAP(CTDLFindResultsListCtrl)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLFindResultsListCtrl message handlers

void CTDLFindResultsListCtrl::PreSubclassWindow()
{
	CEnListCtrl::PreSubclassWindow();

	// setup up result list
	InsertColumn(0, CEnString(IDS_FT_TASK));
	InsertColumn(1, CEnString(IDS_FT_WHATMATCHED), LVCFMT_LEFT, 250);

	ListView_SetExtendedListViewStyleEx(*this, LVS_EX_ONECLICKACTIVATE, LVS_EX_ONECLICKACTIVATE);
	ListView_SetExtendedListViewStyleEx(*this, LVS_EX_UNDERLINEHOT, LVS_EX_UNDERLINEHOT);
	ListView_SetExtendedListViewStyleEx(*this, LVS_EX_LABELTIP, LVS_EX_LABELTIP);

	SetHotCursor(AfxGetApp()->LoadStandardCursor(IDC_ARROW));
	SetView(LVS_REPORT);
	EnableHeaderTracking(FALSE);
	SetFirstColumnStretchy(TRUE);
	RefreshUserPreferences();
}

int CTDLFindResultsListCtrl::GetResultCount() const
{
	return GetResultCount(NULL);
}

int CTDLFindResultsListCtrl::GetResultCount(const CFilteredToDoCtrl* pTDC) const
{
	int nCount = 0;
	int nItem = GetItemCount();
	
	while (nItem--)
	{
		FTDRESULT* pRes = GetResult(nItem);

		if (pRes && (pTDC == NULL || pRes->pTDC == pTDC))
			nCount++;
	}
	
	return nCount;
}

int CTDLFindResultsListCtrl::GetAllResults(CFTDResultsArray& aResults) const
{
	return GetResults(NULL, aResults);
}

int CTDLFindResultsListCtrl::GetResults(const CFilteredToDoCtrl* pTDC, CFTDResultsArray& aResults) const
{
	int nNumItem = GetItemCount();
	int nCount = 0;

	aResults.RemoveAll();
	aResults.SetSize(GetResultCount(pTDC));

	for (int nItem = 0; nItem < nNumItem; nItem++)
	{
		FTDRESULT* pRes = GetResult(nItem);

		if (pRes && (pTDC == NULL || pRes->pTDC == pTDC))
		{
			aResults.SetAt(nCount, *pRes);
			nCount++;
		}
	}

	return aResults.GetSize();
}

int CTDLFindResultsListCtrl::GetResultIDs(const CFilteredToDoCtrl* pTDC, CDWordArray& aTaskIDs) const
{
	CFTDResultsArray aResults;
	int nNumRes = GetResults(pTDC, aResults);

	for (int nRes = 0; nRes < nNumRes; nRes++)
		aTaskIDs.Add(aResults[nRes].dwTaskID);

	return aResults.GetSize();
}

void CTDLFindResultsListCtrl::DeleteResults(const CFilteredToDoCtrl* pTDC)
{
	// work backwards from the last list res
	int nItem = GetItemCount();

	while (nItem--)
	{
		FTDRESULT* pRes = GetResult(nItem);

		if (pRes && pRes->pTDC == pTDC)
		{
			DeleteItem(nItem);
			delete pRes;
		}
	}
}

void CTDLFindResultsListCtrl::DeleteAllResults()
{
	// work backwards from the last list res
	int nItem = GetItemCount();

	while (nItem--)
	{
		FTDRESULT* pRes = GetResult(nItem);
		delete pRes;

		DeleteItem(nItem);
	}
}

void CTDLFindResultsListCtrl::DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct) 
{
	// we only need handle header items
	int nItem = lpDrawItemStruct->itemID;
	FTDRESULT* pRes = GetResult(nItem);

	if (pRes)
	{
		CEnListCtrl::DrawItem(lpDrawItemStruct);

		// draw shortcut overlay for references
		if (pRes->bRef)
		{
			CDC* pDC = CDC::FromHandle(lpDrawItemStruct->hDC);
			ShellIcons::DrawIcon(pDC, ShellIcons::SI_SHORTCUT, CRect(lpDrawItemStruct->rcItem).TopLeft());
		}

		return;
	}

	// else
	CString sText = GetItemText(nItem, 0);

	if (!sText.IsEmpty())
	{
		CDC* pDC = CDC::FromHandle(lpDrawItemStruct->hDC);
		
		CRect rLabel, rItem;
		GetItemRect(nItem, rLabel, LVIR_LABEL);
		GetItemRect(nItem, rItem, LVIR_BOUNDS);
		
		rLabel.left -= 2;
		
		int nSave = pDC->SaveDC();
		
		pDC->SelectObject(&m_fontBold);
		pDC->SetTextColor(0);
		pDC->SetBkColor(GetSysColor(COLOR_WINDOW));
		pDC->SetBkMode(OPAQUE);
		pDC->DrawText(sText, rLabel, DT_TOP | DT_LEFT | DT_END_ELLIPSIS | DT_NOPREFIX);
		
		// draw a horizontal dividing line
		pDC->DrawText(sText, rLabel, DT_TOP | DT_LEFT | DT_END_ELLIPSIS | DT_NOPREFIX | DT_CALCRECT);
		pDC->FillSolidRect(rLabel.right + 6, rLabel.top + rLabel.Height() / 2, rItem.right - rLabel.right, 1, GetSysColor(COLOR_3DSHADOW));
		
		pDC->RestoreDC(nSave);
	}
}

COLORREF CTDLFindResultsListCtrl::GetItemTextColor(int nItem, int nSubItem, BOOL bSelected, BOOL bDropHighlighted, BOOL bWndFocus) const
{
   if (!bSelected && !bDropHighlighted)
   {
      FTDRESULT* pRes = (FTDRESULT*)GetItemData(nItem);

      if (pRes && pRes->bDone && nSubItem == 0 && m_crDone != (COLORREF)-1)
         return m_crDone;
   }

   // else
   return CEnListCtrl::GetItemTextColor(nItem, nSubItem, bSelected, bDropHighlighted, bWndFocus);
}

CFont* CTDLFindResultsListCtrl::GetItemFont(int nItem, int nSubItem)
{
	FTDRESULT* pRes = GetResult(nItem);

	if (pRes && pRes->bDone && nSubItem == 0 && m_fontStrike.GetSafeHandle())
		return &m_fontStrike;

	// else
	return CEnListCtrl::GetItemFont(nItem, nSubItem);
}

void CTDLFindResultsListCtrl::RefreshUserPreferences()
{
	CPreferences prefs;
	
	// update user completed tasks colour
	if (prefs.GetProfileInt(_T("Preferences"), _T("SpecifyDoneColor"), FALSE))
		m_crDone = (COLORREF)prefs.GetProfileInt(_T("Preferences\\Colors"), _T("TaskDone"), -1);
	else
		m_crDone = (COLORREF)-1;

	// update strike thru font
	if (prefs.GetProfileInt(_T("Preferences"), _T("StrikethroughDone"), FALSE))
	{
		if (!m_fontStrike.GetSafeHandle())
			GraphicsMisc::CreateFont(m_fontStrike, (HFONT)SendMessage(WM_GETFONT), GMFS_STRIKETHRU);
	}
	else
		GraphicsMisc::VerifyDeleteObject(m_fontStrike);

	if (IsWindowVisible())
		Invalidate();
}

int CTDLFindResultsListCtrl::AddResult(const SEARCHRESULT& result, LPCTSTR szTask, /*LPCTSTR szPath,*/ const CFilteredToDoCtrl* pTDC)
{
	int nPos = GetItemCount();
		
	// add result
	int nIndex = InsertItem(nPos, szTask);
	SetItemText(nIndex, 1, Misc::FormatArray(result.aMatched));
	UpdateWindow();
		
	// map identifying data
	FTDRESULT* pRes = new FTDRESULT(result, pTDC);
	SetItemData(nIndex, (DWORD)pRes);

	return nIndex;
}

BOOL CTDLFindResultsListCtrl::AddHeaderRow(LPCTSTR szText, BOOL bSpaceAbove)
{
	int nPos = GetItemCount();

	// add space above?
	if (bSpaceAbove)
	{
		int nIndex = InsertItem(nPos, _T(""));
		SetItemData(nIndex, 0);
		nPos++;
	}
	
	// add result
	int nIndex = InsertItem(nPos, szText);
	SetItemData(nIndex, 0);

	// bold font for rendering
	if (m_fontBold.GetSafeHandle() == NULL)
		GraphicsMisc::CreateFont(m_fontBold, (HFONT)SendMessage(WM_GETFONT), GMFS_BOLD);

	return (nIndex != -1);
}

