@echo off
:loop
cls
echo Searching for com.* folder in the parent directory...

set "changelog_found=0"
for /d %%i in ("..\com.*") do (
    if exist "%%i\CHANGELOG.md" (
        echo Found CHANGELOG.md in %%i
        copy "%%i\CHANGELOG.md" . /Y
        set "changelog_found=1"
        goto :changelog_copied
    )
)

:changelog_copied
if %changelog_found%==0 (
    echo No com.* folder with CHANGELOG.md found in the parent directory.
)

echo Running DocFxTocGenerator and DocFX Build
DocFxTocGenerator -d .\manual -vs
docfx build --serve

echo.
echo Press Enter to run again or Ctrl + C to exit.
pause >nul

goto loop
