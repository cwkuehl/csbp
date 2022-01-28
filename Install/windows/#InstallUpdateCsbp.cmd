:: Installation and update for program CSBP (c) 2022 cwkuehl.de
@echo off

if exist Temp goto Download1
md Temp

:Download1
if exist Jhh-1.0.jar goto Download2
echo 1. Installation
call :Download
goto Download3

:Download2
echo Update
call :Download
del Leer*.*

:Download3

:Pruefen1
if exist log4j.properties goto Pruefen2
copy Leerlog4j.properties log4j.properties

:Pruefen2
if exist ServerConfig.properties goto Pruefen3
cd >cd.txt
gawk-3.1.6.exe "BEGIN{getline p<\"cd.txt\";gsub(/\\/,\"/\",p);}{sub(/jdbc:hsqldb:file:~\/hsqldb\/JHH6/,\"jdbc:hsqldb:file:\"p\"/hsqldb/JHH6\");print}" LeerServerConfig.properties >ServerConfig.properties
del cd.txt

:Pruefen3
if exist hsqldb goto Pruefen4
md hsqldb

:Pruefen4
if exist hsqldb\JHH6.properties goto Ende
copy LeerJHH6.properties hsqldb\JHH6.properties
copy LeerJHH6.script hsqldb\JHH6.script
goto Ende

:Download
  echo Download
  if exist temp\zip rmdir /s/q temp\zip
  mkdir temp\zip
  powershell -command "& { (New-Object Net.WebClient).DownloadFile('http://cwkuehl.de/wp-content/uploads/2018/02/Jhh-6_0.zip', 'temp\zip\jhh6.zip') }"
  echo Unzip
  powershell -command "& { function unzip($filename) { if (!(test-path $filename)) { throw \"$filename does not exist\"; } $shell = new-object -com shell.application; $shell.namespace($pwd.path).copyhere($shell.namespace((join-path $pwd $filename)).items(), 0x14); } unzip('temp\zip\jhh6.zip'); }"
  del temp\zip\jhh6.zip
  rmdir /s/q temp\zip
goto :eof

:Ende
del *.sh
call #Csbp.cmd
:: pause
