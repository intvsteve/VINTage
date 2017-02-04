#############################################################################
# Machine-Specific Configuration for Building jzIntv                        #
# ------------------------------------------------------------------------- #
# The INTV.jzIntvUI refers to the jzIntv emulator. The contents of this file
# are intended to be customized for your local build environment, and never
# updated in source control.
#
# Local builds jzIntv from the jzIntv source can be accomplished in the
# INTV.jzIntvUI project build in Xamarin Studio / Visual Studio if properly
# configured. It is enabled by defining the JZINTV_DIR variable in this
# makefile, which is consumed by the build_jzIntv.mak file. It may be
# necessary to make other changes in order for your jzIntv build to succeed.
# A prerequisite is that you must be able to build jzIntv on your computer
# already. More extensive effort may be needed to ensure a proper
# environment is created to execute such builds.
#
# ------------------------------------------------------------------------- #
# REQUIRED VARIABLES                                                        #
# ------------------------------------------------------------------------- #
# Set this variable to the absolute path to the jzIntv

# Mac \/
# JZINTV_DIR = ~/Projects/jzIntv

# JZINTV_DIR = ~/jzIntv
# Win /\

#
# Depending on your specific environment, it may be necessary to perform
# custom configuration to get jzIntv to build. When the build_jzIntv.mak
# file is invoked from a C# project, the process that invokes it (such as
# Xamarin Studio or Visual Studio) may not set up an environment conducive
# to running 'make'. In such a case, you may need to execute custom shell
# commands or take other measures to get this portion of the build to work.
#
# NOTE: If you set SKIP_IF_JZINTV_EMPTY=1 and JZINTV_DIR is empty, nothing
#       will be built. The INTV.jzIntvUI project sets this by default.
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
# ADD_ENVIRONMENT_PATH = /opt/local/bin

# ADD_ENVIRONMENT_PATH = /mingw/bin
# Win /\

#
# --------------------- Source Control Update Command --------------------- #
#
# If you have set up access to a source control system for the jzIntv code
# (either for your own private purposes, some public location, or otherwise)
# you can define the 'sync' command to get the most recent sources prior to
# building jzIntv.
#

# SYNC_JZINTV = svn update $(JZINTV_DIR)

## ----------------------------- Validation ------------------------------ ##

ifeq (,$(JZINTV_DIR))
  ifneq (1,$(SKIP_IF_JZINTV_EMPTY))
    $(error Set the JZINTV_DIR variable to the directory containing the jzIntv src directory)
  else
    SKIP_BUILD = 1
  endif
endif
