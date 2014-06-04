#if !defined(AFX_TDLCSVCOLUMNSETUPLISTCTRL_H__E379E120_FF91_417F_ADBB_0DD6A98089AA__INCLUDED_)
#define AFX_TDLCSVCOLUMNSETUPLISTCTRL_H__E379E120_FF91_417F_ADBB_0DD6A98089AA__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// TDLCsvColumnSetupListCtrl.h : header file
//

#include "tdcenum.h"

#include "..\shared\InputListCtrl.h"

/////////////////////////////////////////////////////////////////////////////
// CTDLCsvAttributeSetupListCtrl window

struct CSVCOLUMNMAPPING
{
	CSVCOLUMNMAPPING() : nTDCAttrib(TDCA_NONE) {}
	CSVCOLUMNMAPPING(const CString& sName, TDC_ATTRIBUTE tdcAttrib, DWORD dwData = 0) 
	{ 
		sColumnName = sName; 
		nTDCAttrib = tdcAttrib;
		dwItemData = dwData;
	}

	CSVCOLUMNMAPPING(UINT nNameID, TDC_ATTRIBUTE tdcAttrib, DWORD dwData = 0) 
	{ 
		sColumnName.LoadString(nNameID); 
		nTDCAttrib = tdcAttrib;
		dwItemData = dwData;
	}

	TDC_ATTRIBUTE nTDCAttrib;
	CEnString sColumnName;
	DWORD dwItemData;
};

class CTDCCsvColumnMapping : public CArray<CSVCOLUMNMAPPING, CSVCOLUMNMAPPING&>{};

class CTDLCsvAttributeSetupListCtrl : public CInputListCtrl
{
// Construction
public:
	CTDLCsvAttributeSetupListCtrl(BOOL bImporting, BOOL bOneToOneMapping = TRUE);

	void SetColumnMapping(const CTDCCsvColumnMapping& aMapping);
	int GetColumnMapping(CTDCCsvColumnMapping& aMapping) const;

// attributes
protected:
	CComboBox m_cbAttributes;
	CTDCCsvColumnMapping m_aMapping;
	BOOL m_bImporting, m_bOneToOneMapping;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDLCsvAttributeSetupListCtrl)
	protected:
	virtual void PreSubclassWindow();
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CTDLCsvAttributeSetupListCtrl();

	// Generated message map functions
protected:
	//{{AFX_MSG(CTDLCsvAttributeSetupListCtrl)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG
	afx_msg void MeasureItem(LPMEASUREITEMSTRUCT lpMeasureItemStruct);
	afx_msg void OnAttribEditCancel();
	afx_msg void OnAttribEditOK();
	afx_msg void OnNameEditOK(NMHDR* pNMHDR, LRESULT* pResult);

	DECLARE_MESSAGE_MAP()

protected:
	void BuildListCtrl();
	
	virtual void EditCell(int nItem, int nCol);
	virtual BOOL IsEditing() const;
	virtual BOOL CanDeleteSelectedCell() const;
	virtual BOOL DeleteSelectedCell();
	virtual BOOL CanEditSelectedCell() const;

	void PrepareEdit(int nRow, int nCol);

	int FindMappedAttribute(TDC_ATTRIBUTE nAttrib, int nIgnoreRow = -1) const;
	int FindMappedAttribute(const CString& sName, int nIgnoreRow = -1) const;

	static CString GetAttributeName(TDC_ATTRIBUTE nAttrib);
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TDLCSVCOLUMNSETUPLISTCTRL_H__E379E120_FF91_417F_ADBB_0DD6A98089AA__INCLUDED_)
