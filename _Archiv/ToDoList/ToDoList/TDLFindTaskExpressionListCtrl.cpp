// FindTaskExpressionListCtrl.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "TDLFindTaskExpressionListCtrl.h"
#include "tdcstatic.h"
#include "TDCCustomAttributeHelper.h"

#include "..\shared\HoldRedraw.h"
#include "..\shared\dialoghelper.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

enum { ATTRIB, OPERATOR, VALUE, ANDOR };
enum { ATTRIB_ID = 5000, OPERATOR_ID, ANDOR_ID, DATE_ID, TIME_ID };

const float COL_PROPORTIONS[] = { 15.0f/47, 13.0f/47, 13.0f/47, 6.0f/47 };

/////////////////////////////////////////////////////////////////////////////
// CTDLFindTaskExpressionListCtrl

CTDLFindTaskExpressionListCtrl::CTDLFindTaskExpressionListCtrl()
{
	SetAutoRowPrompt(CEnString(IDS_FP_NEW_RULE));
}

CTDLFindTaskExpressionListCtrl::~CTDLFindTaskExpressionListCtrl()
{
}

BEGIN_MESSAGE_MAP(CTDLFindTaskExpressionListCtrl, CInputListCtrl)
	//{{AFX_MSG_MAP(CTDLFindTaskExpressionListCtrl)
	ON_WM_KILLFOCUS()
	ON_WM_SIZE()
	//}}AFX_MSG_MAP
	ON_WM_CHAR()
	ON_WM_MEASUREITEM_REFLECT()
	ON_CBN_CLOSEUP(ATTRIB_ID, OnAttribEditCancel)
	ON_CBN_SELENDCANCEL(ATTRIB_ID, OnAttribEditCancel)
	ON_CBN_SELENDOK(ATTRIB_ID, OnAttribEditOK)
	ON_CBN_SELENDCANCEL(OPERATOR_ID, OnOperatorEditCancel)
	ON_CBN_SELENDOK(OPERATOR_ID, OnOperatorEditOK)
	ON_CBN_SELENDCANCEL(ANDOR_ID, OnAndOrEditCancel)
	ON_CBN_SELENDOK(ANDOR_ID, OnAndOrEditOK)
	ON_NOTIFY_REFLECT(LVN_ENDLABELEDIT, OnValueEditOK)
	ON_NOTIFY_REFLECT_EX(LVN_ITEMCHANGED, OnSelItemChanged)
	ON_NOTIFY(DTN_DATETIMECHANGE, DATE_ID, OnDateChange)
	ON_NOTIFY(DTN_CLOSEUP, DATE_ID, OnDateCloseUp)
	ON_REGISTERED_MESSAGE(WM_TEN_UNITSCHANGE, OnTimeUnitsChange)
	ON_EN_CHANGE(TIME_ID, OnTimeChange)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLFindTaskExpressionListCtrl message handlers

void CTDLFindTaskExpressionListCtrl::PreSubclassWindow() 
{
	// create child controls
	CreateControl(m_cbAttributes, ATTRIB_ID);
	CreateControl(m_cbOperators, OPERATOR_ID, FALSE);
	CreateControl(m_cbAndOr, ANDOR_ID, FALSE);
	CreateControl(m_dtDate, DATE_ID);
	CreateControl(m_eTime, TIME_ID);

	CInputListCtrl::PreSubclassWindow(); // we need combo to be created first

	// build and/or combo too
	int nItem = m_cbAndOr.AddString(CEnString(IDS_FP_AND));
	m_cbAndOr.SetItemData(nItem, TRUE);
				
	nItem = m_cbAndOr.AddString(CEnString(IDS_FP_OR));
	m_cbAndOr.SetItemData(nItem, FALSE);

	// post message for our setup
	m_header.EnableTracking(FALSE);
	ShowGrid(TRUE, TRUE);

	InsertColumn(ATTRIB, CEnString(IDS_FT_ATTRIB), LVCFMT_LEFT, 120);
	InsertColumn(OPERATOR, CEnString(IDS_FT_MATCHES), LVCFMT_LEFT, 160);
	InsertColumn(VALUE, CEnString(IDS_FT_VALUE), LVCFMT_LEFT, 130);
	InsertColumn(ANDOR, CEnString(IDS_FT_ANDOR), LVCFMT_LEFT, 60);
	SetView(LVS_REPORT);
//	SetLastColumnStretchy(TRUE);

	AutoAdd(TRUE, FALSE);

	BuildListCtrl();
	m_cbAttributes.SetCustomAttributes(m_aAttribDefs);

	SetColumnType(ATTRIB, ILCT_DROPLIST);
	SetColumnType(OPERATOR, ILCT_DROPLIST);
	SetColumnType(ANDOR, ILCT_DROPLIST);
}

