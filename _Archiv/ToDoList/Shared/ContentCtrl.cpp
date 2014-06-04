// ContentCtrl.cpp: implementation of the CContentCtrl class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ContentCtrl.h"
#include "autoflag.h"
#include "misc.h"
#include "uitheme.h"
#include "binarydata.h"

#include "IContentControl.h"
#include "ISpellCheck.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

BOOL CONTENTFORMAT::FormatIsText() const
{
	GUID id;
	return (Misc::GuidFromString(*this, id) == FALSE);
}

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CContentCtrl::CContentCtrl(IContentControl* pContentCtrl) : 
	m_pContentCtrl(pContentCtrl), m_bSettingContent(FALSE)
{

}

CContentCtrl::~CContentCtrl()
{
	if (m_pContentCtrl)
	{
		m_pContentCtrl->Release();
		m_pContentCtrl = NULL;
	}
}

CContentCtrl::operator HWND() const
{ 
	if (m_pContentCtrl)
		return m_pContentCtrl->GetHwnd();
	
	// else
	return NULL;
}

BOOL CContentCtrl::PreTranslateMessage(MSG* pMsg)
{
	if (m_pContentCtrl)
	{
		// allow tooltip handling thru
		CWnd* pWnd = CWnd::FromHandle(GetSafeHwnd());
		pWnd->FilterToolTipMessage(pMsg);

		// only process if we have the focus
		if (HasFocus())
			return m_pContentCtrl->ProcessMessage(pMsg);
	}

	return FALSE;
}

BOOL CContentCtrl::Attach(IContentControl* pContentCtrl)
{
	ASSERT (pContentCtrl && pContentCtrl->GetHwnd());

	if (pContentCtrl && pContentCtrl->GetHwnd())
	{
		// release existing control
		if (m_pContentCtrl)
			m_pContentCtrl->Release();

		m_pContentCtrl = pContentCtrl;
		return TRUE;
	}

	// else
	return FALSE;
}

void CContentCtrl::SetUITheme(const UITHEME* pTheme)
{
	if (m_pContentCtrl)
		m_pContentCtrl->SetUITheme(pTheme);
}

void CContentCtrl::SavePreferences(IPreferences* pPrefs, LPCTSTR szKey) const
{
	if (m_pContentCtrl && szKey && *szKey)
		m_pContentCtrl->SavePreferences(pPrefs, szKey);
}

void CContentCtrl::LoadPreferences(const IPreferences* pPrefs, LPCTSTR szKey)
{
	if (m_pContentCtrl && szKey && *szKey)
		m_pContentCtrl->LoadPreferences(pPrefs, szKey);
}

BOOL CContentCtrl::HasFocus() const
{
	if (m_pContentCtrl)
	{
		HWND hFocus = ::GetFocus();
		HWND hThis = GetSafeHwnd();

		return (hFocus == hThis) || ::IsChild(hThis, hFocus);
	}

	// else
	return FALSE;
}

void CContentCtrl::SetFocus()
{
	HWND hwndThis = GetSafeHwnd();

	if (::IsWindowEnabled(hwndThis)/* && !HasFocus()*/)
		::SetFocus(hwndThis);
}

ISpellCheck* CContentCtrl::GetSpellCheckInterface()
{
	if (m_pContentCtrl)
		return m_pContentCtrl->GetSpellCheckInterface();

	// else
	return NULL;
}

BOOL CContentCtrl::Undo()
{
	if (m_pContentCtrl)
		return m_pContentCtrl->Undo();

	// else
	return FALSE;
}

BOOL CContentCtrl::Redo()
{
	if (m_pContentCtrl)
		return m_pContentCtrl->Redo();

	// else
	return FALSE;
}

BOOL CContentCtrl::ModifyStyle(DWORD dwRemove, DWORD dwAdd, UINT nFlags)
{
	if (GetSafeHwnd())
		return CWnd::ModifyStyle(*this, dwRemove, dwAdd, nFlags);

	// else
	return FALSE;
}

BOOL CContentCtrl::ModifyStyleEx(DWORD dwRemove, DWORD dwAdd, UINT nFlags)
{
	if (GetSafeHwnd())
		return CWnd::ModifyStyleEx(*this, dwRemove, dwAdd, nFlags);

	// else
	return FALSE;
}

LRESULT CContentCtrl::SendMessage(UINT message, WPARAM wParam, LPARAM lParam)
{
	if (m_pContentCtrl)
		return ::SendMessage(*this, message, wParam, lParam);

	return 0L;
}

BOOL CContentCtrl::SetReadOnly(BOOL bReadOnly)
{
	if (m_pContentCtrl)
	{
		m_pContentCtrl->SetReadOnly(bReadOnly != FALSE);
		return TRUE;
	}

	return FALSE;
}

BOOL CContentCtrl::PostMessage(UINT message, WPARAM wParam, LPARAM lParam)
{
	if (m_pContentCtrl)
		return ::PostMessage(*this, message, wParam, lParam);

	return FALSE;
}

int CContentCtrl::GetContent(unsigned char* pContent) const
{
	if (m_pContentCtrl)
		return m_pContentCtrl->GetContent(pContent);

	// else
	return 0;
}

BOOL CContentCtrl::SetContent(const unsigned char* pContent, int nLength, BOOL bResetSelection)
{
	CAutoFlag af(m_bSettingContent, TRUE);

	if (m_pContentCtrl)
		return m_pContentCtrl->SetContent((unsigned char*)pContent, nLength, bResetSelection);

	// else
	return false;
}

BOOL CContentCtrl::SetContent(const CBinaryData& content, BOOL bResetSelection)
{
	return SetContent(content.Get(), content.GetLength(), bResetSelection);
}

int CContentCtrl::GetContent(CBinaryData& content) const
{
	int nLenBytes = 0;
	content.Empty();
	
	if (m_pContentCtrl)
	{
		nLenBytes = m_pContentCtrl->GetContent(NULL);
		
		if (nLenBytes) // excludes NULL
		{
			unsigned char* szContent = content.GetBuffer(nLenBytes);
			nLenBytes = m_pContentCtrl->GetContent(szContent);
			content.ReleaseBuffer(nLenBytes);
		}
	}
	
	return nLenBytes;
}

int CContentCtrl::GetTextContent(CString& sContent) const
{
	int nLen = 0;
	sContent.Empty();

	if (m_pContentCtrl)
	{
		int nLen = m_pContentCtrl->GetTextContent(NULL);

		if (nLen)
		{
			LPTSTR szContent = sContent.GetBuffer(nLen + 1);
			nLen = m_pContentCtrl->GetTextContent(szContent, nLen + 1);
			sContent.ReleaseBuffer(nLen);
		}
	}

	// else
	return nLen;
}

BOOL CContentCtrl::SetTextContent(LPCTSTR szContent, BOOL bResetSelection)
{
	CAutoFlag af(m_bSettingContent, TRUE);

	if (m_pContentCtrl)
		return m_pContentCtrl->SetTextContent(szContent, bResetSelection);

	// else
	return false;
}

LPCTSTR CContentCtrl::GetTypeID() const
{
	if (m_pContentCtrl)
		return m_pContentCtrl->GetTypeID();

	// else
	return _T("");
}

BOOL CContentCtrl::IsFormat(const CONTENTFORMAT& cf) const
{
	return (!cf.IsEmpty() && cf == GetContentFormat());
}

CONTENTFORMAT CContentCtrl::GetContentFormat() const
{
	return GetTypeID();
}
