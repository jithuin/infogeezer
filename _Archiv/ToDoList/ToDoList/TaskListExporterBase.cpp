// TaskFileHtmlExporter.cpp: implementation of the CTaskListExporterBase class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "todolist.h"
#include "TasklistExporterBase.h"
#include "tdlschemadef.h"
#include "recurringtaskedit.h"
#include "tdcstatic.h"

#include "..\shared\xmlfile.h"
#include "..\shared\preferences.h"
#include "..\shared\enstring.h"
#include "..\shared\timehelper.h"

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

static TDC_ATTRIBUTE ATTRIB_ORDER[] = 
{
	TDCA_POSITION,
	TDCA_TASKNAME,
	TDCA_ID,
	TDCA_PARENTID,
	TDCA_PRIORITY,
	TDCA_RISK,	
	TDCA_PERCENT,
	TDCA_TIMEEST,
	TDCA_TIMESPENT,
	TDCA_CREATIONDATE,
	TDCA_CREATEDBY,
	TDCA_LASTMOD,
	TDCA_STARTDATE,
	TDCA_DUEDATE,
	TDCA_DONEDATE,
	TDCA_RECURRENCE, 
	TDCA_ALLOCTO,	
	TDCA_ALLOCBY,	
	TDCA_STATUS,	
	TDCA_CATEGORY,	
	TDCA_TAGS,		
	TDCA_EXTERNALID,
	TDCA_COST,		
	TDCA_VERSION,	
	TDCA_CUSTOMATTRIB,
	TDCA_FLAG,		
	TDCA_DEPENDENCY,
	TDCA_FILEREF,	
	TDCA_COMMENTS,	
};
static int NUMORDER = sizeof(ATTRIB_ORDER) / sizeof(TDC_ATTRIBUTE);

CTaskListExporterBase::CTaskListExporterBase() : WANTPOS(FALSE), ROUNDTIMEFRACTIONS(TRUE)
{
}

CTaskListExporterBase::~CTaskListExporterBase()
{
	
}

bool CTaskListExporterBase::ExportOutput(LPCTSTR szDestFilePath, const CString& sOutput) const
{
	if (sOutput.IsEmpty())
		return false;

	CStdioFileEx fileOut;
	
	if (fileOut.Open(szDestFilePath, CFile::modeCreate | CFile::modeWrite))
	{
		fileOut.WriteString(sOutput);
		return true;
	}
	
	// else
	return false;
}

bool CTaskListExporterBase::InitConsts(const ITaskList10* pTasks, LPCTSTR /*szDestFilePath*/, BOOL /*bSilent*/, 
									   IPreferences* pPrefs, LPCTSTR szKey)
{
	// we go straight for the application preferences
	szKey = _T("Preferences");
		
	ROUNDTIMEFRACTIONS = pPrefs->GetProfileInt(szKey, _T("RoundTimeFractions"), FALSE);

	if (pTasks)
	{
		// detect whether we want task position
		HTASKITEM hFirstTask = pTasks->GetFirstTask(NULL);
		WANTPOS = (hFirstTask && pTasks->TaskHasAttribute(hFirstTask, TDL_TASKPOS));

		// get map of _enabled_ custom attribute labels
		MAPCUSTID2LABEL.RemoveAll();
		int nNumCust = pTasks->GetCustomAttributeCount();

		for (int nCust = 0; nCust < nNumCust; nCust++)
		{
			if (_ttoi(pTasks->GetCustomAttributeValue(nCust, TDL_CUSTOMATTRIBENABLED)))
			{
				MAPCUSTID2LABEL[pTasks->GetCustomAttributeID(nCust)] = pTasks->GetCustomAttributeLabel(nCust);
			}
		}
	}

	BuildAttribList(pTasks, NULL);

	return true;
}

bool CTaskListExporterBase::Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, 
								   IPreferences* pPrefs, LPCTSTR szKey)
{
	ASSERT(pSrcTaskFile->GetTaskListCount());

	if (pSrcTaskFile->GetTaskListCount() == 0)
		return false;

	const ITaskList10* pTasks10 = GetITLInterface<ITaskList10>(pSrcTaskFile->GetTaskList(0), IID_TASKLIST10);
	ASSERT(pTasks10);
	
	if (pTasks10 == NULL)
		return false;

	if (!InitConsts(pTasks10, szDestFilePath, bSilent, pPrefs, szKey))
		return false;
	
	CString sOutput;
	
	for (int nTaskList = 0; nTaskList < pSrcTaskFile->GetTaskListCount(); nTaskList++)
	{
		pTasks10 = GetITLInterface<ITaskList10>(pSrcTaskFile->GetTaskList(nTaskList), IID_TASKLIST10);
		ASSERT(pTasks10);
		
		if (pTasks10 == NULL)
			return false;

		// add title block
		sOutput += FormatTitle(pTasks10);
		
		// then header
		sOutput += FormatHeader();
		
		// then tasks
		sOutput += ExportTaskAndSubtasks(pTasks10, NULL, 0);
	}
	
	return ExportOutput(szDestFilePath, sOutput);
}

