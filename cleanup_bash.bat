rem #########################################################################
rem # Clean up the file defined in setup_bash.bat and get make result.      #
rem # --------------------------------------------------------------------- #
rem # This batch file "parses" the output from a call to bash -c "make ..." #
rem # that is assumed to redirect output to BASH_OUTPUT, including the      #
rem # return value from the call to make.                                   #
rem #                                                                       #
rem # This batch file must  be called after using bash to call make. After  #
rem # retrieving the return value from make, the BASH_OUTPUT file is        #
rem # deleted. The result is stored in MAKE_RESULT.                         #
rem #                                                                       #
rem # --------------------------------------------------------------------- #
rem # Environment Variables Initialized in this Batch File:                 #
rem #   MAKE_RESULT : the exit code from the call to make (via bash)        #
rem #                                                                       #
rem # --------------------------------------------------------------------- #
rem # Example:                                                              #
rem #                                                                       #
rem # call setup_bash.bat c:\VINTage\project\path                           #
rem # <batch file contents>                                                 #
rem # call cleanup_bash.bat                                                 #
rem #                                                                       #
rem #########################################################################

rem # --------------------------------------------------------------------- #
rem # Echo the contents of the BASH_OUTPUT file and retain the last line of
rem # the file as the return value from make. This assumes that make is
rem # run in the following manner:
rem #
rem # bash -c "make -C%PROJ_DIR% -f <makefile> ; echo $?" > %BASH_OUTPUT%
rem #
rem # --------------------------------------------------------------------- #
setlocal EnableDelayedExpansion
for /f "delims=" %%x in (%BASH_OUTPUT%) do (
  set "DONTCARE=%MAKE_RESULT%"
  set "MAKE_RESULT=%%x"
  echo %%x
)
del %BASH_OUTPUT%
endlocal & set MAKE_RESULT=%MAKE_RESULT%