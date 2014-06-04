// MenuIconMgr.cpp: implementation of the CMenuIconMgr class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "MenuIconMgr.h"
#include "themed.h"
#include "graphicsmisc.h"

typedef unsigned long ULONG_PTR; 

#ifndef COLOR_MENUHILIGHT
#	define COLOR_MENUHILIGHT 29
#endif

#define NOCOLOR ((COLORREF)-1)
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMenuIconMgr::CMenuIconMgr()
{
   
}

CMenuIconMgr::~CMenuIconMgr()
{
	// cleanup icons
	ClearImages();
}

CMap<UINT, UINT, HICON, HICON>& CMenuIconMgr::ImageMap(BOOL bNormal)
{
	return (bNormal ? m_mapID2Icon : m_mapID2DisabledIcon); 
}

BOOL CMenuIconMgr::Initialize(CWnd* pWnd)
{
	if (!IsHooked())
		return HookWindow(pWnd->GetSafeHwnd());
	
	// else
	return TRUE;
}

void CMenuIconMgr::Release()
{
	if (IsHooked())
	{
		ClearImages();
		CSubclassWnd::HookWindow(NULL);
	}
}

void CMenuIconMgr::ClearImages(BOOL bNormal)
{
	POSITION pos = ImageMap(bNormal).GetStartPosition();
	
	while (pos)
	{
		UINT nCmdID = 0;
		HICON hIcon = NULL;
		
		ImageMap(bNormal).GetNextAssoc(pos, nCmdID, hIcon);
		::DestroyIcon(hIcon);
	}
	
	ImageMap(bNormal).RemoveAll();
}

void CMenuIconMgr::ClearImages()
{
	ClearImages(TRUE);
	ClearImages(FALSE);
}

int CMenuIconMgr::AddImages(const CToolBar& toolbar)
{
	// iterate the non-separator items extracting their images 
	// from the imagelist
	const CToolBarCtrl& tbc = toolbar.GetToolBarCtrl();
	int nBtnCount = tbc.GetButtonCount(), nImage = 0;

	CImageList* pIL = tbc.GetImageList();
	CImageList* pILDis = tbc.GetDisabledImageList();
	
	for (int nBtn = 0; nBtn < nBtnCount; nBtn++)
	{
		UINT nCmdID = toolbar.GetItemID(nBtn);
		
		if (nCmdID != ID_SEPARATOR)
		{
			m_mapID2Icon[nCmdID] = pIL->ExtractIcon(nImage);

			if (pILDis)
				m_mapID2DisabledIcon[nCmdID] = pILDis->ExtractIcon(nImage);

			nImage++;
		}
	}
	
	return nImage;
}

int CMenuIconMgr::AddImages(const CUIntArray& aCmdIDs, const CImageList& il, const CImageList* pILDisabled)
{
	ASSERT (aCmdIDs.GetSize() == il.GetImageCount());
	ASSERT (!pILDisabled || (aCmdIDs.GetSize() == pILDisabled->GetImageCount()) || pILDisabled->GetImageCount() == 0);
	
	if (aCmdIDs.GetSize() != il.GetImageCount())
		return 0;
	
	if (pILDisabled && pILDisabled->GetImageCount() && aCmdIDs.GetSize() != pILDisabled->GetImageCount())
		return 0;
	
	int nBtnCount = aCmdIDs.GetSize();

	CImageList* pIL = const_cast<CImageList*>(&il);
	CImageList* pILDis = const_cast<CImageList*>(pILDisabled);
	
	for (int nBtn = 0; nBtn < nBtnCount; nBtn++)
	{
		SetImage(aCmdIDs[nBtn], pIL->ExtractIcon(nBtn), TRUE);

		if (pILDis && pILDis->GetImageCount())
			SetImage(aCmdIDs[nBtn], pILDis->ExtractIcon(nBtn), FALSE);
	}
	   
	return nBtnCount;
	
}

int CMenuIconMgr::AddImages(const CUIntArray& aCmdIDs, UINT nIDBitmap, int nCx, COLORREF crMask)
{
	CBitmap bm;
	
	if (bm.LoadBitmap(nIDBitmap))
	{
		CImageList il, ilDis;

		if (il.Create(nCx, 16, ILC_COLOR32 | ILC_MASK, 0, 1))
			il.Add(&bm, crMask);

		if (ilDis.Create(nCx, 16, ILC_COLOR32 | ILC_MASK, 0, 1))
		{
			GraphicsMisc::VerifyDeleteObject(bm);
			bm.LoadBitmap(nIDBitmap);

			if (GraphicsMisc::GrayScale(bm, crMask))
				ilDis.Add(&bm, crMask);
		}

		return AddImages(aCmdIDs, il, &ilDis);
	}
	
	return 0;
}

void CMenuIconMgr::ChangeImageID(UINT nCmdID, UINT nNewCmdID)
{
	// normal
	HICON hIcon = LoadItemImage(nCmdID, TRUE);
	
	if (hIcon)
	{
		m_mapID2Icon.RemoveKey(nCmdID);
		m_mapID2Icon[nNewCmdID] = hIcon;
	}

	// disabled
	hIcon = LoadItemImage(nCmdID, FALSE);
		
	if (hIcon)
	{
		m_mapID2DisabledIcon.RemoveKey(nCmdID);
		m_mapID2DisabledIcon[nNewCmdID] = hIcon;
	}
}

HICON CMenuIconMgr::LoadItemImage(UINT nCmdID, BOOL bNormal) 
{
	HICON hIcon = NULL;
	
	ImageMap(bNormal).Lookup(nCmdID, hIcon);

	// fallback for disabled icon
	if (hIcon == NULL && !bNormal)
		ImageMap(TRUE).Lookup(nCmdID, hIcon);
	
	return hIcon;
}

