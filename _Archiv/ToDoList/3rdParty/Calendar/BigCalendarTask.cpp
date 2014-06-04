//Written for the ToDoList (http://www.codeproject.com/KB/applications/todolist2.aspx)
//Design and latest version - http://www.codeproject.com/KB/applications/TDL_Calendar.aspx
//by demon.code.monkey@googlemail.com

#include "stdafx.h"
#include "BigCalendarCtrl.h"
#include "BigCalendarTask.h"
#include "CalendarData.h"
#include "CalendarDefines.h"
#include "..\..\Shared\graphicsMisc.h"

#define COLOUR_DOT_TASK_START		RGB(50,200,50)
#define COLOUR_DOT_TASK_END			RGB(230,50,50)
#define COLOUR_DOT_COMPLETED_TASK	RGB(200,200,200)

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

CBigCalendarTask::CBigCalendarTask(CBigCalendarCtrl* _pParent/*=NULL*/,
								   DWORD _dwStyleCompletedTasks/*=0*/,
								   int nCellFontSize /*=8*/) 
:	CTransparentListBox(nCellFontSize), m_pParent(_pParent)
{
	BOOL bGrey = ((_dwStyleCompletedTasks & COMPLETEDTASKS_GREY) != 0);
	BOOL bStrikethru = ((_dwStyleCompletedTasks & COMPLETEDTASKS_STRIKETHRU) != 0);
	SetCompletedItemDisplayStyle(bGrey, bStrikethru);
}

CBigCalendarTask::~CBigCalendarTask()
{
}

BEGIN_MESSAGE_MAP(CBigCalendarTask, CTransparentListBox)
	//{{AFX_MSG_MAP(CBigCalendarTask)
	ON_WM_CREATE()
	ON_WM_PAINT()
	ON_WM_LBUTTONDOWN()
	ON_WM_RBUTTONDOWN()
	ON_WM_MOUSEWHEEL()
	ON_WM_SETCURSOR()
	ON_NOTIFY_EX(TTN_NEEDTEXT, 0, OnToolTipText)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

int CBigCalendarTask::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
	if (CTransparentListBox::OnCreate(lpCreateStruct) == -1)
		return -1;

	return 0;
}

void CBigCalendarTask::OnPaint() 
{
	CPaintDC dc(this);

	// base class drawing
	CTransparentListBox::DoPaint(&dc);

	// our extra drawing
	int nItems = GetCount();
	if (nItems > 0)
	{
		CRect rcListBox;
		GetWindowRect(rcListBox);
		ScreenToClient(rcListBox);

		//draw a little dot next to each task (if the item is visible)
		for (int i = 0; i < nItems; i++)
		{
			CRect rc;
			GetItemRect(i, rc);
			CPoint ptCheck(rc.left+1, rc.top+1);
			if (rcListBox.PtInRect(ptCheck))
			{
				//this item is currently visible
				CRect rcDot(rc.left, rc.top+2, rc.left+3, rc.bottom-2);

				CBrush br;
				DWORD dwTaskID = ((SItem*)GetItemData(i))->dwItemData;

				BOOL bCompleteTask = FALSE;
				m_mapTaskIDToCompleteTask.Lookup(dwTaskID, bCompleteTask);
				if (bCompleteTask)
				{
					//completed task (grey)
					br.CreateSolidBrush(COLOUR_DOT_COMPLETED_TASK);
				}
				else
				{
					BOOL bStartTask = FALSE;
					m_mapTaskIDToStartTask.Lookup(dwTaskID, bStartTask);
					if (bStartTask)
					{
						//start task (green)
						br.CreateSolidBrush(COLOUR_DOT_TASK_START);
					}
					else
					{
						//due task (red)
						br.CreateSolidBrush(COLOUR_DOT_TASK_END);
					}
				}
				dc.FillRect(&rcDot, &br);
			}
		}
	}
}

void CBigCalendarTask::OnLButtonDown(UINT nFlags, CPoint point) 
{
	CTransparentListBox::OnLButtonDown(nFlags, point);

	//inform parent of newly-selected task
	int nListboxID = GetDlgCtrlID();
	int nSelectedTask = GetCurSel();
	m_pParent->SelectTask(nListboxID, nSelectedTask);
	m_pParent->ClickedOnTaskInListbox();
}

void CBigCalendarTask::OnRButtonDown(UINT nFlags, CPoint point) 
{
	CTransparentListBox::OnRButtonDown(nFlags, point);

	//inform parent of newly-selected task
	int nListboxID = GetDlgCtrlID();
	int nSelectedTask = GetCurSel();
	m_pParent->SelectTask(nListboxID, nSelectedTask);
	m_pParent->ClickedOnTaskInListbox();

	//TODO: show popup
}

BOOL CBigCalendarTask::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt) 
{
	return m_pParent->OnMouseWheel(nFlags, zDelta, pt);
}

BOOL CBigCalendarTask::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	SetCursor(GraphicsMisc::HandCursor());
	return TRUE;
}

int CBigCalendarTask::AddItem(const CString& _strText,
							  DWORD _dwTaskID,
							  BOOL _bStartTask,
							  BOOL _bDueTask,
							  BOOL _bCompleteTask)
{
	m_mapTaskIDToStartTask.SetAt(_dwTaskID, _bStartTask);
	m_mapTaskIDToDueTask.SetAt(_dwTaskID, _bDueTask);
	m_mapTaskIDToCompleteTask.SetAt(_dwTaskID, _bCompleteTask);

	int nReturn = AddString(_strText, _dwTaskID, _bCompleteTask);

	return nReturn;
}

void CBigCalendarTask::ResetContent()
{
	m_mapTaskIDToStartTask.RemoveAll();
	m_mapTaskIDToDueTask.RemoveAll();
	m_mapTaskIDToCompleteTask.RemoveAll();

	CTransparentListBox::ResetContent();
}

void CBigCalendarTask::PreSubclassWindow()
{
	CTransparentListBox::PreSubclassWindow();
	EnableToolTips(TRUE);
}

int CBigCalendarTask::OnToolHitTest(CPoint point, TOOLINFO* pTI) const
{
	BOOL bNotUsed = FALSE;
	UINT iItemIndex = ItemFromPoint(point, bNotUsed);
	if (iItemIndex != -1)
	{
		pTI->hwnd = m_hWnd;
		pTI->uId = iItemIndex;
		GetItemRect(iItemIndex, &pTI->rect);
		pTI->lpszText = LPSTR_TEXTCALLBACK;
	}
	return iItemIndex;
}

BOOL CBigCalendarTask::OnToolTipText(UINT id, NMHDR* pTTTStruct, LRESULT* pResult)
{
	TOOLTIPTEXT* pTTT = (TOOLTIPTEXT*)pTTTStruct;

	::SendMessage(pTTT->hdr.hwndFrom, TTM_SETMAXTIPWIDTH, 0, 0);
	::SendMessage(pTTT->hdr.hwndFrom, TTM_SETDELAYTIME, TTDT_AUTOPOP, MAKELONG(SHRT_MAX, 0));

	DWORD dwTaskID = ((SItem*)GetItemData(pTTTStruct->idFrom))->dwItemData;
	
	static CString strParentsString;
	strParentsString = m_pParent->GetCalendarData()->GetTaskParentsString(dwTaskID);

	pTTT->lpszText = (LPTSTR)(LPCTSTR)strParentsString;
	*pResult = 0;

	return TRUE;    
}
