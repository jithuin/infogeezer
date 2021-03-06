// OutlookImportDlg.cpp : implementation file
//

#include "stdafx.h"
#include "OutlookImpExp.h"
#include "OutlookImportDlg.h"

#include "..\shared\IPreferences.h"
#include "..\shared\ITaskList.h"
#include "..\shared\misc.h"
#include "..\shared\localizer.h"

#include "..\3rdparty\msoutl.h"

#include <afxtempl.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

const int olFolderDeletedItems	= 3;
const int olFolderOutbox		= 4;
const int olFolderSentMail		= 5;
const int olFolderInbox			= 6;
const int olFolderCalendar		= 9;
const int olFolderContacts		= 10;
const int olFolderJournal		= 11;
const int olFolderNotes			= 12;
const int olFolderTasks			= 13;

using namespace OutlookAPI;

LPCTSTR PATHDELIM = _T(" \\ ");

/////////////////////////////////////////////////////////////////////////////
// COutlookImportDlg dialog


COutlookImportDlg::COutlookImportDlg(CWnd* pParent /*=NULL*/)
	: CDialog(COutlookImportDlg::IDD, pParent), m_pDestTaskFile(NULL), m_pOutlook(NULL)
{
	//{{AFX_DATA_INIT(COutlookImportDlg)
	//}}AFX_DATA_INIT
}

void COutlookImportDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(COutlookImportDlg)
	DDX_Control(pDX, IDC_TASKLIST, m_lbTasks);
	DDX_Check(pDX, IDC_REMOVEOUTLOOKTASKS, m_bRemoveOutlookTasks);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(COutlookImportDlg, CDialog)
	//{{AFX_MSG_MAP(COutlookImportDlg)
	ON_WM_DESTROY()
	ON_BN_CLICKED(IDC_CHOOSEFOLDER, OnChoosefolder)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// COutlookImportDlg message handlers

BOOL COutlookImportDlg::ImportTasks(ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey)
{
	m_pDestTaskFile = GetITLInterface<ITaskList7>(pDestTaskFile, IID_TASKLIST7);
	ASSERT(m_pDestTaskFile);

	if (!m_pDestTaskFile)
		return FALSE;

	CString sKey(szKey);
	sKey += _T("\\Outlook");

	m_bRemoveOutlookTasks = pPrefs->GetProfileInt(sKey, _T("RemoveOutlookTasks"), FALSE);

	if (DoModal() == IDOK)
	{
		pPrefs->WriteProfileInt(sKey, _T("RemoveOutlookTasks"), m_bRemoveOutlookTasks);
		return TRUE;
	}

	// else
	return FALSE;
}

BOOL COutlookImportDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	CLocalizer::EnableTranslation(m_lbTasks, FALSE);
	
	ASSERT(m_pOutlook == NULL);
	m_pOutlook = new _Application;

	if (m_pOutlook->CreateDispatch(_T("Outlook.Application")))
	{
		_NameSpace nmspc(m_pOutlook->GetNamespace(_T("MAPI")));
		nmspc.m_lpDispatch->AddRef(); // to keep it alive

		m_pFolder = new MAPIFolder(nmspc.GetDefaultFolder(olFolderTasks));
		m_pFolder->m_lpDispatch->AddRef(); // to keep it alive

		AddFolderItemsToList(m_pFolder);
	}
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void COutlookImportDlg::AddFolderItemsToList(MAPIFolder* pFolder, const CString& sPath)
{
	_Items items(pFolder->GetItems());
	items.m_lpDispatch->AddRef(); // to keep it alive

	int nItem, nCount = items.GetCount();

	for (nItem = 1; nItem <= nCount; nItem++)
	{
		LPDISPATCH lpd = items.Item(COleVariant((short)nItem));
		lpd->AddRef(); // to keep it alive
		_TaskItem task(lpd);

		CString sItem(task.GetSubject());

		if (!sPath.IsEmpty())
			sItem.Format(_T("%s%s%s"), sPath, PATHDELIM, task.GetSubject());

		int nIndex = m_lbTasks.AddString(sItem);

		m_lbTasks.SetItemData(nIndex, (DWORD)lpd);
		m_lbTasks.SetCheck(nIndex, TRUE);
	}

	// likewise for subfolders
	_Items folders(pFolder->GetFolders());
	nCount = folders.GetCount();

	for (nItem = 1; nItem <= nCount; nItem++)
	{
		MAPIFolder folder(folders.Item(COleVariant((short)nItem)));
		folder.m_lpDispatch->AddRef();
		CString sSubPath(folder.GetName());

		if (!sPath.IsEmpty())
			sSubPath.Format(_T("%s%s%s"), sPath, PATHDELIM, folder.GetName());

		AddFolderItemsToList(&folder, sSubPath);
	}
}

