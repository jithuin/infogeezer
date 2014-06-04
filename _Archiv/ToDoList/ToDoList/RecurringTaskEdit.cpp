// RecurrenceEdit.cpp: implementation of the CRecurringTaskEdit class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "RecurringTaskEdit.h"
#include "resource.h"

#include "..\shared\datehelper.h"
#include "..\shared\dialoghelper.h"
#include "..\shared\autoflag.h"
#include "..\Shared\enstring.h"
#include "..\Shared\localizer.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

const int REBTN_OPTIONS = 1;

//////////////////////////////////////////////////////////////////////

CString CTDLRecurringTaskEdit::s_sOptionsBtnTip;

void CTDLRecurringTaskEdit::SetDefaultButtonTip(LPCTSTR szOption)
{
	if (szOption && *szOption)
		s_sOptionsBtnTip = szOption;
}

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CTDLRecurringTaskEdit::CTDLRecurringTaskEdit() : m_bInOnSetReadOnly(FALSE)
{
	CString sTip(s_sOptionsBtnTip.IsEmpty() ? _T("Options") : s_sOptionsBtnTip);
	AddButton(REBTN_OPTIONS, _T("..."), sTip);
}

CTDLRecurringTaskEdit::~CTDLRecurringTaskEdit()
{

}

BEGIN_MESSAGE_MAP(CTDLRecurringTaskEdit, CEnEdit)
	//{{AFX_MSG_MAP(CRecurringTaskEdit)
	//}}AFX_MSG_MAP
	ON_WM_SETCURSOR()
	ON_WM_CTLCOLOR_REFLECT()
	ON_MESSAGE(EM_SETREADONLY, OnSetReadOnly)
	ON_WM_STYLECHANGING()
	ON_CONTROL_REFLECT_EX(EN_CHANGE, OnReflectChangeDisplayText)
END_MESSAGE_MAP()
//////////////////////////////////////////////////////////////////////

void CTDLRecurringTaskEdit::PreSubclassWindow()
{
	CEnEdit::PreSubclassWindow();

	SetWindowText(GetRegularity(m_tr)); // for display purposes
}

BOOL CTDLRecurringTaskEdit::OnReflectChangeDisplayText()
{
	GetWindowText(m_tr.sRegularity);

	if (m_tr.nRegularity == TDIR_ONCE && IsDefaultString(m_tr.sRegularity))
		m_tr.sRegularity.Empty();

	return FALSE; // allow parent to process too
}

void CTDLRecurringTaskEdit::OnBtnClick(UINT nID)
{
	switch (nID)
	{
	case REBTN_OPTIONS:
		DoEdit();
		break;
	}
}

BOOL CTDLRecurringTaskEdit::DoEdit(BOOL bForce)
{
	if ((!m_bReadOnly && IsWindowEnabled()) || bForce)
	{
		CTDLRecurringTaskOptionDlg dialog(m_tr, m_dtDefault);
		
		if (dialog.DoModal() == IDOK)
		{
			TDIRECURRENCE tr;
			dialog.GetRecurrenceOptions(tr);
			
			if (m_tr != tr)
			{
				// reset display string if the units have changed
				if (m_tr.nRegularity != tr.nRegularity)
					m_tr.sRegularity.Empty();
				else
					// restore current text
					tr.sRegularity = m_tr.sRegularity;

				m_tr = tr;
				SetWindowText(GetRegularity(m_tr)); // for display purposes
				
				// notify parent
				GetParent()->SendMessage(WM_COMMAND, MAKEWPARAM(GetDlgCtrlID(), EN_CHANGE), (LPARAM)GetSafeHwnd());
			}

			return TRUE;
		}
	}

	// all else
	return FALSE;
}

BOOL CTDLRecurringTaskEdit::OnSetCursor(CWnd* /*pWnd*/, UINT /*nHitTest*/, UINT /*message*/) 
{
	::SetCursor(AfxGetApp()->LoadStandardCursor(IDC_ARROW));
	
	return TRUE;
}

HBRUSH CTDLRecurringTaskEdit::CtlColor(CDC* pDC, UINT /*nCtlColor*/) 
{
	HBRUSH hbr = NULL;
	pDC->SetBkMode(TRANSPARENT);

	if (!IsWindowEnabled() || m_bReadOnly)
		hbr = GetSysColorBrush(COLOR_3DFACE);
	else
		hbr = GetSysColorBrush(COLOR_WINDOW);

	return hbr;
}

