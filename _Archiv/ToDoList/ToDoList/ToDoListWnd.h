// ToDoListWnd.h : header file
//

#if !defined(AFX_TODOLISTWND_H__13051D32_D372_4205_BA71_05FAC2159F1C__INCLUDED_)
#define AFX_TODOLISTWND_H__13051D32_D372_4205_BA71_05FAC2159F1C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

//#include "todoctrl.h"
#include "filteredtodoctrl.h"
#include "preferencesdlg.h"
#include "tdlfindtasksDlg.h"
#include "todoctrlmgr.h"
#include "TDLImportExportMgr.h"
#include "TDLContentMgr.h"
#include "TDLfilterbar.h"
#include "TDLSendTasksDlg.h"
#include "taskselectiondlg.h"
#include "todoctrlreminders.h"
#include "tdlTasklistStorageMgr.h"

#include "..\shared\trayicon.h"
#include "..\shared\toolbarhelper.h"
#include "..\shared\filemisc.h"
#include "..\shared\ShortcutManager.h"
#include "..\shared\driveinfo.h"
#include "..\shared\entoolbar.h"
#include "..\shared\tabctrlex.h"
#include "..\shared\enrecentfilelist.h"
#include "..\shared\enmenu.h"
#include "..\shared\dialoghelper.h"
#include "..\shared\tabbedcombobox.h"
#include "..\shared\deferWndMove.h"
#include "..\shared\enBrowserctrl.h"
#include "..\shared\UIExtensionMgr.h"
#include "..\shared\menuiconmgr.h"
#include "..\shared\autocombobox.h"
#include "..\shared\browserdlg.h"
#include "..\shared\uithemefile.h"
#include "..\shared\toolbarhelper.h"
#include "..\shared\StatusbarProgress.h"
#include "..\shared\stickieswnd.h"

#include "..\3rdparty\statusbarACT.h"

/////////////////////////////////////////////////////////////////////////////
// CToDoListWnd dialog

const UINT WM_TDL_SHOWWINDOW = ::RegisterWindowMessage(_T("WM_TDL_SHOWWINDOW"));
const UINT WM_TDL_GETVERSION = ::RegisterWindowMessage(_T("WM_TDL_GETVERSION"));
const UINT WM_TDL_ISCLOSING = ::RegisterWindowMessage(_T("WM_TDL_ISCLOSING"));
const UINT WM_TDL_REFRESHPREFS = ::RegisterWindowMessage(_T("WM_TDL_REFRESHPREFS"));
const UINT WM_TDL_RESTORE = ::RegisterWindowMessage(_T("WM_TDL_RESTORE"));

/////////////////////////////////////////////////////////////////////////////

class CToDoListWnd : public CFrameWnd, public CDialogHelper
{
public:
	// Construction
	CToDoListWnd(); 
	~CToDoListWnd();
	
	static int GetVersion();
	static CString GetTitle();

	BOOL Create(const TDCSTARTUP& startup);
	
protected:
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CToDoListWnd)
	protected:
	virtual BOOL OnCommand(WPARAM wParam, LPARAM lParam);
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	//}}AFX_VIRTUAL
	virtual void OnOK() {}
	virtual void OnCancel();
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	int OnCreate(LPCREATESTRUCT lpCreateStruct);

	// Implementation
