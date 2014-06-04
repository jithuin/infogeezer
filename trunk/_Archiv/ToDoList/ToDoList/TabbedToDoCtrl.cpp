// Fi M_BlISlteredToDoCtrl.cpp: implementation of the CTabbedToDoCtrl class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TabbedToDoCtrl.h"
#include "todoitem.h"
#include "resource.h"
#include "tdcstatic.h"
#include "tdcmsg.h"
#include "tdccustomattributehelper.h"
#include "tdltaskicondlg.h"

#include "..\shared\holdredraw.h"
#include "..\shared\datehelper.h"
#include "..\shared\enstring.h"
#include "..\shared\preferences.h"
#include "..\shared\deferwndmove.h"
#include "..\shared\autoflag.h"
#include "..\shared\holdredraw.h"
#include "..\shared\osversion.h"
#include "..\shared\graphicsmisc.h"
#include "..\shared\iuiextension.h"

#include <math.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

#ifndef LVS_EX_DOUBLEBUFFER
#define LVS_EX_DOUBLEBUFFER 0x00010000
#endif

#ifndef LVS_EX_LABELTIP
#define LVS_EX_LABELTIP     0x00004000
#endif

const UINT SORTWIDTH = 10;
const COLORREF NO_COLOR = (COLORREF)-1;

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CTabbedToDoCtrl::CTabbedToDoCtrl(CContentMgr& mgr, const CONTENTFORMAT& cfDefault) :
	CToDoCtrl(mgr, cfDefault), 
	m_bTreeNeedResort(FALSE),
	m_bTaskColorChange(FALSE),
	m_bUpdatingExtensions(FALSE)
{
	// add extra controls to implement list-view
	for (int nCtrl = 0; nCtrl < NUM_FTDCCTRLS; nCtrl++)
	{
		const TDCCONTROL& ctrl = FTDCCONTROLS[nCtrl];

		AddRCControl(_T("CONTROL"), ctrl.szClass, CString((LPCTSTR)ctrl.nIDCaption), 
					ctrl.dwStyle, ctrl.dwExStyle,
					ctrl.nX, ctrl.nY, ctrl.nCx, ctrl.nCy, ctrl.nID);
	}

	// tab is on by default
	m_aStyles.SetAt(TDCS_SHOWTREELISTBAR, 1);
}

CTabbedToDoCtrl::~CTabbedToDoCtrl()
{
	// cleanup extension views
	int nView = m_aExtViews.GetSize();

	while (nView--)
	{
		IUIExtensionWindow* pExtWnd = m_aExtViews[nView];

		if (pExtWnd)
			pExtWnd->Release();
	}

	m_aExtViews.RemoveAll();
}

BEGIN_MESSAGE_MAP(CTabbedToDoCtrl, CToDoCtrl)
//{{AFX_MSG_MAP(CTabbedToDoCtrl)
	ON_WM_SETCURSOR()
	ON_WM_DESTROY()
	//}}AFX_MSG_MAP
	ON_REGISTERED_MESSAGE(WM_TDCN_VIEWPRECHANGE, OnPreTabViewChange)
	ON_REGISTERED_MESSAGE(WM_TDCN_VIEWPOSTCHANGE, OnPostTabViewChange)
	ON_NOTIFY(NM_CUSTOMDRAW, 0, OnListHeaderCustomDraw)
	ON_NOTIFY(NM_RCLICK, 0, OnRClickListHeader)
	ON_NOTIFY(LVN_COLUMNCLICK, IDC_FTC_TASKLIST, OnClickListHeader)
	ON_NOTIFY(NM_CLICK, IDC_FTC_TASKLIST, OnListClick)
	ON_NOTIFY(NM_DBLCLK, IDC_FTC_TASKLIST, OnListDblClick)
	ON_NOTIFY(NM_KEYDOWN, IDC_FTC_TASKLIST, OnListKeyDown)
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_FTC_TASKLIST, OnListSelChanged)
	ON_WM_MEASUREITEM()
	ON_WM_DRAWITEM()
	ON_NOTIFY(LVN_GETINFOTIP, IDC_FTC_TASKLIST, OnListGetInfoTip)
	ON_NOTIFY(LVN_GETDISPINFO, IDC_FTC_TASKLIST, OnListGetDispInfo)
	ON_REGISTERED_MESSAGE(WM_TLDT_DROP, OnDropObject)
	ON_REGISTERED_MESSAGE(WM_PCANCELEDIT, OnEditCancel)
	ON_REGISTERED_MESSAGE(WM_NCG_WIDTHCHANGE, OnGutterWidthChange)
	ON_REGISTERED_MESSAGE(WM_NCG_RECALCCOLWIDTH, OnGutterRecalcColWidth)
	ON_REGISTERED_MESSAGE(WM_IUI_SELECTTASK, OnUIExtSelectTask)
	ON_REGISTERED_MESSAGE(WM_IUI_MODIFYSELECTEDTASK, OnUIExtModifySelectedTask)
	ON_WM_ERASEBKGND()
END_MESSAGE_MAP()

///////////////////////////////////////////////////////////////////////////

void CTabbedToDoCtrl::DoDataExchange(CDataExchange* pDX)
{
	CToDoCtrl::DoDataExchange(pDX);
	
	DDX_Control(pDX, IDC_FTC_TASKLIST, m_list);
	DDX_Control(pDX, IDC_FTC_TABCTRL, m_tabViews);
}

BOOL CTabbedToDoCtrl::OnInitDialog()
{
	CToDoCtrl::OnInitDialog();

	ListView_SetExtendedListViewStyleEx(m_list, 
										LVS_EX_FULLROWSELECT | LVS_EX_DOUBLEBUFFER, 
										LVS_EX_FULLROWSELECT | LVS_EX_DOUBLEBUFFER);
	m_dtList.Register(&m_list, this);

	// prevent the list overwriting the label edit
	m_list.ModifyStyle(0, WS_CLIPSIBLINGS);

	// and hook it
	ScHookWindow(m_list);

	// add all columns
	BuildListColumns();
		
	m_tabViews.AttachView(m_tree, FTCV_TASKTREE, CEnString(IDS_TASKTREE), GraphicsMisc::LoadIcon(IDI_TASKTREE_STD), NULL);
	m_tabViews.AttachView(m_list, FTCV_TASKLIST, CEnString(IDS_LISTVIEW), GraphicsMisc::LoadIcon(IDI_LISTVIEW_STD), NewViewData());

	Resize();

	return FALSE;
}

BOOL CTabbedToDoCtrl::PreTranslateMessage(MSG* pMsg) 
{
	return CToDoCtrl::PreTranslateMessage(pMsg);
}

void CTabbedToDoCtrl::SetUITheme(const UITHEME& theme)
{
	CToDoCtrl::SetUITheme(theme);

	m_tabViews.SetBackgroundColor(theme.crAppBackLight);
}

BOOL CTabbedToDoCtrl::LoadTasks(const CTaskFile& file)
{
	CPreferences prefs;

	BOOL bSuccess = CToDoCtrl::LoadTasks(file);

	// reload last view
	if (GetView() == FTCV_UNSET)
	{
		CString sKey = GetPreferencesKey(); // no subkey
		
		if (!sKey.IsEmpty()) // first time
		{
			CPreferences prefs;
			FTC_VIEW nView = (FTC_VIEW)prefs.GetProfileInt(sKey, _T("View"), FTCV_UNSET);

			if ((nView != FTCV_UNSET) && (nView != GetView()))
				SetView(nView);

			// clear the view so we don't keep restoring it
			prefs.WriteProfileInt(sKey, _T("View"), FTCV_UNSET);		
		}
	}
	return bSuccess;
}

void CTabbedToDoCtrl::OnDestroy() 
{
	if (GetView() != FTCV_UNSET)
	{
		CPreferences prefs;
		CString sKey = GetPreferencesKey(); // no subkey
		
		// save view
		if (!sKey.IsEmpty())
			prefs.WriteProfileInt(sKey, _T("View"), GetView());

		// extensions
		int nView = m_aExtViews.GetSize();

		if (nView)
		{
			CString sKey = GetPreferencesKey(_T("UIExtensions"));

			while (nView--)
			{
				IUIExtensionWindow* pExtWnd = m_aExtViews[nView];

				if (pExtWnd)
					pExtWnd->SavePreferences(&prefs, sKey);
			}
		}
	}
		
	CToDoCtrl::OnDestroy();
}

void CTabbedToDoCtrl::BuildListColumns(BOOL bResizeCols)
{
	while (m_list.DeleteColumn(0));
	
	// we handle title column separately
	int nPos = 0;

	for (int nCol = 0; nCol < NUM_LATEST_COLUMNS - 1; nCol++)
	{
		const TDCCOLUMN& col = COLUMNS[nCol];

		// insert custom columns before dependency
		if (col.nColID == TDCC_DEPENDENCY)
		{
			for (int nAttrib = 0; nAttrib < m_aCustomAttribDefs.GetSize(); nAttrib++)
			{
				const TDCCUSTOMATTRIBUTEDEFINITION& attribDef = m_aCustomAttribDefs[nAttrib];
				int nColAlign = GetListColumnAlignment(attribDef.nColumnAlignment);

				int nIndex = m_list.InsertColumn(nPos++, _T(""), nColAlign, 1);
				ASSERT(nIndex >= 0);

				// set column item data
				TDC_COLUMN nColID = attribDef.GetColumnID();
				m_list.SetColumnItemData(nIndex, nColID);
			}
		}
		
		int nColAlign = GetListColumnAlignment(col.nAlignment);
		int nIndex = m_list.InsertColumn(nPos++, _T(""), nColAlign, 10);
		ASSERT(nIndex >= 0);

		// set column item data
		m_list.SetColumnItemData(nIndex, col.nColID);
	}

	// title column
	if (HasStyle(TDCS_RIGHTSIDECOLUMNS))
	{
		int nIndex = m_list.InsertColumn(0, _T(""), LVCFMT_LEFT, 10);
		m_list.SetColumnItemData(nIndex, TDCC_CLIENT);
	}
	else
	{
		int nIndex = m_list.InsertColumn(nPos, _T(""), LVCFMT_LEFT, 10);
		m_list.SetColumnItemData(nIndex, TDCC_CLIENT);
	}

	if (bResizeCols)
		UpdateListColumnWidths();
}

int CTabbedToDoCtrl::GetListColumnAlignment(int nDTAlign)
{
	switch (nDTAlign)
	{
	case DT_RIGHT:	return LVCFMT_RIGHT;
	case DT_CENTER:	return LVCFMT_CENTER;
	default:		return LVCFMT_LEFT; // all else
	}
}


void CTabbedToDoCtrl::OnRClickListHeader(NMHDR* /*pNMHDR*/, LRESULT* pResult)
{
	// forward on to parent
	const MSG* pMsg = GetCurrentMessage();
	LPARAM lPos = MAKELPARAM(pMsg->pt.x, pMsg->pt.y);

	GetParent()->SendMessage(WM_CONTEXTMENU, (WPARAM)GetSafeHwnd(), lPos);

	*pResult = 0;
}

void CTabbedToDoCtrl::OnClickListHeader(NMHDR* pNMHDR, LRESULT* pResult)
{
	ASSERT(InListView());

	NMLISTVIEW* pNMLV = (NMLISTVIEW*)pNMHDR;
	
	int nCol = pNMLV->iSubItem;
	TDC_COLUMN nColID = GetListColumnID(nCol);
	BOOL bSortable = FALSE;

	TDCCOLUMN* pCol = GetListColumn(nCol);

	if (pCol)
		bSortable = pCol->bSortable;

	else if (CTDCCustomAttributeHelper::IsCustomColumn(nColID))
		bSortable = CTDCCustomAttributeHelper::IsColumnSortable(nColID, m_aCustomAttribDefs);

	if (bSortable)
	{
		VIEWDATA* pLVData = GetActiveViewData();
		
		const TDSORTCOLUMNS& sort = pLVData->sort;
		TDC_COLUMN nPrev = sort.nBy1;
		
		Sort(nColID);
		
		// notify parent
		if (sort.nBy1 != nPrev)
			GetParent()->SendMessage(WM_TDCN_SORT, GetDlgCtrlID(), MAKELPARAM((WORD)nPrev, (WORD)sort.nBy1));
	}

	*pResult = 0;
}

void CTabbedToDoCtrl::OnListGetDispInfo(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMLVDISPINFO* lplvdi = (NMLVDISPINFO*)pNMHDR;
	*pResult = 0;

	UINT nMask = lplvdi->item.mask;
	DWORD dwTaskID = (DWORD)lplvdi->item.lParam;

	if ((nMask & LVIF_TEXT) &&  m_dwEditingID != dwTaskID)
	{
		TODOITEM* pTDI = m_data.GetTask(dwTaskID);

		// it's possible that the task does not exist if it's just been 
		// deleted from the tree view
		if (!pTDI)
			return;

		// all else
		lplvdi->item.pszText = (LPTSTR)(LPCTSTR)pTDI->sTitle;
	}

	if (nMask & LVIF_IMAGE)
	{
		if (!HasStyle(TDCS_TREETASKICONS))
			lplvdi->item.iImage = -1;
		else
		{
			const TODOITEM* pTDI = NULL;
			const TODOSTRUCTURE* pTDS = NULL;
			
			if (!m_data.GetTask(dwTaskID, pTDI, pTDS))
				return;

			BOOL bHasChildren = pTDS->HasSubTasks();
			int nImage = -1;
			
			if (!pTDI->sIcon.IsEmpty())
				nImage = m_ilTaskIcons.GetImageIndex(pTDI->sIcon);
			
			else if (HasStyle(TDCS_SHOWPARENTSASFOLDERS) && bHasChildren)
				nImage = 0;
			
			lplvdi->item.iImage = nImage;
		}
	}
}

TDC_COLUMN CTabbedToDoCtrl::GetListColumnID(int nCol) const
{
	return (TDC_COLUMN)m_list.GetColumnItemData(nCol);
}

TDCCOLUMN* CTabbedToDoCtrl::GetListColumn(int nCol) const
{
	TDC_COLUMN nColID = GetListColumnID(nCol);

	nCol = NUM_LATEST_COLUMNS;
	
	while (nCol--)
	{
		if (COLUMNS[nCol].nColID == nColID)
			return &COLUMNS[nCol];
	}

	// else
	return NULL;
}

int CTabbedToDoCtrl::GetListColumnIndex(TDC_COLUMN nColID) const
{
	int nCol = m_list.GetColumnCount();

	while (nCol--)
	{
		if (GetListColumnID(nCol) == nColID)
			return nCol;
	}

	ASSERT(0);
	return -1;
}

void CTabbedToDoCtrl::SetVisibleColumns(const CTDCColumnIDArray& aColumns)
{
	CToDoCtrl::SetVisibleColumns(aColumns);

	UpdateListColumnWidths();
}

IUIExtensionWindow* CTabbedToDoCtrl::GetExtensionWnd(FTC_VIEW nView, BOOL bAutoCreate)
{
	ASSERT(nView >= FTCV_FIRSTUIEXTENSION && nView <= FTCV_LASTUIEXTENSION);

	if (nView < FTCV_FIRSTUIEXTENSION || nView > FTCV_LASTUIEXTENSION)
		return NULL;

	VIEWDATA* pData = GetViewData(nView);
	ASSERT(pData && pData->pExtension);
	
	int nExtension = (nView - FTCV_FIRSTUIEXTENSION);
	IUIExtensionWindow* pExtWnd = NULL;

	// Create the extension window if required
	if (m_tabViews.GetViewHwnd(nView) == NULL)
	{
		if (!bAutoCreate)
			return NULL;

		CWaitCursor cursor;

		UINT nCtrlID = (IDC_FTC_EXTENSIONWINDOW1 + nExtension);
		pExtWnd = pData->pExtension->CreateExtWindow(nCtrlID, WS_CHILD, 0, 0, 0, 0, GetSafeHwnd());
		
		if (pExtWnd == NULL)
			return NULL;
		
		HWND hWnd = pExtWnd->GetHwnd();
		ASSERT (hWnd);
		
		if (!hWnd)
			return NULL;
		
		pExtWnd->SetUITheme(&m_theme);
		
		// update focus first because initializing views can take time
		::SetFocus(hWnd);
		
		m_aExtViews[nExtension] = pExtWnd;
		
		// restore state
		CPreferences prefs;
		CString sKey = GetPreferencesKey(_T("UIExtensions"));
		
		pExtWnd->LoadPreferences(&prefs, sKey);
		
		// and update tab control with our new HWND
		m_tabViews.SetViewHwnd((FTC_VIEW)nView, hWnd);
		
		// initialize update state
		pData->bNeedTaskUpdate = TRUE;
		
		Invalidate();
	}
	else // already exists
	{
		pExtWnd = m_aExtViews[nExtension];
	}

	return pExtWnd;
}

