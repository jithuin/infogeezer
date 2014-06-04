// WeekdayComboBox.cpp : implementation file
//

#include "stdafx.h"
#include "weekdaycombobox.h"
#include "datehelper.h"
#include "localizer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CWeekdayComboBox

CWeekdayComboBox::CWeekdayComboBox()
{
}

CWeekdayComboBox::~CWeekdayComboBox()
{
}


BEGIN_MESSAGE_MAP(CWeekdayComboBox, CComboBox)
	//{{AFX_MSG_MAP(CWeekdayComboBox)
	ON_WM_CREATE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CWeekdayComboBox message handlers

void CWeekdayComboBox::PreSubclassWindow() 
{
	InitCombo();
	
	CComboBox::PreSubclassWindow();
}

int CWeekdayComboBox::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CComboBox::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	InitCombo();
	
	return 0;
}

void CWeekdayComboBox::InitCombo()
{
	ASSERT(GetSafeHwnd());

	CLocalizer::EnableTranslation(*this, FALSE);

	ResetContent();

	for (int nDOW = 1; nDOW <= 7; nDOW++)
		AddString(CDateHelper::GetWeekdayName(nDOW, FALSE));
}
