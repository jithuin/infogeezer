// fileedit.cpp : implementation file
//

#include "stdafx.h"
#include "fileedit.h"
#include "folderdialog.h"
#include "enfiledialog.h"
#include "filemisc.h"
#include "enstring.h"

#include <shlwapi.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////

// static members
CString CFileEdit::s_sBrowseBtnTip;
CString CFileEdit::s_sGoBtnTip;
CString CFileEdit::s_sBrowseFilesTitle;
CString CFileEdit::s_sBrowseFoldersTitle;

void CFileEdit::SetDefaultButtonTips(LPCTSTR szBrowse, LPCTSTR szGo)
{
	if (szBrowse && *szBrowse)
		s_sBrowseBtnTip = szBrowse;

	if (szGo && *szGo)
		s_sGoBtnTip = szGo;
}

void CFileEdit::SetDefaultBrowseTitles(LPCTSTR szBrowseFiles, LPCTSTR szBrowseFolders)
{
	if (szBrowseFiles && *szBrowseFiles)
		s_sBrowseFilesTitle = szBrowseFiles;

	if (szBrowseFolders && *szBrowseFolders)
		s_sBrowseFoldersTitle = szBrowseFolders;
}


/////////////////////////////////////////////////////////////////////////////
// CFileEdit

LPCTSTR FILEMASK = _T("*?\"<>|");
LPCTSTR URLMASK = _T("*\"<>|");

const UINT VIEWBTN = 0x24;
const UINT BROWSEBTN = 0x31;

IMPLEMENT_DYNAMIC(CFileEdit, CEnEdit)

CFileEdit::CFileEdit(int nStyle, LPCTSTR szFilter) : 
					CEnEdit(nStyle & FES_COMBOSTYLEBTN),
					m_nStyle(nStyle), 
					m_bTipNeeded(FALSE),
					m_sFilter(szFilter),
					ICON_WIDTH(20),
					m_sCurFolder(FileMisc::GetCwd())
{
	if (!(m_nStyle & FES_NOBROWSE))
	{
		CString sTip(s_sBrowseBtnTip.IsEmpty() ? FILEEDIT_BROWSE : s_sBrowseBtnTip);
		AddButton(FEBTN_BROWSE, BROWSEBTN, sTip, CALC_BTNWIDTH, _T("Wingdings"));
	}

	if (m_nStyle & FES_GOBUTTON)
	{
		BOOL bFolders = (m_nStyle & FES_FOLDERS);

		CString sTip(s_sGoBtnTip.IsEmpty() ? (bFolders ? FILEEDIT_VIEWFOLDER : FILEEDIT_VIEW) : s_sGoBtnTip);
		AddButton(FEBTN_GO, VIEWBTN, sTip, CALC_BTNWIDTH, _T("Wingdings"));
	}

	// mask
	if (nStyle & FES_ALLOWURL)
		SetMask(URLMASK, ME_EXCLUDE);
	else
		SetMask(FILEMASK, ME_EXCLUDE);
}

CFileEdit::~CFileEdit()
{
}


BEGIN_MESSAGE_MAP(CFileEdit, CEnEdit)
	//{{AFX_MSG_MAP(CFileEdit)
	ON_WM_PAINT()
	ON_WM_NCCALCSIZE()
	ON_WM_KILLFOCUS()
	ON_WM_NCHITTEST()
	ON_CONTROL_REFLECT_EX(EN_CHANGE, OnChange)
	ON_WM_SETFOCUS()
	//}}AFX_MSG_MAP
	ON_MESSAGE(WM_SETTEXT, OnSetText)
	ON_NOTIFY_RANGE(TTN_NEEDTEXT, 0, 0xffff, OnNeedTooltip)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CFileEdit message handlers

void CFileEdit::EnableStyle(int nStyle, BOOL bEnable)
{
	if (bEnable)
		m_nStyle |= nStyle;
	else
		m_nStyle &= ~nStyle;

	if (GetSafeHwnd())
		SendMessage(WM_NCPAINT);
}

BOOL CFileEdit::OnChange() 
{
	EnableButton(FEBTN_GO, GetWindowTextLength());
	SendMessage(WM_NCPAINT);
	
	return FALSE;
}

