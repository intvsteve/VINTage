rem #########################################################################
rem # custom.bat Generator in Windows                                       #
rem # --------------------------------------------------------------------- #
rem # This batch determines if it is necessary to generate the custom.bat   #
rem # file. The custom.bat file must define the following variable:         #
rem #   MSYS_PATH : the absolute path to an MSYS installation               #
rem #                                                                       #
rem # This batch file must  be called at the top of any batch file that is  #
rem # used to invoke the makefiles used in various VINTage projects.        #
rem #                                                                       #
rem # --------------------------------------------------------------------- #
rem # Environment Variables Initialized in this Batch File:                 #
rem #   MSYS_PATH : Initialized in custom.mak                               #
rem #                                                                       #
rem # --------------------------------------------------------------------- #
rem # Example:                                                              #
rem #                                                                       #
rem # call custom_bat_rule.bat c:\VINTage\project\path                      #
rem #                                                                       #
rem # <remainder of calling batch file>                                     #
rem #                                                                       #
rem #########################################################################

if exist custom.bat (
    rem # ----------------------------------------------------------------- #
    rem custom.bat is present. Leave it alone.
    rem # ----------------------------------------------------------------- #
) else (
    rem # ----------------------------------------------------------------- #
    rem # Emit a default version of custom.bat.
    rem # ----------------------------------------------------------------- #
    echo Creating initial version of custom.bat.
    echo You will need to adjust the configuration for your system.
    echo rem #########################################################################>  custom.bat
    echo rem # custom.bat for Windows                                                #>> custom.bat
    echo rem # --------------------------------------------------------------------- #>> custom.bat
    echo rem # This batch file provides a bridge between the Microsoft Visual Studio #>> custom.bat
    echo rem # project build system and the MSYS / MSYS2 environment used to execute #>> custom.bat
    echo rem # makefiles in Windows. This is achieved by specifying the absolute     #>> custom.bat
    echo rem # to your MSYS environment in the MSYS_PATH environment variable.       #>> custom.bat
    echo rem #                                                                       #>> custom.bat
    echo rem # NOTE: The path must be absolute, and to the MSYS 'bin' directory.     #>> custom.bat
    echo rem #########################################################################>> custom.bat
    echo.>>                                                                              custom.bat
    echo set MSYS_PATH=>>                                                                custom.bat
)

call custom.bat
