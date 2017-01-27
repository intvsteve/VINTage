rem #########################################################################
rem # Clean local build of jzIntv from application build output in Windows  #
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:
rem #  %1 : Build output path of the LtoFlash project.
rem #
rem # This batch file simply deletes the jzIntv directory from the
rem # INTV.jzIntvUI project's build output directory if it exists.
rem #

if exist %1jzIntv (
rmdir /S /Q %1jzIntv
)
