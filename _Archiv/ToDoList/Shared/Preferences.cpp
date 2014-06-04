// Preferences.cpp: implementation of the CPreferences class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "Preferences.h"
#include "misc.h"
#include "filemisc.h"
#include "driveinfo.h"
#include "regkey.h"

#include "..\3rdparty\stdiofileex.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CIniSectionArray CPreferences::s_aIni;
BOOL CPreferences::s_bDirty = FALSE;
BOOL CPreferences::s_bIni = FALSE;
int CPreferences::s_nRef = 0;

CPreferences::CPreferences()
{
	// if no other object is active we need to do the setup
	if (s_nRef == 0)
	{
		// figure out whether we're using an ini file or the registry
		s_bIni = (AfxGetApp()->m_pszRegistryKey == NULL);
		
		if (s_bIni)
		{
			// check for existing backup file first
			CString sIniPath(AfxGetApp()->m_pszProfileName);
			CString sBackupPath(sIniPath);
			
			FileMisc::ReplaceExtension(sBackupPath, _T("bak.ini"));
			
			if (FileMisc::FileExists(sBackupPath))
				::CopyFile(sBackupPath, sIniPath, FALSE);			
			
			// read the ini file
			CStdioFileEx file;
			//DWORD dwTick = GetTickCount();
			
			if (file.Open(sIniPath, CFile::modeRead))
			{
				CString sLine;
				INISECTION* pCurSection = NULL;
				
				while (file.ReadString(sLine))
				{
					if (sLine.IsEmpty())
						continue;
					
					// is it a section ?
					else if (sLine[0] == '[')
					{
						CString sSection = sLine.Mid(1, sLine.GetLength() - 2);

						// assume (for speed) that the section is already unique
						pCurSection = new INISECTION(sSection);
						s_aIni.Add(pCurSection);
						// pCurSection = GetSection(sSection, TRUE);
						ASSERT (pCurSection != NULL);
					}
					// else an entry
					else if (pCurSection)
					{
						int nEquals = sLine.Find('=');
						
						if (nEquals != -1)
						{
							CString sEntry = sLine.Left(nEquals);
							sEntry.TrimRight();
							
							CString sValue = sLine.Mid(nEquals + 1);
							sValue.TrimLeft();
							
							// remove quotes
							sValue.Replace(_T("\""), _T(""));
							
							if (!sEntry.IsEmpty())
								SetEntryValue(*pCurSection, sEntry, sValue);
						}
					}
				}
			}

			s_bDirty = FALSE;
			//TRACE (_T("CPreferences(Load Ini = %dms)\n"), GetTickCount() - dwTick);

			// delete backup
			::DeleteFile(sBackupPath);
		}
	}
	
	s_nRef++; // increment reference count
}

CPreferences::~CPreferences()
{
	s_nRef--; // decrement reference count
	
	// save ini?
	if (s_nRef == 0 && s_bIni)
	{
		Save();

		// cleanup
		int nSection = s_aIni.GetSize();
		
		while (nSection--)
			delete s_aIni[nSection];
		
		s_aIni.RemoveAll();
	}
}

BOOL CPreferences::Save(BOOL bForce)
{
	if (s_bDirty || bForce)
	{
		// backup file first
		CString sIniPath(AfxGetApp()->m_pszProfileName);
		CString sBackupPath(sIniPath);

		FileMisc::ReplaceExtension(sBackupPath, _T("bak.ini"));
		::CopyFile(sIniPath, sBackupPath, FALSE);

		// write prefs
		CStdioFileEx file;

#ifdef _DEBUG
		DWORD dwTick = GetTickCount();
#endif
		if (file.Open(sIniPath, CFile::modeWrite | CFile::modeCreate))
		{
			for (int nSection = 0; nSection < s_aIni.GetSize(); nSection++)
			{
				// write section line
				INISECTION* pSection = s_aIni[nSection];
				
				CString sLine;
				sLine.Format(_T("[%s]\n"), pSection->sSection);
				
				file.WriteString(sLine);
				
				// write entries to a CStringArray, then sort it and write it to file
				CStringArray aEntries;
				aEntries.SetSize(pSection->aEntries.GetCount(), 10);
				
				// save map to array
				POSITION pos = pSection->aEntries.GetStartPosition();
				int nEntry = 0;
				
				while (pos)
				{
					CString sDummy;
					INIENTRY ie;
					
					pSection->aEntries.GetNextAssoc(pos, sDummy, ie);
					sLine.Format(_T("%s=%s"), ie.sName, ie.sValue);
					
					aEntries.SetAt(nEntry++, sLine);
				}
				
				// sort array
				Misc::SortArray(aEntries);
				
				// format by newlines
				CString sSection = Misc::FormatArray(aEntries, _T("\n")) + _T("\n\n");
				
				// save to file
				file.WriteString(sSection);
			}
		}
		else
			return FALSE;
		
#ifdef _DEBUG
		TRACE (_T("CPreferences(Save = %dms)\n"), GetTickCount() - dwTick);
#endif
		s_bDirty = FALSE;

		// delete backup
		::DeleteFile(sBackupPath);
	}
	
	return TRUE;
}

