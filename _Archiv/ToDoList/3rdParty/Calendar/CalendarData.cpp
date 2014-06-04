//Written for the ToDoList (http://www.codeproject.com/KB/applications/todolist2.aspx)
//Design and latest version - http://www.codeproject.com/KB/applications/TDL_Calendar.aspx
//by demon.code.monkey@googlemail.com

#include "stdafx.h"
#include "CalendarUtils.h"
#include "CalendarData.h"

#include "..\..\Shared\IUIExtension.h"
#include "..\..\todolist\tdcenum.h"

#define CALENDAR_INI_SECTION_NAME	_T("UI_Extension_Plugin_CALENDAR")

CCalendarData::CCalendarData()
:	m_nTaskID(-1)
{
}

CCalendarData::~CCalendarData()
{
	ClearImportantDateMap();
}

BOOL CCalendarData::IsImportantDate(const COleDateTime& _dt) const
{
	BOOL bReturn = FALSE;

	time_t tDate = CCalendarUtils::DateToTime(_dt);

	CMapTimeToTaskItemList* pmapItems = NULL;
	if (m_mapDateToHTASKITEMs.Lookup(tDate, pmapItems))
	{
		bReturn = TRUE;
	}

	return bReturn;
}

#ifdef _DEBUG
void CCalendarData::DumpTasks(HTASKITEM _hParentTask/*=NULL*/)
{
	for (HTASKITEM hTask = m_tasks.GetFirstTask(_hParentTask);
		 hTask != NULL;
		 hTask = m_tasks.GetNextTask(hTask))
	{
		DWORD dwTaskID = m_tasks.GetTaskID(hTask);

		COleDateTime dtStart = m_tasks.GetTaskStartDateOle(hTask);
		COleDateTime dtDue = m_tasks.GetTaskDueDateOle(hTask);

		time_t tStart = 0;
		time_t tDue = 0;
		//these calls needed to get rid of seconds, minutes etc from time_t values
		if (CCalendarUtils::IsDateValid(dtStart))
		{
			tStart = CCalendarUtils::DateToTime(dtStart);
		}
		if (CCalendarUtils::IsDateValid(dtDue))
		{
			tDue = CCalendarUtils::DateToTime(dtDue);
		}

		CString strTaskTitle = m_tasks.GetTaskTitle(hTask);
		CString strTaskStartDate;
		CString strTaskDueDate;
		if (tStart != 0)
		{
			strTaskStartDate = CCalendarUtils::DateToString(dtStart);
		}
		if (tDue != 0)
		{
			strTaskDueDate = CCalendarUtils::DateToString(dtDue);
		}
		TRACE(_T("TASK ID=%d \"%s\": Start=%s, Due=%s\n"), dwTaskID, strTaskTitle, strTaskStartDate, strTaskDueDate);

		DumpTasks(hTask);
	}
}
#endif

void CCalendarData::GetTasks(const COleDateTime& _dt,
							 CTaskInfoList& _listTasks) const
{
	ASSERT(IsImportantDate(_dt));
	_listTasks.RemoveAll();

	time_t tDate = CCalendarUtils::DateToTime(_dt);

	CMapTimeToTaskItemList* pmapItems = NULL;
	if (m_mapDateToHTASKITEMs.Lookup(tDate, pmapItems))
	{
		for (POSITION pos = pmapItems->GetStartPosition(); pos; )
		{
			HTASKITEM hTask = NULL;
			void* pUnused = NULL;
			pmapItems->GetNextAssoc(pos, hTask, pUnused);

			int nTaskID = m_tasks.GetTaskID(hTask);
			CString strTaskTitle = m_tasks.GetTaskTitle(hTask);

			COleDateTime dtStartDate = m_tasks.GetTaskStartDateOle(hTask);
			BOOL bIsStart = (CCalendarUtils::CompareDates(dtStartDate, _dt) == 0);
			COleDateTime dtDueDate = m_tasks.GetTaskDueDateOle(hTask);
			BOOL bIsDue = (CCalendarUtils::CompareDates(dtDueDate, _dt) == 0);
			COleDateTime dtDoneDate = m_tasks.GetTaskDoneDateOle(hTask);
			BOOL bIsDone = (CCalendarUtils::CompareDates(dtDoneDate, _dt) == 0);
			BOOL bIsComplete = m_tasks.IsTaskDone(hTask);

			CTaskInfo ti(nTaskID, strTaskTitle, bIsStart, bIsDue, bIsDone, bIsComplete);
			_listTasks.AddTail(ti);
		}
	}
}

