// TaskListTxtExporter.h: interface for the CTaskListCsvExporter class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TASKLISTCSVEXPORTER2_H__CF68988D_FBBD_431D_BB56_464E8737D993__INCLUDED_)
#define AFX_TASKLISTCSVEXPORTER2_H__CF68988D_FBBD_431D_BB56_464E8737D993__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "TaskListExporterBase.h"
#include "TDLCsvImportExportDlg.h"

#include "..\SHARED\Itasklist.h"
#include "..\SHARED\IImportExport.h"

class CTaskListCsvExporter : public IExportTasklist, protected CTaskListExporterBase  
{
public:
	CTaskListCsvExporter();
	virtual ~CTaskListCsvExporter();

    void Release() { delete this; }
	void SetLocalizer(ITransText* /*pTT*/) {}

	LPCTSTR GetMenuText() const { return _T("Spreadsheet"); }
	LPCTSTR GetFileFilter() const { return _T("Spreadsheet Files (*.csv)|*.csv||"); }
	LPCTSTR GetFileExtension() const { return _T("csv"); }

	bool Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);
   	bool Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);

protected:
	CString DELIM, INDENT;
	CTDCCsvColumnMapping m_aColumnMapping;

protected:
	// base-class overrides
	virtual CString ExportTask(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const;

	virtual CString FormatAttribute(TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel, const CString& sValue) const;
	virtual CString FormatAttribute(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth, TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel) const;
	virtual CString FormatHeaderItem(TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel) const;

	virtual bool InitConsts(const ITaskList10* pTasks, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);
};

#endif // !defined(AFX_TASKLISTTXTEXPORTER_H__CF68988D_FBBD_431D_BB56_464E8737D993__INCLUDED_)