void CTDLRecurringTaskEdit::GetRecurrenceOptions(TDIRECURRENCE& tr) const
{
	tr = m_tr;

	if (tr.sRegularity.IsEmpty())
		tr.sRegularity = GetRegularity(tr.nRegularity, FALSE);
}

void CTDLRecurringTaskEdit::SetRecurrenceOptions(const TDIRECURRENCE& tr)
{
	m_tr = tr;

	if (GetSafeHwnd())
		SetWindowText(GetRegularity(m_tr)); // for display purposes
}

BOOL CTDLRecurringTaskEdit::ModifyStyle(DWORD dwRemove, DWORD dwAdd, UINT /*nFlags*/)
{
	if ((dwRemove & ES_READONLY) != (dwAdd & ES_READONLY))
	{
		m_bReadOnly = (dwAdd & ES_READONLY);
		
		EnableButton(1, !m_bReadOnly);
		Invalidate();
	}

	// make sure we stay readonly
	return CEnEdit::ModifyStyle(dwRemove & ~ES_READONLY, dwAdd | ES_READONLY);
}

LRESULT CTDLRecurringTaskEdit::OnSetReadOnly(WPARAM wp, LPARAM /*lp*/)
{
	m_bReadOnly = wp;
	EnableButton(REBTN_OPTIONS, !m_bReadOnly);

	// always set to readonly
	// we set this flag so that OnStyleChanging doesn't respond to this trickery
	CAutoFlag af(m_bInOnSetReadOnly, TRUE);

	return DefWindowProc(EM_SETREADONLY, TRUE, 0);
}

void CTDLRecurringTaskEdit::OnStyleChanging(int nStyleType, LPSTYLESTRUCT lpStyleStruct)
{
	CEnEdit::OnStyleChanging(nStyleType, lpStyleStruct);

	if (nStyleType == GWL_STYLE && !m_bInOnSetReadOnly)
	{
		// check for change in readonly style
		if ((lpStyleStruct->styleOld & ES_READONLY) != (lpStyleStruct->styleNew & ES_READONLY))
		{
			m_bReadOnly = (lpStyleStruct->styleNew & ES_READONLY);
			lpStyleStruct->styleNew |= ES_READONLY; // make sure we stay readonly

			EnableButton(REBTN_OPTIONS, !m_bReadOnly);
			Invalidate();
		}
	}
}

CString CTDLRecurringTaskEdit::GetRegularity(const TDIRECURRENCE& tr, BOOL bIncOnce)
{
	if (tr.sRegularity.IsEmpty())
		return GetRegularity(tr.nRegularity, bIncOnce);
	
	// else
	return tr.sRegularity;
}

CString CTDLRecurringTaskEdit::GetRegularity(TDI_REGULARITY nRegularity, BOOL bIncOnce)
{
	CEnString sRegularity;

	switch (nRegularity)
	{
	case TDIR_ONCE:    
		if (bIncOnce)
			sRegularity.LoadString(IDS_ONCEONLY); 
		break;

	case TDIR_DAY_WEEKDAYS:
	case TDIR_DAY_RECREATEAFTERNDAYS:
	case TDIR_DAY_EVERY:   
		sRegularity.LoadString(IDS_DAILY);    
		break;

	case TDIR_WEEK_RECREATEAFTERNWEEKS:
	case TDIR_WEEK_EVERY:  
		sRegularity.LoadString(IDS_WEEKLY);   
		break;

	case TDIR_MONTH_SPECIFICDAY:
	case TDIR_MONTH_RECREATEAFTERNMONTHS:
	case TDIR_MONTH_EVERY: 
		sRegularity.LoadString(IDS_MONTHLY);  
		break;

	case TDIR_YEAR_SPECIFICDAYMONTH:
	case TDIR_YEAR_RECREATEAFTERNYEARS:
	case TDIR_YEAR_EVERY:  
		sRegularity.LoadString(IDS_YEARLY);   
		break;
	}

	return sRegularity;
}

BOOL CTDLRecurringTaskEdit::IsDefaultString(const CString& sRegularity)
{
	int nReg = (int)TDIR_YEAR_EVERY + 1;

	while (nReg--)
	{
		if (sRegularity.CompareNoCase(GetRegularity((TDI_REGULARITY)nReg, TRUE)) == 0)
			return TRUE;
	}

	// else
	return FALSE;
}

