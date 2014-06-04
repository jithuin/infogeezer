// RichEditSpellCheck.cpp: implementation of the CRichEditSpellCheck class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "RichEditSpellCheck.h"
#include "RichEditBaseCtrl.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

const CString DELIMS(" \t\r\n.,:;-/?<>|~!@#$%^&*()+=");

CRichEditSpellCheck::CRichEditSpellCheck(CRichEditBaseCtrl& re) : m_re(re)
{
	m_crCurrentWord.cpMin = m_crCurrentWord.cpMax = 0;
}

CRichEditSpellCheck::~CRichEditSpellCheck()
{

}

LPCTSTR CRichEditSpellCheck::GetFirstWord() const
{
	m_crCurrentWord.cpMin = m_crCurrentWord.cpMax = -1;

	return GetNextWord();
}

LPCTSTR CRichEditSpellCheck::GetNextWord() const
{
	CHARRANGE cr;

	cr.cpMin = m_re.SendMessage(EM_FINDWORDBREAK, WB_RIGHT, m_crCurrentWord.cpMax);
	cr.cpMax = m_re.SendMessage(EM_FINDWORDBREAK, WB_RIGHTBREAK, cr.cpMin + 1);

	GetWord(cr, m_sCurrentWord);
	int nLength = m_sCurrentWord.GetLength();

	if (nLength)
	{
		m_crCurrentWord = cr;

		// if there's any trailing whitespace then trim it off
		m_sCurrentWord.TrimRight(DELIMS);

		// and update char range
		m_crCurrentWord.cpMax -= nLength - m_sCurrentWord.GetLength();
		nLength = m_sCurrentWord.GetLength();

		// if there's any leading whitespace then trim it off
		m_sCurrentWord.TrimLeft(DELIMS);

		// and update char range
		m_crCurrentWord.cpMin += nLength - m_sCurrentWord.GetLength();
		nLength = m_sCurrentWord.GetLength();

		// if there was some text but it was all whitespace, return
		// a non-empty string so that searching is not terminated
		// and move the selection to the end of the whitespace
		if (m_sCurrentWord.IsEmpty())
		{
			m_crCurrentWord.cpMin = m_crCurrentWord.cpMax = cr.cpMax;
			m_sCurrentWord = " ";
		}
	}

	return m_sCurrentWord;
}

LPCTSTR CRichEditSpellCheck::GetCurrentWord() const
{
	return m_sCurrentWord;
}

BOOL CRichEditSpellCheck::GetWord(const CHARRANGE& cr, CString& sWord) const
{
	if (cr.cpMax > cr.cpMin)
		sWord = m_re.GetTextRange(cr);
	else
		sWord.Empty();

	// else
	return !sWord.IsEmpty();
}

void CRichEditSpellCheck::SelectCurrentWord()
{
	m_re.SetSel(m_crCurrentWord);
	
	// need to make sure line is visible
	CPoint ptSel = m_re.GetCharPos(m_crCurrentWord.cpMax);
	
	CRect rClient;
	m_re.GetClientRect(rClient);
	
	if (ptSel.y >= rClient.bottom)
	{
		while (ptSel.y >= rClient.bottom)
		{
			m_re.LineScroll(1);
			ptSel = m_re.GetCharPos(m_crCurrentWord.cpMax);
		}
		
		// one more for good measure
		m_re.LineScroll(1);
	}
	else if (ptSel.y < rClient.top)
	{

		while (ptSel.y < rClient.top)
		{
			m_re.LineScroll(-1);
			ptSel = m_re.GetCharPos(m_crCurrentWord.cpMax);
		}
		
		// one more for good measure
		m_re.LineScroll(-1);
	}
}

void CRichEditSpellCheck::ReplaceCurrentWord(LPCTSTR szWord)
{
	m_re.SetSel(m_crCurrentWord);
	m_re.ReplaceSel(szWord, TRUE);
}

void CRichEditSpellCheck::ClearSelection()
{
	m_crCurrentWord.cpMin = m_crCurrentWord.cpMax;
	m_re.SetSel(m_crCurrentWord);
}
