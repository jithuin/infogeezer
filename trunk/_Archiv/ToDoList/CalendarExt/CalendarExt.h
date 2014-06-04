// CalendarExtApp.h: interface for the CCalendarExtApp class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_CALENDAREXTAPP_H__8FDB207D_40DB_405D_8D76_A9898B6DAE21__INCLUDED_)
#define AFX_CALENDAREXTAPP_H__8FDB207D_40DB_405D_8D76_A9898B6DAE21__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\SHARED\IUIExtension.h"

// {4A5678B7-6583-43D7-BB00-B1CA5C16D541}
static const GUID CAL_TYPEID = 
{ 0x4a5678b7, 0x6583, 0x43d7, { 0xbb, 0x0, 0xb1, 0xca, 0x5c, 0x16, 0xd5, 0x41 } };

class CCalendarExtApp : public IUIExtension  
{
public:
	CCalendarExtApp();
	virtual ~CCalendarExtApp();

    void Release(); // releases the interface
	void SetLocalizer(ITransText* pTT);

	LPCTSTR GetMenuText() const { return _T("Calendar"); }
	HICON GetIcon() const { return m_hIcon; }
	LPCTSTR GetTypeID() const;

	IUIExtensionWindow* CreateExtWindow(UINT nCtrlID, DWORD nStyle, 
										long nLeft, long nTop, long nWidth, long nHeight, 
										HWND hwndParent);

protected:
	HICON m_hIcon;
};

#endif // !defined(AFX_CALENDAREXTAPP_H__8FDB207D_40DB_405D_8D76_A9898B6DAE21__INCLUDED_)