CString CPreferences::ToString(int nValue)
{
	CString sValue;
	sValue.Format(_T("%ld"), nValue);
	return sValue;
}

CString CPreferences::ToString(double dValue)
{
	CString sValue;
	sValue.Format(_T("%.6f"), dValue);
	return sValue;
}

UINT CPreferences::GetProfileInt(LPCTSTR lpszSection, LPCTSTR lpszEntry, int nDefault) const
{
	if (s_bIni)
	{
		CString sValue = GetProfileString(lpszSection, lpszEntry);
		
		if (sValue.IsEmpty()) 
			return nDefault;
		else
			return _ttol(sValue);
	}
	else
		return AfxGetApp()->GetProfileInt(lpszSection, lpszEntry, nDefault);
}

BOOL CPreferences::WriteProfileInt(LPCTSTR lpszSection, LPCTSTR lpszEntry, int nValue)
{
	if (s_bIni)
		return WriteProfileString(lpszSection, lpszEntry, ToString(nValue));
	else
		return AfxGetApp()->WriteProfileInt(lpszSection, lpszEntry, nValue);
}

CString CPreferences::GetProfileString(LPCTSTR lpszSection, LPCTSTR lpszEntry, LPCTSTR lpszDefault) const
{
	if (s_bIni)
	{
		INISECTION* pSection = GetSection(lpszSection, FALSE);
		
		if (pSection)
			return GetEntryValue(*pSection, lpszEntry, lpszDefault);
		else
			return lpszDefault;
	}
	else
		return AfxGetApp()->GetProfileString(lpszSection, lpszEntry, lpszDefault);
}

BOOL CPreferences::WriteProfileString(LPCTSTR lpszSection, LPCTSTR lpszEntry, LPCTSTR lpszValue)
{
	if (s_bIni)
	{
		INISECTION* pSection = GetSection(lpszSection, TRUE);
		ASSERT(pSection);
		
		if (pSection)
		{
			SetEntryValue(*pSection, lpszEntry, lpszValue);
			return TRUE;
		}
		
		// else
		return FALSE;
	}
	else
		return AfxGetApp()->WriteProfileString(lpszSection, lpszEntry, lpszValue);
}



double CPreferences::GetProfileDouble(LPCTSTR lpszSection, LPCTSTR lpszEntry, double dDefault) const
{
	CString sValue = GetProfileString(lpszSection, lpszEntry, ToString(dDefault));
	
	if (sValue.IsEmpty())
		return dDefault;
	else
		return Misc::Atof(sValue);
}

BOOL CPreferences::WriteProfileDouble(LPCTSTR lpszSection, LPCTSTR lpszEntry, double dValue)
{
	return WriteProfileString(lpszSection, lpszEntry, ToString(dValue));
}

int CPreferences::GetArrayItems(LPCTSTR lpszSection, CStringArray& aItems) const
{
	aItems.RemoveAll();
	int nCount = GetProfileInt(lpszSection, _T("ItemCount"), 0);
	
	// items
	for (int nItem = 0; nItem < nCount; nItem++)
	{
		CString sItemKey, sItem;
		sItemKey.Format(_T("Item%d"), nItem);
		sItem = GetProfileString(lpszSection, sItemKey);
		
		if (!sItem.IsEmpty())
			aItems.Add(sItem);
	}
	
	return aItems.GetSize();
}

void CPreferences::WriteArrayItems(const CStringArray& aItems, LPCTSTR lpszSection)
{
	int nCount = aItems.GetSize();
	
	// items
	for (int nItem = 0; nItem < nCount; nItem++)
	{
		CString sItemKey;
		sItemKey.Format(_T("Item%d"), nItem);
		WriteProfileString(lpszSection, sItemKey, aItems[nItem]);
	}
	
	// item count
	WriteProfileInt(lpszSection, _T("ItemCount"), nCount);
}

CString CPreferences::GetEntryValue(INISECTION& section, LPCTSTR lpszEntry, LPCTSTR lpszDefault)
{
	CString sKey(lpszEntry);
	sKey.MakeUpper();
	
	INIENTRY ie;
	
	if (section.aEntries.Lookup(sKey, ie) && !ie.sValue.IsEmpty())
		return ie.sValue;
	
	return lpszDefault;
}

