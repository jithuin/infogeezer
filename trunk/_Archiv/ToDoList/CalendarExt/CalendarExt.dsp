# Microsoft Developer Studio Project File - Name="CalendarExt" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Dynamic-Link Library" 0x0102

CFG=CalendarExt - Win32 Unicode Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "CalendarExt.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "CalendarExt.mak" CFG="CalendarExt - Win32 Unicode Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "CalendarExt - Win32 Unicode Debug" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "CalendarExt - Win32 Unicode Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "CalendarExt - Win32 Unicode Debug"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "CalendarExt___Win32_Unicode_Debug"
# PROP BASE Intermediate_Dir "CalendarExt___Win32_Unicode_Debug"
# PROP BASE Ignore_Export_Lib 0
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Unicode_Debug"
# PROP Intermediate_Dir "Unicode_Debug"
# PROP Ignore_Export_Lib 1
# PROP Target_Dir ""
F90=df.exe
# ADD BASE CPP /nologo /MDd /W3 /Gm /GX /Zi /Od /I "..\3rdParty\Calendar" /D "_DEBUG" /D "WIN32" /D "_WINDOWS" /D "_WINDLL" /D "_AFXDLL" /D "_MBCS" /D "_AFXEXT" /D "_EXPORTING" /D "NO_TL_ENCRYPTDECRYPT" /D "NO_TL_MERGE" /FR /Yu"stdafx.h" /FD /GZ /c
# ADD CPP /nologo /MDd /W3 /Gm /GX /Zi /Od /I "..\3rdParty\Calendar" /D "_WINDLL" /D "_AFXDLL" /D "_AFXEXT" /D "_EXPORTING" /D "NO_TL_ENCRYPTDECRYPT" /D "NO_TL_MERGE" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /FR /Yu"stdafx.h" /FD /GZ /c
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0xc09 /d "_DEBUG" /d "_AFXDLL"
# ADD RSC /l 0xc09 /d "_DEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 Msimg32.lib msimg32.lib /nologo /subsystem:windows /dll /incremental:no /debug /machine:I386 /pdbtype:sept
# ADD LINK32 Msimg32.lib msimg32.lib /nologo /subsystem:windows /dll /incremental:no /debug /machine:I386 /pdbtype:sept
# SUBTRACT LINK32 /pdb:none
# Begin Special Build Tool
SOURCE="$(InputPath)"
PostBuild_Cmds=mkdir ..\ToDoList	mkdir ..\ToDoList\unicode_debug	copy unicode_debug\CalendarExt.dll ..\todolist\unicode_debug /y
# End Special Build Tool

!ELSEIF  "$(CFG)" == "CalendarExt - Win32 Unicode Release"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "CalendarExt___Win32_Unicode_Release"
# PROP BASE Intermediate_Dir "CalendarExt___Win32_Unicode_Release"
# PROP BASE Ignore_Export_Lib 0
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Unicode_Release"
# PROP Intermediate_Dir "Unicode_Release"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
F90=df.exe
# ADD BASE CPP /nologo /MD /W3 /GX /O2 /I "..\3rdParty\Calendar" /D "NDEBUG" /D "WIN32" /D "_WINDOWS" /D "_WINDLL" /D "_AFXDLL" /D "_MBCS" /D "_AFXEXT" /D "_EXPORTING" /D "NO_TL_ENCRYPTDECRYPT" /D "NO_TL_MERGE" /Yu"stdafx.h" /FD /c
# ADD CPP /nologo /MD /W3 /GX /O2 /I "..\3rdParty\Calendar" /D "_WINDLL" /D "_AFXDLL" /D "_AFXEXT" /D "_EXPORTING" /D "NO_TL_ENCRYPTDECRYPT" /D "NO_TL_MERGE" /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /Yu"stdafx.h" /FD /c
# SUBTRACT CPP /Fr
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0xc09 /d "NDEBUG" /d "_AFXDLL"
# ADD RSC /l 0xc09 /d "NDEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 Msimg32.lib msimg32.lib /nologo /subsystem:windows /dll /machine:I386
# ADD LINK32 Msimg32.lib msimg32.lib /nologo /subsystem:windows /dll /map /machine:I386
# SUBTRACT LINK32 /pdb:none
# Begin Special Build Tool
SOURCE="$(InputPath)"
PostBuild_Cmds=mkdir ..\ToDoList	mkdir ..\ToDoList\unicode_release	copy unicode_release\CalendarExt.dll ..\todolist\unicode_release /y
# End Special Build Tool

