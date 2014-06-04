// MySpellCheck.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "..\shared\ispellcheck.h"
#include "..\shared\spellchecker.h"

BOOL APIENTRY DllMain( HANDLE /*hModule*/, 
                       DWORD  /*ul_reason_for_call*/, 
                       LPVOID /*lpReserved*/
					 )
{
    return TRUE;
}

DLL_DECLSPEC ISpellChecker* CreateSpellCheckerInterface(LPCTSTR szAffPath, LPCTSTR szDicPath)
{
	// some simple checks first
	if (GetFileAttributes(szDicPath) == 0xffffffff)
		return NULL;

	CSpellChecker* pSC = new CSpellChecker(szAffPath, szDicPath);

	if (!pSC->IsValid())
	{
		delete pSC;
		pSC = NULL;
	}

	return pSC;
}

