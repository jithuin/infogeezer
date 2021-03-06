#if !defined(AFX_RICHEDITBASECTRL_H__E7F84BEA_24A6_42D4_BE92_4B8891484048__INCLUDED_)
#define AFX_RICHEDITBASECTRL_H__E7F84BEA_24A6_42D4_BE92_4B8891484048__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// olericheditctrl.h : header file
//

#include <richole.h>

/////////////////////////////////////////////////////////////////////////////
// CRichEditBaseCtrl window

#ifndef CP_UNICODE
#	define CP_UNICODE 1200
#endif

#ifndef EM_SETTEXTEX
#	define EM_SETTEXTEX (WM_USER + 97)
#	define ST_DEFAULT 0
#	define ST_KEEPUNDO 1
#	define ST_SELECTION 2

	struct SETTEXTEX
	{
		DWORD flags; 
		UINT  codepage; 
	};
#endif

enum // REBC_BORDERS
{
	REBCB_NONE		= 0x00,
	REBCB_TOP		= 0x01,
	REBCB_LEFT		= 0x02,
	REBCB_BOTTOM	= 0x04,
	REBCB_RIGHT		= 0x08,
	REBCB_ALL		= 0x0f
};

class CRichEditBaseCtrl : public CRichEditCtrl
{
// Construction
public:
	CRichEditBaseCtrl();
	virtual ~CRichEditBaseCtrl();

	void DoEditFind(UINT nIDTitle = 0);
	void DoEditReplace(UINT nIDTitle = 0);
	
	BOOL Undo();
	BOOL Redo();

	CString GetTextRange(const CHARRANGE& cr);
	CString GetSelText();
	BOOL SetTextEx(const CString& sText, DWORD dwFlags = ST_KEEPUNDO | ST_SELECTION, UINT nCodePage = CP_ACP); 

	void EnableSelectOnFocus(BOOL bEnable) { m_bEnableSelectOnFocus = bEnable; }
	void SelectCurrentWord();

	void GetMargins(CRect& rMargins) { rMargins = m_rMargins; }
	void SetMargins(LPCRECT pMargins);
	void SetMargins(int nLeft, int nTop, int nRight, int nBottom);
	void SetMargins(int nMargin);
	
	BOOL InsertTable(int nRows, int nCols, int nColWidth = 1000, int nTextIndent = 50, DWORD dwBorders = REBCB_ALL);
	BOOL InsertHorizontalLine(int nWidth = -1);

	// Attributes
protected:
	BOOL m_bEnableSelectOnFocus;
	BOOL m_bInOnFocus;
	CRect m_rMargins;
	
#undef FINDTEXTEX
#undef TEXTRANGE
#ifdef _UNICODE
#define TEXTRANGE	TEXTRANGEW
#define FINDTEXTEX	FINDTEXTEXW
#else
#define TEXTRANGE	TEXTRANGEA
#define FINDTEXTEX	FINDTEXTEXA
#endif /* UNICODE */

	struct FIND_STATE
	{
		FIND_STATE() : pFindReplaceDlg(NULL), bFindOnly(FALSE), bCase(FALSE), bNext(TRUE), bWord(FALSE) { }

		CFindReplaceDialog* pFindReplaceDlg; // find or replace dialog
		BOOL bFindOnly; // Is pFindReplace the find or replace?
		CString strFind;    // last find string
		CString strReplace; // last replace string
		BOOL bCase; // TRUE==case sensitive, FALSE==not
		int bNext;  // TRUE==search down, FALSE== search up
		BOOL bWord; // TRUE==match whole word, FALSE==not
	};
	FIND_STATE m_findState;
	
	// Operations
public:
	
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CRichEditBaseCtrl)
protected:
	virtual void PreSubclassWindow();
	//}}AFX_VIRTUAL
	
	// Implementation
protected:
	class CRichEditOleCallback : public IRichEditOleCallback
	{
	public:
		CRichEditOleCallback();
		virtual ~CRichEditOleCallback();
		
		void SetOwner(CRichEditBaseCtrl* pOwner) { m_pOwner = pOwner; }
		
		// IRichEditOleCallback
		virtual HRESULT STDMETHODCALLTYPE GetNewStorage(LPSTORAGE* lplpstg);
		virtual HRESULT STDMETHODCALLTYPE QueryInterface(REFIID iid, void ** ppvObject);
		virtual ULONG   STDMETHODCALLTYPE AddRef();
		virtual ULONG   STDMETHODCALLTYPE Release();
		virtual HRESULT STDMETHODCALLTYPE GetInPlaceContext(LPOLEINPLACEFRAME FAR *lplpFrame,
			LPOLEINPLACEUIWINDOW FAR *lplpDoc, LPOLEINPLACEFRAMEINFO lpFrameInfo);
		virtual HRESULT STDMETHODCALLTYPE ShowContainerUI(BOOL fShow);
		virtual HRESULT STDMETHODCALLTYPE QueryInsertObject(LPCLSID lpclsid, LPSTORAGE lpstg, LONG cp);
		virtual HRESULT STDMETHODCALLTYPE DeleteObject(LPOLEOBJECT lpoleobj);
		virtual HRESULT STDMETHODCALLTYPE QueryAcceptData(LPDATAOBJECT lpdataobj, CLIPFORMAT FAR *lpcfFormat,
            DWORD reco, BOOL fReally, HGLOBAL hMetaPict);
		virtual HRESULT STDMETHODCALLTYPE ContextSensitiveHelp(BOOL fEnterMode);
		virtual HRESULT STDMETHODCALLTYPE GetClipboardData(CHARRANGE FAR *lpchrg, DWORD reco, LPDATAOBJECT FAR *lplpdataobj);
		virtual HRESULT STDMETHODCALLTYPE GetDragDropEffect(BOOL fDrag, DWORD grfKeyState, LPDWORD pdwEffect);
		virtual HRESULT STDMETHODCALLTYPE GetContextMenu(WORD seltyp, LPOLEOBJECT lpoleobj, CHARRANGE FAR *lpchrg,
            HMENU FAR *lphmenu);
		
	protected:
		int m_iNumStorages;
		IStorage* m_pStorage;
		DWORD m_dwRef;
		CRichEditBaseCtrl* m_pOwner;
	};
	
	friend class CRichEditOleCallback;
	
