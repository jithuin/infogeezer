// PreferencesPageBase.cpp : implementation file
//

#include "stdafx.h"
#include "PreferencesBase.h"
#include "dialoghelper.h"
#include "graphicsmisc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPreferencesPageBase property page

IMPLEMENT_DYNCREATE(CPreferencesPageBase, CPropertyPage)

CPreferencesPageBase::CPreferencesPageBase(UINT nID) : CPropertyPage(nID), m_brush(NULL), m_crback((COLORREF)-1)
{
}

CPreferencesPageBase::~CPreferencesPageBase()
{
	GraphicsMisc::VerifyDeleteObject(m_brush);
}

BEGIN_MESSAGE_MAP(CPreferencesPageBase, CPropertyPage)
//{{AFX_MSG_MAP(CPreferencesPageBase)
//}}AFX_MSG_MAP
	ON_CONTROL_RANGE(BN_CLICKED, 0, 0xffff, OnControlChange)
	ON_CONTROL_RANGE(EN_CHANGE, 0, 0xffff, OnControlChange)
	ON_CONTROL_RANGE(CBN_EDITCHANGE, 0, 0xffff, OnControlChange)
	ON_CONTROL_RANGE(CBN_SELCHANGE, 0, 0xffff, OnControlChange)
	ON_CONTROL_RANGE(CLBN_CHKCHANGE, 0, 0xffff, OnControlChange)
	ON_WM_CTLCOLOR()
	ON_WM_ERASEBKGND()
END_MESSAGE_MAP()

CWnd* CPreferencesPageBase::GetDlgItem(UINT nID) const
{
	static CWnd wnd;
	CWnd* pWnd = CDialog::GetDlgItem(nID);

	if (pWnd)
		return pWnd;
	
	// else
	wnd.m_hWnd = NULL;
	return &wnd;
}

BOOL CPreferencesPageBase::OnEraseBkgnd(CDC* pDC)
{
	if (m_brush == NULL)
		return CPropertyPage::OnEraseBkgnd(pDC);

	//CDialogHelper::ExcludeChildren(this, pDC);

	// else
	CRect rClient;
	GetClientRect(rClient);
	pDC->FillSolidRect(rClient, m_crback);

	return TRUE;
}

HBRUSH CPreferencesPageBase::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);

	if (nCtlColor == CTLCOLOR_STATIC && m_brush != NULL)
	{
		pDC->SetBkMode(TRANSPARENT);
		hbr = m_brush;
	}
	
	return hbr;
}

void CPreferencesPageBase::OnControlChange(UINT /*nID*/)
{
	if (IsWindowVisible())
		GetParent()->SendMessage(WM_PPB_CTRLCHANGE);
}

void CPreferencesPageBase::SetBackgroundColor(COLORREF color)
{
	if (color == m_crback)
		return;

	m_crback = color;

	GraphicsMisc::VerifyDeleteObject(m_brush);

	if (color != (COLORREF)-1)
		m_brush = ::CreateSolidBrush(color);

	if (GetSafeHwnd())
		Invalidate(TRUE);
}


/////////////////////////////////////////////////////////////////////////////
// CPreferencesDlgBase dialog


CPreferencesDlgBase::CPreferencesDlgBase(UINT nID, CWnd* pParent) : CDialog(nID, pParent), m_nInitPage(-1)
{
}

BEGIN_MESSAGE_MAP(CPreferencesDlgBase, CDialog)
//{{AFX_MSG_MAP(CPreferencesDlg)
ON_WM_DESTROY()
//}}AFX_MSG_MAP
END_MESSAGE_MAP()

void CPreferencesDlgBase::OnOK()
{
	CDialog::OnOK();
	
	m_pphost.OnOK();
	SavePreferences();
}

void CPreferencesDlgBase::OnApply()
{
	m_pphost.OnApply();
	SavePreferences();
}

int CPreferencesDlgBase::DoModal(int nInitPage)
{
	if (nInitPage != -1)
		m_nInitPage = nInitPage;
	
	return CDialog::DoModal();
}