LRESULT CTabbedToDoCtrl::OnPreTabViewChange(WPARAM nOldView, LPARAM nNewView) 
{
	EndLabelEdit(FALSE);

	// notify parent
	GetParent()->SendMessage(WM_TDCN_VIEWPRECHANGE, nOldView, nNewView);

	// show the incoming selection and hide the outgoing in that order
	BOOL bFiltered = FALSE;

	// take a note of what task is currently singly selected
	// so that we can prevent unnecessary calls to UpdateControls
	DWORD dwSelTaskID = GetSingleSelectedTaskID();
	
	switch (nNewView)
	{
	case FTCV_TASKTREE:
		// make sure something is selected
		if (GetSelectedCount() == 0)
		{
			HTREEITEM hti = m_tree.GetSelectedItem();

			if (!hti)
				hti = m_tree.GetChildItem(NULL);

			CToDoCtrl::SelectTask(GetTaskID(hti));
		}

		// update sort
		if (m_bTreeNeedResort)
		{
			m_bTreeNeedResort = FALSE;
			Resort();
		}

		m_tree.EnsureVisible(Selection().GetFirstItem());
		break;

	case FTCV_TASKLIST:
		// processed any unhandled comments
		HandleUnsavedComments(); 		
		
		// set the prompt now we know how tall the header is
		m_mgrPrompts.SetPrompt(m_list, IDS_TDC_FILTEREDTASKLISTPROMPT, LVM_GETITEMCOUNT, 0, m_list.GetHeaderHeight());

		// make sure row height is correct by forcing a WM_MEASUREITEM
		RemeasureList();

		// update column widths
		UpdateListColumnWidths(FALSE);

		// restore selection
		ResyncListSelection();

		m_list.EnsureVisible(GetFirstSelectedItem(), FALSE);
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		{
			VIEWDATA* pData = GetViewData((FTC_VIEW)nNewView);
			ASSERT(pData && pData->pExtension);

			// start progress if initializing from another view, 
			// will be cleaned up in OnPostTabViewChange
			UINT nProgressMsg = 0;

			if (nOldView != -1)
			{
				if (GetExtensionWnd((FTC_VIEW)nNewView, FALSE) == NULL)
					nProgressMsg = IDS_INITIALISINGTABBEDVIEW;

				else if (pData->bNeedTaskUpdate)
					nProgressMsg = IDS_UPDATINGTABBEDVIEW;

				if (nProgressMsg)
					BeginExtensionProgress(pData, nProgressMsg);
			}

			IUIExtensionWindow* pExtWnd = GetExtensionWnd((FTC_VIEW)nNewView, TRUE);
			ASSERT(pExtWnd && pExtWnd->GetHwnd());
			
			if (pData->bNeedTaskUpdate)
			{
				// start progress if not already
				// will be cleaned up in OnPostTabViewChange
				if (nProgressMsg == 0)
					BeginExtensionProgress(pData);

				CTaskFile tasks;
				GetAllTasks(tasks);
				
				UpdateExtensionView(pExtWnd, tasks, IUI_ALL);
				pData->bNeedTaskUpdate = FALSE;
			}
				
			// set the selection
			pExtWnd->SelectTask(dwSelTaskID);
		}
		break;
	}

	// update controls only if the selection has changed and 
	// we didn't refilter (RefreshFilter will already have called UpdateControls)
	if (HasSingleSelectionChanged(dwSelTaskID) && !bFiltered)
		UpdateControls();

	return 0L; // allow tab change
}

void CTabbedToDoCtrl::UpdateExtensionView(IUIExtensionWindow* pExtWnd, const CTaskFile& tasks, 
										  IUI_UPDATETYPE nType, TDC_ATTRIBUTE nAttrib)
{
	m_bUpdatingExtensions = TRUE;

	pExtWnd->UpdateTasks(&tasks, nType, nAttrib);

	m_bUpdatingExtensions = FALSE;
}

DWORD CTabbedToDoCtrl::GetSingleSelectedTaskID() const
{
	if (GetSelectedCount() == 1) 
		return GetTaskID(GetSelectedItem());

	// else
	return 0;
}

BOOL CTabbedToDoCtrl::HasSingleSelectionChanged(DWORD dwSelID) const
{
	// multi-selection
	if (GetSelectedCount() != 1)
		return TRUE;

	// different selection
	if (GetTaskID(GetSelectedItem()) != dwSelID)
		return TRUE;

	// dwSelID is still the only selection
	return FALSE;
}

LRESULT CTabbedToDoCtrl::OnPostTabViewChange(WPARAM nOldView, LPARAM nNewView)
{
	switch (nNewView)
	{
	case FTCV_TASKTREE:
		break;

	case FTCV_TASKLIST:
		{
			// update sort
			VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);

			if (pLVData->bNeedResort)
			{
				pLVData->bNeedResort = FALSE;
				Resort();
			}

			// update column widths
			UpdateListColumnWidths(FALSE);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		// stop any progress
		GetParent()->SendMessage(WM_TDCM_LENGTHYOPERATION, FALSE);
		break;
	}

	// notify parent
	GetParent()->SendMessage(WM_TDCN_VIEWPOSTCHANGE, nOldView, nNewView);

	return 0L;
}

VIEWDATA* CTabbedToDoCtrl::GetViewData(FTC_VIEW nView) const
{
	return (VIEWDATA*)m_tabViews.GetViewData(nView);
}

VIEWDATA* CTabbedToDoCtrl::GetActiveViewData() const
{
	return (VIEWDATA*)m_tabViews.GetViewData(GetView());
}

void CTabbedToDoCtrl::SetView(FTC_VIEW nView) 
{
	// take a note of what task is currently singly selected
	// so that we can prevent unnecessary calls to UpdateControls
	DWORD dwSelTaskID = GetSingleSelectedTaskID();
	
	if (!m_tabViews.SetActiveView(nView, TRUE))
		return;

	// update controls only if the selection has changed and 
	if (HasSingleSelectionChanged(dwSelTaskID))
		UpdateControls();
}

void CTabbedToDoCtrl::SetNextView() 
{
	// take a note of what task is currently singly selected
	// so that we can prevent unnecessary calls to UpdateControls
	DWORD dwSelTaskID = GetSingleSelectedTaskID();
	
	m_tabViews.ActivateNextView();

	// update controls only if the selection has changed and 
	if (HasSingleSelectionChanged(dwSelTaskID))
		UpdateControls();
}

void CTabbedToDoCtrl::RemeasureList()
{
	CRect rList;
	m_list.GetWindowRect(rList);
	ScreenToClient(rList);

	WINDOWPOS wpos = { m_list, NULL, rList.left, rList.top, rList.Width(), rList.Height(), SWP_NOZORDER };
	m_list.SendMessage(WM_WINDOWPOSCHANGED, 0, (LPARAM)&wpos);

	// set tree header height to match listview
	int nHeight = m_list.GetHeaderHeight();

	if (nHeight != -1)
		m_tree.SetHeaderHeight(nHeight);
}

LRESULT CTabbedToDoCtrl::OnGutterRecalcColWidth(WPARAM wParam, LPARAM lParam)
{
	NCGRECALCCOLUMN* pNCRC = (NCGRECALCCOLUMN*)lParam;
	
	// special case: PATH column
	if (pNCRC->nColID != TDCC_PATH)
		return CToDoCtrl::OnGutterRecalcColWidth(wParam, lParam);

	// else tree does not show the path column
	pNCRC->nWidth = 0;
	return TRUE;
}

LRESULT CTabbedToDoCtrl::OnUIExtSelectTask(WPARAM /*wParam*/, LPARAM lParam)
{
	if (!m_bUpdatingExtensions)
	{
		// check there's an actual change
		DWORD dwTaskID = (DWORD)lParam;

		if (HasSingleSelectionChanged(dwTaskID))
			return SelectTask(dwTaskID);
	}

	// else
	return 0L;
}

LRESULT CTabbedToDoCtrl::OnUIExtModifySelectedTask(WPARAM wParam, LPARAM lParam)
{
	CStringArray aValues;
	CBinaryData bdEmpty;

	try
	{
		switch (wParam)
		{
		case TDCA_TASKNAME:
			return SetSelectedTaskTitle((LPCTSTR)lParam);

		case TDCA_DONEDATE:
			return SetSelectedTaskDate(TDCD_DONEDATE, *((double*)lParam));

		case TDCA_DUEDATE:
			return SetSelectedTaskDate(TDCD_DUEDATE, *((double*)lParam));

		case TDCA_STARTDATE:
			return SetSelectedTaskDate(TDCD_STARTDATE, *((double*)lParam));

		case TDCA_PRIORITY:
			return SetSelectedTaskPriority(*((int*)lParam));

		case TDCA_COLOR:
			return SetSelectedTaskColor(*((COLORREF*)lParam));

		case TDCA_ALLOCTO:
			Misc::Split((LPCTSTR)lParam, aValues);
			return SetSelectedTaskAllocTo(aValues);

		case TDCA_ALLOCBY:
			return SetSelectedTaskAllocBy((LPCTSTR)lParam);

		case TDCA_STATUS:
			return SetSelectedTaskStatus((LPCTSTR)lParam);

		case TDCA_CATEGORY:
			Misc::Split((LPCTSTR)lParam, aValues);
			return SetSelectedTaskCategories(aValues);

		case TDCA_TAGS:
			Misc::Split((LPCTSTR)lParam, aValues);
			return SetSelectedTaskTags(aValues);

		case TDCA_PERCENT:
			return SetSelectedTaskPercentDone(*((int*)lParam));

		case TDCA_TIMEEST:
			return SetSelectedTaskTimeEstimate(*((double*)lParam));

		case TDCA_TIMESPENT:
			return SetSelectedTaskTimeSpent(*((double*)lParam));

		case TDCA_FILEREF:
			return SetSelectedTaskFileRef((LPCTSTR)lParam);

		case TDCA_COMMENTS:
			return SetSelectedTaskComments((LPCTSTR)lParam, bdEmpty);

		case TDCA_FLAG:
			return SetSelectedTaskFlag(*((BOOL*)lParam));
			break;

		case TDCA_RISK: 
			return SetSelectedTaskRisk(*((int*)lParam));

		case TDCA_EXTERNALID: 
			return SetSelectedTaskExtID((LPCTSTR)lParam);

		case TDCA_COST: 
			return SetSelectedTaskCost(*((double*)lParam));

		case TDCA_DEPENDENCY: 
			Misc::Split((LPCTSTR)lParam, aValues);
			return SetSelectedTaskDependencies(aValues);

		case TDCA_VERSION:
			return SetSelectedTaskVersion((LPCTSTR)lParam);

		case TDCA_RECURRENCE: 
		case TDCA_CREATIONDATE:
		case TDCA_CREATEDBY:
		default:
			ASSERT(0);
			// fall thru
		}
	}
	catch (...)
	{
		// fall thru
	}
	
	return FALSE;
}

void CTabbedToDoCtrl::UpdateListColumnWidths(BOOL bCheckListVisibility)
{
	if (bCheckListVisibility && InTreeView())
		return;

	int nNumCol = m_list.GetColumnCount();
	int nTotalWidth = 0;
	BOOL bFirstWidth = TRUE;

	CClientDC dc(&m_list);
	CFont* pOldFont = GraphicsMisc::PrepareDCFont(&dc, &m_list);
	float fAve = GraphicsMisc::GetAverageCharWidth(&dc);

	CHoldRedraw hr(m_list, NCR_PAINT | NCR_UPDATE);
	
	for (int nCol = 0; nCol < nNumCol; nCol++)
	{
		TDC_COLUMN nColID = GetListColumnID(nCol);

		// get column width from tree except for some special cases
		// the reason we can't just take the tree's widths is that the
		// list always shows _all_ items whereas the tree hides some
		int nWidth = 0, nTreeColWidth = m_tree.GetColumnWidth(nColID);
		CString sLongest;

		if (IsColumnShowing(nColID))
		{
			switch (nColID)
			{
			case TDCC_CLIENT:
				continue; // we'll deal with this at the end

			case TDCC_EXTERNALID:
				sLongest = m_find.GetLongestExternalID(FALSE);
				break;

			case TDCC_POSITION:
				sLongest = m_find.GetLongestPosition(FALSE);
				break;

			case TDCC_CATEGORY:
				{
					// determine the longest visible string
					sLongest = m_find.GetLongestCategory(FALSE);

					// include whatever's in the category edit field
					CString sEdit;
					m_cbCategory.GetWindowText(sEdit);

					if (sEdit.GetLength() > sLongest.GetLength())
						sLongest = sEdit;
				}
				break;

			case TDCC_TAGS:
				{
					// determine the longest visible string
					sLongest = m_find.GetLongestTag(FALSE);

					// include whatever's in the category edit field
					CString sEdit;
					m_cbTags.GetWindowText(sEdit);

					if (sEdit.GetLength() > sLongest.GetLength())
						sLongest = sEdit;
				}
				break;

			case TDCC_ALLOCTO:
				{
					// determine the longest visible string
					sLongest = m_find.GetLongestAllocTo(FALSE);

					// include whatever's in the category edit field
					CString sEdit;
					m_cbAllocTo.GetWindowText(sEdit);

					if (sEdit.GetLength() > sLongest.GetLength())
						sLongest = sEdit;
				}
				break;

			case TDCC_RECURRENCE:
				sLongest = m_find.GetLongestRecurrence(FALSE);
				break;

			case TDCC_TIMEEST:
			case TDCC_TIMESPENT:
				if (HasStyle(TDCS_DISPLAYHMSTIMEFORMAT))
					nWidth = nTreeColWidth;
				else
				{
					BOOL bTimeEst = (nColID == TDCC_TIMEEST);
					sLongest = m_find.GetLongestTime(s_tdDefault.nTimeEstUnits, bTimeEst);
				}
				break;

			case TDCC_PATH:
				nTreeColWidth = 150;
				break;

				// all else
			default:
				break;
			}

			if (!sLongest.IsEmpty())
			{
				int nAveExtent = (int)(sLongest.GetLength() * fAve);
				int nTextExtent = dc.GetTextExtent(sLongest).cx;

				// mimic width of tree columns
				nWidth = max(nAveExtent, nTextExtent) + 2 * NCG_COLPADDING;

				// can't be narrower than tree column 
				nWidth = max(nWidth, nTreeColWidth);
			}
			else
				nWidth = nTreeColWidth;

			// maximum width
			if (m_nMaxColWidth != -1 && nWidth > m_nMaxColWidth)
				nWidth = m_nMaxColWidth;

			// DON'T KNOW WHAT THIS IS FOR???
			if (nWidth && bFirstWidth)
			{
				bFirstWidth = FALSE;
				nWidth += 1;
			}

			// sort indicator 
			BOOL bTreeColWidened = !(m_bMultiSort || m_sort.nBy1 == TDCC_NONE) && (m_sort.nBy1 == nColID);
			
			VIEWDATA* pListData = GetViewData(FTCV_TASKLIST);
			BOOL bListColWidened = !(pListData->bMultiSort || pListData->sort.nBy1 == TDCC_NONE) && pListData->sort.nBy1 == nColID;
			
			if (!bTreeColWidened && bListColWidened)
				nWidth += SORTWIDTH;
			
			else if (bTreeColWidened && !bListColWidened)
				nWidth -= SORTWIDTH;
		}

		m_list.SetColumnWidth(nCol, nWidth);

		nTotalWidth += nWidth;
	}

	// client column is what's left
	CRect rList;
	m_list.GetClientRect(rList);

	int nColWidth = max(300, rList.Width() - nTotalWidth);
	m_list.SetColumnWidth(GetListColumnIndex(TDCC_CLIENT), nColWidth);

	// cleanup
	dc.SelectObject(pOldFont);
}

void CTabbedToDoCtrl::RebuildCustomAttributeUI()
{
	CToDoCtrl::RebuildCustomAttributeUI();

	// rebuild custom list columns
	BuildListColumns(TRUE);
}

