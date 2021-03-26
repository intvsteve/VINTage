#############################################################################
# jzIntv Tools Build                                                        #
# ------------------------------------------------------------------------- #
# When properly configured via custom.mak and custom_jzintv.mak, produces   #
# the tools used by the INTV.jzIntv assembly that is part of the VINTage /  #
# LUI project. The root common.mak file determines the operating system.    #
#                                                                           #
#############################################################################

# ------------------------------------------------------------------------- #
# This makefile is in a subdirectory, so set ROOT_DIR accordingly.
# ------------------------------------------------------------------------- #
ROOT_DIR = ..

include $(ROOT_DIR)/common.mak
-include $(ROOT_DIR)/custom_jzIntv.mak

# ------------------------------------------------------------------------- #
# If SKIP_BUILD == 1, do nothing. Otherwise, carry on!
# ------------------------------------------------------------------------- #
ifneq (1,$(SKIP_BUILD))

# ------------------------------------------------------------------------- #
# Define the destination directory to use for the tool executables.
# ------------------------------------------------------------------------- #
TOOL_OUTPUT_DIR_WIN   = tools
TOOL_OUTPUT_DIR_MAC   = tools/Mac
TOOL_OUTPUT_DIR_LINUX = tools/Linux
TOOL_OUTPUT_DIR       = $(TOOL_OUTPUT_DIR_$(TARGET_OS))

# ------------------------------------------------------------------------- #
# Validate configuration for tools build.
# ------------------------------------------------------------------------- #
ifeq (,$(TOOL_OUTPUT_DIR))
  $(error Set the TOOL_OUTPUT_DIR_$(TARGET_OS) variable appropriately)
endif

# ------------------------------------------------------------------------- #
# Define the directory that will contain the output of jzIntv tool build.
# ------------------------------------------------------------------------- #
TOOL_INPUT_DIR = $(JZINTV_DIR)/bin

# ------------------------------------------------------------------------- #
# Declare jzIntv tools to build and include in the INTV.jzIntv output.
# ------------------------------------------------------------------------- #
JZINTV_UTILITIES = \
  bin2luigi \
  bin2rom \
  intvname \
  luigi2bin \
  rom_merge \
  rom2bin \
  rom2luigi

# ------------------------------------------------------------------------- #
# Define the actual tool executable names. EXE_SUFFIX is in common.mak.
# ------------------------------------------------------------------------- #
JZINTV_APPS = $(addsuffix $(EXE_SUFFIX), $(JZINTV_UTILITIES))

# ------------------------------------------------------------------------- #
# The directory in which the jzIntv / SDK-1600 source is found.
# ------------------------------------------------------------------------- #
JZINTV_SRC = $(JZINTV_DIR)/src

# ------------------------------------------------------------------------- #
# Define the prerequisite for producing the tools. By default assume local
# source build. If a jzIntv binary distribution is used, this is updated.
# ------------------------------------------------------------------------- #
TOOL_INPUT_PREREQ = $(addprefix $(JZINTV_SRC)/,$(TARGET_MAKEFILE))

# ------------------------------------------------------------------------- #
# Set up variables if using a distribution.
# ------------------------------------------------------------------------- #
ifneq (,$(JZINTV_DIST_URL))
  USING_JZINTV_DIST       = 1
  JZINTV_DOWNLOAD_TARGET  = $(JZINTV_DIR)/$(JZINTV_DIST_FILENAME)
  JZINTV_UNZIP_TARGET_DIR = $(basename $(JZINTV_DIST_FILENAME))
  TOOL_INPUT_DIR          = $(JZINTV_DIR)/$(JZINTV_UNZIP_TARGET_DIR)/bin

  # ----------------------------------------------------------------------- #
  # For a binary distribution, we need to alter the prerequisite to be the
  # distribution ZIP file.
  # ----------------------------------------------------------------------- #
  ifeq (BIN,$(JZINTV_DIST_TYPE))
    TOOL_INPUT_PREREQ = $(JZINTV_DOWNLOAD_TARGET)
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
endif

# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# BUILD ALL THE THINGS!
# ------------------------------------------------------------------------- #
all: $(addprefix $(TOOL_OUTPUT_DIR)/, $(JZINTV_APPS))

# ------------------------------------------------------------------------- #
# Define the rules for distribution-based builds.
# ------------------------------------------------------------------------- #
ifeq (1,$(USING_JZINTV_DIST))
ifneq ($(MAKECMDGOALS),clean)
# ------------------------------------------------------------------------- #
# Rule to download a distribution.
# ------------------------------------------------------------------------- #
$(JZINTV_DOWNLOAD_TARGET):
	@echo Retrieving jzIntv Distribution: $@ ...
	@curl -o $@ $(JZINTV_DIST_URL)

# ----------------------------------------------------------------------- #
# Define the rule to extract source. It is triggered by the makefile
# prerequisite for the target tool being built.
# ----------------------------------------------------------------------- #
  ifeq (SRC,$(JZINTV_DIST_TYPE))
