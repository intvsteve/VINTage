rem #########################################################################
rem # Batch file for Building jzIntv Tools in Windows                       #
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:                                                            #
rem #  %1 : Absolute path to the INTV.jzIntv project                        #
rem #  %2 : Build target - if empty, 'all' is implied; 'clean' also works   #
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
rem # To build the jzIntv tools in Windows, you will need to have a valid   #
rem # build environment (usually MSYS or MSYS2) that has a version of g++   #
rem # capable of building C++14 or newer.                                   #
rem #                                                                       #
rem # This feature has been tested using MSYS with gcc 6.3.0. It has not    #
rem # been verified with MSYS2, though it should not be substantially       #
rem # different.                                                            #
rem #                                                                       #
rem # To enable the INTV.jzIntv project to build and clean the jzIntv tools #
rem # that it uses from locally available sources, rather than use those    #
rem # included in the source distribution, you must define the following    #
rem # variable:                                                             #
rem #   MSYS_PATH                                                           #
rem #                                                                       #
rem # Also, you probably do not want to have any spaces in your path. ;)    #
rem #                                                                       #
rem #########################################################################

rem # --------------------------------------------------------------------- #
rem # Pull in local configuration to enable the build. This also defines
rem # the PROJ_DIR variable used to set the working directory for make.
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
set BUILD_GOAL_TEXT = Building
if [%2] == [clean] (@echo Cleaning jzIntv Tools via MSYS from this location: %MSYS_PATH%) else (@echo Building jzIntv Tools via MSYS from this location: %MSYS_PATH%)

rem # --------------------------------------------------------------------- #
rem # The actual build is done via the build_tools.mak makefile, which
rem # requires proper configuration to be done in custom.bat (called via
rem # the call of custom_bat_rule.bat above) as well as custom.mak.
rem # --------------------------------------------------------------------- #
call setup_bash.bat %1
bash -c "make -C%PROJ_DIR% -f build_tools.mak %2 SKIP_IF_JZINTV_EMPTY=1 ; echo $?" > %BASH_OUTPUT%
call cleanup_bash.bat
exit %MAKE_RESULT%

:SkipBuild
@echo build_tools.bat: Local jzIntv Tools build skipped. MSYS_PATH not set.
