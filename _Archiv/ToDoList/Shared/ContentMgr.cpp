// ContentMgr.cpp: implementation of the CContentMgr class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ContentMgr.h"
#include "IContentControl.h"
#include "filemisc.h"
#include "binarydata.h"
#include "localizer.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CContentMgr::CContentMgr() : m_bInitialized(FALSE), m_bSomeBadVersions(FALSE)
{

}

CContentMgr::~CContentMgr()
{
	Release();
}

void CContentMgr::Release()
{
	if (!m_bInitialized)
		return;

	// cleanup
	int nInterface = m_aContent.GetSize();

	while (nInterface--)
		m_aContent[nInterface]->Release();

	m_aContent.RemoveAll();

	m_bInitialized = FALSE;
}

BOOL CContentMgr::SomePluginsHaveBadversions() const 
{ 
	Initialize(); // initialize on demand

	return m_bSomeBadVersions; 
}

void CContentMgr::Initialize() const
{
	if (m_bInitialized)
		return;

	// we need a non-const pointer to update the array
	CContentMgr* pMgr = const_cast<CContentMgr*>(this);

	// look at every dll from whereever we are now
	CFileFind ff;
    CString sSearchPath = FileMisc::GetModuleFileName(), sFolder, sDrive;

	FileMisc::SplitPath(sSearchPath, &sDrive, &sFolder);
	FileMisc::MakePath(sSearchPath, sDrive, sFolder, _T("*"), _T(".dll"));

	BOOL bContinue = ff.FindFile(sSearchPath);
	
	while (bContinue)
	{
		bContinue = ff.FindNextFile();
		
		if (!ff.IsDots() && !ff.IsDirectory())
		{
			CString sDllPath = ff.GetFilePath(), sLog;

			if (IsContentDll(sDllPath))
			{
				int nDllVer = 0;
				IContent* pContent = CreateContentInterface(sDllPath, &nDllVer);

				if (pContent)
				{
					// init translator
					pContent->SetLocalizer(CLocalizer::GetLocalizer());

					// save
					pMgr->m_aContent.Add(pContent);
				}
				else
				{
					if (nDllVer < ICONTENTCTRL_VERSION)
						pMgr->m_bSomeBadVersions = TRUE;
				}
			}
		}
	}

	pMgr->m_bInitialized = TRUE;
}

void CContentMgr::UpdateLocalizer()
{
	if (!m_bInitialized)
		return;

	int nInterface = m_aContent.GetSize();

	while (nInterface--)
		m_aContent[nInterface]->SetLocalizer(CLocalizer::GetLocalizer());
}

int CContentMgr::GetNumContent() const
{
	Initialize(); // initialize on demand

	return m_aContent.GetSize();
}

CString CContentMgr::GetContentTypeID(int nContent) const
{
	Initialize(); // initialize on demand

	if (nContent >= 0 && nContent < m_aContent.GetSize())
	{
		ASSERT (m_aContent[nContent] != NULL);
		return m_aContent[nContent]->GetTypeID();
	}

	// else
	return "";
}

CString CContentMgr::GetContentTypeDescription(int nContent) const
{
	Initialize(); // initialize on demand

	if (nContent >= 0 && nContent < m_aContent.GetSize())
	{
		ASSERT (m_aContent[nContent] != NULL);
		return m_aContent[nContent]->GetTypeDescription();
	}
	
	// else
	return "";
}

BOOL CContentMgr::ContentFormatIsText(int nContent) const
{
	return GetContentFormat(nContent).FormatIsText();
}

BOOL CContentMgr::ContentFormatIsText(const CString& sTypeID) const
{
	return ContentFormatIsText(FindContent(sTypeID));
}

BOOL CContentMgr::CreateContentControl(int nContent, CContentCtrl& ctrl, UINT nCtrlID, DWORD nStyle, 
							 DWORD dwExStyle, const CRect& rect, HWND hwndParent)
{
	Initialize(); // initialize on demand

	if (nContent >= 0 && nContent < m_aContent.GetSize())
	{
		ASSERT (m_aContent[nContent] != NULL);

		IContentControl* pControl = m_aContent[nContent]->CreateCtrl((WORD)nCtrlID, nStyle, rect.left, 
															rect.top, rect.Width(), rect.Height(), hwndParent);

		if (pControl && pControl->GetHwnd())
		{
			if (dwExStyle)
				CWnd::ModifyStyleEx(pControl->GetHwnd(), 0, dwExStyle, 0);

			return ctrl.Attach(pControl);
		}

		if (pControl)
			pControl->Release();
	}

	// else
	return FALSE;
}

BOOL CContentMgr::CreateContentControl(const CONTENTFORMAT& cf, CContentCtrl& ctrl, UINT nCtrlID, DWORD nStyle, 
							 DWORD dwExStyle, const CRect& rect, HWND hwndParent)
{
	// check if the CContentCtrl already has a valid control
	if (ctrl.GetSafeHwnd() && ctrl.IsFormat(cf))
		return TRUE;

	return CreateContentControl(FindContent(cf), ctrl, nCtrlID, nStyle, dwExStyle, rect, hwndParent);
}

int CContentMgr::FindContent(LPCTSTR szID) const
{
	Initialize(); // initialize on demand

	int nContent = m_aContent.GetSize();

	while (nContent--)
	{
		ASSERT (m_aContent[nContent] != NULL);

		if (GetContentTypeID(nContent).Compare(szID) == 0)
			return nContent;
	}

	// else not found
	return -1;
}

CONTENTFORMAT CContentMgr::GetContentFormat(int nContent) const
{
	return GetContentTypeID(nContent);
}

BOOL CContentMgr::ConvertContentToHtml(const CBinaryData& content, CString& sHtml, LPCTSTR szID, LPCTSTR szCharSet, LPCTSTR szImageDir)
{
	Initialize(); // initialize on demand

	int nContent = FindContent(szID);

	if (nContent == -1)
		return FALSE;

	LPTSTR szHtml = NULL;

	ASSERT (m_aContent[nContent] != NULL);
	
	int nHtmlLen = m_aContent[nContent]->ConvertToHtml(content.Get(), content.GetLength(), szCharSet, szHtml, szImageDir);

	if (nHtmlLen)
		sHtml = szHtml;

	delete [] szHtml; // cleanup
	
	return (nHtmlLen > 0);
}
