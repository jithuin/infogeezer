// MouseWheelMgr.h: interface for the CMouseWheelMgr class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MOUSEWHEELMGR_H__6738593F_D10B_45D9_ACA0_335DA7C0F630__INCLUDED_)
#define AFX_MOUSEWHEELMGR_H__6738593F_D10B_45D9_ACA0_335DA7C0F630__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "HookMgr.h"

class CMouseWheelMgr : public CHookMgr<CMouseWheelMgr>  
{
   friend class CHookMgr<CMouseWheelMgr>;

public:
	virtual ~CMouseWheelMgr();
 	static BOOL Initialize();
	static void Release();

protected:
	CMouseWheelMgr();
 	static CMouseWheelMgr& Instance() { return CHookMgr<CMouseWheelMgr>::GetInstance(); }

  	virtual BOOL OnMouseEx(UINT uMouseMsg, const MOUSEHOOKSTRUCTEX& info);
};

#endif // !defined(AFX_MOUSEWHEELMGR_H__6738593F_D10B_45D9_ACA0_335DA7C0F630__INCLUDED_)
