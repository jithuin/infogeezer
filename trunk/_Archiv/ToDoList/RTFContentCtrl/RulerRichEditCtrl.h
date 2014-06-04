#if !defined(AFX_RULERRICHEDITCTRL_H__4CD13283_82E4_484A_83B4_DBAD5B64F17C__INCLUDED_)
#define AFX_RULERRICHEDITCTRL_H__4CD13283_82E4_484A_83B4_DBAD5B64F17C__INCLUDED_

#include "RulerRichEdit.h"
#include "RRECRuler.h"
#include "RRECToolbar.h"
#include "ids.h"

/////////////////////////////////////////////////////////////////////////////
// Helper structs

struct CharFormat : public CHARFORMAT2
{
	CharFormat(DWORD mask = 0)
	{
		memset(this, 0, sizeof (CharFormat));
		cbSize = sizeof(CharFormat);
		dwMask = mask;
	};

};

struct ParaFormat2 : public PARAFORMAT2
{
	ParaFormat2(DWORD mask = 0)
	{
		memset(this, 0, sizeof (ParaFormat2));
		cbSize = sizeof(ParaFormat2);
		dwMask = mask;
	}
};

struct ParaFormat : public PARAFORMAT
{
	ParaFormat(DWORD mask = 0)
	{
		memset(this, 0, sizeof (ParaFormat));
		cbSize = sizeof(ParaFormat);
		dwMask = mask;
	}
};

/////////////////////////////////////////////////////////////////////////////
// CRulerRichEditCtrl window

class CRulerRichEditCtrl : public CWnd
{

public:
// Construction/creation/destruction
	CRulerRichEditCtrl();
	virtual ~CRulerRichEditCtrl();
	virtual BOOL Create(DWORD dwStyle, const RECT &rect, CWnd* pParentWnd, UINT nID, BOOL autohscroll = FALSE);

// Attributes
	void	SetMode(int mode);
	int		GetMode() const;

	void ShowToolbar(BOOL show = TRUE);
	void ShowRuler(BOOL show = TRUE);

	BOOL IsToolbarVisible() const;
	BOOL IsRulerVisible() const;

	void SetWordWrap(BOOL bWrap = TRUE);
	BOOL HasWordWrap() const { return m_bWordWrap; }

	CRulerRichEdit& GetRichEditCtrl();

	void SetSelectedWebLink(const CString& sWebLink);

	void SetFileLinkOption(RE_PASTE nLinkOption, BOOL bDefault) { m_rtf.SetFileLinkOption(nLinkOption, bDefault); }
	RE_PASTE GetFileLinkOption() const { return m_rtf.GetFileLinkOption(); }
	BOOL IsFileLinkOptionDefault() const { return m_rtf.IsFileLinkOptionDefault(); }

// Implementation
	CString GetRTF();
	int GetRTFLength();
	void	SetRTF(const CString& rtf);
	BOOL	Save(CString& filename);
	BOOL	Load(CString& filename);

	void SetReadOnly(BOOL readOnly);
	BOOL GetReadOnly() const;

protected:
// Formatting
	void DoFont();
	void DoColor();
	void DoBold();
	void DoItalic();
	void DoUnderline();
	void DoLeftAlign();
	void DoCenterAlign();
	void DoRightAlign();
	void DoJustify();
	void DoIndent();
	void DoOutdent();
	void DoBullet();
	void DoNumberList();
	void DoStrikethrough();
	void DoSuperscript();
	void DoSubscript();
	void DoInsertTable();
	void DoInsertHorzLine();

	void IncrementFontSize(BOOL bIncrement = TRUE);
	void SetCurrentFontName(const CString& font);
	void SetCurrentFontSize(int points);
	void SetCurrentFontColor(COLORREF color, BOOL bForeground);

