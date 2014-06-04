// TDLCsvAttributeSetupListCtrl.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLCsvAttributeSetupListCtrl.h"
#include "tdcstatic.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

enum { IMPORT_COLUMNNAME, IMPORT_COLUMNID };
enum { EXPORT_COLUMNID, EXPORT_COLUMNNAME };

enum { ATTRIB_ID = 5000 };
const int COL_WIDTH = 200;

/////////////////////////////////////////////////////////////////////////////
// CTDLCsvAttributeSetupListCtrl

CTDLCsvAttributeSetupListCtrl::CTDLCsvAttributeSetupListCtrl(BOOL bImporting, BOOL bOneToOneMapping)
 : m_bImporting(bImporting), m_bOneToOneMapping(bOneToOneMapping)
{
}

CTDLCsvAttributeSetupListCtrl::~CTDLCsvAttributeSetupListCtrl()
{
}


BEGIN_MESSAGE_MAP(CTDLCsvAttributeSetupListCtrl, CInputListCtrl)
	//{{AFX_MSG_MAP(CTDLCsvAttributeSetupListCtrl)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
	ON_WM_MEASUREITEM_REFLECT()
	ON_CBN_CLOSEUP(ATTRIB_ID, OnAttribEditCancel)
	ON_CBN_SELENDCANCEL(ATTRIB_ID, OnAttribEditCancel)
	ON_CBN_SELENDOK(ATTRIB_ID, OnAttribEditOK)
	ON_NOTIFY_REFLECT(LVN_ENDLABELEDIT, OnNameEditOK)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLCsvAttributeSetupListCtrl message handlers

void CTDLCsvAttributeSetupListCtrl::SetColumnMapping(const CTDCCsvColumnMapping& aMapping)
{
	m_aMapping.Copy(aMapping);

	if (GetSafeHwnd())
		BuildListCtrl();
}

int CTDLCsvAttributeSetupListCtrl::GetColumnMapping(CTDCCsvColumnMapping& aMapping) const
{
	aMapping.Copy(m_aMapping);
	return aMapping.GetSize();
}

void CTDLCsvAttributeSetupListCtrl::PreSubclassWindow() 
{
	// create child controls
	CreateControl(m_cbAttributes, ATTRIB_ID);

	CInputListCtrl::PreSubclassWindow(); // we need combo to be created first

	// build column combo because that is static
	for (int nAttrib = 0; nAttrib < ATTRIB_COUNT; nAttrib++)
	{
		const TDCATTRIBUTE& att = ATTRIBUTES[nAttrib];

		// ignore special attributes
		if (att.attrib == TDCA_COLOR || att.attrib == TDCA_PROJNAME ||
			att.attrib == TDCA_RECURRENCE || att.attrib == TDCA_POSITION||
			att.attrib == TDCA_TASKNAMEORCOMMENTS || att.attrib == TDCA_ANYTEXTATTRIBUTE)
			continue;

		int nItem = m_cbAttributes.AddString(CEnString(att.nAttribResID)); 
		m_cbAttributes.SetItemData(nItem, (DWORD)att.attrib); 
	}

	m_header.EnableTracking(FALSE);
	ShowGrid(TRUE, TRUE);

	if (m_bImporting)
	{
		InsertColumn(IMPORT_COLUMNNAME, CEnString(IDS_CSV_COLUMNNAME), LVCFMT_LEFT, COL_WIDTH);
		InsertColumn(IMPORT_COLUMNID, CEnString(IDS_CSV_MAPSTOATTRIBUTE), LVCFMT_LEFT, COL_WIDTH);

		SetColumnType(IMPORT_COLUMNNAME, ILCT_TEXT);
		DisableColumnEditing(IMPORT_COLUMNNAME, TRUE);
		SetColumnType(IMPORT_COLUMNID, ILCT_DROPLIST);
	}
	else // export
	{
		InsertColumn(EXPORT_COLUMNID, CEnString(IDS_CSV_ATTRIBUTE), LVCFMT_LEFT, COL_WIDTH);
		InsertColumn(EXPORT_COLUMNNAME, CEnString(IDS_CSV_MAPSTOCOLUMNNAME), LVCFMT_LEFT, COL_WIDTH);

		SetColumnType(EXPORT_COLUMNID, ILCT_TEXT);
		DisableColumnEditing(EXPORT_COLUMNID, TRUE);
		SetColumnType(EXPORT_COLUMNNAME, ILCT_TEXT);
	}
	SetView(LVS_REPORT);
//	SetLastAttributeStretchy(TRUE);

	AutoAdd(FALSE, FALSE);

	BuildListCtrl();
}

