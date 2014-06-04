// FtpTasklistStorage.h: interface for the CFtpTasklistStorage class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_FTPTASKLISTSTORAGE_H__14908CE5_AA9F_4AFC_B72E_3F2BDD0993F0__INCLUDED_)
#define AFX_FTPTASKLISTSTORAGE_H__14908CE5_AA9F_4AFC_B72E_3F2BDD0993F0__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "..\shared\ITaskListStorage.h"

class CFtpTasklistStorage : public ITasklistStorage  
{
public:
	CFtpTasklistStorage();
	virtual ~CFtpTasklistStorage();

	// interface implementation
    void Release() { delete this; }
	void SetLocalizer(ITransText* pTT);

	// caller must copy result only
	LPCTSTR GetMenuText() const { return _T("EasyFtp"); }
	LPCTSTR GetTypeID() const { return _T("14908CE5_AA9F_4AFC_B72E_3F2BDD0993F0"); }

	bool RetrieveTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pDestTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);
	bool StoreTasklist(ITS_TASKLISTINFO* pFInfo, ITaskList* pSrcTaskFile, IPreferences* pPrefs, LPCTSTR szKey, bool bSilent);
};

#endif // !defined(AFX_FTPTASKLISTSTORAGE_H__14908CE5_AA9F_4AFC_B72E_3F2BDD0993F0__INCLUDED_)
