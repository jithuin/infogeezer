#ifndef _MISCFILE_FUNCTIONS_H_
#define _MISCFILE_FUNCTIONS_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\3rdParty\stdiofileex.h"

#include <time.h>

enum 
{
	FBS_TIMESTAMP	= 0x01,
	FBS_APPVERSION	= 0x02,
};

enum FB_MODULE_SORT
{
	FBM_UNSORTED,
	FBM_SORTBY_HMODULE,
	FBM_SORTBY_FILENAME,
};

class CFileBackup
{
public:
	CFileBackup(const CString& sFile = _T(""), 
				const CString& sFolder = _T(""), 
				DWORD dwFlags = 0, 
				const CString& sExt = _T(".bak"));
	~CFileBackup();

	BOOL MakeBackup(const CString& sFile, 
					const CString& sFolder = _T(""), 
					DWORD dwFlags = 0, 
					const CString& sExt = _T(".bak"));
	BOOL RestoreBackup();

	static CString BuildBackupPath(const CString& sFile, 
									const CString& sFolder = _T(""), 
									DWORD dwFlags = 0, 
									const CString& sExt = ".bak");

protected:
	CString m_sFile, m_sBackup;
};

class CTempFileBackup : public CFileBackup
{
public:
	CTempFileBackup(const CString& sFile = _T(""), 
					const CString& sFolder = _T(""), 
					DWORD dwFlags = 0, 
					const CString& sExt = _T(".bak"));	
	~CTempFileBackup(); // auto deletes backup file

};

namespace FileMisc
{
	CString& TerminatePath(CString& sPath);
	CString& UnterminatePath(CString& sPath);
	CString TerminatePath(LPCTSTR szPath);
	CString UnterminatePath(LPCTSTR szPath);

	time_t GetFileLastModified(LPCTSTR szPath);
	BOOL GetFileLastModified(LPCTSTR szPath, FILETIME& fileTime); 
	BOOL GetFileLastModified(LPCTSTR szPath, SYSTEMTIME& sysTime, BOOL bLocalTime = true);
	BOOL ResetLastModified(LPCTSTR szPath); // resets to current time
	double GetFileSize(LPCTSTR szPath);

	BOOL RemoveFolder(LPCTSTR szFolder, 
					  HANDLE hTerminate = NULL, 
					  BOOL bProcessMsgLoop = FALSE);

	BOOL DeleteFolderContents(LPCTSTR szFolder, 
							  BOOL bIncludeSubFolders, 
							  LPCTSTR szFileMask = NULL, 
							  HANDLE hTerminate = NULL, 
							  BOOL bProcessMsgLoop = FALSE);

	double GetFolderSize(LPCTSTR szFolder, 
						 BOOL bIncludeSubFolders = TRUE, 
						 LPCTSTR szFileMask = NULL, 
						 HANDLE hTerminate = NULL, 
						 BOOL bProcessMsgLoop = FALSE);

	BOOL CreateFolder(LPCTSTR szFolder);
	BOOL CreateFolderFromFilePath(LPCTSTR szFilePath);
	BOOL FolderExists(LPCTSTR szFolder);
	BOOL FileExists(LPCTSTR szFile);
	BOOL PathExists(LPCTSTR szPath); // file or folder
	BOOL FolderFromFilePathExists(LPCTSTR szFilePath);
	BOOL PathHasWildcard(LPCTSTR szFilePath);

	CString GetCwd();
	void SetCwd(const CString& sCwd);

	int FindFiles(const CString& sFolder, CStringArray& aFiles, BOOL bCheckSubFolders = TRUE, LPCTSTR szPattern = _T("*.*"));
	BOOL FolderContainsFiles(LPCTSTR szFolder, BOOL bCheckSubFolders = TRUE, LPCTSTR szFilter = _T("*.*"));

	CString& ValidateFilename(CString& sFilename, LPCTSTR szReplace = _T(""));
	CString& ValidateFilepath(CString& sFilepath, LPCTSTR szReplace = _T(""));

	BOOL HasExtension(LPCTSTR szFilePath, LPCTSTR szExt);
	CString GetExtension(LPCTSTR szFilePath);
	CString& ReplaceExtension(CString& sFilePath, LPCTSTR szExt);
	CString& RemoveExtension(CString& sFilePath);
	
	CString GetTempFolder();
	CString GetTempFileName(LPCTSTR szPrefix, UINT uUnique = 0);
	CString GetTempFileName(LPCTSTR szFilename, LPCTSTR szExt);
	BOOL IsTempFile(LPCTSTR szFilename);

	BOOL CanOpenFile(LPCTSTR szPathname, BOOL bDenyWrite = FALSE);
	BOOL LoadFile(LPCTSTR szPathname, CString& sText, BOOL bDenyWrite = FALSE);
	int LoadFileLines(LPCTSTR szPathname, CStringArray& aLines, int nLineCount = -1);

	BOOL SaveFile(LPCTSTR szPathname, const CString& sText, SFE_SAVEAS nSaveAs = SFE_ASCOMPILED);

