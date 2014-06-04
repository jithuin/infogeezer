// TDCCustomAttributeHelper.cpp: implementation of the CTDCCustomAttributeHelper class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "todolist.h"
#include "TDCCustomAttributeHelper.h"
#include "TDCstruct.h"
#include "tdliconcombobox.h"

#include "..\shared\DialogHelper.h"
#include "..\shared\checkcombobox.h"
#include "..\shared\autocombobox.h"
#include "..\shared\wclassdefines.h"
#include "..\shared\maskedit.h"
#include "..\shared\orderedtreectrl.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CWnd* CTDCCustomAttributeHelper::CreateCustomAttribute(const TDCCUSTOMATTRIBUTEDEFINITION& attrib, 
													   const CTDCImageList& ilImages,
													   CWnd* pParent, UINT nCtrlID, BOOL bVisible)
{
	DWORD dwDataType = attrib.GetDataType();
	DWORD dwListType = attrib.GetListType();

	CWnd* pControl = NULL;
	DWORD dwStyle = WS_CHILD | WS_TABSTOP | (bVisible ? WS_VISIBLE : 0), dwExStyle = 0;
	LPCTSTR szClass = NULL, szWindowText = NULL;

	switch (dwListType)
	{
	case TDCCA_NOTALIST:
		{
			switch (dwDataType)
			{
			case TDCCA_STRING:
				pControl = new CEdit;
				szClass = WC_EDIT;
				dwStyle |= ES_LEFT | ES_AUTOHSCROLL;
				dwExStyle |= WS_EX_CLIENTEDGE;
				break;
				
			case TDCCA_DATE:
				pControl = new CDateTimeCtrl;
				szClass = WC_DATETIMEPICK;
				dwStyle |= DTS_SHORTDATEFORMAT | DTS_RIGHTALIGN | DTS_SHOWNONE;
				dwExStyle |= WS_EX_CLIENTEDGE;
				break;
				
			case TDCCA_INTEGER:
				pControl = new CMaskEdit(_T("0123456789"));
				szClass = WC_EDIT;
				dwStyle |= ES_LEFT | ES_AUTOHSCROLL;
				dwExStyle |= WS_EX_CLIENTEDGE;
				break;
				
			case TDCCA_DOUBLE:
				pControl = new CMaskEdit(_T(".0123456789"), ME_LOCALIZEDECIMAL);
				szClass = WC_EDIT;
				dwStyle |= ES_LEFT | ES_AUTOHSCROLL;
				dwExStyle |= WS_EX_CLIENTEDGE;
				break;
		
			// these don't have controls
			case TDCCA_BOOL:
			case TDCCA_ICON:
				break;
			}
		}
		break;
		
	case TDCCA_AUTOLIST:
		pControl = new CAutoComboBox(ACBS_ALLOWDELETE);
		szClass = WC_COMBOBOX;
		dwStyle |= CBS_DROPDOWN | CBS_SORT | WS_VSCROLL | CBS_AUTOHSCROLL | CBS_HASSTRINGS;
		dwExStyle |= WS_EX_CLIENTEDGE;

		// add number mask as required
		switch (dwDataType)
		{
		case TDCCA_INTEGER:
			((CAutoComboBox*)pControl)->SetEditMask(_T("0123456789"));
			break;
			
		case TDCCA_DOUBLE:
			((CAutoComboBox*)pControl)->SetEditMask(_T(".0123456789"), ME_LOCALIZEDECIMAL);
			break;
		}
		break;
		
	case TDCCA_FIXEDLIST:
		szClass = WC_COMBOBOX;
		dwStyle |= CBS_DROPDOWNLIST | WS_VSCROLL | CBS_AUTOHSCROLL;
		dwExStyle |= WS_EX_CLIENTEDGE;

		switch (dwDataType)
		{
		case TDCCA_ICON:
			pControl = new CTDLIconComboBox(ilImages);
			dwStyle |= CBS_HASSTRINGS | CBS_OWNERDRAWFIXED;
			break;
			
		default:
			pControl = new CComboBox;
			break;
		}
		break;
		
	case TDCCA_AUTOMULTILIST:
		pControl = new CCheckComboBox(ACBS_ALLOWDELETE);
		szClass = WC_COMBOBOX;
		dwStyle |= CBS_DROPDOWN | CBS_SORT | WS_VSCROLL | CBS_AUTOHSCROLL | CBS_OWNERDRAWFIXED | CBS_HASSTRINGS;
		dwExStyle |= WS_EX_CLIENTEDGE;

		// add number mask as required
		switch (dwDataType)
		{
		case TDCCA_INTEGER:
			((CAutoComboBox*)pControl)->SetEditMask(_T("0123456789"));
			break;
			
		case TDCCA_DOUBLE:
			((CAutoComboBox*)pControl)->SetEditMask(_T(".0123456789"), ME_LOCALIZEDECIMAL);
			break;
		}
		break;
		
	case TDCCA_FIXEDMULTILIST:
		pControl = new CCheckComboBox;
		szClass = WC_COMBOBOX;
		dwStyle |= CBS_DROPDOWNLIST | WS_VSCROLL | CBS_AUTOHSCROLL | CBS_OWNERDRAWFIXED | CBS_HASSTRINGS;
		dwExStyle |= WS_EX_CLIENTEDGE;
		break;
	}

	ASSERT (pControl);

	if (pControl)
	{
		HWND hwnd = ::CreateWindowEx(dwExStyle, szClass, szWindowText, dwStyle, 0, 0, 0, 0, 
									pParent->GetSafeHwnd(), (HMENU)nCtrlID, NULL, NULL);

		if (hwnd == NULL)
		{
			delete pControl;
			pControl = NULL;
		}
		else 
		{
			pControl->SubclassWindow(hwnd);

			// font
			HFONT hFont = (HFONT)pParent->SendMessage(WM_GETFONT); // gets the parent's font
			
			if (!hFont)
				hFont = (HFONT)::GetStockObject(DEFAULT_GUI_FONT);
			
			::SendMessage(hwnd, WM_SETFONT, (WPARAM)hFont, 0); 

			// add default data to lists
			if (dwListType != TDCCA_NOTALIST) 
			{
				ASSERT(pControl->IsKindOf(RUNTIME_CLASS(CComboBox)));
				CDialogHelper::SetComboBoxItems((CComboBox&)(*pControl), attrib.aDefaultListData);
			}
		}
	}

	return pControl;
}