LRESULT CFileEdit::OnSetText(WPARAM /*wp*/, LPARAM /*lp*/)
{
	LRESULT lr = Default();

	EnableButton(FEBTN_GO, GetWindowTextLength());
	SendMessage(WM_NCPAINT);

	return lr;
}

void CFileEdit::OnSetReadOnly(BOOL bReadOnly)
{
	EnableButton(FEBTN_BROWSE, !bReadOnly && IsWindowEnabled());
}

void CFileEdit::OnPaint() 
{
	m_bTipNeeded = FALSE;

	if (GetFocus() != this)
	{
		CString sText;
		GetWindowText(sText);

		if (sText.IsEmpty())
			Default();
		else
		{
			CPaintDC dc(this); // device context for painting
			
			CFont* pFont = GetFont();
			CFont* pFontOld = (CFont*)dc.SelectObject(pFont);
			
			// see if the text length exceeds the client width
			CRect rClient;
			GetClientRect(rClient);

			CSize sizeText = dc.GetTextExtent(sText);

			if (sizeText.cx <= rClient.Width() - 4) // -2 allows for later DeflateRect
			{
				DefWindowProc(WM_PAINT, (WPARAM)(HDC)dc, 0);
			}
			else
			{
				// fill bkgnd
				HBRUSH hBkgnd = NULL;
				
				if (!IsWindowEnabled() || (GetStyle() & ES_READONLY))
					hBkgnd = GetSysColorBrush(COLOR_3DFACE);
				else
					hBkgnd = (HBRUSH)GetParent()->SendMessage(WM_CTLCOLOREDIT, (WPARAM)(HDC)dc, (LPARAM)(HWND)GetSafeHwnd());
				
				if (!hBkgnd || hBkgnd == GetStockObject(NULL_BRUSH))
					hBkgnd = ::GetSysColorBrush(COLOR_WINDOW);
				
				::FillRect(dc, rClient, hBkgnd);
				
				rClient.DeflateRect(1, 1);
				rClient.right -= 2;
				
				dc.SetBkMode(TRANSPARENT);
				
				if (!IsWindowEnabled())
					dc.SetTextColor(::GetSysColor(COLOR_GRAYTEXT));
				
				dc.DrawText(sText, rClient, DT_PATH_ELLIPSIS);
				m_bTipNeeded = TRUE;
			}

         dc.SelectObject(pFontOld);
		}
	}
	else
		Default();
}

void CFileEdit::NcPaint(CDC* pDC, const CRect& rWindow)
{
	CEnEdit::NcPaint(pDC, rWindow);

	DrawFileIcon(pDC, rWindow);
}

void CFileEdit::DrawFileIcon(CDC* pDC, const CRect& rWindow)
{
	CRect rIcon = GetIconRect();
	rIcon.OffsetRect(-rWindow.TopLeft());
	
	HBRUSH hBkgnd = NULL;
	
	if (!IsWindowEnabled() || (GetStyle() & ES_READONLY))
		hBkgnd = GetSysColorBrush(COLOR_3DFACE);
	else
		hBkgnd = (HBRUSH)GetParent()->SendMessage(WM_CTLCOLOREDIT, (WPARAM)pDC->GetSafeHdc(), (LPARAM)(HWND)GetSafeHwnd());

	if (!hBkgnd || hBkgnd == GetStockObject(NULL_BRUSH))
		hBkgnd = ::GetSysColorBrush(COLOR_WINDOW);

	pDC->FillRect(rIcon, CBrush::FromHandle(hBkgnd));

	if (GetWindowTextLength())
	{
		CString sFilePath;
		GetWindowText(sFilePath);

		// remove double quotes
		sFilePath.Remove('\"');
		sFilePath.TrimLeft();
		sFilePath.TrimRight();

		rIcon.top++;
		rIcon.left++;
		
		int nImage = -1;

		if (HasStyle(FES_FOLDERS))
			nImage = m_ilSys.GetFolderImageIndex();
		else
		{
			// try parent for override
			HICON hIcon = (HICON)GetParent()->SendMessage(WM_FE_GETFILEICON, GetDlgCtrlID(), 
															(LPARAM)(LPCTSTR)sFilePath);

			if (!hIcon || !::DrawIconEx(pDC->GetSafeHdc(), rIcon.left, rIcon.top, hIcon,
								 16, 16, 0, NULL, DI_NORMAL))
			{
				nImage = m_ilSys.GetFileImageIndex(sFilePath);
			}
		}

		if (nImage != -1)
			m_ilSys.GetImageList()->Draw(pDC, nImage, rIcon.TopLeft(), ILD_TRANSPARENT);
	}
}

