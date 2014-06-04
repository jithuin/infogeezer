// PreferencesDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "PreferencesDlg.h"

#include "..\shared\winclasses.h"
#include "..\shared\wclassdefines.h"
#include "..\shared\enstring.h"
#include "..\shared\dialoghelper.h"
#include "..\shared\deferwndmove.h"
#include "..\shared\localizer.h"
#include "..\shared\graphicsmisc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPreferencesDlg dialog

// default priority colors
const COLORREF PRIORITYLOWCOLOR = RGB(30, 225, 0);
const COLORREF PRIORITYHIGHCOLOR = RGB(255, 0, 0);
const int MIN_WIDTH = 827;
const int MIN_HEIGHT = 505;

const char PATHDELIM = '>';

CPreferencesDlg::CPreferencesDlg(CShortcutManager* pShortcutMgr, 
								 UINT nMenuID, 
								 const CContentMgr* pContentMgr, 
								 const CImportExportMgr* pExportMgr,
								 CWnd* pParent /*=NULL*/)
	: 
	CPreferencesDlgBase(IDD_PREFERENCES, pParent), 
	m_pageShortcuts(pShortcutMgr, nMenuID, FALSE), 
	m_pageUI(pContentMgr), m_pageFile2(pExportMgr),
	m_sizeCurrent(-1, -1)
{
	CPreferencesDlgBase::AddPage(&m_pageGen);
	CPreferencesDlgBase::AddPage(&m_pageMultiUser);
	CPreferencesDlgBase::AddPage(&m_pageFile);
	CPreferencesDlgBase::AddPage(&m_pageFile2);
	CPreferencesDlgBase::AddPage(&m_pageUI);
	CPreferencesDlgBase::AddPage(&m_pageUITasklist);
	CPreferencesDlgBase::AddPage(&m_pageUITasklistColors);
	CPreferencesDlgBase::AddPage(&m_pageTask);
	CPreferencesDlgBase::AddPage(&m_pageTaskCalc);
	CPreferencesDlgBase::AddPage(&m_pageTaskDef);
	CPreferencesDlgBase::AddPage(&m_pageExport);
	CPreferencesDlgBase::AddPage(&m_pageTool);
	CPreferencesDlgBase::AddPage(&m_pageShortcuts);
	
	ForwardMessage(WM_PTDP_LISTCHANGE);
	ForwardMessage(WM_PUITCP_ATTRIBCHANGE);
	ForwardMessage(WM_PTP_TESTTOOL);
	ForwardMessage(WM_PGP_CLEARMRU);
	ForwardMessage(WM_PGP_CLEANUPDICTIONARY);
	ForwardMessage(WM_PPB_CTRLCHANGE);
	
	LoadPreferences();
}

CPreferencesDlg::~CPreferencesDlg()
{
}

void CPreferencesDlg::DoDataExchange(CDataExchange* pDX)
{
	CPreferencesDlgBase::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPreferencesDlg)
	DDX_Control(pDX, IDC_PAGES, m_tcPages);
	//}}AFX_DATA_MAP
	DDX_Text(pDX, IDC_CATEGORY_TITLE, m_sCategoryTitle);
}

BEGIN_MESSAGE_MAP(CPreferencesDlg, CPreferencesDlgBase)
	//{{AFX_MSG_MAP(CPreferencesDlg)
	ON_BN_CLICKED(IDC_HELP2, OnHelp)
	ON_NOTIFY(TVN_SELCHANGED, IDC_PAGES, OnSelchangedPages)
	ON_BN_CLICKED(IDC_APPLY, OnApply)
	ON_WM_SIZE()
	ON_WM_DESTROY()
	ON_WM_GETMINMAXINFO()
	//}}AFX_MSG_MAP
	ON_WM_HELPINFO()
	ON_REGISTERED_MESSAGE(WM_PTDP_LISTCHANGE, OnDefPageListChange)
	ON_REGISTERED_MESSAGE(WM_PUITCP_ATTRIBCHANGE, OnColourPageAttributeChange)
	ON_REGISTERED_MESSAGE(WM_PTP_TESTTOOL, OnToolPageTestTool)
	ON_REGISTERED_MESSAGE(WM_PGP_CLEARMRU, OnGenPageClearMRU)
	ON_REGISTERED_MESSAGE(WM_PGP_CLEANUPDICTIONARY, OnGenPageCleanupDictionary)
	ON_REGISTERED_MESSAGE(WM_PPB_CTRLCHANGE, OnControlChange)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPreferencesDlg message handlers