!ENDIF 

# Begin Target

# Name "CalendarExt - Win32 Unicode Debug"
# Name "CalendarExt - Win32 Unicode Release"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=..\Shared\AutoFlag.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Base64Coder.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\BigCalendarCtrl.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\BigCalendarTask.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\BinaryData.cpp
# End Source File
# Begin Source File

SOURCE=.\CalendarButtonsDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\CalendarData.cpp
# End Source File
# Begin Source File

SOURCE=.\CalendarExt.cpp
# End Source File
# Begin Source File

SOURCE=.\CalendarExt.rc
# End Source File
# Begin Source File

SOURCE=.\CalendarPrefsDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\CalendarUtils.cpp
# End Source File
# Begin Source File

SOURCE=.\CalendarWnd.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\custombutton.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\DateHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\DialogHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\driveinfo.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnBitmap.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnMenu.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnString.cpp
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

SOURCE=..\Shared\FolderDialog.cpp
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

SOURCE=..\Shared\ImageProcessors.cpp
# End Source File
# Begin Source File

SOURCE=..\RTFContentCtrl\InsertTableDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Localizer.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\menubutton.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\MiniCalendarCtrl.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\MiniCalendarMonthPicker.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Misc.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\OSVersion.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Regkey.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\RegUtil.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\StatusBarACT.cpp
# End Source File
# Begin Source File

SOURCE=.\StdAfx.cpp
# ADD CPP /Yc"stdafx.h"
# End Source File
# Begin Source File

SOURCE=..\3rdParty\StdioFileEx.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Subclass.cpp
# End Source File
# Begin Source File

SOURCE=..\ToDoList\TaskFile.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\TaskInfo.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Themed.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\TimeHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\ToDoList\ToDoitem.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\TransparentListBox.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\VersionInfo.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\WinClasses.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\XmlFile.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\XmlNodeWrapper.cpp
# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl"
# Begin Source File

SOURCE=..\3rdParty\Calendar\BigCalendarCtrl.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\BigCalendarTask.h
# End Source File
# Begin Source File

SOURCE=.\CalendarButtonsDlg.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\CalendarData.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\CalendarDefines.h
# End Source File
# Begin Source File

SOURCE=.\CalendarExt.h
# End Source File
# Begin Source File

SOURCE=.\CalendarExtResource.h
# End Source File
# Begin Source File

SOURCE=.\CalendarPrefsDlg.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\CalendarUtils.h
# End Source File
# Begin Source File

SOURCE=.\CalendarWnd.h
# End Source File
# Begin Source File

SOURCE=..\Shared\DialogHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnMenu.h
# End Source File
# Begin Source File

SOURCE=..\RTFContentCtrl\InsertTableDlg.h
# End Source File
# Begin Source File

SOURCE=..\Shared\IUIExtension.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\MemDC.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\MiniCalendarCtrl.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\MiniCalendarMonthPicker.h
# End Source File
# Begin Source File

SOURCE=.\resource.h
# End Source File
# Begin Source File

SOURCE=.\StdAfx.h
# End Source File
# Begin Source File

SOURCE=..\ToDoList\TaskFile.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\TaskInfo.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Calendar\TransparentListBox.h
# End Source File
# Begin Source File

SOURCE=..\Shared\XmlCharMap.h
# End Source File
# End Group
# Begin Group "Resource Files"

# PROP Default_Filter "ico;cur;bmp;dlg;rc2;rct;bin;rgs;gif;jpg;jpeg;jpe"
# Begin Source File

SOURCE=.\res\CalendarExt.rc2
# End Source File
# Begin Source File

SOURCE=.\res\icon1.ico
# End Source File
# Begin Source File

SOURCE=.\res\minical_arrow_L.bmp
# End Source File
# Begin Source File

SOURCE=.\res\minical_arrow_R.bmp
# End Source File
# Begin Source File

SOURCE=.\res\toolbar1.bmp
# End Source File
# End Group
# Begin Source File

SOURCE=.\ReadMe.txt
# End Source File
# End Target
# End Project
