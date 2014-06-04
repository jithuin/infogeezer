// iCalExporter.cpp: implementation of the CiCalExporter class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "iCalImportExport.h"
#include "iCalExporter.h"

#include "..\Shared\DateHelper.h"
//#include "..\Shared\localizer.h"

#include "..\todolist\tdcenum.h"

#include "..\3rdParty\stdiofileex.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CiCalExporter::CiCalExporter()
{
	
}

CiCalExporter::~CiCalExporter()
{
	
}

void CiCalExporter::SetLocalizer(ITransText* /*pTT*/)
{
	//CLocalizer::Initialize(pTT);
}

void CiCalExporter::WriteHeader(CStdioFileEx& fileOut)
{
	WriteString(fileOut, _T("BEGIN:VCALENDAR"));
	WriteString(fileOut, _T("PRODID:iCalExporter (c) AbstractSpoon 2009-11"));
	WriteString(fileOut, _T("VERSION:2.0"));
}

bool CiCalExporter::Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL /*bSilent*/, IPreferences* /*pPrefs*/, LPCTSTR /*szKey*/)
{
	CStdioFileEx fileOut;
	fileOut.SetCodePage(CP_UTF8);
	
	if (fileOut.Open(szDestFilePath, CFile::modeCreate | CFile::modeWrite, SFE_MULTIBYTE))
	{
		// header
		WriteHeader(fileOut);
		
		// export first task
		const ITaskList6* pTasks = GetITLInterface<ITaskList6>(pSrcTaskFile, IID_TASKLIST6);
		ExportTask(pTasks, pTasks->GetFirstTask(), "", fileOut);
		
		// footer
		WriteString(fileOut, _T("END:VCALENDAR"));
		
		return true;
	}
	
	return false;
}

bool CiCalExporter::Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL /*bSilent*/, IPreferences* /*pPrefs*/, LPCTSTR /*szKey*/)
{
	CStdioFileEx fileOut;
	fileOut.SetCodePage(CP_UTF8);
	
	if (fileOut.Open(szDestFilePath, CFile::modeCreate | CFile::modeWrite, SFE_MULTIBYTE))
	{
		// header
		WriteHeader(fileOut);
		
		for (int nTaskList = 0; nTaskList < pSrcTaskFile->GetTaskListCount(); nTaskList++)
		{
			const ITaskList6* pTasks = GetITLInterface<ITaskList6>(pSrcTaskFile->GetTaskList(nTaskList), IID_TASKLIST6);

			if (pTasks)
			{
				// export first task
				ExportTask(pTasks, pTasks->GetFirstTask(), "", fileOut);
			}
		}

		// footer
		WriteString(fileOut, _T("END:VCALENDAR"));
		
		return true;
	}
	
	return false;
}