bool CTaskListExporterBase::Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, 
								   IPreferences* pPrefs, LPCTSTR szKey)
{
	const ITaskList10* pTasks10 = GetITLInterface<ITaskList10>(pSrcTaskFile, IID_TASKLIST10);
	ASSERT(pTasks10);
	
	if (pTasks10 == NULL)
		return false;

	if (!InitConsts(pTasks10, szDestFilePath, bSilent, pPrefs, szKey))
		return false;

	// add title block
	CString sOutput = FormatTitle(pTasks10);
	
	// then header
	sOutput += FormatHeader();
	
	// then tasks
	sOutput += ExportTaskAndSubtasks(pTasks10, NULL, 0);
	
	return ExportOutput(szDestFilePath, sOutput);
}

CString CTaskListExporterBase::FormatHeader() const
{
	CString sHeader;

	// make sure these are in the order we want
	for (int nAtt = 0; nAtt < NUMORDER; nAtt++)
	{
		TDC_ATTRIBUTE attrib = ATTRIB_ORDER[nAtt];
		int nFind = FindAttribute(attrib);

		if (nFind != -1) // found
		{
			if (attrib == TDCA_CUSTOMATTRIB)
			{
				POSITION pos = MAPCUSTID2LABEL.GetStartPosition();

				while (pos)
				{
					CString sCustID, sCustLabel;
					MAPCUSTID2LABEL.GetNextAssoc(pos, sCustID, sCustLabel);

					sHeader += FormatHeaderItem(attrib, sCustLabel);
				}
			}
			else
			{
				CString sLabel = ARRLABELS[nFind];
				sHeader += FormatHeaderItem(attrib, sLabel);
			}
			
		}
	}

	return sHeader;
}

