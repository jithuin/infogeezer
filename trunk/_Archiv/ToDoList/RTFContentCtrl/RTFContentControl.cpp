// RTFContentControl.cpp : implementation file
//

#include "stdafx.h"
#include "RTFContentCtrl.h"
#include "RTFContentControl.h"
//#include "EditWebLinkDlg.h"
#include "CreateFileLinkDlg.h"

#include "..\todolist\tdcmsg.h"
#include "..\todolist\tdlschemadef.h"

#include "..\shared\itasklist.h"
#include "..\shared\enfiledialog.h"
#include "..\shared\autoflag.h"
#include "..\shared\richedithelper.h"
#include "..\shared\misc.h"
#include "..\shared\filemisc.h"
#include "..\shared\uitheme.h"
#include "..\shared\enstring.h"
#include "..\shared\preferences.h"
#include "..\shared\binarydata.h"
#include "..\shared\mswordhelper.h"
#include "..\shared\enmenu.h"

#include "..\3rdparty\compression.h"
#include "..\3rdparty\zlib\zlib.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CRTFContentControl

CRTFContentControl::CRTFContentControl() : m_bAllowNotify(TRUE), m_reSpellCheck(m_rtf)
{
	// add custom protocol to comments field for linking to task IDs
	m_rtf.AddProtocol(TDL_PROTOCOL, TRUE);
	m_rtf.SetGotoErrorMsg(CEnString(IDS_COMMENTSGOTOERRMSG));
}

CRTFContentControl::~CRTFContentControl()
{
}


BEGIN_MESSAGE_MAP(CRTFContentControl, CRulerRichEditCtrl)
	//{{AFX_MSG_MAP(CRTFContentControl)
	ON_WM_CONTEXTMENU()
	ON_WM_CREATE()
	ON_WM_DESTROY()
	//}}AFX_MSG_MAP
	ON_EN_CHANGE(RTF_CONTROL, OnChangeText)
	ON_EN_KILLFOCUS(RTF_CONTROL, OnKillFocus)
	ON_MESSAGE(WM_SETFONT, OnSetFont)
	ON_WM_STYLECHANGING()
	ON_REGISTERED_MESSAGE(WM_UREN_CUSTOMURL, OnCustomUrl)
//	ON_NOTIFY_RANGE(TTN_NEEDTEXTA, 0, 0xffff, OnNeedTooltipText)
//	ON_NOTIFY_RANGE(TTN_NEEDTEXTW, 0, 0xffff, OnNeedTooltipText)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CRTFContentControl message handlers

void CRTFContentControl::OnChangeText() 
{
	if (m_bAllowNotify && !m_rtf.IsIMEComposing())
		GetParent()->SendMessage(WM_TDCN_COMMENTSCHANGE);
}

/*
void CRTFContentControl::OnNeedTooltipText(UINT nID, NMHDR* pNMHDR, LRESULT* pResult)
{
	static CString sTipText(_T("some text"));
	//sTipText.Empty();

	if (!sTipText.IsEmpty())
	{
		TOOLTIPTEXT* pTTT = (TOOLTIPTEXT*)pNMHDR;
		pTTT->lpszText = (LPTSTR)(LPCTSTR)sTipText;
	}

	*pResult = 0;
}
*/
void CRTFContentControl::OnKillFocus() 
{
	if (m_bAllowNotify)
		GetParent()->SendMessage(WM_TDCN_COMMENTSKILLFOCUS);
}

LRESULT CRTFContentControl::OnSetFont(WPARAM wp, LPARAM lp)
{
	// richedit2.0 sends a EN_CHANGE notification if it contains
	// text when it receives a font change.
	// to us though this is a bogus change so we prevent a notification
	// being sent
	CAutoFlag af(m_bAllowNotify, FALSE);

	return CRulerRichEditCtrl::OnSetFont(wp, lp);
}

// ICustomControl implementation
int CRTFContentControl::GetContent(unsigned char* pContent) const
{
	return GetContent(this, pContent);
}

// hack to get round GetRTF not being const
int CRTFContentControl::GetContent(const CRTFContentControl* pCtrl, unsigned char* pContent)
{
	int nLen = 0;
	
	if (pContent)
	{
		CString sContent;
		
		// cast away constness
		sContent = ((CRTFContentControl*)pCtrl)->GetRTF();
		nLen = sContent.GetLength();
		
		// compress it
		if (!nLen)
			return 0;

		unsigned char* pCompressed = NULL;
		int nLenCompressed = 0;

		if (Compression::Compress((unsigned char*)(LPSTR)(LPCTSTR)sContent, nLen, pCompressed, nLenCompressed) && nLenCompressed)
		{
			CopyMemory(pContent, pCompressed, nLenCompressed);
			nLen = nLenCompressed;
			delete [] pCompressed;
		}
		else
			nLen = 0;
	}
	else
		nLen = ((CRTFContentControl*)pCtrl)->GetRTFLength();
	
	return nLen;
}

