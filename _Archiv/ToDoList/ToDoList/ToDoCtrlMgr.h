// CToDoCtrlMgr.h : header file
//

#if !defined(AFX_TODOCTRLMGR_H__13051D32_D372_4205_BA71_05FAC2159F1C__INCLUDED_)
#define AFX_TODOCTRLMGR_H__13051D32_D372_4205_BA71_05FAC2159F1C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "filteredtodoctrl.h"
#include "preferencesdlg.h"

#include "..\shared\filemisc.h"
#include "..\shared\driveinfo.h"
#include "..\shared\taskliststoragemgr.h"

/////////////////////////////////////////////////////////////////////////////
// CToDoCtrlMgr dialog
 
enum TDCM_PATHTYPE { TDCM_UNDEF = -1, TDCM_REMOVABLE, TDCM_FIXED, TDCM_REMOTE, TDCM_OTHER }; // drive types
enum TDCM_DUESTATUS { TDCM_NONE = -1, TDCM_PAST, TDCM_TODAY, TDCM_FUTURE }; 

class CPreferences;

class CToDoCtrlMgr
{
	// Construction
public:
	CToDoCtrlMgr(CTabCtrl& tabCtrl); 
	~CToDoCtrlMgr();

	inline int GetCount() const { return m_aToDoCtrls.GetSize(); }
	inline int GetSelToDoCtrl() const { return m_tabCtrl.GetCurSel(); }

	void SetPrefs(const CPreferencesDlg* pPrefs);

	void RefreshColumns(int nIndex, CTDCColumnIDArray& aColumns /*out*/);
	BOOL HasOwnColumns(int nIndex) const;
	void SetHasOwnColumns(int nIndex, BOOL bHas = TRUE);

	CFilteredToDoCtrl& GetToDoCtrl(int nIndex);
	const CFilteredToDoCtrl& GetToDoCtrl(int nIndex) const;

	int RemoveToDoCtrl(int nIndex, BOOL bDelete = FALSE); // returns new selection
	int AddToDoCtrl(CFilteredToDoCtrl* pCtrl, const TSM_TASKLISTINFO* pInfo = NULL, BOOL bLoaded = TRUE);
	BOOL IsPristine(int nIndex) const;
	BOOL IsLoaded(int nIndex) const;
	void SetLoaded(int nIndex, BOOL bLoaded = TRUE);

	BOOL HasTasks(int nIndex) const;
	BOOL AnyHasTasks() const;
	BOOL AnyIsModified() const;

	int FindToDoCtrl(const CFilteredToDoCtrl* pTDC) const;
	int FindToDoCtrl(LPCTSTR szFilePath) const;
	CString GetFilePath(int nIndex, BOOL bStrict = TRUE) const;
	void ClearFilePath(int nIndex);
	BOOL IsFilePathEmpty(int nIndex) const;
	TDCM_PATHTYPE GetFilePathType(int nIndex) const;
	TDCM_PATHTYPE RefreshPathType(int nIndex); 
	CString GetFriendlyProjectName(int nIndex) const;

	CString GetDisplayPath(int nIndex) const;
	
	BOOL UsesStorage(int nIndex) const;
	BOOL GetStorageDetails(int nIndex, TSM_TASKLISTINFO& info) const;
	BOOL SetStorageDetails(int nIndex, const TSM_TASKLISTINFO& info);
	BOOL ClearStorageDetails(int nIndex);

	BOOL RefreshLastModified(int nIndex); // true if changed
	time_t GetLastModified(int nIndex) const;
	void SetModifiedStatus(int nIndex, BOOL bMod);
	BOOL GetModifiedStatus(int nIndex) const;
	void RefreshModifiedStatus(int nIndex);

	int GetReadOnlyStatus(int nIndex) const;
	BOOL RefreshReadOnlyStatus(int nIndex); // true if changed
	void UpdateToDoCtrlReadOnlyUIState(int nIndex);
	void UpdateToDoCtrlReadOnlyUIState(CFilteredToDoCtrl& tdc);

	void SetDueItemStatus(int nIndex, TDCM_DUESTATUS nStatus);
	TDCM_DUESTATUS GetDueItemStatus(int nIndex) const;

	int GetLastCheckoutStatus(int nIndex) const;
	void SetLastCheckoutStatus(int nIndex, BOOL bStatus);
	int CanCheckOut(int nIndex) const;
	int CanCheckIn(int nIndex) const;
	int CanCheckInOut(int nIndex) const;

	void MoveToDoCtrl(int nIndex, int nNumPlaces);
	BOOL CanMoveToDoCtrl(int nIndex, int nNumPlaces) const;

	int SortToDoCtrlsByName();
	BOOL PathSupportsSourceControl(int nIndex) const;
	BOOL PathSupportsSourceControl(LPCTSTR szPath) const;
	BOOL IsSourceControlled(int nIndex) const;

