// XmlFile.cpp: implementation of the CXmlFile class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"

#include "XmlFile.h"
#include "misc.h"
#include "filemisc.h"

#include "..\3rdparty\xmlnodewrapper.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////

#include "xmlcharmap.h"
#include "htmlcharmap.h"

CString& XML2TXT(CString& xml) { return CHtmlCharMap::ConvertFromRep(xml); } // we use the html map for backwards compatibility
CString& TXT2XML(CString& txt) { return CXmlCharMap::ConvertToRep(txt); }
CString& TXT2HTML(CString& txt) { return CHtmlCharMap::ConvertToRep(txt); }

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CXmlItem::CXmlItem(CXmlItem* pParent, const CString& sName, const CString& sValue, XI_TYPE nType) 
:
m_pParent(pParent), 
m_pSibling(NULL), 
m_sName(sName), 
m_sValue(sValue), 
m_nType(nType)
{
	ValidateString(m_sValue);
}

CXmlItem::CXmlItem(const CXmlItem& xi, CXmlItem* pParent) 
: 
m_pParent(pParent), 
m_pSibling(NULL), 
m_sName(xi.m_sName), 
m_sValue(xi.m_sValue), 
m_nType(xi.m_nType)
{
	Copy(xi, TRUE);
}

CXmlItem::~CXmlItem()
{
	Reset();
}

void CXmlItem::Copy(const CXmlItem& xi, BOOL bCopySiblings)
{
	Reset();
	
	// copy own name and value
	m_sName = xi.GetName();
	m_sValue = xi.GetValue();
	m_nType = xi.GetType();
	
	// copy siblings
	if (bCopySiblings)
	{
		const CXmlItem* pXISibling = xi.GetSibling();
		
		if (pXISibling)
			m_pSibling = new CXmlItem(*pXISibling, m_pParent);
	}
	
	// copy children
	POSITION pos = xi.GetFirstItemPos();
	
	while (pos)
	{
		const CXmlItem* pXIChild = xi.GetNextItem(pos);
		ASSERT (pXIChild);
		
		AddItem(*pXIChild, TRUE);
	}
}

void CXmlItem::Reset()
{
	// delete children
	POSITION pos = m_lstItems.GetHeadPosition();
	
	while (pos)
		delete m_lstItems.GetNext(pos);
	
	m_lstItems.RemoveAll();
	
	// and siblings
	// note: because sibling ~tor calls its own Reset() the chain 
	// of siblings will be correctly cleaned up
	delete m_pSibling;
	m_pSibling = NULL;
	m_nType = XIT_ELEMENT;
}

const CXmlItem* CXmlItem::GetItem(const CString& sItemName, const CString& sSubItem) const
{
	return GetItemEx(sItemName, sSubItem);
}

CXmlItem* CXmlItem::GetItem(const CString& sItemName, const CString& sSubItem)
{
	// cast away constness
	return const_cast<CXmlItem*>(GetItemEx(sItemName, sSubItem));
}

CXmlItem* CXmlItem::GetAddItem(const CString& sItemName, XI_TYPE nType)
{
	ASSERT(!sItemName.IsEmpty());
	
	if (sItemName.IsEmpty())
		return NULL;
	
	CXmlItem* pXI = GetItem(sItemName);
	
	if (pXI == NULL) // means sItemName does not exist
		pXI = AddItem(sItemName, _T(""), nType);
	
	return pXI;
}

BOOL CXmlItem::HasItem(const CString& sItemName, const CString& sSubItemName) const
{
	return (GetItem(sItemName, sSubItemName) != NULL);
}

// special internal version
const CXmlItem* CXmlItem::GetItemEx(const CString& sItemName, const CString& sSubItem) const
{
	if (sItemName.IsEmpty())
		return NULL;
	
	POSITION pos = m_lstItems.GetHeadPosition();
	
	while (pos)
	{
		// match on item name first
		const CXmlItem* pXI = m_lstItems.GetNext(pos);
		
		if (pXI->NameMatches(sItemName))
		{
			// then subitem
			if (!sSubItem.IsEmpty())
				return pXI->GetItemEx(sSubItem);
			
			// else
			return pXI;
		}
	}
	
	// not found
	return NULL;
}

const CXmlItem* CXmlItem::FindItem(const CString& sItemName, const CString& sItemValue, BOOL bSearchChildren) const
{
	return FindItemEx(sItemName, sItemValue, bSearchChildren);
}

CXmlItem* CXmlItem::FindItem(const CString& sItemName, const CString& sItemValue, BOOL bSearchChildren)
{
	// cast away constness
	return (CXmlItem*)FindItemEx(sItemName, sItemValue, bSearchChildren);
}

const CXmlItem* CXmlItem::FindItem(const CString& sItemName, int nItemValue, BOOL bSearchChildren) const
{
	return FindItemEx(sItemName, ToString(nItemValue), bSearchChildren);
}

const CXmlItem* CXmlItem::FindItem(const CString& sItemName, double dItemValue, BOOL bSearchChildren) const
{
	return FindItemEx(sItemName, ToString(dItemValue), bSearchChildren);
}

CXmlItem* CXmlItem::FindItem(const CString& sItemName, int nItemValue, BOOL bSearchChildren)
{
	// cast away constness
	return (CXmlItem*)FindItemEx(sItemName, ToString(nItemValue), bSearchChildren);
}

CXmlItem* CXmlItem::FindItem(const CString& sItemName, double dItemValue, BOOL bSearchChildren)
{
	// cast away constness
	return (CXmlItem*)FindItemEx(sItemName, ToString(dItemValue), bSearchChildren);
}