bool CRTFContentControl::SetContent(const unsigned char* pContent, int nLength, BOOL bResetSelection)
{
	unsigned char* pDecompressed = NULL;

	// content may need decompressing 
	// always work in bytes
	if (nLength && strncmp((const char*)pContent, RTFTAG, LENTAG) != 0)
	{
		int nLenDecompressed = 0;

		if (Compression::Decompress(pContent, nLength, pDecompressed, nLenDecompressed))
		{
			pContent = pDecompressed;
			nLength = nLenDecompressed;
		}
		else
			return false;
	}

	// content must begin with rtf tag or be empty
	if (nLength && (nLength < LENTAG || strncmp((const char*)pContent, RTFTAG, LENTAG)))
		return false;

	CAutoFlag af(m_bAllowNotify, FALSE);
	CReSaveCaret* pSave = NULL;

	// save caret position
	if (!bResetSelection)
		pSave = new CReSaveCaret(m_rtf);

	CString sContent;

#ifdef _UNICODE
	CBinaryData(pContent, nLength).Get(sContent);
#else
	memcpy(sContent.GetBufferSetLength(nLength), pContent, nLength);
#endif

	SetRTF(sContent);
	m_rtf.ParseAndFormatText(TRUE);

	delete [] pDecompressed;
	
	// restore caret
	if (!bResetSelection)
		delete pSave;
	else // or reset
		m_rtf.SetSel(0, 0);

	return true; 
}

int CRTFContentControl::GetTextContent(LPTSTR szContent, int nLength) const
{
	if (!szContent)
		return m_rtf.GetWindowTextLength();

	ASSERT(nLength > 0);
	
	return m_rtf.GetWindowText(szContent, nLength);
}

bool CRTFContentControl::SetTextContent(LPCTSTR szContent, BOOL bResetSelection)
{
	CAutoFlag af(m_bAllowNotify, TRUE);
	CReSaveCaret* pSave = NULL;

	// save caret position
	if (!bResetSelection)
		pSave = new CReSaveCaret(m_rtf);

	m_rtf.SendMessage(WM_SETTEXT, 0, (LPARAM)szContent);

	// restore caret
	if (!bResetSelection)
		delete pSave;
	else // or reset
		m_rtf.SetSel(0, 0);

	return true; 
}

void CRTFContentControl::SetUITheme(const UITHEME* pTheme)
{
	m_toolbar.SetBackgroundColors(pTheme->crToolbarLight, pTheme->crToolbarDark, pTheme->nRenderStyle != UIRS_GLASS, pTheme->nRenderStyle != UIRS_GRADIENT);
	m_ruler.SetBackgroundColor(pTheme->crToolbarLight);
}

HWND CRTFContentControl::GetHwnd() const
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	
	return GetSafeHwnd();
}

LPCTSTR CRTFContentControl::GetTypeID() const
{
	static CString sID;

	Misc::GuidToString(RTF_TYPEID, sID);

	return sID;
}

void CRTFContentControl::SetReadOnly(bool bReadOnly)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CRulerRichEditCtrl::SetReadOnly((BOOL)bReadOnly);
}

void CRTFContentControl::Release()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	DestroyWindow();
	delete this;
}

void CRTFContentControl::EnableMenuItem(CMenu* pMenu, UINT nCmdID, BOOL bEnable)
{
	pMenu->EnableMenuItem(nCmdID, MF_BYCOMMAND | (bEnable ? MF_ENABLED : MF_GRAYED));
}

void CRTFContentControl::CheckMenuItem(CMenu* pMenu, UINT nCmdID, BOOL bCheck)
{
	pMenu->CheckMenuItem(nCmdID, MF_BYCOMMAND | (bCheck ? MF_CHECKED  : MF_UNCHECKED));
}

BOOL CRTFContentControl::RemoveAdvancedFeatures(CMenu* pMenu)
{
	BOOL bRemoveAdvancedFeatures = ((FileMisc::GetModuleDriveType() == DRIVE_FIXED) && 
									!CMSWordHelper::IsWordInstalled(12));

	if (bRemoveAdvancedFeatures)
	{
		CEnMenu::DeleteMenu(*pMenu, ID_EDIT_SUPERSCRIPT, MF_BYCOMMAND, TRUE);
		CEnMenu::DeleteMenu(*pMenu, ID_EDIT_SUBSCRIPT, MF_BYCOMMAND, TRUE);
		CEnMenu::DeleteMenu(*pMenu, BUTTON_INSERTTABLE, MF_BYCOMMAND, TRUE);
		CEnMenu::DeleteMenu(*pMenu, ID_EDIT_HORZRULE, MF_BYCOMMAND, TRUE);
	}

	return bRemoveAdvancedFeatures;
}