void CTDLFindTaskExpressionListCtrl::SetCustomAttributes(const CTDCCustomAttribDefinitionArray& aAttribDefs)
{
	m_aAttribDefs.Copy(aAttribDefs);
	m_cbAttributes.SetCustomAttributes(m_aAttribDefs);
}

void CTDLFindTaskExpressionListCtrl::SetSearchParams(const SEARCHPARAM& param)
{
	m_aSearchParams.RemoveAll();
	m_aSearchParams.Add(param);

	if (GetSafeHwnd())
		BuildListCtrl();
}

void CTDLFindTaskExpressionListCtrl::ClearSearch()
{
	m_aSearchParams.RemoveAll();

	if (GetSafeHwnd())
		BuildListCtrl();
}

void CTDLFindTaskExpressionListCtrl::SetSearchParams(const CSearchParamArray& params)
{
	m_aSearchParams.Copy(params);

	if (GetSafeHwnd())
		BuildListCtrl();
}

int CTDLFindTaskExpressionListCtrl::GetSearchParams(CSearchParamArray& params) const
{
	params.Copy(m_aSearchParams);

	return params.GetSize();
}

void CTDLFindTaskExpressionListCtrl::MeasureItem(LPMEASUREITEMSTRUCT lpMeasureItemStruct)
{
	CRect rCombo;
	m_cbAttributes.GetWindowRect(rCombo);
	m_nItemHeight = rCombo.Height();
    
    lpMeasureItemStruct->itemHeight = m_nItemHeight - 1;
}

BOOL CTDLFindTaskExpressionListCtrl::DeleteSelectedCell()
{
	int nRow, nCol;
	GetCurSel(nRow, nCol);

	if (nRow < GetRuleCount())
	{
		if (nCol == ATTRIB)
		{
			CInputListCtrl::DeleteSelectedCell();
			m_aSearchParams.RemoveAt(nRow);

			ValidateListData();
			SetCurSel(nRow);

			return TRUE;
		}
		else if (nCol == VALUE) // clear text
		{
			SetItemText(nRow, nCol, _T(""));
			m_aSearchParams[nRow].ClearValue();
		}
	}
	
	// else
	return FALSE;
}

CWnd* CTDLFindTaskExpressionListCtrl::GetEditControl(int nItem, int nCol)
{
	if (nItem < 0 || nItem > GetRuleCount() || nCol > ANDOR)
		return NULL;

	const SEARCHPARAM& sp = m_aSearchParams[nItem];

	switch (nCol)
	{
	case ATTRIB:
		return &m_cbAttributes;

	case OPERATOR:
		if (!sp.AttributeIs(TDCA_NONE))
		{
			return &m_cbOperators;
		}
		break;

	case VALUE:
		if (sp.OperatorIs(FO_SET) || sp.OperatorIs(FO_NOT_SET))
		{
			// handled by operator
		}
		else
		{
			FIND_ATTRIBTYPE nType = sp.GetAttribType();

			switch (nType)
			{
			case FT_DATE:
				return &m_dtDate;

			case FT_TIME:
				return &m_eTime;

			case FT_BOOL:
				// do nothing: it's handled by the operator
				break;

			case FT_NONE:
				// do nothing.
				break;

			case FT_STRING:
			case FT_DATE_REL:
			default:
				return CInputListCtrl::GetEditControl();
			}
		}
		break;

	case ANDOR:
		return &m_cbAndOr;
	}

	// all else
	return NULL;
}

