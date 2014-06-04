// GraphicsMisc.h: interface for the GraphicsMisc class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_GRAPHICSMISC_H__A3408501_A44D_407B_A8C3_B6AB31370CD2__INCLUDED_)
#define AFX_GRAPHICSMISC_H__A3408501_A44D_407B_A8C3_B6AB31370CD2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

enum 
{ 
	GMFS_BOLD		= 0x01, 
	GMFS_ITALIC		= 0x02, 
	GMFS_UNDERLINED	= 0x04, 
	GMFS_STRIKETHRU	= 0x08, 
	GMFS_SYMBOL		= 0x10,

	GMFS_ALL		= 0xff
};

enum 
{
	GMDR_LEFT		= 0x1, 		
	GMDR_TOP		= 0x2, 
	GMDR_RIGHT		= 0x4, 
	GMDR_BOTTOM		= 0x8, 

	GMDR_ALL		= 0xf
};

const COLORREF NOCOLOR = (COLORREF)-1;

namespace GraphicsMisc  
{
	void DrawGradient(CDC* pDC, LPCRECT pRect, COLORREF crFrom, COLORREF crTo, BOOL bHorz, int nBorder = -1);
	void DrawGlass(CDC* pDC, LPCRECT pRect, COLORREF crFrom, COLORREF crTo, BOOL bHorz, int nBorder = -1);
	void DrawGlassWithGradient(CDC* pDC, LPCRECT pRect, COLORREF crFrom, COLORREF crTo, BOOL bHorz, int nBorder = -1);
	
	HFONT CreateFont(HFONT hFont, DWORD dwFlags = 0, DWORD dwMask = GMFS_ALL);
	HFONT CreateFont(LPCTSTR szFaceName, int nPoint = -1, DWORD dwFlags = 0);
	BOOL CreateFont(CFont& font, LPCTSTR szFaceName, int nPoint = -1, DWORD dwFlags = 0);
	BOOL CreateFont(CFont& fontOut, HFONT fontIn, DWORD dwFlags = 0, DWORD dwMask = GMFS_ALL);
	
	HCURSOR HandCursor();
	HICON LoadIcon(UINT nIDIcon, int nSize = 16);
	
	DWORD GetFontFlags(HFONT hFont);
	int GetFontNameSize(HFONT hFont, CString& sFaceName);
	BOOL SameFont(HFONT hFont, LPCTSTR szFaceName, int nPoint);
	BOOL SameFontNameSize(HFONT hFont1, HFONT hFont2);
	CFont& WingDings();
	CFont& Marlett();
	int DrawSymbol(CDC* pDC, char cSymbol, const CRect& rText, UINT nFlags, CFont* pFont = NULL);
	CFont* PrepareDCFont(CDC* pDC, CWnd* pWndRef = NULL, CFont* pFont = NULL, int nStockFont = DEFAULT_GUI_FONT); // returns 'old' font
	
	int GetTextWidth(UINT nIDString, CWnd& wndRef, CFont* pRefFont = NULL);
	int GetTextWidth(const CString& sText, CWnd& wndRef, CFont* pRefFont = NULL);
	int AFX_CDECL GetTextWidth(CDC* pDC, LPCTSTR lpszFormat, ...);
	float GetAverageCharWidth(CDC* pDC);
	int GetAverageStringWidth(const CString& sText, CDC* pDC);
	int GetAverageMaxStringWidth(const CString& sText, CDC* pDC);

	inline BOOL VerifyDeleteObject(HGDIOBJ hObj)
	{
		if (hObj == NULL)
			return TRUE;

		// else
		if (::DeleteObject(hObj))
			return TRUE;

		// else
		ASSERT(0);
		return FALSE;
	}

	inline BOOL VerifyDeleteObject(CGdiObject& obj)
	{
		if (obj.m_hObject == NULL)
			return TRUE;

		// else
		if (obj.DeleteObject())
			return TRUE;

		// else
		ASSERT(0);
		return FALSE;

	}
	BOOL GrayScale(CBitmap& bitmap, COLORREF crMask = GetSysColor(COLOR_3DFACE));
	
	COLORREF Lighter(COLORREF color, double dAmount);
	COLORREF Darker(COLORREF color, double dAmount);
	COLORREF Blend(COLORREF color1, COLORREF color2, double dAmount);
	COLORREF GetBestTextColor(COLORREF crBack);

	void DrawRect(CDC* pDC, const CRect& rect, COLORREF crFill, COLORREF crBorder = NOCOLOR, 
					int nCornerRadius = 0, DWORD dwEdges = GMDR_ALL);
	
	BOOL ForceIconicRepresentation(HWND hWnd, BOOL bForce = TRUE);
	BOOL EnableAeroPeek(HWND hWnd, BOOL bEnable = TRUE);
	BOOL EnableFlip3D(HWND hWnd, BOOL bEnable = TRUE);
	BOOL DwmSetWindowAttribute(HWND hWnd, DWORD dwAttrib, LPCVOID pData, DWORD dwDataSize);

	BOOL GetAvailableScreenSpace(const CRect& rWnd, CRect& rScreen);
	BOOL GetAvailableScreenSpace(HWND hWnd, CRect& rScreen);
	BOOL GetAvailableScreenSpace(CRect& rScreen);

};

#endif // !defined(AFX_GRAPHICSMISC_H__A3408501_A44D_407B_A8C3_B6AB31370CD2__INCLUDED_)
