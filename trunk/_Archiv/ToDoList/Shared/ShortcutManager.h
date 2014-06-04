// ShortcutManager.h: interface for the CShortcutManager class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_SHORTCUTMANAGER_H__08D5DF0A_7D5E_4266_A244_59C4B2BD5DC2__INCLUDED_)
#define AFX_SHORTCUTMANAGER_H__08D5DF0A_7D5E_4266_A244_59C4B2BD5DC2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "Subclass.h"
#include <afxtempl.h>

// some more 'subtle' invalid modifiers
enum 
{
	HKCOMB_EXFKEYS		= 0x0100, // lets function keys thru
	HKCOMB_EDITCTRLS	= 0x0200, // prevents clashing shortcuts working when in an edit control
};

const DWORD NO_SHORTCUT = 0xffffffff;

class CShortcutManager : protected CSubclassWnd  
{
public:
	CShortcutManager(BOOL bAutoSendCmds = TRUE);
	virtual ~CShortcutManager();

	// hooks AfxGetMainWnd() and only sends commands there
	BOOL Initialize(CWnd* pOwner, WORD wInvalidComb = HKCOMB_EDITCTRLS, WORD wFallbackModifiers = 0);
	BOOL Release();

	UINT ProcessMessage(const MSG* pMsg, DWORD* pShortcut) const; // call this in PreTranslateMessage. returns the cmd ID or 0

	// AddShortcut fails if the shortcut is already being used
	BOOL AddShortcut(UINT nCmdID, WORD wVirtKeyCode, WORD wModifiers = HOTKEYF_CONTROL); 
	BOOL AddShortcut(UINT nCmdID, DWORD dwShortcut); 

	// SetShortcut never fails and will overwrite any existing shortcuts
	void SetShortcut(UINT nCmdID, WORD wVirtKeyCode, WORD wModifiers = HOTKEYF_CONTROL); 
	void SetShortcut(UINT nCmdID, DWORD dwShortcut); 
	
	void DeleteShortcut(UINT nCmdID);
	void SaveSettings();

	UINT GetCommandID(DWORD dwShortcut);
	DWORD GetShortcut(UINT nCmdID) const;
	WORD ValidateModifiers(WORD wModifiers, WORD wVirtKeyCode) const;

	static CString GetShortcutText(DWORD dwShortcut);
	CString GetShortcutTextByCmd(UINT nCmdID);

	int IsEmpty() { return (m_mapID2Shortcut.GetCount() == 0); }
	WORD GetInvalidComb() { return m_wInvalidComb; }

	int BuildMapping(UINT nMenuID, CStringArray& aMapping, char cDelim = '\t');

protected:
	CMap<DWORD, DWORD, UINT, UINT&> m_mapShortcut2ID; // for use in ProcessMsg
	CMap<UINT, UINT, DWORD, DWORD&> m_mapID2Shortcut; // for use in PrepareMenuItems
	WORD m_wInvalidComb, m_wFallbackModifiers;
	BOOL m_bAutoSendCmds;

protected:
	virtual LRESULT WindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp);

	void PrepareMenuItems(CMenu* pMenu) const;
	DWORD GetShortcut(WORD wVirtKeyCode, BOOL bExtended) const;
	void LoadSettings();
	int BuildMapping(CMenu* pMenu, LPCTSTR szParentName, CStringArray& aMapping, char cDelim);

	static CString GetKeyName(WORD wVirtKeyCode, BOOL bExtended = FALSE); 
	static BOOL IsEditShortcut(DWORD dwShortcut);
};

#endif // !defined(AFX_SHORTCUTMANAGER_H__08D5DF0A_7D5E_4266_A244_59C4B2BD5DC2__INCLUDED_)
