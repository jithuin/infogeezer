#if !defined(AFX_TDLLANGUAGECOMBOBOX_H__5EEB7D2C_19CF_460E_B188_646AB514678E__INCLUDED_)
#define AFX_TDLLANGUAGECOMBOBOX_H__5EEB7D2C_19CF_460E_B188_646AB514678E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// TDLLanguageComboBox.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CTDLLanguageComboBox window

class CTDLLanguageComboBox : public CComboBoxEx
{
// Construction
public:
	CTDLLanguageComboBox(LPCTSTR szFilter = _T("*.csv"));

	void SetLanguageFile(LPCTSTR szFile);
	CString GetLanguageFile(BOOL bRelative = FALSE) const;
	BOOL IsDefaultLanguage() const;
	BOOL HasYourLanguage() const;

	int SelectLanguage(LPCTSTR szLanguage);

	static CString GetLanguageFile(LPCTSTR szLanguage, LPCTSTR szExt = _T("csv"), BOOL bRelative = FALSE);
	static CString GetDefaultLanguage();

protected:
	mutable CString m_sSelLanguage;
	CString m_sFilter;
	CImageList m_il;

protected:
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDLLanguageComboBox)
	protected:
	virtual void PreSubclassWindow();
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CTDLLanguageComboBox();

	// Generated message map functions
protected:
	//{{AFX_MSG(CTDLLanguageComboBox)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnDestroy();
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()

protected:
	void BuildLanguageList();
	int AddString(LPCTSTR szLanguage, HBITMAP hbmFlag, DWORD dwItemData);

	static CString GetTranslationFolder();
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TDLLANGUAGECOMBOBOX_H__5EEB7D2C_19CF_460E_B188_646AB514678E__INCLUDED_)