BOOL CPreferencesDlgBase::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// restore previous size
	CRect rWindow;
	GetWindowRect(rWindow);

	rWindow.bottom = rWindow.top + m_prefs.GetProfileInt(_T("Preferences\\DialogState"), _T("Height"), rWindow.Height());
	rWindow.right = rWindow.left + m_prefs.GetProfileInt(_T("Preferences\\DialogState"), _T("Width"), rWindow.Width());

	MoveWindow(rWindow);
	CenterWindow();

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CPreferencesDlgBase::CreatePPHost(UINT nHostID)
{
	if (GetDlgItem(nHostID))
	{
		CRect rPPHost;
		GetDlgItem(nHostID)->GetWindowRect(rPPHost);
		ScreenToClient(rPPHost);
		
		return CreatePPHost(rPPHost);
	}
	
	// else
	return FALSE;
}

BOOL CPreferencesDlgBase::CreatePPHost(LPRECT pRect)
{
	if (m_pphost.Create(pRect, this))
	{
		if (m_nInitPage > 0 && m_nInitPage < m_pphost.GetPageCount())
			return SetActivePage(m_nInitPage);
		
		// else
		return TRUE;
	}
	
	// else
	return FALSE;
}

BOOL CPreferencesDlgBase::SetActivePage(int nPage)
{
	CPropertyPage* pPage = m_pphost.GetPage(nPage);
	ASSERT (pPage);
	
	return m_pphost.SetActivePage(pPage);
}

BOOL CPreferencesDlgBase::SetActivePage(CPreferencesPageBase* pPage)
{
	if (pPage->IsKindOf(RUNTIME_CLASS(CPreferencesPageBase)))
		return m_pphost.SetActivePage(pPage);
	
	// else
	return FALSE;
}

BOOL CPreferencesDlgBase::AddPage(CPreferencesPageBase* pPage)
{
	if (pPage->IsKindOf(RUNTIME_CLASS(CPreferencesPageBase)))
		return m_pphost.AddPage(pPage);
	
	// else
	return FALSE;
}

void CPreferencesDlgBase::OnDestroy() 
{
	CDialog::OnDestroy();
}

void CPreferencesDlgBase::Initialize(CPreferences& prefs)
{
	LoadPreferences(prefs); // this initializes the dialog data
	SavePreferences(prefs); // this writes it back to the prefs
}

void CPreferencesDlgBase::LoadPreferences()
{
	LoadPreferences(m_prefs);
}

void CPreferencesDlgBase::LoadPreferences(CPreferences& prefs)
{
	// cycle the page loading the preferences for each one
	int nPage = m_pphost.GetPageCount();
	
	while (nPage--)
	{
		CPreferencesPageBase* pPage = (CPreferencesPageBase*)m_pphost.GetPage(nPage);
		
		if (pPage->IsKindOf(RUNTIME_CLASS(CPreferencesPageBase)))
			pPage->LoadPreferences(prefs);
	}
	
	// initial page
	if (m_nInitPage < 0 || m_nInitPage >= m_pphost.GetPageCount())
		m_nInitPage = prefs.GetProfileInt(_T("Preferences\\DialogState"), _T("StartPage"), 0);
}

void CPreferencesDlgBase::SavePreferences()
{
	SavePreferences(m_prefs);

	// notify parent
	GetParent()->SendMessage(WM_PDB_ONAPPLY);
}

void CPreferencesDlgBase::SavePreferences(CPreferences& prefs)
{
	// cycle the page saving the preferences for each one
	int nPage = m_pphost.GetPageCount();
	
	while (nPage--)
	{
		CPreferencesPageBase* pPage = (CPreferencesPageBase*)m_pphost.GetPage(nPage);
		
		if (pPage->IsKindOf(RUNTIME_CLASS(CPreferencesPageBase)))
			pPage->SavePreferences(prefs);
	}
	
	if (GetSafeHwnd())
	{
		CRect rWindow;
		GetWindowRect(rWindow);

		prefs.WriteProfileInt(_T("Preferences\\DialogState"), _T("Height"), rWindow.Height());
		prefs.WriteProfileInt(_T("Preferences\\DialogState"), _T("Width"), rWindow.Width());
		prefs.WriteProfileInt(_T("Preferences\\DialogState"), _T("StartPage"), m_pphost.GetActiveIndex());
	}

	prefs.Save();
}

/////////////////////////////////////////////////////////////////////////////