void CTabbedToDoCtrl::ReposTaskTree(CDeferWndMove* pDWM, const CRect& rPos)
{
	CRect rView;
	m_tabViews.Resize(rPos, pDWM, rView);

	CToDoCtrl::ReposTaskTree(pDWM, rView);
}

void CTabbedToDoCtrl::UpdateTasklistVisibility()
{
	BOOL bTasksVis = (m_nMaxState != TDCMS_MAXCOMMENTS);
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::UpdateTasklistVisibility();
		break;

	case FTCV_TASKLIST:
		m_list.ShowWindow(bTasksVis ? SW_SHOW : SW_HIDE);
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	// handle tab control
	m_tabViews.ShowWindow(bTasksVis && HasStyle(TDCS_SHOWTREELISTBAR) ? SW_SHOW : SW_HIDE);
}

BOOL CTabbedToDoCtrl::OnEraseBkgnd(CDC* pDC)
{
	// clip out tab ctrl
	if (m_tabViews.GetSafeHwnd())
		ExcludeChild(this, pDC, &m_tabViews);

	return CToDoCtrl::OnEraseBkgnd(pDC);
}

void CTabbedToDoCtrl::Resize(int cx, int cy)
{
	CToDoCtrl::Resize(cx, cy);

	if (m_list.GetSafeHwnd())
		UpdateListColumnWidths();
}

BOOL CTabbedToDoCtrl::WantTaskContextMenu() const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
	case FTCV_TASKLIST:
		return TRUE;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		return TRUE;
//		return FALSE;

	default:
		ASSERT(0);
	}

	return FALSE;
}

BOOL CTabbedToDoCtrl::GetSelectionBoundingRect(CRect& rSelection) const
{
	rSelection.SetRectEmpty();
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::GetSelectionBoundingRect(rSelection);
		break;

	case FTCV_TASKLIST:
		{
			rSelection.SetRectEmpty();

			POSITION pos = m_list.GetFirstSelectedItemPosition();

			// initialize to first item
			if (pos)
				VERIFY(GetItemTitleRect(m_list.GetNextSelectedItem(pos), TDCTR_LABEL, rSelection));

			// rest of selection
			while (pos)
			{
				CRect rItem;
				VERIFY(GetItemTitleRect(m_list.GetNextSelectedItem(pos), TDCTR_LABEL, rItem));
					
				rSelection |= rItem;
			}

			m_list.ClientToScreen(rSelection);
			ScreenToClient(rSelection);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	return (!rSelection.IsRectEmpty());
}

void CTabbedToDoCtrl::SelectAll()
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::SelectAll();
		break;

	case FTCV_TASKLIST:
		{
			int nNumItems = m_list.GetItemCount();
			BOOL bAllTasks = (CToDoCtrl::GetTaskCount() == (UINT)nNumItems);
			CDWordArray aTaskIDs;

			for (int nItem = 0; nItem < nNumItems; nItem++)
			{
				// select item
				m_list.SetItemState(nItem, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);

				// save ID only not showing all tasks
				if (!bAllTasks)
					aTaskIDs.Add(m_list.GetItemData(nItem));
			}

			// select items in tree
			if (bAllTasks)
				CToDoCtrl::SelectAll();
			else
				MultiSelectItems(aTaskIDs, TSHS_SELECT, FALSE);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
}

void CTabbedToDoCtrl::DeselectAll()
{
	CToDoCtrl::DeselectAll();
	ClearListSelection();
}

int CTabbedToDoCtrl::GetTasks(CTaskFile& tasks, const TDCGETTASKS& filter) const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::GetTasks(tasks, filter);

	case FTCV_TASKLIST:
		{
			// ISO date strings
			// must be done first before any tasks are added
			tasks.EnableISODates(HasStyle(TDCS_SHOWDATESINISO));

			// we return exactly what's selected in the list and in the same order
			// so we make sure the filter includes TDCGT_NOTSUBTASKS
			for (int nItem = 0; nItem < m_list.GetItemCount(); nItem++)
			{
				HTREEITEM hti = GetTreeItem(nItem);
				AddTreeItemToTaskFile(hti, tasks, NULL, filter, FALSE);
			}

			if (filter.dwFlags & TDCGTF_FILENAME)
				tasks.SetFileName(m_sLastSavePath);

			return tasks.GetTaskCount();
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		return CToDoCtrl::GetTasks(tasks, filter); // for now

	default:
		ASSERT(0);
	}

	return 0;
}

int CTabbedToDoCtrl::GetSelectedTasks(CTaskFile& tasks, const TDCGETTASKS& filter, DWORD dwFlags) const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::GetSelectedTasks(tasks, filter, dwFlags);

	case FTCV_TASKLIST:
		{
			// we return exactly what's selected in the list and in the same order
			// so we make sure the filter includes TDCGT_NOTSUBTASKS
			POSITION pos = m_list.GetFirstSelectedItemPosition();

			while (pos)
			{
				int nItem = m_list.GetNextSelectedItem(pos);
				HTREEITEM hti = GetTreeItem(nItem);

				AddTreeItemToTaskFile(hti, tasks, NULL, filter, FALSE);
			}

			return tasks.GetTaskCount();
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		return CToDoCtrl::GetSelectedTasks(tasks, filter, dwFlags); // for now

	default:
		ASSERT(0);
	}

	return 0;
}

void CTabbedToDoCtrl::OnListHeaderCustomDraw(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMCUSTOMDRAW* pNMCD = (NMCUSTOMDRAW*)pNMHDR;
	
	if (pNMCD->dwDrawStage == CDDS_PREPAINT)
		*pResult |= CDRF_NOTIFYITEMDRAW | CDRF_NOTIFYPOSTPAINT;	
	
	else if (pNMCD->dwDrawStage == CDDS_ITEMPREPAINT)
		*pResult |= CDRF_NOTIFYPOSTPAINT;	

	else if (pNMCD->dwDrawStage == CDDS_ITEMPOSTPAINT)
	{
		CDC* pDC = CDC::FromHandle(pNMCD->hdc);
		CFont* pOldFont = (CFont*)pDC->SelectObject(CWnd::GetFont());

		DrawColumnHeaderText(pDC, pNMCD->dwItemSpec, pNMCD->rc, pNMCD->uItemState);

		pDC->SelectObject(pOldFont);
	}
}

void CTabbedToDoCtrl::OnMeasureItem(int nIDCtl, LPMEASUREITEMSTRUCT lpMeasureItemStruct)
{
	if (nIDCtl == IDC_FTC_TASKLIST)
		lpMeasureItemStruct->itemHeight = m_tree.TCH().GetItemHeight();
	else
		CToDoCtrl::OnMeasureItem(nIDCtl, lpMeasureItemStruct);
}

void CTabbedToDoCtrl::RemoveDeletedListItems()
{
	int nItem = m_list.GetItemCount();

	while (nItem--)
	{
		DWORD dwTaskID = m_list.GetItemData(nItem);

		if (!m_data.GetTask(dwTaskID))
			m_list.DeleteItem(nItem);
	}
}

CTabbedToDoCtrl::TDI_STATE CTabbedToDoCtrl::GetListItemState(int nItem)
{
	if (m_list.GetItemState(nItem, LVIS_DROPHILITED) & LVIS_DROPHILITED)
		return TDIS_DROPHILITED;
	
	else if (m_list.GetItemState(nItem, LVIS_SELECTED) & LVIS_SELECTED)
		return (TasksHaveFocus() ? TDIS_SELECTED : TDIS_SELECTEDNOTFOCUSED);
	
	return TDIS_NONE;
}

void CTabbedToDoCtrl::GetItemColors(int nItem, NCGITEMCOLORS& colors)
{
	TDI_STATE nState = GetListItemState(nItem);
	DWORD dwID = GetTaskID(nItem);

	colors.crText = GetSysColor(COLOR_WINDOWTEXT);
	colors.crBack = GetSysColor(COLOR_WINDOW);

	if (nItem % 2)
	{
		COLORREF crAlt = m_tree.GetAlternateLineColor();

		if (crAlt != NOCOLOR)
			colors.crBack = crAlt;
	}

	CToDoCtrl::GetItemColors(dwID, &colors, nState);
}

void CTabbedToDoCtrl::OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpDrawItemStruct)
{
	if (nIDCtl == IDC_FTC_TASKLIST)
	{
		// don't bother drawing if we're just switching to the tree view
		if (InTreeView())
			return;
			
		int nItem = lpDrawItemStruct->itemID;
		DWORD dwTaskID = lpDrawItemStruct->itemData, dwRefID = dwTaskID;

		const TODOITEM* pTDI = NULL;
		const TODOSTRUCTURE* pTDS = NULL;

		if (!m_data.GetTask(dwTaskID, pTDI, pTDS))
			return;

		// resolve dwRefID
 		if (dwRefID == dwTaskID)
 			dwRefID = 0;

		CRect rItem, rTitleBounds;

		m_list.GetSubItemRect(nItem, 0, LVIR_BOUNDS, rItem);
		GetItemTitleRect(nItem, TDCTR_BOUNDS, rTitleBounds);

		CDC* pDC = CDC::FromHandle(lpDrawItemStruct->hDC);
		int nSaveDC = pDC->SaveDC(); // so that DrawFocusRect works

		pDC->SetBkMode(TRANSPARENT);

		COLORREF crGrid = m_tree.GetGridlineColor();
		NCGITEMCOLORS colors = { 0, 0, 0, FALSE, FALSE };
		
		GetItemColors(nItem, colors);
		TDI_STATE nState = GetListItemState(nItem);

		// fill back color
		if (HasStyle(TDCS_FULLROWSELECTION) || !colors.bBackSet)
		{
			DrawItemBackColor(pDC, rItem, colors.crBack);
		}
		else
		{
			// calculate the rect containing the rest of the columns
			CRect rCols(rItem);

			if (HasStyle(TDCS_RIGHTSIDECOLUMNS))
			{
				rCols.left = rTitleBounds.right;
			}
			else
				rCols.right = rTitleBounds.left;

			// fill the background colour of the rest of the columns
			rCols.right--;

			DrawItemBackColor(pDC, rCols, colors.crBack);

			// check to see whether we should fill the label cell background
			// with the alternate line colour
			if (nItem % 2) // odd row
			{
				COLORREF crAltLine = m_tree.GetAlternateLineColor();
					
				if (crAltLine != NOCOLOR)
				{
					rTitleBounds.OffsetRect(-1, 0);
					pDC->FillSolidRect(rTitleBounds, crAltLine);
					rTitleBounds.OffsetRect(1, 0);
				}
			}
		}

		// and set the text color
		if (colors.bTextSet)
			pDC->SetTextColor(colors.crText);
		else
			pDC->SetTextColor(GetSysColor(COLOR_WINDOWTEXT));
		
		// render column text
		// use CToDoCtrl::OnGutterDrawItem to do the hard work
		NCGDRAWITEM ncgDI;
		CRect rFocus; // will be set up during TDCC_CLIENT handling
		int nNumCol = m_list.GetColumnCount();
		
		for (int nCol = 0; nCol < nNumCol; nCol++)
		{
			// client column/task title is special case
			TDC_COLUMN nColID = GetListColumnID(nCol);

			if (nColID == TDCC_CLIENT)
			{
				BOOL bTopLevel = pTDS->ParentIsRoot();
				BOOL bFolder = pTDS->HasSubTasks() && HasStyle(TDCS_SHOWPARENTSASFOLDERS);
				BOOL bDoneAndStrikeThru = pTDI->IsDone() && HasStyle(TDCS_STRIKETHOUGHDONETASKS);

				// icon
				CRect rSubItem;
				
				if (GetItemTitleRect(nItem, TDCTR_ICON, rSubItem))
				{
					if (!pTDI->sIcon.IsEmpty())
					{
						int nImage = m_ilTaskIcons.GetImageIndex(pTDI->sIcon);
						m_ilTaskIcons.Draw(pDC, nImage, rSubItem.TopLeft(), ILD_TRANSPARENT);
					}
					else if (bFolder)
						m_ilTaskIcons.Draw(pDC, 0, rSubItem.TopLeft(), ILD_TRANSPARENT);
				}

				// checkbox
				if (GetItemTitleRect(nItem, TDCTR_CHECKBOX, rSubItem))
				{
					int nImage = pTDI->IsDone() ? 2 : 1;
					CImageList::FromHandle(m_hilDone)->Draw(pDC, nImage, rSubItem.TopLeft(), ILD_TRANSPARENT);
				}

				// set fonts before calculating title rect
				HFONT hFontOld = NULL;

				if (bDoneAndStrikeThru)
					hFontOld = (HFONT)::SelectObject(lpDrawItemStruct->hDC, m_hFontDone);

				else if (bTopLevel) 
					hFontOld = (HFONT)::SelectObject(lpDrawItemStruct->hDC, m_hFontBold);

				// Task title 
				GetItemTitleRect(nItem, TDCTR_LABEL, rSubItem, pDC, pTDI->sTitle);
				rSubItem.OffsetRect(-1, 0);

				// back colour
				if (!HasStyle(TDCS_FULLROWSELECTION) && colors.bBackSet)
				{
					DrawItemBackColor(pDC, rSubItem, colors.crBack);
				}

				// text
				DrawGutterItemText(pDC, pTDI->sTitle, rSubItem, LVCFMT_LEFT);
				rFocus = rSubItem; // save for focus rect drawing

				// vertical divider
				if (crGrid != NOCOLOR)
					pDC->FillSolidRect(rTitleBounds.right - 1, rTitleBounds.top, 1, rTitleBounds.Height(), crGrid);

				// draw shortcut for reference tasks
				// NOTE: there should no longer be any references
// 				if (dwRefID)
// 					DrawShortcutIcon(pDC, rSubItem.TopLeft());

				// render comment text if not editing this task label
				if (m_dwEditingID != dwTaskID)
				{
					// deselect bold font if set
					if (bTopLevel && !bDoneAndStrikeThru)
					{
						::SelectObject(lpDrawItemStruct->hDC, hFontOld);
						hFontOld = NULL;
					}	

					rTitleBounds.top++;
					rTitleBounds.left = rSubItem.right + 4;
					DrawCommentsText(pDC, pTDI, pTDS, rTitleBounds, nState);
				}

				if (hFontOld)
					::SelectObject(lpDrawItemStruct->hDC, hFontOld);
			}
			else
			{
				// get col rect
				CRect rSubItem;
				m_list.GetSubItemRect(nItem, nCol, LVIR_LABEL, rSubItem);
				rSubItem.OffsetRect(-1, 0);

				ncgDI.pDC = pDC;
				ncgDI.dwItem = NULL;
				ncgDI.dwParentItem = NULL;
				ncgDI.nLevel = 0;
				ncgDI.nItemPos = 0;
				ncgDI.rWindow = &rSubItem;
				ncgDI.rItem = &rSubItem;
				ncgDI.nColID = nColID;
				ncgDI.nTextAlign = m_list.GetColumnDrawAlignment(nCol);
				
				DrawItemColumn(pTDI, pTDS, &ncgDI, nState, dwRefID);
			}
		}

		// base gridline
		if (crGrid != NOCOLOR)
			pDC->FillSolidRect(rItem.left, rItem.bottom - 1, rItem.Width() - 1, 1, crGrid);

		pDC->RestoreDC(nSaveDC); // so that DrawFocusRect works
		
		// focus rect
		if ((lpDrawItemStruct->itemState & ODS_FOCUS) && (nState == TDIS_SELECTED))
		{
			pDC->DrawFocusRect(rFocus);
		}

	}
	else
		CToDoCtrl::OnDrawItem(nIDCtl, lpDrawItemStruct);
}

COLORREF CTabbedToDoCtrl::GetItemLineColor(HTREEITEM hti)
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::GetItemLineColor(hti);

	case FTCV_TASKLIST:
		{
			COLORREF crAltLines = m_tree.GetAlternateLineColor();

			if (crAltLines != NOCOLOR)
			{
				int nItem = FindListTask(GetTaskID(hti));

				if (nItem != -1 && (nItem % 2))
					return crAltLines;
			}
				
			// else
			return GetSysColor(COLOR_WINDOW);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	// else
	return GetSysColor(COLOR_WINDOW);
}