void CiCalExporter::ExportTask(const ITaskList6* pTasks, HTASKITEM hTask, const CString& sParentUID, CStdioFile& fileOut)
{
	if (!hTask)
		return;
	
	// attributes
	time_t tStartDate = pTasks->GetTaskStartDate(hTask);
	time_t tDueDate = pTasks->GetTaskDueDate(hTask, FALSE);
	
	// if task only has a start date then make the due date the same as the start and vice versa
	if (tDueDate == 0 && tStartDate)
		tDueDate = tStartDate;
	
	else if (tStartDate == 0 && tDueDate)
		tStartDate = tDueDate;
	
	// construct a unique ID
	CString sUID, sFile = fileOut.GetFilePath();
    
	sFile.Replace(_T("\\"), _T(""));
    sFile.Replace(_T(":"), _T(""));
	sUID.Format(_T("%ld@%s.com"), pTasks->GetTaskID(hTask), sFile);
		
	// tasks must have a start date or a due date or both
	if (tStartDate || tDueDate)
	{
		// header
		WriteString(fileOut, _T("BEGIN:VEVENT"));
		
		COleDateTime dtStart(tStartDate);
		WriteString(fileOut, FormatDateTime(_T("DTSTART"), dtStart));
		
		// neither Google Calendar not Outlook pay any attention to the 'DUE' tag so we won't either.
		// instead we use 'DTEND' to mark the duration of the task. There is also no way to mark a
		// task as complete so we ignore our completion status
		
		COleDateTime dtDue(tDueDate);
		WriteString(fileOut, FormatDateTime(_T("DTEND"), dtDue, FALSE));
		
		WriteString(fileOut, _T("SUMMARY:%s"), pTasks->GetTaskTitle(hTask));
		WriteString(fileOut, _T("STATUS:%s"), pTasks->GetTaskStatus(hTask));
		WriteString(fileOut, _T("CATEGORIES:%s"), pTasks->GetTaskCategory(hTask, 0));
		WriteString(fileOut, _T("URL:%s"), pTasks->GetTaskFileReferencePath(hTask));
		WriteString(fileOut, _T("ORGANIZER:%s"), pTasks->GetTaskAllocatedBy(hTask));
		WriteString(fileOut, _T("ATTENDEE:%s"), pTasks->GetTaskAllocatedTo(hTask));
		WriteString(fileOut, _T("UID:%s"), sUID);

		// comments
		CString sComments = pTasks->GetTaskComments(hTask);

		sComments.Replace(_T("\r\n"), _T(" "));
		sComments.Replace(_T("\n"), _T(" "));

		WriteString(fileOut, _T("DESCRIPTION:%s"), sComments);

		// recurrence
		int nRegularity, nUnused;
		DWORD dwSpecific1, dwSpecific2;

		if (pTasks->GetTaskRecurrence(hTask, nRegularity, dwSpecific1, dwSpecific2, nUnused, nUnused))
			WriteString(fileOut, FormatRecurrence(nRegularity, dwSpecific1, dwSpecific2));

		// parent child relationship
		WriteString(fileOut, _T("RELATED-TO;RELTYPE=PARENT:%s"), sParentUID);
		
		// footer
		WriteString(fileOut, _T("END:VEVENT"));
	}
	
	// copy across first child
	ExportTask(pTasks, pTasks->GetFirstTask(hTask), sUID, fileOut);
	
	// copy across first sibling
	ExportTask(pTasks, pTasks->GetNextTask(hTask), sParentUID, fileOut);
}

CString CiCalExporter::FormatRecurrence(int nRegularity, DWORD dwSpecific1, DWORD dwSpecific2)
{
	CString sRecurrence;

	switch (nRegularity)
	{
	case TDIR_DAY_EVERY: // TDIR_DAILY
	case TDIR_DAY_RECREATEAFTERNDAYS:
		sRecurrence.Format(_T("RRULE:FREQ=DAILY;INTERVAL=%d"), dwSpecific1);
		break;

	case TDIR_DAY_WEEKDAYS:
		sRecurrence.Format(_T("RRULE:FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR"));
		break;

	// -----------------------------------------------
	
	case TDIR_WEEK_EVERY: // TDIR_WEEKLY
	case TDIR_WEEK_RECREATEAFTERNWEEKS:
		{
			sRecurrence.Format(_T("RRULE:FREQ=WEEKLY;INTERVAL=%d;BYDAY="), dwSpecific1);

			if (dwSpecific2 & DHW_SUNDAY)
				sRecurrence += _T("SU,");
					
			if (dwSpecific2 & DHW_MONDAY)
				sRecurrence += _T("MO,");

			if (dwSpecific2 & DHW_TUESDAY)
				sRecurrence += _T("TU,");

			if (dwSpecific2 & DHW_WEDNESDAY)
				sRecurrence += _T("WE,");

			if (dwSpecific2 & DHW_THURSDAY)
				sRecurrence += _T("TH,");

			if (dwSpecific2 & DHW_FRIDAY)
				sRecurrence += _T("FR,");

			if (dwSpecific2 & DHW_SATURDAY)
				sRecurrence += _T("SA,");

			// strip off trailing comma
			int nLast = sRecurrence.GetLength() - 1;

			if (sRecurrence[nLast] == ',')
				sRecurrence = sRecurrence.Left(nLast);
		}
		break;

	// -----------------------------------------------
	
	case TDIR_MONTH_EVERY: // TDIR_MONTHLY
	case TDIR_MONTH_RECREATEAFTERNMONTHS:
		sRecurrence.Format(_T("RRULE:FREQ=MONTHLY;INTERVAL=%d;BYMONTHDAY=%d"), dwSpecific1, dwSpecific2);
		break;
	
	case TDIR_MONTH_SPECIFICDAY:
		sRecurrence.Format(_T("RRULE:FREQ=MONTHLY;INTERVAL=%d;BYDAY=%s"), dwSpecific2, FormatDayOfMonth(dwSpecific1));
		break;

	// -----------------------------------------------
	
	case TDIR_YEAR_EVERY: // TDIR_YEARLY
	case TDIR_YEAR_RECREATEAFTERNYEARS:
		sRecurrence.Format(_T("RRULE:FREQ=YEARLY;BYMONTH=%d"), dwSpecific1);
		break;
	
	case TDIR_YEAR_SPECIFICDAYMONTH:
		sRecurrence.Format(_T("RRULE:FREQ=YEARLY;BYMONTH=%d;BYDAY=%s"), dwSpecific2, FormatDayOfMonth(dwSpecific1));
		break;
	}

	ASSERT(!sRecurrence.IsEmpty());
	return sRecurrence;
}

