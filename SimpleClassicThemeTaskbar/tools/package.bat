@echo off
echo Packaging binaries..
REM Release-x64-sctt-1.0.7421.31289-.7z
set command=""%~2tools\sigcheck64" -n %1 -nobanner"
for /f "tokens=*" %%a in ('%command%') do @set ver=%%a
"%~2tools\7z.exe" a "%~2binaries\%~4-%~3-sctt-%ver%.zip" "%~5*.exe" "%~5*.dll" >> nul
del "C:\Users\Anis\OneDrive\Visual Studio Repositories\SimpleClassicThemeTaskbar\Binaries\%~4-%~3-sctt.zip" /q
copy "%~2binaries\%~4-%~3-sctt-%ver%.zip" "C:\Users\Anis\OneDrive\Visual Studio Repositories\SimpleClassicThemeTaskbar\Binaries\%~4-%~3-sctt.zip"
copy "%~2binaries\%~4-%~3-sctt-%ver%.zip" "%~2binaries\latest\%~4-%~3-sctt.zip"