Installation of program CSBP (c) 2023 cwkuehl.de

Generell wird die Installation mit einer aktuellen .NET-Runtime ausgeliefert.
Ausserdem muss GTK+ 3.24 installiert sein.

Installation fuer Windows (getestet fuer Windows 7 und 10):
1) GTK+ 3.24 kann beispielsweise bei https://github.com/tschoonj/GTK-for-Windows-Runtime-Environment-Installer/releases/download/2022-01-04/gtk3-runtime-3.24.31-2022-01-04-ts-win64.exe heruntergeladen werden.
2) Speichern der Batch-Datei als #InstallUpdateCsbp.cmd in einem leeren Verzeichnis, z.B. D:\Haushalt\CSBP.
3) Nur fuer Windows 7, weil Powershell zu alt ist: Es muss curl.zip von https://curl.se/download.html heruntergeladen werden. Die Datei curl.exe muss in das Verzeichnis kopiert werden.
4) Starten von cmd.exe.
5) Wechseln in das Verzeichnis:
 D:
 cd \Haushalt\CSBP
6) Ausfuehren der Datei #InstallUpdateCsbp.cmd.
7) Beim naechsten Mal starten mit #Csbp.cmd.