void CTDLCsvAttributeSetupListCtrl::BuildListCtrl()
{
	DeleteAllItems();

	if (m_bImporting)
	{
		for (int nRow = 0; nRow < m_aMapping.GetSize(); nRow++)
		{
			int nItem = InsertItem(GetItemCount(), m_aMapping[nRow].sColumnName);
			SetItemText(nItem, IMPORT_COLUMNID, GetAttributeName(m_aMapping[nRow].nTDCAttrib));
			SetItemData(nItem, m_aMapping[nRow].nTDCAttrib);
		}
	}
	else // export
	{
		for (int nRow = 0; nRow < m_aMapping.GetSize(); nRow++)
		{
			int nItem = InsertItem(GetItemCount(), GetAttributeName(m_aMapping[nRow].nTDCAttrib));
			SetItemText(nItem, EXPORT_COLUMNNAME, m_aMapping[nRow].sColumnName);
			SetItemData(nItem, m_aMapping[nRow].nTDCAttrib);
		}
	}

	if (GetItemCount())
		SetCurSel(0);
}

// static helper
CString CTDLCsvAttributeSetupListCtrl::GetAttributeName(TDC_ATTRIBUTE nAtt)
{
	for (int nAttrib = 0; nAttrib < ATTRIB_COUNT; nAttrib++)
	{
		const TDCATTRIBUTE& att = ATTRIBUTES[nAttrib];

		if (nAtt == att.attrib)
			return CEnString(att.nAttribResID);
	}

	return _T("");
}

int CTDLCsvAttributeSetupListCtrl::FindMappedAttribute(TDC_ATTRIBUTE nAtt, int nIgnoreRow) const
{
	for (int nRow = 0; nRow < GetItemCount(); nRow++)
	{
		if (nRow == nIgnoreRow)
			continue;

		if (nAtt == (TDC_ATTRIBUTE)GetItemData(nRow))
			return nRow;
	}

	return -1;
}

int CTDLCsvAttributeSetupListCtrl::FindMappedAttribute(const CString& sName, int nIgnoreRow) const
{
	for (int nRow = 0; nRow < GetItemCount(); nRow++)
	{
		if (nRow == nIgnoreRow)
			continue;

		if (m_bImporting)
		{
			if (GetItemText(nRow, IMPORT_COLUMNNAME).CompareNoCase(sName) == 0)
				return nRow;
		}
		else // exporting
		{
			if (GetItemText(nRow, EXPORT_COLUMNNAME).CompareNoCase(sName) == 0)
				return nRow;
		}
	}

	return -1;
}

void CTDLCsvAttributeSetupListCtrl::EditCell(int nItem, int nCol)
{
	if ((m_bImporting && nCol == IMPORT_COLUMNID) ||
		(!m_bImporting && nCol == EXPORT_COLUMNID))
	{
		PrepareEdit(nItem, nCol);
		ShowControl(m_cbAttributes, nItem, nCol); 
	}
	else
		CInputListCtrl::EditCell(nItem, nCol);
}

BOOL CTDLCsvAttributeSetupListCtrl::IsEditing() const 
{
	return CInputListCtrl::IsEditing() ||
			m_cbAttributes.IsWindowVisible();
}

BOOL CTDLCsvAttributeSetupListCtrl::CanEditSelectedCell() const 
{
	return CInputListCtrl::CanEditSelectedCell();
}

