// GraphicsMisc.cpp: implementation of the GraphicsMisc class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "GraphicsMisc.h"
#include "enstring.h"
#include "enbitmapex.h"
#include "imageprocessors.h"

#include <windef.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

const COLORREF NO_COLOR = (COLORREF)-1;

//////////////////////////////////////////////////////////////////////

void GraphicsMisc::DrawGradient(CDC* pDC, LPCRECT pRect, COLORREF crFrom, COLORREF crTo, BOOL bHorz, int nBorder)
{
	TRIVERTEX vert[2];
	GRADIENT_RECT gRect;

	vert[0] .x      = pRect->left;
	vert[0] .y      = pRect->top;
	vert[0] .Red    = MAKEWORD(0, GetRValue(crFrom));
	vert[0] .Green  = MAKEWORD(0, GetGValue(crFrom));
	vert[0] .Blue   = MAKEWORD(0, GetBValue(crFrom));
	vert[0] .Alpha  = 0x0000;
	vert[1] .x      = pRect->right;
	vert[1] .y      = pRect->bottom; 
	vert[1] .Red    = MAKEWORD(0, GetRValue(crTo));
	vert[1] .Green  = MAKEWORD(0, GetGValue(crTo));
	vert[1] .Blue   = MAKEWORD(0, GetBValue(crTo));
	vert[1] .Alpha  = 0x0000;
	gRect.UpperLeft  = 0;
	gRect.LowerRight = 1;

	GradientFill(pDC->GetSafeHdc(), vert, 2, &gRect, 1, bHorz ? GRADIENT_FILL_RECT_H : GRADIENT_FILL_RECT_V);

	if (nBorder >= 0)
	{
		// draw a border in from the edge
		CRect rBorder(pRect);
		rBorder.DeflateRect(nBorder, nBorder);
		DrawRect(pDC, rBorder, NOCOLOR, crFrom);
	}
}

void GraphicsMisc::DrawGlass(CDC* pDC, LPCRECT pRect, COLORREF crFrom, COLORREF crTo, BOOL bHorz, int nBorder)
{
	CRect rBarFrom(pRect), rBarTo(pRect);

	if (bHorz)
	{
		rBarFrom.right = rBarFrom.left + (rBarFrom.Width() * 2 / 5);
		rBarTo.left = rBarFrom.right;
	}
	else // vert
	{
		rBarFrom.bottom = rBarFrom.top + (rBarFrom.Height() * 2 / 5);
		rBarTo.top = rBarFrom.bottom;
	}

	DrawGradient(pDC, rBarFrom, Lighter(crFrom, 0.2), crFrom, bHorz);
	DrawGradient(pDC, rBarTo, crTo, Lighter(crTo, 0.2), bHorz);
//	pDC->FillSolidRect(rBarFrom, crFrom);
//	pDC->FillSolidRect(rBarTo, crTo);

	if (nBorder >= 0)
	{
		// draw a border in from the edge
		CRect rBorder(pRect);
		rBorder.DeflateRect(nBorder, nBorder);
		DrawRect(pDC, rBorder, NOCOLOR, crFrom);
	}
}

void GraphicsMisc::DrawGlassWithGradient(CDC* pDC, LPCRECT pRect, COLORREF crFrom, COLORREF crTo, BOOL bHorz, int nBorder)
{
	// draw the glass first
	CRect rBarFrom(pRect), rBarTo(pRect);

	if (bHorz)
	{
		rBarFrom.right = rBarFrom.left + (rBarFrom.Width() * 2 / 10);
		rBarTo.left = rBarTo.right - (rBarTo.Width() * 4 / 10);
	}
	else // vert
	{
		rBarFrom.bottom = rBarFrom.top + (rBarFrom.Height() * 2 / 10);
		rBarTo.top = rBarTo.bottom - (rBarTo.Height() * 4 / 10);
	}

	pDC->FillSolidRect(rBarFrom, crFrom);
	pDC->FillSolidRect(rBarTo, crTo);

	// then the gradient
	CRect rGrad(pRect);

	if (bHorz)
	{
		rGrad.left = rBarFrom.right;
		rGrad.right = rBarTo.left;
	}
	else
	{
		rGrad.top = rBarFrom.bottom;
		rGrad.bottom = rBarTo.top;
	}

	DrawGradient(pDC, rGrad, crFrom, crTo, bHorz); // no border

	if (nBorder >= 0)
	{
		// draw a border in from the edge
		CRect rBorder(pRect);
		rBorder.DeflateRect(nBorder, nBorder);
		DrawRect(pDC, rBorder, NOCOLOR, crFrom);
	}
}

