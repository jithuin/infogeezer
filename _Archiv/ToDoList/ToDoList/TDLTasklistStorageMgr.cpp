// TDLTasklistStorageMgr.cpp: implementation of the CTDLTasklistStorageMgr class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TDLTasklistStorageMgr.h"

#include "..\shared\Regkey.h"
#include "..\shared\FileMisc.h"
#include "..\shared\EnFileDialog.h"
#include "..\shared\localizer.h"

#include <Shlobj.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

#ifndef CSIDL_PROFILE
#	define CSIDL_PROFILE 0x28
#endif

//////////////////////////////////////////////////////////////////////

class CBaseCloudStorage : public ITasklistStorage  
{
public:
	CBaseCloudStorage() {}
	virtual ~CBaseCloudStorage() {}
	
	// interface implementation
    virtual void Release() { delete this; }
	void SetLocalizer(ITransText* pTT) { CLocalizer::Initialize(pTT); }

	virtual BOOL IsInstalled(CString& sUserFolder) const = 0;
	virtual bool RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);
	virtual bool StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);

protected:
	CString FormatDisplayName(const CString& sFilePath);
	void UpdateTaskListInfo(ITS_TASKLISTINFO* pFInfo, const CString& sFilePath);
};

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

bool CBaseCloudStorage::RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* /*pDestTaskFile*/, IPreferences* /*pPrefs*/, LPCTSTR /*szKey*/, bool bSilent)
{
	CString sUserFolder;
	
	if (IsInstalled(sUserFolder))
	{
		CString sSrcPath = pFInfo->szTasklistID;
		CString sDestPath = pFInfo->szLocalFileName;
		
		if (bSilent && sSrcPath.IsEmpty())
			return false;
		
		if (sSrcPath.IsEmpty())
		{
			CFileOpenDialog	dialog(GetMenuText(), 
									_T("tdl"), 
									NULL, 
									EOFN_DEFAULTOPEN, 
									_T("Tasklists (*.tdl)|*.tdl"));

			dialog.m_ofn.lpstrInitialDir = sUserFolder;
			
			if (dialog.DoModal() != IDOK)
				return false;
			
			// else
			sSrcPath = dialog.GetPathName();

			if (sDestPath.IsEmpty())
				sDestPath = sSrcPath;
		}
		
		if (FileMisc::IsSameFile(sSrcPath, sDestPath) || ::CopyFile(sSrcPath, sDestPath, FALSE))
		{
			// return information to caller
			UpdateTaskListInfo(pFInfo, sDestPath);
			return true;
		}
	}
	
	return false;
}

bool CBaseCloudStorage::StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* /*pSrcTaskFile*/, IPreferences* /*pPrefs*/, LPCTSTR /*szKey*/, bool bSilent)
{
	CString sUserFolder;
	
	if (IsInstalled(sUserFolder))
	{
		CString sSrcPath = pFInfo->szLocalFileName;
		CString sDestPath = pFInfo->szTasklistID;
		
		if (bSilent && sDestPath.IsEmpty())
			return false;
		
		if (sDestPath.IsEmpty())
		{
			CFileSaveDialog	dialog(GetMenuText(), 
									_T("tdl"), 
									FileMisc::GetFileNameFromPath(sDestPath.IsEmpty() ? sSrcPath : sDestPath), 
									EOFN_DEFAULTSAVE, 
									_T("Tasklists (*.tdl)|*.tdl"));

			dialog.m_ofn.lpstrInitialDir = sUserFolder;
			
			if (dialog.DoModal() != IDOK)
				return false;
			
			// else 
			sDestPath = dialog.GetPathName();
		}
		
		if (FileMisc::IsSameFile(sSrcPath, sDestPath) || ::CopyFile(sSrcPath, sDestPath, FALSE))
		{
			// return information to caller
			UpdateTaskListInfo(pFInfo, sDestPath);
			return true;
		}
	}
	
	return false;
}

CString CBaseCloudStorage::FormatDisplayName(const CString& sFilePath)
{
	CString sDisplay;
	sDisplay.Format(_T("%s: %s"), GetMenuText(), FileMisc::GetFileNameFromPath(sFilePath));
	
	return sDisplay;
}