void CTDLFindTaskExpressionListCtrl::EditCell(int nItem, int nCol)
{
	// handle new rules
	if (nItem == GetRuleCount() && nCol == ATTRIB)
		AddRule();

	CWnd* pEdit = GetEditControl(nItem, nCol);
	ASSERT (pEdit);

	if (!pEdit)
		return;

	const SEARCHPARAM& sp = m_aSearchParams[nItem];

	switch (nCol)
	{
	case ATTRIB:
	case OPERATOR:
	case ANDOR:
		ShowControl(*pEdit, nItem, nCol);
		break;

	case VALUE:
		if (sp.GetOperator() != FO_SET && sp.GetOperator() != FO_NOT_SET)
		{
			switch (sp.GetAttribType())
			{
			case FT_DATE:
			case FT_TIME:
				ShowControl(*pEdit, nItem, nCol);
				break;

			default:
				PrepareEdit(nItem, nCol);
				CInputListCtrl::EditCell(nItem, nCol);
			}
		}
		break;
	}
}

IL_COLUMNTYPE CTDLFindTaskExpressionListCtrl::GetCellType(int nRow, int nCol) const
{
	if (nRow < 0 || nRow >= GetRuleCount() || nCol > ANDOR)
	{
		return CInputListCtrl::GetCellType(nRow, nCol);
	}

	const SEARCHPARAM& sp = m_aSearchParams[nRow];

	switch (nCol)
	{
	case ATTRIB:
		return ILCT_DROPLIST;

	case OPERATOR:
		if (!sp.AttributeIs(TDCA_NONE))
		{
			return ILCT_DROPLIST;
		}
		break;

	case VALUE:
		if (sp.OperatorIs(FO_SET) || sp.OperatorIs(FO_NOT_SET))
		{
			// handled by operator
		}
		else
		{
			switch (sp.GetAttribType())
			{
			case FT_DATE:
				return ILCT_DATE;

			case FT_TIME:
				// do nothing.
				break;

			case FT_BOOL:
				// do nothing: it's handled by the operator
				break;

			case FT_NONE:
				// do nothing.
				break;
			}
		}
		break;

	case ANDOR:
		return ILCT_DROPLIST;
	}

	// all else
	return CInputListCtrl::GetCellType(nRow, nCol);
}

BOOL CTDLFindTaskExpressionListCtrl::IsEditing() const
{
	return CInputListCtrl::IsEditing() ||
			m_cbOperators.IsWindowVisible() ||
			m_cbAndOr.IsWindowVisible() ||
			m_cbAttributes.IsWindowVisible() ||
			m_dtDate.IsWindowVisible() ||
			m_eTime.IsWindowVisible();
}

BOOL CTDLFindTaskExpressionListCtrl::CanEditSelectedCell() const
{
	int nRow, nCol;
	GetCurSel(nRow, nCol);

	if (nRow < m_aSearchParams.GetSize())
	{
		const SEARCHPARAM& rule = m_aSearchParams[nRow];

		if (nCol == VALUE)
		{
			// Boolean type is a special case
			if (rule.GetAttribType() == FT_BOOL)
				return FALSE;

			// as is using the set/notset operators
			if (rule.OperatorIs(FO_SET) || rule.OperatorIs(FO_NOT_SET))
				return FALSE;
		}
	}

	// else
	return CInputListCtrl::CanEditSelectedCell();
}

void CTDLFindTaskExpressionListCtrl::PrepareEdit(int nRow, int /*nCol*/)
{
	const SEARCHPARAM& sp = m_aSearchParams[nRow];

	switch (sp.GetAttribType())
	{
	case FT_STRING:
		m_editBox.SetMask(_T(""));
		break;
		
	case FT_INTEGER:
		m_editBox.SetMask(_T("1234567890"));
		break;
		
	case FT_DATE_REL:
		m_editBox.SetMask(_T("tT+-1234567890dDwWmMyY"));
		break;
		
	case FT_DOUBLE:
		m_editBox.SetMask(_T("1234567890."), ME_LOCALIZEDECIMAL);
		break;

	case FT_DATE:
	case FT_BOOL:
	case FT_NONE:
	default:
		ASSERT(0);
	}
}