int CTDLRecurringTaskEdit::CalcMaxRegularityWidth(CDC* pDC, BOOL bIncOnce)
{
	int nReg = (int)TDIR_YEAR_EVERY + 1;
	int nMax = 0;

	while (nReg--)
	{
		CString sRegularity = GetRegularity((TDI_REGULARITY)nReg, bIncOnce);
		int nItem = pDC->GetTextExtent(sRegularity).cx;
		nMax = max(nItem, nMax);
	}

	return nMax;
}

/////////////////////////////////////////////////////////////////////////////
// CRecurringTaskOptionDlg dialog

#define WM_VALUECHANGE (WM_APP+1)

CTDLRecurringTaskOptionDlg::CTDLRecurringTaskOptionDlg(const TDIRECURRENCE& tr, const COleDateTime& dtDefault, CWnd* pParent /*=NULL*/)
	: CDialog(IDD_RECURRING_TASK_DIALOG, pParent),
	m_pageDaily(tr, dtDefault),
	m_pageWeekly(tr, dtDefault),
	m_pageMonthly(tr, dtDefault),
	m_pageYearly(tr, dtDefault)
{
	//{{AFX_DATA_INIT(CRecurringTaskOptionDlg)
	//}}AFX_DATA_INIT
	
	switch (tr.nRegularity)
	{
	case TDIR_ONCE:    
		m_nFrequency = TDIR_ONCE;
		break;

	case TDIR_DAY_WEEKDAYS:
	case TDIR_DAY_RECREATEAFTERNDAYS:
	case TDIR_DAY_EVERY:   
		m_nFrequency = TDIR_DAILY;    
		break;

	case TDIR_WEEK_RECREATEAFTERNWEEKS:
	case TDIR_WEEK_EVERY:  
		m_nFrequency = TDIR_WEEKLY;    
		break;

	case TDIR_MONTH_SPECIFICDAY:
	case TDIR_MONTH_RECREATEAFTERNMONTHS:
	case TDIR_MONTH_EVERY: 
		m_nFrequency = TDIR_MONTHLY;    
		break;

	case TDIR_YEAR_SPECIFICDAYMONTH:
	case TDIR_YEAR_RECREATEAFTERNYEARS:
	case TDIR_YEAR_EVERY:  
		m_nFrequency = TDIR_YEARLY;
		break;
	}

	m_nRecalcFrom = tr.nRecalcFrom;
	m_nReuse = tr.nReuse;

	m_ppHost.AddPage(&m_pageDaily);
	m_ppHost.AddPage(&m_pageWeekly);
	m_ppHost.AddPage(&m_pageMonthly);
	m_ppHost.AddPage(&m_pageYearly);
}


void CTDLRecurringTaskOptionDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CRecurringTaskOptionDlg)
	DDX_CBIndex(pDX, IDC_RECALCFROM, m_nRecalcFrom);
	DDX_CBIndex(pDX, IDC_REUSEOPTIONS, m_nReuse);
	//}}AFX_DATA_MAP
	DDX_CBIndex(pDX, IDC_FREQUENCY, (int&)m_nFrequency);
}

BEGIN_MESSAGE_MAP(CTDLRecurringTaskOptionDlg, CDialog)
	//{{AFX_MSG_MAP(CRecurringTaskOptionDlg)
	//}}AFX_MSG_MAP
//	ON_MESSAGE(WM_VALUECHANGE, OnValueChange)
	ON_CBN_SELCHANGE(IDC_FREQUENCY, OnSelchangeFrequency)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CRecurringTaskOptionDlg message handlers
/////////////////////////////////////////////////////////////////////////////

void CTDLRecurringTaskOptionDlg::GetRecurrenceOptions(TDIRECURRENCE& tr) const
{
	tr.nRecalcFrom = (TDI_RECURFROMOPTION)m_nRecalcFrom;
	tr.nReuse = (TDI_RECURREUSEOPTION)m_nReuse;

	switch (m_nFrequency)
	{
	case TDIR_DAILY:
		m_pageDaily.GetRecurrenceOptions(tr);
		break;

	case TDIR_WEEKLY:
		m_pageWeekly.GetRecurrenceOptions(tr);
		break;

	case TDIR_MONTHLY:
		m_pageMonthly.GetRecurrenceOptions(tr);
		break;

	case TDIR_YEARLY:
		m_pageYearly.GetRecurrenceOptions(tr);
		break;
	}
}