	//void InsertHorizontalRule();

// Overrides
	//{{AFX_VIRTUAL(CRulerRichEditCtrl)
	protected:
	virtual BOOL OnNotify(WPARAM wParam, LPARAM lParam, LRESULT* pResult);
	//}}AFX_VIRTUAL

protected:
// Message handlers
	//{{AFX_MSG(CRulerRichEditCtrl)
	afx_msg void OnPaint();
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnButtonFont();
	afx_msg void OnButtonColor();
	afx_msg void OnButtonBold();
	afx_msg void OnButtonStrikethrough();
	afx_msg void OnButtonItalic();
	afx_msg void OnButtonUnderline();
	afx_msg void OnButtonLeftAlign();
	afx_msg void OnButtonCenterAlign();
	afx_msg void OnButtonRightAlign();
	afx_msg void OnButtonIndent();
	afx_msg void OnButtonOutdent();
	afx_msg void OnButtonBullet();
	afx_msg void OnSetFocus(CWnd* pOldWnd);
	afx_msg LRESULT OnSetText (WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGetText (WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnGetTextLength (WPARAM wParam, LPARAM lParam);
	afx_msg void OnEnable(BOOL bEnable);
	//}}AFX_MSG
	afx_msg void OnKillFocusToolbar(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnButtonNumberList();
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg LRESULT OnSetFont(WPARAM wp, LPARAM lp);
	afx_msg void OnEnHScroll();
	afx_msg void OnEnSelChange(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg LRESULT OnUpdateToolbar(WPARAM wParam, LPARAM lParam);
	afx_msg void OnButtonTextColor();
	afx_msg void OnButtonBackColor();
	afx_msg void OnButtonWordwrap();
	afx_msg void OnButtonInsertTable();
	afx_msg void OnButtonJustify();
	afx_msg void OnButtonShrinkFont();
	afx_msg void OnButtonGrowFont();

	LRESULT OnTrackRuler(WPARAM mode, LPARAM pt);
	LRESULT OnGetScrollPos(WPARAM, LPARAM);
	LRESULT OnSetCurrentFontName(WPARAM font, LPARAM size);
	LRESULT OnSetCurrentFontSize(WPARAM font, LPARAM size);
	LRESULT OnSetCurrentFontColor(WPARAM font, LPARAM size);

	DECLARE_MESSAGE_MAP()

protected:
	// Internal data
	int				m_rulerPosition;	// The x-position of the ruler line when dragging a tab
	CPen			m_pen;				// The pen to use for drawing the XORed ruler line

	CDWordArray		m_tabs;				// An array containing the tab-positions in device pixels
	int				m_margin;			// The margin to use for the ruler and buttons

	int				m_physicalInch;		// The number of pixels for an inch on screen
	int				m_movingtab;		// The tab-position being moved, or -1 if none
	int				m_offset;			// Internal offset of the tab-marker being moved.
	CharFormat		m_cfDefault;
	CHARRANGE		m_crSel;

	BOOL			m_showToolbar;
	BOOL			m_showRuler;
	BOOL			m_readOnly;
	BOOL			m_bWordWrap;
	
	// Sub-controls
	CRulerRichEdit	m_rtf;
	CRRECToolBar	m_toolbar;
	CRRECRuler		m_ruler;

	// Handle to the RTF 2.0 dll
//	HINSTANCE		m_richEditModule;

	// Private helpers
	void	SetTabStops(LPLONG tabs, int size);
	void	UpdateTabStops();

	BOOL	CreateToolbar();
	BOOL	CreateRuler();
	BOOL	CreateRTFControl(BOOL autohscroll);
	void	CreateMargins();

	void	UpdateToolbarButtons();
	void	UpdateEditRect();

	void	SetEffect(int mask, int effect);
	void	SetAlignment(int alignment);

	void	LayoutControls(int width, int height);

	BOOL FixupTabStops(ParaFormat& para);
	void BuildTabStops(ParaFormat& para);

};

#endif // !defined(AFX_RULERRICHEDITCTRL_H__4CD13283_82E4_484A_83B4_DBAD5B64F17C__INCLUDED_)
