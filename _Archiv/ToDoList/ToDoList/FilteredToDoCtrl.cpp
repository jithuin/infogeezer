// Fi M_BlISlteredToDoCtrl.cpp: implementation of the CFilteredToDoCtrl class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "FilteredToDoCtrl.h"
#include "todoitem.h"
#include "resource.h"
#include "tdcstatic.h"
#include "tdcmsg.h"
#include "TDCCustomAttributeHelper.h"
#include "TDCSearchParamHelper.h"

#include "..\shared\holdredraw.h"
#include "..\shared\datehelper.h"
#include "..\shared\enstring.h"
#include "..\shared\preferences.h"
#include "..\shared\deferwndmove.h"
#include "..\shared\autoflag.h"
#include "..\shared\holdredraw.h"
#include "..\shared\osversion.h"
#include "..\shared\graphicsmisc.h"
#include "..\shared\savefocus.h"
#include "..\shared\IUIExtension.h"
#include "..\shared\filemisc.h"

#include <math.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#ifndef LVS_EX_DOUBLEBUFFER
#define LVS_EX_DOUBLEBUFFER 0x00010000
#endif

#ifndef LVS_EX_LABELTIP
#define LVS_EX_LABELTIP     0x00004000
#endif

const UINT SORTWIDTH = 10;
#define NOCOLOR ((COLORREF)-1)

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CFilteredToDoCtrl::CFilteredToDoCtrl(CContentMgr& mgr, const CONTENTFORMAT& cfDefault) :
	CTabbedToDoCtrl(mgr, cfDefault), 
		m_bLastFilterWasCustom(FALSE),
		m_bCustomFilter(FALSE)
{
}

CFilteredToDoCtrl::~CFilteredToDoCtrl()
{

}

BEGIN_MESSAGE_MAP(CFilteredToDoCtrl, CTabbedToDoCtrl)
//{{AFX_MSG_MAP(CFilteredToDoCtrl)
	ON_WM_DESTROY()
	//}}AFX_MSG_MAP
	ON_REGISTERED_MESSAGE(WM_TDCN_VIEWPRECHANGE, OnPreTabViewChange)
	ON_NOTIFY(TVN_ITEMEXPANDED, IDC_TASKLIST, OnTreeExpandItem)
	ON_MESSAGE(WM_TDC_REFRESHFILTER, OnRefreshFilter)
	ON_CBN_EDITCHANGE(IDC_DUETIME, OnEditChangeDueTime)
END_MESSAGE_MAP()

///////////////////////////////////////////////////////////////////////////

BOOL CFilteredToDoCtrl::OnInitDialog()
{
	CTabbedToDoCtrl::OnInitDialog();

	return FALSE;
}

