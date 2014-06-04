// Misc.cpp: implementation of the CMisc class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "Misc.h"
#include "enstring.h"

#include <Lmcons.h>
#include <math.h>
#include <locale.h>
#include <afxtempl.h>
#include <atlconv.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

CString Misc::FormatGetLastError(DWORD dwLastErr)
{
	if (dwLastErr == -1)
		dwLastErr = GetLastError();

	LPTSTR lpMessage;
	DWORD dwErrCode = GetLastError();
	FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER |
				   FORMAT_MESSAGE_FROM_SYSTEM,
				   NULL,               // no source buffer needed
				   dwErrCode,          // error code for this message
				   NULL,               // default language ID
				   (LPTSTR)&lpMessage, // allocated by fcn
				   NULL,               // minimum size of buffer
				   NULL);              // no inserts


	CString sError(lpMessage);
	LocalFree(lpMessage);

	return sError;
}

BOOL Misc::CopyTexttoClipboard(const CString& sText, HWND hwnd, UINT nFormat, BOOL bClear) 
{
	if (!::OpenClipboard(hwnd)) 
		return FALSE; 
	
	HGLOBAL hglbCopy = NULL; 
	BOOL bResult = FALSE;

	try
	{
		if (bClear)
			::EmptyClipboard(); 
		
		// Allocate a global memory object for the text. 
		hglbCopy = ::GlobalAlloc(GMEM_MOVEABLE, (sText.GetLength() + 1) * sizeof(TCHAR)); 
		
		if (hglbCopy) 
		{ 
			// Lock the handle and copy the text to the buffer. 
			LPTSTR lptstrCopy = (LPTSTR)::GlobalLock(hglbCopy); 
			memcpy(lptstrCopy, (LPVOID)(LPCTSTR)sText, sText.GetLength() * sizeof(TCHAR)); 
			lptstrCopy[sText.GetLength()] = (TCHAR) 0;    // null character 

			::GlobalUnlock(hglbCopy); 
			
			// Place the handle on the clipboard. 
			if (nFormat == 0)
			{
#ifndef _UNICODE
				nFormat = CF_TEXT;
#else
				nFormat = CF_UNICODETEXT;
#endif
			}
			
			if (::SetClipboardData(nFormat, hglbCopy) == NULL)
				::GlobalFree(hglbCopy);			

			::CloseClipboard();
			hglbCopy = NULL;

			bResult = TRUE;
		}
	}
	catch(...)
	{
		::CloseClipboard();

		if (hglbCopy)
			::GlobalFree(hglbCopy);
	}

	return bResult;
}

CString Misc::GetClipboardText(HWND hwnd, UINT nFormat)
{
	if (!::OpenClipboard(hwnd)) 
		return ""; 

	CString sText;

	try
	{
		if (nFormat == 0)
		{
#ifndef _UNICODE
			nFormat = CF_TEXT;
#else
			nFormat = CF_UNICODETEXT;
#endif
		}
		
		HANDLE hData = ::GetClipboardData(nFormat);

		sText = (LPCTSTR)GlobalLock(hData);
		::GlobalUnlock(hData);

	}
	catch(...)
	{
		sText.Empty();
	}

	::CloseClipboard(); // always

	return sText;
}

BOOL Misc::ClipboardHasFormat(UINT nFormat, HWND hwnd)
{
	if (!::OpenClipboard(hwnd))
		return FALSE;

	UINT nFmt = 0;
	try
	{
		nFmt = ::EnumClipboardFormats(0);

		while (nFmt) 
		{
			if (nFmt == nFormat)
				break;

			nFmt = ::EnumClipboardFormats(nFmt);
		}
	}
	catch (...)
	{
		nFmt = 0;
	}

	::CloseClipboard(); // always

	return (nFormat && nFmt == nFormat);
}

int Misc::FilterString(CString& sText, const CString& sFilter)
{
	int nPos = sText.GetLength();

	while (nPos--)
	{
		if (sFilter.Find(sText[nPos]) == -1)
			sText.Delete(nPos);
	}

	return sText.GetLength();
}

BOOL Misc::IsGuid(LPCTSTR szGuid)
{
	GUID guid;

	return GuidFromString(szGuid, guid);
}

BOOL Misc::GuidFromString(LPCTSTR szGuid, GUID& guid)
{
#pragma comment(lib, "Rpcrt4.lib")

#ifdef _UNICODE
	RPC_STATUS rpcs = UuidFromString((unsigned short*)(LPCTSTR)szGuid, &guid);
#else
	RPC_STATUS rpcs = UuidFromString((unsigned char*)(LPCTSTR)szGuid, &guid);
#endif

	if (rpcs != RPC_S_OK)
	{
		NullGuid(guid);
		return FALSE;
	}

	return TRUE;
}

BOOL Misc::GuidToString(const GUID& guid, CString& sGuid)
{
#pragma comment(lib, "Rpcrt4.lib")

	LPTSTR pszGuid;
	
#ifdef _UNICODE
	if (RPC_S_OK == UuidToString((GUID*)&guid, (unsigned short**)&pszGuid))
#else
	if (RPC_S_OK == UuidToString((GUID*)&guid, (unsigned char**)&pszGuid))
#endif
		sGuid = CString(pszGuid);
	else
		sGuid.Empty();
	
#ifdef _UNICODE
	RpcStringFree((unsigned short**)&pszGuid);
#else
	RpcStringFree((unsigned char**)&pszGuid);
#endif
	
	return !sGuid.IsEmpty();
}

