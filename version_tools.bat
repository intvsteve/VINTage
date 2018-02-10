rem #############################################################################
rem # Project Version Tools                                                     #
rem # ------------------------------------------------------------------------- #
rem # ARGUMENTS:                                                                #
rem #  %1 : working directory containing this batch file                        #
rem #  %2 : Build target                                                        #
rem #  %3 : value for ALLOW_LOCAL_CHANGES argument                              #
rem #                                                                           #
rem # This batch file does just enough to run version_tools.mak in MSYS.        #
rem #                                                                           #
rem #############################################################################

rem # ------------------------------------------------------------------------- #
rem # This batch file must run in the root directory.
rem # ------------------------------------------------------------------------- #
cd %1
call custom_bat_rule.bat

if [%MSYS_PATH%] == [] goto SkipBuild

rem # ------------------------------------------------------------------------- #
rem # Run the makefile to do the actual version-related work.
rem # ------------------------------------------------------------------------- #
call setup_bash %1
bash -c "make -C%PROJ_DIR% -f version_tools.mak %2 ALLOW_LOCAL_CHANGES=%3 ; echo $?" > %BASH_OUTPUT%
call cleanup_bash
exit %MAKE_RESULT%

:SkipBuild
@echo version_tools.bat: Local version update skipped. MSYS_PATH not set.
