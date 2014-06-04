// TDLMultiSortDlg.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLMultiSortDlg.h"
#include "tdcstatic.h"

#include "..\shared\enstring.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDLMultiSortDlg dialog

CTDLMultiSortDlg::CTDLMultiSortDlg(const TDSORTCOLUMNS& sort, const CTDCColumnIDArray& aVisibleColumns, 
								   const CTDCCustomAttribDefinitionArray& aAttribDefs, CWnd* pParent /*=NULL*/)
	: CDialog(CTDLMultiSortDlg::IDD, pParent), 
		m_nSortBy1(sort.nBy1), 
		m_nSortBy2(sort.nBy2), 
		m_nSortBy3(sort.nBy3), 
		m_bAscending1(sort.bAscending1), 
		m_bAscending2(sort.bAscending2), 
		m_bAscending3(sort.bAscending3),
		m_aVisibleColumns(aVisibleColumns),
		m_aAttribDefs(aAttribDefs)
{
	//{{AFX_DATA_INIT(CTDLMultiSortDlg)
	//}}AFX_DATA_INIT
}


void CTDLMultiSortDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CTDLMultiSortDlg)
	DDX_Control(pDX, IDC_SORTBY3, m_cbSortBy3);
	DDX_Control(pDX, IDC_SORTBY2, m_cbSortBy2);
	DDX_Control(pDX, IDC_SORTBY1, m_cbSortBy1);
	DDX_Check(pDX, IDC_ASCENDING1, m_bAscending1);
	DDX_Check(pDX, IDC_ASCENDING2, m_bAscending2);
	DDX_Check(pDX, IDC_ASCENDING3, m_bAscending3);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CTDLMultiSortDlg, CDialog)
	//{{AFX_MSG_MAP(CTDLMultiSortDlg)
	ON_CBN_SELCHANGE(IDC_SORTBY1, OnSelchangeSortby1)
	ON_CBN_SELCHANGE(IDC_SORTBY2, OnSelchangeSortby2)
	ON_CBN_SELCHANGE(IDC_SORTBY3, OnSelchangeSortby3)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLMultiSortDlg message handlers

BOOL CTDLMultiSortDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	BuildCombos();

	// disable 3rd combo if 2nd combo not set
	m_cbSortBy3.EnableWindow(m_nSortBy2 != TDCC_NONE);
	
	GetDlgItem(IDC_ASCENDING3)->EnableWindow(m_nSortBy2 != TDCC_NONE && m_nSortBy3 != TDCC_NONE);
	GetDlgItem(IDC_ASCENDING2)->EnableWindow(m_nSortBy2 != TDCC_NONE);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CTDLMultiSortDlg::BuildCombos()
{
	CComboBox* COMBOS[3] = { &m_cbSortBy1, &m_cbSortBy2, &m_cbSortBy3 };
	const TDC_COLUMN* SORTBY[3] = { &m_nSortBy1, &m_nSortBy2, &m_nSortBy3 };

	int nCol;
	for (nCol = 0; nCol < NUM_LATEST_COLUMNS; nCol++)
	{
		TDCCOLUMN& col = COLUMNS[nCol];
		TDC_COLUMN nColID = col.nColID;

		if (IsColumnVisible(nColID))
		{
			CEnString sColumn(col.nIDLongName);
			
			for (int nSort = 0; nSort < 3; nSort++)
			{
				int nIndex = COMBOS[nSort]->AddString(sColumn);
				COMBOS[nSort]->SetItemData(nIndex, nColID);
				
				if (*(SORTBY[nSort]) == nColID)
					COMBOS[nSort]->SetCurSel(nIndex);
			}
		}
	}

	// custom columns
	for (nCol = 0; nCol < m_aAttribDefs.GetSize(); nCol++)
	{
		const TDCCUSTOMATTRIBUTEDEFINITION& attribDef = m_aAttribDefs.GetData()[nCol];

		if (attribDef.bSortable)
		{
			TDC_COLUMN nColID = attribDef.GetColumnID();
			CEnString sColumn(IDS_CUSTOMCOLUMN, attribDef.sLabel);

			for (int nSort = 0; nSort < 3; nSort++)
			{
				int nIndex = COMBOS[nSort]->AddString(sColumn);
				COMBOS[nSort]->SetItemData(nIndex, nColID);
				
				if (*(SORTBY[nSort]) == nColID)
					COMBOS[nSort]->SetCurSel(nIndex);
			}
		}
	}

	// add blank item at top of 2nd and 3rd combo
	for (int nSort = 1; nSort < 3; nSort++)
	{
		int nIndex = COMBOS[nSort]->InsertString(0, _T(""));
		COMBOS[nSort]->SetItemData(nIndex, TDCC_NONE);
		
		if (*(SORTBY[nSort]) == TDCC_NONE)
			COMBOS[nSort]->SetCurSel(nIndex);
	}

	// set selection to first item if first combo selection is not set
	if (m_cbSortBy1.GetCurSel() == -1)
	{
		m_cbSortBy1.SetCurSel(0);
		m_nSortBy1 = (TDC_COLUMN)m_cbSortBy1.GetItemData(0);
	}
}

BOOL CTDLMultiSortDlg::IsColumnVisible(TDC_COLUMN col) const
{
	// special cases:
	if (col == TDCC_CLIENT)
		return TRUE;

	else if (col == TDCC_NONE)
		return FALSE;

	// else test column
	int nCol = m_aVisibleColumns.GetSize();

	while (nCol--)
	{
		if (col == m_aVisibleColumns[nCol])
			return TRUE;
	}

	// not found
	return FALSE;
}

void CTDLMultiSortDlg::OnSelchangeSortby1() 
{
	UpdateData();
	
	int nSel = m_cbSortBy1.GetCurSel();
	m_nSortBy1 = (TDC_COLUMN)m_cbSortBy1.GetItemData(nSel);
}

void CTDLMultiSortDlg::OnSelchangeSortby2() 
{
	UpdateData();
		
	int nSel = m_cbSortBy2.GetCurSel();
	m_nSortBy2 = (TDC_COLUMN)m_cbSortBy2.GetItemData(nSel);

	GetDlgItem(IDC_ASCENDING2)->EnableWindow(m_nSortBy2 != TDCC_NONE);

	// disable 3rd combo if 2nd combo not set
	m_cbSortBy3.EnableWindow(m_nSortBy2 != TDCC_NONE);
	GetDlgItem(IDC_ASCENDING3)->EnableWindow(m_nSortBy2 != TDCC_NONE && m_nSortBy3 != TDCC_NONE);
}

void CTDLMultiSortDlg::OnSelchangeSortby3() 
{
	UpdateData();
		
	int nSel = m_cbSortBy3.GetCurSel();
	m_nSortBy3 = (TDC_COLUMN)m_cbSortBy3.GetItemData(nSel);

	GetDlgItem(IDC_ASCENDING3)->EnableWindow(m_nSortBy2 != TDCC_NONE && m_nSortBy3 != TDCC_NONE);
}

void CTDLMultiSortDlg::GetSortBy(TDSORTCOLUMNS& sort) const
{
	sort.nBy1 = m_nSortBy1;
	sort.bAscending1 = m_bAscending1;

	sort.nBy2 = m_nSortBy2;

	// if nBy2 is not set then make sure nBy3 isn't set
	if (sort.nBy2 != TDCC_NONE)
	{
		sort.bAscending2 = m_bAscending2;
		sort.nBy3 = m_nSortBy3;
		sort.bAscending3 = m_bAscending3;
	}
	else
		sort.nBy3 = TDCC_NONE;
}

