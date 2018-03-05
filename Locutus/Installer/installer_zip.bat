rem #########################################################################
rem # Batch file for ZIPing Installer setup files in Windows                #
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:                                                            #
rem #  %1 : Absolute path to the Visual Studio project invoking this file   #
rem #  %2 : The Visual Studio solution name, used to identify target        #
rem #  %3 : The value to use for ALLOW_LOCAL_CHANGES                        #
rem #  %4 : The make target (used for clean)                                #
rem #                                                                       #
rem # This batch file provides a bridge between the Microsoft Visual Studio #
rem # project build system and the MSYS / MSYS2 environment used to build   #
rem # a zip file of an installer setup.exe file in Windows. The actual      #
rem # build and clean steps are done via 'make' in the MSYS environment.    #
rem #                                                                       #
rem # To create the zip file in Windows, you will need to have a valid      #
rem # build environment (usually MSYS or MSYS2) that has a version of zip   #
rem # installed in the MinGW environment.                                   #
rem #                                                                       #
rem # To enable the Installer.zip project to build and clean the zip        #
rem # file, you must define the following variable:                         #
rem #   MSYS_PATH                                                           #
rem #                                                                       #
rem # Also, you probably do not want to have any spaces in your path. ;)    #
rem #                                                                       #
rem #########################################################################

rem # --------------------------------------------------------------------- #
rem # Pull in local configuration to enable the build. This also defines
rem # the PROJ_DIR variable used to set the working directory for make.
rem # --------------------------------------------------------------------- #
cd %1..\..
call custom_bat_rule.bat

rem # --------------------------------------------------------------------- #
rem # If MSYS_PATH is empty, there is nothing to do.
rem # --------------------------------------------------------------------- #
if [%MSYS_PATH%] == [] goto SkipBuild

rem # --------------------------------------------------------------------- #
rem # Friendly text.
rem # --------------------------------------------------------------------- #
if [%4] == [clean] (@echo Cleaning ZIP of installer for: %2) else (@echo Creating ZIP of installer for: %2)

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
rem # --------------------------------------------------------------------- #
call setup_bash.bat %1
bash -c "make -C%PROJ_DIR% -f installer_zip.mak %4 SVN_LOCAL_REPO_PATH=../.. TARGET_NAME=%2 ALLOW_LOCAL_CHANGES?=%3 ; echo $?" > %BASH_OUTPUT%
call cleanup_bash.bat
exit %MAKE_RESULT%

:SkipBuild
@echo installer_zip.bat: Local installer zip skipped. MSYS_PATH not set.
