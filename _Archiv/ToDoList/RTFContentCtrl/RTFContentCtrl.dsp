# Microsoft Developer Studio Project File - Name="RTFContentCtrl" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Dynamic-Link Library" 0x0102

CFG=RTFContentCtrl - Win32 Unicode Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "RTFContentCtrl.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "RTFContentCtrl.mak" CFG="RTFContentCtrl - Win32 Unicode Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "RTFContentCtrl - Win32 Unicode Debug" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "RTFContentCtrl - Win32 Unicode Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "RTFContentCtrl - Win32 Unicode Debug"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "RTFContentCtrl___Win32_Unicode_Debug"
# PROP BASE Intermediate_Dir "RTFContentCtrl___Win32_Unicode_Debug"
# PROP BASE Ignore_Export_Lib 0
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Unicode_Debug"
# PROP Intermediate_Dir "Unicode_Debug"
# PROP Ignore_Export_Lib 1
# PROP Target_Dir ""
F90=df.exe
# ADD BASE CPP /nologo /MDd /W4 /Gm /GX /ZI /Od /D "_DEBUG" /D "WIN32" /D "_WINDOWS" /D "_MBCS" /D "_EXPORTING" /D "_WINDLL" /D "_AFXDLL" /Yu"stdafx.h" /FD /GZ /c
# ADD CPP /nologo /MDd /W4 /Gm /GX /ZI /Od /D "_EXPORTING" /D "_WINDLL" /D "_AFXDLL" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /Yu"stdafx.h" /FD /GZ /c
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0xc09 /d "_DEBUG" /d "_AFXDLL"
# ADD RSC /l 0xc09 /d "_DEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 msimg32.lib /nologo /subsystem:windows /dll /debug /machine:I386 /pdbtype:sept
# ADD LINK32 msimg32.lib /nologo /subsystem:windows /dll /debug /machine:I386 /pdbtype:sept
# SUBTRACT LINK32 /pdb:none
# Begin Special Build Tool
SOURCE="$(InputPath)"
PostBuild_Cmds=mkdir ..\ToDoList	mkdir ..\ToDoList\unicode_debug	copy unicode_debug\rtfcontentctrl.dll ..\todolist\unicode_debug /y
# End Special Build Tool

!ELSEIF  "$(CFG)" == "RTFContentCtrl - Win32 Unicode Release"

# PROP BASE Use_MFC 6
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "RTFContentCtrl___Win32_Unicode_Release"
# PROP BASE Intermediate_Dir "RTFContentCtrl___Win32_Unicode_Release"
# PROP BASE Ignore_Export_Lib 0
# PROP BASE Target_Dir ""
# PROP Use_MFC 6
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Unicode_Release"
# PROP Intermediate_Dir "Unicode_Release"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
F90=df.exe
# ADD BASE CPP /nologo /MD /W4 /GX /O2 /D "NDEBUG" /D "WIN32" /D "_WINDOWS" /D "_MBCS" /D "_EXPORTING" /D "_WINDLL" /D "_AFXDLL" /Yu"stdafx.h" /FD /c
# ADD CPP /nologo /MD /W4 /GX /O2 /D "_EXPORTING" /D "_WINDLL" /D "_AFXDLL" /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_UNICODE" /D "UNICODE" /Yu"stdafx.h" /FD /c
# SUBTRACT CPP /Fr
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0xc09 /d "NDEBUG" /d "_AFXDLL"
# ADD RSC /l 0xc09 /d "NDEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 msimg32.lib /nologo /subsystem:windows /dll /machine:I386
# ADD LINK32 msimg32.lib /nologo /subsystem:windows /dll /map /machine:I386
# SUBTRACT LINK32 /pdb:none
# Begin Special Build Tool
SOURCE="$(InputPath)"
PostBuild_Cmds=mkdir ..\ToDoList	mkdir ..\ToDoList\unicode_release	copy unicode_release\rtfcontentctrl.dll ..\todolist\unicode_release /y
# End Special Build Tool

!ENDIF 

# Begin Target

# Name "RTFContentCtrl - Win32 Unicode Debug"
# Name "RTFContentCtrl - Win32 Unicode Release"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=..\Shared\AutoFlag.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\BinaryData.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ClipboardBackup.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ColourPopup.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Compression.cpp
# End Source File
# Begin Source File

SOURCE=.\CreateFileLinkDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\driveinfo.cpp
# End Source File
# Begin Source File

SOURCE=.\EditWebLinkDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnBitmap.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnBitmapEx.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\enfiledialog.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnMenu.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\EnString.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\entoolbar.cpp
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

SOURCE=..\Shared\HoldRedraw.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ImageProcessors.cpp
# End Source File
# Begin Source File