// special internal version
const CXmlItem* CXmlItem::FindItemEx(const CString& sItemName, const CString& sItemValue, BOOL bSearchChildren) const
{
	// check our name and value
	if (m_sName.CompareNoCase(sItemName) == 0 && m_sValue.Compare(sItemValue) == 0)
		return this;
	
	const CXmlItem* pFound = NULL;
	
	// search all our children
	if (bSearchChildren)
	{
		POSITION pos = GetFirstItemPos();
		
		while (pos && !pFound)
		{
			const CXmlItem* pXIChild = GetNextItem(pos);
			ASSERT (pXIChild);
			
			pFound = pXIChild->FindItemEx(sItemName, sItemValue, TRUE); // child will search its own siblings
		}
	}
	
	// then our siblings
	if (!pFound)
	{
		// only get first sibling as each sibling does its own
		const CXmlItem* pXISibling = GetSibling();
		
		if (pXISibling)
			pFound = pXISibling->FindItemEx(sItemName, sItemValue, TRUE);
	}
	
	return pFound;
}

CString CXmlItem::GetItemValue(const CString& sItemName, const CString& sSubItem) const
{
	if (sItemName.IsEmpty() && sSubItem.IsEmpty())
		return GetValue();
	
	const CXmlItem* pXI = GetItem(sItemName, sSubItem);
	
	if (pXI)
		return pXI->GetValue();
	
	// else
	return "";
}

int CXmlItem::GetItemCount(const CString& sItemName, const CString& sSubItem) const
{
	int nCount = 0;
	const CXmlItem* pXI = GetItem(sItemName, sSubItem);
	
	while (pXI)
	{
		nCount++;
		pXI = pXI->GetSibling();
	}
	
	return nCount;
}

const CXmlItem* CXmlItem::GetItem(const CString& sItemName, int nIndex) const
{
	const CXmlItem* pXI = GetItem(sItemName);
	
	while (pXI && nIndex--)
		pXI = pXI->GetSibling();
	
	return pXI;
}

CXmlItem* CXmlItem::GetItem(const CString& sItemName, int nIndex)
{
	CXmlItem* pXI = GetItem(sItemName);
	
	while (pXI && nIndex--)
		pXI = pXI->GetSibling();
	
	return pXI;
}

CXmlItem* CXmlItem::AddItem(const CString& sName, const CString& sValue, XI_TYPE nType)
{
	ASSERT (!sName.IsEmpty());
	
	if (sName.IsEmpty())
		return NULL;
	
	CXmlItem* pXI = new CXmlItem(this, sName, sValue, nType);
	
	return AddItem(pXI);
}

CXmlItem* CXmlItem::AddItem(const CXmlItem& xi, BOOL bCopySiblings)
{
	CXmlItem* pXI = new CXmlItem(this);
	pXI->Copy(xi, bCopySiblings);
	
	return AddItem(pXI);
}

CXmlItem* CXmlItem::AddItem(CXmlItem* pXI)
{
	CXmlItem* pXIParent = pXI->GetParent();
	
	if (pXIParent && pXIParent != this)
		pXIParent->RemoveItem(pXI);
	
	pXI->m_pParent = this;
	
	// if an item of the same name already exists then add this
	// item as a sibling to the existing item else its a new item
	// so add and map name to this object
	CXmlItem* pXPExist = GetItem(pXI->GetName());
	
	if (pXPExist)
		pXPExist->AddSibling(pXI);
	else
		m_lstItems.AddTail(pXI);
	
	return pXI;
}

CXmlItem* CXmlItem::AddItem(const CString& sName, int nValue, XI_TYPE nType)
{
	ASSERT(nType != XIT_CDATA);
	return AddItem(sName, ToString(nValue), nType);
}

CXmlItem* CXmlItem::AddItem(const CString& sName, double dValue, XI_TYPE nType)
{
	ASSERT(nType != XIT_CDATA);
	return AddItem(sName, ToString(dValue), nType);
}

CXmlItem* CXmlItem::SetItemValue(const CString& sName, const CString& sValue, XI_TYPE nType)
{
	CXmlItem* pXI = GetItem(sName);
	
	if (!pXI)
		return AddItem(sName, sValue, nType);
	
	// else already exists
	pXI->SetValue(sValue);
	pXI->SetType(nType);
	
	return pXI;
}

CXmlItem* CXmlItem::SetItemValue(const CString& sName, int nValue, XI_TYPE nType)
{
	ASSERT(nType != XIT_CDATA);
	return SetItemValue(sName, ToString(nValue), nType);
}

CXmlItem* CXmlItem::SetItemValue(const CString& sName, double dValue, XI_TYPE nType)
{
	ASSERT(nType != XIT_CDATA);
	return SetItemValue(sName, ToString(dValue), nType);
}

BOOL CXmlItem::SetName(const CString& sName)
{
	// can't have any siblings
	if (!sName || !(*sName) || GetSibling())
		return FALSE;
	
	m_sName = sName;
	return TRUE;
}

void CXmlItem::SetValue(const CString& sValue)
{
	m_sValue = sValue;
	ValidateString(m_sValue);
}

void CXmlItem::SetValue(int nValue)
{
	m_sValue = ToString(nValue);
}

void CXmlItem::SetValue(double dValue)
{
	m_sValue = ToString(dValue);
}

