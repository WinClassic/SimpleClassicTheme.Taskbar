@echo off
echo %~0
echo %~1
echo %~2
echo %~3
echo %~4
echo %~5
echo Packaging binaries..
REM Release-x64-sctt-1.0.7421.31289-.7z

REM ============================================================
REM Get Version
REM ============================================================
set command=""
if %~3==x86 set command=""%~2tools\sigcheck64" -n %1 -nobanner"
if %~3==x64 set command=""%~2tools\sigcheck64" -n "%~2bin\x86\%~4\net5.0-windows\SimpleClassicThemeTaskbar.exe" -nobanner"
echo %command%
for /f "tokens=*" %%a in ('%command%') do @set ver=%%a

REM ============================================================
REM Change version if x64
echo %ver%
REM ============================================================
if %~3==x86 goto copy
"%~2tools\rcedit-x64" %1 --set-product-version "%ver%"
"%~2tools\rcedit-x64" %1 --set-file-version "%ver%"

REM ============================================================
REM Copy and pack files to appropriate directories
REM ============================================================
:copy
"%~2tools\7z.exe" a "%~2binaries\%~4-%~3-sctt-%ver%.zip" "%~5*.exe" "%~5*.dll" "%~5nl\" "%~5nl-NL\" "%~5ref\" >> nul
del "%userprofile%\OneDrive\Visual Studio Repositories\SimpleClassicThemeTaskbar\Binaries\%~4-%~3-sctt.zip" /q
copy "%~2binaries\%~4-%~3-sctt-%ver%.zip" "%userprofile%\OneDrive\Visual Studio Repositories\SimpleClassicThemeTaskbar\Binaries\%~4-%~3-sctt.zip"
REM copy "%~2binaries\%~4-%~3-sctt-%ver%.zip" "%~2binaries\latest\%~4-%~3-sctt.zip"
copy "%~2binaries\%~4-%~3-sctt-%ver%.zip" "%~2binaries\latest\Git Package\%~4\SimpleClassicThemeTaskbar_%~3.zip"
echo %ver% > "%~2binaries\latest\Git Package\%~4\version.txt"