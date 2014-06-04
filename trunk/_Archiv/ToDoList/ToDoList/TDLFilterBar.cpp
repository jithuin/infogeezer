// FilterBar.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLFilterBar.h"
#include "tdcmsg.h"
#include "filteredtodoctrl.h"

#include "..\shared\deferwndmove.h"
#include "..\shared\dlgunits.h"
#include "..\shared\enstring.h"
#include "..\shared\localizer.h"
#include "..\shared\graphicsmisc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////

const int CTRLXSPACING = 6; // dlu
const int CTRLYSPACING = 2; // dlu
const int CTRLLABELLEN = 45;
const int CTRLLEN = 75;
const int CTRLHEIGHT = 13;

const COLORREF NO_COLOR = (COLORREF)-1;

struct FILTERCTRL
{
	UINT nIDLabel;
	UINT nIDCtrl;
	TDC_COLUMN nType;
};

static FILTERCTRL FILTERCTRLS[] = 
{
	{ IDC_FILTERLABEL, IDC_FILTERCOMBO, TDCC_NONE },
	{ IDC_TITLEFILTERLABEL, IDC_TITLEFILTERTEXT, TDCC_NONE },
	{ IDC_STARTFILTERLABEL, IDC_STARTFILTERCOMBO, TDCC_STARTDATE },
	{ IDC_DUEFILTERLABEL, IDC_DUEFILTERCOMBO, TDCC_DUEDATE },
	{ IDC_PRIORITYFILTERLABEL, IDC_PRIORITYFILTERCOMBO, TDCC_PRIORITY },
	{ IDC_RISKFILTERLABEL, IDC_RISKFILTERCOMBO, TDCC_RISK },
	{ IDC_ALLOCTOFILTERLABEL, IDC_ALLOCTOFILTERCOMBO, TDCC_ALLOCTO },
	{ IDC_ALLOCBYFILTERLABEL, IDC_ALLOCBYFILTERCOMBO, TDCC_ALLOCBY },
	{ IDC_STATUSFILTERLABEL, IDC_STATUSFILTERCOMBO, TDCC_STATUS },
	{ IDC_CATEGORYFILTERLABEL, IDC_CATEGORYFILTERCOMBO, TDCC_CATEGORY },
	{ IDC_TAGFILTERLABEL, IDC_TAGFILTERCOMBO, TDCC_TAGS },
	{ IDC_VERSIONFILTERLABEL, IDC_VERSIONFILTERCOMBO, TDCC_VERSION },
	{ IDC_OPTIONFILTERLABEL, IDC_OPTIONFILTERCOMBO, TDCC_NONE }
};

const int NUMFILTERCTRLS = sizeof(FILTERCTRLS) / sizeof(FILTERCTRL);

#define WM_WANTCOMBOPROMPT (WM_APP+1)

/////////////////////////////////////////////////////////////////////////////
// CFilterBar dialog

CTDLFilterBar::CTDLFilterBar(CWnd* pParent /*=NULL*/)
	: CDialog(IDD_FILTER_BAR, pParent), 
	  m_cbCategoryFilter(TRUE, IDS_TDC_NONE, IDS_TDC_ANY),
	  m_cbAllocToFilter(TRUE, IDS_TDC_NOBODY, IDS_TDC_ANYONE),
	  m_cbAllocByFilter(TRUE, IDS_TDC_NOBODY, IDS_TDC_ANYONE),
	  m_cbStatusFilter(TRUE, IDS_TDC_NONE, IDS_TDC_ANY),
	  m_cbVersionFilter(TRUE, IDS_TDC_NONE, IDS_TDC_ANY),
	  m_cbTagFilter(TRUE, IDS_TDC_NONE, IDS_TDC_ANY),
	  m_nView(FTCV_UNSET),
	  m_bCustomFilter(FALSE)
{
	//{{AFX_DATA_INIT(CFilterBar)
	//}}AFX_DATA_INIT

	m_aVisibility.SetSize(TDCC_LAST);

	// add update button to title text
	m_eTitleFilter.AddButton(1, 0xC4, 
							CEnString(IDS_TDC_UPDATEFILTER_TIP),
							CALC_BTNWIDTH, _T("Wingdings"));
}


