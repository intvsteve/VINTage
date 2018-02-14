#############################################################################
# Common Utilities                                                          #
# ------------------------------------------------------------------------- #
# This makefile includes some features that are useful for several tasks    #
# that use GNU 'make' to git 'er done.                                      #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# Determine the target OS. Modified from the technique found here:
#   http://stackoverflow.com/questions/714100/os-detecting-makefile
#
# The following block defines the value for TARGET_OS, a makefile variable
# that has numerous uses throughout other makefiles in this project.
# ------------------------------------------------------------------------- #
ifeq ($(OS),Windows_NT)
  # ----------------------------------------------------------------------- #
  # Windows has a handy environment variable! How about that. Hopefully
  # it won't change.
  # ----------------------------------------------------------------------- #
  TARGET_OS ?= WIN
else
  # ----------------------------------------------------------------------- #
  # Use the uname shell function available in both macOS and Linux to get
  # the operating system name. Hopefully nobody changes this!
  # ----------------------------------------------------------------------- #
  UNAME_S := $(shell uname -s)
  ifeq ($(UNAME_S),Linux)
    TARGET_OS ?= LINUX
  endif
  ifeq ($(UNAME_S),Darwin)
    TARGET_OS ?= MAC
  endif
endif

# ------------------------------------------------------------------------- #
# Verify that we know which OS we're on, and complain if we don't.
# ------------------------------------------------------------------------- #
ifeq (,$(TARGET_OS))
  $(error Unable to determine target operating system! Please specify TARGET_OS variable)
endif

# ------------------------------------------------------------------------- #
# Define the suffix to use for executables.
# ------------------------------------------------------------------------- #
ifeq (WIN,$(TARGET_OS))
  # ----------------------------------------------------------------------- #
  # Oh, Windows, you clever devil. :P Other OSs: NO EXTENSION FOR YOU! :P
  # ----------------------------------------------------------------------- #
  EXE_SUFFIX = .exe
endif
