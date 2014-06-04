# Microsoft Developer Studio Project File - Name="ToDoList" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Application" 0x0101

CFG=ToDoList - Win32 Unicode Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "ToDoList.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "ToDoList.mak" CFG="ToDoList - Win32 Unicode Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "ToDoList - Win32 Unicode Debug" (based on "Win32 (x86) Application")
!MESSAGE "ToDoList - Win32 Unicode Release" (based on "Win32 (x86) Application")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "ToDoList - Win32 Unicode Debug"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "ToDoList___Win32_Unicode_Debug"
# PROP BASE Intermediate_Dir "ToDoList___Win32_Unicode_Debug"
# PROP BASE Ignore_Export_Lib 1
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Unicode_Debug"
# PROP Intermediate_Dir "Unicode_Debug"
# PROP Ignore_Export_Lib 1
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MDd /W4 /Gm /GX /Zi /Od /D "_DEBUG" /D "WIN32" /D "_WINDOWS" /D "_AFXDLL" /D "_MBCS" /D "NO_DIAGRAM_TEMPLATE" /D "NO_XML_TEMPLATE" /FR /Yu"stdafx.h" /FD /GZ /c
# ADD CPP /nologo /MDd /W4 /Gm /GX /Zi /Od /D "_AFXDLL" /D "NO_DIAGRAM_TEMPLATE" /D "NO_XML_TEMPLATE" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /FR /Yu"stdafx.h" /FD /GZ /c
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0xc09 /d "_DEBUG" /d "_AFXDLL"
# ADD RSC /l 0xc09 /d "_DEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 Msimg32.lib msimg32.lib Winmm.lib /nologo /subsystem:windows /debug /machine:I386 /pdbtype:sept
# ADD LINK32 msimg32.lib Winmm.lib /nologo /entry:"wWinMainCRTStartup" /subsystem:windows /debug /machine:I386 /pdbtype:sept /libpath:"..\3rdparty\vld"

!ELSEIF  "$(CFG)" == "ToDoList - Win32 Unicode Release"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "ToDoList___Win32_Unicode_Release"
# PROP BASE Intermediate_Dir "ToDoList___Win32_Unicode_Release"
# PROP BASE Ignore_Export_Lib 1
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Unicode_Release"
# PROP Intermediate_Dir "Unicode_Release"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MD /W4 /GX /Zi /O1 /D "NDEBUG" /D "WIN32" /D "_WINDOWS" /D "_MBCS" /D "NO_DIAGRAM_TEMPLATE" /D "NO_XML_TEMPLATE" /D "USE_TRANSTEXT" /D "_AFXDLL" /FR /Yu"stdafx.h" /FD /c
# ADD CPP /nologo /MD /W4 /GX /Zd /O1 /D "NO_DIAGRAM_TEMPLATE" /D "NO_XML_TEMPLATE" /D "USE_TRANSTEXT" /D "_AFXDLL" /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /Yu"stdafx.h" /FD /c
# SUBTRACT CPP /Fr
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0xc09 /d "NDEBUG" /d "_AFXDLL"
# ADD RSC /l 0xc09 /d "NDEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 Msimg32.lib msimg32.lib Winmm.lib /nologo /subsystem:windows /pdb:"ToDoList.pdb" /map /machine:I386 /pdbtype:con /MAPINFO:LINES /MAPINFO:EXPORTS
# SUBTRACT BASE LINK32 /pdb:none /debug
# ADD LINK32 msimg32.lib Winmm.lib /nologo /entry:"wWinMainCRTStartup" /subsystem:windows /map /machine:I386 /pdbtype:con /MAPINFO:LINES /MAPINFO:EXPORTS

!ENDIF 

# Begin Target

# Name "ToDoList - Win32 Unicode Debug"
# Name "ToDoList - Win32 Unicode Release"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=..\Shared\AboutDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\autocombobox.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\AutoFlag.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Base64Coder.cpp
# ADD CPP /Yu
# End Source File
# Begin Source File