void CTDLFilterBar::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CFilterBar)
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_TAGFILTERCOMBO, m_cbTagFilter);
	DDX_Control(pDX, IDC_VERSIONFILTERCOMBO, m_cbVersionFilter);
	DDX_Control(pDX, IDC_OPTIONFILTERCOMBO, m_cbOptions);
	DDX_Control(pDX, IDC_FILTERCOMBO, m_cbTaskFilter);
	DDX_Control(pDX, IDC_STARTFILTERCOMBO, m_cbStartFilter);
	DDX_Control(pDX, IDC_DUEFILTERCOMBO, m_cbDueFilter);
	DDX_Control(pDX, IDC_ALLOCTOFILTERCOMBO, m_cbAllocToFilter);
	DDX_Control(pDX, IDC_ALLOCBYFILTERCOMBO, m_cbAllocByFilter);
	DDX_Control(pDX, IDC_CATEGORYFILTERCOMBO, m_cbCategoryFilter);
	DDX_Control(pDX, IDC_STATUSFILTERCOMBO, m_cbStatusFilter);
	DDX_Control(pDX, IDC_PRIORITYFILTERCOMBO, m_cbPriorityFilter);
	DDX_Control(pDX, IDC_RISKFILTERCOMBO, m_cbRiskFilter);
	DDX_Control(pDX, IDC_TITLEFILTERTEXT, m_eTitleFilter);
	DDX_Text(pDX, IDC_TITLEFILTERTEXT, m_filter.sTitle);
	
	// special handling
	if (pDX->m_bSaveAndValidate)
	{
		// filter
		m_filter.nShow = m_cbTaskFilter.GetSelectedFilter(m_sCustomFilter);
		m_bCustomFilter = (m_filter.nShow == FS_CUSTOM);

		m_filter.nStartBy = m_cbStartFilter.GetSelectedFilter();
		m_filter.nDueBy = m_cbDueFilter.GetSelectedFilter();

		// priority
		int nIndex;
		DDX_CBIndex(pDX, IDC_PRIORITYFILTERCOMBO, nIndex);

		if (nIndex == 0) // any
			m_filter.nPriority = FM_ANYPRIORITY;

		else if (nIndex == 1) // none
			m_filter.nPriority = FM_NOPRIORITY;
		else
			m_filter.nPriority = nIndex - 2;

		// risk
		DDX_CBIndex(pDX, IDC_RISKFILTERCOMBO, nIndex);

		if (nIndex == 0) // any
			m_filter.nRisk = FM_ANYRISK;

		else if (nIndex == 1) // none
			m_filter.nRisk = FM_NORISK;
		else
			m_filter.nRisk = nIndex - 2;

		// check combos
		m_cbCategoryFilter.GetChecked(m_filter.aCategories);
		m_cbAllocToFilter.GetChecked(m_filter.aAllocTo);
		m_cbStatusFilter.GetChecked(m_filter.aStatus);
		m_cbAllocByFilter.GetChecked(m_filter.aAllocBy);
		m_cbVersionFilter.GetChecked(m_filter.aVersions);
		m_cbTagFilter.GetChecked(m_filter.aTags);

		// flags
		m_filter.dwFlags = m_cbOptions.GetOptions();
	}
	else
	{
		// filter
		if (m_bCustomFilter)
			m_cbTaskFilter.SelectFilter(m_sCustomFilter);
		else
			m_cbTaskFilter.SelectFilter(m_filter.nShow);
		
		m_cbStartFilter.SelectFilter(m_filter.nStartBy);
		m_cbDueFilter.SelectFilter(m_filter.nDueBy);

		// priority
		int nIndex;
		
		if (m_filter.nPriority == FM_ANYPRIORITY)
			nIndex = 0;

		else if (m_filter.nPriority == FM_NOPRIORITY)
			nIndex = 1;
		else
			nIndex = m_filter.nPriority + 2;

		DDX_CBIndex(pDX, IDC_PRIORITYFILTERCOMBO, nIndex);

		// risk
		if (m_filter.nRisk == FM_ANYRISK)
			nIndex = 0;

		else if (m_filter.nRisk == FM_NORISK)
			nIndex = 1;
		else
			nIndex = m_filter.nRisk + 2;

		DDX_CBIndex(pDX, IDC_RISKFILTERCOMBO, nIndex);

		// check combos
		m_cbCategoryFilter.SetChecked(m_filter.aCategories);
		m_cbAllocToFilter.SetChecked(m_filter.aAllocTo);
		m_cbStatusFilter.SetChecked(m_filter.aStatus);
		m_cbAllocByFilter.SetChecked(m_filter.aAllocBy);
		m_cbVersionFilter.SetChecked(m_filter.aVersions);
		m_cbTagFilter.SetChecked(m_filter.aTags);

		// options
		m_cbOptions.Initialize(m_filter, m_nView, m_bWantHideParents);
	}
}


