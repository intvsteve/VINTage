rem #########################################################################
rem # Batch file for ZIPing source from Source Control in Windows           #
rem # --------------------------------------------------------------------- #
rem # ARGUMENTS:                                                            #
rem #  %1 : Absolute path to the Visual Studio project invoking this file   #
rem #  %2 : The Visual Studio solution name, used to identify target        #
rem #  %3 : The value to use for ALLOW_LOCAL_CHANGES                        #
rem #  %4 : The make target (used for clean)                                #
rem #                                                                       #
rem # This batch file provides a bridge between the Microsoft Visual Studio #
rem # project build system and the MSYS / MSYS2 environment that is used    #
rem # to create a ZIP file containing the source code. Whether MSYS is      #
rem # required depends on the source control system in use and whether      #
rem # you feel like either creating an nmake file, or rewriting this .bat   #
rem # file to do everything directly.                                       #
rem #                                                                       #
rem # As this was originally written, to create the ZIP file in Windows,    #
rem # you will need to have a supported source control environment.         #
rem # Presently, this means GitHub or SVN.                                  #
rem #                                                                       #
rem # To enable the Installer.source project to build and clean the ZIP     #
rem # file, you must define the following variables:                        #
rem #   MSYS_PATH: the path for your MSYS environment                       #
rem #     and ONE of the following:                                         #
rem #       GIT_REPO: the source in a Git(Hub) repo                         #
rem #         -- OR --                                                      #
rem #       SVN_REPO: The URL of the source in a SVN repo                   #
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
if [%4] == [clean] (@echo Cleaning source ZIP via MSYS from this location: %MSYS_PATH%) else (@echo Creating source ZIP via MSYS from this location: %MSYS_PATH%)

rem # --------------------------------------------------------------------- #
rem # Use bash (via MSYS) to run GNU make using installer.source.mak.
rem # --------------------------------------------------------------------- #
call setup_bash.bat %1
bash -c "make -C%PROJ_DIR% -f installer.source.mak %4 SVN_LOCAL_REPO_PATH=../.. ALLOW_LOCAL_CHANGES?=%3 ; echo $?" > %BASH_OUTPUT%
call cleanup_bash.bat
exit %MAKE_RESULT%

:SkipBuild
@echo installer.source.bat: Local installer source ZIP skipped.
if [%MSYS_PATH%] == [] @echo MSYS_PATH not set!
