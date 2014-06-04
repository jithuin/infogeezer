// PreferencesTaskDefPage.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "PreferencesTaskDefPage.h"
#include "tdcenum.h"

#include "..\shared\enstring.h"
#include "..\shared\misc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

const LPCTSTR ENDL = _T("\r\n");

/////////////////////////////////////////////////////////////////////////////
// CPreferencesTaskDefPage property page

IMPLEMENT_DYNCREATE(CPreferencesTaskDefPage, CPreferencesPageBase)

CPreferencesTaskDefPage::CPreferencesTaskDefPage() : 
	CPreferencesPageBase(CPreferencesTaskDefPage::IDD)
{
	//{{AFX_DATA_INIT(CPreferencesTaskDefPage)
	m_bUpdateInheritAttributes = FALSE;
	//}}AFX_DATA_INIT
	m_nSelAttribUse = -1;

	// attrib use
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_PRIORITY, PTPA_PRIORITY, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_RISK, PTPA_RISK, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_TIMEEST, PTPA_TIMEEST, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_ALLOCTO, PTPA_ALLOCTO, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_ALLOCBY, PTPA_ALLOCBY, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_STATUS, PTPA_STATUS, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_CATEGORY, PTPA_CATEGORY, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_PTDP_COLOR, PTPA_COLOR, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_PTDP_DUEDATE, PTPA_DUEDATE, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_PTDP_VERSION, PTPA_VERSION, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_STARTDATE, PTPA_STARTDATE, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_FLAG, PTPA_FLAG, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_EXTERNALID, PTPA_EXTERNALID, -1)); 
	m_aAttribPrefs.Add(ATTRIBPREF(IDS_TDLBC_TAGS, PTPA_TAGS, -1)); 

	m_eCost.SetMask(_T(".0123456789"), ME_LOCALIZEDECIMAL);
}

CPreferencesTaskDefPage::~CPreferencesTaskDefPage()
{
}

void CPreferencesTaskDefPage::DoDataExchange(CDataExchange* pDX)
{
	CPreferencesPageBase::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CPreferencesTaskDefPage)
	DDX_Control(pDX, IDC_DEFAULTRISK, m_cbDefRisk);
	DDX_Control(pDX, IDC_DEFAULTPRIORITY, m_cbDefPriority);
	DDX_Text(pDX, IDC_DEFAULTCREATEDBY, m_sDefCreatedBy);
	DDX_Text(pDX, IDC_DEFAULTCOST, m_dDefCost);
	DDX_Check(pDX, IDC_UPDATEINHERITATTRIB, m_bUpdateInheritAttributes);
	DDX_Text(pDX, IDC_CATEGORYLIST, m_sDefCategoryList);
	DDX_Text(pDX, IDC_STATUSLIST, m_sDefStatusList);
	DDX_Text(pDX, IDC_ALLOCTOLIST, m_sDefAllocToList);
	DDX_Text(pDX, IDC_ALLOCBYLIST, m_sDefAllocByList);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_DEFAULTTIMESPENT, m_eTimeSpent);
	DDX_Control(pDX, IDC_DEFAULTTIMEEST, m_eTimeEst);
	DDX_Control(pDX, IDC_DEFAULTCOST, m_eCost);
	DDX_Control(pDX, IDC_SETDEFAULTCOLOR, m_btDefColor);
	DDX_Control(pDX, IDC_INHERITATTRIBUTES, m_lbAttribUse);
	DDX_CBPriority(pDX, IDC_DEFAULTPRIORITY, m_nDefPriority);
	DDX_CBRisk(pDX, IDC_DEFAULTRISK, m_nDefRisk);
	DDX_Text(pDX, IDC_DEFAULTTIMEEST, m_dDefTimeEst);
	DDX_Text(pDX, IDC_DEFAULTTIMESPENT, m_dDefTimeSpent);
	DDX_Text(pDX, IDC_DEFAULTALLOCTO, m_sDefAllocTo);
	DDX_LBIndex(pDX, IDC_INHERITATTRIBUTES, m_nSelAttribUse);
	DDX_Text(pDX, IDC_DEFAULTALLOCBY, m_sDefAllocBy);
	DDX_Text(pDX, IDC_DEFAULTSTATUS, m_sDefStatus);
	DDX_Text(pDX, IDC_DEFAULTTAGS, m_sDefTags);
	DDX_Text(pDX, IDC_DEFAULTCATEGORY, m_sDefCategory);
	DDX_Check(pDX, IDC_USEPARENTATTRIB, m_bInheritParentAttributes);
	DDX_Check(pDX, IDC_USECREATIONFORDEFSTARTDATE, m_bUseCreationForDefStartDate);
}


