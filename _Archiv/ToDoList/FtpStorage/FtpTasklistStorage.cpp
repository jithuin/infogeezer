// FtpTasklistStorage.cpp: implementation of the CFtpTasklistStorage class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "FtpStorage.h"
#include "FtpTasklistStorage.h"

#include "..\shared\RemoteFile.h"
#include "..\shared\misc.h"
#include "..\shared\filemisc.h"
#include "..\shared\enstring.h"
#include "..\shared\localizer.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CFtpTasklistStorage::CFtpTasklistStorage()
{

}

CFtpTasklistStorage::~CFtpTasklistStorage()
{

}

bool CFtpTasklistStorage::RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, 
										   IPreferences* pPrefs, LPCTSTR szKey, bool bSilent)
{
	CString sLocalPath(pFInfo->szLocalFileName);

	if (sLocalPath.IsEmpty())
		sLocalPath = FileMisc::GetTempFolder();

	// split the tasklist ID into it constituent parts
	CStringArray aIDParts;
	CString sRemotePath;
	CRemoteFile rf(GetMenuText());
	
	if (Misc::Split(pFInfo->szTasklistID, aIDParts, TRUE, _T("::")) == 3)
	{
		rf.SetServer(aIDParts[0]);
		sRemotePath = aIDParts[1];
		rf.SetUsername(aIDParts[2]);

		// only set the password if the other info was okay
		rf.SetPassword(pFInfo->szPassword);
	}

	DWORD dwOptions = RMO_CREATEDOWNLOADDIR | RMO_USETEMPFILE | RMO_KEEPFILENAME;
	
	if (!bSilent)
		dwOptions |= RMO_ALLOWDIALOG;

	CString sKey(szKey);
	sKey += _T("\\") + CEnString(AFX_IDS_APP_TITLE);
	
	if (rf.GetFile(sRemotePath, sLocalPath, pPrefs, sKey, dwOptions, CEnString(IDS_TDLFILEFILTER)) == RMERR_SUCCESS)
	{
		// return information to caller 
		_tcsncpy(pFInfo->szLocalFileName, sLocalPath, _MAX_PATH);
		_tcsncpy(pFInfo->szDisplayName, rf.GetServer() + sRemotePath, _MAX_PATH);
		_tcsncpy(pFInfo->szPassword, rf.GetPassword(), PASSWORD_LEN);

		CString sTaskID = rf.GetServer() + _T("::") + sRemotePath + _T("::") + rf.GetUsername();
		_tcsncpy(pFInfo->szTasklistID, sTaskID, TASKLISTID_LEN);

		return true;
	}

	return false;
}

bool CFtpTasklistStorage::StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, 
										IPreferences* pPrefs, LPCTSTR szKey, bool bSilent)
{
	CString sLocalPath = pFInfo->szLocalFileName;

	// if not yet saved then save to temp filepath
	if (sLocalPath.IsEmpty())
		sLocalPath = FileMisc::GetTempFileName(_T("rmf"));

	CRemoteFile rf;
	DWORD dwOptions = RMO_NOCANCELPROGRESS;

	if (bSilent)
		dwOptions |= RMO_NOPROGRESS;
	else
		dwOptions |= RMO_ALLOWDIALOG;

	// split the tasklist ID into it constituent parts
	CStringArray aIDParts;
	CString sRemotePath;
	
	if (Misc::Split(pFInfo->szTasklistID, aIDParts, TRUE, _T("::")) == 3)
	{
		rf.SetServer(aIDParts[0]);
		sRemotePath = aIDParts[1];
		rf.SetUsername(aIDParts[2]);

		// only set the password if the other info was okay
		rf.SetPassword(pFInfo->szPassword);
	}

	CString sKey(szKey);
	sKey += _T("\\") + CEnString(AFX_IDS_APP_TITLE);

	return (rf.SetFile(sLocalPath, sRemotePath, pPrefs, sKey, dwOptions) == RMERR_SUCCESS);
}

void CFtpTasklistStorage::SetLocalizer(ITransText* pTT)
{
	CLocalizer::Initialize(pTT);
}

