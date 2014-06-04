// GanttPreferencesDlg.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "GanttPreferencesDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

const COLORREF NO_COLOR = (COLORREF)-1;
const COLORREF DEF_TODAYCOLOR		= RGB(255, 0, 0);
const COLORREF DEF_WEEKENDCOLOR		= RGB(224, 224, 224);
const COLORREF DEF_PARENTCOLOR		= RGB(0, 0, 0);

/////////////////////////////////////////////////////////////////////////////
// CGanttPreferencesDlg dialog

CGanttPreferencesDlg::CGanttPreferencesDlg(CWnd* pParent /*=NULL*/)
	: CDialog(IDD_PREFERENCES_DIALOG, pParent)
{
	//{{AFX_DATA_INIT(CGanttPreferencesDlg)
	m_bDisplayAllocTo = TRUE;
	m_bAutoScrollSelection = TRUE;
	m_bSpecifyWeekendColor = TRUE;
	m_bSpecifyTodayColor = TRUE;
	m_bAutoCalcParentDates = TRUE;
	m_bCalculateMissingStartDates = TRUE;
	m_bCalculateMissingDueDates = TRUE;
	m_nParentColoring = 0;
	//}}AFX_DATA_INIT
	m_crParent = DEF_PARENTCOLOR;
	m_crToday = DEF_TODAYCOLOR;
	m_crWeekend = DEF_WEEKENDCOLOR;
}


void CGanttPreferencesDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CGanttPreferencesDlg)
	DDX_Check(pDX, IDC_DISPLAYALLOCTO, m_bDisplayAllocTo);
	DDX_Check(pDX, IDC_AUTOSCROLLSELECTION, m_bAutoScrollSelection);
	DDX_Check(pDX, IDC_WEEKENDCOLOR, m_bSpecifyWeekendColor);
	DDX_Check(pDX, IDC_TODAYCOLOR, m_bSpecifyTodayColor);
	DDX_Check(pDX, IDC_CALCULATEPARENTDATES, m_bAutoCalcParentDates);
	DDX_Check(pDX, IDC_CALCULATEMISSINGSTARTDATE, m_bCalculateMissingStartDates);
	DDX_Check(pDX, IDC_CALCULATEMISSINGDUEDATE, m_bCalculateMissingDueDates);
	DDX_Radio(pDX, IDC_DEFAULTPARENTCOLORS, m_nParentColoring);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_SETTODAYCOLOR, m_btTodayColor);
	DDX_Control(pDX, IDC_SETWEEKENDCOLOR, m_btWeekendColor);
	DDX_Control(pDX, IDC_SETPARENTCOLOR, m_btParentColor);
}


BEGIN_MESSAGE_MAP(CGanttPreferencesDlg, CDialog)
	//{{AFX_MSG_MAP(CGanttPreferencesDlg)
	ON_BN_CLICKED(IDC_SETWEEKENDCOLOR, OnSetWeekendcolor)
	ON_BN_CLICKED(IDC_SETTODAYCOLOR, OnSetTodaycolor)
	ON_BN_CLICKED(IDC_WEEKENDCOLOR, OnWeekendcolor)
	ON_BN_CLICKED(IDC_TODAYCOLOR, OnTodaycolor)
	ON_BN_CLICKED(IDC_DEFAULTPARENTCOLORS, OnChangeParentColoring)
	ON_BN_CLICKED(IDC_NOPARENTCOLOR, OnChangeParentColoring)
	ON_BN_CLICKED(IDC_SETPARENTCOLOR, OnSetParentColor)
	ON_BN_CLICKED(IDC_SPECIFYPARENTCOLOR, OnChangeParentColoring)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CGanttPreferencesDlg message handlers

COLORREF CGanttPreferencesDlg::GetTodayColor() const 
{ 
	return m_bSpecifyTodayColor ? m_crToday : NO_COLOR; 
}

COLORREF CGanttPreferencesDlg::GetWeekendColor() const 
{ 
	return m_bSpecifyWeekendColor ? m_crWeekend : NO_COLOR; 
}

void CGanttPreferencesDlg::OnSetWeekendcolor() 
{
	m_crWeekend = m_btWeekendColor.GetColor();
}

void CGanttPreferencesDlg::OnSetTodaycolor() 
{
	m_crToday = m_btTodayColor.GetColor();
}

