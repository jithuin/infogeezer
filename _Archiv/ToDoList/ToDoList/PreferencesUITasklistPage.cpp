// PreferencesUITasklistPage.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "PreferencesUITasklistPage.h"

#include "..\shared\colordef.h"
#include "..\shared\misc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPreferencesUITasklistPage property page

CPreferencesUITasklistPage::CPreferencesUITasklistPage() : 
	CPreferencesPageBase(CPreferencesUITasklistPage::IDD)
{
	//{{AFX_DATA_INIT(CPreferencesUITasklistPage)
	m_bHideDoneTimeField = FALSE;
	m_bHideListParents = FALSE;
	m_nPercentIncrement = -1;
	//}}AFX_DATA_INIT

}

CPreferencesUITasklistPage::~CPreferencesUITasklistPage()
{
}

void CPreferencesUITasklistPage::DoDataExchange(CDataExchange* pDX)
{ 
	CPreferencesPageBase::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPreferencesUITasklistPage)
	DDX_Control(pDX, IDC_PERCENTINCREMENT, m_cbPercentIncrement);
	DDX_Check(pDX, IDC_USEISODATEFORMAT, m_bUseISOForDates);
	DDX_Check(pDX, IDC_SHOWWEEKDAYINDATES, m_bShowWeekdayInDates);
	DDX_Check(pDX, IDC_SHOWPARENTSASFOLDERS, m_bShowParentsAsFolders);
	DDX_Check(pDX, IDC_DISPLAYFIRSTCOMMENTLINE, m_bDisplayFirstCommentLine);
	DDX_Check(pDX, IDC_LIMITINFOTIPCOMMENTS, m_bLimitInfoTipCommentsLength);
	DDX_Check(pDX, IDC_AUTOFOCUSTASKLIST, m_bAutoFocusTasklist);
	DDX_Check(pDX, IDC_SHOWSUBTASKCOMPLETION, m_bShowSubtaskCompletion);
	DDX_Check(pDX, IDC_RIGHTSIDECOLUMNS, m_bShowColumnsOnRight);
	DDX_Check(pDX, IDC_HIDEDUETIME, m_bHideDueTimeField);
	DDX_Check(pDX, IDC_HIDESTARTTIME, m_bHideStartTimeField);
	DDX_Check(pDX, IDC_LIMITCOLWIDTHS, m_bLimitColumnWidths);
	DDX_Text(pDX, IDC_COLWIDTHS, m_nMaxColumnWidth);
	DDX_Check(pDX, IDC_HIDEDONETIME, m_bHideDoneTimeField);
	DDX_Check(pDX, IDC_HIDELISTPARENTS, m_bHideListParents);
	//}}AFX_DATA_MAP
	DDX_Check(pDX, IDC_DISPLAYPATHINHEADER, m_bShowPathInHeader);
	DDX_Check(pDX, IDC_STRIKETHROUGHDONE, m_bStrikethroughDone);
	DDX_Check(pDX, IDC_FULLROWSELECTION, m_bFullRowSelection);
	DDX_Check(pDX, IDC_TREECHECKBOXES, m_bTreeCheckboxes);
	DDX_Check(pDX, IDC_TREETASKICONS, m_bTreeTaskIcons);
	DDX_Control(pDX, IDC_COLUMNVISIBILITY, m_lbColumnVisibility);
	DDX_Check(pDX, IDC_SHOWINFOTIPS, m_bShowInfoTips);
	DDX_Check(pDX, IDC_SHOWCOMMENTS, m_bShowComments);
	DDX_LBIndex(pDX, IDC_COLUMNVISIBILITY, m_nSelColumnVisibility);
	DDX_Text(pDX, IDC_INFOTIPCOMMENTSMAX, m_nMaxInfoTipCommentsLength);
	DDX_Check(pDX, IDC_HIDEUNDEFINEDTIMEST, m_bHideZeroTimeCost);
	DDX_Check(pDX, IDC_HIDESTARTDUEFORDONETASKS, m_bHideStartDueForDoneTasks);
	DDX_Check(pDX, IDC_ROUNDTIMEFRACTIONS, m_bRoundTimeFractions);
	DDX_Check(pDX, IDC_SHOWNONFILEREFSASTEXT, m_bShowNonFilesAsText);
	DDX_Check(pDX, IDC_USEHMSTIMEFORMAT, m_bUseHMSTimeFormat);
	DDX_Check(pDX, IDC_SHOWPERCENTPROGRESSBAR, m_bShowPercentAsProgressbar);
	DDX_Check(pDX, IDC_HIDEPERCENTFORDONETASKS, m_bHidePercentForDoneTasks);

	if (pDX->m_bSaveAndValidate)
		m_nPercentIncrement = CDialogHelper::GetSelectedItemAsValue(m_cbPercentIncrement);
	else
		CDialogHelper::SelectItemByValue(m_cbPercentIncrement, m_nPercentIncrement);
}