protected:
	CRichEditOleCallback m_callback;
	
	// Generated message map functions
protected:
	virtual HRESULT GetNewStorage(LPSTORAGE* /*lplpstg*/) { return S_OK; }
	virtual HRESULT GetInPlaceContext(LPOLEINPLACEFRAME FAR* /*lplpFrame*/,
		LPOLEINPLACEUIWINDOW FAR* /*lplpDoc*/, LPOLEINPLACEFRAMEINFO /*lpFrameInfo*/) { return S_FALSE; }
	virtual HRESULT ShowContainerUI(BOOL /*fShow*/) { return S_FALSE; }
	virtual HRESULT QueryInsertObject(LPCLSID /*lpclsid*/, LPSTORAGE /*lpstg*/, LONG /*cp*/) { return S_OK; }
	virtual HRESULT DeleteObject(LPOLEOBJECT /*lpoleobj*/) { return S_OK; }
	virtual HRESULT QueryAcceptData(LPDATAOBJECT /*lpdataobj*/, CLIPFORMAT FAR* /*lpcfFormat*/,
		DWORD /*reco*/, BOOL /*fReally*/, HGLOBAL /*hMetaPict*/) { return S_OK; }
	virtual HRESULT ContextSensitiveHelp(BOOL /*fEnterMode*/) { return S_OK; }
	virtual HRESULT GetClipboardData(CHARRANGE FAR* /*lpchrg*/, DWORD /*reco*/, LPDATAOBJECT FAR* /*lplpdataobj*/) { return E_NOTIMPL; }
	virtual HRESULT GetDragDropEffect(BOOL /*fDrag*/, DWORD /*grfKeyState*/, LPDWORD /*pdwEffect*/) { return S_OK; }
	virtual HRESULT GetContextMenu(WORD /*seltyp*/, LPOLEOBJECT /*lpoleobj*/, CHARRANGE FAR* /*lpchrg*/,
		HMENU FAR* /*lphmenu*/) { return S_OK; }
	
	virtual void OnFindNext(LPCTSTR lpszFind, BOOL bNext, BOOL bCase, BOOL bWord);
	virtual void OnReplaceSel(LPCTSTR lpszFind, BOOL bNext, BOOL bCase,
		BOOL bWord, LPCTSTR lpszReplace);
	virtual void OnReplaceAll(LPCTSTR lpszFind, LPCTSTR lpszReplace,
		BOOL bCase, BOOL bWord);
	
	//{{AFX_MSG(CRichEditBaseCtrl)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	//}}AFX_MSG
	afx_msg LRESULT OnFindReplaceCmd(WPARAM wParam, LPARAM lParam);
	afx_msg void OnDestroy();
	afx_msg void OnSetFocus(CWnd* pOldWnd);
	afx_msg LRESULT OnEditSetSelection(WPARAM wParam, LPARAM lParam);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	
	DECLARE_MESSAGE_MAP()
		
	void AdjustDialogPosition(CDialog* pDlg);
	BOOL FindText(LPCTSTR lpszFind, BOOL bCase = TRUE, BOOL bWord = TRUE, BOOL bWrap = TRUE);
	void TextNotFound(LPCTSTR lpszFind);
	BOOL FindText(BOOL bWrap = TRUE);
	long FindAndSelect(DWORD dwFlags, FINDTEXTEX& ft);
	void DoEditFindReplace(BOOL bFindOnly, UINT nIDTitle);
	BOOL SameAsSelected(LPCTSTR lpszCompare, BOOL bCase, BOOL bWord);
	BOOL IsFindDialog(HWND hwnd) const;

	virtual CFindReplaceDialog* NewFindReplaceDlg() { return new CFindReplaceDialog; }
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_RICHEDITBASECTRL_H__E7F84BEA_24A6_42D4_BE92_4B8891484048__INCLUDED_)