BOOL CCalendarData::GetTaskTitle(DWORD _dwItemID,
								 CString& _strTitle) const
{
	BOOL bReturn = FALSE;

	HTASKITEM hTask = m_tasks.FindTask(_dwItemID);
	if (hTask)
	{
		_strTitle = m_tasks.GetTaskTitle(hTask);
		bReturn = TRUE;
	}

	return bReturn;
}

BOOL CCalendarData::GetTaskStartDate(DWORD _dwItemID,
									 COleDateTime& _date) const
{
	BOOL bReturn = FALSE;

	HTASKITEM hTask = m_tasks.FindTask(_dwItemID);
	if (hTask)
	{
		_date = m_tasks.GetTaskStartDateOle(hTask);
		bReturn = (_date.m_dt != 0);
	}

	return bReturn;
}

BOOL CCalendarData::GetTaskDueDate(DWORD _dwItemID,
								   COleDateTime& _date) const
{
	BOOL bReturn = FALSE;

	HTASKITEM hTask = m_tasks.FindTask(_dwItemID);
	if (hTask)
	{
		_date = m_tasks.GetTaskDueDateOle(hTask);
		bReturn = (_date.m_dt != 0);
	}

	return bReturn;
}

BOOL CCalendarData::GetTaskDoneDate(DWORD _dwItemID,
									COleDateTime& _date) const
{
	BOOL bReturn = FALSE;

	HTASKITEM hTask = m_tasks.FindTask(_dwItemID);
	if (hTask)
	{
		if (m_tasks.IsTaskDone(hTask))
		{
			_date = m_tasks.GetTaskDoneDateOle(hTask);
			bReturn = (_date.m_dt != 0);
		}
	}

	return bReturn;
}

BOOL CCalendarData::GetTaskTimeEstimate(DWORD _dwItemID,
										double& _dTimeEstimate,
										TCHAR& _cUnit) const
{
	BOOL bReturn = FALSE;

	HTASKITEM hTask = m_tasks.FindTask(_dwItemID);
	if (hTask)
	{
		_dTimeEstimate = m_tasks.GetTaskTimeEstimate(hTask, _cUnit, FALSE);
		bReturn = (_dTimeEstimate > 0);
	}

	return bReturn;
}

BOOL CCalendarData::GetTaskTimeSpent(DWORD _dwItemID,
									 double& _dTimeSpent,
									 TCHAR& _cUnit) const
{
	BOOL bReturn = FALSE;

	HTASKITEM hTask = m_tasks.FindTask(_dwItemID);
	if (hTask)
	{
		_dTimeSpent = m_tasks.GetTaskTimeSpent(hTask, _cUnit, FALSE);
		bReturn = (_dTimeSpent > 0);
	}

	return bReturn;
}

CString CCalendarData::GetTaskParentsString(DWORD _dwItemID) const
{
	CString strReturn;
	HTASKITEM hTask = m_tasks.FindTask(_dwItemID);
	if (hTask)
	{
		int nNumParents = 0;
		strReturn = GetTaskParentsString(hTask, nNumParents);
	}
	else
	{
		ASSERT(FALSE);
	}
	return strReturn;
}

CString CCalendarData::GetTaskParentsString(HTASKITEM _hCurrentTask,
											int& _nNumParents) const
{
	CString strReturn;
	if (_hCurrentTask)
	{
		HTASKITEM hParentTask = m_tasks.GetTaskParent(_hCurrentTask);
		CString strParent = GetTaskParentsString(hParentTask, _nNumParents);
		CString strCurrent = m_tasks.GetTaskTitle(_hCurrentTask);
		if (!strParent.IsEmpty())
		{
			strReturn = strParent;
			strReturn += "\n";

			for (int i = 2; i < _nNumParents; i++)
			{
				strReturn += "     ";
			}
			strReturn += "    ";
		}
		strReturn += strCurrent;
		_nNumParents++;
	}
	return strReturn;
}