protected:
	HICON m_hIcon;
	CImageList m_ilTabCtrl, m_ilCheckboxes;
	CEnToolBar m_toolbar;
	CTrayIcon m_trayIcon;
	CEnRecentFileList m_mruList;
	CTabCtrlEx m_tabCtrl;
	CShortcutManager m_mgrShortcuts;
	CTaskListDropTarget m_dropTarget;
	CPreferencesDlg* m_pPrefs;
	CTDLFindTasksDlg m_findDlg;
	CTDLImportExportMgr m_mgrImportExport;
	CTDLTasklistStorageMgr m_mgrStorage;
	CToDoCtrlMgr m_mgrToDoCtrls;
	CFont m_fontTree, m_fontComments; // shared by all tasklists
	CEnMenu m_menubar;
	CTDLContentMgr m_mgrContent;
	CEnBrowserCtrl m_IE;
	CTDLFilterBar m_filterBar;
	CStatusBarACT	m_statusBar;
	HWND m_hwndLastFocus;
	CMenuIconMgr m_mgrMenuIcons;
	CUIExtensionMgr m_mgrUIExtensions;
	CAutoComboBox m_cbQuickFind;
	CWndPromptManager m_mgrPrompts;
	CBrowserDlg m_dlgNotifyDue;
	CToDoCtrlReminders m_reminders;
	CUIThemeFile m_theme;
	TDC_MAXSTATE m_nMaxState, m_nPrevMaxState;
	TDCSTARTUP m_startupOptions;
	CDWordArray m_aPriorityColors;
	CFont m_fontMain;
	int m_nLastSelItem; // just for flicker-free todoctrl switching
	TODOITEM m_tdiDefault;
	CStatusBarProgress m_sbProgress;
	CToolbarHelper m_tbHelper;
	TDC_COLUMN m_nContextColumnID;

	CString m_sQuickFind;
	CString m_sThemeFile;
	CEnString m_sCurrentFocus;

	BOOL m_bVisible;
	BOOL m_bShowFilterBar, m_bShowProjectName;
	BOOL m_bShowStatusBar, m_bShowToolbar;
	BOOL m_bShowTasklistBar, m_bShowTreeListBar;
	BOOL m_bInNewTask;
	BOOL m_bSaving;
	BOOL m_bInTimer;
	BOOL m_bClosing, m_bEndingSession;
	BOOL m_bFindShowing;
	BOOL m_bQueryOpenAllow;
	BOOL m_bPasswordPrompting;
	BOOL m_bReloading; // on startup
	BOOL m_bStartHidden;
	BOOL m_bLogging;

	// Generated message map functions
	//{{AFX_MSG(CToDoListWnd)
	afx_msg BOOL OnQueryOpen();
	afx_msg void OnAddtimetologfile();
	afx_msg void OnArchiveSelectedTasks();
	afx_msg void OnCloseallbutthis();
	afx_msg void OnCopyTaskasDependency();
	afx_msg void OnCopyTaskasDependencyFull();
	afx_msg void OnCopyTaskasPath();
	afx_msg void OnCopyTaskasLink();
	afx_msg void OnCopyTaskasLinkFull();
	afx_msg void OnEditClearReminder();
	afx_msg void OnEditCleartaskcolor();
	afx_msg void OnEditCleartaskicon();
	afx_msg void OnEditDectaskpercentdone();
	afx_msg void OnEditDectaskpriority();
	afx_msg void OnEditFlagtask();
	afx_msg void OnEditInctaskpercentdone();
	afx_msg void OnEditInctaskpriority();
	afx_msg void OnEditInsertdate();
	afx_msg void OnEditInsertdatetime();
	afx_msg void OnEditInserttime();
	afx_msg void OnEditOffsetdates();
	afx_msg void OnEditRedo();
	afx_msg void OnEditSelectall();
	afx_msg void OnEditSetReminder();
	afx_msg void OnEditSettaskicon();
	afx_msg void OnEditUndo();
	afx_msg void OnEnable(BOOL bEnable);
	afx_msg void OnFileChangePassword();
	afx_msg void OnFileOpenarchive();
	afx_msg void OnGotoNexttask();
	afx_msg void OnGotoPrevtask();
	afx_msg void OnMeasureItem(int nIDCtl, LPMEASUREITEMSTRUCT lpMeasureItemStruct);
	afx_msg void OnNewsubtask();
	afx_msg void OnNewtask();
	afx_msg void OnPrintpreview();
	afx_msg void OnQuickFind();
	afx_msg void OnQuickFindNext();
	afx_msg void OnQuickFindPrev();
	afx_msg void OnSendTasks();
	afx_msg void OnSendSelectedTasks();
	afx_msg void OnShowKeyboardshortcuts();
	afx_msg void OnShowTimelogfile();
	afx_msg void OnSortMulti();
	afx_msg void OnSysColorChange();
	afx_msg void OnTabctrlPreferences();
	afx_msg void OnTasklistSelectColumns();
	afx_msg void OnToolsCheckforupdates();
	afx_msg void OnToolsTransformactivetasklist();
	afx_msg void OnUpdateAddtimetologfile(CCmdUI* pCmdUI);
	afx_msg void OnUpdateArchiveSelectedCompletedTasks(CCmdUI* pCmdUI);
	afx_msg void OnUpdateCloseallbutthis(CCmdUI* pCmdUI);
	afx_msg void OnUpdateCopyTaskasDependency(CCmdUI* pCmdUI);
	afx_msg void OnUpdateCopyTaskasDependencyFull(CCmdUI* pCmdUI);
	afx_msg void OnUpdateCopyTaskasPath(CCmdUI* pCmdUI);
	afx_msg void OnUpdateCopyTaskasLink(CCmdUI* pCmdUI);
	afx_msg void OnUpdateCopyTaskasLinkFull(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditClearReminder(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditCleartaskcolor(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditCleartaskicon(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditDectaskpercentdone(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditDectaskpriority(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditFlagtask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditInctaskpercentdone(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditInctaskpriority(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditInsertdate(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditInsertdatetime(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditInserttime(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditOffsetdates(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditRedo(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditSelectall(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditSetReminder(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditSettaskicon(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditUndo(CCmdUI* pCmdUI);
	afx_msg void OnUpdateFileChangePassword(CCmdUI* pCmdUI);
	afx_msg void OnUpdateFileOpenarchive(CCmdUI* pCmdUI);
	afx_msg void OnUpdateGotoNexttask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateGotoPrevtask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewsubtask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateQuickFind(CCmdUI* pCmdUI);
	afx_msg void OnUpdateQuickFindNext(CCmdUI* pCmdUI);
	afx_msg void OnUpdateQuickFindPrev(CCmdUI* pCmdUI);
	afx_msg void OnUpdateShowTimelogfile(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSendTasks(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSendSelectedTasks(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSortMulti(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewClearfilter(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewCollapseDuetasks(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewCollapseStartedtasks(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewCollapseall(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewCollapsetask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewExpandDuetasks(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewExpandStartedtasks(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewExpandall(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewExpandtask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewFilter(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewProjectname(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewShowTasklistTabbar(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewShowTreeListTabbar(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewShowfilterbar(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewSorttasklisttabs(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewStatusBar(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewCycleTaskViews(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewToggleTreeandList(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewTogglefilter(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewToggletaskexpanded(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewToggletasksandcomments(CCmdUI* pCmdUI);
	afx_msg void OnUpdateWindow(CCmdUI* pCmdUI);
	afx_msg void OnViewClearfilter();
	afx_msg void OnViewCollapseDuetasks();
	afx_msg void OnViewCollapseStartedtasks();
	afx_msg void OnViewCollapseall();
	afx_msg void OnViewCollapsetask();
	afx_msg void OnViewExpandDuetasks();
	afx_msg void OnViewExpandStartedtasks();
	afx_msg void OnViewExpandall();
	afx_msg void OnViewExpandtask();
	afx_msg void OnViewFilter();
	afx_msg void OnViewProjectname();
	afx_msg void OnViewShowTasklistTabbar();
	afx_msg void OnViewShowTreeListTabbar();
	afx_msg void OnViewShowfilterbar();
	afx_msg void OnViewSorttasklisttabs();
	afx_msg void OnViewStatusBar();
	afx_msg void OnViewCycleTaskViews();
	afx_msg void OnViewToggleTreeandList();
	afx_msg void OnViewTogglefilter();
	afx_msg void OnViewToggletaskexpanded();
	afx_msg void OnViewToggletasksandcomments();
	afx_msg void OnWindowPosChanged(WINDOWPOS FAR* lpwndpos);
	afx_msg void OnWindowPosChanging(WINDOWPOS FAR* lpwndpos);
	afx_msg void OnTasklistCustomColumns();
	afx_msg void OnEditGotoDependency();
	afx_msg void OnUpdateEditGotoDependency(CCmdUI* pCmdUI);
	afx_msg void OnEditRecurrence();
	afx_msg void OnUpdateEditRecurrence(CCmdUI* pCmdUI);
	afx_msg BOOL OnQueryEndSession();
	afx_msg void OnEditClearAttribute();
	afx_msg void OnUpdateEditClearAttribute(CCmdUI* pCmdUI);
	afx_msg void OnEditClearFocusedAttribute();
	afx_msg void OnUpdateEditClearFocusedAttribute(CCmdUI* pCmdUI);
	afx_msg void OnUpdateTasklistCustomcolumns(CCmdUI* pCmdUI);
	//}}AFX_MSG
#if _MSC_VER >= 1400
	afx_msg void OnActivateApp(BOOL bActive, DWORD dwThreadID);
#else
	afx_msg void OnActivateApp(BOOL bActive, HTASK hTask);
#endif
	afx_msg BOOL OnCopyData(CWnd* pWnd, COPYDATASTRUCT* pCopyDataStruct);
	afx_msg BOOL OnEraseBkgnd(CDC* pDC);
	afx_msg BOOL OnHelpInfo(HELPINFO* pHelpInfo);
	afx_msg BOOL OnOpenRecentFile(UINT nID);
	afx_msg void OnEndSession(BOOL bEnding);
	afx_msg LRESULT OnAddToolbarTools(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnAppRestoreFocus(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnDoubleClkReminderCol(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnDropFile(WPARAM wParam, LPARAM lParam);
	afx_msg void OnFileOpenFromUserStorage(UINT nCmdID);
	afx_msg void OnFileSaveToUserStorage(UINT nCmdID);
	afx_msg LRESULT OnFindApplyAsFilter(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnFindAddSearch(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnFindDeleteSearch(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnFindDlgClose(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnFindDlgFind(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnFindSelectAll(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnFindSelectResult(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnFocusChange(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnGetFont(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnGetIcon(WPARAM bLargeIcon, LPARAM /*not used*/);
	afx_msg LRESULT OnHotkey(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnPostOnCreate(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnPowerBroadcast(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnPreferencesClearMRU(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnPreferencesCleanupDictionary(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnPreferencesDefaultListChange(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnPreferencesTestTool(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnQuickFindItemAdded(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnSelchangeFilter(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnToDoCtrlDoLengthyOperation(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnToDoCtrlDoTaskLink(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnToDoCtrlNotifyListChange(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnToDoCtrlNotifyMinWidthChange(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnToDoCtrlNotifyMod(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnToDoCtrlNotifyRecreateRecurringTask(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnToDoCtrlNotifyTimeTrack(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnToDoCtrlNotifyViewChange(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnToDoCtrlReminder(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnToDoCtrlTaskHasReminder(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnToDoCtrlTaskIsDone(WPARAM wParam, LPARAM lParam);
	afx_msg LRESULT OnToDoListGetVersion(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnToDoListIsClosing(WPARAM /*wp*/, LPARAM /*lp*/) { return m_bClosing; }
	afx_msg LRESULT OnToDoListRefreshPrefs(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnToDoListRestore(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnToDoListShowWindow(WPARAM wp, LPARAM lp);
	afx_msg LRESULT OnWebUpdateWizard(WPARAM wp, LPARAM lp);
	afx_msg void OnAbout();
	afx_msg void OnArchiveCompletedtasks();
	afx_msg void OnClose();
	afx_msg void OnCloseTasklist();
	afx_msg void OnCloseall();
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	afx_msg void OnDeleteAllTasks();
	afx_msg void OnDeleteTask();
	afx_msg void OnDrawItem(int nIDCtl, LPDRAWITEMSTRUCT lpDrawItemStruct);
	afx_msg void OnEditChangeQuickFind();
	afx_msg void OnEditCopy();
	afx_msg void OnEditCopyashtml();
	afx_msg void OnEditCopyastext();
	afx_msg void OnEditCut();
	afx_msg void OnEditOpenfileref();
	afx_msg void OnEditPasteAfter();
	afx_msg void OnEditPasteSub();
	afx_msg void OnEditSetfileref();
	afx_msg void OnEditTaskcolor();
	afx_msg void OnEditTaskdone();
	afx_msg void OnEditTasktext();
	afx_msg void OnEditTimeTrackTask();
	afx_msg void OnExit();
	afx_msg void OnExport();
	afx_msg void OnFileEncrypt();
	afx_msg void OnFileResetversion();
	afx_msg void OnFindTasks();
	afx_msg void OnImportTasklist();
	afx_msg void OnInitMenuPopup(CMenu* pPopupMenu, UINT nIndex, BOOL bSysMenu);
	afx_msg void OnLoad();
	afx_msg void OnMBtnClickTabcontrol(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnMaximizeComments();
	afx_msg void OnMaximizeTasklist();
	afx_msg void OnMinimizeToTray();
	afx_msg void OnMove(int x, int y);
	afx_msg void OnMovetaskdown();
	afx_msg void OnMovetaskleft();
	afx_msg void OnMovetaskright();
	afx_msg void OnMovetaskup();
	afx_msg void OnNeedTooltipText(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnNew();
	afx_msg void OnNewsubtaskAtbottom();
	afx_msg void OnNewsubtaskAttop();
	afx_msg void OnNewtaskAfterselectedtask();
	afx_msg void OnNewtaskAtbottom();
	afx_msg void OnNewtaskAtbottomSelected();
	afx_msg void OnNewtaskAttop();
	afx_msg void OnNewtaskAttopSelected();
	afx_msg void OnNewtaskBeforeselectedtask();
	afx_msg void OnNexttopleveltask();
	afx_msg void OnPreferences();
	afx_msg void OnPrevtopleveltask();
	afx_msg void OnPrint();
	afx_msg void OnReload();
	afx_msg void OnSave();
	afx_msg void OnSaveall();
	afx_msg void OnSaveas();
	afx_msg void OnSelChangeQuickFind();
	afx_msg void OnSelchangeTabcontrol(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnSelchangingTabcontrol(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnSetPriority(UINT nCmdUI);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnSort();
	afx_msg void OnSortBy(UINT nCmdID);
	afx_msg void OnSpellcheckTasklist();
	afx_msg void OnSpellcheckcomments();
	afx_msg void OnSpellchecktitle();
	afx_msg void OnSplitTaskIntoPieces(UINT nCmdID);
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnToolsCheckin();
	afx_msg void OnToolsCheckout();
	afx_msg void OnToolsShowtasksDue(UINT nCmdID);
 	afx_msg void OnToolsRemovefromsourcecontrol();
	afx_msg void OnToolsToggleCheckin();
	afx_msg void OnTrayIconClick(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTrayIconDblClk(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTrayIconRClick(NMHDR* pNMHDR, LRESULT* pResult);
	afx_msg void OnTrayiconClose();
	afx_msg void OnTrayiconCreatetask();
	afx_msg void OnTrayiconShow();
	afx_msg void OnTrayiconShowDueTasks(UINT nCmdID);
	afx_msg void OnUpdateArchiveCompletedtasks(CCmdUI* pCmdUI);
	afx_msg void OnUpdateCloseall(CCmdUI* pCmdUI);
	afx_msg void OnUpdateDeletealltasks(CCmdUI* pCmdUI);
	afx_msg void OnUpdateDeletetask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditCopy(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditCut(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditOpenfileref(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditPasteAfter(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditPasteSub(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditSetfileref(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditTasktext(CCmdUI* pCmdUI);
	afx_msg void OnUpdateEditTimeTrackTask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateExport(CCmdUI* pCmdUI);
	afx_msg void OnUpdateFileEncrypt(CCmdUI* pCmdUI);
	afx_msg void OnUpdateFileResetversion(CCmdUI* pCmdUI);
	afx_msg void OnUpdateImport(CCmdUI* pCmdUI);
	afx_msg void OnUpdateMaximizeComments(CCmdUI* pCmdUI);
	afx_msg void OnUpdateMaximizeTasklist(CCmdUI* pCmdUI);
	afx_msg void OnUpdateMovetaskdown(CCmdUI* pCmdUI);
	afx_msg void OnUpdateMovetaskleft(CCmdUI* pCmdUI);
	afx_msg void OnUpdateMovetaskright(CCmdUI* pCmdUI);
	afx_msg void OnUpdateMovetaskup(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNew(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewsubtaskAtBottom(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewsubtaskAttop(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewtask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewtaskAfterselectedtask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewtaskAtbottom(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewtaskAtbottomSelected(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewtaskAttop(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewtaskAttopSelected(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNewtaskBeforeselectedtask(CCmdUI* pCmdUI);
	afx_msg void OnUpdateNexttopleveltask(CCmdUI* pCmdUI);
	afx_msg void OnUpdatePrevtopleveltask(CCmdUI* pCmdUI);
	afx_msg void OnUpdatePrint(CCmdUI* pCmdUI);
	afx_msg void OnUpdateRecentFileMenu(CCmdUI* pCmdUI);
	afx_msg void OnUpdateReload(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSBSelectionCount(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSBTaskCount(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSave(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSaveToWeb(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSaveall(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSaveas(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSetPriority(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSort(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSortBy(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSpellcheckTasklist(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSpellcheckcomments(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSpellchecktitle(CCmdUI* pCmdUI);
	afx_msg void OnUpdateSplitTaskIntoPieces(CCmdUI* pCmdUI);
	afx_msg void OnUpdateTaskcolor(CCmdUI* pCmdUI);
	afx_msg void OnUpdateTaskdone(CCmdUI* pCmdUI);
	afx_msg void OnUpdateToolsCheckin(CCmdUI* pCmdUI);
	afx_msg void OnUpdateToolsCheckout(CCmdUI* pCmdUI);
 	afx_msg void OnUpdateToolsRemovefromsourcecontrol(CCmdUI* pCmdUI);
	afx_msg void OnUpdateToolsToggleCheckin(CCmdUI* pCmdUI);
	afx_msg void OnUpdateUserTool(CCmdUI* pCmdUI);
	afx_msg void OnUpdateUserUIExtension(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewMovetasklistleft(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewMovetasklistright(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewNext(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewNextSel(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewPrev(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewPrevSel(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewRefreshfilter(CCmdUI* pCmdUI);
	afx_msg void OnUpdateViewToolbar(CCmdUI* pCmdUI);
	afx_msg void OnUserTool(UINT nCmdID);
	afx_msg void OnUserUIExtension(UINT nCmdID);
	afx_msg void OnViewMovetasklistleft();
	afx_msg void OnViewMovetasklistright();
	afx_msg void OnViewNext();
	afx_msg void OnViewNextSel();
	afx_msg void OnViewPrev();
	afx_msg void OnViewPrevSel();
	afx_msg void OnViewRefreshfilter();
	afx_msg void OnViewToolbar();
	afx_msg void OnWindow(UINT nCmdID);
#ifdef _DEBUG
	afx_msg void OnDebugEndSession();
	afx_msg void OnDebugShowSetupDlg();
#endif
	DECLARE_MESSAGE_MAP()
		
	// OnTimer helpers
	void OnTimerReadOnlyStatus(int nCtrl = -1);
	void OnTimerTimestampChange(int nCtrl = -1);
	void OnTimerAutoSave();
	void OnTimerCheckoutStatus(int nCtrl = -1);
	void OnTimerDueItems(int nCtrl = -1);
	void OnTimerTimeTracking();
	void OnTimerAutoMinimize();
	
	virtual void LoadSettings();
	virtual void SaveSettings();
	void RestorePosition();
	void RestoreVisibility();
	
	BOOL Export2Html(const CTaskFile& tasks, LPCTSTR szFilePath, LPCTSTR szStylesheet = NULL) const;
	BOOL GetAutoExportExtension(CString& sExt) const;

	TDC_FILE DelayOpenTaskList(LPCTSTR szFilePath); // 0 = failed, 1 = success, -1 = cancelled
	TDC_FILE OpenTaskList(LPCTSTR szFilePath, BOOL bNotifyDueTasks = TRUE); // 0 = failed, 1 = success, -1 = cancelled
	TDC_FILE OpenTaskList(CFilteredToDoCtrl* pCtrl, LPCTSTR szFilePath = NULL, TSM_TASKLISTINFO* pInfo = NULL);

	TDC_PREPAREPATH PrepareFilePath(CString& sFilePath, TSM_TASKLISTINFO* pInfo = NULL);

	void ReloadTaskList(int nIndex, BOOL bNotifyDueTasks);
	BOOL VerifyTaskListOpen(int nIndex, BOOL bWantNotifyDueTasks);
	BOOL ImportFile(LPCTSTR szFilePath, BOOL bSilent);

	void InitShortcutManager();
	void InitMenuIconManager();
	void InitUIFont();
	BOOL LoadMenubar();
	BOOL InitCheckboxImageList();
	BOOL InitToolbar();
	BOOL InitStatusbar();
	BOOL InitFilterbar();
	BOOL InitFindDialog(BOOL bShow = FALSE);

	BOOL CreateNewTask(LPCTSTR szTitle, TDC_INSERTWHERE nInsertWhere, BOOL bEdit = TRUE);
	BOOL CanCreateNewTask(TDC_INSERTWHERE nInsertWhere) const;

	BOOL ProcessStartupOptions(const TDCSTARTUP& startup/*, BOOL bPreviousInstance*/);
	TDC_COLUMN GetSortBy(UINT nSortID);
	UINT GetSortID(TDC_COLUMN nSortBy);
	void CheckMinWidth();
	void MinimizeToTray();
	void Show(BOOL bAllowToggle);
	void RefreshPauseTimeTracking();
	BOOL IsActivelyTimeTracking() const;
	void SetTimer(UINT nTimerID, BOOL bOn);
	void RefreshTabOrder();
	void UpdateGlobalHotkey();
	LPCTSTR GetFileFilter(BOOL bOpen);
	LPCTSTR GetDefaultFileExt();
	void CheckForUpdates(BOOL bManual);
	void UpdateCwd();
	BOOL WantTasklistTabbarVisible() const;
	void UpdateAeroFeatures();
	void UpdateDefaultTaskAttributes(const CPreferencesDlg& prefs);
	void CopySelectedTasksToClipboard(TDC_TASKS2CLIPBOARD nAsFormat);
	void CheckNotifyReadonly(int nIndex) const;
	void SetUITheme(const CString& sThemeFile);

	void PrepareFilter(FTDCFILTER& filter) const;
	void RefreshFilterControls();
	void RefreshFilterBarCustomFilters();

	void Resize(int cx = 0, int cy = 0, BOOL bMaximized = FALSE);
	BOOL CalcToDoCtrlRect(CRect& rect, int cx = 0, int cy = 0, BOOL bMaximized = FALSE);
	int ReposTabBar(CDeferWndMove& dwm, const CPoint& ptOrg, int nWidth, BOOL bCalcOnly = FALSE);

	void PrepareEditMenu(CMenu* pMenu);
	void PrepareSortMenu(CMenu* pMenu);
	void PrepareSourceControlMenu(CMenu* pMenu);
	void AddUserStorageToMenu(CMenu* pMenu);

	void ShowFindDialog(BOOL bShow = TRUE);
	void AddFindResult(const SEARCHRESULT& result, const CFilteredToDoCtrl* pTDC);
	void UpdateFindDialogCustomAttributes(const CFilteredToDoCtrl* pCtrl = NULL);
	
	void PrepareToolbar(int nOption);
	void SetToolbarOption(int nOption);
	void AppendTools2Toolbar(BOOL bAppend = TRUE);
	void PopulateToolArgs(USERTOOLARGS& args) const;

	CFilteredToDoCtrl& GetToDoCtrl();
	CFilteredToDoCtrl& GetToDoCtrl(int nIndex);
	const CFilteredToDoCtrl& GetToDoCtrl() const;
	const CFilteredToDoCtrl& GetToDoCtrl(int nIndex) const;
	CFilteredToDoCtrl* NewToDoCtrl(BOOL bVisible = TRUE, BOOL bEnabled = TRUE);
	BOOL CreateToDoCtrl(CFilteredToDoCtrl* pCtrl, BOOL bVisible = TRUE, BOOL bEnabled = TRUE);
	int AddToDoCtrl(CFilteredToDoCtrl* pCtrl, TSM_TASKLISTINFO* pInfo = NULL, BOOL bResizeDlg = TRUE);
	inline int GetTDCCount() const { return m_mgrToDoCtrls.GetCount(); }
	BOOL SelectToDoCtrl(LPCTSTR szFilePath, BOOL bCheckPassword, int nNotifyDueTasksBy = -1);
	BOOL SelectToDoCtrl(int nIndex, BOOL bCheckPassword, int nNotifyDueTasksBy = -1);
	int GetSelToDoCtrl() const;
	BOOL CreateNewTaskList(BOOL bAddDefTask);
	BOOL VerifyToDoCtrlPassword() const;
	BOOL VerifyToDoCtrlPassword(int nIndex) const;
	
	// caller must flush todoctrls if required before calling these
	BOOL CloseToDoCtrl(int nIndex);
	TDC_FILE ConfirmSaveTaskList(int nIndex, DWORD dwFlags = 0);
	TDC_FILE SaveTaskList(int nIndex, LPCTSTR szFilePath = NULL, BOOL bAuto = FALSE); // returns FALSE only if the user was prompted to save and cancelled
	TDC_FILE SaveAll(DWORD dwFlags); // returns FALSE only if the user was prompted to save and cancelled
	
	void UpdateTooltip();
	void UpdateCaption();
	void UpdateStatusbar();
	void UpdateTabSwitchTooltip();
	void UpdateSBPaneAndTooltip(UINT nIDPane, UINT nIDTextFormat, const CString& sValue, UINT nIDTooltip, TDC_COLUMN nTDCC);
	void UpdateStatusBarInfo(const CFilteredToDoCtrl& tdc, TDCSTATUSBARINFO& sbi) const;

	void UpdateToDoCtrlPreferences(CFilteredToDoCtrl* pCtrl);
	void UpdateToDoCtrlPreferences();
	const CPreferencesDlg& Prefs() const;
	void ResetPrefs();

	UINT MapNewTaskPos(int nPos, BOOL bSubtask);
	UINT GetNewTaskCmdID();
	UINT GetNewSubtaskCmdID();

	// helpers
	int GetTasks(CFilteredToDoCtrl& tdc, BOOL bHtmlComments, BOOL bTransform, 
					const CTaskSelectionDlg& taskSel, CTaskFile& tasks, LPCTSTR szHtmlImageDir) const;
	int GetTasks(CFilteredToDoCtrl& tdc, BOOL bHtmlComments, BOOL bTransform, 
					TSD_TASKS nWhatTasks, TDCGETTASKS& filter, DWORD dwSelFlags, CTaskFile& tasks, LPCTSTR szHtmlImageDir) const;
	
	void DoSendTasks(BOOL bSelected);
	void DoPreferences(int nInitPage = -1);
	void DoPrint(BOOL bPreview = FALSE);
	BOOL DoBackup(int nIndex);
	BOOL DoDueTaskNotification(const CFilteredToDoCtrl* pCtrl, int nDueBy);
	BOOL DoDueTaskNotification(int nDueBy); // works on active tasklist
	
	BOOL DoExit(BOOL bRestart = FALSE, BOOL bClosingWindows = FALSE);
	BOOL DoQueryEndSession(BOOL bQuery, BOOL bEnding);

	TDC_ARCHIVE GetAutoArchiveOptions(LPCTSTR szFilePath, CString& sArchivePath, BOOL& bRemoveFlagged) const;

	static void EnableTDLProtocol(BOOL bEnable);
	static void SetupUIStrings();
	static void EnableDynamicMenuTranslation(BOOL bEnable);
	static void HandleLoadTasklistError(TDC_FILE nErr, LPCTSTR szTasklist);
	static CString GetEndSessionFilePath();
	static BOOL IsEndSessionFilePath(const CString& sFilePath);
	static BOOL LogIntermediateTaskList(CTaskFile& tasks, LPCTSTR szRefPath);
	
	void TranslateUIElements();
	BOOL UpdateLanguageTranslationAndRestart(const CString& sLangFile, BOOL bAdd2Dict, const CPreferencesDlg& curPrefs);

	static int MessageBox(UINT nIDText, UINT nIDCaption = 0, UINT nType = MB_OK, LPCTSTR szData = NULL);
	static int MessageBox(const CString& sText, UINT nIDCaption = 0, UINT nType = MB_OK);
	static int MessageBox(const CString& sText, const CString& sCaption, UINT nType = MB_OK);
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TODOLISTWND_H__13051D32_D372_4205_BA71_05FAC2159F1C__INCLUDED_)
