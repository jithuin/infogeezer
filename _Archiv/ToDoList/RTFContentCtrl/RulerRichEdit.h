#if !defined(AFX_RULERRICHEDIT_H__E10A8ED3_2E1D_402E_A599_003214085F1A__INCLUDED_)
#define AFX_RULERRICHEDIT_H__E10A8ED3_2E1D_402E_A599_003214085F1A__INCLUDED_

// RulerRichEdit.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CRulerRichEdit window

#include "..\shared\urlricheditctrl.h"
#include "..\shared\richedithelper.h"

class CRulerRichEdit : public CUrlRichEditCtrl
{
public:
// Construction/destruction
	CRulerRichEdit();
	virtual ~CRulerRichEdit();

	void Paste(BOOL bSimple = FALSE);
	BOOL IsIMEComposing() const { return m_bIMEComposing; }
	BOOL SetParaFormat(PARAFORMAT2 &pf);
	BOOL SetParaFormat(PARAFORMAT &pf);
	BOOL GetParaFormat(PARAFORMAT2 &pf);
	BOOL GetParaFormat(PARAFORMAT &pf);
	DWORD GetSelectionCharFormat(CHARFORMAT2 &cf) const;
	DWORD GetSelectionCharFormat(CHARFORMAT &cf) const;
	BOOL SetSelectionCharFormat(CHARFORMAT2 &cf);
	BOOL SetSelectionCharFormat(CHARFORMAT &cf);

	void SetFileLinkOption(RE_PASTE nLinkOption, BOOL bDefault);
	RE_PASTE GetFileLinkOption() const { return m_nFileLinkOption; }
	BOOL IsFileLinkOptionDefault() const { return m_bLinkOptionIsDefault; }

protected:
	BOOL m_bPasteSimple;
	BOOL m_bIMEComposing;
	RE_PASTE m_nFileLinkOption;
	BOOL m_bLinkOptionIsDefault;

protected:
// Message handlers
	//{{AFX_MSG(CRulerRichEdit)
	afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
	afx_msg UINT OnGetDlgCode();
	afx_msg void OnLButtonDblClk(UINT nFlags, CPoint point);
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	//}}AFX_MSG
	afx_msg LRESULT OnDropFiles(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnIMEStartComposition(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnIMEEndComposition(WPARAM wp, LPARAM lp);

	DECLARE_MESSAGE_MAP()

	virtual CLIPFORMAT GetAcceptableClipFormat(LPDATAOBJECT lpDataOb, CLIPFORMAT format);
	virtual HRESULT GetDragDropEffect(BOOL fDrag, DWORD grfKeyState, LPDWORD pdwEffect);

	virtual CFindReplaceDialog* NewFindReplaceDlg();
};

#endif // !defined(AFX_RULERRICHEDIT_H__E10A8ED3_2E1D_402E_A599_003214085F1A__INCLUDED_)
