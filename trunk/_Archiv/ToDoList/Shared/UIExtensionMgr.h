// ImportExportMgr.h: interface for the CUIExtensionMgr class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_UIEXTENSIONMGR_H__C258D849_69ED_46A7_A2F0_351C5C9FB3B3__INCLUDED_)
#define AFX_UIEXTENSIONMGR_H__C258D849_69ED_46A7_A2F0_351C5C9FB3B3__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "IUIExtension.h"

#include <afxtempl.h>

class IUIExtension;
class CUIExtensionWnd;

class CUIExtensionMgr  
{
public:
	CUIExtensionMgr();
	virtual ~CUIExtensionMgr();
	
	virtual void Release();
	void UpdateLocalizer();

	BOOL SomePluginsHaveBadversions() const { return m_bSomeBadVersions; }

	int GetNumUIExtensions() const;
	int FindUIExtension(LPCTSTR szTypeID) const;

	CString GetUIExtensionMenuText(int nExtension) const;
	HICON GetUIExtensionIcon(int nExtension) const;
	CString GetUIExtensionTypeID(int nExtension) const;
	IUIExtension* GetUIExtension(int nExtension) const;

	BOOL CreateExtensionWindow(int nExtension, CUIExtensionWnd& wnd, 
								unsigned short nCtrlID, unsigned long nStyle, 
								long nLeft, long nTop, long nWidth, long nHeight, 
								HWND hwndParent);
protected:
	BOOL m_bInitialized;
	BOOL m_bSomeBadVersions;
	CArray<IUIExtension*, IUIExtension*> m_aUIExtensions;
	
protected:
	virtual void Initialize() const;
	static CString& Clean(CString& sText);

};

#endif // !defined(AFX_UIEXTENSIONMGR_H__C258D849_69ED_46A7_A2F0_351C5C9FB3B3__INCLUDED_)