BOOL CXmlItem::RemoveItem(CXmlItem* pXI)
{
	if (!pXI)
		return FALSE;
	
	// lookup by name first
	const CString& sName = pXI->GetName();
	CXmlItem* pXIMatch = GetItem(sName);
	
	if (!pXIMatch)
		return FALSE;
	
	// now search the sibling chain looking for exact match
	CXmlItem* pXIPrevSibling = NULL;
	
	while (pXIMatch != pXI)
	{
		pXIPrevSibling = pXIMatch;
		pXIMatch = pXIMatch->GetSibling();
	}
	
	if (!pXIMatch) // no match
		return FALSE;
	
	// else
	ASSERT (pXIMatch == pXI);
	
	CXmlItem* pNextSibling = pXI->GetSibling();
	
	if (!pXIPrevSibling) // head of the chain
	{
		POSITION pos = m_lstItems.Find(pXI);
		
		if (!pNextSibling)
			m_lstItems.RemoveAt(pos);
		else
			m_lstItems.SetAt(pos, pNextSibling);
	}
	else // somewhere else in the chain
	{
		pXIPrevSibling->m_pSibling = pNextSibling; // can be NULL
	}
	
	// clear item's sibling
	pXI->m_pSibling = NULL;
	
	// and parent
	pXI->m_pParent = NULL;
	
	return TRUE;
}

BOOL CXmlItem::DeleteItem(CXmlItem* pXI)
{
	if (RemoveItem(pXI))
	{
		delete pXI;
		return TRUE;
	}
	
	return FALSE;
}

BOOL CXmlItem::DeleteItem(const CString& sItemName, const CString& sSubItemName)
{
	CXmlItem* pXI = GetItem(sItemName, sSubItemName);
	
	if (pXI)
	{
		pXI->Reset(); // delete children and siblings
		DeleteItem(pXI); // delete pXI
	}
	
	return (pXI != NULL);
}

BOOL CXmlItem::AddSibling(CXmlItem* pXI)
{
	ASSERT (pXI);
	
	if (!pXI)
		return FALSE;
	
	// must share the same name and parent
	ASSERT (m_sName.CompareNoCase(pXI->GetName()) == 0 && m_pParent == pXI->GetParent());
	
	if (!(m_sName.CompareNoCase(pXI->GetName()) == 0 && m_pParent == pXI->GetParent()))
		return FALSE;
	
	if (!m_pSibling)
		m_pSibling = pXI;
	else
		m_pSibling->AddSibling(pXI); // recursive
	
	return TRUE;
}

POSITION CXmlItem::GetFirstItemPos() const
{
	return m_lstItems.GetHeadPosition();
}

const CXmlItem* CXmlItem::GetNextItem(POSITION& pos) const
{
	if (!pos)
		return NULL;
	
	// else
	return m_lstItems.GetNext(pos);
}

CXmlItem* CXmlItem::GetNextItem(POSITION& pos)
{
	if (!pos)
		return NULL;
	
	// else
	return m_lstItems.GetNext(pos);
}

BOOL CXmlItem::NameMatches(const CXmlItem* pXITest) const
{
	if (!pXITest)
		return FALSE;
	
	return NameMatches(pXITest->GetName());
}

BOOL CXmlItem::NameMatches(const CString& sName) const
{
	return (m_sName.CompareNoCase(sName) == 0);
}

BOOL CXmlItem::ValueMatches(const CXmlItem* pXITest, BOOL bIgnoreCase) const
{
	if (!pXITest)
		return FALSE;
	
	// else
	return ValueMatches(pXITest->GetValue(), bIgnoreCase);
}

BOOL CXmlItem::ValueMatches(const CString& sValue, BOOL bIgnoreCase) const
{
	if (bIgnoreCase)
		return (m_sValue.CompareNoCase(sValue) == 0);
	
	// else
	return (m_sValue == sValue);
}

BOOL CXmlItem::ItemValueMatches(const CXmlItem* pXITest, const CString& sItemName, BOOL bIgnoreCase) const
{
	if (!sItemName || !*sItemName)
		return FALSE;
	
	const CXmlItem* pXIItem = GetItem(sItemName);
	const CXmlItem* pXITestItem = pXITest->GetItem(sItemName);
	
	if (pXIItem && pXITestItem)
		return pXIItem->ValueMatches(pXITestItem, bIgnoreCase);
	
	// else
	return FALSE;
}

void CXmlItem::SortItems(const CString& sItemName, const CString& sKeyName, BOOL bAscending)
{
	SortItems(sItemName, sKeyName, XISK_STRING, bAscending);
}

void CXmlItem::SortItemsI(const CString& sItemName, const CString& sKeyName, BOOL bAscending)
{
	SortItems(sItemName, sKeyName, XISK_INT, bAscending);
}

void CXmlItem::SortItemsF(const CString& sItemName, const CString& sKeyName, BOOL bAscending)
{
	SortItems(sItemName, sKeyName, XISK_FLOAT, bAscending);
}

