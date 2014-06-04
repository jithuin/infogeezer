// MSWordHelper.cpp: implementation of the CMSWordHelper class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "MSWordHelper.h"
#include "filemisc.h"
#include "fileregister.h"
#include "regkey.h"
#include "misc.h"

#include "..\3rdparty\msword.h"

#include <shlwapi.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

using namespace WordAPI;

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

WordAPI::_Application* CMSWordHelper::s_pWord = NULL;
int CMSWordHelper::s_nRefCount = 0;

// some handy const variants
static COleVariant varNull(_T("")), varZero((short)0), varOne((short)1);
static COleVariant varFalse((short)FALSE), varTrue((short)TRUE);

#define RPC_NOT_AVAIL 0x800706BA

CMSWordHelper::CMSWordHelper(int nMinVersion)
{
	if (s_pWord == NULL && IsWordInstalled(nMinVersion))
	{
		ASSERT(s_nRefCount == 0);
		RestartWord();
	}

	// ref count even on failure
	s_nRefCount++;
}

CMSWordHelper::~CMSWordHelper()
{
	s_nRefCount--;

	if (s_nRefCount == 0 && s_pWord)
	{
		try
		{
			s_pWord->Quit(varZero,   // SaveChanges
						  varZero,   // OriginalFormat
						  varFalse); // RouteDocument
		}
		catch(...)
		{
			// do nothing
		}

		delete s_pWord;
		s_pWord = NULL;
	}
}

BOOL CMSWordHelper::IsWordInstalled(int nMinVersion)
{
	if (CFileRegister::IsRegisteredApp(_T("docx"), _T("WINWORD.EXE"), TRUE))
	{
		return (GetInstalledWordVersion() >= nMinVersion);
	}

	// else
	return FALSE;
}

int CMSWordHelper::GetInstalledWordVersion()
{
	CRegKey reg;
	
	if (reg.Open(HKEY_CLASSES_ROOT, _T("Word.Application\\CurVer")) == ERROR_SUCCESS)
	{
		CString sCurVer;
		reg.Read(_T(""), sCurVer);
		
		int nLastDot = sCurVer.ReverseFind('.');
		
		if (nLastDot != -1)
		{
			CString sVersion = sCurVer.Mid(nLastDot + 1);
			
			if (Misc::IsNumber(sVersion))
				return _ttoi(sVersion);
		}
	}

	// all else
	return -1;
}

BOOL CMSWordHelper::RestartWord()
{
	if (s_pWord == NULL)
	{
		// first time
		s_pWord = new WordAPI::_Application;
	}
	else
	{
		// detach the interface without Releasing since we are
		// assuming that the interface is cactus
		s_pWord->DetachDispatch();
	}

	// try to restart word
	if (!s_pWord->CreateDispatch(_T("Word.Application")))
	{
		delete s_pWord;
		s_pWord = NULL;

		return FALSE;
	}

	// else all's well
	return TRUE;
}

BOOL CMSWordHelper::ConvertContent(const CString& sInput, WdOpenFormat nInputFormat, 
								   CString& sOutput, WdSaveFormat nOutputFormat, 
									WordAPI::MsoEncoding nEncoding)
{
	if (sInput.IsEmpty())
	{
		sOutput.Empty();
		return TRUE;
	}

	if (!IsValid())
		return FALSE;

	CString sInputFilename = FileMisc::GetTempFileName(_T("temp"));
	CString sOutputFilename = FileMisc::GetTempFileName(_T("temp"));
	
	// rtf must be saved as multibyte
	SFE_SAVEAS nSaveAs = (nInputFormat == wdOpenFormatRTF) ? SFE_MULTIBYTE : SFE_ASCOMPILED;

	if (FileMisc::SaveFile(sInputFilename, sInput, nSaveAs))
	{
		::DeleteFile(sOutputFilename);

		if (ConvertFile(sInputFilename, nInputFormat, sOutputFilename, nOutputFormat, nEncoding))
		{
			FileMisc::LoadFile(sOutputFilename, sOutput);
		}
	}

	// cleanup
	::DeleteFile(sInputFilename);
	::DeleteFile(sOutputFilename);

	return !sOutput.IsEmpty();
}

BOOL CMSWordHelper::ConvertFile(const CString& sInputFilename, WdOpenFormat nInputFormat, 
								const CString& sOutputFilename, WdSaveFormat nOutputFormat,
								MsoEncoding nEncoding)
{
	if (!IsValid())
		return FALSE;

	if (!FileMisc::FileExists(sInputFilename))
		return FALSE;

	// caller is responsible for ensuring output file does not exist
	if (FileMisc::FileExists(sOutputFilename))
		return FALSE;

	// get document object first. This is a great way to validate
	// that the Word object is still operating.
	Documents docs;

	try
	{
		docs.AttachDispatch(s_pWord->GetDocuments());
	}
	catch (COleException *e) 
	{ 
		SCODE sc = e->m_sc;
		e->Delete(); 

		// assume someone has forcibly closed word from under us
		if (sc == RPC_NOT_AVAIL)
		{
			// try to restart word
			if (!RestartWord())
				return FALSE;
			else
				docs.AttachDispatch(s_pWord->GetDocuments());
		}
	}
	catch(...)
	{
		return FALSE;
	}

	LPDISPATCH lpd = NULL;

	try 
	{
		lpd = docs.Open(COleVariant(sInputFilename),		// FileName
						varFalse,							// ConfirmConversions
						varFalse,							// ReadOnly
						varFalse,							// AddToRecentFiles
						varNull,							// PasswordDocument
						varNull,							// PasswordTemplate
						varTrue,							// Revert
						varNull,							// WritePasswordDocument
						varNull,							// WritePasswordTemplate
						COleVariant((long)nInputFormat),	// Format
						COleVariant((long)nEncoding),		// Encoding
						varFalse,							// Visible
						varFalse,							// OpenAndRepair
						varZero,							// DocumentDirection
						varTrue,							// NoEncodingDialog
						varNull);							// XMLTransform
		
	}
	catch (...)
	{
		// do nothing
	}

	if (lpd == NULL)
		return FALSE;

	_Document doc(lpd);
	BOOL bSuccess = TRUE;

	try 
	{
		doc.SaveAs(COleVariant(sOutputFilename),		// FileName
					COleVariant((short)nOutputFormat),	// FileFormat
					varFalse,							// LockComments
					varNull,							// Password
					varFalse,							// AddToRecentFiles
					varNull,							// WritePassword
					varFalse,							// ReadOnlyRecommended
					varFalse,							// EmbedTrueTypeFonts
					varFalse,							// SaveNativePictureFormat
					varFalse,							// SaveFormsData
					varFalse,							// SaveAsAOCELetter
					COleVariant((long)nEncoding),		// Encoding
					varFalse,							// InsertLineBreaks
					varFalse,							// AllowSubstitutions
					varOne,								// LineEnding
					varFalse);							// AddBiDiMarks
	}
	catch (...)
	{
		bSuccess = FALSE;
	}

	// cleanup
	doc.Close(varZero,   // SaveChanges
			  varZero,   // OriginalFormat
			  varFalse); // RouteDocument

	return bSuccess;
}