CWnd* CTDCCustomAttributeHelper::CreateCustomAttributeLabel(const TDCCUSTOMATTRIBUTEDEFINITION& attrib, 
															CWnd* pParent, UINT nCtrlID, BOOL bVisible)
{
	CStatic* pLabel = new CStatic;
	ASSERT (pLabel);

	if (pLabel)
	{
		DWORD dwStyle = WS_CHILD | (bVisible ? WS_VISIBLE : 0) | SS_CENTERIMAGE;
		CString sLabel(attrib.sLabel);

		if (sLabel.IsEmpty())
			sLabel = attrib.sColumnTitle;

		if (!pLabel->Create(sLabel, dwStyle, CRect(0, 0, 0, 0), pParent, nCtrlID))
		{
			delete pLabel;
			pLabel = NULL;
		}
		else // font
		{
			HFONT hFont = (HFONT)pParent->SendMessage(WM_GETFONT); // gets the parent's font
			
			if (!hFont)
				hFont = (HFONT)::GetStockObject(DEFAULT_GUI_FONT);
			
			pLabel->SendMessage(WM_SETFONT, (WPARAM)hFont, 0); 

			// assume label is already in user's language
			CLocalizer::EnableTranslation(*pLabel, FALSE);
		}
	}

	return pLabel;
}

BOOL CTDCCustomAttributeHelper::GetControl(UINT nCtrlID, const CTDCCustomControlArray& aControls, 
										   CUSTOMATTRIBCTRLITEM& ctrl)
{
	int nCtrl = aControls.GetSize();

	while (nCtrl--)
	{
		const CUSTOMATTRIBCTRLITEM& ctrlCustom = aControls.GetData()[nCtrl];

		if (ctrlCustom.nCtrlID == nCtrlID)
		{
			ctrl = ctrlCustom;
			return TRUE;
		}
	}

	// not found 
	return FALSE;
}

BOOL CTDCCustomAttributeHelper::GetControl(const CString& sUniqueID, const CTDCCustomControlArray& aControls, 
										   CUSTOMATTRIBCTRLITEM& ctrl)
{
	int nCtrl = aControls.GetSize();

	while (nCtrl--)
	{
		const CUSTOMATTRIBCTRLITEM& ctrlCustom = aControls.GetData()[nCtrl];

		if (ctrlCustom.sAttribID == sUniqueID)
		{
			ctrl = ctrlCustom;
			return TRUE;
		}
	}

	// not found 
	return FALSE;
}