void CTDLRecurringTaskOptionDlg::OnSelchangeFrequency() 
{
	UpdateData();

	BOOL bOnce = (m_nFrequency == TDIR_ONCE);

	GetDlgItem(IDC_REUSEOPTIONS)->EnableWindow(!bOnce);
	GetDlgItem(IDC_RECALCFROM)->EnableWindow(!bOnce);

	m_ppHost.ShowWindow(bOnce ? SW_HIDE : SW_SHOW);
	m_ppHost.SetActivePage(m_nFrequency - 1, FALSE);
}

BOOL CTDLRecurringTaskOptionDlg::OnInitDialog()  
{
	CDialog::OnInitDialog();

	// create propertypage host
	m_ppHost.Create(IDC_PAGE_FRAME, this);

	OnSelchangeFrequency();
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLRecurringTaskOptionDlg::OnOK()  
{
	CDialog::OnOK();

	m_ppHost.OnOK();
}

/////////////////////////////////////////////////////////////////////////////
// CTDLRecurringTaskDailyOptionPage property page

CTDLRecurringTaskDailyOptionPage::CTDLRecurringTaskDailyOptionPage(const TDIRECURRENCE& tr, const COleDateTime& /*dtDefault*/) 
	: CPropertyPage(IDD_RECURRING_DAILY_PAGE)
{
	//{{AFX_DATA_INIT(CTDLRecurringTaskDailyOptionPage)
	m_nEveryNumDays = 1;
	m_RecreateAfterNumDays = 1;
	m_nDailyOption = 0;
	//}}AFX_DATA_INIT
	
	// overwrite specific values
	switch (tr.nRegularity)
	{
	case TDIR_DAY_EVERY: 
		m_nDailyOption = 0;
		m_nEveryNumDays = tr.dwSpecific1;
		break;

	case TDIR_DAY_WEEKDAYS:
		m_nDailyOption = 1;
		break;

	case TDIR_DAY_RECREATEAFTERNDAYS:
		m_nDailyOption = 2;
		m_RecreateAfterNumDays = tr.dwSpecific1;
		break;
	}
}

CTDLRecurringTaskDailyOptionPage::~CTDLRecurringTaskDailyOptionPage()
{
}

void CTDLRecurringTaskDailyOptionPage::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLRecurringTaskDailyOptionPage)
	CDialogHelper::DDX_Text(pDX, IDC_EVERYDAYS, m_nEveryNumDays);
	CDialogHelper::DDX_Text(pDX, IDC_RECREATEDAYS, m_RecreateAfterNumDays);
	DDX_Radio(pDX, IDC_EVERY, m_nDailyOption);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLRecurringTaskDailyOptionPage, CPropertyPage)
	//{{AFX_MSG_MAP(CTDLRecurringTaskDailyOptionPage)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
	ON_EN_CHANGE(IDC_EVERYDAYS, OnChangeDailyValues)
	ON_EN_CHANGE(IDC_RECREATEDAYS, OnChangeDailyValues)
	ON_BN_CLICKED(IDC_EVERY, OnChangeDailyValues)
	ON_BN_CLICKED(IDC_WEEKDAYS, OnChangeDailyValues)
	ON_BN_CLICKED(IDC_RECREATE, OnChangeDailyValues)
END_MESSAGE_MAP()

