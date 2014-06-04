// TDLTaskIconDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLTaskIconDlg.h"
#include "TDCImageList.h"

#include "..\shared\Preferences.h"
#include "..\shared\holdredraw.h"
#include "..\shared\dialoghelper.h"
#include "..\shared\misc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDLTaskIconDlg dialog

const LPCTSTR NO_ICON = _T("__NONE__");

CTDLTaskIconDlg::CTDLTaskIconDlg(const CTDCImageList& ilIcons, const CString& sSelName, CWnd* pParent /*=NULL*/)
	: CDialog(CTDLTaskIconDlg::IDD, pParent), m_ilIcons(ilIcons), m_sIconName(sSelName), m_bMultiSel(FALSE)
{

}

CTDLTaskIconDlg::CTDLTaskIconDlg(const CTDCImageList& ilIcons, const CStringArray& aSelNames, CWnd* pParent /*=NULL*/)
	: CDialog(CTDLTaskIconDlg::IDD, pParent), m_ilIcons(ilIcons), m_bMultiSel(TRUE)
{
	if (aSelNames.GetSize())
		m_sIconName = aSelNames[0];

	m_aIconNames.Copy(aSelNames);
}


void CTDLTaskIconDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLTaskIconDlg)
	DDX_Control(pDX, IDC_ICONLIST, m_lcIcons);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLTaskIconDlg, CDialog)
	//{{AFX_MSG_MAP(CTDLTaskIconDlg)
	ON_NOTIFY(NM_DBLCLK, IDC_ICONLIST, OnDblclkIconlist)
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_ICONLIST, OnItemchangedIconlist)
	ON_NOTIFY(LVN_ENDLABELEDIT, IDC_ICONLIST, OnEndlabeleditIconlist)
	ON_WM_DESTROY()
	ON_WM_SIZE()
	ON_WM_GETMINMAXINFO()
	ON_BN_CLICKED(IDC_EDITLABEL, OnEditlabel)
	ON_NOTIFY(LVN_BEGINLABELEDIT, IDC_ICONLIST, OnBeginlabeleditIconlist)
	ON_WM_ERASEBKGND()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLTaskIconDlg message handlers

BOOL CTDLTaskIconDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	ListView_SetImageList(m_lcIcons, m_ilIcons, LVSIL_SMALL);
	BuildListCtrl();

	// disable OK button if nothing selected
	EnableDisable();
	m_lcIcons.SetFocus();
	
	return FALSE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

CString CTDLTaskIconDlg::GetIconName() const 
{ 
	if (m_sIconName == NO_ICON)
		return _T("");

	// else
	return m_sIconName; 
}

int CTDLTaskIconDlg::GetIconNames(CStringArray& aSelNames) const
{
	if (m_lcIcons.GetSafeHwnd())
	{
		// get directly from list itself
		aSelNames.RemoveAll();

		POSITION pos = m_lcIcons.GetFirstSelectedItemPosition();
		CString sImage;

		while (pos)
		{
			int nIndex = m_lcIcons.GetNextSelectedItem(pos);
			int nImage = m_lcIcons.GetItemData(nIndex);

			// weed out the empty icon if there are other icons
			if (nImage == -1)
			{
				if (aSelNames.GetSize() || pos)
					continue;

				aSelNames.Add(NO_ICON);
			}
			else
				aSelNames.Add(m_ilIcons.GetImageName(nImage));
		}
	}
	else // just copy what we've got
		aSelNames.Copy(m_aIconNames);

	return aSelNames.GetSize();
}

CString CTDLTaskIconDlg::GetUserIconName(const CString& sImage) const
{
	CPreferences prefs;
	return prefs.GetProfileString(_T("TaskIcons"), sImage);
}

void CTDLTaskIconDlg::EnableDisable()
{
	GetDlgItem(IDOK)->EnableWindow(!m_sIconName.IsEmpty());
	GetDlgItem(IDC_EDITLABEL)->EnableWindow(!m_sIconName.IsEmpty() && (m_sIconName != NO_ICON));
}

