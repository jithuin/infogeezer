#if !defined(AFX_RTFCONTENTCONTROL_H__4F1A93A0_7829_4DBB_AA0B_A2F62E4E7F50__INCLUDED_)
#define AFX_RTFCONTENTCONTROL_H__4F1A93A0_7829_4DBB_AA0B_A2F62E4E7F50__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// RTFContentControl.h : header file
//

#include "rulerricheditctrl.h"

#include "..\shared\IContentControl.h"
#include "..\shared\toolbarhelper.h"
#include "..\shared\richeditspellcheck.h"
#include "..\shared\subclass.h"
#include "..\shared\menuiconmgr.h"

/////////////////////////////////////////////////////////////////////////////
// CRTFContentControl window

static const char* RTFTAG = "{\\rtf"; // always in bytes
static int LENTAG = strlen(RTFTAG); // in bytes

class CRTFContentControl : public CRulerRichEditCtrl, public IContentControl
{
// Construction
public:
	CRTFContentControl();

	// ICustomControl implementation
	int GetContent(unsigned char* pContent) const;
	bool SetContent(const unsigned char* pContent, int nLength, BOOL bResetSelection);
	int GetTextContent(LPTSTR szContent, int nLength = -1) const;
	bool SetTextContent(LPCTSTR szContent, BOOL bResetSelection);
	void SetReadOnly(bool bReadOnly);
	HWND GetHwnd() const;
	LPCTSTR GetTypeID() const;
	void Release();
	bool ProcessMessage(MSG* pMsg);
	ISpellCheck* GetSpellCheckInterface() { return &m_reSpellCheck; }
	bool Undo() { return GetRichEditCtrl().Undo() != 0; }
	bool Redo() { return GetRichEditCtrl().Redo() != 0; }
	void SetUITheme(const UITHEME* pTheme);
	void SavePreferences(IPreferences* pPrefs, LPCTSTR szKey) const;
	void LoadPreferences(const IPreferences* pPrefs, LPCTSTR szKey);

// Attributes
protected:
	BOOL m_bAllowNotify;
	CToolbarHelper m_tbHelper;
	CRichEditSpellCheck m_reSpellCheck;
//	CToolTipCtrl m_tt;
	CMenuIconMgr m_mgrMenuIcons;
	CharFormat m_cfCopiedFormat;

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CRTFContentControl)
	public:
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CRTFContentControl();

	// Generated message map functions
protected:
	//{{AFX_MSG(CRTFContentControl)
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	//}}AFX_MSG
	afx_msg void OnChangeText();
	afx_msg void OnKillFocus();
	afx_msg LRESULT OnSetFont(WPARAM wp, LPARAM lp);
	afx_msg void OnStyleChanging(int nStyleType, LPSTYLESTRUCT lpStyleStruct);
	afx_msg LRESULT OnCustomUrl(WPARAM wp, LPARAM lp);
//	afx_msg void OnNeedTooltipText(UINT nID, NMHDR* pNMHDR, LRESULT* pResult);

	DECLARE_MESSAGE_MAP()

//	virtual LRESULT ScWindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp);

	BOOL CanPaste();
	BOOL Paste(BOOL bSimple = FALSE);
	BOOL IsTDLClipboardEmpty() const;
	void InitMenuIconManager();

	static int GetContent(const CRTFContentControl* pCtrl, unsigned char* pContent);
	static void EnableMenuItem(CMenu* pMenu, UINT nCmdID, BOOL bEnable);
	static void CheckMenuItem(CMenu* pMenu, UINT nCmdID, BOOL bCheck);
	static BOOL RemoveAdvancedFeatures(CMenu* pMenu);
	static void RemoveAdvancedFeature(CMenu* pMenu, UINT nCmdID);
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_RTFCONTENTCONTROL_H__4F1A93A0_7829_4DBB_AA0B_A2F62E4E7F50__INCLUDED_)