void CTabbedToDoCtrl::DrawColumnHeaderText(CDC* pDC, int nCol, const CRect& rCol, UINT nState)
{
	// filter out zero width columns
	if (rCol.Width() == 0)
		return;

	TDC_COLUMN nColID = GetListColumnID(nCol);
	int nAlignment = m_list.GetColumnDrawAlignment(nCol);
	CEnString sColumn;
	CString sFont;

	if (CTDCCustomAttributeHelper::IsCustomColumn(nColID))
	{
		TDCCUSTOMATTRIBUTEDEFINITION attribDef;

		if (CTDCCustomAttributeHelper::GetAttributeDef(nColID, m_aCustomAttribDefs, attribDef))
			sColumn = attribDef.GetColumnTitle();
	}
	else
	{
		const TDCCOLUMN* pCol = GetListColumn(nCol);
		ASSERT(pCol);

		sColumn.LoadString(pCol->nIDName);

		// special cases
		if (sColumn.IsEmpty())
			sColumn = CString(static_cast<TCHAR>(pCol->nIDName));

		sFont = pCol->szFont;
	}

	if (nColID == TDCC_CLIENT && HasStyle(TDCS_SHOWPATHINHEADER) && m_list.GetSelectedCount() == 1)
	{
		int nColWidthInChars = (int)(rCol.Width() / m_fAveHeaderCharWidth);
		CString sPath = m_data.GetTaskPath(GetSelectedTaskID(), nColWidthInChars);
			
		if (!sPath.IsEmpty())
			sColumn.Format(_T("%s [%s]"), CEnString(IDS_TDC_COLUMN_TASK), sPath);
	}
	else if (sColumn == _T("%%"))
		sColumn = _T("%");

	const UINT DEFFLAGS = DT_END_ELLIPSIS | DT_VCENTER | DT_SINGLELINE | DT_NOPREFIX;
	pDC->SetBkMode(TRANSPARENT);
	
	CRect rText(rCol);
	rText.bottom -= 2;
	
	switch (nAlignment)
	{
	case DT_LEFT:
		rText.left += NCG_COLPADDING - 1;
		break;
		
	case DT_RIGHT:
		rText.right -= NCG_COLPADDING;
		break;

	case DT_CENTER:
		rText.right--;
	}
	
	BOOL bDown = (nState & CDIS_SELECTED);

	VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);
	BOOL bSorted = (IsSortable() && (pLVData->sort.nBy1 == nColID) && !pLVData->bMultiSort);
	
	if (bDown)
		rText.OffsetRect(1, 1);
	
	if (bSorted)
		rText.right -= (SORTWIDTH + 2);

	if (!sFont.IsEmpty())
	{
		CFont* pOldFont = NULL;

		if (sFont.CompareNoCase(_T("WingDings")) == 0)
			pOldFont = pDC->SelectObject(&GraphicsMisc::WingDings());
		
		else if (sFont.CompareNoCase(_T("Marlett")) == 0)
			pOldFont = pDC->SelectObject(&GraphicsMisc::Marlett());
		
		pDC->DrawText(sColumn, rText, DEFFLAGS | nAlignment);
		
		if (pOldFont)
			pDC->SelectObject(pOldFont);
	}
	else
		pDC->DrawText(sColumn, rText, DEFFLAGS | nAlignment);

	// draw sort direction if required
	if (bSorted)
	{
		// adjust for sort arrow
		if (nAlignment == DT_CENTER)
			rText.left = ((rText.left + rText.right + pDC->GetTextExtent(sColumn).cx) / 2) + 2;
			
		else if (nAlignment == DT_RIGHT)
			rText.left = rText.right + 2;
		else
			rText.left = rText.left + pDC->GetTextExtent(sColumn).cx + 2;

		BOOL bAscending = pLVData->sort.bAscending1 ? 1 : -1;
		int nMidY = (bDown ? 1 : 0) + (rText.top + rText.bottom) / 2;
		POINT ptArrow[3] = { 
							{ 0, 0 }, 
							{ 3, -bAscending * 3 }, 
							{ 7, bAscending } };
		
		// translate the arrow to the appropriate location
		int nPoint = 3;
		
		while (nPoint--)
		{
			ptArrow[nPoint].x += rText.left + 3 + (bDown ? 1 : 0);
			ptArrow[nPoint].y += nMidY;
		}
		pDC->Polyline(ptArrow, 3);
	}
}


BOOL CTabbedToDoCtrl::SetStyle(TDC_STYLE nStyle, BOOL bOn, BOOL bWantUpdate)
{
	// base class processing
	if (CToDoCtrl::SetStyle(nStyle, bOn, bWantUpdate))
	{
		// post-precessing
		switch (nStyle)
		{
		case TDCS_RIGHTSIDECOLUMNS:
			BuildListColumns();
			break;

		case TDCS_SHOWINFOTIPS:
			ListView_SetExtendedListViewStyleEx(m_list, LVS_EX_INFOTIP, bOn ? LVS_EX_INFOTIP : 0);
			break;

		case TDCS_TREETASKICONS:
			if (bOn && !IsColumnShowing(TDCC_ICON)) // this overrides
				ListView_SetImageList(m_list, m_ilTaskIcons, LVSIL_NORMAL);
			else
				ListView_SetImageList(m_list, NULL, LVSIL_NORMAL);
			break;

		case TDCS_SHOWTREELISTBAR:
			m_tabViews.ShowTabControl(bOn);

			if (bWantUpdate)
				Resize();
			break;

		// detect preferences that can affect task text colors
		// and then handle this in NotifyEndPreferencesUpdate
		case TDCS_COLORTEXTBYPRIORITY:
		case TDCS_COLORTEXTBYATTRIBUTE:
		case TDCS_COLORTEXTBYNONE:
		case TDCS_TREATSUBCOMPLETEDASDONE:
		case TDCS_USEEARLIESTDUEDATE:
		case TDCS_USELATESTDUEDATE:
		case TDCS_USEEARLIESTSTARTDATE:
		case TDCS_USELATESTSTARTDATE:
		case TDCS_USEHIGHESTPRIORITY:
		case TDCS_INCLUDEDONEINPRIORITYCALC:
		case TDCS_DUEHAVEHIGHESTPRIORITY:
		case TDCS_DONEHAVELOWESTPRIORITY:
			m_bTaskColorChange = TRUE;
			break;
		}

		return TRUE;
	}

	return FALSE;
}

BOOL CTabbedToDoCtrl::SetPriorityColors(const CDWordArray& aColors)
{
	CDWordArray aPriorityExist;
	aPriorityExist.Copy(m_aPriorityColors);

	if (CToDoCtrl::SetPriorityColors(aColors))
	{
		if (!m_bTaskColorChange)
			m_bTaskColorChange = !Misc::MatchAll(aColors, m_aPriorityColors);

		return TRUE;
	}

	// else
	return FALSE;
}

void CTabbedToDoCtrl::SetCompletedTaskColor(COLORREF color)
{
	if (!m_bTaskColorChange)
		m_bTaskColorChange = (color != m_crDone);

	CToDoCtrl::SetCompletedTaskColor(color);
}

void CTabbedToDoCtrl::SetFlaggedTaskColor(COLORREF color)
{
	if (!m_bTaskColorChange)
		m_bTaskColorChange = (color != m_crFlagged);

	CToDoCtrl::SetFlaggedTaskColor(color);
}

void CTabbedToDoCtrl::SetReferenceTaskColor(COLORREF color)
{
	if (!m_bTaskColorChange)
		m_bTaskColorChange = (color != m_crReference);

	CToDoCtrl::SetReferenceTaskColor(color);
}

void CTabbedToDoCtrl::SetAttributeColors(TDC_ATTRIBUTE nAttrib, const CTDCColorMap& colors)
{
	if (!m_bTaskColorChange)
	{
		m_bTaskColorChange = (colors.GetCount() != m_mapAttribColors.GetCount());

		if (!m_bTaskColorChange)
		{
			CString sItem;
			COLORREF crItem, crExist;
			POSITION pos = colors.GetStartPosition();

			while (pos && !m_bTaskColorChange)
			{
				colors.GetNextAssoc(pos, sItem, crItem);
				m_bTaskColorChange = (!m_mapAttribColors.Lookup(sItem, crExist) || (crItem != crExist));
			}
		}
	}

	CToDoCtrl::SetAttributeColors(nAttrib, colors);
}

void CTabbedToDoCtrl::SetStartedTaskColors(COLORREF crStarted, COLORREF crStartedToday)
{
	if (!m_bTaskColorChange)
		m_bTaskColorChange = (crStarted != m_crStarted || crStartedToday != m_crStartedToday);

	CToDoCtrl::SetStartedTaskColors(crStarted, crStartedToday);
}

void CTabbedToDoCtrl::SetDueTaskColors(COLORREF crDue, COLORREF crDueToday)
{
	if (!m_bTaskColorChange)
		m_bTaskColorChange = (crDue != m_crDue || crDueToday != m_crDueToday);

	CToDoCtrl::SetDueTaskColors(crDue, crDueToday);
}

void CTabbedToDoCtrl::NotifyBeginPreferencesUpdate()
{
	// base class
	CToDoCtrl::NotifyBeginPreferencesUpdate();

	// nothing else for us to do
}

void CTabbedToDoCtrl::NotifyEndPreferencesUpdate()
{
	// base class
	CToDoCtrl::NotifyEndPreferencesUpdate();

	// notify extension windows
	if (HasAnyExtensionViews())
	{
		// we need to update in 2 ways:
		// 1. Tell the extensions that application settings have changed
		// 2. Refresh tasks if their text colour may have changed
		CPreferences prefs;
		CString sKey = GetPreferencesKey(_T("UIExtensions"));
		
		int nExt = m_aExtViews.GetSize();
		FTC_VIEW nCurView = GetView();

		while (nExt--)
		{
			FTC_VIEW nExtView = (FTC_VIEW)(FTCV_FIRSTUIEXTENSION + nExt);
			IUIExtensionWindow* pExtWnd = m_aExtViews[nExt];
			
			if (pExtWnd)
			{
				VIEWDATA* pData = GetViewData(nExtView);
				ASSERT(pData);

				// if this extension is active and wants a 
				// color update we want to start progress
				BOOL bWantColorUpdate = (m_bTaskColorChange && pExtWnd->WantUpdate(TDCA_COLOR));

				if (bWantColorUpdate && nExtView == nCurView)
					BeginExtensionProgress(pData);

				// notify all extensions of prefs change
				pExtWnd->LoadPreferences(&prefs, sKey, TRUE);

				// Update task colours if necessary
				if (bWantColorUpdate)
				{
					if (nExtView == nCurView)
					{
						TDCGETTASKS filter;
						filter.aAttribs.Add(TDCA_COLOR);
						
						CTaskFile tasks;
						GetTasks(tasks, filter);
						
						UpdateExtensionView(pExtWnd, tasks, IUI_EDIT, TDCA_COLOR);
						pData->bNeedTaskUpdate = FALSE;
					}
					else // mark for update
					{
						pData->bNeedTaskUpdate = TRUE;
					}
				}

				// cleanup progress
				if (bWantColorUpdate && nExtView == nCurView)
					EndExtensionProgress();
			}
		}
	}

	m_bTaskColorChange = FALSE;
}

void CTabbedToDoCtrl::BeginExtensionProgress(const VIEWDATA* pData, UINT nMsg)
{
	ASSERT(pData);

	if (nMsg == 0)
		nMsg = IDS_UPDATINGTABBEDVIEW;

	CEnString sMsg(nMsg, pData->pExtension->GetMenuText());
	GetParent()->SendMessage(WM_TDCM_LENGTHYOPERATION, TRUE, (LPARAM)(LPCTSTR)sMsg);
}

void CTabbedToDoCtrl::EndExtensionProgress()
{
	GetParent()->SendMessage(WM_TDCM_LENGTHYOPERATION, FALSE);
}

CString CTabbedToDoCtrl::GetControlDescription(const CWnd* pCtrl) const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		break; // handled below

	case FTCV_TASKLIST:
		if (pCtrl == &m_list)
			return CEnString(IDS_LISTVIEW);
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		if (pCtrl)
		{
			HWND hwnd = m_tabViews.GetViewHwnd(nView);

			if (CDialogHelper::IsChildOrSame(hwnd, pCtrl->GetSafeHwnd()))
			{
				return m_tabViews.GetViewName(nView);
			}
		}
		break;

	default:
		ASSERT(0);
	}

	return CToDoCtrl::GetControlDescription(pCtrl);
}

BOOL CTabbedToDoCtrl::AddTasksToTree(const CTaskFile& tasks, HTREEITEM htiDest, HTREEITEM htiDestAfter, TDC_RESETIDS nResetID)
{
	if (!CToDoCtrl::AddTasksToTree(tasks, htiDest, htiDestAfter, nResetID))
		return FALSE;

	FTC_VIEW nView = GetView();
	
	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		GetViewData(FTCV_TASKLIST)->bNeedTaskUpdate = TRUE;
		break; // handled above
		
	case FTCV_TASKLIST:
		RebuildList(NULL);
		UpdateExtensionViews(TDCA_NEWTASK);
		break;
		
	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		GetViewData(FTCV_TASKLIST)->bNeedTaskUpdate = TRUE;
		UpdateExtensionViews(TDCA_NEWTASK);
		break;
		
	default:
		ASSERT(0);
	}


	return TRUE;
}

BOOL CTabbedToDoCtrl::DeleteSelectedTask(BOOL bWarnUser, BOOL bResetSel)
{
	BOOL bDel = CToDoCtrl::DeleteSelectedTask(bWarnUser, bResetSel);
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		// handled above
		break;

	case FTCV_TASKLIST:
		if (bDel)
		{
			// work out what to select
			DWORD dwSelID = GetSelectedTaskID();
			int nSel = FindListTask(dwSelID);

			if (nSel == -1 && m_list.GetItemCount())
				nSel = 0;

			if (nSel != -1)
				SelectTask(m_list.GetItemData(nSel));
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	return bDel;
}

BOOL CTabbedToDoCtrl::SelectedTasksHaveChildren() const
{
	return CToDoCtrl::SelectedTasksHaveChildren();
}

HTREEITEM CTabbedToDoCtrl::CreateNewTask(const CString& sText, TDC_INSERTWHERE nWhere, BOOL bEditText)
{
	HTREEITEM htiNew = NULL;
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		htiNew = CToDoCtrl::CreateNewTask(sText, nWhere, bEditText);
		break;

	case FTCV_TASKLIST:
		htiNew = CToDoCtrl::CreateNewTask(sText, nWhere, FALSE); // note FALSE

		if (htiNew)
		{
			DWORD dwTaskID = GetTaskID(htiNew);
			ASSERT(dwTaskID == m_dwNextUniqueID - 1);
			
			// make the new task appear
			RebuildList(NULL); 
			
			// select that task
			SelectTask(dwTaskID);
			
			if (bEditText)
			{
				m_dwLastAddedID = dwTaskID;
				EditSelectedTask(TRUE);
			}
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	return htiNew;
}

BOOL CTabbedToDoCtrl::CanCreateNewTask(TDC_INSERTWHERE nInsertWhere) const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
	case FTCV_TASKLIST:
		return CToDoCtrl::CanCreateNewTask(nInsertWhere);

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		return FALSE; // for now

	default:
		ASSERT(0);
	}

	return FALSE;
}