BOOL CTDLRecurringTaskDailyOptionPage::OnInitDialog()  
{
	CPropertyPage::OnInitDialog();

	UpdateData(FALSE);
	OnChangeDailyValues(); // to enable options

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLRecurringTaskDailyOptionPage::GetRecurrenceOptions(TDIRECURRENCE& tr) const
{
	switch (m_nDailyOption)
	{
	case 0:   
		tr.nRegularity = TDIR_DAY_EVERY;
		tr.dwSpecific1 = m_nEveryNumDays;
		break;

	case 1:
		tr.nRegularity = TDIR_DAY_WEEKDAYS;
		break;

	case 2:
		tr.nRegularity = TDIR_DAY_RECREATEAFTERNDAYS;
		tr.dwSpecific1 = m_RecreateAfterNumDays;
		break;
	}
}

void CTDLRecurringTaskDailyOptionPage::OnChangeDailyValues()
{
	UpdateData();

 	GetDlgItem(IDC_EVERYDAYS)->EnableWindow(m_nDailyOption == 0);
 	GetDlgItem(IDC_RECREATEDAYS)->EnableWindow(m_nDailyOption == 2);
}

/////////////////////////////////////////////////////////////////////////////
// CTDLRecurringTaskWeeklyOptionPage property page

CTDLRecurringTaskWeeklyOptionPage::CTDLRecurringTaskWeeklyOptionPage(const TDIRECURRENCE& tr, const COleDateTime& dtDefault) 
	: CPropertyPage(IDD_RECURRING_WEEKLY_PAGE)
{
	//{{AFX_DATA_INIT(CTDLRecurringTaskWeeklyOptionPage)
	m_nWeeklyOption = 0;
	m_nEveryNumWeeks = 1;
	m_nRecreateAfterNumWeeks = 1;
	//}}AFX_DATA_INIT

	// first set to default values
	if (dtDefault.m_dt > 0.0)
	{
		SYSTEMTIME stDefault;
		dtDefault.GetAsSystemTime(stDefault);

		m_dwWeekdays = 2 << (stDefault.wDayOfWeek - 1);
	}
	
	// overwrite specific values
	switch (tr.nRegularity)
	{
	case TDIR_WEEK_EVERY:
		m_nWeeklyOption = 0;
		m_nEveryNumWeeks = tr.dwSpecific1;
		m_dwWeekdays = tr.dwSpecific2;
		break;

	case TDIR_WEEK_RECREATEAFTERNWEEKS:
		m_nWeeklyOption = 1;
		m_nRecreateAfterNumWeeks = tr.dwSpecific1;
		m_dwWeekdays = tr.dwSpecific2;
		break;
	}
}

CTDLRecurringTaskWeeklyOptionPage::~CTDLRecurringTaskWeeklyOptionPage()
{
}

void CTDLRecurringTaskWeeklyOptionPage::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLRecurringTaskWeeklyOptionPage)
	DDX_Control(pDX, IDC_WEEKDAYS, m_lbWeekdays);
	DDX_Radio(pDX, IDC_EVERY, m_nWeeklyOption);
	CDialogHelper::DDX_Text(pDX, IDC_EVERYNWEEKS, m_nEveryNumWeeks);
	CDialogHelper::DDX_Text(pDX, IDC_RECREATEWEEKS, m_nRecreateAfterNumWeeks);
	//}}AFX_DATA_MAP

	if (pDX->m_bSaveAndValidate)
		m_dwWeekdays = m_lbWeekdays.GetChecked();
	else
		m_lbWeekdays.SetChecked(m_dwWeekdays);
}


BEGIN_MESSAGE_MAP(CTDLRecurringTaskWeeklyOptionPage, CPropertyPage)
	//{{AFX_MSG_MAP(CTDLRecurringTaskWeeklyOptionPage)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
	ON_EN_CHANGE(IDC_EVERYNWEEKS, OnChangeWeeklyValues)
	ON_EN_CHANGE(IDC_RECREATEWEEKS, OnChangeWeeklyValues)
	ON_CLBN_CHKCHANGE(IDC_WEEKDAYS, OnChangeWeeklyValues)
	ON_BN_CLICKED(IDC_EVERY, OnChangeWeeklyValues)
	ON_BN_CLICKED(IDC_RECREATE, OnChangeWeeklyValues)
END_MESSAGE_MAP()

