rem #########################################################################
rem # Batch file for Building jzIntv in Windows                             #
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:                                                            #
rem #  %1 : Absolute path to the INTV.jzIntvUI project                      #
rem #  %2 : Build output directory                                          #
rem #  %3 : Build target - if empty, jzIntv is implied; 'clean' also works  #
rem #                                                                       #
rem # This batch file provides a bridge between the Microsoft Visual Studio #
rem # project build system and the MSYS / MSYS2 environment used to build   #
rem # jzIntv in Windows. The actual build and clean steps are done via      #
rem # 'make' in the MSYS environment.                                       #
rem #                                                                       #
rem # NOTE: In addition to configuring the MSYS_PATH variable, you must     #
rem #       also configure custom.mak. See detailed notes in custom.mak.    #
rem #                                                                       #
rem # NOTE: The MSYS_PATH variable is defined in the custom.bat file in the #
rem #       root directory containing the directory containing this         #
rem #       project. If the file is not present, a default version will be  #
rem #       created via custom_bat_rule.bat, which sets MSYS_PATH to empty. #
rem #                                                                       #
rem # To build the jzIntv emulator in Windows, you will need to have a      #
rem # valid build environment (usually MSYS or MSYS2) that has a version of #
rem # g++ capable of building C++14 or newer.                               #
rem #                                                                       #
rem # This feature has been tested using MSYS with gcc 6.3.0. It has not    #
rem # been verified with MSYS2, though it should not be substantially       #
rem # different.                                                            #
rem #                                                                       #
rem # To enable the INTV.jzIntvUI project to build and clean jzIntv from    #
rem # locally available sources, you must define the following variable:    #
rem #   MSYS_PATH                                                           #
rem #                                                                       #
rem # Also, you probably do not want to have any spaces in your path. ;)    #
rem #                                                                       #
rem #########################################################################

rem # --------------------------------------------------------------------- #
rem # Pull in local configuration to enable the build. This ensures the
rem # following environment variables get defined:
rem #   MSYS_PATH : path to the root of the MSYS system
rem #   PROJ_DIR  : sets the working directory for calling make in bash
rem
rem # --------------------------------------------------------------------- #
cd %1..
call custom_bat_rule.bat

rem # --------------------------------------------------------------------- #
rem # If MSYS_PATH is empty, there is nothing to do.
rem # --------------------------------------------------------------------- #
if [%MSYS_PATH%] == [] goto SkipBuild

rem # --------------------------------------------------------------------- #
rem # Friendly text.
rem # --------------------------------------------------------------------- #
if [%3] == [clean] (@echo Cleaning jzIntv via MSYS from this location: %MSYS_PATH%) else (@echo Building jzIntv via MSYS from this location: %MSYS_PATH%)

rem # --------------------------------------------------------------------- #
rem # The actual build is done in the build_jzIntv.mak makefile, which
rem # requires proper configuration to be done in custom.bat (called via
rem # the call of custom_bat_rule.bat above) as well as custom.mak. In
rem # addition, the output directory is "slash-converted" for MSYS.
rem # --------------------------------------------------------------------- #
call setup_bash.bat %1
set JZINTV_TMP_PROJDIR=_jzintvoutdir%RANDOM%.txt
echo /%2jzIntv/Win | sed -e 's/\\\\/\//g' -e 's/://' > %JZINTV_TMP_PROJDIR%
set /p PROJ_OUT_DIR= <%JZINTV_TMP_PROJDIR%
del %JZINTV_TMP_PROJDIR%
bash -c "make -C%PROJ_DIR% -f build_jzIntv.mak %3 SKIP_IF_JZINTV_EMPTY=1 CONTENT_DIR=%PROJ_OUT_DIR% ; echo $?" > %BASH_OUTPUT%
call cleanup_bash.bat
exit %MAKE_RESULT%

:SkipBuild
@echo build_jzIntv.bat: Local jzIntv build skipped. MSYS_PATH not set.
