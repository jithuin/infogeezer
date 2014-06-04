// MLOExporter.h: interface for the CMLOExporter class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MLOEXPORTER_H__F588E6B1_3646_4994_99A2_4223FDDA1A31__INCLUDED_)
#define AFX_MLOEXPORTER_H__F588E6B1_3646_4994_99A2_4223FDDA1A31__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\SHARED\IImportExport.h"
#include "..\SHARED\ITasklist.h"

#include <afxtempl.h>

class CXmlItem;
class ITransText;

class CMLOExporter : public IExportTasklist  
{
public:
	CMLOExporter();
	virtual ~CMLOExporter();

	// interface implementation
	void Release() { delete this; }
	void SetLocalizer(ITransText* pTT);

	// caller must copy only
    LPCTSTR GetMenuText() const { return _T("My Life Organized"); }
	LPCTSTR GetFileFilter() const { return _T("MLO Task Files (*.ml)|*.ml||"); }
	LPCTSTR GetFileExtension() const { return _T("ml"); }

	bool Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);
	bool Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);

protected:
	bool ExportTask(const ITaskList7* pSrcTaskFile, HTASKITEM hTask, CXmlItem* pXIDestParent);

	void BuildPlacesMap(const ITaskList7* pSrcTaskFile, HTASKITEM hTask, CMapStringToString& mapPlaces);
	void ExportPlaces(const ITaskList7* pSrcTaskFile, CXmlItem* pDestPrj);
};

#endif // !defined(AFX_MLOEXPORTER_H__F588E6B1_3646_4994_99A2_4223FDDA1A31__INCLUDED_)