void CPreferences::SetEntryValue(INISECTION& section, LPCTSTR lpszEntry, LPCTSTR szValue)
{
	INIENTRY entry;
	CString sKey(lpszEntry);
	sKey.MakeUpper();

	if (!section.aEntries.Lookup(sKey, entry) || entry.sValue != szValue)
	{
		section.aEntries[sKey] = INIENTRY(lpszEntry, szValue);
		s_bDirty = TRUE;
	}
}

INISECTION* CPreferences::GetSection(LPCTSTR lpszSection, BOOL bCreateNotExist)
{
	int nSection = FindSection(lpszSection);

	if (nSection != -1)
		return s_aIni[nSection];
	
	// add a new section
	if (bCreateNotExist)
	{
		INISECTION* pSection = new INISECTION(lpszSection);
		
		s_aIni.Add(pSection);
		return pSection;
	}
	
	// else
	return NULL;
}

int CPreferences::FindSection(LPCTSTR lpszSection, BOOL bIncSubSections)
{
	int nLenSection = lstrlen(lpszSection);
	int nSection = s_aIni.GetSize();
	
	while (nSection--)
	{
		const CString& sSection = s_aIni[nSection]->sSection;

		if (sSection.GetLength() < nLenSection)
			continue;
		
		else if (sSection.GetLength() == nLenSection)
		{
			if (sSection.CompareNoCase(lpszSection) == 0)
				return nSection;
		}
		else // sSection.GetLength() > nLenSection
		{
			if (bIncSubSections)
			{
				// look for parent section at head of subsection
				CString sTest = sSection.Left(nLenSection);

				if (sTest.CompareNoCase(lpszSection) == 0)
					return nSection;
			}
		}
	}

	// not found
	return -1;
}

BOOL CPreferences::DeleteSection(LPCTSTR lpszSection, BOOL bIncSubSections)
{
	ASSERT(lpszSection && *lpszSection);

	if (s_bIni)
	{
		int nSection = FindSection(lpszSection);
		 
		if (nSection != -1)
		{
			delete s_aIni[nSection];
			s_aIni.RemoveAt(nSection);
			s_bDirty = TRUE;
			
			if (bIncSubSections)
			{
				nSection = FindSection(lpszSection, TRUE);

				while (nSection != -1)
				{
					delete s_aIni[nSection];
					s_aIni.RemoveAt(nSection);
					nSection = FindSection(lpszSection, TRUE);
				} 
			}

			return TRUE;
		}

		// not found
		return FALSE;
	}

	// else registry
	CString sFullKey;
	sFullKey.Format(_T("%s\\%s"), AfxGetApp()->m_pszRegistryKey, lpszSection);

	return CRegKey::DeleteKey(HKEY_CURRENT_USER, sFullKey);
}

BOOL CPreferences::DeleteEntry(LPCTSTR lpszSection, LPCTSTR lpszEntry)
{
	ASSERT(lpszSection && *lpszSection);
	ASSERT(lpszEntry && *lpszEntry);

	if (s_bIni)
	{
		INISECTION* pSection = GetSection(lpszSection, FALSE);

		if (pSection)
		{
			CString sKey(lpszEntry);
			sKey.MakeUpper();

			pSection->aEntries.RemoveKey(sKey);
			s_bDirty = TRUE;

			return TRUE;
		}

		// not found
		return FALSE;
	}

	// else registry
	CString sFullKey;
	sFullKey.Format(_T("%s\\%s\\%s"), AfxGetApp()->m_pszRegistryKey, lpszSection, lpszEntry);

	return CRegKey::DeleteKey(HKEY_CURRENT_USER, sFullKey);
}

BOOL CPreferences::HasSection(LPCTSTR lpszSection) const
{
	if (s_bIni)
	{
		return (GetSection(lpszSection, FALSE) != NULL);
	}

	// else registry
	CString sFullKey;
	sFullKey.Format(_T("%s\\%s"), AfxGetApp()->m_pszRegistryKey, lpszSection);

	return CRegKey::KeyExists(HKEY_CURRENT_USER, sFullKey);
}

int CPreferences::GetSectionNames(CStringArray& aSections)
{
	aSections.RemoveAll();

	if (s_bIni)
	{
		int nSection = s_aIni.GetSize();
		aSections.SetSize(nSection);
		
		while (nSection--)
			aSections[nSection] = s_aIni[nSection]->sSection;
	}

	return aSections.GetSize();
}

CString CPreferences::KeyFromFile(LPCTSTR szFilePath, BOOL bFileNameOnly)
{
	CString sKey = bFileNameOnly ? FileMisc::GetFileNameFromPath(szFilePath) : szFilePath;
	sKey.Replace('\\', '_');

	// if the filepath is on a removable drive then we strip off the drive letter
	if (!bFileNameOnly)
	{
		int nType = CDriveInfo::GetPathType(szFilePath);

		if (nType == DRIVE_REMOVABLE)
			sKey = sKey.Mid(1);
	}

	return sKey;
}