void CTDCCustomAttributeHelper::RebuildCustomAttributeUI(const CTDCCustomAttribDefinitionArray& aAttribDefs, 
														 CTDCCustomControlArray& aControls, 
														 const CTDCImageList& ilImages, 
														 COrderedTreeCtrl& tree,
														 CWnd* pParent, int nColumnPos)
{
	ASSERT_VALID(pParent);

	// remove all existing custom attribute fields
	for (int nCtrl = 0; nCtrl < aControls.GetSize(); nCtrl++)
	{
		const CUSTOMATTRIBCTRLITEM& ctrl = aControls.GetData()[nCtrl];

		delete pParent->GetDlgItem(ctrl.nCtrlID);
		delete pParent->GetDlgItem(ctrl.nLabelID);
	}

	aControls.RemoveAll();

	// remove all custom columns
	for (int nColID = TDCC_CUSTOMCOLUMN_FIRST; nColID <= TDCC_CUSTOMCOLUMN_LAST; nColID++)
	{
		tree.RemoveGutterColumn(nColID);
	}

	// recreate controls and columns
	UINT nID = IDC_FIRST_CUSTOMDATAFIELD;

	for (int nAttrib = 0; nAttrib < aAttribDefs.GetSize(); nAttrib++)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& attribDef = aAttribDefs.GetData()[nAttrib];

		// don't add disabled controls
		if (!attribDef.bEnabled)
			continue;

		TDC_COLUMN nColID = (TDC_COLUMN)(TDCC_CUSTOMCOLUMN_FIRST + nAttrib);

		// NOTE: flag and image types don't need controls because they are 
		// handled by clicking the tasklist directly
		switch (attribDef.GetDataType())
		{
		case TDCCA_BOOL:
			break;

		case TDCCA_ICON:
			if (attribDef.GetListType() == TDCCA_NOTALIST)
				break;
			// else fall thru

		default:
			{
				CUSTOMATTRIBCTRLITEM ctrl;
				
				ctrl.nCol = nColID;
				ctrl.nCtrlID = nID++;
				ctrl.nLabelID = nID++;
				ctrl.sAttribID = attribDef.sUniqueID;
				
				CWnd* pLabel = CreateCustomAttributeLabel(attribDef, pParent, ctrl.nLabelID, FALSE);
				ASSERT_VALID(pLabel);
				
				CWnd* pCtrl = CreateCustomAttribute(attribDef, ilImages, pParent, ctrl.nCtrlID, FALSE);
				ASSERT_VALID(pCtrl);
				
				if (pCtrl && pLabel)
				{
					ctrl.nCol = (TDC_COLUMN)(TDCC_CUSTOMCOLUMN_FIRST + nAttrib);
					
					aControls.Add(ctrl);
				}
				else
				{
					ASSERT(0);
					delete pCtrl;
					delete pLabel;
				}
			}
		}

		// always add column
		tree.InsertGutterColumn(nColumnPos++, nColID, attribDef.GetColumnTitle(), 0, attribDef.nColumnAlignment);
	}

	tree.RecalcGutter();
}

CString CTDCCustomAttributeHelper::GetAttributeTypeID(TDC_COLUMN nColID, const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	TDCCUSTOMATTRIBUTEDEFINITION attribDef;

	if (GetAttributeDef(nColID, aAttribDefs, attribDef))
		return attribDef.sUniqueID;

	// all else
	return _T("");
}

BOOL CTDCCustomAttributeHelper::GetAttributeDef(TDC_COLUMN nColID, 
														const CTDCCustomAttribDefinitionArray& aAttribDefs,
														TDCCUSTOMATTRIBUTEDEFINITION& attribDef)
{
	if (!IsCustomColumn(nColID))
		return FALSE;

	for (int nCol = 0; nCol < aAttribDefs.GetSize(); nCol++)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& def = aAttribDefs.GetData()[nCol];

		if (def.GetColumnID() == nColID)
		{
			attribDef = def;
			return TRUE;
		}
	}

	// all else
	ASSERT(0);
	return FALSE;
}

