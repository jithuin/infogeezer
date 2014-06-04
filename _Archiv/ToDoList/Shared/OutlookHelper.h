// OutlookHelper.h: interface for the COutlookHelper class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_OUTLOOKHELPER_H__C57DEEC1_2B2B_490A_9F2B_1A7B9127313F__INCLUDED_)
#define AFX_OUTLOOKHELPER_H__C57DEEC1_2B2B_490A_9F2B_1A7B9127313F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <afxtempl.h>

enum OUTLOOK_FIELDTYPE
{
	OA_FIRST = 0,
	OA_BCC = 0,
	OA_BILLINGINFORMATION,
	OA_BODY,
	OA_CATEGORIES,
	OA_CC,
	OA_COMPANIES,
	OA_CONVERSATIONTOPIC,
	OA_CREATIONTIME,
	OA_ENTRYID,
	OA_EXPIRYTIME,
	OA_FLAGREQUEST,
	OA_IMPORTANCE,
	OA_LASTMODIFICATIONTIME,
	OA_MESSAGECLASS,
	OA_MILEAGE,
	OA_PERMISSION,
	OA_RECEIVEDBYNAME,
	OA_RECEIVEDTIME,
	OA_REMINDERTIME,
	OA_REPLYRECIPIENTNAMES,
	OA_SENDEREMAILADDRESS,
	OA_SENDERNAME,
	OA_SENSITIVITY,
	OA_SENTON,
	OA_SENTONBEHALFOFNAME,
	OA_SUBJECT,
	OA_TASKCOMPLETEDDATE,
	OA_TASKDUEDATE,
	OA_TASKSTARTDATE,
//	OA_TASKSUBJECT,
	OA_TO,
	// insert here

	OA_LAST,
};

class COutlookItemDataMap : public CMap<OUTLOOK_FIELDTYPE, OUTLOOK_FIELDTYPE, CString, CString&>
{
public:
	COutlookItemDataMap();
	COutlookItemDataMap(const COutlookItemDataMap& map);

	CString GetFieldData(OUTLOOK_FIELDTYPE nField) const;
	BOOL HasFieldData(OUTLOOK_FIELDTYPE nField) const;
};

// predecs
namespace OutlookAPI
{
	class _Application;
	class Selection;
	class _MailItem;
}

class COutlookHelper  
{
public:
	COutlookHelper();
	virtual ~COutlookHelper();

	BOOL IsValid() const { return (s_pOutlook != NULL); }

	static BOOL IsOutlookInstalled();
	static BOOL IsUrlHandlerInstalled();
	static BOOL InstallUrlHandler();
	static BOOL QueryInstallUrlHandler(UINT nIDQuery, UINT nMBOptions = MB_YESNO, int nMBSuccess = IDYES);

	static BOOL IsOutlookObject(COleDataObject* pObject);
	static BOOL IsOutlookObject(LPCTSTR szFilePath);

	// caller deletes before COutlookHelper object goes out of scope
	OutlookAPI::_MailItem* GetFirstFileObject(const CStringArray& aFiles);
	OutlookAPI::_MailItem* GetFileObject(LPCTSTR szFilePath);
	OutlookAPI::Selection* GetSelection();
	int GetSelectionCount();
	OutlookAPI::_MailItem* GetFirstSelectedObject();

	static OutlookAPI::_MailItem* GetFirstObject(OutlookAPI::Selection* pSelection);
	static int GetItemData(OutlookAPI::_MailItem& obj, COutlookItemDataMap& mapData, BOOL bIncludeConfidential = TRUE);
	static CString GetItemData(OutlookAPI::_MailItem& obj, OUTLOOK_FIELDTYPE nField);
	static CString GetItemID(OutlookAPI::_MailItem& obj);
	static CString GetItemClass(OutlookAPI::_MailItem& obj);
	static BOOL IsConfidential(OUTLOOK_FIELDTYPE nField);
	static CString FormatItemAsUrl(OutlookAPI::_MailItem& obj);

	static BOOL HasDenyConfidential() { return s_bDenyConfidential; }
	static void ResetDenyConfidential() { s_bDenyConfidential = FALSE; }

	static const CLIPFORMAT CF_OUTLOOK;

protected:
	static OutlookAPI::_Application* s_pOutlook;
	static int s_nRefCount;
	static BOOL s_bDenyConfidential;

protected:
	static CString MapDate(DATE date);

};

#endif // !defined(AFX_OUTLOOKHELPER_H__C57DEEC1_2B2B_490A_9F2B_1A7B9127313F__INCLUDED_)