SOURCE=..\Shared\BinaryData.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\browserdlg.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\checkcombobox.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\checklistboxex.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ClipboardBackup.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\colorbutton.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\colorcombobox.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ColourPicker.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ColourPopup.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ContentCtrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ContentMgr.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\contenttypecombobox.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\custombutton.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\DateHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\deferWndMove.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\DialogHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\DlgUnits.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\DockManager.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\DragDrop.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\driveinfo.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EditShortcutMgr.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnBitmap.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnBitmapEx.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\enbrowserctrl.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\EnCheckComboBox.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\encolordialog.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\EnCommandLineInfo.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\enedit.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\enfiledialog.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnHeaderCtrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\enlistctrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnMenu.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnRecentFileList.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\enstatic.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\EnString.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\entoolbar.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\fileedit.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\FILEMISC.CPP
# End Source File
# Begin Source File

SOURCE=..\Shared\FileRegister.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\FileVersionInfo.cpp
# End Source File
# Begin Source File

SOURCE=.\FilteredToDoCtrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\FocusWatcher.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\FolderDialog.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\FontComboBox.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ggets.cpp
# PROP Exclude_From_Build 1
# End Source File
# Begin Source File

SOURCE=..\Shared\GraphicsMisc.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\GroupLine.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\GUI.CPP
# End Source File
# Begin Source File

SOURCE=..\Shared\HoldRedraw.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\hotkeyctrlex.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\HotTracker.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ImageProcessors.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ImportExportMgr.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Ini.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\InputListCtrl.cpp
# End Source File
# Begin Source File

SOURCE=.\KeyboardShortcutDisplayDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\LightBox.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Localizer.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\3rdParty\LockableHeaderCtrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\MASKEDIT.CPP
# End Source File
# Begin Source File

SOURCE=..\Shared\menubutton.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\MenuIconMgr.cpp
# End Source File
# Begin Source File

SOURCE=.\MergeToDoList.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Misc.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\monthcombobox.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\MouseWheelMgr.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\msoutl.cpp
# End Source File
# Begin Source File

SOURCE=.\MultiTaskFile.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\NcGutter.cpp
# End Source File
# Begin Source File

SOURCE=.\OffsetDatesDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\OrderedTreeCtrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\OSVersion.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\OutlookHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ownerdrawcomboboxbase.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\passworddialog.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\popupEditctrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\popupListboxctrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Preferences.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\PreferencesBase.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesExportPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesFile2Page.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesFilePage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesGenPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesMultiUserPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesShortcutsPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesTaskCalcPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesTaskDefPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesTaskPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesToolPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesUIPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesUITasklistColorsPage.cpp
# End Source File
# Begin Source File

SOURCE=.\PreferencesUITasklistPage.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ProgressCtrlWithTimer.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ProgressThread.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\PropertyPageHost.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\RCCtrlParser.cpp
# End Source File
# Begin Source File

SOURCE=.\RecurringTaskEdit.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Regkey.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\RegUtil.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\richeditbasectrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\RichEditHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\RichEditNcBorder.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\RichEditSpellCheck.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\RuntimeDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\SaveFocus.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ScrollingPropertyPageHost.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ShellIcons.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ShortcutManager.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\SpellCheckDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\StatLink.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\StatusBarACT.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\StatusBarProgress.cpp
# End Source File
# Begin Source File

SOURCE=.\StdAfx.cpp
# ADD CPP /Yc"stdafx.h"
# End Source File
# Begin Source File

SOURCE=..\3rdParty\StdioFileEx.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\StickiesAPI.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\StickiesWnd.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Subclass.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\symbolbutton.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\SysImageList.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\tabbedcombobox.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=.\TabbedToDoCtrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\tabctrlex.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=.\TaskClipboard.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskFile.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskListCsvExporter.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskListCsvImporter.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskListDropTarget.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskListExporterBase.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskListHtmlExporter.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\TasklistStorageMgr.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskListTdlExporter.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskListTdlImporter.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskListTxtExporter.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskSelectionDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TaskTimeLog.cpp
# End Source File
# Begin Source File

SOURCE=.\TDCCustomAttributeHelper.cpp
# End Source File
# Begin Source File

SOURCE=.\TDCImageList.cpp
# End Source File
# Begin Source File

SOURCE=.\TDCListView.cpp
# End Source File
# Begin Source File

SOURCE=.\TDCSearchParamHelper.cpp
# End Source File
# Begin Source File

SOURCE=.\TDCTreeView.cpp
# End Source File
# Begin Source File

