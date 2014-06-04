#if !defined(AFX_TODOCTRLTREEVIEW_H__7FD70732_B37D_494A_9CEE_5C9AA08C8503__INCLUDED_)
#define AFX_TODOCTRLTREEVIEW_H__7FD70732_B37D_494A_9CEE_5C9AA08C8503__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ToDoCtrlTreeView.h : header file
//

#include "..\shared\orderedTreectrl.h"

/////////////////////////////////////////////////////////////////////////////
// CTDCTreeView window

class CTDCTreeView : public COrderedTreeCtrl
{
// Construction
public:
	CTDCTreeView();

// Attributes
public:

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDCTreeView)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CTDCTreeView();

	// Generated message map functions
protected:
	//{{AFX_MSG(CTDCTreeView)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TODOCTRLTREEVIEW_H__7FD70732_B37D_494A_9CEE_5C9AA08C8503__INCLUDED_)
