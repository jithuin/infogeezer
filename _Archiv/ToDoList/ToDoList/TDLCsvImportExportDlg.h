#if !defined(AFX_TDLCSVIMPORTEXPORTDLG_H__3230FA12_9619_426A_9D8A_FC4D76A56596__INCLUDED_)
#define AFX_TDLCSVIMPORTEXPORTDLG_H__3230FA12_9619_426A_9D8A_FC4D76A56596__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// TDLCsvImportExportDlg.h : header file
//

#include "TDLCsvAttributeSetupListCtrl.h"

#include "..\shared\fileedit.h"
#include "..\shared\IPreferences.h"

/////////////////////////////////////////////////////////////////////////////
// CTDLCsvImportExportDlg dialog

class CTDLCsvImportExportDlg : public CDialog
{
// Construction
public:
	CTDLCsvImportExportDlg(const CString& sFilePath, IPreferences* pPrefs, LPCTSTR szKey, CWnd* pParent = NULL);   // import constructor
	CTDLCsvImportExportDlg(const CString& sFilePath, const CTDCAttributeArray& aExportAttributes, IPreferences* pPrefs, LPCTSTR szKey, CWnd* pParent = NULL);   // export constructor

	int GetColumnMapping(CTDCCsvColumnMapping& aMapping) const;
	CString GetDelimiter() const;

protected:
// Dialog Data
	//{{AFX_DATA(CTDLCsvImportExportDlg)
	CString	m_sDelim;
	CString	m_sFilePath;
	BOOL	m_bAlwaysExportTaskIDs;
	//}}AFX_DATA
	CFileEdit	m_eFilePath;
	CTDLCsvAttributeSetupListCtrl m_lcColumnSetup;
	BOOL m_bImporting;
	CTDCCsvColumnMapping m_aMasterColumnMapping;
	CTDCAttributeArray m_aExportAttributes;

	IPreferences* m_pPrefs;
	CString m_sPrefKey;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDLCsvImportExportDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CTDLCsvImportExportDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnChangeCsvdelimiter();
	afx_msg void OnExportTaskids();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
	virtual void OnOK();

protected:
	int BuildImportColumnMapping(CTDCCsvColumnMapping& aImportMapping) const;
	int BuildExportColumnMapping(CTDCCsvColumnMapping& aExportMapping) const;

	void BuildDefaultMasterColumnMapping();
	void UpdateMasterColumnMappingFromList();

	int FindMasterColumn(TDC_ATTRIBUTE attrib) const;
	CString GetMasterColumnName(TDC_ATTRIBUTE attrib) const;
	void SetMasterColumnName(TDC_ATTRIBUTE attrib, LPCTSTR szColumn);

	int FindMasterColumn(LPCTSTR szColumn) const;
	TDC_ATTRIBUTE GetMasterColumnAttribute(LPCTSTR szColumn) const;
	void SetMasterColumnAttribute(LPCTSTR szColumn, TDC_ATTRIBUTE attrib);

	int LoadMasterColumnMapping();
	void SaveMasterColumnMapping() const;

	void InitializeDelimiter();

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TDLCSVIMPORTEXPORTDLG_H__3230FA12_9619_426A_9D8A_FC4D76A56596__INCLUDED_)