CString CTaskListExporterBase::ExportTaskAndSubtasks(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const
{
	CString sTask = ExportTask(pTasks, hTask, nDepth); // virtual call
	
	// sub-tasks
	if (pTasks->GetFirstTask(hTask))
		sTask += ExportSubtasks(pTasks, hTask, nDepth); // virtual call
	
	return sTask;
}

CString CTaskListExporterBase::ExportSubtasks(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const
{
	CString sSubtasks;
	HTASKITEM hSubTask = pTasks->GetFirstTask(hTask);
	
	while (hSubTask)
	{
		sSubtasks += ENDL;
		sSubtasks += ExportTaskAndSubtasks(pTasks, hSubTask, nDepth + 1);
		
		hSubTask = pTasks->GetNextTask(hSubTask);
	}

	return sSubtasks;
}

CString CTaskListExporterBase::ExportTask(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const
{
	CString sTask;

	// if depth == 0 then we're at the root so just add sub-tasks
	if (nDepth > 0)
	{
		ASSERT (hTask);

		// make sure these are in the order we want
		for (int nAtt = 0; nAtt < NUMORDER; nAtt++)
		{
			TDC_ATTRIBUTE attrib = ATTRIB_ORDER[nAtt];
			int nFind = FindAttribute(attrib);

			if (nFind != -1) // found
			{
				CString sLabel = ARRLABELS[nFind];
				sTask += FormatAttribute(pTasks, hTask, nDepth, attrib,	sLabel);
			}
		}

		// notes section
		if (!pTasks->IsTaskDone(hTask))
			sTask += GetSpaceForNotes();
			
		sTask += ENDL;
	}

	return sTask;
}

CString CTaskListExporterBase::FormatAttribute(const ITaskList10* pTasks, HTASKITEM hTask, int /*nDepth*/,
											   TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel) const
{
	CString sItem;

	switch (nAttrib)
	{
	case TDCA_ALLOCBY: 
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKALLOCBY);
		break;
		
	case TDCA_ALLOCTO:
		sItem = FormatAllocToList(pTasks, hTask, sAttribLabel);
		break;
		
	case TDCA_CATEGORY:
		sItem = FormatCategoryList(pTasks, hTask, sAttribLabel);
		break;
		
	case TDCA_COLOR:
		break;
		
	case TDCA_COMMENTS:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKCOMMENTS);
		break;
		
	case TDCA_COST:			
		// handle explicitly to localise decimal point
		if (WantAttribute(TDCA_COST))
		{
			BOOL bHasCalc = pTasks->TaskHasAttribute(hTask, TDL_TASKCALCCOST);
			double dCost = pTasks->GetTaskCost(hTask, bHasCalc);
			
			sItem = FormatAttribute(TDCA_COST, sAttribLabel, Misc::Format(dCost, 2));
		}
		break;
		
	case TDCA_CREATIONDATE:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKCREATIONDATESTRING);
		break;
		
	case TDCA_CREATEDBY:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKCREATEDBY);
		break;
		
	case TDCA_CUSTOMATTRIB:
		sItem = FormatCustomAttributes(pTasks, hTask);
		break;
		
	case TDCA_DEPENDENCY:	
		sItem = FormatDependencyList(pTasks, hTask, sAttribLabel);
		break;
		
	case TDCA_DONEDATE:
	case TDCA_DONETIME:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKDONEDATESTRING);
		break;
		
	case TDCA_DUEDATE:
	case TDCA_DUETIME:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKDUEDATESTRING);
		break;
		
	case TDCA_EXTERNALID:	
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKEXTERNALID);
		break;
		
	case TDCA_FILEREF:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKFILEREFPATH);
		break;
		
	case TDCA_FLAG:
		sItem = (pTasks->IsTaskFlagged(hTask) ? sAttribLabel : _T(""));
		break;
		
	case TDCA_ICON:
		break;
		
	case TDCA_ID:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKID); 
		break;
		
	case TDCA_LASTMOD:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKLASTMODSTRING);
		break;
		
	case TDCA_PARENTID:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKPARENTID); 
		break;
		
	case TDCA_PERCENT:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKPERCENTDONE);
		break;
		
	case TDCA_POSITION:
		if (WANTPOS)
			sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKPOS);
		break;
		
	case TDCA_PRIORITY:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKHIGHESTPRIORITY, TDL_TASKPRIORITY);
		break;
		
	case TDCA_RECURRENCE:	
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKRECURRENCE);
		break;
		
	case TDCA_RISK:			
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKHIGHESTRISK, TDL_TASKRISK);
		break;
		
	case TDCA_STARTDATE:
	case TDCA_STARTTIME:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKSTARTDATESTRING);
		break;
		
	case TDCA_STATUS:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKSTATUS);
		break;
		
	case TDCA_TAGS:
		sItem = FormatTagList(pTasks, hTask, sAttribLabel);
		break;
		
	case TDCA_TASKNAME:
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKTITLE);
		break;
		
	case TDCA_TIMEEST:
		// handle explicitly to localise decimal point
		if (WantAttribute(TDCA_TIMEEST))
		{
			TCHAR cUnits = 0;
			double dTime = pTasks->GetTaskTimeEstimate(hTask, cUnits, TRUE);
			
			sItem = FormatAttribute(nAttrib, sAttribLabel, FormatTime(dTime, cUnits));
		}
		break;
		
	case TDCA_TIMESPENT:
		// handle explicitly to localise decimal point
		if (WantAttribute(TDCA_TIMESPENT))
		{
			TCHAR cUnits = 0;
			double dTime = pTasks->GetTaskTimeSpent(hTask, cUnits, TRUE);

			sItem = FormatAttribute(nAttrib, sAttribLabel, FormatTime(dTime, cUnits));
		}
		break;
		
	case TDCA_VERSION:		
		sItem = FormatAttribute(pTasks, hTask, nAttrib, sAttribLabel, TDL_TASKVERSION);
		break;
	}

    return sItem;
}

CString CTaskListExporterBase::FormatTime(double dTime, TCHAR cUnits) const
{
	return CTimeHelper().FormatTime(dTime, cUnits, (ROUNDTIMEFRACTIONS ? 0 : 2));
}

CString CTaskListExporterBase::FormatAttribute(const ITaskList10* pTasks, HTASKITEM hTask, TDC_ATTRIBUTE nAttrib, 
											const CString& sAttribLabel, LPCTSTR szAttribName, LPCTSTR szAltAttribName) const
{
	CString sAttribText;

	if (WantAttribute(nAttrib))
	{
		// get the attribute name that we will be using
		CString sAttribName;

		if (pTasks->TaskHasAttribute(hTask, szAttribName) || (szAltAttribName == NULL))
		{
			sAttribName = szAttribName;
		}
		else if (szAltAttribName /*&& pTasks->TaskHasAttribute(hTask, szAltAttribName)*/)
		{
			sAttribName = szAltAttribName;
		}

		sAttribText = FormatAttribute(nAttrib, sAttribLabel, pTasks->GetTaskAttribute(hTask, sAttribName));
	}

	return sAttribText;
}

