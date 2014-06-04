#if !defined(AFX_PREFERENCESUITASKLISTCOLORSPAGE_H__9612D6FB_2A00_46DA_99A4_1AC6270F060D__INCLUDED_)
#define AFX_PREFERENCESUITASKLISTCOLORSPAGE_H__9612D6FB_2A00_46DA_99A4_1AC6270F060D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PreferencesUITasklistPageColors.h : header file
//

#include "tdcenum.h"

#include "..\shared\colorbutton.h"
#include "..\shared\colorcombobox.h"
#include "..\shared\autocombobox.h"
#include "..\shared\preferencesbase.h"
#include "..\shared\groupline.h"

#include "..\3rdparty\fontcombobox.h"

#include <afxtempl.h>

/////////////////////////////////////////////////////////////////////////////
// CPreferencesUITasklistColorsPage dialog

enum { COLOROPT_ATTRIB, COLOROPT_PRIORITY, COLOROPT_DEFAULT, COLOROPT_NONE };

struct ATTRIBCOLOR
{
	CString sAttrib;
	COLORREF color;
};

const UINT WM_PUITCP_ATTRIBCHANGE = ::RegisterWindowMessage(_T("WM_PUITCP_ATTRIBCHANGE"));

typedef CArray<ATTRIBCOLOR, ATTRIBCOLOR&> CAttribColorArray;

class CPreferencesUITasklistColorsPage : public CPreferencesPageBase
{
	DECLARE_DYNCREATE(CPreferencesUITasklistColorsPage)

// Construction
public:
	CPreferencesUITasklistColorsPage();
	~CPreferencesUITasklistColorsPage();

	BOOL GetColorPriority() const { return m_bColorPriority; }
	int GetTextColorOption() const { return m_nTextColorOption; }
	BOOL GetHidePriorityNumber() const { return m_bHidePriorityNumber; }
	int GetPriorityColors(CDWordArray& aColors) const;
	int GetAttributeColors(TDC_ATTRIBUTE& nAttrib, CAttribColorArray& aColors) const;
	BOOL GetTreeFont(CString& sFaceName, int& nPointSize) const;
	BOOL GetCommentsFont(CString& sFaceName, int& nPointSize) const;
	COLORREF GetGridlineColor() const { return m_bSpecifyGridColor ? m_crGridlines : -1; }
	COLORREF GetDoneTaskColor() const { return m_bSpecifyDoneColor ? m_crDone : -1; }
	COLORREF GetAlternateLineColor() const { return m_bAlternateLineColor ? m_crAltLine : -1; }
	void GetDueTaskColors(COLORREF& crDue, COLORREF& crDueToday) const;
	void GetStartedTaskColors(COLORREF& crStarted, COLORREF& crStartedToday) const;
	BOOL GetColorTaskBackground() const { return m_bColorTaskBackground; }
	BOOL GetCommentsUseTreeFont() const { return m_bSpecifyTreeFont && m_bCommentsUseTreeFont; }
	COLORREF GetFlaggedTaskColor() const { return m_bSpecifyFlaggedColor ? m_crFlagged : -1; }
	COLORREF GetReferenceTaskColor() const { return m_bSpecifyReferenceColor ? m_crReference : -1; }

