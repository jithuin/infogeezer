// IImportExport.h: interface and implementation of the IImportExport class.
//
/////////////////////////////////////////////////////////////////////////////////

#if !defined(AFX_ITASKLISTSTORAGE_H__12345678_BA15_4851_A41B_2B4EC1DC12D5__INCLUDED_)
#define AFX_ITASKLISTSTORAGE_H__12345678_BA15_4851_A41B_2B4EC1DC12D5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <Windows.h>

// function to be exported from dll to create instance of interface
#ifdef _EXPORTING // declare this in project settings for dll _only_
#	define DLL_DECLSPEC __declspec(dllexport)
#else
#	define DLL_DECLSPEC __declspec(dllimport)
#endif 

#define ITASKLISTSTORAGE_VERSION 0x0000

//////////////////////////////////////////////////////////////////////

class ITasklistStorage;
class ITaskList;
class IPreferences;
class ITransText;

//////////////////////////////////////////////////////////////////////

typedef ITasklistStorage* (*PFNCREATEHANDLER)(); // function prototype
extern "C" DLL_DECLSPEC ITasklistStorage* CreateTasklistStorageInterface();

typedef int (*PFNGETVERSION)(); // function prototype
extern "C" DLL_DECLSPEC int GetInterfaceVersion();

//////////////////////////////////////////////////////////////////////

#pragma warning(disable:4505)

// helper method
static ITasklistStorage* CreateTasklistStorageInterface(LPCTSTR szDllPath)
{
    ITasklistStorage* pInterface = NULL;
    HMODULE hDll = LoadLibrary(szDllPath);
	
    if (hDll)
    {
        PFNCREATEHANDLER pCreate = (PFNCREATEHANDLER)GetProcAddress(hDll, "CreateTasklistStorageInterface");
		
        if (pCreate)
		{
			// check version
			if (!ITASKLISTSTORAGE_VERSION)
				pInterface = pCreate();
			else
			{
				PFNGETVERSION pVersion = (PFNGETVERSION)GetProcAddress(hDll, "GetInterfaceVersion");

				if (pVersion && pVersion() >= ITASKLISTSTORAGE_VERSION)
					pInterface = pCreate();
			}
		}

		if (hDll && !pInterface)
			FreeLibrary(hDll);
    }
	
    return pInterface;
}

static BOOL IsTasklistStorageDll(LPCTSTR szDllPath)
{
    HMODULE hDll = LoadLibrary(szDllPath);
	
    if (hDll)
    {
		PFNCREATEHANDLER pCreate = (PFNCREATEHANDLER)GetProcAddress(hDll, "CreateTasklistStorageInterface");
		FreeLibrary(hDll);

		return (pCreate != NULL);
	}

	return FALSE;
}

//////////////////////////////////////////////////////////////////////

const int TASKLISTID_LEN = 1024;
const int PASSWORD_LEN = 32;

struct ITS_TASKLISTINFO
{
	TCHAR szTasklistID[TASKLISTID_LEN+1];
	TCHAR szLocalFileName[_MAX_PATH+1];
	TCHAR szDisplayName[_MAX_PATH+1];
	TCHAR szPassword[PASSWORD_LEN+1];
};

//////////////////////////////////////////////////////////////////////

class ITasklistStorage
{
public:
    virtual void Release() = 0; // releases the interface

	// caller must copy result only
	virtual LPCTSTR GetMenuText() const = 0;
	virtual LPCTSTR GetTypeID() const = 0;

	virtual void SetLocalizer(ITransText* pTT) = 0;

	virtual bool RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent = FALSE) = 0;
	virtual bool StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent = FALSE) = 0;

};

//////////////////////////////////////////////////////////////////////

static void ReleaseStorageInterface(ITasklistStorage*& pInterface)
{
    if (pInterface)
    {
        pInterface->Release();
        pInterface = NULL;
    }
}

//////////////////////////////////////////////////////////////////////

#endif // !defined(AFX_ITasklistSTORAGE_H__12345678_BA15_4851_A41B_2B4EC1DC12D5__INCLUDED_)