BOOL CTDLRecurringTaskWeeklyOptionPage::OnInitDialog()  
{
	CPropertyPage::OnInitDialog();

	CLocalizer::EnableTranslation(m_lbWeekdays, FALSE);
	
	// init weekdays
	m_lbWeekdays.SetChecked(m_dwWeekdays);

	UpdateData(FALSE);
	OnChangeWeeklyValues(); // enable states

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLRecurringTaskWeeklyOptionPage::GetRecurrenceOptions(TDIRECURRENCE& tr) const
{
	switch (m_nWeeklyOption)
	{
	case 0:  
		tr.nRegularity = TDIR_WEEK_EVERY;
		tr.dwSpecific1 = m_nEveryNumWeeks;
		tr.dwSpecific2 = m_dwWeekdays;
		break;

	case 1:
		tr.nRegularity = TDIR_WEEK_RECREATEAFTERNWEEKS;
		tr.dwSpecific1 = m_nRecreateAfterNumWeeks;
		tr.dwSpecific2 = m_dwWeekdays;
		break;
	}
}

void CTDLRecurringTaskWeeklyOptionPage::OnChangeWeeklyValues()
{
	UpdateData();

 	GetDlgItem(IDC_EVERYNWEEKS)->EnableWindow(m_nWeeklyOption == 0);
	GetDlgItem(IDC_WEEKDAYS)->EnableWindow(m_nWeeklyOption == 0);
  	GetDlgItem(IDC_RECREATEWEEKS)->EnableWindow(m_nWeeklyOption == 1);
}

/////////////////////////////////////////////////////////////////////////////
// CTDLRecurringTaskMonthlyOptionPage property page

CTDLRecurringTaskMonthlyOptionPage::CTDLRecurringTaskMonthlyOptionPage(const TDIRECURRENCE& tr, const COleDateTime& dtDefault) 
	: CPropertyPage(IDD_RECURRING_MONTHLY_PAGE)
{
	//{{AFX_DATA_INIT(CTDLRecurringTaskMonthlyOptionPage)
	m_nEveryDayOfMonth = 1;
	m_nEveryNumMonths = 1;
	m_nSpecificNumber = 0;
	m_nSpecificDayOfWeek = 0;
	m_nSpecificNumMonths = 1;
	m_nRecreateAfterNumMonths = 1;
	m_nMonthlyOption = 0;
	//}}AFX_DATA_INIT

	// first set to default values
	if (dtDefault.m_dt > 0.0)
	{
		SYSTEMTIME stDefault;
		dtDefault.GetAsSystemTime(stDefault);

		m_nEveryDayOfMonth = stDefault.wDay;
		m_nEveryNumMonths = stDefault.wMonth - 1;
	}
	
	// then overwrite specific values
	switch (tr.nRegularity)
	{
	case TDIR_MONTH_EVERY: 
		m_nMonthlyOption = 0;
		m_nEveryNumMonths = tr.dwSpecific1;
		m_nEveryDayOfMonth = tr.dwSpecific2;
		break;

	case TDIR_MONTH_SPECIFICDAY:
		m_nMonthlyOption = 1;
		m_nSpecificNumber = LOWORD(tr.dwSpecific1) - 1;
		m_nSpecificDayOfWeek = HIWORD(tr.dwSpecific1) - 1;
		m_nSpecificNumMonths = tr.dwSpecific2;
		break;

	case TDIR_MONTH_RECREATEAFTERNMONTHS:
		m_nMonthlyOption = 2;
		m_nRecreateAfterNumMonths = tr.dwSpecific1;
		break;
	}

}

CTDLRecurringTaskMonthlyOptionPage::~CTDLRecurringTaskMonthlyOptionPage()
{
}

void CTDLRecurringTaskMonthlyOptionPage::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLRecurringTaskMonthlyOptionPage)
	DDX_Control(pDX, IDC_THEWEEKDAY, m_cbWeekdays);
	DDX_CBIndex(pDX, IDC_THESPECIFICWEEK, m_nSpecificNumber);
	DDX_CBIndex(pDX, IDC_THEWEEKDAY, m_nSpecificDayOfWeek);
	DDX_Radio(pDX, IDC_DAY, m_nMonthlyOption);
	CDialogHelper::DDX_Text(pDX, IDC_DAYMONTHDAY, m_nEveryDayOfMonth);
	CDialogHelper::DDX_Text(pDX, IDC_DAYMONTHS, m_nEveryNumMonths);
	CDialogHelper::DDX_Text(pDX, IDC_THEMONTHS, m_nSpecificNumMonths);
	CDialogHelper::DDX_Text(pDX, IDC_RECREATEMONTHS, m_nRecreateAfterNumMonths);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLRecurringTaskMonthlyOptionPage, CPropertyPage)
	//{{AFX_MSG_MAP(CTDLRecurringTaskMonthlyOptionPage)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
	ON_EN_CHANGE(IDC_DAYMONTHDAY, OnChangeMonthlyValues)
	ON_EN_CHANGE(IDC_DAYMONTHS, OnChangeMonthlyValues)
	ON_EN_CHANGE(IDC_THEMONTHS, OnChangeMonthlyValues)
	ON_EN_CHANGE(IDC_RECREATEMONTHS, OnChangeMonthlyValues)
	ON_CBN_SELCHANGE(IDC_THESPECIFICWEEK, OnChangeMonthlyValues)
	ON_CBN_SELCHANGE(IDC_THEWEEKDAY, OnChangeMonthlyValues)
	ON_BN_CLICKED(IDC_DAY, OnChangeMonthlyValues)
	ON_BN_CLICKED(IDC_SPECIFIC, OnChangeMonthlyValues)
	ON_BN_CLICKED(IDC_RECREATE, OnChangeMonthlyValues)
