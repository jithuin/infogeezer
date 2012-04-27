@echo off
@rem 2006.08.15 uj ISW107.cmd IDEXX-hez
@rem 2006.09.13 uj ISW108.cmd IDEXX-hez
@rem 2007.01.15 uj ISW109.cmd IDEXX-hez
@rem 2008.08.13 uj ISW111.cmd IDEXX-hez
SETLOCAL ENABLEDELAYEDEXPANSION

:_start
cls
echo koddugo verzio : RP3.bin
echo.

::==============================================
::   setting local variables
::==============================================

SET ROOTPATH=\\77ks\prg
SET EXEPATH=proj\asztali_keszulek\ux3\UX3-Software\Koddugo\IDEX_RP3
SET CMDPATH=proj\asztali_keszulek\ux3\UX3-Software\Koddugo\IDEX_RP3\BURNRP3.CMD
::SET CMDPATH2=d:\isw111.cmd

::SET PATH=%PATH%;"%EXEPATH%"



::===========================================
::           checking arguments
::===========================================

IF NOT EXIST %ROOTPATH%\%EXEPATH%\i2cgyart.exe GOTO exehiba
IF NOT EXIST %ROOTPATH%\%CMDPATH% GOTO cmdhiba
::IF NOT EXIST %CMDPATH2% GOTO cmdhiba

FOR /f "tokens=* delims= " %%s in (%ROOTPATH%\%CMDPATH%) do ( Call:get_path "%%s")
::FOR /f "tokens=* delims= " %%s in (%CMDPATH2%) do ( Call:get_path "%%s")
SET BINPATH=%result%
IF NOT EXIST %BINPATH% GOTO binhiba

::===========================================
::   Running application
::===========================================

cd \
cd "%EXEPATH%"
i2cgyart.exe /b /f=%ROOTPATH%\%CMDPATH%
::i2cgyart.exe /b /f=%CMDPATH2%
if errorlevel 7 goto fajl_hiba
if errorlevel 6 goto:EOF
if errorlevel 1 goto hiba
pause
goto:EOF

::============================================
::       Error options
::============================================

:hiba
 cls
 echo.
 echo.
 echo    Ü  Ü Ü ÜÜÜ    Ü   Ü Ü
 echo    ÛÜÜÛ Û ÛÜÜß Üß ßÜ Û Û
 echo    Û  Û Û ÛÜÜß ÛßßßÛ Ü Ü
 echo.
 echo.
 echo    A koddugo programozasa/ellenorzese alatt hiba lepett fel! 
 echo.
 echo    lehetseges okok: 
 echo      - hibas programozas : AZ EPPEN PROGRAMOZOTT KODDUGOT EL KELL KULONITENI!!
 echo.
 echo.
pause
 echo.
 echo.
 echo    Hibas programozas eseten a koddugot el kell kuloniteni! Megtortent?
 echo.
 echo.
 echo.
choice /c:vx   Kilepeshez usson 'V' betut!  Programozas folytatasahoz usson 'X' betut!
 if errorlevel 2 goto _start
 if errorlevel 1 goto:EOF

:fajl_hiba
ECHO.a szukséges fajlok nem elerhetok
pause
GOTO:EOF

:exehiba
ECHO.nem találom a %ROOTPATH%\%EXEPATH% fájlt
ECHO.Kérem ellenõrizze az elérési utat.
pause
GOTO:EOF

:cmdhiba
ECHO.nem találom a %ROOTPATH%\%CMDPATH% fájlt
ECHO.Kérem ellenõrizze az elérési utat.
pause
GOTO:EOF

:binhiba
echo.nem találom a %BINPATH% fájlt.
echo. Kérem adja meg fent említett fájl teljes elérési útját a %ROOTPATH%\%CMDPATH% fájlban a "File=" sorban
pause
GOTO:EOF

::==========================================================
::        Lokális függvények
::==========================================================

:get_path
set f=%~1

IF "File=" EQU "%f:~0,5%" ( 
::for /f "tokens=* delims=^= " %%a in ("%f%") DO set binpath=%%a
set result=%f:~5,1000%
)
GOTO:EOF


:_esc
goto:EOF
