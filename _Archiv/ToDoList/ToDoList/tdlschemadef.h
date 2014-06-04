#if !defined(AFX_TDLSCHEMADEF_5951FDE6_508A_4A9D_A55D_D16EB026AEF7__INCLUDED_)
#define AFX_TDLSCHEMADEF_5951FDE6_508A_4A9D_A55D_D16EB026AEF7__INCLUDED_


static const CString TDL_ROOT(_T("TODOLIST"));
static const CString TDL_CHARSET(_T("CHARSET"));

static const CString TDL_PROJECTNAME(_T("PROJECTNAME"));
static const CString TDL_LASTSORTBY(_T("LASTSORTBY"));
static const CString TDL_LASTSORTDIR(_T("LASTSORTDIR"));
static const CString TDL_NEXTUNIQUEID(_T("NEXTUNIQUEID"));
static const CString TDL_ARCHIVE(_T("ARCHIVE"));
static const CString TDL_FILEFORMAT(_T("FILEFORMAT"));
static const CString TDL_FILEVERSION(_T("FILEVERSION"));
static const CString TDL_CHECKEDOUTTO(_T("CHECKEDOUTTO"));
static const CString TDL_COMMENTSTYPE(_T("CUSTOMCOMMENTSTYPE"));		
static const CString TDL_REPORTTITLE(_T("REPORTTITLE"));		
static const CString TDL_REPORTDATE(_T("REPORTDATE"));		
static const CString TDL_REPORTDATEOLE(_T("REPORTDATEOLE"));		
static const CString TDL_FILENAME(_T("FILENAME"));	
static const CString TDL_METADATA(_T("METADATA"));	
static const CString TDL_EARLIESTDUEDATE(_T("EARLIESTDUEDATE"));		
static const CString TDL_EXPORTSTYLE(_T("EXPORTSTYLE"));		

static const CString TDL_CUSTOMATTRIBDEFS(_T("CUSTOMATTRIB"));	
static const CString TDL_CUSTOMATTRIBID(_T("ID"));	
static const CString TDL_CUSTOMATTRIBTYPE(_T("TYPE"));	
static const CString TDL_CUSTOMATTRIBLABEL(_T("LABEL"));	
static const CString TDL_CUSTOMATTRIBCOLTITLE(_T("COLTITLE"));	
static const CString TDL_CUSTOMATTRIBCOLALIGN(_T("COLALIGN"));	
static const CString TDL_CUSTOMATTRIBSORTABLE(_T("SORTABLE"));	
static const CString TDL_CUSTOMATTRIBLISTDATA(_T("LISTDATA"));	
static const CString TDL_CUSTOMATTRIBENABLED(_T("ENABLED"));	
	
static const CString TDL_TASK(_T("TASK"));

// task attributes
static const CString TDL_TASKID(_T("ID"));
static const CString TDL_TASKREFID(_T("REFID"));
static const CString TDL_TASKPOS(_T("POS"));
static const CString TDL_TASKPARENTID(_T("PARENTID"));
static const CString TDL_TASKTITLE(_T("TITLE"));
static const CString TDL_TASKALLOCTO(_T("PERSON"));
static const CString TDL_TASKNUMALLOCTO(_T("NUMPERSON"));
static const CString TDL_TASKALLOCBY(_T("ALLOCATEDBY"));
static const CString TDL_TASKCATEGORY(_T("CATEGORY"));
static const CString TDL_TASKNUMCATEGORY(_T("NUMCATEGORY"));
static const CString TDL_TASKTAG(_T("TAG"));
static const CString TDL_TASKNUMTAGS(_T("NUMTAGS"));
static const CString TDL_TASKSTATUS(_T("STATUS"));
static const CString TDL_TASKFILEREFPATH(_T("FILEREFPATH"));
static const CString TDL_TASKCREATEDBY(_T("CREATEDBY"));
static const CString TDL_TASKCOLOR(_T("COLOR"));
static const CString TDL_TASKFLAG(_T("FLAG"));
static const CString TDL_TASKWEBCOLOR(_T("WEBCOLOR"));
static const CString TDL_TASKPRIORITY(_T("PRIORITY"));
static const CString TDL_TASKPERCENTDONE(_T("PERCENTDONE"));
static const CString TDL_TASKTIMEESTIMATE(_T("TIMEESTIMATE"));
static const CString TDL_TASKTIMEESTUNITS(_T("TIMEESTUNITS"));
static const CString TDL_TASKTIMESPENT(_T("TIMESPENT"));
static const CString TDL_TASKTIMESPENTUNITS(_T("TIMESPENTUNITS"));
static const CString TDL_TASKRECURRENCEREG(_T("REGULARITY"));
static const CString TDL_TASKRECURRENCESPEC1(_T("REGPECIFIC1"));
static const CString TDL_TASKRECURRENCESPEC2(_T("REGPECIFIC2"));
static const CString TDL_TASKRECURRENCEFROMDUE(_T("REGFROMDUE")); // Deprecated
static const CString TDL_TASKRECURRENCEFROM(_T("RECURFROM"));
static const CString TDL_TASKRECURRENCEREUSE(_T("RECURREUSE"));
static const CString TDL_TASKDEPENDENCY(_T("DEPENDS"));
static const CString TDL_TASKNUMDEPENDENCY(_T("NUMDEPENDS"));
static const CString TDL_TASKVERSION(_T("VERSION"));
static const CString TDL_TASKRECURRENCE(_T("RECURRENCE"));
static const CString TDL_TASKICONINDEX(_T("ICONINDEX"));
static const CString TDL_TASKMETADATA(_T("METADATA"));
static const CString TDL_TASKRISK(_T("RISK"));		
static const CString TDL_TASKCOST(_T("COST"));		
static const CString TDL_TASKEXTERNALID(_T("EXTERNALID"));

