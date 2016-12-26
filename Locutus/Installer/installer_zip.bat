rem #########################################################################
rem # Machine-Specific Setup for ZIPing Installer setup files in Windows    #
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:
rem #  %1 : Absolute path to the Visual Studio project invoking this file
rem #  %2 : The Visual Studio solution name, used to identify target
rem #
rem # This batch file provides a bridge between the Microsoft Visual Studio
rem # project build system and the MSYS / MSYS2 environment used to build
rem # a zip file of an installer setup.exe file in Windows. The actual build
rem # and clean steps are done via 'make' in the MSYS environment.
rem #
rem # To create the zip file in Windows, you will need to have a valid
rem # build environment (usually MSYS or MSYS2) that has a version of zip
rem # installed in the MinGW environment.
rem #
rem # To enable the Installer.zip project to build and clean the zip
rem # file, you must define the following variable: MSYS_PATH
rem # Also, you probably do not want to have any spaces in your path. ;)
rem #

set MSYS_PATH=D:\Users\Steve\Projects\MinGW\msys\1.0\bin

rem # If MSYS_PATH is empty, there is nothing to do.
if [%MSYS_PATH%] == [] goto SkipBuild

rem # --------------------------------------------------------------------- #
rem # Here is where the magic happens. Using the path defined by MSYS_PATH,
rem # add the MSYS_PATH to the front of the existing PATH so tools can be
rem # found. The heavy lifting is all done in the installer_zip.mak makefile.
rem # Use sed to convert the project directory (passed into this batch file
rem # from the Installer.zip project) to a POSIX-style path, stored in the
rem # the PROJ_DIR variable. This variable is used to invoke the make tool
rem # in the proper directory for executing installer_zip.mak. The TARGET_NAME
rem # variable is used to discern which installer output file to use - one
rem # for the desktop environment (Windows Vista or later), or Windows xp.
rem #
rem # NOTE: If / when installers are no longer built using InstallShield LE,
rem #       but rather WIX or something else instead, the makefile will need
rem #       to be updated for the inevitable change to the relative paths.
rem #
@echo Building jzIntv Tools via MSYS from this location: %MSYS_PATH%
@echo.
set PATH=%MSYS_PATH%;%PATH%
echo /%1 | sed -e 's/\\\\/\//g' -e 's/://' > _projdir.txt
set /p PROJ_DIR= <_projdir.txt
del _projdir.txt
bash -c "make -C%PROJ_DIR% -f installer_zip.mak TARGET_NAME=%2 %3"
set PROJ_DIR=
exit

:SkipBuild
@echo installer_zip.bat: Local installer zip skipped. MSYS_PATH not set.