TDC_ATTRIBUTE CTDCCustomAttributeHelper::GetAttributeID(TDC_COLUMN nColID, 
											const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	if (!IsCustomColumn(nColID))
		return TDCA_NONE;

	for (int nCol = 0; nCol < aAttribDefs.GetSize(); nCol++)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& def = aAttribDefs.GetData()[nCol];

		if (def.GetColumnID() == nColID)
			return def.GetAttributeID();
	}

	// all else
	ASSERT(0);
	return TDCA_NONE;
}

TDC_ATTRIBUTE CTDCCustomAttributeHelper::GetAttributeID(const CString& sUniqueID, 
											const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	for (int nCol = 0; nCol < aAttribDefs.GetSize(); nCol++)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& def = aAttribDefs.GetData()[nCol];

		if (sUniqueID.CompareNoCase(def.sUniqueID) == 0)
			return def.GetAttributeID();
	}

	// all else
	ASSERT(0);
	return TDCA_NONE;
}

CString CTDCCustomAttributeHelper::GetAttributeTypeID(TDC_ATTRIBUTE nAttribID, const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	TDCCUSTOMATTRIBUTEDEFINITION attribDef;

	if (GetAttributeDef(nAttribID, aAttribDefs, attribDef))
		return attribDef.sUniqueID;

	// all else
	return _T("");
}

BOOL CTDCCustomAttributeHelper::GetAttributeDef(TDC_ATTRIBUTE nAttribID, 
														const CTDCCustomAttribDefinitionArray& aAttribDefs,
														TDCCUSTOMATTRIBUTEDEFINITION& attribDef)
{
	if (!IsCustomAttribute(nAttribID))
		return FALSE;

	for (int nAttrib = 0; nAttrib < aAttribDefs.GetSize(); nAttrib++)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& def = aAttribDefs.GetData()[nAttrib];

		if (def.GetAttributeID() == nAttribID)
		{
			attribDef = def;
			return TRUE;
		}
	}

	// all else
	ASSERT(0);
	return FALSE;
}

BOOL CTDCCustomAttributeHelper::GetAttributeDef(const CString& sUniqueID, 
														const CTDCCustomAttribDefinitionArray& aAttribDefs,
														TDCCUSTOMATTRIBUTEDEFINITION& attribDef)
{
	int nAttrib = FindAttribute(sUniqueID, aAttribDefs);

	if (nAttrib != -1)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& def = aAttribDefs.GetData()[nAttrib];
		attribDef = def;
		return TRUE;
	}

	// all else
	ASSERT(0);
	return FALSE;
}

DWORD CTDCCustomAttributeHelper::GetAttributeDataType(const CString& sUniqueID, 
													const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	int nAttrib = FindAttribute(sUniqueID, aAttribDefs);

	if (nAttrib != -1)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& def = aAttribDefs.GetData()[nAttrib];
		return def.GetDataType();
	}

	// all else
	ASSERT(0);
	return TDCCA_STRING;
}

BOOL CTDCCustomAttributeHelper::IsColumnSortable(TDC_COLUMN nColID, 
												const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	ASSERT(IsCustomColumn(nColID));

	TDCCUSTOMATTRIBUTEDEFINITION attribDef;

	if (GetAttributeDef(nColID, aAttribDefs, attribDef))
		return attribDef.bSortable;

	// else
	return FALSE;
}

BOOL CTDCCustomAttributeHelper::IsCustomAttribute(TDC_ATTRIBUTE nAttribID)
{
	return (nAttribID >= TDCA_CUSTOMATTRIB_FIRST && nAttribID <= TDCA_CUSTOMATTRIB_LAST);
}

BOOL CTDCCustomAttributeHelper::IsCustomColumn(TDC_COLUMN nColID)
{
	return (nColID >= TDCC_CUSTOMCOLUMN_FIRST && nColID <= TDCC_CUSTOMCOLUMN_LAST);
}

BOOL CTDCCustomAttributeHelper::IsCustomControl(UINT nCtrlID)
{
	return (nCtrlID >= IDC_FIRST_CUSTOMDATAFIELD && nCtrlID <= IDC_LAST_CUSTOMDATAFIELD);
}

int CTDCCustomAttributeHelper::FindAttribute(const CString& sAttribID, 
											 const CTDCCustomAttribDefinitionArray& aAttribDefs, int nIgnore)
{
	// validate attribute type id
	ASSERT(!sAttribID.IsEmpty());

	if (sAttribID.IsEmpty())
		return -1;

	// search attribute defs for unique ID
	int nAttribDef = aAttribDefs.GetSize();

	while (nAttribDef--)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& attrib = aAttribDefs.GetData()[nAttribDef];

		if (nAttribDef != nIgnore && attrib.sUniqueID.CompareNoCase(sAttribID) == 0)
			return nAttribDef;
	}

	// not found
	return -1;
}

