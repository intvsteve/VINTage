#############################################################################
# jzIntv Build                                                              #
# ------------------------------------------------------------------------- #
# When properly configured via custom.mak and custom_jzintv.mak, produces   #
# the jzIntv emulator and places the minimum required output files needed   #
# for including a copy of jzIntv in the LUI / VINTage project.              #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# This makefile is in a subdirectory, so set ROOT_DIR accordingly.
# ------------------------------------------------------------------------- #
ROOT_DIR = ..

include $(ROOT_DIR)/common.mak
-include $(ROOT_DIR)/custom_jzIntv.mak

# ------------------------------------------------------------------------- #
# If SKIP_BUILD == 1, do nothing.
# ------------------------------------------------------------------------- #
ifneq (1,$(SKIP_BUILD))

# ------------------------------------------------------------------------- #
# Define the target directory to put jzIntv into.
# ------------------------------------------------------------------------- #
TARGET_DIR_WIN   = jzIntv/Win
TARGET_DIR_MAC   = jzIntv/Mac
TARGET_DIR_LINUX = jzIntv/Linux
TARGET_DIR       = $(TARGET_DIR_$(TARGET_OS))

# ------------------------------------------------------------------------- #
# Validate configuration for the jzIntv build.
# ------------------------------------------------------------------------- #
ifeq (,$(TARGET_DIR))
  $(error Set the TARGET_DIR_$(TARGET_OS) variable appropriately)
endif

# ------------------------------------------------------------------------- #
# Root directory to locate jzIntv files. This is either the output from a
# source build, or a local extraction from a ZIP file distribution.
# ------------------------------------------------------------------------- #
JZINTV_OUTPUT_DIR = $(JZINTV_DIR)

# ------------------------------------------------------------------------- #
# Define the SDL library version suffix. More details regarding valid values
# for JZINTV_SDL_VERSION are in custom_jzintv.mak.
# ------------------------------------------------------------------------- #
SDL_LIB_VER_1        = 
SDL_LIB_VER_2        = 2
SDL_LIB_VER          = $(SDL_LIB_VER_$(JZINTV_SDL_VERSION))

# ------------------------------------------------------------------------- #
# Define SDL support file name. This depends on platform:
#   Mac:     We include a .dmg from libsdl.org
#   Windows: We include the appropriate DLL, presently from jzIntv directly
#   Linux:   SDL is statically linked so no support file is needed
#
# Until 64-bit Windows builds of jzIntv are supported, and / or the jzIntv
# source discontinues including the SDL DLLs directly, we won't need to
# fetch the binaries directly from the SDL site. If we do, though, the
# 32-bit and 64-bit downloads follow the same naming pattern as the others -
# we just would need to choose based on the platform. Just in case that
# arises, the following full URLs can act as examples for Windows:
#   https://www.libsdl.org/release/SDL2-2.0.14-win32-x86.zip
#   https://www.libsdl.org/release/SDL2-2.0.14-win32-x64.zip
#   https://www.libsdl.org/release/SDL-1.2.15-win32.zip
#   https://www.libsdl.org/release/SDL-1.2.15-win32-x64.zip
# ------------------------------------------------------------------------- #
SDL_LIB_FILENAME_WIN = SDL$(SDL_LIB_VER).dll
SDL_LIB_FILENAME_MAC = SDL$(SDL_LIB_VER)-$(SDL_DMG_VERSION).dmg
SDL_LIB_FILENAME     = $(SDL_LIB_FILENAME_$(TARGET_OS))

# ------------------------------------------------------------------------- #
# Define whether or not the SDL support file must be downloaded via curl.
# ------------------------------------------------------------------------- #
SDL_DOWNLOAD_REQUIRED_WIN   = 
SDL_DOWNLOAD_REQUIRED_MAC   = 1
SDL_DOWNLOAD_REQUIRED_LINUX = 
SDL_DOWNLOAD_REQUIRED       = $(SDL_DOWNLOAD_REQUIRED_$(TARGET_OS))