BOOL CTDLFindTaskExpressionListCtrl::AddRule()
{
	SEARCHPARAM sp(TDCA_TASKNAMEORCOMMENTS, FO_INCLUDES);

	int nRow = InsertRule(GetRuleCount(), sp);

	// make sure the 'and/or' text of the preceding rule is set
	if (nRow > 0)
	{
		SEARCHPARAM& spPrev = m_aSearchParams[nRow - 1];
		CEnString sAndOr(spPrev.GetAnd() ? IDS_FP_AND : IDS_FP_OR);
		SetItemText(nRow - 1, ANDOR, sAndOr);
	}

	SetCurSel(nRow, ATTRIB);
	EnsureVisible(nRow, FALSE);

	return TRUE;
}

BOOL CTDLFindTaskExpressionListCtrl::DeleteSelectedRule()
{
	int nRow = GetCurSel();

	if (nRow != -1 && CanDeleteSelectedCell())
	{
		DeleteItem(nRow);
		m_aSearchParams.RemoveAt(nRow);

		ValidateListData();

		SetCurSel(nRow);
		EnsureVisible(nRow, FALSE);

		return TRUE;
	}

	return FALSE;
}

void CTDLFindTaskExpressionListCtrl::MoveSelectedRuleUp()
{
	if (CanMoveSelectedRuleUp())
	{
		int nRow, nCol;
		GetCurSel(nRow, nCol);

		// save off rule
		SEARCHPARAM sp = m_aSearchParams[nRow];

		// delete rule
		m_aSearchParams.RemoveAt(nRow);
		DeleteItem(nRow);

		// reinsert rule
		nRow = InsertRule(nRow - 1, sp);

		// sanity check
		ValidateListData();

		// restore selection
		SetCurSel(nRow, nCol);
		EnsureVisible(nRow, FALSE);
	}
}

int CTDLFindTaskExpressionListCtrl::InsertRule(int nRow, const SEARCHPARAM& sp)
{
	m_aSearchParams.InsertAt(nRow, sp);

	CString sItem = m_cbAttributes.GetAttributeName(sp);
	int nNew = InsertItem(nRow, sItem);

	SetItemText(nNew, OPERATOR, GetOpName(sp.GetOperator()));

	UpdateValueColumnText(nRow);
	
	// omit and/or for last rule
	if (nRow < GetRuleCount() - 1)
	{
		CEnString sAndOr(sp.GetAnd() ? IDS_FP_AND : IDS_FP_OR);
		SetItemText(nNew, ANDOR, sAndOr);
	}

	return nNew;
}

BOOL CTDLFindTaskExpressionListCtrl::CanMoveSelectedRuleUp() const
{
	int nRow = GetCurSel();

	return (nRow > 0 && nRow < GetRuleCount());
}

void CTDLFindTaskExpressionListCtrl::MoveSelectedRuleDown()
{
	if (CanMoveSelectedRuleDown())
	{
		int nRow, nCol;
		GetCurSel(nRow, nCol);

		// save off rule
		SEARCHPARAM sp = m_aSearchParams[nRow];

		// delete rule
		m_aSearchParams.RemoveAt(nRow);
		DeleteItem(nRow);

		// reinsert rule
		nRow = InsertRule(nRow + 1, sp);
	
		// sanity check
		ValidateListData();

		// restore selection
		SetCurSel(nRow, nCol);
		EnsureVisible(nRow, FALSE);
	}
}

BOOL CTDLFindTaskExpressionListCtrl::CanMoveSelectedRuleDown() const
{
	return (GetCurSel() < GetRuleCount() - 1);
}

