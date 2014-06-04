#if !defined(AFX_TDCMSG_H__5951FDE6_508A_4A9D_A55D_D16EB026AEF7__INCLUDED_)
#define AFX_TDCMSG_H__5951FDE6_508A_4A9D_A55D_D16EB026AEF7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

// tdcmsg.h : header file
//
static LPCTSTR TDL_PROTOCOL = _T("tdl://");
static LPCTSTR TDL_EXTENSION = _T(".tdl");

// notification messages
const UINT WM_TDCN_MODIFY = ::RegisterWindowMessage(_T("WM_TDCN_MODIFY")); // lParam == <TDC_ATTRIBUTE>
const UINT WM_TDCN_SORT = ::RegisterWindowMessage(_T("WM_TDCN_SORT")); 
const UINT WM_TDCN_COMMENTSCHANGE = ::RegisterWindowMessage(_T("WM_TDCN_COMMENTSCHANGE"));
const UINT WM_TDCN_COMMENTSKILLFOCUS = ::RegisterWindowMessage(_T("WM_TDCN_COMMENTSKILLFOCUS"));
const UINT WM_TDCN_TIMETRACK = ::RegisterWindowMessage(_T("WM_TDCN_TIMETRACK")); // lParam = 0/1 => stop/start
const UINT WM_TDCN_VIEWPRECHANGE = ::RegisterWindowMessage(_T("WM_TDCN_VIEWPRECHANGE"));
const UINT WM_TDCN_VIEWPOSTCHANGE = ::RegisterWindowMessage(_T("WM_TDCN_VIEWPOSTCHANGE"));
const UINT WM_TDCN_SELECTIONCHANGE = ::RegisterWindowMessage(_T("WM_TDCN_SELECTIONCHANGE"));
const UINT WM_TDCN_RECREATERECURRINGTASK = ::RegisterWindowMessage(_T("WM_TDCN_RECREATERECURRINGTASK"));
const UINT WM_TDCN_DOUBLECLKREMINDERCOL = ::RegisterWindowMessage(_T("WM_TDCN_DOUBLECLKREMINDERCOL"));

// from the filterbar
const UINT WM_FBN_FILTERCHNG = ::RegisterWindowMessage(_T("WM_FBN_FILTERCHNG")); 

// sent when one of the auto dropdown lists is changed
const UINT WM_TDCN_LISTCHANGE = ::RegisterWindowMessage(_T("WM_TDCN_LISTCHANGE")); // lParam == <TDC_ATTRIBUTE>

// request messages
const UINT WM_TDCM_GETCLIPBOARD = ::RegisterWindowMessage(_T("WM_TDCM_GETCLIPBOARD")); // lParam == match hwnd
const UINT WM_TDCM_HASCLIPBOARD = ::RegisterWindowMessage(_T("WM_TDCM_HASCLIPBOARD")); // lParam == match hwnd
const UINT WM_TDCM_TASKISDONE = ::RegisterWindowMessage(_T("WM_TDCM_TASKISDONE")); // format as WM_TDCM_TASKLINK
const UINT WM_TDCM_TASKHASREMINDER = ::RegisterWindowMessage(_T("WM_TDCM_TASKHASREMINDER")); // wParam = TaskID, lParam = TDC* 

// instruction messages
// sent when a task outside 'this' todoctrl needs displaying
const UINT WM_TDCM_TASKLINK = ::RegisterWindowMessage(_T("WM_TDCM_TASKLINK")); // wParam = taskID, lParam = taskfile
const UINT WM_TDCM_LENGTHYOPERATION = ::RegisterWindowMessage(_T("WM_TDCM_LENGTHYOPERATION")); // wParam = start/stop, lParam = text to display

// internal TDC message
const UINT WM_TDC_RESTOREFOCUSEDITEM			= (WM_APP + 1);
const UINT WM_TDC_REFRESHPERCENTSPINVISIBILITY	= (WM_APP + 2);
const UINT WM_TDC_REFRESHFILTER					= (WM_APP + 3);
const UINT WM_TDC_RECREATERECURRINGTASK			= (WM_APP + 4);



#endif // AFX_TDCMSG_H__5951FDE6_508A_4A9D_A55D_D16EB026AEF7__INCLUDED_