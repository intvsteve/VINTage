#############################################################################
# Machine-Specific Configuration for Building jzIntv Tools                  #
# ------------------------------------------------------------------------- #
# The INTV.jzIntv component requires the following utilities from jzIntv:   #
#  bin2luigi
#  bin2rom
#  intvname
#  luigi2bin
#  rom2bin
#  rom2luigi
#
# The contents of this file are intended to be customized for your local
# build environment, and never updated in source control.
#
# Local builds of these jzIntv tools from jzIntv source can be accomplished
# in the INTV.jzIntv project build in Xamarin Studio / Visual Studio if
# properly configured. It is enabled by defining the JZINTV_DIR variable in
# this makefile, which is consumed by the build_tools.mak file. It may be
# necessary to make other changes in order for your jzIntv build to succeed.
# A prerequisite is that you must be able to build jzIntv -- or at least the
# tools listed above -- on your computer already. More extensive effort may
# be needed to ensure a proper environment is created to execute such builds.
#
# ------------------------------------------------------------------------- #
# REQUIRED VARIABLES                                                        #
# ------------------------------------------------------------------------- #
# Set this variable to the absolute path to the jzIntv

# Mac \/
#JZINTV_DIR_MAC = ~/Projects/jzIntv

#JZINTV_DIR_WIN = ~/jzIntv
# Win /\

#
# Depending on your specific environment, it may be necessary to perform
# custom configuration to get the utilities needed by LUI to build. When
# the build_tools.mak file is invoked from a C# project, the process that
# invokes it (such as Xamarin Studio or Visual Studio) may not set up an
# environment conducive to running 'make'. In such a case, you may need
# to execute custom shell commands or take other measures to get this
# portion of the build to work.
#
# NOTE: If you set SKIP_IF_JZINTV_EMPTY=1 and JZINTV_DIR is empty, nothing
#       will be built. The INTV.jzIntv project sets this by default.
# NOTE: If you set SKIP_BUILD=1 nothing is built or verified.
#
#############################################################################

## ------------------------ Other customizations ------------------------- ##
#
## ---------------------- PATH Environment Variable ---------------------- ##
#
# You may need to ensure proper version of gcc can be found by jzIntv build.
# If you define this variable, the value will be PREPENDED to the PATH
# environment variable during the build.

# Mac \/
#ADD_ENVIRONMENT_PATH_MAC = /opt/local/bin

#ADD_ENVIRONMENT_PATH_WIN = /mingw/bin
# Win /\

#
# --------------------- Source Control Update Command --------------------- #
#
# If you have set up access to a source control system for the jzIntv code
# (either for your own private purposes, some public location, or otherwise)
# you can define the 'sync' command to get the most recent sources prior to
# building the jzIntv components.
#

#SYNC_JZINTV = svn update $(JZINTV_DIR_$(TARGET_OS))