void CTDLFindTaskExpressionListCtrl::PrepareControl(CWnd& ctrl, int nRow, int nCol)
{
	UNREFERENCED_PARAMETER(ctrl);

	if (!GetRuleCount())
		return;

	SEARCHPARAM& sp = m_aSearchParams[nRow];

	switch (nCol)
	{
	case ATTRIB:
		{
			ASSERT (&ctrl == &m_cbAttributes);
	
			m_cbAttributes.SelectAttribute(sp);
			CDialogHelper::RefreshMaxDropWidth(m_cbAttributes);
		}
		break;

	case OPERATOR:
		{
			ASSERT (&ctrl == &m_cbOperators);

			m_cbOperators.ResetContent();
			
			FIND_ATTRIBTYPE nType = sp.GetAttribType();
			
			switch (nType)
			{
			case FT_STRING:
				AddOperatorToCombo(FO_SET);
				AddOperatorToCombo(FO_NOT_SET);
				AddOperatorToCombo(FO_EQUALS);
				AddOperatorToCombo(FO_NOT_EQUALS);
				AddOperatorToCombo(FO_INCLUDES);
				AddOperatorToCombo(FO_NOT_INCLUDES);
				break;

			case FT_INTEGER:
			case FT_DOUBLE:
			case FT_TIME:
				AddOperatorToCombo(FO_SET);
				AddOperatorToCombo(FO_NOT_SET);
				AddOperatorToCombo(FO_EQUALS);
				AddOperatorToCombo(FO_NOT_EQUALS);
				AddOperatorToCombo(FO_GREATER);
				AddOperatorToCombo(FO_GREATER_OR_EQUAL);
				AddOperatorToCombo(FO_LESS);
				AddOperatorToCombo(FO_LESS_OR_EQUAL);
				break;

			case FT_DATE:
			case FT_DATE_REL:
				AddOperatorToCombo(FO_SET);
				AddOperatorToCombo(FO_NOT_SET);
				AddOperatorToCombo(FO_EQUALS);
				AddOperatorToCombo(FO_NOT_EQUALS);
				AddOperatorToCombo(FO_AFTER);
				AddOperatorToCombo(FO_ON_OR_AFTER);
				AddOperatorToCombo(FO_BEFORE);
				AddOperatorToCombo(FO_ON_OR_BEFORE);
				break;

			case FT_BOOL:
				AddOperatorToCombo(FO_SET);
				AddOperatorToCombo(FO_NOT_SET);
				break;
			}
	
			CDialogHelper::SelectItemByData(m_cbOperators, (DWORD)sp.GetOperator());
		}
		break;

	case ANDOR:
		{
			ASSERT (&ctrl == &m_cbAndOr);

			if (sp.GetAnd())
				m_cbAndOr.SelectString(-1, CEnString(IDS_FP_AND));
			else
				m_cbAndOr.SelectString(-1, CEnString(IDS_FP_OR));
		}
		break;

	case VALUE:
		switch (sp.GetAttribType())
		{
		case FT_DATE:
			ASSERT (&ctrl == &m_dtDate);

			// if the rule does not yet have a date then set it now to
			// the current date because that's whats the date ctrl will default to
			if (sp.ValueAsDate().m_dt <= 0)
			{
				sp.SetValue(COleDateTime::GetCurrentTime());
				SetItemText(nRow, nCol, sp.ValueAsDate().Format(VAR_DATEVALUEONLY));
			}
			else
				m_dtDate.SetTime(sp.ValueAsDouble());
			break;

		case FT_TIME:
			ASSERT (&ctrl == &m_eTime);
			m_eTime.SetTime(sp.ValueAsDouble(), (TCHAR)sp.GetFlags());
			break;
		}
		break;

	}
}

void CTDLFindTaskExpressionListCtrl::AddOperatorToCombo(FIND_OPERATOR nOp)
{
	int i = m_cbOperators.AddString(GetOpName(nOp)); 
	m_cbOperators.SetItemData(i, (DWORD)nOp); 
}