BOOL CCalendarData::TasklistUpdated(const ITaskList* _pTasks,
									IUI_UPDATETYPE nUpdate, int nEditAttribute)
{
//#ifdef _DEBUG
//	TRACE("before copying\n");
//	DumpTasks(NULL);
//#endif

	BOOL bTasksCopied = FALSE;

	// Need to check dwFlags for how to update m_tasks
	if (nUpdate == IUI_EDIT)
	{
		//_pTasks contains only certain tasks so we must update only those
		HTASKITEM hTask = _pTasks->GetFirstTask();
		bTasksCopied = UpdateTasks(_pTasks, hTask, nEditAttribute);
	}
	else // copy all tasks
	{
		bTasksCopied = m_tasks.CopyFrom(_pTasks);
	}

//#ifdef _DEBUG
//	TRACE("after copying\n");
//	DumpTasks(NULL);
//#endif

	if (bTasksCopied)
	{
		//add tasks with start/due dates to the ImportantDate map
		ClearImportantDateMap();
		CalculateImportantDates();
	}

	//note bTasksCopied may be FALSE if some details were changed that we are not interested in (such as comments, colour etc)

	return bTasksCopied;
}

// updates the current task and all its children, then moves onto its sibling. calls itself recursively
// returns TRUE if it or any of its children or siblings have been updated
BOOL CCalendarData::UpdateTasks(const ITaskList* _pTasks,
								HTASKITEM _hCurrentTask, int nEditAttribute)
{
	BOOL bTasksCopied = FALSE;

	for (HTASKITEM hTask = _hCurrentTask; hTask != NULL; hTask = _pTasks->GetNextTask(hTask))
	{
		// find corresponding task in m_tasks
		DWORD dwID = _pTasks->GetTaskID(hTask);
		HTASKITEM hDest = m_tasks.FindTask(dwID);

		// update whatever interests us
		if (hDest)
		{
			TCHAR cUnit = 'h';

			switch (nEditAttribute)
			{
			case TDCA_TASKNAME:
				{
					LPCTSTR szOld = m_tasks.GetTaskTitle(hDest);
					LPCTSTR szNew = _pTasks->GetTaskTitle(hTask);

					if (lstrcmp(szNew, szOld) != 0)
					{
						m_tasks.SetTaskTitle(hDest, szNew);
						bTasksCopied = TRUE;
					}
				}
				break;
				
			case TDCA_DONEDATE:
				{
					time_t tOld = m_tasks.GetTaskDoneDate(hDest);
					time_t tNew = _pTasks->GetTaskDoneDate(hTask);

					if (tNew != tOld)
					{
						m_tasks.SetTaskDoneDate(hDest, _pTasks->GetTaskDoneDate(hTask));
						bTasksCopied = TRUE;
					}
				}
				break;

			case TDCA_DUEDATE:
				{
					time_t tOld = m_tasks.GetTaskDueDate(hDest);
					time_t tNew = _pTasks->GetTaskDueDate(hTask);

					if (tNew != tOld)
					{
						m_tasks.SetTaskDueDate(hDest, tNew);
						bTasksCopied = TRUE;
					}
				}
				break;

			case TDCA_STARTDATE:
				{
					time_t tOld = m_tasks.GetTaskStartDate(hDest);
					time_t tNew = _pTasks->GetTaskStartDate(hTask);

					if (tNew != tOld)
					{
						m_tasks.SetTaskStartDate(hDest, tNew);
						bTasksCopied = TRUE;
					}
				}
				break;

			case TDCA_PERCENT:
				{
					int nOld = m_tasks.GetTaskPercentDone(hDest, FALSE);
					int nNew = _pTasks->GetTaskPercentDone(hTask, FALSE);

					if (nNew != nOld)
					{
						m_tasks.SetTaskPercentDone(hDest, nNew);
						bTasksCopied = TRUE;
					}
				}
				break;

			case TDCA_TIMEEST:
				{
					TCHAR cOld, cNew;
					double dOld = m_tasks.GetTaskTimeEstimate(hDest, cOld, FALSE);
					double dNew = _pTasks->GetTaskTimeEstimate(hTask, cNew, FALSE);

					if (dNew != dOld || cOld != cNew)
					{
						m_tasks.SetTaskTimeEstimate(hDest, dNew, cNew);
						bTasksCopied = TRUE;
					}
				}
				break;

			case TDCA_TIMESPENT:
				{
					TCHAR cOld, cNew;
					double dOld = m_tasks.GetTaskTimeSpent(hDest, cOld, FALSE);
					double dNew = _pTasks->GetTaskTimeSpent(hTask, cNew, FALSE);

					if (dNew != dOld || cOld != cNew)
					{
						// cUnit will now hold the requires time unit
						m_tasks.SetTaskTimeSpent(hDest, dNew, cNew);
						bTasksCopied = TRUE;
					}
				}
				break;

			default:
				ASSERT(0);
				return FALSE;
			}
		}

		HTASKITEM hTaskFirstChild = _pTasks->GetFirstTask(hTask);

		if (hTaskFirstChild != NULL)
		{
			if (UpdateTasks(_pTasks, hTaskFirstChild, nEditAttribute)) // RECURSIVE CALL
				bTasksCopied = TRUE;

		}
	}

	return bTasksCopied;
}