HFONT GraphicsMisc::CreateFont(HFONT hFont, DWORD dwFlags, DWORD dwMask)
{
	if (hFont == NULL)
		hFont = (HFONT)GetStockObject(DEFAULT_GUI_FONT);

	LOGFONT lf;
	::GetObject(hFont, sizeof(lf), &lf);
	
	if (dwMask & GMFS_UNDERLINED)
		lf.lfUnderline = (BYTE)(dwFlags & GMFS_UNDERLINED);
	
	if (dwMask & GMFS_ITALIC)
		lf.lfItalic = (BYTE)(dwFlags & GMFS_ITALIC);
	
	if (dwMask & GMFS_STRIKETHRU)
		lf.lfStrikeOut = (BYTE)(dwFlags & GMFS_STRIKETHRU);
	
	if (dwMask & GMFS_BOLD)
		lf.lfWeight = (dwFlags & GMFS_BOLD) ? FW_BOLD : FW_NORMAL;
	
	HFONT hFontOut = CreateFontIndirect(&lf);
	
	// verify the font creation
	if (!SameFontNameSize(hFont, hFontOut))
	{
		VerifyDeleteObject(hFontOut);
		hFontOut = NULL;
	}
	
	return hFontOut;
}

BOOL GraphicsMisc::CreateFont(CFont& fontOut, HFONT fontIn, DWORD dwFlags, DWORD dwMask)
{
	VerifyDeleteObject(fontOut);

	return fontOut.Attach(CreateFont(fontIn, dwFlags, dwMask));
}

HFONT GraphicsMisc::CreateFont(LPCTSTR szFaceName, int nPoint, DWORD dwFlags)
{
	HFONT hDefFont = (HFONT)GetStockObject(DEFAULT_GUI_FONT);
	ASSERT (hDefFont);
	
	LOGFONT lf;
	::GetObject(hDefFont, sizeof(lf), &lf);
	
	// set the charset
	if (dwFlags & GMFS_SYMBOL)
		lf.lfCharSet = SYMBOL_CHARSET;

	else if (!lf.lfCharSet)
		lf.lfCharSet = DEFAULT_CHARSET;
	
	if (szFaceName && *szFaceName)
		lstrcpy(lf.lfFaceName, szFaceName);
	
	if (nPoint > 0)
	{
		HDC hDC = ::GetDC(NULL);
		lf.lfHeight = -MulDiv(abs(nPoint), GetDeviceCaps(hDC, LOGPIXELSY), 72);
		::ReleaseDC(NULL, hDC);
	}
	else if (dwFlags & GMFS_SYMBOL)
		lf.lfHeight = MulDiv(lf.lfHeight, 12, 10);
	
	lf.lfWidth = 0;
	lf.lfUnderline = (BYTE)(dwFlags & GMFS_UNDERLINED);
	lf.lfItalic = (BYTE)(dwFlags & GMFS_ITALIC);
	lf.lfStrikeOut = (BYTE)(dwFlags & GMFS_STRIKETHRU);
	lf.lfWeight = (dwFlags & GMFS_BOLD) ? FW_BOLD : FW_NORMAL;
	
	HFONT hFont = CreateFontIndirect(&lf);

	// verify the font creation
	if (!SameFont(hFont, szFaceName, nPoint))
	{
		VerifyDeleteObject(hFont);
		hFont = NULL;
	}
	
	return hFont;
}

BOOL GraphicsMisc::CreateFont(CFont& font, LPCTSTR szFaceName, int nPoint, DWORD dwFlags)
{
	VerifyDeleteObject(font);

	return font.Attach(CreateFont(szFaceName, nPoint, dwFlags));
}