END_MESSAGE_MAP()

BOOL CTDLRecurringTaskMonthlyOptionPage::OnInitDialog()  
{
	CPropertyPage::OnInitDialog();

	UpdateData(FALSE);
	OnChangeMonthlyValues(); // enable states
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLRecurringTaskMonthlyOptionPage::GetRecurrenceOptions(TDIRECURRENCE& tr) const
{
	switch (m_nMonthlyOption)
	{
	case 0: 
		tr.nRegularity = TDIR_MONTH_EVERY;
		tr.dwSpecific1 = m_nEveryNumMonths;
		tr.dwSpecific2 = m_nEveryDayOfMonth;
		break;

	case 1:
		tr.nRegularity = TDIR_MONTH_SPECIFICDAY;
		tr.dwSpecific1 = MAKELONG(m_nSpecificNumber + 1, m_nSpecificDayOfWeek + 1);
		tr.dwSpecific2 = m_nSpecificNumMonths;
		break;

	case 2:
		tr.nRegularity = TDIR_MONTH_RECREATEAFTERNMONTHS;
		tr.dwSpecific1 = m_nRecreateAfterNumMonths;
		tr.dwSpecific2 = 0;
		break;

	}
}

void CTDLRecurringTaskMonthlyOptionPage::OnChangeMonthlyValues()
{
	UpdateData();

 	GetDlgItem(IDC_DAYMONTHDAY)->EnableWindow(m_nMonthlyOption == 0);
 	GetDlgItem(IDC_DAYMONTHS)->EnableWindow(m_nMonthlyOption == 0);
 	GetDlgItem(IDC_THEMONTHS)->EnableWindow(m_nMonthlyOption == 1);
 	GetDlgItem(IDC_THESPECIFICWEEK)->EnableWindow(m_nMonthlyOption == 1);
 	GetDlgItem(IDC_THEWEEKDAY)->EnableWindow(m_nMonthlyOption == 1);
 	GetDlgItem(IDC_RECREATEMONTHS)->EnableWindow(m_nMonthlyOption == 2);
}

/////////////////////////////////////////////////////////////////////////////
// CTDLRecurringTaskYearlyOptionPage property page

CTDLRecurringTaskYearlyOptionPage::CTDLRecurringTaskYearlyOptionPage(const TDIRECURRENCE& tr, const COleDateTime& dtDefault) 
	: CPropertyPage(IDD_RECURRING_YEARLY_PAGE)
{
	//{{AFX_DATA_INIT(CTDLRecurringTaskYearlyOptionPage)
	m_nYearlyOption = 0;
	m_nEveryDayOfMonth = 1;
	m_nSpecificNumber = 0;
	m_nRecreateAfterNumYears = 1;
	m_nSpecificMonth = 0;
	m_nSpecificDayOfWeek = 0;
	m_nEveryMonth = 0;
	//}}AFX_DATA_INIT

	// first set to default values
	if (dtDefault.m_dt > 0.0)
	{
		SYSTEMTIME stDefault;
		dtDefault.GetAsSystemTime(stDefault);

		m_nEveryMonth = stDefault.wMonth - 1;
		m_nEveryDayOfMonth = stDefault.wDay;
	}
	
	// then overwrite specific values
	switch (tr.nRegularity)
	{
	case TDIR_YEAR_EVERY:  
		m_nYearlyOption = 0;
		m_nEveryMonth = tr.dwSpecific1 - 1;
		m_nEveryDayOfMonth = tr.dwSpecific2;
		break;

	case TDIR_YEAR_SPECIFICDAYMONTH:
		m_nYearlyOption = 1;
		m_nSpecificNumber = LOWORD(tr.dwSpecific1) - 1;
		m_nSpecificDayOfWeek = HIWORD(tr.dwSpecific1) - 1;
		m_nSpecificMonth = tr.dwSpecific2 - 1;
		break;

	case TDIR_YEAR_RECREATEAFTERNYEARS:
		m_nYearlyOption = 2;
		m_nRecreateAfterNumYears = tr.dwSpecific1;
		break;
	}
}


CTDLRecurringTaskYearlyOptionPage::~CTDLRecurringTaskYearlyOptionPage()
{
}

void CTDLRecurringTaskYearlyOptionPage::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLRecurringTaskYearlyOptionPage)
	DDX_Radio(pDX, IDC_EVERY, m_nYearlyOption);
	CDialogHelper::DDX_Text(pDX, IDC_RECREATEYEARS, m_nRecreateAfterNumYears);
	CDialogHelper::DDX_Text(pDX, IDC_EVERYMONTHDAY, m_nEveryDayOfMonth);
	DDX_CBIndex(pDX, IDC_THESPECIFICWEEK, m_nSpecificNumber);
	DDX_CBIndex(pDX, IDC_THEMONTH, m_nSpecificMonth);
	DDX_CBIndex(pDX, IDC_THEWEEKDAY, m_nSpecificDayOfWeek);
	DDX_CBIndex(pDX, IDC_EVERYMONTHLIST, m_nEveryMonth);
	DDX_Control(pDX, IDC_THEMONTH, m_cbSpecificMonthList);
	DDX_Control(pDX, IDC_THEWEEKDAY, m_cbDaysOfWeek);
	DDX_Control(pDX, IDC_EVERYMONTHLIST, m_cbEveryMonthList);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CTDLRecurringTaskYearlyOptionPage, CPropertyPage)
	//{{AFX_MSG_MAP(CTDLRecurringTaskYearlyOptionPage)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
 	ON_CBN_SELCHANGE(IDC_THESPECIFICWEEK, OnChangeYearlyValues)
 	ON_CBN_SELCHANGE(IDC_THEWEEKDAY, OnChangeYearlyValues)
 	ON_CBN_SELCHANGE(IDC_THEMONTH, OnChangeYearlyValues)
 	ON_CBN_SELCHANGE(IDC_EVERYMONTHLIST, OnChangeYearlyValues)
 	ON_EN_CHANGE(IDC_RECREATEYEARS, OnChangeYearlyValues)
 	ON_EN_CHANGE(IDC_EVERYMONTHDAY, OnChangeYearlyValues)
	ON_BN_CLICKED(IDC_EVERY, OnChangeYearlyValues)
	ON_BN_CLICKED(IDC_SPECIFIC, OnChangeYearlyValues)
	ON_BN_CLICKED(IDC_RECREATE, OnChangeYearlyValues)