void CXmlItem::SortItems(const CString& sItemName, const CString& sKeyName, XI_SORTKEY nKey, BOOL bAscending)
{
	if (!sItemName || !sKeyName)
		return;
	
	// 1. sort immediate children first
	CXmlItem* pXIItem = GetItem(sItemName);
	
	if (!pXIItem)
		return;
	
	// make sure item has key value
	if (!pXIItem->GetItem(sKeyName))
		return;
	
	// make sure at least one sibling exists
	BOOL bContinue = (pXIItem->GetSibling() != NULL);
	
	while (bContinue)
	{
		CXmlItem* pXIPrev = NULL;
		CXmlItem* pXISibling = NULL;
		
		// get this again because we have to anyway 
		// for subsequent loops
		pXIItem = GetItem(sItemName);
		POSITION pos = m_lstItems.Find(pXIItem);
		
		// reset continue flag so that if there are no
		// switches then the sorting is done
		bContinue = FALSE;
		pXISibling = pXIItem->GetSibling();
		
		while (pXISibling)
		{
			int nCompare = CompareItems(pXIItem, pXISibling, sKeyName, nKey);
			
			if (!bAscending)
				nCompare = -nCompare;
			
			if (nCompare > 0)
			{
				// switch items
				if (pXIPrev)
					pXIPrev->m_pSibling = pXISibling;
				
				else // we're at the head of the chain
				{
					m_lstItems.SetAt(pos, pXISibling);
					//	m_mapItems[sItemName] = pXISibling;
				}
				
				pXIItem->m_pSibling = pXISibling->m_pSibling;
				pXISibling->m_pSibling = pXIItem;
				pXIPrev = pXISibling;
				
				bContinue = TRUE; // loop once more
			}
			else
			{
				pXIPrev = pXIItem;
				pXIItem = pXISibling;
			}
			
			pXISibling = pXIItem->GetSibling(); // next
		}
	}
	
	// 2. sort children's children
	pXIItem = GetItem(sItemName);
	
	while (pXIItem)
	{
		pXIItem->SortItems(sItemName, sKeyName, nKey, bAscending);
		pXIItem = pXIItem->GetSibling();
	}
}

int CXmlItem::CompareItems(const CXmlItem* pXIItem1, const CXmlItem* pXIItem2, 
						   const CString& sKeyName, XI_SORTKEY nKey)
{
	const CString& sValue1 = pXIItem1->GetItemValue(sKeyName);
	const CString& sValue2 = pXIItem2->GetItemValue(sKeyName);
	
	double dDiff = 0;
	
	switch (nKey)
	{
	case XISK_STRING:
		dDiff = (double)CString(sValue1).CompareNoCase(sValue2);
		break;
		
	case XISK_INT:
		dDiff = (double)(_ttoi(sValue1) - _ttoi(sValue2));
		break;
		
	case XISK_FLOAT:
		dDiff = _ttof(sValue1) - _ttof(sValue2);
		break;
	}
	
	return (dDiff < 0) ? -1 : ((dDiff > 0) ? 1 : 0);
}

void CXmlItem::ValidateString(CString& sText, TCHAR cReplace)
{
	// remove nasties that XML does not like
	int nLen = sText.GetLength();
	
	for(int nChar = 0; nChar < nLen; nChar++)
	{
		TCHAR c = sText[nChar];
		
		switch (c)
		{
		case 0x2026: // ellipsis
			sText.SetAt(nChar, cReplace);
			break;
		}
		
		// default handling
		// from http://support.microsoft.com/kb/315580
		BOOL bValid =  ((c >= 0xE000 && c <= 0xFFFD) ||
			(c >  0x009F && c <= 0xD7FF) ||
			(c >= 0x0020 && c <  0x0082) ||
			(c == 0x09 || c == 0x0A || c == 0x0D));
		
		if (!bValid)
		{
			TRACE (_T("CXmlItem::ValidateString(replacing 0x%08X with 0x%08X)\n"), c, cReplace);
			sText.SetAt(nChar, cReplace);
		}
	}
}

BOOL CXmlItem::NameIs(const CString& sName) const 
{ 
	return (m_sName == sName); 
}

CString CXmlItem::GetName() const 
{ 
	return m_sName; 
}

CString CXmlItem::GetValue() const 
{ 
	return m_sValue; 
}

int CXmlItem::GetNameLen() const 
{ 
	return m_sName.GetLength(); 
}

int CXmlItem::GetValueLen() const 
{ 
	return m_sValue.GetLength(); 
}

int CXmlItem::GetItemCount() const 
{ 
	return m_lstItems.GetCount(); 
}

int CXmlItem::GetValueI() const 
{ 
	return _ttoi(m_sValue); 
}

int CXmlItem::GetItemValueI(const CString& sItemName, const CString& sSubItemName) const 
{ 
	return _ttoi(GetItemValue(sItemName, sSubItemName)); 
}

double CXmlItem::GetValueF() const 
{ 
	return Misc::Atof(m_sValue); 
}

double CXmlItem::GetItemValueF(const CString& sItemName, const CString& sSubItemName) const 
{ 
	return Misc::Atof(GetItemValue(sItemName, sSubItemName)); 
}

void CXmlItem::DeleteAllItems() 
{ 
	Reset(); 
}

const CXmlItem* CXmlItem::GetParent() const 
{ 
	return m_pParent; 
}

const CXmlItem* CXmlItem::GetSibling() const 
{ 
	return m_pSibling; 
}

CXmlItem* CXmlItem::GetParent() 
{ 
	return m_pParent; 
}

CXmlItem* CXmlItem::GetSibling() 
{ 
	return m_pSibling; 
}

BOOL CXmlItem::SetType(XI_TYPE nType)
{
	// some sanity checks
	ASSERT (nType == XIT_ELEMENT || !m_lstItems.GetCount());
	
	m_nType = nType;
	
	return TRUE;
}

BOOL CXmlItem::IsCDATA() const 
{ 
	return (m_nType == XIT_CDATA); 
}

BOOL CXmlItem::IsAttribute(int nMaxAttribLen) const 
{ 
	return (m_nType == XIT_ATTRIB && GetValueLen() <= nMaxAttribLen && !m_lstItems.GetCount()); 
}

CString CXmlItem::ToString(int nValue) 
{ 
	return Misc::Format(nValue);
}