DWORD GraphicsMisc::GetFontFlags(HFONT hFont)
{
	if (!hFont)
		return 0;

	LOGFONT lf;
	::GetObject(hFont, sizeof(lf), &lf);

	DWORD dwFlags = 0;
	
	dwFlags |= (lf.lfItalic ? GMFS_ITALIC : 0);
	dwFlags |= (lf.lfUnderline ? GMFS_UNDERLINED : 0);
	dwFlags |= (lf.lfStrikeOut ? GMFS_STRIKETHRU : 0);
	dwFlags |= (lf.lfWeight >= FW_BOLD ? GMFS_BOLD : 0);
	
	return dwFlags;
}

int GraphicsMisc::GetFontNameSize(HFONT hFont, CString& sFaceName)
{
	if (!hFont)
	{
		sFaceName.Empty();
		return 0;
	}
	
	LOGFONT lf;
	::GetObject(hFont, sizeof(lf), &lf);
	
	sFaceName = lf.lfFaceName;
	
	HDC hDC = ::GetDC(NULL);
	int nPoint = MulDiv(abs(lf.lfHeight), 72, GetDeviceCaps(hDC, LOGPIXELSY));
	::ReleaseDC(NULL, hDC);
	
	return nPoint;
}

BOOL GraphicsMisc::SameFont(HFONT hFont, LPCTSTR szFaceName, int nPoint)
{
	CString sFontName;
	int nFontSize = GetFontNameSize(hFont, sFontName);

	return ((nPoint <= 0 || nPoint == nFontSize) && 
			(!szFaceName || sFontName.CompareNoCase(szFaceName) == 0));
}

BOOL GraphicsMisc::SameFontNameSize(HFONT hFont1, HFONT hFont2)
{
	CString sName1;
	int nSize1 = GetFontNameSize(hFont1, sName1);

	return SameFont(hFont2, sName1, nSize1);
}

HICON GraphicsMisc::LoadIcon(UINT nIDIcon, int nSize)
{
	HICON hIcon = (HICON)::LoadImage(AfxGetResourceHandle(), MAKEINTRESOURCE(nIDIcon), 
									IMAGE_ICON, nSize, nSize, LR_LOADMAP3DCOLORS);

	return hIcon;
}

HCURSOR GraphicsMisc::HandCursor()
{
#ifndef IDC_HAND
#	define IDC_HAND  MAKEINTRESOURCE(32649) // from winuser.h
#endif
	static HCURSOR cursor = NULL;
	
	if (!cursor)
	{
		cursor = ::LoadCursor(NULL, IDC_HAND);
		
		// fallback hack for win9x
		if (!cursor)
		{
			CString sWinHlp32;
			
			GetWindowsDirectory(sWinHlp32.GetBuffer(MAX_PATH+1), MAX_PATH);
			sWinHlp32.ReleaseBuffer();
			sWinHlp32 += _T("\\winhlp32.exe");
			
			HMODULE hMod = LoadLibrary(sWinHlp32);
			
			if (hMod)
				cursor = ::LoadCursor(hMod, MAKEINTRESOURCE(106));
		}
	}

	return cursor;
}

CFont& GraphicsMisc::WingDings()
{
	static CFont font;
				
	if (!font.GetSafeHandle())
		font.Attach(CreateFont(_T("Wingdings"), -1, GMFS_SYMBOL));

	return font;
}

CFont& GraphicsMisc::Marlett()
{
	static CFont font;
				
	if (!font.GetSafeHandle())
		font.Attach(CreateFont(_T("Marlett"), -1, GMFS_SYMBOL));

	return font;
}

int GraphicsMisc::GetTextWidth(UINT nIDString, CWnd& wndRef, CFont* pRefFont)
{
	CEnString sText(nIDString);
	return GetTextWidth(sText, wndRef, pRefFont);
}