static const CString TDL_TASKCUSTOMATTRIBDATA(_T("CUSTOMATTRIB"));	
static const CString TDL_TASKCUSTOMATTRIBID(_T("ID"));	
static const CString TDL_TASKCUSTOMATTRIBNAME(_T("NAME"));	
static const CString TDL_TASKCUSTOMATTRIBVALUE(_T("VALUE"));	
static const CString TDL_TASKCUSTOMATTRIBDATESTRING(_T("DATESTRING"));	

// comments		
static const CString TDL_TASKCOMMENTS(_T("COMMENTS"));
static const CString TDL_TASKCUSTOMCOMMENTS(_T("CUSTOMCOMMENTS"));		
static const CString TDL_TASKCOMMENTSTYPE(_T("COMMENTSTYPE"));
static const CString TDL_TASKHTMLCOMMENTS(_T("HTMLCOMMENTS"));		

// dates
static const CString TDL_TASKDONEDATE(_T("DONEDATE"));
static const CString TDL_TASKDONEDATESTRING(_T("DONEDATESTRING"));		
static const CString TDL_TASKDUEDATE(_T("DUEDATE"));
static const CString TDL_TASKDUEDATESTRING(_T("DUEDATESTRING")); 	
static const CString TDL_TASKSTARTDATE(_T("STARTDATE"));
static const CString TDL_TASKSTARTDATESTRING(_T("STARTDATESTRING"));
static const CString TDL_TASKCREATIONDATE(_T("CREATIONDATE"));
static const CString TDL_TASKCREATIONDATESTRING(_T("CREATIONDATESTRING"));		
static const CString TDL_TASKLASTMOD(_T("LASTMOD"));
static const CString TDL_TASKLASTMODSTRING(_T("LASTMODSTRING"));
		
// calculated values
static const CString TDL_TASKTEXTCOLOR(_T("TEXTCOLOR"));		
static const CString TDL_TASKTEXTWEBCOLOR(_T("TEXTWEBCOLOR"));		
static const CString TDL_TASKPRIORITYCOLOR(_T("PRIORITYCOLOR"));		
static const CString TDL_TASKPRIORITYWEBCOLOR(_T("PRIORITYWEBCOLOR"));		
static const CString TDL_TASKCALCTIMEESTIMATE(_T("CALCTIMEESTIMATE"));		
static const CString TDL_TASKCALCTIMEESTUNITS(_T("CALCTIMEESTUNITS"));		
static const CString TDL_TASKCALCTIMESPENT(_T("CALCTIMESPENT"));		
static const CString TDL_TASKCALCTIMESPENTUNITS(_T("CALCTIMESPENTUNITS"));		
static const CString TDL_TASKCALCSTARTDATE(_T("CALCSTARTDATE"));		
static const CString TDL_TASKCALCSTARTDATESTRING(_T("CALCSTARTDATESTRING"));		
static const CString TDL_TASKCALCDUEDATE(_T("CALCDUEDATE"));		
static const CString TDL_TASKCALCDUEDATESTRING(_T("CALCDUEDATESTRING"));		
static const CString TDL_TASKCALCCOMPLETION(_T("CALCPERCENTDONE"));		
static const CString TDL_TASKHIGHESTPRIORITY(_T("HIGHESTPRIORITY"));		
static const CString TDL_TASKHIGHESTRISK(_T("HIGHESTRISK"));		
static const CString TDL_TASKCALCCOST(_T("CALCCOST"));		


#endif // AFX_TDLSCHEMADEF_5951FDE6_508A_4A9D_A55D_D16EB026AEF7__INCLUDED_