void CTabbedToDoCtrl::RebuildList(const void* pContext)
{
	if (!m_data.GetTaskCount())
	{
		m_list.DeleteAllItems(); 
		return;
	}

	// cache current selection
	TDCSELECTIONCACHE cache;
	CacheListSelection(cache);

	// note: the call to CListCtrl::Scroll at the bottom fails if the 
	// list has redraw disabled so it must happen outside the scope of hr2
	CSize sizePos(0, 0);
	{
		CHoldRedraw hr(GetSafeHwnd());
		CHoldRedraw hr2(m_list);
		CWaitCursor cursor;

		// cache scrolled pos
		if (m_list.GetItemCount())
		{
			CRect rItem;
			m_list.GetItemRect(0, rItem, LVIR_BOUNDS);

			sizePos.cy = -rItem.top + rItem.Height();
		}

		// remove all existing items
		m_list.DeleteAllItems();
		
		// rebuild the list from the tree
		AddTreeItemToList(NULL, pContext);

		// redo last sort
		FTC_VIEW nView = GetView();

		switch (nView)
		{
		case FTCV_TASKTREE:
		case FTCV_UNSET:
			break;

		case FTCV_TASKLIST:
			if (IsSortable())
			{
				VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);
				pLVData->bNeedResort = FALSE;

				Resort();
			}
			break;

		case FTCV_UIEXTENSION1:
		case FTCV_UIEXTENSION2:
		case FTCV_UIEXTENSION3:
		case FTCV_UIEXTENSION4:
		case FTCV_UIEXTENSION5:
		case FTCV_UIEXTENSION6:
		case FTCV_UIEXTENSION7:
		case FTCV_UIEXTENSION8:
		case FTCV_UIEXTENSION9:
		case FTCV_UIEXTENSION10:
		case FTCV_UIEXTENSION11:
		case FTCV_UIEXTENSION12:
		case FTCV_UIEXTENSION13:
		case FTCV_UIEXTENSION14:
		case FTCV_UIEXTENSION15:
		case FTCV_UIEXTENSION16:
			break;

		default:
			ASSERT(0);
		}

		// restore selection
		RestoreListSelection(cache);

		// don't update controls is only one item is selected and it did not
		// change as a result of the filter
		BOOL bSelChange = !(GetSelectedCount() == 1 && 
							cache.aSelTaskIDs.GetSize() == 1 &&
							GetTaskID(GetSelectedItem()) == cache.aSelTaskIDs[0]);

		if (bSelChange)
			UpdateControls();
	}
	
	// restore pos
	if (sizePos.cy != 0)
		m_list.Scroll(sizePos);

	UpdateListColumnWidths();
}

int CTabbedToDoCtrl::AddItemToList(DWORD dwTaskID)
{
	// omit task references from list
	if (CToDoCtrl::IsTaskReference(dwTaskID))
		return -1;

	// else
	return m_list.InsertItem(LVIF_TEXT | LVIF_IMAGE | LVIF_PARAM, 
							m_list.GetItemCount(), 
							LPSTR_TEXTCALLBACK, 
							0, // nState
							0, // nStateMask
							I_IMAGECALLBACK, 
							dwTaskID);
}

void CTabbedToDoCtrl::AddTreeItemToList(HTREEITEM hti, const void* pContext)
{
	// add task
	if (hti)
	{
		// if the add fails then it's a task reference
		if (CTabbedToDoCtrl::AddItemToList(GetTaskID(hti)) == -1)
		{
			return; 
		}
	}

	// children
	HTREEITEM htiChild = m_tree.GetChildItem(hti);

	while (htiChild)
	{
		AddTreeItemToList(htiChild, pContext);
		htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
	}
}

void CTabbedToDoCtrl::SetModified(BOOL bMod, TDC_ATTRIBUTE nAttrib, DWORD dwModTaskID)
{
	CToDoCtrl::SetModified(bMod, nAttrib, dwModTaskID);

	if (bMod)
	{
		if (nAttrib == TDCA_DELETE || nAttrib == TDCA_ARCHIVE)
			RemoveDeletedListItems();
		else
		{
			FTC_VIEW nView = GetView();

			switch (nView)
			{
			case FTCV_TASKLIST:
				m_list.Invalidate(FALSE);
				break;
				
			case FTCV_TASKTREE:
			case FTCV_UNSET:
				// handled below
				break;
				
			case FTCV_UIEXTENSION1:
			case FTCV_UIEXTENSION2:
			case FTCV_UIEXTENSION3:
			case FTCV_UIEXTENSION4:
			case FTCV_UIEXTENSION5:
			case FTCV_UIEXTENSION6:
			case FTCV_UIEXTENSION7:
			case FTCV_UIEXTENSION8:
			case FTCV_UIEXTENSION9:
			case FTCV_UIEXTENSION10:
			case FTCV_UIEXTENSION11:
			case FTCV_UIEXTENSION12:
			case FTCV_UIEXTENSION13:
			case FTCV_UIEXTENSION14:
			case FTCV_UIEXTENSION15:
			case FTCV_UIEXTENSION16:
				// handled below
				break;
				
			default:
				ASSERT(0);
			}
		}

		UpdateExtensionViews(nAttrib);
	}
}

BOOL CTabbedToDoCtrl::ModCausesColorChange(TDC_ATTRIBUTE nModType) const
{
	switch (nModType)
	{
	case TDCA_COLOR:
		return !HasStyle(TDCS_COLORTEXTBYPRIORITY) &&
				!HasStyle(TDCS_COLORTEXTBYATTRIBUTE) &&
				!HasStyle(TDCS_COLORTEXTBYNONE);

	case TDCA_CATEGORY:
	case TDCA_ALLOCBY:
	case TDCA_ALLOCTO:
	case TDCA_STATUS:
	case TDCA_VERSION:
	case TDCA_EXTERNALID:
	case TDCA_TAGS:
		return (HasStyle(TDCS_COLORTEXTBYATTRIBUTE) && m_nColorByAttrib == nModType);

	case TDCA_DONEDATE:
		return (m_crDone != NO_COLOR);

	case TDCA_DUEDATE:
		return (m_crDue != NO_COLOR || m_crDueToday != NO_COLOR);

	case TDCA_PRIORITY:
		return HasStyle(TDCS_COLORTEXTBYPRIORITY);
	}

	// all else
	return FALSE;
}

void CTabbedToDoCtrl::UpdateExtensionViews(TDC_ATTRIBUTE nAttrib)
{
	if (!HasAnyExtensionViews())
		return;

	FTC_VIEW nCurView = GetView();

	switch (nAttrib)
	{
	// for a simple attribute change (or addition) update all extensions
	// at the same time so that they won't need updating when the user switches view
	case TDCA_TASKNAME:
		// Initial edit of new task is special case
		if (m_dwLastAddedID == GetSelectedTaskID())
		{
			UpdateExtensionViews(TDCA_NEWTASK); // RECURSIVE CALL
			return;
		}
		// else fall thru

	case TDCA_DONEDATE:
	case TDCA_DUEDATE:
	case TDCA_STARTDATE:
	case TDCA_PRIORITY:
	case TDCA_COLOR:
	case TDCA_ALLOCTO:
	case TDCA_ALLOCBY:
	case TDCA_STATUS:
	case TDCA_CATEGORY:
	case TDCA_TAGS:
	case TDCA_PERCENT:
	case TDCA_TIMEEST:
	case TDCA_TIMESPENT:
	case TDCA_FILEREF:
	case TDCA_COMMENTS:
	case TDCA_FLAG:
	case TDCA_CREATIONDATE:
	case TDCA_CREATEDBY:
	case TDCA_RISK: 
	case TDCA_EXTERNALID: 
	case TDCA_COST: 
	case TDCA_DEPENDENCY: 
	case TDCA_RECURRENCE: 
	case TDCA_VERSION:
		{	
			// Check extensions for this OR a color change
			BOOL bColorChange = ModCausesColorChange(nAttrib);

			if (!AnyExtensionViewWantsChange(nAttrib))
			{
				// if noone wants either we can stop
				if (!bColorChange || !AnyExtensionViewWantsChange(TDCA_COLOR))
					return;
			}

			// note: we need to get 'True' tasks and all their parents
			// because of the possibility of colour changes
			CTaskFile tasks;
			DWORD dwFlags = TDCGSTF_RESOLVEREFERENCES | TDCGSTF_NOTSUBTASKS;

			if (bColorChange)
				dwFlags |= TDCGSTF_ALLPARENTS;

			CToDoCtrl::GetSelectedTasks(tasks, TDCGETTASKS(), dwFlags);
			
			// refresh all extensions 
			int nExt = m_aExtViews.GetSize();
			
			while (nExt--)
			{
				FTC_VIEW nExtView = (FTC_VIEW)(FTCV_FIRSTUIEXTENSION + nExt);
				
				VIEWDATA* pData = GetViewData(nExtView);
				ASSERT(pData);
				
				// if the window is not active and is already marked
				// for a full update then we don't need to do
				// anything more because it will get this update when
				// it is next activated
				if (nExtView != nCurView && pData->bNeedTaskUpdate)
					continue;
				
				IUIExtensionWindow* pExtWnd = m_aExtViews[nExt];
				
				if (pExtWnd)
				{
					// check for both this attribute or a color change
					if (pExtWnd->WantUpdate(nAttrib))
					{
						UpdateExtensionView(pExtWnd, tasks, IUI_EDIT, nAttrib);
					}
					else if (bColorChange && pExtWnd->WantUpdate(TDCA_COLOR))
					{
						UpdateExtensionView(pExtWnd, tasks, IUI_EDIT, TDCA_COLOR);
					}

					pData->bNeedTaskUpdate = FALSE;
				}
				else
				{
					// not created so need full update
					pData->bNeedTaskUpdate = TRUE;
				}
			}
		}
		break;
		
	// else refresh the current view (if it's an extension) 
	// and mark the others as needing updates.
	case TDCA_DELETE:
	case TDCA_UNDO:
	case TDCA_NEWTASK: 
	case TDCA_MOVE:
	case TDCA_ARCHIVE:
		{
			int nExt = m_aExtViews.GetSize();
			
			while (nExt--)
			{
				FTC_VIEW nExtView = (FTC_VIEW)(FTCV_FIRSTUIEXTENSION + nExt);
				
				VIEWDATA* pData = GetViewData(nExtView);
				ASSERT(pData);
				
				IUIExtensionWindow* pExtWnd = m_aExtViews[nExt];

				if (pExtWnd && (nExtView == nCurView))
				{
					BeginExtensionProgress(pData);

					// update all tasks
					CTaskFile tasks;
					GetTasks(tasks);
					
					UpdateExtensionView(pExtWnd, tasks, IUI_ALL);
					pData->bNeedTaskUpdate = FALSE;

					EndExtensionProgress();
				}
				else
					pData->bNeedTaskUpdate = TRUE;
			}
		}
		break;	
		
	case TDCA_PROJNAME:
	case TDCA_ENCRYPT:
	default:
		// do nothing
		break;
	}
}

BOOL CTabbedToDoCtrl::AnyExtensionViewWantsChange(TDC_ATTRIBUTE nAttrib) const
{
	// find the first extension wanting this change
	FTC_VIEW nCurView = GetView();
	int nExt = m_aExtViews.GetSize();
	
	while (nExt--)
	{
		FTC_VIEW nExtView = (FTC_VIEW)(FTCV_FIRSTUIEXTENSION + nExt);
		
		VIEWDATA* pData = GetViewData(nExtView);
		ASSERT(pData);
		
		// if the window is not active and is already marked
		// for a full update then we don't need to do
		// anything more because it will get this update when
		// it is next activated
		if (nExtView != nCurView && pData->bNeedTaskUpdate)
			continue;
		
		IUIExtensionWindow* pExtWnd = m_aExtViews[nExt];
		
		if (pExtWnd && pExtWnd->WantUpdate(nAttrib))
			return TRUE;
	}

	// not found
	return FALSE;
}

BOOL CTabbedToDoCtrl::ModNeedsResort(TDC_ATTRIBUTE nModType) const
{
	if (!HasStyle(TDCS_RESORTONMODIFY))
		return FALSE;

	// need to check all three sort attributes if multi-sorting
	VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);

	BOOL bListNeedsResort = CToDoCtrl::ModNeedsResort(nModType, pLVData->sort.nBy1);

	if (!bListNeedsResort && pLVData->bMultiSort)
	{
		bListNeedsResort |= CToDoCtrl::ModNeedsResort(nModType, pLVData->sort.nBy2);
		
		if (!bListNeedsResort)
			bListNeedsResort |= CToDoCtrl::ModNeedsResort(nModType, pLVData->sort.nBy3);
	}

	BOOL bTreeNeedsResort = CToDoCtrl::ModNeedsResort(nModType);
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		pLVData->bNeedResort |= bListNeedsResort;
		return bTreeNeedsResort;

	case FTCV_TASKLIST:
		m_bTreeNeedResort |= bTreeNeedsResort;
		return bListNeedsResort;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	return FALSE;
}

void CTabbedToDoCtrl::ResortSelectedTaskParents() 
{ 
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::ResortSelectedTaskParents();
		break;

	case FTCV_TASKLIST:
		Resort(); // do a full sort
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
}

TDC_COLUMN CTabbedToDoCtrl::GetSortBy() const 
{ 
	VIEWDATA* pVData = GetActiveViewData();

	return pVData ? pVData->sort.nBy1 : m_sort.nBy1; 
}

void CTabbedToDoCtrl::GetSortBy(TDSORTCOLUMNS& sort) const
{
	VIEWDATA* pVData = GetActiveViewData();

	sort = pVData ? pVData->sort : m_sort;
}

BOOL CTabbedToDoCtrl::SelectTask(DWORD dwTaskID)
{
	BOOL bRes = CToDoCtrl::SelectTask(dwTaskID);

	// check task has not been filtered out
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		// handled above
		break;

	case FTCV_TASKLIST:
		{
			int nItem = FindListTask(dwTaskID);

			if (nItem == -1)
			{
				ASSERT(0);
				return FALSE;
			}
			
			// remove focused state from existing task
			int nFocus = m_list.GetNextItem(-1, LVNI_FOCUSED | LVNI_SELECTED);

			if (nFocus != -1)
				m_list.SetItemState(nFocus, 0, LVIS_SELECTED | LVIS_FOCUSED);

			m_list.SetItemState(nItem, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);
			m_list.EnsureVisible(nItem, FALSE);

			ScrollClientColumnIntoView();
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		{
			int nExtension = (nView - FTCV_UIEXTENSION1);

			if (nExtension >= 0 && nExtension < m_aExtViews.GetSize())
			{
				IUIExtensionWindow* pExtWnd = m_aExtViews[nExtension];
				
				if (pExtWnd)
					pExtWnd->SelectTask(dwTaskID);
			}
		}
		break;

	default:
		ASSERT(0);
	}

	return bRes;
}

BOOL CTabbedToDoCtrl::EditSelectedTask(BOOL bTaskIsNew)
{
	ScrollClientColumnIntoView(); // handles views

	return CToDoCtrl::EditSelectedTask(bTaskIsNew);
}

void CTabbedToDoCtrl::ScrollClientColumnIntoView()
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		// nothing to do
		break;

	case FTCV_TASKLIST:
		// check task has not been filtered out
		if (m_list.GetItemCount())
		{
			// scroll client column into view if necessary
			CRect rItem, rClient;
			GetItemTitleRect(0, TDCTR_EDIT, rItem);

			m_list.GetWindowRect(rClient);
			ScreenToClient(rClient);

			CSize pos(0, 0);

			if (rItem.left < rClient.left)
			{
				// scroll left edge to left client edge
				pos.cx = rItem.left - rClient.left;
			}
			else if (rItem.right > rClient.right)
			{
				// scroll right edge to right client edge so long as
				// left edge is still in view
				int nToScrollRight = rItem.right - rClient.right;
				int nMaxScrollLeft = rItem.left - rClient.left;

				pos.cx = min(nMaxScrollLeft, nToScrollRight);
			}

			if (pos.cx)
				m_list.Scroll(pos);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
}

