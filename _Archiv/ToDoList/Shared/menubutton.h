#if !defined(AFX_MENUBUTTON_H__58ABCD1E_5585_4F85_8A2B_51DB556FD9DE__INCLUDED_)
#define AFX_MENUBUTTON_H__58ABCD1E_5585_4F85_8A2B_51DB556FD9DE__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// menubutton.h : header file
//

#include "custombutton.h"

/////////////////////////////////////////////////////////////////////////////
// CMenuButton window

enum MENUBTN_STYLE
{
	MBS_RIGHT,
	MBS_DOWN,
};

class CMenuButton : public CCustomButton
{
// Construction
public:
	CMenuButton(UINT nMenuID = 0, int nSubMenu = 0, MENUBTN_STYLE nStyle = MBS_RIGHT);

	void SetWindowText(LPCTSTR lpszString);
	void SetWindowText(TCHAR nChar);

	UINT TrackPopupMenu(CMenu* pMenu, int nSubMenu = -1);

protected:
	MENUBTN_STYLE m_nStyle;
	UINT m_nMenuID;
	int m_nSubMenu;

protected:
	virtual BOOL DoAction();
	virtual void DoExtraPaint(CDC* pDC, const CRect& rExtra);
	void PrepareState(CMenu* pMenu);

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMenuButton)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CMenuButton();

	// Generated message map functions
protected:
	//{{AFX_MSG(CMenuButton)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MENUBUTTON_H__58ABCD1E_5585_4F85_8A2B_51DB556FD9DE__INCLUDED_)