void CTDLTaskIconDlg::BuildListCtrl()
{
	int nSelImage = -1;

	if (m_lcIcons.GetItemCount())
		GetIconNames(m_aIconNames); // to restore selection

	// scope this else CListCtrl::EnsureVisible will not work
	{
		CHoldRedraw hr(m_lcIcons);
		CPreferences prefs;

		m_lcIcons.DeleteAllItems();
		m_lcIcons.ModifyStyle(m_bMultiSel ? LVS_SINGLESEL : 0, LVS_SORTASCENDING | (m_bMultiSel ? 0 : LVS_SINGLESEL));
		
		int nImage;
		for (nImage = 0; nImage < m_ilIcons.GetImageCount(); nImage++)
		{
			CString sImage, sKey, sBaseName;

			sBaseName = m_ilIcons.GetImageName(nImage);

			// backwards compatibility
			sKey.Format(_T("Icon%d"), nImage + 1);
			sImage = prefs.GetProfileString(_T("TaskIcons"), sKey);

			if (sImage.IsEmpty())
				sImage = prefs.GetProfileString(_T("TaskIcons"), sBaseName, sBaseName);

			if (sImage != sBaseName)
				m_mapRenamedItems[sBaseName] = sImage;

			int nIndex = m_lcIcons.InsertItem(nImage, sImage, nImage);
			m_lcIcons.SetItemData(nIndex, nImage);

			// look out for selected image
			if (!m_bMultiSel)
			{
				if (!m_sIconName.IsEmpty() && m_sIconName == sBaseName)
					nSelImage = nImage;
			}
			else
			{
				if (Misc::Find(m_aIconNames, sBaseName, FALSE, FALSE) != -1)
				{
					m_lcIcons.SetItemState(nIndex, LVIS_SELECTED, LVIS_SELECTED);

					if (nSelImage == -1)
						nSelImage = nImage;
				}
			}
		}

		// add a <none> option for turning off a task's image
		if (!m_bMultiSel)
		{
			int nIndex = m_lcIcons.InsertItem(nImage, _T("<none>"), -1);
			m_lcIcons.SetItemData(nIndex, (DWORD)-1);
		}
	}
	
	// restore selection
	int nSel = FindListItem(nSelImage);
	m_lcIcons.SetItemState(nSel, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);

	if (nSel == -1)
		m_sIconName.Empty();
	else
		m_lcIcons.EnsureVisible(nSel, FALSE);
}

void CTDLTaskIconDlg::OnDblclkIconlist(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NMITEMACTIVATE* pNMListView = ( NMITEMACTIVATE*)pNMHDR;
	
	int nImage = m_lcIcons.GetItemData(pNMListView->iItem);
	m_sIconName = m_ilIcons.GetImageName(nImage);

	// disable OK button if nothing selected
	GetDlgItem(IDOK)->EnableWindow(!m_sIconName.IsEmpty());
	
	if (!m_sIconName.IsEmpty())
		EndDialog(IDOK);

	*pResult = 0;
}

void CTDLTaskIconDlg::OnItemchangedIconlist(NMHDR* pNMHDR, LRESULT* pResult) 
{
	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;

	if (!(pNMListView->uOldState & LVIS_SELECTED) &&
		 (pNMListView->uNewState & LVIS_SELECTED))
	{
		int nImage = m_lcIcons.GetItemData(pNMListView->iItem);

		if (nImage == -1)
			m_sIconName = NO_ICON;
		else 
			m_sIconName = m_ilIcons.GetImageName(nImage);

		// disable OK button if nothing selected
		EnableDisable();
	}
		
	*pResult = 0;
}

void CTDLTaskIconDlg::OnBeginlabeleditIconlist(NMHDR* pNMHDR, LRESULT* pResult) 
{
	LV_DISPINFO* pDispInfo = (LV_DISPINFO*)pNMHDR;

	int nImage = m_lcIcons.GetItemData(pDispInfo->item.iItem);

	*pResult = (nImage == -1);
}