void CBaseCloudStorage::UpdateTaskListInfo(ITS_TASKLISTINFO* pFInfo,  
											const CString& sFilePath)
{
	ASSERT(FileMisc::FileExists(sFilePath));
	
	_tcsncpy(pFInfo->szLocalFileName, sFilePath, _MAX_PATH);
	_tcsncpy(pFInfo->szDisplayName, FormatDisplayName(sFilePath), _MAX_PATH);
//	_tcsncpy(pFInfo->szPassword, rf.GetPassword(), PASSWORD_LEN);
	
	_tcsncpy(pFInfo->szTasklistID, sFilePath, TASKLISTID_LEN);
}

//////////////////////////////////////////////////////////////////////
// Private File based storage providers

class CGoogleDriveStorage : public CBaseCloudStorage  
{
public:
	CGoogleDriveStorage();
	virtual ~CGoogleDriveStorage();

	static BOOL IsGoogleDriveInstalled();
	
	// caller must copy result only
	LPCTSTR GetMenuText() const { return _T("Google Drive"); }
	LPCTSTR GetTypeID() const { return _T("3ADF91FF-CAB9-44FF-BAA6-F397172149AF"); }
	
	bool RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);
	bool StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);
	
protected:
	BOOL IsInstalled(CString& sUserFolder) const;
};

class CSkyDriveStorage : public CBaseCloudStorage  
{
public:
	CSkyDriveStorage();
	virtual ~CSkyDriveStorage();
	
	static BOOL IsSkyDriveInstalled();
	
	// caller must copy result only
	LPCTSTR GetMenuText() const { return _T("SkyDrive"); }
	LPCTSTR GetTypeID() const { return _T("0BE9A394-86EC-412C-A88A-496A737E6110"); }
	
	bool RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);
	bool StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);
	
protected:
	BOOL IsInstalled(CString& sUserFolder) const;
};

class CDropBoxStorage : public CBaseCloudStorage  
{
public:
	CDropBoxStorage();
	virtual ~CDropBoxStorage();
	
	static BOOL IsDropBoxInstalled();
	
	// caller must copy result only
	LPCTSTR GetMenuText() const { return _T("DropBox"); }
	LPCTSTR GetTypeID() const { return _T("9F553544-85AA-4EDD-B0FD-71486246C3E2"); }
	
	bool RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);
	bool StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);
	
protected:
	BOOL IsInstalled(CString& sUserFolder) const;
};

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CTDLTasklistStorageMgr::CTDLTasklistStorageMgr()
{
#ifdef _DEBUG
	Initialize();
#endif
}

CTDLTasklistStorageMgr::~CTDLTasklistStorageMgr()
{

}

void CTDLTasklistStorageMgr::Initialize() const
{
	if (m_bInitialized)
		return;

	// base class
	CTasklistStorageMgr::Initialize();
	
	// we need a non-const pointer to update the array
	CTDLTasklistStorageMgr* pMgr = const_cast<CTDLTasklistStorageMgr*>(this);

	// add built-in storage handlers
	if (CGoogleDriveStorage::IsGoogleDriveInstalled())
		pMgr->m_aStorage.Add(new CGoogleDriveStorage);

	if (CSkyDriveStorage::IsSkyDriveInstalled())
		pMgr->m_aStorage.Add(new CSkyDriveStorage);

	if (CDropBoxStorage::IsDropBoxInstalled())
		pMgr->m_aStorage.Add(new CDropBoxStorage);
}

///////////////////////////////////////////////////////////////////////

CGoogleDriveStorage::CGoogleDriveStorage()
{

}

CGoogleDriveStorage::~CGoogleDriveStorage()
{

}

BOOL CGoogleDriveStorage::IsGoogleDriveInstalled()
{
	CString sUserFolder;
	return CGoogleDriveStorage().IsInstalled(sUserFolder);
}