int GraphicsMisc::GetTextWidth(const CString& sText, CWnd& wndRef, CFont* pRefFont)
{
	CDC* pDC = wndRef.GetDC();
	ASSERT(pDC);

	if (!pDC)
		return -1;
	
	if (pRefFont == NULL)
		pRefFont = wndRef.GetFont();

	CFont* pOldFont = pDC->SelectObject(pRefFont);
	int nLength = pDC->GetTextExtent(sText).cx;

	pDC->SelectObject(pOldFont);
	wndRef.ReleaseDC(pDC);

	return nLength;
}

int AFX_CDECL GraphicsMisc::GetTextWidth(CDC* pDC, LPCTSTR lpszFormat, ...)
{
	static TCHAR BUFFER[2048];

	ASSERT(AfxIsValidString(lpszFormat));

	va_list argList;
	va_start(argList, lpszFormat);
//fabio_2005
#if _MSC_VER >= 1400
	_vstprintf_s(BUFFER, lpszFormat, argList);
#else
	_vstprintf(BUFFER, lpszFormat, argList);
#endif
	va_end(argList);

	return pDC->GetTextExtent(BUFFER).cx;
}

float GraphicsMisc::GetAverageCharWidth(CDC* pDC)
{
	ASSERT(pDC);
	
	TEXTMETRIC tm = { 0 };
	pDC->GetTextMetrics(&tm);
	
	return (float)tm.tmAveCharWidth;
}

int GraphicsMisc::GetAverageStringWidth(const CString& sText, CDC* pDC)
{
	if (sText.IsEmpty())
		return 0;

	return (int)(GetAverageCharWidth(pDC) * sText.GetLength());
}

int GraphicsMisc::GetAverageMaxStringWidth(const CString& sText, CDC* pDC)
{
	if (sText.IsEmpty())
		return 0;

	int nAveWidth = GetAverageStringWidth(sText, pDC);
	int nActualWidth = pDC->GetTextExtent(sText).cx;

	return max(nAveWidth, nActualWidth);
}

CFont* GraphicsMisc::PrepareDCFont(CDC* pDC, CWnd* pWndRef, CFont* pFont, int nStockFont)
{
	if (pFont == NULL && pWndRef != NULL)
		pFont = CFont::FromHandle((HFONT)pWndRef->SendMessage(WM_GETFONT, 0, 0));

	if (pFont)
		return pDC->SelectObject(pFont);
	
	// else
	return (CFont*)pDC->SelectStockObject(nStockFont);
}

COLORREF GraphicsMisc::GetBestTextColor(COLORREF crBack)
{
	// base text color on luminance
	return (RGBX(crBack).Luminance() < 128) ? RGB(255, 255, 255) : 0;
}

COLORREF GraphicsMisc::Lighter(COLORREF color, double dAmount)
{
	if (color == NO_COLOR)
		return NO_COLOR;

	int red = GetRValue(color);
	int green = GetGValue(color);
	int blue = GetBValue(color);
	
	red += (int)((255 - red) * dAmount);
	green += (int)((255 - green) * dAmount);
	blue += (int)((255 - blue) * dAmount);

	red = min(255, red);
	green = min(255, green);
	blue = min(255, blue);
	
	return RGB(red, green, blue);
}

COLORREF GraphicsMisc::Darker(COLORREF color, double dAmount)
{
	if (color == NO_COLOR)
		return NO_COLOR;

	int red = GetRValue(color);
	int green = GetGValue(color);
	int blue = GetBValue(color);
	
	red -= (int)(red * dAmount);
	green -= (int)(green * dAmount);
	blue -= (int)(blue * dAmount);

	red = max(0, red);
	green = max(0, green);
	blue = max(0, blue);
	
	return RGB(red, green, blue);
}

COLORREF GraphicsMisc::Blend(COLORREF color1, COLORREF color2, double dAmount)
{
	if (color1 == NO_COLOR || color2 == NO_COLOR)
		return NO_COLOR;

	int red1 = GetRValue(color1);
	int green1 = GetGValue(color1);
	int blue1 = GetBValue(color1);
	
	int red2 = GetRValue(color2);
	int green2 = GetGValue(color2);
	int blue2 = GetBValue(color2);
	
	int redBlend = (int)((red1 + red2) * dAmount);
	int greenBlend = (int)((green1 + green2) * dAmount);
	int blueBlend = (int)((blue1 + blue2) * dAmount);

	return RGB(redBlend, greenBlend, blueBlend);
}