$(TOOL_INPUT_PREREQ): $(JZINTV_DOWNLOAD_TARGET)
	@echo Extracting jzIntv source from $< ...
	@unzip -qquo $< -d $(dir $<)

  endif
endif
endif

# ------------------------------------------------------------------------- #
# Function: CreateToolPrerequisiteRule
# ------------------------------------------------------------------------- #
# This function defines a rule for a jzIntv tool to be copied to the
# INTV.jzIntv/tools directory. It requires the following arguments:
#   $(1) : the tool whose copy-to-INTV.jzIntv rule is to be declared
# ------------------------------------------------------------------------- #
define CreateToolPrerequisiteRule
$(addprefix $(TOOL_OUTPUT_DIR)/,$(1)): $(addprefix $(TOOL_INPUT_DIR)/,$(1))
	@echo Updating $(1) ...
	@echo Copying $$^ to $$@
	@mkdir -p $$(dir $$@)
	@cp -fp $$^ $$@
	@echo

endef

# ------------------------------------------------------------------------- #
# Declare the targets for the jzIntv tools to be built and put into the
# appropriate tools directory.
# ------------------------------------------------------------------------- #
$(foreach tool,$(JZINTV_APPS),$(eval $(call CreateToolPrerequisiteRule,$(tool))))

# ------------------------------------------------------------------------- #
# Function: CreateBuildToolRule
# ------------------------------------------------------------------------- #
# This function defines a rule for a specific jzIntv build target that acts
# as the prerequisite for the tool to copy into the appropriate INTV.jzIntv
# tools directory.
#
# If SYNC_JZINTV is defined, the jzIntv source is synchronized first.
#
# If pulling from a distribution, the prerequisite rule will fetch the
# distribution as needed.
#
# This function requires the following arguments:
#   $(1) : the jzIntv tool to be built
# ------------------------------------------------------------------------- #
define CreateBuildToolRule
$(addprefix $(TOOL_INPUT_DIR)/,$(1)): $(TOOL_INPUT_PREREQ)
ifneq (BIN,$(JZINTV_DIST_TYPE))
ifneq (,$(SYNC_JZINTV))
	@echo Syncing $(JZINTV_DIR) ...
	$(SYNC_JZINTV)
endif
	@echo Building $(1) ...
	make -C $(JZINTV_SRC) -f $$(notdir $$^) ../bin/$(1) $(CUSTOM_BUILD_FLAGS)
	strip $$@
	@echo
else
	@echo Extracting $$@ from $$<
	@unzip -qquo $$< $(JZINTV_UNZIP_TARGET_DIR)/bin/$(1) -d $$(dir $$<)
endif

endef

# ------------------------------------------------------------------------- #
# Declare the targets to be built by the jzIntv build or extracted from dist.
# ------------------------------------------------------------------------- #
$(foreach tool,$(JZINTV_APPS),$(eval $(call CreateBuildToolRule,$(tool))))

# ------------------------------------------------------------------------- #
# Rule: clean
# ------------------------------------------------------------------------- #
# Clean the output of the jzIntv build. NUKE THE MOON!
# ------------------------------------------------------------------------- #
clean:
	@echo Cleaning SDK-1600 tools ...
ifneq (BIN,$(JZINTV_DIST_TYPE))
ifneq ("$(wildcard $(JZINTV_SRC)/$(TARGET_MAKEFILE))","")
	make -C $(JZINTV_SRC) -f $(TARGET_MAKEFILE) clean
endif
endif
ifeq (11,$(JZINTV_CLEAN_DIST)$(USING_JZINTV_DIST))
	@echo Removing $(notdir $(JZINTV_DOWNLOAD_TARGET)) in $(dir $(JZINTV_DOWNLOAD_TARGET))
	@rm -f $(JZINTV_DOWNLOAD_TARGET)
	@echo Removing SDK-1600 tools previously extracted from $(notdir $(JZINTV_DOWNLOAD_TARGET)) ...
	@rm -f $(addprefix $(TOOL_INPUT_DIR)/, $(JZINTV_APPS))
	@rm -frd $(JZINTV_DIR)/$(JZINTV_UNZIP_TARGET_DIR)
endif
	@echo Cleaning SDK-1600 tools from $(CURDIR)/$(TOOL_OUTPUT_DIR) ...
	@rm -f $(addprefix $(TOOL_OUTPUT_DIR)/, $(JZINTV_APPS))
	@echo

else
# ------------------------------------------------------------------------- #
# Rule: all
# ------------------------------------------------------------------------- #
# No-op the build.
# ------------------------------------------------------------------------- #
all:
	@echo build_tools.mak: Skipped SDK-1600 Tools build.

# ------------------------------------------------------------------------- #
# Rule: clean:
# ------------------------------------------------------------------------- #
# No-op the clean.
# ------------------------------------------------------------------------- #
clean:
	@echo build_tools.mak: Skipped SDK-1600 Tools clean.

endif

# ------------------------------------------------------------------------- #
# Get the rule to generate custom_jzIntv.mak if needed.
# ------------------------------------------------------------------------- #
include $(ROOT_DIR)/custom_jzIntv_mak_rule.mak
