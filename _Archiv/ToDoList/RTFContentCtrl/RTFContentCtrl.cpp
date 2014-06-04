// RTFContentCtrl.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include "RTFContentCtrl.h"
#include "RTFContentControl.h"

#include "..\3rdparty\compression.h"
#include "..\3rdparty\stdiofileex.h"

#include "..\shared\filemisc.h"
#include "..\shared\misc.h"
#include "..\shared\mswordhelper.h"
#include "..\shared\localizer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

// from r2h.h
typedef int (*PFNCONVERTRTF2HTML)(const char*, const char*, unsigned long, const char*);
const CString RTF2HTML_FNAME(_T("Rtf2Html"));

/*
static AFX_EXTENSION_MODULE AfxdllDLL = { NULL, NULL };

extern "C" int APIENTRY
DllMain(HINSTANCE hInstance, DWORD dwReason, LPVOID lpReserved)
{
	// Remove this if you use lpReserved
	UNREFERENCED_PARAMETER(lpReserved);

	if (dwReason == DLL_PROCESS_ATTACH)
	{
		TRACE0("AFXDLL.DLL Initializing!\n");
		
		// Extension DLL one-time initialization
		if (!AfxInitExtensionModule(AfxdllDLL, hInstance))
			return 0;

		// Insert this DLL into the resource chain
		// NOTE: If this Extension DLL is being implicitly linked to by
		//  an MFC Regular DLL (such as an ActiveX Control)
		//  instead of an MFC application, then you will want to
		//  remove this line from DllMain and put it in a separate
		//  function exported from this Extension DLL.  The Regular DLL
		//  that uses this Extension DLL should then explicitly call that
		//  function to initialize this Extension DLL.  Otherwise,
		//  the CDynLinkLibrary object will not be attached to the
		//  Regular DLL's resource chain, and serious problems will
		//  result.

		new CDynLinkLibrary(AfxdllDLL);
	}
	else if (dwReason == DLL_PROCESS_DETACH)
	{
		TRACE0("AFXDLL.DLL Terminating!\n");
		// Terminate the library before destructors are called
		AfxTermExtensionModule(AfxdllDLL);
	}
	return 1;   // ok
}
*/

static CRTFContentCtrlApp theApp;

DLL_DECLSPEC IContent* CreateContentInterface()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return &theApp;
//	return new CRTFContentCtrlApp;
}

DLL_DECLSPEC int GetInterfaceVersion() 
{ 
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return ICONTENTCTRL_VERSION; 
}

CRTFContentCtrlApp::CRTFContentCtrlApp() : m_pWordHelper(NULL)
{
}

LPCTSTR CRTFContentCtrlApp::GetTypeID() const
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	static CString sID;

	Misc::GuidToString(RTF_TYPEID, sID); 

	return sID;
}

LPCTSTR CRTFContentCtrlApp::GetTypeDescription() const
{
	return _T("Rich Text");
}

void CRTFContentCtrlApp::SetLocalizer(ITransText* pTT)
{
	CLocalizer::Initialize(pTT);
}

IContentControl* CRTFContentCtrlApp::CreateCtrl(unsigned short nCtrlID, unsigned long nStyle, 
						long nLeft, long nTop, long nWidth, long nHeight, HWND hwndParent)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CRTFContentControl* pControl = new CRTFContentControl;

	if (pControl)
	{
		CRect rect(nLeft, nTop, nLeft + nWidth, nTop + nHeight);

		if (pControl->Create(nStyle, rect, CWnd::FromHandle(hwndParent), nCtrlID, TRUE))
		{
			return pControl;
		}
	}

	delete pControl;
	return NULL;
}

void CRTFContentCtrlApp::Release()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	delete m_pWordHelper;
	m_pWordHelper = NULL;

	CleanupTemporaryImages();
}

void CRTFContentCtrlApp::CleanupTemporaryImages()
{
	int nFolder = m_aTempImageFolders.GetSize();

	while (nFolder--)
	{
		const CString& sFolder = m_aTempImageFolders[nFolder];

		if (FileMisc::RemoveFolder(sFolder))
			m_aTempImageFolders.RemoveAt(nFolder);
	}
}

