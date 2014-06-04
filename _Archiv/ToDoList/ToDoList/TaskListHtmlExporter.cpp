// TaskFileHtmlExporter.cpp: implementation of the CTaskListHtmlExporter class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "todolist.h"
#include "TasklistHtmlExporter.h"
#include "tdlschemadef.h"
#include "recurringtaskedit.h"

#include "..\shared\xmlfile.h"
#include "..\shared\preferences.h"
#include "..\shared\filemisc.h"

#include "..\3rdparty\stdiofileex.h"

#include <locale.h>
#include <Shlwapi.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

static LPCTSTR ENDL = _T("\n");
static LPCTSTR COMMENTS_DONECOLOR = _T("#808080");
static LPCTSTR DOCTYPE = _T("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">\n\n");
static LPCTSTR TAB = _T("&nbsp;&nbsp;&nbsp;&nbsp;");
static LPCTSTR SPACE = _T("&nbsp;");

enum { STYLE_WRAP, STYLE_TABLE, STYLE_PARA };

CTaskListHtmlExporter::CTaskListHtmlExporter() : STRIKETHRUDONE(TRUE), EXPORTSTYLE(STYLE_WRAP), ROOT(TRUE)
{
}

CTaskListHtmlExporter::~CTaskListHtmlExporter()
{
	
}

bool CTaskListHtmlExporter::ExportOutput(LPCTSTR szDestFilePath, const CString& sOutput)
{
	CString sHtmlOutput;
	
	sHtmlOutput += DOCTYPE;
	sHtmlOutput += _T("<html>\n<head>\n");
	sHtmlOutput += _T("<style>body { line-height: 1.0; }</style>\n");
	sHtmlOutput += CHARSET;
	sHtmlOutput += _T("</head>\n<body>\n");
	sHtmlOutput += sOutput;
	sHtmlOutput += _T("</body>\n</html>\n");

	return CTaskListExporterBase::ExportOutput(szDestFilePath, sHtmlOutput);
}

bool CTaskListHtmlExporter::InitConsts(const ITaskList10* pTasks, LPCTSTR szDestFilePath, BOOL bSilent, 
									   IPreferences* pPrefs, LPCTSTR szKey)
{
	// base consts
	if (!CTaskListExporterBase::InitConsts(pTasks, szDestFilePath, bSilent, pPrefs, szKey))
		return false;

	// then ours
	szKey = _T("Preferences");
		
	CString sFont = pPrefs->GetProfileString(szKey, _T("HTMLFont"), _T("Verdana"));
	int nFontSize = pPrefs->GetProfileInt(szKey, _T("HtmlFontSize"), 2);
	
	DEFAULTFONT.Format(_T("<font face='%s' size='%d'>"), sFont, nFontSize);
	HTMLNOTES.Empty();
	
	if (pPrefs->GetProfileInt(szKey, _T("ExportSpaceForNotes"), FALSE))
	{
		int nLine = pPrefs->GetProfileInt(szKey, _T("LineSpaces"), 8);
		
		if (nLine > 0)
		{
			HTMLNOTES = "<pre>";
			
			while (nLine--)
				HTMLNOTES += ENDL;
			
			HTMLNOTES += "</pre>";
		}
	}
	
	STRIKETHRUDONE = pPrefs->GetProfileInt(szKey, _T("StrikethroughDone"), TRUE);
	EXPORTSTYLE = _ttoi(pTasks->GetMetaData(TDL_EXPORTSTYLE));

	// charset
#ifdef _UNICODE
	CString sCS = "UTF-16"; 
#else
	CString sCS = pTasks->GetHtmlCharSet();
#endif
	
	if (!sCS.IsEmpty())
		CHARSET.Format(_T("<meta http-equiv=\"content-type\" content=\"text/html; charset=%s\">\n"), sCS);

	return true;
}

bool CTaskListHtmlExporter::Export(const ITaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey)
{
	return CTaskListExporterBase::Export(pSrcTaskFile, szDestFilePath, bSilent, pPrefs, szKey);
}

bool CTaskListHtmlExporter::Export(const IMultiTaskList* pSrcTaskFile, LPCTSTR szDestFilePath, BOOL bSilent, IPreferences* pPrefs, LPCTSTR szKey)
{
	return CTaskListExporterBase::Export(pSrcTaskFile, szDestFilePath, bSilent, pPrefs, szKey);
}

CString CTaskListHtmlExporter::FormatTitle(const ITaskList10* pTasks) const
{
	CString sTitleBlock;
			
	// title and date
	CString sTitle = pTasks->GetReportTitle();
	CString sDate = pTasks->GetReportDate();
			
	if (!sTitle.IsEmpty())
	{
		CString sProjTitle;
		sProjTitle.Format(_T("<h2>%s</h2>%s<p/>"), sTitle, sDate);
		
		sTitleBlock += DEFAULTFONT;
		sTitleBlock += sProjTitle;
	}
	else if (!sDate.IsEmpty())
	{
		sTitleBlock += DEFAULTFONT;
		sTitleBlock += sDate;
		sTitleBlock += _T("<p/>");
	}
	
	if (EXPORTSTYLE == STYLE_TABLE)
	{
		sTitleBlock += _T("<table border=\"0\">\n");
	}

	return sTitleBlock;
}

CString CTaskListHtmlExporter::FormatHeader() const
{
	CString sHeader = CTaskListExporterBase::FormatHeader();

	if (EXPORTSTYLE == STYLE_TABLE && !sHeader.IsEmpty())
	{
		sHeader = _T("<thead><tr>") + sHeader + _T("</tr></thead>");
	}

	return sHeader;
}

CString CTaskListHtmlExporter::FormatHeaderItem(TDC_ATTRIBUTE /*nAttrib*/, const CString& sAttribLabel) const
{
	CString sItem;

	if (EXPORTSTYLE == STYLE_TABLE)
	{
		sItem.Format(_T("<th>%s</th>"), sAttribLabel);
	}

	return sItem;
}

CString CTaskListHtmlExporter::ExportTask(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const
{
	// snapshot depth so we only embolden top-level tasks
	ROOT = (nDepth <= 1);

	CString sTask = CTaskListExporterBase::ExportTask(pTasks, hTask, nDepth);

	switch (EXPORTSTYLE)
	{
	case STYLE_WRAP:
	case STYLE_PARA:
		// figure out indent
		if (WANTPOS)
		{
			if (nDepth > 1)
			{
				sTask = _T("<blockquote>") + sTask;

				if (!pTasks->GetFirstTask(hTask)) // no sub-tasks
					sTask += _T("</blockquote>");
			}
		}
		else
		{
			if (nDepth > 0)
				sTask = _T("<li>") + sTask;
			
			// handle subtasks
			if (pTasks->GetFirstTask(hTask)) // at least one sub-task
				sTask += _T("<ul>");
		}

		// wrapped tasks without subtasks
		if (EXPORTSTYLE == STYLE_WRAP && !pTasks->GetFirstTask(hTask))
			sTask += _T("<br>");

		sTask += _T("<br>");
		break;
		
	case STYLE_TABLE:
		sTask = _T("<tr>") + sTask + _T("</tr>");
		break;
	}

	return sTask;
}

CString CTaskListHtmlExporter::ExportSubtasks(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth) const
{
	CString sSubtasks = CTaskListExporterBase::ExportSubtasks(pTasks, hTask, nDepth);

	if (!sSubtasks.IsEmpty()) // at least one sub-task
	{
		if (EXPORTSTYLE != STYLE_TABLE)
		{
			if (!WANTPOS)
				sSubtasks += _T("</ul>");
			else
				sSubtasks += _T("</blockquote>");
		}
	}

	if (nDepth == 0) // root
	{
		ASSERT(hTask == NULL);

		if (EXPORTSTYLE == STYLE_TABLE)
		{
			sSubtasks += _T("</table>\n");
		}
	}

	return sSubtasks;
}

CString CTaskListHtmlExporter::FormatAttribute(TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel, const CString& sValue) const
{
	CString sFmtAttrib;

	if (!sValue.IsEmpty())
	{
		// special case: task title following pos
		switch (nAttrib)
		{
		case TDCA_POSITION:
			sFmtAttrib.Format(_T("%s "), sValue);
			break;

		case TDCA_TASKNAME:
			{
				if (EXPORTSTYLE != STYLE_TABLE && WANTPOS)
					sFmtAttrib = TAB;
				
				// embolden top-level tasks only
				if (ROOT)
					sFmtAttrib += _T("<b>") + sValue + _T("</b>");
				else
					sFmtAttrib += sValue;

				if (EXPORTSTYLE == STYLE_PARA)
					sFmtAttrib += _T("<br>");
			}
			break;

		// all else
		default:
			{
				switch (EXPORTSTYLE)
				{
				case STYLE_WRAP:
					if (nAttrib == TDCA_COMMENTS)
					{
						sFmtAttrib = ENDL + sValue;
					}
					else
					{
						if (sAttribLabel.IsEmpty())
							sFmtAttrib.Format(_T(" | %s"), sValue);
						else
							sFmtAttrib.Format(_T(" | %s: %s"), sAttribLabel, sValue);
					}				
					break;
					
				case STYLE_TABLE:
					// special case: custom attrib
					if (nAttrib == TDCA_CUSTOMATTRIB)
						sFmtAttrib.Format(_T("<td>%s</td>"), sValue);
					else
						sFmtAttrib = sValue;
					break;
					
				case STYLE_PARA:
					if (sAttribLabel.IsEmpty())
						sFmtAttrib = sValue;
					else
						sFmtAttrib.Format(_T("%s: %s"), sAttribLabel, sValue);

					if (nAttrib != TDCA_COMMENTS)
						sFmtAttrib += _T("<br>");
					break;
				}
			}
			break;
		}
	}

	return sFmtAttrib;
}

CString CTaskListHtmlExporter::FormatAttribute(const ITaskList10* pTasks, HTASKITEM hTask, int nDepth, 
											   TDC_ATTRIBUTE nAttrib, const CString& sAttribLabel) const
{
	// base processing
	CString sItem = CTaskListExporterBase::FormatAttribute(pTasks, hTask, nDepth, nAttrib, sAttribLabel);

	// extra processing
	CString sColor = pTasks->GetTaskWebColor(hTask);
	BOOL bColor = TRUE;
	BOOL bStrikeThru = (STRIKETHRUDONE && pTasks->IsTaskDone(hTask));
	BOOL bBlockQuote = FALSE;
	
	switch (nAttrib)
	{
	case TDCA_ID:
		bColor = FALSE;
		bStrikeThru = FALSE;
		break;
		
	case TDCA_PARENTID:
		// ignore if not set
		if (pTasks->GetTaskParentID(hTask) == 0)
			sItem.Empty();

		bColor = FALSE;
		bStrikeThru = FALSE;
		break;
		
	case TDCA_PRIORITY:
		{
			int nPriority = pTasks->GetTaskPriority(hTask, TRUE);
			CString sPriority;

			if (nPriority >= 0)
			{
				CString sPriorityCol = pTasks->GetTaskPriorityWebColor(hTask);
				sPriority.Format(_T("<font color='%s'>%d</font>"), sPriorityCol, nPriority);

				sItem = FormatAttribute(nAttrib, sAttribLabel, sPriority);
			}
			
			bStrikeThru = FALSE;
		}
		break;
		
	case TDCA_FILEREF:
		if (!sItem.IsEmpty())
		{
			CString sFilePath = pTasks->GetTaskFileReferencePath(hTask), sFileName;

			if (PathIsURL(sFilePath))
				sFileName = sFilePath;
			else
			{
				sFileName = FileMisc::GetFileNameFromPath(sFilePath);

				// handle the absence of a filename
				if (sFileName.IsEmpty())
					sFileName = sFilePath;
			}

			CString sFileRef;

			sFileRef.Format(_T("<a href=\"%s\">%s</a>"), sFilePath, sFileName);
			sItem = FormatAttribute(nAttrib, sAttribLabel, sFileRef);
			
			bColor = FALSE;
			bStrikeThru = FALSE;
		}
		break;
		
	case TDCA_COMMENTS:
		if (pTasks->TaskHasAttribute(hTask, TDL_TASKHTMLCOMMENTS))
		{
			sItem = pTasks->GetTaskAttribute(hTask, TDL_TASKHTMLCOMMENTS);
			sItem.TrimRight();
			
			// note: we reset the font after the comments because the font
			// face and size may well have been changed
			if (!sItem.IsEmpty())
				sItem += DEFAULTFONT;
			
			bColor = FALSE;
			bStrikeThru = FALSE;
			bBlockQuote = (EXPORTSTYLE != STYLE_TABLE);
		}
		else if (!sItem.IsEmpty())
		{
			TXT2XML(sItem); // TODO
			
			// replace carriage returns with <br>
			sItem.Replace(ENDL, _T("<br>"));
			
			// replace tab characters with multiple &nbsp;
			sItem.Replace(_T("\t"), TAB);
			
			sColor = pTasks->IsTaskDone(hTask) ? COMMENTS_DONECOLOR : _T("#606060");
			bStrikeThru = FALSE;
		}
		break;
	}
	
	if (!sItem.IsEmpty())
	{
		if (bStrikeThru)
		{
			CString sStrikeThru;
			sStrikeThru.Format(_T("<s>%s</s>"), sItem);
			sItem = sStrikeThru;
		}
		
		if (bColor)
		{
			CString sColored;
			sColored.Format(_T("<font color='%s'>%s</font>"), sColor, sItem);
			sItem = sColored;
		}
		
		if (bBlockQuote)
		{
			CString sBlockQuote;
			sBlockQuote.Format(_T("<blockquote>%s</blockquote>"), sItem);
			sItem = sBlockQuote;
		}
	}

	// we've already handled custom attrib above
	if (EXPORTSTYLE == STYLE_TABLE && nAttrib != TDCA_CUSTOMATTRIB)
	{
		if (sItem.IsEmpty())
			sItem = SPACE;

		sItem = _T("<td>") + sItem + _T("</td>");
	}

    return sItem;
}