LRESULT CTabbedToDoCtrl::OnEditCancel(WPARAM wParam, LPARAM lParam)
{
	// check if we need to delete the just added item
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		// handled below
		break;

	case FTCV_TASKLIST:
		if (GetSelectedTaskID() == m_dwLastAddedID)
		{
			int nDelItem = GetFirstSelectedItem();
			m_list.DeleteItem(nDelItem);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	return CToDoCtrl::OnEditCancel(wParam, lParam);
}

int CTabbedToDoCtrl::FindListTask(DWORD dwTaskID) const
{
	if (!dwTaskID)
		return -1;

	LVFINDINFO lvfi;
	ZeroMemory(&lvfi, sizeof(lvfi));

    lvfi.flags = LVFI_PARAM;
    lvfi.lParam = dwTaskID;
    lvfi.vkDirection = VK_DOWN;

	return m_list.FindItem(&lvfi);
}

DWORD CTabbedToDoCtrl::GetFocusedListTaskID() const
{
	int nItem = GetFocusedListItem();

	if (nItem != -1)
		return m_list.GetItemData(nItem);

	// else
	return 0;
}

int CTabbedToDoCtrl::GetFocusedListItem() const
{
	return m_list.GetNextItem(-1, LVNI_FOCUSED | LVNI_SELECTED);
}

void CTabbedToDoCtrl::SetSelectedListTasks(const CDWordArray& aTaskIDs, DWORD dwFocusedTaskID)
{
	ClearListSelection();

	BOOL bFocusHandled = FALSE;

	for (int nTask = 0; nTask < aTaskIDs.GetSize(); nTask++)
	{
		DWORD dwTaskID = aTaskIDs[nTask];
		int nItem = FindListTask(dwTaskID);

		if (nItem != -1)
		{
			if (dwTaskID == dwFocusedTaskID)
			{
				m_list.SetItemState(nItem, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);
				bFocusHandled = TRUE;
			}
			else
				m_list.SetItemState(nItem, LVIS_SELECTED, LVIS_SELECTED);
		}
		else // deselect in tree
		{
			HTREEITEM hti = m_find.GetItem(dwTaskID);
			Selection().SetItem(hti, TSHS_DESELECT, FALSE);
		}
	}

	// handle focus if it wasn't in aTasksID
	if (!bFocusHandled)
	{
		int nItem = FindListTask(dwFocusedTaskID);

		if (nItem != -1)
			m_list.SetItemState(nItem, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);
	}
}

LRESULT CTabbedToDoCtrl::OnGutterWidthChange(WPARAM wParam, LPARAM lParam)
{
	CToDoCtrl::OnGutterWidthChange(wParam, lParam);

	// update column widths if in list view
	UpdateListColumnWidths();
	
	return 0;
}

void CTabbedToDoCtrl::OnListSelChanged(NMHDR* /*pNMHDR*/, LRESULT* pResult)
{
	*pResult = 0;
}

void CTabbedToDoCtrl::ClearListSelection()
{
	//CHoldRedraw hr2(m_list);

	POSITION pos = m_list.GetFirstSelectedItemPosition();

	while (pos)
	{
		int nItem = m_list.GetNextSelectedItem(pos);
		m_list.SetItemState(nItem, 0, LVIS_SELECTED);
	}
}

int CTabbedToDoCtrl::GetFirstSelectedItem() const
{
	return m_list.GetNextItem(-1, LVNI_SELECTED);
}

int CTabbedToDoCtrl::GetSelectedListTaskIDs(CDWordArray& aTaskIDs, DWORD& dwFocusedTaskID) const
{
	aTaskIDs.RemoveAll();
	aTaskIDs.SetSize(m_list.GetSelectedCount());

	int nCount = 0;
	POSITION pos = m_list.GetFirstSelectedItemPosition();

	while (pos)
	{
		int nItem = m_list.GetNextSelectedItem(pos);

		aTaskIDs[nCount] = m_list.GetItemData(nItem);
		nCount++;
	}

	dwFocusedTaskID = GetFocusedListTaskID();

	return aTaskIDs.GetSize();
}

void CTabbedToDoCtrl::CacheListSelection(TDCSELECTIONCACHE& cache) const
{
	if (GetSelectedListTaskIDs(cache.aSelTaskIDs, cache.dwFocusedTaskID) == 0)
		return;

	// breadcrumbs
	cache.aBreadcrumbs.RemoveAll();

	// cache the preceding and following 10 tasks
	int nFocus = GetFocusedListItem(), nItem;
	int nMin = 0, nMax = m_list.GetItemCount() - 1;

	nMin = max(nMin, nFocus - 11);
	nMax = min(nMax, nFocus + 11);

	// following tasks first
	for (nItem = (nFocus + 1); nItem <= nMax; nItem++)
		cache.aBreadcrumbs.InsertAt(0, m_list.GetItemData(nItem));

	for (nItem = (nFocus - 1); nItem >= nMin; nItem--)
		cache.aBreadcrumbs.InsertAt(0, m_list.GetItemData(nItem));
}

void CTabbedToDoCtrl::RestoreListSelection(const TDCSELECTIONCACHE& cache)
{
	ClearListSelection();

	if (cache.aSelTaskIDs.GetSize() == 0)
		return;

	DWORD dwFocusedTaskID = cache.dwFocusedTaskID;
	ASSERT(dwFocusedTaskID);

	if (FindListTask(dwFocusedTaskID) == -1)
	{
		dwFocusedTaskID = 0;
		int nID = cache.aBreadcrumbs.GetSize();
		
		while (nID--)
		{
			dwFocusedTaskID = cache.aBreadcrumbs[nID];
			
			if (FindListTask(dwFocusedTaskID) != -1)
				break;
			else
				dwFocusedTaskID = 0;
		}
	}

	SetSelectedListTasks(cache.aSelTaskIDs, dwFocusedTaskID);
}

BOOL CTabbedToDoCtrl::SetTreeFont(HFONT hFont)
{
	CToDoCtrl::SetTreeFont(hFont);

	if (m_list.GetSafeHwnd())
	{
		if (!hFont) // set to our font
		{
			// for some reason i can not yet explain, our font
			// is not correctly set so we use our parent's font instead
			// hFont = (HFONT)SendMessage(WM_GETFONT);
			hFont = (HFONT)GetParent()->SendMessage(WM_GETFONT);
		}

		HFONT hListFont = (HFONT)m_list.SendMessage(WM_GETFONT);
		BOOL bChange = (hFont != hListFont || !GraphicsMisc::SameFontNameSize(hFont, hListFont));

		if (bChange)
		{
			m_list.SendMessage(WM_SETFONT, (WPARAM)hFont, TRUE);
			RemeasureList();

			if (InListView())
				m_list.Invalidate(TRUE);
		}
	}

	// other views
	// TODO

	return TRUE;
}

BOOL CTabbedToDoCtrl::AddView(IUIExtension* pExtension)
{
	if (!pExtension)
		return FALSE;

	// remove any existing views of this type
	RemoveView(pExtension);

	// add to tab control
	HICON hIcon = pExtension->GetIcon();
	CEnString sName(pExtension->GetMenuText());

	int nIndex = m_aExtViews.GetSize();
	FTC_VIEW nView = (FTC_VIEW)(FTCV_UIEXTENSION1 + nIndex);

	VIEWDATA* pData = NewViewData();
	ASSERT(pData);

	pData->pExtension = pExtension;

	// we pass NULL for the hWnd because we are going to load
	// only on demand
	if (m_tabViews.AttachView(NULL, nView, sName, hIcon, pData))
	{
		m_aExtViews.Add(NULL); // placeholder
		return TRUE;
	}

	return FALSE;
}

BOOL CTabbedToDoCtrl::RemoveView(IUIExtension* pExtension)
{
	// search for any views having this type
	int nView = m_aExtViews.GetSize();

	while (nView--)
	{
		IUIExtensionWindow* pExtWnd = m_aExtViews[nView];

		if (pExtWnd) // can be NULL
		{
			CString sExtType = pExtension->GetTypeID();
			CString sExtWndType = pExtWnd->GetTypeID();

			if (sExtType == sExtWndType)
			{
				VERIFY (m_tabViews.DetachView(pExtWnd->GetHwnd()));
				pExtWnd->Release();

				m_aExtViews.RemoveAt(nView);

				return TRUE;
			}
		}
	}

	return FALSE;
}

void CTabbedToDoCtrl::OnListClick(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMITEMACTIVATE pNMIA = (LPNMITEMACTIVATE)pNMHDR;

	UpdateTreeSelection();

	if (pNMIA->iItem != -1) // validate item
	{
		int nHit = pNMIA->iItem;
		TDC_COLUMN nCol = GetListColumnID(pNMIA->iSubItem);

		DWORD dwClickID = m_list.GetItemData(nHit);
		HTREEITEM htiHit = m_find.GetItem(dwClickID);
		ASSERT(htiHit);

		// column specific handling
		BOOL bCtrl = Misc::KeyIsPressed(VK_CONTROL);

		switch (nCol)
		{
		case TDCC_CLIENT:
			if (dwClickID == m_dw2ndClickItem)
				m_list.PostMessage(LVM_EDITLABEL);
			else
			{
				CRect rCheck;
								
				if (GetItemTitleRect(nHit, TDCTR_CHECKBOX, rCheck))
				{
					CPoint ptHit(::GetMessagePos());
					m_list.ScreenToClient(&ptHit);

					if (rCheck.PtInRect(ptHit))
					{
						BOOL bDone = m_data.IsTaskDone(dwClickID);
						SetSelectedTaskDone(!bDone);
					}
				}
			}
			break;
			
		case TDCC_FILEREF:
			if (bCtrl)
			{
				CString sFile = m_data.GetTaskFileRef(dwClickID);
				
				if (!sFile.IsEmpty())
					GotoFile(sFile, TRUE);
			}
			break;
			
		case TDCC_DEPENDENCY:
			if (bCtrl)
			{
				CStringArray aDepends;
				m_data.GetTaskDependencies(dwClickID, aDepends);
				
				if (aDepends.GetSize())
					ShowTaskLink(0, aDepends[0]);
			}
			break;
			
		case TDCC_TRACKTIME:
			if (!IsReadOnly())
			{
				if (GetSelectedCount() == 1 && IsItemSelected(nHit) && m_data.IsTaskTimeTrackable(dwClickID))
				{
					int nPrev = FindListTask(m_dwTimeTrackTaskID);

					if (nPrev == -1)
					{
						m_list.RedrawItems(nPrev, nPrev);
						m_list.UpdateWindow();
					}

					TimeTrackTask(htiHit);
					m_list.RedrawItems(nHit, nHit);
					m_list.UpdateWindow();

					// resort if required
					VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);

					if (!pLVData->bMultiSort && pLVData->sort.nBy1 == TDCC_TRACKTIME)
						Sort(TDCC_TRACKTIME, FALSE);
				}
			}
			break;
			
		case TDCC_DONE:
			if (!IsReadOnly())
				SetSelectedTaskDone(!m_data.IsTaskDone(dwClickID));
			break;
			
		case TDCC_FLAG:
			if (!IsReadOnly())
			{
				BOOL bFlagged = m_data.IsTaskFlagged(dwClickID);
				SetSelectedTaskFlag(!bFlagged);
			}
			break;

		default:
			// handle clicks on 'flag' custom attributes
			if (HandleCustomColumnClick(nCol))
				return;
		}
	}

	m_dw2ndClickItem = 0;
	
	*pResult = 0;
}

void CTabbedToDoCtrl::OnListDblClick(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMITEMACTIVATE pNMIA = (LPNMITEMACTIVATE)pNMHDR;

	if (pNMIA->iItem != -1) // validate item
	{
		DWORD dwTaskID = GetTaskID(pNMIA->iItem);
		TDC_COLUMN nCol = GetListColumnID(pNMIA->iSubItem);
		
		switch (nCol)
		{
		case TDCC_CLIENT:
			{
				// where did they double-click?
				CRect rItem;
				
				if (GetItemTitleRect(pNMIA->iItem, TDCTR_ICON, rItem) && rItem.PtInRect(pNMIA->ptAction))
					EditSelectedTaskIcon();
				else
				{
					CClientDC dc(&m_list);
					GetItemTitleRect(pNMIA->iItem, TDCTR_LABEL, rItem, &dc, GetSelectedTaskTitle());
					
					if (rItem.PtInRect(pNMIA->ptAction))
						m_list.PostMessage(LVM_EDITLABEL);
				}
			}
			break;

		case TDCC_FILEREF:
			{
				CString sFile = m_data.GetTaskFileRef(dwTaskID);
				
				if (!sFile.IsEmpty())
					GotoFile(sFile, TRUE);
			}
			break;
			
		case TDCC_DEPENDENCY:
			{
				CStringArray aDepends;
				m_data.GetTaskDependencies(dwTaskID, aDepends);
				
				if (aDepends.GetSize())
					ShowTaskLink(0, aDepends[0]);
			}
			break;
						
		case TDCC_RECURRENCE:
			m_eRecurrence.DoEdit();
			break;
					
		case TDCC_ICON:
			EditSelectedTaskIcon();
			break;
			
		case TDCC_REMINDER:
			AfxGetMainWnd()->SendMessage(WM_TDCN_DOUBLECLKREMINDERCOL);
			break;
		}
	}
	
	*pResult = 0;
}

void CTabbedToDoCtrl::OnListKeyDown(NMHDR* /*pNMHDR*/, LRESULT* pResult)
{
	// for reasons I have not yet divined, we are not receiving this message
	// as expected. So I've added an ASSERT(0) should it ever come to life
	// and have handled the key down message in ScWindowProc
	//LPNMKEY pNMK = (LPNMKEY)pNMHDR;
	ASSERT(0);
//	TRACE ("CTabbedToDoCtrl::OnListKeyDown\n");
//	UpdateTreeSelection();

	*pResult = 0;
}

BOOL CTabbedToDoCtrl::PtInHeader(CPoint ptScreen) const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::PtInHeader(ptScreen);

	case FTCV_TASKLIST:
		return m_list.PtInHeader(ptScreen);

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	// else
	return FALSE;
}

BOOL CTabbedToDoCtrl::IsMultiSorting() const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::IsMultiSorting();

	case FTCV_TASKLIST:
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	// all else
	return GetActiveViewData()->bMultiSort;
}

void CTabbedToDoCtrl::MultiSort(const TDSORTCOLUMNS& sort)
{
	ASSERT (sort.nBy1 != TDCC_NONE);

	if (sort.nBy1 == TDCC_NONE)
		return;

	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::MultiSort(sort);
		break;

	case FTCV_TASKLIST:
		{
			TDSORTPARAMS ss;
			ss.sort = sort;
			ss.pData = &m_data;
			ss.bSortChildren = FALSE;
			ss.dwTimeTrackID = 0;
			ss.pAttributeDefs = &m_aCustomAttribDefs;

			m_list.SortItems(CToDoCtrl::SortFuncMulti, (DWORD)&ss);
			
			VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);

			pLVData->bModSinceLastSort = FALSE;
			pLVData->bMultiSort = TRUE;
			pLVData->sort = sort;

			// update registry
			SaveSortState(CPreferences());

			UpdateListColumnWidths();
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
}

void CTabbedToDoCtrl::Sort(TDC_COLUMN nBy, BOOL bAllowToggle)
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::Sort(nBy, bAllowToggle);
		break;

	case FTCV_TASKLIST:
		{
			VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);

			// first time?
			if (pLVData->sort.bAscending1 == -1 || nBy != pLVData->sort.nBy1) 
			{
				int nCol = GetListColumnIndex(nBy);
				TDCCOLUMN* pTDCC = GetListColumn(nCol);

				if (pTDCC)
					pLVData->sort.bAscending1 = pTDCC->bSortAscending;
				
				else if (CTDCCustomAttributeHelper::IsCustomColumn(nBy))
					pLVData->sort.bAscending1 = TRUE;
			}
			// if there's been a mod since last sorting then its reasonable to assume
			// that the user is not toggling direction but wants to simply resort
			// in the same direction.
			else if (bAllowToggle && !pLVData->bModSinceLastSort)
				pLVData->sort.bAscending1 = !pLVData->sort.bAscending1;
			
			// do the sort using whatever we can out of CToDoCtrlData
			TDSORTPARAMS ss(nBy, pLVData->sort.bAscending1);

			ss.pData = &m_data;
			ss.bSortChildren = FALSE;
			ss.dwTimeTrackID = m_dwTimeTrackTaskID;
			ss.pAttributeDefs = &m_aCustomAttribDefs;

			m_list.SortItems(CToDoCtrl::SortFunc, (DWORD)&ss);
			
			pLVData->sort.nBy1 = nBy;
			pLVData->sort.nBy2 = TDCC_NONE;
			pLVData->sort.nBy3 = TDCC_NONE;
			pLVData->bModSinceLastSort = FALSE;
			pLVData->bMultiSort = FALSE;
			
			// update registry
			SaveSortState(CPreferences());

			UpdateListColumnWidths();
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		ExtensionDoAppCommand(nView, bAllowToggle ? IUI_TOGGLESORT : IUI_RESORT);
		break;

	default:
		ASSERT(0);
	}
}

BOOL CTabbedToDoCtrl::MoveSelectedTask(TDC_MOVETASK nDirection) 
{ 
	return !InTreeView() ? FALSE : CToDoCtrl::MoveSelectedTask(nDirection); 
}

BOOL CTabbedToDoCtrl::CanMoveSelectedTask(TDC_MOVETASK nDirection) const 
{ 
	return !InTreeView() ? FALSE : CToDoCtrl::CanMoveSelectedTask(nDirection); 
}

BOOL CTabbedToDoCtrl::GotoNextTask(TDC_GOTO nDirection)
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::GotoNextTask(nDirection);

	case FTCV_TASKLIST:
		if (CanGotoNextTask(nDirection))
		{
			int nSel = GetFirstSelectedItem();

			if (nDirection == TDCG_NEXT)
				nSel++;
			else
				nSel--;

			return SelectTask(m_list.GetItemData(nSel));
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
	
	// else
	return FALSE;
}

