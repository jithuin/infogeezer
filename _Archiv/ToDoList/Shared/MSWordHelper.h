// MSWordHelper.h: interface for the CMSWordHelper class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MSWORDHELPER_H__A86706EB_171B_4FD4_AB0B_C486D09C5181__INCLUDED_)
#define AFX_MSWORDHELPER_H__A86706EB_171B_4FD4_AB0B_C486D09C5181__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

// predecs
namespace WordAPI
{

	enum MsoEncoding
	{
		msoEncodingAutoDetect							= 50001, //	Web browser auto-detects type of encoding to use.
	};

	enum WdOpenFormat
	{
		wdOpenFormatAllWord								=  6, // A Microsoft Word format that is backward compatible with earlier versions of Word.
		wdOpenFormatAuto								=  0, // The existing format.
		wdOpenFormatDocument							=  1, // Word format.
		wdOpenFormatEncodedText							=  5, // Encoded text format.
		wdOpenFormatRTF									=  3, // Rich text format (RTF).
		wdOpenFormatTemplate							=  2, // As a Word template.
		wdOpenFormatText								=  4, // Unencoded text format.
		wdOpenFormatOpenDocumentText					= 18, // OpenDocument Text format.
		wdOpenFormatUnicodeText							=  5, // Unicode text format.
		wdOpenFormatWebPages							=  7, // HTML format.
		wdOpenFormatXML									=  8, // XML format.
		wdOpenFormatAllWordTemplates					= 13, // Word template format.
		wdOpenFormatDocument97							=  1, // Microsoft Word 97 document format.
		wdOpenFormatTemplate97							=  2, // Word 97 template format.
		wdOpenFormatXMLDocument							=  9, // XML document format.
		wdOpenFormatXMLDocumentSerialized				= 14, // Open XML file format saved as a single XML file.
		wdOpenFormatXMLDocumentMacroEnabled				= 10, // XML document format with macros enabled.
		wdOpenFormatXMLDocumentMacroEnabledSerialized	= 15, // Open XML file format with macros enabled saved as a single XML file.
		wdOpenFormatXMLTemplate							= 11, // XML template format.
		wdOpenFormatXMLTemplateSerialized				= 16, // Open XML template format saved as a XML single file.
		wdOpenFormatXMLTemplateMacroEnabled				= 12, // XML template format with macros enabled.
		wdOpenFormatXMLTemplateMacroEnabledSerialized	= 17, // Open XML template format with macros enabled saved as a single XML file.
	};

	enum WdSaveFormat
	{
		wdSaveFormatDocument							=  0, // Microsoft Office Word 97 - 2003 binary file format.
		wdSaveFormatDOSText								=  4, // Microsoft DOS text format.
		wdSaveFormatDOSTextLineBreaks					=  5, // Microsoft DOS text with line breaks preserved.
		wdSaveFormatEncodedText							=  7, // Encoded text format.
		wdSaveFormatFilteredHTML						= 10, // Filtered HTML format.
		wdSaveFormatFlatXML								= 19, // Open XML file format saved as a single XML file.
		wdSaveFormatFlatXMLMacroEnabled					= 20, // Open XML file format with macros enabled saved as a single XML file.
		wdSaveFormatFlatXMLTemplate						= 21, // Open XML template format saved as a XML single file.
		wdSaveFormatFlatXMLTemplateMacroEnabled			= 22, // Open XML template format with macros enabled saved as a single XML file.
		wdSaveFormatOpenDocumentText					= 23, // OpenDocument Text format.
		wdSaveFormatHTML								=  8, // Standard HTML format.
		wdSaveFormatRTF									=  6, // Rich text format (RTF).
		wdSaveFormatTemplate							=  1, // Word template format.
		wdSaveFormatText								=  2, // Microsoft Windows text format.
		wdSaveFormatTextLineBreaks						=  3, // Windows text format with line breaks preserved.
		wdSaveFormatUnicodeText							=  7, // Unicode text format.
		wdSaveFormatWebArchive							=  9, // Web archive format.
		wdSaveFormatXML									= 11, // Extensible Markup Language (XML) format.
		wdSaveFormatDocument97							=  0, // Microsoft Word 97 document format.
		wdSaveFormatDocumentDefault						= 16, // Word default document file format. For Word 2010, this is the DOCX format.
		wdSaveFormatPDF									= 17, // PDF format.
		wdSaveFormatTemplate97							=  1, // Word 97 template format.
		wdSaveFormatXMLDocument							= 12, // XML document format.
		wdSaveFormatXMLDocumentMacroEnabled				= 13, // XML document format with macros enabled.
		wdSaveFormatXMLTemplate							= 14, // XML template format.
		wdSaveFormatXMLTemplateMacroEnabled				= 15, // XML template format with macros enabled.
		wdSaveFormatXPS									= 18, // XPS format.
	};

	class _Application;
}

class CMSWordHelper
{
public:
	CMSWordHelper(int nMinVersion = 0);
	virtual ~CMSWordHelper();

	BOOL IsValid() const { return (s_pWord != NULL); }

	BOOL ConvertContent(const CString& sInput, WordAPI::WdOpenFormat nInputFormat, 
						CString& sOutput, WordAPI::WdSaveFormat nOutputFormat, 
						WordAPI::MsoEncoding nEncoding = WordAPI::msoEncodingAutoDetect);

	BOOL ConvertFile(const CString& sInputFilename, WordAPI::WdOpenFormat nInputFormat, 
					const CString& sOutputFilename, WordAPI::WdSaveFormat nOutputFormat,
					WordAPI::MsoEncoding nEncoding = WordAPI::msoEncodingAutoDetect);

	static BOOL IsWordInstalled(int nMinVersion = 0);
	static int GetInstalledWordVersion();

protected:
	static WordAPI::_Application* s_pWord;
	static int s_nRefCount;
	CStringArray m_aTempFiles;

protected:
	static BOOL RestartWord();

};

#endif // !defined(AFX_MSWORDHELPER_H__A86706EB_171B_4FD4_AB0B_C486D09C5181__INCLUDED_)