void CTDLCsvAttributeSetupListCtrl::PrepareEdit(int nRow, int nCol)
{
	if (m_bImporting && nCol == IMPORT_COLUMNID)
	{
		TDC_ATTRIBUTE nAttrib = (TDC_ATTRIBUTE)GetItemData(nRow);
		CString sAttrib = GetAttributeName(nAttrib);

		int nFind = m_cbAttributes.FindStringExact(0, sAttrib);

		if (nFind == -1)
			nFind = 0;

		m_cbAttributes.SetCurSel(nFind);
	}		
}

BOOL CTDLCsvAttributeSetupListCtrl::CanDeleteSelectedCell() const
{
	int nRow, nCol;
	GetCurSel(nRow, nCol);

	return ((m_bImporting && nCol == IMPORT_COLUMNID) ||
			(!m_bImporting && nCol == EXPORT_COLUMNNAME));
}

BOOL CTDLCsvAttributeSetupListCtrl::DeleteSelectedCell()
{
	int nRow, nCol;
	GetCurSel(nRow, nCol);

	if ((m_bImporting && nCol == IMPORT_COLUMNID) ||
		(!m_bImporting && nCol == EXPORT_COLUMNNAME))
	{
		if (m_bImporting)
		{
			m_aMapping[nRow].nTDCAttrib = TDCA_NONE;
			SetItemData(nRow, (DWORD)TDCA_NONE);
		}
		else // exporting
			m_aMapping[nRow].sColumnName.Empty();

		return CInputListCtrl::DeleteSelectedCell();
	}
	
	// else
	return FALSE;
}

void CTDLCsvAttributeSetupListCtrl::MeasureItem(LPMEASUREITEMSTRUCT lpMeasureItemStruct)
{
	CRect rCombo;
	m_cbAttributes.GetWindowRect(rCombo);
	m_nItemHeight = rCombo.Height();
    
    lpMeasureItemStruct->itemHeight = m_nItemHeight - 1; 
}

void CTDLCsvAttributeSetupListCtrl::OnAttribEditCancel()
{
	HideControl(m_cbAttributes);
}

void CTDLCsvAttributeSetupListCtrl::OnAttribEditOK()
{
	ASSERT (m_bImporting);

	HideControl(m_cbAttributes);

	// update attribute type
	int nSel = m_cbAttributes.GetCurSel();
	int nRow = GetCurSel(); // list selection

	if (nSel != CB_ERR)
	{
		CString sSel;

		TDC_ATTRIBUTE nAttrib = (TDC_ATTRIBUTE)m_cbAttributes.GetItemData(nSel);
		m_cbAttributes.GetLBText(nSel, sSel);

		// check that this column ID is not already in use
		if (m_bOneToOneMapping)
		{
			int nExist = FindMappedAttribute(nAttrib, nRow);

			if (nExist != -1)
			{
				// clear it
				SetItemText(nExist, IMPORT_COLUMNID, _T(""));
				m_aMapping[nExist].nTDCAttrib = TDCA_NONE;
				SetItemData(nExist, (DWORD)TDCA_NONE);
			}
		}

		SetItemText(nRow, IMPORT_COLUMNID, sSel);
		m_aMapping[nRow].nTDCAttrib = nAttrib;
		SetItemData(nRow, nAttrib);
	}
}

void CTDLCsvAttributeSetupListCtrl::OnNameEditOK(NMHDR* /*pNMHDR*/, LRESULT* pResult)
{
	ASSERT (!m_bImporting);

	// update column name
	int nRow = GetCurSel(); // list selection
	CString sSel = GetItemText(nRow, EXPORT_COLUMNNAME);

	// check that this column name is not already in use
	if (m_bOneToOneMapping)
	{
		int nExist = FindMappedAttribute(sSel, nRow);
		
		if (nExist != -1)
		{
			// clear it
			SetItemText(nExist, EXPORT_COLUMNNAME, _T(""));
			m_aMapping[nExist].sColumnName.Empty();
		}
	}	

	SetItemText(nRow, EXPORT_COLUMNNAME, sSel);
	m_aMapping[nRow].sColumnName = sSel;

	*pResult = 0;
}

