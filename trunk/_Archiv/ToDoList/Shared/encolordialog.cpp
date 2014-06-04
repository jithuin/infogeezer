// encolordialog.cpp : implementation file
//

#include "stdafx.h"
#include "encolordialog.h"
#include "encolordialog.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CEnColorDialog

IMPLEMENT_DYNAMIC(CEnColorDialog, CColorDialog)

CEnColorDialog::CEnColorDialog(COLORREF clrInit, DWORD dwFlags, CWnd* pParentWnd) :
	CColorDialog(clrInit, dwFlags, pParentWnd)
{
	// restore previously saved custom colors
	for (int nColor = 0; nColor < 16; nColor++)
	{
		CString sKey;
		sKey.Format(_T("CustomColor%d"), nColor);

		COLORREF color = (COLORREF)AfxGetApp()->GetProfileInt(_T("ColorDialog"), sKey, (int)RGB(255, 255, 255));
		m_cc.lpCustColors[nColor] = color;
	}
}

CEnColorDialog::~CEnColorDialog()
{
	// save any custom colors
	COLORREF* pColors = GetSavedCustomColors();
	
	for (int nColor = 0; nColor < 16; nColor++)
	{
		CString sKey;
		sKey.Format(_T("CustomColor%d"), nColor);
		
		int nColorVal = (int)pColors[nColor];
		AfxGetApp()->WriteProfileInt(_T("ColorDialog"), sKey, nColorVal);
	}
}

BEGIN_MESSAGE_MAP(CEnColorDialog, CColorDialog)
	//{{AFX_MSG_MAP(CEnColorDialog)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