BOOL CTDCCustomAttributeHelper::GetAttributeDef(const CUSTOMATTRIBCTRLITEM& ctrl,
														   const CTDCCustomAttribDefinitionArray& aAttribDefs,
														   TDCCUSTOMATTRIBUTEDEFINITION& attribDef)
{
	if (!IsCustomControl(ctrl.nCtrlID))
		return FALSE;

	// search attribute defs for unique ID
	int nAttribDef = FindAttribute(ctrl.sAttribID, aAttribDefs);

	if (nAttribDef != -1)
	{
		attribDef = aAttribDefs[nAttribDef];
		return TRUE;
	}

	// all else
	return FALSE;
}
/*
CWnd* CTDCCustomAttributeHelper::GetControlFromAttributeDef(CWnd* pParent, 
															const TDCCUSTOMATTRIBUTEDEFINITION& attribDef,
															const CTDCCustomControlArray& aControls)
{
	ASSERT_VALID(pParent);

	if (!pParent || !pParent->GetSafeHwnd())
		return FALSE;

	// search for attribute type ID in aControls
	int nCtrl = aControls.GetSize();
	CUSTOMATTRIBCTRLITEM ctrl;

	while (nCtrl--)
	{
		ctrl = aControls[nCtrl];

		if (ctrl.sAttribID.CompareNoCase(attribDef.sUniqueID) == 0)
			break;
	}

	if (nCtrl != -1) // found
		return pParent->GetDlgItem(ctrl.nCtrlID);

	// all else
	return NULL;
}
*/
CString CTDCCustomAttributeHelper::GetAttributeTypeID(UINT nCtrlID, const CTDCCustomControlArray& aControls)
{
	CUSTOMATTRIBCTRLITEM ctrl;

	if (GetControl(nCtrlID, aControls, ctrl))
		return ctrl.sAttribID;

	// not found
	ASSERT(0);
	return _T("");
}

void CTDCCustomAttributeHelper::UpdateCustomAttributeControls(CWnd* pParent, CTDCCustomControlArray& aControls,
																const CTDCCustomAttribDefinitionArray& aAttribDefs,
																const CMapStringToString& mapData)
{
	int nCtrl = aControls.GetSize();
	CString sData;

	while (nCtrl--)
	{
		const CUSTOMATTRIBCTRLITEM& ctrl = aControls.GetData()[nCtrl];
		
		if (mapData.Lookup(ctrl.sAttribID, sData))
			UpdateCustomAttributeControl(pParent, ctrl, aAttribDefs, sData);
	}
}

void CTDCCustomAttributeHelper::ClearCustomAttributeControls(CWnd* pParent, CTDCCustomControlArray& aControls,
															const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	int nCtrl = aControls.GetSize();

	while (nCtrl--)
	{
		const CUSTOMATTRIBCTRLITEM& ctrl = aControls.GetData()[nCtrl];
		UpdateCustomAttributeControl(pParent, ctrl, aAttribDefs, _T(""));
	}
}

BOOL CTDCCustomAttributeHelper::GetControlData(CWnd* pParent, const CTDCCustomControlArray& aControls,
												const CTDCCustomAttribDefinitionArray& aAttribDefs,
												CMapStringToString& mapData)
{
	mapData.RemoveAll();

	int nCtrl = aControls.GetSize();

	while (nCtrl--)
	{
		const CUSTOMATTRIBCTRLITEM& ctrl = aControls.GetData()[nCtrl];
		mapData[ctrl.sAttribID] = GetControlData(pParent, ctrl, aAttribDefs);
	}

	return mapData.GetCount();
}