//clear the cached list of important dates
void CCalendarData::ClearImportantDateMap()
{
	if (!m_mapDateToHTASKITEMs.IsEmpty())
	{
		for (POSITION posClear = m_mapDateToHTASKITEMs.GetStartPosition(); posClear; )
		{
			time_t tUnused = 0;
			CMapTimeToTaskItemList* pmapItems = NULL;
			m_mapDateToHTASKITEMs.GetNextAssoc(posClear, tUnused, pmapItems);
			delete pmapItems;
		}
		m_mapDateToHTASKITEMs.RemoveAll();
	}
}

void CCalendarData::CalculateImportantDates(HTASKITEM _hParentTask/*=NULL*/)
{
	//cache the important dates into m_mapDateToHTASKITEMs
	for (HTASKITEM hTask = m_tasks.GetFirstTask(_hParentTask);
		 hTask != NULL;
		 hTask = m_tasks.GetNextTask(hTask))
	{
		COleDateTime dtStart = m_tasks.GetTaskStartDateOle(hTask);
		COleDateTime dtDue = m_tasks.GetTaskDueDateOle(hTask);
		COleDateTime dtDone = m_tasks.GetTaskDoneDateOle(hTask);

		time_t tStart = 0;
		time_t tDue = 0;
		time_t tDone = 0;

		//these calls needed to get rid of seconds, minutes etc from time_t values
		if (CCalendarUtils::IsDateValid(dtStart))
		{
			tStart = CCalendarUtils::DateToTime(dtStart);
		}
		if (CCalendarUtils::IsDateValid(dtDue))
		{
			tDue = CCalendarUtils::DateToTime(dtDue);
		}
		if (CCalendarUtils::IsDateValid(dtDone))
		{
			tDone = CCalendarUtils::DateToTime(dtDone);
		}

		if (tStart != 0)
		{
			CMapTimeToTaskItemList* pmapItems = NULL;
			if (!m_mapDateToHTASKITEMs.Lookup(tStart, pmapItems))
			{
				pmapItems = new CMapTimeToTaskItemList;
				m_mapDateToHTASKITEMs.SetAt(tStart, pmapItems);
			}
			pmapItems->SetAt(hTask, NULL);
		}

		if (tDue != 0)
		{
			CMapTimeToTaskItemList* pmapItems = NULL;
			if (!m_mapDateToHTASKITEMs.Lookup(tDue, pmapItems))
			{
				pmapItems = new CMapTimeToTaskItemList;
				m_mapDateToHTASKITEMs.SetAt(tDue, pmapItems);
			}
			pmapItems->SetAt(hTask, NULL);
		}

		if (tDone != 0)
		{
			CMapTimeToTaskItemList* pmapItems = NULL;
			if (!m_mapDateToHTASKITEMs.Lookup(tDone, pmapItems))
			{
				pmapItems = new CMapTimeToTaskItemList;
				m_mapDateToHTASKITEMs.SetAt(tDone, pmapItems);
			}
			pmapItems->SetAt(hTask, NULL);
		}

		//call self recursively to do child tasks
		CalculateImportantDates(hTask);
	}
}
/*
void CCalendarData::ReadValueFromIni(const CString& _strKey,
									 CString& _strValue,
									 LPCTSTR _pszDefault)
{
	CString strIniFilename = AfxGetApp()->m_pszProfileName;

	CString strDefault;
	if (_pszDefault != NULL)
	{
		strDefault = _pszDefault;
	}

	const DWORD dwChunkSize = 512;

	DWORD dwLen = dwChunkSize;
	TCHAR* pszValue = new TCHAR[dwLen+1];
	DWORD dwCopied = ::GetPrivateProfileString(CALENDAR_INI_SECTION_NAME, _strKey, strDefault, pszValue, dwLen, strIniFilename);
	while (dwCopied + 1 >= dwLen)
	{
		dwLen += dwChunkSize;
		delete[] pszValue;
		pszValue = new TCHAR[dwLen + 1];
		dwCopied = ::GetPrivateProfileString(CALENDAR_INI_SECTION_NAME, _strKey, strDefault, pszValue, dwLen, strIniFilename);
	}

	_strValue = pszValue;
	delete[] pszValue;
}

void CCalendarData::ReadValueFromIni(const CString& _strKey,
									 DWORD& _dwValue,
									 DWORD _dwDefault)
{
	CString strDefault;
	strDefault.Format(_T("%d"), _dwDefault);

	CString strValue;
	ReadValueFromIni(_strKey, strValue, strDefault);

	_dwValue = _ttol(strValue);
}

void CCalendarData::ReadValueFromIni(const CString& _strKey,
									 int& _nValue,
									 int _nDefault)
{
	ReadValueFromIni(_strKey, (DWORD&)_nValue, _nDefault);
}

void CCalendarData::ReadValueFromIni(const CString& _strKey,
									 CRect& _rcValue,
									 const CRect& _rcDefault)
{
	CString strDefault = RectToString(_rcDefault);
	CString strValue;
	ReadValueFromIni(_strKey, strValue, strDefault);
	_rcValue = StringToRect(strValue);
}

void CCalendarData::WriteValueToIni(const CString& _strKey,
									const CString& _strValue)
{
	CString strIniFilename = AfxGetApp()->m_pszProfileName;
	VERIFY(::WritePrivateProfileString(CALENDAR_INI_SECTION_NAME, _strKey, _strValue, strIniFilename));
}

void CCalendarData::WriteValueToIni(const CString& _strKey,
									DWORD _dwValue)
{
	CString strValue;
	strValue.Format(_T("%d"), _dwValue);
	WriteValueToIni(_strKey, strValue);
}

void CCalendarData::WriteValueToIni(const CString& _strKey,
									const CRect& _rcValue)
{
	CString strValue = RectToString(_rcValue);
	WriteValueToIni(_strKey, strValue);
}

CRect CCalendarData::StringToRect(const CString& _strRect)
{
	CString strRect(_strRect);
	CRect rcReturn(0,0,0,0);
	
	TCHAR* pch = _tcstok(strRect.GetBuffer(MAX_PATH), _T(","));
	CRect rcValue(0,0,0,0);
	int nLoop = 0;
	while (pch != NULL)
	{
		TRACE(_T("pch:%s\n"), pch);
		int nVal = _ttoi(pch);
		switch (nLoop)
		{
			case 0:	rcValue.top = nVal; break;
			case 1:	rcValue.left = nVal; break;
			case 2:	rcValue.bottom = nVal; break;
			case 3:	rcValue.right = nVal; break;
			default: ASSERT(FALSE); break;
		}
		nLoop++;
		pch = _tcstok(NULL, _T(","));
	}
	strRect.ReleaseBuffer();

	if (nLoop == 4)
	{
		rcReturn = rcValue;
	}
	else
	{
		ASSERT(FALSE);	//malformed CRect details
	}

	return rcReturn;
}

CString CCalendarData::RectToString(const CRect& _rcRect)
{
	CString strOut;
	strOut.Format(_T("%d,%d,%d,%d"), _rcRect.top, _rcRect.left, _rcRect.bottom, _rcRect.right);
	return strOut;
}
*/