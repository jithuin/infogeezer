// GPImporter.cpp: implementation of the CGPImporter class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "GPExport.h"
#include "GPImporter.h"

#include "..\shared\xmlfileex.h"
#include "..\shared\datehelper.h"
#include "..\shared\enstring.h"
//#include "..\shared\localizer.h"

#include "..\todolist\tdlschemadef.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

const UINT ONEDAY = 24 * 60 * 60;

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CGPImporter::CGPImporter()
{

}

CGPImporter::~CGPImporter()
{

}

void CGPImporter::SetLocalizer(ITransText* /*pTT*/)
{
	//CLocalizer::Initialize(pTT);
}

bool CGPImporter::Import(LPCTSTR szSrcFilePath, ITaskList* pDestTaskFile, BOOL /*bSilent*/, IPreferences* /*pPrefs*/, LPCTSTR /*szKey*/)
{
	CXmlFile fileSrc;

	if (!fileSrc.Load(szSrcFilePath, _T("project")))
		return false;

	const CXmlItem* pXISrcTasks = fileSrc.GetItem(_T("tasks"));

	if (!pXISrcTasks) // must exist
		return false;

	const CXmlItem* pXISrcTask = pXISrcTasks->GetItem(_T("task"));

	if (!pXISrcTask) // must exist
		return false;

	ITaskList8* pITL8 = GetITLInterface<ITaskList8>(pDestTaskFile, IID_TASKLIST8);

	if (!ImportTask(pXISrcTask, pITL8, NULL))
		return false;

	// fix up resource allocations
	FixupResourceAllocations(fileSrc.Root(), pITL8);

	// and dependencies
	FixupDependencies(pXISrcTask, pITL8);

	return true; // no tasks to import
}

bool CGPImporter::ImportTask(const CXmlItem* pXISrcTask, ITaskList8* pDestTaskFile, HTASKITEM htDestParent)
{
	if (!pXISrcTask)
		return true;

	CString sName = pXISrcTask->GetItemValue(_T("name"));
	DWORD dwID = GetTDLTaskID(pXISrcTask->GetItemValueI(_T("id")));
	ASSERT(dwID);

	HTASKITEM hTask = pDestTaskFile->NewTask(sName, htDestParent, dwID);
	ASSERT (hTask);

	if (!hTask)
		return false;

	// completion
	int nPercentDone = pXISrcTask->GetItemValueI(_T("complete"));
	pDestTaskFile->SetTaskPercentDone(hTask, (unsigned char)nPercentDone);

	// dates
	time_t tStart;
	
	if (CDateHelper::DecodeDate(pXISrcTask->GetItemValue(_T("start")), tStart))
	{
		pDestTaskFile->SetTaskStartDate(hTask, tStart);

		int nDuration = pXISrcTask->GetItemValueI(_T("duration"));

		// only add duration to leaf tasks else it'll double up
		if (nDuration && !pXISrcTask->HasItem(_T("task")))
		{
			if (nPercentDone == 100)
			{
				pDestTaskFile->SetTaskDoneDate(hTask, tStart + (nDuration - 1) * ONEDAY); // gp dates are inclusive
				pDestTaskFile->SetTaskTimeSpent(hTask, nDuration, 'D');
			}
			else
			{
				pDestTaskFile->SetTaskDueDate(hTask, tStart + (nDuration - 1) * ONEDAY); // gp dates are inclusive
				pDestTaskFile->SetTaskTimeEstimate(hTask, nDuration, 'D');
			}
		}
	}

	// priority
	int nPriority = pXISrcTask->GetItemValueI(_T("priority"));
	pDestTaskFile->SetTaskPriority(hTask, (unsigned char)(nPriority * 3 + 2)); // 2, 5, 8

	// file ref
	CString sFileRef = pXISrcTask->GetItemValue(_T("webLink"));
	sFileRef.TrimLeft();

	if (!sFileRef.IsEmpty())
	{
		// decode file paths
		if (sFileRef.Find(_T("file://")) == 0)
		{
			sFileRef = sFileRef.Mid(7);
			sFileRef.Replace(_T("%20"), _T(""));
		}

		pDestTaskFile->SetTaskFileReferencePath(hTask, sFileRef);
	}

	// comments
	pDestTaskFile->SetTaskComments(hTask, pXISrcTask->GetItemValue(_T("notes")));

	// dependency
	// do this after we've imported all the tasks because GP does it 
	// the opposite way round to TDL

	// children
	if (!ImportTask(pXISrcTask->GetItem(_T("task")), pDestTaskFile, hTask))
		return false;

	// siblings
	return ImportTask(pXISrcTask->GetSibling(), pDestTaskFile, htDestParent);
}

