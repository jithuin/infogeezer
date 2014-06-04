#if !defined(AFX_TDLIMPORTOUTLOOKMSGSDLG_H__56B41CA4_32AC_485E_8436_49958724F2C9__INCLUDED_)
#define AFX_TDLIMPORTOUTLOOKMSGSDLG_H__56B41CA4_32AC_485E_8436_49958724F2C9__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// TDLImportOutlookMsgsDlg.h : header file
//

#include "tdlcsvattributesetuplistctrl.h"

#include "..\shared\OutlookHelper.h"
#include "..\shared\enstring.h"

/////////////////////////////////////////////////////////////////////////////
// CTDLImportOutlookObjectsDlg dialog

// predecs
struct OUTLOOK_FIELD;

class CTDLImportOutlookObjectsDlg : public CDialog
{
// Construction
public:
	CTDLImportOutlookObjectsDlg(OutlookAPI::_MailItem& refItem, CWnd* pParent = NULL);   // standard constructor

	BOOL GetWantConfidentialAttributes() const { return !m_bHideConfidential; }
	int GetColumnMapping(CTDCCsvColumnMapping& aMapping);

	static CString GetOutlookFieldName(OUTLOOK_FIELDTYPE nFieldType);

protected:
// Dialog Data
	//{{AFX_DATA(CTDLImportOutlookObjectsDlg)
	enum { IDD = IDD_OUTLOOKMSGIMPORT_DIALOG };
	BOOL	m_bHideUnmapped;
	BOOL	m_bHideConfidential;
	//}}AFX_DATA
	CTDLCsvAttributeSetupListCtrl m_lcFieldMapping;
	COutlookItemDataMap m_mapRefData;
	CString m_sClass;
	OutlookAPI::_MailItem& m_refItem;

	// the master mapping always contains all fields
	// with their last mapped attributes, whilst the mapping
	// passed to m_lcFieldMapping just contains the visible
	// items
	CTDCCsvColumnMapping m_aMasterMapping;

	static CEnString CONFIDENTIAL;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDLImportOutlookObjectsDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	virtual void OnOK();

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CTDLImportOutlookObjectsDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnHideAttributes();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

	void BuildMasterMapping();
	void SaveMasterMapping() const;
	void UpdateMasterMapping();

	CString FormatFieldAndData(const OUTLOOK_FIELD& oaField) const;
	void RemoveUnwantedAttributes(CTDCCsvColumnMapping& aMapping) const;

	// some mapping helpers
	static int FindField(const CTDCCsvColumnMapping& aMapping, OUTLOOK_FIELDTYPE nFieldType);
	static TDC_ATTRIBUTE GetFieldMapping(const CTDCCsvColumnMapping& aMapping, OUTLOOK_FIELDTYPE nFieldType);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TDLIMPORTOUTLOOKMSGSDLG_H__56B41CA4_32AC_485E_8436_49958724F2C9__INCLUDED_)