void CTDLFindTaskExpressionListCtrl::ValidateListData() const
{
#ifdef _DEBUG
	for (int nRule = 0; nRule < GetRuleCount(); nRule++)
	{
		const SEARCHPARAM& rule = m_aSearchParams[nRule];

		// check matching attribute text 
		CString sRuleAttrib = m_cbAttributes.GetAttributeName(rule);
		CString sListAttrib = GetItemText(nRule, ATTRIB);
		ASSERT (sRuleAttrib == sListAttrib);

		// check matching operator text 
		CString sRuleOp = GetOpName(rule.GetOperator());
		CString sListOp = GetItemText(nRule, OPERATOR);
		ASSERT (sListOp.IsEmpty() || sRuleOp == sListOp);

		// check valid operator
		ASSERT(rule.HasValidOperator());
	}
#endif
}

void CTDLFindTaskExpressionListCtrl::OnAttribEditCancel()
{
	HideControl(m_cbAttributes);
}

void CTDLFindTaskExpressionListCtrl::OnAttribEditOK()
{
	HideControl(m_cbAttributes);

	// update item text and keep data store synched
	int nRow = GetCurSel();

	if (nRow != CB_ERR)
	{
		SEARCHPARAM& sp = m_aSearchParams[nRow];
		SEARCHPARAM spNew;

		if (m_cbAttributes.GetSelectedAttribute(spNew))
		{
			sp = spNew;

			// update list text
			CString sAttrib = m_cbAttributes.GetSelectedAttributeText();
			SetItemText(nRow, ATTRIB, sAttrib);

			// clear the operator cell text if the operator was no longer valid
			if (sp.OperatorIs(FO_NONE))
				SetItemText(nRow, OPERATOR, _T(""));

			UpdateValueColumnText(nRow);
			ValidateListData();
		}
	}
}

BOOL CTDLFindTaskExpressionListCtrl::OnSelItemChanged(NMHDR* /*pNMHDR*/, LRESULT* pResult) 
{
//	NM_LISTVIEW* pNMListView = (NM_LISTVIEW*)pNMHDR;
//	int nRow = pNMListView->iItem;

	// always make sure we hide the datetime ctrl and the time edit
	HideControl(m_dtDate);
	HideControl(m_eTime);

	*pResult = 0;
	
	return FALSE; // continue routing
}

void CTDLFindTaskExpressionListCtrl::OnValueEditOK(NMHDR* pNMHDR, LRESULT* pResult) 
{
	LV_DISPINFO* pDispInfo = (LV_DISPINFO*)pNMHDR;
	
	int nRow = pDispInfo->item.iItem;
	int nCol = pDispInfo->item.iSubItem;

	ASSERT (nCol == VALUE);

	SEARCHPARAM& sp = m_aSearchParams[nRow];

	switch (sp.GetAttribType())
	{
	case FT_STRING:
	case FT_DATE_REL:
	case FT_INTEGER:
	case FT_DOUBLE:
		sp.SetValue(pDispInfo->item.pszText);
		break;
		
	case FT_DATE:
	case FT_BOOL:
		// not handled here
		break;
	}
		
	*pResult = 0;
}

void CTDLFindTaskExpressionListCtrl::OnOperatorEditCancel()
{
	HideControl(m_cbOperators);
}

void CTDLFindTaskExpressionListCtrl::OnOperatorEditOK()
{
	HideControl(m_cbOperators);

	// update operator type
	int nSel = m_cbOperators.GetCurSel();

	if (nSel != CB_ERR)
	{
		CString sSel;
		m_cbOperators.GetLBText(nSel, sSel);

		int nRow = GetCurSel();
		SetItemText(nRow, OPERATOR, sSel);

		// keep data store synched
		SEARCHPARAM& rule = m_aSearchParams[nRow];
		FIND_OPERATOR nNewOp = (FIND_OPERATOR)m_cbOperators.GetItemData(nSel);

		rule.SetOperator(nNewOp);

		// if the op is set/notset then clear the field
		if (nNewOp == FO_SET || nNewOp == FO_NOT_SET)
		{
			rule.ClearValue();
		}

		UpdateValueColumnText(nRow);
		ValidateListData();
	}
}

