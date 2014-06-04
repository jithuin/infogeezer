// iCalImporter.h: interface for the CiCalImporter class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_ICALIMPORTER_H__36D6AF21_80EA_4361_85E9_B9BCDB38F913__INCLUDED_)
#define AFX_ICALIMPORTER_H__36D6AF21_80EA_4361_85E9_B9BCDB38F913__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\SHARED\IImportExport.h"
#include "..\SHARED\ITasklist.h"

class ITransText;

class CiCalImporter : public IImportTasklist  
{
public:
	CiCalImporter();
	virtual ~CiCalImporter();

	// interface implementation
	void Release() { delete this; }
	void SetLocalizer(ITransText* pTT);

	// caller must copy only
	LPCTSTR GetMenuText() const { return _T("iCalendar"); }
	LPCTSTR GetFileFilter() const { return _T("iCalendar Files (*.ics)|*.ics||"); }
	LPCTSTR GetFileExtension() const { return _T("ics"); }

	bool Import(LPCTSTR szSrcFilePath, ITaskList* pDestTaskFile, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);
};

#endif // !defined(AFX_ICALIMPORTER_H__36D6AF21_80EA_4361_85E9_B9BCDB38F913__INCLUDED_)