CString CXmlItem::ToString(double dValue) 
{ 
	// non-localized version 
	CString sValue; 
	sValue.Format(_T("%.8f"), dValue); 
	return sValue; 
}

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CXmlFile::CXmlFile(const CString& sRootItemName) 
: 
m_xiRoot(NULL, sRootItemName), 
m_pCallback(NULL)
{
}

CXmlFile::~CXmlFile()
{
	
}

BOOL CXmlFile::LoadContent(const CString& sContent, const CString& sRootItemName)
{
	Reset();
	
	CXmlDocumentWrapper doc;
	
	if (!doc.IsValid())
		return FALSE;
	
	CString sRootName(sRootItemName);
	
	if (sRootName.IsEmpty())
		sRootName = m_xiRoot.GetName();
	
	if (!doc.LoadXML(sContent))
	{
		// try removing any bad chars
		CString sTemp(sContent);
		FixInputString(sTemp, sRootName);
		
		// then try again
		if (!doc.LoadXML(sTemp))
		{
			m_nFileError = XFL_BADMSXML;
			return FALSE;
		}
	}
	
	// everything is cool
	if (!ParseRootItem(sRootName, &doc))
	{
		m_nFileError = XFL_MISSINGROOT;
		return FALSE;
	}
	
	// else
	return TRUE;
}

BOOL CXmlFile::Load(const CString& sFilePath, const CString& sRootItemName, IXmlParse* pCallback)
{
	if (sFilePath.IsEmpty())
		return FALSE;
	
	if (GetFileHandle() != (HANDLE)CStdioFileEx::hFileNull)
		Close();
	
	m_pCallback = pCallback;
	
	if (Open(sFilePath, XF_READ))
	{
		if (!sRootItemName.IsEmpty())
			return LoadEx(sRootItemName, pCallback);
		
		else if (m_xiRoot.GetNameLen())
			return LoadEx(m_xiRoot.GetName(), pCallback);
	}
	
	// else
	return FALSE;
}

BOOL CXmlFile::Save(const CString& sFilePath, SFE_SAVEAS nSaveAs)
{
	if (sFilePath.IsEmpty())
		return FALSE;
	
	if (GetFileHandle() != (HANDLE)CStdioFileEx::hFileNull)
		Close();
	
	if (Open(sFilePath, XF_WRITE, nSaveAs))
	{
		BOOL bRes = SaveEx();
		Close();
		
		return bRes;
	}
	
	// error handling
	m_nFileError = GetLastError();
	
	return FALSE;
}

BOOL CXmlFile::Open(const CString& sFilePath, XF_OPEN nOpenFlag, SFE_SAVEAS nSaveAs)
{
	if (sFilePath.IsEmpty())
		return FALSE;
	
	if (GetFileHandle() != (HANDLE)CStdioFileEx::hFileNull)
		Close();
	
	UINT nFileOpenFlags = 0;
	
	switch (nOpenFlag)
	{
	case XF_READ:
		nFileOpenFlags = CStdioFileEx::shareDenyNone | CStdioFileEx::modeRead;
		break;
		
	case XF_WRITE:
		nFileOpenFlags = CStdioFileEx::shareExclusive | 
			CStdioFileEx::modeWrite | 
			CStdioFileEx::modeCreate;
		break;
		
	case XF_READWRITE:
		nFileOpenFlags = CStdioFileEx::shareExclusive | 
			CStdioFileEx::modeReadWrite | 
			CStdioFileEx::modeCreate | 
			CStdioFileEx::modeNoTruncate;
		break;
	}
	
	BOOL bRes = (nFileOpenFlags && CStdioFileEx::Open(sFilePath, nFileOpenFlags, nSaveAs));
	
	if (!bRes)
		m_nFileError = GetLastError();
	
	return bRes;
}

BOOL CXmlFile::SaveEx()
{
	if (GetFileHandle() == (HANDLE)CStdioFileEx::hFileNull)
		return FALSE;
	
	BOOL bRes = FALSE;
	CString sXml;
	
	if (Export(sXml))
	{	
		try
		{
			// move to start
			Seek(0, CStdioFileEx::begin);
			
			// write the xml
			CStdioFileEx::WriteString((LPCTSTR)sXml);
			
			// update the file end
			VERIFY(::SetEndOfFile(GetFileHandle()));
			
			// verify file length matches length of xml
			DWORD dwFileSizeInBytes = ::GetFileSize(GetFileHandle(), NULL);
			DWORD dwXmlSizeInBytes = GetBytesWritten();
			
			if (dwFileSizeInBytes == dwXmlSizeInBytes)
				bRes = TRUE;
		}
		catch (...)
		{
			m_nFileError = GetLastError();
		}
	}
	
	return bRes;
}

