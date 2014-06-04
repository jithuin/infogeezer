#if !defined(AFX_TDLCOLUMNSELECTIONDLG_H__B8AF67E2_DDBA_4D08_B930_397D338F9D43__INCLUDED_)
#define AFX_TDLCOLUMNSELECTIONDLG_H__B8AF67E2_DDBA_4D08_B930_397D338F9D43__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// TDLColumnSelectionDlg.h : header file
//

#include "tdlcolumnlistbox.h"

/////////////////////////////////////////////////////////////////////////////
// CTDLColumnSelectionDlg dialog

class CTDLColumnSelectionDlg : public CDialog
{
// Construction
public:
	CTDLColumnSelectionDlg(const CTDCColumnIDArray& aColumns, const CTDCColumnIDArray& sDefault, 
							BOOL bActiveTasklist, CWnd* pParent = NULL);   // standard constructor

	int GetVisibleColumns(CTDCColumnIDArray& aColumns) const;
	BOOL GetApplyActiveTasklist() const { return m_bActiveTasklist; }

protected:
// Dialog Data
	//{{AFX_DATA(CTDLColumnSelectionDlg)
	enum { IDD = IDD_COLUMNSELECTION_DIALOG };
	CTDLColumnListBox	m_lbColumns;
	int		m_bActiveTasklist;
	//}}AFX_DATA
	CTDCColumnIDArray m_aVisibleColumns, m_aDefaultColumns;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDLColumnSelectionDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	virtual void OnOK();

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CTDLColumnSelectionDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSelectallcols();
	afx_msg void OnClearallcols();
	afx_msg void OnDefaultcols();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TDLCOLUMNSELECTIONDLG_H__B8AF67E2_DDBA_4D08_B930_397D338F9D43__INCLUDED_)
