// iCalImporter.cpp: implementation of the CiCalImporter class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "iCalImportExport.h"
#include "iCalImporter.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CiCalImporter::CiCalImporter()
{

}

CiCalImporter::~CiCalImporter()
{

}

void CiCalImporter::SetLocalizer(ITransText* /*pTT*/)
{
	//CLocalizer::Initialize(pTT);
}

bool CiCalImporter::Import(LPCTSTR /*szSrcFilePath*/, ITaskList* /*pDestTaskFile*/, BOOL /*bSilent*/, IPreferences* /*pPrefs*/, LPCTSTR /*szKey*/)
{
//	HTASKITEM hTask = pDestTaskFile->NewTask("test");
//	pDestTaskFile->SetTaskComments(hTask, "comments");
//	pDestTaskFile->SetTaskPriority(hTask, 7);

	
	return false;
}