BOOL Misc::GuidIsNull(const GUID& guid)
{
	static GUID NULLGUID = { 0 };
	
	return SameGuids(guid, NULLGUID);
}

BOOL Misc::SameGuids(const GUID& guid1, const GUID& guid2)
{
	return (memcmp(&guid1, &guid2, sizeof(GUID)) == 0);
}

void Misc::NullGuid(GUID& guid)
{
   ZeroMemory(&guid, sizeof(guid));
}

void Misc::ProcessMsgLoop()
{
	MSG msg;

	while (::PeekMessage((LPMSG) &msg, NULL, 0, 0, PM_REMOVE))
	{
		if (::IsDialogMessage(msg.hwnd, (LPMSG)&msg))
		{
			// do nothing - its already been done
		}
		else
		{
			::TranslateMessage(&msg);
			::DispatchMessage(&msg);
		}
	}
}

CString Misc::FormatComputerNameAndUser(char cSeparator)
{
	static CString sMachineAndUser;
	
	if (sMachineAndUser.IsEmpty())
		sMachineAndUser.Format(_T("%s%c%s"), GetComputerName(), cSeparator, GetUserName());

	return sMachineAndUser;
}

CString Misc::GetComputerName()
{
	static CString sMachine;

	if (sMachine.IsEmpty()) // init first time only
	{
		DWORD LEN = MAX_COMPUTERNAME_LENGTH + 1;
		
		::GetComputerName(sMachine.GetBuffer(LEN), &LEN);
		sMachine.ReleaseBuffer();
	}

	return sMachine;
}

CString Misc::GetUserName()
{
	static CString sUser;

	if (sUser.IsEmpty()) // init first time only
	{
		DWORD LEN = UNLEN + 1;
		
		::GetUserName(sUser.GetBuffer(LEN), &LEN);
		sUser.ReleaseBuffer();
	}

	return sUser;
}

CString Misc::GetListSeparator()
{
	static CString sSep;
	const int BUFLEN = 10;
	
	if (sSep.IsEmpty()) // init first time only
	{
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_SLIST, sSep.GetBuffer(BUFLEN), BUFLEN - 1);
		sSep.ReleaseBuffer();
		
		// Trim extra spaces
		sSep.TrimLeft();
		sSep.TrimRight();
		
		// If none found, use a comma
		if (!sSep.GetLength())
			sSep = ',';
	}

	return sSep;
}

CString Misc::FormatArray(const CStringArray& array, LPCTSTR szSep)
{
	int nCount = array.GetSize();

	if (nCount == 0)
		return "";

	CString sSep(szSep);

	if (!szSep)
		sSep = GetListSeparator() + ' ';

	CString sText;

	for (int nItem = 0; nItem < nCount; nItem++)
	{
		if (nItem > 0 && array[nItem].GetLength())
			sText += sSep;

		sText += array[nItem];
	}

	return sText;
}

CString Misc::FormatArray(const CDWordArray& array, LPCTSTR szSep)
{
	int nCount = array.GetSize();

	if (nCount == 0)
		return "";

	CString sSep(szSep);

	if (!szSep)
		sSep = GetListSeparator() + ' ';

	CString sText;

	for (int nItem = 0; nItem < nCount; nItem++)
	{
		CString sItem;
		sItem.Format(_T("%ld"), array[nItem]);

		if (nItem > 0)
			sText += sSep;

		sText += sItem;
	}

	return sText;
}

void Misc::Trace(const CStringArray& array)
{
	int nCount = array.GetSize();

	for (int nItem = 0; nItem < nCount; nItem++)
		TRACE(_T("%s, "), array[nItem]);

	TRACE(_T("\n"));
}

int Misc::GetTotalLength(const CStringArray& array)
{
	int nLength = 0;
	int nItem = array.GetSize();

	while (nItem--)
		nLength += array.GetData()[nItem].GetLength();

	return nLength;
}

int Misc::Split(const CString& sText, char cDelim, CStringArray& aValues)
{
	return Split(sText, aValues, TRUE, CString(cDelim));
}

int Misc::Split(const CString& sText, CStringArray& aValues, BOOL bAllowEmpty, const CString& sSep)
{
	ASSERT (!sSep.IsEmpty());

	aValues.RemoveAll();

	// parse on separator unless enclosed in double-quotes
	int nLen = sText.GetLength(), nSepLen = sSep.GetLength();
	BOOL bInQuotes = FALSE, bAddWord = FALSE;
	CString sWord;

	for (int nPos = 0; nPos < nLen; nPos++)
	{
		TCHAR chr = sText[nPos];

		// check if we're at the beginning of a separator string
		if (chr == sSep[0] && (nSepLen == 1 || sText.Find(sSep, nPos) == nPos))
		{
			if (bInQuotes)
				sWord += sSep;
			else
				bAddWord = TRUE;

			nPos += nSepLen - 1; // minus 1 because the loop also increments
		}
		else if (chr == '\"') // double quote
		{
			// flip bInQuotes
			bInQuotes = !bInQuotes;

			// Workaround to handle Quoted empty string as last word
			if (!bInQuotes && bAllowEmpty && sWord.IsEmpty())
				sWord = ' ';
		}
		else // everything else
		{
			sWord += chr;

			// also if its the last char then add it
			bAddWord = (nPos == nLen - 1);
		}

		if (bAddWord)
		{
			sWord.TrimLeft();
			sWord.TrimRight();
			
			if (bAllowEmpty || !sWord.IsEmpty())
				aValues.Add(sWord);
			
			sWord.Empty(); // next word
			bAddWord = FALSE;
		}
	}

	// if the string ends with a separator and allow empty then add an empty string
	if (!sWord.IsEmpty())
	{
		sWord.TrimLeft();
		sWord.TrimRight();
			
		aValues.Add(sWord);
	}
	else if (bAllowEmpty && nLen)
	{
		if (sText.Find(sSep, nLen - nSepLen) == (nLen - nSepLen))
			aValues.Add(sWord);
	}

	return aValues.GetSize();
}

