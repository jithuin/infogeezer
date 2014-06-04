// PreferencesUITasklistColorsPage.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "PreferencesUITasklistColorsPage.h"

#include "..\shared\colordef.h"
#include "..\shared\dialoghelper.h"
#include "..\shared\enstring.h"
#include "..\shared\localizer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CPreferencesUITasklistColorsPage property page

// default priority colors
const COLORREF PRIORITYLOWCOLOR = RGB(30, 225, 0);
const COLORREF PRIORITYHIGHCOLOR = RGB(255, 0, 0);
const COLORREF CBMASKCOLOR = RGB(255, 0, 0);
const COLORREF ALTERNATELINECOLOR = RGB(230, 230, 255);
const COLORREF GRIDLINECOLOR = RGB(192, 192, 192);
const COLORREF TASKDONECOLOR = RGB(128, 128, 128);
const COLORREF TASKDUECOLOR = RGB(255, 0, 0);
const COLORREF TASKSTARTCOLOR = RGB(0, 255, 0);
const COLORREF FILTEREDCOLOR = RGB(200, 200, 200);
const COLORREF FLAGGEDCOLOR = RGB(128, 64, 0);
const COLORREF REFERENCECOLOR = RGB(128, 0, 64);

IMPLEMENT_DYNCREATE(CPreferencesUITasklistColorsPage, CPreferencesPageBase)

CPreferencesUITasklistColorsPage::CPreferencesUITasklistColorsPage() : 
	CPreferencesPageBase(CPreferencesUITasklistColorsPage::IDD),
	m_nTextColorOption(COLOROPT_DEFAULT), m_cbAttributes(TRUE), m_nColorAttribute(TDCA_NONE)

{
	//{{AFX_DATA_INIT(CPreferencesUITasklistColorsPage)
	//}}AFX_DATA_INIT
	// priority colors
	m_nSelPriorityColor = 0; 
}

CPreferencesUITasklistColorsPage::~CPreferencesUITasklistColorsPage()
{
}

void CPreferencesUITasklistColorsPage::DoDataExchange(CDataExchange* pDX)
{
	CPreferencesPageBase::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPreferencesUITasklistColorsPage)
	DDX_Control(pDX, IDC_ATTRIBUTETOCOLORBY, m_cbColorByAttribute);
	DDX_Check(pDX, IDC_COLORTASKBKGND, m_bColorTaskBackground);
	DDX_Check(pDX, IDC_COMMENTSUSETREEFONT, m_bCommentsUseTreeFont);
	DDX_Check(pDX, IDC_USEHLSGRADIENT, m_bHLSColorGradient);
	DDX_Check(pDX, IDC_HIDEPRIORITYNUMBER, m_bHidePriorityNumber);
	DDX_Check(pDX, IDC_ALTERNATELINECOLOR, m_bAlternateLineColor);
	//}}AFX_DATA_MAP
	DDX_CBString(pDX, IDC_ATTRIBUTECOLORS, m_sSelAttrib);
	DDX_Control(pDX, IDC_SETATTRIBUTECOLOR, m_btAttribColor);
	DDX_Control(pDX, IDC_ATTRIBUTECOLORS, m_cbAttributes);
	DDX_Radio(pDX, IDC_COLORTEXTBYATTRIBUTE, m_nTextColorOption);
	DDX_Check(pDX, IDC_DUETASKCOLOR, m_bSpecifyDueColor);
	DDX_Check(pDX, IDC_DUETODAYTASKCOLOR, m_bSpecifyDueTodayColor);
	DDX_Check(pDX, IDC_STARTTASKCOLOR, m_bSpecifyStartColor);
	DDX_Check(pDX, IDC_STARTTODAYTASKCOLOR, m_bSpecifyStartTodayColor);
	DDX_Control(pDX, IDC_SETSTARTTASKCOLOR, m_btStartColor);
	DDX_Control(pDX, IDC_SETSTARTTODAYTASKCOLOR, m_btStartTodayColor);
	DDX_Control(pDX, IDC_SETDUETASKCOLOR, m_btDueColor);
	DDX_Control(pDX, IDC_SETDUETODAYTASKCOLOR, m_btDueTodayColor);
	DDX_Control(pDX, IDC_SETDONECOLOR, m_btDoneColor);
	DDX_Control(pDX, IDC_SETGRIDLINECOLOR, m_btGridlineColor);
	DDX_Control(pDX, IDC_TREEFONTSIZE, m_cbTreeFontSize);
	DDX_Control(pDX, IDC_TREEFONTLIST, m_cbTreeFonts);
	DDX_Control(pDX, IDC_COMMENTSFONTSIZE, m_cbCommentsFontSize);
	DDX_Control(pDX, IDC_COMMENTSFONTLIST, m_cbCommentsFonts);
	DDX_Check(pDX, IDC_SPECIFYTREEFONT, m_bSpecifyTreeFont);
	DDX_Check(pDX, IDC_SPECIFYCOMMENTSFONT, m_bSpecifyCommentsFont);
	DDX_Check(pDX, IDC_SPECIFYGRIDLINECOLOR, m_bSpecifyGridColor);
	DDX_Check(pDX, IDC_SPECIFYDONECOLOR, m_bSpecifyDoneColor);
	DDX_Check(pDX, IDC_SPECIFYFLAGGEDCOLOR, m_bSpecifyFlaggedColor);
	DDX_Check(pDX, IDC_SPECIFYREFERENCECOLOR, m_bSpecifyReferenceColor);
	DDX_Control(pDX, IDC_SETFLAGGEDCOLOR, m_btFlaggedColor);
	DDX_Control(pDX, IDC_SETREFERENCECOLOR, m_btReferenceColor);
	DDX_Control(pDX, IDC_SETALTLINECOLOR, m_btAltLineColor);
	DDX_Control(pDX, IDC_SETPRIORITYCOLOR, m_btSetColor);
	DDX_Control(pDX, IDC_LOWPRIORITYCOLOR, m_btLowColor);
	DDX_Control(pDX, IDC_HIGHPRIORITYCOLOR, m_btHighColor);
	DDX_Check(pDX, IDC_COLORPRIORITY, m_bColorPriority);
	DDX_Radio(pDX, IDC_INDIVIDUALPRIORITYCOLORS, m_bGradientPriorityColors);
	DDX_CBIndex(pDX, IDC_PRIORITYCOLORS, m_nSelPriorityColor);
	DDX_Control(pDX, IDC_PRIORITYCOLORS, m_cbPriorityColors);

	m_cbTreeFonts.DDX(pDX, m_sTreeFont);
	m_cbCommentsFonts.DDX(pDX, m_sCommentsFont);

	// font sizes
	if (pDX->m_bSaveAndValidate)
	{
		m_nTreeFontSize = CDialogHelper::GetSelectedItemAsValue(m_cbTreeFontSize);
		m_nCommentsFontSize = CDialogHelper::GetSelectedItemAsValue(m_cbCommentsFontSize);
		m_nColorAttribute = (TDC_ATTRIBUTE)CDialogHelper::GetSelectedItemData(m_cbColorByAttribute);
	}
	else
	{
		if (!CDialogHelper::SelectItemByValue(m_cbTreeFontSize, m_nTreeFontSize))
		{
			m_nTreeFontSize = 9;
			CDialogHelper::SelectItemByValue(m_cbTreeFontSize, m_nTreeFontSize);
		}

		if (!CDialogHelper::SelectItemByValue(m_cbCommentsFontSize, m_nCommentsFontSize))
		{
			m_nCommentsFontSize = 9;
			CDialogHelper::SelectItemByValue(m_cbCommentsFontSize, m_nCommentsFontSize);
		}

		CDialogHelper::SelectItemByData(m_cbColorByAttribute, m_nColorAttribute);
	}
}