void CRTFContentControl::OnContextMenu(CWnd* pWnd, CPoint point) 
{
	if (pWnd == &m_rtf)
	{
		// prepare a simple edit menu
		CMenu menu;

		if (menu.LoadMenu(IDR_POPUP))
		{
			CMenu* pPopup = menu.GetSubMenu(0);

			if (pPopup)
			{
				BOOL bCanEdit = m_rtf.IsWindowEnabled() && !(m_rtf.GetStyle() & ES_READONLY);
				BOOL bHasText = m_rtf.GetTextLength();

				CHARRANGE cr;
				m_rtf.GetSel(cr);

				BOOL bHasSel = (cr.cpMax - cr.cpMin);

				CharFormat cf(CFM_BOLD | CFM_ITALIC | CFM_UNDERLINE | CFM_STRIKEOUT | 
								CFM_COLOR | CFM_BACKCOLOR | CFM_SUBSCRIPT | CFM_SUPERSCRIPT);
				m_rtf.GetSelectionCharFormat(cf);
				
				CheckMenuItem(pPopup, BUTTON_BOLD, (cf.dwEffects & CFE_BOLD));
				CheckMenuItem(pPopup, BUTTON_ITALIC, (cf.dwEffects & CFE_ITALIC));
				CheckMenuItem(pPopup, BUTTON_UNDERLINE, (cf.dwEffects & CFE_UNDERLINE));
				CheckMenuItem(pPopup, BUTTON_STRIKETHRU, (cf.dwEffects & CFE_STRIKEOUT));

				if (!RemoveAdvancedFeatures(pPopup))
				{
					CheckMenuItem(pPopup, ID_EDIT_SUPERSCRIPT, (cf.dwEffects & CFE_SUPERSCRIPT));
					CheckMenuItem(pPopup, ID_EDIT_SUBSCRIPT, (cf.dwEffects & CFE_SUBSCRIPT));

					EnableMenuItem(pPopup, ID_EDIT_SUPERSCRIPT, bCanEdit);
					EnableMenuItem(pPopup, ID_EDIT_SUBSCRIPT, bCanEdit);
					EnableMenuItem(pPopup, BUTTON_INSERTTABLE, bCanEdit);
					EnableMenuItem(pPopup, ID_EDIT_HORZRULE, bCanEdit); 
				}

				EnableMenuItem(pPopup, BUTTON_BOLD, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_ITALIC, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_UNDERLINE, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_STRIKETHRU, bCanEdit);

				EnableMenuItem(pPopup, ID_EDIT_COPYFORMATTING, (bHasText && cf.dwEffects));
				EnableMenuItem(pPopup, ID_EDIT_PASTEFORMATTING, (bCanEdit && bHasText && m_cfCopiedFormat.dwEffects));

				EnableMenuItem(pPopup, BUTTON_GROWFONT, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_SHRINKFONT, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_OUTDENT, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_INDENT, bCanEdit);

				EnableMenuItem(pPopup, ID_EDIT_CUT, bCanEdit && bHasSel);
				EnableMenuItem(pPopup, ID_EDIT_COPY, bHasSel);
				EnableMenuItem(pPopup, ID_EDIT_PASTE, bCanEdit && CanPaste());
				EnableMenuItem(pPopup, ID_EDIT_PASTESIMPLE, bCanEdit && CanPaste());
				EnableMenuItem(pPopup, ID_EDIT_DELETE, bCanEdit && bHasSel);
				EnableMenuItem(pPopup, ID_EDIT_SELECT_ALL, bHasText);

				EnableMenuItem(pPopup, ID_EDIT_PASTEASREF, bCanEdit && !IsTDLClipboardEmpty());

				ParaFormat pf(PFM_ALIGNMENT | PFM_NUMBERING);
				m_rtf.GetParaFormat(pf);
				
				CheckMenuItem(pPopup, BUTTON_LEFTALIGN, (pf.wAlignment == PFA_LEFT));
				CheckMenuItem(pPopup, BUTTON_CENTERALIGN, (pf.wAlignment == PFA_CENTER));
				CheckMenuItem(pPopup, BUTTON_RIGHTALIGN, (pf.wAlignment == PFA_RIGHT));
				CheckMenuItem(pPopup, BUTTON_JUSTIFY, (pf.wAlignment == PFA_JUSTIFY));
				CheckMenuItem(pPopup, BUTTON_BULLET, (pf.wNumbering == PFN_BULLET));
				CheckMenuItem(pPopup, BUTTON_NUMBER, (pf.wNumbering > PFN_BULLET));
				
				EnableMenuItem(pPopup, BUTTON_LEFTALIGN, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_CENTERALIGN, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_RIGHTALIGN, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_JUSTIFY, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_BULLET, bCanEdit);
				EnableMenuItem(pPopup, BUTTON_NUMBER, bCanEdit);
				
				int nUrl = m_rtf.GetContextUrl();
				EnableMenuItem(pPopup, ID_EDIT_OPENURL, nUrl != -1);
//				EnableMenuItem(pPopup, ID_EDIT_WEBLINK, bHasText);

				if (nUrl != -1)
				{
					CEnString sMenu;
					pPopup->GetMenuString(ID_EDIT_OPENURL, sMenu, MF_BYCOMMAND);
					sMenu.Translate();
					
					// restrict url length to 250 pixels
					CEnString sUrl(m_rtf.GetUrl(nUrl, TRUE));
					CClientDC dc(this);
					sUrl.FormatDC(&dc, 250, ES_PATH);

					CString sText;
					sText.Format(_T("%s: %s"), sMenu, sUrl);
					pPopup->ModifyMenu(ID_EDIT_OPENURL, MF_BYCOMMAND, ID_EDIT_OPENURL, sText);
				}

				EnableMenuItem(pPopup, ID_EDIT_FILEBROWSE, bCanEdit);
				EnableMenuItem(pPopup, ID_EDIT_INSERTDATESTAMP, bCanEdit);
				EnableMenuItem(pPopup, ID_EDIT_SPELLCHECK, bCanEdit && bHasText);

				EnableMenuItem(pPopup, ID_EDIT_FIND, bHasText);
				EnableMenuItem(pPopup, ID_EDIT_FINDREPLACE, bCanEdit && bHasText);

				CheckMenuItem(pPopup, ID_EDIT_SHOWTOOLBAR, IsToolbarVisible());
				CheckMenuItem(pPopup, ID_EDIT_SHOWRULER, IsRulerVisible());
				CheckMenuItem(pPopup, ID_EDIT_WORDWRAP, HasWordWrap());

				// check pos
				if (point.x == -1 && point.y == -1)
				{
					point = GetCaretPos();
					::ClientToScreen(m_rtf, &point);
				}

				UINT nCmdID = ::TrackPopupMenu(*pPopup, TPM_RETURNCMD | TPM_LEFTALIGN | TPM_LEFTBUTTON, 
												point.x, point.y, 0, GetSafeHwnd(), NULL);

				switch (nCmdID)
				{
				case BUTTON_BOLD:
				case BUTTON_ITALIC:
				case BUTTON_UNDERLINE:
				case BUTTON_STRIKETHRU:
					SendMessage(WM_COMMAND, nCmdID);
					break;

				case ID_EDIT_SUPERSCRIPT:
					DoSuperscript();
					break;

				case ID_EDIT_SUBSCRIPT:
					DoSubscript();
					break;

				case BUTTON_GROWFONT: 
					IncrementFontSize(TRUE);
					break;

				case BUTTON_SHRINKFONT:
					IncrementFontSize(FALSE);
					break;

				case BUTTON_OUTDENT:
					DoOutdent();
					break;

				case BUTTON_INDENT:
					DoIndent();
					break;

				case BUTTON_LEFTALIGN: 
					DoLeftAlign();
					break;

				case BUTTON_CENTERALIGN:
					DoCenterAlign();
					break;

				case BUTTON_RIGHTALIGN:
					DoRightAlign();
					break;

				case BUTTON_JUSTIFY:
					DoJustify();
					break;

				case BUTTON_BULLET:
					DoBullet();
					break;

				case BUTTON_NUMBER:
					DoNumberList();
					break;

				case ID_EDIT_COPY:
					m_rtf.Copy();
					break;

				case ID_EDIT_CUT:
					m_rtf.Cut();
					break;

				case ID_EDIT_COPYFORMATTING:
					m_cfCopiedFormat = cf;
					break;

				case ID_EDIT_PASTEFORMATTING:
					m_rtf.SetSelectionCharFormat(m_cfCopiedFormat);
					break;

				case ID_EDIT_FIND:
					m_rtf.DoEditFind(IDS_FIND_TITLE);
					break;

				case ID_EDIT_HORZRULE:
					DoInsertHorzLine();
					break;

				case BUTTON_INSERTTABLE:
					DoInsertTable();
					break;

				case ID_EDIT_FINDREPLACE:
					m_rtf.DoEditReplace(IDS_REPLACE_TITLE);
					break;

				case ID_EDIT_PASTE:
					Paste();
					break;

				case ID_EDIT_PASTESIMPLE:
					Paste(TRUE); // TRUE ==  simple
					break;

				case ID_EDIT_PASTEASREF:
					{
						// try to get the clipboard for any tasklist
						ITaskList* pClipboard = (ITaskList*)GetParent()->SendMessage(WM_TDCM_GETCLIPBOARD, 0, FALSE);

						// verify that we can get the corresponding filename
						CString sFileName;
						ITaskList4* pClip4 = GetITLInterface<ITaskList4>(pClipboard, IID_TASKLIST4);

						if (pClip4)
						{
							sFileName = pClip4->GetAttribute(TDL_FILENAME);
							sFileName.Replace(_T(" "), _T("%20"));
						}
						else // get the clipboard for just this tasklist
							pClipboard = (ITaskList*)GetParent()->SendMessage(WM_TDCM_GETCLIPBOARD, 0, TRUE);

						if (pClipboard && pClipboard->GetFirstTask())
						{
							// build single string containing each top level item as a different ref
							CString sRefs, sRef;
							HTASKITEM hClip = pClipboard->GetFirstTask();
							
							while (hClip)
							{
								if (sFileName.IsEmpty())
									sRef.Format(_T(" %s%d"), TDL_PROTOCOL, pClipboard->GetTaskID(hClip));
								else
									sRef.Format(_T(" %s%s?%d"), TDL_PROTOCOL, sFileName, pClipboard->GetTaskID(hClip));

								sRefs += sRef;
								
								hClip = pClipboard->GetNextTask(hClip);
							}

							sRefs += ' ';
							m_rtf.ReplaceSel(sRefs, TRUE);
						}
					}
					break;
		
				case ID_EDIT_DELETE:
					m_rtf.ReplaceSel(_T(""), TRUE);
					break;

				case ID_EDIT_SELECT_ALL:
					m_rtf.SetSel(0, -1);
					break;
		
				case ID_EDIT_OPENURL:
					if (nUrl != -1)
						m_rtf.GoToUrl(nUrl);
					break;
					
// 				case ID_EDIT_WEBLINK:
// 					{
// 						// select the word under the caret
// 						m_rtf.SelectCurrentWord();
// 
// 						CEditWebLinkDlg dialog;
// 
// 						if (dialog.DoModal() == IDOK)
// 							SetSelectedWebLink(dialog.GetWebLink());
// 					}
// 					break;
					
				case ID_EDIT_FILEBROWSE:
					{
						CString sFile;
						
						if (nUrl != -1)
						{
							sFile = m_rtf.GetUrl(nUrl, TRUE);

							if (!FileMisc::FileExists(sFile))
								sFile.Empty();
						}
									
						CFileOpenDialog dialog(CEnString(IDS_BROWSE_TITLE), NULL, sFile, EOFN_DEFAULTOPEN);
						
						if (dialog.DoModal() == IDOK)
						{
							CString sFile = dialog.GetPathName();

							if (!m_rtf.IsFileLinkOptionDefault())
							{
								CCreateFileLinkDlg dialog2(sFile, m_rtf.GetFileLinkOption(), FALSE);

								if (dialog2.DoModal() == IDOK)
								{
									RE_PASTE nLinkOption = dialog2.GetLinkOption();
									BOOL bDefault = dialog2.GetMakeLinkOptionDefault();
									
									m_rtf.SetFileLinkOption(nLinkOption, bDefault);
								}
								else
									break; // user cancelled
							}

							CRichEditHelper::PasteFile(m_rtf, sFile, m_rtf.GetFileLinkOption());
						}
					}
					break;

				case ID_PREFERENCES:
					{
						CCreateFileLinkDlg dialog(NULL, m_rtf.GetFileLinkOption(), m_rtf.IsFileLinkOptionDefault(), TRUE);
						
						if (dialog.DoModal() == IDOK)
						{
							RE_PASTE nLinkOption = dialog.GetLinkOption();
							BOOL bDefault = dialog.GetMakeLinkOptionDefault();

							m_rtf.SetFileLinkOption(nLinkOption, bDefault);
						}
					}
					break;
		
				case ID_EDIT_INSERTDATESTAMP:
					{
						COleDateTime date = COleDateTime::GetCurrentTime();
						m_rtf.ReplaceSel(date.Format(), TRUE);
					}
					break;
	
				case ID_EDIT_SPELLCHECK:
					GetParent()->PostMessage(WM_ICC_WANTSPELLCHECK);
					break;

				case ID_EDIT_SHOWTOOLBAR:
					ShowToolbar(!IsToolbarVisible());
					break;

				case ID_EDIT_SHOWRULER:
					ShowRuler(!IsRulerVisible());
					break;

				case ID_EDIT_WORDWRAP:
					SetWordWrap(!HasWordWrap());
					break;
				}
			}
		}
	}
}

