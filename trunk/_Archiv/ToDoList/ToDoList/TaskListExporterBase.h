// TaskFileHtmlExporter.h: interface for the CTaskListExporterBase class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TASKEXPORTERBASE_H__E4FD92AB_2BF2_40E3_9C8E_5018A72AEA89__INCLUDED_)
#define AFX_TASKEXPORTERBASE_H__E4FD92AB_2BF2_40E3_9C8E_5018A72AEA89__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "tdcenum.h"

#include "..\SHARED\Itasklist.h"
#include "..\SHARED\Ipreferences.h"

class CTaskListExporterBase
{
public:
	CTaskListExporterBase();
	virtual ~CTaskListExporterBase();

	void Release() { delete this; }

	bool Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);
   	bool Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);

protected:
	// Pseudo-const variables
	BOOL ROUNDTIMEFRACTIONS, WANTPOS;
	CTDCAttributeArray ARRATTRIBUTES;

private:
	CStringArray ARRLABELS;
	CMapStringToString MAPCUSTID2LABEL;

protected:
	// overridables
	virtual bool ExportOutput(LPCTSTR szDestFilePath, const CString& sOutput) const;
	virtual CString ExportTask(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const;
	virtual bool InitConsts(const ITaskList10* pTasks, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);
	virtual CString GetSpaceForNotes() const { return _T(""); }
	virtual CString ExportSubtasks(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const;
	virtual CString FormatAttribute(TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel, const CString& sValue) const = 0;
	virtual CString FormatAttribute(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth, TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel) const;
	virtual CString FormatTitle(const ITaskList10* /*pTasks*/) const { return _T(""); }
	virtual CString FormatHeaderItem(TDC_ATTRIBUTE /*nAttrib*/, const CString& /*sAttribLabel*/) const { return _T(""); }
	virtual CString FormatHeader() const;

private:
	// helpers
	CString ExportTaskAndSubtasks(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const;
	CString FormatAttribute(const ITaskList10* pTasks, HTASKITEM hTask, TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel, 
							LPCTSTR szAttribName, LPCTSTR szAltAttribName = NULL) const;
	CString FormatCustomAttributes(const ITaskList10* pTasks, HTASKITEM hTask) const;
	CString FormatTime(double dTime, TCHAR cUnits) const;

	CString FormatCategoryList(const ITaskList10* pTasks, HTASKITEM hTask, const CString& sAttribLabel) const;
	CString FormatAllocToList(const ITaskList10* pTasks, HTASKITEM hTask, const CString& sAttribLabel) const;
	CString FormatTagList(const ITaskList10* pTasks, HTASKITEM hTask, const CString& sAttribLabel) const;
	CString FormatDependencyList(const ITaskList10* pTasks, HTASKITEM hTask, const CString& sAttribLabel) const;

	void BuildAttribList(const ITaskList10* pTasks, HTASKITEM hTask);
	void CheckAddAttribtoList(const ITaskList10* pTasks, HTASKITEM hTask, TDC_ATTRIBUTE attrib, LPCTSTR szAttribName);
	BOOL WantAttribute(TDC_ATTRIBUTE attrib) const;
	int FindAttribute(TDC_ATTRIBUTE attrib) const;
	
	static CString GetAttribLabel(TDC_ATTRIBUTE attrib);
};

#endif // !defined(AFX_TASKEXPORTERBASE_H__E4FD92AB_2BF2_40E3_9C8E_5018A72AEA89__INCLUDED_)