void CFileEdit::OnBtnClick(UINT nID)
{
	switch (nID)
	{
	case FEBTN_BROWSE:
		{
			// show browse dialog
			CString sFilename;
			GetWindowText(sFilename);

			// filedialog spits if file is actually a url
			if (::PathIsURL(sFilename))
				sFilename = m_sCurFolder;

			else if (!sFilename.IsEmpty()) // make fullpath
				FileMisc::MakeFullPath(sFilename, m_sCurFolder);
			
			if (HasStyle(FES_FOLDERS))
			{
				// if folder not exists revert to current folder
				if (!FileMisc::FolderExists(sFilename))
					sFilename = m_sCurFolder;

				CString sTitle(s_sBrowseFoldersTitle.IsEmpty() ? FILEEDIT_SELECTFOLDER : s_sBrowseFoldersTitle);
				CFolderDialog dialog(sTitle, sFilename);
				
				if (dialog.DoModal() == IDOK)
				{
					SetFocus();

					CString sPath(dialog.GetFolderPath());

					if (HasStyle(FES_RELATIVEPATHS))
						sPath = FileMisc::GetRelativePath(sPath, m_sCurFolder, TRUE);

					SetWindowText(sPath);
					
					// send parent a change notification
					GetParent()->SendMessage(WM_COMMAND, MAKELPARAM(EN_CHANGE, GetDlgCtrlID()), (LPARAM)GetSafeHwnd());
				}
			}
			else
			{
				BOOL bOpenFileDlg = !HasStyle(FES_SAVEAS);
				DWORD dwFlags = bOpenFileDlg ? EOFN_DEFAULTOPEN : EOFN_DEFAULTSAVE;

				// if file not exists revert to current folder
				if (!FileMisc::FileExists(sFilename))
					sFilename.Empty();

				CString sTitle(s_sBrowseFilesTitle.IsEmpty() ? FILEEDIT_BROWSE_TITLE : s_sBrowseFilesTitle);
				CEnFileDialog dialog(bOpenFileDlg, sTitle, NULL, sFilename, dwFlags, m_sFilter);

				// custom attributes
				if (sFilename.IsEmpty())
					dialog.m_ofn.lpstrInitialDir = m_sCurFolder;

				if (dialog.DoModal() == IDOK)
				{
					SetFocus();

					CString sPath(dialog.GetPathName());

					if (HasStyle(FES_RELATIVEPATHS))
						sPath = FileMisc::GetRelativePath(sPath, m_sCurFolder, FALSE);

					SetWindowText(sPath);
					
					// send parent a change notification
					GetParent()->SendMessage(WM_COMMAND, MAKELPARAM(EN_CHANGE, GetDlgCtrlID()), (LPARAM)GetSafeHwnd());
				}
			}
		}
		break;
		
	case FEBTN_GO:
		{
			CString sPath;
			GetWindowText(sPath);
			
			sPath.TrimLeft();
			sPath.TrimRight();
			
			if (!sPath.IsEmpty())
			{
				CWaitCursor cursor;
				
				// set the current directory in case its a relative path
				if (!m_sCurFolder.IsEmpty())
					SetCurrentDirectory(m_sCurFolder);

				// try our parent first
				if (!GetParent()->SendMessage(WM_FE_DISPLAYFILE, GetDlgCtrlID(), (LPARAM)(LPCTSTR)sPath))
				{
					GotoFile(sPath, m_sCurFolder); 
				}
			}
		}
		break;
	}
}

