// TaskListCsvImporter.h: interface for the CTaskListCsvImporter class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TASKLISTCSVIMPORTER_H__ADF211CB_FBD2_42A2_AD51_DFF58E566753__INCLUDED_)
#define AFX_TASKLISTCSVIMPORTER_H__ADF211CB_FBD2_42A2_AD51_DFF58E566753__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "tdcenum.h"
#include "TDLCsvImportExportDlg.h"

#include "..\shared\Itasklist.h"
#include "..\shared\IImportExport.h"

class CTaskListCsvImporter : public IImportTasklist  
{
public:
	CTaskListCsvImporter();
	virtual ~CTaskListCsvImporter();

    void Release() { delete this; }
	void SetLocalizer(ITransText* /*pTT*/) {}

	LPCTSTR GetMenuText() const { return _T("Spreadsheet"); }
	LPCTSTR GetFileFilter() const { return _T("Spreadsheet Files (*.csv)|*.csv||"); }
	LPCTSTR GetFileExtension() const { return _T("csv"); }

	bool Import(LPCTSTR szSrcFilePath, ITaskList* pDestTaskFile, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);

protected:
	CString DELIM, INDENT;
	int ROOTDEPTH;

	CTDCCsvColumnMapping m_aColumnMapping;

protected:
	BOOL ImportTask(ITaskList9* pTasks, const CString& sLine) const;

	bool InitConsts(LPCTSTR szSrcFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey);
	int GetDepth(const CString& sLine);

	int AttributeIndex(TDC_ATTRIBUTE attrib) const;
	BOOL WantAttribute(TDC_ATTRIBUTE attrib) const { return (AttributeIndex(attrib) != -1); }

	void GetTaskAndParentIDs(const CStringArray& sValues, DWORD& dwTaskID, DWORD& dwParentID) const;
	CString GetTaskTitle(const CStringArray& sValues) const;

	static CString GetLine(const CStringArray& aLines, int& nLine);
	static time_t String2Date(const CString& sDate);

	void AddAttributeToTask(ITaskList9* pTasks, HTASKITEM hTask, TDC_ATTRIBUTE nAttrib, const CStringArray& aValues) const;

};

#endif // !defined(AFX_TASKLISTCSVImPORTER_H__ADF211CB_FBD2_42A2_AD51_DFF58E566753__INCLUDED_)