SOURCE=.\TDCViewTabControl.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLAddLoggedTimeDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLAttribListBox.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLCmdlineOptionsDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLColumnListBox.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLColumnSelectionDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLContentMgr.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLCsvAttributeSetupListCtrl.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLCsvImportExportDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLCustomAttributeDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLExportDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLFilterBar.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLFilterComboBox.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLFilterDateComboBox.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLFilterDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLFilterOptionComboBox.cpp
# End Source File
# Begin Source File

SOURCE=.\tdlfindresultslistctrl.cpp
# End Source File
# Begin Source File

SOURCE=.\tdlfindtaskattributecombobox.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLFindTaskExpressionListCtrl.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLFindTasksDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\tdliconcombobox.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLImportDialog.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLImportExportMgr.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLImportOutlookObjectsDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLLanguageComboBox.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLLanguageDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLMultiSortDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLOleMessageFilter.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLPrefMigrationDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLPrintDialog.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLPriorityComboBox.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLReuseRecurringTaskDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLRiskComboBox.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLSendTasksDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLSetReminderDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLShowReminderDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLTaskIconDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLTasklistImportDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLTasklistStorageMgr.cpp
# End Source File
# Begin Source File

SOURCE=.\TDLTransformDialog.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Themed.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\timecombobox.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\TimeEdit.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\TimeHelper.cpp
# End Source File
# Begin Source File

SOURCE=.\todocommentsctrl.cpp
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrl.cpp
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlData.cpp
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlFind.cpp
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlMgr.cpp
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlReminders.cpp
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlUndo.cpp
# End Source File
# Begin Source File

SOURCE=.\ToDoitem.cpp
# End Source File
# Begin Source File

SOURCE=.\ToDoList.cpp
# End Source File
# Begin Source File

SOURCE=.\ToDoList.rc
# End Source File
# Begin Source File

SOURCE=.\ToDoListWnd.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ToolbarHelper.cpp
# End Source File
# Begin Source File

SOURCE=.\ToolsCmdlineParser.cpp
# End Source File
# Begin Source File

SOURCE=.\ToolsHelper.cpp
# End Source File
# Begin Source File

SOURCE=.\ToolsUserInputDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\TRAYICON.CPP
# End Source File
# Begin Source File

SOURCE=..\Shared\TreeCtrlHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\TreeDragDropHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\TreeSelectionHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\UIExtensionMgr.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\UIExtensionUIHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\UIExtensionWnd.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\UIThemeFile.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\urlricheditctrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\VersionInfo.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\webbrowserctrl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\weekdaychecklistbox.cpp
# ADD CPP /I "..\ToDoList"
# End Source File
# Begin Source File

SOURCE=..\Shared\Weekdaycombobox.cpp
# End Source File
# Begin Source File

SOURCE=.\WelcomeWizard.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\WinClasses.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\WndPrompt.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\XHTMLStatic.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\XmlFile.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\xmlfileex.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\XmlNodeWrapper.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\XNamedColors.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\XPTabCtrl.cpp
# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl"
# Begin Source File

SOURCE=..\Shared\AboutDlg.h
# End Source File
# Begin Source File

SOURCE=..\Shared\autocombobox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\AutoFlag.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Base64Coder.h
# End Source File
# Begin Source File

SOURCE=..\Shared\BinaryData.h
# End Source File
# Begin Source File

SOURCE=..\Shared\browserdlg.h
# End Source File
# Begin Source File

SOURCE=..\Shared\checkcombobox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\checklistboxex.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ClipboardBackup.h
# End Source File
# Begin Source File

SOURCE=..\Shared\colorbutton.h
# End Source File
# Begin Source File

SOURCE=..\Shared\colorcombobox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ColorDef.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ColourPicker.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ColourPopup.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ContentCtrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ContentMgr.h
# End Source File
# Begin Source File

SOURCE=..\Shared\contenttypecombobox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\custombutton.h
# End Source File
# Begin Source File

SOURCE=..\Shared\DateHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\deferWndMove.h
# End Source File
# Begin Source File

SOURCE=..\Shared\DialogHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\DlgUnits.h
# End Source File
# Begin Source File

SOURCE=..\Shared\DockManager.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\DragDrop.h
# End Source File
# Begin Source File

SOURCE=..\Shared\driveinfo.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EditShortcutMgr.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnBitmap.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnBitmapEx.h
# End Source File
# Begin Source File

SOURCE=..\Shared\enbrowserctrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnCheckComboBox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\encolordialog.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnCommandLineInfo.h
# End Source File
# Begin Source File

