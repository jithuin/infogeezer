# Microsoft Developer Studio Project File - Name="GPExport" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Dynamic-Link Library" 0x0102

CFG=GPExport - Win32 Unicode Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "GPExport.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "GPExport.mak" CFG="GPExport - Win32 Unicode Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "GPExport - Win32 Unicode Debug" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "GPExport - Win32 Unicode Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "GPExport - Win32 Unicode Debug"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "GPExport___Win32_Unicode_Debug"
# PROP BASE Intermediate_Dir "GPExport___Win32_Unicode_Debug"
# PROP BASE Ignore_Export_Lib 1
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Unicode_Debug"
# PROP Intermediate_Dir "Unicode_Debug"
# PROP Ignore_Export_Lib 1
# PROP Target_Dir ""
F90=df.exe
# ADD BASE CPP /nologo /MDd /W4 /Gm /GX /ZI /Od /D "_DEBUG" /D "WIN32" /D "_WINDOWS" /D "_WINDLL" /D "_AFXDLL" /D "_MBCS" /D "_USRDLL" /D "_EXPORTING" /Yu"stdafx.h" /FD /GZ /c
# ADD CPP /nologo /MDd /W4 /Gm /GX /ZI /Od /D "_WINDLL" /D "_AFXDLL" /D "_USRDLL" /D "_EXPORTING" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /Yu"stdafx.h" /FD /GZ /c
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
PostBuild_Cmds=mkdir ..\ToDoList	mkdir ..\ToDoList\unicode_debug	copy unicode_debug\gpexport.dll ..\todolist\unicode_debug /y
# End Special Build Tool

!ELSEIF  "$(CFG)" == "GPExport - Win32 Unicode Release"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "GPExport___Win32_Unicode_Release"
# PROP BASE Intermediate_Dir "GPExport___Win32_Unicode_Release"
# PROP BASE Ignore_Export_Lib 1
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Unicode_Release"
# PROP Intermediate_Dir "Unicode_Release"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
F90=df.exe
# ADD BASE CPP /nologo /MD /W4 /GX /O2 /D "NDEBUG" /D "WIN32" /D "_WINDOWS" /D "_WINDLL" /D "_AFXDLL" /D "_MBCS" /D "_USRDLL" /D "_EXPORTING" /Yu"stdafx.h" /FD /c
# ADD CPP /nologo /MD /W4 /GX /O2 /D "_WINDLL" /D "_AFXDLL" /D "_USRDLL" /D "_EXPORTING" /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /Yu"stdafx.h" /FD /c
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
PostBuild_Cmds=mkdir ..\ToDoList	mkdir ..\ToDoList\unicode_release	copy unicode_release\gpexport.dll ..\todolist\unicode_release /y
# End Special Build Tool

!ENDIF 

# Begin Target

# Name "GPExport - Win32 Unicode Debug"
# Name "GPExport - Win32 Unicode Release"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=..\Shared\DateHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\driveinfo.cpp
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

SOURCE=.\GPExport.cpp
# End Source File
# Begin Source File

SOURCE=.\GPExport.rc
# End Source File
# Begin Source File

SOURCE=.\GPExporter.cpp
# End Source File
# Begin Source File

SOURCE=.\GPImporter.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Misc.cpp
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

SOURCE=..\Shared\TimeHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\VersionInfo.cpp
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

SOURCE=..\Shared\DateHelper.h
# End Source File
# Begin Source File

SOURCE=.\GPExport.h
# End Source File
# Begin Source File

SOURCE=.\GPExporter.h
# End Source File
# Begin Source File

SOURCE=.\GPImporter.h
# End Source File
# Begin Source File

SOURCE=..\Shared\HtmlCharMap.h
# End Source File
# Begin Source File

SOURCE=..\Shared\IImportExport.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ITaskList.h
# End Source File
# Begin Source File

SOURCE=.\Resource.h
# End Source File
# Begin Source File

SOURCE=.\StdAfx.h
# End Source File
# Begin Source File

SOURCE=..\Shared\stringres.h
# End Source File
# Begin Source File

SOURCE=..\Shared\XmlCharMap.h
# End Source File
# Begin Source File

SOURCE=..\Shared\XmlFile.h
# End Source File
# End Group
# Begin Group "Resource Files"

# PROP Default_Filter "ico;cur;bmp;dlg;rc2;rct;bin;rgs;gif;jpg;jpeg;jpe"
# Begin Source File

SOURCE=.\res\GPExport.rc2
# End Source File
# End Group
# Begin Source File

SOURCE=.\ReadMe.txt
# End Source File
# End Target
# End Project
