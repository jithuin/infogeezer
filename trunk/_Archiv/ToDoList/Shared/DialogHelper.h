// EDialog.h: interface for the EDialog class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_DIALOGHELPER_H__8543A453_171B_11D4_AE08_0000E8425C3E__INCLUDED_)
#define AFX_DIALOGHELPER_H__8543A453_171B_11D4_AE08_0000E8425C3E__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

enum RT_CTRLSTATE // for SetCtrlState
{
	RTCS_ENABLED,
	RTCS_DISABLED,
	RTCS_READONLY,
};

class CDialogHelper
{
public:
	// safe versions if the window text is empty
	static void DDX_Text(CDataExchange* pDX, int nIDC, BYTE& value);
	static void DDX_Text(CDataExchange* pDX, int nIDC, short& value);
	static void DDX_Text(CDataExchange* pDX, int nIDC, int& value);
	static void DDX_Text(CDataExchange* pDX, int nIDC, UINT& value);
	static void DDX_Text(CDataExchange* pDX, int nIDC, long& value);
	static void DDX_Text(CDataExchange* pDX, int nIDC, DWORD& value);
	static void DDX_Text(CDataExchange* pDX, int nIDC, CString& value);
	static void DDX_Text(CDataExchange* pDX, int nIDC, float& value);
	static void DDX_Text(CDataExchange* pDX, int nIDC, double& value);

	static UINT MessageBoxEx(CWnd* pWnd, UINT nIDText, UINT nIDCaption, UINT nType = MB_OK);
	static UINT MessageBoxEx(CWnd* pWnd, LPCTSTR szText, UINT nIDCaption, UINT nType = MB_OK);

	static BOOL IsChildOrSame(HWND hWnd, HWND hwndChild);
	static BOOL ControlWantsEnter(HWND hwnd);
	static CString GetControlLabel(const CWnd* pWnd);

	static int GetChildControlIDs(const CWnd* pParent, CUIntArray& aCtrlIDs, LPCTSTR szClass = NULL);
	static void RemoveControlID(UINT nCtrlID, CUIntArray& aCtrlIDs);

	// font helper
	static void SetFont(CWnd* pWnd, HFONT hFont, BOOL bRedraw = TRUE);
    static HFONT GetFont(const CWnd* pWnd);
    static HFONT GetFont(HWND hWnd);
	
	// comboboxes
	static int SetComboBoxItems(CComboBox& combo, const CStringArray& aItems);
	static int RefreshMaxDropWidth(CComboBox& combo, CDC* pDCRef = NULL, int nTabWidth = 0);
	static int CalcMaxTextWidth(CComboBox& combo, int nMinWidth = 0, BOOL bDropped = FALSE, CDC* pDCRef = NULL, int nTabWidth = 0);
	static BOOL SelectItemByValue(CComboBox& combo, int nValue);
	static BOOL SelectItemByData(CComboBox& combo, DWORD dwItemData);
	static int GetSelectedItemAsValue(const CComboBox& combo);
	static CString GetSelectedItem(const CComboBox& combo);
	static DWORD GetSelectedItemData(const CComboBox& combo);
	static BOOL IsDroppedComboBox(HWND hCtrl);
	static int FindItemByValue(const CComboBox& combo, int nValue);
	static int AddString(CComboBox& combo, LPCTSTR szItem, DWORD dwItemData);

	// listboxes
	static int RefreshMaxColumnWidth(CListBox& list, CDC* pDCRef = NULL);
	static int CalcMaxTextWidth(CListBox& list, int nMinWidth = 0, CDC* pDCRef = NULL);

	// better dialog control shortcut handling
	static BOOL ProcessDialogControlShortcut(const MSG* pMsg);
	static UINT GetShortcut(const CString& sText);

	// static helpers
	static CRect OffsetCtrl(const CWnd* pParent, UINT nCtrlID, int dx = 0, int dy = 0); 
	static CRect OffsetCtrl(const CWnd* pParent, CWnd* pChild, int dx = 0, int dy = 0);
	static CRect ResizeCtrl(const CWnd* pParent, UINT nCtrlID, int cx = 0, int cy = 0);
	static CRect MoveCtrl(const CWnd* pParent, UINT nCtrlID, int x = 0, int y = 0);

	static void OffsetCtrls(const CWnd* pParent, const CUIntArray& aCtrlIDs, int x = 0, int y = 0); 
	static void ResizeCtrls(const CWnd* pParent, const CUIntArray& aCtrlIDs, int cx = 0, int cy = 0);
	static void MoveCtrls(const CWnd* pParent, const CUIntArray& aCtrlIDs, int x = 0, int y = 0);

	static CRect GetCtrlRect(const CWnd* pParent, UINT nCtrlID);
	static CRect GetCtrlRect(const CWnd* pParent, const CWnd* pChild);

	static void SetControlState(const CWnd* pParent, UINT nCtrlID, RT_CTRLSTATE nState);
	static void SetControlState(HWND hCtrl, RT_CTRLSTATE nState);
	static void SetControlsState(const CWnd* pParent, UINT nCtrlIDFrom, UINT nCtrlIDTo, RT_CTRLSTATE nState);

	static void ShowControls(const CWnd* pParent, UINT nCtrlIDFrom, UINT nCtrlIDTo, BOOL bShow = TRUE);
	static void ShowControls(const CWnd* pParent, const CUIntArray& aCtrlIDs, BOOL bShow = TRUE);
	static void ShowControl(const CWnd* pParent, UINT nCtrlID, BOOL bShow = TRUE);
	static void ShowControl(CWnd* pCtrl, BOOL bShow = TRUE);

	static void ExcludeControls(const CWnd* pParent, CDC* pDC, UINT nCtrlIDFrom, UINT nCtrlIDTo, BOOL bIgnoreCorners = FALSE);
	static void ExcludeChildren(const CWnd* pParent, CDC* pDC, BOOL bIgnoreCorners = FALSE);
	static void ExcludeChild(const CWnd* pParent, CDC* pDC, const CWnd* pChild, BOOL bIgnoreCorners = FALSE);
	static void ExcludeChild(const CWnd* pParent, CDC* pDC, UINT nCtrlID, BOOL bIgnoreCorners = FALSE);

protected:
	CDialogHelper() : m_bInUpdateEx(FALSE) {}

	// helpers for updating only a single item
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, BYTE& value, BOOL bSaveAndValidate = TRUE);
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, short& value, BOOL bSaveAndValidate = TRUE);
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, int& value, BOOL bSaveAndValidate = TRUE);
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, UINT& value, BOOL bSaveAndValidate = TRUE);
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, long& value, BOOL bSaveAndValidate = TRUE);
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, DWORD& value, BOOL bSaveAndValidate = TRUE);
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, CString& value, BOOL bSaveAndValidate = TRUE);
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, float& value, BOOL bSaveAndValidate = TRUE);
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, double& value, BOOL bSaveAndValidate = TRUE);
	BOOL UpdateDataEx(CWnd* pWnd, int nIDC, CWnd& ctrl, BOOL bSaveAndValidate = TRUE);
	BOOL InUpdateEx() { return m_bInUpdateEx; }

	// for combo and listboxes only
	BOOL UpdateDataExact(CWnd* pWnd, int nIDC, CString& value, BOOL bSaveAndValidate = TRUE);

	static CWnd* FindNextMatch(CWnd* pCurrent, UINT nShortcut);

private:
	BOOL m_bInUpdateEx;

};

#endif // !defined(AFX_DIALOGHELPER_H__8543A453_171B_11D4_AE08_0000E8425C3E__INCLUDED_)