void CTDLTaskIconDlg::OnEndlabeleditIconlist(NMHDR* pNMHDR, LRESULT* pResult) 
{
	LV_DISPINFO* pDispInfo = (LV_DISPINFO*)pNMHDR;

	// update the currently selected name
	// and keep track of edited items
	if (pDispInfo->item.pszText)
	{
		int nImage = m_lcIcons.GetItemData(pDispInfo->item.iItem);
		m_sIconName = m_ilIcons.GetImageName(nImage);

		m_mapRenamedItems[m_sIconName] = pDispInfo->item.pszText;

		SaveRenamedImages();
		BuildListCtrl(); // to resort items
	}
	
	*pResult = 0; // don't save changes
}

void CTDLTaskIconDlg::OnDestroy() 
{
	CDialog::OnDestroy();
	
	// save off any saved names to the preferences
	SaveRenamedImages();

	// snapshot the selected icons
	GetIconNames(m_aIconNames);
}

void CTDLTaskIconDlg::SaveRenamedImages()
{
	CPreferences prefs;
	prefs.DeleteSection(_T("TaskIcons"));
	
	if (!m_mapRenamedItems.GetCount())
		return;
	
	for (int nImage = 0; nImage < m_ilIcons.GetImageCount(); nImage++)
	{
		// only save the ones that have been renamed
		CString sKey, sRename, sBaseName = m_ilIcons.GetImageName(nImage);
		
		// don't save the name if it's the same as its default name
		if (m_mapRenamedItems.Lookup(sBaseName, sRename) && sRename != sBaseName)
		{
			prefs.WriteProfileString(_T("TaskIcons"), sBaseName, sRename);
		}
	}

	m_mapRenamedItems.RemoveAll();
}

int CTDLTaskIconDlg::FindListItem(int nImage) const
{
	LVFINDINFO lvfi = { 0 };
	
	lvfi.lParam = nImage;
	lvfi.flags = LVFI_PARAM;
	lvfi.vkDirection = VK_DOWN;
	
	return m_lcIcons.FindItem(&lvfi);
}

void CTDLTaskIconDlg::OnSize(UINT nType, int cx, int cy) 
{
	static CSize sizePrev(0, 0);

	CDialog::OnSize(nType, cx, cy);
	
	// resize the list-ctrl
	if (sizePrev.cx > 0 || sizePrev.cy > 0)
	{
		CDialogHelper::ResizeCtrl(this, IDC_ICONLIST, cx - sizePrev.cx, cy - sizePrev.cy);
		CDialogHelper::OffsetCtrl(this, IDC_EDITLABEL, 0, cy - sizePrev.cy);
		CDialogHelper::OffsetCtrl(this, IDOK, cx - sizePrev.cx, cy - sizePrev.cy);
		CDialogHelper::OffsetCtrl(this, IDCANCEL, cx - sizePrev.cx, cy - sizePrev.cy);
	}

	// keep track of 'previous' size
	sizePrev.cx = cx;
	sizePrev.cy = cy;
}

void CTDLTaskIconDlg::OnGetMinMaxInfo(MINMAXINFO FAR* lpMMI) 
{
	CDialog::OnGetMinMaxInfo(lpMMI);

	lpMMI->ptMinTrackSize.x = 300;
	lpMMI->ptMinTrackSize.y = 300;
}

void CTDLTaskIconDlg::OnEditlabel() 
{
	int nItem = m_lcIcons.GetNextItem(-1, LVNI_SELECTED);

	if (nItem != -1)
	{
		m_lcIcons.SetFocus();
		m_lcIcons.EditLabel(nItem);
	}
}

BOOL CTDLTaskIconDlg::PreTranslateMessage(MSG* pMsg) 
{
	int nItem = m_lcIcons.GetNextItem(-1, LVNI_SELECTED);

	if (nItem != -1 && pMsg->message == WM_KEYUP && pMsg->wParam == VK_F2)
	{
		m_lcIcons.SetFocus();
		m_lcIcons.EditLabel(nItem);
		return TRUE;
	}
	
	return CDialog::PreTranslateMessage(pMsg);
}

BOOL CTDLTaskIconDlg::OnEraseBkgnd(CDC* pDC) 
{
	if (m_lcIcons.GetSafeHwnd())
		pDC->ExcludeClipRect(CDialogHelper::GetCtrlRect(this, IDC_ICONLIST));
	
	return CDialog::OnEraseBkgnd(pDC);
}