BOOL Misc::MatchAll(const CStringArray& array1, const CStringArray& array2, BOOL bOrderSensitive, BOOL bCaseSensitive)
{
	int nSize1 = array1.GetSize();
	int nSize2 = array2.GetSize();

	if (nSize1 != nSize2)
		return 0;

	if (bOrderSensitive)
	{
		for (int nItem1 = 0; nItem1 < nSize1; nItem1++)
		{
			// check for non-equality
			if (bCaseSensitive)
			{
				if (array1[nItem1] != array2[nItem1])
					return FALSE;
			}
			else 
			{
				if (NaturalCompare(array1[nItem1], array2[nItem1]) != 0)
					return FALSE;
			}
		}

		return TRUE;
	}

	// else order not important
	for (int nItem1 = 0; nItem1 < nSize1; nItem1++)
	{
		// look for matching item in array2
		if (Find(array2, array1[nItem1], bCaseSensitive, FALSE) == -1)
		{
			// no-match found == not the same
			return FALSE;
		}
	}

	return TRUE;
}

BOOL Misc::MatchAny(const CStringArray& array1, const CStringArray& array2, BOOL bCaseSensitive, BOOL bPartialOK) 
{
	int nSize1 = array1.GetSize();

	for (int nItem1 = 0; nItem1 < nSize1; nItem1++)
	{
		// look for matching item
		if (Find(array2, array1[nItem1], bCaseSensitive, bPartialOK) != -1)
			return TRUE;
	}
	
	return FALSE;
}

int Misc::Find(const CStringArray& array, LPCTSTR szItem, BOOL bCaseSensitive, BOOL bPartialOK)
{
	ASSERT (szItem);

	if (szItem == NULL)
		return -1;

	int nSize = array.GetSize();

	for (int nItem = 0; nItem < nSize; nItem++)
	{
		// look for matching item
		CString sArrItem = array[nItem];

		// special case: empty item
		if (szItem[0] == 0)
		{
			if (sArrItem.IsEmpty())
				return nItem;
		}
		else if (bCaseSensitive)
		{
			if (bPartialOK)
			{
				if (sArrItem.Find(szItem) != -1)
					return nItem;
			}
			else
			{
				if (sArrItem == szItem)
					return nItem;
			}
		}
		else // case sensitive
		{
			if (bPartialOK)
			{
				if (FindWord(szItem, sArrItem, TRUE, FALSE))
					return nItem;
			}
			else
			{
				if (NaturalCompare(sArrItem, szItem) == 0)
					return nItem;
			}
		}
	}

	return -1;
}

int Misc::Find(const CDWordArray& array, DWORD dwItem)
{
	int nSize = array.GetSize();

	for (int nItem = 0; nItem < nSize; nItem++)
	{
		if (array[nItem] == dwItem)
			return nItem;
	}

	return -1;
}

int Misc::Find(const CUIntArray& array, UINT uItem)
{
	int nSize = array.GetSize();

	for (int nItem = 0; nItem < nSize; nItem++)
	{
		if (array[nItem] == uItem)
			return nItem;
	}

	return -1;
}

int Misc::RemoveItems(const CStringArray& aItems, CStringArray& aFrom, BOOL bCaseSensitive)
{
	int nRemoved = 0; // counter
	int nItem = aItems.GetSize();

	while (nItem--)
	{
		if (RemoveItem(aItems[nItem], aFrom, bCaseSensitive))
			nRemoved++;
	}

	return nRemoved;
}

BOOL Misc::RemoveItem(LPCTSTR szItem, CStringArray& aFrom, BOOL bCaseSensitive)
{
	int nFind = Find(aFrom, szItem, bCaseSensitive);
	
	if (nFind != -1)
		aFrom.RemoveAt(nFind);

	return (nFind != -1);
}

int Misc::AddUniqueItems(const CStringArray& aItems, CStringArray& aTo, BOOL bCaseSensitive)
{
	int nAdded = 0; // counter
	int nItem = aItems.GetSize();

	while (nItem--)
	{
		if (AddUniqueItem(aItems[nItem], aTo, bCaseSensitive))
			nAdded++;
	}

	return nAdded;
}

BOOL Misc::AddUniqueItem(const CString& sItem, CStringArray& aTo, BOOL bCaseSensitive)
{
	if (sItem.IsEmpty())
		return FALSE;

	int nFind = Find(aTo, sItem, bCaseSensitive);

	if (nFind == -1) // doesn't already exist
	{
		aTo.Add(sItem);
		return TRUE;
	}

	return FALSE; // not added
}