	BOOL ExtractResource(UINT nID, LPCTSTR szType, const CString& sTempFilePath, HINSTANCE hInst = NULL);
	BOOL ExtractResource(LPCTSTR szModulePath, UINT nID, LPCTSTR szType, const CString& sTempFilePath);

	CString GetModuleFileName(HMODULE hMod = NULL);
	CString GetModuleFolder(HMODULE hMod = NULL);
	int GetModuleDriveType(HMODULE hMod = NULL);
	CString GetModuleVersion(HMODULE hMod = NULL);
	CString GetModuleVersion(LPCTSTR szModulePath);
	BOOL GetModuleVersion(HMODULE hMod, CUIntArray& aVersionParts);
	
	CString GetAppFileName();
	CString GetAppFolder(LPCTSTR szSubFolder = NULL);
	CString GetAppResourceFolder(LPCTSTR szResFolder = _T("Resources"));

	// returns the exe and all the dlls, etc, in use by the app
	int GetAppModuleFileNames(CStringArray& aFileNames, FB_MODULE_SORT nSort = FBM_UNSORTED);
	int GetAppModuleHandles(CDWordArray& aHandles, BOOL bSorted = TRUE);
	
	CString GetAppVersion();
	BOOL GetAppVersion(CUIntArray& aVersionParts);
	BOOL SplitVersionNumber(LPCTSTR szVersion, CUIntArray& aVersionParts);
	int CompareVersions(LPCTSTR szVersion1, LPCTSTR szVersion2, int nNumItems = 0);
	int CompareVersions(CUIntArray& aVersion1, CUIntArray& aVersion2, int nNumItems = 0);

	CString GetWindowsFolder();
	CString GetWindowsSystemFolder();

	void SplitPath(LPCTSTR szPath, CString* pDrive, CString* pDir = NULL, CString* pFName = NULL, CString* pExt = NULL);
	CString& MakePath(CString& sPath, LPCTSTR szDrive, LPCTSTR szDir = NULL, LPCTSTR szFName = NULL, LPCTSTR szExt = NULL);
	
	CString GetRelativePath(const CString& sFilePath, const CString& sRelativeToFolder, BOOL bFolder);
	CString& MakeRelativePath(CString& sFilePath, const CString& sRelativeToFolder, BOOL bFolder);
	CString GetFullPath(const CString& sFilePath, const CString& sRelativeToFolder = GetCwd());
	CString& MakeFullPath(CString& sFilePath, const CString& sRelativeToFolder = GetCwd());

	CString ResolveShortcut(LPCTSTR szShortcut);
	CString GetLongPathName(LPCTSTR szShortPath);
	BOOL GetSpecialFolder(int nCSIDL, CString& sFolder, BOOL bAutoCreate = FALSE);

	BOOL IsSameFile(const CString& sFilePath1, const CString& sFilePath2);

	CString GetFolderFromFilePath(LPCTSTR szFilePath);
	CString GetFileNameFromPath(LPCTSTR szFilepath, BOOL bIncExtension = TRUE);

	// will delete the source folder on success
	BOOL MoveFolder(LPCTSTR szSrcFolder, 
					LPCTSTR szDestFolder, 
					HANDLE hTerminate = NULL, 
					BOOL bProcessMsgLoop = TRUE);

	BOOL CopyFolder(LPCTSTR szSrcFolder, 
					LPCTSTR szDestFolder, 
					HANDLE hTerminate = NULL, 
					BOOL bProcessMsgLoop = TRUE);

	// will delete the source folder only if file mask was "*.*"
	BOOL MoveFolder(LPCTSTR szSrcFolder, 
					LPCTSTR szDestFolder, 
					BOOL bIncludeSubFolders, 
					LPCTSTR szFileMask, 
					HANDLE hTerminate = NULL, 
					BOOL bProcessMsgLoop = TRUE);

	BOOL CopyFolder(LPCTSTR szSrcFolder, 
					LPCTSTR szDestFolder, 
					BOOL bIncludeSubFolders, 
					LPCTSTR szFileMask, 
					HANDLE hTerminate = NULL, 
					BOOL bProcessMsgLoop = TRUE);


	BOOL IsLoggingEnabled();
	void EnableLogging(BOOL bEnable = TRUE, BOOL bResetLogFile = TRUE, BOOL bWantDateTime = TRUE);
	void LogTimeElapsed(DWORD& dwTickStart, LPCTSTR szFormat, ...);
	void LogText(LPCTSTR szText, ...);
	void LogAppModuleState(FB_MODULE_SORT nSort = FBM_UNSORTED);

	// append a line of text to a text file
	BOOL AppendLineToFile(LPCTSTR szPathname, const CString& sLine, SFE_SAVEAS nSaveAs = SFE_ASIS);

	DWORD Run(HWND hwnd, LPCTSTR lpFile, LPCTSTR lpDirectory = NULL, int nShowCmd = SW_SHOW);

	int GetDropFilePaths(HDROP hDrop, CStringArray& aFiles);
	int GetDroppedFilePaths(COleDataObject* pObject, CStringArray& aFiles);

}

#endif // _MISCFILE_FUNCTIONS_H_