void CRTFContentControl::InitMenuIconManager()
{
	if (!m_mgrMenuIcons.Initialize(this))
		return;
	
	m_mgrMenuIcons.ClearImages();
	
	CUIntArray aCmdIDs;
	
	aCmdIDs.Add(BUTTON_FONT);
	aCmdIDs.Add(BUTTON_BOLD);
	aCmdIDs.Add(BUTTON_ITALIC);
	aCmdIDs.Add(BUTTON_UNDERLINE);
	aCmdIDs.Add(BUTTON_STRIKETHRU);
	aCmdIDs.Add(BUTTON_GROWFONT);
	aCmdIDs.Add(BUTTON_SHRINKFONT);
	aCmdIDs.Add(BUTTON_LEFTALIGN);
	aCmdIDs.Add(BUTTON_CENTERALIGN);
	aCmdIDs.Add(BUTTON_RIGHTALIGN);
	aCmdIDs.Add(BUTTON_JUSTIFY);
	aCmdIDs.Add(BUTTON_BULLET);
	aCmdIDs.Add(BUTTON_NUMBER);
	aCmdIDs.Add(BUTTON_OUTDENT);
	aCmdIDs.Add(BUTTON_INDENT);
	aCmdIDs.Add(BUTTON_INSERTTABLE);
	aCmdIDs.Add(BUTTON_TEXTCOLOR);
	aCmdIDs.Add(BUTTON_BACKCOLOR);
	aCmdIDs.Add(BUTTON_WORDWRAP);
		
	m_mgrMenuIcons.AddImages(aCmdIDs, IDB_TOOLBAR, 16, RGB(255, 0, 255));
}

