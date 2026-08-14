@echo off
chcp 65001 >nul
setlocal

set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe

if not exist "%CSC%" (
    echo C# compiler not found.
    echo Please install the .NET Framework.
    pause
    exit /b 1
)

"%CSC%" /codepage:65001 /target:winexe /optimize+ /out:RobbyCopy.exe /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll RobbyCopy.cs

if errorlevel 1 (
    echo.
    echo Compilation failed.
    echo Please make sure that RobbyCopy.cs is saved as UTF-8.
) else (
    echo.
    echo Done: RobbyCopy.exe
)

pause
