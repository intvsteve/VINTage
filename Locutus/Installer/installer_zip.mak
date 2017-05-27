#############################################################################
## Windows Installer (setup.exe) ZIP File Build
##
## Consumes the output of an InstallShield LE installer build - which
## produces a setup.exe file - and places it in a properly named .zip file.
##
#############################################################################

# Base name of the output zip file.
NAME ?= LTOFlash
# Version string used for the file name.
VERSION ?= 1.0.0.3020

# Determine the target OS. Modified from the technique found here:
#  http://stackoverflow.com/questions/714100/os-detecting-makefile
ifeq ($(OS),Windows_NT)
  TARGET_OS ?= WIN
else
  UNAME_S := $(shell uname -s)
  ifeq ($(UNAME_S),Linux)
    TARGET_OS ?= LINUX
  endif
  ifeq ($(UNAME_S),Darwin)
    TARGET_OS ?= MAC
  endif
endif

ifeq (,$(TARGET_OS))
  $(error Unable to determine target operating system! Please specify TARGET_OS variable)
else
  ifneq (WIN,$(TARGET_OS))
    $(error This makefile is only to be used for Windows installer zip builds!)
  endif
endif

# The names of installer output files are different for xp vs. other versions of Windows.
ifeq (.xp.,$(findstring .xp.,$(TARGET_NAME)))
  INSTALLER_SOURCE = xp
  INSTALLER_TARGET = xp
else
  INSTALLER_SOURCE = desktop
  INSTALLER_TARGET = Windows
endif

# Define the names of the installer output files -- which are prerequisites for the target.
INSTALLER_OUTPUT_FILE = Express/SingleImage/DiskImages/DISK1/setup.exe
INSTALLER_OUTPUT_DIR := $(CURDIR).$(INSTALLER_SOURCE)/Installer.$(INSTALLER_SOURCE)

# Define the names of the input files for the zip command.
ZIP_SOURCE_DIR = $(INSTALLER_OUTPUT_DIR)
ZIP_SOURCE_FILE = $(ZIP_SOURCE_DIR)/$(INSTALLER_OUTPUT_FILE)

# Define the zip output location and file name.
ZIP_OUTPUT_DIR = $(CURDIR).$(INSTALLER_SOURCE)
ZIP_OUTPUT_FILENAME = $(NAME)-$(INSTALLER_TARGET)-$(VERSION).zip
ZIP_OUTPUT_FILE = $(ZIP_OUTPUT_DIR)/$(ZIP_OUTPUT_FILENAME)

# IF SKIP_BUILD == 1, do nothing.
ifneq (1,$(SKIP_BUILD))

# BUILD ALL THE THINGS!
all: $(ZIP_OUTPUT_FILE)

# Rule to zip up the setup.exe file.
$(ZIP_OUTPUT_FILE) : $(ZIP_SOURCE_FILE)
	@echo Creating Installer ZIP...
	cd $(dir $<) && zip -9 $@ $(notdir $<)
	@echo

# Clean the output of the installer zip build.
clean:
	@echo Cleaning Installer ZIP...
	@echo -- DID NOT DELETE $(ZIP_OUTPUT_FILE) --
#	rm -f $(ZIP_OUTPUT_FILE)
	@echo

else

# No-op the build.
all:
	@echo installer_win.mak: Skipped installer zip build.

# No-op the clean.
clean:
	@echo installer_win.mak: Skipped installer zip clean.

endif