BOOL CPreferencesDlg::OnInitDialog() 
{
	CRect rClient;
	GetClientRect(rClient);
	m_sizeCurrent = rClient.Size();
	
	CRect rHost;
	GetDlgItem(IDC_HOSTFRAME)->GetWindowRect(rHost);
	ScreenToClient(rHost);
	rHost.DeflateRect(1, 1);
	
	VERIFY(CreatePPHost(rHost));

	CPreferencesDlgBase::OnInitDialog();
	
	// disable translation of category title because
	// categories will already been translated
	CLocalizer::EnableTranslation(::GetDlgItem(*this, IDC_CATEGORY_TITLE), FALSE);

	m_tcPages.SetIndent(0);

	// add pages to tree control
	AddPage(&m_pageGen, IDS_PREF_GEN);
	AddPage(&m_pageMultiUser, IDS_PREF_MULTIUSER);
	AddPage(&m_pageMultiUser, IDS_PREF_MULTIUSERFILE);
	AddPage(&m_pageMultiUser, IDS_PREF_MULTIUSERSS);
	AddPage(&m_pageFile, IDS_PREF_FILE);
	AddPage(&m_pageFile, IDS_PREF_FILELOAD);
	AddPage(&m_pageFile, IDS_PREF_FILEARCHIVE);
	AddPage(&m_pageFile, IDS_PREF_FILENOTIFY);
	AddPage(&m_pageFile2, IDS_PREF_FILEMORE);
	AddPage(&m_pageFile2, IDS_PREF_FILEBACKUP);
	AddPage(&m_pageFile2, IDS_PREF_FILESAVE);
	AddPage(&m_pageUI, IDS_PREF_UI);
	AddPage(&m_pageUI, IDS_PREF_UIFILTERING);
	AddPage(&m_pageUI, IDS_PREF_UITOOLBAR);
	AddPage(&m_pageUI, IDS_PREF_UISORTING);
	AddPage(&m_pageUI, IDS_PREF_UITABBAR);
	AddPage(&m_pageUI, IDS_PREF_UICOMMENTS);
	AddPage(&m_pageUITasklist, IDS_PREF_UITASKCOLUMNS);
	AddPage(&m_pageUITasklist, IDS_PREF_UITASK);
	AddPage(&m_pageUITasklistColors, IDS_PREF_UITASKCOLOR);
	AddPage(&m_pageTask, IDS_PREF_TASK);
	AddPage(&m_pageTask, IDS_PREF_TASKTIME);
	AddPage(&m_pageTaskCalc, IDS_PREF_TASKCALCS);
	AddPage(&m_pageTaskDef, IDS_PREF_TASKDEF);
	AddPage(&m_pageTaskDef, IDS_PREF_TASKDEFINHERIT);
	AddPage(&m_pageExport, IDS_PREF_EXPORT);
	AddPage(&m_pageTool, IDS_PREF_TOOLS);
	AddPage(&m_pageShortcuts, IDS_PREF_SHORTCUT); 
	
	SynchronizeTree();

	GetDlgItem(IDC_APPLY)->EnableWindow(FALSE);

	// fixup icon
	HICON hIcon = GraphicsMisc::LoadIcon(IDI_PREFERENCES_DIALOG_STD);
	SetIcon(hIcon, FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CPreferencesDlg::OnHelpInfo(HELPINFO* /*pHelpInfo*/)
{
	OnHelp();
	return FALSE;
}

void CPreferencesDlg::SynchronizeTree()
{
	HTREEITEM hti = NULL;
	
	if (m_mapPP2HTI.Lookup(GetActivePage(), (void*&)hti) && hti)
		m_tcPages.SelectItem(hti);
}

BOOL CPreferencesDlg::PreTranslateMessage(MSG* pMsg) 
{
	// special handling for hotkeys
	if (CWinClasses::IsClass(pMsg->hwnd, WC_HOTKEY))
		return FALSE;
	
	return CPreferencesDlgBase::PreTranslateMessage(pMsg);
}

void CPreferencesDlg::OnHelp() 
{
	// who is the active page?
	int nSel = GetActiveIndex();
	CString sHelpPage = "prefs";
	
	switch (nSel)
	{
	case PREFPAGE_GEN:
		sHelpPage += "#General";
		break;
		
	case PREFPAGE_MULTIUSER:
		sHelpPage += "#MultiUserControl";
		break;
		
	case PREFPAGE_FILE:
		sHelpPage += "#FileActions";
		break;

	case PREFPAGE_FILE2:
		sHelpPage += "#FileActionsmore";
		break;
		
	case PREFPAGE_UI:
		sHelpPage += "#UserInterfacegtGeneral";
		break;
		
	case PREFPAGE_UITASK:
		sHelpPage += "#UserInterfacegtTasklistgtAttributes";
		break;
		
	case PREFPAGE_UIFONTCOLOR:
		sHelpPage += "#UserInterfacegtTasklistgtFontsandColours";
		break;
		
	case PREFPAGE_TASK:
		sHelpPage += "#TasksgtAttributeCalculations";
		break;
		
	case PREFPAGE_TASKDEF:
		sHelpPage += "#TasksgtDefaultTaskAttributes";
		break;
		
	case PREFPAGE_TOOL:
		sHelpPage += "#Tools";
		break;
		
	case PREFPAGE_SHORTCUT:
		sHelpPage += "#KeyboardShortcuts";
		break;

	default:
		ASSERT(0);
		break;
	}
	
	AfxGetApp()->WinHelp((DWORD)(LPCTSTR)sHelpPage);
}

void CPreferencesDlg::AddPage(CPreferencesPageBase* pPage, UINT nIDPath)
{
	CEnString sPath(nIDPath);
	
	if (FindPage(pPage) != -1) 
	{
		HTREEITEM htiParent = TVI_ROOT; // default
		int nFind = sPath.Find(PATHDELIM);
		
		while (nFind != -1)
		{
			CString sParent = sPath.Left(nFind);
			sPath = sPath.Mid(nFind + 1);
			
			// see if parent already exists
			HTREEITEM htiParentParent = htiParent;
			htiParent = m_tcPages.GetChildItem(htiParentParent);
			
			while (htiParent)
			{
				if (sParent.CompareNoCase(m_tcPages.GetItemText(htiParent)) == 0)
					break;
				
				htiParent = m_tcPages.GetNextItem(htiParent, TVGN_NEXT);
			}
			
			if (!htiParent)
			{
				htiParent = m_tcPages.InsertItem(sParent, htiParentParent);
				
				// embolden root items
				if (htiParentParent == TVI_ROOT)
					m_tcPages.SetItemState(htiParent, TVIS_BOLD, TVIS_BOLD);
			}
			
			nFind = sPath.Find(PATHDELIM);
		}
		
		HTREEITEM hti = m_tcPages.InsertItem(sPath, htiParent); // whatever's left
		m_tcPages.EnsureVisible(hti);
		
		// embolden root items
		if (htiParent == TVI_ROOT)
			m_tcPages.SetItemState(hti, TVIS_BOLD, TVIS_BOLD);
		
		// map both ways
		m_tcPages.SetItemData(hti, (DWORD)pPage);

		// don't remap the page if already done
		HTREEITEM htiMap = NULL;

		if (!m_mapPP2HTI.Lookup(pPage, (void*&)htiMap))
			m_mapPP2HTI[(void*)pPage] = (void*)hti;

		// set page background to window back
		pPage->SetBackgroundColor(GetSysColor(COLOR_WINDOW));
	}
}

void CPreferencesDlg::OnSelchangedPages(NMHDR* /*pNMHDR*/, LRESULT* pResult) 
{
	HTREEITEM htiSel = m_tcPages.GetSelectedItem();
	
	while (m_tcPages.ItemHasChildren(htiSel))
		htiSel = m_tcPages.GetChildItem(htiSel);
	
	CPreferencesPageBase* pPage = (CPreferencesPageBase*)m_tcPages.GetItemData(htiSel);
	ASSERT (pPage);
	
	if (pPage && CPreferencesDlgBase::SetActivePage(pPage))
	{
		// special handling:
		// if we're activating the defaults page then update the
		// priority colors
		if (pPage == &m_pageTaskDef)
		{
			CDWordArray aColors;
			m_pageUITasklistColors.GetPriorityColors(aColors);
			m_pageTaskDef.SetPriorityColors(aColors);
		}
		
		// update caption
		m_sCategoryTitle = _T("   ") + GetItemPath(htiSel);
		UpdateData(FALSE);
	}
	
	m_tcPages.SetFocus();
	
	*pResult = 0;
}

BOOL CPreferencesDlg::SetActivePage(int nPage)
{
	if (CPreferencesDlgBase::SetActivePage(nPage))
	{
		SynchronizeTree();
		
		return TRUE;
	}
	
	return FALSE;
}

BOOL CPreferencesDlg::IsPrefTab(HWND hWnd) const
{
	return (hWnd == m_pageGen || 
			hWnd == m_pageMultiUser || 
			hWnd == m_pageFile || 
			hWnd == m_pageFile2 || 
			hWnd == m_pageUI || 
			hWnd == m_pageUITasklist || 
			hWnd == m_pageUITasklistColors || 
			hWnd == m_pageTask || 
			hWnd == m_pageTaskCalc || 
			hWnd == m_pageTaskDef || 
			hWnd == m_pageExport || 
			hWnd == m_pageTool || 
			hWnd == m_pageShortcuts);
}

CString CPreferencesDlg::GetItemPath(HTREEITEM hti) const
{
	CString sPath = m_tcPages.GetItemText(hti);
	hti = m_tcPages.GetParentItem(hti);
	
	while (hti)
	{
		sPath = m_tcPages.GetItemText(hti) + " > " + sPath;
		hti = m_tcPages.GetParentItem(hti);
	}
	
	return sPath;
}

LRESULT CPreferencesDlg::OnDefPageListChange(WPARAM wp, LPARAM lp)
{
	// decode params
	int nList = LOWORD(wp);
	BOOL bAdd = HIWORD(wp);
	LPCTSTR szItem = (LPCTSTR)lp;

	// pass on to attribute color page
	if (!EnsurePageCreated(&m_pageUITasklistColors))
		return 0L;
	
	switch (nList)
	{
	case PTDP_ALLOCBY:
		if (bAdd)
			m_pageUITasklistColors.AddAttribute(TDCA_ALLOCBY, szItem);
		else
			m_pageUITasklistColors.DeleteAttribute(TDCA_ALLOCBY, szItem);
		break;

	case PTDP_ALLOCTO:
		if (bAdd)
			m_pageUITasklistColors.AddAttribute(TDCA_ALLOCTO, szItem);
		else
			m_pageUITasklistColors.DeleteAttribute(TDCA_ALLOCTO, szItem);
		break;

	case PTDP_STATUS:
		if (bAdd)
			m_pageUITasklistColors.AddAttribute(TDCA_STATUS, szItem);
		else
			m_pageUITasklistColors.DeleteAttribute(TDCA_STATUS, szItem);
		break;

	case PTDP_CATEGORY:
		if (bAdd)
			m_pageUITasklistColors.AddAttribute(TDCA_CATEGORY, szItem);
		else
			m_pageUITasklistColors.DeleteAttribute(TDCA_CATEGORY, szItem);
		break;
	}

	// forward on to main app
	return AfxGetMainWnd()->SendMessage(WM_PTDP_LISTCHANGE, wp, lp);
}

LRESULT CPreferencesDlg::OnColourPageAttributeChange(WPARAM wp, LPARAM lp)
{
	// decode params
	BOOL bAdd = LOWORD(wp);
	TDC_ATTRIBUTE nAttrib = (TDC_ATTRIBUTE)HIWORD(wp);
	LPCTSTR szItem = (LPCTSTR)lp;

	// pass on to task def page
	if (!EnsurePageCreated(&m_pageTaskDef))
		return 0L;

	switch (nAttrib)
	{
	case TDCA_ALLOCBY:
		if (bAdd)
			m_pageTaskDef.AddListItem(PTDP_ALLOCBY, szItem);
		else
			m_pageTaskDef.DeleteListItem(PTDP_ALLOCBY, szItem);
		break;

	case TDCA_ALLOCTO:
		if (bAdd)
			m_pageTaskDef.AddListItem(PTDP_ALLOCTO, szItem);
		else
			m_pageTaskDef.DeleteListItem(PTDP_ALLOCTO, szItem);
		break;

	case TDCA_STATUS:
		if (bAdd)
			m_pageTaskDef.AddListItem(PTDP_STATUS, szItem);
		else
			m_pageTaskDef.DeleteListItem(PTDP_STATUS, szItem);
		break;

	case TDCA_CATEGORY:
		if (bAdd)
			m_pageTaskDef.AddListItem(PTDP_CATEGORY, szItem);
		else
			m_pageTaskDef.DeleteListItem(PTDP_CATEGORY, szItem);
		break;
	}

	// forward on to main app
	// wrap it up as a WM_PTDP_LISTCHANGE to simplify the handling
	return AfxGetMainWnd()->SendMessage(WM_PTDP_LISTCHANGE, MAKEWPARAM(PTDP_CATEGORY, bAdd), lp);
}

LRESULT CPreferencesDlg::OnToolPageTestTool(WPARAM wp, LPARAM lp)
{
	// forward on to main app
	return AfxGetMainWnd()->SendMessage(WM_PTP_TESTTOOL, wp, lp);
}

LRESULT CPreferencesDlg::OnGenPageClearMRU(WPARAM wp, LPARAM lp)
{
	// forward on to main app
	return AfxGetMainWnd()->SendMessage(WM_PGP_CLEARMRU, wp, lp);
}

LRESULT CPreferencesDlg::OnGenPageCleanupDictionary(WPARAM wp, LPARAM lp)
{
	// forward on to main app
	return AfxGetMainWnd()->SendMessage(WM_PGP_CLEANUPDICTIONARY, wp, lp);
}

void CPreferencesDlg::OnApply() 
{
	CPreferencesDlgBase::OnApply();	

	GetDlgItem(IDC_APPLY)->EnableWindow(FALSE);
}

LRESULT CPreferencesDlg::OnControlChange(WPARAM /*wp*/, LPARAM /*lp*/)
{
	GetDlgItem(IDC_APPLY)->EnableWindow(TRUE);

	return 0L;
}

void CPreferencesDlg::OnSize(UINT nType, int cx, int cy) 
{
	CPreferencesDlgBase::OnSize(nType, cx, cy);

	if (!m_pphost.GetSafeHwnd())
		return;
	
	Resize(cx, cy);
}

void CPreferencesDlg::OnGetMinMaxInfo(MINMAXINFO FAR* lpMMI) 
{
	CPreferencesDlgBase::OnGetMinMaxInfo(lpMMI);

	lpMMI->ptMinTrackSize.x = MIN_WIDTH;
	lpMMI->ptMinTrackSize.y = MIN_HEIGHT;
}

void CPreferencesDlg::Resize(int cx, int cy)
{
	if (cx == 0 || cy == 0)
	{
		CRect rClient;
		GetClientRect(rClient);

		cx = rClient.Width();
		cy = rClient.Height();
	}

	// calculate deltas
	int nDX = cx - m_sizeCurrent.cx;
	int nDY = cy - m_sizeCurrent.cy;

	if (nDX == 0 && nDY == 0)
		return;

	m_sizeCurrent.cx = cx;
	m_sizeCurrent.cy = cy;
	
	CDeferWndMove dwm(10);
	
	// offset buttons
	dwm.OffsetCtrl(this, IDC_HELP2, 0, nDY);
	dwm.OffsetCtrl(this, IDOK, nDX, nDY);
	dwm.OffsetCtrl(this, IDCANCEL, nDX, nDY);
	dwm.OffsetCtrl(this, IDC_APPLY, nDX, nDY);

	// resize tree
	dwm.ResizeCtrl(this, IDC_PAGES, 0, nDY);
	
	// PPHost and label and border
	dwm.ResizeCtrl(this, m_pphost.GetDlgCtrlID(), nDX, nDY);
	dwm.ResizeCtrl(this, IDC_CATEGORY_TITLE, nDX, 0);
	dwm.ResizeCtrl(this, IDC_HOSTFRAME, nDX, nDY);

	GetDlgItem(IDC_HOSTFRAME)->Invalidate(TRUE);

}