BOOL GraphicsMisc::ForceIconicRepresentation(HWND hWnd, BOOL bForce)
{
#ifndef DWMWA_FORCE_ICONIC_REPRESENTATION
# define DWMWA_FORCE_ICONIC_REPRESENTATION 7
#endif
	
	return DwmSetWindowAttribute(hWnd, DWMWA_FORCE_ICONIC_REPRESENTATION, &bForce, sizeof(bForce));
}

BOOL GraphicsMisc::EnableAeroPeek(HWND hWnd, BOOL bEnable)
{
#ifndef DWMWA_DISALLOW_PEEK
# define DWMWA_DISALLOW_PEEK 11
#endif
	
	BOOL bDisallow = !bEnable;
	
	return DwmSetWindowAttribute(hWnd, DWMWA_DISALLOW_PEEK, &bDisallow, sizeof(bDisallow));
}

BOOL GraphicsMisc::EnableFlip3D(HWND hWnd, BOOL bEnable)
{
#ifndef DWMWA_FLIP3D_POLICY
# define DWMWA_FLIP3D_POLICY 8
# define DWMFLIP3D_DEFAULT      0
# define DWMFLIP3D_EXCLUDEBELOW 1
# define DWMFLIP3D_EXCLUDEABOVE 2
#endif
	
	int nPolicy = bEnable ? DWMFLIP3D_DEFAULT : DWMFLIP3D_EXCLUDEBELOW;
	
	return DwmSetWindowAttribute(hWnd, DWMWA_FLIP3D_POLICY, &nPolicy, sizeof(nPolicy));
}

BOOL GraphicsMisc::DwmSetWindowAttribute(HWND hWnd, DWORD dwAttrib, LPCVOID pData, DWORD dwDataSize)
{
	HMODULE hMod = ::LoadLibrary(_T("Dwmapi.dll"));
	
	if (hMod)
	{
		typedef HRESULT (WINAPI *PFNDWMSETWINDOWATTRIBUTE)(HWND, DWORD, LPCVOID, DWORD);
		PFNDWMSETWINDOWATTRIBUTE pFn = (PFNDWMSETWINDOWATTRIBUTE)::GetProcAddress(hMod, "DwmSetWindowAttribute");
		
		if (pFn)
		{
			HRESULT hr = pFn(hWnd, dwAttrib, pData, dwDataSize);
			return SUCCEEDED(hr);
		}
	}
	
	return FALSE;
}

int GraphicsMisc::DrawSymbol(CDC* pDC, char cSymbol, const CRect& rText, UINT nFlags, CFont* pFont)
{
	if (cSymbol == 0)
		return 0;

	CFont* pOldFont = pFont ? pDC->SelectObject(pFont) : NULL;
	pDC->SetBkMode(TRANSPARENT);
	int nResult = 0;

	// draw as ANSI string
	char szAnsi[2] = { cSymbol, 0 };
	nResult = ::DrawTextA(*pDC, szAnsi, 1, (LPRECT)(LPCRECT)rText, nFlags);
	
	if (pFont)
		pDC->SelectObject(pOldFont);

	return nResult;
}

