// TaskTimeLog.cpp: implementation of the CTaskTimeLog class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "todolist.h"
#include "TaskTimeLog.h"

#include "..\shared\Filemisc.h"
#include "..\shared\misc.h"
#include "..\shared\datehelper.h"
#include "..\shared\timehelper.h"
#include "..\shared\enstring.h"
#include "..\shared\FileRegister.h"

#include "..\3rdParty\stdiofileex.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

const LPCTSTR HEADER_LINE = _T("TODOTIMELOG VERSION");

const TCHAR TAB = '\t';

//////////////////////////////////////////////////////////////////////

enum CSVFMT_VERSION
{
	VER_NONE = -1,
	VER_0,
	// insert here
	VER_LATEST
};

//////////////////////////////////////////////////////////////////////

struct VERSION_INFO
{
	CSVFMT_VERSION nVersion;
	LPCTSTR szRowFormat;
	int nTimeField;
};

//////////////////////////////////////////////////////////////////////

const VERSION_INFO VERSIONS[] = 
{
	{ VER_0, _T("%s,%s,%s,%s,%s,%s"), 2 },
	// insert here
	{ VER_LATEST, _T("%s,%s,%s,%s,%s,%s,%s,%s"), 7 }
};
const int NUM_VERSIONS = sizeof(VERSIONS) / sizeof(VERSION_INFO);

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CTaskTimeLog::CTaskTimeLog(LPCTSTR szRefPath, SFE_SAVEAS nSaveAs) 
	: 
	m_sRefPath(szRefPath), 
	m_nSaveAs(nSaveAs),
	m_nVersion(VER_NONE),
	m_bLogExists(FALSE),
	m_sDelim(Misc::GetListSeparator())
{
}

CTaskTimeLog::~CTaskTimeLog()
{

}

BOOL CTaskTimeLog::LogTime(DWORD dwTaskID, LPCTSTR szTaskTitle, double dTime, BOOL bLogSeparately)
{
	return LogTime(dwTaskID, szTaskTitle, dTime, COleDateTime::GetCurrentTime(), bLogSeparately);
}

BOOL CTaskTimeLog::LogTime(DWORD dwTaskID, LPCTSTR szTaskTitle, double dTime, 
						   const COleDateTime& dtWhen, BOOL bLogSeparately)
{
	CString sLogPath = GetLogPath(dwTaskID, bLogSeparately);

	// init state
	Initialise(sLogPath);
	ASSERT(m_nVersion != VER_NONE);

	// if the file doesn't exist then we insert the column headings as the first line
	// and we use the passed in format for the log file
	CString sLog;

	if (!m_bLogExists)
	{
		// format header line and column header
		sLog.Format(_T("%s %d\n%s\n"), HEADER_LINE, m_nVersion, GetLatestColumnHeader());
	}

	COleDateTime dtEnd = dtWhen;
	COleDateTime dtStart = dtEnd - COleDateTime(dTime / 24); // dTime is in hours

	sLog += FormatItemRow(dwTaskID, szTaskTitle, dTime, dtStart, dtEnd);

	return FileMisc::AppendLineToFile(sLogPath, sLog, SFE_ASIS);
}

CString CTaskTimeLog::FormatItemRow(DWORD dwTaskID, LPCTSTR szTaskTitle, double dTime, 
									const COleDateTime& dtStart, const COleDateTime& dtEnd) const
{
	CString sItem, sRowFormat(GetRowFormat(m_nVersion));

	switch (m_nVersion)
	{
	case VER_0:
		sItem.Format(sRowFormat, 
					Misc::Format(dwTaskID),
					szTaskTitle,
					Misc::Format(dTime, 3), 
					Misc::GetUserName(),
					CDateHelper::FormatDate(dtEnd, DHFD_ISO | DHFD_TIME), 
					CDateHelper::FormatDate(dtStart, DHFD_ISO | DHFD_TIME));
		break;

	case VER_LATEST:
		sItem.Format(sRowFormat, 
					Misc::Format(dwTaskID), 
					szTaskTitle,
					Misc::GetUserName(),
					CDateHelper::FormatDate(dtStart, DHFD_ISO),
					CTimeHelper::FormatISOTime(dtStart), 
					CDateHelper::FormatDate(dtEnd, DHFD_ISO), 
					CTimeHelper::FormatISOTime(dtEnd),
					Misc::Format(dTime, 3));		
		break;

	default:
		ASSERT(0);
	}

	return sItem;
}