CString Misc::GetAM()
{
	static CString sAM;
	const int BUFLEN = 10;

	if (sAM.IsEmpty()) // init first time only
	{
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_S1159, sAM.GetBuffer(BUFLEN), BUFLEN - 1);
		sAM.ReleaseBuffer();
	}

	return sAM;
}

CString Misc::GetPM()
{
	static CString sPM;
	const int BUFLEN = 10;

	if (sPM.IsEmpty()) // init first time only
	{
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_S2359, sPM.GetBuffer(BUFLEN), BUFLEN - 1);
		sPM.ReleaseBuffer();
	}

	return sPM;
}

CString Misc::GetTimeFormat(BOOL bIncSeconds)
{
	static CString sFormat;
	const int BUFLEN = 100;

	if (sFormat.IsEmpty()) // init first time only
	{
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_STIMEFORMAT, sFormat.GetBuffer(BUFLEN), BUFLEN - 1);
		sFormat.ReleaseBuffer();
	}

	if (!bIncSeconds)
	{
		CString sTemp(sFormat);

		if (!sTemp.Replace(GetTimeSeparator() + "ss", _T("")))
			sTemp.Replace(GetTimeSeparator() + "s", _T(""));

		return sTemp;
	}

	return sFormat;
}

CString Misc::GetTimeSeparator()
{
	static CString sSep;
	const int BUFLEN = 10;

	if (sSep.IsEmpty()) // init first time only
	{
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_STIME , sSep.GetBuffer(BUFLEN), BUFLEN - 1);
		sSep.ReleaseBuffer();

		// Trim extra spaces
		sSep.TrimLeft();
		sSep.TrimRight();
		
		// If none found, use a dot
		if (!sSep.GetLength())
			sSep = ":";
	}

	return sSep;
}

CString Misc::GetDateSeparator()
{
	static CString sSep;
	const int BUFLEN = 10;

	if (sSep.IsEmpty()) // init first time only
	{
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_SDATE , sSep.GetBuffer(BUFLEN), BUFLEN - 1);
		sSep.ReleaseBuffer();

		// Trim extra spaces
		sSep.TrimLeft();
		sSep.TrimRight();
		
		// If none found, use a slash
		if (!sSep.GetLength())
			sSep = "/";
	}

	return sSep;
}

CString Misc::GetShortDateFormat(BOOL bIncDOW)
{
	static CString sFormat;
	const int BUFLEN = 100;

	if (sFormat.IsEmpty()) // init first time only
	{
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_SSHORTDATE, sFormat.GetBuffer(BUFLEN), BUFLEN - 1);
		sFormat.ReleaseBuffer();
	}

	if (bIncDOW)
	{
		CString sTemp = "ddd "+ sFormat;
		return sTemp;
	}

	return sFormat;
}

CString Misc::GetDecimalSeparator()
{
	static CString sSep;
	const int BUFLEN = 10;

	if (sSep.IsEmpty()) // init first time only
	{
		GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_SDECIMAL, sSep.GetBuffer(BUFLEN), BUFLEN - 1);
		sSep.ReleaseBuffer();

		// Trim extra spaces
		sSep.TrimLeft();
		sSep.TrimRight();
		
		// If none found, use a dot
		if (!sSep.GetLength())
			sSep = ".";
	}

	return sSep;
}

BOOL Misc::IsMetricMeasurementSystem()
{
	const int BUFLEN = 2;
	CString sSystem;
	
	GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_IMEASURE, sSystem.GetBuffer(BUFLEN), BUFLEN - 1);
	sSystem.ReleaseBuffer(BUFLEN - 1);
	
	return (_ttoi(sSystem) == 0);
}

BOOL Misc::IsMultibyteString(const CString& sText)
{
#ifndef _UNICODE
	int nChar = sText.GetLength();

	while (nChar--)
	{
		if (IsDBCSLeadByte(sText[nChar]))
			return TRUE;
	}
#else
	UNREFERENCED_PARAMETER(sText);
#endif

	return FALSE;
}

char* Misc::WideToMultiByte(const WCHAR* szFrom, UINT nCodePage)
{
	int nLen = wcslen(szFrom);
	return WideToMultiByte(szFrom, nLen, nCodePage);
}

char* Misc::WideToMultiByte(const WCHAR* szFrom, int& nLength, UINT nCodePage)
{
	int clen = WideCharToMultiByte(nCodePage, 0, szFrom, nLength, NULL, 0, NULL, NULL);

	// if clen == 0, some unknown codepage was probably used.
	ASSERT((nLength && clen) || (!nLength && !clen));

	if (clen == 0 && nLength) 
		return NULL;

	char* cTo = new char[clen+1];
	cTo[clen] = 0; // null terminator

	if (clen)
		WideCharToMultiByte(nCodePage, 0, szFrom, nLength, cTo, clen, NULL, NULL);

	nLength = clen;

	return cTo;
}

WCHAR* Misc::MultiByteToWide(const char* szFrom, UINT nCodePage)
{
	int nLen = strlen(szFrom);
	return MultiByteToWide(szFrom, nLen, nCodePage);
}

WCHAR* Misc::MultiByteToWide(const char* szFrom, int& nLength, UINT nCodepage)
{
	// Use api convert routine
	int wlen = MultiByteToWideChar(nCodepage, 0, szFrom, nLength, NULL, 0);

	// if wlen == 0, some unknown codepage was probably used.
	ASSERT((nLength && wlen) || (!nLength && !wlen));

	if (wlen == 0 && nLength) 
		return NULL;

	WCHAR* wTo = new WCHAR[wlen+1];
	wTo[wlen] = 0; // null terminator

	if (wlen)
		MultiByteToWideChar(nCodepage, 0, szFrom, nLength, wTo, wlen);

	nLength = wlen;

	return wTo;
}