SOURCE=..\Shared\enedit.h
# End Source File
# Begin Source File

SOURCE=..\Shared\enfiledialog.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnHeaderCtrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\enlistctrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnMenu.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnRecentFileList.h
# End Source File
# Begin Source File

SOURCE=..\Shared\enstatic.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnString.h
# End Source File
# Begin Source File

SOURCE=..\Shared\entoolbar.h
# End Source File
# Begin Source File

SOURCE=..\Shared\fileedit.h
# End Source File
# Begin Source File

SOURCE=..\Shared\FILEMISC.H
# End Source File
# Begin Source File

SOURCE=..\Shared\FileRegister.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\FileVersionInfo.h
# End Source File
# Begin Source File

SOURCE=.\FilteredToDoCtrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\FocusWatcher.h
# End Source File
# Begin Source File

SOURCE=..\Shared\FolderDialog.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\FontComboBox.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\FontPreviewCombo.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ggets.h
# End Source File
# Begin Source File

SOURCE=..\Shared\GraphicsMisc.h
# End Source File
# Begin Source File

SOURCE=..\Shared\GroupLine.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\GUI.H
# End Source File
# Begin Source File

SOURCE=..\Shared\HoldRedraw.h
# End Source File
# Begin Source File

SOURCE=..\Shared\HookMgr.h
# End Source File
# Begin Source File

SOURCE=..\Shared\hotkeyctrlex.h
# End Source File
# Begin Source File

SOURCE=..\Shared\HotTracker.h
# End Source File
# Begin Source File

SOURCE=..\Shared\HtmlCharMap.h
# End Source File
# Begin Source File

SOURCE=..\Shared\IContentControl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\IEncryption.h
# End Source File
# Begin Source File

SOURCE=..\Shared\IImportExport.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ImageProcessors.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ImportExportMgr.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Ini.h
# End Source File
# Begin Source File

SOURCE=..\Shared\InputListCtrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\IPreferences.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ISpellCheck.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ITaskList.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ITaskListStorage.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ITransText.h
# End Source File
# Begin Source File

SOURCE=..\Shared\IUIExtension.h
# End Source File
# Begin Source File

SOURCE=.\KeyboardShortcutDisplayDlg.h
# End Source File
# Begin Source File

SOURCE=..\Shared\LightBox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\LimitSingleInstance.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Localizer.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\LockableHeaderCtrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\MASKEDIT.H
# End Source File
# Begin Source File

SOURCE=..\Shared\menubutton.h
# End Source File
# Begin Source File

SOURCE=..\Shared\MenuIconMgr.h
# End Source File
# Begin Source File

SOURCE=.\MergeToDoList.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Misc.h
# End Source File
# Begin Source File

SOURCE=..\Shared\monthcombobox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\MouseWheelMgr.h
# End Source File
# Begin Source File

SOURCE=.\MultiTaskFile.h
# End Source File
# Begin Source File

SOURCE=..\Shared\NcGutter.h
# End Source File
# Begin Source File

SOURCE=.\OffsetDatesDlg.h
# End Source File
# Begin Source File

SOURCE=..\Shared\OrderedTreeCtrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\OSVersion.h
# End Source File
# Begin Source File

SOURCE=..\Shared\OutlookHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ownerdrawcomboboxbase.h
# End Source File
# Begin Source File

SOURCE=..\Shared\passworddialog.h
# End Source File
# Begin Source File

SOURCE=..\Shared\popupEditCtrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\popupListboxctrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Preferences.h
# End Source File
# Begin Source File

SOURCE=..\Shared\PreferencesBase.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesDlg.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesExportPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesFile2Page.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesFilePage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesGenPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesMultiUserPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesShortcutsPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesTaskCalcPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesTaskDefPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesTaskPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesToolPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesUIPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesUITasklistColorsPage.h
# End Source File
# Begin Source File

SOURCE=.\PreferencesUITasklistPage.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ProgressCtrlWithTimer.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ProgressThread.h
# End Source File
# Begin Source File

SOURCE=..\Shared\PropertyPageHost.h
# End Source File
# Begin Source File

SOURCE=..\Shared\RCCtrlParser.h
# End Source File
# Begin Source File

SOURCE=.\RecurringTaskEdit.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Regkey.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\RegUtil.h
# End Source File
# Begin Source File

SOURCE=.\Resource.h
# End Source File
# Begin Source File