BOOL CRTFContentControl::IsTDLClipboardEmpty() const 
{ 
	// try for any clipboard first
	ITaskList* pClipboard = (ITaskList*)GetParent()->SendMessage(WM_TDCM_GETCLIPBOARD, 0, FALSE);
	ITaskList4* pClip4 = GetITLInterface<ITaskList4>(pClipboard, IID_TASKLIST4);

	if (pClip4)
		return (pClipboard->GetFirstTask() == NULL);

	// else try for 'our' clipboard only
	return (!GetParent()->SendMessage(WM_TDCM_HASCLIPBOARD, 0, TRUE)); 
}

BOOL CRTFContentControl::Paste(BOOL bSimple)
{
	if (Misc::ClipboardHasFormat(CF_HDROP, GetSafeHwnd()))
	{
		if (::OpenClipboard(GetSafeHwnd()))
		{
			HANDLE hData = ::GetClipboardData(CF_HDROP);
			ASSERT(hData);

			CStringArray aFiles;
			int nNumFiles = FileMisc::GetDropFilePaths((HDROP)hData, aFiles);

			::CloseClipboard();

			if (nNumFiles > 0)
			{
				if (bSimple)
				{
					return CRichEditHelper::PasteFiles(m_rtf, aFiles, REP_ASFILEURL);
				}
				else
				{
					if (!m_rtf.IsFileLinkOptionDefault())
					{
						CCreateFileLinkDlg dialog(aFiles[0], m_rtf.GetFileLinkOption(), FALSE);

						if (dialog.DoModal() == IDOK)
						{
							RE_PASTE nLinkOption = dialog.GetLinkOption();
							BOOL bDefault = dialog.GetMakeLinkOptionDefault();

							m_rtf.SetFileLinkOption(nLinkOption, bDefault);
						}
						else
							return FALSE; // cancelled
					}

					return CRichEditHelper::PasteFiles(m_rtf, aFiles, m_rtf.GetFileLinkOption());
				}
			}
			else if (nNumFiles == -1) // error
			{
				AfxMessageBox(IDS_PASTE_ERROR, MB_OK | MB_ICONERROR);
				return FALSE;
			}
		}
	}

	// else
	m_rtf.Paste(bSimple);
	return TRUE;
}