CString CTaskListExporterBase::FormatCategoryList(const ITaskList10* pTasks, HTASKITEM hTask, const CString& sAttribLabel) const
{
	CString sAttribs;
	int nItemCount = pTasks->GetTaskCategoryCount(hTask);
	
	for (int nItem = 0; nItem < nItemCount; nItem++)
	{
		if (!sAttribs.IsEmpty())
			sAttribs += '+';

		sAttribs += pTasks->GetTaskCategory(hTask, nItem);
	}
	
	return FormatAttribute(TDCA_CATEGORY, sAttribLabel, sAttribs);
}

CString CTaskListExporterBase::FormatAllocToList(const ITaskList10* pTasks, HTASKITEM hTask, const CString& sAttribLabel) const
{
	CString sAttribs;
	int nItemCount = pTasks->GetTaskAllocatedToCount(hTask);
	
	for (int nItem = 0; nItem < nItemCount; nItem++)
	{
		if (!sAttribs.IsEmpty())
			sAttribs += '+';
		
		sAttribs += pTasks->GetTaskAllocatedTo(hTask, nItem);
	}
	
	return FormatAttribute(TDCA_CATEGORY, sAttribLabel, sAttribs);
}

CString CTaskListExporterBase::FormatTagList(const ITaskList10* pTasks, HTASKITEM hTask, const CString& sAttribLabel) const
{
	CString sAttribs;
	int nItemCount = pTasks->GetTaskTagCount(hTask);
	
	for (int nItem = 0; nItem < nItemCount; nItem++)
	{
		if (!sAttribs.IsEmpty())
			sAttribs += '+';
		
		sAttribs += pTasks->GetTaskTag(hTask, nItem);
	}
	
	return FormatAttribute(TDCA_CATEGORY, sAttribLabel, sAttribs);
}

CString CTaskListExporterBase::FormatDependencyList(const ITaskList10* pTasks, HTASKITEM hTask, const CString& sAttribLabel) const
{
	CString sAttribs;
	int nItemCount = pTasks->GetTaskDependencyCount(hTask);
	
	for (int nItem = 0; nItem < nItemCount; nItem++)
	{
		if (!sAttribs.IsEmpty())
			sAttribs += '+';
		
		sAttribs += pTasks->GetTaskDependency(hTask, nItem);
	}
	
	return FormatAttribute(TDCA_CATEGORY, sAttribLabel, sAttribs);
}

CString CTaskListExporterBase::FormatCustomAttributes(const ITaskList10* pTasks, HTASKITEM hTask) const
{
	CString sCustAttribs;
	POSITION pos = MAPCUSTID2LABEL.GetStartPosition();

	while (pos)
	{
		CString sCustID, sCustLabel;
		MAPCUSTID2LABEL.GetNextAssoc(pos, sCustID, sCustLabel);

		CString sCustValue = pTasks->GetTaskCustomAttributeData(hTask, sCustID);

		if (!sCustValue.IsEmpty())
		{
			CString sCustAttrib = FormatAttribute(TDCA_CUSTOMATTRIB, sCustLabel, sCustValue);
			sCustAttribs += sCustAttrib;
		}
	}

	return sCustAttribs;
}