double Misc::Round(double dValue)
{
	if (dValue > 0)
	{
		if ((dValue - (int)dValue) > 0.5)
			return ceil(dValue);
		else
			return floor(dValue);
	}
	else
	{
		if ((dValue - (int)dValue) > -0.5)
			return floor(dValue);
		else
			return ceil(dValue);
	}
}

float Misc::Round(float fValue)
{
	return (float)Round((double)fValue);
}

BOOL Misc::IsNumber(const CString& sValue)
{
	if (sValue.IsEmpty())
		return FALSE;

	static const CString DELIMS(_T("+-."));

	for (int nChar = 0; nChar < sValue.GetLength(); nChar++)
	{
		TCHAR chr = sValue[nChar];

		if (!_istdigit(chr) && DELIMS.Find(chr) == -1)
			return FALSE;
	}

	return TRUE;
}

BOOL Misc::IsSymbol(const CString& sValue)
{
	if (sValue.IsEmpty())
		return FALSE;

	for (int nChar = 0; nChar < sValue.GetLength(); nChar++)
	{
		TCHAR chr = sValue[nChar];

		if (_istalnum(chr)) // alpha-Numeric
			return FALSE;
	}

	return TRUE;
}

const CString& Misc::GetLongest(const CString& str1, const CString& str2, BOOL bAsDouble)
{
	if (bAsDouble)
	{
		// check as numbers
		double dStr1 = fabs(Atof(str1)), dStr2 = fabs(Atof(str2));

		return (dStr1 > dStr2) ? str1 : str2;
	}

	// all else
	return (str1.GetLength() > str2.GetLength()) ? str1 : str2;
}

double Misc::Atof(const CString& sValue)
{
	// needs special care to handle decimal point properly
	// especially since we've no way of knowing how it is encoded.
	// so we assume that if a period is present then it's encoded
	// in 'english' else it's in native format
	char* szLocale = _strdup(setlocale(LC_NUMERIC, NULL));
	
	if (sValue.Find('.') != -1)
		setlocale(LC_NUMERIC, "English");
	else
		setlocale(LC_NUMERIC, "");

	double dVal = _tcstod(sValue, NULL);

	// restore locale
	setlocale(LC_NUMERIC, szLocale);
	free(szLocale);
	
	return dVal; 
}

BOOL Misc::ShutdownBlockReasonCreate(HWND hWnd, LPCTSTR szReason)
{
	typedef BOOL (WINAPI *PFNSHUTDOWNBLOCKREASONCREATE)(HWND, LPCWSTR);

	// load user32.dll once only
	static HMODULE hUser32 = LoadLibrary(_T("user32.dll"));

	if (hUser32)
	{
		static PFNSHUTDOWNBLOCKREASONCREATE fnCreate = (PFNSHUTDOWNBLOCKREASONCREATE)GetProcAddress(hUser32, "ShutdownBlockReasonCreate");

		if (fnCreate)
		{
#ifdef _UNICODE
			LPCWSTR wszReason = szReason;
#else
			int nLength = lstrlen(szReason);
			LPWSTR wszReason = MultiByteToWide(szReason, nLength);
#endif

			BOOL bRes = fnCreate(hWnd, wszReason);

#ifndef _UNICODE
			delete [] wszReason;
#endif
			return bRes;
		}
	}

	// must be < XP
	return FALSE;
}

BOOL Misc::ShutdownBlockReasonDestroy(HWND hWnd)
{
	typedef BOOL (WINAPI *PFNSHUTDOWNBLOCKREASONDESTROY)(HWND);

	// load user32.dll once only
	static HMODULE hUser32 = LoadLibrary(_T("user32.dll"));

	if (hUser32)
	{
		static PFNSHUTDOWNBLOCKREASONDESTROY fnDestroy = (PFNSHUTDOWNBLOCKREASONDESTROY)GetProcAddress(hUser32, "ShutdownBlockReasonDestroy");

		if (fnDestroy)
			return fnDestroy(hWnd);
	}

	// must be < XP
	return FALSE;
}

BOOL Misc::IsWorkStationLocked()
{
	// note: we can't call OpenInputDesktop directly because it's not
	// available on win 9x
	typedef HDESK (WINAPI *PFNOPENDESKTOP)(LPSTR lpszDesktop, DWORD dwFlags, BOOL fInherit, ACCESS_MASK dwDesiredAccess);
	typedef BOOL (WINAPI *PFNCLOSEDESKTOP)(HDESK hDesk);
	typedef BOOL (WINAPI *PFNSWITCHDESKTOP)(HDESK hDesk);

	// load user32.dll once only
	static HMODULE hUser32 = LoadLibrary(_T("user32.dll"));

	if (hUser32)
	{
		static PFNOPENDESKTOP fnOpenDesktop = (PFNOPENDESKTOP)GetProcAddress(hUser32, "OpenDesktopA");
		static PFNCLOSEDESKTOP fnCloseDesktop = (PFNCLOSEDESKTOP)GetProcAddress(hUser32, "CloseDesktop");
		static PFNSWITCHDESKTOP fnSwitchDesktop = (PFNSWITCHDESKTOP)GetProcAddress(hUser32, "SwitchDesktop");

		if (fnOpenDesktop && fnCloseDesktop && fnSwitchDesktop)
		{
			HDESK hDesk = fnOpenDesktop("Default", 0, FALSE, DESKTOP_SWITCHDESKTOP);

			if (hDesk)
			{
				BOOL bLocked = !fnSwitchDesktop(hDesk);
					
				// cleanup
				fnCloseDesktop(hDesk);

				return bLocked;
			}
		}
	}

	// must be win9x
	return FALSE;
}

