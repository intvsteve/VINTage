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
SDL_LIB_VER_1 = 
SDL_LIB_VER_2 = 2
SDL_LIB_VER = $(SDL_LIB_VER_$(JZINTV_SDL_VERSION))

# ------------------------------------------------------------------------- #
# Define files required for jzIntv to run, readme files, et. al.
# ------------------------------------------------------------------------- #
JZINTV_SUPPORT_FILES_WIN   = bin/SDL$(SDL_LIB_VER).dll
JZINTV_SUPPORT_FILES_MAC   =
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
# If SKIP_BUILD == 1, do nothing.
# ------------------------------------------------------------------------- #
ifneq (1,$(SKIP_BUILD))

# ------------------------------------------------------------------------- #
# Define the prerequisite for producing jzIntv. By default assume local
# source build. If a jzIntv distribution is used, this is updated later.
# ------------------------------------------------------------------------- #
JZINTV_PREREQ = $(addprefix $(JZINTV_DIR)/src/,$(TARGET_MAKEFILE))

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
# Set up variables if pulling from distribution or building locally. If a
# distribution is used rather than a build directly from source, modify any
# existing variables as needed and set up those required for distribution.
# ------------------------------------------------------------------------- #
ifneq (,$(JZINTV_DIST_URL))
  USING_JZINTV_DIST       = 1
  JZINTV_DOWNLOAD_TARGET  = $(JZINTV_DIR)/$(JZINTV_DIST_FILENAME)
  JZINTV_UNZIP_TARGET_DIR = $(basename $(JZINTV_DIST_FILENAME))
  JZINTV_OUTPUT_DIR       = $(JZINTV_DIR)/$(JZINTV_UNZIP_TARGET_DIR)
  JZINTV_PREREQ           = $(JZINTV_DOWNLOAD_TARGET)
  JZINTV_SUPPORT_PREREQ   = $(JZINTV_DOWNLOAD_TARGET)

  # ----------------------------------------------------------------------- #
  # Helper variables just to make the shell calls to unzip easier to read.
  # ----------------------------------------------------------------------- #
  JZINTV_ZIP_LIST_ARG = $(JZINTV_DOWNLOAD_TARGET) $(JZINTV_UNZIP_TARGET_DIR)
  AWK_UNZIP_FILTER    = /-----/ {p = ++p % 2; next} p {print $$NF}

  # ----------------------------------------------------------------------- #
  # Release notes are split across multiple files.
  # ----------------------------------------------------------------------- #
  JZINTV_RELEASE_NOTES = $(notdir $(shell unzip -l $(JZINTV_ZIP_LIST_ARG)/Release* | awk '$(AWK_UNZIP_FILTER)'))

  # ----------------------------------------------------------------------- #
  # Enumerate documentation files explicitly.
  # ----------------------------------------------------------------------- #
  JZINTV_DOCFILES = $(notdir $(shell unzip -l $(JZINTV_ZIP_LIST_ARG)/$(JZINTV_DOCUMENTATION_DIR)/* | awk '$(AWK_UNZIP_FILTER)'))
  JZINTV_DOCUMENTATION = $(addprefix $(JZINTV_DOCUMENTATION_DIR)/, $(JZINTV_DOCFILES))

  # ----------------------------------------------------------------------- #
  # Add the jzintv documentation files to the support files list.
  # ----------------------------------------------------------------------- #
  JZINTV_SUPPORT_FILES += $(JZINTV_DOCUMENTATION)

else

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
$(JZINTV_DOWNLOAD_TARGET):
	@echo Retrieving jzIntv Distribution: $@ ...
	@curl -o $@ $(JZINTV_DIST_URL)

# ------------------------------------------------------------------------- #
# Function: ExtractSupportFileRule
# ------------------------------------------------------------------------- #
# This function defines a rule to extract a specific jzIntv support file
# from the jzIntv distribution to JZINTV_OUTPUT_DIR. It requires the
# following arguments:
#   $(1) : the support file to be extracted
# ------------------------------------------------------------------------- #
define ExtractSupportFileRule
$(JZINTV_OUTPUT_DIR)/$(1) : $(JZINTV_PREREQ)
	@echo Extracting $$@ from $$<
	@unzip -qquo $$< $(JZINTV_UNZIP_TARGET_DIR)/$1 -d $$(dir $$<)

endef

# ------------------------------------------------------------------------- #
# Declare the targets for the support files to extract from a distribution
# to the intermediate directory before copying to build output.
# ------------------------------------------------------------------------- #
$(foreach f,$(JZINTV_SUPPORT_FILES),$(eval $(call ExtractSupportFileRule,$(f))))

endif

# ------------------------------------------------------------------------- #
# Function: CopySupportFileRule
# ------------------------------------------------------------------------- #
# This function defines a rule to copy a specific jzIntv support file from
# the jzIntv built output to TARGET_DIR. It requires the following arguments:
#   $(1) : the support file to be copied
# ------------------------------------------------------------------------- #
define CopySupportFileRule
$(TARGET_DIR)/$(1) : $(JZINTV_OUTPUT_DIR)/$(1)
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
$(TARGET_DIR)/bin/$(JZINTV_APP) : $(JZINTV_OUTPUT_DIR)/bin/$(JZINTV_APP)
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
$(JZINTV_OUTPUT_DIR)/bin/$(JZINTV_APP) : $(JZINTV_PREREQ)
ifeq (,$(USING_JZINTV_DIST))
ifneq (,$(SYNC_JZINTV))
	@echo Syncing $(JZINTV_DIR) ...
	$(SYNC_JZINTV)
endif
	@echo Building jzIntv ...
	make -C $(JZINTV_DIR)/src -f $(notdir $^) ../bin/$(JZINTV_APP)
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
$(CONTENT_DIR)/$(1) : $(TARGET_DIR)/$(1)
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
$(CONTENT_DIR)/bin/$(JZINTV_APP) : $(TARGET_DIR)/bin/$(JZINTV_APP)
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
ifeq (,$(USING_JZINTV_DIST))
	@echo Cleaning jzIntv ...
	make -C $(JZINTV_DIR)/src -f $(TARGET_MAKEFILE) clean
	rm -rf $(TARGET_DIR)/bin/$(JZINTV_APP) $(addprefix $(TARGET_DIR)/,$(JZINTV_SUPPORT_FILES)) $(CONTENT_DIR)
else
ifeq (1,$(JZINTV_CLEAN_DIST))
	@echo Removing $(notdir $(JZINTV_DOWNLOAD_TARGET)) in $(dir $(JZINTV_DOWNLOAD_TARGET))
	@rm -f $(JZINTV_DOWNLOAD_TARGET)
	@echo Removing jzIntv files previously extracted from $(notdir $(JZINTV_DOWNLOAD_TARGET)) ...
	@rm -f $(addprefix $(JZINTV_OUTPUT_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES))
	@rm -frd $(JZINTV_DIR)/$(JZINTV_UNZIP_TARGET_DIR)
endif
endif
	@echo Cleaning jzintv from $(CURDIR)/$(TARGET_DIR) ...
	@rm -frd $(addprefix $(TARGET_DIR)/, bin/$(JZINTV_APP) $(JZINTV_SUPPORT_FILES) $(JZINTV_DOCUMENTATION_DIR))
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
