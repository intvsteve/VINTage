rem #########################################################################
rem # Copy local build of jzIntv to application build output in Windows     #
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:                                                            #
rem #  %1 : Absolute path to the LtoFlash project                           #
rem #  %2 : Build output path of the LtoFlash project                       #
rem #                                                                       #
rem # This batch file simply copies the jzIntv directory from the           #
rem # INTV.jzIntvUI project's jzIntv\Win directory to the build output      #
rem # directory if the jzIntv.exe file exists.                              #
rem #                                                                       #
rem #########################################################################

@echo Copying jzIntv to: %2jzIntv
if exist %1..\..\INTV.jzIntvUI\jzIntv\Win\bin\jzIntv.exe (
  mkdir %2jzIntv\Win
  xcopy /E /Y %1..\..\INTV.jzIntvUI\jzIntv\Win %2jzIntv\Win
  del %2jzIntv\Win\bin\.readme.txt
  del %2jzIntv\Win\doc\.readme.txt
) else (
  @echo Nothing to be copied.
)
