// MultiTaskFile.cpp: implementation of the CMultiTaskFile class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "todolist.h"
#include "MultiTaskFile.h"
#include "TaskFile.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMultiTaskFile::CMultiTaskFile()
{

}

CMultiTaskFile::~CMultiTaskFile()
{
	// cleanup
	int nTaskFile = GetTaskListCount();

	while (nTaskFile--)
		delete m_aTaskFiles[nTaskFile];
}

CTaskFile& CMultiTaskFile::GetTaskFile(int nTaskFile) 
{
	while (m_aTaskFiles.GetSize() <= nTaskFile)
	{
		CTaskFile* pTF = new CTaskFile; 
		m_aTaskFiles.Add(pTF);
	}

	return *(m_aTaskFiles[nTaskFile]);
}

HRESULT CMultiTaskFile::QueryInterface(REFIID /*riid*/, void __RPC_FAR *__RPC_FAR *ppvObject)
{
	*ppvObject = reinterpret_cast<ITaskList*>(this);

	// compare riid against our supported version numbers
/*
	if (IsEqualIID(riid, IID_TASKLIST))
	{
		*ppvObject = reinterpret_cast<ITaskList*>(this);
		AddRef();
	}
	else if (IsEqualIID(riid, IID_TASKLIST2))
	{
		*ppvObject = reinterpret_cast<ITaskList2*>(this);
		AddRef();
	}
	else if (IsEqualIID(riid, IID_TASKLIST3))
	{
		*ppvObject = reinterpret_cast<ITaskList3*>(this);
		AddRef();
	}
	else if (IsEqualIID(riid, IID_TASKLIST4))
	{
		*ppvObject = reinterpret_cast<ITaskList4*>(this);
		AddRef();
	}
	else if (IsEqualIID(riid, IID_TASKLIST5))
	{
		*ppvObject = reinterpret_cast<ITaskList5*>(this);
		AddRef();
	}
	else if (IsEqualIID(riid, IID_TASKLIST6))
	{
		*ppvObject = reinterpret_cast<ITaskList6*>(this);
		AddRef();
	}
	else if (IsEqualIID(riid, IID_TASKLIST7))
	{
		*ppvObject = reinterpret_cast<ITaskList7*>(this);
		AddRef();
	}
*/
	
	return (*ppvObject ? S_OK : E_NOTIMPL);
}

int CMultiTaskFile::GetTaskListCount() const
{
	return m_aTaskFiles.GetSize();
}

const ITaskList* CMultiTaskFile::GetTaskList(int nTaskList) const
{
	ASSERT(m_aTaskFiles.GetSize() > nTaskList);

	if (m_aTaskFiles.GetSize() <= nTaskList)
		return NULL;

	// else
	return m_aTaskFiles[nTaskList];
}