BOOL CXmlFile::LoadEx(const CString& sRootItemName, IXmlParse* pCallback)
{
	m_nFileError = XFL_NONE; // reset
	
	if (GetFileHandle() == (HANDLE)CStdioFileEx::hFileNull)
	{
		m_nFileError = ERROR_INVALID_HANDLE;
		return FALSE;
	}
	
	// concatenate entire file into one long string
	CString sFileContents;
	
	try
	{
		CStdioFileEx::ReadFile(sFileContents);
	}
	catch (...)
	{
		m_nFileError = GetLastError();
		
		// cleanup
		m_pCallback = NULL;
		
		return FALSE;
	}
	
	CString sRootName(sRootItemName);
	
	if (sRootName.IsEmpty())
		sRootName = m_xiRoot.GetName();
	
	m_pCallback = pCallback;
	
	BOOL bRes = FALSE;
	
	try
	{
		CXmlDocumentWrapper doc;
		
		if (doc.IsValid())
		{
			if (!doc.LoadXML(sFileContents))
			{
				// try removing any bad chars
				FixInputString(sFileContents, sRootName);
				
				// then try again
				if (!doc.LoadXML(sFileContents))
					m_nFileError = XFL_BADMSXML;
			}
			
			// now read it into CXmlItem structures
			if (m_nFileError == XFL_NONE)
			{
				if (!ParseRootItem(sRootName, &doc))
					m_nFileError = XFL_MISSINGROOT;
				else
					bRes = TRUE;
			}
		}
	}
	catch (...)
	{
		m_nFileError = XFL_BADMSXML;
	}
	
	// cleanup
	m_pCallback = NULL;
	
	return bRes;
}

void CXmlFile::FixInputString(CString& sXml, const CString& sRootItem)
{
	CXmlItem::ValidateString(sXml);
	
	// check for any other obvious problems
	if (sRootItem)
	{
		CString sRoot(sRootItem);
		sRoot = '<' + sRoot;
		
		int nRoot = sXml.Find(sRoot);
		int nHeader = sXml.Find(_T("<?xml"));
		
		if (nHeader > nRoot)
		{
			// what should we do?
			
			/*
			// if there is another instance of sRootItem after the 
			// header tag then try deleting everything before the header
			// tag
			if (sXml.Find(sRoot, nHeader) != -1)
			{
			sXml = sXml.Right(nHeader);
			}
			else // try moving the header to the start
			{
			int nHeaderEnd = sXml.Find('>', nHeader) + 1;
			CString sHeader = sXml.Mid(nHeader, nHeaderEnd - nHeader);
			
			  sXml = sHeader + sXml.Left(nHeader) + sXml.Right(nHeaderEnd);
			  }
			*/
		}
	}
	
}

BOOL CXmlFile::ParseRootItem(const CString& sRootItemName, CXmlDocumentWrapper* pDoc)
{
	ASSERT (pDoc);
	
	m_xiRoot.Reset();
	
	CString sRootItem(sRootItemName), sItem;
	sRootItem.TrimLeft();
	sRootItem.TrimRight();
	
	// save off the header string
	// Valik - Change IXMLDOMNode* to IXMLDOMNodePtr to prevent an ambiguous symbol error (C2872) in VC 7.1
	CXmlNodeWrapper node(pDoc->AsNode());
	
	if (0 != node.Name().CompareNoCase(sRootItem))
		return FALSE;
	
	m_sHeader = pDoc->GetHeader();
	
	// parse rest of file
	ParseItem(m_xiRoot, &node);
	
	return TRUE;
}

BOOL CXmlFile::ParseItem(CXmlItem& xi, CXmlNodeWrapper* pNode)
{
	CStringArray aNames, aValues;
	
	int nNumAttrib = pNode->GetAttributes(aNames, aValues);
	
	for (int nAttrib = 0; nAttrib < nNumAttrib; nAttrib++)
	{
		const CString& sName = aNames[nAttrib];
		const CString& sVal = aValues[nAttrib];
		
		xi.AddItem(sName, sVal);
		
		if (!ContinueParsing(sName, sVal))
			return TRUE;
	}
	
	CXmlNodeWrapper nodeChild(pNode->GetFirstChildNode());
	
	//for (int nNode = 0; nNode < nNumNodes; nNode++)
	while (nodeChild.IsValid())
	{
		CString sName(nodeChild.Name());
		CString sVal(nodeChild.GetText());
		
		// Valik - Fully qualify NODE_CDATA_SECTION to prevent an ambiguous symbol error (C2872) in VC 7.1
		int nNodeType = nodeChild.NodeTypeVal();
		XI_TYPE nType = XIT_ELEMENT;
		
		if (nNodeType == MSXML2::NODE_CDATA_SECTION)
			nType = XIT_CDATA;
		
		else if (nNodeType == MSXML2::NODE_ATTRIBUTE)
			nType = XIT_ATTRIB;
		
		// if sName is empty then sVal relates to pNode
		if (!sName.IsEmpty())
		{
			CXmlItem* pXI = xi.AddItem(sName, sVal, nType);
			
			if (!ContinueParsing(sName, sVal))
				return TRUE;
			
			ParseItem(*pXI, &nodeChild);
		}
		// need to take care here not to overwrite a node's value by carriage returns
		// which can result if we load the XML preserving whitespace
		else
		{
			//BOOL bHasValue = (xi.GetValueLen() != 0);
			//BOOL bValueIsCR = (sVal == "\n");
			
			//if (nodeChild.IsPreservingWhiteSpace() && bHasValue && bValueIsCR)
			if (nodeChild.IsPreservingWhiteSpace() && (xi.GetValueLen() != 0) && (sVal == "\n"))
			{
				// ignore
				ASSERT(1); // for debugging
			}
			else
			{
				xi.SetValue(sVal);
				xi.SetType(nType);
			}
		}
		
		nodeChild = nodeChild.GetNextSibling();
	}
	
	return TRUE;
}

void CXmlFile::CopyFrom(const CXmlFile& file)
{
	m_xiRoot.Reset();
	m_xiRoot.Copy(file.m_xiRoot, TRUE);
}

