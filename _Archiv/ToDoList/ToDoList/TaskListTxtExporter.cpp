// TaskListTxtExporter.cpp: implementation of the CTaskListTxtExporter class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "todolist.h"
#include "TaskListTxtExporter.h"
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

static LPCTSTR ENDL = _T("\n");

CTaskListTxtExporter::CTaskListTxtExporter()
{
}

CTaskListTxtExporter::~CTaskListTxtExporter()
{
	
}

bool CTaskListTxtExporter::Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey)
{
	return CTaskListExporterBase::Export(pSrcTaskFile, szDestFilePath, bSilent, pPrefs, szKey);
}

bool CTaskListTxtExporter::Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey)
{
	return CTaskListExporterBase::Export(pSrcTaskFile, szDestFilePath, bSilent, pPrefs, szKey);
}

bool CTaskListTxtExporter::InitConsts(const ITaskList10* pTasks, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey)
{
	// base consts
	if (!CTaskListExporterBase::InitConsts(pTasks, szDestFilePath, bSilent, pPrefs, szKey))
		return false;

	// then ours
	szKey = _T("Preferences");

	if (pPrefs->GetProfileInt(szKey, _T("UseSpaceIndents"), TRUE))
		INDENT = CString(' ', pPrefs->GetProfileInt(szKey, _T("TextIndent"), 2));
	else
		INDENT = '\t';
	
	if (pPrefs->GetProfileInt(szKey, _T("ExportSpaceForNotes"), FALSE))
	{
		TEXTNOTES.Empty();
		int nLine = pPrefs->GetProfileInt(szKey, _T("LineSpaces"), 8);
		
		if (nLine > 0)
		{
			while (nLine--)
				TEXTNOTES += "\n";
		}
	}

	return true;
}

CString CTaskListTxtExporter::FormatAttribute(TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel, const CString& sValue) const 
{
	CString sFmtAttrib;

	if (!sValue.IsEmpty())
	{
		switch (nAttrib)
		{
		case TDCA_POSITION:
		case TDCA_TASKNAME:
			sFmtAttrib.Format(_T("%s "), sValue);
			break;

		// all else
		default:
			sFmtAttrib.Format(_T("(%s: %s) "), sAttribLabel, sValue);
			break;
		}
	}

	return sFmtAttrib;
}

CString CTaskListTxtExporter::FormatAttribute(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth, 
											  TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel) const
{
	// base processing
	CString sItem = CTaskListExporterBase::FormatAttribute(pTasks, hTask, nDepth, nAttrib, sAttribLabel);

	// extra processing
	switch (nAttrib)
	{
	case TDCA_COMMENTS:
	case TDCA_FILEREF:
		{
			// indent
			for (int nTab = 0; nTab < nDepth; nTab++)
				sItem = INDENT + sItem;
		}
		break;

	case TDCA_PROJNAME:
		{
			CString sTitle = pTasks->GetReportTitle();
			CString sDate = pTasks->GetReportDate();
			
			if (!sTitle.IsEmpty())
				sItem.Format(_T("%s\n%s\n"), sTitle, sDate);

			else if (!sDate.IsEmpty())
				sItem.Format(_T("%s\n"), sDate);
		}
		break;

	case TDCA_PARENTID:
		// ignore if not set
		if (pTasks->GetTaskParentID(hTask) == 0)
			sItem.Empty();
		break;
	}

    return sItem;
}

CString CTaskListTxtExporter::ExportTask(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const
{
	return CTaskListExporterBase::ExportTask(pTasks, hTask, nDepth);
}