void CTaskListExporterBase::BuildAttribList(const ITaskList10* pTasks, HTASKITEM hTask)
{
	if (hTask)
	{
		CheckAddAttribtoList(pTasks, hTask, TDCA_POSITION,	TDL_TASKPOS);
		CheckAddAttribtoList(pTasks, hTask, TDCA_TASKNAME,	TDL_TASKTITLE);
		CheckAddAttribtoList(pTasks, hTask, TDCA_ID,		TDL_TASKID);
		CheckAddAttribtoList(pTasks, hTask, TDCA_PARENTID,	TDL_TASKPARENTID);
		CheckAddAttribtoList(pTasks, hTask, TDCA_PRIORITY,	TDL_TASKPRIORITY);
		CheckAddAttribtoList(pTasks, hTask, TDCA_RISK,		TDL_TASKRISK);
		CheckAddAttribtoList(pTasks, hTask, TDCA_PERCENT,	TDL_TASKPERCENTDONE);
		CheckAddAttribtoList(pTasks, hTask, TDCA_TIMEEST,	TDL_TASKTIMEESTIMATE);
		CheckAddAttribtoList(pTasks, hTask, TDCA_TIMESPENT, TDL_TASKTIMESPENT);
		CheckAddAttribtoList(pTasks, hTask, TDCA_CREATIONDATE, TDL_TASKCREATIONDATESTRING);
		CheckAddAttribtoList(pTasks, hTask, TDCA_CREATEDBY, TDL_TASKCREATEDBY);
		CheckAddAttribtoList(pTasks, hTask, TDCA_LASTMOD,	TDL_TASKLASTMODSTRING);
		CheckAddAttribtoList(pTasks, hTask, TDCA_STARTDATE, TDL_TASKSTARTDATESTRING);
		CheckAddAttribtoList(pTasks, hTask, TDCA_DUEDATE,	TDL_TASKDUEDATESTRING);
		CheckAddAttribtoList(pTasks, hTask, TDCA_DONEDATE,	TDL_TASKDONEDATESTRING);
		CheckAddAttribtoList(pTasks, hTask, TDCA_RECURRENCE, TDL_TASKRECURRENCE);
		CheckAddAttribtoList(pTasks, hTask, TDCA_ALLOCTO,	TDL_TASKALLOCTO);
		CheckAddAttribtoList(pTasks, hTask, TDCA_ALLOCBY,	TDL_TASKALLOCBY);
		CheckAddAttribtoList(pTasks, hTask, TDCA_STATUS,	TDL_TASKSTATUS);
		CheckAddAttribtoList(pTasks, hTask, TDCA_CATEGORY,	TDL_TASKCATEGORY);
		CheckAddAttribtoList(pTasks, hTask, TDCA_TAGS,		TDL_TASKTAG);
		CheckAddAttribtoList(pTasks, hTask, TDCA_EXTERNALID, TDL_TASKEXTERNALID);
		CheckAddAttribtoList(pTasks, hTask, TDCA_COST,		TDL_TASKCOST);
		CheckAddAttribtoList(pTasks, hTask, TDCA_VERSION,	TDL_TASKVERSION);
		CheckAddAttribtoList(pTasks, hTask, TDCA_FLAG,		TDL_TASKFLAG);
		CheckAddAttribtoList(pTasks, hTask, TDCA_DEPENDENCY, TDL_TASKDEPENDENCY);
		CheckAddAttribtoList(pTasks, hTask, TDCA_FILEREF,	TDL_TASKFILEREFPATH);
		CheckAddAttribtoList(pTasks, hTask, TDCA_COMMENTS,	TDL_TASKCOMMENTS);
	}
	else // root => initialize arrays
	{
		ARRATTRIBUTES.RemoveAll();
		ARRLABELS.RemoveAll();

		// always add custom attribs
		if (MAPCUSTID2LABEL.GetCount())
		{
			ARRATTRIBUTES.Add(TDCA_CUSTOMATTRIB);
			ARRLABELS.Add(_T(""));
		}
	}

	// subtasks
	hTask = pTasks->GetFirstTask(hTask);

	while (hTask) // at least one sub-task
	{
		BuildAttribList(pTasks, hTask);

		// next subtask
		hTask = pTasks->GetNextTask(hTask);
	}
}

void CTaskListExporterBase::CheckAddAttribtoList(const ITaskList10* pTasks, HTASKITEM hTask, 
												TDC_ATTRIBUTE attrib, LPCTSTR szAttribName)
{
	if (pTasks->TaskHasAttribute(hTask, szAttribName) && !WantAttribute(attrib))
	{
		ARRATTRIBUTES.Add(attrib);

		// translate label once only
		CEnString sLabel(GetAttribLabel(attrib));
		sLabel.Translate();

		ARRLABELS.Add(sLabel);
	}
}

BOOL CTaskListExporterBase::WantAttribute(TDC_ATTRIBUTE attrib) const
{
	return (FindAttribute(attrib) != -1);
}

int CTaskListExporterBase::FindAttribute(TDC_ATTRIBUTE attrib) const
{
	int nAttrib = ARRATTRIBUTES.GetSize();
	
	while (nAttrib--)
	{
		if (ARRATTRIBUTES[nAttrib] == attrib)
			return nAttrib; 
	}

	// not found
	return -1;
}

CString CTaskListExporterBase::GetAttribLabel(TDC_ATTRIBUTE attrib)
{
	for (int nAttrib = 0; nAttrib < ATTRIB_COUNT; nAttrib++)
	{
		const TDCATTRIBUTE& att = ATTRIBUTES[nAttrib];

		if (attrib == att.attrib)
			return CEnString(att.nAttribResID);
	}

	return _T("");
}
