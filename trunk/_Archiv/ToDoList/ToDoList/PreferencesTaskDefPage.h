#if !defined(AFX_PREFERENCESTASKDEFPAGE_H__852964E3_4ABD_4B66_88BA_F553177616F2__INCLUDED_)
#define AFX_PREFERENCESTASKDEFPAGE_H__852964E3_4ABD_4B66_88BA_F553177616F2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PreferencesTaskDefPage.h : header file
//

#include "tdcenum.h"
#include "tdlprioritycombobox.h"
#include "tdlriskcombobox.h"

#include <afxtempl.h>

#include "..\shared\preferencesbase.h"
#include "..\shared\colorbutton.h"
#include "..\shared\timeedit.h"
#include "..\shared\wndPrompt.h"
#include "..\shared\autocombobox.h"
#include "..\shared\groupline.h"
#include "..\shared\dialoghelper.h"
#include "..\shared\maskedit.h"
#include "..\Shared\checklistboxex.h"

/////////////////////////////////////////////////////////////////////////////
// CPreferencesTaskDefPage dialog

enum PTP_ATTRIB
{
	PTPA_NONE = -1,
	PTPA_PRIORITY,
	PTPA_COLOR,
	PTPA_ALLOCTO,
	PTPA_ALLOCBY,
	PTPA_STATUS,
	PTPA_CATEGORY,
	PTPA_TIMEEST,
	PTPA_RISK,
	PTPA_DUEDATE,
	PTPA_VERSION,
	PTPA_STARTDATE,
	PTPA_FLAG,
	PTPA_EXTERNALID,
	PTPA_TAGS,
	// add to end
};

enum PTDP_LIST
{ 
	PTDP_CATEGORY, 
	PTDP_STATUS, 
	PTDP_ALLOCTO, 
	PTDP_ALLOCBY 
};
// wParam = MAKEWPARAM(enum, 0 for delete, 1 for add)
// lParam = LPCTSTR
const UINT WM_PTDP_LISTCHANGE = ::RegisterWindowMessage(_T("WM_PTDP_LISTCHANGE"));

class CPreferencesTaskDefPage : public CPreferencesPageBase, public CDialogHelper
{
	DECLARE_DYNCREATE(CPreferencesTaskDefPage)

// Construction
public:
	CPreferencesTaskDefPage();
	~CPreferencesTaskDefPage();

	void SetPriorityColors(const CDWordArray& aColors);

	int GetDefaultPriority() const { return m_nDefPriority; }
	int GetDefaultRisk() const { return m_nDefRisk; }
	int GetDefaultAllocTo(CStringArray& aAllocTo) const;
	CString GetDefaultAllocBy() const { return m_sDefAllocBy; }
	CString GetDefaultStatus() const { return m_sDefStatus; }
	int GetDefaultTags(CStringArray& aTags) const;
	int GetDefaultCategories(CStringArray& aCats) const;
	CString GetDefaultCreatedBy() const { return m_sDefCreatedBy; }
	double GetDefaultTimeEst(TCHAR& nUnits) const;
	double GetDefaultTimeSpent(TCHAR& nUnits) const;
	double GetDefaultCost() const { return m_dDefCost; }
	COLORREF GetDefaultColor() const { return m_crDef; }
	BOOL GetAutoDefaultStartDate() const { return m_bUseCreationForDefStartDate; }
	int GetParentAttribsUsed(CTDCAttributeArray& aAttribs, BOOL& bUpdateAttrib) const;

	int GetListItems(PTDP_LIST nList, CStringArray& aItems) const;
	BOOL AddListItem(PTDP_LIST nList, LPCTSTR szItem);
	BOOL DeleteListItem(PTDP_LIST nList, LPCTSTR szItem);

protected:
// Dialog Data
	//{{AFX_DATA(CPreferencesTaskDefPage)
	enum { IDD = IDD_PREFTASKDEF_PAGE };
	CTDLRiskComboBox	m_cbDefRisk;
	CTDLPriorityComboBox	m_cbDefPriority;
	CString	m_sDefCreatedBy;
	double	m_dDefCost;
	BOOL	m_bUpdateInheritAttributes;
	CString	m_sDefCategoryList;
	CString	m_sDefStatusList;
	CString	m_sDefAllocToList;
	CString	m_sDefAllocByList;
	//}}AFX_DATA
	CTimeEdit	m_eTimeEst;
	CTimeEdit	m_eTimeSpent;
	CMaskEdit m_eCost;
	CCheckListBoxEx	m_lbAttribUse;
	int		m_nDefPriority;
	int		m_nDefRisk;
	double		m_dDefTimeEst, m_dDefTimeSpent;
	CString	m_sDefAllocTo;
	CString	m_sDefAllocBy;
	CString	m_sDefStatus;
	CString	m_sDefTags;
	CString	m_sDefCategory;
	CColorButton	m_btDefColor;
	COLORREF m_crDef;
	BOOL	m_bInheritParentAttributes;
	int		m_nSelAttribUse;
	BOOL	m_bUseCreationForDefStartDate;
	CWndPromptManager m_mgrPrompts;
	CGroupLineManager m_mgrGroupLines;

	struct ATTRIBPREF
	{
		ATTRIBPREF() : nAttrib(PTPA_NONE), bUse(FALSE) {}
		ATTRIBPREF(UINT nIDName, PTP_ATTRIB attrib, BOOL use) : nAttrib(attrib), bUse(use) { sName.LoadString(nIDName); }

		CString sName;
		PTP_ATTRIB nAttrib;
		BOOL bUse;
	};
	CArray<ATTRIBPREF, ATTRIBPREF&> m_aAttribPrefs;


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CPreferencesTaskDefPage)
	public:
	virtual void OnOK();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CPreferencesTaskDefPage)
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	afx_msg void OnSetdefaultcolor();
	afx_msg void OnUseparentattrib();
	afx_msg void OnAttribUseChange();
	afx_msg LRESULT OnListAddItem(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnListDeleteItem(WPARAM wp, LPARAM lp);
	DECLARE_MESSAGE_MAP()
		
	virtual void LoadPreferences(const CPreferences& prefs);
	virtual void SavePreferences(CPreferences& prefs);
	
	BOOL HasCheckedAttributes() const;
	
	static PTDP_LIST MapCtrlIDToList(UINT nListID);

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PREFERENCESTASKDEFPAGE_H__852964E3_4ABD_4B66_88BA_F553177616F2__INCLUDED_)