void GraphicsMisc::DrawRect(CDC* pDC, const CRect& rect, COLORREF crFill, COLORREF crBorder, int nCornerRadius, DWORD dwEdges)
{
	if (rect.IsRectEmpty())
		return;

	if (nCornerRadius == 0)
	{
		// can't have border color and no edges
		if (dwEdges == 0)
			crBorder = NOCOLOR;

		// if both colours are set there's an optimisation we can do
		if ((crFill != NOCOLOR) && (crBorder != NOCOLOR))
		{
			pDC->FillSolidRect(rect, crBorder);

			if (crFill != crBorder)
			{
				CRect rFill(rect);

				if ((dwEdges & GMDR_ALL) == GMDR_ALL)
				{
					rFill.DeflateRect(1, 1);
				}
				else
				{
					if (dwEdges & GMDR_LEFT)
						rFill.left++;

					if (dwEdges & GMDR_TOP)
						rFill.top++;

					if (dwEdges & GMDR_RIGHT)
						rFill.right--;

					if (dwEdges & GMDR_BOTTOM)
						rFill.bottom--;
				}

				pDC->FillSolidRect(rFill, crFill);
			}
		}
		else if (crFill != NOCOLOR) // inside of rect
		{
			pDC->FillSolidRect(rect, crFill);
		}
		else if (crBorder != NOCOLOR) // border
		{
			if (dwEdges & GMDR_TOP)
				pDC->FillSolidRect(rect.left, rect.top, rect.Width(), 1, crBorder);

			if (dwEdges & GMDR_BOTTOM)
				pDC->FillSolidRect(rect.left, rect.bottom - 1, rect.Width(), 1, crBorder);

			if (dwEdges & GMDR_LEFT)
				pDC->FillSolidRect(rect.left, rect.top, 1, rect.Height(), crBorder);

			if (dwEdges & GMDR_RIGHT)
				pDC->FillSolidRect(rect.right - 1, rect.top, 1, rect.Height(), crBorder);
		}
	}
	else // round-rect
	{
		CPen* pOldPen = NULL, penBorder;
		CBrush* pOldBrush = NULL, brFill;

		// inside of rect
		if (crFill != NOCOLOR)
		{
			brFill.CreateSolidBrush(crFill);
			pOldBrush = pDC->SelectObject(&brFill);
		}

		// border
		if (crBorder != NOCOLOR)
		{
			penBorder.CreatePen(PS_SOLID, 1, crBorder);
			pOldPen = pDC->SelectObject(&penBorder);
		}

		pDC->RoundRect(rect, CPoint(nCornerRadius, nCornerRadius));

		// cleanup
		pDC->SelectObject(pOldBrush);
		pDC->SelectObject(pOldPen);
	}
}

BOOL GraphicsMisc::GrayScale(CBitmap& bitmap, COLORREF crMask)
{
	// create 'nice' disabled image
	C32BIPArray aProcessors;
	CImageGrayer ip2(0.33, 0.33, 0.33);
	CImageLightener ip3(0.2);
	CImageTinter ip4(GetSysColor(COLOR_3DSHADOW), 10);
	CImageContraster ip5(-30);
	
	aProcessors.Add(&ip2);
	aProcessors.Add(&ip3);
	aProcessors.Add(&ip4);
	aProcessors.Add(&ip5);

	CEnBitmap bmpEnhanced(crMask);
	bmpEnhanced.Attach(bitmap.Detach());

	BOOL bRes = bmpEnhanced.ProcessImage(aProcessors, crMask);

	bitmap.Attach(bmpEnhanced.Detach());
	return bRes;
}

BOOL GraphicsMisc::GetAvailableScreenSpace(const CRect& rWnd, CRect& rScreen)
{
	if (rWnd.IsRectEmpty())
		return FALSE;

	HMONITOR hMon = MonitorFromPoint(rWnd.CenterPoint(), MONITOR_DEFAULTTONULL);

	if (hMon == NULL)
		return FALSE;

	MONITORINFO mi = { sizeof(MONITORINFO), 0 };
	
	if (GetMonitorInfo(hMon, &mi))
	{
		rScreen = mi.rcWork;
		return TRUE;
	}

	// else
	ASSERT(0);
	return FALSE;

}

BOOL GraphicsMisc::GetAvailableScreenSpace(HWND hWnd, CRect& rScreen)
{
	if (hWnd == NULL)
		return FALSE;

	// else
	CRect rWnd;
	::GetWindowRect(hWnd, rWnd);

	return GetAvailableScreenSpace(rWnd, rScreen);
}

BOOL GraphicsMisc::GetAvailableScreenSpace(CRect& rScreen)
{
	return SystemParametersInfo(SPI_GETWORKAREA, 0, &rScreen, 0);
}