CString CiCalExporter::FormatDayOfMonth(DWORD dwDOM)
{
	CString sDOM;
	int nDOW = HIWORD(dwDOM);
	int nWhich = LOWORD(dwDOM);

	switch (nDOW)
	{
	case 1:
		sDOM.Format(_T("%dSU"), nWhich);
		break;
		
	case 2: 
		sDOM.Format(_T("%dMO"), nWhich);
		break;
		
	case 3: 
		sDOM.Format(_T("%dTU"), nWhich);
		break;
		
	case 4: 
		sDOM.Format(_T("%dWE"), nWhich);
		break;
		
	case 5: 
		sDOM.Format(_T("%dTH"), nWhich);
		break;
		
	case 6: 
		sDOM.Format(_T("%dFR"), nWhich);
		break;
		
	case 7: 
		sDOM.Format(_T("%dSA"), nWhich);
		break;

	default:
		ASSERT(0);
	}

	return sDOM;
}

CString CiCalExporter::FormatDateTime(LPCTSTR szType, const COleDateTime& date, BOOL bStartOfDay)
{
	CString sDateTime;

	if (CDateHelper::DateHasTime(date))
	{
		sDateTime.Format(_T("%s;VALUE=DATE-TIME:%04d%02d%02dT%02d%02d%02d"), szType, 
						date.GetYear(), date.GetMonth(), date.GetDay(),
						date.GetHour(), date.GetMinute(), date.GetSecond());
	}
	else // no time component
	{
		COleDateTime dtDay(date);
		
		if (!bStartOfDay)
			dtDay.m_dt += 1.0;
		
		sDateTime.Format(_T("%s;VALUE=DATE-TIME:%04d%02d%02dT000000"), szType, 
						dtDay.GetYear(), dtDay.GetMonth(), dtDay.GetDay());	
	}
	
	return sDateTime;
}

void __cdecl CiCalExporter::WriteString(CStdioFile& fileOut, LPCTSTR lpszFormat, ...)
{
	ASSERT(AfxIsValidString(lpszFormat));
	CString sLine;
	
	va_list argList;
	va_start(argList, lpszFormat);
	sLine.FormatV(lpszFormat, argList);
	va_end(argList);
	
	sLine.TrimRight();
	
	// write line out in pieces no longer than 75 bytes
	while (sLine.GetLength() > 75)
	{
		CString sTemp = sLine.Left(75);
		sLine = sLine.Mid(75);
		
		fileOut.WriteString(sTemp);
		fileOut.WriteString(_T("\r\n ")); // note space at beginning of next line
	}
	
	// write out whatever's left
	fileOut.WriteString(sLine);
	fileOut.WriteString(_T("\r\n"));
}