CString CTDCCustomAttributeHelper::GetControlData(CWnd* pParent, const CUSTOMATTRIBCTRLITEM& ctrl,
													const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	ASSERT_VALID(pParent);

	DWORD dwDataType = 0, dwListType = 0;
	VERIFY(GetControlAttributeTypes(ctrl, aAttribDefs, dwDataType, dwListType));

	CWnd* pCtrl = pParent->GetDlgItem(ctrl.nCtrlID);
	ASSERT_VALID(pCtrl);

	if (pCtrl == NULL)
		return _T("");

	TDCCADATA data;
	CString sText;
	CStringArray aItems;
	COleDateTime date;

	switch (dwListType)
	{
	case TDCCA_NOTALIST:
		switch (dwDataType)
		{
		case TDCCA_STRING:
		case TDCCA_INTEGER:
		case TDCCA_DOUBLE:
			pCtrl->GetWindowText(sText);
			data.Set(sText);
			break;
			
		case TDCCA_DATE:
			((CDateTimeCtrl*)pCtrl)->GetTime(date);
			data.Set(date);
			break;
			
			// these don't have controls
		case TDCCA_ICON:
		case TDCCA_BOOL:
			ASSERT(0);
			break;
		}
		break;
		
	case TDCCA_AUTOLIST:
		pCtrl->GetWindowText(sText);
		data.Set(sText);
		break;
		
	case TDCCA_FIXEDLIST:
		// decode icons
		if (dwDataType == TDCCA_ICON)
		{
			sText = ((CTDLIconComboBox*)pCtrl)->GetSelectedImage();
		}
		else
		{
			pCtrl->GetWindowText(sText);
		}
		data.Set(sText);
		break;
		
	case TDCCA_AUTOMULTILIST:
		((CCheckComboBox*)pCtrl)->GetChecked(aItems);
		data.Set(aItems);
		break;
		
	case TDCCA_FIXEDMULTILIST:
		((CCheckComboBox*)pCtrl)->GetChecked(aItems);
		data.Set(aItems);
		break;
	}

	return data.AsString();
}

void CTDCCustomAttributeHelper::UpdateCustomAttributeControl(CWnd* pParent, const CUSTOMATTRIBCTRLITEM& ctrl,
															  const CTDCCustomAttribDefinitionArray& aAttribDefs,
															  const CString& sData)
{
	ASSERT_VALID(pParent);

	DWORD dwDataType = 0, dwListType = 0;
	VERIFY(GetControlAttributeTypes(ctrl, aAttribDefs, dwDataType, dwListType));

	CWnd* pCtrl = pParent->GetDlgItem(ctrl.nCtrlID);
	ASSERT_VALID(pCtrl);

	if (pCtrl == NULL)
		return;

	TDCCADATA data(sData);
	CStringArray aItems;

	switch (dwListType)
	{
	case TDCCA_NOTALIST:
		{
			switch (dwDataType)
			{
			case TDCCA_STRING:
			case TDCCA_INTEGER:
			case TDCCA_DOUBLE:
				pCtrl->SetWindowText(data.AsString());
				break;
				
			case TDCCA_DATE:
				{
					CDateTimeCtrl* pDTC = (CDateTimeCtrl*)pCtrl;
					COleDateTime date = data.AsDate();

					if (date.m_dt <= 0.0)
					{
						pDTC->SetTime(COleDateTime::GetCurrentTime());
						pDTC->SendMessage(DTM_SETSYSTEMTIME, GDT_NONE, 0);
					}
					else
						pDTC->SetTime(date);
				}
				break;
				
			// these don't have controls
			case TDCCA_ICON:
			case TDCCA_BOOL:
				break;
			}
		}
		break;
		
	case TDCCA_AUTOLIST:
		{
			CAutoComboBox* pACB = (CAutoComboBox*)pCtrl;

			if (sData.IsEmpty())
				pACB->SetCurSel(-1);
			else
			{
				pACB->AddString(sData);
				pACB->SelectString(-1, sData);
			}
		}
		break;
		
	case TDCCA_FIXEDLIST:
		{
			CComboBox* pCB = (CComboBox*)pCtrl;

			if (sData.IsEmpty())
			{
				pCB->SetCurSel(-1);
			}
			else if (dwDataType == TDCCA_ICON)
			{
				((CTDLIconComboBox*)pCtrl)->SelectImage(sData);
			}
			else
				pCB->SelectString(-1, sData);
		}
		break;
		
	case TDCCA_AUTOMULTILIST:
	case TDCCA_FIXEDMULTILIST:
		{
			CCheckComboBox* pCCB = (CCheckComboBox*)pCtrl;

			data.AsArray(aItems);
			pCCB->SetChecked(aItems);
		}
		break;
	}
}