void COutlookImportDlg::OnOK()
{
	CDialog::OnOK();

	ASSERT(m_pOutlook && m_pDestTaskFile);

	// make sure nothing has changed
	_NameSpace nmspc(m_pOutlook->GetNamespace(_T("MAPI")));
	nmspc.m_lpDispatch->AddRef(); // to keep it alive

	MAPIFolder mapi(nmspc.GetDefaultFolder(olFolderTasks));
	mapi.m_lpDispatch->AddRef(); // to keep it alive

	_Items items(mapi.GetItems());
	items.m_lpDispatch->AddRef(); // to keep it alive

	int nTaskCount = items.GetCount();
	int nLBCount = m_lbTasks.GetCount();

	// iterate the listbox items looking for checked items
	for (int nItem = 0; nItem <= nLBCount; nItem++)
	{
		if (m_lbTasks.GetCheck(nItem))
		{
			// get the task index that this item points to
			LPDISPATCH lpd = (LPDISPATCH)m_lbTasks.GetItemData(nItem);
			_TaskItem task(lpd);

			// create a new task
			CString sTitle;
			m_lbTasks.GetText(nItem, sTitle); // gets the full path

			HTASKITEM hTask = m_pDestTaskFile->NewTask(sTitle);
			ASSERT(hTask);

			SetTaskAttributes(hTask, &task);

			// delete the item as we go if required
			if (m_bRemoveOutlookTasks)
				DeleteTaskFromFolder(&task, &mapi);
/*
			// split the full path by delimiters
			CStringArray aTasks;
			int nNumTask = Misc::Split(sTitle, aTasks, FALSE, PATHDELIM);

			HTASKITEM hParent = NULL;

			for (int nTask = 0; nTask < nNumTask; nTask++)
			{
				HTASKITEM hTask = m_pDestTaskFile->NewTask(aTasks[nTask], hParent);
				ASSERT(hTask);

				// only get attributes for leaf items
				if (nTask == nNumTask - 1)
				{
					SetTaskAttributes(hTask, &task);

					// delete the item as we go if required
					if (m_bRemoveOutlookTasks)
						DeleteTaskFromFolder(&task, &mapi);
				}

				hParent = hTask;
			}
*/
		}
	}
}

BOOL COutlookImportDlg::DeleteTaskFromFolder(_TaskItem* pTask, MAPIFolder* pFolder)
{
	// look through this folders tasks first
	_Items items(pFolder->GetItems());
	items.m_lpDispatch->AddRef(); // to keep it alive
	int nItem, nTaskCount = items.GetCount();

	for (nItem = 1; nItem <= nTaskCount; nItem++)
	{
		LPDISPATCH lpd = items.Item(COleVariant((short)nItem));
		_TaskItem taskTest(lpd);

		if (TaskPathsMatch(pTask, &taskTest))
		{
			items.Remove(nItem);
			return TRUE;
		}
	}

	// then for subfolders
	_Items folders(pFolder->GetFolders());
	int nCount = folders.GetCount();

	for (nItem = 1; nItem <= nCount; nItem++)
	{
		MAPIFolder folder(folders.Item(COleVariant((short)nItem)));

		if (DeleteTaskFromFolder(pTask, &folder))
			return TRUE;
	}

	return FALSE;
}

