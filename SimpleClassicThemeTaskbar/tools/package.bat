@echo off
echo Packaging binaries..
set command=""%~2tools\sigcheck64" -n %1 -nobanner"
for /f "tokens=*" %%a in ('%command%') do @set ver=%%a
"%~2tools\7z.exe" a "%~2binaries\sctt-%~3-%ver%-%~4.7z" "%~5*.exe" "%~5*.dll" >> nul
del "%~2binaries\latest\sctt-%~3-*-%~4.7z" /q
copy "%~2binaries\sctt-%~3-%ver%-%~4.7z" "%~2binaries\latest\sctt-%~3-%ver%-%~4.7z"