BOOL CTDCCustomAttributeHelper::GetControlAttributeTypes(const CUSTOMATTRIBCTRLITEM& ctrl,
														 const CTDCCustomAttribDefinitionArray& aAttribDefs,
														 DWORD& dwDataType, DWORD& dwListType)
{
	ASSERT(IsCustomControl(ctrl.nCtrlID));

	if (!IsCustomControl(ctrl.nCtrlID))
		return FALSE;

	// search attribute defs for unique ID
	int nAttribDef = FindAttribute(ctrl.sAttribID, aAttribDefs);

	if (nAttribDef != -1)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& attrib = aAttribDefs.GetData()[nAttribDef];
	
		dwDataType = attrib.GetDataType();
		dwListType = attrib.GetListType();
		return TRUE;
	}

	// not found
	return FALSE;
}

FIND_ATTRIBTYPE CTDCCustomAttributeHelper::GetAttributeFindType(const CString& sUniqueID, BOOL bRelativeDate,
																const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	TDC_ATTRIBUTE nAttribID = GetAttributeID(sUniqueID, aAttribDefs);
	ASSERT(nAttribID != TDCA_NONE);

	if (nAttribID == TDCA_NONE)
		return FT_NONE;

	ASSERT(IsCustomAttribute(nAttribID));

	if (!IsCustomAttribute(nAttribID))
		return FT_NONE;

	return GetAttributeFindType(nAttribID, bRelativeDate, aAttribDefs);
}

FIND_ATTRIBTYPE CTDCCustomAttributeHelper::GetAttributeFindType(TDC_ATTRIBUTE nAttribID, BOOL bRelativeDate, 
																const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	if (!IsCustomAttribute(nAttribID))
		return SEARCHPARAM::GetAttribType(nAttribID, bRelativeDate);

	TDCCUSTOMATTRIBUTEDEFINITION attribDef;
	VERIFY (GetAttributeDef(nAttribID, aAttribDefs, attribDef));

	// treat lists as strings
	if (attribDef.IsList())
		return FT_STRING;

	// else
	DWORD dwDataType = attribDef.GetDataType();

	switch (dwDataType)
	{
	case TDCCA_STRING:		return FT_STRING;
	case TDCCA_INTEGER:		return FT_INTEGER;
	case TDCCA_DOUBLE:		return FT_DOUBLE;
	case TDCCA_DATE:		return (bRelativeDate ? FT_DATE_REL : FT_DATE);
	case TDCCA_BOOL:		return FT_BOOL;
	case TDCCA_ICON:		return FT_STRING;
	}

	ASSERT(0);
	return FT_NONE;
}

int CTDCCustomAttributeHelper::AppendUniqueAttributes(const CTDCCustomAttribDefinitionArray& aAttribDefs,
														CTDCCustomAttribDefinitionArray& aMasterDefs)
{
	if (aMasterDefs.GetSize() == 0)
	{
		aMasterDefs.Copy(aAttribDefs);
	}
	else
	{
		for (int nAttrib = 0; nAttrib < aAttribDefs.GetSize(); nAttrib++)
		{
			TDCCUSTOMATTRIBUTEDEFINITION attrib = aAttribDefs[nAttrib];

			// look for duplicate attrib ID in master list
			if (FindAttribute(attrib.sUniqueID, aMasterDefs) == -1)
				aMasterDefs.Add(attrib);
			
			// else skip
		}
	}

	return aMasterDefs.GetSize();
}

int CTDCCustomAttributeHelper::CalcLongestListItem(const TDCCUSTOMATTRIBUTEDEFINITION& attribDef, CDC* pDC)
{
	ASSERT (attribDef.IsList());

	if (!attribDef.IsList())
		return 0;

	DWORD dwDataType = attribDef.GetDataType();
	int nItem = attribDef.aDefaultListData.GetSize(), nLongest = 0;

	while (nItem--)
	{
		const CString& sItem = attribDef.aDefaultListData[nItem];
		int nItemLen = 0;

		switch (dwDataType)
		{
		case TDCCA_STRING:
		case TDCCA_INTEGER:	
		case TDCCA_DOUBLE:	
		case TDCCA_DATE:	
		case TDCCA_BOOL:
			nItemLen = pDC->GetTextExtent(sItem).cx;
			break;

		case TDCCA_ICON:	
			{
				nItemLen = 20; // for the icon

				// check for trailing text
				CString sDummy, sName;

				if (attribDef.DecodeImageTag(sItem, sDummy, sName) && !sName.IsEmpty())
					nItemLen += pDC->GetTextExtent(sName).cx;
			}
			break;
		}

		nLongest = max(nLongest, nItemLen);
	}

	return nLongest;
}
