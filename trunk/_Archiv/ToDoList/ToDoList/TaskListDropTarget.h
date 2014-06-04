// TaskListDropTarget.h: interface for the CTaskListDropTarget class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TASKLISTDROPTARGET_H__56519FB1_2923_45BB_97A2_08B8B1DC7C97__INCLUDED_)
#define AFX_TASKLISTDROPTARGET_H__56519FB1_2923_45BB_97A2_08B8B1DC7C97__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <afxole.h>

// predecs
namespace OutlookAPI
{
	class Selection;
}

struct TLDT_DATA
{	
	TLDT_DATA() : hti(NULL), nItem(-1), pObject(NULL), pFilePaths(NULL), pOutlookSelection(NULL) {}

	CString GetFirstFile() const { return (pFilePaths && pFilePaths->GetSize()) ? pFilePaths->GetAt(0) : _T(""); }
	int GetFileCount() const { return (pFilePaths ? pFilePaths->GetSize() : 0); }
	CString GetFile(int nFile) const { return (pFilePaths && nFile < pFilePaths->GetSize()) ? pFilePaths->GetAt(nFile) : _T(""); }

	HTREEITEM hti;
	int nItem;
	COleDataObject* pObject;
	CStringArray* pFilePaths;
	OutlookAPI::Selection* pOutlookSelection;
};

class COutlookHelper;

const UINT WM_TLDT_DROP = ::RegisterWindowMessage(_T("WM_TLDT_DROP")); // wparam = TLDT_DATA*, lParam == (CWnd*)target

class CTaskListDropTarget : public COleDropTarget  
{
public:
	CTaskListDropTarget();
	virtual ~CTaskListDropTarget();

	BOOL Register(CWnd* pTarget, CWnd* pOwner);

protected:
	CWnd* m_pOwner;
	int m_nLVPrevHilite;
	COutlookHelper* m_pOutlook;

protected:
	virtual DROPEFFECT OnDragEnter(CWnd* pWnd, COleDataObject* pObject, DWORD dwKeyState, CPoint point);
	virtual void OnDragLeave(CWnd* pWnd);
	virtual DROPEFFECT OnDragOver(CWnd* pWnd, COleDataObject* pObject, DWORD dwKeyState, CPoint point);
	virtual BOOL OnDrop(CWnd* pWnd, COleDataObject* pObject, DROPEFFECT dropEffect, CPoint point);

protected:

	enum TLDT_HITTEST
	{
		TLDTHT_NONE = -1,
		TLDTHT_TREE,
		TLDTHT_LIST,
		TLDTHT_FILEEDIT,
		TLDTHT_CAPTION,
	};

	TLDT_HITTEST DoHitTest(CWnd* pWnd, CPoint point, HTREEITEM& htiHit, int& nHit);
	BOOL InitializeOutlook();
};

#endif // !defined(AFX_TASKLISTDROPTARGET_H__56519FB1_2923_45BB_97A2_08B8B1DC7C97__INCLUDED_)
