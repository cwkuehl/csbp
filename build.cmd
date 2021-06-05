@rem Installationsvoraussetzungen:
@rem Installation von
@rem VSCodeSetup-x64-1.51.1.exe oder neuer
@rem dotnet-sdk-5.0.100-win-x64.exe oder neuer
@rem GTK durch Starten im vscode.

@echo off
set src=%~dp0
echo %src%
set tgt=C:\Eigen\Haushalt\CSBP1\publish\
echo %tgt%
dotnet publish %src%CSBP\CSBP.csproj --configuration Release --framework net5.0 -p:Version=1.0.0.0

xcopy %src%CSBP\bin\Release\net5.0\publish\ %tgt% /s /e /c /i /r /y /q
xcopy %src%CSBP\Resources\ %tgt%Resources /s /e /c /i /r /y /q
del %tgt%Resources\T*.*
del %tgt%Resources\M*.*

echo cd %tgt% >%tgt%csbp.cmd
echo start CSBP.exe "DB_DRIVER_CONNECT=Data Source=%userprofile%\Documents\csbp\csbp.db" >>%tgt%csbp.cmd

pause