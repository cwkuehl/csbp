:: Installation and update for program CSBP (c) 2022 cwkuehl.de
@echo off
if not "%~n0"=="#InstallUpdateCsbp" goto start
copy #InstallUpdateCsbp.cmd _.cmd
_.cmd
goto :eof

:Download
  echo Download
  if exist temp\zip rmdir /s/q temp\zip
  md temp\zip
  powershell -command "& { (New-Object System.Net.WebClient).DownloadFile('https://cwkuehl.de/wp-content/uploads/2022/01/csbp-net6-win-x64-runtime.zip', 'temp\zip\csbp.zip') }"
  :: powershell -command "& { invoke-webrequest -uri 'https://cwkuehl.de/wp-content/uploads/2022/01/csbp-net6-win-x64-runtime.zip' -outfile 'temp\zip\csbp.zip' }"
  if exist temp\zip\csbp.zip goto Unzip
    curl --insecure -o temp\zip\csbp.zip https://cwkuehl.de/wp-content/uploads/2022/01/csbp-net6-win-x64-runtime.zip
  :Unzip
  echo Unzip
  powershell -command "& { function unzip($filename) { if (!(test-path $filename)) { throw \"$filename does not exist\"; } $shell = new-object -com shell.application; $shell.namespace($pwd.path).copyhere($shell.namespace((join-path $pwd $filename)).items(), 0x14); } unzip('temp\zip\csbp.zip'); }"
  xcopy /e/h/y/q csbp .
  rmdir /s/q csbp
  del temp\zip\csbp.zip
  rmdir /s/q temp\zip
goto :eof

:start
for /F "tokens=1" %%i in ('date /t') do set mydate=%%i
echo %mydate% %time% Installation and update for program CSBP (c) 2022 cwkuehl.de >> Log.txt

if not exist Temp md Temp
call :Download
set DBNAME=
if exist %USERPROFILE%\Documents\csbp\csbp.db goto Update
if exist %USERPROFILE%\Documents\hsqldb\csbp.db goto Update

echo Install
echo %mydate% %time% Database initializing. >> Log.txt
if not exist %USERPROFILE%\Documents\csbp md %USERPROFILE%\Documents\csbp
set DBNAME=%USERPROFILE%\Documents\csbp\csbp.db
move EmptyCsbp.db %DBNAME%
del EmptyCsbp.db
goto Script

:Update
echo Update
echo %mydate% %time% Database exists. >> Log.txt
del EmptyCsbp.db

:Script
:: Generate start script and start CSBP
echo :: Start program CSBP (c) 2022 cwkuehl.de > #Csbp0.cmd
echo cd %~dp0\publish >> #Csbp0.cmd
if "%DBNAME%"=="" (
  echo start CSBP.exe >> #Csbp0.cmd
) else (
  echo start CSBP.exe "DB_DRIVER_CONNECT=Data Source=%DBNAME%" >> #Csbp0.cmd
)
if not exist #Csbp.cmd move #Csbp0.cmd #Csbp.cmd

:Ende
start cmd /k "#Csbp.cmd && exit"
start cmd /k "del _.cmd && exit"