	CString UpdateTabItemText(int nIndex);
	CString GetTabItemText(int nIndex) const;
	CString GetTabItemTooltip(int nIndex) const;

	BOOL ArchiveDoneTasks(int nIndex);
	BOOL ArchiveSelectedTasks(int nIndex);
	CString GetArchivePath(LPCTSTR szFilePath) const;
	CString GetArchivePath(int nIndex) const;

	void SetNeedsPreferenceUpdate(int nIndex, BOOL bNeed);
	BOOL GetNeedsPreferenceUpdate(int nIndex) const;
	void SetAllNeedPreferenceUpdate(BOOL bNeed, int nExcept = -1);

	void PreparePopupMenu(CMenu& menu, UINT nID1, int nMax = 20) const;
	COleDateTime GetMostRecentEdit() const;

	// Implementation
protected:
	struct TDCITEM
	{
		TDCITEM() 
		{ 
			pTDC = NULL; 
			bModified = FALSE; 
			bLastStatusReadOnly = -1; 
			tLastMod = 0; 
			bLastCheckoutSuccess = -1;
            nPathType = TDCM_UNDEF;
			nDueStatus = TDCM_NONE;
			bNeedPrefUpdate = TRUE;
			nCreated = -1;
			bLoaded = TRUE;
			bHasOwnColumns = FALSE;
		}
		
		TDCITEM(CFilteredToDoCtrl* pCtrl, int nIndex, BOOL loaded, const TSM_TASKLISTINFO* pInfo = NULL) 
		{ 
			pTDC = pCtrl; 
			nCreated = nIndex;
			bLoaded = loaded;
			
			bModified = FALSE; 
			bLastStatusReadOnly = -1;
			tLastMod = 0;
			bLastCheckoutSuccess = -1;
			nDueStatus = TDCM_NONE;
			bNeedPrefUpdate = TRUE;
			bHasOwnColumns = FALSE;

			if (pInfo && pInfo->HasInfo())
				storageinfo = *pInfo;
			else
			{
				CString sFilePath = pCtrl->GetFilePath();
				
				if (!sFilePath.IsEmpty())
				{
					bLastStatusReadOnly = (CDriveInfo::IsReadonlyPath(sFilePath) > 0);
					tLastMod = FileMisc::GetFileLastModified(sFilePath);
				}
			}
			
            RefreshPathType();
		}
		
		CFilteredToDoCtrl* pTDC;
		BOOL bModified;
		BOOL bLastStatusReadOnly;
		time_t tLastMod;
		BOOL bLastCheckoutSuccess;
        TDCM_PATHTYPE nPathType;
		TDCM_DUESTATUS nDueStatus;
		BOOL bNeedPrefUpdate;
		int nCreated; // creation index regardless of actual position
		BOOL bLoaded;
		BOOL bHasOwnColumns;

		TSM_TASKLISTINFO storageinfo;
		
		inline TDCM_PATHTYPE GetPathType() const { return nPathType; }
        
		BOOL UsesStorage() const { return !storageinfo.sStorageID.IsEmpty(); }

        void RefreshPathType() 
        { 
			// special case
  			if (UsesStorage())
  				nPathType = TDCM_OTHER;
  			else
			{
				LPCTSTR szFilePath = pTDC->GetFilePath();
				nPathType = TranslatePathType(CDriveInfo::GetPathType(szFilePath));
			}
        }

		CString GetFriendlyProjectName() const 
		{ 
 			if (UsesStorage() && pTDC->GetProjectName().IsEmpty())
 				return storageinfo.szDisplayName;

			// else
			return pTDC->GetFriendlyProjectName(nCreated); 
		}

		static TDCM_PATHTYPE TranslatePathType(int nDriveInfoType)
		{
            switch (nDriveInfoType)
            {
            case DRIVE_REMOTE:
                return TDCM_REMOTE;
				
            case DRIVE_REMOVABLE:
            case DRIVE_CDROM:
                return TDCM_REMOVABLE;
				
            case DRIVE_FIXED:
                return TDCM_FIXED;
			}

			// all else
			return TDCM_UNDEF;
		}
	};
	
	CArray<TDCITEM, TDCITEM&> m_aToDoCtrls;
	CTabCtrl& m_tabCtrl;
	const CPreferencesDlg* m_pPrefs;

protected:
	TDCITEM& GetTDCItem(int nIndex);
	const TDCITEM& GetTDCItem(int nIndex) const;

	// sort function
	static int SortProc(const void* v1, const void* v2);
	BOOL AreToDoCtrlsSorted() const;

	BOOL PathTypeSupportsSourceControl(TDCM_PATHTYPE nType) const;
	const CPreferencesDlg& Prefs() const;

	void RestoreColumns(TDCITEM* pTDCI, const CPreferences& prefs);
	void SaveColumns(const TDCITEM* pTDCI, CPreferences& prefs) const;

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TODOCTRLMGR_H__13051D32_D372_4205_BA71_05FAC2159F1C__INCLUDED_)