BOOL Misc::IsScreenSaverActive()
{
#ifndef SPI_GETSCREENSAVERRUNNING
#  define SPI_GETSCREENSAVERRUNNING 114
#endif

	BOOL bSSIsRunning = FALSE;
	::SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, &bSSIsRunning, 0);
	
	return bSSIsRunning; 
}

LANGID Misc::GetUserDefaultUILanguage()
{
	typedef LANGID (WINAPI *FNGETUSERDEFAULTUILANGUAGE)(VOID);

	// must link dynamically to kernel32 else problem with win95/NT4
	static HMODULE hLib = LoadLibrary(_T("kernel32.dll"));
	LANGID nLangID = 0;

	if (hLib)
	{
		FNGETUSERDEFAULTUILANGUAGE pFN = (FNGETUSERDEFAULTUILANGUAGE)GetProcAddress(hLib, "GetUserDefaultUILanguage");

		if (pFN)
			nLangID = pFN();
	}

	return nLangID;
}

CString Misc::GetDefCharset()
{
	CString sDefCharset;
	GetLocaleInfo(LOCALE_USER_DEFAULT, LOCALE_IDEFAULTANSICODEPAGE, sDefCharset.GetBuffer(7), 6);
	sDefCharset.ReleaseBuffer();

	return sDefCharset;
}

BOOL Misc::LCMapString(CString& sText, DWORD dwMapFlags)
{
	int nLen = sText.GetLength();

	WORD wLangID = MAKELANGID(LANG_NEUTRAL, SUBLANG_SYS_DEFAULT);
	DWORD dwLCID = MAKELCID(wLangID, SORT_DEFAULT);

	BOOL bRes = ::LCMapString(dwLCID, 
								dwMapFlags, 
								(LPCTSTR)sText, 
								-1, 
								sText.GetBuffer(nLen+1), 
								nLen+1);
	sText.ReleaseBuffer();
	
	return bRes;
}

BOOL Misc::MakeUpper(CString& sText)
{
	return LCMapString(sText, LCMAP_UPPERCASE | LCMAP_LINGUISTIC_CASING);
}

BOOL Misc::MakeLower(CString& sText)
{
	return LCMapString(sText, LCMAP_LOWERCASE | LCMAP_LINGUISTIC_CASING);
}

BOOL Misc::FindWord(LPCTSTR szWord, LPCTSTR szText, BOOL bCaseSensitive, BOOL bMatchWholeWord)
{
	CString sWord(szWord), sText(szText);
	
	if (sWord.GetLength() > sText.GetLength())
		return FALSE;
	
	sWord.TrimLeft();
	sWord.TrimRight();
	
	if (!bCaseSensitive)
	{
		MakeUpper(sWord);
		MakeUpper(sText);
	}
	
	int nFind = sText.Find(sWord);
	
	if (nFind == -1)
		return FALSE;
	
	else if (bMatchWholeWord) // test whole word
	{
		const CString DELIMS("()-\\/{}[]:;,. ?\"'");
		
		// prior and next chars must be delimeters
		TCHAR cPrevChar = 0, cNextChar = 0;
		
		// prev
		if (nFind == 0) // word starts at start
			cPrevChar = ' '; // known delim
		else
			cPrevChar = sText[nFind - 1];
		
		// next
		if ((nFind + sWord.GetLength()) < sText.GetLength())
			cNextChar = sText[nFind + sWord.GetLength()];
		else
			cNextChar = ' '; // known delim
		
		if (DELIMS.Find(cPrevChar) == -1 || DELIMS.Find(cNextChar) == -1)
			return FALSE;
	}
	
	return TRUE;
}

int Misc::ParseSearchString(LPCTSTR szLookFor, CStringArray& aWords)
{
	aWords.RemoveAll();
	
	// parse on spaces unless enclosed in double-quotes
	int nLen = lstrlen(szLookFor);
	BOOL bInQuotes = FALSE, bAddWord = FALSE;
	CString sWord;
	
	for (int nPos = 0; nPos < nLen; nPos++)
	{
		switch (szLookFor[nPos])
		{
		case ' ': // word break
			if (bInQuotes)
				sWord += szLookFor[nPos];
			else
				bAddWord = TRUE;
			break;
			
		case '\"':
			// whether its the start or end we add the current word
			// and flip bInQuotes
			bInQuotes = !bInQuotes;
			bAddWord = TRUE;
			break;
			
		default: // everything else
			sWord += szLookFor[nPos];
			
			// also if its the last char then add it
			bAddWord = (nPos == nLen - 1);
			break;
		}
		
		if (bAddWord)
		{
			sWord.TrimLeft();
			sWord.TrimRight();
			
			if (!sWord.IsEmpty())
				aWords.Add(sWord);
			
			sWord.Empty(); // next word
		}
	}
	
	return aWords.GetSize();
}

