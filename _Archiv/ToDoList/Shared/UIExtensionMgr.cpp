// ImportExportMgr.cpp: implementation of the CUIExtensionMgr class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "UIExtensionMgr.h"
#include "ITaskList.h"
#include "IUIExtension.h"
#include "filemisc.h"
#include "localizer.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CUIExtensionMgr::CUIExtensionMgr() : m_bInitialized(FALSE)
{

}

CUIExtensionMgr::~CUIExtensionMgr()
{
	Release();
}

void CUIExtensionMgr::Initialize() const
{
	if (m_bInitialized)
		return;

	// we need a non-const pointer to update the array
	CUIExtensionMgr* pMgr = const_cast<CUIExtensionMgr*>(this);

	// look at every dll from wherever we are now
	CFileFind ff;
    CString sSearchPath = FileMisc::GetAppFileName(), sFolder, sDrive;

	FileMisc::SplitPath(sSearchPath, &sDrive, &sFolder);
	FileMisc::MakePath(sSearchPath, sDrive, sFolder, _T("*"), _T(".dll"));

	BOOL bContinue = ff.FindFile(sSearchPath);
	
	while (bContinue)
	{
		bContinue = ff.FindNextFile();
		
		if (!ff.IsDots() && !ff.IsDirectory())
		{
			CString sDllPath = ff.GetFilePath();

			if (IsUIExtemsionDll(sDllPath))
			{
				IUIExtension* pExtension = CreateUIExtensionInterface(sDllPath);

				if (pExtension)
				{
					// init translator
					pExtension->SetLocalizer(CLocalizer::GetLocalizer());

					// save
					pMgr->m_aUIExtensions.Add(pExtension);
				}
			}
		}
	}

	pMgr->m_bInitialized = TRUE;
}

void CUIExtensionMgr::Release()
{
	if (!m_bInitialized)
		return;

	// cleanup
	int nInterface = m_aUIExtensions.GetSize();

	while (nInterface--)
		m_aUIExtensions[nInterface]->Release();

	m_aUIExtensions.RemoveAll();
}

void CUIExtensionMgr::UpdateLocalizer()
{
	if (!m_bInitialized)
		return;

	int nInterface = m_aUIExtensions.GetSize();

	while (nInterface--)
		m_aUIExtensions[nInterface]->SetLocalizer(CLocalizer::GetLocalizer());
}

int CUIExtensionMgr::GetNumUIExtensions() const
{
	Initialize(); // initialize on demand

	return m_aUIExtensions.GetSize();
}

CString CUIExtensionMgr::GetUIExtensionMenuText(int nExtension) const
{
	IUIExtension* pExt = GetUIExtension(nExtension);
	CString sText = (pExt ? pExt->GetMenuText() : _T(""));

	return Clean(sText);
}

CString CUIExtensionMgr::GetUIExtensionTypeID(int nExtension) const
{
	IUIExtension* pExt = GetUIExtension(nExtension);
	CString sText = (pExt ? pExt->GetTypeID() : _T(""));

	return Clean(sText);
}

IUIExtension* CUIExtensionMgr::GetUIExtension(int nExtension) const
{
	Initialize(); // initialize on demand

	IUIExtension* pExt = NULL;

	if (nExtension >= 0 && nExtension < m_aUIExtensions.GetSize())
		pExt = m_aUIExtensions[nExtension];

	return pExt;
}

HICON CUIExtensionMgr::GetUIExtensionIcon(int nExtension) const
{
	IUIExtension* pExt = GetUIExtension(nExtension);
	return (pExt ? pExt->GetIcon() : NULL);
}

CString& CUIExtensionMgr::Clean(CString& sText)
{
	sText.TrimLeft();
	sText.TrimRight();

	return sText;
}

int CUIExtensionMgr::FindUIExtension(LPCTSTR szTypeID) const
{
	Initialize(); // initialize on demand

	int nContent = GetNumUIExtensions();

	while (nContent--)
	{
		ASSERT (m_aUIExtensions[nContent] != NULL);

		if (GetUIExtensionTypeID(nContent).Compare(szTypeID) == 0)
			return nContent;
	}

	// else not found
	return -1;
}

