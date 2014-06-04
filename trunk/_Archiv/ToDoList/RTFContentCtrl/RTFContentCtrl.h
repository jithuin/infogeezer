// RTFContentCtrl.h : main header file for the RTFCONTENTCTRL DLL
//

#if !defined(AFX_RTFCONTENTCTRL_H__2C6426B5_72CA_4C0F_9CE1_A03CD5C833C7__INCLUDED_)
#define AFX_RTFCONTENTCTRL_H__2C6426B5_72CA_4C0F_9CE1_A03CD5C833C7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

#include "..\shared\icontentcontrol.h"

/////////////////////////////////////////////////////////////////////////////
// CRTFContentCtrlApp
// See RTFContentCtrl.cpp for the implementation of this class
//

// {849CF988-79FE-418A-A40D-01FE3AFCAB2C}
static const GUID RTF_TYPEID = 
{ 0x849cf988, 0x79fe, 0x418a, { 0xa4, 0xd, 0x1, 0xfe, 0x3a, 0xfc, 0xab, 0x2c } };

class CMSWordHelper;

class CRTFContentCtrlApp : public IContent, public CWinApp
{
public:
	CRTFContentCtrlApp();

	// IContent implementation
	LPCTSTR GetTypeID() const;
	LPCTSTR GetTypeDescription() const;

	void SetLocalizer(ITransText* pTT);

	IContentControl* CreateCtrl(unsigned short nCtrlID, unsigned long nStyle, 
						long nLeft, long nTop, long nWidth, long nHeight, HWND hwndParent);
	void Release();

	int ConvertToHtml(const unsigned char* pContent, int nLength,
					  LPCTSTR szCharSet, LPTSTR& szHtml, LPCTSTR szImageDir);

protected:
	CMSWordHelper* m_pWordHelper;
	CStringArray m_aTempImageFolders;

	void CleanupTemporaryImages();
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_RTFCONTENTCTRL_H__2C6426B5_72CA_4C0F_9CE1_A03CD5C833C7__INCLUDED_)