CRect CTabbedToDoCtrl::GetSelectedItemsRect() const
{
	CRect rInvalid(0, 0, 0, 0), rItem;
	POSITION pos = m_list.GetFirstSelectedItemPosition();

	while (pos)
	{
		int nItem = m_list.GetNextSelectedItem(pos);

		if (m_list.GetItemRect(nItem, rItem, LVIR_BOUNDS))
			rInvalid |= rItem;
	}

	return rInvalid;
}

BOOL CTabbedToDoCtrl::CanGotoNextTask(TDC_GOTO nDirection) const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::CanGotoNextTask(nDirection);

	case FTCV_TASKLIST:
		{
			int nSel = GetFirstSelectedItem();

			if (nDirection == TDCG_NEXT)
				return (nSel >= 0 && nSel < m_list.GetItemCount() - 1);
			
			// else prev
			return (nSel > 0 && nSel <= m_list.GetItemCount() - 1);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
	
	// else
	return FALSE;
}

BOOL CTabbedToDoCtrl::GotoNextTopLevelTask(TDC_GOTO nDirection)
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::GotoNextTopLevelTask(nDirection);

	case FTCV_TASKLIST:
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	// else
	return FALSE; // not supported
}

BOOL CTabbedToDoCtrl::CanGotoNextTopLevelTask(TDC_GOTO nDirection) const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::CanGotoNextTopLevelTask(nDirection);

	case FTCV_TASKLIST:
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	// else
	return FALSE; // not supported
}

void CTabbedToDoCtrl::ExpandTasks(TDC_EXPANDCOLLAPSE nWhat, BOOL bExpand)
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::ExpandTasks(nWhat, bExpand);

	case FTCV_TASKLIST:
		// no can do!
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		if (bExpand)
		{
			switch (nWhat)
			{
			case TDCEC_ALL:
				ExtensionDoAppCommand(nView, IUI_EXPANDALL);
				break;

			case TDCEC_SELECTED:
				ExtensionDoAppCommand(nView, IUI_EXPANDSELECTED);
				break;
			}
		}
		else // collapse
		{
			switch (nWhat)
			{
			case TDCEC_ALL:
				ExtensionDoAppCommand(nView, IUI_COLLAPSEALL);
				break;

			case TDCEC_SELECTED:
				ExtensionDoAppCommand(nView, IUI_COLLAPSESELECTED);
				break;
			}
		}
		break;

	default:
		ASSERT(0);
	}
}

BOOL CTabbedToDoCtrl::CanExpandTasks(TDC_EXPANDCOLLAPSE nWhat, BOOL bExpand) const 
{ 
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::CanExpandTasks(nWhat, bExpand);

	case FTCV_TASKLIST:
		// no can do!
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		if (bExpand)
		{
			switch (nWhat)
			{
			case TDCEC_ALL:
				return ExtensionCanDoAppCommand(nView, IUI_EXPANDALL);

			case TDCEC_SELECTED:
				return ExtensionCanDoAppCommand(nView, IUI_EXPANDSELECTED);
			}
		}
		else // collapse
		{
			switch (nWhat)
			{
			case TDCEC_ALL:
				return ExtensionCanDoAppCommand(nView, IUI_COLLAPSEALL);

			case TDCEC_SELECTED:
				return ExtensionCanDoAppCommand(nView, IUI_COLLAPSESELECTED);
			}
		}
		break;

	default:
		ASSERT(0);
	}

	// else
	return FALSE; // not supported
}

void CTabbedToDoCtrl::ExtensionDoAppCommand(FTC_VIEW nView, IUI_APPCOMMAND nCmd)
{
	int nExt = (nView - FTCV_UIEXTENSION1);
	
	if (nExt >= 0 && nExt < m_aExtViews.GetSize())
	{
		IUIExtensionWindow* pExt = m_aExtViews[nExt];

		if (pExt)
			pExt->DoAppCommand(nCmd);
	}
	else
		ASSERT(0);
}

BOOL CTabbedToDoCtrl::ExtensionCanDoAppCommand(FTC_VIEW nView, IUI_APPCOMMAND nCmd) const
{
	int nExt = (nView - FTCV_UIEXTENSION1);
	
	if (nExt >= 0 && nExt < m_aExtViews.GetSize())
	{
		IUIExtensionWindow* pExt = m_aExtViews[nExt];

		if (pExt)
			return pExt->CanDoAppCommand(nCmd);
	}
	else
		ASSERT(0);

	return FALSE;
}

void CTabbedToDoCtrl::SetFocusToTasks()
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::SetFocusToTasks();
		break;

	case FTCV_TASKLIST:
		m_list.SetFocus();
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		ExtensionDoAppCommand(nView, IUI_SETFOCUS);
		break;

	default:
		ASSERT(0);
	}
}

BOOL CTabbedToDoCtrl::TasksHaveFocus() const
{ 
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::TasksHaveFocus(); 

	case FTCV_TASKLIST:
		return (::GetFocus() == m_list);

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		return ExtensionCanDoAppCommand(nView, IUI_SETFOCUS);

	default:
		ASSERT(0);
	}
	
	return FALSE;
}

int CTabbedToDoCtrl::FindTasks(const SEARCHPARAMS& params, CResultArray& aResults) const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::FindTasks(params, aResults);

	case FTCV_TASKLIST:
		{
			for (int nItem = 0; nItem < m_list.GetItemCount(); nItem++)
			{
				DWORD dwTaskID = GetTaskID(nItem);
				SEARCHRESULT result;

				if (m_data.TaskMatches(dwTaskID, params, result))
					aResults.Add(result);
			}
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	return aResults.GetSize();
}


BOOL CTabbedToDoCtrl::SelectTask(CString sPart, TDC_SELECTTASK nSelect)
{
	int nFind = -1;
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::SelectTask(sPart, nSelect);

	case FTCV_TASKLIST:
		switch (nSelect)
		{
		case TDC_SELECTFIRST:
			nFind = FindListTask(sPart);
			break;
			
		case TDC_SELECTNEXT:
			nFind = FindListTask(sPart, GetFirstSelectedItem() + 1);
			break;
			
		case TDC_SELECTNEXTINCLCURRENT:
			nFind = FindListTask(sPart, GetFirstSelectedItem());
			break;
			
		case TDC_SELECTPREV:
			nFind = FindListTask(sPart, GetFirstSelectedItem() - 1, FALSE);
			break;
			
		case TDC_SELECTLAST:
			nFind = FindListTask(sPart, m_list.GetItemCount() - 1, FALSE);
			break;
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	// else
	if (nFind != -1)
		return SelectTask(GetTaskID(nFind));

	return FALSE;
}

int CTabbedToDoCtrl::FindListTask(const CString& sPart, int nStart, BOOL bNext)
{
	// build a search query
	SEARCHPARAMS params;
	params.aRules.Add(SEARCHPARAM(TDCA_TASKNAMEORCOMMENTS, FO_INCLUDES, sPart));

	// we need to do this manually because CListCtrl::FindItem 
	// only looks at the start of the string
	SEARCHRESULT result;

	int nFrom = nStart;
	int nTo = bNext ? m_list.GetItemCount() : -1;
	int nInc = bNext ? 1 : -1;

	for (int nItem = nFrom; nItem != nTo; nItem += nInc)
	{
		DWORD dwTaskID = GetTaskID(nItem);

		if (m_data.TaskMatches(dwTaskID, params, result))
			return nItem;
	}

	return -1; // no match
}

void CTabbedToDoCtrl::SelectNextTasksInHistory()
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::SelectNextTasksInHistory();
		break;

	case FTCV_TASKLIST:
		if (CanSelectNextTasksInHistory())
		{
			// let CToDoCtrl do it's thing
			CToDoCtrl::SelectNextTasksInHistory();

			// then update our own selection
			ResyncListSelection();
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
}

BOOL CTabbedToDoCtrl::MultiSelectItems(const CDWordArray& aTasks, TSH_SELECT nState, BOOL bRedraw)
{
	BOOL bRes = CToDoCtrl::MultiSelectItems(aTasks, nState, bRedraw);

	// extra processing
	if (bRes)
	{
		FTC_VIEW nView = GetView();

		switch (nView)
		{
		case FTCV_TASKTREE:
		case FTCV_UNSET:
			break;

		case FTCV_TASKLIST:
			ResyncListSelection();
			break;

		case FTCV_UIEXTENSION1:
		case FTCV_UIEXTENSION2:
		case FTCV_UIEXTENSION3:
		case FTCV_UIEXTENSION4:
		case FTCV_UIEXTENSION5:
		case FTCV_UIEXTENSION6:
		case FTCV_UIEXTENSION7:
		case FTCV_UIEXTENSION8:
		case FTCV_UIEXTENSION9:
		case FTCV_UIEXTENSION10:
		case FTCV_UIEXTENSION11:
		case FTCV_UIEXTENSION12:
		case FTCV_UIEXTENSION13:
		case FTCV_UIEXTENSION14:
		case FTCV_UIEXTENSION15:
		case FTCV_UIEXTENSION16:
			break;

		default:
			ASSERT(0);
		}
	}

	return bRes;
}

void CTabbedToDoCtrl::ResyncListSelection()
{
	ClearListSelection();
		
	// then update our own selection
	TDCSELECTIONCACHE cacheTree;

	CacheTreeSelection(cacheTree);
	RestoreListSelection(cacheTree);
	
	// now check that the tree is synced with us!
	TDCSELECTIONCACHE cacheList;
	CacheListSelection(cacheList);

	if (!Misc::MatchAll(cacheTree.aSelTaskIDs, cacheList.aSelTaskIDs))
		RestoreTreeSelection(cacheList);
}


void CTabbedToDoCtrl::SelectPrevTasksInHistory()
{
	if (CanSelectPrevTasksInHistory())
	{
		// let CToDoCtrl do it's thing
		CToDoCtrl::SelectPrevTasksInHistory();

		// extra processing
		FTC_VIEW nView = GetView();

		switch (nView)
		{
		case FTCV_TASKTREE:
		case FTCV_UNSET:
			// handled above
			break;

		case FTCV_TASKLIST:
			// then update our own selection
			ResyncListSelection();
			break;

		case FTCV_UIEXTENSION1:
		case FTCV_UIEXTENSION2:
		case FTCV_UIEXTENSION3:
		case FTCV_UIEXTENSION4:
		case FTCV_UIEXTENSION5:
		case FTCV_UIEXTENSION6:
		case FTCV_UIEXTENSION7:
		case FTCV_UIEXTENSION8:
		case FTCV_UIEXTENSION9:
		case FTCV_UIEXTENSION10:
		case FTCV_UIEXTENSION11:
		case FTCV_UIEXTENSION12:
		case FTCV_UIEXTENSION13:
		case FTCV_UIEXTENSION14:
		case FTCV_UIEXTENSION15:
		case FTCV_UIEXTENSION16:
			break;

		default:
			ASSERT(0);
		}
	}
}

void CTabbedToDoCtrl::InvalidateItem(HTREEITEM hti)
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::InvalidateItem(hti);
		break;

	case FTCV_TASKLIST:
		{
			int nItem = GetListItem(hti);
			CRect rItem;

			if (GetItemTitleRect(nItem, TDCTR_BOUNDS, rItem))
				m_list.InvalidateRect(rItem, FALSE);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
}

LRESULT CTabbedToDoCtrl::ScWindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_RBUTTONDOWN:
		{
			// work out what got hit and make sure it's selected
			CPoint ptHit(lp);
			LVHITTESTINFO lvhti = { { ptHit.x, ptHit.y }, 0, 0, 0 };

			int nHit = m_list.HitTest(&lvhti);

			if (!IsItemSelected(nHit))
				SelectTask(GetTaskID(nHit));
		}
		break;

	case WM_LBUTTONDOWN:
		{
			// work out what got hit
			CPoint ptHit(lp);
			LVHITTESTINFO lvhti = { { ptHit.x, ptHit.y }, 0, 0, 0 };

			m_list.SubItemHitTest(&lvhti);
			int nHit = lvhti.iItem;

			if (nHit != -1 && !IsReadOnly())
			{
				TDC_COLUMN nCol = GetListColumnID(lvhti.iSubItem);
				DWORD dwTaskID = GetTaskID(nHit);

				// if the user is clicking on an already multi-selected
				// item since we may need to carry out an operation on multiple items
				int nSelCount = GetSelectedCount();

				if (nSelCount > 1 && IsItemSelected(nHit))
				{
					switch (nCol)
					{
					case TDCC_DONE:
						{
							BOOL bDone = m_data.IsTaskDone(dwTaskID);
							SetSelectedTaskDone(!bDone);
							return 0; // eat it
						}
						break;

					case TDCC_CLIENT:
						{
							CRect rCheck;

							if (GetItemTitleRect(nHit, TDCTR_CHECKBOX, rCheck) && rCheck.PtInRect(ptHit))
							{
								BOOL bDone = m_data.IsTaskDone(dwTaskID);
								SetSelectedTaskDone(!bDone);
								return 0; // eat it
							}
						}
						break;
						
					case TDCC_FLAG:
						{
							BOOL bFlagged = m_data.IsTaskFlagged(dwTaskID);
							SetSelectedTaskFlag(!bFlagged);
							return 0; // eat it
						}
						break;
					}
				}
				else if (nSelCount == 1)
				{
					// if the click was on the task title of an already singly selected item
					// we record the task ID unless the control key is down in which case
					// it really means that the user has deselected the item
					if (!Misc::KeyIsPressed(VK_CONTROL))
					{
						m_dw2ndClickItem = 0;
						
						int nSel = GetFirstSelectedItem();
						if (nHit != -1 && nHit == nSel && nCol == TDCC_CLIENT)
						{
							// unfortunately we cannot rely on the flags attribute of LVHITTESTINFO
							// to see if the user clicked on the text because LVIR_LABEL == LVIR_BOUNDS
							CRect rLabel;
							CClientDC dc(&m_list);
							CFont* pOld = NULL;

							if (m_tree.GetParentItem(GetTreeItem(nHit)) == NULL) // top level items
								pOld = (CFont*)dc.SelectObject(CFont::FromHandle(m_hFontBold));
							else
								pOld = (CFont*)dc.SelectObject(m_list.GetFont());

							GetItemTitleRect(nHit, TDCTR_LABEL, rLabel, &dc, GetSelectedTaskTitle());
			
							if (rLabel.PtInRect(ptHit))
								m_dw2ndClickItem = m_list.GetItemData(nHit);

							// cleanup
							dc.SelectObject(pOld);
						}

						// note: we handle WM_LBUTTONUP in OnListClick() to 
						// decide whether to do a label edit
					}
				}
			}

			// because the visual state of the list selection is actually handled by
			// whether the tree selection is up to date we need to update the tree
			// selection here, because the list ctrl does it this way natively.
			LRESULT	lr = CSubclasser::ScWindowProc(hRealWnd, msg, wp, lp);
			UpdateTreeSelection();

			return lr;
		}
		break;

	case LVM_EDITLABEL:
		if (!IsReadOnly())
			EditSelectedTask(FALSE);
		return 0; // eat it

	case WM_KEYDOWN:
		{
			// if any of the cursor keys are used and nothing is currently selected
			// then we select the top/bottom item and ignore the default list ctrl processing
			LRESULT lr = CSubclasser::ScWindowProc(hRealWnd, msg, wp, lp);
			m_wKeyPress = (WORD)wp;

			if (Misc::IsCursorKey(wp))
				UpdateTreeSelection();

			return lr;
		}
		break;

	case WM_KEYUP:
		if (Misc::IsCursorKey(wp))
			UpdateControls();
		break;

	case WM_MOUSEWHEEL:
	case WM_VSCROLL:
	case WM_HSCROLL:
		if (InListView())
		{
			if (IsTaskLabelEditing())
				EndLabelEdit(FALSE);

			// extra processing for WM_HSCROLL
 			if (msg == WM_HSCROLL)
 				m_list.RedrawHeader();
		}
		break;

		
	case WM_NOTIFY:
		{
			LPNMHDR pNMHDR = (LPNMHDR)lp;
			
			switch (pNMHDR->code)
			{
			case NM_CUSTOMDRAW:
				if (pNMHDR->hwndFrom == m_list.GetHeader())
				{
					LRESULT lr = 0;
					OnListHeaderCustomDraw(pNMHDR, &lr);
					return lr;
				}
				break;
			}
		}
		break;
	}

	return CSubclasser::ScWindowProc(hRealWnd, msg, wp, lp);
}

