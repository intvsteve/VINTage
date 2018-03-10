#############################################################################
# Windows Installer (setup.exe) ZIP File Build                              #
# ------------------------------------------------------------------------- #
# This makefile provides rules to consume the output of an InstallShield    #
# LE installer build - which produces a setup.exe file - and places it in a #
# properly named .zip file.                                                 #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# This makefile is in a subdirectory, so set ROOT_DIR accordingly.
# ------------------------------------------------------------------------- #
ROOT_DIR = ../..
VERSION_CS_CLASS_OUTPUT_DIR = $(ROOT_DIR)

include $(ROOT_DIR)/version.mak

# ------------------------------------------------------------------------- #
# Customizable Variables                                                    #
# ------------------------------------------------------------------------- #
# The rules to create the zipped installer uses these variables:
#
#   PRODUCT_NAME  : the base of the .zip file name
#   VERSION       : the full version of the product defined via version.mak
# ------------------------------------------------------------------------- #

# ------------------------------------------------------------------------- #
# Set the base name of the ZIP to be created.
# ------------------------------------------------------------------------- #
PRODUCT_NAME ?= LTOFlash

# ------------------------------------------------------------------------- #
# If there are modified files, indicate this in the file name.
# ------------------------------------------------------------------------- #
ifneq ($(SVN_DIRTY),)
  # ----------------------------------------------------------------------- #
  # Set this variable to modify the output file name indicating it's not
  # from pristine sources.
  # ----------------------------------------------------------------------- #
  LOCAL_MODS = -$(SVN_DIRTY)m
endif

# ------------------------------------------------------------------------- #
# If version has not been set, do so, using an obviously bogus value.
# ------------------------------------------------------------------------- #
VERSION ?= 1.0.0.undefined

# ------------------------------------------------------------------------- #
# The names of installer output files are different for xp vs. other
# versions of Windows.
# ------------------------------------------------------------------------- #
ifeq (.xp.,$(findstring .xp.,$(TARGET_NAME)))
  INSTALLER_SOURCE = xp
  INSTALLER_TARGET = xp
else
  INSTALLER_SOURCE = desktop
  INSTALLER_TARGET = Windows
endif

# ------------------------------------------------------------------------- #
# Define the names of the installer output files -- which are prerequisites
# for the target.
# ------------------------------------------------------------------------- #
INSTALLER_OUTPUT_FILE = Express/SingleImage/DiskImages/DISK1/setup.exe
INSTALLER_OUTPUT_DIR := $(CURDIR).$(INSTALLER_SOURCE)/Installer.$(INSTALLER_SOURCE)

# ------------------------------------------------------------------------- #
# Define the names of the input files for the zip command.
# ------------------------------------------------------------------------- #
ZIP_SOURCE_DIR = $(INSTALLER_OUTPUT_DIR)
ZIP_SOURCE_FILE = $(ZIP_SOURCE_DIR)/$(INSTALLER_OUTPUT_FILE)

# ------------------------------------------------------------------------- #
# Define the zip output location and file name.
# ------------------------------------------------------------------------- #
ZIP_OUTPUT_DIR = $(CURDIR).$(INSTALLER_SOURCE)
ZIP_OUTPUT_FILENAME = $(PRODUCT_NAME)-$(INSTALLER_TARGET)-$(VERSION)$(LOCAL_MODS).zip
ZIP_OUTPUT_FILE = $(ZIP_OUTPUT_DIR)/$(ZIP_OUTPUT_FILENAME)

# ------------------------------------------------------------------------- #
# IF SKIP_BUILD == 1, do nothing.
# ------------------------------------------------------------------------- #
ifneq (1,$(SKIP_BUILD))

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# BUILD ALL THE THINGS!
# ------------------------------------------------------------------------- #
all: $(ZIP_OUTPUT_FILE)

# ------------------------------------------------------------------------- #
# Rule: ZIP_OUTPUT_FILE
# ------------------------------------------------------------------------- #
# Creates a ZIP containing the setup.exe file.
# ------------------------------------------------------------------------- #
$(ZIP_OUTPUT_FILE) : $(ZIP_SOURCE_FILE)
	@echo Creating Installer ZIP...
	cd $(dir $<) && zip -9 $@ $(notdir $<)
	@echo

# ------------------------------------------------------------------------- #
# Rule: clean
# ------------------------------------------------------------------------- #
# Clean the output of the installer zip build. Note that because we generate
# the names based on a computed value for the version (see version.mak),
# this rule will not clean up things if you build, edit a new file, and
# then make clean. There are myriad ways this can happen.
# ------------------------------------------------------------------------- #
clean:
	@echo Cleaning Installer ZIP...
	@echo -- DID NOT DELETE $(ZIP_OUTPUT_FILE) --
#	rm -f $(ZIP_OUTPUT_FILE)
	@echo

else

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# No-op the build.
# ------------------------------------------------------------------------- #
all:
	@echo installer_win.mak: Skipped installer zip build.

# ------------------------------------------------------------------------- #
# Rule: clean
# ------------------------------------------------------------------------- #
# No-op the clean.
# ------------------------------------------------------------------------- #
clean:
	@echo installer_win.mak: Skipped installer zip clean.

endif

# ------------------------------------------------------------------------- #
# Get the rule to generate custom.mak if needed.
# ------------------------------------------------------------------------- #
include $(ROOT_DIR)/custom_mak_rule.mak
