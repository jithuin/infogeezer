// UIThemeFile.cpp: implementation of the CUIThemeFile class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "UIThemeFile.h"
#include "xmlfile.h"
#include "filemisc.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CUIThemeFile::CUIThemeFile()
{
	Reset();
}

CUIThemeFile::~CUIThemeFile()
{

}

BOOL CUIThemeFile::LoadThemeFile(LPCTSTR szThemeFile)
{
	CXmlFile xiFile;

	if (!xiFile.Load(szThemeFile, _T("TODOLIST")))
		return FALSE;

	const CXmlItem* pXITheme = xiFile.GetItem(_T("UITHEME"));

	if (!pXITheme)
		return FALSE;

	// else
	nRenderStyle = GetRenderStyle(pXITheme);
	nImageStyle = GetImageStyle(pXITheme);

	crAppBackDark = GetColor(pXITheme, _T("APPBACKDARK"));
	crAppBackLight = GetColor(pXITheme, _T("APPBACKLIGHT"));
	crAppLinesDark = GetColor(pXITheme, _T("APPLINESDARK"), COLOR_3DSHADOW);
	crAppLinesLight = GetColor(pXITheme, _T("APPLINESLIGHT"), COLOR_3DHIGHLIGHT);
	crAppText = GetColor(pXITheme, _T("APPTEXT"), COLOR_WINDOWTEXT);
	crMenuBack = GetColor(pXITheme, _T("MENUBACK"), COLOR_3DFACE);
	crToolbarDark = GetColor(pXITheme, _T("TOOLBARDARK"));
	crToolbarLight = GetColor(pXITheme, _T("TOOLBARLIGHT"));
	crStatusBarDark = GetColor(pXITheme, _T("STATUSBARDARK"));
	crStatusBarLight = GetColor(pXITheme, _T("STATUSBARLIGHT"));
	crStatusBarText = GetColor(pXITheme, _T("STATUSBARTEXT"), COLOR_WINDOWTEXT);

	// toolbars
	CString sFolder = FileMisc::GetFolderFromFilePath(szThemeFile);
	const CXmlItem* pTBImage = pXITheme->GetItem(_T("TOOLBARIMAGES"));

	while (pTBImage)
	{
		CString sKey = pTBImage->GetItemValue(_T("KEY"));
		CString sFile = pTBImage->GetItemValue(_T("FILE"));

		if (!sKey.IsEmpty() && !sFile.IsEmpty())
			m_mapToolbarImageFiles[sKey] = FileMisc::MakeFullPath(sFile, sFolder);

		pTBImage = pTBImage->GetSibling();
	}

	return TRUE;
}

void CUIThemeFile::Reset()
{
	Reset(*this);
}

void CUIThemeFile::Reset(UITHEME& theme)
{
	theme.nRenderStyle = UIRS_GRADIENT;
	theme.nImageStyle = UIIS_STANDARD;

	theme.crAppBackDark = GetSysColor(COLOR_3DFACE);
	theme.crAppBackLight = GetSysColor(COLOR_3DFACE);
	theme.crAppLinesDark = GetSysColor(COLOR_3DSHADOW);
	theme.crAppLinesLight = GetSysColor(COLOR_3DHIGHLIGHT);
	theme.crAppText = GetSysColor(COLOR_WINDOWTEXT);
	theme.crMenuBack = GetSysColor(COLOR_3DFACE);
	theme.crToolbarDark = GetSysColor(COLOR_3DFACE);
	theme.crToolbarLight = GetSysColor(COLOR_3DFACE);
	theme.crStatusBarDark = GetSysColor(COLOR_3DFACE);
	theme.crStatusBarLight = GetSysColor(COLOR_3DFACE);
	theme.crStatusBarText = GetSysColor(COLOR_WINDOWTEXT);
}

COLORREF CUIThemeFile::GetColor(const CXmlItem* pXITheme, LPCTSTR szName, int nColorID)
{
	const CXmlItem* pXIColor = pXITheme->GetItem(_T("COLOR"));

	while (pXIColor)
	{
		if (pXIColor->GetItemValue(_T("NAME")).CompareNoCase(szName) == 0)
		{
			BYTE bRed = (BYTE)pXIColor->GetItemValueI(_T("R"));
			BYTE bGreen = (BYTE)pXIColor->GetItemValueI(_T("G"));
			BYTE bBlue = (BYTE)pXIColor->GetItemValueI(_T("B"));

			return RGB(bRed, bGreen, bBlue);
		}

		pXIColor = pXIColor->GetSibling();
	}

	// not found
	if (nColorID != -1)
		return GetSysColor(nColorID);

	// else
	return UIT_NOCOLOR;
}

UI_RENDER_STYLE CUIThemeFile::GetRenderStyle(const CXmlItem* pXITheme)
{
	CString sStyle = pXITheme->GetItemValue(_T("STYLE"));

	if (sStyle == _T("GLASS"))
		return UIRS_GLASS;

	else if (sStyle == _T("GLASSWITHGRADIENT"))
		return UIRS_GLASSWITHGRADIENT;

	// else
	return UIRS_GRADIENT;
}

UI_IMAGE_STYLE CUIThemeFile::GetImageStyle(const CXmlItem* pXITheme)
{
	CString sStyle = pXITheme->GetItemValue(_T("IMAGE_STYLE"));

	if (sStyle == _T("METRO"))
		return UIIS_METRO;

	else if (sStyle == _T("GRAYSCALE"))
		return UIIS_GRAYSCALE;

	// else
	return UIIS_STANDARD;
}

CString CUIThemeFile::GetToolbarImageFile(LPCTSTR szKey) const
{
	CString sImageFile;
	m_mapToolbarImageFiles.Lookup(szKey, sImageFile);

	return sImageFile;
}
