// ToDoCtrlTreeView.cpp : implementation file
//

#include "stdafx.h"
#include "TDCTreeView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDCTreeView

CTDCTreeView::CTDCTreeView() : COrderedTreeCtrl(NCGS_SHOWHEADER)
{
}

CTDCTreeView::~CTDCTreeView()
{
}


BEGIN_MESSAGE_MAP(CTDCTreeView, COrderedTreeCtrl)
	//{{AFX_MSG_MAP(CTDCTreeView)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDCTreeView message handlers
