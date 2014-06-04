// MLOImporter.h: interface for the CMLOImporter class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MLOIMPORTER_H__E1C1DB38_D45E_481E_8D91_7D8455C5155E__INCLUDED_)
#define AFX_MLOIMPORTER_H__E1C1DB38_D45E_481E_8D91_7D8455C5155E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\SHARED\IImportExport.h"
#include "..\SHARED\iTasklist.h"

class CXmlItem;
class ITransText;

class CMLOImporter : public IImportTasklist  
{
public:
	CMLOImporter();
	virtual ~CMLOImporter();
	
	void Release() { delete this; }
	void SetLocalizer(ITransText* pTT);

	LPCTSTR GetMenuText() const { return _T("My Life Organized"); }
	LPCTSTR GetFileFilter() const { return _T("MLO Task Files (*.ml, *.xml)|*.ml;*.xml||"); }
	LPCTSTR GetFileExtension() const { return _T("ml"); }
	
	bool Import(LPCTSTR szSrcFilePath, ITaskList* pDestTaskFile, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);

protected:
	bool ImportTask(const CXmlItem* pXIMLOTask, ITaskList5* pDestTaskFile, HTASKITEM hParent) const;
	time_t GetDate(const CXmlItem* pXIMLOTask, LPCTSTR szDateField) const;

};

#endif // !defined(AFX_MLOIMPORTER_H__E1C1DB38_D45E_481E_8D91_7D8455C5155E__INCLUDED_)