BEGIN_MESSAGE_MAP(CPreferencesUITasklistColorsPage, CPreferencesPageBase)
	//{{AFX_MSG_MAP(CPreferencesUITasklistColorsPage)
	ON_BN_CLICKED(IDC_SETALTLINECOLOR, OnAltlinecolor)
	ON_BN_CLICKED(IDC_ALTERNATELINECOLOR, OnSpecifyAlternatelinecolor)
	ON_BN_CLICKED(IDC_COLORTEXTBYATTRIBUTE, OnChangeTextColorOption)
	ON_BN_CLICKED(IDC_COMMENTSUSETREEFONT, OnCommentsusetreefont)
	ON_BN_CLICKED(IDC_SPECIFYCOMMENTSFONT, OnSpecifycommentsfont)
	ON_BN_CLICKED(IDC_POPULATEATTRIBLIST, OnPopulateattriblist)
	ON_BN_CLICKED(IDC_SETATTRIBUTECOLOR, OnSetAttribcolor)
	ON_CBN_EDITCHANGE(IDC_ATTRIBUTECOLORS, OnEditAttribcolors)
	ON_CBN_SELCHANGE(IDC_ATTRIBUTECOLORS, OnSelchangeAttribcolors)
	ON_BN_CLICKED(IDC_COLORTASKSBYCOLOR, OnChangeTextColorOption)
	ON_BN_CLICKED(IDC_COLORTEXTBYPRIORITY, OnChangeTextColorOption)
	ON_CBN_SELCHANGE(IDC_ATTRIBUTETOCOLORBY, OnSelchangeAttributetocolorby)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_STARTTASKCOLOR, OnStarttaskcolor)
	ON_BN_CLICKED(IDC_SETSTARTTASKCOLOR, OnSetStarttaskcolor)
	ON_BN_CLICKED(IDC_STARTTODAYTASKCOLOR, OnStarttodaytaskcolor)
	ON_BN_CLICKED(IDC_SETSTARTTODAYTASKCOLOR, OnSetStarttodaytaskcolor)
	ON_BN_CLICKED(IDC_DUETASKCOLOR, OnDuetaskcolor)
	ON_BN_CLICKED(IDC_SETDUETASKCOLOR, OnSetduetaskcolor)
	ON_BN_CLICKED(IDC_DUETODAYTASKCOLOR, OnDuetodaytaskcolor)
	ON_BN_CLICKED(IDC_SETDUETODAYTASKCOLOR, OnSetduetodaytaskcolor)
	ON_BN_CLICKED(IDC_SPECIFYTREEFONT, OnSpecifytreefont)
	ON_BN_CLICKED(IDC_SETGRIDLINECOLOR, OnSetgridlinecolor)
	ON_BN_CLICKED(IDC_SPECIFYGRIDLINECOLOR, OnSpecifygridlinecolor)
	ON_BN_CLICKED(IDC_SETDONECOLOR, OnSetdonecolor)
	ON_BN_CLICKED(IDC_SPECIFYDONECOLOR, OnSpecifydonecolor)
	ON_BN_CLICKED(IDC_LOWPRIORITYCOLOR, OnLowprioritycolor)
	ON_BN_CLICKED(IDC_HIGHPRIORITYCOLOR, OnHighprioritycolor)
	ON_BN_CLICKED(IDC_SETPRIORITYCOLOR, OnSetprioritycolor)
	ON_BN_CLICKED(IDC_GRADIENTPRIORITYCOLORS, OnChangePriorityColorOption)
	ON_BN_CLICKED(IDC_COLORPRIORITY, OnColorPriority)
	ON_BN_CLICKED(IDC_SETFLAGGEDCOLOR, OnSetflaggedcolor)
	ON_BN_CLICKED(IDC_SETREFERENCECOLOR, OnSetReferencecolor)
	ON_BN_CLICKED(IDC_SPECIFYFLAGGEDCOLOR, OnSpecifyflaggedcolor)
	ON_BN_CLICKED(IDC_SPECIFYREFERENCECOLOR, OnSpecifyReferencecolor)
	ON_CBN_SELCHANGE(IDC_PRIORITYCOLORS, OnSelchangePrioritycolors)
	ON_BN_CLICKED(IDC_INDIVIDUALPRIORITYCOLORS, OnChangePriorityColorOption)
	ON_REGISTERED_MESSAGE(WM_ACB_ITEMADDED, OnAttribAdded)
	ON_REGISTERED_MESSAGE(WM_ACB_ITEMDELETED, OnAttribDeleted)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPreferencesUITasklistColorsPage message handlers

BOOL CPreferencesUITasklistColorsPage::OnInitDialog() 
{
	CPreferencesPageBase::OnInitDialog();

	// disable translation of user-entered text
	CLocalizer::EnableTranslation(m_cbAttributes, FALSE);

	m_mgrGroupLines.AddGroupLine(IDC_TASKCOLOURGROUP, *this);

	GetDlgItem(IDC_GRADIENTPRIORITYCOLORS)->EnableWindow(m_bColorPriority);
	GetDlgItem(IDC_INDIVIDUALPRIORITYCOLORS)->EnableWindow(m_bColorPriority);
	GetDlgItem(IDC_PRIORITYCOLORS)->EnableWindow(m_bColorPriority && !m_bGradientPriorityColors);
	GetDlgItem(IDC_USEHLSGRADIENT)->EnableWindow(m_bColorPriority && m_bGradientPriorityColors);
	GetDlgItem(IDC_TREEFONTSIZE)->EnableWindow(m_bSpecifyTreeFont);
	GetDlgItem(IDC_TREEFONTSIZELABEL)->EnableWindow(m_bSpecifyTreeFont);
	GetDlgItem(IDC_TREEFONTLIST)->EnableWindow(m_bSpecifyTreeFont);
	GetDlgItem(IDC_COMMENTSUSETREEFONT)->EnableWindow(m_bSpecifyTreeFont);
	GetDlgItem(IDC_SPECIFYCOMMENTSFONT)->EnableWindow(!m_bCommentsUseTreeFont || !m_bSpecifyTreeFont);

	BOOL bColorByAttrib = (m_nTextColorOption == COLOROPT_ATTRIB);
	GetDlgItem(IDC_ATTRIBUTETOCOLORBY)->EnableWindow(bColorByAttrib);
	GetDlgItem(IDC_ATTRIBUTECOLORS)->EnableWindow(bColorByAttrib);
	GetDlgItem(IDC_POPULATEATTRIBLIST)->EnableWindow(bColorByAttrib);

	BOOL bEnableCommentsFont = m_bSpecifyCommentsFont && (!m_bSpecifyTreeFont || !m_bCommentsUseTreeFont);
	GetDlgItem(IDC_COMMENTSFONTSIZE)->EnableWindow(bEnableCommentsFont);
	GetDlgItem(IDC_COMMENTSFONTSIZELABEL)->EnableWindow(bEnableCommentsFont);
	GetDlgItem(IDC_COMMENTSFONTLIST)->EnableWindow(bEnableCommentsFont);

	m_btSetColor.EnableWindow(m_bColorPriority && !m_bGradientPriorityColors);
	m_btLowColor.EnableWindow(m_bColorPriority && m_bGradientPriorityColors);
	m_btHighColor.EnableWindow(m_bColorPriority && m_bGradientPriorityColors);
	m_btGridlineColor.EnableWindow(m_bSpecifyGridColor);
	m_btDoneColor.EnableWindow(m_bSpecifyDoneColor);
	m_btAltLineColor.EnableWindow(m_bAlternateLineColor);
	m_btStartColor.EnableWindow(m_bSpecifyStartColor);
	m_btStartTodayColor.EnableWindow(m_bSpecifyStartTodayColor);
	m_btDueColor.EnableWindow(m_bSpecifyDueColor);
	m_btDueTodayColor.EnableWindow(m_bSpecifyDueTodayColor);
	m_btAttribColor.EnableWindow(bColorByAttrib && !m_sSelAttrib.IsEmpty());
	m_btFlaggedColor.EnableWindow(m_bSpecifyFlaggedColor);
	m_btReferenceColor.EnableWindow(m_bSpecifyReferenceColor);
	
	m_btGridlineColor.SetColor(m_crGridlines);
	m_btLowColor.SetColor(m_crLow);
	m_btHighColor.SetColor(m_crHigh);
	m_btSetColor.SetColor(m_aPriorityColors[0]);
	m_btDoneColor.SetColor(m_crDone);
	m_btAltLineColor.SetColor(m_crAltLine);
	m_btStartColor.SetColor(m_crStart);
	m_btStartTodayColor.SetColor(m_crStartToday);
	m_btDueColor.SetColor(m_crDue);
	m_btDueTodayColor.SetColor(m_crDueToday);
	m_btFlaggedColor.SetColor(m_crFlagged);
	m_btReferenceColor.SetColor(m_crReference);

	// priority colors
	for (int nPriority = 0; nPriority < m_aPriorityColors.GetSize(); nPriority++)
		m_cbPriorityColors.SetColor(nPriority, (COLORREF)m_aPriorityColors[nPriority]);

	// attribute colors
	CDialogHelper::AddString(m_cbColorByAttribute, CEnString(IDS_TDLBC_CATEGORY), TDCA_CATEGORY);
	CDialogHelper::AddString(m_cbColorByAttribute, CEnString(IDS_TDLBC_STATUS), TDCA_STATUS);
	CDialogHelper::AddString(m_cbColorByAttribute, CEnString(IDS_TDLBC_ALLOCTO), TDCA_ALLOCTO);
	CDialogHelper::AddString(m_cbColorByAttribute, CEnString(IDS_TDLBC_ALLOCBY), TDCA_ALLOCBY);
	CDialogHelper::AddString(m_cbColorByAttribute, CEnString(IDS_TDLBC_VERSION), TDCA_VERSION);
	CDialogHelper::AddString(m_cbColorByAttribute, CEnString(IDS_TDLBC_EXTERNALID), TDCA_EXTERNALID);
	CDialogHelper::AddString(m_cbColorByAttribute, CEnString(IDS_TDLBC_TAGS), TDCA_TAGS);

	if (!m_sSelAttrib.IsEmpty())
	{
		int nColor = m_aAttribColors.GetSize();

		while (nColor--)
		{
			const ATTRIBCOLOR& ac = m_aAttribColors[nColor];
			
			if (!ac.sAttrib.IsEmpty())
				m_cbAttributes.AddString(ac.sAttrib);
		}

		nColor = FindAttribColor(m_sSelAttrib);

		if (nColor != -1)
			m_btAttribColor.SetColor(m_aAttribColors[nColor].color);
	}

	UpdateData(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPreferencesUITasklistColorsPage::OnLowprioritycolor() 
{
	m_crLow = m_btLowColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnHighprioritycolor() 
{
	m_crHigh = m_btHighColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSetprioritycolor() 
{
	VERIFY(m_nSelPriorityColor >= 0);

	m_aPriorityColors.SetAt(m_nSelPriorityColor, m_btSetColor.GetColor());
	m_cbPriorityColors.SetColor(m_nSelPriorityColor, m_btSetColor.GetColor());

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnChangePriorityColorOption() 
{
	UpdateData();

	GetDlgItem(IDC_PRIORITYCOLORS)->EnableWindow(m_bColorPriority && !m_bGradientPriorityColors);
	GetDlgItem(IDC_USEHLSGRADIENT)->EnableWindow(m_bColorPriority && m_bGradientPriorityColors);

	m_btSetColor.EnableWindow(m_bColorPriority && !m_bGradientPriorityColors && m_nSelPriorityColor >= 0);
	m_btLowColor.EnableWindow(m_bColorPriority && m_bGradientPriorityColors);
	m_btHighColor.EnableWindow(m_bColorPriority && m_bGradientPriorityColors);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnColorPriority() 
{
	UpdateData();

	GetDlgItem(IDC_GRADIENTPRIORITYCOLORS)->EnableWindow(m_bColorPriority);
	GetDlgItem(IDC_INDIVIDUALPRIORITYCOLORS)->EnableWindow(m_bColorPriority);

	OnChangePriorityColorOption(); // to handle the other controls

	// if the text color option is COLOROPT_PRIORITY and 
	// the user has turned off priority coloring then switch to default
	if (!m_bColorPriority && m_nTextColorOption == COLOROPT_PRIORITY)
	{
		m_nTextColorOption = COLOROPT_DEFAULT;
		UpdateData(FALSE);
	}

	CPreferencesPageBase::OnControlChange();
}

int CPreferencesUITasklistColorsPage::GetPriorityColors(CDWordArray& aColors) const 
{ 
	aColors.RemoveAll();

	if (m_bColorPriority)
	{
		if (!m_bGradientPriorityColors)
			aColors.Copy(m_aPriorityColors); 
		else
		{
			aColors.Add(m_crLow);

			if (m_bHLSColorGradient)
			{
				HLSX hlsLow, hlsHigh;
				RGBX rgbLow(m_crLow), rgbHigh(m_crHigh);

				// convert rgb limits to hls
				RGBX::RGB2HLS(rgbLow, hlsLow);
				RGBX::RGB2HLS(rgbHigh, hlsHigh);

				float fHueInc = (hlsHigh.fHue - hlsLow.fHue) / 10;
				float fSatInc = (hlsHigh.fSaturation - hlsLow.fSaturation) / 10;
				float fLumInc = (hlsHigh.fLuminosity - hlsLow.fLuminosity) / 10;

				HLSX hlsColor = hlsLow;
				int nPriority = 10; // aColors[0] added at top

				while (nPriority--)
				{
					hlsColor.fHue += fHueInc;
					hlsColor.fSaturation += fSatInc;
					hlsColor.fLuminosity += fLumInc;
					
					RGBX rgbColor;
					RGBX::HLS2RGB(hlsColor, rgbColor);

					COLORREF color = min(RGB(255, 255, 255), (COLORREF)rgbColor);
					aColors.Add((DWORD)color);
				}

			}
			else // RGB Gradient
			{
				BYTE redLow = GetRValue(m_crLow);
				BYTE greenLow = GetGValue(m_crLow);
				BYTE blueLow = GetBValue(m_crLow);

				BYTE redHigh = GetRValue(m_crHigh);
				BYTE greenHigh = GetGValue(m_crHigh);
				BYTE blueHigh = GetBValue(m_crHigh);

				for (int nPriority = 1; nPriority <= 10; nPriority++) // m_aPriorityColors[0] added at top
				{
					double dRed = (redLow * (10 - nPriority) / 10) + (redHigh * nPriority / 10);
					double dGreen = (greenLow * (10 - nPriority) / 10) + (greenHigh * nPriority / 10);
					double dBlue = (blueLow * (10 - nPriority) / 10) + (blueHigh * nPriority / 10);

					double dColor = dRed + (dGreen * 256) + (dBlue * 256 * 256);

					aColors.Add((DWORD)min(RGB(255, 255, 255), (COLORREF)(int)dColor));
				}
			}
		}
	}
	else
	{
		// grayscale
		aColors.Add(RGB(240, 240, 240));	// 0
		aColors.Add(RGB(216, 216, 216));	// 1
		aColors.Add(RGB(192, 192, 192));	// 2
		aColors.Add(RGB(168, 168, 168));	// 3
		aColors.Add(RGB(144, 144, 144));	// 4
		aColors.Add(RGB(120, 120, 120));	// 5
		aColors.Add(RGB(96, 96, 96));		// 6
		aColors.Add(RGB(72, 72, 72));		// 7
		aColors.Add(RGB(48, 48, 48));		// 8
		aColors.Add(RGB(24, 24, 24));		// 9
		aColors.Add(0);						// 10
	}
	
	return aColors.GetSize(); 
}

void CPreferencesUITasklistColorsPage::GetStartedTaskColors(COLORREF& crStarted, COLORREF& crStartedToday) const 
{ 
	crStarted = m_bSpecifyStartColor ? m_crStart : -1; 
	crStartedToday = m_bSpecifyStartTodayColor ? m_crStartToday : -1; 
}

void CPreferencesUITasklistColorsPage::GetDueTaskColors(COLORREF& crDue, COLORREF& crDueToday) const 
{ 
	crDue = m_bSpecifyDueColor ? m_crDue : -1; 
	crDueToday = m_bSpecifyDueTodayColor ? m_crDueToday : -1; 
}

int CPreferencesUITasklistColorsPage::GetAttributeColors(TDC_ATTRIBUTE& nAttrib, CAttribColorArray& aColors) const
{
	ASSERT(m_nColorAttribute != TDCA_NONE);

	nAttrib = m_nColorAttribute;
	aColors.Copy(m_aAttribColors);

	return aColors.GetSize();
}

void CPreferencesUITasklistColorsPage::OnSelchangePrioritycolors() 
{
	UpdateData();

	ASSERT (m_nSelPriorityColor >= 0);
	
	if (m_nSelPriorityColor >= 0)
	{
		m_btSetColor.SetColor(m_aPriorityColors[m_nSelPriorityColor]);
		m_btSetColor.EnableWindow(TRUE);
	}
	else
		m_btSetColor.EnableWindow(FALSE);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSpecifytreefont() 
{
	UpdateData();

	GetDlgItem(IDC_TREEFONTLIST)->EnableWindow(m_bSpecifyTreeFont);
	GetDlgItem(IDC_TREEFONTSIZE)->EnableWindow(m_bSpecifyTreeFont);
	GetDlgItem(IDC_TREEFONTSIZELABEL)->EnableWindow(m_bSpecifyTreeFont);
	GetDlgItem(IDC_COMMENTSUSETREEFONT)->EnableWindow(m_bSpecifyTreeFont);

	GetDlgItem(IDC_SPECIFYCOMMENTSFONT)->EnableWindow(!m_bCommentsUseTreeFont || !m_bSpecifyTreeFont);
	GetDlgItem(IDC_COMMENTSFONTSIZE)->EnableWindow(m_bSpecifyCommentsFont && !m_bCommentsUseTreeFont);
	GetDlgItem(IDC_COMMENTSFONTSIZELABEL)->EnableWindow(m_bSpecifyCommentsFont && !m_bCommentsUseTreeFont);
	GetDlgItem(IDC_COMMENTSFONTLIST)->EnableWindow(m_bSpecifyCommentsFont && !m_bCommentsUseTreeFont);

	CPreferencesPageBase::OnControlChange();
}

BOOL CPreferencesUITasklistColorsPage::GetTreeFont(CString& sFaceName, int& nPointSize) const
{
	if (m_bSpecifyTreeFont)
	{
		sFaceName = m_sTreeFont;
		nPointSize = m_nTreeFontSize;
	}

	return m_bSpecifyTreeFont;
}

BOOL CPreferencesUITasklistColorsPage::GetCommentsFont(CString& sFaceName, int& nPointSize) const
{
	if ((m_bSpecifyTreeFont && m_bCommentsUseTreeFont) || !m_bSpecifyCommentsFont)
		return FALSE;

	sFaceName = m_sCommentsFont;
	nPointSize = m_nCommentsFontSize;

	return m_bSpecifyCommentsFont;
}

void CPreferencesUITasklistColorsPage::OnSetgridlinecolor() 
{
	m_crGridlines = m_btGridlineColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSpecifygridlinecolor() 
{
	UpdateData();	

	m_btGridlineColor.EnableWindow(m_bSpecifyGridColor);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSetdonecolor() 
{
	m_crDone = m_btDoneColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSetReferencecolor() 
{
	m_crReference = m_btReferenceColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSetflaggedcolor() 
{
	m_crFlagged = m_btFlaggedColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSpecifydonecolor() 
{
	UpdateData();	

	m_btDoneColor.EnableWindow(m_bSpecifyDoneColor);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSpecifyflaggedcolor() 
{
	UpdateData();	

	m_btFlaggedColor.EnableWindow(m_bSpecifyFlaggedColor);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSpecifyReferencecolor() 
{
	UpdateData();	

	m_btReferenceColor.EnableWindow(m_bSpecifyReferenceColor);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnAltlinecolor() 
{
	m_crAltLine = m_btAltLineColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSpecifyAlternatelinecolor() 
{
	UpdateData();	
	
	m_btAltLineColor.EnableWindow(m_bAlternateLineColor);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnChangeTextColorOption() 
{
	UpdateData();

	// if the text color option is COLOROPT_PRIORITY and 
	// the user has got priority coloring turned off then switch it on
	if (!m_bColorPriority && m_nTextColorOption == COLOROPT_PRIORITY)
	{
		m_bColorPriority = TRUE;
		UpdateData(FALSE);

		OnColorPriority();
	}

	BOOL bColorByAttrib = (m_nTextColorOption == COLOROPT_ATTRIB);

	GetDlgItem(IDC_ATTRIBUTETOCOLORBY)->EnableWindow(bColorByAttrib);
	GetDlgItem(IDC_ATTRIBUTECOLORS)->EnableWindow(bColorByAttrib);
	GetDlgItem(IDC_POPULATEATTRIBLIST)->EnableWindow(bColorByAttrib);
	m_btAttribColor.EnableWindow(bColorByAttrib && !m_sSelAttrib.IsEmpty());

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSetAttribcolor() 
{
	UpdateData();

	int nColor = FindAttribColor(m_sSelAttrib);

	if (nColor >= 0)
		m_aAttribColors[nColor].color = m_btAttribColor.GetColor();
	else
	{
		ATTRIBCOLOR ac;
		ac.sAttrib = m_sSelAttrib;
		ac.color = m_btAttribColor.GetColor();

		m_aAttribColors.Add(ac);
	}

	CPreferencesPageBase::OnControlChange();
}

int CPreferencesUITasklistColorsPage::FindAttribColor(LPCTSTR szAttrib)
{
	int nColor = m_aAttribColors.GetSize();

	while (nColor--)
	{
		ATTRIBCOLOR& ac = m_aAttribColors[nColor];

		if (ac.sAttrib.CompareNoCase(szAttrib) == 0)
			return nColor;
	}

	return -1;
}

void CPreferencesUITasklistColorsPage::OnEditAttribcolors() 
{
	UpdateData();

	m_btAttribColor.EnableWindow(m_nTextColorOption == COLOROPT_ATTRIB && 
								!m_sSelAttrib.IsEmpty());

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSelchangeAttribcolors() 
{
	UpdateData();

	int nSel = m_cbAttributes.GetCurSel();

	if (nSel != CB_ERR)
		m_cbAttributes.GetLBText(nSel, m_sSelAttrib);
	
	int nColor = FindAttribColor(m_sSelAttrib);

	if (nColor >= 0)
		m_btAttribColor.SetColor(m_aAttribColors[nColor].color);

	m_btAttribColor.EnableWindow(m_nTextColorOption == COLOROPT_ATTRIB && 
								!m_sSelAttrib.IsEmpty());
}

LRESULT CPreferencesUITasklistColorsPage::OnAttribAdded(WPARAM /*wParam*/, LPARAM lParam)
{
   CString sAttrib((LPCTSTR)lParam);

   if (!sAttrib.IsEmpty() && FindAttribColor(sAttrib) == -1)
   {
		ATTRIBCOLOR ac;
		ac.sAttrib = sAttrib;
		ac.color = 0;

		m_aAttribColors.Add(ac);

		// notify parent
		GetParent()->SendMessage(WM_PUITCP_ATTRIBCHANGE, 1, lParam);

		CPreferencesPageBase::OnControlChange();
   }
  
   return 0L;
}

LRESULT CPreferencesUITasklistColorsPage::OnAttribDeleted(WPARAM /*wParam*/, LPARAM lParam)
{
   CString sAttrib((LPCTSTR)lParam);

   if (!sAttrib.IsEmpty())
   {
      int nDel = FindAttribColor(sAttrib);

      if (nDel != -1)
         m_aAttribColors.RemoveAt(nDel);

		// notify parent
		GetParent()->SendMessage(WM_PUITCP_ATTRIBCHANGE, 0, lParam);

		CPreferencesPageBase::OnControlChange();
   }
  
   return 0L;
}

void CPreferencesUITasklistColorsPage::OnCommentsusetreefont() 
{
	UpdateData();

	BOOL bCommentsUseTreeFont = (m_bCommentsUseTreeFont && m_bSpecifyTreeFont);
	GetDlgItem(IDC_SPECIFYCOMMENTSFONT)->EnableWindow(!bCommentsUseTreeFont);
	GetDlgItem(IDC_COMMENTSFONTSIZE)->EnableWindow(m_bSpecifyCommentsFont && !bCommentsUseTreeFont);
	GetDlgItem(IDC_COMMENTSFONTSIZELABEL)->EnableWindow(m_bSpecifyCommentsFont && !bCommentsUseTreeFont);
	GetDlgItem(IDC_COMMENTSFONTLIST)->EnableWindow(m_bSpecifyCommentsFont && !bCommentsUseTreeFont);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSpecifycommentsfont() 
{
	UpdateData();
	
	BOOL bCommentsUseTreeFont = (m_bCommentsUseTreeFont && m_bSpecifyTreeFont);
	GetDlgItem(IDC_COMMENTSFONTSIZE)->EnableWindow(m_bSpecifyCommentsFont && !bCommentsUseTreeFont);
	GetDlgItem(IDC_COMMENTSFONTSIZELABEL)->EnableWindow(m_bSpecifyCommentsFont && !bCommentsUseTreeFont);
	GetDlgItem(IDC_COMMENTSFONTLIST)->EnableWindow(m_bSpecifyCommentsFont && !bCommentsUseTreeFont);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnDuetaskcolor() 
{
	UpdateData();	
	
	m_btDueColor.EnableWindow(m_bSpecifyDueColor);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnStarttodaytaskcolor() 
{
	UpdateData();	
	
	m_btStartTodayColor.EnableWindow(m_bSpecifyStartTodayColor);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSetStarttaskcolor() 
{
	m_crStart = m_btStartColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSetStarttodaytaskcolor() 
{
	m_crStartToday = m_btStartTodayColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnStarttaskcolor() 
{
	UpdateData();	
	
	m_btStartColor.EnableWindow(m_bSpecifyStartColor);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnDuetodaytaskcolor() 
{
	UpdateData();	
	
	m_btDueTodayColor.EnableWindow(m_bSpecifyDueTodayColor);

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSetduetaskcolor() 
{
	m_crDue = m_btDueColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::OnSetduetodaytaskcolor() 
{
	m_crDueToday = m_btDueTodayColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesUITasklistColorsPage::DeleteAttribute(TDC_ATTRIBUTE nAttrib, LPCTSTR szAttrib)
{
	if (nAttrib != m_nColorAttribute)
		return;

	int nCat = FindAttribColor(szAttrib);
	
	if (nCat != -1)
	{
		m_aAttribColors.RemoveAt(nCat);

		// do we need to update the combo
		if (m_cbAttributes.GetSafeHwnd())
			m_cbAttributes.DeleteString(szAttrib);

		CPreferencesPageBase::OnControlChange();
	}
}

void CPreferencesUITasklistColorsPage::AddAttribute(TDC_ATTRIBUTE nAttrib, LPCTSTR szAttrib)
{
	if (nAttrib != m_nColorAttribute)
		return;

	if (szAttrib && *szAttrib && FindAttribColor(szAttrib) == -1)
	{
		ATTRIBCOLOR ac;
		ac.sAttrib = szAttrib;
		ac.color = 0;
		
		m_aAttribColors.Add(ac);

		// do we need to update the combo
		if (m_cbAttributes.GetSafeHwnd())
			m_cbAttributes.AddString(szAttrib);

		CPreferencesPageBase::OnControlChange();
	}
}

void CPreferencesUITasklistColorsPage::LoadPreferences(const CPreferences& prefs)
{
	m_crLow = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("Low"), PRIORITYLOWCOLOR);
	m_crHigh = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("High"), PRIORITYHIGHCOLOR);

	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P0"), RGB(30, 225, 0)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P1"), RGB(30, 225, 0)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P2"), RGB(30, 225, 0)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P3"), RGB(30, 225, 0)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P4"), RGB(0, 0, 255)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P5"), RGB(0, 0, 255)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P6"), RGB(0, 0, 255)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P7"), RGB(0, 0, 255)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P8"), RGB(255, 0, 0)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P9"), RGB(255, 0, 0)));
	m_aPriorityColors.Add(prefs.GetProfileInt(_T("Preferences\\Colors"), _T("P10"), RGB(255, 0, 0)));

	// attribute colors
	CString sKey = _T("Preferences\\AttribColors"), sAttrib(_T("Attrib"));
	int nNumColor = prefs.GetProfileInt(sKey, _T("Count"), -1);
	m_nColorAttribute = (TDC_ATTRIBUTE)prefs.GetProfileInt(_T("Preferences\\AttribColors"), _T("Attribute"), TDCA_CATEGORY);

	// backwards compat
	if (nNumColor == -1)
	{
		sKey = _T("Preferences\\CatColors");
		nNumColor = prefs.GetProfileInt(sKey, _T("Count"), -1);
		sAttrib = _T("Category");

		ASSERT(m_nColorAttribute == TDCA_CATEGORY);
	}

	for (int nColor = 0; nColor < nNumColor; nColor++)
	{
		CString sColorKey = Misc::MakeKey(_T("\\P%d"), nColor, sKey);

		ATTRIBCOLOR ac;
		ac.color = prefs.GetProfileInt(sColorKey, _T("Color"), 0);
		ac.sAttrib = prefs.GetProfileString(sColorKey, sAttrib);

		if (!ac.sAttrib.IsEmpty())
		{
			m_sSelAttrib = ac.sAttrib;
			m_aAttribColors.Add(ac);
		}
	}

	// add in the users default list items
	AddDefaultListItemsToAttributeColors(prefs);
	
	// prefs
	m_bColorPriority = prefs.GetProfileInt(_T("Preferences"), _T("ColorPriority"), TRUE);
	m_bGradientPriorityColors = !prefs.GetProfileInt(_T("Preferences"), _T("IndividualPriorityColors"), FALSE);
	m_sTreeFont = prefs.GetProfileString(_T("Preferences"), _T("TreeFont"), _T("Arial"));
	m_nTreeFontSize = prefs.GetProfileInt(_T("Preferences"), _T("FontSize"), 8);
	m_bSpecifyTreeFont = prefs.GetProfileInt(_T("Preferences"), _T("SpecifyTreeFont"), FALSE);
	m_sCommentsFont = prefs.GetProfileString(_T("Preferences"), _T("CommentsFont"), _T("Arial"));
	m_nCommentsFontSize = prefs.GetProfileInt(_T("Preferences"), _T("CommentsFontSize"), 8);
	m_bSpecifyCommentsFont = prefs.GetProfileInt(_T("Preferences"), _T("SpecifyCommentsFont"), TRUE);
	m_bSpecifyGridColor = prefs.GetProfileInt(_T("Preferences"), _T("SpecifyGridColor"), TRUE);
	m_bSpecifyDoneColor = prefs.GetProfileInt(_T("Preferences"), _T("SpecifyDoneColor"), TRUE);
	m_bSpecifyStartColor = prefs.GetProfileInt(_T("Preferences"), _T("SpecifyStartColor"), FALSE);
	m_bSpecifyStartTodayColor = prefs.GetProfileInt(_T("Preferences"), _T("SpecifyStartTodayColor"), FALSE);
	m_bSpecifyDueColor = prefs.GetProfileInt(_T("Preferences"), _T("SpecifyDueColor"), TRUE);
	m_bSpecifyDueTodayColor = prefs.GetProfileInt(_T("Preferences"), _T("SpecifyDueTodayColor"), TRUE);
	m_bColorTaskBackground = prefs.GetProfileInt(_T("Preferences"), _T("ColorTaskBackground"), FALSE);
	m_bCommentsUseTreeFont = prefs.GetProfileInt(_T("Preferences"), _T("CommentsUseTreeFont"), FALSE);
	m_bHLSColorGradient = prefs.GetProfileInt(_T("Preferences"), _T("HLSColorGradient"), TRUE);
	m_bHidePriorityNumber = prefs.GetProfileInt(_T("Preferences"), _T("HidePriorityNumber"), FALSE);
	m_bAlternateLineColor = prefs.GetProfileInt(_T("Preferences"), _T("AlternateLineColor"), TRUE);
	m_bSpecifyFlaggedColor = prefs.GetProfileInt(_T("Preferences"), _T("FlaggedColor"), FALSE);
	m_bSpecifyReferenceColor = prefs.GetProfileInt(_T("Preferences"), _T("ReferenceColor"), FALSE);

	m_crGridlines = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("Gridlines"), GRIDLINECOLOR);
	m_crDone = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("TaskDone"), TASKDONECOLOR);
	m_crStart = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("TaskStart"), TASKSTARTCOLOR);
	m_crStartToday = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("TaskStartToday"), TASKSTARTCOLOR);
	m_crDue = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("TaskDue"), TASKDUECOLOR);
	m_crDueToday = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("TaskDueToday"), TASKDUECOLOR);
	m_crAltLine = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("AlternateLines"), ALTERNATELINECOLOR);
	m_crFlagged = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("Flagged"), FLAGGEDCOLOR);
	m_crReference = prefs.GetProfileInt(_T("Preferences\\Colors"), _T("Reference"), REFERENCECOLOR);

	// bkwds compatibility
	if (prefs.GetProfileInt(_T("Preferences"), _T("ColorByPriority"), FALSE))
		m_nTextColorOption = COLOROPT_PRIORITY;

	m_nTextColorOption = prefs.GetProfileInt(_T("Preferences"), _T("TextColorOption"), m_nTextColorOption);
}

void CPreferencesUITasklistColorsPage::AddDefaultListItemsToAttributeColors(const CPreferences& prefs)
{
	CStringArray aDefAttribs;
	int nDefColor = 0;

	switch (m_nColorAttribute)
	{
	case TDCA_CATEGORY:
		nDefColor = prefs.GetArrayItems(_T("Preferences\\CategoryList"), aDefAttribs);
		break;

	case TDCA_ALLOCBY:
		nDefColor = prefs.GetArrayItems(_T("Preferences\\AllocByList"), aDefAttribs);
		break;

	case TDCA_ALLOCTO:
		nDefColor = prefs.GetArrayItems(_T("Preferences\\AllocToList"), aDefAttribs);
		break;

	case TDCA_STATUS:
		nDefColor = prefs.GetArrayItems(_T("Preferences\\StatusList"), aDefAttribs);
		break;
	}

	while (nDefColor--)
	{
		const CString& sDef = aDefAttribs[nDefColor];
		
		if (sDef.IsEmpty())
			continue;
		
		// make sure this attrib is not already specified
		if (FindAttribColor(sDef) != -1)
			continue; // skip
		
		ATTRIBCOLOR ac;
		ac.color = 0;
		ac.sAttrib = sDef;
		
		m_sSelAttrib = sDef;
		m_aAttribColors.Add(ac);

		// add to combo
		if (m_cbAttributes.GetSafeHwnd())
			m_cbAttributes.AddString(sDef);
	}
}

void CPreferencesUITasklistColorsPage::SavePreferences(CPreferences& prefs)
{
	// save settings
	// priority colors
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("Low"), m_crLow);
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("High"), m_crHigh);

	int nColor = 11;

	while (nColor--)
	{
		CString sKey = Misc::MakeKey(_T("P%d"), nColor);
		prefs.WriteProfileInt(_T("Preferences\\Colors"), sKey, m_aPriorityColors[nColor]);
	}

	// attrib colors
	int nNumColor = m_aAttribColors.GetSize();

	prefs.WriteProfileInt(_T("Preferences\\AttribColors"), _T("Attribute"), m_nColorAttribute);
	prefs.WriteProfileInt(_T("Preferences\\AttribColors"), _T("Count"), nNumColor);

	for (nColor = 0; nColor < nNumColor; nColor++)
	{
		CString sKey = Misc::MakeKey(_T("Preferences\\AttribColors\\P%d"), nColor);

		const ATTRIBCOLOR& ac = m_aAttribColors[nColor];
		prefs.WriteProfileInt(sKey, _T("Color"), ac.color);
		prefs.WriteProfileString(sKey, _T("Attrib"), ac.sAttrib);
	}

	// save settings
	prefs.WriteProfileInt(_T("Preferences"), _T("TextColorOption"), m_nTextColorOption);
	prefs.WriteProfileInt(_T("Preferences"), _T("ColorPriority"), m_bColorPriority);
	prefs.WriteProfileInt(_T("Preferences"), _T("IndividualPriorityColors"), !m_bGradientPriorityColors);
	prefs.WriteProfileString(_T("Preferences"), _T("TreeFont"), m_sTreeFont);
	prefs.WriteProfileInt(_T("Preferences"), _T("FontSize"), m_nTreeFontSize);
	prefs.WriteProfileInt(_T("Preferences"), _T("SpecifyTreeFont"), m_bSpecifyTreeFont);
	prefs.WriteProfileString(_T("Preferences"), _T("CommentsFont"), m_sCommentsFont);
	prefs.WriteProfileInt(_T("Preferences"), _T("CommentsFontSize"), m_nCommentsFontSize);
	prefs.WriteProfileInt(_T("Preferences"), _T("SpecifyCommentsFont"), m_bSpecifyCommentsFont);
	prefs.WriteProfileInt(_T("Preferences"), _T("SpecifyGridColor"), m_bSpecifyGridColor);
	prefs.WriteProfileInt(_T("Preferences"), _T("SpecifyDoneColor"), m_bSpecifyDoneColor);
	prefs.WriteProfileInt(_T("Preferences"), _T("SpecifyDueColor"), m_bSpecifyDueColor);
	prefs.WriteProfileInt(_T("Preferences"), _T("SpecifyDueTodayColor"), m_bSpecifyDueTodayColor);
	prefs.WriteProfileInt(_T("Preferences"), _T("SpecifyStartColor"), m_bSpecifyStartColor);
	prefs.WriteProfileInt(_T("Preferences"), _T("SpecifyStartTodayColor"), m_bSpecifyStartTodayColor);
	prefs.WriteProfileInt(_T("Preferences"), _T("ColorTaskBackground"), m_bColorTaskBackground);
	prefs.WriteProfileInt(_T("Preferences"), _T("CommentsUseTreeFont"), m_bCommentsUseTreeFont);
	prefs.WriteProfileInt(_T("Preferences"), _T("HLSColorGradient"), m_bHLSColorGradient);
	prefs.WriteProfileInt(_T("Preferences"), _T("HidePriorityNumber"), m_bHidePriorityNumber);
	prefs.WriteProfileInt(_T("Preferences"), _T("AlternateLineColor"), m_bAlternateLineColor);
	prefs.WriteProfileInt(_T("Preferences"), _T("FlaggedColor"), m_bSpecifyFlaggedColor);
	prefs.WriteProfileInt(_T("Preferences"), _T("ReferenceColor"), m_bSpecifyReferenceColor);

	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("Gridlines"), m_crGridlines);
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("TaskDone"), m_crDone);
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("TaskStart"), m_crStart);
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("TaskStartToday"), m_crStartToday);
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("TaskDue"), m_crDue);
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("TaskDueToday"), m_crDueToday);
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("AlternateLines"), m_crAltLine);
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("Flagged"), m_crFlagged);
	prefs.WriteProfileInt(_T("Preferences\\Colors"), _T("Reference"), m_crReference);
}

void CPreferencesUITasklistColorsPage::OnPopulateattriblist() 
{
	CPreferences prefs;
	AddDefaultListItemsToAttributeColors(prefs);
}

void CPreferencesUITasklistColorsPage::OnSelchangeAttributetocolorby() 
{
	UpdateData(TRUE);
}