CString Misc::Format(double dVal, int nDecPlaces)
{
	char* szLocale = _strdup(setlocale(LC_NUMERIC, NULL)); // current locale
	setlocale(LC_NUMERIC, ""); // local default

	CString sValue;

	if (nDecPlaces < 0)
		sValue.Format(_T("%f"), dVal);
	else
		sValue.Format(_T("%.*f"), nDecPlaces, dVal);
				
	// restore locale
	setlocale(LC_NUMERIC, szLocale);
	free(szLocale);

	return sValue;
}

CString Misc::Format(int nVal)
{
	CString sValue;
	sValue.Format(_T("%ld"), nVal);

	return sValue;
}

CString Misc::Format(DWORD dwVal)
{
	CString sValue;
	sValue.Format(_T("%lu"), dwVal);

	return sValue;
}

CString Misc::FormatCost(double dCost)
{
	CString sValue;
	sValue.Format(_T("%.6f"), dCost);

	char* szLocale = _strdup(setlocale(LC_NUMERIC, NULL)); // current locale
	setlocale(LC_NUMERIC, ""); // local default

	const UINT BUFSIZE = 100;
	TCHAR szCost[BUFSIZE + 1];

	GetCurrencyFormat(NULL, 0, sValue, NULL, szCost, BUFSIZE);
	sValue = szCost;
				
	// restore locale
	setlocale(LC_NUMERIC, szLocale);
	free(szLocale);

	return sValue;
}

BOOL Misc::KeyIsPressed(DWORD dwVirtKey) 
{ 
	return (GetKeyState(dwVirtKey) & 0x8000); 
}

BOOL Misc::IsCursorKeyPressed(DWORD dwKeys)
{
	if (dwKeys & MKC_LEFTRIGHT)
	{
		if (KeyIsPressed(VK_RIGHT) || KeyIsPressed(VK_LEFT) ||
			KeyIsPressed(VK_HOME) || KeyIsPressed(VK_END))
			return TRUE;
	}
	
	if (dwKeys & MKC_UPDOWN)
	{
		if (KeyIsPressed(VK_NEXT) || KeyIsPressed(VK_DOWN) ||
			KeyIsPressed(VK_UP) || KeyIsPressed(VK_PRIOR))
			return TRUE;
	}
	
	// else 
	return FALSE;
}

BOOL Misc::IsCursorKey(DWORD dwVirtKey, DWORD dwKeys)
{
	if (dwKeys & MKC_LEFTRIGHT)
	{
		switch (dwVirtKey)
		{
		case VK_RIGHT:
		case VK_LEFT:
		case VK_HOME:
		case VK_END:
			return TRUE;
		}
	}
	
	if (dwKeys & MKC_UPDOWN)
	{
		switch (dwVirtKey)
		{
		case VK_NEXT:  
		case VK_DOWN:
		case VK_UP:
		case VK_PRIOR: 
			return TRUE;
		}
	}
	
	// else 
	return FALSE;
}

BOOL Misc::ModKeysArePressed(DWORD dwKeys) 
{
	// test for failure
	if (dwKeys & MKS_CTRL)
	{
		if (!KeyIsPressed(VK_CONTROL))
			return FALSE;
	}
	else if (KeyIsPressed(VK_CONTROL))
	{
		return FALSE;
	}

	if (dwKeys & MKS_SHIFT)
	{
		if (!KeyIsPressed(VK_SHIFT))
			return FALSE;
	}
	else if (KeyIsPressed(VK_SHIFT))
	{
		return FALSE;
	}

	if (dwKeys & MKS_ALT)
	{
		if (!KeyIsPressed(VK_MENU))
			return FALSE;
	}
	else if (KeyIsPressed(VK_MENU))
	{
		return FALSE;
	}

	return TRUE;
}

BOOL Misc::HasFlag(DWORD dwFlags, DWORD dwFlag) 
{ 
	if ((dwFlags & dwFlag) == dwFlag)
		return TRUE;
	else
		return FALSE; 
}

CString Misc::MakeKey(const CString& sFormat, int nKeyVal, LPCTSTR szParentKey)
{
	ASSERT(!sFormat.IsEmpty());

	CString sKey;
	sKey.Format(sFormat, nKeyVal);

	if (szParentKey && *szParentKey)
	{
		CString sTemp;
		sTemp.Format(_T("%s\\%s"), szParentKey, sKey);

		// remove doubled-up back slashes
		sTemp.Replace(_T("\\\\"), _T("\\"));
		sKey = sTemp;
	}

	return sKey;
}

CString Misc::MakeKey(const CString& sFormat, LPCTSTR szKeyVal, LPCTSTR szParentKey)
{
	CString sKey;
	sKey.Format(sFormat, szKeyVal);

	return MakeKey(sKey, 0, szParentKey);
}

int Misc::NaturalCompare(LPCTSTR szString1, LPCTSTR szString2)
{
#ifdef _UNICODE
	// initialize once only per session
	static HMODULE hShlwapi = ::LoadLibrary(_T("Shlwapi.dll"));
	   
	typedef int (WINAPI *PFNSTRCMPLOGICAL)(PCWSTR, PCWSTR);
	static PFNSTRCMPLOGICAL pFn = (PFNSTRCMPLOGICAL)::GetProcAddress(hShlwapi, "StrCmpLogicalW");
	
	if (pFn)
		return pFn(szString1, szString2);
#endif
	
	// all else
	return _tcsicmp(szString1, szString2);
}

