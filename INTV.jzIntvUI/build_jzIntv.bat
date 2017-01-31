rem #########################################################################
rem # Machine-Specific Configuration for Building jzIntv in Windows         #
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:
rem #  %1 : Absolute path to the INTV.jzIntvUI project
rem #  %2 : Build output directory
rem #  %3 : Build target - if empty, 'jzIntv' is implied; 'clean' also works
rem #
rem # This batch file provides a bridge between the Microsoft Visual Studio
rem # project build system and the MSYS / MSYS2 environment used to build
rem # jzIntv in Windows. The actual build and clean steps are done via
rem # 'make' in the MSYS environment.
rem #
rem # NOTE: In addition to configuring the MSYS_PATH variable, you must
rem #       also configure custom.mak. See detailed notes in custom.mak.
rem #
rem # To build the jzIntv emulator in Windows, you will need to have a valid
rem # build environment (usually MSYS or MSYS2) that has a version of g++
rem # capable of building C++14 or newer.
rem #
rem # This feature has been tested using MSYS with gcc 4.9.3. It has not
rem # been verified with MSYS2, though it should not be substantially
rem # different.
rem #
rem # To enable the INTV.jzIntvUI project to build and clean jzIntv from
rem # locally available sources, you must define the following variable:
rem #   MSYS_PATH
rem # Also, you probably do not want to have any spaces in your path. ;)
rem #

 set MSYS_PATH=D:\Users\Steve\Projects\MinGW\msys\1.0\bin

rem # If MSYS_PATH is empty, there is nothing to do.
if [%MSYS_PATH%] == [] goto SkipBuild

rem # --------------------------------------------------------------------- #
rem # Here is where the magic happens. Using the path defined by MSYS_PATH,
rem # add the MSYS_PATH to the front of the existing PATH so tools can be
rem # found. The heavy lifting is all done in the build_jzIntv.mak makefile,
rem # which requires proper configuration to be done in custom.mak. Then,
rem # use sed to convert the project directory (passed into this batch file
rem # from the INTV.jzIntvUI project) to a POSIX-style path, stored in the
rem # the PROJ_DIR variable. This variable is used to invoke the make tool
rem # in the proper directory for executing build_jzIntv.mak.
rem #
@echo Building jzIntv via MSYS from this location: %MSYS_PATH%
@echo.
set PATH=%MSYS_PATH%;%PATH%
echo /%1 | sed -e 's/\\\\/\//g' -e 's/://' > _projdir.txt
set /p PROJ_DIR= <_projdir.txt
del _projdir.txt

echo /%2jzIntv/Win | sed -e 's/\\\\/\//g' -e 's/://' > _outdir.txt
set /p PROJ_OUT_DIR= <_outdir.txt
del _outdir.txt

bash -c "make -C%PROJ_DIR% -f build_jzIntv.mak SKIP_IF_JZINTV_EMPTY=1 CONTENT_DIR=%PROJ_OUT_DIR% %3"
set PROJ_DIR=
set PROJ_OUT_DIR=
exit

:SkipBuild
@echo build_jzIntv.bat: Local jzIntv build skipped. MSYS_PATH not set.
