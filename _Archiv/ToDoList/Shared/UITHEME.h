// UITHEME.h: interface for the UITHEME class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_UITHEME_H__7747061B_7841_47E8_BB36_89C11D6DC934__INCLUDED_)
#define AFX_UITHEME_H__7747061B_7841_47E8_BB36_89C11D6DC934__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

enum UI_RENDER_STYLE
{
	UIRS_GRADIENT,
	UIRS_GLASS,
	UIRS_GLASSWITHGRADIENT,
};

enum UI_IMAGE_STYLE
{
	UIIS_STANDARD,
	UIIS_GRAYSCALE,
	UIIS_METRO,
};

const COLORREF UIT_NOCOLOR = (COLORREF)-1;

struct UITHEME
{
	UI_RENDER_STYLE nRenderStyle;
	UI_IMAGE_STYLE nImageStyle;

	COLORREF crAppBackDark;
	COLORREF crAppBackLight;
	COLORREF crAppLinesDark;
	COLORREF crAppLinesLight;
	COLORREF crAppText;
	COLORREF crMenuBack;
	COLORREF crToolbarDark;
	COLORREF crToolbarLight;
	COLORREF crStatusBarDark;
	COLORREF crStatusBarLight;
	COLORREF crStatusBarText;
	//COLORREF ;
	//COLORREF ;
	//COLORREF ;
	//COLORREF ;

	
};

#endif // !defined(AFX_UITHEME_H__7747061B_7841_47E8_BB36_89C11D6DC934__INCLUDED_)
