// FilterOptionComboBox.cpp : implementation file
//

#include "stdafx.h"
#include "todolist.h"
#include "TDLFilterOptionComboBox.h"
#include "tdcstatic.h"

#include "..\shared\misc.h"
#include "..\Shared\enstring.h"
#include "..\Shared\localizer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CFilterOptionComboBox

CTDLFilterOptionComboBox::CTDLFilterOptionComboBox() : CEnCheckComboBox(TRUE, 0, IDS_TDC_NONE), m_dwOptions((DWORD)-1)
{
}

CTDLFilterOptionComboBox::~CTDLFilterOptionComboBox()
{
}


BEGIN_MESSAGE_MAP(CTDLFilterOptionComboBox, CEnCheckComboBox)
	//{{AFX_MSG_MAP(CFilterOptionComboBox)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CFilterOptionComboBox message handlers

void CTDLFilterOptionComboBox::Initialize(DWORD dwFlags, DWORD dwOptions)
{
	// translation done via CEnString
	CLocalizer::EnableTranslation(*this, FALSE);

	ResetContent();
	m_dwOptions = (DWORD)-1;

	CEnString sFlag;

	for (int nItem = 0; nItem < NUM_FILTEROPT; nItem++)
	{
		UINT nFlag = FILTER_OPTIONS[nItem][1];

		if (Misc::HasFlag(dwFlags, nFlag))
		{
			sFlag.LoadString(FILTER_OPTIONS[nItem][0]);
			
			int nIndex = AddString(sFlag);
			
			if (nIndex != CB_ERR)
			{
				m_mapItemFlag[sFlag] = nFlag; // map item to enum

				// is the item checked?
				SetCheck(nIndex, Misc::HasFlag(dwOptions, nFlag));
			}
		}
	}
}

void CTDLFilterOptionComboBox::Initialize(const FTDCFILTER& filter, FTC_VIEW /*nView*/, BOOL bWantHideParents)
{
	// translation done via CEnString
	CLocalizer::EnableTranslation(GetSafeHwnd(), FALSE);

	ResetContent();
	m_dwOptions = (DWORD)-1;

	// add flags to droplist depending on the filter being used
	CEnString sFlag;

	for (int nItem = 0; nItem < NUM_FILTEROPT; nItem++)
	{
		UINT nFlag = FILTER_OPTIONS[nItem][1];
		BOOL bAddFlag = FALSE;

		switch (nFlag)
		{
		case FO_ANYCATEGORY:
		case FO_ANYALLOCTO:
		case FO_ANYTAG:
			bAddFlag = (filter.nShow != FS_CUSTOM && filter.nShow != FS_SELECTED);
			break;

		case FO_SHOWALLSUB:
			bAddFlag = TRUE;
			break;

		case FO_HIDEPARENTS:
			bAddFlag = bWantHideParents;
			break;

		case FO_HIDECOLLAPSED:
			bAddFlag = (filter.nShow != FS_SELECTED);
			break;

		case FO_HIDEOVERDUE:
			bAddFlag = (filter.nDueBy == FD_TODAY) ||
						(filter.nDueBy == FD_TOMORROW) ||
						(filter.nDueBy == FD_ENDTHISWEEK) || 
						(filter.nDueBy == FD_ENDNEXTWEEK) || 
						(filter.nDueBy == FD_ENDTHISMONTH) ||
						(filter.nDueBy == FD_ENDNEXTMONTH) ||
						(filter.nDueBy == FD_ENDTHISYEAR) ||
						(filter.nDueBy == FD_NEXTSEVENDAYS) ||
						(filter.nDueBy == FD_ENDNEXTYEAR);
			break;

		case FO_HIDEDONE:
			bAddFlag = (filter.nShow != FS_SELECTED && filter.nShow != FS_NOTDONE && filter.nShow != FS_DONE);
			break;

		default:
			ASSERT(0);
			break;		
		}	

		if (bAddFlag)
		{
			sFlag.LoadString(FILTER_OPTIONS[nItem][0]);
			
			if (AddString(sFlag) != CB_ERR)
				m_mapItemFlag[sFlag] = nFlag; // map item to enum
		}
	}

	// set states
	SetOptions(filter.dwFlags);
}

void CTDLFilterOptionComboBox::OnCheckChange(int /*nIndex*/)
{
	// update m_dwOptions
	m_dwOptions = 0;

	for (int nItem = 0; nItem < GetCount(); nItem++)
	{
		if (GetCheck(nItem))
		{
			// get flag for item
			UINT nFlag = 0;
			CString sFlag;

			GetLBText(nItem, sFlag);
			VERIFY(m_mapItemFlag.Lookup(sFlag, nFlag));

			// aggregate
			m_dwOptions |= nFlag;
		}
	}
}

void CTDLFilterOptionComboBox::SetOptions(DWORD dwOptions)
{
	ASSERT(GetSafeHwnd());

	if (dwOptions == m_dwOptions)
		return;

	for (int nItem = 0; nItem < GetCount(); nItem++)
	{
		// get flag for item
		UINT nFlag = 0;
		CString sFlag;

		GetLBText(nItem, sFlag);
		VERIFY(m_mapItemFlag.Lookup(sFlag, nFlag));

		// set state
		SetCheck(nItem, Misc::HasFlag(dwOptions, nFlag));
	}

	m_dwOptions = dwOptions;
}

DWORD CTDLFilterOptionComboBox::GetOptions() const 
{
	return m_dwOptions;
}