SOURCE=..\Shared\richeditbasectrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\RichEditHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\RichEditNcBorder.h
# End Source File
# Begin Source File

SOURCE=..\Shared\RichEditSpellCheck.h
# End Source File
# Begin Source File

SOURCE=..\Shared\RuntimeDlg.h
# End Source File
# Begin Source File

SOURCE=..\Shared\SaveFocus.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Schemadef.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ScrollingPropertyPageHost.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\SendFileTo.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ShellIcons.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ShortcutManager.h
# End Source File
# Begin Source File

SOURCE=..\Shared\SpellCheckDlg.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\StatLink.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\StatusBarACT.h
# End Source File
# Begin Source File

SOURCE=..\Shared\StatusBarProgress.h
# End Source File
# Begin Source File

SOURCE=.\StdAfx.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\StdioFileEx.h
# End Source File
# Begin Source File

SOURCE=..\Shared\StickiesAPI.h
# End Source File
# Begin Source File

SOURCE=..\Shared\StickiesWnd.h
# End Source File
# Begin Source File

SOURCE=..\Shared\stringres.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Subclass.h
# End Source File
# Begin Source File

SOURCE=..\Shared\symbolbutton.h
# End Source File
# Begin Source File

SOURCE=..\Shared\SysImageList.h
# End Source File
# Begin Source File

SOURCE=..\Shared\tabbedcombobox.h
# End Source File
# Begin Source File

SOURCE=.\TabbedToDoCtrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\tabctrlex.h
# End Source File
# Begin Source File

SOURCE=.\TaskClipboard.h
# End Source File
# Begin Source File

SOURCE=.\TaskFile.h
# End Source File
# Begin Source File

SOURCE=.\TaskListCsvExporter.h
# End Source File
# Begin Source File

SOURCE=.\TaskListCsvImporter.h
# End Source File
# Begin Source File

SOURCE=.\TaskListDropTarget.h
# End Source File
# Begin Source File

SOURCE=.\TaskListExporterBase.h
# End Source File
# Begin Source File

SOURCE=.\TaskListHtmlExporter.h
# End Source File
# Begin Source File

SOURCE=..\Shared\TaskListStorageMgr.h
# End Source File
# Begin Source File

SOURCE=.\TaskListTdlExporter.h
# End Source File
# Begin Source File

SOURCE=.\TaskListTdlImporter.h
# End Source File
# Begin Source File

SOURCE=.\TaskListTxtExporter.h
# End Source File
# Begin Source File

SOURCE=.\TaskSelectionDlg.h
# End Source File
# Begin Source File

SOURCE=.\TaskTimeLog.h
# End Source File
# Begin Source File

SOURCE=.\TDCCustomAttributeHelper.h
# End Source File
# Begin Source File

SOURCE=.\tdcenum.h
# End Source File
# Begin Source File

SOURCE=.\TDCImageList.h
# End Source File
# Begin Source File

SOURCE=.\TDCListView.h
# End Source File
# Begin Source File

SOURCE=.\tdcmsg.h
# End Source File
# Begin Source File

SOURCE=.\TDCSearchParamHelper.h
# End Source File
# Begin Source File

SOURCE=.\tdcstatic.h
# End Source File
# Begin Source File

SOURCE=.\tdcstruct.h
# End Source File
# Begin Source File

SOURCE=.\TDCTreeView.h
# End Source File
# Begin Source File

SOURCE=.\TDCViewTabControl.h
# End Source File
# Begin Source File

SOURCE=.\TDLAddLoggedTimeDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLAttribListBox.h
# End Source File
# Begin Source File

SOURCE=.\TDLCmdlineOptionsDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLColumnListBox.h
# End Source File
# Begin Source File

SOURCE=.\TDLColumnSelectionDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLContentMgr.h
# End Source File
# Begin Source File

SOURCE=.\TDLCsvAttributeSetupListCtrl.h
# End Source File
# Begin Source File

SOURCE=.\TDLCsvImportExportDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLCustomAttributeDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLExportDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLFilterBar.h
# End Source File
# Begin Source File

SOURCE=.\TDLFilterComboBox.h
# End Source File
# Begin Source File

SOURCE=.\TDLFilterDateComboBox.h
# End Source File
# Begin Source File

SOURCE=.\TDLFilterDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLFilterOptionComboBox.h
# End Source File
# Begin Source File