SOURCE=.\InsertTableDlg.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Localizer.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\MenuIconMgr.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Misc.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\msoutl.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\msword.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\MSWordHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\OSVersion.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\OutlookHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\Preferences.cpp
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

SOURCE=.\RRECRuler.cpp
# End Source File
# Begin Source File

SOURCE=.\RRECToolbar.cpp
# End Source File
# Begin Source File

SOURCE=.\RTFContentControl.cpp
# End Source File
# Begin Source File

SOURCE=.\RTFContentCtrl.cpp
# End Source File
# Begin Source File

SOURCE=.\RTFContentCtrl.rc
# End Source File
# Begin Source File

SOURCE=.\RulerRichEdit.cpp
# End Source File
# Begin Source File

SOURCE=.\RulerRichEditCtrl.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\SizeComboBox.cpp
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

SOURCE=..\Shared\Themed.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\ToolbarHelper.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\urlricheditctrl.cpp
# End Source File
# Begin Source File

SOURCE=..\3rdParty\rtf2html\Util.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\VersionInfo.cpp
# End Source File
# Begin Source File

SOURCE=..\Shared\WinClasses.cpp
# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl"
# Begin Source File

SOURCE=..\Shared\AutoFlag.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ClipboardBackup.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\ColourPopup.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Compression.h
# End Source File
# Begin Source File

SOURCE=.\CreateFileLinkDlg.h
# End Source File
# Begin Source File

SOURCE=.\EditWebLinkDlg.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnBitmap.h
# End Source File
# Begin Source File

SOURCE=..\Shared\EnBitmapEx.h
# End Source File
# Begin Source File

SOURCE=..\Shared\enfiledialog.h
# End Source File
# Begin Source File

SOURCE=..\Shared\entoolbar.h
# End Source File
# Begin Source File

SOURCE=..\Shared\FILEMISC.H
# End Source File
# Begin Source File

SOURCE=..\3rdParty\FontComboBox.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\FontPreviewCombo.h
# End Source File
# Begin Source File

SOURCE=..\Shared\HoldRedraw.h
# End Source File
# Begin Source File

SOURCE=..\Shared\IContentControl.h
# End Source File
# Begin Source File

SOURCE=.\ids.h
# End Source File
# Begin Source File

SOURCE=.\InsertTableDlg.h
# End Source File
# Begin Source File

SOURCE=..\Shared\IPreferences.h
# End Source File
# Begin Source File

SOURCE=..\Shared\MenuIconMgr.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Misc.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\msword.h
# End Source File
# Begin Source File

SOURCE=..\Shared\MSWordHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Preferences.h
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

SOURCE=..\Shared\RichEditHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\RichEditNcBorder.h
# End Source File
# Begin Source File

SOURCE=.\RRECRuler.h
# End Source File
# Begin Source File

SOURCE=.\RRECToolbar.h
# End Source File
# Begin Source File

SOURCE=.\RTFContentControl.h
# End Source File
# Begin Source File

SOURCE=.\RTFContentCtrl.h
# End Source File
# Begin Source File

SOURCE=.\RulerRichEdit.h
# End Source File
# Begin Source File

SOURCE=.\RulerRichEditCtrl.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Schemadef.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\SizeComboBox.h
# End Source File
# Begin Source File

SOURCE=.\StdAfx.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Subclass.h
# End Source File
# Begin Source File

SOURCE=..\Shared\Themed.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Tmschema.h
# End Source File
# Begin Source File

SOURCE=..\Shared\ToolbarHelper.h
# End Source File
# Begin Source File

SOURCE=..\Shared\urlricheditctrl.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\rtf2html\Util.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Uxtheme.h
# End Source File
# Begin Source File

SOURCE=..\Shared\wclassdefines.h
# End Source File
# Begin Source File

SOURCE=..\Shared\WinClasses.h
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Zlib\ZCONF.H
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Zlib\ZLIB.H
# End Source File
# End Group
# Begin Group "Resource Files"

# PROP Default_Filter "ico;cur;bmp;dlg;rc2;rct;bin;rgs;gif;jpg;jpeg;jpe"
# Begin Source File

SOURCE=.\res\RTFContentCtrl.rc2
# End Source File
# Begin Source File

SOURCE=.\toolbar.bmp
# End Source File
# Begin Source File

SOURCE=.\toolbar2.bmp
# End Source File
# End Group
# Begin Group "Libs"

# PROP Default_Filter "lib"
# Begin Source File

SOURCE=..\3rdParty\Zlib\ZLIBSTAT.LIB
# End Source File
# Begin Source File

SOURCE=..\3rdParty\Zlib\zlibstatD.lib
# End Source File
# End Group
# Begin Source File

SOURCE=.\ReadMe.txt
# End Source File
# End Target
# End Project
