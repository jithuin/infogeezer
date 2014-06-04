// TaskListTxtExporter.cpp: implementation of the CTaskListCsvExporter class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "todolist.h"
#include "TaskListcsvExporter.h"
#include "tdlschemadef.h"
#include "recurringtaskedit.h"

#include "..\shared\Preferences.h"
#include "..\shared\enstring.h"

#include "..\3rdparty\stdiofileex.h"

#include <locale.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

////////////////////////////////////////////////////////////////////// 
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

static LPCTSTR SPACE = _T(" ");
static LPCTSTR ENDL = _T("\n");
static LPCTSTR ONEDBLQUOTE = _T("\"");
static LPCTSTR TWODBLQUOTE = _T("\"\"");

CTaskListCsvExporter::CTaskListCsvExporter()
{
}

CTaskListCsvExporter::~CTaskListCsvExporter()
{
	
}

bool CTaskListCsvExporter::Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey)
{
	return CTaskListExporterBase::Export(pSrcTaskFile, szDestFilePath, bSilent, pPrefs, szKey);
}

bool CTaskListCsvExporter::Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey)
{
	return CTaskListExporterBase::Export(pSrcTaskFile, szDestFilePath, bSilent, pPrefs, szKey);
}

CString CTaskListCsvExporter::ExportTask(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const
{
	CString sTask = CTaskListExporterBase::ExportTask(pTasks, hTask, nDepth);

	// prevent double line feeds
	sTask.TrimRight(ENDL);

	return sTask;
}

bool CTaskListCsvExporter::InitConsts(const ITaskList10* pTasks, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey)
{
	// base consts
	if (!CTaskListExporterBase::InitConsts(pTasks, szDestFilePath, bSilent, pPrefs, szKey))
		return false;

	// default delim
	DELIM = Misc::GetListSeparator();

	// we go straight for the application preferences
	szKey = _T("Preferences");

	if (pPrefs->GetProfileInt(szKey, _T("UseSpaceIndents"), TRUE))
		INDENT = CString(' ', pPrefs->GetProfileInt(szKey, _T("TextIndent"), 2));
	else
		INDENT = "  "; // don't use tabs - excel strips them off

	CTDLCsvImportExportDlg dialog(szDestFilePath, ARRATTRIBUTES, pPrefs, szKey);
	
	if (!bSilent && dialog.DoModal() != IDOK)
		return false;
	
	dialog.GetColumnMapping(m_aColumnMapping);
	DELIM = dialog.GetDelimiter();

	return true;
}

CString CTaskListCsvExporter::FormatHeaderItem(TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel) const
{
	CString sHeader = (sAttribLabel + DELIM);

	// extra processing
	switch (nAttrib)
	{
	case TDCA_TIMEEST:
	case TDCA_TIMESPENT:
		// Times are special case: Need another column for units
		sHeader += DELIM;
		break;
	}

	return sHeader;
}

CString CTaskListCsvExporter::FormatAttribute(TDC_ATTRIBUTE nAttrib, const CString& /*sAttribLabel*/, const CString& sValue) const
{
	// we always export regardless of whether there is a value or not
	BOOL bNeedQuoting = (nAttrib == TDCA_COMMENTS);
	CString sAttrib(sValue);
	
	// double up quotes
	if (sAttrib.Find(ONEDBLQUOTE) != -1)
	{
		sAttrib.Replace(ONEDBLQUOTE, TWODBLQUOTE);
		bNeedQuoting = TRUE;
	}
	
	// look for commas or whatever is the list delimiter
	if (sAttrib.Find(DELIM) != -1)
		bNeedQuoting = TRUE;
	
	if (bNeedQuoting)
		sAttrib = ONEDBLQUOTE + sAttrib + ONEDBLQUOTE;
	
	// replace carriage returns
	sAttrib.Replace(ENDL, SPACE);
	sAttrib += DELIM;

	return sAttrib;
}

CString CTaskListCsvExporter::FormatAttribute(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth, 
											  TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel) const
{
	// base processing
	CString sItem = CTaskListExporterBase::FormatAttribute(pTasks, hTask, nDepth, nAttrib, sAttribLabel);

	// extra processing
	if (!sItem.IsEmpty())
	{
		switch (nAttrib)
		{
		case TDCA_TASKNAME:
			{
				CString sIndent;

				// indent task title
				while (--nDepth)
					sIndent += INDENT;
				
				sItem = sIndent + sItem;
			}
			break;
			
		case TDCA_TIMEEST:
		case TDCA_TIMESPENT:
			{
				TCHAR szUnits[2] = { 0 };
				double dTime = (nAttrib == TDCA_TIMEEST) ?
								pTasks->GetTaskTimeEstimate(hTask, szUnits[0], TRUE) :
								pTasks->GetTaskTimeSpent(hTask, szUnits[0], TRUE);
				
				CString sTime;
				sTime.Format(_T("%.*f"), (ROUNDTIMEFRACTIONS ? 0 : 2), dTime);

				sItem = FormatAttribute(nAttrib, sAttribLabel, sTime) + 
						FormatAttribute(nAttrib, sAttribLabel, szUnits);
			}
			break;

		// we must include parent ID even if it's a root task
		case TDCA_PARENTID:
			if (pTasks->GetTaskParentID(hTask) == 0)
				sItem = FormatAttribute(nAttrib, sAttribLabel, _T(""));
			break;
		}
	}

    return sItem;
}