	void DeleteAttribute(TDC_ATTRIBUTE nAttrib, LPCTSTR szAttrib);
	void AddAttribute(TDC_ATTRIBUTE nAttrib, LPCTSTR szAttrib);

protected:
// Dialog Data
	//{{AFX_DATA(CPreferencesUITasklistColorsPage)
	enum { IDD = IDD_PREFUITASKLISTCOLORS_PAGE };
	CComboBox	m_cbColorByAttribute;
	BOOL	m_bColorTaskBackground;
	BOOL	m_bCommentsUseTreeFont;
	BOOL	m_bHLSColorGradient;
	BOOL	m_bHidePriorityNumber;
	BOOL	m_bAlternateLineColor;
	int		m_nTextColorOption;
	//}}AFX_DATA
	CAutoComboBox	m_cbAttributes;
	CString	m_sSelAttrib;
	CColorComboBox m_cbPriorityColors;
	BOOL	m_bSpecifyDueColor;
	BOOL	m_bSpecifyDueTodayColor;
	BOOL	m_bSpecifyStartColor;
	BOOL	m_bSpecifyStartTodayColor;
	BOOL	m_bSpecifyGridColor;
	BOOL	m_bSpecifyDoneColor;
	BOOL	m_bSpecifyFlaggedColor; 
	BOOL	m_bSpecifyReferenceColor;
	CColorButton	m_btFilteredColor;
	CColorButton	m_btAttribColor;
	CColorButton	m_btDoneColor;
	CColorButton	m_btGridlineColor;
	CColorButton	m_btDueColor;
	CColorButton	m_btDueTodayColor;
	CColorButton	m_btStartColor;
	CColorButton	m_btStartTodayColor;
	CColorButton	m_btFlaggedColor;
	CColorButton	m_btReferenceColor;
	CComboBox	m_cbTreeFontSize, m_cbCommentsFontSize;
	CFontComboBox	m_cbTreeFonts, m_cbCommentsFonts;
	BOOL	m_bSpecifyTreeFont;
	BOOL	m_bSpecifyCommentsFont;
	CColorButton	m_btSetColor;
	CColorButton	m_btLowColor;
	CColorButton	m_btHighColor;
	BOOL	m_bColorPriority;
	int		m_bGradientPriorityColors;
	int		m_nSelPriorityColor;
	BOOL	m_bShowTimeColumn;
	CDWordArray m_aPriorityColors;
	CAttribColorArray m_aAttribColors;
	COLORREF m_crLow, m_crHigh;
	CString m_sTreeFont;
	int		m_nTreeFontSize;
	CString m_sCommentsFont;
	int		m_nCommentsFontSize;
	COLORREF m_crGridlines, m_crDone;
	CColorButton	m_btAltLineColor;
	COLORREF m_crAltLine;
	COLORREF m_crDue, m_crDueToday;
	COLORREF m_crStart, m_crStartToday;
	CGroupLineManager m_mgrGroupLines;
	COLORREF m_crFlagged;
	COLORREF m_crReference;
	TDC_ATTRIBUTE m_nColorAttribute;

// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CPreferencesUITasklistColorsPage)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CPreferencesUITasklistColorsPage)
	afx_msg void OnAltlinecolor();
	afx_msg void OnSpecifyAlternatelinecolor();
	afx_msg void OnChangeTextColorOption();
	afx_msg void OnCommentsusetreefont();
	afx_msg void OnSpecifycommentsfont();
	afx_msg void OnPopulateattriblist();
	afx_msg void OnSelchangeAttributetocolorby();
	//}}AFX_MSG
	afx_msg void OnSetAttribcolor();
	afx_msg void OnEditAttribcolors();
	afx_msg void OnSelchangeAttribcolors();
	virtual BOOL OnInitDialog();
	afx_msg void OnStarttaskcolor();
	afx_msg void OnSetStarttaskcolor();
	afx_msg void OnStarttodaytaskcolor();
	afx_msg void OnSetStarttodaytaskcolor();
	afx_msg void OnDuetaskcolor();
	afx_msg void OnSetduetaskcolor();
	afx_msg void OnDuetodaytaskcolor();
	afx_msg void OnSetduetodaytaskcolor();
	afx_msg void OnSpecifytreefont();
	afx_msg void OnSetgridlinecolor();
	afx_msg void OnSpecifygridlinecolor();
	afx_msg void OnSetdonecolor();
	afx_msg void OnSpecifydonecolor();
	afx_msg void OnLowprioritycolor();
	afx_msg void OnHighprioritycolor();
	afx_msg void OnSetprioritycolor();
	afx_msg void OnChangePriorityColorOption();
	afx_msg void OnColorPriority();
	afx_msg void OnSelchangePrioritycolors();
	afx_msg LRESULT OnAttribAdded(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnAttribDeleted(WPARAM wParam, LPARAM lParam);
	afx_msg void OnSpecifyflaggedcolor();
	afx_msg void OnSetflaggedcolor();
	afx_msg void OnSpecifyReferencecolor();
	afx_msg void OnSetReferencecolor();

	DECLARE_MESSAGE_MAP()

protected:
	int FindAttribColor(LPCTSTR szAttrib);
	void AddDefaultListItemsToAttributeColors(const CPreferences& prefs);
	
	virtual void LoadPreferences(const CPreferences& prefs);
	virtual void SavePreferences(CPreferences& prefs);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PREFERENCESUITASKLISTPAGE_H__9612D6FB_2A00_46DA_99A4_1AC6270F060D__INCLUDED_)