# ------------------------------------------------------------------------- #
# Define files required for jzIntv to run, readme files, et. al.
# ------------------------------------------------------------------------- #
JZINTV_SUPPORT_FILES_WIN   = bin/$(SDL_LIB_FILENAME)
JZINTV_SUPPORT_FILES_MAC   = bin/$(SDL_LIB_FILENAME)
JZINTV_SUPPORT_FILES_LINUX =
JZINTV_SUPPORT_FILES       = $(JZINTV_SUPPORT_FILES_$(TARGET_OS)) \
                             COPYING.txt \
                             README.txt

# ------------------------------------------------------------------------- #
# Define the directory containing documentation files for jzIntv.
# ------------------------------------------------------------------------- #
JZINTV_DOCUMENTATION_DIR = doc/jzintv

# ------------------------------------------------------------------------- #
# The jzIntv executable to be built and included in the INTV.jzIntv output.
# ------------------------------------------------------------------------- #
JZINTV_EXECUTABLE = jzintv

# ------------------------------------------------------------------------- #
# The actual executable file name.
# ------------------------------------------------------------------------- #
JZINTV_APP = $(addsuffix $(EXE_SUFFIX), $(JZINTV_EXECUTABLE))

# ------------------------------------------------------------------------- #
# The directory in which the jzIntv source is found.
# ------------------------------------------------------------------------- #
JZINTV_SRC = $(JZINTV_DIR)/src

# ------------------------------------------------------------------------- #
# Define the prerequisite for producing jzIntv. By default assume local
# source build. If a jzIntv distribution is used, this is updated later.
# ------------------------------------------------------------------------- #
JZINTV_PREREQ = $(addprefix $(JZINTV_SRC)/,$(TARGET_MAKEFILE))

# ----------------------------------------------------------------------- #
# If CONTENT_DIR is defined, set CONTENT_COPY_FILES to be the collection
# of jzIntv files to be copied to the CONTENT_DIR. Presently, this is a
# Mac-only feature.
# ----------------------------------------------------------------------- #
ifneq (,$(CONTENT_DIR))
  # ----------------------------------------------------------------------- #
  # Declare the jzintv executable and support files to be copied.
  # ----------------------------------------------------------------------- #
  CONTENT_COPY_FILES = $(addprefix $(CONTENT_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES))
endif

