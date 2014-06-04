// UIThemeFile.h: interface for the CUIThemeFile class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_UITHEMEFILE_H__A652A09A_AB3F_4B65_A3D9_B4B975A36195__INCLUDED_)
#define AFX_UITHEMEFILE_H__A652A09A_AB3F_4B65_A3D9_B4B975A36195__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\SHARED\uitheme.h"

class CXmlItem;

class CUIThemeFile : public UITHEME  
{
public:
	CUIThemeFile();
	virtual ~CUIThemeFile();

	BOOL LoadThemeFile(LPCTSTR szThemeFile);
	void Reset();

	CString GetToolbarImageFile(LPCTSTR szKey) const;

	BOOL HasGlass() const { return (nRenderStyle != UIRS_GRADIENT); }
	BOOL HasGradient() const { return (nRenderStyle != UIRS_GLASS); }

	static void Reset(UITHEME& theme);

protected:
	CMapStringToString m_mapToolbarImageFiles;

protected:
	static COLORREF GetColor(const CXmlItem* pXITheme, LPCTSTR szName, int nColorID = -1);
	static UI_RENDER_STYLE GetRenderStyle(const CXmlItem* pXITheme);
	static UI_IMAGE_STYLE GetImageStyle(const CXmlItem* pXITheme);

};

#endif // !defined(AFX_UITHEMEFILE_H__A652A09A_AB3F_4B65_A3D9_B4B975A36195__INCLUDED_)
