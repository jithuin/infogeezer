// TaskTimeLog.h: interface for the CTaskTimeLog class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TASKTIMELOG_H__6C9F21CD_509E_4890_9B28_F8C6E52FF54B__INCLUDED_)
#define AFX_TASKTIMELOG_H__6C9F21CD_509E_4890_9B28_F8C6E52FF54B__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\3rdParty\stdiofileex.h"

class CTaskTimeLog  
{
public:
	CTaskTimeLog(LPCTSTR szRefPath, SFE_SAVEAS nSaveAs = SFE_ASIS);
	virtual ~CTaskTimeLog();

	BOOL LogTime(DWORD dwTaskID, LPCTSTR szTaskTitle, double dTime, BOOL bLogSeparately); // time must be in hours
	BOOL LogTime(DWORD dwTaskID, LPCTSTR szTaskTitle, double dTime, const COleDateTime& dtWhen, BOOL bLogSeparately); // time must be in hours
	double CalcAccumulatedTime(DWORD dwTaskID, BOOL bLogSeparately); // returns time in hours
	CString GetLogPath(DWORD dwTaskID, BOOL bLogSeparately);

protected:
	CString m_sRefPath;
	SFE_SAVEAS m_nSaveAs;
	int m_nVersion;
	BOOL m_bLogExists;
	CString m_sDelim;

protected: 
	void Initialise(const CString& sLogPath);
	CString GetLatestColumnHeader() const;
	CString GetRowFormat(int nVersion) const;
	CString FormatItemRow(DWORD dwTaskID, LPCTSTR szTaskTitle, double dTime, 
							const COleDateTime& dtStart, const COleDateTime& dtEnd) const;

};

#endif // !defined(AFX_TASKTIMELOG_H__6C9F21CD_509E_4890_9B28_F8C6E52FF54B__INCLUDED_)