# ------------------------------------------------------------------------- #
# Set up variables if using a distribution.
# ------------------------------------------------------------------------- #
ifneq (,$(JZINTV_DIST_URL))
  USING_JZINTV_DIST       = 1
  JZINTV_DOWNLOAD_TARGET  = $(JZINTV_DIR)/$(JZINTV_DIST_FILENAME)
  JZINTV_UNZIP_TARGET_DIR = $(basename $(JZINTV_DIST_FILENAME))
  JZINTV_OUTPUT_DIR       = $(JZINTV_DIR)/$(JZINTV_UNZIP_TARGET_DIR)
  JZINTV_SUPPORT_PREREQ   = $(JZINTV_DOWNLOAD_TARGET)

  # ----------------------------------------------------------------------- #
  # Helper variables just to make the shell calls to unzip easier to read.
  # ----------------------------------------------------------------------- #
  JZINTV_ZIP_LIST_ARG = $(JZINTV_DOWNLOAD_TARGET) $(JZINTV_UNZIP_TARGET_DIR)
  AWK_UNZIP_FILTER    = /-----/ {p = ++p % 2; next} p {print $$NF}

  ifneq ($(MAKECMDGOALS),clean)
  # ----------------------------------------------------------------------- #
  # Release notes are split across multiple files.
  # ----------------------------------------------------------------------- #
  JZINTV_RELEASE_NOTES = $(notdir $(shell unzip -l $(JZINTV_ZIP_LIST_ARG)/Release* | awk '$(AWK_UNZIP_FILTER)'))

  # ----------------------------------------------------------------------- #
  # Enumerate documentation files explicitly.
  # ----------------------------------------------------------------------- #
  JZINTV_DOCFILES = $(notdir $(shell unzip -l $(JZINTV_ZIP_LIST_ARG)/$(JZINTV_DOCUMENTATION_DIR)/* | awk '$(AWK_UNZIP_FILTER)'))
  endif

  JZINTV_DOCUMENTATION = $(addprefix $(JZINTV_DOCUMENTATION_DIR)/, $(JZINTV_DOCFILES))

  # ----------------------------------------------------------------------- #
  # Add the jzintv documentation files to the support files list.
  # ----------------------------------------------------------------------- #
  JZINTV_SUPPORT_FILES += $(JZINTV_DOCUMENTATION)
  
  # ----------------------------------------------------------------------- #
  # For a binary distribution, we need to alter the prerequisite to be the
  # distribution ZIP file.
  # ----------------------------------------------------------------------- #
  ifeq (BIN,$(JZINTV_DIST_TYPE))
    JZINTV_PREREQ = $(JZINTV_DOWNLOAD_TARGET)
  endif

  # ----------------------------------------------------------------------- #
  # For a source distribution, we need to alter the JZINTV_SRC directory to
  # be the location at which the unzipped source from the distribution ZIP
  # file will land. Also, disable the sync command if it was defined.
  # ----------------------------------------------------------------------- #
  ifeq (SRC,$(JZINTV_DIST_TYPE))
    JZINTV_SRC  = $(JZINTV_DIR)/$(JZINTV_UNZIP_TARGET_DIR)/src
    SYNC_JZINTV = 
  endif

else
  # ----------------------------------------------------------------------- #
  # Reset the value of JZINTV_DIST_TYPE in order to preserve build-from-svn
  # repo behavior.
  # ----------------------------------------------------------------------- #
  JZINTV_DIST_TYPE = 

  # ----------------------------------------------------------------------- #
  # Gather all the release notes from the root directory.
  # NOTE: This may actually not be accurate. Rather than get super fancy, it
  # may be wise to sync FIRST, then build. In the build-and-sync case, it's
  # possible that a sync will bring down a new readme file. If the sync
  # happens AFTER the evaluation of this variable, the list may be stale.
  # That said, this should be evaluated lazily, such that as long as we run
  # the rule to build the jzintv executable BEFORE this rule, and the value
  # of SYNC_JZINTV indicates that a sync should occur, this will be OK. In
  # reality, this out-of-sync case should happen ... never? (lulz)
  # ----------------------------------------------------------------------- #
  JZINTV_RELEASE_NOTES = $(notdir $(wildcard $(JZINTV_DIR)/Release*))

  # ----------------------------------------------------------------------- #
  # In this build we can use simple copy rule for a directory.
  # ----------------------------------------------------------------------- #
  JZINTV_SUPPORT_FILES += $(JZINTV_DOCUMENTATION_DIR)

endif

# ----------------------------------------------------------------------- #
# Add release notes files to the support files output.
# ----------------------------------------------------------------------- #
JZINTV_SUPPORT_FILES += $(JZINTV_RELEASE_NOTES)

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# BUILD ALL THE THINGS!
# ------------------------------------------------------------------------- #
all: $(addprefix $(TARGET_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES)) $(CONTENT_COPY_FILES)

# ------------------------------------------------------------------------- #
# Define the rule to download the distribution if necessary.
# ------------------------------------------------------------------------- #
ifeq (1,$(USING_JZINTV_DIST))
ifneq ($(MAKECMDGOALS),clean)
$(JZINTV_DOWNLOAD_TARGET):
	@echo Retrieving jzIntv Distribution: $@ ...
	@curl -o $@ $(JZINTV_DIST_URL)

# ----------------------------------------------------------------------- #
# Define the rule to extract source. It is triggered by the makefile
# prerequisite for the jzintv application build.
# ----------------------------------------------------------------------- #
  ifeq (SRC,$(JZINTV_DIST_TYPE))
$(JZINTV_PREREQ): $(JZINTV_DOWNLOAD_TARGET)
	@echo Extracting jzIntv source from $< ...
	@unzip -qquo $< -d $(dir $<)

  endif

# ------------------------------------------------------------------------- #
# Function: ExtractSupportFileRule
# ------------------------------------------------------------------------- #
# This function defines a rule to extract a specific jzIntv support file
# from the jzIntv distribution to JZINTV_OUTPUT_DIR. It requires the
# following arguments:
#   $(1) : the support file to be extracted
#
# NOTE: If the SDL support file must be downloaded, rather than copied from
#       the jzIntv distribution / build, it is excluded from this rule
#       generator. A bespoke rule for this case will be defined later.
# ------------------------------------------------------------------------- #
define ExtractSupportFileRule
ifneq (1$(SDL_LIB_FILENAME),$(SDL_DOWNLOAD_REQUIRED)$(1))
$(JZINTV_OUTPUT_DIR)/$(1): $(JZINTV_SUPPORT_PREREQ)
	@echo Extracting $$@ from $$<
	@unzip -qquo $$< $(JZINTV_UNZIP_TARGET_DIR)/$1 -d $$(dir $$<)

endif
endef

# ------------------------------------------------------------------------- #
# Declare the targets for the support files to extract from a distribution
# to the intermediate directory before copying to build output.
# ------------------------------------------------------------------------- #
$(foreach f,$(JZINTV_SUPPORT_FILES),$(eval $(call ExtractSupportFileRule,$(f))))

endif
endif

# ------------------------------------------------------------------------- #
# Rule: Download SDL support file
# ------------------------------------------------------------------------- #
# If the SDL support file for the target build is not included as part of
# jzIntv, but rather must be downloaded directly, define the rule to do so.
# ------------------------------------------------------------------------- #
ifeq (1,$(SDL_DOWNLOAD_REQUIRED))
ifneq ($(MAKECMDGOALS),clean)
$(JZINTV_OUTPUT_DIR)/bin/$(SDL_LIB_FILENAME):
	@echo Downloading $(notdir $@) from $(SDL_DOWNLOAD_URL) ...
	@curl -o $@ $(SDL_DOWNLOAD_URL)/$(SDL_LIB_FILENAME)

endif
endif

# ------------------------------------------------------------------------- #
# Function: CopySupportFileRule
# ------------------------------------------------------------------------- #
# This function defines a rule to copy a specific jzIntv support file from
# the jzIntv built output to TARGET_DIR. It requires the following arguments:
#   $(1) : the support file to be copied
# ------------------------------------------------------------------------- #
define CopySupportFileRule
$(TARGET_DIR)/$(1): $(JZINTV_OUTPUT_DIR)/$(1)
	@echo Copying $(1) ...
	@mkdir -p $$(dir $$@)
	@cp -fpR $$^ $$(dir $$@)
	@echo

endef

# ------------------------------------------------------------------------- #
# Declare the targets for the support files to copy into the appropriate
# output directory.
# ------------------------------------------------------------------------- #
$(foreach f,$(JZINTV_SUPPORT_FILES),$(eval $(call CopySupportFileRule,$(f))))

# ------------------------------------------------------------------------- #
# Rule: Copy jzintv executable
# ------------------------------------------------------------------------- #
# This rule copies the jzintv executable from the jzintv build output to
# the defined target directory.
# ------------------------------------------------------------------------- #
$(TARGET_DIR)/bin/$(JZINTV_APP): $(JZINTV_OUTPUT_DIR)/bin/$(JZINTV_APP)
	@echo Copying $^ ...
	@cp -fp $^ $(dir $@)
	@echo

# ------------------------------------------------------------------------- #
# Rule: Build or extract jzIntv
# ------------------------------------------------------------------------- #
# This rule either extracts the jzIntv executable from a distribution, or it
# it invokes the jzIntv build at its source. If SYNC_JZINTV is defined when
# building from source, the jzIntv source is synchronized first.
# ------------------------------------------------------------------------- #
$(JZINTV_OUTPUT_DIR)/bin/$(JZINTV_APP): $(JZINTV_PREREQ)
ifneq (BIN,$(JZINTV_DIST_TYPE))
ifneq (,$(SYNC_JZINTV))
	@echo Syncing $(JZINTV_DIR) ...
	$(SYNC_JZINTV)
endif
	@echo Building jzIntv ...
	make -C $(JZINTV_SRC) -f $(notdir $^) ../bin/$(JZINTV_APP)
	strip $@
else
	@echo Extracting $@ from $<
	@unzip -qquo $< $(JZINTV_UNZIP_TARGET_DIR)/bin/$(JZINTV_APP) -d $(dir $<)
endif
	@echo

  # ----------------------------------------------------------------------- #
  # If CONTENT_DIR is defined, the rules necessary to copy files to the
  # content directory will be defined.
  # ----------------------------------------------------------------------- #
  ifneq (,$(CONTENT_DIR))

# ------------------------------------------------------------------------- #
# Function: CopyContentSupportFileRule
# ------------------------------------------------------------------------- #
# This function defines a rule for a specific jzIntv support file that is to
# be included as part of the output of the build. On Mac, for some reason
# copying a directory w/o creating the target directory first results in the
# content of source being copied rather than the directory itself... The
# source (doc/jzintv) does not have a trailing /, so it  seems odd that the
# resulting observed behavior happens... It works fine in the above rule that
# copies the files into the project-relative location, after all... Anyhow,
# this function requires the following arguments:
#   $(1) : the content file to copy
# ------------------------------------------------------------------------- #
define CopyContentSupportFileRule
$(CONTENT_DIR)/$(1): $(TARGET_DIR)/$(1)
	@echo Copying to Content $(1) ...
	@mkdir -p $$(dir $$@)
	@cp -fpR $$^ $$(dir $$@)
	@echo

endef

# ------------------------------------------------------------------------- #
# Declare the targets for the support files to put into the appropriate
# output directory.
# ------------------------------------------------------------------------- #
$(foreach f,$(JZINTV_SUPPORT_FILES),$(eval $(call CopyContentSupportFileRule,$(f))))

# ------------------------------------------------------------------------- #
# Ruile: Copy jzintv executable to content directory
# ------------------------------------------------------------------------- #
# This rule copies the jzIntv emulator executable to the content directory.
# ------------------------------------------------------------------------- #
$(CONTENT_DIR)/bin/$(JZINTV_APP): $(TARGET_DIR)/bin/$(JZINTV_APP)
	@echo Copying to Content $^ ...
	@mkdir -p $(dir $@)
	@cp -fp $^ $(dir $@)
	@echo

endif

# ------------------------------------------------------------------------- #
# Rule: clean
# ------------------------------------------------------------------------- #
# Clean the output of the jzIntv build. NUKE THE MOON!
# ------------------------------------------------------------------------- #
clean:
	@echo Cleaning jzIntv ...
ifneq (BIN,$(JZINTV_DIST_TYPE))
ifneq ("$(wildcard $(JZINTV_SRC)/$(TARGET_MAKEFILE))","")
	make -C $(JZINTV_SRC) -f $(TARGET_MAKEFILE) clean
endif
endif
ifeq (1,$(JZINTV_CLEAN_DIST))
	@echo Removing $(notdir $(JZINTV_DOWNLOAD_TARGET)) in $(dir $(JZINTV_DOWNLOAD_TARGET))
	@rm -f $(JZINTV_DOWNLOAD_TARGET)
	@echo Removing jzIntv files previously extracted from $(notdir $(JZINTV_DOWNLOAD_TARGET)) ...
	@rm -f $(addprefix $(JZINTV_OUTPUT_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES))
	@rm -frd $(JZINTV_DIR)/$(JZINTV_UNZIP_TARGET_DIR)
endif
	@echo Cleaning jzintv from $(CURDIR)/$(TARGET_DIR) ...
	@rm -frd $(addprefix $(TARGET_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES) *.txt $(JZINTV_DOCUMENTATION_DIR)) $(CONTENT_DIR)
	@echo

else

# No-op the build.
all:
	@echo build_jzIntv.mak: Skipped jzIntv build.

# No-op the clean.
clean:
	@echo build_jzIntv.mak: Skipped jzIntv clean.

endif

# ------------------------------------------------------------------------- #
# Get the rule to generate custom_jzIntv.mak if needed.
# ------------------------------------------------------------------------- #
include $(ROOT_DIR)/custom_jzIntv_mak_rule.mak