BEGIN_MESSAGE_MAP(CPreferencesUITasklistPage, CPreferencesPageBase)
	//{{AFX_MSG_MAP(CPreferencesUITasklistPage)
	ON_BN_CLICKED(IDC_SHOWCOMMENTS, OnShowcomments)
	ON_BN_CLICKED(IDC_SHOWINFOTIPS, OnShowinfotips)
	ON_BN_CLICKED(IDC_LIMITINFOTIPCOMMENTS, OnLimitinfotipcomments)
	ON_BN_CLICKED(IDC_LIMITCOLWIDTHS, OnLimitcolwidths)
	ON_BN_CLICKED(IDC_TREECHECKBOXES, OnTreecheckboxes)
	ON_BN_CLICKED(IDC_SHOWPARENTSASFOLDERS, OnShowparentsasfolders)
	ON_BN_CLICKED(IDC_TREETASKICONS, OnTreetaskicons)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPreferencesUITasklistPage message handlers

BOOL CPreferencesUITasklistPage::OnInitDialog() 
{
	CPreferencesPageBase::OnInitDialog();

	m_mgrGroupLines.AddGroupLine(IDC_COLUMNGROUP, *this);
	m_mgrGroupLines.AddGroupLine(IDC_DATETIMEGROUP, *this);
	m_mgrGroupLines.AddGroupLine(IDC_OTHERGROUP, *this);

	GetDlgItem(IDC_DISPLAYFIRSTCOMMENTLINE)->EnableWindow(m_bShowComments);
	GetDlgItem(IDC_LIMITINFOTIPCOMMENTS)->EnableWindow(m_bShowInfoTips);
	GetDlgItem(IDC_INFOTIPCOMMENTSMAX)->EnableWindow(m_bShowInfoTips && m_bLimitInfoTipCommentsLength);
	GetDlgItem(IDC_COLWIDTHS)->EnableWindow(m_bLimitColumnWidths);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

int CPreferencesUITasklistPage::GetMaxInfoTipCommentsLength() const
{
	if (m_bShowInfoTips)
		return m_bLimitInfoTipCommentsLength ? max(0, m_nMaxInfoTipCommentsLength) : -1;

	return -1;
}

int CPreferencesUITasklistPage::GetMaxColumnWidth() const
{
	return m_bLimitColumnWidths ? max(0, m_nMaxColumnWidth) : -1;
}

void CPreferencesUITasklistPage::OnShowcomments() 
{
	UpdateData();

	GetDlgItem(IDC_DISPLAYFIRSTCOMMENTLINE)->EnableWindow(m_bShowComments);
}

void CPreferencesUITasklistPage::OnShowinfotips() 
{
	UpdateData();

	GetDlgItem(IDC_LIMITINFOTIPCOMMENTS)->EnableWindow(m_bShowInfoTips);
	GetDlgItem(IDC_INFOTIPCOMMENTSMAX)->EnableWindow(m_bShowInfoTips && m_bLimitInfoTipCommentsLength); 

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistPage::OnLimitinfotipcomments() 
{
	UpdateData();

	GetDlgItem(IDC_INFOTIPCOMMENTSMAX)->EnableWindow(m_bShowInfoTips && m_bLimitInfoTipCommentsLength);

	CPreferencesPageBase::OnControlChange();
}

int CPreferencesUITasklistPage::GetDefaultColumns(CTDCColumnIDArray& aColumns) const
{
	return m_lbColumnVisibility.GetVisibleColumns(aColumns);
}

void CPreferencesUITasklistPage::SetDefaultColumns(const CTDCColumnIDArray& aColumns) 
{
	m_lbColumnVisibility.SetVisibleColumns(aColumns);
	SaveColumns();
}

void CPreferencesUITasklistPage::SaveColumns() const
{
	CPreferences prefs;
	CTDCColumnIDArray aCols;

	int nCol = m_lbColumnVisibility.GetVisibleColumns(aCols);
	prefs.WriteProfileInt(_T("Preferences\\ColumnVisibility"), _T("Count"), nCol);

	while (nCol--)
	{
		CString sKey = Misc::MakeKey(_T("Col%d"), nCol);
		prefs.WriteProfileInt(_T("Preferences\\ColumnVisibility"), sKey, aCols[nCol]);
	}
}

void CPreferencesUITasklistPage::LoadPreferences(const CPreferences& prefs)
{
	// column visibility
	CTDCColumnIDArray aCols;
	int nCol = prefs.GetProfileInt(_T("Preferences\\ColumnVisibility"), _T("Count"), -1);

	if (nCol > 0)
	{
		while (nCol--)
		{
			CString sKey = Misc::MakeKey(_T("Col%d"), nCol);
			TDC_COLUMN col = (TDC_COLUMN)prefs.GetProfileInt(_T("Preferences\\ColumnVisibility"), sKey, -1);
			
			if (col != (TDC_COLUMN)-1)
				aCols.Add(col);
		}
	}

	m_lbColumnVisibility.SetVisibleColumns(aCols);

	// prefs
	m_bShowInfoTips = prefs.GetProfileInt(_T("Preferences"), _T("ShowInfoTips"), FALSE);
	m_bShowComments = prefs.GetProfileInt(_T("Preferences"), _T("ShowComments"), TRUE);
	m_bDisplayFirstCommentLine = prefs.GetProfileInt(_T("Preferences"), _T("DisplayFirstCommentLine"), FALSE);
	m_bStrikethroughDone = prefs.GetProfileInt(_T("Preferences"), _T("StrikethroughDone"), TRUE);
	m_bShowPathInHeader = prefs.GetProfileInt(_T("Preferences"), _T("ShowPathInHeader"), TRUE);
	m_bFullRowSelection = prefs.GetProfileInt(_T("Preferences"), _T("FullRowSelection"), FALSE);
	m_bTreeCheckboxes = prefs.GetProfileInt(_T("Preferences"), _T("TreeCheckboxes"), TRUE);
	m_bTreeTaskIcons = prefs.GetProfileInt(_T("Preferences"), _T("TreeTaskIcons"), TRUE);
	m_bUseISOForDates = prefs.GetProfileInt(_T("Preferences"), _T("DisplayDatesInISO"), FALSE);
	m_bShowWeekdayInDates = prefs.GetProfileInt(_T("Preferences"), _T("ShowWeekdayInDates"), FALSE);
	m_bShowParentsAsFolders = prefs.GetProfileInt(_T("Preferences"), _T("ShowParentsAsFolders"), TRUE);
	m_nMaxInfoTipCommentsLength = prefs.GetProfileInt(_T("Preferences"), _T("MaxInfoTipCommentsLength"), 100);
	m_bLimitInfoTipCommentsLength = prefs.GetProfileInt(_T("Preferences"), _T("LimitInfoTipCommentsLength"), TRUE);
	m_bHidePercentForDoneTasks = prefs.GetProfileInt(_T("Preferences"), _T("HidePercentForDoneTasks"), TRUE);
	m_bHideStartDueForDoneTasks = prefs.GetProfileInt(_T("Preferences"), _T("HideStartDueForDoneTasks"), TRUE);
	m_bHideZeroTimeCost = prefs.GetProfileInt(_T("Preferences"), _T("HideZeroTimeEst"), TRUE);
	m_bShowPercentAsProgressbar = prefs.GetProfileInt(_T("Preferences"), _T("ShowPercentAsProgressbar"), FALSE);
	m_bRoundTimeFractions = prefs.GetProfileInt(_T("Preferences"), _T("RoundTimeFractions"), FALSE);
	m_bShowNonFilesAsText = prefs.GetProfileInt(_T("Preferences"), _T("ShowNonFilesAsText"), FALSE);
	m_bUseHMSTimeFormat = prefs.GetProfileInt(_T("Preferences"), _T("UseHMSTimeFormat"), FALSE);
	m_bAutoFocusTasklist = prefs.GetProfileInt(_T("Preferences"), _T("AutoFocusTasklist"), FALSE);
	m_bShowSubtaskCompletion = prefs.GetProfileInt(_T("Preferences"), _T("ShowSubtaskCompletion"), FALSE);
	m_bShowColumnsOnRight = prefs.GetProfileInt(_T("Preferences"), _T("ShowColumnsOnRight"), FALSE);
	m_bHideDueTimeField = prefs.GetProfileInt(_T("Preferences"), _T("HideDueTimeField"), FALSE);
	m_bHideStartTimeField = prefs.GetProfileInt(_T("Preferences"), _T("HideStartTimeField"), FALSE);
	m_bHideDoneTimeField = prefs.GetProfileInt(_T("Preferences"), _T("HideDoneTimeField"), FALSE);
	m_bLimitColumnWidths = prefs.GetProfileInt(_T("Preferences"), _T("LimitColumnWidths"), FALSE);
	m_nMaxColumnWidth = prefs.GetProfileInt(_T("Preferences"), _T("MaxColumnWidth"), 60);
	m_bHideListParents = prefs.GetProfileInt(_T("Preferences"), _T("AlwaysHideListParents"), FALSE);
	m_nPercentIncrement = prefs.GetProfileInt(_T("Preferences"), _T("PercentIncrement"), 5);
//	m_b = prefs.GetProfileInt(_T("Preferences"), _T(""), FALSE);
}

void CPreferencesUITasklistPage::SavePreferences(CPreferences& prefs)
{
	// save settings
	// column visibility
	CTDCColumnIDArray aCols;
	int nCol = m_lbColumnVisibility.GetVisibleColumns(aCols);

	prefs.WriteProfileInt(_T("Preferences\\ColumnVisibility"), _T("Count"), nCol);

	while (nCol--)
	{
		CString sKey = Misc::MakeKey(_T("Col%d"), nCol);
		prefs.WriteProfileInt(_T("Preferences\\ColumnVisibility"), sKey, aCols[nCol]);
	}

	// save settings
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowInfoTips"), m_bShowInfoTips);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowComments"), m_bShowComments);
	prefs.WriteProfileInt(_T("Preferences"), _T("DisplayFirstCommentLine"), m_bDisplayFirstCommentLine);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowPercentColumn"), m_bShowPercentColumn);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowPriorityColumn"), m_bShowPriorityColumn);
	prefs.WriteProfileInt(_T("Preferences"), _T("StrikethroughDone"), m_bStrikethroughDone);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowPathInHeader"), m_bShowPathInHeader);
	prefs.WriteProfileInt(_T("Preferences"), _T("FullRowSelection"), m_bFullRowSelection);
	prefs.WriteProfileInt(_T("Preferences"), _T("TreeCheckboxes"), m_bTreeCheckboxes);
	prefs.WriteProfileInt(_T("Preferences"), _T("TreeTaskIcons"), m_bTreeTaskIcons);
	prefs.WriteProfileInt(_T("Preferences"), _T("DisplayDatesInISO"), m_bUseISOForDates);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowWeekdayInDates"), m_bShowWeekdayInDates);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowParentsAsFolders"), m_bShowParentsAsFolders);
	prefs.WriteProfileInt(_T("Preferences"), _T("MaxInfoTipCommentsLength"), max(m_nMaxInfoTipCommentsLength, 0));
	prefs.WriteProfileInt(_T("Preferences"), _T("LimitInfoTipCommentsLength"), m_bLimitInfoTipCommentsLength);
	prefs.WriteProfileInt(_T("Preferences"), _T("HidePercentForDoneTasks"), m_bHidePercentForDoneTasks);
	prefs.WriteProfileInt(_T("Preferences"), _T("HideStartDueForDoneTasks"), m_bHideStartDueForDoneTasks);
	prefs.WriteProfileInt(_T("Preferences"), _T("HideZeroTimeEst"), m_bHideZeroTimeCost);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowPercentAsProgressbar"), m_bShowPercentAsProgressbar);
	prefs.WriteProfileInt(_T("Preferences"), _T("RoundTimeFractions"), m_bRoundTimeFractions);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowNonFilesAsText"), m_bShowNonFilesAsText);
	prefs.WriteProfileInt(_T("Preferences"), _T("UseHMSTimeFormat"), m_bUseHMSTimeFormat);
	prefs.WriteProfileInt(_T("Preferences"), _T("AutoFocusTasklist"), m_bAutoFocusTasklist);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowSubtaskCompletion"), m_bShowSubtaskCompletion);
	prefs.WriteProfileInt(_T("Preferences"), _T("ShowColumnsOnRight"), m_bShowColumnsOnRight);
	prefs.WriteProfileInt(_T("Preferences"), _T("HideDueTimeField"), m_bHideDueTimeField);
	prefs.WriteProfileInt(_T("Preferences"), _T("HideStartTimeField"), m_bHideStartTimeField);
	prefs.WriteProfileInt(_T("Preferences"), _T("HideDoneTimeField"), m_bHideDoneTimeField);
	prefs.WriteProfileInt(_T("Preferences"), _T("LimitColumnWidths"), m_bLimitColumnWidths);
	prefs.WriteProfileInt(_T("Preferences"), _T("MaxColumnWidth"), m_nMaxColumnWidth);
	prefs.WriteProfileInt(_T("Preferences"), _T("AlwaysHideListParents"), m_bHideListParents);
	prefs.WriteProfileInt(_T("Preferences"), _T("PercentIncrement"), m_nPercentIncrement);
//	prefs.WriteProfileInt(_T("Preferences"), _T(""), m_b);
}



void CPreferencesUITasklistPage::OnLimitcolwidths() 
{
	UpdateData();

	GetDlgItem(IDC_COLWIDTHS)->EnableWindow(m_bLimitColumnWidths);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistPage::OnTreecheckboxes() 
{
	// if the user is turning this preference on then make sure that
	// the completion column is hidden else the checkbox won't turn up
	UpdateData();

	if (m_bTreeCheckboxes)
		m_lbColumnVisibility.SetColumnVisible(TDCC_DONE, FALSE);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistPage::OnShowparentsasfolders() 
{
	UpdateData();

	if (m_bShowParentsAsFolders)
		m_lbColumnVisibility.SetColumnVisible(TDCC_ICON);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistPage::OnTreetaskicons() 
{
	// if the user is turning this preference on then make sure that
	// the completion column is hidden else the checkbox won't turn up
	UpdateData();

	if (m_bTreeTaskIcons)
		m_lbColumnVisibility.SetColumnVisible(TDCC_ICON, FALSE);

	CPreferencesPageBase::OnControlChange();
}