int CFileEdit::GotoFile(LPCTSTR szPath, BOOL bHandleError)
{
	return GotoFile(szPath, FileMisc::GetCwd(), bHandleError);
}

BOOL CFileEdit::GotoFile(LPCTSTR szPath, LPCTSTR szFolder, BOOL bHandleError)
{
	int nRes = FileMisc::Run(*AfxGetMainWnd(), szPath, szFolder, SW_SHOWNORMAL); 
	
	if ((nRes < 32) && bHandleError)
	{
		CEnString sMessage, sFullPath = FileMisc::GetFullPath(szPath, szFolder);
				
		switch (nRes)
		{
		case SE_ERR_FNF:
			sMessage.Format(FILEEDIT_FILENOTFOUND, sFullPath);
			break;
			
		case SE_ERR_NOASSOC:
			sMessage.Format(FILEEDIT_ASSOCAPPFAILURE, sFullPath);
			break;
			
		default:
			sMessage.Format(FILEEDIT_FILEOPENFAILED, sFullPath, nRes);
			break;
		}
		
		AfxMessageBox(sMessage, MB_OK);
	}

	return nRes;
}

void CFileEdit::SetCurrentFolder(LPCTSTR szFolder)
{
	m_sCurFolder = szFolder; 
	FileMisc::UnterminatePath(m_sCurFolder);
}

void CFileEdit::OnNcCalcSize(BOOL bCalcValidRects, NCCALCSIZE_PARAMS FAR* lpncsp) 
{
	CEnEdit::OnNcCalcSize(bCalcValidRects, lpncsp);

	if (bCalcValidRects)
		lpncsp->rgrc[0].left += ICON_WIDTH;
}

void CFileEdit::OnKillFocus(CWnd* pNewWnd) 
{
	CEnEdit::OnKillFocus(pNewWnd);
	
	Invalidate();
}

CRect CFileEdit::GetIconRect() const
{
	CRect rButton;
	GetClientRect(rButton);

	rButton.right = rButton.left;
	rButton.left -= ICON_WIDTH;
	rButton.top -= m_nTopBorder;
	rButton.bottom += m_nBottomBorder;

	ClientToScreen(rButton);

	return rButton;
}

#if _MSC_VER >= 1400
LRESULT CFileEdit::OnNcHitTest(CPoint point)
#else
UINT CFileEdit::OnNcHitTest(CPoint point)
#endif
{
	if (GetIconRect().PtInRect(point))
		return HTBORDER;
	
	return CEnEdit::OnNcHitTest(point);
}

void CFileEdit::OnNeedTooltip(UINT /*id*/, NMHDR* pNMHDR, LRESULT* pResult)
{
	TOOLTIPTEXT* pTTT = (TOOLTIPTEXT*)pNMHDR;
	*pResult = 0;

	switch (pTTT->hdr.idFrom)
	{
	case -1:
		if (m_bTipNeeded)
		{
			static CString sFilePath;
			GetWindowText(sFilePath);
 
			pTTT->lpszText = (LPTSTR)(LPCTSTR)sFilePath;
		}
		break;
	}
}

void CFileEdit::RecalcBtnRects()
{
	CEnEdit::RecalcBtnRects();

	if (m_tooltip.GetSafeHwnd())
	{
		CRect rClient;
		GetClientRect(rClient);

		m_tooltip.SetToolRect(this, (UINT)-1, rClient);
	}
}

void CFileEdit::OnSetFocus(CWnd* pOldWnd) 
{
	CEnEdit::OnSetFocus(pOldWnd);
	
	Invalidate();	
}

void CFileEdit::PreSubclassWindow() 
{
	CEnEdit::PreSubclassWindow();
	
	EnableButton(FEBTN_GO, GetWindowTextLength());
	m_bFirstShow = TRUE;
}

BOOL CFileEdit::InitializeTooltips()
{
	// create tooltip
	if (!m_tooltip.GetSafeHwnd() && CEnEdit::InitializeTooltips())
	{
		CRect rClient;
		GetClientRect(rClient);

		m_tooltip.AddTool(this, LPSTR_TEXTCALLBACK, rClient, (UINT)-1);
	}

	return (m_tooltip.GetSafeHwnd() != NULL);
}