BOOL CRTFContentControl::CanPaste()
{
	static CLIPFORMAT formats[] = 
	{ 
		(CLIPFORMAT)::RegisterClipboardFormat(CF_RTF),
		(CLIPFORMAT)::RegisterClipboardFormat(CF_RETEXTOBJ), 
		(CLIPFORMAT)::RegisterClipboardFormat(_T("Embedded Object")),
		(CLIPFORMAT)::RegisterClipboardFormat(_T("Rich Text Format")),
		CF_BITMAP,

#ifndef _UNICODE
		CF_TEXT,
#else
		CF_UNICODETEXT,
#endif
		CF_DIB,
		CF_HDROP
	};

	// for reasons that I'm not entirely clear on even if we 
	// return that CF_HDROP is okay, the richedit itself will
	// veto the drop. So I'm experimenting with handling this ourselves
	if (Misc::ClipboardHasFormat(CF_HDROP, GetSafeHwnd()))
		return TRUE;

	// else try richedit itself
	const long formats_count = sizeof(formats) / sizeof(CLIPFORMAT);
	BOOL bCanPaste = FALSE;
	
	for( long i=0;  i<formats_count;  ++i )
		bCanPaste |= m_rtf.CanPaste( formats[i] );

	if (!bCanPaste)
		bCanPaste = m_rtf.CanPaste(0);
	
	return bCanPaste;
}