BOOL CXmlFile::Export(CString& sOutput) const
{
	BOOL bRes = FALSE;
	sOutput.Empty();
	
	try
	{
		CXmlDocumentWrapper doc(GetXmlHeader(), m_xiRoot.GetName()); 
		
		if (BuildDOM(&doc))
		{
			sOutput = doc.GetXML(TRUE);
			
			if (sOutput.IsEmpty()) // sanity check
				m_nFileError = XFL_BADMSXML;
			else
			{
				// carriage return after each attribute
				sOutput.Replace(_T("><"), _T(">\r\n<"));
				bRes = TRUE;
			}
		}
	}
	catch (...)
	{
		m_nFileError = XFL_BADMSXML;
	}
	
	return bRes;
}

BOOL CXmlFile::BuildDOM(CXmlDocumentWrapper* pDoc) const
{
	if (!pDoc || !pDoc->IsValid())
		return FALSE;
	
	// Valik - Change IXMLDOMNode* to IXMLDOMNodePtr to prevent an ambiguous symbol error (C2872) in VC 7.1
	MSXML2::IXMLDOMNodePtr pNode = pDoc->AsNode();
	CXmlNodeWrapper node(pNode);
	
	BOOL bRes = Export(&m_xiRoot, &node);
	
	return bRes;
}

BOOL CXmlFile::Transform(const CString& sTransformPath, CString& sOutput, const CString& sEncoding) const
{
	CXmlDocumentWrapper doc(m_sHeader, m_xiRoot.GetName());
	
	if (BuildDOM(&doc))
	{
		sOutput = doc.Transform(sTransformPath);
		
		// encoding
		// note: the Transform function always returns UTF-16
#ifdef _UNICODE
		UNREFERENCED_PARAMETER(sEncoding);
#else
		if (!sEncoding.IsEmpty())
			sOutput.Replace(_T("UTF-16"), sEncoding);
#endif
	}
	else
		sOutput.Empty();
	
	return TRUE;
}

BOOL CXmlFile::TransformToFile(const CString& sTransformPath, const CString& sOutputPath, const CString& sEncoding) const
{
	CString sOutput;
	
	if (Transform(sTransformPath, sOutput, sEncoding))
	{
		return FileMisc::SaveFile(sOutputPath, sOutput);//, SFE_MULTIBYTE);
	}
	
	return FALSE;
}

BOOL CXmlFile::Export(const CXmlItem* pItem, CXmlNodeWrapper* pNode) const
{
	// own value
	if (pItem->GetValueLen())
		pNode->SetText(pItem->GetValue());
	
	// attributes and items
	POSITION pos = pItem->GetFirstItemPos();
	int nNode = 0;
	
	while (pos)
	{
		const CXmlItem* pXIChild = pItem->GetNextItem(pos);
		ASSERT (pXIChild);
		
		CString sItem = pXIChild->GetName();
		
		if (pXIChild->IsAttribute())
		{
			ASSERT (!pXIChild->GetSibling());
			pNode->SetValue(sItem, pXIChild->GetValue());
		}
		else if (pXIChild->IsCDATA())
		{
			// create a named node to wrap the CDATA
			MSXML2::IXMLDOMNodePtr pChildNode = pNode->InsertNode(nNode++, (LPCTSTR)sItem);
			MSXML2::IXMLDOMCDATASectionPtr pCData = 
				pNode->ParentDocument()->createCDATASection((LPCTSTR)pXIChild->GetValue());
			pChildNode->appendChild(pCData);
		}
		else // node
		{
			while (pXIChild)
			{
				// Valik - Change IXMLDOMNode* to IXMLDOMNodePtr to prevent an ambiguous symbol error (C2872) in VC 7.1
				MSXML2::IXMLDOMNodePtr pChildNode = pNode->InsertNode(nNode++, (LPCTSTR)sItem);
				CXmlNodeWrapper nodeChild(pChildNode);
				ASSERT (nodeChild.IsValid());
				
				Export(pXIChild, &nodeChild);
				
				// siblings
				pXIChild = pXIChild->GetSibling();
			}
		}
	}
	
	return TRUE;
}

void CXmlFile::Trace() const 
{ 
#ifdef _DEBUG
	CString sXml;
	Export(sXml);
	
	// because the string might be long, output it in chunks of 255 chars
	int nPos = 0;
	
	while (nPos < sXml.GetLength())
	{
		OutputDebugString(sXml.Mid(nPos, 255));
		nPos += 255;
	}
#endif
}

void CXmlFile::Reset() 
{ 
	m_xiRoot.Reset(); 
	m_pCallback = NULL;
}

int CXmlFile::GetItemCount() const 
{ 
	return m_xiRoot.GetItemCount(); 
}

void CXmlFile::Close() 
{ 
	CStdioFileEx::Close(); 
}

int CXmlFile::GetLastFileError() 
{ 
	return m_nFileError; 
}

const CXmlItem* CXmlFile::Root() const 
{ 
	return &m_xiRoot; 
}

CXmlItem* CXmlFile::Root() 
{ 
	return &m_xiRoot; 
}

CString CXmlFile::GetRootValue() const
{
	return m_xiRoot.GetValue();
}

int CXmlFile::GetRootValueI() const
{
	return m_xiRoot.GetValueI();
}

double CXmlFile::GetRootValueF() const
{
	return m_xiRoot.GetValueF();
}

const CXmlItem* CXmlFile::GetItem(const CString& sItemName) const 
{ 
	return m_xiRoot.GetItem(sItemName); 
} 

CXmlItem* CXmlFile::GetItem(const CString& sItemName) 
{ 
	return m_xiRoot.GetItem(sItemName); 
}

const CXmlItem* CXmlFile::GetItem(const CString& sItemName, int nIndex) const
{
	return m_xiRoot.GetItem(sItemName, nIndex); 
}