SOURCE=.\tdlfindresultslistctrl.h
# End Source File
# Begin Source File

SOURCE=.\tdlfindtaskattributecombobox.h
# End Source File
# Begin Source File

SOURCE=.\TDLFindTaskExpressionListCtrl.h
# End Source File
# Begin Source File

SOURCE=.\TDLFindTasksDlg.h
# End Source File
# Begin Source File

SOURCE=.\tdliconcombobox.h
# End Source File
# Begin Source File

SOURCE=.\TDLImportDialog.h
# End Source File
# Begin Source File

SOURCE=.\TDLImportExportMgr.h
# End Source File
# Begin Source File

SOURCE=.\TDLImportOutlookObjectsDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLLanguageComboBox.h
# End Source File
# Begin Source File

SOURCE=.\TDLLanguageDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLMultiSortDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLOleMessageFilter.h
# End Source File
# Begin Source File

SOURCE=.\TDLPrefMigrationDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLPrintDialog.h
# End Source File
# Begin Source File

SOURCE=.\TDLPriorityComboBox.h
# End Source File
# Begin Source File

SOURCE=.\TDLReuseRecurringTaskDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLRiskComboBox.h
# End Source File
# Begin Source File

SOURCE=.\tdlschemadef.h
# End Source File
# Begin Source File

SOURCE=.\TDLSendTasksDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLSetReminderDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLShowReminderDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLTaskIconDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLTasklistImportDlg.h
# End Source File
# Begin Source File

SOURCE=.\TDLTasklistStorageMgr.h
# End Source File
# Begin Source File

SOURCE=.\TDLTransformDialog.h
# End Source File
# Begin Source File

SOURCE=.\tdstringres.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\TemplateSmartPtr.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Themed.h
# End Source File
# Begin Source File

SOURCE=..\Shared\timecombobox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\TimeEdit.h
# End Source File
# Begin Source File

SOURCE=..\Shared\TimeHelper.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Tmschema.h
# End Source File
# Begin Source File

SOURCE=.\todocommentsctrl.h
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrl.h
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlData.h
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlFind.h
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlMgr.h
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlReminders.h
# End Source File
# Begin Source File

SOURCE=.\ToDoCtrlUndo.h
# End Source File
# Begin Source File

SOURCE=.\ToDoitem.h
# End Source File
# Begin Source File

SOURCE=.\ToDoList.h
# End Source File
# Begin Source File

SOURCE=.\ToDoListWnd.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\TOM.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ToolbarHelper.h
# End Source File
# Begin Source File

SOURCE=.\ToolsCmdlineParser.h
# End Source File
# Begin Source File

SOURCE=.\ToolsHelper.h
# End Source File
# Begin Source File

SOURCE=.\ToolsUserInputDlg.h
# End Source File
# Begin Source File

SOURCE=..\Shared\TRAYICON.H
# End Source File
# Begin Source File

SOURCE=..\Shared\TreeCtrlHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\TreeDragDropHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\TreeSelectionHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\UIExtensionMgr.h
# End Source File
# Begin Source File

SOURCE=..\Shared\UIExtensionUIHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\UIExtensionWnd.h
# End Source File
# Begin Source File

SOURCE=..\Shared\UITHEME.h
# End Source File
# Begin Source File

SOURCE=..\Shared\UIThemeFile.h
# End Source File
# Begin Source File

SOURCE=..\Shared\urlricheditctrl.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Uxtheme.h
# End Source File
# Begin Source File

SOURCE=..\Shared\VersionInfo.h
# End Source File
# Begin Source File

SOURCE=..\Shared\wclassdefines.h
# End Source File
# Begin Source File

SOURCE=..\Shared\webbrowserctrl.h
# End Source File
# Begin Source File

SOURCE=..\Shared\weekdaychecklistbox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Weekdaycombobox.h
# End Source File
# Begin Source File

SOURCE=.\WelcomeWizard.h
# End Source File
# Begin Source File

SOURCE=..\Shared\WinClasses.h
# End Source File
# Begin Source File

SOURCE=..\Shared\winstyles.h
# End Source File
# Begin Source File

SOURCE=..\Shared\WndPrompt.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\XHTMLStatic.h
# End Source File
# Begin Source File

SOURCE=..\Shared\XmlCharMap.h
# End Source File
# Begin Source File

SOURCE=..\Shared\XmlFile.h
# End Source File
# Begin Source File