BOOL CGoogleDriveStorage::IsInstalled(CString& sUserFolder) const
{
	CRegKey reg;
	
	if (reg.Open(HKEY_CURRENT_USER, _T("Software\\Google\\Drive")) == ERROR_SUCCESS)
	{
		CString sInstalled;
		
		if ((reg.Read(_T("Installed"), sInstalled) == ERROR_SUCCESS) && (sInstalled == "True"))
		{
			// Google encrypts the user's folder location so we have a guess
			if (FileMisc::GetSpecialFolder(CSIDL_PROFILE, sUserFolder))
			{
				sUserFolder += _T("\\Google Drive");
				
				// if that folder doesn't exist then we'll have
				// to prompt the user
				if (!FileMisc::FolderExists(sUserFolder))
					sUserFolder.Empty();
			}
			
			return TRUE;
		}
	}
	
	// else
	return FALSE;
}

bool CGoogleDriveStorage::RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent)
{
	return CBaseCloudStorage::RetrieveTasklist(pFInfo, pDestTaskFile, pPrefs, szKey, bSilent);
}

bool CGoogleDriveStorage::StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent)
{
	return CBaseCloudStorage::StoreTasklist(pFInfo, pSrcTaskFile, pPrefs, szKey, bSilent);
}

///////////////////////////////////////////////////////////////////////

CSkyDriveStorage::CSkyDriveStorage()
{

}

CSkyDriveStorage::~CSkyDriveStorage()
{

}

BOOL CSkyDriveStorage::IsSkyDriveInstalled()
{
	CString sUserFolder;
	return CSkyDriveStorage().IsInstalled(sUserFolder);
}

BOOL CSkyDriveStorage::IsInstalled(CString& sUserFolder) const
{
	CRegKey reg;
	
	if (reg.Open(HKEY_CURRENT_USER, _T("Software\\Microsoft\\SkyDrive")) == ERROR_SUCCESS)
	{
		if (reg.Read(_T("UserFolder"), sUserFolder) == ERROR_SUCCESS)
		{
			return FileMisc::FolderExists(sUserFolder);
		}
	}
	
	// else
	return FALSE;
}

bool CSkyDriveStorage::RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent)
{
	return CBaseCloudStorage::RetrieveTasklist(pFInfo, pDestTaskFile, pPrefs, szKey, bSilent);
}

bool CSkyDriveStorage::StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent)
{
	return CBaseCloudStorage::StoreTasklist(pFInfo, pSrcTaskFile, pPrefs, szKey, bSilent);
}

///////////////////////////////////////////////////////////////////////

CDropBoxStorage::CDropBoxStorage()
{

}

CDropBoxStorage::~CDropBoxStorage()
{

}

BOOL CDropBoxStorage::IsDropBoxInstalled()
{
	CString sUserFolder;
	return CDropBoxStorage().IsInstalled(sUserFolder);
}

BOOL CDropBoxStorage::IsInstalled(CString& sUserFolder) const
{
	CRegKey reg;
	
	if (reg.Open(HKEY_CURRENT_USER, _T("Software\\DropBox")) == ERROR_SUCCESS)
	{
		CString sInstallPath;

		if (reg.Read(_T("InstallPath"), sInstallPath) == ERROR_SUCCESS)
		{
			if (FileMisc::FolderExists(sInstallPath))
			{
				// DropBox encrypts the user's folder location so we have a guess
				if (FileMisc::GetSpecialFolder(CSIDL_PROFILE, sUserFolder))
				{
					sUserFolder += _T("\\DropBox");

					// if that folder doesn't exist then we'll have
					// to prompt the user
					if (!FileMisc::FolderExists(sUserFolder))
						sUserFolder.Empty();
				}
			
				return TRUE;
			}
		}
	}
	
	// else
	return FALSE;
	
}

bool CDropBoxStorage::RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent)
{
	return CBaseCloudStorage::RetrieveTasklist(pFInfo, pDestTaskFile, pPrefs, szKey, bSilent);
}

bool CDropBoxStorage::StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent)
{
	return CBaseCloudStorage::StoreTasklist(pFInfo, pSrcTaskFile, pPrefs, szKey, bSilent);
}

///////////////////////////////////////////////////////////////////////