BEGIN_MESSAGE_MAP(CPreferencesTaskDefPage, CPreferencesPageBase)
	//{{AFX_MSG_MAP(CPreferencesTaskDefPage)
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_SETDEFAULTCOLOR, OnSetdefaultcolor)
	ON_BN_CLICKED(IDC_USEPARENTATTRIB, OnUseparentattrib)
	ON_CLBN_CHKCHANGE(IDC_INHERITATTRIBUTES, OnAttribUseChange)
	ON_REGISTERED_MESSAGE(WM_ACB_ITEMADDED, OnListAddItem)
	ON_REGISTERED_MESSAGE(WM_ACB_ITEMDELETED, OnListDeleteItem)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CPreferencesTaskDefPage message handlers

BOOL CPreferencesTaskDefPage::OnInitDialog() 
{
	CPreferencesPageBase::OnInitDialog();
	
	m_mgrGroupLines.AddGroupLine(IDC_DEFGROUP, *this);
	m_mgrGroupLines.AddGroupLine(IDC_INHERITGROUP, *this);
	m_mgrGroupLines.AddGroupLine(IDC_DROPLISTGROUP, *this);

	GetDlgItem(IDC_INHERITATTRIBUTES)->EnableWindow(m_bInheritParentAttributes);
	GetDlgItem(IDC_UPDATEINHERITATTRIB)->EnableWindow(m_bInheritParentAttributes);
	
	m_btDefColor.SetColor(m_crDef);

	int nAttrib = m_aAttribPrefs.GetSize();
	
	while (nAttrib--)
	{
		int nIndex = m_lbAttribUse.InsertString(0, m_aAttribPrefs[nAttrib].sName);

		m_lbAttribUse.SetItemData(nIndex, m_aAttribPrefs[nAttrib].nAttrib);
		m_lbAttribUse.SetCheck(nIndex, m_aAttribPrefs[nAttrib].bUse ? 1 : 0);
	}

	// init edit prompts()
	m_mgrPrompts.SetEditPrompt(IDC_DEFAULTALLOCTO, *this, CEnString(IDS_PTDP_NAMEPROMPT));
	m_mgrPrompts.SetEditPrompt(IDC_DEFAULTALLOCBY, *this, CEnString(IDS_PTDP_NAMEPROMPT));
	m_mgrPrompts.SetEditPrompt(IDC_DEFAULTSTATUS, *this, CEnString(IDS_PTDP_STATUSPROMPT));
	m_mgrPrompts.SetEditPrompt(IDC_DEFAULTTAGS, *this, CEnString(IDS_PTDP_TAGSPROMPT));
	m_mgrPrompts.SetEditPrompt(IDC_DEFAULTCATEGORY, *this, CEnString(IDS_PTDP_CATEGORYPROMPT));
	m_mgrPrompts.SetEditPrompt(IDC_DEFAULTCREATEDBY, *this, CEnString(IDS_PTDP_NAMEPROMPT));
	m_mgrPrompts.SetComboEditPrompt(IDC_ALLOCTOLIST, *this, CEnString(IDS_PTDP_NAMEPROMPT));
	m_mgrPrompts.SetComboEditPrompt(IDC_ALLOCBYLIST, *this, CEnString(IDS_PTDP_NAMEPROMPT));
	m_mgrPrompts.SetComboEditPrompt(IDC_STATUSLIST, *this, CEnString(IDS_PTDP_STATUSPROMPT));
	m_mgrPrompts.SetComboEditPrompt(IDC_CATEGORYLIST, *this, CEnString(IDS_PTDP_CATEGORYPROMPT));

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CPreferencesTaskDefPage::SetPriorityColors(const CDWordArray& aColors)
{
	m_cbDefPriority.SetColors(aColors);
}

void CPreferencesTaskDefPage::OnOK() 
{
	CPreferencesPageBase::OnOK();
}

void CPreferencesTaskDefPage::OnSetdefaultcolor() 
{
	m_crDef = m_btDefColor.GetColor();

	CPreferencesPageBase::OnControlChange();
}

void CPreferencesTaskDefPage::OnUseparentattrib() 
{
	UpdateData();

	GetDlgItem(IDC_INHERITATTRIBUTES)->EnableWindow(m_bInheritParentAttributes);
	GetDlgItem(IDC_UPDATEINHERITATTRIB)->EnableWindow(m_bInheritParentAttributes);

	CPreferencesPageBase::OnControlChange();
}

int CPreferencesTaskDefPage::GetListItems(PTDP_LIST nList, CStringArray& aItems) const
{
	aItems.RemoveAll();

	// include default task attributes
	CString sDef;

	switch (nList)
	{
	case PTDP_ALLOCBY:	sDef = m_sDefAllocByList + ENDL + m_sDefAllocBy; break;
	case PTDP_ALLOCTO:	sDef = m_sDefAllocToList + ENDL + m_sDefAllocTo; break;
	case PTDP_STATUS:	sDef = m_sDefStatusList + ENDL + m_sDefStatus; break; 
	case PTDP_CATEGORY:	sDef = m_sDefCategoryList + ENDL + m_sDefCategory; break;

	default: ASSERT(0);	return 0;
	}

	CStringArray aDef;
	Misc::Split(sDef, aDef, FALSE, ENDL);
	Misc::AddUniqueItems(aDef, aItems);

	return aItems.GetSize();
}

BOOL CPreferencesTaskDefPage::AddListItem(PTDP_LIST nList, LPCTSTR szItem)
{
	UpdateData();

	CString* pList = NULL;

	switch (nList)
	{
	case PTDP_ALLOCBY:	pList = &m_sDefAllocByList; break;
	case PTDP_ALLOCTO:	pList = &m_sDefAllocToList; break;
	case PTDP_STATUS:	pList = &m_sDefStatusList; break;
	case PTDP_CATEGORY:	pList = &m_sDefCategoryList; break;

	default: ASSERT(0);	return FALSE;
	}

	// parse string into array
	CStringArray aItems;

	if (Misc::Split(*pList, aItems, FALSE, ENDL))
	{
		// add to array
		if (Misc::AddUniqueItem(szItem, aItems))
		{
			// update edit control
			*pList = Misc::FormatArray(aItems, ENDL);
			UpdateData(FALSE);

			return TRUE;
		}
	}

	return FALSE; // not unique
}

BOOL CPreferencesTaskDefPage::DeleteListItem(PTDP_LIST nList, LPCTSTR szItem)
{
	UpdateData();

	CString* pList = NULL;

	switch (nList)
	{
	case PTDP_ALLOCBY:	pList = &m_sDefAllocByList; break;
	case PTDP_ALLOCTO:	pList = &m_sDefAllocToList; break;
	case PTDP_STATUS:	pList = &m_sDefStatusList; break;
	case PTDP_CATEGORY:	pList = &m_sDefCategoryList; break;

	default: ASSERT(0);	return FALSE;
	}

	// parse string into array
	CStringArray aItems;

	if (Misc::Split(*pList, aItems, FALSE, ENDL))
	{
		// delete from array
		if (Misc::RemoveItem(szItem, aItems))
		{
			// update edit control
			*pList = Misc::FormatArray(aItems, ENDL);
			UpdateData(FALSE);

			return TRUE;
		}
	}

	return FALSE; // not found
}

PTDP_LIST CPreferencesTaskDefPage::MapCtrlIDToList(UINT nListID)
{
	switch (nListID)
	{
	case IDC_ALLOCBYLIST:	return PTDP_ALLOCBY;
	case IDC_ALLOCTOLIST:	return PTDP_ALLOCTO;
	case IDC_STATUSLIST:	return PTDP_STATUS;
	case IDC_CATEGORYLIST:	return PTDP_CATEGORY;
	}

	ASSERT(0);
	return (PTDP_LIST)-1;
}

LRESULT CPreferencesTaskDefPage::OnListAddItem(WPARAM wp, LPARAM lp)
{
	UINT nCtrlID = LOWORD(wp);
	int nList = MapCtrlIDToList(nCtrlID);

	if (nList != -1)
		GetParent()->SendMessage(WM_PTDP_LISTCHANGE, MAKEWPARAM(nList, 1), lp);

	return 0L;

	//CPreferencesPageBase::OnControlChange();
}

LRESULT CPreferencesTaskDefPage::OnListDeleteItem(WPARAM wp, LPARAM lp)
{
	UINT nCtrlID = LOWORD(wp);
	int nList = MapCtrlIDToList(nCtrlID);

	if (nList != -1)
		GetParent()->SendMessage(WM_PTDP_LISTCHANGE, MAKEWPARAM(nList, 0), lp);

	return 0L;

	//CPreferencesPageBase::OnControlChange();
}

void CPreferencesTaskDefPage::OnAttribUseChange()
{
	UpdateData();

	ASSERT (m_nSelAttribUse >= 0);
	
	if (m_nSelAttribUse >= 0)
	{
		PTP_ATTRIB nSelAttrib = (PTP_ATTRIB)m_lbAttribUse.GetItemData(m_nSelAttribUse);

		// search for this item
		int nAttrib = m_aAttribPrefs.GetSize();
		
		while (nAttrib--)
		{
			if (m_aAttribPrefs[nAttrib].nAttrib == nSelAttrib)
			{
				m_aAttribPrefs[nAttrib].bUse = m_lbAttribUse.GetCheck(m_nSelAttribUse);
				break;
			}
		}
	}

	GetDlgItem(IDC_UPDATEINHERITATTRIB)->EnableWindow(m_bInheritParentAttributes);

	CPreferencesPageBase::OnControlChange();
}

BOOL CPreferencesTaskDefPage::HasCheckedAttributes() const
{
	int nAttrib = m_aAttribPrefs.GetSize();

	while (nAttrib--)
	{
		if (m_aAttribPrefs[nAttrib].bUse)
			return TRUE;
	}

	// else
	return FALSE;
}

int CPreferencesTaskDefPage::GetParentAttribsUsed(CTDCAttributeArray& aAttribs, BOOL& bUpdateAttrib) const
{
	aAttribs.RemoveAll();

	if (m_bInheritParentAttributes)
	{
		bUpdateAttrib = m_bUpdateInheritAttributes;
		int nIndex = (int)m_aAttribPrefs.GetSize();
		
		while (nIndex--)
		{
			if (m_aAttribPrefs[nIndex].bUse)
			{
				switch (m_aAttribPrefs[nIndex].nAttrib)
				{
				case PTPA_PRIORITY:		aAttribs.Add(TDCA_PRIORITY);	break;
				case PTPA_COLOR:		aAttribs.Add(TDCA_COLOR);		break;
				case PTPA_ALLOCTO:		aAttribs.Add(TDCA_ALLOCTO);		break;
				case PTPA_ALLOCBY:		aAttribs.Add(TDCA_ALLOCBY);		break;
				case PTPA_STATUS:		aAttribs.Add(TDCA_STATUS);		break;
				case PTPA_CATEGORY:		aAttribs.Add(TDCA_CATEGORY);	break;
				case PTPA_TIMEEST:		aAttribs.Add(TDCA_TIMEEST);		break;
				case PTPA_RISK:			aAttribs.Add(TDCA_RISK);		break;
				case PTPA_DUEDATE:		aAttribs.Add(TDCA_DUEDATE);		break;
				case PTPA_VERSION:		aAttribs.Add(TDCA_VERSION);		break;
				case PTPA_STARTDATE:	aAttribs.Add(TDCA_STARTDATE);	break;
				case PTPA_FLAG:			aAttribs.Add(TDCA_FLAG);		break;
				case PTPA_EXTERNALID:	aAttribs.Add(TDCA_EXTERNALID);	break;
				case PTPA_TAGS:			aAttribs.Add(TDCA_TAGS);		break;
				}
			}
		}
	}
	else
		bUpdateAttrib = FALSE;

	return aAttribs.GetSize();
}

int CPreferencesTaskDefPage::GetDefaultCategories(CStringArray& aCats) const
{
	return Misc::Split(m_sDefCategory, aCats);
}

int CPreferencesTaskDefPage::GetDefaultTags(CStringArray& aTags) const
{
	return Misc::Split(m_sDefTags, aTags);
}

int CPreferencesTaskDefPage::GetDefaultAllocTo(CStringArray& aAllocTo) const
{
	return Misc::Split(m_sDefAllocTo, aAllocTo);
}

double CPreferencesTaskDefPage::GetDefaultTimeEst(TCHAR& nUnits) const 
{ 
	nUnits = m_eTimeEst.GetUnits();

	return m_dDefTimeEst; 
}

double CPreferencesTaskDefPage::GetDefaultTimeSpent(TCHAR& nUnits) const 
{ 
	nUnits = m_eTimeSpent.GetUnits();

	return m_dDefTimeSpent; 
}

void CPreferencesTaskDefPage::LoadPreferences(const CPreferences& prefs)
{
	// load settings
	m_nDefPriority = prefs.GetProfileInt(_T("Preferences"), _T("DefaultPriority"), 5); 
	m_nDefRisk = prefs.GetProfileInt(_T("Preferences"), _T("DefaultRisk"), 0); 
	m_sDefAllocTo = prefs.GetProfileString(_T("Preferences"), _T("DefaultAllocTo"));
	m_sDefAllocBy = prefs.GetProfileString(_T("Preferences"), _T("DefaultAllocBy"));
	m_sDefStatus = prefs.GetProfileString(_T("Preferences"), _T("DefaultStatus"));
	m_sDefTags = prefs.GetProfileString(_T("Preferences"), _T("DefaultTags"));
	m_sDefCategory = prefs.GetProfileString(_T("Preferences"), _T("DefaultCategory"));
	m_sDefCreatedBy = prefs.GetProfileString(_T("Preferences"), _T("DefaultCreatedBy"), Misc::GetUserName());
	m_crDef = prefs.GetProfileInt(_T("Preferences"), _T("DefaultColor"), 0);
	m_bInheritParentAttributes = prefs.GetProfileInt(_T("Preferences"), _T("InheritParentAttributes"), prefs.GetProfileInt(_T("Preferences"), _T("UseParentAttributes")));
	m_bUpdateInheritAttributes = prefs.GetProfileInt(_T("Preferences"), _T("UpdateInheritAttributes"), FALSE);
	m_bUseCreationForDefStartDate = prefs.GetProfileInt(_T("Preferences"), _T("UseCreationForDefStartDate"), TRUE);
	m_dDefCost = Misc::Atof(prefs.GetProfileString(_T("Preferences"), _T("DefaultCost"), _T("0")));
	m_dDefTimeEst = prefs.GetProfileDouble(_T("Preferences"), _T("DefaultTimeEstimate"), 0);
	m_eTimeEst.SetUnits((TCHAR)prefs.GetProfileInt(_T("Preferences"), _T("DefaultTimeEstUnits"), THU_HOURS));
	m_dDefTimeSpent = prefs.GetProfileDouble(_T("Preferences"), _T("DefaultTimeSpent"), 0);
	m_eTimeSpent.SetUnits((TCHAR)prefs.GetProfileInt(_T("Preferences"), _T("DefaultTimeSpentUnits"), THU_HOURS));
	
   // attribute use
	int nIndex = m_aAttribPrefs.GetSize();
	
	while (nIndex--)
	{
		CString sKey = Misc::MakeKey(_T("Attrib%d"), m_aAttribPrefs[nIndex].nAttrib);
		m_aAttribPrefs[nIndex].bUse = prefs.GetProfileInt(_T("Preferences\\AttribUse"), sKey, FALSE);
	}

	// combo lists
	CStringArray aItems;

	prefs.GetArrayItems(_T("Preferences\\CategoryList"), aItems);
	Misc::SortArray(aItems);
	m_sDefCategoryList = Misc::FormatArray(aItems, ENDL);

	prefs.GetArrayItems(_T("Preferences\\StatusList"), aItems);
	Misc::SortArray(aItems);
	m_sDefStatusList = Misc::FormatArray(aItems, ENDL);

	prefs.GetArrayItems(_T("Preferences\\AllocToList"), aItems);
	Misc::SortArray(aItems);
	m_sDefAllocToList = Misc::FormatArray(aItems, ENDL);

	prefs.GetArrayItems(_T("Preferences\\AllocByList"), aItems);
	Misc::SortArray(aItems);
	m_sDefAllocByList = Misc::FormatArray(aItems, ENDL);
}

void CPreferencesTaskDefPage::SavePreferences(CPreferences& prefs)
{
	// save settings
	prefs.WriteProfileInt(_T("Preferences"), _T("DefaultPriority"), m_nDefPriority);
	prefs.WriteProfileInt(_T("Preferences"), _T("DefaultRisk"), m_nDefRisk);
	prefs.WriteProfileString(_T("Preferences"), _T("DefaultAllocTo"), m_sDefAllocTo);
	prefs.WriteProfileString(_T("Preferences"), _T("DefaultAllocBy"), m_sDefAllocBy);
	prefs.WriteProfileString(_T("Preferences"), _T("DefaultStatus"), m_sDefStatus);
	prefs.WriteProfileString(_T("Preferences"), _T("DefaultTags"), m_sDefTags);
	prefs.WriteProfileString(_T("Preferences"), _T("DefaultCategory"), m_sDefCategory);
	prefs.WriteProfileString(_T("Preferences"), _T("DefaultCreatedBy"), m_sDefCreatedBy);
	prefs.WriteProfileInt(_T("Preferences"), _T("DefaultColor"), m_crDef);
	prefs.WriteProfileInt(_T("Preferences"), _T("InheritParentAttributes"), m_bInheritParentAttributes);
	prefs.WriteProfileInt(_T("Preferences"), _T("UpdateInheritAttributes"), m_bUpdateInheritAttributes);
	prefs.WriteProfileInt(_T("Preferences"), _T("UseCreationForDefStartDate"), m_bUseCreationForDefStartDate);
	prefs.WriteProfileDouble(_T("Preferences"), _T("DefaultCost"), m_dDefCost);
	prefs.WriteProfileDouble(_T("Preferences"), _T("DefaultTimeEstimate"), m_dDefTimeEst);
	prefs.WriteProfileInt(_T("Preferences"), _T("DefaultTimeEstUnits"), m_eTimeEst.GetUnits());
	prefs.WriteProfileDouble(_T("Preferences"), _T("DefaultTimeSpent"), m_dDefTimeSpent);
	prefs.WriteProfileInt(_T("Preferences"), _T("DefaultTimeSpentUnits"), m_eTimeSpent.GetUnits());
	
	// attribute usage
	int nIndex = m_aAttribPrefs.GetSize();

	while (nIndex--)
	{
		CString sKey = Misc::MakeKey(_T("Attrib%d"), m_aAttribPrefs[nIndex].nAttrib);
		prefs.WriteProfileInt(_T("Preferences\\AttribUse"), sKey, m_aAttribPrefs[nIndex].bUse);
	}

	// combo lists
	CStringArray aItems;

	Misc::Split(m_sDefCategoryList, aItems, FALSE, ENDL);
	prefs.WriteArrayItems(aItems, _T("Preferences\\CategoryList"));

	Misc::Split(m_sDefStatusList, aItems, FALSE, ENDL);
	prefs.WriteArrayItems(aItems, _T("Preferences\\StatusList"));

	Misc::Split(m_sDefAllocToList, aItems, FALSE, ENDL);
	prefs.WriteArrayItems(aItems, _T("Preferences\\AllocToList"));

	Misc::Split(m_sDefAllocByList, aItems, FALSE, ENDL);
	prefs.WriteArrayItems(aItems, _T("Preferences\\AllocByList"));
}