BOOL COutlookImportDlg::TaskPathsMatch(_TaskItem* pTask1, _TaskItem* pTask2)
{
	CString sPath1 = GetFullPath(pTask1);
	CString sPath2 = GetFullPath(pTask2);

	return (sPath1 == sPath2);
}

CString COutlookImportDlg::GetFullPath(_TaskItem* pTask)
{
	CString sPath(pTask->GetSubject()), sFolder;
	LPDISPATCH lpd = pTask->GetParent();

	do
	{
		try
		{
			MAPIFolder folder(lpd);
			sFolder = folder.GetName(); // will throw when we hit the highest level
			sPath = sFolder + PATHDELIM + sPath;

			lpd = folder.GetParent(); 
		}
		catch (...)
		{
			break;
		}
	}
	while (true);

	return sPath;
}

void COutlookImportDlg::SetTaskAttributes(HTASKITEM hTask, _TaskItem* pTask)
{
	// set it's attributes
	m_pDestTaskFile->SetTaskComments(hTask, pTask->GetBody());

	// can have multiple categories
	CStringArray aCats;
	Misc::Split(pTask->GetCategories(), aCats);

	for (int nCat = 0; nCat < aCats.GetSize(); nCat++)
		m_pDestTaskFile->AddTaskCategory(hTask, aCats[nCat]);
	
	if (pTask->GetComplete())
		m_pDestTaskFile->SetTaskDoneDate(hTask, ConvertDate(pTask->GetDateCompleted()));
	
	m_pDestTaskFile->SetTaskDueDate(hTask, ConvertDate(pTask->GetDueDate()));
	m_pDestTaskFile->SetTaskStartDate(hTask, ConvertDate(pTask->GetStartDate()));
	m_pDestTaskFile->SetTaskCreationDate(hTask, ConvertDate(pTask->GetCreationTime()));
	m_pDestTaskFile->SetTaskLastModified(hTask, ConvertDate(pTask->GetLastModificationTime()));
	m_pDestTaskFile->SetTaskAllocatedBy(hTask, pTask->GetDelegator());
	m_pDestTaskFile->SetTaskAllocatedTo(hTask, pTask->GetOwner());
	m_pDestTaskFile->SetTaskPriority(hTask, (unsigned char)(pTask->GetImportance() * 5));
	m_pDestTaskFile->SetTaskComments(hTask, pTask->GetBody());

	// save outlook ID unless removing from Outlook
	if (!m_bRemoveOutlookTasks)
	{
		CString sFileLink;
		sFileLink.Format(_T("outlook:%s"), pTask->GetEntryID());
		m_pDestTaskFile->SetTaskFileReferencePath(hTask, sFileLink);
	}

/*
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
	m_pDestTaskFile->SetTask(hTask, pTask->Get());
*/
}

void COutlookImportDlg::OnDestroy() 
{
	CDialog::OnDestroy();
	
	delete m_pOutlook;
	delete m_pFolder;

	m_pOutlook = NULL;
	m_pDestTaskFile = NULL;
	m_pFolder = NULL;
}

time_t COutlookImportDlg::ConvertDate(DATE date)
{
	if (date <= 0.0)
		return 0;

	SYSTEMTIME st;
	COleDateTime dt(date);

	dt.GetAsSystemTime(st);

	tm t = { st.wSecond, st.wMinute, st.wHour, st.wDay, st.wMonth - 1, st.wYear - 1900, 0 };
	return mktime(&t);
}

void COutlookImportDlg::OnChoosefolder() 
{
	_NameSpace nmspc(m_pOutlook->GetNamespace(_T("MAPI")));
	nmspc.m_lpDispatch->AddRef(); // to keep it alive

	LPDISPATCH pDisp = nmspc.PickFolder();

	if (pDisp)
	{
		delete m_pFolder;
		m_pFolder = new MAPIFolder(pDisp);

		m_lbTasks.ResetContent();
		AddFolderItemsToList(m_pFolder);
	}

/*
    if (pFolder==NULL)
      return;

    if (m_pFolder->GetDefaultItemType()!=olContactItem)
    {
      MessageBox("Select folder is not a Contact folder.",
                 "Outlook Contacts");
      return;
    }
*/

}