CXmlItem* CXmlFile::GetItem(const CString& sItemName, int nIndex)
{
	return m_xiRoot.GetItem(sItemName, nIndex); 
}

CXmlItem* CXmlFile::GetAddItem(const CString& sItemName, XI_TYPE nType) 
{ 
	return m_xiRoot.GetAddItem(sItemName, nType); 
}

const CXmlItem* CXmlFile::FindItem(const CString& sItemName, const CString& sItemValue, BOOL bSearchChildren) const
{ 
	return m_xiRoot.FindItem(sItemName, sItemValue, bSearchChildren); 
}

CXmlItem* CXmlFile::FindItem(const CString& sItemName, const CString& sItemValue, BOOL bSearchChildren)
{ 
	return m_xiRoot.FindItem(sItemName, sItemValue, bSearchChildren); 
}

const CXmlItem* CXmlFile::FindItem(const CString& sItemName, int nItemValue, BOOL bSearchChildren) const
{ 
	return m_xiRoot.FindItem(sItemName, nItemValue, bSearchChildren); 
}

CXmlItem* CXmlFile::FindItem(const CString& sItemName, int nItemValue, BOOL bSearchChildren)
{ 
	return m_xiRoot.FindItem(sItemName, nItemValue, bSearchChildren); 
}

const CXmlItem* CXmlFile::FindItem(const CString& sItemName, double dItemValue, BOOL bSearchChildren) const
{ 
	return m_xiRoot.FindItem(sItemName, dItemValue, bSearchChildren); 
}

CXmlItem* CXmlFile::FindItem(const CString& sItemName, double dItemValue, BOOL bSearchChildren)
{ 
	return m_xiRoot.FindItem(sItemName, dItemValue, bSearchChildren); 
}

CXmlItem* CXmlFile::AddItem(const CString& sName, const CString& sValue, XI_TYPE nType) 
{ 
	return m_xiRoot.AddItem(sName, sValue, nType); 
}

CXmlItem* CXmlFile::AddItem(const CString& sName, int nValue, XI_TYPE nType) 
{ 
	return m_xiRoot.AddItem(sName, nValue, nType); 
}

CXmlItem* CXmlFile::AddItem(const CString& sName, double dValue, XI_TYPE nType) 
{ 
	return m_xiRoot.AddItem(sName, dValue, nType); 
}

BOOL CXmlFile::DeleteItem(CXmlItem* pXI) 
{ 
	return m_xiRoot.DeleteItem(pXI); 
}

BOOL CXmlFile::DeleteItem(const CString& sItemName, const CString& sSubItemName) 
{ 
	return m_xiRoot.DeleteItem(sItemName, sSubItemName); 
}

CString CXmlFile::GetItemValue(const CString& sItemName, const CString& sSubItemName) const 
{ 
	return m_xiRoot.GetItemValue(sItemName, sSubItemName); 
}

int CXmlFile::GetItemCount(const CString& sItemName) const
{
	return m_xiRoot.GetItemCount(sItemName); 
}

int CXmlFile::GetItemValueI(const CString& sItemName, const CString& sSubItemName) const 
{ 
	return m_xiRoot.GetItemValueI(sItemName, sSubItemName); 
}

double CXmlFile::GetItemValueF(const CString& sItemName, const CString& sSubItemName) const 
{ 
	return m_xiRoot.GetItemValueF(sItemName, sSubItemName); 
}

CXmlItem* CXmlFile::SetItemValue(const CString& sName, const CString& sValue, XI_TYPE nType)
{
	return m_xiRoot.SetItemValue(sName, sValue, nType); 
}

CXmlItem* CXmlFile::SetItemValue(const CString& sName, int nValue, XI_TYPE nType)
{
	return m_xiRoot.SetItemValue(sName, nValue, nType); 
}

CXmlItem* CXmlFile::SetItemValue(const CString& sName, double dValue, XI_TYPE nType)
{
	return m_xiRoot.SetItemValue(sName, dValue, nType); 
}

CString CXmlFile::GetFilePath() const 
{ 
	return CStdioFileEx::GetFilePath(); 
}

CString CXmlFile::GetFileName() const 
{ 
	return FileMisc::GetFileNameFromPath(GetFilePath());
}

const HANDLE CXmlFile::GetFileHandle() const 
{ 
	return (HANDLE)CStdioFileEx::m_hFile; 
}

CString CXmlFile::GetXmlHeader() const 
{ 
	return m_sHeader.IsEmpty() ? CXmlDocumentWrapper().GetHeader() : m_sHeader; 
}

void CXmlFile::SetXmlHeader(const CString& sHeader) 
{ 
	m_sHeader = sHeader; m_sHeader.MakeLower(); 
}

void CXmlFile::SortItems(const CString& sItemName, const CString& sKeyName, BOOL bAscending)
{ 
	m_xiRoot.SortItems(sItemName, sKeyName, bAscending); 
}

void CXmlFile::SortItemsI(const CString& sItemName, const CString& sKeyName, BOOL bAscending)
{ 
	m_xiRoot.SortItemsI(sItemName, sKeyName, bAscending); 
}

void CXmlFile::SortItemsF(const CString& sItemName, const CString& sKeyName, BOOL bAscending)
{ 
	m_xiRoot.SortItemsF(sItemName, sKeyName, bAscending); 
}

BOOL CXmlFile::ContinueParsing(const CString& sItem, const CString& sValue) 
{ 
	return (!m_pCallback || m_pCallback->Continue(sItem, sValue)); 
}