BOOL CMenuIconMgr::AddImage(UINT nCmdID, HICON hIcon, BOOL bNormal)
{
	// we copy the icon's small image
	CImageList il;
	
	if (il.Create(16, 16, ILC_COLOR32 | ILC_MASK, 0, 1))
	{
		il.Add(hIcon);
		return SetImage(nCmdID, il.ExtractIcon(0), bNormal);
	}
	
	return FALSE;
}

BOOL CMenuIconMgr::AddImage(UINT nCmdID, const CImageList& il, int nImage, BOOL bNormal)
{
	CImageList* pIL = const_cast<CImageList*>(&il);

	return SetImage(nCmdID, pIL->ExtractIcon(nImage), bNormal);
}

void CMenuIconMgr::DeleteImage(UINT nCmdID)
{
	SetImage(nCmdID, NULL, TRUE);
	SetImage(nCmdID, NULL, FALSE);
}

BOOL CMenuIconMgr::SetImage(UINT nCmdID, HICON hIcon, BOOL bNormal)
{
	if (nCmdID)
	{
		if (hIcon)
		{
			ImageMap(bNormal)[nCmdID] = hIcon;
		}
		else
		{
			::DestroyIcon(LoadItemImage(nCmdID, bNormal));
			ImageMap(bNormal).RemoveKey(nCmdID);
		}

		return TRUE;
	}
	
	return FALSE;
}

LRESULT CMenuIconMgr::WindowProc(HWND /*hRealWnd*/, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_INITMENUPOPUP:
		if (!HIWORD(lp)) // let windows look after the system menu 
		{
			LRESULT lr = CSubclassWnd::Default();
			OnInitMenuPopup(CMenu::FromHandle((HMENU)wp));
			return lr;
		}
		break;
		
	case WM_DRAWITEM:
		if (OnDrawItem(wp, (LPDRAWITEMSTRUCT)lp))
			return 0L;
		break;
		
	case WM_MEASUREITEM:
		if (OnMeasureItem(wp, (LPMEASUREITEMSTRUCT)lp))
			return 0L;
		break;
	}
	
	return CSubclassWnd::Default();
}

BOOL CMenuIconMgr::OnDrawItem(int /*nIDCtl*/, LPDRAWITEMSTRUCT lpdis)
{
    if (lpdis == NULL || lpdis->CtlType != ODT_MENU)
        return FALSE; // not for a menu
	
	BOOL bDisabled = (lpdis->itemState & (ODS_GRAYED | ODS_DISABLED));
    HICON hIcon = LoadItemImage(lpdis->itemID, !bDisabled);
	
    if (hIcon)
    {
        ICONINFO iconinfo;
        GetIconInfo(hIcon, &iconinfo);
		
        BITMAP bitmap;
        GetObject(iconinfo.hbmColor, sizeof(bitmap), &bitmap);
		
        ::DrawIconEx(lpdis->hDC, lpdis->rcItem.left, lpdis->rcItem.top, hIcon, 
					bitmap.bmWidth, bitmap.bmHeight, 0, NULL, DI_IMAGE | DI_MASK);
	
		// cleanup
		GraphicsMisc::VerifyDeleteObject(iconinfo.hbmColor);
		GraphicsMisc::VerifyDeleteObject(iconinfo.hbmMask);

		return TRUE;
    }
	
	return FALSE;
}

void CMenuIconMgr::OnInitMenuPopup(CMenu* pMenu)
{
	ASSERT (pMenu);

    MENUINFO mnfo;
    mnfo.cbSize = sizeof(mnfo);
    mnfo.fMask = MIM_STYLE;
    mnfo.dwStyle = MNS_CHECKORBMP | MNS_AUTODISMISS;
	::SetMenuInfo(pMenu->GetSafeHmenu(), &mnfo);
	
    MENUITEMINFO minfo;
    minfo.cbSize = sizeof(minfo);
	
    for (UINT pos=0; pos<pMenu->GetMenuItemCount(); pos++)
    {
        minfo.fMask = MIIM_FTYPE | MIIM_ID;
        pMenu->GetMenuItemInfo(pos, &minfo, TRUE);
		
        HICON hIcon = LoadItemImage(minfo.wID, TRUE);
		
        if (hIcon && !(minfo.fType & MFT_OWNERDRAW))
        {
            minfo.fMask = MIIM_BITMAP | MIIM_DATA;
            minfo.hbmpItem = HBMMENU_CALLBACK;
			
			::SetMenuItemInfo(pMenu->GetSafeHmenu(), pos, TRUE, &minfo);
        }
    }
}

BOOL CMenuIconMgr::OnMeasureItem(int /*nIDCtl*/, LPMEASUREITEMSTRUCT lpmis)
{
    if ((lpmis==NULL)||(lpmis->CtlType != ODT_MENU))
        return FALSE;
	
    lpmis->itemWidth = 16;
    lpmis->itemHeight = 16;
	
    HICON hIcon = LoadItemImage(lpmis->itemID, TRUE);
	
    if (hIcon)
    {
        ICONINFO iconinfo;
        ::GetIconInfo(hIcon, &iconinfo);
		
        BITMAP bitmap;
        ::GetObject(iconinfo.hbmColor, sizeof(bitmap), &bitmap);
		
        lpmis->itemWidth = bitmap.bmWidth;
        lpmis->itemHeight = bitmap.bmHeight;
	
		// cleanup
		GraphicsMisc::VerifyDeleteObject(iconinfo.hbmColor);
		GraphicsMisc::VerifyDeleteObject(iconinfo.hbmMask);
		
		return TRUE;
    }
	
	return FALSE;
}
