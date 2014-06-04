//Written for the ToDoList (http://www.codeproject.com/KB/applications/todolist2.aspx)
//Design and latest version - http://www.codeproject.com/KB/applications/TDL_Calendar.aspx
//by demon.code.monkey@googlemail.com

#ifndef _BIGCALENDARTASK_H_
#define _BIGCALENDARTASK_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "TransparentListBox.h"

class CBigCalendarCtrl;


class CBigCalendarTask : public CTransparentListBox
{
public:
	CBigCalendarTask(CBigCalendarCtrl* _pParent=NULL, DWORD _dwStyleCompletedTasks=0, int nCellFontSize = 8);
	virtual ~CBigCalendarTask();

	int  AddItem(const CString& _strText, DWORD _dwTaskID, BOOL _bStartTask, BOOL _bDueTask, BOOL _bCompleteTask);
	void ResetContent();

protected:
	int	 OnToolHitTest(CPoint point, TOOLINFO* pTI) const;
	BOOL OnToolTipText(UINT id, NMHDR* pTTTStruct, LRESULT* pResult);

	//{{AFX_MSG(CBigCalendarTask)
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnPaint();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnRButtonDown(UINT nFlags, CPoint point);
	afx_msg BOOL OnMouseWheel(UINT nFlags, short zDelta, CPoint pt);
	afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
	protected:
	virtual void PreSubclassWindow();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

protected:
	CBigCalendarCtrl*	m_pParent;
private:
	CMap <DWORD, DWORD&, BOOL, BOOL&> m_mapTaskIDToStartTask;
	CMap <DWORD, DWORD&, BOOL, BOOL&> m_mapTaskIDToDueTask;
	CMap <DWORD, DWORD&, BOOL, BOOL&> m_mapTaskIDToCompleteTask;
};


#endif//_BIGCALENDARTASK_H_
