# Microsoft Developer Studio Project File - Name="OutlookImpExp" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Dynamic-Link Library" 0x0102

CFG=OutlookImpExp - Win32 Unicode Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "OutlookImpExp.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "OutlookImpExp.mak" CFG="OutlookImpExp - Win32 Unicode Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "OutlookImpExp - Win32 Unicode Debug" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "OutlookImpExp - Win32 Unicode Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "OutlookImpExp - Win32 Unicode Debug"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "OutlookImpExp___Win32_Unicode_Debug"
# PROP BASE Intermediate_Dir "OutlookImpExp___Win32_Unicode_Debug"
# PROP BASE Ignore_Export_Lib 1
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Unicode_Debug"
# PROP Intermediate_Dir "Unicode_Debug"
# PROP Ignore_Export_Lib 1
# PROP Target_Dir ""
F90=df.exe
# ADD BASE CPP /nologo /MDd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_WINDLL" /D "_AFXDLL" /D "_MBCS" /D "_AFXEXT" /D "_EXPORTING" /Yu"stdafx.h" /FD /GZ /c
# ADD CPP /nologo /MDd /W3 /Gm /GX /ZI /Od /D "_WINDLL" /D "_AFXDLL" /D "_AFXEXT" /D "_EXPORTING" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /Yu"stdafx.h" /FD /GZ /c
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0xc09 /d "_DEBUG" /d "_AFXDLL"
# ADD RSC /l 0xc09 /d "_DEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 /nologo /subsystem:windows /dll /debug /machine:I386 /pdbtype:sept
# ADD LINK32 /nologo /subsystem:windows /dll /debug /machine:I386 /pdbtype:sept
# SUBTRACT LINK32 /pdb:none
# Begin Special Build Tool
SOURCE="$(InputPath)"
PostBuild_Cmds=mkdir ..\ToDoList	mkdir ..\ToDoList\unicode_debug	copy unicode_debug\outlookimpexp.dll ..\todolist\unicode_debug /y
# End Special Build Tool

!ELSEIF  "$(CFG)" == "OutlookImpExp - Win32 Unicode Release"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "OutlookImpExp___Win32_Unicode_Release"
# PROP BASE Intermediate_Dir "OutlookImpExp___Win32_Unicode_Release"
# PROP BASE Ignore_Export_Lib 1
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Unicode_Release"
# PROP Intermediate_Dir "Unicode_Release"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
F90=df.exe
# ADD BASE CPP /nologo /MD /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_WINDLL" /D "_AFXDLL" /D "_MBCS" /D "_AFXEXT" /D "_EXPORTING" /Yu"stdafx.h" /FD /c
# ADD CPP /nologo /MD /W3 /GX /O2 /D "_WINDLL" /D "_AFXDLL" /D "_AFXEXT" /D "_EXPORTING" /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /Yu"stdafx.h" /FD /c
# SUBTRACT CPP /Fr
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0xc09 /d "NDEBUG" /d "_AFXDLL"
# ADD RSC /l 0xc09 /d "NDEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 /nologo /subsystem:windows /dll /machine:I386
# ADD LINK32 /nologo /subsystem:windows /dll /map /machine:I386
# SUBTRACT LINK32 /pdb:none
# Begin Special Build Tool
SOURCE="$(InputPath)"
PostBuild_Cmds=mkdir ..\ToDoList	mkdir ..\ToDoList\unicode_release	copy unicode_release\outlookimpexp.dll ..\todolist\unicode_release /y
# End Special Build Tool

!ENDIF 

# Begin Target

# Name "OutlookImpExp - Win32 Unicode Debug"
# Name "OutlookImpExp - Win32 Unicode Release"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=..\Shared\checklistboxex.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\driveinfo.cpp
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

SOURCE=..\3rdParty\ggets.cpp
# PROP Exclude_From_Build 1
# End Source File
# Begin Source File

SOURCE=..\Shared\Localizer.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Misc.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\msoutl.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\OSVersion.cpp
# End Source File
# Begin Source File

SOURCE=.\OutlookExporter.cpp
# End Source File
# Begin Source File

SOURCE=.\OutlookImpExp.cpp
# End Source File
# Begin Source File

SOURCE=.\OutlookImpExp.rc
# End Source File
# Begin Source File

SOURCE=.\OutlookImportDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\OutlookImporter.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Regkey.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\RegUtil.cpp
# End Source File
# Begin Source File

SOURCE=.\StdAfx.cpp
# ADD CPP /Yc"stdafx.h"
# End Source File
# Begin Source File

SOURCE=..\3rdParty\StdioFileEx.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Themed.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\VersionInfo.cpp
# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl"
# Begin Source File

SOURCE=..\Shared\checklistboxex.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Misc.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\msoutl.h
# End Source File
# Begin Source File

SOURCE=.\OutlookExporter.h
# End Source File
# Begin Source File

SOURCE=.\OutlookImpExp.h
# End Source File
# Begin Source File

SOURCE=.\OutlookImportDlg.h
# End Source File
# Begin Source File

SOURCE=.\OutlookImporter.h
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

SOURCE=.\StdAfx.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Themed.h
# End Source File
# End Group
# Begin Group "Resource Files"

# PROP Default_Filter "ico;cur;bmp;dlg;rc2;rct;bin;rgs;gif;jpg;jpeg;jpe"
# Begin Source File

SOURCE=.\res\OutlookImpExp.rc2
# End Source File
# End Group
# Begin Source File

SOURCE=.\ReadMe.txt
# End Source File
# Begin Source File

SOURCE=.\res\todolist.exe.manifest
# End Source File
# End Target
# End Project
