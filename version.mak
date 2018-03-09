#############################################################################
# Project Version Variable Definitions                                      #
# ------------------------------------------------------------------------- #
# The variables defined assist with updating product version numbers.       #
# The core update features are intended to operate on a SVN repo. There     #
# are currently no plans to adapt 'true' version updates this for use with  #
# Git. That is to say, the  derivation of the specific revision number will #
# need to be rewritten if the 'master' version of the sources moves from    #
# its current SVN repo to GitHub.                                           #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# This file includes common.mak and custom.mak, which allow for
# customization of SVN repo location, et. al.
# ------------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# ROOT_DIR needs to be defined appropriately if this file is included by
# another makefile that is in a subdirectory. This file is shared among
# multiple projects.
# ------------------------------------------------------------------------- #
ROOT_DIR ?= .

include $(ROOT_DIR)/common.mak
-include $(ROOT_DIR)/custom.mak

# ------------------------------------------------------------------------- #
# Other Externally Defined Variables
# ------------------------------------------------------------------------- #
# Some variables that this file cares about are only defined by the projects
# in the Visual Studio / Xamarin Studio / MonoDevelop environments:
#
#   SVN_LOCAL_REPO_PATH : if specified - the local path to the repository
#   PROJECT_NAME        : if specified - the name of a C# project that is
#                         invoking this makefile (used for default values)
# ------------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# The variables described below are used in the process of generating a
# version information class used in C# sources, as well to update Info.plist
# files on Mac. If builds are done within a Git repository with local file
# changes, the class specified by these variables will be parsed to define
# the local-build-with-changes version number. These variables are:
#
#   VERSION_CS_CLASS_NAME       : name of a C# class to generate (or parse)
#   VERSION_CS_CLASS_NAMESPACE  : namespace for the class
#   VERSION_CS_CLASS_OUTPUT_DIR : the directory in which to generate class
#   VERSION_CS                  : name of the file containing C# class
#                                 NOTE: derived from the above variables
# ------------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# Version Information C# Class Generation Settings                          #
# ------------------------------------------------------------------------- #
# To automate the updating of various version strings used in the C#
# projects that constitute VINTage, you can define the VERSION_CS_* values
# documented below. Note that it's possible to override these values, e.g.
# in your custom.mak file at the project root, though doing so will break
# the projects that use the generated class - or at a minimum ensure that
# the class will not be updated as expected.
# ------------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# VERSION_CS_CLASS_NAME is the name of the class and is used to determine
# name of the file in which the class is generated. The name of the
# generated file is simply $(VERSION_CS_CLASS_NAME).cs.
# ------------------------------------------------------------------------- #
VERSION_CS_CLASS_NAME ?= VersionInfo

# ------------------------------------------------------------------------- #
# VERSION_CS is the name of the generated C# class' file.
# ------------------------------------------------------------------------- #
VERSION_CS = $(VERSION_CS_CLASS_NAME).cs

# ------------------------------------------------------------------------- #
# VERSION_CS_CLASS_NAMESPACE is the namespace in which the class defined by
# VERSION_CS_CLASS_NAME will be placed.
# ------------------------------------------------------------------------- #
VERSION_CS_CLASS_NAMESPACE ?= INTV

# ------------------------------------------------------------------------- #
# VERSION_CS_CLASS_OUTPUT_DIR is the destination directory in which to
# generate the class file.
# ------------------------------------------------------------------------- #
VERSION_CS_CLASS_OUTPUT_DIR ?= .

# ------------------------------------------------------------------------- #
# This file may set or initialize the value of the following variables, if
# SVN_REPO is set in custom.mak or elsewhere, and is a valid SVN repository:
#
#   ALLOW_LOCAL_CHANGES : if 0, fails the build if local file changes exist
#   SVN_REVISION        : the current revision number of SVN_REPO
#   SVN_DIRTY           : the number of locally modified files
#   SVN_MODS            : a string reporting number of modified files
#   BUILD_REVISION      : a string reporting revision with modifications
#   CUSTOM_BUILD        : if set, additional info in product about build
#
# The value of the SVN_* variables will be empty if the value of SVN_REPO
# is not specified. In that situation, the value of ALLOW_LOCAL_CHANGES is
# irrelevant.
# ------------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# ALLOW_LOCAL_CHANGES has a default value depending on the context in which
# this makefile is used. If it is 0, but local changes have been made to
# any files in the repository, this makefile will emit an error.
#
# If this makefile is invoked with a PROJECT_NAME that implies it's an
# installer, the default value of ALLOW_LOCAL_CHANGES will be 0. Otherwise,
# the default value will be 1.
# ------------------------------------------------------------------------- #
ifneq (,$(findstring Installer.,$(PROJECT_NAME)))
  ALLOW_LOCAL_CHANGES ?= 0
else
  ALLOW_LOCAL_CHANGES ?= 1
endif

