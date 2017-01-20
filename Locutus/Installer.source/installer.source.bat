rem #########################################################################
rem # Machine-Specific Setup to ZIPing source from Source Control in Windows#
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:
rem #  %1 : Absolute path to the Visual Studio project invoking this file
rem #  %2 : The Visual Studio solution name, used to identify target
rem #
rem # This batch file provides a bridge between the Microsoft Visual Studio
rem # project build system and the MSYS / MSYS2 environment that is used
rem # to create a zip file containng the source code. Whether MSYS is
rem # required depends on the source control system in use and whether
rem # you feel like either creating an nmake file, or rewriting this .bat
rem # file to do everything directly.
rem #
rem # As this was originally written, to create the zip file in Windows,
rem # you will need to have a supported source control environment.
rem # Presently, this means GitHub or SVN.
rem #
rem # To enable the Installer.source project to build and clean the zip
rem # file, you must define the following variables:
rem #   MSYS_PATH: the path for your MSYS environment
rem #   GIT_REPO: the URL of the source in a GitHub repo
rem #   SVN_REPO: The URL of the source in a SVN repo
rem #
rem # Also, you probably do not want to have any spaces in your path. ;)
rem #

rem # MSYS_PATH is required for make and zip. The rest is up to you.
set MSYS_PATH=D:\Users\Steve\Projects\MinGW\msys\1.0\bin

if [%MSYS_PATH%] == [] goto SkipBuild

rem # If you specify a GitHub repo, a svn export of the repo is created.
set GIT_REPO=https://github.com/intvsteve/VINTage/trunk

rem # If GIT_REPO is empty, fall through to the SVN repo.
if [%GIT_REPO%] == [] goto TrySVN
:GitExport
goto RunMake

:TrySVN
rem # If you specify an SVN repo, it will be exported to a .zip file.
rem set SVN_REPO=svn://......

if [%SVN_REPO%] == [] goto SkipBuild

:RunMake
rem # --------------------------------------------------------------------- #
rem # Here is where the magic happens. Using the path defined by MSYS_PATH,
rem # add the MSYS_PATH to the front of the existing PATH so tools can be
rem # found. The heavy lifting is all done in installer.source.mak.
rem # Use sed to convert the project directory (passed into this batch file
rem # from the Installer.source project) to a POSIX-style path, stored in the
rem # the PROJ_DIR variable. This variable is used to invoke the make tool
rem # in the proper directory for executing installer.source.mak.
rem #
@echo Creating Source ZIP via MSYS from this location: %MSYS_PATH%
@echo.
set PATH=%MSYS_PATH%;%PATH%
echo /%1 | sed -e 's/\\\\/\//g' -e 's/://' > _projdir.txt
set /p PROJ_DIR= <_projdir.txt
del _projdir.txt
bash -c "make -C%PROJ_DIR% -f installer.source.mak %3"
set PROJ_DIR=
exit

:SkipBuild
@echo installer.source.bat: Local installer source zip skipped.
if [%MSYS_PATH%] == [] @echo MSYS_PATH not set!
if NOT [%MSYS_PATH%] == [] @echo   MSYS_PATH: %MSYS_PATH%
if [%GIT_REPO%] == [] (if [%SVN_REPO%] == [] (@echo Either GIT_REPO or SVN_REPO must be defined!))
@echo   GIT_REPO: %GIT_REPO%
@echo   SVN_REPO: %SVN_REPO%
