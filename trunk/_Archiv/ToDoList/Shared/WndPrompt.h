// EditPrompt.h: interface for the CWndPrompt class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_WNDPROMPT_H__485A738F_5BCB_4D7E_9B3E_6E9382AC62D8__INCLUDED_)
#define AFX_WNDPROMPT_H__485A738F_5BCB_4D7E_9B3E_6E9382AC62D8__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "Subclass.h"
#include <afxtempl.h>

class CWndPrompt : public CSubclassWnd  
{
public:
	CWndPrompt();
	virtual ~CWndPrompt();

	BOOL Initialize(HWND hWnd, LPCTSTR szPrompt, UINT nCheckMsg, LRESULT lRes = 0, int nVertOffset = 0);
	void SetPrompt(LPCTSTR szPrompt, int nVertOffset = -1);

protected:
	CString m_sPrompt;
	UINT m_nCheckMsg;
	LRESULT m_lCheckResult;
	int m_nVertOffset;
	CString m_sClass; // for some tweaking

protected:
	virtual LRESULT WindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp);
	BOOL WantPrompt(BOOL bCheckFocus = TRUE);
	void DrawPrompt(HDC hdc);
};

class CWndPromptManager
{
public:
	CWndPromptManager();
	virtual ~CWndPromptManager();

	BOOL SetPrompt(UINT nIDCtrl, HWND hwndParent, LPCTSTR szPrompt, 
					UINT nCheckMsg, LRESULT lRes = 0, int nVertOffset = 0);
	BOOL SetPrompt(HWND hWnd, LPCTSTR szPrompt, UINT nCheckMsg, 
					LRESULT lRes = 0, int nVertOffset = 0);

	BOOL SetPrompt(UINT nIDCtrl, HWND hwndParent, UINT nIDPrompt, 
					UINT nCheckMsg, LRESULT lRes = 0, int nVertOffset = 0);
	BOOL SetPrompt(HWND hWnd, UINT nIDPrompt, UINT nCheckMsg, 
					LRESULT lRes = 0, int nVertOffset = 0);

	// special cases
	BOOL SetEditPrompt(UINT nIDEdit, HWND hwndParent, LPCTSTR szPrompt);
	BOOL SetEditPrompt(HWND hwndEdit, LPCTSTR szPrompt);
	BOOL SetComboPrompt(UINT nIDCombo, HWND hwndParent, LPCTSTR szPrompt);
	BOOL SetComboPrompt(HWND hwndCombo, LPCTSTR szPrompt);
	BOOL SetComboEditPrompt(UINT nIDCombo, HWND hwndParent, LPCTSTR szPrompt);
	BOOL SetComboEditPrompt(HWND hwndCombo, LPCTSTR szPrompt);

	BOOL SetEditPrompt(UINT nIDEdit, HWND hwndParent, UINT nIDPrompt);
	BOOL SetEditPrompt(HWND hwndEdit, UINT nIDPrompt);
	BOOL SetComboPrompt(UINT nIDCombo, HWND hwndParent, UINT nIDPrompt);
	BOOL SetComboPrompt(HWND hwndCombo, UINT nIDPrompt);
	BOOL SetComboEditPrompt(UINT nIDCombo, HWND hwndParent, UINT nIDPrompt);
	BOOL SetComboEditPrompt(HWND hwndCombo, UINT nIDPrompt);

protected:
	CMap<HWND, HWND, CWndPrompt*, CWndPrompt*&> m_mapWnds;

};

#endif // !defined(AFX_WNDPROMPT_H__485A738F_5BCB_4D7E_9B3E_6E9382AC62D8__INCLUDED_)