SOURCE=..\Shared\xmlfileex.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\XmlNodeWrapper.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\XNamedColors.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\XPTabCtrl.h
# End Source File
# End Group
# Begin Group "Resource Files"

# PROP Default_Filter "ico;cur;bmp;dlg;rc2;rct;bin;rgs;gif;jpg;jpeg;jpe"
# Begin Source File

SOURCE=.\res\app_extra_metro.bmp
# End Source File
# Begin Source File

SOURCE=.\res\app_extra_std.bmp
# End Source File
# Begin Source File

SOURCE=.\res\app_std.ico
# End Source File
# Begin Source File

SOURCE=.\res\app_toolbar.bmp
# End Source File
# Begin Source File

SOURCE=.\res\app_toolbar_metro.bmp
# End Source File
# Begin Source File

SOURCE=.\res\app_toolbar_std.bmp
# End Source File
# Begin Source File

SOURCE=.\res\custattrib_toolbar.bmp
# End Source File
# Begin Source File

SOURCE=.\res\custattrib_toolbar_metro.bmp
# End Source File
# Begin Source File

SOURCE=.\res\custattrib_toolbar_std.bmp
# End Source File
# Begin Source File

SOURCE=.\res\English.gif
# End Source File
# Begin Source File

SOURCE=.\res\find_dialog_std.ico
# End Source File
# Begin Source File

SOURCE=.\res\find_toolbar.bmp
# End Source File
# Begin Source File

SOURCE=.\res\find_toolbar_metro.bmp
# End Source File
# Begin Source File

SOURCE=.\res\find_toolbar_std.bmp
# End Source File
# Begin Source File

SOURCE=.\res\list_view_std.ico
# End Source File
# Begin Source File

SOURCE=.\res\prefs_dlg_std.ico
# End Source File
# Begin Source File

SOURCE=.\res\shield.ico
# End Source File
# Begin Source File

SOURCE=.\res\src_control_std.bmp
# End Source File
# Begin Source File

SOURCE=.\res\style_para.ico
# End Source File
# Begin Source File

SOURCE=.\res\style_table.ico
# End Source File
# Begin Source File

SOURCE=.\res\style_wrap.ico
# End Source File
# Begin Source File

SOURCE=.\res\task_icons_std.bmp
# End Source File
# Begin Source File

SOURCE=.\res\task_tree_std.ico
# End Source File
# Begin Source File

SOURCE=.\res\ToDoList.rc2
# End Source File
# Begin Source File

SOURCE=.\res\tray_std.ico
# End Source File
# Begin Source File

SOURCE=.\res\tray_tracking_std.ico
# End Source File
# Begin Source File

SOURCE=.\RES\XPcheckboxes.bmp
# End Source File
# Begin Source File

SOURCE=.\res\YourLanguage.GIF
# End Source File
# End Group
# Begin Group "Resources"

# PROP Default_Filter "tdl;xsl"
# Begin Group "Misc"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Resources\Misc\todolist_schema.xsd
# End Source File
# Begin Source File

SOURCE=.\Resources\Misc\todolist_simple.xsd
# End Source File
# Begin Source File

SOURCE=.\Resources\Misc\todolist_xml_schema.txt
# End Source File
# End Group
# Begin Group "Stylesheets"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Resources\Stylesheets\CalcTime_Spent_eng.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\Done_Sort_eng.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\Due_Sort_eng.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\flat_sorted_todolist.xsl
# End Source File
# Begin Source File

SOURCE=".\Resources\Stylesheets\Project-Overview-HTML.xsl"
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\SimpStyler0.2.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\Start_Sort_eng.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\TDLProjectSummary.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\Time_Spent_eng.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\Time_Spent_xls_eng.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\ToDoList_New.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\ToDoList_report.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\TodoListStyler_Firefox.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\TodoListStyler_v1.5.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\ToDoListTableStylesheet_v1.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\Z_DetailedReport.xsl
# End Source File
# Begin Source File

SOURCE=.\Resources\Stylesheets\Z_SimpleReport.xsl
# End Source File
# End Group
# Begin Group "Tasklists"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Resources\TaskLists\Introduction.tdl
# End Source File
# Begin Source File

SOURCE=.\Resources\TaskLists\ToDoListDocumentation.tdl
# End Source File
# End Group
# Begin Group "Themes"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Resources\Themes\ThemeBeige.xml
# End Source File
# Begin Source File

