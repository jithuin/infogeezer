// ISpellChecker.h: interface for the ISpellChecker class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_ISPELLCHECK_H__7741547B_BA15_4851_A41B_2B4EC1DC12D5__INCLUDED_)
#define AFX_ISPELLCHECK_H__7741547B_BA15_4851_A41B_2B4EC1DC12D5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#pragma warning(disable:4189)
#pragma warning(disable:4127)

#include <Windows.h>

// function to be exported from dll to create instance of interface
#ifdef _EXPORTING // declare this in project settings for dll _only_
#	define DLL_DECLSPEC __declspec(dllexport)
#else
#	define DLL_DECLSPEC __declspec(dllimport)
#endif 

#define ISPELLCHECK_VERSION 0x0000

class ISpellChecker;

typedef ISpellChecker* (*PFNCREATE)(LPCTSTR, LPCTSTR); // function prototype
extern "C" DLL_DECLSPEC ISpellChecker* CreateSpellCheckerInterface(LPCTSTR szAffPath, LPCTSTR szDicPath); // single exported function

typedef int (*PFNGETVERSION)(); // function prototype
extern "C" DLL_DECLSPEC int GetInterfaceVersion();

#pragma warning(disable:4505)

// helper method
static ISpellChecker* CreateSpellCheckerInterface(LPCTSTR szDllPath, LPCTSTR szAffPath, LPCTSTR szDicPath)
{
    ISpellChecker* pInterface = NULL;
    HMODULE hDll = LoadLibrary(szDllPath);
	
    if (hDll)
    {
        PFNCREATE pCreate = (PFNCREATE)GetProcAddress(hDll, "CreateSpellCheckerInterface");
		
        if (pCreate)
		{
			if (!ISPELLCHECK_VERSION)
				pInterface = pCreate(szAffPath, szDicPath);
			else
			{
				// check version
				PFNGETVERSION pVersion = (PFNGETVERSION)GetProcAddress(hDll, "GetInterfaceVersion");
				
				if (pVersion && pVersion() >= ISPELLCHECK_VERSION)
					pInterface = pCreate(szAffPath, szDicPath);
			}
		}
    }
	
    return pInterface;
}

static BOOL IsSpellCheckDll(LPCTSTR szDllPath)
{
    HMODULE hDll = LoadLibrary(szDllPath);
	
    if (hDll)
    {
        PFNCREATE pCreate = (PFNCREATE)GetProcAddress(hDll, "CreateSpellCheckerInterface");
		FreeLibrary(hDll);

		return (NULL != pCreate);
	}

	return FALSE;
}

class ISpellChecker
{
public:
    virtual void Release() = 0; // releases the interface
	
    // 
    virtual bool CheckSpelling(LPCTSTR szWord) = 0;
    virtual bool CheckSpelling(LPCTSTR szWord, LPTSTR*& pSuggestions, int& nNumSuggestions) = 0;
	
    // frees a previously returned buffer and sets the ptr to NULL
    virtual void FreeSuggestions(LPTSTR*& pSuggestions, int nNumSuggestions) = 0;
	
};

static void ReleaseSpellCheckerInterface(ISpellChecker*& pInterface)
{
    if (pInterface)
    {
        pInterface->Release();
        pInterface = NULL;
    }
}

class ISpellCheck
{
public:
	virtual LPCTSTR GetFirstWord() const = 0;
	virtual LPCTSTR GetNextWord() const = 0;
	virtual LPCTSTR GetCurrentWord() const = 0;

	virtual void SelectCurrentWord() = 0;
	virtual void ReplaceCurrentWord(LPCTSTR szWord) = 0;

	virtual void ClearSelection() = 0;
};

#endif // !defined(AFX_ISpellCheck_H__7741547B_BA15_4851_A41B_2B4EC1DC12D5__INCLUDED_)