void CTabbedToDoCtrl::UpdateTreeSelection()
{
	// update the tree selection if it needs to be
	CDWordArray aListTaskIDs, aTreeTaskIDs;
	DWORD dwTreeFocusedID, dwListFocusedID;

	GetSelectedTaskIDs(aTreeTaskIDs, dwTreeFocusedID, FALSE);
	GetSelectedListTaskIDs(aListTaskIDs, dwListFocusedID);
	
	if (!Misc::MatchAll(aTreeTaskIDs, aListTaskIDs) || (dwTreeFocusedID != dwListFocusedID))
	{
		// select tree item first then multi-select after
		HTREEITEM htiFocus = m_find.GetItem(dwListFocusedID);
		m_tree.SelectItem(htiFocus);

		MultiSelectItems(aListTaskIDs, TSHS_SELECT, FALSE);

		// reset list selection
		int nFocus = FindListTask(dwListFocusedID);
		m_list.SetItemState(nFocus, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);
		m_list.RedrawItems(nFocus, nFocus);
	}
}

BOOL CTabbedToDoCtrl::IsItemSelected(int nItem) const
{
	HTREEITEM hti = GetTreeItem(nItem);
	return hti ? Selection().HasItem(hti) : FALSE;
}

HTREEITEM CTabbedToDoCtrl::GetTreeItem(int nItem) const
{
	if (nItem < 0 || nItem >= m_list.GetItemCount())
		return NULL;

	DWORD dwID = m_list.GetItemData(nItem);
	return m_find.GetItem(dwID);
}

int CTabbedToDoCtrl::GetListItem(HTREEITEM hti) const
{
	DWORD dwID = GetTaskID(hti);
	return (dwID ? FindListTask(dwID) : -1);
}

BOOL CTabbedToDoCtrl::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	if (&m_list == pWnd)
	{
		CPoint pt(::GetMessagePos());
		m_list.ScreenToClient(&pt);

		LVHITTESTINFO lvhti = { { pt.x, pt.y }, 0, 0, 0 };
		m_list.SubItemHitTest(&lvhti);

		int nHit = lvhti.iItem;

		if (nHit >= 0)
		{
			TDC_COLUMN nCol	= GetListColumnID(lvhti.iSubItem);
			DWORD dwID = m_list.GetItemData(nHit);

			BOOL bCtrl = Misc::KeyIsPressed(VK_CONTROL);
			BOOL bShowHand = FALSE;

			switch (nCol)
			{
			case TDCC_FILEREF:
				if (bCtrl)
				{
					CString sFile = m_data.GetTaskFileRef(dwID);
					bShowHand = (!sFile.IsEmpty());
				}
				break;
				
			case TDCC_DEPENDENCY:
				if (bCtrl)
				{
					CStringArray aDepends;
					m_data.GetTaskDependencies(dwID, aDepends);
					bShowHand = aDepends.GetSize();
				}
				break;
				
			case TDCC_TRACKTIME:
				if (!IsReadOnly())
				{
					bShowHand = ((!IsItemSelected(nHit) || GetSelectedCount() == 1) && 
								 m_data.IsTaskTimeTrackable(dwID));
				}
				break;
				
			case TDCC_ICON:
			case TDCC_FLAG:
				bShowHand = (dwID && !IsReadOnly());
				break;
		
			default:
				// handle custom attributes
				if (!IsReadOnly() && CTDCCustomAttributeHelper::IsCustomColumn(nCol))
				{
					TDCCUSTOMATTRIBUTEDEFINITION attribDef;
					VERIFY (CTDCCustomAttributeHelper::GetAttributeDef(nCol, m_aCustomAttribDefs, attribDef));
					
					switch (attribDef.GetDataType())
					{
					case TDCCA_BOOL:
					case TDCCA_ICON:
						bShowHand = TRUE;
						break;
					}
				}
			}

			if (bShowHand)
			{
				::SetCursor(GraphicsMisc::HandCursor());
				return TRUE;
			}
		}
	}
		
	return CToDoCtrl::OnSetCursor(pWnd, nHitTest, message);
}

LRESULT CTabbedToDoCtrl::OnDropObject(WPARAM wParam, LPARAM lParam)
{
	if (IsReadOnly())
		return 0L;

	TLDT_DATA* pData = (TLDT_DATA*)wParam;
	CWnd* pTarget = (CWnd*)lParam;
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		ASSERT(pTarget == &m_tree);
		return CToDoCtrl::OnDropObject(wParam, lParam);

	case FTCV_TASKLIST:
		if (pTarget == &m_list)
		{
			ASSERT (InListView());

			// simply convert the list item into the corresponding tree
			// item and pass to base class
			if (pData->nItem != -1)
				m_list.SetCurSel(pData->nItem);

			pData->hti = GetTreeItem(pData->nItem);
			pData->nItem = -1;
			lParam = (LPARAM)&m_tree;
		}
		// default handling
		return CToDoCtrl::OnDropObject(wParam, lParam);

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}

	// else
	return 0L;
}

BOOL CTabbedToDoCtrl::GetItemTitleRect(HTREEITEM hti, TDC_TITLERECT nArea, CRect& rect) const
{
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		return CToDoCtrl::GetItemTitleRect(hti, nArea, rect);

	case FTCV_TASKLIST:
		{
			int nItem = GetListItem(hti);
			return GetItemTitleRect(nItem, nArea, rect);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		if (nArea == TDCTR_EDIT || nArea == TDCTR_BOUNDS || nArea == TDCTR_LABEL)
		{
			int nExt = (nView - FTCV_UIEXTENSION1);

			if (nExt >= 0 && nExt < m_aExtViews.GetSize())
			{
				IUIExtensionWindow* pExt = m_aExtViews[nExt];

				if (pExt->GetLabelEditRect(rect))
				{
					// convert to our coordinates
					::ClientToScreen(pExt->GetHwnd(), &(rect.TopLeft()));
					::ClientToScreen(pExt->GetHwnd(), &(rect.BottomRight()));
					ScreenToClient(rect);

					return TRUE;
				}
			}
		}
		break;

	default:
		ASSERT(0);
	}

	return FALSE;
}

BOOL CTabbedToDoCtrl::GetItemTitleRect(int nItem, TDC_TITLERECT nArea, CRect& rect, CDC* pDC, LPCTSTR szTitle) const
{
	ASSERT (InListView());
	
	if (nItem == -1)
		return FALSE;

	// basic title rect
	int nColIndex = GetListColumnIndex(TDCC_CLIENT);
	const_cast<CTDCListView*>(&m_list)->GetSubItemRect(nItem, nColIndex, LVIR_LABEL, rect);

	if (nColIndex == 0 && COSVersion() >= OSV_XP) // right side columns in XP
		rect.left -= 4;

	BOOL bIcon = HasStyle(TDCS_TREETASKICONS) && !IsColumnShowing(TDCC_ICON);
	BOOL bCheckbox = HasStyle(TDCS_TREECHECKBOXES) && !IsColumnShowing(TDCC_DONE);

	switch (nArea)
	{
	case TDCTR_CHECKBOX:
		if (bCheckbox)
		{
			rect.right = rect.left + 16;

			// tree always places checkbox at bottom of rect
			rect.top += (rect.Height() - 16);

			return TRUE;
		}
		break;

	case TDCTR_ICON:
		if (bIcon)
		{
			if (bCheckbox)
				rect.left += 18;

			rect.right = rect.left + 16;

			// tree places icon vertically centred in rect
			rect.top += ((rect.Height() - 16) / 2);

			return TRUE;
		}
		break;
		
	case TDCTR_LABEL:
		{
			if (bIcon)
				rect.left += 18;

 			if (bCheckbox)
 				rect.left += 18;

			if (pDC && szTitle)
			{
				int nTextExt = pDC->GetTextExtent(szTitle).cx;
				rect.right = rect.left + min(rect.Width(), nTextExt + 6 + 1);
			}

			return TRUE;
		}
		break;

	case TDCTR_BOUNDS:
		return TRUE; // nothing more to do

	case TDCTR_EDIT:
		if (GetItemTitleRect(nItem, TDCTR_LABEL, rect)) // recursive call
		{
			CRect rClient, rInter;
			m_list.GetClientRect(rClient);
			
			if (rInter.IntersectRect(rect, rClient))
				rect = rInter;
			
			rect.top--;

			m_list.ClientToScreen(rect);
			ScreenToClient(rect);
			
			return TRUE;
		}
		break;
	}

	return FALSE;
}

void CTabbedToDoCtrl::OnListGetInfoTip(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMLVGETINFOTIP* pLVGIT = (NMLVGETINFOTIP*)pNMHDR;
	*pResult = 0;

	int nHit = pLVGIT->iItem;

	if (nHit >= 0)
	{
		HTREEITEM hti = GetTreeItem(nHit);
		DWORD dwTaskID = m_list.GetItemData(nHit);
		ASSERT(dwTaskID);

		if (!dwTaskID)
			return;

		// we only display info tips when over the task title
		CRect rTitle;
		CClientDC dc(&m_list);
		CFont* pOld = NULL;
		
		if (m_tree.GetParentItem(hti) == NULL) // top level item
			pOld = (CFont*)dc.SelectObject(CFont::FromHandle(m_hFontBold));
		else
			pOld = (CFont*)dc.SelectObject(m_list.GetFont());
		
		GetItemTitleRect(nHit, TDCTR_LABEL, rTitle, &dc, GetTaskTitle(dwTaskID));
		
		// cleanup
		dc.SelectObject(pOld);
	
		CPoint pt(::GetMessagePos());
		m_list.ScreenToClient(&pt);
		
		if (rTitle.PtInRect(pt))
		{
			//fabio_2005
#if _MSC_VER >= 1400
			_tcsncpy_s(pLVGIT->pszText, pLVGIT->cchTextMax, FormatInfoTip(dwTaskID), pLVGIT->cchTextMax);
#else
			_tcsncpy(pLVGIT->pszText, FormatInfoTip(dwTaskID), pLVGIT->cchTextMax);
#endif
		}
	}
}

void CTabbedToDoCtrl::UpdateSelectedTaskPath()
{
	CToDoCtrl::UpdateSelectedTaskPath();

	// extra processing
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		// handled above
		break;

	case FTCV_TASKLIST:
		// redraw the client column header
		m_list.RedrawHeaderColumn(GetListColumnIndex(TDCC_CLIENT));
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
}

void CTabbedToDoCtrl::SaveSortState(CPreferences& prefs)
{
	// ignore this if we have no tasks
	if (GetTaskCount() == 0)
		return;

	// create a new key using the filepath
	ASSERT (GetSafeHwnd());
	
	CString sKey = GetPreferencesKey(_T("SortColState"));
	
	if (!sKey.IsEmpty())
	{
		VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);

		prefs.WriteProfileInt(sKey, _T("ListMulti"), pLVData->bMultiSort);
		prefs.WriteProfileInt(sKey, _T("ListColumn"), pLVData->sort.nBy1);
		prefs.WriteProfileInt(sKey, _T("ListColumn2"), pLVData->sort.nBy2);
		prefs.WriteProfileInt(sKey, _T("ListColumn3"), pLVData->sort.nBy3);
		prefs.WriteProfileInt(sKey, _T("ListAscending"), pLVData->sort.bAscending1);
		prefs.WriteProfileInt(sKey, _T("ListAscending2"), pLVData->sort.bAscending2);
		prefs.WriteProfileInt(sKey, _T("ListAscending3"), pLVData->sort.bAscending3);
	}

	// base class
	CToDoCtrl::SaveSortState(prefs);
}

void CTabbedToDoCtrl::LoadSortState(const CPreferences& prefs, const CString& sFilePath)
{
	CString sKey = GetPreferencesKey(_T("SortColState"), sFilePath);
	VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);
	
	if (!sKey.IsEmpty() && prefs.HasSection(sKey))
	{
		pLVData->bMultiSort = prefs.GetProfileInt(sKey, _T("ListMulti"), FALSE);
		pLVData->sort.nBy1 = (TDC_COLUMN)prefs.GetProfileInt(sKey, _T("ListColumn"), TDCC_NONE);
		pLVData->sort.nBy2 = (TDC_COLUMN)prefs.GetProfileInt(sKey, _T("ListColumn2"), TDCC_NONE);
		pLVData->sort.nBy3 = (TDC_COLUMN)prefs.GetProfileInt(sKey, _T("ListColumn3"), TDCC_NONE);
		pLVData->sort.bAscending1 = prefs.GetProfileInt(sKey, _T("ListAscending"), TRUE);
		pLVData->sort.bAscending2 = prefs.GetProfileInt(sKey, _T("ListAscending2"), TRUE);
		pLVData->sort.bAscending3 = prefs.GetProfileInt(sKey, _T("ListAscending3"), TRUE);
	}
	else // backward compatibility
	{
		sKey = GetPreferencesKey(_T("SortState"), sFilePath);
		
		if (!sKey.IsEmpty() && prefs.HasSection(sKey))
		{
			pLVData->bMultiSort = prefs.GetProfileInt(sKey, _T("Multi"), FALSE);

			int nSortBy = prefs.GetProfileInt(sKey, _T("Column"), 0);
			pLVData->sort.nBy1 = MapSortByToColumn(nSortBy);

			nSortBy = prefs.GetProfileInt(sKey, _T("Column2"), 0);
			pLVData->sort.nBy2 = MapSortByToColumn(nSortBy);

			nSortBy = prefs.GetProfileInt(sKey, _T("Column3"), 0);
			pLVData->sort.nBy3 = MapSortByToColumn(nSortBy);

			pLVData->sort.bAscending1 = prefs.GetProfileInt(sKey, _T("Ascending"), TRUE);
			pLVData->sort.bAscending2 = prefs.GetProfileInt(sKey, _T("Ascending2"), TRUE);
			pLVData->sort.bAscending3 = prefs.GetProfileInt(sKey, _T("Ascending3"), TRUE);
		}
	}
	pLVData->bNeedResort = (pLVData->sort.nBy1 != TDCC_NONE);

	CToDoCtrl::LoadSortState(prefs, sFilePath);
}


void CTabbedToDoCtrl::RedrawReminders() const
{ 
	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
	case FTCV_UNSET:
		CToDoCtrl::RedrawReminders();
		break;
		
	case FTCV_TASKLIST:
		if (IsColumnShowing(TDCC_REMINDER))
		{
			CListCtrl* pList = const_cast<CTDCListView*>(&m_list);
			pList->Invalidate(FALSE);
		}
		break;

	case FTCV_UIEXTENSION1:
	case FTCV_UIEXTENSION2:
	case FTCV_UIEXTENSION3:
	case FTCV_UIEXTENSION4:
	case FTCV_UIEXTENSION5:
	case FTCV_UIEXTENSION6:
	case FTCV_UIEXTENSION7:
	case FTCV_UIEXTENSION8:
	case FTCV_UIEXTENSION9:
	case FTCV_UIEXTENSION10:
	case FTCV_UIEXTENSION11:
	case FTCV_UIEXTENSION12:
	case FTCV_UIEXTENSION13:
	case FTCV_UIEXTENSION14:
	case FTCV_UIEXTENSION15:
	case FTCV_UIEXTENSION16:
		break;

	default:
		ASSERT(0);
	}
}

BOOL CTabbedToDoCtrl::IsViewSet() const 
{ 
	return (GetView() != FTCV_UNSET); 
}

BOOL CTabbedToDoCtrl::InListView() const 
{ 
	return (GetView() == FTCV_TASKLIST); 
}

BOOL CTabbedToDoCtrl::InTreeView() const 
{ 
	return (GetView() == FTCV_TASKTREE || !IsViewSet()); 
}

BOOL CTabbedToDoCtrl::InExtensionView() const
{
	return IsExtensionView(GetView());
}

BOOL CTabbedToDoCtrl::IsExtensionView(FTC_VIEW nView)
{
	return (nView >= FTCV_UIEXTENSION1 && nView <= FTCV_UIEXTENSION16);
}

BOOL CTabbedToDoCtrl::HasAnyExtensionViews() const
{
	int nView = m_aExtViews.GetSize();

	while (nView--)
	{
		if (m_aExtViews[nView] != NULL)
			return TRUE;
	}

	// else
	return FALSE;
}
