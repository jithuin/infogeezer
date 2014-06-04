// MLOExporter.cpp: implementation of the CMLOExporter class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "MLOImport.h"
#include "MLOExporter.h"

#include "..\shared\xmlfileex.h"
#include "..\shared\datehelper.h"
#include "..\shared\timehelper.h"
//#include "..\shared\localizer.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMLOExporter::CMLOExporter()
{
	
}

CMLOExporter::~CMLOExporter() 
{
	
}

void CMLOExporter::SetLocalizer(ITransText* /*pTT*/)
{
	//CLocalizer::Initialize(pTT);
}

bool CMLOExporter::Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL /*bSilent*/, IPreferences* /*pPrefs*/, LPCTSTR /*szKey*/)
{
	CXmlFile fileDest(_T("MyLifeOrganized-xml"));
	
	const ITaskList7* pITL7 = GetITLInterface<ITaskList7>(pSrcTaskFile, IID_TASKLIST7);
	ASSERT (pITL7);
	
	// export tasks
	CXmlItem* pXITasks = fileDest.AddItem(_T("TaskTree"));
	
	if (!ExportTask(pITL7, pSrcTaskFile->GetFirstTask(), pXITasks))
		return false;
	
	// export resource allocations
	ExportPlaces(pITL7, fileDest.Root());
	
	// save result
	return (fileDest.Save(szDestFilePath, SFE_MULTIBYTE) != FALSE);
}

bool CMLOExporter::Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL /*bSilent*/, IPreferences* /*pPrefs*/, LPCTSTR /*szKey*/)
{
	CXmlFile fileDest(_T("MyLifeOrganized-xml"));
	
	// export tasks
	CXmlItem* pXITasks = fileDest.AddItem(_T("TaskTree"));
	
	for (int nTaskList = 0; nTaskList < pSrcTaskFile->GetTaskListCount(); nTaskList++)
	{
		const ITaskList7* pITL7 = GetITLInterface<ITaskList7>(pSrcTaskFile->GetTaskList(nTaskList), IID_TASKLIST7);
		
		if (pITL7)
		{
			// export tasks
			if (!ExportTask(pITL7, pITL7->GetFirstTask(), pXITasks))
				return false;
			
			// export resource allocations
			ExportPlaces(pITL7, fileDest.Root());
		}
	}
	
	// save result
	return (fileDest.Save(szDestFilePath, SFE_MULTIBYTE) != FALSE);
}

bool CMLOExporter::ExportTask(const ITaskList7* pSrcTaskFile, HTASKITEM hTask, 
							  CXmlItem* pXIDestParent)
{
	if (!hTask)
		return true;
	
	// create a new item corresponding to pXITask at the dest
	CXmlItem* pXIDestItem = pXIDestParent->AddItem(_T("TaskNode"));
	
	if (!pXIDestItem)
		return false;
	
	// copy across the appropriate attributes
	pXIDestItem->AddItem(_T("Caption"), pSrcTaskFile->GetTaskTitle(hTask));
	
	// priority
	int nPriority = pSrcTaskFile->GetTaskPriority(hTask, FALSE);
	int nImportance = max(nPriority, 0) * 10;
	
	pXIDestItem->AddItem(_T("Importance"), nImportance);
	
	// dates
	time_t tDue = pSrcTaskFile->GetTaskDueDate(hTask, FALSE);
	time_t tDone = pSrcTaskFile->GetTaskDoneDate(hTask);
	
	if (tDone)
		pXIDestItem->AddItem(_T("CompletionDateTime"), CDateHelper::FormatDate(tDone, DHFD_ISO));
	
	if (tDue)
		pXIDestItem->AddItem(_T("DueDateTime"), CDateHelper::FormatDate(tDue, DHFD_ISO));
	
	// time estimate
	TCHAR cTimeUnits;
	double dTimeEst = pSrcTaskFile->GetTaskTimeEstimate(hTask, cTimeUnits, FALSE);
	
	if (dTimeEst > 0.0)
		pXIDestItem->AddItem(_T("EstimateMax"), CTimeHelper().GetTime(dTimeEst, cTimeUnits, THU_DAYS));
	
	// comments
	LPCTSTR szComments = pSrcTaskFile->GetTaskComments(hTask);
	
	if (szComments && *szComments)
		pXIDestItem->AddItem(_T("Note"), szComments);
	
	// copy across first child
	if (!ExportTask(pSrcTaskFile, pSrcTaskFile->GetFirstTask(hTask), pXIDestItem))
		return false;
	
	// copy across first sibling
	return ExportTask(pSrcTaskFile, pSrcTaskFile->GetNextTask(hTask), pXIDestParent);
}

void CMLOExporter::BuildPlacesMap(const ITaskList7* pSrcTaskFile, HTASKITEM hTask, CMapStringToString& mapPlaces)
{
	if (!hTask)
		return;
	
	int nCat = pSrcTaskFile->GetTaskCategoryCount(hTask);
	
	while (nCat--)
	{
		CString sCat = pSrcTaskFile->GetTaskCategory(hTask, nCat);
		CString sCatUpper(sCat);
		sCat.MakeUpper();
		
		mapPlaces[sCatUpper] = sCat;
	}
	
	// children
	BuildPlacesMap(pSrcTaskFile, pSrcTaskFile->GetFirstTask(hTask), mapPlaces);
	
	// siblings
	BuildPlacesMap(pSrcTaskFile, pSrcTaskFile->GetNextTask(hTask), mapPlaces);
}

void CMLOExporter::ExportPlaces(const ITaskList7* pSrcTaskFile, CXmlItem* pDestPrj)
{
	CMapStringToString mapPlaces;
	BuildPlacesMap(pSrcTaskFile, pSrcTaskFile->GetFirstTask(), mapPlaces);
	
	if (mapPlaces.GetCount())
	{
		CXmlItem* pXIResources = pDestPrj->AddItem(_T("PlacesList"));
		CString sPlace, sPlaceUpper;
		
		POSITION pos = mapPlaces.GetStartPosition();
		
		while (pos)
		{
			mapPlaces.GetNextAssoc(pos, sPlaceUpper, sPlace);
			
			CXmlItem* pXIRes = pXIResources->AddItem(_T("TaskPlace"));
			
			if (pXIRes)
				pXIRes->AddItem(_T("Caption"), sPlace);
		}
	}
}
