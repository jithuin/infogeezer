// tdliconcombobox.cpp : implementation file
//

#include "stdafx.h"
#include "tdliconcombobox.h"
#include "tdcimagelist.h"
#include "tdcstruct.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTDLIconComboBox

CTDLIconComboBox::CTDLIconComboBox(const CTDCImageList& ilImages)
	:
	m_ilImages(ilImages)
{
}

CTDLIconComboBox::~CTDLIconComboBox()
{
}


BEGIN_MESSAGE_MAP(CTDLIconComboBox, COwnerdrawComboBoxBase)
	//{{AFX_MSG_MAP(CTDLIconComboBox)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTDLIconComboBox message handlers

void CTDLIconComboBox::DrawItemText(CDC& dc, const CRect& rect, int nItem, UINT nItemState,
									DWORD dwItemData, const CString& sItem, BOOL bList)
{
	CString sImage, sName;
	
	if (TDCCUSTOMATTRIBUTEDEFINITION::DecodeImageTag(sItem, sImage, sName))
	{
		// draw image
		if (m_ilImages.GetSafeHandle() && !sImage.IsEmpty())
		{
			CPoint pt = rect.TopLeft();
			pt.y--;

			m_ilImages.Draw(&dc, sImage, pt, ILD_TRANSPARENT);
		}

		// draw optional text
		if (!sName.IsEmpty())
		{
			CRect rText(rect);
			rText.left += 18;

			COwnerdrawComboBoxBase::DrawItemText(dc, rText, nItem, nItemState, dwItemData, sName, bList);
		}

		return;
	}

	// all else
	COwnerdrawComboBoxBase::DrawItemText(dc, rect, nItem, nItemState, dwItemData, sItem, bList);
}

int CTDLIconComboBox::SelectImage(const CString& sImage)
{
	CString sPartial = TDCCUSTOMATTRIBUTEDEFINITION::EncodeImageTag(sImage, _T(""));
	return SelectString(-1, sPartial);
}

CString CTDLIconComboBox::GetSelectedImage() const
{
	CString sItem, sImage, sName;
	GetWindowText(sItem);

	TDCCUSTOMATTRIBUTEDEFINITION::DecodeImageTag(sItem, sImage, sName);
	return sImage;
}