BEGIN_MESSAGE_MAP(CTDLFilterBar, CDialog)
	//{{AFX_MSG_MAP(CFilterBar)
	ON_WM_SIZE()
	//}}AFX_MSG_MAP
	ON_CBN_SELCHANGE(IDC_FILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_STARTFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_DUEFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_ALLOCTOFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_TAGFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_VERSIONFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_ALLOCBYFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_STATUSFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_PRIORITYFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_CATEGORYFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_SELCHANGE(IDC_RISKFILTERCOMBO, OnSelchangeFilter)
	ON_CBN_CLOSEUP(IDC_OPTIONFILTERCOMBO, OnSelchangeFilter)
//	ON_EN_CHANGE(IDC_TITLEFILTERTEXT, OnSelchangeFilter)
	ON_NOTIFY_EX_RANGE(TTN_NEEDTEXT, 0, 0xFFFF, OnToolTipNotify)
	ON_WM_ERASEBKGND()
	ON_REGISTERED_MESSAGE(WM_EE_BTNCLICK, OnEEBtnClick)
	ON_WM_CTLCOLOR()
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CFilterBar message handlers

BOOL CTDLFilterBar::Create(CWnd* pParentWnd, UINT nID, BOOL bVisible)
{
	if (CDialog::Create(IDD_FILTER_BAR, pParentWnd))
	{
		SetDlgCtrlID(nID);
		ShowWindow(bVisible ? SW_SHOW : SW_HIDE);

		return TRUE;
	}

	return FALSE;
}

void CTDLFilterBar::OnSelchangeFilter() 
{
	FTDCFILTER fPrev(m_filter);
	UpdateData(); // updates m_filter

	GetParent()->SendMessage(WM_FBN_FILTERCHNG, (WPARAM)&m_filter, (LPARAM)(LPCTSTR)m_sCustomFilter);

	// if the main 'Show" filter has changed then we need to re-init the options
	if (m_filter.nShow != fPrev.nShow)
	{
		m_cbOptions.Initialize(m_filter, m_nView, m_bWantHideParents);

		// enable/disable controls
		if (m_filter.nShow == FS_SELECTED || fPrev.nShow == FS_SELECTED)
			ReposControls();
	}
}

LRESULT CTDLFilterBar::OnEEBtnClick(WPARAM /*wp*/, LPARAM /*lp*/)
{
	OnSelchangeFilter();
	return 0L;
}

BOOL CTDLFilterBar::PreTranslateMessage(MSG* pMsg)
{
	// handle return key in title field
	if (pMsg->message == WM_KEYDOWN && pMsg->hwnd == m_eTitleFilter &&
		pMsg->wParam == VK_RETURN)
	{
		OnSelchangeFilter();
		return TRUE;
	}

	return CDialog::PreTranslateMessage(pMsg);
}

void CTDLFilterBar::SetCustomFilter(BOOL bCustom, LPCTSTR szFilter)
{
	if (bCustom)
		m_cbTaskFilter.SelectFilter(szFilter);

	m_bCustomFilter = bCustom;
	m_sCustomFilter = szFilter;

	// disable controls if a custom filter.
	// just do a repos because this also handles enabled state
	ReposControls();
}

void CTDLFilterBar::AddCustomFilters(const CStringArray& aFilters)
{
	m_cbTaskFilter.AddCustomFilters(aFilters); 
}

int CTDLFilterBar::GetCustomFilters(CStringArray& aFilters) const
{
	return m_cbTaskFilter.GetCustomFilters(aFilters); 
}

void CTDLFilterBar::RemoveCustomFilters()
{
	m_cbTaskFilter.RemoveCustomFilters(); 
}

void CTDLFilterBar::ShowDefaultFilters(BOOL bShow)
{
	m_cbTaskFilter.ShowDefaultFilters(bShow); 
}

void CTDLFilterBar::RefreshFilterControls(const CFilteredToDoCtrl& tdc)
{
	m_bWantHideParents = tdc.HasStyle(TDCS_ALWAYSHIDELISTPARENTS);

	// column visibility
	CTDCColumnIDArray aColumns;
	tdc.GetVisibleColumns(aColumns);

	SetVisibleFilters(aColumns);

	// get filter
	tdc.GetFilter(m_filter);
	m_nView = tdc.GetView();

	// get custom filter
	m_bCustomFilter = tdc.HasCustomFilter();
	m_sCustomFilter = tdc.GetCustomFilterName();

	if (m_bCustomFilter && m_sCustomFilter.IsEmpty())
		m_sCustomFilter = CEnString(IDS_UNNAMEDFILTER);
	
	// alloc to filter
	CStringArray aItems;

	tdc.GetAllocToNames(aItems);
	m_cbAllocToFilter.SetStrings(aItems);
	m_cbAllocToFilter.AddEmptyString(); // add blank item to top
	
	// alloc by filter
	tdc.GetAllocByNames(aItems);
	m_cbAllocByFilter.SetStrings(aItems);
	m_cbAllocByFilter.AddEmptyString(); // add blank item to top
	
	// category filter
	tdc.GetCategoryNames(aItems);
	m_cbCategoryFilter.SetStrings(aItems);
	m_cbCategoryFilter.AddEmptyString(); // add blank item to top
	
	// status filter
	tdc.GetStatusNames(aItems);
	m_cbStatusFilter.SetStrings(aItems);
	m_cbStatusFilter.AddEmptyString(); // add blank item to top
	
	// version filter
	tdc.GetVersionNames(aItems);
	m_cbVersionFilter.SetStrings(aItems);
	m_cbVersionFilter.AddEmptyString(); // add blank item to top
	
	// tag filter
	tdc.GetTagNames(aItems);
	m_cbTagFilter.SetStrings(aItems);
	m_cbTagFilter.AddEmptyString(); // add blank item to top
	
	// priority
	m_cbPriorityFilter.SetColors(m_aPriorityColors);
	m_cbPriorityFilter.InsertColor(0, NO_COLOR, CString((LPCTSTR)IDS_TDC_ANY)); // add a blank item
	
	// risk never needs changing
	
	// update UI
	UpdateData(FALSE);
	Invalidate();

	// disable controls if a custom filter.
	// just do a repos because this also handles enabled state
	ReposControls();
}

void CTDLFilterBar::SetFilterLabelAlignment(BOOL bLeft)
{
	UINT nAlign = bLeft ? SS_LEFT : SS_RIGHT;
	
	int nLabel = NUMFILTERCTRLS;
	
	while (nLabel--)
	{
		UINT nLabelID = FILTERCTRLS[nLabel].nIDLabel;

		if (nLabelID)
		{
			CWnd* pLabel = GetDlgItem(nLabelID);
			pLabel->ModifyStyle(SS_TYPEMASK, nAlign);
			pLabel->Invalidate();
		}
	}
}

void CTDLFilterBar::SetPriorityColors(const CDWordArray& aColors)
{
	m_aPriorityColors.Copy(aColors);

	if (m_cbPriorityFilter.GetSafeHwnd())
	{
		// save and restore current selection
		int nSel = m_cbPriorityFilter.GetCurSel();

		m_cbPriorityFilter.SetColors(aColors);
		m_cbPriorityFilter.InsertColor(0, NO_COLOR, CString((LPCTSTR)IDS_TDC_ANY)); // add a blank item

		m_cbPriorityFilter.SetCurSel(nSel);
	}
}

void CTDLFilterBar::OnSize(UINT nType, int cx, int cy) 
{
	CDialog::OnSize(nType, cx, cy);
	
	// check we're not in the middle of creation
	if (m_cbStatusFilter.GetSafeHwnd())
		ReposControls(cx, FALSE);
}

int CTDLFilterBar::CalcHeight(int nWidth)
{
	return ReposControls(nWidth, TRUE);
}

void CTDLFilterBar::SetVisibleFilters(const CTDCColumnIDArray& aFilters)
{
	// clear first
	int nFilter = m_aVisibility.GetSize();

	while (nFilter--)
		m_aVisibility[nFilter] = FALSE;

	// then set the visible ones
	int nItem = aFilters.GetSize();

	while (nItem--)
	{
		nFilter = aFilters[nItem];
		m_aVisibility[nFilter] = TRUE;
	}

	ReposControls();
}

BOOL CTDLFilterBar::WantShowFilter(TDC_COLUMN nType)
{
	if (nType == TDCC_NONE)
		return TRUE;

	return (BOOL)m_aVisibility[(DWORD)nType];
}

void CTDLFilterBar::EnableMultiSelection(BOOL bEnable)
{
	m_cbCategoryFilter.EnableMultiSelection(bEnable);
	m_cbAllocToFilter.EnableMultiSelection(bEnable);
	m_cbStatusFilter.EnableMultiSelection(bEnable);
	m_cbAllocByFilter.EnableMultiSelection(bEnable);
	m_cbVersionFilter.EnableMultiSelection(bEnable);
	m_cbTagFilter.EnableMultiSelection(bEnable);
}

int CTDLFilterBar::ReposControls(int nWidth, BOOL bCalcOnly)
{
	CDeferWndMove dwm(bCalcOnly ? 0 : NUMFILTERCTRLS + 1);

	if (nWidth <= 0)
	{
		CRect rClient;
		GetClientRect(rClient);

		nWidth = rClient.Width();
	}

	// Note: All calculations are performed in DLU until just before the move
	// is performed. This ensures that we minimize the risk of rounding errors.
	CDlgUnits dlu(*this);
	
	int nXPosDLU = 0, nYPosDLU = 0;
	int nWidthDLU = dlu.FromPixelsX(nWidth), nCtrlHeightDLU = CTRLHEIGHT;

	for (int nCtrl = 0; nCtrl < NUMFILTERCTRLS; nCtrl++)
	{
		CRect rCtrl, rCtrlDLU;
		const FILTERCTRL& fc = FILTERCTRLS[nCtrl];
		
		// display this control only if the corresponding column
		// is also showing
		BOOL bWantCtrl = WantShowFilter(fc.nType);
		
		if (bWantCtrl)
		{
			// if we're at the start of the line then just move ctrls
			// else we must check whether there's enough space to fit
			if (nXPosDLU > 0) // we're not the first
			{
				// do we fit?
				if ((nXPosDLU + CTRLLEN) > nWidthDLU) // no
				{
					// move to next line
					nXPosDLU = 0;
					nYPosDLU += CTRLYSPACING + (2 * nCtrlHeightDLU);
				}
			}
			
			// move label
			rCtrlDLU.left = nXPosDLU;
			rCtrlDLU.right = nXPosDLU + CTRLLEN;
			rCtrlDLU.top = nYPosDLU;
			rCtrlDLU.bottom = nYPosDLU + nCtrlHeightDLU;

			rCtrl = rCtrlDLU;
			dlu.ToPixels(rCtrl);
			
			if (fc.nIDLabel && !bCalcOnly)
				dwm.MoveWindow(GetDlgItem(fc.nIDLabel), rCtrl);
			
			// update YPos for the ctrl
			rCtrlDLU.OffsetRect(0, nCtrlHeightDLU);
			
			// move ctrl
			rCtrl = rCtrlDLU;
			dlu.ToPixels(rCtrl);
			
			if (!bCalcOnly)
				dwm.MoveWindow(GetDlgItem(fc.nIDCtrl), rCtrl);
			
			// update XPos for the control
			nXPosDLU = rCtrlDLU.right + CTRLXSPACING;
		}

		// show/hide and enable as appropriate
		if (!bCalcOnly)
		{
			BOOL bSelFilter = (m_filter.nShow == FS_SELECTED);
			BOOL bEnable = bWantCtrl;
			
			// special cases
			if (bEnable && fc.nIDCtrl != IDC_FILTERCOMBO)
			{
				if (bSelFilter && fc.nIDCtrl != IDC_OPTIONFILTERCOMBO)
					bEnable = FALSE;

				else if (m_bCustomFilter)
					bEnable = FALSE;
			}

			if (fc.nIDLabel)
			{
				GetDlgItem(fc.nIDLabel)->ShowWindow(bWantCtrl ? SW_SHOW : SW_HIDE);

				// note the first ctrl is always enabled even for custom filter
				GetDlgItem(fc.nIDLabel)->EnableWindow(bEnable);
			}
			
			GetDlgItem(fc.nIDCtrl)->ShowWindow(bWantCtrl ? SW_SHOW : SW_HIDE);
			GetDlgItem(fc.nIDCtrl)->EnableWindow(bEnable);
		}
	}

	// update bottom of filter bar
	nYPosDLU += (2 * nCtrlHeightDLU) + 2;

	return dlu.ToPixelsY(nYPosDLU);
}

void CTDLFilterBar::SetFilter(const FTDCFILTER& filter, FTC_VIEW nView)
{
	m_filter = filter;
	m_nView = nView;
	m_bCustomFilter = FALSE;

	SetCustomFilter(FALSE);

	UpdateData(FALSE);
}

BOOL CTDLFilterBar::OnInitDialog() 
{
	CDialog::OnInitDialog();

	// disable translation of user-data combos
	CLocalizer::EnableTranslation(m_cbAllocByFilter, FALSE);
	CLocalizer::EnableTranslation(m_cbAllocToFilter, FALSE);
	CLocalizer::EnableTranslation(m_cbCategoryFilter, FALSE);
	CLocalizer::EnableTranslation(m_cbStatusFilter, FALSE);
	CLocalizer::EnableTranslation(m_cbVersionFilter, FALSE);
	CLocalizer::EnableTranslation(m_cbTagFilter, FALSE);
	
	// one-time init for risk filter combo
	CEnString sAny(IDS_TDC_ANY);
	m_cbRiskFilter.InsertString(0, sAny); // add a blank item

	m_eTitleFilter.ModifyStyle(0, ES_WANTRETURN, 0);
	m_mgrPrompts.SetEditPrompt(m_eTitleFilter, sAny);

	EnableToolTips();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CTDLFilterBar::OnToolTipNotify(UINT /*id*/, NMHDR* pNMHDR, LRESULT* /*pResult*/)
{
    // Get the tooltip structure.
    TOOLTIPTEXT *pTTT = (TOOLTIPTEXT *)pNMHDR;

    // Actually the idFrom holds Control's handle.
    UINT CtrlHandle = pNMHDR->idFrom;

    // Check once again that the idFrom holds handle itself.
    if (pTTT->uFlags & TTF_IDISHWND)
    {
		static CString sTooltip;
		sTooltip.Empty();

        // Get the control's ID.
        UINT nID = ::GetDlgCtrlID( HWND( CtrlHandle ));

        switch( nID )
        {
        case IDC_CATEGORYFILTERCOMBO:
			sTooltip = m_cbCategoryFilter.GetTooltip();
            break;

        case IDC_ALLOCTOFILTERCOMBO:
			sTooltip = m_cbAllocToFilter.GetTooltip();
            break;

        case IDC_STATUSFILTERCOMBO:
			sTooltip = m_cbStatusFilter.GetTooltip();
            break;

        case IDC_ALLOCBYFILTERCOMBO:
			sTooltip = m_cbAllocByFilter.GetTooltip();
            break;

        case IDC_VERSIONFILTERCOMBO:
			sTooltip = m_cbVersionFilter.GetTooltip();
            break;

        case IDC_TAGFILTERCOMBO:
			sTooltip = m_cbTagFilter.GetTooltip();
            break;

        case IDC_OPTIONFILTERCOMBO:
			sTooltip = m_cbOptions.GetTooltip();
            break;
        }

		if (!sTooltip.IsEmpty())
		{
			// disable translation of the tip
			CLocalizer::EnableTranslation(pNMHDR->hwndFrom, FALSE);

			// Set the tooltip text.
			::SendMessage(pNMHDR->hwndFrom, TTM_SETMAXTIPWIDTH, 0, 300);
			pTTT->lpszText = (LPTSTR)(LPCTSTR)sTooltip;
	        return TRUE;
		}
    }

    // Not handled.
    return FALSE;
}

void CTDLFilterBar::SetUIColors(COLORREF crBack, COLORREF crText)
{
	m_crUIText = crText;

	if (crBack != m_crUIBack)
	{
		m_crUIBack = crBack;
		GraphicsMisc::VerifyDeleteObject(m_brUIBack);

		if (crBack != NO_COLOR)
			m_brUIBack.CreateSolidBrush(m_crUIBack);
	}

	Invalidate();
}

BOOL CTDLFilterBar::OnEraseBkgnd(CDC* pDC) 
{
	int nDC = pDC->SaveDC();
	
	// clip out all the child controls to reduce flicker
	if (!(GetStyle() & WS_CLIPCHILDREN))
	{
		ExcludeControls(this, pDC, IDC_FILTERLABEL, IDC_OPTIONFILTERCOMBO, TRUE);
	}

	if (m_brUIBack.GetSafeHandle())
	{
		CRect rect;
		GetClientRect(rect);
		pDC->FillSolidRect(rect, m_crUIBack);
	}
	else
		CDialog::OnEraseBkgnd(pDC);
	
	pDC->RestoreDC(nDC);
	
	return TRUE;
}

HBRUSH CTDLFilterBar::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor) 
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);

	if (nCtlColor == CTLCOLOR_STATIC && m_brUIBack.GetSafeHandle())
	{
		pDC->SetTextColor(m_crUIText);
		pDC->SetBkMode(TRANSPARENT);
		hbr = m_brUIBack;
	}
	
	return hbr;
}


