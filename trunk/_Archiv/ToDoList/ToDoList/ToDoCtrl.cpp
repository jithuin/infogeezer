// ToDoCtrl.cpp : implementation file
//

#include "stdafx.h"
#include "ToDoCtrl.h"
#include "tdlschemadef.h"
#include "taskfile.h"
#include "taskclipboard.h"
#include "tdcmsg.h"
#include "todocommentsctrl.h"
#include "tdstringres.h"
#include "resource.h"
#include "tasktimelog.h"
#include "todoitem.h"
#include "tdcstatic.h"
#include "tdcenum.h"
#include "tdltaskicondlg.h"
#include "tdlreuserecurringtaskdlg.h"
#include "tdlimportoutlookobjectsdlg.h"
#include "tdccustomattributehelper.h"

#include "..\shared\holdredraw.h"
#include "..\shared\osversion.h"
#include "..\shared\deferWndMove.h"
#include "..\shared\dlgunits.h"
#include "..\shared\themed.h"
#include "..\shared\datehelper.h"
#include "..\shared\driveinfo.h"
#include "..\shared\toolbarhelper.h"
#include "..\shared\colordef.h"
#include "..\shared\spellcheckdlg.h"
#include "..\shared\passworddialog.h"
#include "..\shared\winclasses.h"
#include "..\shared\wclassdefines.h"
#include "..\shared\enfiledialog.h"
#include "..\shared\misc.h"
#include "..\shared\graphicsmisc.h"
#include "..\shared\enstring.h"
#include "..\shared\stringres.h"
#include "..\shared\treectrlhelper.h"
#include "..\shared\filemisc.h"
#include "..\shared\preferences.h"
#include "..\shared\autoflag.h"
#include "..\shared\enbitmap.h"
#include "..\shared\enmenu.h"
#include "..\shared\outlookhelper.h"
#include "..\shared\savefocus.h"
#include "..\shared\localizer.h"

#include "..\3rdparty\msoutl.h"
#include "..\3rdparty\shellicons.h"

#include <Windowsx.h>
#include <float.h>
#include <Lmcons.h>
#include <math.h>
#include <afxpriv.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


/////////////////////////////////////////////////////////////////////////////
// CToDoCtrl dialog

const int CTRLHEIGHT = 13; // dlu
const int CTRLVSPACING = 5; // dlu
const int CTRLHSPACING = 4; // dlu
const int CTRLLABELLEN = 45; // dlu
const int CTRLCTRLLEN = 65; // dlu
const unsigned short SPLITSIZE = 6; 
const unsigned short MINCOMMENTSIZE = 0;
const unsigned short DEFCOMMENTSIZE = 260;
const unsigned short MINNONCOMMENTHEIGHT = 250; // what's above the comment section
const unsigned short MINNONCOMMENTWIDTH = 350; // what's to the left of the comment section
const COLORREF BLACK = RGB(0, 0, 0);
const COLORREF WHITE = RGB(240, 240, 240);
const TCHAR CLOCKCHAR[] = { 0xb9, 0 };
const TCHAR LINKCHAR[] = { 0x24, 0 };
const TCHAR FLAGCHAR[] = { 0x4f, 0 };
const TCHAR BELLCHAR[] = { 0x25, 0 };
const COLORREF COMMENTSCOLOR = RGB(98, 98, 98);
const UINT DAY_IN_SECS = 24 * 60 * 60;
LPCTSTR DEFAULT_UNICODE_HEADER = _T("version=\"1.0\" encoding=\"UTF-16\"");

//////////////////////////////////////////////////////////////////////////////

enum 
{
	ID_TIME_TRACK = 0xfff0,
	ID_EXTERNALID_LINK,
	ID_DEPENDS_LINK,
};

// file formats changes
// as changes are made, insert a new value *BEFORE* FILEFORMAT_CURRENT
// which will effectively increment FILEFORMAT_CURRENT and be useful
// for conditional code
enum 
{
	FILEFORMAT_SORTBYID = 8,
	FILEFORMAT_CURRENT // increment this when format changes
};

enum // flags for UpdateTask
{
	UTF_TIMEUNITSONLY	= 0x01,
	UTF_RECALCTIME		= 0x02,		
};

enum 
{ 
	TIMER_TRACK = 1, 
	TIMER_MIDNIGHT 
};

// what got changed as a result of AdjustTaskDates
enum 
{
	ADJUSTED_NONE	= 0x00,
	ADJUSTED_START	= 0x01,
	ADJUSTED_DUE	= 0x02,
	ADJUSTED_DONE	= 0x04,
};

//////////////////////////////////////////////////////////////////////////////

const int TIMETRACKPERIOD = 10000; // 10 secs in millesecs
const int MIDNIGHTPERIOD = 60000; // 1 minute in millesecs

#ifdef _DEBUG
const double TICKS2HOURS = 10.0 / (1000 * 3600);
#else
const double TICKS2HOURS = 1.0 / (1000 * 3600);
#endif

//////////////////////////////////////////////////////////////////////////////


// private class to help optimize xml parsing
class CXmlParseController : public IXmlParse
{
public:
	CXmlParseController(LPCTSTR szItem) : m_sItem(szItem) {}
	
	virtual BOOL Continue(LPCTSTR szItem, LPCTSTR /*szValue*/) const
	{
		return (m_sItem.CompareNoCase(szItem) != 0);
	}
	
protected:
	CString m_sItem;
};

//////////////////////////////////////////////////////////////////////////////

#define HOLD_REDRAW(tdc, tree) 	CHoldRedraw hr(tdc); CHoldRedraw hr2(tree);
#define HOLD_REDRAW_NOPAINT(tdc, tree) 	CHoldRedraw hr(tdc, 0); CHoldRedraw hr2(tree, 0);
#define HOLD_REDRAW_UPDATE(tdc, tree) 	CHoldRedraw hr(tdc, NCR_NCPAINT | NCR_PAINT | NCR_UPDATE); \
										CHoldRedraw hr2(tree, NCR_NCPAINT | NCR_PAINT | NCR_UPDATE);

#define IMPLEMENT_UNDO(op) CUndoAction ua(m_data, op)
#define IMPLEMENT_UNDOEDIT() CUndoAction ua(m_data, TDCUAT_EDIT)

//////////////////////////////////////////////////////////////////////////////

// local method to avoid including StdioFileEx in header
SFE_SAVEAS GetSaveAs(BOOL bUnicode)
{
	// set save format to match load format
	if (bUnicode == FALSE)
		return SFE_MULTIBYTE;

	else if (bUnicode != -1)
		return SFE_UNICODE;

	// else
	return SFE_ASCOMPILED;
}

//////////////////////////////////////////////////////////////////////////////

// static variables
int CToDoCtrl::s_nCommentsSize = -1; // unset
TODOITEM CToDoCtrl::s_tdDefault;
short CToDoCtrl::s_nExtendedSelection = HOTKEYF_CONTROL | HOTKEYF_SHIFT;
CTDCAttributeArray CToDoCtrl::s_aParentAttribs;
BOOL CToDoCtrl::s_bUpdateInheritAttrib = FALSE; 
 
//////////////////////////////////////////////////////////////////////////////

CToDoCtrl::CToDoCtrl(CContentMgr& mgr, const CONTENTFORMAT& cfDefault) : 
						m_bModified(FALSE), 
						m_dwNextUniqueID(1), 
						m_nPriority(-1), 
						m_nFileVersion(0),
						m_bArchive(FALSE),
						m_dTimeEstimate(0),
						m_dTimeSpent(0),
						m_eFileRef(FES_COMBOSTYLEBTN | FES_GOBUTTON | FES_ALLOWURL | FES_RELATIVEPATHS),
						m_nFileFormat(FILEFORMAT_CURRENT),
						m_bModSinceLastSort(FALSE),
						m_crDone(NOCOLOR),
						m_bCheckedOut(FALSE),
						m_nCommentsSize(DEFCOMMENTSIZE),
						m_bSplitting(FALSE),
						m_data(m_aStyles),
						m_selection(m_tree),
						m_wKeyPress(FALSE),
						m_dwTimeTrackTaskID(0),
						m_treeDragDrop(Selection(), m_tree, DD_USESTEXTCALLBACK/* | DD_USESIMAGECALLBACK*/),
						m_dwLastAddedID(0),
						m_cbAllocBy(ACBS_ALLOWDELETE),
						m_cbAllocTo(ACBS_ALLOWDELETE),
						m_cbCategory(ACBS_ALLOWDELETE),
						m_cbTags(ACBS_ALLOWDELETE),
						m_cbStatus(ACBS_ALLOWDELETE),
						m_cbVersion(ACBS_ALLOWDELETE),
						m_hFontTree(NULL),
						m_hFontDone(NULL),
						m_hFontComments(NULL),
						m_nMaxInfotipCommentsLength(-1),
						m_ctrlComments(NULL),
						m_bSourceControlled(FALSE),
						m_mgrContent(mgr),
						m_cfDefault(cfDefault),
						m_hilDone(NULL),
						m_dLogTime(0),
						m_hFontBold(NULL),
						m_dwEditingID(0),
						m_crStarted(NOCOLOR), 
						m_crStartedToday(NOCOLOR),
						m_crDue(NOCOLOR), 
						m_crDueToday(NOCOLOR),
						m_cbCommentsType(&mgr),
						m_bDragDropSubtasksAtTop(TRUE),
						m_timeDue(TCB_NOTIME),
						m_timeStart(TCB_NOTIME),
						m_timeDone(TCB_NOTIME),
						m_bDelayLoaded(FALSE),
						m_bCommentsChange(FALSE),
						m_eTaskName(PEC_AUTODESTROY),
						m_find(m_tree, m_data),
						m_nMaxColWidth(-1),
						m_crFlagged(NOCOLOR),
						m_bMultiSort(FALSE),
						m_bFirstLoadCommentsPrefs(TRUE),
						m_bIsUnicode(-1), // == SFE_ASCOMPILED
						m_bDeletingAll(FALSE),
						m_nMaxState(TDCMS_NORMAL)
{
	SetBordersDLU(0);
	
	// add controls
	for (int nCtrl = 0; nCtrl < NUM_TDCCTRLS; nCtrl++)
	{
		const TDCCONTROL& ctrl = TDCCONTROLS[nCtrl];

		AddRCControl(_T("CONTROL"), ctrl.szClass, CString((LPCTSTR)ctrl.nIDCaption), 
					ctrl.dwStyle, ctrl.dwExStyle,
					ctrl.nX, ctrl.nY, ctrl.nCx, ctrl.nCy, ctrl.nID);
	}
	
    // init styles array
    // MFC sets all elements to zero
    m_aStyles.SetSize(TDCS_LAST);
	
	// set up number masks
	m_ePercentDone.SetMask(_T("0123456789"));
	m_eCost.SetMask(_T("-.0123456789"), ME_LOCALIZEDECIMAL);
	
	// add chess clock button to 'time spent'
	m_eTimeSpent.InsertButton(0, ID_TIME_TRACK, CLOCKCHAR, 
								CEnString(IDS_TDC_STARTSTOPCLOCK), 
								DEF_BTNWIDTH + 4, _T("Wingdings"));

	// add link button to dependency
	m_eDependency.AddButton(ID_DEPENDS_LINK, LINKCHAR, 
							CEnString(IDS_TDC_DEPENDSLINK_TIP),
							CALC_BTNWIDTH, _T("Wingdings"));

	// set default task creation date to zero so that new tasks
	// always get the current date
	s_tdDefault.dateCreated.m_dt = 0.0;
	s_tdDefault.sCommentsTypeID = cfDefault;

	m_cpColour.SetSelectionMode(CP_MODE_TEXT);

	// turn off pos column because we override it
	m_tree.ShowGutterPosColumn(FALSE);

	m_sort.nBy1 = TDCC_NONE;
	m_sort.nBy2 = TDCC_NONE;
	m_sort.nBy3 = TDCC_NONE;
	m_sort.bAscending1 = -1;
	m_sort.bAscending2 = FALSE;
	m_sort.bAscending3 = FALSE;

	m_data.SetDefaultCommentsFormat(m_cfDefault);
}

CToDoCtrl::~CToDoCtrl()
{
	GraphicsMisc::VerifyDeleteObject(m_hFontDone);
	GraphicsMisc::VerifyDeleteObject(m_brDue);
	GraphicsMisc::VerifyDeleteObject(m_brDueToday);
}

void CToDoCtrl::DoDataExchange(CDataExchange* pDX)
{
	CRuntimeDlg::DoDataExchange(pDX);
	
	DDX_Control(pDX, IDC_TASKLIST, m_tree);
	DDX_Control(pDX, IDC_ALLOCTO, m_cbAllocTo);
	DDX_Control(pDX, IDC_ALLOCBY, m_cbAllocBy);
	DDX_Control(pDX, IDC_STATUS, m_cbStatus);
	DDX_Control(pDX, IDC_CATEGORY, m_cbCategory);
	DDX_Control(pDX, IDC_TAGS, m_cbTags);
	DDX_Control(pDX, IDC_TIMEEST, m_eTimeEstimate);
	DDX_Control(pDX, IDC_TIMESPENT, m_eTimeSpent);
	DDX_Control(pDX, IDC_DUEDATE, m_dateDue);
	DDX_Control(pDX, IDC_DUETIME, m_timeDue);
	DDX_Control(pDX, IDC_DONEDATE, m_dateDone);
	DDX_Control(pDX, IDC_DONETIME, m_timeDone);
	DDX_Control(pDX, IDC_STARTDATE, m_dateStart);
	DDX_Control(pDX, IDC_STARTTIME, m_timeStart);
	DDX_Control(pDX, IDC_PRIORITY, m_cbPriority);
	DDX_Control(pDX, IDC_RISK, m_cbRisk);
	DDX_Control(pDX, IDC_PERCENT, m_ePercentDone);
	DDX_Control(pDX, IDC_COST, m_eCost);
	DDX_Control(pDX, IDC_DEPENDS, m_eDependency);
	DDX_Control(pDX, IDC_PERCENTSPIN, m_spinPercent);
	DDX_Control(pDX, IDC_FILEPATH, m_eFileRef);
	DDX_Control(pDX, IDC_EXTERNALID, m_eExternalID);
	DDX_Control(pDX, IDC_RECURRENCE, m_eRecurrence);
	DDX_Control(pDX, IDC_VERSION, m_cbVersion);
	DDX_Control(pDX, IDC_COMMENTSTYPE, m_cbCommentsType);
	DDX_Control(pDX, IDC_COLOUR, m_cpColour);

	DDX_Text(pDX, IDC_PROJECTNAME, m_sProjectName);
	DDX_Text(pDX, IDC_TIMEEST, m_dTimeEstimate);
	DDX_Text(pDX, IDC_TIMESPENT, m_dTimeSpent);
	DDX_Text(pDX, IDC_EXTERNALID, m_sExternalID);
	DDX_Text(pDX, IDC_DEPENDS, m_sDepends);
	DDX_Text(pDX, IDC_FILEPATH, m_sFileRefPath);
	DDX_Text(pDX, IDC_COST, m_dCost);

	DDX_AutoCBString(pDX, IDC_ALLOCBY, m_sAllocBy);
	DDX_AutoCBString(pDX, IDC_STATUS, m_sStatus);
	DDX_AutoCBString(pDX, IDC_VERSION, m_sVersion);

	DDX_CBPriority(pDX, IDC_PRIORITY, m_nPriority);
	DDX_CBRisk(pDX, IDC_RISK, m_nRisk);
	DDX_ColourPicker(pDX, IDC_COLOUR, m_crColour);
	
	// custom
	if (pDX->m_bSaveAndValidate)
	{
		CString sPercent;
		m_ePercentDone.GetWindowText(sPercent);
		
		m_nPercentDone = max(0, _ttoi(sPercent));
		m_nPercentDone = min(100, m_nPercentDone);

		m_nTimeEstUnits = m_eTimeEstimate.GetUnits();
		m_nTimeSpentUnits = m_eTimeSpent.GetUnits();

		m_cbCategory.GetChecked(m_aCategory);
		m_cbAllocTo.GetChecked(m_aAllocTo);
		m_cbTags.GetChecked(m_aTags);
		m_eRecurrence.GetRecurrenceOptions(m_tRecurrence);

		if (m_crColour == CLR_DEFAULT)
			m_crColour = NOCOLOR; // unset

		CTDCCustomAttributeHelper::GetControlData(this, m_aCustomControls, m_aCustomAttribDefs, m_mapCustomCtrlData);
	}
	else
	{
		m_spinPercent.SetPos(m_nPercentDone);
		
		m_eTimeEstimate.SetUnits(m_nTimeEstUnits);
		m_eTimeSpent.SetUnits(m_nTimeSpentUnits);

		m_cbCategory.SetChecked(m_aCategory);
		m_cbAllocTo.SetChecked(m_aAllocTo);
		m_cbTags.SetChecked(m_aTags);

		m_eRecurrence.SetRecurrenceOptions(m_tRecurrence);

		if (m_mapCustomCtrlData.GetCount() == 0)
			CTDCCustomAttributeHelper::ClearCustomAttributeControls(this, m_aCustomControls, m_aCustomAttribDefs);
		else
			CTDCCustomAttributeHelper::UpdateCustomAttributeControls(this, m_aCustomControls, m_aCustomAttribDefs, m_mapCustomCtrlData);
	}
}

void CToDoCtrl::UpdateComments(BOOL bSaveAndValidate)
{
	if (bSaveAndValidate)
	{
		m_ctrlComments.GetTextContent(m_sTextComments);
		m_ctrlComments.GetContent(m_customComments);
	}
	else
	{
		// if we can't set the custom comments or
		// there are no custom comments then try setting the text comments
		BOOL bCommentsFocused = m_ctrlComments.HasFocus();

		if (!m_ctrlComments.SetContent(m_customComments, !bCommentsFocused) || m_customComments.IsEmpty())
			m_ctrlComments.SetTextContent(m_sTextComments, !bCommentsFocused);
	}
}

BEGIN_MESSAGE_MAP(CToDoCtrl, CRuntimeDlg)
//{{AFX_MSG_MAP(CToDoCtrl)
	//}}AFX_MSG_MAP
	ON_WM_CTLCOLOR()
	ON_WM_SIZE()
	ON_WM_ERASEBKGND()
	ON_WM_DESTROY()
	ON_WM_CONTEXTMENU()
	ON_WM_LBUTTONDOWN()
	ON_WM_LBUTTONUP()
	ON_WM_MOUSEMOVE()
	ON_WM_SETCURSOR()
	ON_WM_CAPTURECHANGED()
	ON_WM_SHOWWINDOW()
	ON_WM_TIMER()
	ON_NOTIFY(NM_SETFOCUS, IDC_TASKLIST, OnTreeChangeFocus)
	ON_NOTIFY(NM_KILLFOCUS, IDC_TASKLIST, OnTreeChangeFocus)
	ON_NOTIFY(NM_CUSTOMDRAW, IDC_TASKLIST, OnTreeCustomDraw)
	ON_NOTIFY(TVN_GETDISPINFO, IDC_TASKLIST, OnTreeGetDispInfo)
	ON_NOTIFY(TVN_KEYDOWN, IDC_TASKLIST, OnTreeKeyDown)
	ON_NOTIFY(TVN_BEGINLABELEDIT, IDC_TASKLIST, OnTreeBeginlabeledit)
	ON_NOTIFY(TVN_SELCHANGED, IDC_TASKLIST, OnTreeSelChanged)
	ON_NOTIFY(TVN_GETINFOTIP, IDC_TASKLIST, OnTreeGetInfoTip)
	ON_NOTIFY(DTN_DATETIMECHANGE, IDC_DONEDATE, OnDatechange)
	ON_NOTIFY(DTN_DATETIMECHANGE, IDC_DUEDATE, OnDatechange)
	ON_NOTIFY(DTN_DATETIMECHANGE, IDC_STARTDATE, OnDatechange)
	ON_NOTIFY(DTN_CLOSEUP, IDC_DONEDATE, OnDatechange)
	ON_NOTIFY(DTN_CLOSEUP, IDC_DUEDATE, OnDatechange)
	ON_NOTIFY(DTN_CLOSEUP, IDC_STARTDATE, OnDatechange)
	ON_CBN_SELCHANGE(IDC_DUETIME, OnSelChangeDueTime)
	ON_CBN_EDITCHANGE(IDC_DUETIME, OnSelChangeDueTime)
	ON_CBN_SELCHANGE(IDC_STARTTIME, OnSelChangeStartTime)
	ON_CBN_EDITCHANGE(IDC_STARTTIME, OnSelChangeStartTime)
	ON_CBN_SELCHANGE(IDC_DONETIME, OnSelChangeDoneTime)
	ON_CBN_EDITCHANGE(IDC_DONETIME, OnSelChangeDoneTime)
	ON_CBN_SELCHANGE(IDC_PRIORITY, OnChangePriority)
	ON_CBN_SELCHANGE(IDC_RISK, OnChangeRisk)
	ON_EN_CHANGE(IDC_TIMEEST, OnChangeTimeEstimate)
	ON_EN_CHANGE(IDC_TIMESPENT, OnChangeTimeSpent)
	ON_EN_CHANGE(IDC_PROJECTNAME, OnChangeProjectName)
	ON_EN_CHANGE(IDC_EXTERNALID, OnChangeExternalID)
	ON_EN_CHANGE(IDC_COST, OnChangeCost)
	ON_EN_CHANGE(IDC_DEPENDS, OnChangeDependency)
	ON_EN_CHANGE(IDC_RECURRENCE, OnChangeRecurrence)
	ON_REGISTERED_MESSAGE(WM_TDCN_COMMENTSCHANGE, OnChangeComments)
	ON_REGISTERED_MESSAGE(WM_TDCN_COMMENTSKILLFOCUS, OnCommentsKillFocus)
	ON_REGISTERED_MESSAGE(WM_NCG_WANTREDRAW, OnGutterWantRedraw)
	ON_REGISTERED_MESSAGE(WM_NCG_DRAWITEM, OnGutterDrawItem)
	ON_REGISTERED_MESSAGE(WM_NCG_DRAWITEMCOLUMN, OnGutterDrawItemColumn)
	ON_REGISTERED_MESSAGE(WM_NCG_POSTDRAWITEM, OnGutterPostDrawItem)
	ON_REGISTERED_MESSAGE(WM_NCG_RECALCCOLWIDTH, OnGutterRecalcColWidth)
	ON_REGISTERED_MESSAGE(WM_NCG_NOTIFYHEADERCLICK, OnGutterNotifyHeaderClick)
	ON_REGISTERED_MESSAGE(WM_NCG_NOTIFYSCROLL, OnGutterNotifyScroll)
	ON_REGISTERED_MESSAGE(WM_NCG_WIDTHCHANGE, OnGutterWidthChange)
	ON_REGISTERED_MESSAGE(WM_NCG_GETCURSOR, OnGutterGetCursor)
	ON_REGISTERED_MESSAGE(WM_NCG_NOTIFYITEMCLICK, OnGutterNotifyItemClick)
	ON_REGISTERED_MESSAGE(WM_NCG_GETITEMCOLORS, OnGutterGetItemColors)
	ON_REGISTERED_MESSAGE(WM_EE_BTNCLICK, OnEEBtnClick)
	ON_CBN_SELCHANGE(IDC_ALLOCTO, OnSelChangeAllocTo)
	ON_CBN_SELCHANGE(IDC_ALLOCBY, OnSelChangeAllocBy)
	ON_CBN_SELCHANGE(IDC_STATUS, OnSelChangeStatus)
	ON_CBN_SELCHANGE(IDC_VERSION, OnSelChangeVersion)
	ON_CBN_SELCHANGE(IDC_CATEGORY, OnSelChangeCategory)
	ON_CBN_SELCHANGE(IDC_TAGS, OnSelChangeTag)
	ON_CBN_SELENDCANCEL(IDC_ALLOCTO, OnSelCancelAllocTo)
	ON_CBN_SELENDCANCEL(IDC_CATEGORY, OnSelCancelCategory)
	ON_CBN_SELENDCANCEL(IDC_TAGS, OnSelCancelTag)
	ON_EN_CHANGE(IDC_PERCENT, OnChangePercent)
	ON_EN_CHANGE(IDC_FILEPATH, OnChangeFileRefPath)
	ON_REGISTERED_MESSAGE(WM_FE_GETFILEICON, OnFileEditWantIcon)
	ON_REGISTERED_MESSAGE(WM_FE_DISPLAYFILE, OnFileEditDisplayFile)
	ON_REGISTERED_MESSAGE(WM_TLDT_DROP, OnDropObject)
	ON_REGISTERED_MESSAGE(WM_TEN_UNITSCHANGE, OnTimeUnitsChange)
	ON_REGISTERED_MESSAGE(WM_DD_DRAGABORT, OnTreeDragDrop)
	ON_REGISTERED_MESSAGE(WM_DD_DRAGDROP, OnTreeDragDrop)
	ON_REGISTERED_MESSAGE(WM_DD_DRAGENTER, OnTreeDragDrop)
	ON_REGISTERED_MESSAGE(WM_DD_DRAGOVER, OnTreeDragOver)
	ON_REGISTERED_MESSAGE(WM_DD_PREDRAGMOVE, OnTreeDragDrop)
	ON_MESSAGE(WM_TDC_RESTOREFOCUSEDITEM, OnTreeRestoreFocusedItem)
	ON_REGISTERED_MESSAGE(WM_TDCM_HASCLIPBOARD, OnTDCHasClipboard)
	ON_REGISTERED_MESSAGE(WM_TDCM_GETCLIPBOARD, OnTDCGetClipboard)
	ON_REGISTERED_MESSAGE(WM_TDCM_TASKLINK, OnTDCDoTaskLink)
	ON_REGISTERED_MESSAGE(WM_ACB_ITEMADDED, OnAutoComboAddDelete)
	ON_REGISTERED_MESSAGE(WM_ACB_ITEMDELETED, OnAutoComboAddDelete)
	ON_MESSAGE(WM_GETFONT, OnGetFont)
	ON_REGISTERED_MESSAGE(WM_ICC_WANTSPELLCHECK, OnCommentsWantSpellCheck)
	ON_CBN_SELENDOK(IDC_COMMENTSTYPE, OnSelChangeCommentsType)
	ON_WM_SETTINGCHANGE()
	ON_MESSAGE(WM_TDC_REFRESHPERCENTSPINVISIBILITY, OnRefreshPercentSpinVisibility)
	ON_MESSAGE(CPN_SELENDOK, OnChangeColour)
	ON_REGISTERED_MESSAGE(WM_PENDEDIT, OnEditEnd)
	ON_REGISTERED_MESSAGE(WM_PCANCELEDIT, OnEditCancel)
	ON_MESSAGE(WM_TDC_RECREATERECURRINGTASK, OnRecreateRecurringTask)

	// custom data updates
	ON_NOTIFY_RANGE(DTN_DATETIMECHANGE, IDC_FIRST_CUSTOMDATAFIELD, IDC_LAST_CUSTOMDATAFIELD, OnCustomAttributeChange)
	ON_NOTIFY_RANGE(DTN_CLOSEUP, IDC_FIRST_CUSTOMDATAFIELD, IDC_LAST_CUSTOMDATAFIELD, OnCustomAttributeChange)
	ON_CONTROL_RANGE(EN_CHANGE, IDC_FIRST_CUSTOMDATAFIELD, IDC_LAST_CUSTOMDATAFIELD, OnCustomAttributeChange)
	ON_CONTROL_RANGE(CBN_SELCHANGE, IDC_FIRST_CUSTOMDATAFIELD, IDC_LAST_CUSTOMDATAFIELD, OnCustomAttributeChange)
	ON_CONTROL_RANGE(CBN_SELENDCANCEL, IDC_FIRST_CUSTOMDATAFIELD, IDC_LAST_CUSTOMDATAFIELD, OnCustomAttributeChange)
	ON_CONTROL_RANGE(BN_CLICKED, IDC_FIRST_CUSTOMDATAFIELD, IDC_LAST_CUSTOMDATAFIELD, OnCustomAttributeChange)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CToDoCtrl message handlers

void CToDoCtrl::EnableExtendedSelection(BOOL bCtrl, BOOL bShift)
{
	if (bCtrl)
		s_nExtendedSelection |= HOTKEYF_CONTROL;
	else
		s_nExtendedSelection &= ~HOTKEYF_CONTROL;

	if (bShift)
		s_nExtendedSelection |= HOTKEYF_SHIFT;
	else
		s_nExtendedSelection &= ~HOTKEYF_SHIFT;
}

TDC_FILEFMT CToDoCtrl::CompareFileFormat() const
{
    if (m_nFileFormat == FILEFORMAT_CURRENT)
        return TDCFF_SAME;
	
    else if (m_nFileFormat > FILEFORMAT_CURRENT)
        return TDCFF_NEWER;
	
    else
        return TDCFF_OLDER;
}

void CToDoCtrl::EnableEncryption(BOOL bEnable)
{
	if (!bEnable && !m_sPassword.IsEmpty())
	{
		m_sPassword.Empty();
		SetModified(TRUE, TDCA_ENCRYPT, 0);
	}
	else if (bEnable && m_sPassword.IsEmpty())
	{
		CEnString sExplanation(IDS_ENABLEENCRYPTION);

		if (CPasswordDialog::RetrievePassword(TRUE, m_sPassword, sExplanation))
			SetModified(TRUE, TDCA_ENCRYPT, 0);
	}
}

BOOL CToDoCtrl::VerifyPassword(const CString& sExplanation) const
{
	if (!IsEncrypted())
		return TRUE; // no password to check

	return CPasswordDialog::VerifyPassword(m_sPassword, sExplanation);
}

BOOL CToDoCtrl::CanEncrypt()
{
	return CTaskFile::CanEncrypt();
}

BOOL CToDoCtrl::Create(const RECT& rect, CWnd* pParentWnd, UINT nID, BOOL bVisible, BOOL bEnabled)
{
	DWORD dwStyle = (WS_CHILD | (bVisible ? WS_VISIBLE : 0) | (bEnabled ? 0 : WS_DISABLED));
	
	return CRuntimeDlg::Create(NULL, dwStyle, 0/*WS_EX_NOPARENTNOTIFY*/, rect, pParentWnd, nID);
}

BOOL CToDoCtrl::OnInitDialog() 
{
	CRuntimeDlg::OnInitDialog();

	VERIFY (CreateContentControl());

	// disable translation of auto-combos
	CLocalizer::EnableTranslation(m_cbAllocBy, FALSE);
	CLocalizer::EnableTranslation(m_cbAllocTo, FALSE);
	CLocalizer::EnableTranslation(m_cbCategory, FALSE);
	CLocalizer::EnableTranslation(m_cbStatus, FALSE);
	CLocalizer::EnableTranslation(m_cbVersion, FALSE);
	CLocalizer::EnableTranslation(m_cbTags, FALSE);

	// prevent the tree overwriting the label edit
	m_tree.ModifyStyle(0, WS_CLIPSIBLINGS);

	// percent spin
	m_spinPercent.SetRange(0, 100);
	m_spinPercent.SetBuddy(GetDlgItem(IDC_PERCENT));
	
	UDACCEL uda = { 0, m_nPercentIncrement };
	m_spinPercent.SetAccel(1, &uda);
	
	// extra gutter columns
	for (int nCol = 0; nCol < NUM_LATEST_COLUMNS; nCol++)
	{
		TDCCOLUMN& tdcc = COLUMNS[nCol];
		
		if (tdcc.nColID != TDCC_CLIENT)
		{
			m_tree.AddGutterColumn(tdcc.nColID, _T(" "), 0, tdcc.nAlignment);
			
			if (tdcc.bSymbolFont)
			{
				m_tree.SetGutterColumnHeaderTitle(tdcc.nColID, (UINT)tdcc.nIDName, tdcc.szFont);
				m_tree.SetGutterColumnHeaderTip(tdcc.nColID, CEnString(tdcc.nIDLongName));
			}
			else
				m_tree.SetGutterColumnHeaderTitle(tdcc.nColID, CEnString(tdcc.nIDName), tdcc.szFont);
		}
		else
			m_tree.SetGutterColumnHeaderTitle(tdcc.nColID, CEnString(tdcc.nIDName));
	}
	
	// set name of pos column
	m_tree.SetGutterColumnHeaderTitle(OTC_POSCOLUMNID, CEnString(IDS_TDC_COLUMN_POS));
	
	// init dates
	m_dateStart.SendMessage(DTM_SETSYSTEMTIME, GDT_NONE, 0);
	m_dateDue.SendMessage(DTM_SETSYSTEMTIME, GDT_NONE, 0);
	m_dateDone.SendMessage(DTM_SETSYSTEMTIME, GDT_NONE, 0);
	
	m_dtTree.Register(&m_tree, this);
	m_dtFileRef.Register(&m_eFileRef, this); 

	// task icon image list
	LoadTaskIcons();

	// custom font
	if (m_hFontTree)
	{
		m_tree.SendMessage(WM_SETFONT, (WPARAM)m_hFontTree, TRUE);
		
		if (HasStyle(TDCS_COMMENTSUSETREEFONT))
			m_ctrlComments.SendMessage(WM_SETFONT, (WPARAM)m_hFontTree, TRUE);
		else if (m_hFontComments)
			m_ctrlComments.SendMessage(WM_SETFONT, (WPARAM)m_hFontComments, TRUE);
	}

	InitEditPrompts();

	m_cpColour.SetWindowText(CString((LPCTSTR)IDS_SAMPLETEXT));
	
	// tree drag drop
	m_treeDragDrop.Initialize(this);
	
	// enabled states
	UpdateControls();
	SetFocusToTasks();

	// snapshot the font char width
	CClientDC dc(this);
	CFont* pOld = (CFont*)dc.SelectObject(GetParent()->GetFont());

	m_fAveHeaderCharWidth = GraphicsMisc::GetAverageCharWidth(&dc);
	dc.SelectObject(pOld);

	// notify parent that we have been created
	CWnd* pParent = GetParent();

	if (pParent)
		pParent->SendMessage(WM_PARENTNOTIFY, MAKEWPARAM(WM_CREATE, GetDlgCtrlID()), (LPARAM)GetSafeHwnd());

	// and start the track timer
	// which runs persistently
	SetTimer(TIMER_TRACK, TIMETRACKPERIOD, NULL);

	// and the time which checks for midnight (day changeover)
	SetTimer(TIMER_MIDNIGHT, MIDNIGHTPERIOD, NULL);
	
	return FALSE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CToDoCtrl::UpdateItemHeight()
{
	short nItemHeight = m_tree.GetItemHeight();
	short nMinHeight = CalcMinItemImageHeight();

	if (nItemHeight == nMinHeight)
		return; // nothing more to do

	if (nMinHeight == 16)
		m_tree.SetItemHeight(-1);

	else if (nItemHeight < 17)
		m_tree.SetItemHeight(nMinHeight);

	else // nothing else to do
		return;

	CRect rTree;
	m_tree.GetWindowRect(rTree);
	m_tree.SetWindowPos(NULL, 0, 0, rTree.Width(), rTree.Height(), SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOZORDER);
}

short CToDoCtrl::CalcMinItemImageHeight() const
{
	// if gridlines not showing then no more to do
	if (m_tree.GetGridlineColor() == NOCOLOR)
		return 16;
		
	// see if any of: Icon, file-ref or dependency columns
	// are showing
	if (!IsColumnShowing(TDCC_ICON) && 
		!IsColumnShowing(TDCC_DEPENDENCY) && 
		!IsColumnShowing(TDCC_FILEREF) &&
		!HasStyle(TDCS_TREETASKICONS))
	{
		return 16;
	}

	// all else
	return 17; 
}

void CToDoCtrl::LoadTaskIcons()
{
	const COLORREF MAGENTA = RGB(255, 0, 255);

	VERIFY(m_ilTaskIcons.LoadImages(m_sLastSavePath, MAGENTA, HasStyle(TDCS_SHOWDEFAULTTASKICONS)));

	// update tree
	if (HasStyle(TDCS_TREETASKICONS) && !IsColumnShowing(TDCC_ICON)) // this overrides
		TreeView_SetImageList(m_tree, m_ilTaskIcons, TVSIL_NORMAL);
	else
		TreeView_SetImageList(m_tree, NULL, TVSIL_NORMAL);

	UpdateItemHeight();
}

BOOL CToDoCtrl::CreateContentControl(BOOL bResendComments)
{
	CPreferences prefs;
	
	// save outgoing content prefs provided they've already been loaded
	CString sKey = GetPreferencesKey(m_ctrlComments.GetContentFormat());

	if (!m_bFirstLoadCommentsPrefs)
		m_ctrlComments.SavePreferences(&prefs, sKey);

	// check to see if we really need to (re)create the control
	if (m_ctrlComments.IsFormat(m_cfComments))
	{
		// load preferences
		CString sKey = GetPreferencesKey(m_cfComments);

		m_ctrlComments.LoadPreferences(&prefs, sKey);

		// only reset this flag if we have a tasklist attached
		if (!GetFilePath().IsEmpty())
			m_bFirstLoadCommentsPrefs = FALSE;

		return TRUE;
	}

	// create new content control
	CRect rect(0, 211, 171, 238); // in DLU
	CDlgUnits(*this).ToPixels(rect);

	if (m_ctrlComments.GetSafeHwnd())
	{
		::GetWindowRect(m_ctrlComments, &rect);
		ScreenToClient(rect);
	}

	BOOL bVisible = (m_nMaxState != TDCMS_MAXTASKLIST) || HasStyle(TDCS_SHOWCOMMENTSALWAYS);
	DWORD dwStyle = WS_TABSTOP | WS_CHILD  | (bVisible ? WS_VISIBLE : 0) | WS_CLIPSIBLINGS; 

	BOOL bSuccess = m_mgrContent.CreateContentControl(m_cfComments, m_ctrlComments, 
													IDC_COMMENTS, dwStyle, WS_EX_CLIENTEDGE, rect, *this);

	if (!bSuccess)
	{
		// now that we are going to fallback on m_cfDefault
		// check to see if we really need to (re)create the control
		if (!m_ctrlComments.IsFormat(m_cfDefault))
		{
			bSuccess = m_mgrContent.CreateContentControl(m_cfDefault, m_ctrlComments, 
														IDC_COMMENTS, dwStyle, WS_EX_CLIENTEDGE, rect, *this);
		}
		else
			bSuccess = TRUE;

		// update comments type
		if (bSuccess)
			m_cfComments = m_cfDefault;
	}

	if (bSuccess)
	{
		// set the font
		SetTreeFont(m_hFontTree);
		SetCommentsFont(m_hFontComments, bResendComments);

		// make sure its in the right pos in the tab order
		::SetWindowPos(m_ctrlComments, ::GetDlgItem(*this, IDC_COMMENTSLABEL), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

		// (re)set the edit prompt
		m_mgrPrompts.SetEditPrompt(m_ctrlComments, IDS_TDC_EDITPROMPT_COMMENTS);

		if (CThemed::IsThemeActive())
			m_ctrlComments.SetUITheme(&m_theme);

		// load preferences
		CString sKey = GetPreferencesKey(m_cfComments);
		m_ctrlComments.LoadPreferences(&prefs, sKey);
	}

	return bSuccess; 
}

void CToDoCtrl::InitEditPrompts()
{
	m_mgrPrompts.SetEditPrompt(IDC_PROJECTNAME, *this, IDS_TDC_EDITPROMPT_PROJECT);
	m_mgrPrompts.SetEditPrompt(m_eFileRef, IDS_TDC_EDITPROMPT_FILEREF);
	m_mgrPrompts.SetEditPrompt(m_eExternalID, IDS_TDC_EDITPROMPT_EXTID);
	m_mgrPrompts.SetEditPrompt(m_eDependency, IDS_TDC_EDITPROMPT_DEPENDS); 

	m_mgrPrompts.SetComboEditPrompt(m_cbAllocBy, IDS_TDC_EDITPROMPT_NAME);
	m_mgrPrompts.SetComboEditPrompt(m_cbAllocTo.GetSafeHwnd(), IDS_TDC_EDITPROMPT_NAME);
	m_mgrPrompts.SetComboEditPrompt(m_cbCategory.GetSafeHwnd(), IDS_TDC_EDITPROMPT_CATEGORY);
	m_mgrPrompts.SetComboEditPrompt(m_cbTags.GetSafeHwnd(), IDS_TDC_EDITPROMPT_TAGS);
	m_mgrPrompts.SetComboEditPrompt(m_cbStatus, IDS_TDC_EDITPROMPT_STATUS);
	m_mgrPrompts.SetComboEditPrompt(m_cbVersion, IDS_TDC_EDITPROMPT_VER);

	m_mgrPrompts.SetEditPrompt(m_ctrlComments, IDS_TDC_EDITPROMPT_COMMENTS);
	m_mgrPrompts.SetPrompt(m_tree, IDS_TDC_TASKLISTPROMPT, TVM_GETCOUNT);
}

void CToDoCtrl::SetCompletedTaskColor(COLORREF color)
{
	if (color != m_crDone)
	{
		m_crDone = color;
		
		if (m_tree.GetSafeHwnd())
			m_tree.Invalidate(FALSE);
	}
}

void CToDoCtrl::SetFlaggedTaskColor(COLORREF color)
{
	if (color != m_crFlagged)
	{
		m_crFlagged = color;
		
		if (m_tree.GetSafeHwnd())
			m_tree.Invalidate(FALSE);
	}
}

void CToDoCtrl::SetReferenceTaskColor(COLORREF color)
{
	if (color != m_crReference)
	{
		m_crReference = color;
		
		if (m_tree.GetSafeHwnd())
			m_tree.Invalidate(FALSE);
	}
}

void CToDoCtrl::SetGridlineColor(COLORREF color) 
{ 
	m_tree.SetGridlineColor(color);
	UpdateItemHeight();
}

BOOL CToDoCtrl::SetTreeFont(HFONT hFont)
{
	m_hFontTree = hFont;

	if (m_tree.GetSafeHwnd())
	{
		if (!hFont) // set to our font
		{
			// for some reason i can not yet explain, our font
			// is not correctly set so we use our parent's font instead
			// hFont = (HFONT)SendMessage(WM_GETFONT);
			hFont = (HFONT)GetParent()->SendMessage(WM_GETFONT);
		}

		HFONT hTreeFont = (HFONT)m_tree.SendMessage(WM_GETFONT);
		BOOL bChange = (hFont != hTreeFont || !GraphicsMisc::SameFontNameSize(hFont, hTreeFont));

		if (bChange)
			m_tree.SendMessage(WM_SETFONT, (WPARAM)hFont, TRUE);

		// done font
		if (bChange || m_hFontDone == NULL)
		{
			GraphicsMisc::VerifyDeleteObject(m_hFontDone);
			m_hFontDone = NULL;

			if (HasStyle(TDCS_STRIKETHOUGHDONETASKS))
			{
				m_hFontDone = GraphicsMisc::CreateFont(hFont, GMFS_STRIKETHRU);
			}

			// bold tree font
			GraphicsMisc::VerifyDeleteObject(m_hFontBold);
			m_hFontBold = GraphicsMisc::CreateFont(hFont, GMFS_BOLD);
		} 

		// comments font
		if (HasStyle(TDCS_COMMENTSUSETREEFONT))
			m_ctrlComments.SendMessage(WM_SETFONT, (WPARAM)hFont, TRUE);

		UpdateItemHeight();
	}

	return TRUE;
}

BOOL CToDoCtrl::SetCommentsFont(HFONT hFont)
{
	return SetCommentsFont(hFont, TRUE);
}

BOOL CToDoCtrl::SetCommentsFont(HFONT hFont, BOOL bResendComments)
{
	m_hFontComments = hFont;
	
	if (m_ctrlComments.GetSafeHwnd())
	{
		if (!HasStyle(TDCS_COMMENTSUSETREEFONT))
		{
			if (!hFont)
				hFont = (HFONT)GetStockObject(DEFAULT_GUI_FONT);

			m_ctrlComments.SendMessage(WM_SETFONT, (WPARAM)hFont, TRUE);
		}

		if (bResendComments)
		{
			// we've had some trouble with plugins using the
			// richedit control so after a font change we always
			// resend the content
			if (m_customComments.GetLength())
			{
				if (!m_ctrlComments.SetContent(m_customComments, FALSE))
					m_ctrlComments.SetTextContent(m_sTextComments, FALSE);
			}
			else
				m_ctrlComments.SetTextContent(m_sTextComments, FALSE);
		}
	}
	
	return TRUE;
}

void CToDoCtrl::SetMaxColumnWidth(int nWidth)
{
	if ((nWidth == -1 || nWidth >= 16) && nWidth != m_nMaxColWidth)
	{
		m_nMaxColWidth = nWidth;

		m_tree.RecalcGutter();
	}
}

TDCCOLUMN* CToDoCtrl::GetColumn(TDC_COLUMN nColID)
{
	int nCol = GetColumnIndex(nColID);

	if (nCol != -1)
		return &COLUMNS[nCol];

	// else
	return NULL;
}

int CToDoCtrl::GetColumnIndex(TDC_COLUMN nColID)
{
	int nCol = NUM_LATEST_COLUMNS;
	
	while (nCol--)
	{
		if (COLUMNS[nCol].nColID == nColID)
			return nCol;
	}

	// else
	return -1;
}

void CToDoCtrl::SetUITheme(const UITHEME& theme)
{
	ASSERT(GetSafeHwnd());

	m_theme = theme;

	GraphicsMisc::VerifyDeleteObject(m_brUIBack);
	m_brUIBack.CreateSolidBrush(m_theme.crAppBackLight);

	m_ctrlComments.SetUITheme(&m_theme);
	Invalidate();
}

void CToDoCtrl::SetMaximizeState(TDC_MAXSTATE nState)
{
	HandleUnsavedComments();

	if (m_nMaxState != nState)
	{
		m_nMaxState = nState;

		if (GetSafeHwnd())
		{
			Resize();
			UpdateControls(NULL);
			
			// make sure focus is set correctly
			switch (nState)
			{
			case TDCMS_NORMAL:
				// nothing to do
				break;
				
			case TDCMS_MAXTASKLIST:
				if (!HasStyle(TDCS_SHOWCOMMENTSALWAYS) || !m_ctrlComments.HasFocus())
				{
					SetFocusToTasks();
				}
				break;
				
			case TDCMS_MAXCOMMENTS:
				m_ctrlComments.SetFocus();
				break;
			}
		}
	}
}

void CToDoCtrl::OnSize(UINT nType, int cx, int cy) 
{
	CRuntimeDlg::OnSize(nType, cx, cy);
	
	EndLabelEdit(TRUE);
	Resize(cx, cy);
}

void CToDoCtrl::Resize(int cx, int cy)
{
	if (m_tree.GetSafeHwnd())
	{
		if (!cx && !cy)
		{
			CRect rClient;
			GetClientRect(rClient);
			
			cx = rClient.right;
			cy = rClient.bottom;
			
			// check again 
			if (!cx && !cy)
				return;
		}

		// hide unused controls
		ShowHideControls();

		// written to use DeferWindowPos()
		{
			CDeferWndMove dwm(60);
			CRect rAvailable(0, 0, cx, cy);

			ReposComments(&dwm, rAvailable);
			ReposControls(&dwm, rAvailable);
			ReposProjectName(&dwm, rAvailable);
			ReposTaskTree(&dwm, rAvailable);
		}
		
		UpdateWindow();
		UpdateSelectedTaskPath();
	}
}

void CToDoCtrl::ReposProjectName(CDeferWndMove* pDWM, CRect& rAvailable)
{
	// project name
	CDlgUnits dlu(*this);
	CRect rLabel = GetCtrlRect(IDC_PROJECTLABEL); 
	CRect rProject = GetCtrlRect(IDC_PROJECTNAME); 

	int nOffset = rAvailable.left - rLabel.left;

	rLabel.OffsetRect(nOffset, 0);
	rProject.left += nOffset;
	rProject.right = rAvailable.right;

	pDWM->MoveWindow(GetDlgItem(IDC_PROJECTLABEL), rLabel);
	pDWM->MoveWindow(GetDlgItem(IDC_PROJECTNAME), rProject);

	if (m_nMaxState != TDCMS_MAXTASKLIST && HasStyle(TDCS_SHOWPROJECTNAME))
	{
		rAvailable.top = rProject.bottom + 5;
	}
	else
		rAvailable.top = rProject.top;
}

void CToDoCtrl::ReposControls(CDeferWndMove* pDWM, CRect& rAvailable)
{
	if (m_nMaxState != TDCMS_MAXTASKLIST)
	{
		// first count up the visible controls
		// so we can allocate the correct amount of space
		BOOL bAllVisible = !HasStyle(TDCS_SHOWCTRLSASCOLUMNS);

		CTDCControlArray aControls;
		int nVisibleCtrls = GetControls(aControls, TRUE);

		if (nVisibleCtrls == 0) // nothing to do
			return;
		
		// for converting dlus to pixels
		CDlgUnits dlu(*this);
		
		int nCols = 2, nRows = 2;
		
		// figure out number of ctrl rows / columns
		if (HasStyle(TDCS_AUTOREPOSCTRLS))
		{
			switch (m_nControlsPos)
			{
			case TDCUIL_RIGHT: // vertical
			case TDCUIL_LEFT:
				{
					// to take account of the 'extra' CTRLVSPACING that will occur
					// below the last row we add it into our calculations
					int nAvailHeight = rAvailable.Height() + dlu.ToPixelsX(CTRLVSPACING) - 2;
					int nCtrlHeight = dlu.ToPixelsY(2 * CTRLHEIGHT + CTRLVSPACING);
					
					nRows = max(2, nAvailHeight / nCtrlHeight);
					nCols = (nVisibleCtrls / nRows) + ((nVisibleCtrls % nRows) ? 1 : 0);
				}
				break;

			case TDCUIL_BOTTOM: // horizontal
				{
					// to take account of the 'extra' CTRLHSPACING that will occur
					// after the last column we add it into our calculations
					int nAvailWidth = rAvailable.Width() + dlu.ToPixelsX(CTRLHSPACING);
					int nCtrlWidth = dlu.ToPixelsX(CTRLCTRLLEN + CTRLHSPACING);
					
					nCols = max(2, nAvailWidth / nCtrlWidth);
					nRows = (nVisibleCtrls / nCols) + ((nVisibleCtrls % nCols) ? 1 : 0);
				}
				break;
			}
		}
		else // fixed controls
		{
			switch (m_nControlsPos)
			{
			case TDCUIL_RIGHT: // vertical
			case TDCUIL_LEFT:
				{
					nRows = 12;
					nCols = (nVisibleCtrls / nRows) + ((nVisibleCtrls % nRows) ? 1 : 0);
				}
				break;

			case TDCUIL_BOTTOM: // horizontal
				{
					nCols = 2;
					nRows = (nVisibleCtrls / nCols) + ((nVisibleCtrls % nCols) ? 1 : 0);
				}
				break;
			}
		}
		
		// calc ctrl rect
		CRect rCtrls(rAvailable);

		// remember to take account of the 'taking-account' of the 'extra' spacing above
		switch (m_nControlsPos)
		{
		case TDCUIL_RIGHT: // vertical
			{
				rCtrls.top += 2;
				rCtrls.left = rCtrls.right - nCols * dlu.ToPixelsX(CTRLCTRLLEN + CTRLHSPACING) + dlu.ToPixelsX(CTRLHSPACING);
				rAvailable.right = rCtrls.left - SPLITSIZE;
			}
			break;
			
		case TDCUIL_LEFT:
			{
				rCtrls.top += 2;
				rCtrls.right = rCtrls.left + nCols * dlu.ToPixelsX(CTRLCTRLLEN + CTRLHSPACING) - dlu.ToPixelsX(CTRLHSPACING);
				rAvailable.left = rCtrls.right + SPLITSIZE;
			}
			break;
			
		case TDCUIL_BOTTOM: // horizontal
			{
				rCtrls.top = rCtrls.bottom - nRows * dlu.ToPixelsY(2 * CTRLHEIGHT + CTRLVSPACING) + dlu.ToPixelsY(CTRLVSPACING);
				rAvailable.bottom = rCtrls.top - SPLITSIZE;
			}
			break;
		}
		
		// now iterate the visible controls settings their positions dynamically
		CRect rItem(rCtrls);
		rItem.bottom = rItem.top + dlu.ToPixelsY(2 * CTRLHEIGHT);
		rItem.right = rItem.left + dlu.ToPixelsX(CTRLCTRLLEN);

		int nCol = 0;
		int nRightEdge = rCtrls.right;
		
		for (int nCtrl = 0; nCtrl < aControls.GetSize(); nCtrl++)
		{
			const CTRLITEM& ctrl = aControls[nCtrl];
			ASSERT(IsCtrlShowing(ctrl));

			ReposControl(ctrl, pDWM, &dlu, rItem, nRightEdge);
			
			nCol++;

			// if we've passed the column count, reset to next row
			if (nCol >= nCols)
			{
				nCol = 0;
				rItem.OffsetRect(-(rItem.left - rCtrls.left), dlu.ToPixelsY(2 * CTRLHEIGHT + CTRLVSPACING));
			}
			else // offset to next column
			{
				rItem.OffsetRect(dlu.ToPixelsX(CTRLCTRLLEN + CTRLHSPACING), 0);
			}
		}
	}
}

int CToDoCtrl::GetControls(CTDCControlArray& aControls, BOOL bVisible) const
{
	aControls.RemoveAll();

	// standard controls, except for file link which is always last
	int nCtrl;
	for (nCtrl = 0; nCtrl < NUM_CTRLITEMS - 1; nCtrl++)
	{
		CTRLITEM& ctrl = CTRLITEMS[nCtrl];

		if (!bVisible || IsCtrlShowing(ctrl))
			aControls.Add(ctrl);
	}
	
	// custom attribs
	for (nCtrl = 0; nCtrl < m_aCustomControls.GetSize(); nCtrl++)
	{
		CTRLITEM ctrl = m_aCustomControls[nCtrl];
		aControls.Add(ctrl);
	}

	// finally file link
	CTRLITEM& ctrlFileRef = CTRLITEMS[NUM_CTRLITEMS - 1];

	if (IsCtrlShowing(ctrlFileRef))
		aControls.Add(ctrlFileRef);

	return aControls.GetSize();
}

void CToDoCtrl::ReposControl(const CTRLITEM& ctrl, CDeferWndMove* pDWM, const CDlgUnits* pDLU, 
							 const CRect& rItem, int nClientRight)
{
	// move label
	CRect rLabel(rItem);
	rLabel.bottom = rLabel.top + pDLU->ToPixelsY(CTRLHEIGHT);

	pDWM->MoveWindow(GetDlgItem(ctrl.nLabelID), rLabel);
				
	// move control
	CRect rCtrl(rItem);
	rCtrl.top += pDLU->ToPixelsY(CTRLHEIGHT);
				
	// some special cases
	switch (ctrl.nCtrlID)
	{
	case IDC_PERCENT:
		{
			CRect rSpin = GetCtrlRect(IDC_PERCENTSPIN); // gets current pos
			rSpin.OffsetRect(rCtrl.right - rSpin.right, 0);
			rSpin.top = rCtrl.top;
			rSpin.bottom = rCtrl.bottom;
			pDWM->MoveWindow(&m_spinPercent, rSpin);
			
			rCtrl.right = rSpin.left + 2;
		}
		break;
		
	case IDC_FILEPATH:
		{
			// file path control can take as much space as is left
			rCtrl.right = nClientRight;
		}
		break;
		
	case IDC_ALLOCTO:
	case IDC_ALLOCBY:
	case IDC_STATUS:
	case IDC_CATEGORY:
	case IDC_TAGS:
	case IDC_PRIORITY:
	case IDC_VERSION:
	case IDC_RISK:
		{
			// don't move it if it hasn't actually moved
			// to prevent flickering. Note we do this 
			// before we add 200 to the bottom of the rect
			CRect rPos;
			GetCtrlRect(ctrl.nCtrlID);

			if (rPos == rCtrl)
				return; 

			// else
			rCtrl.bottom += 200; // combo box drop height
		}
		break;

/*		// handle custom attributes
	default:
		if (CTDCCustomAttributeHelper::IsCustomControl(ctrl.nCtrlID))
		{
			TDCCUSTOMATTRIBUTEDEFINITION attribDef;

			if (CTDCCustomAttributeHelper::GetAttributeDef(ctrl.nCol, m_aCustomAttribDefs, attribDef))
			{
				switch (attribDef.GetDataType())
				{
				case TDCCA_ICON:
					// same as combos above
					{
						CRect rPos;
						GetCtrlRect(ctrl.nCtrlID);

						if (rPos == rCtrl)
							return; 

						// else
						rCtrl.bottom += 200; // combo box drop height
					}
					break;
				}
			}
		}
		break;
*/	}

	pDWM->MoveWindow(GetDlgItem(ctrl.nCtrlID), rCtrl);
}

void CToDoCtrl::ReposTaskTree(CDeferWndMove* pDWM, const CRect& rAvailable)
{
	pDWM->MoveWindow(&m_tree, rAvailable);
}

void CToDoCtrl::ReposComments(CDeferWndMove* pDWM, CRect& rAvailable /*in/out*/) 
{
	CRect rComField(rAvailable);
	CDlgUnits dlu(*this);

	if (m_nMaxState != TDCMS_MAXTASKLIST || HasStyle(TDCS_SHOWCOMMENTSALWAYS))
	{
		if (m_nMaxState == TDCMS_MAXCOMMENTS)
		{
			rAvailable.SetRectEmpty();
		}
		else 
		{
			int nCommentSize = HasStyle(TDCS_SHAREDCOMMENTSHEIGHT) ? s_nCommentsSize : m_nCommentsSize;
			
			switch (m_nCommentsPos)
			{
			case TDCUIL_RIGHT: // vertical
				{
					nCommentSize = min(nCommentSize, rAvailable.Width() - GetMinNonCommentSize());
					rComField.top = 2;
					rComField.left = rAvailable.right - nCommentSize;
					rAvailable.right = rComField.left - SPLITSIZE;
				}
				break;
				
			case TDCUIL_LEFT:
				{
					nCommentSize = min(nCommentSize, rAvailable.Width() - GetMinNonCommentSize());
					rComField.top = 2;
					rComField.right = rAvailable.left + nCommentSize;
					rAvailable.left = rComField.right + SPLITSIZE;
				}
				break;
				
			case TDCUIL_BOTTOM: // horizontal
				{
					nCommentSize = min(nCommentSize, rAvailable.Height() - GetMinNonCommentSize());
					rComField.left = 0;
					rComField.top = rAvailable.bottom - nCommentSize;
					rAvailable.bottom = rComField.top - SPLITSIZE;
				}
				break;
			}

			// splitter between comments and controls/task tree
			if (m_nCommentsPos == TDCUIL_BOTTOM || m_nCommentsPos == m_nControlsPos)
			{
				CRect rDiv(rComField);

				switch (m_nCommentsPos)
				{
				case TDCUIL_RIGHT: // vertical
					{
						rDiv.right = rDiv.left - SPLITSIZE;
						rDiv.left = rDiv.right - 2;

						if (m_nMaxState != TDCMS_MAXTASKLIST)
							rAvailable.right -= (SPLITSIZE + 2);
					}
					break;
					
				case TDCUIL_LEFT:
					{
						rDiv.left = rDiv.right + SPLITSIZE;
						rDiv.right = rDiv.left + 2;

						if (m_nMaxState != TDCMS_MAXTASKLIST)
							rAvailable.left += (SPLITSIZE + 2);
					}
					break;
					
				case TDCUIL_BOTTOM: // horizontal
					{
						rDiv.bottom = rDiv.top - SPLITSIZE;
						rDiv.top = rDiv.bottom - 2;
						rAvailable.bottom -= SPLITSIZE + 2;
					}
					break;
				}

				pDWM->MoveWindow(GetDlgItem(IDC_SPLITTER), rDiv);
			}
		}

		// label
		// NOTE: label can be resized smaller so we must restore 
		// the actual size each time
		CRect rLabel = GetCtrlRect(IDC_COMMENTSLABEL);

		rLabel.right = rLabel.left + dlu.ToPixelsX(CTRLLABELLEN);
		rLabel.OffsetRect(rComField.left - rLabel.left, rComField.top - rLabel.top);

		// type combo
		// NOTE: combo can be resized smaller so we must restore 
		// the actual size each time
		CRect rCombo = GetCtrlRect(IDC_COMMENTSTYPE);

		rCombo.right = rCombo.left + dlu.ToPixelsX(CTRLCTRLLEN);
		rCombo.OffsetRect(rLabel.right + 4 - rCombo.left, rComField.top - rCombo.top);
		
		if (m_nCommentsPos == TDCUIL_LEFT)
		{
			rLabel.right = min(rCombo.left, rComField.right);
			rCombo.right = min(rCombo.right, rComField.right);
		}

		pDWM->MoveWindow(GetDlgItem(IDC_COMMENTSLABEL), rLabel);
		pDWM->MoveWindow(&m_cbCommentsType, rCombo);

		// make sure drop list displays correctly
		CDialogHelper::RefreshMaxDropWidth(m_cbCommentsType);
		
		// comments below combo
		rComField.top = rCombo.bottom + 5;
		pDWM->MoveWindow(GetDlgItem(IDC_COMMENTS), rComField);
	}
}

void CToDoCtrl::ShowHideControl(const CTRLITEM& ctrl)
{
	CWnd* pCtrl = GetDlgItem(ctrl.nCtrlID);
	CWnd* pLabel = GetDlgItem(ctrl.nLabelID);
	
	if (!pCtrl || !pLabel)
		return;
	
	BOOL bCtrlShowing = IsCtrlShowing(ctrl);
	int nShowCtrl = bCtrlShowing ? SW_SHOW : SW_HIDE;
	
	pCtrl->ShowWindow(nShowCtrl);
	pLabel->ShowWindow(nShowCtrl);
	
	// some additions and modifications
	switch (ctrl.nCtrlID)
	{
	case IDC_PERCENT:
		m_spinPercent.ShowWindow(nShowCtrl);
		break;
	}
}

void CToDoCtrl::ShowHideControls()
{
	// now show/hide appropriate controls
	int nCtrl;
	for (nCtrl = 0; nCtrl < NUM_CTRLITEMS; nCtrl++)
	{
		const CTRLITEM& ctrl = CTRLITEMS[nCtrl];
		ShowHideControl(ctrl);
	}

	// always show custom controls
	for (nCtrl = 0; nCtrl < m_aCustomControls.GetSize(); nCtrl++)
	{
		const CTRLITEM& ctrl = m_aCustomControls[nCtrl];
		ShowHideControl(ctrl);
	}

	// comments
	BOOL bCommentsVis = (m_nMaxState != TDCMS_MAXTASKLIST || HasStyle(TDCS_SHOWCOMMENTSALWAYS));
	ShowControls(IDC_COMMENTSLABEL, IDC_COMMENTS, bCommentsVis);

	// re-hide the comments type combo if necessary
	BOOL bHide = !bCommentsVis || (m_mgrContent.GetNumContent() <= 1);
	m_cbCommentsType.ShowWindow(bHide ? SW_HIDE : SW_SHOW);

	// and horz comments divider
	bHide = !bCommentsVis || (m_nCommentsPos != TDCUIL_BOTTOM && m_nCommentsPos != m_nControlsPos);
 	GetDlgItem(IDC_SPLITTER)->ShowWindow(bHide ? SW_HIDE : SW_SHOW);

	// task tree
	UpdateTasklistVisibility();
	
	// project name
	BOOL bMaximize = (m_nMaxState != TDCMS_NORMAL);
	BOOL bShowProjectName = !bMaximize && HasStyle(TDCS_SHOWPROJECTNAME);
	ShowControls(IDC_PROJECTLABEL, IDC_PROJECTNAME, bShowProjectName);
}

void CToDoCtrl::EnableDisableControl(const CTRLITEM& ctrl, DWORD dwTaskID, BOOL bEnable, BOOL bReadOnly, BOOL bIsParent, BOOL bEditTime)
{
	CWnd* pCtrl = GetDlgItem(ctrl.nCtrlID);
	CWnd* pLabel = GetDlgItem(ctrl.nLabelID);
	
	if (!pCtrl || !pLabel)
		return;
	
	BOOL bCtrlShowing = IsCtrlShowing(ctrl);
	
	// control state
	RT_CTRLSTATE nCtrlState = (!bEnable || !bCtrlShowing) ? RTCS_DISABLED : 
								(bReadOnly ? RTCS_READONLY : RTCS_ENABLED);
	RT_CTRLSTATE nLabelState = (CThemed::IsThemeActive() && bCtrlShowing) ? RTCS_ENABLED : nCtrlState;
	
	// some additions and modifications
	switch (ctrl.nCtrlID)
	{
	case IDC_PERCENT:
		{
			BOOL bAveSubTaskCompletion = HasStyle(TDCS_AVERAGEPERCENTSUBCOMPLETION) && bIsParent;
			BOOL bEditPercent = !HasStyle(TDCS_AUTOCALCPERCENTDONE) && (GetSelectedCount() > 1 || !bAveSubTaskCompletion);
			
			if (!bEditPercent && bEnable)
				nCtrlState = RTCS_READONLY;
			
			SetControlState(m_spinPercent, nCtrlState);
		}
		break;
		
	case IDC_TIMEEST:
		if (!bEditTime && bEnable)
			nCtrlState = RTCS_READONLY;
		break;
		
	case IDC_TIMESPENT:
		{
			BOOL bIsClocking = m_data.IsTaskTimeTrackable(dwTaskID) && (dwTaskID == m_dwTimeTrackTaskID);
			
			if ((!bEditTime || bIsClocking) && bEnable)
				nCtrlState = RTCS_READONLY;
		}
		break;
		
	case IDC_DUETIME:
		if (nCtrlState == RTCS_ENABLED && !SelectedTaskHasDate(TDCD_DUE))
			nCtrlState = RTCS_READONLY;
		break;
		
	case IDC_STARTTIME:
		if (nCtrlState == RTCS_ENABLED && !SelectedTaskHasDate(TDCD_START))
			nCtrlState = RTCS_READONLY;
		break;
		
	case IDC_DONETIME:
		if (nCtrlState == RTCS_ENABLED && !SelectedTaskHasDate(TDCD_DONE))
			nCtrlState = RTCS_READONLY;
		break;
	}
	
	SetControlState(*pLabel, nLabelState);
	SetControlState(*pCtrl, nCtrlState);
}

void CToDoCtrl::EnableDisableControls(HTREEITEM hti)
{
	BOOL bMaximize = (m_nMaxState != TDCMS_NORMAL);
	BOOL bEnable = (hti && !bMaximize);
	BOOL bIsParent = Selection().ItemsAreAllParents();
	BOOL bReadOnly = IsReadOnly();
	BOOL bEditTime = !bIsParent || HasStyle(TDCS_ALLOWPARENTTIMETRACKING);

	DWORD dwTaskID = GetTaskID(hti);

	if (dwTaskID)
		bEnable &= (HasStyle(TDCS_ALLOWREFERENCEEDITING) || !m_data.IsTaskReference(dwTaskID));

	// now enable/disable appropriate controls
	int nCtrl;
	for (nCtrl = 0; nCtrl < NUM_CTRLITEMS; nCtrl++)
	{
		const CTRLITEM& ctrl = CTRLITEMS[nCtrl];
		EnableDisableControl(ctrl, dwTaskID, bEnable, bReadOnly, bIsParent, bEditTime);
	}

	// and customcontrols
	for (nCtrl = 0; nCtrl < m_aCustomControls.GetSize(); nCtrl++)
	{
		const CTRLITEM& ctrl = m_aCustomControls[nCtrl];
		EnableDisableControl(ctrl, dwTaskID, bEnable, bReadOnly, bIsParent, bEditTime);
	}

	// comments
	CString sCommentsType;
	GetSelectedTaskCustomComments(sCommentsType);
	BOOL bEditComments = (m_mgrContent.FindContent(sCommentsType) != -1);
	
	BOOL bCommentsVis = (m_nMaxState != TDCMS_MAXTASKLIST || HasStyle(TDCS_SHOWCOMMENTSALWAYS));
	RT_CTRLSTATE nCtrlState = (!bCommentsVis || !hti) ? RTCS_DISABLED : 
							((bReadOnly || !bEditComments) ? RTCS_READONLY : RTCS_ENABLED);

	switch (nCtrlState)
	{
	case RTCS_DISABLED:
		SetControlState(this, IDC_COMMENTS, nCtrlState);
		break;

	case RTCS_ENABLED:
		SetControlState(this, IDC_COMMENTS, nCtrlState);
		m_ctrlComments.SetReadOnly(FALSE);
		break;

	case RTCS_READONLY:
		SetControlState(this, IDC_COMMENTS, RTCS_ENABLED);
		m_ctrlComments.SetReadOnly(TRUE);
		break;
	}

	// if bEditComments is FALSE it means we have multiple comments types
	// selected. So we enable the selected comments type but keep the 
	// comments control readonly
	if (nCtrlState == RTCS_READONLY && !bReadOnly)
		SetControlState(this, IDC_COMMENTSTYPE, RTCS_ENABLED);
	else
		SetControlState(this, IDC_COMMENTSTYPE, nCtrlState);

	RT_CTRLSTATE nLabelState = (CThemed::IsThemeActive() && bCommentsVis) ? RTCS_ENABLED : nCtrlState;
	SetControlState(this, IDC_COMMENTSLABEL, nLabelState);

	// project name
	BOOL bShowProjectName = !bMaximize && HasStyle(TDCS_SHOWPROJECTNAME);
	nCtrlState = !bShowProjectName ? RTCS_DISABLED : (bReadOnly ? RTCS_READONLY : RTCS_ENABLED);
	SetControlState(this, IDC_PROJECTNAME, nCtrlState);

	nLabelState = (CThemed::IsThemeActive() && bShowProjectName) ? RTCS_ENABLED : nCtrlState;
	SetControlState(this, IDC_PROJECTLABEL, nCtrlState);
}

int CToDoCtrl::GetMinNonCommentSize()
{
	if (m_nCommentsPos == TDCUIL_LEFT || m_nCommentsPos == TDCUIL_RIGHT)
	{
		// two field widths
		CDlgUnits dlu(*this);
		return dlu.ToPixelsX(2 * (CTRLLABELLEN + CTRLCTRLLEN + CTRLHSPACING));
	}
	else
		return MINNONCOMMENTHEIGHT;
}

BOOL CToDoCtrl::IsCtrlShowing(const CTRLITEM& ctrl) const
{
	if (m_nMaxState != TDCMS_NORMAL)
		return FALSE;

	// is this a custom control?
	if (CTDCCustomAttributeHelper::IsCustomControl(ctrl.nCtrlID))
		return TRUE;

	BOOL bAllVisible = !HasStyle(TDCS_SHOWCTRLSASCOLUMNS);
	
	switch (ctrl.nCtrlID)
	{
	case IDC_COLOUR:
		return (!HasStyle(TDCS_COLORTEXTBYATTRIBUTE) &&
				!HasStyle(TDCS_COLORTEXTBYPRIORITY) &&
				!HasStyle(TDCS_COLORTEXTBYNONE));

	case IDC_DUETIME:
		if (HasStyle(TDCS_HIDEDUETIMEFIELD))
			return FALSE;
		break;

	case IDC_STARTTIME:
		if (HasStyle(TDCS_HIDESTARTTIMEFIELD))
			return FALSE;
		break;

	case IDC_DONETIME:
		if (HasStyle(TDCS_HIDEDONETIMEFIELD))
			return FALSE;
		break;
	}
	
	// all else
	return (bAllVisible || IsColumnShowing(ctrl.nCol));
}

// wrapper for CTreeCtrl
BOOL CToDoCtrl::SetCheckImageList(HIMAGELIST hImageList)
{
	m_hilDone = hImageList;
	
	if (HasStyle(TDCS_TREECHECKBOXES) && !IsColumnShowing(TDCC_DONE))
		TreeView_SetImageList(m_tree, m_hilDone, TVSIL_STATE);
	
	return TRUE;
}

void CToDoCtrl::OnTreeSelChanged(NMHDR* pNMHDR, LRESULT* pResult)
{
	*pResult = 0;

	// ignore if we're in the middle of deleting all items
	if (m_bDeletingAll)
		return;

	LPNMTREEVIEW pNMTV = (LPNMTREEVIEW)pNMHDR;
	HTREEITEM hti = pNMTV->itemNew.hItem;
	
	// update comments once only
	HandleUnsavedComments();
	
	// we don't support nothing having being selected unless there
	// are no items in the tree
	BOOL bCtrl = Misc::KeyIsPressed(VK_CONTROL) && (s_nExtendedSelection & HOTKEYF_CONTROL);
	BOOL bShift = Misc::KeyIsPressed(VK_SHIFT) && (s_nExtendedSelection & HOTKEYF_SHIFT);
	
	// cursor handled here
	// <shift>+cursor handled here
	// <ctrl>+cursor handled in PreTranslateMessage
	// <ctrl>+<shift>+cursor handled in PreTranslateMessage
	if (m_wKeyPress)
	{
		switch (m_wKeyPress)
		{
		case VK_NEXT:  
		case VK_DOWN:
		case VK_UP:
		case VK_PRIOR: 
		case VK_RIGHT:
		case VK_LEFT:
		case VK_HOME:
		case VK_END:
			if (!bCtrl)
			{
				Selection().RemoveAll();
				
				if (bShift)
					Selection().AddItems(Selection().GetAnchor(), hti);
				else
				{
					Selection().SetAnchor(hti);
					Selection().AddItem(hti);
				}
			}
			break;

		default:
			// else handle alphanum method of changing selection
			Selection().RemoveAll();
			Selection().SetAnchor(hti);
			Selection().AddItem(hti);
			break;
		}
		
		m_tree.RedrawGutter(); 
	}

	// update edit fields to reflect new selection unless the 
	// cursor key is still down ie the selection is about to change again
	if (!Misc::IsCursorKey(m_wKeyPress) || !Misc::KeyIsPressed(m_wKeyPress))
		UpdateControls(hti); 

	m_wKeyPress = 0;

	// notify parent
	GetParent()->PostMessage(WM_TDCN_SELECTIONCHANGE);
}

void CToDoCtrl::UpdateSelectedTaskPath()
{
	// add the item path to the header
	if (HasStyle(TDCS_SHOWPATHINHEADER))
	{
		CEnString sHeader(IDS_TDC_COLUMN_TASK);
		
		if (GetSelectedCount() == 1)
		{
			CRect rTree;
			m_tree.GetClientRect(rTree);

			int nColWidthInChars = (int)(rTree.Width() / m_fAveHeaderCharWidth);
			CString sPath = m_data.GetTaskPath(GetSelectedTaskID(), nColWidthInChars);
			
			if (!sPath.IsEmpty())
				sHeader.Format(_T("%s [%s]"), CEnString(IDS_TDC_COLUMN_TASK), sPath);
		}
		
		m_tree.SetGutterColumnHeaderTitle(TDCC_CLIENT, sHeader);
	}
}

void CToDoCtrl::UpdateControls(HTREEITEM hti)
{
	if (!hti)
		hti = GetSelectedItem();
	
	BOOL bReadOnly = IsReadOnly();
	CString sCommentsType(m_cfDefault);
	
	if (hti)
	{
		int nSelCount = GetSelectedCount();
		DWORD dwTaskID = GetTrueTaskID(hti); 

		BOOL bMaximize = (m_nMaxState != TDCMS_NORMAL);
		BOOL bEnable = (hti && !bMaximize);
		BOOL bIsParent = Selection().ItemsAreAllParents();
		BOOL bAveSubTaskCompletion = HasStyle(TDCS_AVERAGEPERCENTSUBCOMPLETION) && bIsParent;
		BOOL bEditTime = !bIsParent || HasStyle(TDCS_ALLOWPARENTTIMETRACKING);
		BOOL bEditPercent = !HasStyle(TDCS_AUTOCALCPERCENTDONE) && (nSelCount > 1 || !bAveSubTaskCompletion);

		m_sTextComments = GetSelectedTaskComments();
		m_nPriority = (int)GetSelectedTaskPriority();
		m_nRisk = (int)GetSelectedTaskRisk();
		m_sAllocBy = GetSelectedTaskAllocBy();
		m_sStatus = GetSelectedTaskStatus();
		m_sFileRefPath = GetSelectedTaskFileRef();
		m_sExternalID = GetSelectedTaskExtID();
		m_sVersion = GetSelectedTaskVersion();
		m_crColour = GetSelectedTaskColor();
		m_dCost = GetSelectedTaskCost();

		if (m_crColour == 0)
			m_crColour = CLR_DEFAULT;

		// can only edit comments if the comments type for the task is available
		m_customComments = GetSelectedTaskCustomComments(sCommentsType);
		
		// special cases
		GetSelectedTaskCategories(m_aCategory);
		GetSelectedTaskAllocTo(m_aAllocTo);
		GetSelectedTaskTags(m_aTags);

		CStringArray aDepends;
		GetSelectedTaskDependencies(aDepends);
		m_sDepends = Misc::FormatArray(aDepends);

		if (bEditTime)
		{
			m_dTimeEstimate = GetSelectedTaskTimeEstimate(m_nTimeEstUnits);
			m_dTimeSpent = GetSelectedTaskTimeSpent(m_nTimeSpentUnits);
		}
		else
		{
			m_nTimeEstUnits = s_tdDefault.nTimeEstUnits;
			m_dTimeEstimate = m_data.CalcTimeEstimate(dwTaskID, m_nTimeEstUnits);
			m_nTimeSpentUnits = s_tdDefault.nTimeSpentUnits;
			m_dTimeSpent = m_data.CalcTimeSpent(dwTaskID, m_nTimeEstUnits);
		}

		// chess clock for time spent
		m_eTimeSpent.CheckButton(ID_TIME_TRACK, (dwTaskID == m_dwTimeTrackTaskID));
		m_eTimeSpent.EnableButton(ID_TIME_TRACK, bEditTime && bEnable && !bReadOnly && 
									nSelCount == 1 && m_data.IsTaskTimeTrackable(dwTaskID));

		// dependency link button
		m_eDependency.EnableButton(ID_DEPENDS_LINK, bEnable && !m_sDepends.IsEmpty());

		// percent done
		if (IsSelectedTaskDone())
			m_nPercentDone = 100;
		
		else if (bEditPercent)
			m_nPercentDone = GetSelectedTaskPercent();
		else
			m_nPercentDone = m_data.CalcPercentDone(dwTaskID);		
		
		// special handling for start date/time
		COleDateTime dateStart = GetSelectedTaskDate(TDCD_START);
		SetCtrlDate(m_dateStart, dateStart);
		m_timeStart.SetOleTime(dateStart);

		// special handling for due date/time
		COleDateTime dateDue = GetSelectedTaskDate(TDCD_DUE);
		SetCtrlDate(m_dateDue, dateDue);
		m_timeDue.SetOleTime(dateDue);

		// special handling for done date/time
		COleDateTime dateDone = GetSelectedTaskDate(TDCD_DONE);
		SetCtrlDate(m_dateDone, dateDone);
		m_timeDone.SetOleTime(dateDone);

		// recurrence
		GetSelectedTaskRecurrence(m_tRecurrence);

		// use due date if present else start date
		if (dateDue.m_dt > 0)
			m_eRecurrence.SetDefaultDate(dateDue);
		else
			m_eRecurrence.SetDefaultDate(dateStart);

		// custom attributes
		GetSelectedTaskCustomAttributeData(m_mapCustomCtrlData);
	}
	else // clear controls
	{
		m_sTextComments.Empty();
		m_customComments.Empty();
		m_nPriority = 0;
		m_nRisk = 0;
		m_sAllocBy.Empty();
		m_sStatus.Empty();
		m_sFileRefPath.Empty();
		m_sExternalID.Empty();
		m_sDepends.Empty();
		m_nPercentDone = 0;
		m_dTimeEstimate = m_dTimeSpent = 0;
		m_dCost = 0;
		m_tRecurrence = TDIRECURRENCE();
		m_sVersion.Empty();
		m_crColour = CLR_DEFAULT;

		m_aCategory.RemoveAll();
		m_aAllocTo.RemoveAll();
		m_aTags.RemoveAll();

		COleDateTime date;
		SetCtrlDate(m_dateDue, date);
		SetCtrlDate(m_dateDone, date);
		SetCtrlDate(m_dateStart, date);

		m_eTimeSpent.EnableButton(ID_TIME_TRACK, FALSE);
		m_eDependency.EnableButton(ID_DEPENDS_LINK, FALSE);

		m_mapCustomCtrlData.RemoveAll();
	}

	// recreate comments control if necessary
	// note: if more than one comments type is selected then sCommentsType
	// will be empty which will put the comments type combo in an
	// indeterminate state which is the desired effect since this requires
	// the user to reset the type before they can edit
	m_cfComments = sCommentsType;
	m_cbCommentsType.SetSelectedFormat(m_cfComments);

	if (!sCommentsType.IsEmpty())
		CreateContentControl();

	// update control content
	UpdateData(FALSE);
	UpdateComments(FALSE);

	// and task header
	UpdateSelectedTaskPath();
	
	// show hide controls
	EnableDisableControls(hti);
	
	// tree
	m_tree.ModifyStyle(bReadOnly ? TVS_EDITLABELS : 0, bReadOnly ? 0 : TVS_EDITLABELS);
	m_treeDragDrop.EnableDragDrop(!bReadOnly);
}

void CToDoCtrl::UpdateTasklistVisibility()
{
	BOOL bTasksVis = (m_nMaxState != TDCMS_MAXCOMMENTS);

	// if only the comments are visible then set the focus to the comments
	// before hiding the tasks, else Windows will select all the text
	// in the comments control
	if (!bTasksVis)
		m_ctrlComments.SetFocus();

	m_tree.ShowWindow(bTasksVis ? SW_SHOW : SW_HIDE);
}

void CToDoCtrl::SetCtrlDate(CDateTimeCtrl& ctrl, const COleDateTime& date)
{
	if (date.m_dt)
	{
		ctrl.SetTime(date);
	}
	else
	{
		ctrl.SetTime(COleDateTime::GetCurrentTime());
		ctrl.SendMessage(DTM_SETSYSTEMTIME, GDT_NONE, 0);
	}
}

void CToDoCtrl::UpdateTask(TDC_ATTRIBUTE nAttrib, DWORD dwFlags)
{
	if (!m_tree.GetSafeHwnd())
		return;
	
	HTREEITEM hti = GetSelectedItem();
	
	if (!hti)
		return;
	
	// special case to circumvent CSaveFocus else
	// it can mess up IME input
	if (nAttrib == TDCA_COMMENTS)
	{
		m_bCommentsChange = TRUE;
		return;
	}	
	
	// else
	CSaveFocus sf;
	UpdateData();
	
	switch (nAttrib)
	{
	case TDCA_DONEDATE:
		{
			COleDateTime date;
			m_dateDone.GetTime(date);
			
			if (SetSelectedTaskDate(TDCD_DONE, date, TRUE))
			{
				// check if we need to modify percent done also
				if (!IsSelectedTaskDone())
				{
					int nPercentDone = GetSelectedTaskPercent();
					
					if (nPercentDone == 100)
						nPercentDone = 0;
					
					SetSelectedTaskPercentDone(nPercentDone);
					m_nPercentDone = nPercentDone;
					
					UpdateData(FALSE);
				}
				else if (m_nPercentDone != 100) // make the percent field look right
				{
					m_nPercentDone = 100;
					UpdateData(FALSE);
				}
			}
			else
			{
				UpdateControls(hti);
			}

			// enable done time field depending on whether the done date is valid or not
			m_timeDone.EnableWindow(date.m_dt > 0);
		}
		break;
		
	case TDCA_DONETIME:
		{
			SetSelectedTaskDate(TDCD_DONETIME, m_timeDone.GetOleTime(), TRUE);
		//	RecalcSelectedTimeEstimate();
		}
		break;
		
	case TDCA_STARTDATE:
		{
			COleDateTime date;
			m_dateStart.GetTime(date);
			
			SetSelectedTaskDate(TDCD_STARTDATE, date, TRUE);
			RecalcSelectedTimeEstimate();

			// enable start time field depending on whether the start date is valid or not
			m_timeStart.EnableWindow(date.m_dt > 0);
		}
		break;
		
	case TDCA_STARTTIME:
		{
			SetSelectedTaskDate(TDCD_STARTTIME, m_timeStart.GetOleTime(), TRUE);
			RecalcSelectedTimeEstimate();
		}
		break;
		
	case TDCA_DUEDATE:
		{
			COleDateTime date;
			m_dateDue.GetTime(date);
			
			SetSelectedTaskDate(TDCD_DUEDATE, date, TRUE);
			RecalcSelectedTimeEstimate();

			// enable due time field depending on whether the due date is valid or not
			m_timeDue.EnableWindow(date.m_dt > 0);
		}
		break;
		
	case TDCA_DUETIME:
		{
			SetSelectedTaskDate(TDCD_DUETIME, m_timeDue.GetOleTime(), TRUE);
			RecalcSelectedTimeEstimate();
		}
		break;
		
	case TDCA_COST:
		SetSelectedTaskCost(m_dCost);
		break;
		
	case TDCA_RECURRENCE:
		SetSelectedTaskRecurrence(m_tRecurrence);
		break;
		
	case TDCA_DEPENDENCY:
		{
			m_eDependency.EnableButton(ID_DEPENDS_LINK, !m_sDepends.IsEmpty());
			CStringArray aDepends;
			Misc::Split(m_sDepends, aDepends);
			SetSelectedTaskDependencies(aDepends);
		}
		break;
		
	case TDCA_PRIORITY:
		SetSelectedTaskPriority(m_nPriority);
		break;
		
	case TDCA_RISK:
		SetSelectedTaskRisk(m_nRisk);
		break;
		
	case TDCA_COLOR:
		SetSelectedTaskColor(m_crColour);
		break;
		
	case TDCA_EXTERNALID:
		SetSelectedTaskExtID(m_sExternalID);
		break;
		
	case TDCA_ALLOCTO:
		SetSelectedTaskAllocTo(m_aAllocTo);
		break;
		
	case TDCA_ALLOCBY:
		SetSelectedTaskAllocBy(m_sAllocBy);
		break;
		
	case TDCA_STATUS:
		SetSelectedTaskStatus(m_sStatus);
		break;
		
	case TDCA_VERSION:
		SetSelectedTaskVersion(m_sVersion);
		break;
		
	case TDCA_CATEGORY:
		SetSelectedTaskCategories(m_aCategory);
		break;
		
	case TDCA_TAGS:
		SetSelectedTaskTags(m_aTags);
		break;
		
	case TDCA_PERCENT:
		{
			// note: we need to take account of 'done' state too because
			// we maintain the task percent at its pre-done state even
			// if the UI says its '100%'
			BOOL bWasDone = IsSelectedTaskDone();
			SetSelectedTaskPercentDone(m_nPercentDone);
			
			// check if we need to update 'done' state
			BOOL bDoneChange = (bWasDone && m_nPercentDone < 100) || (!bWasDone && m_nPercentDone == 100);
			
			if (bDoneChange)
			{
				SetSelectedTaskDone(m_nPercentDone == 100);
				nAttrib = TDCA_DONEDATE;
			}
		}
		break;
		
	case TDCA_TIMEEST:
		if (dwFlags & UTF_TIMEUNITSONLY)
			SetSelectedTaskTimeEstimateUnits(m_nTimeEstUnits, dwFlags & UTF_RECALCTIME);
		else
			SetSelectedTaskTimeEstimate(m_dTimeEstimate, m_nTimeEstUnits);
		break;
		
	case TDCA_TIMESPENT:
		if (dwFlags & UTF_TIMEUNITSONLY)
			SetSelectedTaskTimeSpentUnits(m_nTimeSpentUnits, dwFlags & UTF_RECALCTIME);
		else
			SetSelectedTaskTimeSpent(m_dTimeSpent, m_nTimeSpentUnits);
		break;
		
	case TDCA_FILEREF:
		SetSelectedTaskFileRef(m_sFileRefPath);
		break;
		
	default:
		// handle custom attributes
		if (CTDCCustomAttributeHelper::IsCustomAttribute(nAttrib))
		{
			CString sData, sAttribID = CTDCCustomAttributeHelper::GetAttributeTypeID(nAttrib, m_aCustomAttribDefs);

			if (m_mapCustomCtrlData.Lookup(sAttribID, sData))
				SetSelectedTaskCustomAttributeData(sAttribID, sData);
		}
	}
}

void CToDoCtrl::RecalcSelectedTimeEstimate()
{
	if (HasStyle(TDCS_AUTOCALCTIMEESTIMATES))
	{
		const TODOITEM* pTDI = GetSelectedTask();
		
		if (pTDI)
		{
			COleDateTime dateStart = pTDI->dateStart, dateDue = pTDI->dateDue;
			
			if (dateStart > 0 && dateDue >= dateStart) // both exist
			{
				double dEst = 0;
				double dStartTime = CDateHelper::GetTimeOnly(pTDI->dateStart) * 24;
				double dDueTime = CDateHelper::GetTimeOnly(pTDI->dateDue) * 24;

				double dStartDate = CDateHelper::GetDateOnly(pTDI->dateStart);
				double dDueDate = CDateHelper::GetDateOnly(pTDI->dateDue);

				// no time? then calculate the difference inclusive
				if (dStartTime == 0 && dDueTime == 0)
				{
					int nDays = CDateHelper::CalcWeekdaysFromTo(dateStart, dateDue, TRUE); // TRUE == inclusive
					dEst = nDays * CTimeHelper::GetHoursInOneDay();
				}
				else
				{
					if (dStartTime == 0)
						dStartTime = 9; // 9 am

					if (dDueTime == 0)
						dDueTime = 18; // 6 pm

					// are the dates on the same day?
					if (dStartDate == dDueDate)
						dEst = dDueTime - dStartTime;

					// are the dates on adjacent days
					else if (dStartDate + 1 == dDueDate)
					{
						// add time from start until end of the day.
						dEst = 18 - dStartTime; // 18 == 6pm

						// add time from 9am until due
						dEst += dDueTime - 9; // 9 == 9am
					}
					else // at least one clear day between them
					{
						int nDays = CDateHelper::CalcWeekdaysFromTo(dateStart, dateDue, FALSE) - 1;

						// add time from start until end of the day.
						dEst = 18 - dStartTime; // 18 == 6pm

						// add time from 9am until due
						dEst += dDueTime - 9; // 9 == 9am

						if (nDays > 0)
							dEst += nDays * CTimeHelper::GetHoursInOneDay();
					}
				}

				dEst = CTimeHelper().GetTime(dEst, THU_HOURS, m_nTimeEstUnits);

				SetSelectedTaskTimeEstimate(dEst, m_nTimeEstUnits);
			}
// 			else
// 				SetSelectedTaskTimeEstimate(0);
		}
	}
}

void CToDoCtrl::OnChangePriority()
{
	UpdateTask(TDCA_PRIORITY);
}

void CToDoCtrl::OnChangeRisk()
{
	UpdateTask(TDCA_RISK);
}

void CToDoCtrl::OnDatechange(NMHDR* pNMHDR, LRESULT* pResult)
{
	// only update the task when the popdown calendar is closed
	// or the popdown calendar is being closed
	switch (pNMHDR->idFrom)
	{
	case IDC_DONEDATE:	
		if (m_dateDone.GetMonthCalCtrl() == NULL || pNMHDR->code == DTN_CLOSEUP)
			UpdateTask(TDCA_DONEDATE);	
		break;
	
	case IDC_STARTDATE:	
		if (m_dateStart.GetMonthCalCtrl() == NULL || pNMHDR->code == DTN_CLOSEUP)
			UpdateTask(TDCA_STARTDATE); 
		break;
	
	case IDC_DUEDATE:	
		if (m_dateDue.GetMonthCalCtrl() == NULL || pNMHDR->code == DTN_CLOSEUP)
			UpdateTask(TDCA_DUEDATE); 	
		break;
	}
	
	*pResult = 0;
}

void CToDoCtrl::OnSelChangeDueTime()
{
	UpdateTask(TDCA_DUETIME);
}

void CToDoCtrl::OnSelChangeStartTime()
{
	UpdateTask(TDCA_STARTTIME);
}

void CToDoCtrl::OnSelChangeDoneTime()
{
	UpdateTask(TDCA_DONETIME);
}

void CToDoCtrl::OnCustomAttributeChange(UINT nCtrlID, NMHDR* /*pNMHDR*/, LRESULT* pResult)
{
	OnCustomAttributeChange(nCtrlID);
	*pResult = 0;
}

void CToDoCtrl::OnCustomAttributeChange(UINT nCtrlID)
{
	ASSERT(CTDCCustomAttributeHelper::IsCustomControl(nCtrlID));

	CUSTOMATTRIBCTRLITEM ctrl;

	if (CTDCCustomAttributeHelper::GetControl(nCtrlID, m_aCustomControls, ctrl))
	{
		TDC_ATTRIBUTE nAttribID = CTDCCustomAttributeHelper::GetAttributeID(ctrl.nCol, m_aCustomAttribDefs);
		ASSERT(CTDCCustomAttributeHelper::IsCustomAttribute(nAttribID));

		UpdateTask(nAttribID);
	}
}

BOOL CToDoCtrl::SetSelectedTaskCustomAttributeData(const CString& sAttribID, const CString& sData)
{
	if (IsReadOnly())
		return FALSE;

	if (sAttribID.IsEmpty())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskCustomAttributeData(dwTaskID, sAttribID, sData);
			
		if (nItemRes == SET_CHANGE)
		{
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
 		SetModified(TRUE, TDCA_CUSTOMATTRIB, dwModTaskID);

		// update UI
		CUSTOMATTRIBCTRLITEM ctrl;

		if (CTDCCustomAttributeHelper::GetControl(sAttribID, m_aCustomControls, ctrl))
		{
			CTDCCustomAttributeHelper::UpdateCustomAttributeControl(this, ctrl, m_aCustomAttribDefs, sData);
		}
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::OnEraseBkgnd(CDC* pDC) 
{
	int nDC = pDC->SaveDC();
	
	// clip out all the child controls to reduce flicker
	if (!(GetStyle() & WS_CLIPCHILDREN))
	{
		if (m_tree.GetSafeHwnd())
			ExcludeControls(pDC, IDC_FIRST + 1, IDC_LAST - 1, TRUE);
	}

	if (m_brUIBack.GetSafeHandle())
	{
		CRect rect;
		GetClientRect(rect);
		pDC->FillSolidRect(rect, m_theme.crAppBackLight);
	}
	else
		CRuntimeDlg::OnEraseBkgnd(pDC);
	
	pDC->RestoreDC(nDC);
	
	return TRUE;
}

void CToDoCtrl::SetDefaultAllocToNames(const CStringArray& aNames)
{
	// remove the old names first
	if (m_aDefAllocTo.GetSize())
		m_cbAllocTo.DeleteItems(m_aDefAllocTo);

	m_cbAllocTo.AddUniqueItems(aNames);
	m_aDefAllocTo.Copy(aNames);
}

void CToDoCtrl::SetDefaultAllocByNames(const CStringArray& aNames)
{
	// remove the old names first
	if (m_aDefAllocBy.GetSize())
		m_cbAllocBy.DeleteItems(m_aDefAllocBy);

	m_cbAllocBy.AddUniqueItems(aNames);
	m_aDefAllocBy.Copy(aNames);
}

void CToDoCtrl::SetDefaultCategoryNames(const CStringArray& aNames)
{
	// remove the old names first
	if (m_aDefCategory.GetSize())
		m_cbCategory.DeleteItems(m_aDefCategory);

	m_cbCategory.AddUniqueItems(aNames);
	m_aDefCategory.Copy(aNames);
}

void CToDoCtrl::SetDefaultTagNames(const CStringArray& aNames)
{
	// remove the old names first
	if (m_aDefTags.GetSize())
		m_cbTags.DeleteItems(m_aDefTags);

	m_cbTags.AddUniqueItems(aNames);
	m_aDefTags.Copy(aNames);
}

void CToDoCtrl::SetDefaultStatusNames(const CStringArray& aNames)
{
	// remove the old names first
	if (m_aDefStatus.GetSize())
		m_cbStatus.DeleteItems(m_aDefStatus);

	m_cbStatus.AddUniqueItems(aNames);
	m_aDefStatus.Copy(aNames);
}

void CToDoCtrl::NewList()
{
	Flush();

	BOOL bConfirmDelete = HasStyle(TDCS_CONFIRMDELETE);
	
	if (bConfirmDelete)
		SetStyle(TDCS_CONFIRMDELETE, FALSE);
	
	DeleteAllTasks();
	
	if (bConfirmDelete)
		SetStyle(TDCS_CONFIRMDELETE, TRUE);
	
	m_sProjectName.Empty();
	m_nFileVersion = 0;
	m_bModified = FALSE;
	m_sPassword.Empty();
	
	UpdateData(FALSE);
	UpdateComments(FALSE);
}

BOOL CToDoCtrl::SetSelectedTaskColor(COLORREF color)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskColor(dwTaskID, color);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_COLOR);
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		m_cpColour.SetColour(color);
		SetModified(TRUE, TDCA_COLOR, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::EditSelectedTaskIcon()
{
	if (IsReadOnly())
		return FALSE;

	CTDLTaskIconDlg dialog(m_ilTaskIcons, GetSelectedTaskIcon());

	if (dialog.DoModal() != IDOK)
		return FALSE;

	return SetSelectedTaskIcon(dialog.GetIconName());
}

BOOL CToDoCtrl::ClearSelectedTaskIcon()
{
	if (IsReadOnly())
		return FALSE;

	return SetSelectedTaskIcon(_T(""));
}

BOOL CToDoCtrl::SetSelectedTaskIcon(const CString& sIcon)
{
	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskIcon(dwTaskID, sIcon);
		
		if (nItemRes == SET_CHANGE)
		{
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
		SetModified(TRUE, TDCA_ICON, dwModTaskID);
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::CanPasteText()
{
	return (!IsReadOnly() && CWinClasses::IsEditControl(::GetFocus()));
}

BOOL CToDoCtrl::PasteText(const CString& sText)
{
	if (!CanPasteText() || sText.IsEmpty())
		return FALSE;

	HWND hFocus = ::GetFocus();
	::SendMessage(hFocus, EM_REPLACESEL, TRUE, (LPARAM)(LPCTSTR)sText);
	
	return TRUE;
}

BOOL CToDoCtrl::SetSelectedTaskComments(const CString& sComments, const CBinaryData& customComments)
{
	return SetSelectedTaskComments(sComments, customComments, FALSE); 
}

BOOL CToDoCtrl::SetSelectedTaskComments(const CString& sComments, const CBinaryData& customComments, BOOL bInternal)
{
	if (IsReadOnly())
		return FALSE;

	if (!bInternal)
		Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;

	IMPLEMENT_UNDOEDIT();
		
	// do not change the custom comments unless the comments
	// type matches the current content control type
	BOOL bCanChangeCustom = (m_mgrContent.FindContent(m_cfComments) != -1);
	LPCTSTR szFormat = bCanChangeCustom ? m_cfComments : NULL;
	
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskComments(dwTaskID, sComments, customComments, szFormat);
		
		if (nItemRes == SET_CHANGE)
		{
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		// refresh the comments of the active task if we were called externally
		// note: we don't use SetTextChange because that doesn't handle custom comments
		if (!bInternal)
		{
			// try custom comments if that's what they are
			if (!bCanChangeCustom || 
				!m_ctrlComments.SetContent(customComments, FALSE) || 
				customComments.IsEmpty())
			{
				// else text comments
				m_ctrlComments.SetTextContent(sComments, FALSE);
			}
		}

		Selection().InvalidateAll();

		SetModified(TRUE, TDCA_COMMENTS, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::SetSelectedTaskTitle(const CString& sTitle)
{
	if (IsReadOnly() || GetSelectedCount() != 1)
		return FALSE;

	Flush();
	
	IMPLEMENT_UNDOEDIT();
		
	DWORD dwTaskID = GetSelectedTaskID();
	int nRes = m_data.SetTaskTitle(dwTaskID, sTitle);
	
	if (nRes == SET_CHANGE)
	{
		m_tree.SetItemText(GetSelectedItem(), sTitle);
		
		InvalidateSelectedItem();
		SetModified(TRUE, TDCA_TASKNAME, dwTaskID);
	}
	
	return (nRes != SET_FAILED);
}

CString CToDoCtrl::GetSelectedTaskComments() const
{
	if (GetSelectedCount() == 1)
		return m_data.GetTaskComments(GetSelectedTaskID());
	
	// else
	return _T("");
}

const CBinaryData& CToDoCtrl::GetSelectedTaskCustomComments(CString& sCommentsTypeID) const
{
	if (GetSelectedCount() == 1)
		return m_data.GetTaskCustomComments(GetSelectedTaskID(), sCommentsTypeID);
	
	// else
	sCommentsTypeID.Empty();
    POSITION pos = Selection().GetFirstItemPos();
	
    while (pos)
    {
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		
		const TODOITEM* pTDI = m_data.GetTask(dwTaskID);
		ASSERT (pTDI);
		
		if (pTDI)
		{
			if (sCommentsTypeID.IsEmpty())
			{
				sCommentsTypeID = pTDI->sCommentsTypeID;
			}
			else if (sCommentsTypeID != pTDI->sCommentsTypeID)
			{
				sCommentsTypeID.Empty();
				break;
			}
		}
	}
	
	static CBinaryData content;
	return content;
}

HTREEITEM CToDoCtrl::GetSelectedItem() const 
{ 
	if (m_tree.GetSafeHwnd())
		return Selection().GetFirstItem();
	
	// else
	return NULL;
}

BOOL CToDoCtrl::GetSelectionBoundingRect(CRect& rSelection) const
{
	if (Selection().GetBoundingRect(rSelection))
	{
		m_tree.ClientToScreen(rSelection);
		ScreenToClient(rSelection);

		return TRUE;
	}

	return FALSE;
}

CString CToDoCtrl::GetSelectedTaskTitle() const
{
	if (GetSelectedCount() == 1)
		return GetTaskTitle(GetSelectedTaskID());
	
	// else
	return _T("");
}

int CToDoCtrl::GetSelectedTaskPriority() const
{
   int nPriority = -1;
   POSITION pos = Selection().GetFirstItemPos();

   while (pos)
   {
      DWORD dwTaskID = Selection().GetNextItemData(pos);
      int nTaskPriority = m_data.GetTaskPriority(dwTaskID);

      if (nPriority == -1)
         nPriority = nTaskPriority;

      else if (nPriority != nTaskPriority)
         return -1;
   }
	
	return nPriority;
}

int CToDoCtrl::GetSelectedTaskRisk() const
{
   int nRisk = -1;
   POSITION pos = Selection().GetFirstItemPos();

   while (pos)
   {
      DWORD dwTaskID = Selection().GetNextItemData(pos);
      int nTaskRisk = m_data.GetTaskRisk(dwTaskID);

      if (nRisk == -1)
         nRisk = nTaskRisk;

      else if (nRisk != nTaskRisk)
         return -1; // == various
   }
	
	return nRisk;
}

CString CToDoCtrl::GetSelectedTaskIcon() const
{
	CString sIcon;
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		sIcon = m_data.GetTaskIcon(dwTaskID);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			CString sTaskIcon = m_data.GetTaskIcon(dwTaskID);
			
			if (sIcon != sTaskIcon)
				return _T("");
		}
	}
	
	return sIcon;
}

BOOL CToDoCtrl::SelectedTaskHasDate(TDC_DATE nDate) const
{
	return (GetSelectedTaskDate(nDate).m_dt) > 0.0;
}

COleDateTime CToDoCtrl::GetSelectedTaskDate(TDC_DATE nDate) const
{
	COleDateTime date; // == 0
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		date = m_data.GetTaskDate(dwTaskID, nDate);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			COleDateTime dateTask = m_data.GetTaskDate(dwTaskID, nDate);
			
			// first check if both date are set or not set
			if (date.m_dt == 0.0 && dateTask.m_dt == 0.0)
				continue;
			
			else if (date.m_dt == 0.0) // means dateTask.m_dt != 0
				return dateTask;
			else 
				return date;
		}

		// if we get here then no dates were set
	}
	
	return date;
}

COleDateTime CToDoCtrl::GetTaskDate(DWORD dwTaskID, TDC_DATE nDate) const
{
	return m_data.GetTaskDate(dwTaskID, nDate);
}

int CToDoCtrl::IsSelectedTaskFlagged() const
{
	return m_data.IsTaskFlagged(GetSelectedTaskID());
}

BOOL CToDoCtrl::IsSelectedTaskReference() const
{
	return m_data.IsTaskReference(GetSelectedTaskID());
}

double CToDoCtrl::GetSelectedTaskTimeEstimate(TCHAR& nUnits) const
{
	double dTime = 0.0;
	nUnits = s_tdDefault.nTimeEstUnits;
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		dTime = m_data.GetTaskTimeEstimate(dwTaskID, nUnits);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			
			TCHAR nTaskUnits;
			double dTaskTime = m_data.GetTaskTimeEstimate(dwTaskID, nTaskUnits);
			
			if (dTime != dTaskTime || nUnits != nTaskUnits)
				return 0.0;
		}
	}
	
	return dTime;
}

double CToDoCtrl::CalcSelectedTaskTimeEstimate(TCHAR nUnits) const
{
	double dTime = 0.0;
	
	CHTIList selection;
	Selection().CopySelection(selection);
	CTreeSelectionHelper::RemoveChildDuplicates(selection, m_tree);
	
	POSITION pos = selection.GetHeadPosition();
	
	while (pos)
	{
		HTREEITEM hti = selection.GetNext(pos);
		DWORD dwTaskID = GetTaskID(hti);

		dTime += m_data.CalcTimeEstimate(dwTaskID, nUnits);
	}

	return dTime;
}

double CToDoCtrl::GetSelectedTaskTimeSpent(TCHAR& nUnits) const
{
	double dTime = 0.0;
	nUnits = s_tdDefault.nTimeSpentUnits;
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		dTime = m_data.GetTaskTimeSpent(dwTaskID, nUnits);
		
		while (pos)
		{
			DWORD dwTaskID = Selection().GetNextItemData(pos);
			
			TCHAR nTaskUnits;
			double dTaskTime = m_data.GetTaskTimeSpent(dwTaskID, nTaskUnits);
			
			if (dTime != dTaskTime || nUnits != nTaskUnits)
				return 0.0;
		}
	}
	
	return dTime;
}

double CToDoCtrl::CalcSelectedTaskTimeSpent(TCHAR nUnits) const
{
	double dTime = 0.0;
	
	CHTIList selection;
	Selection().CopySelection(selection);
	CTreeSelectionHelper::RemoveChildDuplicates(selection, m_tree);
	
	POSITION pos = selection.GetHeadPosition();
	
	while (pos)
	{
		HTREEITEM hti = selection.GetNext(pos);
		DWORD dwTaskID = GetTaskID(hti);

		dTime += m_data.CalcTimeSpent(dwTaskID, nUnits);
	}

	return dTime;
}

BOOL CToDoCtrl::SetSelectedTaskPriority(int nPriority)
{
	if (IsReadOnly())
		return FALSE;

	Flush();

	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_FAILED;
	DWORD dwModTaskID = 0;

	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskPriority(dwTaskID, nPriority);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_PRIORITY);
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		if (m_nPriority != nPriority)
		{
			m_nPriority = nPriority;
			m_cbPriority.SetPriority(m_nPriority);
		}
		
		SetModified(TRUE, TDCA_PRIORITY, dwModTaskID);
	}

	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::SetSelectedTaskRisk(int nRisk)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_FAILED;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskRisk(dwTaskID, nRisk);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_RISK);
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		if (m_nRisk != nRisk)
		{
			m_nRisk = nRisk;
			m_cbRisk.SetRisk(m_nRisk);
		}
		
		SetModified(TRUE, TDCA_RISK, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::SetSelectedTaskFlag(BOOL bFlagged)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_FAILED;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskFlag(dwTaskID, bFlagged);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_FLAG);
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		SetModified(TRUE, TDCA_FLAG, dwModTaskID);
	
		if (IsColumnShowing(TDCC_FLAG))
		{
			if (Selection().GetCount() > 1)
				m_tree.RedrawGutter();
			else
				m_tree.RedrawGutterItem((DWORD)GetSelectedItem());
		}
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::IncrementSelectedTaskPriority(BOOL bUp)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_FAILED;
	DWORD dwModTaskID = 0;
	int nAmount = (bUp ? 1 : -1);

	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nPriority = m_data.GetTaskPriority(dwTaskID) + nAmount;

		// need to jump over -1
		if (nPriority < 0)
		{
			if (nAmount < 0)
				nPriority = FM_NOPRIORITY;
			else
				nPriority = 0;
		}

		int nItemRes = m_data.SetTaskPriority(dwTaskID, nPriority);
		
		if (nItemRes == SET_CHANGE)
		{
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		m_nPriority = m_cbPriority.IncrementPriority(nAmount);
		SetModified(TRUE, TDCA_PRIORITY, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

void CToDoCtrl::ShowTaskHasIncompleteDependenciesError(const CString& sIncomplete)
{
	if (GetSelectedCount() == 1)
	{
		int nRet = AfxMessageBox(IDS_TDC_SELTASKHASDEPENDENCY, MB_YESNOCANCEL | MB_ICONEXCLAMATION);
		
		if (nRet == IDYES)
			ShowTaskLink(0, sIncomplete);
	}
	else
		AfxMessageBox(IDS_TDC_SELTASKSHAVEDEPENDENCIES, MB_OK | MB_ICONEXCLAMATION);

}

void CToDoCtrl::ShowTaskHasCircularDependenciesError(const CDWordArray& aTaskIDs) const
{
	if (GetSelectedCount() == 1)
		AfxMessageBox(IDS_TDC_SELTASKHASCIRCULARDEPENDENCY, MB_OK | MB_ICONEXCLAMATION);
	else
	{
		CEnString sMessage(CEnString(IDS_TDC_SELTASKSHAVECIRCULARDEPENDENCIES), Misc::FormatArray(aTaskIDs));
		AfxMessageBox(sMessage, MB_OK | MB_ICONEXCLAMATION);
	}
}

// external version
BOOL CToDoCtrl::SetSelectedTaskDate(TDC_DATE nDate, const COleDateTime& date)
{
	// some special cases:
	switch (nDate)
	{
	case TDCD_START:
		if (SetSelectedTaskDate(TDCD_STARTDATE, date, FALSE))
		{
			SetSelectedTaskDate(TDCD_STARTTIME, date, FALSE);
			return TRUE;
		}
		break;

	case TDCD_DUE:
		if (SetSelectedTaskDate(TDCD_DUEDATE, date, FALSE))
		{
			SetSelectedTaskDate(TDCD_DUETIME, date, FALSE);
			return TRUE;
		}
		break;

	case TDCD_DONE:
		if (SetSelectedTaskDate(TDCD_DONEDATE, date, FALSE))
		{
			SetSelectedTaskDate(TDCD_DONETIME, date, FALSE);
			return TRUE;
		}
		break;
	}

	// all else
	return SetSelectedTaskDate(nDate, date, FALSE);
}

// internal version
BOOL CToDoCtrl::SetSelectedTaskDate(TDC_DATE nDate, const COleDateTime& date, BOOL bDateEdited)
{
	ASSERT (nDate != TDCD_DUE && nDate != TDCD_START); // for getting only

	if (nDate == TDCD_DUE || nDate == TDCD_START)
		return FALSE;

	// special case
	if (nDate == TDCD_DONE)
		return SetSelectedTaskDone(date, bDateEdited);

	if (IsReadOnly())
		return FALSE;

	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;

	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		// due, start, creation
		int nItemRes = m_data.SetTaskDate(dwTaskID, nDate, date);

		// post-processing
		if (nItemRes == SET_CHANGE)
		{
			if (nDate == TDCD_DUEDATE)
			{
				ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_DUEDATE);
				m_eRecurrence.SetDefaultDate(date);
			}
			else if (nDate == TDCD_STARTDATE)
				ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_STARTDATE);

			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;

			FixupTaskDependencyDates(dwTaskID, nDate);
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		switch (nDate)
		{
		case TDCD_CREATE:	
			SetModified(TRUE, TDCA_CREATIONDATE, dwModTaskID); 
			break;

		case TDCD_STARTDATE:
			if (bDateEdited)
				m_timeStart.SetOleTime(date);
			// fall thru

		case TDCD_STARTTIME:
			SetModified(TRUE, TDCA_STARTDATE, dwModTaskID); 
			break;
		
		case TDCD_DUEDATE:	
			if (bDateEdited && date.m_dt == 0.0)
				m_timeDue.SetOleTime(date);
			// fall thru

		case TDCD_DUETIME:
			SetModified(TRUE, TDCA_DUEDATE, dwModTaskID); 
			break;

		case TDCD_DONETIME:	
			SetModified(TRUE, TDCA_DONEDATE, dwModTaskID); 
			break;

		//case TDCD_DONE:
		default:
			ASSERT(0);
			return FALSE;
		}

		// only update controls if the date was changed implicitly
		if (!bDateEdited)
			UpdateControls();
		

	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::OffsetSelectedTaskDate(TDC_DATE nDate, int nAmount, int nUnits, BOOL bAndSubtasks)
{
	if (IsReadOnly())
		return FALSE;

	Flush();

	CHTIList htiSel;
	Selection().CopySelection(htiSel);

	// remove children from tree if bAndSubtasks specified
	// else we might offset them twice
	if (bAndSubtasks)
		CTreeSelectionHelper::RemoveChildDuplicates(htiSel, m_tree);

	POSITION pos = htiSel.GetHeadPosition();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		HTREEITEM hti = htiSel.GetNext(pos);
		DWORD dwTaskID = m_find.GetTaskID(hti);

		int nItemRes = m_data.OffsetTaskDate(dwTaskID, nDate, nAmount, nUnits, bAndSubtasks, FALSE);
		
		if (nItemRes == SET_CHANGE)
		{
			FixupTaskDependencyDates(dwTaskID, nDate);
			nRes = SET_CHANGE;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		switch (nDate)
		{
		case TDCD_CREATE:	
			SetModified(TRUE, TDCA_CREATIONDATE, dwModTaskID); 
			break;

		case TDCD_START:	
			SetModified(TRUE, TDCA_STARTDATE, dwModTaskID); 
			break;

		case TDCD_DUE:		
			SetModified(TRUE, TDCA_DUEDATE, dwModTaskID); 
			break;

		case TDCD_DONE:		
			SetModified(TRUE, TDCA_DONEDATE, dwModTaskID); 
			break;

		default:
			ASSERT(0);
			return FALSE;
		}

		UpdateControls();
	}

	return (nRes != SET_FAILED);
}

void CToDoCtrl::SetInheritedParentAttributes(const CTDCAttributeArray& aAttribs, BOOL bUpdateAttrib)
{
	s_aParentAttribs.Copy(aAttribs);
	s_bUpdateInheritAttrib = bUpdateAttrib;
}

COLORREF CToDoCtrl::GetSelectedTaskColor() const
{
	return m_data.GetTaskColor(GetSelectedTaskID());
}

BOOL CToDoCtrl::SelectionHasIncompleteSubtasks(BOOL bExcludeRecurring) const
{
	POSITION pos = Selection().GetFirstItemPos();

	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		
		if (m_data.TaskHasIncompleteSubtasks(dwTaskID, bExcludeRecurring))
			return TRUE;
	}

	return FALSE;
}

int CToDoCtrl::CheckWantSubtasksCompleted()
{
	if (SelectionHasIncompleteSubtasks(TRUE))
	{
		// ask the user what to do
		UINT nIDMessage = (GetSelectedCount() == 1) ? 
							IDS_TDC_SELTASKHASINCOMPLETE : IDS_TDC_TASKHASINCOMPLETE;
		
		int nRet = AfxMessageBox(nIDMessage, MB_YESNOCANCEL | MB_ICONQUESTION);

		if (nRet == IDYES)
			return 1;

		else if (nRet == IDCANCEL)
			return -1;
	}

	return 0; // no incomplete subtasks or IDNO
}

BOOL CToDoCtrl::SetSelectedTaskDone(BOOL bDone)
{
	COleDateTime date;

	if (bDone)
		date = COleDateTime::GetCurrentTime();

	return SetSelectedTaskDone(date, FALSE);
}

BOOL CToDoCtrl::SetSelectedTaskDone(const COleDateTime& date, BOOL bDateEdited)
{
	if (IsReadOnly())
		return FALSE;

	Flush();

	// check for various flies in the ointment but only
	// if we are setting the completed status to done,
	// else we're just changing the date
	BOOL bDone = (date.m_dt > 0);
	TDC_SETTASKDONE nSetChildren = TDC_SETLEAVE;

	if (bDone && m_selection.HasIncompleteTasks())
	{
		// check for circular dependencies
		CDWordArray aTaskIDs;
		
		if (SelectionHasCircularDependencies(aTaskIDs))
		{
			// reset ctrl date
			SetCtrlDate(m_dateDone, 0.0);
			
			// show error msg
			ShowTaskHasCircularDependenciesError(aTaskIDs);
			
			return FALSE;
		}

		// check for incomplete dependents
		CString sIncomplete;

		if (SelectionHasIncompleteDependencies(sIncomplete))
		{
			ShowTaskHasIncompleteDependenciesError(sIncomplete);
			return FALSE;
		}
		
		// check for incomplete subtasks
		int nRet = CheckWantSubtasksCompleted();

		if (nRet == -1)
			return FALSE; // cancel

		// else 
		nSetChildren = (nRet ? TDC_SETDONE : TDC_SETLEAVE);
	}
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	BOOL bSomeRecurred = FALSE, bSomeDone = FALSE;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		HTREEITEM hti = Selection().GetNextItem(pos);

		DWORD dwTaskID = GetTaskID(hti);
		BOOL bWasDone = m_data.IsTaskDone(dwTaskID);

		// Handle recurring tasks, but only if changing state to completed
		COleDateTime dtNext;
		BOOL bRecurring = FALSE, bReuseTask = FALSE, bDueDate = TRUE;
		COleDateTime dtDone(date);
		
		if (bDone && !bWasDone)
		{
			TDIRECURRENCE tr;

			bRecurring = m_data.GetTaskRecurrence(dwTaskID, tr) &&
						 m_data.GetTaskNextOccurrence(dwTaskID, dtNext, bDueDate);

			if (bRecurring)
			{
				if (tr.nReuse == TDIRO_REUSE)
				{
					bReuseTask = TRUE;
				}
				else if (tr.nReuse == TDIRO_CREATE)
				{
					bReuseTask = FALSE;
				}
				else // ask
				{
					CTDLReuseRecurringTaskDlg dialog;

					if (dialog.DoModal() == IDCANCEL)
						return FALSE;

					bReuseTask = dialog.GetWantReuseTask();
				}

				if (bReuseTask)
				{
					dtDone = 0.0;
					nSetChildren = TDC_SETUNDONE;
					bSomeRecurred = TRUE;
				}
				else
				{
					bSomeDone = TRUE;
				}
			}
			else
				bSomeDone = TRUE;
		}
		else
			bSomeDone = TRUE;

		int nItemRes = SetTaskDone(hti, dtDone, nSetChildren);

		if (nItemRes == SET_CHANGE || (bRecurring && bDone && !bWasDone))
		{
			if (bRecurring)
			{
				if (bReuseTask)
				{
					// we need to move both the due date and the start date forward
					// so we first cache the old dates
					COleDateTime dtStart = m_data.GetTaskDate(dwTaskID, TDCD_START);
					COleDateTime dtDue = m_data.GetTaskDate(dwTaskID, TDCD_DUE);

					// remember to propagate changes to children
					if (bDueDate) // dtNext is the new due date
					{
						m_data.SetTaskDate(dwTaskID, TDCD_DUE, dtNext);
						ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_DUEDATE);

						// bump start date 
						if (dtStart.m_dt > 0 && dtDue.m_dt > 0)
						{
							dtStart += (dtNext - dtDue);
							m_data.SetTaskDate(dwTaskID, TDCD_START, dtStart);
							ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_STARTDATE);
						}

						dtDue = dtNext;
					}
					else // dtNext is the new start date
					{
						m_data.SetTaskDate(dwTaskID, TDCD_START, dtNext);
						ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_STARTDATE);

						// bump due date 
						if (dtStart.m_dt > 0 && dtDue.m_dt > 0)
						{
							dtDue += (dtNext - dtStart);
							m_data.SetTaskDate(dwTaskID, TDCD_DUE, dtDue);
							ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_DUEDATE);
						}

						dtStart = dtNext;
					}

					// update date controls too if there's only one selected item
					if (GetSelectedCount() == 1)
					{
						SetCtrlDate(m_dateDue, dtDue);
						SetCtrlDate(m_dateStart, dtStart);
					}
					
					// reset time spent
					m_data.ResetTaskTimeSpent(dwTaskID, TRUE); // TRUE == and children				

					// notify parent
					GetParent()->SendMessage(WM_TDCN_RECREATERECURRINGTASK, dwTaskID, dwTaskID);
				}
				else // copy task below existing
				{
					PostMessage(WM_TDC_RECREATERECURRINGTASK, dwTaskID);
				}
			}

			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		// only update controls if the date was changed implicitly
		if (!bDateEdited)
			UpdateControls();
		else
			m_timeDone.SetOleTime(date);

		m_tree.UpdateWindow();

		// clear date control
		if (!bDone)
			SetCtrlDate(m_dateDone, 0.0);
		
		// if some tasks have recurred then we also need to report 
		// that the due date has changed also
		if (bSomeRecurred)
			SetModified(TRUE, TDCA_DUEDATE, dwModTaskID);

		if (bSomeDone || bSomeRecurred)
			SetModified(TRUE, TDCA_DONEDATE, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

LRESULT CToDoCtrl::OnRecreateRecurringTask(WPARAM wParam, LPARAM /*lParam*/)
{
	DWORD dwTaskID = wParam;
	HTREEITEM hti = m_find.GetItem(dwTaskID);
	ASSERT(hti);

	// take copy of task
	CTaskFile task;
	AddTreeItemToTaskFile(hti, task, NULL, TDCGT_ALL);
	
	// insert below existing
	IMPLEMENT_UNDO(TDCUAT_ADD);

	HTREEITEM htiParent = m_tree.GetParentItem(hti);
	AddTasksToTree(task, htiParent, hti, TDCR_YES);

	// reset new task(s) state to 'undone' including children
	hti = Selection().GetFirstItem();
	ASSERT(hti);

	SetTaskDone(hti, 0.0, TDC_SETUNDONE);

	// update due and start dates
	COleDateTime dtNext;
	BOOL bDueDate = TRUE;

	VERIFY(m_data.GetTaskNextOccurrence(dwTaskID, dtNext, bDueDate));

	DWORD dwNewTaskID = GetTaskID(hti);

	// we need to move both the due date and the start date forward
	// same as we did when reusing the task
	COleDateTime dtStart = m_data.GetTaskDate(dwTaskID, TDCD_START);
	COleDateTime dtDue = m_data.GetTaskDate(dwTaskID, TDCD_DUE);
	
	if (bDueDate) // dtNext is the new due date
	{
		m_data.SetTaskDate(dwNewTaskID, TDCD_DUE, dtNext);
		ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_DUEDATE);

		// bump start date 
		if (dtStart.m_dt > 0 && dtDue.m_dt > 0)
		{
			dtStart += (dtNext - dtDue);
			m_data.SetTaskDate(dwNewTaskID, TDCD_START, dtStart);
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_STARTDATE);
		}
		
		dtDue = dtNext;
	}
	else // dtNext is the new start date
	{
		m_data.SetTaskDate(dwNewTaskID, TDCD_START, dtNext);
		ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_STARTDATE);
		
		// bump due date 
		if (dtStart.m_dt > 0 && dtDue.m_dt > 0)
		{
			dtDue += (dtNext - dtStart);
			m_data.SetTaskDate(dwNewTaskID, TDCD_DUE, dtDue);
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_DUEDATE);
		}
		
		dtStart = dtNext;
	}
	
	// reset time spent
	m_data.ResetTaskTimeSpent(dwNewTaskID, TRUE); // TRUE == and children

	// mark as changed
	SetModified(TRUE, TDCA_COPY, dwNewTaskID);

	// notify parent
	GetParent()->SendMessage(WM_TDCN_RECREATERECURRINGTASK, dwTaskID, dwNewTaskID);

	return 0L;
}

int CToDoCtrl::SetTaskDone(HTREEITEM hti, const COleDateTime& date, TDC_SETTASKDONE nSetChildren)
{
	DWORD dwTaskID = GetTaskID(hti);
	int nRes = 0;
	BOOL bDone = (date.m_dt > 0);
	BOOL bWasDone = m_data.IsTaskDone(dwTaskID);

	if (m_data.SetTaskDate(dwTaskID, TDCD_DONE, date) == SET_CHANGE)
		nRes = SET_CHANGE;
		
	m_tree.TCH().SetItemChecked(hti, bDone);
		
	if (m_dwTimeTrackTaskID == dwTaskID)
	{
		EndTimeTracking();
		UpdateControls();
	}

	if (nSetChildren != TDC_SETLEAVE)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			if (SetTaskDone(htiChild, date, nSetChildren) == SET_CHANGE)
				nRes = SET_CHANGE;
			
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
	
	// update any dependents dates
	FixupTaskDependencyDates(dwTaskID, TDCD_DONE);

	return nRes;
}

BOOL CToDoCtrl::IsSelectedTaskDone() const
{
	return m_data.IsTaskDone(GetSelectedTaskID());
}

BOOL CToDoCtrl::IsSelectedTaskDue() const
{
	return m_data.IsTaskDue(GetSelectedTaskID());
}

BOOL CToDoCtrl::SetSelectedTaskPercentDone(int nPercent)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskPercent(dwTaskID, nPercent);
		
		if (nItemRes == SET_CHANGE)
		{
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		if (m_nPercentDone != nPercent)
		{
			m_nPercentDone = nPercent;
			UpdateDataEx(this, IDC_PERCENT, m_nPercentDone, FALSE);
		}
		
		SetModified(TRUE, TDCA_PERCENT, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::SetSelectedTaskCost(double dCost)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskCost(dwTaskID, dCost);
			
		if (nItemRes == SET_CHANGE)
		{
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		if (m_dCost != dCost)
		{
			m_dCost = dCost;
			UpdateDataEx(this, IDC_COST, m_dCost, FALSE);
		}
		
		SetModified(TRUE, TDCA_COST, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::SetSelectedTaskRecurrence(const TDIRECURRENCE& tr)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskRecurrence(dwTaskID, tr);
			
		if (nItemRes == SET_CHANGE)
		{
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		if (m_tRecurrence != tr)
		{
			m_tRecurrence = tr;
			m_eRecurrence.SetRecurrenceOptions(tr);
		}
		
		SetModified(TRUE, TDCA_RECURRENCE, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::GetSelectedTaskRecurrence(TDIRECURRENCE& tr) const
{
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		m_data.GetTaskRecurrence(dwTaskID, tr);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			
			TDIRECURRENCE trTask;
			m_data.GetTaskRecurrence(dwTaskID, trTask);
			
			if (tr != trTask)
				tr = TDIRECURRENCE();
		}
	}
	
	return (tr.nRegularity != TDIR_ONCE);
}

void CToDoCtrl::SetPercentDoneIncrement(int nAmount)
{
	m_nPercentIncrement = abs(nAmount);
	m_nPercentIncrement = max(1, min(50, m_nPercentIncrement));

	if (m_spinPercent.GetSafeHwnd())
	{
		UDACCEL uda = { 0, m_nPercentIncrement };
		m_spinPercent.SetAccel(1, &uda);
	}
}

int CToDoCtrl::GetNextPercentDone(int nPercent, BOOL bUp)
{
	int nIncrement = (bUp ? m_nPercentIncrement : -m_nPercentIncrement);

	// we need to replicate the arithmetic performed by the 
	// spin button control, so that to the user the result
	// is the same as clicking the spin buttons
	if (m_nPercentIncrement > 1)
	{
		// bump the % to the next upper (if +ve) or
		// next lower (if -ve) whole increment
		if (nPercent % m_nPercentIncrement)
		{
			if (bUp)
				nPercent = ((nPercent / m_nPercentIncrement) + 1) * m_nPercentIncrement;
			else
				nPercent = (nPercent / m_nPercentIncrement) * m_nPercentIncrement;
		}
	}

	return (nPercent + nIncrement);
}

BOOL CToDoCtrl::IncrementSelectedTaskPercentDone(BOOL bUp)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	while (pos)
	{
		HTREEITEM hti = Selection().GetNextItem(pos);
		DWORD dwTaskID = GetTaskID(hti);

		BOOL bDone = m_data.IsTaskDone(dwTaskID);

		int nPercent = m_data.GetTaskPercent(dwTaskID, TRUE);
		nPercent = GetNextPercentDone(nPercent, bUp);
				
		// need to handle transition to/from 100% as special case
		if (bDone && nPercent < 100)
		{
			m_data.SetTaskDate(dwTaskID, TDCD_DONE, 0.0);
			m_tree.TCH().SetItemChecked(hti, FALSE);
		}
		else if (!bDone && nPercent >= 100)
		{
			m_data.SetTaskDate(dwTaskID, TDCD_DONE, COleDateTime::GetCurrentTime());
			m_tree.TCH().SetItemChecked(hti, TRUE);
		}

		int nItemRes = m_data.SetTaskPercent(dwTaskID, nPercent);
		
		if (nItemRes == SET_CHANGE)
		{
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		// don't update m_nPercentDone for multiple selection
		// else they all end up as the same value
		if (GetSelectedCount() == 1)
		{
			int nPercent = GetNextPercentDone(m_nPercentDone, bUp);

			if (m_nPercentDone != nPercent)
			{
				m_nPercentDone = nPercent;
				UpdateDataEx(this, IDC_PERCENT, m_nPercentDone, FALSE);
			}
		}
		
		SetModified(TRUE, TDCA_PERCENT, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

void CToDoCtrl::ApplyLastInheritedChangeToSubtasks(DWORD dwTaskID, TDC_ATTRIBUTE nAttrib)
{
	// special case: 
	if (nAttrib == TDCA_ALL)
	{
		for (int nAtt = TDCA_FIRSTATTRIBUTE; nAtt < TDCA_ATTRIBUTECOUNT; nAtt++)
		{
			if (WantUpdateInheritedAttibute((TDC_ATTRIBUTE)nAtt))
			{
				// FALSE means do not apply if parent is blank
				m_data.ApplyLastChangeToSubtasks(dwTaskID, (TDC_ATTRIBUTE)nAtt, FALSE);
			}
		}
	}
	else if (WantUpdateInheritedAttibute(nAttrib))
	{
		m_data.ApplyLastChangeToSubtasks(dwTaskID, nAttrib);
	}
}

BOOL CToDoCtrl::SetSelectedTaskTimeEstimate(double dTime, TCHAR nUnits)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		HTREEITEM hti = Selection().GetNextItem(pos);

		// ignore parent tasks
		if (!m_tree.ItemHasChildren(hti) || HasStyle(TDCS_ALLOWPARENTTIMETRACKING))
		{
			DWORD dwTaskID = GetTaskID(hti);
			int nItemRes = m_data.SetTaskTimeEstimate(dwTaskID, dTime, nUnits);
			
			if (nItemRes == SET_CHANGE)
			{
				ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_STATUS);
				nRes = SET_CHANGE;
				dwModTaskID = dwTaskID;
			}
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		if (m_dTimeEstimate != dTime || m_nTimeEstUnits != nUnits)
		{
			// note: setting the time field changes m_nTimeEstUnits
			// so we have to do them separately
			m_dTimeEstimate = dTime;
			UpdateDataEx(this, IDC_TIMEEST, m_dTimeEstimate, FALSE);

			m_nTimeEstUnits = nUnits;
			m_eTimeEstimate.SetUnits(m_nTimeEstUnits);
		}
		
		// update % complete?
		if (HasStyle(TDCS_AUTOCALCPERCENTDONE) && GetSelectedCount() == 1)
		{
			m_nPercentDone = m_data.CalcPercentDone(GetSelectedTaskID());		
			UpdateDataEx(this, IDC_PERCENT, m_nPercentDone, FALSE);
		}
		
		SetModified(TRUE, TDCA_TIMEEST, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::SetSelectedTaskTimeSpent(double dTime, TCHAR nUnits)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		HTREEITEM hti = Selection().GetNextItem(pos);

		// ignore parent tasks
		if (!m_tree.ItemHasChildren(hti) || HasStyle(TDCS_ALLOWPARENTTIMETRACKING))
		{
			DWORD dwTaskID = GetTaskID(hti);
			int nItemRes = m_data.SetTaskTimeSpent(dwTaskID, dTime, nUnits);
			
			if (nItemRes == SET_CHANGE)
			{
				nRes = SET_CHANGE;
				dwModTaskID = dwTaskID;
			}
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		if (m_dTimeSpent != dTime || m_nTimeSpentUnits != nUnits)
		{
			m_dTimeSpent = dTime;
			m_nTimeSpentUnits = nUnits;
			
			UpdateDataEx(this, IDC_TIMESPENT, m_dTimeSpent, FALSE);
			m_eTimeSpent.SetUnits(m_nTimeSpentUnits);
		}
		
		// update % complete?
		if (HasStyle(TDCS_AUTOCALCPERCENTDONE) && GetSelectedCount() == 1)
		{
			m_nPercentDone = m_data.CalcPercentDone(GetSelectedTaskID());		
			UpdateDataEx(this, IDC_PERCENT, m_nPercentDone, FALSE);
		}
		
		SetModified(TRUE, TDCA_TIMESPENT, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::SetSelectedTaskTimeEstimateUnits(TCHAR nUnits, BOOL bRecalcTime)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		HTREEITEM hti = Selection().GetNextItem(pos);

		// ignore parent tasks
		if (!m_tree.ItemHasChildren(hti) || HasStyle(TDCS_ALLOWPARENTTIMETRACKING))
		{
			DWORD dwTaskID = GetTaskID(hti);

			TCHAR nCurUnits;
			double dCurTime = m_data.GetTaskTimeEstimate(dwTaskID, nCurUnits);

			if (nCurUnits != nUnits)
			{
				double dTime = dCurTime;

				if (bRecalcTime)
					dTime = CTimeHelper().GetTime(dTime, nCurUnits, nUnits);

				int nItemRes = m_data.SetTaskTimeEstimate(dwTaskID, dTime, nUnits);
				
				if (nItemRes == SET_CHANGE)
				{
					nRes = SET_CHANGE;
					dwModTaskID = dwTaskID;
				}
			}
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		if (m_nTimeEstUnits != nUnits)
		{
			m_nTimeEstUnits = nUnits;
			m_eTimeEstimate.SetUnits(m_nTimeEstUnits);
		}

		// update controls if only one item selected
		if (GetSelectedCount() == 1)
		{
			if (bRecalcTime)
			{
				m_dTimeEstimate = GetSelectedTaskTimeEstimate(nUnits);
				UpdateDataEx(this, IDC_TIMEEST, m_dTimeEstimate, FALSE);
			}
			// update % complete?
			else if (HasStyle(TDCS_AUTOCALCPERCENTDONE))
			{
				m_nPercentDone = m_data.CalcPercentDone(GetSelectedTaskID());		
				UpdateDataEx(this, IDC_PERCENT, m_nPercentDone, FALSE);
			}
		}
		
		SetModified(TRUE, TDCA_TIMEEST, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

BOOL CToDoCtrl::SetSelectedTaskTimeSpentUnits(TCHAR nUnits, BOOL bRecalcTime)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		HTREEITEM hti = Selection().GetNextItem(pos);

		// ignore parent tasks
		if (!m_tree.ItemHasChildren(hti) || HasStyle(TDCS_ALLOWPARENTTIMETRACKING))
		{
			DWORD dwTaskID = GetTaskID(hti);

			TCHAR nCurUnits;
			double dCurTime = m_data.GetTaskTimeSpent(dwTaskID, nCurUnits);

			if (nCurUnits != nUnits)
			{
				double dTime = dCurTime;

				if (bRecalcTime)
					dTime = CTimeHelper().GetTime(dTime, nCurUnits, nUnits);

				int nItemRes = m_data.SetTaskTimeSpent(dwTaskID, dTime, nUnits);
				
				if (nItemRes == SET_CHANGE)
				{
					nRes = SET_CHANGE;
					dwModTaskID = dwTaskID;
				}
			}
		}
	}
	
	// update UI
	if (nRes == SET_CHANGE)
	{
		if (m_nTimeSpentUnits != nUnits)
		{
			m_nTimeSpentUnits = nUnits;
			m_eTimeSpent.SetUnits(m_nTimeSpentUnits);
		}

		// update controls if only one item selected
		if (GetSelectedCount() == 1)
		{
			if (bRecalcTime)
			{
				m_dTimeSpent = GetSelectedTaskTimeSpent(nUnits);
				UpdateDataEx(this, IDC_TIMESPENT, m_dTimeSpent, FALSE);
			}
			// update % complete?
			else if (HasStyle(TDCS_AUTOCALCPERCENTDONE))
			{
				m_nPercentDone = m_data.CalcPercentDone(GetSelectedTaskID());		
				UpdateDataEx(this, IDC_PERCENT, m_nPercentDone, FALSE);
			}
		}
		
		SetModified(TRUE, TDCA_TIMESPENT, dwModTaskID);
	}
	
	return (nRes != SET_FAILED);
}

int CToDoCtrl::GetSelectedTaskPercent() const
{
	int nPercent = 0;
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		nPercent = m_data.GetTaskPercent(dwTaskID, FALSE);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			int nTaskPercent = m_data.GetTaskPercent(dwTaskID, FALSE);
			
			if (nPercent != nTaskPercent)
				return -1;
		}
	}
	
	return nPercent;
}

CString CToDoCtrl::GetSelectedTaskPath(BOOL bIncludeTaskName, int nMaxLen) const
{
	CString sPath;

	if (GetSelectedCount() == 1)
	{
		DWORD dwTaskID = GetSelectedTaskID();
		CString sTaskTitle = bIncludeTaskName ? m_data.GetTaskTitle(dwTaskID) : _T("");

		if (bIncludeTaskName && nMaxLen != -1)
			nMaxLen -= sTaskTitle.GetLength();

		sPath = GetTaskPath(dwTaskID, nMaxLen);
	
		if (bIncludeTaskName)
			sPath += sTaskTitle;
	}

	return sPath;
}

double CToDoCtrl::GetSelectedTaskCost() const
{
	double dCost = 0.0;
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		dCost = m_data.GetTaskCost(dwTaskID);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			double dTaskCost = m_data.GetTaskCost(dwTaskID);

			if (dCost != dTaskCost)
				return 0.0;
		}
	}
	
	return dCost;
}

CString CToDoCtrl::GetSelectedTaskCustomAttributeData(const CString& sAttribID) const
{
	CString sData;
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		sData = m_data.GetTaskCustomAttributeData(dwTaskID, sAttribID);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			CString sTaskData = m_data.GetTaskCustomAttributeData(dwTaskID, sAttribID);

			if (sData != sTaskData)
				return _T("");
		}
	}
	
	return sData;
}

int CToDoCtrl::GetSelectedTaskCustomAttributeData(CMapStringToString& mapData) const
{
	mapData.RemoveAll();

	int nCtrl = m_aCustomControls.GetSize();

	while (nCtrl--)
	{
		const CUSTOMATTRIBCTRLITEM& ctrl = m_aCustomControls.GetData()[nCtrl];
		mapData[ctrl.sAttribID] = GetSelectedTaskCustomAttributeData(ctrl.sAttribID);
	}

	return mapData.GetCount();
}

double CToDoCtrl::CalcSelectedTaskCost() const
{
	double dCost = 0.0;
	
	CHTIList selection;
	Selection().CopySelection(selection);
	CTreeSelectionHelper::RemoveChildDuplicates(selection, m_tree);

	POSITION pos = selection.GetHeadPosition();
	
	while (pos)
	{
		HTREEITEM hti = selection.GetNext(pos);
		DWORD dwTaskID = GetTaskID(hti);

		dCost += m_data.CalcCost(dwTaskID);
	}
	
	return dCost;
}

BOOL CToDoCtrl::SetTextChange(int nChange, CString& sItem, const CString& sNewItem, TDC_ATTRIBUTE nAttrib, 
							  UINT nIDC, DWORD dwTaskID, CAutoComboBox* pCombo)
{
	ASSERT(!IsReadOnly());
	
	if (nChange == SET_CHANGE)
	{
		if (sItem != sNewItem)
		{
			// make sure it exists
			if (pCombo)
				pCombo->AddUniqueItem(sNewItem);

			sItem = sNewItem;
			UpdateDataEx(this, nIDC, sItem, FALSE);
		}
		
		SetModified(TRUE, nAttrib, dwTaskID);
	}
	
	return (nChange != SET_FAILED);
}

BOOL CToDoCtrl::SetSelectedTaskAllocTo(const CStringArray& aAllocTo)
{
	if (IsReadOnly())
		return FALSE;

	Flush();

	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskAllocTo(dwTaskID, aAllocTo);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_ALLOCTO);
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		m_cbAllocTo.SetChecked(aAllocTo);
		SetModified(TRUE, TDCA_ALLOCTO, dwModTaskID);
	}
	
	return TRUE;
}

BOOL CToDoCtrl::SetSelectedTaskAllocBy(const CString& sAllocBy)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskAllocBy(dwTaskID, sAllocBy);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_ALLOCBY);
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	return SetTextChange(nRes, m_sAllocBy, sAllocBy, TDCA_ALLOCBY, IDC_ALLOCBY, dwModTaskID, &m_cbAllocBy);
}

BOOL CToDoCtrl::SetSelectedTaskVersion(const CString& sVersion)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskVersion(dwTaskID, sVersion);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_VERSION);
			nRes = SET_CHANGE;
		}
	}
	
	return SetTextChange(nRes, m_sVersion, sVersion, TDCA_VERSION, IDC_VERSION, dwModTaskID, &m_cbVersion);
}

BOOL CToDoCtrl::SetSelectedTaskStatus(const CString& sStatus)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskStatus(dwTaskID, sStatus);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_STATUS);
			nRes = SET_CHANGE;
		}
	}

	return SetTextChange(nRes, m_sStatus, sStatus, TDCA_STATUS, IDC_STATUS, dwModTaskID, &m_cbStatus);
}

BOOL CToDoCtrl::SetSelectedTaskCategories(const CStringArray& aCats)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskCategories(dwTaskID, aCats);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_CATEGORY);
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		m_cbCategory.SetChecked(aCats);
		SetModified(TRUE, TDCA_CATEGORY, dwModTaskID);
	}

	return TRUE;
}

BOOL CToDoCtrl::SetSelectedTaskTags(const CStringArray& aTags)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes = m_data.SetTaskTags(dwTaskID, aTags);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_TAGS);
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	if (nRes == SET_CHANGE)
	{
		m_cbTags.SetChecked(aTags);
		SetModified(TRUE, TDCA_TAGS, dwModTaskID);
	}

	return TRUE;
}

BOOL CToDoCtrl::TimeTrackSelectedTask()
{
	if (!CanTimeTrackSelectedTask())
		return FALSE;
	
	TimeTrackTask(GetSelectedItem());
	
	return FALSE;
}

BOOL CToDoCtrl::CanTimeTrackSelectedTask() const
{
	if (IsReadOnly() || GetSelectedCount() != 1)
		return FALSE;
	
	return (m_data.IsTaskTimeTrackable(GetSelectedTaskID()));
}

BOOL CToDoCtrl::IsSelectedTaskBeingTimeTracked() const
{
	DWORD dwTaskID = GetSelectedTaskID();
	
	return (m_data.IsTaskTimeTrackable(dwTaskID) && m_dwTimeTrackTaskID == dwTaskID);
}

BOOL CToDoCtrl::IsActivelyTimeTracking() const
{
	if (!m_dwTimeTrackTaskID || m_bTimeTrackingPaused)
		return FALSE;

	DWORD dwSelTaskID = GetSelectedTaskID();
	BOOL bTrackingSelTask = (dwSelTaskID == m_dwTimeTrackTaskID);

	if (!bTrackingSelTask && HasStyle(TDCS_TRACKSELECTEDTASKONLY))
		return FALSE;
	
	// does it permit tracking
	DWORD dwTrackID = bTrackingSelTask ? dwSelTaskID : m_dwTimeTrackTaskID;

	if (!m_data.IsTaskTimeTrackable(dwTrackID))
		return FALSE;

	if (HasStyle(TDCS_PAUSETIMETRACKINGONSCRNSAVER))
	{
		if (Misc::IsScreenSaverActive() || Misc::IsWorkStationLocked())
			return FALSE;
	}

	// yeah!
	return TRUE;
}

int CToDoCtrl::GetSelectedTaskAllocTo(CStringArray& aAllocTo) const
{
	aAllocTo.RemoveAll();

	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		
		m_data.GetTaskAllocTo(dwTaskID, aAllocTo);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);

			CStringArray aTaskAllocTo;
			m_data.GetTaskAllocTo(dwTaskID, aTaskAllocTo);
			
			if (!Misc::MatchAll(aAllocTo, aTaskAllocTo))
			{
				aAllocTo.RemoveAll();
				break;
			}
		}
	}
	
	return aAllocTo.GetSize();
}

CString CToDoCtrl::GetSelectedTaskAllocBy() const
{
	CString sAllocBy;
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		sAllocBy = m_data.GetTaskAllocBy(dwTaskID);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			CString sTaskAllocBy = m_data.GetTaskAllocBy(dwTaskID);
			
			if (sAllocBy != sTaskAllocBy)
				return _T("");
		}
	}
	
	return sAllocBy;
}

CString CToDoCtrl::GetSelectedTaskVersion() const
{
	CString sVersion;
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		sVersion = m_data.GetTaskVersion(dwTaskID);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			CString sTaskVersion = m_data.GetTaskVersion(dwTaskID);
			
			if (sVersion != sTaskVersion)
				return _T("");
		}
	}
	
	return sVersion;
}

CString CToDoCtrl::GetSelectedTaskStatus() const
{
	CString sStatus;
	
	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		sStatus = m_data.GetTaskStatus(dwTaskID);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);
			CString sTaskStatus = m_data.GetTaskStatus(dwTaskID);
			
			if (sStatus != sTaskStatus)
				return _T("");
		}
	}
	
	return sStatus;
}

int CToDoCtrl::GetSelectedTaskCategories(CStringArray& aCats) const
{
	aCats.RemoveAll();

	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		
		m_data.GetTaskCategories(dwTaskID, aCats);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);

			CStringArray aTaskCats;
			m_data.GetTaskCategories(dwTaskID, aTaskCats);
			
			if (!Misc::MatchAll(aCats, aTaskCats))
			{
				aCats.RemoveAll();
				break;
			}
		}
	}
	
	return aCats.GetSize();
}

int CToDoCtrl::GetSelectedTaskTags(CStringArray& aTags) const
{
	aTags.RemoveAll();

	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		
		m_data.GetTaskTags(dwTaskID, aTags);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);

			CStringArray aTaskTags;
			m_data.GetTaskTags(dwTaskID, aTaskTags);
			
			if (!Misc::MatchAll(aTags, aTaskTags))
			{
				aTags.RemoveAll();
				break;
			}
		}
	}
	
	return aTags.GetSize();
}

int CToDoCtrl::GetSelectedTaskDependencies(CStringArray& aDepends) const
{
	aDepends.RemoveAll();

	if (GetSelectedCount())
	{
		// get first item's value as initial
		POSITION pos = Selection().GetFirstItemPos();
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		
		m_data.GetTaskDependencies(dwTaskID, aDepends);
		
		while (pos)
		{
			dwTaskID = Selection().GetNextItemData(pos);

			CStringArray aTaskaDepends;
			m_data.GetTaskDependencies(dwTaskID, aTaskaDepends);
			
			if (!Misc::MatchAll(aDepends, aTaskaDepends))
			{
				aDepends.RemoveAll();
				break;
			}
		}
	}
	
	return aDepends.GetSize();
}

BOOL CToDoCtrl::SetSelectedTaskFileRef(const CString& sFilePath)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes =	m_data.SetTaskFileRef(dwTaskID, sFilePath);
		
		if (nItemRes == SET_CHANGE)
		{
			nRes = SET_CHANGE;
			dwModTaskID = dwTaskID;
		}
	}
	
	return SetTextChange(nRes, m_sFileRefPath, sFilePath, TDCA_FILEREF, IDC_FILEPATH, dwModTaskID);
}

BOOL CToDoCtrl::SetSelectedTaskDependencies(const CStringArray& aDepends)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes =	m_data.SetTaskDependencies(dwTaskID, aDepends);
		
		if (nItemRes == SET_CHANGE)
      	{
         	// make sure our start date matches our dependents due date
			if (HasStyle(TDCS_AUTOADJUSTDEPENDENCYDATES))
			{
				for (int nDepends = 0; nDepends < aDepends.GetSize(); nDepends++)
				{
         			DWORD dwIDDepends = _ttoi(aDepends[nDepends]);

					TODOITEM* pTDIDepends = m_data.GetTask(dwIDDepends);
					ASSERT (pTDIDepends);

         			if (pTDIDepends && (pTDIDepends->IsDone() || pTDIDepends->HasDue()))
					{
						TDC_DATE nDate = pTDIDepends->IsDone() ? TDCD_DONE : TDCD_DUE;
						UINT nAdjusted = AdjustTaskDates(dwTaskID, dwIDDepends, nDate);

						// and then fix up our dependents
						if (Misc::HasFlag(nAdjusted, ADJUSTED_DUE))
							FixupTaskDependencyDates(dwTaskID, TDCD_DUE);
					}
				}
			}

			nRes = SET_CHANGE;
 			dwModTaskID = dwTaskID;
     	}
	}
	
	if (nRes == SET_CHANGE)
		SetModified(TRUE, TDCA_DEPENDENCY, dwModTaskID);

	return TRUE;
}

BOOL CToDoCtrl::SetSelectedTaskExtID(const CString& sID)
{
	if (IsReadOnly())
		return FALSE;

	Flush();
	
	POSITION pos = Selection().GetFirstItemPos();
	int nRes = SET_NOCHANGE;
	DWORD dwModTaskID = 0;
	
	IMPLEMENT_UNDOEDIT();
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		int nItemRes =	m_data.SetTaskExtID(dwTaskID, sID);
		
		if (nItemRes == SET_CHANGE)
		{
			ApplyLastInheritedChangeToSubtasks(dwTaskID, TDCA_EXTERNALID);
			nRes = SET_CHANGE;
 			dwModTaskID = dwTaskID;
		}
	}
	
	return SetTextChange(nRes, m_sExternalID, sID, TDCA_EXTERNALID, IDC_EXTERNALID, dwModTaskID);
}

CString CToDoCtrl::GetSelectedTaskFileRef() const
{
	if (GetSelectedCount() == 1)
		return m_data.GetTaskFileRef(GetSelectedTaskID());
	
	// else
	return _T("");
}

CString CToDoCtrl::GetSelectedTaskExtID() const
{
	if (GetSelectedCount() == 1)
		return m_data.GetTaskExtID(GetSelectedTaskID());
	
	// else
	return _T("");
}

BOOL CToDoCtrl::GotoSelectedTaskFileRef()
{
	return GotoFile(GetSelectedTaskFileRef());
}

BOOL CToDoCtrl::GetInsertLocation(TDC_INSERTWHERE nWhere, HTREEITEM& htiDest, HTREEITEM& htiDestAfter) const
{
	HTREEITEM htiSel = GetSelectedItem();
	htiDest = htiDestAfter = NULL;
	
	// handle no selected item
	if (!htiSel)
	{
		switch (nWhere)
		{
		case TDC_INSERTATTOP:
		case TDC_INSERTATTOPOFSELTASKPARENT:
		case TDC_INSERTATTOPOFSELTASK:
		case TDC_INSERTBEFORESELTASK:
			htiDest = TVI_ROOT;
			htiDestAfter = TVI_FIRST;
			break;
			
		case TDC_INSERTATBOTTOM:
		case TDC_INSERTATBOTTOMOFSELTASKPARENT:
		case TDC_INSERTATBOTTOMOFSELTASK:
		case TDC_INSERTAFTERSELTASK:
			htiDest = TVI_ROOT;
			htiDestAfter = TVI_LAST; 
			break;
		}
	}
	else // determine the actual pos to insert
	{
		switch (nWhere)
		{
		case TDC_INSERTATTOP:
			htiDest = TVI_ROOT;
			htiDestAfter = TVI_FIRST;
			break;
			
		case TDC_INSERTATBOTTOM:
			htiDest = TVI_ROOT;
			htiDestAfter = TVI_LAST; 
			break;
			
		case TDC_INSERTATTOPOFSELTASKPARENT:
			htiDest = m_tree.GetParentItem(htiSel);
			htiDestAfter = TVI_FIRST;
			break;
			
		case TDC_INSERTATBOTTOMOFSELTASKPARENT:
			htiDest = m_tree.GetParentItem(htiSel);
			htiDestAfter = TVI_LAST;
			break;
			
		case TDC_INSERTAFTERSELTASK:
			htiDest = m_tree.GetParentItem(htiSel);
			htiDestAfter = htiSel;
			break;
			
		case TDC_INSERTBEFORESELTASK:
			htiDest = m_tree.GetParentItem(htiSel);
			htiDestAfter = m_tree.GetNextItem(htiSel, TVGN_PREVIOUS);
			
			if (!htiDestAfter)
				htiDestAfter = TVI_FIRST;
			break;
			
		case TDC_INSERTATTOPOFSELTASK: // subtask
			htiDest = htiSel;
			htiDestAfter = TVI_FIRST;
			break;
			
		case TDC_INSERTATBOTTOMOFSELTASK: // subtask
			htiDest = htiSel;
			htiDestAfter = TVI_LAST; 
			break;
		}
	}

	return (htiDestAfter != NULL);
}

HTREEITEM CToDoCtrl::CreateNewTask(const CString& sText, TDC_INSERTWHERE nWhere, BOOL bEditText)
{
	if (IsReadOnly())
		return NULL;
	
	if (sText.IsEmpty())
		return NULL;

	Flush();

	// are we an archive and should we warn user if we are
	if (m_bArchive && HasStyle(TDCS_WARNADDDELETEARCHIVE))
	{
		if (MessageBoxEx(this, IDS_TDC_WARNADDTOARCHIVE, IDS_TDC_CONFIRMADD_TITLE, 
						MB_YESNOCANCEL | MB_ICONQUESTION) != IDYES) 
		{
			return NULL;
		}
	}

	// handle special case when tasklist is empty
	if (GetTaskCount() == 0)
		nWhere = TDC_INSERTATBOTTOM;
	
	HTREEITEM htiParent = NULL, htiAfter = NULL;

	if (GetInsertLocation(nWhere, htiParent, htiAfter))
	{
		HTREEITEM htiNew = InsertItem(sText, htiParent, htiAfter, bEditText);

		DWORD dwTaskID = GetTaskID(htiNew);
		ASSERT(dwTaskID == m_dwNextUniqueID - 1);

		return htiNew;
	}

	// else
	ASSERT(0);
	return NULL;
}

BOOL CToDoCtrl::CanCreateNewTask(TDC_INSERTWHERE nInsertWhere) const
{
	if (IsReadOnly())
		return FALSE;

	// can't handle multiple selection
	if (GetSelectedCount() > 1)
		return FALSE;

	HTREEITEM htiParent = NULL, htiAfter = NULL;
	VERIFY (GetInsertLocation(nInsertWhere, htiParent, htiAfter));

	// inserting at root is always okay
	if (htiParent == TVI_ROOT || htiParent == NULL)
		return TRUE;

	switch (nInsertWhere)
	{
	case TDC_INSERTATTOP:
	case TDC_INSERTATBOTTOM:
		return TRUE;

	// parent cannot be reference
	case TDC_INSERTATTOPOFSELTASKPARENT:
	case TDC_INSERTATBOTTOMOFSELTASKPARENT:
	case TDC_INSERTAFTERSELTASK:
	case TDC_INSERTBEFORESELTASK:
	case TDC_INSERTATTOPOFSELTASK: 
	case TDC_INSERTATBOTTOMOFSELTASK:
		return (!m_data.IsTaskReference(GetTaskID(htiParent)));
	}

	ASSERT(0);
	return FALSE;
}

TODOITEM* CToDoCtrl::CreateNewTask(HTREEITEM htiParent)
{
	TODOITEM* pTDINew = m_data.NewTask(&s_tdDefault);

	if (pTDINew)
	{
		// copy over parent attributes
		BOOL bRootParent = (!htiParent || htiParent == TVI_ROOT);
		DWORD dwParentID = bRootParent ? 0 : GetTaskID(htiParent);

		if (!bRootParent && s_aParentAttribs.GetSize())
			m_data.CopyTaskAttributes(pTDINew, dwParentID, s_aParentAttribs);
	}

	return pTDINew;
}

BOOL CToDoCtrl::WantUpdateInheritedAttibute(TDC_ATTRIBUTE nAttrib)
{
	if (s_bUpdateInheritAttrib)
	{
		int nAtt = s_aParentAttribs.GetSize();

		while (nAtt--)
		{
			if (s_aParentAttribs[nAtt] == nAttrib)
				return TRUE;
		}
	}

	// else
	return FALSE;
}

HTREEITEM CToDoCtrl::InsertItem(const CString& sText, HTREEITEM htiParent, HTREEITEM htiAfter, 
								BOOL bEdit)
{
	m_dwLastAddedID = 0;
	
	if (IsReadOnly())
		return NULL;
	
	if (sText.IsEmpty())
		return NULL;
	
	IMPLEMENT_UNDO(TDCUAT_ADD);

	// create the new task item
	TODOITEM* pTDINew = CreateNewTask(htiParent);
	
	// and initialize
	pTDINew->sTitle = sText;

	if (pTDINew->dateStart.m_dt == -1)
		pTDINew->dateStart = floor(COleDateTime::GetCurrentTime());

	// copy parent attribs
    BOOL bRootParent = (!htiParent || htiParent == TVI_ROOT);
	DWORD dwParentID = bRootParent ? 0 : GetTaskID(htiParent);
	DWORD dwAfterID = GetTaskID(htiAfter);

	// handle TVI_FIRST and TVI_LAST
	// Note: we don't modify the htiAfter directly because this is 
	// needed as-is in the call to CTreeCtrl::InsertItem
	if (htiAfter == TVI_FIRST)
		dwAfterID = 0;

	else if (htiAfter == TVI_LAST)
		dwAfterID = GetTaskID(m_tree.TCH().GetLastChildItem(htiParent));

	// and map it
	m_data.AddTask(m_dwNextUniqueID, pTDINew, dwParentID, dwAfterID);
	ResetCachedCalculations();
	
	// bold state
	BOOL bBold = bRootParent;
	DWORD dwTaskID = m_dwNextUniqueID;
	
	HTREEITEM htiNew = m_tree.TCH().InsertItem(LPSTR_TEXTCALLBACK, 
												I_IMAGECALLBACK, 
												I_IMAGECALLBACK, 
												dwTaskID, // lParam
												htiParent, 
												htiAfter,
												bBold,
												FALSE); // unchecked
	
	if (htiNew)
	{
		// issues relating to parent
		if (!bRootParent)
		{
			// if the parent was being clocked then stop it now
			// since having children makes a task un-clockable
			if (m_dwTimeTrackTaskID && m_dwTimeTrackTaskID == dwParentID && 
				!HasStyle(TDCS_ALLOWPARENTTIMETRACKING))
				EndTimeTracking();

			// if the parent was marked as done and this task is not
			// then mark the parent as incomplete too
			FixupParentCompletion(htiParent);
		}

		// check state
		m_tree.TCH().SetItemChecked(htiNew, pTDINew->IsDone());

		m_dwNextUniqueID++;

		// if we're just about to edit the item text then do not sort
		SetModified(TRUE, bEdit ? TDCA_NONE : TDCA_NEWTASK, dwTaskID); 
		
		SelectItem(htiNew);
		m_tree.Invalidate(TRUE);
		m_tree.UpdateWindow();

		if (bEdit)
			EditSelectedTask(TRUE);
		else
			SetFocusToTasks();
	}
	else // cleanup
		m_data.DeleteTask(m_dwNextUniqueID);
	
	return htiNew;
}

BOOL CToDoCtrl::IsSortable() const
{
	return IsSortable(GetSortBy());
}

BOOL CToDoCtrl::IsSortable(TDC_COLUMN nSortBy)
{
	return (nSortBy != TDCC_NONE);
}

void CToDoCtrl::ResortSelectedTaskParents()
{
	CHTIList lstParents;
	Selection().GetUniqueParents(lstParents);

	POSITION pos = lstParents.GetHeadPosition();

	while (pos)
	{
		HTREEITEM htiParent = lstParents.GetNext(pos);

		if (IsMultiSorting())
		{
			TDSORTCOLUMNS sort;

			GetSortBy(sort);
			MultiSortTree(sort, htiParent, FALSE);
		}
		else
			SortTree(m_sort.nBy1, m_sort.bAscending1, htiParent, FALSE);
	}
}

BOOL CToDoCtrl::CanSplitSelectedTask()
{
	int nSelCount = GetSelectedCount();
	
	return (!IsReadOnly() && (nSelCount > 1 || (nSelCount == 1 && !IsSelectedTaskDone() && 
								!ItemHasChildren(GetSelectedItem()))));
}

BOOL CToDoCtrl::SplitSelectedTask(int nNumSubtasks)
{
	if (nNumSubtasks < 2)
		return FALSE;
	
	if (!GetSelectedCount())
		return FALSE;

	Flush();
	
	IMPLEMENT_UNDO(TDCUAT_ADD);
	HOLD_REDRAW(NULL, m_tree);
	
	POSITION pos = Selection().GetFirstItemPos();
	DWORD dwModTaskID = 0;
	
	while (pos)
	{
		HTREEITEM hti = Selection().GetNextItem(pos);
				
		if (ItemHasChildren(hti))
			continue;
		
		DWORD dwTaskID = GetTaskID(hti);

		TODOITEM* pTDI = m_data.GetTask(dwTaskID);
		ASSERT(pTDI);
		
		if (!pTDI || pTDI->IsDone())
			continue;
		
		// calculate how much time to allocate to each
		BOOL bHasDueTime = pTDI->HasDueTime();
		double dSubTime = 0, dSubEst = pTDI->dTimeEstimate / nNumSubtasks;
		
		if (pTDI->HasStart() && pTDI->dateDue > pTDI->dateStart)
			dSubTime = (pTDI->dateDue - pTDI->dateStart) / nNumSubtasks;

		DWORD dwPrevSiblingID = 0;
		
		for (int nSubtask = 0; nSubtask < nNumSubtasks; nSubtask++)
		{
			TODOITEM* tdiSub = m_data.NewTask(pTDI); // copy parent
			
			// allocate time slice and dates
			tdiSub->dTimeEstimate = dSubEst;
			
			if (dSubTime)
			{
				tdiSub->dateStart += (nSubtask * dSubTime);
				tdiSub->dateDue = tdiSub->dateStart + COleDateTime(dSubTime);

				// clear due time if parent didn't have any
				if (!bHasDueTime)
					tdiSub->dateDue = CDateHelper::GetDateOnly(tdiSub->dateDue);
			}
			else if (nSubtask) // not the first
			{
				tdiSub->ClearStart();
				tdiSub->ClearDue();
			}

			// clear time spent from all but first task
			if (nSubtask)
				tdiSub->dTimeSpent = 0;

			// map it
			DWORD dwChildID = m_dwNextUniqueID++;
			m_data.AddTask(dwChildID, tdiSub, dwTaskID, dwPrevSiblingID);
			
			// create tree item
			HTREEITEM htiSub = m_tree.TCH().InsertItem(LPSTR_TEXTCALLBACK, 
														I_IMAGECALLBACK, 
														I_IMAGECALLBACK, 
														dwChildID,
														hti,
														NULL,
														FALSE,
														FALSE);
			
			dwPrevSiblingID = dwChildID;
			dwModTaskID = dwChildID;
		}

		// clear parent time spent/est
		pTDI->dTimeSpent = pTDI->dTimeEstimate = 0;

		// show new subtasks
		m_tree.Expand(hti, TVE_EXPAND);
	}
	
	SetModified(TRUE, TDCA_NEWTASK, dwModTaskID);
	
	return TRUE;
}

BOOL CToDoCtrl::SelectedTasksHaveChildren() const
{
	return Selection().AnyItemsHaveChildren();
}

BOOL CToDoCtrl::DeleteSelectedTask(BOOL bWarnUser, BOOL bResetSel)
{
	if (IsReadOnly())
		return FALSE;
	
	// check there's something to delete
	int nSelCount = GetSelectedCount();
	
	if (!nSelCount)
		return FALSE;

	// cache focus
	HWND hFocus = ::GetFocus();
	
	// if need confirmation then make sure the user knows that child items will get removed too
	if (bWarnUser && HasStyle(TDCS_CONFIRMDELETE))
	{
		CEnString sMessage(IDS_TDC_CONFIRMDELETESEL);
		UINT nFlags = MB_ICONEXCLAMATION | MB_YESNOCANCEL;
		
		if (SelectedTasksHaveChildren())
		{
			sMessage += (nSelCount > 1) ? CEnString(IDS_TDC_CONFIRMDELETEMOREWARNSUBTASKS) : 
										CEnString(IDS_TDC_CONFIRMDELETEONEWARNSUBTASKS);
			nFlags |= MB_DEFBUTTON2;
		}
		else
			nFlags |= MB_DEFBUTTON1;
		
		if (MessageBoxEx(this, sMessage, IDS_TDC_CONFIRMDELETE_TITLE, nFlags) != IDYES)
			return FALSE;
	}
	
	// snapshot selection against changes
	CHTIList selection;
	Selection().CopySelection(selection);
	
	// reset anchor if we're going to delete it
	if (Selection().AnchorHasSelectedParent())
		Selection().SetAnchor(NULL);
	
	// remove real tree selection first if its going to be deleted
	// but before we do, lets figure out where the selection is
	// going to go _after_ the deletion
	HTREEITEM htiNextSel = NULL;
	CTreeCtrlHelper tch(m_tree);
	
	if (bResetSel)
	{
		HTREEITEM hti = Selection().GetFirstItem();
		
		htiNextSel = tch.GetNextVisibleItem(hti, FALSE);
		
		// don't include items we're about to delete!
		while (htiNextSel)
		{
			if (Selection().HasItem(htiNextSel) || Selection().HasSelectedParent(htiNextSel))
				htiNextSel = tch.GetNextVisibleItem(htiNextSel);
			else
				break;
		}
		
		// else try previous item
		if (!htiNextSel)
		{
			htiNextSel = tch.GetPrevVisibleItem(hti, FALSE);
			
			// don't include items we're about to delete!
			while (htiNextSel)
			{
				if (Selection().HasItem(htiNextSel) || Selection().HasSelectedParent(htiNextSel))
					htiNextSel = tch.GetPrevVisibleItem(htiNextSel);
				else
					break;
			}
		}
	}
	
	if (Selection().HasItem(m_tree.GetSelectedItem()))
		m_tree.TCH().SelectItem(NULL);
	
	// because of the free-form nature of the selection list
	// it can contain children and their parents in the same list
	// which can be a problem if we delete the parent first.
	// so we first remove all items from the list if their
	// parent is in the list because they'll be deleted anyhow
	Selection().RemoveChildDuplicates(selection, m_tree);
	
	// Note: if removing all items we cannot prevent redrawing
	// because Win9x gets itself into a state where the first
	// new task created after the deletion is invisible
	// So, if deleting all items, we withhold the first item
	// and delete it after redrawing has been re-enabled
	BOOL bRemoveAll = Selection().ContainsAllItems();
	HTREEITEM htiDelayDelete = NULL;

	if (bRemoveAll)
		htiDelayDelete = selection.RemoveHead();

	// clear attribute calculations before deleting
	ResetSelectedTaskCachedCalculations(TDCA_DELETE);
	
	// clear selection before deleting
	Selection().RemoveAll(TRUE);

	// now we can delete with impunity
	{
		HOLD_REDRAW(NULL, m_tree);
		POSITION pos = selection.GetHeadPosition();

		while (pos)
		{
			HTREEITEM hti = selection.GetNext(pos);
			DWORD dwTaskID = m_find.GetTaskID(hti);

			m_tree.DeleteItem(hti);
			m_data.DeleteTask(dwTaskID);
		}
		
		RemoveOrphanTaskReferences();
	}

	// delete the delayed item
	if (htiDelayDelete)
	{
		DWORD dwTaskID = m_find.GetTaskID(htiDelayDelete);
		m_tree.DeleteItem(htiDelayDelete);
		m_data.DeleteTask(dwTaskID);
	}
	
	// refresh rest of UI
	SetModified(TRUE, TDCA_DELETE, 0);
	UpdateControls();
	
	// set next selection
	if (htiNextSel)
		SelectItem(htiNextSel);

	// restore focus
	if (hFocus)
		::SetFocus(hFocus);
	else
		SetFocusToTasks();

	return TRUE;
}

BOOL CToDoCtrl::DeleteSelectedTask()
{
	Flush();

	IMPLEMENT_UNDO(TDCUAT_DELETE);

	// are we an archive and should we warn user if we are
	if (m_bArchive && HasStyle(TDCS_WARNADDDELETEARCHIVE))
	{
		if (MessageBoxEx(this, IDS_TDC_WARNDELETEFROMARCHIVE, IDS_TDC_CONFIRMDELETE_TITLE, 
						MB_YESNOCANCEL | MB_ICONQUESTION) != IDYES)
		{
			return FALSE;
		}
		else
			return DeleteSelectedTask(FALSE, TRUE); // FALSE ==  don't warn again
	}
	
	// else do the standard warning against deletion
	return DeleteSelectedTask(TRUE, TRUE);
}

BOOL CToDoCtrl::RemoveOrphanTaskReferences(HTREEITEM hti)
{
	BOOL bRemoved = FALSE;

	// process children
	HTREEITEM htiChild = m_tree.GetChildItem(hti);
	
	while (htiChild)
	{
		// get next item in case htiChild gets cleaned up
		HTREEITEM htiNext = m_tree.GetNextItem(htiChild, TVGN_NEXT);

		if (RemoveOrphanTaskReferences(htiChild)) // RECURSIVE call
			bRemoved = TRUE;

		// then check child itself
		// Note: CToDoCtrlData ought to have already cleaned up the data
		DWORD dwTaskID = GetTaskID(htiChild);
		
		if (m_data.GetTask(dwTaskID, FALSE) == NULL)
		{
			m_tree.DeleteItem(htiChild);
			bRemoved = TRUE;
		}
		
		// next
		htiChild = htiNext;
	}
	
	return bRemoved;
}

BOOL CToDoCtrl::EditSelectedTask(BOOL bTaskIsNew)
{
	if (IsReadOnly() || GetSelectedCount() != 1)
		return FALSE;

	if (IsTaskLabelEditing())
		return TRUE;
	
	HTREEITEM htiSel = GetSelectedItem();
	ASSERT (htiSel);

	if (bTaskIsNew)
		m_dwLastAddedID = GetTaskID(htiSel);
	
	// save task id being edited
	m_dwEditingID = GetSelectedTaskID();

	// create edit if nec.
	if (!m_eTaskName.GetSafeHwnd() && !m_eTaskName.Create(this, IDC_FTC_EDIT, WS_POPUP | WS_CHILD | WS_BORDER/* | WS_CLIPSIBLINGS*/))
		return FALSE;

	// set font
	CFont* pFontTree = m_tree.GetFont();

	if (pFontTree)
		m_eTaskName.SetFont(pFontTree);
	
	// set text
	m_eTaskName.SetWindowText(GetSelectedTaskTitle());
	
	// zero the left margin so that the text position remains the same
	m_eTaskName.SetMargins(0, HIWORD(m_eTaskName.GetMargins()));
	
	// move to the top of the Z-order
	m_eTaskName.SetWindowPos(&wndTopMost, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

	// size and show
	CRect rPos;
	GetItemTitleRect(htiSel, TDCTR_EDIT, rPos);
	ClientToScreen(rPos);

	// make sure it's at least 150 px long
	rPos.right = max(rPos.right, rPos.left + 150);

	m_eTaskName.Show(rPos);

	return TRUE;
}

BOOL CToDoCtrl::GetItemTitleRect(HTREEITEM hti, TDC_TITLERECT nArea, CRect& rect) const
{
	// basic title rect
	m_tree.GetItemRect(hti, rect, TRUE); // label only

	BOOL bFolder = HasStyle(TDCS_SHOWPARENTSASFOLDERS);
	BOOL bCheckbox = HasStyle(TDCS_TREECHECKBOXES) && !IsColumnShowing(TDCC_DONE);

	switch (nArea)
	{
	case TDCTR_CHECKBOX:
		if (bCheckbox)
		{
			rect.right = rect.left;
			rect.left -= 16;

			if (bFolder)
			{
				rect.right = rect.left;
				rect.left -= 16;
			}
			
			return TRUE;
		}
		break;

	case TDCTR_LABEL:
		return TRUE; // nothing more to do

	case TDCTR_BOUNDS:
		{
			m_tree.GetItemRect(hti, rect, FALSE); // whole cell
			return TRUE;
		}
		break;

	case TDCTR_EDIT:
		{
			CRect rBounds;
			m_tree.GetItemRect(hti, rBounds, FALSE); // whole cell
			rect.right = rBounds.right;

			CRect rClient, rInter;
			m_tree.GetClientRect(rClient);

			if (rInter.IntersectRect(rect, rClient))
				rect = rInter;
			
			rect.top--;

/*			// when creating a task that causes the gutter width to change
			// CTreeCtrl::ClientToScreen seems to miscalculate the left of
			// tree rect, so we calculate it ourselves until we find a more
			// correct solution
			int nGutterWidth = m_tree.GetGutterWidth();

			if (!HasStyle(TDCS_RIGHTSIDECOLUMNS) && nGutterWidth > 0)
			{
				CRect rWindow;
				m_tree.GetWindowRect(rWindow);

				// snapshot this before ClientToScreen
				int nRectLeft = rect.left;

				m_tree.ClientToScreen(rect);
				rect.left = nRectLeft + rWindow.left + m_tree.GetGutterWidth();
			}
			else
				m_tree.ClientToScreen(rect);
*/
			// convert to 'our' client coords
			m_tree.ClientToScreen(rect);
			ScreenToClient(rect);
			
			return TRUE;
		}
	}

	return FALSE;
}

BOOL CToDoCtrl::IsTaskLabelEditing() const
{ 
	return (!IsReadOnly() && GetSelectedCount() == 1 && m_dwEditingID != 0);
}

void CToDoCtrl::OnTreeBeginlabeledit(NMHDR* /*pNMHDR*/, LRESULT* pResult) 
{
	//TV_DISPINFO* pTVDispInfo = (TV_DISPINFO*)pNMHDR;
	
	// can't edit multiple items. return TRUE to stop
	// don't edit if the left mouse button is still down
	*pResult = (IsReadOnly() || GetSelectedCount() != 1 || Misc::KeyIsPressed(VK_LBUTTON));

	if (*pResult == 0) // good to edit
	{
		EditSelectedTask(FALSE);
		*pResult = TRUE; // cancel default editing
	}
}

LRESULT CToDoCtrl::OnEditEnd(WPARAM /*wParam*/, LPARAM lParam)
{
	BOOL bReturnSelected = (BOOL)lParam;

	CString sText;
	m_eTaskName.GetWindowText(sText);

	if (!sText.IsEmpty())
	{
		Flush();
		
		IMPLEMENT_UNDOEDIT();
			
		int nRes = m_data.SetTaskTitle(m_dwEditingID, sText);
		
		if (nRes == SET_CHANGE || (m_dwEditingID == m_dwLastAddedID))
			SetModified(TRUE, TDCA_TASKNAME, m_dwEditingID);
	}

	if (GetSelectedCount() == 0) // user clicked into space
		SelectTask(m_dwEditingID);

	// end can occurs either when the user selected return
	// or if the edit box loses the focus so we need to check
	// the lParam and only set the focus if the user chose return
	if (bReturnSelected)
		SetFocusToTasks();

	InvalidateSelectedItem();

	m_dwEditingID = m_dwLastAddedID = 0;
	return 0L;
}

LRESULT CToDoCtrl::OnEditCancel(WPARAM /*wParam*/, LPARAM lParam)
{
	if (GetSelectedCount() == 0) // user clicked into space
		SelectTask(m_dwEditingID);

	// user hit escape key so we may need to delete this task if newly created
	else if (GetSelectedTaskID() == m_dwLastAddedID) 
	{
		ASSERT (lParam);
		UNREFERENCED_PARAMETER(lParam);

		CHoldRedraw hr(m_tree);
		
		// make sure this item is not selected
		HTREEITEM hti = GetSelectedItem();
		ASSERT(GetTaskID(hti) == m_dwLastAddedID);

		// set selection to next task and if that fails then previous
		if (!GotoNextTask(TDCG_NEXT) && !GotoNextTask(TDCG_PREV))
			Selection().RemoveAll();
		
		// then delete and remove from undo
		m_tree.DeleteItem(hti);
		m_data.DeleteTask(m_dwLastAddedID);
		m_data.DeleteLastUndoAction();

		// reset cached calcs because we're not calling SetModified
		ResetCachedCalculations();
		
		// resync fields for selected task
		UpdateControls();
		
		// notify parent of deletion
		GetParent()->SendMessage(WM_TDCN_MODIFY, GetDlgCtrlID(), (LPARAM)TDCA_DELETE); 

	}

	SetFocusToTasks();
	InvalidateSelectedItem();

	m_dwEditingID = m_dwLastAddedID = 0;
	return 0L;
}

BOOL CToDoCtrl::DeleteAllTasks()
{
	if (!GetTaskCount() || IsReadOnly())
		return FALSE;

	Flush();

	if (HasStyle(TDCS_CONFIRMDELETE))
	{
		if (MessageBoxEx(this, IDS_TDC_CONFIRMDELETEALL, IDS_TDC_CONFIRMDELETE_TITLE, 
			MB_YESNOCANCEL | MB_DEFBUTTON2) == IDNO)
			return FALSE;
	}
	// are we an archive and should we warn user if we are
	else if (m_bArchive && HasStyle(TDCS_WARNADDDELETEARCHIVE))
	{
		if (MessageBoxEx(this, IDS_TDC_WARNDELETEFROMARCHIVE, IDS_TDC_CONFIRMDELETE_TITLE, 
						MB_YESNOCANCEL | MB_ICONQUESTION) != IDYES)
		{
			return FALSE;
		}
	}
		
	// must do these first
	Selection().ClearHistory();
	Selection().RemoveAll(); 
	Selection().SetAnchor(NULL); 
	
	IMPLEMENT_UNDO(TDCUAT_DELETE);
	HOLD_REDRAW(*this, 0);

	m_tree.DeleteAllItems();
	m_data.DeleteAllTasks();

	SetModified(TRUE, TDCA_DELETE, 0);
	UpdateControls();
	
	return TRUE;
}

BOOL CToDoCtrl::SetStyle(TDC_STYLE nStyle, BOOL bOn)
{
	HandleUnsavedComments();

	return SetStyle(nStyle, bOn, TRUE);
}

BOOL CToDoCtrl::SetStyle(TDC_STYLE nStyle, BOOL bOn, BOOL bWantUpdate)
{
	ASSERT (GetSafeHwnd());
	
	bOn = bOn ? TRUE : FALSE; // normalize
	
	if (bOn != HasStyle(nStyle))
	{
        m_aStyles[(int)nStyle] = (WORD)bOn;
		
		switch (nStyle)
		{
		case TDCS_SHOWDEFAULTTASKICONS:
			LoadTaskIcons();
			break;

		case TDCS_NODUEDATEISDUETODAY:
			if (bWantUpdate)
				m_tree.RecalcGutter();
			break;
			
		case TDCS_SHOWDATESINISO:
			{
				DWORD dwStyle = m_timeDue.GetStyle();
				
				if (bOn)
					dwStyle |= TCB_ISO;
				else
					dwStyle &= ~(TCB_ISO);
				
				m_timeStart.SetStyle(dwStyle);
				m_timeDue.SetStyle(dwStyle);
				m_timeDone.SetStyle(dwStyle);
				
				if (bWantUpdate)
					m_tree.RecalcGutter();
			}
			break;
			
		case TDCS_SHOWINFOTIPS:
			m_tree.ModifyStyle(bOn ? 0 : TVS_INFOTIP, bOn ? TVS_INFOTIP : 0);
			
			if (bOn)
			{
				// make tooltips remain for as long as possible
				CToolTipCtrl* pTT = m_tree.GetToolTips();
				
				if (pTT)
					pTT->SetDelayTime(TTDT_AUTOPOP, 30000);
			}
			break;
			
		case TDCS_USEEARLIESTDUEDATE:
			if (bOn)
				m_aStyles[TDCS_USELATESTDUEDATE] = FALSE; // mutually exclusive

			if (bWantUpdate)
				Invalidate(FALSE);
			break;

		case TDCS_USELATESTDUEDATE:
			if (bOn)
				m_aStyles[TDCS_USEEARLIESTDUEDATE] = FALSE; // mutually exclusive

			if (bWantUpdate)
				Invalidate(FALSE);
			break;

		case TDCS_USEEARLIESTSTARTDATE:
			if (bOn)
				m_aStyles[TDCS_USELATESTSTARTDATE] = FALSE; // mutually exclusive

			if (bWantUpdate)
				Invalidate(FALSE);
			break;

		case TDCS_USELATESTSTARTDATE:
			if (bOn)
				m_aStyles[TDCS_USEEARLIESTSTARTDATE] = FALSE; // mutually exclusive

			if (bWantUpdate)
				Invalidate(FALSE);
			break;

		case TDCS_SHOWCOMMENTSINLIST:
		case TDCS_SHOWFIRSTCOMMENTLINEINLIST:
		case TDCS_STRIKETHOUGHDONETASKS:
		case TDCS_SHOWSUBTASKCOMPLETION:
			if (bWantUpdate)
				Invalidate(FALSE);
			break;
			
		case TDCS_TASKCOLORISBACKGROUND:
			if (bOn)
			{
				m_cpColour.SetSelectionMode(CP_MODE_BK);
				m_cpColour.SetBkColour(m_cpColour.GetTextColour());
				m_cpColour.SetTextColour(CLR_DEFAULT);
				
			}
			else
			{
				m_cpColour.SetSelectionMode(CP_MODE_TEXT);
				m_cpColour.SetTextColour(m_cpColour.GetBkColour());
				m_cpColour.SetBkColour(CLR_DEFAULT);
			}
			
			if (bWantUpdate)
				Invalidate(FALSE);
			break;
			
		case TDCS_SORTDONETASKSATBOTTOM:
			if (bWantUpdate)
				Resort();
			break;
			
		case TDCS_DUEHAVEHIGHESTPRIORITY:
		case TDCS_DONEHAVELOWESTPRIORITY:
			if (bWantUpdate && GetSortBy() == TDCC_PRIORITY)
				Resort();
			break;
			
		case TDCS_DONEHAVELOWESTRISK:
			if (bWantUpdate && GetSortBy() == TDCC_RISK)
				Resort();
			break;
			
		case TDCS_RIGHTSIDECOLUMNS:
			m_tree.EnableGutterStyle(NCGS_RIGHTCOLUMNS, bOn);
			break;
			
		case TDCS_DISPLAYHMSTIMEFORMAT:
			Invalidate(FALSE);
			
			if (bWantUpdate)
				m_tree.RecalcGutter();
			break;
			
		case TDCS_TREATSUBCOMPLETEDASDONE:
			if (bWantUpdate)
			{
				Invalidate(FALSE);
				m_tree.RecalcGutter();
			}
			break;
			
			
		case TDCS_CALCREMAININGTIMEBYDUEDATE:
		case TDCS_CALCREMAININGTIMEBYSPENT:
		case TDCS_CALCREMAININGTIMEBYPERCENT:
			if (bWantUpdate)
				m_tree.RedrawGutter();
			break;
			
		case TDCS_USEHIGHESTPRIORITY:
		case TDCS_INCLUDEDONEINPRIORITYCALC:
		case TDCS_HIDEPRIORITYNUMBER:
			if (bWantUpdate && IsColumnShowing(TDCC_PRIORITY))
				m_tree.RedrawGutter();
			break;
			
		case TDCS_USEHIGHESTRISK:
		case TDCS_INCLUDEDONEINRISKCALC:
			if (bWantUpdate && IsColumnShowing(TDCC_RISK))
				m_tree.RedrawGutter();
			break;
			
		case TDCS_SHOWNONFILEREFSASTEXT:
			if (bWantUpdate && IsColumnShowing(TDCC_FILEREF))
				m_tree.RecalcGutter();
			break;
			
		case TDCS_USEPERCENTDONEINTIMEEST:
			if (bWantUpdate && IsColumnShowing(TDCC_TIMEEST))
				m_tree.RecalcGutter();
			break;
			
		case TDCS_HIDEZEROTIMECOST:
			if (bWantUpdate && (IsColumnShowing(TDCC_TIMEEST) || 
				IsColumnShowing(TDCC_TIMESPENT) || IsColumnShowing(TDCC_COST)))
				m_tree.RecalcGutter();
			break;
			
		case TDCS_ROUNDTIMEFRACTIONS:
			if (bWantUpdate && (IsColumnShowing(TDCC_TIMEEST) || IsColumnShowing(TDCC_TIMESPENT)))
				m_tree.RecalcGutter();
			break;
			
		case TDCS_HIDEPERCENTFORDONETASKS:
		case TDCS_INCLUDEDONEINAVERAGECALC:
		case TDCS_WEIGHTPERCENTCALCBYNUMSUB:
		case TDCS_SHOWPERCENTASPROGRESSBAR:
			if (bWantUpdate && IsColumnShowing(TDCC_PERCENT))
				m_tree.RedrawGutter();
			break;
			
		case TDCS_AVERAGEPERCENTSUBCOMPLETION:
		case TDCS_AUTOCALCPERCENTDONE:
			if (bWantUpdate)
			{
				if (IsColumnShowing(TDCC_PERCENT))
					m_tree.RecalcGutter();
				else
					m_tree.RedrawGutter();
			}
			break;
			
		case TDCS_HIDESTARTDUEFORDONETASKS:
			if (bWantUpdate && (IsColumnShowing(TDCC_STARTDATE) || IsColumnShowing(TDCC_DUEDATE)))
				m_tree.RedrawGutter();
			break;
			
		case TDCS_SHOWWEEKDAYINDATES:
				if (bWantUpdate && (IsColumnShowing(TDCC_STARTDATE) || IsColumnShowing(TDCC_LASTMOD) ||
					IsColumnShowing(TDCC_DUEDATE) || IsColumnShowing(TDCC_DONEDATE)))
				{
					m_tree.RecalcGutter();
				}
			break;
			
		case TDCS_SHOWPROJECTNAME:
			{
				// ensure focus is ok
				if (!bOn && GetFocus() == GetDlgItem(IDC_PROJECTNAME))
					m_tree.SetFocus();
				
				if (bWantUpdate)
				{
					HOLD_REDRAW(*this, 0);
					Resize();
				}
			}
			break;
			
		case TDCS_SHAREDCOMMENTSHEIGHT:
			if (bOn)
				LoadSplitPos(CPreferences());
			else
				m_nCommentsSize = s_nCommentsSize;
			// fall thru
			
		case TDCS_SHOWCTRLSASCOLUMNS:
		case TDCS_SHOWCOMMENTSALWAYS:
		case TDCS_AUTOREPOSCTRLS:
		case TDCS_COLORTEXTBYPRIORITY:
		case TDCS_COLORTEXTBYATTRIBUTE:
		case TDCS_COLORTEXTBYNONE:
		case TDCS_HIDEDUETIMEFIELD:
		case TDCS_HIDESTARTTIMEFIELD:
		case TDCS_HIDEDONETIMEFIELD:
			if (bWantUpdate)
				Resize();
			break;
			
		case TDCS_READONLY:
		case TDCS_CONFIRMDELETE:
		case TDCS_CHECKOUTONLOAD:
		case TDCS_SORTVISIBLETASKSONLY:
		case TDCS_AUTOCALCTIMEESTIMATES:
		case TDCS_FOCUSTREEONENTER:
		case TDCS_PAUSETIMETRACKINGONSCRNSAVER:
		case TDCS_TRACKSELECTEDTASKONLY:
		case TDCS_LOGTIMETRACKING:
		case TDCS_LOGTASKTIMESEPARATELY:
		case TDCS_WARNADDDELETEARCHIVE:
		case TDCS_ALLOWPARENTTIMETRACKING:
		case TDCS_USES3RDPARTYSOURCECONTROL:
		case TDCS_REFILTERONMODIFY:
		case TDCS_SHOWPARENTSASFOLDERS:
		case TDCS_INCLUDEUSERINCHECKOUT:
		case TDCS_ALLOWREFERENCEEDITING:
			// do nothing
			break;

		case TDCS_AUTOADJUSTDEPENDENCYDATES:
			if (bOn)
			{
				// :TODO: Auto adjust all dependents' dates
			}
			break;
			
		case TDCS_ENABLESOURCECONTROL:
			break;
			
		case TDCS_SHOWPATHINHEADER:
			if (!bOn)
				m_tree.SetGutterColumnHeaderTitle(TDCC_CLIENT, CEnString(IDS_TDC_TASK));
			else
				UpdateSelectedTaskPath();
			break;
			
		case TDCS_FULLROWSELECTION:
			if (bOn)
				m_tree.ModifyStyle(TVS_HASLINES, TVS_FULLROWSELECT);
			else
				m_tree.ModifyStyle(TVS_FULLROWSELECT, TVS_HASLINES);
			break;
			
		case TDCS_TREECHECKBOXES:
			if (bOn && !IsColumnShowing(TDCC_DONE)) // this overrides
				TreeView_SetImageList(m_tree, m_hilDone, TVSIL_STATE);
			else
				TreeView_SetImageList(m_tree, NULL, TVSIL_STATE);
			break;
			
		case TDCS_TREETASKICONS:
			if (bOn && !IsColumnShowing(TDCC_ICON)) // this overrides
				TreeView_SetImageList(m_tree, m_ilTaskIcons, TVSIL_NORMAL);
			else
				TreeView_SetImageList(m_tree, NULL, TVSIL_NORMAL);

			UpdateItemHeight();
			break;
			
		case TDCS_COMMENTSUSETREEFONT:
			if (bOn && m_hFontTree)
				m_ctrlComments.SendMessage(WM_SETFONT, (WPARAM)m_hFontTree, TRUE);
			
			else if (!bOn && m_hFontComments)
				m_ctrlComments.SendMessage(WM_SETFONT, (WPARAM)m_hFontComments, TRUE);
			else
				m_ctrlComments.SendMessage(WM_SETFONT, (WPARAM)CDialogHelper::GetFont(this), TRUE);
			break;
			
		default:
			//ASSERT(0); // just to help catch forgotten styles
			break;
		}
		
		if (bWantUpdate)
		{
			ResetCachedCalculations();
			UpdateControls();
		}
		
		return TRUE; // style was changed
	}
	
	return FALSE; // no change
}

BOOL CToDoCtrl::SetStyles(const CTDCStylesMap& styles)
{
	HandleUnsavedComments();

	// first evaluate whether there any changes or not
	// without changing anything
	BOOL bChange = FALSE;
	int nStyle;
	
	for (nStyle = TDCS_FIRST; nStyle < TDCS_LAST && !bChange; nStyle++)
	{
		BOOL bWantOn = -1, bIsOn = HasStyle((TDC_STYLE)nStyle); // undefined
		
		if (styles.Lookup((TDC_STYLE)nStyle, bWantOn))
		{
			bWantOn = bWantOn ? 1 : 0; // normalize
			bChange = (bWantOn != bIsOn);
		}
	}
	
	if (!bChange)
		return FALSE;
	
	HOLD_REDRAW(*this, NULL);
	
	for (nStyle = TDCS_FIRST; nStyle < TDCS_LAST; nStyle++)
	{
		BOOL bOn = -1; // undefined
		
		if (styles.Lookup((TDC_STYLE)nStyle, bOn))
			SetStyle((TDC_STYLE)nStyle, bOn, FALSE);
	}
	
	Resize();
	m_tree.RecalcGutter();
	ResetCachedCalculations();
	UpdateControls();
	Resort(); 

	return TRUE;
}

BOOL CToDoCtrl::HasStyle(TDC_STYLE nStyle) const 
{ 
	// some special handling for dependent styles
	switch (nStyle)
	{
	case TDCS_CHECKOUTONLOAD:
		if (!HasStyle(TDCS_ENABLESOURCECONTROL))
            return FALSE;
		break;
		
	case TDCS_SHOWFIRSTCOMMENTLINEINLIST:
		if (!HasStyle(TDCS_SHOWCOMMENTSINLIST))
            return FALSE;
		break;

	case TDCS_LOGTASKTIMESEPARATELY:
		if (!HasStyle(TDCS_LOGTIMETRACKING))
            return FALSE;
		break;
	}
	
	return m_aStyles[nStyle] ? TRUE : FALSE; 
}

void CToDoCtrl::SetLayoutPositions(TDC_UILOCATION nControlsPos, TDC_UILOCATION nCommentsPos, BOOL bResize)
{
	BOOL bChanged = (nControlsPos != m_nControlsPos || nCommentsPos != m_nCommentsPos);

	m_nControlsPos = nControlsPos;
	m_nCommentsPos = nCommentsPos;

	if (bChanged && bResize)
		Resize();
}

int CToDoCtrl::GetCustomAttributeDefs(CTDCCustomAttribDefinitionArray& aAttrib) const
{
	aAttrib.Copy(m_aCustomAttribDefs);
	return aAttrib.GetSize();
}

void CToDoCtrl::SetCustomAttributeDefs(const CTDCCustomAttribDefinitionArray& aAttrib)
{
	ASSERT(!IsReadOnly());

	m_aCustomAttribDefs.Copy(aAttrib);
	RebuildCustomAttributeUI();

	SetModified(TRUE, TDCA_CUSTOMATTRIBDEFS, 0);
	UpdateControls();
}

void CToDoCtrl::SetVisibleColumns(const CTDCColumnIDArray& aColumns)
{
	if (!Misc::MatchAll(m_aVisibleColumns, aColumns))
	{
		BOOL bDoneColWasVisible = IsColumnShowing(TDCC_DONE);
		BOOL bIconColWasVisible = IsColumnShowing(TDCC_ICON);

		// update columns
		m_aVisibleColumns.Copy(aColumns);

		// if the Done column visibility has changed then we need to
		// potentially update out the tree checkboxes
		if ((bDoneColWasVisible != IsColumnShowing(TDCC_DONE)) && HasStyle(TDCS_TREECHECKBOXES))
		{
			if (bDoneColWasVisible) // now it's gone so show tree checkboxes
				TreeView_SetImageList(m_tree, m_hilDone, TVSIL_STATE);
			else
				TreeView_SetImageList(m_tree, NULL, TVSIL_STATE);
		}

		// if the Done column visibility has changed then we need to
		// potentially update out the tree icons
		if ((bIconColWasVisible != IsColumnShowing(TDCC_ICON)) && HasStyle(TDCS_TREETASKICONS))
		{
			if (bIconColWasVisible) // now it's gone so show tree checkboxes
				TreeView_SetImageList(m_tree, m_ilTaskIcons, TVSIL_NORMAL);
			else
				TreeView_SetImageList(m_tree, NULL, TVSIL_NORMAL);
		}

		m_tree.RecalcGutter(FALSE);
		UpdateItemHeight();
		
		// hide/show controls which may have been affected
		if (HasStyle(TDCS_SHOWCTRLSASCOLUMNS))
		{
			UpdateControls();
			
			// and resize to allow for control realignment
			Resize();
		}
	}
}

int CToDoCtrl::GetVisibleColumns(CTDCColumnIDArray& aColumns) const
{
	aColumns.Copy(m_aVisibleColumns);

	return aColumns.GetSize();
}

BOOL CToDoCtrl::IsColumnShowing(TDC_COLUMN nColumn) const
{
	if (nColumn == TDCC_CLIENT || CTDCCustomAttributeHelper::IsCustomColumn(nColumn))
		return TRUE; // always visible

	else if (nColumn == OTC_POSCOLUMNID)
		return m_tree.IsGutterPosColumnShowing();

	// else
	int nCol = m_aVisibleColumns.GetSize();

	while (nCol--)
	{
		if (m_aVisibleColumns[nCol] == nColumn)
			return TRUE;
	}

	// else
	return FALSE;
}

TDC_FILE CToDoCtrl::Save(const CString& sFilePath)
{
	CTaskFile tasks;

	return Save(tasks, sFilePath);
}

TDC_FILE CToDoCtrl::Save(CTaskFile& tasks/*out*/, const CString& sFilePath)
{
	ASSERT (GetSafeHwnd());
	
	if (!GetSafeHwnd())
		return TDCO_OTHER;

	// can't save if file is readonly
	if (CDriveInfo::IsReadonlyPath(sFilePath) > 0)
		return TDCO_NOTALLOWED;
	
	// can't save if not checked-out
	// unless we're saving to another filename or this is our first save
	CString sSavePath(sFilePath);

	BOOL bFirstSave = (m_sLastSavePath.IsEmpty() || 
						m_sLastSavePath.CompareNoCase(sSavePath) != 0);
	
	if (HasStyle(TDCS_ENABLESOURCECONTROL) && !m_bCheckedOut && !bFirstSave)
		return TDCO_NOTCHECKEDOUT;
	
	if (sSavePath.IsEmpty())
	{
		if (m_sLastSavePath.IsEmpty())
			return TDCO_OTHER;
		else
			sSavePath = m_sLastSavePath;
	}

	// check for later changes if it's a network file
	BOOL bCheckforLaterChanges = (CDriveInfo::IsRemotePath(sFilePath) > 0);
	
	if (bCheckforLaterChanges && m_nFileVersion > 0) // else its newly created
	{
		if (FileMisc::FileExists(sSavePath)) // file exists (sanity check)
		{
			// i was going to use filetimes but these are too unreliable
			// instead we open the xml file and look at its internal version
			CTaskFile temp;
			CXmlParseController xpc(TDL_FILEVERSION);
			
			if (temp.Load(sSavePath, &xpc, FALSE)) // FALSE => don't decrypt
			{
				if (temp.GetFileVersion() > m_nFileVersion)
				{
					CEnString sMessage(IDS_TDC_CONFIRMOVERWRITE, sSavePath);
					
					if (MessageBoxEx(this, sMessage, IDS_TDC_CONFIRMOVERWRITE_TITLE, 
						MB_ICONWARNING | MB_YESNOCANCEL) != IDYES)
						return TDCO_CANCELLED;
				}
			}
		}
	}
	
	// get tasklist
	GetAllTasks(tasks);

	// file header info
	if (IsModified())
		m_nFileVersion++;
	
	// save globals
	SaveGlobals(tasks);

	// encrypt prior to setting checkout status and file info (so these are visible without decryption)
	// this simply fails if password is empty
	tasks.Encrypt(m_sPassword);

	// then append header info
	AppendTaskFileHeader(tasks);
	
	// checkout status
	// if this is a first time save and source control is enabled
	// then check it out
	if (HasStyle(TDCS_ENABLESOURCECONTROL) && (m_bCheckedOut || bFirstSave))
		tasks.SetCheckedOutTo(GetSourceControlID());

	// backup the file if opening in read-write
	CTempFileBackup backup(sSavePath);
	
	// do the save
	if (tasks.Save(sSavePath))
	{
		m_sLastSavePath = sSavePath;
		m_bModified = FALSE;
		m_bCheckedOut = tasks.IsCheckedOutTo(GetSourceControlID());
		
		// update fileref with the todolist's folder
		m_eFileRef.SetCurrentFolder(FileMisc::GetFolderFromFilePath(sSavePath));

		return TDCO_SUCCESS;
	}

	// restore the backup
	backup.RestoreBackup();
	
	// else error handling
	int nFileErr = tasks.GetLastFileError();

	switch (nFileErr)
	{
	case XFL_BADMSXML:
		return TDCO_BADMSXML;

	default:
		// if nFileErr is greater than zero then it represents GetLastError
		// so we append this to TDCO_OTHER
		if (nFileErr > 0)
		{
			switch (nFileErr)
			{
			case ERROR_ACCESS_DENIED:		return TDCO_NOTALLOWED;
			case ERROR_SHARING_VIOLATION:	return TDCO_INUSE;

			default: return (TDC_FILE)(TDCO_OTHER + nFileErr);
			}
		}
		
		// all else
		return TDCO_OTHER;
	}
}

void CToDoCtrl::SaveGlobals(CTaskFile& file) const
{
	// Note: we always remove the default items before we save
	CStringArray aItems;

	if (GetCategoryNames(aItems))
	{
		Misc::RemoveItems(m_aDefCategory, aItems); 
		file.SetCategoryNames(aItems);
	}

	if (GetTagNames(aItems))
	{
		Misc::RemoveItems(m_aDefTags, aItems); 
		file.SetTagNames(aItems);
	}

	if (GetStatusNames(aItems))
	{
		Misc::RemoveItems(m_aDefStatus, aItems);
		file.SetStatusNames(aItems);
	}

	if (GetAllocToNames(aItems))
	{
		Misc::RemoveItems(m_aDefAllocTo, aItems);
		file.SetAllocToNames(aItems);
	}

	if (GetAllocByNames(aItems))
	{
		Misc::RemoveItems(m_aDefAllocBy, aItems);
		file.SetAllocByNames(aItems);
	}

	if (GetVersionNames(aItems))
		file.SetVersionNames(aItems);
}

void CToDoCtrl::LoadGlobals(const CTaskFile& file)
{
	CStringArray aItems;

	if (file.GetCategoryNames(aItems))
		m_cbCategory.AddUniqueItems(aItems);

	if (file.GetTagNames(aItems))
		m_cbTags.AddUniqueItems(aItems);

	if (file.GetStatusNames(aItems))
		m_cbStatus.AddUniqueItems(aItems);

	if (file.GetAllocToNames(aItems))
		m_cbAllocTo.AddUniqueItems(aItems);

	if (file.GetAllocByNames(aItems))
		m_cbAllocBy.AddUniqueItems(aItems);

	if (file.GetVersionNames(aItems))
		m_cbVersion.AddUniqueItems(aItems);
}

void CToDoCtrl::SaveCustomAttributeDefinitions(CTaskFile& file) const
{
	file.SetCustomAttributeDefs(m_aCustomAttribDefs);
}

void CToDoCtrl::LoadCustomAttributeDefinitions(const CTaskFile& file)
{
	file.GetCustomAttributeDefs(m_aCustomAttribDefs);

	// Add Fields and columns to view
	RebuildCustomAttributeUI();
}

void CToDoCtrl::RebuildCustomAttributeUI()
{
	// insert columns before the first 'icon' column
	int nPos = GetColumnIndex(TDCC_DEPENDENCY);

	CTDCCustomAttributeHelper::RebuildCustomAttributeUI(m_aCustomAttribDefs, m_aCustomControls, 
														m_ilTaskIcons, m_tree, this, nPos);

	Resize();
}

BOOL CToDoCtrl::CheckRestoreBackupFile(const CString& sFilePath)
{
	// check for the existence of a backup file
	CString sBackup = CFileBackup::BuildBackupPath(sFilePath);

	if (FileMisc::FileExists(sBackup))
	{
		double dBackupSize = FileMisc::GetFileSize(sBackup);
		double dSize = FileMisc::GetFileSize(sFilePath);

		if (dSize == 0 && dBackupSize > 0) // definitely a bad save
		{
			::DeleteFile(sFilePath);
			::MoveFile(sBackup, sFilePath);
		}
		else if (dBackupSize > 0 && dSize == dBackupSize) // same size == same file
		{
			::DeleteFile(sBackup);
		}
		else if (dBackupSize > dSize) // prompt
		{
			CEnString sMessage(IDS_BACKUPFILEFOUND, sFilePath);
			int nRet = AfxMessageBox(sMessage, MB_YESNOCANCEL);

			switch (nRet)
			{
			case IDYES:
				::DeleteFile(sFilePath);
				::MoveFile(sBackup, sFilePath);
				break;

			case IDNO: // keep the backup just in case
				{
					CString sRename = CFileBackup::BuildBackupPath(sBackup);
					::MoveFile(sBackup, sRename);
				}
				break;

			case IDCANCEL:
				return FALSE; // do nothing
			}
		}
	}

	// all else
	return TRUE;
}

TDC_FILE CToDoCtrl::Load(const CString& sFilePath)
{
	CTaskFile file;
	return Load(sFilePath, file);
}

TDC_FILE CToDoCtrl::Load(const CString& sFilePath, CTaskFile& tasks/*out*/)
{
	ASSERT (GetSafeHwnd());
	
	if (!GetSafeHwnd())
		return TDCO_OTHER;
	
	if (!CheckRestoreBackupFile(sFilePath))
		return TDCO_CANCELLED;

	if (!FileMisc::FileExists(sFilePath))
		return TDCO_NOTEXIST;

	tasks.SetPassword(m_sPassword);
	
	// work out whether we're going to write to the file
	// as part of the open -> checkout
	BOOL bWantCheckOut = HasStyle(TDCS_ENABLESOURCECONTROL) && HasStyle(TDCS_CHECKOUTONLOAD);
	BOOL bReadOnly = (CDriveInfo::IsReadonlyPath(sFilePath) > 0);

	SetReadonly(bReadOnly); // so that other functions know we are read-only

	XF_OPEN nMode = (bReadOnly || !bWantCheckOut) ? XF_READ : XF_READWRITE;

	// backup the file if opening in read-write
	CTempFileBackup backup;

	if (nMode == XF_READWRITE)
		backup.MakeBackup(sFilePath);

	if (!tasks.Open(sFilePath, nMode, FALSE))
	{
		// if opening in readwrite mode failed then try readonly
		if (nMode == XF_READWRITE && tasks.Open(sFilePath, XF_READ, FALSE))
		{
			// probably a share error
			bReadOnly = TRUE;
			bWantCheckOut = FALSE;
		}
		else
		{
			// do minor error handling
			switch (GetLastError())
			{
			case ERROR_ACCESS_DENIED:		return TDCO_NOTALLOWED;
			case ERROR_SHARING_VIOLATION:	return TDCO_INUSE;

			default:						return TDCO_OTHER;
			}
		}
	}

	// store format for when we save
	m_bIsUnicode = tasks.IsFileUnicode();
	
	if (tasks.LoadEx())
	{
		m_bSourceControlled = tasks.IsSourceControlled();

		// this is going to seem counter-intuitive but we check-out
		// the tasklist before decrypting it else we'll need to 
		// encrypt and resave it when we check it out. so we check it 
		// out first and if the decryption fails we must check it back in

		if (!bReadOnly && HasStyle(TDCS_ENABLESOURCECONTROL))
		{
			if (bWantCheckOut)
				m_bCheckedOut = tasks.CheckOut(GetSourceControlID());
			else
				m_bCheckedOut = tasks.IsCheckedOutTo(GetSourceControlID());
		}

		m_bSourceControlled = tasks.IsSourceControlled();
		
		if (tasks.Decrypt())
		{
			// save off password
			m_sPassword = tasks.GetPassword();
			
			// get comments type from header of older tasklists for the
			// setting of the enclosed tasks
			CONTENTFORMAT cf = tasks.GetCommentsType();

			if (!cf.IsEmpty())
				m_cfComments = cf;

			// meta data
			tasks.GetMetaData(m_mapMetaData);

			// clear undo stack
			m_data.ClearUndo();

			// and selection history unless reloading
			if (m_sLastSavePath != sFilePath)
			{
				Selection().RemoveAll();
				Selection().ClearHistory();
			}
					
			LoadTasks(tasks);
			LoadSplitPos(CPreferences());
			LoadTaskIcons();
			
			// init fileref with the todolist's folder
			m_eFileRef.SetCurrentFolder(FileMisc::GetFolderFromFilePath(sFilePath));

			// lastly, if the tasklist does not have the 'earliest due date'
			// attribute in its header then we resave the tasklist to update
			// the format (relates to delayed loading)
			COleDateTime dtDummy;

			if (!tasks.GetEarliestDueDate(dtDummy) && tasks.GetTaskCount())
			{
				tasks.Close(); // else file won't be writable
				Save(sFilePath);
			}
			SetModified(FALSE);
			
			return TDCO_SUCCESS;
		}

		// else check back in if necessary
		if (HasStyle(TDCS_CHECKOUTONLOAD) && m_bCheckedOut)
		{
			tasks.SetCheckedOutTo("");
			tasks.Save(sFilePath);
		}
	}
	else if (nMode == XF_READWRITE) // restore the backup
		backup.RestoreBackup();

	// else do error handling
	int nFileErr = tasks.GetLastFileError();
	
	switch (nFileErr)
	{
	case XFL_CANCELLED:
		return TDCO_CANCELLED;
		
	case XFL_MISSINGROOT:
		return TDCO_NOTTASKLIST;
		
	case XFL_BADMSXML:
		return TDCO_BADMSXML;
		
	case XFL_NOENCRYPTIONDLL:
		return TDCO_NOENCRYPTIONDLL;
		
	case XFL_UNKNOWNENCRYPTION:
		return TDCO_UNKNOWNENCRYPTION;
	}

	// if nFileErr is greater than zero then it represents GetLastError
	// so we append this to TDCO_OTHER
	if (nFileErr > 0)
		return (TDC_FILE)(TDCO_OTHER + nFileErr);
	
	// all else
	return TDCO_OTHER;
}

BOOL CToDoCtrl::DelayLoad(const CString& sFilePath, COleDateTime& dtEarliestDue)
{
	ASSERT (m_bDelayLoaded || m_sLastSavePath.IsEmpty());

	m_bDelayLoaded = FALSE;
	CTaskFile temp;
	
	if (temp.LoadHeader(sFilePath) && (temp.IsArchive() || temp.GetEarliestDueDate(dtEarliestDue)))
	{
		// save off some of the header info
		SetFilePath(sFilePath);
		SetProjectName(temp.GetProjectName());
		SetModified(FALSE);

		temp.GetCustomAttributeDefs(m_aCustomAttribDefs);

		m_bDelayLoaded = TRUE;
	}

	return m_bDelayLoaded;
}

BOOL CToDoCtrl::LoadTasks(const CTaskFile& file)
{
	ASSERT (GetSafeHwnd());
	
	if (!GetSafeHwnd())
		return FALSE;

	// save visible state
	BOOL bHidden = !IsWindowVisible();

	CPreferences prefs;
	SaveExpandedState(prefs);

	// save selection state if reloading same file
	CDWordArray aTaskIDs;
	DWORD dwFocusedTaskID = 0;
	TDCSELECTIONCACHE cache;

	if (m_sLastSavePath == file.GetFilePath())
		CacheTreeSelection(cache);
	
	// file header info
	m_sXmlHeader = file.GetXmlHeader();
	m_nFileFormat = file.GetFileFormat();
	m_nFileVersion = file.GetFileVersion();
	m_sProjectName = file.GetProjectName();
	m_bArchive = file.IsArchive();
	
	// next unique ID
	m_dwNextUniqueID = file.GetNextUniqueID();
	
	// backwards compatibility
	if (!m_dwNextUniqueID)
		m_dwNextUniqueID = 1;
	
	m_sLastSavePath = file.GetFilePath();
	
	// sorting
	LoadSortState(prefs, file.GetFilePath());
	m_tree.PressGutterColumnHeader((UINT)-1);

	// globals
	LoadGlobals(file);

	// custom attributes
	LoadCustomAttributeDefinitions(file);

	if (file.GetTaskCount())
	{
		HOLD_REDRAW(*this, m_tree);
	
		HTREEITEM htiSel = NULL;
		DWORD dwFirstVis = (m_tree.GetCount() ? GetTaskID(m_tree.GetFirstVisibleItem()) : 0);
		HTREEITEM htiFirst = SetAllTasks(file);

		// reset cached calcs after loading but before sorting
		ResetCachedCalculations();

		if (m_tree.GetCount())
		{
			// redo last sort
			if (IsSortable())
			{
				if (IsMultiSorting())
				{
					TDSORTCOLUMNS sort;

					GetSortBy(sort);
					MultiSort(sort);
				}
				else
					Sort(GetSortBy(), FALSE); // FALSE means 'Don't Toggle'
			}

			// restore last expanded state
			htiSel = LoadExpandedState(prefs);
		}

		// scroll to first visible item
		if (dwFirstVis)
		{
			HTREEITEM htiFirstVis = m_find.GetItem(dwFirstVis);
			
			if (htiFirstVis)
				m_tree.SelectSetFirstVisible(htiFirstVis);
		}
		
		// restore last selected state
		if (cache.aSelTaskIDs.GetSize())
		{
			RestoreTreeSelection(cache);
		}
		else
		{
			if (!htiSel)
				htiSel = htiFirst;
			
			if (htiSel)
				SelectItem(htiSel);
		}
	}
	else
		m_bModified = FALSE;

	// reset last taskedit
	m_tLastTaskMod = COleDateTime::GetCurrentTime();

	LoadSplitPos(prefs);

	m_bDelayLoaded = FALSE;

	// restore previously visibility
	if (bHidden)
		ShowWindow(SW_HIDE);

	return TRUE;
}

int CToDoCtrl::GetArchivableTasks(CTaskFile& tasks, BOOL bSelectedOnly) const
{
	if (bSelectedOnly)
		return GetSelectedTasks(tasks);

	// else
	return GetTasks(tasks, TDCGETTASKS(TDCGT_DONE));
}

void CToDoCtrl::RemoveArchivedTasks(const CTaskFile& tasks, TDC_ARCHIVE nRemove, BOOL bRemoveFlagged)
{
	if (IsReadOnly() || nRemove == TDC_REMOVENONE || !tasks.GetTaskCount())
		return; // nothing to do

	CPreferences prefs;
	SaveExpandedState(prefs);
	
	IMPLEMENT_UNDO(TDCUAT_ARCHIVE);
	
	// clear selection
	Selection().RemoveAll();
	m_tree.TCH().SelectItem(NULL);
	
	CHoldRedraw hr(m_tree);

	// reset the find helper before we start
	//m_find.ResetItemMap();
	
	HTASKITEM hTask = tasks.GetFirstTask();
	BOOL bRemoved = FALSE;

	while (hTask)
	{
		bRemoved |= RemoveArchivedTask(tasks, hTask, nRemove, bRemoveFlagged);
		hTask = tasks.GetNextTask(hTask);
	}

	if (bRemoved)
		SetModified(TRUE, TDCA_ARCHIVE, 0);
}

BOOL CToDoCtrl::RemoveArchivedTask(const CTaskFile& tasks, HTASKITEM hTask, TDC_ARCHIVE nRemove, BOOL bRemoveFlagged)
{
	DWORD dwTaskID = tasks.GetTaskID(hTask);
	const TODOITEM* pTDI = NULL;
	const TODOSTRUCTURE* pTDS = NULL;

	if (!m_data.GetTask(dwTaskID, pTDI, pTDS))
	{
		ASSERT(0);
		return FALSE;
	}

	BOOL bRemove = FALSE;
	BOOL bDone = m_data.IsTaskDone(pTDI, pTDS, TDCCHECKCHILDREN);

	// filter on completed tasks (and flagged tasks if requested)
	if (bDone)
	{
		// Note: it's not enough just to check this task's flagged state 
		// we need to do a deep search for flagged children and grandchildren, etc
		// otherwise they'll get removed when if we remove this task.
		if (!bRemoveFlagged)
		{
			CResultArray aResults;
			SEARCHPARAMS params;
			params.aRules.Add(SEARCHPARAM(TDCA_FLAG, FO_SET));

			bRemove = !m_data.FindTasks(pTDI, pTDS, params, aResults);
		}
		else
			bRemove = TRUE;

		if (bRemove)
		{
			bRemove = FALSE; // reset

			switch (nRemove)
			{
			case TDC_REMOVEALL:
				bRemove = TRUE;
				break;
				
			case TDC_REMOVEIFSIBLINGSANDSUBTASKSCOMPLETE:
				{
					TODOSTRUCTURE* pTDSParent = pTDS->GetParentTask();

					if (pTDSParent->IsRoot() || !m_data.AreChildTasksDone(pTDSParent))
						bRemove = TRUE;
				}
				break;
				
			default:
				ASSERT (0);
			}
			
			if (bRemove && !RemoveArchivedTask(dwTaskID))
				return FALSE;
		}
	}
	
	// process children if not deleted
	if (!bRemove)
	{
		HTASKITEM hChildTask = tasks.GetFirstTask(hTask);
		
		while (hChildTask)
		{
			bRemove |= RemoveArchivedTask(tasks, hChildTask, nRemove, bRemoveFlagged);
			hChildTask = tasks.GetNextTask(hChildTask);
		}
	}

	return bRemove;
}

BOOL CToDoCtrl::RemoveArchivedTask(DWORD dwTaskID)
{
	HTREEITEM hti = m_find.GetItem(dwTaskID);
	ASSERT(hti);
	
	if (!hti)
		return FALSE;
	
	m_tree.DeleteItem(hti);
	m_data.DeleteTask(dwTaskID);

	return TRUE;
}

BOOL CToDoCtrl::ArchiveDoneTasks(const CString& sArchivePath, TDC_ARCHIVE nRemove, BOOL bRemoveFlagged)
{
	// can't archive archives
	if (m_bArchive)
		return FALSE;
	
	if (!GetTaskCount() || sArchivePath.IsEmpty())
		return FALSE;
	
	// 1. first see if there's anything to archive
	CTaskFile xiDone;
	
	if (!GetArchivableTasks(xiDone, FALSE))
		return FALSE;

	if (!ArchiveTasks(sArchivePath, xiDone))
		return FALSE;

	// 5. remove archived tasks from current list
	RemoveArchivedTasks(xiDone, nRemove, bRemoveFlagged);

	return TRUE;
}

BOOL CToDoCtrl::ArchiveSelectedTasks(const CString& sArchivePath, BOOL bRemove)
{
	// can't archive archives
	if (m_bArchive)
		return FALSE;
	
	if (!GetTaskCount() || sArchivePath.IsEmpty())
		return FALSE;
	
	// 1. first see if there's anything to archive
	CTaskFile xiSel;
	
	if (!GetArchivableTasks(xiSel, TRUE))
		return FALSE;
	
	if (!ArchiveTasks(sArchivePath, xiSel))
		return FALSE;

	// delete the selected tasks
	return bRemove ? DeleteSelectedTask() : TRUE;
}

BOOL CToDoCtrl::ArchiveTasks(const CString& sArchivePath, const CTaskFile& tasks)
{
	// can't archive archives
	ASSERT (!m_bArchive);
	
	// must be something to archive
	ASSERT(tasks.GetTaskCount());
	
	// 2. load existing archive if present
	CTaskFile file(m_sPassword);
	TASKFILE_HEADER tfh;
	
	if (file.Load(sArchivePath))
	{
		VERIFY(file.IsArchive());

		// cache header for writing back
		file.GetHeader(tfh);
		tfh.dwNextID = 0; // will be set by taskfile

		// if the file format has changed we may need to re-initialize the xml header
		if (tfh.bUnicode && tfh.sXmlHeader.IsEmpty())
			tfh.sXmlHeader = DEFAULT_UNICODE_HEADER;
		
		// increment file version if not 3rd party source controlled
		if (!HasStyle(TDCS_USES3RDPARTYSOURCECONTROL))
		{
			int nFileVer = file.GetFileVersion() + 1;
			tfh.nFileVersion = max(nFileVer, 1);
		}
	}
	else // or initialize first time
	{
		tfh.bArchive = TRUE;
		tfh.nFileFormat = FILEFORMAT_CURRENT;
		tfh.sProjectName.Format(_T("%s (%s)"), m_sProjectName, CEnString(IDS_TDC_ARCHIVEPROJECT));
		tfh.nFileVersion = 1;
		tfh.sXmlHeader = m_sXmlHeader;

		file.SetSaveAs(GetSaveAs(m_bIsUnicode));
	}
	
	// 3. merge in new done items
	file.Merge(tasks, TRUE, TRUE);
	
	// 4. encrypt archive
	file.Encrypt();

	// 5. Write File header
	file.SetHeader(tfh);

	// 6. save
	return file.Save(sArchivePath);
}

HTREEITEM CToDoCtrl::AddTaskToTreeItem(const CTaskFile& file, HTASKITEM hTask, HTREEITEM htiParent, HTREEITEM htiAfter, TDC_RESETIDS nResetID)
{
	HTREEITEM hti = TVI_ROOT; // default for root item
	TODOITEM* pTDI = NULL;
	
	if (hTask)
	{
		// fixup htiAfter
		if (htiAfter == TVI_LAST)
			htiAfter = m_tree.TCH().GetLastChildItem(htiParent);

		// add new task to map
		DWORD dwTaskID = file.GetTaskID(hTask);

		// make sure that m_dwNextUniqueID exceeds this ID
		if (dwTaskID && nResetID == TDCR_NO)
		{
			ASSERT (m_find.GetItem(dwTaskID) == NULL); // sanity check
			m_dwNextUniqueID = max(m_dwNextUniqueID, dwTaskID + 1);
		}
		else if (dwTaskID && nResetID == TDCR_CHECK)
		{
			// see if it already exist
			if (m_data.GetTask(dwTaskID))
				dwTaskID = m_dwNextUniqueID++; // provide a new unique ID

			else // make sure that m_dwNextUniqueID exceeds this ID
				m_dwNextUniqueID = max(m_dwNextUniqueID, dwTaskID + 1);
		}
		else 
			dwTaskID = m_dwNextUniqueID++;
		
		// add items to map
		DWORD dwParentID = GetTaskID(htiParent);
		DWORD dwPrevTaskID = GetTaskID(htiAfter);
		
		pTDI = m_data.NewTask(file, hTask);
		ASSERT(pTDI);

		if (!pTDI)
			return NULL;
		
		// add this item to data structure
		m_data.AddTask(dwTaskID, pTDI, dwParentID, dwPrevTaskID);
		
		// add this item to tree
		hti = InsertTreeItem(pTDI, dwTaskID, htiParent, htiAfter);
	}
	
	// then children
	HTREEITEM htiFirstItem = (hti == TVI_ROOT) ? NULL : hti;
	HTASKITEM hChildTask = file.GetFirstTask(hTask);
	
	while (hChildTask)
	{
		HTREEITEM htiChild = AddTaskToTreeItem(file, hChildTask, hti, TVI_LAST, nResetID);
		
		if (!htiFirstItem)
			htiFirstItem = htiChild;
		
		hChildTask = file.GetNextTask(hChildTask); // next
	}
		
	return htiFirstItem;
}

HTREEITEM CToDoCtrl::InsertTreeItem(const TODOITEM* pTDI, DWORD dwTaskID, HTREEITEM htiParent, HTREEITEM htiAfter)
{
	HTREEITEM hti = m_tree.TCH().InsertItem(LPSTR_TEXTCALLBACK,
											I_IMAGECALLBACK, 
											I_IMAGECALLBACK, 
											dwTaskID,
											htiParent,
											htiAfter,
											(htiParent == NULL || htiParent == TVI_ROOT), // bold
											pTDI->IsDone()); // checked

	// add unique items to comboboxes
	m_cbAllocTo.AddUniqueItems(pTDI->aAllocTo);
	m_cbAllocBy.AddUniqueItem(pTDI->sAllocBy);
	m_cbStatus.AddUniqueItem(pTDI->sStatus);
	m_cbCategory.AddUniqueItems(pTDI->aCategories);
	m_cbTags.AddUniqueItems(pTDI->aTags);
	m_cbVersion.AddUniqueItem(pTDI->sVersion);

	return hti;
}

void CToDoCtrl::OnTreeGetDispInfo(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMTVDISPINFO lptvdi = (LPNMTVDISPINFO)pNMHDR;
	*pResult = 0;
	
	UINT nMask = lptvdi->item.mask;
	HTREEITEM hti = lptvdi->item.hItem;
	DWORD dwTaskID = (DWORD)lptvdi->item.lParam;
			
	// title text (unless it's being edited)
	if ((nMask & TVIF_TEXT) && m_dwEditingID != dwTaskID)
	{
		const TODOITEM* pTDI = m_data.GetTask(dwTaskID);
		ASSERT (pTDI);
		
		if (!pTDI)
			return;

		lptvdi->item.pszText = NULL;
		
		if (HasStyle(TDCS_SHOWSUBTASKCOMPLETION))
		{
			int nSubtasksCount, nSubtasksDone;
			
			if (m_data.GetSubtaskTotals(GetTaskID(hti), nSubtasksCount, nSubtasksDone))
			{
				static CString sText;
				sText.Format(_T("(%d/%d) %s"), nSubtasksDone, nSubtasksCount, pTDI->sTitle);
				
				lptvdi->item.pszText = (LPTSTR)(LPCTSTR)sText;
			}
		}
		
		// all else
		if (lptvdi->item.pszText == NULL)
			lptvdi->item.pszText = (LPTSTR)(LPCTSTR)pTDI->sTitle;
	}

	// image
	if (nMask & (TVIF_SELECTEDIMAGE | TVIF_IMAGE))
	{
		if (!HasStyle(TDCS_TREETASKICONS))
		{
			lptvdi->item.iImage = lptvdi->item.iSelectedImage = -1;
		}
		else
		{
			const TODOITEM* pTDI = NULL;
			const TODOSTRUCTURE* pTDS = NULL;

			if (!m_data.GetTask(dwTaskID, pTDI, pTDS))
				return;

			BOOL bHasChildren = pTDS->HasSubTasks();
			int nImage = -1;
			
			if (!pTDI->sIcon.IsEmpty())
			{
				nImage = m_ilTaskIcons.GetImageIndex(pTDI->sIcon);
			}
			else if (HasStyle(TDCS_SHOWPARENTSASFOLDERS) && pTDS->HasSubTasks())
			{
				nImage = 0;
			}
			
			lptvdi->item.iImage = lptvdi->item.iSelectedImage = nImage;
		}
	}
}

void CToDoCtrl::DrawItemBackColor(CDC* pDC, const LPRECT rect, COLORREF crBack)
{
	pDC->FillSolidRect(rect, crBack);
}

void CToDoCtrl::OnTreeCustomDraw(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMCUSTOMDRAW* pNMCD = (NMCUSTOMDRAW*)pNMHDR;
	
	if (pNMCD->dwDrawStage == CDDS_PREPAINT)
		*pResult |= CDRF_NOTIFYITEMDRAW | CDRF_NOTIFYPOSTPAINT;	
	
	else 
	{
		NMTVCUSTOMDRAW* pTVCD = (NMTVCUSTOMDRAW*)pNMCD;
		
		HTREEITEM hti = (HTREEITEM)pNMCD->dwItemSpec;
		DWORD dwTaskID = (DWORD)pNMCD->lItemlParam;

		if (dwTaskID == 0)
			return;

		// check row is visible
		CRect rItem, rClient;
		m_tree.GetItemRect(hti, rItem, FALSE);
		m_tree.GetClientRect(rClient);
			
		if (rItem.bottom <= 0 || rItem.top >= rClient.bottom)
			return;
		
		const TODOITEM* pTDI = NULL;
		const TODOSTRUCTURE* pTDS = NULL;
		DWORD dwRefID = dwTaskID; // because GetTask might update it

		if (!m_data.GetTask(dwTaskID, pTDI, pTDS))
			return;

		TDI_STATE nState = GetTreeItemState(hti);
		
		if (pNMCD->dwDrawStage == CDDS_ITEMPREPAINT)
		{
			// set fonts and colors
			NCGITEMCOLORS colors = { (DWORD)hti, 0, 0, FALSE, FALSE };
			OnGutterGetItemColors(m_tree.GetDlgCtrlID(), (LPARAM)&colors);

			if (colors.bBackSet)
			{
				pTVCD->clrTextBk = colors.crBack;
				*pResult |= CDRF_NEWFONT;
			}

			if (colors.bTextSet)
			{
				pTVCD->clrText = colors.crText;
				*pResult |= CDRF_NEWFONT;
			}
			
			// also
			if (pTDI->IsDone() && HasStyle(TDCS_STRIKETHOUGHDONETASKS))
			{
				::SelectObject(pNMCD->hdc, m_hFontDone);
				*pResult |= CDRF_NEWFONT;
			}
			
			*pResult |= CDRF_NOTIFYPOSTPAINT;	
		}
		else if (pNMCD->dwDrawStage == CDDS_ITEMPOSTPAINT)
		{
			CDC* pDC = CDC::FromHandle(pNMCD->hdc);
			int nDC = pDC->SaveDC();
			
			CRect rText;
			m_tree.GetItemRect(hti, rText, TRUE);

			// draw shortcut for references
			if (dwTaskID != dwRefID)
				ShellIcons::DrawIcon(pDC, ShellIcons::SI_SHORTCUT, rText.TopLeft(), false);
			
			// render comment text if not editing this task label
			if (m_dwEditingID != dwTaskID && !pTDI->sComments.IsEmpty())
			{
				rText.top++;
				rText.left = rText.right + 4;
				rText.right = pTVCD->nmcd.rc.right;
				DrawCommentsText(pDC, pTDI, pTDS, rText, nState);
			}
			
			// cleanup
			pDC->RestoreDC(nDC);		
		}
	}
}

void CToDoCtrl::DrawCommentsText(CDC* pDC, const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, const CRect& rText, TDI_STATE nState)
{
	if (!pTDI->sComments.IsEmpty())
	{
		BOOL bSelected = (nState == TDIS_SELECTED);
		BOOL bFullRow = HasStyle(TDCS_FULLROWSELECTION);
		
		CString sTemp, sComments(MAKEINTRESOURCE(IDS_TDC_SEECOMMENTS));
		
		// if we're showing the comments but not the first line
		// and the first line is empty then scoot
		if (HasStyle(TDCS_SHOWCOMMENTSINLIST))
		{
			// calc the max possible comments to render
			int nMaxChars = (int)(rText.Width() / GraphicsMisc::GetAverageCharWidth(pDC));
			
			if (nMaxChars == 0)
				return;
			
			CString sTemp = pTDI->sComments.Left(nMaxChars);
			int nFind = sTemp.FindOneOf(_T("\n\r")); 
			
			if (HasStyle(TDCS_SHOWFIRSTCOMMENTLINEINLIST))
			{
				if (nFind == 0) 
					return; 
				
				else if (nFind > 0) 
					sTemp = sTemp.Left(nFind); 
			}
			else if (nFind != -1)
			{
				sTemp.Replace('\n', ' ');
				sTemp.Replace('\r', ' ');
			}

			sComments.Format(_T("[%s]"), sTemp);
		}
		
		BOOL bFullRowSelected = (bSelected && bFullRow);
		COLORREF crPrev = NOCOLOR;
		
		if (!bFullRowSelected)
		{
			// use 'done' color if done, else use grey unless the window
			// background is a mid-grey
			COLORREF crComments = COMMENTSCOLOR;
			
			if (m_data.IsTaskDone(pTDI, pTDS, TDCCHECKALL))
			{
				if (m_crDone != NOCOLOR)
					crComments = m_crDone;
			}
			else
			{
				RGBX rgbBkGnd(GetSysColor(COLOR_WINDOW));
				
				if (rgbBkGnd.IsGray(2) && rgbBkGnd.Luminance() > 48 && rgbBkGnd.Luminance() < 208)
					crComments = (rgbBkGnd.Luminance() < 128) ? RGB(255, 255, 255) : 0;
			}
			
			crPrev = pDC->SetTextColor(crComments);
		}
		else
		{
			if (bSelected/*pNMCD->uItemState & CDIS_FOCUS*/)
				crPrev = pDC->SetTextColor(GetSysColor(COLOR_HIGHLIGHTTEXT));
			else
				crPrev = pDC->SetTextColor(GetSysColor(COLOR_WINDOWTEXT));
		}
		
		HFONT hOldFont = NULL;

		if (pTDI->IsDone() && m_hFontDone)
			hOldFont = (HFONT)pDC->SelectObject(m_hFontDone);
		
		pDC->SetBkMode(TRANSPARENT);
		pDC->ExtTextOut(rText.left, rText.top, ETO_CLIPPED, &rText, sComments, NULL);

		// restore previous colour
		if (crPrev != NOCOLOR)
			pDC->SetTextColor(crPrev);

		// cleanup
		pDC->SelectObject(hOldFont);
	}
}

void CToDoCtrl::GetTaskTextColors(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS,
								 COLORREF& crText, COLORREF& crBack, BOOL bAllowInvert) const
{
	crText = GetSysColor(COLOR_WINDOWTEXT);
	BOOL bDone = m_data.IsTaskDone(pTDI, pTDS, TDCCHECKALL);

	if (bDone)
	{
		if (m_crDone != NOCOLOR)
			crText = m_crDone; // parent and/or item is done
		else
			crText = pTDI->color; 
	}
	else // all incomplete tasks
	{
		// if it's a ref task just return the ref colour
		if (pTDI->dwTaskRefID && (m_crReference != NOCOLOR))
		{
			crText = m_crReference;
		}
		else
		{
			BOOL bDue = m_data.IsTaskDue(pTDI, pTDS); // due today or overdue
			BOOL bDueToday = bDue ? m_data.IsTaskDue(pTDI, pTDS, TRUE) : FALSE;
			BOOL bOverDue = (bDue && !bDueToday);

			BOOL bStarted = m_data.IsTaskStarted(pTDI, pTDS); // started by now
			BOOL bStartedToday = bStarted ? m_data.IsTaskStarted(pTDI, pTDS, TRUE) : FALSE;
			BOOL bStartedBefore = (bStarted && !bStartedToday);

			// do we have a custom 'due today' color
			if (bDueToday && m_crDueToday != NOCOLOR)
			{
				crText = m_crDueToday;
			}
			// likewise for overdue
			else if (bOverDue && m_crDue != NOCOLOR)
			{
				crText = m_crDue;
			}
			// do we have a custom 'started today' color
			else if (bStartedToday && m_crStartedToday != NOCOLOR)
			{
				crText = m_crStartedToday;
			}
			// likewise for started
			else if (bStartedBefore && m_crStarted != NOCOLOR)
			{
				crText = m_crStarted;
			}
			else if (m_crFlagged != NOCOLOR && pTDI->bFlagged)
			{
				crText = m_crFlagged;
			}
			else if (HasStyle(TDCS_COLORTEXTBYPRIORITY))
			{
				int nPriority = FM_NOPRIORITY;
				
				if (bDueToday)
				{
					nPriority = m_data.GetHighestPriority(pTDI, pTDS, FALSE); // ignore due tasks
				}
				else if (bOverDue)
				{
					nPriority = 10;
				}
				else
				{
					nPriority = m_data.GetHighestPriority(pTDI, pTDS);
				}
				
				if (nPriority != FM_NOPRIORITY)
					crText = GetPriorityColor(nPriority); 
			}
			else if (HasStyle(TDCS_COLORTEXTBYATTRIBUTE))
			{
				switch (m_nColorByAttrib)
				{
				case TDCA_CATEGORY:
					GetAttributeColor(pTDI->GetFirstCategory(), crText);
					break;

				case TDCA_ALLOCBY:
					GetAttributeColor(pTDI->sAllocBy, crText);
					break;

				case TDCA_ALLOCTO:
					GetAttributeColor(pTDI->GetFirstAllocTo(), crText);
					break;

				case TDCA_STATUS:
					GetAttributeColor(pTDI->sStatus, crText);
					break;

				case TDCA_VERSION:
					GetAttributeColor(pTDI->sVersion, crText);
					break;

				case TDCA_EXTERNALID:
					GetAttributeColor(pTDI->sExternalID, crText);
					break;

				case TDCA_TAGS:
					GetAttributeColor(pTDI->GetFirstTag(), crText);
					break;

				default:
					ASSERT(0);
					break;
				}
			}
			else if (!HasStyle(TDCS_COLORTEXTBYNONE) && pTDI->color)
			{
				crText = pTDI->color; 
			}
		}
	}

	// does the user want the text colour to be used for the background?
	if (bAllowInvert && crText != GetSysColor(COLOR_WINDOWTEXT))
	{
		if (HasStyle(TDCS_TASKCOLORISBACKGROUND) && !bDone)
		{
			crBack = crText;
			crText = GraphicsMisc::GetBestTextColor(crBack);
		}
	}
}

CToDoCtrl::TDI_STATE CToDoCtrl::GetTreeItemState(HTREEITEM hti)
{
	if (m_tree.GetItemState(hti, TVIS_DROPHILITED) & TVIS_DROPHILITED)
		return TDIS_DROPHILITED;
	
	else if (IsItemSelected(hti))
		return (m_tree.TCH().HasFocus() ? TDIS_SELECTED : TDIS_SELECTEDNOTFOCUSED);
	
	return TDIS_NONE;
}

void CToDoCtrl::OnTreeChangeFocus(NMHDR* /*pNMHDR*/, LRESULT* pResult) 
{
	Selection().InvalidateAll(FALSE);
	*pResult = 0;
}

void CToDoCtrl::OnTreeKeyDown(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMTVKEYDOWN pTVKey = (LPNMTVKEYDOWN)pNMHDR;
	m_wKeyPress = 0;
	
	// cursor handled in OnTreeSelChanged
	// <shift>+cursor handled in OnTreeSelChanged
	// <ctrl>+cursor handled in PreTranslateMessage
	// <ctrl>+<shift>+cursor handled in PreTranslateMessage
	
	// all we do is get a copy of the key pressed for reference
	switch (pTVKey->wVKey)
	{
	case VK_NEXT:  
	case VK_DOWN:
	case VK_UP:
	case VK_PRIOR: 
	case VK_RIGHT:
	case VK_LEFT:
	case VK_HOME:
	case VK_END:
		m_wKeyPress = pTVKey->wVKey;
		break;
		
	default:
		// handle alphanum method of changing selection
		{
			// convert to char because its easier to work out what
			// are valid chars
			UINT nChar = MapVirtualKey(pTVKey->wVKey, 2);

			if (nChar >= 0x20 && nChar <= 0xFF)
				m_wKeyPress = pTVKey->wVKey;
		}
		break;
	}

	*pResult = 0;
}

void CToDoCtrl::PreSubclassWindow() 
{
	if (!m_hFontDone && HasStyle(TDCS_STRIKETHOUGHDONETASKS)) // create first time
		m_hFontDone = GraphicsMisc::CreateFont(NULL, -1, GMFS_STRIKETHRU);

	CRuntimeDlg::PreSubclassWindow();
}

void CToDoCtrl::OnDestroy() 
{
	EndTimeTracking();
	
	// save state
	CPreferences prefs;

	SaveExpandedState(prefs);
	SaveSplitPos(prefs);
	SaveSortState(prefs);
	
	CString sKey = GetPreferencesKey(m_cfComments);
	m_ctrlComments.SavePreferences(&prefs, sKey);
	
	// have to clean up custom controls here
	// because we rely on their window handles
	// to get the CWnd pointers
	for (int nCtrl = 0; nCtrl < m_aCustomControls.GetSize(); nCtrl++)
	{
		CTRLITEM& ctrl = m_aCustomControls[nCtrl];

		delete GetDlgItem(ctrl.nCtrlID);
		delete GetDlgItem(ctrl.nLabelID);
	}
	
	CRuntimeDlg::OnDestroy();
}

void CToDoCtrl::SetModified(BOOL bMod)
{
	if (!IsReadOnly())
	{
		m_bModified = bMod;
		m_bModSinceLastSort |= bMod;

		if (bMod)
			m_tLastTaskMod = COleDateTime::GetCurrentTime();
	}
}

// internal version
void CToDoCtrl::SetModified(BOOL bMod, TDC_ATTRIBUTE nAttrib, DWORD /*dwModTaskID*/)
{
	if (IsReadOnly())
		return;
	
	SetModified(bMod);
	
	if (bMod)
	{
		// redraw the gutter if the associated column is visible
		BOOL bRedraw = FALSE;
		UINT nRecalcColID = 0;
		
		switch (nAttrib)
		{
		case TDCA_CREATIONDATE:
			// this can only be modified for new tasks via the commandline
			// so nothing needs to be done
			break;

		case TDCA_DONEDATE:
			if (IsColumnShowing(TDCC_DONEDATE))
				nRecalcColID = TDCC_DONEDATE;

			Invalidate(); // text color can be affected
			
			if (IsColumnShowing(TDCC_TIMEEST) && HasStyle(TDCS_USEPERCENTDONEINTIMEEST))
				nRecalcColID = TDCC_TIMEEST;

			if (IsColumnShowing(TDCC_DUEDATE))
			{
				if (nRecalcColID == 0)
					nRecalcColID = TDCC_DUEDATE;
				else
					nRecalcColID = 0xffff; // means 'recalc all'
			}
			break;
			
		case TDCA_DUEDATE:
			if (IsColumnShowing(TDCC_DUEDATE))
				nRecalcColID = TDCC_DUEDATE;
			else
				bRedraw = IsColumnShowing(TDCC_PRIORITY);
			
			// don't redraw while month cal control is visible
			if (m_dateDue.GetMonthCalCtrl() == NULL)
				m_tree.Invalidate(); // text color can be affected 
			break;
			
		case TDCA_STARTDATE:
			if (IsColumnShowing(TDCC_STARTDATE))
				nRecalcColID = TDCC_STARTDATE;
			break;
			
		case TDCA_EXTERNALID:
			if (IsColumnShowing(TDCC_EXTERNALID))
				nRecalcColID = TDCC_EXTERNALID;
			break;
			
		case TDCA_RISK:
			bRedraw = IsColumnShowing(TDCC_RISK);
			break;
			
		case TDCA_RECURRENCE:
			if (IsColumnShowing(TDCC_RECURRENCE))
				nRecalcColID = TDCC_RECURRENCE;
			break;
			
		case TDCA_DEPENDENCY:
			bRedraw = IsColumnShowing(TDCC_DEPENDENCY);
			break;
			
		case TDCA_PRIORITY:
			bRedraw = IsColumnShowing(TDCC_PRIORITY);

			if (!bRedraw && HasStyle(TDCS_SHOWPERCENTASPROGRESSBAR))
				bRedraw = IsColumnShowing(TDCC_PERCENT);
			
			// text color can be affected 
			if (HasStyle(TDCS_COLORTEXTBYPRIORITY))
				Invalidate(); 
			break;
			
		case TDCA_COLOR:
			// text color can be affected 
			if (!HasStyle(TDCS_COLORTEXTBYPRIORITY))
				Invalidate(); 
			break;
			
		case TDCA_ALLOCTO:
			if (IsColumnShowing(TDCC_ALLOCTO))
				nRecalcColID = TDCC_ALLOCTO;
			break;
			
		case TDCA_ALLOCBY:
			if (IsColumnShowing(TDCC_ALLOCBY))
				nRecalcColID = TDCC_ALLOCBY;
			break;
			
		case TDCA_STATUS:
			if (IsColumnShowing(TDCC_STATUS))
				nRecalcColID = TDCC_STATUS;
			break;
			
		case TDCA_VERSION:
			if (IsColumnShowing(TDCC_VERSION))
				nRecalcColID = TDCC_VERSION;
			break;
			
		case TDCA_CATEGORY:
			if (IsColumnShowing(TDCC_CATEGORY))
				nRecalcColID = TDCC_CATEGORY;
			break;
			
		case TDCA_TAGS:
			if (IsColumnShowing(TDCC_TAGS))
				nRecalcColID = TDCC_TAGS;
			break;
			
		case TDCA_PERCENT:
			bRedraw = IsColumnShowing(TDCC_PERCENT);
			Invalidate(); // text color can be affected 
			break;
			
		case TDCA_TIMEEST:
			if (IsColumnShowing(TDCC_TIMEEST))
				nRecalcColID = TDCC_TIMEEST;
			else
				bRedraw = HasStyle(TDCS_AUTOCALCPERCENTDONE);
			break;
			
		case TDCA_TIMESPENT:
			if (IsColumnShowing(TDCC_TIMESPENT))
				nRecalcColID = TDCC_TIMESPENT;
			else
				bRedraw = HasStyle(TDCS_AUTOCALCPERCENTDONE);
			break;
			
		case TDCA_COST:
			if (IsColumnShowing(TDCC_COST))
				nRecalcColID = TDCC_COST;
			break;
			
		case TDCA_FILEREF:
			bRedraw = IsColumnShowing(TDCC_FILEREF);
			break;

		case TDCA_ICON:
			bRedraw = IsColumnShowing(TDCC_ICON);
			break;

		case TDCA_FLAG:
			bRedraw = IsColumnShowing(TDCC_FLAG);
			break;

		case TDCA_TASKNAME:
			bRedraw = IsColumnShowing(TDCC_PATH);
			break;

		case TDCA_PROJNAME:
		case TDCA_COMMENTS:
		case TDCA_ENCRYPT:
		case TDCA_COPY:
			break;

		case TDCA_NONE:
		case TDCA_MOVE:
		case TDCA_DELETE:
		case TDCA_ARCHIVE:
		case TDCA_UNDO:
		case TDCA_NEWTASK:
		case TDCA_CUSTOMATTRIB:
		case TDCA_CUSTOMATTRIBDEFS:
			nRecalcColID = 0xffff; // means 'recalc all'
			break;

		case TDCA_TASKNAMEORCOMMENTS:
		case TDCA_ANYTEXTATTRIBUTE:
		default:
			ASSERT(0);
			break;
		}
		
		// update after deleting are done in DeleteSelectedTask()
		if (nAttrib != TDCA_DELETE)
			ResetSelectedTaskCachedCalculations(nAttrib);

		if (HasStyle(TDCS_SHOWSUBTASKCOMPLETION))
			UpdateSelectedTaskPath();
		
		if (nRecalcColID)
		{
			if (nRecalcColID == 0xffff)
				m_tree.RecalcGutter();

			else if (!m_tree.RecalcGutterColumn(nRecalcColID))
				m_tree.RedrawGutter();
		}
		else if (bRedraw)
			m_tree.RedrawGutter();

		if (ModNeedsResort(nAttrib))
		{
			// if the mod was a task completion and the parent completed state 
			// is based on this then we need to resort the entire tree 
			// likewise for start dates and due dates
			if ((nAttrib == TDCA_DONEDATE && HasStyle(TDCS_TREATSUBCOMPLETEDASDONE)) ||
				(nAttrib == TDCA_DUEDATE && (HasStyle(TDCS_USEEARLIESTDUEDATE) || HasStyle(TDCS_USELATESTDUEDATE))) ||
				(nAttrib == TDCA_STARTDATE && (HasStyle(TDCS_USEEARLIESTSTARTDATE) || HasStyle(TDCS_USELATESTSTARTDATE))))
			{
				Resort();
			}
			else // attributes that only have a local effect
			{
				ResortSelectedTaskParents();
			}
		}
		
		UpdateWindow();
		GetParent()->SendMessage(WM_TDCN_MODIFY, GetDlgCtrlID(), (LPARAM)nAttrib);

		// special case: if this was the project name being edited make sure
		// the focus is set back to the name
		if (nAttrib == TDCA_PROJNAME)
			GetDlgItem(IDC_PROJECTNAME)->SetFocus();
	}
}

void CToDoCtrl::ResetCachedCalculations(TDC_ATTRIBUTE nAttrib) const
{
	m_data.ResetCachedCalculations(nAttrib);
}

void CToDoCtrl::ResetSelectedTaskCachedCalculations(TDC_ATTRIBUTE nAttrib) const
{
	switch (nAttrib)
	{
	// attributes whose effects vary
	case TDCA_DONEDATE:
	case TDCA_DUEDATE:
	case TDCA_STARTDATE:
	case TDCA_RISK:
	case TDCA_PRIORITY:
	case TDCA_PERCENT:
	case TDCA_TIMEEST:
	case TDCA_TIMESPENT:
	case TDCA_COST:
	case TDCA_CUSTOMATTRIB:
	case TDCA_CUSTOMATTRIBDEFS:
	case TDCA_NEWTASK:
	case TDCA_ALLOCTO:
	case TDCA_CATEGORY:
	case TDCA_TAGS:
	case TDCA_TASKNAME:
		{
			POSITION pos = Selection().GetFirstItemPos();
			
			while (pos)
			{
				DWORD dwTaskID = Selection().GetNextItemData(pos);
				m_data.ResetCachedCalculations(dwTaskID, nAttrib);
			}
		}
		break;
	
	// attributes affecting all calculations
	case TDCA_COPY:
	case TDCA_MOVE:
	case TDCA_DELETE:
	case TDCA_ARCHIVE:
	case TDCA_UNDO:
		m_data.ResetCachedCalculations();
		break;
		
	// attributes having no effect
	case TDCA_CREATIONDATE:
	case TDCA_EXTERNALID:
	case TDCA_RECURRENCE:
	case TDCA_DEPENDENCY:
	case TDCA_COLOR:
	case TDCA_ALLOCBY:
	case TDCA_STATUS:
	case TDCA_VERSION:
	case TDCA_PROJNAME:
	case TDCA_COMMENTS:
	case TDCA_ENCRYPT:
	case TDCA_FLAG:
	case TDCA_ICON:
	case TDCA_FILEREF:
	case TDCA_NONE:
		break;
		
	// attributes that should never be called
	case TDCA_TASKNAMEORCOMMENTS:
	case TDCA_ANYTEXTATTRIBUTE:
	default:
		ASSERT(0);
		break;
	}
}

BOOL CToDoCtrl::ModNeedsResort(TDC_ATTRIBUTE nModType) const
{
	if (!HasStyle(TDCS_RESORTONMODIFY))
		return FALSE;

	BOOL bNeedSort = ModNeedsResort(nModType, m_sort.nBy1);

	if (!bNeedSort && IsMultiSorting())
	{
		bNeedSort |= ModNeedsResort(nModType, m_sort.nBy2);
		
		if (!bNeedSort)
			bNeedSort |= ModNeedsResort(nModType, m_sort.nBy3);
	}

	return bNeedSort;
}

BOOL CToDoCtrl::ModNeedsResort(TDC_ATTRIBUTE nModType, TDC_COLUMN nSortBy) const
{
	if (IsSortable(nSortBy))
	{
		switch (nModType)
		{
		case TDCA_TASKNAME:		return (nSortBy == TDCC_CLIENT);
		case TDCA_STARTDATE:	return (nSortBy == TDCC_STARTDATE);
		case TDCA_PRIORITY:		return (nSortBy == TDCC_PRIORITY);
		case TDCA_ALLOCTO:		return (nSortBy == TDCC_ALLOCTO);
		case TDCA_ALLOCBY:		return (nSortBy == TDCC_ALLOCBY);
		case TDCA_STATUS:		return (nSortBy == TDCC_STATUS);
		case TDCA_CATEGORY:		return (nSortBy == TDCC_CATEGORY);
		case TDCA_TAGS:			return (nSortBy == TDCC_TAGS);
		case TDCA_TIMEEST:		return (nSortBy == TDCC_TIMEEST);
		case TDCA_RISK:			return (nSortBy == TDCC_RISK);
		case TDCA_EXTERNALID:	return (nSortBy == TDCC_EXTERNALID);
		case TDCA_VERSION:		return (nSortBy == TDCC_VERSION);
		case TDCA_RECURRENCE:	return (nSortBy == TDCC_RECURRENCE);
		case TDCA_ICON:			return (nSortBy == TDCC_ICON);
		case TDCA_COLOR:		return (nSortBy == TDCC_COLOR);
		case TDCA_FLAG:			return (nSortBy == TDCC_FLAG);

		case TDCA_NEWTASK:
			return TRUE;
			
		case TDCA_DONEDATE:
			{
				if (HasStyle(TDCS_SORTDONETASKSATBOTTOM))
					return TRUE;
			
				// else also check other attribs affected by completed items
				return ((nSortBy == TDCC_PERCENT) || 
						(nSortBy == TDCC_DONEDATE) ||
						(nSortBy == TDCC_DONE) ||
						(nSortBy == TDCC_STARTDATE) ||
						(nSortBy == TDCC_DUEDATE) ||
						((nSortBy == TDCC_RISK) && HasStyle(TDCS_DONEHAVELOWESTRISK)) ||
						((nSortBy == TDCC_PRIORITY) && HasStyle(TDCS_DONEHAVELOWESTPRIORITY)));
			}
			
		case TDCA_DUEDATE:
			{
				// also check priority
				if ((nSortBy == TDCC_PRIORITY) && HasStyle(TDCS_DUEHAVEHIGHESTPRIORITY))
					return TRUE;
				
				// and time remaining
				if ((nSortBy == TDCC_REMAINING) && HasStyle(TDCS_CALCREMAININGTIMEBYDUEDATE))
					return TRUE;
				
				return (nSortBy == TDCC_DUEDATE);
			}
			break;

		case TDCA_PERCENT:
			{
				// and time remaining
				if (nSortBy == TDCC_REMAINING && HasStyle(TDCS_CALCREMAININGTIMEBYPERCENT))
					return TRUE;

				return (nSortBy == TDCC_PERCENT);
			}
			break;
			
		case TDCA_TIMESPENT:
			{
				// and time remaining
				if (nSortBy == TDCC_REMAINING && HasStyle(TDCS_CALCREMAININGTIMEBYSPENT))
					return TRUE;
				
				return (nSortBy == TDCC_TIMESPENT);
			}
			break;
			

		case TDCA_MOVE:
		case TDCA_DELETE:
		case TDCA_ARCHIVE:
		case TDCA_ENCRYPT:
		case TDCA_UNDO:
		case TDCA_NONE:
			break;

		default: // handle custom attributes
			if (CTDCCustomAttributeHelper::IsCustomColumn(nSortBy))
			{
				return CTDCCustomAttributeHelper::IsColumnSortable(nSortBy, m_aCustomAttribDefs);
			}
			break;
		}
	}

	// all else
	return FALSE;
}

LRESULT CToDoCtrl::OnChangeComments(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
	if (!m_ctrlComments.SettingContent())
		UpdateTask(TDCA_COMMENTS);

	return 0L;
}

LRESULT CToDoCtrl::OnCommentsKillFocus(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
	HandleUnsavedComments();

	return 0L;
}

void CToDoCtrl::OnChangeFileRefPath()
{
	UpdateTask(TDCA_FILEREF);
}

void CToDoCtrl::OnChangeProjectName()
{
	UpdateData();
	SetModified(TRUE, TDCA_PROJNAME, 0);
}

void CToDoCtrl::InvalidateSelectedItem()
{
	HTREEITEM htiSel = GetSelectedItem();

	if (htiSel)
	{
		// if the selected item is a reference or a task with references
		// then we invalidate the whole tree
		DWORD dwTaskID = GetTaskID(htiSel);

		if (m_data.IsTaskReference(dwTaskID) || m_data.TaskHasReferences(dwTaskID))
			m_tree.Invalidate(FALSE);
		else
			InvalidateItem(htiSel);
	}
}

void CToDoCtrl::InvalidateItem(HTREEITEM hti)
{
	CRect rItem;
	
	if (m_tree.GetItemRect(hti, rItem, FALSE))
		m_tree.InvalidateRect(rItem, FALSE);
// 	else
// 		TRACE("CToDoCtrl::InvalidateItem(failed)\n");
}

LRESULT CToDoCtrl::OnTreeDragOver(WPARAM /*wParam*/, LPARAM lParam)
{
	const MSG* pMsg = GetCurrentMessage();
	UINT nRes = m_treeDragDrop.ProcessMessage(pMsg);
	
	// handle WM_DD_DRAGOVER for creating task shortcuts
	if (nRes != DD_DROPEFFECT_NONE)
	{
		const DRAGDROPINFO* pDDI = (DRAGDROPINFO*)lParam;

		// only interested in left button drags with ctrl+shift pressed
		if (pDDI->bLeftDrag && Misc::ModKeysArePressed(MKS_CTRL | MKS_SHIFT)) 
			nRes = DD_DROPEFFECT_LINK;
	}

	return nRes;
}

LRESULT CToDoCtrl::OnTreeDragDrop(WPARAM /*wParam*/, LPARAM lParam)
{
	const MSG* pMsg = GetCurrentMessage();
	UINT nRes = m_treeDragDrop.ProcessMessage(pMsg);
	
	// handle WM_DD_DRAGDROP
	if (nRes)
	{
		const DRAGDROPINFO* pDDI = (DRAGDROPINFO*)lParam;
		HTREEITEM htiDrop, htiAfter;
		
		if (pMsg->message == WM_DD_DRAGDROP)
		{
			if (!m_treeDragDrop.GetDropTarget(htiDrop, htiAfter))
				return 0;
			
			BOOL bCopy = FALSE;
			BOOL bMove = FALSE;
			BOOL bRef = FALSE;
			
			m_tree.RedrawGutter();
			
			// if htiAfter is NULL then we are dropping 'on' an item
			// so we need to decide where
			BOOL bDropOn = (htiAfter == NULL);
			
			if (bDropOn)
				htiAfter = m_bDragDropSubtasksAtTop ? TVI_FIRST : TVI_LAST;
			
			if (pDDI->bLeftDrag) 
			{
				bMove  = Misc::ModKeysArePressed(MKS_NONE);
				bCopy = Misc::ModKeysArePressed(MKS_CTRL);
				bRef = Misc::ModKeysArePressed(MKS_CTRL | MKS_SHIFT);
			}
			else // right drag
			{
				CEnMenu menu;
				
				if (menu.LoadMenu(IDR_TREEDRAGDROP, *this, TRUE))
				{
					if (htiDrop && htiDrop != TVI_ROOT)
						m_tree.SelectDropTarget(htiDrop);
					
					CMenu* pSubMenu = menu.GetSubMenu(0);
					CPoint ptCursor(pMsg->pt);
					
					// disable task ref and dependency if dropping in-between tasks
					if (!bDropOn)
					{
						pSubMenu->EnableMenuItem(ID_TDD_SETTASKDEPENDENCY, MF_BYCOMMAND | MF_GRAYED);
						pSubMenu->EnableMenuItem(ID_TDD_ADDTASKDEPENDENCY, MF_BYCOMMAND | MF_GRAYED);
						pSubMenu->EnableMenuItem(ID_TDD_SETFILELINK, MF_BYCOMMAND | MF_GRAYED);
					}
					
					UINT nCmdID = ::TrackPopupMenu(*pSubMenu, TPM_RETURNCMD | TPM_LEFTALIGN | TPM_LEFTBUTTON, 
													ptCursor.x, ptCursor.y, 0, *this, NULL);

					switch (nCmdID)
					{
					case ID_TDD_REFTASK:
						bRef = TRUE;
						break;
						
					case ID_TDD_COPYTASK:
						bCopy = TRUE;
						break;
						
					case ID_TDD_MOVETASK:
						bMove = TRUE;
						break;
						
					case ID_TDD_SETTASKDEPENDENCY:
						{
							DWORD dwTargetID = GetTaskID(htiDrop);
							
							IMPLEMENT_UNDOEDIT();

							// replace task dependencies with this one
							CStringArray aDepends;
							aDepends.Add(TODOITEM::MakeTaskLink(GetSelectedTaskID()));

							m_data.SetTaskDependencies(dwTargetID, aDepends);
							SetModified(TRUE, TDCA_DEPENDENCY, dwTargetID);
						}
						break;

					case ID_TDD_ADDTASKDEPENDENCY:
						{
							DWORD dwTargetID = GetTaskID(htiDrop);

							IMPLEMENT_UNDOEDIT();

							// append this task to task dependencies
							CStringArray aDepends;
							m_data.GetTaskDependencies(dwTargetID, aDepends);
							aDepends.Add(Misc::Format(GetSelectedTaskID()));

							m_data.SetTaskDependencies(dwTargetID, aDepends);
							SetModified(TRUE, TDCA_DEPENDENCY, dwTargetID);
						}
						break;
						
					case ID_TDD_SETFILELINK:
						{
							DWORD dwTargetID = GetTaskID(htiDrop);

							IMPLEMENT_UNDOEDIT();

							CString sTaskRef;
							sTaskRef.Format(_T("tdl://%d"), GetSelectedTaskID());

							m_data.SetTaskFileRef(dwTargetID, sTaskRef);
							SetModified(TRUE, TDCA_FILEREF, dwTargetID);
						}
						break;
					}

					m_tree.SelectDropTarget(NULL);
					m_tree.RedrawGutter();
				}
			}
			
			if (bCopy || bRef)
			{
				// copy selection as xml
				CTaskFile tasks;
				TDCGETTASKS filter;
				
				// if pasting references then we don't want all subtasks just 
				// the ones actually selected
				if (GetSelectedTasks(tasks, filter, (bRef ? TDCGSTF_NOTSUBTASKS : 0)))
				{
					IMPLEMENT_UNDO(TDCUAT_COPY);
					HOLD_REDRAW(*this, m_tree);

					// fix up the dependencies of the copied tasks
					if (bCopy)
						PrepareTaskIDsForPaste(tasks, TDCR_YES);
					else
						PrepareTaskIDsForPasteAsRef(tasks);
					
					// then add them with impunity!
					AddTasksToTree(tasks, htiDrop, htiAfter, TDCR_NO);
					
					// if the parent was marked as done and this task is not
					// then mark the parent as incomplete too
					FixupParentCompletion(htiDrop);
				}
			}
			else if (bMove)
			{
				CTaskFile tasks;
				
				if (GetSelectedTasks(tasks))
				{
					IMPLEMENT_UNDO(TDCUAT_MOVE);
					HOLD_REDRAW(*this, m_tree);
					
					DeleteSelectedTask(FALSE, FALSE);
					AddTasksToTree(tasks, htiDrop, htiAfter, TDCR_NO);
					
					// if the parent was marked as done and this task is not
					// then mark the parent as incomplete too
					FixupParentCompletion(htiDrop);
				}
				
			}
		}
	}

	return nRes;
}

void CToDoCtrl::PrepareTaskIDsForPaste(CTaskFile& tasks, TDC_RESETIDS nResetID) const
{
	if (nResetID == TDCR_NO || (tasks.GetTaskCount() == 0))
		return; // nothing to do

	// work out what new IDs we need and map them to the existing IDs
	CMapID2ID mapID;
	mapID.InitHashTable(tasks.GetTaskCount());

	DWORD dwNextID(max(tasks.GetNextUniqueID(), m_dwNextUniqueID));
	BuildTaskIDMapForPaste(tasks, tasks.GetFirstTask(NULL), dwNextID, mapID, nResetID);

	// then fix them
	PrepareTaskIDsForPaste(tasks, tasks.GetFirstTask(NULL), mapID);
}

void CToDoCtrl::PrepareTaskIDsForPasteAsRef(CTaskFile& tasks) const
{
	if (tasks.GetTaskCount() == 0)
		return; // nothing to do

	CMapID2ID mapID;
	mapID.InitHashTable(tasks.GetTaskCount());

	// work out what new IDs we need and map them to the existing IDs
	DWORD dwNextID(max(tasks.GetNextUniqueID(), m_dwNextUniqueID));
	BuildTaskIDMapForPaste(tasks, tasks.GetFirstTask(NULL), dwNextID, mapID, TDCR_YES);

	// work thru the 'new' tasks adding internal ref back to original
	POSITION pos = mapID.GetStartPosition();

	while (pos)
	{
		DWORD dwOrg, dwRef;
		mapID.GetNextAssoc(pos, dwOrg, dwRef);

		HTASKITEM hTask = tasks.FindTask(dwOrg);

		if (hTask)
		{
			// replace org ID with ref ID
			tasks.SetTaskID(hTask, dwRef);

			// don't create references to references
			if (tasks.GetTaskReferenceID(hTask) == 0)
			{
				// link ref task back to org
				tasks.SetTaskReferenceID(hTask, dwOrg);
			}
		}
	}
}

void CToDoCtrl::BuildTaskIDMapForPaste(CTaskFile& tasks, HTASKITEM hTask, DWORD& dwNextID, 
										CMapID2ID& mapID, TDC_RESETIDS nResetID) const
{
	if (nResetID == TDCR_NO) // sanity check
		return;

	if (!hTask)
		return;

	// we map this task only if it needs a new ID
	DWORD dwTaskID = tasks.GetTaskID(hTask);

	if (dwTaskID <= 0 || nResetID == TDCR_YES || m_data.GetTask(dwTaskID) != NULL)
		mapID[dwTaskID] = dwNextID++;

	// children
	BuildTaskIDMapForPaste(tasks, tasks.GetFirstTask(hTask), dwNextID, mapID, nResetID);

	// siblings
	BuildTaskIDMapForPaste(tasks, tasks.GetNextTask(hTask), dwNextID, mapID, nResetID);
}

void CToDoCtrl::PrepareTaskIDsForPaste(CTaskFile& tasks, HTASKITEM hTask, const CMapID2ID& mapID) const
{
	if (!hTask)
		return;

	// change task's own ID
	DWORD dwTaskID = tasks.GetTaskID(hTask), dwNewID = 0;

	if (mapID.Lookup(dwTaskID, dwNewID))
	{
		ASSERT (dwNewID); // sanity check
		tasks.SetTaskID(hTask, dwNewID);
	}

	// dependencies first
	CStringArray aDepends;
	int nDepend = tasks.GetTaskDependencies(hTask, aDepends);

	if (nDepend)
	{
		BOOL bChanged = FALSE;
		CString sFile;

		while (nDepend--)
		{
			CString sDepends = aDepends[nDepend];

			if (PrepareTaskIDsForPaste(sDepends, mapID))
			{
				aDepends[nDepend] = sDepends;
				bChanged = TRUE;
			}
		}

		// update taskfile if any ID was changed
		if (bChanged)
			tasks.SetTaskDependencies(hTask, aDepends);
	}
	
	// file reference
	CString sFileRef = tasks.GetTaskFileReferencePath(hTask);

	if (PrepareTaskIDsForPaste(sFileRef, mapID))
		tasks.SetTaskFileReferencePath(hTask, sFileRef);

	// children
	PrepareTaskIDsForPaste(tasks, tasks.GetFirstTask(hTask), mapID);

	// siblings
	PrepareTaskIDsForPaste(tasks, tasks.GetNextTask(hTask), mapID);
}

BOOL CToDoCtrl::PrepareTaskIDsForPaste(CString& sLink, const CMapID2ID& mapID) const
{
	DWORD dwTaskID;
	CString sFile, sFilePrefix;

	if (sLink.Find(TDL_PROTOCOL) >= 0)
		sFilePrefix = TDL_PROTOCOL;

	ParseTaskLink(sLink, dwTaskID, sFile);
	
	if (!dwTaskID)
		return FALSE;

	// does the ID need changing
	DWORD dwNewID = 0;
	
	if (mapID.Lookup(dwTaskID, dwNewID))
	{
		ASSERT (dwNewID); // sanity check
		
		// make sure the file path matches us
		if (!sFile.IsEmpty() && !m_sLastSavePath.IsEmpty())
		{
			if (!FileMisc::IsSameFile(sFile, m_sLastSavePath))
				return FALSE;
		}
		
		// update dependency
		sLink = sFilePrefix + TODOITEM::MakeTaskLink(dwNewID, sFile);
		return TRUE;
	}

	return FALSE;

}

BOOL CToDoCtrl::IsReservedShortcut(DWORD dwShortcut)
{
	// check this is not a reserved shortcut used by the tree or a.n.other ctrl
	switch (dwShortcut)
	{
	case MAKELONG(VK_UP, HOTKEYF_EXT):
	case MAKELONG(VK_PRIOR, HOTKEYF_EXT):
	case MAKELONG(VK_DOWN, HOTKEYF_EXT):
	case MAKELONG(VK_NEXT, HOTKEYF_EXT):

	case MAKELONG(VK_SPACE, HOTKEYF_CONTROL):
	case MAKELONG(VK_DELETE, HOTKEYF_CONTROL | HOTKEYF_EXT):
		return TRUE;

	case MAKELONG(VK_UP, HOTKEYF_CONTROL | HOTKEYF_EXT):
	case MAKELONG(VK_PRIOR, HOTKEYF_CONTROL | HOTKEYF_EXT):
	case MAKELONG(VK_DOWN, HOTKEYF_CONTROL | HOTKEYF_EXT):
	case MAKELONG(VK_NEXT, HOTKEYF_CONTROL | HOTKEYF_EXT):
		return (s_nExtendedSelection & HOTKEYF_CONTROL);

	case MAKELONG(VK_UP, HOTKEYF_SHIFT | HOTKEYF_EXT):
	case MAKELONG(VK_PRIOR, HOTKEYF_SHIFT | HOTKEYF_EXT):
	case MAKELONG(VK_DOWN, HOTKEYF_SHIFT | HOTKEYF_EXT):
	case MAKELONG(VK_NEXT, HOTKEYF_SHIFT | HOTKEYF_EXT):
		return (s_nExtendedSelection & HOTKEYF_SHIFT);

	case MAKELONG(VK_UP, HOTKEYF_SHIFT | HOTKEYF_CONTROL | HOTKEYF_EXT):
	case MAKELONG(VK_PRIOR, HOTKEYF_SHIFT | HOTKEYF_CONTROL | HOTKEYF_EXT):
	case MAKELONG(VK_DOWN, HOTKEYF_SHIFT | HOTKEYF_CONTROL | HOTKEYF_EXT):
	case MAKELONG(VK_NEXT, HOTKEYF_SHIFT | HOTKEYF_CONTROL | HOTKEYF_EXT):
		return (s_nExtendedSelection & (HOTKEYF_CONTROL | HOTKEYF_SHIFT)); // both
	}

	// all else
	return FALSE;
}

BOOL CToDoCtrl::PreTranslateMessage(MSG* pMsg) 
{
	BOOL bCtrl = Misc::KeyIsPressed(VK_CONTROL);
	BOOL bShift = Misc::KeyIsPressed(VK_SHIFT);
	BOOL bAlt = Misc::KeyIsPressed(VK_MENU);

	if (m_ctrlComments.PreTranslateMessage(pMsg))
		return TRUE;

	if (m_treeDragDrop.ProcessMessage(pMsg))
		return TRUE;
	
	switch (pMsg->message)
	{
	case  WM_CHAR:
		// eat any enter keys destined for non-multiline
		// edits else they'll beep
		if (pMsg->wParam == VK_RETURN && CWinClasses::IsEditControl(pMsg->hwnd))
		{
			DWORD dwStyle = ::GetWindowLong(pMsg->hwnd, GWL_STYLE);

			if (!(dwStyle & ES_MULTILINE))
				return TRUE;
		}
		break;

	case WM_NCMOUSEMOVE:
		// make sure tree tooltips go away
		if (HasStyle(TDCS_SHOWINFOTIPS))
		{
			if (pMsg->hwnd != m_tree || pMsg->wParam != HTCLIENT)
			{
				CWnd* pTT = m_tree.GetToolTips();
				
				if (pTT)
					pTT->SendMessage(TTM_POP); 
			}
		}
		break;

	case WM_KEYDOWN:
		{
			// handle <return> and <escape> for tree label editing
			if (pMsg->hwnd == (HWND)m_tree.SendMessage(TVM_GETEDITCONTROL))
			{
				switch (pMsg->wParam)
				{
				case VK_ESCAPE:
					m_tree.SendMessage(TVM_ENDEDITLABELNOW, TRUE);
					return TRUE;
					
				case VK_RETURN:
					m_tree.SendMessage(TVM_ENDEDITLABELNOW, FALSE);
					return TRUE;
				}
			}
			// handle <ctrl>/Shift>+cursor for tree.
			// we have to handle these here before the tree gets hold of them
			// because its default behaviour does not suit our multiple
			// selection implementation
			else if (pMsg->hwnd == (HWND)m_tree)
			{
				// cursor handled in OnTreeSelChanged
				// <shift>+cursor handled in OnTreeSelChanged
				// <ctrl>+cursor handled here
				// <ctrl>+<shift>+cursor here
				if (!bAlt)
				{
					// update comments once only
					HandleUnsavedComments();

					// get the real currently selected item
					HTREEITEM hti = m_tree.GetSelectedItem();
					CTreeCtrlHelper tch(m_tree);
					
					bCtrl &= (s_nExtendedSelection & HOTKEYF_CONTROL);
					bShift &= (s_nExtendedSelection & HOTKEYF_SHIFT);
					
					switch (pMsg->wParam)
					{
					case VK_NEXT:  
					case VK_DOWN:
						if (bCtrl)
						{
							HTREEITEM htiNext = NULL;
							
							if (pMsg->wParam == VK_NEXT)
								htiNext = tch.GetNextPageVisibleItem(hti);
							else
								htiNext = tch.GetNextVisibleItem(hti);
							
							if (htiNext)
							{
								tch.SelectItem(htiNext);
								
								// toggle items if shift is also down, but not the one 
								// we're just moving on to
								if (bShift)
								{
									HTREEITEM htiPrev = tch.GetPrevVisibleItem(htiNext, FALSE);
									MultiSelectItems(htiPrev, hti, TSHS_TOGGLE);
								}
							}
							
							return TRUE;
						}
						break;
						
					case VK_UP:
					case VK_PRIOR: 
						if (bCtrl)
						{
							HTREEITEM htiPrev = NULL;
							
							if (pMsg->wParam == VK_PRIOR)
								tch.GetPrevPageVisibleItem(hti);
							else
								tch.GetPrevVisibleItem(hti);
							
							if (htiPrev)
							{
								tch.SelectItem(htiPrev);
								
								// toggle items if shift is also down, but not the one 
								// we're just moving on to
								if (bShift)
								{
									HTREEITEM htiNext = tch.GetNextVisibleItem(htiPrev, FALSE);
									MultiSelectItems(htiNext, hti, TSHS_TOGGLE);
								}
							}
							
							return TRUE;
						}
						break;
						
					case VK_SPACE:
						if (bCtrl && !bShift)
						{
							// toggle real selected item state
							MultiSelectItem(hti, TSHS_TOGGLE);
							return TRUE;
						}
						break;
					}
				}
				
				// see if the tree wants it
				if (((CWnd*)&m_tree)->PreTranslateMessage(pMsg))
					return TRUE;
			}
			// handle <enter> and TDCS_FOCUSTREEONENTER
			else if (HasStyle(TDCS_FOCUSTREEONENTER) && pMsg->wParam == VK_RETURN)
			{
				// have to be careful here because dropped-down comboboxes should
				// take precedence. so we have to check explicitly for them
				// and make sure they're not in a dropped state
				CWnd* pFocus = GetFocus();
				
				if (pFocus && IsChild(pFocus))
				{
					if (!ControlWantsEnter(*pFocus))
						SetFocusToTasks();

					return FALSE; // allow further routing
				}
			}
		}
		break;
	}
	
	return CRuntimeDlg::PreTranslateMessage(pMsg);
}

void CToDoCtrl::RefreshItemStates(HTREEITEM hti, BOOL bBold, BOOL bCheck, BOOL bAndChildren)
{
	if (!bBold && !bCheck)
		return;

	if (hti && hti != TVI_ROOT)
	{
		if (bBold)
			m_tree.TCH().SetItemBold(hti, m_tree.GetParentItem(hti) == NULL);

		if (bCheck)
		{
			const TODOITEM* pTDI = m_find.GetTask(hti);
			ASSERT (pTDI);

			if (pTDI)
				m_tree.TCH().SetItemChecked(hti, pTDI->IsDone());
		}
	}

	// children
	if (bAndChildren)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);

		while (htiChild)
		{
			RefreshItemStates(htiChild, bBold, bCheck);
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
}

BOOL CToDoCtrl::MoveSelectedTask(TDC_MOVETASK nDirection)
{
	if (!CanMoveSelectedTask(nDirection))
		return FALSE;
	
	// else
	Flush(); // end any editing action
	SetFocusToTasks(); // else datetime controls get their focus screwed

	// do the move
	MoveSelection(nDirection);

	if (nDirection == TDCM_UP || nDirection == TDCM_DOWN)
	{
		// if moving up or down make sure we scroll ahead a bit
		m_tree.TCH().SetMinDistanceToEdge(Selection().GetFirstItem(), TCHE_TOP, 2);

		if (GetSelectedCount() > 1)
			m_tree.TCH().SetMinDistanceToEdge(Selection().GetLastItem(), TCHE_BOTTOM, 2);
	}
	else
	{
		// update bold states
		RefreshItemStates(NULL, TRUE, FALSE);
	}

	SetModified(TRUE, TDCA_MOVE, 0);

	return TRUE;
}

BOOL CToDoCtrl::CanMoveSelectedTask(TDC_MOVETASK nDirection) const
{
	if (IsReadOnly())
		return FALSE;

	CHTIList selection;

	Selection().CopySelection(selection);
	Selection().RemoveChildDuplicates(selection, m_tree);

	if (!selection.GetCount() || !Selection().ItemsAreAllSiblings(selection, m_tree))
		return FALSE;

	// must be able to move all items
	POSITION pos = selection.GetHeadPosition();
	
	while (pos)
	{
		HTREEITEM hti = selection.GetNext(pos);
#ifdef _DEBUG
		DWORD dwTaskID = GetTaskID(hti);
#endif
		
		if (!CanMoveItem(hti, nDirection))
			return FALSE;
	}

	return TRUE;
}

BOOL CToDoCtrl::CanMoveItem(HTREEITEM hti, TDC_MOVETASK nDirection) const
{
	if (!hti)
		return FALSE;

	switch (nDirection)
	{
	case TDCM_DOWN:
		return (m_tree.GetNextItem(hti, TVGN_NEXT) != NULL);

	case TDCM_UP:
		return (m_tree.GetNextItem(hti, TVGN_PREVIOUS) != NULL);

	case TDCM_LEFT:
		{
			// must have a parent which will become its sibling
			// and not the root
			HTREEITEM htiParent = m_tree.GetParentItem(hti);
			return (htiParent && htiParent != TVI_ROOT);
		}
		break;

	case TDCM_RIGHT:
		// must have a prior sibling (which will become the parent)
		return CanMoveItem(hti, TDCM_UP);
	}

	return FALSE;
}

BOOL CToDoCtrl::MoveSelection(TDC_MOVETASK nDirection)
{
	if (CanMoveSelectedTask(nDirection))
	{
		ResetCachedCalculations();

		// 0. save off the selection state so we can restore it after
		HTREEITEM htiTreeSel, htiAnchor;
		Selection().GetAnchorSel(htiAnchor, htiTreeSel);

		DWORD dwTreeSelID = GetTaskID(htiTreeSel);
		DWORD dwAnchorID = GetTaskID(htiAnchor);

		// 1. keep track of task IDs so we can restore selection
		CDWordArray aSelTaskIDs;
		m_selection.GetTaskIDs(aSelTaskIDs);

		// 2. prepare selection
		Selection().RemoveChildDuplicates();
		Selection().SortIfAllSiblings(FALSE);

		// 3. copy tree selection
		CHTIList selection;
		Selection().CopySelection(selection);

		// copy the task IDs that we're actually going to move
		CDWordArray aTaskIDs;
		m_selection.GetTaskIDs(aTaskIDs);

		// 4. figure out the parent to move tasks to, and the item
		// to insert after
		HTREEITEM htiSel = Selection().GetFirstItem();
		HTREEITEM htiDestParent = NULL, htiDestAfter = NULL;

		switch (nDirection)
		{
		case TDCM_DOWN:	
			htiDestAfter = m_tree.GetNextItem(Selection().GetLastItem(), TVGN_NEXT);
			htiDestParent = m_tree.GetParentItem(htiDestAfter);
			break;

		case TDCM_LEFT:
			htiDestAfter = m_tree.GetParentItem(htiSel);
			htiDestParent = m_tree.GetParentItem(htiDestAfter);
			break;

		case TDCM_UP:
			htiDestParent = m_tree.GetParentItem(htiSel);

			// we need item before previous item to copy *after*
			htiDestAfter = m_tree.GetNextItem(htiSel, TVGN_PREVIOUS);
			htiDestAfter = m_tree.GetNextItem(htiDestAfter, TVGN_PREVIOUS);

			if (htiDestAfter == NULL)
				htiDestAfter = TVI_FIRST;
			break;

		case TDCM_RIGHT:
			htiDestParent = m_tree.GetNextItem(htiSel, TVGN_PREVIOUS);
			htiDestAfter = m_tree.TCH().GetLastChildItem(htiDestParent);
			break;
		}

		// 5. clear selection
		Selection().RemoveAll(TRUE, FALSE);
		m_tree.TCH().SelectItem(NULL);

		// 6. move the tasks
		IMPLEMENT_UNDO(TDCUAT_MOVE);

		DWORD dwDestParentID = m_find.GetTaskID(htiDestParent);
		DWORD dwDestPrevSiblingID = m_find.GetTaskID(htiDestAfter);
		
		m_data.MoveTasks(aTaskIDs, dwDestParentID, dwDestPrevSiblingID);
		
		// 7. move the tree items
		{
			HOLD_REDRAW(*this, m_tree);
			POSITION pos = selection.GetHeadPosition();
			
			while (pos)
			{
				HTREEITEM hti = selection.GetNext(pos);
				htiDestAfter = MoveItem(hti, htiDestParent, htiDestAfter);

				RefreshItemStates(htiDestAfter, TRUE, FALSE);
			}
			
			// refresh parent states if moving to the right (adding subtasks)
			if (nDirection == TDCM_RIGHT)
				FixupParentCompletion(htiDestParent);
		}

		// 8. restore tree state
		MultiSelectItems(aSelTaskIDs, TSHS_SELECT);
		Selection().RestoreAnchorSel(m_find.GetItem(dwAnchorID), m_find.GetItem(dwTreeSelID));

		// 9. propagate new parent's attributes to subtasks
		ApplyLastInheritedChangeToSubtasks(dwDestParentID, TDCA_ALL);

		return TRUE;
	}

	return FALSE;
}

HTREEITEM CToDoCtrl::MoveItem(HTREEITEM hti, HTREEITEM htiDestParent, HTREEITEM htiDestPrevSibling)
{
	DDWHERE nWhere = DD_BELOW; // most likely
	HTREEITEM htiTarget = htiDestPrevSibling;

	// validate htiTarget
	if (htiTarget == TVI_FIRST)
	{
		htiTarget = m_tree.GetChildItem(htiDestParent);
		nWhere = DD_ABOVE;
	}
	else if (htiTarget == TVI_LAST)
		htiTarget = CTreeCtrlHelper(m_tree).GetLastChildItem(htiDestParent);

	// if htiTarget is NULL then the target parent has no children at present
	// so we just move directly on to it
	if (htiTarget == NULL)
	{
		htiTarget = htiDestParent;
		nWhere = DD_ON;
	}

	// do the move and return the new tree item
	return CTreeDragDropHelper::MoveTree(m_tree, htiTarget, hti, nWhere, DD_USESTEXTCALLBACK/* | DD_USESIMAGECALLBACK*/);
}


BOOL CToDoCtrl::GotoNextTopLevelTask(TDC_GOTO nDirection)
{
	HTREEITEM htiNext = m_tree.TCH().GetNextTopLevelItem(GetSelectedItem(), nDirection == TDCG_NEXT);
	
	if (htiNext)
		SelectItem(htiNext);

	return (htiNext != NULL);
}

BOOL CToDoCtrl::CanGotoNextTopLevelTask(TDC_GOTO nDirection) const
{
	return (NULL != m_tree.TCH().GetNextTopLevelItem(GetSelectedItem(), nDirection == TDCG_NEXT));
}

BOOL CToDoCtrl::GotoNextTask(TDC_GOTO nDirection)
{
	HTREEITEM htiNext = NULL;

	if (nDirection == TDCG_NEXT)
		htiNext = m_tree.TCH().GetNextVisibleItem(GetSelectedItem());
	else
		htiNext = m_tree.TCH().GetPrevVisibleItem(GetSelectedItem());
	
	if (htiNext)
		SelectItem(htiNext);

	return (htiNext != NULL);
}

BOOL CToDoCtrl::CanGotoNextTask(TDC_GOTO nDirection) const
{
	HTREEITEM htiNext = NULL;

	if (nDirection == TDCG_NEXT)
		htiNext = m_tree.TCH().GetNextVisibleItem(GetSelectedItem());
	else
		htiNext = m_tree.TCH().GetPrevVisibleItem(GetSelectedItem());

	return (htiNext != NULL);
}

LRESULT CToDoCtrl::OnGutterGetItemColors(WPARAM /*wParam*/, LPARAM lParam)
{
	NCGITEMCOLORS* pColors = (NCGITEMCOLORS*)lParam;
	HTREEITEM hti = (HTREEITEM)pColors->dwItem;
	DWORD dwTaskID = GetTaskID(hti);
	TDI_STATE nState = GetTreeItemState(hti);

	pColors->crText = GetSysColor(COLOR_WINDOWTEXT);
	pColors->crBack = GetItemLineColor(hti);

	GetItemColors(dwTaskID, pColors, nState);

	return 0L;
}

void CToDoCtrl::GetItemColors(DWORD dwTaskID, NCGITEMCOLORS* pColors, TDI_STATE nState)
{
	ASSERT(dwTaskID);

	if (!dwTaskID)
		return;

	switch (nState)
	{
	case TDIS_DROPHILITED:
		if (!pColors->bBackSet)
		{
			pColors->crBack = GetSysColor(COLOR_3DFACE);
			pColors->bBackSet = TRUE;
		}

		if (!pColors->bTextSet)
		{
			pColors->crText = GetSysColor(COLOR_WINDOWTEXT);
			pColors->bTextSet = TRUE;
		}
		break;
		
	case TDIS_SELECTED:
		if (!pColors->bBackSet)
		{
			pColors->crBack = GetSysColor(COLOR_HIGHLIGHT);
			pColors->bBackSet = TRUE;
		}

		if (!pColors->bTextSet)
		{
			pColors->crText = GetSysColor(COLOR_HIGHLIGHTTEXT);
			pColors->bTextSet = TRUE;
		}
		break;

	case TDIS_SELECTEDNOTFOCUSED:
		if (!pColors->bBackSet)
		{
			COLORREF color = GetSysColor(COLOR_HIGHLIGHT);
			
			pColors->crBack = GraphicsMisc::Lighter(color, 0.5);
			pColors->bBackSet = TRUE;
		}

		if (!pColors->bTextSet)
		{
			pColors->crText = GetSysColor(COLOR_HIGHLIGHTTEXT);
			pColors->bTextSet = TRUE;
		}
		break;

	default:
		{
			const TODOITEM* pTDI = NULL;
			const TODOSTRUCTURE* pTDS = NULL;
		
			// if reference task colour is NOT set we get
			// the true task else the ref task
			BOOL bWantTrueTask = (m_crReference == NOCOLOR);

			if (!m_data.GetTask(dwTaskID, pTDI, pTDS, bWantTrueTask))
				return;

			// we handle all other colours
			GetTaskTextColors(pTDI, pTDS, pColors->crText, pColors->crBack);
			pColors->bTextSet = pColors->bBackSet = TRUE;
		}
		break;
	}
}

LRESULT CToDoCtrl::OnGutterDrawItem(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
	return FALSE;
}

LRESULT CToDoCtrl::OnGutterDrawItemColumn(WPARAM /*wParam*/, LPARAM lParam)
{
	NCGDRAWITEM* pNCGDI = (NCGDRAWITEM*)lParam;
	HTREEITEM hti = (HTREEITEM)pNCGDI->dwItem;
	DWORD dwTaskID = GetTaskID(hti);

	const TODOITEM* pTDI = NULL;
	const TODOSTRUCTURE* pTDS = NULL;

	// cache taskID before GetTask might change it
	DWORD dwRefID = dwTaskID;

	if (!m_data.GetTask(dwTaskID, pTDI, pTDS))
		return FALSE;

	if (dwRefID == dwTaskID)
		dwRefID = 0;

	return DrawItemColumn(pTDI, pTDS, pNCGDI, GetTreeItemState(hti), dwRefID);
}

BOOL CToDoCtrl::DrawItemColumn(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, const NCGDRAWITEM* pNCGDI, TDI_STATE nState, DWORD dwRefID)
{
	ASSERT(pTDI && pTDS);

	if (!pTDI || !pTDS)
		return FALSE;

	TDC_COLUMN nColID = (TDC_COLUMN)pNCGDI->nColID;

	// is it visible
	if (!CRect().IntersectRect(pNCGDI->rItem, pNCGDI->rWindow))
		return TRUE;
	
	// is it showing
	if (!IsColumnShowing(nColID))
		return TRUE;

	CRect rItem(pNCGDI->rItem);
	DWORD dwTaskID = pTDS->GetTaskID();
	
	// vertical divider
	if (m_tree.GetGridlineColor() !=-1)
		pNCGDI->pDC->FillSolidRect(rItem.right - 1, rItem.top, 1, rItem.Height(), m_tree.GetGridlineColor());
	
	// specific columns
	switch (nColID)
	{
	case TDCC_POSITION:
		{
			CString sPosition = m_data.GetTaskPosition(pTDI, pTDS);
			DrawGutterItemText(pNCGDI->pDC, sPosition, rItem, pNCGDI->nTextAlign);
		}
		break;

	case TDCC_PRIORITY:
		// priority color
		if (!HasStyle(TDCS_DONEHAVELOWESTPRIORITY) || !m_data.IsTaskDone(pTDI, pTDS, TDCCHECKALL))
		{
			rItem.DeflateRect(2, 1, 3, 2);
			
			// first draw the priority colour
			int nPriority = m_data.GetHighestPriority(pTDI, pTDS, FALSE);
			BOOL bHasPriority = (nPriority != FM_NOPRIORITY);
			
			if (bHasPriority)
			{
				COLORREF crFill = GetPriorityColor(nPriority);
				COLORREF crBorder = GraphicsMisc::Darker(crFill, 0.5);
				
				GraphicsMisc::DrawRect(pNCGDI->pDC, rItem, crFill, crBorder);//, nCornerRadius);
			}
			
			// then, if the task is also due, draw a small tag
			// to indicate this
			if (m_data.IsTaskDue(pTDI, pTDS))
			{
				BOOL bDueToday = m_data.IsTaskDue(pTDI, pTDS, TRUE);
				
				// unless it's due today and the user doesn't want today's tasks marked as due
				// or there's no due color 
				if ((!bDueToday && m_crDue != NOCOLOR) || 
					(bDueToday && m_crDueToday != NOCOLOR))
				{
					POINT pt[3] = { { rItem.left, rItem.top + 7 }, 
					{ rItem.left, rItem.top }, 
					{ rItem.left + 7, rItem.top } };
					
					CBrush* pOldBr = pNCGDI->pDC->SelectObject(bDueToday ? &m_brDueToday : &m_brDue);
					pNCGDI->pDC->SelectStockObject(NULL_PEN);
					
					pNCGDI->pDC->Polygon(pt, 3);
					pNCGDI->pDC->SelectObject(pOldBr);
					
					// draw a black line between the two
					pNCGDI->pDC->SelectStockObject(BLACK_PEN);
					pNCGDI->pDC->MoveTo(rItem.left, rItem.top + 6);
					pNCGDI->pDC->LineTo(rItem.left + 7, rItem.top - 1);
				}
			}
			
			// draw priority number over the top
			if (bHasPriority && !HasStyle(TDCS_HIDEPRIORITYNUMBER))
			{
				COLORREF color = GetPriorityColor(nPriority);
				CString sPriority = Misc::Format(nPriority);
				
				rItem.left++; // centres it properly
				//rItem.top -= 2;
				
				// pick best colour for text
				int nLum = RGBX(color).Luminance();
				COLORREF crText = (nLum < 128) ? RGB(255, 255, 255) : 0;
				
				DrawGutterItemText(pNCGDI->pDC, sPriority, rItem, DT_CENTER, crText);
			}
		}
		break;
		
	case TDCC_RISK:
		if (HasStyle(TDCS_INCLUDEDONEINRISKCALC) || !m_data.IsTaskDone(pTDI, pTDS, TDCCHECKALL))
		{
			int nRisk = m_data.GetHighestRisk(pTDI, pTDS);
			
			if (nRisk != FM_NORISK)
			{
				CString sRisk = Misc::Format(nRisk);
				DrawGutterItemText(pNCGDI->pDC, sRisk, rItem, pNCGDI->nTextAlign);
			}
		}
		break;
		
	case TDCC_RECURRENCE:
		{
			CString sRecurrence = CTDLRecurringTaskEdit::GetRegularity(pTDI->trRecurrence, FALSE);
			DrawGutterItemText(pNCGDI->pDC, sRecurrence, rItem, pNCGDI->nTextAlign);
		}
		break;
		
	case TDCC_ID:
		{
			CString sID;

			if (dwRefID)
				sID.Format(_T("(%d) %d"), dwTaskID, dwRefID);
			else
				sID.Format(_T("%d"), dwTaskID);

			DrawGutterItemText(pNCGDI->pDC, sID, rItem, pNCGDI->nTextAlign);
		}
		break;
		
	case TDCC_PARENTID:
		{
			CString sID = Misc::Format(pTDS->GetParentTaskID());
			DrawGutterItemText(pNCGDI->pDC, sID, rItem, pNCGDI->nTextAlign);
		}
		break;
		
	case TDCC_RECENTEDIT:
		if (pTDI->IsRecentlyEdited())
			DrawGutterItemText(pNCGDI->pDC, "*", rItem, pNCGDI->nTextAlign);
		break;
		
	case TDCC_COST:
		{
			double dCost = m_data.CalcCost(pTDI, pTDS);
			
			if (dCost != 0.0 || !HasStyle(TDCS_HIDEZEROTIMECOST))
				DrawGutterItemText(pNCGDI->pDC, Misc::Format(dCost, 2), rItem, pNCGDI->nTextAlign);
		}
		break;
		
	case TDCC_EXTERNALID:
		DrawGutterItemText(pNCGDI->pDC, pTDI->sExternalID, rItem, pNCGDI->nTextAlign);
		break;
		
	case TDCC_VERSION:
		DrawGutterItemText(pNCGDI->pDC, pTDI->sVersion, rItem, pNCGDI->nTextAlign);
		break;
		
	case TDCC_ALLOCTO:
		DrawGutterItemText(pNCGDI->pDC, m_data.FormatTaskAllocTo(dwTaskID), rItem, pNCGDI->nTextAlign);
		break;
		
	case TDCC_ALLOCBY:
		DrawGutterItemText(pNCGDI->pDC, pTDI->sAllocBy, rItem, pNCGDI->nTextAlign);
		break;
		
	case TDCC_STATUS:
		DrawGutterItemText(pNCGDI->pDC, pTDI->sStatus, rItem, pNCGDI->nTextAlign);
		break;
		
	case TDCC_CATEGORY:
		DrawGutterItemText(pNCGDI->pDC, m_data.FormatTaskCategories(dwTaskID), rItem, pNCGDI->nTextAlign);
		break;
		
	case TDCC_TAGS:
		DrawGutterItemText(pNCGDI->pDC, m_data.FormatTaskTags(dwTaskID), rItem, pNCGDI->nTextAlign);
		break;
		
	case TDCC_CREATEDBY:
		DrawGutterItemText(pNCGDI->pDC, pTDI->sCreatedBy, rItem, pNCGDI->nTextAlign);
		break;
		
	case TDCC_PERCENT:
		{
			BOOL bDone = -1; // so we know when it's set
			BOOL bHidePercent = HasStyle(TDCS_HIDEPERCENTFORDONETASKS);
			
			if (bHidePercent)
				bDone = m_data.IsTaskDone(pTDI, pTDS, TDCCHECKALL);
			
			if (!bHidePercent || !bDone)
			{
				int nPercent = m_data.CalcPercentDone(pTDI, pTDS);
				
				CString sPercent;
				sPercent.Format(_T("%d%%"), nPercent);
				
				// draw progressbar?
				if (HasStyle(TDCS_SHOWPERCENTASPROGRESSBAR))
				{
					// if the task is done then draw in 'done' colour else priority colour
					if (bDone == -1) // not yet determined
						bDone = m_data.IsTaskDone(pTDI, pTDS, TDCCHECKALL);
					
					COLORREF crBar(m_crDone);
					
					if (!bDone || !HasStyle(TDCS_DONEHAVELOWESTPRIORITY)) // determine appropriate priority
					{
						int nPriority = m_data.GetHighestPriority(pTDI, pTDS, FALSE);
						crBar = GetPriorityColor(nPriority);
						
						// check for due
						if (m_data.IsTaskDue(pTDI, pTDS))
						{
							BOOL bDueToday = m_data.IsTaskDue(pTDI, pTDS, TRUE);
							
							if (bDueToday && m_crDueToday != NOCOLOR)
								crBar = m_crDueToday;
							
							else if (m_crDue != NOCOLOR)
								crBar = m_crDue;
						}
					}
					
					if (crBar != NOCOLOR)
					{
						rItem.DeflateRect(2, 1, 3, 2);
						CRect rProgress(rItem);

						// draw border
						COLORREF crBorder = GraphicsMisc::Darker(crBar, 0.5);
						GraphicsMisc::DrawRect(pNCGDI->pDC, rProgress, NOCOLOR, crBorder);

						// draw fill
						rProgress.DeflateRect(1, 1);
						rProgress.right = rProgress.left + MulDiv(rProgress.Width(), nPercent, 100);

						if (rProgress.Width() > 0)
						{
							pNCGDI->pDC->FillSolidRect(rProgress, crBar); 

							// draw text in colour to suit pbar
							// modify text colour to suit
							int nLum = RGBX(crBar).Luminance();
							COLORREF crPBText = (nLum < 128) ? RGB(255, 255, 255) : 0;
							
							DrawGutterItemText(pNCGDI->pDC, sPercent, rItem, pNCGDI->nTextAlign, crPBText);
							
							// then clip out the pbar rect
							pNCGDI->pDC->ExcludeClipRect(rProgress);
						}
					}
				}
				
				rItem.bottom--;
				DrawGutterItemText(pNCGDI->pDC, sPercent, rItem, pNCGDI->nTextAlign);
			}
		}
		break;
		
	case TDCC_TIMEEST:
	case TDCC_TIMESPENT:
		{
			BOOL bTimeEst = (nColID == TDCC_TIMEEST);
			int nUnits = bTimeEst ? s_tdDefault.nTimeEstUnits : s_tdDefault.nTimeSpentUnits; // good default value
			
			// get actual task time units
			if (!pTDS->HasSubTasks() || HasStyle(TDCS_ALLOWPARENTTIMETRACKING))
				nUnits = bTimeEst ? pTDI->nTimeEstUnits : pTDI->nTimeSpentUnits;
			
			// draw time
			double dTime = bTimeEst ? m_data.CalcTimeEstimate(pTDI, pTDS, nUnits) :
										m_data.CalcTimeSpent(pTDI, pTDS, nUnits);
			
			if (dTime > 0 || !HasStyle(TDCS_HIDEZEROTIMECOST))
			{
				  int nDecPlaces = HasStyle(TDCS_ROUNDTIMEFRACTIONS) ? 0 : 2;
				  CString sTime;
				  
				  if (HasStyle(TDCS_DISPLAYHMSTIMEFORMAT))
					  sTime = CTimeHelper().FormatTimeHMS(dTime, nUnits, (BOOL)nDecPlaces);
				  else
					  sTime = CTimeHelper().FormatTime(dTime, nUnits, nDecPlaces);
				  
				  // show text in red if we're currently tracking
				  BOOL bTimeTracking = (m_dwTimeTrackTaskID == dwTaskID) &&
					  (nState == TDIS_SELECTED || 
					  nState == TDIS_SELECTEDNOTFOCUSED ||
					  !HasStyle(TDCS_TRACKSELECTEDTASKONLY));
				  
				  COLORREF crText = NOCOLOR;
				  
				  if (!bTimeEst && bTimeTracking)
					  crText = 255;
				  
				  DrawGutterItemText(pNCGDI->pDC, sTime, rItem, pNCGDI->nTextAlign, crText);
			}
		}
		break;
		
	case TDCC_TRACKTIME:
		if (m_dwTimeTrackTaskID == dwTaskID)
		{
			COLORREF crText = NOCOLOR;
			
			// show icon in red if we're currently tracking
			if (nState == TDIS_SELECTED || nState == TDIS_SELECTEDNOTFOCUSED ||
				!HasStyle(TDCS_TRACKSELECTEDTASKONLY))
				crText = 255;
			
			DrawGutterItemText(pNCGDI->pDC, CLOCKCHAR, rItem, pNCGDI->nTextAlign, crText, &GraphicsMisc::WingDings(), TRUE);
		}
		break;
		
	case TDCC_FLAG:
		if (pTDI->bFlagged)
			DrawGutterItemText(pNCGDI->pDC, FLAGCHAR, rItem, pNCGDI->nTextAlign, NOCOLOR, &GraphicsMisc::WingDings(), TRUE);
		break;

	case TDCC_PATH:
		if (pTDS->GetParentTaskID())
		{
			CEnString sPath = GetTaskPath(dwTaskID);
			sPath.FormatDC(pNCGDI->pDC, 150, ES_PATH);

			DrawGutterItemText(pNCGDI->pDC, sPath, rItem, pNCGDI->nTextAlign);
		}
		break;
		
	case TDCC_REMINDER:
		if (GetParent()->SendMessage(WM_TDCM_TASKHASREMINDER, dwTaskID, (LPARAM)this))
			DrawGutterItemText(pNCGDI->pDC, BELLCHAR, rItem, pNCGDI->nTextAlign, NOCOLOR, &GraphicsMisc::WingDings(), TRUE);
		break;
		
	case TDCC_STARTDATE:
		{
			COleDateTime date;
			BOOL bDone = m_data.IsTaskDone(pTDI, pTDS, TDCCHECKALL);
			
			if (bDone && !HasStyle(TDCS_HIDESTARTDUEFORDONETASKS))
				date = pTDI->dateStart;

			else if (!bDone)
				date = m_data.CalcStartDate(pTDI, pTDS);
			
			DrawGutterItemDate(pNCGDI->pDC, date, TDCD_START, rItem);
		}
		break;
		
	case TDCC_DUEDATE:
		{
			COleDateTime date;
			BOOL bDone = m_data.IsTaskDone(pTDI, pTDS, TDCCHECKALL);
			
			if (bDone && !HasStyle(TDCS_HIDESTARTDUEFORDONETASKS))
				date = pTDI->dateDue;

			else if (!bDone)
				date = m_data.CalcDueDate(pTDI, pTDS);
			
			DrawGutterItemDate(pNCGDI->pDC, date, TDCD_DUE, rItem);
		}
		break;
		
	case TDCC_REMAINING:
		{
			int nUnits = THU_HOURS;
			double dRemaining = m_data.CalcRemainingTime(pTDI, pTDS, nUnits);

			// format appropriately
			CString sTime;
			
			if (HasStyle(TDCS_CALCREMAININGTIMEBYPERCENT) || HasStyle(TDCS_CALCREMAININGTIMEBYSPENT))
			{
				sTime = CTimeHelper().FormatTime(dRemaining, nUnits, TRUE);
			}
			else // TDCS_CALCREMAININGTIMEBYDUEDATE
			{
				COleDateTime date = m_data.CalcDueDate(pTDI, pTDS);
				
				if (date.m_dt > 0) 
				{
					if (HasStyle(TDCS_DISPLAYHMSTIMEFORMAT))
						sTime = CTimeHelper(24.0, 7.0).FormatTimeHMS(dRemaining, THU_DAYS, TRUE);
					else
					{
						// find best units for display
						if (fabs(dRemaining) >= 1.0)
						{
							sTime = CTimeHelper().FormatTime(dRemaining, THU_DAYS, 1);
						}
						else
						{
							dRemaining *= 24; // to hours

							if (fabs(dRemaining) >= 1.0)
							{
								sTime = CTimeHelper().FormatTime(dRemaining, THU_HOURS, 1);
							}
							else
							{
								dRemaining *= 60; // to mins
								sTime = CTimeHelper().FormatTime(dRemaining, THU_MINS, 0);
							}
						}
					}
				}
			}
			
			if (!sTime.IsEmpty())
				DrawGutterItemText(pNCGDI->pDC, sTime, rItem, pNCGDI->nTextAlign);
		}
		break;
		
	case TDCC_DONEDATE:
		DrawGutterItemDate(pNCGDI->pDC, pTDI->dateDone, TDCD_DONE, rItem);
		break;
		
	case TDCC_CREATIONDATE:
		DrawGutterItemDate(pNCGDI->pDC, pTDI->dateCreated, TDCD_CREATE, rItem);
		break;
		
	case TDCC_LASTMOD:
		DrawGutterItemDate(pNCGDI->pDC, pTDI->dateLastMod, TDCD_LASTMOD, rItem);
		break;

	case TDCC_ICON:
		{
			int nIcon = m_ilTaskIcons.GetImageIndex(pTDI->sIcon);

			if (nIcon == -1 && pTDS->HasSubTasks() && HasStyle(TDCS_SHOWPARENTSASFOLDERS))
				nIcon = 0;

			if (nIcon >= 0)
			{
				// center vertically and horizontally
				CPoint pt(rItem.TopLeft());
				pt.x += (rItem.Width() - 16) /2;
				pt.y += (rItem.Height() - 16) /2;
				m_ilTaskIcons.Draw(pNCGDI->pDC, nIcon, pt, ILD_TRANSPARENT);
			}
		}
		break;

	case TDCC_DEPENDENCY:
		if (pTDI->aDependencies.GetSize())
		{
			// draw our app icon 
			HICON hIcon = (HICON)AfxGetMainWnd()->SendMessage(WM_GETICON, ICON_SMALL);
			
			if (hIcon)
			{
				// center vertically and horizontally
				CPoint pt(rItem.TopLeft());
				pt.x += (rItem.Width() - 16) /2;
				pt.y += (rItem.Height() - 16) /2;
				::DrawIconEx(pNCGDI->pDC->GetSafeHdc(), pt.x, pt.y, hIcon, 16, 16, 0, NULL, DI_NORMAL);
			}
		}
		break;
		
	case TDCC_FILEREF:
		if (!pTDI->sFileRefPath.IsEmpty())
		{
			// first check for a tdl://
			if (pTDI->sFileRefPath.Find(TDL_PROTOCOL) != -1)
			{
				// draw our app icon 
				HICON hIcon = (HICON)AfxGetMainWnd()->SendMessage(WM_GETICON, ICON_SMALL);
				
				if (hIcon)
				{
					CPoint pt(rItem.TopLeft());
					pt.x += (rItem.Width() - 16) /2;
					pt.y += (rItem.Height() - 16) /2;
					::DrawIconEx(pNCGDI->pDC->GetSafeHdc(), pt.x, pt.y, hIcon, 16, 16, 0, NULL, DI_NORMAL);
				}
			}
			else
			{
				// get the associated image, failing if necessary
				CString sFileRef = pTDI->sFileRefPath;
				sFileRef.Remove('\"'); // remove quotes
				
				int nImage = m_ilFileRef.GetFileImageIndex(sFileRef, TRUE);
				
				if (nImage == -1 && HasStyle(TDCS_SHOWNONFILEREFSASTEXT))
					DrawGutterItemText(pNCGDI->pDC, sFileRef, rItem, DT_LEFT);
				else
				{
					if (nImage == -1)
						nImage = m_ilFileRef.GetFileImageIndex(sFileRef, FALSE);
					
					if (nImage >= 0)
					{
						CPoint pt(rItem.TopLeft());
						pt.x += (rItem.Width() - 16) /2;
						pt.y += (rItem.Height() - 16) /2;
						m_ilFileRef.GetImageList()->Draw(pNCGDI->pDC, nImage, pt, ILD_TRANSPARENT);
					}
				}
			}
		}
		break;
		
	case TDCC_DONE:
		{
			BOOL bDone = pTDI->IsDone();
			rItem.DeflateRect((rItem.Width() - 16) / 2, (rItem.Height() - 16) / 2);
			
			if (m_hilDone)
			{
				int nImage = bDone ? 2 : 1; // first image is blank
				ImageList_Draw(m_hilDone, nImage, pNCGDI->pDC->GetSafeHdc(), 
					rItem.left, rItem.top, ILD_TRANSPARENT);
			}
			else
			{
				rItem.right--;
				rItem.bottom--;
				CThemed::DrawFrameControl(this, pNCGDI->pDC, rItem, DFC_BUTTON, 
					DFCS_BUTTONCHECK | (bDone ? DFCS_CHECKED : 0));
			}
		}
		break;

	default:
		// custom attribute columns
		if (CTDCCustomAttributeHelper::IsCustomColumn(nColID))
		{
			TDCCUSTOMATTRIBUTEDEFINITION attribDef;
			VERIFY (CTDCCustomAttributeHelper::GetAttributeDef(nColID, m_aCustomAttribDefs, attribDef));

			TDCCADATA data(pTDI->GetCustomDataValue(attribDef.sUniqueID));

			if (data.IsEmpty())
				break;

			switch (attribDef.GetDataType())
			{
			case TDCCA_DATE:
				DrawGutterItemDate(pNCGDI->pDC, data.AsDate(), TDCD_CUSTOM, rItem);
				break;

			case TDCCA_ICON:
				{
					CString sImage = data.AsString();
					CString sName = attribDef.GetImageName(sImage);
					
					// calculate icon and text position
					rItem.left += NCG_COLPADDING;

					// centre icon vertically
					CPoint pt(rItem.left, (rItem.CenterPoint().y - 8));
					int nTextAlign = pNCGDI->nTextAlign;

					switch (nTextAlign)
					{
					case DT_RIGHT:
						rItem.right -= (NCG_COLPADDING + 16);
						pt.x = rItem.right;
						break;

					case DT_CENTER:
						// if there is associated text then we align left
						if (sName.IsEmpty())
							pt.x = (rItem.left + ((rItem.Width() - 16) / 2));
						else 
						{
							nTextAlign = DT_LEFT;
							rItem.left += 16;
						}
						break;

					case DT_LEFT:
					default:
						rItem.left += 16;
						break;
					}

					m_ilTaskIcons.Draw(pNCGDI->pDC, sImage, pt, ILD_TRANSPARENT);

					if (attribDef.IsList() && !sName.IsEmpty()) // optional text
						DrawGutterItemText(pNCGDI->pDC, sName, rItem, nTextAlign);
				}
				break;

			case TDCCA_BOOL:
				if (attribDef.sColumnTitle.GetLength() == 1 && data.AsBool())
					DrawGutterItemText(pNCGDI->pDC, attribDef.sColumnTitle, rItem, pNCGDI->nTextAlign);
				else
					DrawGutterItemText(pNCGDI->pDC, data.AsString(), rItem, pNCGDI->nTextAlign);
				break;

			case TDCCA_DOUBLE:
				DrawGutterItemText(pNCGDI->pDC, Misc::Format(data.AsDouble(), 2), rItem, pNCGDI->nTextAlign);
				break;

			default:
				if (attribDef.GetListType() == TDCCA_FIXEDMULTILIST || attribDef.GetListType() == TDCCA_AUTOMULTILIST)
				{
					CStringArray aItems;

					if (data.AsArray(aItems))
						DrawGutterItemText(pNCGDI->pDC, Misc::FormatArray(aItems), rItem, pNCGDI->nTextAlign);
				}
				else
					DrawGutterItemText(pNCGDI->pDC, data.AsString(), rItem, pNCGDI->nTextAlign);
			}
		}
		else
		{
			ASSERT(0);
			return FALSE;
		}
	}
		
	return TRUE; // we handled it
}

BOOL CToDoCtrl::FormatDate(const COleDateTime& date, TDC_DATE nDate, CString& sDate, CString& /*sTime*/, CString& /*sDow*/)
{	
	sDate.Empty();
//	sTime.Empty();
//	sDow.Empty();

	if (date.m_dt > 0) 
	{
		DWORD dwFmt = 0;

		if (HasStyle(TDCS_SHOWWEEKDAYINDATES))
			dwFmt |= DHFD_DOW;
					
		if (HasStyle(TDCS_SHOWDATESINISO))
			dwFmt |= DHFD_ISO;
					
		if (CDateHelper::GetTimeOnly(date) > 0)
		{
			if ((nDate == TDCD_DONE && !HasStyle(TDCS_HIDEDONETIMEFIELD)) || 
				(nDate == TDCD_DUE && !HasStyle(TDCS_HIDEDUETIMEFIELD)) ||  
				(nDate == TDCD_START && !HasStyle(TDCS_HIDESTARTTIMEFIELD)) || 
				 nDate == TDCD_LASTMOD) 
			{
				dwFmt |= DHFD_TIME | DHFD_NOSEC;
			}
		}
					
		sDate = CDateHelper::FormatDate(date, dwFmt);
		return TRUE;
//		return CDateHelper::FormatDate(date, dwFmt, sDate, sTime, sDow);
	}

	// else
	return FALSE;
}

LRESULT CToDoCtrl::OnGutterPostDrawItem(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
//	NCGDRAWITEM* pNCGDI = (NCGDRAWITEM*)lParam;

	return FALSE;
}

LRESULT CToDoCtrl::OnGutterWantRedraw(WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(wParam);
	ASSERT (wParam == IDC_TASKLIST);
	
	BOOL bCancel = FALSE;
	const MSG* pMsg = (MSG*)lParam;
	
	switch (pMsg->message)
	{
	case TVM_SELECTITEM:
		// we deal with drop highlight changes ourselves
		if (pMsg->wParam == TVGN_DROPHILITE)
		{
			static HTREEITEM htiPrevDropHilite = NULL;
			
			// we do it ourselves
			if (htiPrevDropHilite != (HTREEITEM)pMsg->lParam)
			{
				m_tree.RedrawGutterItem((DWORD)htiPrevDropHilite);
				m_tree.RedrawGutterItem((DWORD)pMsg->lParam);
				
				htiPrevDropHilite = (HTREEITEM)pMsg->lParam;
			}
			
			bCancel = TRUE;
		}

	case WM_KEYDOWN:
	case WM_KEYUP:
		{
			BOOL bUpdateControls = FALSE;

			// ignore cursor keys because this is handled by TVM_SELECTITEM
			switch (pMsg->wParam)
			{
				// we can't ignore these if the ctrl key is un-reserved
				// and the control key is pressed 
				// simply because we don't have control over what happens
				case VK_DOWN:
				case VK_UP:
				case VK_PRIOR: 
				case VK_NEXT:  
				case VK_END:   
				case VK_HOME:  
					bCancel = !Misc::KeyIsPressed(VK_CONTROL) ||
							   IsReservedShortcut(MAKELONG(pMsg->wParam, HOTKEYF_CONTROL | HOTKEYF_EXT));
					bUpdateControls = (pMsg->message == WM_KEYUP); 
					break;

				case VK_LEFT:
				case VK_RIGHT:
					bCancel = TRUE;
					bUpdateControls = (pMsg->message == WM_KEYUP); 
					break;
			}

			// special hack to detect cursor key *UP* messages 
			if (bUpdateControls)
				UpdateControls();
		}

		break;
	}
	
	return bCancel;
}

void CToDoCtrl::DrawGutterItemText(CDC* pDC, const CString& sText, const CRect& rect, int nAlign, COLORREF crText, CFont* pFont, BOOL bSymbol)
{
	if (sText.IsEmpty())
		return;
	
	CRect rText(rect);
	CPoint ptText(0, rText.top);
	
	switch (nAlign)
	{
	case DT_LEFT:
		rText.left += NCG_COLPADDING;
		break;
		
	case DT_RIGHT:
		rText.right -= NCG_COLPADDING + 1;
		break;

	case DT_CENTER:
		break;
	}
	
	COLORREF crOld = NOCOLOR;

	if (crText != NOCOLOR)
		crOld = pDC->SetTextColor(crText);

	pDC->SetBkMode(TRANSPARENT);

	if (bSymbol)
		GraphicsMisc::DrawSymbol(pDC, (char)sText[0], rect, DT_SINGLELINE | DT_VCENTER | DT_NOPREFIX | nAlign, pFont);
	else
		pDC->DrawText(sText, rText, DT_SINGLELINE | DT_VCENTER | DT_NOPREFIX | nAlign);

	if (crOld != NOCOLOR)
		pDC->SetTextColor(crOld);
}

void CToDoCtrl::DrawGutterItemDate(CDC* pDC, const COleDateTime& date, TDC_DATE nDate, const CRect& rect, COLORREF crText)
{
	if (date.m_dt == 0) 
		return; // nothing to do

	CString sDate, sTime, sDow;
		
	if (!FormatDate(date, nDate, sDate, sTime, sDow))
		return; // nothing to do

	// set text alignment to left unless the weekday is showing
	int nAlign = HasStyle(TDCS_SHOWWEEKDAYINDATES) ? TA_RIGHT : TA_LEFT;

	DrawGutterItemText(pDC, sDate, rect, nAlign, crText);
/*
	// render date, time and dow separately to ensure alignment
	// also compensate for the padding that happens inside DrawGutterItemText
	CString sDateMax, sTimeMax, sDummy;
	COleDateTime dateMax(2000, 12, 31, 23, 59, 0);
	int nSpace = pDC->GetTextExtent(_T(" ")).cx;
	
	FormatDate(dateMax, nDate, sDateMax, sTimeMax, sDummy);
	
	int nMaxDate = pDC->GetTextExtent(sDateMax).cx;
	int nMaxTime = pDC->GetTextExtent(sTimeMax).cx;
	
	// draw time
	CRect rDraw(rect);
	
	if (!sTime.IsEmpty())
		DrawGutterItemText(pDC, sTime, rDraw, TA_RIGHT, crText);
	
	// adjust rect for time
	if (!sTime.IsEmpty() || m_find.FindVisibleTaskWithTime(nDate))
	{
		rDraw.right -= nMaxTime;
		
		if (!HasStyle(TDCS_SHOWDATESINISO))
			rDraw.right -= nSpace;
	}
	
	// then date
	DrawGutterItemText(pDC, sDate, rDraw, TA_RIGHT, crText);
	
	// adjust rect for date
	rDraw.right -= (nMaxDate + nSpace);
	
	// then dow
	if (!sDow.IsEmpty())
		DrawGutterItemText(pDC, sDow, rDraw, TA_RIGHT, crText);
*/
}

BOOL CToDoCtrl::SetPriorityColors(const CDWordArray& aColors)
{
	ASSERT (aColors.GetSize() == 11);
	
	m_aPriorityColors.RemoveAll();
	
	if (aColors.GetSize() == 11)
	{
		m_aPriorityColors.Copy(aColors);
        m_cbPriority.SetColors(aColors);

		if (GetSafeHwnd())
			Invalidate();
		
		return TRUE;
	}
	
	// else
	return FALSE; // invalid combination
}

void CToDoCtrl::SetStartedTaskColors(COLORREF crStarted, COLORREF crStartedToday)
{
	m_crStarted = crStarted;
	m_crStartedToday = crStartedToday;
}

void CToDoCtrl::SetDueTaskColors(COLORREF crDue, COLORREF crDueToday)
{
	if (m_crDue != crDue)
	{
		GraphicsMisc::VerifyDeleteObject(m_brDue);

		if (crDue != NOCOLOR)
			m_brDue.CreateSolidBrush(crDue);

		m_crDue = crDue;
	}

	if (m_crDueToday != crDueToday)
	{
		GraphicsMisc::VerifyDeleteObject(m_brDueToday);

		if (crDueToday != NOCOLOR)
			m_brDueToday.CreateSolidBrush(crDueToday);

		m_crDueToday = crDueToday;
	}
}

void CToDoCtrl::SetAttributeColors(TDC_ATTRIBUTE nAttrib, const CTDCColorMap& colors)
{
	m_nColorByAttrib = nAttrib;
	m_mapAttribColors.RemoveAll();

	CString sAttrib;
	COLORREF color;
	POSITION pos = colors.GetStartPosition();

	while (pos)
	{
		colors.GetNextAssoc(pos, sAttrib, color);
		sAttrib.MakeUpper();

		m_mapAttribColors[sAttrib] = color;
	}

	if (GetSafeHwnd() && HasStyle(TDCS_COLORTEXTBYATTRIBUTE))
		Invalidate();
}

BOOL CToDoCtrl::GetAttributeColor(const CString& sAttrib, COLORREF& color) const
{
	if (!sAttrib.IsEmpty())
	{
		CString sUpper(sAttrib);
		sUpper.MakeUpper();

		if (m_mapAttribColors.Lookup(sUpper, color))
			return TRUE; 
	}
	
	// else
	return FALSE;
}

COLORREF CToDoCtrl::GetPriorityColor(int nPriority) const
{
	if (nPriority < 0 || nPriority >= m_aPriorityColors.GetSize())
		return 0;

	return (COLORREF)m_aPriorityColors[nPriority];
}

void CToDoCtrl::SetProjectName(const CString& sProjectName)
{
	if (m_sProjectName != sProjectName)
	{
		m_sProjectName = sProjectName;

		if (GetSafeHwnd())
			UpdateDataEx(this, IDC_PROJECTNAME, m_sProjectName, FALSE);
	}
}

CString CToDoCtrl::GetFriendlyProjectName(int nUntitledIndex) const
{
	CString sProjectName(m_sProjectName);
	sProjectName.TrimRight();
	
	if (sProjectName.IsEmpty())
	{
		if (!m_sLastSavePath.IsEmpty())
			sProjectName = FileMisc::GetFileNameFromPath(m_sLastSavePath, FALSE);
		else
		{
			if (nUntitledIndex > 0)
				sProjectName.Format(_T("(%s%d)"), CEnString(IDS_TDC_UNTITLEDFILE), nUntitledIndex);
			else
				sProjectName.Format(_T("(%s)"), CEnString(IDS_TDC_UNTITLEDFILE));
		}
	}
	
	return sProjectName;
}

void CToDoCtrl::OnContextMenu(CWnd* pWnd, CPoint point) 
{
	m_treeDragDrop.CancelDrag();

	// eat it if it came from us
	if (pWnd == this)
	{
		return;
	}
	else if (pWnd == &m_tree)
	{
		// check for (-1, -1)
		if (point.x == -1 && point.y == -1)
		{
			HTREEITEM htiSel = GetSelectedItem();
			CRect rItem;
			
			if (htiSel)
			{
				m_tree.TCH().EnsureVisibleEx(htiSel, FALSE);
				m_tree.GetItemRect(htiSel, rItem, TRUE);
				point = rItem.CenterPoint();
			}
			else
				point.x = point.y = 10;
			
			m_tree.ClientToScreen(&point);
		}
		else
		{
			CPoint ptTree(point);
			m_tree.ScreenToClient(&ptTree);
			
			if (ptTree.y >= 0) // == below header
			{
				if (ptTree.x < 0)
					ptTree.x = 0; // because point may be in the gutter
				
				HTREEITEM htiHit = m_tree.HitTest(ptTree);
				
				if (htiHit)
				{
					if (!Selection().HasItem(htiHit))
						SelectItem(htiHit);
					else
						m_tree.TCH().SelectItem(htiHit);
				}
				else // tree selection will have been set to NULL so we need to fix it
				{
					HTREEITEM htiAnchor = Selection().GetAnchor();
					m_tree.TCH().SelectItem(htiAnchor);
				}
			}
		}
		// fall thru to allow parent to handle
	}
	// don't forward it on if it came from the content ctrl
	else if (IsChildOrSame(m_ctrlComments, pWnd->GetSafeHwnd()))
		return;

	// else
	CRuntimeDlg::OnContextMenu(pWnd, point);
}

BOOL CToDoCtrl::PtInHeader(CPoint ptScreen) const
{
	return m_tree.PtInHeader(ptScreen);
}

TDC_COLUMN CToDoCtrl::ColumnHitTest(CPoint ptScreen) const
{
	UINT nColID = m_tree.ColumnHitTest(ptScreen);

	return (nColID == NCG_CLIENTCOLUMNID) ? TDCC_NONE : (TDC_COLUMN)nColID;
}

LRESULT CToDoCtrl::OnAutoComboAddDelete(WPARAM wp, LPARAM /*lp*/)
{
	UINT nCtrlID = LOWORD(wp);

	switch (nCtrlID)
	{
	case IDC_CATEGORY:
		UpdateTask(TDCA_CATEGORY);
		GetParent()->SendMessage(WM_TDCN_LISTCHANGE, 0, TDCA_CATEGORY);
		break;

	case IDC_TAGS:
		UpdateTask(TDCA_TAGS);
		GetParent()->SendMessage(WM_TDCN_LISTCHANGE, 0, TDCA_TAGS);
		break;

	case IDC_STATUS:
		UpdateTask(TDCA_STATUS);
		GetParent()->SendMessage(WM_TDCN_LISTCHANGE, 0, TDCA_STATUS);
		break;

	case IDC_ALLOCTO:
		UpdateTask(TDCA_ALLOCTO);
		GetParent()->SendMessage(WM_TDCN_LISTCHANGE, 0, TDCA_ALLOCTO);
		break;

	case IDC_ALLOCBY:
		UpdateTask(TDCA_ALLOCBY);
		GetParent()->SendMessage(WM_TDCN_LISTCHANGE, 0, TDCA_ALLOCBY);
		break;

	case IDC_VERSION:
		UpdateTask(TDCA_VERSION);
		GetParent()->SendMessage(WM_TDCN_LISTCHANGE, 0, TDCA_VERSION);
		break;

	default:
		if (CTDCCustomAttributeHelper::IsCustomControl(nCtrlID))
		{
			OnCustomAttributeChange(nCtrlID);
			GetParent()->SendMessage(WM_TDCN_LISTCHANGE, 0, TDCA_CUSTOMATTRIB);
		}
		else
		{
			ASSERT(0);
			return 0L;
		}
	}

	// mark ourselves as modified because we now save the lists to the taskfile
	SetModified(TRUE);

	return 0L;
}

void CToDoCtrl::OnSelChangeAllocTo()
{
	UpdateTask(TDCA_ALLOCTO);
}

void CToDoCtrl::OnSelChangeAllocBy()
{
	UpdateTask(TDCA_ALLOCBY);
}

void CToDoCtrl::OnSelChangeStatus()
{
	UpdateTask(TDCA_STATUS);
}

void CToDoCtrl::OnSelChangeVersion()
{
	UpdateTask(TDCA_VERSION);
}

void CToDoCtrl::OnSelChangeCategory()
{
	UpdateTask(TDCA_CATEGORY);
}

void CToDoCtrl::OnSelChangeTag()
{
	UpdateTask(TDCA_TAGS);
}

void CToDoCtrl::OnSelCancelAllocTo()
{
	// reset selection
	if (GetSelectedCount() > 1)
		m_cbAllocTo.SetWindowText(_T(""));
	else
	{
		HTREEITEM hti = Selection().GetFirstItem();

		if (hti)
		{
			CStringArray aTo;
			m_data.GetTaskAllocTo(GetTaskID(hti), aTo);

			m_cbAllocTo.SetChecked(aTo);
		}
	}
}

void CToDoCtrl::OnSelCancelCategory()
{
	// reset selection
	if (GetSelectedCount() > 1)
		m_cbCategory.SetWindowText(_T(""));
	else
	{
		HTREEITEM hti = Selection().GetFirstItem();

		if (hti)
		{
			CStringArray aCats;
			m_data.GetTaskCategories(GetTaskID(hti), aCats);

			m_cbCategory.SetChecked(aCats);
		}
	}
}

void CToDoCtrl::OnSelCancelTag()
{
	// reset selection
	if (GetSelectedCount() > 1)
		m_cbTags.SetWindowText(_T(""));
	else
	{
		HTREEITEM hti = Selection().GetFirstItem();

		if (hti)
		{
			CStringArray aTags;
			m_data.GetTaskTags(GetTaskID(hti), aTags);

			m_cbTags.SetChecked(aTags);
		}
	}
}

void CToDoCtrl::OnChangeTimeEstimate()
{
	UpdateTask(TDCA_TIMEEST); 
}

void CToDoCtrl::OnChangeTimeSpent()
{
	UpdateTask(TDCA_TIMESPENT); 
}

void CToDoCtrl::OnChangeExternalID()
{
	UpdateTask(TDCA_EXTERNALID); 
}

void CToDoCtrl::OnChangeCost()
{
	UpdateTask(TDCA_COST); 
}

void CToDoCtrl::OnChangeDependency()
{
	UpdateTask(TDCA_DEPENDENCY); 
}

void CToDoCtrl::OnChangeRecurrence()
{
	UpdateTask(TDCA_RECURRENCE); 
}

void CToDoCtrl::OnChangePercent()
{
	// don't handle if this was an 'auto' change
	if (!HasStyle(TDCS_AUTOCALCPERCENTDONE))
		UpdateTask(TDCA_PERCENT);
}

void CToDoCtrl::OnTreeGetInfoTip(NMHDR* pNMHDR, LRESULT* pResult)
{
	NMTVGETINFOTIP* pTVGIT = (NMTVGETINFOTIP*)pNMHDR;
	DWORD dwTaskID = (DWORD)pTVGIT->lParam;

	if (dwTaskID)
	{
		//fabio_2005
#if _MSC_VER >= 1400
		_tcsncpy_s(pTVGIT->pszText, pTVGIT->cchTextMax, FormatInfoTip(dwTaskID), pTVGIT->cchTextMax);
#else
		_tcsncpy(pTVGIT->pszText, FormatInfoTip(dwTaskID), pTVGIT->cchTextMax);
#endif

	}
	
	*pResult = 0;
}

CString CToDoCtrl::FormatInfoTip(DWORD dwTaskID) const
{
	const TODOITEM* pTDI = m_data.GetTask(dwTaskID);
	ASSERT(pTDI);

	CString sTip;
	
	if (pTDI)
	{
		// format text multilined
		sTip.Format(_T("Task:\t\t %s"),	pTDI->sTitle);

		CString sComments = pTDI->sComments;
		int nLen = sComments.GetLength();
		
		if (nLen && m_nMaxInfotipCommentsLength != 0)
		{
			if (m_nMaxInfotipCommentsLength > 0 && nLen > m_nMaxInfotipCommentsLength)
				sComments = sComments.Left(m_nMaxInfotipCommentsLength) + MISC_ELLIPSIS;

			sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_COMMENTS), sComments);
		}
		
		if (IsColumnShowing(TDCC_STATUS) && !pTDI->sStatus.IsEmpty())
			sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_STATUS), pTDI->sStatus);
		
		if (IsColumnShowing(TDCC_CATEGORY) && pTDI->aCategories.GetSize())
			sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_CATEGORY), m_data.FormatTaskCategories(pTDI));
		
		if (IsColumnShowing(TDCC_TAGS) && pTDI->aTags.GetSize())
			sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_TAGS), m_data.FormatTaskTags(pTDI));
		
		if (IsColumnShowing(TDCC_ALLOCTO) && pTDI->aAllocTo.GetSize())
			sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_ALLOCTO), m_data.FormatTaskAllocTo(pTDI));
		
		if (IsColumnShowing(TDCC_ALLOCBY) && !pTDI->sAllocBy.IsEmpty())
			sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_ALLOCBY), pTDI->sAllocBy); 
		
		if (IsColumnShowing(TDCC_VERSION) && !pTDI->sVersion.IsEmpty())
			sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_VERSION), pTDI->sVersion); 

		DWORD dwDateFmt = HasStyle(TDCS_SHOWDATESINISO) ? DHFD_ISO : 0;
		
		if (pTDI->IsDone())
			sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_DONEDATE), CDateHelper::FormatDate(pTDI->dateDone, dwDateFmt));
		else
		{
			if (IsColumnShowing(TDCC_PRIORITY) && pTDI->nPriority != FM_NOPRIORITY)
				sTip += CEnString(_T("\n%s %d"), CEnString(IDS_TDCTIP_PRIORITY), pTDI->nPriority);
			
			if (IsColumnShowing(TDCC_RISK) && pTDI->nRisk != FM_NORISK)
				sTip += CEnString(_T("\n%s %d"), CEnString(IDS_TDCTIP_RISK), pTDI->nRisk);
			
			if (IsColumnShowing(TDCC_STARTDATE) && pTDI->HasStart())
				sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_STARTDATE), CDateHelper::FormatDate(pTDI->dateStart, dwDateFmt));
			
			if (IsColumnShowing(TDCC_DUEDATE) && pTDI->HasDue())
			{
				if (pTDI->HasDueTime())
					dwDateFmt |= DHFD_TIME | DHFD_NOSEC;

				sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_DUEDATE), CDateHelper::FormatDate(pTDI->dateDue, dwDateFmt));
			}
			
			if (IsColumnShowing(TDCC_PERCENT))
				sTip += CEnString(_T("\n%s %d"), CEnString(IDS_TDCTIP_PERCENT), m_data.CalcPercentDone(dwTaskID));
			
			if (IsColumnShowing(TDCC_TIMEEST))
				sTip += CEnString(_T("\n%s %.2f %c"), CEnString(IDS_TDCTIP_TIMEEST), m_data.CalcTimeEstimate(dwTaskID, TDITU_HOURS), TIME_HOUR_ABBREV);
			
			if (IsColumnShowing(TDCC_TIMESPENT))
				sTip += CEnString(_T("\n%s %.2f %c"), CEnString(IDS_TDCTIP_TIMESPENT), m_data.CalcTimeSpent(dwTaskID, TDITU_HOURS), TIME_HOUR_ABBREV);
		}
		
		if (IsColumnShowing(TDCC_COST))
			sTip += CEnString(_T("\n%s %.2f"), CEnString(IDS_TDCTIP_COST), m_data.CalcCost(dwTaskID));
		
		if (IsColumnShowing(TDCC_DEPENDENCY))
		{
			if (pTDI->aDependencies.GetSize())
				sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_DEPENDS), Misc::FormatArray(pTDI->aDependencies));
			
			CDWordArray aDependents;
			
			if (m_data.GetTaskDependents(dwTaskID, aDependents))
				sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_DEPENDENTS), Misc::FormatArray(aDependents));
		}
		
		if (IsColumnShowing(TDCC_FILEREF) && !pTDI->sFileRefPath.IsEmpty())
			sTip += CEnString(_T("\n%s %s"), CEnString(IDS_TDCTIP_FILEREF), pTDI->sFileRefPath);
		
		if (pTDI->dateLastMod > 0)
			sTip += CEnString(_T("\n%s %s (%s)"), CEnString(IDS_TDCTIP_LASTMOD), pTDI->dateLastMod.Format(VAR_DATEVALUEONLY), pTDI->dateLastMod.Format(VAR_TIMEVALUEONLY));

	}

	return sTip;
}

BOOL CToDoCtrl::CopyCurrentSelection() const
{
	ClearCopiedItem();

	// copy selected tasks to clipboard
	TDCGETTASKS filter(TDCGT_ALL, TDCGTF_FILENAME);
	CTaskFile tasks;

	if (!GetSelectedTasks(tasks, filter))
		return FALSE;
	
	CTaskClipboard::SetTasks(tasks, GetClipboardID());

	// also copy selected task titles to clipboard in order 
	CHTIList selection;
	Selection().CopySelection(selection);
	CTreeSelectionHelper::OrderSelection(selection, m_tree);

	CString sTitles;
	POSITION pos = selection.GetHeadPosition();
	
	while (pos)
	{
		HTREEITEM hti = selection.GetNext(pos);
		
		sTitles += GetTaskTitle(GetTaskID(hti));
		sTitles += "\n";
	}
	
	Misc::CopyTexttoClipboard(sTitles, *this, 0, FALSE);
	
	return TRUE;
}

BOOL CToDoCtrl::IsClipboardEmpty(BOOL bCheckID) const
{
	if (CTaskClipboard::IsEmpty())
		return TRUE;

	else if (bCheckID)
		return !CTaskClipboard::ClipIDMatches(GetClipboardID());

	//else
	return FALSE;
}

BOOL CToDoCtrl::CopySelectedTask() const
{
	return CopyCurrentSelection();
}

BOOL CToDoCtrl::CutSelectedTask()
{
	if (CopyCurrentSelection())
	{
		IMPLEMENT_UNDO(TDCUAT_DELETE);

		DeleteSelectedTask(FALSE, TRUE);
		return TRUE;
	}
	
	return FALSE;
}

BOOL CToDoCtrl::CanPaste() const
{
	return (!IsReadOnly() && !IsClipboardEmpty() && GetSelectedCount() <= 1);
}

void CToDoCtrl::ClearCopiedItem() const
{
	CTaskClipboard::Reset();
}

BOOL CToDoCtrl::PasteTasks(TDC_PASTE nWhere)
{
	if (GetSelectedCount() > 1 || !CanPaste() || IsClipboardEmpty(FALSE))
		return FALSE;
	
	CTaskFile tasks;
	CString sClipID = GetClipboardID();

	if (CTaskClipboard::GetTasks(tasks, sClipID))
	{
		HTREEITEM htiDest = NULL, htiDestAfter = NULL;

		switch (nWhere)
		{
		case TDCP_ONSELTASK:
			htiDest = m_tree.GetSelectedItem();
			htiDestAfter = TVI_FIRST;
			break;

		case TDCP_BELOWSELTASK:
			htiDestAfter = m_tree.GetSelectedItem();
			
			if (!htiDestAfter)
				htiDestAfter = TVI_LAST;
			else
				htiDest = m_tree.GetParentItem(htiDestAfter);
			break;

		case TDCP_ATBOTTOM:
			htiDestAfter = TVI_LAST;
			break;
		}
			
		// reset the task IDs if the tasks did *not* originate from us
		// and we're not empty
		TDC_RESETIDS nResetID = TDCR_YES;
		
		if (CTaskClipboard::ClipIDMatches(sClipID))
			nResetID = TDCR_CHECK;

		else if (GetTaskCount() == 0)
			nResetID = TDCR_NO;
	
		// and fix up the dependencies of the tasks
		PrepareTaskIDsForPaste(tasks, nResetID);

		IMPLEMENT_UNDO(TDCUAT_PASTE);
		HOLD_REDRAW(*this, m_tree);

		// no need to re-check IDs as we've already done it
		if (AddTasksToTree(tasks, htiDest, htiDestAfter, TDCR_NO))
		{
			FixupParentCompletion(htiDest);
			return TRUE;
		}
	}

   // else
   return FALSE;
}

void CToDoCtrl::FixupParentCompletion(HTREEITEM htiParent)
{
   // if the parent was marked as done and this task is not
   // then mark the parent as incomplete too
   if (htiParent)
   {
      DWORD dwParentID = m_find.GetTaskID(htiParent);

	  if (!dwParentID) // top level item 
         return;

      const TODOITEM* pTDI = m_data.GetTask(dwParentID);
      ASSERT(pTDI);
      
      if (pTDI && pTDI->IsDone())
      {
         if (m_selection.ItemHasIncompleteSubtasks(htiParent))
         {
            // work our way up the chain setting parent to incomplete
            do 
            {
               m_data.SetTaskDate(dwParentID, TDCD_DONE, 0.0);
               m_tree.TCH().SetItemChecked(htiParent, FALSE);
               
               htiParent = m_tree.GetParentItem(htiParent);
               dwParentID = htiParent ? GetTaskID(htiParent) : 0;
               
            } 
            while(dwParentID);
         }
      }
   }
}

BOOL CToDoCtrl::AddTasksToTree(const CTaskFile& tasks, HTREEITEM htiDest, HTREEITEM htiDestAfter, TDC_RESETIDS nResetIDs)
{
	if (!htiDest)
		htiDest = TVI_ROOT;
	
	HTASKITEM hTask = tasks.GetFirstTask();
	
	if (!hTask)
		return FALSE;
	
	// cache and clear current selection
	DWORD dwSelID = GetTaskID(Selection().GetFirstItem());

	m_tree.TCH().SelectItem(NULL);
	Selection().RemoveAll();
	
	while (hTask)
	{
		htiDestAfter = AddTaskToTreeItem(tasks, hTask, htiDest, htiDestAfter, nResetIDs);
		
		// add item to selection
		Selection().AddItem(htiDestAfter);
		
		// next task
		hTask = tasks.GetNextTask(hTask);
	}
	
	m_tree.Expand(htiDest, TVE_EXPAND);
	
	if (GetSelectedCount())
		m_tree.TCH().SelectItem(Selection().GetFirstItem());
	else
		SelectTask(dwSelID);
	
	SetModified(TRUE, TDCA_NEWTASK, 0);
	UpdateControls();
	
	return TRUE;
}

LRESULT CToDoCtrl::OnGutterNotifyItemClick(WPARAM /*wParam*/, LPARAM lParam)
{
	NCGITEMCLICK* pNGIC = (NCGITEMCLICK*)lParam;
	HTREEITEM htiHit = (HTREEITEM)pNGIC->dwItem;
	HTREEITEM htiPrev = (HTREEITEM)pNGIC->dwPrevItem;

	LRESULT lr = 0;

	if (htiHit)
	{
		UINT nHitFlags = 0;
		m_tree.HitTest(pNGIC->ptClick, &nHitFlags);
		
		switch (pNGIC->nMsgID)
		{
		case WM_LBUTTONDOWN:
			{
				ProcessItemLButtonDown(htiHit, nHitFlags, TDCC_CLIENT);

				m_selection.nLBtnDownFlags = nHitFlags;
				m_selection.nNcLBtnDownColID = -1;

				lr = 1L;  // we handled it
			}
			break;
			
		case WM_NCLBUTTONDOWN:
			{
				ProcessItemLButtonDown(htiHit, 0, (TDC_COLUMN)pNGIC->nColID);
	
				m_selection.nNcLBtnDownColID = pNGIC->nColID;
				m_selection.nLBtnDownFlags = 0;

				lr = 1L;  // we handled it
			}
			break;

			// we handle these in the event that they weren't handled in the 
			// DOWN message handlers above and so long as we're still over
			// the same pos
		case WM_LBUTTONUP:
			{
				if (htiHit == htiPrev && nHitFlags == m_selection.nLBtnDownFlags)
					ProcessItemLButtonUp(htiHit, nHitFlags, TDCC_CLIENT);

				m_selection.nLBtnDownFlags = 0;
				lr = 1L;  // we handled it
			}
			break;
			
		case WM_NCLBUTTONUP:
			{
				if (htiHit == htiPrev && pNGIC->nColID == m_selection.nNcLBtnDownColID)
				{
					ProcessItemLButtonUp(htiHit, 0, (TDC_COLUMN)pNGIC->nColID);
				}
				// else if we were dragging but we're not anymore and
				// the items are different then shift the focus back to the prev item
				else if (htiHit != htiPrev && !m_treeDragDrop.IsDragging())
				{
					// post this message so that the tree's finished its stuff first
					PostMessage(WM_TDC_RESTOREFOCUSEDITEM, m_tree.GetDlgCtrlID(), (LPARAM)htiPrev);
					
					// update anchor
					Selection().SetAnchor(htiPrev);
				}

				m_selection.nNcLBtnDownColID = -1;
				lr = 1L;  // we handled it
			}
			break;
			
		case WM_RBUTTONDOWN:
		case WM_NCRBUTTONDOWN:
			// if the hit item is not selected then 
			// select it first
			if (!IsItemSelected(htiHit))
			{
				SelectItem(htiHit);
				lr = 1L;  // we handled it
			}
			break;

		case WM_LBUTTONDBLCLK:
			if (htiHit && pNGIC->nColID == TDCC_CLIENT && !IsReadOnly())
			{
				// if full row selection is turned on user can double click anywhere 
				// in 'client' column else must be on the item label.
				// Also allow icon editing if double-clicking on icon
				if (!m_tree.ItemHasChildren(htiHit))
				{
					BOOL bHitIcon = (nHitFlags & TVHT_ONITEMICON);

					if (bHitIcon)
						EditSelectedTaskIcon();
					else
					{
						BOOL bHitLabel = (nHitFlags & TVHT_ONITEMLABEL);
						BOOL bFullRow = HasStyle(TDCS_FULLROWSELECTION);

						if (bHitLabel || bFullRow)
							EditSelectedTask(FALSE);
					}
				}
			}
			break;

		case WM_NCLBUTTONDBLCLK:
			if (htiHit)
			{
				switch (pNGIC->nColID)
				{
				// open file links
				case TDCC_FILEREF:
					{
						CString sFile = m_data.GetTaskFileRef(GetTaskID(htiHit));
						
						if (!sFile.IsEmpty())
						{
							GotoFile(sFile);
							return TRUE;
						}
					}
					break;
					
				case TDCC_DEPENDENCY:
					{
						CStringArray aDepends;
						m_data.GetTaskDependencies(GetTaskID(htiHit), aDepends);
						
						if (aDepends.GetSize())
						{
							ShowTaskLink(0, aDepends[0]);
							return TRUE;
						}
					}
					break;
					
				case TDCC_RECURRENCE:
					m_eRecurrence.DoEdit();
					return TRUE; // eat it
					
// 				case TDCC_ICON:
// 					EditSelectedTaskIcon();
// 					return TRUE; // eat it

				case TDCC_REMINDER:
					AfxGetMainWnd()->SendMessage(WM_TDCN_DOUBLECLKREMINDERCOL);
					return TRUE; // eat it
				}
			}
			break;
		}
	}
	else
	{
		// make sure the tree has something properly selected
		PostMessage(WM_TDC_RESTOREFOCUSEDITEM, m_tree.GetDlgCtrlID(), (LPARAM)GetSelectedItem());
	}

	m_treeDragDrop.EnableDragDrop(!IsReadOnly() && GetSelectedCount());
	
	return lr;
}

void CToDoCtrl::ProcessItemLButtonDown(HTREEITEM htiHit, int nHitFlags, TDC_COLUMN nColID)
{
	BOOL bCtrl = Misc::KeyIsPressed(VK_CONTROL);
	BOOL bShift = Misc::KeyIsPressed(VK_SHIFT);
	
	HTREEITEM htiAnchor = Selection().GetAnchor();
	
	if (!htiAnchor && bShift)
		htiAnchor = htiHit;
	
	if (bCtrl)
	{
		// don't change the selection if the user is clicking on
		// an item's fileref
		if (nColID == TDCC_FILEREF)
			return;

		if (bShift)
			MultiSelectItems(htiAnchor, htiHit, TSHS_SELECT);
		else
		{
			// if the item clicked is already selected and there are no
			// other selected items then assume (for now) that the user
			// is going to perform a drag rather than a toggle
			if (GetSelectedCount() != 1 || !Selection().HasItem(htiHit))
				MultiSelectItem(htiHit, TSHS_TOGGLE);
		}
		
		// save item handle so we don't rehandle in LButtonUp handler
		m_selection.htiLastHandledLBtnDown = htiHit;
	}
	else if (bShift) 
	{
		Selection().RemoveAll();
		MultiSelectItems(htiAnchor, htiHit, TSHS_SELECT);
		
		// save item handle so we don't rehandle in LButtonUp handler
		m_selection.htiLastHandledLBtnDown = htiHit;
	}
	else if (!Selection().HasItem(htiHit))
	{
		// if the user clicked on the tree item itself then
		// don't call SelectItem here else in combination with the 
		// tree's own handling of this message it can trigger a label edit
		if (nHitFlags & TVHT_ONITEM)
		{
			Selection().RemoveAll();
			Selection().AddItem(htiHit);

			// notify parent
			GetParent()->PostMessage(WM_TDCN_SELECTIONCHANGE);
		}
		else
			SelectItem(htiHit);
	
		// save item handle so we don't rehandle in LButtonUp handler
		m_selection.htiLastHandledLBtnDown = htiHit;
	}
	
	// update anchor
	if (!bShift)
		Selection().SetAnchor(htiHit);
}

UINT CToDoCtrl::MapColumnToCtrlID(TDC_COLUMN nColID) const
{
	int nCtrl;
	for (nCtrl = 0; nCtrl < NUM_CTRLITEMS; nCtrl++)
	{
		const CTRLITEM& ctrl = CTRLITEMS[nCtrl];

		if (ctrl.nCol == nColID)
			return ctrl.nCtrlID;
	}

	for (nCtrl = 0; nCtrl < m_aCustomControls.GetSize(); nCtrl++)
	{
		const CTRLITEM& ctrl = m_aCustomControls[nCtrl];

		if (ctrl.nCol == nColID)
			return ctrl.nCtrlID;
	}

	// pick up any stragglers
	switch (nColID)
	{
	case TDCC_DONE:
		return IDC_DONEDATE;

	case TDCC_TRACKTIME:
		return IDC_TIMESPENT;
	}

	ASSERT(0);
	return 0L;
}

TDC_COLUMN CToDoCtrl::MapCtrlIDToColumn(UINT nCtrlID) const
{
	int nCtrl;
	for (nCtrl = 0; nCtrl < NUM_CTRLITEMS; nCtrl++)
	{
		const CTRLITEM& ctrl = CTRLITEMS[nCtrl];

		if (ctrl.nCtrlID == nCtrlID)
			return ctrl.nCol;
	}

	for (nCtrl = 0; nCtrl < m_aCustomControls.GetSize(); nCtrl++)
	{
		const CTRLITEM& ctrl = m_aCustomControls[nCtrl];

		if (ctrl.nCtrlID == nCtrlID)
			return ctrl.nCol;
	}

	// pick up any stragglers
	switch (nCtrlID)
	{
	case IDC_DONEDATE:
		return TDCC_DONE;

	case IDC_TIMESPENT:
		return TDCC_TRACKTIME;
	}

//	ASSERT(0);
	return TDCC_NONE;
}

BOOL CToDoCtrl::HandleCustomColumnClick(TDC_COLUMN nColID)
{
	if (!CTDCCustomAttributeHelper::IsCustomColumn(nColID))
		return FALSE;

	TDCCUSTOMATTRIBUTEDEFINITION attribDef;
	VERIFY (CTDCCustomAttributeHelper::GetAttributeDef(nColID, m_aCustomAttribDefs, attribDef));
	
	TDCCADATA data(GetSelectedTaskCustomAttributeData(attribDef.sUniqueID));
	
	switch (attribDef.GetDataType())
	{
	case TDCCA_BOOL:
		// toggle the flag state
		if (data.AsBool())
		{
			data.Set(false);
		}
		else
		{
			TCHAR nChar = 0;

			if (attribDef.aDefaultListData.GetSize())
				nChar = attribDef.aDefaultListData[0][0];
		
			data.Set(true, nChar);
		}
		SetSelectedTaskCustomAttributeData(attribDef.sUniqueID, data.AsString());
		break;
		
	case TDCCA_ICON:
		if (attribDef.IsList())
		{
			CString sImage, sDummy, sTag = attribDef.GetNextListItem(data.AsString());
			
			if (sTag.IsEmpty() || attribDef.DecodeImageTag(sTag, sImage, sDummy))
				SetSelectedTaskCustomAttributeData(attribDef.sUniqueID, sImage);
		}
		else
		{
			CTDLTaskIconDlg dialog(m_ilTaskIcons, data.AsString());
			
			if (dialog.DoModal() == IDOK)
				SetSelectedTaskCustomAttributeData(attribDef.sUniqueID, dialog.GetIconName());
		}
		break;
		
	default:
		break;
	}

	return TRUE;
}

void CToDoCtrl::ProcessItemLButtonUp(HTREEITEM htiHit, int nHitFlags, TDC_COLUMN nColID)
{
	BOOL bCtrl = Misc::KeyIsPressed(VK_CONTROL);
	BOOL bShift = Misc::KeyIsPressed(VK_SHIFT);
	BOOL bAlt = Misc::KeyIsPressed(VK_MENU);

	// if the alt key is down then move focus to edit field corresponding to this column
	if (bAlt)
	{
		UINT nCtrlID = MapColumnToCtrlID(nColID);

		if (nCtrlID)
		{
			// make sure the edit controls are visible
			if (m_nMaxState != TDCMS_NORMAL)
				SetMaximizeState(TDCMS_NORMAL);

			GetDlgItem(nCtrlID)->SetFocus();
		}

		return;
	}

	DWORD dwHitID = GetTaskID(htiHit);
	
	// if control is down and we're clicking on the file ref
	// then always display the file ref if its got one
	if (bCtrl && nColID == TDCC_FILEREF)
	{
		CString sFile = m_data.GetTaskFileRef(dwHitID);
		
		if (!sFile.IsEmpty())
			GotoFile(sFile);
	}
	else if (bCtrl && nColID == TDCC_DEPENDENCY)
	{
		CStringArray aDepends;
		m_data.GetTaskDependencies(dwHitID, aDepends);
		
		if (aDepends.GetSize())
			ShowTaskLink(0, aDepends[0]);
	}
	else if (!IsReadOnly())
	{
		// if the button was over the checkbox and the button down
		// was too then set the appropriate done state
		if ((nHitFlags & TVHT_ONITEMSTATEICON) || nColID == TDCC_DONE)
		{
			SetSelectedTaskDone(!m_data.IsTaskDone(dwHitID));
			return;
		}
		else
		{
			switch (nColID)
			{
			case TDCC_TRACKTIME:
				if (GetSelectedCount() == 1 && IsItemSelected(htiHit) && m_data.IsTaskTimeTrackable(dwHitID))
					TimeTrackTask(htiHit);
				return;
			
			case TDCC_FLAG:
				SetSelectedTaskFlag(!m_data.IsTaskFlagged(dwHitID));
				return;

			case TDCC_ICON:
				EditSelectedTaskIcon();
				return;
			}
			
			// try custom columns
			if (HandleCustomColumnClick(nColID))
				return;
		}
	}
	
	// don't reprocess clicks that were processes in the button down handler
	if (!htiHit || htiHit == m_selection.htiLastHandledLBtnDown)
	{
		m_selection.htiLastHandledLBtnDown = NULL;
		return; // handled in button down
	}

	// if ctrl or shift are not pressed and we've 
	// only got one item selected and its the same 
	// as htiHit and htiPrev then we've nothing to do
	// else clear all and select the hit item
	if (!bCtrl && !bShift)
	{
		ASSERT (GetSelectedCount());
		
		if (GetSelectedCount() > 1 || GetSelectedItem() != htiHit)
		{
			// if the user clicked on the tree item itself then
			// don't call SelectItem here else in combination with the 
			// tree's own handling of this message it can trigger a label edit
			if (nHitFlags & TVHT_ONITEM)
			{
				Selection().RemoveAll();
				Selection().AddItem(htiHit);

				m_tree.RedrawGutter();

				UpdateControls(htiHit);
			}
			else
				SelectItem(htiHit);
		}
		
		// update anchor
		Selection().SetAnchor(htiHit);
	}
}

void CToDoCtrl::TimeTrackTask(HTREEITEM hti)
{
	ASSERT (GetSelectedCount() == 1); // sanity check
	
	DWORD dwTaskID = GetTrueTaskID(hti), dwPrevTaskID = 0;

	if (dwTaskID == m_dwTimeTrackTaskID)
		EndTimeTracking(); // toggle off
	else
	{
		dwPrevTaskID = m_dwTimeTrackTaskID;
	
		if (m_data.IsTaskTimeTrackable(dwTaskID))
		{
			// if there's a current task being tracked then end it
			EndTimeTracking();

			TODOITEM* pTDI = m_data.GetTask(dwTaskID);
			ASSERT (pTDI);

			if (pTDI)
			{
				m_dwTimeTrackTaskID = dwTaskID;
				m_dwTickLast = GetTickCount();
				
				// if the task's start date has not been set then set it now
				if (pTDI->dateStart == 0)
					pTDI->dateStart = floor(COleDateTime::GetCurrentTime());

				// notify parent
				GetParent()->SendMessage(WM_TDCN_TIMETRACK, GetDlgCtrlID(), TRUE);

				// resort if appropriate
				if (!IsMultiSorting() && m_sort.nBy1 == TDCC_TRACKTIME)
					Sort(TDCC_TRACKTIME, FALSE);
			}
		}
	}
	
	if (hti == GetSelectedItem())
		UpdateControls();
	
	if (IsColumnShowing(TDCC_TRACKTIME) || IsColumnShowing(TDCC_TIMESPENT))
	{
		// redraw whole view if we have a previous ID
		// or if this item has references
		if (dwPrevTaskID || m_data.TaskHasReferences(dwTaskID))
			m_tree.RedrawGutter();
		else
			m_tree.RedrawGutterItem((DWORD)hti);
	}
}

void CToDoCtrl::EndTimeTracking()
{
	// if there's a current task being tracked then log it
	if (m_dwTimeTrackTaskID)
	{
		// add whatever time is still waiting
		IncrementTrackedTime();
		
		// log it
		if (HasStyle(TDCS_LOGTIMETRACKING))
		{
			CTaskTimeLog log(GetFilePath(), GetSaveAs(m_bIsUnicode));

			if (!log.LogTime(m_dwTimeTrackTaskID, GetTaskTitle(m_dwTimeTrackTaskID), 
						m_dLogTime, HasStyle(TDCS_LOGTASKTIMESEPARATELY)))
			{
				AfxMessageBox(IDS_LOGFILELOCKED);
			}
		}

		// reset taskID and time
		m_dwTickLast = 0;
		m_dwTimeTrackTaskID = 0;
		m_dLogTime = 0;

		// notify parent
		if (GetSafeHwnd())
			GetParent()->SendMessage(WM_TDCN_TIMETRACK, GetDlgCtrlID(), FALSE);
	}
}

void CToDoCtrl::AddTimeToTaskLogFile(DWORD dwTaskID, double dHours, const COleDateTime& dtWhen, BOOL bAddToTimeSpent)
{
	if (!HasStyle(TDCS_LOGTIMETRACKING) || dHours == 0.0)
		return; // sanity check

	CTaskTimeLog log(GetFilePath());
	
	if (!log.LogTime(dwTaskID, GetTaskTitle(dwTaskID), dHours, dtWhen, HasStyle(TDCS_LOGTASKTIMESEPARATELY)))
	{
		UpdateWindow();
		AfxMessageBox(IDS_LOGFILELOCKED);
	}
	else if (bAddToTimeSpent)
	{
		TCHAR nUnits = 0;
		double dTime = m_data.GetTaskTimeSpent(dwTaskID, nUnits);

		dTime += CTimeHelper().GetTime(dHours, THU_HOURS, nUnits);

		if (GetSelectedCount() == 1 && GetSelectedTaskID() == dwTaskID)
			SetSelectedTaskTimeSpent(dTime, nUnits);
		else
		{
			m_data.SetTaskTimeSpent(dwTaskID, dTime, nUnits);
			SetModified(TRUE, TDCA_TIMESPENT, dwTaskID);
		}
	}
}

CString CToDoCtrl::GetSelectedTaskTimeLogPath() const
{
	DWORD dwTaskID = GetSelectedTaskID();

	if (dwTaskID)
		return CTaskTimeLog(GetFilePath()).GetLogPath(dwTaskID, HasStyle(TDCS_LOGTASKTIMESEPARATELY));

	// else
	return _T("");
}

LRESULT CToDoCtrl::OnTreeRestoreFocusedItem(WPARAM wParam, LPARAM lParam)
{
	if (wParam == IDC_TASKLIST)
		m_tree.TCH().SelectItem((HTREEITEM)lParam);
	
	return 0L;
}

LRESULT CToDoCtrl::OnGetFont(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
	return (LRESULT)::SendMessage(::GetParent(*this), WM_GETFONT, 0, 0);
}

BOOL CToDoCtrl::IsTaskRecurring(DWORD dwTaskID) const
{
	TDIRECURRENCE trTask;
	
	if (m_data.GetTaskRecurrence(dwTaskID, trTask))
		return trTask.nRegularity != TDIR_ONCE;

	return FALSE;
}

BOOL CToDoCtrl::MultiSelectItems(const CDWordArray& aTasks, TSH_SELECT nState, BOOL bRedraw)
{
	HandleUnsavedComments();

	// clear existing selection and add to history
	Selection().RemoveAll();

	// select tasks in one hit
	int nTask = aTasks.GetSize();
	BOOL bSel = FALSE;

	while (nTask--)
	{
		HTREEITEM hti = m_find.GetItem(aTasks[nTask]);
		
		if (hti)
			bSel |= Selection().SetItem(hti, nState, bRedraw);
	}

	if (bSel || aTasks.GetSize() == 0)
	{
		// avoid calling UpdateControls again if it was called 
		// because the tree selection changed inside FixupTreeSelection
		BOOL bTreeSelChanged = Selection().FixupTreeSelection();

		if (bRedraw)
			m_tree.RedrawGutter();
		
		if (!bTreeSelChanged)
			UpdateControls(); // load newly selected item

		UpdateSelectedTaskPath();
		
		// re-enable dragdrop
		m_treeDragDrop.EnableDragDrop(!IsReadOnly() && GetSelectedCount());

		// notify parent
		GetParent()->PostMessage(WM_TDCN_SELECTIONCHANGE);
	}

	// else
	return bSel;
}

BOOL CToDoCtrl::MultiSelectItem(HTREEITEM hti, TSH_SELECT nState, BOOL bRedraw) 
{ 
	HandleUnsavedComments();

	if (Selection().SetItem(hti, nState, bRedraw))
	{
		if (bRedraw)
			m_tree.RedrawGutter();
		
		UpdateControls(); // load newly selected item
		UpdateSelectedTaskPath();
		
		// re-enable dragdrop
		m_treeDragDrop.EnableDragDrop(!IsReadOnly() && GetSelectedCount());

		// notify parent
		GetParent()->PostMessage(WM_TDCN_SELECTIONCHANGE);
		
		return TRUE;
	}
	
	return FALSE;
}

BOOL CToDoCtrl::MultiSelectItems(HTREEITEM htiFrom, HTREEITEM htiTo, TSH_SELECT nState, BOOL bRedraw)
{
	HandleUnsavedComments();

	if (Selection().SetItems(htiFrom, htiTo, nState, bRedraw))
	{
		// avoid calling UpdateControls again if it was called 
		// because the tree selection changed inside FixupTreeSelection
		BOOL bTreeSelChanged = Selection().FixupTreeSelection();

		if (bRedraw)
			m_tree.RedrawGutter();
		
		if (!bTreeSelChanged)
			UpdateControls(); // load newly selected items

		UpdateSelectedTaskPath();
		
		// re-enable dragdrop
		m_treeDragDrop.EnableDragDrop(!IsReadOnly() && GetSelectedCount());

		// notify parent
		GetParent()->PostMessage(WM_TDCN_SELECTIONCHANGE);
		
		return TRUE;
	}
	
	return FALSE;
}

BOOL CToDoCtrl::SelectTask(DWORD dwTaskID) 
{ 
	HTREEITEM hti = m_find.GetItem(dwTaskID);

	if (hti)
		SelectItem(hti);

	return (hti != NULL);
}

void CToDoCtrl::SelectItem(HTREEITEM hti) 
{ 
	HandleUnsavedComments();

	if (m_tree.GetSafeHwnd()) 
	{
		// we're going to try to prevent unnecessary calls to UpdateControls 
		// mainly because if the last selected task has a big comment then
		// it creates an appreciable delay
		if (GetSelectedCount() == 1 && GetSelectedItem() == m_tree.GetSelectedItem() &&
			hti == GetSelectedItem())
			return;

		// This is a nasty hack until I can figure out a better method.
		// The problem is that the auto droplists (category, status, alloc to/by)
		// rely on focus changes to get them to update, and if any of the
		// Goto..Task() methods are called there is no change of focus.
		HWND hFocus = ::GetFocus();

		if (IsChildOrSame(GetSafeHwnd(), hFocus))
			::SendMessage(hFocus, WM_KILLFOCUS, 0, 0);

		if (Selection().RemoveAll() && !hti)
			m_tree.RedrawGutter();
		
		if (hti)
		{
			Selection().AddItem(hti);
			Selection().SetAnchor(hti);
			
			if (!m_tree.TCH().SelectItem(hti))
				UpdateControls(); // else tree sel change will 

			m_tree.TCH().EnsureVisibleEx(hti, FALSE);
		}
		else
			UpdateControls(); // disable controls
		
		UpdateSelectedTaskPath();
		
		m_treeDragDrop.EnableDragDrop(!IsReadOnly());
		::SendMessage(hFocus, WM_SETFOCUS, 0, 0);

		// notify parent
		GetParent()->PostMessage(WM_TDCN_SELECTIONCHANGE);
	}
}

void CToDoCtrl::DeselectAll() 
{ 
	HandleUnsavedComments();

	if (Selection().RemoveAll())
	{

		m_tree.RedrawGutter();
		
		UpdateControls(); // load newly selected item
		UpdateSelectedTaskPath();

		// notify parent
		GetParent()->PostMessage(WM_TDCN_SELECTIONCHANGE);
	}
	
	// re-enable dragdrop
	m_treeDragDrop.EnableDragDrop(FALSE);
}

void CToDoCtrl::SelectAll() 
{ 
	HandleUnsavedComments();

	if (Selection().AddAll())
	{
		m_tree.RedrawGutter();
		
		UpdateControls(); // load newly selected item
		UpdateSelectedTaskPath();

		// notify parent
		GetParent()->PostMessage(WM_TDCN_SELECTIONCHANGE);
	}
	
	// re-enable dragdrop
	m_treeDragDrop.EnableDragDrop(FALSE);
}

LRESULT CToDoCtrl::OnGutterNotifyScroll(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
	m_eTaskName.EndEdit(FALSE);
	return 0L;
}

LRESULT CToDoCtrl::OnGutterNotifyHeaderClick(WPARAM /*wParam*/, LPARAM lParam)
{
	NCGHDRCLICK* pNGHC = (NCGHDRCLICK*)lParam;
	
	if (pNGHC->nMsgID != WM_NCLBUTTONUP)
	{
		pNGHC->bPressed = FALSE;
		return TRUE;
	}
	
	TDC_COLUMN nPrev = m_sort.nBy1;
	TDC_COLUMN nSortBy = TDCC_NONE;
	
	// convert index to TDC_COLUMN
	TDC_COLUMN nColID = (TDC_COLUMN)pNGHC->nColID;

	// check for visibility
	if (!IsColumnShowing(nColID))
		return TRUE; 

	// check for default attribute
	TDCCOLUMN* pTDCC = GetColumn(nColID);
	
	// could also be a custom column
	if (pTDCC && pTDCC->bSortable)
		nSortBy = nColID;
		
	else if (!pTDCC && CTDCCustomAttributeHelper::IsColumnSortable(nColID, m_aCustomAttribDefs))
		nSortBy = nColID;

	// do the sort
	if (nSortBy != TDCC_NONE)
	{
		Sort(nSortBy);
		pNGHC->bPressed = FALSE;
		
		// notify parent
		GetParent()->SendMessage(WM_TDCN_SORT, GetDlgCtrlID(), MAKELPARAM((WORD)nPrev, (WORD)nSortBy));
		
		return TRUE; // we handled it
	}
	
	return 0;
}

LRESULT CToDoCtrl::OnGutterRecalcColWidth(WPARAM /*wParam*/, LPARAM lParam)
{
	NCGRECALCCOLUMN* pNCRC = (NCGRECALCCOLUMN*)lParam;

	TDC_COLUMN nColID = (TDC_COLUMN)pNCRC->nColID;

	// let tree do default handling for 'pos' column because 
	// this also handles alternate line coloring
	if (nColID == OTC_POSCOLUMNID)
		return FALSE;

	// handle hidden columns
	if (!IsColumnShowing(nColID))
	{
		pNCRC->nWidth = 0;
		return TRUE;
	}
	
	switch (nColID)
	{
	case TDCC_ID:
		{
			DWORD dwRefID = m_find.GetLargestReferenceID();

			if (dwRefID)
				pNCRC->nWidth = GraphicsMisc::GetTextWidth(pNCRC->pDC, _T("%u (%u)"), m_dwNextUniqueID - 1, dwRefID);
			else
				pNCRC->nWidth = GraphicsMisc::GetTextWidth(pNCRC->pDC, _T("%u"), m_dwNextUniqueID - 1);
		}
		break; 

	case TDCC_PARENTID:
		pNCRC->nWidth = GraphicsMisc::GetTextWidth(pNCRC->pDC, _T("%u"), m_dwNextUniqueID - 1);
		break; 

	case TDCC_POSITION:
		{
			// determine the longest visible string
			CString sLongest = m_find.GetLongestPosition();
			pNCRC->nWidth = GraphicsMisc::GetAverageMaxStringWidth(sLongest, pNCRC->pDC);
		}
		break;
		
	case TDCC_PRIORITY:
		if (HasStyle(TDCS_HIDEPRIORITYNUMBER))
			pNCRC->nWidth = 16;
		else
			pNCRC->nWidth = pNCRC->pDC->GetTextExtent("10").cx;
		break; 
		
	case TDCC_RISK:
		pNCRC->nWidth = pNCRC->pDC->GetTextExtent("10").cx;
		break; 
		
	case TDCC_RECURRENCE:
		{
			// determine the longest visible string
			pNCRC->nWidth = CTDLRecurringTaskEdit::CalcMaxRegularityWidth(pNCRC->pDC, FALSE);
		}
		break; 
		
	case TDCC_REMAINING:
		{
			int nWidth1 = pNCRC->pDC->GetTextExtent("12m4w").cx;
			int nWidth2 = pNCRC->pDC->GetTextExtent("100.0W").cx;

			pNCRC->nWidth = max(nWidth1, nWidth2);
		}
		break; 
		
	case TDCC_EXTERNALID:
		{
			// determine the longest visible string
			CString sLongest = m_find.GetLongestExternalID();
			pNCRC->nWidth = GraphicsMisc::GetAverageMaxStringWidth(sLongest, pNCRC->pDC);
		}
		break; 
		
	case TDCC_FILEREF:
		pNCRC->nWidth = HasStyle(TDCS_SHOWNONFILEREFSASTEXT) ? 60 : 16; 
		break; 
		
	case TDCC_TRACKTIME:
	case TDCC_FLAG:
	case TDCC_RECENTEDIT:
	case TDCC_DEPENDENCY:
	case TDCC_ICON:
	case TDCC_DONE:
	case TDCC_REMINDER:
		pNCRC->nWidth = 16; 
		break; 
		
	case TDCC_VERSION:
		pNCRC->nWidth = CalcMaxTextWidth(m_cbVersion, 20, FALSE, pNCRC->pDC);
		break;
		
	case TDCC_ALLOCTO:
		{
			// determine the longest visible string
			CString sLongest = m_find.GetLongestAllocTo();

			// include whatever's in the alloc to edit field
			CString sEdit;
			m_cbAllocTo.GetWindowText(sEdit);

			if (sEdit.GetLength() > sLongest.GetLength())
				sLongest = sEdit;

			pNCRC->nWidth = GraphicsMisc::GetAverageMaxStringWidth(sLongest, pNCRC->pDC);
		}
		break;
		
	case TDCC_ALLOCBY:
		pNCRC->nWidth = CalcMaxTextWidth(m_cbAllocBy, 20, FALSE, pNCRC->pDC);
		break;
		
	case TDCC_STATUS:
		pNCRC->nWidth = CalcMaxTextWidth(m_cbStatus, 20, FALSE, pNCRC->pDC);
		break;
		
	case TDCC_CATEGORY:
		{
			// determine the longest visible string
			CString sLongest = m_find.GetLongestCategory();

			// include whatever's in the category edit field
			CString sEdit;
			m_cbCategory.GetWindowText(sEdit);

			if (sEdit.GetLength() > sLongest.GetLength())
				sLongest = sEdit;

			pNCRC->nWidth = GraphicsMisc::GetAverageMaxStringWidth(sLongest, pNCRC->pDC);
		}
		break;
		
	case TDCC_TAGS:
		{
			// determine the longest visible string
			CString sLongest = m_find.GetLongestTag();

			// include whatever's in the tags edit field
			CString sEdit;
			m_cbTags.GetWindowText(sEdit);

			if (sEdit.GetLength() > sLongest.GetLength())
				sLongest = sEdit;

			pNCRC->nWidth = GraphicsMisc::GetAverageMaxStringWidth(sLongest, pNCRC->pDC);
		}
		break;
		
	case TDCC_CREATEDBY:
		pNCRC->nWidth = 30; // for now
		break;
		
	case TDCC_COST:
		{
			// calc the greatest positive/negative cumulative cost of the top level items
			int nWidthMaxCost = 1, nWidthMinCost = 1;
			
			HTREEITEM hti = m_tree.GetChildItem(NULL);
			double dMaxCost = DBL_MIN, dMinCost = DBL_MAX;
			
			while (hti)
			{
				DWORD dwTaskID = GetTaskID(hti);
				const TODOITEM* pTDI = m_data.GetTask(dwTaskID);
				ASSERT (pTDI);
				
				if (pTDI)
				{
					double dCost = m_data.CalcCost(GetTaskID(hti));

					dMaxCost = max(dMaxCost, dCost);
					dMinCost = min(dMinCost, dCost);
				}
				
				hti = m_tree.GetNextItem(hti, TVGN_NEXT);
			}
			
			if (GetTaskCount() && (dMaxCost > 0 || dMinCost < 0 || !HasStyle(TDCS_HIDEZEROTIMECOST)))
			{
				nWidthMaxCost = GraphicsMisc::GetTextWidth(pNCRC->pDC, _T("%.2f"), dMaxCost);
				nWidthMinCost = GraphicsMisc::GetTextWidth(pNCRC->pDC, _T("%.2f"), dMinCost);
			}
			
			pNCRC->nWidth = max(nWidthMinCost, nWidthMaxCost);
		}
		break;
		
	case TDCC_TIMEEST:
	case TDCC_TIMESPENT:
		if (HasStyle(TDCS_DISPLAYHMSTIMEFORMAT))
			pNCRC->nWidth = pNCRC->pDC->GetTextExtent("12m4w").cx;
		else
		{
			BOOL bTimeEst = (nColID == TDCC_TIMEEST);
			CString sLongest = m_find.GetLongestTime(s_tdDefault.nTimeEstUnits, bTimeEst);

			pNCRC->nWidth = GraphicsMisc::GetAverageMaxStringWidth(sLongest, pNCRC->pDC) + 4; // add a bit to handle different time unit widths
		}
		break;
		
	case TDCC_PERCENT:
		pNCRC->nWidth = pNCRC->pDC->GetTextExtent("100%").cx;
		break;
		
	case TDCC_LASTMOD:
		pNCRC->nWidth = CalcMaxDateColWidth(TDCD_LASTMOD, pNCRC->pDC);
		break;

	case TDCC_DUEDATE:
		pNCRC->nWidth = CalcMaxDateColWidth(TDCD_DUE, pNCRC->pDC);
		break;

	case TDCC_CREATIONDATE:
		pNCRC->nWidth = CalcMaxDateColWidth(TDCD_CREATE, pNCRC->pDC);
		break;

	case TDCC_STARTDATE:
		pNCRC->nWidth = CalcMaxDateColWidth(TDCD_START, pNCRC->pDC);
		break;

	case TDCC_DONEDATE:
		pNCRC->nWidth = CalcMaxDateColWidth(TDCD_DONE, pNCRC->pDC);
		break;

	case TDCC_PATH:
		pNCRC->nWidth = 150; // TODO
		break;

	default:
		// custom columns
		if (CTDCCustomAttributeHelper::IsCustomColumn(nColID))
		{
			// determine the longest visible string depending on type
			TDCCUSTOMATTRIBUTEDEFINITION attribDef;
			CTDCCustomAttributeHelper::GetAttributeDef(nColID, m_aCustomAttribDefs, attribDef);

			if (!attribDef.bEnabled)
			{
				pNCRC->nWidth = 0; // hidden
				return TRUE;
			}
			else 
			{
				switch (attribDef.GetDataType())
				{
				case TDCCA_DATE:
					pNCRC->nWidth = CalcMaxDateColWidth(TDCD_CUSTOM, pNCRC->pDC);
					break;

				case TDCCA_ICON:
					if (attribDef.IsList())
					{
						pNCRC->nWidth = CTDCCustomAttributeHelper::CalcLongestListItem(attribDef, pNCRC->pDC);
					}
					else // single icon, no text
					{
						pNCRC->nWidth = 16; 
					}
					break;

				case TDCCA_BOOL:
					if (attribDef.sColumnTitle.GetLength() == 1)
					{
						pNCRC->nWidth = GraphicsMisc::GetTextWidth(pNCRC->pDC, attribDef.sColumnTitle);
					}
					else 
					{
						pNCRC->nWidth = GraphicsMisc::GetTextWidth(pNCRC->pDC, _T("+"));
					}
					break;

				case TDCCA_DOUBLE:
					{
						// numerals are always the same width so we don't need average width
						CString sLongest = m_find.GetLongestCustomDoubleAttribute(attribDef.sUniqueID);
						pNCRC->nWidth = GraphicsMisc::GetTextWidth(pNCRC->pDC, sLongest);
					}
					break;

				case TDCCA_INTEGER:
					{
						// numerals are always the same width so we don't need average width
						CString sLongest = m_find.GetLongestCustomAttribute(attribDef.sUniqueID);
						pNCRC->nWidth = GraphicsMisc::GetTextWidth(pNCRC->pDC, sLongest); // 
					}
					break;

				default:
					{
						CString sLongest = m_find.GetLongestCustomAttribute(attribDef.sUniqueID);
						pNCRC->nWidth = GraphicsMisc::GetAverageMaxStringWidth(sLongest, pNCRC->pDC);
					}
					break;
				}
			}
		}
		else
		{
			ASSERT (0);
		}
		break;
	}

	// maximum width
	if (m_nMaxColWidth != -1 && (int)pNCRC->nWidth > m_nMaxColWidth)
		pNCRC->nWidth = m_nMaxColWidth;

	// minimum width
	else if (pNCRC->nWidth < 16)
		pNCRC->nWidth = 16;
	
	return TRUE;
}

int CToDoCtrl::CalcMaxDateColWidth(TDC_DATE nDate, CDC* pDC)
{
	// basic date string
	COleDateTime date(2000, 12, 31, 23, 59, 0);
	DWORD dwFlags = 0;

	if (HasStyle(TDCS_SHOWDATESINISO))
		dwFlags |= DHFD_ISO;

	// time component
	if (m_find.FindVisibleTaskWithTime(nDate))
		dwFlags |= DHFD_TIME | DHFD_NOSEC;

	CString sDate = CDateHelper::FormatDate(date, dwFlags);
	int nWidth = pDC->GetTextExtent(sDate).cx;

	// add longest day of week if required
	if (HasStyle(TDCS_SHOWWEEKDAYINDATES))
	{
		nWidth += pDC->GetTextExtent(" ").cx; // spacer
		nWidth += CDateHelper::CalcLongestWeekdayName(pDC, TRUE); // short day
	}
	
	return nWidth;
}

int CToDoCtrl::GetTasks(CTaskFile& tasks, const TDCGETTASKS& filter) const
{
	// ISO date strings
	// must be done first before any tasks are added
	tasks.EnableISODates(HasStyle(TDCS_SHOWDATESINISO));

	// set save format to match load format
	tasks.SetSaveAs(GetSaveAs(m_bIsUnicode));
	
	AddTreeChildrenToTaskFile(NULL, tasks, NULL, filter);
	
	// filename
	if (filter.HasFlag(TDCGTF_FILENAME))
		tasks.SetFileName(m_sLastSavePath);

	// meta data	
	if (m_mapMetaData.GetCount())
		tasks.SetMetaData(m_mapMetaData);

	// and custom attributes
	SaveCustomAttributeDefinitions(tasks);

	// finally the file header
	AppendTaskFileHeader(tasks);
	
	return tasks.GetTaskCount();
}

void CToDoCtrl::SetUnicode(BOOL bUnicode) 
{ 
	if ((!bUnicode) != (!m_bIsUnicode))
		m_sXmlHeader.Empty();

	m_bIsUnicode = bUnicode; 
}

void CToDoCtrl::AppendTaskFileHeader(CTaskFile& tasks) const
{
	// if the file format has changed we may need to re-initialize the xml header
	if (m_bIsUnicode && m_sXmlHeader.IsEmpty())
		tasks.SetXmlHeader(DEFAULT_UNICODE_HEADER);
	else
		tasks.SetXmlHeader(m_sXmlHeader);

	// add other taskfile related stuff
	tasks.SetProjectName(m_sProjectName);
	tasks.SetArchive(m_bArchive);
	tasks.SetFileFormat(FILEFORMAT_CURRENT);
	tasks.SetEarliestDueDate(m_data.GetEarliestDueDate());

	tasks.SetNextUniqueID(m_dwNextUniqueID);
	ASSERT (tasks.GetNextUniqueID() == m_dwNextUniqueID);

	tasks.SetFileName(FileMisc::GetFileNameFromPath(m_sLastSavePath));
	
	if (!HasStyle(TDCS_USES3RDPARTYSOURCECONTROL))
	{
		tasks.SetFileVersion(m_nFileVersion);
	}
}

int CToDoCtrl::GetAllTasks(CTaskFile& tasks) const
{
	// and custom attributes
	SaveCustomAttributeDefinitions(tasks);

	tasks.EnableISODates(HasStyle(TDCS_SHOWDATESINISO));

	// build task file from data structure not the tree
	if (AddSubTasksToTaskFile(m_data.GetStructure(), tasks, NULL))
	{
		tasks.SetSaveAs(GetSaveAs(m_bIsUnicode));
		return tasks.GetTaskCount();
	}
	
	// else
	return 0;
}

BOOL CToDoCtrl::AddSubTasksToTaskFile(const TODOSTRUCTURE* pTDSParent, CTaskFile& tasks, HTASKITEM hParentTask) const
{
   for (int nSubTask = 0; nSubTask < pTDSParent->GetSubTaskCount(); nSubTask++)
   {
      const TODOSTRUCTURE* pTDS = pTDSParent->GetSubTask(nSubTask);
      ASSERT(pTDS);
 
      if (!pTDS)
         return FALSE;

      DWORD dwTaskID = pTDS->GetTaskID();
      ASSERT(dwTaskID);

      if (!dwTaskID)
         return FALSE;

      const TODOITEM* pTDI = m_data.GetTask(dwTaskID, FALSE);
      ASSERT(pTDI);
 
      if (!pTDI)
         return FALSE;

	  // position is 1-based
	  //int nPos = nSubTask + 1;

      if (!AddTaskToTaskFile(pTDI, pTDS, /*nPos,*/ tasks, hParentTask))
      {
         ASSERT(0);
         return FALSE;
      }
   }

   return TRUE;
}

BOOL CToDoCtrl::AddTaskToTaskFile(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, /*int nPos,*/ CTaskFile& tasks, HTASKITEM hParentTask) const
{
   CString sTitle = pTDI->sTitle;
   DWORD dwTaskID = pTDS->GetTaskID();

   HTASKITEM hTask = tasks.NewTask(sTitle, hParentTask, dwTaskID);
   ASSERT(hTask);

   if (!hTask)
      return FALSE;

   // copy all other attributes
   SetAllTaskAttributes(pTDI, pTDS, tasks, hTask);

   // copy children
   return AddSubTasksToTaskFile(pTDS, tasks, hTask);
}

void CToDoCtrl::HandleUnsavedComments()
{
	if (m_bCommentsChange)
	{
		UpdateComments(TRUE); // no longer handled by UpdateData
		SetSelectedTaskComments(m_sTextComments, m_customComments, TRUE); // TRUE == internal call
		m_bCommentsChange = FALSE;
	}
}

HTREEITEM CToDoCtrl::SetAllTasks(const CTaskFile& tasks)
{
	Selection().RemoveAll(FALSE);

	m_tree.DeleteAllItems();
	m_data.DeleteAllTasks();

	// build data structure first 
	m_data.BuildDataModel(tasks);

	// then tree structure
	return RebuildTree();
}

HTREEITEM CToDoCtrl::RebuildTree(const void* pContext)
{
	// prevent handling selection changes
	CAutoFlag af(m_bDeletingAll, TRUE);

	m_tree.TCH().SelectItem(NULL);
	m_tree.DeleteAllItems();

	if (BuildTreeItem(NULL, m_data.GetStructure(), pContext))
		return m_tree.GetChildItem(NULL);

	// else
	return NULL;
}

BOOL CToDoCtrl::BuildTreeItem(HTREEITEM htiParent, const TODOSTRUCTURE* pTDS, const void* pContext)
{
	// sanity checks
#ifdef _DEBUG
	if (pTDS)
	{
		if (htiParent == NULL || htiParent == TVI_ROOT)
		{
			ASSERT (pTDS->GetTaskID() == 0);

			if (pTDS->GetTaskID() != 0)
				return FALSE;
		}
		else
		{
			ASSERT (m_find.GetTaskID(htiParent) == pTDS->GetTaskID());
			
			if (m_find.GetTaskID(htiParent) != pTDS->GetTaskID())
				return FALSE;
		}
	}
	else
	{
		ASSERT(htiParent);
		ASSERT(!m_tree.ItemHasChildren(htiParent));

		DWORD dwTaskID = m_find.GetTaskID(htiParent);
		pTDS = m_data.LocateTask(dwTaskID);
	}
#endif

	// rebuild
	int nSubtask = pTDS->GetSubTaskCount();

	// reverse order is quicker according to Raymond Chen
	while (nSubtask--)
	{
		const TODOSTRUCTURE* pTDSChild = pTDS->GetSubTask(nSubtask);
		DWORD dwTaskID = pTDSChild->GetTaskID();

		// Get the 'True' task for matching but also hang
		// on to the 'original' task ID because it may get
		// overwritten and we need it for when we add the item
		DWORD dwOrgID = dwTaskID;

		const TODOITEM* pTDIChild = m_data.GetTask(dwTaskID);
		ASSERT(pTDIChild);

		// is this task wanted?
		BOOL bHasChildren = pTDSChild->HasSubTasks();
		BOOL bAddTask = TRUE;

		if (!bHasChildren && pContext)
			bAddTask = WantAddTask(pTDIChild, pTDSChild, pContext);

		if (bAddTask)
		{
			// add this item to tree
			HTREEITEM htiChild = InsertTreeItem(pTDIChild, dwOrgID, htiParent, TVI_FIRST);
			ASSERT(htiChild);

			// update next unique ID
			m_dwNextUniqueID = max(m_dwNextUniqueID, dwOrgID + 1);

			// and its children
			if (bHasChildren)
			{
				BuildTreeItem(htiChild, pTDSChild, pContext);

				// if no subtasks got added to the tree then we may need to remove the parent too
				if (!m_tree.ItemHasChildren(htiChild))
				{
					BOOL bRemoveTask = (pContext != NULL && !WantAddTask(pTDIChild, pTDSChild, pContext));

					if (bRemoveTask)
					{
						m_tree.DeleteItem(htiChild);
					}
				}
			}
		}
	}

	return TRUE;
}

BOOL CToDoCtrl::WantAddTask(const TODOITEM* /*pTDI*/, const TODOSTRUCTURE* /*pTDS*/, const void* /*pContext*/) const
{
	return TRUE;
}

int CToDoCtrl::GetSelectedTasks(CTaskFile& tasks, const TDCGETTASKS& filter, DWORD dwFlags) const
{
	if (!GetSelectedCount())
		return 0;
	
	CHTIList selection;
	Selection().CopySelection(selection);

	// remove duplicate subtasks if the caller has 
	// already asked for subtasks
	BOOL bWantSubtasks = !(dwFlags & TDCGSTF_NOTSUBTASKS);
	BOOL bResolveReferences = (dwFlags & TDCGSTF_RESOLVEREFERENCES);
	BOOL bWantAllParents = (dwFlags & TDCGSTF_ALLPARENTS);
	BOOL bWantImmediateParent = (bWantAllParents ? FALSE : (dwFlags & TDCGSTF_IMMEDIATEPARENT));

	if (bWantSubtasks)
		Selection().RemoveChildDuplicates(selection, m_tree);
	
	CTreeSelectionHelper::OrderSelection(selection, m_tree);
	POSITION pos = selection.GetHeadPosition();

	// ISO date strings
	// must be done first before any tasks are added
	tasks.EnableISODates(HasStyle(TDCS_SHOWDATESINISO));

	while (pos)
	{
		HTREEITEM hti = selection.GetNext(pos);
		DWORD dwTaskID = GetTaskID(hti);

		// do we need to resolve task references?
		if (bResolveReferences)
		{
			DWORD dwRefID = GetTrueTaskID(hti);
			
			if (dwRefID != dwTaskID)
				hti = m_find.GetItem(dwRefID); // true task
		}
		
		// does the user want this task's parent(s) ?
		if (bWantAllParents || bWantImmediateParent)
		{
			VERIFY(AddItemAndParentToTaskFile(hti, tasks, filter, bWantAllParents, bWantSubtasks));
		}
		else
		{
			// find the parent task previously added (or not)
			DWORD dwParentID = GetTaskParentID(hti);
			HTASKITEM hParent = tasks.FindTask(dwParentID);

			VERIFY(AddTreeItemToTaskFile(hti, tasks, hParent, filter, bWantSubtasks, dwParentID));
		}
	}

	// filename
	if (filter.HasFlag(TDCGTF_FILENAME))
		tasks.SetFileName(m_sLastSavePath);

	// meta data
	if (m_mapMetaData.GetCount())
		tasks.SetMetaData(m_mapMetaData);

	// custom data definitions
	if (m_aCustomAttribDefs.GetSize())
		tasks.SetCustomAttributeDefs(m_aCustomAttribDefs);

	return (tasks.GetTaskCount());
}

BOOL CToDoCtrl::AddItemAndParentToTaskFile(HTREEITEM hti, CTaskFile& tasks, const TDCGETTASKS& filter, 
											BOOL bAllParents, BOOL bWantSubtasks) const
{
	// add parents first, recursively if necessarily
	HTREEITEM htiParent = m_tree.GetParentItem(hti);
	HTASKITEM hParent = NULL;
	DWORD dwParentID = 0;

	if (htiParent)
	{
		// Note: we never want parent's subtasks, so we pass FALSE
		if (bAllParents)
		{
			VERIFY(AddItemAndParentToTaskFile(htiParent, tasks, filter, TRUE, FALSE));
		}
		else
		{
			DWORD dwGrandParentID = GetTaskParentID(htiParent);
			HTASKITEM hGrandParent = tasks.FindTask(dwGrandParentID);

			VERIFY(AddTreeItemToTaskFile(htiParent, tasks, hGrandParent, filter, FALSE, dwGrandParentID));
		}

		// now find the parent we just added
		dwParentID = GetTaskID(htiParent);
		hParent = tasks.FindTask(dwParentID);
	}

	// now add item itself
	if (htiParent)
		ASSERT(hParent);

	return AddTreeItemToTaskFile(hti, tasks, hParent, filter, bWantSubtasks, dwParentID);
}

DWORD CToDoCtrl::GetTaskParentID(HTREEITEM hti) const
{
	if (!hti || hti == TVI_ROOT || hti == TVI_FIRST || hti == TVI_LAST)
		return 0;
	
	return GetTaskID(m_tree.GetParentItem(hti));
}

void CToDoCtrl::CacheTreeSelection(TDCSELECTIONCACHE& cache) const
{
	if (GetSelectedTaskIDs(cache.aSelTaskIDs, cache.dwFocusedTaskID, FALSE) == 0)
		return;
 
	// we prefer the 'down' breadcrumbs because this way the 
	// focused item appears to stay still, just like when we delete items
	// but in case the item in question is last, we also get the 'up' breadcrumbs
	cache.aBreadcrumbs.RemoveAll();

	m_find.GetTaskBreadcrumbs(cache.dwFocusedTaskID, cache.aBreadcrumbs, TCFBC_VISIBLEONLY);
	m_find.GetTaskBreadcrumbs(cache.dwFocusedTaskID, cache.aBreadcrumbs, TCFBC_VISIBLEONLY | TCFBC_APPEND | TCFBC_UP);
}

void CToDoCtrl::RestoreTreeSelection(const TDCSELECTIONCACHE& cache)
{
	if (cache.aSelTaskIDs.GetSize() == 0)
		return;

	DWORD dwFocusedTaskID = cache.dwFocusedTaskID;
	ASSERT(dwFocusedTaskID);

	if (m_find.GetItem(dwFocusedTaskID) == NULL)
	{
		dwFocusedTaskID = 0;
		int nID = cache.aBreadcrumbs.GetSize();
		
		while (nID--)
		{
			dwFocusedTaskID = cache.aBreadcrumbs[nID];
			
			if (m_find.GetItem(dwFocusedTaskID))
				break;
			else
				dwFocusedTaskID = 0;
		}
	}
	
	CToDoCtrl::RestoreTreeSelection(cache.aSelTaskIDs, dwFocusedTaskID);
}

int CToDoCtrl::GetSelectedTaskIDs(CDWordArray& aTaskIDs) const
{
	DWORD dwFocusID;
	return GetSelectedTaskIDs(aTaskIDs, dwFocusID, FALSE);
}

int CToDoCtrl::GetSelectedTaskIDs(CDWordArray& aTaskIDs, DWORD& dwFocusedTaskID, BOOL bRemoveChildDupes) const
{
	if (!GetSelectedCount())
		return 0;
	
	CHTIList selection;
	
	Selection().CopySelection(selection);

	if (bRemoveChildDupes)
		Selection().RemoveChildDuplicates(selection, m_tree);
	
	TDCGETTASKS filter(TDCGT_ALL, 0);
	POSITION pos = selection.GetHeadPosition();
	
	while (pos)
		aTaskIDs.Add(GetTaskID(selection.GetNext(pos)));

	// focused item
	HTREEITEM htiFocus = m_tree.GetSelectedItem();
	dwFocusedTaskID = GetTaskID(htiFocus);

	ASSERT (GetSelectedCount() > 1 || dwFocusedTaskID == aTaskIDs[0]); // sanity check
	
	return (aTaskIDs.GetSize());
}

void CToDoCtrl::RestoreTreeSelection(const CDWordArray& aTaskIDs, DWORD dwDefaultID)
{
	BOOL bSel = MultiSelectItems(aTaskIDs);
	
	if (!bSel) // nothing restored
	{
		ASSERT(GetSelectedCount() == 0);
		
		HTREEITEM hti = NULL;
		
		if (dwDefaultID)
			hti = m_find.GetItem(dwDefaultID);
		
		if (!hti)
			hti = m_tree.GetChildItem(NULL); // select first item
		
		SelectItem(hti); // select first item
		return;
	}
	
	// don't update controls is only one item is selected and it did not
	// change as a result of the filter
	BOOL bSelChange = !(GetSelectedCount() == 1 && aTaskIDs.GetSize() == 1 &&
		GetSelectedTaskID() == aTaskIDs[0]);
	
	if (bSelChange)
		UpdateControls();
}


BOOL CToDoCtrl::InsertTasks(const CTaskFile& tasks, TDC_INSERTWHERE nWhere)
{
	if (IsReadOnly())
		return FALSE;
	
	HTREEITEM htiParent = NULL, htiAfter = NULL;

	if (!GetInsertLocation(nWhere, htiParent, htiAfter))
		return FALSE;

	// else
	IMPLEMENT_UNDO(TDCUAT_ADD);
	HOLD_REDRAW(*this, m_tree);

	// Fix up dependencies if not inserting into new tasklist
	if (GetTaskCount())
	{
		CTaskFile copy(tasks);

		// and always assign new IDs
		PrepareTaskIDsForPaste(copy, TDCR_YES);
		return AddTasksToTree(copy, htiParent, htiAfter, TDCR_NO);
	}

	return AddTasksToTree(tasks, htiParent, htiAfter, TDCR_NO);
}

BOOL CToDoCtrl::SetTaskAttributes(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, CTaskFile& file, 
								  HTASKITEM hTask, const TDCGETTASKS& filter, BOOL bTitleCommentsOnly) const
{
	ASSERT(pTDI);
	
	if (!pTDI)
		return FALSE;
	
	BOOL bDone = pTDI->IsDone();
	BOOL bTitleOnly = filter.HasFlag(TDCGTF_TITLESONLY);
	BOOL bHtmlComments = filter.HasFlag(TDCGTF_HTMLCOMMENTS);
	BOOL bTransform = filter.HasFlag(TDCGTF_TRANSFORM);
	
	// attributes
	file.SetTaskReferenceID(hTask, pTDI->dwTaskRefID);

	// if task is a reference we use a bit of sleight of hand
	// and write the 'true' task's title but nothing else
	if (pTDI->dwTaskRefID)
		file.SetTaskTitle(hTask, m_data.GetTaskTitle(pTDI->dwTaskRefID));
	else
		file.SetTaskTitle(hTask, pTDI->sTitle);
	
	// hide IDs if not wanted
	if (bTitleOnly || bTitleCommentsOnly || !filter.WantAttribute(TDCA_ID))
		file.HideAttribute(hTask, TDL_TASKID);

	// ignore everything else if we are a reference
	if (pTDI->dwTaskRefID)
		return TRUE;
	
	if (!bTransform)
		file.SetTaskIcon(hTask, pTDI->sIcon);
	
	// comments
	if (!bTitleOnly && filter.WantAttribute(TDCA_COMMENTS))
	{
		if (bHtmlComments && !pTDI->customComments.IsEmpty())
		{
			CString sHtml;
			
			if (m_mgrContent.ConvertContentToHtml(pTDI->customComments, sHtml, pTDI->sCommentsTypeID, file.GetHtmlCharSet(), file.GetHtmlImageFolder()))
				file.SetTaskHtmlComments(hTask, sHtml, bTransform);
		}
		
		if (!pTDI->sComments.IsEmpty())
			file.SetTaskComments(hTask, pTDI->sComments);
	}
	
	// highest priority, because we need it further down
	int nHighestPriority = m_data.GetHighestPriority(pTDI, pTDS, FALSE);
	
	if (!(bTitleOnly || bTitleCommentsOnly))
	{
		// items position
		if (filter.WantAttribute(TDCA_POSITION))
			file.SetTaskPosition(hTask, m_data.GetTaskPosition(pTDI, pTDS));
		
		if (pTDI->bFlagged && filter.WantAttribute(TDCA_FLAG))
			file.SetTaskFlag(hTask, pTDI->bFlagged != FALSE);
		
		if ((pTDI->trRecurrence.nRegularity != TDIR_ONCE || !pTDI->trRecurrence.sRegularity.IsEmpty()) && 
			filter.WantAttribute(TDCA_RECURRENCE))
		{
			file.SetTaskRecurrence(hTask, pTDI->trRecurrence);
		}
		
		if (pTDI->aAllocTo.GetSize() && filter.WantAttribute(TDCA_ALLOCTO))
			file.SetTaskAllocatedTo(hTask, pTDI->aAllocTo);
		
		if (!pTDI->sAllocBy.IsEmpty() && filter.WantAttribute(TDCA_ALLOCBY))
			file.SetTaskAllocatedBy(hTask, pTDI->sAllocBy);
		
		if (!pTDI->sStatus.IsEmpty() && filter.WantAttribute(TDCA_STATUS))
			file.SetTaskStatus(hTask, pTDI->sStatus);
		
		if (!pTDI->sVersion.IsEmpty() && filter.WantAttribute(TDCA_VERSION))
			file.SetTaskVersion(hTask, pTDI->sVersion);
		
		if (pTDI->aCategories.GetSize() && filter.WantAttribute(TDCA_CATEGORY))
			file.SetTaskCategories(hTask, pTDI->aCategories);
		
		if (pTDI->aTags.GetSize() && filter.WantAttribute(TDCA_TAGS))
			file.SetTaskTags(hTask, pTDI->aTags);
		
		if (!pTDI->sFileRefPath.IsEmpty() && filter.WantAttribute(TDCA_FILEREF))
			file.SetTaskFileReferencePath(hTask, pTDI->sFileRefPath);
		
		if (!pTDI->sCreatedBy.IsEmpty() && filter.WantAttribute(TDCA_CREATEDBY))
			file.SetTaskCreatedBy(hTask, pTDI->sCreatedBy);
		
		if (!pTDI->sExternalID.IsEmpty() && filter.WantAttribute(TDCA_EXTERNALID))
			file.SetTaskExternalID(hTask, pTDI->sExternalID);
		
		if (pTDI->aDependencies.GetSize() && filter.WantAttribute(TDCA_DEPENDENCY))
			file.SetTaskDependencies(hTask, pTDI->aDependencies);
		
		if (filter.WantAttribute(TDCA_PRIORITY))
		{
			file.SetTaskPriority(hTask, pTDI->nPriority);
			
			if (nHighestPriority > pTDI->nPriority)
				file.SetTaskHighestPriority(hTask, nHighestPriority);
		}
		
		if (filter.WantAttribute(TDCA_RISK))
		{
			file.SetTaskRisk(hTask, pTDI->nRisk);
			
			int nHighestRisk = m_data.GetHighestRisk(pTDI, pTDS);
			
			if (nHighestRisk > pTDI->nRisk)
				file.SetTaskHighestRisk(hTask, nHighestRisk);
		}
		
		// percent done
		if (filter.WantAttribute(TDCA_PERCENT))
		{
			// don't allow incomplete tasks to be 100%
			int nPercent = pTDI->IsDone() ? 100 : min(99, pTDI->nPercentDone); 
			file.SetTaskPercentDone(hTask, (unsigned char)nPercent);
			
			// calculated percent
			nPercent = m_data.CalcPercentDone(pTDI, pTDS);
			
			if (nPercent > 0)
				file.SetTaskCalcCompletion(hTask, nPercent);
		}
		
		// cost
		if (filter.WantAttribute(TDCA_COST))
		{
			//if (pTDI->dCost > 0)
			file.SetTaskCost(hTask, pTDI->dCost);
			
			double dCost = m_data.CalcCost(pTDI, pTDS);
			
			//if (dCost > 0)
			file.SetTaskCalcCost(hTask, dCost);
		}
		
		// time estimate
		if (filter.WantAttribute(TDCA_TIMEEST))
		{
			if (pTDI->dTimeEstimate > 0 || pTDI->nTimeEstUnits != TDITU_HOURS)
				file.SetTaskTimeEstimate(hTask, pTDI->dTimeEstimate, pTDI->nTimeEstUnits);
			
			// for calc'ed estimate use this item's units if it
			// has a non-zero time estimate, else its first subtask's units
			TCHAR cUnits = m_data.GetBestCalcTimeEstUnits(pTDI, pTDS);
			double dTime = m_data.CalcTimeEstimate(pTDI, pTDS, cUnits);
			
			if (dTime > 0)
				file.SetTaskCalcTimeEstimate(hTask, dTime, cUnits);
		}
		
		// time spent
		if (filter.WantAttribute(TDCA_TIMESPENT))
		{
			if (pTDI->dTimeSpent > 0|| pTDI->nTimeSpentUnits != TDITU_HOURS)
				file.SetTaskTimeSpent(hTask, pTDI->dTimeSpent, pTDI->nTimeSpentUnits);
			
			// for calc'ed spent use this item's units if it
			// has a non-zero time estimate, else its first subtask's units
			TCHAR cUnits = m_data.GetBestCalcTimeSpentUnits(pTDI, pTDS);
			double dTime = m_data.CalcTimeSpent(pTDI, pTDS, cUnits);
			
			if (dTime > 0)
				file.SetTaskCalcTimeSpent(hTask, dTime, cUnits);
		}
		
		// done date
		if (bDone)
		{
			file.SetTaskDoneDate(hTask, pTDI->dateDone);
			
			// hide it if column not visible
			if (!filter.WantAttribute(TDCA_DONEDATE))
			{
				file.HideAttribute(hTask, TDL_TASKDONEDATE);
				file.HideAttribute(hTask, TDL_TASKDONEDATESTRING);
			}
		}
		
		// add due date if we're filtering by due date
		if (filter.dateDueBy > 0 || filter.WantAttribute(TDCA_DUEDATE))
		{
			if (pTDI->HasDue())
				file.SetTaskDueDate(hTask, pTDI->dateDue);
		
			else if (HasStyle(TDCS_NODUEDATEISDUETODAY))
				file.SetTaskDueDate(hTask, CDateHelper::GetDateOnly(COleDateTime::GetCurrentTime()));
		}

		if (filter.WantAttribute(TDCA_DUEDATE) && (HasStyle(TDCS_USEEARLIESTDUEDATE) || HasStyle(TDCS_USELATESTDUEDATE)))
		{
			double dDate = m_data.CalcDueDate(pTDI, pTDS);
			
			if (dDate > 0)
				file.SetTaskCalcDueDate(hTask, dDate);
		}
		
		// start date
		if (filter.WantAttribute(TDCA_STARTDATE))
		{
			if (pTDI->HasStart())
				file.SetTaskStartDate(hTask, pTDI->dateStart);
		
			if (HasStyle(TDCS_USEEARLIESTDUEDATE) || HasStyle(TDCS_USELATESTDUEDATE))
			{
				double dDate = m_data.CalcStartDate(pTDI, pTDS);
				
				if (dDate > 0)
					file.SetTaskCalcStartDate(hTask, dDate);
			}
		}
		
		// creation date
		if (pTDI->HasCreation() && filter.WantAttribute(TDCA_CREATIONDATE))
			file.SetTaskCreationDate(hTask, pTDI->dateCreated);
		
		// modify date
		if (pTDI->HasLastMod() && filter.WantAttribute(TDCA_LASTMOD))
			file.SetTaskLastModified(hTask, pTDI->dateLastMod);
		
		// custom comments
		if (filter.WantAttribute(TDCA_COMMENTS))
		{
			// Even if it's a text format we still need to write out the comments format
			// unless there were no comments or the comment type is the same as the default
			if (CONTENTFORMAT(pTDI->sCommentsTypeID).FormatIsText())
			{
				if (!pTDI->sComments.IsEmpty() || pTDI->sCommentsTypeID != m_cfDefault)
					file.SetTaskCustomComments(hTask, _T(""), pTDI->sCommentsTypeID);
			}
			// else we save the custom comments either if there are any comments or if the
			// comments type is different from the default
			else if (!pTDI->customComments.IsEmpty() || pTDI->sCommentsTypeID != m_cfDefault)
				file.SetTaskCustomComments(hTask, pTDI->customComments, pTDI->sCommentsTypeID);
		}

		// custom data 
		if (filter.WantAttribute(TDCA_CUSTOMATTRIB))
			file.SetTaskCustomAttributeData(hTask, pTDI->mapCustomData);
	}
	else if (bDone)
	{
		file.SetTaskDoneDate(hTask, pTDI->dateDone);
		file.HideAttribute(hTask, TDL_TASKDONEDATE);
		file.HideAttribute(hTask, TDL_TASKDONEDATESTRING);
	}

	// task color
	if (pTDI->color)
		file.SetTaskColor(hTask, pTDI->color);

	// actual text color
	COLORREF crText = NOCOLOR, crBack = NOCOLOR;
	GetTaskTextColors(pTDI, pTDS, crText, crBack);

	if (HasStyle(TDCS_TASKCOLORISBACKGROUND))
		crText = crBack;

	if (crText != NOCOLOR)
		file.SetTaskTextColor(hTask, crText);

	// priority color
	file.SetTaskPriorityColor(hTask, GetPriorityColor(nHighestPriority));

	return TRUE;
}

BOOL CToDoCtrl::SetAllTaskAttributes(const TODOITEM* pTDI, const TODOSTRUCTURE* pTDS, CTaskFile& file, HTASKITEM hTask) const
{
	ASSERT(pTDI);
	
	if (!pTDI)
		return FALSE;

	// SPECIAL CASE:
	// if task is a reference we use a bit of sleight of hand
	// and write the 'true' task's title but nothing else
	if (pTDI->dwTaskRefID)
	{
		file.SetTaskTitle(hTask, m_data.GetTaskTitle(pTDI->dwTaskRefID));
		file.SetTaskReferenceID(hTask, pTDI->dwTaskRefID);

		return TRUE;
	}

	// 'true' tasks
	file.SetTaskAttributes(hTask, pTDI);

	// dynamically calculated attributes
	int nHighestPriority = m_data.GetHighestPriority(pTDI, pTDS, FALSE); 
	
	if (nHighestPriority > pTDI->nPriority)
		file.SetTaskHighestPriority(hTask, nHighestPriority);
	
	int nHighestRisk = m_data.GetHighestRisk(pTDI, pTDS);
	
	if (nHighestRisk > pTDI->nRisk)
		file.SetTaskHighestRisk(hTask, nHighestRisk);
	
	// calculated percent
	int nPercent = m_data.CalcPercentDone(pTDI, pTDS);
	
	if (nPercent > 0)
		file.SetTaskCalcCompletion(hTask, nPercent);
	
	// cost
	double dCost = m_data.CalcCost(pTDI, pTDS);
	
	if (dCost != 0)
		file.SetTaskCalcCost(hTask, dCost);
	
	// for calc'ed estimate use this item's units if it
	// has a non-zero time estimate, else its first subtask's units
	TCHAR cUnits = m_data.GetBestCalcTimeEstUnits(pTDI, pTDS);
	double dTime = m_data.CalcTimeEstimate(pTDI, pTDS, cUnits);
	
	if (dTime > 0)
		file.SetTaskCalcTimeEstimate(hTask, dTime, cUnits);
	
	// for calc'ed spent use this item's units if it
	// has a non-zero time estimate, else its first subtask's units
	cUnits = m_data.GetBestCalcTimeEstUnits(pTDI, pTDS);
	dTime = m_data.CalcTimeSpent(pTDI, pTDS, cUnits);
	
	if (dTime > 0)
		file.SetTaskCalcTimeSpent(hTask, dTime, cUnits);
	
	// due date
	if (HasStyle(TDCS_USEEARLIESTDUEDATE) || HasStyle(TDCS_USELATESTDUEDATE))
	{
		double dDate = m_data.CalcDueDate(pTDI, pTDS);
		
		if (dDate > 0)
			file.SetTaskCalcDueDate(hTask, dDate);
	}
	
	// start date
	if (HasStyle(TDCS_USEEARLIESTSTARTDATE) || HasStyle(TDCS_USELATESTSTARTDATE))
	{
		double dDate = m_data.CalcStartDate(pTDI, pTDS);
		
		if (dDate > 0)
			file.SetTaskCalcStartDate(hTask, dDate);
	}
	
	// actual text color
	COLORREF crText, crBack;
	GetTaskTextColors(pTDI, pTDS, crText, crBack, FALSE);

	if (crText)
		file.SetTaskTextColor(hTask, crText);
	
	// priority color
	file.SetTaskPriorityColor(hTask, GetPriorityColor(nHighestPriority));
	
	return TRUE;
}

int CToDoCtrl::AddTreeChildrenToTaskFile(HTREEITEM hti, CTaskFile& file, HTASKITEM hTask, const TDCGETTASKS& filter) const
{
	HTREEITEM htiChild = m_tree.GetChildItem(hti);
	int nChildren = 0;
	
	while (htiChild)
	{
		if (AddTreeItemToTaskFile(htiChild, file, hTask, filter, TRUE)) // TRUE = want subtasks
			nChildren++;
		
		// next
		htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
	}
	
	return nChildren;
}

BOOL CToDoCtrl::AddTreeItemToTaskFile(HTREEITEM hti, CTaskFile& file, HTASKITEM hParentTask, 
										const TDCGETTASKS& filter, BOOL bWantSubtasks, DWORD dwParentID) const
{
	// attributes
	DWORD dwTaskID = GetTaskID(hti);
	const TODOITEM* pTDI = m_data.GetTask(dwTaskID, FALSE);
	ASSERT (pTDI);
	
	if (pTDI)
	{
		BOOL bDone = pTDI->IsDone();
		BOOL bGoodAsDone = bDone ? TRUE : m_data.IsTaskDone(GetTaskID(hti), TDCCHECKALL);
		BOOL bTitleCommentsOnly = m_tree.ItemHasChildren(hti) &&
								filter.HasFlag(TDCGTF_PARENTTITLECOMMENTSONLY);
		
		CString sTitle = pTDI->sTitle;
		
		HTASKITEM hTask = file.NewTask(sTitle, hParentTask, dwTaskID, dwParentID);
		ASSERT(hTask);
		
		if (!hTask)
			return FALSE;

		const TODOSTRUCTURE* pTDS = m_data.LocateTask(dwTaskID);
		ASSERT (pTDS);
		
	    SetTaskAttributes(pTDI, pTDS, file, hTask, filter, bTitleCommentsOnly);

		// we return TRUE if we match the filter _or_ we have any matching children
		BOOL bMatch = FALSE;
		
		if (bWantSubtasks)
		{
			bMatch = AddTreeChildrenToTaskFile(hti, file, hTask, filter);
		}

		if (!bMatch) //  no children matched
		{
			switch (filter.nFilter)
			{
			case TDCGT_ALL:
				bMatch = TRUE; // always
				break;
				
			case TDCGT_DUE:
			case TDCGT_DUETOMORROW:
			case TDCGT_DUETHISWEEK:
			case TDCGT_DUENEXTWEEK:
			case TDCGT_DUETHISMONTH:
			case TDCGT_DUENEXTMONTH:
				// remember to check for 'Auto-Due-Today' tasks
				bMatch |= (!bGoodAsDone && (pTDI->IsDue(filter.dateDueBy) || (!pTDI->HasDue() && HasStyle(TDCS_NODUEDATEISDUETODAY))));
				break;
				
			case TDCGT_DONE:
				bMatch |= (bGoodAsDone || bDone);
				break;
				
			case TDCGT_NOTDONE:
				bMatch |= !bGoodAsDone; // 'good as' includes 'done'

				// check 'flagged' flag
				if (!bMatch && filter.HasFlag(TDCGTF_KEEPFLAGGED) && pTDI->bFlagged) 
					bMatch = TRUE;
				break;
				
			default:
				bMatch = FALSE;
			}

			// then check 'allocated to' if set
			if (bMatch && !filter.sAllocTo.IsEmpty())
				bMatch = (Misc::Find(pTDI->aAllocTo, filter.sAllocTo) != -1);
		}
		
		// if we don't match, we remove the item unless we're flagged
		if (!bMatch)
			file.DeleteTask(hTask);
		
		return bMatch;
	}
	
	return FALSE;
}

void CToDoCtrl::OnGotoFileRef()
{
	if (!m_sFileRefPath.IsEmpty())
		GotoFile(m_sFileRefPath);
}

void CToDoCtrl::SetFocusToTasks()
{
	if (GetFocus() != &m_tree)
		m_tree.SetFocus();

	// ensure the selected tree item is visible
	if (GetSelectedCount())
		m_tree.TCH().EnsureVisibleEx(m_tree.GetSelectedItem(), FALSE);
	else
		SelectItem(m_tree.GetChildItem(NULL));

	m_tree.RedrawGutter();
}

void CToDoCtrl::SetFocusToComments()
{
	// fail if comments are not visible
	if (!::IsWindowVisible(m_ctrlComments))
		return;

	::SetFocus(m_ctrlComments);
}

CString CToDoCtrl::GetControlDescription(const CWnd* pCtrl) const
{
	CEnString sText; 

	if (IsChild(pCtrl))
	{
		// comments field
		if (::IsChild(m_ctrlComments, *pCtrl))
			sText.LoadString(IDS_TDC_FIELD_COMMENTS);

		else if (pCtrl == &m_tree)
			sText.LoadString(IDS_TASKTREE);

		else if (pCtrl == &m_cbCommentsType)
			sText.LoadString(IDS_COMMENTSTYPE);

		else
			sText = GetControlLabel(pCtrl);

		sText.Replace(_T("&"), _T(""));
	}

	return sText;
}

LRESULT CToDoCtrl::OnDropObject(WPARAM wParam, LPARAM lParam)
{
	if (IsReadOnly())
		return 0L;

	ASSERT(wParam && lParam);
	
	TLDT_DATA* pData = (TLDT_DATA*)wParam;
	CWnd* pTarget = (CWnd*)lParam;

	CString sFile = pData->GetFirstFile();
	BOOL bSingleFile = (!sFile.IsEmpty() && pData->GetFileCount() == 1);
	BOOL bSingleOObj = (pData->pOutlookSelection && pData->pOutlookSelection->GetCount() == 1);
	
	if (pTarget == &m_tree)
	{
		// check for single file or single outlook object being
		// dropped on a tree item which means update the item's file ref
		if (pData->hti && (bSingleFile || bSingleOObj))
		{
			SelectItem(pData->hti);

			if (bSingleOObj)
			{
				// format outlook link
				OutlookAPI::_MailItem item(pData->pOutlookSelection->Item(COleVariant((short)1)));
				sFile.Format(_T("outlook:%s"), COutlookHelper::GetItemID(item));
			}

			SetSelectedTaskFileRef(sFile);
		}
		else if (CreateTasksFromOutlookObjects(pData) != 0)
		{
			// nothing else to do
		}
		else if (pData->pObject)
		{
			// TODO
		}

		SetFocusToTasks();
	}
	else if (pTarget == &m_eFileRef && (bSingleFile || bSingleOObj))
	{
		if (bSingleOObj)
		{
			// format outlook link
			OutlookAPI::_MailItem item(pData->pOutlookSelection->Item(COleVariant((short)1)));
			sFile.Format(_T("outlook:%s"), COutlookHelper::GetItemID(item));
		}
		
		m_eFileRef.SetWindowText(sFile);
		m_eFileRef.SetFocus();
	}
	
	return 0;
}

int CToDoCtrl::CreateTasksFromOutlookObjects(TLDT_DATA* pData)
{
	// first figure out if we've got something to process
	// and grab its data for the dialog

	// NOTE: we've got to be a bit clever here because if the dialog
	// has been displayed before, the user may have elected to hide
	// confidential fields in which case we need to know this so as not
	// to ask for that data which in turn will trigger the outlook
	// security popup.

	// first thing to do is to get the first outlook object so we 
	// can prime the dialog
	COutlookHelper outlook;
	OutlookAPI::_MailItem* pItem = NULL;
	
	if (pData->pOutlookSelection)
		pItem = outlook.GetFirstObject(pData->pOutlookSelection);

	if (!pItem && pData->pFilePaths)
		pItem = outlook.GetFirstFileObject(*pData->pFilePaths);

	if (pItem == NULL)
		return 0; // nothing looking like an outlook object

	// display the mapping dialog and create the tasks
	CTDLImportOutlookObjectsDlg dialog(*pItem);
	CTDCCsvColumnMapping aMapping;
	
	if (dialog.DoModal() != IDOK || !dialog.GetColumnMapping(aMapping))
	{
		delete pItem;
		return -1; // cancel
	}

	BOOL bWantConfidential = dialog.GetWantConfidentialAttributes();

	// cleanup the temporary file object
	delete pItem;

	// accumulate the new tasks
	CTaskFile tasks;

	if (pData->pOutlookSelection)
	{
		// 1-based indexing
		for (short nSel = 1; nSel <= pData->pOutlookSelection->GetCount(); nSel++)
		{
			OutlookAPI::_MailItem item(pData->pOutlookSelection->Item(COleVariant(nSel)));

			VERIFY(CreateTaskFromOutlookObject(aMapping, &item, bWantConfidential, tasks));
		}
	}
	else if (pData->pFilePaths)
	{
		// 0-based indexing
		for (int nFile = 0; nFile < pData->GetFileCount(); nFile++)
		{
			OutlookAPI::_MailItem* pItem = outlook.GetFileObject(pData->GetFile(nFile));

			if (pItem)
				VERIFY(CreateTaskFromOutlookObject(aMapping, pItem, bWantConfidential, tasks));

			// cleanup
			delete pItem;
		}
	}

	// add to current tasklist
	if (tasks.GetTaskCount())
	{
		if (pData->hti)
		{
			SelectItem(pData->hti);
			VERIFY(InsertTasks(tasks, TDC_INSERTATBOTTOMOFSELTASK));
		}
		else
			VERIFY(InsertTasks(tasks, TDC_INSERTATBOTTOM));
	}

	return tasks.GetTaskCount();
}

BOOL CToDoCtrl::CreateTaskFromOutlookObject(const CTDCCsvColumnMapping& aMapping, OutlookAPI::_MailItem* pItem, BOOL bWantConfidential, CTaskFile& tasks)
{
	ASSERT(pItem);

	COutlookItemDataMap mapData;
	
	if (COutlookHelper::GetItemData(*pItem, mapData, bWantConfidential) == 0)
		return FALSE;

	// populate an empty task for the purpose of retrieving the data
	TODOITEM tdi(_T("outlook_task"));

	for (int nAttrib = 0; nAttrib < aMapping.GetSize(); nAttrib++)
	{
		const CSVCOLUMNMAPPING& attrib = aMapping[nAttrib];

		OUTLOOK_FIELDTYPE oaType = (OUTLOOK_FIELDTYPE)attrib.dwItemData;
		CString sData;
		CStringArray aData;
		
		if (attrib.nTDCAttrib == TDCA_NONE || !mapData.Lookup(oaType, sData) || sData.IsEmpty())
			continue;
		
		switch(attrib.nTDCAttrib)
		{
		case TDCA_TASKNAME:
			tdi.sTitle = sData;
			break;
			
		case TDCA_CATEGORY: 
			Misc::Split(sData, aData);
			tdi.aCategories.Append(aData);
			break;
			
		case TDCA_TAGS: 
			Misc::Split(sData, aData);
			tdi.aTags.Append(aData);
			break;
			
		case TDCA_STATUS: 
			tdi.sStatus = sData;
			break;
			
		case TDCA_EXTERNALID: 
			tdi.sExternalID = sData;
			break;
			
		case TDCA_ALLOCBY: 
			tdi.sAllocBy = sData;
			break;
			
		case TDCA_ALLOCTO: 
			Misc::Split(sData, aData);
			tdi.aAllocTo.Append(aData);
			break;
			
		case TDCA_VERSION: 
			tdi.sVersion = sData;
			break;
			
		case TDCA_FILEREF: 
			// special case:
			if (oaType == OA_ENTRYID)
				tdi.sFileRefPath.Format(_T("outlook:%s"), sData);
			else
				tdi.sFileRefPath = sData;
			break;
			
		case TDCA_DEPENDENCY: 
			Misc::Split(sData, aData);
			tdi.aDependencies.Append(aData);
			break;
			
		case TDCA_COMMENTS:
			if (!tdi.sComments.IsEmpty())
				tdi.sComments += '\n';
			else
				tdi.sCommentsTypeID = m_cfDefault;

			// special case:
			if (oaType == OA_ENTRYID)
			{
				CString sLink;
				sLink.Format(_T("outlook:%s"), sData);
				tdi.sComments += sLink;
			}
			else
			{
				tdi.sComments += CTDLImportOutlookObjectsDlg::GetOutlookFieldName(oaType);
				tdi.sComments += _T(": ");
				tdi.sComments += sData;
			}
			break;
			
		case TDCA_STARTDATE: 
			CDateHelper::DecodeDate(sData, tdi.dateStart);
			break;
			
		case TDCA_DUEDATE: 
			CDateHelper::DecodeDate(sData, tdi.dateDue);
			break;
			
		case TDCA_DONEDATE: 
			CDateHelper::DecodeDate(sData, tdi.dateDone);
			break;
			
		case TDCA_LASTMOD: 
			CDateHelper::DecodeDate(sData, tdi.dateLastMod);
			break;
			
		case TDCA_CREATIONDATE: 
			CDateHelper::DecodeDate(sData, tdi.dateCreated);
			break;
			
		case TDCA_PRIORITY: 
			// map 0->0, 1->5, 2->10
			tdi.nPriority = (_ttoi(sData) * 5);
			break;
			
		case TDCA_RISK: 
			tdi.nRisk = _ttoi(sData);
			break;
			
		case TDCA_FLAG: 
			tdi.bFlagged = !sData.IsEmpty();
			break;
			
		case TDCA_COST: 
			tdi.dCost = _ttof(sData);
			break;
			
		case TDCA_TIMEEST: 
			tdi.dTimeEstimate = _ttof(sData);
			break;
			
		case TDCA_TIMESPENT: 
			tdi.dTimeSpent = _ttof(sData);
			break;
		}
	}

	// handle attachments
	OutlookAPI::Attachments attachments(pItem->GetAttachments());

	for (short nAttach = 1; nAttach <= attachments.GetCount(); nAttach++)
	{
		OutlookAPI::Attachment item(attachments.Item(COleVariant(nAttach)));
		// item.SaveAsFile()
	}

	HTASKITEM hTask = tasks.NewTask(tdi.sTitle);
	ASSERT(hTask);

	if (!hTask)
		return FALSE;

	tasks.SetTaskAttributes(hTask, &tdi);
	return TRUE;
}

void CToDoCtrl::SaveSortState(CPreferences& prefs)
{
	// ignore this if we have no tasks
	if (GetTaskCount() == 0)
		return;
	
	// create a new key using the filepath
	ASSERT (GetSafeHwnd());
	
	CString sKey = GetPreferencesKey(_T("SortColState"));
	
	if (!sKey.IsEmpty())
	{
		prefs.WriteProfileInt(sKey, _T("Multi"), m_bMultiSort);
		prefs.WriteProfileInt(sKey, _T("Column"), m_sort.nBy1);
		prefs.WriteProfileInt(sKey, _T("Column2"), m_sort.nBy2);
		prefs.WriteProfileInt(sKey, _T("Column3"), m_sort.nBy3);
		prefs.WriteProfileInt(sKey, _T("Ascending"), m_sort.bAscending1);
		prefs.WriteProfileInt(sKey, _T("Ascending2"), m_sort.bAscending2);
		prefs.WriteProfileInt(sKey, _T("Ascending3"), m_sort.bAscending3);

		// delete old sort state
		sKey = GetPreferencesKey(_T("SortState"));
		prefs.DeleteSection(sKey);
	}
}

CString CToDoCtrl::GetPreferencesKey(const CString& sSubKey, const CString& sFilePath) const
{
	CString sKey;
	BOOL bRes = FALSE;

	// szFilePath takes preference
	if (!sFilePath.IsEmpty())
		bRes = GetPreferencesKey(sFilePath, sSubKey, sKey);

	// fallback on current filepath
	if (!bRes)
		bRes = GetPreferencesKey(m_sLastSavePath, sSubKey, sKey);

	ASSERT (!sKey.IsEmpty());
	
	return sKey;
}

BOOL CToDoCtrl::GetPreferencesKey(const CString& sFilePath, const CString& sSubKey, CString& sKey)
{
	CString sKeyPath = (sFilePath.IsEmpty() ? _T("Default") : CPreferences::KeyFromFile(sFilePath));
	
	if (!sSubKey.IsEmpty())
		sKey.Format(_T("FileStates\\%s\\%s"), sKeyPath, sSubKey);
	else
		sKey.Format(_T("FileStates\\%s"), sKeyPath);

	return (!sKey.IsEmpty());
}

// for backwards compatibility
TDC_COLUMN CToDoCtrl::MapSortByToColumn(int nSortBy)
{
	enum 
	{
		TDC_UNSORTED = 0,
		TDC_SORTBYNAME,
		TDC_SORTBYDONEDATE,
		TDC_SORTBYDUEDATE,
		TDC_SORTBYSTARTDATE,
		TDC_SORTBYPRIORITY,
		TDC_SORTBYCOLOR,
		TDC_SORTBYALLOCTO,
		TDC_SORTBYALLOCBY,
		TDC_SORTBYSTATUS,
		TDC_SORTBYCATEGORY,
		TDC_SORTBYPERCENT,
		TDC_SORTBYTIMEEST,
		TDC_SORTBYTIMESPENT,
		TDC_SORTBYID,
		TDC_SORTBYDONE,
		TDC_SORTBYFLAG,
		TDC_SORTBYCREATIONDATE,
		TDC_SORTBYCREATEDBY,
		TDC_SORTBYLASTMOD,
		TDC_SORTBYRISK,	
		TDC_SORTBYEXTERNALID,
		TDC_SORTBYCOST,
		TDC_SORTBYVERSION,
		TDC_SORTBYRECURRENCE,
		TDC_SORTBYRECENTEDIT,
		TDC_SORTBYREMAINING,
		TDC_SORTBYICON,
		TDC_SORTBYPARENTID,
		TDC_SORTBYTIMETRACKING,
		TDC_SORTBYFILEREF,
		TDC_SORTBYPATH,
		TDC_SORTBYPOSITION,
		TDC_SORTBYTAG,
	};

	switch (nSortBy)
	{
	case TDC_SORTBYNAME:		return TDCC_CLIENT;		
	case TDC_SORTBYPRIORITY:	return TDCC_PRIORITY;		
	case TDC_SORTBYPERCENT:		return TDCC_PERCENT;		
	case TDC_SORTBYTIMEEST:		return TDCC_TIMEEST;		
	case TDC_SORTBYTIMESPENT:	return TDCC_TIMESPENT;	
	case TDC_SORTBYSTARTDATE:	return TDCC_STARTDATE;	
	case TDC_SORTBYDUEDATE:		return TDCC_DUEDATE;		
	case TDC_SORTBYDONEDATE:	return TDCC_DONEDATE;		
	case TDC_SORTBYALLOCTO:		return TDCC_ALLOCTO;		
	case TDC_SORTBYALLOCBY:		return TDCC_ALLOCBY;		
	case TDC_SORTBYSTATUS:		return TDCC_STATUS;		
	case TDC_SORTBYCATEGORY:	return TDCC_CATEGORY;		
	case TDC_SORTBYID:			return TDCC_ID;			
	case TDC_SORTBYFLAG:		return TDCC_FLAG;			
	case TDC_SORTBYCREATIONDATE:return TDCC_CREATIONDATE; 
	case TDC_SORTBYCREATEDBY:	return TDCC_CREATEDBY;	
	case TDC_SORTBYLASTMOD:		return TDCC_LASTMOD;		
	case TDC_SORTBYRISK:		return TDCC_RISK;			
	case TDC_SORTBYEXTERNALID:	return TDCC_EXTERNALID;	
	case TDC_SORTBYCOST:		return TDCC_COST;			
	case TDC_SORTBYRECURRENCE:	return TDCC_RECURRENCE;	
	case TDC_SORTBYVERSION:		return TDCC_VERSION;		
	case TDC_SORTBYRECENTEDIT:	return TDCC_RECENTEDIT;	
	case TDC_SORTBYREMAINING:	return TDCC_REMAINING;		
	case TDC_SORTBYDONE:		return TDCC_DONE;		
	case TDC_SORTBYICON:		return TDCC_ICON;		
	case TDC_SORTBYFILEREF:		return TDCC_FILEREF;		
	case TDC_SORTBYTIMETRACKING:return TDCC_TRACKTIME;
	case TDC_SORTBYPARENTID:	return TDCC_PARENTID;
	case TDC_SORTBYTAG:			return TDCC_TAGS;
	}

	// everything else
	return TDCC_NONE;
}

void CToDoCtrl::LoadSortState(const CPreferences& prefs, const CString& sFilePath)
{
	CString sKey = GetPreferencesKey(_T("SortColState"), sFilePath);
	
	if (!sKey.IsEmpty() && prefs.HasSection(sKey))
	{
		m_bMultiSort = prefs.GetProfileInt(sKey, _T("Multi"), FALSE);
		m_sort.nBy1 = (TDC_COLUMN)prefs.GetProfileInt(sKey, _T("Column"), TDCC_NONE);
		m_sort.nBy2 = (TDC_COLUMN)prefs.GetProfileInt(sKey, _T("Column2"), TDCC_NONE);
		m_sort.nBy3 = (TDC_COLUMN)prefs.GetProfileInt(sKey, _T("Column3"), TDCC_NONE);
		m_sort.bAscending1 = prefs.GetProfileInt(sKey, _T("Ascending"), TRUE);
		m_sort.bAscending2 = prefs.GetProfileInt(sKey, _T("Ascending2"), TRUE);
		m_sort.bAscending3 = prefs.GetProfileInt(sKey, _T("Ascending3"), TRUE);

		return;
	}

	// backward compatibility
	sKey = GetPreferencesKey(_T("SortState"), sFilePath);
	
	if (!sKey.IsEmpty() && prefs.HasSection(sKey))
	{
		m_bMultiSort = prefs.GetProfileInt(sKey, _T("Multi"), FALSE);

		int nSortBy = prefs.GetProfileInt(sKey, _T("Column"), 0);
		m_sort.nBy1 = MapSortByToColumn(nSortBy);

		nSortBy = prefs.GetProfileInt(sKey, _T("Column2"), 0);
		m_sort.nBy2 = MapSortByToColumn(nSortBy);

		nSortBy = prefs.GetProfileInt(sKey, _T("Column3"), 0);
		m_sort.nBy3 = MapSortByToColumn(nSortBy);

		m_sort.bAscending1 = prefs.GetProfileInt(sKey, _T("Ascending"), TRUE);
		m_sort.bAscending2 = prefs.GetProfileInt(sKey, _T("Ascending2"), TRUE);
		m_sort.bAscending3 = prefs.GetProfileInt(sKey, _T("Ascending3"), TRUE);
	}
}

void CToDoCtrl::SaveSplitPos(CPreferences& prefs)
{
	ASSERT (GetSafeHwnd());
	
	CString sKey = GetPreferencesKey(); // no subkey
	
	if (!sKey.IsEmpty())
		prefs.WriteProfileInt(sKey, _T("SplitPos"), m_nCommentsSize);

	if (HasStyle(TDCS_SHAREDCOMMENTSHEIGHT))
		prefs.WriteProfileInt(_T("FileStates"), _T("SharedSplitPos"), s_nCommentsSize);
}

void CToDoCtrl::LoadSplitPos(const CPreferences& prefs)
{
	s_nCommentsSize = prefs.GetProfileInt(_T("FileStates"), _T("SharedSplitPos"), DEFCOMMENTSIZE);

	CString sKey = GetPreferencesKey(); // no subkey
	
	if (!sKey.IsEmpty())
		m_nCommentsSize = prefs.GetProfileInt(sKey, _T("SplitPos"), DEFCOMMENTSIZE);
	else
		m_nCommentsSize = s_nCommentsSize;
}

void CToDoCtrl::SaveExpandedState(CPreferences& prefs)
{
	ASSERT (GetSafeHwnd());
	
	// ignore this if we have no tasks
	if (GetTaskCount() == 0)
		return;
	
	// create a new key using the filepath and simply save the ID 
	// of every expanded item
	CString sKey = GetPreferencesKey(_T("ExpandedState"));
	
	if (!sKey.IsEmpty())
	{
		// remove previous expanded state
		prefs.DeleteSection(sKey);

		// save items themselves
		int nCount = SaveTreeExpandedState(prefs, sKey);
		
		// save expanded count
		prefs.WriteProfileInt(sKey, _T("Count"), nCount);
		
		// and selected item
		prefs.WriteProfileInt(sKey, _T("SelItem"), GetSelectedTaskID());
	}
}

HTREEITEM CToDoCtrl::LoadExpandedState(const CPreferences& prefs)
{
	ASSERT (GetSafeHwnd());
	
	CString sKey = GetPreferencesKey(_T("ExpandedState"));
	
	if (!sKey.IsEmpty())
		LoadTreeExpandedState(prefs, sKey);

	// restore prev selected item
	DWORD dwSel = prefs.GetProfileInt(sKey, _T("SelItem"), 0);
	return m_find.GetItem(dwSel);
}

void CToDoCtrl::LoadTreeExpandedState(const CPreferences& prefs, const CString& sKey)
{
	ASSERT (!sKey.IsEmpty());

	int nCount = prefs.GetProfileInt(sKey, _T("Count"), 0);

	if (!nCount || !m_tree.GetCount())
		return;

	HOLD_REDRAW(*this, m_tree);

	CHTIMap map;
	m_tree.TCH().BuildHTIMap(map);

	for (int nItem = 0; nItem < nCount; nItem++)
	{
		CString sItem = Misc::MakeKey(_T("Item%d"), nItem);
		DWORD dwTaskID = (DWORD)prefs.GetProfileInt(sKey, sItem, 0);
		HTREEITEM hti = NULL;
		
		if (dwTaskID && map.Lookup(dwTaskID, hti) && hti)
		{
		//	m_tree.Expand(hti, TVE_EXPAND);
			m_tree.TCH().SetItemStateEx(hti, TVIS_EXPANDED, TVIS_EXPANDED);
		}
	}
}

int CToDoCtrl::SaveTreeExpandedState(CPreferences& prefs, const CString& sKey, HTREEITEM hti, int nStart)
{
	HTREEITEM htiChild = m_tree.GetChildItem(hti);
	int nCount = nStart;
	
	while (htiChild)
	{
		if (m_tree.TCH().IsItemExpanded(htiChild) > 0)
		{
			CString sItem = Misc::MakeKey(_T("Item%d"), nCount);
			
			prefs.WriteProfileInt(sKey, sItem, (int)GetTaskID(htiChild));
			nCount++;
			
			// now its children
			nCount += SaveTreeExpandedState(prefs, sKey, htiChild, nCount);
		}
		
		htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
	}	
	
	return (nCount - nStart);
}


LRESULT CToDoCtrl::OnGutterWidthChange(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
	// let parent know if min width has changed
// 	int nPrevWidth = LOWORD(lParam);
// 	int nNewWidth = HIWORD(lParam);
// 	
// 	if (nNewWidth < nPrevWidth || (!nPrevWidth && nNewWidth))
// 		GetParent()->SendMessage(WM_TDCN_MINWIDTHCHANGE);

	return 0;
}

LRESULT CToDoCtrl::OnGutterGetCursor(WPARAM /*wParam*/, LPARAM lParam)
{
	NCGGETCURSOR* pNCGGC = (NCGGETCURSOR*)lParam;
	HTREEITEM hti = (HTREEITEM)pNCGGC->dwItem;
	BOOL bShowHand = FALSE;

	switch (pNCGGC->nColID)
	{
	case TDCC_FILEREF:
		// we handle the file ref column if the ctrl key is down
		if (Misc::KeyIsPressed(VK_CONTROL) && hti)
		{
			DWORD dwTaskID = GetTaskID(hti);
			const TODOITEM* pTDI = GetTask(dwTaskID);
			ASSERT (pTDI);
			
			bShowHand = (pTDI && !pTDI->sFileRefPath.IsEmpty());
		}
		break;
		
	case TDCC_DEPENDENCY:
		// we handle the depends column if the ctrl key is down
		if (Misc::KeyIsPressed(VK_CONTROL) && hti)
		{
			DWORD dwTaskID = GetTaskID(hti);
			const TODOITEM* pTDI = GetTask(dwTaskID);
			ASSERT (pTDI);
			
			bShowHand = (pTDI && pTDI->aDependencies.GetSize() == 1);
		}
		break;
		
	case TDCC_TRACKTIME:
		// check tasklist is editable, task is trackable and 
		// neither the ctrl not shift keys are pressed (ie => multiple selection)
		bShowHand = (Misc::ModKeysArePressed(0) && !IsReadOnly() && m_data.IsTaskTimeTrackable(GetTaskID(hti)));
		break;

	case TDCC_ICON:
	case TDCC_FLAG:
		bShowHand = (!IsReadOnly() && hti!= NULL);
		break;

	default:
		// handle 'flag' custom attributes
		if (!IsReadOnly() && CTDCCustomAttributeHelper::IsCustomColumn((TDC_COLUMN)pNCGGC->nColID))
		{
			TDCCUSTOMATTRIBUTEDEFINITION attribDef;
			VERIFY (CTDCCustomAttributeHelper::GetAttributeDef((TDC_COLUMN)pNCGGC->nColID, m_aCustomAttribDefs, attribDef));
			
			switch (attribDef.GetDataType())
			{
			case TDCCA_BOOL:
			case TDCCA_ICON:
				bShowHand = TRUE;
				break;
			}
		}
	}

	if (bShowHand)
		return (LRESULT)GraphicsMisc::HandCursor();

	// all else
	return 0L;
}

void CToDoCtrl::EndLabelEdit(BOOL bCancel)
{
	m_eTaskName.EndEdit(bCancel);
}

void CToDoCtrl::Flush(BOOL bEndTimeTracking, BOOL bResetCalcs) // called to end current editing actions
{
	CWnd* pFocus = GetFocus();

	if (pFocus)
	{
		if (pFocus == &m_eTaskName)
			EndLabelEdit(FALSE);

		else if (m_cbCategory.IsChild(pFocus))
			m_cbCategory.Flush();
		
		else if (m_cbTags.IsChild(pFocus))
			m_cbTags.Flush();
		
		else if (m_cbStatus.IsChild(pFocus))
			m_cbStatus.Flush();
		
		else if (m_cbAllocBy.IsChild(pFocus))
			m_cbAllocBy.Flush();
		
		else if (m_cbAllocTo.IsChild(pFocus))
			m_cbAllocTo.Flush();

		// flush custom attributes also
		// TODO
	}

	m_treeDragDrop.CancelDrag();

	HandleUnsavedComments();

	if (bResetCalcs)
		ResetCachedCalculations();

	if (bEndTimeTracking)
		EndTimeTracking();
}

CString CToDoCtrl::GetSourceControlID(BOOL bAlternate) const
{
	if (HasStyle(TDCS_INCLUDEUSERINCHECKOUT))
	{
		if (!bAlternate)
			return Misc::FormatComputerNameAndUser();

		// else
		return Misc::GetComputerName();
	}

	// else
	if (!bAlternate)
		return Misc::GetComputerName();

	// else
	return Misc::FormatComputerNameAndUser();
}

BOOL CToDoCtrl::CheckIn()
{
	ASSERT (HasStyle(TDCS_ENABLESOURCECONTROL) && !HasStyle(TDCS_USES3RDPARTYSOURCECONTROL));

	if (!HasStyle(TDCS_ENABLESOURCECONTROL) || HasStyle(TDCS_USES3RDPARTYSOURCECONTROL))
		return FALSE;
	
	if (m_sLastSavePath.IsEmpty())
		return TRUE;
	
	CTaskFile file(m_sPassword);
	
	// backup the file
	CTempFileBackup backup(m_sLastSavePath);

	// load the file in its encrypted state because we're not interested 
	// in changing that bit of it
	if (file.Open(m_sLastSavePath, XF_READWRITE, FALSE) && file.LoadEx())
	{
		m_bSourceControlled = file.IsSourceControlled();

		if (m_bSourceControlled)
		{
			CString sCheckedOutTo = file.GetCheckOutTo();
			
			if (sCheckedOutTo.IsEmpty() || 
				m_bCheckedOut && 
				(sCheckedOutTo == GetSourceControlID() || sCheckedOutTo == GetSourceControlID(TRUE))) // its us
			{
				file.SetCheckedOutTo(""); // retain the item
				
				// and rewrite the file but keeping the same timestamp
				FILETIME ftMod;
				::GetFileTime((HANDLE)file.GetFileHandle(), NULL, NULL, &ftMod);
				
				m_bCheckedOut = !file.SaveEx(); // ie if successfully saved then m_bCheckedOut == 0
				
				::SetFileTime((HANDLE)file.GetFileHandle(), NULL, NULL, &ftMod);
				
				return !m_bCheckedOut;
			}
		}
	}
	
	// else someone else or invalid file
	return FALSE;
}

BOOL CToDoCtrl::CheckOut()
{
	CString sTemp;
	return CheckOut(sTemp);
}

BOOL CToDoCtrl::RemoveFromSourceControl()
{
	// can't be checked out to someone
	if (IsCheckedOut())
		return FALSE;

	m_bSourceControlled = FALSE;

	if (!m_sLastSavePath.IsEmpty())
		return (Save() == TDCO_SUCCESS);

	// else
	return TRUE;
}

BOOL CToDoCtrl::CheckOut(CString& sCheckedOutTo)
{
	if (m_bDelayLoaded)
		return FALSE;

	ASSERT (HasStyle(TDCS_ENABLESOURCECONTROL) && !HasStyle(TDCS_USES3RDPARTYSOURCECONTROL));

	if (!HasStyle(TDCS_ENABLESOURCECONTROL) || HasStyle(TDCS_USES3RDPARTYSOURCECONTROL))
		return FALSE;
	
	if (m_sLastSavePath.IsEmpty())
		return TRUE;
	
	CTaskFile file(m_sPassword);
	CWaitCursor cursor;
	
	// backup the file
	CTempFileBackup backup(m_sLastSavePath);

	if (file.Open(m_sLastSavePath, XF_READWRITE, FALSE) && file.LoadEx())
	{
		m_bCheckedOut = file.CheckOut(GetSourceControlID(), sCheckedOutTo);
		m_bSourceControlled = file.IsSourceControlled();

		if (m_bCheckedOut)
		{
		    file.Decrypt();
			
			LoadTasks(file); // reload file
			return TRUE;
		}
	}
	
	return FALSE;
}

int CToDoCtrl::FindTasks(const SEARCHPARAMS& params, CResultArray& aResults) const
{
	return m_find.FindTasks(params, aResults);
}

DWORD CToDoCtrl::FindFirstTask(const SEARCHPARAMS& params, SEARCHRESULT& result) const
{
	return m_find.FindFirstTask(params, result);
}

BOOL CToDoCtrl::SelectTask(CString sPart, TDC_SELECTTASK nSelect)
{
	FileMisc::LogText(_T("CToDoCtrl::SelectTask(Begin)"), sPart);

	SEARCHRESULT result;
	DWORD dwFoundID = 0;

	// build a search query
	SEARCHPARAMS params;
	params.aRules.Add(SEARCHPARAM(TDCA_TASKNAMEORCOMMENTS, FO_INCLUDES, sPart));

	switch (nSelect)
	{
	case TDC_SELECTFIRST:
		FileMisc::LogText(_T("\tCToDoCtrl::SelectTask(%s, TDC_SELECTFIRST)"), sPart);
		dwFoundID = m_find.FindFirstTask(params, result);
		break;

	case TDC_SELECTNEXT:
	case TDC_SELECTNEXTINCLCURRENT:
	case TDC_SELECTPREV:
		{
			HTREEITEM hti = GetSelectedItem(); // our start point

			if (!hti)
				return FALSE;

			// we want to start searching from the next item
			switch (nSelect)
			{
				case TDC_SELECTNEXT:
					FileMisc::LogText(_T("\tCToDoCtrl::SelectTask(%s, TDC_SELECTNEXT)"), sPart);
					hti = m_tree.TCH().GetNextItem(hti);
					break;

 				case TDC_SELECTNEXTINCLCURRENT:
					FileMisc::LogText(_T("\tCToDoCtrl::SelectTask(%s, TDC_SELECTNEXTINCLCURRENT)"), sPart);
 					// nothing to do
 					break;

				case TDC_SELECTPREV:
					FileMisc::LogText(_T("\tCToDoCtrl::SelectTask(%s, TDC_SELECTPREV)"), sPart);
					hti = m_tree.TCH().GetPrevItem(hti);
					break;
			}

			if (!hti)
				return FALSE; // at the end/start

			BOOL bNext = (nSelect != TDC_SELECTPREV);
			dwFoundID = m_find.FindNextTask(GetTaskID(hti), params, result, bNext);
		}
		break;

	case TDC_SELECTLAST:
		{
			FileMisc::LogText(_T("\tCToDoCtrl::SelectTask(%s, TDC_SELECTLAST)"), sPart);
			HTREEITEM hti = m_tree.TCH().GetLastItem(); // our start point

			if (!hti)
				return FALSE;

			dwFoundID = m_find.FindNextTask(GetTaskID(hti), params, result, FALSE);
		}
		break;

	default:
		ASSERT(0);
		break;
	}

	if (dwFoundID)
	{
		FileMisc::LogText(_T("\tCToDoCtrl::SelectTask(Task = %ld)"), dwFoundID);
		SelectTask(dwFoundID);
	}

	FileMisc::LogText(_T("CToDoCtrl::SelectTask(End)"), sPart);

	return (dwFoundID != 0);
}

void CToDoCtrl::OnLButtonDown(UINT nFlags, CPoint point) 
{
	// is it over the comments splitter
	ASSERT (!m_bSplitting);

	BOOL bEnable = (m_nMaxState == TDCMS_NORMAL) || 
					(m_nMaxState == TDCMS_MAXTASKLIST && HasStyle(TDCS_SHOWCOMMENTSALWAYS));
	
	if (bEnable && GetSplitterRect().PtInRect(point))
	{
		m_bSplitting = TRUE;
		SetCapture();
	}
	
	CRuntimeDlg::OnLButtonDown(nFlags, point);
}

void CToDoCtrl::OnLButtonUp(UINT nFlags, CPoint point) 
{
	if (m_bSplitting)
	{
		ReleaseCapture();
		m_bSplitting = FALSE;

        SaveSplitPos(CPreferences());
	}
	
	CRuntimeDlg::OnLButtonUp(nFlags, point);
}

void CToDoCtrl::OnMouseMove(UINT nFlags, CPoint point)
{
	if (m_bSplitting)
	{
		CRect rComments, rClient, rSplitter = GetSplitterRect();
		GetClientRect(rClient);

		int nNewSize, nMaxCommentSize;
		int nCommentsSize = HasStyle(TDCS_SHAREDCOMMENTSHEIGHT) ? s_nCommentsSize : m_nCommentsSize;
		
		switch (m_nCommentsPos)
		{
		case TDCUIL_RIGHT: // vertical
			{
				nMaxCommentSize  = rClient.Width() - GetMinNonCommentSize();
				nNewSize = nCommentsSize - (point.x - rSplitter.left);
			}
			break;
			
		case TDCUIL_LEFT: // vertical
			{
				nMaxCommentSize  = rClient.Width() - GetMinNonCommentSize();
				nNewSize = nCommentsSize + (point.x - rSplitter.right);
			}
			break;
			
		case TDCUIL_BOTTOM: // horizontal
		default:
			{
				nMaxCommentSize  = rClient.Height() - GetMinNonCommentSize();
				nNewSize = nCommentsSize - (point.y - rSplitter.top);
			}
			break;
		}

		nNewSize = min(max(nNewSize, MINCOMMENTSIZE), nMaxCommentSize);
		
		if (nNewSize != nCommentsSize)
		{
			m_nCommentsSize = s_nCommentsSize = nNewSize;
			Resize();
			UpdateWindow();
		}
	}
	
	CRuntimeDlg::OnMouseMove(nFlags, point);
}

CRect CToDoCtrl::GetSplitterRect()
{
	CRect rSplitter;
		
	switch (m_nCommentsPos)
	{
	case TDCUIL_RIGHT: // vertical
		{
			::GetWindowRect(m_ctrlComments, rSplitter);
			ScreenToClient(rSplitter);
			
			rSplitter.right = rSplitter.left;
			rSplitter.left -= SPLITSIZE;
		}
		break;
		
	case TDCUIL_LEFT:
		{
			::GetWindowRect(m_ctrlComments, rSplitter);
			ScreenToClient(rSplitter);
			
			rSplitter.left = rSplitter.right;
			rSplitter.right += SPLITSIZE;
		}
		break;
		
	case TDCUIL_BOTTOM: // horizontal
		{
			rSplitter = GetCtrlRect(IDC_SPLITTER);

			rSplitter.bottom++;
			rSplitter.top--;
		}
		break;
	}
	
	return rSplitter;
}

BOOL CToDoCtrl::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message) 
{
	BOOL bEnableSplit = (m_nMaxState == TDCMS_NORMAL) || 
						(m_nMaxState == TDCMS_MAXTASKLIST && HasStyle(TDCS_SHOWCOMMENTSALWAYS));

	if (bEnableSplit)
	{
		CPoint point(::GetMessagePos());
		ScreenToClient(&point);
		
		if (GetSplitterRect().PtInRect(point))
		{
			switch (m_nCommentsPos)
			{
			case TDCUIL_RIGHT: // vertical
			case TDCUIL_LEFT:
				SetCursor(AfxGetApp()->LoadCursor(AFX_IDC_HSPLITBAR));
				break;
				
			case TDCUIL_BOTTOM: // horizontal
				SetCursor(AfxGetApp()->LoadCursor(AFX_IDC_VSPLITBAR));
				break;
			}
	
			return TRUE;
		}
	}
	
	// else
	return CRuntimeDlg::OnSetCursor(pWnd, nHitTest, message);
}

void CToDoCtrl::OnCaptureChanged(CWnd *pWnd) 
{
	if (m_bSplitting)
	{
		// save latest split pos
		s_nCommentsSize = m_nCommentsSize;
		m_bSplitting = FALSE;
	}
	
	CRuntimeDlg::OnCaptureChanged(pWnd);
}

void CToDoCtrl::OnShowWindow(BOOL bShow, UINT nStatus) 
{
	CRuntimeDlg::OnShowWindow(bShow, nStatus);
	
	if (bShow)
	{
		// resize if we have shared splitter pos and its different
		if (HasStyle(TDCS_SHAREDCOMMENTSHEIGHT) && (m_nCommentsSize != s_nCommentsSize))
		{
			m_nCommentsSize = s_nCommentsSize;
			Resize();
		}

		// re-calc the tree nc border if the comments are to the right
		// to 'fix' a UI quirkf
		if (m_nCommentsPos == TDCUIL_LEFT || m_nCommentsPos == TDCUIL_RIGHT)
			m_tree.SetWindowPos(NULL, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER); 
	}
}

LRESULT CToDoCtrl::OnTimeUnitsChange(WPARAM wParam, LPARAM /*lParam*/)
{
	int nRecalcTime = IDNO;
	BOOL bWantQueryRecalc = (GetSelectedCount() > 1);
	
	if (!bWantQueryRecalc) // one item selected
	{
		TCHAR nDummy;

		// see if the time is non-zero and if so we prompt
		switch (wParam)
		{
		case IDC_TIMEEST:
			bWantQueryRecalc = (GetSelectedTaskTimeEstimate(nDummy) > 0.0);
			break;
		
		case IDC_TIMESPENT:
			bWantQueryRecalc = (GetSelectedTaskTimeSpent(nDummy) > 0.0);
			break;
		}
	}

	if (bWantQueryRecalc)
		nRecalcTime = MessageBox(CEnString(IDS_TDC_RECALCPROMPT), CEnString(IDS_TDC_RECALCTITLE), MB_ICONQUESTION | MB_YESNOCANCEL);

	if (nRecalcTime != IDCANCEL)
	{
		DWORD dwFlags = UTF_TIMEUNITSONLY | (nRecalcTime == IDYES ? UTF_RECALCTIME : 0);

		switch (wParam)
		{
		case IDC_TIMEEST:
			UpdateTask(TDCA_TIMEEST, dwFlags); 
			break;
		
		case IDC_TIMESPENT:
			UpdateTask(TDCA_TIMESPENT, dwFlags); 
			break;
		}

	}
	
	return (nRecalcTime == IDCANCEL);
}

void CToDoCtrl::SpellcheckSelectedTask(BOOL bTitle) 
{
	Flush();

	IMPLEMENT_UNDO(TDCUAT_EDIT);

	// one off spell check
	CSpellCheckDlg dialog;
	dialog.SetDictionaryDownloadUrl(_T("http://abstractspoon.pbworks.com/w/page/1262231/Spellcheck-Dictionaries"));
	
	SpellcheckItem(GetSelectedItem(), &dialog, bTitle, TRUE);
}

LRESULT CToDoCtrl::OnCommentsWantSpellCheck(WPARAM /*wParam*/, LPARAM /*lParam*/)
{
	SpellcheckSelectedTask(FALSE); 

	return 0L;
}

BOOL CToDoCtrl::CanSpellcheckSelectedTaskComments()
{
	if (!CanSpellcheckComments())
		return FALSE;

	return !GetSelectedTaskComments().IsEmpty();
}

BOOL CToDoCtrl::CanSpellcheckComments()
{
	return (NULL != m_ctrlComments.GetSpellCheckInterface());
}

void CToDoCtrl::Spellcheck()
{
	IMPLEMENT_UNDO(TDCUAT_EDIT);

	// top level items
	CSpellCheckDlg dialog;
	dialog.SetDictionaryDownloadUrl(_T("http://abstractspoon.pbworks.com/w/page/1262231/Spellcheck-Dictionaries"));

	HTREEITEM hti = m_tree.GetChildItem(NULL);
	
	while (hti)
	{
		if (!SpellcheckItem(hti, &dialog))
			return;
		
		hti = m_tree.GetNextItem(hti, TVGN_NEXT);
	}
}

BOOL CToDoCtrl::SpellcheckItem(HTREEITEM hti, CSpellCheckDlg* pSpellChecker, BOOL bTitle, BOOL bNotifyNoErrors)
{
	ASSERT(pSpellChecker);
	
	if (!pSpellChecker)
		return FALSE;

	if (!bTitle && !CanSpellcheckComments())
		return TRUE;
	
	DWORD dwTaskID = GetTaskID(hti);
	const TODOITEM* pTDI = m_data.GetTask(dwTaskID);
	ASSERT (pTDI);
	
	if (pTDI)
	{
		if (bTitle)
		{
			CString sTitle = m_data.GetTaskTitle(dwTaskID);
			
			if (sTitle.IsEmpty())
				return TRUE;

			// else
			SelectItem(hti);
			pSpellChecker->SetText(sTitle);
		}
		else
		{
			SelectItem(hti);
			pSpellChecker->SetSpellCheck(m_ctrlComments.GetSpellCheckInterface());
		}
		
		int nRet = pSpellChecker->DoModal(TRUE);
		UpdateWindow();
			
		if (nRet == IDOK && !IsReadOnly())
		{
			int nChange = SET_NOCHANGE;
			
			if (bTitle)
			{
				CString sTitle = pSpellChecker->GetCorrectedText();
				nChange = m_data.SetTaskTitle(dwTaskID, sTitle);
				
				if (nChange == SET_CHANGE)
				{
					InvalidateItem(hti);
					m_tree.UpdateWindow();
				}
			}
			else if (pSpellChecker->MadeChanges()) 
			{
				UpdateTask(TDCA_COMMENTS);
				nChange = SET_CHANGE;
			}
			
			if (nChange == SET_CHANGE)
				SetModified(TRUE, TDCA_TASKNAME, dwTaskID);
		}
		else if (nRet == IDNOERRORS && bNotifyNoErrors)
		{
			MessageBoxEx(this, bTitle ? IDS_TDC_NOTITLESPELLERRORS : IDS_TDC_NOCOMMENTSPELLERRORS, 
				IDS_TDC_SPELLCHECK_TITLE);
		}
		else if (nRet == IDCANCEL)
			return FALSE;
		
		return TRUE;
	}
	
	// else
	ASSERT(0);
	return FALSE;
}

BOOL CToDoCtrl::SpellcheckItem(HTREEITEM hti, CSpellCheckDlg* pSpellChecker)
{
	if (!SpellcheckItem(hti, pSpellChecker, TRUE, FALSE) || 
		!SpellcheckItem(hti, pSpellChecker, FALSE, FALSE))
		return FALSE;
	
	// subtasks
	HTREEITEM htiSub = m_tree.GetChildItem(hti);
	
	while (htiSub)
	{
		if (!SpellcheckItem(htiSub, pSpellChecker))
			return FALSE;
		
		htiSub = m_tree.GetNextItem(htiSub, TVGN_NEXT);
	}
	
	return TRUE;
}

void CToDoCtrl::SetDefaultTaskAttributes(const TODOITEM& tdi)
{
	s_tdDefault = tdi;

	// set default task creation date to zero so that new tasks
	// always get the current date
	s_tdDefault.dateCreated.m_dt = 0.0;

	// pass on default time units for calculations
	m_data.SetDefaultTimeUnits(tdi.nTimeEstUnits, tdi.nTimeSpentUnits);
}

LRESULT CToDoCtrl::OnEEBtnClick(WPARAM wParam, LPARAM lParam)
{
	switch (wParam)
	{
	case IDC_TIMESPENT:
		if (lParam == ID_TIME_TRACK && GetSelectedCount() == 1)
		{
			HandleUnsavedComments();
			TimeTrackTask(GetSelectedItem());
		}
		break;

	case IDC_DEPENDS:
		GotoSelectedTaskDependency();
		break;
	}
	
	return 0L;
}

BOOL CToDoCtrl::GotoSelectedTaskDependency()
{
	CStringArray aDepends;
	GetSelectedTaskDependencies(aDepends);
	
	if (aDepends.GetSize())
		return ShowTaskLink(0, aDepends[0]);

	// else
	return FALSE;
}

BOOL CToDoCtrl::EditSelectedTaskRecurrence()
{
	if (!IsReadOnly())
		return m_eRecurrence.DoEdit(TRUE); // TRUE forces an edit

	// else
	return FALSE;
}

void CToDoCtrl::OnTimer(UINT nIDEvent) 
{
	switch (nIDEvent)
	{
	case TIMER_TRACK:
		IncrementTrackedTime();
		break;

	case TIMER_MIDNIGHT:
		{
			// check if we've just passed midnight, in which case some tasks
			// may have just become due
			static time_t tPrev = time(NULL); // once only
			time_t tNow = time(NULL);

			if (tNow / DAY_IN_SECS > tPrev / DAY_IN_SECS)
			{
				OnTimerMidnight();
			}

			tPrev = tNow;
		}
		break;
	}

	
	CRuntimeDlg::OnTimer(nIDEvent);
}

void CToDoCtrl::OnTimerMidnight()
{
	m_data.ResetCachedCalculations();
	Invalidate();
}

void CToDoCtrl::IncrementTrackedTime()
{
	// if we are editing the title of the task being tracked then leave immediately
	// and wait until the editing has ended
	if (IsTaskLabelEditing() && m_dwTimeTrackTaskID == GetSelectedTaskID())
		return;

	DWORD dwTick = GetTickCount();
	
	if (IsActivelyTimeTracking())
	{
		TCHAR nUnits = 0;
		double dTime = m_data.GetTaskTimeSpent(m_dwTimeTrackTaskID, nUnits);
		
		ASSERT (m_dwTickLast);
		double dInc = (dwTick - m_dwTickLast) * TICKS2HOURS;
		
		dTime = CTimeHelper().GetTime(dTime, nUnits, THU_HOURS);
		dTime += dInc;
		m_dLogTime += dInc;
		dTime = CTimeHelper().GetTime(dTime, THU_HOURS, nUnits);
		
		if (m_dwTimeTrackTaskID == GetSelectedTaskID() && GetSelectedCount() == 1)
			SetSelectedTaskTimeSpent(dTime, nUnits);
		else
		{
			m_data.SetTaskTimeSpent(m_dwTimeTrackTaskID, dTime, nUnits);
			SetModified(TRUE, TDCA_TIMESPENT, m_dwTimeTrackTaskID);
		}
	}
	
	m_dwTickLast = dwTick;
}

LRESULT CToDoCtrl::OnCustomUrl(WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(wParam);
	ASSERT(wParam == (WPARAM)m_ctrlComments.GetDlgCtrlID());
	
	// FALSE says only handle "tdc://" links
	GotoFile((LPCTSTR)lParam, FALSE);
	
	return 0;
}

void CToDoCtrl::SelectNextTasksInHistory() 
{ 
	HandleUnsavedComments();

	if (Selection().NextSelection())
		m_tree.RedrawGutter();
}

void CToDoCtrl::SelectPrevTasksInHistory() 
{ 
	HandleUnsavedComments();

	if (Selection().PrevSelection())
		m_tree.RedrawGutter();
}

LRESULT CToDoCtrl::OnFileEditWantIcon(WPARAM wParam, LPARAM lParam)
{
	if (wParam == IDC_FILEPATH)
	{
		const CString& sUrl = (LPCTSTR)lParam;
		
		if (sUrl.Find(TDL_PROTOCOL) != -1)
			return (LRESULT)AfxGetMainWnd()->SendMessage(WM_GETICON, ICON_SMALL, 0);
	}
	
	return 0;
}

LRESULT CToDoCtrl::OnFileEditDisplayFile(WPARAM wParam, LPARAM lParam)
{
	if (wParam == IDC_FILEPATH)
	{
		// FALSE says only handle "tdc://" links
		return GotoFile((LPCTSTR)lParam, FALSE);
	}
	
	return 0;
}

BOOL CToDoCtrl::GotoFile(const CString& sUrl, BOOL bShellExecute)
{
	if (sUrl.IsEmpty())
		return FALSE;
	
	// see if its a 'tdl://'
	int nFind = sUrl.Find(TDL_PROTOCOL);
	
	if (nFind >= 0)
	{
		OnTDCDoTaskLink(0, (LPARAM)(LPCTSTR)sUrl);
		return TRUE;
	}
	else if (bShellExecute)
	{
		// set the current working directory to that of the tasklist
		CString sTasklistFolder = FileMisc::GetFolderFromFilePath(m_sLastSavePath);
		return (CFileEdit::GotoFile(sUrl, sTasklistFolder) >= 32);
	}
	
	return FALSE;
}

void CToDoCtrl::ExpandTasks(TDC_EXPANDCOLLAPSE nWhat, BOOL bExpand)
{
	if (!CanExpandTasks(nWhat, bExpand))
		return;

	HOLD_REDRAW(*this, m_tree);
	
	switch (nWhat)
	{
	case TDCEC_ALL:
		m_tree.TCH().ExpandAll(bExpand);
		m_tree.RecalcGutter();
		break;

	case TDCEC_SELECTED:
		{
			POSITION pos = Selection().GetFirstItemPos();
			
			while (pos)
			{
				HTREEITEM hti = Selection().GetNextItem(pos);
				m_tree.TCH().ExpandItem(hti, bExpand, TRUE);
			}

			m_tree.RecalcGutter();
		}
		break;

	case TDCEC_DUE:
		{
			// build due date search
			SEARCHPARAMS params;
			params.aRules.Add(SEARCHPARAM(TDCA_DUEDATE, FO_ON_OR_BEFORE, CDateHelper::GetDate(DHD_TODAY)));
			params.bIgnoreDone = true;

			SearchAndExpand(params, bExpand);
		}
		break;

	case TDCEC_STARTED:
		{
			// build due date search
			SEARCHPARAMS params;
			params.aRules.Add(SEARCHPARAM(TDCA_STARTDATE, FO_ON_OR_BEFORE, CDateHelper::GetDate(DHD_TODAY)));
			params.bIgnoreDone = true;

			SearchAndExpand(params, bExpand);
		}
		break;
	}
}

void CToDoCtrl::SearchAndExpand(const SEARCHPARAMS& params, BOOL bExpand)
{
	// perform the search
	CResultArray aResults;
	int nNumRes = m_find.FindTasks(params, aResults);
	
	// do the expansion
	for (int nRes = 0; nRes < nNumRes; nRes++)
	{
		HTREEITEM hti = m_find.GetItem(aResults[nRes].dwTaskID);
		
		if (hti)
			m_tree.TCH().ExpandItem(hti, bExpand, TRUE);
	}
	
	if (nNumRes)
		m_tree.RecalcGutter();
}

BOOL CToDoCtrl::CanExpandTasks(TDC_EXPANDCOLLAPSE nWhat, BOOL bExpand) const
{
	if (m_tree.GetCount() == 0)
		return FALSE;
		
	switch (nWhat)
	{
	case TDCEC_ALL:
		break;

	case TDCEC_SELECTED:
		{
			int nFullyExpanded = Selection().IsSelectionExpanded(TRUE);
			
			if (nFullyExpanded == -1)	// selected items have no children
			{
				return FALSE; // can neither expand nor collapse
			}
			
			if (bExpand)
				return !nFullyExpanded;
			
			// else
			return Selection().IsSelectionExpanded(FALSE);
		}
		break;

	case TDCEC_DUE:
		break;

	case TDCEC_STARTED:
		break;
	}

	// all else
	return TRUE;
}

CString CToDoCtrl::GetClipboardID() const
{
	CString sClipID = CPreferences::KeyFromFile(GetFilePath(), FALSE);

	if (sClipID.IsEmpty())
		sClipID = _T("New_Tasklist");

	return sClipID;
}

LRESULT CToDoCtrl::OnTDCHasClipboard(WPARAM /*wParam*/, LPARAM lParam)
{
	BOOL bCheckID = (lParam != FALSE);
	return !IsClipboardEmpty(bCheckID);
}

LRESULT CToDoCtrl::OnTDCGetClipboard(WPARAM wParam, LPARAM lParam)
{
	if (OnTDCHasClipboard(wParam, lParam))
	{
		static CTaskFile tasks;

		if (CTaskClipboard::GetTasks(tasks, GetClipboardID()))
 			return (LRESULT)&tasks;
	}

	// else
	return NULL;
}

LRESULT CToDoCtrl::OnTDCDoTaskLink(WPARAM wParam, LPARAM lParam)
{
	return ShowTaskLink(wParam, (LPCTSTR)lParam);
}

UINT CToDoCtrl::AdjustTaskDates(DWORD dwTaskID, DWORD dwDependencyID, TDC_DATE nDate)
{
	UINT nAdjusted = ADJUSTED_NONE;
	TODOITEM* pTDI = m_data.GetTask(dwTaskID);
	ASSERT (pTDI);
	
	// if we're already completed then no adjustment is necessary
	if (pTDI && !pTDI->IsDone())
	{
		const TODOITEM* pTDIDepends = m_data.GetTask(dwDependencyID);
		ASSERT (pTDIDepends);
	
		if (pTDIDepends)
		{
			COleDateTime dtNewStart;
				
			if (nDate == TDCD_DONE && pTDIDepends->IsDone())
			{
				// start dates match the preceding task's end date 
				dtNewStart = pTDIDepends->dateDone.m_dt + 1;
			}
			else if (nDate == TDCD_DUE && !pTDIDepends->IsDone() && pTDIDepends->HasDue())
			{		
				// or it's due date if it's not yet finished
				dtNewStart = pTDIDepends->dateDue.m_dt + 1;
			}

			if (dtNewStart != pTDI->dateStart)
			{
				// bump the due date too if present
				if (pTDI->HasDue() && pTDI->HasStart())
				{
					double dLength = max(0, pTDI->dateDue - pTDI->dateStart);
					pTDI->dateDue.m_dt = dtNewStart.m_dt + dLength;
					nAdjusted |= ADJUSTED_DUE;
				}
				
				// always set the start date
				pTDI->dateStart = dtNewStart;
				nAdjusted |= ADJUSTED_START;
			}
		}
	}

	return nAdjusted;
}

void CToDoCtrl::FixupTaskDependencyDates(DWORD dwTaskID, TDC_DATE nDate)
{
	if (!HasStyle(TDCS_AUTOADJUSTDEPENDENCYDATES))
		return;

	// check for circular dependency before continuing
	if (m_data.TaskHasCircularDependencies(dwTaskID))
		return;
	
	// who is dependent on us -> GetTaskDependents
	CDWordArray aDependents;

	if (m_data.GetTaskDependents(dwTaskID, aDependents))
	{
		for (int nLink = 0; nLink < aDependents.GetSize(); nLink++)
		{
			DWORD dwIDDependent = aDependents[nLink];
			UINT nAdjusted = AdjustTaskDates(dwIDDependent, dwTaskID, nDate);
			
			if (Misc::HasFlag(nAdjusted, ADJUSTED_DUE))
			{
				// then process this task's dependents
				FixupTaskDependencyDates(dwIDDependent, TDCD_DUE);
			}
		}
	}
}

BOOL CToDoCtrl::ShowTaskLink(DWORD dwTaskID, const CString& sLink)
{
	CString sFile;
	ParseTaskLink(sLink, dwTaskID, sFile);

	// if there's a file attached then pass to parent because
	// we can't handle it.
	if (!sFile.IsEmpty())
	{
		return GetParent()->SendMessage(WM_TDCM_TASKLINK, dwTaskID, (LPARAM)(LPCTSTR)sFile);
	}
	else if (dwTaskID)
	{
		if (SelectTask(dwTaskID))
		{
			SetFocusToTasks();
			return TRUE;
		}
		else
			MessageBoxEx(this, CEnString(IDS_TDC_TASKIDNOTFOUND, dwTaskID), IDS_TDC_TASKIDNOTFOUND_TITLE);
	}
	else
		MessageBoxEx(this, IDS_TDC_ZEROINVALIDTASKID, IDS_TDC_ZEROINVALIDTASKID_TITLE);

	return FALSE;
}

void CToDoCtrl::ParseTaskLink(const CString& sLink, DWORD& dwTaskID, CString& sFile)
{
	CString sCleaned(sLink);

	// strip off protocol (if not done)
	int nProtocol = sCleaned.Find(TDL_PROTOCOL);

	if (nProtocol != -1)
		sCleaned = sCleaned.Mid(nProtocol + lstrlen(TDL_PROTOCOL));

	// cleanup
	sCleaned.Replace(_T("%20"), _T(" "));
	sCleaned.Replace(_T("/"), _T("\\"));

	// parse the url
	TODOITEM::ParseTaskLink(sCleaned, dwTaskID, sFile);
}

BOOL CToDoCtrl::SelectionHasIncompleteDependencies(CString& sIncomplete) const
{
	POSITION pos = Selection().GetFirstItemPos();

	// traverse the selected items looking for the first one
	// who has an incomplete dependent.
	while (pos)
	{
		HTREEITEM hti = Selection().GetNextItem(pos);

		if (TaskHasIncompleteDependencies(hti, sIncomplete))
			return TRUE;
	}

	return FALSE;
}

int CToDoCtrl::GetSelectionRecurringTaskCount() const
{
	int nRecurring = 0;
	POSITION pos = Selection().GetFirstItemPos();

	// traverse the selected items looking for the first one
	// who has an incomplete dependent.
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		
		if (m_data.IsTaskRecurring(dwTaskID))
			nRecurring++;
	}

	return nRecurring;
}

BOOL CToDoCtrl::SelectionHasDates(TDC_DATE nDate, BOOL bAll) const
{
	POSITION pos = Selection().GetFirstItemPos();

	// traverse the selected items looking for the first one
	// who has a due date or the first that doesn't
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);
		double dDate = m_data.GetTaskDate(dwTaskID, nDate);

		if (!bAll && dDate > 0)
			return TRUE;
		
		else if (bAll && dDate == 0)
			return FALSE;
	}

	return bAll; // there were no dissenting tasks
}

BOOL CToDoCtrl::SelectedTasksHaveIcons() const
{
	POSITION pos = Selection().GetFirstItemPos();

	// traverse the selected items looking for the first one
	// who has an icon
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		if (!m_data.GetTaskIcon(dwTaskID).IsEmpty())
			return TRUE;
	}

	return FALSE; // there were no tasks with icons
}

BOOL CToDoCtrl::SelectedTasksAreAllDone() const
{
	POSITION pos = Selection().GetFirstItemPos();

	// traverse the selected items looking for the first one
	// who is incomplete
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		if (!m_data.IsTaskDone(dwTaskID))
			return FALSE;
	}

	return TRUE; // all tasks were completed
}

int CToDoCtrl::SelectionHasCircularDependencies(CDWordArray& aTaskIDs) const
{
	POSITION pos = Selection().GetFirstItemPos();
	aTaskIDs.RemoveAll();

	// traverse the selected items looking for the first one
	// who has an incomplete dependent.
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		if (m_data.TaskHasCircularDependencies(dwTaskID))
		{
			aTaskIDs.Add(dwTaskID);
			return TRUE;
		}
	}

	return aTaskIDs.GetSize();
}

BOOL CToDoCtrl::TaskHasIncompleteDependencies(HTREEITEM hti, CString& sIncomplete) const
{
	CStringArray aDepends;
	int nNumDepends = m_data.GetTaskDependencies(GetTaskID(hti), aDepends);

	for (int nDepends = 0; nDepends < nNumDepends; nDepends++)
	{
		CString sFile;
		DWORD dwTaskID;
		
		ParseTaskLink(aDepends[nDepends], dwTaskID, sFile);
		
		// see if dependent is one of 'our' tasks
		if (dwTaskID && sFile.IsEmpty())
		{
			if (!IsTaskDone(dwTaskID))
			{
				sIncomplete = aDepends[nDepends];
				return TRUE;
			}
		}
		// else pass to parent if we can't handle
		else if (!sFile.IsEmpty())
		{
			BOOL bDependentIsDone = GetParent()->SendMessage(WM_TDCM_TASKISDONE, 
				dwTaskID, (LPARAM)(LPCTSTR)sFile);
			
			if (!bDependentIsDone)
			{
				sIncomplete = aDepends[nDepends];
				return TRUE;
			}
		}
	}

	// check this tasks subtasks
	HTREEITEM htiChild = m_tree.GetChildItem(hti);

	while (htiChild)
	{
		if (TaskHasIncompleteDependencies(htiChild, sIncomplete))
			return TRUE;

		htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
	}

	return FALSE;
}

void CToDoCtrl::OnSelChangeCommentsType()
{
	HandleUnsavedComments();

	BOOL bMixedSelection = (m_cfComments.IsEmpty());

	// recreate comments control if necessary
	m_cbCommentsType.GetSelectedFormat(m_cfComments);
	CreateContentControl(TRUE);

	IMPLEMENT_UNDOEDIT();

	// modify the comments type of the selected tasks
	// without clearing any custom comments
	BOOL bChange = FALSE;
	POSITION pos = Selection().GetFirstItemPos();
	DWORD dwModTaskID = 0;
		
	while (pos)
	{
		DWORD dwTaskID = Selection().GetNextItemData(pos);

		if (m_data.SetTaskCommentsType(dwTaskID, m_cfComments) == SET_CHANGE)
		{
			bChange = TRUE;
			dwModTaskID = dwTaskID;
		}
	}

	if (bChange)
		SetModified(TRUE, TDCA_COMMENTS, dwModTaskID);

	// update comments control state if previously in a mixed state
	if (bMixedSelection)
		UpdateControls(GetSelectedItem());
}

void CToDoCtrl::OnSettingChange(UINT uFlags, LPCTSTR lpszSection)
{
    CRuntimeDlg::OnSettingChange(uFlags, lpszSection);

    // spin button visibility goes badly wrong after a settings change
    // this is my best current solution other than subclassing the
    // spin button. Simply calling ShowWindow(SW_HIDE) from here
    // does not work.
    PostMessage(WM_TDC_REFRESHPERCENTSPINVISIBILITY);
}

LRESULT CToDoCtrl::OnRefreshPercentSpinVisibility(WPARAM /*wp*/, LPARAM /*lp*/)
{
    if (!m_ePercentDone.IsWindowVisible())
        m_spinPercent.ShowWindow(SW_HIDE);

    return 0L;
}

LRESULT CToDoCtrl::OnChangeColour(WPARAM /*wp*/, LPARAM /*lp*/)
{
	UpdateTask(TDCA_COLOR);

	return 0L;
}

BOOL CToDoCtrl::CanUndoLastAction(BOOL bUndo) const 
{ 
	// handle comments field
	if (m_ctrlComments.HasFocus())
		return m_bCommentsChange;

	if (bUndo)
	{
		// handle simple edit fields
		CWnd* pFocus = GetFocus();

		if (pFocus)
		{
			CEdit* pEdit = NULL;
			UINT nFocusID = pFocus ? pFocus->GetDlgCtrlID() : 0;

			if (IsTaskLabelEditing() || nFocusID == IDC_PROJECTNAME ||
				m_cbCategory.IsChild(pFocus) || m_cbAllocTo.IsChild(pFocus) || m_cbTags.IsChild(pFocus))
			{
				pEdit = (CEdit*)pFocus;
			}

			if (pEdit)
				return pEdit->CanUndo();
		}
	}

	return m_data.CanUndoLastAction(bUndo); 
}

BOOL CToDoCtrl::UndoLastAction(BOOL bUndo)
{
	// handle comments field
	if (m_ctrlComments.HasFocus())
		return bUndo ? m_ctrlComments.Undo() : m_ctrlComments.Redo();

	// else handle if we're editing a tree label or if the focus is on the 
	// project name, category or alloc to fields
	CWnd* pFocus = GetFocus();

	if (pFocus)
	{
		UINT nFocusID = pFocus ? pFocus->GetDlgCtrlID() : 0;

		if (IsTaskLabelEditing() || nFocusID == IDC_PROJECTNAME)
		{
			CEdit* pEdit = (CEdit*)pFocus;
			
			// try to undo and prevent further processing 
			pEdit->Undo();
			return FALSE; 
		}
		else if (m_cbCategory.IsChild(pFocus) || 
				m_cbAllocTo.IsChild(pFocus) || 
				m_cbTags.IsChild(pFocus))
		{
			CEdit* pEdit = (CEdit*)pFocus;

			// only process and prevent further processing if the 
			// edit has something to undo
			if (pEdit->CanUndo())
			{
				pEdit->Undo();
				return FALSE; // combo edit takes precedence
			}
		}
	}

	HOLD_REDRAW(*this, m_tree);
	
	if (m_data.CanUndoLastAction(bUndo))
	{
		// fix up selection first in case we are about to delete the selected item
		Selection().RemoveAll(FALSE, FALSE);

		// get the list of the task IDs that will be undone/redone
		CDWordArray aTaskIDs;
		TDCUNDOACTIONTYPE nUndoType = m_data.GetLastUndoActionType(bUndo);

		// but not if the result is that the items in question were deleted
		if ((nUndoType == TDCUAT_DELETE && !bUndo) || (nUndoType == TDCUAT_ADD && bUndo))
		{
			// don't get IDs
		}
		else
			m_data.GetLastUndoActionTaskIDs(bUndo, aTaskIDs);

		CArrayUndoElements aElms;

		if (m_data.UndoLastAction(bUndo, aElms))
		{
			VERIFY(UndoLastActionItems(aElms));

			// fixup bold/checkstates
			RefreshItemStates();

			// restore selection
			if (aTaskIDs.GetSize())
				m_selection.SelectTasks(aTaskIDs);
			else
				Selection().PrevSelection(FALSE);

			// update current selection
			UpdateControls();

 			SetModified(TRUE, TDCA_UNDO, 0);

			return TRUE;
		}
	}

	return FALSE;
}

BOOL CToDoCtrl::UndoLastActionItems(const CArrayUndoElements& aElms)
{
	// restore elements
	int nNumElm = aElms.GetSize();
	
	// now undo
	for (int nElm = 0; nElm < nNumElm; nElm++)
	{
		TDCUNDOELEMENT elm = aElms[nElm];
		
		if (elm.nOp == TDCUEO_EDIT)
		{
			ASSERT(0); // we shouldn't ever have these
		}
		else if (elm.nOp == TDCUEO_DELETE)
		{
			// find tree item and delete it
			// note: DeleteTask on the Parent will already have disposed of the children
			// so we can expect hti to be NULL on occasion. ie don't ASSERT it
			HTREEITEM hti = m_find.GetItem(elm.dwTaskID);
			
			if (hti)
				m_tree.DeleteItem(hti);
		}
		else if (elm.nOp == TDCUEO_ADD)
		{
			// find parent item and restore task
			HTREEITEM htiParent = m_find.GetItem(elm.dwParentID);
			HTREEITEM htiPrevSibling = m_find.GetItem(elm.dwPrevSiblingID);
			
	//		ASSERT((elm.dwParentID && htiParent) || (!elm.dwParentID && !htiParent));

			if ((elm.dwParentID && htiParent) || (!elm.dwParentID && !htiParent))
			{
				HTREEITEM htiNew = m_tree.TCH().InsertItem(LPSTR_TEXTCALLBACK, 
															I_IMAGECALLBACK, 
															I_IMAGECALLBACK, 
															elm.dwTaskID, // lParam
															htiParent, 
															htiPrevSibling ? htiPrevSibling : TVI_FIRST,
															FALSE,
															FALSE);
				
				ASSERT(htiNew);
				
				if (!htiNew)
					return FALSE;
			}
		}
		else if (elm.nOp == TDCUEO_MOVE)
		{
			// move the task back to it original location
			HTREEITEM hti = m_find.GetItem(elm.dwTaskID); // current tree item
			HTREEITEM htiDestParent = m_find.GetItem(elm.dwParentID); // original owner
			HTREEITEM htiDestPrevSibling = m_find.GetItem(elm.dwPrevSiblingID); // original previous sibling

	//		ASSERT((elm.dwParentID && htiDestParent) || (!elm.dwParentID && !htiDestParent));

			if ((elm.dwParentID && htiDestParent) || (!elm.dwParentID && !htiDestParent))
			{
				DDWHERE nWhere = DD_BELOW; // most likely
				HTREEITEM htiTarget = htiDestPrevSibling;
				
				// if elm.dwPrevSiblingID is NULL then the task was the first 
				// child of elm.dwParentID
				if (!htiTarget)
				{
					// insert as top child
					if (m_tree.ItemHasChildren(htiDestParent))
					{
						htiTarget = m_tree.GetChildItem(htiDestParent);
						nWhere = DD_ABOVE;
					}
					else
					{
						htiTarget = htiDestParent;
						nWhere = DD_ON;
					}
				}
				
				// undo the move
				if (!CTreeDragDropHelper::MoveTree(m_tree, htiTarget, hti, nWhere, DD_USESTEXTCALLBACK/* | DD_USESIMAGECALLBACK*/))
				{
					ASSERT(0);
					return FALSE;
				}
			}
		}
		else
			return FALSE;
	}
	
	return TRUE;
}

void CToDoCtrl::Resort(BOOL bAllowToggle) 
{ 
	if (IsMultiSorting())
	{
		TDSORTCOLUMNS sort;

		GetSortBy(sort);
		MultiSort(sort);
	}
	else
		Sort(GetSortBy(), bAllowToggle); 
}

BOOL CToDoCtrl::IsMultiSorting() const
{
	return m_bMultiSort;
}

void CToDoCtrl::Sort(TDC_COLUMN nBy, BOOL bAllowToggle)
{
	// special case
	if (nBy == TDCC_NONE && m_sort.nBy1 == TDCC_NONE)
		return; // nothing to do

	if (nBy != TDCC_NONE)
	{
		// first time?
		if (m_sort.bAscending1 == -1 || nBy != m_sort.nBy1)
		{
			TDCCOLUMN* pTDCC = GetColumn(nBy);

			if (pTDCC)
				m_sort.bAscending1 = pTDCC->bSortAscending;
			
			else if (CTDCCustomAttributeHelper::IsCustomColumn(nBy))
				m_sort.bAscending1 = (m_tree.GetGutterColumnSort(nBy) != NCGSORT_DOWN);
		}
		// if there's been a mod since last sorting then its reasonable to assume
		// that the user is not toggling direction but wants to simply resort
		// in the same direction, otherwise we toggle the direction.
		else if (!m_bModSinceLastSort && bAllowToggle)
			m_sort.bAscending1 = !m_sort.bAscending1;
		
		// update the column header
		m_tree.SetGutterColumnSort(nBy, m_sort.bAscending1 ? NCGSORT_UP : NCGSORT_DOWN);
	}
	else
	{
		m_tree.SetGutterColumnSort(0, NCGSORT_NONE);
		m_sort.bAscending1 = TRUE;
	}
	
	// note: without this check new tasklists with a single
	// task demonstrate a display bug where the task name 
	// does not appear. and we don't want to get rid of the
	// CHoldRedraw (the culprit) because it speeds up sorting
	if (m_data.GetTaskCount() > 1)
	{
		HOLD_REDRAW(NULL, m_tree);
		SortTree(nBy, m_sort.bAscending1);
	}
	
	m_bMultiSort = FALSE;
	m_sort.nBy1 = nBy;
	m_sort.nBy2 = TDCC_NONE;
	m_sort.nBy3 = TDCC_NONE;
	m_bModSinceLastSort = FALSE;

	// update registry
	SaveSortState(CPreferences());
}

void CToDoCtrl::GetSortBy(TDSORTCOLUMNS& sort) const
{
	sort = m_sort;
}

void CToDoCtrl::MultiSort(const TDSORTCOLUMNS& sort)
{
	ASSERT (sort.nBy1 != TDCC_NONE);

	if (sort.nBy1 == TDCC_NONE)
		return;

	// note: without this check new tasklists with a single
	// task demonstrate a display bug where the task name 
	// does not appear. and we don't want to get rid of the
	// CHoldRedraw (the culprit) because it speeds up sorting
	if (m_data.GetTaskCount() > 1)
	{
		HOLD_REDRAW(NULL, m_tree);
		MultiSortTree(sort);
	}

	// clear sort direction on header
	m_tree.SetGutterColumnSort(0, NCGSORT_NONE);
	
	m_sort = sort;
	m_bMultiSort = TRUE;
	m_bModSinceLastSort = FALSE;

	// update registry
	SaveSortState(CPreferences());
}

void CToDoCtrl::SortTree(TDC_COLUMN nBy, BOOL bAscending, HTREEITEM htiRoot, BOOL bSortChildren)
{
	TDSORTPARAMS ss;

	ss.sort.nBy1 = nBy;
	ss.sort.nBy2 = TDCC_NONE;
	ss.sort.nBy3 = TDCC_NONE;
	ss.sort.bAscending1 = bAscending;
	ss.sort.bAscending2 = TRUE;
	ss.sort.bAscending3 = TRUE;
	ss.pData = &m_data;
	ss.pAttributeDefs = &m_aCustomAttribDefs;
	ss.bSortChildren = bSortChildren;
	// we only sort the due today tasks high if the user has assigned them a colour
	ss.bSortDueTodayHigh = (m_crDueToday != NOCOLOR);
	ss.dwTimeTrackID = m_dwTimeTrackTaskID;

	SortTreeItem(htiRoot, ss);
}

void CToDoCtrl::MultiSortTree(const TDSORTCOLUMNS& sort, HTREEITEM htiRoot, BOOL bSortChildren)
{
	TDSORTPARAMS ss;
	ss.sort = sort;
	ss.pData = &m_data;
	ss.pAttributeDefs = &m_aCustomAttribDefs;
	ss.bSortChildren = bSortChildren;
	// we only sort the due today tasks high if the user has assigned them a colour
	ss.bSortDueTodayHigh = (m_crDueToday != NOCOLOR);
	ss.dwTimeTrackID = 0;
	
	SortTreeItem(htiRoot, ss, TRUE);
}

void CToDoCtrl::SortTreeItem(HTREEITEM hti, const TDSORTPARAMS& ss, BOOL bMulti)
{
	// don't sort if not expanded and not the root item (hti == NULL)
	if (hti && hti != TVI_ROOT && HasStyle(TDCS_SORTVISIBLETASKSONLY))
	{
		if (!(m_tree.GetItemState(hti, TVIS_EXPANDED) & TVIS_EXPANDED))
			return;
	}
	
	TVSORTCB tvs = { hti, bMulti ? CToDoCtrl::SortFuncMulti : CToDoCtrl::SortFunc, (LPARAM)&ss };
	
	// sort this items children first
	m_tree.SortChildrenCB(&tvs);
	
	// then its children's children
	if (ss.bSortChildren)
	{
		HTREEITEM htiChild = m_tree.GetChildItem(hti);
		
		while (htiChild)
		{
			SortTreeItem(htiChild, ss, bMulti);
			htiChild = m_tree.GetNextItem(htiChild, TVGN_NEXT);
		}
	}
}

int CALLBACK CToDoCtrl::SortFuncMulti(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
	TDSORTPARAMS* pSS = (TDSORTPARAMS*)lParamSort;
	ASSERT (pSS->sort.nBy1 != TDCC_NONE);

	int nCompare = SortTasks(lParam1, lParam2, 
							pSS->sort.nBy1, 
							pSS->sort.bAscending1, 
							pSS->bSortDueTodayHigh, 
							pSS->dwTimeTrackID, 
							pSS->pData,
							pSS->pAttributeDefs);

	if (nCompare == 0 && pSS->sort.nBy2 != TDCC_NONE)
	{
		nCompare = SortTasks(lParam1, lParam2, 
							pSS->sort.nBy2, 
							pSS->sort.bAscending2, 
							pSS->bSortDueTodayHigh, 
							pSS->dwTimeTrackID, 
							pSS->pData,
							pSS->pAttributeDefs);

		if (nCompare == 0 && pSS->sort.nBy3 != TDCC_NONE)
		{
			nCompare = SortTasks(lParam1, lParam2, 
								pSS->sort.nBy3, 
								pSS->sort.bAscending3, 
								pSS->bSortDueTodayHigh, 
								pSS->dwTimeTrackID, 
								pSS->pData,
								pSS->pAttributeDefs);
		}
	}

	return nCompare;
}

int CALLBACK CToDoCtrl::SortFunc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
	TDSORTPARAMS* pSS = (TDSORTPARAMS*)lParamSort;
	
	return SortTasks(lParam1, lParam2, 
					pSS->sort.nBy1, 
					pSS->sort.bAscending1, 
					pSS->bSortDueTodayHigh, 
					pSS->dwTimeTrackID, 
					pSS->pData,
					pSS->pAttributeDefs);
}

int CToDoCtrl::SortTasks(LPARAM lParam1, LPARAM lParam2, TDC_COLUMN nSortBy, BOOL bAscending, 
						 BOOL bSortDueTodayHigh, DWORD dwTimeTrackedID, const CToDoCtrlData* pData,
						 const CTDCCustomAttribDefinitionArray* pAttibuteDefs)
{
	if (nSortBy == TDCC_TRACKTIME)
	{
		int nCompare = ((DWORD)lParam1 == dwTimeTrackedID) ? -1 : ((DWORD)lParam2 == dwTimeTrackedID) ? 1 : 0;
		return bAscending ? nCompare : -nCompare;
	}
	// handle custom attribute
	else if (nSortBy >= TDCC_CUSTOMCOLUMN_FIRST && nSortBy <= TDCC_CUSTOMCOLUMN_LAST)
	{
		CString sAttribID = CTDCCustomAttributeHelper::GetAttributeTypeID(nSortBy, *pAttibuteDefs);
		return pData->CompareTasks(lParam1, lParam2, sAttribID, bAscending);
	}

	// else default attribute
	return pData->CompareTasks(lParam1, lParam2, nSortBy, bAscending, bSortDueTodayHigh);
}

HBRUSH CToDoCtrl::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor) 
{
	HBRUSH hbr = CRuntimeDlg::OnCtlColor(pDC, pWnd, nCtlColor);

	if (CThemed::IsThemeActive() && nCtlColor == CTLCOLOR_STATIC && pWnd != &m_eRecurrence)
	{
		pDC->SetTextColor(m_theme.crAppText);
		pDC->SetBkMode(TRANSPARENT);
		hbr = m_brUIBack;
	}
	
	return hbr;
}

void CToDoCtrl::RedrawReminders() const
{ 
	if (IsColumnShowing(TDCC_REMINDER))
	{
		CTDCTreeView* pTree = const_cast<CTDCTreeView*>(&m_tree);
		pTree->RedrawGutter(); 
	}
}

TDC_ATTRIBUTE CToDoCtrl::GetFocusedControlAttribute() const
{
	HWND hFocus = ::GetFocus();
	UINT nCtrlID = ::GetDlgCtrlID(hFocus);
	TDC_COLUMN nColID = MapCtrlIDToColumn(nCtrlID);

	// handle edit controls of combos
	if (nColID == TDCC_NONE && CWinClasses::IsEditControl(hFocus))
	{
		hFocus = ::GetParent(hFocus);

		if (CWinClasses::IsComboBox(hFocus))
		{
			nCtrlID = ::GetDlgCtrlID(hFocus);
			nColID = MapCtrlIDToColumn(nCtrlID);
		}
	}

	TDC_ATTRIBUTE nAttrib = MapColumnToAttribute(nColID);

	if (nAttrib == TDCA_NONE)
		nAttrib = CTDCCustomAttributeHelper::GetAttributeID(nColID, m_aCustomAttribDefs);

	return nAttrib;
}

BOOL CToDoCtrl::CanClearSelectedTaskFocusedAttribute() const
{
	if (IsReadOnly())
		return FALSE;

	TDC_ATTRIBUTE nAttrib = GetFocusedControlAttribute();

	return CanClearSelectedTaskAttribute(nAttrib);
}

BOOL CToDoCtrl::ClearSelectedTaskFocusedAttribute()
{
	if (!CanClearSelectedTaskFocusedAttribute())
		return FALSE;

	TDC_ATTRIBUTE nAttrib = GetFocusedControlAttribute();

	return ClearSelectedTaskAttribute(nAttrib);
}

BOOL CToDoCtrl::CanClearSelectedTaskAttribute(TDC_ATTRIBUTE nAttrib) const
{
	if (IsReadOnly())
		return FALSE;

	return ((nAttrib >= TDCA_FIRSTATTRIBUTE && nAttrib < TDCA_ATTRIBUTECOUNT &&
			nAttrib != TDCA_TASKNAME && nAttrib != TDCA_PROJNAME) ||
			CTDCCustomAttributeHelper::IsCustomAttribute(nAttrib));
}

BOOL CToDoCtrl::ClearSelectedTaskAttribute(TDC_ATTRIBUTE nAttrib)
{
	switch (nAttrib)
	{
	case TDCA_DONEDATE:		return SetSelectedTaskDate(TDCD_DONE, 0.0);
	case TDCA_DUEDATE:		return SetSelectedTaskDate(TDCD_DUE, 0.0);
	case TDCA_STARTDATE:	return SetSelectedTaskDate(TDCD_START, 0.0);
		
	case TDCA_PRIORITY:		return SetSelectedTaskPriority(FM_NOPRIORITY);
	case TDCA_RISK:			return SetSelectedTaskRisk(FM_NORISK);
		
	case TDCA_ALLOCTO:		return SetSelectedTaskAllocTo(CStringArray());
	case TDCA_CATEGORY:		return SetSelectedTaskCategories(CStringArray());
	case TDCA_TAGS:			return SetSelectedTaskTags(CStringArray());
	case TDCA_DEPENDENCY:	return SetSelectedTaskDependencies(CStringArray());
		
	case TDCA_ALLOCBY:		return SetSelectedTaskAllocBy(_T(""));
	case TDCA_STATUS:		return SetSelectedTaskStatus(_T(""));
	case TDCA_FILEREF:		return SetSelectedTaskFileRef(_T(""));
	case TDCA_VERSION:		return SetSelectedTaskVersion(_T(""));
	case TDCA_EXTERNALID:	return SetSelectedTaskExtID(_T(""));
	case TDCA_ICON:			return SetSelectedTaskIcon(_T(""));
		
	case TDCA_PERCENT:		return SetSelectedTaskPercentDone(0);
	case TDCA_FLAG:			return SetSelectedTaskFlag(FALSE);
	case TDCA_COST:			return SetSelectedTaskCost(0.0);
	case TDCA_COLOR:		return SetSelectedTaskColor(0);
	case TDCA_RECURRENCE:	return SetSelectedTaskRecurrence(TDIRECURRENCE());
		
	case TDCA_TIMEEST:		
		{
			TCHAR nUnits;
			GetSelectedTaskTimeEstimate(nUnits); // preserve existing units
			return SetSelectedTaskTimeEstimate(0.0, nUnits);
		}

	case TDCA_TIMESPENT:
		{
			TCHAR nUnits;
			GetSelectedTaskTimeSpent(nUnits); // preserve existing units
			return SetSelectedTaskTimeSpent(0.0, nUnits);
		}
	}

	// fall thru to custom attributes
	CString sCustomAttribID = CTDCCustomAttributeHelper::GetAttributeTypeID(nAttrib, m_aCustomAttribDefs);

	return SetSelectedTaskCustomAttributeData(sCustomAttribID, _T(""));
}
