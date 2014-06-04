// TaskListCsvExporter.cpp: implementation of the CTaskListTdlExporter class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "TaskListtdlExporter.h"
#include "TaskFile.h"
#include "tdlschemadef.h"
#include "tdcstruct.h"

#include "..\shared\filemisc.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CTaskListTdlExporter::CTaskListTdlExporter()
{

}

CTaskListTdlExporter::~CTaskListTdlExporter()
{

}

bool CTaskListTdlExporter::Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey)
{
	if (pSrcTaskFile->GetTaskListCount() == 1)
		return Export(pSrcTaskFile->GetTaskList(0), szDestFilePath, bSilent, pPrefs, szKey);

	// create a single tasklist out of the many
	CTaskFile tasks;
	int nTaskListCount = pSrcTaskFile->GetTaskListCount();

	// accumulate custom task attribute definitions
	CTDCCustomAttribDefinitionArray aAttribDefs;

	for (int nTaskList = 0; nTaskList < nTaskListCount; nTaskList++)
	{
		const ITaskList8* pTasks8 = GetITLInterface<ITaskList8>(pSrcTaskFile->GetTaskList(nTaskList), IID_TASKLIST8);
		
		if (pTasks8)
		{
			// do not export the destination file if it also appears in the list
			CString sFilePath = pTasks8->GetAttribute(TDL_FILENAME);

			if (FileMisc::IsSameFile(sFilePath, szDestFilePath))
				continue;

			// save off custom attribute definitions
			CopyCustomDataDefinitions(pTasks8, aAttribDefs);

			// create a top-level task for each tasklist
			CString sProjectName = pTasks8->GetProjectName();

			if (sProjectName.IsEmpty())
				sProjectName = FileMisc::GetFileNameFromPath(sFilePath, FALSE);

			HTASKITEM hFileTask = tasks.NewTask(sProjectName, NULL);

			// copy tasklist into that top-level task resetting IDs
			// and keeping track of the ID mapping as we go
			CID2IDMap mapIDs;

			tasks.CopyTaskFrom(pTasks8, pTasks8->GetFirstTask(), hFileTask, TRUE, &mapIDs);

			// verify task count is as expected
			ASSERT(mapIDs.GetCount() == pTasks8->GetTaskCount());

			// create file links back to original tasks
			HTASKITEM hFirstTask = tasks.GetFirstTask(hFileTask);
			CreateReverseFileLinks(tasks, hFirstTask, mapIDs, FileMisc::GetFileNameFromPath(sFilePath));

			// fixup dependency IDs
			CID2IDMap mapReverseIDs;
			BuildReverseIDMap(mapIDs, mapReverseIDs);

			FixupInternalDependencies(tasks, hFirstTask, mapReverseIDs);
		}
	}

	// inject custom attribute defs in one hit
	tasks.SetCustomAttributeDefs(aAttribDefs);

	return (tasks.Save(szDestFilePath) != FALSE);
}

bool CTaskListTdlExporter::Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL /*bSilent*/, IPreferences* /*pPrefs*/, LPCTSTR /*szKey*/)
{
	CTaskFile tasks(pSrcTaskFile);

	// save off custom attribute definitions
	CTDCCustomAttribDefinitionArray aAttribDefs;

	CopyCustomDataDefinitions(pSrcTaskFile, aAttribDefs);
	tasks.SetCustomAttributeDefs(aAttribDefs);

	return (tasks.Save(szDestFilePath) != FALSE);
}

void CTaskListTdlExporter::CreateReverseFileLinks(CTaskFile& tasks, HTASKITEM hTask, const CID2IDMap& mapIDs, const CString& sFileName)
{
	if (!hTask)
		return;

	DWORD dwID = tasks.GetTaskID(hTask), dwOrgID = 0;
	CString sFileLink;

	VERIFY (mapIDs.Lookup(dwID, dwOrgID));
	ASSERT (dwOrgID > 0);

	sFileLink.Format(_T("tdl://%s?%d"), sFileName, dwOrgID);
	tasks.SetTaskFileReferencePath(hTask, sFileLink);

	// first sibling
	CreateReverseFileLinks(tasks, tasks.GetNextTask(hTask), mapIDs, sFileName);

	// first child
	CreateReverseFileLinks(tasks, tasks.GetFirstTask(hTask), mapIDs, sFileName);
}

void CTaskListTdlExporter::CopyCustomDataDefinitions(const ITaskList* pTasks, CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	const ITaskList10* pTasks10 = GetITLInterface<ITaskList10>(pTasks, IID_TASKLIST10);
	
	if (pTasks10 == NULL) // unsupported
		return;

	// copy custom attribute definitions
	int nNumCustom = pTasks10->GetCustomAttributeCount();

	for (int nCustom = 0; nCustom < nNumCustom; nCustom++)
	{
		TDCCUSTOMATTRIBUTEDEFINITION attribDef;
		CString sAttribPath;
		
		attribDef.SetAttributeType(_ttoi(pTasks10->GetCustomAttributeValue(nCustom, TDL_CUSTOMATTRIBTYPE)));
		attribDef.sUniqueID = pTasks10->GetCustomAttributeValue(nCustom, TDL_CUSTOMATTRIBID);
		attribDef.sLabel = pTasks10->GetCustomAttributeValue(nCustom, TDL_CUSTOMATTRIBLABEL);
		attribDef.sColumnTitle = pTasks10->GetCustomAttributeValue(nCustom, TDL_CUSTOMATTRIBCOLTITLE);
		attribDef.nColumnAlignment = _ttoi(pTasks10->GetCustomAttributeValue(nCustom, TDL_CUSTOMATTRIBCOLALIGN));
		attribDef.bSortable = _ttoi(pTasks10->GetCustomAttributeValue(nCustom, TDL_CUSTOMATTRIBSORTABLE));
		Misc::Split(pTasks10->GetCustomAttributeValue(nCustom, TDL_CUSTOMATTRIBLISTDATA), '\n', attribDef.aDefaultListData);

		aAttribDefs.Add(attribDef);
	}
}

void CTaskListTdlExporter::FixupInternalDependencies(CTaskFile& tasks, HTASKITEM hTask, const CID2IDMap& mapIDs)
{
	if (!hTask)
		return;

	CStringArray aDepends;
	int nNumDepends = tasks.GetTaskDependencies(hTask, aDepends);

	for (int nDepend = 0; nDepend < nNumDepends; nDepend++)
	{
		DWORD dwDependID = _ttoi(aDepends[nDepend]);

		// only handle  simple numeric dependencies
		if (dwDependID > 0)
		{
			DWORD dwNewID = 0;

			VERIFY (mapIDs.Lookup(dwDependID, dwNewID));
			ASSERT (dwNewID > 0);

			aDepends[nDepend] = Misc::Format(dwNewID);
		}
	}

	// reassign new dependencies
	tasks.SetTaskDependencies(hTask, aDepends);

	// first sibling
	FixupInternalDependencies(tasks, tasks.GetNextTask(hTask), mapIDs);

	// first child
	FixupInternalDependencies(tasks, tasks.GetFirstTask(hTask), mapIDs);

}

void CTaskListTdlExporter::BuildReverseIDMap(const CID2IDMap& mapIDs, CID2IDMap& mapReverseIDs)
{
	mapReverseIDs.RemoveAll();

	POSITION pos = mapIDs.GetStartPosition();
	DWORD dwID, dwOrgID;

	while (pos)
	{
		mapIDs.GetNextAssoc(pos, dwID, dwOrgID);
		ASSERT(dwID > 0 && dwOrgID > 0);

		mapReverseIDs[dwOrgID] = dwID;
	}
}