int CRTFContentControl::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	CAutoFlag af(m_bAllowNotify, FALSE);
	
	if (CRulerRichEditCtrl::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	m_rtf.SetEventMask(m_rtf.GetEventMask() | ENM_CHANGE);
	
	// set max edit length
	m_rtf.LimitText(1024 * 1024 * 1024); // one gigabyte
	
	InitMenuIconManager();

	// if MS word is not installed we remove features that
	// our backup RTF2HTML converter cannot handle
	BOOL bRemoveAdvancedFeatures = !CMSWordHelper::IsWordInstalled(12);

	if (bRemoveAdvancedFeatures)
	{
		m_toolbar.GetToolBarCtrl().HideButton(BUTTON_INSERTTABLE);
	}

	// helper for toolbar tooltips
	// initialize after hiding table button
	m_tbHelper.Initialize(&m_toolbar, this);

/*	// helper for richedit tooltips
	if (m_tt.Create(this, TTS_ALWAYSTIP))
	{
		CSubclasser::ScHookWindow(m_ruler);

		m_tt.Activate(TRUE);
		m_tt.AddTool(&m_ruler, LPSTR_TEXTCALLBACK);
	}
*/
	return 0;
}

/*
LRESULT CRTFContentControl::ScWindowProc(HWND hRealWnd, UINT msg, WPARAM wp, LPARAM lp)
{
	switch (msg)
	{
	case WM_MOUSEMOVE:
	case WM_MOUSELEAVE:
		{
			AFX_MANAGE_STATE(AfxGetAppModuleState());

			m_tt.RelayEvent(const_cast<MSG*>(GetCurrentMessage()));
		}
		break;
	}

	return CSubclasser::ScDefault(hRealWnd); 
}
*/