void CGanttPreferencesDlg::OnWeekendcolor() 
{
	UpdateData();
	m_btWeekendColor.EnableWindow(m_bSpecifyWeekendColor);
}

void CGanttPreferencesDlg::OnTodaycolor() 
{
	UpdateData();
	m_btTodayColor.EnableWindow(m_bSpecifyTodayColor);
}

BOOL CGanttPreferencesDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	m_mgrGroupLines.AddGroupLine(IDC_COLORSGROUP, *this);
	m_mgrGroupLines.AddGroupLine(IDC_DATESGROUP, *this);

	m_btWeekendColor.EnableWindow(m_bSpecifyWeekendColor);
	m_btTodayColor.EnableWindow(m_bSpecifyTodayColor);
	m_btParentColor.EnableWindow(m_nParentColoring == 2);
	m_btWeekendColor.SetColor(m_crWeekend);
	m_btTodayColor.SetColor(m_crToday);
	m_btParentColor.SetColor(m_crParent);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CGanttPreferencesDlg::OnChangeParentColoring() 
{
	UpdateData();
	m_btParentColor.EnableWindow(m_nParentColoring == 2);
}

void CGanttPreferencesDlg::OnSetParentColor() 
{
	m_crParent = m_btParentColor.GetColor();
}

int CGanttPreferencesDlg::GetParentColoring(COLORREF& crParent) const
{
	crParent = m_crParent;
	return m_nParentColoring;
}

void CGanttPreferencesDlg::SavePreferences(IPreferences* pPrefs, LPCTSTR szKey) const
{
	pPrefs->WriteProfileInt(szKey, _T("DisplayAllocTo"), m_bDisplayAllocTo);
	pPrefs->WriteProfileInt(szKey, _T("AutoScrollSelection"), m_bAutoScrollSelection);
	pPrefs->WriteProfileInt(szKey, _T("AutoCalcParentDates"), m_bAutoCalcParentDates);
	pPrefs->WriteProfileInt(szKey, _T("CalculateMissingStartDates"), m_bCalculateMissingStartDates);
	pPrefs->WriteProfileInt(szKey, _T("CalculateMissingDueDates"), m_bCalculateMissingDueDates);
	pPrefs->WriteProfileInt(szKey, _T("TodayColor"), (int)m_crToday);
	pPrefs->WriteProfileInt(szKey, _T("WeekendColor"), (int)m_crWeekend);
	pPrefs->WriteProfileInt(szKey, _T("ParentColoring"), m_nParentColoring);
	pPrefs->WriteProfileInt(szKey, _T("ParentColor"), (int)m_crParent);
}

void CGanttPreferencesDlg::LoadPreferences(const IPreferences* pPrefs, LPCTSTR szKey) 
{
	m_bDisplayAllocTo = pPrefs->GetProfileInt(szKey, _T("DisplayAllocTo"), TRUE);
	m_bAutoScrollSelection = pPrefs->GetProfileInt(szKey, _T("AutoScrollSelection"), TRUE);
	m_bAutoCalcParentDates = pPrefs->GetProfileInt(szKey, _T("AutoCalcParentDates"), TRUE);
	m_bCalculateMissingStartDates = pPrefs->GetProfileInt(szKey, _T("CalculateMissingStartDates"), TRUE);
	m_bCalculateMissingDueDates = pPrefs->GetProfileInt(szKey, _T("CalculateMissingDueDates"), TRUE);
	m_bSpecifyTodayColor = pPrefs->GetProfileInt(szKey, _T("SpecifyTodayColor"), TRUE);
	m_crToday = (COLORREF)pPrefs->GetProfileInt(szKey, _T("TodayColor"), DEF_TODAYCOLOR);
	m_bSpecifyWeekendColor = pPrefs->GetProfileInt(szKey, _T("SpecifyWeekendColor"), TRUE);
	m_crWeekend = (COLORREF)pPrefs->GetProfileInt(szKey, _T("WeekendColor"), DEF_WEEKENDCOLOR);
	m_crParent = (COLORREF)pPrefs->GetProfileInt(szKey, _T("ParentColor"), DEF_PARENTCOLOR);
	m_nParentColoring = pPrefs->GetProfileInt(szKey, _T("ParentColoring"), 0);
}
