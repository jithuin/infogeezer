
CALL :getCurrentDriveLetter

start %CURDRV%\Start.exe
start %CURDRV%\PortableApps\_Stuff\DecimalInternetClock.exe

::start .\bLadeWikiPortable\bLadeWikiPortable.bat
CALL :StartPortableApplication Ditto
::start .\DittoPortable\DittoPortable.exe
CALL :StartPortableApplication Executor
::start .\ExecutorPortable\ExecutorPortable.exe
CALL :StartPortableApplication Pidgin
::start .\PidginPortable\PidginPortable.exe
CALL :StartPortableApplication PNotes
::start .\PNotesPortable\PNotesPortable.exe
CALL :StartPortableApplication TaskCoach
::start .\TaskCoachPortable\TaskCoachPortable.exe
CALL :StartPortableApplication VirtuaWin
::start .\VirtuaWinPortable\VirtuaWinPortable.exe
::CALL :ssa WinSplit
::CALL :ssa PowerResizer
GOTO:EOF

::Get current drive letter and store it to CURDRV variable
:getCurrentDriveLetter
for /f "delims=\" %%d in ('cd') do set CURDRV=%%d
GOTO:EOF

::Start Portable Application
::This function suggest that the application directory and the filename has its certain naming convention
:: convetion:
::    dir  -  xyzPortable
::   file  -  xyzPortable.exe
:StartPortableApplication

cd %CURDRV%\PortableApps\%~1Portable
start .\%~1Portable.exe
cd..
GOTO:EOF

:: Start Default Application
::
::    1. argument : directory name
::    2. argument : filename
:::sda
::cd %~1
::start .\%~2
::cd..
::pause
::GOTO:EOF


::Start Similar Application 
::that have the similar file and directory name
::Remarks:
::(This function suggest that the application directory  
::   and the filename has its certain naming convention)
::
:: convetion:
::    dir  -  xyz
::   file  -  xyz.exe
:::ssa
::cd %~1
::start .\%~1.exe
::cd..
::GOTO:EOF