bool CRTFContentControl::ProcessMessage(MSG* pMsg) 
{
	if (!IsWindowEnabled())
		return false;

	// process editing shortcuts
	switch (pMsg->message)
	{
	case WM_KEYDOWN:
		{
			AFX_MANAGE_STATE(AfxGetStaticModuleState());

			BOOL bCtrl = Misc::KeyIsPressed(VK_CONTROL);
			BOOL bShift = Misc::KeyIsPressed(VK_SHIFT);
			BOOL bAlt = Misc::KeyIsPressed(VK_MENU);
			BOOL bEnabled = !GetReadOnly();

			if (bCtrl && !bAlt)
			{

				if (bShift)
				{
					switch (pMsg->wParam)
					{
						// copy formatting
					case 'c': 
					case 'C':
						{
							CharFormat cf(CFM_BOLD | CFM_ITALIC | CFM_UNDERLINE | CFM_STRIKEOUT | 
											CFM_COLOR | CFM_BACKCOLOR | CFM_SUBSCRIPT | CFM_SUPERSCRIPT);

							m_rtf.GetSelectionCharFormat(m_cfCopiedFormat);
						}
						return TRUE;

						// paste formatting
					case 'v':
					case 'V':
						m_rtf.SetSelectionCharFormat(m_cfCopiedFormat);
						return TRUE;

						// paste simple
					case 'p':
					case 'P':
						Paste(TRUE); 
						return TRUE;
					}
				}
				else // simple 'Ctrl+'
				{
					switch (pMsg->wParam)
					{
					case 'c': 
					case 'C':
						m_rtf.Copy();
						return TRUE;

						// paste normal
					case 'v':
					case 'V':
						Paste(FALSE); 
						return TRUE;

					case 'x':
					case 'X':
						m_rtf.Cut();
						return TRUE;

					case 'a':
					case 'A':
						m_rtf.SetSel(0, -1);
						return TRUE;

					case 'b':
					case 'B':
						DoBold();
						return TRUE;

					case 'i':
					case 'I':
						DoItalic();
						return TRUE;

					case 'u':
					case 'U':
						DoUnderline();
						return TRUE;

					case 'l':
					case 'L':
						DoLeftAlign();
						return TRUE;

					case 'e':
					case 'E':
						DoCenterAlign();
						return TRUE;

					case 'r':
					case 'R':
						DoRightAlign();
						return TRUE;
	/*
					case 'j':
					case 'J':
						DoJustify();
						return TRUE;
	*/
					case 0xBD: // '-'
						DoStrikethrough();
						return TRUE;

					case 0xBE: // '>'
						IncrementFontSize(TRUE);
						return TRUE;

					case 0xBC: // '<'
						IncrementFontSize(FALSE);
						return TRUE;

					case 'h':
					case 'H':
						if (bEnabled)
						{
							m_rtf.DoEditReplace();
							return TRUE;
						}
						// else fall thru

					case 'f':
					case 'F':
						m_rtf.DoEditFind();
						return TRUE;

					case 'z':
					case 'Z':
						return TRUE; // to prevent the richedit performing the undo
						
					case 'y':
					case 'Y':
						return TRUE; // to prevent the richedit performing the redo

					case 'j':
					case 'J':
						if (bShift)
							DoOutdent();
						else
							DoIndent();
						return TRUE;
					}
				}
			}
			else if (bEnabled && pMsg->wParam == '\t')
			{
				CHARRANGE cr;
				m_rtf.GetSel(cr);
				
				// if nothing is selected then just insert tabs
				if (cr.cpMin == cr.cpMax)
					m_rtf.ReplaceSel(_T("\t"), TRUE);
				else
				{
					if (!bShift)
						DoIndent();
					else
						DoOutdent();
				}
				
				return TRUE;
			}
		}
		break;
	}

	return false;
}

void CRTFContentControl::SavePreferences(IPreferences* pPrefs, LPCTSTR szKey) const
{
	pPrefs->WriteProfileInt(szKey, _T("ShowToolbar"), IsToolbarVisible());
	pPrefs->WriteProfileInt(szKey, _T("ShowRuler"), IsRulerVisible());
	pPrefs->WriteProfileInt(szKey, _T("WordWrap"), HasWordWrap());

	pPrefs->WriteProfileInt(szKey, _T("FileLinkOption"), m_rtf.GetFileLinkOption());
	pPrefs->WriteProfileInt(szKey, _T("FileLinkOptionIsDefault"), m_rtf.IsFileLinkOptionDefault());
}

void CRTFContentControl::LoadPreferences(const IPreferences* pPrefs, LPCTSTR szKey)
{
	BOOL bShowToolbar = pPrefs->GetProfileInt(szKey, _T("ShowToolbar"), m_showToolbar);
	BOOL bShowRuler = pPrefs->GetProfileInt(szKey, _T("ShowRuler"), m_showRuler);
	BOOL bWordWrap = pPrefs->GetProfileInt(szKey, _T("WordWrap"), TRUE);
	
	ShowToolbar(bShowToolbar);
	ShowRuler(bShowRuler);
	SetWordWrap(bWordWrap);

	RE_PASTE nLinkOption = (RE_PASTE)pPrefs->GetProfileInt(szKey, _T("FileLinkOption"), REP_ASIMAGE);
	BOOL bLinkOptionIsDefault = pPrefs->GetProfileInt(szKey, _T("FileLinkOptionIsDefault"), FALSE);

	m_rtf.SetFileLinkOption(nLinkOption, bLinkOptionIsDefault);
}

void CRTFContentControl::OnStyleChanging(int nStyleType, LPSTYLESTRUCT lpStyleStruct)
{
	if (nStyleType == GWL_EXSTYLE && (lpStyleStruct->styleNew & WS_EX_CLIENTEDGE))
		lpStyleStruct->styleNew &= ~WS_EX_CLIENTEDGE;

	CRulerRichEditCtrl::OnStyleChanging(nStyleType, lpStyleStruct);
}

LRESULT CRTFContentControl::OnCustomUrl(WPARAM wp, LPARAM lp)
{
	UNREFERENCED_PARAMETER(wp);
	ASSERT (wp == RTF_CONTROL);

	CString sUrl((LPCTSTR)lp);
	sUrl.MakeLower();

	if (sUrl.Find(TDL_PROTOCOL) != -1 || sUrl.Find(TDL_EXTENSION) != -1)
		return GetParent()->SendMessage(WM_TDCM_TASKLINK, 0, lp);

	return 0;

}