END_MESSAGE_MAP()

BOOL CTDLRecurringTaskYearlyOptionPage::OnInitDialog()  
{
	CPropertyPage::OnInitDialog();

	UpdateData(FALSE);
	OnChangeYearlyValues();

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLRecurringTaskYearlyOptionPage::GetRecurrenceOptions(TDIRECURRENCE& tr) const
{
	switch (m_nYearlyOption)
	{
	case 0:  
		tr.nRegularity = TDIR_YEAR_EVERY;
		tr.dwSpecific1 = m_nEveryMonth + 1;
		tr.dwSpecific2 = m_nEveryDayOfMonth;
		break;

	case 1:
		tr.nRegularity = TDIR_YEAR_SPECIFICDAYMONTH;
		tr.dwSpecific1 = MAKELONG(m_nSpecificNumber + 1, m_nSpecificDayOfWeek + 1);
		tr.dwSpecific2 = m_nSpecificMonth + 1;
		break;

	case 2:
		tr.nRegularity = TDIR_YEAR_RECREATEAFTERNYEARS;
		tr.dwSpecific1 = m_nRecreateAfterNumYears;
		tr.dwSpecific2 = 0;
		break;
	}
}

void CTDLRecurringTaskYearlyOptionPage::OnChangeYearlyValues()
{
	UpdateData();

 	GetDlgItem(IDC_EVERYMONTHLIST)->EnableWindow(m_nYearlyOption == 0);
 	GetDlgItem(IDC_EVERYMONTHDAY)->EnableWindow(m_nYearlyOption == 0);
 	GetDlgItem(IDC_THESPECIFICWEEK)->EnableWindow(m_nYearlyOption == 1);
 	GetDlgItem(IDC_THEWEEKDAY)->EnableWindow(m_nYearlyOption == 1);
 	GetDlgItem(IDC_THEMONTH)->EnableWindow(m_nYearlyOption == 1);
 	GetDlgItem(IDC_RECREATEYEARS)->EnableWindow(m_nYearlyOption == 2);
}

