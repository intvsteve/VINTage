rem #########################################################################
rem # Setup to execute make in bash                                         #
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:                                                            #
rem #  %1 : Absolute path to a Visual Studio project                        #
rem #                                                                       #
rem # This batch file should be called to set up the environment to run     #
rem # GNU make used in various VINTage and jzIntv projects. It is presumed  #
rem # that custom.mak has already been called to initialize MSYS_PATH.      #
rem # If MSYS_PATH is valid, this batch file prepends the path to the       #
rem # environment's PATH variable and converts the provided project path to #
rem # the proper form for use in MSYS. It also defines the BASH_OUTPUT      #
rem # variable, for use in capturing the output of the invocation of make   #
rem # in MSYS's bash. To extract the result of the call to make, call the   #
rem # cleanup_bash.bat batch file.                                          #
rem #                                                                       #
rem # --------------------------------------------------------------------- #
rem # Environment Variables Initialized in this Batch File:                 #
rem # --------------------------------------------------------------------- #
rem #   PROJ_DIR    : the %1 argument with '\' replaced by '/'              #
rem #   BASH_OUTPUT : the name of the output file to use when calling make  #
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
rem # If MSYS_PATH is empty, there is nothing to do.
rem # --------------------------------------------------------------------- #
if [%MSYS_PATH%] == [] goto Skip

rem # --------------------------------------------------------------------- #
rem # Here is where the magic happens. Using the path defined by MSYS_PATH,
rem # add the MSYS_PATH to the front of the existing PATH so MSYS tools can
rem # be found. The heavy lifting is all done in the makefile that is called
rem # by the caller of this batch function. Calling the make file requires
rem # proper configuration to be done in custom.mak file in the same
rem # directory as this one. Here, use sed to convert the project directory
rem # path (passed into this batch file from the Visual Studio project) to
rem # a POSIX-style path as required in MSYS. The trick is to store the
rem # path in the the PROJ_DIR variable. This variable is used to invoke
rem # the make tool in the proper directory for executing a makefile. The
rem # BASH_OUTPUT variable is defined to redirect the output from bash assoc
rem # well as by the cleanup_bash.bat code.
rem # --------------------------------------------------------------------- #
set PATH=%MSYS_PATH%;%PATH%
set INTV_TMP_PROJDIR=_projdir%RANDOM%.txt
echo /%1 | sed -e 's/\\\\/\//g' -e 's/://' > %INTV_TMP_PROJDIR%
set /p PROJ_DIR= <%INTV_TMP_PROJDIR%
del %INTV_TMP_PROJDIR%
set BASH_OUTPUT=%1_bashmakeresult%RANDOM%.txt

goto Done

:Skip
@echo MSYS_PATH not set.

:Done