void CTDLFindTaskExpressionListCtrl::OnAndOrEditCancel()
{
	HideControl(m_cbAndOr);
}

void CTDLFindTaskExpressionListCtrl::OnAndOrEditOK()
{
	HideControl(m_cbAndOr);

	// update operator type
	int nSel = m_cbAndOr.GetCurSel();

	if (nSel != CB_ERR)
	{
		CString sSel;
		m_cbAndOr.GetLBText(nSel, sSel);

		int nRow = GetCurSel();
		SetItemText(nRow, ANDOR, sSel);

		// keep data store synched
		m_aSearchParams[nRow].SetAnd(m_cbAndOr.GetItemData(nSel));

		ValidateListData();
	}
}

void CTDLFindTaskExpressionListCtrl::BuildListCtrl()
{
	DeleteAllItems();

	for (int nParam = 0; nParam < GetRuleCount(); nParam++)
	{
		const SEARCHPARAM& sp = m_aSearchParams[nParam];

		// attrib
		CString sAttrib = m_cbAttributes.GetAttributeName(sp);
		int nItem = InsertItem(nParam, sAttrib);

		// operator
		CString sOp = GetOpName(sp.GetOperator());
		SetItemText(nItem, OPERATOR, sOp);

		// value
		UpdateValueColumnText(nItem);

		// and/or (but not for last row)
		if (nParam < GetRuleCount() - 1)
		{
			CEnString sAndOr(sp.GetAnd() ? IDS_FP_AND : IDS_FP_OR);
			SetItemText(nItem, ANDOR, sAndOr);
		}
	}

	ValidateListData();
	SetCurSel(0);
}

void CTDLFindTaskExpressionListCtrl::UpdateValueColumnText(int nRow)
{
	ASSERT(nRow >= 0 && nRow < m_aSearchParams.GetSize());
	ASSERT(m_aSearchParams.GetSize() >= GetItemCount() - 1);

	if (nRow < 0 || nRow >= m_aSearchParams.GetSize())
		return;
	
	if (m_aSearchParams.GetSize() < GetItemCount() - 1)
		return;

	const SEARCHPARAM& sp = m_aSearchParams[nRow];
	CString sValue;
		
	// value (but not boolean)
	if (sp.GetOperator() != FO_SET && sp.GetOperator() != FO_NOT_SET)
	{
		try
		{
			switch (sp.GetAttribType())
			{
			case FT_STRING:
			case FT_DATE_REL:
			case FT_INTEGER:
			case FT_DOUBLE:
				sValue = sp.ValueAsString();
				break;
				
			case FT_DATE:
				sValue = sp.ValueAsDate().Format(VAR_DATEVALUEONLY);
				break;
				
			case FT_TIME:
				sValue = CTimeHelper().FormatTime(sp.ValueAsDouble(), sp.GetFlags(), 2);
				break;
				
			case FT_BOOL:
				// handled by operator
				break;
			}
		}
		catch (...)
		{
			// bad value but we continue
			sValue.Empty();
		}
	}
			
	SetItemText(nRow, VALUE, sValue);
}

CString CTDLFindTaskExpressionListCtrl::GetOpName(FIND_OPERATOR op)
{
	int nOp = OP_COUNT;

	while (nOp--)
	{
		if (OPERATORS[nOp].op == op)
		{
			if (OPERATORS[nOp].nOpResID)
				return CEnString(OPERATORS[nOp].nOpResID);
			else
				return _T("");
		}
	}

	ASSERT (0); // not found
	return _T("");
}

void CTDLFindTaskExpressionListCtrl::OnDateChange(NMHDR* pNMHDR, LRESULT* pResult)
{
	LPNMDATETIMECHANGE pDTHDR = (LPNMDATETIMECHANGE)pNMHDR;
	COleDateTime dt(pDTHDR->st);

	// update the rule 
	int nRow = GetCurSel();

	m_aSearchParams[nRow].SetValue(dt);
	UpdateValueColumnText(nRow);
	
	*pResult = 0;
}

