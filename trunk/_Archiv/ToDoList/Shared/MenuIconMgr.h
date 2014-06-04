// MenuIconMgr.h: interface for the CMenuIconMgr class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MENUICONMGR_H__0FF0228C_515C_4E93_A957_1952AFD0F3A1__INCLUDED_)
#define AFX_MENUICONMGR_H__0FF0228C_515C_4E93_A957_1952AFD0F3A1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\shared\Subclass.h"
#include <afxtempl.h>

class CMenuIconMgr : public CSubclassWnd  
{
public:
	CMenuIconMgr();
	virtual ~CMenuIconMgr();
	
	BOOL Initialize(CWnd* pWnd);
	void Release();

	BOOL AddImage(UINT nCmdID, HICON hIcon, BOOL bNormal = TRUE); // hIcon will be copied
	BOOL SetImage(UINT nCmdID, HICON hIcon, BOOL bNormal = TRUE); // hIcon will be cleaned up
	BOOL AddImage(UINT nCmdID, const CImageList& il, int nImage, BOOL bNormal = TRUE);

	int AddImages(const CToolBar& toolbar);
	int AddImages(const CUIntArray& aCmdIDs, const CImageList& il, const CImageList* pILDisabled = NULL);
	int AddImages(const CUIntArray& aCmdIDs, UINT nIDBitmap, int nCx, COLORREF crMask);
	
	void ChangeImageID(UINT nCmdID, UINT nNewCmdID);
	void DeleteImage(UINT nCmdID);
	
	void ClearImages();
	
protected:
	CMap<UINT, UINT, HICON, HICON> m_mapID2Icon;
	CMap<UINT, UINT, HICON, HICON> m_mapID2DisabledIcon;
	
	virtual LRESULT WindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp);
	
	BOOL OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpdis);
	void OnInitMenuPopup(CMenu* pMenu);
	BOOL OnMeasureItem(int nIDCtl, LPMEASUREITEMSTRUCT lpmis);
	
	HICON LoadItemImage(UINT nCmdID, BOOL bNormal);
	void ClearImages(BOOL bNormal);

private:
	CMap<UINT, UINT, HICON, HICON>& ImageMap(BOOL bNormal);
};

#endif // !defined(AFX_MENUICONMGR_H__0FF0228C_515C_4E93_A957_1952AFD0F3A1__INCLUDED_)
