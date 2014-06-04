// RichEditSpellCheck.h: interface for the CRichEditSpellCheck class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_RICHEDITSPELLCHECK_H__9A6FC513_71CB_4207_9FED_7B1429010FE5__INCLUDED_)
#define AFX_RICHEDITSPELLCHECK_H__9A6FC513_71CB_4207_9FED_7B1429010FE5__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "ISpellCheck.h"

class CRichEditBaseCtrl;

class CRichEditSpellCheck : public ISpellCheck  
{
public:
	CRichEditSpellCheck(CRichEditBaseCtrl& re);
	virtual ~CRichEditSpellCheck();

	LPCTSTR GetFirstWord() const;
	LPCTSTR GetNextWord() const;
	LPCTSTR GetCurrentWord() const;

	void SelectCurrentWord();
	void ReplaceCurrentWord(LPCTSTR szWord);
	
	void ClearSelection();

protected:
	CRichEditBaseCtrl& m_re;
	mutable CHARRANGE m_crCurrentWord;
	mutable CString m_sCurrentWord;

protected:
	BOOL GetWord(const CHARRANGE& cr, CString& sWord) const;
};

#endif // !defined(AFX_RICHEDITSPELLCHECK_H__9A6FC513_71CB_4207_9FED_7B1429010FE5__INCLUDED_)