# ------------------------------------------------------------------------- #
# Since the version number is currently specified via SVN revision, if we're
# running vs. a Git repo, extract the version number from the specified
# version class. We'll still compute the number of dirty files directly
# from Git activity, though.
# ------------------------------------------------------------------------- #
ifneq ($(GIT_REPO),)
  VERSION = $(shell grep -o '[[:digit:]]\+\.[[:digit:]]\+\.[[:digit:]]\+\.[[:digit:]]\+' $(VERSION_CS_CLASS_OUTPUT_DIR)/$(VERSION_CS))
  SVN_REVISION = $(subst .,,$(suffix $(VERSION)))
  SVN_DIRTY := $(shell echo `(git status -s || :) | grep -v '^?' | wc -l`)
endif

# ------------------------------------------------------------------------- #
# Fetch SVN revision number and whether there are dirty files. Thanks, Joe!
# ------------------------------------------------------------------------- #
ifneq ($(SVN_REPO),)
  SVN_REVISION := $(shell (svn info -R  $(SVN_LOCAL_REPO_PATH) || :) | grep "Last Changed Rev:" | cut -d' ' -f4 | sort -n | tail -1)
  SVN_DIRTY := $(shell echo `(svn status $(SVN_LOCAL_REPO_PATH) || :) | grep -v '^?' | wc -l`)
endif

# ------------------------------------------------------------------------- #
# If no source control repository is defined, complain and set to zero.
# If we messed up and failed to get anything... Pretend one thing changed.
# ------------------------------------------------------------------------- #
ifeq ($(SVN_DIRTY),)
  ifeq (,$(GIT_REPO)$(SVN_REPO))
    $(warning No source control repo set. Unable to determine dirty file count. Using 0.)
    SVN_DIRTY := 0
  else
    $(warning Dirty file count is empty instead of a number. Assuming 1. Good luck.)
    SVN_DIRTY := 1
  endif
endif

# ----------------------------------------------------------------------- #
# If there are dirty files, check the ALLOW_LOCAL_CHANGES setting.
# Depending on that value, the build will either fail, or define SVN_MODS.
# ----------------------------------------------------------------------- #
ifneq ($(SVN_DIRTY),0)
  ifeq ($(ALLOW_LOCAL_CHANGES),0)
    # If there are dirty files, and allowing an update if there are dirty
    # dirty files is not enabled, then fail.

# ------------------------------------------------------------------------- #
# Definition: DIRTY_FILES_ERROR
# ------------------------------------------------------------------------- #
# This is simply a fancy multi-line string error message.
# ------------------------------------------------------------------------- #
define DIRTY_FILES_ERROR

ERROR
-----------------------------------------------------------------------------
SVN repo: '$(SVN_REPO)'
has $(SVN_DIRTY) modified file(s).
Skipping version update.

To allow version updates, set ALLOW_LOCAL_CHANGES=1
endef

    # ------------------------------------------------------------------- #
    # Do not report the error for clean.
    # ------------------------------------------------------------------- #
    ifneq ($(MAKECMDGOALS),clean)
      $(error $(DIRTY_FILES_ERROR))
    endif
  else
    # ------------------------------------------------------------------- #
    # Dirty little trick to get a space in the text.
    # ------------------------------------------------------------------- #
    SVN_MODS = 
    ifeq ($(SVN_DIRTY),1)
      SVN_FILE_WORD = file
    else
      SVN_FILE_WORD = files
    endif
    SVN_MODS += ($(SVN_DIRTY) modified $(SVN_FILE_WORD))
  endif
endif

# ------------------------------------------------------------------------- #
# If no repos are defined, try getting prior version from .finished file.
# ------------------------------------------------------------------------- #
ifeq ($(SVN_REPO)$(GIT_REPO),)
  ifneq ($(wildcard $(ROOT_DIR)/.$(VERSION_CS_CLASS_NAMESPACE)_$(VERSION_CS_CLASS_NAME).finished),)
    SVN_REVISION = $(firstword $(shell cat $(ROOT_DIR)/.$(VERSION_CS_CLASS_NAMESPACE)_$(VERSION_CS_CLASS_NAME).finished 2>&1))
  endif
endif

# ------------------------------------------------------------------------- #
# If we set the value of SVN_REVISION, poke it into the VERSION_REVISION.
# ------------------------------------------------------------------------- #
ifneq ($(SVN_REVISION),)
  VERSION_REVISION = $(SVN_REVISION)
endif

# ------------------------------------------------------------------------- #
# Set BUILD_REVISION to a user-friendly value to display some details.
# ------------------------------------------------------------------------- #
BUILD_REVISION = Build $(VERSION_REVISION)$(CUSTOM_BUILD)$(SVN_MODS)

# ------------------------------------------------------------------------- #
# Define the short version number, which contains only major.minor.build.
# ------------------------------------------------------------------------- #
VERSION_SHORT ?= $(VERSION_MAJOR).$(VERSION_MINOR).$(VERSION_BUILD)

# ------------------------------------------------------------------------- #
# Define the complete version number.
# ------------------------------------------------------------------------- #
VERSION ?= $(VERSION_SHORT).$(VERSION_REVISION)

# ------------------------------------------------------------------------- #
# Set the copyright date to the current year if it's not already set.
# ------------------------------------------------------------------------- #
COPYRIGHT_DATE ?= $(shell date "+%Y")