void CTDLFindTaskExpressionListCtrl::OnDateCloseUp(NMHDR* /*pNMHDR*/, LRESULT* pResult)
{
//	LPNMDATETIMECHANGE pDTHDR = (LPNMDATETIMECHANGE)pNMHDR;

	HideControl(m_dtDate);
	
	*pResult = 0;
}


void CTDLFindTaskExpressionListCtrl::OnKillFocus(CWnd* pNewWnd) 
{
	CInputListCtrl::OnKillFocus(pNewWnd);
	
	if (pNewWnd != &m_dtDate)
		HideControl(m_dtDate);

	if (pNewWnd != &m_eTime)
		HideControl(m_eTime);
}

void CTDLFindTaskExpressionListCtrl::OnTimeChange()
{
	// update the rule 
	int nRow = GetCurSel();
	SEARCHPARAM& rule = m_aSearchParams[nRow];

	rule.SetValue(m_eTime.GetTime());
	rule.SetFlags(m_eTime.GetUnits());

	UpdateValueColumnText(nRow);
}

LRESULT CTDLFindTaskExpressionListCtrl::OnTimeUnitsChange(WPARAM /*wp*/, LPARAM /*lp*/)
{
	// update the rule 
	int nRow = GetCurSel();
	SEARCHPARAM& rule = m_aSearchParams[nRow];

	rule.SetFlags(m_eTime.GetUnits());

	UpdateValueColumnText(nRow);

	return 0L;
}

void CTDLFindTaskExpressionListCtrl::OnSize(UINT nType, int cx, int cy) 
{
	CInputListCtrl::OnSize(nType, cx, cy);
	
	// resize columns by proportion
	SetColumnWidth(ATTRIB, (int)(cx * COL_PROPORTIONS[ATTRIB]));
	SetColumnWidth(OPERATOR, (int)(cx * COL_PROPORTIONS[OPERATOR]));
	SetColumnWidth(VALUE, (int)(cx * COL_PROPORTIONS[VALUE]));
	SetColumnWidth(ANDOR, (int)(cx * COL_PROPORTIONS[ANDOR]));
}

void CTDLFindTaskExpressionListCtrl::OnChar(UINT nChar, UINT nRepCnt, UINT nFlags) 
{
	int nItem, nCol;
	GetCurSel(nItem, nCol);
	
	if (GetRuleCount() > 0 || nCol == ATTRIB)
	{
		// if the user typed an alphanumeric char then begin editing automatically
		// numeric keys only work for value column
		if (isalpha(nChar) || (nCol == VALUE && isdigit(nChar)))
		{
			EditCell(nItem, nCol);

			// forward key down on to edit control
			CWnd* pEdit = GetEditControl(nItem, nCol);

			if (pEdit)
			{
				pEdit->PostMessage(WM_CHAR, nChar, MAKELPARAM(nRepCnt, nFlags));
				return; // eat it
			}
		}
	}
	
	CInputListCtrl::OnKeyDown(nChar, nRepCnt, nFlags);
}

BOOL CTDLFindTaskExpressionListCtrl::PreTranslateMessage(MSG* pMsg) 
{
	if (pMsg->message == WM_KEYDOWN)
	{
		int nSel = GetCurSel();
		
		if (nSel != -1)
		{
			// if the focus is on the attrib list and the user hits the spacebar
			// then toggle the enabled state
			switch (pMsg->wParam)
			{
			case VK_F2:
				EditSelectedCell();
				break;

			case VK_DELETE:
				DeleteSelectedRule();
				break;

			case VK_UP:
				if (Misc::ModKeysArePressed(MKS_CTRL))
				{
					MoveSelectedRuleUp();
					return TRUE; // eat it
				}
				break;

			case VK_DOWN:
				if (Misc::ModKeysArePressed(MKS_CTRL))
				{
					MoveSelectedRuleDown();
					return TRUE; // eat it
				}
				break;
			}
		}
	}
	
	return CInputListCtrl::PreTranslateMessage(pMsg);
}

