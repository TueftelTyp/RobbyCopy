@echo off
chcp 65001 >nul
setlocal

set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe

if not exist "%CSC%" (
    echo Der C#-Compiler wurde nicht gefunden.
    echo Bitte .NET Framework installieren.
    pause
    exit /b 1
)

"%CSC%" /codepage:65001 /target:winexe /optimize+ /out:RobbyCopy.exe /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll RobbyCopy.cs

if errorlevel 1 (
    echo.
    echo Kompilierung fehlgeschlagen.
    echo Bitte sicherstellen, dass RobbyCopy.cs als UTF-8 gespeichert ist.
) else (
    echo.
    echo Fertig: RobbyCopy.exe
)

pause