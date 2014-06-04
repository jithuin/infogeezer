#if !defined(AFX_TDLICONCOMBOBOX_H__0DAA207C_F8BC_4BDD_ADCB_E840D2C9A73F__INCLUDED_)
#define AFX_TDLICONCOMBOBOX_H__0DAA207C_F8BC_4BDD_ADCB_E840D2C9A73F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// tdliconcombobox.h : header file
//

#include "..\shared\ownerdrawcomboboxbase.h"

/////////////////////////////////////////////////////////////////////////////
// CTDLIconComboBox window

class CTDCImageList;

class CTDLIconComboBox : public COwnerdrawComboBoxBase
{
// Construction
public:
	CTDLIconComboBox(const CTDCImageList& ilImages);

	int SelectImage(const CString& sImage);
	CString GetSelectedImage() const;

// Attributes
protected:
	const CTDCImageList& m_ilImages;

// Operations
public:

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CTDLIconComboBox)
	//}}AFX_VIRTUAL

// Implementation
public:
	virtual ~CTDLIconComboBox();

	// Generated message map functions
protected:
	//{{AFX_MSG(CTDLIconComboBox)
		// NOTE - the ClassWizard will add and remove member functions here.
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()

protected:
	virtual void DrawItemText(CDC& dc, const CRect& rect, int nItem, UINT nItemState,
								DWORD dwItemData, const CString& sItem, BOOL bList);	

};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TDLICONCOMBOBOX_H__0DAA207C_F8BC_4BDD_ADCB_E840D2C9A73F__INCLUDED_)
