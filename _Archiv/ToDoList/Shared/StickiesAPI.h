#if !defined(AFX_STICKIESAPI_H__CD9AB24A_FE93_46D1_98FA_FC1A192427DE__INCLUDED_)
#define AFX_STICKIESAPI_H__CD9AB24A_FE93_46D1_98FA_FC1A192427DE__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// stickiesapi.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CStickiesAPI 

const LRESULT STICKY_TIMEOUT = 0xffffffff;
const LRESULT STICKY_SUCCESS = 0;

class CStickiesAPI 
{
// Construction
public:
	CStickiesAPI(LPCTSTR szStickiesPath = NULL);

	BOOL Initialize(CWnd* pCallback, int nCommandID = 1, LPCTSTR szStickiesPath = NULL);
	void Release();

	BOOL IsValid() const { return (m_hwndCallback != NULL); }
	LRESULT SendCommand(int nCommandID, LPCTSTR szCommand, LPCTSTR szStickyID = NULL, LPCTSTR szExtra = NULL);

	static CString FormatCommandString(LPCTSTR szCommand, LPCTSTR szStickyID = NULL, LPCTSTR szExtra = NULL);

// Attributes
protected:
	CString m_sStickiesPath;
	HWND m_hwndCallback;
	int m_nInitialCmdID;

// Implementation
public:
	virtual ~CStickiesAPI();

protected:
	HWND GetStickiesWindow(BOOL bAutoStart = TRUE);
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_STICKIESAPI_H__CD9AB24A_FE93_46D1_98FA_FC1A192427DE__INCLUDED_)