//////////////////////////////////////////////////////////////

// private string compare function
int StringSortProc(const void* v1, const void* v2)
{
	CString* pStr1 = (CString*)v1;
	CString* pStr2 = (CString*)v2;

	return Misc::NaturalCompare(*pStr1, *pStr2);
}

void Misc::SortArray(CStringArray& array, STRINGSORTPROC pSortProc)
{
	qsort(array.GetData(), array.GetSize(), sizeof(CString*), pSortProc ? pSortProc : StringSortProc);
}

// private dword compare function
int DWordSortProc(const void* v1, const void* v2)
{
	DWORD* pDW1 = (DWORD*)v1;
	DWORD* pDW2 = (DWORD*)v2;

	if (*pDW1 < *pDW2)
		return -1;
	
	else if (*pDW1 > *pDW2)
		return 1;

	// else
	return 0;
}

void Misc::SortArray(CDWordArray& array, DWORDSORTPROC pSortProc)
{
	qsort(array.GetData(), array.GetSize(), sizeof(DWORD*), pSortProc ? pSortProc : DWordSortProc);
}

//////////////////////////////////////////////////////////////

int Misc::ShowMessageBox(HWND hwndParent, LPCTSTR szCaption, LPCTSTR szInstruction, LPCTSTR szText, UINT nFlags)
{
#ifndef TD_WARNING_ICON
	
#define TD_WARNING_ICON         MAKEINTRESOURCEW(-1)
#define TD_ERROR_ICON           MAKEINTRESOURCEW(-2)
#define TD_INFORMATION_ICON     MAKEINTRESOURCEW(-3)
#define TD_SHIELD_ICON          MAKEINTRESOURCEW(-4)
	
	enum 
	{
		TDCBF_OK_BUTTON            = 0x0001, // selected control return value IDOK
		TDCBF_YES_BUTTON           = 0x0002, // selected control return value IDYES
		TDCBF_NO_BUTTON            = 0x0004, // selected control return value IDNO
		TDCBF_CANCEL_BUTTON        = 0x0008, // selected control return value IDCANCEL
		TDCBF_RETRY_BUTTON         = 0x0010, // selected control return value IDRETRY
		TDCBF_CLOSE_BUTTON         = 0x0020  // selected control return value IDCLOSE
	};
	
#endif // TD_WARNING_ICON
	
	HMODULE hMod = ::LoadLibrary(_T("Comctl32.dll"));
	
	if (hMod)
	{
		typedef HRESULT (WINAPI *PFNTASKDIALOG)(HWND, HINSTANCE, PCWSTR, PCWSTR, PCWSTR, DWORD, PCWSTR, int*);
		PFNTASKDIALOG pFn = (PFNTASKDIALOG)::GetProcAddress(hMod, "TaskDialog");
		
		if (pFn)
		{
			// convert string to unicode as required
#ifdef _UNICODE
			LPCWSTR wszCaption = szCaption;
			LPCWSTR wszInstruction = szInstruction;
			LPCWSTR wszText = szText;
#else
			LPWSTR wszCaption = MultiByteToWide(szCaption);
			LPWSTR wszText = MultiByteToWide(szText);
			LPWSTR wszInstruction = MultiByteToWide(szInstruction);
#endif
			
			int nResult = 0;
			
			// buttons and icons
			LPCWSTR wszIcon = NULL;
			DWORD dwButtons = 0;
			int nButtons = (nFlags & MB_TYPEMASK);
			
			if (nButtons == MB_OK)
			{
				wszIcon = TD_INFORMATION_ICON;
				dwButtons = TDCBF_OK_BUTTON;
			}
			else if (nButtons == MB_OKCANCEL)
			{
				wszIcon = TD_WARNING_ICON;
				dwButtons = TDCBF_OK_BUTTON | TDCBF_CANCEL_BUTTON;
			}
			else if (nButtons == MB_RETRYCANCEL)
			{
				wszIcon = TD_ERROR_ICON;
				dwButtons = TDCBF_RETRY_BUTTON | TDCBF_CANCEL_BUTTON;
			}
			else if (nButtons == MB_YESNO)
			{
				wszIcon = TD_INFORMATION_ICON;
				dwButtons = TDCBF_YES_BUTTON | TDCBF_NO_BUTTON;
			}
			else if (nButtons == MB_YESNOCANCEL)
			{
				wszIcon = TD_WARNING_ICON;
				dwButtons = TDCBF_YES_BUTTON | TDCBF_NO_BUTTON | TDCBF_CANCEL_BUTTON;
			}
			
			HRESULT hr = pFn(hwndParent, NULL, wszCaption, wszInstruction, wszText, dwButtons, wszIcon, &nResult);
			
			// clean up
#ifndef _UNICODE
			delete [] wszText;
			delete [] wszCaption;
			delete [] wszInstruction;
#endif
			
			return SUCCEEDED(hr) ? nResult : IDCANCEL;
		}
	}
	
	// all else
	CEnString sCaption(szCaption);
	
	if (szInstruction && *szInstruction)
	{
		if (sCaption.IsEmpty())
			szCaption = AfxGetAppName();

		sCaption.Format(_T("%s - %s"), szInstruction, szCaption);
	}
		
	return ::MessageBox(hwndParent, szText, sCaption, nFlags);
}