int CRTFContentCtrlApp::ConvertToHtml(const unsigned char* pContent, int nLength, 
									  LPCTSTR szCharSet, LPTSTR& szHtml, LPCTSTR szImageDir)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if (nLength == 0)
		return 0; // nothing to convert

	// we may have to decompress it first
	unsigned char* pDecompressed = NULL;

	if (strncmp((const char*)pContent, RTFTAG, LENTAG) != 0)
	{
		int nLenDecompressed = 0;

		if (Compression::Decompress(pContent, nLength, pDecompressed, nLenDecompressed))
		{
			pContent = pDecompressed;
			nLength = nLenDecompressed;
		}
		else
			return 0;
	}

	// save rich text content to file
	CString sHtml, sRTF((const char*)pContent, nLength);
	nLength = 0; // reuse for resultant html length

	// doesn't matter where the rtf file goes.
	CString sTempRtf = FileMisc::GetTempFileName(RTF2HTML_FNAME, _T("rtf")), sTempHtml;
	FileMisc::SaveFile(sTempRtf, sRTF, SFE_MULTIBYTE); // rtf must be saved as Ansi

	CString sImageDir(szImageDir), sUniqueDir;
	BOOL bTempImageDir = sImageDir.IsEmpty();

	if (bTempImageDir)
		sImageDir = FileMisc::GetTempFolder();

	// create a unique image folder every time this is called for this session
	static int nImgCount = 1;

	FileMisc::TerminatePath(sImageDir);
	sUniqueDir.Format(_T("%simg%d"), sImageDir, nImgCount++);
	FileMisc::CreateFolder(sUniqueDir);

	// track temp image folders for later cleanup
	if (bTempImageDir)
		m_aTempImageFolders.Add(sUniqueDir);

	// try MS Word first
	// initialize first time
	if (m_pWordHelper == NULL)
		m_pWordHelper = new CMSWordHelper(12);

	if (m_pWordHelper->IsValid())
	{
		// html file must be placed inside the unique folder
		sTempHtml.Format(_T("%s\\%s.html"), sUniqueDir, RTF2HTML_FNAME);

		// prepare file system for Word else it will spit
		::DeleteFile(sTempHtml);

		BOOL bSuccess = m_pWordHelper->ConvertFile(sTempRtf, WordAPI::wdOpenFormatRTF, 
												  sTempHtml, WordAPI::wdSaveFormatFilteredHTML);

		if (bSuccess &&  FileMisc::LoadFile(sTempHtml, sHtml) && !sHtml.IsEmpty())
		{
			// strip everything but the body
			int nStart = sHtml.Find(_T("<body"));
			ASSERT(nStart >= 0);

			// find next end tag
			nStart = sHtml.Find('>', nStart);
			ASSERT(nStart >= 0);

			int nEnd = sHtml.Find(_T("</body>"));
			ASSERT(nEnd > nStart);

			sHtml = sHtml.Mid(nStart + 1, nEnd - nStart - 1);

			// replace the image file paths with our 
			CString sSrcFolderName = RTF2HTML_FNAME + '_';
				
			CString sDstFolderName = sUniqueDir;
			FileMisc::TerminatePath(sDstFolderName);
			sDstFolderName += sSrcFolderName;
			sDstFolderName.Replace('\\', '/');

			sHtml.Replace(sSrcFolderName, sDstFolderName);
			nLength = sHtml.GetLength();
		}
	}

	// if that failed try our best converter
	if (nLength == 0)
	{
		CString sRtf2HtmlPath = FileMisc::GetAppFolder() + _T("\\rtf2htmlbridge.dll");
		static HMODULE hMod = LoadLibrary(sRtf2HtmlPath);

		if (hMod)
		{
			typedef int (*PFNCONVERTRTF2HTML)(LPCTSTR, LPCTSTR, LPCTSTR, 
												LPCTSTR, LPCTSTR, LPCTSTR, 
												LPCTSTR, LPCTSTR, LPCTSTR, 
												LPCTSTR, LPCTSTR);
			
			try
			{
				PFNCONVERTRTF2HTML fnRtf2Html = (PFNCONVERTRTF2HTML)GetProcAddress(hMod, "fnRtf2Html");
				
				if (fnRtf2Html)
				{
					// arguments
					CString sCharSet = _T("/CS:") + CString(szCharSet);

					sImageDir.Format(_T("/ID:%s"), sUniqueDir);
					sTempHtml = FileMisc::GetTempFileName(_T("Rtf2Html"), _T("html"));

					nLength = 0;
					BOOL bSuccess = fnRtf2Html(sTempRtf,						// rtf input file
												FileMisc::GetTempFolder(),  // folder for html output file
												sImageDir,					// folder for saving images
												_T("/IT:png"),				// image file format
												_T("/DS:content"),			// return only body of html
												sCharSet,					// character set
												_T(""),						// unused
												_T(""),						// unused 
												_T(""),						// unused 
												_T(""),						// unused 
												_T(""));					// unused

					if (bSuccess && FileMisc::LoadFile(sTempHtml, sHtml) && !sHtml.IsEmpty())
					{
						nLength = sHtml.GetLength();
					}
				}
			}
			catch (...)
			{
				// nLength = 0
			}
		}
	}

	// success?
	if (nLength)
	{
		szHtml = new TCHAR[nLength + 1];

		memcpy(szHtml, sHtml, nLength * sizeof(TCHAR));
		szHtml[nLength] = 0;
	}

	// cleanup temp files
	// NOTE: we clean up temporary image folders when we close
	DeleteFile(sTempRtf);
	DeleteFile(sTempHtml);
		
	delete [] pDecompressed;

	// cleanup empty 'permanent' image folders.
	// This will fail if the folder contains anything
	if (!bTempImageDir)
		RemoveDirectory(sUniqueDir);

	return nLength;
}