DWORD CGPImporter::GetTDLTaskID(int nGPTaskID)
{
	// we deliberately add 1 to the task ID because GP has zero-indexed
	// task IDs and TDL has one-based IDs. This reverses the 
	// process in the exporter that deducted one from each task ID
	return ((DWORD)nGPTaskID + 1);
}

void CGPImporter::FixupResourceAllocations(const CXmlItem* pXISrcPrj, ITaskList8* pDestTaskFile)
{
	BuildResourceMap(pXISrcPrj);
			
	const CXmlItem* pXIAllocations = pXISrcPrj->GetItem(_T("allocations"));

	if (pXIAllocations && m_mapResources.GetCount())
	{
		const CXmlItem* pXIAlloc = pXIAllocations->GetItem(_T("allocation"));

		while (pXIAlloc)
		{
			DWORD dwTaskID = GetTDLTaskID(pXIAlloc->GetItemValueI(_T("task-id")));
			int nResID = pXIAlloc->GetItemValueI(_T("resource-id"));

			// look up task
			HTASKITEM hTask = pDestTaskFile->FindTask(dwTaskID);

			if (hTask)
			{
				CString sRes;

				if (m_mapResources.Lookup(nResID, sRes) && !sRes.IsEmpty())
					pDestTaskFile->AddTaskAllocatedTo(hTask, sRes);
			}
			
			pXIAlloc = pXIAlloc->GetSibling();
		}
	}
}

void CGPImporter::FixupDependencies(const CXmlItem* pXISrcTask, ITaskList8* pDestTaskFile)
{
	if (!pXISrcTask)
		return;

	const CXmlItem* pXIDepends = pXISrcTask->GetItem(_T("depend"));

	while (pXIDepends)
	{
		if (pXIDepends->GetItemValueI(_T("type")) == 2)
		{
			// GP records dependencies in reverse to TDL
			// so what appears to be the dependency target is actually the source
			DWORD dwSrcDependID = GetTDLTaskID(pXIDepends->GetItemValueI(_T("id")));
			HTASKITEM hTask = pDestTaskFile->FindTask(dwSrcDependID);

			if (hTask)
			{
				DWORD dwDestDependID = GetTDLTaskID(pXISrcTask->GetItemValueI(_T("id")));
				pDestTaskFile->AddTaskDependency(hTask, dwDestDependID);
			}
		}

		// next dependency
		pXIDepends = pXIDepends->GetSibling();
	}

	// children
	FixupDependencies(pXISrcTask->GetItem(_T("task")), pDestTaskFile);

	// siblings
	FixupDependencies(pXISrcTask->GetSibling(), pDestTaskFile);
}

void CGPImporter::BuildResourceMap(const CXmlItem* pXISrcPrj)
{
	m_mapResources.RemoveAll();

	const CXmlItem* pXIResources = pXISrcPrj->GetItem(_T("resources"));

	if (pXIResources)
	{
		const CXmlItem* pXIRes = pXIResources->GetItem(_T("resource"));

		while (pXIRes)
		{
			m_mapResources[pXIRes->GetItemValueI(_T("id"))] = pXIRes->GetItemValue(_T("name"));
			pXIRes = pXIRes->GetSibling();
		}
	}
}