SOURCE=.\Resources\Themes\ThemeBlue.xml
# End Source File
# Begin Source File

SOURCE=.\Resources\Themes\ThemeBlueXP.xml
# End Source File
# Begin Source File

SOURCE=.\Resources\Themes\ThemeGray.xml
# End Source File
# Begin Source File

SOURCE=.\Resources\Themes\ThemeGreen.xml
# End Source File
# Begin Source File

SOURCE=.\Resources\Themes\ThemeGreenXP.xml
# End Source File
# Begin Source File

SOURCE=.\Resources\Themes\ThemePurple.xml
# End Source File
# Begin Source File

SOURCE=.\Resources\Themes\ThemeSteel.xml
# End Source File
# Begin Source File

SOURCE=.\Resources\Themes\ThemeVS2010.xml
# End Source File
# Begin Source File

SOURCE=.\Resources\Themes\ThemeWin7Basic.xml
# End Source File
# End Group
# Begin Group "Scripts"

# PROP Default_Filter ""
# End Group
# Begin Group "Translations"

# PROP Default_Filter ""
# Begin Source File

SOURCE=".\Resources\Translations\Français (France).csv"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Français (France).gif"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\German (Germany).csv"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\German (Germany).gif"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Italiano (Italia).csv"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Italiano (Italia).gif"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Japanese (Japan).csv"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Japanese (Japan).gif"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Nederlands (Belgium).csv"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Nederlands (Belgium).gif"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Russian (Russia).csv"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Russian (Russia).gif"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Simplified Chinese (China).csv"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Simplified Chinese (China).gif"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Slovak (Slovensko).csv"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Slovak (Slovensko).gif"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Spanish (Spain).csv"
# End Source File
# Begin Source File

SOURCE=".\Resources\Translations\Spanish (Spain).gif"
# End Source File
# Begin Source File

SOURCE=.\Resources\Translations\YourLanguage.csv
# End Source File
# End Group
# End Group
# Begin Group "Binaries"

# PROP Default_Filter ""
# Begin Group "Unicode_Debug"

# PROP Default_Filter "exe;dll"
# Begin Source File

SOURCE=.\Unicode_Debug\install.bmp
# End Source File
# Begin Source File

SOURCE=.\Unicode_Debug\install.ico
# End Source File
# Begin Source File

SOURCE=.\Unicode_Debug\Itenso.Rtf.Converter.Html.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Debug\Itenso.Rtf.Interpreter.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Debug\Itenso.Rtf.Parser.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Debug\Itenso.Solutions.Community.Rtf2Html.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Debug\Itenso.Sys.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Debug\Rtf2HtmlBridge.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Debug\WebUpdateSvc.exe
# End Source File
# Begin Source File

SOURCE=.\Unicode_Debug\WebUpdateSvc.LIC
# End Source File
# End Group
# Begin Group "Unicode_Release"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Unicode_Release\install.bmp
# End Source File
# Begin Source File

SOURCE=.\Unicode_Release\install.ico
# End Source File
# Begin Source File

SOURCE=.\Unicode_Release\Itenso.Rtf.Converter.Html.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Release\Itenso.Rtf.Interpreter.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Release\Itenso.Rtf.Parser.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Release\Itenso.Solutions.Community.Rtf2Html.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Release\Itenso.Sys.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Release\Rtf2HtmlBridge.dll
# End Source File
# Begin Source File

SOURCE=.\Unicode_Release\WebUpdateSvc.exe
# End Source File
# Begin Source File

SOURCE=.\Unicode_Release\WebUpdateSvc.LIC
# End Source File
# End Group
# End Group
# Begin Source File

SOURCE=.\RES\todolist.exe.manifest
# End Source File
# End Target
# End Project
# Section ToDoList : {8856F961-340A-11D0-A96B-00C04FD705A2}
# 	2:21:DefaultSinkHeaderFile:webbrowserctrl.h
# 	2:16:DefaultSinkClass:CWebBrowserCtrl
# End Section
# Section ToDoList : {D30C1661-CDAF-11D0-8A3E-00C04FC9E26E}
# 	2:5:Class:CWebBrowserCtrl
# 	2:10:HeaderFile:webbrowserctrl.h
# 	2:8:ImplFile:webbrowserctrl.cpp
# End Section