BOOL CFilteredToDoCtrl::LoadTasks(const CTaskFile& file)
{
	BOOL bViewWasUnset = (GetView() == FTCV_UNSET);

	if (!CTabbedToDoCtrl::LoadTasks(file))
		return FALSE;

	// save visible state
	BOOL bHidden = !IsWindowVisible();

	// reload last view
	if (bViewWasUnset)
	{
		LoadSettings();

		// always refresh the tree filter because all other
		// views depend on it
		if (IsFilterSet(FTCV_TASKTREE))
			RefreshTreeFilter(); // always

		// handle other views
		FTC_VIEW nView = GetView();

		switch (nView)
		{
		case FTCV_TASKLIST:
			if (IsFilterSet(nView))
			{
				RefreshListFilter();
			}
			else if (!GetPreferencesKey().IsEmpty()) // first time
			{
				VIEWDATA2* pData = GetViewData2(nView);
				pData->bNeedRefilter = TRUE;
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
			RefreshExtensionFilter(nView);
			break;
		}
	}
	else if (IsFilterSet(GetView()))
		RefreshFilter();

	// restore previously visibility
	if (bHidden)
		ShowWindow(SW_HIDE);

	return TRUE;
}

BOOL CFilteredToDoCtrl::DelayLoad(const CString& sFilePath, COleDateTime& dtEarliestDue)
{
	if (CTabbedToDoCtrl::DelayLoad(sFilePath, dtEarliestDue))
	{
		LoadSettings();
		return TRUE;
	}
	
	// else
	return FALSE;
}

void CFilteredToDoCtrl::SaveSettings() const
{
	CPreferences prefs;

	SaveFilter(m_filter, prefs);
	SaveFilter(m_lastFilter, prefs, _T("Last"));
	
	// record if we had a custom filter
	CString sKey = GetPreferencesKey(_T("Filter"));

	if (!sKey.IsEmpty())
	{
		prefs.WriteProfileInt(sKey, _T("CustomFilter"), m_bCustomFilter);
		prefs.WriteProfileInt(sKey, _T("LastFilterWasCustom"), m_bLastFilterWasCustom);

		SaveCustomFilter(m_sCustomFilter, m_customFilter, prefs);
		SaveCustomFilter(m_sCustomFilter, m_lastCustomFilter, prefs, _T("Last"));
	}
}

void CFilteredToDoCtrl::LoadSettings()
{
	if (HasStyle(TDCS_RESTOREFILTERS))
	{
		CPreferences prefs;

		// restore in reverse order
		LoadFilter(prefs, m_lastFilter, _T("Last"));
		LoadFilter(prefs, m_filter);
		
		// record if we had a custom filter
		CString sKey = GetPreferencesKey(_T("Filter"));
		
		if (!sKey.IsEmpty())
		{
			m_bCustomFilter = prefs.GetProfileInt(sKey, _T("CustomFilter"));
			m_bLastFilterWasCustom = prefs.GetProfileInt(sKey, _T("LastFilterWasCustom"));
			
			// restore in reverse order
			LoadCustomFilter(prefs, m_sCustomFilter, m_lastCustomFilter, _T("Last"));
			LoadCustomFilter(prefs, m_sCustomFilter, m_customFilter);
		}
	}
}

void CFilteredToDoCtrl::LoadCustomFilter(const CPreferences& prefs, CString& sFilter, SEARCHPARAMS& params, const CString& sSubKey)
{
	CString sKey = GetPreferencesKey(_T("Filter\\Custom"));

	if (!sKey.IsEmpty())
	{
		if (!sSubKey.IsEmpty())
			sKey += "\\" + sSubKey;

		sFilter = prefs.GetProfileString(sKey, _T("Name"));

		// reset
		params.Clear();

		// load
		params.bIgnoreDone = prefs.GetProfileInt(sKey, _T("IgnoreDone"), FALSE);
		params.bIgnoreOverDue = prefs.GetProfileInt(sKey, _T("IgnoreOverDue"), FALSE);
		params.bWantAllSubtasks = prefs.GetProfileInt(sKey, _T("WantAllSubtasks"), FALSE);

		int nNumRules = prefs.GetProfileInt(sKey, _T("NumRules"), 0), nNumDupes = 0;
		SEARCHPARAM rule, rulePrev;

		for (int nRule = 0; nRule < nNumRules; nRule++)
		{
			CString sRule = Misc::MakeKey(_T("Rule%d"), nRule, sKey);

			// stop loading if we meet an invalid rule
			if (!CTDCSearchParamHelper::LoadRule(prefs, sRule, m_aCustomAttribDefs, rule))
				break;

			// stop loading if we get more than 2 duplicates in a row.
			// this handles a bug resulting in an uncontrolled 
			// proliferation of duplicates that I cannot reproduce
			if (rule == rulePrev)
			{
				nNumDupes++;

				if (nNumDupes > 2)
					break;
			}
			else
			{
				nNumDupes = 0; // reset
				rulePrev = rule; // new 'previous rule
			}

			params.aRules.Add(rule);
		}
	}
}

void CFilteredToDoCtrl::SaveCustomFilter(const CString& sFilter, const SEARCHPARAMS& params, CPreferences& prefs, const CString& sSubKey) const
{
	// check anything to save
	if (params.aRules.GetSize() == 0)
		return;

	CString sKey = GetPreferencesKey(_T("Filter\\Custom"));

	if (!sKey.IsEmpty())
	{
		if (!sSubKey.IsEmpty())
			sKey += "\\" + sSubKey;

		// delete existing filter
		prefs.DeleteSection(sKey, TRUE);

		prefs.WriteProfileString(sKey, _T("Name"), sFilter);
		prefs.WriteProfileInt(sKey, _T("IgnoreDone"), params.bIgnoreDone);
		prefs.WriteProfileInt(sKey, _T("IgnoreOverDue"), params.bIgnoreOverDue);
		prefs.WriteProfileInt(sKey, _T("WantAllSubtasks"), params.bWantAllSubtasks);

		int nNumRules = 0, nNumDupes = 0;
		SEARCHPARAM rulePrev;

		for (int nRule = 0; nRule < params.aRules.GetSize(); nRule++)
		{
			CString sRule = Misc::MakeKey(_T("Rule%d"), nRule, sKey);
			const SEARCHPARAM& rule = params.aRules[nRule];

			// stop saving if we meet an invalid rule
			if (!CTDCSearchParamHelper::SaveRule(prefs, sRule, rule))
				break;

			// stop saving if we get more than 2 duplicates in a row.
			// this handles a bug resulting in an uncontrolled 
			// proliferation of duplicates that I cannot reproduce
			if (rule == rulePrev)
			{
				nNumDupes++;

				if (nNumDupes > 2)
					break;
			}
			else
			{
				nNumDupes = 0; // reset
				rulePrev = rule; // new 'previous rule
			}

			nNumRules++;
		}
		
		prefs.WriteProfileInt(sKey, _T("NumRules"), nNumRules);
	}
}

void CFilteredToDoCtrl::LoadFilter(const CPreferences& prefs, FTDCFILTER& filter, const CString& sSubKey)
{
	CString sKey = GetPreferencesKey(_T("Filter"));

	if (!sKey.IsEmpty())
	{
		if (!sSubKey.IsEmpty())
			sKey += "\\" + sSubKey;

		// backwards compatibility
		int nShow = prefs.GetProfileInt(sKey, _T("Filter"), FS_ALL);
		int nStartBy = FD_ANY, nDueBy = FD_ANY;

		if (nShow != FS_CUSTOM && nShow < FS_ALL)
		{
			enum 
			{
				FT_ALL,
				FT_NOTDONE,
				FT_DONE, 
				FT_DUETODAY,
				FT_DUETOMORROW,
				FT_DUEENDTHISWEEK, 
				FT_DUEENDNEXTWEEK, 
				FT_DUEENDTHISMONTH,
				FT_DUEENDNEXTMONTH,
				FT_DUEENDTHISYEAR,
				FT_DUEENDNEXTYEAR,
				FT_STARTTODAY,
				FT_STARTENDTHISWEEK, 
				FT_FLAGGED, 
				FT_DUENEXTSEVENDAYS,
				FT_STARTNEXTSEVENDAYS,
				FT_STARTTOMORROW,
				FT_STARTENDNEXTWEEK, 
				FT_STARTENDTHISMONTH,
				FT_STARTENDNEXTMONTH,
				FT_STARTENDTHISYEAR, 
				FT_STARTENDNEXTYEAR,
				FT_SELECTED,
			};

			switch (nShow)
			{
				case FT_ALL:
					nShow = FS_ALL;
					break;

				case FT_NOTDONE:
					nShow = FS_NOTDONE;
					break;
					
				case FT_DONE: 
					nShow = FS_DONE;
					break;
					
				case FT_FLAGGED: 
					nShow = FS_FLAGGED;
					break;

				case FT_SELECTED:
					nShow = FS_SELECTED;
					break;
					
				///////////////////////////////
					
				case FT_DUETODAY:
					nShow = FS_ALL;
					nDueBy = FD_TODAY;
					break;
					
				case FT_DUETOMORROW:
					nShow = FS_ALL;
					nDueBy = FD_TOMORROW;
					break;
					
				case FT_DUEENDTHISWEEK:
					nShow = FS_ALL;
					nDueBy = FD_ENDTHISWEEK;
					break;
					
				case FT_DUEENDNEXTWEEK: 
					nShow = FS_ALL;
					nDueBy = FD_ENDNEXTWEEK;
					break;
					
				case FT_DUEENDTHISMONTH:
					nShow = FS_ALL;
					nDueBy = FD_ENDTHISMONTH;
					break;
					
				case FT_DUEENDNEXTMONTH:
					nShow = FS_ALL;
					nDueBy = FD_ENDNEXTMONTH;
					break;
					
				case FT_DUEENDTHISYEAR:
					nShow = FS_ALL;
					nDueBy = FD_ENDTHISYEAR;
					break;
					
				case FT_DUEENDNEXTYEAR:
					nShow = FS_ALL;
					nDueBy = FD_ENDNEXTYEAR;
					break;
			
				case FT_DUENEXTSEVENDAYS:
					nShow = FS_ALL;
					nDueBy = FD_NEXTSEVENDAYS;
					break;
					
				///////////////////////////////
					
				case FT_STARTTODAY:
					nShow = FS_ALL;
					nStartBy = FD_TODAY;
					break;
					
				case FT_STARTTOMORROW:
					nShow = FS_ALL;
					nStartBy = FD_TOMORROW;
					break;
					
				case FT_STARTENDTHISWEEK: 
					nShow = FS_ALL;
					nStartBy = FD_ENDTHISWEEK;
					break;
					
				case FT_STARTNEXTSEVENDAYS:
					nShow = FS_ALL;
					nStartBy = FD_NEXTSEVENDAYS;
					break;
					
				case FT_STARTENDNEXTWEEK: 
					nShow = FS_ALL;
					nStartBy = FD_ENDNEXTWEEK;
					break;
					
				case FT_STARTENDTHISMONTH:
					nShow = FS_ALL;
					nStartBy = FD_ENDTHISMONTH;
					break;
					
				case FT_STARTENDNEXTMONTH:
					nShow = FS_ALL;
					nStartBy = FD_ENDNEXTMONTH;
					break;
					
				case FT_STARTENDTHISYEAR: 
					nShow = FS_ALL;
					nStartBy = FD_ENDTHISYEAR;
					break;
					
				case FT_STARTENDNEXTYEAR:
					nShow = FS_ALL;
					nStartBy = FD_ENDNEXTYEAR;
					break;
					
				default:
					ASSERT(0);
			}
		}
		else
		{
			nStartBy = prefs.GetProfileInt(sKey, _T("Start"), FD_ANY);
			nDueBy = prefs.GetProfileInt(sKey, _T("Due"), FD_ANY);
		}

		filter.nShow = (FILTER_SHOW)nShow;
		filter.nStartBy = (FILTER_DATE)nStartBy;
		filter.nDueBy = (FILTER_DATE)nDueBy;

		filter.sTitle = prefs.GetProfileString(sKey, _T("Title"));
		filter.nPriority = prefs.GetProfileInt(sKey, _T("Priority"), FM_ANYPRIORITY);
		filter.nRisk = prefs.GetProfileInt(sKey, _T("Risk"), FM_ANYRISK);

		// cats
		CString sCategory = prefs.GetProfileString(sKey, _T("Category"));
		Misc::Split(sCategory, filter.aCategories, TRUE);

		// alloc to
		CString sAllocTo = prefs.GetProfileString(sKey, _T("AllocTo"));
		Misc::Split(sAllocTo, filter.aAllocTo, TRUE);

		// alloc by
		CString sAllocBy = prefs.GetProfileString(sKey, _T("AllocBy"));
		Misc::Split(sAllocBy, filter.aAllocBy, TRUE);

		// status
		CString sStatus = prefs.GetProfileString(sKey, _T("Status"));
		Misc::Split(sStatus, filter.aStatus, TRUE);

		// version
		CString sVersion = prefs.GetProfileString(sKey, _T("Version"));
		Misc::Split(sVersion, filter.aVersions, TRUE);

		// tags
		CString sTags = prefs.GetProfileString(sKey, _T("Tags"));
		Misc::Split(sTags, filter.aTags, TRUE);

		// options
		filter.SetFlag(FO_ANYCATEGORY, prefs.GetProfileInt(sKey, _T("AnyCategory"), TRUE));
		filter.SetFlag(FO_ANYALLOCTO, prefs.GetProfileInt(sKey, _T("AnyAllocTo"), TRUE));
		filter.SetFlag(FO_ANYTAG, prefs.GetProfileInt(sKey, _T("AnyTag"), TRUE));
		filter.SetFlag(FO_HIDEPARENTS, prefs.GetProfileInt(sKey, _T("HideParents"), FALSE));
		filter.SetFlag(FO_HIDEOVERDUE, prefs.GetProfileInt(sKey, _T("HideOverDue"), FALSE));
		filter.SetFlag(FO_HIDEDONE, prefs.GetProfileInt(sKey, _T("HideDone"), FALSE));
		filter.SetFlag(FO_HIDECOLLAPSED, prefs.GetProfileInt(sKey, _T("HideCollapsed"), FALSE));
		filter.SetFlag(FO_SHOWALLSUB, prefs.GetProfileInt(sKey, _T("ShowAllSubtasks"), FALSE));
	}
}

void CFilteredToDoCtrl::SaveFilter(const FTDCFILTER& filter, CPreferences& prefs, const CString& sSubKey) const
{
	CString sKey = GetPreferencesKey(_T("Filter"));

	if (!sKey.IsEmpty())
	{
		if (!sSubKey.IsEmpty())
			sKey += _T("\\") + sSubKey;

		prefs.WriteProfileInt(sKey, _T("Filter"), filter.nShow);
		prefs.WriteProfileInt(sKey, _T("Start"), filter.nStartBy);
		prefs.WriteProfileInt(sKey, _T("Due"), filter.nDueBy);
		prefs.WriteProfileString(sKey, _T("Title"), filter.sTitle);
		prefs.WriteProfileInt(sKey, _T("Priority"), filter.nPriority);
		prefs.WriteProfileInt(sKey, _T("Risk"), filter.nRisk);
		prefs.WriteProfileString(sKey, _T("AllocBy"), Misc::FormatArray(filter.aAllocBy));
		prefs.WriteProfileString(sKey, _T("Status"), Misc::FormatArray(filter.aStatus));
		prefs.WriteProfileString(sKey, _T("Version"), Misc::FormatArray(filter.aVersions));
		prefs.WriteProfileString(sKey, _T("AllocTo"), Misc::FormatArray(filter.aAllocTo));
		prefs.WriteProfileString(sKey, _T("Category"), Misc::FormatArray(filter.aCategories));
		prefs.WriteProfileString(sKey, _T("Tags"), Misc::FormatArray(filter.aTags));

		// options
		prefs.WriteProfileInt(sKey, _T("AnyAllocTo"), filter.HasFlag(FO_ANYALLOCTO));
		prefs.WriteProfileInt(sKey, _T("AnyCategory"), filter.HasFlag(FO_ANYCATEGORY));
		prefs.WriteProfileInt(sKey, _T("AnyTag"), filter.HasFlag(FO_ANYTAG));
		prefs.WriteProfileInt(sKey, _T("HideParents"), filter.HasFlag(FO_HIDEPARENTS));
		prefs.WriteProfileInt(sKey, _T("HideOverDue"), filter.HasFlag(FO_HIDEOVERDUE));
		prefs.WriteProfileInt(sKey, _T("HideDone"), filter.HasFlag(FO_HIDEDONE));
		prefs.WriteProfileInt(sKey, _T("HideCollapsed"), filter.HasFlag(FO_HIDECOLLAPSED));
		prefs.WriteProfileInt(sKey, _T("ShowAllSubtasks"), filter.HasFlag(FO_SHOWALLSUB));
	}
}

void CFilteredToDoCtrl::OnDestroy() 
{
	SaveSettings();

	CTabbedToDoCtrl::OnDestroy();
}

void CFilteredToDoCtrl::OnEditChangeDueTime()
{
	// need some special hackery to prevent a re-filter in the middle
	// of the user manually typing into the time field
	BOOL bNeedsRefilter = ModNeedsRefilter(TDCA_DUEDATE, FTCV_TASKTREE, GetSelectedTaskID());
	
	if (bNeedsRefilter)
		SetStyle(TDCS_REFILTERONMODIFY, FALSE, FALSE);
	
	CTabbedToDoCtrl::OnSelChangeDueTime();
	
	if (bNeedsRefilter)
		SetStyle(TDCS_REFILTERONMODIFY, TRUE, FALSE);
}

void CFilteredToDoCtrl::OnTreeExpandItem(NMHDR* /*pNMHDR*/, LRESULT* /*pResult*/)
{
	if (m_filter.HasFlag(FO_HIDECOLLAPSED))
	{
		if (InListView())
			RefreshListFilter();
		else
		{
			VIEWDATA2* pLVData = GetViewData2(FTCV_TASKLIST);
			pLVData->bNeedRefilter = TRUE;
		}
	}
}

LRESULT CFilteredToDoCtrl::OnPreTabViewChange(WPARAM nOldView, LPARAM nNewView) 
{
	if (nNewView != FTCV_TASKTREE)
	{
		VIEWDATA2* pData = GetViewData2((FTC_VIEW)nNewView);
		ASSERT(pData);

		BOOL bFiltered = FALSE;

		// take a note of what task is currently singly selected
		// so that we can prevent unnecessary calls to UpdateControls
		DWORD dwSelTaskID = GetSingleSelectedTaskID();

		switch (nNewView)
		{
		case FTCV_TASKLIST:
			// update filter as required
			if (pData->bNeedRefilter)
			{
				bFiltered = TRUE;
				RefreshListFilter();
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
			// update filter as required
			if (pData->bNeedRefilter)
			{
				// initialise progress depending on whether extension
				// window is already created
				UINT nProgressMsg = 0;

				if (GetExtensionWnd((FTC_VIEW)nNewView, FALSE) == NULL)
					nProgressMsg = IDS_INITIALISINGTABBEDVIEW;

				BeginExtensionProgress(pData, nProgressMsg);
				RefreshExtensionFilter((FTC_VIEW)nNewView);

				bFiltered = TRUE;
			}
			break;
		}
		
		// update controls only if the selection has changed and 
		// we didn't refilter (RefreshFilter will already have called UpdateControls)
		BOOL bSelChange = HasSingleSelectionChanged(dwSelTaskID);
		
		if (bSelChange && !bFiltered)
			UpdateControls();
	}

	return CTabbedToDoCtrl::OnPreTabViewChange(nOldView, nNewView);
}

BOOL CFilteredToDoCtrl::AddTasksToTree(const CTaskFile& tasks, HTREEITEM htiDest, HTREEITEM htiDestAfter, TDC_RESETIDS nResetID)
{
	if (!CTabbedToDoCtrl::AddTasksToTree(tasks, htiDest, htiDestAfter, nResetID))
		return FALSE;

	if (HasFilter() || HasCustomFilter())
	{
		FTC_VIEW nView = GetView();
		
		switch (nView)
		{
		case FTCV_TASKTREE:
			// mark all other views as needing refiltering
			SetListNeedRefilter(TRUE);
			SetExtensionsNeedRefilter(TRUE);
			break;
			
		case FTCV_TASKLIST:
			// mark extensions as needing refiltering
			RefreshListFilter();
			SetExtensionsNeedRefilter(TRUE);
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
			SetExtensionsNeedRefilter(TRUE);
			SetListNeedRefilter(TRUE);
			RefreshExtensionFilter(nView);
			break;
		}
	}

	return TRUE;
}

BOOL CFilteredToDoCtrl::ArchiveDoneTasks(const CString& sFilePath, TDC_ARCHIVE nFlags, BOOL bRemoveFlagged)
{
	if (CTabbedToDoCtrl::ArchiveDoneTasks(sFilePath, nFlags, bRemoveFlagged))
	{
		if (InListView())
		{
			if (IsFilterSet(FTCV_TASKLIST))
				RefreshListFilter();
		}
		else if (IsFilterSet(FTCV_TASKTREE))
			RefreshTreeFilter();

		return TRUE;
	}

	// else
	return FALSE;
}

BOOL CFilteredToDoCtrl::ArchiveSelectedTasks(const CString& sFilePath, BOOL bRemove)
{
	if (CTabbedToDoCtrl::ArchiveSelectedTasks(sFilePath, bRemove))
	{
		if (InListView())
		{
			if (IsFilterSet(FTCV_TASKLIST))
				RefreshListFilter();
		}
		else if (IsFilterSet(FTCV_TASKTREE))
			RefreshTreeFilter();

		return TRUE;
	}

	// else
	return FALSE;
}

int CFilteredToDoCtrl::GetArchivableTasks(CTaskFile& tasks, BOOL bSelectedOnly) const
{
	if (bSelectedOnly || !IsFilterSet(FTCV_TASKTREE))
		return CTabbedToDoCtrl::GetArchivableTasks(tasks, bSelectedOnly);

	// else process the entire data hierarchy
	GetCompletedTasks(m_data.GetStructure(), tasks, NULL, FALSE);

	return tasks.GetTaskCount();
}

BOOL CFilteredToDoCtrl::RemoveArchivedTask(DWORD dwTaskID)
{
	ASSERT(m_data.GetTask(dwTaskID,FALSE));
	
	// note: if the tasks does not exist in the tree then this is not a bug
	// if a filter is set
	HTREEITEM hti = m_find.GetItem(dwTaskID);
	ASSERT(hti || IsFilterSet(FTCV_TASKTREE));
	
	if (!hti && !IsFilterSet(FTCV_TASKTREE))
		return FALSE;
	
	if (hti)
		m_tree.DeleteItem(hti);

	return m_data.DeleteTask(dwTaskID);
}

void CFilteredToDoCtrl::GetCompletedTasks(const TODOSTRUCTURE* pTDS, CTaskFile& tasks, HTASKITEM hTaskParent, BOOL bSelectedOnly) const
{
	const TODOITEM* pTDI = NULL;

	if (!pTDS->IsRoot())
	{
		DWORD dwTaskID = pTDS->GetTaskID();

		pTDI = m_data.GetTask(dwTaskID);
		ASSERT(pTDI);

		if (!pTDI)
			return;

		// we add the task if it is completed (and optionally selected) or it has children
		if (pTDI->IsDone() || pTDS->HasSubTasks())
		{
			HTASKITEM hTask = tasks.NewTask(_T(""), hTaskParent, dwTaskID);
			ASSERT(hTask);

			// copy attributes
			TDCGETTASKS allTasks;
			SetTaskAttributes(pTDI, pTDS, tasks, hTask, allTasks, FALSE);

			// this task is now the new parent
			hTaskParent = hTask;
		}
	}

	// children
	if (pTDS->HasSubTasks())
	{
		for (int nSubtask = 0; nSubtask < pTDS->GetSubTaskCount(); nSubtask++)
		{
			const TODOSTRUCTURE* pTDSChild = pTDS->GetSubTask(nSubtask);
			GetCompletedTasks(pTDSChild, tasks, hTaskParent, bSelectedOnly); // RECURSIVE call
		}

		// if no subtasks were added and the parent is not completed 
		// (and optionally selected) then we remove it
		if (hTaskParent && tasks.GetFirstTask(hTaskParent) == NULL)
		{
			ASSERT(pTDI);

			if (pTDI && !pTDI->IsDone())
				tasks.DeleteTask(hTaskParent);
		}
	}
}

int CFilteredToDoCtrl::GetFilteredTasks(CTaskFile& tasks, const TDCGETTASKS& filter) const
{
	// synonym for GetTasks which always returns the filtered tasks
	return GetTasks(tasks, filter);
}

FILTER_SHOW CFilteredToDoCtrl::GetFilter(FTDCFILTER& filter) const
{
	filter = m_filter;
	return m_filter.nShow;
}

void CFilteredToDoCtrl::SetFilter(const FTDCFILTER& filter)
{
	FTDCFILTER fPrev = m_filter;
	m_filter = filter;

	FTC_VIEW nView = GetView();

	if (m_bDelayLoaded)
	{
		// mark everything needing refilter
		GetViewData2(FTCV_TASKTREE)->bNeedRefilter = TRUE;
		SetListNeedRefilter(TRUE);
		SetExtensionsNeedRefilter(TRUE);
	}
	else
	{
		BOOL bTreeNeedsFilter = m_bCustomFilter || !FiltersMatch(fPrev, filter, FTCV_TASKTREE);
		BOOL bViewNeedRefilter = m_bCustomFilter || !FiltersMatch(fPrev, filter, nView); 

		m_bCustomFilter = FALSE;

		if (bTreeNeedsFilter)
		{
			// this will mark all other views as needing refiltering
			RefreshFilter();
		}
		else if (bViewNeedRefilter)
		{
			// mark everything as needing updating then update what we need
			SetListNeedRefilter(TRUE);
			SetExtensionsNeedRefilter(TRUE);

			switch (nView)
			{
			case FTCV_TASKTREE:
			case FTCV_UNSET:
				break; // handled above

			case FTCV_TASKLIST:
				RefreshListFilter();
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
				RefreshExtensionFilter(nView);
				break;

			default:
				ASSERT(0);
			}
		}
	}

	// modify the tree prompt depending on whether there is a filter set
	if (IsFilterSet(FTCV_TASKTREE))
		m_mgrPrompts.SetPrompt(m_tree, IDS_TDC_FILTEREDTASKLISTPROMPT, TVM_GETCOUNT);
	else
		m_mgrPrompts.SetPrompt(m_tree, IDS_TDC_TASKLISTPROMPT, TVM_GETCOUNT);
}
	
void CFilteredToDoCtrl::ClearFilter()
{
	if (HasFilter() || HasCustomFilter())
	{
		FTDCFILTER filter; // empty filter
		SetFilter(filter);
		m_lastFilter = filter; // clear

		m_customFilter = SEARCHPARAMS(); // empty ruleset
		m_bCustomFilter = FALSE;
		m_bLastFilterWasCustom = FALSE;
	}
}

void CFilteredToDoCtrl::ToggleFilter()
{
	if (HasFilter() || HasCustomFilter()) // turn off
	{
		// save last filter and clear
		// the order here is important because ClearFilter()
		// will also reset the last filter
		if (m_bCustomFilter)
		{
			SEARCHPARAMS temp = m_customFilter;
			ClearFilter();
			m_bLastFilterWasCustom = TRUE;
			m_lastCustomFilter = temp;
		}
		else
		{
			FTDCFILTER temp = m_filter;
			ClearFilter();
			m_lastFilter = temp; 
		}
	}
	else // restore
	{
		if (m_bLastFilterWasCustom)
			SetCustomFilter(m_lastCustomFilter, m_sCustomFilter);
		else
			SetFilter(m_lastFilter);
	}
}

BOOL CFilteredToDoCtrl::FiltersMatch(const FTDCFILTER& filter1, const FTDCFILTER& filter2, FTC_VIEW nView) const
{
	if (nView == FTCV_UNSET)
		return FALSE;

	DWORD dwIgnore = 0;

	if (nView == FTCV_TASKTREE)
		dwIgnore = (FO_HIDECOLLAPSED | FO_HIDEPARENTS);

	return FTDCFILTER::FiltersMatch(filter1, filter2, dwIgnore);
}

BOOL CFilteredToDoCtrl::IsFilterSet(FTC_VIEW nView) const
{
	return IsFilterSet(m_filter, nView) || HasCustomFilter();
}

BOOL CFilteredToDoCtrl::IsFilterSet(const FTDCFILTER& filter, FTC_VIEW nView) const
{
	if (nView == FTCV_UNSET)
		return FALSE;

	DWORD dwIgnore = 0;

	if (nView == FTCV_TASKTREE)
		dwIgnore = (FO_HIDECOLLAPSED | FO_HIDEPARENTS);

	return filter.IsSet(dwIgnore);
}

UINT CFilteredToDoCtrl::GetTaskCount(UINT* pVisible) const
{
	if (pVisible)
	{
		if (InListView())
			*pVisible = m_list.GetItemCount();
		else
			*pVisible = m_tree.GetCount();
	}

	return CTabbedToDoCtrl::GetTaskCount();
}

int CFilteredToDoCtrl::FindTasks(const SEARCHPARAMS& params, CResultArray& aResults) const
{
	if (params.bIgnoreFilteredOut)
		return CTabbedToDoCtrl::FindTasks(params, aResults);
	
	// else all tasks
	return m_data.FindTasks(params, aResults);
}

BOOL CFilteredToDoCtrl::HasCustomFilter() const 
{ 
	return m_bCustomFilter; 
}

CString CFilteredToDoCtrl::GetCustomFilterName() const 
{ 
	if (HasCustomFilter())
		return m_sCustomFilter; 

	// else
	return _T("");
}

BOOL CFilteredToDoCtrl::SetCustomFilter(const SEARCHPARAMS& params, LPCTSTR szName)
{
	m_customFilter = params;
	m_customFilter.aAttribDefs.Copy(m_aCustomAttribDefs);

	m_sCustomFilter = szName;

	RestoreCustomFilter();

	return TRUE;
}

BOOL CFilteredToDoCtrl::RestoreCustomFilter()
{
	m_bCustomFilter = TRUE;

	if (m_bDelayLoaded)
	{
		// mark everything needing refilter
		GetViewData2(FTCV_TASKTREE)->bNeedRefilter = TRUE;
		SetListNeedRefilter(TRUE);
		SetExtensionsNeedRefilter(TRUE);
	}
	else
	{
		RefreshFilter();
	}

	return TRUE;
}

void CFilteredToDoCtrl::BuildFilterQuery(SEARCHPARAMS& params) const
{
	if (m_bCustomFilter)
	{
		params = m_customFilter;
		params.aAttribDefs.Copy(m_aCustomAttribDefs);

		return;
	}

	// reset the search
	params.aRules.RemoveAll();
	params.bIgnoreDone = m_filter.HasFlag(FO_HIDEDONE);
	params.bWantAllSubtasks = m_filter.HasFlag(FO_SHOWALLSUB);
	
	// handle principle filter
	switch (m_filter.nShow)
	{
	case FS_ALL:
		break;

	case FS_DONE:
		params.aRules.Add(SEARCHPARAM(TDCA_DONEDATE, FO_SET));
		params.bIgnoreDone = FALSE;
		break;

	case FS_NOTDONE:
		params.aRules.Add(SEARCHPARAM(TDCA_DONEDATE, FO_NOT_SET));
		params.bIgnoreDone = TRUE;
		break;

	case FS_FLAGGED:
		params.aRules.Add(SEARCHPARAM(TDCA_FLAG, FO_SET));
		break;

	case FS_SELECTED:
		params.aRules.Add(SEARCHPARAM(TDCA_SELECTION, FO_SET));
		break;

	default:
		ASSERT(0); // to catch unimplemented filters
		break;
	}

	// handle start date filters
	COleDateTime dateStart;

	if (InitFilterDate(m_filter.nStartBy, dateStart))
	{
		params.aRules.Add(SEARCHPARAM(TDCA_STARTDATE, FO_ON_OR_BEFORE, dateStart.m_dt));
		params.aRules.Add(SEARCHPARAM(TDCA_DONEDATE, FO_NOT_SET));
	}
	else if (m_filter.nStartBy == FD_NONE)
	{
		params.aRules.Add(SEARCHPARAM(TDCA_STARTDATE, FO_NOT_SET));
		params.aRules.Add(SEARCHPARAM(TDCA_DONEDATE, FO_NOT_SET));
	}

	// handle due date filters
	COleDateTime dateDue;

	if (InitFilterDate(m_filter.nDueBy, dateDue))
	{
		params.aRules.Add(SEARCHPARAM(TDCA_DUEDATE, FO_ON_OR_BEFORE, dateDue.m_dt));
		params.aRules.Add(SEARCHPARAM(TDCA_DONEDATE, FO_NOT_SET));
			
		// this flag only applies to due filters
		params.bIgnoreOverDue = m_filter.HasFlag(FO_HIDEOVERDUE);
	}
	else if (m_filter.nDueBy == FD_NONE)
	{
		params.aRules.Add(SEARCHPARAM(TDCA_DUEDATE, FO_NOT_SET));
		params.aRules.Add(SEARCHPARAM(TDCA_DONEDATE, FO_NOT_SET));
	}

	// handle other attributes
	AddNonDateFilterQueryRules(params);
}

void CFilteredToDoCtrl::AddNonDateFilterQueryRules(SEARCHPARAMS& params) const
{
	// title text
	if (!m_filter.sTitle.IsEmpty())
	{
		switch (m_filter.nTitleOption)
		{
		case FT_FILTERONTITLECOMMENTS:
			params.aRules.Add(SEARCHPARAM(TDCA_TASKNAMEORCOMMENTS, FO_INCLUDES, m_filter.sTitle));
			break;
			
		case FT_FILTERONANYTEXT:
			params.aRules.Add(SEARCHPARAM(TDCA_ANYTEXTATTRIBUTE, FO_INCLUDES, m_filter.sTitle));
			break;
			
		case FT_FILTERONTITLEONLY:
		default:
			params.aRules.Add(SEARCHPARAM(TDCA_TASKNAME, FO_INCLUDES, m_filter.sTitle));
			break;
		}
	}

	// note: these are all 'AND' 
	// category
	if (m_filter.aCategories.GetSize())
	{
		CString sMatchBy = Misc::FormatArray(m_filter.aCategories);

		if (m_filter.aCategories.GetSize() == 1 && sMatchBy.IsEmpty())
			params.aRules.Add(SEARCHPARAM(TDCA_CATEGORY, FO_NOT_SET));

		else if (m_filter.dwFlags & FO_ANYCATEGORY)
			params.aRules.Add(SEARCHPARAM(TDCA_CATEGORY, FO_INCLUDES, sMatchBy));
		else
			params.aRules.Add(SEARCHPARAM(TDCA_CATEGORY, FO_EQUALS, sMatchBy));
	}

	// allocated to
	if (m_filter.aAllocTo.GetSize())
	{
		CString sMatchBy = Misc::FormatArray(m_filter.aAllocTo);

		if (m_filter.aAllocTo.GetSize() == 1 && sMatchBy.IsEmpty())
			params.aRules.Add(SEARCHPARAM(TDCA_ALLOCTO, FO_NOT_SET));

		else if (m_filter.dwFlags & FO_ANYALLOCTO)
			params.aRules.Add(SEARCHPARAM(TDCA_ALLOCTO, FO_INCLUDES, sMatchBy));
		else
			params.aRules.Add(SEARCHPARAM(TDCA_ALLOCTO, FO_EQUALS, sMatchBy));
	}

	// allocated by
	if (m_filter.aAllocBy.GetSize())
	{
		CString sMatchBy = Misc::FormatArray(m_filter.aAllocBy);

		if (m_filter.aAllocBy.GetSize() == 1 && sMatchBy.IsEmpty())
			params.aRules.Add(SEARCHPARAM(TDCA_ALLOCBY, FO_NOT_SET));
		else
			params.aRules.Add(SEARCHPARAM(TDCA_ALLOCBY, FO_INCLUDES, sMatchBy));
	}

	// status
	if (m_filter.aStatus.GetSize())
	{
		CString sMatchBy = Misc::FormatArray(m_filter.aStatus);

		if (m_filter.aStatus.GetSize() == 1 && sMatchBy.IsEmpty())
			params.aRules.Add(SEARCHPARAM(TDCA_STATUS, FO_NOT_SET));
		else
			params.aRules.Add(SEARCHPARAM(TDCA_STATUS, FO_INCLUDES, sMatchBy));
	}

	// version
	if (m_filter.aVersions.GetSize())
	{
		CString sMatchBy = Misc::FormatArray(m_filter.aVersions);

		if (m_filter.aVersions.GetSize() == 1 && sMatchBy.IsEmpty())
			params.aRules.Add(SEARCHPARAM(TDCA_VERSION, FO_NOT_SET));
		else
			params.aRules.Add(SEARCHPARAM(TDCA_VERSION, FO_INCLUDES, sMatchBy));
	}

	// tags
	if (m_filter.aTags.GetSize())
	{
		CString sMatchBy = Misc::FormatArray(m_filter.aTags);

		if (m_filter.aTags.GetSize() == 1 && sMatchBy.IsEmpty())
			params.aRules.Add(SEARCHPARAM(TDCA_TAGS, FO_NOT_SET));
		else
			params.aRules.Add(SEARCHPARAM(TDCA_TAGS, FO_INCLUDES, sMatchBy));
	}

	// priority
	if (m_filter.nPriority != FM_ANYPRIORITY)
	{
		if (m_filter.nPriority == FM_NOPRIORITY)
			params.aRules.Add(SEARCHPARAM(TDCA_PRIORITY, FO_NOT_SET));

		else if (m_filter.nPriority != FM_ANYPRIORITY)
			params.aRules.Add(SEARCHPARAM(TDCA_PRIORITY, FO_GREATER_OR_EQUAL, m_filter.nPriority));
	}

	// risk
	if (m_filter.nRisk != FM_ANYRISK)
	{
		if (m_filter.nRisk == FM_NORISK)
			params.aRules.Add(SEARCHPARAM(TDCA_RISK, FO_NOT_SET));
		
		else if (m_filter.nRisk != FM_ANYRISK)
			params.aRules.Add(SEARCHPARAM(TDCA_RISK, FO_GREATER_OR_EQUAL, m_filter.nRisk));
	}

	// special case: no aRules + ignore completed
	if ((params.bIgnoreDone) && params.aRules.GetSize() == 0)
		params.aRules.Add(SEARCHPARAM(TDCA_DONEDATE, FO_NOT_SET));
}

LRESULT CFilteredToDoCtrl::OnRefreshFilter(WPARAM wParam, LPARAM lParam)
{
	BOOL bUndo = lParam;
	FTC_VIEW nView = (FTC_VIEW)wParam;

	if (nView == FTCV_TASKTREE)
		RefreshFilter();

	// if undoing then we must also refresh the list filter because
	// otherwise ResyncListSelection will fail in the case where
	// we are undoing a delete because the undone item will not yet be in the list.
	if (nView == FTCV_TASKLIST || bUndo)
		RefreshListFilter();
	
   // resync selection?
   if (bUndo)
      ResyncListSelection();

	return 0L;
}

void CFilteredToDoCtrl::RefreshFilter() 
{
	CSaveFocus sf;

	RefreshTreeFilter(); // always

	FTC_VIEW nView = GetView();

	switch (nView)
	{
	case FTCV_TASKTREE:
		// mark all other views as needing refiltering
		SetListNeedRefilter(TRUE);
		SetExtensionsNeedRefilter(TRUE);
		break;

	case FTCV_TASKLIST:
		// mark extensions as needing refiltering
		RefreshListFilter();
		SetExtensionsNeedRefilter(TRUE);
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
		SetExtensionsNeedRefilter(TRUE);
		SetListNeedRefilter(TRUE);
		RefreshExtensionFilter(nView);
		break;
	}
}

void CFilteredToDoCtrl::SetListNeedRefilter(BOOL bRefilter)
{
	GetViewData2(FTCV_TASKLIST)->bNeedRefilter = bRefilter;
}

void CFilteredToDoCtrl::SetExtensionsNeedRefilter(BOOL bRefilter, FTC_VIEW nIgnore)
{
	for (int nExt = 0; nExt < m_aExtViews.GetSize(); nExt++)
	{
		FTC_VIEW nView = (FTC_VIEW)(FTCV_UIEXTENSION1 + nExt);

		if (nView == nIgnore)
			continue;

		// else
		VIEWDATA2* pVData2 = GetViewData2(nView);
		ASSERT(pVData2);

		pVData2->bNeedRefilter = bRefilter;
	}
}

void CFilteredToDoCtrl::RefreshTreeFilter() 
{
	if (!m_data.GetTaskCount())
		return;

	BOOL bTreeVis = !InListView();

	// save and reset current focus to work around a bug
	CSaveFocus sf;
	SetFocusToTasks();
	
	// cache current selection and task breadcrumbs before clearing selection
	TDCSELECTIONCACHE cache;
	CacheTreeSelection(cache);

	// grab the selected items for the filtering
	m_aSelectedTaskIDs.Copy(cache.aSelTaskIDs);
	
	Selection().RemoveAll();
	
	// also cache scrolled pos
	DWORD dwFirstVis = bTreeVis ? GetTaskID(m_tree.GetFirstVisibleItem()) : 0;
	
	// and expanded state
	CPreferences prefs;
	SaveExpandedState(prefs);
	
	CHoldRedraw hr(GetSafeHwnd());
	CWaitCursor cursor;
	
	// rebuild the tree
	// scope the redraw holding else we trigger a tree display
	// bug in Vista.
	{
		CHoldRedraw hr2(m_tree);
		RebuildTree();

		// redo last sort
		if (bTreeVis && IsSortable())
		{
			Resort();
			m_bTreeNeedResort = FALSE;
		}
		else
			m_bTreeNeedResort = TRUE;
		
		// restore expanded state
		LoadExpandedState(prefs);
			
		// restore scrolled pos
		if (dwFirstVis)
		{
			HTREEITEM hti = m_find.GetItem(dwFirstVis);
			m_tree.SelectSetFirstVisible(hti);
			m_tree.SelectItem(NULL);
		}
	}
	
	// restore selection
	RestoreTreeSelection(cache);
}

HTREEITEM CFilteredToDoCtrl::RebuildTree(const void* /*pContext*/)
{
	m_tree.TCH().SelectItem(NULL);
	m_tree.DeleteAllItems();

	// build a find query that matches the filter
	if (HasFilter() || HasCustomFilter())
	{
		SEARCHPARAMS filter;
		BuildFilterQuery(filter);

		return CTabbedToDoCtrl::RebuildTree(&filter);
	}

	// else
	return CTabbedToDoCtrl::RebuildTree(NULL);
}

BOOL CFilteredToDoCtrl::WantAddTask(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, const void* pContext) const
{
	BOOL bWantTask = CTabbedToDoCtrl::WantAddTask(pTDI, pTDS, pContext);
	
	if (bWantTask && pContext != NULL) // it's a filter
	{
		const SEARCHPARAMS* pFilter = static_cast<const SEARCHPARAMS*>(pContext);
		SEARCHRESULT result;
		
		// special case: selected item
		if (pFilter->HasAttribute(TDCA_SELECTION))
		{
			bWantTask = (Misc::Find(m_aSelectedTaskIDs, pTDS->GetTaskID()) != -1);

			// check parents
			if (!bWantTask && pFilter->bWantAllSubtasks)
			{
				TODOSTRUCTURE* pTDSParent = pTDS->GetParentTask();

				while (pTDSParent && !bWantTask)
				{
					bWantTask = (Misc::Find(m_aSelectedTaskIDs, pTDSParent->GetTaskID()) != -1);
					pTDSParent = pTDSParent->GetParentTask();
				}
			}
		}
		else // rest of attributes
			bWantTask = m_data.TaskMatches(pTDI, pTDS, *pFilter, result);

		if (bWantTask && pTDS->HasSubTasks())
		{
			// NOTE: the only condition under which this method is called for
			// a parent is if none of its subtasks matched the filter.
			//
			// So if we're a parent and match the filter we need to do an extra check
			// to see if what actually matched was the absence of attributes
			//
			// eg. if the parent category is "" and the filter rule is 
			// TDCA_CATEGORY is (FO_NOT_SET or FO_NOT_INCLUDES or FO_NOT_EQUAL) 
			// then we don't treat this as a match.
			//
			// The attributes to check are:
			//  Category
			//  Status
			//  Alloc To
			//  Alloc By
			//  Version
			//  Priority
			//  Risk
			//  Tags
			
			int nNumRules = pFilter->aRules.GetSize();
			
			for (int nRule = 0; nRule < nNumRules && bWantTask; nRule++)
			{
				const SEARCHPARAM& sp = pFilter->aRules[nRule];

				if (!sp.OperatorIs(FO_NOT_EQUALS) && 
					!sp.OperatorIs(FO_NOT_INCLUDES) && 
					!sp.OperatorIs(FO_NOT_SET))
				{
					continue;
				}
				
				// else check for empty parent attributes
				switch (sp.GetAttribute())
				{
				case TDCA_ALLOCTO:
					bWantTask = (pTDI->aAllocTo.GetSize() > 0);
					break;
					
				case TDCA_ALLOCBY:
					bWantTask = !pTDI->sAllocBy.IsEmpty();
					break;
					
				case TDCA_VERSION:
					bWantTask = !pTDI->sVersion.IsEmpty();
					break;
					
				case TDCA_STATUS:
					bWantTask = !pTDI->sStatus.IsEmpty();
					break;
					
				case TDCA_CATEGORY:
					bWantTask = (pTDI->aCategories.GetSize() > 0);
					break;
					
				case TDCA_TAGS:
					bWantTask = (pTDI->aTags.GetSize() > 0);
					break;
					
				case TDCA_PRIORITY:
					bWantTask = (pTDI->nPriority != FM_NOPRIORITY);
					break;
					
				case TDCA_RISK:
					bWantTask = (pTDI->nRisk != FM_NORISK);
					break;
				}
			}
		}
	}
	
	return bWantTask; 
}

void CFilteredToDoCtrl::RefreshExtensionFilter(FTC_VIEW nView)
{
	IUIExtensionWindow* pExtWnd = GetExtensionWnd(nView, TRUE);
	ASSERT(pExtWnd);

	if (pExtWnd)
	{
		// update view with filtered tasks
		CTaskFile tasks;
		GetFilteredTasks(tasks);
		UpdateExtensionView(pExtWnd, tasks, IUI_ALL);

		// and clear all update flags
		VIEWDATA2* pVData2 = GetViewData2(nView);

		pVData2->bNeedTaskUpdate = FALSE;
		pVData2->bNeedRefilter = FALSE;
	}
}

// base class override
void CFilteredToDoCtrl::RebuildList(const void* pContext)
{
	if (pContext)
		CTabbedToDoCtrl::RebuildList(pContext);
	else
		RefreshListFilter();
}

void CFilteredToDoCtrl::RefreshListFilter() 
{
	GetViewData2(FTCV_TASKLIST)->bNeedRefilter = FALSE;

	// build a find query that matches the filter
	SEARCHPARAMS filter;
	BuildFilterQuery(filter);

	// rebuild the list
	RebuildList(filter);
}

void CFilteredToDoCtrl::RebuildList(const SEARCHPARAMS& filter)
{
	// since the tree will have already got the items we want 
	// we can optimize the rebuild under certain circumstances
	// which are: 
	// 1. the list is sorted
	// 2. or the tree is unsorted
	// otherwise we need the data in it's unsorted state and the 
	// tree doesn't have it
	VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);

	if (IsSortable(pLVData->sort.nBy1) || !IsSortable(m_sort.nBy1))
	{
		// rebuild the list from the tree
		CTabbedToDoCtrl::RebuildList(&filter);
	}
	else // rebuild from scratch
	{
		// cache current selection
		TDCSELECTIONCACHE cache;
		CacheListSelection(cache);

		// grab the selected items for the filtering
		m_aSelectedTaskIDs.Copy(cache.aSelTaskIDs);

		// remove all existing items
		m_list.DeleteAllItems();

		// do the find
		CResultArray aResults;
		m_data.FindTasks(filter, aResults);

		// add tasks to list
		for (int nRes = 0; nRes < aResults.GetSize(); nRes++)
		{
			const SEARCHRESULT& res = aResults[nRes];

			// some more filtering required
			if (HasStyle(TDCS_ALWAYSHIDELISTPARENTS) || m_filter.HasFlag(FO_HIDEPARENTS))
			{
				TODOSTRUCTURE* pTDS = m_data.LocateTask(res.dwTaskID);
				ASSERT(pTDS);

				if (pTDS->HasSubTasks())
					continue;
			}
			else if (m_filter.HasFlag(FO_HIDECOLLAPSED))
			{
				HTREEITEM hti = m_find.GetItem(res.dwTaskID);
				ASSERT(hti);			

				if (m_tree.ItemHasChildren(hti) && !m_tree.TCH().IsItemExpanded(hti))
					continue;
			}

			CTabbedToDoCtrl::AddItemToList(res.dwTaskID);
		}

		// restore selection
		RestoreListSelection(cache);
	}
}

void CFilteredToDoCtrl::AddTreeItemToList(HTREEITEM hti, const void* pContext)
{
	if (pContext == NULL)
	{
		CTabbedToDoCtrl::AddTreeItemToList(hti, NULL);
		return;
	}

	// else it's a filter
	const SEARCHPARAMS* pFilter = static_cast<const SEARCHPARAMS*>(pContext);

	if (hti)
	{
		BOOL bAdd = TRUE;
		DWORD dwTaskID = GetTaskID(hti);

		// check if parent items are to be ignored
		if (m_filter.HasFlag(FO_HIDEPARENTS) || HasStyle(TDCS_ALWAYSHIDELISTPARENTS))
		{
			// quick test first
			if (m_tree.ItemHasChildren(hti))
				bAdd = FALSE;
			else
			{
				const TODOSTRUCTURE* pTDS = m_data.LocateTask(dwTaskID);
				ASSERT(pTDS);

				bAdd = !pTDS->HasSubTasks();
			}
		}
		// else check if it's a parent item that's only present because
		// it has matching subtasks
		else if (m_tree.ItemHasChildren(hti))
		{
			const TODOSTRUCTURE* pTDS = m_data.LocateTask(dwTaskID);
			const TODOITEM* pTDI = m_data.GetTask(dwTaskID); 
			SEARCHRESULT result;

			// ie. check that parent actually matches
			bAdd = m_data.TaskMatches(pTDI, pTDS, *pFilter, result);
		}

		if (bAdd)
			AddItemToList(dwTaskID);
	}

	// always check the children unless collapsed tasks ignored
	if (!m_filter.HasFlag(FO_HIDECOLLAPSED) || !hti || m_tree.TCH().IsItemExpanded(hti))
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			// check
			AddTreeItemToList(htiChild, pContext);
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
}

BOOL CFilteredToDoCtrl::SetStyles(const CTDCStylesMap& styles)
{
	if (CTabbedToDoCtrl::SetStyles(styles))
	{
		// do we need to re-filter?
		if (GetViewData2(FTCV_TASKLIST)->bNeedRefilter)
		{
			RefreshTreeFilter(); // always

			if (InListView())
				RefreshListFilter();
		}

		return TRUE;
	}

	return FALSE;
}

BOOL CFilteredToDoCtrl::SetStyle(TDC_STYLE nStyle, BOOL bOn, BOOL bWantUpdate)
{
	// base class processing
	if (CTabbedToDoCtrl::SetStyle(nStyle, bOn, bWantUpdate))
	{
		// post-precessing
		switch (nStyle)
		{
		case TDCS_DUEHAVEHIGHESTPRIORITY:
		case TDCS_DONEHAVELOWESTPRIORITY:
		case TDCS_ALWAYSHIDELISTPARENTS:
			GetViewData2(FTCV_TASKLIST)->bNeedRefilter = TRUE;
			break;

		case TDCS_TREATSUBCOMPLETEDASDONE:
			if (m_filter.nShow != FS_ALL)
				GetViewData2(FTCV_TASKLIST)->bNeedRefilter = TRUE;
			break;
		}

		return TRUE;
	}

	return FALSE;
}

BOOL CFilteredToDoCtrl::InitFilterDate(FILTER_DATE nDate, COleDateTime& date) const
{
	switch (nDate)
	{
	case FD_TODAY:
		date = CDateHelper::GetDate(DHD_TODAY);
		break;

	case FD_TOMORROW:
		date = CDateHelper::GetDate(DHD_TOMORROW);
		break;

	case FD_ENDTHISWEEK:
		date = CDateHelper::GetDate(DHD_ENDTHISWEEK);
		break;

	case FD_ENDNEXTWEEK: 
		date = CDateHelper::GetDate(DHD_ENDNEXTWEEK);
		break;

	case FD_ENDTHISMONTH:
		date = CDateHelper::GetDate(DHD_ENDTHISMONTH);
		break;

	case FD_ENDNEXTMONTH:
		date = CDateHelper::GetDate(DHD_ENDNEXTMONTH);
		break;

	case FD_ENDTHISYEAR:
		date = CDateHelper::GetDate(DHD_ENDTHISYEAR);
		break;

	case FD_ENDNEXTYEAR:
		date = CDateHelper::GetDate(DHD_ENDNEXTYEAR);
		break;

	case FD_NEXTSEVENDAYS:
		date = CDateHelper::GetDate(DHD_TODAY) + 6; // +6 because filter is FO_ON_OR_BEFORE
		break;

	case FD_NOW:
		date = COleDateTime::GetCurrentTime();
		break;

	case FD_ANY:
		break;

	default:
		ASSERT(0);
		break;
	}

	return (date.m_dt != 0.0);
}

BOOL CFilteredToDoCtrl::SplitSelectedTask(int nNumSubtasks)
{
   if (CTabbedToDoCtrl::SplitSelectedTask(nNumSubtasks))
   {
      if (InListView())
         RefreshListFilter();
 
      return TRUE;
   }
 
   return FALSE;
}

HTREEITEM CFilteredToDoCtrl::CreateNewTask(LPCTSTR szText, TDC_INSERTWHERE nWhere, BOOL bEditText)
{
	HTREEITEM htiNew = CTabbedToDoCtrl::CreateNewTask(szText, nWhere, bEditText);

	if (htiNew)
	{
		SetListNeedRefilter(!InListView());
		SetExtensionsNeedRefilter(TRUE, GetView());
	}

	return htiNew;
}

BOOL CFilteredToDoCtrl::CanCreateNewTask(TDC_INSERTWHERE nInsertWhere) const
{
	return CTabbedToDoCtrl::CanCreateNewTask(nInsertWhere);
}

void CFilteredToDoCtrl::SetModified(BOOL bMod, TDC_ATTRIBUTE nAttrib, DWORD dwModTaskID)
{
	CTabbedToDoCtrl::SetModified(bMod, nAttrib, dwModTaskID);

	if (bMod)
	{
		if (ModNeedsRefilter(nAttrib, FTCV_TASKTREE, dwModTaskID))
		{
			PostMessage(WM_TDC_REFRESHFILTER, FTCV_TASKTREE, (nAttrib == TDCA_UNDO));
		}
		else if (ModNeedsRefilter(nAttrib, FTCV_TASKLIST, dwModTaskID))
		{
			if (InListView())
				PostMessage(WM_TDC_REFRESHFILTER, FTCV_TASKLIST, (nAttrib == TDCA_UNDO));
			else
			{
				GetViewData2(FTCV_TASKLIST)->bNeedRefilter = TRUE;
			}
		}
	}
}

void CFilteredToDoCtrl::EndTimeTracking()
{
	BOOL bWasTimeTracking = IsActivelyTimeTracking();
	
	CTabbedToDoCtrl::EndTimeTracking();
	
	// do we need to refilter?
	if (bWasTimeTracking && m_bCustomFilter && m_customFilter.HasAttribute(TDCA_TIMESPENT))
	{
		RefreshFilter();
	}
}

BOOL CFilteredToDoCtrl::ModNeedsRefilter(TDC_ATTRIBUTE nModType, FTC_VIEW nView, DWORD dwModTaskID) const 
{
	if ((nModType == TDCA_NONE) || !HasStyle(TDCS_REFILTERONMODIFY))
		return FALSE;

	if (!m_bCustomFilter && !HasFilter())
		return FALSE;

	// we only need to refilter if the modified attribute
	// actually affects the filter
	BOOL bNeedRefilter = FALSE;

	if (m_bCustomFilter) // 'Find' filter
	{
		bNeedRefilter = m_customFilter.HasAttribute(nModType); 
		
		// don't refilter on Time Spent if time tracking
		if (bNeedRefilter && nModType == TDCA_TIMESPENT && IsActivelyTimeTracking())
		{
			bNeedRefilter = FALSE;
		}
	}
	else if (HasFilter()) // 'Filter Bar' filter
	{
		switch (nModType)
		{
		case TDCA_TASKNAME:		
			bNeedRefilter = !m_filter.sTitle.IsEmpty(); 
			break;
			
		case TDCA_PRIORITY:		
			bNeedRefilter = (m_filter.nPriority != -1); 
			break;
			
		case TDCA_FLAG:		
			bNeedRefilter = (m_filter.nShow == FS_FLAGGED); 
			break;
			
		case TDCA_RISK:			
			bNeedRefilter = (m_filter.nRisk != -1);
			break;
			
		case TDCA_ALLOCBY:		
			bNeedRefilter = (m_filter.aAllocBy.GetSize() > 0);
			break;
			
		case TDCA_STATUS:		
			bNeedRefilter = (m_filter.aStatus.GetSize() > 0);
			break;
			
		case TDCA_VERSION:		
			bNeedRefilter = (m_filter.aVersions.GetSize() > 0);
			break;
			
		case TDCA_CATEGORY:		
			bNeedRefilter = (m_filter.aCategories.GetSize() > 0);
			break;
			
		case TDCA_TAGS:		
			bNeedRefilter = (m_filter.aTags.GetSize() > 0);
			break;
			
		case TDCA_ALLOCTO:		
			bNeedRefilter = (m_filter.aAllocTo.GetSize() > 0);
			break;
			
		case TDCA_PERCENT:
			bNeedRefilter = (m_filter.nShow == FS_DONE || m_filter.nShow == FS_NOTDONE);
			break;
			
		case TDCA_DONEDATE:		
			// changing the DONE date requires refiltering if:
			bNeedRefilter = 
				// 1. The user wants to hide completed tasks
				(m_filter.HasFlag(FO_HIDEDONE) ||
				// 2. OR the user wants only completed tasks
				(m_filter.nShow == FS_DONE) || 
				// 3. OR the user wants only incomplete tasks
				(m_filter.nShow == FS_NOTDONE) ||
				// 4. OR a due date filter is active
				(m_filter.nDueBy != FD_ANY) ||
				// 5. OR a start date filter is active
				(m_filter.nStartBy != FD_ANY) ||
				// 6. OR the user is filtering on priority
				(m_filter.nPriority > 0));
			break;
			
		case TDCA_DUEDATE:		
			// changing the DUE date requires refiltering if:
			bNeedRefilter = 
				// 1. The user wants to hide overdue tasks
				(m_filter.HasFlag(FO_HIDEOVERDUE) ||
				// 2. OR the user is filtering on priority
				(m_filter.nPriority > 0) ||
				// 3. OR a due date filter is active
				(m_filter.nDueBy != FD_ANY) &&
				// 3. AND the user doesn't want only completed tasks
				(m_filter.nShow != FS_DONE));
			break;
			
		case TDCA_STARTDATE:		
			// changing the START date requires refiltering if:
			bNeedRefilter = 
				// 1. A start date filter is active
				((m_filter.nStartBy != FD_ANY) &&
				// 2. AND the user doesn't want only completed tasks
				(m_filter.nShow != FS_DONE));
			break;
			
		default:
			// fall thru
			break;
		}
	}

	// handle attributes common to both filter types
	if (!bNeedRefilter)
	{
		switch (nModType)
		{
		case TDCA_NEWTASK:
			// if we refilter in the middle of adding a task it messes
			// up the tree items so we handle it in CreateNewTask
			break;
			
		case TDCA_DELETE:
			// this is handled in SetModified
			break;
			
		case TDCA_COPY:
		case TDCA_UNDO:
			bNeedRefilter = TRUE;
			break;
			
		case TDCA_MOVE:
			bNeedRefilter = (nView == FTCV_TASKLIST && !IsSortable());
			break;

		case TDCA_SELECTION:
			// never need to refilter
			break;
		}
	}

	// finally, if this was a task edit we can just test to 
	// see if the modified task still matches the filter.
	if (bNeedRefilter && dwModTaskID)
	{
		// VERY SPECIAL CASE
		// The task being time tracked has been filtered out
		// in which case we don't need to check if it matches
		if (dwModTaskID == m_dwTimeTrackTaskID)
		{
			if (m_find.GetItem(dwModTaskID) == NULL)
			{
				ASSERT(HasTask(dwModTaskID));
				ASSERT(nModType == TDCA_TIMESPENT);

				return FALSE;
			}
			// else fall thru
		}

		SEARCHPARAMS params;
		SEARCHRESULT result;

		// This will handle custom and 'normal' filters
		BuildFilterQuery(params);
		bNeedRefilter = !m_data.TaskMatches(dwModTaskID, params, result);
		
		// extra handling for 'Find Tasks' filters 
		if (bNeedRefilter && m_bCustomFilter)
		{
			// don't refilter on Time Spent if time tracking
			bNeedRefilter = !(nModType == TDCA_TIMESPENT && IsActivelyTimeTracking());
		}
	}

	return bNeedRefilter;
}

void CFilteredToDoCtrl::Sort(TDC_COLUMN nBy, BOOL bAllowToggle)
{
	// special case
	if (nBy == TDCC_POSITION)
		nBy = TDCC_NONE;

	// we rebuild the entire listview if 'unsorting'
	if (InListView() && nBy == TDCC_NONE)
	{
		VIEWDATA* pLVData = GetViewData(FTCV_TASKLIST);
		TDC_COLUMN nPrevBy = pLVData->sort.nBy1;

		pLVData->sort.nBy1 = nBy;
		pLVData->bMultiSort = FALSE;

		if (nPrevBy != TDCC_NONE)
		{
			RefreshListFilter();
			UpdateListColumnWidths();
		}
		
		return;
	}

	CTabbedToDoCtrl::Sort(nBy, bAllowToggle);
}

VIEWDATA2* CFilteredToDoCtrl::GetViewData2(FTC_VIEW nView) const
{
	return (VIEWDATA2*)m_tabViews.GetViewData(nView);
}

VIEWDATA2* CFilteredToDoCtrl::GetActiveViewData2() const
{
	return (VIEWDATA2*)m_tabViews.GetViewData(GetView());
}

void CFilteredToDoCtrl::OnTimerMidnight()
{
	CTabbedToDoCtrl::OnTimerMidnight();

	// don't re-filter delay-loaded tasklists
	if (IsDelayLoaded())
		return;
	
	if ((m_filter.nStartBy != FD_NONE && m_filter.nStartBy != FD_ANY) ||
		(m_filter.nDueBy != FD_NONE && m_filter.nDueBy != FD_ANY))
	{
		RefreshFilter();
	}
}
