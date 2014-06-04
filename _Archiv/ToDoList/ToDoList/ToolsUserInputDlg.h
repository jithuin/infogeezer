#if !defined(AFX_TOOLSUSERINPUTDLG_H__7C10499F_E103_4106_8581_DCD5D55FAEF5__INCLUDED_)
#define AFX_TOOLSUSERINPUTDLG_H__7C10499F_E103_4106_8581_DCD5D55FAEF5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ToolsUserInputDlg.h : header file
//

#include "ToolsCmdlineParser.h"
#include "..\shared\runtimedlg.h"
#include "..\shared\dialoghelper.h"

#include <afxtempl.h>

/////////////////////////////////////////////////////////////////////////////
// CToolsUserInputDlg dialog

class CToolsUserInputDlg : public CRuntimeDlg
{
// Construction
public:
	CToolsUserInputDlg(const CToolsCmdlineParser& tcp); 
	virtual ~CToolsUserInputDlg();

	int DoModal(LPCTSTR szTitle) { return CRuntimeDlg::DoModal(szTitle); }
	CString GetResult(LPCTSTR szItemName);

protected:
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CToolsUserInputDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL
	virtual void OnOK();

	struct TUINPUTITEM
	{
		UINT nCtrlID;
		CString sLabel; // this will be a simple static text item
		CString sName;
		CString sDefValue;
		CLA_TYPE nType;
		UINT nStyle;
		CSize sizeDLU;
		CWnd* pCtrl; // ptr to dynamically allocated items
	};

	CArray<TUINPUTITEM, TUINPUTITEM&> m_aInputItems;
	CMapStringToString m_mapResults; // mapped by name

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CToolsUserInputDlg)
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TOOLSUSERINPUTDLG_H__7C10499F_E103_4106_8581_DCD5D55FAEF5__INCLUDED_)