CString CTaskTimeLog::GetLogPath(DWORD dwTaskID, BOOL bLogSeparately)
{
	CString sLogPath, sDrive, sFolder, sFileName;

	// use ref filename as the basis for the log filename
	FileMisc::SplitPath(m_sRefPath, &sDrive, &sFolder, &sFileName);
	
	if (bLogSeparately)
		sLogPath.Format(_T("%s%s%s\\%ld_Log.csv"), sDrive, sFolder, sFileName, dwTaskID);
	else
		sLogPath.Format(_T("%s%s%s_Log.csv"), sDrive, sFolder, sFileName);

	return sLogPath;
}

double CTaskTimeLog::CalcAccumulatedTime(DWORD dwTaskID, BOOL bLogSeparately)
{
	CString sLogPath = GetLogPath(dwTaskID, bLogSeparately);
	double dTotalTime = 0;

	if (FileMisc::FileExists(sLogPath))
	{
		CStdioFileEx file;

		const VERSION_INFO& vi = VERSIONS[m_nVersion];
		ASSERT(vi.nVersion == m_nVersion);

		if (file.Open(sLogPath, CFile::modeRead | CFile::typeText))
		{
			CString sLine;

			while (file.ReadString(sLine))
			{
				// decode it
				CStringArray aParts;

				if (Misc::Split(sLine, aParts, TRUE) > vi.nTimeField)
				{
					long nLogID = _ttol(aParts[0]); // always the first item

					if (nLogID > 0 && (DWORD)nLogID == dwTaskID)
					{
						dTotalTime += _ttof(aParts[vi.nTimeField]);
					}
				}
			}
		}
	}

	return dTotalTime;
}

CString CTaskTimeLog::GetLatestColumnHeader() const // always the latest version
{
	// sanity check
	ASSERT(VER_LATEST == NUM_VERSIONS - 1);

	CString sRowFormat = GetRowFormat(VER_LATEST);
	CString sColumnHeader;

	sColumnHeader.Format(sRowFormat,
						CEnString(IDS_CSV_TASKID),
						CEnString(IDS_CSV_TASKTITLE),
						CEnString(IDS_CSV_USERID),
						CEnString(IDS_CSV_STARTDATE),
						CEnString(IDS_CSV_STARTTIME),
						CEnString(IDS_CSV_ENDDATE),
						CEnString(IDS_CSV_ENDTIME),
						CEnString(IDS_CSV_TIMESPENT));
	
	return sColumnHeader;
}

CString CTaskTimeLog::GetRowFormat(int nVersion) const
{
	// sanity checks
	ASSERT(VER_LATEST == NUM_VERSIONS - 1);
	ASSERT(nVersion <= VER_LATEST);

	const VERSION_INFO& vi = VERSIONS[nVersion];
	ASSERT(vi.nVersion == nVersion);

	// replace default comma with delimiter
	CString sRowFormat(vi.szRowFormat);
	sRowFormat.Replace(_T(","), m_sDelim);

	return sRowFormat;
}

void CTaskTimeLog::Initialise(const CString& sLogPath)
{
	// Once only
	if (m_nVersion != VER_NONE)
		return; 

	m_bLogExists = FileMisc::FileExists(sLogPath);

	if (!m_bLogExists) // new log file
	{
		m_nVersion = VER_LATEST;
	
		// if Excel is installed we use a tab as delimiter
		if (CFileRegister::IsRegisteredApp(_T("csv"), _T("EXCEL.EXE"), TRUE))
			m_sDelim = TAB;
	}
	else
	{
		// get version and delimiter from file
		m_nVersion = VER_0;
		
		CStringArray aLines;
		
		if (FileMisc::LoadFileLines(sLogPath, aLines, 2))
		{
			CString sLine = aLines[0];
			
			// version
			if (sLine.Find(HEADER_LINE) == 0)
			{
				int nSpace = sLine.ReverseFind(' ');
				
				if (nSpace != -1)
				{
					m_nVersion = _ttoi(((LPCTSTR)sLine) + nSpace + 1);
					sLine = aLines[1];
				}
			}

			// check for tab char in column header
			if (sLine.Find(TAB) != -1)
				m_sDelim = TAB;
		}
	}
}
